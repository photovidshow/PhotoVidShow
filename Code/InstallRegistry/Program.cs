using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;     

namespace InstallRegistry
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0) return;

                if (args[0] == "-r")
                {
                    string exe_path = args[1];
                    for (int i = 2; i < args.Length; i++)
                    {
                        exe_path += " " + args[i];
                    }

                    RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);

                    key.CreateSubKey("PhotoVidShow");
                    key = key.OpenSubKey("PhotoVidShow", true);


                    key.CreateSubKey("Launch");
                    key = key.OpenSubKey("Launch", true);

                    key.SetValue("folder", exe_path);
                }  
            }
            catch
            {
            }
        }
    }
}
