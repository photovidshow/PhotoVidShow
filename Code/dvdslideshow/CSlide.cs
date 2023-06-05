/*
 * Created by SharpDevelop.
 * User: user
 * Date: 03/01/2005
 * Time: 13:26
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
	/// Description of CSlide.	
	/// </summary>
	public class CSlide
	{
		protected Color	mBackGroundColour;
        protected CImage mBackgroundImage = null;
	
		protected float				mDisplayTimeLength = 8.0f;
		protected bool				mUsePanZoom;

        public bool UsePanZoom
        {
            get { return mUsePanZoom; }
            set { mUsePanZoom = value; }
        }

        private int mRotation = 0;
 
		public int					mStartFrameOffset =0;
	
		private CTransitionEffect	mTransitionEffect ;
		private bool				mUseDefaultTransistionEffect = true;

		private	int					mId;

        private float mMasterBackgroundImageAspect = 0;   // used (by templates) for adjustments of decorations if output aspect changes
        private CImage.DrawImageWithAspectType mMasterBackgroundImageAspectType = CImage.DrawImageWithAspectType.KeepAspectByChangingCoverageArea;

		// led
		protected int				mLengthInFrames = 0;

		private static int			mIDCount=0;

        // If a template we can indicate here how many subframes needed for motion blur for this slide
        protected int mForcedMotionBlurSubFrames = 1;

        // This is needed for templates. If this is set to true, when "image1" is set to something, we look
        // at changing the length of this slide (for example if a video decoration)
        private bool mReCalcSlideLengthOnImage1DecorationChange = false;

        private bool mMarkedAsChapter = false;

        public bool MarkedAsChapter
        {
            get { return mMarkedAsChapter; }
            set { mMarkedAsChapter = value; }
        }

        public bool ReCalcSlideLengthOnImage1DecorationChange
        {
            set { mReCalcSlideLengthOnImage1DecorationChange = value; }
            get { return mReCalcSlideLengthOnImage1DecorationChange; }
        }

        public float MasterBackgroundImageAspect
        {
            get { return mMasterBackgroundImageAspect; }
        }

        public CImage.DrawImageWithAspectType MasterBackgroundImageAspectType
        {
            get { return mMasterBackgroundImageAspectType; }
        }
        
        public int ForcedMotionBlurSubFrames
        {
            get 
            {
                int fb = mForcedMotionBlurSubFrames;

                if (mTransitionEffect != null)
                {
                    int transitionfb = mTransitionEffect.ForcedMB;
                    if (transitionfb > fb) fb = transitionfb;
                }

                return fb;
            }

            set { mForcedMotionBlurSubFrames = value; }
        }

        // best time to view this slide in storyboard (used by templates) if -1, then storyboard decides
        private float mThumbnailPreviewTime = -1;

        public float ThumbnailPreviewTime
        {
            get { return mThumbnailPreviewTime; }
            set { mThumbnailPreviewTime = value; }
        }

        protected RenderSurface mFinalRenderSurface; // used when rendering

        public virtual CImage BackgroundImage
        {
            get { return mBackgroundImage; }
            set { mBackgroundImage = value; }
        }

		virtual public float DisplayLength
		{
			get
			{
				return mDisplayTimeLength;
			}
			set
			{
				if (mDisplayTimeLength==value) return ;
				mDisplayTimeLength=value ;
                VerifyTransitionEffectNotLongerThanHalfSlideLength();
				CGlobals.mCurrentProject.DeclareChange("A slide length has been set");
			}
		}

        public virtual CDecoration GetPostTransisionDecoration(int frame)
        {
            return null;
        }

        virtual public void ResetAllMediaStreams()
        {
        }

        virtual public void StopAllNonPlayingMediaStreams(CSlide current_previewing_slide, CSlide next_preview_slide)
        {
        }
         

		public int ID 
		{
			get { return mId; }
		}

		public bool UsesDefaultTransistionEffect 
		{ 
			get { return mUseDefaultTransistionEffect ; } 
			set { mUseDefaultTransistionEffect=value ; }
		}

		public CTransitionEffect TransistionEffect 
		{ 
			get { return mTransitionEffect ; }
			set { mTransitionEffect=value; }
		}

		public float StartTime
		{
			get 
			{
				return ((float) mStartFrameOffset)/  
					((float)CGlobals.mCurrentProject.DiskPreferences.frames_per_second);
			}
		}

        public float GetSecondsSinceStartOfSlide(int frame)
        {
            if (frame < mStartFrameOffset) return 0;

            return ((float)(frame - mStartFrameOffset)) /
                     ((float)CGlobals.mCurrentProject.DiskPreferences.frames_per_second);
        }


        // LEGACY for Stillpicture and videoslides
		//*******************************************************************
		public int Rotation
		{
			get
			{
				return mRotation;
			}
		}


		//*******************************************************************
		public RotateFlipType GetRotationFlipType()
		{
			RotateFlipType type = RotateFlipType.RotateNoneFlipNone;

			if (mRotation == 90) type = RotateFlipType.Rotate90FlipNone;
			if (mRotation == 180) type = RotateFlipType.Rotate180FlipNone;
			if (mRotation == 270) type = RotateFlipType.Rotate270FlipNone;

			return type;
		}


        //*******************************************************************
        public CImageDecoration.OrientationType GetOrientationType()
        {
            CImageDecoration.OrientationType type = CImageDecoration.OrientationType.NONE;

            if (mRotation == 90) type = CImageDecoration.OrientationType.CW90;
            if (mRotation == 180) type = CImageDecoration.OrientationType.CW180;
            if (mRotation == 270) type = CImageDecoration.OrientationType.CW270;

            return type;
        }

        // end legacy
        ////////////////////////////////////////////////////////////////////

		// SRG HACK
		virtual public void SetLengthWithoutUpdate(float val)
		{
			mDisplayTimeLength = val;
            VerifyTransitionEffectNotLongerThanHalfSlideLength();
		}

		// led hack
		private bool					mPartOfAMenu = false;

		public bool PartOfAMenu
		{
			get { return mPartOfAMenu ; }
			set { mPartOfAMenu = value; mLengthInFrames =0 ;}
		}

 
		public CSlide()
		{
			mUsePanZoom = true ;
			mBackGroundColour = Color.Black;
			mId= mIDCount++;
		}

		public virtual CSlide Clone()
		{
			return new CSlide(this);
		}


        // ### SRG TODO REPLACE ABOVE IF WORKS 
        // New better way of doing a clone of a slide?
        public CSlide XMLClone()
        {
            CSlide newSlide = null;

            try
            {
                XmlDocument saveDoc = new XmlDocument();
                XmlElement docElement = saveDoc.CreateElement("doc");
                saveDoc.AppendChild(docElement);
                this.Save(docElement, saveDoc);

                XmlNodeList list = docElement.GetElementsByTagName("Slide");


                if (list.Count != 0)
                {
                    XmlElement element = list[0] as XmlElement;
                    newSlide = CSlide.CreateFromType(element.GetAttribute("Type"));
                    newSlide.Load(element);
                }
            }
            catch
            {
            }

            if (newSlide == null)
            {
                SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromSlide(this);
                ManagedCore.Log.Error("Failed to do XML Clone for slide in slideshow '" + message.Slideshow + "' slide:" + message.SlideNumber);
            }

            return newSlide;
        }


		//*******************************************************************
		public CSlide(CSlide copy)
		{
			mUsePanZoom = copy.mUsePanZoom;
			mBackGroundColour = copy.mBackGroundColour;
			mId= mIDCount++;
			mDisplayTimeLength = copy.mDisplayTimeLength;
			mUsePanZoom = copy.mUsePanZoom;
			mRotation = copy.mRotation;
			mStartFrameOffset = copy.mStartFrameOffset;
			mTransitionEffect = null;
            mMasterBackgroundImageAspect = copy.mMasterBackgroundImageAspect;
            mMasterBackgroundImageAspectType = copy.mMasterBackgroundImageAspectType;
		
			mLengthInFrames = copy.mLengthInFrames;
		}

        //*******************************************************************
        public void RenderFrame(bool ignore_pan_zoom, bool ignore_decors, int req_width, int req_height)
        {
            RenderFrame(mStartFrameOffset, ignore_pan_zoom, ignore_decors, req_width, req_height);
        }

    
        //*******************************************************************
        public virtual void RenderFrame(int frame, bool ignore_pan_zoom, bool ignore_decors, int req_width, int req_height)
        {
            GetVideo(frame, ignore_pan_zoom, ignore_decors, req_width, req_height, null);
        }

		//*******************************************************************
		protected virtual CImage GetVideo(int frame, bool ignore_pan_zoom,bool ignore_decors, int req_width, int req_height, CImage render_to_this)
		{
            RenderBackgroundColour();
            return null;
		}

        //*******************************************************************
        protected void RenderBackgroundColour()
        {
            GraphicsEngine ge = GraphicsEngine.Current;
            if (ge == null) return;
            ge.ClearRenderTarget(mBackGroundColour.R, mBackGroundColour.G, mBackGroundColour.B, 255);
            return;
        }

		//*******************************************************************
		public virtual string GetSourceFileName()
		{
			return "";
		}

		//*******************************************************************
		public virtual void Rotate(int amount)
		{
			mRotation+=amount;

			while (mRotation > 360)
			{
				mRotation -=360;
			}

			while(mRotation < 0)
			{
				mRotation+=360;
			}
		}


		//*******************************************************************
		public virtual void RebuildToNewCanvas(CGlobals.OutputAspectRatio ratio)
		{

		}

        //*******************************************************************
        public virtual void DeclareSlideAspectChange(CGlobals.OutputAspectRatio oldAspect, CGlobals.OutputAspectRatio newAspect)
        {
            
        }
	
		//*******************************************************************
		public void	SetBackgroundColor(Color new_color)
		{
			mBackGroundColour = new_color ;
		}


		//*******************************************************************
		public virtual int CalcLengthInFrame(int offset)
		{
			mStartFrameOffset = offset;
 			
			mLengthInFrames = (int) (( mDisplayTimeLength * CGlobals.mCurrentProject.DiskPreferences.frames_per_second)+0.5f);

			// ok returns frame when next slide shoud begin

            int transitionTimeInFrames = 0;
            if (mTransitionEffect!=null)
            {
                transitionTimeInFrames = mTransitionEffect.GetTotalRenderLengthInFrames() + 1;
            }

            int nextFrameOffset = mStartFrameOffset + mLengthInFrames - transitionTimeInFrames + 1;

            //
            // Just in case nextFrameOffset less than mStartFrameOffset, then set to mStartFrameOffset
            // This can happen of transition effect longer than the slide length
            //
            if (nextFrameOffset < mStartFrameOffset)
            {
                nextFrameOffset = mStartFrameOffset;
            }

            return nextFrameOffset;
		}

        //*******************************************************************
        public int CalcFrameLengthMinusTransition()
        {
            return mLengthInFrames - mTransitionEffect.GetTotalRenderLengthInFrames();
        }
	
		//*******************************************************************
		public int GetFrameLength()
		{
			return mLengthInFrames;
		}	

		//*******************************************************************
		public int GetGlobalEndFrameBeforeTransition()
		{
            return mStartFrameOffset + mLengthInFrames - mTransitionEffect.GetTotalRenderLengthInFrames() + 1;
		
		}

		//*******************************************************************
		// Calculate the global frame number this slide would end on
		public int CalcEndFrame()
		{
			return mStartFrameOffset + GetFrameLength() ;
		}

		//*******************************************************************
		public virtual void RenderUnAttachedDecorations(Rectangle r, int frame_num, bool postTrans, RenderSurface finalSurface)
		{
		}

		//*******************************************************************
		public static CSlide CreateFromType(string type)
		{
			if (type=="VideoSlide") return new CVideoSlide();
			if (type=="StillPictureSlide") return new CStillPictureSlide();
            if (type == "BlankStillPictureSlide") return new CBlankStillPictureSlide();
            if (type == "MultiSlideSlide") return new CMultiSlideSlide();
			if (type=="Slide") return new CSlide();

			ManagedCore.Log.Error("Unknown slide type:"+type);
			return null ;
		}

        //***************************************************************************************
        public virtual int GetNumberRequiredMotionBlurSubFrames(int frame)
        {
            // same as max for this slide for now
            return GetMaxRequiredMotionBlurSubFrames();
        }

        //***************************************************************************************
        public virtual int GetMaxRequiredMotionBlurSubFrames()
        {
            // should really be calling a child override version
            return ForcedMotionBlurSubFrames;
        }


        //*******************************************************************
        // is this slide allowed to be edited in storyboard, i.e. add text/filters etc etc.  Default is true
        public virtual bool AllowedToBeEditited()
        {
            return true;
        }

        //*******************************************************************
        // if the slide is only 1 decoration for example then simple rotation is allowed
        public virtual CDecoration SupportsSimpleOrientationChange()
        {
            return null;
        }

        //*******************************************************************
        // Returns true if a rotation was possible and occured; else returns false
        public virtual bool SimpleOrientateChangeClockwise()
        {
            return false;
        }

        //*******************************************************************
        // Returns true if a rotation was possible and occured; else returns false
        public virtual bool SimpleOrientateChangeAniClockwise()
        {
            return false;
        }

        //***************************************************************************************
        public virtual void ReCalcAllVideoLinkStartEndTimes()
        {
        }

        //***************************************************************************************
        private void VerifyTransitionEffectNotLongerThanHalfSlideLength()
        {
            if (mTransitionEffect == null)
            {
                return;
            }

            //
            // If transition effect longer than half slide length, then set transition effect time to half slide length
            // This is to prevent weird visual side effects
            //
            if (mTransitionEffect.Length > (mDisplayTimeLength /2 ))
            {
                mTransitionEffect.Length = mDisplayTimeLength /2;
            }
        }


        //*******************************************************************
        public virtual void Save(XmlElement parent, XmlDocument doc)
        {
            XmlElement slide = doc.CreateElement("Slide");

            slide.SetAttribute("Type", "Slide");

            SaveSlidePart(slide, doc);

            parent.AppendChild(slide);
        }


        //*******************************************************************
        public virtual void Load(XmlElement element)
        {
            LoadSlidePart(element);
        }


        //*******************************************************************
        protected void SaveSlidePart(XmlElement element, XmlDocument doc)
        {
            element.SetAttribute("PanZoom", this.mUsePanZoom.ToString());

            if (mRotation != 0)
            {
                element.SetAttribute("Rotate", this.mRotation.ToString());
            }

            element.SetAttribute("DisplayTimeLength", this.mDisplayTimeLength.ToString());

            if (mTransitionEffect == null || mUseDefaultTransistionEffect == true)
            {
                element.SetAttribute("UsesDefaultTransitionEffect", mUseDefaultTransistionEffect.ToString());
            }

            if (mBackgroundImage != null)
            {
                element.SetAttribute("BackgroundImage", mBackgroundImage.ImageFilename);
            }

            if (mTransitionEffect != null)
            {
                XmlElement effect = doc.CreateElement("TransitionEffect");

                mTransitionEffect.Save(element, doc, effect);
            }

            if (mForcedMotionBlurSubFrames != 1)
            {
                element.SetAttribute("ForceMBSubFrames", mForcedMotionBlurSubFrames.ToString());
            }

            if (mThumbnailPreviewTime != -1)
            {
                element.SetAttribute("ThumbnailPreviewTime", mThumbnailPreviewTime.ToString());
            }

            if (mReCalcSlideLengthOnImage1DecorationChange == true)
            {
                element.SetAttribute("ReCalcSlideLengthOnImage1Change", mReCalcSlideLengthOnImage1DecorationChange.ToString());
            }

            if (mMasterBackgroundImageAspect != 0)
            {
                element.SetAttribute("MasterBackgroundAspect", mMasterBackgroundImageAspect.ToString());
            }

            if (mMasterBackgroundImageAspectType != CImage.DrawImageWithAspectType.KeepAspectByChangingCoverageArea)
            {
                element.SetAttribute("MastetAspectType", ((int)mMasterBackgroundImageAspectType).ToString());
            }

            if (mMarkedAsChapter == true)
            {
                element.SetAttribute("ChapterMarker", mMarkedAsChapter.ToString());
            }
        }

        //*******************************************************************
        protected void LoadSlidePart(XmlElement element)
        {
            mUsePanZoom = bool.Parse(element.GetAttribute("PanZoom"));

            string s1 = element.GetAttribute("UsesDefaultTransitionEffect");

            if (s1 != "")
            {
                mUseDefaultTransistionEffect = bool.Parse(s1);
            }

            string s = element.GetAttribute("Rotate");
            if (s != "")
            {
                mRotation = int.Parse(s);
            }

            s1 = element.GetAttribute("DisplayTimeLength");
            if (s1 != "")
            {
                this.mDisplayTimeLength = float.Parse(s1, CultureInfo.InvariantCulture);
            }

            XmlNodeList list = element.GetElementsByTagName("TransitionEffect");

            // is a transition effect?
            if (list.Count > 0)
            {
                // Take first transistion effect
                XmlElement e = list[0] as XmlElement;

                mTransitionEffect = CTransitionEffect.CreateFromType(e.GetAttribute("Type"));
                if (mTransitionEffect != null)
                {
                    mTransitionEffect.Load(e);
                }
            }

            s1 = element.GetAttribute("ForceMBSubFrames");
            if (s1 != "")
            {
                this.mForcedMotionBlurSubFrames = int.Parse(s1);
            }

            s1 = element.GetAttribute("ThumbnailPreviewTime");
            if (s1 != "")
            {
                this.mThumbnailPreviewTime = float.Parse(s1);
            }

            s1 = element.GetAttribute("ReCalcSlideLengthOnImage1Change");
            if (s1 != "")
            {
                mReCalcSlideLengthOnImage1DecorationChange = bool.Parse(s1);
            }

            s1 = element.GetAttribute("MasterBackgroundAspect");
            if (s1 != "")
            {
                mMasterBackgroundImageAspect = float.Parse(s1);
            }

            s1 = element.GetAttribute("MastetAspectType");
            if (s1 != "")
            {
                mMasterBackgroundImageAspectType = (CImage.DrawImageWithAspectType)int.Parse(s1);
            }

            s1 = element.GetAttribute("ChapterMarker");
            if (s1 != "")
            {
                mMarkedAsChapter = bool.Parse(s1);
            }
        }
	}
}
