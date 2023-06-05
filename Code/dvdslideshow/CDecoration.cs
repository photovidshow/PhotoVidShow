/*
 * Created by SharpDevelop.
 * User: user
 * Date: 03/01/2005
 * Time: 13:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Xml;
using System.Globalization;
using DVDSlideshow.GraphicsEng;
 
namespace DVDSlideshow
{
	/// <summary>
	/// Description of CDecoration.	
	/// </summary>
	public abstract class CDecoration
	{
        protected static RectangleF mLastCoverageArea;
        protected static float mLastRotation;

		protected RectangleF 	mCoverageArea;			// note area in w&h is from 0.0 to 1.0 as we don't know how big a slide will be
        protected bool mUsePreviousDecorationCoverageArea = false;
        protected int mUsePreviousDecorationCoverageAreaPercentGain = 0;

		private float		mTransparency;

        // ### SRG TODO LEGGACY, REMOVE ALL REFERENCES OF THIS, THIS IS NO LONGER USED
		protected bool		mAttachedToSlideImage = false;	// (i.e is effected by pan zoom effect etc)

        private int mID = -1;

        public virtual RectangleF CoverageArea
        {
            get { return mCoverageArea; }
            set { mCoverageArea = value; }
        }       

        public virtual bool IsTemplateFrameworkDecoration()
        {
            return true;
        }

        public virtual bool IsBorderDecoration()
        {
            return false;
        }

        public virtual bool IsBackgroundDecoration()
        {
            return false;
        }

        // i.e. not a border/filter/background or template decoration
        public bool IsUserDecoration()
        {
            if ( IsBorderDecoration() ||
                 IsBackgroundDecoration() ||
                 IsFilter() ||
                 IsTemplateFrameworkDecoration() ||
                 this is CGroupedDecoration )
            {
                return false;
            }

            return true;
        }

        //*******************************************************************
        public virtual bool HasTemplateImageFilename()
        {
            return false;
        }

        public int ID
        {
            get { return mID; }
        }

		private static int mGlobalID = 0;

        // Used for filter texture effects 
        private bool mRenderPostTransition = false;

        public virtual bool IsLayer()
        {
            return false;
        }

        public virtual bool IsFilter()
        {
            return false;
        }

        public bool IsGroupableDecoration()
        {
            if (this.IsBorderDecoration() == true) return false;
            if (this is CGroupedDecoration == true) return false;

            return true;
        }

        public bool RenderPostTransition
        {
            get { return mRenderPostTransition; }
            set { mRenderPostTransition = value; }
        }

        public bool AttachedToSlideImage
        {
            get { return mAttachedToSlideImage; }
            set { mAttachedToSlideImage = false; }  // LEGACY ALWAYS SET TO FALSE
        }

		public float Transparency
		{
			get{return mTransparency; }
			set
			{
				mTransparency=value ;
			}
		}

        //*******************************************************************
		public CDecoration()
		{	
		}

        //*******************************************************************
        public void AssignID()
        {
            mID = mGlobalID++;
        }

		//*******************************************************************
		public CDecoration(RectangleF coverage, int order)
		{
			mTransparency = 0.0f ;
			mCoverageArea = coverage ;
			mAttachedToSlideImage = false;
		}

		//*******************************************************************
		public abstract CDecoration Clone();


        //*******************************************************************
        // New better way of doing a clone
        public CDecoration XMLClone()
        {
            CDecoration newDecoration = null;

            try
            {
                XmlDocument saveDoc = new XmlDocument();
                XmlElement docElement = saveDoc.CreateElement("doc");
                saveDoc.AppendChild(docElement);
                this.Save(docElement, saveDoc);

                XmlNodeList list = docElement.GetElementsByTagName("Decoration");

                if (list.Count != 0)
                {
                    XmlElement element = list[0] as XmlElement;
                    newDecoration = CDecoration.CreateFromType(element.GetAttribute("Type"));
                    newDecoration.Load(element);
                }
            }
            catch
            {
            }

            if (newDecoration == null)
            {
                ManagedCore.Log.Error("Failed to do XML Clone on Decoration");
            }

            return newDecoration;
        }
      
        //*******************************************************************
        virtual public void ResetAllMediaStreams()
        {
        }

        //*******************************************************************
        virtual public void StopAllNonPlayingMediaStreams(CSlide currentSlide, CSlide nextSlide)
        {
        }


		//*******************************************************************
		public CDecoration(CDecoration copy)
		{
			mTransparency =copy.mTransparency;
			mCoverageArea = copy.mCoverageArea ;
			mAttachedToSlideImage = copy.mAttachedToSlideImage;
		}


        //*******************************************************************
        public virtual Rectangle RenderToGraphics(RectangleF r, int frame_num, CSlide originating_slide, RenderSurface inputSurface)
        {
            return new Rectangle(0, 0, 1, 1);
        }


		//*******************************************************************
        public virtual Rectangle RenderToGraphics(Graphics gp, RectangleF r, int frame_num, CSlide originating_slide, CImage original_image)
		{
            return new Rectangle(0, 0, 1, 1);
		}

        //*******************************************************************
        public Rectangle RenderToGraphics(Graphics gp, RectangleF r, int frame_num, CSlide originating_slide)
        {
            return RenderToGraphics(gp, r, frame_num, originating_slide, null);
        }

		//*******************************************************************
		// get coverage area given the input rectangle
		
		public Rectangle GetCoverageArea(Rectangle r)
		{
				
			float x = ((float)r.Width)*this.mCoverageArea.X+r.X;
			float y = ((float)r.Height)*this.mCoverageArea.Y+r.Y;
			float width = ((float)r.Width)*this.mCoverageArea.Width ;
			float height = ((float)r.Height)*this.mCoverageArea.Height ;

			Rectangle r1 = new Rectangle((int)x,(int)y,(int)width,(int)height);
		
			return r1;
		}

        //*******************************************************************
        public virtual void DeclareSlideAspectChange(CGlobals.OutputAspectRatio oldAspect, CGlobals.OutputAspectRatio newAspect)
        {
            // default, do nothing
        }
       
        //*******************************************************************
        protected void AdjustCoverageAreaForNewAspect(CGlobals.OutputAspectRatio oldAspect, CGlobals.OutputAspectRatio newAspect)
        {
            if (oldAspect == CGlobals.OutputAspectRatio.TV16_9 &&
                newAspect == CGlobals.OutputAspectRatio.TV4_3)
            {
                float newWidth = mCoverageArea.Width * 1.33333f;
                float different = newWidth - mCoverageArea.Width;
                float halfdifferent = different / 2.0f;

                mCoverageArea = new RectangleF(mCoverageArea.X - halfdifferent, mCoverageArea.Y, newWidth, mCoverageArea.Height);
            }
            else if (oldAspect == CGlobals.OutputAspectRatio.TV4_3 &&
                     newAspect == CGlobals.OutputAspectRatio.TV16_9)
            {
                float newWidth = mCoverageArea.Width / 1.33333f;
                float different = mCoverageArea.Width - newWidth ;
                float halfdifferent = different / 2.0f;

                mCoverageArea = new RectangleF(mCoverageArea.X + halfdifferent, mCoverageArea.Y, newWidth, mCoverageArea.Height);
            }
        }

		//*******************************************************************
		public virtual void Save(XmlElement parent, XmlDocument doc)
		{
		}


		//*******************************************************************
		protected void SaveDecrationPart(XmlElement element, XmlDocument doc)
		{
            if (mAttachedToSlideImage == true)
            {
                element.SetAttribute("Attached", this.mAttachedToSlideImage.ToString());
            }

            if (mTransparency != 0)
            {
                element.SetAttribute("Alpha", this.mTransparency.ToString());
            }

            if (mRenderPostTransition == true)
            {
                element.SetAttribute("PostTrans", this.mRenderPostTransition.ToString());
            }

			// fix old bug in saved docs
			if (mTransparency>1) mTransparency = 0;

            if (mUsePreviousDecorationCoverageArea==true)
            {
                if (mUsePreviousDecorationCoverageAreaPercentGain != 0)
                {
                    element.SetAttribute("UsePreviousDecorationCoverageAreaPercent", mUsePreviousDecorationCoverageAreaPercentGain.ToString());
                }
                else
                {
                    element.SetAttribute("UsePreviousDecorationCoverageArea", mUsePreviousDecorationCoverageArea.ToString());
                }
            }
            else
            {
			    CGlobals.SaveRectangle(element, doc, "CoverageArea", this.mCoverageArea);
            }

            if (mID != -1)
            {
                element.SetAttribute("id", this.mID.ToString());
            }
		
		}

        //*******************************************************************
        protected RectangleF GetLastCoverageAreaPlusGainPercent()
        {
            RectangleF result = mLastCoverageArea;

            if (mUsePreviousDecorationCoverageAreaPercentGain != 0)
            {
                float gain = ((float)mUsePreviousDecorationCoverageAreaPercentGain) / 100.0f;

                float w = mLastCoverageArea.Width +  (mLastCoverageArea.Width * gain);
                float h = mLastCoverageArea.Height +  (mLastCoverageArea.Height* gain);

                float x = mLastCoverageArea.X - ((w - mLastCoverageArea.Width) / 2.0f);
                float y = mLastCoverageArea.Y - ((h - mLastCoverageArea.Height) / 2.0f);

                result = new RectangleF(x, y, w, h);

            }

            return result;
        }


        //*******************************************************************
        public virtual bool VerfifyAllMediaFilesToRenderThisExist()
        {
            return true;
        }

		//*******************************************************************
		public virtual void Load(XmlElement element)
		{
		}


		//*******************************************************************
		protected void LoadDecrationPart(XmlElement element)
		{
          //  string s = element.GetAttribute("Attached");
           // if (s != "")
           // {
            //    mAttachedToSlideImage = bool.Parse(s);
           // }

            string s = element.GetAttribute("Alpha");
            if (s != "")
            {
                mTransparency = float.Parse(s);
            }

            s = element.GetAttribute("UsePreviousDecorationCoverageArea");
            if (s != "")
            {
                mUsePreviousDecorationCoverageArea = bool.Parse(s);
            }

            s = element.GetAttribute("UsePreviousDecorationCoverageAreaPercent");
            if (s != "")
            {
                mUsePreviousDecorationCoverageAreaPercentGain = int.Parse(s);
                mUsePreviousDecorationCoverageArea = true;
            }

            if (mUsePreviousDecorationCoverageArea == false)
            {
                mCoverageArea = CGlobals.LoadRectangle(element, "CoverageArea");
            }

			string id = element.GetAttribute("id");
			if (id!="") this.mID = int.Parse(id);

            s = element.GetAttribute("PostTrans");
            if (s != "")
            {
                mRenderPostTransition = bool.Parse(s);
            }

			// if loadinf ID make sure any new one is alway after this
			if (mID !=-1 && mGlobalID<=mID)
			{
				mGlobalID=mID+1;
			}
		}


		//*******************************************************************
		static public CDecoration CreateFromType(string type)
		{
			if (type=="ImageDecoration") return new CClipArtDecoration();
			if (type=="ButtonDecoration") return new CMenuSlideshowButton();
			if (type=="ButtonSlideshowDecoration") return new CMenuSlideshowButton();
			if (type=="TextDecoration") return new CTextDecoration();
            if (type == "ScrollingTextDecoration") return new CScrollingTextDecoration();
            if (type == "ButtonLinkNextPreviousDecoration") return new CMenuLinkPreviousNextButton();
            if (type == "ButtonLinkSubMenu") return new CMenuLinkSubMenuButton();
            if (type == "VideoDecoration") return new CVideoDecoration();
            if (type == "BlurFilterDecoration") return new CBlurFilterDecoration();
            if (type == "ColourTransformDecoration") return new CColourTransformDecoration();
            if (type == "MonotoneTransformDecoration") return new CMonotoneTransformDecoration();
            if (type == "SepiaTransformDecoration") return new CSepiaTransformDecoration();
            if (type == "GroupedDecoration") return new CGroupedDecoration();
            if (type == "PlayAllButton") return new CMenuPlayAllButton();
            if (type == "PlayAllLoopedButton") return new CMenuPlayAllLoopedButton();

            // legacy backwards compatibility
            if (type == "ButtonLinkDecoration") return new CMenuLinkPreviousNextButton();
         
			Console.WriteLine("Error: Unknown Decoratiomn type on load");
			return null;
		}

	}
}
