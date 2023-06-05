using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    /// <summary>
    /// This class represents a UDF character specification (Charspec)
    /// </summary>
    public class Charspec
    {
        //
        // ECMA 167 1/7.2.1 
        // 64 bytes in length
        //
        private byte CharacterSetType = 0;
        private byte[] CharacterSetInfo = new byte[63];

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        public Charspec(string value)
        {
            if (value.Length > 63)
            {
                UDFLog.Warning("Charspec '"+ value+ "' is too big (truncating to 63 characters)");
                value = value.Substring(0, 63);
            }

            byte[] b2 = System.Text.Encoding.ASCII.GetBytes(value);
            System.Buffer.BlockCopy(b2, 0, CharacterSetInfo, 0, b2.Length);
        }

        /// <summary>
        /// Writes the charspect to the given binary writer
        /// </summary>
        /// <param name="writer"></param>
        public void Write(BinaryWriter writer)
        {
            writer.Write(CharacterSetType);
            writer.Write(CharacterSetInfo);
        }
    }
}


