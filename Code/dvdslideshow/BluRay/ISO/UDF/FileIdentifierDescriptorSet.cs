using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    /// <summary>
    /// This class represents a set of UDF FileIdentifierDescriptors (FIDS)
    /// Basically this represents the table contents of a folder (i.e. pointers to files and subfolders)
    /// </summary>
    public class FileIdentifierDescriptorSet : UDFFileSystemEntry
    {
        //
        // Used for debug purposes
        //
        private static int maxFidSetSize = 0;

        private List<FileIdentifierDescriptor> FIDEntries = new List<FileIdentifierDescriptor>();
        private string ParentName;
        private int SizeInBytes = 0;

        /// <summary>
        /// Get the FID entries
        /// </summary>
        public List<FileIdentifierDescriptor> Entries
        {
            get { return FIDEntries; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="address"></param>
        /// <param name="subEntires"></param>
        /// <param name="parentAddress"></param>
        /// <param name="parentFSE"></param>
        public FileIdentifierDescriptorSet(ShortAd address, List<ExtendedFileEntry> subEntires, LongAd parentAddress, FileSystemEntry parentFSE) :
            base(address)
        {
            ParentName = parentFSE.Name;

            //
            // First entry is pointer to parent
            //
            FileIdentifierDescriptor pointerToParentEntry = new FileIdentifierDescriptor(new FileSystemEntry("..", true, parentFSE.TimeStamp, ""), parentAddress, null);
            FIDEntries.Add(pointerToParentEntry);

            foreach (ExtendedFileEntry entry in subEntires)
            {
                LongAd entryAddress = new LongAd(entry.StoredAddress.TotalLength, new LbAddr(entry.StoredAddress.SectorLocation, 1));
                FileIdentifierDescriptor fidEntry = new FileIdentifierDescriptor(entry.FileSystemEntry,entryAddress, entry);
                FIDEntries.Add(fidEntry);
            }
        }

        /// <summary>
        /// Get the size of this FIDS in bytes
        /// </summary>
        /// <returns></returns>
        public int GetSizeInBytes()
        {
            if (SizeInBytes == 0)
            {
                foreach (FileIdentifierDescriptor fidEntry in FIDEntries)
                {
                    SizeInBytes += fidEntry.GetLengthInBytes();
                }
            }

            return SizeInBytes;
        }

        /// <summary>
        /// Write the dstring to the given binary writer
        /// </summary>
        /// <param name="writer"></param>
        public override void Write(BinaryWriter writer)
        {
            UDFLog.Info("Writing FIDS ");
            OutputAddress();
            UDFLog.Info(" (" + ParentName + ")");

            VerifyPosition(writer);

            //
            // write out fid entries
            //
            foreach (FileIdentifierDescriptor fidEntry in FIDEntries)
            {
                fidEntry.PartitionOffset = PartitionOffset;
                fidEntry.Write(writer);
            }

            int size = GetSizeInBytes();

            UDFLog.Info(" FidsLE= " + size);

            //
            // Write out rest of sector
            //
            int remainder = size %2048;
            if (remainder != 0)
            {
                byte[] remainderBytes = new byte[2048 - remainder];
                writer.Write(remainderBytes);
            }

            if (maxFidSetSize < size)
            {
                maxFidSetSize = size;
                UDFLog.Info("MAX FIDS size = " + size);
            }

            //
            // Write out sub folders as Extended File entries
            //
            foreach (FileIdentifierDescriptor fidEntry in FIDEntries)
            {
                ExtendedFileEntry subEntry = fidEntry.SubEntry;
                if (subEntry != null)
                {
                    subEntry.PartitionOffset = PartitionOffset;
                    subEntry.Write(writer);
                }
            }

            //
            // Now write out links
            //
            foreach (FileIdentifierDescriptor fidEntry in FIDEntries)
            {
                ExtendedFileEntry subEntry = fidEntry.SubEntry;
                if (subEntry != null)
                {
                    if (subEntry.LinkedEntry != null)
                    {
                        subEntry.LinkedEntry.PartitionOffset = PartitionOffset;
                        subEntry.LinkedEntry.Write(writer);
                    }
                }
            }
        }
    }
}
