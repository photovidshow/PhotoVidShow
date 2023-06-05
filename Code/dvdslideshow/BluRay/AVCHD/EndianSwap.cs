using System;
using System.Collections.Generic;
using System.Text;

namespace DVDSlideshow.BluRay.AVCHD
{
    public class EndianSwap
    {
        public static ushort Swap(ushort value)
        {
            ushort swapped = (ushort)(value >> 8);
            swapped |= (ushort)(value << 8);
            return swapped;
        }

        public static uint Swap(uint value)
        {
            return ((value & 0x000000ff) << 24) +
                   ((value & 0x0000ff00) << 8) +
                   ((value & 0x00ff0000) >> 8) +
                   ((value & 0xff000000) >> 24);
        }

        public static ulong Swap(ulong value)
        {
            ulong uvalue = value;
            ulong swapped =
                 ( (0x00000000000000FF) & (uvalue >> 56)
                 | (0x000000000000FF00) & (uvalue >> 40)
                 | (0x0000000000FF0000) & (uvalue >> 24)
                 | (0x00000000FF000000) & (uvalue >> 8)
                 | (0x000000FF00000000) & (uvalue << 8)
                 | (0x0000FF0000000000) & (uvalue << 24)
                 | (0x00FF000000000000) & (uvalue << 40)
                 | (0xFF00000000000000) & (uvalue << 56));
            return swapped;
        }

    } 
}
