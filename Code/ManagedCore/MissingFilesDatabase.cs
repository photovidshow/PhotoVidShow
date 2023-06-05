using System;
using System.Collections;
using System.IO;

namespace ManagedCore
{
	/// <summary>
	/// Summary description for MissingFilesDatabase.
	/// </summary>
	/// 

    // ************************************************************************************
	public class MissingFile
	{
		public MissingFile(string fullName) 
		{
			mName = Path.GetFileName(fullName);
            mFullName = fullName;
        }

		public string mName;
        public string mFullName;                    // used to tell the user the original directory the file was stored in
        public string mFoundDirecotry = "";
	}


    //************************************************************************************
	public class MissingFilesDatabase
	{
		private static MissingFilesDatabase mInstance ;
		private ArrayList mMissingFiles;
        private bool mAbortedLoad = false;
        private string mAbortedReasonString = "";


        //************************************************************************************
		private  MissingFilesDatabase()
		{
	
			mMissingFiles = new ArrayList();
		}

        //************************************************************************************
        public bool AbortedLoad
        {
            get { return mAbortedLoad; }
            set
            {
                mAbortedLoad = value;
                if (mAbortedLoad == false)
                {
                    mAbortedReasonString = "";
                }
            }
        }

        //************************************************************************************
        public string AbortedReasonString
        {
            get { return mAbortedReasonString; }
            set { mAbortedReasonString = value; }
        }

        //************************************************************************************
        public void AbortLoad(string reason)
        {
            mAbortedLoad = true;
            mAbortedReasonString = reason;
        }

        //************************************************************************************
		public static MissingFilesDatabase GetInstance()
		{
			if (mInstance == null)
			{
				mInstance = new MissingFilesDatabase();
			}
			return mInstance ;
		}

        //************************************************************************************
		public ArrayList GetMissingFilesData()
		{
			return mMissingFiles ;
		}

        //************************************************************************************
		public void Clear()
		{
			mMissingFiles.Clear();
            mAbortedLoad = false;
            mAbortedReasonString = "";
		}

        //************************************************************************************
		public ArrayList GetUnlinkedFiles()
		{
			ArrayList a = new ArrayList();
			foreach (MissingFile mf in mMissingFiles)
			{
				if (mf.mFoundDirecotry == "")
				{
					a.Add(mf);
				}
			}
			return a;
		}

        //************************************************************************************
		public void AddFile(string name)
		{
			foreach (MissingFile mf in mMissingFiles)
			{
				if (mf.mFullName.ToLower() == name.ToLower())
				{
					CDebugLog.GetInstance().Warning("File '"+name+" already defined in missing files database... ignoring");
					return ;
				}
			}

			mMissingFiles.Add(new MissingFile(name));
		}

        //************************************************************************************
		public string SearchForNewFileLink(string filename)
		{
			string s = Path.GetFileName(filename)  ;
				
			foreach (MissingFile mf in mMissingFiles)
			{
				if (mf.mName.ToLower() == s.ToLower())
				{
                    // may not have been found yet? i.e. duplicate missing file
                    if (mf.mFoundDirecotry == "")
                    {
                        return "alreadydeclaredmissing";
                    }

					string t = mf.mFoundDirecotry;
					t+="/";
					t+=mf.mName;
					return t;
				}
			}
			return null;
		}
	}
}
