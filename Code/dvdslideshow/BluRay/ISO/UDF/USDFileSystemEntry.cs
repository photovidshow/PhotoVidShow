using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    public abstract class UDFFileSystemEntry
    {
        private ShortAd Address;

        public ShortAd StoredAddress 
        {
            get { return Address; }
            set { Address = value; }
        }

        protected uint PartitionOff;

        public uint PartitionOffset
        {
            get { return PartitionOff; }
            set { PartitionOff = value; }
        }

        public void OutputAddress()
        {
            ulong hexAddress = ((ulong)(PartitionOff + StoredAddress.SectorLocation)) * 2048;

            UDFLog.Info("Location: " + StoredAddress.SectorLocation +
                        " Length: " + StoredAddress.TotalLength +
                        " Hex: " + hexAddress.ToString("x"));
        }
                                                  
        public abstract void Write(BinaryWriter writer);

        
        public void PadAndWrite(BinaryWriter writer)
        {
            ulong currentWriterPosition = (ulong)writer.BaseStream.Position;

            //
            // Write out blank until reach next entry
            //          
            ulong nextPosition = (((ulong)(PartitionOffset + StoredAddress.SectorLocation))) * 2048;

            ulong toWrite = 0;
            if (currentWriterPosition < nextPosition)
            {
                toWrite = nextPosition - currentWriterPosition;
            }
            else if (currentWriterPosition > nextPosition)
            {
                int cockup = 1;
                cockup++;
            }

            if (toWrite >= 2048)
            {
                byte[] blankSector = new byte[2048];
                while (toWrite >= 2048)
                {
                    writer.Write(blankSector);
                    toWrite -= 2048;
                }
            }

            if (toWrite > 0)
            {
                byte[] remainder = new byte[toWrite];
                writer.Write(remainder);
                toWrite = 0;
            }

            Write(writer);
        }


        public UDFFileSystemEntry(ShortAd address)
        {
            Address = address;
        }

        public UDFFileSystemEntry()
        {
        }

        protected void VerifyPosition(BinaryWriter writer)
        {
            ulong assumePosition = (((ulong)(PartitionOff + StoredAddress.SectorLocation))) * 2048;
            ulong actualPosition = (ulong)writer.BaseStream.Position;

            if (assumePosition != actualPosition)
            {
                UDFLog.Error("Writting FS entry is wrong position " + assumePosition + " " + actualPosition);
            }
        }
    }
}
