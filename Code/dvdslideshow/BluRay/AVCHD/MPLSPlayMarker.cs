using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.AVCHD
{
    public class MPLSPlayMarker
    {
        // 14 bytes
        private byte   markID;
        private byte   markType;
        private ushort playItemRef;
        private uint   time;
        private ushort entryEsPid;
        private uint   duration;

        public MPLSPlayMarker(uint secondsFromStart)
        {
            markID = 0;
            markType = 1;
            playItemRef = 0;
            time = 27000000 + (secondsFromStart * 45000);  // 10 mins +  ( secondsFromStart * (45000 = 1 second) ) ; 
            entryEsPid = 0xffff;
            duration = 0;
        }

        public void Write(BinaryWriter writter)
        {
            writter.Write(markID);
            writter.Write(markType);
            writter.Write(EndianSwap.Swap(playItemRef));
            writter.Write(EndianSwap.Swap(time));
            writter.Write(EndianSwap.Swap(entryEsPid));
            writter.Write(EndianSwap.Swap(duration));
        }
    }
}
