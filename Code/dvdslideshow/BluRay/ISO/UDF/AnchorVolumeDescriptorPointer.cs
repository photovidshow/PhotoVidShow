using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    /// <summary>
    /// This class represents a UDF anchor volume descriptor pointer.
    /// This descriptor is located in the ISO at sector 256 and the very last sector.
    /// </summary>
    public class AnchorVolumeDescriptorPointer : IDescriptor
    {
        //
        // ECMA 167 3/10.2 
        //
        private Tag DescriptorTag;
        private ExtentAD MainVolumeDescriptorSequenceExtent; // MVDS
        private ExtentAD ReserveVolumeDescriptorSequenceExtent; // RVDS
        private byte [] Reserved = new byte[480];

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="RVDSlocation"> </param>
        public AnchorVolumeDescriptorPointer(uint RVDSlocation)
        {
            DescriptorTag = new Tag(Tag.AnchorVolumeDescriptorPointerTag);        

            //
            // MVDS is at sector 32 and is 16 sectors long
            // 
            MainVolumeDescriptorSequenceExtent = new ExtentAD(32, 16 * 2048);  

            //
            // The given RVDS location is normally the sector after the main partition
            //
            ReserveVolumeDescriptorSequenceExtent = new ExtentAD(RVDSlocation, 16 * 2048); 
            DescriptorTag.CalcDescriptorCRC(this);
        }

        /// <summary>
        /// Write the descriptor to the given binary writer
        /// </summary>
        /// <param name="writer"></param>
        public void Write(BinaryWriter writer)
        {
            DescriptorTag.Write(writer,0);
            MainVolumeDescriptorSequenceExtent.Write(writer);
            ReserveVolumeDescriptorSequenceExtent.Write(writer);
            writer.Write(Reserved);
        }
    }
}
