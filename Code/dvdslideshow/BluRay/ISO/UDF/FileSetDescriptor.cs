using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    /// <summary>
    /// This class represents a UDF File Set Descritor (FSD)
    /// This can be thought of as the complete file system inside a partition and
    /// contains a reference to the top "root" directory.
    /// </summary>
    public class FileSetDescriptor : USDLinkedEntry, IDescriptor
    {
        private Tag Descriptor;
        private TimeStamp RecordingDateAndTime;
        private ushort InterchangeLevel;
        private ushort MaximumInterchangeLevel;
        private uint CharacterSetList;
        private uint MaximumCharacterSetList;
        private uint FileSetNumber;
        private uint FileSetDescriptorNumber;
        private Charspec LogicalVolumeIdentifierCharacter;
        private DString LogicalVolumeIdentifier;
        private Charspec FileSetCharacterSet;
        private DString FileSetIndentifier;
        private DString CopyrightFileIndentifier;
        private DString AbstractFileIdentifier;
        private LongAd RootDirectoryICB;
        private EntityID DomainIdentifier;
        private LongAd NextExtent;
        private LongAd SystemStreamDirectoryICB;
        private  byte[] Reserved;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="address"></param>
        /// <param name="volumeIdentifier"></param>
        public FileSetDescriptor(ShortAd address, string volumeIdentifier) :
            base(address)
        {
            Descriptor = new Tag(Tag.FileSetDescriptorTag);
            RecordingDateAndTime = new TimeStamp(IsoUdfGenerator.GetTimeStamp());
            InterchangeLevel = 3;
            MaximumInterchangeLevel = 3;
            CharacterSetList = 1;
            MaximumCharacterSetList = 1;
            FileSetNumber = 0;
            FileSetDescriptorNumber = 0;
            LogicalVolumeIdentifierCharacter = new Charspec("OSTA Compressed Unicode");
            LogicalVolumeIdentifier = new DString(volumeIdentifier, 128);
            FileSetCharacterSet = new Charspec("OSTA Compressed Unicode");

            //
            // Truncate volume identifer to 14 characters
            //
            if (volumeIdentifier.Length > 14)
            {
                volumeIdentifier = volumeIdentifier.Substring(0, 14);
            }

            FileSetIndentifier = new DString(volumeIdentifier, 32);
            CopyrightFileIndentifier = new DString("", 32);
            AbstractFileIdentifier = new DString("", 32);
            RootDirectoryICB = new LongAd(0x800, new LbAddr(2,1));
            DomainIdentifier = new EntityID("*OSTA UDF Compliant", 0x250, true);
            NextExtent = new LongAd(0, new LbAddr(0, 0));
            SystemStreamDirectoryICB = new LongAd(0, new LbAddr(0, 0));
            Reserved = new byte[32];  
        }

        /// <summary>
        /// Writes the FSD to the given binary writer
        /// </summary>
        /// <param name="writer"></param>
        public override void Write(BinaryWriter writer)
        {
            //
            // If writing to file, verify position
            //
            if (writer.BaseStream is FileStream)
            {
                Descriptor.CalcDescriptorCRC(this);

                UDFLog.Info("Writing FSD ");
                OutputAddress();
                UDFLog.Info("");
                VerifyPosition(writer);
            }

            Descriptor.Write(writer, StoredAddress);  
            RecordingDateAndTime.Write(writer);
            writer.Write(InterchangeLevel);
            writer.Write(MaximumInterchangeLevel);
            writer.Write(CharacterSetList);
            writer.Write(MaximumCharacterSetList);
            writer.Write(FileSetNumber);
            writer.Write(FileSetDescriptorNumber);
            LogicalVolumeIdentifierCharacter.Write(writer);
            LogicalVolumeIdentifier.Write(writer);
            FileSetCharacterSet.Write(writer);
            FileSetIndentifier.Write(writer);
            CopyrightFileIndentifier.Write(writer);
            AbstractFileIdentifier.Write(writer);
            RootDirectoryICB.Write(writer);
            DomainIdentifier.Write(writer);
            NextExtent.Write(writer);
            SystemStreamDirectoryICB.Write(writer);
            writer.Write(Reserved);

            if (writer.BaseStream is FileStream)
            {
                //
                // Pad to 1 sector (FSD is 512 bytes)
                //
                byte[] blank = new byte[2048 - 512];
                writer.Write(blank);

                //
                //  Write out a TD
                //
                TerminatingDescriptor td = new TerminatingDescriptor(PartitionOffset);
                td.Write(writer);
                writer.Write(blank);

                //
                // write out linked entry (if exists)
                //
                LinkedEntry.PartitionOffset = PartitionOffset;
                LinkedEntry.PadAndWrite(writer) ;

                //
                // Write out root FID
                //
                FileIdentifierDescriptorSet set = (LinkedEntry as ExtendedFileEntry).LinkedEntry as FileIdentifierDescriptorSet;
                set.PartitionOffset = PartitionOffset;
                set.Write(writer);
            }
        }
    }
}
