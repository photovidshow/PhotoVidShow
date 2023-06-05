using System;
using System.Drawing;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for CMenuFrameStyle.
	/// </summary>
	public class CMenuButtonStyle
	{
		private CImage mFrame;
		private CImage mFrameMask =null;

        public CMenuButtonStyle(string frame, string frame_mask)
		{
			mFrame = new CImage(frame,false);

			if (System.IO.File.Exists(frame_mask)==true)
			{
				mFrameMask = new CImage(frame_mask,false);
			}
		}

		public CImage SlideshowButtonFrameImage
		{
            get { return mFrame; }
		}

		public CImage SlideshowButtonMaskImage
		{
            get { return mFrameMask; }
		}
	}
}
