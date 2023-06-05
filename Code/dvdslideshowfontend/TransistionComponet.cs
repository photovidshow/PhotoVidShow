using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;
using DVDSlideshow;
using ManagedCore;
using DVDSlideshow.GraphicsEng;

namespace dvdslideshowfontend
{
	/// <summary>
	/// Summary description for TransitionComponent.
	/// </summary>
	/// 

    // represents the TransitionComponent icon in the scrollable slidehow view
	public class TransitionComponent : StoryboardComponent
	{
		private Image mImage = null;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
	
		private CSlide mSlide;
		private CSlide mNextSlide;
        private CSlideShow mSlideshow;
        private TransitionComponentControl mTransitionControl = null;
        private Pen mWhitePen = new Pen(Color.White, 2);

        public TransitionComponentControl TransitionControl
        {
            get { return mTransitionControl; }
            set
            {
                mTransitionControl = value;
                if (mTransitionControl != null)
                {
                    mTransitionControl.ParentTransition = this;
                    mTransitionControl.SetImage(mImage);
                }
            }
        }

        public CSlide Slide
        {
            get { return mSlide; }
        }

        public CSlide NextSlide
        {
            get { return mNextSlide; }
            set { mNextSlide = value; }
        }

		private object thislock = new object();

		//*******************************************************************
		public void RebuildCurrentTransPictureBox()
		{
			lock(thislock)
			{
				if (mNextSlide==null)
				{
					return ;
				}

                int ww = this.Width;
                float rat = CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction();
                int hh = (int)(((float)ww) * rat);

                Image newBitmap = mImage;

                if (newBitmap != null && (newBitmap.Width != ww || newBitmap.Height != hh))
                {
                    newBitmap = null;
                }

                try
                {
                    CTransitionEffect te = mSlide.TransistionEffect;

                    //
                    // Draw no tranisition effect white cross?
                    //
                    if (te.Length <= 0.0f)
                    {                  
                        if (newBitmap ==null)
                        {
                            newBitmap = new Bitmap(ww, hh);
                        }

                        using (Graphics g = Graphics.FromImage(newBitmap)) 
                        {
                            g.Clear(Color.Black);
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                            Point point1 = new Point(0, 0);
                            Point point2 = new Point(0, hh);
                            Point point3 = new Point(ww, hh);
                            Point point4 = new Point(ww, 0);

                            g.DrawLine(mWhitePen, point1, point3);
                            g.DrawLine(mWhitePen, point2, point4);
                        }
                    }
                    else
                    {
                        bool vs = CGlobals.VideoSeeking;
                        bool wbsc = CGlobals.WaitVideoSeekCompletion;
                        bool mute_sound = CGlobals.MuteSound;

                        CGlobals.VideoSeeking = true;
                        CGlobals.WaitVideoSeekCompletion = true;
                        CGlobals.MuteSound = true;
                    

                        RenderVideoParameters renderParameters = new RenderVideoParameters();
                        renderParameters.req_height = hh;
                        renderParameters.req_width = ww;
                        renderParameters.renderToBitmap = newBitmap as Bitmap;

                        float half_transition_length_in_frames = (te.Length / 2.0f) * CGlobals.mCurrentProject.DiskPreferences.frames_per_second;
                        float mid_transition_frame = mSlide.CalcEndFrame() - half_transition_length_in_frames;

                        renderParameters.frame = (int)mid_transition_frame;
                        renderParameters.ignore_pan_zoom = true;
                        renderParameters.ignore_decorations = true;
                        newBitmap = mSlideshow.RenderVideoToBitmap(renderParameters);

                        CGlobals.WaitVideoSeekCompletion = wbsc;
                        CGlobals.VideoSeeking = vs;
                        CGlobals.MuteSound = mute_sound;
                    }

                    if (mTransitionControl != null)
                    {
                        mTransitionControl.SetImage(newBitmap);
                    }

                    if (mImage!=null && mImage != newBitmap)
                    {
                        mImage.Dispose();
                    }

                    mImage = newBitmap;
                }
                catch (Exception exception)
                {
                  	CDebugLog.GetInstance().Error("Exception thrown when trying to generate transition component bitmap: "+exception.Message);
                }
			}
		}

		//*******************************************************************
		public TransitionComponent(CImageSlide for_slide, CSlideShow for_slideshow)
		{
			mSlide = for_slide;
            mSlideshow = for_slideshow;

            mWidth = 40;
            mHeight = 31;
			CSlideShow ss = Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow;

			mNextSlide = ss.GetNextSlide(mSlide);
			if (mNextSlide==null)
			{
				CDebugLog.GetInstance().Error("Next slide was null when building transition picture box");
				return ;
			}
		}

		//*******************************************************************
		public void Blank()
		{
			mSlide=null;
			mNextSlide=null;
            mSlideshow = null;
            if (mTransitionControl != null)
            {
                mTransitionControl.ParentTransition = null;
                mTransitionControl.SetImage(null); // just in case
            }
            mTransitionControl = null;

            if (mImage != null)
            {
                mImage.Dispose();
                mImage = null;
            }
		}

		//*******************************************************************
        public void ReConstruct(CImageSlide for_slide, CSlideShow for_slideshow)
        {
            mSlide = for_slide;
            mSlideshow = for_slideshow;
            mNextSlide = mSlideshow.GetNextSlide(mSlide);
            if (mNextSlide == null)
            {
                CDebugLog.GetInstance().Error("Next slide was null when re-constructing transition picture box");
                return;
            }
        }


        //*******************************************************************
        // This is called from child TransitionComponentControl because this code contains a rebuildPanel call which
        // which will then invalidate the child TransitionComponentControl
        public void TansitionEffectClicked(Point p)
        {
            if (mSlide == null) return;

            Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();
            Form1.mMainForm.GoToMainMenu();

            TransitionSelectionMenu t;

            try
            {
                t = new TransitionSelectionMenu(mSlide, Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow);
            }
            catch (Exception e2)
            {
                CDebugLog.GetInstance().Error("Exception occurred on creation of TransitionSelectionMenu: "+e2.Message);
                return;
            }

            t.StartPosition = FormStartPosition.Manual; ;
            t.Bounds = new Rectangle(p.X, p.Y - 450, t.Width, t.Height);
            t.ShowDialog();

            if (t.TransistionTimesChanged == true)
            {
                CSlideShow ss = Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow;
                if (ss != null)
                {
                    ss.InValidateLength();
                    ss.CalcLengthOfAllSlides();
                    ss.ReCalcLoopMusicSlides();

                    Form1.mMainForm.GetSlideShowManager().RebuildPanel(null, Form1.mMainForm.GetSlideShowManager().ScrollPosition);
                }
            }
            if (t.ChangedAllTransitions == true)
            {
                Form1.mMainForm.GetSlideShowManager().RebuildAllTransitions();
            }
            else
            {
                RebuildCurrentTransPictureBox();
            }
        }
	}
}
