using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    /// <summary>
    /// Abstract class representing a generic UDF allocation descriptor
    /// </summary>
    public abstract class AllocationDescriptor
    {
        /// <summary>
        /// Writes the allocation descriptor to the given binary writer
        /// </summary>
        /// <param name="writer"></param>
        public abstract void Write(BinaryWriter writer);

        /// <summary>
        /// Length of the allocation
        /// </summary>
        public abstract uint TotalLength
        {
            get;
        }

        /// <summary>
        /// Sector location of the allocation
        /// </summary>
        public abstract uint SectorLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Number of bytes this allocation descriptor class uses in memory
        /// </summary>
        public abstract uint AllocationDescriptorSizeInBytes
        {
            get;
        }
    }
}
