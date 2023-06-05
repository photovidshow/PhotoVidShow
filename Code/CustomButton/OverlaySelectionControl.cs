using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;
using System.IO;
using System.Collections;
using ManagedCore;

namespace CustomButton
{
    public partial class OverlaySelectionControl : FilterTabControlBase
    {
        public delegate void ChangedBorderCallback();

        public event ChangedBorderCallback ChangedBorder;

        public OverlaySelectionControl()
        {
            InitializeComponent();

            bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);

            if (designMode == true) return;

            if (CGlobals.mIsTemplateUser == false)
            {
                mGenerateImages.Visible = false;
            }


            PopulateCandidateFiltersListView(mBordersListView, CGlobals.GetBordersDirectory(), false, this.mBorderImageList);
        }


        //*************************************************************************************************
        public void SetSlide(CSlide slide, CSlideShow slideshow, RebuildGuiSlideshowCallbackDelegate callback, Control previewControl)
        {
            mPreviewInControl = previewControl;
            mGuiRebuildSlideshowCallback = callback;
            mCurrentEditingSlide = slide;
            mCurrentEditingSlideshow = slideshow;

            SelectChosenBorder();

            mPreviewController = new MiniPreviewController(
                 mCurrentEditingSlideshow,
                 mCurrentEditingSlide,
                 mPreviewInControl);
        }

        //*************************************************************************************************
        private CDecoration GetCurrentBorderDecoration()
        {
            if (mCurrentEditingSlide == null) return null;

            CImageSlide currentImageEditingSlide = mCurrentEditingSlide as CImageSlide;

            ArrayList decors = currentImageEditingSlide.GetAllAndSubDecorations();

            // do in reverse order
            foreach (CDecoration dec in decors)
            {
                if (dec.IsBorderDecoration() == true)
                {
                    mClearButton.Enabled = true;
                    mShowBorderAfterTransitionEffectCheckBox.Enabled = true;
                    return dec;
                }
            }

            mClearButton.Enabled = false;
            mShowBorderAfterTransitionEffectCheckBox.Enabled = false;

            return null;
        }

        //*************************************************************************************************
        private bool DoesListViewItemMatchDecoration(CDecoration dec, FilterListViewItem listItem)
        {
            CClipArtDecoration cad = dec as CClipArtDecoration;
            if (cad == null) return false;

            if (Path.GetFileName(listItem.mFilename) ==
                Path.GetFileName(cad.mImage.ImageFilename))
            {
                return true;
            }
            return false;

        }


        //*************************************************************************************************
        private void SelectChosenBorder()
        {
            CDecoration borderDecor = GetCurrentBorderDecoration();
            if (borderDecor == null)
            {
                SelectNothing();
                return ;
            }

            foreach (FilterListViewItem listItem in mBordersListView.Items)
            {
                mShowBorderAfterTransitionEffectCheckBox.Checked = borderDecor.RenderPostTransition;

                if ( DoesListViewItemMatchDecoration(borderDecor, listItem)==true)
                {
                    listItem.Selected = true;
                    mBordersListView.Select();
                    break;
                }
            }    
        }

        //*************************************************************************************************
        private void mBordersListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool sendChange = true;

            try
            {
                CDecoration borderDecor = GetCurrentBorderDecoration();

                ListView.SelectedListViewItemCollection selectedItems = mBordersListView.SelectedItems;

                CImageSlide imageSlide = mCurrentEditingSlide as CImageSlide;

                if (selectedItems.Count == 0)
                {
                    if (borderDecor == null)
                    {
                        return;
                    }

                    imageSlide.RemoveDecoration(borderDecor);

                    mClearButton.Enabled = false;
                    mShowBorderAfterTransitionEffectCheckBox.Enabled = false;

                    return;
                }

                FilterListViewItem item = selectedItems[0] as FilterListViewItem;
                if (DoesListViewItemMatchDecoration(borderDecor, item) == true)
                {
                    // the same, ignore
                    sendChange = false;
                    return;
                }


                if (borderDecor != null)
                {
                    imageSlide.RemoveDecoration(borderDecor);
                }

                CClipArtDecoration newBorder = new CClipArtDecoration(item.mFilename, new RectangleF(0, 0, 1, 1), 0);
                newBorder.RenderMethod = item.mRenderMethod;
                newBorder.MarkAsBorderDecoration();

                if (newBorder.VerfifyAllMediaFilesToRenderThisExist() == false)
                {
                    sendChange = false;
                    return;
                }

                imageSlide.AddDecoration(newBorder);

                mClearButton.Enabled = true;
                mShowBorderAfterTransitionEffectCheckBox.Enabled = true;
                newBorder.RenderPostTransition = mShowBorderAfterTransitionEffectCheckBox.Checked;
            }
            finally
            {
                if (ChangedBorder != null && sendChange==true)
                {
                    ChangedBorder();
                }
            }
        }

        //*************************************************************************************************

        private void mShowBorderAfterTransitionEffectCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CDecoration borderDecor = GetCurrentBorderDecoration();
            if (borderDecor == null) return;

            if (borderDecor.RenderPostTransition == mShowBorderAfterTransitionEffectCheckBox.Checked)
            {
                return;
            }

            borderDecor.RenderPostTransition = mShowBorderAfterTransitionEffectCheckBox.Checked;

            CImageSlide id = mCurrentEditingSlide as CImageSlide;

            SelectChosenBorder();

            if (ChangedBorder != null)
            {
                ChangedBorder();
            }
        }

        //*************************************************************************************************
        private void SelectNothing()
        {
            ListView.SelectedListViewItemCollection selectedItems = mBordersListView.SelectedItems;

            if (selectedItems.Count > 0)
            {
                selectedItems[0].Selected = false;
                mBordersListView.Select();
            }
        }


        //*************************************************************************************************
        private void mClearButton_Click(object sender, EventArgs e)
        {
            SelectNothing();
        }

        //*************************************************************************************************
        protected override string GetThumbnailsDirectory()
        {
            return CGlobals.GetBordersDirectory() + "\\thumbnails";
        }


        //*************************************************************************************************
        private Bitmap GenerateImagesForListViewItem(FilterListViewItem item, Color backgrondColor)
        {   
            CBlankStillPictureSlide bsps = new CBlankStillPictureSlide();
            bsps.SetBackgroundColor(backgrondColor);
            bsps.UsePanZoom = false;
            CClipArtDecoration newBorder = new CClipArtDecoration(item.mFilename, new RectangleF(0, 0, 1, 1), 0);    
            newBorder.RenderMethod = item.mRenderMethod;
            newBorder.MarkAsBorderDecoration();  
            bsps.AddDecoration(newBorder);   
            CSlideShow slideshow = new CSlideShow("");
      
            slideshow.FadeIn = false;
            slideshow.FadeOut = false;
            slideshow.mSlides.Add(bsps);

            RenderVideoParameters rvp = new RenderVideoParameters();

            Bitmap endBitmap = new Bitmap(mBorderImageList.ImageSize.Width, mBorderImageList.ImageSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int w = CGlobals.mCurrentProject.DiskPreferences.CanvasWidth;
            int h = CGlobals.mCurrentProject.DiskPreferences.CanvasHeight;

            rvp.req_width = w;
            rvp.req_height = h;

            Bitmap bitmap = slideshow.RenderVideoToBitmap(rvp);

            using (Graphics g = Graphics.FromImage(endBitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.DrawImage(bitmap, 0, 0, endBitmap.Width, endBitmap.Height);
            }

            return endBitmap;
        }


        //*************************************************************************************************
        private void mGenerateImages_Click(object sender, EventArgs e)
        {
            ListView.ListViewItemCollection collection = mBordersListView.Items;

            string thumbnailsDirectory = GetThumbnailsDirectory();

            System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(thumbnailsDirectory);

            foreach (FileInfo file in downloadedMessageInfo.GetFiles())
            {
                file.Delete();
            }

            foreach (ListViewItem lvi in collection)
            {
                FilterListViewItem flvi = lvi as FilterListViewItem;
                if (flvi != null)
                {
                    Color backgroundColor = Color.White;

                    // special case
                    if (flvi.mFilename.ToLower().EndsWith("vignette5.png"))
                    {
                        backgroundColor = Color.Black;
                    }

                    Bitmap bitmap = GenerateImagesForListViewItem(flvi, backgroundColor);

                    if (bitmap == null)
                    {
                        Log.Error("Could not generate image for alphamap " + lvi.Text);
                        lvi.ImageIndex = 0;
                    }
                    else
                    {
                        mBorderImageList.Images.Add(bitmap);
                        lvi.ImageIndex = mBorderImageList.Images.Count - 1;

                        string fn = thumbnailsDirectory + "\\" + lvi.Text + ".jpg";
                        try
                        {
                            bitmap.Save(fn, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                        catch
                        {
                        }
                    }
                }

                mBordersListView.RedrawItems(0, collection.Count - 1, false);
            }
        }
    }
}
