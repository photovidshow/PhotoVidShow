using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using DVDSlideshow;
using System.Drawing;
using ManagedCore;
using CustomButton;

namespace dvdslideshowfontend
{
    /// <summary>
    /// Summary description for CSlideShowNarrationManager.
    /// </summary>
    public class CSlideShowNarrationManager : CSlideShowAudioManager
    {

        private System.Windows.Forms.OpenFileDialog mOpenFileDialog;
        private DateTime mLastShownToolTipLabel = new DateTime();   // used to prevent it being shown to rapid slowing the gui
        private bool mCheckForScrollTimerRunning = false;    // flag indicating that we are current moving and scrolling a narration
        private Timer mScrollTimer = null;
        private MusicThumbnail mScrollingThumbnail = null;
        private float mStartScrollTime = 0;

        //*******************************************************************
        public CSlideShowNarrationManager(Form1 main_window) : base(main_window)
        {
            mOpenFileDialog = new System.Windows.Forms.OpenFileDialog();

            mOpenFileDialog.Filter = CGlobals.GetTotalAudioFilter();
            mScrollTimer = new Timer();
            mScrollTimer.Interval = 50;
            mScrollTimer.Stop();
            mScrollTimer.Tick += new EventHandler(mScrollTimer_Tick);

            string myMusic = DefaultFolders.GetFolder("MyMusic");
            mOpenFileDialog.InitialDirectory = myMusic;
            mOpenFileDialog.Title = "Open narration";
            mOpenFileDialog.Multiselect = true;
 
            main_window.GetDoNarrationButton().ButtonClick+= new EventHandler(mDoNarrationButton_Click);
            main_window.GetAddNarrationFromAudioFileToolStripMenuItem().Click += new EventHandler(AddNarrationFromAudioFileToolStripMenuItem_Click);
        }

      
        //*******************************************************************
        protected override int GetThumnailsYPosition()
        {
            return 157;
        }

        //*******************************************************************
        protected override ICollection GetMusicSlides()
        {
            return GetCurrentSlideShow().NarrationSlides;
        }

        //*******************************************************************
        protected override bool MoveableThumbnails()
        {
            return true;
        }

        //*******************************************************************
        protected override MusicThumbnail CreateThumbnail(CMusicSlide ms)
        {
            return new MusicThumbnail(ms, this, MusicThumbnail.AudioType.NARRATION);
        }

        //*******************************************************************
        private float CalcFrameThumbnailIsOn(MusicThumbnail thumnail)
        {
            // create the thumbnails for the selected files
            int scroll_position = this.mMainForm.GetSlideShowManager().ScrollPosition * this.mMainForm.GetSlideShowManager().SlideWidth;

            int x = thumnail.Left + scroll_position - mInitialXoffset;

            float frame = GetSlideFrameFromXOffset(x);

            return frame;
        }

        //*******************************************************************
        private float ReCalcThumbnailWidth(MusicThumbnail thumbnail)
        {
            CNarrationAudioSlide narration = thumbnail.mMusicSlide as CNarrationAudioSlide;

            float frame = CalcFrameThumbnailIsOn(thumbnail);

            float time = frame / CGlobals.mCurrentProject.DiskPreferences.AudioFramesPerSecond;
            if (time < 0) time = 0;

            int startFrameOffset = (int)((time * CGlobals.mCurrentProject.DiskPreferences.AudioFramesPerSecond) + 0.4999f);

            int fr = startFrameOffset + narration.GetFinalOutputLengthInFrames();

            int startXPos = GetXOffsetForSlideFrame(startFrameOffset) + mInitialXoffset;

            int width = GetXOffsetForSlideFrame(fr) - startXPos + mInitialXoffset;

            if (thumbnail.Width != width)
            {
                thumbnail.Width = width;
            }

            return time;
        }


        //*******************************************************************
        public override void ThumbnailStartedMoving(MusicThumbnail thumbnail)
        {
            mCheckForScrollTimerRunning = false;
            mScrollingThumbnail = null;

            // move thumbnail to top

            mCurrentMusicSlideThumbnails.Remove(thumbnail);
            mCurrentMusicSlideThumbnails.Add(thumbnail);

            CNarrationAudioSlide nas = thumbnail.mMusicSlide as CNarrationAudioSlide;
            if (nas == null) return;
            mStartScrollTime = nas.StartNarrationTime;
           
        }

    
        //*******************************************************************
        public override void ThumbnailMoving(MusicThumbnail thumbnail, int newPos)
        {
            // story board is not linear space!? so we have to recalc music thumbnail width as it is being moved
        
            if (mCheckForScrollTimerRunning == false)
            {
                int xpos = thumbnail.Left;

                mCheckForScrollTimerRunning = true;
                CNarrationAudioSlide narration = thumbnail.mMusicSlide as CNarrationAudioSlide;
                narration.StartNarrationTime = ReCalcThumbnailWidth(thumbnail);

                this.mMainForm.GetSlideShowManager().SetStartDragScrollPosition();
                this.mMainForm.GetSlideShowManager().ScrollPanelIfCursorOutsideStoryboardPanel();
                mScrollingThumbnail = thumbnail;
                thumbnail.Left = xpos;
                narration.StartNarrationTime = ReCalcThumbnailWidth(mScrollingThumbnail);
                mScrollTimer.Start();
                this.mMainForm.GetSlideShowManager().mCurrentSlideShow.CalcLengthOfAllSlides();
                CheckThumbnailNotMovedPassEndOfSlideshow(narration.StartNarrationTime, narration);
            }
            else
            {
                float startTime = ReCalcThumbnailWidth(mScrollingThumbnail);
                CNarrationAudioSlide narration = thumbnail.mMusicSlide as CNarrationAudioSlide;
                CheckThumbnailNotMovedPassEndOfSlideshow(startTime, narration);
            }


            DateTime dt = DateTime.Now;
            TimeSpan ts = dt - mLastShownToolTipLabel;
            if (Math.Abs(ts.TotalMilliseconds) > 15)
            {
                float time = ReCalcThumbnailWidth(thumbnail);

                thumbnail.ShowToolTipLabel(time, 10000);
                mLastShownToolTipLabel = dt;
            }

        }

        //*******************************************************************
        private void mScrollTimer_Tick(object sender, EventArgs e)
        {
            if (mScrollingThumbnail != null)
            {
                CNarrationAudioSlide narration = mScrollingThumbnail.mMusicSlide as CNarrationAudioSlide;
                if (narration != null)
                {
                    int xpos = mScrollingThumbnail.Left;
                    narration.StartNarrationTime = ReCalcThumbnailWidth(mScrollingThumbnail);
                    CheckThumbnailNotMovedPassEndOfSlideshow(narration.StartNarrationTime, narration);

                    this.mMainForm.GetSlideShowManager().ScrollPanelIfCursorOutsideStoryboardPanel();
                    mScrollingThumbnail.Left = xpos;
                    narration.StartNarrationTime = ReCalcThumbnailWidth(mScrollingThumbnail);              
                    this.mMainForm.GetSlideShowManager().mCurrentSlideShow.CalcLengthOfAllSlides();
                    CheckThumbnailNotMovedPassEndOfSlideshow(narration.StartNarrationTime, narration);
                }
            }
        }

        //*******************************************************************
        private float CheckThumbnailNotMovedPassEndOfSlideshow(float candidateStartTime, CNarrationAudioSlide nas)
        {
            float startTime = candidateStartTime;

            float length = (float) mMainForm.GetSlideShowManager().mCurrentSlideShow.GetLengthInSeconds();
            if (startTime > length)
            {
                float suggesttime =length - ((float)nas.GetDurationInSeconds());

                if (mStartScrollTime < suggesttime)
                {
                    nas.StartNarrationTime = suggesttime;
                }
                else
                {
                    nas.StartNarrationTime = mStartScrollTime;
                }

                startTime = nas.StartNarrationTime;

                this.DrawFromCurrentSrcollPosition();
            }

            return startTime;
        }


        //*******************************************************************
        public override void ThumbnailMoved(MusicThumbnail thumbnail)
        {
            mScrollTimer.Stop();
            mCheckForScrollTimerRunning = false;
            mScrollingThumbnail = null;

            thumbnail.HideToolTipLabel();

            CNarrationAudioSlide narration = thumbnail.mMusicSlide as CNarrationAudioSlide;

            float startTime = ReCalcThumbnailWidth(thumbnail);
            float endTime = startTime + ((float)narration.GetDurationInSeconds());

            startTime = CheckThumbnailNotMovedPassEndOfSlideshow(startTime, narration);

            CSlideShow slideshow = GetCurrentSlideShow();
            // Ok do we overlap, if so can we move a bit to solve problem

            List<CNarrationAudioSlide> slides = slideshow.NarrationSlides;

            bool foundPlacement = false;
            bool giveUp = false;

            while (giveUp == false && foundPlacement == false)
            {
                foreach (CNarrationAudioSlide slide in slides)
                {
                    if (slide == thumbnail.mMusicSlide)
                    {
                        if (slides.Count == 1)
                        {
                            foundPlacement = true;
                        }

                        continue;
                    }

                    float hangingBefore=0;
                    float hangingAfter=0;

                    if (slide.Overlaps(startTime, endTime, ref hangingBefore, ref hangingAfter)==true)
                    {
                        if (hangingBefore > hangingAfter)
                        {
                            startTime = slide.StartNarrationTime - ((float)narration.GetDurationInSeconds()); ;
                        }
                        else
                        {
                            startTime = (float) (slide.StartNarrationTime + slide.GetDurationInSeconds());
                        }
                        endTime = (float) (startTime + narration.GetDurationInSeconds());

                        // new time outside scope of slideshow?
                        if (startTime < 0 || endTime > slideshow.GetLengthInSeconds())
                        {
                            giveUp = true;
                            foundPlacement = false;

                        }

                        // does moved new time now overlaps another slide?

                        foreach (CNarrationAudioSlide secondCheckSlide in slides)
                        {
                            if (secondCheckSlide == thumbnail.mMusicSlide) continue;
                            if (secondCheckSlide.Overlaps(startTime, endTime, ref hangingBefore, ref hangingAfter) == true)
                            {
                                // it does, just give up
                                giveUp=true;
                                foundPlacement = false;

                                break;
                            }
                        }
                            
                        break;
                    } // found overlap

                    foundPlacement = true;
                } // loop through each slide 
            }// while

            if (foundPlacement == true)
            {
                narration.StartNarrationTime = startTime;
                narration.CalcLengthInFrame();
            }
            else
            {
                narration.StartNarrationTime = mStartScrollTime;
                narration.CalcLengthInFrame();
            }

            //
            // We may have now changed the order of the narrations, so tell the slideshow manager to re-sort the narration array
            //
            slideshow.ReSortNarrationSlides();
                
            // did we find an overlap
            //    if (foundOverlap == true)
            {
                this.RebuildPanel();
            }

            if (mStartScrollTime != narration.StartNarrationTime)
            {
                CGlobals.mCurrentProject.DeclareChange("Moved narration");
            }
        }

        //*******************************************************************
        private void mDoNarrationButton_Click(object sender, EventArgs e)
        {
            ArrayList list = mMainForm.GetSlideShowManager().GetHighlightedThumbnailSlides();

            CSlide fromSlide = null;
            if (list.Count > 0)
            {
                fromSlide = list[0] as CSlide;
            }

            RecordNarration(fromSlide);
        }

        //*******************************************************************
        // If 'fromSlide' null then it will start recording from current preview tracker bar position
        //
        public void RecordNarration(CSlide fromSlide)
        {
            CSlideShowManager slideshowManager = mMainForm.GetSlideShowManager();

            if (slideshowManager != null) slideshowManager.StopIfPlayingAndWaitForCompletion();

            mMainForm.GoToMainMenu(false);

            if (fromSlide != null)
            {
                float addOffsetTime = 0;
                CSlide previousSlide = slideshowManager.mCurrentSlideShow.GetPreviousSlide(fromSlide);
                if (previousSlide!=null && previousSlide.TransistionEffect != null)
                {
                    addOffsetTime = previousSlide.TransistionEffect.Length / 2;
                }

                slideshowManager.MoveTrackerToSlide(fromSlide, addOffsetTime);
            }

            CustomButton.RecordNarrationForm rnf = new RecordNarrationForm(slideshowManager.Play, slideshowManager.Stop, slideshowManager.PauseVideoPreview);

            int devices = rnf.GetDevicesCount();
            if (devices == 0)
            {
                UserMessage.Show("No record devices found", "No device", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (slideshowManager.mCurrentSlideShow.mSlides.Count <1)
            {
                UserMessage.Show("Can not record narration as there are no slides in the slideshow.", "No slides", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            rnf.StartPosition = FormStartPosition.Manual;
            rnf.Location = new Point(mMainForm.Location.X + 70, mMainForm.Location.Y + 150);
       
            int recordStartFrame = slideshowManager.GetCurrentTrackBarFrame();
            float recordStartTime = recordStartFrame / CGlobals.mCurrentProject.DiskPreferences.AudioFramesPerSecond;

            rnf.RecordStartTime = recordStartTime;

            slideshowManager.SlideshowPreviewFinished += new CSlideShowManager.SlideshowPreviewFinishedCallback(rnf.SlideshowFinishedCallback);

            try
            {
                rnf.ShowDialog();
            }
            finally
            {
                slideshowManager.SlideshowPreviewFinished -= new CSlideShowManager.SlideshowPreviewFinishedCallback(rnf.SlideshowFinishedCallback);
            }

            if (rnf.OutputWMAFilename != "")
            {
                CNarrationAudioSlide narration = new CNarrationAudioSlide(rnf.OutputWMAFilename);
                narration.StartNarrationTime = recordStartTime;
                slideshowManager.mCurrentSlideShow.RemoveOrShortenOverlapppingNarrations(narration);
                bool narrationStartsAfterSlideshow = false;
                slideshowManager.mCurrentSlideShow.AddNarrationSlide(narration,-1, out narrationStartsAfterSlideshow);

                this.RebuildPanel();
            }

        }

        //*******************************************************************
        private void AddNarrationFromAudioFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();
            mMainForm.GoToMainMenu();

            if (AddNarrationDialogStart() == true)
            {
                this.RebuildPanel();
            }   
        }

        //*******************************************************************
        public bool AddNarrationDialogStart()
        {
            if (mOpenFileDialog.ShowDialog().Equals(System.Windows.Forms.DialogResult.OK))
            {
                if (GetCurrentSlideShow() == null)
                {
                    ManagedCore.Log.Error("Trying to add a narration to a null slideshow in AddNarrationDialogStart");
                    return false;
                }

                if (mOpenFileDialog.FileNames.Length > 0)
                {
                    if (ManagedCore.IO.IsDriveOkToUse(mOpenFileDialog.FileNames[0]) == false) return false;

                    mOpenFileDialog.InitialDirectory = mOpenFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(mOpenFileDialog.FileNames[0]);  // rember current folder

                    int currentScrollPos = mMainForm.GetSlideShowManager().ScrollPosition;

                    bool narrationStartsAfterSlideshowEnd = false;
                    GetCurrentSlideShow().AddNarrationSlides(mOpenFileDialog.FileNames, currentScrollPos, out narrationStartsAfterSlideshowEnd);

                    if (narrationStartsAfterSlideshowEnd == true)
                    {
                        mMainForm.GetSlideShowMusicManager().AudioStartsAfterSlideshowEndsWarning("narration");
                    }
                    return true;
                }
            }
            return false;
        }


        //*******************************************************************
        public override void DeleteItem(CMusicSlide item)
        {

            ArrayList to_remove = new ArrayList();

            to_remove.Add(item);

            if (GetCurrentSlideShow() == null)
            {
                Log.Error("Can not delete audio slides, null slideshow in DeleteItem ");
                return;
            }

            GetCurrentSlideShow().RemoveNarrationSlides(to_remove);
        }

    }
}
