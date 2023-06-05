using System;
using System.Collections.Generic;
using System.Text;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    /// <summary>
    /// 16 sectors in size.
    /// First sector starts with "BEA01"
    /// Second sector starts with "NSR03"
    /// Thirs sector starts with "TSR01"
    /// </summary>
    public class VolumeRegconitionSequence
    {
        private byte[] sector1 = { 0x00, 0x42, 0x45, 0x41, 0x30, 0x31, 0x01 }; // "BEA01"
        private byte[] sector2 = { 0x00, 0x4E, 0x53, 0x52, 0x30, 0x33, 0x01 }; // "NSR03";
        private byte[] sector3 = { 0x00, 0x54, 0x45, 0x41, 0x30, 0x31, 0x01 }; // "TEA01";

        public void Write(IsoFile file)
        {
            //
            // Create blank sector
            //
            byte [] buffer = new byte[2048];

            //
            // Write BEA01
            //
            Buffer.BlockCopy(sector1, 0, buffer, 0, 7);
            file.WriteSector(buffer);

            //
            // Write NSR03
            //
            Buffer.BlockCopy(sector2, 0, buffer, 0, 7);
            file.WriteSector(buffer);

            //
            // Write TEA01
            //
            Buffer.BlockCopy(sector3, 0, buffer, 0, 7);
            file.WriteSector(buffer);

            //
            // Remaining sectors are blank
            // 
            file.WriteBlankSectors(13);
        }
    }
}
