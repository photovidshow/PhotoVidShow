using System;
using System.Collections.Generic;
using System.Text;

namespace DVDSlideshow.BluRay.BDMV
{
    /// <summary>
    /// This class provides local support for endian swap methods
    /// </summary>
    public class EndianSwap
    {
        /// <summary>
        /// Swap ushort byte order
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ushort Swap(ushort value)
        {
            ushort swapped = (ushort)(value >> 8);
            swapped |= (ushort)(value <<8);
            return swapped;
        }

        /// <summary>
        /// Swap uint byte order
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static uint Swap(uint value)
        {
            return ((value & 0x000000ff) << 24) +
                   ((value & 0x0000ff00) << 8) +
                   ((value & 0x00ff0000) >> 8) +
                   ((value & 0xff000000) >> 24);
        }

    }
}
