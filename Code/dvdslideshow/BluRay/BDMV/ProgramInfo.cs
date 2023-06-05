using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.BDMV
{
    /// <summary>
    /// Program info used inside a PAT payload
    /// </summary>
    public class ProgramInfo
    {
        public ushort programNumber;            // 16 bits;
        public byte reserved = 0x7;             // 3  bits;

        //
        // If program number is 0 we use networkPID else programMapPID
        //
        public ushort networkPID;                 // 13 bits
        public ushort programMapPID;             // 13 bits

        public uint GetLengthInBytes()
        {
            return 4;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(EndianSwap.Swap(programNumber));
            ushort nextWord = (ushort)(reserved << 13);
            if (programNumber == 0)
            {
                nextWord |= networkPID;
            }
            else
            {
                nextWord |= programMapPID;
            }

            nextWord = EndianSwap.Swap(nextWord);

            writer.Write(nextWord);
        }
    }
}
