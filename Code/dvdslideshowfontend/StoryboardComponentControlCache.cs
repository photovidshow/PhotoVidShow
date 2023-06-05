using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ManagedCore;

namespace dvdslideshowfontend
{
    public class StoryboardComponentControlCache
    {
        private static StoryboardComponentControlCache mInstance;
        private List<ThumbnailControl> mCachedThumbnailControls = new List<ThumbnailControl>();
        private List<TransitionComponentControl> mCachedTransitionControls = new List<TransitionComponentControl>();
        private List<SlideShowSeperatorControl> mCachedSeperatorControls = new List<SlideShowSeperatorControl>();
        private List<SlideshowTimeLabelComponentControl> mCacheTimeLabelControls = new List<SlideshowTimeLabelComponentControl>();

        // ************************************************************************************
        public static StoryboardComponentControlCache GetInstance()
        {
            if (mInstance == null)
            {
                mInstance = new StoryboardComponentControlCache();
            }
            return mInstance;
        }

        // ************************************************************************************
        private StoryboardComponentControlCache()
        {

        }

        // ************************************************************************************
        public void ReleaseControl(Control c)
        {
            ThumbnailControl tc = c as ThumbnailControl;
            if (tc != null)
            {
                tc.Blank();
                mCachedThumbnailControls.Add(tc);
                return;
            }

            SlideshowTimeLabelComponentControl stlcc = c as SlideshowTimeLabelComponentControl;
            if (stlcc != null)
            {
                stlcc.Blank();
                mCacheTimeLabelControls.Add(stlcc);
                return;
            }

            TransitionComponentControl tcc = c as TransitionComponentControl;
            if (tcc != null)
            {
                tcc.Blank();
                mCachedTransitionControls.Add(tcc);
                return;
            }

            SlideShowSeperatorControl sssc = c as SlideShowSeperatorControl;
            if (sssc != null)
            {
                sssc.Blank();
                mCachedSeperatorControls.Add(sssc);
                return;
            }

        }

        // ************************************************************************************
        public Control GetControlForStoryboardComponent(StoryboardComponent component)
        {
            Thumbnail tn= component as Thumbnail;
            if (tn!=null)
            {
                 return GetThumbnailControl(tn);
            }

            SlideshowTimeLabelComponent stlc = component as SlideshowTimeLabelComponent;
            if (stlc != null)
            {
                return GetTimeLabelControl(stlc);
            }

            TransitionComponent tc = component as TransitionComponent;
            if (tc != null)
            {
                return GetTransitionControl(tc);
            }

            SlideShowSeperator sss = component as SlideShowSeperator;
            if (sss != null)
            {
                return GetSeperatorControl(sss);
            }

            Log.Error("Unknown storyboard component in GetControlForStoryboardComponent");
            return null;
        }

        // ************************************************************************************
        private SlideShowSeperatorControl GetSeperatorControl(SlideShowSeperator sss)
        {
            SlideShowSeperatorControl sssc = null;
            if (mCachedSeperatorControls.Count > 0)
            {
                sssc = mCachedSeperatorControls[0];
                mCachedSeperatorControls.RemoveAt(0);
            }
            else
            {
                sssc = new SlideShowSeperatorControl();
            }

            SetPosition(sssc, sss);
            sss.SeperatorControl = sssc;
            sssc.BackColor = sss.BackColor;

            return sssc;
        }


        // ************************************************************************************
        private TransitionComponentControl GetTransitionControl(TransitionComponent tc)
        {
            TransitionComponentControl tcc = null;
            if (mCachedTransitionControls.Count > 0)
            {
                tcc = mCachedTransitionControls[0];
                mCachedTransitionControls.RemoveAt(0);
            }
            else
            {
                tcc = new TransitionComponentControl();
            }

            SetPosition(tcc, tc);
            tc.TransitionControl = tcc;

            return tcc;
        }

        // ************************************************************************************
        private SlideshowTimeLabelComponentControl GetTimeLabelControl(SlideshowTimeLabelComponent stlc)
        {
            SlideshowTimeLabelComponentControl stlcc = null;
            if (mCacheTimeLabelControls.Count > 0)
            {
                stlcc = mCacheTimeLabelControls[0];
                mCacheTimeLabelControls.RemoveAt(0);
            }
            else
            {
                stlcc = new SlideshowTimeLabelComponentControl();
            }

            SetPosition(stlcc, stlc);
            stlc.TimeLabelControl = stlcc;
            stlcc.SetTime(stlc.TimeString);

            return stlcc;
               
        }

        // ************************************************************************************
        private ThumbnailControl GetThumbnailControl(Thumbnail tn)
        {
            ThumbnailControl tc = null;
            if (mCachedThumbnailControls.Count > 0)
            {
                tc = mCachedThumbnailControls[0];
                mCachedThumbnailControls.RemoveAt(0);
            }
            else
            {
                tc =  new ThumbnailControl();
            }

            SetPosition(tc, tn);
            tn.ForThumbnailControl = tc;

            if (tn.ImageValid == false)
            {
                tn.InvalidateImage();
            }
        
            tc.SetImage(tn.Image);
            tc.SetHighlightColours(tn.Highlighted);
            tc.SlideLengthComboEnabled = tn.SlideLengthComboEnabled;
            tc.ReCalcComboText();
            tc.ReDrawSideIcons();

            return tc;
        }

        // ************************************************************************************
        private void SetPosition(UserControl control, StoryboardComponent sc)
        {
            control.Left = sc.Left;
            control.Top = sc.Top;
            control.Width = sc.Width;
            control.Height = sc.Height;
        }

        // ************************************************************************************
        // Method used to create intial controls, then add them to the storyboard panel; and then remove them.
        // This is called before adding first lots of controls as it prevents an initial flicker
        public void EnsureCreateInitialCachedControls(Panel p)
        {
            if (mCachedThumbnailControls.Count!=0) return;

            p.SuspendLayout();

            int numberInitialControlSet = 20;
            int setSize = 4;
            int offscreen = 4000;

            Control[] list = new Control[numberInitialControlSet * setSize];

            for (int i = 0; i < numberInitialControlSet; i++)
            {
                ThumbnailControl tc = new ThumbnailControl();
                tc.Left = offscreen;

                SlideshowTimeLabelComponentControl stlcc = new SlideshowTimeLabelComponentControl();
                stlcc.Left = offscreen;

                TransitionComponentControl tcc = new TransitionComponentControl();
                tcc.Left = offscreen;

                SlideShowSeperatorControl sssc = new SlideShowSeperatorControl();
                sssc.Left = offscreen;

                list[i * setSize] = tc;
                list[(i * setSize) + 1] = stlcc;
                list[(i * setSize) + 2] = tcc;
                list[(i * setSize) + 3] = sssc;
            }

            p.Controls.AddRange(list);

            for (int i = 0; i < numberInitialControlSet * setSize; i++)
            {
                p.Controls.Remove(list[i]);
                ReleaseControl(list[i]);
            }

            p.ResumeLayout();

        }
    }
}
