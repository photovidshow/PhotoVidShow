
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    public class ShortAd : AllocationDescriptor
    {
        private uint Length;    // bytes
        private uint Location;  // sector

        public override uint TotalLength
        {
            get { return Length; }
        }

        public override uint SectorLocation
        {
            get { return Location; }
            set { Location = value; }
        }

        public ShortAd(uint location, uint length)
        {
            Length = length;
            Location = location;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Length);
            writer.Write(Location);
        }

        public override uint AllocationDescriptorSizeInBytes
        {
            get { return 8; }
        }

    }
}
