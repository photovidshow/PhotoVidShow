using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    public class LongAd : AllocationDescriptor
    {
        // 
        // ECMA 167 4/14.14.2 
        // Total 16 bytes
        //
        private uint  extentLength;
        private LbAddr extentLocation;

        // OSTA 2.3.4.3
        private ushort Flags = 0;
        private uint UDFUniqueID = 0;

        public override uint TotalLength
        {
            get { return extentLength; }
        }

        public override uint SectorLocation
        {
            get { return extentLocation.LogicalBlockNumber; }
            set { extentLocation.LogicalBlockNumber = value; }
        }

        public uint ExtentLength
        {
            get { return extentLength; }
        }

        public LbAddr ExtentLocation
        {
            get { return extentLocation; }
        }

        public uint UniqueID
        {
            get { return UDFUniqueID; }
            set { UDFUniqueID=value; }
        }

        public override uint AllocationDescriptorSizeInBytes
        {
            get { return 16; }
        }

        public LongAd(uint extentLength, LbAddr location)
        {
            this.extentLength = extentLength;
            this.extentLocation = location;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(extentLength);
            extentLocation.Write(writer);
            writer.Write(Flags);
            writer.Write(UDFUniqueID);
        }
    }
}
