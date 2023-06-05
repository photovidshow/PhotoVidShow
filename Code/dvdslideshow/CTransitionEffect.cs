/*
 * Created by SharpDevelop.
 * User: user
 * Date: 03/01/2005
 * Time: 15:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml;
using ManagedCore;
using System.Globalization;
using DVDSlideshow.GraphicsEng;


namespace DVDSlideshow
{ 
	/// <summary>
	/// Description of CTransitionEffect.	
	/// </summary>
	public abstract class CTransitionEffect
	{
        //
        // Used in normal slide transition
        //
		protected float mLength = 1.5f; // in seconds
		private int mIndex =-1;
        protected bool mUseImage2Alpha = false;
        protected bool mNeedsDualRenderSurface = false;
        protected int mForecedMotionBlur = 1;
        protected bool mRenderNextSlideBeforeCurrentSlide = false;

        //
        // used in animation decorations
        //
        private float initialDelay = 0;
        private float endDelay = 0;

        public float InitialDelay
        {
            get { return initialDelay; }
            set { initialDelay = value; }
        }

        public float EndDelay
        {
            get { return endDelay; }
            set { endDelay = value; }
        }

        public float GetDeltaAferDelay(float delta)
        {
            if (delta < initialDelay) return 0;
            if (delta > 1.0 - endDelay) return 1;
            if (initialDelay + endDelay >= 1) return 1;

            delta -= initialDelay;
            delta *= (1.0f / (1.0f - initialDelay - endDelay));
            if (delta < 0) delta = 0;
            if (delta > 1) delta = 1;
            return delta;
        }

        public virtual bool RenderNextSlideBeforeCurrentSlide
        {
            get { return mRenderNextSlideBeforeCurrentSlide; }
        }

        public virtual float Length
        {
            get { return mLength; }
            set { mLength = value; }
        }

        public virtual bool NeedsDuelRenderSurface
        {
            get { return mNeedsDualRenderSurface; }
        }

        public virtual bool UseImage2Alpha
        {
            get { return mUseImage2Alpha; }
            set { mUseImage2Alpha = value; }
        }

        public virtual int ForcedMB
        {
            get { return mForecedMotionBlur; }
            set { mForecedMotionBlur = value; }
        }

      
        // because when playing slideshows we often need to do transitions so we need to two images before create a third merged image.
        // as odds are these two temporary buffers will be the same as last time we did a transisiton its best to cache these bitmaps
        // rather then trusting .net to new and delete them all the time
       //  private static CImage mSlideImageBuffer1 = null;
       //  private static CImage mSlideImageBuffer2 = null;

        // used to say which part of the slide to do the transition on
        // current only used on PhotoCruz version  
        // This part is saved out
        private RectangleF? mTransitionRegion;

        public RectangleF TransitionRegion
        {
            set { mTransitionRegion = value; }
        }

		public int Index
		{
			get
			{
				return mIndex;
			}
			set
			{
				mIndex = value;
			}
		}


		//*******************************************************************
		public CTransitionEffect()
		{
		}

		//*******************************************************************
        public void SaveTransitionPart(XmlElement element, XmlDocument doc)
		{
			element.SetAttribute("Length",mLength.ToString());

            if (mIndex != -1)
            {
                element.SetAttribute("Index", mIndex.ToString());
            }

            if (mTransitionRegion.HasValue == true)
            {
                CGlobals.SaveRectangle(element, doc, "TransitionRegion", mTransitionRegion.Value);
            }

            if (initialDelay != 0.0)
            {
                element.SetAttribute("InitialDelay", initialDelay.ToString());
            }

            if (endDelay != 0.0)
            {
                element.SetAttribute("EndDelay", endDelay.ToString());
            }

            if (mRenderNextSlideBeforeCurrentSlide == true)
            {
                element.SetAttribute("RenderNextFirst", mRenderNextSlideBeforeCurrentSlide.ToString());
            }
		}


		//*******************************************************************
		public void LoadTransitionPart(XmlElement element)
		{
			mLength = float.Parse(element.GetAttribute("Length"),CultureInfo.InvariantCulture);	

			mIndex=-1;

			string s=  element.GetAttribute("Index");
			if (s!="")
			{
				mIndex = int.Parse(s);
			}

            XmlNodeList list = element.GetElementsByTagName("TransitionRegion");
			if (list.Count >0) 
            {
                mTransitionRegion = CGlobals.LoadRectangle(element, "TransitionRegion");
            }

            string s1 = element.GetAttribute("InitialDelay");

            if (s1 != "")
            {
                initialDelay = float.Parse(s1);
            }

            s1 = element.GetAttribute("EndDelay");
            if (s1 != "")
            {
                endDelay = float.Parse(s1);
            }

            s1 = element.GetAttribute("RenderNextFirst");
            if (s1 != "")
            {
                mRenderNextSlideBeforeCurrentSlide = bool.Parse(s1);
            }
		}

		//*******************************************************************
		public CTransitionEffect(float length)
		{
			mLength = length ;
		}

        //*******************************************************************
        // Called to do slide transition
        public virtual void Process(float delta,
                                    CSlide thisSlide,
                                    CSlide nextSlide,
                                    int frame,
                                    int req_width,
                                    int req_height,
                                    bool ignore_pan_zoom)
        {
            if (mNeedsDualRenderSurface == true)
            {
                ProcessDualRenderSurfaces(delta, thisSlide, nextSlide, frame, req_width, req_height, ignore_pan_zoom);
                return;
            }

            float adjusted_delta = GetAdjustedDelta(delta);

            // Always render this slide if any of the below is true 
            // e.g. (for photocruz a delta may be 1 but a transition region is defined, so still render this frame)
            if (adjusted_delta < 1 || nextSlide == null || mTransitionRegion.HasValue)
            {
                thisSlide.RenderFrame(frame, ignore_pan_zoom, false, req_width, req_height);
            }

            //
            // This has to be > zero not zero.
            // I did a change here that said it needs to be zero for video decorations to start, but this causes other problems
            // mainly in the templates that contain groups, e.g. pan wooden frame, being blank. 
            //
            if (nextSlide != null && delta > 0 && adjusted_delta > 0)
            {
                using (RenderSurface nextSlideSurface = GraphicsEngine.Current.GenerateRenderSurface((uint)req_width, (uint)req_height, this.ToString() + "::Process"))
                {
                    RenderSurface current_surface = GraphicsEngine.Current.GetRenderTarget();

                    GraphicsEngine.Current.SetRenderTarget(nextSlideSurface);
                    GraphicsEngine.Current.ClearRenderTarget(0, 0, 0, 255);

                    nextSlide.RenderFrame(frame, ignore_pan_zoom, false, req_width, req_height);

                    GraphicsEngine.Current.SetRenderTarget(current_surface);

                    // shall we only do transition on a certain region?
                    // needed for PhotoCruz thing

                    Rectangle? innerRegion = null;
                    if (mTransitionRegion.HasValue)
                    {
                        // need to create to sub images 
                        float x = ((float)req_width) * this.mTransitionRegion.Value.X;
                        float y = ((float)req_height) * this.mTransitionRegion.Value.Y;
                        float width = ((float)req_width) * this.mTransitionRegion.Value.Width;
                        float height = ((float)req_height) * this.mTransitionRegion.Value.Height;

                        innerRegion = CGlobals.ToRectangle(x, y, width, height);
                    }

                    Process2(delta, nextSlideSurface, frame, innerRegion);
                }
            }
        }

        //*******************************************************************
        // Some transition effects alter this and next slide, in this
        // case we need dual render surfaces
        private void ProcessDualRenderSurfaces(float delta,
                                     CSlide thisSlide,
                                     CSlide nextSlide,
                                     int frame,
                                     int req_width,
                                     int req_height,
                                     bool ignore_pan_zoom)
        {
            if (nextSlide == null || delta == 0)
            {
                thisSlide.RenderFrame(frame, ignore_pan_zoom, false, req_width, req_height);
                return;
            }

            RenderSurface current_surface = GraphicsEngine.Current.GetRenderTarget();;

            if (mRenderNextSlideBeforeCurrentSlide == true)
            {
                using (RenderSurface nextSlideSurface = GraphicsEngine.Current.GenerateRenderSurface((uint)req_width, (uint)req_height, this.ToString() + "::ProcessDualRenderSurfaces2"))
                {
                    GraphicsEngine.Current.SetRenderTarget(nextSlideSurface);
                    GraphicsEngine.Current.ClearRenderTarget(0, 0, 0, 255);

                    nextSlide.RenderFrame(frame, ignore_pan_zoom, false, req_width, req_height);

                    GraphicsEngine.Current.SetRenderTarget(current_surface);

                    ProcessDualRenderSurfaces(delta, null, nextSlideSurface, frame);
                }
            }

            // This is done in two passes to try and reduce memory usage
            using (RenderSurface thisSlideSurface = GraphicsEngine.Current.GenerateRenderSurface((uint)req_width, (uint)req_height, this.ToString() + "::ProcessDualRenderSurfaces"))
            {
                GraphicsEngine.Current.SetRenderTarget(thisSlideSurface);
                GraphicsEngine.Current.ClearRenderTarget(0, 0, 0, 255);

                thisSlide.RenderFrame(frame, ignore_pan_zoom, false, req_width, req_height);

                GraphicsEngine.Current.SetRenderTarget(current_surface);

                ProcessDualRenderSurfaces(delta, thisSlideSurface, null, frame);
            }

            if (mRenderNextSlideBeforeCurrentSlide == false)
            {
                using (RenderSurface nextSlideSurface = GraphicsEngine.Current.GenerateRenderSurface((uint)req_width, (uint)req_height, this.ToString() + "::ProcessDualRenderSurfaces2"))
                {
                    GraphicsEngine.Current.SetRenderTarget(nextSlideSurface);
                    GraphicsEngine.Current.ClearRenderTarget(0, 0, 0, 255);

                    nextSlide.RenderFrame(frame, ignore_pan_zoom, false, req_width, req_height);

                    GraphicsEngine.Current.SetRenderTarget(current_surface);

                    ProcessDualRenderSurfaces(delta, null, nextSlideSurface, frame);
                }
            }
        }

        //*******************************************************************
        protected virtual void ProcessDualRenderSurfaces(float delta, RenderSurface thisSlideSurface, RenderSurface nextSlideSurface, int frame)
        {
        }

        //*******************************************************************
        public void SimpleProcessDualRenderSurfaces(float delta, RenderSurface thisSlideSurface, RenderSurface nextSlideSurface, int frame)
        {
            ProcessDualRenderSurfaces(delta, thisSlideSurface, nextSlideSurface, frame);
        }

		//*******************************************************************
        // Called to do decoration transition
        public void Process(RenderSurface to_draw, int req_width, int req_height, float delta, int frame, Rectangle? innerRegion)
        {
            Process2(delta, to_draw, frame, innerRegion);
        }

        //*******************************************************************
        public CImage SimpleProcess(float delta, CImage i1, CImage i2, int frame, CImage render_to_this)
        {
            return Process(delta, i1,i2,frame, render_to_this);
        }


		//*******************************************************************
        protected virtual CImage Process(float delta, CImage i1, CImage i2, int frame, CImage render_to_this)
        {
            Log.Error("CTransitionEffect::Process method no longer supported");
            return null;
        }

        //*******************************************************************
        protected virtual void Process2(float delta, RenderSurface nextSlideSurface, int frame, Rectangle? innerRegion)
        {
            Log.Error("CTransitionEffect::process2 was called which was un-expected");
        }


        //*******************************************************************
        public virtual void SimpleProcess2(float delta, RenderSurface nextSlideSurface, int frame, Rectangle? innerRegion)
        {
            Process2(delta, nextSlideSurface, frame, innerRegion);
        }

		//*******************************************************************
        public abstract CTransitionEffect Clone();

		//*******************************************************************
        public int GetTotalRenderLengthInFrames()
		{
			return (int)(mLength * ((float)CGlobals.mCurrentProject.DiskPreferences.frames_per_second)) ;
		}

        //*******************************************************************
        protected virtual float GetAdjustedDelta(float delta)
        {
            return delta;
        }

		//*******************************************************************
		public virtual void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
		{
		}

		
		//*******************************************************************
		public virtual void Load(XmlElement element)
		{
		}

		//*******************************************************************
		public static CTransitionEffect CreateFromType(string type)
		{
			System.Random r = new Random();

			if (type=="BlurTransitionEffect") return new BlurTransitionEffect();
			if (type=="SimpleAlphaBlendTransitionEffect") return new SimpleAlphaBlendTransitionEffect();
            if (type == "AlphaBlendSlideMixer") return new AlphaBlendSlideMixer();
			if (type=="MiddleColorTransitionEffect") return new MiddleColorTransitionEffect();
            if (type=="MiddleSaturateToColorTransitionEffect") return new MiddleSaturateToColorTransitionEffect();
			if (type=="AlphaSwipeTransitionEffect") return new AlphaSwipTransitionEffect();
			if (type=="FadeDownTransitionEffect") return new FadeDownTransitionEffect();
			if (type=="FadeUpTransitionEffect") return new FadeUpTransitionEffect();
			if (type=="RandomFizzTransitionEffect") return new RandomFizzTransition();
			if (type=="CircleInTransitionEffect") return new CircleInTransitionEffect();
			if (type=="CircleOutTransitionEffect") return new CircleOutTransitionEffect();
			if (type=="RandomTransitionEffect") return new RandomTransitionEffect();
			if (type=="FadeToBWTransitionEffect") return new FadeToBWTransitionEffect();
			if (type=="HBlurWhiteout") return new HBlurWhiteout();
            if (type=="MultipleLinesUpDownTransitionEffect") return new MultipleLinesUpDownTransitionEffect();
            if (type=="MultipleLinesLeftRightTransitionEffect") return new MultipleLinesLeftRightTransitionEffect();
            if (type == "MultipleLinesDiagonalTransitionEffect") return new MultipleLinesDiagonalTransitionEffect();
            if (type == "MultiCirclesInOutTransitionEffect") return new MultiCirclesInOutTransitionEffect();
            if (type == "MiddleSplitUpDownTransition") return new MiddleSplitUpDownTransition();
            if (type == "MiddleSplitLeftRightTransition") return new MiddleSplitLeftRightTransition();
            if (type == "MiddleSplitDiagonalTransitionEffect") return new MiddleSplitDiagonalTransitionEffect();
            if (type == "DiagonalSquaresSaturateTransitionEffect") return new DiagonalSquaresSaturateTransitionEffect();
            if (type == "CameraSnapshotTransitionEffect") return new CameraSnapshotTransitionEffect();
            if (type == "MoveToNextSlideTransitionEffect") return new MoveToNextSlideTransitionEffect();
            if (type == "MovementTransitionEffect") return new MovementTransitionEffect();
            if (type == "RadialTransitionEffect") return new RadialTransitionEffect();
            if (type == "MultiSquaresInOutTransitionEffect") return new MultiSquaresInOutTransitionEffect();
            if (type == "MultiSquaresAlternateInOutTransitionEffect") return new MultiSquaresAlternateInOutTransitionEffect();
            if (type == "MultiCrossInOutTransitionEffect") return new MultiCrossInOutTransitionEffect();
            if (type == "WibbleTransitionEffect") return new WibbleTransitionEffect();
            if (type == "Shader075ThenAlphaBlendTransitionEffect") return new Shader075ThenAlphaBlendTransitionEffect();
            if (type == "ShaderPlusAlphaBlendTransitionEffect") return new ShaderPlusAlphaBlendTransitionEffect();
            if (type == "ShaderMixTransitionEffect") return new ShaderMixTransitionEffect();
            if (type == "CloudTransitionEffect") return new CloudTransitionEffect();
            if (type == "NextSlideBlurInTransitionEffect") return new NextSlideBlurInTransitionEffect();
            if (type == "BatmanTransitionEffect") return new BatmanTransitionEffect();
            if (type == "DiamondTransitionEffect") return new DiamondTransitionEffect();
            if (type == "RectangleTransitionEffect") return new RectangleTransitionEffect();
            if (type == "CrossTransitionEffect") return new CrossTransitionEffect();
            if (type == "FanTransitionEffect") return new FanTransitionEffect();

			CDebugLog.GetInstance().Error("Trying to create an unknown transition effect :"+type);

			return new SimpleAlphaBlendTransitionEffect();
		}
	}

}
