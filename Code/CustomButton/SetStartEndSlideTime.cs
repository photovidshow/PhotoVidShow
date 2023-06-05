using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;

namespace CustomButton
{
    public partial class SetStartEndSlideTime : UserControl
    {
        private int mStartTime = 0;
        private int mEndTime = 0;
        private int mMaxTime = 0;
        private bool mUpdatingInternally = false;
        private bool mScrolledOccured = false;

        public delegate void TimesChangedCallbackDelegate();

        //*******************************************************************
        public event TimesChangedCallbackDelegate TimesChangedCallback;


        //*******************************************************************
        public float StartTime
        {
            get { return ((float)mStartTime) / 100.0f; }
        }

        //*******************************************************************
        public float EndTime
        {
            get { return ((float)mEndTime) / 100.0f; }
        }

        //*******************************************************************
        public float MaxTime
        {
            get { return ((float)mMaxTime) / 100.0f; }
        }

        //*******************************************************************
        public void SetTrackbarBacgroundColor(Color c)
        {
            mStartSlideTimeTrackBar.BackColor = c;
            mEndSlideTimeTrackBar.BackColor = c;
        }

        //*******************************************************************
        public SetStartEndSlideTime()
        {
            InitializeComponent();

            // Only send update when finished scrolling
            mStartSlideTimeTrackBar.MouseUp += new MouseEventHandler(SlideTimeTrackBar_MouseUp);
            mEndSlideTimeTrackBar.MouseUp += new MouseEventHandler(SlideTimeTrackBar_MouseUp);
        }

        //*******************************************************************
        void SlideTimeTrackBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (TimesChangedCallback != null && mScrolledOccured == true)
            {
                TimesChangedCallback();
            }
            mScrolledOccured = false;
        }

        //*******************************************************************
        public void Set(float startTime, float endTime, float maxTime)
        {
            if (endTime > maxTime) endTime = maxTime;
            if (startTime > endTime) startTime = endTime;

            int iStartTime = (int)((startTime * 100) + 0.4999f);
            int iEndTime = (int)((endTime * 100) + 0.4999f);
            int iMaxTime = (int)((maxTime * 100) + 0.4999f);

            SetMultiply100(iStartTime, iEndTime, iMaxTime);

        }
        //*******************************************************************
        private bool SetMultiply100(int startTime, int endTime, int maxTime)
        {
            mUpdatingInternally = true;
            try
            {

                if (maxTime < endTime)
                {
                    ManagedCore.Log.Error("Max time bigger than end time (max:"+ maxTime + ", end:"+ endTime+")");
                    maxTime = endTime;
                }

                if (mStartSlideTimeTrackBar.Maximum != maxTime ||
                     mStartSlideTimeTrackBar.Minimum != 0 ||
                     mStartSlideTimeTrackBar.Value != startTime ||
                     mEndSlideTimeTrackBar.Maximum != maxTime ||
                     mEndSlideTimeTrackBar.Minimum != 0 ||
                     mEndSlideTimeTrackBar.Value != endTime)
                {

                    mStartSlideTimeTrackBar.Maximum = maxTime;
                    mStartTimeValueNumerical.Maximum = (decimal)(((float)maxTime) / 100.0f);
                    mStartSlideTimeTrackBar.Minimum = 0;
                    mStartSlideTimeTrackBar.Value = startTime;

                    mEndSlideTimeTrackBar.Maximum = maxTime;
                    mEndTimeValueNumerical.Maximum = (decimal)(((float)maxTime) / 100.0f);
                    mEndSlideTimeTrackBar.Minimum = 0;
                    mEndSlideTimeTrackBar.Value = endTime;

                    // Set attributes after, as the above may call some events which overwrite them
                    mStartTime = startTime;
                    mEndTime = endTime;
                    mMaxTime = maxTime;

                    UpdateTimeLabels();
                    return true;
                }
                return false;
            }
            finally
            {
                mUpdatingInternally = false;
            }
        }

        //*******************************************************************
        private void mStartSlideTimeTrackBar_Scroll(object sender, EventArgs e)
        {
            if (mStartSlideTimeTrackBar.Value != mStartTime)
            {
                mStartTime = mStartSlideTimeTrackBar.Value;

                if (mStartTime > mEndTime)
                {
                    mStartTime = mEndTime;
                    mStartSlideTimeTrackBar.Value = mStartTime;
                }

                mScrolledOccured = true;

                UpdateTimeLabels();
            }
        }

        //*******************************************************************
        private void mEndSlideTimeTrackBar_Scroll(object sender, EventArgs e)
        {
            if (mEndSlideTimeTrackBar.Value != mEndTime)
            {
                mEndTime = mEndSlideTimeTrackBar.Value;

                if (mEndTime < mStartTime)
                {
                    mEndTime = mStartTime;
                    mEndSlideTimeTrackBar.Value = mStartTime;
                }

                mScrolledOccured = true;

                UpdateTimeLabels();
            }
        }

        //*******************************************************************
        private void UpdateTimeLabels()
        { 
            mUpdatingInternally = true;
            try
            {
                float startTime = ((float)mStartTime) / 100.0f;
                float endTime = ((float)mEndTime) / 100.0f;
                float maxTime = ((float)mMaxTime) / 100.0f;

                startTime = CGlobals.ClampF(startTime, 0, maxTime);
                endTime = CGlobals.ClampF(endTime, 0, maxTime);

                mStartTimeValueNumerical.Maximum = (decimal)maxTime;
                mStartTimeValueNumerical.Value = (decimal)startTime;

                mEndTimeValueNumerical.Maximum = (decimal)maxTime;
                mEndTimeValueNumerical.Value = (decimal)endTime;

                mMaxTimeLabel.Text = string.Format("({0:0.0}s)", maxTime);
            }
            finally
            {
                mUpdatingInternally = false;
            }
        }

        //*******************************************************************
        private void mEndTimeValueNumerical_ValueChanged(object sender, EventArgs e)
        {
            if (mUpdatingInternally == true)
            {
                return;
            }

            decimal val = decimal.Round(mEndTimeValueNumerical.Value, 1, MidpointRounding.AwayFromZero);

            int endtime = (int)((((float)val) * 100) + 0.4999f);

            if (SetMultiply100(mStartTime, endtime, mMaxTime) == true)
            {
                if (TimesChangedCallback != null)
                {
                    TimesChangedCallback();
                }
            }
           
        }

        //*******************************************************************
        private void mStartTimeValueNumerical_ValueChanged(object sender, EventArgs e)
        {
            if (mUpdatingInternally == true)
            {
                return;
            }

            decimal val = decimal.Round(mStartTimeValueNumerical.Value, 1, MidpointRounding.AwayFromZero);

            int starttime = (int)((((float)val) * 100) + 0.4999f);

            if (SetMultiply100(starttime, mEndTime, mMaxTime) == true)
            {
                if (TimesChangedCallback != null)
                {
                    TimesChangedCallback();
                }
            }
        }

    }
}
