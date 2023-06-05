using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    public class LbAddr
    {
        private uint logicalBlockNumber; // logical block size in LogicalVolumeDescriptor
        private ushort partitionReferenceNumber;

        public uint LogicalBlockNumber
        {
            get { return logicalBlockNumber; }
            set { logicalBlockNumber = value; }
        }

        public LbAddr(uint logicalBlockNumber, ushort partitionReferenceNumber)
        {
            this.logicalBlockNumber = logicalBlockNumber;
            this.partitionReferenceNumber = partitionReferenceNumber;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(logicalBlockNumber);
            writer.Write(partitionReferenceNumber);
        }
    }
}
