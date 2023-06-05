using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;
using System.IO;

namespace CustomButton
{
    public partial class AdjustImageColoursControl : UserControl
    {
        public delegate void ImageChangedCallbackDelegate();

        public event ImageChangedCallbackDelegate ImageChanged; 

        private List<CClipArtDecoration> mForDecorations;

        //*************************************************************************************************
        public AdjustImageColoursControl()
        {
            InitializeComponent();
            DisableAll();
        }

        //*************************************************************************************************
        public void SetForDecoration(List<CImageDecoration> decorations)
        {
            if (decorations == null)
            {
                mForDecorations = null;

                mResetButton_Click(this, new EventArgs());
                DisableAll();
                return;
            }

            mForDecorations = new List<CClipArtDecoration>();

            foreach (CImageDecoration dec in decorations)
            {
                CClipArtDecoration cad = dec as CClipArtDecoration;
                if (cad != null)
                {
                    mForDecorations.Add(cad);
                }
            }

            EnableAll();

            float [] brightness = mForDecorations[0].mImage.Brightness;

            mBrightnessContrastControll1.BrightnessR = brightness[0];
            mBrightnessContrastControll1.BrightnessG = brightness[1];
            mBrightnessContrastControll1.BrightnessB = brightness[2];

            float[] contrast = mForDecorations[0].mImage.Contrast;

            mBrightnessContrastControll1.ContrastR = contrast[0];
            mBrightnessContrastControll1.ContrastG = contrast[1];
            mBrightnessContrastControll1.ContrastB = contrast[2];


            if (brightness[0] != brightness[1] ||
                brightness[0] != brightness[2] ||
                contrast[0] != contrast[1] ||
                contrast[0] != contrast[2])
            {
                mBrightnessContrastControll1.ChangeIndividialChannelsCheckBox.Checked = true;
            }
            else
            {
                mBrightnessContrastControll1.ChangeIndividialChannelsCheckBox.Checked = false;
            }

            mBlackAndWhiteButton.Checked = mForDecorations[0].BlackAndWhite;
        }


        //*************************************************************************************************
        private void DisableAll()
        {
            foreach (Control c in this.Controls)
            {
                c.Enabled = false;
            }
        }

        //*************************************************************************************************
        private void EnableAll()
        {
            foreach (Control c in this.Controls)
            {
                c.Enabled = true;
            }
        }

        //*************************************************************************************************
        private void SetBrightnessOnDecor()
        {
            if (mForDecorations == null) return;

            float[] brighness = new float[3];
            brighness[0] = mBrightnessContrastControll1.BrightnessR;
            brighness[1] = mBrightnessContrastControll1.BrightnessG;
            brighness[2] = mBrightnessContrastControll1.BrightnessB;

            foreach (CClipArtDecoration dec in mForDecorations)
            {
                dec.Brightness = brighness;
            }

            ImageChanged();

        }
    
        //*************************************************************************************************
        private void mBrightnessContrastControll1_BrightnessChanged()
        {
            SetBrightnessOnDecor();
        }
        //*************************************************************************************************
        private void mBrightnessContrastControll1_BrightnessRChanged()
        {
            SetBrightnessOnDecor();
        }
        //*************************************************************************************************
        private void mBrightnessContrastControll1_BrightnessGChanged()
        {
            SetBrightnessOnDecor();
        }
        //*************************************************************************************************
        private void mBrightnessContrastControll1_BrightnessBChanged()
        {
            SetBrightnessOnDecor();
        }


        //*************************************************************************************************
        private void SetContrastOnDecor()
        {
            if (mForDecorations == null) return;

            float[] contrast = new float[3];
            contrast[0] = mBrightnessContrastControll1.ContrastR;
            contrast[1] = mBrightnessContrastControll1.ContrastG;
            contrast[2] = mBrightnessContrastControll1.ContrastB;

            foreach (CClipArtDecoration decs in mForDecorations)
            {
                decs.Contrast = contrast;
            }
            ImageChanged();
        }

        //*************************************************************************************************
        private void mBrightnessContrastControll1_ContrastBChanged()
        {
            SetContrastOnDecor();
        }

        //*************************************************************************************************
        private void mBrightnessContrastControll1_ContrastChanged()
        {
            SetContrastOnDecor();
        }

        //*************************************************************************************************
        private void mBrightnessContrastControll1_ContrastGChanged()
        {
            SetContrastOnDecor();
        }

        //*************************************************************************************************
        private void mBrightnessContrastControll1_ContrastRChanged()
        {
            SetContrastOnDecor();
        }

        //*************************************************************************************************
        public void mResetButton_Click(object sender, EventArgs e)
        {
            mBrightnessContrastControll1.BrightnessR = 1;
            mBrightnessContrastControll1.BrightnessG = 1;
            mBrightnessContrastControll1.BrightnessB = 1;
            mBrightnessContrastControll1.ContrastR = 1;
            mBrightnessContrastControll1.ContrastG = 1;
            mBrightnessContrastControll1.ContrastB = 1;

            SetBrightnessOnDecor();
            SetContrastOnDecor();
            mBrightnessContrastControll1.ChangeIndividialChannelsCheckBox.Checked = false;
            mBlackAndWhiteButton.Checked = false;
        }

        //*************************************************************************************************
        private void mBlackAndWhiteButton_CheckedChanged(object sender, EventArgs e)
        {
            if (mForDecorations == null) return;

            if (mForDecorations[0].BlackAndWhite != mBlackAndWhiteButton.Checked)
            {
                foreach (CClipArtDecoration dec in mForDecorations)
                {
                    dec.BlackAndWhite = mBlackAndWhiteButton.Checked;
                }
                ImageChanged();
            }
        }
    }
}
