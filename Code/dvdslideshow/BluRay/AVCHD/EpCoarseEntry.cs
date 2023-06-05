using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.AVCHD
{
    public class EpCoarseEntry
    {
        private uint   refFineIndex;      // 18 bits
        private ushort pTS;               // 14 bits
        private uint   sPN;               // 32 bits

        public EpCoarseEntry(uint refFineIndex, ushort upperPTS, uint upperSPN)
        {
            this.refFineIndex = refFineIndex;
            this.pTS = upperPTS;
            this.sPN = upperSPN;
        }

        public void Write(BinaryWriter writter)
        {
            uint nextDword = (uint)(refFineIndex << 14);
            nextDword |= (uint)(pTS);
            writter.Write(EndianSwap.Swap(nextDword));
            writter.Write(EndianSwap.Swap(sPN));
        }
    }
}
