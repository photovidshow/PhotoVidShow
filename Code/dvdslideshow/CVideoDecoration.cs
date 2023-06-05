using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using ManagedCore;
using System.Globalization;
using DVDSlideshow.GraphicsEng;
using System.Collections.Generic;
using System.Collections;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for CVideoDecoration .
	/// </summary>
	public class CVideoDecoration : CImageDecoration
	{
		private CVideoPlayer mPlayer ;
        private double mStartVideoOffset = 0;
        private double mEndVideoOffset = 0;
        private float mMusicFadeWhilePlayingVideo = 0.5f; 
        private bool mEnableMusicFadeWhilePlayingVideo = false; // no fade
        private bool mGlobalPlayer = false;   
        private string mStringID = "";
        private string mRefernceStringID = "";
        private bool mFadeAudioIn = true;   // fade video audio in at start
        private float mFadeAudioInLength = 0.2f;
        private bool mFadeAudioOut = true;  // fade video audio out at end
        private float mFadeAudioOutLength = 0.2f;
        private CMusicPeformanceAudioUnit mPreviewAudioUnit; // reference to audio unit in music performance

        private static Random mStringIdRandomStream = null;

        public CMusicPeformanceAudioUnit PreviewAudioUnit
        {
            set { mPreviewAudioUnit = value; }
            get { return mPreviewAudioUnit;  }
        }

        public static string GenerateRandomStringId()
        {
            if (mStringIdRandomStream == null)
            {
                string hash = DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString();
                int iHash = Math.Abs(hash.GetHashCode());

                mStringIdRandomStream = new Random(iHash);
            }

            return ".videolink" + mStringIdRandomStream.Next();
        }

        public bool UseGlobalPlayer
        {
            set { mGlobalPlayer = value; }
            get { return mGlobalPlayer; }
        }

        private double mVolume = 1.0;

        public string StringID
        {
            get { return mStringID;}
            set 
            { 
                mStringID = value;
                if (mPlayer != null)
                {
                    mPlayer.StringID = value;
                }
            }
        }

        public string ReferenceStringId
        {
            get { return mRefernceStringID; }
        }

        public double Volume
        {
            get { return mVolume; }

            set
            {
                if (mVolume != value)
                {
                    mVolume = value;
                    CGlobals.mCurrentProject.DeclareChange(true, "Video Audio Volume changed");
                }
            }
        }

        private static CVideoDecoration mDefaultVideoDecoration = new CVideoDecoration();

        public float MusicFadeWhilePlayingVideo
        {
            get { return mMusicFadeWhilePlayingVideo; }
            set { mMusicFadeWhilePlayingVideo = value; }
        }

        public bool EnableMusicFadeWhilePlayingVideo
        {
            get { return mEnableMusicFadeWhilePlayingVideo; }
            set { mEnableMusicFadeWhilePlayingVideo = value; }
        }

        public bool FadeAudioIn
        {
            get { return mFadeAudioIn; }
            set { mFadeAudioIn = value; }
        }

        public float FadeAudioInLength
        {
            get { return mFadeAudioInLength;  }
            set { mFadeAudioInLength = value;  }
        }

        public bool FadeAudioOut
        {
            get { return mFadeAudioOut; }
            set { mFadeAudioOut = value; }
        }

        public float FadeAudioOutLength
        {
            get { return mFadeAudioOutLength; }
            set { mFadeAudioOutLength = value; }
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

        //*******************************************************************
        public double GetOriginalVideoDurationInSeconds()
        {
            if (mPlayer == null)
            {
                return 8;
            }
            return mPlayer.GetDurationInSeconds();
        }


        //*******************************************************************
        public double GetTrimmedVideoDurationInSeconds()
        {
            if (mPlayer == null)
            {
                return 8;
            }
            return mPlayer.GetDurationInSeconds() - (this.mStartVideoOffset + this.mEndVideoOffset);

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



		public CVideoDecoration ()
		{
		}


		public CVideoPlayer Player
		{
			get
			{
				return mPlayer ;
			}
            set
            {
                mPlayer = value;
           }
		}


		//*******************************************************************
		public CVideoDecoration (string filename, RectangleF coverage, int order) : base(coverage,order)
		{
			Init(filename);
		}

        //*******************************************************************
        // Used by the trim gui editior and global video database
        public CVideoDecoration(CVideoPlayer usePlayer)
        {
            mPlayer = usePlayer;
            mYFlipped = true;
        }

        //*******************************************************************
        // Used when creating templates
        public CVideoDecoration(string referenceVideoName)
        {
            mYFlipped = true;
            InitFromReferenceString(referenceVideoName);
        }


        //*******************************************************************
        public static CVideoDecoration FromVideoSlide(CVideoSlide slide)
        {
            CVideoDecoration vd = new CVideoDecoration(slide.SourceFilename, new RectangleF(0, 0, 1, 1), 0);

            vd.StartVideoOffset = slide.StartVideoOffset;
            vd.EndVideoOffset = slide.EndVideoOffset;
            vd.MusicFadeWhilePlayingVideo = slide.MusicFadeWhilePlayingVideo;
            vd.EnableMusicFadeWhilePlayingVideo = slide.EnableMusicFadeWhilePlayingVideo;
            return vd;
        }

		//*******************************************************************
		private void Init(string filename)
        {
			try
			{
				mPlayer = new CVideoPlayer(mStringID);
				mPlayer.Load(filename);

                CGlobals.MuteSound = true;

                // 
                // SRG 7/10/16
                // Not doing this can sometimes cause a (seek) later on to fail with tested mp4/mov files
                //
                mPlayer.Start(false);   

                mPlayer.Stop(this.mStartVideoOffset);

                CGlobals.MuteSound = false;
                mPlayer.SetVolume(0);

                // Video decorations might as well loop, if they can
                mPlayer.Loop = true;

                mYFlipped = true; // default this is needed with directx

			}
			catch (Exception e)
			{
                mPlayer = null;
                if (e is NoVideoCodecInstalledError)
                {
                    // if could not load because of codec issue, re throw,
                    throw e;
                }
			} 
				
		}

    
		//*******************************************************************
		public CVideoDecoration (CVideoDecoration copy) : base(copy)
		{
			if (copy.Player!=null)
			{
				Init(copy.Player.mFileName);
			}
			
		}

		//*******************************************************************
		public override CDecoration Clone()
		{
			return new CVideoDecoration (this);
		}

        //*******************************************************************
        public int GetStartVideoOffsetFrame()
        {
            double frame = this.mStartVideoOffset;
            frame = frame * CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

            return (int)frame;
        }

        //*******************************************************************
        public override Rectangle RenderToGraphics(Graphics gp, RectangleF r, int framenum, CSlide originating_slide, CImage original_image)
        {
            return new Rectangle(0, 0, 1, 1);
        }

        //*******************************************************************
        public override float GetOriginialImageAspectRatio()
        {
            if (mPlayer != null)
            {
                if (mOrientation == OrientationType.CW90 ||
                    mOrientation == OrientationType.CW270)
                {
                    return ((float)mPlayer.GetHeight()) / ((float)mPlayer.GetWidth());
                }

                return ((float)mPlayer.GetWidth()) / ((float)mPlayer.GetHeight());
            }
            return 1;
        }


        //*******************************************************************
        public override Rectangle RenderToGraphics(RectangleF r, int frame_num, CSlide originating_slide, RenderSurface inputSurface)
        {
            Rectangle drawn_region = new Rectangle(0, 0, 1, 1);

            if (mPlayer==null || InAnimatedTimeWindow(frame_num, originating_slide) == false)
            {
                return drawn_region;
            }

            // Video decors (if rendered normal) must ignore the alpha channel, as this can be anything.
            // If the video has an alpha map, this will be applied later
            if (mRenderMethod == RenderMethodType.NORMAL)
            {
                mRenderMethod = RenderMethodType.NORMAL_NO_IMAGE_ALPHA_BLEND;
            }

            Surface surface = null;

            if (mPlayer.CreateMipMaps == false && IsBackgroundDecoration() == false && IsFilter() == false && (MoveInEffect != null || MoveOutEffect !=null || originating_slide.UsePanZoom == true) )
            {
                //
                // Apply mip mapping to video textures if they are not a background decoration, a filter and contain any movement.
                //
                mPlayer.CreateMipMaps = true;
            }
            else
            {
                mPlayer.CreateMipMaps = false;
            }
                
            if (CGlobals.VideoSeeking == true)
            {
                float fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

                float frames_in = 0;

                //
                // Speed up. If what we are rendering too is very small (e.g. transition box)
                // simply render the -1 frame (e.g. preview frame)
                // This means it will probably use a cache video frame snapshot, rather than doing a slow seek.
                //
                if (r.Width <100 || r.Height<57)
                {
                    frame_num = -1;
                }

                if (frame_num >= 0)
                {
                    float frames_in_video_displayed = this.GetStartOffsetTime(originating_slide.DisplayLength) * fps;
                    frames_in = frame_num - (originating_slide.mStartFrameOffset + frames_in_video_displayed);
                }
                else
                {
                    frames_in = 30;
                }

                if (frames_in < 0) return drawn_region;

                frames_in += GetStartVideoOffsetFrame();

                float time_in = frames_in / fps;

                // ok may be beyond end point of player, so as looped, work out loop point
                float endTime = (float) (mPlayer.GetDurationInSeconds() - mEndVideoOffset);
                float playedFor = endTime - ((float)mStartVideoOffset);

                int count = 0;
                while (time_in > endTime && count < 100)
                { 
                    time_in -=playedFor;
                    count++;
                }

                surface = mPlayer.GetVideoSnapshot(time_in);
            }
            else
            {
                surface = mPlayer.GetNextImage(frame_num, (float)mStartVideoOffset, (float)mEndVideoOffset);
            }

            Surface rs = surface;

            if (rs!=null)
            {

                float sw = 1;
                float sh = 1;
                float sx = 0;
                float sy = 0;

                RectangleF coverageArea = mCoverageArea;

                // Do we need to adjust our coverage area because of a master image?
                // this only applies if we are currently not 16:9 (default)
                if (mOriginalTemplateImageNumber != 0 &&
                    originating_slide.MasterBackgroundImageAspect != 0 &&
                    CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio != CGlobals.OutputAspectRatio.TV16_9)
                {
                    coverageArea = RecalcCoverageAreaBasedOnMasterMaskCoverageArea(originating_slide.MasterBackgroundImageAspect, originating_slide.MasterBackgroundImageAspectType);
                }

                if (mUsePreviousDecorationCoverageArea == false)
                {
                    // If Keep aspect by clipping enabled, clip some of the source uv coords so we keep our original source image aspect ratio (needed for templates)
                    if (mDrawImageAspectType == CImage.DrawImageWithAspectType.KeepAspectByClipping)
                    {
                        // HAS to be aspect non rotated, the rotation is worked out in the call to CalcImageSourceClipAreaToMaintainAspectRatioWithCoverageArea
                        float aspect = ((float) Player.GetWidth()) / ((float) Player.GetHeight());
                        float imageWidth = 1.0f;
                        float imageHeight = 1.0f / aspect;

                        RectangleF srcRec = CalcImageSourceClipAreaToMaintainAspectRatioWithCoverageArea(coverageArea, imageWidth, imageHeight);
                        sx = srcRec.X;
                        sy = srcRec.Y / imageHeight;
                        sw = (srcRec.Width + sx);
                        sh = (srcRec.Height + sy) / imageHeight;
                    }
                    else if (mDrawImageAspectType == CImage.DrawImageWithAspectType.KeepAspectByChangingCoverageArea)
                    {
                        float imageWidth = 1.0f;
                        float imageHeight = 1.0f / GetOriginialImageAspectRatio();

                        coverageArea = CGlobals.CalcBestFitRectagleF(coverageArea, new RectangleF(0, 0, imageWidth, imageHeight / CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction()));
                    }
                    mLastCoverageArea = coverageArea;
                    mLastRotation = mRotation;
                }
                else
                {
                    coverageArea = GetLastCoverageAreaPlusGainPercent();
                    mRotation = mLastRotation;
                }

                drawn_region = RenderToGraphics(surface, coverageArea, sx, sy, sw, sh, r, frame_num, originating_slide, inputSurface);
            }

            return drawn_region;
		}

		//*******************************************************************
		public void StopVideoPlayer()
		{
            if (mPlayer == null)
            {
                return;
            }

            if (this.mRefernceStringID != "")
            {
                // 
                // Don't stop referenced videos, as parent video owner will do this
                //
                return;
            }

            mPlayer.Reset(this.mStartVideoOffset);	
		}


		//*******************************************************************
		public void ResetVideoPlayer()
		{
            if (mPlayer == null)
            {
                return;
            }

            if (this.mRefernceStringID != "")
            {
                // 
                // Don't reset referenced videos, as parent video owner will do this
                //
                return;
            }
            mPlayer.Reset(this.mStartVideoOffset);
		}


        //*******************************************************************
        override public void ResetAllMediaStreams()
        {
            ResetVideoPlayer();
        }

        //*******************************************************************
        override public void StopAllNonPlayingMediaStreams(CSlide currentSlide, CSlide nextSlide)
        {
            if (mPlayer == null)
            {
                return;
            }

            if (IsPlayerUsedOnSlides(this.Player, currentSlide, nextSlide) == false &&
                mPlayer.mStatus != CVideoPlayer.EStatus.STOPPED_AT_START)
            {
                this.StopVideoPlayer();
                CVideoPlayerCache.GetInstance().RemoveThisPlayerIfCacheFull(mPlayer);
            }
        }

        //*******************************************************************
        public static bool IsPlayerUsedOnSlides(CVideoPlayer player, CSlide currentSlide, CSlide nextSlide)
        {
            // ok make current slide and next slide share a player with this video decor

            List<CSlide> slides = new List<CSlide>();
            if (currentSlide != null)
            {
                slides.Add(currentSlide);
            }

            if (nextSlide != null)
            {
                slides.Add(nextSlide);
            }

            foreach (CSlide slide in slides)
            {
                CImageSlide imageSlide = slide as CImageSlide;
                if (imageSlide != null)
                {
                    CVideoSlide vs = slide as CVideoSlide;
                    if (vs != null)
                    {
                        if (vs.Player == player)
                        {
                            return true;
                        }
                    }

                    ArrayList decors = imageSlide.GetAllAndSubDecorations();

                    foreach (CDecoration decor in decors)
                    {
                        CVideoDecoration vd = decor as CVideoDecoration;
                        if (vd != null)
                        {
                            if (vd.Player == player)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }


		//*******************************************************************
		public void SeekToTime(double time, bool wait)
		{
            if (mPlayer == null)
            {
                return;
            }
			mPlayer.SeekToTime(time,wait);
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
		public bool HasDolbyAC3Sound()
		{
            if (mPlayer == null)
            {
                return false;
            }
			return this.mPlayer.HasDolbyAC3Sound();
		}


        //*******************************************************************
        public static CVideoDecoration FromClipArtDecoration(CClipArtDecoration cad, string videoFilename, double startOffset, double endOffset)
        {
            CVideoDecoration vd = new CVideoDecoration();
            try
            {
                XmlDocument saveDoc = new XmlDocument();
                XmlElement docElement = saveDoc.CreateElement("doc");
                saveDoc.AppendChild(docElement);
                cad.Save(docElement, saveDoc);

                XmlNodeList list = docElement.GetElementsByTagName("Decoration");

                if (list.Count != 0)
                {
                    XmlElement element = list[0] as XmlElement;

                    vd.LoadImageDecorationPart(element);
                }
            }
            catch
            {
            }

            vd.FlipY();
            vd.StartVideoOffset = startOffset;
            vd.EndVideoOffset = endOffset;

            if (videoFilename.StartsWith(".") == true)
            {
                vd.InitFromReferenceString(videoFilename);
            }
            else
            {
                vd.Init(videoFilename);
            }

            return vd;

        }

        //*******************************************************************
        public override void DeclareSlideAspectChange(CGlobals.OutputAspectRatio oldAspect, CGlobals.OutputAspectRatio newAspect)
        {
            if (IsBorderDecoration() == false &&
                IsBackgroundDecoration() == false &&
                IsFilter() == false &&
                mDrawImageAspectType == CImage.DrawImageWithAspectType.Stretch)
            {
                AdjustCoverageAreaForNewAspect(oldAspect, newAspect);
            }
        }


        //*******************************************************************
        public bool MatchesSlideLength(float slideLength)
        {
            double videoLength = GetTrimmedVideoDurationInSeconds();
            return MatchesSlideLength(slideLength, (float)videoLength);
        }

        //*******************************************************************
        public bool MatchesSlideLength(float slideLength, float videoLength)
        {
            //
            // Set the tolerance to 0.3 seconds, this is because a video imported into PhotoVidShow 
            // with old haali/MS-DTV codecs will not report the same video length as say lav.
            // So if you import your (old) project where videos were loading from different
            // codecs, your slide length may not be linked to your video length unless
            // we have this tolerance.
            //
            if (Math.Abs(slideLength - videoLength) < 0.3f)
            {
                if (StartOffsetTimeRawValue == CAnimatedDecoration.SlideStart &&
                    EndOffsetTimeRawValue == CAnimatedDecoration.SlideEnd)
                {
                    return true;                
                }
            }

            return false;
        }

        //*******************************************************************
        public void SetPreviewPlaybackVolume(int volume)
        {
            if (mPlayer != null)
            {
                mPlayer.SetVolume(volume);
            }
        }

        //*******************************************************************
        public int GetVideoWidth()
        {
            if (mPlayer == null)
            {
                return 100;
            }
            return mPlayer.GetWidth();
        }

        //*******************************************************************
        public int GetVideoHeight()
        {
            if (mPlayer == null)
            {
                return 100;
            }
            return mPlayer.GetHeight();
        }

        //*******************************************************************
        public string GetFilename()
        {
            if (mPlayer==null)
            {
                return "";
            }
            return mPlayer.mFileName;
        } 

        //*******************************************************************
        public override bool VerfifyAllMediaFilesToRenderThisExist()
        {
            if (mPlayer == null)
            {
                return false;
            }
            return true;
        }

		//*******************************************************************
		public override void Save(XmlElement parent, XmlDocument doc)
		{
            if (mPlayer == null) return;

			XmlElement decoration = doc.CreateElement("Decoration");

			decoration.SetAttribute("Type","VideoDecoration");
            if (mRefernceStringID != "")
            {
                decoration.SetAttribute("VideoName", mRefernceStringID);
            }
            else
            {
                string filename = CGlobals.StripRootHeader(this.mPlayer.mFileName);

                decoration.SetAttribute("VideoName", filename);
            }

            if (mStartVideoOffset != mDefaultVideoDecoration.StartVideoOffset)
            {
                decoration.SetAttribute("StartVideoOffset", this.mStartVideoOffset.ToString());
            }

            if (mVolume != mDefaultVideoDecoration.Volume)
            {
                decoration.SetAttribute("Volume", this.mVolume.ToString());
            }

            if (mEndVideoOffset != mDefaultVideoDecoration.EndVideoOffset)
            {
                decoration.SetAttribute("EndVideoOffset", this.mEndVideoOffset.ToString());
            }

            if (mMusicFadeWhilePlayingVideo != mDefaultVideoDecoration.MusicFadeWhilePlayingVideo)
            {
                decoration.SetAttribute("MusicFade", this.mMusicFadeWhilePlayingVideo.ToString());
            }

            if (mEnableMusicFadeWhilePlayingVideo != mDefaultVideoDecoration.EnableMusicFadeWhilePlayingVideo)
            {
                decoration.SetAttribute("EnableMusicFade", this.mEnableMusicFadeWhilePlayingVideo.ToString());
            }

            decoration.SetAttribute("FadeAudioIn", mFadeAudioIn.ToString());
            decoration.SetAttribute("FadeAudioInLength", mFadeAudioInLength.ToString());
  
            decoration.SetAttribute("FadeAudioOut", mFadeAudioOut.ToString());
            decoration.SetAttribute("FadeAudioOutLength", mFadeAudioOutLength.ToString());
            

            if (mStringID != "")
            {
                decoration.SetAttribute("StringID", this.mStringID);
            }

            if (mGlobalPlayer == true)
            {
                decoration.SetAttribute("GlobalPlayer", mGlobalPlayer.ToString());
            }

            SaveImageDecorationPart(decoration, doc);

			parent.AppendChild(decoration); 
		}

		//*******************************************************************
		public override void Load(XmlElement element)
		{
            LoadImageDecorationPart(element);

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
                mVolume = double.Parse(ss, CultureInfo.InvariantCulture);
            }

            ss = element.GetAttribute("MusicFade");
            if (ss != "")
            {
                this.mMusicFadeWhilePlayingVideo = float.Parse(ss);
            }

            ss = element.GetAttribute("EnableMusicFade");
            if (ss != "")
            {
                this.mEnableMusicFadeWhilePlayingVideo = bool.Parse(ss);
            }

            ss = element.GetAttribute("FadeAudioIn");
            if (ss != "")
            {
                mFadeAudioIn = bool.Parse(ss);
            }

            ss = element.GetAttribute("FadeAudioInLength");
            if (ss != "")
            {
                FadeAudioInLength = float.Parse(ss);
            }

            ss = element.GetAttribute("FadeAudioOut");
            if (ss != "")
            {
                mFadeAudioOut = bool.Parse(ss);
            }

            ss = element.GetAttribute("FadeAudioOutLength");
            if (ss != "")
            {
                FadeAudioOutLength = float.Parse(ss);
            }

            ss = element.GetAttribute("GlobalPlayer");
            if (ss != "")
            {
                mGlobalPlayer = bool.Parse(ss);
            }

            if (mGlobalPlayer == true)
            {
                CGlobalVideoDatabase.GetInstance().SetupFromVideoDecoration(this, filename);
                mYFlipped = true;
            }
            else
            {

                if (filename != null && filename.Length > 0 && filename[0] != '.')
                {
                    mStringID = element.GetAttribute("StringID");
                    Init(filename);
                }
                else
                {
                    InitFromReferenceString(filename);
                }
            }
		}

        //*******************************************************************
        private void InitFromReferenceString(string name)
        {
            // Use another player 
            CVideoPlayer p = CVideoPlayerDatabase.GetInstance().GetPlayer(name);
            if (p != null)
            {
                this.mPlayer = p;
                mRefernceStringID = name;
            }
            else
            {
                SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromDecoration(this);
                Log.Error("VideoDecoration could not find referenced player :" + name + " in slideshow '" + message.Slideshow + "' slide:" + message.SlideNumber);
            }
            mYFlipped = true;
        }

        //*******************************************************************
        public double GetFrameRate()
        {
            if (mPlayer==null)
            {
                return 29.97;
            }
            return mPlayer.GetFrameRate();
        }
	}
}
