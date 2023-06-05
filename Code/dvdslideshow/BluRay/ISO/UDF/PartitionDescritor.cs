using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    public class PartitionDescritor : IDescriptor
    {
        //
        // ECMA 167 3/10.5 
        //
        private Tag DescriptorTag;
        private uint VolumeDescriptorSequenceNumber;
        private ushort PartitionFlags;
        private ushort PartitionNumber;
        private EntityID PartitionContents;
        private byte[] PartitionContentsUse = new byte[128];
        private uint AccessType;
        private uint PartitionStartingLocation; // START
        private uint PartitionLength; // SIZE
        private EntityID ImplementationIdentifier;
        private byte[] ImplementationUse = new byte[128];
        private byte[] Reserved = new byte[156];

        //
        // File system (stored in the partition)
        //
        private FileSystem fileSystemTree;

        public FileSystem FileSystemTree
        {
            get { return fileSystemTree;  }
        }

        public uint PartitionLengthInSectors
        {
            get { return PartitionLength; }
        }

        public PartitionDescritor(string rootFolder, string volumeIdentifier, string applicationName, bool streamFolderFirst)
        {
            DescriptorTag = new Tag(Tag.PartitionDescritorTag);
            VolumeDescriptorSequenceNumber = 2; 
            PartitionFlags = 1; // means the volume space has been allocated for this partition
            PartitionNumber = 0;
            PartitionContents = new EntityID("+NSR03", 0, false);  // from OSTA
            // PartitionContentsUse is blank
            AccessType = 1; // read only
            PartitionStartingLocation = 288;    // start partition sector 288 = 0x90000 into file (i.e. after AVDP which is located at 0x80000) 
            ImplementationIdentifier = new EntityID("*" + applicationName, 0, false);
            // ImplementationUse blank
            // Reserved blank

            fileSystemTree = new FileSystem(rootFolder, volumeIdentifier, applicationName, streamFolderFirst);
            PartitionLength = fileSystemTree.LengthInSectors;

            DescriptorTag.CalcDescriptorCRC(this);
        }

        public void Write(BinaryWriter writer)
        {
            DescriptorTag.Write(writer,0);
            writer.Write(VolumeDescriptorSequenceNumber);
            writer.Write(PartitionFlags);
            writer.Write(PartitionNumber);
            PartitionContents.Write(writer);
            writer.Write(PartitionContentsUse);
            writer.Write(AccessType);
            writer.Write(PartitionStartingLocation);
            writer.Write(PartitionLength);
            ImplementationIdentifier.Write(writer);
            writer.Write(ImplementationUse);
            writer.Write(Reserved);
        }
    }

}


