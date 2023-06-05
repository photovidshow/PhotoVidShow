using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.BDMV
{
    /// <summary>
    /// This class represents the payload data inside a TS packet
    /// </summary>
    public abstract class Payload
    {
        /// <summary>
        /// Writes the payload to the given writer
        /// </summary>
        /// <param name="writer"></param>
        public abstract void Write(BinaryWriter writer);

        /// <summary>
        /// Gets the length of the payload in bytes
        /// </summary>
        /// <returns></returns>
        public abstract uint GetLengthInBytes();

    }
}
