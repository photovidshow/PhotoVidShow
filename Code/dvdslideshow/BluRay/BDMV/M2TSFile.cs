using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.BDMV
{
    /// <summary>
    /// This class is used as a handle to a .mt2s file
    /// </summary>
    public class M2TSFile
    {
        private BinaryWriter writer = null;
        private byte[] bankSector = new byte[2048];

        public BinaryWriter Writer
        {
            get { return writer; }
        }

        /// <summary>
        /// Create a new .m2ts file 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static M2TSFile CreateNewFromFilename(string name)
        {
            M2TSFile file = new M2TSFile();

            if (File.Exists(name) == true)
            {
                File.Delete(name);
            }

            file.writer = new BinaryWriter( new FileStream(name, FileMode.Create));
            return file;
        }

        /// <summary>
        /// Open and amend an existing .m2ts file
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static M2TSFile OpenFromFilename(string name)
        {
            M2TSFile file = new M2TSFile();

            if (File.Exists(name) == false)
            {
                BDMVLog.Error("Could not find file '" + name + "'");
                return null;
            }

            file.writer = new BinaryWriter(new FileStream(name, FileMode.Open));
            return file;
        }

        /// <summary>
        /// Private construcor, use a static method above to create an instance of this class
        /// </summary>
        private M2TSFile()
        {
        }

        /// <summary>
        /// Write/overrite the given bytes to the current position in the file
        /// </summary>
        /// <param name="bytes"></param>
        public void WriteBytes(byte[] bytes)
        {
            writer.Write(bytes);
        }

        /// <summary>
        /// Closes the .m2ts file
        /// </summary>
        public void Close()
        {
            if (writer != null)
            {
                writer.Close();
            }
        }
    }
}
