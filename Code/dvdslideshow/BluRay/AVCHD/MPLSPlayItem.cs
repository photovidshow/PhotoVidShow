using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.AVCHD
{
    public class MPLSPlayItem
    {
        private byte[] clipId = new byte[5];
        private byte reserved ;
        private byte multiAngle;          // 1 bit
        private byte connectionCondition; // 4 bits;

        private byte stcId;
        private uint inTime;
        private uint outTime;

        private ushort reserved2;

        // STN
        private byte numVideo;
        private byte numAudio;
        private byte numPg;
        private byte numIg;
        private byte numSecondaryAudio;
        private byte numSecondaryVideo;
        private byte numPipPg;

        // Video stream
        private byte videoStreamType;
        private ushort videoStreamPID;
        private byte videoCodingType;
        private byte videoCodingFormat;  // 4 bits;
        private byte videoCodingRate;    // 4 bits;

        // Audio stream
        private byte audioStreamType;
        private ushort audioStreamPID;
        private byte audioCodingType;
        private byte audioCodingFormat;  // 4 bits;
        private byte audioCodingRate;    // 4 bits;


        public MPLSPlayItem(string identifier, M2TSVideo video)
        {
            for (int i = 0; i < 5; i++)
            {
                clipId[i] = (byte) (identifier[i]);
            }

            reserved = 0;
            multiAngle = 0;
            connectionCondition = 1;
            stcId = 0;

            inTime =  0x019bfcc0;   // Always 10 mins
            double end = ((double)video.LengthInSeconds) * 45000;   //  45000 = 1 second; 
            end += inTime; // 10 mins + length
            outTime = (uint)end;   

            reserved2 = 0;
            numVideo = 1;
            numAudio = 0;
            if (video.ContainsAudio == true)
            {
                numAudio = 1;
            }

            numPg = 0;
            numIg = 0;
            numSecondaryAudio = 0;
            numSecondaryVideo = 0;
            numPipPg = 0;

            //
            // Video stream
            //
            videoStreamType = 1;
            videoStreamPID = 0x1011;
            videoCodingType = 0x1b;     // AVC 
            videoCodingFormat = (byte)video.Resolution;     
            videoCodingRate = (byte)video.FPS;      

            //
            // Auido stream
            //
            audioStreamType = 1;
            audioStreamPID = 0x1100;
            audioCodingType = 0x80;     // Auido
            audioCodingFormat = 3;      // LPCM
            audioCodingRate = 1;       // 48Khz stereo

        }

        public uint GetLengthInBytes()
        {
            // 
            // Fixed numbers based on if there is an audio stream
            //
            if (numAudio == 0)
            {
                return 0x42;
            }
            return 0x52;
        }

        public void Write(BinaryWriter writter)
        {
            ushort length = (ushort)(GetLengthInBytes() -2) ;    
            writter.Write(EndianSwap.Swap(length));

            //
            // Write identifer (5 bytes)
            //
            for (int i = 0; i < clipId.Length; i++)
            {
                writter.Write(clipId[i]);
            }

            //
            // Write m2ts
            //
            uint m2ts = 0x5354324d;    // 'M2TS'
            writter.Write(m2ts);

            writter.Write(reserved);

            byte nextByte = (byte)(multiAngle << 4);
            nextByte |= (byte)(connectionCondition);
            writter.Write(nextByte);

            writter.Write(stcId);

            writter.Write(EndianSwap.Swap(inTime));
            writter.Write(EndianSwap.Swap(outTime));

            //
            // Skip UO_mask_table, random_access_flag, reserved, still_mode and still_time
            //
            for (int i = 0; i < 12; i++)
            {
                writter.Write((byte)0);
            }

            //
            // Write stn length ( 16 bits) 
            //
            ushort stnLength = 30;
            if (numAudio == 1)
            {
                stnLength += 16;
            }
            writter.Write(EndianSwap.Swap(stnLength));

            writter.Write(reserved2);
            writter.Write(numVideo);
            writter.Write(numAudio);
            writter.Write(numPg);
            writter.Write(numIg);
            writter.Write(numSecondaryAudio);
            writter.Write(numSecondaryVideo);
            writter.Write(numPipPg);

            //
            // 5 reserved bytes
            //
            for (int i = 0; i < 5; i++)
            {
                writter.Write((byte)0);
            }

            //
            // Write video stream
            //
            writter.Write((byte)9);    // Write length fixed
            writter.Write(videoStreamType);
            writter.Write(EndianSwap.Swap(videoStreamPID));

            for (int i = 0; i < 6; i++)
            {
                writter.Write((byte)0);
            }

            //
            // Write video stream coding type
            //
            writter.Write((byte)5);   // Write length fixed
            writter.Write(videoCodingType);
            nextByte = (byte)(videoCodingFormat << 4);
            nextByte |= (byte)(videoCodingRate);
            writter.Write(nextByte);
            for (int i = 0; i < 3; i++)
            {
                writter.Write((byte)0);
            }

            if (numAudio == 1)
            {
                //
                // Write audio stream
                //
                writter.Write((byte)9);    // Write length fixed
                writter.Write(audioStreamType);
                writter.Write(EndianSwap.Swap(audioStreamPID));

                for (int i = 0; i < 6; i++)
                {
                    writter.Write((byte)0);
                }

                //
                // Write audio stream coding type
                //
                writter.Write((byte)5);   // Write length fixed
                writter.Write(audioCodingType);
                nextByte = (byte)(audioCodingFormat << 4);
                nextByte |= (byte)(audioCodingRate);
                writter.Write(nextByte);
                for (int i = 0; i < 3; i++)
                {
                    writter.Write((byte)0);
                }
            }
        }
    }
}
