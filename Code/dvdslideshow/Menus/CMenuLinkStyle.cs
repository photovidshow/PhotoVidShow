using System;
using System.Drawing;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for CMenuLinkStyle.
    /// </summary>
    public class CMenuLinkStyle
    {
        private CImage mNext;
        private CImage mPrevious;

        public CMenuLinkStyle(string previousImageFilename, string nextImageFilename)
        {
            mPrevious = new CImage(previousImageFilename, false);
            mNext = new CImage(nextImageFilename, false);
        }

        public CImage NextButtonImage
        {
            get { return mNext ; }
        }

        public CImage PreviousButtonImage
        {
            get { return mPrevious; }
        }
    }
}
