using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.BDMV
{
    /// <summary>
    /// Selection Information Table (SIT):
    /// The SIT is used only in "partial" (i.e. recorded) bitstreams.
    /// PID must be 0x1f
    /// </summary>
    public class SIT : Payload
    {
        public const ushort PID = 0x1f;

        private byte sectionSyntaxIndicator;        // 1 bit
        private byte DVBReservedFutureUse =0x1;    // 1 bit
        private byte ISOReserved = 0x3;             // 2 bits
        private ushort sectionLength;                 // 12 bits
        private ushort DVBReservedFutureUse2 = 0xffff; // 16 bits
        private byte ISOReserved2 = 0x3;            // 2 bits
        private byte versionNumber;                 // 5 bits
        private byte currentNextIndicator;          // 1 bit
        private byte sectionNumber;                 // 8  bits  
        private byte lastSectionNumber;             // 8  bits
        private byte DVBReservedForFutureUse2 =0xf; // 4  bits
        private ushort transmissionInfoLoopLength;  // 12 bits
        private List<Descriptor> programDescriptors = new List<Descriptor>();

        //
        // Only one service ID
        //
        private ushort serviceID;                   // 16 bits
        private byte DVBReservedFutureUse3 = 1;     // 1 bit
        private byte runningStatus;                 // 3 bits
        private ushort serviceLoopLength;           // 12 bits  (this is zero)

        public SIT() 
        {
            //
            // If anything is changed here then the CRC needs to be changed at the end
            // 
            sectionSyntaxIndicator=1;
            versionNumber = 0;
            currentNextIndicator = 1;
            sectionNumber = 0;
            lastSectionNumber = 0;
            transmissionInfoLoopLength = 0; 

            PartialTransportStreamDescriptor ptds = new PartialTransportStreamDescriptor();
            programDescriptors.Add(ptds);
            serviceID = 1;
            runningStatus = 0;
            serviceLoopLength = 0;

            foreach (Descriptor d in programDescriptors)
            {
                transmissionInfoLoopLength += (ushort)d.GetLengthInBytes();
            }

            //
            // Initial section + descriptors + serviceIDs + CRC
            //
            sectionLength = (ushort) (7 + transmissionInfoLoopLength + 4 + 4);
        }

        public override uint GetLengthInBytes()
        {
            uint size = 18;

            foreach (Descriptor d in programDescriptors)
            {
                size += d.GetLengthInBytes();
            }

            return size;
        }

        public override void  Write(BinaryWriter writer)
        {
            writer.Write((byte)0x7f);
            ushort nextWord = (ushort)(sectionSyntaxIndicator<<15);
            nextWord |= (ushort)(DVBReservedFutureUse<<14);
            nextWord |= (ushort)(ISOReserved<<12);
            nextWord |= sectionLength;
            nextWord = EndianSwap.Swap(nextWord);
            writer.Write(nextWord);

            writer.Write(EndianSwap.Swap(DVBReservedFutureUse2));

            byte nextByte = (byte)(ISOReserved2<<6);
            nextByte |= (byte)(versionNumber<<1);
            nextByte |= currentNextIndicator;
            writer.Write(nextByte);

            writer.Write(sectionNumber);
            writer.Write(lastSectionNumber);

            nextWord = (ushort)(DVBReservedForFutureUse2<<12);
            nextWord |= transmissionInfoLoopLength;
            nextWord = EndianSwap.Swap(nextWord);
            writer.Write(nextWord);

            foreach (Descriptor d in programDescriptors)
            {
                d.Write(writer);
            }

            writer.Write(EndianSwap.Swap(serviceID));
          
            nextWord = (ushort)(DVBReservedFutureUse3<<15);
            nextWord |= (ushort)(runningStatus<< 12);
            nextWord |= serviceLoopLength;
            nextWord = EndianSwap.Swap(nextWord);
            writer.Write(nextWord);

            //
            // Fixed CRC (needs re-calculating if anything in the SIT is changed)
            //
            writer.Write((byte)0x34);
            writer.Write((byte)0x1e);
            writer.Write((byte)0xe7);
            writer.Write((byte)0x4e);
        }
    }
}
