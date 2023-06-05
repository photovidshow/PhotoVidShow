using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.AVCHD
{
    public class ClipInfoStream
    {
        private ushort pid;
        private byte coding_type;
        private byte format;          // 4 bits
        private byte rate;            // 4 bits
        private byte aspect;          // 4 bits
        private byte reserved;        // 2 bits
        private byte oc_flag;         // 1 bit
        private byte reserved1;       // 1 bits

        public ClipInfoStream(ushort pid, M2TSVideo video)
        {
            this.pid = pid;


            if (video != null)
            {
                coding_type = 0x1b;     // AVC (h264)
                format = (byte)video.Resolution;    
                rate = (byte)video.FPS;      
                aspect = (byte)video.Aspect;     // 16:9 
            }
            else
            {
                coding_type = 0x80; // AUDIO
                format = 3;     // LPCM 
                rate = 1;        // 48khz stereo
                aspect = 0;     // not used
            }

            reserved =0;
            oc_flag=0;
            reserved1 =0;
        }

        public uint GetLengthInBytes()
        {
            return 0x18;  // 0x15 + pid (2byte) + length (1byte)
        }

        public void Write(BinaryWriter writter)
        {
            writter.Write(EndianSwap.Swap(pid));
            byte length = (byte)0x15; // fixed
            writter.Write(length);
            writter.Write(coding_type);

            byte nextByte = (byte)(format<<4);
            nextByte |= rate;
            writter.Write(nextByte);

            nextByte = (byte)(aspect << 4);
            nextByte |= (byte)(reserved << 2);
            nextByte |= (byte)(oc_flag << 1);
            nextByte |= reserved1;
            writter.Write(nextByte);

            // ???
            writter.Write((byte)0);
            writter.Write((byte)0);
            for (int i = 0; i < 12; i++)
            {
                writter.Write((byte)0x30);
            }
            writter.Write((byte)0);
            writter.Write((byte)0);
            writter.Write((byte)0);
            writter.Write((byte)0);

        }
    }

}
