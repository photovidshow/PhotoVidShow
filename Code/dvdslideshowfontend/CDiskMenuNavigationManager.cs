using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using DVDSlideshow;
using ManagedCore;

namespace dvdslideshowfontend
{
    // **********************************************************************************************
    // This class manages the navigation tab page for menus.  This includes changing of the navigation
    // button images as well as the 'play all' toggles.
    public class CDiskMenuNavigationManager
    {
        private Panel mNavigationButtonsPanel;
        private CheckBox mPlayAllCheckBox;
        private CheckBox mPlayAllLoopedCheckBox;
        private CDiskMenuManager mParentManager;
        private bool mRebuildingLabels = false;

        // **********************************************************************************************
        public CDiskMenuNavigationManager(CDiskMenuManager parentManager)
        {
            Form1 mainForm = parentManager.MainWindow;

            mParentManager = parentManager;
            mNavigationButtonsPanel = mainForm.GetMenuNavigationButtonsPanel();
            mPlayAllCheckBox = mainForm.GetMenuNavigationPlayAllButton();
            mPlayAllLoopedCheckBox = mainForm.GetMenuNavigationPlayAllLoopedButton();

            mPlayAllCheckBox.CheckedChanged += new EventHandler(mPlayAllCheckBox_CheckedChanged);
            mPlayAllLoopedCheckBox.CheckedChanged += new EventHandler(mPlayAllLoopedCheckBox_CheckedChanged);

            AddMenuNavigationButtonsThumbnailsToTab();
        }

        // **********************************************************************************************  
        private List<CDecoration> GetAllPlayAllButtons(CMainMenu menu)
        {
            CImageSlide slide = menu.BackgroundSlide;
            List<CDecoration> items = new List<CDecoration>();
            foreach (CDecoration dec in slide.GetAllAndSubDecorations())
            {
                CMenuPlayAllButton mpab = dec as CMenuPlayAllButton;
                if (mpab != null)
                {
                    items.Add(mpab);
                }
            }
            return items;
        }

        // **********************************************************************************************  
        private void mPlayAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mRebuildingLabels == true)
            {
                return;
            }

            CMainMenu menu = mParentManager.GetCurrentMenuInDecorationsWindow();
            if (menu==null)
            {
                return;
            }

            CImageSlide slide = menu.BackgroundSlide;
            List<CDecoration> toRemove = GetAllPlayAllButtons(menu);

            foreach (CDecoration remove in toRemove)
            {
                slide.RemoveDecoration(remove);
            }

            if (mPlayAllCheckBox.Checked == true)
            {
                CMenuLayout layout = CMenuLayoutDatabase.GetInstance().GetLayout(menu.Layout);
                CTextStyle textStyle = layout.GetHeaderTextSpec().Style.Clone();
                textStyle.Format.Alignment = System.Drawing.StringAlignment.Center;

                CTextDecoration td = new CTextDecoration("Play all", new RectangleF(0.0f, 0.0f, 0.0f, 0.0f), 0, 18);
                RectangleF coverage = layout.GetHeaderTextSpec().ReCalcMenuTextCoverageArea(td, td.TextStyle.FontSize);
                float x = 0.3f - (coverage.Width / 2);
                float y = 0.85f;
                RectangleF coverage2 = new RectangleF(x, y, coverage.Width, coverage.Height);
                CMenuPlayAllButton mpab = new CMenuPlayAllButton(coverage2, textStyle, td.mText);

                slide.AddDecoration(mpab);
                CGlobals.mCurrentProject.DeclareChange("Added play all button");
            }
            else
            {
                CGlobals.mCurrentProject.DeclareChange("Remmoved play all button");
            }

            mParentManager.MainWindow.GetDecorationManager().RePaint();

           

        }

        // **********************************************************************************************  
        private List<CDecoration> GetAllPlayAllLoopedButtons(CMainMenu menu)
        {
            CImageSlide slide = menu.BackgroundSlide;
            List<CDecoration> items = new List<CDecoration>();
            foreach (CDecoration dec in slide.GetAllAndSubDecorations())
            {
                CMenuPlayAllLoopedButton mpabl = dec as CMenuPlayAllLoopedButton;
                if (mpabl != null)
                {
                    items.Add(mpabl);
                }
            }
            return items;
        }

        // *********************************************************************************************
        private void mPlayAllLoopedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mRebuildingLabels == true)
            {
                return;
            }

            CMainMenu menu = mParentManager.GetCurrentMenuInDecorationsWindow();
            if (menu == null)
            {
                return;
            }

            CImageSlide slide = menu.BackgroundSlide;
            List<CDecoration> toRemove = GetAllPlayAllLoopedButtons(menu);

            foreach (CDecoration remove in toRemove)
            {
                slide.RemoveDecoration(remove);
            }

            if (this.mPlayAllLoopedCheckBox.Checked == true)
            {
                CMenuLayout layout = CMenuLayoutDatabase.GetInstance().GetLayout(menu.Layout);
                CTextStyle textStyle = layout.GetHeaderTextSpec().Style.Clone();
                textStyle.Format.Alignment = System.Drawing.StringAlignment.Center;

                CTextDecoration td = new CTextDecoration("Play all looped", new RectangleF(0.0f, 0.0f, 0.0f, 0.0f), 0, 18);
                RectangleF coverage = layout.GetHeaderTextSpec().ReCalcMenuTextCoverageArea(td, td.TextStyle.FontSize);
                float x = 0.63f - (coverage.Width / 2);
                float y = 0.85f;
                RectangleF coverage2 = new RectangleF(x, y, coverage.Width, coverage.Height);
                CMenuPlayAllLoopedButton mpabl = new CMenuPlayAllLoopedButton(coverage2, textStyle, td.mText);

                slide.AddDecoration(mpabl);
                CGlobals.mCurrentProject.DeclareChange("Added play all looped button");
            }
            else
            {
                CGlobals.mCurrentProject.DeclareChange("Removed play all looped button");
            }

            mParentManager.MainWindow.GetDecorationManager().RePaint();
        }

        // ********************************************************************************************************************
        public void RebuildLabels(CMainMenu menu)
        {
            mRebuildingLabels = true;
            try
            {
                List<CDecoration> playAllButtons = GetAllPlayAllButtons(menu);
                if (playAllButtons.Count > 0)
                {
                    mPlayAllCheckBox.Checked = true;
                }
                else
                {
                    mPlayAllCheckBox.Checked = false;
                }

                List<CDecoration> playAllLoopedButtons = GetAllPlayAllLoopedButtons(menu);

                if (playAllLoopedButtons.Count > 0)
                {
                    mPlayAllLoopedCheckBox.Checked = true;
                }
                else
                {
                    mPlayAllLoopedCheckBox.Checked = false;
                }
            }
            finally
            {
                mRebuildingLabels = false;
            }
        }

        //*******************************************************************
        public void AddMenuNavigationButtonsThumbnailsToTab()
        {
            int count = 0;
            int id=0;

            string root = CGlobals.GetRootDirectory();
           
		    for (int i = 1; i < 100; i++)
            {
                string frame_previous = root + "\\MenuButtonStyles\\previous" + i + ".png";
                string frame_next = root + "\\MenuButtonStyles\\next" + i + ".png";

                if (System.IO.File.Exists(frame_previous) && System.IO.File.Exists(frame_next))
                {
                    MenuSelectionThumbnail thumbnail = new MenuSelectionThumbnail(frame_next, id++, UserSelectedNavigatiopButton);

                    thumbnail.Left = 2 + count;
                    if (id % 2 == 1)
                    {
                        thumbnail.Top = 2;
                    }
                    else
                    {
                        thumbnail.Top = 4 + thumbnail.Height;
                        count += thumbnail.Width + 3;
                    }

                    mNavigationButtonsPanel.Controls.Add(thumbnail);
                }
            }
        }

        //*******************************************************************
        public void UserSelectedNavigatiopButton(int id)
        {
            CMainMenu menu = mParentManager.GetCurrentMenuInDecorationsWindow();
            if (menu==null)
            {
                return;
            }

            if (menu.LinkStyleID != id)
            {
                menu.SetLinkStyle(id);

                CGlobals.mCurrentProject.DeclareChange("Changed navigation style");

                mParentManager.MainWindow.GetDecorationManager().RePaint();
            }
        }

        //*******************************************************************
        public void DisablePlayAllOptions()
        {
            mPlayAllCheckBox.Enabled = false;
            mPlayAllLoopedCheckBox.Enabled = false;

        }

        //*******************************************************************
        public void EnablePlayAllOptions()
        {
            mPlayAllCheckBox.Enabled = true;
            mPlayAllLoopedCheckBox.Enabled = true;
        }
    }
}
