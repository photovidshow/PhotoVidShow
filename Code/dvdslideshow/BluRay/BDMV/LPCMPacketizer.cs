using System;
using System.Collections.Generic;
using System.Text;

namespace DVDSlideshow.BluRay.BDMV
{
    /// <summary>
    /// This class can packetize an audio LPCM stream into PES packets.
    /// Currently this class will only work for 48khz 16 bit stereo audio streams
    /// </summary>
    public class LPCMPacketizer : Packetizer
    {
        private byte streamID;
        private int packetSize = 972 -12;   // Copied from txmuxer else 0.005 second of audio at 48khz 16bit stereo
        private long PTS = 54000000;
        private int frameStepTime = 450;    // 0.005 of a second 
        private uint wavHeaderSize;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="LPCMFilename"></param>
        /// <param name="streamID"></param>
        /// <param name="wavHeaderSize"></param>
        public LPCMPacketizer(string LPCMFilename, byte streamID, uint wavHeaderSize) :
            base (LPCMFilename)
        {
            this.wavHeaderSize = wavHeaderSize;
            this.streamID = streamID;

            if (wavHeaderSize != 0)
            {
                byte[] header = PeekBytes(0, (int)wavHeaderSize);
                if (header == null || header.Length != wavHeaderSize)
                {
                    BDMVLog.Error(LPCMFilename + " too small to packetize into PES packets");
                    return;
                }
                MoveIndex(header.Length);
            }
        }

        /// <summary>
        /// Constructor allowing a zero stream
        /// </summary>
        /// <param name="streamID"></param>
        public LPCMPacketizer(byte streamID)
        {
            this.streamID = streamID;
        }

        /// <summary>
        /// Returns the next Audio PES packet, or null if the stream is ended
        /// </summary>
        /// <returns></returns>
        public PES GetNextPacket()
        {
            byte [] nextBytes = PeekBytes(0, packetSize);
            if (nextBytes == null) return null;
            if (nextBytes.Length == 0) return null;

            //
            // Ok sample data needs to be endian swapped.
            //
            int toSwapBytesLength = nextBytes.Length;

            //
            // Make sure length 2 byte aligned
            //
            if (toSwapBytesLength % 2 != 0) toSwapBytesLength--;
            if (toSwapBytesLength >= 2)
            {
                for (int i = 0; i < toSwapBytesLength-1; i+=2)
                {
                    byte temp = nextBytes[i];
                    nextBytes[i] = nextBytes[i + 1];
                    nextBytes[i + 1] = temp;
                }
            }

            MoveIndex(nextBytes.Length);

            PES pes = new PES(streamID, nextBytes, PTS, PTS, true);
            PTS += frameStepTime;

            return pes;
        }
    }
}
