using System;
using MangedToUnManagedWrapper;
using System.Xml;
using ManagedCore;
using System.Globalization;
//using System.Windows.Controls;
//using System.Windows;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for CMusicTrack.
	/// </summary>
	public class CMusicSlide : IDisposable
	{
		object this_lock = new object();
		public string mName;
		private MangedToUnManagedWrapper.CAudioPlayer mPlayer;
	
		protected int	mStartFrameOffset =-1;

        public const int MaxMusicSlides = 125;

        public int StartFrameOffset
        {
            get { return mStartFrameOffset; }
        }
        

		private double mStartMusicOffset =0;
		private double mEndMusicOffset =0 ;
		private bool mFadeIn =false;
        private float mFadeInLength = 1.0f;
		private bool mFadeOut =false;
        private float mFadeOutLength = 1.0f;
		private double mMasterVolume = 1.0;
		private double mInitialSilence=0.0;
        private bool mDisposed = false;

		private bool mIsLoopMusicSlide = false;	// the system created this music slide so music is looped

		private bool mIsPlaying = false;

		private static int mNumberOfMusicSlidesInMemory =0;

		public static int TotalMusicSlidesInMemory
		{
			get
			{
				return mNumberOfMusicSlidesInMemory;
			}
		}

		public double InitialSilence
		{
			get { return mInitialSilence; }
			set { mInitialSilence = value;}
		}

		public bool FadeIn
		{
			get { return mFadeIn; }
			set { mFadeIn = value ; }
		}

        public float FadeInLength
        {
            get { return mFadeInLength; }
            set { mFadeInLength = value; }
        }

		public double Volume
		{
			get { return mMasterVolume; }
			set { mMasterVolume = value; }
		}
		
		public bool FadeOut
		{
			get { return mFadeOut; }
			set { mFadeOut = value ; }
		}

        public float FadeOutLength
        {
            get { return mFadeOutLength; }
            set { mFadeOutLength = value; }
        }

        public MangedToUnManagedWrapper.CAudioPlayer Player
		{
			get
			{
				return mPlayer;
			}
		}

		//*******************************************************************
		public double StartMusicOffset
		{
			get 
			{
				return mStartMusicOffset;
			}
			set 
			{
				mStartMusicOffset=value;
			}
		}

		//*******************************************************************
		public double EndMusicOffset
		{
			get 
			{
				return mEndMusicOffset;
			}
			set 
			{
				mEndMusicOffset=value;
			}
		}

		//*******************************************************************
		public bool IsLoopMusicSlide
		{
			get
			{
				return mIsLoopMusicSlide;
			}
		}
	
		//*******************************************************************
		public CMusicSlide()
		{
			mNumberOfMusicSlidesInMemory++;
		}

		//*******************************************************************
		public CMusicSlide(string name)
		{
			mNumberOfMusicSlidesInMemory++;
			mName = name ;	
			Init();
		}

		//*******************************************************************
		public CMusicSlide(string name,
			bool loop_slide,
			double start_offset,
			bool fade_in,
			double end_offset,
			bool fade_out,
			double volume)
		{
			mNumberOfMusicSlidesInMemory++;
			mIsLoopMusicSlide= loop_slide;
			mName = name ;	
			mFadeOut = fade_out;
			mFadeIn = fade_in;
			mStartMusicOffset = start_offset;
			mEndMusicOffset = end_offset;
			mMasterVolume = volume;

         

            Init();
		}

      //  private MediaElement me;

        //*******************************************************************
        private void Init()
		{
			mPlayer = new CAudioPlayer(mName);

            /*
            me = new MediaElement();
          
            me.Source = new System.Uri(mName);
            me.LoadedBehavior = MediaState.Manual;
            me.Position = TimeSpan.Zero;
            me.MediaOpened += Me_MediaOpened;
            me.MediaFailed += Me_MediaFailed; ;
            me.Play();
            */
        }

        /*
        private void Me_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Me_MediaOpened(object sender, RoutedEventArgs e)
        {
            me.Pause();
            Duration d = me.NaturalDuration;
            if (d.HasTimeSpan == true)
            {
                int mins = d.TimeSpan.Minutes;
                int secs = d.TimeSpan.Seconds;
            }
            me.Stop();
        }
        */


        //*******************************************************************
        ~CMusicSlide()
		{
            Dispose();
		}

        //*******************************************************************
        public void Dispose()
        {
            if (mDisposed == false)
            {
                mDisposed = true;

                mNumberOfMusicSlidesInMemory--;
                if (mPlayer != null)
                {
                    mPlayer.Dispose();      // Will delete the low level C++ Directshow graph.
                    mPlayer.Release();      // Allows object to be GC in future
                    mPlayer = null;
                }
            }
        }

		//*******************************************************************
		public void ApplyWithVolume(double vol)
		{
			double to_apply = this.mMasterVolume * vol;

			int db_loss = CGlobals.vol_ratio_to_dx9_dbloss((float)to_apply);

			if (this.mPlayer!=null)
			{
				this.mPlayer.SetVolume(db_loss);
			}
		}

		//*******************************************************************
		// called every frame on preview to capture fade in and out on a music slide
		public void UpdateVolume(double vol, int frame)
		{
			if (this.mPlayer==null) return ;

            float fps = CGlobals.mCurrentProject.DiskPreferences.AudioFramesPerSecond;

			int audio_start_frame =((int) (this.mInitialSilence*fps))+mStartFrameOffset;

			double the_vol = vol * this.mMasterVolume;
			int master_db_loss =CGlobals.vol_ratio_to_dx9_dbloss((float)the_vol);

			if (frame < this.mStartFrameOffset) return;
            if (frame >= this.mStartFrameOffset + this.GetFinalOutputLengthInFrames()) return;

		
			int s_frame_diff = frame - audio_start_frame;
			float s_dif = ((float)s_frame_diff)/ fps;
            int e_frame_diff = GetFinalOutputLengthInFrames() - (frame - mStartFrameOffset);
			float e_dif = ((float)e_frame_diff)/fps;

			if (s_dif < mFadeInLength && mFadeIn ==true)
			{
				double delta =  s_dif / mFadeInLength;
				if (delta <0) delta = 0;
				if (delta >1) delta =1;

				float db_loss_mus = CGlobals.vol_ratio_to_dx9_dbloss((float)delta);

				if (((int)db_loss_mus) < master_db_loss)
				{
					master_db_loss = (int)db_loss_mus;
				}
			}
			else if  (e_dif< mFadeOutLength && mFadeOut ==true)
			{
				double delta =  e_dif / mFadeOutLength;
				if (delta <0) delta = 0;
				if (delta >1) delta =1;

				float db_loss_mus = CGlobals.vol_ratio_to_dx9_dbloss((float)delta);

				if (((int)db_loss_mus) < master_db_loss)
				{
					master_db_loss = (int) db_loss_mus;
				}
			}
			
			if (this.mPlayer!=null)
			{
				this.mPlayer.SetVolume(master_db_loss);
			}
		}

		//*******************************************************************
		public void Play()
		{
			this.Play(mStartFrameOffset);
		}

		//*******************************************************************
		public void Play(int current_frame)
		{
			lock (this_lock)
			{

				if (mStartFrameOffset==-1)
				{
					CDebugLog.GetInstance().Error("Called Play on MusicSlide before CalcLengthInFrame had been done");
				}

                int fl = mStartFrameOffset + GetFinalOutputLengthInFrames();

				// if asked frame outside actually playback area just stop audio.

                float fps = CGlobals.mCurrentProject.DiskPreferences.AudioFramesPerSecond;

				int rel_frame = current_frame - mStartFrameOffset;
				int audio_start_frame = ((int)(mInitialSilence*fps));

                if (rel_frame < audio_start_frame || rel_frame >= GetFinalOutputLengthInFrames() - 1) 
				{
					Stop();
					return ;
				}

				if (CGlobals.VideoSeeking==true)
				{
					//		Console.WriteLine("seeking to time "+rel_frame);

					double st = (((double)(rel_frame))/fps) + mStartMusicOffset- mInitialSilence;;
			//		Console.WriteLine("music seek to "+ st+" wait = "+CGlobals.WaitVideoSeekCompletion);

					// SRG ok when seeking with preview track bar we get bombarded with seek commands
					// with no wait.  ignore these and wait for the final "with wait" command which
					// means the user had finished their seek and ready to play again

					if (CGlobals.WaitVideoSeekCompletion==true)
					{
						mPlayer.SeekToTime(st, 
							CGlobals.WaitVideoSeekCompletion);
					}

					if (CGlobals.ContinuePlayingAudioAfterSeeking==false)
					{
						// called pause here becuase we've already done the seek to where
						// we want to go and stop does a seek back to the start
						Pause();
					}
					CGlobals.ContinuePlayingAudioAfterSeeking=false;

					return ;
				}
					
				mIsPlaying=true;
				mPlayer.Play();
				//	Console.WriteLine("Playing track "+mName+" frame is "+current_frame+" end music frame ="+fl);
			}

		}

		//*******************************************************************
		public void Stop()
		{	
			lock (this_lock)
			{
				if (mIsPlaying==false) return ;

				mIsPlaying=false;
			//	Console.WriteLine("Called stop "+this.mName);
				mPlayer.Stop(this.StartMusicOffset,false);
			}
		}

		//*******************************************************************
		private void Pause()
		{	
			lock (this_lock)
			{
				if (mIsPlaying==false) return ;
				mIsPlaying=false;
			//	Console.WriteLine("Called pause "+this.mName);
				mPlayer.Pause();
			}
		}


		//*******************************************************************
		public void Reset()
		{
			mIsPlaying=false;
			mPlayer.Stop(this.StartMusicOffset,true);
		}

		//*******************************************************************
		public double GetDurationInSeconds()
		{
			return mPlayer.GetDurationInSeconds() - (mStartMusicOffset+mEndMusicOffset) + this.mInitialSilence;
		}


		//*******************************************************************
		public virtual int CalcLengthInFrame(int offset)
		{
			mStartFrameOffset = offset;
            return mStartFrameOffset + GetFinalOutputLengthInFrames() + 1;
		}	


		//*******************************************************************
        public int GetFinalOutputLengthInFrames()
		{
			double d = GetDurationInSeconds();
            return (int)(d * CGlobals.mCurrentProject.DiskPreferences.AudioFramesPerSecond );
		}

		//*******************************************************************
		public virtual void Save(XmlElement parent, XmlDocument doc)
		{
			XmlElement track = doc.CreateElement("MusicSlide");

            SaveMusicSlidePart(track);

			parent.AppendChild(track); 
		}


        //*******************************************************************
        protected void SaveMusicSlidePart(XmlElement element)
        {
            string filename = CGlobals.StripRootHeader(mName);
            element.SetAttribute("Name", filename);
            element.SetAttribute("Loop", this.mIsLoopMusicSlide.ToString());
            element.SetAttribute("StartOffset", this.mStartMusicOffset.ToString());
            element.SetAttribute("EndOffset", this.mEndMusicOffset.ToString());
            element.SetAttribute("FadeIn", this.mFadeIn.ToString());
            if (mFadeIn==true)
            {
                element.SetAttribute("FadeInLength", this.mFadeInLength.ToString());
            }
            element.SetAttribute("FadeOut", this.mFadeOut.ToString());
            if (mFadeOut==true)
            {
                element.SetAttribute("FadeOutLength", this.mFadeOutLength.ToString());
            }
            element.SetAttribute("Volume", this.mMasterVolume.ToString());
            element.SetAttribute("InitialSilence", this.mInitialSilence.ToString());
        }


		//*******************************************************************
		public virtual void Load(XmlElement element)
		{
			mName = element.GetAttribute("Name");
			string ll = element.GetAttribute("Loop");
			if (ll!="")
			{
				this.mIsLoopMusicSlide= bool.Parse(ll);
			}

			ll = element.GetAttribute("StartOffset");
			if (ll!="")
			{
				this.mStartMusicOffset = double.Parse(ll,CultureInfo.InvariantCulture);
			}

			ll = element.GetAttribute("EndOffset");
			if (ll!="")
			{
				this.mEndMusicOffset = double.Parse(ll,CultureInfo.InvariantCulture);
			}


			ll = element.GetAttribute("FadeIn");
			if (ll!="")
			{
				this.mFadeIn = bool.Parse(ll);
			}

            ll = element.GetAttribute("FadeInLength");
            if (ll!="")
            {
                this.mFadeInLength = float.Parse(ll);
            }

			ll = element.GetAttribute("FadeOut");
			if (ll!="")
			{
				this.mFadeOut = bool.Parse(ll);
			}

            ll = element.GetAttribute("FadeOutLength");
            if (ll != "")
            {
                this.mFadeOutLength = float.Parse(ll);
            }

            ll = element.GetAttribute("Volume");
			if (ll!="")
			{
				this.mMasterVolume = double.Parse(ll,CultureInfo.InvariantCulture);
			}

			ll = element.GetAttribute("InitialSilence");
			if (ll!="")
			{
				this.mInitialSilence = double.Parse(ll,CultureInfo.InvariantCulture);
			}

            mName = CGlobals.GetFullPathFilename(mName);
            mName = CGlobals.CheckFileExistsElseThrow(mName);

			Init();
		}


	}
}
