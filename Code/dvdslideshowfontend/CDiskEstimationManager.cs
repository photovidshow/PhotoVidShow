using System;
using System.Windows.Forms;
using DVDSlideshow;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using CustomButton;
using ManagedCore;

namespace dvdslideshowfontend
{
	/// <summary>
	/// Summary description for CDiskEstimationManager.
	/// </summary>
	public class CDiskEstimationManager
	{
        private ToolStripDropDownButton mOutputDiskTypeDropDownButton;
        private ToolStripDropDownButton mOutputAspectDropDownButton;
		private Panel mInnerDiskEstimationPanel;
		private PictureBox mPB ;

        private ToolStripMenuItem _videoFileToolStripMenuItem;

        private ToolStripMenuItem _dVDVideoToolStripMenuItem;
        private ToolStripMenuItem _blurayVideoToolStripMenuItem;

        private string mDVDString = "DVD-Video";
        private string mVideoString = "Video file";
        private string mBluRayString = "Blu-ray";

        // *********************************************************************
		public CDiskEstimationManager()
		{
			//
			// TODO: Add constructor logic here
			//

			mOutputDiskTypeDropDownButton = Form1.mMainForm.GetOutputDiskTypeDropDownButton();
            mOutputAspectDropDownButton = Form1.mMainForm.GetOutputAspectButton();

			mInnerDiskEstimationPanel = Form1.mMainForm.GetInnerDiskEstimationPanel();
			mPB = Form1.mMainForm.GetDiskEstimationInnerPictureBox();

			mPB.Image = new Bitmap(mInnerDiskEstimationPanel.Width, mInnerDiskEstimationPanel.Height);


        //    mOutputDiskTypeDropDownButton.Items.Add(new ImageComboItem(mVideoString,0 ));
         //   mOutputDiskTypeDropDownButton.Items.Add(new ImageComboItem(mDVDString, 0));


			Form1.mMainForm.Resize+=new EventHandler(MainFormResize);


            mOutputDiskTypeDropDownButton.DropDownItems[0].Click += new EventHandler(this.SelectVideoForComputer);
            mOutputDiskTypeDropDownButton.DropDownItems[1].Click += new EventHandler(this.SelectDVDVideo);
            mOutputDiskTypeDropDownButton.DropDownItems[2].Click += new EventHandler(this.SelectBluRayVideo);
          

            mOutputAspectDropDownButton.DropDownItems[0].Click += new EventHandler(this.Select16By9Aspect);
            mOutputAspectDropDownButton.DropDownItems[1].Click += new EventHandler(this.Select4By3Aspect);

			CProject.InformWhenProjectChange.Add(new ProjectChanged(this.ProjectHadChanged));

            _videoFileToolStripMenuItem = Form1.mMainForm.GetVideoFileToolStripMenuItem();
            _dVDVideoToolStripMenuItem = Form1.mMainForm.GetDVDVideoToolStripMenuItem();
            _blurayVideoToolStripMenuItem = Form1.mMainForm.GetBlurayVideoToolStripMenuItem();

            _videoFileToolStripMenuItem.Click += new System.EventHandler(this.videoFileToolStripMenuItem_Click);
            _dVDVideoToolStripMenuItem.Click += new System.EventHandler(this.dVDVideoToolStripMenuItem_Click);
            _blurayVideoToolStripMenuItem.Click +=new System.EventHandler(this.blurayVideoToolStripMenuItem_Click);

            Form1.mMainForm.GetDVDMenuButton().MouseEnter += new EventHandler(MenuButtonEnter);

            Form1.mMainForm.GetDVDMenuButton().DropDownItemClicked += new ToolStripItemClickedEventHandler(menuToolStip_DropDownItemClicked);

			RebuildPB();
		}

        // *********************************************************************
        void MenuButtonEnter(object sender, EventArgs e)
        {
            ToolStripSplitButton menuToolStip = Form1.mMainForm.GetDVDMenuButton();

            if (menuToolStip.Visible == false)
            {
                return;
            }

            CMainMenu mainMenu = CGlobals.mCurrentProject.MainMenu;
            System.Collections.ArrayList menus = mainMenu.GetSelfAndAllSubMenus();
            if ((menuToolStip.DropDownItems.Count==1 && menus.Count ==1) ||
                (menus.Count + 1 == menuToolStip.DropDownItems.Count))
            {
                return;
            }

            menuToolStip.DropDownItems.Clear();
            if (menus.Count <= 1)
            {
                menuToolStip.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                            Form1.mMainForm.GetIntroSlideshowToolStripMenuItem()});
                return;
            }

            Image menuImage = Form1.mMainForm.GetDiskMenuImageList().Images[0];
            Image subMenuImage = Form1.mMainForm.GetDiskMenuImageList().Images[1];

            ToolStripItem[] items = new ToolStripItem[menus.Count + 1];
            items[0] = Form1.mMainForm.GetIntroSlideshowToolStripMenuItem();
            for (int i = 1; i <= menus.Count; i++)
            {
                ToolStripMenuItem item = null;
              
                if (i == 1)
                {
                    item = new ToolStripMenuItem("Top menu (" + i + ")", menuImage);
                }
                else
                {
                    item = new ToolStripMenuItem("Sub menu (" + i + ")", subMenuImage);
                }

                item.ImageScaling = ToolStripItemImageScaling.None;
                items[i] = item;
            }

            menuToolStip.DropDownItems.AddRange(items);

          
        }

        // *********************************************************************
        void menuToolStip_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripSplitButton menuToolStip = Form1.mMainForm.GetDVDMenuButton();
            int index = menuToolStip.DropDownItems.IndexOf(e.ClickedItem);
            if (index > 0)
            {
                index--;    // First item disk intro
                System.Collections.ArrayList menus = CGlobals.mCurrentProject.MainMenu.GetSelfAndAllSubMenus();
                if (index < menus.Count)
                {
                    Form1.mMainForm.GetDecorationManager().SelectMenuAndSlideshow(menus[index] as CMainMenu, null);
                }
            }
        }

        // *********************************************************************
        public void MainFormResize(object o, System.EventArgs e)
		{
			RebuildPB();
		}

        // *********************************************************************
		public void ProjectHadChanged(bool store_memento, string description)
		{
			RebuildPB();
		}

        private delegate void SelectOutputDelegate(object o, EventArgs e);

        // *********************************************************************
        public void SelectVideoForComputer(object o, EventArgs e)
        {
            if (Form1.mMainForm.InvokeRequired == true)
            {
                SelectOutputDelegate my_delegate = new SelectOutputDelegate(SelectVideoForComputer);

                Form1.mMainForm.BeginInvoke(my_delegate, new object[] { o, e });
        
            }
            OutputChanged(CGlobals.VideoType.MP4, mOutputDiskTypeDropDownButton.DropDownItems[0], mVideoString);
        }

        // *********************************************************************
        public void SelectDVDVideo(object o, EventArgs e)
        {
            if (Form1.mMainForm.InvokeRequired == true)
            {
                SelectOutputDelegate my_delegate = new SelectOutputDelegate(SelectDVDVideo);

                Form1.mMainForm.BeginInvoke(my_delegate, new object[] { o, e });
            }

            OutputChanged(CGlobals.VideoType.DVD, mOutputDiskTypeDropDownButton.DropDownItems[1], mDVDString);
        }

        
        // *********************************************************************
        public void SelectBluRayVideo(object o, EventArgs e)
        {
            if (Form1.mMainForm.InvokeRequired == true)
            {
                SelectOutputDelegate my_delegate = new SelectOutputDelegate(SelectBluRayVideo);
                Form1.mMainForm.BeginInvoke(my_delegate, new object[] { o, e });
            }

            OutputChanged(CGlobals.VideoType.BLURAY, mOutputDiskTypeDropDownButton.DropDownItems[2], mBluRayString);
        }


        // *********************************************************************
        private void OutputChanged(CGlobals.VideoType type, ToolStripItem item, string displaytext)
		{
            Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();
            Form1.mMainForm.GoToMainMenu();

			DVDSlideshow.CDiskAuthoringPreferences pr = CGlobals.mCurrentProject.DiskPreferences;

            //
            // Check if selected blu-ray that we only allow 16:9 aspect (for now anyway)
            //
            if (pr.OutputAspectRatio != DVDSlideshow.CGlobals.OutputAspectRatio.TV16_9)
            {
                if (type == CGlobals.VideoType.BLURAY)
                {
                    DialogResult res = UserMessage.Show(item.Text + " output is not allowed to have a non 16:9 aspect ratio", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    return;
                }
            }

            bool changed = pr.OutputVideoType != type;

            if (changed)
            {
                //
                // Ok if we have switched to video output and current slideshow is the intro slideshow, re-select first slideshow in current menu
                //
                if (type == CGlobals.VideoType.MP4)
                {

                    if (CGlobals.mCurrentProject.PreMenuSlideshow != null &&
                        Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow == CGlobals.mCurrentProject.PreMenuSlideshow)
                    {
                        Form1.mMainForm.GetDecorationManager().SelectFirstSlideshowInCurrentMenu();
                    }
                }

                //
                // If selected (changed) video ouput, check if multiple slideshows exists, if so warn user.
                //
                if (type == CGlobals.VideoType.MP4)
                {
                    if (CGlobals.mCurrentProject.GetAllProjectSlideshows(false).Count > 1)
                    {
                        DialogResult res = UserMessage.Show("Multiple slideshows currently exist for this project.  Only the currently selected slideshow will be available for this output, do you wish to continue?", "Multiple slideshows exist",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                        if (res == DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                }
            }

            pr.OutputVideoType = type;
            Form1.mMainForm.SuspendLayout();

            if (type == CGlobals.VideoType.MP4)
            {
                Form1.mMainForm.GetCreateAndBurnButton().Visible = false;
                Form1.mMainForm.GetCreateVideoButton().Visible = true;
                Form1.mMainForm.GetDVDMenuButton().Visible = false;
                Form1.mMainForm.GetSelectSlideshowToolStripItemCombo().Visible = false;
                Form1.mMainForm.GetIntroSlideshowToolStripMenuItem().Visible = false;
                Form1.mMainForm.GetDVDMenuButtonSeperator().Visible = false;
                Form1.mMainForm.GetSetMenuthumbnailButton().Visible = false;
            }
            else
            {
                Form1.mMainForm.GetCreateAndBurnButton().Visible = true;
                Form1.mMainForm.GetCreateVideoButton().Visible = false;
                Form1.mMainForm.GetDVDMenuButton().Visible = true;

                //
                // The slideshow combo visibility is detmined on number of items.
                //
                Form1.mMainForm.GetDiskMenuManager().UpdateSlideshowList(false);

                if (type == CGlobals.VideoType.BLURAY ||
                    type == CGlobals.VideoType.DVD)
                {
                    Form1.mMainForm.GetIntroSlideshowToolStripMenuItem().Visible = true;
                }
                else
                {
                    Form1.mMainForm.GetIntroSlideshowToolStripMenuItem().Visible = false;
                }
                Form1.mMainForm.GetDVDMenuButtonSeperator().Visible = true;
                Form1.mMainForm.GetSetMenuthumbnailButton().Visible = true;
            }
            Form1.mMainForm.GetDiskMenuManager().DiskMenuNavigationManager.EnablePlayAllOptions();

            Form1.mMainForm.ResumeLayout();

            mOutputDiskTypeDropDownButton.Text = displaytext;
            mOutputDiskTypeDropDownButton.Image = item.Image;

            if (changed)
            {
                CGlobals.mCurrentProject.DeclareChange(true, "Output Type Change");

                Form1.mMainForm.GoToMainMenu();
                Form1.mMainForm.UpdateOuterTabControlTabs();
                Form1.mMainForm.GetDecorationManager().RePaint();

                //
                // Some CM marked thubnmails may also need to change
                //
                Form1.mMainForm.GetSlideShowManager().ValidateAllCMThumbnails();
            }
            
		}

        // *********************************************************************
        private void SelectVideoType()
        {
            // ensure the setting show in the Combo box matches the stored value

            DVDSlideshow.CDiskAuthoringPreferences pr = CGlobals.mCurrentProject.DiskPreferences;

            if (pr.OutputVideoType == CGlobals.VideoType.MP4)
            {
                SelectVideoForComputer(this, new EventArgs());
            }
            else if (pr.OutputVideoType == CGlobals.VideoType.DVD)
            {
                SelectDVDVideo(this, new EventArgs());
            }
            else if (pr.OutputVideoType == CGlobals.VideoType.BLURAY)
            {
                this.SelectBluRayVideo(this, new EventArgs());
            }
         
        }

        // *********************************************************************
        public void SetupNewProjectOutputType()
        {
            //
            // This function gets called when we initiate a new project, it sets up
            // the output project type to the default value stored in the UserSettings
            //
            CGlobals.VideoType type = (CGlobals.VideoType)UserSettings.GetIntValue("OutputType", "DefaultOutputType");
            SetupNewProjectOutputType(type);
        }

        // *********************************************************************
        private void SetupNewProjectOutputType(CGlobals.VideoType type)
        {
            if (type == CGlobals.mCurrentProject.DiskPreferences.OutputVideoType)
            {
                return;
            }
            if (type == CGlobals.VideoType.MP4)
            {         
                SelectVideoForComputer(this, new EventArgs());
                OutputAspectChanged(CGlobals.OutputAspectRatio.TV16_9);
            }
            else if (type == CGlobals.VideoType.DVD)
            {
                SelectDVDVideo(this, new EventArgs());
                OutputAspectChanged(CGlobals.OutputAspectRatio.TV16_9);
            }
            else if (type == CGlobals.VideoType.BLURAY)
            {
                SelectBluRayVideo(this, new EventArgs());
                OutputAspectChanged(CGlobals.OutputAspectRatio.TV16_9);
            }
        }

        // *********************************************************************
        private delegate void RebuildPBDelegate();
		public void RebuildPB()
		{
            return;
            /*
            if (Form1.mMainForm.InvokeRequired)
            {
                Form1.mMainForm.BeginInvoke(new RebuildPBDelegate(RebuildPB));
                return ;
            }

			int box_h = 12;
			int start_v=10;


            SelectVideoType();

			// get file size 

            DVDSlideshow.CDiskAuthoringPreferences pr = CGlobals.mCurrentProject.DiskPreferences;

            CGlobals.VideoType type = pr.OutputVideoType;
			int max_mb = pr.MaxMBOfDiskType;

			int w = mPB.Width-10;
			int h = mPB.Height;

			Graphics gp = Graphics.FromImage(mPB.Image);

			SolidBrush backbrush = new SolidBrush(this.mPB.BackColor);
			gp.FillRectangle(backbrush,0,0, mPB.Width,mPB.Height);

			SolidBrush brush = new SolidBrush(Color.White);
			gp.FillRectangle(brush,0,start_v,w,box_h);

			Pen p = new Pen(Color.Black);
			gp.DrawRectangle(p,0,start_v,w,box_h);

            Font a = new Font("Segeo UI", 8);

			Brush brush2 = new SolidBrush(Color.Black);

			gp.DrawString("0MB",a,brush2,0,-2);

            if (type == CGlobals.VideoType.DVD)
			{
				gp.DrawString(max_mb.ToString()+"MB",a,brush2,w-70,-2);
			}
			else
			{
				float x1 = ((float)w-50) / 700.0f;
				x1*=650;
				int xx1 = (int)x1;

				gp.DrawString(max_mb.ToString()+"MB",a,brush2,w-65,-2);
				gp.DrawString("650MB",a,brush2,xx1-20,-2);
				Pen p4 = new Pen(Color.BlueViolet,1);
				p4.DashStyle = DashStyle.Dash;
				gp.DrawLine(p4,xx1,start_v+1,xx1,11+box_h-2);
			}

			Pen p3 = new Pen(Color.Red,1);
			p3.DashStyle = DashStyle.Dash;

			float meg_pix = ((float)max_mb) / ((float)w-50);  

			gp.DrawLine(p3,w-50,start_v+1,w-50,11+box_h-2);

			float estimated_length = (float) CGlobals.mCurrentProject.GetDiskUsageEstimation(false,false);

			float megs_used = estimated_length/1024.0f/1024.0f;
		
			int  pixels_l = (int)(megs_used/meg_pix);

			SolidBrush green_brush = new SolidBrush(Color.FromArgb(150,0,255,0));
			SolidBrush red_brush = new SolidBrush(Color.FromArgb(150,255,0,0));

			if (pixels_l > w-50)
			{
				if (pixels_l > w-4)
				{
					pixels_l = w-4;
				}
				gp.FillRectangle(red_brush,2,start_v+1,pixels_l,box_h-1);
			}
			else
			{
				gp.FillRectangle(green_brush,2,start_v+1,pixels_l,box_h-1);
			}

			gp.DrawString(((int)megs_used).ToString()+"MB",a,brush2,0,start_v+box_h);

			gp.Dispose();

			mPB.Invalidate();
             */
		}

        // *********************************************************************
        private void Select16By9Aspect(object o, EventArgs e)
        {
            OutputAspectChanged(CGlobals.OutputAspectRatio.TV16_9);
        }

        // *********************************************************************
        private void Select4By3Aspect(object o, EventArgs e)
        {
            OutputAspectChanged(CGlobals.OutputAspectRatio.TV4_3);
        }

        // *********************************************************************
        // change aspect ratio
        private void OutputAspectChanged(CGlobals.OutputAspectRatio ratio)
        {
            CSlideShowManager ssm = Form1.mMainForm.GetSlideShowManager(); 

            if (ssm != null) ssm.StopIfPlayingAndWaitForCompletion();

            Form1.mMainForm.GoToMainMenu();

            DVDSlideshow.CDiskAuthoringPreferences pr = CGlobals.mCurrentProject.DiskPreferences;

            if (pr.OutputVideoType == CGlobals.VideoType.BLURAY )
            {
                if (ratio != CGlobals.OutputAspectRatio.TV16_9)
                {
                    DialogResult res = UserMessage.Show("Blu-ray outputs are only allowed to be 16:9 widescreen", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    return;
                }
            }

            CGlobals.OutputAspectRatio oldAspect = CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio;

            if (ratio == CGlobals.OutputAspectRatio.TV16_9)
            {
                if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV16_9)
                {
                    return;
                }

                mOutputAspectDropDownButton.Image = mOutputAspectDropDownButton.DropDownItems[0].Image;
                CGlobals.mCurrentProject.RebuildToNewCanvas(CGlobals.OutputAspectRatio.TV16_9);
            }
            else
            {
                if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV4_3)
                {
                    return;
                }

                mOutputAspectDropDownButton.Image = mOutputAspectDropDownButton.DropDownItems[1].Image;
                CGlobals.mCurrentProject.RebuildToNewCanvas(CGlobals.OutputAspectRatio.TV4_3);
            }

            if (oldAspect != ratio)
            {
                CGlobals.mCurrentProject.DeclareSlideAspectChange(oldAspect, ratio);
            }

            ssm.InvalidateAllThumbnailsDueToApectRatioChange();

            // rebuild viewing area
            Form1.mMainForm.ViewingAreaResize(true);

            if (Form1.mMainForm.GetForceNoMemento() == false)
            {
                CGlobals.mCurrentProject.DeclareChange(true, "Output Aspect Ratio Change");
            }

            CDecorationsManager dm = Form1.mMainForm.GetDecorationManager();

            if (dm != null)
            {
                //	this.mDecorationManager.RePaint();
                //	this.pictureBox1.Invalidate();
                // big hack
                dm.SetCurrentSlide(null);
                Form1.mMainForm.GoToMainMenu();
                dm.RePaint();
                //	this.pictureBox1.Invalidate();
            }
        }


        // *****************************************************************************************************
        public void SetOutputAspectRatioComboAndOutputVideoType()
        {
         
            if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV16_9)
            {
                mOutputAspectDropDownButton.Image = mOutputAspectDropDownButton.DropDownItems[0].Image;
            }
            if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV4_3)
            {
                mOutputAspectDropDownButton.Image = mOutputAspectDropDownButton.DropDownItems[1].Image;
            }

            SelectVideoType();
         
        }


        // *****************************************************************************************************
        public void SelectInitialOutputType(Form mainForm, bool initialStartup)
        {
            //
            // Gets called when the program first starts up. From here this method then decides what the initial
            // project output type will be
            //
            CGlobals.VideoType outputType = (CGlobals.VideoType)UserSettings.GetIntValue("OutputType", "DefaultOutputType");

            if (outputType == CGlobals.VideoType.DVD)
            {
                _dVDVideoToolStripMenuItem.Checked = true;
            }
            if (outputType == CGlobals.VideoType.BLURAY)
            {
                _blurayVideoToolStripMenuItem.Checked = true;
            }
            if (outputType == CGlobals.VideoType.MP4)
            {
                _videoFileToolStripMenuItem.Checked = true;
            }

            if (initialStartup == true)
            {
                // 
                // Let the user select the type?
                //
                if (UserSettings.GetBoolValue("OutputType", "ShowOptionsAtStartup") == true)
                {
                    mainForm.Shown += new EventHandler(ShowSelectOutputTypeWindow);
                }
                else
                {
                    //
                    // Else set it to the value stored in the UserSettings
                    //
                    SetupNewProjectOutputType(outputType);
                    Form1.mMainForm.ShowingStartupOptions = false;
                }
            }
            else
            {
                ShowSelectOutputTypeWindow(false);
            }
        }

        //*******************************************************************
        private void ShowSelectOutputTypeWindow(object o, EventArgs e)
        {
            ShowSelectOutputTypeWindow(true);
        }

        //*******************************************************************
        private void ShowSelectOutputTypeWindow(bool initialStartup)
        {
            CustomButton.StartupOutputChoiceForm socf = new StartupOutputChoiceForm(initialStartup);
            socf.StartPosition = FormStartPosition.CenterParent;
            socf.ShowDialog(Form1.mMainForm);

            CSlideShow ss =  Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow;
            ss.mDefaultSlide.DisplayLength = socf.DefaultSlideTime;
            ss.mDefaultSlide.UsePanZoom = socf.EnableAutoPanZoom;

            //
            // Setup project output type to the option the user selected
            //
            if (socf.Selected == StartupOutputChoiceForm.StartupOptions.DVDVideoDisc)
            {
                SetupNewProjectOutputType(CGlobals.VideoType.DVD);
            }
            else if (socf.Selected == StartupOutputChoiceForm.StartupOptions.videoFile)
            {
                SetupNewProjectOutputType(CGlobals.VideoType.MP4);
            }
            else if (socf.Selected == StartupOutputChoiceForm.StartupOptions.blurayDisc)
            {
                SetupNewProjectOutputType(CGlobals.VideoType.BLURAY);
            }

            //
            // If we have unticked 'Show at startup' store choice in UserSettings
            //
            if (socf.ShowOptionAtStartup == false && initialStartup == true)
            {
                UserSettings.SetBoolValue("OutputType", "ShowOptionsAtStartup", false);

                if (socf.Selected == StartupOutputChoiceForm.StartupOptions.DVDVideoDisc)
                {
                    dVDVideoToolStripMenuItem_Click(this, new EventArgs());
                }
                else if ( socf.Selected == StartupOutputChoiceForm.StartupOptions.videoFile)
                {
                    videoFileToolStripMenuItem_Click(this, new EventArgs());
                }
                else
                {
                    blurayVideoToolStripMenuItem_Click(this, new EventArgs());
                }
            }

            Form1.mMainForm.ShowingStartupOptions= false;
        }

        // *****************************************************************************************************
        private void videoFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_videoFileToolStripMenuItem.Checked == true)
            {
                return;
            }
            else
            {
                _videoFileToolStripMenuItem.Checked = true;
                _dVDVideoToolStripMenuItem.Checked = false;
                _blurayVideoToolStripMenuItem.Checked = false;
                UserSettings.SetIntValue("OutputType", "DefaultOutputType", (int)CGlobals.VideoType.MP4);
            }
        }

        // *****************************************************************************************************
        private void dVDVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_dVDVideoToolStripMenuItem.Checked == true)
            {
                return;
            }
            else
            {
                _videoFileToolStripMenuItem.Checked = false;
                _dVDVideoToolStripMenuItem.Checked = true;
                _blurayVideoToolStripMenuItem.Checked = false;
                UserSettings.SetIntValue("OutputType", "DefaultOutputType", (int)CGlobals.VideoType.DVD);
            }
        }

        // *****************************************************************************************************
        private void blurayVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_blurayVideoToolStripMenuItem.Checked == true)
            {
                return;
            }
            else
            {
                _videoFileToolStripMenuItem.Checked = false;
                _dVDVideoToolStripMenuItem.Checked = false;
                _blurayVideoToolStripMenuItem.Checked = true;
                UserSettings.SetIntValue("OutputType", "DefaultOutputType", (int)CGlobals.VideoType.BLURAY);
            }
        }
	}
}
