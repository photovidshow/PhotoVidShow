using System;
using System.Collections.Generic;
using System.Text;

namespace DVDSlideshow.BluRay.BDMV
{
    /// <summary>
    /// This class represents an audio stream program map entry
    /// </summary>
    public class AudioStreamProgramMapEntry : ProgramMapEntry
    {
        private const byte streamID =0x80;
        private const ushort PID = 4352;
        private const uint HDMIstring = 0x564d4448;
        private const uint LPCMstring = 0x40318000;

        /// <summary>
        /// Constructor
        /// </summary>
        public AudioStreamProgramMapEntry()
            : base(streamID, PID)
        {
            descriptors.Add(new LPCMRegistrationDescriptor(HDMIstring, LPCMstring));  //  HDMI, LPCM 48khz 16 bit stereo

            //
            // Debug: Other types of audio descritors currently not needed for blu-ray 
            // e.g. ac3 audio, or language descriptor
            //
            //  descriptors.Add(new AudioDescriptor());
            //  descriptors.Add(new LanguageDescriptor());
            //
        }
    }
}
