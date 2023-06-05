using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    public class LogicalVolumeInformation
    {
        //
        // OSTA 2.2.7.2 bytes ImplementationUse[460]
        //
        private Charspec LVICharset;
        private DString LogicalVolumeIdentifier;
        private DString LVInfo1;
        private DString LVInfo2;
        private DString LVInfo3;
        private EntityID ImplementationID;
        private byte[] ImplementationUse = new byte[128];

        public LogicalVolumeInformation(string volumeIdentifier, string applicationName)
        {
            LVICharset = new Charspec("OSTA Compressed Unicode");
            LogicalVolumeIdentifier = new DString(volumeIdentifier, 128);      
            LVInfo1 = new DString("", 36);
            LVInfo2 = new DString("", 36);
            LVInfo3 = new DString("", 36);

            ImplementationID = new EntityID("*" + applicationName, 0, false);   // OSTA says have leading '*'
        }

        public void Write(BinaryWriter writer)
        {
            LVICharset.Write(writer);
            LogicalVolumeIdentifier.Write(writer);
            LVInfo1.Write(writer);
            LVInfo2.Write(writer);
            LVInfo3.Write(writer);
            ImplementationID.Write(writer);
            writer.Write(ImplementationUse);
        }
    }
}
