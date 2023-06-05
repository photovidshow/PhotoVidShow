using System;
using System.Collections.Generic;
using System.Text;

namespace DVDSlideshow
{
    public class CRunExternalDVDBurnTool
    {
        public static void RunTool()
        {
            System.Diagnostics.Process mCurrentProcess = new System.Diagnostics.Process();
            string exe_name = Environment.GetCommandLineArgs()[0];

            string path = System.IO.Path.GetDirectoryName(exe_name);
            exe_name = path + "//DVDVideoBurnerTool//DVD Video Burner.exe";

            mCurrentProcess.StartInfo.FileName = exe_name;
  
            mCurrentProcess.Start();
        }
    }
}
