using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;
using System.IO;
using DVDSlideshow.GraphicsEng;
using ManagedCore;

namespace CustomButton
{
    public partial class SelectAlphaMapControl : UserControl
    {
        private List<CImageDecoration> mForDecorations; // same image

        public delegate void AlphaMapSelectedCallbackDelegate();

        public event AlphaMapSelectedCallbackDelegate AlphaMapSelected;

        //******************************************************************************************
        public SelectAlphaMapControl()
        {
            InitializeComponent();

            if (CGlobals.mIsTemplateUser == false)
            {
                mGenerateImages.Visible = false;
            }
        }


        //*************************************************************************************************
        public void SetForDecoration(List<CImageDecoration> decorations)
        {
            mForDecorations = decorations;

            PopulateListView();
            SelectCurrentAlphaMap();
        }

        //******************************************************************************************
        private void SelectCurrentAlphaMap()
        {
            if (mForDecorations==null || mForDecorations.Count == 0 || mForDecorations[0].AlphaMap == null)
            {
                foreach (ListViewItem item in  mAlphaMapsListView.Items)
                {
                    if (item.Selected == true)
                    {
                        item.Selected = false;
                    }
                }
                return;
            }

            string name = Path.GetFileNameWithoutExtension(mForDecorations[0].AlphaMap.ImageFilename);

            foreach (ListViewItem lvi in mAlphaMapsListView.Items)
            {
                if (lvi.Text == name)
                {
                    lvi.Selected = true;
                    mAlphaMapsListView.Select();
                    break;
                }
            }
        }

        //******************************************************************************************
        private void PopulateListView()
        {
            if (mAlphaMapsListView.Items.Count != 0)
            {
                return;
            }

            string thumbnailsDirectory = GetThumbnailsDirectory();
            FileInfo[] files = null;
            try
            {
                System.IO.DirectoryInfo directoryInfo = new DirectoryInfo(thumbnailsDirectory);

                files = directoryInfo.GetFiles();
            }
            catch (Exception e)
            {
                Log.Warning("Could not get mask thumbnails file info in folder " + thumbnailsDirectory + " " + e.Message);
            }

            string folder = CGlobals.GetAlphaMapDirectory();

            string[] filters = Directory.GetFiles(folder);

            int size = filters.Length;

            ListViewItem[] items = new ListViewItem[size];

            int index = 0;

            // add file based filters
            foreach (string filter in filters)
            {
                string name = Path.GetFileNameWithoutExtension(filter);

                ListViewItem lvu = new ListViewItem(name);
                lvu.ImageIndex = 0;
                items[index++] = lvu;

                if (files != null)
                {
                    string fn = name + ".jpg";
                    foreach (FileInfo fi in files)
                    {
                        if (fi.Name == fn)
                        {
                            FileStream fs = new FileStream(thumbnailsDirectory + "\\" + fi.Name, FileMode.Open, FileAccess.Read);
                            Bitmap thumbnail = Bitmap.FromStream(fs, true, false) as Bitmap;
                            mAlphaMapImageList.Images.Add(thumbnail.Clone() as Bitmap);
                            fs.Close();
                            fs.Dispose();
                            thumbnail.Dispose();
                            lvu.ImageIndex = mAlphaMapImageList.Images.Count - 1;
                            break;
                        }
                    }
                }

            }

            this.mAlphaMapsListView.Items.AddRange(items);
        }

        //******************************************************************************************
        private void mAlphaMapsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mForDecorations==null || mForDecorations.Count == 0) return;

            bool changed = false;

            if (mAlphaMapsListView.SelectedItems.Count > 0)
            {
                string SelectdName = mAlphaMapsListView.SelectedItems[0].Text;

                string decorationName = "";

                if (mForDecorations[0].AlphaMap != null)
                {
                    decorationName = Path.GetFileNameWithoutExtension(mForDecorations[0].AlphaMap.ImageFilename);
                }

                if (SelectdName != decorationName)
                {
                    string filename = CGlobals.GetAlphaMapDirectory() + @"\" + SelectdName + ".jpg";

                    if (File.Exists(filename))
                    {
                        foreach (CImageDecoration imageDec in mForDecorations)
                        {
                            CImage map = new CImage(filename, false);
                            imageDec.AlphaMap = map;
                        }
                        changed = true;                    
                    }
                    else
                    {
                        Log.Error("Alpha map could not be found:" + filename);
                    }
                }
            }
            else if (mForDecorations[0].AlphaMap !=null)
            {
                foreach (CImageDecoration imageDec in mForDecorations)
                {
                    imageDec.AlphaMap = null;
                }
                changed = true;
            }

            if (changed==true && AlphaMapSelected != null)
            {
                AlphaMapSelected();
            }

        }

        //******************************************************************************************
        private void mAlphaMapsListView_VisibleChanged(object sender, EventArgs e)
        {
            if (mForDecorations==null || mForDecorations.Count == 0) return;
            if (Visible == true)
            {
                this.SelectCurrentAlphaMap();
                SelectCurrentAlphaMap();
            }
        }

        //******************************************************************************************
        private void mClearButton_Click(object sender, EventArgs e)
        {
            if (mAlphaMapsListView.SelectedItems.Count > 0)
            {
                mAlphaMapsListView.SelectedItems[0].Selected = false;
                mAlphaMapsListView.Select();
            }
        }

        //*************************************************************************************************
        private string GetThumbnailsDirectory()
        {
            return CGlobals.GetAlphaMapDirectory() + "\\thumbnails";
        }


        //*************************************************************************************************
        private Bitmap GenerateImagesForListViewItem(string alphaMap)
        {
            string file = CGlobals.GetGuiImagesDirectory()+ "\\Sicily2.jpg";

            CClipArtDecoration cad = new CClipArtDecoration(file, new RectangleF(0,0,1,1),0);
            CBlankStillPictureSlide bsps = new CBlankStillPictureSlide(cad);
            bsps.UsePanZoom = false;
            cad.AlphaMap = new CImage( CGlobals.GetAlphaMapDirectory() + "\\" + alphaMap +".jpg", false);
            CSlideShow slideshow = new CSlideShow("");
            bsps.SetBackgroundColor(Color.White);
            slideshow.FadeIn = false;
            slideshow.FadeOut = false;
            slideshow.mSlides.Add(bsps);
           
            RenderVideoParameters rvp = new RenderVideoParameters();

            int w = cad.mImage.GetRawImage().Width;
            int h = cad.mImage.GetRawImage().Height;

            Bitmap endBitmap = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            rvp.req_width = w*5;
            rvp.req_height = h*5;

            Bitmap bitmap = slideshow.RenderVideoToBitmap(rvp);

            using (Graphics g = Graphics.FromImage(endBitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.DrawImage(bitmap, 0, 0, endBitmap.Width, endBitmap.Height);
            }

            return endBitmap;
        }


        //******************************************************************************************
        private void mGenerateImages_Click(object sender, EventArgs e)
        {
            ListView.ListViewItemCollection collection = mAlphaMapsListView.Items;

            string thumbnailsDirectory = GetThumbnailsDirectory();

            System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(thumbnailsDirectory);

            foreach (FileInfo file in downloadedMessageInfo.GetFiles())
            {
                file.Delete();
            }

            foreach (ListViewItem lvi in collection)
            {
                Bitmap bitmap = GenerateImagesForListViewItem(lvi.Text);

                if (bitmap == null)
                {
                    Log.Error("Could not generate image for alphamap " + lvi.Text);
                    lvi.ImageIndex = 0;
                }
                else
                {
                    mAlphaMapImageList.Images.Add(bitmap);
                    lvi.ImageIndex = mAlphaMapImageList.Images.Count - 1;

                    string fn = thumbnailsDirectory + "\\" + lvi.Text + ".jpg";
                    try
                    {
                        bitmap.Save(fn, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    catch
                    {
                    }
                }

                mAlphaMapsListView.RedrawItems(0, collection.Count - 1, false);
            }
        }
    }
}
