using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    /// <summary>
    /// This class represents a UDF implementation use volume descriptor  (IUVD)
    /// </summary>
    public class ImplementationUseVolumeDescriptor : IDescriptor
    {
        //
        // ECMA 167 3/10.4 
        //
        private Tag DescriptorTag;
        private uint VolumeDescriptorSequenceNumber;
        private EntityID ImplementationIdentifier;

        //
        // OSTA 2.2.7.2 bytes ImplementationUse[460]
        //
        private LogicalVolumeInformation logicalVolumeInformation;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="volumeIdentifier"></param>
        /// <param name="applicationName"></param>
        public ImplementationUseVolumeDescriptor(string volumeIdentifier, string applicationName)
        {
            DescriptorTag = new Tag(Tag.ImplementationUseVolumeDescriptorTag);
            VolumeDescriptorSequenceNumber = 1;
            ImplementationIdentifier = new EntityID("*UDF LV Info", 0x250, false);
            logicalVolumeInformation = new LogicalVolumeInformation(volumeIdentifier, applicationName);
            DescriptorTag.CalcDescriptorCRC(this);
        }

        /// <summary>
        /// Write the IUVD to the given binary writer
        /// </summary>
        /// <param name="writer"></param>
        public void Write(BinaryWriter writer)
        {
            DescriptorTag.Write(writer,0);
            writer.Write(VolumeDescriptorSequenceNumber);
            ImplementationIdentifier.Write(writer);
            logicalVolumeInformation.Write(writer);
        }

    }
}
