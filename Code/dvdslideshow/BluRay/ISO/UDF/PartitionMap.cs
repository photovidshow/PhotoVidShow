using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    public abstract class PartitionMap
    {
        public abstract int LengthInBytes();
        public abstract void Write(BinaryWriter writer);
    }
}
