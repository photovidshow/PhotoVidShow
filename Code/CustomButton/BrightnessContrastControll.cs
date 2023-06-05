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
    public delegate void BrightnessContrastChangedCallback();
 
    public partial class BrightnessContrastControll : UserControl
    {

        public event BrightnessContrastChangedCallback BrightnessChanged;
        public event BrightnessContrastChangedCallback BrightnessRChanged;
        public event BrightnessContrastChangedCallback BrightnessGChanged;
        public event BrightnessContrastChangedCallback BrightnessBChanged;
        public event BrightnessContrastChangedCallback ContrastChanged;
        public event BrightnessContrastChangedCallback ContrastRChanged;
        public event BrightnessContrastChangedCallback ContrastGChanged;
        public event BrightnessContrastChangedCallback ContrastBChanged;
        public event BrightnessContrastChangedCallback FinishedBrightnessContrastChange;

        private int mContrastMultiple = 500;
        private float mContrastMax = 4.0f;

        public CheckBox ChangeIndividialChannelsCheckBox
        {
            get { return mColourAdjustCheckBox; }
        }

        public float BrightnessR
        {
            get { return (((float)mBrightnessRTrackBar.Value) / 100.0f); }
            set 
            {
                mBrightnessRTrackBar.Value = CGlobals.Clamp((int) (value * 100.0f), 0, 200);
                mBrightnessRnumericUpDown1.Value= (Decimal) CGlobals.ClampF(value - 1.0f, -1.0f, 1.0f);
            }
        }
        public float BrightnessG
        {
            get { return (((float)mBrightnessGTrackBar.Value) / 100.0f); }
            set
            {
                mBrightnessGTrackBar.Value = CGlobals.Clamp((int) (value * 100.0f), 0, 200);
                mBrightnessGnumericUpDown1.Value = (Decimal)CGlobals.ClampF(value - 1.0f, -1.0f, 1.0f);
            }
        }

        public float BrightnessB
        {
            get { return (((float)mBrightnessBTrackBar.Value) / 100.0f); }
            set
            {
                mBrightnessBTrackBar.Value = CGlobals.Clamp((int) (value * 100.0f), 0, 200);
                mBrightnessBnumericUpDown1.Value = (Decimal)CGlobals.ClampF(value - 1.0f, -1.0f, 1.0f);
            }
        }

        public float ContrastR
        {
            get { return (((float)this.mContrastRTrackBar.Value) / 100.0f); }
            set
            {
                mContrastRTrackBar.Value = CGlobals.Clamp((int)(value * 100.0f), 0, mContrastMultiple);
                mContrastRnumericUpDown1.Value = (Decimal)CGlobals.ClampF(value -1.0f, -1.0f, mContrastMax);
            }
        }

        public float ContrastG
        {
            get { return (((float)this.mContrastGTrackBar.Value) / 100.0f); }
            set
            {
                mContrastGTrackBar.Value = CGlobals.Clamp((int)(value * 100.0f), 0, mContrastMultiple);
                mContrastGnumericUpDown1.Value = (Decimal)CGlobals.ClampF(value - 1.0f, -1.0f, mContrastMax);
            }
        }

        public float ContrastB
        {
            get { return (((float)this.mContrastBTrackBar.Value) / 100.0f); }
            set
            {
                mContrastBTrackBar.Value = CGlobals.Clamp((int)(value * 100.0f), 0, mContrastMultiple);
                mContrastBnumericUpDown1.Value = (Decimal)CGlobals.ClampF(value - 1.0f, -1.0f, mContrastMax);
            }
        }


        //*************************************************************************************************
        public BrightnessContrastControll()
        {
            InitializeComponent();
        }

        //*************************************************************************************************
        private void mBrightnessR_Scroll(object sender, EventArgs e)
        {
            this.mBrightnessRnumericUpDown1.Value = (Decimal)CGlobals.ClampF(BrightnessR - 1.0f, -1.0f, 1.0f);

            if (BrightnessRChanged != null)
            {
                BrightnessRChanged();
            }

            if (mColourAdjustCheckBox.Checked == false)
            {
                BrightnessG = BrightnessR;
                BrightnessB = BrightnessR;
                BrightnessChanged();
            }
            else if (BrightnessRChanged!=null)
            {
                BrightnessRChanged();
            }
        }

        //*************************************************************************************************
        private void mBrightnessG_Scroll(object sender, EventArgs e)
        {
            this.mBrightnessGnumericUpDown1.Value = (Decimal)CGlobals.ClampF(BrightnessG - 1.0f, -1.0f, 1.0f);

            if (BrightnessGChanged != null)
            {
                BrightnessGChanged();
            }
        }

        //*************************************************************************************************
        private void mBrightnessB_Scroll(object sender, EventArgs e)
        {
            this.mBrightnessBnumericUpDown1.Value = (Decimal)CGlobals.ClampF(BrightnessB - 1.0f, -1.0f, 1.0f);

            if (BrightnessBChanged != null)
            {
                BrightnessBChanged();
            }
        }

        //*************************************************************************************************
        private void mBrightnessRnumericUpDown1_UpButtonSelected()
        {
            BrightnessR = (float) mBrightnessRnumericUpDown1.Value + 1;
            if (mColourAdjustCheckBox.Checked == false)
            {
                BrightnessG = BrightnessR;
                BrightnessB = BrightnessR;
                BrightnessChanged();
            }
            else if (BrightnessRChanged != null)
            {
                BrightnessRChanged();
            }

            if (FinishedBrightnessContrastChange != null)
            {
                FinishedBrightnessContrastChange();
            }
        }

        //*************************************************************************************************
        private void mBrightnessRnumericUpDown1_DownButtonSelected()
        {
            mBrightnessRnumericUpDown1_UpButtonSelected();
        }

        //*************************************************************************************************
        private void mBrightnessGnumericUpDown1_UpButtonSelected()
        {
            BrightnessG = (float)mBrightnessGnumericUpDown1.Value + 1;
            if (BrightnessGChanged != null)
            {
                BrightnessGChanged();
            }
            if (FinishedBrightnessContrastChange != null)
            {
                FinishedBrightnessContrastChange();
            }
        }

        //*************************************************************************************************
        private void mBrightnessGnumericUpDown1_DownButtonSelected()
        {
            mBrightnessGnumericUpDown1_UpButtonSelected();
        }

        //*************************************************************************************************
        private void mBrightnessBnumericUpDown1_UpButtonSelected()
        {
            BrightnessB = (float)mBrightnessBnumericUpDown1.Value + 1;
            if (BrightnessBChanged != null)
            {
                BrightnessBChanged();
            }
            if (FinishedBrightnessContrastChange != null)
            {
                FinishedBrightnessContrastChange();
            }
        }

        //*************************************************************************************************
        private void mBrightnessBnumericUpDown1_DownButtonSelected()
        {
            mBrightnessBnumericUpDown1_UpButtonSelected();

            if (FinishedBrightnessContrastChange != null)
            {
                FinishedBrightnessContrastChange();
            }
        }



        //*************************************************************************************************
        //*************************************************************************************************
        //*************************************************************************************************
        //        contrast

        //*************************************************************************************************
        private void mContrastRTrackBar_Scroll(object sender, EventArgs e)
        {
            mContrastRnumericUpDown1.Value = (Decimal)CGlobals.ClampF(ContrastR - 1.0f, -1.0f, mContrastMax);

            if (mColourAdjustCheckBox.Checked == false)
            {
                ContrastG = ContrastR;
                ContrastB = ContrastR;
                ContrastChanged();
            }
            else if (ContrastRChanged != null)
            {
                ContrastRChanged();
            }
        }
        //*************************************************************************************************
        private void mContrastRnumericUpDown1_UpButtonSelected()
        {
            ContrastR = (float)mContrastRnumericUpDown1.Value + 1;
            if (mColourAdjustCheckBox.Checked == false)
            {
                ContrastG = ContrastR;
                ContrastB = ContrastR;
                ContrastChanged();
            }
            else if (ContrastRChanged != null)
            {
                ContrastRChanged();
            }
            if (FinishedBrightnessContrastChange != null)
            {
                FinishedBrightnessContrastChange();
            }
        }
        //*************************************************************************************************
        private void mContrastRnumericUpDown1_DownButtonSelected()
        {
            mContrastRnumericUpDown1_UpButtonSelected();
        }


        //*************************************************************************************************
        private void mContrastGTrackBar_Scroll(object sender, EventArgs e)
        {
            mContrastGnumericUpDown1.Value = (Decimal)CGlobals.ClampF(ContrastG - 1.0f, -1.0f, mContrastMax);

            if (ContrastGChanged != null)
            {
                ContrastGChanged();
            }
        }
        //*************************************************************************************************
        private void mContrastGnumericUpDown1_UpButtonSelected()
        {
            ContrastG = (float)mContrastGnumericUpDown1.Value + 1;
            if (ContrastGChanged != null)
            {
                ContrastGChanged();
            }
            if (FinishedBrightnessContrastChange != null)
            {
                FinishedBrightnessContrastChange();
            }
        }
        //*************************************************************************************************
        private void mContrastGnumericUpDown1_DownButtonSelected()
        {
            mContrastGnumericUpDown1_UpButtonSelected();
        }


        //*************************************************************************************************
        private void mContrastBTrackBar_Scroll(object sender, EventArgs e)
        {
            mContrastBnumericUpDown1.Value = (Decimal)CGlobals.ClampF(ContrastB - 1.0f, -1.0f, mContrastMax);

            if (ContrastBChanged != null)
            {
                ContrastBChanged();
            }
        }

        //*************************************************************************************************
        private void mContrastBnumericUpDown1_UpButtonSelected()
        {
            ContrastB = (float)mContrastBnumericUpDown1.Value + 1;
            if (ContrastBChanged != null)
            {
                ContrastBChanged();
            }
            if (FinishedBrightnessContrastChange != null)
            {
                FinishedBrightnessContrastChange();
            }
        }

        //*************************************************************************************************
        private void mContrastBnumericUpDown1_DownButtonSelected()
        {
            mContrastBnumericUpDown1_UpButtonSelected();
        }

        //*************************************************************************************************
        private void mColourAdjustCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.SuspendLayout();
            try
            {
                if (mColourAdjustCheckBox.Checked == false)
                {         
                    mBrightnessRLabel.Visible = false;
                    mBrightnessGLabel.Visible = false;
                    mBrightnessBLabel.Visible = false;
                    mContrastRLabel.Visible = false;
                    mContrastGLabel.Visible = false;
                    mContrastBLabel.Visible = false;

                    mBrightnessBTrackBar.Visible = false;
                    mBrightnessGTrackBar.Visible = false;
                    mContrastGTrackBar.Visible = false;
                    mContrastBTrackBar.Visible = false;

                    mBrightnessGnumericUpDown1.Visible = false;
                    mBrightnessBnumericUpDown1.Visible = false;
                    mContrastGnumericUpDown1.Visible = false;
                    mContrastBnumericUpDown1.Visible = false;

                    if (BrightnessR != BrightnessG ||
                        BrightnessR != BrightnessB ||
                        ContrastR != ContrastG ||
                        ContrastR != ContrastB)
                    {
                        BrightnessR = 1;
                        BrightnessG = 1;
                        BrightnessB = 1;
                        ContrastR = 1;
                        ContrastG = 1;
                        ContrastB = 1;

                        if (BrightnessChanged !=null)
                        {
                            BrightnessChanged();
                        }

                        if (ContrastChanged !=null)
                        {
                            ContrastChanged();
                        }

                        if (FinishedBrightnessContrastChange != null)
                        {
                            FinishedBrightnessContrastChange();
                        }
                    }
                }
                else
                {    
                   foreach (Control c in Controls)
                   {
                        c.Visible = true;
                   }               
                }
            }
            finally
            {
                this.ResumeLayout();
            }
        }

        //*************************************************************************************************
        private void mBrightnessRTrackBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (FinishedBrightnessContrastChange != null)
            {
                FinishedBrightnessContrastChange();
            }
        }

        //*************************************************************************************************
        private void mBrightnessGTrackBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (FinishedBrightnessContrastChange != null)
            {
                FinishedBrightnessContrastChange();
            }
        }

        //*************************************************************************************************
        private void mBrightnessBTrackBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (FinishedBrightnessContrastChange != null)
            {
                FinishedBrightnessContrastChange();
            }
        }

        //*************************************************************************************************
        private void mContrastRTrackBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (FinishedBrightnessContrastChange != null)
            {
                FinishedBrightnessContrastChange();
            }
        }

        //*************************************************************************************************
        private void mContrastGTrackBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (FinishedBrightnessContrastChange != null)
            {
                FinishedBrightnessContrastChange();
            }
        }

        //*************************************************************************************************
        private void mContrastBTrackBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (FinishedBrightnessContrastChange != null)
            {
                FinishedBrightnessContrastChange();
            }
        }
    }
}
