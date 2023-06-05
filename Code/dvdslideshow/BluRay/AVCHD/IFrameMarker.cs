using System;
using System.Collections.Generic;
using System.Text;

namespace DVDSlideshow.BluRay.AVCHD
{
    public class IFrameMarker
    {      
        private uint packetNumber;
        private long presentationTimeStamp;
        private uint sizeInBytes;

        public uint TsPacketNunber
        {
            get { return packetNumber; }
        }

        public long PresentationTimeStamp
        {
            get { return presentationTimeStamp; }
        }

        public uint FrameSizeInBytes
        {
            get { return sizeInBytes; }
        }

        public IFrameMarker(uint packetNumber, long presentationTimeStamp, uint sizeInBytes)
        {
            this.packetNumber = packetNumber;
            this.presentationTimeStamp = presentationTimeStamp;
            this.sizeInBytes = sizeInBytes;
        }       
    }
}
