using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    /// <summary>
    /// This class represents a UDF ICBTag
    /// </summary>
    public class ICBTag
    {
        public enum ICBTagType
        {
            MetaData = 0,
            MetaDataMirror,
            Directory,
            File
        }

        private uint PriorRecordedNumberOfDirectEntries;
        private ushort Strategy;
        private ushort StrategyParameter;
        private ushort MaximumNumberOfEntries;
        private byte Reserved;
        private byte FileType;
        private LbAddr ParentICBLocation;
        private ushort Flags;

        /// <summary>
        /// Get the ICB file type
        /// </summary>
        public byte ICBFileType
        {
            set { FileType = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public ICBTag(ICBTagType type)
        {
            PriorRecordedNumberOfDirectEntries = 0;
            Strategy = 4;
            StrategyParameter = 0;
            MaximumNumberOfEntries = 1;
            Flags = 32; 

            switch (type)
            {
                case ICBTagType.MetaData:
                {
                    FileType = 250;
                    break;
                }
                case ICBTagType.MetaDataMirror:
                {
                    FileType = 251;
                    break;
                }
                case ICBTagType.Directory:
                {
                    FileType = 4;
                    Flags |= 0x210;      // Archived / Contiguous
                    break;
                }
                case ICBTagType.File:
                {
                    FileType = 5;
                    Flags |= 0x211;      // Archived / Contiguous / LongAD used  
                    break;
                }
            }
    
            Reserved = 0;
            ParentICBLocation = new LbAddr(0, 0);
        }

        /// <summary>
        /// Write the ICBTag to the given binary writer
        /// </summary>
        /// <param name="writer"></param>
        public void Write(BinaryWriter writer)
        {
            writer.Write(PriorRecordedNumberOfDirectEntries);
            writer.Write(Strategy);
            writer.Write(StrategyParameter);
            writer.Write(MaximumNumberOfEntries);
            writer.Write(Reserved);
            writer.Write(FileType);
            ParentICBLocation.Write(writer);
            writer.Write(Flags);
        }
    }
}
