
// represents a timeline description of all the individual audio which will occur in a video.


using System;
using System.Collections;
using MangedToUnManagedWrapper;
using System.Xml;
using ManagedCore;
using System.Windows.Forms;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for CMusicTrack.
	/// </summary>
	/// 
	
	public class CMusicPerformance
	{
		private ArrayList mMusicPerformanceChannels;
		private bool mFadeIn=false;
		private bool mFadeOut=true;
		private int mLengthInFrames =0;
		private bool mIsDolbyAC3=false;
		static private bool mAbort=false;

		public bool IsDolbyAC3
		{
			get
			{
				return mIsDolbyAC3;
			}
		}


		private ArrayList MusicPerformanceChannels
		{
			get
			{
				return mMusicPerformanceChannels;
			}
		}

        public bool ContainsAudio
        {
            get
            {
                if (mMusicPerformanceChannels.Count >= 1)
                {
                    return true;
                }
                return false;
            }
        }
        
		//*******************************************************************
		static public void Abort()
		{
			CMusicPerformaceChannel.Abort();
			mAbort=true;
		}

		//*******************************************************************
		private CMusicPerformance()
		{
			mMusicPerformanceChannels = new ArrayList();
		}

        //*******************************************************************
        public void CalculatePreviewAudioLevels(double time, ref float outMusicVol, ref float outVideoVol, ref float outNarrationVol, float backGroundMusicVol, float videoAudioVol, float narrationVolume)
        {

            outMusicVol = 1.0f;
            outVideoVol = 1.0f;
            outNarrationVol = 1.0f;

            double length_in_seconds = mLengthInFrames / CGlobals.mCurrentProject.DiskPreferences.AudioFramesPerSecond;

            // Apply any fade requests in the music channel (i.e. because video is playing) 
            bool applied_music_fade = false;

            foreach (CMusicPerformaceChannel channel in mMusicPerformanceChannels)
            {
                if (channel.MusicPerformanceFadeSectionRequests.Count >0)
                {
                    // SRG MUST BE MUSIC CHANNNEL !!??

                    foreach (CMusicPeformanceFadeSectionRequest request in channel.MusicPerformanceFadeSectionRequests)
                    {
                        double end = request.mStartOffset + request.mLength;
                        if (time > request.mStartOffset && time <= end)
                        {
                            double delta = 0.0;
                            double vol_ratio = request.mFadeRatio;
                            // fade down
                            if (request.mFadeDownLength > 0 && time < request.mStartOffset + request.mFadeDownLength)
                            {
                                delta = 1.0 - ((time - request.mStartOffset) / request.mFadeDownLength );
                                vol_ratio = ((1.0 - request.mFadeRatio) * delta) + request.mFadeRatio;
                            }
                            else if (request.mFadeUpLength > 0 && time > end - request.mFadeUpLength)
                            {
                                delta = 1.0 - ((end - time) / request.mFadeUpLength);
                                vol_ratio = ((request.mFadeUpEndRatio - request.mFadeRatio) * delta) + request.mFadeRatio;
                            }
                            if (delta > 1.0)
                            {
                                vol_ratio = request.mFadeUpEndRatio;
                            }
                            if (delta < 0.0)
                            {
                               vol_ratio = 1.0;
                            }

                            outMusicVol *= ((float)vol_ratio);
                            // ok peform fade
                            applied_music_fade = true;
                            break;
                        }
                    }

                }

                if (applied_music_fade == true) break;
            }
                
            // APPLY FADE IN/OUT and START/END slideshow

            if (this.mFadeIn == true &&
                time <= CGlobals.FadeInAudioTime)
            {
                float delta = (float) time / CGlobals.FadeInAudioTime ;

                outVideoVol = delta * outVideoVol;
                outMusicVol = delta * outMusicVol;
                outNarrationVol = delta * outNarrationVol;
                
            }

            else if (this.mFadeOut == true &&
                time >= length_in_seconds -  CGlobals.FadeOutAudioTime)
            {
                float f1 = (float)(length_in_seconds - time);
                float ft = (float)CGlobals.FadeOutAudioTime ;

                float delta = f1 / ft;
                outVideoVol = delta * outVideoVol;
                outMusicVol = delta * outMusicVol;
                outNarrationVol = delta * outNarrationVol;
            }

            outMusicVol *= backGroundMusicVol;
            outVideoVol *= videoAudioVol;
            outNarrationVol *= narrationVolume;
        }

        //*******************************************************************
        public static CMusicPerformance FromVideo(CVideo video)
		{
			CMusicPerformaceChannel.cc=0;

			CSlideShow slideshow = video as CSlideShow;
			if (slideshow!=null)
			{
				return FromSlideshow(slideshow) ;
			}

			CMainMenu menu = video as CMainMenu;
			if (menu!=null)
			{
				return FromMenu(menu) ;
			}

			return null;
		}

        //*******************************************************************
        // returns the video decoration we apply audio too in a given slide. else returns null
        // Note: if there are multiple video decorations with audio, we ignore all of them.
        public static CVideoDecoration GetAudioVideoDecorationForSlide(CSlide forSlide)
        {
            CImageSlide ims = forSlide as CImageSlide;
            if (ims == null) return null;

            CVideoDecoration foundDecor = null;

            foreach (CDecoration dec in ims.GetAllAndSubDecorations())
            {
                CVideoDecoration vd = dec as CVideoDecoration;
                if (vd != null &&
                    vd.ContainsAudio() == true &&
                    vd.IsFilter() == false)
                {
                    if (foundDecor != null)
                    {
                        return null;
                    }
                    foundDecor = vd;
                }
            }

            return foundDecor;
        }

        //*******************************************************************
        private static bool AddVideoDecorationAudio(CSlide forSlide, CMusicPerformaceChannel video_slide_channel,  float fps, ref float apply_fade, ref double fade_start_time, ref double fade_length)
        {
            CImageSlide ims = forSlide as CImageSlide;
            if (ims == null) return false;

            CVideoDecoration vd = GetAudioVideoDecorationForSlide(forSlide);
            if (vd == null || vd.Player==null) return false;

            // If video decoration has a 0% volume then ignore it.
            if (vd.Volume == 0)
            {
                vd.PreviewAudioUnit = null;
                return false;
            }

            float start_time = ims.StartTime + vd.GetStartOffsetTime(ims.DisplayLength);
            float lengthDecorationShownFor = vd.GetLengthTimeShownFor(ims.DisplayLength);
            double totallengthofvideo = vd.Player.GetDurationInSeconds();
            float lengthOfLoopedVideoPlayback = (float) ( totallengthofvideo - vd.StartVideoOffset - vd.EndVideoOffset);
            float lengthLeft = lengthDecorationShownFor;

            int count =0;
            int frameOffset = 0;
            while (lengthLeft > 0.1f && count < 100)
            {
                float nextAuLength = lengthLeft > lengthOfLoopedVideoPlayback ? lengthOfLoopedVideoPlayback : lengthLeft;

                CMusicPeformanceAudioUnit au = new CMusicPeformanceAudioUnit();
                au.mFilename = vd.Player.mFileName;
                au.mTotalLengthOfAudioInSeconds = totallengthofvideo;
                au.mLengthInFrames = (int)((nextAuLength * fps)+0.4999f);
                au.mStartFrameOffset =  frameOffset + (int)((start_time * fps)+0.4999f);
                au.mWavStartOffset = (int)((vd.StartVideoOffset * fps)+0.4999f);
                au.mVolume = (float)vd.Volume;
                au.mFadeIn = vd.FadeAudioIn;
                au.mFadeInLength = vd.FadeAudioInLength;
                au.mFadeOut = vd.FadeAudioOut;
                au.mFadeOutLength = vd.FadeAudioOutLength;
                            
                video_slide_channel.AddAudioUnit(au);

                //
                // video decoration also has reference to audio unit (used when previewing)
                //
                if (count == 0)
                {
                    vd.PreviewAudioUnit = au;   // SRG fix me vd could have a list of previewaudiounits not just one
                }

                lengthLeft -= lengthOfLoopedVideoPlayback;
                frameOffset += au.mLengthInFrames;
                count++;
            }

            // need to reduce background music when video playing?
            // if so create a fade request which will get added to the music channel later
            if (vd.EnableMusicFadeWhilePlayingVideo)
            {
                apply_fade = vd.MusicFadeWhilePlayingVideo;
                fade_start_time = start_time;
                fade_length = lengthDecorationShownFor;                         
            }

            return true;
        }
    
        //*******************************************************************
		private static CMusicPerformance FromSlideshow(CSlideShow slideshow)
		{
			CMusicPerformaceChannel.cc=0;
            
			// CMusicPeformanceFadeSectionRequest
			ArrayList FadeSectionRequests = new ArrayList();

			CMusicPerformance mp = new CMusicPerformance();

			mp.mFadeIn = slideshow.FadeInAudio;
			mp.mFadeOut = slideshow.FadeOutAudio;
			mp.mLengthInFrames = slideshow.GetFinalOutputLengthInFrames();
	
			// ok get any video slides audio
			bool has_video_slide_audio=false;

			CMusicPerformaceChannel video_slide_channel = new CMusicPerformaceChannel();
			video_slide_channel.mVolume = slideshow.VideoAudioVolume;

			// if slideshow video audio has not been set to 0
			// add video slides audio to performance
			if (slideshow.VideoAudioVolume > 0.0001)
			{
                float fps = CGlobals.mCurrentProject.DiskPreferences.AudioFramesPerSecond;

				foreach (CSlide slide in slideshow.mSlides)
				{
					// If there is a video decoration which has audio and is not a filter, then use that instead!
                    // unless there are multiples of these, in which case we ignore them.

                    float apply_fade = 1.0f;
                    double fade_start_time = 0;
                    double fade_length = 0;
                    
				    bool hasVideoDecorationAudio = AddVideoDecorationAudio(slide, video_slide_channel, fps, ref apply_fade, ref fade_start_time, ref fade_length);

                    if (hasVideoDecorationAudio)
                    {
                        has_video_slide_audio = true;
                    }
						
                    // No found video audio so get it from video slide instead...
					if (hasVideoDecorationAudio == false)
					{
						CVideoSlide vs = slide as CVideoSlide;
						if (vs != null)
						{
							if (vs.ContainsAudio() == true)
							{
                                // 
                                // This prevents us trying to create an .ac3 6 channel file
                                //
								// if (vs.HasDolbyAC3Sound() == true)
								// {
								//      mp.mIsDolbyAC3 = true;
								// }

								has_video_slide_audio = true;
	
								CMusicPeformanceAudioUnit au = new CMusicPeformanceAudioUnit();
								au.mFilename = vs.SourceFilename;
                                au.mLengthInFrames = (int) (vs.DisplayLength * fps);
								au.mStartFrameOffset = (int) (vs.StartTime *fps);
								au.mWavStartOffset = (int)(vs.StartVideoOffset * fps);
								au.mVolume = (float)vs.Volume;
								au.mTotalLengthOfAudioInSeconds = vs.Player.GetDurationInSeconds();

								video_slide_channel.AddAudioUnit(au);

								// need to reduce background music when video playing?
								// if so create a fade request which will get added to the music channel later
								if (vs.EnableMusicFadeWhilePlayingVideo==true)
								{
                                    apply_fade = vs.MusicFadeWhilePlayingVideo;
                                    fade_start_time = slide.StartTime;
                                    fade_length = slide.DisplayLength;
                                }
							}
						}
					}
                    if (apply_fade < 0.999)
                    {
                        CMusicPeformanceFadeSectionRequest request = new CMusicPeformanceFadeSectionRequest();

                        double start = fade_start_time;
                        double length = fade_length;

                        request.mStartOffset = (float)start - request.mFadeDownLength;
                        request.mLength = (float)length + request.mFadeDownLength + request.mFadeUpLength;
                        request.mFadeRatio = apply_fade;
                        FadeSectionRequests.Add(request);
                    }
                }
			}
		
			if (has_video_slide_audio==true)
			{
                //
                // Ok Some video audio untis will overlap, this needs to be sorted here
                //
                MergeOverlappingVideoAudioUnits(video_slide_channel);
				mp.AddChannel(video_slide_channel);
			}


			// ok get background music

			bool has_background_audio=false;
			CMusicPerformaceChannel background_music_channel = new CMusicPerformaceChannel();
			background_music_channel.mVolume = slideshow.BackgroundMusicVolume;


			foreach (CMusicSlide ms in slideshow.mMusicSlides)
			{
				has_background_audio=true;
				CMusicPeformanceAudioUnit au = new CMusicPeformanceAudioUnit();
				au.mFilename = ms.mName;
                au.mLengthInFrames = ms.GetFinalOutputLengthInFrames();
                au.mStartFrameOffset = ms.StartFrameOffset;
                au.mWavStartOffset = (int)(ms.StartMusicOffset * CGlobals.mCurrentProject.DiskPreferences.AudioFramesPerSecond);
				au.mFadeIn = ms.FadeIn;
                au.mFadeInLength = ms.FadeInLength;
				au.mFadeOut = ms.FadeOut;
                au.mFadeOutLength = ms.FadeOutLength;
				au.mVolume = (float)ms.Volume;
				au.mInitialSilence = (float)ms.InitialSilence;
				au.mTotalLengthOfAudioInSeconds = ms.Player.GetDurationInSeconds();
				background_music_channel.AddAudioUnit(au);
			}

			if (has_background_audio==true)
			{

				// if we had fade requests we need to add them to the music channel
				if (FadeSectionRequests.Count >0)
				{
					// clip and remove overlapping etc
					ClipAndMergeFadeRequests(ref FadeSectionRequests, slideshow);
					background_music_channel.AddFadeRequests(FadeSectionRequests);
				}
	
				mp.AddChannel(background_music_channel);
			}

            // Narration
            if (slideshow.NarrationSlides.Count > 0)
            {
                CMusicPerformaceChannel narration_channel = new CMusicPerformaceChannel();
                narration_channel.mVolume = slideshow.NarrationVolume;

                foreach (CNarrationAudioSlide narration in slideshow.NarrationSlides)
                {
                    CMusicPeformanceAudioUnit au = new CMusicPeformanceAudioUnit();
                    au.mFilename = narration.mName;
                    au.mLengthInFrames = narration.GetFinalOutputLengthInFrames();
                    au.mStartFrameOffset = narration.StartFrameOffset;
                    au.mWavStartOffset = (int)(narration.StartMusicOffset * CGlobals.mCurrentProject.DiskPreferences.AudioFramesPerSecond);
                    au.mFadeIn = narration.FadeIn;
                    au.mFadeInLength = narration.FadeInLength;
                    au.mFadeOut = narration.FadeOut;
                    au.mFadeOutLength = narration.FadeOutLength;
                    au.mVolume = (float)narration.Volume;
                    au.mInitialSilence = (float)narration.InitialSilence;
                    au.mTotalLengthOfAudioInSeconds = narration.Player.GetDurationInSeconds();
                    narration_channel.AddAudioUnit(au);

                    //
                    // The narration slide also has a reference to this audio unit as used when previewing
                    //
                    narration.PreviewAudioUnit = au;
                }

                mp.AddChannel(narration_channel);
            }

			return mp;
		}

        //*******************************************************************
        //
        // This will merge two overlapping video audio units such that
        // they no longer overlap
        //
        private static void MergeOverlappingVideoAudioUnits(CMusicPerformaceChannel videoChannel)
        {
            ArrayList AudioUnits = videoChannel.MusicPerformanceAudioUnits;
            if (AudioUnits.Count <=1)
            {
                return;
            }

            for (int i=0;i<AudioUnits.Count-1;i++)
            {
                CMusicPeformanceAudioUnit a = AudioUnits[i] as CMusicPeformanceAudioUnit;
                CMusicPeformanceAudioUnit b = AudioUnits[i + 1] as CMusicPeformanceAudioUnit;
                if (a!= null && b != null)
                {
                    int aEndFrame = a.mStartFrameOffset + a.mLengthInFrames;
                    int bStartFrame = b.mStartFrameOffset;
                    if (aEndFrame> bStartFrame)
                    {
                        //
                        // They overlap, split the difference between the two overlapping units
                        //
                        int different = aEndFrame - bStartFrame;
                        int half = different / 2;
                        int newALength = a.mLengthInFrames - half;
                        if (newALength < 0)
                        {
                            newALength = 0;
                        }
                        a.mLengthInFrames = newALength;

                        int newBStartOffset = bStartFrame + half;
                        if (newBStartOffset > (bStartFrame + b.mLengthInFrames))
                        {
                            b.mLengthInFrames = 0;
                        }
                        else
                        {
                            b.mLengthInFrames -= half;
                            if (b.mLengthInFrames <0)
                            {
                                b.mLengthInFrames = 0;
                            }
                        }

                        //
                        // Move the wav start offset by the same amount we have moved the start frame offset
                        // as we have more or less performed a small trim at the start of the b video.
                        //
                        b.mWavStartOffset += (newBStartOffset - b.mStartFrameOffset); 
                        b.mStartFrameOffset = newBStartOffset;
                      
                    }
                }
            }
        }

        //*******************************************************************
        // Perform clipping and remove overlaps in fade requests
        private static void ClipAndMergeFadeRequests(ref ArrayList FadeSectionRequests, CSlideShow slideshow)
		{
			double slideshowEndTime = slideshow.GetLengthInSeconds();

			CMusicPeformanceFadeSectionRequest lastRequest = null;
            
            ArrayList newList = new ArrayList();


            foreach (CMusicPeformanceFadeSectionRequest request in FadeSectionRequests)
            {
                // fade needed straight away, if so don't bother fading down
                if (request.mStartOffset < 0)
                {
                    request.mLength -= -request.mStartOffset;
                    request.mStartOffset = 0;
                    request.mFadeDownLength = 0;            
                }

                // fade needed at end of slideshow, if do don't bother fading back up
                if (request.mStartOffset + request.mLength >= slideshowEndTime)
                {
                    request.mLength = slideshowEndTime - request.mStartOffset;
                    request.mFadeUpLength = 0;
                }

                // check does not overlap last fade if so just merge them
                if (lastRequest != null)
                {
                    // ok overlap
                    if (lastRequest.mStartOffset + lastRequest.mLength >= request.mStartOffset)
                    {
                        // same fade ratio? just merge request
                        if (request.mFadeRatio == lastRequest.mFadeRatio)
                        {
                            double end = request.mStartOffset + request.mLength;
                            lastRequest.mLength = end - lastRequest.mStartOffset;
                        }
                        else 
                        {
                            // ok set last fade up to finish at same as current request fade ratio
                            // amd remove anyfade down on current request
                            double end = request.mStartOffset + request.mLength;
                            request.mStartOffset = lastRequest.mStartOffset + lastRequest.mLength;
                            request.mLength = end - request.mStartOffset;
                            request.mFadeDownLength = 0;
                            lastRequest.mFadeUpEndRatio = request.mFadeRatio;
                            newList.Add(lastRequest);
                            lastRequest = request;
                        }
                    }
                    else
                    {
                        newList.Add(lastRequest);
                        lastRequest = request;
                    }
                }
                else
                {
                    lastRequest = request;
                }
            }

            if (lastRequest != null)
            {
                newList.Add(lastRequest);
            }

            FadeSectionRequests = newList;
		}


		
		//*******************************************************************
		private static CMusicPerformance FromMenu(CMainMenu menu)
		{
			CMusicPerformance mp = new CMusicPerformance();
			CMusicPerformaceChannel.cc=0;
			mp.mFadeIn = false;
			mp.mFadeOut = menu.FadeAudioOut;
            mp.mLengthInFrames = (int)(menu.Length * CGlobals.mCurrentProject.DiskPreferences.AudioFramesPerSecond);

			// ok get any video slides audio
			bool has_video_slide_audio=false;
		

			CMusicPerformaceChannel video_slide_channel = new CMusicPerformaceChannel();

            CVideoSlide vs = menu.BackgroundSlide as CVideoSlide;
			if (vs!=null)
			{
				if (vs.ContainsAudio()==true)
				{
					has_video_slide_audio=true;

                    // 
                    // This prevents us trying to create an .ac3 6 channel file
                    //
					// if (vs.HasDolbyAC3Sound()==true)
					// {
					//  	mp.mIsDolbyAC3=true;
					// }

					CMusicPeformanceAudioUnit au = new CMusicPeformanceAudioUnit();
					au.mFilename = vs.SourceFilename;

					// hack getting the length of the menu slide always returns 0. for a special reason i can't remeber
					// so getting direct length from video player
					au.mLengthInFrames = (int) (vs.GetDurationInSeconds() * CGlobals.mCurrentProject.DiskPreferences.AudioFramesPerSecond);
					au.mStartFrameOffset = 0;	// always 0!!

					video_slide_channel.AddAudioUnit(au);
				}
			}
 
			if (has_video_slide_audio==true)
			{
				mp.AddChannel(video_slide_channel);
			}


			// ok get background music

			bool has_background_audio=false;
			CMusicPerformaceChannel background_music_channel = new CMusicPerformaceChannel();

			CMusicSlide ms = menu.MusicSlide;
			if (ms!=null)
			{
				has_background_audio=true;
				CMusicPeformanceAudioUnit au = new CMusicPeformanceAudioUnit();
				au.mFilename = ms.mName;
                au.mLengthInFrames = ms.GetFinalOutputLengthInFrames();
                au.mStartFrameOffset = ms.StartFrameOffset;

				background_music_channel.AddAudioUnit(au);
			}

			if (has_background_audio==true)
			{
				mp.AddChannel(background_music_channel);
			}

			return mp;
		}


		//*******************************************************************
		private void AddChannel(CMusicPerformaceChannel channel)
		{
			mMusicPerformanceChannels.Add(channel);
		}

		
		//*******************************************************************
		private CMusicPerformaceChannel GetChannel(int index)
		{
			return (CMusicPerformaceChannel) mMusicPerformanceChannels[index];
		}


		//*******************************************************************
		public void RipDolbyAC3(string name)
		{
			if (mMusicPerformanceChannels.Count <0)
			{
                CDebugLog.GetInstance().Error("No music performance channels when ripping for ac3");
				return ;
			}
			if (mMusicPerformanceChannels.Count >1)
			{
				CDebugLog.GetInstance().Warning("More than 1 channel when ripping ac3, only doing video slides channel");
			}

			((CMusicPerformaceChannel)mMusicPerformanceChannels[0]).RipAC3(name);

			MangedToUnManagedWrapper.ManagedObject.Clear();
		
		}

        //*******************************************************************
        private void EnsureMixedBufferSameLengthAsVideoStream(string outputAudioFile, Int64 audioFileSize)
        {
            const int maxWriteInOneGo =1024 * 1024;
       
            double length_in_seconds_audio = ((double)(audioFileSize>>2)) / CGlobals.mCurrentProject.DiskPreferences.AudioFrequency;
            double length_in_seconds_video = ((double)mLengthInFrames) / CGlobals.mCurrentProject.DiskPreferences.AudioFramesPerSecond;

            try
            {
                // if audio less than half a second that video, then pad out
                if (length_in_seconds_audio < length_in_seconds_video - 0.5)
                {
                    double missingInSeconds = length_in_seconds_video - length_in_seconds_audio;
                    Log.Info("Main audio file '" + outputAudioFile + "' is shorter than main video by " + missingInSeconds + " seconds (padding with silent).");
                    using (var fileStream = new System.IO.FileStream(outputAudioFile, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.None))
                    using (var bw = new System.IO.BinaryWriter(fileStream))
                    {
                        int amount = (int)((length_in_seconds_video - length_in_seconds_audio) * CGlobals.mCurrentProject.DiskPreferences.AudioFrequency);
                        amount *= 4;

                        byte[] emptyAudio = null;
                        while (amount > 0)
                        {
                            int writeNow = amount;
                            if (writeNow > maxWriteInOneGo)
                            {
                                writeNow = maxWriteInOneGo;
                            }

                            if (emptyAudio != null && emptyAudio.Length != writeNow)
                            {
                                emptyAudio = null;
                            }

                            if (emptyAudio == null)
                            {
                                emptyAudio = new byte[writeNow];
                            }

                            bw.Write(emptyAudio);

                            amount -= writeNow;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception occurred when padding main audio file '" + outputAudioFile + "' (" + e.Message+")");
            }
        }

		//*******************************************************************
		public void EncodeIntoSingleStereoStream(string name, int frequency)
		{
			CGlobals.DeclareEncodeCheckpoint('m');
			mAbort=false;

			if (1==1)
			{
				// how many channels will we neeed
				if (this.mMusicPerformanceChannels.Count<1) 
				{
					CGlobals.DeclareEncodeCheckpoint('n');
					return ;
				}
					
				ArrayList channels_to_encode = new ArrayList();

				// if only 1 channel this is going to be simple
				if (mMusicPerformanceChannels.Count==1)
				{
					channels_to_encode.Add(mMusicPerformanceChannels[0]);
				}
				else
				{
					// try to merge channels to give mimimum mixing required

					for (int i=0;i<mMusicPerformanceChannels.Count;i++)
					{
						if (mAbort==true)
						{	
							MangedToUnManagedWrapper.ManagedObject.Clear();
							CGlobals.DeclareEncodeCheckpoint('o');
							return ;
						}
						bool encode_channel =true;
						if (i != mMusicPerformanceChannels.Count-1)
						{
							for (int j=i+1;j<mMusicPerformanceChannels.Count;j++)
							{
								if (GetChannel(i).WillOverlapWith(GetChannel(j))==false)
								{
									// SRG code me
									//	merge_channel(j,i)
									//	encode_channel= false
										
								}
							}
						}
						if (encode_channel==true)
						{
							channels_to_encode.Add(GetChannel(i));
						}
					}
				}

				CManagedFileBufferCache [] mpf = new CManagedFileBufferCache[channels_to_encode.Count];

				CManagedAudioMixer mixer= new CManagedAudioMixer(frequency);

				for (int count=0; count < channels_to_encode.Count; count++)
				{
					mpf[count]= ((CMusicPerformaceChannel)channels_to_encode[count]).Encode(frequency);
					mixer.AddStream(mpf[count],((CMusicPerformaceChannel)channels_to_encode[count]).mVolume);
				}

				CManagedFileBufferCache mixed_buffer =null ;
				// if only 1 channel no pointing mixing it.
				if (channels_to_encode.Count==1)
				{
					mixed_buffer = (CManagedFileBufferCache) mpf[0];
				}
				else
				{
					CGlobals.DeclareEncodeCheckpoint('{');
					int buffer_num = mixer.Process(); // result mixing is in first mpf;
					CGlobals.DeclareEncodeCheckpoint('}');
					mixed_buffer = (CManagedFileBufferCache) mpf[buffer_num];
				}
                mixer.Release();

          
				//mixed_buffer.WriteHackWavHead();


				CGlobals.DeclareEncodeCheckpoint('p');
				if (mFadeIn==true && mAbort==false)
				{
					CManagedAudioFader fader = new CManagedAudioFader(mixed_buffer,frequency);
					float offset = 0;
					fader.Process(CManagedAudioFader.ManagedFadeType.FADE_IN,offset,CGlobals.FadeInAudioTime);
					fader.Release();
				}

				if (mFadeOut==true && mAbort==false)
				{
					CManagedAudioFader fader = new CManagedAudioFader(mixed_buffer,frequency);
					float offset = (((float)mLengthInFrames) / CGlobals.mCurrentProject.DiskPreferences.AudioFramesPerSecond)-CGlobals.FadeOutAudioTime;
					fader.Process(CManagedAudioFader.ManagedFadeType.FADE_OUT,offset,CGlobals.FadeOutAudioTime+2);
					fader.Release();

				}

             
				string cache_name = (string) mixed_buffer.GetFilename().Clone();
                Int64 length_of_cache= mixed_buffer.GetSize();

				foreach (CManagedFileBufferCache fbc in mpf)
				{
					if (fbc!=null)
					{
						fbc.Dispose();
					}
				}

             
				mixed_buffer.Dispose();

                // make sure single audio stream same length as video  (can be shorter for example if only video audio and slideshow ends with pictures slides)
                EnsureMixedBufferSameLengthAsVideoStream(cache_name, length_of_cache);

				if (mAbort==false) 
				{	
					try
					{
						System.IO.File.Move(cache_name, name);
					}
					catch (Exception exception)
					{
						CDebugLog.GetInstance().Error("Could not rename audio file from '"+cache_name+"' to '"+name+"' ("+exception.Message+")");
					}
				}

			}

			// delete all cache files
			string t = CGlobals.GetTempDirectory();
			for (int i=0;i<100;i++)
			{
				string nname = t+"\\cache"+i;
				if (System.IO.File.Exists(nname)==true)
				{
					ManagedCore.IO.ForceDeleteIfExists(nname,true);
				}
			}

			MangedToUnManagedWrapper.ManagedObject.Clear();
			GC.Collect();
			CGlobals.DeclareEncodeCheckpoint('q');
		}
	}
}




