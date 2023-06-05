using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using DVDSlideshow;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using ZBobb;
using System.IO;
using ManagedCore;
using DVDSlideshow.GraphicsEng;


namespace dvdslideshowfontend
{
	/// <summary>
	/// Summary description for CDiskMenuManager.
	/// </summary>
	/// 

	public class CDiskMenuManager
	{
		private Form1	mMainWindow;
		private Panel	mMenuTemplatesPanel;
		private Panel	mMenuLayoutTemplatesPlanel;
		private Panel	mButtonStylePanel;

		private TrackBar MenuRepeatLengthTrackbar;
		private Label	 MenuRepeatDurationTextbox;

		private Button	AddNextMenuButton;
		private Button  RemoveSlideshowButton;
        private Button  AddSlideshowButton;
		private ContextMenu	ResetMusicMenu;

        private CDiskMenuNavigationManager mDiskMenuNavigationManager;
        private CDiskIntroSlideshowManager mDiskIntroSlideshowManager;
        private CDiskMenuHighlightManager mDiskHighlightManager;
        private OpenFileDialog mBackgroundOpenFileDialog;
        private OpenFileDialog mMusicOpenFileDialog;

        private ToolStripComboBox mSlideshowComboBox;
        private List<CSlideShow> mSlideshowsInComboBox = new List<CSlideShow>();    // linked to items in combo box;

        private bool mRebuildingLabels = false;
	
		// led
        private string mContextSlideshow;

		private		bool mRepeatLengthChanged=false;

        public Form1 MainWindow
        {
            get { return mMainWindow; }
        }

        public CDiskMenuNavigationManager DiskMenuNavigationManager
        {
            get { return mDiskMenuNavigationManager;  }
        }

        public CDiskIntroSlideshowManager DiskIntroSlideshowManager
        {
            get { return mDiskIntroSlideshowManager; }
        }

		public CDiskMenuManager(Form1 mainwindow, 
								Panel menutemplatepanel, 
								Panel menuLayoutpanel,
								Panel menuStylePanel,
								ContextMenu	ResetMusic)
		{
			ResetMusicMenu = ResetMusic;

			Form1.mMainForm.GetAddBackgroundMenuMusicButton().ContextMenu = ResetMusicMenu;

			mMainWindow = mainwindow;
            mDiskMenuNavigationManager = new CDiskMenuNavigationManager(this);
            mDiskIntroSlideshowManager = new CDiskIntroSlideshowManager(mMainWindow);
            mDiskHighlightManager = new CDiskMenuHighlightManager(this);

			mMenuTemplatesPanel = menutemplatepanel;
			mMenuLayoutTemplatesPlanel = menuLayoutpanel;
			mButtonStylePanel =menuStylePanel;

			MenuRepeatLengthTrackbar = mainwindow.GetMenuRepeatLengthTrackbar();
			MenuRepeatDurationTextbox = mainwindow.GetMenuRepeatDurationTextbox();

			MenuRepeatLengthTrackbar.ValueChanged += new EventHandler(this.MenuDurationTrackbarChanged);
			MenuRepeatLengthTrackbar.MouseUp += new MouseEventHandler(this.MenuDurationTrackbar_MouseUp);
			MenuRepeatLengthTrackbar.Scroll += new EventHandler(this.MenuDurationTrackbar_Scroll);

			AddNextMenuButton = Form1.mMainForm.GetAddNextMenuButton();
			RemoveSlideshowButton = Form1.mMainForm.GetRemoveSlideshowButton();
            AddSlideshowButton = Form1.mMainForm.GetAddNewSlideshowButton();

			AddNextMenuButton.Click +=new EventHandler(this.AddNextMenu_Click);
			RemoveSlideshowButton.Click+=new EventHandler(this.DeleteSlideshow_Click2);
			mainwindow.GetMenuThemePlayingThumbNailsCheckBox().CheckedChanged+=new EventHandler(this.UserChangedPlayingThumbnailsCheckBox);
            mainwindow.GetZoomingMenuBackGroundCheckBox().CheckedChanged += new EventHandler(this.UserChangedZoominBackgroundCheckBox);

			mainwindow.GetMenuAddDecorateButton().Click+=new EventHandler(this.MenuAddDecoration_Click);
            mainwindow.GetPreviewMenuButton().Click += new EventHandler(this.PreviewMenuButton_Click);

			//AddMenuTemplatesThumbnailsToTab();    done later because of video decors

			AddMenuLayoutThumbnailsToTab();
			AddButtonStylesThumbnailsToTab();

            mSlideshowComboBox = mainwindow.GetSelectSlideshowToolStripItemCombo();
            mSlideshowComboBox.SelectedIndexChanged += new EventHandler(SlideshowComboBox_SelectedIndexChanged);
			CProject.InformWhenProjectChange.Add(new ProjectChanged(this.ProjectHasChanged));
		}

        //*******************************************************************
        private void SlideshowComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mRebuildingLabels == true)
            {
                return;
            }

            //
            // Should not happen but just in case
            //
            if (mSlideshowsInComboBox.Count != mSlideshowComboBox.Items.Count)
            {
                UpdateSlideshowListInternal(false);
            }

            int index = mSlideshowComboBox.SelectedIndex;
            if (index < mSlideshowsInComboBox.Count)
            {
                CSlideShow ss = mSlideshowsInComboBox[index];

                if (ss == CGlobals.mCurrentProject.PreMenuSlideshow)
                {
                    mMainWindow.mIntroSlideshowToolStripMenuItem_Click(sender, e);
                }
                else
                {
                    mMainWindow.GetDecorationManager().SelectMenuAndSlideshow(null, ss);
                    mMainWindow.GoToMainMenu(true);     // this makes sure the correct menu tabs are set
                }
            }

        }

        //*******************************************************************
        public void UpdateSlideshowList(bool forceUpdate)
        {
            mRebuildingLabels = true;
            try
            {
                UpdateSlideshowListInternal(forceUpdate);
            }
            finally
            {
                mRebuildingLabels = false;
            }
        }

        //*******************************************************************
        private void UpdateSlideshowListInternal(bool forceUpdate)
        {   
            bool needsRebuild = false;
            int nextItem =0;

            if (forceUpdate)
            {
                needsRebuild = true;
            }
     
            //
            // Check various conditions where project slideshow list does not match combo list
            //
            if (needsRebuild==false && mSlideshowsInComboBox.Count != mSlideshowComboBox.Items.Count)
            {
                needsRebuild = true;
            }

            bool introAllowed = CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.DVD ||
                                CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.BLURAY;

            if (needsRebuild == false && CGlobals.mCurrentProject.PreMenuSlideshow != null && introAllowed)
            {
                if (mSlideshowComboBox.Items.Count > 0)
                {
                    if (CGlobals.mCurrentProject.PreMenuSlideshow != mSlideshowsInComboBox[0])
                    {
                        needsRebuild = true;
                    }
                    else
                    {
                        nextItem++;
                    }
                }
                else
                {
                    needsRebuild = true;
                }

            }
            if (needsRebuild == false)
            {
                ArrayList list = CGlobals.mCurrentProject.GetAllProjectSlideshows(false);
                foreach (CSlideShow item in list)
                {
                    if (nextItem >= mSlideshowsInComboBox.Count)
                    {
                        needsRebuild = true;
                        break;
                    }

                    if (item != mSlideshowsInComboBox[nextItem])
                    {
                        needsRebuild = true;
                        break;
                    }
                    nextItem++;
                }

                if (nextItem != mSlideshowComboBox.Items.Count)
                {
                    needsRebuild = true;
                }
            }

            //
            // Rebuild combo list of slideshow names
            //
            if (needsRebuild)
            {
                ArrayList newItems = new ArrayList();
                mSlideshowComboBox.Items.Clear();
                mSlideshowsInComboBox.Clear();

                if (CGlobals.mCurrentProject.PreMenuSlideshow != null && introAllowed)
                {
                    newItems.Add("Intro slideshow");    // pre menu slideshow is always called 'intro slideshow' in combo box
                    mSlideshowsInComboBox.Add(CGlobals.mCurrentProject.PreMenuSlideshow);
                }

                ArrayList list = CGlobals.mCurrentProject.GetAllProjectSlideshows(false);
                foreach (CSlideShow item in list)
                {
                    newItems.Add(item.GetSlideshowLabelName());                  
                    mSlideshowsInComboBox.Add(item);
                }

                mSlideshowComboBox.Items.AddRange(newItems.ToArray());
            }

            //
            // Make sure the select slideshow is the one selected in the combo list
            //
            CSlideShow selected = mMainWindow.GetSlideShowManager().mCurrentSlideShow;

            for (int index = 0; index < mSlideshowsInComboBox.Count; index++)
            {
                CSlideShow item = mSlideshowsInComboBox[index];     
                if (item == selected)
                {
                    if (mSlideshowComboBox.SelectedIndex != index)
                    {
                        mSlideshowComboBox.SelectedIndex = index;
                    }
                    break;
                }
            }


            //
            // Change visibility of combo, if only one item then hide
            //
            if (mSlideshowComboBox.Items.Count <= 1)
            {
                mSlideshowComboBox.Visible = false;
            }
            else
            {
                mSlideshowComboBox.Visible = true;
            }
        }

		//*******************************************************************
		public void ResetMusicButtonSelect()
		{
			Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();
			CMainMenu m = GetCurrentMenuInDecorationsWindow();
            m.MusicSlide = null;
			RebuildLabels();
			CGlobals.mCurrentProject.DeclareChange(true,"Menu Music Change");
		}

		//*******************************************************************
		public void MenuAddDecoration_Click(object o, EventArgs e)
		{
			Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();
            Form1.mMainForm.GetDecorationManager().SetMainTabControlToDecorationsTab();
		}

        //*******************************************************************
        private void PreviewMenuButton_Click(object o, EventArgs e)
        {
            Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();
            Form1.mMainForm.GoToMainMenu();

            CMainMenu m = GetCurrentMenuInDecorationsWindow();

            CustomButton.PreviewMenuForm pmf = new CustomButton.PreviewMenuForm(m);
            pmf.ShowDialog();
        }

		//*******************************************************************
		public void MenuDurationTrackbarChanged(object o, EventArgs e)
		{
			CMainMenu m = this.GetCurrentMenuInDecorationsWindow();
			if (m!=null)
			{
				m.Length = this.MenuRepeatLengthTrackbar.Value;
                m.BackgroundSlide.SetLengthWithoutUpdate(m.Length);
			}
			this.RebuildLabels();
		}

		
		//*******************************************************************
		public void MenuDurationTrackbar_Scroll(object o, EventArgs e)
		{
            Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();

			this.mRepeatLengthChanged=true;
		}

		//*******************************************************************
		public void MenuDurationTrackbar_MouseUp(object o, System.Windows.Forms.MouseEventArgs e)
		{
			if (this.mRepeatLengthChanged==true)
			{
			
				CGlobals.mCurrentProject.DeclareChange(true,"Menu Duration Change");
			}
			mRepeatLengthChanged=false;
		}

		//*******************************************************************
		public void AddNextMenu_Click(object o, EventArgs e)
		{
			Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();

			CSlide s = this.mMainWindow.GetDecorationManager().mCurrentSlide;

			CMainMenu mm = this.mMainWindow.mCurrentProject.GetMenuWhichContainsSlide(s);

			if (mm==null) return ;

			bool illegal_before =  mm.IsAnySubtitledDecorationIllegal();

			CMainMenu sub_menu = mm.CreateSubMenu();

			if (sub_menu!=null)
			{
				CSlideShow slidshow = CGlobals.mCurrentProject.CreateNewSlideshow(sub_menu, Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow);
			}

			this.mMainWindow.GetDecorationManager().RePaint();

			if (illegal_before==false &&
				mm.IsAnySubtitledDecorationIllegal()==true)
			{
                DialogResult res = UserMessage.Show("Adding a new sub menu has caused the link arrow button to intersect with a slideshow button. This will need to be resolved before burn time! There must be at least a 2% gap between all slideshow buttons and link arrow buttons and they are not allowed to intersect", "Error",
					MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
			}

		}

		//*******************************************************************
		public void AddMenuLayoutThumbnailsToTab()
		{
            int numberHorizontal = -1;
            int count = 0;
            int h = 4;
            int first = 0;

			string root = CGlobals.GetRootDirectory()+ @"\MenuLayouts\";

			for (int i=0;i<CMenuLayoutDatabase.GetInstance().GetNumEntries() ;i++)
			{
				CMenuLayout ml = CMenuLayoutDatabase.GetInstance().GetLayout(i);
                MenuSelectionThumbnail thumbnail = new MenuSelectionThumbnail(root + ml.GetLayoutImage(), i, UserSelectedLayout);
                first++;

                if (numberHorizontal < 0)
                {
                    // determine how many thumbnails can be displayed on one row
                    numberHorizontal = (int)(mMenuLayoutTemplatesPlanel.Width / thumbnail.Width);
                }
                // set the position for the thumbnail and add it to the panel's controls

                thumbnail.Left = 2 + count;
                thumbnail.Top = h;
                mMenuLayoutTemplatesPlanel.Controls.Add(thumbnail);
                count += thumbnail.Width + 5;

                if (first >= numberHorizontal)
                {
                    count = 0;
                    first = 0;
                    h += thumbnail.Height + 5;
                }

			}
			Log.Info("Done adding menu layouts to tab");
		}

	
		//*******************************************************************
		public void AddMenuTemplatesThumbnailsToTab()
		{
			int numberHorizontal = -1;
			int count = 0;
			int h = 4;
            int first = 0;

			string [] fileEntries = Directory.GetFiles(CGlobals.GetRootDirectory()+@"\MenuTemplates");
            CGlobals.mDefaultMenuTemplate = CGlobals.GetRootDirectory() + "\\menutemplates\\background105.pvsi";
            List<string> fileEntriesList = new List<string>(fileEntries);

			foreach(string fileName in fileEntriesList)
			{
				if (CGlobals.IsImageFilename(fileName) || CGlobals.IsVideoFilename(fileName))
				{
                    // ok if a pvsi file and we've also got the .jpg version, then ignore
                    if (fileName.EndsWith(".pvsi"))
                    {
                        string jpgversion = fileName.Substring(0, fileName.Length-5)+".jpg";
                        if (fileEntriesList.Contains(jpgversion) == true)
                        {
                            continue;
                        }

                        string pngversion = fileName.Substring(0, fileName.Length - 5) + ".png";
                        if (fileEntriesList.Contains(pngversion) == true)
                        {
                            continue;
                        }

                    }

					first++;

					MenuThumbnail thumbnail = new MenuThumbnail(fileName);

					if (numberHorizontal < 0)
					{
						// determine how many thumbnails can be displayed on one row
						numberHorizontal = (int) (mMenuTemplatesPanel.Width / thumbnail.Width);
					}
					// set the position for the thumbnail and add it to the panel's controls
					
					thumbnail.Left = 2 + count;
					thumbnail.Top = h;
					mMenuTemplatesPanel.Controls.Add(thumbnail);
				    count+=thumbnail.Width+5;

					if (first >= numberHorizontal)
					{
						count = 0;
						first=0;
						h+=thumbnail.Height+5 ;
					}
				}	
			}
			Log.Info("Done adding menu templates");
		}


		//*******************************************************************
		public void AddButtonStylesThumbnailsToTab()
		{
			int entries = CMenuButtonStyleDatabase.GetInstance().GetNumStyles();
            int numberHorizontal = -1;
            int count = 0;
            int h = 2;
            int first = 0;

			for (int i=0;i<entries;i++)
			{

				CMenuButtonStyle style = CMenuButtonStyleDatabase.GetInstance().GetStyle(i);

				CImage image = style.SlideshowButtonFrameImage;

                MenuStyleThumbnail thumbnail = new MenuStyleThumbnail(image.ImageFilename, i, this);

				// set the position for the thumbnail and add it to the panel's controls
                first++;

                if (numberHorizontal < 0)
                {
                    // determine how many thumbnails can be displayed on one row
                    numberHorizontal = (int)(mButtonStylePanel.Width / thumbnail.Width);
                }
                // set the position for the thumbnail and add it to the panel's controls

                thumbnail.Left = 2 + count;
                thumbnail.Top = h;
                mButtonStylePanel.Controls.Add(thumbnail);
                count += thumbnail.Width + 3;

                if (first >= numberHorizontal)
                {
                    count = 0;
                    first = 0;
                    h += thumbnail.Height + 3;
                }
			}
		}

		//*******************************************************************
		public void RebuildLabels()
		{
            mRebuildingLabels = true;
            try
            {
			    // update menu music
			    TextBox t =mMainWindow.GetMenuBackgroundMusicTextBox();
			    CMainMenu m = GetCurrentMenuInDecorationsWindow();
			    if (m!=null)
			    {
                    mDiskMenuNavigationManager.RebuildLabels(m);
                    mDiskHighlightManager.RebuildLabels(m);

				    string s = m.GetAudioFileName();

				    if (s=="")
				    {
					    t.Text="None";
				    }
				    else
				    {
					    s =System.IO.Path.GetFileName(s);
					    t.Text = s;
				    }
			    }
    		
			    t =mMainWindow.GetMenuBackgroundTextBox();
			    if (m==null) 
			    {
				    t.Text="";
				    MenuRepeatLengthTrackbar.Value = 30;
				    MenuRepeatDurationTextbox.Text = MenuRepeatLengthTrackbar.Value.ToString();
				    return ;
			    }

			    // if not in menutemplates directory then must be a user defined background

			    string root = CGlobals.GetRootDirectory()+@"\MenuTemplates";
			    root= root.ToLower();
                CImageSlide iss = m.BackgroundSlide as CImageSlide;

			    t.Text="";

			    string fn = iss.SourceFilename;
			    if (fn!=null)
			    {
				    fn = fn.ToLower();
				    if (fn.StartsWith(root)==false)
				    {
					    fn =System.IO.Path.GetFileName(fn);
					    t.Text = fn;
				    }
			    }

			    this.MenuRepeatLengthTrackbar.Value = (int) m.Length;
			    this.MenuRepeatDurationTextbox.Text = MenuRepeatLengthTrackbar.Value.ToString();
			    Form1.mMainForm.GetMenuThemePlayingThumbNailsCheckBox().Checked = m.PlayingThumbnails;
                Form1.mMainForm.GetZoomingMenuBackGroundCheckBox().Checked = m.ZoomingBackground;

			    Form1.mMainForm.UpdateMenuIdText();

                UpdateSlideshowListInternal(false);
            }
            finally
            {
                mRebuildingLabels = false;
            }
		}

		//*******************************************************************
		public void SetMenuMusic_Click(object sender, System.EventArgs e)
		{
			Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();

            if (mMusicOpenFileDialog == null)
            {
                mMusicOpenFileDialog = new OpenFileDialog();
                mMusicOpenFileDialog.Filter = CGlobals.GetTotalAudioFilter();
                mMusicOpenFileDialog.InitialDirectory = DefaultFolders.GetFolder("MyMusic"); ;
                mMusicOpenFileDialog.Title = "Open music";
            }

			if (mMusicOpenFileDialog.ShowDialog().Equals(DialogResult.OK))
			{
                if (IO.IsDriveOkToUse(mMusicOpenFileDialog.FileName) == false)
                {
                    return;
                }

				CMainMenu m = GetCurrentMenuInDecorationsWindow();
				m.SetMusicSlide(mMusicOpenFileDialog.FileName);
                mMusicOpenFileDialog.InitialDirectory = Path.GetDirectoryName(mMusicOpenFileDialog.FileName);  // rember current folder 

            }
			RebuildLabels();

			CGlobals.mCurrentProject.DeclareChange(true,"Menu Music Change");
		}

		//*******************************************************************
		public void SetMenuBackground(object sender, System.EventArgs e)
		{
			Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();
            if (mBackgroundOpenFileDialog == null)
            {
                mBackgroundOpenFileDialog = new OpenFileDialog();
                mBackgroundOpenFileDialog.Filter = CGlobals.GetTotalImageVideoFilter();
                mBackgroundOpenFileDialog.InitialDirectory = DefaultFolders.GetFolder("MyPictures");
                mBackgroundOpenFileDialog.Title = "Open image or video";
            }

			if (mBackgroundOpenFileDialog.ShowDialog().Equals(DialogResult.OK))
			{
                if (ManagedCore.IO.IsDriveOkToUse(mBackgroundOpenFileDialog.FileName) == false)
                {
                    return;
                }

				CMainMenu m =GetCurrentMenuInDecorationsWindow();
				m.SetBackground(mBackgroundOpenFileDialog.FileName);
                mMainWindow.GetDecorationManager().SetCurrentSlide(m.BackgroundSlide);
                mBackgroundOpenFileDialog.InitialDirectory = Path.GetDirectoryName(mBackgroundOpenFileDialog.FileName);  // rember current folder 
            }

            RebuildLabels();

			CGlobals.mCurrentProject.DeclareChange(true,"Menu Background Change");
		}



		//*******************************************************************
		public void RightSelectedSlideShow(object o, MouseEventArgs e, string slideshowid, CMainMenu menu)
		{
			mContextSlideshow = slideshowid;

            ContextMenuStrip cm = new ContextMenuStrip();
       
			if (menu==null)
			{
                CDebugLog.GetInstance().Error("Null menu on call to RightSelectedSlideShow");
				return ;
			}

			if (slideshowid=="")
			{
				CDebugLog.GetInstance().Error("Slideshow name not set on call to RightSelectedSlideShow");
				return ;
			}

			ArrayList slideshow_list = menu.GetSlideshowsSelectableFromMenu();

            ToolStripMenuItem upOrderItem = new ToolStripMenuItem();
            upOrderItem.Image = mMainWindow.GetGlobalMenuItemIconsImageList().Images[10];
            upOrderItem.Text = "Move up order";
            upOrderItem.Click += new System.EventHandler(this.OrderUp_Click);
            cm.Items.Add(upOrderItem);

            ToolStripMenuItem downOrderItem = new ToolStripMenuItem();
            downOrderItem.Image = mMainWindow.GetGlobalMenuItemIconsImageList().Images[9];
            downOrderItem.Text = "Move down order";
            downOrderItem.Click += new System.EventHandler(this.OrderDown_Click);
            cm.Items.Add(downOrderItem);

            bool addedMoveToMenuSeperator = false;

			ArrayList menu_list = CGlobals.mCurrentProject.MainMenu.GetSelfAndAllSubMenus();
	
			// move to another menu
			if (menu_list.Count>1)
			{
				int current_menu =1;

				foreach (CMainMenu mm in menu_list)
				{
					if (mm.ID != menu.ID && mm.GetNumSlideShowButtons() < CMainMenu.MaxSlideshowButtons)
					{
                        ToolStripMenuItem move_slideshow_item = new ToolStripMenuItem();
						move_slideshow_item.Text = "Move to menu "+current_menu;
						move_slideshow_item.Click+=new EventHandler(MoveToMenu_Click);

                        if (addedMoveToMenuSeperator==false)
                        {
                            cm.Items.Add(new ToolStripSeparator());
                            addedMoveToMenuSeperator = true;
                        }
						cm.Items.Add(move_slideshow_item);
					}
					current_menu++;
				}

                cm.Items.Add(new ToolStripSeparator());
			}


            ToolStripMenuItem delete_slideshow = new ToolStripMenuItem();
            delete_slideshow.Image = mMainWindow.GetGlobalMenuItemIconsImageList().Images[0];
			delete_slideshow.Text = "Delete slideshow";
			delete_slideshow.Click+=new EventHandler(DeleteSlideshow_Click);
			cm.Items.Add(delete_slideshow);

			// if only 1 slideshow in whole project then can't delete
			if (menu_list.Count <2 &&
				slideshow_list.Count<2)
			{
				delete_slideshow.Enabled=false;
			}


			if (slideshow_list.Count <2)
			{
                upOrderItem.Enabled = false;
                downOrderItem.Enabled = false;
			}
			else
			{
				// if first in list
				if ((slideshow_list[0] as CSlideShow).Name == slideshowid)
				{
                    upOrderItem.Enabled = false;
				}

				// if last in list
				if ((slideshow_list[slideshow_list.Count-1] as CSlideShow).Name == slideshowid)
				{
                    downOrderItem.Enabled = false;
				}
			}

			Point p = new Point(e.X,e.Y);
			cm.Show((Control)o,p);

		}

		//*******************************************************************
		public void OrderUp_Click(object o, EventArgs e)
		{
			int i=1;
			i++;

			CSlideShow s = CGlobals.mCurrentProject.GetSlideshow(this.mContextSlideshow);
			if (s==null)
			{
				CDebugLog.GetInstance().Error("Unknown slideshow '"+mContextSlideshow+ "' on call to OrderUp_Click");
				return ;
			}

			CMainMenu m = CGlobals.mCurrentProject.GetMenuWhichContainsSlideshow(s);

			if (m==null)
			{
				CDebugLog.GetInstance().Error("No menu contains links to slideshow: "+mContextSlideshow);
				return ;
			}

			int index =0;
			int prev =-1;
            foreach (CDecoration d in m.BackgroundSlide.Decorations)
			{
				CMenuSlideshowButton msb = d as CMenuSlideshowButton;
				if (msb!=null)
				{
					if (msb.GetInnerImageStringId() != mContextSlideshow)
					{
						prev = index ;
					}
					else
					{
						if (prev==-1)
						{
							CDebugLog.GetInstance().Warning("Can not move slideshow up in the order, already at the top");
							return ;
						}

                        CDecoration t = m.BackgroundSlide.Decorations[prev] as CDecoration;
                        m.BackgroundSlide.Decorations[prev] = m.BackgroundSlide.Decorations[index];
                        m.BackgroundSlide.Decorations[index] = t;

						m.RebuildMenu();
						this.mMainWindow.GetDecorationManager().RePaint();
						CGlobals.mCurrentProject.DeclareChange(true,"Slideshow Move");
						return ;
					}
				}
				index++;
			}
		}

		//*******************************************************************
		public void OrderDown_Click(object o, EventArgs e)
		{
			CSlideShow s = CGlobals.mCurrentProject.GetSlideshow(this.mContextSlideshow);
			if (s==null)
			{
				CDebugLog.GetInstance().Error("Unknown slideshow '"+mContextSlideshow + "' on call to OrderDown_Click");
				return ;
			}

			CMainMenu m = CGlobals.mCurrentProject.GetMenuWhichContainsSlideshow(s);

			if (m==null)
			{
				CDebugLog.GetInstance().Error("No menu contains links to slideshow:"+mContextSlideshow);
				return ;
			}

			int index =0;
			int current =-1;
            foreach (CDecoration d in m.BackgroundSlide.Decorations)
			{
				CMenuSlideshowButton msb = d as CMenuSlideshowButton;
				if (msb!=null)
				{
					if (msb.GetInnerImageStringId() == mContextSlideshow)
					{
						current = index ;
					}
					else
					{
						if (current==-1)
						{
							index++;
							continue;
						}

                        CDecoration t = m.BackgroundSlide.Decorations[current] as CDecoration;
                        m.BackgroundSlide.Decorations[current] = m.BackgroundSlide.Decorations[index];
                        m.BackgroundSlide.Decorations[index] = t;

						m.RebuildMenu();
						this.mMainWindow.GetDecorationManager().RePaint();
						CGlobals.mCurrentProject.DeclareChange(true,"Slideshow Move");
						return ;
					}
				}
				index++;
			}

			CDebugLog.GetInstance().Warning("Can not move slideshow down in the order, already at the bottom");
		}
	

		//*******************************************************************
		public void MoveToMenu_Click(object o, EventArgs e)
		{	
			CSlideShow s = CGlobals.mCurrentProject.GetSlideshow(this.mContextSlideshow);
			if (s==null)
			{
				CDebugLog.GetInstance().Error("Unknown slideshow '"+mContextSlideshow+"' on call to MoveToMenu_Click");
				return ;
			}

			CMainMenu m = CGlobals.mCurrentProject.GetMenuWhichContainsSlideshow(s);

			if (m==null)
			{
				CDebugLog.GetInstance().Error("No menu contains links to slideshow:"+mContextSlideshow);
				return ;
			}

            ToolStripMenuItem mi = (ToolStripMenuItem)o;
			string t = mi.Text.Remove(0,13);
            int index = 1;
            try
            {
                index = int.Parse(t);
            }
            catch
            {
                CDebugLog.GetInstance().Error("Could not pass text to determine menu '" + t + "'");
            }

			CMenuSlideshowButton msb = m.GetMenuSlideshowButton(CGlobals.mCurrentProject.GetSlideshow(this.mContextSlideshow));

			if (msb==null)
			{
				CDebugLog.GetInstance().Error("Could not get MenuSlideshowButton in MoveToMenu_Click");
				return ;
			}

			ArrayList all_menu = CGlobals.mCurrentProject.MainMenu.GetSelfAndAllSubMenus();

			CMainMenu newMenu = (CMainMenu) all_menu[index-1];

			bool removed_old_menu=false;

			CGlobals.mCurrentProject.MoveSlideshowToAnotherMenu(s,m,newMenu, out removed_old_menu);

			// ouch 
			if (removed_old_menu==true)
			{
				Form1.mMainForm.GoToMainMenu();
			}

			this.mMainWindow.GetDecorationManager().SelectFirstSlideshowInCurrentMenu();
			this.mMainWindow.GetDecorationManager().RePaint();
			CGlobals.mCurrentProject.DeclareChange(true,"Slideshow Menu Move");

		}

		//*******************************************************************
		private void DeleteSlideshow(CSlideShow s)
		{
			if (s.mSlides.Count>0)
			{
                DialogResult res = UserMessage.Show("Are you sure you want to delete this slideshow?", "Delete slideshow",
					MessageBoxButtons.YesNo,MessageBoxIcon.Question);
				if (res == DialogResult.No)
				{
					return ;
				}
			}

			bool removed_current_menu = false;
			CGlobals.mCurrentProject.RemoveSlideshow(s, out removed_current_menu);

			// ouch 
			if (removed_current_menu==true)
			{
				Form1.mMainForm.GoToMainMenu();
			}
			
			this.mMainWindow.GetDecorationManager().SelectFirstSlideshowInCurrentMenu();
			this.mMainWindow.GetDecorationManager().RePaint();
			CGlobals.mCurrentProject.DeclareChange(true,"Delete Slideshow");
		}

		//*******************************************************************
		// menu button
		public void DeleteSlideshow_Click2(object o, EventArgs e)
		{
			Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();
			// must be the one currently selected
			CSlideShow s = Form1.mMainForm.GetDecorationManager().CurrentSelectedSlideShow();

			if (s!=null)
			{
				DeleteSlideshow(s);
			}
		}


		//*******************************************************************
		// from context
		public void DeleteSlideshow_Click(object o, EventArgs e)
		{
			CSlideShow s = CGlobals.mCurrentProject.GetSlideshow(this.mContextSlideshow);
			if (s==null)
			{
				CDebugLog.GetInstance().Error("Unknown slideshow '"+mContextSlideshow+"' on call to DeleteSlideshow_Click");
				return ;
			}

			DeleteSlideshow(s);
		}

		//*******************************************************************
		public CMainMenu GetCurrentMenuInDecorationsWindow()
		{
			CSlide s = this.mMainWindow.GetDecorationManager().mCurrentSlide;
			if (s==null) return null;

			CMainMenu m = CGlobals.mCurrentProject.GetMenuWhichContainsSlide(s);

			return m ;
		}

		//*******************************************************************
		public void UserSelectedStyle(int id)
		{
			Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();
			CMainMenu m = GetCurrentMenuInDecorationsWindow();
			if (m==null) return ;

			if (m.ButtonStyleID==id) return ;

			m.SetButtonStyle(id);
			this.mMainWindow.GetDecorationManager().RePaint();

			CGlobals.mCurrentProject.DeclareChange(true,"Menu Style Change");
		}

		//*******************************************************************
		public void UserChangedPlayingThumbnailsCheckBox(object o, System.EventArgs e)
		{
			CheckBox cb = Form1.mMainForm.GetMenuThemePlayingThumbNailsCheckBox();
			Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();
			CMainMenu m = GetCurrentMenuInDecorationsWindow();
			if (m==null) return ;

			if (m.PlayingThumbnails==cb.Checked) return ;

			m.PlayingThumbnails = cb.Checked;
			this.mMainWindow.GetDecorationManager().RePaint();
			CGlobals.mCurrentProject.DeclareChange(true,"Menu Style Change");
		}

        //*******************************************************************
        private void UserChangedZoominBackgroundCheckBox(object o, System.EventArgs e)
        {
            CheckBox cb = Form1.mMainForm.GetZoomingMenuBackGroundCheckBox();
            Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();
            CMainMenu m = GetCurrentMenuInDecorationsWindow();
            if (m == null) return;
            if (m.ZoomingBackground == cb.Checked) return;

            m.ZoomingBackground = cb.Checked;
 
            this.mMainWindow.GetDecorationManager().RePaint();
            CGlobals.mCurrentProject.DeclareChange(true, "Menu Style Change");
            
        }

		//*******************************************************************
		public void UserSelectedLayout(int id)
		{
			Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();
			CMainMenu m = GetCurrentMenuInDecorationsWindow();
			if (m==null) return ;

			// we may have added stuff and want to go back to this style else it will do redaunt code. whatever...
			//if (m.LayoutStyleID ==id) return 
			
			if (m.CanUseMenuLayout(id)==true)
			{
				m.SetLayout(id);
				this.mMainWindow.GetDecorationManager().RePaint();

				CGlobals.mCurrentProject.DeclareChange(true,"Menu Layout Change");
			}
		}

		//*******************************************************************
		public void DoHideUnHide()
		{
            this.AddNextMenuButton.Enabled = true;
            this.AddSlideshowButton.Enabled = true;
			if (CGlobals.mCurrentProject.GetAllProjectSlideshows(false).Count<2)
			{
				this.RemoveSlideshowButton.Enabled=false;
                return;
			}
			else
			{
                this.RemoveSlideshowButton.Enabled = true;
			}

            
			CSlide s = this.mMainWindow.GetDecorationManager().mCurrentSlide;
			if (s!=null)
			{
				CMainMenu mm = this.mMainWindow.mCurrentProject.GetMenuWhichContainsSlide(s);
          

				if (mm!=null)
				{
					if (mm.SubMenus.Count==0)
					{
						this.AddNextMenuButton.Enabled=true;
					}
					else
					{
						this.AddNextMenuButton.Enabled=false;
					}

                    if (mm.GetSlideShowButtons().Count >= CMainMenu.MaxSlideshowButtons)
                    {
                        this.AddSlideshowButton.Enabled = false;
                    }
				}
			}
		}

		//*******************************************************************
        private delegate void ProjectHasChangedDelegate(bool store_momento, string ss);
		public void ProjectHasChanged(bool store_momento, string ss)
		{
            if (Form1.mMainForm.InvokeRequired == true)
            {
                Form1.mMainForm.BeginInvoke(new ProjectHasChangedDelegate(ProjectHasChanged),
                                            new object[] { store_momento, ss});
                return;
            }

			if (CGlobals.mCurrentProject==null) return;
			DoHideUnHide();
		}
	}
}
