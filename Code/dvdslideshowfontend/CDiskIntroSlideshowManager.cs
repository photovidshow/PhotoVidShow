using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;

namespace dvdslideshowfontend
{
    // **************************************************************************************************
    // This class manages the disc intro slideshow tab page
    // **************************************************************************************************
    public class CDiskIntroSlideshowManager
    {
        private Form1 mMainForm;
        private bool mUpdating = false;
        private CheckBox mIncludeIntroSlideshowCheckBox;
        private Button mImportSlideshowButton;

        // **************************************************************************************************
        public CDiskIntroSlideshowManager(Form1 mainForm)
        {
            mMainForm = mainForm;

            mIncludeIntroSlideshowCheckBox = mMainForm.GetIncludeIntroVideoCheckBox();
            mImportSlideshowButton = mMainForm.GetDiskIntroSlideshowImportSlideshowButton();

            mIncludeIntroSlideshowCheckBox.CheckedChanged += new EventHandler(mIncludeIntroSlideshowCheckBox_CheckedChanged);
            mImportSlideshowButton.Click += new EventHandler(mImportSlideshowButton_Click);

        }

        // **************************************************************************************************
        public void RebuildLabels()
        {
            mUpdating = true;
            try
            {
                CSlideShow ss = CGlobals.mCurrentProject.PreMenuSlideshow;
                if (ss != null)
                {
                    mIncludeIntroSlideshowCheckBox.Checked = true;
                    mImportSlideshowButton.Enabled = true;
                    SelectPreMenuSlideshow();
                }
                else
                {
                    mIncludeIntroSlideshowCheckBox.Checked = false;
                    mImportSlideshowButton.Enabled = false;
                    mMainForm.GetDecorationManager().RePaint();
                }
            }
            finally
            {
                mUpdating = false;
            }
        }

        // **************************************************************************************************
        private void mImportSlideshowButton_Click(object sender, EventArgs e)
        {
            mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();

            if (ShowWarningIfCurrentIntroSlideshowExists() == true)
            {
                ImportSlideshowsForm.ImportSlideshows(true);
                CGlobals.mCurrentProject.DeclareChange("Imported intro slideshow");
            }
        }

        // **************************************************************************************************
        private void mIncludeIntroSlideshowCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mUpdating == true)
            {
                return;
            }

            mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();

            if (mIncludeIntroSlideshowCheckBox.Checked == false && ShowWarningIfCurrentIntroSlideshowExists() == false)
            {
                mUpdating = true;
                mIncludeIntroSlideshowCheckBox.Checked = true;
                mUpdating = false;
                return;
            }

            if (mIncludeIntroSlideshowCheckBox.Checked == true)
            {
                CSlideShow ss = new CSlideShow(CProject.PreMenuIdentityString);
                CGlobals.mCurrentProject.PreMenuSlideshow = ss;
                mImportSlideshowButton.Enabled = true;
                SelectPreMenuSlideshow();
                CGlobals.mCurrentProject.DeclareChange("Added intro slideshow");
            }
            else
            {
                CGlobals.mCurrentProject.PreMenuSlideshow = null;
                mImportSlideshowButton.Enabled = false;
                mMainForm.GetDecorationManager().SelectFirstSlideshowInCurrentMenu();
                mMainForm.GoToMainMenu();
                CGlobals.mCurrentProject.DeclareChange("Removed intro slideshow");
            }     
        }

        // **************************************************************************************************
        private void SelectPreMenuSlideshow()
        {
            CSlideShow ss = CGlobals.mCurrentProject.PreMenuSlideshow;
            if (ss != null)
            {
                mMainForm.SelectedSlideShow(ss.Name);
                mMainForm.GetDecorationManager().RePaint();
            }
        }

        // **************************************************************************************************
        private bool ShowWarningIfCurrentIntroSlideshowExists()
        {
            if (CGlobals.mCurrentProject.PreMenuSlideshow == null)
            {
                return true;
            }

            if (CGlobals.mCurrentProject.PreMenuSlideshow.mSlides.Count <= 0)
            {
                return true;
            }

            string message = "This will permanently remove the current intro slideshow.\r\n\r\nAre you sure you wish to continue?";

            DialogResult result = ManagedCore.UserMessage.Show(message, "Remove current intro slideshow?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            return result == DialogResult.Yes;
        }
    }
}
