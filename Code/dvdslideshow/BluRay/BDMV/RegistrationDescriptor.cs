using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.BDMV
{
    public class RegistrationDescriptor : Descriptor
    {
        private uint asciicode;

        public RegistrationDescriptor(uint ascii)
            : base(5)
        {
            asciicode = ascii;
        }

        public override uint GetLengthInBytes()
        {
            return 6;
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write((byte)4); // length
            writer.Write(asciicode);
        }
    }
}
