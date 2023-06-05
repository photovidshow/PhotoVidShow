using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    public class PrimaryVolumeDescriptor : IDescriptor
    {
        //
        // ECMA 167 3/10.1 
        //
        private Tag descriptorTag;
        private uint VolumeDescriptorSequenceNumber = 0;
        private uint PrimaryVolumeDescriptorNumber = 0;
        private DString VolumeIdentifier;
        private ushort VolumeSequenceNumber;
        private ushort MaximumVolumeSequenceNumber;
        private ushort InterchangeLevel;
        private ushort MaximumInterchangeLevel;
        private uint CharacterSetList;
        private uint MaximumCharacterSetList;
        private DString VolumeSetIdentifier;
        private Charspec DescriptorCharacterSet;
        private Charspec ExplanatoryCharacterSet;
        private ExtentAD VolumeAbstract;
        private ExtentAD VolumeCopyrightNotice;
        private EntityID ApplicationIdentifier;
        private TimeStamp RecordingDateAndTime;
        private EntityID ImplementationIdentifier;
        private byte[] ImplementationUse = new byte[64];
        private uint PredecessorVolumeDescriptorSequenceLocation = 0;
        private ushort Flags = 0;
        private byte[] Reserved = new byte[22];

        public PrimaryVolumeDescriptor(string volumeSetIdentifier, string volumeIdentifier, string applicationName)
        {
            descriptorTag = new Tag(Tag.PrimaryVolumeDescriptorTag);

            //
            // Generate volume identifier (uni-code, so truncate to 14 characters max)
            //
            if (volumeIdentifier.Length > 14)
            {
                volumeIdentifier = volumeIdentifier.Substring(0, 14);
            }
            VolumeIdentifier = new DString(volumeIdentifier, 32);

            //
            // Set volume sequence number, first and only volume sequence number
            //
            VolumeSequenceNumber = 1;
            MaximumVolumeSequenceNumber = 1;

            //
            // At Level 2, the following restrictions shall apply:
            //-  The number in any Length of File Identifier and Length of Component Identifier field shall not exceed 14.
            //-  The maximum length of a resolved pathname (4/8.7.1) shall not exceed 1 023.
            //-  The number in any File Link Count field in a File Entry shall not exceed 8.
            // Note 53: This interchange level provides compatibility with ISO/IEC 9945-1 file system restrictions.
            //
            InterchangeLevel = 2;   
            MaximumInterchangeLevel = 2;

            //
            // This field shall identify the character sets specified by any field, whose contents are specified
            // to be a charspec (1/7.2.1), of any descriptor specified in Part 3 and recorded on the volume
            // described by this descriptor.
            //
            CharacterSetList = 1;
            MaximumCharacterSetList = 1;

            VolumeSetIdentifier = new DString(volumeSetIdentifier, 128);
        
            DescriptorCharacterSet = new Charspec("OSTA Compressed Unicode");

            ExplanatoryCharacterSet = new Charspec("OSTA Compressed Unicode");

            VolumeAbstract = new ExtentAD(0, 0);
            VolumeCopyrightNotice = new ExtentAD(0, 0);

            ApplicationIdentifier = new EntityID(applicationName, 0, false);

            RecordingDateAndTime = new TimeStamp(IsoUdfGenerator.GetTimeStamp());

            ImplementationIdentifier = new EntityID("*" + applicationName, 0, false);   // OSTA says have leading '*'

            // Implementation use is just blank zeros

            // PredecessorVolumeDescriptorSequenceLocation = blank

            // Flags = blank

            descriptorTag.CalcDescriptorCRC(this);          
        }
      
        public void Write(BinaryWriter outWriter)
        {
            descriptorTag.Write(outWriter,0);
            outWriter.Write(VolumeDescriptorSequenceNumber);
            outWriter.Write(PrimaryVolumeDescriptorNumber);
            VolumeIdentifier.Write(outWriter);
            outWriter.Write(VolumeSequenceNumber);
            outWriter.Write(MaximumVolumeSequenceNumber);
            outWriter.Write(InterchangeLevel);
            outWriter.Write(MaximumInterchangeLevel);
            outWriter.Write(CharacterSetList);
            outWriter.Write(MaximumCharacterSetList);
            VolumeSetIdentifier.Write(outWriter);
            DescriptorCharacterSet.Write(outWriter);
            ExplanatoryCharacterSet.Write(outWriter);
            VolumeAbstract.Write(outWriter);
            VolumeCopyrightNotice.Write(outWriter);
            ApplicationIdentifier.Write(outWriter);
            RecordingDateAndTime.Write(outWriter);
            ImplementationIdentifier.Write(outWriter);
            outWriter.Write(ImplementationUse);
            outWriter.Write(PredecessorVolumeDescriptorSequenceLocation);
            outWriter.Write(Flags);
            outWriter.Write(Reserved);
        }
    }
}
