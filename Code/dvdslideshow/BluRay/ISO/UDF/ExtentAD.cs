using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    /// <summary>
    /// This class represents a UDF ExtentAD
    /// </summary>
    public class ExtentAD
    {
        private uint Length;
        private uint Location;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="location"></param>
        /// <param name="length"></param>
        public ExtentAD(uint location, uint length)
        {
            Length = length;
            Location = location;
        }

        /// <summary>
        /// Write the ExtentAD to the given binary writer
        /// </summary>
        /// <param name="writer"></param>
        public void Write(BinaryWriter writer)
        {
            //
            // We write it length then location, which seems back-to-front
            //
            writer.Write(Length);
            writer.Write(Location);
        } 
    }
}
