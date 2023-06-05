using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    public abstract class USDLinkedEntry : UDFFileSystemEntry
    {
        private UDFFileSystemEntry SubLinkedEntry = null;

        public UDFFileSystemEntry LinkedEntry
        {
            get { return SubLinkedEntry; }
            set { SubLinkedEntry = value; }
        }

        public USDLinkedEntry(ShortAd address) :
            base(address)
        {
        }
    }
}
