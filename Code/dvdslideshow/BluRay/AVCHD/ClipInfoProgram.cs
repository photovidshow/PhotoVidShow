using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.AVCHD
{
    public class ClipInfoProgram
    {
        private uint spnProgramSequenceStart;
        private ushort programMapPid;
        private byte numStreams;
        private byte numGroups;

        ClipInfoStream videoStream;
        ClipInfoStream audioStream;

        public ClipInfoProgram(bool includeAudioStream, M2TSVideo video)
        {
            spnProgramSequenceStart = 0;
            programMapPid = 0x100;

            if (includeAudioStream == false)
            {
                numStreams = 1;
            }
            else
            {
                numStreams = 2;
                audioStream = new ClipInfoStream(0x1100, null);
            }

            numGroups=0;
            videoStream = new ClipInfoStream(0x1011, video);
        }

        public uint GetLengthInBytes()
        {
            uint length = 8;
            length += videoStream.GetLengthInBytes();
            if (audioStream != null)
            {
                length += audioStream.GetLengthInBytes();
            }
            return length;
        }

        public void Write(BinaryWriter writter)
        {
            writter.Write(EndianSwap.Swap(spnProgramSequenceStart));
            writter.Write(EndianSwap.Swap(programMapPid));
            writter.Write(numStreams);
            writter.Write(numGroups);
            videoStream.Write(writter);
            if (audioStream != null)
            {
                audioStream.Write(writter);
            }
        } 
    }

}
