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
    public partial class CreateSlideFromTemplateForm : Form
    {
        private CImageSlide mImageSlide;
        private CSlideShow mSlideshow;

        public CImageSlide ResultingSlide
        {
            get { return mImageSlide; }
        }

        //*************************************************************************************************    
        public CreateSlideFromTemplateForm()
        {
            InitializeComponent();

            try
            {
                CGlobals.mCurrentProject.IgnoreProjectChanges = true;

                // Check to see if 4:3 project
                if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV4_3)
                {
                    int oldwidth = mPreviewPanel.Width;

                    mPreviewPanel.Width = (int)(((float)mPreviewPanel.Height) * 1.33333f);

                    this.ClientSize = new Size(this.ClientSize.Width - (oldwidth - mPreviewPanel.Width), this.ClientSize.Height);
                }

                mPredefinedSlideDesignsControl.GetApplyDesignButton().Visible = false;
                mPredefinedSlideDesignsControl.GetEditSlideMediaButton().Visible = false;
                mPredefinedSlideDesignsControl.GetUseNextSlidePicturesToPopulateDesignCheckBox().Visible = false;

                mSlideshow = new CSlideShow("");
                mSlideshow.FadeIn = false;
                mSlideshow.FadeIn = false;

                mImageSlide = new CBlankStillPictureSlide(Color.Black);

                ArrayList list = new ArrayList();
                list.Add(mImageSlide);

                mSlideshow.AddSlides(list);

                mPredefinedSlideDesignsControl.SetSlide(mImageSlide, mSlideshow, null, mPreviewPanel);
                mPredefinedSlideDesignsControl.SelectedPreviewDesignChanged += new PredefinedSlideDesignsControl.SlideDesignChangedCallbackDelegate(SlideDesignChangedCallback);
            }
            finally
            {
                CGlobals.mCurrentProject.IgnoreProjectChanges = false;
            }
        }

        //*************************************************************************************************   
        private void SlideDesignChangedCallback(CImageSlide forSlide)
        {
            if (forSlide != null)
            {
                mCreateSlideButton.Enabled = true;
            }
            else
            {
                mCreateSlideButton.Enabled = false;
            }
        }

        //*************************************************************************************************    
        private void mCreateSlideButton_Click(object sender, EventArgs e)
        {
            mPredefinedSlideDesignsControl.mApplyDesignButton_Click(this, e);

            mImageSlide = mPredefinedSlideDesignsControl.CurrentEditingSlide as CImageSlide;

            DialogResult = DialogResult.OK;


            this.Close();
        }


        //*************************************************************************************************   
        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //*************************************************************************************************  
        private void CreateSlideFromTemplateForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MiniPreviewController.StopAnyPlayingController();
        }
    }
}
