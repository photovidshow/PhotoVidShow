using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    /// <summary>
    /// This class represents a UDF Extended File Entry
    /// </summary>
    public class ExtendedFileEntry : USDLinkedEntry, IDescriptor
    {
        //
        // Ecma 14.17
        //
        private Tag Description;
        private ICBTag ICB;
        private uint UID;
        private uint GID;
        private uint Permissions;
        private ushort FileLinkCount;
        private byte RecordFormat;
        private byte RecordDisplayAttributes;
        private uint RecordLength;

        //
        // The file size in bytes. This shall be equal to the sum of the Information Lengths of 
        // the allocation descriptors for the body of the file.
        //
        private ulong InformationLength;
        
        //
        // Same as the above if no streams
        //
        private ulong ObjectSize;

        private ulong LogicalBlocksRecorded;
        private TimeStamp AccessDateAndTime;
        private TimeStamp ModificationDateAndTime;
        private TimeStamp CreationDateAndTime;
        private TimeStamp AttributeDataAndTime;

        //
        // This field shall contain 1 for the first instance of a file and shall be incremented by 1
        // when directed to do so by the user
        //
        private uint Checkpoint;
        private uint Reserved;
        private LongAd ExtendedAttributeICB;
        private LongAd StreamDirectoryICB;
        private EntityID ImplementationIdentifier;
        private long UniqieId;
        private uint LengthOfExtendedAttributes;    // L_EA
        private uint LengthOfAllocationDescriptors; // L_AD

        // Copied from proshow iso (Only used in debug)
        private byte[] ExtendedAttributes = { 0x06,0x01,0x03,0x00,0x70,0x00,0x01,0x00,
                                              0xa1,0xba,0x08,0x00,0x02,0x00,0x00,0x00,
                                              0x18,0x00,0x00,0x00,0xff,0xff,0xff,0xff,
                                              0x00,0x08,0x00,0x00,0x01,0x00,0x00,0x00,
                                              0x34,0x00,0x00,0x00,0x04,0x00,0x00,0x00,
                                              0x00,0x2a,0x55,0x44,0x46,0x20,0x46,0x72,
                                              0x65,0x65,0x45,0x41,0x53,0x70,0x61,0x63,
                                              0x65,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                                              0x50,0x02,0x00,0x00,0x00,0x00,0x00,0x00, 
                                              0xb0,0x05,0x00,0x00,0x00,0x08,0x00,0x00,
                                              0x01,0x00,0x00,0x00,0x38,0x00,0x00,0x00,
                                              0x08,0x00,0x00,0x00,0x00,0x2a,0x55,0x44,
                                              0x46,0x20,0x44,0x56,0x44,0x20,0x43,0x47,
                                              0x4d,0x53,0x20,0x49,0x6e,0x66,0x6f,0x00,
                                              0x00,0x00,0x00,0x00,0x50,0x02,0x00,0x00, 
                                              0x00,0x00,0x00,0x00,0x98,0x05,0x00,0x00,
                                              0x00,0x00,0x00,0x00};

        private List<AllocationDescriptor> AllocationDescriptors;

        //
        // The file on the local hard-drive.  Needed when creating
        //
        private FileSystemEntry FSE;

        /// <summary>
        /// Get the ICBtag
        /// </summary>
        public ICBTag Icb
        {
            get { return ICB; }
        }

        public List<AllocationDescriptor> AllocationDescriptorSet
        {

            set
            {
                AllocationDescriptors = value;
                if (value == null)
                {
                    LengthOfAllocationDescriptors = 0;
                    SetInformationLength(0);
                }
                else
                {
                    LengthOfAllocationDescriptors = 0;
                    ulong totalLength = 0;

                    foreach (AllocationDescriptor ad in AllocationDescriptors)
                    {
                        totalLength += ad.TotalLength;
                        LengthOfAllocationDescriptors += ad.AllocationDescriptorSizeInBytes;
                    }

                    SetInformationLength(totalLength);
                }
            }
            get
            {
                return AllocationDescriptors;
            }
        }

        /// <summary>
        /// Get ot set UniqieID
        /// </summary>
        public long UniqieID
        {
            get { return UniqieId; }
            set { UniqieId = value; }
        }

        /// <summary>
        /// Gets the local file system entry
        /// </summary>
        public FileSystemEntry FileSystemEntry
        {
            get { return FSE; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fse"></param>
        /// <param name="address"></param>
        /// <param name="linkedItem"></param>
        /// <param name="type"></param>
        /// <param name="applicationName"></param>
        public ExtendedFileEntry(FileSystemEntry fse, ShortAd address, ICBTag.ICBTagType type, string applicationName) :
            base(address)
        {
            FSE = fse;
            Initialise(type, applicationName);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fse"></param>
        /// <param name="address"></param>
        /// <param name="linkedItem"></param>
        /// <param name="type"></param>
        /// <param name="applicationName"></param>
        public ExtendedFileEntry(FileSystemEntry fse, ShortAd address, UDFFileSystemEntry linkedItem, ICBTag.ICBTagType type, string applicationName) :
            base(address)
        {
            FSE = fse;
            base.LinkedEntry = linkedItem;
            Initialise(type, applicationName);
        }

        /// <summary>
        /// Private initialiser used by the constructors
        /// </summary>
        /// <param name="linkedItem"></param>
        /// <param name="type"></param>
        /// <param name="applicationName"></param>
        private void Initialise(ICBTag.ICBTagType type, string applicationName)
        {
            Description = new Tag(Tag.ExtendedFileEntryTag);

            ICB = new ICBTag(type);
            UID = 0xffffffff;
            GID = 0xffffffff;
            Permissions = 0x14a5;   // 1010010100101

            if (type == ICBTag.ICBTagType.Directory)
            {
                FileLinkCount = (ushort)(FSE.SubFolderCount+1);  // number of times entry is mentioned. For root this is 3, for BDMV this is 9 (i.e. itself + nummber of child directories)
            }
            else if (type == ICBTag.ICBTagType.File)
            {
                FileLinkCount = 1;
            }

            RecordFormat = 0;
            RecordDisplayAttributes = 0;
            RecordLength = 0;

          //  SetInformationLength(linkedItem.TotalLength);
            
            DateTime datetime = FSE.TimeStamp;

            AccessDateAndTime = new TimeStamp(IsoUdfGenerator.GetAccessTime(datetime, FSE.Name));
            ModificationDateAndTime = new TimeStamp(datetime);
            CreationDateAndTime = new TimeStamp(IsoUdfGenerator.GetCreatingTime(datetime, FSE.Name));
            AttributeDataAndTime = new TimeStamp(datetime);

            Checkpoint = 1;
            Reserved = 0;

            ExtendedAttributeICB = new LongAd(0, new LbAddr(0, 0));
            StreamDirectoryICB = new LongAd(0, new LbAddr(0, 0));

            ImplementationIdentifier = new EntityID("*" + applicationName, 0, false);

            UniqieId = 0; // 64 bit
            LengthOfExtendedAttributes = 0;

            //
            // Write out extended file attributes
            // Extended attributes are only used in test cases (to compare with proshow iso)
            //
            if ((type == ICBTag.ICBTagType.Directory || type == ICBTag.ICBTagType.File) && IsoUdfGenerator.GenerateExtendedAttributes == true)
            {
                LengthOfExtendedAttributes=132;

                ExtendedAttributes[4] += (byte) (this.StoredAddress.SectorLocation -2);
                ExtendedAttributes[12] += (byte)(this.StoredAddress.SectorLocation - 2);
            }
         
            LengthOfAllocationDescriptors = 0;
        }

        /// <summary>
        /// Private method to set both the InformationLength and ObjectSize
        /// </summary>
        /// <param name="length"></param>
        private void SetInformationLength(ulong length)
        {
            InformationLength = length;   // 64 
            ObjectSize = InformationLength;           // 64 bit

            if ((InformationLength % 2048) == 0)
            {
                LogicalBlocksRecorded = InformationLength / 2048;
            }
            else
            {
                LogicalBlocksRecorded = (InformationLength / 2048) + 1;
            }
        }

        /// <summary>
        /// Writes the ExtendedFileEntry to the given binary writer
        /// </summary>
        /// <param name="writer"></param>
        public override void Write(BinaryWriter writer)
        {
            if (writer.BaseStream is FileStream)
            {
                Description.CalcDescriptorCRC(this);
                UDFLog.Info("Writing EFSE ");
                OutputAddress();
                UDFLog.Info(" (" + FSE.Name + ")");
                VerifyPosition(writer);
            }

            Description.Write(writer, StoredAddress);
            ICB.Write(writer);
            writer.Write(UID);
            writer.Write(GID);
            writer.Write(Permissions);
            writer.Write(FileLinkCount);
            writer.Write(RecordFormat);
            writer.Write(RecordDisplayAttributes);
            writer.Write(RecordLength);
            writer.Write(InformationLength);
            writer.Write(ObjectSize);
            writer.Write(LogicalBlocksRecorded);
            AccessDateAndTime.Write(writer);
            ModificationDateAndTime.Write(writer);
            CreationDateAndTime.Write(writer);
            AttributeDataAndTime.Write(writer);
            writer.Write(Checkpoint);
            writer.Write(Reserved);
            ExtendedAttributeICB.Write(writer);
            StreamDirectoryICB.Write(writer);
            ImplementationIdentifier.Write(writer);
            writer.Write(UniqieId);
            writer.Write(LengthOfExtendedAttributes);
            writer.Write(LengthOfAllocationDescriptors);

            if (LengthOfExtendedAttributes != 0)
            {
                writer.Write(ExtendedAttributes);
            }

            if (AllocationDescriptors == null || AllocationDescriptors.Count == 0)
            {
                UDFLog.Error("Allocation descriptors not set for " + FSE.Name);
            }
            else
            {
                int count = 1;
                foreach (AllocationDescriptor ad in AllocationDescriptors)
                {
                    UDFLog.Info("Writing allocation descriptor " + count + " sec:" + ad.SectorLocation + " len:" + ad.TotalLength); 
                    ad.Write(writer);
                    count++;
                }
            }
            
            if (writer.BaseStream is FileStream)
            {
                uint sizeOfEntry = 216 + LengthOfExtendedAttributes + LengthOfAllocationDescriptors;

                UDFLog.Info(" Length = " + sizeOfEntry);

                byte[] blank = new byte[2048 - sizeOfEntry];
                writer.Write(blank);
            }
        }

        /// <summary>
        /// If this entry is a folder or a FileSetDescriptor, it recursively writes out the
        /// subfolderss files to the ISO; else it writes the represented local hard drive file 
        /// to the ISO.
        /// </summary>
        /// <param name="partitionStart"></param>
        /// <param name="writer"></param>
        public void WriteFiles(uint partitionStart, BinaryWriter writer)
        {     
            if (FSE.Folder == false)
            {
                long currentPosition = writer.BaseStream.Position;
                if ((currentPosition % 2048) != 0)
                {
                    UDFLog.Error("Not sector aligned when writing UDF2.5 extended file entry");
                }

                currentPosition /= 2048;
                uint currentSector = (uint)currentPosition;

                string filename = FSE.Directory + "\\" + FSE.Name;
                if (File.Exists(filename) == false)
                {
                    UDFLog.Error("Can not find file " + filename);
                }
                //
                // Only file writing to iso really takes any time, so only thing aborting on
                //
                else if (IsoUdfGenerator.AbortCreation == false)
                {
                    uint itemSector = this.AllocationDescriptors[0].SectorLocation + partitionStart;
                    //
                    // Write out any empty sectors;
                    //
                    if (currentSector < itemSector)
                    {
                        uint sectorsToWrite = itemSector - currentSector;
                        UDFLog.Info("Writing " + sectorsToWrite + " blank sectors");
                        byte[] blank = new byte[2048];
                        for (uint i = 0; i < sectorsToWrite; i++)
                        {
                            writer.Write(blank);
                            currentSector++;
                        }
                    }
                    if (itemSector < currentSector)
                    {
                        UDFLog.Error("Item sector less than current sector in USD2.5 extended file entry"); 
                    }

                    UDFLog.Info("Writing into iso '" + FSE.Name + "' at sector:" + currentSector);
                    using (FileStream stream = File.OpenRead(filename))
                    {
                        BinaryReader reader = new BinaryReader(stream);

                        // create a buffer to hold the bytes 
                        byte[] buffer = new Byte[2048];
                        int bytesRead;

                        //
                        // While the read method returns bytes keep writing them to the output stream
                        //
                        while ((bytesRead = stream.Read(buffer, 0, 2048)) > 0 && IsoUdfGenerator.AbortCreation == false)
                        {
                            writer.Write(buffer, 0, bytesRead);

                            //
                            // Pad end of file to end of sector
                            //
                            if (bytesRead < 2048)
                            {
                                Byte[] padding = new byte[2048 - bytesRead];
                                writer.Write(padding);
                            }
                            currentSector++;
                        }
                    }
                }
              
            }
            else if (LinkedEntry != null)
            {
                FileSetDescriptor fsd = LinkedEntry as FileSetDescriptor;
                if (fsd != null)
                {
                    (fsd.LinkedEntry as ExtendedFileEntry).WriteFiles(partitionStart, writer);
                }
                else
                {
                    FileIdentifierDescriptorSet set = LinkedEntry as FileIdentifierDescriptorSet;
                    if (set != null)
                    {
                        List<FileIdentifierDescriptor> fids = set.Entries;

                        //
                        // Write out files first
                        //
                        if (IsoUdfGenerator.AbortCreation == false)
                        {
                            foreach (FileIdentifierDescriptor fid in fids)
                            {
                                if (fid.SubEntry != null && fid.SubEntry.FSE.Folder == false)
                                {
                                    fid.SubEntry.WriteFiles(partitionStart, writer);
                                    if (IsoUdfGenerator.AbortCreation == true)
                                    {
                                        return;
                                    }
                                }
                            }
                        }

                        if (IsoUdfGenerator.AbortCreation == false)
                        {
                            //
                            // Now write out folders
                            //
                            foreach (FileIdentifierDescriptor fid in fids)
                            {
                                if (fid.SubEntry != null && fid.SubEntry.FSE.Folder == true)
                                {
                                    fid.SubEntry.WriteFiles(partitionStart, writer);
                                }
                                if (IsoUdfGenerator.AbortCreation == true)
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
