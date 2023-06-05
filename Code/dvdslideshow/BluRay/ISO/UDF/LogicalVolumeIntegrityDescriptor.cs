using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    public class LogicalVolumeIntegrityDescriptor : IDescriptor
    {
        private Tag Descriptor;    
        private TimeStamp RecordingDateAndTime;
        private uint IntegrityType;
        private ExtentAD nextIntegrityExtent;
        private byte[] LogicalVolumeContentsUse = new byte[32];
        private uint NumberOfPartitions;
        private uint LengthOfImplementationUse;
        private uint[] FreeSpaceTable;
        private uint[] SizeTable;
        private LogicalVolumeIntegrityDescriptorImplementationUse LVIDIU;

        public LogicalVolumeIntegrityDescriptor(uint numberOfFiles, uint numberOfDirectories, uint sizeOfMainPartition, string applicationName)
        {
            Descriptor = new Tag(Tag.LogicalVolumeIntegrityDescriptorTag);
            RecordingDateAndTime = new TimeStamp(IsoUdfGenerator.GetTimeStamp());
            IntegrityType = 1; // Close Integrity Descriptor
            nextIntegrityExtent = new ExtentAD(0, 0);

            LogicalVolumeContentsUse[0] = (byte) ( numberOfFiles + (numberOfDirectories-1) + 0x10);  // -1 because directory count includes root

            NumberOfPartitions = 2; // 1 main and another...
            LengthOfImplementationUse = 46; // size of LVIDIU

            FreeSpaceTable = new uint[NumberOfPartitions];
            SizeTable = new uint[NumberOfPartitions];
            SizeTable[0] = sizeOfMainPartition;      
            SizeTable[1] = 0x40;      //  64 bytes second partition. Not totally sure why.

            LVIDIU = new LogicalVolumeIntegrityDescriptorImplementationUse(numberOfFiles, numberOfDirectories, applicationName);

            Descriptor.CalcDescriptorCRC(this);
        }

        public int GetLengthInBytes()
        {
            return (int)(80 + (NumberOfPartitions * 8) + LengthOfImplementationUse);
        }

        public void Write(BinaryWriter writer)
        {
            Descriptor.Write(writer, 0);
            RecordingDateAndTime.Write(writer);
            writer.Write(IntegrityType);
            nextIntegrityExtent.Write(writer);
            writer.Write(LogicalVolumeContentsUse);
            writer.Write(NumberOfPartitions);
            writer.Write(LengthOfImplementationUse);

            foreach (uint freeSpace in FreeSpaceTable)
            {
                writer.Write(freeSpace);
            }

            foreach (uint size in SizeTable)
            {
                writer.Write(size);
            }

            LVIDIU.Write(writer);

        }
    }
}
