using System;
using System.Collections.Generic;
using System.Text;
using MangedToUnManagedWrapper;

namespace DVDSlideshow
{
    public class CWmaEncoder
    {

        public void Encode(string inputName, string outputName)
        {
            CManagedWMAEncoder mwa = new CManagedWMAEncoder();
            mwa.Encode(inputName, outputName);
        }
    }
}
