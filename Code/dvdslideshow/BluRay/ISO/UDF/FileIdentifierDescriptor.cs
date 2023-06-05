using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    /// <summary>
    /// This class represents a UDF FileIdentifierDescriptor (FID)
    /// </summary>
    public class FileIdentifierDescriptor : IDescriptor
    {
        private Tag Descriptor;
        private ushort FileVersionNumber;
        private byte FileCharacteristics;
        private byte LengthOfFileIndentifier ; // L_FI
        private LongAd ICB;
        private ushort LengthOfImplementationUse;
        // private byte[] ImplementationUse;  // not used
        private DString FileIdentifier;
        private byte[] padding;
        private ExtendedFileEntry subEntry;
        private int SizeInBytes = 0;

        private uint partitionOffset = 0;

        /// <summary>
        /// Get the sub extended file entry
        /// </summary>
        public ExtendedFileEntry SubEntry
        {
            get { return subEntry; }
        }

        /// <summary>
        /// Get or set the partition offset
        /// </summary>
        public uint PartitionOffset
        {
            get { return partitionOffset; }
            set { partitionOffset = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="address"></param>
        /// <param name="subEntry"></param>
        public FileIdentifierDescriptor(FileSystemEntry entry, LongAd address, ExtendedFileEntry subEntry) 
        {
            this.subEntry = subEntry;

            Descriptor = new Tag(Tag.FileIdentifierDescriptorTag);
            FileVersionNumber = 1;

            // 
            // Is this a link to the parent folder
            //
            if (entry.Name == "..")
            {      
                FileCharacteristics = 10;
                LengthOfFileIndentifier = 0;
            }
            else
            {
                int uniLength = (entry.Name.Length * 2)+1;
                LengthOfFileIndentifier = (byte)uniLength;
                FileIdentifier = new DString(entry.Name, (byte)uniLength+1, false);
                if (entry.Folder == true)
                {
                    //
                    // Pointer to sub directory
                    //
                    FileCharacteristics = 2;                  
                }
                else
                {
                    //
                    // A file pointer
                    //
                    FileCharacteristics = 0;
                }
            }

            ICB = address; 

            if (subEntry != null)
            {
                ICB.UniqueID = (uint)subEntry.UniqieID;
            }

            LengthOfImplementationUse = 0;

            // ImplementationUse not used

            //
            // Padding
            // This field shall be 4 x ip((L_FI+L_IU+38+3)/4) - (L_FI+L_IU+38) bytes long and shall contain all #00 bytes.
            //
            int normalLength = LengthOfFileIndentifier + LengthOfImplementationUse + 38;        
            int padLength = 4 * ((normalLength + 3) / 4) - normalLength;

            if (padLength > 1)
            {
                //
                // This is hack becuase FileIdentifier buffer is actually bigger
                //
                if (padLength % 2 == 1)
                {
                    padLength--;
                }
                padding = new byte[padLength];
            }
            else
            {
                padLength = 0;
            }

            if (FileIdentifier != null)
            {
                SizeInBytes = 38 + FileIdentifier.GetLengthInBytes() + padLength;
            }
            else
            {
                SizeInBytes = 38 + padLength;
            }        
        }

        /// <summary>
        /// Returns the size of this FID in bytes
        /// </summary>
        /// <returns></returns>
        public int GetLengthInBytes()
        {
            return SizeInBytes;
        }

        /// <summary>
        /// Write the FID to the given binary writer
        /// </summary>
        /// <param name="writer"></param>
        public void Write(BinaryWriter writer)
        {
            if (writer.BaseStream is FileStream)
            {
                //
                // Only calculate CRC on writing to file, as this will then recursively call this.Write as a MemoryStream
                //
                Descriptor.CalcDescriptorCRC(this);

                ulong hexAddress = ((ulong)(partitionOffset + ICB.ExtentLocation.LogicalBlockNumber)) * 2048;

                UDFLog.Info("Writing FID");
                UDFLog.Info(" Pointer: " + ICB.ExtentLocation.LogicalBlockNumber +
                            " Length: " + ICB.ExtentLength +
                            " Hex: " + hexAddress.ToString("x"));
                if (subEntry != null)
                {
                    UDFLog.Info(" Pointer to " + subEntry.FileSystemEntry.Name);
                }
                else
                {
                    UDFLog.Info(" ..");
                }
            }

            Descriptor.Write(writer, partitionOffset);
            writer.Write(FileVersionNumber);
            writer.Write(FileCharacteristics);
            writer.Write(LengthOfFileIndentifier);
            ICB.Write(writer);
            writer.Write(LengthOfImplementationUse);

            if (FileIdentifier != null)
            {
                FileIdentifier.Write(writer);
            }

            if (padding != null)
            {
                writer.Write(padding);
            }
        }
    }
}
