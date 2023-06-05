/*
 * Created by SharpDevelop.
 * User: user
 * Date: 03/01/2005
 * Time: 12:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Globalization;
using ManagedCore ;
using System.Collections.Generic;

//using Microsoft.DirectX.AudioVideoPlayback;
using MangedToUnManagedWrapper;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
	/// <summary>
	/// Description of Class1.	
	/// </summary>
	/// 

	public delegate void MusicSyncInvalidCallback();

	public class CSlideShow : CVideo
	{
        public enum ChapterMarkersType
        {
            None,
            RegularTimedInterval,
            SetFromSlides
        };

		private string 				mName;

		public string Name
		{
			get
			{
				return mName;
			}
            set
            {
                mName = value;
            }
		}

		public CTextDecoration 		mScrollText;
		public CSlide 				mDefaultSlide;
		public ArrayList			mSlides;
		private bool				mFadeIn;
		private Color				mFadeInColor;
		private bool				mFadeOut;
		private Color				mFadeOutColor;
		public ArrayList			mMusicSlides;
		private bool				mFadeOutAudio ;
		private bool				mFadeInAudio;
		private bool				mLoopMusic = true;	// not used if sync length is set to true;
		private bool				mSyncLengthToMusic=false;

		private	int					mId;
		private static int			mIDCount=0;

		private	float				mVideoAudioVol = 1.0f;
		private float				mBackGroundMusicVol = 1.0f;
        private float               mNarrationVol = 1.0f;

		private CSlide				mMenuThumbnailSlide = null;
        private CMusicPerformance   mPreviewMusicPerformace = null;

        private List<CNarrationAudioSlide> mNarrationSlides = new List<CNarrationAudioSlide>();

        private ChapterMarkersType mChaperMarkers = ChapterMarkersType.RegularTimedInterval;
        private DateTime mChapterMarkersTimeInterval =  new DateTime(2014,1,1,0,5,0); // 5 minutes

		// if true this means the slideshow contains a single videoslide
		// which is a mpeg2 video of the correct format (i.e. a pre encoded
		// slideshow ready for the dvd author stage)
		// (this is only used in PhotoCruz at the moment
		private bool				mPreEncodedSlideshow=false;

        public List<CNarrationAudioSlide> NarrationSlides
        {
            get 
            {
                // ok remove any narrations that start after end of slideshow

                List<CNarrationAudioSlide> toRemove = null;
                foreach (CNarrationAudioSlide narration in mNarrationSlides)
                {
                    if (narration.StartNarrationTime > this.GetLengthInSeconds())
                    {
                        if (toRemove == null)
                        {
                            toRemove = new List<CNarrationAudioSlide>();
                        }

                        toRemove.Add(narration);
                    }
                }

                if (toRemove != null)
                {
                    foreach (CNarrationAudioSlide narration in toRemove)
                    {
                        mNarrationSlides.Remove(narration);
                    }
                }

                return mNarrationSlides;
            }
        }

		public bool PreEncodedSlideshow
		{
			get
			{
				return mPreEncodedSlideshow;
			}
			set
			{
				mPreEncodedSlideshow = value;
			}
		}

		public CSlide MenuThumbnailSlide
		{
			get
			{
				return mMenuThumbnailSlide;
			}
			set
			{
				mMenuThumbnailSlide = value;
				CGlobals.mCurrentProject.DeclareChange("Set Menu Thumbnail");
			}
		}

		public bool FadeOutAudio
		{
			get
			{
				return mFadeOutAudio;
			}
			set
			{
				mFadeOutAudio = value;
			}
		}

		public float VideoAudioVolume
		{
			get
			{
				return mVideoAudioVol;
			}
			set
			{
				mVideoAudioVol=value;
			}
		}


		public float BackgroundMusicVolume
		{
			get
			{
				return mBackGroundMusicVol;
			}
			set
			{
				mBackGroundMusicVol=value;
			}
		}


        public float NarrationVolume
        {
            get
            {
                return mNarrationVol;
            }
            set
            {
                mNarrationVol = value;
            }
        }

		public bool FadeInAudio
		{
			get
			{
				return mFadeInAudio;
			}
			set
			{
				mFadeInAudio = value;
			}
		}

		public bool	 LoopMusic
		{
			get
			{
				return mLoopMusic;
			}
			set
			{
				if (mLoopMusic==value) return;
				mLoopMusic = value;
				this.ReCalcLoopMusicSlides();
			}
		}

		public bool	 FadeIn
		{
			get
			{
				return mFadeIn;
			}
			set
			{
				mFadeIn = value;
			}
		}

		public bool FadeOut
		{
			get
			{
				return mFadeOut;
			}
			set
			{
				mFadeOut = value;
			}
		}

		public Color FadeInColor
		{
			get
			{
				return mFadeInColor;
			}
			set
			{
				mFadeInColor = value;
			}
		}

		public Color FadeOutColor
		{
			get
			{
				return mFadeOutColor;
			}
			set
			{
				mFadeOutColor = value;
			}
		}
				

		
		public event MusicSyncInvalidCallback InvalidAudioSync;

		public int ID
		{
			get
			{
				return mId;
			}
		}



		public bool SyncLengthToMusic
		{
			get
			{
				return mSyncLengthToMusic;
			}
			set
			{
				SetSyncToSlideshowWithoutMemento(value);
				CGlobals.mCurrentProject.DeclareChange("Slide show Sync to music changed");
			}


		}

		CSlide DefaultSlide
		{
			get
			{
				return mDefaultSlide;
			}
		}

        public ChapterMarkersType ChapterMarkersTypeToUse
        {
            get { return mChaperMarkers; }
            set { mChaperMarkers = value ; }
        }

        public DateTime ChapterMarkerTimeInterval 
        {
            get { return mChapterMarkersTimeInterval; }
            set { mChapterMarkersTimeInterval = value ; }
        }

        public List<DateTime> ChapterMarkers
        {
            get
            {
                if (mChaperMarkers == ChapterMarkersType.None)
                {
                    return new List<DateTime>();
                }

                if (mChaperMarkers == ChapterMarkersType.RegularTimedInterval)
                {
                    return GenerateTimeIntervalChapterMarkers();
                }

                return GenerateChapterMarkersFromSlides();
            }
        }

		// calc
		private int	mLastFrame =-1;
        private int mLastFrameBasedOnEncodeRatio = -1;
	
		//*******************************************************************
		public CSlideShow(string name)
		{
			mName=name;
			mFadeIn=true;
			mFadeOut=true;
			mFadeOutAudio = true;
			mFadeInAudio = false;

			mFadeInColor = Color.Black;
			mFadeOutColor = Color.Black;

			mDefaultSlide = new CSlide() ;
            mDefaultSlide.TransistionEffect = new SimpleAlphaBlendTransitionEffect(1.5f);
            mDefaultSlide.TransistionEffect.Index = 1;

			mSlides = new ArrayList();
			mMusicSlides = new ArrayList();

			mId = mIDCount++;
 
			//Video video = Video.FromFile(name, true);

			//video.Owner = panel1;
		}


		public void SetSyncToSlideshowWithoutMemento(bool val)
		{
			if (mSyncLengthToMusic == val) return ;
			mSyncLengthToMusic = val;
			this.InValidateLength();
		}

		

		//*******************************************************************
		public void InValidateLength()
		{
			mLastFrame =-1;
            mLastFrameBasedOnEncodeRatio = -1;
            mPreviewMusicPerformace = null;
		}

        //*******************************************************************
        public void AddSlides(ArrayList slides)
        {
            AddSlides(slides, true, null, null, true);
        }

        //*******************************************************************
        public void AddSlides(ArrayList slides, bool setSlideTimeLengthAndTransistion)
        {
            AddSlides(slides, setSlideTimeLengthAndTransistion, null, null, true);
        }

        //*******************************************************************
        // before and after can't be both set
        public void AddSlides(ArrayList slides, bool setSlideTimeLengthAndTransistion, CSlide afterSlide, CSlide beforeSlide)
        {
            AddSlides(slides, setSlideTimeLengthAndTransistion, afterSlide, beforeSlide, true);
        }

		//*******************************************************************
        public void AddSlides(ArrayList slides, bool setSlideTimeLengthAndTransistion, CSlide afterSlide, CSlide beforeSlide, bool declareProjectUpdate)
		{
            bool added = false;
            if (afterSlide != null)
            {
                int index = mSlides.IndexOf(afterSlide);
                if (index != -1)
                {
                    mSlides.InsertRange(index+1, slides);
                    added = true;
                }
            }
            else if (beforeSlide != null)
            {
                int index = mSlides.IndexOf(beforeSlide);
                if (index >=0)
                {
                    mSlides.InsertRange(index, slides);
                    added = true;
                }
            }

            if (added == false)
            {
                mSlides.AddRange(slides);
                added = true;
            }

			// copy default values to new slides
			foreach (CSlide s in slides)
			{
                if (setSlideTimeLengthAndTransistion==true)
                {
                    s.TransistionEffect = mDefaultSlide.TransistionEffect.Clone();
                }

                // Legacy
                if (s is CBlankStillPictureSlide == false &&
                    s is CVideoSlide == false)
                {
                    s.UsePanZoom = mDefaultSlide.UsePanZoom;
                }

                bool ignoreSetLength = false;

                // Also do not set slide time if a video based slide (it has already been set)
                CBlankStillPictureSlide bps = s as CBlankStillPictureSlide;
                if (bps != null)
                {
                    if (bps.Decorations.Count > 0)
                    {
                        if (bps.Decorations[0] is CVideoDecoration)
                        {
                            ignoreSetLength = true;
                        }
                        else if (setSlideTimeLengthAndTransistion == true)
                        {
                            s.UsePanZoom = mDefaultSlide.UsePanZoom;
                        }
                    }
                }

                if (s is CStillPictureSlide == true && ignoreSetLength == false && setSlideTimeLengthAndTransistion == true)
				{
					s.SetLengthWithoutUpdate(this.mDefaultSlide.DisplayLength);
				}
			}
			InValidateLength();
			ReCalcLoopMusicSlides();

            if (declareProjectUpdate)
            {
                CGlobals.mCurrentProject.DeclareChange("Added slides to slideshow");
            }
		}

		
		//*******************************************************************
		public void AddSlides(string[] filenames, CSlide afterSlide, CSlide beforeSlide, ManagedCore.Progress.IProgressCallback ic)		
		{
			int total_slides = CGlobals.mCurrentProject.GetTotalNumberOfSlides();

			ArrayList slides = new ArrayList();

			if (filenames.Length <=0) return ;

			if (ic!=null) 
			{
				ic.Begin(0, filenames.Length);
			}
			int the_count =0;
			foreach (string file in filenames)
			{
                if (ic.Cancelled==true)
                {
                    return;
                }

				if (ic !=null)
				{
					ic.StepTo(the_count);
				}
				try
				{
					if (ic!=null)
					{ 
						ic.SetText("Processing File '"+System.IO.Path.GetFileName(file)+"'");
					}

					string lower = file.ToLower();
                    if (CGlobals.IsImageFilename(lower) == true)
                    {
                        CClipArtDecoration clipartDecoration = new CClipArtDecoration(file, new RectangleF(0,0,1,1), 0);
                        clipartDecoration.SetToSmoothEdgeAlphamap();
                        slides.Add(new CBlankStillPictureSlide(clipartDecoration));
                    }
					else
					{
                        // unknown file format, try loading as video format
                        CVideoDecoration videoDecoration = new CVideoDecoration(file, new RectangleF(0, 0, 1, 1), 0);
						bool dont_add = false;
             
                        // normally no pan and zoom, so by default do not set smooth edge alha map

						if (dont_add==false && videoDecoration.Player != null)
						{
                            slides.Add(new CBlankStillPictureSlide(videoDecoration));
						}
					}
				}
				catch(Exception theException)
				{
                    if (theException is NoVideoCodecInstalledError)
                    {
                        NoVideoCodecInstalledError nvcexcpetion = theException as NoVideoCodecInstalledError;
                        if (nvcexcpetion.mDetectedFFDShow == true)
                        {

                            UserMessage.Show("Failed to load video '" + System.IO.Path.GetFileName(file) + "\n\r\n\rFFDShow codecs were detected but may be configured not to allow PhotoVidShow to use them.\n\r\n\r" +
                                                                    "Switch off 'Use ffdshow only in' option in the ffdshow video decoder configuration.\n\r\n\r"+
                                                                    "This can be found from Start->All Programs->FFDShow->Video decoder configuration (DirectShow control)", "Video load failed",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
  
                        }
                        else
                        {

                            UserMessage.Show("Failed to load video '" + System.IO.Path.GetFileName(file) + "'\n\r\n\rYou may need to install some codecs for this video file to work correctly.\n\r\n\rSee http://www.photovidshow.com/extras.html for details.", "Video load failed",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                    else
                    {
                        string message = "An error occurred when opening file '" + System.IO.Path.GetFileName(file) + "'. Ignoring file.";
                        Log.Error(message);
                        UserMessage.Show(message, "File load failure",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
				}

				the_count++;
			}

            if (ic.Cancelled == false)
            {
                AddSlides(slides, true, afterSlide, beforeSlide, true);
            }
		}

        //*******************************************************************
        public void RemoveSlides(ArrayList slides)
        {
            RemoveSlides(slides, true);
        }
 
		//*******************************************************************
		public void RemoveSlides(ArrayList slides, bool declareProjectChange)
		{	
			foreach (CSlide s in slides)
			{
				CVideoSlide vs = s as CVideoSlide;
				if (vs!=null)
				{
					vs.Stop();
				}
				this.mSlides.Remove(s);

				// if slide was the menu thumbnail, set it back to null
				if (mMenuThumbnailSlide!= null &&
					mMenuThumbnailSlide.ID == s.ID)
				{
					mMenuThumbnailSlide=null;
				}
			}
			InValidateLength();
			ReCalcLoopMusicSlides();

            if (declareProjectChange == true)
            {
                CGlobals.mCurrentProject.DeclareChange("Removed slides from slideshow");
            }
		}


		//*******************************************************************
		public void RemoveMusicSlides(ArrayList tracks)
		{
			foreach (CMusicSlide m in tracks)
			{
                m.Dispose();
				this.mMusicSlides.Remove(m);
			}

			InValidateLength();

			this.ReCalcLoopMusicSlides();
			CGlobals.mCurrentProject.DeclareChange("Removed music slides");
		}

        //*******************************************************************
        public void RemoveNarrationSlides(ArrayList tracks)
        {
            bool removed = false;
            foreach (CMusicSlide m in tracks)
            {
                CNarrationAudioSlide nas = m as CNarrationAudioSlide;
                if (nas != null)
                {
                    this.mNarrationSlides.Remove(nas);
                    removed = true;
                }

            }

            if (removed == true)
            {
                InValidateLength();
                CGlobals.mCurrentProject.DeclareChange("Removed narration slides");
            }
        }


		//*******************************************************************
		public void MoveSlides(ArrayList slides, int num)
		{
			// check if moving slides to end

			int index =0;
			if (num== this.mSlides.Count)
			{
				foreach (CSlide slide in slides)
				{
					mSlides.Remove(slide);
				}
				index = mSlides.Count;
			}
				// move slides to start or middle of slideshow
			else
			{
				CSlide before_slide = this.mSlides[num] as CSlide;
				if (slides.Contains(before_slide)==true)
				{
					ManagedCore.CDebugLog.GetInstance().Error(
						"Can not move slides after a given slide as the given slide was one to be moved");
					return;
				}
				
				foreach (CSlide slide in slides)
				{
					mSlides.Remove(slide);
				}

				index = mSlides.IndexOf(before_slide);
			}

			foreach (CSlide slide in slides)
			{
				mSlides.Insert(index, slide);
				index++;
			}

			InValidateLength();
			CGlobals.mCurrentProject.DeclareChange("Moved Slides");
		}

	
		//*******************************************************************
		//	void ResetNonPlayingVideoSlides(CSlide current_slide, CSlide next_slide)
		//	{
		//		foreach (CSlide slide in mSlides)
		///		{
		//			if (slide is CVideoSlide &&
		//				slide != current_slide &&
		//				slide != next_slide)
		//			{
		//				((CVideoSlide)slide).ResetVideoPlayer();
		//			}
		//		}
		//	}



		//*******************************************************************
		public void ResetAllMediaStreams()
		{
            mPreviewMusicPerformace = null;

			foreach (CSlide slide in mSlides)
			{
                slide.ResetAllMediaStreams();
			}

			foreach (CMusicSlide ms in this.mMusicSlides)
			{
				ms.Reset();
			}

            foreach (CNarrationAudioSlide narration in this.mNarrationSlides)
            {
                narration.Reset();
            }
		}    

		//*******************************************************************
		void StopNonPlayingVideoSlides(int current_frame,
			CSlide current_slide, 
			CSlide next_slide,
			CMusicSlide current_music_slide,
            CNarrationAudioSlide current_narration_slide)
		{
			foreach (CSlide slide in mSlides)
			{
				
				if(slide != current_slide &&
					slide != next_slide)
				{
                    slide.StopAllNonPlayingMediaStreams(current_slide, next_slide);
				}

				else if (slide==next_slide)
				{
					if (slide.mStartFrameOffset > current_frame)
					{
                        slide.StopAllNonPlayingMediaStreams(current_slide, null);
					}
				}
			}

			foreach (CMusicSlide ms in this.mMusicSlides)
			{
				if (ms != current_music_slide)
				{
					ms.Stop();
				}
			}

            foreach (CNarrationAudioSlide narration in this.mNarrationSlides)
            {
                if (narration != current_narration_slide)
                {
                    narration.Stop();
                }
            }
		}

		//*******************************************************************
        protected override void GetVideo(int frame, bool ignore_pan_zoom, bool ignore_decorations, int req_width, int req_height, RenderSurface surface)
		{
            // SRG TO REMOVE IN END
            RenderVideoParameters parameters = new RenderVideoParameters();
            parameters.frame = frame;
            parameters.ignore_decorations = ignore_decorations;
            parameters.ignore_pan_zoom = ignore_pan_zoom;
            parameters.req_width = req_width;
            parameters.req_height = req_height;
            parameters.render_to_surface = surface;

            RenderVideo(parameters);
        }

        //*******************************************************************
        private float GetFadeInTime()
        {
            float fadeInTime = CGlobals.FadeInTime;
            //
            // Ok ensure first slide length twice the size of fade in time
            //
            if (mSlides.Count > 0)
            {
                CSlide firstSlide = mSlides[0] as CSlide;
                if (firstSlide.DisplayLength / 2 < fadeInTime)
                {
                    fadeInTime = firstSlide.DisplayLength / 2;
                }
            }
            return fadeInTime;
        }

        //*******************************************************************
        private float GetFadeOutTime()
        {
            float fadeOutTime = CGlobals.FadeOutTime;
            //
            // Ok ensure last slide length twice the size of fade in time
            //
            if (mSlides.Count > 0)
            {
                CSlide lastSlide = mSlides[mSlides.Count-1] as CSlide;
                if (lastSlide.DisplayLength / 2 < fadeOutTime)
                {
                    fadeOutTime = lastSlide.DisplayLength / 2;
                }
            }
            return fadeOutTime;
        }

        //*******************************************************************
        protected override void GetVideo(GraphicsEngine engine, RenderVideoParameters parameters)
        {
			CGlobals.DeclareEncodeCheckpoint('C');

			CalcLengthOfAllSlides();    // ### SRG TODO WHY IS IT DOING THIS EVERY FRAME!!!

            int frame = parameters.frame;
            CSlide current_slide = GetCurrentSlide(frame);
            CSlide next_slide = GetNextSlide(frame);

            engine.ClearRenderTarget(0, 0, 0, 255);

			CMusicSlide current_music_slide = null;
            CNarrationAudioSlide current_narration_slide = null;
			
			if (CGlobals.MuteSound==false)
			{
                float videoMasterVolume = 1;
                float musicMasterVolume = 1;
                float narrationMasterVolume = 1;

                if (mPreviewMusicPerformace == null)
                {
                    mPreviewMusicPerformace = CMusicPerformance.FromVideo(this);
                }

                double time = ((double)frame) / CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

                if (mPreviewMusicPerformace != null)
                {
                    mPreviewMusicPerformace.CalculatePreviewAudioLevels( time , ref musicMasterVolume, ref videoMasterVolume, ref narrationMasterVolume, this.mBackGroundMusicVol, this.mVideoAudioVol, this.mNarrationVol);
                }

                current_music_slide = GetCurrentMusicSlide(frame);
				
				if (current_music_slide!=null)
				{
                    current_music_slide.UpdateVolume(musicMasterVolume, frame);
					current_music_slide.Play(frame);
					
				}

                if (SetPreviewVolumeForVideoDecorations(current_slide, videoMasterVolume, time) == false)
				{
					CVideoSlide vs = current_slide as CVideoSlide;
					if (vs!=null) vs.SetPreviewPlaybackVolume(videoMasterVolume);
				}
                if (SetPreviewVolumeForVideoDecorations(next_slide, videoMasterVolume, time) == false)
				{
					CVideoSlide vs = next_slide as CVideoSlide;
					if (vs!=null) vs.SetPreviewPlaybackVolume(videoMasterVolume);
				}

                current_narration_slide = GetCurrentNarrationSlide(frame);
                if (current_narration_slide != null && current_narration_slide.PreviewAudioUnit !=null)
                {
                    double narrationVolume = current_narration_slide.PreviewAudioUnit.GetPreviewVolumeForTime(time);
                    narrationVolume *= narrationMasterVolume;
                    current_narration_slide.ApplyWithVolume(narrationVolume);
                    current_narration_slide.Play(frame);               
                }			
			}

			if (current_slide==null)
			{
                return;
			}

			if (CGlobals.VideoSeeking==false)
			{
                StopNonPlayingVideoSlides(frame, current_slide, next_slide, current_music_slide, current_narration_slide);
			}

            float delta_with_next_slide = CalcDeltaBetweenCurrentSlideAndNextSlide(current_slide, next_slide, frame);

            // Render post transition decorations (normally filter textures)
            // This is only done for one slide (if mixing two together); so use which ever we are nearer 
            CSlide process_post_transition_slide = current_slide;
            if (delta_with_next_slide > 0.5 && next_slide != null)
            {
                process_post_transition_slide = next_slide;
            }

            RenderSurface finalSurface = null;
            DisposableObject<RenderSurface> postTransitionDecorationSurface = new DisposableObject<RenderSurface>();
            using ( postTransitionDecorationSurface )
            {
                CDecoration postTransistionDecoration = process_post_transition_slide.GetPostTransisionDecoration(frame);

                if (postTransistionDecoration != null && postTransistionDecoration.IsLayer() == true)
                {
                    finalSurface = engine.GetRenderTarget();
                    postTransitionDecorationSurface.Assign(engine.GenerateRenderSurface((uint)parameters.req_width, (uint)parameters.req_height, this.ToString() +"::GetVideo"));
                    engine.SetRenderTarget(postTransitionDecorationSurface.Object);
                    engine.ClearRenderTarget(0, 0, 0, 255);
                }


                float fadeInTime = GetFadeInTime();
                float fadeOutTime = GetFadeOutTime();
                //
		        // Fade in 
                //
		        if (mFadeIn == true &&
                    current_slide == mSlides[0] &&  // ensure we're on the first frame too
                    frame <= (CGlobals.mCurrentProject.DiskPreferences.frames_per_second * fadeInTime))
		        {
			        CSlide s2 = CGlobals.BlankSlide;
			        s2.SetBackgroundColor(mFadeInColor);
                    SimpleAlphaBlendTransitionEffect effect = new SimpleAlphaBlendTransitionEffect(fadeInTime);

                    float delta = ((float)frame) / (fadeInTime * CGlobals.mCurrentProject.DiskPreferences.frames_per_second);
                    effect.Process(delta, s2, current_slide, frame, parameters.req_width, parameters.req_height, parameters.ignore_pan_zoom);
		        }

                //
		        // Fade out
                //
                else if (mFadeOut == true &&
                         current_slide == mSlides[mSlides.Count-1] &&  // ensure we're on the last slide too
                         frame >= (mLastFrame - (CGlobals.mCurrentProject.DiskPreferences.frames_per_second * fadeOutTime)))
                {
                    CSlide s2 = CGlobals.BlankSlide;
                    s2.SetBackgroundColor(mFadeOutColor);
                    SimpleAlphaBlendTransitionEffect effect = new SimpleAlphaBlendTransitionEffect(fadeOutTime);

                    float f1 = (float)(mLastFrame - frame);
                    float ft = (float)fadeOutTime * CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

                    float delta = f1 / ft;
                    if (delta > 1) delta = 1;
                    if (delta < 0) delta = 0;

                    effect.Process(delta, s2, current_slide, frame, parameters.req_width, parameters.req_height, parameters.ignore_pan_zoom );
                }
                else
                {
                    // ok do normal mix between slides
                    MixSlides(current_slide, next_slide, frame, parameters.req_width, parameters.req_height, parameters.ignore_pan_zoom, null, delta_with_next_slide);

                    CGlobals.DeclareEncodeCheckpoint('D');
                }

                if (postTransistionDecoration!=null)
                {
                    // Process post transition decorations, if it has any
                    process_post_transition_slide.RenderUnAttachedDecorations(new Rectangle(0, 0, parameters.req_width, parameters.req_height), frame, true, finalSurface);
                    postTransitionDecorationSurface.Release();
                }
            }
            return ;
		}

        //*******************************************************************
        // sets the preview volume for all video decorations
        // returns true, if there is playing video decoration with volume;
        private bool SetPreviewVolumeForVideoDecorations(CSlide forSlide, float master_video_volume, double time)
        {
            CImageSlide ims = forSlide as CImageSlide;
            if (ims == null) return false;

            bool found_vd = false;

            CVideoDecoration playing_vd = CMusicPerformance.GetAudioVideoDecorationForSlide(forSlide);
            if (playing_vd != null)
            {
                found_vd = true;
                if (playing_vd.PreviewAudioUnit == null)
                {
                    //
                    // No audio unit means no volume
                    //
                    playing_vd.SetPreviewPlaybackVolume(CGlobals.vol_ratio_to_dx9_dbloss(0));
                }
                else
                {
                    double videoDecorationVolume = playing_vd.PreviewAudioUnit.GetPreviewVolumeForTime(time);

                    double the_vol = videoDecorationVolume * master_video_volume;
                    int db_loss = CGlobals.vol_ratio_to_dx9_dbloss((float)the_vol);

                    playing_vd.SetPreviewPlaybackVolume(db_loss);
                }
            }

            foreach (CDecoration dec in ims.GetAllAndSubDecorations())
            {
                CVideoDecoration vd = dec as CVideoDecoration;
                if (vd != null && vd != playing_vd)
                {
                    vd.Volume = 0;
                    vd.SetPreviewPlaybackVolume(CGlobals.vol_ratio_to_dx9_dbloss(0));
                }
            }

            return found_vd;

        }

        //*******************************************************************
        public static float CalcDeltaBetweenCurrentSlideAndNextSlide(CSlide current_slide, CSlide next_slide, int frame)
        {
            float transition_delta = 0;

            CTransitionEffect effect = null;

            if (current_slide != null)
            {
                effect = current_slide.TransistionEffect;
            }

            if (next_slide == null || effect == null)
            {
                return transition_delta;
            }

            
            // assume transition ever when frame length over;

            int end_f = current_slide.CalcEndFrame();

            int frames_to_go = end_f - frame;

            if (frames_to_go < 0)
            {
                Console.WriteLine("ERROR: processing slide after it should have finsihed");
                frames_to_go = 0;
            }

            int effect_length = (int)(effect.Length * CGlobals.mCurrentProject.DiskPreferences.frames_per_second);

            if (frames_to_go >= effect_length)
            {
                transition_delta = 0.0f;
            }
            else if (frames_to_go <= 0)
            {
                transition_delta = 1.0f;
            }
            else
            {
                transition_delta = 1.0f - (((float)frames_to_go) / ((float)effect_length));
            }
            return transition_delta;

        }
 
		//*******************************************************************
        void MixSlides(CSlide current_slide, CSlide next_slide, int frame, int req_width, int req_height, bool ignore_pan_zoom, CImage render_to_this, float delta_with_next_slide)
		{
			if (next_slide==null)
			{
                current_slide.RenderFrame(frame, ignore_pan_zoom, false, req_width, req_height);
                return;
			}

			CGlobals.DeclareEncodeCheckpoint('E');

            CTransitionEffect effect = current_slide.TransistionEffect;

			// munge slides together
            effect.Process(delta_with_next_slide, current_slide, next_slide, frame, req_width, req_height, ignore_pan_zoom);

			CGlobals.DeclareEncodeCheckpoint('F');
		}
		
		//*******************************************************************
		public bool IsValidToSyncroniseToMusic()
		{
			double time = GetLengthOfAllVideoSlidesInSeconds();
			time += GetNumVariableTimeSlides()* 0.2f;
			double music_time = GetLengthOfBackgroundMusic(false);

			if (music_time < time)
			{
				return false;
			}
			return true;
		}


		//*******************************************************************
		private void NittyGrittySetSlideLengths()
		{
			int offset=0;
			foreach (CSlide slide in mSlides)
			{
				offset = slide.CalcLengthInFrame(offset) ;
			}
	
			CSlide last_slide = ((CSlide)mSlides[mSlides.Count-1]);

			mLastFrame = last_slide.mStartFrameOffset+last_slide.GetFrameLength() - 1 ;
            mLastFrameBasedOnEncodeRatio = CGlobals.mCurrentProject.DiskPreferences.RenderToEncodeFPSRatio;
		}

  
		//*******************************************************************
		// Forces all the slides in slideshow to calc their length in frames
		public void CalcLengthOfAllSlides()
		{
			int offset = 0;

			if (mSlides.Count==0)
			{
				//Console.WriteLine("Warning: No slides in slide show");
				return ;
			}

			bool had_to_change_length_of_slides = false;

			if (this.mSyncLengthToMusic==true &&
				this.IsValidToSyncroniseToMusic()==false)
			{
				if (InvalidAudioSync!=null)
				{
					InvalidAudioSync();
				}
				mSyncLengthToMusic=false;
			}

			if (this.mSyncLengthToMusic==true)
			{
				NittyGrittySetSlideLengths();
				double total_music = this.GetLengthOfBackgroundMusic(false);
			
				if (total_music - this.GetLengthInSeconds() > 0.4 ||
					total_music - this.GetLengthInSeconds() < -0.4) 
				{
					// first set all varible length slides to default value

					double the_diff = total_music - this.GetLengthInSeconds();
					CDebugLog.GetInstance().Trace("Recalc slideshow to match music diff="+the_diff);
					
					foreach (CSlide slide in mSlides)
					{
						CStillPictureSlide s = slide as CStillPictureSlide;
						if (s!=null)
						{
							s.SetLengthWithoutUpdate(8.0f);
						}
					}

					NittyGrittySetSlideLengths();

					double total_time = this.GetLengthInSeconds();

					int total_variable = this.GetNumVariableTimeSlides();
				
					double to_add = total_music - total_time;
					double each_add = to_add /= ((double)total_variable);

			
					double real_clock_timer =0.0f;
					double actual_clock_timer =0.0f;
			
					foreach (CSlide slide in mSlides)
					{
						CStillPictureSlide s = slide as CStillPictureSlide;
						if (s!=null)
						{
							real_clock_timer+=each_add;

							double diff = real_clock_timer - actual_clock_timer;

							int frames = (int) (diff * CGlobals.mCurrentProject.DiskPreferences.frames_per_second);

							float l = ((float)frames) / CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

							actual_clock_timer+=l;

							l+=8.0f;
							if (l<0.2f) l = 0.2f;

							s.SetLengthWithoutUpdate(l);
						}
					}
					had_to_change_length_of_slides = true;
				}

			}
			
			NittyGrittySetSlideLengths();

			CalcLengthOfMusicSlides();
		}

		//*******************************************************************
		private void CalcLengthOfMusicSlides()
		{
			int offset=0;

			foreach (CMusicSlide ms in this.mMusicSlides)
			{
				offset = ms.CalcLengthInFrame(offset);
			}
            
            foreach (CNarrationAudioSlide narration in this.mNarrationSlides)
            {
                narration.CalcLengthInFrame();
            }

		}

		//*******************************************************************
		public override int GetTotalRenderLengthInFrames()
		{
			if (this.mLastFrame < 0 || CGlobals.mLimitVideoSeconds > 0) // srg fixme debug only
			{
				CalcLengthOfAllSlides();
			}

			if (this.mLastFrame <0) return 0;

			return this.mLastFrame +1;
		}

        //*******************************************************************
        public override int GetFinalOutputLengthInFrames()
        {
            return GetTotalRenderLengthInFrames() / mLastFrameBasedOnEncodeRatio;
        }
         
		//*******************************************************************      
		public override double GetLengthInSeconds()
		{
			if (this.mLastFrame < 0 )
			{
				CalcLengthOfAllSlides();
			}

			if (this.mLastFrame <0) return 0;

			return this.mLastFrame/CGlobals.mCurrentProject.DiskPreferences.frames_per_second;
		}    

		//*******************************************************************
		// return the current slide which would be show given a frame number
		public CSlide GetCurrentSlide(int frame)
		{
			if (mSlides.Count==0) return null;

			int current_slide =0;
			int current_total_frames =0;
			CSlide cs = (CSlide) mSlides[current_slide];

			while (frame > current_total_frames+cs.GetFrameLength())
			{
				// if we're on last slide it must be that
				if (current_slide >= mSlides.Count-1) 
				{
				//	Console.WriteLine("Warning frame number "+frame+" exceeds slide show length returning last frame"); 
					return cs ;
				}

				current_slide++;
				cs = (CSlide) mSlides[current_slide];
				current_total_frames=cs.mStartFrameOffset ;
				
			}

            CGlobals.mCurrentProcessSlide = current_slide;
			
			return (CSlide) mSlides[current_slide];
		}

		//*******************************************************************
		public CMusicSlide GetCurrentMusicSlide(int frame)
		{
			if (this.mMusicSlides.Count==0) return null;

			int current_slide =0;
			int current_total_frames =0;
			CMusicSlide cs =  (CMusicSlide) mMusicSlides[current_slide];

            while (frame >= current_total_frames + cs.GetFinalOutputLengthInFrames())
			{
				if (current_slide >= mMusicSlides.Count-1) 
				{
					return null ;
				}

				current_slide++;
				cs = (CMusicSlide) mMusicSlides[current_slide];
                current_total_frames = cs.StartFrameOffset;
				
			}
			
			return (CMusicSlide) mMusicSlides[current_slide];

		}

        //*******************************************************************
        public CNarrationAudioSlide GetCurrentNarrationSlide(int frame)
        {
            if (this.mNarrationSlides.Count == 0) return null;

            int current_slide = 0;
            int current_total_frames = 0;
            CNarrationAudioSlide cs = mNarrationSlides[current_slide];

            current_total_frames = cs.StartFrameOffset;

            while (frame >= current_total_frames + cs.GetFinalOutputLengthInFrames())
            {
                if (current_slide >= mNarrationSlides.Count - 1)
                {
                    return null;
                }

                current_slide++;
                cs = mNarrationSlides[current_slide];
                current_total_frames = cs.StartFrameOffset;

            }

            CNarrationAudioSlide current = mNarrationSlides[current_slide];
            if (current.StartFrameOffset > frame) return null;  // maybe blank between narrations

            return current;
        }


		//*******************************************************************
		public void AddMusicSlides(ArrayList tracks)
		{
			mMusicSlides.AddRange(tracks);

			InValidateLength();

			if (this.LoopMusic==true)
			{
				ReCalcLoopMusicSlides();
			}

			CGlobals.mCurrentProject.DeclareChange("Added Music slides");
		}

        //*******************************************************************
        public void AddNarrationSlide(CNarrationAudioSlide slide, int scrollPosition, out bool narrationStartsAfterSlideshowEnds)
        {
            ArrayList list = new ArrayList();
            list.Add(slide);
            AddNarrationSlides(list, scrollPosition, out narrationStartsAfterSlideshowEnds);
        }

        //*******************************************************************
        public void AddNarrationSlides(ArrayList tracks, int scrollPosition, out bool narrationStartsAfterSlideshowEnds)
        {
            narrationStartsAfterSlideshowEnds = false;

            foreach (object o in tracks)
            {
                CNarrationAudioSlide ms = o as CNarrationAudioSlide;
                if (ms != null)
                {
                    bool addedAfterSlideshow = false;
                    AddNarrationSlideInternal(ms, scrollPosition, out addedAfterSlideshow);
                    if (addedAfterSlideshow == true)
                    {
                        narrationStartsAfterSlideshowEnds = true;
                    }
                }
            }

            //
            // The narrations must be stored in the list in the order they are played.
            // This should be so, but just incase lets re-sort them now 
            //
            ReSortNarrationSlides();

            CGlobals.mCurrentProject.DeclareChange("Added narration slides");
        }

        //*******************************************************************
        private void AddNarrationSlideInternal(CNarrationAudioSlide slide, int scrollPosition, out bool narrationStartsAfterSlideshowEnds)
        {
            narrationStartsAfterSlideshowEnds = false;

            // if narration time not set append to end
            if (slide.StartNarrationTime < 0)
            {
                if (mNarrationSlides.Count == 0)
                {
                    slide.StartNarrationTime = 0;
                }
                else
                {
                    // try to slot the narration at the current given scroll (slide) position

                    float candidateTime = 0;

                    if (scrollPosition < mSlides.Count)
                    {
                        candidateTime = (mSlides[scrollPosition] as CSlide).StartTime;
                    }

                    bool found = false;
                    float startTime = candidateTime;

                    while (found == false)
                    {
                        found = true;

                        float hangBefore = 0;
                        float hangAfter = 0;

                        foreach (CNarrationAudioSlide narration in mNarrationSlides)
                        {
                            if (narration.Overlaps(startTime, startTime + ((float)slide.GetDurationInSeconds()), ref hangBefore, ref hangAfter) == true)
                            {
                                startTime = narration.StartNarrationTime + ((float)narration.GetDurationInSeconds());
                                found = false;
                                break;
                            }                      
                        }                 
                    }

                    slide.StartNarrationTime = startTime;

                }
            }

            int index = 0;
            while ((index <= (mNarrationSlides.Count-1)) &&
                   mNarrationSlides[index].StartNarrationTime < slide.StartNarrationTime)
            {
                index++;
            }

            mNarrationSlides.Insert(index, slide);
            CalcLengthOfMusicSlides();

            double slideshowLength = GetLengthInSeconds();
            if (slide.StartNarrationTime >= slideshowLength)
            {
                narrationStartsAfterSlideshowEnds = true;
            }

            return;
        }


        //*******************************************************************
        public void RemoveOrShortenOverlapppingNarrations(CNarrationAudioSlide forNarration)
        {
            float forStartTime = forNarration.StartNarrationTime;
            float forEndTime = forStartTime + ((float)forNarration.GetDurationInSeconds());

            List<CNarrationAudioSlide> narrationsToRemove = new List<CNarrationAudioSlide>();

            foreach (CNarrationAudioSlide narration in mNarrationSlides)
            {
                if (forNarration == narration)
                {
                    continue;
                }


                float hangBefore = 0;
                float hangAfter = 0;

                float startTime = narration.StartNarrationTime;
                float endTime = startTime + ((float)narration.GetDurationInSeconds());

                bool overlap = forNarration.Overlaps(startTime, endTime, ref hangBefore, ref hangAfter);

                if (overlap == false)
                {
                    continue;
                }

                if (hangBefore <= 0 && hangAfter <= 0)
                {
                    narrationsToRemove.Add(narration);
                    continue;
                }

                if (hangBefore > 0)
                {
                    narration.EndMusicOffset += narration.GetDurationInSeconds() - hangBefore;
                }
                else if (hangAfter > 0)
                {
                    narration.StartMusicOffset += narration.GetDurationInSeconds() - hangAfter;
                    narration.StartNarrationTime = forEndTime;
                }
            }

            foreach (CNarrationAudioSlide narration in narrationsToRemove)
            {
                mNarrationSlides.Remove(narration);
            }
        }

        //*******************************************************************
        public void ReSortNarrationSlides()
        {
            if (mNarrationSlides.Count <= 1)
            {
                return;
            }

            CNarrationAudioSlide[] narrationList = mNarrationSlides.ToArray();

            for (int j = 0; j < narrationList.Length - 1; j++)
            {
                for (int i = 0; i < narrationList.Length - 1; i++)
                {
                    if (narrationList[i].StartFrameOffset > narrationList[i + 1].StartFrameOffset)
                    {
                        CNarrationAudioSlide temp = narrationList[i + 1];
                        narrationList[i + 1] = narrationList[i];
                        narrationList[i] = temp;
                    }
                }
            }

            mNarrationSlides.Clear();
            mNarrationSlides.AddRange(narrationList);
        }

		//*******************************************************************
		// Work out if we need loop music slides and add/remove them etc.
		public void ReCalcLoopMusicSlides()
		{
			// no music slides? nothing to do!
			if (this.mMusicSlides.Count<1) return;

			// ok first remove any loop slides;
			ArrayList to_remove = new ArrayList();
			foreach (CMusicSlide ms in this.mMusicSlides)
			{
				if (ms.IsLoopMusicSlide==true)
				{
					to_remove.Add(ms);
				}
			}

			foreach (CMusicSlide mstor in to_remove)
			{
				this.mMusicSlides.Remove(mstor);
			}

            try
            {

                // not doing looping music or no music slides left, all done then.
                if (this.mLoopMusic == false || this.mMusicSlides.Count < 1)
                {
                    CalcLengthOfMusicSlides();
                    //
                    // Will cause disposing of old music slides in finally part
                    //
                    return;
                }

                // ok next create new loop music slides

                double slide_show_l = this.GetLengthInSeconds();
                double music_l = this.GetLengthOfBackgroundMusic(true);

                ArrayList loop_list = (ArrayList)this.mMusicSlides.Clone();
                int next_to_loop = 0;

                // keep adding loop tracks until slideshow time has been complete
                bool max_reached = false;// incase we have short audio and massive video (can crash otherwise)

                while (music_l < slide_show_l && max_reached == false)
                {
                    CMusicSlide tms = (CMusicSlide)loop_list[next_to_loop];

                    //
                    // First we reuse old looped music slides. I.e. to prevent
                    // more directsshow graph building occurring.
                    //
                    CMusicSlide nms = null;

                    foreach (CMusicSlide old_ms in to_remove)
                    {
                        if (old_ms.mName == tms.mName)
                        {
                            nms = old_ms;

                            //
                            // Offsets and fades are set to default.  I.e. looped music can not be trimmed.
                            // This is to prevent too many looped slides being generated by accident. i.e.
                            // If you were to trim a music to 2 seconds for example, then you could end up
                            // with 100s of 2 second looped music slides following it etc.
                            //
                            nms.StartMusicOffset = 0;
                            nms.FadeIn = false;
                            nms.EndMusicOffset = 0;
                            nms.FadeOut = false;
                            nms.Volume = tms.Volume;
                            break;
                        }
                    }
                    if (nms != null)
                    {
                        to_remove.Remove(nms);
                    }
                    else
                    {
                        if (CMusicSlide.TotalMusicSlidesInMemory < CMusicSlide.MaxMusicSlides)
                        {
                            //
                            // See comment above about criteria for creating a looped music slide
                            //
                            nms = new CMusicSlide(tms.mName,
                                true,
                                0,
                                false,
                                0,
                                false,
                                tms.Volume);
                        }
                        else
                        {
                            max_reached = true;
                            Log.Error("Too many music slides in memory " + CMusicSlide.TotalMusicSlidesInMemory);
                        }
                    }
                    if (nms != null)
                    {
                        this.mMusicSlides.Add(nms);
                    }

                    music_l = GetLengthOfBackgroundMusic(true);
                    next_to_loop++;
                    if (next_to_loop >= loop_list.Count)
                    {
                        next_to_loop = 0;
                    }
                }
                CalcLengthOfMusicSlides();
            }

            finally
            {
                // free up ms that are not needed
                foreach (CMusicSlide ms in to_remove)
                {
                    ms.Dispose();
                }
                to_remove.Clear();
                GC.Collect();
            }

		}

        //*******************************************************************
        private ArrayList GenrateAudioSlidesFromFilenames(string[] filenames, bool narration, out bool audioStartsAfterSlideshowEnds)
        {
            ArrayList audioTracks = new ArrayList();
            audioStartsAfterSlideshowEnds = false;

            double slide_show_l = this.GetLengthInSeconds();

            double audio_l = 0;
            if (narration == false)
            {
                audio_l = this.GetLengthOfBackgroundMusic(false);
            }
          
            foreach (string file in filenames)
            {                
                if (CMusicSlide.TotalMusicSlidesInMemory >= CMusicSlide.MaxMusicSlides)
                {
                    Log.Error("Added too many music slides. Ignoring remainder");
                    break;
                }

                string lower = file.ToLower();
                if (CGlobals.IsMusicFilename(lower) == true)
                {
                    CMusicSlide ms = null;
                    try
                    {
                        if (narration == true)
                        {
                            CNarrationAudioSlide ns = new CNarrationAudioSlide(file);
                            ns.StartNarrationTime = -1;
                            ms = ns;
                            
                        }
                        else
                        {
                            ms = new CMusicSlide(file);
                        }
                    }
                    catch
                    {
                        string tlo = file.ToLower();
                        if (tlo.EndsWith(".wma") == true)
                        {
                            UserMessage.Show("Could not import audio file '" + System.IO.Path.GetFileName(file) + "'\n\rThis file may contain copy protection. If so, PhotoVidShow can not import copy protected media.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            ms = null;
                        }
                        else
                        {
                            UserMessage.Show("Could not import audio file '" + System.IO.Path.GetFileName(file) + "'.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            ms = null;

                        }
                    }

                    if (ms != null)
                    {
                        //
                        // Is the current music soundtrack already longer than the slideshow?
                        //
                        if (narration == false && audioStartsAfterSlideshowEnds == false)
                        {                          
                            if (audio_l >= slide_show_l)
                            {
                                //
                                // Warn user after import that some music goes beyond the end of the slideshow
                                //
                                audioStartsAfterSlideshowEnds = true;
                            }
                            audio_l += ms.GetDurationInSeconds();
                        }

                        audioTracks.Add(ms);
                    }
                }
            }

            return audioTracks;

        }

		//*******************************************************************
		public void AddBackgroundMusicSlides(string [] filenames, out bool musicStartsAfterSlideshowEnds)
		{
            ArrayList music_tracks = GenrateAudioSlidesFromFilenames(filenames, false, out musicStartsAfterSlideshowEnds);

            if (music_tracks == null) return;

            if (music_tracks.Count > 0)
            {
                AddMusicSlides(music_tracks);
            }
		}

        
        //*******************************************************************
        public void AddNarrationSlides(string[] filenames, int scrollPosition, out bool narrationStartsAfterSlideshowEnds)
        {
            narrationStartsAfterSlideshowEnds = false;

            bool willNotBeSet = false;
            ArrayList narration_tracks = GenrateAudioSlidesFromFilenames(filenames, true, out willNotBeSet);

            if (narration_tracks == null)
            {
                return;
            }

            if (narration_tracks.Count > 0)
            {
                AddNarrationSlides(narration_tracks, scrollPosition, out narrationStartsAfterSlideshowEnds);
            }
        }

		//*******************************************************************
		// return the next slide which would be show given a frame number
		public CSlide GetNextSlide(int frame)
		{
			CSlide cs = GetCurrentSlide(frame);
			return GetNextSlide(cs) ;
		}

		//*******************************************************************
		// return the next slide which would be show given the current slide
		public CSlide GetNextSlide(CSlide current_slide)
		{
			int i =0;
			for (i=0;i < mSlides.Count-1;i++)
			{
				if (current_slide == (CSlide)mSlides[i])
				{
					return (CSlide)mSlides[i+1];
				}
			}
			return null ;
		}

		//*******************************************************************
		// return the previous slide which would be show given the current slide
		// null if first slide
		public CSlide GetPreviousSlide(CSlide current_slide)
		{
			CSlide previous = null;
			int i=0;
			for (i=0;i < mSlides.Count;i++)
			{
				if (current_slide == (CSlide)mSlides[i])
				{
					return previous;
				}
				previous = (CSlide)mSlides[i];
			}
			return null ;
		}

		//*******************************************************************
		public void RebuildToNewCanvas(CGlobals.OutputAspectRatio ratio)
		{
			int i =0;
			for (i=0;i < mSlides.Count;i++)
			{
				((CSlide)mSlides[i]).RebuildToNewCanvas(ratio);
			}
		}

        //*******************************************************************
        public void DeclareSlideAspectChange(CGlobals.OutputAspectRatio oldAspect, CGlobals.OutputAspectRatio newAspect)
        {
            int i = 0;
            for (i = 0; i < mSlides.Count; i++)
            {
                ((CSlide)mSlides[i]).DeclareSlideAspectChange(oldAspect, newAspect);
            }
        }


        //*******************************************************************
        private CSlide ConvertToCBlankStillPictureSlide(CSlide slide)
        {
            CBlankStillPictureSlide newSlide = null;

            CVideoSlide videoSlide = slide as CVideoSlide;

            if (videoSlide != null)
            {
                //
                // If the video slide has a null player, it may mean we were unable to locate the source video file (i.e. was reported as a missing file which needs re-linking).
                // In this case we can't convert. After the re-link has occured this then get called again and will be converted correctly.
                // 
                if (videoSlide.Player == null)
                {
                    Log.Info("Did not convert CVideoSlide as it had a null player (the source file name probably needs re-linking)");
                    return slide;
                }

                CVideoDecoration vd = CVideoDecoration.FromVideoSlide(videoSlide);
                if (vd != null)
                {
                    newSlide = new CBlankStillPictureSlide(vd);
                    vd.Orientation = slide.GetOrientationType();
                    vd.ReCalcCoverageAreaToMatchOriginalImageAspectRatio();
                }

            }
            else if (slide is CStillPictureSlide)
            {
                CStillPictureSlide sps = slide as CStillPictureSlide;
                CClipArtDecoration cad = CClipArtDecoration.FromStillPictureSlide(sps);
                if (cad != null)
                {
                    newSlide = new CBlankStillPictureSlide(cad);
                    cad.Orientation = slide.GetOrientationType();
                    cad.ReCalcCoverageAreaToMatchOriginalImageAspectRatio();
                    newSlide.DisplayLength = slide.DisplayLength;
                    newSlide.PanZoom = sps.PanZoom;
                }
            }
           
            if (newSlide==null)
            {
                Log.Warning("Can not convert unexpected slide type in ConvertToCBlankStillPictureSlide");
                return slide;
            }

            CImageSlide imageSlide = slide as CImageSlide;

            newSlide.TransistionEffect = slide.TransistionEffect;
       
            ArrayList decors = imageSlide.GetAllAndSubDecorations();

            foreach (CDecoration d in decors)
            {
                d.AttachedToSlideImage = false; // legacy

                // becuase of old text decor used background which is now backplabe
                CTextDecoration td = d as CTextDecoration;
                if (td != null && td.IsBackgroundDecoration() == true)
                {
                    td.SetBackplane(true);
                    td.UnMarkAsBackgroundDecoration();
                }
                    
                newSlide.AddDecoration(d);
            }

            return newSlide;

        }

		//*******************************************************************
		public override string GetAudioFileName()
		{
            // ?????? this is used menus only ??? why is this here
			if (mMusicSlides.Count <1) return "";
			return ((CMusicSlide)mMusicSlides[0]).mName;
		}

		//*******************************************************************
		//
		public double GetLengthOfAllVideoSlidesInSeconds()
		{
			double total_time =0.0f;
			foreach (CSlide slide in mSlides)
			{
				CVideoSlide vs = slide as CVideoSlide;
				if (vs!=null)
				{
					total_time+=vs.GetDurationInSeconds();
				}			
			}	
			return total_time;
		}


		//*******************************************************************
		public int GetNumStillPictureSlides()
		{
			int count=0;
			foreach (CSlide slide in mSlides)
			{
				if (slide is CStillPictureSlide)
				{
					count++;
				}			
			}	
			return count;
		}

        //*******************************************************************
        public int GetNumMultiSlideSlides()
        {
            int count = 0;
            foreach (CSlide slide in mSlides)
            {
                if (slide is CMultiSlideSlide)
                {
                    count++;
                }
            }
            return count;
        }
		
		//*******************************************************************
		public int GetNumVideoSlides()
		{
			int count=0;
			foreach (CSlide slide in mSlides)
			{
				if (slide is CVideoSlide)
				{
					count++;
				}			
			}	
			return count;
		}

		//*******************************************************************
		//
		public int GetNumVariableTimeSlides()
		{

			int total =0;
			foreach (CSlide slide in mSlides)
			{
				if (slide is CStillPictureSlide)
				{
					total++;
				}			
			}	
			return total;

		}

		//*******************************************************************
		public double GetLengthOfBackgroundMusic(bool include_looped_tracks)
		{
			double total_time =0.0f;
			foreach (CMusicSlide ms in this.mMusicSlides)
			{
				if (ms.IsLoopMusicSlide==false || include_looped_tracks==true)
				{
					total_time+=ms.GetDurationInSeconds();
				}
			}
		    return total_time;

		}


        //*******************************************************************
        public bool ContainsSlide(CSlide s)
        {
            foreach (CSlide ms in this.mSlides)
            {
                if (s.ID == ms.ID) return true;
            }
            return false;
        }

		//*******************************************************************
		// this is used by the project author to work out if it's worth doing
		// 23.976 3:2 pulldown rather than normal 29.97
		public bool IsNTSCFilmSlideshow()
		{
			bool do_as_32_pulldown = false;

			foreach (CSlide slide in this.mSlides)
			{
				CVideoSlide vs = slide as CVideoSlide;
				if (vs!=null)
				{
					// is 23.976 and more than 10 minutes long
					
					if (System.Math.Abs(1/vs.GetFrameRate() - 23.976) < 0.001f
						//	&&vs.DisplayLength > 10*60)
						)
					{
						do_as_32_pulldown=true;
					}
					else
					{
						return false;
					}
				}

                CImageSlide imageSlide = slide as CImageSlide;

                ArrayList list = imageSlide.GetAllAndSubDecorations();
                foreach (CDecoration d in list)
                {
                    CVideoDecoration vd = d as CVideoDecoration;
                    if (vd != null)
                    {
                        if (System.Math.Abs(1 / vd.GetFrameRate() - 23.976) < 0.001f)
                        {
                            do_as_32_pulldown = true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
			}


			return do_as_32_pulldown;
		}

        //*******************************************************************
        public override int GetMaxNumberRequiredMotionBlurSubFrames()
        {
            if (CGlobals.mCurrentProject.DiskPreferences.TurnOffMotionBlur == true)
            {
                return 1;
            }

            int max = 1;
            foreach (CSlide slide in mSlides)
            {
                int cand = slide.GetMaxRequiredMotionBlurSubFrames();
                if (cand > max)
                {
                    max = cand;
                }
            }
            return max;
        }

        //*******************************************************************
        public override int GetNumberRequiredMotionBlurSubFrames(int frame)
        {
            CSlide current_slide = GetCurrentSlide(frame);
            CSlide next_slide = GetNextSlide(frame);

            int required_frames = 1;

            if (current_slide != null)
            {
                int current_slide_frames = current_slide.GetNumberRequiredMotionBlurSubFrames(frame);
                if (current_slide_frames > required_frames)
                {
                    required_frames = current_slide_frames;
                }
            }

            if (next_slide != null)
            {
                if (next_slide.mStartFrameOffset <= frame)
                {
                    int next_slide_frames = next_slide.GetNumberRequiredMotionBlurSubFrames(frame);
                    if (next_slide_frames > required_frames)
                    {
                        required_frames = next_slide_frames;
                    }
                }
            }

            return required_frames;
        }

        //*******************************************************************
        // Used when exporting templates
        public void ConvertAllSlidesToMultiSlideSlide()
        {
            // All slides in current slideshow should get added to one new multislideslide

            List<CImageSlide> mSubSlides = new List<CImageSlide>();

            foreach (CSlide slide in mSlides)
            {
                CImageSlide imageSlide = slide as CImageSlide;
                if (imageSlide != null)
                {
                    if (imageSlide is CMultiSlideSlide == true)
                    {
                        Log.Error("Slideshow already contains multislideslide, ignoring....");
                        continue;
                    }
                    mSubSlides.Add(imageSlide);
                }
            }

            if (mSubSlides.Count == 0) return;

            CMultiSlideSlide multislide = new CMultiSlideSlide(mSubSlides);

            // force mb takes the same as slide 0 of sub slides
            multislide.ForcedMotionBlurSubFrames = mSubSlides[0].ForcedMotionBlurSubFrames;
            multislide.PreviewPlayWhenEditingSlideMedia = mSubSlides[0].PreviewPlayWhenEditingSlideMedia;
            multislide.SetLengthWithoutUpdate((float)this.GetLengthInSeconds());

            // thumbnail preview also takes the same as slide 0 of sub slides
            multislide.ThumbnailPreviewTime = mSubSlides[0].ThumbnailPreviewTime;

            // set transition effect of multi slide slide as last slide effect
            multislide.TransistionEffect = mSubSlides[mSubSlides.Count - 1].TransistionEffect.Clone();

            mSlides.Clear();
            mSlides.Add(multislide);
            
        }

        //*******************************************************************
        public void ConvertMultiSlideSlideToSlides(CMultiSlideSlide slide)
        {
            int index = mSlides.IndexOf(slide);
            if (index < 0) return;

            mSlides.Remove(slide);

            List<CImageSlide> subSlides = slide.SubSlides;

            foreach (CImageSlide subSlide in subSlides)
            {
                mSlides.Insert(index++,subSlide);
            }

            CalcLengthOfAllSlides();
        }

        //*******************************************************************
        //
        // This method gets an alias or label name for the given slideshow
        // This is usually what the user has entered in the text decoration next to the
        // slideshow button in the menu
        //
        public string GetSlideshowLabelName()
        {
            CSlideShow s = this;

            string name = s.Name;

            CMainMenu menu = CGlobals.mCurrentProject.GetMenuWhichContainsSlideshow(s);

            if (menu == null) return s.Name;

            ArrayList buttons = menu.GetSlideShowButtons();

            foreach (CMenuSlideshowButton button in buttons)
            {
                if (button.GetInnerImageStringId() == name)
                {
                    ArrayList textattach = button.AttachedChildDecorations;

                    foreach (int d in textattach)
                    {
                        CDecoration dec = menu.BackgroundSlide.GetDecorationFromID(d);
                        if (dec != null)
                        {
                            CTextDecoration td = dec as CTextDecoration;
                            if (td != null
                                && td.mText.Length > 0)
                            {
                                string ase = td.mText;
                                ase = ase.Replace('\r', ' ');
                                ase = ase.Replace('\n', ' ');
                                if (ase.Length > 17) ase = ase.Substring(0, 17);
                                return ase;
                            }
                        }
                    }
                }
            }
            if (name.Length > 17) name = name.Substring(0, 17);

            return name;
        }


        //*******************************************************************
        private List<DateTime> GenerateTimeIntervalChapterMarkers()
        {
            // Needed (when calling GetLengthInSeconds) incase we skipped the generate video, i.e. when debugging
            CalcLengthOfAllSlides();

            TimeSpan tick = new TimeSpan(mChapterMarkersTimeInterval.Hour,mChapterMarkersTimeInterval.Minute,mChapterMarkersTimeInterval.Second);
            TimeSpan totalTick = tick;
            DateTime currentTime = mChapterMarkersTimeInterval;

            List<DateTime> markers = new List<DateTime>();
            double slideshowLengthInSeconds = GetLengthInSeconds();

            while (totalTick.TotalSeconds < slideshowLengthInSeconds)
            {
                markers.Add(currentTime);
                currentTime = currentTime.Add(tick);
                totalTick = totalTick.Add(tick);
            }

            return markers;
        }

        //*******************************************************************
        private List<DateTime> GenerateChapterMarkersFromSlides()
        {
            // Needed (when calling GetLengthInSeconds) incase we skipped the generate video, i.e. when debugging
            CalcLengthOfAllSlides();

            List<DateTime> markers = new List<DateTime>();
            foreach (CSlide slide in mSlides)
            {
                if (slide.MarkedAsChapter == true)
                {
                    float startTime = slide.StartTime;

                    TimeSpan ts = TimeSpan.FromSeconds(startTime);
                    DateTime marker = new DateTime(2014, 1, 1, ts.Hours, ts.Minutes, ts.Seconds);
                    markers.Add(marker);
                }
            }

            return markers;
        }

        //*******************************************************************
        public void Load(XmlElement element)
        {
            string s = element.GetAttribute("Name");

            if (s != "")
            {
                mName = s;
            }

            s = element.GetAttribute("FadeIn");
            if (s != "")
            {
                mFadeIn = bool.Parse(s);
            }

            string fo = element.GetAttribute("FadeOut");
            if (fo != "")
            {
                this.mFadeOut = bool.Parse(fo);
            }


            string fic = element.GetAttribute("FadeInColor");
            if (fic != "")
            {
                this.mFadeInColor = CGlobals.ParseColor(fic);
            }

            string foc = element.GetAttribute("FadeOutColor");
            if (foc != "")
            {
                this.mFadeOutColor = CGlobals.ParseColor(foc);
            }


            string fia = element.GetAttribute("FadeInAudio");
            if (fia != "")
            {
                mFadeInAudio = bool.Parse(fia);
            }

            string foa = element.GetAttribute("FadeOutAudio");
            if (foa != "")
            {
                mFadeOutAudio = bool.Parse(foa);
            }

            string lm = element.GetAttribute("LoopMusic"); ;
            if (lm != "")
            {
                mLoopMusic = bool.Parse(lm);
            }

            string stm = element.GetAttribute("SyncToMusic");
            if (stm != "")
            {
                this.mSyncLengthToMusic = bool.Parse(stm);
            }

            string vv = element.GetAttribute("VideoVolume");
            if (vv != "")
            {
                this.mVideoAudioVol = float.Parse(vv, CultureInfo.InvariantCulture);
            }


            string mv = element.GetAttribute("MusicVolume");
            if (mv != "")
            {
                this.mBackGroundMusicVol = float.Parse(mv, CultureInfo.InvariantCulture);
            }

            string pes = element.GetAttribute("PreEncodedSlideshow");
            if (pes != "")
            {
                this.mPreEncodedSlideshow = bool.Parse(pes);
            }

            string chapterMarkers = element.GetAttribute("ChapterMarkers");
            if (chapterMarkers != "")
            {
                this.mChaperMarkers = (CSlideShow.ChapterMarkersType) int.Parse(chapterMarkers);
            }

            string chapterMarkersTimedInterval = element.GetAttribute("ChapterMarkersTimedInterval");
            if (chapterMarkersTimedInterval != "")
            {
                mChapterMarkersTimeInterval = DateTime.Parse(chapterMarkersTimedInterval);
            }

            XmlNodeList list = element.GetElementsByTagName("DefaultSlide");

            bool loaded_default_slide = true;
            if (list.Count != 0)
            {
                loaded_default_slide = false;
            }

            list = element.GetElementsByTagName("Slide");


            int slides_added = 0;


            foreach (XmlElement e in list)
            {
                // As some slide have sub slide (i.e. multislideslides)
                // Only load child slides
                if (e.ParentNode == element || e.ParentNode.Name == "DefaultSlide")
                {
                    CSlide slide = CSlide.CreateFromType(e.GetAttribute("Type"));

                    if (slide == null) continue;

                    try
                    {
                        slide.Load(e);

                        if (loaded_default_slide == false)
                        {
                            this.mDefaultSlide = slide;
                            loaded_default_slide = true;
                        }
                        else
                        {
                            // Convert older style slides (v3.1.4 and previous to new type) and not PhotoCruz
                            if (slide is CBlankStillPictureSlide == false &&
                                slide is CMultiSlideSlide == false &&
                                CGlobals.RunningPhotoCruz == false)
                            {
                                slide = ConvertToCBlankStillPictureSlide(slide);
                            }

                            mSlides.Add(slide);
                            slides_added++;
                        }
                    }

                    catch (ErrorException exception)
                    {
                        CDebugLog.GetInstance().Error("Exception occurred when loading a slide... Ignoring :" + exception.Message);

                        if (CGlobals.RunningPhotoCruz == true)
                        {
                            UserMessage.Show("Failed to load slide (ignoring)", "Error");
                        }

                    }
                    catch (ManagedCore.MissingFileException mfe)
                    {
                        if (CGlobals.RunningPhotoCruz == true)
                        {
                            UserMessage.Show("Could not find file '" + mfe.mOriginalFullFilename + "'\r\nIgnoring slide file is used on.", "Missing file");
                        }
                    }
                    catch (IgnoreOperationException)
                    {
                    }
                }
            }


            list = element.GetElementsByTagName("MusicSlide");

            foreach (XmlElement e in list)
            {
                try
                {
                    CMusicSlide ms = new CMusicSlide();
                    ms.Load(e);
                    this.mMusicSlides.Add(ms);
                }
                catch (ErrorException)
                {
                    CDebugLog.GetInstance().Warning("Failed To load music... Ignoring");
                }
                catch (ManagedCore.MissingFileException)
                {
                }
                catch (IgnoreOperationException)
                {
                }
            }

            list = element.GetElementsByTagName("NarrationAudioSlide");

            foreach (XmlElement e in list)
            {
                try
                {
                    CNarrationAudioSlide narration = new CNarrationAudioSlide();
                    narration.Load(e);
                    this.mNarrationSlides.Add(narration);
                }
                catch (ErrorException)
                {
                    CDebugLog.GetInstance().Warning("Failed to load narration... Ignoring");
                }
                catch (ManagedCore.MissingFileException)
                {
                }
                catch (IgnoreOperationException)
                {
                }
            }

            CalcLengthOfAllSlides();

            //
            // Version 4.1.8 and earlier had a bug where narrations could of been stored in the wrong order, so here we make sure the 
            // loaded narrations are in the correct order.
            // 
            ReSortNarrationSlides();

            // set thumbnail
            string mtis = element.GetAttribute("MenuThumbnailIndex");
            if (mtis != "")
            {
                try
                {
                    this.mMenuThumbnailSlide = (CSlide)this.mSlides[int.Parse(mtis)];
                }
                catch (Exception e4)
                {
                    CDebugLog.GetInstance().Error("Menuthumbnail index invalid in slideshow: " + e4.Message);
                }
            }
        }

        //*******************************************************************
        public void Save(XmlElement parent, XmlDocument doc, bool asTemplate)
        {
            XmlElement slideshow = doc.CreateElement("Slideshow");

            if (asTemplate == false)
            {
                slideshow.SetAttribute("Name", this.mName);
                slideshow.SetAttribute("FadeIn", this.mFadeIn.ToString());
                slideshow.SetAttribute("FadeOut", this.mFadeOut.ToString());
                slideshow.SetAttribute("FadeInColor", this.mFadeInColor.ToString());
                slideshow.SetAttribute("FadeOutColor", this.mFadeOutColor.ToString());
                slideshow.SetAttribute("FadeInAudio", this.mFadeInAudio.ToString());
                slideshow.SetAttribute("FadeOutAudio", this.mFadeOutAudio.ToString());
                slideshow.SetAttribute("LoopMusic", this.mLoopMusic.ToString());
                slideshow.SetAttribute("SyncToMusic", this.mSyncLengthToMusic.ToString());
                slideshow.SetAttribute("VideoVolume", this.mVideoAudioVol.ToString());
                slideshow.SetAttribute("MusicVolume", this.mBackGroundMusicVol.ToString());
                if (mPreEncodedSlideshow == true)
                {
                    slideshow.SetAttribute("PreEncodedSlideshow", this.mPreEncodedSlideshow.ToString());
                }

                if (mChaperMarkers == ChapterMarkersType.None || mChaperMarkers == ChapterMarkersType.SetFromSlides)
                {
                    slideshow.SetAttribute("ChapterMarkers", ((int)mChaperMarkers).ToString());

                }

                if (mChaperMarkers == ChapterMarkersType.RegularTimedInterval && mChapterMarkersTimeInterval.Minute !=5)
                {
                    slideshow.SetAttribute("ChapterMarkersTimedInterval", mChapterMarkersTimeInterval.ToShortTimeString());
                }        
               
                // save default slide

                XmlElement default_slide = doc.CreateElement("DefaultSlide");
                this.mDefaultSlide.Save(default_slide, doc);
                slideshow.AppendChild(default_slide);
            }

            foreach (CSlide slide in mSlides)
            {
                slide.Save(slideshow, doc);
            }

            if (asTemplate == false)
            {
                foreach (CMusicSlide mt in mMusicSlides)
                {
                    mt.Save(slideshow, doc);
                }

                foreach (CNarrationAudioSlide narration in mNarrationSlides)
                {
                    narration.Save(slideshow, doc);
                }

                if (mMenuThumbnailSlide != null)
                {
                    int index = mSlides.IndexOf(mMenuThumbnailSlide);
                    if (index >= 0)
                    {
                        slideshow.SetAttribute("MenuThumbnailIndex", index.ToString());
                    }
                }
            }

            parent.AppendChild(slideshow);
        }


	}	
}
