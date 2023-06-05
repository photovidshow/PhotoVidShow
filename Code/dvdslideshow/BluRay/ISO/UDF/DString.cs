using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    /// <summary>
    /// This class represents a UDF DString.  This class creates UNI code charcaters
    /// </summary>
    public class DString
    {
        private byte[] byteDString;

        /// <summary>
        /// Constructor, given a string value and the max size of the container for the dstring.
        /// The max length must be at least 3 bytes bigger than twice the length of the input string 
        /// as this gets converted into uni code.
        /// By default the last byte gets set to the string length and the first two byte are set to
        /// 0x10 0x00
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLength"></param>
        public DString(string value, int maxLength)
        {
             Initialise(value, maxLength, true);
        }

        /// <summary>
        /// This is the same as the default constructor but allowas to not to use the last
        /// byte is the string length.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLength"></param>
        /// <param name="lastLengthOnLastByte"></param>
        public DString(string value, int maxLength, bool lastLengthOnLastByte)
        {
            Initialise(value, maxLength, lastLengthOnLastByte);
        }

        /// <summary>
        /// Private initialise method used by both constructors
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLength"></param>
        /// <param name="lastLengthOnLastByte"></param>
        private void Initialise(string value, int maxLength, bool lastLengthOnLastByte)
        {
            byteDString = new byte[maxLength];

            if (value != "")
            {
                //
                // If value set, first two bytes have to be 0x10 0x00  (not totally sure why)
                //
                byteDString[0] = 0x10;
                byteDString[1] = 0;

                UnicodeEncoding coding = new UnicodeEncoding();

                //
                // Ensure string fits inside dstring else truncate
                //
                int maxSize = maxLength - 2;
                if (lastLengthOnLastByte==true)
                {
                    maxSize--;
                }
                if (value.Length > maxSize/2)
                {
                    UDFLog.Warning("String to big for dstring, truncating");
                    value = value.Substring(0,maxSize /2);
                }

                coding.GetBytes(value, 0, value.Length, byteDString, 2);

                if (lastLengthOnLastByte == true)
                {
                    //
                    // last byte is length of string
                    //
                    int count = byteDString.Length - 1;
                    for (int i = count - 1; i >= 0; i--)
                    {
                        if (byteDString[i] != 0)
                        {
                            break;
                        }
                        count--;
                    }
                    byteDString[byteDString.Length - 1] = (byte)count;
                }
            } 
        }

        /// <summary>
        /// Returns the size of the dstring container; this might not the length of the string.
        /// </summary>
        /// <returns></returns>
        public int GetLengthInBytes()
        {
            return byteDString.Length;
        }

        /// <summary>
        /// Write the dstring to the given binary writer
        /// </summary>
        /// <param name="writer"></param>
        public void Write(BinaryWriter writer)
        {
            writer.Write(byteDString);
        }
    }
}
