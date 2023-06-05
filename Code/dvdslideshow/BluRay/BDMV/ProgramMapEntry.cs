using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.BDMV
{
    /// <summary>
    /// An entry in the PMT (Program Map Table)
    /// </summary>
    public abstract class ProgramMapEntry
    {
        private byte streamType;        // 8 bits
        private byte reserved = 0x7;    // 3 bits
        private ushort PID;               // 13 bits;
        private byte reserved2 = 0xf;   // 4 bits;
        private ushort ESInfoLength;      // 12 bits;
        protected List<Descriptor> descriptors = new List<Descriptor>();

        public ProgramMapEntry(byte streamType, ushort pid)
        {
            this.streamType = streamType;
            PID = pid;
            ESInfoLength = 0; // Calculated on write
        }

        public virtual void Write(BinaryWriter writer)
        {
            ESInfoLength = 0;
            foreach (Descriptor d in descriptors)
            {
                ESInfoLength += (ushort)d.GetLengthInBytes();
            }

            writer.Write(streamType);
            ushort nextWord = (ushort)(reserved << 13);
            nextWord |= PID;
            nextWord = EndianSwap.Swap(nextWord);
            writer.Write(nextWord);

            nextWord = (ushort)(reserved2 << 12);
            nextWord |= ESInfoLength;
            nextWord = EndianSwap.Swap(nextWord);
            writer.Write(nextWord);

            foreach (Descriptor d in descriptors)
            {
                d.Write(writer);
            }
        }

        public virtual uint GetLengthInBytes()
        {
            uint size = 5;
            foreach (Descriptor d in descriptors)
            {
                size += d.GetLengthInBytes();
            }
            return size;
        }

    }
}
