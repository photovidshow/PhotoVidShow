using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;

namespace CustomButton
{
    public partial class StartupOutputChoiceForm : Form
    {
        public enum StartupOptions
        {
            None,
            videoFile,
            DVDVideoDisc,
            blurayDisc
        };

        private StartupOptions mSelected = StartupOptions.None;
        private float mDefualtSlideTime = 8.0f;
        private bool mEnableAutoPanZoom = true;

        public StartupOptions Selected
        {
            get
            {
                return mSelected;
            }
        }

        public float DefaultSlideTime
        {
            get { return mDefualtSlideTime; }
        }

        public bool EnableAutoPanZoom
        {
            get { return mEnableAutoPanZoom;  }
        }

        public bool ShowOptionAtStartup
        {
            get
            {
                return mShowAtStartupCheckBox.Checked;
            }
        }

        // ****************************************************************************************
        public StartupOutputChoiceForm(bool initialStartup)
        {
            InitializeComponent();

            if (initialStartup==false)
            {
                mShowAtStartupCheckBox.Visible = false;
            }
            CTextStyle titlestyle = CTextStyleDatabase.GetInstance().GetStyleFromName("Vanilla").Clone();
            titlestyle.FontSize = 15;

            GDITextDrawer textdrawer = new GDITextDrawer();

            mTopPictureBox.Image = new Bitmap(mTopPictureBox.Width, mTopPictureBox.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics gp = Graphics.FromImage(mTopPictureBox.Image))
            {
                gp.Clear(Color.Transparent);
                textdrawer.DrawString(gp, "Create new slideshow", 5, 7, titlestyle);
            }


            mDefaultSlideTimeCombo.Items.Add("0.1");
            mDefaultSlideTimeCombo.Items.Add("0.25");
            mDefaultSlideTimeCombo.Items.Add("0.5");
            mDefaultSlideTimeCombo.Items.Add("0.75");
            for (int i = 1; i <= 60; i++)
            {
                mDefaultSlideTimeCombo.Items.Add(i);
            }
        }


        // ****************************************************************************************
        private void RetrieveProjectSettings()
        {
            try
            {
                float time = float.Parse(mDefaultSlideTimeCombo.Text);
                mDefualtSlideTime = time;    
            }
            catch
            {
            }

            mEnableAutoPanZoom = mCreateProjectWithAutoPanAndZoomTickBox.Checked;
        }


        // ****************************************************************************************
        private void mDVDButton_Click(object sender, EventArgs e)
        {
            RetrieveProjectSettings();
            mSelected = StartupOptions.DVDVideoDisc;
            this.Close();
        }

        // ****************************************************************************************
        private void mVideoFileOption_Click(object sender, EventArgs e)
        {
            RetrieveProjectSettings();
            mSelected = StartupOptions.videoFile;
            this.Close();
        }

        // ****************************************************************************************
        private void mBlurayButton_Click(object sender, EventArgs e)
        {
            RetrieveProjectSettings();
            mSelected = StartupOptions.blurayDisc;
            this.Close();
        }

        // ****************************************************************************************
        private void mCancelButton_Click(object sender, EventArgs e)
        {
            RetrieveProjectSettings();
            this.Close();
        }

       
    }
}
