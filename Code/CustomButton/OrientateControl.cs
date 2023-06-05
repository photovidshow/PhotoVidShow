using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;
using System.Drawing.Imaging;


namespace CustomButton
{
    public partial class OrientateControl : UserControl
    {     
        public delegate void ImageChangedCallbackDelegate();
        public event ImageChangedCallbackDelegate ImageChanged;

        private List<CImageDecoration> mForDecorations; // same image

        //*************************************************************************************************
        public OrientateControl()
        {
            InitializeComponent();
        }

        //*************************************************************************************************
        public void SetForDecoration(List<CImageDecoration> decorations)
        {
            mForDecorations = decorations;
        }

        //*************************************************************************************************
        private void mRotateCW90Button_Click(object sender, EventArgs e)
        {
            if (mForDecorations != null)
            {
                foreach (CImageDecoration dec in mForDecorations)
                {
                    dec.RotateCW90();
                }
                UpdateCallback();
            }
        }

        //*************************************************************************************************
        private void mRotateCCW90Button_Click(object sender, EventArgs e)
        {
            if (mForDecorations != null)
            {
                foreach (CImageDecoration dec in mForDecorations)
                {
                    dec.RotateCCW90();
                }
                UpdateCallback();
            }
        }

        //*************************************************************************************************
        private void mFlipYButton_Click(object sender, EventArgs e)
        {
            if (mForDecorations != null)
            {
                foreach (CImageDecoration dec in mForDecorations)
                {
                    dec.FlipY();
                }
                UpdateCallback();
            }
        }

        //*************************************************************************************************
        private void mFlipXButton_Click(object sender, EventArgs e)
        {
            if (mForDecorations != null)
            {
                foreach (CImageDecoration dec in mForDecorations)
                {
                    dec.FlipX();
                }
                UpdateCallback();
            }
        }

        //*************************************************************************************************
        private void mResetOrientationButton_Click(object sender, EventArgs e)
        {
            if (mForDecorations != null)
            {
                foreach (CImageDecoration dec in mForDecorations)
                {
                    dec.ResetRotateAndFlip();
                }
                UpdateCallback();
            }
        }

        //*************************************************************************************************
        private void UpdateCallback()
        {
            if (mForDecorations != null && ImageChanged != null)
            {
                ImageChanged();
            }
        }
    }
}
