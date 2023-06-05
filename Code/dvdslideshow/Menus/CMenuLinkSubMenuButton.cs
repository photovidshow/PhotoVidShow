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
    public class CMenuLinkSubMenuButton : CMenuLinkButton
	{
     
        private CMenuButtonInnerVideoRenderer mInnerRenderer;
	
		public CImage Mask
		{
			set 
            {
                if (mInnerRenderer == null)
                {
                    Init(value);
                }

                if (mInnerRenderer != null)
                {
                    mInnerRenderer.Mask = value;
                }
            }
		}


		//*******************************************************************
		public CMenuLinkSubMenuButton()
		{
		}

		//*******************************************************************
        public CMenuLinkSubMenuButton(int inner_menu_id,
                                      CImage frame,
                                      CImage mask, 
                                      RectangleF coverage)
            :
			base(frame, coverage, inner_menu_id)
		{
			Init(mask);
		}

        //*******************************************************************
        public CMenuLinkSubMenuButton(CMainMenu inner_menu,
                                      CImage frame,
                                      CImage mask,
                                      RectangleF coverage)
            :
            base(frame, coverage, inner_menu.ID)
        {
            mInnerRenderer = new CMenuButtonInnerVideoRenderer(inner_menu);
            if (mask != null) mInnerRenderer.Mask = mask;
        }

		//*******************************************************************
		private void Init(CImage mask) 
		{
            CVideo inner_video = CGlobals.mCurrentProject.GetMenu(MenuLinkID);

            if (inner_video == null)
            {
                CDebugLog.GetInstance().Error("Menu sub link button can't find inner video id:" + MenuLinkID);
            }
            else
            {
                mInnerRenderer = new CMenuButtonInnerVideoRenderer(inner_video);
                if (mask != null) mInnerRenderer.Mask = mask;
            } 
		}

		//*******************************************************************
        public override Rectangle RenderToGraphics(RectangleF r, int frame_num, CSlide originating_slide, RenderSurface inputImage)
		{
            if (mInnerRenderer == null)
            {
                Init(null);
            }

            float x = r.Width * this.mCoverageArea.X + r.X;
            float y = r.Height * this.mCoverageArea.Y + r.Y;
            float width = r.Width * this.mCoverageArea.Width;
            float height = r.Height * this.mCoverageArea.Height;

            bool playing_thumbnails = GetMenuWeAreIn().PlayingThumbnails;

            Rectangle r1 = new Rectangle((int)x, (int)y, (int)width, (int)height);

            if (mInnerRenderer !=null)
            {
                 mInnerRenderer.Render(r1, frame_num, playing_thumbnails, true, Transparency);
            }

            Rectangle drawn_region = base.RenderToGraphics(r, frame_num, originating_slide, inputImage);

  
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

            decoration.SetAttribute("Type", "ButtonLinkSubMenu");

			SaveMenuLinkButtonPart(decoration, doc);
		
			parent.AppendChild(decoration); 
		}

		//*******************************************************************
		public override void Load(XmlElement element)
		{
            LoadMenuLinkButtonPart(element);
		
			ManagedCore.CDebugLog.GetInstance().Trace("Loaded slideshow button, coverage = "+
				this.mCoverageArea.X+" "+
				this.mCoverageArea.Y+" "+
				this.mCoverageArea.Width+" "+
				this.mCoverageArea.Height+" ");

			if (this.mAttachedToSlideImage == true)
			{
				CDebugLog.GetInstance().Warning("Loaded sub link menu button decoration attached to slide. Un-attaching");

				mAttachedToSlideImage =false ;
			}

            // inner renderer is initilised later when all sub menus have been loaded in
		}
	}
}
