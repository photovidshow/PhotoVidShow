//#define DESIGN_MODE
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using ManagedCore;
using DVDSlideshow;
using DVDSlideshow.Memento;
using MangedToUnManagedWrapper;
using CustomTabControl;
using Microsoft.Win32;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Collections.Generic;
using DVDSlideshow.GraphicsEng.Direct3D;
using DVDSlideshow.GraphicsEng;
using CustomButton;

// for 16:9

//1156, 706 client size
//1010,744 min size


namespace dvdslideshowfontend
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class Form1 : System.Windows.Forms.Form, DVDSlideshow.Memento.IOriginator
    {
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.PictureBox pictureBox1;

        // dpi settings of computer run on, these will be re set on start up
        private static float dpiX = 96;
        private static float dpiY = 96;
        private ImageList mOutputImageList;
        private ToolStripDropDownButton mOutputDiskTypeDropDownButton;
        private ToolStripMenuItem videoForCompuerToolStripMenuItem;
        private ToolStripMenuItem videoForComputerOrWebToolStripMenuItem;

        private ToolStripMenuItem blurayVideoDiskToolStripMenuItem;
        private ToolStripDropDownButton mOutputAspectDropDownButton;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem wdwdwdToolStripMenuItem;
        private ToolStripButton mCreateAndBurnButton;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton mCreateVideoButton;
        private ToolStripLabel mOutputToolStripLabel;
        private ToolStrip mTopToolStripBar;
        private ToolStripButton mNewToolStripButton;
        private ToolStripButton mOpenToolStripButton;
        private ToolStripButton mSaveToolStripButton;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton mUndoToolStripButton;
        private ToolStripButton mRedoToolStripButton;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripSplitButton mDVDMenuToolStripButton;
        private ToolStripSeparator mDVDMenuToolStripSeperator;
        private TabPage tabPage2;
        private GroupBox groupBox2;
        private SetStartEndSlideTime mDecorationStartEndSlideTimeControl;
        private TrackBar ClipartTransparencyTrackerBar;
        private Label label6;
        private Label mForceEnlargeLabel;
        private Label label13;
        private Label mDecorationRotationLabel;
        private Button OrderLayersButton;
        private NumericUpDown mDecorationPositionLeftTextBox;
        private NumericUpDown mDecorationPositionHeightTextBox;
        private NumericUpDown mDecorationPositionWidthTextBox;
        private NumericUpDown mDecorationPositionTopTextBox;
        private Label label18;
        private Button mEffectsEditorButton;
        private Label ClipartTransparencyLabel;
        private ComboBox mSelectedDecorationCombo;
        private TabControl DecorationTabOptions;
        private TabPage AddTextTab;
        private CheckBox BackPlaneCheckbox;
        private Button ColourPickerBackPlane;
        private Label mTextTemplateTypeLabel;
        private TrackBar BackPlaneTransparencyTrackBar;
        private ComboBox mTextTemplateTypeCombo;
        private Label BackPlaneTransparencyLabel;
        private Button mFontButton;
        private Button mEditTextButton;
        private RadioButton RightAlignedRadioBox;
        private ComboBox SizeComboBox;
        private Button AddTextButton;
        private RadioButton CentreAlignedRadioBox;
        private RadioButton LeftAlignedRadioBox;
        private TabPage ClipArtTab;
        private CheckBox mTemplateFrameworkImageCheckbox;
        private ComboBox mTemplateImageNumberCombo;
        private ComboBox mTemplateAspectCombo;
        private Button AddClipArtButton;
        private Button EditImageButton;
        private ComboBox mRenderMethodCombo;
        private CheckBox mClipArtLockAspectCheckBox;
        public Button AddNewImageButton;
        private Button RotateDecorationClockWiseButton;
        private Button RotateDecorationAntiClockWiseButton;
        private Button FlipDecorationLeftRightButton;
        private Button FlipDecorationUpDownButton;
        private TrackBar mDecorationRotationTrackbar;
        private Label Transparentlabel;
        private PictureBox mFontPreviewPictureBox;
        private CheckBox mZoomingMenuBackGroundCheckBox;
        private Button mPreviewMenuButton;
        private ToolStripMenuItem openExplorerWindowToolStripMenuItem;
        private ToolStrip mHelpToolStrip;
        private ToolStripButton mHelpButton;
        private BitmapButton.SimpleBitmapButton mRewindToStartSlideshowButton;
        private CheckBox mReCalcPanZoomTemplateCheckbox;
        private Label label5;
        private ComboBox mPanZoomUseEquationComboBox;
        private MenuStrip mTopMenuStrip;
        private ToolStripMenuItem mFileToolStripMenuItem;
        private ToolStripMenuItem mNewProjectToolStripMenuItem;
        private ToolStripMenuItem mOpenProjectToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem mImportSlideshowsToolStripMenuItem;
        private ToolStripMenuItem mImportTemplateToolStripMenuItem;
        private ToolStripMenuItem mExportTemplateToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem mSaveProjectToolStripMenuItem;
        private ToolStripMenuItem mSaveProjectAsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem mDisplayToolStripMenuItem;
        private ToolStripMenuItem mToolsToolStripMenuItem;
        private ToolStripMenuItem mHelpToolStripMenuItem;
        private ToolStripMenuItem mDebugToolStripMenuItem;
        private ToolStripMenuItem mExitToolStripMenuItem;
        private ToolStripMenuItem mShowSelectedSlideshowToolStripMenuItem;
        private ToolStripMenuItem mHelpToolStripMenuItem1;
        private ToolStripMenuItem mCheckForUpdatesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripMenuItem mVisitPhotoVidShowWebsiteToolStripMenuItem;
        private ToolStripMenuItem mAboutToolStripMenuItem;
        private ToolStripMenuItem mShowMementoStackToolStripMenuItem;
        private ToolStripMenuItem mShowImagesCacheToolStripMenuItem;
        private ToolStripMenuItem mShowTextBoundaryToolStripMenuItem;
        private ToolStripMenuItem mPurchaseToolStripMenuItem;
        private ContextMenuStrip mDecorationsContextMenuStrip;
        private ToolStripMenuItem mBringToFrontToolStripMenuItem;
        private ToolStripMenuItem mSendToBackToolStripMenuItem;
        private ToolStripMenuItem mDeleteToolStripMenuItem;
        private ImageList mGlobalMenuItemIcons;
        private NumericUpDown mDecorationRotationNumericArrows;
        private NumericUpDown mClipartTransparencyNumerical;
        private ToolStripMenuItem mVideoImportToolStipItem;
        private ToolStripMenuItem mUseFFDShowForMovVideos;
        private ToolStripMenuItem mUseFFDShowForMp4Videos;
        private ToolStripMenuItem mUseFFDShowForMtsVideos;
        private ToolStripMenuItem setDefaultProjectTypeToolStripMenuItem;
        private ToolStripMenuItem videoFileToolStripMenuItem;
        private ToolStripMenuItem dVDVideoToolStripMenuItem;
        private ToolStripMenuItem blurayToolStripMenuItem;
        private ToolStripMenuItem mAddOrEditMusicSlidesToolStripMenuItemMenuItem;
        private TabControl mMenuThemeTab;
        private TabPage mMenuBackgroundTab;
        private TabPage mMenuLayoutTab;
        private TabPage mMenuFrameStyleTab;
        private TabPage mMenuNavigationStyleTab;
        private TabPage mMenuHighlightStlyeTab;
        private Panel mMenuNavigationButtonsPanel;
        private Label mMenuNavigationButtonsLabel;
        private CheckBox mMenuNavigationPlayAllLoopedButton;
        private CheckBox mMenuNavigationPlayAllButton;
        private ComboBox mMenuHighlightPlayButtonStyleCombo;
        private ComboBox mMenuHighlightNavigationButtonStyleCombo;
        private ComboBox mMenuHighlightSlideshowButtonStyleCombo;
        private Button mMenuHighlightColorButton;
        private Label mMenuHighlightSlideshowButtonsLabel;
        private Label mMenuHighlightPlayButtonsLabel;
        private Label mMenuHighlightNavigationButtonLabel;
        private Label mMenuHighlightColorLabel;
        private Label mMenuHighlightStyleLabel;
        private TabPage mIntroSlideshowTabPage;
        public TabControlEx mIntoSlideshowTabControl;
        private TabPage mIntroSlideshowInnerTabPage;
        private CheckBox mIncludeIntroVideoCheckBox;
        private Label mInclideIntroSlideshowLabel;
        private ToolStripComboBox mSelectSlideshowToolStripItemCombo;
        private Button mDiskIntroSlideshowImportSlideshowButton;
        private Label label23;
        private ImageList mIntroVideoTabPagesImageList;
        private Button mMenuHighlightResetButton;
        private ToolStripMenuItem mIntroSlideshowToolStripMenuItem;
        private ImageList mDiskMenuImageList;
        private ToolStripMenuItem setMaxVideoLoadedAtOnceToolStripMenuItem;
        private ToolStripMenuItem mSetDefaultFolderLocationsMenuItem;
        private ToolStripMenuItem showVideoCacheToolStripMenuItem;
        private Button mDoneDecorationMenuButton;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem mLaunchDVDBurnerToolToolStripMenuItem;
        private ToolStripMenuItem tutorialsToolStripMenuItem;
        private ToolStripMenuItem mVisitTutorialsToolStripMenuItem;
        private ToolTip mMainToolTip;
        private Label label10;
        private Label label4;
        private Label label14;
        private Label label11;
        private List<TabPage> mOriginalDecorationTabOption = new List<TabPage>();

        public List<TabPage> GetOriginalDecorationTabOptions()
        {
            return mOriginalDecorationTabOption;
        }

        public System.Windows.Forms.Panel slideshowpanel;
        private System.Windows.Forms.Panel panel3;
#if (DESIGN_MODE)
        public Panel scrollslideshowpanel;
#else
        public ManualScrollablePanel scrollslideshowpanel;
#endif
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabControl mOuterTabControl;
        private System.Windows.Forms.Label label2;

        public DVDSlideshow.CProject mCurrentProject;


        private CSlideShowManager mSlideShowManager;
        private CSlideShowMusicManager mSlideShowMusicManager;
        private CSlideShowNarrationManager mSlideShowNarrationManager;

        private System.Windows.Forms.TrackBar mStoryboardTrackBar;
        private System.Windows.Forms.Timer SlideShowTimer;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.MainMenu mainMenu2;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private CustomTabControl.TabControlEx mMainMenuTabControl;
        private System.Windows.Forms.TabPage mMenuStructureTabPage;
        private System.Windows.Forms.TabPage mEmptyMenuTabPage;

        private CDecorationsManager mDecorationManager;
        private CDiskMenuManager mDiskMenuManager;
        private CDiskEstimationManager mDiskEstimationManager;
        private System.Windows.Forms.OpenFileDialog LoadProjectDialog;
        private System.Windows.Forms.SaveFileDialog SaveProjectDialog;

        private System.Windows.Forms.NotifyIcon notifyIcon1;



        private System.Windows.Forms.Label SlideShowTimeLabel;

        private System.Windows.Forms.TrackBar StartPanTrackBar;
        private System.Windows.Forms.TrackBar EndPanTrackBar;
        public TabControlEx mSlideOptionsTab;
        private System.Windows.Forms.TabPage ClipPanTabPage;
        private System.Windows.Forms.Label StartPanTrackBarLabel;
        private System.Windows.Forms.Label EndPanTrackBarLabel;


        public static Form1 mMainForm = null;
        private System.Windows.Forms.PictureBox DiskEstimationInnerPictureBox;
        private System.Windows.Forms.TabPage mMenuThemeTabPage;

        private ErrorLog mErrorLog;

        public static bool locked = false;
        private BitmapButton.SimpleBitmapButton NewSlideShow;
        private System.Windows.Forms.Button AddNextMenuButton;

        private System.Windows.Forms.ColorDialog colorDialog2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox MenuBackgroundMusicName;
        private System.Windows.Forms.Panel MenuTemplatesPanel;
        private System.Windows.Forms.Button SetMenuBackgroundButton;
        private System.Windows.Forms.TextBox MenuBackgroundName;
        private System.Windows.Forms.Button AddBackgroundMenuMusicButton;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TrackBar MenuRepeatLengthTrackbar;
        public Button MenuAddDecorateButton;
        private System.Windows.Forms.Button RemoveSlideshowButton;

        private bool mForceNoMemento = false;
        public bool mControlKeyOn = false;
        public bool mShiftKeyOn = false;
        private BitmapButton.SimpleBitmapButton mPlaySlideshowButton;
        private System.Windows.Forms.Label TotalSlidesSlideshowTextBox;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label mTotalMenuStatsLabel;
        private System.Windows.Forms.Label mMenuIndexLabel;
        private System.Windows.Forms.Panel MenuLayoutsPanel;
        private System.Windows.Forms.Panel MenuStylePanel;
        private System.Windows.Forms.Label MenuRepeatDurationTextbox;
        private System.Windows.Forms.Label MenuThemeLabel;
        private System.Windows.Forms.Label blankout;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenu ResetMenuMusicContextMenu;
        private System.Windows.Forms.MenuItem menuItem17;
        private System.Windows.Forms.ImageList DecoratePanZoomImageList;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Button RandomPanZoomButton;
        private BitmapButton.SimpleBitmapButton mStopSlideshowButton;
        public ToolTip mToolTip = null;
        private System.Windows.Forms.PictureBox LogoPictureBox;
        private AddSlidesToolIpBalloon mAddSlidesBalloon = null;//new AddSlidesToolIpBalloon();

        private string mOpenWithFile;
        private System.Windows.Forms.CheckBox mMenuThemePlayingThumbnailsCheckBox;
        private System.Diagnostics.Process mHelpProcess = null;
        private System.Windows.Forms.ImageList MenuItemsImageList;
        private System.Windows.Forms.TabPage mPreDesignedSlidesTabPage;
        private PictureBox mPreviewPanZoomPictureBox;
        private TrackBar mPanZoomInitialDelayTrackBar;
        private Label mPanZoomInitialDelayLabel;
        private Label mPanZoomEndDelayLabel;
        private TrackBar mPanZoomEndDelayTrackBar;
        private CheckBox mTurnOffPanZoomForSlideTickBox;
        private Label mEndPanDelayLabel;
        private Label mStartPanDelayLabel;
        private Panel mHiddenDisplayWindow;
        private FormWindowState mLastWindowState = FormWindowState.Normal;
        private TabPage mBordersTab;
        private TabPage mFiltersTabPage;
        private CustomButton.SlideFiltersControl mFiltersControl;
        private CustomButton.PredefinedSlideDesignsControl mPredefinedSlideDesignsControl1;
        private CustomButton.OverlaySelectionControl mOverlaySelectionControl1;
        private Label mForceEnlargeLabel2;
        private Panel InnerDiskEstimationPanel;
        private BitmapButton.SimpleBitmapButton mImportSlideshowButton;
        private PictureBox pictureBox3;
        private TabPage mBackgroundTabPage;
        private SelectBackgroundControl mSelectBackgroundControl1;
        private Button mPanZoomLinkStartAreaButton;
        private Label label17;
        private Label StartRotationTrackBarLabel;
        private TrackBar StartPanRotationTrackBar;
        private Label label20;
        private Label EndRotationTrackBarLabel;
        private TrackBar EndPanRotationTrackBar;
        private Panel panel7;
        private ToolStrip mStoryboardToolStrip;
        private ToolStripSplitButton SlideshowAddSlidesButton;
        private ToolStripMenuItem AddBlankSlideMenuItemButton;
        private ToolStripMenuItem mCreateSlideFromTemplateToolStripMenuItem;
        private ToolStripSplitButton SlideshowAddMusicButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSplitButton mDoNarrationButton;
        private ToolStripMenuItem mAddNarrationFromAudioFileToolStripMenuItem;
        private ToolStripButton mStoryboardEditItemButton;
        private ToolStripButton mDeleteButton;
        private ToolStripButton RotateSlideAntiClockwiseButton;
        private ToolStripButton RotateSlideClockWiseButton;
        private ToolStripButton mSetMenuthumbnailButton;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton SlideshowAdvanceOptionsButton;
        private CheckBox mPanZoomOnAllCheckBox;
        private bool mDoingGoToMainMenu = false;
        private bool mShowingStartupOptions = true;

        public bool ShowingStartupOptions
        {
            get { return mShowingStartupOptions; }
            set { mShowingStartupOptions = value; }
        }

        // hack we want to prompt user to update if first time they have run program
        // but we don't want to hassle older customers who are just doing an update
        private bool mHadLicenceBeforeAppStarted = false;

        //
        // If set to true, we do not change the tab in the menus (i.e. intro or structure)
        //
        private bool mForceNoMenuTabChange = false;


        private void myKeyPressEventHandler(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            //	if (e.Control)
            {
                int i = 1;
                i++;
                i++;
            }
        }

        //*******************************************************************
        public static void ReduceFontSizeToMatchDPI(Control.ControlCollection collection)
        {
            // adjust controls to match dpi settings if not default 96 dpi
            if (Math.Abs(dpiX - 96.0) < 0.01) return;

            foreach (Control control in collection)
            {
                // If panel or group box, ignore setting font on this control 
                // as this can auto set child controls, which is not what we want.
                if (control is Panel == false && control is GroupBox == false)
                {
                    float size = control.Font.Size * (96.0f / dpiX);

                    FontStyle style = System.Drawing.FontStyle.Regular;
                    if (control.Font.Bold == true)
                    {
                        style = System.Drawing.FontStyle.Bold;
                    }
                    if (control.Font.Italic == true)
                    {
                        style = System.Drawing.FontStyle.Italic;
                    }

                    if (CGlobals.IsWinVistaOrHigher())
                    {
                        control.Font = new System.Drawing.Font("Segeo UI", size, style, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    }
                    else
                    {
                        control.Font = new System.Drawing.Font("Arial", size, style, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    }
                }

                ReduceFontSizeToMatchDPI(control.Controls);
            }
        }

        //*******************************************************************
        public Form1(string open_with_file)
        {
            // work out dpi settings for computer being used
            using (Graphics graphics = this.CreateGraphics())
            {
                dpiX = graphics.DpiX;
                dpiY = graphics.DpiY;
                CGlobals.dpiX = dpiX;
                CGlobals.dpiY = dpiY;
            }

            // ok .net2 works different get region info BEFORE we set culture info
            string iso_name = System.Globalization.RegionInfo.CurrentRegion.ThreeLetterISORegionName;

            // set the culture to american so we can parse our xml strings
            // ok
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            mOpenWithFile = open_with_file;

            string logo = CGlobals.GetRootDirectory() + "\\logo\\logo.jpg";

            CDebugLog.GetInstance().Trace("Form1 started");

            // clear out any old temp files
            ManagedCore.CryptoFS.ClearDFiles();

            // This is first time we access PhotoVidShow in user directory.  If this returns unauthrised exception, it may mean we have 
            // re-installed windows on itself locking these files.  This needs to be reported.

            string license_file = "";

            //
            // if running stream and 'steam_appid.txt' files exists it means we have overridden the steam protection system
            // Therefore we run in normal mode (i.e. trial/photovidshow licence key)
            //


            if (CGlobals.RunningSteamVersion == true && CGlobals.RunningSteamDemo == false && System.IO.File.Exists(CGlobals.GetRootDirectory()+ "\\steam_appid.txt") == false)
            {
                //
                // Steam version does not do PhotoVidShow license keys
                //
                ManagedCore.License.License.Valid = true;
            }

            //
            // If running Windows store version as non trial then licence must be valid
            //
            if (CGlobals.RunningWindowsStoreVersion == true && CGlobals.RunningWindowsStoreTrial == false)
            {
                //
                // Windows store version does not do PhotoVidShow license keys
                //
                ManagedCore.License.License.Valid = true;
            }

            if (ManagedCore.License.License.Valid == false)
            {
                try
                {
                    //
                    // Check for new license file
                    // 
                    license_file = CGlobals.GetUserDirectory() + "\\photovidshow run config(do not delete).txt";  // new software license file
                    if (System.IO.File.Exists(license_file) == false)
                    {
                        //
                        // Try old license file
                        //
                        license_file = CGlobals.GetUserDirectory() + "\\license.txt";  // old software license file
                    }
                    if (System.IO.File.Exists(license_file) == true)
                    {
                        ManagedCore.License.CSoftwareKey key = new ManagedCore.License.CSoftwareKey("");
                        key.Load(license_file);
                        int key_num = 0;
                        if (key.IsValidForThisComputer(1, ref key_num) == true)
                        {
                            ManagedCore.License.License.Valid = true;
                            ManagedCore.License.License.RunningWithKey = key;
                            ManagedCore.License.License.RunningKeyNum = key_num;
                        }
                    }
                }
                catch (System.UnauthorizedAccessException exception)
                {
                    UserMessage.Show("Your windows user account does not have read rights access for file '" + license_file + "'\r\n\r\nPlease change so this file has read access for this user account as well as all the files stored in " + CGlobals.GetUserDirectory() + "\r\n\r\n" + exception.Message, "Read access denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            // try license2
            if (ManagedCore.License.License.Valid == false)
            {
                //
                // Try new license2 file first
                //
                string license_file2 = CGlobals.GetUserDirectory() + "//photovidshow run config2(do not delete).txt";  // new software license file
                if (System.IO.File.Exists(license_file2) == false)
                {
                    //
                    // try old license2 file
                    //
                    license_file2 = CGlobals.GetUserDirectory() + "\\license2.txt"; // old software license file
                }
                if (System.IO.File.Exists(license_file2) == true)
                {
                    ManagedCore.License.CSoftwareKey key2 = new ManagedCore.License.CSoftwareKey("");
                    key2.Load(license_file2);
                    int key_num = 0;
                    if (key2.IsValidForThisComputer(1, ref key_num) == true)
                    {
                        ManagedCore.License.License.Valid = true;
                        ManagedCore.License.License.RunningWithKey = key2;
                        ManagedCore.License.License.RunningKeyNum = key_num;
                    }

                }
            }        

            SplashForm.StartSplash(logo, Color.FromArgb(128, 128, 128), false);

            //
            // Associate .pds, .pds2 file extensions with PVS. This should of been done already when installing. But just in case.
            //
            // This does not seem to work on windows 7. Works with XP and Vista though.  On vista this throws an exception if not running in admin mode AND
            // the project files are not associated.  Therefore added try catch loop just in case.
            //
            try
            {

                if (!IsAssociated(".pds"))
                    Associate(".pds", "PhotoVidShow.ProjectFile", "PhotoVidShow project", "AppIcons.ico", System.Environment.GetCommandLineArgs()[0]);

                if (!IsAssociated(".pds2"))
                    Associate(".pds2", "PhotoVidShow.ProjectFile", "PhotoVidShow project", "AppIcons.ico", System.Environment.GetCommandLineArgs()[0]);
            }
            catch
            {
            }

            mMainForm = this;

            // 
            // Required for Windows Form Designer support
            //

            CDebugLog.GetInstance().Trace("Doing init components");

            InitializeComponent();

            // Shrink outer tab control
            mOuterTabControl.Location = new Point(-8, -6);

            // set menu tab control ex image list
            List<string> tabImages = new List<string>();
            tabImages.Add("menustructure.png");
            tabImages.Add("menutheme.png");
            tabImages.Add("blank.png");
            mMainMenuTabControl.SetTabImages(tabImages);

            foreach (TabPage page in mSlideOptionsTab.TabPages)
            {
                this.GetPreDesignedSlidesTabPage();

                mOriginalDecorationTabOption.Add(page);
            }


            MiniPreviewController.mGlobalGuiFormThread = this;

            // Create a hidden window to allow directx to present to, when we don't want to draw anthing to the screen
            mHiddenDisplayWindow = new Panel();
            mMainForm.Controls.Add(mHiddenDisplayWindow);
            mHiddenDisplayWindow.Width = 0;
            mHiddenDisplayWindow.Height = 0;

            // Disable import/export template if not a template user
            if (CGlobals.mIsTemplateUser == false)
            {
                mImportTemplateToolStripMenuItem.Visible = false;
                mExportTemplateToolStripMenuItem.Visible = false;
            }
            else
            {
                mReCalcPanZoomTemplateCheckbox.Visible = true;
            }

            if (CGlobals.mIsDebug == false)
            {
                this.mTopMenuStrip.Items.Remove(this.mDebugToolStripMenuItem);
            }

            mToolTip = new ToolTip();
            mToolTip.ShowAlways = true;

            this.mRedoToolStripButton.Enabled = false;
            this.mUndoToolStripButton.Enabled = false;

            CDebugLog.GetInstance().Trace("Done init components");
            // hide tab controls so only accesible via buttons
            this.mOuterTabControl.Region =
                new Region(
                new RectangleF(
                this.tabPage1.Left,
                this.tabPage1.Top,
                this.tabPage1.Width,
                this.tabPage1.Height));

            this.mMainMenuTabControl.SelectedIndex = 0;

            this.pictureBox1.Image = new Bitmap(10, 10);

            //
            CDebugLog.GetInstance().Trace("Creating error log");

            mErrorLog = new ErrorLog();

            if (CGlobals.mIsDebug == true)
            {
                mErrorLog.StartShow();
            }

            mUseFFDShowForMovVideos.Checked = UserSettings.GetBoolValue("VideoSettings", "ForceUseFFDShowIfInstalledForMov");
            mUseFFDShowForMp4Videos.Checked = UserSettings.GetBoolValue("VideoSettings", "ForceUseFFDShowIfInstalledForMp4");
            mUseFFDShowForMtsVideos.Checked = UserSettings.GetBoolValue("VideoSettings", "ForceUseFFDShowIfInstalledForMts");

            // is ffmpeg installed and we're windows 7 or above, add video import settings option
            if (CGlobals.IsWin7OrHigher() == true && IsFFDShowInstalled() == true)
            {
                mToolsToolStripMenuItem.DropDownItems.Add(mVideoImportToolStipItem);
            }

            //
            // If running Windows Store version remove the tools -> Launch dvd burner tool
            //
            if (CGlobals.RunningWindowsStoreVersion == true)
            {
                mTopMenuStrip.Items.Remove(settingsToolStripMenuItem);
            }

            CDebugLog.GetInstance().Trace("Creating project");
            mCurrentProject = new CProject();

            // remove tabs we're currently not using
            UpdateOuterTabControlTabs();

            System.EventArgs e = new System.EventArgs();
            SlideShowTimer.Stop();

            this.pictureBox1.Visible = false;

            CDebugLog.GetInstance().Trace("Creating CDecorationsManager");
            mDecorationManager = new CDecorationsManager(this.tabPage2, panel4, this.pictureBox1, mDecorationsContextMenuStrip, this);

            CDebugLog.GetInstance().Trace("Creating CDiskEstimationManager");
            mDiskEstimationManager = new CDiskEstimationManager();


            CDebugLog.GetInstance().Trace("Creating CDiskMenuManager");
            mDiskMenuManager = new CDiskMenuManager(this, this.MenuTemplatesPanel, this.MenuLayoutsPanel, this.MenuStylePanel, this.ResetMenuMusicContextMenu);

            List<string> fontSizesStrings = new List<string>();
            for (int i = 8; i < 100; i += 2)
            {
                fontSizesStrings.Add(i.ToString());
            }
            SizeComboBox.Items.AddRange(fontSizesStrings.ToArray());
            SizeComboBox.Text = "26";

            CProject.InformWhenProjectChange.Add(new ProjectChanged(this.ProjectHasChanged));

            System.EventArgs e2 = new System.EventArgs();
            this.ViewingAreaResize(this, e2);
            this.panel4.Resize += new EventHandler(this.ViewingAreaResize);
            this.ResizeEnd += new EventHandler(this.ViewingAreaResizeEnd);

            //
            // is this computer in a ntsc region?
            //
            if (iso_name == "USA" ||
                iso_name == "CAN" ||
                iso_name == "JPN" ||
                iso_name == "MEX" ||
                iso_name == "KOR" ||
                iso_name == "VEN" ||
                iso_name == "PHL" ||
                iso_name == "CHL"       // Chile
                //  || iso_name == "GBR")	// use this to test as this machine was in america
                )
            {
                CDebugLog.GetInstance().Trace("Setting to NTSC");
                CGlobals.mCurrentProject.DiskPreferences.SetToNTSC();
            }
            else
            {
                //
                // Default is PAL, most of world and all of Europe
                //
                CDebugLog.GetInstance().Trace("Setting to PAL");
                CGlobals.mCurrentProject.DiskPreferences.SetToPAL();
            }

            // Load moving effects database
            CAnimatedDecorationEffectDatabase.GetInstance().Inport();

            // Initialise graphics engine 
            // Set up direct3D as default graphics engine
            Direct3DDevice device = new Direct3DDevice();

            Control window = panel4;
            GraphicsEngineInitialiseResult result = device.Initialise(window, (uint)panel4.Width, (uint)panel4.Height, mHiddenDisplayWindow);

            if (result.InitialisedOK == false)
            {
                UserMessage.Show("PhotoVidShow could not detect Microsoft DirectX 9.\r\n\r\nTry updating your graphics card driver.\r\n\r\nIf running Windows XP, try installing Microsoft DirectX 9.0c if not already installed.", "Failed to initialise", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                Environment.Exit(0);
            }

            if (result.Capabilities.CanDoPixelShader20 == false)
            {
                UserMessage.Show("Sorry, the computer's hardware does not support pixel shader (v2.0).\r\n\r\nPhotoVidShow requires this ability to run correctly. ", "No pixel shader 2.0 ability", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

            if (result.Capabilities.CanDoAutoMipMappingCreation == false)
            {
                UserMessage.Show("Sorry, the computer's hardware does not support automatic mipmap generation.\r\n\r\nPhotoVidShow requires this ability to run correctly.\r\n\r\nUpdating the computers graphics card driver may solve this problem.", "No automatic mipmap generation ability", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

            if (result.Capabilities.CanDo64BitTextures == false)
            {
                UserMessage.Show("The computer's hardware does not support 64 bit render targets.  This is required to do motion blur effects.  This feature will be disabled.\r\n\r\nUpdating the computers graphics card driver may solve this problem.", "No 64 bit texture support", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DVDSlideshow.Encoder.HardwareSupportsMotionBlur = false;
            }

            GraphicsEngine.Current = device;

            if (CDebugLog.GetInstance().CreatedLogFileThisSession() == true)
            {
                CDebugLog.GetInstance().LogToLogFile(device.GetLogCapabilitiesString());
            }

            // ok we have an engine, draw bakcground image slides
            GetSelectBackgroundControl().PopulateBackgroundListView();
            this.mDiskMenuManager.AddMenuTemplatesThumbnailsToTab();

            CDebugLog.GetInstance().Trace("Doing new");
            
           

            // this then reset's to picturebox size, needed for some reason
            this.ViewingAreaResizeEnd(this, e);

            CDebugLog.GetInstance().Trace("Waiting for registration");
            if ( WaitForRegistration() == false)
            {
                return;
            }


            if (ManagedCore.License.License.Valid == false)
            {
                FrontEndGlobals.mApplicationName += " - Evaluation Copy";
            }

            this.TopMost = true;
            this.BringToFront();
            this.Focus();
            this.TopMost = false;

            //	this.BringToFront();
            //	this.Focus();

            CDebugLog.GetInstance().Trace("Done Form1 construction");

            this.KeyDown += new KeyEventHandler(this.OnKeyDown2);
            this.KeyUp += new KeyEventHandler(this.OnKeyUp2);
            this.KeyPreview = true;

            mAddSlidesBalloon = new AddSlidesToolIpBalloon();

            ReCalcTitleBarString();

            this.Move += new EventHandler(this.MoveWindow);

            this.Paint += new PaintEventHandler(this.CheckForOpenWithFile);

            mToolTip.SetToolTip(this.mPlaySlideshowButton, "Play");
            mToolTip.SetToolTip(this.mRewindToStartSlideshowButton, "Go to start");
            mToolTip.SetToolTip(this.mStopSlideshowButton, "Stop");
            mToolTip.SetToolTip(this.mStoryboardTrackBar, "Seek");

            //
            // By default the GUI sets video as the output type, but this can change if it is set
            // in the UserSettings or by an initial query project type form
            //

            NewProject(true);

           
        }

        //*******************************************************************
        private bool IsFFDShowInstalled()
        {
            List<string> guidsToVerify = new List<string>();
            guidsToVerify.Add("{04FE9017-F873-410E-871E-AB91661A4EF7}"); // ffdshow Video Decoder
            guidsToVerify.Add("{0F40E1E5-4F79-4988-B1A9-CC98794E6B55}"); //	ffdshow Audio Decoder

            CLSIDChecker checker = new CLSIDChecker();
            return checker.IsInstalled(guidsToVerify);
        }

        //*******************************************************************
        private void CheckForOpenWithFile(object o, System.Windows.Forms.PaintEventArgs e)
        {
            try
            {
                this.Paint -= new PaintEventHandler(this.CheckForOpenWithFile);
                if (this.mOpenWithFile != "")
                {
                    this.OpenFormFile(mOpenWithFile);
                    Form1.mMainForm.GoToMainMenu();
                    Form1.mMainForm.UpdateOuterTabControlTabs();
                    Form1.mMainForm.GetDecorationManager().RePaint();
                }
            }
            catch
            {
            }
        }

        //*******************************************************************
        private void MoveWindow(object o, System.EventArgs e)
        {
            mAddSlidesBalloon.Hide();
        }

        //*******************************************************************
        public delegate void ReCalcTitleBarStringDelegate();
        public void ReCalcTitleBarString()
        {
            if (this.InvokeRequired == true)
            {
                BeginInvoke(new ReCalcTitleBarStringDelegate(ReCalcTitleBarString));
                return;
            }

            string s1 = "";
            if (CGlobals.mCurrentProject.mChangedSinceSave == true)
            {
                s1 = "*";
            }

            this.Text = CGlobals.mCurrentProject.GetNameWithoutPath() + s1 + " - " + FrontEndGlobals.mApplicationName;
        }


        //*******************************************************************

        public void OnKeyDown2(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Control)
            {
                mControlKeyOn = true;
            }
            if (e.Shift)
            {
                mShiftKeyOn = true;
            }
        }


        //*******************************************************************
        public void OnKeyUp2(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            /*
            if (e.KeyCode == System.Windows.Forms.Keys.ControlKey)
            {
                mControlKeyOn= false;
            }
            if (e.KeyCode == System.Windows.Forms.Keys.ShiftKey)
            {
                mShiftKeyOn= false;
            }
            */

            mControlKeyOn = false;
            mShiftKeyOn = false;

            if (e.Control)
            {
                mControlKeyOn = true;
            }
            if (e.Shift)
            {
                mShiftKeyOn = true;
            }
        }

        //*******************************************************************
        private delegate void BringMainWindowToFocusDelegate();
        private void BringMainWindowToFocus()
        {
            if (this.InvokeRequired == true)
            {
                BeginInvoke(new BringMainWindowToFocusDelegate(BringMainWindowToFocus));
                return;
            }

            this.TopMost = true;
            this.BringToFront();
            this.Focus();
            this.TopMost = false;
        }


        //*******************************************************************
        private delegate void AddStartBalloonHelpDelegate();
        private void AddStartBalloonHelp()
        {
            if (this.InvokeRequired == true)
            {
                BeginInvoke(new AddStartBalloonHelpDelegate(AddStartBalloonHelp));
                return;
            }
            Point point = Form1.mMainForm.PointToScreen(new Point(20, 415));
            mAddSlidesBalloon.Show(point);
        }


        //*******************************************************************
        public void ShowLogoLonger()
        {
            System.Threading.Thread.Sleep(1000);
            SplashForm.CloseSplash();

            BringMainWindowToFocus();

            //	this.Focus();

            if (mOpenWithFile == "")
            {

                try
                {


                    /*
                    // first time we've run app.  auto check for update.
                    if ((
                        ManagedCore.License.License.Valid == false ||
                        mHadLicenceBeforeAppStarted == false) &&
                        System.IO.File.Exists(CGlobals.GetUserDirectory() + "\\rn.txt") == false)
                    {

                        DialogResult res = UserMessage.Show("As this is the first time you have run PhotoVidShow, do you wish to\n\rcheck you are running the latest version? (Recommended)\n\r", "Check for updates",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (res == DialogResult.Yes)
                        {
                            CheckForUpdatesMenuItem_Click(this, new System.EventArgs());
                        }
                    }

                    System.IO.File.Create(CGlobals.GetUserDirectory() + "\\rn.txt");
                     */

                    System.Threading.Thread.Sleep(200);

                    while (mShowingStartupOptions == true)
                    {
                        System.Threading.Thread.Sleep(200);
                    }

                    AddStartBalloonHelp();
                }
                catch
                {
                }

            }

            //	this.+=new PaintEventHandler(this.AddSlidesButtonVisible);
        }

        public void AddSlidesButtonVisible(Object o, System.Windows.Forms.PaintEventArgs e)
        {
            //this.mAddSlidesBalloon.Hide();
            //this.Paint-=new PaintEventHandler(this.AddSlidesButtonVisible);
        }


        //*******************************************************************
        public bool WaitForRegistration()
        {
            // if already registered kill splash screen

            if (ManagedCore.License.License.Valid == true)
            {
                mHadLicenceBeforeAppStarted = true;
                mPurchaseToolStripMenuItem.Visible = false;

                System.Threading.ThreadStart ts = new System.Threading.ThreadStart(ShowLogoLonger);
                System.Threading.Thread new_thread = new Thread(ts);
                new_thread.Start();
            }
            else if (CGlobals.RunningSteamDemo == true || CGlobals.RunningWindowsStoreTrial)
            {
                // 
                // If running stream demo or Windows Store trial, also kill splash screen
                // 
                mPurchaseToolStripMenuItem.Visible = false;
                System.Threading.ThreadStart ts = new System.Threading.ThreadStart(ShowLogoLonger);
                System.Threading.Thread new_thread = new Thread(ts);
                new_thread.Start();
            }
            else
            {
                // else wait for user to perform actions there

                while (SplashForm.mDone == false)
                {
                    System.Threading.Thread.Sleep(100);
                }

                if (SplashForm.mClosedApp == true)
                {
                    return false;
                }

                System.Threading.ThreadStart ts2 = new System.Threading.ThreadStart(ShowLogoLonger);
                System.Threading.Thread new_thread2 = new Thread(ts2);
                new_thread2.Start();
            }

            return true; 
        }


        public Panel GetSlideShowPanel()
        {
#if (!DESIGN_MODE)
            return scrollslideshowpanel.GetPanel();
#else
            return scrollslideshowpanel;
#endif
        }

        public Panel GetSlideShowMusicPlanel()
        {
            return GetSlideShowPanel();
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.mHelpToolStrip = new System.Windows.Forms.ToolStrip();
            this.mHelpButton = new System.Windows.Forms.ToolStripButton();
            this.mTopToolStripBar = new System.Windows.Forms.ToolStrip();
            this.mNewToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.mOpenToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.mSaveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mUndoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.mRedoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.mDVDMenuToolStripButton = new System.Windows.Forms.ToolStripSplitButton();
            this.mIntroSlideshowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mSelectSlideshowToolStripItemCombo = new System.Windows.Forms.ToolStripComboBox();
            this.mDVDMenuToolStripSeperator = new System.Windows.Forms.ToolStripSeparator();
            this.mOutputToolStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.mOutputDiskTypeDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.videoForCompuerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.videoForComputerOrWebToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blurayVideoDiskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mOutputAspectDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.wdwdwdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mCreateAndBurnButton = new System.Windows.Forms.ToolStripButton();
            this.mCreateVideoButton = new System.Windows.Forms.ToolStripButton();
            this.LogoPictureBox = new System.Windows.Forms.PictureBox();
            this.InnerDiskEstimationPanel = new System.Windows.Forms.Panel();
            this.DiskEstimationInnerPictureBox = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.slideshowpanel = new System.Windows.Forms.Panel();
            this.scrollslideshowpanel = new CustomButton.ManualScrollablePanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.mRewindToStartSlideshowButton = new BitmapButton.SimpleBitmapButton();
            this.TotalSlidesSlideshowTextBox = new System.Windows.Forms.Label();
            this.mStoryboardToolStrip = new System.Windows.Forms.ToolStrip();
            this.SlideshowAddSlidesButton = new System.Windows.Forms.ToolStripSplitButton();
            this.AddBlankSlideMenuItemButton = new System.Windows.Forms.ToolStripMenuItem();
            this.mCreateSlideFromTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openExplorerWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SlideshowAddMusicButton = new System.Windows.Forms.ToolStripSplitButton();
            this.mAddOrEditMusicSlidesToolStripMenuItemMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mDoNarrationButton = new System.Windows.Forms.ToolStripSplitButton();
            this.mAddNarrationFromAudioFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mStoryboardEditItemButton = new System.Windows.Forms.ToolStripButton();
            this.RotateSlideAntiClockwiseButton = new System.Windows.Forms.ToolStripButton();
            this.RotateSlideClockWiseButton = new System.Windows.Forms.ToolStripButton();
            this.mSetMenuthumbnailButton = new System.Windows.Forms.ToolStripButton();
            this.mDeleteButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.SlideshowAdvanceOptionsButton = new System.Windows.Forms.ToolStripButton();
            this.mStopSlideshowButton = new BitmapButton.SimpleBitmapButton();
            this.SlideShowTimeLabel = new System.Windows.Forms.Label();
            this.mStoryboardTrackBar = new System.Windows.Forms.TrackBar();
            this.mPlaySlideshowButton = new BitmapButton.SimpleBitmapButton();
            this.panel5 = new System.Windows.Forms.Panel();
            this.mOuterTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.mMainMenuTabControl = new CustomTabControl.TabControlEx();
            this.mMenuStructureTabPage = new System.Windows.Forms.TabPage();
            this.mImportSlideshowButton = new BitmapButton.SimpleBitmapButton();
            this.label3 = new System.Windows.Forms.Label();
            this.mMenuIndexLabel = new System.Windows.Forms.Label();
            this.mTotalMenuStatsLabel = new System.Windows.Forms.Label();
            this.RemoveSlideshowButton = new System.Windows.Forms.Button();
            this.NewSlideShow = new BitmapButton.SimpleBitmapButton();
            this.AddNextMenuButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.mMenuThemeTabPage = new System.Windows.Forms.TabPage();
            this.mMenuThemeTab = new System.Windows.Forms.TabControl();
            this.mMenuBackgroundTab = new System.Windows.Forms.TabPage();
            this.MenuTemplatesPanel = new System.Windows.Forms.Panel();
            this.mMenuLayoutTab = new System.Windows.Forms.TabPage();
            this.MenuLayoutsPanel = new System.Windows.Forms.Panel();
            this.mMenuFrameStyleTab = new System.Windows.Forms.TabPage();
            this.MenuStylePanel = new System.Windows.Forms.Panel();
            this.mMenuNavigationStyleTab = new System.Windows.Forms.TabPage();
            this.mMenuNavigationButtonsPanel = new System.Windows.Forms.Panel();
            this.mMenuNavigationButtonsLabel = new System.Windows.Forms.Label();
            this.mMenuNavigationPlayAllLoopedButton = new System.Windows.Forms.CheckBox();
            this.mMenuNavigationPlayAllButton = new System.Windows.Forms.CheckBox();
            this.mMenuHighlightStlyeTab = new System.Windows.Forms.TabPage();
            this.mMenuHighlightResetButton = new System.Windows.Forms.Button();
            this.mMenuHighlightColorLabel = new System.Windows.Forms.Label();
            this.mMenuHighlightStyleLabel = new System.Windows.Forms.Label();
            this.mMenuHighlightPlayButtonsLabel = new System.Windows.Forms.Label();
            this.mMenuHighlightNavigationButtonLabel = new System.Windows.Forms.Label();
            this.mMenuHighlightSlideshowButtonsLabel = new System.Windows.Forms.Label();
            this.mMenuHighlightColorButton = new System.Windows.Forms.Button();
            this.mMenuHighlightPlayButtonStyleCombo = new System.Windows.Forms.ComboBox();
            this.mMenuHighlightNavigationButtonStyleCombo = new System.Windows.Forms.ComboBox();
            this.mMenuHighlightSlideshowButtonStyleCombo = new System.Windows.Forms.ComboBox();
            this.AddBackgroundMenuMusicButton = new System.Windows.Forms.Button();
            this.mPreviewMenuButton = new System.Windows.Forms.Button();
            this.MenuAddDecorateButton = new System.Windows.Forms.Button();
            this.mZoomingMenuBackGroundCheckBox = new System.Windows.Forms.CheckBox();
            this.mMenuThemePlayingThumbnailsCheckBox = new System.Windows.Forms.CheckBox();
            this.blankout = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.MenuRepeatDurationTextbox = new System.Windows.Forms.Label();
            this.MenuRepeatLengthTrackbar = new System.Windows.Forms.TrackBar();
            this.MenuThemeLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.SetMenuBackgroundButton = new System.Windows.Forms.Button();
            this.MenuBackgroundName = new System.Windows.Forms.TextBox();
            this.MenuBackgroundMusicName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mEmptyMenuTabPage = new System.Windows.Forms.TabPage();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.mSlideOptionsTab = new CustomTabControl.TabControlEx();
            this.mPreDesignedSlidesTabPage = new System.Windows.Forms.TabPage();
            this.mPredefinedSlideDesignsControl1 = new CustomButton.PredefinedSlideDesignsControl();
            this.mBackgroundTabPage = new System.Windows.Forms.TabPage();
            this.mSelectBackgroundControl1 = new CustomButton.SelectBackgroundControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.mSelectedDecorationCombo = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.mDecorationPositionHeightTextBox = new System.Windows.Forms.NumericUpDown();
            this.mDecorationPositionLeftTextBox = new System.Windows.Forms.NumericUpDown();
            this.mDecorationPositionWidthTextBox = new System.Windows.Forms.NumericUpDown();
            this.mDoneDecorationMenuButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.mClipartTransparencyNumerical = new System.Windows.Forms.NumericUpDown();
            this.mDecorationPositionTopTextBox = new System.Windows.Forms.NumericUpDown();
            this.mDecorationRotationNumericArrows = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.mDecorationStartEndSlideTimeControl = new CustomButton.SetStartEndSlideTime();
            this.ClipartTransparencyTrackerBar = new System.Windows.Forms.TrackBar();
            this.mForceEnlargeLabel = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.mDecorationRotationLabel = new System.Windows.Forms.Label();
            this.OrderLayersButton = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.mEffectsEditorButton = new System.Windows.Forms.Button();
            this.ClipartTransparencyLabel = new System.Windows.Forms.Label();
            this.DecorationTabOptions = new System.Windows.Forms.TabControl();
            this.AddTextTab = new System.Windows.Forms.TabPage();
            this.mFontPreviewPictureBox = new System.Windows.Forms.PictureBox();
            this.ColourPickerBackPlane = new System.Windows.Forms.Button();
            this.BackPlaneCheckbox = new System.Windows.Forms.CheckBox();
            this.mTextTemplateTypeLabel = new System.Windows.Forms.Label();
            this.BackPlaneTransparencyTrackBar = new System.Windows.Forms.TrackBar();
            this.mTextTemplateTypeCombo = new System.Windows.Forms.ComboBox();
            this.BackPlaneTransparencyLabel = new System.Windows.Forms.Label();
            this.mFontButton = new System.Windows.Forms.Button();
            this.mEditTextButton = new System.Windows.Forms.Button();
            this.RightAlignedRadioBox = new System.Windows.Forms.RadioButton();
            this.SizeComboBox = new System.Windows.Forms.ComboBox();
            this.AddTextButton = new System.Windows.Forms.Button();
            this.CentreAlignedRadioBox = new System.Windows.Forms.RadioButton();
            this.LeftAlignedRadioBox = new System.Windows.Forms.RadioButton();
            this.ClipArtTab = new System.Windows.Forms.TabPage();
            this.mTemplateFrameworkImageCheckbox = new System.Windows.Forms.CheckBox();
            this.mTemplateImageNumberCombo = new System.Windows.Forms.ComboBox();
            this.mTemplateAspectCombo = new System.Windows.Forms.ComboBox();
            this.AddClipArtButton = new System.Windows.Forms.Button();
            this.EditImageButton = new System.Windows.Forms.Button();
            this.mRenderMethodCombo = new System.Windows.Forms.ComboBox();
            this.mClipArtLockAspectCheckBox = new System.Windows.Forms.CheckBox();
            this.AddNewImageButton = new System.Windows.Forms.Button();
            this.RotateDecorationClockWiseButton = new System.Windows.Forms.Button();
            this.RotateDecorationAntiClockWiseButton = new System.Windows.Forms.Button();
            this.FlipDecorationLeftRightButton = new System.Windows.Forms.Button();
            this.FlipDecorationUpDownButton = new System.Windows.Forms.Button();
            this.mDecorationRotationTrackbar = new System.Windows.Forms.TrackBar();
            this.Transparentlabel = new System.Windows.Forms.Label();
            this.ClipPanTabPage = new System.Windows.Forms.TabPage();
            this.label17 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.mPanZoomUseEquationComboBox = new System.Windows.Forms.ComboBox();
            this.mPanZoomInitialDelayTrackBar = new System.Windows.Forms.TrackBar();
            this.mReCalcPanZoomTemplateCheckbox = new System.Windows.Forms.CheckBox();
            this.mPanZoomInitialDelayLabel = new System.Windows.Forms.Label();
            this.EndRotationTrackBarLabel = new System.Windows.Forms.Label();
            this.StartRotationTrackBarLabel = new System.Windows.Forms.Label();
            this.StartPanTrackBarLabel = new System.Windows.Forms.Label();
            this.EndPanTrackBarLabel = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.mPreviewPanZoomPictureBox = new System.Windows.Forms.PictureBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.EndPanRotationTrackBar = new System.Windows.Forms.TrackBar();
            this.EndPanTrackBar = new System.Windows.Forms.TrackBar();
            this.panel7 = new System.Windows.Forms.Panel();
            this.StartPanRotationTrackBar = new System.Windows.Forms.TrackBar();
            this.mPanZoomOnAllCheckBox = new System.Windows.Forms.CheckBox();
            this.mStartPanDelayLabel = new System.Windows.Forms.Label();
            this.mPanZoomLinkStartAreaButton = new System.Windows.Forms.Button();
            this.StartPanTrackBar = new System.Windows.Forms.TrackBar();
            this.label8 = new System.Windows.Forms.Label();
            this.mEndPanDelayLabel = new System.Windows.Forms.Label();
            this.mForceEnlargeLabel2 = new System.Windows.Forms.Label();
            this.mPanZoomEndDelayLabel = new System.Windows.Forms.Label();
            this.mPanZoomEndDelayTrackBar = new System.Windows.Forms.TrackBar();
            this.mTurnOffPanZoomForSlideTickBox = new System.Windows.Forms.CheckBox();
            this.RandomPanZoomButton = new System.Windows.Forms.Button();
            this.mFiltersTabPage = new System.Windows.Forms.TabPage();
            this.mFiltersControl = new CustomButton.SlideFiltersControl();
            this.mBordersTab = new System.Windows.Forms.TabPage();
            this.mOverlaySelectionControl1 = new CustomButton.OverlaySelectionControl();
            this.DecoratePanZoomImageList = new System.Windows.Forms.ImageList(this.components);
            this.mIntroSlideshowTabPage = new System.Windows.Forms.TabPage();
            this.mIntoSlideshowTabControl = new CustomTabControl.TabControlEx();
            this.mIntroSlideshowInnerTabPage = new System.Windows.Forms.TabPage();
            this.label23 = new System.Windows.Forms.Label();
            this.mDiskIntroSlideshowImportSlideshowButton = new System.Windows.Forms.Button();
            this.mIncludeIntroVideoCheckBox = new System.Windows.Forms.CheckBox();
            this.mInclideIntroSlideshowLabel = new System.Windows.Forms.Label();
            this.mIntroVideoTabPagesImageList = new System.Windows.Forms.ImageList(this.components);
            this.panel4 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.SlideShowTimer = new System.Windows.Forms.Timer(this.components);
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.mainMenu2 = new System.Windows.Forms.MainMenu(this.components);
            this.LoadProjectDialog = new System.Windows.Forms.OpenFileDialog();
            this.SaveProjectDialog = new System.Windows.Forms.SaveFileDialog();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.colorDialog2 = new System.Windows.Forms.ColorDialog();
            this.ResetMenuMusicContextMenu = new System.Windows.Forms.ContextMenu();
            this.menuItem17 = new System.Windows.Forms.MenuItem();
            this.MenuItemsImageList = new System.Windows.Forms.ImageList(this.components);
            this.mOutputImageList = new System.Windows.Forms.ImageList(this.components);
            this.mTopMenuStrip = new System.Windows.Forms.MenuStrip();
            this.mFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mNewProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mOpenProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.mImportSlideshowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mImportTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mExportTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.mSaveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mSaveProjectAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.mExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mDisplayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mShowSelectedSlideshowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setDefaultProjectTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.videoFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dVDVideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blurayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setMaxVideoLoadedAtOnceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mSetDefaultFolderLocationsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mLaunchDVDBurnerToolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tutorialsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mVisitTutorialsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mHelpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mCheckForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.mPurchaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mVisitPhotoVidShowWebsiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mAboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mDebugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mShowMementoStackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mShowImagesCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mShowTextBoundaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showVideoCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mVideoImportToolStipItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mUseFFDShowForMovVideos = new System.Windows.Forms.ToolStripMenuItem();
            this.mUseFFDShowForMp4Videos = new System.Windows.Forms.ToolStripMenuItem();
            this.mUseFFDShowForMtsVideos = new System.Windows.Forms.ToolStripMenuItem();
            this.mDecorationsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mBringToFrontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mSendToBackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mDeleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mGlobalMenuItemIcons = new System.Windows.Forms.ImageList(this.components);
            this.mDiskMenuImageList = new System.Windows.Forms.ImageList(this.components);
            this.mMainToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panel1.SuspendLayout();
            this.mHelpToolStrip.SuspendLayout();
            this.mTopToolStripBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).BeginInit();
            this.InnerDiskEstimationPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DiskEstimationInnerPictureBox)).BeginInit();
            this.slideshowpanel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.mStoryboardToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mStoryboardTrackBar)).BeginInit();
            this.panel5.SuspendLayout();
            this.mOuterTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.mMainMenuTabControl.SuspendLayout();
            this.mMenuStructureTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.mMenuThemeTabPage.SuspendLayout();
            this.mMenuThemeTab.SuspendLayout();
            this.mMenuBackgroundTab.SuspendLayout();
            this.mMenuLayoutTab.SuspendLayout();
            this.mMenuFrameStyleTab.SuspendLayout();
            this.mMenuNavigationStyleTab.SuspendLayout();
            this.mMenuHighlightStlyeTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MenuRepeatLengthTrackbar)).BeginInit();
            this.mEmptyMenuTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.mSlideOptionsTab.SuspendLayout();
            this.mPreDesignedSlidesTabPage.SuspendLayout();
            this.mBackgroundTabPage.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mDecorationPositionHeightTextBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mDecorationPositionLeftTextBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mDecorationPositionWidthTextBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mClipartTransparencyNumerical)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mDecorationPositionTopTextBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mDecorationRotationNumericArrows)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ClipartTransparencyTrackerBar)).BeginInit();
            this.DecorationTabOptions.SuspendLayout();
            this.AddTextTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mFontPreviewPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BackPlaneTransparencyTrackBar)).BeginInit();
            this.ClipArtTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mDecorationRotationTrackbar)).BeginInit();
            this.ClipPanTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mPanZoomInitialDelayTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mPreviewPanZoomPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EndPanRotationTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EndPanTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StartPanRotationTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StartPanTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mPanZoomEndDelayTrackBar)).BeginInit();
            this.mFiltersTabPage.SuspendLayout();
            this.mBordersTab.SuspendLayout();
            this.mIntroSlideshowTabPage.SuspendLayout();
            this.mIntoSlideshowTabControl.SuspendLayout();
            this.mIntroSlideshowInnerTabPage.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.mTopMenuStrip.SuspendLayout();
            this.mDecorationsContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(212)))), ((int)(((byte)(234)))));
            this.panel1.Controls.Add(this.mHelpToolStrip);
            this.panel1.Controls.Add(this.mTopToolStripBar);
            this.panel1.Controls.Add(this.LogoPictureBox);
            this.panel1.Controls.Add(this.InnerDiskEstimationPanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1194, 47);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // mHelpToolStrip
            // 
            this.mHelpToolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mHelpToolStrip.AutoSize = false;
            this.mHelpToolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(212)))), ((int)(((byte)(234)))));
            this.mHelpToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.mHelpToolStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.mHelpToolStrip.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.mHelpToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mHelpButton});
            this.mHelpToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.mHelpToolStrip.Location = new System.Drawing.Point(1130, 0);
            this.mHelpToolStrip.Name = "mHelpToolStrip";
            this.mHelpToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.mHelpToolStrip.Size = new System.Drawing.Size(41, 49);
            this.mHelpToolStrip.TabIndex = 56;
            this.mHelpToolStrip.Text = "toolStrip1";
            // 
            // mHelpButton
            // 
            this.mHelpButton.AutoSize = false;
            this.mHelpButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mHelpButton.Image = ((System.Drawing.Image)(resources.GetObject("mHelpButton.Image")));
            this.mHelpButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mHelpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mHelpButton.Margin = new System.Windows.Forms.Padding(0);
            this.mHelpButton.Name = "mHelpButton";
            this.mHelpButton.Size = new System.Drawing.Size(36, 46);
            this.mHelpButton.Text = "Help";
            this.mHelpButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.mHelpButton.ToolTipText = "Open the user manual in a window";
            this.mHelpButton.Click += new System.EventHandler(this.HelpButton_Click);
            // 
            // mTopToolStripBar
            // 
            this.mTopToolStripBar.AutoSize = false;
            this.mTopToolStripBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(212)))), ((int)(((byte)(234)))));
            this.mTopToolStripBar.Dock = System.Windows.Forms.DockStyle.None;
            this.mTopToolStripBar.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTopToolStripBar.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.mTopToolStripBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mNewToolStripButton,
            this.mOpenToolStripButton,
            this.mSaveToolStripButton,
            this.toolStripSeparator4,
            this.mUndoToolStripButton,
            this.mRedoToolStripButton,
            this.toolStripSeparator5,
            this.mDVDMenuToolStripButton,
            this.mSelectSlideshowToolStripItemCombo,
            this.mDVDMenuToolStripSeperator,
            this.mOutputToolStripLabel,
            this.mOutputDiskTypeDropDownButton,
            this.mOutputAspectDropDownButton,
            this.toolStripSeparator3,
            this.mCreateAndBurnButton,
            this.mCreateVideoButton});
            this.mTopToolStripBar.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.mTopToolStripBar.Location = new System.Drawing.Point(0, 0);
            this.mTopToolStripBar.Name = "mTopToolStripBar";
            this.mTopToolStripBar.Padding = new System.Windows.Forms.Padding(0);
            this.mTopToolStripBar.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.mTopToolStripBar.Size = new System.Drawing.Size(859, 49);
            this.mTopToolStripBar.TabIndex = 55;
            this.mTopToolStripBar.Text = "toolStrip1";
            this.mTopToolStripBar.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.mTopToolStripBar_ItemClicked);
            // 
            // mNewToolStripButton
            // 
            this.mNewToolStripButton.AutoSize = false;
            this.mNewToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mNewToolStripButton.Image")));
            this.mNewToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mNewToolStripButton.Margin = new System.Windows.Forms.Padding(0);
            this.mNewToolStripButton.Name = "mNewToolStripButton";
            this.mNewToolStripButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.mNewToolStripButton.Size = new System.Drawing.Size(34, 46);
            this.mNewToolStripButton.Text = "New";
            this.mNewToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.mNewToolStripButton.ToolTipText = "New project";
            this.mNewToolStripButton.Click += new System.EventHandler(this.NewSlideshowButton_Click);
            // 
            // mOpenToolStripButton
            // 
            this.mOpenToolStripButton.AutoSize = false;
            this.mOpenToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mOpenToolStripButton.Image")));
            this.mOpenToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mOpenToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mOpenToolStripButton.Margin = new System.Windows.Forms.Padding(0);
            this.mOpenToolStripButton.Name = "mOpenToolStripButton";
            this.mOpenToolStripButton.Size = new System.Drawing.Size(38, 46);
            this.mOpenToolStripButton.Text = "Open";
            this.mOpenToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.mOpenToolStripButton.ToolTipText = "Open a project";
            this.mOpenToolStripButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // mSaveToolStripButton
            // 
            this.mSaveToolStripButton.AutoSize = false;
            this.mSaveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mSaveToolStripButton.Image")));
            this.mSaveToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mSaveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mSaveToolStripButton.Margin = new System.Windows.Forms.Padding(0);
            this.mSaveToolStripButton.Name = "mSaveToolStripButton";
            this.mSaveToolStripButton.Size = new System.Drawing.Size(36, 46);
            this.mSaveToolStripButton.Text = "Save";
            this.mSaveToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.mSaveToolStripButton.ToolTipText = "Save project";
            this.mSaveToolStripButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.AutoSize = false;
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 44);
            // 
            // mUndoToolStripButton
            // 
            this.mUndoToolStripButton.AutoSize = false;
            this.mUndoToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mUndoToolStripButton.Image")));
            this.mUndoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mUndoToolStripButton.Margin = new System.Windows.Forms.Padding(0);
            this.mUndoToolStripButton.Name = "mUndoToolStripButton";
            this.mUndoToolStripButton.Size = new System.Drawing.Size(38, 46);
            this.mUndoToolStripButton.Text = "Undo";
            this.mUndoToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.mUndoToolStripButton.Click += new System.EventHandler(this.UndoButton_Click);
            // 
            // mRedoToolStripButton
            // 
            this.mRedoToolStripButton.AutoSize = false;
            this.mRedoToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mRedoToolStripButton.Image")));
            this.mRedoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mRedoToolStripButton.Margin = new System.Windows.Forms.Padding(0);
            this.mRedoToolStripButton.Name = "mRedoToolStripButton";
            this.mRedoToolStripButton.Size = new System.Drawing.Size(38, 46);
            this.mRedoToolStripButton.Text = "Redo";
            this.mRedoToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.mRedoToolStripButton.Click += new System.EventHandler(this.RedoButton_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.AutoSize = false;
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 44);
            // 
            // mDVDMenuToolStripButton
            // 
            this.mDVDMenuToolStripButton.AutoSize = false;
            this.mDVDMenuToolStripButton.DropDownButtonWidth = 14;
            this.mDVDMenuToolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mIntroSlideshowToolStripMenuItem});
            this.mDVDMenuToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("mDVDMenuToolStripButton.Image")));
            this.mDVDMenuToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mDVDMenuToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mDVDMenuToolStripButton.Margin = new System.Windows.Forms.Padding(0);
            this.mDVDMenuToolStripButton.Name = "mDVDMenuToolStripButton";
            this.mDVDMenuToolStripButton.Size = new System.Drawing.Size(82, 46);
            this.mDVDMenuToolStripButton.Text = "Disk menu";
            this.mDVDMenuToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.mDVDMenuToolStripButton.ToolTipText = "Go to the disk main menu";
            this.mDVDMenuToolStripButton.ButtonClick += new System.EventHandler(this.mDVDMenuToolStripButton_Click);
            // 
            // mIntroSlideshowToolStripMenuItem
            // 
            this.mIntroSlideshowToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mIntroSlideshowToolStripMenuItem.Image")));
            this.mIntroSlideshowToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mIntroSlideshowToolStripMenuItem.Name = "mIntroSlideshowToolStripMenuItem";
            this.mIntroSlideshowToolStripMenuItem.Size = new System.Drawing.Size(137, 36);
            this.mIntroSlideshowToolStripMenuItem.Text = "Disk intro";
            this.mIntroSlideshowToolStripMenuItem.ToolTipText = "Create or edit the disk intro slideshow";
            this.mIntroSlideshowToolStripMenuItem.Click += new System.EventHandler(this.mIntroSlideshowToolStripMenuItem_Click);
            // 
            // mSelectSlideshowToolStripItemCombo
            // 
            this.mSelectSlideshowToolStripItemCombo.AutoSize = false;
            this.mSelectSlideshowToolStripItemCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mSelectSlideshowToolStripItemCombo.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.mSelectSlideshowToolStripItemCombo.Items.AddRange(new object[] {
            "My slideshow 1"});
            this.mSelectSlideshowToolStripItemCombo.Margin = new System.Windows.Forms.Padding(4, 10, 1, 0);
            this.mSelectSlideshowToolStripItemCombo.Name = "mSelectSlideshowToolStripItemCombo";
            this.mSelectSlideshowToolStripItemCombo.Size = new System.Drawing.Size(120, 23);
            this.mSelectSlideshowToolStripItemCombo.ToolTipText = "Select the slideshow to edit";
            // 
            // mDVDMenuToolStripSeperator
            // 
            this.mDVDMenuToolStripSeperator.AutoSize = false;
            this.mDVDMenuToolStripSeperator.Name = "mDVDMenuToolStripSeperator";
            this.mDVDMenuToolStripSeperator.Size = new System.Drawing.Size(6, 44);
            // 
            // mOutputToolStripLabel
            // 
            this.mOutputToolStripLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mOutputToolStripLabel.Margin = new System.Windows.Forms.Padding(0, 15, 0, 2);
            this.mOutputToolStripLabel.Name = "mOutputToolStripLabel";
            this.mOutputToolStripLabel.Size = new System.Drawing.Size(45, 13);
            this.mOutputToolStripLabel.Text = "Output";
            // 
            // mOutputDiskTypeDropDownButton
            // 
            this.mOutputDiskTypeDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.videoForCompuerToolStripMenuItem,
            this.videoForComputerOrWebToolStripMenuItem,
            this.blurayVideoDiskToolStripMenuItem});
            this.mOutputDiskTypeDropDownButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mOutputDiskTypeDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("mOutputDiskTypeDropDownButton.Image")));
            this.mOutputDiskTypeDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mOutputDiskTypeDropDownButton.Name = "mOutputDiskTypeDropDownButton";
            this.mOutputDiskTypeDropDownButton.Size = new System.Drawing.Size(117, 44);
            this.mOutputDiskTypeDropDownButton.Text = "DVD-Video";
            // 
            // videoForCompuerToolStripMenuItem
            // 
            this.videoForCompuerToolStripMenuItem.Image = global::dvdslideshowfontend.Properties.Resources.video2;
            this.videoForCompuerToolStripMenuItem.Name = "videoForCompuerToolStripMenuItem";
            this.videoForCompuerToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.videoForCompuerToolStripMenuItem.Text = "Video file for computer or web";
            this.videoForCompuerToolStripMenuItem.Click += new System.EventHandler(this.videoForCompuerToolStripMenuItem_Click);
            // 
            // videoForComputerOrWebToolStripMenuItem
            // 
            this.videoForComputerOrWebToolStripMenuItem.Image = global::dvdslideshowfontend.Properties.Resources.dvd_video_small2;
            this.videoForComputerOrWebToolStripMenuItem.Name = "videoForComputerOrWebToolStripMenuItem";
            this.videoForComputerOrWebToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.videoForComputerOrWebToolStripMenuItem.Text = "DVD-Video disk";
            // 
            // blurayVideoDiskToolStripMenuItem
            // 
            this.blurayVideoDiskToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("blurayVideoDiskToolStripMenuItem.Image")));
            this.blurayVideoDiskToolStripMenuItem.Name = "blurayVideoDiskToolStripMenuItem";
            this.blurayVideoDiskToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.blurayVideoDiskToolStripMenuItem.Text = "Blu-ray video disk";
            // 
            // mOutputAspectDropDownButton
            // 
            this.mOutputAspectDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mOutputAspectDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.wdwdwdToolStripMenuItem});
            this.mOutputAspectDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("mOutputAspectDropDownButton.Image")));
            this.mOutputAspectDropDownButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mOutputAspectDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mOutputAspectDropDownButton.Margin = new System.Windows.Forms.Padding(5, 3, 0, 2);
            this.mOutputAspectDropDownButton.Name = "mOutputAspectDropDownButton";
            this.mOutputAspectDropDownButton.Size = new System.Drawing.Size(84, 37);
            this.mOutputAspectDropDownButton.ToolTipText = "Select output screen aspect";
            this.mOutputAspectDropDownButton.Click += new System.EventHandler(this.toolStripDropDownButton1_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripMenuItem2.Image = global::dvdslideshowfontend.Properties.Resources.widescreen;
            this.toolStripMenuItem2.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripMenuItem2.Size = new System.Drawing.Size(211, 40);
            this.toolStripMenuItem2.Text = "16:9 widescreen";
            // 
            // wdwdwdToolStripMenuItem
            // 
            this.wdwdwdToolStripMenuItem.Image = global::dvdslideshowfontend.Properties.Resources.standard1;
            this.wdwdwdToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.wdwdwdToolStripMenuItem.Name = "wdwdwdToolStripMenuItem";
            this.wdwdwdToolStripMenuItem.Size = new System.Drawing.Size(211, 40);
            this.wdwdwdToolStripMenuItem.Text = "4:3 standard";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.AutoSize = false;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 44);
            // 
            // mCreateAndBurnButton
            // 
            this.mCreateAndBurnButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mCreateAndBurnButton.Image = ((System.Drawing.Image)(resources.GetObject("mCreateAndBurnButton.Image")));
            this.mCreateAndBurnButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mCreateAndBurnButton.Margin = new System.Windows.Forms.Padding(1, 1, 0, 2);
            this.mCreateAndBurnButton.Name = "mCreateAndBurnButton";
            this.mCreateAndBurnButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.mCreateAndBurnButton.Size = new System.Drawing.Size(173, 44);
            this.mCreateAndBurnButton.Text = "Create and burn to disk";
            this.mCreateAndBurnButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.mCreateAndBurnButton.ToolTipText = "Create and/or burn to a blank disk";
            this.mCreateAndBurnButton.Click += new System.EventHandler(this.AuthorButton_Click);
            // 
            // mCreateVideoButton
            // 
            this.mCreateVideoButton.Image = ((System.Drawing.Image)(resources.GetObject("mCreateVideoButton.Image")));
            this.mCreateVideoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mCreateVideoButton.Name = "mCreateVideoButton";
            this.mCreateVideoButton.Size = new System.Drawing.Size(132, 44);
            this.mCreateVideoButton.Text = "Create my video";
            this.mCreateVideoButton.Visible = false;
            this.mCreateVideoButton.Click += new System.EventHandler(this.AuthorButton_Click);
            // 
            // LogoPictureBox
            // 
            this.LogoPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LogoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("LogoPictureBox.Image")));
            this.LogoPictureBox.Location = new System.Drawing.Point(979, -1);
            this.LogoPictureBox.Name = "LogoPictureBox";
            this.LogoPictureBox.Size = new System.Drawing.Size(154, 46);
            this.LogoPictureBox.TabIndex = 18;
            this.LogoPictureBox.TabStop = false;
            // 
            // InnerDiskEstimationPanel
            // 
            this.InnerDiskEstimationPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.InnerDiskEstimationPanel.Controls.Add(this.DiskEstimationInnerPictureBox);
            this.InnerDiskEstimationPanel.Location = new System.Drawing.Point(885, 3);
            this.InnerDiskEstimationPanel.Name = "InnerDiskEstimationPanel";
            this.InnerDiskEstimationPanel.Size = new System.Drawing.Size(54, 26);
            this.InnerDiskEstimationPanel.TabIndex = 3;
            // 
            // DiskEstimationInnerPictureBox
            // 
            this.DiskEstimationInnerPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DiskEstimationInnerPictureBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(212)))), ((int)(((byte)(234)))));
            this.DiskEstimationInnerPictureBox.Location = new System.Drawing.Point(21, 19);
            this.DiskEstimationInnerPictureBox.Name = "DiskEstimationInnerPictureBox";
            this.DiskEstimationInnerPictureBox.Size = new System.Drawing.Size(33, 4);
            this.DiskEstimationInnerPictureBox.TabIndex = 0;
            this.DiskEstimationInnerPictureBox.TabStop = false;
            this.DiskEstimationInnerPictureBox.Click += new System.EventHandler(this.DiskEstimationInnerPictureBox_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(212)))), ((int)(((byte)(234)))));
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 705);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1194, 1);
            this.panel2.TabIndex = 2;
            // 
            // slideshowpanel
            // 
            this.slideshowpanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(215)))), ((int)(((byte)(236)))));
            this.slideshowpanel.Controls.Add(this.scrollslideshowpanel);
            this.slideshowpanel.Controls.Add(this.panel3);
            this.slideshowpanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.slideshowpanel.Location = new System.Drawing.Point(0, 460);
            this.slideshowpanel.Margin = new System.Windows.Forms.Padding(0);
            this.slideshowpanel.Name = "slideshowpanel";
            this.slideshowpanel.Size = new System.Drawing.Size(1194, 245);
            this.slideshowpanel.TabIndex = 3;
            // 
            // scrollslideshowpanel
            // 
            this.scrollslideshowpanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scrollslideshowpanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(215)))), ((int)(((byte)(236)))));
            this.scrollslideshowpanel.Location = new System.Drawing.Point(0, 45);
            this.scrollslideshowpanel.Name = "scrollslideshowpanel";
            this.scrollslideshowpanel.Size = new System.Drawing.Size(1196, 200);
            this.scrollslideshowpanel.TabIndex = 3;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(212)))), ((int)(((byte)(234)))));
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.mRewindToStartSlideshowButton);
            this.panel3.Controls.Add(this.TotalSlidesSlideshowTextBox);
            this.panel3.Controls.Add(this.mStoryboardToolStrip);
            this.panel3.Controls.Add(this.mStopSlideshowButton);
            this.panel3.Controls.Add(this.SlideShowTimeLabel);
            this.panel3.Controls.Add(this.mStoryboardTrackBar);
            this.panel3.Controls.Add(this.mPlaySlideshowButton);
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1194, 53);
            this.panel3.TabIndex = 2;
            // 
            // mRewindToStartSlideshowButton
            // 
            this.mRewindToStartSlideshowButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mRewindToStartSlideshowButton.Image = ((System.Drawing.Image)(resources.GetObject("mRewindToStartSlideshowButton.Image")));
            this.mRewindToStartSlideshowButton.Location = new System.Drawing.Point(493, 9);
            this.mRewindToStartSlideshowButton.Name = "mRewindToStartSlideshowButton";
            this.mRewindToStartSlideshowButton.Size = new System.Drawing.Size(24, 24);
            this.mRewindToStartSlideshowButton.TabIndex = 39;
            this.mRewindToStartSlideshowButton.Text = "simpleBitmapButton2";
            // 
            // TotalSlidesSlideshowTextBox
            // 
            this.TotalSlidesSlideshowTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TotalSlidesSlideshowTextBox.Location = new System.Drawing.Point(408, 4);
            this.TotalSlidesSlideshowTextBox.Name = "TotalSlidesSlideshowTextBox";
            this.TotalSlidesSlideshowTextBox.Size = new System.Drawing.Size(96, 18);
            this.TotalSlidesSlideshowTextBox.TabIndex = 40;
            this.TotalSlidesSlideshowTextBox.Text = "Total: 400 slides";
            // 
            // mStoryboardToolStrip
            // 
            this.mStoryboardToolStrip.AutoSize = false;
            this.mStoryboardToolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(212)))), ((int)(((byte)(234)))));
            this.mStoryboardToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.mStoryboardToolStrip.ImageScalingSize = new System.Drawing.Size(44, 44);
            this.mStoryboardToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SlideshowAddSlidesButton,
            this.SlideshowAddMusicButton,
            this.mDoNarrationButton,
            this.toolStripSeparator1,
            this.mStoryboardEditItemButton,
            this.RotateSlideAntiClockwiseButton,
            this.RotateSlideClockWiseButton,
            this.mSetMenuthumbnailButton,
            this.mDeleteButton,
            this.toolStripSeparator2,
            this.SlideshowAdvanceOptionsButton});
            this.mStoryboardToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.mStoryboardToolStrip.Location = new System.Drawing.Point(0, -1);
            this.mStoryboardToolStrip.Name = "mStoryboardToolStrip";
            this.mStoryboardToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.mStoryboardToolStrip.Size = new System.Drawing.Size(421, 48);
            this.mStoryboardToolStrip.TabIndex = 47;
            this.mStoryboardToolStrip.Text = "toolStrip1";
            // 
            // SlideshowAddSlidesButton
            // 
            this.SlideshowAddSlidesButton.AutoSize = false;
            this.SlideshowAddSlidesButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(212)))), ((int)(((byte)(234)))));
            this.SlideshowAddSlidesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SlideshowAddSlidesButton.DropDownButtonWidth = 14;
            this.SlideshowAddSlidesButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddBlankSlideMenuItemButton,
            this.mCreateSlideFromTemplateToolStripMenuItem,
            this.openExplorerWindowToolStripMenuItem});
            this.SlideshowAddSlidesButton.Image = ((System.Drawing.Image)(resources.GetObject("SlideshowAddSlidesButton.Image")));
            this.SlideshowAddSlidesButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.SlideshowAddSlidesButton.ImageTransparentColor = System.Drawing.Color.Maroon;
            this.SlideshowAddSlidesButton.Margin = new System.Windows.Forms.Padding(2, 1, 0, 2);
            this.SlideshowAddSlidesButton.Name = "SlideshowAddSlidesButton";
            this.SlideshowAddSlidesButton.Size = new System.Drawing.Size(60, 44);
            this.SlideshowAddSlidesButton.Text = "toolStripSplitButton1";
            this.SlideshowAddSlidesButton.ToolTipText = "Add pictures or video to the slideshow";
            this.SlideshowAddSlidesButton.ButtonClick += new System.EventHandler(this.AddSlidesButton_Click);
            // 
            // AddBlankSlideMenuItemButton
            // 
            this.AddBlankSlideMenuItemButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(212)))), ((int)(((byte)(234)))));
            this.AddBlankSlideMenuItemButton.Image = ((System.Drawing.Image)(resources.GetObject("AddBlankSlideMenuItemButton.Image")));
            this.AddBlankSlideMenuItemButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.AddBlankSlideMenuItemButton.Name = "AddBlankSlideMenuItemButton";
            this.AddBlankSlideMenuItemButton.Size = new System.Drawing.Size(215, 22);
            this.AddBlankSlideMenuItemButton.Text = "Add blank slide";
            this.AddBlankSlideMenuItemButton.Click += new System.EventHandler(this.AddBlankSlideButton_Click);
            // 
            // mCreateSlideFromTemplateToolStripMenuItem
            // 
            this.mCreateSlideFromTemplateToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(212)))), ((int)(((byte)(234)))));
            this.mCreateSlideFromTemplateToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mCreateSlideFromTemplateToolStripMenuItem.Image")));
            this.mCreateSlideFromTemplateToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mCreateSlideFromTemplateToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.mCreateSlideFromTemplateToolStripMenuItem.Name = "mCreateSlideFromTemplateToolStripMenuItem";
            this.mCreateSlideFromTemplateToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.mCreateSlideFromTemplateToolStripMenuItem.Text = "Create slide from template";
            this.mCreateSlideFromTemplateToolStripMenuItem.Click += new System.EventHandler(this.mCreateSlideFromTemplateToolStripMenuItem_Click);
            // 
            // openExplorerWindowToolStripMenuItem
            // 
            this.openExplorerWindowToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openExplorerWindowToolStripMenuItem.Image")));
            this.openExplorerWindowToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.openExplorerWindowToolStripMenuItem.Name = "openExplorerWindowToolStripMenuItem";
            this.openExplorerWindowToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.openExplorerWindowToolStripMenuItem.Text = "Open explorer window";
            this.openExplorerWindowToolStripMenuItem.Click += new System.EventHandler(this.openExplorerWindowToolStripMenuItem_Click);
            // 
            // SlideshowAddMusicButton
            // 
            this.SlideshowAddMusicButton.AutoSize = false;
            this.SlideshowAddMusicButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SlideshowAddMusicButton.DropDownButtonWidth = 14;
            this.SlideshowAddMusicButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mAddOrEditMusicSlidesToolStripMenuItemMenuItem});
            this.SlideshowAddMusicButton.Image = ((System.Drawing.Image)(resources.GetObject("SlideshowAddMusicButton.Image")));
            this.SlideshowAddMusicButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.SlideshowAddMusicButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SlideshowAddMusicButton.Margin = new System.Windows.Forms.Padding(2, 1, 0, 2);
            this.SlideshowAddMusicButton.Name = "SlideshowAddMusicButton";
            this.SlideshowAddMusicButton.Size = new System.Drawing.Size(60, 44);
            this.SlideshowAddMusicButton.Text = "Add background music to the slideshow";
            this.SlideshowAddMusicButton.ButtonClick += new System.EventHandler(this.AddSlideshowMusicButton_Click);
            // 
            // mAddOrEditMusicSlidesToolStripMenuItemMenuItem
            // 
            this.mAddOrEditMusicSlidesToolStripMenuItemMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mAddOrEditMusicSlidesToolStripMenuItemMenuItem.Image")));
            this.mAddOrEditMusicSlidesToolStripMenuItemMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mAddOrEditMusicSlidesToolStripMenuItemMenuItem.Name = "mAddOrEditMusicSlidesToolStripMenuItemMenuItem";
            this.mAddOrEditMusicSlidesToolStripMenuItemMenuItem.Size = new System.Drawing.Size(270, 22);
            this.mAddOrEditMusicSlidesToolStripMenuItemMenuItem.Text = "Add or edit slideshow music track list";
            this.mAddOrEditMusicSlidesToolStripMenuItemMenuItem.Click += new System.EventHandler(this.mAddOrEditMusicSlidesToolStripMenuItemMenuItem_Click);
            // 
            // mDoNarrationButton
            // 
            this.mDoNarrationButton.AutoSize = false;
            this.mDoNarrationButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mDoNarrationButton.DropDownButtonWidth = 14;
            this.mDoNarrationButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mAddNarrationFromAudioFileToolStripMenuItem});
            this.mDoNarrationButton.Image = ((System.Drawing.Image)(resources.GetObject("mDoNarrationButton.Image")));
            this.mDoNarrationButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mDoNarrationButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mDoNarrationButton.Margin = new System.Windows.Forms.Padding(2, 1, 0, 2);
            this.mDoNarrationButton.Name = "mDoNarrationButton";
            this.mDoNarrationButton.Size = new System.Drawing.Size(46, 44);
            this.mDoNarrationButton.Text = "toolStripSplitButton1";
            this.mDoNarrationButton.ToolTipText = "Record narration from current position or selected slide";
            // 
            // mAddNarrationFromAudioFileToolStripMenuItem
            // 
            this.mAddNarrationFromAudioFileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mAddNarrationFromAudioFileToolStripMenuItem.Image")));
            this.mAddNarrationFromAudioFileToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mAddNarrationFromAudioFileToolStripMenuItem.Name = "mAddNarrationFromAudioFileToolStripMenuItem";
            this.mAddNarrationFromAudioFileToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.mAddNarrationFromAudioFileToolStripMenuItem.Text = "Add narration or sound from an audio file";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.AutoSize = false;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 44);
            // 
            // mStoryboardEditItemButton
            // 
            this.mStoryboardEditItemButton.AutoSize = false;
            this.mStoryboardEditItemButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mStoryboardEditItemButton.Image = ((System.Drawing.Image)(resources.GetObject("mStoryboardEditItemButton.Image")));
            this.mStoryboardEditItemButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mStoryboardEditItemButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mStoryboardEditItemButton.Name = "mStoryboardEditItemButton";
            this.mStoryboardEditItemButton.Size = new System.Drawing.Size(40, 44);
            this.mStoryboardEditItemButton.Text = "toolStripButton1";
            this.mStoryboardEditItemButton.ToolTipText = "Edit slide media";
            // 
            // RotateSlideAntiClockwiseButton
            // 
            this.RotateSlideAntiClockwiseButton.AutoSize = false;
            this.RotateSlideAntiClockwiseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RotateSlideAntiClockwiseButton.Image = ((System.Drawing.Image)(resources.GetObject("RotateSlideAntiClockwiseButton.Image")));
            this.RotateSlideAntiClockwiseButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.RotateSlideAntiClockwiseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RotateSlideAntiClockwiseButton.Name = "RotateSlideAntiClockwiseButton";
            this.RotateSlideAntiClockwiseButton.Size = new System.Drawing.Size(30, 44);
            this.RotateSlideAntiClockwiseButton.ToolTipText = "Rotate left 90 degrees";
            // 
            // RotateSlideClockWiseButton
            // 
            this.RotateSlideClockWiseButton.AutoSize = false;
            this.RotateSlideClockWiseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RotateSlideClockWiseButton.Image = ((System.Drawing.Image)(resources.GetObject("RotateSlideClockWiseButton.Image")));
            this.RotateSlideClockWiseButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.RotateSlideClockWiseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RotateSlideClockWiseButton.Name = "RotateSlideClockWiseButton";
            this.RotateSlideClockWiseButton.Size = new System.Drawing.Size(30, 44);
            this.RotateSlideClockWiseButton.Text = "toolStripButton2";
            this.RotateSlideClockWiseButton.ToolTipText = "Rotate right 90 degrees";
            // 
            // mSetMenuthumbnailButton
            // 
            this.mSetMenuthumbnailButton.AutoSize = false;
            this.mSetMenuthumbnailButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mSetMenuthumbnailButton.Image = ((System.Drawing.Image)(resources.GetObject("mSetMenuthumbnailButton.Image")));
            this.mSetMenuthumbnailButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mSetMenuthumbnailButton.Name = "mSetMenuthumbnailButton";
            this.mSetMenuthumbnailButton.Size = new System.Drawing.Size(44, 44);
            this.mSetMenuthumbnailButton.Text = "toolStripButton3";
            this.mSetMenuthumbnailButton.ToolTipText = "Set slide as menu thumbnail";
            // 
            // mDeleteButton
            // 
            this.mDeleteButton.AutoSize = false;
            this.mDeleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mDeleteButton.Image = ((System.Drawing.Image)(resources.GetObject("mDeleteButton.Image")));
            this.mDeleteButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mDeleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mDeleteButton.Name = "mDeleteButton";
            this.mDeleteButton.Size = new System.Drawing.Size(36, 44);
            this.mDeleteButton.Text = "toolStripButton1";
            this.mDeleteButton.ToolTipText = "Delete selected";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.AutoSize = false;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 44);
            // 
            // SlideshowAdvanceOptionsButton
            // 
            this.SlideshowAdvanceOptionsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SlideshowAdvanceOptionsButton.Image = ((System.Drawing.Image)(resources.GetObject("SlideshowAdvanceOptionsButton.Image")));
            this.SlideshowAdvanceOptionsButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.SlideshowAdvanceOptionsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SlideshowAdvanceOptionsButton.Name = "SlideshowAdvanceOptionsButton";
            this.SlideshowAdvanceOptionsButton.Size = new System.Drawing.Size(40, 44);
            this.SlideshowAdvanceOptionsButton.Text = "toolStripButton3";
            this.SlideshowAdvanceOptionsButton.ToolTipText = "Slideshow settings";
            this.SlideshowAdvanceOptionsButton.Click += new System.EventHandler(this.AdvanceSlideshowOptionButton_Click);
            // 
            // mStopSlideshowButton
            // 
            this.mStopSlideshowButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mStopSlideshowButton.Image = ((System.Drawing.Image)(resources.GetObject("mStopSlideshowButton.Image")));
            this.mStopSlideshowButton.Location = new System.Drawing.Point(556, 1);
            this.mStopSlideshowButton.Name = "mStopSlideshowButton";
            this.mStopSlideshowButton.Size = new System.Drawing.Size(38, 38);
            this.mStopSlideshowButton.TabIndex = 41;
            this.mStopSlideshowButton.Text = "simpleBitmapButton2";
            // 
            // SlideShowTimeLabel
            // 
            this.SlideShowTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SlideShowTimeLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(212)))), ((int)(((byte)(234)))));
            this.SlideShowTimeLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.SlideShowTimeLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.SlideShowTimeLabel.Location = new System.Drawing.Point(1107, 4);
            this.SlideShowTimeLabel.Name = "SlideShowTimeLabel";
            this.SlideShowTimeLabel.Size = new System.Drawing.Size(84, 38);
            this.SlideShowTimeLabel.TabIndex = 12;
            this.SlideShowTimeLabel.Text = "1:45:30/2:20:14";
            // 
            // mStoryboardTrackBar
            // 
            this.mStoryboardTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mStoryboardTrackBar.AutoSize = false;
            this.mStoryboardTrackBar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.mStoryboardTrackBar.LargeChange = 0;
            this.mStoryboardTrackBar.Location = new System.Drawing.Point(588, 7);
            this.mStoryboardTrackBar.Maximum = 1000;
            this.mStoryboardTrackBar.Name = "mStoryboardTrackBar";
            this.mStoryboardTrackBar.Size = new System.Drawing.Size(527, 43);
            this.mStoryboardTrackBar.SmallChange = 0;
            this.mStoryboardTrackBar.TabIndex = 7;
            this.mStoryboardTrackBar.TabStop = false;
            this.mStoryboardTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.mStoryboardTrackBar.Scroll += new System.EventHandler(this.trackBar2_Scroll);
            // 
            // mPlaySlideshowButton
            // 
            this.mPlaySlideshowButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mPlaySlideshowButton.Image = ((System.Drawing.Image)(resources.GetObject("mPlaySlideshowButton.Image")));
            this.mPlaySlideshowButton.Location = new System.Drawing.Point(517, 1);
            this.mPlaySlideshowButton.Name = "mPlaySlideshowButton";
            this.mPlaySlideshowButton.Size = new System.Drawing.Size(38, 38);
            this.mPlaySlideshowButton.TabIndex = 38;
            this.mPlaySlideshowButton.Text = "simpleBitmapButton2";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.mOuterTabControl);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel5.Location = new System.Drawing.Point(0, 71);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(500, 389);
            this.panel5.TabIndex = 5;
            // 
            // mOuterTabControl
            // 
            this.mOuterTabControl.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.mOuterTabControl.Controls.Add(this.tabPage1);
            this.mOuterTabControl.Controls.Add(this.tabPage3);
            this.mOuterTabControl.Controls.Add(this.mIntroSlideshowTabPage);
            this.mOuterTabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mOuterTabControl.ItemSize = new System.Drawing.Size(58, 1);
            this.mOuterTabControl.Location = new System.Drawing.Point(0, 0);
            this.mOuterTabControl.Margin = new System.Windows.Forms.Padding(0);
            this.mOuterTabControl.Multiline = true;
            this.mOuterTabControl.Name = "mOuterTabControl";
            this.mOuterTabControl.Padding = new System.Drawing.Point(0, 0);
            this.mOuterTabControl.SelectedIndex = 0;
            this.mOuterTabControl.Size = new System.Drawing.Size(550, 1034);
            this.mOuterTabControl.TabIndex = 0;
            this.mOuterTabControl.SelectedIndexChanged += new System.EventHandler(this.OuterTabControl_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.mMainMenuTabControl);
            this.tabPage1.Controls.Add(this.splitter1);
            this.tabPage1.Location = new System.Drawing.Point(4, 5);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(542, 1025);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Disk Menu";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // mMainMenuTabControl
            // 
            this.mMainMenuTabControl.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.mMainMenuTabControl.Controls.Add(this.mMenuStructureTabPage);
            this.mMainMenuTabControl.Controls.Add(this.mMenuThemeTabPage);
            this.mMainMenuTabControl.Controls.Add(this.mEmptyMenuTabPage);
            this.mMainMenuTabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.mMainMenuTabControl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mMainMenuTabControl.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.mMainMenuTabControl.ItemSize = new System.Drawing.Size(100, 48);
            this.mMainMenuTabControl.Location = new System.Drawing.Point(0, 0);
            this.mMainMenuTabControl.Margin = new System.Windows.Forms.Padding(0);
            this.mMainMenuTabControl.Multiline = true;
            this.mMainMenuTabControl.Name = "mMainMenuTabControl";
            this.mMainMenuTabControl.Padding = new System.Drawing.Point(0, 0);
            this.mMainMenuTabControl.SelectedIndex = 0;
            this.mMainMenuTabControl.Size = new System.Drawing.Size(539, 1025);
            this.mMainMenuTabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.mMainMenuTabControl.TabIndex = 11;
            this.mMainMenuTabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl3_SelectedIndexChanged);
            // 
            // mMenuStructureTabPage
            // 
            this.mMenuStructureTabPage.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mMenuStructureTabPage.BackgroundImage")));
            this.mMenuStructureTabPage.Controls.Add(this.mImportSlideshowButton);
            this.mMenuStructureTabPage.Controls.Add(this.label3);
            this.mMenuStructureTabPage.Controls.Add(this.mMenuIndexLabel);
            this.mMenuStructureTabPage.Controls.Add(this.mTotalMenuStatsLabel);
            this.mMenuStructureTabPage.Controls.Add(this.RemoveSlideshowButton);
            this.mMenuStructureTabPage.Controls.Add(this.NewSlideShow);
            this.mMenuStructureTabPage.Controls.Add(this.AddNextMenuButton);
            this.mMenuStructureTabPage.Controls.Add(this.label2);
            this.mMenuStructureTabPage.Controls.Add(this.pictureBox2);
            this.mMenuStructureTabPage.ImageIndex = 0;
            this.mMenuStructureTabPage.Location = new System.Drawing.Point(52, 4);
            this.mMenuStructureTabPage.Margin = new System.Windows.Forms.Padding(0);
            this.mMenuStructureTabPage.Name = "mMenuStructureTabPage";
            this.mMenuStructureTabPage.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mMenuStructureTabPage.Size = new System.Drawing.Size(483, 1017);
            this.mMenuStructureTabPage.TabIndex = 0;
            // 
            // mImportSlideshowButton
            // 
            this.mImportSlideshowButton.BackColor = System.Drawing.SystemColors.Control;
            this.mImportSlideshowButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mImportSlideshowButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mImportSlideshowButton.Location = new System.Drawing.Point(24, 90);
            this.mImportSlideshowButton.Name = "mImportSlideshowButton";
            this.mImportSlideshowButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.mImportSlideshowButton.Size = new System.Drawing.Size(132, 24);
            this.mImportSlideshowButton.TabIndex = 45;
            this.mImportSlideshowButton.Text = "Import slideshow";
            this.mImportSlideshowButton.UseVisualStyleBackColor = false;
            this.mImportSlideshowButton.Click += new System.EventHandler(this.mImportSlideshowsToolStripMenuItem_Click);
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.SteelBlue;
            this.label3.Location = new System.Drawing.Point(24, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(260, 38);
            this.label3.TabIndex = 44;
            this.label3.Text = "Disk menu structure";
            // 
            // mMenuIndexLabel
            // 
            this.mMenuIndexLabel.BackColor = System.Drawing.Color.Transparent;
            this.mMenuIndexLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mMenuIndexLabel.ForeColor = System.Drawing.Color.Black;
            this.mMenuIndexLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mMenuIndexLabel.Location = new System.Drawing.Point(320, 343);
            this.mMenuIndexLabel.Name = "mMenuIndexLabel";
            this.mMenuIndexLabel.Size = new System.Drawing.Size(106, 23);
            this.mMenuIndexLabel.TabIndex = 43;
            this.mMenuIndexLabel.Text = "Menu 1/1";
            // 
            // mTotalMenuStatsLabel
            // 
            this.mTotalMenuStatsLabel.BackColor = System.Drawing.Color.Transparent;
            this.mTotalMenuStatsLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTotalMenuStatsLabel.Location = new System.Drawing.Point(29, 334);
            this.mTotalMenuStatsLabel.Name = "mTotalMenuStatsLabel";
            this.mTotalMenuStatsLabel.Size = new System.Drawing.Size(255, 34);
            this.mTotalMenuStatsLabel.TabIndex = 41;
            this.mTotalMenuStatsLabel.Text = "Total slides:0";
            // 
            // RemoveSlideshowButton
            // 
            this.RemoveSlideshowButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.RemoveSlideshowButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RemoveSlideshowButton.Image = ((System.Drawing.Image)(resources.GetObject("RemoveSlideshowButton.Image")));
            this.RemoveSlideshowButton.Location = new System.Drawing.Point(25, 120);
            this.RemoveSlideshowButton.Name = "RemoveSlideshowButton";
            this.RemoveSlideshowButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RemoveSlideshowButton.Size = new System.Drawing.Size(131, 24);
            this.RemoveSlideshowButton.TabIndex = 27;
            this.RemoveSlideshowButton.Text = "Delete slideshow";
            // 
            // NewSlideShow
            // 
            this.NewSlideShow.BackColor = System.Drawing.SystemColors.Control;
            this.NewSlideShow.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.NewSlideShow.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewSlideShow.Location = new System.Drawing.Point(24, 60);
            this.NewSlideShow.Name = "NewSlideShow";
            this.NewSlideShow.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.NewSlideShow.Size = new System.Drawing.Size(132, 24);
            this.NewSlideShow.TabIndex = 23;
            this.NewSlideShow.Text = "Add new slideshow";
            this.NewSlideShow.UseVisualStyleBackColor = false;
            this.NewSlideShow.Click += new System.EventHandler(this.NewSlideShow_Click);
            // 
            // AddNextMenuButton
            // 
            this.AddNextMenuButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("AddNextMenuButton.BackgroundImage")));
            this.AddNextMenuButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.AddNextMenuButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddNextMenuButton.Location = new System.Drawing.Point(24, 171);
            this.AddNextMenuButton.Name = "AddNextMenuButton";
            this.AddNextMenuButton.Size = new System.Drawing.Size(132, 24);
            this.AddNextMenuButton.TabIndex = 25;
            this.AddNextMenuButton.Text = "Add new sub menu";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(-80, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 24);
            this.label2.TabIndex = 4;
            this.label2.Text = "Menu theme";
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(256, 175);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(152, 152);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 42;
            this.pictureBox2.TabStop = false;
            // 
            // mMenuThemeTabPage
            // 
            this.mMenuThemeTabPage.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mMenuThemeTabPage.BackgroundImage")));
            this.mMenuThemeTabPage.Controls.Add(this.mMenuThemeTab);
            this.mMenuThemeTabPage.Controls.Add(this.AddBackgroundMenuMusicButton);
            this.mMenuThemeTabPage.Controls.Add(this.mPreviewMenuButton);
            this.mMenuThemeTabPage.Controls.Add(this.MenuAddDecorateButton);
            this.mMenuThemeTabPage.Controls.Add(this.mZoomingMenuBackGroundCheckBox);
            this.mMenuThemeTabPage.Controls.Add(this.mMenuThemePlayingThumbnailsCheckBox);
            this.mMenuThemeTabPage.Controls.Add(this.blankout);
            this.mMenuThemeTabPage.Controls.Add(this.label12);
            this.mMenuThemeTabPage.Controls.Add(this.MenuRepeatDurationTextbox);
            this.mMenuThemeTabPage.Controls.Add(this.MenuRepeatLengthTrackbar);
            this.mMenuThemeTabPage.Controls.Add(this.MenuThemeLabel);
            this.mMenuThemeTabPage.Controls.Add(this.label7);
            this.mMenuThemeTabPage.Controls.Add(this.SetMenuBackgroundButton);
            this.mMenuThemeTabPage.Controls.Add(this.MenuBackgroundName);
            this.mMenuThemeTabPage.Controls.Add(this.MenuBackgroundMusicName);
            this.mMenuThemeTabPage.Controls.Add(this.label1);
            this.mMenuThemeTabPage.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mMenuThemeTabPage.ImageIndex = 1;
            this.mMenuThemeTabPage.Location = new System.Drawing.Point(52, 4);
            this.mMenuThemeTabPage.Name = "mMenuThemeTabPage";
            this.mMenuThemeTabPage.Size = new System.Drawing.Size(483, 1017);
            this.mMenuThemeTabPage.TabIndex = 2;
            // 
            // mMenuThemeTab
            // 
            this.mMenuThemeTab.Controls.Add(this.mMenuBackgroundTab);
            this.mMenuThemeTab.Controls.Add(this.mMenuLayoutTab);
            this.mMenuThemeTab.Controls.Add(this.mMenuFrameStyleTab);
            this.mMenuThemeTab.Controls.Add(this.mMenuNavigationStyleTab);
            this.mMenuThemeTab.Controls.Add(this.mMenuHighlightStlyeTab);
            this.mMenuThemeTab.Location = new System.Drawing.Point(8, 47);
            this.mMenuThemeTab.Name = "mMenuThemeTab";
            this.mMenuThemeTab.SelectedIndex = 0;
            this.mMenuThemeTab.Size = new System.Drawing.Size(438, 200);
            this.mMenuThemeTab.TabIndex = 51;
            // 
            // mMenuBackgroundTab
            // 
            this.mMenuBackgroundTab.Controls.Add(this.MenuTemplatesPanel);
            this.mMenuBackgroundTab.Location = new System.Drawing.Point(4, 22);
            this.mMenuBackgroundTab.Name = "mMenuBackgroundTab";
            this.mMenuBackgroundTab.Padding = new System.Windows.Forms.Padding(3);
            this.mMenuBackgroundTab.Size = new System.Drawing.Size(430, 174);
            this.mMenuBackgroundTab.TabIndex = 0;
            this.mMenuBackgroundTab.Text = "Background";
            this.mMenuBackgroundTab.UseVisualStyleBackColor = true;
            // 
            // MenuTemplatesPanel
            // 
            this.MenuTemplatesPanel.AutoScroll = true;
            this.MenuTemplatesPanel.BackColor = System.Drawing.Color.White;
            this.MenuTemplatesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MenuTemplatesPanel.Location = new System.Drawing.Point(3, 3);
            this.MenuTemplatesPanel.Name = "MenuTemplatesPanel";
            this.MenuTemplatesPanel.Size = new System.Drawing.Size(424, 168);
            this.MenuTemplatesPanel.TabIndex = 31;
            // 
            // mMenuLayoutTab
            // 
            this.mMenuLayoutTab.Controls.Add(this.MenuLayoutsPanel);
            this.mMenuLayoutTab.Location = new System.Drawing.Point(4, 22);
            this.mMenuLayoutTab.Name = "mMenuLayoutTab";
            this.mMenuLayoutTab.Padding = new System.Windows.Forms.Padding(3);
            this.mMenuLayoutTab.Size = new System.Drawing.Size(430, 174);
            this.mMenuLayoutTab.TabIndex = 1;
            this.mMenuLayoutTab.Text = "Layout";
            this.mMenuLayoutTab.UseVisualStyleBackColor = true;
            // 
            // MenuLayoutsPanel
            // 
            this.MenuLayoutsPanel.AutoScroll = true;
            this.MenuLayoutsPanel.BackColor = System.Drawing.Color.White;
            this.MenuLayoutsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MenuLayoutsPanel.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.MenuLayoutsPanel.Location = new System.Drawing.Point(3, 3);
            this.MenuLayoutsPanel.Name = "MenuLayoutsPanel";
            this.MenuLayoutsPanel.Size = new System.Drawing.Size(424, 168);
            this.MenuLayoutsPanel.TabIndex = 39;
            // 
            // mMenuFrameStyleTab
            // 
            this.mMenuFrameStyleTab.Controls.Add(this.MenuStylePanel);
            this.mMenuFrameStyleTab.Location = new System.Drawing.Point(4, 22);
            this.mMenuFrameStyleTab.Name = "mMenuFrameStyleTab";
            this.mMenuFrameStyleTab.Padding = new System.Windows.Forms.Padding(3);
            this.mMenuFrameStyleTab.Size = new System.Drawing.Size(430, 174);
            this.mMenuFrameStyleTab.TabIndex = 2;
            this.mMenuFrameStyleTab.Text = "Frame";
            this.mMenuFrameStyleTab.UseVisualStyleBackColor = true;
            // 
            // MenuStylePanel
            // 
            this.MenuStylePanel.AutoScroll = true;
            this.MenuStylePanel.BackColor = System.Drawing.Color.White;
            this.MenuStylePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MenuStylePanel.ForeColor = System.Drawing.SystemColors.Highlight;
            this.MenuStylePanel.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.MenuStylePanel.Location = new System.Drawing.Point(3, 3);
            this.MenuStylePanel.Name = "MenuStylePanel";
            this.MenuStylePanel.Size = new System.Drawing.Size(424, 168);
            this.MenuStylePanel.TabIndex = 40;
            // 
            // mMenuNavigationStyleTab
            // 
            this.mMenuNavigationStyleTab.Controls.Add(this.mMenuNavigationButtonsPanel);
            this.mMenuNavigationStyleTab.Controls.Add(this.mMenuNavigationButtonsLabel);
            this.mMenuNavigationStyleTab.Controls.Add(this.mMenuNavigationPlayAllLoopedButton);
            this.mMenuNavigationStyleTab.Controls.Add(this.mMenuNavigationPlayAllButton);
            this.mMenuNavigationStyleTab.Location = new System.Drawing.Point(4, 22);
            this.mMenuNavigationStyleTab.Name = "mMenuNavigationStyleTab";
            this.mMenuNavigationStyleTab.Padding = new System.Windows.Forms.Padding(3);
            this.mMenuNavigationStyleTab.Size = new System.Drawing.Size(430, 174);
            this.mMenuNavigationStyleTab.TabIndex = 3;
            this.mMenuNavigationStyleTab.Text = "Navigation";
            this.mMenuNavigationStyleTab.UseVisualStyleBackColor = true;
            // 
            // mMenuNavigationButtonsPanel
            // 
            this.mMenuNavigationButtonsPanel.AutoScroll = true;
            this.mMenuNavigationButtonsPanel.BackColor = System.Drawing.Color.White;
            this.mMenuNavigationButtonsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mMenuNavigationButtonsPanel.Location = new System.Drawing.Point(6, 24);
            this.mMenuNavigationButtonsPanel.Name = "mMenuNavigationButtonsPanel";
            this.mMenuNavigationButtonsPanel.Size = new System.Drawing.Size(417, 120);
            this.mMenuNavigationButtonsPanel.TabIndex = 3;
            // 
            // mMenuNavigationButtonsLabel
            // 
            this.mMenuNavigationButtonsLabel.AutoSize = true;
            this.mMenuNavigationButtonsLabel.Location = new System.Drawing.Point(6, 8);
            this.mMenuNavigationButtonsLabel.Name = "mMenuNavigationButtonsLabel";
            this.mMenuNavigationButtonsLabel.Size = new System.Drawing.Size(160, 13);
            this.mMenuNavigationButtonsLabel.TabIndex = 2;
            this.mMenuNavigationButtonsLabel.Text = "Menu navigation button style";
            // 
            // mMenuNavigationPlayAllLoopedButton
            // 
            this.mMenuNavigationPlayAllLoopedButton.AutoSize = true;
            this.mMenuNavigationPlayAllLoopedButton.Location = new System.Drawing.Point(174, 150);
            this.mMenuNavigationPlayAllLoopedButton.Name = "mMenuNavigationPlayAllLoopedButton";
            this.mMenuNavigationPlayAllLoopedButton.Size = new System.Drawing.Size(170, 17);
            this.mMenuNavigationPlayAllLoopedButton.TabIndex = 1;
            this.mMenuNavigationPlayAllLoopedButton.Text = "Add \'Play all looped\' button";
            this.mMenuNavigationPlayAllLoopedButton.UseVisualStyleBackColor = true;
            // 
            // mMenuNavigationPlayAllButton
            // 
            this.mMenuNavigationPlayAllButton.AutoSize = true;
            this.mMenuNavigationPlayAllButton.Location = new System.Drawing.Point(6, 151);
            this.mMenuNavigationPlayAllButton.Name = "mMenuNavigationPlayAllButton";
            this.mMenuNavigationPlayAllButton.Size = new System.Drawing.Size(130, 17);
            this.mMenuNavigationPlayAllButton.TabIndex = 0;
            this.mMenuNavigationPlayAllButton.Text = "Add \'Play all\' button";
            this.mMenuNavigationPlayAllButton.UseVisualStyleBackColor = true;
            // 
            // mMenuHighlightStlyeTab
            // 
            this.mMenuHighlightStlyeTab.Controls.Add(this.mMenuHighlightResetButton);
            this.mMenuHighlightStlyeTab.Controls.Add(this.mMenuHighlightColorLabel);
            this.mMenuHighlightStlyeTab.Controls.Add(this.mMenuHighlightStyleLabel);
            this.mMenuHighlightStlyeTab.Controls.Add(this.mMenuHighlightPlayButtonsLabel);
            this.mMenuHighlightStlyeTab.Controls.Add(this.mMenuHighlightNavigationButtonLabel);
            this.mMenuHighlightStlyeTab.Controls.Add(this.mMenuHighlightSlideshowButtonsLabel);
            this.mMenuHighlightStlyeTab.Controls.Add(this.mMenuHighlightColorButton);
            this.mMenuHighlightStlyeTab.Controls.Add(this.mMenuHighlightPlayButtonStyleCombo);
            this.mMenuHighlightStlyeTab.Controls.Add(this.mMenuHighlightNavigationButtonStyleCombo);
            this.mMenuHighlightStlyeTab.Controls.Add(this.mMenuHighlightSlideshowButtonStyleCombo);
            this.mMenuHighlightStlyeTab.Location = new System.Drawing.Point(4, 22);
            this.mMenuHighlightStlyeTab.Name = "mMenuHighlightStlyeTab";
            this.mMenuHighlightStlyeTab.Padding = new System.Windows.Forms.Padding(3);
            this.mMenuHighlightStlyeTab.Size = new System.Drawing.Size(430, 174);
            this.mMenuHighlightStlyeTab.TabIndex = 4;
            this.mMenuHighlightStlyeTab.Text = "Highlight";
            this.mMenuHighlightStlyeTab.UseVisualStyleBackColor = true;
            // 
            // mMenuHighlightResetButton
            // 
            this.mMenuHighlightResetButton.Location = new System.Drawing.Point(9, 145);
            this.mMenuHighlightResetButton.Name = "mMenuHighlightResetButton";
            this.mMenuHighlightResetButton.Size = new System.Drawing.Size(119, 23);
            this.mMenuHighlightResetButton.TabIndex = 11;
            this.mMenuHighlightResetButton.Text = "Reset to default";
            this.mMenuHighlightResetButton.UseVisualStyleBackColor = true;
            // 
            // mMenuHighlightColorLabel
            // 
            this.mMenuHighlightColorLabel.AutoSize = true;
            this.mMenuHighlightColorLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mMenuHighlightColorLabel.Location = new System.Drawing.Point(298, 15);
            this.mMenuHighlightColorLabel.Name = "mMenuHighlightColorLabel";
            this.mMenuHighlightColorLabel.Size = new System.Drawing.Size(35, 13);
            this.mMenuHighlightColorLabel.TabIndex = 10;
            this.mMenuHighlightColorLabel.Text = "Color";
            // 
            // mMenuHighlightStyleLabel
            // 
            this.mMenuHighlightStyleLabel.AutoSize = true;
            this.mMenuHighlightStyleLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mMenuHighlightStyleLabel.Location = new System.Drawing.Point(6, 15);
            this.mMenuHighlightStyleLabel.Name = "mMenuHighlightStyleLabel";
            this.mMenuHighlightStyleLabel.Size = new System.Drawing.Size(83, 13);
            this.mMenuHighlightStyleLabel.TabIndex = 9;
            this.mMenuHighlightStyleLabel.Text = "Highlight style";
            // 
            // mMenuHighlightPlayButtonsLabel
            // 
            this.mMenuHighlightPlayButtonsLabel.AutoSize = true;
            this.mMenuHighlightPlayButtonsLabel.Location = new System.Drawing.Point(6, 102);
            this.mMenuHighlightPlayButtonsLabel.Name = "mMenuHighlightPlayButtonsLabel";
            this.mMenuHighlightPlayButtonsLabel.Size = new System.Drawing.Size(71, 13);
            this.mMenuHighlightPlayButtonsLabel.TabIndex = 8;
            this.mMenuHighlightPlayButtonsLabel.Text = "Play buttons";
            // 
            // mMenuHighlightNavigationButtonLabel
            // 
            this.mMenuHighlightNavigationButtonLabel.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mMenuHighlightNavigationButtonLabel.AutoSize = true;
            this.mMenuHighlightNavigationButtonLabel.Location = new System.Drawing.Point(6, 73);
            this.mMenuHighlightNavigationButtonLabel.Name = "mMenuHighlightNavigationButtonLabel";
            this.mMenuHighlightNavigationButtonLabel.Size = new System.Drawing.Size(102, 13);
            this.mMenuHighlightNavigationButtonLabel.TabIndex = 7;
            this.mMenuHighlightNavigationButtonLabel.Text = "Navigation button";
            // 
            // mMenuHighlightSlideshowButtonsLabel
            // 
            this.mMenuHighlightSlideshowButtonsLabel.AutoSize = true;
            this.mMenuHighlightSlideshowButtonsLabel.Location = new System.Drawing.Point(6, 44);
            this.mMenuHighlightSlideshowButtonsLabel.Name = "mMenuHighlightSlideshowButtonsLabel";
            this.mMenuHighlightSlideshowButtonsLabel.Size = new System.Drawing.Size(99, 13);
            this.mMenuHighlightSlideshowButtonsLabel.TabIndex = 6;
            this.mMenuHighlightSlideshowButtonsLabel.Text = "Slideshow button";
            // 
            // mMenuHighlightColorButton
            // 
            this.mMenuHighlightColorButton.Location = new System.Drawing.Point(301, 37);
            this.mMenuHighlightColorButton.Name = "mMenuHighlightColorButton";
            this.mMenuHighlightColorButton.Size = new System.Drawing.Size(32, 33);
            this.mMenuHighlightColorButton.TabIndex = 3;
            this.mMenuHighlightColorButton.UseVisualStyleBackColor = true;
            // 
            // mMenuHighlightPlayButtonStyleCombo
            // 
            this.mMenuHighlightPlayButtonStyleCombo.AllowDrop = true;
            this.mMenuHighlightPlayButtonStyleCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mMenuHighlightPlayButtonStyleCombo.FormattingEnabled = true;
            this.mMenuHighlightPlayButtonStyleCombo.Items.AddRange(new object[] {
            "Outline rectangle",
            "Covering rectangle",
            "Underline",
            "Circle on left",
            "Circle on right",
            "Square on left",
            "Square on right"});
            this.mMenuHighlightPlayButtonStyleCombo.Location = new System.Drawing.Point(111, 99);
            this.mMenuHighlightPlayButtonStyleCombo.Name = "mMenuHighlightPlayButtonStyleCombo";
            this.mMenuHighlightPlayButtonStyleCombo.Size = new System.Drawing.Size(152, 21);
            this.mMenuHighlightPlayButtonStyleCombo.TabIndex = 2;
            // 
            // mMenuHighlightNavigationButtonStyleCombo
            // 
            this.mMenuHighlightNavigationButtonStyleCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mMenuHighlightNavigationButtonStyleCombo.FormattingEnabled = true;
            this.mMenuHighlightNavigationButtonStyleCombo.Items.AddRange(new object[] {
            "Outline rectangle",
            "Covering rectangle",
            "Highlight",
            "Circle on left",
            "Circle on right",
            "Square on left",
            "Square on right"});
            this.mMenuHighlightNavigationButtonStyleCombo.Location = new System.Drawing.Point(111, 70);
            this.mMenuHighlightNavigationButtonStyleCombo.Name = "mMenuHighlightNavigationButtonStyleCombo";
            this.mMenuHighlightNavigationButtonStyleCombo.Size = new System.Drawing.Size(152, 21);
            this.mMenuHighlightNavigationButtonStyleCombo.TabIndex = 1;
            // 
            // mMenuHighlightSlideshowButtonStyleCombo
            // 
            this.mMenuHighlightSlideshowButtonStyleCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mMenuHighlightSlideshowButtonStyleCombo.FormattingEnabled = true;
            this.mMenuHighlightSlideshowButtonStyleCombo.Items.AddRange(new object[] {
            "Outline rectangle",
            "Covering rectangle",
            "Highlight",
            "Circle on left",
            "Circle on right",
            "Square on left",
            "Square on right"});
            this.mMenuHighlightSlideshowButtonStyleCombo.Location = new System.Drawing.Point(111, 39);
            this.mMenuHighlightSlideshowButtonStyleCombo.Name = "mMenuHighlightSlideshowButtonStyleCombo";
            this.mMenuHighlightSlideshowButtonStyleCombo.Size = new System.Drawing.Size(152, 21);
            this.mMenuHighlightSlideshowButtonStyleCombo.TabIndex = 0;
            // 
            // AddBackgroundMenuMusicButton
            // 
            this.AddBackgroundMenuMusicButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.AddBackgroundMenuMusicButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddBackgroundMenuMusicButton.ForeColor = System.Drawing.SystemColors.Highlight;
            this.AddBackgroundMenuMusicButton.Location = new System.Drawing.Point(8, 277);
            this.AddBackgroundMenuMusicButton.Name = "AddBackgroundMenuMusicButton";
            this.AddBackgroundMenuMusicButton.Size = new System.Drawing.Size(132, 23);
            this.AddBackgroundMenuMusicButton.TabIndex = 13;
            this.AddBackgroundMenuMusicButton.Text = "Set menu music";
            this.AddBackgroundMenuMusicButton.Click += new System.EventHandler(this.AddBackgroundMenuMusicButton_Click);
            // 
            // mPreviewMenuButton
            // 
            this.mPreviewMenuButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mPreviewMenuButton.Location = new System.Drawing.Point(330, 345);
            this.mPreviewMenuButton.Name = "mPreviewMenuButton";
            this.mPreviewMenuButton.Size = new System.Drawing.Size(94, 23);
            this.mPreviewMenuButton.TabIndex = 50;
            this.mPreviewMenuButton.Text = "Preview menu";
            this.mPreviewMenuButton.UseVisualStyleBackColor = true;
            // 
            // MenuAddDecorateButton
            // 
            this.MenuAddDecorateButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.MenuAddDecorateButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MenuAddDecorateButton.Location = new System.Drawing.Point(186, 345);
            this.MenuAddDecorateButton.Name = "MenuAddDecorateButton";
            this.MenuAddDecorateButton.Size = new System.Drawing.Size(140, 23);
            this.MenuAddDecorateButton.TabIndex = 36;
            this.MenuAddDecorateButton.Text = "Edit menu layout / font";
            // 
            // mZoomingMenuBackGroundCheckBox
            // 
            this.mZoomingMenuBackGroundCheckBox.AutoSize = true;
            this.mZoomingMenuBackGroundCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.mZoomingMenuBackGroundCheckBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mZoomingMenuBackGroundCheckBox.Location = new System.Drawing.Point(8, 349);
            this.mZoomingMenuBackGroundCheckBox.Name = "mZoomingMenuBackGroundCheckBox";
            this.mZoomingMenuBackGroundCheckBox.Size = new System.Drawing.Size(138, 17);
            this.mZoomingMenuBackGroundCheckBox.TabIndex = 49;
            this.mZoomingMenuBackGroundCheckBox.Text = "Zooming background";
            this.mZoomingMenuBackGroundCheckBox.UseVisualStyleBackColor = false;
            // 
            // mMenuThemePlayingThumbnailsCheckBox
            // 
            this.mMenuThemePlayingThumbnailsCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.mMenuThemePlayingThumbnailsCheckBox.Checked = true;
            this.mMenuThemePlayingThumbnailsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mMenuThemePlayingThumbnailsCheckBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mMenuThemePlayingThumbnailsCheckBox.Location = new System.Drawing.Point(8, 326);
            this.mMenuThemePlayingThumbnailsCheckBox.Name = "mMenuThemePlayingThumbnailsCheckBox";
            this.mMenuThemePlayingThumbnailsCheckBox.Size = new System.Drawing.Size(172, 24);
            this.mMenuThemePlayingThumbnailsCheckBox.TabIndex = 48;
            this.mMenuThemePlayingThumbnailsCheckBox.Text = "Playing thumbnails";
            this.mMenuThemePlayingThumbnailsCheckBox.UseVisualStyleBackColor = false;
            // 
            // blankout
            // 
            this.blankout.BackColor = System.Drawing.Color.Transparent;
            this.blankout.Location = new System.Drawing.Point(80, 325);
            this.blankout.Name = "blankout";
            this.blankout.Size = new System.Drawing.Size(345, 44);
            this.blankout.TabIndex = 46;
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.Color.White;
            this.label12.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label12.Location = new System.Drawing.Point(370, 306);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 24);
            this.label12.TabIndex = 20;
            this.label12.Text = "Seconds";
            this.label12.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // MenuRepeatDurationTextbox
            // 
            this.MenuRepeatDurationTextbox.BackColor = System.Drawing.Color.White;
            this.MenuRepeatDurationTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MenuRepeatDurationTextbox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MenuRepeatDurationTextbox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.MenuRepeatDurationTextbox.Location = new System.Drawing.Point(338, 306);
            this.MenuRepeatDurationTextbox.Name = "MenuRepeatDurationTextbox";
            this.MenuRepeatDurationTextbox.Size = new System.Drawing.Size(31, 18);
            this.MenuRepeatDurationTextbox.TabIndex = 44;
            this.MenuRepeatDurationTextbox.Text = "00";
            this.MenuRepeatDurationTextbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MenuRepeatLengthTrackbar
            // 
            this.MenuRepeatLengthTrackbar.BackColor = System.Drawing.SystemColors.Control;
            this.MenuRepeatLengthTrackbar.Location = new System.Drawing.Point(144, 301);
            this.MenuRepeatLengthTrackbar.Maximum = 140;
            this.MenuRepeatLengthTrackbar.Minimum = 10;
            this.MenuRepeatLengthTrackbar.Name = "MenuRepeatLengthTrackbar";
            this.MenuRepeatLengthTrackbar.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.MenuRepeatLengthTrackbar.Size = new System.Drawing.Size(192, 45);
            this.MenuRepeatLengthTrackbar.TabIndex = 18;
            this.MenuRepeatLengthTrackbar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.MenuRepeatLengthTrackbar.Value = 30;
            // 
            // MenuThemeLabel
            // 
            this.MenuThemeLabel.BackColor = System.Drawing.Color.Transparent;
            this.MenuThemeLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MenuThemeLabel.ForeColor = System.Drawing.Color.SteelBlue;
            this.MenuThemeLabel.Location = new System.Drawing.Point(24, 8);
            this.MenuThemeLabel.Name = "MenuThemeLabel";
            this.MenuThemeLabel.Size = new System.Drawing.Size(224, 38);
            this.MenuThemeLabel.TabIndex = 45;
            this.MenuThemeLabel.Text = "Disk menu theme";
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label7.Location = new System.Drawing.Point(11, 300);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(128, 24);
            this.label7.TabIndex = 34;
            this.label7.Text = "Menu length";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SetMenuBackgroundButton
            // 
            this.SetMenuBackgroundButton.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.SetMenuBackgroundButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.SetMenuBackgroundButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SetMenuBackgroundButton.Location = new System.Drawing.Point(8, 253);
            this.SetMenuBackgroundButton.Name = "SetMenuBackgroundButton";
            this.SetMenuBackgroundButton.Size = new System.Drawing.Size(132, 23);
            this.SetMenuBackgroundButton.TabIndex = 29;
            this.SetMenuBackgroundButton.Text = "Set background";
            this.SetMenuBackgroundButton.UseVisualStyleBackColor = false;
            this.SetMenuBackgroundButton.Click += new System.EventHandler(this.SetMenuBackgroundButton_Click);
            // 
            // MenuBackgroundName
            // 
            this.MenuBackgroundName.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MenuBackgroundName.Location = new System.Drawing.Point(144, 253);
            this.MenuBackgroundName.Name = "MenuBackgroundName";
            this.MenuBackgroundName.ReadOnly = true;
            this.MenuBackgroundName.Size = new System.Drawing.Size(280, 22);
            this.MenuBackgroundName.TabIndex = 30;
            this.MenuBackgroundName.Text = "None";
            // 
            // MenuBackgroundMusicName
            // 
            this.MenuBackgroundMusicName.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MenuBackgroundMusicName.Location = new System.Drawing.Point(144, 277);
            this.MenuBackgroundMusicName.Name = "MenuBackgroundMusicName";
            this.MenuBackgroundMusicName.ReadOnly = true;
            this.MenuBackgroundMusicName.Size = new System.Drawing.Size(280, 22);
            this.MenuBackgroundMusicName.TabIndex = 32;
            this.MenuBackgroundMusicName.Text = "None";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(323, 330);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 11);
            this.label1.TabIndex = 47;
            // 
            // mEmptyMenuTabPage
            // 
            this.mEmptyMenuTabPage.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mEmptyMenuTabPage.BackgroundImage")));
            this.mEmptyMenuTabPage.Controls.Add(this.pictureBox3);
            this.mEmptyMenuTabPage.ImageIndex = 2;
            this.mEmptyMenuTabPage.Location = new System.Drawing.Point(52, 4);
            this.mEmptyMenuTabPage.Name = "mEmptyMenuTabPage";
            this.mEmptyMenuTabPage.Size = new System.Drawing.Size(483, 1017);
            this.mEmptyMenuTabPage.TabIndex = 1;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox3.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(256, 792);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(152, 152);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 43;
            this.pictureBox3.TabStop = false;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 1025);
            this.splitter1.TabIndex = 7;
            this.splitter1.TabStop = false;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.mSlideOptionsTab);
            this.tabPage3.Location = new System.Drawing.Point(4, 5);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(542, 1025);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "decorations";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // mSlideOptionsTab
            // 
            this.mSlideOptionsTab.AccessibleName = "";
            this.mSlideOptionsTab.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.mSlideOptionsTab.Controls.Add(this.mPreDesignedSlidesTabPage);
            this.mSlideOptionsTab.Controls.Add(this.mBackgroundTabPage);
            this.mSlideOptionsTab.Controls.Add(this.tabPage2);
            this.mSlideOptionsTab.Controls.Add(this.ClipPanTabPage);
            this.mSlideOptionsTab.Controls.Add(this.mFiltersTabPage);
            this.mSlideOptionsTab.Controls.Add(this.mBordersTab);
            this.mSlideOptionsTab.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.mSlideOptionsTab.ImageList = this.DecoratePanZoomImageList;
            this.mSlideOptionsTab.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.mSlideOptionsTab.ItemSize = new System.Drawing.Size(58, 48);
            this.mSlideOptionsTab.Location = new System.Drawing.Point(2, 0);
            this.mSlideOptionsTab.Margin = new System.Windows.Forms.Padding(0);
            this.mSlideOptionsTab.Multiline = true;
            this.mSlideOptionsTab.Name = "mSlideOptionsTab";
            this.mSlideOptionsTab.Padding = new System.Drawing.Point(1, 0);
            this.mSlideOptionsTab.SelectedIndex = 0;
            this.mSlideOptionsTab.Size = new System.Drawing.Size(550, 1025);
            this.mSlideOptionsTab.TabIndex = 17;
            this.mSlideOptionsTab.SelectedIndexChanged += new System.EventHandler(this.tabControl2_SelectedIndexChanged);
            // 
            // mPreDesignedSlidesTabPage
            // 
            this.mPreDesignedSlidesTabPage.BackColor = System.Drawing.Color.White;
            this.mPreDesignedSlidesTabPage.Controls.Add(this.mPredefinedSlideDesignsControl1);
            this.mPreDesignedSlidesTabPage.ImageIndex = 0;
            this.mPreDesignedSlidesTabPage.Location = new System.Drawing.Point(52, 4);
            this.mPreDesignedSlidesTabPage.Margin = new System.Windows.Forms.Padding(0);
            this.mPreDesignedSlidesTabPage.Name = "mPreDesignedSlidesTabPage";
            this.mPreDesignedSlidesTabPage.Size = new System.Drawing.Size(494, 1017);
            this.mPreDesignedSlidesTabPage.TabIndex = 2;
            // 
            // mPredefinedSlideDesignsControl1
            // 
            this.mPredefinedSlideDesignsControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mPredefinedSlideDesignsControl1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mPredefinedSlideDesignsControl1.BackgroundImage")));
            this.mPredefinedSlideDesignsControl1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mPredefinedSlideDesignsControl1.Location = new System.Drawing.Point(0, 0);
            this.mPredefinedSlideDesignsControl1.Margin = new System.Windows.Forms.Padding(0);
            this.mPredefinedSlideDesignsControl1.Name = "mPredefinedSlideDesignsControl1";
            this.mPredefinedSlideDesignsControl1.Size = new System.Drawing.Size(494, 1017);
            this.mPredefinedSlideDesignsControl1.TabIndex = 0;
            // 
            // mBackgroundTabPage
            // 
            this.mBackgroundTabPage.BackgroundImage = global::dvdslideshowfontend.Properties.Resources.back24;
            this.mBackgroundTabPage.Controls.Add(this.mSelectBackgroundControl1);
            this.mBackgroundTabPage.ImageIndex = 1;
            this.mBackgroundTabPage.Location = new System.Drawing.Point(52, 4);
            this.mBackgroundTabPage.Name = "mBackgroundTabPage";
            this.mBackgroundTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.mBackgroundTabPage.Size = new System.Drawing.Size(494, 1017);
            this.mBackgroundTabPage.TabIndex = 5;
            this.mBackgroundTabPage.UseVisualStyleBackColor = true;
            // 
            // mSelectBackgroundControl1
            // 
            this.mSelectBackgroundControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mSelectBackgroundControl1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mSelectBackgroundControl1.BackgroundImage")));
            this.mSelectBackgroundControl1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mSelectBackgroundControl1.Location = new System.Drawing.Point(0, 0);
            this.mSelectBackgroundControl1.Margin = new System.Windows.Forms.Padding(0);
            this.mSelectBackgroundControl1.Name = "mSelectBackgroundControl1";
            this.mSelectBackgroundControl1.Size = new System.Drawing.Size(494, 1017);
            this.mSelectBackgroundControl1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Transparent;
            this.tabPage2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPage2.BackgroundImage")));
            this.tabPage2.Controls.Add(this.mSelectedDecorationCombo);
            this.tabPage2.Controls.Add(this.label14);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.mDecorationPositionHeightTextBox);
            this.tabPage2.Controls.Add(this.mDecorationPositionLeftTextBox);
            this.tabPage2.Controls.Add(this.mDecorationPositionWidthTextBox);
            this.tabPage2.Controls.Add(this.mDoneDecorationMenuButton);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.mClipartTransparencyNumerical);
            this.tabPage2.Controls.Add(this.mDecorationPositionTopTextBox);
            this.tabPage2.Controls.Add(this.mDecorationRotationNumericArrows);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.ClipartTransparencyTrackerBar);
            this.tabPage2.Controls.Add(this.mForceEnlargeLabel);
            this.tabPage2.Controls.Add(this.label13);
            this.tabPage2.Controls.Add(this.mDecorationRotationLabel);
            this.tabPage2.Controls.Add(this.OrderLayersButton);
            this.tabPage2.Controls.Add(this.label18);
            this.tabPage2.Controls.Add(this.mEffectsEditorButton);
            this.tabPage2.Controls.Add(this.ClipartTransparencyLabel);
            this.tabPage2.Controls.Add(this.DecorationTabOptions);
            this.tabPage2.Controls.Add(this.mDecorationRotationTrackbar);
            this.tabPage2.Controls.Add(this.Transparentlabel);
            this.tabPage2.ImageIndex = 2;
            this.tabPage2.Location = new System.Drawing.Point(52, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(494, 1017);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.Click += new System.EventHandler(this.tabPage2_Click);
            // 
            // mSelectedDecorationCombo
            // 
            this.mSelectedDecorationCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mSelectedDecorationCombo.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mSelectedDecorationCombo.FormattingEnabled = true;
            this.mSelectedDecorationCombo.Items.AddRange(new object[] {
            "None",
            "All Decorations Grouped"});
            this.mSelectedDecorationCombo.Location = new System.Drawing.Point(7, 172);
            this.mSelectedDecorationCombo.Name = "mSelectedDecorationCombo";
            this.mSelectedDecorationCombo.Size = new System.Drawing.Size(167, 21);
            this.mSelectedDecorationCombo.TabIndex = 19;
            this.mSelectedDecorationCombo.TabStop = false;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(190, 242);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(15, 13);
            this.label14.TabIndex = 73;
            this.label14.Text = "%";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(190, 216);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(15, 13);
            this.label11.TabIndex = 72;
            this.label11.Text = "%";
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(4, 237);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(62, 23);
            this.label10.TabIndex = 71;
            this.label10.Text = "Size: ";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 211);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 23);
            this.label4.TabIndex = 70;
            this.label4.Text = "Position:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mDecorationPositionHeightTextBox
            // 
            this.mDecorationPositionHeightTextBox.DecimalPlaces = 2;
            this.mDecorationPositionHeightTextBox.Location = new System.Drawing.Point(131, 240);
            this.mDecorationPositionHeightTextBox.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.mDecorationPositionHeightTextBox.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.mDecorationPositionHeightTextBox.Name = "mDecorationPositionHeightTextBox";
            this.mDecorationPositionHeightTextBox.Size = new System.Drawing.Size(56, 20);
            this.mDecorationPositionHeightTextBox.TabIndex = 57;
            this.mDecorationPositionHeightTextBox.TabStop = false;
            // 
            // mDecorationPositionLeftTextBox
            // 
            this.mDecorationPositionLeftTextBox.DecimalPlaces = 2;
            this.mDecorationPositionLeftTextBox.Location = new System.Drawing.Point(69, 214);
            this.mDecorationPositionLeftTextBox.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.mDecorationPositionLeftTextBox.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.mDecorationPositionLeftTextBox.Name = "mDecorationPositionLeftTextBox";
            this.mDecorationPositionLeftTextBox.Size = new System.Drawing.Size(56, 20);
            this.mDecorationPositionLeftTextBox.TabIndex = 54;
            this.mDecorationPositionLeftTextBox.TabStop = false;
            // 
            // mDecorationPositionWidthTextBox
            // 
            this.mDecorationPositionWidthTextBox.DecimalPlaces = 2;
            this.mDecorationPositionWidthTextBox.Location = new System.Drawing.Point(69, 240);
            this.mDecorationPositionWidthTextBox.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.mDecorationPositionWidthTextBox.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.mDecorationPositionWidthTextBox.Name = "mDecorationPositionWidthTextBox";
            this.mDecorationPositionWidthTextBox.Size = new System.Drawing.Size(56, 20);
            this.mDecorationPositionWidthTextBox.TabIndex = 56;
            this.mDecorationPositionWidthTextBox.TabStop = false;
            // 
            // mDoneDecorationMenuButton
            // 
            this.mDoneDecorationMenuButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mDoneDecorationMenuButton.Location = new System.Drawing.Point(348, 351);
            this.mDoneDecorationMenuButton.Name = "mDoneDecorationMenuButton";
            this.mDoneDecorationMenuButton.Size = new System.Drawing.Size(75, 23);
            this.mDoneDecorationMenuButton.TabIndex = 69;
            this.mDoneDecorationMenuButton.TabStop = false;
            this.mDoneDecorationMenuButton.Text = "Done";
            this.mDoneDecorationMenuButton.UseVisualStyleBackColor = true;
            this.mDoneDecorationMenuButton.Visible = false;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(185, 158);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 17);
            this.label6.TabIndex = 18;
            this.label6.Text = "Transparency";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mClipartTransparencyNumerical
            // 
            this.mClipartTransparencyNumerical.AllowDrop = true;
            this.mClipartTransparencyNumerical.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mClipartTransparencyNumerical.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mClipartTransparencyNumerical.Location = new System.Drawing.Point(386, 157);
            this.mClipartTransparencyNumerical.Margin = new System.Windows.Forms.Padding(0);
            this.mClipartTransparencyNumerical.Name = "mClipartTransparencyNumerical";
            this.mClipartTransparencyNumerical.Size = new System.Drawing.Size(16, 18);
            this.mClipartTransparencyNumerical.TabIndex = 68;
            this.mClipartTransparencyNumerical.TabStop = false;
            // 
            // mDecorationPositionTopTextBox
            // 
            this.mDecorationPositionTopTextBox.DecimalPlaces = 2;
            this.mDecorationPositionTopTextBox.Location = new System.Drawing.Point(131, 214);
            this.mDecorationPositionTopTextBox.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.mDecorationPositionTopTextBox.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.mDecorationPositionTopTextBox.Name = "mDecorationPositionTopTextBox";
            this.mDecorationPositionTopTextBox.Size = new System.Drawing.Size(56, 20);
            this.mDecorationPositionTopTextBox.TabIndex = 55;
            this.mDecorationPositionTopTextBox.TabStop = false;
            this.mDecorationPositionTopTextBox.Value = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            // 
            // mDecorationRotationNumericArrows
            // 
            this.mDecorationRotationNumericArrows.AllowDrop = true;
            this.mDecorationRotationNumericArrows.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mDecorationRotationNumericArrows.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mDecorationRotationNumericArrows.Location = new System.Drawing.Point(381, 324);
            this.mDecorationRotationNumericArrows.Margin = new System.Windows.Forms.Padding(0);
            this.mDecorationRotationNumericArrows.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.mDecorationRotationNumericArrows.Minimum = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
            this.mDecorationRotationNumericArrows.Name = "mDecorationRotationNumericArrows";
            this.mDecorationRotationNumericArrows.Size = new System.Drawing.Size(16, 18);
            this.mDecorationRotationNumericArrows.TabIndex = 67;
            this.mDecorationRotationNumericArrows.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.mDecorationStartEndSlideTimeControl);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(211, 193);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(215, 126);
            this.groupBox2.TabIndex = 61;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Shown time";
            // 
            // mDecorationStartEndSlideTimeControl
            // 
            this.mDecorationStartEndSlideTimeControl.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.mDecorationStartEndSlideTimeControl.Location = new System.Drawing.Point(0, 27);
            this.mDecorationStartEndSlideTimeControl.Name = "mDecorationStartEndSlideTimeControl";
            this.mDecorationStartEndSlideTimeControl.Size = new System.Drawing.Size(212, 86);
            this.mDecorationStartEndSlideTimeControl.TabIndex = 0;
            this.mDecorationStartEndSlideTimeControl.TabStop = false;
            // 
            // ClipartTransparencyTrackerBar
            // 
            this.ClipartTransparencyTrackerBar.BackColor = System.Drawing.Color.White;
            this.ClipartTransparencyTrackerBar.Location = new System.Drawing.Point(269, 156);
            this.ClipartTransparencyTrackerBar.Maximum = 100;
            this.ClipartTransparencyTrackerBar.Name = "ClipartTransparencyTrackerBar";
            this.ClipartTransparencyTrackerBar.Size = new System.Drawing.Size(120, 45);
            this.ClipartTransparencyTrackerBar.TabIndex = 19;
            this.ClipartTransparencyTrackerBar.TabStop = false;
            this.ClipartTransparencyTrackerBar.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // mForceEnlargeLabel
            // 
            this.mForceEnlargeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mForceEnlargeLabel.AutoSize = true;
            this.mForceEnlargeLabel.Location = new System.Drawing.Point(397, 1001);
            this.mForceEnlargeLabel.Name = "mForceEnlargeLabel";
            this.mForceEnlargeLabel.Size = new System.Drawing.Size(13, 13);
            this.mForceEnlargeLabel.TabIndex = 66;
            this.mForceEnlargeLabel.Text = "  ";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(8, 154);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(109, 13);
            this.label13.TabIndex = 33;
            this.label13.Text = "Selected decoration";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mDecorationRotationLabel
            // 
            this.mDecorationRotationLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mDecorationRotationLabel.Location = new System.Drawing.Point(400, 323);
            this.mDecorationRotationLabel.Name = "mDecorationRotationLabel";
            this.mDecorationRotationLabel.Size = new System.Drawing.Size(39, 19);
            this.mDecorationRotationLabel.TabIndex = 65;
            this.mDecorationRotationLabel.Text = "0°";
            this.mDecorationRotationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // OrderLayersButton
            // 
            this.OrderLayersButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OrderLayersButton.Location = new System.Drawing.Point(7, 312);
            this.OrderLayersButton.Name = "OrderLayersButton";
            this.OrderLayersButton.Size = new System.Drawing.Size(96, 23);
            this.OrderLayersButton.TabIndex = 62;
            this.OrderLayersButton.TabStop = false;
            this.OrderLayersButton.Text = "Order layers";
            this.mMainToolTip.SetToolTip(this.OrderLayersButton, "Allows you to customize the order the slide decorations are drawn in");
            this.OrderLayersButton.UseVisualStyleBackColor = true;
            // 
            // label18
            // 
            this.label18.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label18.Location = new System.Drawing.Point(198, 320);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(65, 24);
            this.label18.TabIndex = 64;
            this.label18.Text = "Rotation";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mEffectsEditorButton
            // 
            this.mEffectsEditorButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mEffectsEditorButton.Location = new System.Drawing.Point(7, 283);
            this.mEffectsEditorButton.Name = "mEffectsEditorButton";
            this.mEffectsEditorButton.Size = new System.Drawing.Size(149, 23);
            this.mEffectsEditorButton.TabIndex = 42;
            this.mEffectsEditorButton.TabStop = false;
            this.mEffectsEditorButton.Text = "Select motion effects";
            this.mMainToolTip.SetToolTip(this.mEffectsEditorButton, "Customize a motion effect on the elected decoration");
            this.mEffectsEditorButton.UseVisualStyleBackColor = true;
            // 
            // ClipartTransparencyLabel
            // 
            this.ClipartTransparencyLabel.BackColor = System.Drawing.Color.Transparent;
            this.ClipartTransparencyLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClipartTransparencyLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ClipartTransparencyLabel.Location = new System.Drawing.Point(403, 154);
            this.ClipartTransparencyLabel.Name = "ClipartTransparencyLabel";
            this.ClipartTransparencyLabel.Size = new System.Drawing.Size(39, 24);
            this.ClipartTransparencyLabel.TabIndex = 24;
            this.ClipartTransparencyLabel.Text = "0%";
            this.ClipartTransparencyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DecorationTabOptions
            // 
            this.DecorationTabOptions.AccessibleName = "";
            this.DecorationTabOptions.Controls.Add(this.AddTextTab);
            this.DecorationTabOptions.Controls.Add(this.ClipArtTab);
            this.DecorationTabOptions.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DecorationTabOptions.Location = new System.Drawing.Point(3, 13);
            this.DecorationTabOptions.Name = "DecorationTabOptions";
            this.DecorationTabOptions.Padding = new System.Drawing.Point(6, 5);
            this.DecorationTabOptions.SelectedIndex = 0;
            this.DecorationTabOptions.Size = new System.Drawing.Size(427, 130);
            this.DecorationTabOptions.TabIndex = 29;
            // 
            // AddTextTab
            // 
            this.AddTextTab.BackColor = System.Drawing.Color.White;
            this.AddTextTab.Controls.Add(this.mFontPreviewPictureBox);
            this.AddTextTab.Controls.Add(this.ColourPickerBackPlane);
            this.AddTextTab.Controls.Add(this.BackPlaneCheckbox);
            this.AddTextTab.Controls.Add(this.mTextTemplateTypeLabel);
            this.AddTextTab.Controls.Add(this.BackPlaneTransparencyTrackBar);
            this.AddTextTab.Controls.Add(this.mTextTemplateTypeCombo);
            this.AddTextTab.Controls.Add(this.BackPlaneTransparencyLabel);
            this.AddTextTab.Controls.Add(this.mFontButton);
            this.AddTextTab.Controls.Add(this.mEditTextButton);
            this.AddTextTab.Controls.Add(this.RightAlignedRadioBox);
            this.AddTextTab.Controls.Add(this.SizeComboBox);
            this.AddTextTab.Controls.Add(this.AddTextButton);
            this.AddTextTab.Controls.Add(this.CentreAlignedRadioBox);
            this.AddTextTab.Controls.Add(this.LeftAlignedRadioBox);
            this.AddTextTab.Location = new System.Drawing.Point(4, 26);
            this.AddTextTab.Name = "AddTextTab";
            this.AddTextTab.Padding = new System.Windows.Forms.Padding(3);
            this.AddTextTab.Size = new System.Drawing.Size(419, 100);
            this.AddTextTab.TabIndex = 0;
            this.AddTextTab.Text = "Text";
            this.AddTextTab.Click += new System.EventHandler(this.AddTextTab_Click);
            // 
            // mFontPreviewPictureBox
            // 
            this.mFontPreviewPictureBox.Location = new System.Drawing.Point(297, 39);
            this.mFontPreviewPictureBox.Name = "mFontPreviewPictureBox";
            this.mFontPreviewPictureBox.Size = new System.Drawing.Size(100, 24);
            this.mFontPreviewPictureBox.TabIndex = 33;
            this.mFontPreviewPictureBox.TabStop = false;
            // 
            // ColourPickerBackPlane
            // 
            this.ColourPickerBackPlane.BackColor = System.Drawing.SystemColors.ControlText;
            this.ColourPickerBackPlane.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ColourPickerBackPlane.Location = new System.Drawing.Point(98, 73);
            this.ColourPickerBackPlane.Name = "ColourPickerBackPlane";
            this.ColourPickerBackPlane.Size = new System.Drawing.Size(20, 20);
            this.ColourPickerBackPlane.TabIndex = 14;
            this.ColourPickerBackPlane.TabStop = false;
            this.ColourPickerBackPlane.UseVisualStyleBackColor = false;
            this.ColourPickerBackPlane.Click += new System.EventHandler(this.ColourPickerBackPlane_Click);
            // 
            // BackPlaneCheckbox
            // 
            this.BackPlaneCheckbox.BackColor = System.Drawing.Color.White;
            this.BackPlaneCheckbox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BackPlaneCheckbox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.BackPlaneCheckbox.Location = new System.Drawing.Point(8, 69);
            this.BackPlaneCheckbox.Name = "BackPlaneCheckbox";
            this.BackPlaneCheckbox.Size = new System.Drawing.Size(95, 32);
            this.BackPlaneCheckbox.TabIndex = 8;
            this.BackPlaneCheckbox.TabStop = false;
            this.BackPlaneCheckbox.Text = "Back plane";
            this.BackPlaneCheckbox.UseVisualStyleBackColor = false;
            this.BackPlaneCheckbox.CheckedChanged += new System.EventHandler(this.BackPlaneCheckbox_CheckedChanged);
            // 
            // mTextTemplateTypeLabel
            // 
            this.mTextTemplateTypeLabel.AutoSize = true;
            this.mTextTemplateTypeLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTextTemplateTypeLabel.ForeColor = System.Drawing.Color.Fuchsia;
            this.mTextTemplateTypeLabel.Location = new System.Drawing.Point(195, 10);
            this.mTextTemplateTypeLabel.Name = "mTextTemplateTypeLabel";
            this.mTextTemplateTypeLabel.Size = new System.Drawing.Size(99, 13);
            this.mTextTemplateTypeLabel.TabIndex = 32;
            this.mTextTemplateTypeLabel.Text = "Text template type";
            this.mTextTemplateTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mTextTemplateTypeLabel.Visible = false;
            // 
            // BackPlaneTransparencyTrackBar
            // 
            this.BackPlaneTransparencyTrackBar.BackColor = System.Drawing.Color.White;
            this.BackPlaneTransparencyTrackBar.Location = new System.Drawing.Point(119, 71);
            this.BackPlaneTransparencyTrackBar.Maximum = 100;
            this.BackPlaneTransparencyTrackBar.Name = "BackPlaneTransparencyTrackBar";
            this.BackPlaneTransparencyTrackBar.Size = new System.Drawing.Size(104, 45);
            this.BackPlaneTransparencyTrackBar.TabIndex = 15;
            this.BackPlaneTransparencyTrackBar.TabStop = false;
            this.BackPlaneTransparencyTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.BackPlaneTransparencyTrackBar.Value = 50;
            // 
            // mTextTemplateTypeCombo
            // 
            this.mTextTemplateTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mTextTemplateTypeCombo.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTextTemplateTypeCombo.ForeColor = System.Drawing.Color.Fuchsia;
            this.mTextTemplateTypeCombo.FormattingEnabled = true;
            this.mTextTemplateTypeCombo.Items.AddRange(new object[] {
            "None",
            "Single Line",
            "Multi Line"});
            this.mTextTemplateTypeCombo.Location = new System.Drawing.Point(324, 10);
            this.mTextTemplateTypeCombo.Name = "mTextTemplateTypeCombo";
            this.mTextTemplateTypeCombo.Size = new System.Drawing.Size(71, 21);
            this.mTextTemplateTypeCombo.TabIndex = 31;
            this.mTextTemplateTypeCombo.TabStop = false;
            this.mTextTemplateTypeCombo.Visible = false;
            // 
            // BackPlaneTransparencyLabel
            // 
            this.BackPlaneTransparencyLabel.BackColor = System.Drawing.Color.Transparent;
            this.BackPlaneTransparencyLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BackPlaneTransparencyLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.BackPlaneTransparencyLabel.Location = new System.Drawing.Point(219, 72);
            this.BackPlaneTransparencyLabel.Name = "BackPlaneTransparencyLabel";
            this.BackPlaneTransparencyLabel.Size = new System.Drawing.Size(40, 24);
            this.BackPlaneTransparencyLabel.TabIndex = 26;
            this.BackPlaneTransparencyLabel.Text = "0%";
            this.BackPlaneTransparencyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mFontButton
            // 
            this.mFontButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mFontButton.Location = new System.Drawing.Point(144, 39);
            this.mFontButton.Name = "mFontButton";
            this.mFontButton.Size = new System.Drawing.Size(72, 23);
            this.mFontButton.TabIndex = 29;
            this.mFontButton.TabStop = false;
            this.mFontButton.Text = "Font";
            this.mMainToolTip.SetToolTip(this.mFontButton, "Change the font");
            this.mFontButton.UseVisualStyleBackColor = true;
            // 
            // mEditTextButton
            // 
            this.mEditTextButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mEditTextButton.Location = new System.Drawing.Point(7, 40);
            this.mEditTextButton.Name = "mEditTextButton";
            this.mEditTextButton.Size = new System.Drawing.Size(75, 23);
            this.mEditTextButton.TabIndex = 28;
            this.mEditTextButton.TabStop = false;
            this.mEditTextButton.Text = "Edit text";
            this.mMainToolTip.SetToolTip(this.mEditTextButton, "Edit the selected text");
            this.mEditTextButton.UseVisualStyleBackColor = true;
            // 
            // RightAlignedRadioBox
            // 
            this.RightAlignedRadioBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.RightAlignedRadioBox.BackColor = System.Drawing.Color.Transparent;
            this.RightAlignedRadioBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("RightAlignedRadioBox.BackgroundImage")));
            this.RightAlignedRadioBox.Location = new System.Drawing.Point(266, 39);
            this.RightAlignedRadioBox.Name = "RightAlignedRadioBox";
            this.RightAlignedRadioBox.Size = new System.Drawing.Size(24, 24);
            this.RightAlignedRadioBox.TabIndex = 24;
            this.RightAlignedRadioBox.UseVisualStyleBackColor = false;
            this.RightAlignedRadioBox.CheckedChanged += new System.EventHandler(this.RightAlignedRadioBox_CheckedChanged);
            // 
            // SizeComboBox
            // 
            this.SizeComboBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SizeComboBox.Location = new System.Drawing.Point(90, 40);
            this.SizeComboBox.Name = "SizeComboBox";
            this.SizeComboBox.Size = new System.Drawing.Size(48, 21);
            this.SizeComboBox.TabIndex = 18;
            this.SizeComboBox.TabStop = false;
            // 
            // AddTextButton
            // 
            this.AddTextButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddTextButton.Location = new System.Drawing.Point(6, 11);
            this.AddTextButton.Name = "AddTextButton";
            this.AddTextButton.Size = new System.Drawing.Size(75, 23);
            this.AddTextButton.TabIndex = 4;
            this.AddTextButton.TabStop = false;
            this.AddTextButton.Text = "Add text";
            this.mMainToolTip.SetToolTip(this.AddTextButton, "Adds text to the current slide.  You can also double left click in the preview wi" +
        "ndow");
            this.AddTextButton.UseVisualStyleBackColor = true;
            this.AddTextButton.Click += new System.EventHandler(this.AddTextButton_Click);
            // 
            // CentreAlignedRadioBox
            // 
            this.CentreAlignedRadioBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.CentreAlignedRadioBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("CentreAlignedRadioBox.BackgroundImage")));
            this.CentreAlignedRadioBox.Location = new System.Drawing.Point(242, 39);
            this.CentreAlignedRadioBox.Name = "CentreAlignedRadioBox";
            this.CentreAlignedRadioBox.Size = new System.Drawing.Size(24, 24);
            this.CentreAlignedRadioBox.TabIndex = 23;
            this.CentreAlignedRadioBox.CheckedChanged += new System.EventHandler(this.CentreAlignedRadioBox_CheckedChanged);
            // 
            // LeftAlignedRadioBox
            // 
            this.LeftAlignedRadioBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.LeftAlignedRadioBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("LeftAlignedRadioBox.BackgroundImage")));
            this.LeftAlignedRadioBox.Checked = true;
            this.LeftAlignedRadioBox.Location = new System.Drawing.Point(218, 39);
            this.LeftAlignedRadioBox.Name = "LeftAlignedRadioBox";
            this.LeftAlignedRadioBox.Size = new System.Drawing.Size(24, 24);
            this.LeftAlignedRadioBox.TabIndex = 22;
            this.LeftAlignedRadioBox.TabStop = true;
            this.LeftAlignedRadioBox.CheckedChanged += new System.EventHandler(this.LeftAlignedRadioBox_CheckedChanged);
            // 
            // ClipArtTab
            // 
            this.ClipArtTab.BackColor = System.Drawing.Color.White;
            this.ClipArtTab.Controls.Add(this.mTemplateFrameworkImageCheckbox);
            this.ClipArtTab.Controls.Add(this.mTemplateImageNumberCombo);
            this.ClipArtTab.Controls.Add(this.mTemplateAspectCombo);
            this.ClipArtTab.Controls.Add(this.AddClipArtButton);
            this.ClipArtTab.Controls.Add(this.EditImageButton);
            this.ClipArtTab.Controls.Add(this.mRenderMethodCombo);
            this.ClipArtTab.Controls.Add(this.mClipArtLockAspectCheckBox);
            this.ClipArtTab.Controls.Add(this.AddNewImageButton);
            this.ClipArtTab.Controls.Add(this.RotateDecorationClockWiseButton);
            this.ClipArtTab.Controls.Add(this.RotateDecorationAntiClockWiseButton);
            this.ClipArtTab.Controls.Add(this.FlipDecorationLeftRightButton);
            this.ClipArtTab.Controls.Add(this.FlipDecorationUpDownButton);
            this.ClipArtTab.Location = new System.Drawing.Point(4, 26);
            this.ClipArtTab.Name = "ClipArtTab";
            this.ClipArtTab.Padding = new System.Windows.Forms.Padding(3);
            this.ClipArtTab.Size = new System.Drawing.Size(419, 100);
            this.ClipArtTab.TabIndex = 1;
            this.ClipArtTab.Text = "Images / video";
            // 
            // mTemplateFrameworkImageCheckbox
            // 
            this.mTemplateFrameworkImageCheckbox.AutoSize = true;
            this.mTemplateFrameworkImageCheckbox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTemplateFrameworkImageCheckbox.ForeColor = System.Drawing.Color.Fuchsia;
            this.mTemplateFrameworkImageCheckbox.Location = new System.Drawing.Point(249, 38);
            this.mTemplateFrameworkImageCheckbox.Name = "mTemplateFrameworkImageCheckbox";
            this.mTemplateFrameworkImageCheckbox.Size = new System.Drawing.Size(163, 17);
            this.mTemplateFrameworkImageCheckbox.TabIndex = 29;
            this.mTemplateFrameworkImageCheckbox.TabStop = false;
            this.mTemplateFrameworkImageCheckbox.Text = "Template framework image";
            this.mTemplateFrameworkImageCheckbox.UseVisualStyleBackColor = true;
            this.mTemplateFrameworkImageCheckbox.Visible = false;
            // 
            // mTemplateImageNumberCombo
            // 
            this.mTemplateImageNumberCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mTemplateImageNumberCombo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.mTemplateImageNumberCombo.ForeColor = System.Drawing.Color.Fuchsia;
            this.mTemplateImageNumberCombo.FormattingEnabled = true;
            this.mTemplateImageNumberCombo.Items.AddRange(new object[] {
            "None",
            "IMAGE1",
            "IMAGE2",
            "IMAGE3",
            "IMAGE4",
            "IMAGE5",
            "IMAGE6",
            "IMAGE7",
            "IMAGE8",
            "IMAGE9",
            "IMAGE10",
            "IMAGE11",
            "IMAGE12",
            "IMAGE13",
            "IMAGE14",
            "IMAGE15",
            "IMAGE16",
            "IMAGE17",
            "IMAGE18",
            "IMAGE19",
            "IMAGE20",
            "MASK"});
            this.mTemplateImageNumberCombo.Location = new System.Drawing.Point(205, 8);
            this.mTemplateImageNumberCombo.Name = "mTemplateImageNumberCombo";
            this.mTemplateImageNumberCombo.Size = new System.Drawing.Size(81, 21);
            this.mTemplateImageNumberCombo.TabIndex = 28;
            this.mTemplateImageNumberCombo.TabStop = false;
            this.mTemplateImageNumberCombo.Visible = false;
            // 
            // mTemplateAspectCombo
            // 
            this.mTemplateAspectCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mTemplateAspectCombo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.mTemplateAspectCombo.ForeColor = System.Drawing.Color.Fuchsia;
            this.mTemplateAspectCombo.FormattingEnabled = true;
            this.mTemplateAspectCombo.Items.AddRange(new object[] {
            "Strech to fill",
            "Legacy do no use",
            "Zoom to Fill",
            "Fit in Frame"});
            this.mTemplateAspectCombo.Location = new System.Drawing.Point(157, 36);
            this.mTemplateAspectCombo.Name = "mTemplateAspectCombo";
            this.mTemplateAspectCombo.Size = new System.Drawing.Size(82, 21);
            this.mTemplateAspectCombo.TabIndex = 27;
            this.mTemplateAspectCombo.TabStop = false;
            this.mTemplateAspectCombo.Visible = false;
            // 
            // AddClipArtButton
            // 
            this.AddClipArtButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddClipArtButton.Location = new System.Drawing.Point(6, 40);
            this.AddClipArtButton.Name = "AddClipArtButton";
            this.AddClipArtButton.Size = new System.Drawing.Size(132, 23);
            this.AddClipArtButton.TabIndex = 26;
            this.AddClipArtButton.TabStop = false;
            this.AddClipArtButton.Text = "Add clipart";
            this.mMainToolTip.SetToolTip(this.AddClipArtButton, "Add an image from our clipart collection");
            this.AddClipArtButton.UseVisualStyleBackColor = true;
            // 
            // EditImageButton
            // 
            this.EditImageButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EditImageButton.Location = new System.Drawing.Point(6, 69);
            this.EditImageButton.Name = "EditImageButton";
            this.EditImageButton.Size = new System.Drawing.Size(132, 23);
            this.EditImageButton.TabIndex = 24;
            this.EditImageButton.TabStop = false;
            this.EditImageButton.Text = "Edit image";
            this.mMainToolTip.SetToolTip(this.EditImageButton, "Edit the selected image");
            this.EditImageButton.UseVisualStyleBackColor = true;
            // 
            // mRenderMethodCombo
            // 
            this.mRenderMethodCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mRenderMethodCombo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.mRenderMethodCombo.ForeColor = System.Drawing.Color.Fuchsia;
            this.mRenderMethodCombo.FormattingEnabled = true;
            this.mRenderMethodCombo.Location = new System.Drawing.Point(292, 8);
            this.mRenderMethodCombo.Name = "mRenderMethodCombo";
            this.mRenderMethodCombo.Size = new System.Drawing.Size(121, 21);
            this.mRenderMethodCombo.TabIndex = 17;
            this.mRenderMethodCombo.TabStop = false;
            this.mRenderMethodCombo.Visible = false;
            // 
            // mClipArtLockAspectCheckBox
            // 
            this.mClipArtLockAspectCheckBox.AutoSize = true;
            this.mClipArtLockAspectCheckBox.Checked = true;
            this.mClipArtLockAspectCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mClipArtLockAspectCheckBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mClipArtLockAspectCheckBox.Location = new System.Drawing.Point(245, 71);
            this.mClipArtLockAspectCheckBox.Name = "mClipArtLockAspectCheckBox";
            this.mClipArtLockAspectCheckBox.Size = new System.Drawing.Size(83, 17);
            this.mClipArtLockAspectCheckBox.TabIndex = 18;
            this.mClipArtLockAspectCheckBox.TabStop = false;
            this.mClipArtLockAspectCheckBox.Text = "lock aspect";
            this.mClipArtLockAspectCheckBox.UseVisualStyleBackColor = true;
            // 
            // AddNewImageButton
            // 
            this.AddNewImageButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddNewImageButton.Location = new System.Drawing.Point(6, 12);
            this.AddNewImageButton.Name = "AddNewImageButton";
            this.AddNewImageButton.Size = new System.Drawing.Size(132, 23);
            this.AddNewImageButton.TabIndex = 16;
            this.AddNewImageButton.TabStop = false;
            this.AddNewImageButton.Text = "Add image / video";
            this.mMainToolTip.SetToolTip(this.AddNewImageButton, "Browse for a picture or video and add it to the slide as a new decoration");
            this.AddNewImageButton.UseVisualStyleBackColor = true;
            // 
            // RotateDecorationClockWiseButton
            // 
            this.RotateDecorationClockWiseButton.BackColor = System.Drawing.Color.Transparent;
            this.RotateDecorationClockWiseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.RotateDecorationClockWiseButton.Image = global::dvdslideshowfontend.Properties.Resources.Actions_edit_redo_icon_blue_small;
            this.RotateDecorationClockWiseButton.Location = new System.Drawing.Point(165, 68);
            this.RotateDecorationClockWiseButton.Name = "RotateDecorationClockWiseButton";
            this.RotateDecorationClockWiseButton.Size = new System.Drawing.Size(24, 24);
            this.RotateDecorationClockWiseButton.TabIndex = 21;
            this.RotateDecorationClockWiseButton.TabStop = false;
            this.RotateDecorationClockWiseButton.UseVisualStyleBackColor = false;
            // 
            // RotateDecorationAntiClockWiseButton
            // 
            this.RotateDecorationAntiClockWiseButton.BackColor = System.Drawing.Color.Transparent;
            this.RotateDecorationAntiClockWiseButton.Image = ((System.Drawing.Image)(resources.GetObject("RotateDecorationAntiClockWiseButton.Image")));
            this.RotateDecorationAntiClockWiseButton.Location = new System.Drawing.Point(141, 68);
            this.RotateDecorationAntiClockWiseButton.Name = "RotateDecorationAntiClockWiseButton";
            this.RotateDecorationAntiClockWiseButton.Size = new System.Drawing.Size(24, 24);
            this.RotateDecorationAntiClockWiseButton.TabIndex = 20;
            this.RotateDecorationAntiClockWiseButton.TabStop = false;
            this.RotateDecorationAntiClockWiseButton.UseVisualStyleBackColor = false;
            // 
            // FlipDecorationLeftRightButton
            // 
            this.FlipDecorationLeftRightButton.BackColor = System.Drawing.Color.Transparent;
            this.FlipDecorationLeftRightButton.Image = ((System.Drawing.Image)(resources.GetObject("FlipDecorationLeftRightButton.Image")));
            this.FlipDecorationLeftRightButton.Location = new System.Drawing.Point(189, 68);
            this.FlipDecorationLeftRightButton.Name = "FlipDecorationLeftRightButton";
            this.FlipDecorationLeftRightButton.Size = new System.Drawing.Size(24, 24);
            this.FlipDecorationLeftRightButton.TabIndex = 23;
            this.FlipDecorationLeftRightButton.TabStop = false;
            this.FlipDecorationLeftRightButton.UseVisualStyleBackColor = false;
            // 
            // FlipDecorationUpDownButton
            // 
            this.FlipDecorationUpDownButton.BackColor = System.Drawing.Color.Transparent;
            this.FlipDecorationUpDownButton.Image = ((System.Drawing.Image)(resources.GetObject("FlipDecorationUpDownButton.Image")));
            this.FlipDecorationUpDownButton.Location = new System.Drawing.Point(213, 68);
            this.FlipDecorationUpDownButton.Name = "FlipDecorationUpDownButton";
            this.FlipDecorationUpDownButton.Size = new System.Drawing.Size(24, 24);
            this.FlipDecorationUpDownButton.TabIndex = 22;
            this.FlipDecorationUpDownButton.TabStop = false;
            this.FlipDecorationUpDownButton.UseVisualStyleBackColor = false;
            // 
            // mDecorationRotationTrackbar
            // 
            this.mDecorationRotationTrackbar.BackColor = System.Drawing.Color.White;
            this.mDecorationRotationTrackbar.Location = new System.Drawing.Point(260, 324);
            this.mDecorationRotationTrackbar.Maximum = 180;
            this.mDecorationRotationTrackbar.Minimum = -180;
            this.mDecorationRotationTrackbar.Name = "mDecorationRotationTrackbar";
            this.mDecorationRotationTrackbar.Size = new System.Drawing.Size(121, 45);
            this.mDecorationRotationTrackbar.TabIndex = 63;
            this.mDecorationRotationTrackbar.TabStop = false;
            this.mDecorationRotationTrackbar.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // Transparentlabel
            // 
            this.Transparentlabel.BackColor = System.Drawing.Color.Transparent;
            this.Transparentlabel.Location = new System.Drawing.Point(0, 0);
            this.Transparentlabel.Name = "Transparentlabel";
            this.Transparentlabel.Size = new System.Drawing.Size(11, 20);
            this.Transparentlabel.TabIndex = 28;
            // 
            // ClipPanTabPage
            // 
            this.ClipPanTabPage.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ClipPanTabPage.BackgroundImage")));
            this.ClipPanTabPage.Controls.Add(this.label17);
            this.ClipPanTabPage.Controls.Add(this.label20);
            this.ClipPanTabPage.Controls.Add(this.label5);
            this.ClipPanTabPage.Controls.Add(this.mPanZoomUseEquationComboBox);
            this.ClipPanTabPage.Controls.Add(this.mPanZoomInitialDelayTrackBar);
            this.ClipPanTabPage.Controls.Add(this.mReCalcPanZoomTemplateCheckbox);
            this.ClipPanTabPage.Controls.Add(this.mPanZoomInitialDelayLabel);
            this.ClipPanTabPage.Controls.Add(this.EndRotationTrackBarLabel);
            this.ClipPanTabPage.Controls.Add(this.StartRotationTrackBarLabel);
            this.ClipPanTabPage.Controls.Add(this.StartPanTrackBarLabel);
            this.ClipPanTabPage.Controls.Add(this.EndPanTrackBarLabel);
            this.ClipPanTabPage.Controls.Add(this.label9);
            this.ClipPanTabPage.Controls.Add(this.mPreviewPanZoomPictureBox);
            this.ClipPanTabPage.Controls.Add(this.panel6);
            this.ClipPanTabPage.Controls.Add(this.EndPanRotationTrackBar);
            this.ClipPanTabPage.Controls.Add(this.EndPanTrackBar);
            this.ClipPanTabPage.Controls.Add(this.panel7);
            this.ClipPanTabPage.Controls.Add(this.StartPanRotationTrackBar);
            this.ClipPanTabPage.Controls.Add(this.mPanZoomOnAllCheckBox);
            this.ClipPanTabPage.Controls.Add(this.mStartPanDelayLabel);
            this.ClipPanTabPage.Controls.Add(this.mPanZoomLinkStartAreaButton);
            this.ClipPanTabPage.Controls.Add(this.StartPanTrackBar);
            this.ClipPanTabPage.Controls.Add(this.label8);
            this.ClipPanTabPage.Controls.Add(this.mEndPanDelayLabel);
            this.ClipPanTabPage.Controls.Add(this.mForceEnlargeLabel2);
            this.ClipPanTabPage.Controls.Add(this.mPanZoomEndDelayLabel);
            this.ClipPanTabPage.Controls.Add(this.mPanZoomEndDelayTrackBar);
            this.ClipPanTabPage.Controls.Add(this.mTurnOffPanZoomForSlideTickBox);
            this.ClipPanTabPage.Controls.Add(this.RandomPanZoomButton);
            this.ClipPanTabPage.ImageIndex = 3;
            this.ClipPanTabPage.Location = new System.Drawing.Point(52, 4);
            this.ClipPanTabPage.Name = "ClipPanTabPage";
            this.ClipPanTabPage.Size = new System.Drawing.Size(494, 1017);
            this.ClipPanTabPage.TabIndex = 1;
            this.ClipPanTabPage.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            this.label17.BackColor = System.Drawing.Color.Transparent;
            this.label17.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label17.Image = ((System.Drawing.Image)(resources.GetObject("label17.Image")));
            this.label17.Location = new System.Drawing.Point(6, 29);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(70, 26);
            this.label17.TabIndex = 35;
            this.label17.Text = "Rotation";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label17.Click += new System.EventHandler(this.label17_Click);
            // 
            // label20
            // 
            this.label20.BackColor = System.Drawing.Color.Transparent;
            this.label20.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label20.Image = ((System.Drawing.Image)(resources.GetObject("label20.Image")));
            this.label20.Location = new System.Drawing.Point(6, 92);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(70, 26);
            this.label20.TabIndex = 39;
            this.label20.Text = "Rotation";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label5.Location = new System.Drawing.Point(188, 231);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 25);
            this.label5.TabIndex = 43;
            this.label5.Text = "Movement";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // mPanZoomUseEquationComboBox
            // 
            this.mPanZoomUseEquationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mPanZoomUseEquationComboBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mPanZoomUseEquationComboBox.FormattingEnabled = true;
            this.mPanZoomUseEquationComboBox.Items.AddRange(new object[] {
            "Normal (linear)",
            "Smoothed (non linear)",
            "Spring",
            "Quick slow"});
            this.mPanZoomUseEquationComboBox.Location = new System.Drawing.Point(291, 234);
            this.mPanZoomUseEquationComboBox.Name = "mPanZoomUseEquationComboBox";
            this.mPanZoomUseEquationComboBox.Size = new System.Drawing.Size(130, 21);
            this.mPanZoomUseEquationComboBox.TabIndex = 42;
            this.mPanZoomUseEquationComboBox.TabStop = false;
            // 
            // mPanZoomInitialDelayTrackBar
            // 
            this.mPanZoomInitialDelayTrackBar.AutoSize = false;
            this.mPanZoomInitialDelayTrackBar.BackColor = System.Drawing.Color.White;
            this.mPanZoomInitialDelayTrackBar.Location = new System.Drawing.Point(246, 139);
            this.mPanZoomInitialDelayTrackBar.Maximum = 100;
            this.mPanZoomInitialDelayTrackBar.Name = "mPanZoomInitialDelayTrackBar";
            this.mPanZoomInitialDelayTrackBar.Size = new System.Drawing.Size(139, 45);
            this.mPanZoomInitialDelayTrackBar.TabIndex = 18;
            this.mPanZoomInitialDelayTrackBar.TabStop = false;
            this.mPanZoomInitialDelayTrackBar.TickFrequency = 10;
            this.mPanZoomInitialDelayTrackBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            // 
            // mReCalcPanZoomTemplateCheckbox
            // 
            this.mReCalcPanZoomTemplateCheckbox.AutoSize = true;
            this.mReCalcPanZoomTemplateCheckbox.ForeColor = System.Drawing.Color.Fuchsia;
            this.mReCalcPanZoomTemplateCheckbox.Location = new System.Drawing.Point(250, 373);
            this.mReCalcPanZoomTemplateCheckbox.Name = "mReCalcPanZoomTemplateCheckbox";
            this.mReCalcPanZoomTemplateCheckbox.Size = new System.Drawing.Size(144, 17);
            this.mReCalcPanZoomTemplateCheckbox.TabIndex = 41;
            this.mReCalcPanZoomTemplateCheckbox.Text = "Re-calc on template load";
            this.mReCalcPanZoomTemplateCheckbox.UseVisualStyleBackColor = true;
            this.mReCalcPanZoomTemplateCheckbox.Visible = false;
            // 
            // mPanZoomInitialDelayLabel
            // 
            this.mPanZoomInitialDelayLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mPanZoomInitialDelayLabel.Location = new System.Drawing.Point(180, 138);
            this.mPanZoomInitialDelayLabel.Name = "mPanZoomInitialDelayLabel";
            this.mPanZoomInitialDelayLabel.Size = new System.Drawing.Size(70, 45);
            this.mPanZoomInitialDelayLabel.TabIndex = 19;
            this.mPanZoomInitialDelayLabel.Text = "Start pan time";
            this.mPanZoomInitialDelayLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.mPanZoomInitialDelayLabel.Click += new System.EventHandler(this.mPanZoomInitialDelayLabel_Click);
            // 
            // EndRotationTrackBarLabel
            // 
            this.EndRotationTrackBarLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EndRotationTrackBarLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.EndRotationTrackBarLabel.Location = new System.Drawing.Point(391, 98);
            this.EndRotationTrackBarLabel.Name = "EndRotationTrackBarLabel";
            this.EndRotationTrackBarLabel.Size = new System.Drawing.Size(37, 23);
            this.EndRotationTrackBarLabel.TabIndex = 38;
            this.EndRotationTrackBarLabel.Text = "label7";
            // 
            // StartRotationTrackBarLabel
            // 
            this.StartRotationTrackBarLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartRotationTrackBarLabel.Location = new System.Drawing.Point(391, 32);
            this.StartRotationTrackBarLabel.Name = "StartRotationTrackBarLabel";
            this.StartRotationTrackBarLabel.Size = new System.Drawing.Size(37, 23);
            this.StartRotationTrackBarLabel.TabIndex = 34;
            this.StartRotationTrackBarLabel.Text = "label3";
            // 
            // StartPanTrackBarLabel
            // 
            this.StartPanTrackBarLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartPanTrackBarLabel.Location = new System.Drawing.Point(391, 6);
            this.StartPanTrackBarLabel.Name = "StartPanTrackBarLabel";
            this.StartPanTrackBarLabel.Size = new System.Drawing.Size(37, 23);
            this.StartPanTrackBarLabel.TabIndex = 11;
            this.StartPanTrackBarLabel.Text = "label3";
            // 
            // EndPanTrackBarLabel
            // 
            this.EndPanTrackBarLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EndPanTrackBarLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.EndPanTrackBarLabel.Location = new System.Drawing.Point(391, 70);
            this.EndPanTrackBarLabel.Name = "EndPanTrackBarLabel";
            this.EndPanTrackBarLabel.Size = new System.Drawing.Size(37, 23);
            this.EndPanTrackBarLabel.TabIndex = 12;
            this.EndPanTrackBarLabel.Text = "label7";
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label9.Image = ((System.Drawing.Image)(resources.GetObject("label9.Image")));
            this.label9.Location = new System.Drawing.Point(0, 67);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 26);
            this.label9.TabIndex = 8;
            this.label9.Text = "End pan";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mPreviewPanZoomPictureBox
            // 
            this.mPreviewPanZoomPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mPreviewPanZoomPictureBox.Location = new System.Drawing.Point(6, 129);
            this.mPreviewPanZoomPictureBox.Name = "mPreviewPanZoomPictureBox";
            this.mPreviewPanZoomPictureBox.Size = new System.Drawing.Size(168, 126);
            this.mPreviewPanZoomPictureBox.TabIndex = 17;
            this.mPreviewPanZoomPictureBox.TabStop = false;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.Transparent;
            this.panel6.Location = new System.Drawing.Point(76, 118);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(293, 30);
            this.panel6.TabIndex = 15;
            // 
            // EndPanRotationTrackBar
            // 
            this.EndPanRotationTrackBar.AutoSize = false;
            this.EndPanRotationTrackBar.BackColor = System.Drawing.Color.White;
            this.EndPanRotationTrackBar.Location = new System.Drawing.Point(76, 95);
            this.EndPanRotationTrackBar.Maximum = 360;
            this.EndPanRotationTrackBar.Minimum = -360;
            this.EndPanRotationTrackBar.Name = "EndPanRotationTrackBar";
            this.EndPanRotationTrackBar.Size = new System.Drawing.Size(320, 45);
            this.EndPanRotationTrackBar.TabIndex = 37;
            this.EndPanRotationTrackBar.TabStop = false;
            this.EndPanRotationTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // EndPanTrackBar
            // 
            this.EndPanTrackBar.AutoSize = false;
            this.EndPanTrackBar.BackColor = System.Drawing.Color.White;
            this.EndPanTrackBar.Location = new System.Drawing.Point(76, 67);
            this.EndPanTrackBar.Maximum = 100;
            this.EndPanTrackBar.Minimum = 10;
            this.EndPanTrackBar.Name = "EndPanTrackBar";
            this.EndPanTrackBar.Size = new System.Drawing.Size(320, 45);
            this.EndPanTrackBar.TabIndex = 6;
            this.EndPanTrackBar.TabStop = false;
            this.EndPanTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.EndPanTrackBar.Value = 10;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.Transparent;
            this.panel7.Location = new System.Drawing.Point(69, 55);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(343, 30);
            this.panel7.TabIndex = 36;
            // 
            // StartPanRotationTrackBar
            // 
            this.StartPanRotationTrackBar.BackColor = System.Drawing.Color.White;
            this.StartPanRotationTrackBar.Location = new System.Drawing.Point(76, 32);
            this.StartPanRotationTrackBar.Maximum = 360;
            this.StartPanRotationTrackBar.Minimum = -360;
            this.StartPanRotationTrackBar.Name = "StartPanRotationTrackBar";
            this.StartPanRotationTrackBar.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPanRotationTrackBar.Size = new System.Drawing.Size(320, 45);
            this.StartPanRotationTrackBar.TabIndex = 33;
            this.StartPanRotationTrackBar.TabStop = false;
            this.StartPanRotationTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // mPanZoomOnAllCheckBox
            // 
            this.mPanZoomOnAllCheckBox.AutoSize = true;
            this.mPanZoomOnAllCheckBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mPanZoomOnAllCheckBox.Location = new System.Drawing.Point(6, 304);
            this.mPanZoomOnAllCheckBox.Name = "mPanZoomOnAllCheckBox";
            this.mPanZoomOnAllCheckBox.Size = new System.Drawing.Size(200, 17);
            this.mPanZoomOnAllCheckBox.TabIndex = 40;
            this.mPanZoomOnAllCheckBox.TabStop = false;
            this.mPanZoomOnAllCheckBox.Text = "Pan/zoom on all slide decorations";
            this.mPanZoomOnAllCheckBox.UseVisualStyleBackColor = true;
            // 
            // mStartPanDelayLabel
            // 
            this.mStartPanDelayLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mStartPanDelayLabel.Location = new System.Drawing.Point(391, 145);
            this.mStartPanDelayLabel.Name = "mStartPanDelayLabel";
            this.mStartPanDelayLabel.Size = new System.Drawing.Size(50, 39);
            this.mStartPanDelayLabel.TabIndex = 28;
            this.mStartPanDelayLabel.Text = "label3";
            this.mStartPanDelayLabel.Click += new System.EventHandler(this.mStartPanDelayLabel_Click);
            // 
            // mPanZoomLinkStartAreaButton
            // 
            this.mPanZoomLinkStartAreaButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mPanZoomLinkStartAreaButton.Location = new System.Drawing.Point(241, 315);
            this.mPanZoomLinkStartAreaButton.Name = "mPanZoomLinkStartAreaButton";
            this.mPanZoomLinkStartAreaButton.Size = new System.Drawing.Size(187, 39);
            this.mPanZoomLinkStartAreaButton.TabIndex = 32;
            this.mPanZoomLinkStartAreaButton.TabStop = false;
            this.mPanZoomLinkStartAreaButton.Text = "Set start region to previous slide end region";
            this.mPanZoomLinkStartAreaButton.UseVisualStyleBackColor = true;
            // 
            // StartPanTrackBar
            // 
            this.StartPanTrackBar.BackColor = System.Drawing.Color.White;
            this.StartPanTrackBar.Location = new System.Drawing.Point(76, 3);
            this.StartPanTrackBar.Maximum = 100;
            this.StartPanTrackBar.Minimum = 10;
            this.StartPanTrackBar.Name = "StartPanTrackBar";
            this.StartPanTrackBar.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPanTrackBar.Size = new System.Drawing.Size(320, 45);
            this.StartPanTrackBar.TabIndex = 3;
            this.StartPanTrackBar.TabStop = false;
            this.StartPanTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.StartPanTrackBar.Value = 10;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label8.Image = ((System.Drawing.Image)(resources.GetObject("label8.Image")));
            this.label8.Location = new System.Drawing.Point(0, 3);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 26);
            this.label8.TabIndex = 5;
            this.label8.Text = "Start pan";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mEndPanDelayLabel
            // 
            this.mEndPanDelayLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mEndPanDelayLabel.Location = new System.Drawing.Point(391, 192);
            this.mEndPanDelayLabel.Name = "mEndPanDelayLabel";
            this.mEndPanDelayLabel.Size = new System.Drawing.Size(50, 41);
            this.mEndPanDelayLabel.TabIndex = 29;
            this.mEndPanDelayLabel.Text = "label3";
            // 
            // mForceEnlargeLabel2
            // 
            this.mForceEnlargeLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mForceEnlargeLabel2.AutoSize = true;
            this.mForceEnlargeLabel2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mForceEnlargeLabel2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mForceEnlargeLabel2.Location = new System.Drawing.Point(307, 993);
            this.mForceEnlargeLabel2.Name = "mForceEnlargeLabel2";
            this.mForceEnlargeLabel2.Size = new System.Drawing.Size(10, 14);
            this.mForceEnlargeLabel2.TabIndex = 31;
            this.mForceEnlargeLabel2.Text = " ";
            // 
            // mPanZoomEndDelayLabel
            // 
            this.mPanZoomEndDelayLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mPanZoomEndDelayLabel.Location = new System.Drawing.Point(184, 187);
            this.mPanZoomEndDelayLabel.Name = "mPanZoomEndDelayLabel";
            this.mPanZoomEndDelayLabel.Size = new System.Drawing.Size(65, 41);
            this.mPanZoomEndDelayLabel.TabIndex = 22;
            this.mPanZoomEndDelayLabel.Text = "End pan time";
            this.mPanZoomEndDelayLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mPanZoomEndDelayTrackBar
            // 
            this.mPanZoomEndDelayTrackBar.AutoSize = false;
            this.mPanZoomEndDelayTrackBar.BackColor = System.Drawing.Color.White;
            this.mPanZoomEndDelayTrackBar.Location = new System.Drawing.Point(250, 183);
            this.mPanZoomEndDelayTrackBar.Maximum = 100;
            this.mPanZoomEndDelayTrackBar.Name = "mPanZoomEndDelayTrackBar";
            this.mPanZoomEndDelayTrackBar.Size = new System.Drawing.Size(139, 45);
            this.mPanZoomEndDelayTrackBar.TabIndex = 21;
            this.mPanZoomEndDelayTrackBar.TabStop = false;
            this.mPanZoomEndDelayTrackBar.TickFrequency = 10;
            this.mPanZoomEndDelayTrackBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.mPanZoomEndDelayTrackBar.Value = 100;
            // 
            // mTurnOffPanZoomForSlideTickBox
            // 
            this.mTurnOffPanZoomForSlideTickBox.AutoSize = true;
            this.mTurnOffPanZoomForSlideTickBox.BackColor = System.Drawing.Color.Transparent;
            this.mTurnOffPanZoomForSlideTickBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTurnOffPanZoomForSlideTickBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mTurnOffPanZoomForSlideTickBox.Location = new System.Drawing.Point(6, 327);
            this.mTurnOffPanZoomForSlideTickBox.Name = "mTurnOffPanZoomForSlideTickBox";
            this.mTurnOffPanZoomForSlideTickBox.Size = new System.Drawing.Size(189, 17);
            this.mTurnOffPanZoomForSlideTickBox.TabIndex = 24;
            this.mTurnOffPanZoomForSlideTickBox.TabStop = false;
            this.mTurnOffPanZoomForSlideTickBox.Text = "Turn off pan/zoom for this slide";
            this.mTurnOffPanZoomForSlideTickBox.UseVisualStyleBackColor = false;
            // 
            // RandomPanZoomButton
            // 
            this.RandomPanZoomButton.BackColor = System.Drawing.SystemColors.Control;
            this.RandomPanZoomButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RandomPanZoomButton.ForeColor = System.Drawing.Color.Black;
            this.RandomPanZoomButton.Location = new System.Drawing.Point(3, 275);
            this.RandomPanZoomButton.Name = "RandomPanZoomButton";
            this.RandomPanZoomButton.Size = new System.Drawing.Size(180, 23);
            this.RandomPanZoomButton.TabIndex = 17;
            this.RandomPanZoomButton.TabStop = false;
            this.RandomPanZoomButton.Text = "Generates random pan/zoom";
            this.mMainToolTip.SetToolTip(this.RandomPanZoomButton, "Generates a new random pan zoom effect for the current slide");
            this.RandomPanZoomButton.UseVisualStyleBackColor = true;
            this.RandomPanZoomButton.Click += new System.EventHandler(this.RandomPanZoomButton_Click);
            // 
            // mFiltersTabPage
            // 
            this.mFiltersTabPage.Controls.Add(this.mFiltersControl);
            this.mFiltersTabPage.ImageIndex = 4;
            this.mFiltersTabPage.Location = new System.Drawing.Point(52, 4);
            this.mFiltersTabPage.Name = "mFiltersTabPage";
            this.mFiltersTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.mFiltersTabPage.Size = new System.Drawing.Size(494, 1017);
            this.mFiltersTabPage.TabIndex = 4;
            this.mFiltersTabPage.UseVisualStyleBackColor = true;
            // 
            // mFiltersControl
            // 
            this.mFiltersControl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mFiltersControl.BackgroundImage")));
            this.mFiltersControl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mFiltersControl.Location = new System.Drawing.Point(0, 0);
            this.mFiltersControl.Margin = new System.Windows.Forms.Padding(0);
            this.mFiltersControl.Name = "mFiltersControl";
            this.mFiltersControl.Size = new System.Drawing.Size(444, 1017);
            this.mFiltersControl.TabIndex = 0;
            // 
            // mBordersTab
            // 
            this.mBordersTab.Controls.Add(this.mOverlaySelectionControl1);
            this.mBordersTab.ImageIndex = 5;
            this.mBordersTab.Location = new System.Drawing.Point(52, 4);
            this.mBordersTab.Name = "mBordersTab";
            this.mBordersTab.Padding = new System.Windows.Forms.Padding(3);
            this.mBordersTab.Size = new System.Drawing.Size(494, 1017);
            this.mBordersTab.TabIndex = 3;
            this.mBordersTab.UseVisualStyleBackColor = true;
            // 
            // mOverlaySelectionControl1
            // 
            this.mOverlaySelectionControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mOverlaySelectionControl1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mOverlaySelectionControl1.BackgroundImage")));
            this.mOverlaySelectionControl1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mOverlaySelectionControl1.Location = new System.Drawing.Point(0, 0);
            this.mOverlaySelectionControl1.Name = "mOverlaySelectionControl1";
            this.mOverlaySelectionControl1.Size = new System.Drawing.Size(444, 1017);
            this.mOverlaySelectionControl1.TabIndex = 0;
            // 
            // DecoratePanZoomImageList
            // 
            this.DecoratePanZoomImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("DecoratePanZoomImageList.ImageStream")));
            this.DecoratePanZoomImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.DecoratePanZoomImageList.Images.SetKeyName(0, "design.png");
            this.DecoratePanZoomImageList.Images.SetKeyName(1, "background.png");
            this.DecoratePanZoomImageList.Images.SetKeyName(2, "textandimages.png");
            this.DecoratePanZoomImageList.Images.SetKeyName(3, "panandzoom.png");
            this.DecoratePanZoomImageList.Images.SetKeyName(4, "filters.png");
            this.DecoratePanZoomImageList.Images.SetKeyName(5, "border.png");
            // 
            // mIntroSlideshowTabPage
            // 
            this.mIntroSlideshowTabPage.Controls.Add(this.mIntoSlideshowTabControl);
            this.mIntroSlideshowTabPage.Location = new System.Drawing.Point(4, 5);
            this.mIntroSlideshowTabPage.Margin = new System.Windows.Forms.Padding(0);
            this.mIntroSlideshowTabPage.Name = "mIntroSlideshowTabPage";
            this.mIntroSlideshowTabPage.Size = new System.Drawing.Size(542, 1025);
            this.mIntroSlideshowTabPage.TabIndex = 3;
            this.mIntroSlideshowTabPage.Text = "tabPage5";
            this.mIntroSlideshowTabPage.UseVisualStyleBackColor = true;
            // 
            // mIntoSlideshowTabControl
            // 
            this.mIntoSlideshowTabControl.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.mIntoSlideshowTabControl.Controls.Add(this.mIntroSlideshowInnerTabPage);
            this.mIntoSlideshowTabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.mIntoSlideshowTabControl.ImageList = this.mIntroVideoTabPagesImageList;
            this.mIntoSlideshowTabControl.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.mIntoSlideshowTabControl.ItemSize = new System.Drawing.Size(100, 48);
            this.mIntoSlideshowTabControl.Location = new System.Drawing.Point(0, 0);
            this.mIntoSlideshowTabControl.Margin = new System.Windows.Forms.Padding(0);
            this.mIntoSlideshowTabControl.Multiline = true;
            this.mIntoSlideshowTabControl.Name = "mIntoSlideshowTabControl";
            this.mIntoSlideshowTabControl.Padding = new System.Drawing.Point(0, 0);
            this.mIntoSlideshowTabControl.SelectedIndex = 0;
            this.mIntoSlideshowTabControl.Size = new System.Drawing.Size(550, 1025);
            this.mIntoSlideshowTabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.mIntoSlideshowTabControl.TabIndex = 0;
            // 
            // mIntroSlideshowInnerTabPage
            // 
            this.mIntroSlideshowInnerTabPage.BackgroundImage = global::dvdslideshowfontend.Properties.Resources.back24;
            this.mIntroSlideshowInnerTabPage.Controls.Add(this.label23);
            this.mIntroSlideshowInnerTabPage.Controls.Add(this.mDiskIntroSlideshowImportSlideshowButton);
            this.mIntroSlideshowInnerTabPage.Controls.Add(this.mIncludeIntroVideoCheckBox);
            this.mIntroSlideshowInnerTabPage.Controls.Add(this.mInclideIntroSlideshowLabel);
            this.mIntroSlideshowInnerTabPage.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mIntroSlideshowInnerTabPage.ImageIndex = 0;
            this.mIntroSlideshowInnerTabPage.Location = new System.Drawing.Point(52, 4);
            this.mIntroSlideshowInnerTabPage.Margin = new System.Windows.Forms.Padding(0);
            this.mIntroSlideshowInnerTabPage.Name = "mIntroSlideshowInnerTabPage";
            this.mIntroSlideshowInnerTabPage.Size = new System.Drawing.Size(494, 1017);
            this.mIntroSlideshowInnerTabPage.TabIndex = 0;
            this.mIntroSlideshowInnerTabPage.UseVisualStyleBackColor = true;
            // 
            // label23
            // 
            this.label23.BackColor = System.Drawing.Color.Transparent;
            this.label23.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.ForeColor = System.Drawing.Color.SteelBlue;
            this.label23.Location = new System.Drawing.Point(24, 8);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(260, 39);
            this.label23.TabIndex = 45;
            this.label23.Text = "Disk intro slideshow";
            // 
            // mDiskIntroSlideshowImportSlideshowButton
            // 
            this.mDiskIntroSlideshowImportSlideshowButton.AllowDrop = true;
            this.mDiskIntroSlideshowImportSlideshowButton.Enabled = false;
            this.mDiskIntroSlideshowImportSlideshowButton.Location = new System.Drawing.Point(30, 169);
            this.mDiskIntroSlideshowImportSlideshowButton.Name = "mDiskIntroSlideshowImportSlideshowButton";
            this.mDiskIntroSlideshowImportSlideshowButton.Size = new System.Drawing.Size(132, 23);
            this.mDiskIntroSlideshowImportSlideshowButton.TabIndex = 2;
            this.mDiskIntroSlideshowImportSlideshowButton.Text = "Import slideshow";
            this.mDiskIntroSlideshowImportSlideshowButton.UseVisualStyleBackColor = true;
            // 
            // mIncludeIntroVideoCheckBox
            // 
            this.mIncludeIntroVideoCheckBox.AutoSize = true;
            this.mIncludeIntroVideoCheckBox.Location = new System.Drawing.Point(27, 57);
            this.mIncludeIntroVideoCheckBox.Name = "mIncludeIntroVideoCheckBox";
            this.mIncludeIntroVideoCheckBox.Size = new System.Drawing.Size(163, 17);
            this.mIncludeIntroVideoCheckBox.TabIndex = 1;
            this.mIncludeIntroVideoCheckBox.Text = "Include an intro slideshow";
            this.mIncludeIntroVideoCheckBox.UseVisualStyleBackColor = true;
            // 
            // mInclideIntroSlideshowLabel
            // 
            this.mInclideIntroSlideshowLabel.Location = new System.Drawing.Point(42, 89);
            this.mInclideIntroSlideshowLabel.Name = "mInclideIntroSlideshowLabel";
            this.mInclideIntroSlideshowLabel.Size = new System.Drawing.Size(329, 50);
            this.mInclideIntroSlideshowLabel.TabIndex = 0;
            this.mInclideIntroSlideshowLabel.Text = "An intro slideshow will be shown when the disk is started and before any menus.  " +
    "Once shown it will go to the menu.";
            // 
            // mIntroVideoTabPagesImageList
            // 
            this.mIntroVideoTabPagesImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("mIntroVideoTabPagesImageList.ImageStream")));
            this.mIntroVideoTabPagesImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.mIntroVideoTabPagesImageList.Images.SetKeyName(0, "intro.png");
            // 
            // panel4
            // 
            this.panel4.AllowDrop = true;
            this.panel4.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panel4.Controls.Add(this.pictureBox1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(500, 71);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(694, 389);
            this.panel4.TabIndex = 6;
            this.panel4.DragDrop += new System.Windows.Forms.DragEventHandler(this.panel4_DragDrop);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(96, 80);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(434, 0);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // SlideShowTimer
            // 
            this.SlideShowTimer.Interval = 20;
            // 
            // fontDialog1
            // 
            this.fontDialog1.Apply += new System.EventHandler(this.fontDialog1_Apply);
            // 
            // LoadProjectDialog
            // 
            this.LoadProjectDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.LoadProjectDialog_FileOk);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // ResetMenuMusicContextMenu
            // 
            this.ResetMenuMusicContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem17});
            // 
            // menuItem17
            // 
            this.menuItem17.Index = 0;
            this.menuItem17.Text = "Set blank";
            this.menuItem17.Click += new System.EventHandler(this.menuItem17_Click);
            // 
            // MenuItemsImageList
            // 
            this.MenuItemsImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("MenuItemsImageList.ImageStream")));
            this.MenuItemsImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.MenuItemsImageList.Images.SetKeyName(0, "");
            // 
            // mOutputImageList
            // 
            this.mOutputImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("mOutputImageList.ImageStream")));
            this.mOutputImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.mOutputImageList.Images.SetKeyName(0, "dvd video small.png");
            // 
            // mTopMenuStrip
            // 
            this.mTopMenuStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(212)))), ((int)(((byte)(234)))));
            this.mTopMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mFileToolStripMenuItem,
            this.mDisplayToolStripMenuItem,
            this.mToolsToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.tutorialsToolStripMenuItem,
            this.mHelpToolStripMenuItem,
            this.mDebugToolStripMenuItem});
            this.mTopMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mTopMenuStrip.Name = "mTopMenuStrip";
            this.mTopMenuStrip.Size = new System.Drawing.Size(1194, 24);
            this.mTopMenuStrip.TabIndex = 7;
            this.mTopMenuStrip.Text = "menuStrip1";
            // 
            // mFileToolStripMenuItem
            // 
            this.mFileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mNewProjectToolStripMenuItem,
            this.mOpenProjectToolStripMenuItem,
            this.toolStripSeparator6,
            this.mImportSlideshowsToolStripMenuItem,
            this.mImportTemplateToolStripMenuItem,
            this.mExportTemplateToolStripMenuItem,
            this.toolStripSeparator7,
            this.mSaveProjectToolStripMenuItem,
            this.mSaveProjectAsToolStripMenuItem,
            this.toolStripSeparator8,
            this.mExitToolStripMenuItem});
            this.mFileToolStripMenuItem.Name = "mFileToolStripMenuItem";
            this.mFileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.mFileToolStripMenuItem.Text = "File";
            // 
            // mNewProjectToolStripMenuItem
            // 
            this.mNewProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mNewProjectToolStripMenuItem.Image")));
            this.mNewProjectToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mNewProjectToolStripMenuItem.Name = "mNewProjectToolStripMenuItem";
            this.mNewProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.mNewProjectToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.mNewProjectToolStripMenuItem.Text = "New project";
            this.mNewProjectToolStripMenuItem.Click += new System.EventHandler(this.mNewProjectToolStripMenuItem_Click);
            // 
            // mOpenProjectToolStripMenuItem
            // 
            this.mOpenProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mOpenProjectToolStripMenuItem.Image")));
            this.mOpenProjectToolStripMenuItem.Name = "mOpenProjectToolStripMenuItem";
            this.mOpenProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.mOpenProjectToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.mOpenProjectToolStripMenuItem.Text = "Open project";
            this.mOpenProjectToolStripMenuItem.Click += new System.EventHandler(this.openProjectToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(183, 6);
            // 
            // mImportSlideshowsToolStripMenuItem
            // 
            this.mImportSlideshowsToolStripMenuItem.Name = "mImportSlideshowsToolStripMenuItem";
            this.mImportSlideshowsToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.mImportSlideshowsToolStripMenuItem.Text = "Import slideshows";
            this.mImportSlideshowsToolStripMenuItem.Click += new System.EventHandler(this.mImportSlideshowsToolStripMenuItem_Click);
            // 
            // mImportTemplateToolStripMenuItem
            // 
            this.mImportTemplateToolStripMenuItem.ForeColor = System.Drawing.Color.Fuchsia;
            this.mImportTemplateToolStripMenuItem.Name = "mImportTemplateToolStripMenuItem";
            this.mImportTemplateToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.mImportTemplateToolStripMenuItem.Text = "Import template";
            this.mImportTemplateToolStripMenuItem.Click += new System.EventHandler(this.mImportTemplateToolStripMenuItem_Click);
            // 
            // mExportTemplateToolStripMenuItem
            // 
            this.mExportTemplateToolStripMenuItem.ForeColor = System.Drawing.Color.Fuchsia;
            this.mExportTemplateToolStripMenuItem.Name = "mExportTemplateToolStripMenuItem";
            this.mExportTemplateToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.mExportTemplateToolStripMenuItem.Text = "Export template";
            this.mExportTemplateToolStripMenuItem.Click += new System.EventHandler(this.mExportTemplateToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(183, 6);
            // 
            // mSaveProjectToolStripMenuItem
            // 
            this.mSaveProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mSaveProjectToolStripMenuItem.Image")));
            this.mSaveProjectToolStripMenuItem.Name = "mSaveProjectToolStripMenuItem";
            this.mSaveProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mSaveProjectToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.mSaveProjectToolStripMenuItem.Text = "Save project";
            this.mSaveProjectToolStripMenuItem.Click += new System.EventHandler(this.mSaveProjectToolStripMenuItem_Click);
            // 
            // mSaveProjectAsToolStripMenuItem
            // 
            this.mSaveProjectAsToolStripMenuItem.Name = "mSaveProjectAsToolStripMenuItem";
            this.mSaveProjectAsToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.mSaveProjectAsToolStripMenuItem.Text = "Save project as";
            this.mSaveProjectAsToolStripMenuItem.Click += new System.EventHandler(this.mSaveProjectAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(183, 6);
            // 
            // mExitToolStripMenuItem
            // 
            this.mExitToolStripMenuItem.Name = "mExitToolStripMenuItem";
            this.mExitToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.mExitToolStripMenuItem.Text = "Exit";
            this.mExitToolStripMenuItem.Click += new System.EventHandler(this.mExitToolStripMenuItem_Click);
            // 
            // mDisplayToolStripMenuItem
            // 
            this.mDisplayToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mShowSelectedSlideshowToolStripMenuItem});
            this.mDisplayToolStripMenuItem.Name = "mDisplayToolStripMenuItem";
            this.mDisplayToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.mDisplayToolStripMenuItem.Text = "Display";
            // 
            // mShowSelectedSlideshowToolStripMenuItem
            // 
            this.mShowSelectedSlideshowToolStripMenuItem.Checked = true;
            this.mShowSelectedSlideshowToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mShowSelectedSlideshowToolStripMenuItem.Name = "mShowSelectedSlideshowToolStripMenuItem";
            this.mShowSelectedSlideshowToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
            this.mShowSelectedSlideshowToolStripMenuItem.Text = "Show selected slideshow in menu";
            // 
            // mToolsToolStripMenuItem
            // 
            this.mToolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setDefaultProjectTypeToolStripMenuItem,
            this.setMaxVideoLoadedAtOnceToolStripMenuItem,
            this.mSetDefaultFolderLocationsMenuItem});
            this.mToolsToolStripMenuItem.Name = "mToolsToolStripMenuItem";
            this.mToolsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.mToolsToolStripMenuItem.Text = "Settings";
            // 
            // setDefaultProjectTypeToolStripMenuItem
            // 
            this.setDefaultProjectTypeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.videoFileToolStripMenuItem,
            this.dVDVideoToolStripMenuItem,
            this.blurayToolStripMenuItem});
            this.setDefaultProjectTypeToolStripMenuItem.Name = "setDefaultProjectTypeToolStripMenuItem";
            this.setDefaultProjectTypeToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.setDefaultProjectTypeToolStripMenuItem.Text = "Default project type";
            // 
            // videoFileToolStripMenuItem
            // 
            this.videoFileToolStripMenuItem.Name = "videoFileToolStripMenuItem";
            this.videoFileToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.videoFileToolStripMenuItem.Text = "Video file";
            // 
            // dVDVideoToolStripMenuItem
            // 
            this.dVDVideoToolStripMenuItem.Name = "dVDVideoToolStripMenuItem";
            this.dVDVideoToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.dVDVideoToolStripMenuItem.Text = "DVD-Video";
            // 
            // blurayToolStripMenuItem
            // 
            this.blurayToolStripMenuItem.Name = "blurayToolStripMenuItem";
            this.blurayToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.blurayToolStripMenuItem.Text = "Blu-ray";
            // 
            // setMaxVideoLoadedAtOnceToolStripMenuItem
            // 
            this.setMaxVideoLoadedAtOnceToolStripMenuItem.Name = "setMaxVideoLoadedAtOnceToolStripMenuItem";
            this.setMaxVideoLoadedAtOnceToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.setMaxVideoLoadedAtOnceToolStripMenuItem.Text = "Video player settings";
            this.setMaxVideoLoadedAtOnceToolStripMenuItem.Click += new System.EventHandler(this.setMaxVideoLoadedAtOnceToolStripMenuItem_Click);
            // 
            // mSetDefaultFolderLocationsMenuItem
            // 
            this.mSetDefaultFolderLocationsMenuItem.Name = "mSetDefaultFolderLocationsMenuItem";
            this.mSetDefaultFolderLocationsMenuItem.Size = new System.Drawing.Size(215, 22);
            this.mSetDefaultFolderLocationsMenuItem.Text = "Set default folder locations";
            this.mSetDefaultFolderLocationsMenuItem.Click += new System.EventHandler(this.SetDefaultFolderLocationsMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mLaunchDVDBurnerToolToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.settingsToolStripMenuItem.Text = "Tools";
            // 
            // mLaunchDVDBurnerToolToolStripMenuItem
            // 
            this.mLaunchDVDBurnerToolToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mLaunchDVDBurnerToolToolStripMenuItem.Image")));
            this.mLaunchDVDBurnerToolToolStripMenuItem.Name = "mLaunchDVDBurnerToolToolStripMenuItem";
            this.mLaunchDVDBurnerToolToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.mLaunchDVDBurnerToolToolStripMenuItem.Text = "Launch DVD/Blu-ray burner tool";
            this.mLaunchDVDBurnerToolToolStripMenuItem.Click += new System.EventHandler(this.mLaunchDVDBurnerToolToolStripMenuItem_Click);
            // 
            // tutorialsToolStripMenuItem
            // 
            this.tutorialsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mVisitTutorialsToolStripMenuItem});
            this.tutorialsToolStripMenuItem.Name = "tutorialsToolStripMenuItem";
            this.tutorialsToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.tutorialsToolStripMenuItem.Text = "Tutorials";
            // 
            // mVisitTutorialsToolStripMenuItem
            // 
            this.mVisitTutorialsToolStripMenuItem.Name = "mVisitTutorialsToolStripMenuItem";
            this.mVisitTutorialsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.mVisitTutorialsToolStripMenuItem.Text = "Watch tutorials online";
            this.mVisitTutorialsToolStripMenuItem.Click += new System.EventHandler(this.mVisitTutorialseToolStripMenuItem_Click);
            // 
            // mHelpToolStripMenuItem
            // 
            this.mHelpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mHelpToolStripMenuItem1,
            this.mCheckForUpdatesToolStripMenuItem,
            this.toolStripSeparator9,
            this.mPurchaseToolStripMenuItem,
            this.mVisitPhotoVidShowWebsiteToolStripMenuItem,
            this.mAboutToolStripMenuItem});
            this.mHelpToolStripMenuItem.Name = "mHelpToolStripMenuItem";
            this.mHelpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.mHelpToolStripMenuItem.Text = "Help";
            // 
            // mHelpToolStripMenuItem1
            // 
            this.mHelpToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("mHelpToolStripMenuItem1.Image")));
            this.mHelpToolStripMenuItem1.Name = "mHelpToolStripMenuItem1";
            this.mHelpToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.mHelpToolStripMenuItem1.Size = new System.Drawing.Size(220, 22);
            this.mHelpToolStripMenuItem1.Text = "Help";
            this.mHelpToolStripMenuItem1.Click += new System.EventHandler(this.mHelpToolStripMenuItem1_Click);
            // 
            // mCheckForUpdatesToolStripMenuItem
            // 
            this.mCheckForUpdatesToolStripMenuItem.Name = "mCheckForUpdatesToolStripMenuItem";
            this.mCheckForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.mCheckForUpdatesToolStripMenuItem.Text = "Check for updates";
            this.mCheckForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.mCheckForUpdatesToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(217, 6);
            // 
            // mPurchaseToolStripMenuItem
            // 
            this.mPurchaseToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mPurchaseToolStripMenuItem.Image")));
            this.mPurchaseToolStripMenuItem.Name = "mPurchaseToolStripMenuItem";
            this.mPurchaseToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.mPurchaseToolStripMenuItem.Text = "Purchase information";
            this.mPurchaseToolStripMenuItem.Click += new System.EventHandler(this.mPurchaseToolStripMenuItem_Click);
            // 
            // mVisitPhotoVidShowWebsiteToolStripMenuItem
            // 
            this.mVisitPhotoVidShowWebsiteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mVisitPhotoVidShowWebsiteToolStripMenuItem.Image")));
            this.mVisitPhotoVidShowWebsiteToolStripMenuItem.Name = "mVisitPhotoVidShowWebsiteToolStripMenuItem";
            this.mVisitPhotoVidShowWebsiteToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.mVisitPhotoVidShowWebsiteToolStripMenuItem.Text = "Visit PhotoVidShow website";
            this.mVisitPhotoVidShowWebsiteToolStripMenuItem.Click += new System.EventHandler(this.mVisitPhotoVidShowWebsiteToolStripMenuItem_Click);
            // 
            // mAboutToolStripMenuItem
            // 
            this.mAboutToolStripMenuItem.Name = "mAboutToolStripMenuItem";
            this.mAboutToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.mAboutToolStripMenuItem.Text = "About";
            this.mAboutToolStripMenuItem.Click += new System.EventHandler(this.mAboutToolStripMenuItem_Click);
            // 
            // mDebugToolStripMenuItem
            // 
            this.mDebugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mShowMementoStackToolStripMenuItem,
            this.mShowImagesCacheToolStripMenuItem,
            this.mShowTextBoundaryToolStripMenuItem,
            this.showVideoCacheToolStripMenuItem});
            this.mDebugToolStripMenuItem.Name = "mDebugToolStripMenuItem";
            this.mDebugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.mDebugToolStripMenuItem.Text = "Debug";
            // 
            // mShowMementoStackToolStripMenuItem
            // 
            this.mShowMementoStackToolStripMenuItem.Name = "mShowMementoStackToolStripMenuItem";
            this.mShowMementoStackToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.mShowMementoStackToolStripMenuItem.Text = "Show memento stack";
            this.mShowMementoStackToolStripMenuItem.Click += new System.EventHandler(this.mShowMementoStackToolStripMenuItem_Click);
            // 
            // mShowImagesCacheToolStripMenuItem
            // 
            this.mShowImagesCacheToolStripMenuItem.Name = "mShowImagesCacheToolStripMenuItem";
            this.mShowImagesCacheToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.mShowImagesCacheToolStripMenuItem.Text = "Show images cache";
            this.mShowImagesCacheToolStripMenuItem.Click += new System.EventHandler(this.mShowImagesCacheToolStripMenuItem_Click);
            // 
            // mShowTextBoundaryToolStripMenuItem
            // 
            this.mShowTextBoundaryToolStripMenuItem.Name = "mShowTextBoundaryToolStripMenuItem";
            this.mShowTextBoundaryToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.mShowTextBoundaryToolStripMenuItem.Text = "Show text boundary";
            this.mShowTextBoundaryToolStripMenuItem.Click += new System.EventHandler(this.mShowTextBoundaryToolStripMenuItem_Click);
            // 
            // showVideoCacheToolStripMenuItem
            // 
            this.showVideoCacheToolStripMenuItem.Name = "showVideoCacheToolStripMenuItem";
            this.showVideoCacheToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.showVideoCacheToolStripMenuItem.Text = "Show video cache";
            this.showVideoCacheToolStripMenuItem.Click += new System.EventHandler(this.showVideoCacheToolStripMenuItem_Click);
            // 
            // mVideoImportToolStipItem
            // 
            this.mVideoImportToolStipItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mUseFFDShowForMovVideos,
            this.mUseFFDShowForMp4Videos,
            this.mUseFFDShowForMtsVideos});
            this.mVideoImportToolStipItem.Name = "mVideoImportToolStipItem";
            this.mVideoImportToolStipItem.Size = new System.Drawing.Size(236, 22);
            this.mVideoImportToolStipItem.Text = "Video import settings";
            // 
            // mUseFFDShowForMovVideos
            // 
            this.mUseFFDShowForMovVideos.Name = "mUseFFDShowForMovVideos";
            this.mUseFFDShowForMovVideos.Size = new System.Drawing.Size(262, 22);
            this.mUseFFDShowForMovVideos.Text = "Force use FFDShow for \'.mov\' video";
            this.mUseFFDShowForMovVideos.Click += new System.EventHandler(this.mUseFFDShowForMovVideos_Click);
            // 
            // mUseFFDShowForMp4Videos
            // 
            this.mUseFFDShowForMp4Videos.Name = "mUseFFDShowForMp4Videos";
            this.mUseFFDShowForMp4Videos.Size = new System.Drawing.Size(262, 22);
            this.mUseFFDShowForMp4Videos.Text = "Force use FFDShow for \'.mp4\' video";
            this.mUseFFDShowForMp4Videos.Click += new System.EventHandler(this.mUseFFDShowForMp4Videos_Click);
            // 
            // mUseFFDShowForMtsVideos
            // 
            this.mUseFFDShowForMtsVideos.Name = "mUseFFDShowForMtsVideos";
            this.mUseFFDShowForMtsVideos.Size = new System.Drawing.Size(262, 22);
            this.mUseFFDShowForMtsVideos.Text = "Force use FFDShow for \'.mts\' video";
            this.mUseFFDShowForMtsVideos.Click += new System.EventHandler(this.mUseFFDShowForMtsVideos_Click);
            // 
            // mDecorationsContextMenuStrip
            // 
            this.mDecorationsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mBringToFrontToolStripMenuItem,
            this.mSendToBackToolStripMenuItem,
            this.mDeleteToolStripMenuItem});
            this.mDecorationsContextMenuStrip.Name = "mDecorationsContextMenuStrip";
            this.mDecorationsContextMenuStrip.Size = new System.Drawing.Size(146, 70);
            // 
            // mBringToFrontToolStripMenuItem
            // 
            this.mBringToFrontToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mBringToFrontToolStripMenuItem.Image")));
            this.mBringToFrontToolStripMenuItem.Name = "mBringToFrontToolStripMenuItem";
            this.mBringToFrontToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.mBringToFrontToolStripMenuItem.Text = "Bring to front";
            this.mBringToFrontToolStripMenuItem.Click += new System.EventHandler(this.BringToFront_Click);
            // 
            // mSendToBackToolStripMenuItem
            // 
            this.mSendToBackToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mSendToBackToolStripMenuItem.Image")));
            this.mSendToBackToolStripMenuItem.Name = "mSendToBackToolStripMenuItem";
            this.mSendToBackToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.mSendToBackToolStripMenuItem.Text = "Send to back";
            this.mSendToBackToolStripMenuItem.Click += new System.EventHandler(this.SendToBack_Click);
            // 
            // mDeleteToolStripMenuItem
            // 
            this.mDeleteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mDeleteToolStripMenuItem.Image")));
            this.mDeleteToolStripMenuItem.Name = "mDeleteToolStripMenuItem";
            this.mDeleteToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.mDeleteToolStripMenuItem.Text = "Delete";
            this.mDeleteToolStripMenuItem.Click += new System.EventHandler(this.menuItem3_Click);
            // 
            // mGlobalMenuItemIcons
            // 
            this.mGlobalMenuItemIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("mGlobalMenuItemIcons.ImageStream")));
            this.mGlobalMenuItemIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.mGlobalMenuItemIcons.Images.SetKeyName(0, "delete2bw16x16.png");
            this.mGlobalMenuItemIcons.Images.SetKeyName(1, "edit16x16.png");
            this.mGlobalMenuItemIcons.Images.SetKeyName(2, "Paint_brush16x16.png");
            this.mGlobalMenuItemIcons.Images.SetKeyName(3, "RotateAntiClockwise16x16.png");
            this.mGlobalMenuItemIcons.Images.SetKeyName(4, "RotateClockwise16x16.png");
            this.mGlobalMenuItemIcons.Images.SetKeyName(5, "image-x-portable-graymap16x16.png");
            this.mGlobalMenuItemIcons.Images.SetKeyName(6, "font-12816x16.png");
            this.mGlobalMenuItemIcons.Images.SetKeyName(7, "Apps-accessories-text-editor-icon16x16.png");
            this.mGlobalMenuItemIcons.Images.SetKeyName(8, "Stacked Documents 16x16.png");
            this.mGlobalMenuItemIcons.Images.SetKeyName(9, "down16x16.png");
            this.mGlobalMenuItemIcons.Images.SetKeyName(10, "up16x16.png");
            this.mGlobalMenuItemIcons.Images.SetKeyName(11, "transform-move-icon.png");
            this.mGlobalMenuItemIcons.Images.SetKeyName(12, "Actions-transform-scale-icon16x16.png");
            this.mGlobalMenuItemIcons.Images.SetKeyName(13, "addblankslide16x16.png");
            this.mGlobalMenuItemIcons.Images.SetKeyName(14, "fade in 16x16.png");
            this.mGlobalMenuItemIcons.Images.SetKeyName(15, "fade in abd out 16x16.png");
            this.mGlobalMenuItemIcons.Images.SetKeyName(16, "fade out 16x16.png");
            this.mGlobalMenuItemIcons.Images.SetKeyName(17, "mic2small.png");
            this.mGlobalMenuItemIcons.Images.SetKeyName(18, "addslides16x16.png");
            // 
            // mDiskMenuImageList
            // 
            this.mDiskMenuImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("mDiskMenuImageList.ImageStream")));
            this.mDiskMenuImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.mDiskMenuImageList.Images.SetKeyName(0, "home2.png");
            this.mDiskMenuImageList.Images.SetKeyName(1, "submenu.png");
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1194, 706);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.slideshowpanel);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.mTopMenuStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mTopMenuStrip;
            this.Menu = this.mainMenu1;
            this.MinimumSize = new System.Drawing.Size(1020, 744);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PhotoVidShow";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.myKeyPressEventHandler);
            this.panel1.ResumeLayout(false);
            this.mHelpToolStrip.ResumeLayout(false);
            this.mHelpToolStrip.PerformLayout();
            this.mTopToolStripBar.ResumeLayout(false);
            this.mTopToolStripBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).EndInit();
            this.InnerDiskEstimationPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DiskEstimationInnerPictureBox)).EndInit();
            this.slideshowpanel.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.mStoryboardToolStrip.ResumeLayout(false);
            this.mStoryboardToolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mStoryboardTrackBar)).EndInit();
            this.panel5.ResumeLayout(false);
            this.mOuterTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.mMainMenuTabControl.ResumeLayout(false);
            this.mMenuStructureTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.mMenuThemeTabPage.ResumeLayout(false);
            this.mMenuThemeTabPage.PerformLayout();
            this.mMenuThemeTab.ResumeLayout(false);
            this.mMenuBackgroundTab.ResumeLayout(false);
            this.mMenuLayoutTab.ResumeLayout(false);
            this.mMenuFrameStyleTab.ResumeLayout(false);
            this.mMenuNavigationStyleTab.ResumeLayout(false);
            this.mMenuNavigationStyleTab.PerformLayout();
            this.mMenuHighlightStlyeTab.ResumeLayout(false);
            this.mMenuHighlightStlyeTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MenuRepeatLengthTrackbar)).EndInit();
            this.mEmptyMenuTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.mSlideOptionsTab.ResumeLayout(false);
            this.mPreDesignedSlidesTabPage.ResumeLayout(false);
            this.mBackgroundTabPage.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mDecorationPositionHeightTextBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mDecorationPositionLeftTextBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mDecorationPositionWidthTextBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mClipartTransparencyNumerical)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mDecorationPositionTopTextBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mDecorationRotationNumericArrows)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ClipartTransparencyTrackerBar)).EndInit();
            this.DecorationTabOptions.ResumeLayout(false);
            this.AddTextTab.ResumeLayout(false);
            this.AddTextTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mFontPreviewPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BackPlaneTransparencyTrackBar)).EndInit();
            this.ClipArtTab.ResumeLayout(false);
            this.ClipArtTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mDecorationRotationTrackbar)).EndInit();
            this.ClipPanTabPage.ResumeLayout(false);
            this.ClipPanTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mPanZoomInitialDelayTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mPreviewPanZoomPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EndPanRotationTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EndPanTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StartPanRotationTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StartPanTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mPanZoomEndDelayTrackBar)).EndInit();
            this.mFiltersTabPage.ResumeLayout(false);
            this.mBordersTab.ResumeLayout(false);
            this.mIntroSlideshowTabPage.ResumeLayout(false);
            this.mIntoSlideshowTabControl.ResumeLayout(false);
            this.mIntroSlideshowInnerTabPage.ResumeLayout(false);
            this.mIntroSlideshowInnerTabPage.PerformLayout();
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.mTopMenuStrip.ResumeLayout(false);
            this.mTopMenuStrip.PerformLayout();
            this.mDecorationsContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion




        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>


        //*******************************************************************
        // i.e. add a slide show to the current project
        private void NewSlideShow_Click(object sender, System.EventArgs e)
        {
            if (mSlideShowManager != null) mSlideShowManager.StopIfPlayingAndWaitForCompletion();

            GoToMainMenu(true);

            CSlide slide = this.mDecorationManager.mCurrentSlide;
            CMainMenu menu = null;
            if (slide != null)
            {
                menu = CGlobals.mCurrentProject.GetMenuWhichContainsSlide(slide);
            }

            if (menu != null)
            {
                int num_sub_slideshows = menu.GetSlideshowsSelectableFromMenu().Count;
                if (num_sub_slideshows >= 6)
                {
                    DialogResult res = UserMessage.Show("Maximum of 6 slideshows per menu.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            bool illegal_before = menu.IsAnySubtitledDecorationIllegal();

            CSlideShow currentSlideshow = null;
            if (mSlideShowManager != null)
            {
                currentSlideshow = mSlideShowManager.mCurrentSlideShow;
            }

            CSlideShow s = this.mCurrentProject.CreateNewSlideshow(menu, currentSlideshow);
            SelectedSlideShow(s.Name);
            if (mDecorationManager != null)
            {
                mDecorationManager.RePaint();
            }

            if (illegal_before == false &&
                menu.IsAnySubtitledDecorationIllegal() == true)
            {
                DialogResult res = UserMessage.Show("Adding a new slideshow has caused a slideshow button or link arrow button to intersect. This will need to be resolved before burn time! There must be at least a 2% gap between all slideshow buttons and link arrow buttons and they are not allowed to intersect", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        //*******************************************************************
        // add slides button
        private void AddSlidesButton_Click(object sender, System.EventArgs e)
        {
            mAddSlidesBalloon.Hide();
            mSlideShowManager.AddSlidesButton_Click(true);
        }

        //*******************************************************************
        private void contextMenu1_Popup(object sender, System.EventArgs e)
        {

        }

        //*******************************************************************
        // PUSH THE LOAD BUTTON
        private void button1_Click(object sender, System.EventArgs e)
        {
            if (mSlideShowManager != null) mSlideShowManager.StopIfPlayingAndWaitForCompletion();
            GoToMainMenu();

            bool res1 = this.CheckIfOldProjectNeedsSaving();
            if (res1 == true)
            {
                return;
            }

            this.mAddSlidesBalloon.Hide();
            if (LoadProjectDialog.InitialDirectory == "")
            {
                LoadProjectDialog.InitialDirectory = DefaultFolders.GetFolder("MyProjects");
            }
            LoadProjectDialog.Filter = "Project Files (*.pds;*.pds2)|*.pds;*.pds2";
            LoadProjectDialog.FilterIndex = 2;
            LoadProjectDialog.RestoreDirectory = true;

            if (!LoadProjectDialog.ShowDialog().Equals(System.Windows.Forms.DialogResult.OK))
            {
                ReCalcTitleBarString();
                return;
            }

            LoadProjectDialog.InitialDirectory = System.IO.Path.GetDirectoryName(LoadProjectDialog.FileName);  // rember current folder  

            OpenFormFile(LoadProjectDialog.FileName);

            Form1.mMainForm.GoToMainMenu();
            Form1.mMainForm.UpdateOuterTabControlTabs();
            Form1.mMainForm.GetDecorationManager().RePaint();

        }

        // **********************************************************************************************
        private void OpenFormFile(string the_file)
        {
            CProject p;

            mSlideShowManager.SetCurrentSlideShow(null);

            bool load_success = false;

            ManagedCore.MissingFilesDatabase.GetInstance().Clear();

            do
            {
                // Remember current template effects, incase load went wrong
                List<CAnimatedDecorationEffect> oldTemplateEffects = CAnimatedDecorationEffectDatabase.GetInstance().GetAllTemplateEffects();
                CAnimatedDecorationEffectDatabase.GetInstance().ClearAllButDefaults();

                p = new CProject();

                ProgressWindow progressWindow = new ProgressWindow(this, p.Load, null);
                progressWindow.StartDelegateObject = progressWindow;
                progressWindow.Text = "Loading";
                progressWindow.CancelButtons.Enabled = false;

                p.mFilename = the_file;
                //	try
                //	{
                progressWindow.ShowDialog();

                if (ManagedCore.MissingFilesDatabase.GetInstance().AbortedLoad == true)
                {
                    string reason = ManagedCore.MissingFilesDatabase.GetInstance().AbortedReasonString;
                    if (reason != "")
                    {
                        ManagedCore.CDebugLog.GetInstance().PopupError("Project could not load...\n\r\n\r" + reason);
                    }

                    CancelOpenFromFile(oldTemplateEffects);
                    return;
                }

                //	}
                //		catch (Exception e)
                //		{
                //			CDebugLog.GetInstance().Error("Failed to load project..");
                //			CGlobals.mCurrentProject = mCurrentProject;
                //			return ;
                //		}

                if (ManagedCore.MissingFilesDatabase.GetInstance().GetUnlinkedFiles().Count == 0)
                {
                    load_success = true;
                }
                else
                {
                    DialogResult res = UserMessage.Show("Media files this project require can not be found. This may be due to the files being moved to another folder after the project was originally created. Press OK to relink these files or Cancel to abort", "Missing files",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);

                    if (res == DialogResult.OK)
                    {

                        ArrayList missing_files = ManagedCore.MissingFilesDatabase.GetInstance().GetMissingFilesData();

                        ReLinkFilesDialog dialog = new ReLinkFilesDialog();

                        dialog.HandoverMissingFilesList(missing_files);

                        res = dialog.ShowDialog();
                    }

                    // keep current project
                    if (res == DialogResult.Cancel)
                    {
                        CancelOpenFromFile(oldTemplateEffects);
                        return;
                    }
                }

            } while (load_success == false);

            mCurrentProject = p;

            ArrayList slideshows = mCurrentProject.GetAllProjectSlideshows(false);
            if (slideshows.Count > 0)
            {
                CSlideShow s = slideshows[0] as CSlideShow;

                this.mDecorationManager.SetCurrentSlide(mCurrentProject.MainMenu.BackgroundSlide);
                SelectedSlideShow(s.Name);
            }

            this.mDiskMenuManager.RebuildLabels();

            SetOutputAspectRatioComboAndVideoType();
            ViewingAreaResize(true);

            DVDSlideshow.Memento.Caretaker.GetInstance().Clear();

            this.ReEvalDoUndoButtons();

            this.ProjectHasChanged(true, "Load project");
            ReCalcTitleBarString();


            this.mDecorationManager.SetCurrentSlide(null);
            this.GoToMainMenu();
            this.mDecorationManager.RePaint();


            // if there we missing files then the project has changed and will need saving before close
            if (ManagedCore.MissingFilesDatabase.GetInstance().GetMissingFilesData().Count > 0)
            {
                mCurrentProject.DeclareChange(true, "Re-linked");
            }
        }

        // *****************************************************************************************************
        // Gets called when user cancels a load project or loading a project went wrong.  In both cases we
        // load back in the old original project
        private void CancelOpenFromFile(List<CAnimatedDecorationEffect> oldTemplateEffects)
        {
            CGlobals.mCurrentProject = mCurrentProject;
            CAnimatedDecorationEffectDatabase.GetInstance().ClearAllButDefaults();  // just incase some new invalid ones got loaded
            CAnimatedDecorationEffectDatabase.GetInstance().AddEffects(oldTemplateEffects);
            CSlideShow s = mCurrentProject.GetAllProjectSlideshows(false)[0] as CSlideShow;
            SelectedSlideShow(s.Name);
            ReCalcTitleBarString();
            return;

        }

        // *****************************************************************************************************
        private void SetOutputAspectRatioComboAndVideoType()
        {
            mDiskEstimationManager.SetOutputAspectRatioComboAndOutputVideoType();
        }

        //*******************************************************************
        // save
        private void button2_Click(object sender, System.EventArgs e)
        {
            if (mSlideShowManager != null) mSlideShowManager.StopIfPlayingAndWaitForCompletion();

            if (mCurrentProject.mName == null ||
                mCurrentProject.mName == "Untitled")
            {
                SaveAs();
            }
            else
            {
                mCurrentProject.Save();
            }
            ReCalcTitleBarString();
        }


        //*******************************************************************
        // save as
        private void SaveAs()
        {
            if (mSlideShowManager != null) mSlideShowManager.StopIfPlayingAndWaitForCompletion();

            SaveProjectDialog.Filter = "Project Files (*.pds2)|*.pds2";
            SaveProjectDialog.FilterIndex = 2;
            SaveProjectDialog.RestoreDirectory = true;
            SaveProjectDialog.InitialDirectory = LoadProjectDialog.InitialDirectory;
            if (SaveProjectDialog.InitialDirectory == "")
            {
                SaveProjectDialog.InitialDirectory = DefaultFolders.GetFolder("MyProjects");
            }

            if (SaveProjectDialog.ShowDialog() == DialogResult.OK)
            {
                mCurrentProject.mName = SaveProjectDialog.FileName;
                mCurrentProject.Save();
                LoadProjectDialog.InitialDirectory = System.IO.Path.GetDirectoryName(SaveProjectDialog.FileName); ;
            }
            ReCalcTitleBarString();
        }

        //*******************************************************************
        // save as
        private void mSaveProjectAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }


        //*******************************************************************
        private void panel4_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {

        }

        //*******************************************************************
        // add text
        private void AddTextButton_Click(object sender, System.EventArgs e)
        {
            this.mDecorationManager.AddTextButtonPushed();
        }


        //*******************************************************************
        public Button GetTextFontButton()
        {
            return mFontButton;
        }

        //*******************************************************************
        public Button GetDoneDecoratingMenuLayoutButton()
        {
            return mDoneDecorationMenuButton;
        }

        //*******************************************************************
        private void BackPlaneCheckbox_CheckedChanged(object sender, System.EventArgs e)
        {
            mDecorationManager.SelectedBackPlaneCheckBox(BackPlaneCheckbox.Checked);
        }


        //*******************************************************************
        // bring to font
        private void BringToFront_Click(object sender, System.EventArgs e)
        {
            mDecorationManager.BringToFront();
        }

        //*******************************************************************
        // delete decoration
        private void menuItem3_Click(object sender, System.EventArgs e)
        {
            mDecorationManager.DeleteCurrentSelectedDecoration();
        }

        //*******************************************************************
        // send to back
        private void SendToBack_Click(object sender, System.EventArgs e)
        {
            mDecorationManager.SendToBack();

        }

        //*******************************************************************
        private void tabControl2_SelectedIndexChanged(object sender, System.EventArgs e)
        {

        }


        //*******************************************************************
        private void tabPage2_Click(object sender, System.EventArgs e)
        {

        }

        //*******************************************************************
        public TabPage GetAddTextAndImagesTabPage()
        {
            return tabPage2;
        }

        //*******************************************************************
        public void SetMainTabControlToAppropriateDiskTab()
        {
            if (mSlideShowManager == null || mSlideShowManager.mCurrentSlideShow == null)
            {
                return;
            }

            CSlideShow cs = this.mSlideShowManager.mCurrentSlideShow;

            //
            // If current selected slideshow the pre menu slideshow, then show that pre menu tab
            //
            if (CGlobals.mCurrentProject.PreMenuSlideshow != null && CGlobals.mCurrentProject.PreMenuSlideshow == cs)
            {
                if (GetMainTabControl().SelectedTab != this.mIntroSlideshowTabPage)
                {
                    SetMainTabControlToIntroSlideshowTab();
                }
            }
            else
            {
                //
                // Else show normal menus tab
                //
                if (GetMainTabControl().SelectedTab != tabPage1)
                {
                    SetMainTabControlToMenuTab();
                }
            }
        }

        //*******************************************************************
        private void SetMainTabControlToMenuTab()
        {
            if (mForceNoMenuTabChange == true)
            {
                return;
            }

            mOuterTabControl.SelectedTab = tabPage1;
        }

        //*******************************************************************
        private void SetMainTabControlToIntroSlideshowTab()
        {
            if (mForceNoMenuTabChange == true)
            {
                return;
            }
            mOuterTabControl.SelectedTab = mIntroSlideshowTabPage;
        }

        //*******************************************************************
        private void OuterTabControl_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            UpdateOuterTabControlTabs();

            // select decorations
            if (mOuterTabControl.SelectedTab == tabPage3)
            {
                this.mDecorationManager.SelectedDecorationsTab();
            }
        }

        //*******************************************************************
        public void UpdateOuterTabControlTabs()
        {
            // if selected menu and in decorations mode then go back to menu
            if (mOuterTabControl.SelectedTab == tabPage1)
            {
                // hack to stop a flicker when we push play button
                if (this.mSlideShowManager != null &&
                    this.mSlideShowManager.IsPlaying() == true)
                {
                    return;
                }

                if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.MP4)
                {
                    this.mMainMenuTabControl.TabPages.Remove(mMenuStructureTabPage);
                    this.mMainMenuTabControl.TabPages.Remove(mMenuThemeTabPage);

                    if (mMainMenuTabControl.TabPages.Contains(mEmptyMenuTabPage) == false)
                    {
                        mMainMenuTabControl.TabPages.Add(mEmptyMenuTabPage);
                    }
                }
                else
                {
                    this.mMainMenuTabControl.TabPages.Remove(mEmptyMenuTabPage);

                    if (mMainMenuTabControl.TabPages.Contains(mMenuStructureTabPage) == false)
                    {
                        mMainMenuTabControl.TabPages.Add(mMenuStructureTabPage);
                        mMainMenuTabControl.TabPages.Add(mMenuThemeTabPage);
                    }

                }

                GoToMainMenu();
            }
        }

        //*******************************************************************
        public void GoToMainMenu()
        {
            GoToMainMenu(true);
        }

        //*******************************************************************
        public void GoToMainMenu(bool unHighlightAllThumnails)
        {
            //
            // Prevent recursive calls to this function.
            //
            if (mDoingGoToMainMenu == true)
            {
                return;
            }

            try
            {
                mDoingGoToMainMenu = true;

                if (mCurrentProject == null) return;
                if (mCurrentProject.MainMenu == null) return;

                this.mCurrentProject.ResetAllMediaStreams();
                CustomButton.MiniPreviewController.StopAnyPlayingController();

                CSlideShow cs = this.mSlideShowManager.mCurrentSlideShow;

                CMainMenu mm = CGlobals.mCurrentProject.GetMenuWhichContainsSlideshow(cs);

                if (mm == null) mm = this.mCurrentProject.MainMenu;

                this.mDecorationManager.SetCurrentSlide(mm.BackgroundSlide);
                this.mDiskMenuManager.DoHideUnHide();

                if (unHighlightAllThumnails == true)
                {
                    this.mSlideShowManager.UnHighlightAllThumbnails(false);
                }

                SetMainTabControlToAppropriateDiskTab();
            }
            finally
            {
                mDoingGoToMainMenu = false;
            }
        }

        //*******************************************************************
        public void GoToSelectedSlide()
        {
            if (mCurrentProject == null) return;

            this.mCurrentProject.ResetAllMediaStreams();

            if (mSlideShowManager.GetHighlightesThumbnailsCount() == 0)
            {
                GoToMainMenu();
                return;
            }

            this.mSlideShowManager.DecorateButton_Click(null, new EventArgs());
            this.mDiskMenuManager.DoHideUnHide();
        }


        //*******************************************************************
        private bool CheckIfOldProjectNeedsSaving()
        {
            bool do_check = false;

            ArrayList slideshows = mCurrentProject.GetAllProjectSlideshows(false);

            if (mCurrentProject.mChangedSinceSave == true &&
                slideshows.Count > 0)
            {
                if (slideshows.Count > 1)
                {
                    do_check = true;
                }
                else if (((CSlideShow)slideshows[0]).mSlides.Count > 0)
                {
                    do_check = true;
                }
            }

            if (do_check == true)
            {

                DialogResult res = UserMessage.Show("The project '" + CGlobals.mCurrentProject.GetNameWithoutPath() + "' has changed\n\r\n\rDo you want to save the changes?", CGlobals.mCurrentProject.GetNameWithoutPath(),
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                if (res == DialogResult.Cancel)
                {
                    return true;
                }
                if (res == DialogResult.No)
                {
                    return false;
                }
                if (res == DialogResult.Yes)
                {
                    button2_Click(this, new System.EventArgs());
                    return false;
                }
            }

            return false;
        }

        //*******************************************************************
        // hit new project !!!
        public void NewSlideshowButton_Click(object sender, System.EventArgs e)
        {
            NewProject(false);
        }

        //*******************************************************************
        public void NewProject(bool initialStartup)
        {
            if (mSlideShowManager != null) mSlideShowManager.StopIfPlayingAndWaitForCompletion();
            GoToMainMenu();

            // SRG to do save quit current project
            bool cancel = CheckIfOldProjectNeedsSaving();
            if (cancel == true) return;

            CProject old_project = mCurrentProject;
            mCurrentProject = new CProject();

            mCurrentProject.ForceNoMemento = true;

            if (old_project != null)
            {
                if (old_project.DiskPreferences.TVType == CGlobals.OutputTVStandard.PAL)
                {
                    mCurrentProject.DiskPreferences.SetToPAL();
                }
                else
                {
                    mCurrentProject.DiskPreferences.SetToNTSC();
                }
            }


            if (mSlideShowManager == null) mSlideShowManager = new CSlideShowManager(this);
            if (mSlideShowMusicManager == null) mSlideShowMusicManager = new CSlideShowMusicManager(this);
            if (mSlideShowNarrationManager == null) mSlideShowNarrationManager = new CSlideShowNarrationManager(this);

            DVDSlideshow.Memento.Caretaker.GetInstance().Clear();

            CAnimatedDecorationEffectDatabase.GetInstance().ClearAllButDefaults();

            this.ReEvalDoUndoButtons();

            CSlideShow s = mCurrentProject.CreateNewSlideshow(null, null);
            SelectedSlideShow(s.Name);
            this.mDecorationManager.SetCurrentSlide(mCurrentProject.MainMenu.BackgroundSlide);
            this.mDiskMenuManager.RebuildLabels();

            DVDSlideshow.Memento.Caretaker.GetInstance().Clear();
            this.ReEvalDoUndoButtons();
          
            this.ProjectHasChanged(true, "New project");
            ReCalcTitleBarString();

            mDiskEstimationManager.SetupNewProjectOutputType();

            mCurrentProject.ForceNoMemento = false;

            mSlideShowManager.RebuildPanel(null);

            this.ViewingAreaResize(this, new System.EventArgs());
            CGlobals.mCurrentProject.IgnoreProjectChanges = true;
            SetOutputAspectRatioComboAndVideoType();
            CGlobals.mCurrentProject.IgnoreProjectChanges = false;
            this.ViewingAreaResize(true);

            this.mDecorationManager.SetCurrentSlide(null);
            this.GoToMainMenu();
            this.UpdateOuterTabControlTabs();
            this.mDecorationManager.RePaint();

            if (mOpenWithFile == "" || initialStartup == false)
            {
                mDiskEstimationManager.SelectInitialOutputType(this, initialStartup);
            }
        }

        //*******************************************************************
        public void SelectedSlideShow(string id)
        {
            CSlideShow s = mCurrentProject.GetSlideshow(id);
            if (s == null)
            {
                Console.WriteLine("Error: Could not select slide show " + id);
                return;
            }

            this.mSlideShowManager.SetCurrentSlideShow(s);

        }


        //*******************************************************************
        // this get called whenever the slide show manager reports that it has changed
        // itself
        public void SlideShowManagerHasChangedCallback()
        {
            this.mDecorationManager.RePaint();
            if (this.mSlideShowMusicManager != null)
            {
                this.mSlideShowMusicManager.RebuildPanel();
            }
        }

        //*******************************************************************
        private bool CheckForValidVideoSlideslow()
        {
            CSlideShow selectedSlideshow = mSlideShowManager.mCurrentSlideShow;

            if (selectedSlideshow !=null)
            {
                if (selectedSlideshow.mSlides.Count > 0)
                {
                    return true;
                }
            }

            DialogResult result = UserMessage.Show("An empty slideshow was detected which is not allowed when authoring.", "Empty slideshow",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            return false;

        }

        //*******************************************************************
        private bool CheckForValidMenuStructure()
        {
            try
            {
                if (CGlobals.mCurrentProject.PreMenuSlideshow != null)
                {
                    if (CGlobals.mCurrentProject.PreMenuSlideshow.mSlides.Count <= 0)
                    {
                        DialogResult result = UserMessage.Show("The disk intro slideshow contains no slides which is not allowed when authoring.\r\n\r\nPress ok to remove the disk intro slideshow else select cancel.", "Empty disk intro slideshow",
                                      MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);

                        if (result == DialogResult.OK)
                        {
                            CGlobals.mCurrentProject.PreMenuSlideshow = null;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                ArrayList normalSlideshows = CGlobals.mCurrentProject.GetAllProjectSlideshows(false);

                // check if we have a null slideshow but not an empty project
                if (normalSlideshows.Count > 1)
                {
                    for (int df = 0; df < normalSlideshows.Count; df++)
                    {
                        CSlideShow cs = normalSlideshows[df] as CSlideShow;

                        CMainMenu mm = CGlobals.mCurrentProject.GetMenuWhichContainsSlideshow(cs);

                        if (cs != null)
                        {
                            if (cs.mSlides.Count == 0)
                            {
                                ArrayList list = mm.GetSlideshowsSelectableFromMenu();

                                if (list.Count == 1)
                                {
                                    DialogResult result = UserMessage.Show("An empty sub menu was detected which is not allowed when authoring. \n\r\n\rDo you wish to automatically remove it?\n\r", "Empty sub menu",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                                    if (result == DialogResult.Yes)
                                    {
                                        bool removed_menu = false;

                                        CGlobals.mCurrentProject.RemoveSlideshow(cs, out removed_menu);
                                        GoToMainMenu();

                                        GetDecorationManager().SelectFirstSlideshowInCurrentMenu();
                                        GetDecorationManager().RePaint();
                                        CGlobals.mCurrentProject.DeclareChange(true, "Delete Slideshow");

                                        return CheckForValidMenuStructure();
                                    }
                                }
                                else
                                {
                                    UserMessage.Show("At least one slideshow contains no slides. This is not allowed when authoring.\n\r\n\rPlease remove any empty slideshows if they are not required.\n\r\n\rTip: check that an empty sub menu was not made by accident.", "Empty slideshow?",
                                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                                return false;
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                CDebugLog.GetInstance().Error("Exception occurred in Author dialog: "+e.Message);
            }
            return true;
        }

        private void AuthorButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (mSlideShowManager != null) mSlideShowManager.StopIfPlayingAndWaitForCompletion();
                mMainForm.GoToMainMenu();
                mMainForm.GetDecorationManager().RePaint();

                if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType != CGlobals.VideoType.MP4)
                {
                    bool check_result = CheckForValidMenuStructure();
                    if (check_result == false) return;
                }
                else
                {
                    bool check_result = CheckForValidVideoSlideslow();
                    if (check_result == false) return;
                }

                ArrayList mm_list = CGlobals.mCurrentProject.MainMenu.GetSelfAndAllSubMenus();

                int cc = 1;
                foreach (CMainMenu mm in mm_list)
                {
                    if (mm.IsAnySubtitledDecorationIllegal() == true)
                    {
                        UserMessage.Show("Menu " + cc + " has an illegal setup. There must be at least a 2% gap between all slideshow buttons and link arrow buttons and they are not allowed to intersect", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }



                bool res = CheckIfOldProjectNeedsSaving();
                if (res == true)
                {
                    ReCalcTitleBarString();
                    return;
                }
                ReCalcTitleBarString();

                Author a = new Author(this, CGlobals.mDoDVDPadding);

                //a.Show();
                a.ShowDialog();

                ViewingAreaResize(true);
            }
            catch (Exception e2)
            {
                CDebugLog.GetInstance().Error("Exception occurred in Author dialog: "+e2.Message);
            }
        }

        private void fontDialog1_Apply(object sender, System.EventArgs e)
        {

        }

        private void LoadProjectDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        //*******************************************************************
        private void mNewProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.NewSlideshowButton_Click(sender, e);
        }


        //*******************************************************************
        // open from file menu
        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.button1_Click(sender, e);
        }


        //*******************************************************************
        // save from file menu
        private void mSaveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.button2_Click(sender, e);
        }


        //*******************************************************************
        // exit from file menu
        private void mExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSlideShowManager != null) mSlideShowManager.StopIfPlayingAndWaitForCompletion();
            this.Close();
        }


        //*******************************************************************
        public CSlideShowManager GetSlideShowManager()
        {
            return mSlideShowManager;
        }

        //*******************************************************************
        public ToolStripButton GetSetMenuThumbnailButton()
        {
            return this.mSetMenuthumbnailButton;
        }

        //*******************************************************************
        public ToolStripButton GetDeleteSlidesButton()
        {
            return this.mDeleteButton; ;
        }


        //*******************************************************************
        public Label GetTotalSlidesSlideshowTextBox()
        {
            return this.TotalSlidesSlideshowTextBox;
        }

        //*******************************************************************
        public CSlideShowMusicManager GetSlideShowMusicManager()
        {
            return this.mSlideShowMusicManager;
        }

        //*******************************************************************
        public CSlideShowNarrationManager GetSlideShowNarrationManager()
        {
            return this.mSlideShowNarrationManager;
        }

        //*******************************************************************
        //	public Label GetAddMusicHereLabel()
        //	{
        //		return this.AddMusicHereLabel;
        //	}


        private void AddSlideshowMusicButton_Click(object sender, System.EventArgs e)
        {
            mSlideShowMusicManager.AddMusicButton_Click(sender, e);
        }


        // set dvd menu background graphics
        private void SetMenuBackgroundButton_Click(object sender, System.EventArgs e)
        {
            if (mSlideShowManager != null) mSlideShowManager.StopIfPlayingAndWaitForCompletion();
            this.mDiskMenuManager.SetMenuBackground(sender, e);
        }

        public TextBox GetMenuBackgroundMusicTextBox()
        {
            return MenuBackgroundMusicName;
        }

        public TextBox GetMenuBackgroundTextBox()
        {
            return MenuBackgroundName;
        }

        // advance options
        private void AdvanceSlideshowOptionButton_Click(object sender, System.EventArgs e)
        {
            if (mSlideShowManager != null) mSlideShowManager.StopIfPlayingAndWaitForCompletion();

            this.GoToMainMenu();

            SlideShowAdvanceOptions dialog = new SlideShowAdvanceOptions(false);
            dialog.ShowDialog();
        }

        public TrackBar GetSlideShowTrackBar()
        {
            return this.mStoryboardTrackBar;
        }

        private void trackBar2_Scroll(object sender, System.EventArgs e)
        {

        }


        public Label GetSlideShowTimeLabel()
        {
            return SlideShowTimeLabel;
        }

        private void mDecorateButton_Click(object sender, System.EventArgs e)
        {
            if (mSlideShowManager != null) mSlideShowManager.StopIfPlayingAndWaitForCompletion();
            this.mSlideShowManager.DecorateButton_Click(sender, e);
        }

        public CDecorationsManager GetDecorationManager()
        {
            return mDecorationManager;
        }

        public TabControlEx GetDecorationsTabControl()
        {
            return mSlideOptionsTab;
        }

        public TabPage GetTextAndImagesTabPage()
        {
            return tabPage2; ;
        }

        public TabPage GetClipPanTabPage()
        {
            return ClipPanTabPage;

        }

        public TabControl GetMainTabControl()
        {
            return this.mOuterTabControl;
        }

        public TabPage GetDecorationsTabPage()
        {
            return this.tabPage3;
        }

        public TrackBar GetStartPanTrackBar()
        {
            return StartPanTrackBar;
        }

        public TrackBar GetStartPanRotationTrackBar()
        {
            return StartPanRotationTrackBar;
        }

        public TrackBar GetEndPanTrackBar()
        {
            return EndPanTrackBar;
        }

        public TrackBar GetEndPanRotationTrackBar()
        {
            return EndPanRotationTrackBar;
        }

        public Label GetStartPanTrackBarLabel()
        {
            return StartPanTrackBarLabel;
        }

        public Label GetStartRotationTrackBarLabel()
        {
            return StartRotationTrackBarLabel;
        }

        public Label GetEndPanTrackBarLabel()
        {
            return EndPanTrackBarLabel;
        }

        public Label GetEndRotationTrackBarLabel()
        {
            return EndRotationTrackBarLabel;
        }

        public CheckBox GetClipArtLockAspectCheckBox()
        {
            return mClipArtLockAspectCheckBox;
        }

        public TextBox GetDecorXPositionTextBox()
        {
            return null;
            //  return mDecorXPositionTextBox;
        }

        public TextBox GetDecorYPositionTextBox()
        {
            return null;
            //  return mDecorYPositionTextBox;
        }

        public TextBox GetDecorHeightTextBox()
        {
            return null;
            // return mDecorHeightTextBox;
        }

        public TextBox GetDecorWidthTextBox()
        {
            return null;
            //  return mDecorWidthTextBox;
        }

        public ToolStripButton GetRotateSlideAntiClockwiseButton()
        {
            return RotateSlideAntiClockwiseButton;
        }

        public ToolStripButton GetRotateSlideClockWiseButton()
        {
            return RotateSlideClockWiseButton;
        }

        public ToolStripButton GetStoryboardEditItemButton()
        {
            return mStoryboardEditItemButton;
        }

        public Button GetRotateDecorationClockWiseButton()
        {
            return RotateDecorationClockWiseButton;
        }

        public Button GetRotateDecorationAntiClockWiseButton()
        {
            return RotateDecorationAntiClockWiseButton;
        }

        public Button GetFlipDecorationLeftRightButton()
        {
            return FlipDecorationLeftRightButton;
        }

        public Button GetFlipDecorationUpDownButton()
        {
            return FlipDecorationUpDownButton;
        }

        public PictureBox GetPreviewPanZoomPictureBox()
        {
            return mPreviewPanZoomPictureBox;
        }

        public ToolStripDropDownButton GetOutputDiskTypeDropDownButton()
        {
            return mOutputDiskTypeDropDownButton;
        }

        public Panel GetInnerDiskEstimationPanel()
        {
            return InnerDiskEstimationPanel;
        }

        public PictureBox GetDiskEstimationInnerPictureBox()
        {
            return DiskEstimationInnerPictureBox;
        }



        private void pictureBox2_Click(object sender, System.EventArgs e)
        {

        }

        public PictureBox GetPreviewPictureBox() { return this.pictureBox1; }


        public void EndPreview()
        {
            this.mMainMenuTabControl.SelectedTab = this.mMenuThemeTabPage;
            this.mDecorationManager.SetCurrentSlide(mCurrentProject.MainMenu.BackgroundSlide);
        }

        public Button GetAddNextMenuButton()
        {
            return AddNextMenuButton;
        }

        public CDiskMenuManager GetDiskMenuManager()
        {
            return mDiskMenuManager;
        }

        public TrackBar GetMenuRepeatLengthTrackbar()
        {
            return MenuRepeatLengthTrackbar;
        }

        public Label GetMenuRepeatDurationTextbox()
        {
            return MenuRepeatDurationTextbox;
        }

        private void DiskEstimationInnerPictureBox_Click(object sender, System.EventArgs e)
        {

        }

        private void label13_Click(object sender, System.EventArgs e)
        {

        }

        public void ViewingAreaResize(Object o, System.EventArgs e)
        {
            ViewingAreaResize(false);
        }

        public void ViewingAreaResize(bool forceEngineChange)
        {

            System.Drawing.Size s = this.panel4.Size; ;

            Rectangle ir = new Rectangle(0, 0, 1000, (int)(1000.0f * CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction()));
            Rectangle pr = new Rectangle(0, 0, s.Width, s.Height);
            Rectangle or = CGlobals.CalcBestFitRectagle(pr, ir, false);

            this.pictureBox1.Location = new Point(or.X, or.Y);

            s.Width = or.Width;
            s.Height = or.Height;

            this.pictureBox1.Size = s;

            GraphicsEngine ge = GraphicsEngine.Current;
            if (ge != null)
            {
                ge.ResetDefaultWindowLocationAndSize(pictureBox1.Location, pictureBox1.Size);
            }

            // to prevent flicker (when previewing slideshow), only invalidate regions outsize of the picturebox area
            if (pictureBox1.Location.X > 0)
            {
                this.pictureBox1.Parent.Invalidate(new Rectangle(0, 0, pictureBox1.Location.X, pictureBox1.Height));
                int left_block_width = pictureBox1.Location.X + pictureBox1.Width;
                this.pictureBox1.Parent.Invalidate(new Rectangle(left_block_width, 0, pictureBox1.Parent.Size.Width - left_block_width, pictureBox1.Height));
            }

            if (pictureBox1.Location.Y > 0)
            {
                this.pictureBox1.Parent.Invalidate(new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Location.Y));
                int up_block_height = pictureBox1.Location.Y + pictureBox1.Height;
                this.pictureBox1.Parent.Invalidate(new Rectangle(0, up_block_height, pictureBox1.Width, pictureBox1.Parent.Size.Height - up_block_height));
            }

            // if window state change e.g. maximised, we don't get a ResizeEnd callback, so do it now
            if (forceEngineChange == true ||
                (this.WindowState != mLastWindowState && this.WindowState != FormWindowState.Minimized))
            {
                ViewingAreaResizeEnd(this, new EventArgs());
            }
            mLastWindowState = this.WindowState;
        }

        public void ViewingAreaResizeEnd(Object o, System.EventArgs e)
        {
            // reset graphics engine to work at new resolution
            GraphicsEngine ge = GraphicsEngine.Current;
            if (ge != null)
            {
                uint w = (uint)this.pictureBox1.Size.Width;
                uint h = (uint)this.pictureBox1.Size.Height;
                if (w == ge.GetDefaultSurfaceWidth() && h == ge.GetDefaultSurfaceHeight())
                {
                    // nothing changed, no point doing a reset.
                    return;
                }

                CGlobals.mCurrentProject.DiskPreferences.SetPreviewCanvisDimensions((int)w, (int)h);
                ge.Reset(w, h);

                CGlobals.mCurrentProject.RebuildToNewCanvas(CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio);
                mDecorationManager.RePaint();
                mDecorationManager.ResizeDecorationsWindow(o, e);

            }
        }

        public TrackBar GetClipartTransparentTrackbar()
        {
            return this.ClipartTransparencyTrackerBar;
        }

        public NumericUpDown GetClipartTransparencyNumerical()
        {
            return mClipartTransparencyNumerical;
        }

        public Label GetClipartTransparentLabel()
        {
            return this.ClipartTransparencyLabel;
        }


        public TrackBar GetBackPlaneTransparentTrackbar()
        {
            return this.BackPlaneTransparencyTrackBar;
        }

        public Label GetBackPlaneTransparentLabel()
        {
            return this.BackPlaneTransparencyLabel;
        }

        public TabControl GetDecorationTabOptions()
        {
            return DecorationTabOptions;
        }

        public TabPage GetAddTextTab()
        {
            return AddTextTab;
        }

        public TabPage GetClipArtTab()
        {
            return ClipArtTab;
        }

        public Button GetAddNewImageButton()
        {
            return AddNewImageButton;
        }

        public Button GetAddClipArtButton()
        {
            return AddClipArtButton;
        }

        public Button GetEditImageButton()
        {
            return EditImageButton;
        }

        public Button GetOrderLayersButton()
        {
            return OrderLayersButton;
        }

        public ComboBox GetRenderMethodCombo()
        {
            return mRenderMethodCombo;
        }

        public ToolStripMenuItem GetShowSelectedSlideShowMenuItem()
        {
            return this.mShowSelectedSlideshowToolStripMenuItem;
        }

        public Button GetMenuAddDecorateButton()
        {
            return this.MenuAddDecorateButton;
        }

        public Button GetPreviewMenuButton()
        {
            return this.mPreviewMenuButton;
        }

        public TabPage GetPreDesignedSlidesTabPage()
        {
            return this.mPreDesignedSlidesTabPage;
        }

        public TabPage GetSelectFiltersTabPage()
        {
            return this.mFiltersTabPage;
        }

        //*******************************************************************
        public TabPage GetBackgroundTabPage()
        {
            return this.mBackgroundTabPage;
        }

        //*******************************************************************
        public TabPage GetBordersTabPage()
        {
            return this.mBordersTab;
        }

        //*******************************************************************
        public Button GetPlaySlideshowButton()
        {
            return mPlaySlideshowButton;
        }

        //*******************************************************************
        public Button GetRewindSlideshowButton()
        {
            return mRewindToStartSlideshowButton;
        }

        //*******************************************************************
        public Button GetStopSlideshowButton()
        {
            return mStopSlideshowButton;
        }

        //*******************************************************************
        private void UndoButton_Click(object sender, System.EventArgs e)
        {
            if (mSlideShowManager != null) mSlideShowManager.StopIfPlayingAndWaitForCompletion();
            GoToMainMenu();

            //			ProgressWindow pw= new ProgressWindow();
            //			pw.Text="Undo";
            //			pw.CancelButtons.Enabled=false;

            //	System.Threading.ThreadPool.QueueUserWorkItem( new System.Threading.WaitCallback(DVDSlideshow.Memento.Caretaker.GetInstance().Undo ), pw );	

            DVDSlideshow.Memento.Caretaker.GetInstance().Undo(null);

            //			pw.ShowDialog();

            this.ReEvalDoUndoButtons();
            //	this.mDecorationManager.RePaint();
            if (this.mSlideShowMusicManager != null) this.mSlideShowMusicManager.RebuildPanel();
            //	if (this.mSlideShowManager!=null) this.mSlideShowManager.RebuildPanel(null);
            if (this.mDiskMenuManager != null) this.mDiskMenuManager.RebuildLabels();
            this.ReCalcTitleBarString();
            this.ViewingAreaResize(this, new System.EventArgs());
            this.SetOutputAspectRatioComboAndVideoType();
            this.mDecorationManager.SetCurrentSlide(null);
            this.GoToMainMenu();
            this.mDecorationManager.RePaint();

        }

        //*******************************************************************
        private void RedoButton_Click(object sender, System.EventArgs e)
        {
            if (mSlideShowManager != null) mSlideShowManager.StopIfPlayingAndWaitForCompletion();
            GoToMainMenu();

            DVDSlideshow.Memento.Caretaker.GetInstance().Redo();
            this.ReEvalDoUndoButtons();
            //	this.mDecorationManager.RePaint();
            if (this.mSlideShowMusicManager != null) this.mSlideShowMusicManager.RebuildPanel();
            //	if (this.mSlideShowManager!=null) this.mSlideShowManager.RebuildPanel(null);
            if (this.mDiskMenuManager != null) this.mDiskMenuManager.RebuildLabels();
            this.ReCalcTitleBarString();
            this.ViewingAreaResize(this, new System.EventArgs());
            this.SetOutputAspectRatioComboAndVideoType();
            this.mDecorationManager.SetCurrentSlide(null);
            this.GoToMainMenu();
            this.mDecorationManager.RePaint();

        }

        // show memento stack
        private void mShowMementoStackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MementoStackViewer ms = new MementoStackViewer();
            ms.Show();
        }

        // show image cache
        private void mShowImagesCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageCacheViewer ic = new ImageCacheViewer();
            ic.Show();
        }

        //*******************************************************************
        public DVDSlideshow.Memento.Memento CreateMemento()
        {
            ArrayList handlers = new ArrayList();
            CGuiMemento m = new CGuiMemento(this, handlers);

            this.mSlideShowManager.FillInGuiMemento(m);
            this.mDecorationManager.FillInGuiMemento(m);

            m.mTabControl1Index = this.GetMainTabControl().SelectedIndex;

            return m;

        }

        //*******************************************************************
        public void SetMemento(DVDSlideshow.Memento.Memento m)
        {
            mForceNoMemento = true;

            CGuiMemento gm = m as CGuiMemento;
            if (gm == null)
            {
                CDebugLog.GetInstance().Error("SetMemento was not given an expected GuiMemento");
                return;
            }
            this.mSlideShowManager.SetMemento(gm);
            this.mDecorationManager.SetMemento(gm);

            this.GetMainTabControl().SelectedIndex = gm.mTabControl1Index;
            SetOutputAspectRatioComboAndVideoType();

            mForceNoMemento = false;

        }

        //*******************************************************************
        public bool GetForceNoMemento()
        {
            return mForceNoMemento;
        }

        //******************************************************************
        private delegate void ProjectHasChangedDelegate(bool store_memento, string description);
        public void ProjectHasChanged(bool store_memento, string description)
        {
            if (InvokeRequired == true)
            {
                BeginInvoke(new ProjectHasChangedDelegate(ProjectHasChanged),
                                            new object[] { store_memento, description });
                return;
            }

            if (store_memento == true)
            {
                // store mementos
                ArrayList orginators = new ArrayList();
                orginators.Add(this.mCurrentProject);
                orginators.Add(this);
                DVDSlideshow.Memento.Caretaker.GetInstance().Do(orginators, description);
                this.ReEvalDoUndoButtons();
            }

            ArrayList slideshows = this.mCurrentProject.GetAllProjectSlideshows(false);

            int num_slideshows = slideshows.Count;

            double running_time = 0.0;

            for (int i = 0; i < num_slideshows; i++)
            {
                CSlideShow ss = slideshows[i] as CSlideShow;
                if (ss != null)
                {
                    running_time += ss.GetLengthInSeconds();
                }
            }

            DateTime tt = new DateTime((long)(running_time * 10000000.0f));

            // Round to nearest seconds by adding 500 mili seconds
            tt += new TimeSpan(0, 0, 0, 0, 500);

            int hours = tt.Hour;
            int minutes = tt.Minute;
            int seconds = tt.Second;

            string new_label_tt = "";
            if (hours >= 1)
            {
                new_label_tt += hours + " hour";
                if (hours != 1) new_label_tt += "s";
                new_label_tt += " ";

            }

            if (minutes >= 1)
            {
                new_label_tt += minutes + " minute";
                if (minutes != 1) new_label_tt += "s";
                new_label_tt += " ";

            }

            new_label_tt += seconds + " second";
            if (seconds != 1) new_label_tt += "s";
            new_label_tt += " ";

            if (num_slideshows <= 1)
            {
                mTotalMenuStatsLabel.Visible = false;
            }
            else
            {
                mTotalMenuStatsLabel.Visible = true;
                mTotalMenuStatsLabel.Text = "Number of slideshows: " + num_slideshows + "\r\n" + "Approx total run time: " + new_label_tt;
            }

            UpdateMenuIdText();
            ReCalcTitleBarString();

        }

        //*******************************************************************
        public void UpdateMenuIdText()
        {

            CSlide s = GetDecorationManager().mCurrentSlide;
            if (s == null) return;

            CMainMenu m = CGlobals.mCurrentProject.GetMenuWhichContainsSlide(s);

            if (m == null)
            {
                return;
            }

            ArrayList menus = CGlobals.mCurrentProject.MainMenu.GetSelfAndAllSubMenus();

            int total_menus = menus.Count;
            int menu_num = 1;

            int index = 1;
            foreach (CMainMenu mm in menus)
            {
                if (mm.ID == m.ID)
                {
                    menu_num = index;
                }
                index++;
            }

            if (total_menus == 1)
            {
                mMenuIndexLabel.Visible = false;
            }
            else
            {
                mMenuIndexLabel.Visible = true;
                this.mMenuIndexLabel.Text = "Menu " + menu_num + @"/" + total_menus;
            }
        }

        //*******************************************************************
        public ComboBox GetFontSizeComboBox()
        {
            return SizeComboBox;
        }

        //*******************************************************************
        private void LeftAlignedRadioBox_CheckedChanged(object sender, System.EventArgs e)
        {
            if (this.LeftAlignedRadioBox.Checked == true)
            {
                this.mDecorationManager.SelectLeftAllignment();
            }
        }

        //*******************************************************************
        private void CentreAlignedRadioBox_CheckedChanged(object sender, System.EventArgs e)
        {
            if (this.CentreAlignedRadioBox.Checked == true)
            {
                this.mDecorationManager.SelectCentreAllignment();
            }

        }

        //*******************************************************************
        private void RightAlignedRadioBox_CheckedChanged(object sender, System.EventArgs e)
        {
            if (this.RightAlignedRadioBox.Checked == true)
            {
                this.mDecorationManager.SelectRightAllignment();
            }

        }



        //*******************************************************************
        // pick back colour
        private void ColourPickerBackPlane_Click(object sender, System.EventArgs e)
        {
            if (mSlideShowManager != null) mSlideShowManager.StopIfPlayingAndWaitForCompletion();
            this.mDecorationManager.SelectBackColorPushed();
        }

        //*******************************************************************
        public Color GetCurrentBackPlaneColor()
        {
            return this.ColourPickerBackPlane.BackColor;
        }

        //*******************************************************************
        public Button GetCurrentBackPlaneColorButton()
        {
            return this.ColourPickerBackPlane;
        }

        //*******************************************************************
        public CheckBox GetBackPlaneCheckbox()
        {
            return this.BackPlaneCheckbox;
        }

        //*******************************************************************
        public Button GetEditTextButton()
        {
            return mEditTextButton;
        }

        //*******************************************************************
        private void mCheckForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSlideShowManager != null) mSlideShowManager.StopIfPlayingAndWaitForCompletion();

            bool res = this.CheckIfOldProjectNeedsSaving();
            // Determine if text has changed in the textbox by comparing to original text.
            if (res == true)
            {
                return;
            }

            CUpdateWindow window = new CUpdateWindow();
            window.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            window.ShowDialog();
        }

        private void tabControl3_SelectedIndexChanged(object sender, System.EventArgs e)
        {

        }

        //*******************************************************************

        public ImageList GetDecoratePanZoomImageList()
        {

            return this.DecoratePanZoomImageList;
        }

        //*******************************************************************
        public TrackBar GetVideoVolumeTrackBar()
        {
            return null; //  this.mVideoVolumeTrackBar;
        }

        //*******************************************************************
        public TextBox GetVideoVolumeTextBox()
        {
            return null; // this.mVideoVolumeTextBox;
        }

        //*******************************************************************
        private void AddBackgroundMenuMusicButton_Click(object sender, System.EventArgs e)
        {
            if (mSlideShowManager != null) mSlideShowManager.StopIfPlayingAndWaitForCompletion();

            this.mDiskMenuManager.SetMenuMusic_Click(sender, e);
        }

        //*******************************************************************
        public Button GetAddBackgroundMenuMusicButton()
        {
            return this.AddBackgroundMenuMusicButton;
        }

        //*******************************************************************
        public Button GetRemoveSlideshowButton()
        {
            return this.RemoveSlideshowButton;
        }

        //*******************************************************************
        public Button GetAddNewSlideshowButton()
        {
            return this.NewSlideShow;
        }

        private void mLaunchDVDBurnerToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DVDSlideshow.CRunExternalDVDBurnTool.RunTool();
        }

        private void menuItem17_Click(object sender, System.EventArgs e)
        {
            if (mSlideShowManager != null) mSlideShowManager.StopIfPlayingAndWaitForCompletion();

            this.GetDiskMenuManager().ResetMusicButtonSelect();
        }

        private void TextGroupBox_Enter(object sender, System.EventArgs e)
        {

        }

        public Button GetPanZoomLinkStartAreaButton()
        {
            return mPanZoomLinkStartAreaButton;
        }

        private void RandomPanZoomButton_Click(object sender, System.EventArgs e)
        {
            if (mSlideShowManager != null) mSlideShowManager.StopIfPlayingAndWaitForCompletion();
            this.mDecorationManager.RandomPanZoomButton_Click();
        }

        public void ReEvalDoUndoButtons()
        {
            DVDSlideshow.Memento.Caretaker c = DVDSlideshow.Memento.Caretaker.GetInstance();

            if (c.CurrentStackIndex <= 1)
            {
                this.mUndoToolStripButton.Enabled = false;
                this.mCurrentProject.mChangedSinceSave = false;
            }
            else
            {
                this.mUndoToolStripButton.Enabled = true;
            }

            if (c.SnapshotStack.Count > c.CurrentStackIndex)
            {
                this.mRedoToolStripButton.Enabled = true;
            }
            else
            {
                this.mRedoToolStripButton.Enabled = false;
            }
        }

        private void RotateRightLabel_Click(object sender, System.EventArgs e)
        {

        }

        //*******************************************************************
        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Console.WriteLine("FORM CLOSING");
            if (mSlideShowManager != null) mSlideShowManager.StopIfPlayingAndWaitForCompletion();
            CustomButton.MiniPreviewController.StopAnyPlayingController();

            bool res = this.CheckIfOldProjectNeedsSaving();
            // Determine if text has changed in the textbox by comparing to original text.
            if (res == true)
            {
                // Cancel the Closing event from closing the form.
                e.Cancel = true;
                // Call method to save file...
            }

            if (mHelpProcess != null)
            {
                mHelpProcess.Kill();
                mHelpProcess = null;
            }

        }

        //Define these variables
        #region Dll Definition

        [DllImport("user32.dll")]
        public static extern
            bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern
            bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9;

        #endregion Dll Definition

        //	Use this piece of Code
        // Rather then TestApp use ur process name
        static public void BringProcessToFront(System.Diagnostics.Process pr)
        {
            if (pr != null)
            {
                IntPtr hWnd = pr.MainWindowHandle;
                // restore the window 
                ShowWindowAsync(hWnd, SW_RESTORE);
                // show the window or activate it as foreground window
                bool result = SetForegroundWindow(hWnd);
                if (!result)
                {// if u need to display message
                    //UserMessage.Show("Can not set in Foreground.");
                }
            }
            return;
        }

        //*******************************************************************
        private void HelpButton_Click(object sender, System.EventArgs e)
        {
            //	if (mSlideShowManager!=null) mSlideShowManager.StopIfPlayingAndWaitForCompletion();

            if (mHelpProcess != null)
            {
                BringProcessToFront(mHelpProcess);
                return;
            }

            System.Threading.ThreadStart ts = new System.Threading.ThreadStart(OpenHelpchmfile);
            System.Threading.Thread t = new System.Threading.Thread(ts);
            t.Start();

        }

        //*******************************************************************
        private void OpenHelpchmfile()
        {
            try
            {
                mHelpProcess = new System.Diagnostics.Process();
                mHelpProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                mHelpProcess.StartInfo.FileName = CGlobals.GetRootDirectory() + "\\help.chm";
                mHelpProcess.Start();
                mHelpProcess.WaitForExit();
            }
            catch
            {
            }
            mHelpProcess = null;
        }

        // Associate file extension with progID, description, icon and application
        public static void Associate(string extension, string progID, string description, string icon, string application)
        {
            Registry.ClassesRoot.CreateSubKey(extension).SetValue("", progID);
            if (progID != null && progID.Length > 0)
                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(progID))
                {
                    if (description != null)
                        key.SetValue("", description);
                    if (icon != null)
                        key.CreateSubKey("DefaultIcon").SetValue("", application + ",0");
                    if (application != null)
                        key.CreateSubKey(@"Shell\Open\Command").SetValue("", application + " \"%1\"");
                }
        }

        // Return true if extension already associated in registry
        public static bool IsAssociated(string extension)
        {
            return (Registry.ClassesRoot.OpenSubKey(extension, false) != null);
        }

        [DllImport("Kernel32.dll")]
        private static extern uint GetShortPathName(string lpszLongPath, [Out] StringBuilder lpszShortPath, uint cchBuffer);

        // Return short path format of a file name
        private static string ToShortPathName(string longName)
        {
            StringBuilder s = new StringBuilder(1000);
            uint iSize = (uint)s.Capacity;
            uint iRet = GetShortPathName(longName, s, iSize);
            return s.ToString();
        }

        // puchase information
        private void mPurchaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManagedCore.CSystem.OpenBrowser(CGlobals.mPurchaseURLLink);
        }

        // visit website
        private void mVisitPhotoVidShowWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManagedCore.CSystem.OpenBrowser(CGlobals.mWebSite);
        }

        // visit tutorials
        private void mVisitTutorialseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManagedCore.CSystem.OpenBrowser("www.photovidshow.com/tutorials");
        }


        // about
        private void mAboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string logo = CGlobals.GetRootDirectory() + "\\logo\\logo.jpg";
            SplashForm.StartSplash(logo, Color.FromArgb(128, 128, 128), true);
        }

        private void mHelpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.HelpButton_Click(this, new System.EventArgs());
        }

        private void mTestCardButton_Click(object sender, System.EventArgs e)
        {
            CStillPictureSlide sps = CStillPictureSlide.GenerateTestCard();
            ArrayList slides = new ArrayList();
            slides.Add(sps);
            this.mSlideShowManager.mCurrentSlideShow.AddSlides(slides);
            this.mSlideShowManager.RebuildPanel(null);
        }

        public CheckBox GetMenuThemePlayingThumbNailsCheckBox()
        {
            return this.mMenuThemePlayingThumbnailsCheckBox;
        }

        public CheckBox GetZoomingMenuBackGroundCheckBox()
        {
            return mZoomingMenuBackGroundCheckBox;
        }

        private void groupBoxVideoTrim_Enter(object sender, System.EventArgs e)
        {

        }

        private void mImportSlideshowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSlideShowManager != null) mSlideShowManager.StopIfPlayingAndWaitForCompletion();
            GoToMainMenu();

            ImportSlideshowsForm.ImportSlideshows(false);
        }

        public System.Windows.Forms.CheckBox GetTurnOffPanZoomForSlideTickBox()
        {
            return mTurnOffPanZoomForSlideTickBox;
        }

        public Label GetStartPanDelayLabel()
        {
            return mStartPanDelayLabel;
        }

        public TrackBar GetPanZoomInitialDelayTrackBar()
        {
            return mPanZoomInitialDelayTrackBar;
        }

        public Label GetEndPanDelayLabel()
        {
            return mEndPanDelayLabel;
        }

        public TrackBar GetPanZoomEndDelayTrackBar()
        {
            return mPanZoomEndDelayTrackBar;
        }

        public ComboBox GetPanZoomEquationComboBox()
        {
            return mPanZoomUseEquationComboBox;
        }

        public CheckBox GetReCalcPanZoomTemplateCheckbox()
        {
            return mReCalcPanZoomTemplateCheckbox;
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void mPanZoomInitialDelayLabel_Click(object sender, EventArgs e)
        {

        }

        private void mStartPanDelayLabel_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void AddBlankSlideButton_Click(object sender, EventArgs e)
        {
            this.mSlideShowManager.AddBlankSlideButtonClicked(true);
        }

        private void mCreateSlideFromTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mSlideShowManager.CreateSlideFromTemplateClicked();
        }

        private void mShowTextBoundaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CGlobals.ShowTextBoundary)
            {
                CGlobals.ShowTextBoundary = false;
            }
            else
            {
                CGlobals.ShowTextBoundary = true;
            }
        }

        public Label GetTextTemplateTypeLabel()
        {
            return mTextTemplateTypeLabel;
        }

        public ComboBox GetTextTemplateTypeCombo()
        {
            return mTextTemplateTypeCombo;
        }

        public ComboBox GetTemplateAspectCombo()
        {
            return mTemplateAspectCombo;
        }

        public CheckBox GetTemplateFrameworkImageCheckbox()
        {
            return mTemplateFrameworkImageCheckbox;
        }

        public ComboBox GetTemplateImageNumberCombo()
        {
            return mTemplateImageNumberCombo;
        }

        public Button GetEffectsEditorButton()
        {
            return mEffectsEditorButton;
        }

        private void mDecorYPositionTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        public CustomButton.PredefinedSlideDesignsControl GetPreDefinedSlideDesignsControl()
        {
            return mPredefinedSlideDesignsControl1;
        }

        public CustomButton.SlideFiltersControl GetSlideFiltersControl()
        {
            return mFiltersControl;
        }

        public CustomButton.SelectBackgroundControl GetSelectBackgroundControl()
        {
            return mSelectBackgroundControl1;
        }


        public CustomButton.OverlaySelectionControl GetBordersControl()
        {
            return mOverlaySelectionControl1;
        }

        private void AddTextTab_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        public ToolStripSplitButton GetDoNarrationButton()
        {
            return mDoNarrationButton;
        }

        public ToolStripMenuItem GetAddNarrationFromAudioFileToolStripMenuItem()
        {
            return mAddNarrationFromAudioFileToolStripMenuItem;
        }

        private void mExportTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CTemplateImportExporter.ExportCurrentSlideshow();
        }

        private void mImportTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CTemplateImportExporter.ImportTemplate(this.NewSlideshowButton_Click);
            mSlideShowManager.RebuildPanel();
        }

        public ComboBox GetSelectedDecorationCombo()
        {
            return mSelectedDecorationCombo;
        }

        public NumericUpDown GetDecorationPositionTopTextBox()
        {
            return mDecorationPositionTopTextBox;
        }

        public NumericUpDown GetDecorationPositionWidthTextBox()
        {
            return mDecorationPositionWidthTextBox;
        }

        public NumericUpDown GetDecorationPositionHeightTextBox()
        {
            return mDecorationPositionHeightTextBox;
        }

        public NumericUpDown GetDecorationPositionLeftTextBox()
        {
            return mDecorationPositionLeftTextBox;
        }

        public CustomButton.SetStartEndSlideTime GetDecorationStartEndSlideTimeControl()
        {
            return mDecorationStartEndSlideTimeControl;
        }

        public CheckBox GetPanZoomOnAllCheckBox()
        {
            return mPanZoomOnAllCheckBox;
        }

        public TrackBar GetDecorationRotationTrackbar()
        {
            return mDecorationRotationTrackbar;
        }

        public NumericUpDown GetDecorationRotationNumericArrows()
        {
            return mDecorationRotationNumericArrows;
        }

        public Label GetDecorationRotationLabel()
        {
            return mDecorationRotationLabel;
        }

        private void videoForCompuerToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {

        }

        private void mTopToolStripBar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        public ToolStripButton GetSetMenuthumbnailButton()
        {
            return mSetMenuthumbnailButton;
        }

        public ToolStripButton GetCreateAndBurnButton()
        {
            return mCreateAndBurnButton;
        }

        public ToolStripButton GetCreateVideoButton()
        {
            return mCreateVideoButton;
        }

        public ToolStripSplitButton GetDVDMenuButton()
        {
            return mDVDMenuToolStripButton;
        }

        public ToolStripComboBox GetSelectSlideshowToolStripItemCombo()
        {
            return mSelectSlideshowToolStripItemCombo;
        }

        public ToolStripMenuItem GetIntroSlideshowToolStripMenuItem()
        {
            return this.mIntroSlideshowToolStripMenuItem;
        }

        public ToolStripSeparator GetDVDMenuButtonSeperator()
        {
            return mDVDMenuToolStripSeperator;
        }

        public ToolStripDropDownButton GetOutputAspectButton()
        {
            return mOutputAspectDropDownButton;
        }

        private void mDVDMenuToolStripButton_Click(object sender, EventArgs e)
        {
            mSlideShowManager.StopIfPlayingAndWaitForCompletion();

            //
            // If editing pre menu slideshow, then select first slideshow is current menu
            //
            if (mSlideShowManager.mCurrentSlideShow != null &&
                mSlideShowManager.mCurrentSlideShow == CGlobals.mCurrentProject.PreMenuSlideshow)
            {
                mDecorationManager.SelectFirstSlideshowInCurrentMenu();
            }
         
            this.GoToMainMenu();
            // force repaint
            mDecorationManager.RePaint();       
        }

        public PictureBox GetFontPreviewPictureBox()
        {
            return mFontPreviewPictureBox;
        }

        private void openExplorerWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(mSlideShowManager.OpenImagesDialog.InitialDirectory.ToString());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // clear out any old temp files
            ManagedCore.CryptoFS.ClearDFiles();

        }

        public ImageList GetGlobalMenuItemIconsImageList()
        {
            return mGlobalMenuItemIcons;
        }

        private void label5_Click(object sender, EventArgs e)
        {
        }

        public ToolStripMenuItem GetVideoFileToolStripMenuItem()
        {
            return videoFileToolStripMenuItem;
        }
        
        public ToolStripMenuItem GetDVDVideoToolStripMenuItem()
        {
            return dVDVideoToolStripMenuItem;
        }

        public ToolStripMenuItem GetBlurayVideoToolStripMenuItem()
        {
            return blurayToolStripMenuItem;
        }


        private void mUseFFDShowForMovVideos_Click(object sender, EventArgs e)
        {
            mUseFFDShowForMovVideos.Checked = !mUseFFDShowForMovVideos.Checked;         
            UserSettings.SetBoolValue("VideoSettings", "ForceUseFFDShowIfInstalledForMov", mUseFFDShowForMovVideos.Checked);           
        }

        private void mUseFFDShowForMp4Videos_Click(object sender, EventArgs e)
        {
            mUseFFDShowForMp4Videos.Checked = !mUseFFDShowForMp4Videos.Checked;
            UserSettings.SetBoolValue("VideoSettings", "ForceUseFFDShowIfInstalledForMp4", mUseFFDShowForMp4Videos.Checked);
        }

        private void mUseFFDShowForMtsVideos_Click(object sender, EventArgs e)
        {
            mUseFFDShowForMtsVideos.Checked = !mUseFFDShowForMtsVideos.Checked;
            UserSettings.SetBoolValue("VideoSettings", "ForceUseFFDShowIfInstalledForMts", mUseFFDShowForMtsVideos.Checked); 
        }

        private void mAddOrEditMusicSlidesToolStripMenuItemMenuItem_Click(object sender, EventArgs e)
        {
            mSlideShowMusicManager.EditMusicTracksClick(sender, e);
        }

        public Panel GetMenuNavigationButtonsPanel()
        {
            return mMenuNavigationButtonsPanel;
        }

        public CheckBox GetMenuNavigationPlayAllButton()
        {
            return mMenuNavigationPlayAllButton;
        }

        public CheckBox GetMenuNavigationPlayAllLoopedButton()
        {
            return mMenuNavigationPlayAllLoopedButton;
        }

        private void mMenuNavigationPlayAllButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        public CheckBox GetIncludeIntroVideoCheckBox()
        {
            return mIncludeIntroVideoCheckBox;
        }

        public Button GetDiskIntroSlideshowImportSlideshowButton()
        {
            return mDiskIntroSlideshowImportSlideshowButton;
        }

        public ComboBox GetMenuHighlightSlideshowButtonStyleCombo()
        {
            return mMenuHighlightSlideshowButtonStyleCombo;
        }

        public ComboBox GetMenuHighlightNavigationButtonStyleCombo()
        {
            return mMenuHighlightNavigationButtonStyleCombo;
        }

        public ComboBox GetMenuHighlightPlayButtonStyleCombo()
        {
            return mMenuHighlightPlayButtonStyleCombo;
        }

        public Button GetMenuHighlightColorButton()
        {
            return mMenuHighlightColorButton;
        }

        public Button GetMenuHighlightResetButton()
        {
            return mMenuHighlightResetButton;
        }

        public void mIntroSlideshowToolStripMenuItem_Click(object sender, EventArgs e)
        {
             mSlideShowManager.StopIfPlayingAndWaitForCompletion();
            SetMainTabControlToIntroSlideshowTab();
            mForceNoMenuTabChange = true;
            try
            {
                this.GoToMainMenu(true);
                this.mDiskMenuManager.DiskIntroSlideshowManager.RebuildLabels();
            }
            finally
            {
                mForceNoMenuTabChange= false;
            }
        }

        public ImageList GetDiskMenuImageList()
        {
            return mDiskMenuImageList;
        }

        private void setMaxVideoLoadedAtOnceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mSlideShowManager.StopIfPlayingAndWaitForCompletion();
            CMaxVideoPlayersCountWindow mvpcw = new CMaxVideoPlayersCountWindow();
            mvpcw.ShowDialog();
        }

        private void SetDefaultFolderLocationsMenuItem_Click(object sender, EventArgs e)
        {
            mSlideShowManager.StopIfPlayingAndWaitForCompletion();
            CSetDefaultFolderLocationsForm sdflf = new CSetDefaultFolderLocationsForm();
            sdflf.ShowDialog();
        }

        private void showVideoCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VideoCacheViewer vcv = new VideoCacheViewer();
            vcv.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Set these when the form loads:
            // Have the form capture keyboard events first.
            this.KeyPreview = true;
            // Assign the event handler to the form.
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            // Assign the event handler to the text box.
        }

        //
        // Handle global key shortcuts here
        //
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            //
            // Handle deleting decoration/slide
            //
            if (e.KeyData == Keys.Delete || e.KeyData == Keys.Back)
            {
                //
                // If we are focused on certain' text editable controls' then ignore delete command
                //
                Control focusedControl = FindFocusedControl(this);

                if (focusedControl == null)
                {
                    return;
                }

                string name = focusedControl.GetType().ToString();
                //
                //   Is is Numberical up down control
                //
                if (name == "System.Windows.Forms.UpDownBase+UpDownEdit")
                {
                    return;
                }

                //
                // Is it a combo box which is editable
                //
                ComboBox cb = focusedControl as ComboBox;
                if (cb != null && cb.DropDownStyle != ComboBoxStyle.DropDownList)
                {
                    return;
                }

                bool deletedItem = false;
                //
                // First try decorations manager to see if any thing selected.
             
                //
                if (mDecorationManager != null)
                {
                    //
                    // Ignore delete if we are text box editing
                    //
                    if (mDecorationManager.IsTextBoxEditing() == true)
                    {
                        return;  
                    }
                    deletedItem = mDecorationManager.DeleteCurrentSelectedDecoration();
                }

                if (deletedItem == false && mSlideShowManager !=null)
                {
                    ArrayList highlightedSlides = mSlideShowManager.GetHighlightedThumbnailSlides();
                    int count = 0;

                    if (highlightedSlides != null)
                    {
                        count = highlightedSlides.Count;
                    }
                  
                    if (count > 0)
                    {
                        mSlideShowManager.StopIfPlayingAndWaitForCompletion();
                        GoToMainMenu(false);
                        mSlideShowManager.DeleteHighlightedThumbnails();
                    }
                }
            }
        }

        public Control FindFocusedControl(Control control)
        {
            ContainerControl container = control as ContainerControl;
            while (container != null)
            {
                control = container.ActiveControl;
                container = control as ContainerControl;
            }
            return control;
        }
    }
}
