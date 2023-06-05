using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DVDSlideshow;
using System.Globalization;
using System.Collections.Generic;

namespace dvdslideshowfontend
{
	/// <summary>
	/// Summary description for TrimMusicWindow.
	/// </summary>
	public class TrimMusicWindow : System.Windows.Forms.Form
	{
        private class MusicSlideListViewItem : ListViewItem
        {
            public CMusicSlide mForMusicSlide;
            public MusicSlideListViewItem(string id, CMusicSlide ms)
                : base(id)
            {
                mForMusicSlide = ms;
            }
        }

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button mDoneButton;
		private System.Windows.Forms.TrackBar mEndTimeTrackBar;
		private System.Windows.Forms.TrackBar mStartTimeTrackBar;
		private System.Windows.Forms.Button mResetButton;
		private System.Windows.Forms.Label mDurationLabel;
		private System.Windows.Forms.Label mStartTimeLabel;
		private System.Windows.Forms.Label mEndTimeLabel;
		private System.Windows.Forms.Button mStartTimeBack1SecondButton;
		private System.Windows.Forms.Button mStartTimeBack1FrameButton;
		private System.Windows.Forms.Button mStartTimeForward1FrameButton;
		private System.Windows.Forms.Button mStartTimeForward1SecondButton;
		private BitmapButton.SimpleBitmapButton mStopButton;
		private BitmapButton.SimpleBitmapButton mPlayButton;
		private System.Windows.Forms.Button mEndTimeForward1SecondButton;
		private System.Windows.Forms.Button mEndTimeForward1FrameButton;
		private System.Windows.Forms.Button mEndTimeBack1FrameButton;
		private System.Windows.Forms.Button mEndTimeBack1SecondButton;
		private System.Windows.Forms.CheckBox mFadeInCheckBox;
		private System.Windows.Forms.CheckBox mFadeOutCheckBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		public CMusicSlide mForMusicSlide;
		private System.Windows.Forms.PictureBox mPlaybackPointPictureBox;

		private System.Windows.Forms.Timer mPlaybackTimer = null;
		private System.Windows.Forms.Label mPlaybackTimerLabel;

		private bool mPlaying = false;
		private bool mMouseDownMarketPictureBox = false;
		private int mPosMarker = MarketXOffset;
		private object this_lock = new object();
		private System.Windows.Forms.TrackBar mMusicVolmeTrackBar;
		private System.Windows.Forms.Label mVolumeLabel;
		private System.Windows.Forms.ComboBox mStartGapCombo;
		private System.Windows.Forms.Label mInitalGapLabel;
		private System.Windows.Forms.TextBox mVolumeTextBox;
		private System.Windows.Forms.Label mSecondsLabel;
		private System.Windows.Forms.GroupBox mTopGrouo;
		private System.Windows.Forms.GroupBox mGroupBox2;
		private System.Windows.Forms.Label mInv;
		private bool mPausedAtEnd=false;

		private static int MarketXOffset = 13;
        private ListView mAudioListView;
        private Button mAddButton;
        private Button mRemoveButton;
        private Button mMoveUpOrderButton;
        private Button mMoveDownOrderButton;
        private ColumnHeader mNameColumn;
        private ColumnHeader mStartTimeColumn;
        private ColumnHeader mEndTimeColumn;
        private ColumnHeader mLengthColumn;
		private double mCurrentPlaybackVol = 1.0f;
        private CSlideShow mForSlideshow;
        private bool mForNarration = false;
        private TextBox mFadeOutLengthTextBox;
        private TextBox mFadeInLengthTextBox;
        private TrackBar mFadeOutLengthTrackBar;
        private TrackBar mFadeInLengthTrackBar;
        private Button mSlideshowAudioSettings;
        private List<CMusicSlide> mSlides = new List<CMusicSlide>();

        //*******************************************************************
		public TrimMusicWindow(MusicThumbnail for_thumbnail, CSlideShow forSlideshow)
		{
            mForSlideshow = forSlideshow;

            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            SuspendLayout();
            for (float i = 0; i <= 40; i += 0.5f)
            {
                string st = String.Format("{0:f1}", i);
                mStartGapCombo.Items.Add(st);
            }
            ResumeLayout();

            if (for_thumbnail.AudioThumbnailType == MusicThumbnail.AudioType.MUSIC)
            {
                this.Text = "Trim music";
            }
            else
            {
 
                this.Text = "Trim narration";
                mForNarration = true;
                mMoveUpOrderButton.Enabled = false;
                mMoveDownOrderButton.Enabled = false;

                // temporary disable list of narrations options
                this.Bounds = new Rectangle(Bounds.X, Bounds.Y, this.Bounds.Width, this.Bounds.Height - 176);
       
                mAudioListView.Visible = false;
                mAddButton.Visible = false;
                mRemoveButton.Visible = false;
                mMoveUpOrderButton.Visible = false;
                mMoveDownOrderButton.Visible = false;
                mResetButton.Visible = false;
                mSlideshowAudioSettings.Location = new Point(mSlideshowAudioSettings.Location.X - 105, mSlideshowAudioSettings.Location.Y);

                mForMusicSlide = for_thumbnail.mMusicSlide;
                InitForMusicSlide(mForMusicSlide);
            }


            this.mStartTimeTrackBar.Scroll += new EventHandler(this.StartTrackerScroll);
            this.mStartTimeTrackBar.MouseUp += new MouseEventHandler(StartTrackerScroll_MouseUp);
            this.mEndTimeTrackBar.Scroll += new EventHandler(this.EndTrackerScroll);
            this.mEndTimeTrackBar.MouseUp += new MouseEventHandler(EndTrackerScroll_MouseUp);
            this.mPlaybackPointPictureBox.MouseDown += new MouseEventHandler(this.MouseDownMarketPictureBox);
            this.mPlaybackPointPictureBox.MouseUp += new MouseEventHandler(this.MouseUpMarketPictureBox);
            this.mPlaybackPointPictureBox.MouseMove += new MouseEventHandler(this.MouseMoveMarketPictureBox);


            ToolTip tt = new ToolTip();
            //	Form1.mMainForm.mToolTip;
            tt.ShowAlways = true;


            tt.SetToolTip(this.mDoneButton, @"Close window");
            tt.SetToolTip(this.mResetButton, @"Reset trim settings");
            tt.SetToolTip(this.mPlayButton, @"Play");
            tt.SetToolTip(this.mStopButton, @"Stop");
            tt.SetToolTip(this.mStartTimeTrackBar, @"Start time");
            tt.SetToolTip(this.mEndTimeTrackBar, @"End time");

            tt.SetToolTip(this.mStartTimeBack1FrameButton, @"Back 1 frame");
            tt.SetToolTip(this.mStartTimeBack1SecondButton, @"Back 1 second");
            tt.SetToolTip(this.mStartTimeForward1SecondButton, @"Forward 1 second");
            tt.SetToolTip(this.mStartTimeForward1FrameButton, @"Forward 1 frame");

            tt.SetToolTip(this.mEndTimeBack1FrameButton, @"Back 1 frame");
            tt.SetToolTip(this.mEndTimeBack1SecondButton, @"Back 1 second");
            tt.SetToolTip(this.mEndTimeForward1SecondButton, @"Forward 1 second");
            tt.SetToolTip(this.mEndTimeForward1FrameButton, @"Forward 1 frame");
            tt.SetToolTip(this.mPlaybackPointPictureBox, @"Seek / Current playback position");
            tt.SetToolTip(this.mFadeInCheckBox, @"Fade in audio at start");
            tt.SetToolTip(this.mFadeOutCheckBox, @"Fade out audio at end");
            tt.SetToolTip(this.mStartGapCombo, @"Initial silence before audio begins");

            if (mForNarration == false)
            {
                RefreshListView(for_thumbnail.mMusicSlide);
                mAudioListView_SelectedIndexChanged(this, new EventArgs());
            }
        }

        //*******************************************************************
        private void RefreshListView(CMusicSlide selectedSlide)
        {
            MusicSlideListViewItem selectedslvi = null;


            this.SuspendLayout();
            try
            {
                if (mAudioListView.Items != null)
                {
                    mAudioListView.Items.Clear();
                    mSlides.Clear();
                }

                ICollection collection = null;
                if (mForNarration == true)
                {
                    collection = mForSlideshow.NarrationSlides;
                }
                else
                {
                    collection = mForSlideshow.mMusicSlides;
                }


                if (selectedSlide == null && collection.Count > 0)
                {
                    // select first item in icollection
                    foreach (CMusicSlide item in collection)
                    {
                        selectedSlide = item;
                        break;
                    }
                }

                double slideshowLength = mForSlideshow.GetLengthInSeconds();

                foreach (CMusicSlide ms in collection)
                {
                    if (ms.IsLoopMusicSlide == false)
                    {

                        MusicSlideListViewItem lvi = new MusicSlideListViewItem(System.IO.Path.GetFileNameWithoutExtension(ms.mName), ms);

                        float startTime = ((float)ms.StartFrameOffset) / CGlobals.mCurrentProject.DiskPreferences.frames_per_second;
                        float endTime = startTime + ((float)ms.GetDurationInSeconds());
                        float length = endTime - startTime;


                        lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, CGlobals.TimeStringFromSeconds(startTime, false)));
                        lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, CGlobals.TimeStringFromSeconds(endTime, false)));
                        lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, CGlobals.TimeStringFromSeconds(length, false)));

                        if (ms == selectedSlide)
                        {
                            selectedslvi = lvi;
                        }

                        //
                        // Music slides which start after slideshow ends are highlighted red
                        //
                        if (startTime >= slideshowLength)
                        {
                            lvi.ForeColor = Color.Red;
                        }

                        mAudioListView.Items.Add(lvi);
                        mSlides.Add(ms);
                    }

                }
            }
            finally
            {
                this.ResumeLayout();
            }

            if (selectedslvi != null)
            {
                selectedslvi.Selected = true;
                mAudioListView.Select();
                selectedslvi.EnsureVisible();
            }
            else
            {
                this.Close();
            }


        }

        //*******************************************************************
        private void mAudioListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.mPlaying)
            {
                this.mStopButton_Click(sender, e);
            }

            ListView.SelectedListViewItemCollection selected = mAudioListView.SelectedItems;
            if (selected.Count < 1)
            {
                mMoveUpOrderButton.Enabled = false;
                mMoveDownOrderButton.Enabled = false;
                return;
            }

            MusicSlideListViewItem item = selected[0] as MusicSlideListViewItem;
            if (item == null)
            {
                mMoveUpOrderButton.Enabled = false;
                mMoveDownOrderButton.Enabled = false;
                return;
            }

            CMusicSlide selectedSlide = item.mForMusicSlide;

            if (mForNarration == false)
            {
                if (mSlides.Count > 0)
                {
                    if (selectedSlide == mSlides[0])
                    {
                        mMoveUpOrderButton.Enabled = false;
                    }
                    else
                    {
                        mMoveUpOrderButton.Enabled = true;
                    }

                    if (selectedSlide == mSlides[mSlides.Count - 1])
                    {
                        mMoveDownOrderButton.Enabled = false;
                    }
                    else
                    {
                        mMoveDownOrderButton.Enabled = true;
                    }
                }
            }

            InitForMusicSlide(item.mForMusicSlide);
        }


        //*******************************************************************
        private void UpdateListViewLengths()
        {
            mForSlideshow.CalcLengthOfAllSlides();

            this.SuspendLayout();
            try
            {
                ListView.ListViewItemCollection items = mAudioListView.Items;
                if (items.Count < 1) return;

                double slideshowLength = mForSlideshow.GetLengthInSeconds();

                foreach ( MusicSlideListViewItem mslvi in items)
                {
                    CMusicSlide ms = mslvi.mForMusicSlide;

                    float startTime = ((float)ms.StartFrameOffset) / CGlobals.mCurrentProject.DiskPreferences.frames_per_second;
                    float endTime = startTime + ((float)ms.GetDurationInSeconds());
                    float length = endTime - startTime;

                    if (mslvi.SubItems.Count < 4) continue;

                    mslvi.SubItems[1].Text = CGlobals.TimeStringFromSeconds(startTime, false);
                    mslvi.SubItems[2].Text = CGlobals.TimeStringFromSeconds(endTime, false);
                    mslvi.SubItems[3].Text= CGlobals.TimeStringFromSeconds(length, false);

                    if (startTime >= slideshowLength)
                    {
                        mslvi.ForeColor = Color.Red;
                    }
                    else
                    {
                        mslvi.ForeColor = Color.Black;
                    }
                }

            }
            finally
            {
                this.ResumeLayout();
            }

        }


        //*******************************************************************
        private void InitForMusicSlide(CMusicSlide forMs)
        {
            mForMusicSlide = forMs;
		
			CMusicSlide ms = mForMusicSlide;

			double fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

			string fn = System.IO.Path.GetFileName(ms.mName);

			this.mStartTimeTrackBar.Minimum=0;
			this.mStartTimeTrackBar.Maximum =(int) (ms.Player.GetDurationInSeconds() * fps);

			int val = (int)(  ms.StartMusicOffset * fps);
			if (val <0) val=0;
			if (val > mStartTimeTrackBar.Maximum) val = mStartTimeTrackBar.Maximum;

			this.mStartTimeTrackBar.Value = val;
		

			this.mEndTimeTrackBar.Minimum=0;
			this.mEndTimeTrackBar.Maximum =(int)( ms.Player.GetDurationInSeconds() * fps);

			val = (int)( mEndTimeTrackBar.Maximum - (ms.EndMusicOffset * fps));
			if (val <0) val=0;
			if (val > mEndTimeTrackBar.Maximum) val = mEndTimeTrackBar.Maximum;

			this.mEndTimeTrackBar.Value = val;
		
			ResetAndRedrawControls();


		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrimMusicWindow));
            this.mEndTimeTrackBar = new System.Windows.Forms.TrackBar();
            this.mStartTimeTrackBar = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.mResetButton = new System.Windows.Forms.Button();
            this.mDoneButton = new System.Windows.Forms.Button();
            this.mDurationLabel = new System.Windows.Forms.Label();
            this.mStartTimeLabel = new System.Windows.Forms.Label();
            this.mEndTimeLabel = new System.Windows.Forms.Label();
            this.mStartTimeBack1SecondButton = new System.Windows.Forms.Button();
            this.mStartTimeBack1FrameButton = new System.Windows.Forms.Button();
            this.mStartTimeForward1FrameButton = new System.Windows.Forms.Button();
            this.mStartTimeForward1SecondButton = new System.Windows.Forms.Button();
            this.mEndTimeForward1SecondButton = new System.Windows.Forms.Button();
            this.mEndTimeForward1FrameButton = new System.Windows.Forms.Button();
            this.mEndTimeBack1FrameButton = new System.Windows.Forms.Button();
            this.mEndTimeBack1SecondButton = new System.Windows.Forms.Button();
            this.mFadeInCheckBox = new System.Windows.Forms.CheckBox();
            this.mFadeOutCheckBox = new System.Windows.Forms.CheckBox();
            this.mPlaybackTimerLabel = new System.Windows.Forms.Label();
            this.mMusicVolmeTrackBar = new System.Windows.Forms.TrackBar();
            this.mVolumeLabel = new System.Windows.Forms.Label();
            this.mStartGapCombo = new System.Windows.Forms.ComboBox();
            this.mInitalGapLabel = new System.Windows.Forms.Label();
            this.mVolumeTextBox = new System.Windows.Forms.TextBox();
            this.mSecondsLabel = new System.Windows.Forms.Label();
            this.mTopGrouo = new System.Windows.Forms.GroupBox();
            this.mFadeOutLengthTextBox = new System.Windows.Forms.TextBox();
            this.mFadeInLengthTextBox = new System.Windows.Forms.TextBox();
            this.mFadeOutLengthTrackBar = new System.Windows.Forms.TrackBar();
            this.mFadeInLengthTrackBar = new System.Windows.Forms.TrackBar();
            this.mPlaybackPointPictureBox = new System.Windows.Forms.PictureBox();
            this.mGroupBox2 = new System.Windows.Forms.GroupBox();
            this.mInv = new System.Windows.Forms.Label();
            this.mAudioListView = new System.Windows.Forms.ListView();
            this.mNameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mStartTimeColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mEndTimeColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mLengthColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mAddButton = new System.Windows.Forms.Button();
            this.mRemoveButton = new System.Windows.Forms.Button();
            this.mMoveDownOrderButton = new System.Windows.Forms.Button();
            this.mMoveUpOrderButton = new System.Windows.Forms.Button();
            this.mStopButton = new BitmapButton.SimpleBitmapButton();
            this.mPlayButton = new BitmapButton.SimpleBitmapButton();
            this.mSlideshowAudioSettings = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.mEndTimeTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mStartTimeTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mMusicVolmeTrackBar)).BeginInit();
            this.mTopGrouo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mFadeOutLengthTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mFadeInLengthTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mPlaybackPointPictureBox)).BeginInit();
            this.mGroupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mEndTimeTrackBar
            // 
            this.mEndTimeTrackBar.Location = new System.Drawing.Point(4, 90);
            this.mEndTimeTrackBar.Name = "mEndTimeTrackBar";
            this.mEndTimeTrackBar.Size = new System.Drawing.Size(424, 45);
            this.mEndTimeTrackBar.TabIndex = 0;
            this.mEndTimeTrackBar.TabStop = false;
            this.mEndTimeTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // mStartTimeTrackBar
            // 
            this.mStartTimeTrackBar.Location = new System.Drawing.Point(1, 26);
            this.mStartTimeTrackBar.Name = "mStartTimeTrackBar";
            this.mStartTimeTrackBar.Size = new System.Drawing.Size(424, 45);
            this.mStartTimeTrackBar.TabIndex = 1;
            this.mStartTimeTrackBar.TabStop = false;
            this.mStartTimeTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(425, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 31);
            this.label1.TabIndex = 2;
            this.label1.Text = "Start Time";
            // 
            // label2
            // 
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(428, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 23);
            this.label2.TabIndex = 3;
            this.label2.Text = "End Time";
            // 
            // mResetButton
            // 
            this.mResetButton.Location = new System.Drawing.Point(457, 393);
            this.mResetButton.Name = "mResetButton";
            this.mResetButton.Size = new System.Drawing.Size(56, 23);
            this.mResetButton.TabIndex = 5;
            this.mResetButton.TabStop = false;
            this.mResetButton.Text = "Reset";
            this.mResetButton.Click += new System.EventHandler(this.mResetButton_Click);
            // 
            // mDoneButton
            // 
            this.mDoneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mDoneButton.Location = new System.Drawing.Point(457, 470);
            this.mDoneButton.Name = "mDoneButton";
            this.mDoneButton.Size = new System.Drawing.Size(56, 23);
            this.mDoneButton.TabIndex = 1;
            this.mDoneButton.Text = "Close";
            this.mDoneButton.Click += new System.EventHandler(this.mDoneButton_Click);
            // 
            // mDurationLabel
            // 
            this.mDurationLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mDurationLabel.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mDurationLabel.Location = new System.Drawing.Point(9, 151);
            this.mDurationLabel.Name = "mDurationLabel";
            this.mDurationLabel.Size = new System.Drawing.Size(144, 21);
            this.mDurationLabel.TabIndex = 7;
            this.mDurationLabel.Text = "Total duration : 00:00:00.00";
            this.mDurationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mStartTimeLabel
            // 
            this.mStartTimeLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mStartTimeLabel.Location = new System.Drawing.Point(425, 34);
            this.mStartTimeLabel.Name = "mStartTimeLabel";
            this.mStartTimeLabel.Size = new System.Drawing.Size(77, 24);
            this.mStartTimeLabel.TabIndex = 8;
            this.mStartTimeLabel.Text = "00:00:00.00";
            // 
            // mEndTimeLabel
            // 
            this.mEndTimeLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mEndTimeLabel.Location = new System.Drawing.Point(428, 100);
            this.mEndTimeLabel.Name = "mEndTimeLabel";
            this.mEndTimeLabel.Size = new System.Drawing.Size(70, 16);
            this.mEndTimeLabel.TabIndex = 9;
            this.mEndTimeLabel.Text = "00:00:00.00";
            // 
            // mStartTimeBack1SecondButton
            // 
            this.mStartTimeBack1SecondButton.Font = new System.Drawing.Font("Segoe UI", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mStartTimeBack1SecondButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mStartTimeBack1SecondButton.Location = new System.Drawing.Point(9, 50);
            this.mStartTimeBack1SecondButton.Name = "mStartTimeBack1SecondButton";
            this.mStartTimeBack1SecondButton.Size = new System.Drawing.Size(24, 16);
            this.mStartTimeBack1SecondButton.TabIndex = 10;
            this.mStartTimeBack1SecondButton.TabStop = false;
            this.mStartTimeBack1SecondButton.Text = "<<";
            this.mStartTimeBack1SecondButton.Click += new System.EventHandler(this.mStartTimeBack1SecondButton_Click);
            // 
            // mStartTimeBack1FrameButton
            // 
            this.mStartTimeBack1FrameButton.Font = new System.Drawing.Font("Segoe UI", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mStartTimeBack1FrameButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mStartTimeBack1FrameButton.Location = new System.Drawing.Point(33, 50);
            this.mStartTimeBack1FrameButton.Name = "mStartTimeBack1FrameButton";
            this.mStartTimeBack1FrameButton.Size = new System.Drawing.Size(24, 16);
            this.mStartTimeBack1FrameButton.TabIndex = 11;
            this.mStartTimeBack1FrameButton.TabStop = false;
            this.mStartTimeBack1FrameButton.Text = "<";
            this.mStartTimeBack1FrameButton.Click += new System.EventHandler(this.mStartTimeBack1FrameButton_Click);
            // 
            // mStartTimeForward1FrameButton
            // 
            this.mStartTimeForward1FrameButton.Font = new System.Drawing.Font("Segoe UI", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mStartTimeForward1FrameButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mStartTimeForward1FrameButton.Location = new System.Drawing.Point(57, 50);
            this.mStartTimeForward1FrameButton.Name = "mStartTimeForward1FrameButton";
            this.mStartTimeForward1FrameButton.Size = new System.Drawing.Size(24, 16);
            this.mStartTimeForward1FrameButton.TabIndex = 12;
            this.mStartTimeForward1FrameButton.TabStop = false;
            this.mStartTimeForward1FrameButton.Text = ">";
            this.mStartTimeForward1FrameButton.Click += new System.EventHandler(this.mStartTimeForward1FrameButton_Click);
            // 
            // mStartTimeForward1SecondButton
            // 
            this.mStartTimeForward1SecondButton.Font = new System.Drawing.Font("Segoe UI", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mStartTimeForward1SecondButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mStartTimeForward1SecondButton.Location = new System.Drawing.Point(81, 50);
            this.mStartTimeForward1SecondButton.Name = "mStartTimeForward1SecondButton";
            this.mStartTimeForward1SecondButton.Size = new System.Drawing.Size(24, 16);
            this.mStartTimeForward1SecondButton.TabIndex = 13;
            this.mStartTimeForward1SecondButton.TabStop = false;
            this.mStartTimeForward1SecondButton.Text = ">>";
            this.mStartTimeForward1SecondButton.Click += new System.EventHandler(this.mStartTimeForward1SecondButton_Click);
            // 
            // mEndTimeForward1SecondButton
            // 
            this.mEndTimeForward1SecondButton.Font = new System.Drawing.Font("Segoe UI", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mEndTimeForward1SecondButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mEndTimeForward1SecondButton.Location = new System.Drawing.Point(84, 114);
            this.mEndTimeForward1SecondButton.Name = "mEndTimeForward1SecondButton";
            this.mEndTimeForward1SecondButton.Size = new System.Drawing.Size(24, 16);
            this.mEndTimeForward1SecondButton.TabIndex = 47;
            this.mEndTimeForward1SecondButton.TabStop = false;
            this.mEndTimeForward1SecondButton.Text = ">>";
            this.mEndTimeForward1SecondButton.Click += new System.EventHandler(this.mEndTimeForward1SecondButton_Click);
            // 
            // mEndTimeForward1FrameButton
            // 
            this.mEndTimeForward1FrameButton.Font = new System.Drawing.Font("Segoe UI", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mEndTimeForward1FrameButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mEndTimeForward1FrameButton.Location = new System.Drawing.Point(60, 114);
            this.mEndTimeForward1FrameButton.Name = "mEndTimeForward1FrameButton";
            this.mEndTimeForward1FrameButton.Size = new System.Drawing.Size(24, 16);
            this.mEndTimeForward1FrameButton.TabIndex = 46;
            this.mEndTimeForward1FrameButton.TabStop = false;
            this.mEndTimeForward1FrameButton.Text = ">";
            this.mEndTimeForward1FrameButton.Click += new System.EventHandler(this.mEndTimeForward1FrameButton_Click);
            // 
            // mEndTimeBack1FrameButton
            // 
            this.mEndTimeBack1FrameButton.Font = new System.Drawing.Font("Segoe UI", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mEndTimeBack1FrameButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mEndTimeBack1FrameButton.Location = new System.Drawing.Point(36, 114);
            this.mEndTimeBack1FrameButton.Name = "mEndTimeBack1FrameButton";
            this.mEndTimeBack1FrameButton.Size = new System.Drawing.Size(24, 16);
            this.mEndTimeBack1FrameButton.TabIndex = 45;
            this.mEndTimeBack1FrameButton.TabStop = false;
            this.mEndTimeBack1FrameButton.Text = "<";
            this.mEndTimeBack1FrameButton.Click += new System.EventHandler(this.mEndTimeBack1FrameButton_Click);
            // 
            // mEndTimeBack1SecondButton
            // 
            this.mEndTimeBack1SecondButton.Font = new System.Drawing.Font("Segoe UI", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mEndTimeBack1SecondButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mEndTimeBack1SecondButton.Location = new System.Drawing.Point(12, 114);
            this.mEndTimeBack1SecondButton.Name = "mEndTimeBack1SecondButton";
            this.mEndTimeBack1SecondButton.Size = new System.Drawing.Size(24, 16);
            this.mEndTimeBack1SecondButton.TabIndex = 44;
            this.mEndTimeBack1SecondButton.TabStop = false;
            this.mEndTimeBack1SecondButton.Text = "<<";
            this.mEndTimeBack1SecondButton.Click += new System.EventHandler(this.mEndTimeBack1SecondButton_Click);
            // 
            // mFadeInCheckBox
            // 
            this.mFadeInCheckBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mFadeInCheckBox.Location = new System.Drawing.Point(231, 55);
            this.mFadeInCheckBox.Name = "mFadeInCheckBox";
            this.mFadeInCheckBox.Size = new System.Drawing.Size(64, 24);
            this.mFadeInCheckBox.TabIndex = 48;
            this.mFadeInCheckBox.TabStop = false;
            this.mFadeInCheckBox.Text = "Fade in";
            this.mFadeInCheckBox.CheckedChanged += new System.EventHandler(this.mFadeInCheckBox_CheckedChanged);
            // 
            // mFadeOutCheckBox
            // 
            this.mFadeOutCheckBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mFadeOutCheckBox.Location = new System.Drawing.Point(231, 119);
            this.mFadeOutCheckBox.Name = "mFadeOutCheckBox";
            this.mFadeOutCheckBox.Size = new System.Drawing.Size(72, 24);
            this.mFadeOutCheckBox.TabIndex = 49;
            this.mFadeOutCheckBox.TabStop = false;
            this.mFadeOutCheckBox.Text = "Fade out";
            this.mFadeOutCheckBox.CheckedChanged += new System.EventHandler(this.mFadeOutCheckBox_CheckedChanged);
            // 
            // mPlaybackTimerLabel
            // 
            this.mPlaybackTimerLabel.Location = new System.Drawing.Point(84, 260);
            this.mPlaybackTimerLabel.Name = "mPlaybackTimerLabel";
            this.mPlaybackTimerLabel.Size = new System.Drawing.Size(151, 24);
            this.mPlaybackTimerLabel.TabIndex = 50;
            this.mPlaybackTimerLabel.Text = "00:00:00.00 (00:00:00.00)";
            this.mPlaybackTimerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mMusicVolmeTrackBar
            // 
            this.mMusicVolmeTrackBar.Location = new System.Drawing.Point(283, 22);
            this.mMusicVolmeTrackBar.Maximum = 100;
            this.mMusicVolmeTrackBar.Name = "mMusicVolmeTrackBar";
            this.mMusicVolmeTrackBar.Size = new System.Drawing.Size(139, 45);
            this.mMusicVolmeTrackBar.TabIndex = 52;
            this.mMusicVolmeTrackBar.TabStop = false;
            this.mMusicVolmeTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.mMusicVolmeTrackBar.Value = 100;
            this.mMusicVolmeTrackBar.Scroll += new System.EventHandler(this.mMusicVolmeTrackBar_Scroll);
            // 
            // mVolumeLabel
            // 
            this.mVolumeLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mVolumeLabel.Location = new System.Drawing.Point(230, 25);
            this.mVolumeLabel.Name = "mVolumeLabel";
            this.mVolumeLabel.Size = new System.Drawing.Size(53, 14);
            this.mVolumeLabel.TabIndex = 53;
            this.mVolumeLabel.Text = "Volume";
            // 
            // mStartGapCombo
            // 
            this.mStartGapCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mStartGapCombo.Location = new System.Drawing.Point(88, 22);
            this.mStartGapCombo.Name = "mStartGapCombo";
            this.mStartGapCombo.Size = new System.Drawing.Size(47, 21);
            this.mStartGapCombo.TabIndex = 54;
            this.mStartGapCombo.TabStop = false;
            this.mStartGapCombo.SelectedIndexChanged += new System.EventHandler(this.mStartGapCombo_SelectedIndexChanged);
            // 
            // mInitalGapLabel
            // 
            this.mInitalGapLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mInitalGapLabel.Location = new System.Drawing.Point(9, 25);
            this.mInitalGapLabel.Name = "mInitalGapLabel";
            this.mInitalGapLabel.Size = new System.Drawing.Size(74, 27);
            this.mInitalGapLabel.TabIndex = 55;
            this.mInitalGapLabel.Text = "Initial silence";
            // 
            // mVolumeTextBox
            // 
            this.mVolumeTextBox.Location = new System.Drawing.Point(427, 23);
            this.mVolumeTextBox.Name = "mVolumeTextBox";
            this.mVolumeTextBox.ReadOnly = true;
            this.mVolumeTextBox.Size = new System.Drawing.Size(34, 22);
            this.mVolumeTextBox.TabIndex = 56;
            this.mVolumeTextBox.TabStop = false;
            this.mVolumeTextBox.Text = "100%";
            // 
            // mSecondsLabel
            // 
            this.mSecondsLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mSecondsLabel.Location = new System.Drawing.Point(140, 25);
            this.mSecondsLabel.Name = "mSecondsLabel";
            this.mSecondsLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mSecondsLabel.Size = new System.Drawing.Size(68, 23);
            this.mSecondsLabel.TabIndex = 57;
            this.mSecondsLabel.Text = "Seconds";
            // 
            // mTopGrouo
            // 
            this.mTopGrouo.Controls.Add(this.mFadeOutLengthTextBox);
            this.mTopGrouo.Controls.Add(this.mFadeInLengthTextBox);
            this.mTopGrouo.Controls.Add(this.mFadeOutLengthTrackBar);
            this.mTopGrouo.Controls.Add(this.mFadeInLengthTrackBar);
            this.mTopGrouo.Controls.Add(this.mFadeInCheckBox);
            this.mTopGrouo.Controls.Add(this.mStartTimeForward1SecondButton);
            this.mTopGrouo.Controls.Add(this.mStartTimeForward1FrameButton);
            this.mTopGrouo.Controls.Add(this.mStartTimeBack1FrameButton);
            this.mTopGrouo.Controls.Add(this.mStartTimeBack1SecondButton);
            this.mTopGrouo.Controls.Add(this.mEndTimeLabel);
            this.mTopGrouo.Controls.Add(this.mFadeOutCheckBox);
            this.mTopGrouo.Controls.Add(this.mEndTimeBack1SecondButton);
            this.mTopGrouo.Controls.Add(this.mEndTimeBack1FrameButton);
            this.mTopGrouo.Controls.Add(this.mEndTimeForward1FrameButton);
            this.mTopGrouo.Controls.Add(this.mEndTimeForward1SecondButton);
            this.mTopGrouo.Controls.Add(this.mStartTimeLabel);
            this.mTopGrouo.Controls.Add(this.mDurationLabel);
            this.mTopGrouo.Controls.Add(this.label2);
            this.mTopGrouo.Controls.Add(this.label1);
            this.mTopGrouo.Controls.Add(this.mStartTimeTrackBar);
            this.mTopGrouo.Controls.Add(this.mEndTimeTrackBar);
            this.mTopGrouo.Controls.Add(this.mPlaybackPointPictureBox);
            this.mTopGrouo.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.mTopGrouo.Location = new System.Drawing.Point(8, 12);
            this.mTopGrouo.Name = "mTopGrouo";
            this.mTopGrouo.Size = new System.Drawing.Size(504, 175);
            this.mTopGrouo.TabIndex = 58;
            this.mTopGrouo.TabStop = false;
            this.mTopGrouo.Text = "Trim start and end times";
            // 
            // mFadeOutLengthTextBox
            // 
            this.mFadeOutLengthTextBox.Enabled = false;
            this.mFadeOutLengthTextBox.Location = new System.Drawing.Point(385, 119);
            this.mFadeOutLengthTextBox.Name = "mFadeOutLengthTextBox";
            this.mFadeOutLengthTextBox.ReadOnly = true;
            this.mFadeOutLengthTextBox.Size = new System.Drawing.Size(34, 22);
            this.mFadeOutLengthTextBox.TabIndex = 62;
            this.mFadeOutLengthTextBox.TabStop = false;
            this.mFadeOutLengthTextBox.Text = "1.0s";
            // 
            // mFadeInLengthTextBox
            // 
            this.mFadeInLengthTextBox.Enabled = false;
            this.mFadeInLengthTextBox.Location = new System.Drawing.Point(385, 55);
            this.mFadeInLengthTextBox.Name = "mFadeInLengthTextBox";
            this.mFadeInLengthTextBox.ReadOnly = true;
            this.mFadeInLengthTextBox.Size = new System.Drawing.Size(34, 22);
            this.mFadeInLengthTextBox.TabIndex = 61;
            this.mFadeInLengthTextBox.TabStop = false;
            this.mFadeInLengthTextBox.Text = "1.0s";
            // 
            // mFadeOutLengthTrackBar
            // 
            this.mFadeOutLengthTrackBar.AutoSize = false;
            this.mFadeOutLengthTrackBar.Enabled = false;
            this.mFadeOutLengthTrackBar.Location = new System.Drawing.Point(305, 119);
            this.mFadeOutLengthTrackBar.Maximum = 500;
            this.mFadeOutLengthTrackBar.Name = "mFadeOutLengthTrackBar";
            this.mFadeOutLengthTrackBar.Size = new System.Drawing.Size(74, 32);
            this.mFadeOutLengthTrackBar.TabIndex = 54;
            this.mFadeOutLengthTrackBar.TabStop = false;
            this.mFadeOutLengthTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.mFadeOutLengthTrackBar.Value = 100;
            this.mFadeOutLengthTrackBar.Scroll += new System.EventHandler(this.mFadeOutLengthTrackBar_Scroll);
            // 
            // mFadeInLengthTrackBar
            // 
            this.mFadeInLengthTrackBar.AutoSize = false;
            this.mFadeInLengthTrackBar.Enabled = false;
            this.mFadeInLengthTrackBar.Location = new System.Drawing.Point(305, 55);
            this.mFadeInLengthTrackBar.Maximum = 500;
            this.mFadeInLengthTrackBar.Name = "mFadeInLengthTrackBar";
            this.mFadeInLengthTrackBar.Size = new System.Drawing.Size(74, 29);
            this.mFadeInLengthTrackBar.TabIndex = 53;
            this.mFadeInLengthTrackBar.TabStop = false;
            this.mFadeInLengthTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.mFadeInLengthTrackBar.Value = 100;
            this.mFadeInLengthTrackBar.Scroll += new System.EventHandler(this.mFadeInLengthTrackBar_Scroll);
            // 
            // mPlaybackPointPictureBox
            // 
            this.mPlaybackPointPictureBox.Location = new System.Drawing.Point(1, 13);
            this.mPlaybackPointPictureBox.Name = "mPlaybackPointPictureBox";
            this.mPlaybackPointPictureBox.Size = new System.Drawing.Size(435, 13);
            this.mPlaybackPointPictureBox.TabIndex = 51;
            this.mPlaybackPointPictureBox.TabStop = false;
            // 
            // mGroupBox2
            // 
            this.mGroupBox2.Controls.Add(this.mInv);
            this.mGroupBox2.Controls.Add(this.mStartGapCombo);
            this.mGroupBox2.Controls.Add(this.mVolumeTextBox);
            this.mGroupBox2.Controls.Add(this.mInitalGapLabel);
            this.mGroupBox2.Controls.Add(this.mVolumeLabel);
            this.mGroupBox2.Controls.Add(this.mSecondsLabel);
            this.mGroupBox2.Controls.Add(this.mMusicVolmeTrackBar);
            this.mGroupBox2.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.mGroupBox2.Location = new System.Drawing.Point(9, 193);
            this.mGroupBox2.Name = "mGroupBox2";
            this.mGroupBox2.Size = new System.Drawing.Size(504, 59);
            this.mGroupBox2.TabIndex = 59;
            this.mGroupBox2.TabStop = false;
            this.mGroupBox2.Text = "Additional settings";
            // 
            // mInv
            // 
            this.mInv.BackColor = System.Drawing.Color.Transparent;
            this.mInv.Location = new System.Drawing.Point(284, 49);
            this.mInv.Name = "mInv";
            this.mInv.Size = new System.Drawing.Size(152, 23);
            this.mInv.TabIndex = 60;
            // 
            // mAudioListView
            // 
            this.mAudioListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.mNameColumn,
            this.mStartTimeColumn,
            this.mEndTimeColumn,
            this.mLengthColumn});
            this.mAudioListView.FullRowSelect = true;
            this.mAudioListView.GridLines = true;
            this.mAudioListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.mAudioListView.HideSelection = false;
            this.mAudioListView.Location = new System.Drawing.Point(10, 293);
            this.mAudioListView.MultiSelect = false;
            this.mAudioListView.Name = "mAudioListView";
            this.mAudioListView.Size = new System.Drawing.Size(443, 200);
            this.mAudioListView.TabIndex = 60;
            this.mAudioListView.TabStop = false;
            this.mAudioListView.UseCompatibleStateImageBehavior = false;
            this.mAudioListView.View = System.Windows.Forms.View.Details;
            this.mAudioListView.SelectedIndexChanged += new System.EventHandler(this.mAudioListView_SelectedIndexChanged);
            // 
            // mNameColumn
            // 
            this.mNameColumn.Text = "Filename";
            this.mNameColumn.Width = 239;
            // 
            // mStartTimeColumn
            // 
            this.mStartTimeColumn.Text = "Start time";
            this.mStartTimeColumn.Width = 70;
            // 
            // mEndTimeColumn
            // 
            this.mEndTimeColumn.Text = "End time";
            // 
            // mLengthColumn
            // 
            this.mLengthColumn.Text = "Length";
            this.mLengthColumn.Width = 67;
            // 
            // mAddButton
            // 
            this.mAddButton.Location = new System.Drawing.Point(457, 293);
            this.mAddButton.Name = "mAddButton";
            this.mAddButton.Size = new System.Drawing.Size(56, 23);
            this.mAddButton.TabIndex = 61;
            this.mAddButton.TabStop = false;
            this.mAddButton.Text = "Add";
            this.mAddButton.UseVisualStyleBackColor = true;
            this.mAddButton.Click += new System.EventHandler(this.mAddButton_Click);
            // 
            // mRemoveButton
            // 
            this.mRemoveButton.Location = new System.Drawing.Point(457, 318);
            this.mRemoveButton.Name = "mRemoveButton";
            this.mRemoveButton.Size = new System.Drawing.Size(56, 23);
            this.mRemoveButton.TabIndex = 62;
            this.mRemoveButton.TabStop = false;
            this.mRemoveButton.Text = "Remove";
            this.mRemoveButton.UseVisualStyleBackColor = true;
            this.mRemoveButton.Click += new System.EventHandler(this.mRemoveButton_Click);
            // 
            // mMoveDownOrderButton
            // 
            this.mMoveDownOrderButton.BackgroundImage = global::dvdslideshowfontend.Properties.Resources.downsmall;
            this.mMoveDownOrderButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mMoveDownOrderButton.Location = new System.Drawing.Point(457, 368);
            this.mMoveDownOrderButton.Name = "mMoveDownOrderButton";
            this.mMoveDownOrderButton.Size = new System.Drawing.Size(56, 25);
            this.mMoveDownOrderButton.TabIndex = 64;
            this.mMoveDownOrderButton.TabStop = false;
            this.mMoveDownOrderButton.UseVisualStyleBackColor = true;
            this.mMoveDownOrderButton.Click += new System.EventHandler(this.mMoveDownOrderButton_Click);
            // 
            // mMoveUpOrderButton
            // 
            this.mMoveUpOrderButton.BackgroundImage = global::dvdslideshowfontend.Properties.Resources.upsmall;
            this.mMoveUpOrderButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mMoveUpOrderButton.Location = new System.Drawing.Point(457, 343);
            this.mMoveUpOrderButton.Name = "mMoveUpOrderButton";
            this.mMoveUpOrderButton.Size = new System.Drawing.Size(56, 25);
            this.mMoveUpOrderButton.TabIndex = 63;
            this.mMoveUpOrderButton.TabStop = false;
            this.mMoveUpOrderButton.UseVisualStyleBackColor = true;
            this.mMoveUpOrderButton.Click += new System.EventHandler(this.mMoveUpOrderButton_Click);
            // 
            // mStopButton
            // 
            this.mStopButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mStopButton.Image = ((System.Drawing.Image)(resources.GetObject("mStopButton.Image")));
            this.mStopButton.Location = new System.Drawing.Point(44, 255);
            this.mStopButton.Name = "mStopButton";
            this.mStopButton.Size = new System.Drawing.Size(31, 32);
            this.mStopButton.TabIndex = 43;
            this.mStopButton.TabStop = false;
            this.mStopButton.Text = "simpleBitmapButton2";
            this.mStopButton.Click += new System.EventHandler(this.mStopButton_Click);
            // 
            // mPlayButton
            // 
            this.mPlayButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mPlayButton.Image = ((System.Drawing.Image)(resources.GetObject("mPlayButton.Image")));
            this.mPlayButton.Location = new System.Drawing.Point(12, 255);
            this.mPlayButton.Name = "mPlayButton";
            this.mPlayButton.Size = new System.Drawing.Size(31, 32);
            this.mPlayButton.TabIndex = 42;
            this.mPlayButton.TabStop = false;
            this.mPlayButton.Text = "simpleBitmapButton2";
            this.mPlayButton.Click += new System.EventHandler(this.mPlayButton_Click);
            // 
            // mSlideshowAudioSettings
            // 
            this.mSlideshowAudioSettings.Location = new System.Drawing.Point(354, 257);
            this.mSlideshowAudioSettings.Name = "mSlideshowAudioSettings";
            this.mSlideshowAudioSettings.Size = new System.Drawing.Size(159, 23);
            this.mSlideshowAudioSettings.TabIndex = 65;
            this.mSlideshowAudioSettings.TabStop = false;
            this.mSlideshowAudioSettings.Text = "Slideshow audio settings";
            this.mSlideshowAudioSettings.UseVisualStyleBackColor = true;
            this.mSlideshowAudioSettings.Click += new System.EventHandler(this.mSlideshowAudioSettings_Click);
            // 
            // TrimMusicWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(522, 503);
            this.ControlBox = false;
            this.Controls.Add(this.mSlideshowAudioSettings);
            this.Controls.Add(this.mMoveDownOrderButton);
            this.Controls.Add(this.mMoveUpOrderButton);
            this.Controls.Add(this.mRemoveButton);
            this.Controls.Add(this.mAddButton);
            this.Controls.Add(this.mAudioListView);
            this.Controls.Add(this.mGroupBox2);
            this.Controls.Add(this.mTopGrouo);
            this.Controls.Add(this.mDoneButton);
            this.Controls.Add(this.mResetButton);
            this.Controls.Add(this.mPlaybackTimerLabel);
            this.Controls.Add(this.mStopButton);
            this.Controls.Add(this.mPlayButton);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TrimMusicWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Trim music";
            ((System.ComponentModel.ISupportInitialize)(this.mEndTimeTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mStartTimeTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mMusicVolmeTrackBar)).EndInit();
            this.mTopGrouo.ResumeLayout(false);
            this.mTopGrouo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mFadeOutLengthTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mFadeInLengthTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mPlaybackPointPictureBox)).EndInit();
            this.mGroupBox2.ResumeLayout(false);
            this.mGroupBox2.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		//*******************************************************************
		private void ResetAndRedrawControls()
		{
			mFadeInCheckBox.Checked =mForMusicSlide.FadeIn;
            int value = (int)(mForMusicSlide.FadeInLength * 100.0f);
            value = CGlobals.Clamp(value, 0, 500);
            mFadeInLengthTrackBar.Value = value;
            UpdateFadeInLengthTextBox();

			mFadeOutCheckBox.Checked = mForMusicSlide.FadeOut;
            value = (int)(mForMusicSlide.FadeOutLength * 100.0f);
            value = CGlobals.Clamp(value, 0, 500);
            mFadeOutLengthTrackBar.Value = value;
            UpdateFadeOutLengthTextBox();

            int vol = ((int)(mForMusicSlide.Volume*100));
			if (vol<0)vol=0;
			if (vol>100) vol=100;
			this.mVolumeTextBox.Text = vol.ToString()+"%";
			this.mMusicVolmeTrackBar.Value = vol; 
			UpdateTimeLabels();
			UpdatePositionMarker(this.mForMusicSlide.StartMusicOffset);			
			try
			{
                string st = String.Format("{0:f1}", this.mForMusicSlide.InitialSilence);
                mStartGapCombo.Text = st;
			}
			catch
			{
			}

		
			
		}

		//*******************************************************************
		private void mDoneButton_Click(object sender, System.EventArgs e)
		{
			if (this.mPlaying)
			{
				this.mStopButton_Click(sender,e);
			}

            // make sure volumes are back
            this.mForMusicSlide.ApplyWithVolume(1.0);

			this.Close();
		}
			
		//*******************************************************************
		private void UpdateTimeLabels()
		{
			CMusicSlide ms = mForMusicSlide;
			double fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

			double st = ms.StartMusicOffset;
			string stl = CGlobals.TimeStringFromSeconds(st);
			this.mStartTimeLabel.Text = stl;

			double et = ms.EndMusicOffset;
			string etl = CGlobals.TimeStringFromSeconds((ms.Player.GetDurationInSeconds() - et)+0.001);
			this.mEndTimeLabel.Text = etl;

			string du = CGlobals.TimeStringFromSeconds(ms.GetDurationInSeconds()+0.001);
			this.mDurationLabel.Text = "Total duration: "+du;

		}

		//*******************************************************************
		private void UpdatePlaybackTimeLabels(double pb_time)
		{

			double ds = this.mForMusicSlide.GetDurationInSeconds();
			double tgo = ds - (pb_time - this.mForMusicSlide.StartMusicOffset);

			// make sure we only upto duration of music
			if ( tgo <0)
			{
				pb_time += tgo;
			}

			string gt = CGlobals.TimeStringFromSeconds(pb_time+0.001);
			string lt = CGlobals.TimeStringFromSeconds(pb_time+0.001 - this.mForMusicSlide.StartMusicOffset+this.mForMusicSlide.InitialSilence);

			mPlaybackTimerLabel.Text = lt+" / ("+gt+")";
			mPlaybackTimerLabel.Invalidate();
		}


		//*******************************************************************
		private void StartTrackerScroll(object o, System.EventArgs e)
		{
			CMusicSlide ms = mForMusicSlide;
			double fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

			if (mStartTimeTrackBar.Value >= mEndTimeTrackBar.Value-(2*fps))
			{
				int val = mEndTimeTrackBar.Value-(int)(2*fps);
				if (val <0) val=0;

				mStartTimeTrackBar.Value = val;
			}

			ms.StartMusicOffset = ((double)this.mStartTimeTrackBar.Value) / fps;


			StartTrackerChanged();
		}

	    //*******************************************************************
        private void StartTrackerScroll_MouseUp(object o, System.EventArgs e)
        {
            UpdateListViewLengths();
        }

        //*******************************************************************
        private void EndTrackerScroll_MouseUp(object o, System.EventArgs e)
        {
            UpdateListViewLengths();
        }

		//*******************************************************************
		private void StartTrackerChanged()
		{
			CMusicSlide ms = this.mForMusicSlide;
			int x_pos = GetXposFromSecondOffset(ms.StartMusicOffset);

			if (this.mPlaying==false &&
				x_pos > this.mPosMarker)
			{
				this.mPosMarker = x_pos;
				this.ReDrawMarker();
			}

			if (this.mPlaying==true)
			{
				mPosMarker = x_pos;
				this.SetMarketToXPosAndSeek(mPosMarker,ms.StartMusicOffset );
			}

			this.UpdateTimeLabels();
		}

		//*******************************************************************
		private void EndTrackerScroll(object o, System.EventArgs e)
		{
			CMusicSlide ms = mForMusicSlide;
			double fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

			if (mEndTimeTrackBar.Value <= mStartTimeTrackBar.Value+(2*fps))
			{
				int val = mStartTimeTrackBar.Value+(int)(2*fps);
				if (val >mEndTimeTrackBar.Maximum) val = mEndTimeTrackBar.Maximum;
				mEndTimeTrackBar.Value = val;
			}
			EndTrackerChanged();
		}

		//*******************************************************************
		private void EndTrackerChanged()
		{
			CMusicSlide ms = this.mForMusicSlide;
			double fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

			ms.EndMusicOffset = ((double)(this.mEndTimeTrackBar.Maximum - this.mEndTimeTrackBar.Value)) / fps;
		
			int x_pos = GetXposFromSecondOffset(this.GetEndTime());

			if (this.mPosMarker > x_pos)
			{
				this.mPosMarker = x_pos;
				this.ReDrawMarker();
			}

			if (this.mPlaying==true)
			{
				double s_en_t = this.GetEndTime()-1;
				if (s_en_t < ms.StartMusicOffset)
				{
					s_en_t = ms.StartMusicOffset;
				}

				SetMarketToXPosAndSeek(GetXposFromSecondOffset(s_en_t),s_en_t);

			}

			this.UpdateTimeLabels();

		}

		//*******************************************************************
		private void mPlayButton_Click(object sender, System.EventArgs e)
		{

			if (mPlaying==true) 
			{
				this.SetMarketToXPosAndSeek(this.GetXposFromSecondOffset(this.mForMusicSlide.StartMusicOffset),
					this.mForMusicSlide.StartMusicOffset);	                        
					return ;
			}

			mPlaying=true;
			mPausedAtEnd=false;

			if (mPlaybackTimer!=null)
			{
				mPlaybackTimer.Stop();
				mPlaybackTimer = null;
			}

		
		
			CMusicSlide ms = mForMusicSlide;
			double fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

			bool wvs = CGlobals.WaitVideoSeekCompletion;
			bool vs = CGlobals.VideoSeeking;
			CGlobals.WaitVideoSeekCompletion=true;
			CGlobals.VideoSeeking = true;
			CGlobals.ContinuePlayingAudioAfterSeeking=true;
            int start_audio_frame_time = ms.StartFrameOffset + ((int)(ms.InitialSilence * fps));
			ms.Play(start_audio_frame_time);// cause seek to start
			CGlobals.VideoSeeking = false;
			ms.Play(start_audio_frame_time);// cause start of play
			CGlobals.WaitVideoSeekCompletion = wvs;
			CGlobals.VideoSeeking=vs;
			
			// do tick now

			PlaybackTick(this,new EventArgs());

			mPlaybackTimer = new System.Windows.Forms.Timer();
			mPlaybackTimer.Tick+=new EventHandler(this.PlaybackTick);
			mPlaybackTimer.Interval=100;
			mPlaybackTimer.Start();
		}

		//*******************************************************************
		private void mStopButton_Click(object sender, System.EventArgs e)
		{
			if (mPlaying==false) return ;
			mPlaying=false;
			mPausedAtEnd=false;

			if (mPlaybackTimer!=null)
			{
				mPlaybackTimer.Stop();
				mPlaybackTimer = null;
			}

			CMusicSlide ms = mForMusicSlide;
			ms.Stop();
		}

        private Nullable<double> mNextSeekTime = null;

        private void TestIfNeedTodoActualSeek()
        {
            lock (this_lock)
			{
                if (mNextSeekTime.HasValue == false) return;

                double seek_time = mNextSeekTime.Value;
                mNextSeekTime = null;
                if (this.mPlaying == true)
                {
                    if (seek_time < 0)
                    {
                        seek_time = this.GetSecondOffsetFromXpos(mPosMarker);
                    }

                    CMusicSlide ms = mForMusicSlide;

                    bool wvs = CGlobals.WaitVideoSeekCompletion;
                    bool vs = CGlobals.VideoSeeking;
                    CGlobals.WaitVideoSeekCompletion = true;
                    CGlobals.VideoSeeking = true;
                    CGlobals.ContinuePlayingAudioAfterSeeking = true;
                    //	Console.WriteLine("Seeking to time "+seek_time);
                    ms.Player.SeekToTime(seek_time, CGlobals.WaitVideoSeekCompletion);// cause start of play
                    CGlobals.WaitVideoSeekCompletion = wvs;
                    CGlobals.VideoSeeking = vs;
              }
            }
        }

		//*******************************************************************
		private void PlaybackTick(object o, System.EventArgs e)
		{
			lock (this_lock)
			{
                TestIfNeedTodoActualSeek();
				double pos = mForMusicSlide.Player.GetPlaybackPositionInSeconds();

				if (mMouseDownMarketPictureBox==false)
				{
					UpdatePositionMarker(pos);
					UpdatePlaybackTimeLabels(pos);
				}

				if (pos > this.GetEndTime())
				{
					this.mForMusicSlide.Player.Pause();
					mPausedAtEnd=true;
				}
				else if (mPausedAtEnd==true)
				{
					this.mForMusicSlide.Player.Play();
				}

				CMusicSlide ms = mForMusicSlide;

				double s_dif = pos - ms.StartMusicOffset;
                float s_len = ms.FadeInLength;

				double e_dif = this.GetEndTime() -  pos;
                float e_len = ms.FadeOutLength;

                if (s_dif< s_len && this.mForMusicSlide.FadeIn ==true)
				{
					double delta =  s_dif / s_len;
					if (delta <0) delta = 0;
					if (delta >1) delta =1;
					ms.ApplyWithVolume(delta);
					mCurrentPlaybackVol=delta;
				}
				else if  (e_dif< e_len && this.mForMusicSlide.FadeOut ==true)
				{
					double delta =  e_dif / e_len;
					if (delta <0) delta = 0;
					if (delta >1) delta =1;
					ms.ApplyWithVolume(delta);
					mCurrentPlaybackVol = delta;
				}
				else
				{
					ms.ApplyWithVolume(1.0);
					mCurrentPlaybackVol=1.0;
				}
			}
		}


		//*******************************************************************
		private void UpdatePositionMarker(double seconds_offset)
		{
			if (mPlaybackPointPictureBox.Image==null)
			{
				mPlaybackPointPictureBox.Image= new Bitmap(mPlaybackPointPictureBox.Width,mPlaybackPointPictureBox.Height);
			}

		
			int x_pos = GetXposFromSecondOffset(seconds_offset);

			mPosMarker= x_pos;

			ReDrawMarker();

		}

		//*******************************************************************
		private void ReDrawMarker()
		{
			Graphics g = Graphics.FromImage(mPlaybackPointPictureBox.Image);

			g.Clear(mPlaybackPointPictureBox.BackColor);
			Brush b = new SolidBrush(Color.LightSteelBlue);

		
			Point [] points = new Point[3];
			points[0].X=-6 + mPosMarker;
			points[0].Y=0;
			points[1].X=mPosMarker;
			points[1].Y=mPlaybackPointPictureBox.Height;
			points[2].X=6+mPosMarker;
			points[2].Y=0;
			g.FillPolygon(b,points);
			g.Dispose();

			mPlaybackPointPictureBox.Invalidate();
		
		}

		//*******************************************************************
		private void MouseDownMarketPictureBox(object o, System.Windows.Forms.MouseEventArgs e)
		{
			
			SetMarketToXPosAndSeek(e.X, -1);
			ReDrawMarker();

			mMouseDownMarketPictureBox=true;

		}

		//*******************************************************************
		private void MouseUpMarketPictureBox(object o, System.Windows.Forms.MouseEventArgs e)
		{
			mMouseDownMarketPictureBox=false;	
		}

		//*******************************************************************
		private void MouseMoveMarketPictureBox(object o, System.Windows.Forms.MouseEventArgs e)
		{
			if (mMouseDownMarketPictureBox==true)
			{
				double time = GetSecondOffsetFromXpos(this.mPosMarker);
				
				
				SetMarketToXPosAndSeek(e.X, time);
				ReDrawMarker();
				UpdatePlaybackTimeLabels(time);
			}
		}

		//*******************************************************************
		private void SetMarketToXPosAndSeek(int x_pos, double seek_time)
		{
            lock (this_lock)
            {
                if (x_pos < MarketXOffset) x_pos = MarketXOffset;
                if (x_pos > mPlaybackPointPictureBox.Width - 23)
                {
                    x_pos = mPlaybackPointPictureBox.Width - 23;
                }

                int min_x_pos = GetXposFromSecondOffset(this.mForMusicSlide.StartMusicOffset);
                int max_x_pos = GetXposFromSecondOffset(this.GetEndTime()) + 1;


                if (min_x_pos > x_pos)
                {
                    x_pos = min_x_pos;
                }

                if (x_pos > max_x_pos)
                {
                    x_pos = max_x_pos - 1;
                    seek_time = this.GetEndTime() - 1;

                    if (seek_time < mForMusicSlide.StartMusicOffset)
                    {
                        seek_time = mForMusicSlide.StartMusicOffset;
                    }
                }

                mPosMarker = x_pos;

                if (this.mPlaying == true)
                {
                    if (seek_time < 0)
                    {
                        seek_time = this.GetSecondOffsetFromXpos(mPosMarker);
                    }

                    mNextSeekTime = seek_time;
                }
            }
		}

		//*******************************************************************
		private double GetEndTime()
		{
			return this.mForMusicSlide.Player.GetDurationInSeconds() - this.mForMusicSlide.EndMusicOffset;
		}

		//*******************************************************************
		private double GetSecondOffsetFromXpos(int x_pos)
		{
			double xx = (double) (x_pos - MarketXOffset);

			double len =  mForMusicSlide.Player.GetDurationInSeconds();
			double tracker_size = (double) (mStartTimeTrackBar.Width-25);

			double frac = tracker_size / len;

			xx =xx / frac;

		//	Console.WriteLine("x to s = " + x_pos+ " "+xx+" "+frac);

			if (xx < this.mForMusicSlide.StartMusicOffset)
			{
				xx = this.mForMusicSlide.StartMusicOffset;
			}

			double et = GetEndTime();
			if (xx > et)
			{
				xx = et;
			}

			return xx;

		}
		

		//*******************************************************************
		private int GetXposFromSecondOffset(double seconds)
		{
			double len =  mForMusicSlide.Player.GetDurationInSeconds();
			double tracker_size = (double) (mStartTimeTrackBar.Width-25);

			double frac = tracker_size/len;
		
			double x_pos  = seconds * frac;
			x_pos+=MarketXOffset;

		//	Console.WriteLine("s to x = " + seconds+ " "+x_pos+" "+frac);

			return (int) x_pos;

		}

		//*******************************************************************
		private void mStartTimeBack1SecondButton_Click(object sender, System.EventArgs e)
		{
			CMusicSlide ms = mForMusicSlide;
			double fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

			ms.StartMusicOffset = ms.StartMusicOffset-1;
			if (ms.StartMusicOffset <0 ) ms.StartMusicOffset =0;

			int val = (int)(  ms.StartMusicOffset * fps);

			mStartTimeTrackBar.Value = val;
			StartTrackerChanged();
            UpdateListViewLengths();
		}

		//*******************************************************************
		private void mStartTimeBack1FrameButton_Click(object sender, System.EventArgs e)
		{
			CMusicSlide ms = mForMusicSlide;
			double fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

			ms.StartMusicOffset = ms.StartMusicOffset-(1/fps);
			if (ms.StartMusicOffset <0 ) ms.StartMusicOffset =0;

			int val = (int)(  ms.StartMusicOffset * fps);

			mStartTimeTrackBar.Value = val;
			StartTrackerChanged();
            UpdateListViewLengths();
		}

		//*******************************************************************
		private void mStartTimeForward1FrameButton_Click(object sender, System.EventArgs e)
		{
			CMusicSlide ms = mForMusicSlide;
			if (ms.Player.GetDurationInSeconds() -(ms.StartMusicOffset +ms.EndMusicOffset) <=2)return ;
	
			double fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

			ms.StartMusicOffset = ms.StartMusicOffset+(1/fps);
			if (ms.StartMusicOffset >= this.GetEndTime() ) 
			{
				ms.StartMusicOffset = this.GetEndTime() - (1/fps);
			}

			int val = (int)(  ms.StartMusicOffset * fps);

			mStartTimeTrackBar.Value = val;
			StartTrackerChanged();
            UpdateListViewLengths();
		}

		//*******************************************************************
		private void mStartTimeForward1SecondButton_Click(object sender, System.EventArgs e)
		{
			CMusicSlide ms = mForMusicSlide;
			if (ms.Player.GetDurationInSeconds() -(ms.StartMusicOffset +ms.EndMusicOffset) <=2)return ;
		
			double fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

			ms.StartMusicOffset = ms.StartMusicOffset+1;
			if (ms.StartMusicOffset >= this.GetEndTime() ) 
			{
				ms.StartMusicOffset = this.GetEndTime() - (1/fps);
			}

			int val = (int)(  ms.StartMusicOffset * fps);

			mStartTimeTrackBar.Value = val;
			StartTrackerChanged();
            UpdateListViewLengths();

		}

		//*******************************************************************
		private void mEndTimeBack1SecondButton_Click(object sender, System.EventArgs e)
		{
			CMusicSlide ms = mForMusicSlide;
			if (ms.Player.GetDurationInSeconds() -(ms.StartMusicOffset +ms.EndMusicOffset) <=2)return ;
		
			double fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

			ms.EndMusicOffset = ms.EndMusicOffset+1;
			if (this.GetEndTime() <= ms.StartMusicOffset ) 
			{
				ms.EndMusicOffset = (ms.Player.GetDurationInSeconds() - ms.StartMusicOffset) - (1/fps);
			}

			int val = (int)( GetEndTime() * fps);

			mEndTimeTrackBar.Value = val;
			EndTrackerChanged();
            UpdateListViewLengths();
		}

		//*******************************************************************
		private void mEndTimeBack1FrameButton_Click(object sender, System.EventArgs e)
		{
			CMusicSlide ms = mForMusicSlide;
			if (ms.Player.GetDurationInSeconds() -(ms.StartMusicOffset +ms.EndMusicOffset) <=2)return ;
			
			double fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

			ms.EndMusicOffset = ms.EndMusicOffset+(1/fps);
			if (this.GetEndTime() <= ms.StartMusicOffset ) 
			{
				ms.EndMusicOffset = (ms.Player.GetDurationInSeconds() - ms.StartMusicOffset) - (1/fps);
			}

			int val = (int)( GetEndTime() * fps);

			mEndTimeTrackBar.Value = val;
			EndTrackerChanged();
            UpdateListViewLengths();
		}

		//*******************************************************************
		private void mEndTimeForward1FrameButton_Click(object sender, System.EventArgs e)
		{
			CMusicSlide ms = mForMusicSlide;
			double fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

			ms.EndMusicOffset = ms.EndMusicOffset-(1/fps);
			if (ms.EndMusicOffset < 0 ) 
			{
				ms.EndMusicOffset=0;
			}

			int val = (int)( GetEndTime() * fps);
			if (val > mEndTimeTrackBar.Maximum)
			{
				val = mEndTimeTrackBar.Maximum;
			}

			mEndTimeTrackBar.Value = val;
			EndTrackerChanged();
            UpdateListViewLengths();
		}

		//*******************************************************************
		private void mEndTimeForward1SecondButton_Click(object sender, System.EventArgs e)
		{
			CMusicSlide ms = mForMusicSlide;
			double fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

			ms.EndMusicOffset = ms.EndMusicOffset-1;
			if (ms.EndMusicOffset < 0 ) 
			{
				ms.EndMusicOffset=0;
			}

			int val = (int)( GetEndTime() * fps);

			if (val > mEndTimeTrackBar.Maximum)
			{
				val = mEndTimeTrackBar.Maximum;
			}

			mEndTimeTrackBar.Value = val;
			EndTrackerChanged();
            UpdateListViewLengths();
		}

		//*******************************************************************
		private void mResetButton_Click(object sender, System.EventArgs e)
		{
			CMusicSlide ms = mForMusicSlide;
			double fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

			ms.StartMusicOffset=0;
			ms.EndMusicOffset=0;
			ms.FadeOut=false;
			ms.FadeIn=false;
			ms.Volume=1.0;
			ms.InitialSilence=0.0;


			int val = (int)( GetEndTime() * fps);

			if (val > mEndTimeTrackBar.Maximum)
			{
				val = mEndTimeTrackBar.Maximum;
			}
			mEndTimeTrackBar.Value = val;

			mStartTimeTrackBar.Value=0;
			this.StartTrackerChanged();
            UpdateListViewLengths();
			ResetAndRedrawControls();
		}

        //*******************************************************************
        private void mFadeInCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			this.mForMusicSlide.FadeIn= this.mFadeInCheckBox.Checked;

            if (this.mForMusicSlide.FadeIn == true)
            {
                mFadeInLengthTextBox.Enabled = true;
                mFadeInLengthTrackBar.Enabled = true;
            }
            else
            {
                mFadeInLengthTextBox.Enabled = true;
                mFadeInLengthTrackBar.Enabled = false;
            }
		}

        //*******************************************************************
        private void mFadeInLengthTrackBar_Scroll(object sender, EventArgs e)
        {
            float time = (float)mFadeInLengthTrackBar.Value;
            time /= 100;
            time = CGlobals.ClampF(time, 0, 5);
            mForMusicSlide.FadeInLength = time;

            UpdateFadeInLengthTextBox();
        }

        //*******************************************************************
        private void UpdateFadeInLengthTextBox()
        {
            mFadeInLengthTextBox.Text = string.Format("{0:0.0}", mForMusicSlide.FadeInLength) + "s";
        }

        //*******************************************************************
        private void mFadeOutCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.mForMusicSlide.FadeOut = this.mFadeOutCheckBox.Checked;

            if (this.mForMusicSlide.FadeOut == true)
            {
                mFadeOutLengthTextBox.Enabled = true;
                mFadeOutLengthTrackBar.Enabled = true;
            }
            else
            {
                mFadeOutLengthTextBox.Enabled = true;
                mFadeOutLengthTrackBar.Enabled = false;
            }
        }


        //*******************************************************************
        private void mFadeOutLengthTrackBar_Scroll(object sender, EventArgs e)
        {
            float time = (float)mFadeOutLengthTrackBar.Value;
            time /= 100;
            time = CGlobals.ClampF(time, 0, 5);
            mForMusicSlide.FadeOutLength = time;

            UpdateFadeOutLengthTextBox();
        }

        //*******************************************************************
        private void UpdateFadeOutLengthTextBox()
        {
            mFadeOutLengthTextBox.Text = string.Format("{0:0.0}", mForMusicSlide.FadeOutLength) + "s";
        }

        //*******************************************************************
        private void mMusicVolmeTrackBar_Scroll(object sender, System.EventArgs e)
		{
			lock (this_lock)
			{
				double vol = ((double)this.mMusicVolmeTrackBar.Value)/100.0;

				this.mForMusicSlide.Volume = vol;
				this.mVolumeTextBox.Text = ((int)(mForMusicSlide.Volume*100)).ToString()+"%";

				this.mForMusicSlide.ApplyWithVolume(this.mCurrentPlaybackVol );
			}

		}

		//*******************************************************************
		private void mStartGapCombo_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			mForMusicSlide.InitialSilence = double.Parse(this.mStartGapCombo.Text,CultureInfo.InvariantCulture);
			UpdateTimeLabels();
            UpdateListViewLengths();
		}

        //*******************************************************************
        private void mMoveUpOrderButton_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selected = mAudioListView.SelectedItems;
            if (selected.Count < 1) return;

            MusicSlideListViewItem item = selected[0] as MusicSlideListViewItem;
            if (item==null) return;

            Form1.mMainForm.GetSlideShowMusicManager().MoveMusicUpOrder(item.mForMusicSlide);
            this.RefreshListView(item.mForMusicSlide);

        }

        //*******************************************************************
        private void mMoveDownOrderButton_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selected = mAudioListView.SelectedItems;
            if (selected.Count < 1) return;

            MusicSlideListViewItem item = selected[0] as MusicSlideListViewItem;
            if (item == null) return;

            Form1.mMainForm.GetSlideShowMusicManager().MoveMusicDownOrder(item.mForMusicSlide);
            this.RefreshListView(item.mForMusicSlide);

        }

        //*******************************************************************
        private void mAddButton_Click(object sender, EventArgs e)
        {
           
            CMusicSlide added = null;
            if (mForNarration == true)
            {
                if (Form1.mMainForm.GetSlideShowNarrationManager().AddNarrationDialogStart() == true)
                {
                    if (mForSlideshow.NarrationSlides.Count > 0)
                    {
                        added = mForSlideshow.NarrationSlides[mForSlideshow.NarrationSlides.Count - 1];
                    }

                }
            }
            else
            {
                if (Form1.mMainForm.GetSlideShowMusicManager().AddMusicDialogStart() == true)
                {
                    // get last music slide, which isnt a loop one

                    CMusicSlide lastNonLooped = null;
                    foreach (CMusicSlide ms in mForSlideshow.mMusicSlides)
                    {
                        if (ms.IsLoopMusicSlide == false)
                        {
                            lastNonLooped = ms;
                        }
                    }

                    if (lastNonLooped!=null)
                    {
                        added = lastNonLooped;
                    }
                }
            }

            if (added != null)
            {
                RefreshListView(added);
            }
        }

        //*******************************************************************
        private void mRemoveButton_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selected = mAudioListView.SelectedItems;
            if (selected.Count < 1) return;

            MusicSlideListViewItem item = selected[0] as MusicSlideListViewItem;
            if (item == null) return;

            if (mForNarration == true)
            {
                Form1.mMainForm.GetSlideShowNarrationManager().DeleteItem(item.mForMusicSlide);
            }
            else
            {
                Form1.mMainForm.GetSlideShowMusicManager().DeleteItem(item.mForMusicSlide);
            }

            this.RefreshListView(null);
        }

        private void mSlideshowAudioSettings_Click(object sender, EventArgs e)
        {
            SlideShowAdvanceOptions dialog = new SlideShowAdvanceOptions(true);
            dialog.ShowDialog();
        }
    }

}
