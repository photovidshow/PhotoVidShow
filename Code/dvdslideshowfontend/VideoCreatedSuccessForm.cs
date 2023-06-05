using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace dvdslideshowfontend
{
    public partial class VideoCreatedSuccessForm : Form
    {
        public string mFileToPlay = "";

        // ************************************************************************************
        public VideoCreatedSuccessForm(string filename)
        {
            mFileToPlay = filename;

            InitializeComponent();
            this.mFilename.Text = filename;
        }

        // ************************************************************************************
        private void OpenPlayerInOwnThread()
        {
            try
            {
                System.Diagnostics.Process.Start(mFileToPlay);
            }
            catch
            {
            }
        }


        // ************************************************************************************
        private void mPlayButton_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ThreadStart(OpenPlayerInOwnThread));
            t.Start();
            this.Close();
        }

        // ************************************************************************************
        private void mOpenFileLocationButton_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(this.mFilename.Text));
            }
            catch
            {
            }

            this.Close();
        }

        // ************************************************************************************
        private void mCloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
