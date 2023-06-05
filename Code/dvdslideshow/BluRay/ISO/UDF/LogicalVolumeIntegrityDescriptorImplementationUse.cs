using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    public class LogicalVolumeIntegrityDescriptorImplementationUse
    {
        // Implementation use is descriped as the following
        private EntityID ImplementationID;
        private uint NumberOfFiles;
        private uint NumberOfDirectories;
        private ushort MinimumUDFReadRevision;
        private ushort MinimumUDFWriteRevision;
        private ushort MaximumUDFWriteRevision;

        public LogicalVolumeIntegrityDescriptorImplementationUse(uint numberOfFiles, uint numberOfDirectories, string applicationName)
        {
            ImplementationID = new EntityID("*" + applicationName, 0, false);
            NumberOfFiles = numberOfFiles;      
            NumberOfDirectories = numberOfDirectories; 
            MinimumUDFReadRevision = 0x250;     // v2.5
            MinimumUDFWriteRevision = 0x250;
            MaximumUDFWriteRevision = 0x250;
        }

        public void Write(BinaryWriter writer)
        {
            ImplementationID.Write(writer);
            writer.Write(NumberOfFiles);
            writer.Write(NumberOfDirectories);
            writer.Write(MinimumUDFReadRevision);
            writer.Write(MinimumUDFWriteRevision);
            writer.Write(MaximumUDFWriteRevision);
        }

    }
}
