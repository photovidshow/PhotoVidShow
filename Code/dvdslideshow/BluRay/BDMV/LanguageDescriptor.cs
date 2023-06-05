using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.BDMV
{
    /// <summary>
    /// This class represents a language descriptor
    /// This is currently only used in debug situations
    /// </summary>
    public class LanguageDescriptor : Descriptor
    {
        private const byte TAG = 0xa;

        /// <summary>
        /// Constructor
        /// </summary>
        public LanguageDescriptor()
            : base(TAG)
        {
        }

        /// <summary>
        /// Gets the length of the language descriptor in bytes
        /// </summary>
        /// <returns></returns>
        public override uint GetLengthInBytes()
        {
            return 6;
        }

        /// <summary>
        /// Writes the langauge descriptor to the writer
        /// </summary>
        /// <param name="writer"></param>
        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            //
            // Used for debug purpose only at the momement (english)
            //
            writer.Write((byte)4);
            writer.Write((byte)0x65);   // e
            writer.Write((byte)0x6e);   // n 
            writer.Write((byte)0x67);   // g
            writer.Write((byte)0x00);   // null
        }
    }
}
