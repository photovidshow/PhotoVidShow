using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using DVDSlideshow;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DVDSlideshow.GraphicsEng;


namespace CustomButton
{
    public class MiniPreviewController
    {
        //*************************************************************************************************
        private static MiniPreviewController mCurrentPlayingController = null;

        private CSlideShow mCurrentSlideshow = null;
        private CSlide mCurrentSlide = null;
        private Control mWindow = null;
        private CVideoClock mVideoClock = null;
        private bool mDrawingNextImage = false;
        public static Form mGlobalGuiFormThread = null;

        // offsets allow you to adjust which part of the preview is shown
        private float mStartOffsetTime = 0; // seconds
        private float mEndOffsetTime = 0; // seconds
        private bool mMuteSounds = true;
        private RenderVideoParameters.RenderCallbackDelegate mPostRenderCallback = null;
        private RenderVideoParameters.RenderCallbackDelegate mPreRenderCallback = null;


        public CSlideShow CurrentSlideshow
        {
            get { return mCurrentSlideshow; }
        }

        public CSlide CurrentSlide
        {
            get { return mCurrentSlide; }
        }

        public RenderVideoParameters.RenderCallbackDelegate PostRenderCallback
        {
            get { return mPostRenderCallback; }
            set { mPostRenderCallback = value; }
        }

        public RenderVideoParameters.RenderCallbackDelegate PreRenderCallback
        {
            get { return mPreRenderCallback; }
            set { mPreRenderCallback = value; }
        }

        public CSlide ForSlide
        {
            get { return mCurrentSlide; }
        }

        //*************************************************************************************************
        public float StartOffsetTime
        {
            get { return mStartOffsetTime; }
            set { mStartOffsetTime = value; }
        }

        //*************************************************************************************************
        public float EndOffsetTime
        {
            get { return mEndOffsetTime; }
            set { mEndOffsetTime = value; }
        }


        //*************************************************************************************************
        private int GetFramesStartOffset()
        {
            return (int) (mStartOffsetTime * CGlobals.mCurrentProject.DiskPreferences.frames_per_second);
        }

        //*************************************************************************************************
        private int GetFramesEndOffset()
        {
            return (int)(mEndOffsetTime * CGlobals.mCurrentProject.DiskPreferences.frames_per_second);
        }
     
        //*************************************************************************************************
        public static void StopAnyPlayingController()
        {
            if (mCurrentPlayingController != null)
            {
                mCurrentPlayingController.Stop();
                mCurrentPlayingController = null;
            }
        }


        //*************************************************************************************************
        public static MiniPreviewController GetCurrentPlayingController()
        {
            return mCurrentPlayingController;
        }

        //*************************************************************************************************
        public MiniPreviewController(CSlideShow forSlideshow, CSlide forSlide, Control pictureBox)
        {
            Init(forSlideshow, forSlide, pictureBox, true, null);
        }

        //*************************************************************************************************
        public MiniPreviewController(CSlideShow forSlideshow, CSlide forSlide, Control pictureBox, CMusicSlide musicSlide)
        {
            mMuteSounds = false;
            Init(forSlideshow, forSlide, pictureBox, true, musicSlide);
        }

        //*************************************************************************************************
        public MiniPreviewController(CSlideShow forSlideshow, CSlide forSlide, Control pictureBox, float startOffsetTime, float endOffsetTime)
        {
            mStartOffsetTime = startOffsetTime;
            mEndOffsetTime = endOffsetTime;

            Init(forSlideshow, forSlide, pictureBox, true, null);
        }

        //*************************************************************************************************
        public MiniPreviewController(CSlideShow forSlideshow, CSlide forSlide, Control pictureBox, bool playing)
        {
            Init(forSlideshow, forSlide, pictureBox, playing, null);
        }

        //*************************************************************************************************
        private void Init(CSlideShow forSlideshow, CSlide forSlide, Control pictureBox, bool playing, CMusicSlide musicslide)
        {
            mCurrentSlideshow = forSlideshow;

            // No slideshow? create one for it
            if (mCurrentSlideshow == null)
            {
                mCurrentSlideshow = new CSlideShow("temp");
                mCurrentSlideshow.FadeOut = false;
                mCurrentSlideshow.FadeIn = false;
                ArrayList list = new ArrayList();
                list.Add(forSlide);
                mCurrentSlideshow.AddSlides(list,false,null,null,false);
            }

            if (musicslide != null)
            {
                mCurrentSlideshow.mMusicSlides.Add(musicslide);
            }

            mCurrentSlide = forSlide;
            mWindow = pictureBox;

            if (playing == true)
            {
                mVideoClock = new CVideoClock();
                mVideoClock.Tick += new VideoClockTickHandler(this.Tick);

                if (mCurrentPlayingController != null)
                {
                    mCurrentPlayingController.Stop();
                }

                mCurrentPlayingController = this;

                Tick(0);

                mVideoClock.Start();
            }         
        }

        //*************************************************************************************************
        public void Pause()
        {
            if (mVideoClock != null)
            {
                mVideoClock.StopAndDontSendAnyCallbacks();
            }
        }

        //*************************************************************************************************
        public void Continue()
        {
            if (mVideoClock != null)
            {              
                mVideoClock.Start();
            }
        }

        //*************************************************************************************************
        public void Stop()
        {
            if (mVideoClock != null && mVideoClock.Running == true)
            {
                mVideoClock.StopAndDontSendAnyCallbacks();
                mVideoClock.Frame = 0;
                mCurrentSlideshow.ResetAllMediaStreams();
            }
        }

        //*************************************************************************************************
        public bool IsRunning()
        {
            if (mVideoClock == null) return false;
            return mVideoClock.Running;
        }

        //*************************************************************************************************
        public void ResetFrameCountToStart()
        {
            //
            // Ok we need to reset all media streams and set the video clock to 0.  As the clock is still running 
            // (in it's own thread) we need to turn off sending ticks whilst this is hapenning.
            //
            mVideoClock.GainRunLock();
            try
            {
                mCurrentSlideshow.ResetAllMediaStreams();
                mVideoClock.Frame = 0;
            }
            finally
            {
                mVideoClock.ReleaseRunLock();
            }
        }

        //*************************************************************************************************
        public void ResetFrameCountToSlideTime(float seconds)
        {        
            int frame = (int) ( CGlobals.mCurrentProject.DiskPreferences.frames_per_second * seconds);
            if (frame < 0) frame = 0;
            if (frame > this.mCurrentSlide.GetFrameLength() - 1) frame = 0;

            mVideoClock.Frame = frame;

        }


        //*************************************************************************************************
        private delegate void ResetAllMediaStreamsInternalDelegate();
        private void ResetAllMediaStreamsInternal()
        {
            // Make sure this is done on GUI thread to be safe
            if (mGlobalGuiFormThread.InvokeRequired == true)
            {
                if (mDrawingNextImage == true) return;

                IAsyncResult aResult = mGlobalGuiFormThread.BeginInvoke(new ResetAllMediaStreamsInternalDelegate(ResetAllMediaStreamsInternal));
                return;
            }

            mCurrentSlideshow.ResetAllMediaStreams();    
        }

        //*************************************************************************************************
        private void Tick(int frame)
        {
            int actualFrame = frame + this.GetFramesStartOffset();

            if (actualFrame >= this.mCurrentSlide.GetFrameLength() - 1 - GetFramesEndOffset())
            {
                mVideoClock.Frame = 0;
                ResetAllMediaStreamsInternal();
                if (mVideoClock.SendTicks == false)
                {
                    return;
                }
                actualFrame = this.GetFramesStartOffset();
            }

            DoSnapshot(actualFrame, false);
        }

        //*************************************************************************************************
        public Size GetFrameSize()
        {
            int ww = mWindow.Width;
            float rat = CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction();
            int hh = (int)(((float)ww) * rat);

            if (ww == 0 || hh == 0)
            {
                ww = 10;
                hh = 10;
            }

            //
            // If the current default render surface (E.g. the current DirectX Back buffer) is smaller than preview 
            // window size, then reduce in size, keeping the same aspect   This situation can occur if the user 
            // shrinks the PhotoVidShow app such that it is smaller that when it initially opened
            //
            int defaultSurfaceWidth = GraphicsEngine.Current.GetDefaultSurfaceWidth();
            int defaultSurfaceHeight = GraphicsEngine.Current.GetDefaultSurfaceHeight();
            if (ww > defaultSurfaceWidth || hh > defaultSurfaceHeight)
            {
                Rectangle r = CGlobals.CalcBestFitRectagle(new Rectangle(0,0,defaultSurfaceWidth, defaultSurfaceHeight), new Rectangle(0,0,ww,hh));
                ww = r.Width;
                hh = r.Height;
            }

            return new Size(ww, hh);
        }


        //*************************************************************************************************
        private delegate void DoSnapshotDelegate(int frame, bool forceNoVideoSeeking);
        public void DoSnapshot(int snapshotFrame, bool forceNoVideoSeeking)
        {
            //
            // Make sure that this can only be called one at a time
            //
            if (mDrawingNextImage == true)
            {
                return;
            }

            mDrawingNextImage = true;

            // Always make sure this is done on GUI thread (this can be called from Tick() )
            if (mGlobalGuiFormThread.InvokeRequired == true)
            {

                mGlobalGuiFormThread.BeginInvoke(new DoSnapshotDelegate(DoSnapshotInternal),
                                          new object[] { snapshotFrame, forceNoVideoSeeking });
                return;
            }
            else
            {
                DoSnapshotInternal(snapshotFrame, forceNoVideoSeeking);
            }
        }

        //*************************************************************************************************
        private void DoSnapshotInternal(int snapshotFrame, bool forceNoVideoSeeking)
        {
            try
            {
                if (mVideoClock != null && mVideoClock.Running == false)
                {
                    return;
                }

                CSlideShow slideshow = mCurrentSlideshow;

                if (mWindow == null || slideshow == null) return;

                int start_f = this.mCurrentSlide.mStartFrameOffset;

                int frame = start_f + snapshotFrame;

                bool mute_sound = CGlobals.MuteSound;
                bool vs = CGlobals.VideoSeeking;
                bool wsc = CGlobals.WaitVideoSeekCompletion;
                CGlobals.MuteSound = mMuteSounds;

                // need to reset videos on each start of loop
                if (snapshotFrame == 0 || snapshotFrame == 1 || (mVideoClock == null && forceNoVideoSeeking == false) )
                {
                    CGlobals.VideoSeeking = true;
                    CGlobals.WaitVideoSeekCompletion = true;
                }
                else
                {
                    CGlobals.VideoSeeking = false;
                    CGlobals.WaitVideoSeekCompletion = false;
                }

                Size size = GetFrameSize();

                int ww = size.Width;
                int hh = size.Height;

                try
                {
                    RenderVideoParameters rp = new RenderVideoParameters();
                    rp.frame = frame;
                    rp.req_width = ww;
                    rp.req_height = hh;
                    rp.present_direct_to_window = true;
                    rp.present_window = mWindow.Handle;
                    rp.postRenderDelegate = this.mPostRenderCallback;
                    rp.preRenderDelegate = this.mPreRenderCallback;   

                    if (mWindow.Width > ww && mWindow.Height > hh)
                    {
                        rp.scale_back_buffer_to_present_window = true;
                    }

                    slideshow.RenderVideo(rp);
                }
                catch
                {
                }

                CGlobals.VideoSeeking = vs;
                CGlobals.WaitVideoSeekCompletion = wsc;
                CGlobals.MuteSound = mute_sound;
            }
            finally
            {
                mDrawingNextImage = false;
            }

        }
    }
}
