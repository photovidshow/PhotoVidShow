using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using DVDSlideshow;

namespace CustomTabControl
{
	/// <summary>
	/// Summary description for TabControlEx.
	/// </summary>
	public delegate bool PreRemoveTab(int indx);
	public class TabControlEx : TabControl
	{

        private ArrayList mCachedImages = null;
        private ArrayList mCachedImagesSelected = null;

		public TabControlEx()
			: base()
		{
			PreRemoveTabPage = null;
			//this.DrawMode = TabDrawMode.OwnerDrawFixed;
		}

		public PreRemoveTab PreRemoveTabPage;

        // This is an alternative method to have an imagelist, which is generally un-reliable
        // ************************************************************************************
        public void SetTabImages(List<string> tabImages)
        {
            string guiDir = CGlobals.GetGuiImagesDirectory();

            mCachedImages = new ArrayList(tabImages.Count);

            mCachedImagesSelected = new ArrayList(tabImages.Count);
            for (int i = 0; i < tabImages.Count; i++)
            {
                string filename = guiDir + "\\" + tabImages[i];
                Bitmap b = null;
                try
                {
                    b = Image.FromFile(filename) as Bitmap;
                }
                catch
                {
                    ManagedCore.Log.Error("Could not open file'" + filename + "' in SetTabImages");
                }

                if (b != null)
                {
                    mCachedImages.Add(b);

                    Bitmap image2 = (Bitmap)b.Clone();

                    Color cbb = Color.FromArgb(132, 164, 211);

                    DVDSlideshow.BitmapUtils.FadeToBackColor(image2, cbb);
                    mCachedImagesSelected.Add(image2);
                }
                else
                {
                    mCachedImages.Add(null);
                    mCachedImagesSelected.Add(null);
                }

            }
        }

        // ************************************************************************************
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
            base.OnDrawItem(e);;

            int tab = e.Index;
            Rectangle r = GetTabRect(tab);

			e.Graphics.InterpolationMode = InterpolationMode.Default;
			e.Graphics.SmoothingMode=  SmoothingMode.None;
			e.Graphics.CompositingQuality = CompositingQuality.Default;
	
            // first time round retireve the images from the image list (which now seems to be slow?)
            // and cache them here  

            if (mCachedImages == null && this.ImageList !=null)
            {
                mCachedImages = new ArrayList(this.ImageList.Images.Count);

                mCachedImagesSelected = new ArrayList(this.ImageList.Images.Count);
                for (int i = 0; i < this.ImageList.Images.Count; i++)
                {
                    mCachedImages.Add( ImageList.Images[i] );

                    Bitmap image2 = (Bitmap)ImageList.Images[i].Clone();

                    Color cbb = Color.FromArgb(132, 164, 211);

                    DVDSlideshow.BitmapUtils.FadeToBackColor(image2, cbb);
                    mCachedImagesSelected.Add(image2);
                }
            }

            TabPage tabPage = TabPages[tab];

            if (mCachedImages!=null && mCachedImages.Count > tab)
            {
                if (tab != this.SelectedIndex && mCachedImages.Count > tabPage.ImageIndex)
                {
                    Image i = (Image)mCachedImages[tabPage.ImageIndex];
                    if (i != null)
                    {
                        e.Graphics.DrawImage(i, r, 0, 0, i.Width, i.Height, System.Drawing.GraphicsUnit.Pixel);
                    }
                }
                else if (mCachedImagesSelected.Count > tabPage.ImageIndex)
                {
                    Image i = (Image)mCachedImagesSelected[tabPage.ImageIndex];
                    if (i != null)
                    {
                        Rectangle r2 = new Rectangle(r.X - 2, r.Y -2, r.Width + 4, r.Height + 4);

                        e.Graphics.DrawImage(i, r2, 0, 0, i.Width, i.Height, System.Drawing.GraphicsUnit.Pixel);

                        e.Graphics.DrawImage(i, r, 0, 0, i.Width, i.Height, System.Drawing.GraphicsUnit.Pixel);
                    }
                }
            }		
		}
	}
}
