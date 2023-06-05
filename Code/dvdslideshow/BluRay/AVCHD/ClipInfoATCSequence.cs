using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.AVCHD
{
    public class ClipInfoATCSequence
    {
        private uint spnAtcStart;
        private byte numStcSeq;     
        private byte offsetStcId;

        // only 1 stc sequnce so store in this class
        private ushort pcrPid;
        private uint spnStcStart;
        private uint presentationStartTime;
        private uint presentationEndTime;

        public ClipInfoATCSequence(float legnthInSeconds)
        {
            spnAtcStart = 0;
            numStcSeq = 1;
            offsetStcId = 0;
            pcrPid = 0x1001;
            spnStcStart = 0;

            //
            // 45000 = 1 second; 
            // Start time is always at 10 minutes
            //
            presentationStartTime = 0x019bfcc0; // 10 mins

            double end = ((double)legnthInSeconds)*45000;
            end += presentationStartTime;

            this.presentationEndTime = (uint)(end);
        }

        public uint GetLengthInBytes()
        {
            return 20;
        }

        public void Write(BinaryWriter writter)
        {
            writter.Write(EndianSwap.Swap(spnAtcStart));
            writter.Write(numStcSeq);
            writter.Write(offsetStcId);

            writter.Write(EndianSwap.Swap(pcrPid));
            writter.Write(EndianSwap.Swap(spnStcStart));
            writter.Write(EndianSwap.Swap(presentationStartTime));
            writter.Write(EndianSwap.Swap(presentationEndTime));

        }
    }
}
