using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    /// <summary>
    /// This is the interface for a UDF descriptor
    /// </summary>
    public interface IDescriptor
    {
        void Write(BinaryWriter writer);
    }
}
