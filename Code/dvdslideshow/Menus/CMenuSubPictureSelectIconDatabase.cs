using System;
using System.Collections;
using ManagedCore;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for CMenuSubPictureSelectIconDatabase.
    /// </summary>
    public class CMenuSubPictureSelectIconDatabase
    {
        private static CMenuSubPictureSelectIconDatabase mInstance;

        public static CMenuSubPictureSelectIconDatabase GetInstance()
        {
            if (mInstance == null)
            {
                mInstance = new CMenuSubPictureSelectIconDatabase();
            }
            return mInstance;
        }


        //*******************************************************************
        //*******************************************************************
        //*******************************************************************

        private ArrayList mEntries;


        //*******************************************************************
        private CMenuSubPictureSelectIconDatabase()
        {
            BuildDatabase();
        }


        //*******************************************************************
        private void BuildDatabase()
        {
            mEntries = new ArrayList();

            string root = CGlobals.GetRootDirectory();

            for (int i = 1; i < 54; i++)
            {
                try
                {
                    string iconName = root + "\\MenuButtonStyles\\SelectIcon" + i + ".png";

                    CImage icon = new CImage(iconName, false);
                    mEntries.Add(icon);
                }
                catch
                {
                }

            }
        }

        //*******************************************************************
        public CImage GetIcon(int index)
        {
            if (index > mEntries.Count - 1)
            {
                CDebugLog.GetInstance().Error("Entry " + index + " not in menu select icon database");
                index = 0;
            }

            return (CImage)mEntries[index];
        }


        //*******************************************************************
        public int GetNumIcons()
        {
            return mEntries.Count;
        }

    }
}
