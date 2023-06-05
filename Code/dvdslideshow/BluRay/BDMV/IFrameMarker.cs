using System;
using System.Collections.Generic;
using System.Text;

namespace DVDSlideshow.BluRay.BDMV
{
    /// <summary>
    /// This class represents a marker in the m2ts file which points to a start of a I-Frame (key frame)
    /// We store:-
    /// - The TS packet number of the start of the I-Frame
    /// - PTS (Presentation Time Stamp) of the I-Frame
    /// - Size of the I-Frame in bytes
    /// </summary>
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="packetNumber"></param>
        /// <param name="presentationTimeStamp"></param>
        /// <param name="sizeInBytes"></param>
        public IFrameMarker(uint packetNumber, long presentationTimeStamp, uint sizeInBytes)
        {
            this.packetNumber = packetNumber;
            this.presentationTimeStamp = presentationTimeStamp;
            this.sizeInBytes = sizeInBytes;
        }
    }
}
