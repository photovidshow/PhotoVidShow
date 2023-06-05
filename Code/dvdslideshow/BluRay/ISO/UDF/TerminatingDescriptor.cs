using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    public class TerminatingDescriptor : IDescriptor
    {
        private Tag Descriptor;
        private byte[] Reserved = new byte[496];
        private uint Start;

        public TerminatingDescriptor(uint start)
        {
            Descriptor = new Tag(Tag.TerminatingDescriptorTag);
            Descriptor.CalcDescriptorCRC(this);
            Start = start;
        }

        public void Write(BinaryWriter writer)
        {
            Descriptor.Write(writer, Start);
            writer.Write(Reserved);
        }
    }
}
