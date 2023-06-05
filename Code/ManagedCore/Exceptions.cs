using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ManagedCore
{
    public class ErrorException : Exception
    {
        public ErrorException(string a)
            : base(a)
        {
            CDebugLog.GetInstance().Error(a);
        }
    }

    public class MissingFileException : Exception
    {
        public string mFilename;
        public string mOriginalFullFilename;
        public MissingFileException(string file)
        {
            mOriginalFullFilename = file;
            string s = Path.GetFileName(file);
            MissingFilesDatabase.GetInstance().AddFile(mOriginalFullFilename);
            mFilename = s;
        }
    }

    public class IgnoreOperationException : Exception
    {
        public IgnoreOperationException()
        {
        }
    } 


}
