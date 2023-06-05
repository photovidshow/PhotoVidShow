using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;
using System.Collections;

namespace CustomButton
{
    public partial class CreateBlankSlideForm : Form
    {
        private CImageSlide mImageSlide;
        private CSlideShow mSlideshow;
        private MiniPreviewController mPreviewController = null;
        private bool mUseImage = false;

        //*************************************************************************************************
        public CImageSlide ResultingSlide
        {
            get
            {
                if (mUseImage==true)
                {
                    return mImageSlide;
                }
                return null;
            }
        }

        //*************************************************************************************************
        public CreateBlankSlideForm()
        {
            InitializeComponent();

            // Clear background on backgrounds control (to match colour scheme on our form)
            mSelectBackgroundControl.BackgroundImage = null;

            // Adjust preview window if 4:3 project
            if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV4_3)
            {
                int oldwidth = this.mPreviewPanel.Width;

                mPreviewPanel.Width = (int)(((float)mPreviewPanel.Height) * 1.33333f);

            }
            mSlideshow = new CSlideShow("");
            mSlideshow.FadeIn = false;
            mSlideshow.FadeIn = false;

            mImageSlide = new CBlankStillPictureSlide(Color.Black);

            ArrayList list = new ArrayList();
            list.Add(mImageSlide);

            mSlideshow.AddSlides(list, true, null, null, false);

            mSelectBackgroundControl.SetSlide(mImageSlide, mSlideshow);
            mSelectBackgroundControl.GetApplySettingToAllSlidesButton().Visible = false;
            mSelectBackgroundControl.GetClearButton().Visible = false;

            mPreviewController = new MiniPreviewController(mSlideshow, mImageSlide, mPreviewPanel, false);

            this.Paint += new PaintEventHandler(CreateBlankSlideForm_Paint);

            mSelectBackgroundControl.ChangedBackground += new SelectBackgroundControl.ChangedBackgroundCallback(mSelectBackgroundControl_ChangedBackground);
        }

        //************************************************************************************************* 
        void mSelectBackgroundControl_ChangedBackground()
        {
            this.Invalidate();
        }

        //*************************************************************************************************    
        void CreateBlankSlideForm_Paint(object sender, PaintEventArgs e)
        {
            mPreviewController.DoSnapshot(30, false);
        }

        //*************************************************************************************************    
        private void mCreateSlideButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            mUseImage = true;
            this.Close();
        }

        //*************************************************************************************************   
        private void mCancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
