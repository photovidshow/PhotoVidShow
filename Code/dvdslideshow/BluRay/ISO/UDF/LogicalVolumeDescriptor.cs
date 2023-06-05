using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    public class LogicalVolumeDescriptor : IDescriptor
    {     
        //
        // * LVD records the logical volume name, and the start address and length of FSD
        // * And FSD records the root directory of current mounted disc.
        // * There is only one FSD inside a partition.
        // * FSD contains the start address and length of root directory FE
        // * FE is consider as the entry of the file or file folder.
        // * When you get the start address of FSD, it is the offset relative to the start position of Partition.
        // 

        //
        //ECMA 167 3/10.6
        //
        private Tag DescriptorTag;
        private uint VolumeDescriptorSequenceNumber;
        private Charspec DescriptorCharacterSet;
        private DString LogicalVolumeIdentifier; //[128];
        private uint LogicalBlockSize;
        private EntityID DomainIdentifier;

        //
        // This field contains the extent location of the FileSet Descriptor. This is described
        // in 4/3.1 of ECMA 167 as follows:
        // “If the volume is recorded according to Part 3, the extent in which the first File Set Descriptor
        // Sequence of the logical volume is recorded shall be identified by a long_ad (4/14.14.2) recorded
        // in the Logical Volume Contents Use field (see 3/10.6.7) of the Logical Volume Descriptor
        // describing the logical volume in which the File Set Descriptors are recorded.”
        // This field can be used to find the FileSet descriptor, and from the FileSet
        // descriptor the root directory can be found.
        //
        private LongAd LogicalVolumeContentsUse;

        private uint MapTableLength;
        private uint NumberofPartitionMaps;
        private EntityID ImplementationIdentifier;
        private byte[] ImplementationUse = new byte[128];
        private ExtentAD IntegritySequenceExtent;
        private List<PartitionMap> PartitionMaps = new List<PartitionMap>();

        public LogicalVolumeDescriptor(uint metadataFileLocation, uint metadataMirrorFileLocation, string volumeIdentifier, string applicationName)
        {
            DescriptorTag = new Tag(Tag.LogicalVolumeDescriptorTag);
            VolumeDescriptorSequenceNumber = 3;
            DescriptorCharacterSet = new Charspec("OSTA Compressed Unicode");
            LogicalVolumeIdentifier = new DString(volumeIdentifier, 128);
            LogicalBlockSize = 2048;
            DomainIdentifier = new EntityID("*OSTA UDF Compliant", 0x250, true);
            LogicalVolumeContentsUse = new LongAd(4096, new LbAddr(0, 1)); // This is a offset to first File System Entry from start of partition (in PD) 
            ImplementationIdentifier = new EntityID("*" + applicationName, 0, false);
            // ImplementationUse blank

            IntegritySequenceExtent = new ExtentAD(64, 4096);  // SRG ???

            PartitionMap map1 = new NormalPartitionMap(1, 0);
            PartitionMaps.Add(map1);
            PartitionMap map2 = new MetadataPartitionMap(metadataFileLocation, metadataMirrorFileLocation);
            PartitionMaps.Add(map2);

            NumberofPartitionMaps = (uint) PartitionMaps.Count;

            foreach (PartitionMap map in PartitionMaps)
            {
                MapTableLength += (uint)map.LengthInBytes();
            }

            DescriptorTag.CalcDescriptorCRC(this);
        }

        public int LengthInBytes()
        {
            return 440 + (int)MapTableLength;
        }

        public void Write(BinaryWriter writer)
        {
            DescriptorTag.Write(writer,0);
            writer.Write(VolumeDescriptorSequenceNumber);
            DescriptorCharacterSet.Write(writer);
            LogicalVolumeIdentifier.Write(writer);
            writer.Write(LogicalBlockSize);
            DomainIdentifier.Write(writer);
            LogicalVolumeContentsUse.Write(writer);
            writer.Write(MapTableLength);
            writer.Write(NumberofPartitionMaps);
            ImplementationIdentifier.Write(writer);
            writer.Write(ImplementationUse);
            IntegritySequenceExtent.Write(writer);

            foreach (PartitionMap map in PartitionMaps)
            {
                map.Write(writer);
            }
        }
    }
}
