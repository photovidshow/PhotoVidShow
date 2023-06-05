using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;
using DVDSlideshow.GraphicsEng;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;

namespace CustomButton
{
    public partial class PreviewMenuForm : Form
    {
        private CMainMenu mForMenu;
        private MiniPreviewController mPreviewController = null;
        private CMenuSubPictureRenderer mRenderer = new CMenuSubPictureRenderer();
        private CMenuButton mRenderSubititle = null;
        private CImage mSubtitleCompleteImage;
       
        //******************************************************************************************
        public PreviewMenuForm(CMainMenu menu)
        {
            mForMenu = menu;
            if (menu == null)
            {
                return;
            }
        
            CImageSlide imageSlide = mForMenu.BackgroundSlide;
            imageSlide.SetLengthWithoutUpdate(mForMenu.Length); // so preview works ok
        
            InitializeComponent();

            if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV4_3)
            {
                int oldwidth = mPreviewPictureBox.Width;

                mPreviewPictureBox.Width = (int)(((float)mPreviewPictureBox.Height) * 1.33333f);

                this.ClientSize = new Size(this.ClientSize.Width - (oldwidth - mPreviewPictureBox.Width), this.ClientSize.Height);
            }

            this.SuspendLayout();

            // This tells the menu drawer that the inner thumbnail images need re-drawing
            CGlobals.MainMenuNeedsReRender = true;

            //
            // Create subtitle master image
            //
            CreateSubTitleImage();

            mPreviewController = new MiniPreviewController(
                       null,
                       imageSlide,
                       mPreviewPictureBox,
                       menu.MusicSlide);

            //
            // Callback post render incase we need to then render subtitles
            //
            mPreviewController.PostRenderCallback += new RenderVideoParameters.RenderCallbackDelegate(PostRenderCallback);

            //
            // Callback when mouse move to see if we are over a menu button
            //
            mPreviewPictureBox.MouseMove += new MouseEventHandler(mPreviewPictureBox_MouseMove);

            this.ResumeLayout();
        }

        //******************************************************************************************
        private void PostRenderCallback(RenderVideoParameters rvp)
        {
            if (mRenderSubititle != null)
            {
                Rectangle r = new Rectangle(0, 0, mPreviewPictureBox.Width, mPreviewPictureBox.Height);
                RectangleF cov = mRenderSubititle.CoverageArea;
                Rectangle dr = CMenuSubPictureRenderer.GetCoverageAreaForStyle(cov, r, mForMenu, mRenderSubititle);

                float u1 = dr.X / ((float)mPreviewPictureBox.Width);
                float v1 = dr.Y / ((float)mPreviewPictureBox.Height);
                float u2 = u1 + (dr.Width / ((float)mPreviewPictureBox.Width));
                float v2 = v1 + (dr.Height / ((float)mPreviewPictureBox.Height));

                GraphicsEngine.Current.DrawImage(mSubtitleCompleteImage, new RectangleF(u1, v1, u2, v2), dr, true);
            }
        }

        //******************************************************************************************
        private void CreateSubTitleImage()
        {
            Bitmap b = new Bitmap(mPreviewPictureBox.Width, mPreviewPictureBox.Height,System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(b))
            {
                g.Clear(Color.FromArgb(0, 0, 0, 0));

                CImageSlide slide = mForMenu.BackgroundSlide;
                if (slide == null)
                {
                    return;
                }

                List<CMenuButton> buttons = new List<CMenuButton>();

                ArrayList decs = slide.GetAllAndSubDecorations();
                foreach (CDecoration d in decs)
                {
                    CMenuButton button = d as CMenuButton;
                    if (button != null)
                    {
                        buttons.Add(button);
                    }
                }

                mRenderer.Render(buttons, g, mPreviewPictureBox.Size.Width, mPreviewPictureBox.Size.Height, mForMenu);
            }

             mSubtitleCompleteImage = new CImage(b);
        }

        //******************************************************************************************
        private void mDoneButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //******************************************************************************************
        protected override void OnClosed(EventArgs e)
        {
            //
            // Clean up textures;
            //
            mSubtitleCompleteImage.DisposeRawImage();
            GraphicsEngine.Current.ClearCachedTexuresForImage(mSubtitleCompleteImage);
            mSubtitleCompleteImage = null;

            base.OnClosed(e);
        }

        //******************************************************************************************
        private void PreviewMenuForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MiniPreviewController.StopAnyPlayingController();
        }

        //******************************************************************************************
        private void mRestartButton_Click(object sender, EventArgs e)
        {
            mPreviewController.ResetFrameCountToStart();
        }

        //******************************************************************************************
        private Rectangle GetImageRectangleOfRectF(RectangleF d)
        {
            float cw = mPreviewPictureBox.Size.Width;
            float ch = mPreviewPictureBox.Size.Height;
            float x = cw * d.X;
            float y = ch * d.Y;
            float width = cw * d.Width;
            float height = ch * d.Height;

            Rectangle r1 = new Rectangle((int)x, (int)y, (int)width, (int)height);
            return r1;
        }
      
        //******************************************************************************************
        private CMenuButton FindMenuButton(Point pt)
        {
            CImageSlide slide = mForMenu.BackgroundSlide;
            if (slide==null)
            {
                return null;
            }

            ArrayList decs = slide.GetAllAndSubDecorations();

            foreach (CDecoration d in decs)
            {
                CMenuButton menuButton = d as CMenuButton;
                // is the a menu button?
                if (menuButton !=null)
                {
                    //
                    // Menu buttons can not be rotated so test is simple
                    //
                    Rectangle r = GetImageRectangleOfRectF(menuButton.CoverageArea);
                    bool containsPoint =  r.Contains(pt);

                    if (containsPoint)
                    {
                        return menuButton;
                    }
                }
            }
            return null;
        }

        //******************************************************************************************
        private void mPreviewPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            mRenderSubititle = null;

            mPreviewPictureBox.Cursor = Cursors.Default;

            CMenuButton menuButton = FindMenuButton(new Point(e.X, e.Y));
            if (menuButton!=null)
            {
                mRenderSubititle = menuButton;
            }
        }
   
        //******************************************************************************************
        private void RenderSubtitle(CMenuButton menuButton)
        {
            List<CMenuButton> buttons = new List<CMenuButton>();
            buttons.Add(menuButton);

            using (Graphics g = mPreviewPictureBox.CreateGraphics())
            {
                mRenderer.Render(buttons, g, mPreviewPictureBox.Size.Width, mPreviewPictureBox.Size.Height, mForMenu);
            }
        }
    }
}
