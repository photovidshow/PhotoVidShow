using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    public class NormalPartitionMap : PartitionMap
    {
        //
        // 6 bytes
        //
        private byte Type;
        private byte Length;
        private ushort VolumeSequenceNumber;
        private ushort PartitionNumber;

        public NormalPartitionMap(ushort volumeSequenceNumber, ushort partitionNumber)
        {
            Type = 1;
            Length = 6;
            VolumeSequenceNumber = volumeSequenceNumber;
            PartitionNumber = partitionNumber;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Type);
            writer.Write(Length);
            writer.Write(VolumeSequenceNumber);
            writer.Write(PartitionNumber);
        }

        public override int LengthInBytes()
        {
            return Length;
        }
    }
}
