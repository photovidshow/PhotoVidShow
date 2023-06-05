using System;
using System.Collections.Generic;
using System.Text;

namespace ManagedCore
{
    /// <summary>
    /// This class represents the global UserSettings and should be treated as a singleton
    /// Calling any set method will force a save to happen
    /// </summary>
    public class UserSettings : Settings
    {
        //
        // Global static instance
        //
        protected static UserSettings _settings;

        // *********************************************************************************************************************************
        public static UserSettings GetInstance()
        {
            if (_settings == null)
            {
                _settings = new UserSettings();
            }
            return _settings;
        }

        // *********************************************************************************************************************************
        public static void SetBoolValue(string category, string key, bool value)
        {
            Settings settings = GetInstance();
            settings.AddValue(category, key, value.ToString(), true);
            settings.Save(Settings.FileType.Xml);
        }

        // *********************************************************************************************************************************
        public static void SetIntValue(string category, string key, int value) 
        {
            Settings settings = GetInstance();
            settings.AddValue(category, key, value.ToString(), true);
            settings.Save(Settings.FileType.Xml);
        }

        // *********************************************************************************************************************************
        public static void SetStringValue(string category, string key, string value) 
        {
            Settings settings = GetInstance();
            settings.AddValue(category, key, value, true);
            settings.Save(Settings.FileType.Xml);
        }

        // *********************************************************************************************************************************
        public static void SetFloatValue(string category, string key, float value)
        {
            Settings settings = GetInstance();
            settings.AddValue(category, key, value.ToString(), true);
            settings.Save(Settings.FileType.Xml);
        }

        // *********************************************************************************************************************************
        public static string GetStringValue(string category, string key) 
        {
            Settings settings = GetInstance();
            return settings.GetValue(category, key);
        }

        // *********************************************************************************************************************************
        public static bool GetBoolValue(string category, string key) 
        {
            Settings settings = GetInstance();

            string value = settings.GetValue(category, key);
            if (value != "")
            {
                try
                {
                    return bool.Parse(value);
                }
                catch
                {
                    Log.Error("Settings value is not of type bool, category:" + category + " key:" + key);
                }
            }
            return false;
        }

        // *********************************************************************************************************************************
        public static int GetIntValue(string category, string key) 
        {
            Settings settings = GetInstance();

            string value = settings.GetValue(category, key);
            if (value != "")
            {
                try
                {
                    return int.Parse(value);
                }
                catch
                {
                    Log.Error("Settings value is not of type int, category:" + category + " key:" + key);
                }
            }
            return -1;
        }

        // *********************************************************************************************************************************
        public static float GetFloatValue(string category, string key)
        {
            Settings settings = GetInstance();

            string value = settings.GetValue(category, key);
            if (value != "")
            {
                try
                {
                    return float.Parse(value);
                }
                catch
                {
                    Log.Error("Settings value is not of type float, category:" + category + " key:" + key);
                }
            }
            return 0;
        }

        // *********************************************************************************************************************************
        public static void ResetCategory(string name)
        {
            UserSettings settings = GetInstance();
            settings.ResetCategoryInternal(name);
        }

        // *********************************************************************************************************************************
        protected virtual void ResetCategoryInternal(string name)
        {
        }
    }
}
