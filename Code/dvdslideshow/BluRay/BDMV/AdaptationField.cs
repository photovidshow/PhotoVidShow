using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.BDMV
{
    /// <summary>
    /// This class represents an Adaptation Field part of a TS Packet.
    /// Typically instances of this class are used to either:-
    /// 1) Pad out a TS packet to align with the end of the PES payload.
    /// 2) Store PCR information which is transmitted at least every 100ms.
    /// </summary>
    public class AdaptationField
    {
        private byte adaptationFieldLength;
        private byte discontinuityIndicator;            // 1 bit
        private byte randomAccessIndicator;             // 1 bit
        private byte elementaryStreamPriorityIndicator; // 1 bit
        private byte PCRFlag;                           // 1 bit
        private byte OPCRFlag;                          // 1 bit
        private byte splicingPointFlag;                 // 1 bit
        private byte transportPrivateDataFlag;          // 1 bit
        private byte adaptationFieldExtensionFlag;      // 1 bit

        //
        // If PCR set
        //
        private long programClockReferenceBase;         // 33 bits
        private byte reserved =0x3f;                    // 6 bits
        private ushort programClockReferenceExtension;  // 9 bits


        public const long PCRTimeBase = 53955000;     // 90 Khz clock     = (10 minutes - 0.5) seconds 

        public long PCR
        {
            get { return programClockReferenceBase; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="length"></param>
        /// <param name="PCRFlag"></param>
        /// <param name="timeOffset"></param>
        public AdaptationField(byte length, bool PCRFlag, long timeOffset)
        {
            adaptationFieldLength = length; 
            discontinuityIndicator = 0;
            randomAccessIndicator = 0;
            elementaryStreamPriorityIndicator = 0;
            if (PCRFlag == true)
            {
                this.PCRFlag = 1;
            }
            else
            {
                this.PCRFlag = 0;
            }

            OPCRFlag = 0;
            splicingPointFlag = 0;
            transportPrivateDataFlag = 0;
            adaptationFieldExtensionFlag = 0;

            programClockReferenceBase = PCRTimeBase+ timeOffset;   // 90 khz clock
            programClockReferenceExtension = 0;
        }

        /// <summary>
        /// Gets the length of the Adaptation field in bytes
        /// </summary>
        /// <returns></returns>
        public uint GetLengthInBytes()
        {
            return 1 + ((uint)adaptationFieldLength);
        }

        /// <summary>
        /// Writes the adaptation field to the given writer
        /// </summary>
        /// <param name="writer"></param>
        public void Write(BinaryWriter writer)
        {
            writer.Write(adaptationFieldLength);

            if (adaptationFieldLength == 0)
            {
                return;
            }

            byte nextByte = (byte)(discontinuityIndicator<<7);
            nextByte |= (byte)(randomAccessIndicator<<6);
            nextByte |= (byte)(elementaryStreamPriorityIndicator<<5);
            nextByte |= (byte)(PCRFlag<<4);
            nextByte |= (byte)(OPCRFlag<<3);
            nextByte |= (byte)(splicingPointFlag<<2);
            nextByte |= (byte)(transportPrivateDataFlag<<1);
            nextByte |= adaptationFieldExtensionFlag;

            writer.Write(nextByte);

            int padding = adaptationFieldLength-1;

            if (PCRFlag == 1)
            {
                long next48Bit = (long)(programClockReferenceBase << 15);
                next48Bit |= (long)reserved << 9;
                next48Bit |= programClockReferenceExtension;

                writer.Write((byte)(next48Bit >> 40));
                writer.Write((byte)((next48Bit >> 32) & 0xff));
                writer.Write((byte)((next48Bit >> 24) & 0xff));
                writer.Write((byte)((next48Bit >> 16) & 0xff));
                writer.Write((byte)((next48Bit >> 8) & 0xff));
                writer.Write((byte)(next48Bit & 0xff));
                padding -= 6;
            }

            for (int i = 0; i < padding; i++)
            {
                writer.Write((byte)0xff);
            }
        }
    }
}
