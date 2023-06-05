using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;
using System.Drawing;

namespace dvdslideshowfontend
{
    // **********************************************************************************************
    // This class manages the highlight (subtitle) styles for menus.  
    public class CDiskMenuHighlightManager
    {
        private ComboBox mMenuHighlightSlideshowButtonStyleCombo;
        private ComboBox mMenuHighlightNavigationButtonStyleCombo;
        private ComboBox mMenuHighlightPlayButtonStyleCombo;
        private Button mMenuHighlightColorButton;
        private Button mMenuHighlightResetButton;
        private CDiskMenuManager mParentManager;
        private ColorDialog mColorPicker = new ColorDialog();
        private bool mRebuildingLabels = false;

        // ********************************************************************************************************
        public CDiskMenuHighlightManager(CDiskMenuManager parentManager)
        {
            mParentManager = parentManager;
            Form1 mainForm = mParentManager.MainWindow;

            mColorPicker.AnyColor = true;

            mMenuHighlightSlideshowButtonStyleCombo = mainForm.GetMenuHighlightSlideshowButtonStyleCombo();
            mMenuHighlightNavigationButtonStyleCombo = mainForm.GetMenuHighlightNavigationButtonStyleCombo();
            mMenuHighlightPlayButtonStyleCombo = mainForm.GetMenuHighlightPlayButtonStyleCombo();
            mMenuHighlightColorButton = mainForm.GetMenuHighlightColorButton();
            mMenuHighlightResetButton = mainForm.GetMenuHighlightResetButton();

            mMenuHighlightSlideshowButtonStyleCombo.SelectedIndex =2;
            mMenuHighlightNavigationButtonStyleCombo.SelectedIndex = 2;
            mMenuHighlightPlayButtonStyleCombo.SelectedIndex = 2;

            mMenuHighlightSlideshowButtonStyleCombo.SelectedValueChanged += new EventHandler(mMenuHighlightSlideshowButtonStyleCombo_SelectedValueChanged);
            mMenuHighlightNavigationButtonStyleCombo.SelectedValueChanged += new EventHandler(mMenuHighlightNavigationButtonStyleCombo_SelectedValueChanged);
            mMenuHighlightPlayButtonStyleCombo.SelectedValueChanged += new EventHandler(mMenuHighlightPlayButtonStyleCombo_SelectedValueChanged);
            mMenuHighlightColorButton.Click += new EventHandler(mMenuHighlightColorButton_Click);
            mMenuHighlightResetButton.Click += new EventHandler(mMenuHighlightResetButton_Click);
        }

        // ********************************************************************************************************
        void mMenuHighlightResetButton_Click(object sender, EventArgs e)
        {
            CMainMenu menu = mParentManager.GetCurrentMenuInDecorationsWindow();
            if (menu == null)
            {
                return;
            }

            menu.SubPictureStyle.SubPictureButtonStyle = CMenuSubPictureRenderMethod.HighlightImage;
            menu.SubPictureStyle.SubPictureMenuLinkStyle = CMenuSubPictureRenderMethod.HighlightImage;
            menu.SubPictureStyle.SubPicturePlayMethodsStyle = CMenuSubPictureRenderMethod.Underline;
            menu.SubPictureStyle.SubPictureColor = CMenuSubPictureStyle.DefaultSubPictureColor;
            RebuildLabels(menu);
            CGlobals.mCurrentProject.DeclareChange("Reset all menu highlight settings");
            mParentManager.MainWindow.GetDecorationManager().RePaint();
        }

        // ********************************************************************************************************
        private void mMenuHighlightSlideshowButtonStyleCombo_SelectedValueChanged(object sender, EventArgs e)
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

            CMenuSubPictureRenderMethod oldStyle = menu.SubPictureStyle.SubPictureButtonStyle;

            if (mMenuHighlightSlideshowButtonStyleCombo.SelectedIndex == 0)
            {
                menu.SubPictureStyle.SubPictureButtonStyle = CMenuSubPictureRenderMethod.OutlineRectangle;
            }
            else if (mMenuHighlightSlideshowButtonStyleCombo.SelectedIndex == 1)
            {
                menu.SubPictureStyle.SubPictureButtonStyle = CMenuSubPictureRenderMethod.CoveringRectangle;
            }
            else if (mMenuHighlightSlideshowButtonStyleCombo.SelectedIndex == 2)
            {
                menu.SubPictureStyle.SubPictureButtonStyle = CMenuSubPictureRenderMethod.HighlightImage;
            }     
            else if (mMenuHighlightSlideshowButtonStyleCombo.SelectedIndex == 3)
            {
                menu.SubPictureStyle.SubPictureButtonStyle = CMenuSubPictureRenderMethod.CircleOnLeft;
            }
            else if (mMenuHighlightSlideshowButtonStyleCombo.SelectedIndex == 4)
            {
                menu.SubPictureStyle.SubPictureButtonStyle = CMenuSubPictureRenderMethod.CircleOnRight;
            }
            else if (mMenuHighlightSlideshowButtonStyleCombo.SelectedIndex == 5)
            {
                menu.SubPictureStyle.SubPictureButtonStyle = CMenuSubPictureRenderMethod.SquareOnLeft;
            }
            else if (mMenuHighlightSlideshowButtonStyleCombo.SelectedIndex == 6)
            {
                menu.SubPictureStyle.SubPictureButtonStyle = CMenuSubPictureRenderMethod.SquareOnRight;
            }

            if (oldStyle != menu.SubPictureStyle.SubPictureButtonStyle)
            {
                CGlobals.mCurrentProject.DeclareChange("Changed slidshow highlight style");
            }

            mParentManager.MainWindow.GetDecorationManager().RePaint();
        }

        // ********************************************************************************************************
        private void mMenuHighlightNavigationButtonStyleCombo_SelectedValueChanged(object sender, EventArgs e)
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

            CMenuSubPictureRenderMethod oldStyle = menu.SubPictureStyle.SubPictureMenuLinkStyle;

            if (mMenuHighlightNavigationButtonStyleCombo.SelectedIndex == 0)
            {
                menu.SubPictureStyle.SubPictureMenuLinkStyle = CMenuSubPictureRenderMethod.OutlineRectangle;
            }
            else if (mMenuHighlightNavigationButtonStyleCombo.SelectedIndex == 1)
            {
                menu.SubPictureStyle.SubPictureMenuLinkStyle = CMenuSubPictureRenderMethod.CoveringRectangle;
            }
            else if (mMenuHighlightNavigationButtonStyleCombo.SelectedIndex == 2)
            {
                menu.SubPictureStyle.SubPictureMenuLinkStyle = CMenuSubPictureRenderMethod.HighlightImage;
            }
            else if (mMenuHighlightNavigationButtonStyleCombo.SelectedIndex == 3)
            {
                menu.SubPictureStyle.SubPictureMenuLinkStyle = CMenuSubPictureRenderMethod.CircleOnLeft;
            }
            else if (mMenuHighlightNavigationButtonStyleCombo.SelectedIndex == 4)
            {
                menu.SubPictureStyle.SubPictureMenuLinkStyle = CMenuSubPictureRenderMethod.CircleOnRight;
            }
            else if (mMenuHighlightNavigationButtonStyleCombo.SelectedIndex == 5)
            {
                menu.SubPictureStyle.SubPictureMenuLinkStyle = CMenuSubPictureRenderMethod.SquareOnLeft;
            }
            else if (mMenuHighlightNavigationButtonStyleCombo.SelectedIndex == 6)
            {
                menu.SubPictureStyle.SubPictureMenuLinkStyle = CMenuSubPictureRenderMethod.SquareOnRight;
            }

            if (oldStyle != menu.SubPictureStyle.SubPictureMenuLinkStyle)
            {
                CGlobals.mCurrentProject.DeclareChange("Changed navigation highlight style");
            }

            mParentManager.MainWindow.GetDecorationManager().RePaint();
        }

        // ********************************************************************************************************
        private void mMenuHighlightPlayButtonStyleCombo_SelectedValueChanged(object sender, EventArgs e)
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

            CMenuSubPictureRenderMethod oldStyle = menu.SubPictureStyle.SubPicturePlayMethodsStyle;

            if (mMenuHighlightPlayButtonStyleCombo.SelectedIndex == 0)
            {
                menu.SubPictureStyle.SubPicturePlayMethodsStyle = CMenuSubPictureRenderMethod.OutlineRectangle;
            }
            else if (mMenuHighlightPlayButtonStyleCombo.SelectedIndex == 1)
            {
                menu.SubPictureStyle.SubPicturePlayMethodsStyle = CMenuSubPictureRenderMethod.CoveringRectangle;
            }
            else if (mMenuHighlightPlayButtonStyleCombo.SelectedIndex == 2)
            {
                menu.SubPictureStyle.SubPicturePlayMethodsStyle = CMenuSubPictureRenderMethod.Underline;
            }
            else if (mMenuHighlightPlayButtonStyleCombo.SelectedIndex == 3)
            {
                menu.SubPictureStyle.SubPicturePlayMethodsStyle = CMenuSubPictureRenderMethod.CircleOnLeft;
            }
            else if (mMenuHighlightPlayButtonStyleCombo.SelectedIndex == 4)
            {
                menu.SubPictureStyle.SubPicturePlayMethodsStyle = CMenuSubPictureRenderMethod.CircleOnRight;
            }
            else if (mMenuHighlightPlayButtonStyleCombo.SelectedIndex == 5)
            {
                menu.SubPictureStyle.SubPicturePlayMethodsStyle = CMenuSubPictureRenderMethod.SquareOnLeft;
            }
            else if (mMenuHighlightPlayButtonStyleCombo.SelectedIndex == 6)
            {
                menu.SubPictureStyle.SubPicturePlayMethodsStyle = CMenuSubPictureRenderMethod.SquareOnRight;
            }

            if (oldStyle != menu.SubPictureStyle.SubPicturePlayMethodsStyle)
            {
                CGlobals.mCurrentProject.DeclareChange("Changed play buttons highlight style");
            }
        
            mParentManager.MainWindow.GetDecorationManager().RePaint();
        }


        // ********************************************************************************************************
        private void mMenuHighlightColorButton_Click(object sender, EventArgs e)
        {
            CMainMenu menu = mParentManager.GetCurrentMenuInDecorationsWindow();
            if (menu == null)
            {
                return;
            }

            DialogResult result = mColorPicker.ShowDialog();
            if (result != DialogResult.Cancel)
            {
                Color newColor = Color.FromArgb(128, mColorPicker.Color.R, mColorPicker.Color.G, mColorPicker.Color.B);

                if (menu.SubPictureStyle.SubPictureColor != newColor)
                {
                    mMenuHighlightColorButton.BackColor = newColor;
                    menu.SubPictureStyle.SubPictureColor = newColor;
                    CGlobals.mCurrentProject.DeclareChange("Changed highlight color");
                    mParentManager.MainWindow.GetDecorationManager().RePaint();
                }
            }
        }

        // ********************************************************************************************************
        public void RebuildLabels(CMainMenu menu)
        {
            mRebuildingLabels = true;
            try
            {
                mMenuHighlightColorButton.BackColor = menu.SubPictureStyle.SubPictureColor;

                //
                // Slideshow button style
                //
                CMenuSubPictureRenderMethod renderMethod = menu.SubPictureStyle.SubPictureButtonStyle;
                if (renderMethod == CMenuSubPictureRenderMethod.OutlineRectangle)
                {
                    mMenuHighlightSlideshowButtonStyleCombo.SelectedIndex = 0;
                }
                else if (renderMethod == CMenuSubPictureRenderMethod.CoveringRectangle)
                {
                    mMenuHighlightSlideshowButtonStyleCombo.SelectedIndex = 1;
                }
                else if (renderMethod == CMenuSubPictureRenderMethod.HighlightImage)
                {
                    mMenuHighlightSlideshowButtonStyleCombo.SelectedIndex = 2;
                }           
                else if (renderMethod == CMenuSubPictureRenderMethod.CircleOnLeft)
                {
                    mMenuHighlightSlideshowButtonStyleCombo.SelectedIndex = 3;
                }
                else if (renderMethod == CMenuSubPictureRenderMethod.CircleOnRight)
                {
                    mMenuHighlightSlideshowButtonStyleCombo.SelectedIndex = 4;
                }
                else if (renderMethod == CMenuSubPictureRenderMethod.SquareOnLeft)
                {
                    mMenuHighlightSlideshowButtonStyleCombo.SelectedIndex = 5;
                }
                else if (renderMethod == CMenuSubPictureRenderMethod.SquareOnRight)
                {
                    mMenuHighlightSlideshowButtonStyleCombo.SelectedIndex = 6;
                }

                //
                // Link button Style
                //
                renderMethod = menu.SubPictureStyle.SubPictureMenuLinkStyle;
                if (renderMethod == CMenuSubPictureRenderMethod.OutlineRectangle)
                {
                    mMenuHighlightNavigationButtonStyleCombo.SelectedIndex = 0;
                }
                else if (renderMethod == CMenuSubPictureRenderMethod.CoveringRectangle)
                {
                    mMenuHighlightNavigationButtonStyleCombo.SelectedIndex = 1;
                }
                else if (renderMethod == CMenuSubPictureRenderMethod.HighlightImage)
                {
                    mMenuHighlightNavigationButtonStyleCombo.SelectedIndex = 2;
                }
                else if (renderMethod == CMenuSubPictureRenderMethod.CircleOnLeft)
                {
                    mMenuHighlightNavigationButtonStyleCombo.SelectedIndex = 3;
                }
                else if (renderMethod == CMenuSubPictureRenderMethod.CircleOnRight)
                {
                    mMenuHighlightNavigationButtonStyleCombo.SelectedIndex = 4;
                }
                else if (renderMethod == CMenuSubPictureRenderMethod.SquareOnLeft)
                {
                    mMenuHighlightNavigationButtonStyleCombo.SelectedIndex = 5;
                }
                else if (renderMethod == CMenuSubPictureRenderMethod.SquareOnRight)
                {
                    mMenuHighlightNavigationButtonStyleCombo.SelectedIndex = 6;
                }

                //
                // Play button style (does not have highlight as this is text
                //
                renderMethod = menu.SubPictureStyle.SubPicturePlayMethodsStyle;
                if (renderMethod == CMenuSubPictureRenderMethod.OutlineRectangle)
                {
                    mMenuHighlightPlayButtonStyleCombo.SelectedIndex = 0;
                }
                else if (renderMethod == CMenuSubPictureRenderMethod.CoveringRectangle)
                {
                    mMenuHighlightPlayButtonStyleCombo.SelectedIndex = 1;
                }
                else if (renderMethod == CMenuSubPictureRenderMethod.Underline)
                {
                    mMenuHighlightPlayButtonStyleCombo.SelectedIndex = 2;
                }
                else if (renderMethod == CMenuSubPictureRenderMethod.CircleOnLeft)
                {
                    mMenuHighlightPlayButtonStyleCombo.SelectedIndex = 3;
                }
                else if (renderMethod == CMenuSubPictureRenderMethod.CircleOnRight)
                {
                    mMenuHighlightPlayButtonStyleCombo.SelectedIndex = 4;
                }
                else if (renderMethod == CMenuSubPictureRenderMethod.SquareOnLeft)
                {
                    mMenuHighlightPlayButtonStyleCombo.SelectedIndex = 5;
                }
                else if (renderMethod == CMenuSubPictureRenderMethod.SquareOnRight)
                {
                    mMenuHighlightPlayButtonStyleCombo.SelectedIndex = 6;
                }
            }
            finally
            {
                mRebuildingLabels = false;
            }
        }
    }
}
