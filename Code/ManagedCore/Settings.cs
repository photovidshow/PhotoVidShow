using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace ManagedCore
{
    // Class for storing settings, each product must define their own sub class of this
    public class Settings
    {
        //
        // This specifies if the file is an xml or an ini
        //
        public enum FileType 
        {
            Ini, Xml
        }

        //
        // This is the main Table
        //
        public DataTable _settingsTable;

        //
        // This is the filename that was loaded
        //
        private string _fileName = ""; 

         // *********************************************************************************************************************************
        public Settings() //creates the settings
        {
            initializeDataTable();
        }

        // *********************************************************************************************************************************
        public void LoadFromFile(string file, FileType ft) //loads settings from a file (xml or ini)
        {
            _fileName = Path.GetFullPath(file); //saves the filename for future use

            if (File.Exists(_fileName) == false)
            {
                return;
            }

            if (ft == FileType.Ini)
                LoadFromIni();
            else
                LoadFromXml();
        }

        // *********************************************************************************************************************************
        public bool EntryExists(string category, string key)
        {
            foreach (DataRow row in _settingsTable.Rows)
            {
                if ((string)row[0] == category && (string)row[1] == key)
                {
                    return true;
                }
            }
            return false;
        }

        // *********************************************************************************************************************************
        public void AddValue(string category, string key, string value, bool overwriteExisting) 
        {
            if (overwriteExisting)
            {
                foreach (DataRow row in _settingsTable.Rows)
                {
                    if ((string)row[0] == category && (string)row[1] == key)
                    {
                        row[2] = value;
                        return;
                    }
                }
            }
       
            _settingsTable.Rows.Add(category, key, value);
        }

        // *********************************************************************************************************************************
        public string GetValue(string category, string key)
        {
            foreach (DataRow row in _settingsTable.Rows)
            {
                if ((string)row[0] == category && (string)row[1] == key)
                {
                    return (string)row[2];
                }
            }

            Log.Error("Can not get value from missing settings entry, category:" + category + " key:" + key);
            return "";
        }

        // *********************************************************************************************************************
        public void Save(FileType ft) 
        {
            //
            // Sorts the table for saving
            //
            if (_fileName == "")
            {
                return;
            }

            DataView dv = _settingsTable.DefaultView;
            dv.Sort = "Category asc";
            DataTable sortedDT = dv.ToTable();

            if (ft == FileType.Xml)
            {
                sortedDT.WriteXml(_fileName);
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(_fileName))
                {
                    string lastCategory = "";

                    foreach (DataRow row in sortedDT.Rows)
                    {
                        if ((string)row[0] != lastCategory)
                        {
                            lastCategory = (string)row[0];
                            sw.WriteLine("[" + lastCategory + "]");
                        }

                        sw.WriteLine((string)row[1] + "=" + (string)row[2]);
                    }
                }
            }
        }

        // *********************************************************************************************************************
        public void Save(string file, FileType ft) 
        {
            //
            // Saves the filename for future use
            //
            _fileName = Path.GetFullPath(file);

            Save(ft);
        }

        // *********************************************************************************************************************
        private void LoadFromIni() //loads settings from ini
        {
            if (File.Exists(_fileName) == false)
            {
                return;
            }

            //
            // Stream reader that will read the settings
            //
            using (StreamReader sr = new StreamReader(_fileName)) 
            {
                //
                // Holds the category we're at
                //
                string currentCategory = ""; 

                while (sr.EndOfStream == false)
                {
                    string currentLine = sr.ReadLine();

                    if (currentLine.Length < 3)
                    {
                        continue;
                    }

                    //
                    // Checks if the line is a category marker
                    //
                    if (currentLine.StartsWith("[") && currentLine.EndsWith("]")) 
                    {
                        currentCategory = currentLine.Substring(1, currentLine.Length - 2);
                        continue;
                    }

                    //
                    // Or an actual setting
                    //
                    if (currentLine.Contains("=") == false)
                    {
                        continue;
                    }

                    string currentKey = currentLine.Substring(0, currentLine.IndexOf("=", StringComparison.Ordinal));

                    string currentValue = currentLine.Substring(currentLine.IndexOf("=", StringComparison.Ordinal) + 1);

                    AddValue(currentCategory, currentKey, currentValue, true);
                }
            }
        }

        // *********************************************************************************************************************
        private void LoadFromXml() 
        {
            //
            // Loads the settings from an xml file
            //
            _settingsTable.ReadXml(_fileName);
        }
        
        // *********************************************************************************************************************
        private void initializeDataTable() 
        {
            //
            //  Re-initializes the table with the proper columns
            //
            _settingsTable = new DataTable { TableName = "Settings" };

            _settingsTable.Columns.Add("Category", typeof(string));
            _settingsTable.Columns.Add("SettingKey", typeof(string));
            _settingsTable.Columns.Add("SettingsValue", typeof(string));
        }
    }
}

