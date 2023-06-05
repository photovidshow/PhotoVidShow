using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DVDSlideshow;
using System.Globalization;

namespace dvdslideshowfontend
{
    /// <summary>
    /// Summary description for Form2.
    /// </summary>
    public class SlideShowAdvanceOptions : System.Windows.Forms.Form
    {
        private System.Windows.Forms.CheckBox LoopMusicCheckBox;
        private System.Windows.Forms.CheckBox FadeInMusiCheckBox;
        private System.Windows.Forms.CheckBox SyncSlideShowButton;
        private System.Windows.Forms.CheckBox FadeInCheckBox;
        private System.Windows.Forms.ColorDialog FadeInColorDialog;
        private System.Windows.Forms.ColorDialog FadeOutColorDialog;
        private System.Windows.Forms.Button mDoneAdvaneOptionsButton;
        private System.Windows.Forms.Label DefaultSlideTimeLabel;
        private System.Windows.Forms.ComboBox DefaultTimeCombo;
        private System.Windows.Forms.Button ApplyTimeToAllSlidesButton;

        private bool mMementoProjectWhenDone = false;
        private string mMementoMessage = "";
        private System.Windows.Forms.Button FadeInColorButton;
        private System.Windows.Forms.Button FadeOutColorButton;
        private System.Windows.Forms.CheckBox FadeOutCheckBox;
        private System.Windows.Forms.Label FadeOutColorLabel;
        private System.Windows.Forms.Label FadeInColorLabel;
        private System.Windows.Forms.CheckBox FadeOutMusiCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar VideoMasterVolumeTrackBar;

        private bool mAlreadyInSyncChangeCallback = false;
        private TrackBar NarrationMasterVolumeTrackBar;
        private TrackBar MusicMasterVolumeTrackBar;
        private Label label4;
        private TabControl mSlideshowOptionsTabControl;
        private TabPage mSlideshowOptionsTabPage;
        private TabPage mAudioOptionsTabPage;
        private TabPage mChapterMarkersTabPage;
        private ComboBox mChapterMarkersTimedIntervalComboBox;
        private RadioButton mSlideChapterMarkersRadioButton;
        private RadioButton mTimedChapterMarkersRadioButton;
        private CheckBox mIncludeSlideshowMarkersCheckBox;
        private Label mChapterMarkerMinutesLabel;
        private Label label6;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        private bool mRebuilding = false;
        private Button mDisiablePanZoomOnAllSlidesButton;
        private Button mEnablePanZoomOnAllSlidesButton;
        private bool mCMThumbnailsNeedReValidating = false;

        public SlideShowAdvanceOptions(bool startupInAudioSettings)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // If video output then remove chapter markers page
            //
            if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType != CGlobals.VideoType.BLURAY &&
                CGlobals.mCurrentProject.DiskPreferences.OutputVideoType != CGlobals.VideoType.DVD)
            {
                mSlideshowOptionsTabControl.TabPages.Remove(mChapterMarkersTabPage);
            }

            DefaultTimeCombo.Items.Add("0.1");
            DefaultTimeCombo.Items.Add("0.25");
            DefaultTimeCombo.Items.Add("0.5");
            DefaultTimeCombo.Items.Add("0.75");
            for (int i = 1; i <= 60; i++)
            {
                DefaultTimeCombo.Items.Add(i);
            }

            DefaultTimeCombo.LostFocus += new EventHandler(this.DefaultTimeComboChanged);
            DefaultTimeCombo.SelectedIndexChanged += new EventHandler(this.DefaultTimeComboIndexChnaged);


            SetValuesToMatchSlideshow();

            this.FormClosed+= SlideShowAdvanceOptions_FormClosed;

            if (startupInAudioSettings == true)
            {
                mSlideshowOptionsTabControl.SelectedTab = mAudioOptionsTabPage;
            }


            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            }

        private void SlideShowAdvanceOptions_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mCMThumbnailsNeedReValidating==true)
            {
                Form1.mMainForm.GetSlideShowManager().ValidateAllCMThumbnails();
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.LoopMusicCheckBox = new System.Windows.Forms.CheckBox();
            this.FadeInMusiCheckBox = new System.Windows.Forms.CheckBox();
            this.NarrationMasterVolumeTrackBar = new System.Windows.Forms.TrackBar();
            this.MusicMasterVolumeTrackBar = new System.Windows.Forms.TrackBar();
            this.VideoMasterVolumeTrackBar = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.FadeOutMusiCheckBox = new System.Windows.Forms.CheckBox();
            this.SyncSlideShowButton = new System.Windows.Forms.CheckBox();
            this.ApplyTimeToAllSlidesButton = new System.Windows.Forms.Button();
            this.DefaultSlideTimeLabel = new System.Windows.Forms.Label();
            this.DefaultTimeCombo = new System.Windows.Forms.ComboBox();
            this.FadeOutColorLabel = new System.Windows.Forms.Label();
            this.FadeInColorLabel = new System.Windows.Forms.Label();
            this.FadeOutColorButton = new System.Windows.Forms.Button();
            this.FadeInColorButton = new System.Windows.Forms.Button();
            this.FadeOutCheckBox = new System.Windows.Forms.CheckBox();
            this.FadeInCheckBox = new System.Windows.Forms.CheckBox();
            this.FadeInColorDialog = new System.Windows.Forms.ColorDialog();
            this.FadeOutColorDialog = new System.Windows.Forms.ColorDialog();
            this.mDoneAdvaneOptionsButton = new System.Windows.Forms.Button();
            this.mSlideshowOptionsTabControl = new System.Windows.Forms.TabControl();
            this.mSlideshowOptionsTabPage = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.mAudioOptionsTabPage = new System.Windows.Forms.TabPage();
            this.mChapterMarkersTabPage = new System.Windows.Forms.TabPage();
            this.mIncludeSlideshowMarkersCheckBox = new System.Windows.Forms.CheckBox();
            this.mChapterMarkerMinutesLabel = new System.Windows.Forms.Label();
            this.mChapterMarkersTimedIntervalComboBox = new System.Windows.Forms.ComboBox();
            this.mSlideChapterMarkersRadioButton = new System.Windows.Forms.RadioButton();
            this.mTimedChapterMarkersRadioButton = new System.Windows.Forms.RadioButton();
            this.mEnablePanZoomOnAllSlidesButton = new System.Windows.Forms.Button();
            this.mDisiablePanZoomOnAllSlidesButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.NarrationMasterVolumeTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MusicMasterVolumeTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.VideoMasterVolumeTrackBar)).BeginInit();
            this.mSlideshowOptionsTabControl.SuspendLayout();
            this.mSlideshowOptionsTabPage.SuspendLayout();
            this.mAudioOptionsTabPage.SuspendLayout();
            this.mChapterMarkersTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // LoopMusicCheckBox
            // 
            this.LoopMusicCheckBox.Checked = true;
            this.LoopMusicCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.LoopMusicCheckBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.LoopMusicCheckBox.Location = new System.Drawing.Point(6, 6);
            this.LoopMusicCheckBox.Name = "LoopMusicCheckBox";
            this.LoopMusicCheckBox.Size = new System.Drawing.Size(200, 24);
            this.LoopMusicCheckBox.TabIndex = 0;
            this.LoopMusicCheckBox.TabStop = false;
            this.LoopMusicCheckBox.Text = "Loop background music";
            this.LoopMusicCheckBox.CheckedChanged += new System.EventHandler(this.LoopMusicCheckBox_CheckedChanged);
            // 
            // FadeInMusiCheckBox
            // 
            this.FadeInMusiCheckBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FadeInMusiCheckBox.Location = new System.Drawing.Point(6, 32);
            this.FadeInMusiCheckBox.Name = "FadeInMusiCheckBox";
            this.FadeInMusiCheckBox.Size = new System.Drawing.Size(284, 24);
            this.FadeInMusiCheckBox.TabIndex = 1;
            this.FadeInMusiCheckBox.TabStop = false;
            this.FadeInMusiCheckBox.Text = "Fade audio at the start of the slideshow";
            this.FadeInMusiCheckBox.CheckedChanged += new System.EventHandler(this.FadeInMusiCheckBox_CheckedChanged);
            // 
            // NarrationMasterVolumeTrackBar
            // 
            this.NarrationMasterVolumeTrackBar.Location = new System.Drawing.Point(106, 165);
            this.NarrationMasterVolumeTrackBar.Maximum = 100;
            this.NarrationMasterVolumeTrackBar.Name = "NarrationMasterVolumeTrackBar";
            this.NarrationMasterVolumeTrackBar.Size = new System.Drawing.Size(169, 45);
            this.NarrationMasterVolumeTrackBar.TabIndex = 53;
            this.NarrationMasterVolumeTrackBar.TabStop = false;
            this.NarrationMasterVolumeTrackBar.TickFrequency = 50;
            this.NarrationMasterVolumeTrackBar.Value = 100;
            this.NarrationMasterVolumeTrackBar.Scroll += new System.EventHandler(this.NarrationMasterVolumeTrackBar_Scroll);
            // 
            // MusicMasterVolumeTrackBar
            // 
            this.MusicMasterVolumeTrackBar.Location = new System.Drawing.Point(106, 140);
            this.MusicMasterVolumeTrackBar.Maximum = 100;
            this.MusicMasterVolumeTrackBar.Name = "MusicMasterVolumeTrackBar";
            this.MusicMasterVolumeTrackBar.Size = new System.Drawing.Size(169, 45);
            this.MusicMasterVolumeTrackBar.TabIndex = 52;
            this.MusicMasterVolumeTrackBar.TabStop = false;
            this.MusicMasterVolumeTrackBar.TickFrequency = 50;
            this.MusicMasterVolumeTrackBar.Value = 100;
            this.MusicMasterVolumeTrackBar.Scroll += new System.EventHandler(this.MusicMasterVolumeTrackBar_Scroll);
            // 
            // VideoMasterVolumeTrackBar
            // 
            this.VideoMasterVolumeTrackBar.Location = new System.Drawing.Point(106, 114);
            this.VideoMasterVolumeTrackBar.Maximum = 100;
            this.VideoMasterVolumeTrackBar.Name = "VideoMasterVolumeTrackBar";
            this.VideoMasterVolumeTrackBar.Size = new System.Drawing.Size(169, 45);
            this.VideoMasterVolumeTrackBar.TabIndex = 50;
            this.VideoMasterVolumeTrackBar.TabStop = false;
            this.VideoMasterVolumeTrackBar.TickFrequency = 50;
            this.VideoMasterVolumeTrackBar.Value = 100;
            this.VideoMasterVolumeTrackBar.Scroll += new System.EventHandler(this.VideoMasterVolumeTrackBar_Scroll);
            // 
            // label4
            // 
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(5, 161);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 29);
            this.label4.TabIndex = 54;
            this.label4.Text = "Narration Audio";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(128, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 23);
            this.label3.TabIndex = 51;
            this.label3.Text = "Audio master volumes";
            // 
            // label2
            // 
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(-1, 135);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 29);
            this.label2.TabIndex = 8;
            this.label2.Text = "Background Music";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(4, 109);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 29);
            this.label1.TabIndex = 7;
            this.label1.Text = "Video Audio";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FadeOutMusiCheckBox
            // 
            this.FadeOutMusiCheckBox.Checked = true;
            this.FadeOutMusiCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.FadeOutMusiCheckBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FadeOutMusiCheckBox.Location = new System.Drawing.Point(6, 58);
            this.FadeOutMusiCheckBox.Name = "FadeOutMusiCheckBox";
            this.FadeOutMusiCheckBox.Size = new System.Drawing.Size(269, 24);
            this.FadeOutMusiCheckBox.TabIndex = 5;
            this.FadeOutMusiCheckBox.TabStop = false;
            this.FadeOutMusiCheckBox.Text = "Fade audio at the end of the slideshow";
            this.FadeOutMusiCheckBox.CheckedChanged += new System.EventHandler(this.FadeOutMusiCheckBox_CheckedChanged);
            // 
            // SyncSlideShowButton
            // 
            this.SyncSlideShowButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.SyncSlideShowButton.Location = new System.Drawing.Point(6, 6);
            this.SyncSlideShowButton.Name = "SyncSlideShowButton";
            this.SyncSlideShowButton.Size = new System.Drawing.Size(216, 24);
            this.SyncSlideShowButton.TabIndex = 4;
            this.SyncSlideShowButton.TabStop = false;
            this.SyncSlideShowButton.Text = "Sync slideshow to background music";
            this.SyncSlideShowButton.Click += new System.EventHandler(this.SyncSlideShowButton_CheckedChanged);
            // 
            // ApplyTimeToAllSlidesButton
            // 
            this.ApplyTimeToAllSlidesButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ApplyTimeToAllSlidesButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ApplyTimeToAllSlidesButton.Location = new System.Drawing.Point(170, 36);
            this.ApplyTimeToAllSlidesButton.Name = "ApplyTimeToAllSlidesButton";
            this.ApplyTimeToAllSlidesButton.Size = new System.Drawing.Size(142, 23);
            this.ApplyTimeToAllSlidesButton.TabIndex = 20;
            this.ApplyTimeToAllSlidesButton.TabStop = false;
            this.ApplyTimeToAllSlidesButton.Text = "Apply time to all slides";
            this.ApplyTimeToAllSlidesButton.Click += new System.EventHandler(this.ApplyTimeToAllSlidesButton_Click);
            // 
            // DefaultSlideTimeLabel
            // 
            this.DefaultSlideTimeLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.DefaultSlideTimeLabel.Location = new System.Drawing.Point(3, 41);
            this.DefaultSlideTimeLabel.Name = "DefaultSlideTimeLabel";
            this.DefaultSlideTimeLabel.Size = new System.Drawing.Size(97, 32);
            this.DefaultSlideTimeLabel.TabIndex = 21;
            this.DefaultSlideTimeLabel.Text = "Default slide time";
            // 
            // DefaultTimeCombo
            // 
            this.DefaultTimeCombo.Location = new System.Drawing.Point(102, 38);
            this.DefaultTimeCombo.Name = "DefaultTimeCombo";
            this.DefaultTimeCombo.Size = new System.Drawing.Size(45, 21);
            this.DefaultTimeCombo.TabIndex = 19;
            this.DefaultTimeCombo.TabStop = false;
            this.DefaultTimeCombo.Text = "comboBox1";
            // 
            // FadeOutColorLabel
            // 
            this.FadeOutColorLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FadeOutColorLabel.Location = new System.Drawing.Point(152, 113);
            this.FadeOutColorLabel.Name = "FadeOutColorLabel";
            this.FadeOutColorLabel.Size = new System.Drawing.Size(100, 14);
            this.FadeOutColorLabel.TabIndex = 18;
            this.FadeOutColorLabel.Text = "Fade to color";
            // 
            // FadeInColorLabel
            // 
            this.FadeInColorLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FadeInColorLabel.Location = new System.Drawing.Point(152, 84);
            this.FadeInColorLabel.Name = "FadeInColorLabel";
            this.FadeInColorLabel.Size = new System.Drawing.Size(100, 16);
            this.FadeInColorLabel.TabIndex = 17;
            this.FadeInColorLabel.Text = "Fade from color";
            // 
            // FadeOutColorButton
            // 
            this.FadeOutColorButton.BackColor = System.Drawing.SystemColors.MenuText;
            this.FadeOutColorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.FadeOutColorButton.ForeColor = System.Drawing.SystemColors.ActiveBorder;
            this.FadeOutColorButton.Location = new System.Drawing.Point(122, 108);
            this.FadeOutColorButton.Name = "FadeOutColorButton";
            this.FadeOutColorButton.Size = new System.Drawing.Size(24, 23);
            this.FadeOutColorButton.TabIndex = 16;
            this.FadeOutColorButton.TabStop = false;
            this.FadeOutColorButton.UseVisualStyleBackColor = false;
            this.FadeOutColorButton.Click += new System.EventHandler(this.FadeOutColorButton_Click);
            // 
            // FadeInColorButton
            // 
            this.FadeInColorButton.BackColor = System.Drawing.SystemColors.ControlText;
            this.FadeInColorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.FadeInColorButton.ForeColor = System.Drawing.SystemColors.ActiveBorder;
            this.FadeInColorButton.Location = new System.Drawing.Point(122, 79);
            this.FadeInColorButton.Name = "FadeInColorButton";
            this.FadeInColorButton.Size = new System.Drawing.Size(24, 23);
            this.FadeInColorButton.TabIndex = 15;
            this.FadeInColorButton.TabStop = false;
            this.FadeInColorButton.UseVisualStyleBackColor = false;
            this.FadeInColorButton.Click += new System.EventHandler(this.FadeInColorButton_Click);
            // 
            // FadeOutCheckBox
            // 
            this.FadeOutCheckBox.Checked = true;
            this.FadeOutCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.FadeOutCheckBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FadeOutCheckBox.Location = new System.Drawing.Point(6, 106);
            this.FadeOutCheckBox.Name = "FadeOutCheckBox";
            this.FadeOutCheckBox.Size = new System.Drawing.Size(114, 24);
            this.FadeOutCheckBox.TabIndex = 7;
            this.FadeOutCheckBox.TabStop = false;
            this.FadeOutCheckBox.Text = "Fade out at end";
            this.FadeOutCheckBox.CheckedChanged += new System.EventHandler(this.FadeOutCheckBox_CheckedChanged);
            // 
            // FadeInCheckBox
            // 
            this.FadeInCheckBox.Checked = true;
            this.FadeInCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.FadeInCheckBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FadeInCheckBox.Location = new System.Drawing.Point(6, 78);
            this.FadeInCheckBox.Name = "FadeInCheckBox";
            this.FadeInCheckBox.Size = new System.Drawing.Size(104, 24);
            this.FadeInCheckBox.TabIndex = 6;
            this.FadeInCheckBox.TabStop = false;
            this.FadeInCheckBox.Text = "Fade in at start";
            this.FadeInCheckBox.CheckedChanged += new System.EventHandler(this.FadeInCheckBox_CheckedChanged);
            // 
            // mDoneAdvaneOptionsButton
            // 
            this.mDoneAdvaneOptionsButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mDoneAdvaneOptionsButton.Location = new System.Drawing.Point(253, 255);
            this.mDoneAdvaneOptionsButton.Name = "mDoneAdvaneOptionsButton";
            this.mDoneAdvaneOptionsButton.Size = new System.Drawing.Size(75, 23);
            this.mDoneAdvaneOptionsButton.TabIndex = 1;
            this.mDoneAdvaneOptionsButton.Text = "Done";
            this.mDoneAdvaneOptionsButton.Click += new System.EventHandler(this.mDoneAdvaneOptionsButton_Click);
            // 
            // mSlideshowOptionsTabControl
            // 
            this.mSlideshowOptionsTabControl.Controls.Add(this.mSlideshowOptionsTabPage);
            this.mSlideshowOptionsTabControl.Controls.Add(this.mAudioOptionsTabPage);
            this.mSlideshowOptionsTabControl.Controls.Add(this.mChapterMarkersTabPage);
            this.mSlideshowOptionsTabControl.Location = new System.Drawing.Point(6, 6);
            this.mSlideshowOptionsTabControl.Name = "mSlideshowOptionsTabControl";
            this.mSlideshowOptionsTabControl.SelectedIndex = 0;
            this.mSlideshowOptionsTabControl.Size = new System.Drawing.Size(326, 243);
            this.mSlideshowOptionsTabControl.TabIndex = 6;
            // 
            // mSlideshowOptionsTabPage
            // 
            this.mSlideshowOptionsTabPage.BackColor = System.Drawing.Color.White;
            this.mSlideshowOptionsTabPage.Controls.Add(this.mDisiablePanZoomOnAllSlidesButton);
            this.mSlideshowOptionsTabPage.Controls.Add(this.mEnablePanZoomOnAllSlidesButton);
            this.mSlideshowOptionsTabPage.Controls.Add(this.label6);
            this.mSlideshowOptionsTabPage.Controls.Add(this.ApplyTimeToAllSlidesButton);
            this.mSlideshowOptionsTabPage.Controls.Add(this.SyncSlideShowButton);
            this.mSlideshowOptionsTabPage.Controls.Add(this.DefaultSlideTimeLabel);
            this.mSlideshowOptionsTabPage.Controls.Add(this.FadeInCheckBox);
            this.mSlideshowOptionsTabPage.Controls.Add(this.DefaultTimeCombo);
            this.mSlideshowOptionsTabPage.Controls.Add(this.FadeOutCheckBox);
            this.mSlideshowOptionsTabPage.Controls.Add(this.FadeOutColorLabel);
            this.mSlideshowOptionsTabPage.Controls.Add(this.FadeInColorButton);
            this.mSlideshowOptionsTabPage.Controls.Add(this.FadeInColorLabel);
            this.mSlideshowOptionsTabPage.Controls.Add(this.FadeOutColorButton);
            this.mSlideshowOptionsTabPage.Location = new System.Drawing.Point(4, 22);
            this.mSlideshowOptionsTabPage.Name = "mSlideshowOptionsTabPage";
            this.mSlideshowOptionsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.mSlideshowOptionsTabPage.Size = new System.Drawing.Size(318, 217);
            this.mSlideshowOptionsTabPage.TabIndex = 0;
            this.mSlideshowOptionsTabPage.Text = "Slideshow";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(150, 41);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(12, 13);
            this.label6.TabIndex = 23;
            this.label6.Text = "s";
            // 
            // mAudioOptionsTabPage
            // 
            this.mAudioOptionsTabPage.BackColor = System.Drawing.Color.White;
            this.mAudioOptionsTabPage.Controls.Add(this.NarrationMasterVolumeTrackBar);
            this.mAudioOptionsTabPage.Controls.Add(this.LoopMusicCheckBox);
            this.mAudioOptionsTabPage.Controls.Add(this.MusicMasterVolumeTrackBar);
            this.mAudioOptionsTabPage.Controls.Add(this.FadeInMusiCheckBox);
            this.mAudioOptionsTabPage.Controls.Add(this.VideoMasterVolumeTrackBar);
            this.mAudioOptionsTabPage.Controls.Add(this.FadeOutMusiCheckBox);
            this.mAudioOptionsTabPage.Controls.Add(this.label4);
            this.mAudioOptionsTabPage.Controls.Add(this.label1);
            this.mAudioOptionsTabPage.Controls.Add(this.label3);
            this.mAudioOptionsTabPage.Controls.Add(this.label2);
            this.mAudioOptionsTabPage.Location = new System.Drawing.Point(4, 22);
            this.mAudioOptionsTabPage.Name = "mAudioOptionsTabPage";
            this.mAudioOptionsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.mAudioOptionsTabPage.Size = new System.Drawing.Size(318, 217);
            this.mAudioOptionsTabPage.TabIndex = 1;
            this.mAudioOptionsTabPage.Text = "Music/Audio";
            // 
            // mChapterMarkersTabPage
            // 
            this.mChapterMarkersTabPage.BackColor = System.Drawing.Color.White;
            this.mChapterMarkersTabPage.Controls.Add(this.mIncludeSlideshowMarkersCheckBox);
            this.mChapterMarkersTabPage.Controls.Add(this.mChapterMarkerMinutesLabel);
            this.mChapterMarkersTabPage.Controls.Add(this.mChapterMarkersTimedIntervalComboBox);
            this.mChapterMarkersTabPage.Controls.Add(this.mSlideChapterMarkersRadioButton);
            this.mChapterMarkersTabPage.Controls.Add(this.mTimedChapterMarkersRadioButton);
            this.mChapterMarkersTabPage.Location = new System.Drawing.Point(4, 22);
            this.mChapterMarkersTabPage.Name = "mChapterMarkersTabPage";
            this.mChapterMarkersTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.mChapterMarkersTabPage.Size = new System.Drawing.Size(318, 217);
            this.mChapterMarkersTabPage.TabIndex = 2;
            this.mChapterMarkersTabPage.Text = "Chapter markers";
            // 
            // mIncludeSlideshowMarkersCheckBox
            // 
            this.mIncludeSlideshowMarkersCheckBox.Checked = true;
            this.mIncludeSlideshowMarkersCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mIncludeSlideshowMarkersCheckBox.Location = new System.Drawing.Point(6, 6);
            this.mIncludeSlideshowMarkersCheckBox.Name = "mIncludeSlideshowMarkersCheckBox";
            this.mIncludeSlideshowMarkersCheckBox.Size = new System.Drawing.Size(292, 33);
            this.mIncludeSlideshowMarkersCheckBox.TabIndex = 5;
            this.mIncludeSlideshowMarkersCheckBox.Text = "Add chapter markers on the output disk for this slideshow.";
            this.mIncludeSlideshowMarkersCheckBox.UseVisualStyleBackColor = true;
            this.mIncludeSlideshowMarkersCheckBox.CheckedChanged += new System.EventHandler(this.mIncludeSlideshowMarkersTextBox_CheckedChanged);
            // 
            // mChapterMarkerMinutesLabel
            // 
            this.mChapterMarkerMinutesLabel.AutoSize = true;
            this.mChapterMarkerMinutesLabel.Location = new System.Drawing.Point(153, 56);
            this.mChapterMarkerMinutesLabel.Name = "mChapterMarkerMinutesLabel";
            this.mChapterMarkerMinutesLabel.Size = new System.Drawing.Size(51, 13);
            this.mChapterMarkerMinutesLabel.TabIndex = 3;
            this.mChapterMarkerMinutesLabel.Text = "minutes.";
            // 
            // mChapterMarkersTimedIntervalComboBox
            // 
            this.mChapterMarkersTimedIntervalComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mChapterMarkersTimedIntervalComboBox.FormattingEnabled = true;
            this.mChapterMarkersTimedIntervalComboBox.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "5",
            "10",
            "15",
            "20",
            "25",
            "30",
            "45",
            "60"});
            this.mChapterMarkersTimedIntervalComboBox.Location = new System.Drawing.Point(105, 53);
            this.mChapterMarkersTimedIntervalComboBox.Name = "mChapterMarkersTimedIntervalComboBox";
            this.mChapterMarkersTimedIntervalComboBox.Size = new System.Drawing.Size(40, 21);
            this.mChapterMarkersTimedIntervalComboBox.TabIndex = 2;
            this.mChapterMarkersTimedIntervalComboBox.SelectedValueChanged += new System.EventHandler(this.mChapterMarkersTimedIntervalComboBox_SelectedValueChanged);
            // 
            // mSlideChapterMarkersRadioButton
            // 
            this.mSlideChapterMarkersRadioButton.Location = new System.Drawing.Point(6, 80);
            this.mSlideChapterMarkersRadioButton.Name = "mSlideChapterMarkersRadioButton";
            this.mSlideChapterMarkersRadioButton.Size = new System.Drawing.Size(292, 36);
            this.mSlideChapterMarkersRadioButton.TabIndex = 1;
            this.mSlideChapterMarkersRadioButton.Text = "Chapters are set individually from slides in the storyboard.";
            this.mSlideChapterMarkersRadioButton.UseVisualStyleBackColor = true;
            this.mSlideChapterMarkersRadioButton.CheckedChanged += new System.EventHandler(this.mSlideChapterMarkersRadioButton_CheckedChanged);
            // 
            // mTimedChapterMarkersRadioButton
            // 
            this.mTimedChapterMarkersRadioButton.AutoSize = true;
            this.mTimedChapterMarkersRadioButton.Checked = true;
            this.mTimedChapterMarkersRadioButton.Location = new System.Drawing.Point(6, 54);
            this.mTimedChapterMarkersRadioButton.Name = "mTimedChapterMarkersRadioButton";
            this.mTimedChapterMarkersRadioButton.Size = new System.Drawing.Size(95, 17);
            this.mTimedChapterMarkersRadioButton.TabIndex = 0;
            this.mTimedChapterMarkersRadioButton.TabStop = true;
            this.mTimedChapterMarkersRadioButton.Text = "Chapter every";
            this.mTimedChapterMarkersRadioButton.UseVisualStyleBackColor = true;
            this.mTimedChapterMarkersRadioButton.CheckedChanged += new System.EventHandler(this.mTimedChapterMarkersRadioButton_CheckedChanged);
            // 
            // mEnablePanZoomOnAllSlidesButton
            // 
            this.mEnablePanZoomOnAllSlidesButton.Location = new System.Drawing.Point(6, 151);
            this.mEnablePanZoomOnAllSlidesButton.Name = "mEnablePanZoomOnAllSlidesButton";
            this.mEnablePanZoomOnAllSlidesButton.Size = new System.Drawing.Size(192, 23);
            this.mEnablePanZoomOnAllSlidesButton.TabIndex = 24;
            this.mEnablePanZoomOnAllSlidesButton.Text = "Enable pan/zoom on all slides";
            this.mEnablePanZoomOnAllSlidesButton.UseVisualStyleBackColor = true;
            this.mEnablePanZoomOnAllSlidesButton.Click += new System.EventHandler(this.mEnablePanZoomOnAllSlidesButton_Click);
            // 
            // mDisiablePanZoomOnAllSlidesButton
            // 
            this.mDisiablePanZoomOnAllSlidesButton.Location = new System.Drawing.Point(6, 180);
            this.mDisiablePanZoomOnAllSlidesButton.Name = "mDisiablePanZoomOnAllSlidesButton";
            this.mDisiablePanZoomOnAllSlidesButton.Size = new System.Drawing.Size(192, 23);
            this.mDisiablePanZoomOnAllSlidesButton.TabIndex = 25;
            this.mDisiablePanZoomOnAllSlidesButton.Text = "Disable pan/zoom on all slides";
            this.mDisiablePanZoomOnAllSlidesButton.UseVisualStyleBackColor = true;
            this.mDisiablePanZoomOnAllSlidesButton.Click += new System.EventHandler(this.mDisiablePanZoomOnAllSlidesButton_Click);
            // 
            // SlideShowAdvanceOptions
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(336, 283);
            this.ControlBox = false;
            this.Controls.Add(this.mSlideshowOptionsTabControl);
            this.Controls.Add(this.mDoneAdvaneOptionsButton);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SlideShowAdvanceOptions";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Slideshow settings";
            ((System.ComponentModel.ISupportInitialize)(this.NarrationMasterVolumeTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MusicMasterVolumeTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.VideoMasterVolumeTrackBar)).EndInit();
            this.mSlideshowOptionsTabControl.ResumeLayout(false);
            this.mSlideshowOptionsTabPage.ResumeLayout(false);
            this.mSlideshowOptionsTabPage.PerformLayout();
            this.mAudioOptionsTabPage.ResumeLayout(false);
            this.mAudioOptionsTabPage.PerformLayout();
            this.mChapterMarkersTabPage.ResumeLayout(false);
            this.mChapterMarkersTabPage.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion
        //*******************************************************************
        private void mDoneAdvaneOptionsButton_Click(object sender, System.EventArgs e)
        {
            if (mMementoProjectWhenDone == true)
            {
                if (mMementoMessage == "")
                {
                    CGlobals.mCurrentProject.DeclareChange(true, "Advance options change");
                }
                else
                {
                    CGlobals.mCurrentProject.DeclareChange(true, mMementoMessage);
                }
            }
            Close();
        }

        //*******************************************************************
        private void SetValuesToMatchSlideshow()
        {
            mRebuilding = true;
            try
            {
                CSlideShow ss = GetCurrentSlideShow();
                float current_value = ss.mDefaultSlide.DisplayLength;
                this.DefaultTimeCombo.Text = current_value.ToString();

                bool sync = ss.SyncLengthToMusic;
                this.SyncSlideShowButton.Checked = sync;

                this.FadeInColorButton.BackColor = ss.FadeInColor;
                this.FadeOutColorButton.BackColor = ss.FadeOutColor;

                this.FadeInCheckBox.Checked = ss.FadeIn;
                this.FadeOutCheckBox.Checked = ss.FadeOut;

                this.LoopMusicCheckBox.Checked = ss.LoopMusic;
                this.FadeInMusiCheckBox.Checked = ss.FadeInAudio;
                this.FadeOutMusiCheckBox.Checked = ss.FadeOutAudio;

                int vidVol = (int)((ss.VideoAudioVolume * 100.0f) + 0.4999f);
                if (vidVol < 0) vidVol = 0;
                if (vidVol > 100) vidVol = 100;
                this.VideoMasterVolumeTrackBar.Value = vidVol;

                int musVol = (int)((ss.BackgroundMusicVolume * 100.0f) + 0.4999f);
                if (musVol < 0) musVol = 0;
                if (musVol > 100) musVol = 100;
                this.MusicMasterVolumeTrackBar.Value = musVol;

                int narVol = (int)((ss.NarrationVolume * 100.0f) + 0.4999f);
                if (narVol < 0) narVol = 0;
                if (narVol > 100) narVol = 100;
                this.NarrationMasterVolumeTrackBar.Value = narVol;

                if (ss.ChapterMarkersTypeToUse == CSlideShow.ChapterMarkersType.None)
                {
                    mIncludeSlideshowMarkersCheckBox.Checked = false;
                }
                else
                {
                    if (ss.ChapterMarkersTypeToUse == CSlideShow.ChapterMarkersType.RegularTimedInterval)
                    {
                        mTimedChapterMarkersRadioButton.Checked = true;
                    }
                    else
                    {
                        mSlideChapterMarkersRadioButton.Checked = true;
                    }
                }
          
                DateTime dt = ss.ChapterMarkerTimeInterval;
                int minutes = 5;
                if (dt.Hour == 1)
                {
                    minutes = 60;
                }
                else
                {
                    minutes = dt.Minute;
                }

                mChapterMarkersTimedIntervalComboBox.Text = minutes.ToString();

                UpdatePanZoomEnableOnAllButtons();

                SetHideUnhideStuff();
            }
            finally
            {
                mRebuilding = false;
            }
        }

        //*******************************************************************
        private void UpdatePanZoomEnableOnAllButtons()
        {
            CSlideShow ss = GetCurrentSlideShow();

            bool panZoomEnabledOnAll = true;
            bool panZoomDisabledOnAll = true;
            ArrayList slides = ss.mSlides;
            for (int i = 0; i < slides.Count; i++)
            {
                CSlide s = slides[i] as CSlide;
                if (s.UsePanZoom == true)
                {
                    panZoomDisabledOnAll = false;
                }
                else
                {
                    panZoomEnabledOnAll = false;
                }
                if (panZoomEnabledOnAll == false && panZoomDisabledOnAll == false)
                {
                    break;
                }
            }

             mEnablePanZoomOnAllSlidesButton.Enabled = !panZoomEnabledOnAll;
             mDisiablePanZoomOnAllSlidesButton.Enabled = !panZoomDisabledOnAll;
        }

        //*******************************************************************
        private void SetHideUnhideStuff()
        {
            CSlideShow ss = GetCurrentSlideShow();

            this.FadeOutCheckBox.Enabled = true;
            this.FadeInCheckBox.Enabled = true;
            this.FadeInMusiCheckBox.Enabled = true;
            this.FadeOutMusiCheckBox.Enabled = true;

            if (Form1.mMainForm.GetSlideShowManager().IsPlaying() == true)
            {
                this.DefaultTimeCombo.Enabled = false;
                this.ApplyTimeToAllSlidesButton.Enabled = false;
                this.DefaultSlideTimeLabel.Enabled = false;
                this.LoopMusicCheckBox.Enabled = false;
                this.ApplyTimeToAllSlidesButton.Enabled = false;
                this.SyncSlideShowButton.Enabled = false;
                this.FadeInColorButton.Enabled = false;
                this.FadeInColorLabel.Enabled = false;
                this.FadeOutColorButton.Enabled = false;
                this.FadeOutColorLabel.Enabled = false;
                this.FadeOutCheckBox.Enabled = false;
                this.FadeInCheckBox.Enabled = false;
                this.FadeInMusiCheckBox.Enabled = false;
                this.FadeOutMusiCheckBox.Enabled = false;

                return;
            }


            if (this.SyncSlideShowButton.Checked == true)
            {
                this.DefaultTimeCombo.Enabled = false;
                this.ApplyTimeToAllSlidesButton.Enabled = false;
                this.DefaultSlideTimeLabel.Enabled = false;
                this.LoopMusicCheckBox.Enabled = false;
            }
            else
            {
                this.DefaultTimeCombo.Enabled = true;
                this.ApplyTimeToAllSlidesButton.Enabled = true;
                this.DefaultSlideTimeLabel.Enabled = true;
                this.LoopMusicCheckBox.Enabled = true;
            }

            if (ss.mSlides.Count == 0)
            {
                this.ApplyTimeToAllSlidesButton.Enabled = false;
                this.SyncSlideShowButton.Enabled = false;
            }
            else
            {
                this.SyncSlideShowButton.Enabled = true;
            }

            if (ss.FadeIn == true)
            {
                this.FadeInColorButton.Enabled = true;
                this.FadeInColorLabel.Enabled = true;
            }
            else
            {
                this.FadeInColorButton.Enabled = false;
                this.FadeInColorLabel.Enabled = false;
            }

            if (ss.FadeOut == true)
            {
                this.FadeOutColorButton.Enabled = true;
                this.FadeOutColorLabel.Enabled = true;
            }
            else
            {
                this.FadeOutColorButton.Enabled = false;
                this.FadeOutColorLabel.Enabled = false;
            }

            if (mIncludeSlideshowMarkersCheckBox.Checked == true)
            {
                mTimedChapterMarkersRadioButton.Enabled = true;
                mSlideChapterMarkersRadioButton.Enabled = true;

                if (ss.ChapterMarkersTypeToUse == CSlideShow.ChapterMarkersType.RegularTimedInterval)
                {
                    mChapterMarkersTimedIntervalComboBox.Enabled = true;
                }
                else
                {
                    mChapterMarkersTimedIntervalComboBox.Enabled = false;
                }

                mChapterMarkerMinutesLabel.Enabled = true;
            }
            else
            {
                mTimedChapterMarkersRadioButton.Enabled = false;
                mSlideChapterMarkersRadioButton.Enabled = false;
                mChapterMarkersTimedIntervalComboBox.Enabled = false;
                mChapterMarkerMinutesLabel.Enabled = false;
            }
        }

        //*******************************************************************
        private CSlideShow GetCurrentSlideShow()
        {
            return Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow;
        }

        //*******************************************************************
        private void SyncSlideShowButton_CheckedChanged(object sender, System.EventArgs e)
        {
            if (mAlreadyInSyncChangeCallback == true) return;

            mAlreadyInSyncChangeCallback = true;

            // ok try and set the slideshow
            if (GetCurrentSlideShow().IsValidToSyncroniseToMusic() == true ||
                this.SyncSlideShowButton.Checked == false)
            {
                if (this.SyncSlideShowButton.Checked != GetCurrentSlideShow().SyncLengthToMusic)
                {
                    mMementoProjectWhenDone = true;
                }
            }

            GetCurrentSlideShow().SetSyncToSlideshowWithoutMemento(this.SyncSlideShowButton.Checked);

            // force the slideshow to recalc display lenth
            GetCurrentSlideShow().GetLengthInSeconds();

            Form1.mMainForm.GetSlideShowManager().RebuildPanel(null);

            this.SyncSlideShowButton.Checked = GetCurrentSlideShow().SyncLengthToMusic;
            SetHideUnhideStuff();
            mAlreadyInSyncChangeCallback = false;

        }

        //*******************************************************************
        private void ApplyTimeToAllSlidesButton_Click(object sender, System.EventArgs e)
        {
            float current_value = GetCurrentSlideShow().mDefaultSlide.DisplayLength;

            CSlideShow ss = GetCurrentSlideShow();

            foreach (CSlide sl in ss.mSlides)
            {
                CStillPictureSlide sps = sl as CStillPictureSlide;
                if (sps != null)
                {
                    if (sps.DoesSlideContainSingleVideoOfSameLengthAsSlide() == false)
                    {
                        sps.SetLengthWithoutUpdate(current_value);
                    }
                }
            }

            ss.InValidateLength();
            ss.CalcLengthOfAllSlides();
            ss.ReCalcLoopMusicSlides();

            Form1.mMainForm.GetSlideShowManager().RebuildPanel(null);
            this.mMementoProjectWhenDone = true;
        }

        //*******************************************************************
        public void DefaultTimeComboChanged(object o, System.EventArgs e)
        {

            float current_value = GetCurrentSlideShow().mDefaultSlide.DisplayLength;

            float new_value = 8.0f;

            string i = this.DefaultTimeCombo.Text;

            bool valid = true;
            try
            {
                new_value = float.Parse(i, CultureInfo.InvariantCulture);
                if (new_value <= 0) valid = false;
            }
            catch (Exception)
            {
                valid = false;
            }

            if (valid == false)
            {

                DefaultTimeCombo.Text = current_value.ToString();
            }

            if (new_value != current_value)
            {
                GetCurrentSlideShow().mDefaultSlide.SetLengthWithoutUpdate(new_value);
                mMementoProjectWhenDone = true;
            }
        }


        //*******************************************************************
        public void DefaultTimeComboIndexChnaged(object o, System.EventArgs e)
        {
            this.ApplyTimeToAllSlidesButton.Focus();
        }


        //*******************************************************************
        private void FadeInColorButton_Click(object sender, System.EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowDialog();
            if (this.FadeInColorButton.BackColor != cd.Color)
            {
                this.FadeInColorButton.BackColor = cd.Color;
                this.GetCurrentSlideShow().FadeInColor = cd.Color;
                this.mMementoProjectWhenDone = true;
            }

        }

        //*******************************************************************
        private void FadeOutColorButton_Click(object sender, System.EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowDialog();
            if (this.FadeOutColorButton.BackColor != cd.Color)
            {
                this.FadeOutColorButton.BackColor = cd.Color;
                this.GetCurrentSlideShow().FadeOutColor = cd.Color;
                this.mMementoProjectWhenDone = true;
            }
        }


        //*******************************************************************
        private void FadeInCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            if (this.FadeInCheckBox.Checked != this.GetCurrentSlideShow().FadeIn)
            {
                this.GetCurrentSlideShow().FadeIn = this.FadeInCheckBox.Checked;
                this.mMementoProjectWhenDone = true;
                this.SetHideUnhideStuff();
            }
        }

        //*******************************************************************
        private void FadeOutCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            if (this.FadeOutCheckBox.Checked != this.GetCurrentSlideShow().FadeOut)
            {
                this.GetCurrentSlideShow().FadeOut = this.FadeOutCheckBox.Checked;
                this.mMementoProjectWhenDone = true;
                this.SetHideUnhideStuff();
            }
        }


        //*******************************************************************
        private void LoopMusicCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            CSlideShow ss = this.GetCurrentSlideShow();
            if (this.LoopMusicCheckBox.Checked != ss.LoopMusic)
            {
                ss.LoopMusic = this.LoopMusicCheckBox.Checked;
                Form1.mMainForm.GetSlideShowMusicManager().RebuildPanel();
                this.mMementoProjectWhenDone = true;
            }
        }

        //*******************************************************************
        private void FadeInMusiCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            CSlideShow ss = this.GetCurrentSlideShow();
            if (this.FadeInMusiCheckBox.Checked != ss.FadeInAudio)
            {
                ss.FadeInAudio = this.FadeInMusiCheckBox.Checked;
                Form1.mMainForm.GetSlideShowMusicManager().RebuildPanel();
                Form1.mMainForm.GetSlideShowNarrationManager().RebuildPanel();
                this.mMementoProjectWhenDone = true;
            }
        }

        //*******************************************************************
        private void FadeOutMusiCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            CSlideShow ss = this.GetCurrentSlideShow();
            if (this.FadeOutMusiCheckBox.Checked != ss.FadeOutAudio)
            {
                ss.FadeOutAudio = this.FadeOutMusiCheckBox.Checked;
                Form1.mMainForm.GetSlideShowMusicManager().RebuildPanel();
                Form1.mMainForm.GetSlideShowNarrationManager().RebuildPanel();
                this.mMementoProjectWhenDone = true;
            }
        }


        //*******************************************************************
        private void VideoMasterVolumeTrackBar_Scroll(object sender, System.EventArgs e)
        {
            CSlideShow ss = this.GetCurrentSlideShow();

            float vd = ss.VideoAudioVolume;
            float trackbarValue = ((float)this.VideoMasterVolumeTrackBar.Value) / 100.0f;

            if (vd != trackbarValue)
            {
                ss.VideoAudioVolume = trackbarValue;
                this.mMementoProjectWhenDone = true;
            }

        }

        //*******************************************************************
        private void MusicMasterVolumeTrackBar_Scroll(object sender, EventArgs e)
        {
            CSlideShow ss = this.GetCurrentSlideShow();

            float vd = ss.BackgroundMusicVolume;
            float trackbarValue = ((float)this.MusicMasterVolumeTrackBar.Value) / 100.0f;

            if (vd != trackbarValue)
            {
                ss.BackgroundMusicVolume = trackbarValue;
                this.mMementoProjectWhenDone = true;
            }
        }

        //*******************************************************************
        private void NarrationMasterVolumeTrackBar_Scroll(object sender, EventArgs e)
        {
            CSlideShow ss = this.GetCurrentSlideShow();

            float vd = ss.NarrationVolume;
            float trackbarValue = ((float)this.NarrationMasterVolumeTrackBar.Value) / 100.0f;

            if (vd != trackbarValue)
            {
                ss.NarrationVolume = trackbarValue;
                this.mMementoProjectWhenDone = true;
            }
        }

        //*******************************************************************
        private void mIncludeSlideshowMarkersTextBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mRebuilding == true)
            {
                return;
            }

            mCMThumbnailsNeedReValidating = true;

            CSlideShow ss = this.GetCurrentSlideShow();

            if (mIncludeSlideshowMarkersCheckBox.Checked == false)
            {
                ss.ChapterMarkersTypeToUse = CSlideShow.ChapterMarkersType.None;
                mMementoMessage = "Chapter markers turned off ";
                mMementoProjectWhenDone = true;

            }
            else
            {
                if (mTimedChapterMarkersRadioButton.Checked == true)
                {
                    mTimedChapterMarkersRadioButton_CheckedChanged(sender, e);
                }
                else
                {
                    mSlideChapterMarkersRadioButton_CheckedChanged(sender, e);
                }
            }

            SetHideUnhideStuff();
        }

        //*******************************************************************
        private void mTimedChapterMarkersRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (mRebuilding == true)
            {
                return;
            }

            mCMThumbnailsNeedReValidating = true;

            if (mTimedChapterMarkersRadioButton.Checked == true)
            {
                CSlideShow ss = this.GetCurrentSlideShow();
                ss.ChapterMarkersTypeToUse = CSlideShow.ChapterMarkersType.RegularTimedInterval;
                mChapterMarkersTimedIntervalComboBox_SelectedValueChanged(sender, e);
                mChapterMarkersTimedIntervalComboBox.Enabled = true;
                mMementoMessage = "Chapter markers set as timed interval";
                mMementoProjectWhenDone = true;
            }
        }


        //*******************************************************************
        private void mSlideChapterMarkersRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (mRebuilding == true)
            {
                return;
            }

            mCMThumbnailsNeedReValidating = true;

            if (mSlideChapterMarkersRadioButton.Checked == true)
            {
                CSlideShow ss = this.GetCurrentSlideShow();
                ss.ChapterMarkersTypeToUse = CSlideShow.ChapterMarkersType.SetFromSlides;
                mChapterMarkersTimedIntervalComboBox.Enabled = false;
                mMementoMessage = "Chapter markers set from slides";
                mMementoProjectWhenDone = true;
            }
        }

        //*******************************************************************
        private void mChapterMarkersTimedIntervalComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (mRebuilding == true)
            {
                return;
            }

            CSlideShow ss = this.GetCurrentSlideShow();
            int hours =0;
            int minutes = int.Parse(mChapterMarkersTimedIntervalComboBox.Text);
            if (minutes==60)
            {
                hours=1;
                minutes=0;
            }

            DateTime newInterval = new DateTime(2014, 1, 1, hours, minutes, 0);
            if (ss.ChapterMarkerTimeInterval != newInterval)
            {
                ss.ChapterMarkerTimeInterval = newInterval;
                mMementoMessage = "Chapter markers time interval changed";
                mMementoProjectWhenDone = true;
            }
        }

        //*******************************************************************
        private void mEnablePanZoomOnAllSlidesButton_Click(object sender, EventArgs e)
        {
            SetPanZoomOnSlides(true);
            UpdatePanZoomEnableOnAllButtons();
        }


        //*******************************************************************
        private void mDisiablePanZoomOnAllSlidesButton_Click(object sender, EventArgs e)
        {
            SetPanZoomOnSlides(false);
            UpdatePanZoomEnableOnAllButtons();
        }

        //*******************************************************************
        private void SetPanZoomOnSlides(bool value)
        {
            CSlideShow ss = this.GetCurrentSlideShow();

            Form1.mMainForm.GoToMainMenu();

            ss.mDefaultSlide.UsePanZoom = value;

            foreach (CSlide s in ss.mSlides)
            {
                s.UsePanZoom = value;
            }

            this.mMementoProjectWhenDone = true;

          
        }
    }
}