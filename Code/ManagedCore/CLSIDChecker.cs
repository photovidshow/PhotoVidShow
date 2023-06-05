using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace ManagedCore
{
    // Class for checking if 'COM' CLSIDs have been registered or not
    public class CLSIDChecker
    {
        public bool IsInstalled(string CLSID_guids)
        {
            List<String> list = new List<String>();
            list.Add(CLSID_guids);
            return IsInstalled(list);
        }
            
        public bool IsInstalled(List<String> CLSID_guids)
        {
            bool installed = true;

            try
            {
                foreach (string guid in CLSID_guids)
                {
                    RegistryKey key = Registry.ClassesRoot.OpenSubKey("CLSID", false);
                    if (key != null)
                    {
                        key = key.OpenSubKey(guid, false);
                        if (key == null)
                        {
                            installed = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Failed to read registry '" + e.Message + "'");
            }

            return installed;
        }
    }
}
