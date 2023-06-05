using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.BDMV
{
    /// <summary>
    /// This class represents an Audio Descriptor
    /// Currently this is only used for debug purposes
    /// </summary>
    public class AudioDescriptor : Descriptor
    {
        private const byte TAG = 0x80;

        /// <summary>
        /// Constructor
        /// </summary>
        public AudioDescriptor() :
            base(TAG)
        {
        }

        /// <summary>
        /// Gets the length of the audio descriptor in bytes
        /// </summary>
        /// <returns></returns>
        public override uint GetLengthInBytes()
        {
            return 6;
        }

        /// <summary>
        /// Writes the audio descriptor to the writer
        /// </summary>
        /// <param name="writer"></param>
        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write((byte)0x31);
            writer.Write((byte)0x40);
            writer.Write((byte)0x7c);
            writer.Write((byte)061);
            writer.Write((byte)0);
        }
    }

}
