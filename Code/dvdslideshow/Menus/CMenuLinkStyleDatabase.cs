using System;
using System.Collections;
using ManagedCore;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for CMenuLinkStyleDatabase.
    /// </summary>
    public class CMenuLinkStyleDatabase
    {
        private static CMenuLinkStyleDatabase mInstance;

        public static CMenuLinkStyleDatabase GetInstance()
        {
            if (mInstance == null)
            {
                mInstance = new CMenuLinkStyleDatabase();
            }
            return mInstance;
        }

        private ArrayList mEntries;

        //*******************************************************************
        private CMenuLinkStyleDatabase()
        {
            BuildDatabase();
        }

        //*******************************************************************
        private void BuildDatabase()
        {
            mEntries = new ArrayList();

            string prv = CGlobals.GetRootDirectory();

            for (int i = 1; i < 100; i++)
            {
                try
                {
                    string frame_previous = prv + "\\MenuButtonStyles\\previous" + i + ".png";
                    string frame_next = prv + "\\MenuButtonStyles\\next" + i + ".png";

                    if (System.IO.File.Exists(frame_previous))
                    {
                        CMenuLinkStyle style = new CMenuLinkStyle(frame_previous, frame_next);
                        mEntries.Add(style);
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Exception when building menu link style database : " + e.Message);
                }
            }
        }

        //*******************************************************************
        public CMenuLinkStyle GetStyle(int index)
        {
            if (index > mEntries.Count - 1)
            {
                CDebugLog.GetInstance().Error("Entry " + index + " not in menu link style database");
                index = 0;
            }

            return (CMenuLinkStyle)mEntries[index];
        }


        //*******************************************************************
        public int GetNumStyles()
        {
            return mEntries.Count;
        }

    }
}
