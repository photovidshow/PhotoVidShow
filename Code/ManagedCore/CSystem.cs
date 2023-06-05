using System;
using System.Collections.Generic;
using System.Text;
using System.Management;

namespace ManagedCore
{
    public class CSystem
    {
        //************************************************************************************
        public CSystem()
        {
        }

        //************************************************************************************
        public string GetSystemInfo()
        {
            StringBuilder systemInfoBuilder = new StringBuilder();

            try
            {
                systemInfoBuilder.Append("System information\r\n");
                systemInfoBuilder.Append("==========================================\r\n");
                systemInfoBuilder.AppendFormat("Operating system: {0}, {1}, {2}\r\n", GetInfo("OperatingSystem", "Caption"), GetInfo("OperatingSystem", "CSDVersion"), GetInfo("OperatingSystem", "OSArchitecture"));
                systemInfoBuilder.AppendFormat("Machine make: {0}\r\n", GetInfo("ComputerSystem", "Manufacturer"));
                systemInfoBuilder.AppendFormat("Processor(s) {0} , Cores: {1}\r\n", GetInfo("Processor", "Name"), GetInfo("Processor", "NumberOfCores"));
                systemInfoBuilder.AppendFormat("Memory {0} Mb\r\n", GetTotalComputerMemory() / 1024 / 1024);
                systemInfoBuilder.AppendFormat("Video controller: {0}\r\n", GetInfo("VideoController", "Caption"));
                systemInfoBuilder.AppendFormat("Video ram: {0}\r\n", GetInfo("VideoController", "AdapterRam"));
                systemInfoBuilder.AppendFormat("Video description: {0}\r\n", GetInfo("VideoController", "Description"));
                systemInfoBuilder.AppendFormat("Driver version: {0}\r\n", GetInfo("VideoController", "DriverVersion"));
                systemInfoBuilder.AppendFormat("Driver date: {0}\r\n", GetInfo("VideoController", "DriverDate"));
                systemInfoBuilder.AppendFormat("Driver info: {0} Section: {1}\r\n", GetInfo("VideoController", "InfFilename"), GetInfo("VideoController", "InfSection"));
                systemInfoBuilder.AppendFormat("Video mode: {0}\r\n", GetInfo("VideoController", "VideoModeDescription"));
                systemInfoBuilder.AppendFormat("Video processor: {0}\r\n", GetInfo("VideoController", "VideoProcessor"));
                systemInfoBuilder.Append("==========================================");
            }
            catch (Exception e)
            {
                systemInfoBuilder.Append(e.Message);
                systemInfoBuilder.Append("\r\n");
                systemInfoBuilder.Append(e.StackTrace);
                systemInfoBuilder.Append("\r\n");
            }

            return systemInfoBuilder.ToString();
        }

        //************************************************************************************
        public static bool IsRunningWindows10()
        {
            string r = GetInfo("OperatingSystem", "Caption");
            if (r.Contains("Windows 10 ") == true)
            {
                return true;

            }
            return false;
        }

        //************************************************************************************
        public static void OpenBrowser(string webpage)
        {
            if (ManagedCore.CSystem.IsRunningWindows10() == true)
            {
                webpage = webpage.Replace("http://", "") ;
                System.Diagnostics.Process.Start("microsoft-edge:http://" + webpage);
            }
            else
            {
                System.Diagnostics.Process.Start("IExplore", webpage);
            }
        }

        //************************************************************************************
        public static UInt64 GetTotalComputerMemory()
        {
            string Query = "SELECT Capacity FROM Win32_PhysicalMemory";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(Query);

            UInt64 Capacity = 0;
            foreach (ManagementObject WniPART in searcher.Get())
            {
                Capacity += Convert.ToUInt64(WniPART.Properties["Capacity"].Value);
            }

            return Capacity;
        }

        //************************************************************************************
        private static string GetInfo(string category, string id)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("SELECT ");
            builder.Append(id);
            builder.Append(" FROM Win32_");
            builder.Append(category);

            string searchString = builder.ToString();

            string result = string.Empty;

            try
            {

                ManagementObjectSearcher searcher = new ManagementObjectSearcher(searchString);
                foreach (ManagementObject os in searcher.Get())
                {
                    result = os[id].ToString();
                    break;
                }
            }
            catch
            {
            }

            return result;
        }
    }
}
