using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.AVCHD
{
    public class EpFineEntry
    {
        private byte angle;                 // 1 bit
        private byte iEndOffsetPosition;    // 3 bit
        private ushort pTS;                 // 11 bits
        private uint sPN;                   // 17 bits

        public EpFineEntry(uint packetSize, ushort lowerPTS, uint lowerSPN)
        {
            //
            // Not used
            //
            angle = 0;

            pTS = lowerPTS;
            sPN = lowerSPN;

            //
            // Numbers from patent EP2051517 A1
            //
            if (packetSize < 131072)
            {
                iEndOffsetPosition = 1;
            }
            else if (packetSize < 262144)
            {
                iEndOffsetPosition = 2;
            }
            else if (packetSize < 393216)
            {
                iEndOffsetPosition = 3;
            }
            else if (packetSize < 589824)
            {
                iEndOffsetPosition = 4;
            }
            else if (packetSize < 917504)
            {
                iEndOffsetPosition = 5;
            }
            else if (packetSize < 1310720)
            {
                iEndOffsetPosition = 6;
            }
            else if (packetSize == 1310720)
            {
                iEndOffsetPosition = 7;
            }
            else
            {
                iEndOffsetPosition = 0;
            }
        }

        public void Write(BinaryWriter writter)
        {
            uint nextDword = (uint)(angle<<31);
            nextDword |= (uint)(iEndOffsetPosition << 28);
            nextDword |= (uint)(pTS <<17);
            nextDword |= sPN;
            writter.Write(EndianSwap.Swap(nextDword));
        }
    }
}
