using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml;
using ManagedCore;
using DVDSlideshow.GraphicsEng;


namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for CMenuSlideshowButton.
	/// </summary>
	public class CMenuSlideshowButton : CMenuButton
	{
		private string	mInnerImage;
 
		// return to menu after playing slideshow ? else repeat playing the slideshow forever
		// needed for PhotoCruz
		private bool	mReturnToMenuAfterPlay = true;

		public bool ReturnToMenuAfterPlay
		{
			get { return mReturnToMenuAfterPlay ; }
			set { mReturnToMenuAfterPlay  = value; }
		}
		
        private CMenuButtonInnerVideoRenderer mInnerRenderer;
		
        private CImage mCachedEditModeImage=null;

        public CImage Mask
        {
            set { if (mInnerRenderer != null) mInnerRenderer.Mask = value; }
        }

		
		//*******************************************************************
		public CMenuSlideshowButton()
		{
		}

		//*******************************************************************
		public CMenuSlideshowButton(string inner_image, CImage frame, CImage mask, RectangleF coverage) :
			base(frame, coverage)
		{
			mInnerImage = inner_image;
			Init(mask);
		
		}

		//*******************************************************************
		private void Init(CImage mask) 
		{
			CVideo inner_video = CGlobals.mCurrentProject.GetSlideshow(mInnerImage);

            if (inner_video == null)
			{
				Log.Error("Menu button can't find slideshow:"+mInnerImage);
			}
            else
            {
                mInnerRenderer = new CMenuButtonInnerVideoRenderer(inner_video);
                if (mask != null) mInnerRenderer.Mask = mask;
            }
                
		}


		//*******************************************************************
		public	string GetInnerImageStringId() 
		{ 
			return mInnerImage ;
		}


        //*******************************************************************
        public override Rectangle RenderToGraphics(RectangleF r, int frame_num, CSlide originating_slide, RenderSurface inputSurface)
        {
            float x = r.Width * this.mCoverageArea.X + r.X;
            float y = r.Height * this.mCoverageArea.Y + r.Y;
            float width = r.Width * this.mCoverageArea.Width;
            float height = r.Height * this.mCoverageArea.Height;

            Rectangle r1 = new Rectangle((int)x, (int)y, (int)width, (int)height);

            bool playing_thumbnails = GetMenuWeAreIn().PlayingThumbnails;

            if (mInnerRenderer != null)
            {
                mInnerRenderer.Render(r1, frame_num, playing_thumbnails, false, Transparency);
            }

            Rectangle drawn_region = base.RenderToGraphics(r, frame_num, originating_slide, inputSurface);

            return drawn_region;
        }

        //*******************************************************************
        public override void ResetAllMediaStreams()
        {
            if (mInnerRenderer != null)
            {
                mInnerRenderer.ResetAllMediaStreams();
            }
        }

		//*******************************************************************
		public override void Save(XmlElement parent, XmlDocument doc)
		{
			XmlElement decoration = doc.CreateElement("Decoration");

			decoration.SetAttribute("Type","ButtonSlideshowDecoration");

			SaveMenuButtonPart(decoration, doc);
		
			decoration.SetAttribute("InnerVideoName",this.mInnerImage);
			decoration.SetAttribute("ReturnToMenuAfterPlay",mReturnToMenuAfterPlay.ToString());

			parent.AppendChild(decoration); 
		}

		//*******************************************************************
		public override void Load(XmlElement element)
		{
			LoadMenuButtonPart(element);

			ManagedCore.CDebugLog.GetInstance().Trace("Loaded slideshow button, coverage = "+
				this.mCoverageArea.X+" "+
				this.mCoverageArea.Y+" "+
				this.mCoverageArea.Width+" "+
				this.mCoverageArea.Height+" ");

			if (this.mAttachedToSlideImage == true)
			{
				CDebugLog.GetInstance().Warning("Loaded menu slideshow button decoration attached to slide. Un-attaching");

				mAttachedToSlideImage =false ;
			}
			mInnerImage = element.GetAttribute("InnerVideoName");

			string s = element.GetAttribute("ReturnToMenuAfterPlay");
			if (s!="") mReturnToMenuAfterPlay = bool.Parse(s);

            	// the mask will be loaded and set on all button decorations on the loading of the parent menu

			Init(null);
		}
	}
}
