using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO
{
    public class IsoFile
    {
        private BinaryWriter writer = null;
        private byte[] bankSector = new byte[2048];

        public BinaryWriter Writer
        {
            get { return writer; }
        }

        public static IsoFile CreateNewFromFilename(string name)
        {
            IsoFile file = new IsoFile();

            if (File.Exists(name) == true)
            {
                File.Delete(name);
            }

            file.writer = new BinaryWriter( new FileStream(name, FileMode.Create));
            return file;
        }

        private IsoFile()
        {
        }

        public void WriteBlankSectors(int numberSectors)
        {
            if (writer == null)
            {
                return;
            }

            for (int i = 0; i < numberSectors; i++)
            {
                writer.Write(bankSector);
            }
        }

        public void WriteSector(byte[] sector)
        {
           writer.Write(sector);
        }

        public void WriteBytes(byte[] bytes)
        {
            writer.Write(bytes);
        }

        public void Close()
        {
            if (writer != null)
            {
                writer.Close();
            }
        }
    }
}
