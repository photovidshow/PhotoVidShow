using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;
using DVDSlideshow.GraphicsEng;
using System.Collections;


namespace CustomButton
{
    public partial class TrimVideoControl : UserControl
    {
        public delegate void ChangedCallbackDelegate(double previousLength);
        public delegate void ScrollStartedCallbackDelegate();

        public event ChangedCallbackDelegate Trimmed;
        public event ScrollStartedCallbackDelegate ScrollStarted; 

        private object this_lock = new object();
        private Timer mStartTrackerTimer = null;
        private Timer mEndTrackerTimer = null;

        private CVideoDecoration mForVideoDecoration;
        private CVideoDecoration mPreviewVideoDecoration;
        private CSlideShow mTrimSlideShow;
        private bool mUpdating;

        public TrimVideoControl()
        {
            InitializeComponent();

            mVideoTrimStartFrameTrackBar.MouseDown += new MouseEventHandler(StartFrameTrackBarMouseDown);
            mVideoTrimStartFrameTrackBar.Scroll += new EventHandler(this.StartScrollChanged);
            mVideoTrimStartFrameTrackBar.MouseUp += new MouseEventHandler(this.MouseUpStartTrack);
            mVideoTrimEndFrameTrackBar.MouseDown +=new MouseEventHandler(EndFrameTrackBarMouseDown);
            mVideoTrimEndFrameTrackBar.Scroll += new EventHandler(this.EndScrollChanged);
            mVideoTrimEndFrameTrackBar.MouseUp += new MouseEventHandler(this.MouseUpEndTrack);

            mFadeBackgroundMusicTickBox.CheckedChanged += new EventHandler(FadeBackgroundMusicCheckBoxChanged);
            mFadeBackgroundMusicTrackBar.Scroll += new EventHandler(this.FadeBackgroundMusicFadeScrolled);
            mFadeBackgroundMusicTrackBar.MouseUp += new MouseEventHandler(this.FadeBackgroundMusicMouseUp);

            mResetVideoTrimButton.Click += new EventHandler(this.Reset);

            mVideoTrimBackOneFrameStartFrameButton.Click += new EventHandler(GetVideoTimeBackOneFrameStartFrameButton);
            mVideoTrimBackOneSecondStartFrameButton.Click += new EventHandler(GetVideoTimeBackOneSeoncdStartFrameButton);
            mVideoTrimForwardOneFrameStartFrameButton.Click += new EventHandler(GetVideoTimeForwardOneFrameStartFrameButton);
            mVideoTrimForwardOneSecondStartFrameButton.Click += new EventHandler(GetVideoTimeForwardOneSeoncdStartFrameButton);


            mVideoTimeBackOneFrameEndFrameButton.Click += new EventHandler(GetVideoTimeBackOneFrameEndFrameButton);
            mVideoTimeBackOneSeoncdEndFrameButton.Click += new EventHandler(GetVideoTimeBackOneSeoncdEndFrameButton);
            mVideoTimeForwardOneFrameEndFrameButton.Click += new EventHandler(GetVideoTimeForwardOneFrameEndFrameButton);
            mVideoTimeForwardOneSeoncdEndFrameButton.Click += new EventHandler(GetVideoTimeForwardOneSeoncdEndFrameButton);

            mVideoVolumeTrackBar.Scroll += new EventHandler(this.VolumeScrolled);
            mVideoVolumeTrackBar.MouseUp += new MouseEventHandler(this.VolumeMouseUp);

            mFadeAudioInTrackBar.Scroll += new EventHandler(mFadeAudioInTrackBarScrolled);
            mFadeAudioOutTrackBar.Scroll += new EventHandler(mFadeAudioOutTrackBarScrolled);

            /*
            ToolTip tt = Form1.mMainForm.mToolTip;

            tt.SetToolTip(Form1.mMainForm.GetVideoTimeBackOneFrameEndFrameButton(), @"Back 1 frame");
            tt.SetToolTip(Form1.mMainForm.GetVideoTimeBackOneSeoncdEndFrameButton(), @"Back 1 second");
            tt.SetToolTip(Form1.mMainForm.GetVideoTimeForwardOneFrameEndFrameButton(), @"Forward 1 frame");
            tt.SetToolTip(Form1.mMainForm.GetVideoTimeForwardOneSeoncdEndFrameButton(), @"Forward 1 second");

            tt.SetToolTip(Form1.mMainForm.GetVideoTrimBackOneFrameStartFrameButton(), @"Back 1 frame");
            tt.SetToolTip(Form1.mMainForm.GetVideoTrimBackOneSecondStartFrameButton(), @"Back 1 second");
            tt.SetToolTip(Form1.mMainForm.GetVideoTrimForwardOneFrameStartFrameButton(), @"Forward 1 frame");
            tt.SetToolTip(Form1.mMainForm.GetVideoTrimForwardOneSecondStartFrameButton(), @"Forward 1 second");
            tt.SetToolTip(Form1.mMainForm.GetVideoVolumeTrackBar(), @"Set video volume");
            tt.SetToolTip(Form1.mMainForm.GetResetVideoTrimButton(), @"Reset video settings");
            tt.SetToolTip(Form1.mMainForm.GetTrimVideoDoneButton(), @"Finish trimming video");
            
             */

        }


        // ******************************************************
        private void UpdateGuiFromStartButtonTrim(double fps)
        {
            mVideoTrimStartFrameTrackBar.Value = (int)(mPreviewVideoDecoration.StartVideoOffset * fps);

            ReCalcTotalDurationLabel();
            ReDrawStartPictureBox(true);
            this.CommitChanges(true, false);
        }

        // ******************************************************
        private void UpdateGuiFromEndButtonTrim(double duration, double fps)
        {
            int r = ((int)((duration - mPreviewVideoDecoration.EndVideoOffset) * fps)) - 1;
            if (r < 1) r = 1;
            mVideoTrimEndFrameTrackBar.Value = r;

            ReCalcTotalDurationLabel();
            ReDrawEndPictureBox(true);
            this.CommitChanges(true, false);
        }


        // ******************************************************
        private void GetVideoTimeBackOneFrameStartFrameButton(object o, System.EventArgs e)
        {
            double fps = GetVideoSlidefps();
            double sub = 1 / fps;

            if (this.mPreviewVideoDecoration != null)
            {
                mPreviewVideoDecoration.StartVideoOffset = this.mPreviewVideoDecoration.StartVideoOffset - sub;

                if (mPreviewVideoDecoration.StartVideoOffset < 0)
                {
                    mPreviewVideoDecoration.StartVideoOffset = 0;
                }

                UpdateGuiFromStartButtonTrim(fps);
            }

        }

        // ******************************************************
        private void GetVideoTimeBackOneSeoncdStartFrameButton(object o, System.EventArgs e)
        {
            double fps = GetVideoSlidefps();
        
            if (this.mPreviewVideoDecoration != null)
            {
                mPreviewVideoDecoration.StartVideoOffset = this.mPreviewVideoDecoration.StartVideoOffset - 1;

                if (mPreviewVideoDecoration.StartVideoOffset < 0)
                {
                    mPreviewVideoDecoration.StartVideoOffset = 0;
                }

                UpdateGuiFromStartButtonTrim(fps);
            }
        }

        // ******************************************************
        private void GetVideoTimeForwardOneFrameStartFrameButton(object o, System.EventArgs e)
        {
            double fps = GetVideoSlidefps();
            double duration = this.mPreviewVideoDecoration.GetOriginalVideoDurationInSeconds();

            if (this.mPreviewVideoDecoration != null)
            {
                mPreviewVideoDecoration.StartVideoOffset = this.mPreviewVideoDecoration.StartVideoOffset + (1 / fps);

                if (mPreviewVideoDecoration.StartVideoOffset >= duration - mPreviewVideoDecoration.EndVideoOffset)
                {
                    mPreviewVideoDecoration.StartVideoOffset -= (1 / fps);
                }

                UpdateGuiFromStartButtonTrim(fps);
            }

        }

        // ******************************************************
        private void GetVideoTimeForwardOneSeoncdStartFrameButton(object o, System.EventArgs e)
        {
            double fps = GetVideoSlidefps();
            double duration = this.mPreviewVideoDecoration.GetOriginalVideoDurationInSeconds();

            if (this.mPreviewVideoDecoration != null)
            {
                mPreviewVideoDecoration.StartVideoOffset = this.mPreviewVideoDecoration.StartVideoOffset + 1;

                if (mPreviewVideoDecoration.StartVideoOffset >= duration - mPreviewVideoDecoration.EndVideoOffset)
                {
                    mPreviewVideoDecoration.StartVideoOffset = duration - mPreviewVideoDecoration.EndVideoOffset - (1 / fps);
                }

                UpdateGuiFromStartButtonTrim(fps);
            }
        }


        // ******************************************************
        private void GetVideoTimeBackOneFrameEndFrameButton(object o, System.EventArgs e)
        {
            double fps = GetVideoSlidefps();
            double duration = this.mPreviewVideoDecoration.GetOriginalVideoDurationInSeconds();

            if (this.mPreviewVideoDecoration != null)
            {
                mPreviewVideoDecoration.EndVideoOffset = this.mPreviewVideoDecoration.EndVideoOffset + (1 / fps);

                if (duration - mPreviewVideoDecoration.EndVideoOffset <= mPreviewVideoDecoration.StartVideoOffset)
                {
                    mPreviewVideoDecoration.EndVideoOffset -= (1 / fps);
                }
                UpdateGuiFromEndButtonTrim(duration, fps);
            }

        }

        // ******************************************************
        private void GetVideoTimeBackOneSeoncdEndFrameButton(object o, System.EventArgs e)
        {
            double fps = GetVideoSlidefps();
            double duration = this.mPreviewVideoDecoration.GetOriginalVideoDurationInSeconds();

            if (this.mPreviewVideoDecoration != null)
            {
                mPreviewVideoDecoration.EndVideoOffset = this.mPreviewVideoDecoration.EndVideoOffset + 1;

                if (duration - mPreviewVideoDecoration.EndVideoOffset <= mPreviewVideoDecoration.StartVideoOffset)
                {
                    mPreviewVideoDecoration.EndVideoOffset = (duration - mPreviewVideoDecoration.StartVideoOffset) - (1 / fps);
                }

                UpdateGuiFromEndButtonTrim(duration, fps);
            }
        }

        // ******************************************************
        private void GetVideoTimeForwardOneFrameEndFrameButton(object o, System.EventArgs e)
        {
            double fps = GetVideoSlidefps();
            double duration = this.mPreviewVideoDecoration.GetOriginalVideoDurationInSeconds();

            if (this.mPreviewVideoDecoration != null)
            {
                mPreviewVideoDecoration.EndVideoOffset = this.mPreviewVideoDecoration.EndVideoOffset - (1 / fps);

                if (mPreviewVideoDecoration.EndVideoOffset <= 0)
                {
                    mPreviewVideoDecoration.EndVideoOffset = 0;
                }

                UpdateGuiFromEndButtonTrim(duration, fps);
            }

        }

        // ******************************************************
        private void GetVideoTimeForwardOneSeoncdEndFrameButton(object o, System.EventArgs e)
        {
            double fps = GetVideoSlidefps();
            double duration = this.mPreviewVideoDecoration.GetOriginalVideoDurationInSeconds();

            if (this.mPreviewVideoDecoration != null)
            {
                mPreviewVideoDecoration.EndVideoOffset = this.mPreviewVideoDecoration.EndVideoOffset - 1;

                if (mPreviewVideoDecoration.EndVideoOffset <= 0)
                {
                    mPreviewVideoDecoration.EndVideoOffset = 0;
                }

                UpdateGuiFromEndButtonTrim(duration, fps);
            }
        }



        // ******************************************************
        public void Reset(object o, System.EventArgs e)
        {
            if (mForVideoDecoration == null || mPreviewVideoDecoration == null)
            {
                return;
            }

            //
            // If already the default values then return now
            //
            if (mForVideoDecoration.EndVideoOffset == 0 &&
                mForVideoDecoration.StartVideoOffset == 0 &&
                mForVideoDecoration.FadeAudioIn == false &&
                mForVideoDecoration.FadeAudioInLength == 1.0f &&
                mForVideoDecoration.FadeAudioOut == false &&
                mForVideoDecoration.FadeAudioOutLength == 1.0f &&
                mForVideoDecoration.Volume == 1.0 &&
                Math.Abs(mForVideoDecoration.MusicFadeWhilePlayingVideo - 0.5f) < 0.0001 &&
                mForVideoDecoration.EnableMusicFadeWhilePlayingVideo == false) 
            {
                return;
            }

            double previousLength = mForVideoDecoration.GetTrimmedVideoDurationInSeconds();

            mForVideoDecoration.EndVideoOffset = 0;
            mForVideoDecoration.StartVideoOffset = 0;
            mForVideoDecoration.Volume = 1.0;
            mForVideoDecoration.EnableMusicFadeWhilePlayingVideo = false;
            mForVideoDecoration.MusicFadeWhilePlayingVideo = 0.5f;
            mForVideoDecoration.FadeAudioIn = false;
            mForVideoDecoration.FadeAudioInLength = 1.0f;
            mForVideoDecoration.FadeAudioOut = false;
            mForVideoDecoration.FadeAudioOutLength = 1.0f;

            //
            // Re-create preview decoration
            //
            SetOrUpdateForDecorationInternal(mForVideoDecoration, mVideoVolumeTrackBar.Enabled, true);

            //
            // Fire commit changes event to observers
            //
            if (Trimmed != null)
            {
                Trimmed(previousLength);
            }
        }

        // ******************************************************
        private void StartScrollChanged(object o, EventArgs e)
        {
           
            double fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

            if (this.mPreviewVideoDecoration != null)
            {
                if (mVideoTrimStartFrameTrackBar.Value >= mVideoTrimEndFrameTrackBar.Value)
                {
                    mVideoTrimStartFrameTrackBar.Value = mVideoTrimEndFrameTrackBar.Value - 1;
                }

                mPreviewVideoDecoration.StartVideoOffset = ((double)mVideoTrimStartFrameTrackBar.Value) / fps;
                mPreviewVideoDecoration.SeekToTime(mPreviewVideoDecoration.StartVideoOffset, false);

                if (mStartTrackerTimer == null)
                {
                    mStartTrackerTimer = new System.Windows.Forms.Timer();
                    mStartTrackerTimer.Tick += new EventHandler(StartTrackerTick);
                    mStartTrackerTimer.Interval = 100;
                    mStartTrackerTimer.Start();
                }
            }
            ReCalcTotalDurationLabel();

        }

        // ******************************************************
        private void StartTrackerTick(object o, EventArgs e)
        {
            this.ReDrawStartPictureBox(false);
        }

        // ******************************************************
        private void StartFrameTrackBarMouseDown(object o, MouseEventArgs e)
        {
            if (ScrollStarted != null)
            {
                ScrollStarted();
            }
        }

        // ******************************************************
        private void MouseUpStartTrack(object o, MouseEventArgs e)
        {

            if (mStartTrackerTimer != null)
            {
                lock (mStartTrackerTimer)
                {
                    mStartTrackerTimer.Stop();
                    mStartTrackerTimer = null;
                    ReDrawStartPictureBox(true);
                }
            }
            CommitChanges(true, false);
        }


        // ******************************************************
        private void EndScrollChanged(object o, System.EventArgs e)
        {
            double fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

            if (this.mPreviewVideoDecoration != null)
            {

                if (mVideoTrimEndFrameTrackBar.Value <= mVideoTrimStartFrameTrackBar.Value)
                {
                    mVideoTrimEndFrameTrackBar.Value = mVideoTrimStartFrameTrackBar.Value + 1;
                }

                mPreviewVideoDecoration.EndVideoOffset = mPreviewVideoDecoration.GetOriginalVideoDurationInSeconds() - (((double)mVideoTrimEndFrameTrackBar.Value) / fps) - (1 / fps);

                double stt = (((double)mVideoTrimEndFrameTrackBar.Value) / fps);
                mPreviewVideoDecoration.SeekToTime(stt, false);

                if (mEndTrackerTimer == null)
                {
                    mEndTrackerTimer = new System.Windows.Forms.Timer();
                    mEndTrackerTimer.Tick += new EventHandler(EndTrackerTick);
                    mEndTrackerTimer.Interval = 100;
                    mEndTrackerTimer.Start();
                }
            }
            ReCalcTotalDurationLabel();
        }

        // ******************************************************
        private void EndFrameTrackBarMouseDown(object o, MouseEventArgs e)
        {
            if (ScrollStarted != null)
            {
                ScrollStarted();
            }
        }
        
        // ******************************************************
        private void MouseUpEndTrack(object o, MouseEventArgs e)
        {
            if (mEndTrackerTimer != null)
            {
                lock (mEndTrackerTimer)
                {
                    mEndTrackerTimer.Stop();
                    mEndTrackerTimer = null;
                    ReDrawEndPictureBox(true);
                }
            }
            CommitChanges(false, true);
        }

        // ******************************************************
        private void EndTrackerTick(object o, EventArgs e)
        {
            this.ReDrawEndPictureBox(false);
        }

        // ******************************************************
        private void ReDrawStartPictureBox(bool wait)
        {
            int frame = 0;
            mStartFrameTimeLabel.Text = CGlobals.TimeStringFromSeconds(mPreviewVideoDecoration.StartVideoOffset + 0.004);
            RenderToPictureBoxImage(wait, frame, mStartFramePictureBox);
        }

        // ******************************************************
        private void ReDrawEndPictureBox(bool wait)
        {
            double fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

            int frame = (int)((mPreviewVideoDecoration.GetTrimmedVideoDurationInSeconds() * fps) - (1 / fps));
            RenderToPictureBoxImage(wait, frame, mEndFramePictureBox);
            mEndFrameTimeLabel.Text = CGlobals.TimeStringFromSeconds((mPreviewVideoDecoration.GetOriginalVideoDurationInSeconds() - mPreviewVideoDecoration.EndVideoOffset - (1 / fps)) + 0.004);
        }

        // ******************************************************
        private void CommitChanges(bool start_changed, bool end_changed)
        {
            double previousLength = mForVideoDecoration.GetTrimmedVideoDurationInSeconds();

            mForVideoDecoration.StartVideoOffset = mPreviewVideoDecoration.StartVideoOffset;
            mForVideoDecoration.EndVideoOffset = mPreviewVideoDecoration.EndVideoOffset;

            if (Trimmed != null)
            {
                Trimmed(previousLength);

            }
            CGlobals.mCurrentProject.DeclareChange(true, "Video Trim Occurred");        
        }


        // ******************************************************
        private void ReCalcTotalDurationLabel()
        {
            mTotalTrimVideoLength.Text = "Total duration: " + CGlobals.TimeStringFromSeconds(mPreviewVideoDecoration.GetTrimmedVideoDurationInSeconds() + 0.004);
        }

        // ******************************************************
        public Image RenderToImage(bool wait, int frame, int ww, int hh, Bitmap renderToBitmap)
        {
         //   Console.WriteLine("RENDER TO IMAGE " + wait.ToString() + " " + frame.ToString() + " "+ ww.ToString() + " " + hh.ToString());
            lock (this_lock)
            {
                bool vs = CGlobals.VideoSeeking;
                bool wbsc = CGlobals.WaitVideoSeekCompletion;
                bool mute_sound = CGlobals.MuteSound;

                CGlobals.VideoSeeking = true;
                CGlobals.WaitVideoSeekCompletion = wait;
                CGlobals.MuteSound = true;

                if (ww < 1) ww = 1;
                if (hh < 1) hh = 1;

                RenderVideoParameters rp = new RenderVideoParameters();
                rp.frame = frame;
                rp.req_width = ww;
                rp.req_height = hh;
                rp.ignore_decorations = true;
                rp.ignore_pan_zoom = true;
                rp.present_direct_to_window = true;
                rp.present_window = GraphicsEngine.Current.HiddenWindow.Handle;
                rp.renderToBitmap = renderToBitmap;

                Bitmap b = mTrimSlideShow.RenderVideoToBitmap(rp);

                CGlobals.VideoSeeking = vs;
                CGlobals.WaitVideoSeekCompletion = wbsc;
                CGlobals.MuteSound = mute_sound;
                return b;
            }
        }


        // ******************************************************
        private void RenderToPictureBoxImage(bool wait, int frame, PictureBox pictureBox)
        {
            int ww = this.mStartFramePictureBox.Width;
            float rat = CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction();
            int hh = (int)(((float)ww) * rat);

            Bitmap oldBitmap = pictureBox.Image as Bitmap;
            Image image = RenderToImage(wait, frame, ww, hh, oldBitmap);

            pictureBox.Image = image;

            if (oldBitmap != null && oldBitmap != image)
            {
                oldBitmap.Dispose();
            }

            pictureBox.Height = hh;
        }

        // ******************************************************
        private double GetVideoSlidefps()
        {
            return CGlobals.mCurrentProject.DiskPreferences.frames_per_second;
        }

        // ******************************************************
        public void SetForDecoration(CVideoDecoration decoration, CSlide parentSlide)
        {
            CVideoDecoration decorAllowedAudio = CMusicPerformance.GetAudioVideoDecorationForSlide(parentSlide);
            bool audioSettingsAllowed = true;

            if (decorAllowedAudio != decoration)
            {
                audioSettingsAllowed = false;
            }

            SetForDecoration(decoration, audioSettingsAllowed, true);
        }

        // ******************************************************
        public void SetForDecoration(CVideoDecoration decoration, bool audioSettingsAllowed, bool drawPictureBoxes)
        {
            if (mForVideoDecoration == decoration)
            {
                return;
            }

            SetOrUpdateForDecorationInternal(decoration, audioSettingsAllowed, drawPictureBoxes);
        }

        // ******************************************************
        private void SetOrUpdateForDecorationInternal(CVideoDecoration decoration, bool audioSettingsAllowed, bool drawPictureBoxes)
        {
            mUpdating = true;
            try
            {
                mForVideoDecoration = decoration;

                CVideoDecoration newvd = new CVideoDecoration(decoration.Player);
                newvd.StartVideoOffset = decoration.StartVideoOffset;
                newvd.EndVideoOffset = decoration.EndVideoOffset;
                newvd.XFlipped = decoration.XFlipped;
                newvd.YFlipped = decoration.YFlipped;
                newvd.Orientation = decoration.Orientation;

                mPreviewVideoDecoration = newvd;
                mTrimSlideShow = null;

                if (decoration != null)
                {
                    mTrimSlideShow = new CSlideShow("");
                    mTrimSlideShow.FadeIn = false;
                    mTrimSlideShow.FadeOut = false;

                    CBlankStillPictureSlide bs = new CBlankStillPictureSlide(mPreviewVideoDecoration);

                    mTrimSlideShow.mSlides.Add(bs);
                    mTrimSlideShow.CalcLengthOfAllSlides();

                    if (drawPictureBoxes == true)
                    {
                        ReDrawEndPictureBox(true);  // draw end frame first
                        ReDrawStartPictureBox(true);                  
                    }

                    double fps = GetVideoSlidefps();

                    mVideoTrimStartFrameTrackBar.Minimum = 0;
                    mVideoTrimStartFrameTrackBar.Maximum = (int)(decoration.GetOriginalVideoDurationInSeconds() * fps) - 1;
                    mVideoTrimStartFrameTrackBar.Value = (int)(decoration.StartVideoOffset * fps);

                    mVideoTrimEndFrameTrackBar.Minimum = 0;
                    mVideoTrimEndFrameTrackBar.Maximum = ((int)(decoration.GetOriginalVideoDurationInSeconds() * fps)) - 1;

                    int newTrimEndValue = ((int)((decoration.GetOriginalVideoDurationInSeconds() - decoration.EndVideoOffset) * fps)) - 1;
                    if (newTrimEndValue < 0) newTrimEndValue = 0;
                    if (newTrimEndValue > mVideoTrimEndFrameTrackBar.Maximum) newTrimEndValue = mVideoTrimEndFrameTrackBar.Maximum;

                    mVideoTrimEndFrameTrackBar.Value = newTrimEndValue;
                    ReCalcTotalDurationLabel();

                    mVideoVolumeTextBox.Text = ((int)((mForVideoDecoration.Volume * 100.0))).ToString() + "%";
                    double val = this.mForVideoDecoration.Volume;
                    val = val * 100;
                    int int_val = (int)val;
                    if (int_val < 0) int_val = 0;
                    if (int_val > 100) int_val = 100;
                    mVideoVolumeTrackBar.Value = int_val;

                    mFadeBackgroundMusicTickBox.Checked = mForVideoDecoration.EnableMusicFadeWhilePlayingVideo;
                    mFadeBackgroundMusicTrackBar.Value = 100 - ((int)(mForVideoDecoration.MusicFadeWhilePlayingVideo * 100));
                    mFadeBackgroundMusicTextBox.Text = this.mFadeBackgroundMusicTrackBar.Value.ToString() + "%";

                    if (audioSettingsAllowed == false)
                    {
                        mVideoVolumeTextBox.Enabled = false;
                        mVideoVolumeTrackBar.Enabled = false;
                        mFadeBackgroundMusicTickBox.Enabled = false;
                        mFadeBackgroundMusicTrackBar.Enabled = false;
                        mFadeBackgroundMusicTextBox.Enabled = false;
                    }
                    else
                    {
                        mVideoVolumeTextBox.Enabled = true;
                        mVideoVolumeTrackBar.Enabled = true;
                        mFadeBackgroundMusicTickBox.Enabled = true;
                        mFadeBackgroundMusicTrackBar.Enabled = mFadeBackgroundMusicTickBox.Checked == true;
                        mFadeBackgroundMusicTextBox.Enabled = mFadeBackgroundMusicTickBox.Checked == true;
                    }

                    mFadeAudioInCheckBox.Checked = mForVideoDecoration.FadeAudioIn;            
                    int trackBarValue = (int)(mForVideoDecoration.FadeAudioInLength * 100.0f);
                    trackBarValue = CGlobals.Clamp(trackBarValue, 0, 500);
                    mFadeAudioInTrackBar.Value = trackBarValue;
                    UpdateFadeAudioInTextBox();

                    mFadeAudioOutCheckBox.Checked = mForVideoDecoration.FadeAudioOut;
                    trackBarValue = (int)(mForVideoDecoration.FadeAudioOutLength * 100.0f);
                    trackBarValue = CGlobals.Clamp(trackBarValue, 0, 500);
                    mFadeAudioOutTrackBar.Value = trackBarValue;
                    UpdateFadeAudioOutTextBox();
                    
                }
            }
            finally
            {
                mUpdating = false;
            }
        }


        // ******************************************************
        private void VolumeScrolled(object o, System.EventArgs e)
        {
            mVideoVolumeTextBox.Text = mVideoVolumeTrackBar.Value + "%";
        }

        // ******************************************************
        private void VolumeMouseUp(object o, System.EventArgs e)
        {
            double vol = ((double)mVideoVolumeTrackBar.Value) / 100.0;
            if (vol < 0) vol = 0;
            if (vol > 1) vol = 1;
            this.mForVideoDecoration.Volume = vol;
        }

        // ******************************************************
        private void FadeBackgroundMusicCheckBoxChanged(object o, System.EventArgs e)
        {
            if (mUpdating==true)
            {
                return;
            }

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

            if (mPreviewVideoDecoration != null)
            {
                if (mForVideoDecoration.EnableMusicFadeWhilePlayingVideo != mFadeBackgroundMusicTickBox.Checked)
                {
                    mForVideoDecoration.EnableMusicFadeWhilePlayingVideo = mFadeBackgroundMusicTickBox.Checked;
                }
            }
        }

        // ******************************************************
        private void FadeBackgroundMusicFadeScrolled(object o, System.EventArgs e)
        {
            mFadeBackgroundMusicTextBox.Text = this.mFadeBackgroundMusicTrackBar.Value.ToString() + "%";
        }

        // ******************************************************
        private void FadeBackgroundMusicMouseUp(object o, System.EventArgs e)
        {
            double value = (double)this.mFadeBackgroundMusicTrackBar.Value;
            value /= 100.0;
            value = 1.0 - value;

            if (value < 0) value = 0;
            if (value > 1.0) value = 1.0;
            if (mPreviewVideoDecoration != null)
            {
                if (mForVideoDecoration.MusicFadeWhilePlayingVideo != value)
                {
                    mForVideoDecoration.MusicFadeWhilePlayingVideo = (float)value;
                }
            }
        }

        // ******************************************************
        private void mFadeAudioInTrackBarScrolled(object o, System.EventArgs e)
        {
            float value = (float)this.mFadeAudioInTrackBar.Value;
            value /= 100.0f;
            value = CGlobals.ClampF(value, 0, 5);

            mForVideoDecoration.FadeAudioInLength = value;

            UpdateFadeAudioInTextBox();
        }

        // ******************************************************
        private void UpdateFadeAudioInTextBox()
        {
            mFadeAudioInTextBox.Text = string.Format("{0:0.0}", mForVideoDecoration.FadeAudioInLength) + "s";
        }
        
        // ******************************************************
        private void mFadeAudioInCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mFadeAudioInCheckBox.Checked== true)
            {
                mFadeAudioInTrackBar.Enabled = true;
                mFadeAudioInTextBox.Enabled = true;
                if (mUpdating == false)
                {
                    mForVideoDecoration.FadeAudioIn = true;
                }
            }
            else
            {
                mFadeAudioInTrackBar.Enabled = false;
                mFadeAudioInTextBox.Enabled = false;

                if (mUpdating == false)
                {
                    mForVideoDecoration.FadeAudioIn = false;
                }
            }
        }

        // ******************************************************
        private void mFadeAudioOutTrackBarScrolled(object o, System.EventArgs e)
        {
            float value = (float)this.mFadeAudioOutTrackBar.Value;
            value /= 100.0f;
            value = CGlobals.ClampF(value, 0, 5);

            mForVideoDecoration.FadeAudioOutLength = value;

            UpdateFadeAudioOutTextBox();
        }
            

        // ******************************************************
        private void UpdateFadeAudioOutTextBox()
        {
            mFadeAudioOutTextBox.Text = string.Format("{0:0.0}", mForVideoDecoration.FadeAudioOutLength) + "s";
        }

        // ******************************************************
        private void mFadeAudioOutCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mFadeAudioOutCheckBox.Checked == true)
            {
                mFadeAudioOutTrackBar.Enabled = true;
                mFadeAudioOutTextBox.Enabled = true;

                if (mUpdating==false)
                {
                    mForVideoDecoration.FadeAudioOut = true;
                }
            }
            else
            {
                mFadeAudioOutTrackBar.Enabled = false;
                mFadeAudioOutTextBox.Enabled = false;

                if (mUpdating == false)
                {
                    mForVideoDecoration.FadeAudioOut = false;
                }
            }
        }


        // ******************************************************
        private void mVideoPropertiesButton_Click(object sender, EventArgs e)
        {
            if (mForVideoDecoration != null)
            {
                VideoPropertiesForm vpf = new VideoPropertiesForm(mForVideoDecoration);
                vpf.ShowDialog();
            }
        }
    }
}
