using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using DVDSlideshow;
using MangedToUnManagedWrapper;
using System.Management;
using System.Globalization;
using ManagedCore;

namespace dvdslideshowfontend
{
	/// <summary>
	/// Summary description for Form2.
	/// </summary>
	/// 



	public class Author : System.Windows.Forms.Form
	{
		private System.Windows.Forms.RadioButton NtscRadio;
		private System.Windows.Forms.RadioButton PalRadio;
		private System.Windows.Forms.CheckBox mCreateInFolderCheckBox;
		private System.Windows.Forms.Button Go;
		private System.Windows.Forms.TextBox EncodeCheckPointsTextBox;
		private System.Windows.Forms.ProgressBar mCreateProgressBar;
		private System.Windows.Forms.Label ElapsedTime;
		private System.Windows.Forms.Label EstimateTimeLeft;

		public System.Windows.Forms.Timer  mTimer ;
		public CProjectAuthor mPA;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox EncodeVideosCheckbox;
		private System.Windows.Forms.CheckBox EncodeMenuCheckBox;
		private System.Windows.Forms.CheckBox EncodeSlideShow1Checkbox;
		private System.Windows.Forms.CheckBox EncodeSlideShow2CheckBox;
		private System.Windows.Forms.CheckBox EnocodeSlideshow3Checkbox;
		private System.Windows.Forms.CheckBox EncodeBestQualityCheckbox;
		private System.Windows.Forms.CheckBox SubTitlesCheckBox;
		private System.Windows.Forms.CheckBox DVDFileStuctureCheckbox;
		private System.Windows.Forms.ComboBox mWriterComboBox;
		private System.Windows.Forms.Label mWriterLabel;

		public Form1	mMainWindow;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox WriterSpeedCombo;
		private System.Windows.Forms.CheckBox WriteToOpticalMediaCheckBox;
		private System.Windows.Forms.Label mVideoStandardLabel;
        private object folderBrowserExtenderProvider1;
        private object folderBrowserExtenderProvider2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton CreateNewVideoRadio;
		private System.Windows.Forms.RadioButton BurnFromFolderRadio;
		private System.Windows.Forms.TextBox mBurnFolderTestBox;
		private System.Windows.Forms.Label CurrentProcessLabel;
		private System.Windows.Forms.Label CurrentProcessTextBox;
		private System.Windows.Forms.ProgressBar mBufferFull;
		private System.Windows.Forms.Label mBufferLabel;
		private System.Windows.Forms.Label mVolumeNameLabel;
		private System.Windows.Forms.Button mCancelButtonWizard3;

		private OperationWindow mOperationWindow;


		private bool mCreatingVideo = false;
		private bool mAborted = false;
		private IStopWatch mTotalGoTime;
		private IStopWatch mTimeEstimationClock=null;

		private CVideoWriter mCurrentBurnWritter = null;
		private System.Windows.Forms.Label ElapsedTimeTextBox;
		private System.Windows.Forms.Label mETATextBox;
		public double mLastTotalSeconsLeft=-1;
		private TimeSpan mLastETACound;
		private TimeSpan mLastMajourChange;
		private TimeSpan mLargeTimeChangeSpan;
		private double	mPreBuiltBurnEstimateTime=0.0f;
		private System.Windows.Forms.TextBox mVolumeNameTextbox;
		private System.Windows.Forms.Label mDiskEstimateSizeTextBox;
		private System.Windows.Forms.CheckBox mIncludeOriginalVideosTickBox;
		private System.Windows.Forms.CheckBox mIncludeOriginalPicturesTickBox;

		private CGlobals.VideoType mCurrentBurnMode = CGlobals.VideoType.DVD;  // what thing we will burn
        private string mTheVideoPath = "";
		private bool mBurnCDOnly = false;	// applies to  project only!
		private float mBuildStructureTime=30.0f;
		private System.Windows.Forms.CheckBox limitVideoLengthCheckBox;
		private System.Windows.Forms.TextBox MaxVideoLengthTextBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.CheckBox mEncodeAudio;
		private System.Windows.Forms.Label NtscHelpLabel;
		private System.Windows.Forms.Label PalHelpLabel;
		private System.Windows.Forms.Button Minimisebutton;
		private System.Windows.Forms.TextBox mCreateInFolderTextBox;
		private System.Windows.Forms.Label mTotalCompleteLabel;
		private System.Windows.Forms.Label mTotalCompleteLabel2;

		private MangedToUnManagedWrapper.CManagedVideoDiskManager mDeviceManager= null;

		private System.Threading.Thread mAuthorThread=null;
		private System.Windows.Forms.CheckBox mTVCropCheckbox;
		private System.Windows.Forms.ComboBox mCroppingCombo;
		private System.Windows.Forms.ComboBox mCropCombo;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.CheckBox DontEncodeCheckbox;
		private System.Windows.Forms.CheckBox mIgnoreMenusTickBox;

		private bool mEmptyProject=false;
		private bool mDoPadding = false;
        public bool mDoingPaddingProcess = false;
		public bool mBurnReachFinalizeStage = false;
		private System.Windows.Forms.GroupBox mWizardGroup2;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Button mNextWizard1;
		private System.Windows.Forms.GroupBox mWizardGroup1;
		private System.Windows.Forms.Label mAuthoringOptionsLabel;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Button mPreviousWizard2;
		private System.Windows.Forms.Button mCancelWizard1;
		private System.Windows.Forms.Button mCancelWizard2;
		private System.Windows.Forms.Button mNextWizard2;
		private System.Windows.Forms.Button mPreviousWizard3;
		private System.Windows.Forms.GroupBox mWizardGroup3;
		private System.Windows.Forms.Label mAuthorLabel; 

		private System.Windows.Forms.PictureBox mDiskPictureBox;

		private ArrayList mWriterComboManagedDeviceList;
		private bool mDoingTimerTick = false;
        private CManagedVideoDisk mCurrentDevice;
        private string[] mDiskErrors = new string[34];
        private static bool mShownIncludeOriginalMediaWarning = false;
        private Button mRunExternalBurnToolButton;
        private RadioButton BurnPreviousProjectRadio;
        private RadioButton EncodeOnlyRadioButton;
        private ComboBox mResolutionComboBox;
        private Label mResolutionLabel;
        private bool mProjectEncoded = false;

        private int[] mBluRayOutput16by9WidthResolutions = { 1920, 1280, 426 }; // last resolution debug only
        private int[] mBluRayOutput16by9HeightResolutions = { 1080, 720, 240 }; // last resolution debug only

        // 4k
        //private int [] mVideoOutput16by9WidthResolutions = { 3840, 1920, 1600, 1366, 1280, 1136, 1024, 854, 640, 484, 432, 426  };
        //private int[] mVideoOutput16by9HeightResolutions = { 2160, 1080, 900,  768,  720,  640, 576, 480, 360, 272, 240, 240 };

        private int[] mVideoOutput16by9WidthResolutions = { 1920, 1600, 1366, 1280, 1136, 1024, 854, 640, 484, 432, 426 };
        private int[] mVideoOutput16by9HeightResolutions = { 1080, 900, 768, 720, 640, 576, 480, 360, 272, 240, 240 };



        private int[] mVideoOutput4by3WidthResolutions =  { 1600, 1280, 1024, 800, 640, 320 };
        private CheckBox mMotionBlurCheckBox;
        private int[] mVideoOutput4by3HeightResolutions = { 1200, 960,  768,  600, 480, 240 };
        private Button mMp4FileLocation;
        private PictureBox mWandPictureBox;
        private Button mShowImageCache;
        private Label mVideoQualityLabel;
        private ComboBox mVideoQualityComboBox;
        private ComboBox mVideoFPSComboBox;
        private Label mVideoFPSLabel;
        private Panel mVideoQualityPanel;

        private bool mMotionBlurAvailable = true;
        private bool mBurnPreviousTextChangedInternalChange = false;
        private ToolStrip mBurnFolderToolStrip;
        private ToolStripButton mBurnFolderToolStripButton;
        private ToolStrip mCreateInFolderToolStrip;
        private ToolStripButton mCreateInFolderToolStripButton;
        private ToolStrip mMotionBlurHelpToolStrip;
        private ToolStripButton mMotionBlurHelpToolStripButton;
        private CGlobals.VideoType mSelectedVideoFolderBurnType = CGlobals.VideoType.DVD;  // used when burning existing folder
        private Button mDebugShowVideoCache;
        private Button mDebugShowImageCache;
        private bool mInitializing = true;

		//*******************************************************************
		public Author(Form1 main_window, 
			          bool do_padding)
		{

            for (int i = 0; i < 10; i++)
            {
                mDiskErrors[i] = "Unknown burn error.";
            }
            mDiskErrors[10]= "Burning device is not available.";
            mDiskErrors[11]= "Burn failed due to failure to start up burner.";
            mDiskErrors[12]= "Burn failure due to not being able to create a UDF tree.";
            mDiskErrors[13]= "Burn failure due to not being able to create a UDF tree.";
            mDiskErrors[14]= "Burn failure due to not being able to create a UDF tree.";
            mDiskErrors[15]= "Burn failure due to not being able to create a UDF tree.";
            mDiskErrors[16]= "Burn failure due to not being able to create a UDF tree.";
            mDiskErrors[17]= "Burn failure due to not being able to create a UDF tree.";
            mDiskErrors[18]= "Burn failure due to not being able to find burner hardware device.";
            mDiskErrors[19]= "Burn failure due to not being able to enquire available burn modes from the hardware unit.";
            mDiskErrors[20]= "Burn failure as hardware does not support track at once mode.";
            mDiskErrors[21]= "Burn failure as the hardware unit was reported not to be ready.";
            mDiskErrors[22]= "Burn failure as was unable to determine if hardware unit had buffer underrun protection support.";
            mDiskErrors[23]= "Burn failure as was unable to switch on buffer underrun protection on the hardware unit.";
            mDiskErrors[24]= "Burn failure as was unable to determine write speeds for the hardware unit.";
            mDiskErrors[25]= "Burn failure as was unable to read track information from blank DVD media.";
            mDiskErrors[26]= "Burn failure as was unable to get disk information from the DVD blank media.";
            mDiskErrors[27]= "Burn failure as the image was too big for the blank DVD media.";
            mDiskErrors[28]= "Burn failure as hardware unit reported error when writting in track-at-once mode.";
            mDiskErrors[29]= "Burn problem as hardware unit reported error when closing the session, the disk still may be usable though.";
            mDiskErrors[30]= "Burn problem, key failure!";
            mDiskErrors[31] = "Failure when reading the DVD disk structure from the harddrive, unable to burn.";
            mDiskErrors[32] = "Failed as DVD image is likely to be too big.";
            mDiskErrors[33] = "Failed as Blu-ray image is likely to be too big for disc";

			mWriterComboManagedDeviceList = new ArrayList();

			mMainWindow = main_window;
			mDoPadding= do_padding;
			mBurnReachFinalizeStage=false;

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            this.folderBrowserExtenderProvider1 = new FolderBrowserExtenderProvider();
            this.folderBrowserExtenderProvider2 = new FolderBrowserExtenderProvider();

            ((FolderBrowserExtenderProvider)this.folderBrowserExtenderProvider1).FolderBrowserDialog.Description = "Find DVD video folder";
            ((FolderBrowserExtenderProvider)this.folderBrowserExtenderProvider2).FolderBrowserDialog.Description = "Find DVD video folder";
           

            Form1.ReduceFontSizeToMatchDPI(this.Controls);

         //   mBurnFolderToolStripButton.Click;

            ((FolderBrowserExtenderProvider)this.folderBrowserExtenderProvider1).SetBrowseButton(this.mCreateInFolderTextBox, this.mCreateInFolderToolStripButton);
            ((FolderBrowserExtenderProvider)this.folderBrowserExtenderProvider1).SetBrowseButton(this.mBurnFolderTestBox, this.mBurnFolderToolStripButton);
			((FolderBrowserExtenderProvider)this.folderBrowserExtenderProvider2).FolderBrowserDialog.Description = "Find CD/DVD/Blu-ray video folder";
            ((FolderBrowserExtenderProvider)this.folderBrowserExtenderProvider1).FolderBrowserDialog.SelectedPath = DefaultFolders.GetFolder("AuthoredFolder");
			for (int i=1;i<=30;i+=1)
			{
				string d = i.ToString();
				this.mCropCombo.Items.Add(d);
			}

			mCropCombo.Text="10";
			mCropCombo.Enabled=false;
			this.mTVCropCheckbox.Checked=false;
            CGlobals.mCurrentProject.DiskPreferences.FinalDiskCropPercent = 0;

            // 5 percent default for 16/9 and 0 percent for 2.21
            if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV16_9)
            {
                mCropCombo.Text = "5";
                CGlobals.mCurrentProject.DiskPreferences.FinalDiskCropPercent = 0;  // must do this, changing combo will reset FinalDiskCropPercent
            }

            if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV221_1)
            {
                mCropCombo.Text = "5";
                CGlobals.mCurrentProject.DiskPreferences.FinalDiskCropPercent = 0; // // must do this, changing combo will reset FinalDiskCropPercent
            }

			if (CGlobals.mIsDebug==true)
			{
                
				this.Height=352+168;
                this.Width = 610;
                mDebugShowVideoCache.Visible = true;
                mDebugShowImageCache.Visible  = true;
            }
			else
			{
				this.Height=352;
                this.Width = 610;
			}

			if (ManagedCore.CDebugLog.GetInstance().TraceEncode == false)
			{
				this.EncodeCheckPointsTextBox.Visible = false;
			}

            ArrayList slideshows = CGlobals.mCurrentProject.GetAllProjectSlideshows(false);

			// empty project
            if (slideshows.Count <= 1 &&((CSlideShow)slideshows[0]).mSlides.Count < 1)
			{
				mEmptyProject=true;
				this.CreateNewVideoRadio.Enabled=false;
                this.EncodeOnlyRadioButton.Enabled = false;
				this.BurnPreviousProjectRadio.Checked=true;
                this.label11.Text = "Burn a previously created slideshow or burn from a video folder.";
			}
            else if (slideshows.Count > 1)
            {
                string oldText ="slideshow";
                string newText = "slideshows";
                label11.Text = label11.Text.Replace(oldText, newText);
                CreateNewVideoRadio.Text = CreateNewVideoRadio.Text.Replace(oldText, newText);
                EncodeOnlyRadioButton.Text = EncodeOnlyRadioButton.Text.Replace(oldText, newText);
            }
			
			OutputToDiskRadio_CheckedChanged(this,new EventArgs());

			CGlobals.mEncodeAudio = this.mEncodeAudio.Checked;

            if (this.mMainWindow.mCurrentProject.DiskPreferences.OutputVideoType != CGlobals.VideoType.MP4)
            {
                if (CGlobals.mCurrentProject.DiskPreferences.TVType == CGlobals.OutputTVStandard.PAL)
                {
                    this.PalRadio.Checked = true;
                    CGlobals.mCurrentProject.DiskPreferences.SetToPAL();    // 100% ensure output fps correct
                }
                else
                {
                    CGlobals.mCurrentProject.DiskPreferences.SetToNTSC();  // 100% ensure output fps correct
                    if (this.NtscRadio != null)
                    {
                        this.NtscRadio.Checked = true;
                    }
                }
            }

            //
            // If not a mp4, then refresh device list (i.e. mp4 is the only non optical output)
            // 
            if (this.mMainWindow.mCurrentProject.DiskPreferences.OutputVideoType != CGlobals.VideoType.MP4)
            {
                RefreshWriterCombo();
            }
	
			DoHighLightUnHighlight();

			string vol_name =  System.IO.Path.GetFileNameWithoutExtension(this.mMainWindow.mCurrentProject.mName);

			// will cause callback
			this.mVolumeNameTextbox.Text =vol_name;
			mProjectNameTextbox_TextChanged(this, new EventArgs());

			this.mBurnFolderTestBox.TextChanged+=new EventHandler(this.BurnPreviousTextChanged);

            if ( this.mMainWindow.mCurrentProject.DiskPreferences.OutputVideoType != CGlobals.VideoType.MP4)
            {
                mWizardGroup3.Controls.Remove(mMotionBlurCheckBox);
                mWizardGroup3.Controls.Remove(mMotionBlurHelpToolStrip);
                mWizardGroup2.Controls.Add(mMotionBlurCheckBox);
                mWizardGroup2.Controls.Add(mMotionBlurHelpToolStrip);

                mMotionBlurCheckBox.Left = mTVCropCheckbox.Left;
                mMotionBlurCheckBox.Top = mTVCropCheckbox.Top - 20;
                mMotionBlurHelpToolStrip.Left = mMotionBlurCheckBox.Right;
                mMotionBlurHelpToolStrip.Top = mTVCropCheckbox.Top - 22;
            }

            if (this.mMainWindow.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.DVD)
            {
                mAuthorLabel.Text = "Author DVD video";
                mAuthoringOptionsLabel.Text = "Author DVD options";
            }
            else if (this.mMainWindow.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.BLURAY)
            {
                WriteToOpticalMediaCheckBox.Text = "Burn to Blu-ray";
                mBuildStructureTime = 10.0f;
                mAuthorLabel.Text = "Author Blu-ray video";
                mAuthoringOptionsLabel.Text = "Author Blu-ray options";

                mVideoStandardLabel.Visible = false;
                NtscRadio.Visible = false; 
                PalRadio.Visible = false;
                NtscHelpLabel.Visible = false;
                PalHelpLabel.Visible = false;
                mResolutionLabel.Visible = true;
                mResolutionComboBox.Visible = true;
                mVideoQualityPanel.Visible = true;

                mWizardGroup3.Controls.Remove(mResolutionLabel);
                mWizardGroup3.Controls.Remove(mResolutionComboBox);

                mWizardGroup2.Controls.Add(mResolutionLabel);
                mWizardGroup2.Controls.Add(mResolutionComboBox);
                mResolutionLabel.Location = new Point(mResolutionLabel.Location.X, mVideoStandardLabel.Location.Y+4);
                mResolutionComboBox.Location = new Point(mResolutionComboBox.Location.X, mVideoStandardLabel.Location.Y);

                this.Controls.Remove(mVideoQualityPanel);
                mWizardGroup2.Controls.Add(mVideoQualityPanel);
                mVideoQualityPanel.Location = new Point(223, mResolutionComboBox.Top);
                mVideoQualityPanel.Width += 10;
                mVideoQualityComboBox.SelectedIndex = 2;
                mVideoFPSComboBox.SelectedIndex = 5;
                mResolutionComboBox.SelectedIndexChanged += new EventHandler(BlurayResolutionSelectedIndexChanged);
                mVideoFPSComboBox.Items.Clear();
                mVideoFPSComboBox.Items.Add("24");
                mVideoFPSComboBox.Width += 22;            
            }
            else if (this.mMainWindow.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.MP4)
            {
                this.mAuthorLabel.Text = "Create MPEG-4 video";
                mAuthoringOptionsLabel.Text = "Authoring options";

                int top = this.mWizardGroup1.Top;
                mWizardGroup1.Top = this.mWizardGroup3.Top;
                this.mWizardGroup3.Top = top;

                mWriterComboBox.Visible = false;
                mWriterLabel.Text = "File";
                mWriterLabel.Width = mWriterLabel.Width + 10;
                mVolumeNameLabel.Visible = false;
                mVolumeNameTextbox.Visible = false;
                mDiskPictureBox.Visible = false;
                mResolutionLabel.Visible = true;
                mResolutionComboBox.Visible = true;
                mMp4FileLocation.Visible = true;
                WriteToOpticalMediaCheckBox.Checked = false;
                mIgnoreMenusTickBox.Checked = true;
                mWandPictureBox.Visible = true;
                mBufferFull.Visible = false;
                mBufferLabel.Visible = false;

                mWizardGroup2.Controls.Remove(mCreateInFolderTextBox);           
                mWizardGroup3.Controls.Add(mCreateInFolderTextBox);
             
                mCancelButtonWizard3.Left = mPreviousWizard3.Left;
                mPreviousWizard3.Visible = false;

                mCreateInFolderTextBox.Top = mWriterComboBox.Top;
                mCreateInFolderTextBox.Left = mResolutionComboBox.Left;
                mCreateInFolderTextBox.Width = mCreateInFolderToolStrip.Left - 10 - mCreateInFolderTextBox.Left;
                mMp4FileLocation.Top = mWriterComboBox.Top;
                mMp4FileLocation.Left = mCreateInFolderTextBox.Right + 5;

                this.Controls.Remove(mVideoQualityPanel);
                mWizardGroup3.Controls.Add(mVideoQualityPanel);
                mVideoQualityPanel.Location = new Point(223, mResolutionComboBox.Top);
                mVideoQualityComboBox.SelectedIndex = 2;
                mVideoFPSComboBox.SelectedIndex = 5;
            }

   
            // check if motion blur can be used on any slideshow

            CGlobals.mCurrentProject.DiskPreferences.TurnOffMotionBlur = false;

            bool mb_required = false;

            if (Encoder.HardwareSupportsMotionBlur == true)
            {
                foreach (CSlideShow ss in CGlobals.mCurrentProject.GetAllProjectSlideshows(true))
                {
                    int max_mb = ss.GetMaxNumberRequiredMotionBlurSubFrames();
                    if (max_mb > 1) mb_required = true;
                }
            }

            if (mb_required == false)
            {
                mMotionBlurCheckBox.Enabled = false;
                mMotionBlurCheckBox.Checked = false;
                this.mMotionBlurAvailable = false;
            }

			RecalcBurnType();
			RebuildComboStrings();
			DoHighLightUnHighlight();

            mInitializing = false;

           ReDrawDiskEstimationTextBox();
		}

        //*******************************************************************
        private void BlurayResolutionSelectedIndexChanged(object sender, EventArgs e)
        {
            if (mResolutionComboBox.SelectedIndex == 0)
            {
                mVideoFPSComboBox.Items.Clear();
                mVideoFPSComboBox.Items.Add("23.976");
                mVideoFPSComboBox.Items.Add("24");
                mVideoFPSComboBox.SelectedIndex = 1;

            }
            else
            {
                mVideoFPSComboBox.Items.Clear();
                mVideoFPSComboBox.Items.AddRange(new object[] {"23.976", "24", "50 PAL", "59.94 NTSC" });
                mVideoFPSComboBox.SelectedIndex = 1;
            }
        }

		//*******************************************************************
		// if we hit the go button will we burn? and what devices should be available
		private void RecalcBurnType()
		{
			CGlobals.VideoType previousBurnMode = mCurrentBurnMode ;
            CGlobals.VideoType newOutputType = this.mMainWindow.mCurrentProject.DiskPreferences.OutputVideoType;
        
            //
			// Are we not going to burn just create video?
            //
            if (BurnFromFolderRadio.Checked == true)
			{
				if (this.mBurnFolderTestBox.Text!="" &&
					this.mBurnFolderTestBox.Text!=null)
				{
                    newOutputType = mSelectedVideoFolderBurnType;
				}
			}
            //
            // Have we already selected a previous encoded project to brun?
            //
            else if (BurnPreviousProjectRadio.Checked == true)
            {
                newOutputType = mSelectedVideoFolderBurnType;
            }

            if (newOutputType == CGlobals.VideoType.BLURAY)
            {
                this.Text = "Author Blu-ray video";
                this.mAuthorLabel.Text = this.Text;
                mCurrentBurnMode = CGlobals.VideoType.BLURAY;
            }
            else if (newOutputType == CGlobals.VideoType.MP4)
            {
                this.Text = "Create MPEG-4 video";
                this.mAuthorLabel.Text = this.Text;
                mCurrentBurnMode = CGlobals.VideoType.MP4;
            }
            else if (newOutputType == CGlobals.VideoType.DVD)
			{
				this.Text ="Author DVD video";
				this.mAuthorLabel.Text="Author DVD video";
				mCurrentBurnMode = CGlobals.VideoType.DVD;
			}
			
			if (mCurrentBurnMode != previousBurnMode)
			{
				RebuildComboStrings();
				this.DoHighLightUnHighlight();
			}
		}

		//*******************************************************************
		private void ReDrawDiskEstimationTextBox()
		{
            if (this.mInitializing == true)
            {
                return;
            }

            bool do_pics = includeOriginalPictures();
            bool do_vids = includeOriginalVideos();
            long disksize = CDVDVideoWriter.DVDSizeInBytes;

            if (mCurrentBurnMode == CGlobals.VideoType.BLURAY)
            {
                disksize = CBluRayVideoWriter.BluRaySizeInBytes;
            }

            if ((do_pics || do_vids) && mShownIncludeOriginalMediaWarning == false)
            {
                if (mCurrentBurnMode == CGlobals.VideoType.DVD)
                {
                    DoMessageBoxOnFormThread("All original media files will be stored in 'OriginalFiles.zip' files located in the VIDEO_TS folder on the DVD\n\r\n\rIncluding the original media files as well as the DVD video will result in a non compliant DVD video disk. Although most hardware burners and DVD players should except it, it is not recommended to include them if you are making a DVD for the first time.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                if (mCurrentBurnMode == CGlobals.VideoType.BLURAY)
                {            
                    DoMessageBoxOnFormThread("All original media files will be stored in 'OriginalFiles.zip' files located in the root folder on the Blu-ray disc.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                mShownIncludeOriginalMediaWarning = true;
            }

            //
            // Make sure we save the gui preferences to the project before re-calculating disk size
            //
            SetProjectDiskPreferencesFromGui();

            long totalInBytes = GetEstimatedFinalEncodedDiskImage();
            double totalInMb = totalInBytes / 1024 / 1024;

            if (totalInBytes >= disksize)
            {
                this.mDiskEstimateSizeTextBox.ForeColor = Color.Red;
            }
            else
            {
                this.mDiskEstimateSizeTextBox.ForeColor = Color.Black;
            }

            this.mDiskEstimateSizeTextBox.Text = System.IO.Path.GetFileNameWithoutExtension(totalInMb.ToString("0.#") + "Mb");

		}

        //*******************************************************************
        public string DoesDirectoryContainsFolder(string root, string folder, int depth)
        {
            if (depth>=4)
            {
                return "";
            }
            
            string folderUpper = folder.ToUpper();
            String[] dirs = System.IO.Directory.GetDirectories(root);

            foreach (string dir in dirs)
            {
                string fn = System.IO.Path.GetFileName(dir);
                if (fn.ToUpper() == folderUpper)
                {
                    return root;
                }
            }

            foreach (string dir in dirs)
            {
                string path = DoesDirectoryContainsFolder(dir, folder, depth+1);
                if (path != "")
                {
                    return path;
                }
            }

            return "";
        }

      
		//*******************************************************************
		public void BurnPreviousTextChanged(object o, System.EventArgs e)
		{
            if (mBurnPreviousTextChangedInternalChange == true)
            {
                return;
            }

            //
            // This prevents recsive calls to this function
            //
            mBurnPreviousTextChangedInternalChange = true;

            try
            {

                this.DoHighLightUnHighlight();
                if (mBurnFolderTestBox.Text == null ||
                    mBurnFolderTestBox.Text == "")
                {
                    return;
                }

                bool isValidDirectory = false;
                mTheVideoPath = "";

                //
                // Test for containg VIDEO_TS folder (checks sub folders)
                //
                string path = DoesDirectoryContainsFolder(mBurnFolderTestBox.Text, "VIDEO_TS", 1);
                if (path != "")
                {
                    mBurnFolderTestBox.Text = path;
                    isValidDirectory = true;
                    mTheVideoPath = path;
                    mSelectedVideoFolderBurnType = CGlobals.VideoType.DVD;
                }
               

                if (isValidDirectory == false)
                {
                    //
                    // Test for containing BDMV fodler
                    //
                    path = DoesDirectoryContainsFolder(mBurnFolderTestBox.Text, "BDMV", 1);
                    if (path != "")
                    {
                        mBurnFolderTestBox.Text = path;
                        isValidDirectory = true;
                        mTheVideoPath = path;
                        mSelectedVideoFolderBurnType = CGlobals.VideoType.BLURAY;
                    }
                }

                if (isValidDirectory == false)
                {
                    string old_dir = mBurnFolderTestBox.Text;
                    mBurnFolderTestBox.Text = "";
                    DialogResult res = DoMessageBoxOnFormThread("The directory '" + old_dir + "' does not contain a DVD folder 'VIDEO_TS', Blu-ray folder 'BDMV'", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    RecalcBurnType();
                }
            }
            finally
            {
                mBurnPreviousTextChangedInternalChange = false;
                this.DoHighLightUnHighlight();
            }
		}

		//*******************************************************************	
		public void CreateDeviceManager()
		{
			BurnerProgressCallback pc = new BurnerProgressCallback(this);

			mDeviceManager = new  MangedToUnManagedWrapper.CManagedVideoDiskManager(pc);
		}


		//*******************************************************************
		private void RebuildComboStrings()
		{
			mWriterComboManagedDeviceList.Clear();

			mWriterComboBox.Items.Clear();

            if (mDeviceManager != null)
            {
                ArrayList al = mDeviceManager.GetVideoDiskDevices();

                foreach (MangedToUnManagedWrapper.CManagedVideoDisk device in al)
                {
                    // use this line to test what it's like not to have a DVD burner drive installed
                    //if (device.WritesDVD()==true) continue;// srg hack

                    if (device.WritesDVD() == true)                     // if dvd/blu-ray device allow drive always
                    {
                        //
                        // If device writes DVD (and is not a blu-ray drive) and we are burning a DVD, then add the item to the start of the combo
                        //
                        if (mCurrentBurnMode == CGlobals.VideoType.DVD && device.WritesDVD() == true && device.IsSuspectedBluRayDrive() == false)
                        {
                            mWriterComboBox.Items.Insert(0, device.Name());
                            mWriterComboManagedDeviceList.Insert(0, device);
                        }
                        //
                        // else if a suspected blu-ray drive and we are burning a blu-ray disc, then add item to start of combo
                        //
                        else if (mCurrentBurnMode == CGlobals.VideoType.BLURAY && device.IsSuspectedBluRayDrive() == true)
                        {
                            mWriterComboBox.Items.Insert(0, device.Name());
                            mWriterComboManagedDeviceList.Insert(0, device);
                        }
                        //
                        // else add the device to end of combo
                        //
                        else
                        {
                            mWriterComboBox.Items.Add(device.Name());
                            mWriterComboManagedDeviceList.Add(device);
                        }
                    }
                }

                // set us so we're the first item on the combo
                if (mWriterComboBox.Items.Count > 0)
                {
                    mWriterComboBox.SelectedIndex = 0;

                    // srG hack only speed is maximum!!??? ;)
                    if (WriterSpeedCombo.Items.Count == 0)
                    {
                        WriterSpeedCombo.Items.Add("Maximum");
                        WriterSpeedCombo.SelectedIndex = 0;
                    }
                }
            }

            if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.MP4)
            {
                mResolutionComboBox.Items.Clear();

                if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV16_9)
                {
                    // 4k
                    //mResolutionComboBox.Items.Add("3840 x 2160 Widescreen 4K");
                    mResolutionComboBox.Items.Add("1920 x 1080 Widescreen (HD 1080p)");
                    mResolutionComboBox.Items.Add("1600 x 900  Widescreen (HD)");
                    mResolutionComboBox.Items.Add("1366 x 768  Widescreen (HD)");
                    mResolutionComboBox.Items.Add("1280 x 720  Widescreen (HD 720p)");
                    mResolutionComboBox.Items.Add("1136 x 640  Widescreen (Mobile/Computer)");
                    mResolutionComboBox.Items.Add("1024 x 576  Widescreen (Mobile/Computer)");
                    mResolutionComboBox.Items.Add("854 x 480   Widescreen (Web/Mobile)");
                    mResolutionComboBox.Items.Add("640 x 360   Widescreen (Web/Mobile)");
                    mResolutionComboBox.Items.Add("484 x 272   Widescreen (Web/Mobile)");
                    mResolutionComboBox.Items.Add("432 x 240   Widescreen (Web/Mobile)");
                    mResolutionComboBox.Items.Add("426 x 240   Widescreen (Web/Mobile)");
                    mResolutionComboBox.SelectedIndex = 3;
                }
                else
                {
                    mResolutionComboBox.Items.Add("1600 x 1200 Standard");
                    mResolutionComboBox.Items.Add("1280 x 960 Standard");
                    mResolutionComboBox.Items.Add("1024 x 768 Standard");
                    mResolutionComboBox.Items.Add("800 x 600 Standard");
                    mResolutionComboBox.Items.Add("640 x 480 Standard");
                    mResolutionComboBox.Items.Add("320 x 240 Standard");
                    mResolutionComboBox.SelectedIndex = 3;
                }
            }
            else if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.BLURAY)
            {
                mResolutionComboBox.Items.Clear();

                // ### SRG to fix what about 4:3 outputs?
                mResolutionComboBox.Items.Add("HD 1080p (1920 x 1080)");
                mResolutionComboBox.Items.Add("HD 720p (1280 x 720)");

                //
                // This is used in debug builds to test blu-ray by allowing encoding at lower resolution which speeds up testing
                //
                if (CGlobals.mIsDebug == true)
                {
                    mResolutionComboBox.Items.Add("426 x 240   Widescreen (Web/Mobile)");
                }

                mResolutionComboBox.SelectedIndex = 0;
            }
		}

        //*******************************************************************
        private int EraseCurrentSelectDisk()
        {
            CManagedVideoDisk device = mCurrentDevice;
            if (device == null)
            {
                return 1;
            }
           
            return device.EraseDisk();
        }

        public delegate void ShowErasingOnGuiThreadDelegate();
        public void ShowErasingOnGuiThread()
        {
            mOperationWindow = new OperationWindow();//true);
            mOperationWindow.DoSetText("Erasing disk please wait....");
            mOperationWindow.ShowDialog(this);
        }

          
		//*******************************************************************
		private void DoActualDeviceRefresh(object o)
		{
			if (mDeviceManager==null) CreateDeviceManager();

			bool succeed = mDeviceManager.RebuildAvailbleVideoDevicesList();

            ArrayList al = mDeviceManager.GetVideoDiskDevices();     

            RebuildComboStrings();
			
			mOperationWindow.End();

		}

		//*******************************************************************
		public void RefreshWriterCombo()
		{

			mOperationWindow = new OperationWindow();//true);
			mOperationWindow.DoSetText("Scanning for devices....");
		
			System.Threading.ThreadPool.QueueUserWorkItem( new System.Threading.WaitCallback(DoActualDeviceRefresh ), null );
			mOperationWindow.ShowDialog();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// 
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Author));
            this.Go = new System.Windows.Forms.Button();
            this.EncodeCheckPointsTextBox = new System.Windows.Forms.TextBox();
            this.mCreateProgressBar = new System.Windows.Forms.ProgressBar();
            this.ElapsedTime = new System.Windows.Forms.Label();
            this.EstimateTimeLeft = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mShowImageCache = new System.Windows.Forms.Button();
            this.DontEncodeCheckbox = new System.Windows.Forms.CheckBox();
            this.mRunExternalBurnToolButton = new System.Windows.Forms.Button();
            this.mEncodeAudio = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.MaxVideoLengthTextBox = new System.Windows.Forms.TextBox();
            this.limitVideoLengthCheckBox = new System.Windows.Forms.CheckBox();
            this.SubTitlesCheckBox = new System.Windows.Forms.CheckBox();
            this.EncodeBestQualityCheckbox = new System.Windows.Forms.CheckBox();
            this.EnocodeSlideshow3Checkbox = new System.Windows.Forms.CheckBox();
            this.EncodeSlideShow2CheckBox = new System.Windows.Forms.CheckBox();
            this.EncodeSlideShow1Checkbox = new System.Windows.Forms.CheckBox();
            this.EncodeMenuCheckBox = new System.Windows.Forms.CheckBox();
            this.EncodeVideosCheckbox = new System.Windows.Forms.CheckBox();
            this.DVDFileStuctureCheckbox = new System.Windows.Forms.CheckBox();
            this.mWriterComboBox = new System.Windows.Forms.ComboBox();
            this.mWriterLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.WriterSpeedCombo = new System.Windows.Forms.ComboBox();
            this.mVolumeNameTextbox = new System.Windows.Forms.TextBox();
            this.mVolumeNameLabel = new System.Windows.Forms.Label();
            this.mCroppingCombo = new System.Windows.Forms.ComboBox();
            this.mWizardGroup3 = new System.Windows.Forms.GroupBox();
            this.mDebugShowImageCache = new System.Windows.Forms.Button();
            this.mDebugShowVideoCache = new System.Windows.Forms.Button();
            this.mMotionBlurHelpToolStrip = new System.Windows.Forms.ToolStrip();
            this.mMotionBlurHelpToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.mMp4FileLocation = new System.Windows.Forms.Button();
            this.mMotionBlurCheckBox = new System.Windows.Forms.CheckBox();
            this.mResolutionComboBox = new System.Windows.Forms.ComboBox();
            this.mResolutionLabel = new System.Windows.Forms.Label();
            this.mDiskPictureBox = new System.Windows.Forms.PictureBox();
            this.mTotalCompleteLabel2 = new System.Windows.Forms.Label();
            this.mTotalCompleteLabel = new System.Windows.Forms.Label();
            this.mETATextBox = new System.Windows.Forms.Label();
            this.ElapsedTimeTextBox = new System.Windows.Forms.Label();
            this.mBufferLabel = new System.Windows.Forms.Label();
            this.mBufferFull = new System.Windows.Forms.ProgressBar();
            this.CurrentProcessLabel = new System.Windows.Forms.Label();
            this.Minimisebutton = new System.Windows.Forms.Button();
            this.mPreviousWizard3 = new System.Windows.Forms.Button();
            this.mCancelButtonWizard3 = new System.Windows.Forms.Button();
            this.mAuthorLabel = new System.Windows.Forms.Label();
            this.CurrentProcessTextBox = new System.Windows.Forms.Label();
            this.mWizardGroup1 = new System.Windows.Forms.GroupBox();
            this.mBurnFolderToolStrip = new System.Windows.Forms.ToolStrip();
            this.mBurnFolderToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.EncodeOnlyRadioButton = new System.Windows.Forms.RadioButton();
            this.BurnPreviousProjectRadio = new System.Windows.Forms.RadioButton();
            this.label13 = new System.Windows.Forms.Label();
            this.mNextWizard1 = new System.Windows.Forms.Button();
            this.mCancelWizard1 = new System.Windows.Forms.Button();
            this.mBurnFolderTestBox = new System.Windows.Forms.TextBox();
            this.BurnFromFolderRadio = new System.Windows.Forms.RadioButton();
            this.CreateNewVideoRadio = new System.Windows.Forms.RadioButton();
            this.label11 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.mWandPictureBox = new System.Windows.Forms.PictureBox();
            this.mWizardGroup2 = new System.Windows.Forms.GroupBox();
            this.mCreateInFolderToolStrip = new System.Windows.Forms.ToolStrip();
            this.mCreateInFolderToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.mAuthoringOptionsLabel = new System.Windows.Forms.Label();
            this.mPreviousWizard2 = new System.Windows.Forms.Button();
            this.mNextWizard2 = new System.Windows.Forms.Button();
            this.mCancelWizard2 = new System.Windows.Forms.Button();
            this.mIgnoreMenusTickBox = new System.Windows.Forms.CheckBox();
            this.mIncludeOriginalVideosTickBox = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.mCropCombo = new System.Windows.Forms.ComboBox();
            this.mIncludeOriginalPicturesTickBox = new System.Windows.Forms.CheckBox();
            this.PalHelpLabel = new System.Windows.Forms.Label();
            this.PalRadio = new System.Windows.Forms.RadioButton();
            this.mTVCropCheckbox = new System.Windows.Forms.CheckBox();
            this.NtscHelpLabel = new System.Windows.Forms.Label();
            this.mCreateInFolderTextBox = new System.Windows.Forms.TextBox();
            this.mDiskEstimateSizeTextBox = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.mVideoStandardLabel = new System.Windows.Forms.Label();
            this.WriteToOpticalMediaCheckBox = new System.Windows.Forms.CheckBox();
            this.NtscRadio = new System.Windows.Forms.RadioButton();
            this.mCreateInFolderCheckBox = new System.Windows.Forms.CheckBox();
            this.mVideoQualityLabel = new System.Windows.Forms.Label();
            this.mVideoQualityComboBox = new System.Windows.Forms.ComboBox();
            this.mVideoFPSComboBox = new System.Windows.Forms.ComboBox();
            this.mVideoFPSLabel = new System.Windows.Forms.Label();
            this.mVideoQualityPanel = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.mWizardGroup3.SuspendLayout();
            this.mMotionBlurHelpToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mDiskPictureBox)).BeginInit();
            this.mWizardGroup1.SuspendLayout();
            this.mBurnFolderToolStrip.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mWandPictureBox)).BeginInit();
            this.mWizardGroup2.SuspendLayout();
            this.mCreateInFolderToolStrip.SuspendLayout();
            this.mVideoQualityPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // Go
            // 
            this.Go.BackColor = System.Drawing.SystemColors.Control;
            this.Go.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Go.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Go.Location = new System.Drawing.Point(488, 224);
            this.Go.Name = "Go";
            this.Go.Size = new System.Drawing.Size(88, 23);
            this.Go.TabIndex = 6;
            this.Go.Text = "Create";
            this.Go.UseVisualStyleBackColor = false;
            this.Go.Click += new System.EventHandler(this.Go_Click);
            // 
            // EncodeCheckPointsTextBox
            // 
            this.EncodeCheckPointsTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.EncodeCheckPointsTextBox.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EncodeCheckPointsTextBox.Location = new System.Drawing.Point(100, 224);
            this.EncodeCheckPointsTextBox.Name = "EncodeCheckPointsTextBox";
            this.EncodeCheckPointsTextBox.ReadOnly = true;
            this.EncodeCheckPointsTextBox.Size = new System.Drawing.Size(133, 20);
            this.EncodeCheckPointsTextBox.TabIndex = 50;
            // 
            // mCreateProgressBar
            // 
            this.mCreateProgressBar.Location = new System.Drawing.Point(8, 184);
            this.mCreateProgressBar.Name = "mCreateProgressBar";
            this.mCreateProgressBar.Size = new System.Drawing.Size(566, 23);
            this.mCreateProgressBar.TabIndex = 9;
            // 
            // ElapsedTime
            // 
            this.ElapsedTime.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ElapsedTime.Location = new System.Drawing.Point(8, 112);
            this.ElapsedTime.Name = "ElapsedTime";
            this.ElapsedTime.Size = new System.Drawing.Size(88, 23);
            this.ElapsedTime.TabIndex = 10;
            this.ElapsedTime.Text = "Elapsed time:";
            // 
            // EstimateTimeLeft
            // 
            this.EstimateTimeLeft.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EstimateTimeLeft.Location = new System.Drawing.Point(304, 112);
            this.EstimateTimeLeft.Name = "EstimateTimeLeft";
            this.EstimateTimeLeft.Size = new System.Drawing.Size(120, 23);
            this.EstimateTimeLeft.TabIndex = 11;
            this.EstimateTimeLeft.Text = "Estimated time left:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.mShowImageCache);
            this.groupBox1.Controls.Add(this.DontEncodeCheckbox);
            this.groupBox1.Controls.Add(this.mRunExternalBurnToolButton);
            this.groupBox1.Controls.Add(this.mEncodeAudio);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.MaxVideoLengthTextBox);
            this.groupBox1.Controls.Add(this.limitVideoLengthCheckBox);
            this.groupBox1.Controls.Add(this.SubTitlesCheckBox);
            this.groupBox1.Controls.Add(this.EncodeBestQualityCheckbox);
            this.groupBox1.Controls.Add(this.EnocodeSlideshow3Checkbox);
            this.groupBox1.Controls.Add(this.EncodeSlideShow2CheckBox);
            this.groupBox1.Controls.Add(this.EncodeSlideShow1Checkbox);
            this.groupBox1.Controls.Add(this.EncodeMenuCheckBox);
            this.groupBox1.Controls.Add(this.EncodeVideosCheckbox);
            this.groupBox1.Controls.Add(this.DVDFileStuctureCheckbox);
            this.groupBox1.Location = new System.Drawing.Point(6, 999);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(567, 168);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "DEBUG STUFF";
            // 
            // mShowImageCache
            // 
            this.mShowImageCache.Location = new System.Drawing.Point(461, 32);
            this.mShowImageCache.Name = "mShowImageCache";
            this.mShowImageCache.Size = new System.Drawing.Size(97, 35);
            this.mShowImageCache.TabIndex = 32;
            this.mShowImageCache.Text = "Show image cache";
            this.mShowImageCache.UseVisualStyleBackColor = true;
            this.mShowImageCache.Click += new System.EventHandler(this.mShowImageCache_Click);
            // 
            // DontEncodeCheckbox
            // 
            this.DontEncodeCheckbox.Location = new System.Drawing.Point(424, 128);
            this.DontEncodeCheckbox.Name = "DontEncodeCheckbox";
            this.DontEncodeCheckbox.Size = new System.Drawing.Size(104, 24);
            this.DontEncodeCheckbox.TabIndex = 12;
            this.DontEncodeCheckbox.Text = "dont encode";
            // 
            // mRunExternalBurnToolButton
            // 
            this.mRunExternalBurnToolButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mRunExternalBurnToolButton.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mRunExternalBurnToolButton.Location = new System.Drawing.Point(326, 73);
            this.mRunExternalBurnToolButton.Name = "mRunExternalBurnToolButton";
            this.mRunExternalBurnToolButton.Size = new System.Drawing.Size(176, 23);
            this.mRunExternalBurnToolButton.TabIndex = 31;
            this.mRunExternalBurnToolButton.Text = "Run external DVD folder burn tool";
            this.mRunExternalBurnToolButton.UseVisualStyleBackColor = true;
            this.mRunExternalBurnToolButton.Click += new System.EventHandler(this.mRunExternalBurnToolButton_Click);
            // 
            // mEncodeAudio
            // 
            this.mEncodeAudio.Checked = true;
            this.mEncodeAudio.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mEncodeAudio.Location = new System.Drawing.Point(336, 128);
            this.mEncodeAudio.Name = "mEncodeAudio";
            this.mEncodeAudio.Size = new System.Drawing.Size(64, 32);
            this.mEncodeAudio.TabIndex = 11;
            this.mEncodeAudio.Text = "Encode Audio";
            this.mEncodeAudio.CheckedChanged += new System.EventHandler(this.mEncodeAudio_CheckedChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(120, 136);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(144, 23);
            this.label5.TabIndex = 10;
            this.label5.Text = "Max video Lengh (seconds)";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MaxVideoLengthTextBox
            // 
            this.MaxVideoLengthTextBox.Location = new System.Drawing.Point(264, 136);
            this.MaxVideoLengthTextBox.Name = "MaxVideoLengthTextBox";
            this.MaxVideoLengthTextBox.Size = new System.Drawing.Size(48, 22);
            this.MaxVideoLengthTextBox.TabIndex = 9;
            this.MaxVideoLengthTextBox.Text = "0";
            this.MaxVideoLengthTextBox.TextChanged += new System.EventHandler(this.MaxVideoLengthTextBox_TextChanged);
            // 
            // limitVideoLengthCheckBox
            // 
            this.limitVideoLengthCheckBox.Location = new System.Drawing.Point(8, 136);
            this.limitVideoLengthCheckBox.Name = "limitVideoLengthCheckBox";
            this.limitVideoLengthCheckBox.Size = new System.Drawing.Size(112, 24);
            this.limitVideoLengthCheckBox.TabIndex = 8;
            this.limitVideoLengthCheckBox.Text = "Limit video length";
            this.limitVideoLengthCheckBox.CheckedChanged += new System.EventHandler(this.limitVideoLengthCheckBox_CheckedChanged);
            // 
            // SubTitlesCheckBox
            // 
            this.SubTitlesCheckBox.Checked = true;
            this.SubTitlesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SubTitlesCheckBox.Location = new System.Drawing.Point(8, 104);
            this.SubTitlesCheckBox.Name = "SubTitlesCheckBox";
            this.SubTitlesCheckBox.Size = new System.Drawing.Size(104, 24);
            this.SubTitlesCheckBox.TabIndex = 6;
            this.SubTitlesCheckBox.Text = "Add Subtitles";
            // 
            // EncodeBestQualityCheckbox
            // 
            this.EncodeBestQualityCheckbox.Checked = true;
            this.EncodeBestQualityCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.EncodeBestQualityCheckbox.Location = new System.Drawing.Point(8, 64);
            this.EncodeBestQualityCheckbox.Name = "EncodeBestQualityCheckbox";
            this.EncodeBestQualityCheckbox.Size = new System.Drawing.Size(216, 24);
            this.EncodeBestQualityCheckbox.TabIndex = 5;
            this.EncodeBestQualityCheckbox.Text = "Create best/retail quality (slower)";
            // 
            // EnocodeSlideshow3Checkbox
            // 
            this.EnocodeSlideshow3Checkbox.Checked = true;
            this.EnocodeSlideshow3Checkbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.EnocodeSlideshow3Checkbox.Location = new System.Drawing.Point(377, 21);
            this.EnocodeSlideshow3Checkbox.Name = "EnocodeSlideshow3Checkbox";
            this.EnocodeSlideshow3Checkbox.Size = new System.Drawing.Size(104, 32);
            this.EnocodeSlideshow3Checkbox.TabIndex = 4;
            this.EnocodeSlideshow3Checkbox.Text = "Encode Slideshow 3";
            // 
            // EncodeSlideShow2CheckBox
            // 
            this.EncodeSlideShow2CheckBox.Checked = true;
            this.EncodeSlideShow2CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.EncodeSlideShow2CheckBox.Location = new System.Drawing.Point(297, 20);
            this.EncodeSlideShow2CheckBox.Name = "EncodeSlideShow2CheckBox";
            this.EncodeSlideShow2CheckBox.Size = new System.Drawing.Size(88, 32);
            this.EncodeSlideShow2CheckBox.TabIndex = 3;
            this.EncodeSlideShow2CheckBox.Text = "Encode Slideshow 2";
            // 
            // EncodeSlideShow1Checkbox
            // 
            this.EncodeSlideShow1Checkbox.Checked = true;
            this.EncodeSlideShow1Checkbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.EncodeSlideShow1Checkbox.Location = new System.Drawing.Point(208, 24);
            this.EncodeSlideShow1Checkbox.Name = "EncodeSlideShow1Checkbox";
            this.EncodeSlideShow1Checkbox.Size = new System.Drawing.Size(88, 32);
            this.EncodeSlideShow1Checkbox.TabIndex = 2;
            this.EncodeSlideShow1Checkbox.Text = "Encode Slideshow 1";
            // 
            // EncodeMenuCheckBox
            // 
            this.EncodeMenuCheckBox.Checked = true;
            this.EncodeMenuCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.EncodeMenuCheckBox.Location = new System.Drawing.Point(112, 24);
            this.EncodeMenuCheckBox.Name = "EncodeMenuCheckBox";
            this.EncodeMenuCheckBox.Size = new System.Drawing.Size(104, 24);
            this.EncodeMenuCheckBox.TabIndex = 1;
            this.EncodeMenuCheckBox.Text = "Encode menu";
            // 
            // EncodeVideosCheckbox
            // 
            this.EncodeVideosCheckbox.Checked = true;
            this.EncodeVideosCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.EncodeVideosCheckbox.Location = new System.Drawing.Point(8, 24);
            this.EncodeVideosCheckbox.Name = "EncodeVideosCheckbox";
            this.EncodeVideosCheckbox.Size = new System.Drawing.Size(104, 24);
            this.EncodeVideosCheckbox.TabIndex = 0;
            this.EncodeVideosCheckbox.Text = "EncodeVideos";
            // 
            // DVDFileStuctureCheckbox
            // 
            this.DVDFileStuctureCheckbox.Checked = true;
            this.DVDFileStuctureCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DVDFileStuctureCheckbox.Location = new System.Drawing.Point(112, 104);
            this.DVDFileStuctureCheckbox.Name = "DVDFileStuctureCheckbox";
            this.DVDFileStuctureCheckbox.Size = new System.Drawing.Size(160, 24);
            this.DVDFileStuctureCheckbox.TabIndex = 7;
            this.DVDFileStuctureCheckbox.Text = "Create DVD File structure";
            // 
            // mWriterComboBox
            // 
            this.mWriterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mWriterComboBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mWriterComboBox.Location = new System.Drawing.Point(48, 40);
            this.mWriterComboBox.Name = "mWriterComboBox";
            this.mWriterComboBox.Size = new System.Drawing.Size(240, 21);
            this.mWriterComboBox.TabIndex = 16;
            // 
            // mWriterLabel
            // 
            this.mWriterLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mWriterLabel.Location = new System.Drawing.Point(8, 40);
            this.mWriterLabel.Name = "mWriterLabel";
            this.mWriterLabel.Size = new System.Drawing.Size(40, 23);
            this.mWriterLabel.TabIndex = 17;
            this.mWriterLabel.Text = "Writer";
            this.mWriterLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(13, 926);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 23);
            this.label3.TabIndex = 18;
            this.label3.Text = "Speed";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // WriterSpeedCombo
            // 
            this.WriterSpeedCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.WriterSpeedCombo.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WriterSpeedCombo.Location = new System.Drawing.Point(48, 927);
            this.WriterSpeedCombo.Name = "WriterSpeedCombo";
            this.WriterSpeedCombo.Size = new System.Drawing.Size(80, 22);
            this.WriterSpeedCombo.TabIndex = 19;
            // 
            // mVolumeNameTextbox
            // 
            this.mVolumeNameTextbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mVolumeNameTextbox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mVolumeNameTextbox.Location = new System.Drawing.Point(384, 40);
            this.mVolumeNameTextbox.MaxLength = 16;
            this.mVolumeNameTextbox.Name = "mVolumeNameTextbox";
            this.mVolumeNameTextbox.Size = new System.Drawing.Size(120, 20);
            this.mVolumeNameTextbox.TabIndex = 31;
            this.mVolumeNameTextbox.WordWrap = false;
            this.mVolumeNameTextbox.Leave += new System.EventHandler(this.mProjectNameTextbox_TextChanged);
            // 
            // mVolumeNameLabel
            // 
            this.mVolumeNameLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mVolumeNameLabel.Location = new System.Drawing.Point(304, 40);
            this.mVolumeNameLabel.Name = "mVolumeNameLabel";
            this.mVolumeNameLabel.Size = new System.Drawing.Size(80, 23);
            this.mVolumeNameLabel.TabIndex = 30;
            this.mVolumeNameLabel.Text = "Volume name";
            this.mVolumeNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mCroppingCombo
            // 
            this.mCroppingCombo.Location = new System.Drawing.Point(0, 0);
            this.mCroppingCombo.Name = "mCroppingCombo";
            this.mCroppingCombo.Size = new System.Drawing.Size(121, 21);
            this.mCroppingCombo.TabIndex = 0;
            // 
            // mWizardGroup3
            // 
            this.mWizardGroup3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.mWizardGroup3.Controls.Add(this.mDebugShowImageCache);
            this.mWizardGroup3.Controls.Add(this.mDebugShowVideoCache);
            this.mWizardGroup3.Controls.Add(this.mMotionBlurHelpToolStrip);
            this.mWizardGroup3.Controls.Add(this.mMp4FileLocation);
            this.mWizardGroup3.Controls.Add(this.mMotionBlurCheckBox);
            this.mWizardGroup3.Controls.Add(this.mResolutionComboBox);
            this.mWizardGroup3.Controls.Add(this.mResolutionLabel);
            this.mWizardGroup3.Controls.Add(this.mDiskPictureBox);
            this.mWizardGroup3.Controls.Add(this.mWriterComboBox);
            this.mWizardGroup3.Controls.Add(this.mTotalCompleteLabel2);
            this.mWizardGroup3.Controls.Add(this.mTotalCompleteLabel);
            this.mWizardGroup3.Controls.Add(this.mETATextBox);
            this.mWizardGroup3.Controls.Add(this.ElapsedTimeTextBox);
            this.mWizardGroup3.Controls.Add(this.mBufferLabel);
            this.mWizardGroup3.Controls.Add(this.mBufferFull);
            this.mWizardGroup3.Controls.Add(this.CurrentProcessLabel);
            this.mWizardGroup3.Controls.Add(this.ElapsedTime);
            this.mWizardGroup3.Controls.Add(this.EstimateTimeLeft);
            this.mWizardGroup3.Controls.Add(this.mCreateProgressBar);
            this.mWizardGroup3.Controls.Add(this.mWriterLabel);
            this.mWizardGroup3.Controls.Add(this.Go);
            this.mWizardGroup3.Controls.Add(this.EncodeCheckPointsTextBox);
            this.mWizardGroup3.Controls.Add(this.Minimisebutton);
            this.mWizardGroup3.Controls.Add(this.mPreviousWizard3);
            this.mWizardGroup3.Controls.Add(this.mCancelButtonWizard3);
            this.mWizardGroup3.Controls.Add(this.mAuthorLabel);
            this.mWizardGroup3.Controls.Add(this.mVolumeNameLabel);
            this.mWizardGroup3.Controls.Add(this.mVolumeNameTextbox);
            this.mWizardGroup3.Controls.Add(this.CurrentProcessTextBox);
            this.mWizardGroup3.Location = new System.Drawing.Point(6, 616);
            this.mWizardGroup3.Name = "mWizardGroup3";
            this.mWizardGroup3.Size = new System.Drawing.Size(580, 256);
            this.mWizardGroup3.TabIndex = 26;
            this.mWizardGroup3.TabStop = false;
            // 
            // mDebugShowImageCache
            // 
            this.mDebugShowImageCache.Location = new System.Drawing.Point(293, 224);
            this.mDebugShowImageCache.Name = "mDebugShowImageCache";
            this.mDebugShowImageCache.Size = new System.Drawing.Size(19, 23);
            this.mDebugShowImageCache.TabIndex = 58;
            this.mDebugShowImageCache.Text = "I";
            this.mDebugShowImageCache.UseVisualStyleBackColor = true;
            this.mDebugShowImageCache.Visible = false;
            this.mDebugShowImageCache.Click += new System.EventHandler(this.mDebugShowImageCache_Click);
            // 
            // mDebugShowVideoCache
            // 
            this.mDebugShowVideoCache.Location = new System.Drawing.Point(269, 224);
            this.mDebugShowVideoCache.Name = "mDebugShowVideoCache";
            this.mDebugShowVideoCache.Size = new System.Drawing.Size(19, 23);
            this.mDebugShowVideoCache.TabIndex = 57;
            this.mDebugShowVideoCache.Text = "V";
            this.mDebugShowVideoCache.UseVisualStyleBackColor = true;
            this.mDebugShowVideoCache.Visible = false;
            this.mDebugShowVideoCache.Click += new System.EventHandler(this.mDebugShowVideoCache_Click);
            // 
            // mMotionBlurHelpToolStrip
            // 
            this.mMotionBlurHelpToolStrip.AutoSize = false;
            this.mMotionBlurHelpToolStrip.BackColor = System.Drawing.Color.White;
            this.mMotionBlurHelpToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.mMotionBlurHelpToolStrip.ImageScalingSize = new System.Drawing.Size(22, 22);
            this.mMotionBlurHelpToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mMotionBlurHelpToolStripButton});
            this.mMotionBlurHelpToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.mMotionBlurHelpToolStrip.Location = new System.Drawing.Point(546, 76);
            this.mMotionBlurHelpToolStrip.Name = "mMotionBlurHelpToolStrip";
            this.mMotionBlurHelpToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.mMotionBlurHelpToolStrip.Size = new System.Drawing.Size(24, 24);
            this.mMotionBlurHelpToolStrip.TabIndex = 56;
            // 
            // mMotionBlurHelpToolStripButton
            // 
            this.mMotionBlurHelpToolStripButton.AutoSize = false;
            this.mMotionBlurHelpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mMotionBlurHelpToolStripButton.Image = global::dvdslideshowfontend.Properties.Resources.Actions_help_about_icon2;
            this.mMotionBlurHelpToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mMotionBlurHelpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mMotionBlurHelpToolStripButton.Name = "mMotionBlurHelpToolStripButton";
            this.mMotionBlurHelpToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.mMotionBlurHelpToolStripButton.ToolTipText = "Click for more information about motion blur";
            this.mMotionBlurHelpToolStripButton.Click += new System.EventHandler(this.mMotionBlurHelpToolStripButton_Click);
            // 
            // mMp4FileLocation
            // 
            this.mMp4FileLocation.Location = new System.Drawing.Point(519, 40);
            this.mMp4FileLocation.Name = "mMp4FileLocation";
            this.mMp4FileLocation.Size = new System.Drawing.Size(30, 23);
            this.mMp4FileLocation.TabIndex = 56;
            this.mMp4FileLocation.Text = "...";
            this.mMp4FileLocation.UseVisualStyleBackColor = true;
            this.mMp4FileLocation.Visible = false;
            this.mMp4FileLocation.Click += new System.EventHandler(this.mMp4FileLocation_Click);
            // 
            // mMotionBlurCheckBox
            // 
            this.mMotionBlurCheckBox.Checked = true;
            this.mMotionBlurCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mMotionBlurCheckBox.Location = new System.Drawing.Point(444, 76);
            this.mMotionBlurCheckBox.Name = "mMotionBlurCheckBox";
            this.mMotionBlurCheckBox.Size = new System.Drawing.Size(105, 23);
            this.mMotionBlurCheckBox.TabIndex = 53;
            this.mMotionBlurCheckBox.Text = "Motion blur on";
            this.mMotionBlurCheckBox.UseVisualStyleBackColor = true;
            // 
            // mResolutionComboBox
            // 
            this.mResolutionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mResolutionComboBox.DropDownWidth = 260;
            this.mResolutionComboBox.FormattingEnabled = true;
            this.mResolutionComboBox.Location = new System.Drawing.Point(70, 72);
            this.mResolutionComboBox.Name = "mResolutionComboBox";
            this.mResolutionComboBox.Size = new System.Drawing.Size(145, 21);
            this.mResolutionComboBox.TabIndex = 52;
            this.mResolutionComboBox.Visible = false;
            this.mResolutionComboBox.SelectedIndexChanged += new System.EventHandler(this.mResolutionComboBox_SelectedIndexChanged);
            // 
            // mResolutionLabel
            // 
            this.mResolutionLabel.Location = new System.Drawing.Point(7, 76);
            this.mResolutionLabel.Name = "mResolutionLabel";
            this.mResolutionLabel.Size = new System.Drawing.Size(63, 23);
            this.mResolutionLabel.TabIndex = 51;
            this.mResolutionLabel.Text = "Resolution";
            this.mResolutionLabel.Visible = false;
            // 
            // mDiskPictureBox
            // 
            this.mDiskPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.mDiskPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("mDiskPictureBox.Image")));
            this.mDiskPictureBox.Location = new System.Drawing.Point(490, 76);
            this.mDiskPictureBox.Name = "mDiskPictureBox";
            this.mDiskPictureBox.Size = new System.Drawing.Size(70, 69);
            this.mDiskPictureBox.TabIndex = 49;
            this.mDiskPictureBox.TabStop = false;
            // 
            // mTotalCompleteLabel2
            // 
            this.mTotalCompleteLabel2.BackColor = System.Drawing.Color.White;
            this.mTotalCompleteLabel2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTotalCompleteLabel2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mTotalCompleteLabel2.Location = new System.Drawing.Point(412, 136);
            this.mTotalCompleteLabel2.Name = "mTotalCompleteLabel2";
            this.mTotalCompleteLabel2.Size = new System.Drawing.Size(56, 23);
            this.mTotalCompleteLabel2.TabIndex = 28;
            this.mTotalCompleteLabel2.Click += new System.EventHandler(this.mTotalCompleteLabel2_Click);
            // 
            // mTotalCompleteLabel
            // 
            this.mTotalCompleteLabel.BackColor = System.Drawing.Color.Transparent;
            this.mTotalCompleteLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTotalCompleteLabel.Location = new System.Drawing.Point(304, 136);
            this.mTotalCompleteLabel.Name = "mTotalCompleteLabel";
            this.mTotalCompleteLabel.Size = new System.Drawing.Size(120, 16);
            this.mTotalCompleteLabel.TabIndex = 27;
            this.mTotalCompleteLabel.Text = "Total Complete:";
            this.mTotalCompleteLabel.Click += new System.EventHandler(this.mTotalCompleteLabel_Click);
            // 
            // mETATextBox
            // 
            this.mETATextBox.BackColor = System.Drawing.Color.White;
            this.mETATextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mETATextBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mETATextBox.Location = new System.Drawing.Point(412, 112);
            this.mETATextBox.Name = "mETATextBox";
            this.mETATextBox.Size = new System.Drawing.Size(56, 23);
            this.mETATextBox.TabIndex = 26;
            // 
            // ElapsedTimeTextBox
            // 
            this.ElapsedTimeTextBox.BackColor = System.Drawing.Color.White;
            this.ElapsedTimeTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ElapsedTimeTextBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ElapsedTimeTextBox.Location = new System.Drawing.Point(96, 112);
            this.ElapsedTimeTextBox.Name = "ElapsedTimeTextBox";
            this.ElapsedTimeTextBox.Size = new System.Drawing.Size(104, 23);
            this.ElapsedTimeTextBox.TabIndex = 25;
            // 
            // mBufferLabel
            // 
            this.mBufferLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mBufferLabel.Location = new System.Drawing.Point(408, 160);
            this.mBufferLabel.Name = "mBufferLabel";
            this.mBufferLabel.Size = new System.Drawing.Size(40, 16);
            this.mBufferLabel.TabIndex = 23;
            this.mBufferLabel.Text = "Buffer";
            this.mBufferLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // mBufferFull
            // 
            this.mBufferFull.Location = new System.Drawing.Point(456, 160);
            this.mBufferFull.Name = "mBufferFull";
            this.mBufferFull.Size = new System.Drawing.Size(112, 16);
            this.mBufferFull.TabIndex = 22;
            // 
            // CurrentProcessLabel
            // 
            this.CurrentProcessLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentProcessLabel.Location = new System.Drawing.Point(8, 136);
            this.CurrentProcessLabel.Name = "CurrentProcessLabel";
            this.CurrentProcessLabel.Size = new System.Drawing.Size(88, 23);
            this.CurrentProcessLabel.TabIndex = 20;
            this.CurrentProcessLabel.Text = "Current process:";
            // 
            // Minimisebutton
            // 
            this.Minimisebutton.BackColor = System.Drawing.SystemColors.Control;
            this.Minimisebutton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Minimisebutton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Minimisebutton.Location = new System.Drawing.Point(8, 224);
            this.Minimisebutton.Name = "Minimisebutton";
            this.Minimisebutton.Size = new System.Drawing.Size(75, 23);
            this.Minimisebutton.TabIndex = 27;
            this.Minimisebutton.Text = "Minimize";
            this.Minimisebutton.UseVisualStyleBackColor = false;
            this.Minimisebutton.Click += new System.EventHandler(this.Minimisebutton_Click);
            // 
            // mPreviousWizard3
            // 
            this.mPreviousWizard3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mPreviousWizard3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mPreviousWizard3.Location = new System.Drawing.Point(408, 224);
            this.mPreviousWizard3.Name = "mPreviousWizard3";
            this.mPreviousWizard3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mPreviousWizard3.Size = new System.Drawing.Size(75, 23);
            this.mPreviousWizard3.TabIndex = 47;
            this.mPreviousWizard3.Text = "< Back";
            this.mPreviousWizard3.Click += new System.EventHandler(this.mPreviousWizard3_Click);
            // 
            // mCancelButtonWizard3
            // 
            this.mCancelButtonWizard3.BackColor = System.Drawing.SystemColors.Control;
            this.mCancelButtonWizard3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mCancelButtonWizard3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mCancelButtonWizard3.Location = new System.Drawing.Point(320, 224);
            this.mCancelButtonWizard3.Name = "mCancelButtonWizard3";
            this.mCancelButtonWizard3.Size = new System.Drawing.Size(80, 23);
            this.mCancelButtonWizard3.TabIndex = 24;
            this.mCancelButtonWizard3.Text = "Cancel";
            this.mCancelButtonWizard3.UseVisualStyleBackColor = false;
            this.mCancelButtonWizard3.Click += new System.EventHandler(this.CancelOrAbortButton_Click);
            // 
            // mAuthorLabel
            // 
            this.mAuthorLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mAuthorLabel.Location = new System.Drawing.Point(8, 14);
            this.mAuthorLabel.Name = "mAuthorLabel";
            this.mAuthorLabel.Size = new System.Drawing.Size(208, 23);
            this.mAuthorLabel.TabIndex = 48;
            this.mAuthorLabel.Text = "Author";
            // 
            // CurrentProcessTextBox
            // 
            this.CurrentProcessTextBox.BackColor = System.Drawing.Color.White;
            this.CurrentProcessTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentProcessTextBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.CurrentProcessTextBox.Location = new System.Drawing.Point(96, 136);
            this.CurrentProcessTextBox.Name = "CurrentProcessTextBox";
            this.CurrentProcessTextBox.Size = new System.Drawing.Size(200, 48);
            this.CurrentProcessTextBox.TabIndex = 21;
            // 
            // mWizardGroup1
            // 
            this.mWizardGroup1.BackColor = System.Drawing.Color.Transparent;
            this.mWizardGroup1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mWizardGroup1.BackgroundImage")));
            this.mWizardGroup1.Controls.Add(this.mBurnFolderToolStrip);
            this.mWizardGroup1.Controls.Add(this.EncodeOnlyRadioButton);
            this.mWizardGroup1.Controls.Add(this.BurnPreviousProjectRadio);
            this.mWizardGroup1.Controls.Add(this.label13);
            this.mWizardGroup1.Controls.Add(this.mNextWizard1);
            this.mWizardGroup1.Controls.Add(this.mCancelWizard1);
            this.mWizardGroup1.Controls.Add(this.mBurnFolderTestBox);
            this.mWizardGroup1.Controls.Add(this.BurnFromFolderRadio);
            this.mWizardGroup1.Controls.Add(this.CreateNewVideoRadio);
            this.mWizardGroup1.Controls.Add(this.label11);
            this.mWizardGroup1.Location = new System.Drawing.Point(6, 50);
            this.mWizardGroup1.Name = "mWizardGroup1";
            this.mWizardGroup1.Size = new System.Drawing.Size(580, 256);
            this.mWizardGroup1.TabIndex = 30;
            this.mWizardGroup1.TabStop = false;
            // 
            // mBurnFolderToolStrip
            // 
            this.mBurnFolderToolStrip.AutoSize = false;
            this.mBurnFolderToolStrip.BackColor = System.Drawing.Color.White;
            this.mBurnFolderToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.mBurnFolderToolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.mBurnFolderToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mBurnFolderToolStripButton});
            this.mBurnFolderToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.mBurnFolderToolStrip.Location = new System.Drawing.Point(534, 177);
            this.mBurnFolderToolStrip.Name = "mBurnFolderToolStrip";
            this.mBurnFolderToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.mBurnFolderToolStrip.Size = new System.Drawing.Size(40, 44);
            this.mBurnFolderToolStrip.TabIndex = 35;
            this.mBurnFolderToolStrip.Text = "toolStrip1";
            // 
            // mBurnFolderToolStripButton
            // 
            this.mBurnFolderToolStripButton.AutoSize = false;
            this.mBurnFolderToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mBurnFolderToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mBurnFolderToolStripButton.Image")));
            this.mBurnFolderToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mBurnFolderToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mBurnFolderToolStripButton.Name = "mBurnFolderToolStripButton";
            this.mBurnFolderToolStripButton.Size = new System.Drawing.Size(36, 36);
            this.mBurnFolderToolStripButton.ToolTipText = "Select CD/DVD/Blu-ray video folder";
            // 
            // EncodeOnlyRadioButton
            // 
            this.EncodeOnlyRadioButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.EncodeOnlyRadioButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EncodeOnlyRadioButton.Location = new System.Drawing.Point(8, 90);
            this.EncodeOnlyRadioButton.Name = "EncodeOnlyRadioButton";
            this.EncodeOnlyRadioButton.Size = new System.Drawing.Size(323, 24);
            this.EncodeOnlyRadioButton.TabIndex = 33;
            this.EncodeOnlyRadioButton.Text = "Create my slideshow to test or burn later";
            this.EncodeOnlyRadioButton.UseVisualStyleBackColor = false;
            this.EncodeOnlyRadioButton.CheckedChanged += new System.EventHandler(this.EncodeOnlyRadioButton_CheckedChanged);
            // 
            // BurnPreviousProjectRadio
            // 
            this.BurnPreviousProjectRadio.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BurnPreviousProjectRadio.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BurnPreviousProjectRadio.Location = new System.Drawing.Point(8, 120);
            this.BurnPreviousProjectRadio.Name = "BurnPreviousProjectRadio";
            this.BurnPreviousProjectRadio.Size = new System.Drawing.Size(356, 24);
            this.BurnPreviousProjectRadio.TabIndex = 32;
            this.BurnPreviousProjectRadio.Text = "Burn a previously created slideshow";
            this.BurnPreviousProjectRadio.UseVisualStyleBackColor = false;
            this.BurnPreviousProjectRadio.CheckedChanged += new System.EventHandler(this.BurnPreviousProjectRadio_CheckedChanged);
         
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(11, 211);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(389, 23);
            this.label13.TabIndex = 29;
            this.label13.Text = "Select a folder containing a VIDEO_TS for DVD, BDMV for Blu-ray";
            this.label13.Visible = false;
            // 
            // mNextWizard1
            // 
            this.mNextWizard1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mNextWizard1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mNextWizard1.Location = new System.Drawing.Point(496, 224);
            this.mNextWizard1.Name = "mNextWizard1";
            this.mNextWizard1.Size = new System.Drawing.Size(75, 23);
            this.mNextWizard1.TabIndex = 27;
            this.mNextWizard1.Text = "Next >";
            this.mNextWizard1.Click += new System.EventHandler(this.mNextWizard1_Click);
            // 
            // mCancelWizard1
            // 
            this.mCancelWizard1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mCancelWizard1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mCancelWizard1.Location = new System.Drawing.Point(416, 224);
            this.mCancelWizard1.Name = "mCancelWizard1";
            this.mCancelWizard1.Size = new System.Drawing.Size(75, 23);
            this.mCancelWizard1.TabIndex = 28;
            this.mCancelWizard1.Text = "Cancel";
            this.mCancelWizard1.Click += new System.EventHandler(this.mCancelWizard1_Click);
            // 
            // mBurnFolderTestBox
            // 
            this.mBurnFolderTestBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mBurnFolderTestBox.Location = new System.Drawing.Point(11, 184);
            this.mBurnFolderTestBox.Name = "mBurnFolderTestBox";
            this.mBurnFolderTestBox.ReadOnly = true;
            this.mBurnFolderTestBox.Size = new System.Drawing.Size(520, 22);
            this.mBurnFolderTestBox.TabIndex = 25;
            this.mBurnFolderTestBox.Visible = false;
            this.mBurnFolderTestBox.TextChanged += new System.EventHandler(this.BurnPreviousTestBox_TextChanged);
            // 
            // BurnFromFolderRadio
            // 
            this.BurnFromFolderRadio.AllowDrop = true;
            this.BurnFromFolderRadio.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BurnFromFolderRadio.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BurnFromFolderRadio.Location = new System.Drawing.Point(8, 150);
            this.BurnFromFolderRadio.Name = "BurnFromFolderRadio";
            this.BurnFromFolderRadio.Size = new System.Drawing.Size(284, 24);
            this.BurnFromFolderRadio.TabIndex = 21;
            this.BurnFromFolderRadio.Text = "Burn from a video folder";
            this.BurnFromFolderRadio.UseVisualStyleBackColor = false;
            this.BurnFromFolderRadio.CheckedChanged += new System.EventHandler(this.BurnFromFolderRadio_CheckedChanged);
            // 
            // CreateNewVideoRadio
            // 
            this.CreateNewVideoRadio.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.CreateNewVideoRadio.Checked = true;
            this.CreateNewVideoRadio.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CreateNewVideoRadio.Location = new System.Drawing.Point(8, 60);
            this.CreateNewVideoRadio.Name = "CreateNewVideoRadio";
            this.CreateNewVideoRadio.Size = new System.Drawing.Size(216, 24);
            this.CreateNewVideoRadio.TabIndex = 20;
            this.CreateNewVideoRadio.TabStop = true;
            this.CreateNewVideoRadio.Text = "Create and burn my slideshow";
            this.CreateNewVideoRadio.UseVisualStyleBackColor = false;
            this.CreateNewVideoRadio.CheckedChanged += new System.EventHandler(this.CreateNewVideoRadio_CheckedChanged);
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(8, 14);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(430, 48);
            this.label11.TabIndex = 26;
            this.label11.Text = "PhotoVidShow must create your slideshow before it can burn it.   Please select wh" +
    "at you want to do from the follow options.";
            // 
            // panel2
            // 
            this.panel2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel2.BackgroundImage")));
            this.panel2.Controls.Add(this.mWandPictureBox);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(600, 48);
            this.panel2.TabIndex = 29;
            // 
            // mWandPictureBox
            // 
            this.mWandPictureBox.BackColor = System.Drawing.Color.Black;
            this.mWandPictureBox.BackgroundImage = global::dvdslideshowfontend.Properties.Resources.Actions_tools_wizard_icon;
            this.mWandPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.mWandPictureBox.Location = new System.Drawing.Point(2, 0);
            this.mWandPictureBox.Name = "mWandPictureBox";
            this.mWandPictureBox.Size = new System.Drawing.Size(48, 48);
            this.mWandPictureBox.TabIndex = 34;
            this.mWandPictureBox.TabStop = false;
            this.mWandPictureBox.Visible = false;
            // 
            // mWizardGroup2
            // 
            this.mWizardGroup2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.mWizardGroup2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mWizardGroup2.BackgroundImage")));
            this.mWizardGroup2.Controls.Add(this.mCreateInFolderToolStrip);
            this.mWizardGroup2.Controls.Add(this.mAuthoringOptionsLabel);
            this.mWizardGroup2.Controls.Add(this.mPreviousWizard2);
            this.mWizardGroup2.Controls.Add(this.mNextWizard2);
            this.mWizardGroup2.Controls.Add(this.mCancelWizard2);
            this.mWizardGroup2.Controls.Add(this.mIgnoreMenusTickBox);
            this.mWizardGroup2.Controls.Add(this.mIncludeOriginalVideosTickBox);
            this.mWizardGroup2.Controls.Add(this.label10);
            this.mWizardGroup2.Controls.Add(this.mCropCombo);
            this.mWizardGroup2.Controls.Add(this.mIncludeOriginalPicturesTickBox);
            this.mWizardGroup2.Controls.Add(this.PalHelpLabel);
            this.mWizardGroup2.Controls.Add(this.PalRadio);
            this.mWizardGroup2.Controls.Add(this.mTVCropCheckbox);
            this.mWizardGroup2.Controls.Add(this.NtscHelpLabel);
            this.mWizardGroup2.Controls.Add(this.mCreateInFolderTextBox);
            this.mWizardGroup2.Controls.Add(this.mDiskEstimateSizeTextBox);
            this.mWizardGroup2.Controls.Add(this.label1);
            this.mWizardGroup2.Controls.Add(this.mVideoStandardLabel);
            this.mWizardGroup2.Controls.Add(this.WriteToOpticalMediaCheckBox);
            this.mWizardGroup2.Controls.Add(this.NtscRadio);
            this.mWizardGroup2.Controls.Add(this.mCreateInFolderCheckBox);
            this.mWizardGroup2.Location = new System.Drawing.Point(6, 344);
            this.mWizardGroup2.Name = "mWizardGroup2";
            this.mWizardGroup2.Size = new System.Drawing.Size(580, 256);
            this.mWizardGroup2.TabIndex = 22;
            this.mWizardGroup2.TabStop = false;
            this.mWizardGroup2.Enter += new System.EventHandler(this.mWizardGroup2_Enter);
            // 
            // mCreateInFolderToolStrip
            // 
            this.mCreateInFolderToolStrip.AutoSize = false;
            this.mCreateInFolderToolStrip.BackColor = System.Drawing.Color.White;
            this.mCreateInFolderToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.mCreateInFolderToolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.mCreateInFolderToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mCreateInFolderToolStripButton});
            this.mCreateInFolderToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.mCreateInFolderToolStrip.Location = new System.Drawing.Point(539, 157);
            this.mCreateInFolderToolStrip.Name = "mCreateInFolderToolStrip";
            this.mCreateInFolderToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.mCreateInFolderToolStrip.Size = new System.Drawing.Size(40, 44);
            this.mCreateInFolderToolStrip.TabIndex = 36;
            this.mCreateInFolderToolStrip.Text = "toolStrip1";
            // 
            // mCreateInFolderToolStripButton
            // 
            this.mCreateInFolderToolStripButton.AutoSize = false;
            this.mCreateInFolderToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mCreateInFolderToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mCreateInFolderToolStripButton.Image")));
            this.mCreateInFolderToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mCreateInFolderToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mCreateInFolderToolStripButton.Name = "mCreateInFolderToolStripButton";
            this.mCreateInFolderToolStripButton.Size = new System.Drawing.Size(36, 36);
            this.mCreateInFolderToolStripButton.ToolTipText = "Select output folder";
            // 
            // mAuthoringOptionsLabel
            // 
            this.mAuthoringOptionsLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mAuthoringOptionsLabel.Location = new System.Drawing.Point(8, 14);
            this.mAuthoringOptionsLabel.Name = "mAuthoringOptionsLabel";
            this.mAuthoringOptionsLabel.Size = new System.Drawing.Size(272, 23);
            this.mAuthoringOptionsLabel.TabIndex = 47;
            this.mAuthoringOptionsLabel.Text = "Authoring options";
            // 
            // mPreviousWizard2
            // 
            this.mPreviousWizard2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mPreviousWizard2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mPreviousWizard2.Location = new System.Drawing.Point(416, 224);
            this.mPreviousWizard2.Name = "mPreviousWizard2";
            this.mPreviousWizard2.Size = new System.Drawing.Size(75, 23);
            this.mPreviousWizard2.TabIndex = 46;
            this.mPreviousWizard2.Text = "< Back";
            this.mPreviousWizard2.Click += new System.EventHandler(this.mPreviousWizard2_Click);
            // 
            // mNextWizard2
            // 
            this.mNextWizard2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mNextWizard2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mNextWizard2.Location = new System.Drawing.Point(496, 224);
            this.mNextWizard2.Name = "mNextWizard2";
            this.mNextWizard2.Size = new System.Drawing.Size(75, 23);
            this.mNextWizard2.TabIndex = 45;
            this.mNextWizard2.Text = "Next >";
            this.mNextWizard2.Click += new System.EventHandler(this.mNextWizard2_Click);
            // 
            // mCancelWizard2
            // 
            this.mCancelWizard2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mCancelWizard2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mCancelWizard2.Location = new System.Drawing.Point(336, 224);
            this.mCancelWizard2.Name = "mCancelWizard2";
            this.mCancelWizard2.Size = new System.Drawing.Size(75, 23);
            this.mCancelWizard2.TabIndex = 44;
            this.mCancelWizard2.Text = "Cancel";
            this.mCancelWizard2.Click += new System.EventHandler(this.mCancelWizard2_Click);
            // 
            // mIgnoreMenusTickBox
            // 
            this.mIgnoreMenusTickBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mIgnoreMenusTickBox.Location = new System.Drawing.Point(11, 132);
            this.mIgnoreMenusTickBox.Name = "mIgnoreMenusTickBox";
            this.mIgnoreMenusTickBox.Size = new System.Drawing.Size(320, 24);
            this.mIgnoreMenusTickBox.TabIndex = 43;
            this.mIgnoreMenusTickBox.Text = "Ignore menus and play slideshows one after another";
            this.mIgnoreMenusTickBox.CheckedChanged += new System.EventHandler(this.mIgnoreMenusTickBox_CheckedChanged);
            // 
            // mIncludeOriginalVideosTickBox
            // 
            this.mIncludeOriginalVideosTickBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mIncludeOriginalVideosTickBox.Location = new System.Drawing.Point(215, 196);
            this.mIncludeOriginalVideosTickBox.Name = "mIncludeOriginalVideosTickBox";
            this.mIncludeOriginalVideosTickBox.Size = new System.Drawing.Size(192, 24);
            this.mIncludeOriginalVideosTickBox.TabIndex = 8;
            this.mIncludeOriginalVideosTickBox.Text = "Store original video files";
            this.mIncludeOriginalVideosTickBox.CheckedChanged += new System.EventHandler(this.mIncludeOriginalVideosTickBox_CheckedChanged);
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(287, 100);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(25, 24);
            this.label10.TabIndex = 42;
            this.label10.Text = "%";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mCropCombo
            // 
            this.mCropCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mCropCombo.DropDownWidth = 30;
            this.mCropCombo.Location = new System.Drawing.Point(242, 100);
            this.mCropCombo.MaxDropDownItems = 12;
            this.mCropCombo.Name = "mCropCombo";
            this.mCropCombo.Size = new System.Drawing.Size(40, 21);
            this.mCropCombo.TabIndex = 41;
            this.mCropCombo.SelectedIndexChanged += new System.EventHandler(this.mCropCombo_SelectedIndexChanged);
            // 
            // mIncludeOriginalPicturesTickBox
            // 
            this.mIncludeOriginalPicturesTickBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mIncludeOriginalPicturesTickBox.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mIncludeOriginalPicturesTickBox.Location = new System.Drawing.Point(11, 196);
            this.mIncludeOriginalPicturesTickBox.Name = "mIncludeOriginalPicturesTickBox";
            this.mIncludeOriginalPicturesTickBox.Size = new System.Drawing.Size(200, 24);
            this.mIncludeOriginalPicturesTickBox.TabIndex = 7;
            this.mIncludeOriginalPicturesTickBox.Text = "Store original picture files";
            this.mIncludeOriginalPicturesTickBox.CheckedChanged += new System.EventHandler(this.mIncludeOriginalPicturesTickBox_CheckedChanged);
            // 
            // PalHelpLabel
            // 
            this.PalHelpLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PalHelpLabel.Location = new System.Drawing.Point(352, 40);
            this.PalHelpLabel.Name = "PalHelpLabel";
            this.PalHelpLabel.Size = new System.Drawing.Size(208, 23);
            this.PalHelpLabel.TabIndex = 33;
            this.PalHelpLabel.Text = "(Used in Europe and Asia)";
            this.PalHelpLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PalRadio
            // 
            this.PalRadio.Checked = true;
            this.PalRadio.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PalRadio.Location = new System.Drawing.Point(312, 40);
            this.PalRadio.Name = "PalRadio";
            this.PalRadio.Size = new System.Drawing.Size(48, 24);
            this.PalRadio.TabIndex = 1;
            this.PalRadio.TabStop = true;
            this.PalRadio.Text = "PAL";
            this.PalRadio.CheckedChanged += new System.EventHandler(this.PalRadio_CheckedChanged);
            // 
            // mTVCropCheckbox
            // 
            this.mTVCropCheckbox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTVCropCheckbox.Location = new System.Drawing.Point(11, 92);
            this.mTVCropCheckbox.Name = "mTVCropCheckbox";
            this.mTVCropCheckbox.Size = new System.Drawing.Size(237, 40);
            this.mTVCropCheckbox.TabIndex = 34;
            this.mTVCropCheckbox.Text = "Reduce output picture to TV safe region";
            this.mTVCropCheckbox.CheckedChanged += new System.EventHandler(this.mTVCropCheckboxc_CheckedChanged);
            // 
            // NtscHelpLabel
            // 
            this.NtscHelpLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NtscHelpLabel.Location = new System.Drawing.Point(152, 40);
            this.NtscHelpLabel.Name = "NtscHelpLabel";
            this.NtscHelpLabel.Size = new System.Drawing.Size(152, 23);
            this.NtscHelpLabel.TabIndex = 32;
            this.NtscHelpLabel.Text = "(Used in America and Japan)";
            this.NtscHelpLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mCreateInFolderTextBox
            // 
            this.mCreateInFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mCreateInFolderTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mCreateInFolderTextBox.Location = new System.Drawing.Point(123, 164);
            this.mCreateInFolderTextBox.Name = "mCreateInFolderTextBox";
            this.mCreateInFolderTextBox.ReadOnly = true;
            this.mCreateInFolderTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mCreateInFolderTextBox.Size = new System.Drawing.Size(412, 22);
            this.mCreateInFolderTextBox.TabIndex = 26;
            this.mCreateInFolderTextBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // mDiskEstimateSizeTextBox
            // 
            this.mDiskEstimateSizeTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mDiskEstimateSizeTextBox.Location = new System.Drawing.Point(131, 228);
            this.mDiskEstimateSizeTextBox.Name = "mDiskEstimateSizeTextBox";
            this.mDiskEstimateSizeTextBox.Size = new System.Drawing.Size(128, 23);
            this.mDiskEstimateSizeTextBox.TabIndex = 29;
            this.mDiskEstimateSizeTextBox.Text = "1234/4096";
            this.mDiskEstimateSizeTextBox.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(5, 228);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 23);
            this.label1.TabIndex = 28;
            this.label1.Text = "Estimated final disk size";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mVideoStandardLabel
            // 
            this.mVideoStandardLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mVideoStandardLabel.Location = new System.Drawing.Point(8, 40);
            this.mVideoStandardLabel.Name = "mVideoStandardLabel";
            this.mVideoStandardLabel.Size = new System.Drawing.Size(90, 23);
            this.mVideoStandardLabel.TabIndex = 6;
            this.mVideoStandardLabel.Text = "Video standard";
            this.mVideoStandardLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // WriteToOpticalMediaCheckBox
            // 
            this.WriteToOpticalMediaCheckBox.Checked = true;
            this.WriteToOpticalMediaCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.WriteToOpticalMediaCheckBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WriteToOpticalMediaCheckBox.Location = new System.Drawing.Point(411, 196);
            this.WriteToOpticalMediaCheckBox.Name = "WriteToOpticalMediaCheckBox";
            this.WriteToOpticalMediaCheckBox.Size = new System.Drawing.Size(104, 24);
            this.WriteToOpticalMediaCheckBox.TabIndex = 2;
            this.WriteToOpticalMediaCheckBox.Text = "Burn to DVD";
            this.WriteToOpticalMediaCheckBox.Visible = false;
            this.WriteToOpticalMediaCheckBox.CheckedChanged += new System.EventHandler(this.WriteToDVDCheckBox_CheckedChanged);
            // 
            // NtscRadio
            // 
            this.NtscRadio.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NtscRadio.Location = new System.Drawing.Point(104, 40);
            this.NtscRadio.Name = "NtscRadio";
            this.NtscRadio.Size = new System.Drawing.Size(56, 24);
            this.NtscRadio.TabIndex = 0;
            this.NtscRadio.Text = "NTSC";
            this.NtscRadio.CheckedChanged += new System.EventHandler(this.ntscRatio_CheckedChanged);
            // 
            // mCreateInFolderCheckBox
            // 
            this.mCreateInFolderCheckBox.Checked = true;
            this.mCreateInFolderCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mCreateInFolderCheckBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mCreateInFolderCheckBox.Location = new System.Drawing.Point(11, 164);
            this.mCreateInFolderCheckBox.Name = "mCreateInFolderCheckBox";
            this.mCreateInFolderCheckBox.Size = new System.Drawing.Size(128, 24);
            this.mCreateInFolderCheckBox.TabIndex = 2;
            this.mCreateInFolderCheckBox.Text = "Create in folder";
            this.mCreateInFolderCheckBox.CheckedChanged += new System.EventHandler(this.OutputToDiskRadio_CheckedChanged);
            // 
            // mVideoQualityLabel
            // 
            this.mVideoQualityLabel.AutoSize = true;
            this.mVideoQualityLabel.Location = new System.Drawing.Point(0, 5);
            this.mVideoQualityLabel.Name = "mVideoQualityLabel";
            this.mVideoQualityLabel.Size = new System.Drawing.Size(43, 13);
            this.mVideoQualityLabel.TabIndex = 31;
            this.mVideoQualityLabel.Text = "Quality";
            // 
            // mVideoQualityComboBox
            // 
            this.mVideoQualityComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mVideoQualityComboBox.FormattingEnabled = true;
            this.mVideoQualityComboBox.Items.AddRange(new object[] {
            "Low",
            "Good",
            "High",
            "Maximum"});
            this.mVideoQualityComboBox.Location = new System.Drawing.Point(45, 1);
            this.mVideoQualityComboBox.Name = "mVideoQualityComboBox";
            this.mVideoQualityComboBox.Size = new System.Drawing.Size(74, 21);
            this.mVideoQualityComboBox.TabIndex = 32;
            this.mVideoQualityComboBox.SelectedIndexChanged += new System.EventHandler(this.mVideoQualityComboBox_SelectedIndexChanged);
            // 
            // mVideoFPSComboBox
            // 
            this.mVideoFPSComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mVideoFPSComboBox.DropDownWidth = 80;
            this.mVideoFPSComboBox.FormattingEnabled = true;
            this.mVideoFPSComboBox.Items.AddRange(new object[] {
            "12",
            "15",
            "20",
            "24",
            "25",
            "30  (default)",
            "50",
            "60"});
            this.mVideoFPSComboBox.Location = new System.Drawing.Point(151, 1);
            this.mVideoFPSComboBox.Name = "mVideoFPSComboBox";
            this.mVideoFPSComboBox.Size = new System.Drawing.Size(42, 21);
            this.mVideoFPSComboBox.TabIndex = 33;
            this.mVideoFPSComboBox.SelectedIndexChanged += new System.EventHandler(this.mVideoFPSComboBox_SelectedIndexChanged);
            // 
            // mVideoFPSLabel
            // 
            this.mVideoFPSLabel.AutoSize = true;
            this.mVideoFPSLabel.Location = new System.Drawing.Point(127, 5);
            this.mVideoFPSLabel.Name = "mVideoFPSLabel";
            this.mVideoFPSLabel.Size = new System.Drawing.Size(25, 13);
            this.mVideoFPSLabel.TabIndex = 34;
            this.mVideoFPSLabel.Text = "Fps";
            // 
            // mVideoQualityPanel
            // 
            this.mVideoQualityPanel.Controls.Add(this.mVideoQualityLabel);
            this.mVideoQualityPanel.Controls.Add(this.mVideoFPSComboBox);
            this.mVideoQualityPanel.Controls.Add(this.mVideoFPSLabel);
            this.mVideoQualityPanel.Controls.Add(this.mVideoQualityComboBox);
            this.mVideoQualityPanel.Location = new System.Drawing.Point(7, 955);
            this.mVideoQualityPanel.Name = "mVideoQualityPanel";
            this.mVideoQualityPanel.Size = new System.Drawing.Size(210, 26);
            this.mVideoQualityPanel.TabIndex = 35;
            // 
            // Author
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(594, 1045);
            this.ControlBox = false;
            this.Controls.Add(this.mVideoQualityPanel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.mWizardGroup1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mWizardGroup2);
            this.Controls.Add(this.mWizardGroup3);
            this.Controls.Add(this.WriterSpeedCombo);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Author";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Author Video";
            this.MinimumSizeChanged += new System.EventHandler(this.Author_MinimumSizeChanged);
            this.Closed += new System.EventHandler(this.Author_Closed);
            this.Load += new System.EventHandler(this.Author_Load);
            this.SizeChanged += new System.EventHandler(this.Author_SizeChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Author_MouseDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.mWizardGroup3.ResumeLayout(false);
            this.mWizardGroup3.PerformLayout();
            this.mMotionBlurHelpToolStrip.ResumeLayout(false);
            this.mMotionBlurHelpToolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mDiskPictureBox)).EndInit();
            this.mWizardGroup1.ResumeLayout(false);
            this.mWizardGroup1.PerformLayout();
            this.mBurnFolderToolStrip.ResumeLayout(false);
            this.mBurnFolderToolStrip.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mWandPictureBox)).EndInit();
            this.mWizardGroup2.ResumeLayout(false);
            this.mWizardGroup2.PerformLayout();
            this.mCreateInFolderToolStrip.ResumeLayout(false);
            this.mCreateInFolderToolStrip.PerformLayout();
            this.mVideoQualityPanel.ResumeLayout(false);
            this.mVideoQualityPanel.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion


		private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
		
		}

		private void dirListBox1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			//fileListBox1. 
		}


		//*******************************************************************
		private void Timer_Tick(object sender, System.EventArgs e)
		{
			// update eta and time text boxes

			if (mDoingTimerTick==true) return ;
			mDoingTimerTick=true;

			if (ManagedCore.CDebugLog.GetInstance().TraceEncode == true)
			{
				ArrayList list = ManagedCore.CDebugLog.GetInstance().EncodeCheckPointList;

				String encode_checkpoints_message ="";
				for (int i=0;i<31;i++)
				{
					if (list.Count > i)
					{
						string c= list[i].ToString();
						encode_checkpoints_message+=c;
					}
				}
				this.EncodeCheckPointsTextBox.Text = encode_checkpoints_message;
			}

			if (this.mTotalGoTime!=null)
			{
				if (mTimeEstimationClock==null)
				{
					mTimeEstimationClock= new DVDSlideshow.PrecisionStopWatch();
					mTimeEstimationClock.Start();
				}
						
				TimeSpan ged = mTimeEstimationClock.Time;
				TimeSpan tot_since_last = ged - mLastETACound ;
				mLastETACound=ged;


				this.mETATextBox.Text="";
				this.mTotalCompleteLabel2.Text="";
				TimeSpan t = this.mTotalGoTime.Time;
				String ss ;
				ss = System.String.Format("{0:d2}:{1:d2}:{2:d2}", t.Hours,t.Minutes,t.Seconds);
				this.ElapsedTimeTextBox.Text = ss;

				// re set estimation time ??
				if (t.TotalSeconds > 10 && 
					this.mAborted==false &&
					this.mPA!=null)
				{

					float pd = mPA.mPercentageCompete;
					int tot = this.mPA.TotalFramesToEncode;
					int cu = CGlobals.mCurrentProcessFrame;
					if (cu>100)
					{
						// ok quick estimate
						// how may fps have we done on average
						double fs = ((double)pd)/ t.TotalSeconds;
						// multiply that up to number of frames we need to do
						double left = 100-pd;
						double seconds = left / fs;
						// add 30 seconds to do final disc structure stuff 
						seconds +=mBuildStructureTime+mPreBuiltBurnEstimateTime;
						if (mLastTotalSeconsLeft==-1)
						{
							mLastTotalSeconsLeft = seconds;
						}
						else
						{
							if (seconds > 40 &&
								(seconds - mLastTotalSeconsLeft > 10 ||
								mLastTotalSeconsLeft - seconds > 10))
							{
								if ((ged - mLastMajourChange) > mLargeTimeChangeSpan)
								{
									mLastTotalSeconsLeft = seconds;
									mLastMajourChange=ged;
								}	
							}
						}
					}
				}

				// if counting, continue counting down
				if (mLastTotalSeconsLeft!=-1)
				{
					mLastTotalSeconsLeft-=tot_since_last.TotalSeconds;

					// woops
					if (mLastTotalSeconsLeft<0) mLastTotalSeconsLeft =0;

					int sec_int = (int) mLastTotalSeconsLeft;

					TimeSpan ts = new TimeSpan(sec_int/60/60, (sec_int/60)%60, sec_int%60);
					String tss ;
					tss = System.String.Format("{0:d2}:{1:d2}:{2:d2}", ts.Hours,ts.Minutes,ts.Seconds);
					this.mETATextBox.Text=tss;
					
				}
			}
			// not timing
			else
			{
				this.ElapsedTimeTextBox.Text="";
				this.mETATextBox.Text="";
				this.mTotalCompleteLabel2.Text="";
			}

			if (mPA!=null && this.mAborted==false)
			{
				mPA.UpdateStats();

				mCreateProgressBar.Value = (int) mPA.mPercentageCompete;
				SetCurrentProcessText(mPA.GetCurrentProcessText());

				int sq = mCreateProgressBar.Value;
				if (sq==100) sq=99;

				mTotalCompleteLabel2.Text=sq+"%";
				this.Text = sq+"%"+" Author video";

				//ProcessingFrameLabel.Text = CGlobals.mCurrentProcessFrame.ToString();
			}
			else
			{
				this.Text = "Author Video";
			}

			mDoingTimerTick=false;


		}

        //*******************************************************************
        private bool includeOriginalPictures()
        {
            bool doPics =  mIncludeOriginalPicturesTickBox.Visible == true &&
                           mIncludeOriginalPicturesTickBox.Checked == true;

            return doPics;
       
        }

        //*******************************************************************
        private bool includeOriginalVideos()
        {
            bool doVids =   mIncludeOriginalVideosTickBox.Visible == true &&
                           mIncludeOriginalVideosTickBox.Checked == true;

            return doVids;

        }

        //*******************************************************************
        private long GetEstimatedFinalEncodedDiskImage()
        {
            bool do_pics = includeOriginalPictures();
            bool do_vids = includeOriginalVideos();
            bool ignore_menus = mIgnoreMenusTickBox.Checked;

            CGlobals.VideoType outputType = mCurrentBurnMode;
            CGlobals.MP4Quality quality = mMainWindow.mCurrentProject.DiskPreferences.OutputVideoQuality;
            int height = mMainWindow.mCurrentProject.DiskPreferences.CustomVideoOutputHeight;
            float fps = mMainWindow.mCurrentProject.DiskPreferences.OutputVideoFramesPerSecond;

            long total = mMainWindow.mCurrentProject.GetDiskUsageEstimation(outputType, quality, height, fps, do_pics, do_vids, !ignore_menus);

            return total;
        }


		//*******************************************************************
        private void GoGoGoCreateFromProject()
		{
            try
            {
                CGlobals.VideoType discDetectedType = CGlobals.VideoType.DVD;

                bool do_pics = includeOriginalPictures();
                bool do_vids = includeOriginalVideos();
                bool ignore_menus = this.mIgnoreMenusTickBox.Checked;

                long total = GetEstimatedFinalEncodedDiskImage();

                //
                // Verify enough disk space for encoded output on hard drive
                //
                bool result = ManagedCore.IO.VertifyEnoughDiskSpace(CGlobals.GetTempDirectory(), total * 2, "encoding project", true);
                if (result == false)
                {
                    FinishCreateVideoOpetation(true);
                    return;
                }

                if (WriteToOpticalMediaCheckBox.Checked == true)
                {
                    if (mCurrentDevice == null)
                    {
                        FinishCreateVideoOpetation(true);
                        return;
                    }

                    if (CheckForBlankMedia(mCurrentBurnMode, mCurrentDevice, ref discDetectedType) == false)
                    {
                        mAborted = true;
                        FinishCreateVideoOpetation(false);
                        return;
                    }

                    //
                    // If creating a Blu-ray with DVD media, re-check that the estimated final disc size will still fit on a DVD 
                    //
                    if (mCurrentBurnMode == CGlobals.VideoType.BLURAY && discDetectedType == CGlobals.VideoType.DVD)
                    {
                        long disksize = CDVDVideoWriter.DVDSizeInBytes;
                        if (total >= disksize)
                        {
                            DialogResult res2 = DoMessageBoxOnFormThread("The estimated final disc image is too big for a blank DVD, are you sure you wish to continue? (not recommended)", "Disc image too big?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                            if (res2 == DialogResult.Cancel)
                            {
                                FinishCreateVideoOpetation(true);
                                return;
                            }
                        }
                    }
                }

                this.mTimer.Start();

                string output_path = this.mCreateInFolderTextBox.Text;
                if (this.mCreateInFolderCheckBox.Checked == false)
                {
                    output_path = "";
                }

                mPA = new CProjectAuthor(mMainWindow.mCurrentProject, output_path, mMainWindow.GetSlideShowManager().mCurrentSlideShow);
                CGlobals.mCurrentPA = mPA;

                bool do_orig = false;

                // if we gonna burn to get a quick estimation
                if (WriteToOpticalMediaCheckBox.Checked == true)
                {
                    mPreBuiltBurnEstimateTime = 0;

                    // This simply does not work
                    /*
				    long total_bytes = mMainWindow.mCurrentProject.GetDiskUsageEstimation(inlude_org_pictures,inlude_org_videos );
    			
				    // size to burn
				    long size = total_bytes;
				    if (size==0) 
				    {
					    mPreBuiltBurnEstimateTime= 0;
				    }
				    else
				    {
					    size /=1024;

					    // if padding and dvd make sure min size is 1gb
					    if ( (mCurrentBurnMode == CGlobals.VideoType.DVD || mCurrentBurnMode == CGlobals.VideoType.BLURAY) &&
						    this.mDoPadding==true &&
					        size <1024 *1024)
					    {
						    size=1024*1024;
					    }

                        int kilo_bytes_a_second = mCurrentDevice.GetWriteSpeed();
					    double ss = size;
					    ss /=kilo_bytes_a_second;
					    mPreBuiltBurnEstimateTime = ss+CGlobals.mBurnLeadInandOutTime;
				    }
                     */

                    if (do_pics == true || do_vids == true)
                    {
                        do_orig = true;
                    }


                }

                //SRG REMOVE TO TEST

                mPA.mDebugEncodeMenus = EncodeMenuCheckBox.Checked;
                mPA.mDebugEncodeBestQuality = this.EncodeBestQualityCheckbox.Checked;
                mPA.mDebugEncodeSlideshow1 = this.EncodeSlideShow1Checkbox.Checked;
                mPA.mDebugEncodeSlideshow2 = this.EncodeSlideShow2CheckBox.Checked;
                mPA.mDebugEncodeSlideshow3 = this.EnocodeSlideshow3Checkbox.Checked;
                mPA.mDebugEncodeVideos = this.EncodeVideosCheckbox.Checked;
                mPA.mDebugEncodeSubtitles = this.SubTitlesCheckBox.Checked;
                mPA.mDebugCreateDiscFileStructure = this.DVDFileStuctureCheckbox.Checked;

                string finalVideoFile = "";
                try
                {
                    finalVideoFile = mPA.Author(do_pics, do_vids, this.DontEncodeCheckbox.Checked, ignore_menus);
                }
                catch
                {
                    UserMessage.Show("An exception occurred when encoding the project.  See 'log.txt' file for details.", "Exception");
                    mAborted = true;
                }

                // if Project author aborted itself then make our abort flag the same
                if (mPA.Aborted == true)
                {
                    this.mAborted = true;
                }
                else
                {
                    // ONCE ENCODED WE CAN NOT ENCODE AGAIN we have to close window and start again, this is because
                    // we may have already written vob file to the created folder etc.

                    mProjectEncoded = true;
                }

                if (mPA != null) mPA.UpdateStats();

                mPA = null;
                CGlobals.mCurrentPA = null;

                if (mAborted == false)
                {
                    SetProgressBar(100);
                    SetCurrentProcessText("Complete.");
                    SetBufferFull(0);

                    if (WriteToOpticalMediaCheckBox.Checked == true)
                    {
                        string the_path = CGlobals.GetTempDirectory() + "\\dvdfolder";
                        if (output_path != "")
                        {
                            the_path = output_path;
                        }

                        if (mCurrentBurnMode == CGlobals.VideoType.BLURAY)
                        {
                            the_path = CGlobals.GetTempDirectory() + "\\blurayfolder";
                            if (output_path != "")
                            {
                                the_path = output_path + "\\BDMVROOT";
                            }
                        }

                        bool done = false;
                        while (done == false)
                        {
                            done = true;
                            if (DoActualBurn(mCurrentDevice, the_path, do_orig, 0, discDetectedType, CGlobals.GetTempDirectory()) == true)
                            {
                                mTotalGoTime.Stop();
                                if (mAborted == false)
                                {
                                    DialogResult res = DoMessageBoxOnFormThread("Video burn successful!\n\r\n\rDo you wish to burn another disk?", "Operation complete",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                                    if (res == DialogResult.Yes)
                                    {
                                        if (CheckForBlankMedia(mCurrentBurnMode, mCurrentDevice, ref discDetectedType) == false)
                                        {
                                            done = true;
                                        }
                                        else
                                        {
                                            done = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        this.mLastTotalSeconsLeft = 0;
                        mTotalGoTime.Stop();

                        // Video file
                        if (finalVideoFile != "")
                        {
                            VideoCreatedSuccessForm vcsf = new VideoCreatedSuccessForm(finalVideoFile);
                            System.Media.SystemSounds.Asterisk.Play();
                            vcsf.ShowDialog();
                        }
                        else
                        {
                            // DVD just encode only
                            DialogResult res = DoMessageBoxOnFormThread("Video creation successful!", "Operation complete",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }



                FinishCreateVideoOpetation(true);
            }
            catch (Exception e)
            {
                Log.Error(e.Message + "\r\n\r\n" + e.StackTrace);
            }
		}


        // *******************************************************************
        private delegate DialogResult ShowMessageBoxDelegate(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon);
		public DialogResult ShowMessageBox(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
            return UserMessage.Show(this, message, caption, buttons, icon);
		}

        // *******************************************************************
        private DialogResult DoMessageBoxOnFormThread(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            if (this.InvokeRequired == true)
            {
                ShowMessageBoxDelegate my_delegate = new ShowMessageBoxDelegate(ShowMessageBox);

                IAsyncResult aResult = this.BeginInvoke(my_delegate, new object[] { message, caption, buttons, icon });
                aResult.AsyncWaitHandle.WaitOne();

                return (DialogResult) this.EndInvoke(aResult);
            }
           
            // already on correct thread
            return ShowMessageBox(message, caption, buttons, icon);
          
        }

		//*******************************************************************
		private MangedToUnManagedWrapper.CManagedVideoDisk GetSelectDiskDevice()
		{
            if (mDeviceManager == null)
            {
                return null;
            }

			ArrayList al = mDeviceManager.GetVideoDiskDevices();
            if (al.Count == 0)
            {
                return null;
            }
		
			int si = this.mWriterComboBox.SelectedIndex;
            if (si < 0)
            {
                return null;
            }

            if (mWriterComboManagedDeviceList.Count < si + 1)
            {
                return null;
            }
			return (CManagedVideoDisk) mWriterComboManagedDeviceList[si];
		}


		//*******************************************************************
        //
        // This method checks for appropriate blank media for the given video type and device.
        // It returns true if appropriate blank media is found; otherwise false.
        //
        // It will allow the user to attempt to erase a disc if the disk is ersable.
        //
        // A blank DVD will be accepted for Blu-Ray types, so the discDetectedType will
        // be set with the resulting disc detected.
        // The discDetectedType value is only valid when this function returns true
        //
		private bool CheckForBlankMedia(CGlobals.VideoType type, CManagedVideoDisk device, ref CGlobals.VideoType discDetectedType)
		{
            if (device == null)
            {
                return false;
            }

            discDetectedType = type;
            int blank_tries = 0;
			bool retry = true;

			while(retry ==true)
			{
				retry=false;

                int tt = 0;
				if (type == CGlobals.VideoType.DVD) tt=1;
                if (type == CGlobals.VideoType.BLURAY) tt = 2;

				int resultCode = device.ContainsABlankDisk(tt);

                if (resultCode == 3 || resultCode == 4)
                {
                    DialogResult res = DoMessageBoxOnFormThread("We have detected a DVD disc when creating a Blu-ray video.  Although we can still burn to this disc type the resulting disc may not play on all players.  Do you still wish to continue?","DVD disc found",
                                               MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (res != DialogResult.OK)
                    {
                        return false;
                    }

                    discDetectedType = CGlobals.VideoType.DVD;
                }

                if (resultCode == 2 || resultCode == 4)
                {
                    mTotalGoTime.Stop();

                    DialogResult res = DoMessageBoxOnFormThread("This disk is not blank, erase disk?", "Erase disk?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (res == DialogResult.OK)
                    {
                        this.BeginInvoke(new ShowErasingOnGuiThreadDelegate(ShowErasingOnGuiThread));
                        System.Threading.Thread.Sleep(100);
                        int erarse_disk_result = EraseCurrentSelectDisk();
                     
                        mOperationWindow.End();
                        if (erarse_disk_result != 0)
                        {
                            DoMessageBoxOnFormThread("Could not erase disk.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                        mTotalGoTime.Start();
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (resultCode == 0)
                {
                    blank_tries++;

                    mTotalGoTime.Stop();

                    string message = "Is there a blank CD disc inserted into drive?";
                    if (this.mCurrentBurnMode == CGlobals.VideoType.DVD)
                    {
                        message = "Is there a blank DVD disc inserted into the drive?";
                    }
                    if (this.mCurrentBurnMode == CGlobals.VideoType.BLURAY)
                    {
                        message = "Is there a blank Blu-ray disc inserted into the drive?";
                    }

                    DialogResult res = DialogResult.Cancel;
                    if (blank_tries <= 1)
                    {
                        res = DoMessageBoxOnFormThread(message, "Error reading drive",
                            MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    }
                    else
                    {
                        message += "\n\rCould not detect a blank media disc in the device. Select ignore to attempt to burn the disc anyway (may cause errors if burner was unable to write to the disc)";
                        res = DoMessageBoxOnFormThread(message, "Error reading drive",
                            MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
                    }

                    if (res == DialogResult.Ignore)
                    {
                        mTotalGoTime.Start();
                        return true;
                    }

                    if (res == DialogResult.Retry)
                    {
                        retry = true;
                        mTotalGoTime.Start();
                    }
                    if (res == DialogResult.Cancel ||
                        res == DialogResult.Abort)
                    {
                        return false;
                    }
                }
			}

			return true;
		}

		//*******************************************************************
        private delegate void SetCurrentProcessTextDelegate(string t);
		public void SetCurrentProcessText(string t)
		{
            if (this.InvokeRequired == true)
            {
                BeginInvoke(new SetCurrentProcessTextDelegate(SetCurrentProcessText),
                                            new object[] { t });
                return;
            }
			this.CurrentProcessTextBox.Text =t;
		}

        //*******************************************************************
        private delegate void SetProgressBarDelegate(int new_value);
        public void SetProgressBar(int new_value)
        {
            if (this.InvokeRequired == true)
            {
                BeginInvoke(new SetProgressBarDelegate(SetProgressBar),
                                            new object[] { new_value });
                return;
            }

            this.mCreateProgressBar.Value = new_value;
        }

        //*******************************************************************
        private delegate void SetBufferFullDelegate(int new_value);
        public void SetBufferFull(int new_value)
        {
            if (this.InvokeRequired == true)
            {
                BeginInvoke(new SetBufferFullDelegate(SetBufferFull),
                                            new object[] { new_value });
                return;
            }

            this.mBufferFull.Value = new_value;
        }


		//*******************************************************************
		private bool DoActualBurn(CManagedVideoDisk device, string path, bool store_org, int attempt, CGlobals.VideoType discDetectedType, string mpegFolder)
		{
            mDoingPaddingProcess = false;

            SetProgressBar(0);
            SetBufferFull(0);

			if (this.mCurrentBurnMode ==CGlobals.VideoType.DVD)
			{
                mCurrentBurnWritter = new CDVDVideoWriter(device, path, this.mVolumeNameTextbox.Text, mDoPadding);
			}
            else if (this.mCurrentBurnMode == CGlobals.VideoType.BLURAY)
            {
                mCurrentBurnWritter = new CBluRayVideoWriter(device, path, this.mVolumeNameTextbox.Text, mDoPadding, discDetectedType);
                SetCurrentProcessText("Preparing for burn...");

                if (mCurrentBurnWritter.CreateIso() == false)
                {
                    DoMessageBoxOnFormThread("An error occurred when creating the Blu-ray iso", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (mAborted == true)
            {
                return true;
            }

      
            SetCurrentProcessText("Writing lead in...");

            this.mLastTotalSeconsLeft = 0;  // Burn estimation times currently turned off, becuase estimation does not work (G919)


            int result = mCurrentBurnWritter.BurnFromPath(store_org);
			bool sucess =false;
            if (result == 1) sucess = true;

			mCurrentBurnWritter=null;
			SetCurrentProcessText("");

			// ok for a burn failure to occur a sucess has to be false
			// and also because starburn sometimes says there was a problem
			// when there wasn't it also has to have not reached the finalize
			// stage

			if (sucess==false && mBurnReachFinalizeStage==false)
			{
				SetProgressBar(0);
				SetBufferFull(0);
				mLastTotalSeconsLeft=0;

				mTotalGoTime.Stop();

                // unkown error command
                string message= "An error has occurred in the burn process!";

                if (mDoingPaddingProcess == true)
                {
                    message = "An error occurred whilst padding the disk to 1 gig. Although there was a problem, this disk will most likely still be usable.";
                    DoMessageBoxOnFormThread(message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else
                {
                    if (this.mCurrentBurnMode == CGlobals.VideoType.DVD && attempt ==0)
                    {
                        CDVDEncodedProjectVerifier DVDverifier = new CDVDEncodedProjectVerifier(CGlobals.mCurrentProject, CGlobals.GetTempDirectory(), path);
                        DVDverifier.Check();
                    }

                    if (result != 0)
                    {
                        int rr = result % 100;
                        try
                        {
                            message = mDiskErrors[rr];
                        }
                        catch
                        {
                        }
                    }


                    DialogResult res = DialogResult.Cancel;

                    if (attempt < 1)
                    {
                        res = DoMessageBoxOnFormThread(result + " " + message + "\n\rDo you wish to try again?", "Error",
                               MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (this.mCurrentBurnMode == CGlobals.VideoType.DVD)
                        {
                            res = DoMessageBoxOnFormThread(result + " " + message + "\n\rIf you are experiencing repeated problems please email support@photovidshow.com attached with the 'log.txt' file now stored at MyDocuments/PhotoVidShow/log.txt.\n\r\n\rAs your project is now encoded on the harddrive as a DVD it may be worth burning it with the external DVD burn tool, from PhotoVidShow main window select 'Tools -> Launch DVD-Video burner tool'.\n\r\n\rIt also may be worth visiting the FAQ section at our website http://www.photovidshow.com for further help about burn problems.\n\r\n\rDo you wish to try again?", "Error",
                                MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                        }
                        else
                        {
                            res = DoMessageBoxOnFormThread(result + " " + message + "\n\rIf you are experiencing repeated problems please email support@photovidshow.com attached with the 'log.txt' file now stored at MyDocuments/PhotoVidShow/log.txt\n\r\n\rIt also may be worth visiting the FAQ section at our website http://www.photovidshow.com for further help about burn problems.\n\r\n\rDo you wish to try again?", "Error",
                                MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                        }
                    }


                    if (res == DialogResult.Retry)
                    {
                        if (mDeviceManager == null) CreateDeviceManager();

                        mDeviceManager.LogAvailbleVideoDevicesList();

                        bool res1 = DoActualBurn(device, path, false, attempt + 1, discDetectedType, mpegFolder);

                        return res1;
                    }

                    return false;
                }
			}
			
			
            SetProgressBar(100);
            SetBufferFull(0);
	        mLastTotalSeconsLeft=0;
			

			return true;
		}


		//*******************************************************************
        private delegate void FinishCreateVideoOpetationDelegate(bool show_abort_message);
		private void FinishCreateVideoOpetation(bool show_abort_message)
		{
            // this is called from encoder gogogo thread, need to pass onto author dialog thread
            if (mMainWindow.InvokeRequired == true)
            {
                mMainWindow.BeginInvoke(new FinishCreateVideoOpetationDelegate(FinishCreateVideoOpetation),
                                           new object[] { show_abort_message });
                return;
            }

			ManagedCore.CDebugLog.GetInstance().Trace("Called FinishCreateVideoOpetation");

			this.mMainWindow.Show();
			this.mTimer.Stop();
			this.mTotalGoTime.Stop();
			this.ElapsedTimeTextBox.Text="";
			this.mETATextBox.Text="";
			this.mTotalCompleteLabel2.Text="";

			this.mCreatingVideo = false;
			this.DoHighLightUnHighlight();
			this.SetCurrentProcessText("");
			if (mTimeEstimationClock!=null)
			{
				mTimeEstimationClock.Stop();
			}

			if (mAborted==true)
			{
				this.mCreateProgressBar.Value=0;
				this.mBufferFull.Value=0;
				mAborted=false;
				if (show_abort_message==true)
				{
                    DialogResult res = DoMessageBoxOnFormThread("Operation was aborted!", "Aborted",
						MessageBoxButtons.OK,MessageBoxIcon.Information);
				}

                if (mProjectEncoded == false)
                {
                    // aborted whilst encoded allow us to encode again
                    return;
                }

                ManagedCore.CDebugLog.GetInstance().Trace("Closing window");
                this.Close();

			}
			else
			{

				ManagedCore.CDebugLog.GetInstance().Trace("Closing window");

				this.Close();
			}

			this.mCreateProgressBar.Value=0;
			this.mBufferFull.Value=0;
		}


		//*******************************************************************
        private void GoGoGoFromVideoFolder()
		{
            CGlobals.VideoType discDetectedType = CGlobals.VideoType.DVD;

            if (mCurrentDevice == null) 
			{
				FinishCreateVideoOpetation(true);
				return ;

			}

            if (CheckForBlankMedia(mCurrentBurnMode, mCurrentDevice, ref discDetectedType) == false) 
			{
				mAborted=true;
				FinishCreateVideoOpetation(false);
				return ;
			}


            if (DoActualBurn(mCurrentDevice, this.mTheVideoPath, false, 0, discDetectedType, "") == true)
			{
				if (mAborted==false)
				{
                    SetProgressBar(100);
                    SetBufferFull(0);
					mLastTotalSeconsLeft=0;

					mTotalGoTime.Stop();

                    DialogResult res = DoMessageBoxOnFormThread("Video burn successful!", "Operation complete",
						MessageBoxButtons.OK,MessageBoxIcon.Information);
				}
			}

			if (mPA!=null)mPA.UpdateStats();
			FinishCreateVideoOpetation(true);

		
		}

        //*******************************************************************
        private void SetProjectDiskPreferencesFromGui()
        {
            CProject currentProject = CGlobals.mCurrentProject;

            if (currentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.MP4)
            {
                if (currentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV4_3)
                {
                    currentProject.DiskPreferences.CustomVideoOutputWidth = mVideoOutput4by3WidthResolutions[mResolutionComboBox.SelectedIndex];
                    currentProject.DiskPreferences.CustomVideoOutputHeight = mVideoOutput4by3HeightResolutions[mResolutionComboBox.SelectedIndex];
                }
                else
                {
                    currentProject.DiskPreferences.CustomVideoOutputWidth = mVideoOutput16by9WidthResolutions[mResolutionComboBox.SelectedIndex];
                    currentProject.DiskPreferences.CustomVideoOutputHeight = mVideoOutput16by9HeightResolutions[mResolutionComboBox.SelectedIndex];
                }

                // To store these resolution we need to reset type
                currentProject.DiskPreferences.OutputVideoType = CGlobals.VideoType.MP4;

                string[] fpsText = mVideoFPSComboBox.Text.Split(' ');   // first bit of text is fps

                currentProject.DiskPreferences.OutputVideoFramesPerSecond = float.Parse(fpsText[0]);
                currentProject.DiskPreferences.OutputVideoQuality = (CGlobals.MP4Quality)mVideoQualityComboBox.SelectedIndex;
            }


            //
            // If blu-ray set resolution and fps from combo guis
            //
            if (currentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.BLURAY)
            {
                currentProject.DiskPreferences.CustomVideoOutputWidth = mBluRayOutput16by9WidthResolutions[mResolutionComboBox.SelectedIndex];
                currentProject.DiskPreferences.CustomVideoOutputHeight = mBluRayOutput16by9HeightResolutions[mResolutionComboBox.SelectedIndex];

                // To store these resolution we need to reset type
                currentProject.DiskPreferences.OutputVideoType = CGlobals.VideoType.BLURAY;

                string[] fpsText = mVideoFPSComboBox.Text.Split(' ');   // first bit of text is fps

                currentProject.DiskPreferences.OutputVideoFramesPerSecond = float.Parse(fpsText[0]);
                currentProject.DiskPreferences.OutputVideoQuality = (CGlobals.MP4Quality)mVideoQualityComboBox.SelectedIndex;
            }


            if (mMotionBlurCheckBox.Checked == true)
            {
                CGlobals.mCurrentProject.DiskPreferences.TurnOffMotionBlur = false;
            }
            else
            {
                CGlobals.mCurrentProject.DiskPreferences.TurnOffMotionBlur = true;
            }
        }

		//*******************************************************************
		private void Go_Click(object sender, System.EventArgs e)
		{
			// if trial version do a warning

			if (ManagedCore.License.License.TrialValid==true)
			{
                DialogResult res = DoMessageBoxOnFormThread("This is an evaluation copy of PhotoVidShow.  A watermark will be shown on each slideshow.\r\n\r\nDo you still wish to continue?", "Warning",
					MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
				if (res == DialogResult.No)
				{
					return ;
				}
			}
            
            SetProjectDiskPreferencesFromGui();

			mLastTotalSeconsLeft=-1;
			this.mCreatingVideo = true;
			mTimeEstimationClock=null;
			mPreBuiltBurnEstimateTime=0.0;
			mLastETACound = new TimeSpan(0,0,0);	// last time we did a eta checkup
			mLastMajourChange = new TimeSpan(0,0,0);	// last time a large changed occured
			mLargeTimeChangeSpan = new TimeSpan(0,0,10);// how oftern a large change is allowed
			this.DoHighLightUnHighlight();

			mTimer = new System.Windows.Forms.Timer();

			mCreateProgressBar.Minimum=0;
			mCreateProgressBar.Maximum=100;

			mCreateProgressBar.Value = 0;

			mTimer.Interval = 100;
			MangedToUnManagedWrapper.CDVDEncode.ResetProgressState();
	
			if (mPA!=null)
			{
				mPA=null;
			}
			mDoingTimerTick=false;
			mTimer.Tick += new System.EventHandler(this.Timer_Tick);
			mTimer.Start();

			mTotalGoTime = new StopWatch();

			mTotalGoTime.Start();
			mAuthorThread=null ;

            mCurrentDevice = this.GetSelectDiskDevice();

			if (this.CreateNewVideoRadio.Checked==true || this.EncodeOnlyRadioButton.Checked==true)
			{
				System.Threading.ThreadStart s = new System.Threading.ThreadStart(this.GoGoGoCreateFromProject);
				mAuthorThread = new Thread(s);
				mAuthorThread.Start();
			}
			else
			{
				System.Threading.ThreadStart s = new System.Threading.ThreadStart(this.GoGoGoFromVideoFolder);
				mAuthorThread = new Thread(s);
				mAuthorThread.Start();

			}
			if (mAuthorThread!=null)
			{
			//	if (this.BackgroundtaskCheckbutton.Checked==true)
				{
					mAuthorThread.Priority = System.Threading.ThreadPriority.Lowest;
				}
			//	else
			//	{
			//		mAuthorThread.Priority = System.Threading.ThreadPriority.Normal;
			//	}

			}
		}

		//*******************************************************************
		private void PalRadio_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.PalRadio.Checked==true)
			{
				CGlobals.mCurrentProject.DiskPreferences.SetToPAL();
			}
			else
			{
				CGlobals.mCurrentProject.DiskPreferences.SetToNTSC();
			}
		}

		//*******************************************************************
		private void ntscRatio_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.NtscRadio.Checked==true)
			{
				CGlobals.mCurrentProject.DiskPreferences.SetToNTSC();
			}
			else
			{
				CGlobals.mCurrentProject.DiskPreferences.SetToPAL();
			}
		}


		//*******************************************************************
		private void ProcessingFrameLabel_Click(object sender, System.EventArgs e)
		{
		
		}


		//*******************************************************************
		private void CreateNewVideoRadio_CheckedChanged(object sender, System.EventArgs e)
		{
            if (CreateNewVideoRadio.Checked == true)
            {
                RecalcBurnType();
                DoHighLightUnHighlight();
            }
		}

		//*******************************************************************
        private void BurnFromFolderRadio_CheckedChanged(object sender, System.EventArgs e)
		{
            if (BurnFromFolderRadio.Checked == true)
            {
                RecalcBurnType();
                DoHighLightUnHighlight();
            }
		}

        //*******************************************************************
        private void BurnPreviousProjectRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (BurnPreviousProjectRadio.Checked == true)
            {
                RecalcBurnType();
                DoHighLightUnHighlight();
            }
        }

        //*******************************************************************
        private void EncodeOnlyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (EncodeOnlyRadioButton.Checked == true)
            {
                RecalcBurnType();
                DoHighLightUnHighlight();
            }
        }

		//*******************************************************************
		private void DoHighLightUnHighlight()
		{
			bool go_will_burn=false;


			this.Go.Enabled=true;
			if (this.mCreateInFolderCheckBox.Checked==true)
			{
				this.mCreateInFolderTextBox.Enabled=true;
			}
			else
			{
				this.mCreateInFolderTextBox.Enabled=false;
			}
			
			if (this.BurnFromFolderRadio.Checked==true)
			{
				this.mBurnFolderTestBox.Enabled=true;
                this.mBurnFolderToolStripButton.Enabled = true;
				this.mWizardGroup2.Enabled=false;
                this.label13.Visible = true;
                this.mBurnFolderToolStripButton.Visible = true;
                this.mBurnFolderTestBox.Visible = true;
			}
			else
			{
				this.mBurnFolderTestBox.Enabled=false;
                this.mBurnFolderToolStripButton.Enabled = false;
				this.mWizardGroup2.Enabled=true;
                this.label13.Visible = false;
                this.mBurnFolderToolStripButton.Visible = false;
                this.mBurnFolderTestBox.Visible = false;
			}

		
			this.mNextWizard1.Enabled=true;

			// if burn previous video check we have valid folder
			if (this.BurnFromFolderRadio.Checked==true)
			{
                if (mTheVideoPath=="")
				{
					this.Go.Enabled=false;
					this.mNextWizard1.Enabled=false;
				}
				else
				{
					this.Go.Enabled=true;
					go_will_burn=true;
				}
			}
			else
			{
				this.Go.Enabled=true;
			}

			if (mCreatingVideo==true)
			{
				this.mCancelButtonWizard3.Enabled=true;
				this.mCancelButtonWizard3.Text="Abort";
				this.mPreviousWizard3.Enabled=false;
			//	this.AbortButton.Enabled=true;
				this.Minimisebutton.Enabled=true;
				this.Go.Enabled=false;
				this.mBurnFolderTestBox.Enabled=false;
                this.mBurnFolderToolStripButton.Enabled = false;
				this.mWizardGroup2.Enabled=false;
				this.mWriterComboBox.Enabled=false;
				this.WriterSpeedCombo.Enabled=false;
				this.BurnFromFolderRadio.Enabled=false;
				this.CreateNewVideoRadio.Enabled=false;
				mVolumeNameTextbox.Enabled=false;

			    mMotionBlurCheckBox.Enabled = false;
                mMotionBlurHelpToolStrip.Enabled = false;
                mResolutionComboBox.Enabled = false;
                mMp4FileLocation.Enabled = false;
                mVideoQualityPanel.Enabled = false;
			}
			else
			{
				mVolumeNameTextbox.Enabled=true;
				this.mCancelButtonWizard3.Enabled=true;
				this.mCancelButtonWizard3.Text="Cancel";
				this.mPreviousWizard3.Enabled=true;
		//		this.AbortButton.Enabled=false;
				this.Text = "Author Video";
				this.Minimisebutton.Enabled=false;
				this.mWriterComboBox.Enabled=true;
				this.WriterSpeedCombo.Enabled=true;
                mMotionBlurCheckBox.Enabled = mMotionBlurAvailable;
                mMotionBlurHelpToolStrip.Enabled = true;
                mResolutionComboBox.Enabled = true;
                mMp4FileLocation.Enabled = true;
                mVideoQualityPanel.Enabled = true;

				this.BurnFromFolderRadio.Enabled=true;
				if (mEmptyProject==false)
				{
					this.CreateNewVideoRadio.Enabled=true;
				}

				if (this.WriteToOpticalMediaCheckBox.Checked==false &&
					this.BurnFromFolderRadio.Checked==false)
				{
					this.mWriterComboBox.Enabled=false;
					this.WriterSpeedCombo.Enabled=false;
					mVolumeNameTextbox.Enabled=false;
				}
				else
				{
					go_will_burn=true;
				}

			}

			this.mNextWizard2.Enabled=true;

			if (this.WriteToOpticalMediaCheckBox.Checked==false &&
				this.mCreateInFolderCheckBox.Checked==false)
			{
				this.Go.Enabled=false;
				this.mNextWizard2.Enabled=false;
			}

			if (this.WriteToOpticalMediaCheckBox.Checked==true)
			{
				this.mIncludeOriginalVideosTickBox.Visible=true;
                this.mIncludeOriginalPicturesTickBox.Visible = true;

			}
			else
			{
                this.mIncludeOriginalVideosTickBox.Visible = false;
                this.mIncludeOriginalPicturesTickBox.Visible = false;
			}
		
			if (this.mAborted==true)
			{
			//	this.AbortButton.Enabled=false;
				this.Minimisebutton.Enabled=false;
				this.Text = "Author Video";
			}

			if (this.mBurnCDOnly==true)
			{
				this.mIncludeOriginalPicturesTickBox.Enabled=false;
				this.mIncludeOriginalVideosTickBox.Enabled=false;
			}

			// nothing to burn with , disable go
			if (go_will_burn==true &&
				this.mWriterComboBox.Items.Count==0)
			{
				this.Go.Enabled=false;
			}

			// allow us to create in temp folder and not burn when debugging.
			if (CGlobals.mIsDebug==true)
			{
				this.Go.Enabled=true;
			}
		}

		//*******************************************************************
		public ProgressBar GetBufferFull()
		{
			return this.mBufferFull;
		}

		//*******************************************************************
		public ProgressBar GetPercentageDone()
		{
			return this.mCreateProgressBar;
		}

		//*******************************************************************
		private void CancelOrAbortButton_Click(object sender, System.EventArgs e)
		{
			if (mCreatingVideo==true)
			{
				if (mAborted==true) return ;

				if (mCurrentBurnWritter!=null)
				{
                    //
                    // If we have a writter defined and it is not creating an iso, then we are currently burning to the disc, give warning message
                    // 
                    if (mCurrentBurnWritter.CreatingIso() == false)
                    {
                        DialogResult res = DoMessageBoxOnFormThread("Are you sure you wish to abort the burn process? Disk may be unusable after aborting!", "Abort Burn Process",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                        if (res == DialogResult.Cancel)
                        {
                            return;
                        }
                    }

					mCurrentBurnWritter.AbortBurn();
				}

				// are we current creating the video
				if (this.mPA!=null)
				{
					this.mPA.Abort();
				}

				SetCurrentProcessText("Aborting...");
				mAborted=true;
			}
			else
			{
				this.Close();
			}
		}

		//*******************************************************************
		private void OutputToDiskRadio_CheckedChanged(object sender, System.EventArgs e)
		{
            bool file = false;
            string fileExtension = "";

            if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.MP4)
            {
                file = true;
                fileExtension = ".mp4";
            }
          
			// if we've just clicked and no directory has been set then set to a default directory
			if (this.mCreateInFolderCheckBox.Checked==true)
			{
				if (this.mCreateInFolderTextBox.Text=="")
				{
                    string myprogramfiles = DefaultFolders.GetFolder("AuthoredFolder");
                    bool exists = true;
					int count =0;

					string fullname = "";

					while(exists==true)
					{
						string fn = System.IO.Path.GetFileNameWithoutExtension(this.mMainWindow.mCurrentProject.mName);
					
						fullname = myprogramfiles+"\\"+fn;
						if (count >0)
						{
							fullname+=count.ToString();
						}

                        if (file == true)
                        {
                            fullname += fileExtension;
                            exists = System.IO.File.Exists(fullname);
                        }
                        else
                        {
                            exists = System.IO.Directory.Exists(fullname);
                        }

						count++;
					}

					this.mCreateInFolderTextBox.Text = fullname;
				}
			}

			this.DoHighLightUnHighlight();
		}

		//*******************************************************************
		private void WriteToDVDCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			this.DoHighLightUnHighlight();
			this.ReDrawDiskEstimationTextBox();
		}

        //*******************************************************************
		private void mIncludeOriginalPicturesTickBox_CheckedChanged(object sender, System.EventArgs e)
		{
			ReDrawDiskEstimationTextBox();
		}

        //*******************************************************************
		private void mIncludeOriginalVideosTickBox_CheckedChanged(object sender, System.EventArgs e)
		{
			ReDrawDiskEstimationTextBox();
		}

        //*******************************************************************
        private void mResolutionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReDrawDiskEstimationTextBox();
        }

        //*******************************************************************
        private void mVideoQualityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReDrawDiskEstimationTextBox();
        }

        //*******************************************************************
        private void mVideoFPSComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReDrawDiskEstimationTextBox();
        }

        //*******************************************************************
        private void mIgnoreMenusTickBox_CheckedChanged(object sender, EventArgs e)
        {
            ReDrawDiskEstimationTextBox();
        }

		private void textBox1_TextChanged(object sender, System.EventArgs e)
		{
			this.mCreateInFolderTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		}

		private void BurnPreviousTestBox_TextChanged(object sender, System.EventArgs e)
		{
		
		}

		private void limitVideoLengthCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.limitVideoLengthCheckBox.Checked==true)
			{
				try
				{
					CGlobals.mLimitVideoSeconds = float.Parse(this.MaxVideoLengthTextBox.Text,CultureInfo.InvariantCulture);
				}
				catch 
				{
					CGlobals.mLimitVideoSeconds=-1;
				}
			}
			else
			{
				CGlobals.mLimitVideoSeconds=-1;
			}
		}

		private void MaxVideoLengthTextBox_TextChanged(object sender, System.EventArgs e)
		{
			if (this.limitVideoLengthCheckBox.Checked==true)
			{
				try
				{
					CGlobals.mLimitVideoSeconds = float.Parse(this.MaxVideoLengthTextBox.Text,CultureInfo.InvariantCulture);
				}
				catch 
				{
					CGlobals.mLimitVideoSeconds=-1;
				}
			}
		}

		private void Author_Load(object sender, System.EventArgs e)
		{
		
		}

		private void mEncodeAudio_CheckedChanged(object sender, System.EventArgs e)
		{
			CGlobals.mEncodeAudio = this.mEncodeAudio.Checked;
		}

		private void Minimisebutton_Click(object sender, System.EventArgs e)
		{
		//	this.ShowInTaskbar=true;
		//	this.Show();
			this.mMainWindow.Hide();
			this.WindowState = FormWindowState.Minimized;
			
		}

		private void Author_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
		}

		private void Author_SizeChanged(object sender, System.EventArgs e)
		{
			if (this.WindowState!=FormWindowState.Minimized)
			{
				//	this.ShowInTaskbar=false;
				this.mMainWindow.Show();
			}
			else
			{
				this.mMainWindow.Hide();
			}
		}

		private void mTotalCompleteLabel_Click(object sender, System.EventArgs e)
		{
		
		}

		private void mTotalCompleteLabel2_Click(object sender, System.EventArgs e)
		{
		
		}

		private void BackgroundtaskCheckbutton_CheckedChanged(object sender, System.EventArgs e)
		{
			if (mAuthorThread!=null)
			{
				//if (this.BackgroundtaskCheckbutton.Checked==true)
				{
					mAuthorThread.Priority = System.Threading.ThreadPriority.Lowest;
				}
			//	else
			//	{
			//		mAuthorThread.Priority = System.Threading.ThreadPriority.Normal;
			//	}

			}
		}

		private void Author_Closed(object sender, System.EventArgs e)
		{
			//System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Normal;
		}

		private void Author_MinimumSizeChanged(object sender, System.EventArgs e)
		{
			
		}

		private void mCropCombo_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            CGlobals.mCurrentProject.DiskPreferences.FinalDiskCropPercent = int.Parse(this.mCropCombo.Text);
		}

		private void mTVCropCheckboxc_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.mTVCropCheckbox.Checked==true)
			{
				this.mCropCombo.Enabled=true;
                CGlobals.mCurrentProject.DiskPreferences.FinalDiskCropPercent = int.Parse(this.mCropCombo.Text);
			}
			else
			{
				this.mCropCombo.Enabled=false;
                CGlobals.mCurrentProject.DiskPreferences.FinalDiskCropPercent = 0;
			}
		}

		private void mProjectNameTextbox_TextChanged(object sender, System.EventArgs e)
		{
			string a1= (string)this.mVolumeNameTextbox.Text.Clone();

			a1 = a1.ToUpper();
			char [] ass = a1.ToCharArray(); 
			for (int jj=0;jj<ass.Length;jj++)
			{
				if (ass[jj]==' ')
				{
					ass[jj]='_';
				}
				else if ((ass[jj] < 'A' || ass[jj] > 'Z') &&
						 (ass[jj] < '0' || ass[jj] > '9'))
				{
					ass[jj]='_';
				}
			}
			string d2= new string(ass);
			if (d2=="") d2="UNTITLED";
			this.mVolumeNameTextbox.Text=d2;

		}

		private void STartUpDiskDetectionEvent()
		{
			ManagementEventWatcher w = null;
			WqlEventQuery q;
			ManagementOperationObserver observer = new
				ManagementOperationObserver();

			// Bind to local machine
			ConnectionOptions opt = new ConnectionOptions();
			opt.EnablePrivileges = true; //sets required privilege
			ManagementScope scope = new ManagementScope( "root\\CIMV2", opt );

			try
			{
				q = new WqlEventQuery();
				q.EventClassName = "__InstanceModificationEvent";
				q.WithinInterval = new TimeSpan( 0, 0, 1 );

				// DriveType - 5: CDROM
				q.Condition = @"TargetInstance ISA 'Win32_LogicalDisk' and
        TargetInstance.DriveType = 5";
				w = new ManagementEventWatcher( scope, q );

				// register async. event handler
				w.EventArrived += new EventArrivedEventHandler( CDREventArrived );
				w.Start();

				// Do something usefull,block thread for testing
				Console.ReadLine();
			}
			catch( Exception e )
			{
				Console.WriteLine( e.Message );
			}
			finally
			{
				w.Stop();
			}
		}


		private void CDREventArrived(object sender, EventArrivedEventArgs e)
		{
			// Get the Event object and display it
			PropertyData pd = e.NewEvent.Properties["TargetInstance"];
 
			if (pd != null)
			{
				ManagementBaseObject mbo = pd.Value as ManagementBaseObject;
 
				// if CD removed VolumeName == null
				if (mbo.Properties["VolumeName"].Value != null)
				{
					Console.WriteLine("CD has been inserted");
				}
				else
				{
					Console.WriteLine("CD has been ejected");
				}
			}
		}


        // **********************************************************************************************
		private void mNextWizard1_Click(object sender, System.EventArgs e)
		{
			if (this.CreateNewVideoRadio.Checked==true)
			{
				int top = this.mWizardGroup1.Top;
				mWizardGroup1.Top = this.mWizardGroup2.Top;
				this.mWizardGroup2.Top = top;
                this.WriteToOpticalMediaCheckBox.Checked = true;
			}
            else if (this.EncodeOnlyRadioButton.Checked == true)
            {
                int top = this.mWizardGroup1.Top;
                mWizardGroup1.Top = this.mWizardGroup2.Top;
                this.mWizardGroup2.Top = top;
                this.WriteToOpticalMediaCheckBox.Checked = false;
            }
            else if (this.BurnPreviousProjectRadio.Checked == true)
            {
                //
                // Windows strore version does not contain external tool
                // so we just use interal burner with a selected encoded path
                //
                if (CGlobals.RunningWindowsStoreVersion == true)
                {
                    CBurnPreviousEncodedProjectWindow bpepw = new CBurnPreviousEncodedProjectWindow();
                    bpepw.ShowDialog();

                    string path = bpepw.SelectedPath;
                    //
                    // Nothing valid selected just return
                    //
                    if (path == "")
                    {
                        return;
                    }

                    //
                    // Setup type and path we are going to burn
                    //
                    mTheVideoPath = path;
                    mSelectedVideoFolderBurnType = bpepw.SelectedType;
                    RecalcBurnType();

                    //
                    // Switch to burn screen
                    //
                    int top = this.mWizardGroup1.Top;
                    mWizardGroup1.Top = this.mWizardGroup3.Top;
                    this.mWizardGroup3.Top = top;
                }
                else
                {
                    CRunExternalDVDBurnTool.RunTool();
                    this.Close();

                }
            }
            else
            {
                int top = this.mWizardGroup1.Top;
                mWizardGroup1.Top = this.mWizardGroup3.Top;
                this.mWizardGroup3.Top = top;
            }

		}

		private void mPreviousWizard2_Click(object sender, System.EventArgs e)
		{
			int top = this.mWizardGroup2.Top;
			mWizardGroup2.Top = this.mWizardGroup1.Top;
			this.mWizardGroup1.Top = top;
		}

		private void mCancelWizard1_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void mCancelWizard2_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void mNextWizard2_Click(object sender, System.EventArgs e)
		{
            bool do_pics = this.mIncludeOriginalPicturesTickBox.Enabled == true && this.mIncludeOriginalPicturesTickBox.Checked == true;
            bool do_vids = this.mIncludeOriginalVideosTickBox.Enabled == true && this.mIncludeOriginalVideosTickBox.Checked == true;


            if ((do_pics || do_vids) && this.mDiskEstimateSizeTextBox.ForeColor == Color.Red)
            {
                DialogResult res = DoMessageBoxOnFormThread("The estimated final disc image with the included original material is too big for the blank media disc, are you sure you wish to continue with the burn? (not recommended)", "Disc image too big?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (res == DialogResult.Cancel) return;
            }

			int top = this.mWizardGroup2.Top;
			mWizardGroup2.Top = this.mWizardGroup3.Top;
			this.mWizardGroup3.Top = top;
		}

		private void mPreviousWizard3_Click(object sender, System.EventArgs e)
		{
			if (BurnFromFolderRadio.Checked==false &&
                BurnPreviousProjectRadio.Checked==false)
			{
				int top = this.mWizardGroup3.Top;
				mWizardGroup3.Top = this.mWizardGroup2.Top;
				this.mWizardGroup2.Top = top;
			}
			else
			{
				int top = this.mWizardGroup3.Top;
				mWizardGroup3.Top = this.mWizardGroup1.Top;
				this.mWizardGroup1.Top = top;

			}

		}

		private void mWizardGroup2_Enter(object sender, System.EventArgs e)
		{
		
		}

        private void mRunExternalBurnToolButton_Click(object sender, EventArgs e)
        {
            CRunExternalDVDBurnTool.RunTool();
        }


        // *************************************************************************************
        private void mMp4FileLocation_Click(object sender, EventArgs e)
        {
            
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = System.IO.Path.GetFileName(mCreateInFolderTextBox.Text);
            sfd.Filter = "MPEG-4 (*.mp4)|*.mp4";
            sfd.FilterIndex = 1;
            sfd.RestoreDirectory = true;
            sfd.InitialDirectory = System.IO.Path.GetDirectoryName(mCreateInFolderTextBox.Text);

            DialogResult dr = sfd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                mCreateInFolderTextBox.Text = sfd.FileName;
            }
        }

        //****************************************************************************
        private void mShowImageCache_Click(object sender, EventArgs e)
        {
            ImageCacheViewer ic = new ImageCacheViewer();
            ic.Show();
        }

        //****************************************************************************
        private void mMotionBlurHelpToolStripButton_Click(object sender, EventArgs e)
        {
            CustomButton.MotionBlurHelp mbh = new CustomButton.MotionBlurHelp();
            mbh.ShowDialog();
        }


        //****************************************************************************
        private void mDebugShowVideoCache_Click(object sender, EventArgs e)
        {
            VideoCacheViewer vcv = new VideoCacheViewer();
            vcv.Show();

        }

        //****************************************************************************

        private void mDebugShowImageCache_Click(object sender, EventArgs e)
        {
            ImageCacheViewer ic = new ImageCacheViewer();
            ic.Show();
        }
    }

    public class BurnerProgressCallback : MangedToUnManagedWrapper.IVideoDiskCallback
	{

		private Author mAu;
		private bool mDoingPadding= false;
		public BurnerProgressCallback(Author au)
		{
			mAu = au;
		}

        //*******************************************************************
		public override void PadStartedCallback()
		{
			mAu.SetCurrentProcessText("Padding disk to 1Gb...Please wait");
            mAu.mDoingPaddingProcess = true;
			mDoingPadding=true;
		//	mAu.GetPercentageDone().Value = 0;
		}

        //*******************************************************************
		public override void BurnPercentageComplete(int amount)
		{
            // cd burning
            if (amount == 1098)
            {
                amount = 98;
                mAu.SetCurrentProcessText("Burning... Please wait until disk is finished...");
            }
            else
            {
                if (amount < 0) amount = 0;
                if (amount > 100) amount = 100;

                if (amount > 0 && amount < 99 && mDoingPadding == false)
                {
                    mAu.SetCurrentProcessText("Burning...");
                }

                if (amount >= 99 && mDoingPadding == true)
                {
                    mAu.SetCurrentProcessText("Finalizing... Wait for disk to finish!!");
                    mAu.mBurnReachFinalizeStage = true;
                }

            }


            mAu.SetProgressBar(amount);
		}

	
      	//*******************************************************************
		public override void BufferStatusCallback(int percent_done,int bufferFreeSizeInUCHARs,int BufferSizeInUCHARs)
		{
			if (percent_done<0) percent_done=0;
			if (percent_done>100) percent_done=100;

			mAu.SetBufferFull(percent_done);
		}

	}
}
