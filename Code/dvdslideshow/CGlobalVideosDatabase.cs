using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
    // This class is used as a database to store a global videos. e.g. abstract background and filter videos 
    // This allows multiple video decorations to reference the same VideoPlayer.  

    // This class basically creates a CVideoDecoration which is the master owner of the VideoPlayer. This
    // decoration is then stored internally.
    // Clients of this class can then create CVideoDecoration's which referenes the master VideoPlayer.
    // As the stored CVideoDecoration is never deleted, then other CVideoDecorations don't have to worry about
    // the player being destroyed.  This system works better from the gui point of view compared with the old 
    // StringId method with VideoPlayerDatabase

    public class CGlobalVideoDatabase
    {
        private static CGlobalVideoDatabase mInstance;

        public static CGlobalVideoDatabase GetInstance()
        {
            if (mInstance == null)
            {
                mInstance = new CGlobalVideoDatabase();
            }
            return mInstance;
        }

        private List<CVideoDecoration> mDecors = new List<CVideoDecoration>();

        //***************************************************************************************
        private CGlobalVideoDatabase()
        {
        }

          //***************************************************************************************
        private CVideoDecoration GetOrCreateGlobalVd(string fullFilename)
        {
            string filename = CGlobals.StripRootHeader(fullFilename);

            CVideoDecoration storedDecor = null;

            foreach (CVideoDecoration vd in mDecors)
            {           
                if (vd.Player != null)
                {
                    string fn = CGlobals.StripRootHeader(vd.GetFilename());

                    if (fn == filename)
                    {              
                        storedDecor = vd;
                        break;
                    }
                }
            }

            if (storedDecor == null)
            {
                storedDecor = new CVideoDecoration(filename, new RectangleF(0, 0, 1, 1), 0);
                if (storedDecor.Player != null)
                {
                    storedDecor.Player.Loop = true;
                }

                storedDecor.UseGlobalPlayer = true;
                mDecors.Add(storedDecor);
            }

            return storedDecor;
        }


        //***************************************************************************************
        public CVideoDecoration CreateVideoDecoration(string filename)
        {
            CVideoDecoration storedDecor =GetOrCreateGlobalVd(filename);

            CVideoDecoration vd2 = new CVideoDecoration(storedDecor.Player);
            vd2.UseGlobalPlayer = true;
            vd2.CoverageArea = storedDecor.CoverageArea;

            return vd2;
        }

        //***************************************************************************************
        public void SetupFromVideoDecoration(CVideoDecoration vd, string filename)
        {
            CVideoDecoration storedDecor =GetOrCreateGlobalVd(filename);
            vd.Player = storedDecor.Player;
            vd.UseGlobalPlayer = true;
        }


        //***************************************************************************************
        public void Clear()
        {
            mDecors.Clear();
        }

    }
}
