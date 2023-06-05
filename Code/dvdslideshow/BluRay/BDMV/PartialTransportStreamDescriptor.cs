using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.BDMV
{
    /// <summary>
    /// The transmission information descriptor loop of the SIT contains all the information required for controlling and
    /// managing the play-out and copying of partial TSs. The following descriptor is proposed to describe this information.
    /// </summary>
    public class PartialTransportStreamDescriptor : Descriptor
    {
        private byte descriptorLength;                  // 8 bits
        private byte DVBReservedFutureUse=0x3;          // 2 bits

        /// <summary>
        /// The maximum momentary transport packet rate (i.e. 188 bytes divided by the time interval between start
        /// times of two succeeding TS packets). At least an upper bound for this peak_rate should be given. This 22-bit field is
        /// coded as a positive integer in units of 400 bit/s.
        /// </summary>
        private uint peakRate;                          // 22 bits

        private byte DVBReservedFutureUse2=0x3;         // 2 bits
        private uint minimumOverallSmoothingRate;       // 22 bits
        private byte DVBReservedFutureUse3=0x3;         // 2 bits  
        private ushort maximumOverallSmoothingBuffer;   // 14 bits

        public PartialTransportStreamDescriptor() :
            base(0x63)
        {
            descriptorLength = 8;

            //
            // This value is used on all pro-show and on sintel streams.  Blu-ray default???
            //
            peakRate = 88750;  // 4437500 bytes per second  // 4.23 meg second?
            minimumOverallSmoothingRate = 0x3fffff; // value means not defined
            maximumOverallSmoothingBuffer = 0x3fff; // value means not defined
        }

        public override uint GetLengthInBytes()
        {
            return 10;
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(descriptorLength);

            uint next24Bits = (uint)(DVBReservedFutureUse << 22);
            next24Bits |= peakRate;
            writer.Write((byte)(next24Bits >> 16));
            writer.Write((byte)((next24Bits >> 8) & 0xff));
            writer.Write((byte)(next24Bits & 0xff));

            next24Bits = (uint)(DVBReservedFutureUse2 << 22);
            next24Bits |= minimumOverallSmoothingRate;
           
            writer.Write((byte)(next24Bits >> 16));
            writer.Write((byte)((next24Bits >> 8) &0xff));
            writer.Write((byte)(next24Bits & 0xff));

            ushort nextWord = (ushort)(DVBReservedFutureUse3 << 14);
            nextWord |= maximumOverallSmoothingBuffer;
            EndianSwap.Swap(nextWord);
            writer.Write(nextWord);
        }
    }
}
