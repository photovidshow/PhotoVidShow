using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManagedCore;

namespace dvdslideshowfontend
{
    // ************************************************************************************************
    public partial class CSetDefaultFolderLocationsForm : Form
    {
        private string mDefaultFoldersCategory = "DefaultFoldersSettings";
        private string MyPictures = "MyPictures";
        private string MyMusic = "MyMusic";
        private string MyProjects = "MyProjects";
        private string AuthoredFolder = "AuthoredFolder";

        // ************************************************************************************************
        public CSetDefaultFolderLocationsForm()
        {
            InitializeComponent();

            SetFolderStrings();
        }

        // ************************************************************************************************
        private void SetFolderStrings()
        {
            mPhotoAndVideoTextBox.Text = DefaultFolders.GetFolder(MyPictures);
            mMusicAndAudioTextBox.Text = DefaultFolders.GetFolder(MyMusic);
            mProjectTextBox.Text = DefaultFolders.GetFolder(MyProjects);
            mAuthoredRootTextBox.Text = DefaultFolders.GetFolder(AuthoredFolder);
        }

        // ************************************************************************************************
        private void mDoneButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ************************************************************************************************
        private void ProcessSettingDefaultFolder(string name, string description, TextBox textBox, bool requireWriteAccess)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            StringBuilder sb = new StringBuilder();
            sb.Append("Set the default ");
            sb.Append(description);
            sb.Append(" folder");

            folderDialog.Description = sb.ToString();
            folderDialog.ShowNewFolderButton = true;
            folderDialog.SelectedPath = DefaultFolders.GetFolder(name);
            DialogResult result = folderDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string newpath = folderDialog.SelectedPath;
                bool setFolderResult = DefaultFolders.SetFolder(name, newpath, requireWriteAccess);
                if (setFolderResult == false)
                {
                    string message = "Could not set folder to "+newpath+"\r\nThis is maybe because you do not have write access to it.";

                    if (requireWriteAccess == false)
                    {
                        message = "Could not set to this folder";
                    }

                    UserMessage.Show(message, "Failed to set folder", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                textBox.Text = newpath;
                UserSettings.SetStringValue(mDefaultFoldersCategory, name, newpath);
            }
        }

        // ************************************************************************************************
        private void mSetPhotosAndVideoButton_Click(object sender, EventArgs e)
        {
            ProcessSettingDefaultFolder(MyPictures, "photos and video", mPhotoAndVideoTextBox, false);
        }

        // ************************************************************************************************
        private void mSetMusicAndAudioButton_Click(object sender, EventArgs e)
        {
            ProcessSettingDefaultFolder(MyMusic, "music and audio", mMusicAndAudioTextBox, false);
        }

        // ************************************************************************************************
        private void mSetProjectButton_Click(object sender, EventArgs e)
        {
            ProcessSettingDefaultFolder(MyProjects, "projects", mProjectTextBox, true);
        }

        // ************************************************************************************************
        private void mSetAuthoredRootButton_Click(object sender, EventArgs e)
        {
            ProcessSettingDefaultFolder(AuthoredFolder, "authored root", mAuthoredRootTextBox, true);
        }

        // ************************************************************************************************
        private void mResetButton_Click(object sender, EventArgs e)
        {
            UserSettings.ResetCategory(mDefaultFoldersCategory);
            SetFolderStrings();
        }
    }
}
