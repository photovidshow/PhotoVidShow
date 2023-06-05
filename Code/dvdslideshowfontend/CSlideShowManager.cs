//#define DESIGN_MODE

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using DVDSlideshow;
using System.Drawing;
using System.IO;
using ManagedCore;
using DVDSlideshow.GraphicsEng;
using CustomButton;
using System.Text;

namespace dvdslideshowfontend
{
    class AddSlidesObject
    {
        public string[] slides;
        public CSlide insertAfter = null;
        public CSlide insertBefore = null;
    }

	/// <summary>
	/// Summary description for CSlideShowManager.
	/// </summary>
	public class CSlideShowManager
	{
        public delegate void SlideshowPreviewFinishedCallback();

        public event SlideshowPreviewFinishedCallback SlideshowPreviewFinished;

        public int mGetFrameCount = 0;
		private object thislock = new object();
        private bool mGettingNextImage = false;
		private StoryboardComponent [] mControlsToAdd = null;

		private	ArrayList TransitionComponentCache = new ArrayList();
		private ArrayList ThumbnailCache = new ArrayList();
		private ArrayList TimeLabelCache = new ArrayList();
		private ArrayList SeperatorCache = new ArrayList();

		private Form1		mMainForm;
        private Form mGuiThread;
		public CSlideShow	mCurrentSlideShow = null;
		int					mCount =0;
		int					mSlideWidth = 180;
		TrackBar			mTrackBar;
		public bool		mSelectingTrackBar = false;
		int					mCurrentShownSeperator =-1;
		private	CVideoClock	mVideoClock = new CVideoClock();

		private	Timer		mDragScrollTimer;
        private Timer       mSeekWhenStoppedTimer;
		private System.Windows.Forms.OpenFileDialog openFileDialog;

        public OpenFileDialog OpenImagesDialog
        {
            get { return openFileDialog; }
        }

		private ProgressWindow mProgressWindow=null;

		private ArrayList mChachedSlideshowThumnailsList;
		private bool mIgnoreHighlightCallback=false;
		private Panel mTopLinePanel;
		private Panel mStoryboardToAudioLinePanel;
        private Panel mAudioLinePanel;
        private Label mTimeLabel;

        private CDecorationsManager mDecorationsManager;

		// used to enable restart after seek to be 100% correct
		private int mLastSeekFrame = 0;

        private StoryboardComponent[] mCurrentControls = null;
        private int mScrollPosition = 0;

        private Label mEmptyStoaryBoardLabel = null;
        private Label mSoundTrackLabel = null;
        private Label mNarrationLabel = null;
        private int mFrameToStopOnWhenFinished = 0;

        private bool mPreviewSlideshowFramesWhenStopped = true;

        public bool PreviewSlideshowFramesWhenStopped
        {
            get { return mPreviewSlideshowFramesWhenStopped; }
            set { mPreviewSlideshowFramesWhenStopped = value; }
        }

        public int ScrollPosition
        {
            get { return mScrollPosition; }
        }

        public int SlideWidth
        {
            get { return mSlideWidth; }
        }
        
        private double mScrollPositionDragLeavePosition = 0.0;

        // there should only ever be once instance of this
        static private CSlideShowManager mInstance = null ; 

        //*******************************************************************
        static public CSlideShowManager Instance
        {
            get { return mInstance ;}
        }


        //*******************************************************************
        // USED BY PHOTOCRUZ
        public CSlideShowManager(Button play_button,
                                 Button stop_button,
                                 TrackBar track_bar,
                                 CDecorationsManager decorations_manager,
                                 Label time_label,
                                 DoGuiActionCallbackDelegate goto_main_menu_callback,
                                 Form guiThread)
        {
            mInstance = this;

            mGuiThread = guiThread;

            this.mGotoMainMenuCallback = goto_main_menu_callback;
            this.mGotoSelectedSlideCallback = goto_main_menu_callback;

            this.mDecorationsManager = decorations_manager;
            mTrackBar = track_bar;
            mTrackBar.Scroll += new System.EventHandler(this.TrackBar_Scroll);

            this.mTimeLabel = time_label; // this.mMainForm.GetSlideShowTimeLabel();

			mChachedSlideshowThumnailsList = new ArrayList();

			mTrackBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TrackerBarMouseDown);
			mTrackBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TrackerBarMouseUp);

            mSeekWhenStoppedTimer = new Timer();
            mSeekWhenStoppedTimer.Interval = 250;
            mSeekWhenStoppedTimer.Stop();
            mSeekWhenStoppedTimer.Tick += new EventHandler(this.SeekingWhenStoppedTick);
			
			mVideoClock.Tick+=new VideoClockTickHandler(this.GetNextImage);
			mVideoClock.Stopped+=new VideoStopHandler(this.StopComplete);

            play_button.Click += new EventHandler(this.Play_Click);
            stop_button.Click += new EventHandler(this.Stop_Click);
        }


                                 
		//*******************************************************************
        // USED BY PHOTOVIDSHOW
		public CSlideShowManager(Form1 main_window)
		{
            mMainForm = main_window;
            mInstance = this;

            mGotoMainMenuCallback = main_window.GoToMainMenu;
            mGotoSelectedSlideCallback = main_window.GoToSelectedSlide;

            this.mDecorationsManager = main_window.GetDecorationManager();

			mTrackBar = main_window.GetSlideShowTrackBar();
            this.mTimeLabel = main_window.GetSlideShowTimeLabel();

		//	mTrackBar.Scroll += new System.EventHandler(this.TrackBar_Scroll);

			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			openFileDialog.Filter = CGlobals.GetTotalImageVideoFilter();

            string myPictures = DefaultFolders.GetFolder("MyPictures"); 
			openFileDialog.InitialDirectory =myPictures;
			openFileDialog.Title ="Open images or video";

            mGuiThread = main_window;
			mDragScrollTimer = new Timer();
			mDragScrollTimer.Interval = 50;
			mDragScrollTimer.Stop();
			mDragScrollTimer.Tick += new System.EventHandler(this.DragScrollTimerTick);
			mChachedSlideshowThumnailsList = new ArrayList();

            mSeekWhenStoppedTimer = new Timer();
            mSeekWhenStoppedTimer.Interval = 250;
            mSeekWhenStoppedTimer.Stop();
            mSeekWhenStoppedTimer.Tick += new EventHandler(this.SeekingWhenStoppedTick);

			// 
			// TODO: Add constructor logic here
			//

	
			InitializeComponent();

			mMainForm.GetSlideShowPanel().DragOver += new System.Windows.Forms.DragEventHandler(this.ListDragTarget_DragOver);
			mMainForm.GetSlideShowPanel().DragDrop += new System.Windows.Forms.DragEventHandler(this.ListDragTarget_DragDrop);
			mMainForm.GetSlideShowPanel().DragEnter += new System.Windows.Forms.DragEventHandler(this.ListDragTarget_DragEnter);
			mMainForm.GetSlideShowPanel().DragLeave += new System.EventHandler(this.ListDragTarget_DragLeave);
			mMainForm.GetSlideShowPanel().MouseDown +=new System.Windows.Forms.MouseEventHandler(this.MouseDown);
			mMainForm.GetSlideShowPanel().Resize+=new System.EventHandler(this.PanelResized);

			mTrackBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TrackerBarMouseDown);
			mTrackBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TrackerBarMouseUp);
             mTrackBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TrackerBarMouseMove);

#if (!DESIGN_MODE)
            ManualScrollablePanel p = mMainForm.scrollslideshowpanel ;

            if (p != null)
            {

                HScrollBar hScroll = p.GetHScrollBar();

                hScroll.Enabled = true;
                hScroll.Maximum = 0;
                hScroll.Maximum = 6;

                hScroll.SmallChange = 1;
                hScroll.LargeChange = 1;
                hScroll.Visible = true;
                hScroll.Scroll += new ScrollEventHandler(manualScrollPanel_Scroll);
            }
#endif

			mMainForm.GetRotateSlideAntiClockwiseButton().Click += new EventHandler(this.RotateAntiClockwise_Click);
			mMainForm.GetRotateSlideClockWiseButton().Click+= new EventHandler(this.RotateClockwise_Click);
            mMainForm.GetStoryboardEditItemButton().Click += new EventHandler(this.EditSlide_Click);

			mMainForm.GetRotateSlideAntiClockwiseButton().Enabled=false;
			mMainForm.GetRotateSlideClockWiseButton().Enabled=false;
            mMainForm.GetStoryboardEditItemButton().Enabled = false;


			mVideoClock.Tick+=new VideoClockTickHandler(this.GetNextImage);
			mVideoClock.Stopped+=new VideoStopHandler(this.StopComplete);

			this.mMainForm.GetPlaySlideshowButton().Click+=new EventHandler(this.Play_Click);
            this.mMainForm.GetRewindSlideshowButton().Click += new EventHandler(this.Rewind_Click);
			this.mMainForm.GetStopSlideshowButton().Click+=new EventHandler(this.Stop_Click);

			CProject.InformWhenProjectChange.Add(new ProjectChanged(this.ProjectHasChanged));

			Panel ssp = main_window.GetSlideShowPanel();

			mTopLinePanel = new Panel();
			mTopLinePanel.Left=0;
			mTopLinePanel.Width=1008;
			mTopLinePanel.Height=2;
			mTopLinePanel.Top=0;
     		mTopLinePanel.BackColor = Color.DarkGray;
			mTopLinePanel.BorderStyle = BorderStyle.None;

			mStoryboardToAudioLinePanel = new Panel();
			mStoryboardToAudioLinePanel.Left=0;
			mStoryboardToAudioLinePanel.Width=1008;
			mStoryboardToAudioLinePanel.Height=2;
			mStoryboardToAudioLinePanel.Top=124;
        	mStoryboardToAudioLinePanel.BackColor = Color.DarkGray;
			mStoryboardToAudioLinePanel.BorderStyle = BorderStyle.None;

            mAudioLinePanel = new Panel();
            mAudioLinePanel.Left = 0;
            mAudioLinePanel.Width = 1008;
            mAudioLinePanel.Height = 1;
            mAudioLinePanel.Top = 154;
            mAudioLinePanel.BackColor = Color.DarkGray; ;
            mAudioLinePanel.BorderStyle = BorderStyle.None;

			mMainForm.GetDeleteSlidesButton().Click+= new EventHandler(this.DeleteHighlightedThumbnailsCallback);
			mMainForm.GetSetMenuThumbnailButton().Click+=new EventHandler(this.SetSlideAsMenuThumbnailCallback);
		}


        public delegate void DoGuiActionCallbackDelegate();

        private DoGuiActionCallbackDelegate mGotoMainMenuCallback;
        private DoGuiActionCallbackDelegate mGotoSelectedSlideCallback;

        //*******************************************************************
        public void GotoMainMenu()
        {
            mGotoMainMenuCallback();
        }

        //*******************************************************************
        public void GotoSelectedSlide()
        {
            mGotoSelectedSlideCallback();
        }

		//*******************************************************************
        private delegate void ProjectHasChangedDelegate(bool store_momento, string ss);
		public void ProjectHasChanged(bool store_momento, string ss)
		{
            if (Form1.mMainForm !=null && Form1.mMainForm.InvokeRequired == true)
            {
                Form1.mMainForm.BeginInvoke(new ProjectHasChangedDelegate(ProjectHasChanged),
                                            new object[] { store_momento, ss });
                return;
            }

			// if deleted a slideshow removed its cached thumnails
			if (ss=="Delete Slideshow")
			{
				ArrayList to_remove = new ArrayList();
				foreach (CSlideShowManagerThumnailCache cache in mChachedSlideshowThumnailsList)
				{
					bool found=false;
					foreach (CSlideShow s in CGlobals.mCurrentProject.GetAllProjectSlideshows(true))
					{
						if (s.ID == cache.for_slideshow.ID)
						{
							found=true;
						}
					}
					if (found==false)
					{
						to_remove.Add(cache);
					}
				}
				foreach (CSlideShowManagerThumnailCache cache in to_remove)
				{
					mChachedSlideshowThumnailsList.Remove(cache);
					this.RemoveStoryBoardUserControls(cache.usercontrols);
				}
			}

			// if new project has been loaded clear thumnail cache
			if (ss=="Load project" || 
				ss=="New project" || 
				ss=="Rebuild to new canvas" || 
				ss=="Project DeSerialise")
			{
				foreach (CSlideShowManagerThumnailCache cache in mChachedSlideshowThumnailsList)
				{
					RemoveStoryBoardUserControls(cache.usercontrols);
				}

				this.mChachedSlideshowThumnailsList.Clear();
			}

			ReCalcTrackerBar();
		}


			
		//*******************************************************************
		public void RotateAntiClockwise_Click(object o, System.EventArgs e)
		{
            ArrayList slides = GetHighlightedThumbnails();

			this.StopIfPlayingAndWaitForCompletion();

            bool doneRotation = false;

			foreach (Thumbnail tn in slides)
			{
                if (tn.mSlide.SimpleOrientateChangeAniClockwise() == true)
                {
                    doneRotation = true;
                    tn.InvalidateImage();
                }	
			}

            if (doneRotation == true)
            {
                CGlobals.mCurrentProject.DeclareChange(true, "Anti-Clockwise Rotate");
                this.mMainForm.GetDecorationManager().RePaint();
            }
		}

        //*******************************************************************
        //
        // This function insures that all slides have or have not the correct CM image on them
        //
        public void ValidateAllCMThumbnails()
        {
            ArrayList thumbnailsList = GetAllThumbnails();

            bool useCMMarkers = true;
            if (this.mCurrentSlideShow.ChapterMarkersTypeToUse != CSlideShow.ChapterMarkersType.SetFromSlides)
            {
                useCMMarkers = false;
            }
            else if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType != CGlobals.VideoType.BLURAY &&
                     CGlobals.mCurrentProject.DiskPreferences.OutputVideoType != CGlobals.VideoType.DVD)
            {
                useCMMarkers = false;
            }

            foreach (Thumbnail tn in thumbnailsList)
            {
                bool shouldMark = tn.Slide.MarkedAsChapter && useCMMarkers;

                if (tn.ImageIsSetWithCM != shouldMark)
                {
                    tn.InvalidateImage();
                }
            }
        }

        //*******************************************************************
        public void EditSlide_Click(object o, System.EventArgs e)
        {
            ArrayList slides = GetHighlightedThumbnails();

            if (slides.Count <=0) return;

            Thumbnail thumbnail = slides[0] as Thumbnail;
 
            CImageSlide firstSlide = thumbnail.mSlide as CImageSlide;
            if (firstSlide == null) return;

            EditSlideMediaForm esmf = new EditSlideMediaForm(firstSlide, mCurrentSlideShow, null);
            esmf.ShowDialog();

            thumbnail.InvalidateImage(esmf.SlideLengthChanged);

            CGlobals.mCurrentProject.DeclareChange(true, "Edit slide media");
            this.mMainForm.GetDecorationManager().RePaint();
        }

		//*******************************************************************
		public void RotateClockwise_Click(object o, System.EventArgs e)
		{
            ArrayList slides = GetHighlightedThumbnails();

			this.StopIfPlayingAndWaitForCompletion();

            bool doneRotation = false;

			foreach (Thumbnail tn in slides)
			{
                if (tn.mSlide.SimpleOrientateChangeClockwise() == true)
                {
                    doneRotation = true;
                    tn.InvalidateImage();
                }	
			}
   
            if (doneRotation==true)
            {
			    CGlobals.mCurrentProject.DeclareChange(true,"Clockwise Rotate");
                this.mMainForm.GetDecorationManager().RePaint();
            }
		}


        //*******************************************************************
        public void StartPanelScrollTimer()
        {
            mScrollPositionDragLeavePosition = (double)mScrollPosition;
            mDragScrollTimer.Start();
        }

        //*******************************************************************
        public void SetStartDragScrollPosition()
        {
            mScrollPositionDragLeavePosition = (double)mScrollPosition;
        }


        //*******************************************************************
        public void StopPanelScrollTimer()
        {
            mDragScrollTimer.Stop();
        }

		//*******************************************************************
		public void DragScrollTimerTick(object o , System.EventArgs e)
		{
            ScrollPanelIfCursorOutsideStoryboardPanel();
            this.HideAllSepearatorsExcept(-1, true);
        }

        //*******************************************************************
        public void ScrollPanelIfCursorOutsideStoryboardPanel()
        {
            Point point = this.mMainForm.GetSlideShowPanel().PointToClient(Cursor.Position);
            double pos = this.mScrollPositionDragLeavePosition;
            Point s = new Point();

            s.X = -s.X;
            s.Y = -s.Y;

            if (point.X < 0)
            {
                double f = (double)point.X;
                if (f > -30) f = -30;
                mScrollPositionDragLeavePosition -= (((f / 7) * (f / 7))) / (mSlideWidth * 2);

                int new_pos = (int)(mScrollPositionDragLeavePosition + 0.5);
                if (new_pos < 0) new_pos = 0;

                if (new_pos != mScrollPosition)
                {
                    mScrollPosition = new_pos;
                    MoveScrollBarTo(mScrollPosition);
                }
            }

            int tt = this.mMainForm.GetSlideShowPanel().Width;
            if (point.X > tt)
            {
                double f = point.X - tt;
                if (f < 30) f = 30;
                mScrollPositionDragLeavePosition += ((f / 7) * (f / 7)) / (mSlideWidth * 2);

                int new_pos = (int)(mScrollPositionDragLeavePosition + 0.5);

                int max_scroll_pos = CalcMaxPossibleScrollPosition();

                if (new_pos > max_scroll_pos) 
                {
                    new_pos = max_scroll_pos;
                }

                if (new_pos != mScrollPosition)
                {
                    mScrollPosition = new_pos;
                    MoveScrollBarTo(mScrollPosition);
                }
            }
		}

        //*******************************************************************
        private int CalcMaxPossibleScrollPosition()
        {
            if (mMainForm==null) return 0;
#if (!DESIGN_MODE)
            return mMainForm.scrollslideshowpanel.GetHScrollBar().Maximum - (mMainForm.scrollslideshowpanel.GetHScrollBar().LargeChange - 1);
#else
            return 0;
#endif
        }

        
		//*******************************************************************
        public void TrackerBarMouseMove(object o, System.Windows.Forms.MouseEventArgs e)
        {
            if (mSelectingTrackBar == false) return;

            TrackerBarMouseDown(o, e);
        }

		//*******************************************************************
		public void TrackerBarMouseDown(object o, System.Windows.Forms.MouseEventArgs e)
		{
            float x = e.Location.X -14 ;

            float w = mTrackBar.Width -28;

            float rat = ((float)mTrackBar.Maximum) / w;
            float xx = x* rat;

            int ix = (int)(xx + 0.4999f);

            mSelectingTrackBar = true;
            if (ix < 0) ix = 0;
            if (ix > mTrackBar.Maximum) ix = mTrackBar.Maximum;

            mSelectingTrackBar = true;

            this.SetTrackBarValue(ix);
            this.TrackBar_Scroll(o, e);

            if (mVideoClock.Running == false)
            {
                mSeekWhenStoppedTimer.Start();
            }
		}


		//*******************************************************************

		public void TrackerBarMouseUp(object o, System.Windows.Forms.MouseEventArgs e)
		{
            // Weird bug where we can have another window up and click on something to close it,
            // that window disapeatrs but because the mouse was over the tracker bar we get a mouse up event here!
            if (mSelectingTrackBar == false) return;

            mSeekWhenStoppedTimer.Stop();

			// ok finsished trackbar seek, make sure we 100% in sync when we 
			// restart video clock

            MoveVideoClockToFrame(mLastSeekFrame);
		}

		//*******************************************************************

		public void MouseDown(object o, System.Windows.Forms.MouseEventArgs e)
		{
			if (Form1.mMainForm.mShiftKeyOn == false &&
				Form1.mMainForm.mControlKeyOn== false)
			{
				UnHighlightAllThumbnails(true);    
			}
		}


		//*******************************************************************
		public void TrackBar_Scroll(object o, System.EventArgs e)
		{
            if (mCurrentSlideShow != null )
			{
                if (mSelectingTrackBar == true)
                {
                   
                    mCount = this.mTrackBar.Value;
                    mLastSeekFrame = mCount;
                    CGlobals.VideoSeeking = true;
                    UpdateTimeLabel();

                    if (mVideoClock.Running == false && mPreviewSlideshowFramesWhenStopped == true)
                    {
                        if (this.GetHighlightedThumbnails().Count != 0)
                        {
                            this.UnHighlightAllThumbnails(true);
                        }
                        RenderPreviewFrame(mCount);
                    }
                }
			}
		}

	
		//*******************************************************************
		// called when in play
        // this may be a callback from video clock, make sure we are in gui
        // thread

        private Bitmap mCCachedGetNextImage;

        private delegate void GetNextImageDelegate(int frame);
		private void GetNextImage(int frame)
		{
            if (mGuiThread != null && mGuiThread.InvokeRequired == true)
            {
                // is thing already getting next image (i.e. preview when lagging)
                if (mGettingNextImage==true) return;
                mGettingNextImage = true;
                mGuiThread.BeginInvoke(new GetNextImageDelegate(GetNextImage),
                                           new object[] { frame });
                return;
            }

            mGettingNextImage = true;

            try
            {
                if (this.mVideoClock.Running == false)
                {
                    return;
                }

                mGetFrameCount++;
                if (mGetFrameCount % 500 == 498)
                {
                    GC.Collect();
                }

                UpdateTimeLabel();

                // if dragging slidebar set our frame instead
                if (mSelectingTrackBar == false)
                {
                    mCount = frame;
                    CGlobals.VideoSeeking = false;
                    if (mCount > this.mTrackBar.Maximum)
                    {
                        this.mVideoClock.Stop();
                        return;
                    }
                }
                else
                {
                    this.mVideoClock.Frame = mCount;

                }

                if (mCount < this.mTrackBar.Minimum) mCount = this.mTrackBar.Minimum;
                if (mCount > this.mTrackBar.Maximum) mCount = this.mTrackBar.Maximum;

                this.SetTrackBarValue(mCount);

                CImage current_image = null;

                if (mCCachedGetNextImage != null)
                {
                    current_image = new CImage(mCCachedGetNextImage);
                }

                // not sure if this happens or not
                if (!(current_image != null &&
                      current_image.GetRawImage().Width == CGlobals.mCurrentProject.DiskPreferences.CanvasWidth &&
                      current_image.GetRawImage().Height == CGlobals.mCurrentProject.DiskPreferences.CanvasHeight))
                {
                    current_image = null;
                }


                RenderPreviewFrame(mCount);

                

                CSlide next_slide = mCurrentSlideShow.GetNextSlide(mCount);

                if (mSelectingTrackBar == false)
                {
                    mCount++;
                    if (mCount > mCurrentSlideShow.GetTotalRenderLengthInFrames())
                    {
                        mCount = mCurrentSlideShow.GetTotalRenderLengthInFrames();
                    }
                }

            }
            catch (Exception e)
            {
                ManagedCore.CDebugLog.GetInstance().Error("Exception thrown on GetNextImage: "+e.Message);
            }
			finally 
			{
                mGettingNextImage = false;

			}
			//return i;
		}

        //*******************************************************************
        public void SeekingWhenStoppedTick(object o, System.EventArgs e)
        {
            // we need to update preview panel when seeking but stopped
            // This is incase a video which is seeking in it's own thread
            // may have now done a media sample callback
            RenderPreviewFrame(mCount);
        }
        //*******************************************************************
        private delegate void RenderPreviewFrameDelegate(int frame);
        private void RenderPreviewFrame(int frame)
        {
            // make sure we are on gui thread
            if (mGuiThread != null && mGuiThread.InvokeRequired == true)
            {
                mGuiThread.BeginInvoke(new RenderPreviewFrameDelegate(RenderPreviewFrame), new object[] { frame });
                return;
            }

            RenderVideoParameters parameters = new RenderVideoParameters();
            parameters.frame = frame;

            parameters.req_width = CGlobals.mCurrentProject.DiskPreferences.CanvasWidth;
            parameters.req_height = CGlobals.mCurrentProject.DiskPreferences.CanvasHeight;

            parameters.present_direct_to_window = true;

            // turn of decoration manager PB when previewing slideshow, render direct to parent window
            if (this.mDecorationsManager.mPB.Visible == true)
            {
                this.mDecorationsManager.mPB.Visible = false;
            }

            mCurrentSlideShow.RenderVideo(parameters);
        }

		//*******************************************************************
		public void SetImageCount(int val)
		{
			mCount =val;
		}

		//*******************************************************************
		public bool IsPlaying()
		{
            return mVideoClock.Running == true;
		}


        //*******************************************************************
        public void StopIfPlayingAndWaitForCompletion()
        {
            StopIfPlayingAndWaitForCompletion(true);
        }

		//*******************************************************************
		public void StopIfPlayingAndWaitForCompletion(bool gotoMainMenu)
		{
            if (mVideoClock.Running == false || mVideoClock.mStopping == true)
			{
                //
                // Ensure decoraion manager has main menu set
                //
                if (mMainForm != null && gotoMainMenu==true)
                {
                    if (mMainForm.GetDecorationManager().mCurrentSlide == null)
                    {
                        this.GotoMainMenu();
                    }
                }
				return ;
			}
	
			this.mVideoClock.SendTicks=false;
			this.mVideoClock.StopAndDontSendAnyCallbacks();
			System.Threading.Thread.Sleep(100);
			/*
			while (mVideoClock.mInWhileLoop==true)
			{
			
				this.mVideoClock.StopAndDontSendAnyCallbacks();
				
			}*/

			this.mVideoClock.Frame = 0;
            this.SetTrackBarValue(0);
            mFrameToStopOnWhenFinished = 0;
			mCount=0;
            if (gotoMainMenu == true)
            {
                this.GotoMainMenu();
            }

			this.mDecorationsManager.RePaint();
			this.mVideoClock.SendTicks=true;
		}

        //*******************************************************************
        // Only to be used from narration manager, music must not be playing
        public void PauseVideoPreview()
        {
            if (mVideoClock.Running == false || mVideoClock.mStopping == true)
            {
                return;
            }

            this.mVideoClock.SendTicks = false;
            this.mVideoClock.StopAndDontSendAnyCallbacks();
            System.Threading.Thread.Sleep(100);
            this.mVideoClock.SendTicks = true;
        }


		//*******************************************************************
        public void Play_Click(object o, System.EventArgs e)
        {
            Play(false);
        }

        //*******************************************************************
        public void Rewind_Click(object o, System.EventArgs e)
        {
            mSelectingTrackBar = true;
            mLastSeekFrame = 0;
            mTrackBar.Value = 0;
            TrackBar_Scroll(this, new EventArgs());
            mSelectingTrackBar = false;

            if (mVideoClock.Running == true)
            {
                mCurrentSlideShow.ResetAllMediaStreams();
                this.mVideoClock.Frame = 0;
            }
            MoveVideoClockToFrame(mLastSeekFrame);
        }

        //*******************************************************************
        public void Play(bool muteSound)
        {
            if (mVideoClock.Running == true || mVideoClock.mStopping == true) return;

            CustomButton.MiniPreviewController.StopAnyPlayingController();
				
			this.mDecorationsManager.SetCurrentSlide(null);
		
            int seek_pos = mTrackBar.Value;

			SetImageCount(seek_pos);
			this.mVideoClock.Frame = seek_pos;
            mFrameToStopOnWhenFinished = seek_pos;
            mCount = seek_pos;

            CGlobals.MuteSound = muteSound;

            //
            // Make sure correct tab showing
            //
            if (mMainForm != null)
            {
                this.mMainForm.SetMainTabControlToAppropriateDiskTab();
            }

            //
            // Reset all media streams
            //
            if (mCurrentSlideShow!=null)
			{
				this.mCurrentSlideShow.ResetAllMediaStreams();
			}

            // ok start video clock, i.e play

            // let seeks and wait whatever to current position (i.e seek and wait for videos to be in correct position)
            if (seek_pos != 0)
            {
                MoveVideoClockToFrame(seek_pos);
            }

			this.mVideoClock.Start();
           
		}

	
		//*******************************************************************
		public void Stop_Click(object o, System.EventArgs e)
		{
            Stop();

            if (mPreviewSlideshowFramesWhenStopped == true)
            {
                mFrameToStopOnWhenFinished = mCount;
            }
            else
            {
                mFrameToStopOnWhenFinished = 0; 
            }		
		}

        //*******************************************************************
        public void Stop()
        {
            if (mVideoClock.Running == false)
            {
                // If mPreviewSlideshowFramesWhenStopped is false (i.e. photo cruz) then goto main menu (even if already there)
                if (mPreviewSlideshowFramesWhenStopped == false)
                {
                    mDecorationsManager.SetCurrentSlide(null);
                    mFrameToStopOnWhenFinished = 0;
                    StopComplete();
                }

                return;
            } 
            this.mVideoClock.Stop();
        }

        //*******************************************************************
    //    private delegate void SetTrackBarValueDelegate(int number);
        private void SetTrackBarValue(int number)
        {
            number = CGlobals.Clamp(number, mTrackBar.Minimum, mTrackBar.Maximum);

            this.mTrackBar.Value = number;
        }


		//*******************************************************************
		public void StopComplete()
		{       
            // called from video clock , run in own thread, make sure we go back
            // back in gui thread.
            if (mGuiThread != null && mGuiThread.InvokeRequired)
            {
                mGuiThread.Invoke(new MethodInvoker(StopComplete));
                return;
            }

            this.mVideoClock.Frame = mFrameToStopOnWhenFinished;

            if (SlideshowPreviewFinished != null)
            {
                SlideshowPreviewFinished();
            }
        
            SetTrackBarValue(mFrameToStopOnWhenFinished);
	
            mCount = mFrameToStopOnWhenFinished;

            if (mPreviewSlideshowFramesWhenStopped == true)
            {
                // ok render last frame again, this is not really needed but just in case
                RenderPreviewFrame(mCount);

                // stop / reset all players
                mCurrentSlideShow.ResetAllMediaStreams();
            }
            else
            {
                GotoMainMenu();
            }
		}

		//*******************************************************************
		// used for moving do decide weather drop index is valid
		private bool IsDropIndexValidForMovingSlides(int i, ArrayList list)
		{
			bool ok=true;

			if (i>=1)
			{
				CSlide slide = mCurrentSlideShow.mSlides[i-1] as CSlide;
				if (list.Contains(slide)==true)
				{
					ok=false;
				}
			}

			if (i<mCurrentSlideShow.mSlides.Count)
			{
				CSlide slide = mCurrentSlideShow.mSlides[i] as CSlide;
				if (list.Contains(slide)==true)
				{
					ok=false;
				}
			}
			return ok;
		}

		//*******************************************************************
		private void ListDragTarget_DragOver(object sender, System.Windows.Forms.DragEventArgs e) 
		{
            bool MoveDataPresents = e.Data.GetDataPresent(typeof(ArrayList));

			if ( MoveDataPresents == true || e.Data.GetDataPresent(DataFormats.FileDrop ) )
			{
				e.Effect = DragDropEffects.Move;
				
				int i = GetSlideInsertBeforeIndex();

                bool ok = true;

                if ( MoveDataPresents )
                {
				    ArrayList list = e.Data.GetData(typeof(ArrayList)) as ArrayList;

				    ok = IsDropIndexValidForMovingSlides(i,list);
                }

				if (ok==true)
				{
					HideAllSepearatorsExcept(i,false);
				}
				else
				{
					HideAllSepearatorsExcept(-1,false);
				}

			}
		}

		//*******************************************************************
		private void HideAllSepearatorsExcept(int i, bool end_drop)
		{
			if (i==mCurrentShownSeperator || mCurrentControls == null) return ;

			int old_one = mCurrentShownSeperator;
			mCurrentShownSeperator =i ;

			int ii=0;
			for (int j=0;j<this.mCurrentControls.Length;j++)
			{
                SlideShowSeperator sss = mCurrentControls[j] as SlideShowSeperator;
				if (sss!=null)
				{
					if (ii== old_one)
					{
						sss.BackColor = mMainForm.GetSlideShowPanel().BackColor;
					}
					else if (ii==i)
					{
						sss.BackColor = Color.Black;
					}
					ii++;
				}
			}
			this.mMainForm.GetSlideShowPanel().Invalidate();
		}


		//*******************************************************************
		private int GetSlideInsertBeforeIndex()
		{
            int x = this.mScrollPosition;
			Point pt = this.mMainForm.GetSlideShowPanel().PointToClient(Control.MousePosition);
			int actual_x = (x * mSlideWidth) + pt.X;
			actual_x += 20;  // transition part
			actual_x /= this.mSlideWidth;
            if (actual_x < 0) actual_x = 0;
            if (actual_x > mCurrentSlideShow.mSlides.Count)
            {
                actual_x = mCurrentSlideShow.mSlides.Count;
            }

			return actual_x;
		}


		//*******************************************************************
		private void MoveScrollBarTo(int i)
		{
            // check that it is not over maximum possible i.e  scrollpanel.maximum - scrollpanel.large
            int max = CalcMaxPossibleScrollPosition();

            if (i > max) i= max;
            if (i < 0) i = 0;

#if (!DESIGN_MODE)
            if (mMainForm.scrollslideshowpanel.GetHScrollBar().Maximum > i || i==0)
            {
                this.mScrollPosition = i;
                mMainForm.scrollslideshowpanel.GetHScrollBar().Value = i;

            }
#endif

            this.ShowCurrentSlidesFromScrollPosition(mScrollPosition);
		}


        //*******************************************************************
        private void MoveControlsOrderInInternalList()
        {
            if (mCurrentControls == null) return;

            // Sort order of controls base on 'CalculatedLeft'
            // note is must go in the order:-
            // seperator
            // transition
            // time
            // thumb
            List<StoryboardComponent> controls = new List<StoryboardComponent>();

            for (int i = 0; i < mCurrentControls.Length; i++)
            {
                controls.Add(mCurrentControls[i]);
            }

            List<StoryboardComponent> new_list = new List<StoryboardComponent>();

            while (controls.Count > 0)
            {
                SlideShowSeperator use_sss = null;

                // find left most seperator
                for (int i = 0; i < controls.Count; i++)
                {
                    SlideShowSeperator sss = controls[i] as SlideShowSeperator ;
                    if (sss != null)
                    {
                        if (use_sss == null)
                        {
                            use_sss = sss;
                        }
                        else
                        {
                            if (sss.CalculatedLeft < use_sss.CalculatedLeft) use_sss = sss;
                        }
                    }
                }

                if (use_sss != null)
                {
                    new_list.Add(use_sss);
                    controls.Remove(use_sss);
                }


                // find left most transition
                TransitionComponent use_ttt = null;
                for (int i = 0; i < controls.Count; i++)
                {
                    TransitionComponent ttt = controls[i] as TransitionComponent;
                    if (ttt != null)
                    {
                        if (use_ttt == null)
                        {
                            use_ttt = ttt;
                        }
                        else
                        {
                            if (ttt.CalculatedLeft < use_ttt.CalculatedLeft) use_ttt = ttt;
                        }
                    }
                }

                if (use_ttt != null)
                {
                    new_list.Add(use_ttt);
                    controls.Remove(use_ttt);
                }

                // find left most time component
                SlideshowTimeLabelComponent use_stl = null;
                for (int i = 0; i < controls.Count; i++)
                {
                    SlideshowTimeLabelComponent stl = controls[i] as SlideshowTimeLabelComponent;
                    if (stl != null)
                    {
                        if (use_stl == null)
                        {
                            use_stl = stl;
                        }
                        else
                        {
                            if (stl.CalculatedLeft < use_stl.CalculatedLeft) use_stl = stl;
                        }
                    }
                }

                if (use_stl != null)
                {
                    new_list.Add(use_stl);
                    controls.Remove(use_stl);
                }

                // find left most thumbnail
                Thumbnail use_thumb = null;
                for (int i = 0; i < controls.Count; i++)
                {
                    Thumbnail thumb = controls[i] as Thumbnail;
                    if (thumb != null)
                    {
                        if (use_thumb == null)
                        {
                            use_thumb = thumb;
                        }
                        else
                        {
                            if (thumb.CalculatedLeft < use_thumb.CalculatedLeft) use_thumb = thumb;
                        }
                    }
                }

                if (use_thumb != null)
                {
                    new_list.Add(use_thumb);
                    controls.Remove(use_thumb);
                }

            }

            if (new_list.Count != mCurrentControls.Length)
            {
                CDebugLog.GetInstance().Error("Problem after moving slides, slide count no longer matches control count");
            }
            else
            {
                for (int i = 0; i < new_list.Count; i++)
                {
                    mCurrentControls[i] = new_list[i];
                }
            }
        }

        //*******************************************************************
        private void ListDragTarget_DragDrop(object sender, System.Windows.Forms.DragEventArgs e) 
		{
            CSlide firstSlideBeforeMove = null;
            if (mCurrentSlideShow.mSlides.Count > 0)
            {
                firstSlideBeforeMove = mCurrentSlideShow.mSlides[0] as CSlide;
            }

			this.StopIfPlayingAndWaitForCompletion();
			HideAllSepearatorsExcept(-1,true);
		
			ArrayList list = e.Data.GetData(typeof(ArrayList)) as ArrayList;

            // have we just re-arrange current slides in storyboard?
            if (list != null)
            {
                if (list.Count > 0)
                {
                    int num = GetSlideInsertBeforeIndex();
                    bool ok = IsDropIndexValidForMovingSlides(num, list);
                    if (ok == true)
                    {
                        mCurrentSlideShow.MoveSlides(list, num);
                        RebuildPanelAfterJustRearrangedSomeSlides();
                        Form1.mMainForm.GetSlideShowMusicManager().RebuildPanel();
                        Form1.mMainForm.GetSlideShowNarrationManager().RebuildPanel();
                        this.ShowCurrentSlidesFromScrollPosition(mScrollPosition);

                    }
                }
            }
            else
            {
            
                // have we dragged and dropped from an explorer window? (i.e. now add slides )
                try
                {
                    Array a = (Array)e.Data.GetData(DataFormats.FileDrop);

                    if (a != null && a.Length > 0)
                    {
                        // ok goto main menu
                        mMainForm.GoToMainMenu();

                        ArrayList my_files = new ArrayList();


                        for (int i = 0; i < a.Length; i++)
                        {
                            string tem_fin = (string)a.GetValue(i);
                            string ext = Path.GetExtension(tem_fin).ToLower();
                            if (ext != "")
                            {
                                if (CGlobals.IsImageFilename(tem_fin) == true ||
                                    CGlobals.IsVideoFilename(tem_fin) == true)
                                {
                                    if (ManagedCore.IO.IsDriveOkToUse(tem_fin) == false) return;
                                    my_files.Add(tem_fin);
                                }
                            }
                            else
                            {
                                string[] dirFiles = Directory.GetFiles(tem_fin);
                                foreach (string fn2 in dirFiles)
                                {
                                    if (CGlobals.IsImageFilename(fn2) == true ||
                                        CGlobals.IsVideoFilename(fn2) == true)
                                    {
                                        if (ManagedCore.IO.IsDriveOkToUse(tem_fin) == false) return;
                                        my_files.Add(fn2);
                                    }
                                }
                            }
                        }

                        // Call OpenFile asynchronously.
                        // Explorer instance from which file is dropped is not responding
                        // all the time when DragDrop handler is active, so we need to return
                        // immidiately (especially if OpenFile shows MessageBox).

                        int ii = 0;

                        if (my_files.Count == 0) return;

                        String[] s = new String[my_files.Count];
                        foreach (string fn in my_files)
                        {
                            s[ii] = (string)my_files[ii];
                            ii++;
                        }

                        int num = GetSlideInsertBeforeIndex();

                        int no_slides_in_slidshow_before_add = this.mCurrentSlideShow.mSlides.Count;
                
                        // ok two step process, first add slides (to end) then do a move, but only if we not adding to end anyway
                        if (no_slides_in_slidshow_before_add > 0 && num != no_slides_in_slidshow_before_add)
                        {
                            CGlobals.mCurrentProject.ForceNoMemento = true;
                            AddSlides(s, true);
                            CGlobals.mCurrentProject.ForceNoMemento = false;

                            ArrayList slide_list = new ArrayList();
                            for (int slide_no = no_slides_in_slidshow_before_add; slide_no < this.mCurrentSlideShow.mSlides.Count; slide_no++)
                            {
                                slide_list.Add(mCurrentSlideShow.mSlides[slide_no]);
                            }

                            mCurrentSlideShow.MoveSlides(slide_list, num);
                            RebuildPanelAfterJustRearrangedSomeSlides();
                            Form1.mMainForm.GetSlideShowMusicManager().RebuildPanel();
                            Form1.mMainForm.GetSlideShowNarrationManager().RebuildPanel();
                            this.ShowCurrentSlidesFromScrollPosition(mScrollPosition);
 
                        }
                        else
                        {
                            // just do a normal add to end
                            AddSlides(s, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Exception occurred in DragDrop: " + ex.Message);

                    // don't show MessageBox here - Explorer is waiting !
                }
            }

            // If we have changed the first slide and we have not set which thumbnail to show in menu, then we need to repaint menu (as it uses the first slide)
            if (firstSlideBeforeMove != null && mCurrentSlideShow.mSlides[0] != firstSlideBeforeMove && mCurrentSlideShow.MenuThumbnailSlide == null)
            {
                mMainForm.GetDecorationManager().RePaint();
            }
		}

		//*******************************************************************
		public void SetCurrentSlideShow(CSlideShow ss)
		{
			if (ss != this.mCurrentSlideShow)
			{
				int total = CGlobals.mCurrentProject.GetTotalNumberOfSlides();

				CSlideShow old_show = mCurrentSlideShow;
				this.mCurrentSlideShow =ss ;

				mCount=0;

				if (ss!=null)
				{
					// sort of hack. we want to add a callback but we don't know if we've added one already or not so...
					// i.e. we don't know if this is a new slideshow or we just switched back to an old slideshow

					// this removes one if there is already one there else does nothing
					ss.InvalidAudioSync-=new DVDSlideshow.MusicSyncInvalidCallback(this.UnableToSyncMusicCallback);

					// this now adds the callback (should only be 1) (no way of getting length you see!!))
					ss.InvalidAudioSync+=new DVDSlideshow.MusicSyncInvalidCallback(this.UnableToSyncMusicCallback);
				}

                //
                // If old show had no slides and new show has no slides, don't rebuild slideshow panel (stops dialog flickering for small time)
                //
                int oldShowsCount = 0;
                int newShowsCount = 0;
                if (old_show!=null)
                {
                    oldShowsCount = old_show.mSlides.Count;
                }
                if (mCurrentSlideShow !=null)
                {
                    newShowsCount = mCurrentSlideShow.mSlides.Count;
                }

                if (oldShowsCount >0 || newShowsCount >0)
                {
                    RebuildPanel(old_show);
                }

				SetScrollToStart();

                //
                //  Make sure slideshow list combo in top panel updated and correct item is selected
                //
                if (mMainForm != null)
                {
                    mMainForm.GetDiskMenuManager().UpdateSlideshowList(false);
                }
			}
		}


		//*******************************************************************
		public void ReCalcTrackerBar()
		{
            if (mTrackBar.InvokeRequired == true)
            {
                mTrackBar.Invoke(new MethodInvoker(ReCalcTrackerBar));
                return;
            }
                
			if (this.mCurrentSlideShow!=null)
			{ 
				this.mTrackBar.Minimum = 0;
                this.mTrackBar.Maximum = mCurrentSlideShow.GetTotalRenderLengthInFrames();
                this.mTrackBar.SmallChange = mCurrentSlideShow.GetTotalRenderLengthInFrames() / 20;
                this.mTrackBar.LargeChange = mCurrentSlideShow.GetTotalRenderLengthInFrames() / 10;
			}
			else
			{
				this.mTrackBar.Minimum = 0;
				this.mTrackBar.Maximum = 0;
				this.mTrackBar.TickFrequency = 1;
			}

			if (mCount > this.mTrackBar.Maximum) mCount =0 ;
			if (mCount <0) mCount =0 ;

			UpdateTimeLabel();
			this.mTrackBar.Value=mCount;
		}

        
		//*******************************************************************
        public void MoveTrackerToSlide(CSlide slide)
        {
            MoveTrackerToSlide(slide, 0);
        }

		//*******************************************************************
		public void MoveTrackerToSlide(CSlide slide, float plusOffsetTime)
		{
            int offsetFrame =(int)( plusOffsetTime * CGlobals.mCurrentProject.DiskPreferences.frames_per_second);

            int st = slide.mStartFrameOffset + offsetFrame;
			if (st >=mTrackBar.Minimum && st < this.mTrackBar.Maximum)
			{
				this.mTrackBar.Value = st;
				
				mCount = this.mTrackBar.Value;
                slide.ResetAllMediaStreams();
                MoveVideoClockToFrame(mCount);
               
			}
			UpdateTimeLabel();
		}

        //*******************************************************************
        private void MoveVideoClockToFrame(int frame)
        {
            bool muteSound = CGlobals.MuteSound;
 
            mVideoClock.SendTicks = false;
            mVideoClock.Frame = frame;
            CGlobals.VideoSeeking = true;
            CGlobals.WaitVideoSeekCompletion = true;
            CGlobals.MuteSound = false;

            RenderPreviewFrame(frame);  // causes video to seek to correct position

            // ok ready to play again
            mSelectingTrackBar = false;
            CGlobals.VideoSeeking = false;
            CGlobals.WaitVideoSeekCompletion = false;
            CGlobals.MuteSound = muteSound;
            this.mVideoClock.Frame = frame;
            this.mVideoClock.SendTicks = true;
        }


		//*******************************************************************
		private void UnableToSyncMusicCallback()
		{
			if (this.mCurrentSlideShow!=null)
			{
				
				double music_time = mCurrentSlideShow.GetLengthOfBackgroundMusic(false);

			
				if (music_time<=0.001)
				{
                    DialogResult res = UserMessage.Show("Can not synchronize slide show to background music as\nthere is no background music defined in the slide show.", "Invalid operation",
						MessageBoxButtons.OK,MessageBoxIcon.Information);
				}
				else
				{
                    DialogResult res = UserMessage.Show("Can not synchronize slide show to background music as\nthe total video slides exceed background music length.", "Invalid operation",
						MessageBoxButtons.OK,MessageBoxIcon.Information);
				}
				EnableSlideTimes(true);
			}
		}

		//*******************************************************************
		private void EnableSlideTimes(bool val)
		{
			ArrayList al = GetAllThumbnails();
			foreach (Thumbnail tn in al)
			{

                if (tn.SlideLengthComboEnabled != val)
				{
                    if (val == false)
                    {
                        tn.SlideLengthComboEnabled = val;
                    }
                    else
                    {
                        tn.UpdateSlideLengthComboEnabled();
                    }
				}
                
			}
		}

	
		//*******************************************************************
		private void ListDragTarget_DragEnter(object sender, System.Windows.Forms.DragEventArgs e) 
		{
			this.mDragScrollTimer.Stop();
		}

		
		//*******************************************************************
		private void ListDragTarget_DragLeave(object sender, System.EventArgs e) 
		{
            this.StartPanelScrollTimer();
		}

		//*******************************************************************
		public void InitializeComponent()
		{
			this.openFileDialog.Multiselect = true;
		}

		//*******************************************************************
		private void AddSlidesGo(object o)
		{
           // throw new Exception();
		
			try
			{
                AddSlidesObject aso = o as AddSlidesObject;

				mCurrentSlideShow.AddSlides(aso.slides, aso.insertAfter, aso.insertBefore, mProgressWindow as ManagedCore.Progress.IProgressCallback);
				
			}
			catch(Exception e)
			{
				CDebugLog.GetInstance().Error("Problem adding slides; "+e.ToString());
			}

            if (this.mProgressWindow != null)
            {
                this.mProgressWindow.End();
            }

		}


        //*******************************************************************
        // If 'insertAfter' set to true, then it inserts after the selected slide
        // else it inserts before the selected slide.
        // If nothing is selected, it simply appends to the end of slideshow
        private void AddSlides(string[] slides, bool insertAfter)
		{
			int slides_in_current=0;

			if (this.mCurrentSlideShow !=null)
			{ 
				slides_in_current=this.mCurrentSlideShow.mSlides.Count;
			}

            int max_slides_in_slideshow = CGlobals.MaxSlidesPerSlideshow;

            if (slides_in_current + slides.Length > max_slides_in_slideshow)
			{
                StringBuilder builder = new StringBuilder();
                builder.Append("There is a maximum of ");
                builder.Append(max_slides_in_slideshow.ToString());
                builder.Append(" slides allowed per slideshow.");

                CGlobals.VideoType output = CGlobals.mCurrentProject.DiskPreferences.OutputVideoType;
                bool multiSlideshowsAllowed = output == CGlobals.VideoType.DVD || output == CGlobals.VideoType.SVCD || output == CGlobals.VideoType.VCD;

                if (slides_in_current >= max_slides_in_slideshow )
                {
                    if (multiSlideshowsAllowed)
                    {
                        builder.Append("\n\rPlease add any new slides to another slideshow.");
                    }

                    UserMessage.Show(builder.ToString(), "Too many slides",
				    	System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                    return ;
                }

                int slides_left = max_slides_in_slideshow - slides_in_current;

                builder.Append("\n\rDo you wish to load the first ");
                builder.Append(slides_left);
                builder.Append(" slide(s)?");

                if (multiSlideshowsAllowed)
                {
                    builder.Append("\n\r\n\r(To then add additional slides, please create another slideshow)");
                }

                DialogResult result = UserMessage.Show(builder.ToString(), "Too many slides",
				    	System.Windows.Forms.MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Exclamation);
                if (result == DialogResult.Cancel)
                {
                    return ;
                }

                string[] slides2 = new string[max_slides_in_slideshow - slides_in_current];

                for (int ii = 0; ii < max_slides_in_slideshow - slides_in_current; ii++)
				{
					slides2[ii] = slides[ii];
				}

				slides = slides2;

			}

			mProgressWindow=null;

            AddSlidesObject aso = new AddSlidesObject();
            aso.slides = slides;

            //
            // Insert Slides after/before currently selected slides?
            //
            ArrayList highLightedSlides = GetHighlightedThumbnailSlides();

            if (highLightedSlides.Count > 0)
            {
                if (insertAfter == true)
                {
                    aso.insertAfter = highLightedSlides[0] as CSlide;
                }
                else
                {
                    aso.insertBefore = highLightedSlides[0] as CSlide;
                }
            }

            mProgressWindow = new ProgressWindow(AddSlidesGo, aso);//true);
            mProgressWindow.Text = "Loading";
            mProgressWindow.ShowDialog();
            bool cancelled = mProgressWindow.Cancelled;
            mProgressWindow = null;
            
			if (cancelled)
			{
                UserMessage.Show("Operation cancelled", "Operation cancelled",
					System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
			}

            System.GC.Collect();
            this.RebuildPanel(null, mScrollPosition);
            System.GC.Collect();         
		}


		//*******************************************************************
		public void AddSlidesButton_Click(bool insertAfter)
		{
			StopIfPlayingAndWaitForCompletion();
            mMainForm.GoToMainMenu(false);
          
			if (openFileDialog.ShowDialog().Equals(System.Windows.Forms.DialogResult.OK))
			{
				if (mCurrentSlideShow==null)
				{
                    Log.Error("Trying to add slides to a null slideshow in AddSlidesButton_Click");
				}
				else
				{
                    if (openFileDialog.FileNames.Length > 0)
                    {
                        if (ManagedCore.IO.IsDriveOkToUse(openFileDialog.FileNames[0]) == false) return;
                        openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(openFileDialog.FileNames[0]);  // rember current folder  
                        AddSlides(openFileDialog.FileNames, insertAfter);
                    }
				}
			}
		}

		//*******************************************************************
		public ArrayList GetAllThumbnails()
		{
			ArrayList to_return = new ArrayList();
            if (mCurrentControls != null)
            {
                for (int i = 0; i < mCurrentControls.Length; i++)
                {
                    Thumbnail tn = mCurrentControls[i] as Thumbnail;
                    if (tn != null)
                    {
                        to_return.Add(tn);
                    }
                }
            }
			return to_return;
		}


		//*******************************************************************
		private CSlideShowManagerThumnailCache GetCachedThumnnailsForSlideshow(CSlideShow show)
		{
			if (show==null) return null;

			foreach (CSlideShowManagerThumnailCache cache in mChachedSlideshowThumnailsList)
			{
				if (cache.for_slideshow.ID == show.ID)
				{
					return cache;
				}
			}
			return null;
		}


		//*******************************************************************
		public void InvalidateAllThumbnailsDueToApectRatioChange()
		{
            lock (thislock)
            {
                mProgressWindow = new ProgressWindow(InvalidateAllThumbnailsDueToApectRatioChange2, null);//true);
                mProgressWindow.CancelButtons.Visible = false;
                mProgressWindow.Text = "Rebuilding slideshow";
                mProgressWindow.ShowDialog();
                SlideshowThumnailsChanged();
                this.mMainForm.GetSlideShowPanel().Invalidate();
                this.mMainForm.GetSlideShowPanel().Refresh();
                HighlightChanged();
                mProgressWindow = null;
            }
		}

		//*******************************************************************
		private void InvalidateAllThumbnailsDueToApectRatioChange2(object o)
		{
            if (mCurrentControls != null)
            {
                mProgressWindow.Begin(0, mCurrentControls.Length);
                try
                {

                    for (int i = 0; i < mCurrentControls.Length; i++)
                    {
                        Thumbnail tn = mCurrentControls[i] as Thumbnail;
                        if (tn != null)
                        {
                            tn.InvalidateImage();
                        }

                        TransitionComponent ttt = mCurrentControls[i] as TransitionComponent;
                        if (ttt != null)
                        {
                            ttt.RebuildCurrentTransPictureBox();
                        }

                        mProgressWindow.StepTo(i);



                    }
                }
                catch (Exception e)
                {
                    ManagedCore.CDebugLog.GetInstance().Error("Exception occurred in InvalidateAllThumbnailsDueToApectRatioChange2: "+e.Message);
                }
            }
  
            mProgressWindow.End();
		}

		//*******************************************************************
		// quick rebuild of panel when just rearrangine slides

		private void RebuildPanelAfterJustRearrangedSomeSlides()
		{
            if (mCurrentControls == null) return;

			ArrayList list = new ArrayList();
			ArrayList tc_list = new ArrayList();
			ArrayList time_labels = new ArrayList();

			int count =0;
			
			for (int i=0;i<this.mCurrentControls.Length;i++)
			{

                Thumbnail tn = mCurrentControls[i] as Thumbnail;
				if (tn!=null)
				{	
					list.Add(tn);
				}
				
				TransitionComponent ttt =  mCurrentControls[i] as TransitionComponent;
				if (ttt!=null)
				{
					tc_list.Add(ttt);
				}
				SlideshowTimeLabelComponent tlc = mCurrentControls[i] as SlideshowTimeLabelComponent;
				if (tlc !=null)
				{
					time_labels.Add(tlc);
				}
		
			}

			int sn =0 ;

			Thumbnail missing_one_for_tlc = null;
			Thumbnail missing_one_for_ttt = null;

			foreach (CSlide slide in mCurrentSlideShow.mSlides)
			{
				CImageSlide sps = slide as CImageSlide;

				Thumbnail the_tn = null;
		
				if (sps!=null)
				{
					// check if old thumnail matches
					// find slide
					foreach (Thumbnail c in list)
					{
						if (c.mSlide.ID == slide.ID)
						{
                            if (c.CalculatedLeft != count + 10)
                            {
                                c.CalculatedLeft = count + 10;
                            }
							the_tn= c;
							list.Remove(c);
							break;
						}
					}

					// do time labels

					bool done_time_label = false;

					foreach (SlideshowTimeLabelComponent tlc in time_labels)
					{
						if (tlc.mSlide.ID == slide.ID)
						{
                            if (tlc.CalculatedLeft != 4 + count + mSlideWidth - 44) 
							{
                                tlc.CalculatedLeft = 4 + count + mSlideWidth - 44;
							}

							list.Remove(tlc);
							done_time_label=true;

							// if it was the last slide we better find missing and replace
							if (sn == mCurrentSlideShow.mSlides.Count-1 &&
								mCurrentSlideShow.mSlides.Count >1 &&
								missing_one_for_tlc!=null)
							{
								tlc.Blank();
								tlc.ResetToSlide(missing_one_for_tlc.mSlide);
                                tlc.CalculatedLeft = missing_one_for_tlc.CalculatedLeft + 4 + mSlideWidth - 44 - 10;
							}

							tlc.RebuildCurrentTransPictureBox();
							
							break;
						}
					}

					if (done_time_label==false)
					{
						if (missing_one_for_tlc!=null)
						{
							Log.Warning("More than one missing time label component after slideshow rebuild");
						}

						missing_one_for_tlc = the_tn;
					}	


					// do transitions

					bool done_trans = false;

					foreach (TransitionComponent ttt in tc_list)
					{
						if (ttt.Slide.ID == slide.ID)
						{
                            if (ttt.CalculatedLeft != 4 + count + mSlideWidth - 42) 
							{
                                ttt.CalculatedLeft = 4 + count + mSlideWidth - 42;
							}

							CSlide next_slide = mCurrentSlideShow.GetNextSlide(slide);
							// rebuild tansition box if next slide has changed

							bool trans_need_changed=false;

							if ((ttt.NextSlide==null && next_slide!=null) ||
								(ttt.NextSlide!=null && next_slide==null))
							{
								trans_need_changed=true;
							}
							else if (ttt.NextSlide!=null && next_slide!=null)
							{
								if (ttt.NextSlide.ID != next_slide.ID)
								{
									trans_need_changed=true;
								}
							}

							if (trans_need_changed)
							{
								ttt.NextSlide =next_slide;
								ttt.RebuildCurrentTransPictureBox();
							}

							list.Remove(ttt);
							done_trans =true;

							// if it was the last slide we better find missing and replace
							if (sn == mCurrentSlideShow.mSlides.Count-1 &&
								mCurrentSlideShow.mSlides.Count >1 &&
								missing_one_for_ttt!=null)
							{
								ttt.Blank();
                                ttt.ReConstruct(missing_one_for_ttt.mSlide, mCurrentSlideShow);
                                ttt.CalculatedLeft = missing_one_for_ttt.CalculatedLeft + 4 + mSlideWidth - 42 - 10;
								ttt.RebuildCurrentTransPictureBox();
							}

						
							
							break;
						}
					}

					if (done_trans==false)
					{
						if (missing_one_for_ttt!=null)
						{
							Log.Warning("More than one missing transition component after slideshow rebuild");
						}

						missing_one_for_ttt = the_tn;
					}	

				}

				count += this.mSlideWidth;
				sn++;

			}

            MoveControlsOrderInInternalList();
		}

        //*******************************************************************
        public void RebuildPanel()
        {
            RebuildPanel(null, mScrollPosition);
        }

        //*******************************************************************
        public void RebuildPanel(CSlideShow old_show)
        {
            RebuildPanel(old_show, 0);
        }

		//*******************************************************************
		public void RebuildPanel(CSlideShow old_show, int new_scroll_position)
		{
            try
            {
                if (this.mMainForm == null) return;

                lock (thislock)
                {
                    try
                    {
                        mProgressWindow = null;
                        mControlsToAdd = null;

                        // ok hack to stop rebuild window flashing on load up
                        if (old_show == null &&
                            this.mCurrentSlideShow.mSlides.Count == 0)
                        {
                            RebuildPanel2(null);
                        }
                        else
                        {
                            mProgressWindow = new ProgressWindow(this.mMainForm, RebuildPanel2, old_show);//true);
                            mProgressWindow.CancelButtons.Visible = false;
                            mProgressWindow.Text = "Rebuilding slideshow";
                            mProgressWindow.ShowDialog();

                        }

                         
                        ReCalcHScrollBarMinMaxAndViewableValues();

                        // set the scroll min/max
#if (!DESIGN_MODE)
                        ManualScrollablePanel p = mMainForm.scrollslideshowpanel as ManualScrollablePanel;

                        if (p != null && mCurrentSlideShow != null)
                        {

                            if (p.GetHScrollBar().Maximum != mCurrentSlideShow.mSlides.Count - 1)
                            {
                                p.GetHScrollBar().Maximum = mCurrentSlideShow.mSlides.Count - 1;
                            }
                            p.GetHScrollBar().Minimum = 0;
                        }
#endif

                        if (mControlsToAdd != null && mControlsToAdd.Length > 0)
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            // sometimes crash's here

                            mCurrentControls = mControlsToAdd;

                            // Windows forms delays the drawing the the UserControl until it gets added to something (e.g. our scroll panel )
                            // To prevent flickering when we scroll through for the first time, add all new UserControls to scrollpanel now.... and then remove
                            // them all again.

                            Panel sp = mMainForm.GetSlideShowPanel();
                            StoryboardComponentControlCache.GetInstance().EnsureCreateInitialCachedControls(sp);

                            mControlsToAdd = null;
                        }
                        else
                        {
                            mControlsToAdd = null;
                        }

                        MoveScrollBarTo(new_scroll_position);

                        SlideshowThumnailsChanged();
                        this.mMainForm.GetSlideShowPanel().Invalidate();
                        this.mMainForm.GetSlideShowPanel().Refresh();
                        HighlightChanged();


                    }
                    catch 
                    {
                        if (mProgressWindow != null) mProgressWindow.End();
                        mProgressWindow = null;
                    }

                    GC.Collect();
                    Cursor.Current = Cursors.Default;
                }
            }
            catch (Exception eee)
            {
                throw eee;
            }
		}


		//*******************************************************************
		private void CheckWeAreNotNear10000UserObjects()
		{
			int total = 1000;

			// 10000 user objects is around 700-800 slides in our app
			
			while (total >600 && mChachedSlideshowThumnailsList.Count >0)
			{
				total = this.mCurrentSlideShow.mSlides.Count;

				foreach (CSlideShowManagerThumnailCache cache in mChachedSlideshowThumnailsList)
				{
					total+=(cache.usercontrols.Count/4);
				}

				if (total >600 && mChachedSlideshowThumnailsList.Count >0)
				{
					CSlideShowManagerThumnailCache cache = (CSlideShowManagerThumnailCache) mChachedSlideshowThumnailsList[0];
					mChachedSlideshowThumnailsList.Remove(cache);
					RemoveStoryBoardUserControls(cache.usercontrols);
				}

			}
		}

        //*******************************************************************
        public void AddRemoveBlankStoryboardLabels()
        {
            if (mCurrentSlideShow == null) return; ;

            Panel p = this.mMainForm.GetSlideShowPanel();

            if (mCurrentSlideShow.mSlides.Count == 0)
            {
                if (mEmptyStoaryBoardLabel == null)
                {
                    mEmptyStoaryBoardLabel = new Label();
                    mEmptyStoaryBoardLabel.BackColor = Color.Transparent;
                    mEmptyStoaryBoardLabel.ForeColor = Color.Gray;
                    mEmptyStoaryBoardLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                    mEmptyStoaryBoardLabel.TextAlign = ContentAlignment.MiddleCenter;
                    mEmptyStoaryBoardLabel.Text = "Empty\n\rstoryboard";
                    mEmptyStoaryBoardLabel.Width = 140;
                    mEmptyStoaryBoardLabel.Height = 80;
                    mEmptyStoaryBoardLabel.Top = 20;
                    mEmptyStoaryBoardLabel.Left = 0;
                }

                if (p.Controls.Contains(mEmptyStoaryBoardLabel) == false)
                {
                    p.Controls.Add(mEmptyStoaryBoardLabel);
                }
            }
            else if ( p.Controls.Contains(mEmptyStoaryBoardLabel)==true)
            {
                p.Controls.Remove(mEmptyStoaryBoardLabel);
            }


            if (mCurrentSlideShow.mMusicSlides.Count == 0)
            {
                if (mSoundTrackLabel == null)
                {
                    mSoundTrackLabel = new Label();
                    mSoundTrackLabel.Name = "Music";
                    mSoundTrackLabel.BackColor = Color.Transparent;
                    mSoundTrackLabel.ForeColor = Color.Gray;
                    mSoundTrackLabel.Font = new Font("Segoe UI", 10.0f, FontStyle.Bold);
                    mSoundTrackLabel.Text = "Music soundtrack";
 
                    if (CGlobals.IsWinVistaOrHigher() == true)
                    {
                        string note = "\u266B";
                        mSoundTrackLabel.Text += " "+note;
                    }

                    mSoundTrackLabel.Width = 180;
                    mSoundTrackLabel.Height = 25;
                    mSoundTrackLabel.Top = 130;
                    mSoundTrackLabel.Left = 10;
                }
  
                if (p.Controls.Contains(mSoundTrackLabel) == false)
                {
                    p.Controls.Add(mSoundTrackLabel);
                }
            }
            else if (p.Controls.Contains(mSoundTrackLabel) == true)
            {
                p.Controls.Remove(mSoundTrackLabel);
            }

            if (mCurrentSlideShow.NarrationSlides.Count == 0)
            {
                if (mNarrationLabel == null)
                {
                    mNarrationLabel = new Label();
                    mNarrationLabel.Name = "Narration";
                    mNarrationLabel.BackColor = Color.Transparent;
                    mNarrationLabel.ForeColor = Color.Gray;
                    mNarrationLabel.Font = new Font("Segoe UI", 10.0f, FontStyle.Bold);
                    mNarrationLabel.Text = "Narration / sounds";
                    mNarrationLabel.Width = 160;
                    mNarrationLabel.Height = 25;
                    mNarrationLabel.Top = 161;
                    mNarrationLabel.Left = 10;
                }

                if (p.Controls.Contains(mNarrationLabel) == false)
                {
                    p.Controls.Add(mNarrationLabel);
                }
            }
            else if (p.Controls.Contains(mNarrationLabel) == true)
            {
                p.Controls.Remove(mNarrationLabel);
            }
        }

        //*******************************************************************
        private void ShowCurrentSlidesFromScrollPosition(int position)
        {
            Panel p = this.mMainForm.GetSlideShowPanel();
            p.SuspendLayout();

            foreach (Control c in p.Controls)
            {
                StoryboardComponentControlCache.GetInstance().ReleaseControl(c);
            }

            p.Controls.Clear();

            if (mTopLinePanel.Width != p.Width) mTopLinePanel.Width = p.Width;
            if (mStoryboardToAudioLinePanel.Width != p.Width) mStoryboardToAudioLinePanel.Width = p.Width;
            if (mAudioLinePanel.Width != p.Width) mAudioLinePanel.Width = p.Width;

            p.Controls.Add(mTopLinePanel);
            p.Controls.Add(mStoryboardToAudioLinePanel);
            p.Controls.Add(mAudioLinePanel);

            AddRemoveBlankStoryboardLabels();

            if (mCurrentControls == null)
            {
              //  p.Controls.Add(this.mMainForm.GetBlankSlidePictureBox());

               
                p.ResumeLayout();
                return;
            }

            int current_pos = 0;
            int width = this.mMainForm.GetSlideShowPanel().Width;

            // 4 controls per slide
            int current_index = position * 4;

            ArrayList to_add = new ArrayList();
            while (current_pos * mSlideWidth < width)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (mCurrentControls.Length > current_index)
                    {
                        StoryboardComponent c = mCurrentControls[current_index++] as StoryboardComponent;
                        if (c != null)
                        {
                            c.Left = (c.CalculatedLeft % mSlideWidth) + (current_pos * mSlideWidth);
                        }
                        else
                        {
                            int i = 3;
                        }
                        to_add.Add(c);
                    }
                }
                
                current_pos++;
            }

            Control[] controls_to_add = new Control[to_add.Count];
            for (int i=0;i< to_add.Count;i++)
            {
                controls_to_add[i] = StoryboardComponentControlCache.GetInstance().GetControlForStoryboardComponent((StoryboardComponent)to_add[i]);
            }

            p.Controls.AddRange(controls_to_add);

            mMainForm.GetSlideShowMusicManager().DrawFromCurrentSrcollPosition();
            mMainForm.GetSlideShowNarrationManager().DrawFromCurrentSrcollPosition();

            p.ResumeLayout();
        }


		//*******************************************************************
		private void RebuildPanel2(object o)
		{
            try
            {
                CSlideShow old_show = o as CSlideShow;
                //		if (mProgressWindow!=null)
                //		{
                //			mProgressWindow.SetText("Rebuilding slideshow");
                //		}

                if (mMainForm == null)
                {
                    if (mProgressWindow != null)
                    {
                        mProgressWindow.Begin(0, 100);
                        mProgressWindow.StepTo(100);
                        mProgressWindow.End();
                    }
                    mProgressWindow = null;
                    return;
                }

                if (old_show == null)
                {
                    old_show = mCurrentSlideShow;
                }

                // clean up any thumbnails already present
                ArrayList to_remove = new ArrayList();
                ArrayList seperators = new ArrayList();
                ArrayList time_labels = new ArrayList();

                if (mCurrentControls != null)
                {

                    for (int i = 0; i < mCurrentControls.Length; i++)
                    {

                        Thumbnail tn = mCurrentControls[i] as Thumbnail;
                        if (tn != null)
                        {
                            to_remove.Add(tn);
                        }
                        SlideShowSeperator sss = mCurrentControls[i] as SlideShowSeperator;
                        if (sss != null)
                        {
                            seperators.Add(sss);
                            to_remove.Add(sss);
                        }
                        TransitionComponent ttt = mCurrentControls[i] as TransitionComponent;
                        if (ttt != null)
                        {
                            to_remove.Add(ttt);
                        }
                        SlideshowTimeLabelComponent tlc = mCurrentControls[i] as SlideshowTimeLabelComponent;
                        if (tlc != null)
                        {
                            time_labels.Add(tlc);
                            to_remove.Add(tlc);
                        }

                    }
                }

                CSlideShowManagerThumnailCache cache = null;

                // shall we cache the current thumbnail list?
                if (old_show != null &&
                    mCurrentSlideShow != null &&
                    old_show.ID != mCurrentSlideShow.ID &&
                    to_remove.Count > 0)
                {
                    CSlideShowManagerThumnailCache old_cache = GetCachedThumnnailsForSlideshow(old_show);
                    if (old_cache != null)
                    {
                        ManagedCore.CDebugLog.GetInstance().Error("Slideshow manager had two copies of a cached thumbnail");
                        mChachedSlideshowThumnailsList.Remove(old_cache);
                    }

                    cache = new CSlideShowManagerThumnailCache(old_show);

                    cache.usercontrols = (ArrayList)to_remove.Clone();

                    // lets be 100% sure;
                    seperators.Clear();
                    time_labels.Clear();

                    mChachedSlideshowThumnailsList.Add(cache);
                }
                else if (to_remove.Count > 0)
                {
                    ManagedCore.CDebugLog.GetInstance().Trace("Rebuilding slideshow manager but slideshow was the same");
                }

                if (to_remove.Count >0 && to_remove.Count >= mCurrentControls.Length)
                {
                    mCurrentControls = null;
                }

                // ok if we cached the 'to_remove' we now need to cleat the list as they may
                // be currupted by the thumbnail cache stuff
                if (cache != null)
                {
                    to_remove.Clear();             
                }

                // rebuild

                if (mCurrentSlideShow != null)
                {
                    // check if we have the thumbnails under cache

                    CSlideShowManagerThumnailCache new_cache = GetCachedThumnnailsForSlideshow(mCurrentSlideShow);

                    if (new_cache != null)
                    {
                        mChachedSlideshowThumnailsList.Remove(new_cache);
                        StoryboardComponent[] controls = new StoryboardComponent[new_cache.usercontrols.Count];
                        int i = 0;
                        foreach (StoryboardComponent uc in new_cache.usercontrols)
                        {
                            controls[i] = uc;
                            i++;
                        }

                        mCurrentControls =controls;

                        // has slidehow changed since we last cached it
                        if (new_cache.mNeedRebuild == true)
                        {
                            RebuildPanel2(null);
                            return;
                        }
                    }

                        // ok rebuild from scratch (i.e create new controls )
                    else
                    {
                        // ok make sure we won't be going over 10000 user objects
                        // else delete a cheched thumbnail list
                        CheckWeAreNotNear10000UserObjects();

                        int numberHorizontal = -1;
                        // create the thumbnails for the selected files
                        int count = 0;
                        int slide_num = 1;
                        int num_slides = mCurrentSlideShow.mSlides.Count;

                        int y_offset = 3;

                        ArrayList new_controls = new ArrayList();

                        ArrayList no_actual_new_control = new ArrayList();

                        if (mCurrentSlideShow.mSlides.Count != 0)
                        {
                            foreach (CSlide slide in mCurrentSlideShow.mSlides)
                            {
                                CImageSlide sps = slide as CImageSlide;
                                Thumbnail thumbnail = null;
                                SlideShowSeperator seperator = null;
                                TransitionComponent transcomp = null;
                                SlideshowTimeLabelComponent timelabel = null;

                                if (sps != null)
                                {
                                    // check if old thumnail matches

                                    thumbnail = null;
                                    foreach (StoryboardComponent c in to_remove)
                                    {
                                        if (c is Thumbnail &&
                                            ((Thumbnail)c).mSlide.ID == slide.ID)
                                        {
                                            thumbnail = (Thumbnail)c;
                                            to_remove.Remove(c);
                                            thumbnail.ReCalcComboText();
                                            break;
                                        }
                                    }

                                    if (thumbnail == null)
                                    {
                                        thumbnail = GetNewThumbnail(sps);// new Thumbnail(sps,this);

                                        no_actual_new_control.Add(thumbnail);
                                    }


                                    if (seperators.Count > 0)
                                    {
                                        seperator = (SlideShowSeperator)seperators[0];
                                        seperators.Remove(seperator);
                                        to_remove.Remove(seperator);
                                    }
                                    else
                                    {
                                        seperator = GetNewSlideShowSeperator();
                                    }

                                    transcomp = null;

                                    foreach (StoryboardComponent c in to_remove)
                                    {
                                        if (c is TransitionComponent)
                                        {
                                            TransitionComponent tc = c as TransitionComponent;

                                            if (tc.Slide.ID == slide.ID)
                                            {
                                                CSlide tc_ns = mCurrentSlideShow.GetNextSlide(slide);

                                                if (tc != null && tc_ns != null)
                                                {
                                                    if (tc.NextSlide.ID ==
                                                        mCurrentSlideShow.GetNextSlide(slide).ID)
                                                    {
                                                        transcomp = (TransitionComponent)c;
                                                        to_remove.Remove(c);
                                                        break;
                                                    }
                                                }
                                                else if (tc == null && tc_ns == null)
                                                {
                                                    transcomp = (TransitionComponent)c;
                                                    to_remove.Remove(c);
                                                    break;
                                                }

                                            }
                                        }
                                    }

                                    if (transcomp == null && num_slides != slide_num)
                                    {
                                        if (TransitionComponentCache.Count > 0)
                                        {
                                            transcomp = (TransitionComponent)TransitionComponentCache[0];
                                            TransitionComponentCache.Remove(transcomp);
                                            transcomp.ReConstruct(sps, mCurrentSlideShow);
                                        }
                                        else
                                        {
                                            transcomp = new TransitionComponent(sps, mCurrentSlideShow);
                                        }

                                        no_actual_new_control.Add(transcomp);
                                    }


                                    timelabel = null;

                                    if (time_labels.Count > 0)
                                    {
                                        timelabel = (SlideshowTimeLabelComponent)time_labels[0];
                                        time_labels.Remove(timelabel);
                                        to_remove.Remove(timelabel);
                                        timelabel.mSlide = sps;
                                    }
                                    else if (num_slides != slide_num)
                                    {
                                        timelabel = GetNewSlideshowTimeLabel(sps);
                                    }


                                    if (numberHorizontal < 0)
                                    {
                                        // determine how many thumbnails can be displayed on one row
                                        numberHorizontal = (int)(mMainForm.GetSlideShowPanel().Width / thumbnail.Width);
                                    }
                                    // set the position for the thumbnail and add it to the panel's controls
                                    thumbnail.CalculatedLeft = 10 + count;
                                    thumbnail.Top = y_offset;

                                    if (transcomp != null)
                                    {
                                        transcomp.CalculatedLeft = 4 + count + mSlideWidth - 42;
                                        transcomp.Top = 30;
                                    }
                                    //transcomp.Width = 50;
                                    //	transcomp.Height = 65;

                                    if (timelabel != null && transcomp != null)
                                    {
                                        timelabel.RebuildCurrentTransPictureBox();
                                        timelabel.CalculatedLeft = transcomp.CalculatedLeft - 2;
                                        timelabel.Top = 101;
                                    }

                                    seperator.CalculatedLeft = 4 + count;
                                    seperator.Top = y_offset;
                                    seperator.Width = 3;
                                    //	seperator.BackColor = Color.Black;
                                    seperator.Height = thumbnail.Height;
                                    //	seperator.Hide();


                                    new_controls.Add(seperator);

                                    // dont do transisition for last slide
                                    if (num_slides != slide_num)
                                    {
                                        if (transcomp != null)
                                        {
                                            new_controls.Add(transcomp);
                                        }
                                        if (timelabel != null)
                                        {
                                            new_controls.Add(timelabel);
                                        }
                                    }
                                    else
                                    {
                                        if (transcomp != null)
                                        {
                                            transcomp.Blank();
                                        }

                                        if (timelabel != null)
                                        {
                                            timelabel.Blank();
                                        }
                                    }

                                    new_controls.Add(thumbnail);

                                    if (num_slides == slide_num)
                                    {
                                        // add extra seperator at the end

                                        SlideShowSeperator seperator2 = null;

                                        if (seperators.Count > 0)
                                        {
                                            seperator2 = (SlideShowSeperator)seperators[0];
                                            seperators.Remove(seperator2);
                                            to_remove.Remove(seperator2);
                                        }
                                        else
                                        {
                                            seperator2 = GetNewSlideShowSeperator();
                                        }

                                        seperator2.Top = y_offset;
                                        seperator2.Width = 3;
                                        seperator2.Height = thumbnail.Height;
                                        seperator2.CalculatedLeft = 3 + count + mSlideWidth - 32;
                                        new_controls.Add(seperator2);
                                    }

                                    count += mSlideWidth;
                                }


                                slide_num++;
                            }

                        }

                        this.mControlsToAdd = new StoryboardComponent[new_controls.Count];

                        int new_control_index = 0;
                        foreach (StoryboardComponent cc in new_controls)
                        {
                            mControlsToAdd[new_control_index] = cc;
                            new_control_index++;
                        }

                        invalidate_images(no_actual_new_control);
                    }


                }

                if (cache == null)
                {
                    RemoveStoryBoardUserControls(to_remove);
                }

                if (mProgressWindow != null)
                {
                    mProgressWindow.End();
                }
                mProgressWindow = null;
            }
            catch (Exception e)
            {
                throw;
            }
		}

		//*******************************************************************
		private void RemoveStoryBoardUserControls(ICollection to_remove)
		{
			foreach (StoryboardComponent uc in to_remove)
			{
				if (uc as TransitionComponent !=null)
				{
					TransitionComponentCache.Add(uc);
					((TransitionComponent)uc).Blank();
				}
				else if (uc as Thumbnail !=null)
				{
					ThumbnailCache.Add(uc);
					((Thumbnail)uc).Blank(); 
				}
				else if (uc as SlideshowTimeLabelComponent !=null)
				{
					TimeLabelCache.Add(uc);
					((SlideshowTimeLabelComponent)uc).Blank();
				}
				else if (uc as SlideShowSeperator !=null)
				{
					SeperatorCache.Add(uc);
                    ((SlideShowSeperator)uc).Blank();
				}
				else
				{
                    CDebugLog.GetInstance().Warning("Unknown component in call to RemoveStoryBoardUserControls");
				}
			}

		}




		//*******************************************************************
		private Thumbnail GetNewThumbnail(CImageSlide sps)
		{
			if (ThumbnailCache.Count==0)
			{
				return new Thumbnail(sps,this);
			}

			Thumbnail tn= (Thumbnail) ThumbnailCache[0];
			ThumbnailCache.Remove(tn);

			tn.ResetToSlideAndManager(sps,this);

			return tn;
		}

		//*******************************************************************
		private SlideshowTimeLabelComponent GetNewSlideshowTimeLabel(CImageSlide sps)
		{
			if (TimeLabelCache.Count==0)
			{
				return new SlideshowTimeLabelComponent(sps);
			}

			SlideshowTimeLabelComponent ssp = (SlideshowTimeLabelComponent)TimeLabelCache[0];
			TimeLabelCache.Remove(ssp);

			ssp.ResetToSlide(sps);
			return ssp;
		}


		//*******************************************************************
		private SlideShowSeperator GetNewSlideShowSeperator()
		{
			if (SeperatorCache.Count==0)
			{
				return new SlideShowSeperator();
			}

			SlideShowSeperator sss = (SlideShowSeperator)SeperatorCache[0];
			SeperatorCache.Remove(sss);

			return sss;
		}


		//*******************************************************************
		private void invalidate_images(object o)
		{
			ArrayList controls = (ArrayList)o;

			int max=1;

			if (mProgressWindow!=null)
			{
				if (controls.Count >0)
				{
					mProgressWindow.Begin(0,controls.Count);
					max = controls.Count;
				}
				else
				{
					mProgressWindow.Begin(0,max);
				}

				mProgressWindow.SetText("Rebuilding slideshow...");
			}

			int f=0;

			int total_controls = controls.Count;

			for (int jj = total_controls-1; jj>=0;jj--)
			{
                StoryboardComponent cc = (StoryboardComponent)controls[jj];

				Thumbnail tn = cc as Thumbnail;
				if (tn!=null)
				{
                    tn.InvalidateImage();
                   
				}

				TransitionComponent tc = cc as TransitionComponent;
				if (tc!=null)
				{
					tc.RebuildCurrentTransPictureBox();
				}
				if (mProgressWindow!=null)
				{
					mProgressWindow.StepTo(f);
				}
				f++;

			}
			if (mProgressWindow!=null)
			{
				mProgressWindow.StepTo(max);
			}	
		}

		//*******************************************************************
		public void RebuildAllTransitions()
		{
            if (mCurrentControls == null) return;
			ArrayList to_change = new ArrayList();
            for (int i = 0; i < mCurrentControls.Length; i++)
			{
                TransitionComponent ttt = mCurrentControls[i] as TransitionComponent;
				if (ttt!=null)
				{
					to_change.Add(ttt);
				}
			}
			foreach (TransitionComponent tt in to_change)
			{
				tt.RebuildCurrentTransPictureBox();
			}
		}


		//*******************************************************************
		public void RebuildTransitionBoxBeforeSlide(CSlide for_slide)
		{
            if (mCurrentControls == null) return;

            for (int i = 0; i < mCurrentControls.Length; i++)
			{
                TransitionComponent ttt = mCurrentControls[i] as TransitionComponent;
				if (ttt!=null)
				{
					if (ttt.NextSlide != null &&
						ttt.NextSlide.ID == for_slide.ID)
					{
						ttt.RebuildCurrentTransPictureBox();
					}

				
				
				}
			}

		}

		//*******************************************************************
		public void RebuildTransitionBoxAfterSlide(CSlide for_slide)
		{
            if (mCurrentControls == null) return;
            for (int i = 0; i < mCurrentControls.Length; i++)
			{
                TransitionComponent ttt = mCurrentControls[i] as TransitionComponent;
				if (ttt!=null)
				{
					if (ttt.Slide.ID == for_slide.ID)
					{
						ttt.RebuildCurrentTransPictureBox();
					}
				}
			}

		}


		//*******************************************************************
		public void RebuildAllLabelTimes()
		{
            if (mCurrentControls == null) return;

			ArrayList to_change = new ArrayList();
            for (int i = 0; i < mCurrentControls.Length; i++)
			{
                SlideshowTimeLabelComponent ttt = mCurrentControls[i] as SlideshowTimeLabelComponent;
				if (ttt!=null)
				{
					to_change.Add(ttt);
				}
			}
			foreach (SlideshowTimeLabelComponent tt in to_change)
			{
				tt.RebuildCurrentTransPictureBox();
			}
		}

		//*******************************************************************
		private void SlideshowThumnailsChanged()
		{
			if (mCurrentSlideShow==null) return ;

			ReCalcTrackerBar();

			if (mCurrentSlideShow.SyncLengthToMusic==true)
			{
				this.EnableSlideTimes(false);
			}
			else
			{
				this.EnableSlideTimes(true);
			}


			int image_slides = this.mCurrentSlideShow.GetNumStillPictureSlides();
			int video_slides = this.mCurrentSlideShow.GetNumVideoSlides();
            int multi_slide_slides = this.mCurrentSlideShow.GetNumMultiSlideSlides();
            image_slides += multi_slide_slides;

            int total = image_slides + video_slides;

			string slides = "slides";
			if (total==1) slides = "slide";

			string videos = "videos";
			if (video_slides==1) videos = "video";

			string images = "images";
			if (image_slides==1) images = "image";

            if (total == 0)
            {
                Form1.mMainForm.GetTotalSlidesSlideshowTextBox().Visible = false;
            }
            else
            {
                Form1.mMainForm.GetTotalSlidesSlideshowTextBox().Visible = true;
                Form1.mMainForm.GetTotalSlidesSlideshowTextBox().Text = "Total:" + total + " " + slides;
            }

			mMainForm.SlideShowManagerHasChangedCallback();
			CheckIfEnableHScrollbarOrNot();	
		}


		//*******************************************************************
		public void PanelResized(object o, System.EventArgs e)
		{
			CheckIfEnableHScrollbarOrNot();
            ReCalcHScrollBarMinMaxAndViewableValues();
            this.MoveScrollBarTo(this.mScrollPosition);
		}


        //*******************************************************************
        private void ReCalcHScrollBarMinMaxAndViewableValues()
        {
            // set the scroll min/max
#if (!DESIGN_MODE)
            ManualScrollablePanel p = mMainForm.scrollslideshowpanel as ManualScrollablePanel;

            if (p != null && mCurrentSlideShow != null)
            {
                if (p.GetHScrollBar().Visible == true)
                {
                    int items_shown_at_once = ((int)p.Width / mSlideWidth);
                    int items_now_shown = mCurrentSlideShow.mSlides.Count - items_shown_at_once;
                    if (items_now_shown < 0) items_now_shown = 0;

                    if (p.GetHScrollBar().Maximum != mCurrentSlideShow.mSlides.Count)
                    {
                        p.GetHScrollBar().Maximum = mCurrentSlideShow.mSlides.Count;
                    }

                    p.GetHScrollBar().Minimum = 0;

                    p.GetHScrollBar().LargeChange = items_shown_at_once;   // represents area seen in scroll thumb
                }
            }
#endif
        }


		//*******************************************************************
		private void CheckIfEnableHScrollbarOrNot()
		{
#if (!DESIGN_MODE)
            ManualScrollablePanel msp = Form1.mMainForm.scrollslideshowpanel as ManualScrollablePanel;

            if (msp==null) return;

			if (this.mCurrentSlideShow != null)
			{
				int items = this.mCurrentSlideShow.mSlides.Count;

				items=items*mSlideWidth;

                if (items > Form1.mMainForm.GetSlideShowPanel().Width)
                {
                    if (msp.GetHScrollBar().Enabled == false)
                    {
                        msp.GetHScrollBar().Enabled = true; ;
                    }
                }
                else
                {
                    if (msp.GetHScrollBar().Enabled == true)
                    {
                        msp.GetHScrollBar().Enabled = false; ;
                    }
                }
			}
#endif
		}

		//*******************************************************************
		public void UnHighlightAllThumbnails(bool returnToMenu)
		{
			mIgnoreHighlightCallback=true;
            if (mCurrentControls == null) return;

            for (int i = 0; i < mCurrentControls.Length; i++)
			{
                Thumbnail tn = mCurrentControls[i] as Thumbnail;
				if (tn!=null) tn.SetHighlighted(false);
			}
			mIgnoreHighlightCallback=false;
			HighlightChanged();

            if (returnToMenu && IsPlaying() == false)
            {
                this.GotoMainMenu();
            }

		}

		//*******************************************************************
		public void HighlightChanged()
		{
			if (mIgnoreHighlightCallback==true)
			{
				return;
			}

			if (this.GetHighlightesThumbnailsCount() ==0)
			{
				mMainForm.GetRotateSlideAntiClockwiseButton().Enabled=false;
				mMainForm.GetRotateSlideClockWiseButton().Enabled=false;
                mMainForm.GetStoryboardEditItemButton().Enabled = false;
			//	mMainForm.GetDecorateSlidesSlideshowButton().Enabled=false;
				mMainForm.GetSetMenuThumbnailButton().Enabled=false;
				mMainForm.GetDeleteSlidesButton().Enabled=false;
			}
			else
			{
				/*
				ArrayList hs = GetHighlightesThumbnailSlides();

				bool has_pic=false;
				foreach (CSlide s in hs)
				{
					CStillPictureSlide sps = s as CStillPictureSlide;
					if (sps!=null)
					{
						has_pic = true;
						break;
					}
				}
			*/

			//	if (has_pic==true)
			//	{
                ArrayList list = GetHighlightedThumbnails();
                if (list.Count > 0)
                {
                    Thumbnail t = list[0] as Thumbnail;
                    if (t.mSlide != null && t.mSlide.SupportsSimpleOrientationChange() != null)
                    {
                        mMainForm.GetRotateSlideAntiClockwiseButton().Enabled = true;
                        mMainForm.GetRotateSlideClockWiseButton().Enabled = true;
                    }
                    mMainForm.GetStoryboardEditItemButton().Enabled = true;
                }
			//	}

			//	mMainForm.GetDecorateSlidesSlideshowButton().Enabled=true;
				mMainForm.GetSetMenuThumbnailButton().Enabled=true;
				mMainForm.GetDeleteSlidesButton().Enabled=true;
			}
		}

		//*******************************************************************
		// when user pushed shift to multi highlight
		public void MultiHighlightSlidesToSlide(CSlide to_slide)
		{
			int start =-1;
			int end=-1;
	
			ArrayList thumbnails = this.GetAllThumbnails();

			ArrayList the_list = new ArrayList();

			// create the list in order they appear
			foreach (CSlide slide in this.mCurrentSlideShow.mSlides)
			{
				foreach (Thumbnail tn in thumbnails)
				{
					if (slide.ID == tn.mSlide.ID)
					{
						the_list.Add(tn);
					}
				}
			}

		
			foreach (Thumbnail tn in the_list)
			{
				if (tn.mHighLighted==true &&
					tn.mSlide.ID != to_slide.ID)
				{
					if (start==-1 || end !=-1)
					{
						start = tn.mSlide.ID;
					}
				}
				if (tn.mSlide.ID == to_slide.ID)
				{
					end=tn.mSlide.ID;
					if (start!=-1)
					{
						break;
					}
				}
			}

			// we're only slide highlighted just return
			if (start==-1) return;

			bool in_group = false;

			foreach (Thumbnail tn in the_list)
			{
				bool doing_end_one=false;

				if (tn.mSlide.ID == start ||
					tn.mSlide.ID == end )

				{
					if (in_group==true) in_group = false; else in_group=true;
					doing_end_one=true;
				}
				
				if (in_group==true || doing_end_one==true)
				{
					if (tn.mHighLighted==false)
					{
						tn.SetHighlighted(true);
					}
				}
			}
		}


		//*******************************************************************
		public ArrayList GetHighlightedThumbnails()
		{
			ArrayList slides = new ArrayList();
            if (mCurrentControls != null)
            {
                for (int i = 0; i < this.mCurrentControls.Length; i++)
                {
                    Thumbnail tn = mCurrentControls[i] as Thumbnail;
                    if (tn != null)
                    {
                        if (tn.mHighLighted == true)
                        {
                            slides.Add(tn);
                        }
                    }
                }
            }

			return slides;
		}

		//*******************************************************************
		public int GetHighlightesThumbnailsCount()
		{
			int c=0;
            if (mCurrentControls != null)
            {
                for (int i = 0; i < mCurrentControls.Length; i++)
                {
                    Thumbnail tn = mCurrentControls[i] as Thumbnail;
                    if (tn != null)
                    {
                        if (tn.mHighLighted == true)
                        {
                            c++;
                        }
                    }
                }
            }
			return c;
		}


		//*******************************************************************
		public ArrayList GetHighlightedThumbnailSlides()
		{
			ArrayList slides = new ArrayList();
            if (mCurrentControls != null)
            {
                for (int i = 0; i < mCurrentControls.Length; i++)
                {
                    Thumbnail tn = mCurrentControls[i] as Thumbnail;
                    if (tn != null)
                    {
                        if (tn.mHighLighted == true)
                        {
                            slides.Add(tn.mSlide);
                        }
                    }
                }
            }

			return slides;
		}

		//*******************************************************************
		public void DeleteHighlightedThumbnailsCallback(object o, System.EventArgs e)
		{
			this.DeleteHighlightedThumbnails();
		}

		//*******************************************************************
		public void DeleteHighlightedThumbnails()
		{        
            ArrayList to_remove = GetHighlightedThumbnailSlides();

            if (to_remove.Count==0)
            {
                return;
            }

            CustomButton.MiniPreviewController.StopAnyPlayingController();

			if (mCurrentSlideShow==null)
			{
                ManagedCore.CDebugLog.GetInstance().Error("Can not delete slides, null slideshow on call to DeleteHighlightedThumbnails");
				return ;
			}

            // ok work out scroll position after delete.
            // If highlighted slideshows are above scroll position, then change nothing
            // else move to first highlighted position - 3

            int new_scroll_pos = this.mScrollPosition ;
            int index=0;
            foreach (CSlide slide in mCurrentSlideShow.mSlides)
            {
                if (to_remove.Contains(slide) && index < new_scroll_pos)
                {
                    new_scroll_pos = index - 3;
                }
                index++;
            }
            if (new_scroll_pos < 0) new_scroll_pos = 0;

			mCurrentSlideShow.RemoveSlides(to_remove);

            RebuildPanel(null, new_scroll_pos);
			//if (to_remove.Contains(Form1.mMainForm.GetDecorationManager().mCurrentSlide)==true)
			{

                foreach (CSlide s in to_remove)
                {
                    s.ResetAllMediaStreams();
                }

                this.StopIfPlayingAndWaitForCompletion();
				this.GotoMainMenu();
			}
       
		}


		//*******************************************************************
		public void SetScrollToStart()
		{
            if (mMainForm != null)
            {
                MoveScrollBarTo(0);
            }
		}

		//*******************************************************************
		public void MoveHighlightedSlidesToSlideshow(CSlideShow ss)
		{
			ArrayList to_remove = GetHighlightedThumbnailSlides();
		
			if (mCurrentSlideShow==null)
			{
                ManagedCore.CDebugLog.GetInstance().Error("Can not move slides to slidehsow, null slideshow on call to MoveHighlightedSlidesToSlideshow");
				return ;
			}

			CGlobals.mCurrentProject.ForceNoMemento=true;
			mCurrentSlideShow.RemoveSlides(to_remove);
			CGlobals.mCurrentProject.ForceNoMemento=false;
			ss.AddSlides(to_remove, false);
			RebuildPanel(null);

			CSlideShowManagerThumnailCache cache = GetCachedThumnnailsForSlideshow(ss);
			if (cache!=null)
			{
				cache.mNeedRebuild=true;
			}
			HighlightChanged();

			if (to_remove.Contains(Form1.mMainForm.GetDecorationManager().mCurrentSlide)==true)
			{
				this.GotoMainMenu();
			}

		}

		//*******************************************************************
		public void CopyHighlightedSlidesToSlideshow(CSlideShow ss)
		{
			ArrayList to_copy = GetHighlightedThumbnailSlides();

			ArrayList newslides = new ArrayList();
			foreach (CSlide slide in to_copy)
			{
				newslides.Add(slide.XMLClone());
			}

			ss.AddSlides(newslides);
			CSlideShowManagerThumnailCache cache = GetCachedThumnnailsForSlideshow(ss);
			if (cache!=null)
			{
				cache.mNeedRebuild=true;
			}

			mMainForm.SlideShowManagerHasChangedCallback();
			HighlightChanged();
		}

        //*******************************************************************
        public void DuplicateHighlightedSlides()
        {
            ArrayList to_copy = GetHighlightedThumbnailSlides();

            this.StopIfPlayingAndWaitForCompletion();
            Form1.mMainForm.GoToMainMenu();
                  
            if (to_copy.Count == 0) return;

            ArrayList newslides = new ArrayList();
            foreach (CSlide slide in to_copy)
            {
                newslides.Add(slide.XMLClone());
            }

            MiniPreviewController mpc = CustomButton.MiniPreviewController.GetCurrentPlayingController();
            if (mpc != null)
            {
                mpc.Pause();
            }

            mCurrentSlideShow.AddSlides(newslides, false, to_copy[to_copy.Count-1] as CSlide, null);
          
            mMainForm.SlideShowManagerHasChangedCallback();
            HighlightChanged();

            this.RebuildPanel();

            mpc = CustomButton.MiniPreviewController.GetCurrentPlayingController();
            if (mpc != null)
            {
                mpc.Continue();
            }

        }


		//*******************************************************************
		public void UpdateTimeLabel()
		{
			if (mCurrentSlideShow==null) return ;

            // This can be called from project change updates (if authoring, do not to change anything)
            if (CGlobals.mVideoGenerateMode == VideoGenerateMode.FINAL_OUTPUT_MODE)
            {
                return;
            }

            Label l = this.mTimeLabel; // this.mMainForm.GetSlideShowTimeLabel();

            double total_time = (double)mCurrentSlideShow.GetTotalRenderLengthInFrames() / CGlobals.mCurrentProject.DiskPreferences.frames_per_second;
			double current_time = (double) mCount / CGlobals.mCurrentProject.DiskPreferences.frames_per_second;

			//DateTime tt = DateTime.Now;
			//DateTime ct = DateTime.Now;

			DateTime tt = new DateTime((long)(total_time*10000000.0f));
			DateTime ct = new DateTime((long)(current_time*10000000.0f));

		
			string new_label_tt = System.String.Format("{0:d2}:{1:d2}:{2:d2}.{3:d2}", tt.Hour,tt.Minute,tt.Second,tt.Millisecond/10);

			string new_label_ct = System.String.Format("{0:d2}:{1:d2}:{2:d2}.{3:d2}", ct.Hour,ct.Minute,ct.Second,ct.Millisecond/10);

        	this.SetTimeLabelText(" "+new_label_ct+ "\r\n(" +new_label_tt+")") ;

			//	l.Text = current_time.ToString() + "/" + total_time.ToString();
		}

        //*******************************************************************
     //   private delegate void SetTimeLabelTextDelegate(string val);
        private void SetTimeLabelText(string val)
        {
            this.mTimeLabel.Text = val;
        }


		//*******************************************************************
		public void StartDragSlide()
		{
			ArrayList list = GetHighlightedThumbnails();
			if (list.Count <1)
			{
                ManagedCore.CDebugLog.GetInstance().Error("No highlighted images to drag in StartDragSlide");
				return ;
			}

            int i=0; 
			Thumbnail tn = list[i] as Thumbnail;

            // Make sure we get a thumbnail that has a physical control
            while (tn.ForThumbnailControl == null && i < list.Count - 1)
            {
                i++;
                tn = list[i] as Thumbnail;
            }

            try
            {
                if (tn.ForThumbnailControl != null)
                {

                    ArrayList slide_list = GetHighlightedThumbnailSlides();

                    DragDropEffects dropEffect = tn.ForThumbnailControl.DoDragDrop(slide_list, DragDropEffects.All | DragDropEffects.Move);
                }
            }
            finally
            {
                this.mDragScrollTimer.Stop();
            }
		}


		//*******************************************************************
		public void DecorateButton_Click(object sender, System.EventArgs e)
		{
            ArrayList list = GetHighlightedThumbnails();

			StopIfPlayingAndWaitForCompletion(false);
	
			if (list.Count <1) return ;

			Thumbnail s = list[0] as Thumbnail ;

			if (s!=null)
			{
				this.mMainForm.GetDecorationManager().SetCurrentSlide(s.mSlide, s);
			}
		}

		//*******************************************************************
		public void FillInGuiMemento(CGuiMemento m)
		{
			int index=0;
			if (this.mCurrentSlideShow==null)
			{
				m.mSlideShowIndex=0;
				return ;
			}

			foreach (CSlideShow s in CGlobals.mCurrentProject.GetAllProjectSlideshows(true))
			{
				if (s.ID == this.mCurrentSlideShow.ID)
				{
					m.mSlideShowIndex=index;
				}
				index++;
			}
		}

		//*******************************************************************
		public void SetMemento(CGuiMemento m)
		{	
			this.SetCurrentSlideShow(CGlobals.mCurrentProject.GetAllProjectSlideshows(true)[m.mSlideShowIndex] as CSlideShow);
		}

		//*******************************************************************
		public void SetSlideAsMenuThumbnailCallback(object o,System.EventArgs e)
		{
			ArrayList list = this.GetHighlightedThumbnails();
			if (list.Count!=0)
			{
				SetSlideAsMenuThumbnail((list[0] as Thumbnail).mSlide);
			}
		}

		//*******************************************************************
		public void SetSlideAsMenuThumbnail(CImageSlide slide)
		{
			if (mCurrentSlideShow==null) return ;

			// that nasty hack again
			CGlobals.MainMenuNeedsReRender=true;
			mCurrentSlideShow.MenuThumbnailSlide = slide;
            Form1.mMainForm.GoToMainMenu();
			Form1.mMainForm.GetDecorationManager().RePaint();
		}


        //*******************************************************************
        // If 'insertAfter' set to true, then it inserts after a selected slide (if one set)
        // else it inserts before the selected slide.
        // If nothing is selected, it simply appends to the end of slideshow.
        public void AddBlankSlideButtonClicked(bool insertAfter)
        {
            if (this.mCurrentSlideShow == null) return;

            this.StopIfPlayingAndWaitForCompletion();
            mMainForm.GoToMainMenu(false);

            // check we not gone over the max slide count
            if (this.mCurrentSlideShow.mSlides.Count >= CGlobals.MaxSlidesPerSlideshow)
            {
                UserMessage.Show("There is a maximum of " + CGlobals.MaxSlidesPerSlideshow.ToString() + " slides allowed per slideshow.", "Too many slides",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }

            CreateBlankSlideForm cbsf = new CreateBlankSlideForm();

            System.Windows.Forms.DialogResult result = cbsf.ShowDialog(mMainForm);
              
            if (result == DialogResult.OK)
            {
                CBlankStillPictureSlide slide = cbsf.ResultingSlide as CBlankStillPictureSlide;
                if (slide != null)
                {
                    ArrayList slides = new ArrayList();
                    slides.Add(slide);

                    ArrayList highLightedSlides = GetHighlightedThumbnailSlides();

                    if (highLightedSlides.Count == 0)
                    {
                        mCurrentSlideShow.AddSlides(slides);
                    }
                    else
                    {
                        if (insertAfter == true)
                        {
                            mCurrentSlideShow.AddSlides(slides, true, highLightedSlides[0] as CSlide, null);
                        }
                        else
                        {
                            mCurrentSlideShow.AddSlides(slides, true, null, highLightedSlides[0] as CSlide);
                        }
                    }
                }

                RebuildPanel(null, mScrollPosition);
            }

        }

    
        //*******************************************************************
        public void manualScrollPanel_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                if (mScrollPosition != e.NewValue)
                {
                    mScrollPosition = e.NewValue;
                    this.ShowCurrentSlidesFromScrollPosition(mScrollPosition);

                    // This refresh force immediate re-draw of the scroll panel, which looks better
                    this.mMainForm.GetSlideShowPanel().Refresh();
                }
            }  
        }

        //*******************************************************************
        private void InvalidateAllThumbnailsThreadStart(object o)
        {
            ArrayList list = GetAllThumbnails();

            int count = list.Count;
            int index = 0;

            mProgressWindow.Begin(0, count);
            mProgressWindow.SetText("Rebuilding slideshow");
            foreach (Thumbnail thumb in list)
            {  
                thumb.InvalidateImage();
                index++;
                mProgressWindow.StepTo(index);
            }

            mProgressWindow.End();
        }


        //*******************************************************************
        public void InvalidateAllThumbnails()
        {
            mProgressWindow = new ProgressWindow(InvalidateAllThumbnailsThreadStart, null);//true);
            mProgressWindow.Text = "Rebuilding slideshow";
            mProgressWindow.CancelButtons.Visible = false;
            mProgressWindow.ShowDialog();
            mProgressWindow = null;
        }

        
        //*******************************************************************
        public int GetCurrentTrackBarFrame()
        {
            return mTrackBar.Value;
        }


         //*******************************************************************
        private Thumbnail GetThumbnailForSlide(CSlide s)
        {
            ArrayList list = this.GetAllThumbnails();
            foreach (Thumbnail thumb in list)
            {
                if (thumb.mSlide == s)
                {
                    return thumb;
                }
            }
            return null;
        }

        
        //*******************************************************************
        public Thumbnail HighlightSlide(CSlide slide)
        {
            Thumbnail thumb = GetThumbnailForSlide(slide);
            if (thumb != null)
            {
                thumb.SetHighlighted(true);
            }
            return thumb;
        }

        //*******************************************************************
        public void CreateSlideFromTemplateClicked()
        {
            if (this.mCurrentSlideShow == null) return;
            this.StopIfPlayingAndWaitForCompletion();
            mMainForm.GoToMainMenu(false);

            CreateSlideFromTemplateForm csftf = new CreateSlideFromTemplateForm();
            DialogResult result = csftf.ShowDialog();
       
            if (result == DialogResult.OK)
            {
                CImageSlide slide = csftf.ResultingSlide;
                ArrayList list = new ArrayList();
                list.Add(slide);

                ArrayList highLightedSlides = GetHighlightedThumbnailSlides();

                if (highLightedSlides.Count == 0)
                {
                    mCurrentSlideShow.AddSlides(list, false, null, null, false);
                }
                else
                {
                    mCurrentSlideShow.AddSlides(list, false, highLightedSlides[0] as CSlide, null, false);
                }

                this.RebuildPanel();

                bool slideTimeChanged = false;
                // If slide contains something we can edit then bring up the editslide media form
                if (EditSlideMediaForm.ContainsEditableDecoration(csftf.ResultingSlide) == true)
                {
                    EditSlideMediaForm esmf = new EditSlideMediaForm(slide, mCurrentSlideShow, null);
                    esmf.ShowDialog();
                    slideTimeChanged = esmf.SlideLengthChanged;
                }

                Thumbnail t = GetThumbnailForSlide(slide);
                if (t != null)
                {
                    t.InvalidateImage(slideTimeChanged);
                }

                CGlobals.mCurrentProject.DeclareChange("Edit slide media");
                 
                // does not work
                //Form1.mMainForm.GetDecorationManager().RePaint();      
            }
        }

	}

	// when you're not working on a slideshow the program caches/stores the thumnails in that slideshow ,
	// so if you re-open the slideshow it doesn't have to rebuild all the thumbnails.  Speeds up
	// editing.

	public class CSlideShowManagerThumnailCache
	{
		public DVDSlideshow.CSlideShow for_slideshow;
		public ArrayList usercontrols;

		// this flag means the slideshow this cahce represents has changed whilst it was cached/stored 
		// i.e this can only happen if someone moves/copies some slides to this slideshow whilst editing
		// another slidesshow
		public bool mNeedRebuild = false;


		public CSlideShowManagerThumnailCache(DVDSlideshow.CSlideShow for_show)
		{
			for_slideshow = for_show;
			usercontrols = new ArrayList();
		}
	}

}
