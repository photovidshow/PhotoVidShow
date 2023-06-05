using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;
using ManagedCore;

namespace dvdslideshowfontend
{
    public partial class ImportSlideshowsForm : Form
    {
        private CProject mLoadedProject = null;
        private bool mImportForIntroSlideshow = false;

        public bool OnlyAllowOneSelected
        {
            get { return mImportForIntroSlideshow; }
            set { mImportForIntroSlideshow = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slideshow_filename"></param>
        public ImportSlideshowsForm(string slideshow_filename, CProject p, bool importForIntroSlideshow)
        {
            mImportForIntroSlideshow = importForIntroSlideshow;
            mLoadedProject = p;
            InitializeComponent();

            if (mImportForIntroSlideshow)
            {
                mSelectAllTickBox.Visible = false;
                mLabel1.Text = "Select which slideshow to import as the intro slideshow";
                this.Text = "Import a slideshow as the intro slideshow";
            }

            bool firstItem = true;
            this.mSlideshowsFoundTextBox.Text = "Slideshow(s) found in project '"+System.IO.Path.GetFileName(slideshow_filename+"'");
            foreach (CSlideShow slideshow in mLoadedProject.GetAllProjectSlideshows(true))
            {
                string name = p.GetSlideShowReferenceTextInMenu(slideshow);

                bool ticked = firstItem || (mImportForIntroSlideshow == false);
                firstItem= false;

                mCheckedListBox.Items.Add(name, ticked);
            }
            mCheckedListBox.CheckOnClick = true;

            if (mImportForIntroSlideshow == false)
            {
                mSelectAllTickBox.Checked = true;
            }

            Owner = Form1.mMainForm;
         

        }
 
        /// <summary>
        /// 
        /// </summary>
        public static void ImportSlideshows(bool forPreMenuSlideshow)
        {
            System.Windows.Forms.OpenFileDialog LoadProjectDialog = new OpenFileDialog();

            if (forPreMenuSlideshow == false)
            {
                LoadProjectDialog.Title = "Please select which project to import slideshows from";
            }
            else
            {
                LoadProjectDialog.Title = "Please select which project to import a slideshow from";
            }

            LoadProjectDialog.InitialDirectory = DefaultFolders.GetFolder("MyProjects");
            LoadProjectDialog.Filter = "Project Files (*.pds;*.pds2)|*.pds;*.pds2";
            LoadProjectDialog.FilterIndex = 2;
            LoadProjectDialog.RestoreDirectory = true;

            if (!LoadProjectDialog.ShowDialog().Equals(System.Windows.Forms.DialogResult.OK))
            {
                return;
            }

            CProject p;
            CProject current_projet = CGlobals.mCurrentProject;
            bool load_success = false;

            ManagedCore.MissingFilesDatabase.GetInstance().Clear();

            do
            {
                p = new CProject();

                ProgressWindow progressWindow = new ProgressWindow(Form1.mMainForm, p.Load, null);
                progressWindow.StartDelegateObject= progressWindow;

                progressWindow.Text = "Loading";
                progressWindow.CancelButtons.Enabled = false;

                // hack above code seems to override but why????/
                CGlobals.mCurrentProject = current_projet;

                p.mFilename = LoadProjectDialog.FileName;
         
                progressWindow.ShowDialog();

                if (ManagedCore.MissingFilesDatabase.GetInstance().GetUnlinkedFiles().Count == 0)
                {
                    load_success = true;
                }
                else
                {
                    DialogResult res = UserMessage.Show("Media files this project require can not be found. This may be due to the files being moved to another folder after the project was originally created. Press OK to relink these files or Cancel to abort", "Missing files",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);

                    if (res == DialogResult.OK)
                    {

                        ArrayList missing_files = ManagedCore.MissingFilesDatabase.GetInstance().GetMissingFilesData();

                        ReLinkFilesDialog dialog = new ReLinkFilesDialog();

                        dialog.HandoverMissingFilesList(missing_files);

                        res = dialog.ShowDialog();
                    }

                    // keep current project
                    if (res == DialogResult.Cancel)
                    {
                        return;
                    }
                }

            } while (load_success == false);
 
            if (ManagedCore.MissingFilesDatabase.GetInstance().AbortedLoad == true)
            {
                string reason = ManagedCore.MissingFilesDatabase.GetInstance().AbortedReasonString;
                if (reason != "")
                {
                    ManagedCore.CDebugLog.GetInstance().PopupError("Project could not load...\n\r\n\r"+ reason);
                }

                return;
            }

            ImportSlideshowsForm importform = new ImportSlideshowsForm(LoadProjectDialog.FileName, p, forPreMenuSlideshow);
            importform.ShowDialog();
            Form1.mMainForm.GoToMainMenu();

            if (forPreMenuSlideshow == false)
            {
                Form1.mMainForm.GetDecorationManager().SelectFirstSlideshowInCurrentMenu();
            }

            Form1.mMainForm.GetDecorationManager().RePaint();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mCheckedListBox_SelectedValueChanged(object sender, EventArgs e)
        {           
            mSelectAllTickBox.Checked = false;
        }

        private void mCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (mImportForIntroSlideshow == true)
            {
                if (e.NewValue == CheckState.Checked)
                {
                    for (int ix = 0; ix < mCheckedListBox.Items.Count; ++ix)
                    {
                        if (e.Index != ix) mCheckedListBox.SetItemChecked(ix, false);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mSelectAllTickBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mSelectAllTickBox.Checked == true)
            {
                for (int i = 0; i < mCheckedListBox.Items.Count; i++)
                {
                    mCheckedListBox.SetItemChecked(i, true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mImportButton_Click(object sender, EventArgs e)
        {
            int count =0;

            List<CSlideShow> slideshows = new List<CSlideShow>();
            List<String> slideshows_menu_text = new List<String>();

            foreach (CSlideShow slideshow in mLoadedProject.GetAllProjectSlideshows(true))
            {
                if (mCheckedListBox.GetItemChecked(count)==true)
                {
                    slideshows.Add(slideshow);
                    slideshows_menu_text.Add(mLoadedProject.GetSlideShowReferenceTextInMenu(slideshow));
                }

                count++;
            }

            if (slideshows.Count > 0)
            {
                //
                // Is a pre menu slideshow ?
                //
                if (mImportForIntroSlideshow == true)
                {
                    CGlobals.mCurrentProject.PreMenuSlideshow = slideshows[0];
                    slideshows[0].Name = CProject.PreMenuIdentityString;

                    Form1.mMainForm.SelectedSlideShow(CGlobals.mCurrentProject.PreMenuSlideshow.Name);
                }
                else
                {
                    CGlobals.mCurrentProject.ImportSlideshows(slideshows, slideshows_menu_text);

                }

                DialogResult res = UserMessage.Show("Import successful!", "Import complete",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);  
            }

            this.Close();
        }    
    }
}
