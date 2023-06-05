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
    public partial class RecordNarrationForm : Form
    {
        private static string mStoreLocation = "";

        private CAudioRecorder mRecorder;
        private string mTempWavFile;
        private string mWmaFile;

        public delegate void StartSlideShowDelegate(bool muteSound);
        public delegate void StopSlideShowDelegate();
         public delegate void PauseSlideShowDelegate();

        private bool mDoneRecording = false;
        private bool mRecording = false;
        private bool mPaused = true;

        private StartSlideShowDelegate mStartSlideshowCallback;
        private StopSlideShowDelegate mStopSlideshowCallback;
        private PauseSlideShowDelegate mPauseSlideshowCallback;

  
        public float RecordStartTime
        {
            set 
            { 
                StringBuilder sb = new StringBuilder();
                sb.Append("Record narration from time: ");
                sb.Append(CGlobals.TimeStringFromSeconds(value));
                mRecordNarrationFromTimeLabel.Text = sb.ToString();         
            }
        }

        public string OutputWMAFilename
        {
            get
            {
                if (mDoneRecording == true) return mWmaFile;
                return "";
            }
        }

        //*******************************************************************
        public RecordNarrationForm(StartSlideShowDelegate start, StopSlideShowDelegate stop, PauseSlideShowDelegate pause)
        {
            mStartSlideshowCallback = start;
            mStopSlideshowCallback = stop;
            mPauseSlideshowCallback = pause;

            mTempWavFile = CGlobals.GetTempDirectory() + "\\temprec.wav";

            ManagedCore.IO.ForceDeleteIfExists(mTempWavFile, true);

            if (mStoreLocation == "")
            {
                mStoreLocation = ManagedCore.DefaultFolders.GetFolder("MyMusic"); 
            }

            InitializeComponent();

            mStoreLocationTextBox.Text = mStoreLocation;

            string stringfilename = "Narration";
            int index = 1;
            bool exists = true;
            while (exists == true)
            {
                string filename = mStoreLocation + "\\" + stringfilename + index.ToString() + ".wma";

                if (System.IO.File.Exists(filename) == true)
                {
                    index++;
                }
                else
                {
                    exists = false;
                }
            }

            mStoreName.Text = stringfilename + index.ToString();

            mRecorder = new CAudioRecorder();

            List<String> names = new List<string>();

            string name = mRecorder.GetFirstDeviceName();

            while (name != "")
            {
                names.Add(name.Clone() as string);
                name = mRecorder.GetNextDeviceName();
            }

            if (names.Count > 0)
            {
                foreach (string device in names)
                {
                    mDevicesCombo.Items.Add(device);
                }

                mDevicesCombo.Text = names[0];
            }
            else
            {       
                mRecordButton.Enabled = false;
            }
        }

        //*******************************************************************
        public int GetDevicesCount()
        {
            return mDevicesCombo.Items.Count;

        }

        //*******************************************************************
        public void SlideshowFinishedCallback()
        {
            // slideshow finished callback whilst recording
            if (mStopButton.Enabled == true)
            {
                this.mStopButton_Click(this, new EventArgs());
            }
        }

        //*******************************************************************
        private void Record_Click(object sender, EventArgs e)
        {
            if (mPaused == true && mRecording == true)
            {
                mRecorder.Continue();
                mStartSlideshowCallback(true);
                mPaused = false;
                mRecordButton.Text = "Pause";
                mRecordButton.Image = mImageList1.Images[0];
                mToolStrip.Refresh();
                mRecordingLabel.Visible = true;
                mRedDot.Visible = true;
                mTimer1.Start();
                return;
            }

            if (mPaused == false)
            {
                mRecorder.Pause();
                mPauseSlideshowCallback();
                mRecordButton.Text = "Continue recording";
                mRecordButton.Image = mImageList1.Images[1];
                mPaused = true;
                mRecordingLabel.Visible = false;
                mTimer1.Stop();
                mRedDot.Visible = false;
                mToolStrip.Refresh();
                return;
            }

            string name = mStoreName.Text;
            int index = mStoreName.Text.IndexOf('.');
            if (index != -1)
            {
                name = mStoreName.Text.Substring(0, index);
            }

            mWmaFile = mStoreLocationTextBox.Text + "\\" +name + ".wma";
            if (System.IO.File.Exists(mWmaFile) == true)
            {
                DialogResult result = MessageBox.Show("Output file already exists, do you want to overwrite it?", "File already exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    return;
                }
                ManagedCore.IO.ForceDeleteIfExists(mWmaFile, true);
            }

            mRecordButton.Enabled = true;
            mRecordButton.Text = "Pause";
            mRecordButton.Image = mImageList1.Images[0];
            mToolStrip.Refresh();

            mBrowseButton.Enabled = false;
            mStoreName.Enabled = false;
            mDoneButton.Text = "Cancel";
            mEchoRecordingCheckBox.Enabled = false;
            mDevicesCombo.Enabled = false;
            
            mStopButton.Enabled = true;
            mStartSlideshowCallback(true);
            mRecording=true;
            mPaused = false;
            mRecordingLabel.Visible = true;
            mRedDot.Visible = true;
            mTimer1.Start();

            mRecorder.Record(mTempWavFile, mDevicesCombo.SelectedIndex, mEchoRecordingCheckBox.Checked);          
        }

        //*******************************************************************
        private void mStopButton_Click(object sender, EventArgs e)
        {
            if (mRecorder != null)
            {
                StopIfRecording();

                CWmaEncoder encoder = new CWmaEncoder();
                encoder.Encode(mTempWavFile, mWmaFile);

                ManagedCore.IO.ForceDeleteIfExists(mTempWavFile, true);

                mDoneRecording = true;

                this.Close();
            }
        }

        //*******************************************************************
        private void StopIfRecording()
        {
            if (mRecorder != null && mRecording == true)
            {
                mRecorder.Stop();

                mStopSlideshowCallback();
                mRecording = false;
            }
       
        }

        //*******************************************************************
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //*******************************************************************
        private void RecordNarrationForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            StopIfRecording();

            ManagedCore.IO.ForceDeleteIfExists(mTempWavFile, false);

            mRecorder.Dispose();
        }

        //*******************************************************************
        private void mBrowseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select folder to store narrations";
        
            DialogResult result = fbd.ShowDialog();

            if (result != DialogResult.Cancel)
            {
                mStoreLocation = fbd.SelectedPath;
                mStoreLocationTextBox.Text = mStoreLocation;
            }

            fbd.Dispose();
        }

        //*******************************************************************
        private void DoneButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //*******************************************************************
        private void RecordNarrationForm_Load(object sender, EventArgs e)
        {

        }

        //*******************************************************************
        private void mTimer1_Tick(object sender, EventArgs e)
        {
            mRedDot.Visible = !mRedDot.Visible;
        }
    }
}
