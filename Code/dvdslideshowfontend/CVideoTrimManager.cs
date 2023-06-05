using System;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using DVDSlideshow;
using System.Collections;
using DVDSlideshow.GraphicsEng;

namespace dvdslideshowfontend
{
	/// <summary>
	/// Summary description for CVideoTrimManager.
	/// </summary>
	/// 
	

	// responsible for the video trim tab page

	public class CVideoTrimManager
	{
        /*
		object this_lock = new object();

		PictureBox mStartFramePictureBox;
		PictureBox mEndFramePictureBox;
		Label	   mStartFrameTimeLabel;
		Label	   mEndFrameTimeLabel;
		TrackBar   mVideoTrimStartFrameTrackBar;
		TrackBar   mVideoTrimEndFrameTrackBar;
        TrackBar   mFadeBackgroundMusicTrackBar;
        CheckBox mFadeBackgroundMusicTickBox;
        TextBox mFadeBackgroundMusicTextBox;
        
		System.Windows.Forms.Timer	mStartTrackerTimer = null;
		System.Windows.Forms.Timer	mEndTrackerTimer = null;

		CVideoSlide mForVideoSlide;
        CSlideShow mTrimSlideShow;

		// ******************************************************
		public CVideoTrimManager()
		{
            return;

            mFadeBackgroundMusicTextBox = Form1.mMainForm.GetFadeBackgroundMusicTextBox();
            mFadeBackgroundMusicTickBox = Form1.mMainForm.GetFadeBackgroundMusicTickBox();
            mFadeBackgroundMusicTrackBar = Form1.mMainForm.GetFadeBackgroundMusicTrackBar();

			mEndFramePictureBox = Form1.mMainForm.GetTrimVideoEndFramePictureBox(); 
			mStartFramePictureBox =Form1.mMainForm.GetTrimVideoStartFramePictureBox();

			mStartFrameTimeLabel = Form1.mMainForm.GetTrimVideoStartTime();
			mEndFrameTimeLabel = Form1.mMainForm.GetTrimVideoEndTime();

			mVideoTrimStartFrameTrackBar = Form1.mMainForm.GetVideoTrimStartFrameTrackBar();
			mVideoTrimEndFrameTrackBar = Form1.mMainForm.GetVideoTrimEndFrameTrackBar();

			mVideoTrimStartFrameTrackBar.Scroll+=new EventHandler(this.StartScrollChanged);
			mVideoTrimStartFrameTrackBar.MouseUp+=new MouseEventHandler(this.MouseUpStartTrack);
			mVideoTrimEndFrameTrackBar.Scroll+=new EventHandler(this.EndScrollChanged);
			mVideoTrimEndFrameTrackBar.MouseUp+=new MouseEventHandler(this.MouseUpEndTrack);

            mFadeBackgroundMusicTickBox.CheckedChanged += new EventHandler(FadeBackgroundMusicCheckBoxChanged);
            mFadeBackgroundMusicTrackBar.Scroll += new EventHandler(this.FadeBackgroundMusicFadeScrolled);
            mFadeBackgroundMusicTrackBar.MouseUp += new MouseEventHandler(this.FadeBackgroundMusicMouseUp);
            
			Form1.mMainForm.GetResetVideoTrimButton().Click+=new EventHandler(this.Reset);

			Form1.mMainForm.GetVideoTrimBackOneFrameStartFrameButton().Click+=new EventHandler(GetVideoTimeBackOneFrameStartFrameButton);
			Form1.mMainForm.GetVideoTrimBackOneSecondStartFrameButton().Click+=new EventHandler(GetVideoTimeBackOneSeoncdStartFrameButton);
			Form1.mMainForm.GetVideoTrimForwardOneFrameStartFrameButton().Click+=new EventHandler(GetVideoTimeForwardOneFrameStartFrameButton);
			Form1.mMainForm.GetVideoTrimForwardOneSecondStartFrameButton().Click+=new EventHandler(GetVideoTimeForwardOneSeoncdStartFrameButton);


			Form1.mMainForm.GetVideoTimeBackOneFrameEndFrameButton().Click+=new EventHandler(GetVideoTimeBackOneFrameEndFrameButton);
			Form1.mMainForm.GetVideoTimeBackOneSeoncdEndFrameButton().Click+=new EventHandler(GetVideoTimeBackOneSeoncdEndFrameButton);
			Form1.mMainForm.GetVideoTimeForwardOneFrameEndFrameButton().Click+=new EventHandler(GetVideoTimeForwardOneFrameEndFrameButton);
			Form1.mMainForm.GetVideoTimeForwardOneSeoncdEndFrameButton().Click+=new EventHandler(GetVideoTimeForwardOneSeoncdEndFrameButton);


			Form1.mMainForm.GetTrimVideoDoneButton().Click+=new EventHandler(this.Done_click);

			Form1.mMainForm.GetVideoVolumeTrackBar().Scroll += new EventHandler(this.VolumeScrolled);
            Form1.mMainForm.GetVideoVolumeTrackBar().MouseUp += new MouseEventHandler(this.VolumeMouseUp);
        
			ToolTip tt = Form1.mMainForm.mToolTip;
		
			tt.SetToolTip(Form1.mMainForm.GetVideoTimeBackOneFrameEndFrameButton(),@"Back 1 frame");
			tt.SetToolTip(Form1.mMainForm.GetVideoTimeBackOneSeoncdEndFrameButton(),@"Back 1 second");
			tt.SetToolTip(Form1.mMainForm.GetVideoTimeForwardOneFrameEndFrameButton(),@"Forward 1 frame");
			tt.SetToolTip(Form1.mMainForm.GetVideoTimeForwardOneSeoncdEndFrameButton(),@"Forward 1 second");

			tt.SetToolTip(Form1.mMainForm.GetVideoTrimBackOneFrameStartFrameButton(),@"Back 1 frame");
			tt.SetToolTip(Form1.mMainForm.GetVideoTrimBackOneSecondStartFrameButton(),@"Back 1 second");
			tt.SetToolTip(Form1.mMainForm.GetVideoTrimForwardOneFrameStartFrameButton(),@"Forward 1 frame");
			tt.SetToolTip(Form1.mMainForm.GetVideoTrimForwardOneSecondStartFrameButton(),@"Forward 1 second");
			tt.SetToolTip(Form1.mMainForm.GetVideoVolumeTrackBar(),@"Set video volume");
			tt.SetToolTip(Form1.mMainForm.GetResetVideoTrimButton(),@"Reset video settings");
			tt.SetToolTip(Form1.mMainForm.GetTrimVideoDoneButton(),@"Finish trimming video");
	
		}

		// ******************************************************
		public void GetVideoTimeBackOneFrameStartFrameButton(object o, System.EventArgs e)
		{
			double fps = GetVideoSlidefps();
			double sub = 1/fps;
		
			if (this.mForVideoSlide!=null)
			{
				mForVideoSlide.StartVideoOffset =this.mForVideoSlide.StartVideoOffset - sub;

				if (mForVideoSlide.StartVideoOffset <0)
				{
					mForVideoSlide.StartVideoOffset = 0;
				}

				mVideoTrimStartFrameTrackBar.Value =(int)(mForVideoSlide.StartVideoOffset*fps);
			
				this.ReCalcTotalDurationLabel();
				this.ReDrawStartPictureBox(true);
				this.CommitChanges(true,false);
			}

		}

		// ******************************************************
		public void GetVideoTimeBackOneSeoncdStartFrameButton(object o, System.EventArgs e)
		{
			double fps = GetVideoSlidefps();
			double duration = this.mForVideoSlide.Player.GetDurationInSeconds();

			if (this.mForVideoSlide!=null)
			{
				mForVideoSlide.StartVideoOffset =this.mForVideoSlide.StartVideoOffset - 1;

				if (mForVideoSlide.StartVideoOffset < 0 )
				{
					mForVideoSlide.StartVideoOffset =0 ;
				}

				mVideoTrimStartFrameTrackBar.Value =(int)(mForVideoSlide.StartVideoOffset*fps);
			
				this.ReCalcTotalDurationLabel();
				this.ReDrawStartPictureBox(true);
				this.CommitChanges(true,false);
			}
		}

		// ******************************************************
		public void GetVideoTimeForwardOneFrameStartFrameButton(object o, System.EventArgs e)
		{
			double fps = GetVideoSlidefps();
			double duration = this.mForVideoSlide.Player.GetDurationInSeconds();

			if (this.mForVideoSlide!=null)
			{
				mForVideoSlide.StartVideoOffset =this.mForVideoSlide.StartVideoOffset + (1/fps);

				if (mForVideoSlide.StartVideoOffset >= duration - mForVideoSlide.EndVideoOffset)
				{
					mForVideoSlide.StartVideoOffset -= (1/fps);
				}

				mVideoTrimStartFrameTrackBar.Value =(int)(mForVideoSlide.StartVideoOffset*fps);
			
				this.ReCalcTotalDurationLabel();
				this.ReDrawStartPictureBox(true);
				this.CommitChanges(true,false);
			}

		}

		// ******************************************************
		public void GetVideoTimeForwardOneSeoncdStartFrameButton(object o, System.EventArgs e)
		{
			double fps = GetVideoSlidefps();
			double duration = this.mForVideoSlide.Player.GetDurationInSeconds();

			if (this.mForVideoSlide!=null)
			{
				mForVideoSlide.StartVideoOffset =this.mForVideoSlide.StartVideoOffset + 1;

				if (mForVideoSlide.StartVideoOffset >= duration - mForVideoSlide.EndVideoOffset)
				{
					mForVideoSlide.StartVideoOffset = duration - mForVideoSlide.EndVideoOffset - (1/fps);
				}

				mVideoTrimStartFrameTrackBar.Value =(int)(mForVideoSlide.StartVideoOffset*fps);
			
				this.ReCalcTotalDurationLabel();
				this.ReDrawStartPictureBox(true);
				this.CommitChanges(true,false);
			}
		}


		// ******************************************************
		public void GetVideoTimeBackOneFrameEndFrameButton(object o, System.EventArgs e)
		{
			double fps =GetVideoSlidefps();
			double duration = this.mForVideoSlide.Player.GetDurationInSeconds();

			if (this.mForVideoSlide!=null)
			{
				mForVideoSlide.EndVideoOffset =this.mForVideoSlide.EndVideoOffset + (1/fps);

				if (duration - mForVideoSlide.EndVideoOffset <= mForVideoSlide.StartVideoOffset)
				{
					mForVideoSlide.EndVideoOffset -= (1/fps);
				}

				int r = ((int)((duration  - mForVideoSlide.EndVideoOffset)*fps))-1;
				if (r<=1) r = 1;
				mVideoTrimEndFrameTrackBar.Value = r;
			
				this.ReCalcTotalDurationLabel();
				this.ReDrawEndPictureBox(true);
				this.CommitChanges(false,true);
			}

		}

		// ******************************************************
		public void GetVideoTimeBackOneSeoncdEndFrameButton(object o, System.EventArgs e)
		{
			double fps = GetVideoSlidefps();
			double duration = this.mForVideoSlide.Player.GetDurationInSeconds();

			if (this.mForVideoSlide!=null)
			{
				mForVideoSlide.EndVideoOffset =this.mForVideoSlide.EndVideoOffset + 1;

				if (duration - mForVideoSlide.EndVideoOffset <= mForVideoSlide.StartVideoOffset)
				{
					mForVideoSlide.EndVideoOffset = (duration - mForVideoSlide.StartVideoOffset) - (1/fps);
				}

				int r= ((int)((duration  - mForVideoSlide.EndVideoOffset)*fps))-1;
				if (r<1) r=1;

				mVideoTrimEndFrameTrackBar.Value =r ;
			
				this.ReCalcTotalDurationLabel();
				this.ReDrawEndPictureBox(true);
				this.CommitChanges(false,true);
			}
		}

		// ******************************************************
		public void GetVideoTimeForwardOneFrameEndFrameButton(object o, System.EventArgs e)
		{
			double fps = GetVideoSlidefps();
			double duration = this.mForVideoSlide.Player.GetDurationInSeconds();

			if (this.mForVideoSlide!=null)
			{
				mForVideoSlide.EndVideoOffset =this.mForVideoSlide.EndVideoOffset - (1/fps);

				if (mForVideoSlide.EndVideoOffset <=0)
				{
					mForVideoSlide.EndVideoOffset = 0;
				}

				mVideoTrimEndFrameTrackBar.Value =((int)((duration  - mForVideoSlide.EndVideoOffset)*fps))-1;
			
				this.ReCalcTotalDurationLabel();
				this.ReDrawEndPictureBox(true);
				this.CommitChanges(false,true);
			}

		}

		// ******************************************************
		public void GetVideoTimeForwardOneSeoncdEndFrameButton(object o, System.EventArgs e)
		{
			double fps = GetVideoSlidefps();
			double duration = this.mForVideoSlide.Player.GetDurationInSeconds();

			if (this.mForVideoSlide!=null)
			{
				mForVideoSlide.EndVideoOffset =this.mForVideoSlide.EndVideoOffset - 1;

				if (mForVideoSlide.EndVideoOffset <=0)
				{
					mForVideoSlide.EndVideoOffset = 0;
				}

				mVideoTrimEndFrameTrackBar.Value =((int)((duration  - mForVideoSlide.EndVideoOffset)*fps))-1;
			
				this.ReCalcTotalDurationLabel();
				this.ReDrawEndPictureBox(true);
				this.CommitChanges(false,true);
			}
		}



		// ******************************************************
		public void Reset(object o, System.EventArgs e)
		{
			if (this.mForVideoSlide==null)
			{
				return ;
			}

			if (this.mForVideoSlide.EndVideoOffset==0 &&
				this.mForVideoSlide.StartVideoOffset==0 &&
				this.mForVideoSlide.Volume==1.0)
			{
				return ;
			}

			this.mForVideoSlide.EndVideoOffset=0;
			this.mForVideoSlide.StartVideoOffset=0;
			this.mForVideoSlide.Volume=1.0;
			SetWithVideoSlide(mForVideoSlide);
			CommitChanges(true,true);
		}


		// ******************************************************
		public void StartScrollChanged(object o,EventArgs e)
		{
			double fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

			if (this.mForVideoSlide!=null)
			{
				if (mVideoTrimStartFrameTrackBar.Value >= mVideoTrimEndFrameTrackBar.Value)
				{
					mVideoTrimStartFrameTrackBar.Value = mVideoTrimEndFrameTrackBar.Value-1;
				}

				mForVideoSlide.StartVideoOffset = ((double)mVideoTrimStartFrameTrackBar.Value) /fps;
				mForVideoSlide.SeekToTime(mForVideoSlide.StartVideoOffset,false);

				if (mStartTrackerTimer==null)
				{
					mStartTrackerTimer = new System.Windows.Forms.Timer();
					mStartTrackerTimer.Tick+=new EventHandler(StartTrackerTick);
					mStartTrackerTimer.Interval=100;
					mStartTrackerTimer.Start();
				}
			}
			ReCalcTotalDurationLabel();

		}

		// ******************************************************
		public void StartTrackerTick(object o,EventArgs e)
		{
			this.ReDrawStartPictureBox(false);
		}


		// ******************************************************
		public void MouseUpStartTrack(object o,MouseEventArgs e)
		{
			if (mStartTrackerTimer!=null)
			{
				lock(mStartTrackerTimer)
				{
					mStartTrackerTimer.Stop();
					mStartTrackerTimer=null;
					ReDrawStartPictureBox(true);
				}
			}
			CommitChanges(true,false);
		}


		// ******************************************************
		public void EndScrollChanged(object o,System.EventArgs e)
		{
			double fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

			if (this.mForVideoSlide!=null)
			{

				if (mVideoTrimEndFrameTrackBar.Value <= mVideoTrimStartFrameTrackBar.Value)
				{
					mVideoTrimEndFrameTrackBar.Value = mVideoTrimStartFrameTrackBar.Value+1;
				}

				mForVideoSlide.EndVideoOffset = mForVideoSlide.Player.GetDurationInSeconds()-(((double)mVideoTrimEndFrameTrackBar.Value) /fps)-(1/fps);

				double stt = (((double)mVideoTrimEndFrameTrackBar.Value) /fps);
				mForVideoSlide.SeekToTime(stt,false);
			
				if (mEndTrackerTimer==null)
				{
					mEndTrackerTimer = new System.Windows.Forms.Timer();
					mEndTrackerTimer.Tick+=new EventHandler(EndTrackerTick);
					mEndTrackerTimer.Interval=100;
					mEndTrackerTimer.Start();
				}
			}
			ReCalcTotalDurationLabel();
		}

		// ******************************************************
		public void MouseUpEndTrack(object o,MouseEventArgs e)
		{
			if (mEndTrackerTimer!=null)
			{
				lock(mEndTrackerTimer)
				{
					mEndTrackerTimer.Stop();
					mEndTrackerTimer=null;
					ReDrawEndPictureBox(true);
				}
			}
			CommitChanges(false,true);
		}

		// ******************************************************
		public void EndTrackerTick(object o,EventArgs e)
		{
			this.ReDrawEndPictureBox(false);
		}

		// ******************************************************
		public void ReDrawStartPictureBox(bool wait)
		{
            int frame = 0;
            mStartFrameTimeLabel.Text = CGlobals.TimeStringFromSeconds(mForVideoSlide.StartVideoOffset + 0.004);
            RenderToPictureBoxImage(wait, frame, mStartFramePictureBox);
		}

		// ******************************************************
		public void ReDrawEndPictureBox(bool wait)
		{
			double fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

			double length = mForVideoSlide.Player.GetDurationInSeconds();

			int frame = (int) ((mForVideoSlide.GetDurationInSeconds() *fps) - (1/fps));
            RenderToPictureBoxImage(wait, frame, mEndFramePictureBox);
			mEndFrameTimeLabel.Text = CGlobals.TimeStringFromSeconds((mForVideoSlide.Player.GetDurationInSeconds() - mForVideoSlide.EndVideoOffset- (1/fps))+0.004);
		}

		// ******************************************************
		private void CommitChanges(bool start_changed, bool end_changed)
		{

			Thumbnail rn = Form1.mMainForm.GetDecorationManager().SlideThumbnail;

			if (rn!=null)
			{
				rn.SlideLenghComboBox.Text = mForVideoSlide.GetDurationInSeconds().ToString();
				rn.ComboChangedSoInformStoryboardOfChange();	

				if (start_changed==true)
				{
					rn.InvalidateImage();
					Form1.mMainForm.GetDecorationManager().RePaint();
					Form1.mMainForm.GetSlideShowManager().RebuildTransitionBoxBeforeSlide(this.mForVideoSlide);
				}

				if (end_changed)
				{
					//Form1.mMainForm.GetSlideShowManager().RebuildTransitionBoxAfterSlide(this.mCurrentSlide);
				}
			}

			CGlobals.mCurrentProject.DeclareChange(true,"Video Trim Occurred");
		}


		// ******************************************************
		private void ReCalcTotalDurationLabel()
		{
			Form1.mMainForm.GetTotalTrimVideoLength().Text="Total duration: "+CGlobals.TimeStringFromSeconds(mForVideoSlide.GetDurationInSeconds()+0.004);
		}

		// ******************************************************
		public void RenderToPictureBoxImage(bool wait, int frame, PictureBox pictureBox)
		{
			lock (this_lock)
			{
				bool vs = CGlobals.VideoSeeking;
				bool wbsc = CGlobals.WaitVideoSeekCompletion;
				bool mute_sound = CGlobals.MuteSound ;

				int ww = this.mStartFramePictureBox.Width ;
                float rat = CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction();
				int hh = (int) (((float)ww)*rat);
		
				CGlobals.VideoSeeking=true;
				CGlobals.WaitVideoSeekCompletion = wait ;
				CGlobals.MuteSound = true;

                RenderVideoParameters rp = new RenderVideoParameters();
                rp.frame = frame;
                rp.req_width =ww;
                rp.req_height = hh;
                rp.ignore_decorations = true;
                rp.ignore_pan_zoom = true;
                rp.present_direct_to_window = true;
                rp.present_window = GraphicsEngine.Current.HiddenWindow.Handle;

                if (pictureBox.Image != null )
                {
                    pictureBox.Image.Dispose();
                }

                pictureBox.Image = mTrimSlideShow.RenderVideoToBitmap(rp);

                pictureBox.Height = hh;

				CGlobals.VideoSeeking = vs;
				CGlobals.WaitVideoSeekCompletion = wbsc;
				CGlobals.MuteSound = mute_sound;

				return ;
			}

		}

		// ******************************************************
		public double GetVideoSlidefps()
		{
			return CGlobals.mCurrentProject.DiskPreferences.frames_per_second;
		}

		// ******************************************************
		public void SetWithVideoSlide(CVideoSlide slide)
		{
			mForVideoSlide=slide;
            mTrimSlideShow = null;

            if (slide != null)
			{
                mTrimSlideShow = new CSlideShow("");
                mTrimSlideShow.FadeIn = false;
                mTrimSlideShow.FadeOut = false;
                ArrayList al = new ArrayList();
                al.Add(slide);
                mTrimSlideShow.AddSlides(al);
                mTrimSlideShow.CalcLengthOfAllSlides();

                ReDrawStartPictureBox(true);
                ReDrawEndPictureBox(true);

				double fps = GetVideoSlidefps();;

				mVideoTrimStartFrameTrackBar.Minimum=0;
				mVideoTrimStartFrameTrackBar.Maximum = (int)(slide.Player.GetDurationInSeconds()*fps)-1;
				mVideoTrimStartFrameTrackBar.Value = (int)(slide.StartVideoOffset*fps);
			
				mVideoTrimEndFrameTrackBar.Minimum=0;
				mVideoTrimEndFrameTrackBar.Maximum = ((int)(slide.Player.GetDurationInSeconds()*fps))-1;
				mVideoTrimEndFrameTrackBar.Value =((int)((slide.Player.GetDurationInSeconds()  - slide.EndVideoOffset)*fps))-1;
				ReCalcTotalDurationLabel();
				Form1.mMainForm.GetVideoVolumeTextBox().Text = ((int)((slide.Volume*100.0))).ToString() +"%";
				double val = this.mForVideoSlide.Volume;
				val = val * 100;
				int int_val = (int)val;
				if (int_val<0) int_val=0;
				if (int_val>100) int_val=100;
				Form1.mMainForm.GetVideoVolumeTrackBar().Value=int_val;

                mFadeBackgroundMusicTickBox.Checked = slide.EnableMusicFadeWhilePlayingVideo;
                mFadeBackgroundMusicTrackBar.Value = 100 - ((int)(slide.MusicFadeWhilePlayingVideo * 100));
                mFadeBackgroundMusicTextBox.Text = this.mFadeBackgroundMusicTrackBar.Value.ToString() + "%";
			}

		}

		// ******************************************************
		public void Done_click(object o, System.EventArgs e)
		{
			Form1.mMainForm.GetMainTabControl().SelectedIndex=0;
		}

		// ******************************************************
		private void VolumeScrolled(object o,System.EventArgs e)
		{
            Form1.mMainForm.GetVideoVolumeTextBox().Text = Form1.mMainForm.GetVideoVolumeTrackBar().Value + "%";
		}

        // ******************************************************
        private void VolumeMouseUp(object o, System.EventArgs e)
		{
			double vol = ((double)Form1.mMainForm.GetVideoVolumeTrackBar().Value)/100.0;
			if (vol<0) vol=0;
			if (vol>1) vol=1;
			this.mForVideoSlide.Volume = vol;
		}
        
        // ******************************************************
        private void FadeBackgroundMusicCheckBoxChanged(object o, System.EventArgs e)
        {
            if (mFadeBackgroundMusicTickBox.Checked == true)
            {
                this.mFadeBackgroundMusicTextBox.Enabled = true;
                this.mFadeBackgroundMusicTrackBar.Enabled = true;
            }
            else
            {
                this.mFadeBackgroundMusicTextBox.Enabled = false;
                this.mFadeBackgroundMusicTrackBar.Enabled = false;
            }

            if (mForVideoSlide != null)
            {
                if (mForVideoSlide.EnableMusicFadeWhilePlayingVideo != mFadeBackgroundMusicTickBox.Checked)
                {
                    mForVideoSlide.EnableMusicFadeWhilePlayingVideo = mFadeBackgroundMusicTickBox.Checked;
                }
            }
        }

        // ******************************************************
		private void FadeBackgroundMusicFadeScrolled(object o,System.EventArgs e)
		{
			mFadeBackgroundMusicTextBox.Text = this.mFadeBackgroundMusicTrackBar.Value.ToString() +"%";
		}

          // ******************************************************
		private void FadeBackgroundMusicMouseUp(object o,System.EventArgs e)
		{
            double value = (double)this.mFadeBackgroundMusicTrackBar.Value;
            value /=100.0;
            value = 1.0 - value;

            if (value < 0) value = 0;
            if (value > 1.0) value = 1.0;
            if (mForVideoSlide != null)
            {
                if (mForVideoSlide.MusicFadeWhilePlayingVideo != value)
                {
                    mForVideoSlide.MusicFadeWhilePlayingVideo = (float)value ;
                }
            }
        }
*/

	}
}
