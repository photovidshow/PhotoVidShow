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
using System.IO;
using DVDSlideshow;

namespace dvdslideshowfontend
{
    public partial class CBurnPreviousEncodedProjectWindow : Form
    {
        private string mSelectedPath = "";
        public string SelectedPath
        {
            get { return mSelectedPath; }
        }

        private CGlobals.VideoType mSelectedType = CGlobals.VideoType.DVD;
        public CGlobals.VideoType SelectedType
        {
            get { return mSelectedType; }
        }

        private List<string> mProjectDescription = new List<string>();
        private List<CGlobals.VideoType> mProjectType = new List<CGlobals.VideoType> ();
        private List<string> mProjectPath = new List<string>();

        public CBurnPreviousEncodedProjectWindow()
        {
            InitializeComponent();
            mSelectedProjectDescription.Text = "";
            PopulateListBox();

            this.Shown += CBurnPreviousEncodedProjectWindow_Shown;
           
        }

        private void CBurnPreviousEncodedProjectWindow_Shown(object sender, EventArgs e)
        {
            //
            // If there is no previously created projects, no point continuing
            //
            if (mProjectsListBox.Items.Count == 0)
            {
                UserMessage.Show("No previously created slideshows found", "No created slideshows", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
            }
        }

        public long DirSize(DirectoryInfo d)
        {
            long Size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                Size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                Size += DirSize(di);
            }
            return (Size);
        }

        private long GetDirSize(string path)
        {
            DirectoryInfo d = new DirectoryInfo(path);
            long length = DirSize(d);
            return length / 1024 / 1024;
        }

        private void PopulateListBox()
        {
            string user_path = DefaultFolders.GetFolder("AuthoredFolder");
            if (Directory.Exists(user_path) == true)
            {
                string[] results = Directory.GetDirectories(user_path);
                foreach (string path in results)
                {

                    string video_ts_path = path + "\\VIDEO_TS";
                    if (Directory.Exists(video_ts_path) == true)
                    {
                        string name = Path.GetFileName(path);
                        mProjectsListBox.Items.Add(name);
                        mProjectDescription.Add("Project type: DVD-Video "+ GetDirSize(path).ToString() + " Mb");
                        mProjectType.Add(CGlobals.VideoType.DVD);
                        mProjectPath.Add(path);

                    }
                    else
                    {
                        string bdmvroot_path = path + "\\BDMVROOT";
                        if (Directory.Exists(bdmvroot_path) == true)
                        {
                            string name = Path.GetFileName(path);
                            mProjectsListBox.Items.Add(name);
                            mProjectDescription.Add("Project type: Blu-ray " + GetDirSize(path).ToString() + " Mb");
                            mProjectType.Add(CGlobals.VideoType.BLURAY);
                            mProjectPath.Add(bdmvroot_path);
                        }
                    }
                }
            }           
        }

        private void mProjectsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = mProjectsListBox.SelectedIndex;
            if (index >=0 && index < mProjectDescription.Count)
            {
                mSelectedProjectDescription.Text = mProjectDescription[index];
                mSelectedType = mProjectType[index];
                mSelectedPath = mProjectPath[index];
                mSelectProjectButton.Enabled = true;
       
            }
            else
            {
                mSelectedProjectDescription.Text = "";
                mSelectedPath = "";
                mSelectProjectButton.Enabled = false;
            }
        }

        private void mSelectProjectButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mCancelButton_Click(object sender, EventArgs e)
        {
            mSelectedPath = "";
            this.Close();
        }
    }
}
