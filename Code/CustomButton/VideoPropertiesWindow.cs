using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DVDSlideshow;

namespace CustomButton
{
    public partial class VideoPropertiesForm : Form
    {
        private CVideoDecoration mForVideoDecoration;

        public VideoPropertiesForm(CVideoDecoration decoration)
        {
            mForVideoDecoration = decoration;
            CVideoPlayer player = mForVideoDecoration.Player;

            InitializeComponent();
            Text = player.mFileName;
            mFrameHeightLabel.Text = player.GetHeight().ToString();
            mFrameWidthLabel.Text = player.GetWidth().ToString();
            double length = player.GetDurationInSeconds();
            string lengthString = CGlobals.TimeStringFromSeconds(length, true);
            mVideoLengthLabel.Text = lengthString;

            double rate = 1.0 / player.GetFrameRate();

            string rateString = System.String.Format("{0:0.00}", rate); 
            if (rateString.EndsWith("00") == true)
            {
                rateString = ((int)rate).ToString();
            }
            mFrameRateLabel.Text = rateString + " frames / second";

            if (player.ContainsAudio() == true)
            {
                mAudioLabel.Text = "Yes";
            }
            else
            {
                mAudioLabel.Text = "No";
            }
        }

        private void mShowRenderGraphCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mShowRenderGraphCheckBox.Checked == true)
            {

                CVideoPlayer vp = mForVideoDecoration.Player;

                string next = vp.GetFirstFilterDetails();

                StringBuilder sb = new StringBuilder();

                while (next !="")
                {
                    sb.AppendLine(next);
                    next = vp.GetNextFilterDetails();
                }

                mGraphTextBox.Text = sb.ToString();

            }
            else
            {
                mGraphTextBox.Text = "";
            }
        }
    }
}
