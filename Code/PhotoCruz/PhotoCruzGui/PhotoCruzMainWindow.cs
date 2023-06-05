using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using PhotoCruz;
using dvdslideshowfontend;
using System.IO;
using System.Collections;
using MangedToUnManagedWrapper;
using DVDSlideshow;
using ManagedCore;
using DVDSlideshow.GraphicsEng;
using DVDSlideshow.GraphicsEng.Direct3D;

namespace PhotoCruzGui
{
    public partial class PhotoCruzMainWindow : Form
    { 
        private const int mPreviewWindowHeight = 470;
        private const int mPreviewWindowStartWidth = 500;
        private const int mEncodeWindowHeight = 276;
        private const int mEncodeWindowWidth = 508;


        private int [] mWidescreenWidthResolutions = { 1920, 1600, 1366, 1280, 1136, 854, 640, 484, 432, 426  };
        private int[] mWidescreenHeightResolutions = { 1080, 900,  768,  720,  640,  480, 360, 272, 240, 240 };

        private int[] mStandardWidthResolutions =  { 1600, 1280, 1024, 800, 640, 320 };
        private int[] mStandardHeightResolutions = { 1200, 960,  768,  600, 480, 240 };

        private bool mUse4by3Resolutions = false;

		private string mRoot = "";
		private string project_name ="";
        private CDecorationsManager mDecorationManager;
        private CSlideShowManager mSlideShowManager;
        dvdslideshowfontend.ProgressWindow mProgressWindow = null;
        private ConverterType mConverterType;

        private ErrorLog mErrorLog = new ErrorLog();


        public string Root
        {
            get { return mRoot; }
        }

        public string ProjectName
        {
            get { return project_name; }
        }

        public PhotoCruzMainWindow()
        {
        }

        public PhotoCruzMainWindow( ConverterType type )
        {
            mConverterType = type;
         
            InitializeComponent();
            this.mHiddenPanel.Visible = false;

#if (DEBUG)
            CGlobals.mIsDebug = true;
            ManagedCore.CDebugLog.GetInstance().DebugMode = true;
            mErrorLog.StartShow();
            ManagedCore.Log.Info("Running PhotoCruz");

            ImageCacheViewer ic = new ImageCacheViewer();
            ic.Show();
#endif

            this.Size = new Size(mPreviewWindowStartWidth, mPreviewWindowHeight);

            if (type == ConverterType.LIVCREATOR_COVERTER)
            {
                this.Text = "LivCreator";
            }

            GenerateComboBox.Items.Add("DVD video disk with menus");
            GenerateComboBox.Items.Add("Main slideshow video only");
            GenerateComboBox.Items.Add("Blu-ray video disk with menus");
            GenerateComboBox.Text = GenerateComboBox.Items[0] as string;
            GenerateComboBox.SelectedValueChanged += new EventHandler(GenerateComboBox_SelectedValueChanged);

            SizeComboBox.Items.Add("1920 x 1080 Widescreen (HD 1080p)");
            SizeComboBox.Items.Add("1600 x 900  Widescreen (HD)");
            SizeComboBox.Items.Add("1366 x 768  Widescreen (HD)");
            SizeComboBox.Items.Add("1280 x 720  Widescreen (HD 720p)");       
            SizeComboBox.Items.Add("1136 x 640  Widescreen (Mobile/Computer)");
            SizeComboBox.Items.Add("854 x 480   Widescreen (Web/Mobile)");
            SizeComboBox.Items.Add("640 x 360   Widescreen (Web/Mobile)");
            SizeComboBox.Items.Add("484 x 272   Widescreen (Web/Mobile)");
            SizeComboBox.Items.Add("432 x 240   Widescreen (Web/Mobile)");
            SizeComboBox.Items.Add("426 x 240   Widescreen (Web/Mobile)");
            SizeComboBox.SelectedIndex = 3;

            // set to good as qaulity
            mQualityCombo.SelectedIndex = 1;

            // set to 25 fps
            mFpsCombo.SelectedIndex = 4;

            mAuthorGroupBox.Location = new Point(0,700);
       //     ManagedCore.CDebugLog.mTraceEncode = true;
       //     ManagedCore.CDebugLog.mLogEncode = true;

            this.Closing += new System.ComponentModel.CancelEventHandler(this.ClosingApp);

            this.DoneButton.Click += new EventHandler(this.DoneButton_Click);

            this.PreviewPictureBox.Visible = false;

            CDebugLog.GetInstance().Trace("Creating CDecorationsManager 12");
            mDecorationManager = new CDecorationsManager(null, this.PreviewPanel, this.PreviewPictureBox, null, null);
            mDecorationManager.mDisableMenuTextEditing = true;
        
            mSlideShowManager = new CSlideShowManager(this.PCPlayButton, 
                                                      this.StopButton, 
                                                      this.SeekTrackBar,
                                                      this.mDecorationManager,
                                                      this.SeekTimeLabel,
                                                      this.GotoMainMenu,
                                                      this);

            mSlideShowManager.PreviewSlideshowFramesWhenStopped = false;


            this.Shown += new EventHandler(this.FirstStartedCallback);
        }

        // ****************************************************************************************   
        void ChangeResolutionOptionsTo4by4Apect()
        {
            SizeComboBox.Items.Clear();

            SizeComboBox.Items.Add("1600 x 1200 Standard");
            SizeComboBox.Items.Add("1280 x 960 Standard");
            SizeComboBox.Items.Add("1024 x 768 Standard");
            SizeComboBox.Items.Add("800 x 600 Standard");
            SizeComboBox.Items.Add("640 x 480 Standard");
            SizeComboBox.Items.Add("320 x 240 Standard");
            SizeComboBox.SelectedIndex = 3;
            mUse4by3Resolutions = true;
        }


        // ****************************************************************************************   
        void GenerateComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            int index = GenerateComboBox.SelectedIndex;
            //
            // Disk output
            //
            if (index == 0 || index ==2)
            {
                mWriterLabel.Visible = true;
                WriterComboBox.Visible = true;
                WriteToDVDcheckBox.Visible = true;
                WriteToDVDcheckBox.Checked = true;
                mSizeLabel.Visible = false;
                SizeComboBox.Visible = false;
                mQualityCombo.Visible = false;
                mQualityLabel.Visible = false;
                mFpsCombo.Visible = false;
                mFpsLabel.Visible = false;

            }
            else
            {
                mWriterLabel.Visible = false;
                WriterComboBox.Visible = false;
                WriteToDVDcheckBox.Visible = false;
                WriteToDVDcheckBox.Checked = false;
                mSizeLabel.Visible = true;
                SizeComboBox.Visible = true;
                mQualityCombo.Visible = true;
                mQualityLabel.Visible = true;
                mFpsCombo.Visible = true;
                mFpsLabel.Visible = true;
            }

            this.RecalcBurnType();
        }

        // ****************************************************************************************
        private void FirstStartedCallback(object o, System.EventArgs e)
        {
            mProgressWindow = new ProgressWindow(LoadPhotoCruzProject, null);//true);
            mProgressWindow.CancelButtons.Enabled = false;
            mProgressWindow.Text = "Loading...";

            bool cancelled = mProgressWindow.Cancelled;
            mProgressWindow.ShowDialog();
           
            mProgressWindow = null;

            if (cancelled == true)
            {
                this.Close();
                return;
            }

            if (CGlobals.mCurrentProject == null ||
                CGlobals.mCurrentProject.GetAllProjectSlideshows(false).Count <=0) return;

            ArrayList sshows = CGlobals.mCurrentProject.GetAllProjectSlideshows(false);

            CSlideShow main_one = null;
            foreach (CSlideShow ss in sshows)
            {
                if (ss.Name == "MainSlideshow") main_one = ss;
                break;
            }

            if (main_one == null)
            {
                mSlideShowManager.mCurrentSlideShow = (CSlideShow)sshows[sshows.Count - 1];
            }
            else
            {
                mSlideShowManager.mCurrentSlideShow = main_one;    
            }
            mSlideShowManager.ReCalcTrackerBar();

            // Initialise graphics engine
            // Set up direct3D as default graphics engine
            Direct3DDevice device = new Direct3DDevice();
            Control window = PreviewPanel;
            device.Initialise(window, (uint)window.Width, (uint)window.Height, this.mHiddenPanel);
            GraphicsEngine.Current = device;

            //
            // Log out graphics capabilities
            //
            if (CDebugLog.GetInstance().CreatedLogFileThisSession() == true)
            {
                CDebugLog.GetInstance().LogToLogFile(device.GetLogCapabilitiesString());
            }

            CGlobals.mCurrentProject.DiskPreferences.OutputVideoType = CGlobals.VideoType.DVD;

            CGlobals.mCurrentProject.DiskPreferences.SetPreviewCanvisDimensions((int)window.Width, (int)window.Height);


            this.GotoMainMenu();
            //this.mDecorationManager.SetCurrentSlide(CGlobals.mCurrentProject.MainMenu.mBackground);
         
        }

         // ****************************************************************************************
        public void LoadPhotoCruzProject(object o)
        {
            CProject project = null;
            if (mProgressWindow != null)
            {
                mProgressWindow.Begin(0, 100);
                mProgressWindow.SetText("Reading project files...");
            }

            string root_path = Environment.CurrentDirectory;

			mRoot = root_path;

            StreamReader reader = null;

            bool rc = false;
            project_name = "";

            try
            {
                reader = File.OpenText(root_path + "\\dvd\\video");

                project_name = reader.ReadLine();

                string line2 = reader.ReadLine();
                if (line2 != null)
                {
                    string line2upper = line2.ToUpper();
                    if (line2upper.Contains("RECREATE") == true)
                    {
                        rc = true;
                    }
                }

                if (rc == true)
                {
                    string pds_name = root_path + "\\memorials\\" + project_name + "\\" + project_name + ".pds";
                    if (File.Exists(pds_name) == false)
                    {
                        rc = false;
                    }
                }
            }
            catch
            {
                UserMessage.Show("Could not find video file at location:" + root_path + "\\dvd)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                if (mProgressWindow != null) this.mProgressWindow.End();
                return;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }

            // re create from pds file (PhotoCruz only)
            if (rc == true && mConverterType == ConverterType.PHOTOCRUZ_CONVERTER)
            {
                string pds_name = root_path + "\\memorials\\" + project_name + "\\" + project_name + ".pds";
                CProject p = new CProject();
                p.mFilename = pds_name;
                try
                {
                    p.Load(null);
                }
                catch (Exception)
                {
                    UserMessage.Show("There was an error whilst loading pds file:" + project_name + ".pds", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                project = p;

                if (project != null && PreviewPanel != null)
                {
                    if (p.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV16_9)
                    {
                        this.Size = new Size(6 + (int)((this.PreviewPanel.Size.Height * 1.777777f) + 0.4999f), this.Size.Height);
                    }
                }

            }
            // generate pds file
            else
            {

                string project_path = root_path + "\\memorials\\" + project_name;
                try
                {
                    Console.WriteLine("Loading project:" + project_path);
                    PhotoCruz.PCFilesToPDSFileConverter converter = new PCFilesToPDSFileConverter(root_path, project_path, project_name, this.mConverterType);

                    project = converter.Convert(this.mProgressWindow);

                    if (PreviewPanel != null)
                    {
                        if (converter.mAspect == 16.9f)
                        {
                            this.Size = new Size(6 + (int)((this.PreviewPanel.Size.Height * 1.777777f) + 0.4999f), this.Size.Height);
                        }
                        else
                        {
                            // Change resolutions options to 4:3
                            ChangeResolutionOptionsTo4by4Apect();
                        }

                        if (converter.HasChapelMenu() == true && mConverterType == ConverterType.PHOTOCRUZ_CONVERTER)
                        {
                            this.mChapelLabel.Text = "Final DVD includes initial chapel menu";
                        }
                    }
                }
                catch (PhotoCruz.PhotoCruzErrorEx error)
                {
                    UserMessage.Show(error.mErrorString + "\n\rProject=" + project_path, "Error when creating project", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);

                    if (PreviewPanel != null)
                    {
                        Close();
                    }
                }
            }

            if (project == null) return;

            CGlobals.mCurrentProject = project;

            if (mProgressWindow != null)
            {
                this.mProgressWindow.End();
            }
        }


        // ****************************************************************************************
        public delegate void GotoMainMenuDelegate();
        public void GotoMainMenu()
        {
            // correct thread
            if (this.InvokeRequired == true)
            {
                this.Invoke(new GotoMainMenuDelegate(GotoMainMenu));
                return;
            }

            CGlobals.mCurrentProject.ResetAllMediaStreams();

            CSlideShow cs = this.mSlideShowManager.mCurrentSlideShow;

            CMainMenu mm = CGlobals.mCurrentProject.GetMenuWhichContainsSlideshow(cs);

            if (mm == null) mm = CGlobals.mCurrentProject.MainMenu;
            
            if (mm.SubMenus.Count >0)
            {
            	bool chapel_menu = true;
            	CMainMenu submenu = mm.SubMenus[0] as CMainMenu;
            	if (submenu !=null)
            	{
            		ArrayList al = submenu.GetLinkButtons();
            		foreach (CDecoration d in al)
            		{
            			CMenuLinkPreviousNextButton bv = d as CMenuLinkPreviousNextButton;
            			if (bv != null)
            			{
            				if (bv.Link == CMenuLinkPreviousNextButton .LinkType.PREVIOUS_MENU)
            				{
            					chapel_menu = false;
     
            				}
            			}
            		}
            		
            		if (chapel_menu==true)
            		{
            			mm = submenu;
            		}
            		
            	}
            }

            if (this.mConverterType == ConverterType.PHOTOCRUZ_CONVERTER)
            {
                this.mDecorationManager.SetCurrentSlide(mm.BackgroundSlide);
            }
            else if (this.mConverterType == ConverterType.LIVCREATOR_COVERTER)
            {
                if (blank_menu_image == null)
                {
                    blank_menu_image = new Bitmap(300, 300);

                    Graphics g = Graphics.FromImage(blank_menu_image);
                    Brush br = new SolidBrush(Color.Black);
                    g.FillRectangle(br, 0, 0, blank_menu_image.Width, blank_menu_image.Height);
                    g.Dispose();
                }

                CImage i = new CImage(blank_menu_image);
                CStillPictureSlide sps = new CStillPictureSlide(i);
                this.mDecorationManager.SetCurrentSlide(sps);
            }
        }

        private static Bitmap blank_menu_image = null ;

       



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


        private void CreateButton_Click(object sender, EventArgs e)
        {
            CSlideShowManager.Instance.StopIfPlayingAndWaitForCompletion();
#if (DEBUG)
            SetUpAuthorWindow(false);
#else
            SetUpAuthorWindow(true);
#endif

            //    this.SuspendLayout();
            this.PreviewPanel.Location = new Point(0, 700);
            this.mAuthorGroupBox.Location = new Point(4, 0);
            before_burn_width = this.Size.Width;
            this.Size = new Size(mEncodeWindowWidth, mEncodeWindowHeight);
            this.CreateButton.Visible = false;
            this.PhotoCruzCancelButton.Visible = false; ;
            this.SeekTrackBar.Visible = false; ;
            this.StopButton.Visible = false; ;
            this.PCPlayButton.Visible = false; ;
            this.SeekTimeLabel.Visible = false;
            //    this.ResumeLayout();


        }

        private int before_burn_width;

        private void mPreviousWizard3_Click(object sender, EventArgs e)
        {
            this.SuspendLayout();
            this.PreviewPanel.Location = new Point(0, 0);
            this.mAuthorGroupBox.Location = new Point(0, 700);
            this.Size = new Size(before_burn_width, mPreviewWindowHeight);
            this.CreateButton.Visible = true;
            this.PhotoCruzCancelButton.Visible = true; ;
            this.SeekTrackBar.Visible = true; ;
            this.StopButton.Visible = true; ;
            this.PCPlayButton.Visible = true; ;
            this.SeekTimeLabel.Visible = true; ;
            this.ResumeLayout();
            this.Text = "PhotoCruz";
        }

        private void BufferFull_Click(object sender, EventArgs e)
        {

        }


    
        // **********************************************************************
        // **********************************************************************
        // **********************************************************************
        // AUTHOR WINDOW STUFF



		public System.Windows.Forms.Timer  mTimer ;
		public CProjectAuthor mPA;

		private OperationWindow mOperationWindow;

		private bool mCreatingVideo = false;
		private bool mAborted = false;
		private IStopWatch mTotalGoTime;
		private IStopWatch mTimeEstimationClock=null;

		private CVideoWriter mCurrentBurnWritter = null;
	
		public double mLastTotalSeconsLeft=-1;
		private TimeSpan mLastETACound;
		private TimeSpan mLastMajourChange;
		private TimeSpan mLargeTimeChangeSpan;
		private double	mPreBuiltBurnEstimateTime=0.0f;

        private CGlobals.VideoType mCurrentBurnMode = CGlobals.VideoType.VCD;  // what thing we will burn

		private float mBuildStructureTime=30.0f;
	
		private MangedToUnManagedWrapper.CManagedVideoDiskManager mDeviceManager= null;

		private System.Threading.Thread mAuthorThread=null;

		private bool mDoPadding = false;
		public bool mBurnReachFinalizeStage = false;
	

		private ArrayList mWriterComboManagedDeviceList;
		private bool mDoingTimerTick = false;

	
		//*******************************************************************
		public void SetUpAuthorWindow(bool do_padding)
		{
		
			mWriterComboManagedDeviceList = new ArrayList();

		
			mDoPadding= do_padding;
			mBurnReachFinalizeStage=false;

#if (!DEBUG)
        this.WriteToDVDcheckBox.Checked=true;
        this.BuildDVDonly.Visible = false;
#endif

            this.mAuthorLabel.Text = "Author Video";
	
            if (this.mConverterType == ConverterType.LIVCREATOR_COVERTER)
            {
                this.WriteToDVDcheckBox.Checked = false;
                this.BuildDVDonly.Visible = false;
                this.WriteToDVDcheckBox.Visible = false;
                this.WriterComboBox.Visible = false;
                this.BufferFull.Visible = false;
                this.mAuthorLabel.Text = "Author Slideshow Video";
                this.mAuthorLabel.Width = 200;
                this.BufferLabel.Visible = false;
                this.mWriterLabel.Visible = false;        
            }

            CGlobals.mCurrentProject.DiskPreferences.FinalDiskCropPercent = 0;

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

            if (this.mConverterType == ConverterType.PHOTOCRUZ_CONVERTER)
            {
                RefreshWriterCombo();

                DoHighLightUnHighlight();

                // will cause callback

                RecalcBurnType();
                RebuildComboStrings();
            }

			DoHighLightUnHighlight();

			//STartUpDiskDetectionEvent();
		}

		//*******************************************************************
		// if we hit the go button will we burn? and what devices should be available
		private void RecalcBurnType()
		{
            CGlobals.VideoType Prev_burn_mode = mCurrentBurnMode;

            if (GenerateComboBox.SelectedIndex == 2)
            {
                mCurrentBurnMode = CGlobals.VideoType.BLURAY;
            }
            else
            {
                mCurrentBurnMode = CGlobals.VideoType.DVD;
            }

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

			mDeviceManager = new  MangedToUnManagedWrapper.CManagedVideoDiskManager(pc);
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
					device.WritesDVD()==true)
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
		
			if (mDeviceManager==null) CreateDeviceManager();
		
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
			mOperationWindow.DoSetText("Scanning for devices....");
		
			System.Threading.ThreadPool.QueueUserWorkItem( new System.Threading.WaitCallback(DoActualDeviceRefresh ), null );
			mOperationWindow.ShowDialog();
		}

	

		//*******************************************************************
		private void Timer_Tick(object sender, System.EventArgs e)
		{
			// update eta and time text boxes

			if (mDoingTimerTick==true) return ;
			mDoingTimerTick=true;

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

				progressBar1.Value = (int) mPA.mPercentageCompete;
				SetCurrentProcessText(mPA.GetCurrentProcessText());

				int sq = progressBar1.Value;
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
        private delegate void SetProgressBarValueDelegate(int value);
        public void SetProgressBarValue(int value)
        {
            if (progressBar1.InvokeRequired==true)
            {
                progressBar1.BeginInvoke(new SetProgressBarValueDelegate(SetProgressBarValue), new object [] {value});
                return;
            }

	        this.progressBar1.Value=value;
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
		private void GoGoGoCreateFromProject()
		{
            string output_path = "";
			// if also authoring to dvd/cd make sure we have blank media inserted
			CManagedVideoDisk device = GetSelectDiskDevice();

			if (this.WriteToDVDcheckBox.Checked==true)
			{
				if (device==null)
				{	
					FinishCreateVideoOpetation(true);
					return ;
				}

				if (CheckForBlankMedia(mCurrentBurnMode, device) ==false) 
				{
					mAborted=true;
					FinishCreateVideoOpetation(false);
					return ;
				}
			}

		
            // calls with output path to "", which means use temp directory

            mPA = new CProjectAuthor(CGlobals.mCurrentProject, "", mSlideShowManager.mCurrentSlideShow);
			CGlobals.mCurrentPA=mPA;

		
			bool inlude_org_pictures = false;
			bool inlude_org_videos = false;
			bool ignore_menus = false;
			bool do_orig=false;

			// if we gonna burn to get a quick estimation

            if (WriteToDVDcheckBox.Checked == true)
			{
                inlude_org_pictures = false;
                inlude_org_videos = false;

			
				// size to burn
				long size = 0;
				if (size==0) 
				{
					mPreBuiltBurnEstimateTime= 0;
				}
				else
				{
					size /=1024;

					// if padding and dvd make sure min size is 1gb
                    if (this.mCurrentBurnMode == CGlobals.VideoType.DVD &&
						this.mDoPadding==true &&
					    size <1024 *1024)
					{
						size=1024*1024;
					}

					int kilo_bytes_a_second = device.GetWriteSpeed();
					double ss = size;
					ss /=kilo_bytes_a_second;
					mPreBuiltBurnEstimateTime = ss+CGlobals.mBurnLeadInandOutTime;
				}

			
				if (inlude_org_pictures==true ||inlude_org_videos==true)
				{
					do_orig=true;
				}


			}

			//SRG REMOVE TO TEST
            inlude_org_pictures = false;
            inlude_org_videos = false;
            ignore_menus = false;

            if (this.BuildDVDonly.Checked == true)
            {
                mPA.mDebugEncodeVideos = false;
                mPA.mDebugEncodeMenus = false;
            }

            if (GenerateComboBox.SelectedIndex == 1)
            {
                CProject currentProject = CGlobals.mCurrentProject;

                if (this.mUse4by3Resolutions == false)
                {
                    currentProject.DiskPreferences.CustomVideoOutputWidth = mWidescreenWidthResolutions[SizeComboBox.SelectedIndex];
                    currentProject.DiskPreferences.CustomVideoOutputHeight = mWidescreenHeightResolutions[SizeComboBox.SelectedIndex];
                }
                else
                {
                    currentProject.DiskPreferences.CustomVideoOutputWidth = mStandardWidthResolutions[SizeComboBox.SelectedIndex];
                    currentProject.DiskPreferences.CustomVideoOutputHeight = mStandardHeightResolutions[SizeComboBox.SelectedIndex];
                }

                currentProject.DiskPreferences.OutputVideoType = CGlobals.VideoType.MP4;

                string[] fpsText = mFpsCombo.Text.Split(' ');   // first bit of text is fps

                currentProject.DiskPreferences.OutputVideoFramesPerSecond = float.Parse(fpsText[0]);
                currentProject.DiskPreferences.OutputVideoQuality = (CGlobals.MP4Quality)mQualityCombo.SelectedIndex;
      
                ignore_menus = true;
            }
            else if (GenerateComboBox.SelectedIndex == 2)
            {
                CGlobals.mCurrentProject.DiskPreferences.FinalDiskCropPercent = 0;
                CGlobals.mCurrentProject.DiskPreferences.CustomVideoOutputWidth = 1920; // 426;  //  1920;
                CGlobals.mCurrentProject.DiskPreferences.CustomVideoOutputHeight = 1080; //240;   // 1080
                CGlobals.mCurrentProject.DiskPreferences.OutputVideoType = CGlobals.VideoType.BLURAY;     // also sets to resolution above
                CGlobals.mCurrentProject.DiskPreferences.OutputVideoFramesPerSecond = 23.976f;
                CGlobals.mCurrentProject.DiskPreferences.OutputVideoQuality = CGlobals.MP4Quality.High;
                CGlobals.mCurrentProject.DiskPreferences.OutputVideoType = CGlobals.VideoType.BLURAY;
            }
            else
            {
                CGlobals.mCurrentProject.DiskPreferences.SetToNTSC();
                CGlobals.mCurrentProject.DiskPreferences.OutputVideoType = CGlobals.VideoType.DVD;
            }


            if (this.mConverterType == ConverterType.LIVCREATOR_COVERTER)
            {
                ignore_menus = true;
                mPA.mDebugCreateDiscFileStructure = false;
            }

			mPA.Author(inlude_org_pictures,inlude_org_videos, false, ignore_menus );

			// if Project author aborted itself then make our abort flag the same
			if (mPA.Aborted==true)
			{
				this.mAborted= true;
			}

			if (mPA!=null)mPA.UpdateStats();

			mPA=null;
			CGlobals.mCurrentPA=null;

			// copy over mainslideshow into slideshows folder
			if (mAborted==false)
			{
				ArrayList slideshows = CGlobals.mCurrentProject.GetAllProjectSlideshows(false);
				int ii=0;
				foreach (CSlideShow ss in slideshows)
				{
				   if (ss.Name == "MainSlideshow")
				   {
                        // if dvd copy output mpg file to slideshow folder
						string fn = CGlobals.GetTempDirectory()+"\\slideshow"+ii+".mpg";
						if (System.IO.File.Exists(fn)==true)
						{
						   try
						   {
                               string dest = mRoot + "\\memorials\\" + project_name + "\\" + project_name + ".mpg";

                               if (mConverterType == ConverterType.LIVCREATOR_COVERTER)
                               {
                                   dest = mRoot + "\\slideshows\\liv" + project_name + ".mpg";
                               }

                                ManagedCore.IO.ForceDeleteIfExists(dest, false);

                                try
                                {
                                    System.IO.File.Copy(fn, dest);
                                }
                                catch
                                {
                                }
						   }
						   catch
						   {
						   }
						}
						break;
				   }
				   ii++;
				}

                // if mp4, copy to memorial folder
                string mpeg4_fn = CGlobals.GetTempDirectory()+"\\slideshow0.mp4";
                if (System.IO.File.Exists(mpeg4_fn) == true)
                {
                    string dest = mRoot + "\\memorials\\" + project_name + "\\"+project_name+".mp4";
                    ManagedCore.IO.ForceDeleteIfExists(dest, false);
                    try
                    {
                        System.IO.File.Move(mpeg4_fn, dest);
                    }
                    catch
                    {
                    }
                }
			}
	
			if (mAborted==false)
			{
                SetProgressBarValue(100);
                SetBurnBufferValue(0);
			
                if (WriteToDVDcheckBox.Checked == true)
				{
					string the_path = CGlobals.GetTempDirectory()+"\\dvdfolder";

                    if (mCurrentBurnMode == CGlobals.VideoType.BLURAY)
                    {
                        the_path = CGlobals.GetTempDirectory() + "\\blurayfolder";
                    }
					if (output_path !="")
					{
						the_path= output_path;
					}

                    if (this.mCurrentBurnMode != CGlobals.VideoType.DVD && this.mCurrentBurnMode != CGlobals.VideoType.BLURAY)
					{
						the_path= CGlobals.GetTempDirectory()+"\\vcdfolder\\vcd.cue";
						if (output_path !="")
						{
							the_path= output_path+"\\vcd.cue";
						}
					}

                    bool burn_another = true;
                    while (burn_another == true)
                    {

                        if (DoActualBurn(device, the_path, do_orig) == true)
                        {
                            mTotalGoTime.Stop();
                            DialogResult res = UserMessage.Show("Video burn successful! Burn again?", "Operation complete",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                            if (res == DialogResult.No)
                            {
                                burn_another = false;
                            }
                            else
                            {
                                burn_another = this.CheckForBlankMedia(CGlobals.VideoType.DVD, device);
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
					this.mLastTotalSeconsLeft=0;
					mTotalGoTime.Stop();
                    SetCurrentProcessText("");
                    DialogResult res = UserMessage.Show("Video creation successful!", "Operation complete",
						MessageBoxButtons.OK,MessageBoxIcon.Information);
				}

			}

		

			FinishCreateVideoOpetation(true);
			
		} 


		//*******************************************************************
        private delegate MangedToUnManagedWrapper.CManagedVideoDisk GetSelectDiskDeviceDelegate();
		private MangedToUnManagedWrapper.CManagedVideoDisk GetSelectDiskDevice()
		{
			if (mDeviceManager==null) return null;

            if (WriterComboBox.InvokeRequired==true)
            {
                return (MangedToUnManagedWrapper.CManagedVideoDisk) WriterComboBox.Invoke(new GetSelectDiskDeviceDelegate(GetSelectDiskDevice));
            }

			ArrayList al = mDeviceManager.GetVideoDiskDevices();

			if (al.Count==0) return null;
		
			int si = this.WriterComboBox.SelectedIndex;
			if (si<0) return null;

		

				
		//	if (al.Count< si+1) return null;

		//	CManagedVideoDisk dm = (CManagedVideoDisk)al[si];


			if (mWriterComboManagedDeviceList.Count< si+1) return null;
			return (CManagedVideoDisk) mWriterComboManagedDeviceList[si];

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

			this.CurrentProcessTextBox.Text =t;
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
				writer = new CDVDVideoWriter(device, path, "PHOTOCRUZ", this.mDoPadding);
			}
            else if (this.mCurrentBurnMode == CGlobals.VideoType.BLURAY)
            {
                writer = new CBluRayVideoWriter(device, path, "PHOTOCRUZ", mDoPadding, CGlobals.VideoType.BLURAY);

                if (writer.CreateIso() == false)
                {
                    DoMessageBoxOnFormThread("An error occurred when creating the Blu-ray iso", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            else
			{
				// SRG get matching device name
				BurnerProgressCallback bpc= new BurnerProgressCallback(this);
				int a=device.GetCdRecordAddressA();
				int b=device.GetCdRecordAddressB();
				int c=device.GetCdRecordAddressC();
				
				string name = a.ToString()+","+b.ToString()+","+c.ToString();
				//writer = new CVCDVideoWriter(device,name, path, bpc);
			}

			mCurrentBurnWritter = writer;

			this.mLastTotalSeconsLeft= writer.GetBurnTimeEstimation();

			// get speed we'll write at

         	int bs = writer.BurnFromPath(store_org);
			bool sucess =false;
			if (bs==1) sucess= true;

			mCurrentBurnWritter=null;
			SetCurrentProcessText("");

			// ok for a burn failure to occur a sucess has to be false
			// and also because starburn sometimes says there was a problem
			// when there wasn't it also has to have not reached the finalize
			// stage

			if (sucess==false && mBurnReachFinalizeStage==false)
			{
                this.SetProgressBarValue(0);
                this.SetBurnBufferValue(0);

				mLastTotalSeconsLeft=0;

				mTotalGoTime.Stop();
                DialogResult res = UserMessage.Show("An error has occurred in the burn process!", "Error", MessageBoxButtons.OK,MessageBoxIcon.Error);
				
				return false;
			}
			else
			{
                this.SetProgressBarValue(100);
                this.SetBurnBufferValue(0);
				mLastTotalSeconsLeft=0;
			}


			return true;
		}

		//*******************************************************************
        private delegate void FinishCreateVideoOpetationDelegate(bool show_abort_message);
		private void FinishCreateVideoOpetation(bool show_abort_message)
		{
            if (mETATextBox.InvokeRequired==true)
            {
                mETATextBox.BeginInvoke(new FinishCreateVideoOpetationDelegate(FinishCreateVideoOpetation), new object[] { show_abort_message });
                return ;
            }

			ManagedCore.CDebugLog.GetInstance().Trace("called FinishCreateVideoOpetation");

		
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
				this.progressBar1.Value=0;
				this.BufferFull.Value=0;
				mAborted=false;
				if (show_abort_message==true)
				{
                    DialogResult res = UserMessage.Show("Operation was aborted!", "Aborted", MessageBoxButtons.OK,MessageBoxIcon.Information);
				}
				
				return ;

			}
			else
			{

				ManagedCore.CDebugLog.GetInstance().Trace("Closing window");

				this.Close();
			}

			this.progressBar1.Value=0;
			this.BufferFull.Value=0;
		}

     
		//*******************************************************************
		private void Go_Click(object sender, System.EventArgs e)
		{
            // before we start timers or anthing, strip unwanted slideshows

            if (GenerateComboBox.SelectedIndex == 1)
            {
                CProject currentProject = CGlobals.mCurrentProject;

                // strip all everything except main slideshow

                ArrayList slideShows = currentProject.GetAllProjectSlideshows(false);
                while (slideShows.Count > 1)
                {
                    bool removed_menu;
                    currentProject.RemoveSlideshow(slideShows[0] as CSlideShow, out removed_menu);
                    slideShows = currentProject.GetAllProjectSlideshows(false);
                }

                //
                // Make sure currently selected slidshow is the first one
                //
                if (slideShows.Count > 0)
                {
                    mSlideShowManager.mCurrentSlideShow = slideShows[0] as CSlideShow;
                }
            }

            if (mUseMotionBlurCheckBox.Checked == true)
            {
                CGlobals.mCurrentProject.DiskPreferences.TurnOffMotionBlur = false;
            }
            else
            {
                CGlobals.mCurrentProject.DiskPreferences.TurnOffMotionBlur = true;
            }

			mLastTotalSeconsLeft=-1;
			this.mCreatingVideo = true;
			mTimeEstimationClock=null;
			mPreBuiltBurnEstimateTime=0.0;
			mLastETACound = new TimeSpan(0,0,0);	// last time we did a eta checkup
			mLastMajourChange = new TimeSpan(0,0,0);	// last time a large changed occured
			mLargeTimeChangeSpan = new TimeSpan(0,0,10);// how oftern a large change is allowed
			this.DoHighLightUnHighlight();

			mTimer = new System.Windows.Forms.Timer();

			progressBar1.Minimum=0;
			progressBar1.Maximum=100;

			progressBar1.Value = 0;

			mTimer.Interval = 100;
			MangedToUnManagedWrapper.CDVDEncode.ResetProgressState();
			
			if (mPA!=null)
			{
				mPA=null;
			}
			mDoingTimerTick=false;
			mTimer.Tick += new System.EventHandler(this.Timer_Tick);
			mTimer.Start();

			mTotalGoTime = new DVDSlideshow.StopWatch();

			mTotalGoTime.Start();
		
			
			mAuthorThread=null ;

			
			System.Threading.ThreadStart s = new System.Threading.ThreadStart(this.GoGoGoCreateFromProject);
			mAuthorThread = new Thread(s);
			mAuthorThread.Start();
		
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
		private void DoHighLightUnHighlight()
		{
            
			bool go_will_burn=false;

			this.Go.Enabled=true;
			
			if (mCreatingVideo==true)
			{
			
				this.DoneButton.Enabled=true;
				this.DoneButton.Text="Abort";
				this.mPreviousWizard3.Enabled=false;
			//	this.AbortButton.Enabled=true;
				
				this.Go.Enabled=false;
			
				this.WriterComboBox.Enabled=false;
                this.mUseMotionBlurCheckBox.Enabled = false;
                WriteToDVDcheckBox.Enabled = false;
                this.GenerateComboBox.Enabled = false;
                this.SizeComboBox.Enabled = false;
                this.mQualityCombo.Enabled = false;
                this.mFpsCombo.Enabled = false;
			
			}
			else
			{
				
				this.DoneButton.Enabled=true;
				this.DoneButton.Text="Cancel";
			//	this.mPreviousWizard3.Enabled=true;
		//		this.AbortButton.Enabled=false;
				this.Text = "Author Video";
			
                this.Go.Enabled = true;
				go_will_burn=true;
                this.WriterComboBox.Enabled = true;
                this.mUseMotionBlurCheckBox.Enabled = true;
                WriteToDVDcheckBox.Enabled = true;

                if (this.GenerateComboBox.SelectedIndex == 0 || this.GenerateComboBox.SelectedIndex == 2) // stops us generating a dvd after aborting an mp4 (because we have stripped out other slideshows and menus by now!!!)
                {
                    this.GenerateComboBox.Enabled = true;
                }
                this.SizeComboBox.Enabled = true;
                this.mFpsCombo.Enabled = true;
                this.mQualityCombo.Enabled = true;
			}

			if (this.mAborted==true)
			{
			//	this.AbortButton.Enabled=false;
				this.Text = "Author Video";
			}


			// nothing to burn with , disable go
			if (go_will_burn==true &&
				this.WriterComboBox.Items.Count==0 &&
                WriteToDVDcheckBox.Checked == true)
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
			return this.BufferFull;
		}


		//*******************************************************************
		private void DoneButton_Click(object sender, System.EventArgs e)
		{
			if (mCreatingVideo==true)
			{

				if (mAborted==true) return ;

				if (mCurrentBurnWritter!=null)
				{
                    DialogResult res = UserMessage.Show("Are you sure you wish to abort the burn process? Disk may be unusable after aborting!", "Abort Burn Process",
						MessageBoxButtons.OKCancel,MessageBoxIcon.Warning);

					if (res == DialogResult.Cancel)
					{
						return ;
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
		private void AbortButton_Click(object sender, System.EventArgs e)
		{
			if (mAborted==true) return ;

			if (mCurrentBurnWritter!=null)
			{
                DialogResult res = UserMessage.Show("Are you sure you wish to abort the burn process? Disk may be unusable after aborting!", "Abort Burn Process",
					MessageBoxButtons.OKCancel,MessageBoxIcon.Warning);

				if (res == DialogResult.Cancel)
				{
					return ;
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

        private void Go_Click_1(object sender, EventArgs e)
        {
            this.Go_Click(sender, e);
        }

        private void WriteToDVDcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.DoHighLightUnHighlight();
        }
    }
}
