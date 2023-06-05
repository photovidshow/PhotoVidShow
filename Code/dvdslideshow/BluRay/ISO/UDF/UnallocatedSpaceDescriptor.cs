using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    public class UnallocatedSpaceDescriptor : IDescriptor
    {
        private Tag Descriptor;
        private uint VolumeDescriptorSequenceNumber;
        private uint NumberOfAllocationDescriptor;
        // AllocationDescritor;

        public UnallocatedSpaceDescriptor()
        {
            Descriptor = new Tag(Tag.UnallocatedSpaceDescriptorTag);
            VolumeDescriptorSequenceNumber = 4;
            NumberOfAllocationDescriptor = 0;

            Descriptor.CalcDescriptorCRC(this);
        }

        public void Write(BinaryWriter writer)
        {
            Descriptor.Write(writer,0);
            writer.Write(VolumeDescriptorSequenceNumber);
            writer.Write(NumberOfAllocationDescriptor);
        }
    }
}
