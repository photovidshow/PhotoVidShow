using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    public class Tag
    {
        //
        // Ecma Tags
        //
        public const int PrimaryVolumeDescriptorTag = 1;
        public const int AnchorVolumeDescriptorPointerTag = 2;
        public const int ImplementationUseVolumeDescriptorTag = 4;
        public const int PartitionDescritorTag = 5;
        public const int LogicalVolumeDescriptorTag = 6;
        public const int UnallocatedSpaceDescriptorTag = 7;
        public const int TerminatingDescriptorTag = 8;
        public const int LogicalVolumeIntegrityDescriptorTag = 9;
        public const int FileSetDescriptorTag = 256;
        public const int FileIdentifierDescriptorTag = 257;
        public const int ExtendedFileEntryTag = 266;

  
        //
        // ECMA 167 4/7.2 
        //
        private ushort tagIdentifier;
        private ushort descriptorVersion;
        private byte tagChecksum;
        private byte reserved;
        private ushort tagSerialNumber;
        private ushort descriptorCRC;
        private ushort descriptorCRCLength;
        private uint tagLocation;

        public Tag(ushort identifier)
        {
            tagIdentifier = identifier;
            descriptorVersion = 3;
            reserved = 0;
            tagSerialNumber = 1;
            descriptorCRC = 0;
            descriptorCRCLength = 0;
            tagLocation = 0; // set at write time
        }

        public void CalcDescriptorCRC(IDescriptor descriptor)
        {
            //
            // No descriptor will be more than a sector
            //
            byte[] buffer = new byte[2048];
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                BinaryWriter writer = new BinaryWriter(stream);
                descriptor.Write(writer);

                if (stream.Position <= 16)
                {
                    UDFLog.Error("Descriptor to short on write");
                    return;
                }
                descriptorCRCLength = (ushort)(stream.Position - 16); // Minus Tag size

                byte[] contents = new byte[descriptorCRCLength];

                System.Buffer.BlockCopy(buffer, 16, contents, 0, descriptorCRCLength);

                descriptorCRC = CRC.CalculateCRC(contents);
            }
        }


        private void GenerateChecksum(byte[] block)
        {
            //
            // This field shall specify the sum modulo 256 of bytes 0-3 and 5-15 of the tag.
            //
            tagChecksum = 0;

            for (int i=0;i<=3;i++)
            {
                tagChecksum+=block[i];
            }

            for (int i=5;i<=15;i++)
            {
                tagChecksum+=block[i];
            }

            //
            // Write result into block also
            //
            block[4] = (byte) (tagChecksum % 256);
        }

        public void Write(BinaryWriter outWriter, ShortAd address)
        {
            tagLocation = address.SectorLocation;
            InternalWrite(outWriter);
        }

        public void Write(BinaryWriter outWriter)
        {
            InternalWrite(outWriter);
        }

        public void Write(BinaryWriter outWriter, uint partitionStart)
        {
            tagLocation = (uint)(outWriter.BaseStream.Position / 2048);

            tagLocation -= partitionStart;

            InternalWrite(outWriter);
        }

        private void InternalWrite(BinaryWriter outWriter)
        {
            using (MemoryStream stream = new MemoryStream(16))
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(tagIdentifier);
                writer.Write(descriptorVersion);
                writer.Write(tagChecksum);
                writer.Write(reserved);
                writer.Write(tagSerialNumber);
                writer.Write(descriptorCRC);
                writer.Write(descriptorCRCLength);
                writer.Write(tagLocation);

                writer.Close();
                stream.Close();

                byte[] block = stream.ToArray();

                GenerateChecksum(block);
                outWriter.Write(block);
            }  
        }
    }
}
