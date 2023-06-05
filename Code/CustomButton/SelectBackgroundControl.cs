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
using DVDSlideshow.GraphicsEng;

namespace CustomButton
{
    public partial class SelectBackgroundControl : UserControl
    {
        static private Image mVideoBorderImage = null;
        static public Image GetVideoBorderImage()
        {
   			if (mVideoBorderImage == null)
			{
                try
                {
                    mVideoBorderImage = Image.FromFile(CGlobals.GetGuiImagesDirectory() + @"\film.png", false);
                }
                catch
                {
                }
			}
            return mVideoBorderImage;
        }


        public delegate void ChangedBackgroundCallback();

        public event ChangedBackgroundCallback ChangedBackground;
        public event ChangedBackgroundCallback ChangedAllSlidesBackground;

        // global listviewitems and image list,  no point re-generating this every time we add a blank slide
        private static List<ListViewItem> mBackgroundsListViewItems = null;
        private static ImageList mBackGroundImageList = null;

        //******************************************************************************************
        private class BackgroundListViewItem : ListViewItem
        {
            public string mFilename;
            public int mWidth;
            public int mHeight;
            public bool mVideo = false;
            public BackgroundListViewItem(string name, string filename)
                : base(name)
            {
                mFilename = filename;
            }

            //******************************************************************************************
            public override object Clone()
            {
                BackgroundListViewItem newblvi = new BackgroundListViewItem(this.Name, mFilename);

                newblvi.ImageIndex = this.ImageIndex;
                newblvi.mWidth = this.mWidth;
                newblvi.mVideo = this.mVideo;
                newblvi.mHeight = this.mHeight;

                return newblvi;
            }

       }

        private ColorDialog mColourDialog = new ColorDialog();
        private CImageSlide mForSlide;
        private CSlideShow mForSlideshow;
        private OpenFileDialog mOpenFileDialog = new OpenFileDialog();


        //******************************************************************************************
        public SelectBackgroundControl()
        {
            InitializeComponent();

            // have we already created one of these before? if so use cached listviewitems and imagelist
            if (mBackgroundsListViewItems != null)
            {
                List<ListViewItem> newItems = new List<ListViewItem>();
                foreach (ListViewItem item in mBackgroundsListViewItems)
                {
                    newItems.Add((ListViewItem)item.Clone());
                }

                this.mBackgroundsListView.LargeImageList = mBackGroundImageList;
                this.mBackgroundsListView.Items.AddRange(newItems.ToArray());   
            }

            bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);

            if (designMode == true) return;

            mOpenFileDialog.Filter = CGlobals.GetTotalImageVideoFilter();

            string myPictures = DefaultFolders.GetFolder("MyPictures");
            mOpenFileDialog.InitialDirectory = myPictures;
            mOpenFileDialog.Title = "Open image";
            mOpenFileDialog.Multiselect = false;
        }

        //******************************************************************************************
        public Button GetApplySettingToAllSlidesButton()
        {
            return mApplySettingToAllSlides;
        }

        //******************************************************************************************
        public Button GetBrowseButton()
        {
            return mBrowseButton;
        }

        //******************************************************************************************
        public Button GetClearButton()
        {
            return mClearButton;
        }

        //******************************************************************************************
        public void SetSlide(CImageSlide slide, CSlideShow forSlideshow)
        {
            mForSlide = slide;
            mForSlideshow = forSlideshow;
            DeSelectListItem();

        }

        //*******************************************************************
        private RectangleF CalcClipAreaToFitFrame(RectangleF mCoverageArea, float sw, float sh)
        {
     
            float coverage_aspect = (mCoverageArea.Width) / (mCoverageArea.Height );

            float image_aspect = sw / sh;

            if (image_aspect < coverage_aspect) // e.g. 4.3 on 16:9
            {
                float newh = sw / coverage_aspect;

                return new RectangleF(0, (sh - newh) / 2.0f, sw, newh);
            }
            else if (image_aspect > coverage_aspect)
            {
                float neww = sh * coverage_aspect;

                return new RectangleF((sw - neww) / 2.0f, 0, neww, sh);
            }

            return new RectangleF(0, 0, sw, sh);
        }


        //******************************************************************************************
        private bool Abort()
        {
            return false;
        }


        //******************************************************************************************
        public void PopulateBackgroundListView()
        {
            if (mBackgroundsListViewItems == null)
            {
                mBackgroundsListViewItems = new List<ListViewItem>();
            }

            string[] backgrounds = Directory.GetFiles(CGlobals.GetBackgroundsDirectory());
            List<string> backgroundsList = new List<string>(backgrounds);

            int index = 0;
            bool shownMissingThumbanilsError = false;

            // add file based filters
            foreach (string backgroundFilename in backgroundsList)
            {
                // ok if a pvsi file and we've also got the .jpg version, then ignore
                if (backgroundFilename.EndsWith(".pvsi"))
                {
                    string jpgversion = backgroundFilename.Substring(0, backgroundFilename.Length - 5) + ".jpg";
                    if (backgroundsList.Contains(jpgversion) == true)
                    {
                        continue;
                    }

                    string pngversion = backgroundFilename.Substring(0, backgroundFilename.Length - 5) + ".png";
                    if (backgroundsList.Contains(pngversion) == true)
                    {
                        continue;
                    }

                }

                // maybe thumbnail exists

                Image i = null;
                bool isVideo = CGlobals.IsVideoFilename(backgroundFilename);

                if (isVideo || CGlobals.IsImageFilename(backgroundFilename) )
                {

                    string path = System.IO.Path.GetDirectoryName(backgroundFilename);
                    string name = System.IO.Path.GetFileName(backgroundFilename);
                    string nameWithoutExtention = System.IO.Path.GetFileNameWithoutExtension(name);

                    string thumbnail = path + "\\thumbnails\\" + nameWithoutExtention + ".jpg";
                    if (System.IO.File.Exists(thumbnail) == true)
                    {
                        i = Image.FromFile(thumbnail);
                    }
                    else
                    {
                        //
                        // Only generate thumbanil images if we are running from c:\dev
                        // This is for our use only
                        //
                        if (backgroundFilename.ToLower().StartsWith("c:\\dev\\photovidshow") == true)
                        {
                            string listViewName = Path.GetFileNameWithoutExtension(backgroundFilename);

                            string saveThumbnail = thumbnail;

                            if (CGlobals.IsImageFilename(backgroundFilename) == true)
                            {
                                i = Image.FromFile(backgroundFilename);

                            }
                            else if (CGlobals.IsVideoFilename(backgroundFilename) == true)
                            {
                                try
                                {
                                    CVideoDecoration vd = new CVideoDecoration(backgroundFilename, new RectangleF(0, 0, 1, 1), 0);
                                    int w = vd.GetVideoWidth();
                                    int h = vd.GetVideoHeight();

                                    i = DecorationThumbnailGenerator.GenerateThumbnailForDecoration(vd, w, h, null, false, true, false);
                                }
                                catch
                                {
                                    Log.Error("Failed to load background video " + backgroundFilename);
                                    i = null;
                                }
                            }

                            float imageAspect = ((float)i.Width) / ((float)i.Height);
                            int thumbnailWidth = imageList1.ImageSize.Width;
                            int thumbnailHeight = (int)((((float)thumbnailWidth) / imageAspect) + 0.49999f);

                            Image thumbnailImage = i.GetThumbnailImage(thumbnailWidth, thumbnailHeight, new Image.GetThumbnailImageAbort(Abort), System.IntPtr.Zero);
                            thumbnailImage.Save(saveThumbnail); // EXCEPTION HERE no write access
                            thumbnailImage.Dispose();
                        }
                        else
                        {
                            // missing thumbnail image, show grey icon and report error
                            Log.Error("Missing thumbnail image '" + name + "'");
                            if (shownMissingThumbanilsError == false)
                            {
                                shownMissingThumbanilsError = true;
                                MessageBox.Show("You are missing some thumbnail images used by the slide backgrounds.  Re-installing PhotoVidShow may fix this problem.", "Missing thumbnail images", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            i = new Bitmap(134, 75);
                            using (Graphics g = Graphics.FromImage(i))
                            {
                                g.Clear(Color.LightGray);
                            }
                        }
                    }

                    if (i != null)
                    {                    
                        RectangleF rec = CalcClipAreaToFitFrame(new RectangleF(0, 0, imageList1.ImageSize.Width, imageList1.ImageSize.Height), i.Width, i.Height);

                        Bitmap b = new Bitmap(imageList1.ImageSize.Width, imageList1.ImageSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        using (Graphics g = Graphics.FromImage(b))
                        {
                            RectangleF destRec = new RectangleF(0, 0, b.Width, b.Height);
                            g.Clear(Color.White);
                            g.DrawImage(i, destRec, rec, GraphicsUnit.Pixel);

                            if (isVideo == true)
                            {
                                Image border = GetVideoBorderImage();
                                if (border != null)
                                {
                                    g.DrawImage(border, destRec);
                                }
                            }

                        }

                        BackgroundListViewItem blvi = new BackgroundListViewItem("", backgroundFilename);
                        blvi.ImageIndex = index;
                        blvi.mWidth = i.Width;
                        blvi.mVideo = isVideo;
                        blvi.mHeight = i.Height;

                        imageList1.Images.Add(b);
                        mBackgroundsListViewItems.Add(blvi);
                        index++;
                        i.Dispose();
                    }

                }
            }

            mBackgroundsListView.Items.AddRange(mBackgroundsListViewItems.ToArray());
            mBackGroundImageList = imageList1;
        }

        //******************************************************************************************
        private void mBackgroundColourButton_Click(object sender, EventArgs e)
        {
            DialogResult result = mColourDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                mBackgroundColourButton.BackColor = mColourDialog.Color;
            }

        }

        //******************************************************************************************
        private void mSetAsColourButton_Click(object sender, EventArgs e)
        {
            CBlankStillPictureSlide bsps = mForSlide as CBlankStillPictureSlide;
            if (bsps != null)
            {
                Clear(false);
                bsps.BlankColor = mBackgroundColourButton.BackColor;

                DeSelectListItem();

                if (ChangedBackground!=null)
                {
                    ChangedBackground();
                }
            }
        }

        //******************************************************************************************
        private void DeSelectListItem()
        {
            ListView.SelectedListViewItemCollection items = mBackgroundsListView.SelectedItems;
            if (items.Count != 0)
            {
                BackgroundListViewItem item = items[0] as BackgroundListViewItem;
                item.Selected = false;
                mBackgroundsListView.Select();
            }
        }

        //******************************************************************************************
        private void AdjustCoverageAreaToMaintainAspect(CImageDecoration cd)
        {
            RectangleF area = new RectangleF(0, 0, 1, 1);
            float imageAspect = cd.GetOriginialImageAspectRatio();

            float slideAspect = 1 / CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction();

            float correctImageAspect = imageAspect / slideAspect;

            if (imageAspect < slideAspect)
            {
                float w = 1;
                float h = w / correctImageAspect;
                area = new RectangleF(0, -((h - 1) / 2), w, h);
            }
            else
            {
                float h = 1;
                float w = h * correctImageAspect;
                area = new RectangleF(-((w - 1) / 2), 0, w, h);
            }

            cd.CoverageArea = area;


        }


        //******************************************************************************************
        private void mBackgroundsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ListView.SelectedListViewItemCollection items = mBackgroundsListView.SelectedItems;
                if (items.Count == 0)
                {
                    return;
                }

                BackgroundListViewItem item = items[0] as BackgroundListViewItem;
                if (item == null)
                {
                    return;
                }

                Clear(true);

                CImageDecoration decor = null;
                if (item.mVideo == false)
                {
                    decor = new CClipArtDecoration(item.mFilename, new RectangleF(0, 0, 1, 1), 0);
                }
                else
                {
                    CVideoDecoration vd = CGlobalVideoDatabase.GetInstance().CreateVideoDecoration(item.mFilename);
                    decor = vd;
                }

                if (decor.VerfifyAllMediaFilesToRenderThisExist() == true)
                {
                    AdjustCoverageAreaToMaintainAspect(decor);
                    decor.MarkAsBackgroundDecoration();
                    mForSlide.AddDecoration(decor);
                }
            }
            finally
            {
                // This method recusively calls itself (i.e. de-selects current then selects new)
                if (ChangedBackground != null && CGlobals.mCurrentProject.IgnoreProjectChanges == false)
                {
                    CGlobals.mCurrentProject.IgnoreProjectChanges = true;
                    ChangedBackground();
                    CGlobals.mCurrentProject.IgnoreProjectChanges = false;
                    CGlobals.mCurrentProject.DeclareChange("Changed background");
                }
            }

           
        }

        //******************************************************************************************
        private void Clear(bool setToBlack)
        {  
            if (mForSlide == null) return;

            if (setToBlack == true)
            {
                CBlankStillPictureSlide bsps = mForSlide as CBlankStillPictureSlide;
                if (bsps != null)
                {
                    bsps.BlankColor = Color.Black;
                }
            }

            ArrayList list = mForSlide.GetAllAndSubDecorations();
            foreach (CDecoration d in list)
            {
                if (d.IsBackgroundDecoration())
                {
                    mForSlide.RemoveDecoration(d);
                    break;
                }
            }
        }

        //******************************************************************************************
        private void mClearButton_Click(object sender, EventArgs e)
        {
            Clear(true);
            DeSelectListItem();

            if (ChangedBackground != null)
            {
                ChangedBackground();
            }
        }

        //******************************************************************************************
        private void mApplySettingToAllSlides_Click(object sender, EventArgs e)
        {  
            CImageDecoration backgroundDecor = null;

            if (mForSlide is CBlankStillPictureSlide == false) return ;
            
            ArrayList list = mForSlide.GetAllAndSubDecorations();
            foreach (CDecoration d in list)
            {
                if (d.IsBackgroundDecoration() && d is CImageDecoration)
                {
                    backgroundDecor = d as CImageDecoration;
                    break;
                }
            }

            bool setBackgroundDone = false;

            foreach (CSlide s in mForSlideshow.mSlides)
            {
                if (s != mForSlide)
                {                 
                    CImageSlide imageSlide = s as CImageSlide;
                    if (imageSlide != null)
                    {
                        setBackgroundDone = true;

                        list = imageSlide.GetAllAndSubDecorations();
                        foreach (CDecoration d in list)
                        {
                            if (d.IsBackgroundDecoration())
                            {
                                imageSlide.RemoveDecoration(d);
                                break;
                            }
                        }

                        if (backgroundDecor != null)
                        {
                            CImageDecoration newDecor = null;
                            CClipArtDecoration cad = backgroundDecor as CClipArtDecoration;
                            if (cad != null)
                            {
                                newDecor = new CClipArtDecoration(cad.mImage.ImageFilename, new RectangleF(0, 0, 1, 1), 0);
                            }
                            else
                            {
                                CVideoDecoration vd = backgroundDecor as CVideoDecoration;
                                if (vd != null)
                                {
                                    newDecor = CGlobalVideoDatabase.GetInstance().CreateVideoDecoration(vd.GetFilename());
                                }
                            }

                            if (newDecor != null)
                            {
                                AdjustCoverageAreaToMaintainAspect(newDecor);
                                newDecor.MarkAsBackgroundDecoration();
                                imageSlide.AddDecoration(newDecor);
                            }
                        }
                        else
                        {
                            CBlankStillPictureSlide bsps = s as CBlankStillPictureSlide;
                            if (bsps != null)
                            {
                                bsps.BlankColor = (mForSlide as CBlankStillPictureSlide).BlankColor;
                            }
                        }
                    }
                }
            }

            if (setBackgroundDone && ChangedAllSlidesBackground != null)
            {
                ChangedAllSlidesBackground();
            }
        }

        //******************************************************************************************
        private void mBrowseButton_Click(object sender, EventArgs e)
        {
            if (mOpenFileDialog.ShowDialog().Equals(System.Windows.Forms.DialogResult.OK))
            {
                if (mOpenFileDialog.FileNames.Length > 0)
                {
                    if (ManagedCore.IO.IsDriveOkToUse(mOpenFileDialog.FileNames[0]) == false) return;
                    mOpenFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(mOpenFileDialog.FileNames[0]);  // rember current folder  

                    string file = mOpenFileDialog.FileNames[0];

                    try
                    {
                        CImageDecoration imageDecor = null;

                        if (CGlobals.IsImageFilename(file)==true)
                        {
                            imageDecor = new CClipArtDecoration(file, new RectangleF(0, 0, 1, 1), 0);
                        }
                        else if (CGlobals.IsVideoFilename(file)==true)
                        {
                            imageDecor = CGlobalVideoDatabase.GetInstance().CreateVideoDecoration(file);
                        }

                        if (imageDecor != null)
                        {
                            Clear(false);
                            AdjustCoverageAreaToMaintainAspect(imageDecor);
                            imageDecor.MarkAsBackgroundDecoration();

                            if (imageDecor.VerfifyAllMediaFilesToRenderThisExist() == true)
                            {
                                mForSlide.AddDecoration(imageDecor);
                            }

                            if (ChangedBackground != null)
                            {
                                ChangedBackground();
                            }
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Could not load '" + file + "' as a background", "Load failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
