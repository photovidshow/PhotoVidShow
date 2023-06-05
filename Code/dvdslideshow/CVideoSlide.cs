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
    /// Summary description for CVideoSlide.
    /// </summary>
    public class CVideoSlide : CImageSlide
    {
        private CVideoPlayer mPlayer;

        // offsets from start and end of video in terms of seconds
        private double mStartVideoOffset = 0;
        private double mEndVideoOffset = 0;
        private bool mLoop = false;
        private float mMusicFadeWhilePlayingVideo = 0.50f; // half volume;
        private bool mEnableMusicFadeWhilePlayingVideo = false;
        private string mStringID = "";
        private string mRefernceStringID = "";

        private static CVideoSlide mDefaultVideoSlide = new CVideoSlide();

        public string ReferenceStringID
        {
            get { return mRefernceStringID; }
        }

        public string StringID
        {
            get { return mStringID; }
            set
            {
                mStringID = value;
                if (mPlayer != null)
                {
                    mPlayer.StringID = value;
                }
            }
        }


        public float MusicFadeWhilePlayingVideo
        {
            get { return mMusicFadeWhilePlayingVideo; }
            set
            {
                if (mMusicFadeWhilePlayingVideo != value)
                {
                    mMusicFadeWhilePlayingVideo = value;
                    CGlobals.mCurrentProject.DeclareChange(true, "Music fade changed");
                }
            }
        }

        public bool EnableMusicFadeWhilePlayingVideo
        {
            get { return mEnableMusicFadeWhilePlayingVideo; }
            set
            {
                if (value != mEnableMusicFadeWhilePlayingVideo)
                {
                    mEnableMusicFadeWhilePlayingVideo = value;
                    CGlobals.mCurrentProject.DeclareChange(true, "Enable Music fade changed");
                }
            }
        }

        private double mMasterVolume = 1.0;

        public double Volume
        {
            get { return mMasterVolume; }

            set
            {
                if (mMasterVolume != value)
                {
                    mMasterVolume = value;
                    CGlobals.mCurrentProject.DeclareChange(true, "Video Audio Volume changed");
                }
            }
        }

        public CVideoPlayer Player
        {
            get
            {
                return mPlayer;
            }
            set
            {
                mPlayer = value;
            }
        }

        public bool Loop
        {
            get
            {
                return mLoop;
            }
            set
            {
                mLoop = value;
                if (mPlayer != null)
                {
                    mPlayer.Loop = value;
                }
            }
        }

        public double StartVideoOffset
        {
            get
            {
                return mStartVideoOffset;
            }
            set
            {
                mStartVideoOffset = value;
            }
        }


        public double EndVideoOffset
        {
            get
            {
                return mEndVideoOffset;
            }
            set
            {
                mEndVideoOffset = value;
            }
        }


        //private bool mLengthIsVideoLength = true ;


        public override float DisplayLength
        {
            get
            {
                return (float)GetDurationInSeconds();
            }
            set
            {
                if (this.mLoop == true)
                {
                    base.DisplayLength = value;
                }
                else
                {
                    CDebugLog.GetInstance().Warning("Can not set display length on a non looping video slide");
                }
            }
        }

        public override void SetLengthWithoutUpdate(float val)
        {
            if (this.mLoop == true)
            {
                mDisplayTimeLength = val;
            }
            else
            {
                CDebugLog.GetInstance().Warning("Can not force set display length on non looping video slide");
            }
        }

        private static CPanZoom mGlobalVideoSlidePanZoom = new CPanZoom();

        public override CPanZoom PanZoom
        {
            get
            {
                if (mPlayer == null)
                {
                    return new CPanZoom();
                }
    
                // Legacy video slides don't have pan/zoom but motion blur calculations still query this, so
                // we simply return the static global VideoSlide panzoom
                return mGlobalVideoSlidePanZoom;
            }
            set
            {
            }
        }


        public override string SourceFilename
        {
            get
            {
                if (mPlayer == null)
                {
                    return "";
                }
                return mPlayer.mFileName;
            }
        }


        public CVideoSlide()
        {
        }

        //*******************************************************************
        public CVideoSlide(string filename)
        {
            if (filename != null && filename.Length > 0 && filename[0] != '.')
            {
                Init(filename);
            }
            else
            {
                this.InitVideoSlideUsingAnotherPlayer(filename);
            }
        }

        // create a video slide with the same player !! photocruz hack
        //********************************************************************
        public CVideoSlide(CVideoPlayer use_same_player)
        {
            mPlayer = use_same_player;
            this.SetPreviewPlaybackVolume(1.0);
        }

        //*******************************************************************
        public CVideoSlide(CVideoSlide copy)
            : base(copy)
        {
            Init(copy.mPlayer.mFileName);
        }

        //*******************************************************************
        public override CSlide Clone()
        {
            return new CVideoSlide(this);
        }

        //*******************************************************************
        private void Init(string filename)
        {
            try
            {
                mPlayer = new CVideoPlayer(mStringID);
                mPlayer.Load(filename);
                mPlayer.SeekToTime(this.mStartVideoOffset, true);
                CGlobals.MuteSound = true;
                mPlayer.Start(false);
                mPlayer.Stop(this.mStartVideoOffset);
                CGlobals.MuteSound = false;
                mPlayer.Loop = this.mLoop;
                this.SetPreviewPlaybackVolume(1.0);
            }
            catch
            {
                mPlayer = null;
            }

        }


        //*******************************************************************
        public int GetStartVideoOffsetFrame()
        {
            double frame = this.mStartVideoOffset;
            frame = frame * CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

            return (int)frame;
        }


        //*******************************************************************
        protected override CImage GetVideo(int frame, bool ignore_pan_zoom, bool ignore_decorations, int req_width, int req_height, CImage render_to_this)
        {

            CGlobals.DeclareEncodeCheckpoint('K');

            Surface surface = null;

            int rw = req_width;
            int rh = req_height;

            float aspect = CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction();

            if (this.Rotation == 90 ||
                this.Rotation == 90 + 180)
            {
                rh = req_width;
                rw = req_height;
                aspect = 1 / aspect;
            }

            if (mPlayer != null)
            {
                if (CGlobals.VideoSeeking == true)
                {
                    float sfo = this.mStartFrameOffset;
                    float time = frame - sfo + this.GetStartVideoOffsetFrame();
                    time /= CGlobals.mCurrentProject.DiskPreferences.frames_per_second;
                    surface = mPlayer.GetVideoSnapshot(time);
                }
                else
                {
                    if (surface == null)
                    {      
                        surface = mPlayer.GetNextImage(frame, (float)mStartVideoOffset, (float)mEndVideoOffset);     
                    }
                }
            }

            Rectangle dstRect = new Rectangle(0, 0, rw, rh);

            if (surface != null)
            {
                Rectangle srcRect = new Rectangle(0, 0, (int)surface.Width, (int)surface.Height);

                Rectangle bestFitRect = CGlobals.CalcBestFitRectagle(dstRect, srcRect, aspect);

                float x1 = bestFitRect.X;
                float y1 = bestFitRect.Y;

                float x2 = bestFitRect.X;
                float y2 = bestFitRect.Y + bestFitRect.Height;

                float x3 = bestFitRect.X + bestFitRect.Width;
                float y3 = bestFitRect.Y;

                float x4 = bestFitRect.X + bestFitRect.Width;
                float y4 = bestFitRect.Y + bestFitRect.Height;

                GraphicsEngine.Current.DrawImage(surface, new RectangleF(0, 0, 1, 1), x1, y1, x2, y2, x3, y3, x4, y4, false);
            }


            // should be back to normal req_width and reg_height after rotation

            if (this.mDecorations.Count > 0 && ignore_decorations == false)
            {
                this.RenderAllDecorations(dstRect, frame);
            }

            CGlobals.DeclareEncodeCheckpoint('L');
            return null;
        }


        //*******************************************************************
        public void ProcessRotation(CImage image)
        {
            System.Drawing.RotateFlipType rt = GetRotationFlipType();
            if (rt == System.Drawing.RotateFlipType.RotateNoneFlipNone)
            {
                return;
            }

            Image ri = image.GetRawImage();

            ri.RotateFlip(rt);

        }


        //*******************************************************************
        public void StopVideoPlayer()
        {
            if (mPlayer == null)
            {
                return;
            }

            //	if (mPlayer.mStatus != CVideoPlayer.EStatus.STOPPED_AT_START &&
            //		mPlayer.mStatus != CVideoPlayer.EStatus.STOPPED)
            //	{
            mPlayer.Reset(this.mStartVideoOffset);
            //	}
        }

        //*******************************************************************
        public override void StopAllNonPlayingMediaStreams(CSlide current_slide, CSlide next_slide)
        {
            base.StopAllNonPlayingMediaStreams(current_slide, next_slide);

            if (CVideoDecoration.IsPlayerUsedOnSlides(this.Player, current_slide, next_slide) == false)
            {
                this.StopVideoPlayer();
            }
        }


        //*******************************************************************
        public void ResetVideoPlayer()
        {
            if (mPlayer == null)
            {
                return;
            }

            mPlayer.Reset(this.mStartVideoOffset);
            this.SetPreviewPlaybackVolume(1.0);
            // mLastNickedSampleTime = -1;
        }


        //*******************************************************************
        public void SeekToTime(double time, bool wait)
        {
            if (mPlayer == null)
            {
                return;
            }

            mPlayer.SeekToTime(time, wait);
        }

        //*******************************************************************
        public override void ResetAllMediaStreams()
        {
            base.ResetAllMediaStreams();
            ResetVideoPlayer();
        }


        //*******************************************************************
        //	public void SetLengthIsVideoLength(bool val)
        //	{
        //		mLengthIsVideoLength = val ;
        //	}


        //*******************************************************************
        public override int CalcLengthInFrame(int offset)
        {
            //	if (mLengthIsVideoLength==false)
            //	{
            //	return base.CalcLengthInFrame(offset);
            //	}


            mStartFrameOffset = offset;

            int frames = (int)(GetDurationInSeconds() * ((float)CGlobals.mCurrentProject.DiskPreferences.frames_per_second));

            // SRG TO DO calc time to frames
            // mLengthInFrames = CalcFramesFromTime(mDisplayTimeLength);

            mLengthInFrames = frames;

            if (CGlobals.mLimitVideoSeconds > 0)
            {
                int ss2 = (int)(((float)CGlobals.mCurrentProject.DiskPreferences.frames_per_second) *
                    CGlobals.mLimitVideoSeconds);

                if (ss2 < mLengthInFrames)
                    mLengthInFrames = ss2;
            }

            int transitionTime = 0;
            if (TransistionEffect != null)
            {
                transitionTime = TransistionEffect.GetTotalRenderLengthInFrames() + 1;
            }

            // ok returns frame when next slide shoud begin
            return mStartFrameOffset + mLengthInFrames - transitionTime;
        }

        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc)
        {
            XmlElement slide = doc.CreateElement("Slide");

            slide.SetAttribute("Type", "VideoSlide");

            SaveImageSlidePart(slide, doc);

            if (mRefernceStringID != "")
            {
                slide.SetAttribute("VideoName", mRefernceStringID);
            }
            else
            {
                if (mPlayer != null)
                {
                    string filename = CGlobals.StripRootHeader(this.mPlayer.mFileName);
                    slide.SetAttribute("VideoName", filename);
                }
            }

            if (mStartFrameOffset != mDefaultVideoSlide.StartVideoOffset)
            {
                slide.SetAttribute("StartVideoOffset", this.mStartVideoOffset.ToString());
            }

            if (mEndVideoOffset != mDefaultVideoSlide.EndVideoOffset)
            {
                slide.SetAttribute("EndVideoOffset", this.mEndVideoOffset.ToString());
            }

            if (mMasterVolume != mDefaultVideoSlide.Volume)
            {
                slide.SetAttribute("Volume", this.mMasterVolume.ToString());
            }

            if (mLoop != mDefaultVideoSlide.Loop)
            {
                slide.SetAttribute("Loop", this.mLoop.ToString());
            }

            if (mMusicFadeWhilePlayingVideo != mDefaultVideoSlide.MusicFadeWhilePlayingVideo)
            {
                slide.SetAttribute("MusicFade", this.mMusicFadeWhilePlayingVideo.ToString());
            }

            if (mEnableMusicFadeWhilePlayingVideo != mDefaultVideoSlide.EnableMusicFadeWhilePlayingVideo)
            {
                slide.SetAttribute("EnableMusicFade", this.mEnableMusicFadeWhilePlayingVideo.ToString());
            }

            if (mStringID != "")
            {
                slide.SetAttribute("StringID", this.mStringID);
            }

            parent.AppendChild(slide);
        }


        //*******************************************************************
        public override void Load(XmlElement element)
        {
            LoadImageSlidePart(element);

            string filename = element.GetAttribute("VideoName");

            string ss = element.GetAttribute("StartVideoOffset");
            if (ss != "")
            {
                mStartVideoOffset = double.Parse(ss, CultureInfo.InvariantCulture);
            }

            ss = element.GetAttribute("EndVideoOffset");
            if (ss != "")
            {
                mEndVideoOffset = double.Parse(ss, CultureInfo.InvariantCulture);
            }

            ss = element.GetAttribute("Volume");
            if (ss != "")
            {
                this.mMasterVolume = double.Parse(ss, CultureInfo.InvariantCulture);
            }

            ss = element.GetAttribute("Loop");
            if (ss != "")
            {
                this.mLoop = bool.Parse(ss);
            }

            ss = element.GetAttribute("MusicFade");
            if (ss != "")
            {
                this.mMusicFadeWhilePlayingVideo = float.Parse(ss);

                ss = element.GetAttribute("EnableMusicFade");
                if (ss != "")
                {
                    this.mEnableMusicFadeWhilePlayingVideo = bool.Parse(ss);
                }
                else
                {
                    // legacy, before we had enable field
                    if (mMusicFadeWhilePlayingVideo < 0.999)
                    {
                        mEnableMusicFadeWhilePlayingVideo = true;
                    }
                    else
                    {
                        mEnableMusicFadeWhilePlayingVideo = false;
                    }
                }
            }

            if (filename != null && filename.Length > 0 && filename[0] != '.')
            {
                mStringID = element.GetAttribute("StringID");

                try
                {
                    Init(filename);
                }
                // cant ignore if part of a menu
                catch (IgnoreOperationException ioe)
                {
                    if (this.PartOfAMenu != true)
                    {
                        throw ioe;
                    }
                }
            }
            else
            {
                InitVideoSlideUsingAnotherPlayer(filename);
            }
        }

        //*******************************************************************
        private void InitVideoSlideUsingAnotherPlayer(string referenceStringID)
        {
            // Use another player 
            CVideoPlayer p = CVideoPlayerDatabase.GetInstance().GetPlayer(referenceStringID);
            if (p != null)
            {
                this.mPlayer = p;
                mRefernceStringID = referenceStringID;
            }
            else
            {
                SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromSlide(this);
                Log.Error("VideoSlide in slideshow '" + message.Slideshow + "' slide:" + message.SlideNumber + " could not find referenced player :" + referenceStringID);
            }
        }


        //*******************************************************************
        public override string GetSourceFileName()
        {
            if (mPlayer == null)
            {
                return "";
            }

            return mPlayer.mFileName;
        }

        //*******************************************************************
        public double GetDurationInSeconds()
        {
            if (mPlayer == null)
            {
                return 8;
            }

            if (this.mLoop == true)
            {
                return base.DisplayLength;
            }

            return mPlayer.GetDurationInSeconds() - (this.mStartVideoOffset + this.mEndVideoOffset);
        }

        //*******************************************************************
        public void SetPreviewPlaybackVolume(double vol)
        {
            double the_vol = this.mMasterVolume * vol;
            int db_loss = CGlobals.vol_ratio_to_dx9_dbloss((float)the_vol);
            if (mPlayer != null)
            {
                mPlayer.SetVolume(db_loss);
            }
        }


        //*******************************************************************
        public bool ContainsAudio()
        {
            if (mPlayer == null)
            {
                return false;
            }
            return this.mPlayer.ContainsAudio();
        }

        //*******************************************************************
        public void Stop()
        {
            if (mPlayer == null)
            {
                return;
            }
            this.mPlayer.Stop(this.mStartVideoOffset);
        }

        //*******************************************************************
        public bool HasDolbyAC3Sound()
        {
            return this.mPlayer.HasDolbyAC3Sound();
        }

        //*******************************************************************
        public double GetFrameRate()
        {
            if (mPlayer == null)
            {
                return 30;
            }

            return this.mPlayer.GetFrameRate();
        }

        //*******************************************************************
        public string GetFilename()
        {
            if (mPlayer == null)
            {
                return "";
            }

            return this.mPlayer.mFileName;
        }

        //*******************************************************************
        public override void RebuildToNewCanvas(CGlobals.OutputAspectRatio ratio)
        {

        }

        //*******************************************************************
        private void UnAttachAnyAttachedDecorations()
        {
            // you can't have attached decors on video slides so un attach
            // any attached decorations

            foreach (CDecoration d in mDecorations)
            {
                if (d.AttachedToSlideImage == true)
                {
                    d.AttachedToSlideImage = false;
                }
            }
        }


        //*******************************************************************
        public override void RenderAllDecorations(Rectangle r, int frame_num)
        {
            UnAttachAnyAttachedDecorations();
            RenderUnAttachedDecorations(r, frame_num, false, mFinalRenderSurface);
        }

    }
}
