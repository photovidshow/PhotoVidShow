using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ManagedCore;
using DVDSlideshow;

namespace dvdslideshowfontend
{
    public partial class CMaxVideoPlayersCountWindow : Form
    {
        private string maxVideoSettingsKey = "MaximumVideoPlayersCached";
        private string ReduceHDResolutionVideosWhenEditing = "ReduceHDResolutionVideosWhenEditing";
        private bool mInitialising = false;
        public CMaxVideoPlayersCountWindow()
        {
            mInitialising = true;

            InitializeComponent();
            try
            {
                numericUpDown1.Value = UserSettings.GetIntValue("VideoSettings", maxVideoSettingsKey);
                mReduceHDResolutionCheckBox.Checked = UserSettings.GetBoolValue("VideoSettings", ReduceHDResolutionVideosWhenEditing);
            }
            catch
            {
            }
            mInitialising = false;
        }

        private void mOkButton_Click(object sender, EventArgs e)
        {
            int value = (int) numericUpDown1.Value;

            if (value != UserSettings.GetIntValue("VideoSettings", maxVideoSettingsKey))
            {
                UserSettings.SetIntValue("VideoSettings", maxVideoSettingsKey, value);
            }
            this.Close();
        }

        private void mResetButton_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = 4;

            if (mReduceHDResolutionCheckBox.Checked == false)
            {
                mReduceHDResolutionCheckBox.Checked = true;
            }
        }

        private void mCancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mReduceHDResolutionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mInitialising== true)
            {
                return;
            }
            bool currentSettings = UserSettings.GetBoolValue("VideoSettings", ReduceHDResolutionVideosWhenEditing);
            if (currentSettings != mReduceHDResolutionCheckBox.Checked)
            {
                UserSettings.SetBoolValue("VideoSettings", ReduceHDResolutionVideosWhenEditing, mReduceHDResolutionCheckBox.Checked);

                //
                // We need to clear out the video player cache as some cached video frames may be now at the wrong resolution
                //
                CVideoPlayerCache.GetInstance().CleanCache();
            }
        }
    }
}
