using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using ManagedCore;
using DVDSlideshow;
using dvdslideshowfontend;
using System.IO;
using System.Collections;
using MangedToUnManagedWrapper;
using DVDSlideshow.GraphicsEng.Direct3D;
using DVDSlideshow.GraphicsEng;

namespace PhotoCruzBurn
{
    public partial class PhotoCruzBurnMainWindow : Form
    {
        private string mRoot = "";
        private string project_name = "";
        private CDecorationsManager mDecorationManager;
        private CSlideShowManager mSlideShowManager;
        dvdslideshowfontend.ProgressWindow mProgressWindow = null;
        private System.Windows.Forms.FolderBrowserDialog dlgSelectVideoDir;

        private bool mCurrentlyErasing = false;

        public PhotoCruzBurnMainWindow()
        {
            InitializeComponent();

            this.dlgSelectVideoDir = new System.Windows.Forms.FolderBrowserDialog();

            this.Shown += new EventHandler(this.FirstStartedCallback);
        }

        // ****************************************************************************************
        private void FirstStartedCallback(object o, System.EventArgs e)
        {
            // Initialise graphics engine
            // Set up direct3D as default graphics engine
            Direct3DDevice device = new Direct3DDevice();
            Control window = PreviewPanel;
            device.Initialise(window, (uint)window.Width, (uint)window.Height, this.HiddenPanel);
            GraphicsEngine.Current = device;

            //
            // Log out graphics capabilities
            //
            if (CDebugLog.GetInstance().CreatedLogFileThisSession() == true)
            {
                CDebugLog.GetInstance().LogToLogFile(device.GetLogCapabilitiesString());
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

            CGlobals.SetUserDirectory(Environment.CurrentDirectory);
            SetUpDirectories(Environment.CurrentDirectory);

            CGlobals.mShowErrorStack = true;

            // photocruz does not need a license file to run
            ManagedCore.License.License.Valid = true;

            // Set unmanaged log callbacks
            MangedToUnManagedWrapper.CManagedErrors.SetUnmanagedCallbacks();
            DVDSlideshow.BluRay.BluRayErrors.SetCallbacks();

            CGlobals.mDefaultMenuTemplate = CGlobals.GetRootDirectory() + "\\dvd\\motion menu.mpg";

            Application.EnableVisualStyles();
            Application.DoEvents();
            string open_with_file = "";

            string[] args = System.Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                if (args[1].ToLower().EndsWith(".pds"))
                {

                    open_with_file = args[1];
                    string app_dir = System.IO.Path.GetDirectoryName(System.Environment.GetCommandLineArgs()[0]);
                    SetUpDirectories(app_dir);
                }
                else // unknown type
                {
                    MessageBox.Show("Trying to run PhotoCruz with an unknown file type.", "Application error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                    return;
                }
            }

            String appname = System.IO.Path.GetFileNameWithoutExtension(System.Environment.GetCommandLineArgs()[0]);

            System.Diagnostics.Process[] RunningProcesses = System.Diagnostics.Process.GetProcessesByName(appname);

            //
            // Set up log filename
            //
            CDebugLog.GetInstance().LogFileFileName = CGlobals.GetUserDirectory() + "\\log.txt";

            //
            // Ok if we are here we now run the editor
            //
            CDebugLog.GetInstance().LogToLogFile("Started MemoryGuard DVD burn...");

            //
            // Load program settings from the installed directory
            //
            CGlobals.LoadProgramSettingsFile();
            CDebugLog.GetInstance().LogToLogFile("Running version " + "2.3.1");

            //
            // If we created the log.txt file for the first time in this session, dump our system info as well
            //
            if (CDebugLog.GetInstance().CreatedLogFileThisSession() == true)
            {
                CSystem system = new CSystem();
                CDebugLog.GetInstance().LogToLogFile(system.GetSystemInfo());
            }

            ManagedCore.CDebugLog.GetInstance().DebugMode = CGlobals.mIsDebug;
            ManagedCore.Updater.mVersion = CGlobals.VersionNumber();

            //
            // Load user settings from user directory
            //
            PhotoCruzBurnSettings settings = new PhotoCruzBurnSettings();

#if (DEBUG)
            Application.Run(new PhotoCruzBurnMainWindow());

#else
            //	if (CGlobals.mIsDebug==true)
            //	{
            //		Application.Run(new Form1());
            //	} 
            //	else
            {
                try
                {
                    CustomExceptionHandler eh = new CustomExceptionHandler();
                    Application.ThreadException += new ThreadExceptionEventHandler(eh.OnThreadException);

                    AppDomain.CurrentDomain.UnhandledException +=
                        new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);


                    Application.Run(new PhotoCruzBurnMainWindow());
                }
                catch
                {
                    MessageBox.Show("An unexpected error has occurred!", "Application error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                }
            }
#endif
        }

        // Handles the exception event for all other threads

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception t = (Exception)e.ExceptionObject;

            bool skip = false;
            if (t.StackTrace.IndexOf("System.Windows.Forms.ChildWindow") >= 0)
            {
                skip = true;
            }

            string ss = "PhotoCruz: An unexpected error has occurred!";

            if (skip == true) ss += " (skipped) ";

            if (CGlobals.mShowErrorStack == true)
            {
                MessageBox.Show(ss + "\n \n" + t.Message + "\n\n" + t.GetType() +
                    "\n\nStack Trace:\n" +
                    t.StackTrace, "Application error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
            }
            else if (skip == false)
            {
                MessageBox.Show(ss, "Application error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
            }

        }

        internal class CustomExceptionHandler
        {
            public void OnThreadException(object sender, ThreadExceptionEventArgs t)
            {
                bool skip = false;
                if (t.Exception.StackTrace.IndexOf("System.Windows.Forms.ChildWindow") >= 0)
                {
                    skip = true;
                }

                string ss = "PhotoCruz: An unexpected error has occurred!";

                if (skip == true) ss += " (skipped) ";

                if (CGlobals.mShowErrorStack == true)
                {
                    MessageBox.Show(ss + "\n \n" + t.Exception.Message + "\n\n" + t.Exception.GetType() +
                        "\n\nStack Trace:\n" +
                        t.Exception.StackTrace, "Application error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                }
                else if (skip == false)
                {
                    MessageBox.Show(ss, "Application error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                }

                // MessageBox.Show("OnThreadException - Handling the following exception: \n \n" + t.Exception.Message);
                //throw t.Exception; 
            }
        }



        //*******************************************************************
        private void ClosingApp(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mSlideShowManager != null) mSlideShowManager.StopIfPlayingAndWaitForCompletion();
        }

        private void PhotoCruzMainWindow_Load(object sender, EventArgs e)
        {

        }

        private void PreviewPictureBox_Click(object sender, EventArgs e)
        {

        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        private void BufferFull_Click(object sender, EventArgs e)
        {

        }

        public static void SetUpDirectories(string root)
        {
            ManagedCore.IO.SetRootDirectory(root);

            // set up temp;

            try
            {
                string temp = CGlobals.GetTempDirectory();

                MangedToUnManagedWrapper.CManagedMemoryBufferCache.set_temp_directory(temp);

                if (System.IO.Directory.Exists(temp) == false)
                {
                    System.IO.Directory.CreateDirectory(temp);
                }

                temp = CGlobals.GetTempDirectory() + "\\dvdfolder";

                if (System.IO.Directory.Exists(temp) == false)
                {
                    System.IO.Directory.CreateDirectory(temp);
                }

                temp = CGlobals.GetTempDirectory() + "\\blurayfolder";

                if (System.IO.Directory.Exists(temp) == false)
                {
                    System.IO.Directory.CreateDirectory(temp);
                }


            }
            catch (Exception e2)
            {
                CDebugLog.GetInstance().Error("Setting up the initial directories threw an exception :" + e2.Message);
            }
        }


        // **********************************************************************
        // **********************************************************************
        // **********************************************************************
        // **********************************************************************
        // AUTHOR WINDOW STUFF



        public System.Windows.Forms.Timer mTimer;
        public CProjectAuthor mPA;

        private OperationWindow mOperationWindow;

        private bool mCreatingVideo = false;
        private bool mAborted = false;
        private IStopWatch mTotalGoTime;
        private IStopWatch mTimeEstimationClock = null;

        private CVideoWriter mCurrentBurnWritter = null;

        public double mLastTotalSeconsLeft = -1;
        private TimeSpan mLastETACound;
        private TimeSpan mLastMajourChange;
        private TimeSpan mLargeTimeChangeSpan;
        private double mPreBuiltBurnEstimateTime = 0.0f;

        private CGlobals.VideoType mCurrentBurnMode = CGlobals.VideoType.DVD;  // what thing we will burn
        private string mTheVideoPath;
        private float mBuildStructureTime = 30.0f;

        private MangedToUnManagedWrapper.CManagedVideoDiskManager mDeviceManager = null;

        private System.Threading.Thread mAuthorThread = null;


        private bool mDoPadding = false;
        public bool mBurnReachFinalizeStage = false;

        // hack
        private System.Windows.Forms.PictureBox pictureBox2;

        private ArrayList mWriterComboManagedDeviceList;
        private bool mDoingTimerTick = false;


        //*******************************************************************
        public void SetUpAuthorWindow(bool do_padding)
        {

            mWriterComboManagedDeviceList = new ArrayList();


            mDoPadding = do_padding;
            mBurnReachFinalizeStage = false;

            RefreshWriterCombo();

            DoHighLightUnHighlight();

            //	string vol_name =  System.IO.Path.GetFileNameWithoutExtension(this.mMainWindow.mCurrentProject.mName);

            string vol_name = "MEMORYGUARD";

            // will cause callback

            RecalcBurnType();
            RebuildComboStrings();
            DoHighLightUnHighlight();

            //STartUpDiskDetectionEvent();
        }

        //*******************************************************************
        // if we hit the go button will we burn? and what devices should be available
        private void RecalcBurnType()
        {
            CGlobals.VideoType Prev_burn_mode = mCurrentBurnMode;

            mCurrentBurnMode = CGlobals.VideoType.DVD;

            if (mCurrentBurnMode != Prev_burn_mode)
            {
                RebuildComboStrings();
                this.DoHighLightUnHighlight();
            }
        }



        //*******************************************************************	
        public void CreateDeviceManager()
        {

            BurnerProgressCallback pc = new BurnerProgressCallback(this);

            mDeviceManager = new MangedToUnManagedWrapper.CManagedVideoDiskManager(pc);
        }


        //*******************************************************************
        private delegate void RebuildComboStingsDelegate();
        private void RebuildComboStrings()
        {
            if (WriterComboBox.InvokeRequired == true)
            {
                WriterComboBox.BeginInvoke(new RebuildComboStingsDelegate(RebuildComboStrings));
                return;
            }

            mWriterComboManagedDeviceList.Clear();

            WriterComboBox.Items.Clear();
            ArrayList al = mDeviceManager.GetVideoDiskDevices();

            foreach (MangedToUnManagedWrapper.CManagedVideoDisk device in al)
            {
                // use this line to test what it's like not to have a DVD burner drive installed
                //if (device.WritesDVD()==true) continue;// srg hack

                if (this.mCurrentBurnMode != CGlobals.VideoType.DVD ||
                    device.WritesDVD() == true)
                {
                    WriterComboBox.Items.Add(device.Name());
                    mWriterComboManagedDeviceList.Add(device);
                }
            }

            // set us so we're the first item on the combo
            if (WriterComboBox.Items.Count > 0)
            {
                WriterComboBox.SelectedIndex = 0;
            }
        }

        //*******************************************************************
        private void DoActualDeviceRefresh(object o)
        {

            if (mDeviceManager == null) CreateDeviceManager();

            bool succeed = mDeviceManager.RebuildAvailbleVideoDevicesList();
            ArrayList al = mDeviceManager.GetVideoDiskDevices();

            //	DVDSlideshow.CVCDVideoWriter.SetDeviceStringFrom(al);

            RebuildComboStrings();

            mOperationWindow.End();

        }

        //*******************************************************************
        public void RefreshWriterCombo()
        {
            mOperationWindow = new OperationWindow();//true);
            mOperationWindow.BackColor = Color.White;
            mOperationWindow.DoSetText("Scanning for devices....");

            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(DoActualDeviceRefresh), null);
            mOperationWindow.ShowDialog();
        }



        //*******************************************************************
        private void Timer_Tick(object sender, System.EventArgs e)
        {
            // update eta and time text boxes

            if (mDoingTimerTick == true) return;
            mDoingTimerTick = true;

            if (this.mTotalGoTime != null)
            {
                if (mTimeEstimationClock == null)
                {
                    mTimeEstimationClock = new DVDSlideshow.PrecisionStopWatch();
                    mTimeEstimationClock.Start();
                }

                TimeSpan ged = mTimeEstimationClock.Time;
                TimeSpan tot_since_last = ged - mLastETACound;
                mLastETACound = ged;


                this.mETATextBox.Text = "";
                this.mTotalCompleteLabel2.Text = "";
                TimeSpan t = this.mTotalGoTime.Time;
                String ss;
                ss = System.String.Format("{0:d2}:{1:d2}:{2:d2}", t.Hours, t.Minutes, t.Seconds);
                this.ElapsedTimeTextBox.Text = ss;

                // re set estimation time ??
                if (t.TotalSeconds > 10 &&
                    this.mAborted == false &&
                    this.mPA != null)
                {

                    float pd = mPA.mPercentageCompete;
                    int tot = this.mPA.TotalFramesToEncode;
                    int cu = CGlobals.mCurrentProcessFrame;
                    if (cu > 100)
                    {
                        // ok quick estimate
                        // how may fps have we done on average
                        double fs = ((double)pd) / t.TotalSeconds;
                        // multiply that up to number of frames we need to do
                        double left = 100 - pd;
                        double seconds = left / fs;
                        // add 30 seconds to do final disc structure stuff 
                        seconds += mBuildStructureTime + mPreBuiltBurnEstimateTime;
                        if (mLastTotalSeconsLeft == -1)
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
                                    mLastMajourChange = ged;
                                }
                            }
                        }
                    }
                }

                // if counting, continue counting down
                if (mLastTotalSeconsLeft != -1)
                {
                    mLastTotalSeconsLeft -= tot_since_last.TotalSeconds;

                    // woops
                    if (mLastTotalSeconsLeft < 0) mLastTotalSeconsLeft = 0;

                    int sec_int = (int)mLastTotalSeconsLeft;

                    TimeSpan ts = new TimeSpan(sec_int / 60 / 60, (sec_int / 60) % 60, sec_int % 60);
                    String tss;
                    tss = System.String.Format("{0:d2}:{1:d2}:{2:d2}", ts.Hours, ts.Minutes, ts.Seconds);
                    this.mETATextBox.Text = tss;

                }
            }
            // not timing
            else
            {
                this.ElapsedTimeTextBox.Text = "";
                this.mETATextBox.Text = "";
                this.mTotalCompleteLabel2.Text = "";
            }

            if (mPA != null && this.mAborted == false)
            {
                mPA.UpdateStats();

                progressBar1.Value = (int)mPA.mPercentageCompete;
                SetCurrentProcessText(mPA.GetCurrentProcessText());

                //ProcessingFrameLabel.Text = CGlobals.mCurrentProcessFrame.ToString();
            }

            int sq2 = progressBar1.Value;
            if (sq2 == 100) sq2 = 99;

            mTotalCompleteLabel2.Text = sq2 + "%";

            mDoingTimerTick = false;


        }


        //*******************************************************************
        private delegate void SetProgressBarValueDelegate(int value);
        public void SetProgressBarValue(int value)
        {
            if (progressBar1.InvokeRequired == true)
            {
                progressBar1.BeginInvoke(new SetProgressBarValueDelegate(SetProgressBarValue), new object[] { value });
                return;
            }

            this.progressBar1.Value = value;
        }


        //*******************************************************************
        private delegate void SetBurnBufferValueDelegate(int value);
        public void SetBurnBufferValue(int value)
        {
            if (progressBar1.InvokeRequired == true)
            {
                progressBar1.BeginInvoke(new SetBurnBufferValueDelegate(SetBurnBufferValue), new object[] { value });
                return;
            }

            this.BufferFull.Value = value;
        }


        //*******************************************************************
        private void GoGoGoFromVideoFolder()
        {
            CManagedVideoDisk device = GetSelectDiskDevice();

            if (WriteToDVDcheckBox.Checked == true)
            {
                this.mTheVideoPath = this.txtDirName.Text;

                if (device == null)
                {
                    FinishCreateVideoOpetation(true);
                    return;

                }

                if (CheckForBlankMedia(mCurrentBurnMode, device) == false)
                {
                    mAborted = true;
                    FinishCreateVideoOpetation(false);
                    return;
                }


                if (DoActualBurn(device, this.mTheVideoPath, false) == true)
                {
                    if (mAborted == false)
                    {
                        //     SetProgressBar(100);
                        //      SetBufferFull(0);
                        mLastTotalSeconsLeft = 0;

                        mTotalGoTime.Stop();

                        DialogResult res = MessageBox.Show("Video burn successful!", "Operation complete",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }

            if (mPA != null) mPA.UpdateStats();
            FinishCreateVideoOpetation(true);


        }


        //*******************************************************************
        private void GoGoGoCreateFromProject()
        {
            string output_path = "";
            // if also authoring to dvd/cd make sure we have blank media inserted
            CManagedVideoDisk device = GetSelectDiskDevice();

            string source_filename = this.txtDirNameMpeg2.Text;

            if (WriteToDVDcheckBox.Checked == true)
            {
                if (CheckForBlankMedia(mCurrentBurnMode, device) == false)
                {
                    mAborted = true;
                    FinishCreateVideoOpetation(false);
                    return;
                }
            }

            this.mTimer.Start();

            if (device == null)
            {
                FinishCreateVideoOpetation(true);
                return;
            }

            try
            {

                //
                // Create a project and slideshow for the 1 video based slide
                // 
                CProject projet = new CProject();
                projet.DiskPreferences.FinalDiskCropPercent = 0;
                projet.DiskPreferences.SetToNTSC();
                projet.DiskPreferences.OutputVideoType = CGlobals.VideoType.DVD;

                //  projet.MainMenu = new CMainMenu();

                CSlideShow show = new CSlideShow("temp");
                show.FadeIn = false;
                show.FadeOut = false;
                ArrayList slides = new ArrayList();

                //
                // Is the video file we selected already a DVD mpeg2 file?
                //
                CVideoDecoration videoDecoration = new CVideoDecoration(source_filename, new RectangleF(0, 0, 1, 1), 0);

                bool videoFileAlreadyDVDFormat = false;

                if (videoDecoration.Player.IsVideoInCorrrectOutputDVDFormat() == false)
                {
                    CSlide slide = new CBlankStillPictureSlide(videoDecoration);
                    slides.Add(slide);
                }
                else
                {
                    // 
                    // Already in correct format, copy it as slideshow0 in temp 
                    //
                    CStillPictureSlide slide = new CStillPictureSlide();
                    slides.Add(slide);

                    // copy mpg file
                    string dest_filename = CGlobals.GetTempDirectory() + "\\slideshow0.mpg";

                    if (System.IO.File.Exists(dest_filename) == true)
                    {
                        ManagedCore.IO.ForceDeleteIfExists(dest_filename, true);
                    }

                    SetCurrentProcessText("Copying mpg file...");

                    System.IO.File.Copy(source_filename, dest_filename);
                    videoFileAlreadyDVDFormat = true;
                }

                show.AddSlides(slides);
                projet.AddSlideshow(show, null, false);

                // calls with output paht to "", which means use temp directory 
                mPA = new CProjectAuthor(projet, "", show);
                CGlobals.mCurrentPA = mPA;


                bool inlude_org_pictures = false;
                bool inlude_org_videos = false;
                bool ignore_menus = true;
                bool do_orig = false;

                // if we gonna burn to get a quick estimation

                if (WriteToDVDcheckBox.Checked == true)
                {
                    inlude_org_pictures = false;
                    inlude_org_videos = false;

                    // size to burn
                    long size = 0;
                    if (size == 0)
                    {
                        mPreBuiltBurnEstimateTime = 0;
                    }
                    else
                    {
                        size /= 1024;

                        // if padding and dvd make sure min size is 1gb
                        if (this.mCurrentBurnMode == CGlobals.VideoType.DVD &&
                            this.mDoPadding == true &&
                            size < 1024 * 1024)
                        {
                            size = 1024 * 1024;
                        }

                        int kilo_bytes_a_second = device.GetWriteSpeed();
                        double ss = size;
                        ss /= kilo_bytes_a_second;
                        mPreBuiltBurnEstimateTime = ss + CGlobals.mBurnLeadInandOutTime;
                    }


                    if (inlude_org_pictures == true || inlude_org_videos == true)
                    {
                        do_orig = true;
                    }
                }

                inlude_org_pictures = false;
                inlude_org_videos = false;

                if (videoFileAlreadyDVDFormat == true && mCurrentBurnMode == CGlobals.VideoType.DVD)
                {
                    mPA.mDebugEncodeVideos = false;
                    mPA.mDebugEncodeMenus = false;
                    mPA.mDebugCreateDiscFileStructure = true;
                    mPA.mDebugEncodeSlideshow1 = false;
                    mPA.mDebugEncodeSubtitles = false;
                }

                mPA.Author(inlude_org_pictures, inlude_org_videos, false, ignore_menus);

                // if Project author aborted itself then make our abort flag the same
                if (mPA.Aborted == true)
                {
                    this.mAborted = true;
                }

                if (mPA != null) mPA.UpdateStats();

                mPA = null;
                CGlobals.mCurrentPA = null;

                if (mAborted == false)
                {
                    SetProgressBarValue(100);
                    SetBurnBufferValue(0);

                    if (WriteToDVDcheckBox.Checked == true)
                    {
                        string the_path = CGlobals.GetTempDirectory() + "\\dvdfolder";

                        if (mCurrentBurnMode == CGlobals.VideoType.BLURAY)
                        {
                            the_path = CGlobals.GetTempDirectory() + "\\blurayfolder";
                        }
                        if (output_path != "")
                        {
                            the_path = output_path;
                        }

                        if (this.mCurrentBurnMode != CGlobals.VideoType.DVD)
                        {
                            the_path = CGlobals.GetTempDirectory() + "\\vcdfolder\\vcd.cue";
                            if (output_path != "")
                            {
                                the_path = output_path + "\\vcd.cue";
                            }
                        }

                        bool burn_another = true;
                        while (burn_another == true)
                        {

                            if (DoActualBurn(device, the_path, do_orig) == true)
                            {
                                mTotalGoTime.Stop();
                                DialogResult res = MessageBox.Show("Video burn successful! Burn again?", "Operation complete",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                                if (res == DialogResult.No)
                                {
                                    burn_another = false;
                                }
                                else
                                {
                                    burn_another = this.CheckForBlankMedia(mCurrentBurnMode, device);
                                }

                            }
                            else
                            {
                                burn_another = false;
                            }
                        }


                    }
                    else
                    {
                        this.mLastTotalSeconsLeft = 0;
                        mTotalGoTime.Stop();
                        SetCurrentProcessText("");
                        DialogResult res = MessageBox.Show("Video creation successful!", "Operation complete",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }

            }
            catch
            {
            }

            FinishCreateVideoOpetation(true);

        }


        //*******************************************************************
        private delegate MangedToUnManagedWrapper.CManagedVideoDisk GetSelectDiskDeviceDelegate();
        private MangedToUnManagedWrapper.CManagedVideoDisk GetSelectDiskDevice()
        {
            if (mDeviceManager == null) return null;

            if (WriterComboBox.InvokeRequired == true)
            {
                return (MangedToUnManagedWrapper.CManagedVideoDisk)WriterComboBox.Invoke(new GetSelectDiskDeviceDelegate(GetSelectDiskDevice));
            }

            ArrayList al = mDeviceManager.GetVideoDiskDevices();

            if (al.Count == 0) return null;

            int si = this.WriterComboBox.SelectedIndex;
            if (si < 0) return null;




            //	if (al.Count< si+1) return null;

            //	CManagedVideoDisk dm = (CManagedVideoDisk)al[si];


            if (mWriterComboManagedDeviceList.Count < si + 1) return null;
            return (CManagedVideoDisk)mWriterComboManagedDeviceList[si];

            //	return dm;
        }

        public delegate void ShowErasingOnGuiThreadDelegate();
        public void ShowErasingOnGuiThread()
        {
            mOperationWindow = new OperationWindow();//true);
            mOperationWindow.BackColor = Color.White;
            mOperationWindow.DoSetText("Erasing disk please wait....");
            mOperationWindow.ShowDialog(this);
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

                return (DialogResult)this.EndInvoke(aResult);
            }

            // already on correct thread
            return ShowMessageBox(message, caption, buttons, icon);

        }

        //*******************************************************************
        private int EraseCurrentSelectDisk(CManagedVideoDisk device)
        {
            if (device == null)
            {
                return 1;
            }

            return device.EraseDisk();
        }


        //*******************************************************************
        private bool CheckForBlankMedia(CGlobals.VideoType type, CManagedVideoDisk device)
        {
            if (device == null)
            {
                return false;
            }

            int blank_tries = 0;
            bool retry = true;

            while (retry == true)
            {
                retry = false;

                int tt = 0;
                if (type == CGlobals.VideoType.DVD) tt = 1;
                if (type == CGlobals.VideoType.BLURAY) tt = 2;

                int resultCode = device.ContainsABlankDisk(tt);

                if (resultCode == 3 || resultCode == 4)
                {
                    DoMessageBoxOnFormThread("Could not erase disk. Found a DVD disk but was expecting a Blu-Ray disk", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (resultCode == 2 || resultCode == 4)
                {
                    mTotalGoTime.Stop();

                    DialogResult res = DoMessageBoxOnFormThread("This disk is not blank, erase disk?", "Erase disk?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (res == DialogResult.OK)
                    {
                        this.BeginInvoke(new ShowErasingOnGuiThreadDelegate(ShowErasingOnGuiThread));
                        System.Threading.Thread.Sleep(100);
                        int erarse_disk_result = EraseCurrentSelectDisk(device);

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
            if (CurrentProcessTextBox.InvokeRequired == true)
            {
                CurrentProcessTextBox.BeginInvoke(new SetCurrentProcessTextDelegate(SetCurrentProcessText), new object[] { t });
                return;
            }

            this.CurrentProcessTextBox.Text = t;
        }

        //*******************************************************************
        private bool DoActualBurn(CManagedVideoDisk device, string path, bool store_org)
        {
            this.SetProgressBarValue(0);
            this.SetBurnBufferValue(0);

            SetCurrentProcessText("Writing lead in...");

            CVideoWriter writer = null;

            if (this.mCurrentBurnMode == CGlobals.VideoType.DVD)
            {
                writer = new CDVDVideoWriter(device, path, "MGDVD", this.mDoPadding);
            }
            else if (this.mCurrentBurnMode == CGlobals.VideoType.BLURAY)
            {
                path += "\\BDMVROOT";

                writer = new CBluRayVideoWriter(device, path, "MGBD", mDoPadding, CGlobals.VideoType.BLURAY);
              
                if (writer.CreateIso() == false)
                {
                    DoMessageBoxOnFormThread("An error occurred when creating the Blu-ray iso", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            else
            {
                // SRG get mathing device name
                BurnerProgressCallback bpc = new BurnerProgressCallback(this);
                int a = device.GetCdRecordAddressA();
                int b = device.GetCdRecordAddressB();
                int c = device.GetCdRecordAddressC();

                string name = a.ToString() + "," + b.ToString() + "," + c.ToString();
              //  writer = new CVCDVideoWriter(device, name, path, bpc);
            }

            mCurrentBurnWritter = writer;

            this.mLastTotalSeconsLeft = writer.GetBurnTimeEstimation();

            // get speed we'll write at

            int bs = writer.BurnFromPath(store_org);
            bool sucess = false;
            if (bs == 1) sucess = true;

            mCurrentBurnWritter = null;
            SetCurrentProcessText("");

            // ok for a burn failure to occur a sucess has to be false
            // and also because starburn sometimes says there was a problem
            // when there wasn't it also has to have not reached the finalize
            // stage

            if (sucess == false && mBurnReachFinalizeStage == false)
            {
                this.SetProgressBarValue(0);
                this.SetBurnBufferValue(0);

                mLastTotalSeconsLeft = 0;

                mTotalGoTime.Stop();
                DialogResult res = MessageBox.Show("An error has occurred in the burn process!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }
            else
            {
                this.SetProgressBarValue(100);
                this.SetBurnBufferValue(0);
                mLastTotalSeconsLeft = 0;
            }


            return true;
        }

        //*******************************************************************
        private delegate void FinishCreateVideoOpetationDelegate(bool show_abort_message);
        private void FinishCreateVideoOpetation(bool show_abort_message)
        {
            if (mETATextBox.InvokeRequired == true)
            {
                mETATextBox.BeginInvoke(new FinishCreateVideoOpetationDelegate(FinishCreateVideoOpetation), new object[] { show_abort_message });
                return;
            }

            ManagedCore.CDebugLog.GetInstance().Trace("called FinishCreateVideoOpetation");


            this.mTimer.Stop();
            this.mTotalGoTime.Stop();
            this.ElapsedTimeTextBox.Text = "";
            this.mETATextBox.Text = "";
            this.mTotalCompleteLabel2.Text = "";

            this.mCreatingVideo = false;
            this.DoHighLightUnHighlight();
            this.SetCurrentProcessText("");
            if (mTimeEstimationClock != null)
            {
                mTimeEstimationClock.Stop();
            }

            if (mAborted == true)
            {
                this.progressBar1.Value = 0;
                this.BufferFull.Value = 0;
                mAborted = false;
                if (show_abort_message == true)
                {
                    DialogResult res = MessageBox.Show("Operation was aborted!", "Aborted",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                return;

            }
            else
            {

                ManagedCore.CDebugLog.GetInstance().Trace("Closing window");

                this.Close();
            }

            this.progressBar1.Value = 0;
            this.BufferFull.Value = 0;
        }


        //*******************************************************************
        private void Go_Click(object sender, System.EventArgs e)
        {

            mLastTotalSeconsLeft = -1;
            this.mCreatingVideo = true;
            mTimeEstimationClock = null;
            mPreBuiltBurnEstimateTime = 0.0;
            mLastETACound = new TimeSpan(0, 0, 0);	// last time we did a eta checkup
            mLastMajourChange = new TimeSpan(0, 0, 0);	// last time a large changed occured
            mLargeTimeChangeSpan = new TimeSpan(0, 0, 10);// how oftern a large change is allowed
            this.DoHighLightUnHighlight();

            mTimer = new System.Windows.Forms.Timer();

            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;

            progressBar1.Value = 0;

            mTimer.Interval = 100;
            MangedToUnManagedWrapper.CDVDEncode.ResetProgressState();

            if (mPA != null)
            {
                mPA = null;
            }
            mDoingTimerTick = false;
            mTimer.Tick += new System.EventHandler(this.Timer_Tick);
            mTimer.Start();

            mTotalGoTime = new StopWatch();

            mTotalGoTime.Start();


            mAuthorThread = null;


            System.Threading.ThreadStart s = null;

            if (this.txtDirNameMpeg2.Text != "")
            {
                s = new System.Threading.ThreadStart(this.GoGoGoCreateFromProject);
            }
            else
            {
                s = new System.Threading.ThreadStart(this.GoGoGoFromVideoFolder);
            }

            mAuthorThread = new Thread(s);
            mAuthorThread.Start();

            if (mAuthorThread != null)
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
        private void DoHighLightUnHighlight()
        {

            bool go_will_burn = false;

            this.Go.Enabled = true;

            if (mCreatingVideo == true)
            {

                this.Go.Enabled = true;
                this.Go.Text = "Abort";

                this.WriterComboBox.Enabled = false;
                WriteToDVDcheckBox.Enabled = false;

            }
            else
            {
                this.Go.Enabled = true;
                this.Go.Text = "Burn";

                this.WriterComboBox.Enabled = true;
                WriteToDVDcheckBox.Enabled = true;

                this.Go.Enabled = true;
                go_will_burn = true;

            }

            // nothing to burn with , disable go
            if (go_will_burn == true &&
                this.WriterComboBox.Items.Count == 0)
            {
                this.Go.Enabled = false;
            }

            if (this.txtDirName.Text == "" && this.txtDirNameMpeg2.Text == "")
            {
                this.Go.Enabled = false;
            }


            // allow us to create in temp folder and not burn when debugging.
            if (CGlobals.mIsDebug == true)
            {
                this.Go.Enabled = true;
            }

        }

        //*******************************************************************
        public ProgressBar GetBufferFull()
        {
            return this.BufferFull;
        }

        private void btnSelectVideoDir_Click(object sender, EventArgs e)
        {
            // Clear previous selection in dialog
            dlgSelectVideoDir.Reset();

            // Initialize dialog
            dlgSelectVideoDir.Description = "Select VIDEO_TS or BDMVROOT source directory";
            dlgSelectVideoDir.ShowNewFolderButton = false;

            if (dlgSelectVideoDir.ShowDialog() != DialogResult.OK) { return; }

            txtDirName.Text = dlgSelectVideoDir.SelectedPath;

            bool is_valid_dir = false;

            string the_chosen_burn_path = txtDirName.Text;

            if (the_chosen_burn_path.ToUpper().EndsWith("VIDEO_TS") == true &&
                the_chosen_burn_path.Length > 9)
            {

                the_chosen_burn_path = the_chosen_burn_path.Substring(0, the_chosen_burn_path.Length - 9);
                txtDirName.Text = the_chosen_burn_path;
                is_valid_dir = true;
            }

            if (is_valid_dir == false)
            {
                try
                {
                    String[] dirs = System.IO.Directory.GetDirectories(the_chosen_burn_path);

                    foreach (string ss in dirs)
                    {
                        string fn = System.IO.Path.GetFileName(ss);
                        if (fn.ToUpper() == "VIDEO_TS")
                        {
                            txtDirName.Text = the_chosen_burn_path;
                            is_valid_dir = true;

                        }
                    }
                }
                catch (Exception ee)
                {
                }
            }

            if (is_valid_dir == false)
            {
                if (the_chosen_burn_path.ToUpper().EndsWith("BDMVROOT") == true & the_chosen_burn_path.Length > 9)
                {
                    the_chosen_burn_path = the_chosen_burn_path.Substring(0, the_chosen_burn_path.Length - 9);
                    txtDirName.Text = the_chosen_burn_path;
                    is_valid_dir = true;
                    mCurrentBurnMode = CGlobals.VideoType.BLURAY;
                }
            }
            else
            {
                mCurrentBurnMode = CGlobals.VideoType.DVD;
            }
     

            if (is_valid_dir == false)
            {
                string old_dir = txtDirName.Text;
                txtDirName.Text = "";
                DialogResult res = MessageBox.Show("The directory '" + old_dir + "' does not contain a DVD folder 'VIDEO_TS' or a Blu-ray 'BDMVROOT' ", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                this.txtDirNameMpeg2.Text = "";
            }
            DoHighLightUnHighlight();

        }

        private void PhotoCruzBurnMainWindow_Shown(object sender, EventArgs e)
        {

#if (DEBUG)
            SetUpAuthorWindow(false);
#else
            SetUpAuthorWindow(true);
#endif
        }

        private void Go_Click_2(object sender, EventArgs e)
        {
            if (mCreatingVideo == false)
            {
                this.Go_Click(sender, e);
                return;
            }

            if (mCreatingVideo == true)
            {
                if (mAborted == true) return;

                if (mCurrentBurnWritter != null)
                {
                    DialogResult res = MessageBox.Show("Are you sure you wish to abort the burn process? Disk may be unusable after aborting!", "Abort Burn Process",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                    if (res == DialogResult.Cancel)
                    {
                        return;
                    }

                    mCurrentBurnWritter.AbortBurn();
                }

                // are we current creating the video
                if (this.mPA != null)
                {
                    this.mPA.Abort();
                }

                SetCurrentProcessText("Aborting...");
                mAborted = true;
            }
            else
            {
                this.Close();
            }
        }


        private void btnSelectMpegDir_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();

            d.Filter = CGlobals.GetVideoOnlyFileDialogFilter();
            d.Title = "Open video file";

            if (d.ShowDialog().Equals(System.Windows.Forms.DialogResult.OK))
            {
                this.txtDirNameMpeg2.Text = d.FileName;
                this.txtDirName.Text = "";
            }
            DoHighLightUnHighlight();
        }

        private void txtDirNameMpeg2_TextChanged(object sender, EventArgs e)
        {

        }

        private void mAuthorGroupBox_Enter(object sender, EventArgs e)
        {

        }
    }
}
