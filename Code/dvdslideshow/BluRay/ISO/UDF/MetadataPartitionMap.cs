using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    public class MetadataPartitionMap : PartitionMap
    {
        //
        // OSTA 2.2.10 Metadata Partition Map
        //
        private byte Type;
        private byte Length;
        private ushort Reserved = 0;
        private EntityID Identifier;
        private ushort VolumeSequenceNumber;
        private ushort PartitionNumber;
        private uint MetadataFileLocation;
        private uint MetadataMirrorFileLocation;
        private uint MetadataBitmapFileLocation;
        private uint AllocationUnitSize;
        private ushort AlignmentUintSize;
        private byte Flags;
        private byte[] Reserved2 = new byte[5];

        public MetadataPartitionMap(uint metadataFileLocation, uint metadataMirrorFileLocation)
        {
            Type = 2;
            Length = 64;
            Identifier = new EntityID("*UDF Metadata Partition", 0x250, false);
            VolumeSequenceNumber = 1;       
            PartitionNumber = 0;        
            MetadataFileLocation = metadataFileLocation;
            MetadataMirrorFileLocation = metadataMirrorFileLocation;   
            MetadataBitmapFileLocation = 0xffffffff;
            AllocationUnitSize = 32;      
            AlignmentUintSize = 32;    
            Flags = 1;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Type);
            writer.Write(Length);
            writer.Write(Reserved);
            Identifier.Write(writer);
            writer.Write(VolumeSequenceNumber);
            writer.Write(PartitionNumber);
            writer.Write(MetadataFileLocation);
            writer.Write(MetadataMirrorFileLocation);
            writer.Write(MetadataBitmapFileLocation);
            writer.Write(AllocationUnitSize);
            writer.Write(AlignmentUintSize);
            writer.Write(Flags);
            writer.Write(Reserved2);
        }

        public override int LengthInBytes()
        {
            return Length;
        }

    }
}

