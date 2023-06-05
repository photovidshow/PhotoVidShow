using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;
using System.IO;

namespace CustomButton
{
    public partial class ClipartSelectForm : Form
    {
        public ClipartSelectForm()
        {
            InitializeComponent();
            AddClipArtThumnNailsToTab();
        }

        private ClipartThumbnail mHighlighted;

        private string mSelectedItem;

        //*******************************************************************
        public string SelectedItemFilename
        {
            get { return mSelectedItem; }
        }


        //*******************************************************************
        private void AddClipArtThumnNailsToTab()
        {
            List<ClipartThumbnail> thumnails = CreateThumbnails();;

            int numberHorizontal = -1;
            int x = 0;
            int y = 4;

            foreach (ClipartThumbnail thumbnail in thumnails)
            {
                if (numberHorizontal < 0)
                {
                    // determine how many thumbnails can be displayed on one row
                    numberHorizontal = (int)(mClipartPanel.Width);
                }
                // set the position for the thumbnail and add it to the panel's controls

                thumbnail.Left = 2 + x;
                thumbnail.Top = y;
                mClipartPanel.Controls.Add(thumbnail);

                x += thumbnail.Width + 4;

                if (x + thumbnail.Width + 2 >= numberHorizontal)
                {
                    x = 0;
                    y += thumbnail.Height + 4;
                }
            }
        }

        //*******************************************************************
        private List<ClipartThumbnail> CreateThumbnails()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

           List<ClipartThumbnail> thumnails = new List<ClipartThumbnail>();

            try
            {
                // create the thumbnails for the selected files
           
                string[] fileEntries = Directory.GetFiles(CGlobals.GetRootDirectory() + "\\clipart");

                foreach (string fileName in fileEntries)
                {
                    if (CGlobals.IsImageFilename(fileName))
                    {
                       // Console.WriteLine("Loading file " + fileName);
                        ClipartThumbnail thumbnail = new ClipartThumbnail(fileName, this);

                        thumnails.Add(thumbnail);
                    }

                }
            }
            catch (Exception e)
            {
                ManagedCore.CDebugLog.GetInstance().Error("Exception occurred when loading clipart thumbnails: "+e.Message);
            }

            return thumnails;
        }

        //*******************************************************************
        public void SelectClipArt(string filename)
        {
            mSelectedItem = filename;
            this.Close();
        }

        //*******************************************************************
        public void SetHighlighted(ClipartThumbnail item)
        {
            if (mHighlighted != null)
            {
                mHighlighted.UnHighlight();
            }
            mHighlighted = item;
            if (item != null)
            {
                this.mAddToSlideButton.Enabled = true;
            }
            else
            {
                this.mAddToSlideButton.Enabled = false;
            }
        }

        //*******************************************************************
        private void mAddToSlideButton_Click(object sender, EventArgs e)
        {
            if (mHighlighted != null)
            {
                mSelectedItem = mHighlighted.mFileName;
                this.Close();
            }
          
        }

        //*******************************************************************
        private void mCancelButton_Click(object sender, EventArgs e)
        {
            mSelectedItem="";
            this.Close();
        }
    }
}
