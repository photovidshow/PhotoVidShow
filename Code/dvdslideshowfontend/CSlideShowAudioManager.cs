using System;
using System.Collections.Generic;
using System.Text;
using DVDSlideshow;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using ManagedCore;

namespace dvdslideshowfontend
{
    public abstract class CSlideShowAudioManager
    {
        protected Form1 mMainForm;
        protected int mCount = 0;
        protected List<MusicThumbnail> mCurrentMusicSlideThumbnails = new List<MusicThumbnail>();

        protected int mInitialXoffset = 2;    // number of x pixels in music panel which is time 0:00:00

        //*******************************************************************
        public CSlideShowAudioManager(Form1 mainForm)
        {
            mMainForm = mainForm;
        }


        //*******************************************************************
        public void SetImageCount(int val)
        {
            mCount = val;
        }

        
        //*******************************************************************
        public void RebuildPanel()
        {
            List<MusicThumbnail> toDispose = new List<MusicThumbnail>();
            toDispose.AddRange(mCurrentMusicSlideThumbnails);

            mCurrentMusicSlideThumbnails.Clear();

            DrawFromCurrentSrcollPosition();

            foreach (MusicThumbnail mt in toDispose)
            {
                mt.Dispose();
            }
        }
        
        //*******************************************************************
        protected abstract int GetThumnailsYPosition();

        //*******************************************************************
        protected abstract ICollection GetMusicSlides();

        //*******************************************************************
        protected virtual bool MoveableThumbnails()
        {
            return false;
        }

        //*******************************************************************
        public Panel GetMusicPanel()
        {
            return mMainForm.GetSlideShowMusicPlanel();
        }

        //*******************************************************************
        // used for timeline
        public int GetXOffsetForSlideFrame(int frame)
        {
            int l = 0;
            int x_off = 0;
            int slide_n = 0;

            CSlideShow currentSlideshow = this.GetCurrentSlideShow();

            int slideWidth = mMainForm.GetSlideShowManager().SlideWidth;

            if (currentSlideshow.mSlides.Count == 0)
            {
                return 800;
            }

            foreach (CSlide slide in currentSlideshow.mSlides)
            {
                slide_n++;
                int so = slide.mStartFrameOffset;
                int fl = slide.CalcFrameLengthMinusTransition();

                //
                // The slide transition effect may start the same time as mStartFrameOffset, but for this calculation we assume a minimum of
                // 1 frame
                //
                if (fl < 1) fl = 1;
        
                int slideStartTransitionFrame = so + fl;

                if (frame < slideStartTransitionFrame)
                {
                    float ss = (float)slideWidth;
                    float ffl = (float)(fl);

                    if (ffl != 0)
                    {
                        float ratio = ((float)(frame - so)) / ffl;
                        int rr = (int)(ss * ratio);
                        return rr + x_off;
                    }
                    else
                    {
                        return x_off;
                    }
                }
                x_off += slideWidth;

                l += fl;
            }

            return x_off - 50;

        }


        //*******************************************************************
        public float GetSlideFrameFromXOffset(int x)
        {
            int slideWidth = mMainForm.GetSlideShowManager().SlideWidth;

            float slidefract = ((float)x) / ((float)slideWidth);

            int slidenum = (int)slidefract;

            CSlideShow currentSlideshow = this.GetCurrentSlideShow();

            if (slidenum >= currentSlideshow.mSlides.Count) slidenum = currentSlideshow.mSlides.Count - 1;
            if (slidenum < 0) return 0;

            CSlide slide = currentSlideshow.mSlides[slidenum] as CSlide;

            int so = slide.mStartFrameOffset;
            int fl = slide.CalcFrameLengthMinusTransition();
            if (fl < 0) fl = 0;

            float fracIntoSlide = slidefract - slidenum;

            float frameIntoSlide = ((float)fl) * fracIntoSlide;

            float frame = so + frameIntoSlide;

            return frame;
        }


        //*******************************************************************
        protected abstract MusicThumbnail CreateThumbnail(CMusicSlide slide);

        //*******************************************************************
        public void DrawFromCurrentSrcollPosition()
        {
            Form1.mMainForm.GetSlideShowManager().AddRemoveBlankStoryboardLabels(); // remove/add labels

            int yposition = GetThumnailsYPosition();
            // clean up any thumbnails already present
            ArrayList to_remove = new ArrayList();
            for (int i = 0; i < mMainForm.GetSlideShowMusicPlanel().Controls.Count; i++)
            {
                MusicThumbnail tn = mMainForm.GetSlideShowMusicPlanel().Controls[i] as MusicThumbnail;
                if (tn != null && tn.mManager == this)
                {
                    to_remove.Add(tn);
                }
            }

            foreach (MusicThumbnail tn in to_remove)
            {
                mMainForm.GetSlideShowMusicPlanel().Controls.Remove(tn);
            }

            // rebuild
            if (GetCurrentSlideShow() == null) return;

            // create the thumbnails for the selected files
            int scroll_position = this.mMainForm.GetSlideShowManager().ScrollPosition * this.mMainForm.GetSlideShowManager().SlideWidth;

           

            int music_index = 0;
            int total_music = GetCurrentSlideShow().mMusicSlides.Count;

            int panel_width = mMainForm.GetSlideShowMusicPlanel().Width;

            ICollection musicSlides = GetMusicSlides();

            foreach (CMusicSlide ms in musicSlides)
            {
                MusicThumbnail thumbnail = null;

                if (ms != null)
                {
                    int x = GetXOffsetForSlideFrame(ms.StartFrameOffset) + mInitialXoffset;
                    int fr = ms.StartFrameOffset + ms.GetFinalOutputLengthInFrames();

                    int width = GetXOffsetForSlideFrame(fr) - x + mInitialXoffset;

                
                    int start_pos = x - scroll_position;
                    int end_pos = x - scroll_position + width;

                    // are we on visible, if so create the thumbnail and add it to the storyboard panel
                    if (end_pos > 0 && start_pos < panel_width)
                    {
                       
                        foreach (MusicThumbnail old_ms in mCurrentMusicSlideThumbnails)
                        {
                            if (old_ms.mMusicSlide == ms)
                            {
                                thumbnail = old_ms;
                                break;
                            }
                        }
                        if (thumbnail == null)
                        {
                            thumbnail = CreateThumbnail(ms);

                            AddThumbnailsRightClickOptions(thumbnail, music_index, total_music);

                            mCurrentMusicSlideThumbnails.Add(thumbnail);
                        }

                        // set the position for the thumbnail and add it to the panel's controls
                        thumbnail.Left = x - scroll_position;
                        thumbnail.Width = width;
                        thumbnail.Height = 24;
                        thumbnail.Top = yposition;

                        // make sure not too big outside pabel viewing area
                        if (thumbnail.Left < -1000)
                        {
                            thumbnail.Width -= ((-thumbnail.Left) - 1000);
                            thumbnail.Left += ((-thumbnail.Left) - 1000);
                        }

                        if (thumbnail.Left + thumbnail.Width > panel_width + 1000)
                        {
                            thumbnail.Width -= thumbnail.Left + thumbnail.Width - (panel_width + 1000);
                        }

                        int textLeftMargin = thumbnail.MusicLabelTextLeftMargin;

                        //
                        // If the music thumbnail is partially off the screen on the left, then move text right so
                        // it becomes visible
                        //
                        if (thumbnail.Left < 0)
                        {                     
                            int newXPos = (textLeftMargin * 2) - thumbnail.Left;

                            //
                            // Make sure the new text position can fit in.  If not move it back a bit to fit in
                            //
                            int textWidth = thumbnail.MusicLabelText.PreferredWidth;
                            if (newXPos + textWidth > thumbnail.Width)
                            {
                                newXPos = thumbnail.Width - textWidth;
                                if (newXPos < textLeftMargin)
                                {
                                    newXPos = textLeftMargin;
                                }
                            }

                            //
                            // Move text
                            //
                            thumbnail.MusicLabelText.Location = new Point(newXPos, thumbnail.MusicLabelText.Location.Y);
                        }
                        else
                        {
                            thumbnail.MusicLabelText.Location = new Point(textLeftMargin, thumbnail.MusicLabelText.Location.Y);
                        }
                        mMainForm.GetSlideShowMusicPlanel().Controls.Add(thumbnail);
                    } 

                    music_index++;
                }
            }
        }

        //*******************************************************************
        protected virtual void AddThumbnailsRightClickOptions(MusicThumbnail thumbnail, int music_index, int total_music)
        {
        }


        //*******************************************************************
        protected CSlideShow GetCurrentSlideShow()
        {
            return this.mMainForm.GetSlideShowManager().mCurrentSlideShow; ;
        }


        //*******************************************************************
        public void DeleteItem(MusicThumbnail item)
        {
            this.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();

            DeleteItem(item.mMusicSlide);

            RebuildPanel();

            // Rebuild combo if we were synced to music
            if (GetCurrentSlideShow().SyncLengthToMusic == true)
            {
                Form1.mMainForm.GetSlideShowManager().RebuildPanel(null);
            }
        }


        //*******************************************************************
        public abstract void DeleteItem(CMusicSlide item);


        //*******************************************************************
        public void MoveMusicUpOrder(MusicThumbnail item)
        {
            if (MoveMusicUpOrder(item.mMusicSlide) == true)
            {
                RebuildPanel();
            }
        }

        //*******************************************************************
        public bool MoveMusicUpOrder(CMusicSlide ms)
        {
            this.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();

            ArrayList musicSlides = GetCurrentSlideShow().mMusicSlides;

            int index = musicSlides.IndexOf(ms);
            if (index >= 1)
            {
                musicSlides.Remove(ms);
                musicSlides.Insert(index - 1, ms);
                GetCurrentSlideShow().CalcLengthOfAllSlides();

                return true;
            }

            return false;
        }

        //*******************************************************************
        public void MoveMusicDownOrder(MusicThumbnail item)
        {
            if (MoveMusicDownOrder(item.mMusicSlide) == true)
            {
                RebuildPanel();
            }
        }
        //*******************************************************************
        public bool MoveMusicDownOrder(CMusicSlide ms)
        {
            this.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();

            ArrayList musicSlides = GetCurrentSlideShow().mMusicSlides;

            int index = musicSlides.IndexOf(ms);
            if (index >= 0 && index < musicSlides.Count)
            {
                musicSlides.Remove(ms);
                musicSlides.Insert(index + 1, ms);
                GetCurrentSlideShow().CalcLengthOfAllSlides();

                return true;
            }
            return false;
        }


        //*******************************************************************
        public virtual void ThumbnailStartedMoving(MusicThumbnail thumbnail)
        {
        }

        //*******************************************************************
        public virtual void ThumbnailMoving(MusicThumbnail thumbnail, int MousePos)
        {   
        }

        //*******************************************************************
        public virtual void ThumbnailMoved(MusicThumbnail thumbnail)
        {
        
        }
    }
}
