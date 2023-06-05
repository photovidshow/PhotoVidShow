using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;
using System.IO;
using System.Drawing;
using ManagedCore;

namespace CustomButton
{
    public class FilterTabControlBase : UserControl
    {
        //******************************************************************************************
        // Represent a list view item
        protected class FilterListViewItem : ListViewItem
        {
            public enum FilterType
            {
                Blur,
                ColourAdjust,
                Video,      // draw full screen with some render method
                Image,      // draw full screen with some render method
                Monotone,
                Sepia
            }

            public FilterListViewItem(string text, string filename)
                : base(text)
            {
                mFilename = filename;
            }

            public string mFilename;
            public CDecoration mDecoration = null;
            public FilterType mType = FilterType.Video;
            public CImageDecoration.RenderMethodType mRenderMethod = CImageDecoration.RenderMethodType.ADD;
        }

        protected MiniPreviewController mPreviewController = null;
        protected RebuildGuiSlideshowCallbackDelegate mGuiRebuildSlideshowCallback = null;
        protected Control mPreviewInControl;
        protected CSlide mCurrentEditingSlide;
        protected CSlideShow mCurrentEditingSlideshow;

        protected static string[] mRenderMethodString = { "(add)", "(mod1x)", "(mod2x)", "(mod4x)", "(sub1)", "(sub2)" };
        protected static CImageDecoration.RenderMethodType[] mRenderMethods = { CImageDecoration.RenderMethodType.ADD,
                                                                     CImageDecoration.RenderMethodType.MODULATE1X,
                                                                     CImageDecoration.RenderMethodType.MODULATE2X,
                                                                     CImageDecoration.RenderMethodType.MODULATE4X,
                                                                     CImageDecoration.RenderMethodType.SUBTRACT_DEST_MINUS_SRC,
                                                                     CImageDecoration.RenderMethodType.SUBTRACT_SRC_MINUS_DEST };

         //******************************************************************************************
        public static string GetFilterNameFromFilename(string filename)
        {
            CImageDecoration.RenderMethodType renderMethod = CImageDecoration.RenderMethodType.NORMAL;
            return GetFilterNameFromFilename(filename, ref renderMethod);
        }

        //******************************************************************************************
        public static string GetFilterNameFromFilename(string filename, ref CImageDecoration.RenderMethodType renderMethod)
        {
            filename = Path.GetFileNameWithoutExtension(filename);
            for (int count = 0; count < mRenderMethodString.Length; count++)
            {
                if (filename.Contains(mRenderMethodString[count]) == true)
                {
                    filename = filename.Replace(mRenderMethodString[count], "");                 
                    renderMethod = mRenderMethods[count];

                    break;
                }
            }
            filename = FirstLetterToUpper(filename);

            return filename;
        }

        //******************************************************************************************
        protected virtual string GetThumbnailsDirectory()
        {
            return "";
        }

        //******************************************************************************************
        private static string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        //******************************************************************************************
        protected void PopulateCandidateFiltersListView(ListView forListView, string folder, bool addAuxialaryFilters, ImageList forImageList)
        {
            string[] filters = Directory.GetFiles(folder);

            List<ListViewItem> items = new List<ListViewItem>(); 

            if (addAuxialaryFilters == true)
            {
                // sepia transform
                FilterListViewItem sepiaListViewItem = CreateSepiaFilterListItem();
                items.Add(sepiaListViewItem);

                // montone tranform
                FilterListViewItem monotoneListViewItem = CreateMonotoneFilterListItem();
                items.Add(monotoneListViewItem);

                // colour transform
                FilterListViewItem colourAdjustListViewItem = CreateColourAdjustFilterListItem();
                items.Add(colourAdjustListViewItem);

                // add  blur
                FilterListViewItem blurListViewItem = CreateBlurFilterListItem();
                items.Add(blurListViewItem);  
            }


            string thumbnailsDirectory = GetThumbnailsDirectory();
            FileInfo[] files = null;
            if (thumbnailsDirectory != "")
            {
                try
                {
                    System.IO.DirectoryInfo directoryInfo = new DirectoryInfo(thumbnailsDirectory);

                    files = directoryInfo.GetFiles();
                }
                catch (Exception e)
                {
                    Log.Warning("Could not get filter thumbnails file info in folder " + thumbnailsDirectory + " " + e.Message);
                }

            }

            // Because we need to check for matching .jpg/pvsi items, we need to ensure both versions are lower case for string comparison
            // Ensure filenames are all lower case  
            for (int i = 0; i < filters.Length; i++)
            {
                filters[i] = filters[i].ToLower();
            }

            List<string> filtersList = new List<string>(filters);

            // add file based filters
            foreach (string filter in filtersList)
            {
                if (CGlobals.IsImageFilename(filter) == false &&
                    CGlobals.IsVideoFilename(filter) == false)
                {
                    continue;
                }
                
                // ok if a pvsi file and we've also got the .jpg version, then ignore
                if (filter.EndsWith(".pvsi"))
                {
                    string jpgversion = filter.Substring(0, filter.Length - 5) + ".jpg";
                    if (filtersList.Contains(jpgversion) == true)
                    {
                        continue;
                    }

                    string pngversion = filter.Substring(0, filter.Length - 5) + ".png";
                    if (filtersList.Contains(pngversion) == true)
                    {
                        continue;
                    }
                }

                // ok if a pvsv file and we've also got the .wmv version, then ignore
                if (filter.EndsWith(".pvsv"))
                {
                    string wmvversion = filter.Substring(0, filter.Length - 5) + ".wmv";
                    if (filtersList.Contains(wmvversion) == true)
                    {
                        continue;
                    }
                }

                
                // Default render method mod2x unless a pvsi file or png file
                CImageDecoration.RenderMethodType renderMethod = CImageDecoration.RenderMethodType.MODULATE2X;
                if (filter.EndsWith(".png") == true || filter.EndsWith(".pvsi"))
                {
                    renderMethod = CImageDecoration.RenderMethodType.NORMAL;
                }

                string listViewName = GetFilterNameFromFilename(filter, ref renderMethod);

                FilterListViewItem lvu = new FilterListViewItem(listViewName, filter);
                lvu.ImageIndex = 0;
                lvu.mRenderMethod = renderMethod;
                lvu.ToolTipText = "Vintage effect";
                items.Add(lvu);
                if (CGlobals.IsVideoFilename(filter) == true)
                {
                    lvu.mType = FilterListViewItem.FilterType.Video;
                }
                else
                {
                    lvu.mType = FilterListViewItem.FilterType.Image;
                }

                // if thumbnails exists, use them
                if (files != null)
                {
                    string fn = listViewName + ".jpg";
                    foreach (FileInfo fi in files)
                    {
                        if (fi.Name.ToLower() == fn.ToLower())
                        {
                            FileStream fs = new FileStream(thumbnailsDirectory + "\\" + fi.Name, FileMode.Open, FileAccess.Read);
                            Bitmap thumbnail = Bitmap.FromStream(fs, true, false) as Bitmap;
                            forImageList.Images.Add(thumbnail.Clone() as Bitmap);
                            fs.Close();
                            fs.Dispose();
                            thumbnail.Dispose();
                            lvu.ImageIndex = forImageList.Images.Count - 1;
                            break;
                        }
                    }
                }
                else
                {
                    // else use this
                    SetImageIndexForFilterType(lvu);
                }
            }

          
            forListView.Items.AddRange(items.ToArray());

        }

        //******************************************************************************************
        protected void SetImageIndexForFilterType(FilterListViewItem flvi)
        {
            if (flvi.mType == FilterListViewItem.FilterType.Image)
            {
                flvi.ImageIndex = 2;
            }
            if (flvi.mType == FilterListViewItem.FilterType.Video)
            {
                flvi.ImageIndex = 1;
            }
        }

        //******************************************************************************************
        protected FilterListViewItem CreateBlurFilterListItem()
        {
            FilterListViewItem blurListViewItem = new FilterListViewItem("Blur", "");
            blurListViewItem.ImageIndex = 0;
            blurListViewItem.ToolTipText = "Blur";
            blurListViewItem.mType = FilterListViewItem.FilterType.Blur;
            return blurListViewItem;

        }

        //******************************************************************************************
        protected FilterListViewItem CreateColourAdjustFilterListItem()
        {
            FilterListViewItem colourAdjustListViewItem = new FilterListViewItem("Adjust Color", "");
            colourAdjustListViewItem.ImageIndex = 0;
            colourAdjustListViewItem.ToolTipText = "Adjust Color";
            colourAdjustListViewItem.mType = FilterListViewItem.FilterType.ColourAdjust;
            return colourAdjustListViewItem;
        }

        //******************************************************************************************
        protected FilterListViewItem CreateMonotoneFilterListItem()
        {
            FilterListViewItem monotoneListViewItem = new FilterListViewItem("Monotone", "");
            monotoneListViewItem.ImageIndex = 0;
            monotoneListViewItem.ToolTipText = "Monotone Color";
            monotoneListViewItem.mType = FilterListViewItem.FilterType.Monotone;
            return monotoneListViewItem;
        }

        //******************************************************************************************
        protected FilterListViewItem CreateSepiaFilterListItem()
        {
            FilterListViewItem sepiaListViewItem = new FilterListViewItem("Sepia", "");
            sepiaListViewItem.ImageIndex = 0;
            sepiaListViewItem.ToolTipText = "Sepia";
            sepiaListViewItem.mType = FilterListViewItem.FilterType.Sepia;
            return sepiaListViewItem;
        }

    }
}
