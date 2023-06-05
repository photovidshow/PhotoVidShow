using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    /*  Example Blu-ray Disc file entry system 
             
    (#) numbers are sector numbers the entry is located in.
    efse = extended file set entity
    fid = file identider descritpor
              
    BDMV ( efse 4)
        .. (fid 6)
        STREAM(efse 7)
             .. (fid 17)
             00000.m2ts (fid 18)     (file at 98)     	
             00000.m2ts (fid 19)     (file at 10463)
             00000.m2ts (fid 20)     (file at 15656)
        AUXDATA( efse 8)
             .. (fid 21)
             sound.bdmv (fid 22)     (file at 49934)
        BACKUP(efse 9)
             ..   (fid 23)
             BDJO (efse 24)
                     .. (fid 28)
                     00000.bdjo (fid 29)   (file at  50017)
             CLIPINF (efse 25)
                     .. (fid 30)
             JAR 	(26)
         PLAYLIST (27)
        BDJO (10)
             ..   (41)
             00000.bdjo (42)
        CLIPINF (11)
             ..  (43)
             00000.clpi (44)
         00001.clpi (45)
             00002.clpi (46)

        index.bdmv (12)
        JAR (13)
        META (14)
        MovieObecjt.bdmv (15)
        PLAYLIST (16)
    CERTIFICATE (5)
                   
    The partition descriptor (PD) will point to the top efse entry.        
    efse -> fsd -> efse "root" -> fid -> efse "BDMV" -> fid -> *efse 
                                      \> efse "CERIFICATE" -> fid -> *efse

    */

    /// <summary>
    /// This class contains a complete UDF file system tree. I.e. It contains the top extended file system entry (efse)
    /// which then points to child entries.
    /// This class will genertae the UDF file system from a given "root" folder on the local hard-drive
    /// </summary>
    public class FileSystem
    {
        //
        // Stored in ExtendedFileEntries each time an entry is added.  we start at '16' (OSTA 3.2.1.1) and incremement by 1 each time
        //
        private uint nextUnqiueId = 16;

        //
        // Used to keep track of number of files and directories there are in the file system
        //
        private uint numberOfFiles = 0;
        private uint numberOfDirectories = 0;

        //
        // Sector number of the metadara mirror file location 
        //
        private uint metadataMirrorFileLocation=0;

        //
        // Length in sectors of this entire file system (i.e. also size of the partition)
        //
        private uint length = 0;

        // 
        // Top file entry which references the FSD
        //
        private ExtendedFileEntry topFileEntry;

        //
        //  If set to true the STREAM folder on a blu-ray disc is set as the first entry; else the disk is ordered alphabetically
        //
        private bool streamFolderFirst;

        /// <summary>
        /// Get the metedata mirror location
        /// </summary>
        public uint MetadataMirrorFileLocation
        {
            get { return metadataMirrorFileLocation; }
        }

        /// <summary>
        /// Get length in sectors of the complete file system
        /// </summary>
        public uint LengthInSectors
        {
            get { return length; }
        }
      
        /// <summary>
        /// Returns the top extended file entry
        /// </summary>
        public ExtendedFileEntry TopFileEntry
        {
            get { return topFileEntry; }
        }

        /// <summary>
        /// Returns the total number of files in the file system.
        /// </summary>
        public uint NumberOfFiles
        {
            get { return numberOfFiles; }
        }

        /// <summary>
        /// Returns the total number of directories in the file system.
        /// </summary>
        public uint NumberOfDirectories
        {
            get { return numberOfDirectories; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rootFolder"></param>
        /// <param name="volumeIdentifier"></param>
        /// <param name="applicationName"></param>
        /// <param name="streamFolderFirst"></param>
        public FileSystem(string rootFolder, string volumeIdentifier, string applicationName, bool streamFolderFirst)
        {
            this.streamFolderFirst = streamFolderFirst;
            numberOfDirectories = 1;    // starts at 1 (i.e. already include "root" folder)
            numberOfFiles = 0;

            List<ExtendedFileEntry> foundFiles = new List<ExtendedFileEntry>();
            uint sector = 3;

            FileSystemEntry rootFSE = new FileSystemEntry("root", true, IsoUdfGenerator.GetTimeStamp(), "");
            rootFSE.SubFolderCount = Directory.GetDirectories(rootFolder).Length;

            //
            // First calculate the size of the FIDS we will need for root
            //
            uint sectorsRequired = CalcNumberOfSectorsNeededForFIDS(rootFolder, applicationName);
            sector += sectorsRequired;

            ExtendedFileEntry root = new ExtendedFileEntry(rootFSE, new ShortAd(2, 2048), ICBTag.ICBTagType.Directory, applicationName); 
            List<ExtendedFileEntry> extendedFileEntryList = GetDirectoryEntires(rootFolder, root, foundFiles, applicationName, ref sector);
            FileIdentifierDescriptorSet fids = new FileIdentifierDescriptorSet(new ShortAd(3, 2048 * sectorsRequired), extendedFileEntryList, new LongAd(2048, new LbAddr(2, 1)), rootFSE);
            fids.PartitionOffset = 288 + 32;

            root.LinkedEntry = fids;
            root.PartitionOffset = 288 + 32;
            List<AllocationDescriptor> rootAdSet = new List<AllocationDescriptor>();
            rootAdSet.Add( new ShortAd(fids.StoredAddress.SectorLocation, (uint)fids.GetSizeInBytes()));
            root.AllocationDescriptorSet = rootAdSet;

            FileSetDescriptor fsd = new FileSetDescriptor(new ShortAd(0, 512), volumeIdentifier);
            fsd.PartitionOffset = 288 + 32;
            fsd.LinkedEntry = root;

            //
            // First entry at 0x90000 points to FSD at sector 32 (0xA0000)
            //
            topFileEntry = new ExtendedFileEntry(new FileSystemEntry("", true, IsoUdfGenerator.GetTimeStamp(), ""), new ShortAd(0, 0), ICBTag.ICBTagType.MetaData, applicationName);
            topFileEntry.LinkedEntry = fsd;
            topFileEntry.PartitionOffset = 288;
 
            // 
            // 'sector' should now be the last sector after the file tree, here we align in to next 32 sector, and start assigning the Allocation descriptors to the files
            // which will be places here
            //
            uint sizeOfFileTree = sector;
            uint nextFileLocation = sector;
            nextFileLocation = AlignTo32Sector(nextFileLocation);

            // 
            // Ok calc size of file table size; NOTE will also be smaller than a uint
            //
            uint fsTableSize = ((32 + 288 + nextFileLocation) * 2048) - 0xA0000;    
            List<AllocationDescriptor> topFileEntryAdSet = new List<AllocationDescriptor>();
            topFileEntryAdSet.Add(new ShortAd(32, fsTableSize));
            topFileEntry.AllocationDescriptorSet = topFileEntryAdSet;

            nextFileLocation += 32;
            nextFileLocation = SetFilesAllocationDescriptors(foundFiles, nextFileLocation);

            metadataMirrorFileLocation = nextFileLocation;
            metadataMirrorFileLocation = AlignTo32Sector(metadataMirrorFileLocation);

            length = metadataMirrorFileLocation + sizeOfFileTree;
            length = AlignTo32Sector(length);
            length += 32;
        }

        /// <summary>
        /// Recusuve method to genenate UDF entries from a folder on the local-hard drive
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="numOfDirectories"></param>
        /// <param name="numOfFiles"></param>
        /// <returns></returns>
        private List<FileSystemEntry> GetSubFolderAndFilesForFolder(string folder, ref uint numOfDirectories, ref uint numOfFiles)
        {
            List<FileSystemEntry> toPass = new List<FileSystemEntry>();

            string[] entries = Directory.GetDirectories(folder);
            foreach (string entry in entries)
            {
                string directoryName = entry.Remove(0, entry.LastIndexOf('\\') + 1);

                System.IO.DirectoryInfo di = new DirectoryInfo(entry);

                //  FileSystemEntry fsEntry = new FileSystemEntry(directoryName, true, di.CreationTime, folder);
                FileSystemEntry fsEntry = new FileSystemEntry(directoryName, true, IsoUdfGenerator.GetTimeStamp1(di.CreationTime), folder);

                //
                // Calc sub folder count
                //
                string[] subEntries = Directory.GetDirectories(entry);
                fsEntry.SubFolderCount = subEntries.Length;

                toPass.Add(fsEntry);
                numOfDirectories++;
            }

            entries = Directory.GetFiles(folder);
            foreach (string entry in entries)
            {
                FileInfo fileInfo = new FileInfo(entry);
                if (fileInfo.Exists == true && ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden))
                {
                    continue;
                }

                // FileSystemEntry fsEntry = new FileSystemEntry(System.IO.Path.GetFileName(entry), false, fileInfo.CreationTime, folder);
                FileSystemEntry fsEntry = new FileSystemEntry(System.IO.Path.GetFileName(entry), false, IsoUdfGenerator.GetTimeStamp1(fileInfo.CreationTime), folder);

                toPass.Add(fsEntry);
                numOfFiles++;
            }

            toPass.Sort();

            if (streamFolderFirst == true)
            {
                MoveStreamToStart(toPass);
            }

            return toPass;
        }


        /// <summary>
        /// Private method to generate UDF Extended File Entries from a local hard-dive FileSystemEntry list
        /// </summary>
        /// <param name="toPass"></param>
        /// <param name="foundFiles"></param>
        /// <param name="applicationName"></param>
        /// <param name="sector"></param>
        /// <param name="nextID"></param>
        /// <returns></returns>
        private List<ExtendedFileEntry> GenerateExtendedFileEntries(List<FileSystemEntry> toPass, List<ExtendedFileEntry> foundFiles, string applicationName, ref uint sector, ref uint nextID)
        {   
            List<ExtendedFileEntry> extendedFileEntryList = new List<ExtendedFileEntry>();

            //
            // First create all ETFS in folder
            //
            foreach (FileSystemEntry toPassEntry in toPass)
            {
                ICBTag.ICBTagType type = ICBTag.ICBTagType.Directory;
                if (toPassEntry.Folder == false)
                {
                    type = ICBTag.ICBTagType.File;
                }

                ExtendedFileEntry efs = new ExtendedFileEntry(toPassEntry, new ShortAd(sector, 2048), type, applicationName);
                efs.UniqieID = nextID++;
                efs.PartitionOffset = 288 + 32;
                extendedFileEntryList.Add(efs);
                sector++;

                if (type == ICBTag.ICBTagType.File && foundFiles !=null)
                {
                    // 
                    // Store file for now. We will give it a allocation descriptor later
                    //
                    foundFiles.Add(efs);

                }
            }

            return extendedFileEntryList;
        }

        /// <summary>
        /// Private method used to calculate the size of a FIDS, which is needed before actually creating it.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        private uint CalcNumberOfSectorsNeededForFIDS(string folder, string applicationName)
        {
            uint numberOfDirectoiresTemp = 0;
            uint numberOfFilesTemp = 0;
            uint nextUniqueIdTemp = 0;
            uint sector = 0;
            FileSystemEntry parentFSE = new FileSystemEntry();
            LongAd parentAddress = new LongAd(0, new LbAddr(0,0));

            List<FileSystemEntry> toPass = GetSubFolderAndFilesForFolder(folder, ref numberOfDirectoiresTemp, ref numberOfFilesTemp);
            List<ExtendedFileEntry> extendedFileEntryListSub = GenerateExtendedFileEntries(toPass, null, applicationName, ref sector, ref nextUniqueIdTemp);

            ShortAd fidAddr = new ShortAd(0,0);
            FileIdentifierDescriptorSet fidsSub = new FileIdentifierDescriptorSet(fidAddr, extendedFileEntryListSub, parentAddress, parentFSE);

            int fidsSubSize = fidsSub.GetSizeInBytes();
            int sectorsRequired = fidsSubSize / 2048;
            if ((fidsSubSize % 2048) != 0)
            {
                sectorsRequired++;
            }

            return (uint)sectorsRequired;
        }

        /// <summary>
        /// Private method to generate an UDF file system tree from a "root" folder from the local hard-drive.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="parentEfe"></param>
        /// <param name="foundFiles"></param>
        /// <param name="applicationName"></param>
        /// <param name="sector"></param>
        /// <returns></returns>
        private List<ExtendedFileEntry> GetDirectoryEntires(string folder, ExtendedFileEntry parentEfe, List<ExtendedFileEntry> foundFiles, string applicationName, ref uint sector)
        {
            //
            // Generate sub folders and files in folder
            //
            List<FileSystemEntry> toPass = GetSubFolderAndFilesForFolder(folder, ref numberOfDirectories, ref numberOfFiles);

            //
            // Create output UDF ExtendedFileEntry list
            //
            List<ExtendedFileEntry> extendedFileEntryList = GenerateExtendedFileEntries(toPass, foundFiles, applicationName, ref sector, ref nextUnqiueId);

            //
            // Create FIDS and sub ETFS for all sub folders
            //
            foreach (ExtendedFileEntry efs in extendedFileEntryList)
            {
                if (efs.FileSystemEntry.Folder == true)
                {

                    LongAd parentAddress = new LongAd(parentEfe.StoredAddress.TotalLength, new LbAddr(parentEfe.StoredAddress.SectorLocation, 1));
                    parentAddress.UniqueID = (uint)parentEfe.UniqieID;
                    string subFolderName = folder + "\\" + efs.FileSystemEntry.Name;
                    uint FIDSsector = sector;

                    //
                    // First calculate the size of the FIDS we will need for the sub folder
                    //
                    uint sectorsRequired = CalcNumberOfSectorsNeededForFIDS(subFolderName, applicationName);

                    //
                    // Increase sector by this amount
                    //
                    sector += sectorsRequired;
                        
                    //
                    // Now generate subEntries file entries
                    //
                    List<ExtendedFileEntry> subEntries = GetDirectoryEntires(subFolderName, efs, foundFiles, applicationName, ref sector);

                    //
                    // Now create FIDS
                    //
                    ShortAd fidAddr = new ShortAd(FIDSsector, 2048 * sectorsRequired);
                    FileIdentifierDescriptorSet fids = new FileIdentifierDescriptorSet(fidAddr, subEntries, parentAddress, parentEfe.FileSystemEntry);
                    efs.LinkedEntry = fids;
                    efs.LinkedEntry.PartitionOffset = 288 + 32;

                    uint size = (uint)fids.GetSizeInBytes();

                    List<AllocationDescriptor> efsAdSet = new List<AllocationDescriptor>();
                    efsAdSet.Add(new ShortAd(fidAddr.SectorLocation, (uint)fids.GetSizeInBytes()));

                    efs.AllocationDescriptorSet = efsAdSet;
                }
            }

            return extendedFileEntryList;
        }

        /// <summary>
        /// Private method to set the allocation descriptors in the UDF (EFSEs).  This is done after
        /// creating the UDF tree.
        /// </summary>
        /// <param name="files"></param>
        /// <param name="nextFileLocation"></param>
        /// <returns></returns>
        private uint SetFilesAllocationDescriptors(List<ExtendedFileEntry> files, uint nextFileLocation)
        {
            foreach (ExtendedFileEntry file in files)
            {
                FileInfo info = new FileInfo(file.FileSystemEntry.Directory + "\\" + file.FileSystemEntry.Name);
                ulong remaning = (ulong)info.Length;

                List<AllocationDescriptor> fileAdSet = new List<AllocationDescriptor>();
                ulong maxAdSize = (1024 * 1024 * 1024) - (2048);

                // 
                // Split file over multiple allocator descriptors (each one 1gb - 1 sector/2048 bytes)
                //
                while (remaning > 0)
                {
                    ulong nextChunkSize = remaning;
                    if (nextChunkSize > maxAdSize)
                    {
                        nextChunkSize = maxAdSize;
                    }

                    fileAdSet.Add(new LongAd((uint)nextChunkSize, new LbAddr(nextFileLocation, 0)));
                    remaning -= nextChunkSize;
                    if (remaning > 0)
                    {
                        nextFileLocation += (uint)(nextChunkSize / 2048);
                    }
                    else
                    {
                        //
                        // Align to next sector at end of file
                        //
                        uint sectorSize = (uint)(nextChunkSize / 2048);
                        if ((nextChunkSize % 2048) != 0)
                        {
                            sectorSize++;
                        }
                        nextFileLocation += sectorSize;
                    }  
                }

                file.AllocationDescriptorSet = fileAdSet;
            }

            return nextFileLocation;
        }

        /// <summary>
        /// Private method to set the "STREAM" folder on a blu-ray disc as the first
        /// entry in the FIDS (else it is done alphabetically)
        /// This is for debug purposes to replicate with known blu-ray ISOs
        /// </summary>
        /// <param name="entries"></param>
        private void MoveStreamToStart(List<FileSystemEntry> entries)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].Name == "STREAM")
                {
                    FileSystemEntry stream = entries[i];
                    for (int j = i; j > 0; j--)
                    {
                        entries[j] = entries[j - 1];
                    }
                    entries[0] = stream;
                    break;
                }
            }
        }

        /// <summary>
        /// Private method to align to the next 32 sector
        /// </summary>
        /// <param name="sector"></param>
        /// <returns></returns>
        private uint AlignTo32Sector(uint sector)
        {
            if ((sector % 32) != 0)
            {
                sector += (32 - (sector % 32));
            }
            return sector;
        }
    }
}
