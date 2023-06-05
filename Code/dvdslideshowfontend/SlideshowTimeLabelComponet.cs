using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;
using DVDSlideshow;
using ManagedCore;

namespace dvdslideshowfontend
{
	/// <summary>
    /// Summary description for SlideshowTimeLabelComponent.
	/// </summary>
	/// 

    // represents the SlideshowTimeLabelComponent icon in the scrollable slidehow view
    public class SlideshowTimeLabelComponent : StoryboardComponent
	{
		private string mTimeString;
		public CImageSlide mSlide;
        private SlideshowTimeLabelComponentControl mTimeLabelControl = null;

        public string TimeString
        {
            get { return mTimeString; }
        }

        public SlideshowTimeLabelComponentControl TimeLabelControl
        {
            get { return mTimeLabelControl; }
            set 
            {
                mTimeLabelControl = value;

                if (mTimeLabelControl != null)
                {
                    mTimeLabelControl.ParentTimeLabel = this;
                }  
            }
        }

		//*******************************************************************
		public void RebuildCurrentTransPictureBox()
		{
			try
			{
                double f_time=mSlide.StartTime + mSlide.DisplayLength -
                   (mSlide.TransistionEffect.Length / 2) + 0.4999f;

				DateTime tt = new DateTime((long)(f_time*10000000.0f));

				string new_label_tt = System.String.Format("{0:d2}:{1:d2}:{2:d2}", tt.Hour,tt.Minute,tt.Second);

                mTimeString = new_label_tt;

                if (mTimeLabelControl != null)
                {
                    mTimeLabelControl.SetTime(mTimeString);
                }
			}
			catch (Exception exception)
			{
                Log.Error("An exception occurred when RebuildCurrentTransPictureBox. " + exception.Message);
			}
		}

		//*******************************************************************
		public void Blank()
		{
			mSlide=null;
            if (mTimeLabelControl != null)
            {
                mTimeLabelControl.ParentTimeLabel = null;
            }

            mTimeLabelControl = null;
		}

		//*******************************************************************
		public void ResetToSlide(CImageSlide for_slide)
		{
			mSlide = for_slide;
			RebuildCurrentTransPictureBox();
		}

		//*******************************************************************
		public SlideshowTimeLabelComponent(CImageSlide for_slide)
		{
			mSlide = for_slide;
            mWidth = 44;
            mHeight = 16;
			RebuildCurrentTransPictureBox();		
		}

	}
}
