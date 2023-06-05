using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.BDMV
{
    /// <summary>
    /// This class represents a LPCM registration descriptor
    /// </summary>
    public class LPCMRegistrationDescriptor : Descriptor
    {
        private uint asciicode;
        private uint extra;

        private const byte TAG = 5;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ascii"></param>
        /// <param name="extra"></param>
        public LPCMRegistrationDescriptor(uint ascii, uint extra)
            : base(TAG)
        {
            asciicode = ascii;
            this.extra = extra;
        }

        /// <summary>
        /// Gets the length of the LPCM registration descriptor in bytes 
        /// </summary>
        /// <returns></returns>
        public override uint GetLengthInBytes()
        {
            return 10;
        }

        /// <summary>
        /// Writes the LPCM registration descriptor to the given writer
        /// </summary>
        /// <param name="writer"></param>
        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write((byte)8); // length
            writer.Write(asciicode);
            writer.Write(extra);
        }
    }
}
