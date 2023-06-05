using System;
using System.Collections.Generic;
using System.Text;

namespace DVDSlideshow.BluRay.BDMV
{
    public class VideoStreamProgramMapEntry : ProgramMapEntry
    {
        public VideoStreamProgramMapEntry()
            : base(0x1b, 4113)
        {
            descriptors.Add(new AVCVideoDescriptor());
            //descriptors.Add(new LanguageDescriptor());
        }
    }
}
