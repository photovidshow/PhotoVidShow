using System;
using System.Collections.Generic;

namespace ManagedCore
{
    //
    // This class holds onto the application default folder locations
    //
    public class DefaultFolders
    {

        // *********************************************************************************************
        public static string GetFolder(string name)
        {
            return GetInstance().GetFolderInternal(name);
        }


        // *********************************************************************************************
        public static bool SetFolder(string name, string location, bool requireWriteAccess)
        {
            return GetInstance().SetFolderInternal(name, location, requireWriteAccess);
        }


        private static DefaultFolders mInstance = null;
        private Dictionary<string, string> mFoldersDictionary = new Dictionary<string, string>();


        // *********************************************************************************************
        private static DefaultFolders GetInstance()
        {
            if (mInstance == null)
            {
                mInstance = new DefaultFolders();
            }
            return mInstance;
        }

        // *********************************************************************************************
        private string GetFolderInternal(string name)
        {
            string folder;
            bool result = mFoldersDictionary.TryGetValue(name, out folder);
            if (result == true && string.IsNullOrEmpty(folder) == false)
            {
                return folder;
            }

            // Ok try special folders
            if (name.ToLower() == "mypictures")
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures).ToString();
            }

            if (name.ToLower() == "mymusic")
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic).ToString();
            }

            //
            // Not set return MyDocuments
            //
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToString();

        }

        // **************************************************************************************************
        private bool SetFolderInternal(string name, string folder, bool requireWriteAccess)
        {
            if (System.IO.Directory.Exists(folder) == false)
            {
                Log.Error("Can not add folder to default folders list, as it does not exist " + folder);
                return false;
            }

            if (requireWriteAccess == true && IO.HasWriteAccessToDirectory(folder) == false)
            {
                Log.Warning("Can not add folder to default folders list, as there is no write access to it " + folder);
                return false;
            }


            //
            // Already exists? remove old one first
            //
            if (mFoldersDictionary.ContainsKey(name))
            {
                mFoldersDictionary.Remove(name);
            }


            mFoldersDictionary.Add(name, folder);

            return true;
        }

    }
}
