using System;
using System.Collections.Generic;
using System.Text;

namespace DVDSlideshow.BluRay.ISO
{
    //
    // Structure used when generating file tree
    //
    public class FileSystemEntry : IComparable
    {
        private string name;
        private bool isFolder;
        private string directory;
        private int subFolderCount = 0;
        private DateTime timeStamp;

        public FileSystemEntry()
        {
        }

        public FileSystemEntry(string name, bool isFolder, DateTime timeStamp, string directory)
        {
            this.name = name;
            this.isFolder = isFolder;
            this.directory = directory;
            this.timeStamp = timeStamp;
        }

        public string Directory
        {
            get { return directory; }
            set { directory = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool Folder
        {
            get { return isFolder; }
            set { isFolder = value; }
        }

        public DateTime TimeStamp
        {
            get { return timeStamp; }
            set { timeStamp = value; }
        }

        public int SubFolderCount
        {
            get { return subFolderCount; }
            set { subFolderCount = value; }
        }

        public int CompareTo(object obj)
        {
            FileSystemEntry that = obj as FileSystemEntry;
            if (that != null)
            {
                return name.CompareTo(that.name);
            }
            return 0;
        }
    }
}
