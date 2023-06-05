using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;
using System.Drawing.Imaging;
using DVDSlideshow.GraphicsEng;
using ManagedCore;
using System.Windows;
using System.Media;


namespace CustomButton
{
    public partial class EditTemplateMediaOrder : UserControl
    {
        public delegate void MediaSelectedDelegate(CDecoration decoration);

        //*************************************************************************************************
        private class DecorationListViewItem : ListViewItem
        {
            public DecorationListViewItem(string text, List<CDecoration> decors)
                : base(text)
            {
                mForDecorations = decors;
            }

            public List<CDecoration> mForDecorations = new List<CDecoration>(); // list would be decorations with same original template numbers
        }

        private CImageSlide mForSlide;
        private ImageList mImageList = new ImageList();

        private int mThumbnailHeight = 64;
        private int mThumbnailWidth = (int)(((float)64) * 1.3333f);

        public event MediaSelectedDelegate MediaSelectedIndexChanged;
        public event MediaSelectedDelegate MediaSourceChangeRequest;

        private const string mSetImageToolTipText = "Double click to set image";
        private const string mEditImageToolTipText = "Double click to change image";
        private static CImage mTextIcon = null;
        private bool mHideDragDropText = false;

        //*************************************************************************************************
        public EditTemplateMediaOrder()
        {
            if (mTextIcon == null)
            {
                mTextIcon = new CImage(CGlobals.GetGuiImagesDirectory() + "\\text.png", false);
            }

            InitializeComponent();

            // If large font, remove the hint text and increase size of list view (this is to cater for
            // the larger scroll bar and prevents a vertical scroll bar being created.
            if (CGlobals.dpiX > 97)
            {
                mDragDropLabel.Visible = false;
                mHideDragDropText = true;
                mSlideMediaListView.Height += 16;
            }

            mImageList.ColorDepth = ColorDepth.Depth32Bit;
            mImageList.ImageSize = new Size(mThumbnailWidth, mThumbnailHeight);
            mSlideMediaListView.LargeImageList = mImageList;

            mSlideMediaListView.DoDragDrop(mSlideMediaListView.SelectedItems, DragDropEffects.Move);
        }     

        //*************************************************************************************************
        public bool SetForSlide(CImageSlide slide, CImageDecoration selectedDecoration)
        {
            bool selectedDecor = false;

            this.SuspendLayout();

            CustomButton.MiniPreviewController.StopAnyPlayingController();

            List<CDecoration>[] templateDecorationList = new List<CDecoration>[CGlobals.mMaxTemplateNumbers];
    
            try
            {
                mForSlide = slide;
                mImageList.Images.Clear();
                mSlideMediaListView.Clear();

                int imageCount = 1;
                int listViewimageIndex= 0;
                int textCount = 1;

                foreach (CDecoration d in slide.GetAllAndSubDecorations())
                {
                    
                    if (d is CGroupedDecoration ||
                         (d is CImageDecoration == false) ||
                         d.IsFilter() ||
                         d.IsTemplateFrameworkDecoration() ||
                         d.IsBackgroundDecoration() ||
                         d.IsBorderDecoration())
                    {
                        continue;
                    }

                    CImageDecoration imageDecor = d as CImageDecoration;

                    List<CDecoration> list = new List<CDecoration>();

                    list.Add(imageDecor);

                    if (d is CVideoDecoration || 
                        d is CClipArtDecoration)
                    {
                        // is a template image? if so have we already covered it?
                        int templateNum = imageDecor.OriginalTemplateImageNumber;
                        if (templateNum != 0)
                        {
                            if (templateDecorationList[templateNum] != null)
                            {
                                templateDecorationList[templateNum].Add(imageDecor);

                                continue;
                            }

                            templateDecorationList[templateNum] = list;
                        }

                        DecorationListViewItem dlvi = new DecorationListViewItem("", list);   
           
                        if (d.HasTemplateImageFilename() == true)
                        {
                            dlvi.ToolTipText = mSetImageToolTipText;
                        }
                        else
                        {
                            dlvi.ToolTipText = mEditImageToolTipText;
                        }

                        Image rawimage = DecorationThumbnailGenerator.GenerateThumbnailForDecoration(d, mThumbnailWidth, mThumbnailHeight, null, true, false, true);

                        if (rawimage != null)
                        {
                            Image thumbnail = GenerateThumbnailImageFor(rawimage,null);

                            if (thumbnail != null)
                            {
                                // ok store this in listview image
                                mImageList.Images.Add(thumbnail);
                                dlvi.ImageIndex = listViewimageIndex++;

                                int templateNumaber = imageDecor.OriginalTemplateImageNumber;

                                if (templateNum != 0)
                                {
                                    dlvi.Text = "Image (" + templateNum.ToString() + ")";
                                }
                                else
                                {
                                    dlvi.Text = "Image";
                                }

                                imageCount++;

                                if (dlvi.Text != "")
                                {
                                    mSlideMediaListView.Items.Add(dlvi);
                                }
                            }

                            rawimage.Dispose();
                        }
                    }
                    else if (d is CTextDecoration)
                    {
                        CTextDecoration td = d as CTextDecoration;
                        if (td.TemplateEditable == CTextDecoration.TemplateEditableType.None)
                        {
                            continue;
                        }

                        // is a template image? if so have we already covered it?
                        int templateNum = td.OriginalTemplateImageNumber;
                        if (templateNum != 0)
                        {
                            if (templateDecorationList[templateNum] != null)
                            {
                                continue;
                            }

                            templateDecorationList[templateNum] = list;
                        }

                        DecorationListViewItem dlvi = new DecorationListViewItem("Text (" + textCount.ToString() + ")", list);
                        textCount++;

                        Bitmap b = mTextIcon.GetRawImage().Clone() as Bitmap;

                        // ok store this in listview image
                        mImageList.Images.Add(b);
                        dlvi.ImageIndex = listViewimageIndex++;
                        mSlideMediaListView.Items.Add(dlvi);
                    }             
                }

                if (mHideDragDropText == false)
                {
                    mDragDropLabel.Visible = mSlideMediaListView.Items.Count > 1;
                }

                if (mSlideMediaListView.Items.Count > 0)
                {
                
                    if (selectedDecoration != null)
                    {
                        foreach (DecorationListViewItem dlvi in mSlideMediaListView.Items)
                        {
                            if (dlvi.mForDecorations.Contains(selectedDecoration))
                            {
                                selectedDecor = true;
                                dlvi.Selected = true;
                                mSlideMediaListView.Select();
                            }
                        }
                    }
                    
                    // if nothing selected for whatever reason, just select first item
                    if (selectedDecor==false)
                    {
                        selectedDecor = true;
                        mSlideMediaListView.Items[0].Selected = true;
                        mSlideMediaListView.Select();
                    }
                }
                else
                {
                    // force update anyway
                    mSlideMediaListView_SelectedIndexChanged(this, new EventArgs());
                }
            }
            catch (Exception e)
            {
                this.ResumeLayout();
            }

            mSlideMediaListView.DoubleClick += new EventHandler(mSlideMediaListView_DoubleClick);

            return selectedDecor;
        }


        //*************************************************************************************************
        void mSlideMediaListView_DoubleClick(object sender, EventArgs e)
        {
            //Return if the items are not selected in the ListView control.
            if (mSlideMediaListView.SelectedItems.Count == 0)
            {
                return;
            }
            //Returns the location of the mouse pointer in the ListView control.
            Point cp = mSlideMediaListView.PointToClient(Cursor.Position);
            //Obtain the item that is located at the specified location of the mouse pointer.
            DecorationListViewItem dragToItem = mSlideMediaListView.GetItemAt(cp.X, cp.Y) as DecorationListViewItem;
            if (dragToItem == null)
            {
                return;
            }
            else
            {
                if (MediaSourceChangeRequest != null)
                {

                    if (dragToItem.mForDecorations.Count > 0)
                    {
                        if (dragToItem.mForDecorations[0] is CTextDecoration == false)
                        {
                            MediaSourceChangeRequest(dragToItem.mForDecorations[0]);
                        }
                    }
                }
            }
        }

        //*************************************************************************************************
        public void UpdateThumbnailImage(CDecoration dec, Bitmap b)
        {
            foreach (DecorationListViewItem dlvi in mSlideMediaListView.Items)
            {
                if (dlvi.mForDecorations[0] == dec)
                {
                    int index = dlvi.ImageIndex;
                    Image thumbnail = GenerateThumbnailImageFor(b, mImageList.Images[dlvi.ImageIndex] as Bitmap);
                    mImageList.Images[dlvi.ImageIndex] = thumbnail;
                    mSlideMediaListView.RedrawItems(dlvi.ImageIndex, dlvi.ImageIndex, false);

                    // ensure tool tip text is correct
                    if (dec.HasTemplateImageFilename() == false)
                    {
                        dlvi.ToolTipText = mEditImageToolTipText;
                    }
                    break;
                }
            }
        }

        //*************************************************************************************************
        private Image GenerateThumbnailImageFor(Image b, Bitmap thumb)
        {
            Image rawimage = b;

            if (rawimage == null)
            {
                ListView.SelectedListViewItemCollection selected = mSlideMediaListView.SelectedItems;

                if (selected.Count > 0)
                {
                    DecorationListViewItem dlvi = selected[0] as DecorationListViewItem;
                    if (dlvi != null)
                    {
                        rawimage = DecorationThumbnailGenerator.GenerateThumbnailForDecoration(dlvi.mForDecorations[0], mThumbnailWidth, mThumbnailHeight, null, true, false, true);
                    }
                }
            }

            if (rawimage != null)
            {
                // ok store this in listview image

                Bitmap thumbnail = thumb;

                if (thumbnail == null)
                {
                    thumbnail= new Bitmap(mThumbnailWidth, mThumbnailHeight, PixelFormat.Format32bppArgb);
                }

                using (Graphics g = Graphics.FromImage(thumbnail))
                {
                    g.Clear(mSlideMediaListView.BackColor);
                    Rectangle r = CGlobals.CalcBestFitRectagle(
                        new Rectangle(0,0, mThumbnailWidth, mThumbnailHeight),
                        new Rectangle(0,0, rawimage.Width, rawimage.Height));

                    g.DrawImage(rawimage,r);
                }

                return thumbnail;

            }
            return null;
        }


        //*************************************************************************************************
        private void mSlideMediaListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selected = mSlideMediaListView.SelectedItems;


            if (selected.Count == 0)
            {
                if (MediaSelectedIndexChanged != null)
                {
                    MediaSelectedIndexChanged(null);
                }

                mUpOrderButton.Enabled = false;
                mDownOrderButton.Enabled = false;
            }
            else
            {
                DecorationListViewItem item = selected[0] as DecorationListViewItem;

                if (MediaSelectedIndexChanged != null)
                {
                    MediaSelectedIndexChanged(item.mForDecorations[0]);
                }

                mUpOrderButton.Enabled = GetPreviewMatchingListViewItem(item) != null;
                mDownOrderButton.Enabled = GetNextMatchingListViewItem(item) != null;
            }
        }        

        //*************************************************************************************************
        private DecorationListViewItem GetPreviewMatchingListViewItem(DecorationListViewItem item)
        {
            if (item.mForDecorations[0] is CTextDecoration) return null;

            int index = mSlideMediaListView.Items.IndexOf(item);
            if (index > 0)
            {
                for (int i = index - 1; i >= 0; i--)
                {
                    DecorationListViewItem previousItem = mSlideMediaListView.Items[i] as DecorationListViewItem;
                    if (previousItem.mForDecorations[0] is CClipArtDecoration ||
                        previousItem.mForDecorations[0] is CVideoDecoration)
                    {
                        return previousItem;
                    }
                }
            }
            return null;
        }

        //*************************************************************************************************
        private DecorationListViewItem GetNextMatchingListViewItem(DecorationListViewItem item)
        {
            if (item.mForDecorations[0] is CTextDecoration) return null;

            int index = mSlideMediaListView.Items.IndexOf(item);
            if (index >= 0 && index < mSlideMediaListView.Items.Count-1)
            {
                for (int i = index + 1; i < mSlideMediaListView.Items.Count; i++)
                {
                    DecorationListViewItem nextItem = mSlideMediaListView.Items[i] as DecorationListViewItem;
                    if (nextItem.mForDecorations[0] is CClipArtDecoration ||
                        nextItem.mForDecorations[0] is CVideoDecoration)
                    {
                        return nextItem;
                    }
                }
            }
            return null;
        }

        //*************************************************************************************************
        private void mUpOrderButton_Click(object sender, EventArgs e)
        {
             ListView.SelectedListViewItemCollection selected = mSlideMediaListView.SelectedItems;

             if (selected.Count >= 0)
             {
                 DecorationListViewItem lvrt = selected[0] as DecorationListViewItem;

                 DecorationListViewItem previouslvrt = GetPreviewMatchingListViewItem(lvrt);

                 if (previouslvrt != null)
                 {
                     SwapImages(lvrt, previouslvrt);  
                 }
             }
        }

        //*************************************************************************************************
        private void mDownOrderButton_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selected = mSlideMediaListView.SelectedItems;

            if (selected.Count > 0)
            {
                DecorationListViewItem lvrt = selected[0] as DecorationListViewItem;

                DecorationListViewItem nextlvrt = GetNextMatchingListViewItem(lvrt);

                if (nextlvrt != null)
                {
                    SwapImages(lvrt, nextlvrt);
                }
            }
        }


        //*************************************************************************************************
        private void SwapImages(DecorationListViewItem lvrt, DecorationListViewItem otherlvrt)
        {
            try
            {
                if (lvrt.mForDecorations[0] is CTextDecoration || otherlvrt.mForDecorations[0] is CTextDecoration)
                {
                    return;
                }

                if (lvrt.mForDecorations.Count <= 0 || otherlvrt.mForDecorations.Count <= 0)
                {
                    Log.Error("Missing decoration, can not swap in EditTemplateMediaOrder");
                    return;
                }

                bool eventSwap = lvrt.mForDecorations.Count == otherlvrt.mForDecorations.Count;

                // ### SRG TODO G504
                // don't support swapping uneven decors when one or both are video decors 
                if ((lvrt.mForDecorations[0] is CVideoDecoration ||
                    otherlvrt.mForDecorations[0] is CVideoDecoration) &&
                    eventSwap== false)
                {
                    return ;
                }


                CImageDecoration lvrtOrig=null;
                CImageDecoration otherlvrtOrig=null;
                if (eventSwap==false)
                {
                     lvrtOrig = lvrt.mForDecorations[0].XMLClone() as CImageDecoration;
                     otherlvrtOrig = otherlvrt.mForDecorations[0].XMLClone() as CImageDecoration;
                     if (lvrtOrig == null || otherlvrtOrig == null)
                     {
                        return;
                     }
                }

            

                int max = lvrt.mForDecorations.Count;
                if (otherlvrt.mForDecorations.Count > max)
                {
                    max = otherlvrt.mForDecorations.Count;
                }

                for (int i = 0; i < max; i++)
                {
                    CImageDecoration decorA = null;
                    CImageDecoration decorB = null;

                    CClipArtDecoration cad = null;
                    CClipArtDecoration othercad = null;

                    if (i >= lvrt.mForDecorations.Count && lvrtOrig!=null)
                    {
                        cad = lvrtOrig as CClipArtDecoration;
                        if (cad != null)
                        {
                            cad = cad.XMLClone() as CClipArtDecoration; ;
                        }
                    }
                    else
                    {
                        decorA = lvrt.mForDecorations[i] as CImageDecoration;
                        cad = lvrt.mForDecorations[i] as CClipArtDecoration;
                    }

                    if (i >= otherlvrt.mForDecorations.Count && otherlvrtOrig!=null)
                    {
                        othercad = otherlvrtOrig as CClipArtDecoration;
                        if (othercad != null)
                        {
                            othercad = othercad.XMLClone() as CClipArtDecoration;
                        }
                    }
                    else
                    {
                        decorB = otherlvrt.mForDecorations[i] as CImageDecoration;
                        othercad = otherlvrt.mForDecorations[i] as CClipArtDecoration;
                    }

                    // ok simple case, both decors are cad's
                    if (cad != null && othercad != null)
                    {
                        CImage tempImage = cad.mImage;

                        cad.mImage = othercad.mImage;
                        cad.Brightness = cad.mImage.Brightness;
                        cad.Contrast = cad.mImage.Contrast;
                        cad.BlackAndWhite = cad.mImage.BlackAndWhite;

                        othercad.mImage = tempImage;
                        othercad.Brightness = othercad.mImage.Brightness;
                        othercad.Contrast = othercad.mImage.Contrast;
                        othercad.BlackAndWhite = othercad.mImage.BlackAndWhite;
                    }
                    else
                    {
                        // Ok different types i.e. one is VideoDecor and other is CAD
                        // All we need to do is swap in/out effect, coverage, start/end times and render type

                        mForSlide.SwapDecorations(decorA, decorB);

                        RectangleF coverageA = decorA.CoverageArea;
                        CImageDecoration.RenderMethodType rendertypeA = decorA.RenderMethod;
                        CAnimatedDecorationEffect ineffectA = decorA.MoveInEffect;
                        CAnimatedDecorationEffect outeffectA = decorA.MoveOutEffect;
                        float startTimeA = decorA.StartOffsetTimeRawValue;
                        float endTimeA = decorA.EndOffsetTimeRawValue;
                        CImage alphaMapA = decorA.AlphaMap;

                        decorA.CoverageArea = decorB.CoverageArea;
                        decorA.RenderMethod = decorB.RenderMethod;
                        decorA.MoveInEffect = decorB.MoveInEffect;
                        decorA.MoveOutEffect = decorB.MoveOutEffect;
                        decorA.StartOffsetTimeRawValue = decorB.StartOffsetTimeRawValue;
                        decorA.EndOffsetTimeRawValue = decorB.EndOffsetTimeRawValue;
                        decorA.AlphaMap = decorB.AlphaMap;

                        decorB.CoverageArea = coverageA;
                        decorB.RenderMethod = rendertypeA;
                        decorB.MoveInEffect = ineffectA;
                        decorB.MoveOutEffect = outeffectA;
                        decorB.StartOffsetTimeRawValue = startTimeA;
                        decorB.EndOffsetTimeRawValue = endTimeA;
                        decorB.AlphaMap = alphaMapA;

                        if (i < lvrt.mForDecorations.Count)
                        {
                            lvrt.mForDecorations[i] = decorB;
                        }

                        if (i < otherlvrt.mForDecorations.Count)
                        {
                            otherlvrt.mForDecorations[i] = decorA;
                        }
                    }
                }

                // swap images in list view
                int indexa = mSlideMediaListView.Items.IndexOf(lvrt);
                int indexb = mSlideMediaListView.Items.IndexOf(otherlvrt);

                if (indexa >= 0 && indexb >= 0)
                {
                    Image tempThumb = mImageList.Images[indexa];
                    mImageList.Images[indexa] = mImageList.Images[indexb];
                    mImageList.Images[indexb] = tempThumb;

                    mSlideMediaListView.RedrawItems(0, mSlideMediaListView.Items.Count - 1, false);
                }

                otherlvrt.Selected = true;
                mSlideMediaListView.Select();
            }
            catch (Exception e)
            {
                throw e;
            }
          
        }

        
        //*************************************************************************************************
        public void  ReplaceDecoration(CDecoration oldDecor, CDecoration newDecor)
        {
            foreach (DecorationListViewItem dlvi in mSlideMediaListView.Items)
            {
                for (int i = 0; i < dlvi.mForDecorations.Count; i++)
                {
                    if (dlvi.mForDecorations[i] == oldDecor)
                    {
                        dlvi.mForDecorations[i] = newDecor;
                        return;
                    }
                }
            }
            Log.Warning("Edit template media order did not replace decoration");
        }

        //*************************************************************************************************
        private void mSlideMediaListView_DragEnter(object sender, DragEventArgs e)
        {
            int len = e.Data.GetFormats().Length - 1;
            int i;
            for (i = 0; i <= len; i++)
            {
                if (e.Data.GetFormats()[i].Equals("System.Windows.Forms.ListView+SelectedListViewItemCollection"))
                {
                    //The data from the drag source is moved to the target.	
                    e.Effect = DragDropEffects.Move;
                }
            }
        }

        //*************************************************************************************************
        private void mSlideMediaListView_DragDrop(object sender, DragEventArgs e)
        {
            //Return if the items are not selected in the ListView control.
            if (mSlideMediaListView.SelectedItems.Count == 0)
            {
                return;
            }
            //Returns the location of the mouse pointer in the ListView control.
            Point cp = mSlideMediaListView.PointToClient(new Point(e.X, e.Y));
            //Obtain the item that is located at the specified location of the mouse pointer.
            DecorationListViewItem dragToItem = mSlideMediaListView.GetItemAt(cp.X, cp.Y) as DecorationListViewItem;
            if (dragToItem == null)
            {
                return;
            }

            DecorationListViewItem toMove = mSlideMediaListView.SelectedItems[0] as DecorationListViewItem;

            if (toMove != dragToItem)
            {
                if (toMove.mForDecorations[0] is CImageDecoration &&
                   dragToItem.mForDecorations[0] is CImageDecoration)
                {
                    SwapImages(toMove, dragToItem);
                }
            }
        }

        //*************************************************************************************************
        private void mSlideMediaListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            mSlideMediaListView.DoDragDrop(mSlideMediaListView.SelectedItems, DragDropEffects.Move);
        }
    }
}
