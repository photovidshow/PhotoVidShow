using System;
using System.Collections.Generic;
using System.Text;

namespace DVDSlideshow.BluRay.AVCHD
{
    public class File
    {
        public static void Copy(string source, string dest)
        {
            try
            {
                System.IO.File.Copy(source, dest, true);
            }
            catch (Exception exception)
            {
                AVCHDLog.Error("Failed to copy file '" + source + "' " + exception.Message);
            }
        }

        public static void Move(string source, string dest)
        {
            try
            {
                System.IO.File.Move(source, dest);
            }
            catch (Exception exception)
            {
                AVCHDLog.Error("Failed to move file '" + source + "' " + exception.Message);
            }
        }
    }
}
