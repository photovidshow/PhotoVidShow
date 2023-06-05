using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CustomButton;
using DVDSlideshow;
using System.IO;
using System.Collections;

namespace CustomButton
{
    public delegate void RebuildGuiSlideshowCallbackDelegate();

    public partial class SlideFiltersControl : FilterTabControlBase
    {
        public delegate void ChangedSlideCallback();

        public event ChangedSlideCallback ChangedSlide;

        private bool mSwitchOffSelectIndexChange = false; // hack to stop doing a change when switch list views
        private CAnimatedDecoration mAuxilairyOptionDecor = null;
        private CImageSlide mAuxilairyOptionSlide = null;

        private const float mSoftBlur =0;
        private const float mMediumBlur =1.0f;
        private const float mHardBlur =2.0f;

        //******************************************************************************************
        public SlideFiltersControl()
        {
            InitializeComponent();

            bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);

            if (designMode == true) return;

            PopulateCandidateFiltersListView(mFiltersListView, CGlobals.GetFiltersDirectory(), true, mFilterImageList);

            // move blur group box into final position
            this.mBlurPanel.Location = this.mBrightnessContrastControl.Location;
            this.mPanZoomOnAllCheckBox.Location = this.mBrightnessContrastControl.Location;
        }

      
        //******************************************************************************************
        private void mOrderLayersButton_Click(object sender, EventArgs e)
        {
            OrderLayersForm olf = new OrderLayersForm(mCurrentEditingSlide as CImageSlide, mCurrentEditingSlideshow);
            olf.ShowDialog();
            UpdateCurrentFiltersListView();
            HideAllAuxilairyOptions();
            mCurrentFiltersListView_SelectedIndexChanged(sender, e);
            if (ChangedSlide != null)
            {
                ChangedSlide();
            }
        }

        //******************************************************************************************
        private void HideAllAuxilairyOptions()
        {
            this.SuspendLayout();
            this.mBlurPanel.Visible = false;
            this.mBrightnessContrastControl.Visible = false;
            this.mPanZoomOnAllCheckBox.Visible = false;
          
            mAuxilairyOptionDecor = null;
            mAuxilairyOptionSlide = null;
            this.ResumeLayout();
        }

        //******************************************************************************************
        private void ShowAuxilairyOptions(Control control, CAnimatedDecoration forDecor, CImageSlide forSlide)
        {
            this.SuspendLayout();
            try
            {
                mAuxilairyOptionDecor = forDecor;
                mAuxilairyOptionSlide = forSlide;

                bool showControl = false;
     
                // blur filter option
                if (control != mBlurPanel)
                {
                    mBlurPanel.Visible = false;
                }
                else
                {
                    showControl = true;

                    CBlurFilterDecoration blurDecoration = forDecor as CBlurFilterDecoration;
                    if (blurDecoration == null)
                    {
                        mBlurMediumRadio.Checked = true;
                        mNormalBlurRadioButton.Checked = true;
                    }
                    else
                    {
                        if (blurDecoration.Strength == mHardBlur)
                        {
                            mBlurHardRadio.Checked = true;
                        }
                        else if (blurDecoration.Strength == mMediumBlur)
                        {
                            mBlurMediumRadio.Checked = true;
                        }
                        else
                        {
                            mBlurSoftRadio.Checked = true;
                        }

                        if (blurDecoration.BlurFilterType == CBlurFilterDecoration.BlurType.NORMAL)
                        {
                            mNormalBlurRadioButton.Checked = true;
                        }
                        else
                        {
                            mLensBlurRadioButton.Checked = true;
                        }
                    }
                }

                // pan zoom on all filter option
                if (control != mPanZoomOnAllCheckBox)
                {
                    mPanZoomOnAllCheckBox.Visible = false;
                }
                else
                {
                    if (mCurrentEditingSlide.UsePanZoom == false)
                    {
                        mPanZoomOnAllCheckBox.Visible = false;
                    }
                    else
                    {
                        showControl = true;

                        CImageSlide imageSlide = mCurrentEditingSlide as CImageSlide;
                        if (imageSlide !=null)
                        {
                            mPanZoomOnAllCheckBox.Checked = imageSlide.PanZoom.PanZoomOnAll;
                        }
                    }
                }

              
                // adjust colour filter option
                if (control != mBrightnessContrastControl)
                {
                    mBrightnessContrastControl.Visible = false;
                }
                else
                {
                    showControl = true;

                    CColourTransformDecoration colorDecor = forDecor as CColourTransformDecoration;

                    if (colorDecor == null)
                    {
                        mBrightnessContrastControl.BrightnessR = 1;
                        mBrightnessContrastControl.BrightnessG = 1;
                        mBrightnessContrastControl.BrightnessB = 1;
                        mBrightnessContrastControl.ContrastR = 1;
                        mBrightnessContrastControl.ContrastG = 1;
                        mBrightnessContrastControl.ContrastB = 1;
                    }
                    else
                    {
                        mBrightnessContrastControl.BrightnessR = colorDecor.BrightnessR;
                        mBrightnessContrastControl.BrightnessG = colorDecor.BrightnessG;
                        mBrightnessContrastControl.BrightnessB = colorDecor.BrightnessB;
                        mBrightnessContrastControl.ContrastR = colorDecor.ContrastR;
                        mBrightnessContrastControl.ContrastG = colorDecor.ContrastG;
                        mBrightnessContrastControl.ContrastB = colorDecor.ContrastB;

                        if (colorDecor.BrightnessR != colorDecor.BrightnessG ||
                            colorDecor.BrightnessR != colorDecor.BrightnessB ||
                            colorDecor.ContrastR != colorDecor.ContrastG ||
                            colorDecor.ContrastR != colorDecor.ContrastB)
                        {
                            mBrightnessContrastControl.ChangeIndividialChannelsCheckBox.Checked = true;
                        }
                        else
                        {
                            mBrightnessContrastControl.ChangeIndividialChannelsCheckBox.Checked = false;
                        }
                    }      
                }

                if (showControl == true && control != null)
                {
                    control.Visible = true;
                }
            }
            finally
            {
                this.ResumeLayout();
            }
        }

        //******************************************************************************************
        private CAnimatedDecoration CreateDecorationForSelected(FilterListViewItem filterSelectedItem)
        {
            if (filterSelectedItem.mType == FilterListViewItem.FilterType.ColourAdjust)
            {
                CColourTransformDecoration decor = new CColourTransformDecoration(0);
                return decor;

            }
            else if (filterSelectedItem.mType == FilterListViewItem.FilterType.Blur)
            {
                CBlurFilterDecoration bfd = new CBlurFilterDecoration(0);
                return bfd;

            }
            else if (filterSelectedItem.mType == FilterListViewItem.FilterType.Monotone)
            {
                CMonotoneTransformDecoration mfd = new CMonotoneTransformDecoration(0);
                return mfd;
            }
            else if (filterSelectedItem.mType == FilterListViewItem.FilterType.Sepia)
            {
                CSepiaTransformDecoration std = new CSepiaTransformDecoration(0);
                return std;
            }
            else
            {
                CImageDecoration decToAdd = null;
                if (filterSelectedItem.mType == FilterListViewItem.FilterType.Video)
                {
                    decToAdd = CGlobalVideoDatabase.GetInstance().CreateVideoDecoration(filterSelectedItem.mFilename);
                }
                else
                {
                    decToAdd = new CClipArtDecoration(filterSelectedItem.mFilename, new RectangleF(0, 0, 1, 1), 0);
                }

                decToAdd.RenderMethod = filterSelectedItem.mRenderMethod;
                return decToAdd;
            }
        }

      

        //******************************************************************************************
        private void mFiltersListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mCurrentEditingSlide == null) return;

            if (mSwitchOffSelectIndexChange == true) return;
            this.SuspendLayout();
            try
            {
                CurrentFiltersSelectNothing(false);

                EnableCurrentFilterButtons(false, null);
                MiniPreviewController.StopAnyPlayingController(); 

                ListView.SelectedListViewItemCollection collection = mFiltersListView.SelectedItems;
                if (collection.Count == 0)
                {
                    HideAllAuxilairyOptions();

                    mPreviewController = new MiniPreviewController(
                         mCurrentEditingSlideshow,
                         mCurrentEditingSlide,
                         mPreviewInControl);


                    this.mAddFilterButton.Enabled = false;

                    return;
                }

                this.mAddFilterButton.Enabled = true;

                ListViewItem selectedItem = collection[0];

                CImageSlide previewSlide = mCurrentEditingSlide.XMLClone() as CImageSlide;
                CSlideShow previewSlideshow = new CSlideShow("FiltersPreviewSlideshow");
                previewSlideshow.FadeIn = false;
                previewSlideshow.FadeOut = false;

                previewSlideshow.mSlides.Add(previewSlide);
                previewSlideshow.InValidateLength();

                FilterListViewItem filterSelectedItem = selectedItem as FilterListViewItem;

                CAnimatedDecoration decToAdd = CreateDecorationForSelected(filterSelectedItem);
                previewSlide.AddDecoration(decToAdd);

                if (filterSelectedItem.mType == FilterListViewItem.FilterType.ColourAdjust)
                {
                    ShowAuxilairyOptions(mBrightnessContrastControl, decToAdd, previewSlide);
                }
                else if (filterSelectedItem.mType == FilterListViewItem.FilterType.Blur)
                {
                    ShowAuxilairyOptions(mBlurPanel, decToAdd, previewSlide);
                }
                else if (filterSelectedItem.mType == FilterListViewItem.FilterType.Image)
                {
                    ShowAuxilairyOptions(mPanZoomOnAllCheckBox, decToAdd, previewSlide);
                }

                mPreviewController = new MiniPreviewController(
                    previewSlideshow,
                    previewSlide,
                    mPreviewInControl);
            }
            finally
            {
                this.ResumeLayout();
            }
        }

        //*************************************************************************************************
        private void mFiltersListView_Enter(object sender, EventArgs e)
        {
            mFiltersListView_SelectedIndexChanged(sender, e);
        }


        //*************************************************************************************************
        public void SetSlide(CSlide slide, CSlideShow slideshow, RebuildGuiSlideshowCallbackDelegate callback, Control previewControl)
        {
            mPreviewInControl = previewControl;
            mGuiRebuildSlideshowCallback = callback;
            mCurrentEditingSlide = slide;
            mCurrentEditingSlideshow = slideshow;
            UpdateCurrentFiltersListView();
            CandidateFiltersSelectNothing(true);
        }


        //*************************************************************************************************
        private void mBlurSoftRadio_CheckedChanged(object sender, EventArgs e)
        {
            CBlurFilterDecoration decor = mAuxilairyOptionDecor as CBlurFilterDecoration;
            if (decor != null && mBlurSoftRadio.Checked==true)
            {
                decor.Strength = mSoftBlur;
                AuxilaryOptionChanged();
            }
        }


        //*************************************************************************************************
        private void mBlurMediumRadio_CheckedChanged(object sender, EventArgs e)
        {
            CBlurFilterDecoration decor = mAuxilairyOptionDecor as CBlurFilterDecoration;
            if (decor != null && mBlurMediumRadio.Checked == true)
            {
                decor.Strength = mMediumBlur;
                AuxilaryOptionChanged();
            }
        }


        //*************************************************************************************************
        private void mBlurHardRadio_CheckedChanged(object sender, EventArgs e)
        {
            CBlurFilterDecoration decor = mAuxilairyOptionDecor as CBlurFilterDecoration;
            if (decor != null && mBlurHardRadio.Checked==true)
            {
                decor.Strength = mHardBlur;
                AuxilaryOptionChanged();
            }
        }

        //*************************************************************************************************
        private void CandidateFiltersSelectNothing(bool update)
        {
            if (update == false)
            {
                mSwitchOffSelectIndexChange = true;
            }
            try
            {

                // select nothing, better way of doing it?
                ListView.SelectedListViewItemCollection collection = mFiltersListView.SelectedItems;
                if (collection.Count != 0)
                {
                    collection[0].Selected = false;
                }
                else if (update == true)
                {
                    mFiltersListView_SelectedIndexChanged(this, new EventArgs());
                }
            }
            finally
            {
                mSwitchOffSelectIndexChange = false;
            }

        }

        //*************************************************************************************************
        private void mAddFilterButton_Click(object sender, EventArgs e)
        {
            if (mCurrentEditingSlide != null)
            {
                ListView.SelectedListViewItemCollection collection = mFiltersListView.SelectedItems;

                if (collection.Count > 0)
                {
                    FilterListViewItem filterSelectedItem = collection[0] as FilterListViewItem;
                    if (filterSelectedItem != null)
                    {
                        CAnimatedDecoration decor= null;
                        if (mAuxilairyOptionDecor != null)
                        {
                            decor = mAuxilairyOptionDecor;
                            mAuxilairyOptionDecor = null;
                        }
                        else
                        {
                            decor = CreateDecorationForSelected(filterSelectedItem);
                        }

                        CImageSlide currentImageSlide = mCurrentEditingSlide as CImageSlide;

                        if (decor.VerfifyAllMediaFilesToRenderThisExist() == true)
                        {
                            currentImageSlide.AddDecoration(decor);
                        }
                        CandidateFiltersSelectNothing(true);
                        UpdateCurrentFiltersListView();
                        if (ChangedSlide != null)
                        {
                            ChangedSlide();
                        }
                    }
                }
            }
        }

        //*************************************************************************************************
        private bool IsFilterRenderType(CImageDecoration.RenderMethodType renderType)
        {
            foreach (CImageDecoration.RenderMethodType type in mRenderMethods)
            {
                if (type == renderType)
                {
                    return true;
                }
            }
            return false;
        }

        //*************************************************************************************************
        private void UpdateCurrentFiltersListView()
        {
            if (mCurrentEditingSlide == null) return;

            CImageSlide currentImageEditingSlide = mCurrentEditingSlide as CImageSlide;

            this.SuspendLayout();
            try
            {
                EnableCurrentFilterButtons(false, null);
                mCurrentFiltersListView.Clear();

                ArrayList decors = currentImageEditingSlide.GetAllAndSubDecorations();

                // do in reverse order
                for (int i= decors.Count-1; i >=0;i--)
                {
                    CDecoration dec = decors[i] as CDecoration ;

                    CBlurFilterDecoration blurDector = dec as CBlurFilterDecoration;

                    if (blurDector != null)
                    {
                        FilterListViewItem blurListViewItem = CreateBlurFilterListItem();
                        blurListViewItem.mDecoration = dec;   
                        mCurrentFiltersListView.Items.Add(blurListViewItem);
                        continue;
                    }
                    
                    CColourTransformDecoration colourTransformDecor = dec as CColourTransformDecoration;

                    if (colourTransformDecor != null)
                    {
                        // colur transform
                        FilterListViewItem colourAdjustListViewItem = CreateColourAdjustFilterListItem();
                        colourAdjustListViewItem.mDecoration = dec;
                        mCurrentFiltersListView.Items.Add(colourAdjustListViewItem);
                        continue;
                    }

                    CMonotoneTransformDecoration monotoneTransformDecor = dec as CMonotoneTransformDecoration;

                    if (monotoneTransformDecor != null)
                    {
                        // monotone transform
                        FilterListViewItem monotoneListViewItem = CreateMonotoneFilterListItem();
                        monotoneListViewItem.mDecoration = dec;
                        mCurrentFiltersListView.Items.Add(monotoneListViewItem);
                        continue;
                    }

                    CSepiaTransformDecoration sepiaTransformDecor = dec as CSepiaTransformDecoration;
                    if (sepiaTransformDecor != null)
                    {
                        // monotone transform
                        FilterListViewItem sepiaListViewItem = CreateSepiaFilterListItem();
                        sepiaListViewItem.mDecoration = dec;
                        mCurrentFiltersListView.Items.Add(sepiaListViewItem);
                        continue;
                    }


                    // file based filter

                    string filename = "Unknown filter";
                    bool valid_filter = false;
                    FilterListViewItem.FilterType type = FilterListViewItem.FilterType.Image;

                    CImageDecoration imageDecoration = dec as CImageDecoration;
                    if (imageDecoration.IsBorderDecoration() == false &&
                        IsFilterRenderType(imageDecoration.RenderMethod) )
                    {

                        CClipArtDecoration clipartDecoration = dec as CClipArtDecoration;
                        if (clipartDecoration !=null)
                        {
                            filename = clipartDecoration.mImage.ImageFilename;
                            valid_filter = true;
                        }
                        else
                        {
                            CVideoDecoration vd = dec as CVideoDecoration ;
                            if (vd != null)
                            {                    
                                filename = vd.GetFilename();
                                valid_filter = true;
                                type = FilterListViewItem.FilterType.Video;
                            }
                        }
                    }

                
                    if (valid_filter == true)
                    {
                       
                        filename = System.IO.Path.GetFileNameWithoutExtension(filename);

                        // strip render type of text
                        for (int count = 0; count < mRenderMethodString.Length; count++)
                        {
                            if (filename.Contains(mRenderMethodString[count]) == true)
                            {
                                filename = filename.Replace(mRenderMethodString[count], "");
                                break;
                            }
                        }

                        FilterListViewItem lvu = new FilterListViewItem(filename, "");
                        lvu.mType = type;
                        lvu.mDecoration = dec;
                        SetImageIndexForFilterType(lvu);
                        mCurrentFiltersListView.Items.Add(lvu);
                   }
                }
            }
            finally
            {
                this.ResumeLayout();
            }  
        }

        //*************************************************************************************************
        private void mCurrentFiltersListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mSwitchOffSelectIndexChange == true) return;

            this.SuspendLayout();
            try
            {
                CandidateFiltersSelectNothing(false);
                this.mAddFilterButton.Enabled = false;
                MiniPreviewController.StopAnyPlayingController(); 

                ListView.SelectedListViewItemCollection collection = this.mCurrentFiltersListView.SelectedItems;
                if (collection.Count == 0)
                {
                    EnableCurrentFilterButtons(false, null);
                    HideAllAuxilairyOptions();
                }
                else
                {
                    FilterListViewItem filterSelectedItem = collection[0] as FilterListViewItem;

                    if (filterSelectedItem.mType == FilterListViewItem.FilterType.ColourAdjust)
                    {
                        ShowAuxilairyOptions(mBrightnessContrastControl, filterSelectedItem.mDecoration as CAnimatedDecoration, mCurrentEditingSlide as CImageSlide);
                    }
                    else if (filterSelectedItem.mType == FilterListViewItem.FilterType.Blur)
                    {
                        ShowAuxilairyOptions(mBlurPanel, filterSelectedItem.mDecoration as CAnimatedDecoration, mCurrentEditingSlide as CImageSlide);
                    }
                    else if (filterSelectedItem.mType == FilterListViewItem.FilterType.Image)
                    {
                        ShowAuxilairyOptions(mPanZoomOnAllCheckBox, filterSelectedItem.mDecoration as CAnimatedDecoration, mCurrentEditingSlide as CImageSlide);
                    }
                    else
                    {
                        HideAllAuxilairyOptions();
                    }

                    EnableCurrentFilterButtons(true, filterSelectedItem);
                }

                mPreviewController = new MiniPreviewController(
                     mCurrentEditingSlideshow,
                     mCurrentEditingSlide,
                     mPreviewInControl);
            }
            finally
            {
                this.ResumeLayout();
            }
        }

        //*************************************************************************************************
        private void mCurrentFiltersListView_Enter(object sender, EventArgs e)
        {
            mCurrentFiltersListView_SelectedIndexChanged(sender, e);
        }

        //*************************************************************************************************
        private void CurrentFiltersSelectNothing(bool update)
        {
            if (update == false)
            {
                mSwitchOffSelectIndexChange = true;
            }

            try
            {
                // select nothing, better way of doing it?
                ListView.SelectedListViewItemCollection collection = mCurrentFiltersListView.SelectedItems;
                if (collection.Count != 0)
                {
                    collection[0].Selected = false;
                }
                else if (update==true)
                {
                    mCurrentFiltersListView_SelectedIndexChanged(this, new EventArgs());
                }
            }
            finally
            {
                mSwitchOffSelectIndexChange = false;
            }
        }

        //*************************************************************************************************
        private void mRemoveFilterButton_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection collection = this.mCurrentFiltersListView.SelectedItems;
            if (collection.Count != 0)
            {
                FilterListViewItem flvi = collection[0] as FilterListViewItem;

                CDecoration decorToRemove = flvi.mDecoration;
                if (decorToRemove != null &&
                    mCurrentEditingSlide != null)
                {
                    CImageSlide imageSlide = mCurrentEditingSlide as CImageSlide;
                    imageSlide.RemoveDecoration(decorToRemove);

                    UpdateCurrentFiltersListView();
                    this.HideAllAuxilairyOptions();

                    if (ChangedSlide != null)
                    {
                        ChangedSlide();
                    }
                }
            }
        }

        //*************************************************************************************************
        private void EnableCurrentFilterButtons(bool value, ListViewItem selectedItem)
        {
            this.mRemoveFilterButton.Enabled = value;

            if (value == false)
            {
                this.mUpFilterOrderButton.Enabled = value;
                this.mDownFilterOrderButton.Enabled = value;
            }

            if (value == true)
            {
                
                ListView.ListViewItemCollection collection = this.mCurrentFiltersListView.Items;
                if (collection.Count != 0)
                {
                    // can move up order?
                    ListViewItem first_flvi = collection[0];
                    if (first_flvi != selectedItem)
                    {
                        this.mUpFilterOrderButton.Enabled = true;
                    }
                    else
                    {
                        this.mUpFilterOrderButton.Enabled = false;
                    }

                    // can we move down order?
                    ListViewItem last_flvi = collection[collection.Count - 1] as ListViewItem;
                    if (last_flvi != selectedItem)
                    {
                        this.mDownFilterOrderButton.Enabled = true;
                    }
                    else
                    {
                        this.mDownFilterOrderButton.Enabled = false;
                    }
                }
                else
                {
                    this.mUpFilterOrderButton.Enabled = false;
                    this.mDownFilterOrderButton.Enabled = false;
                }
            }
        }



        //*************************************************************************************************
        private void mUpFilterOrderButton_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selectedCollection = mCurrentFiltersListView.SelectedItems;
            ListView.ListViewItemCollection collection = mCurrentFiltersListView.Items;
            if (selectedCollection.Count != 0 && mCurrentEditingSlide != null)
            {
                FilterListViewItem selectedItem = selectedCollection[0] as FilterListViewItem;

                int selectedIndex = collection.IndexOf(selectedItem);
                if (selectedIndex > 0 && selectedIndex < collection.Count)
                {
                    CDecoration decorA = selectedItem.mDecoration;
                    CDecoration decorB = (collection[selectedIndex - 1] as FilterListViewItem).mDecoration;

                    CImageSlide imageSlide = mCurrentEditingSlide as CImageSlide;
                    imageSlide.SwapDecorations(decorA, decorB);

                    UpdateCurrentFiltersListView();

                    // re-select item in newly create list
                    collection = mCurrentFiltersListView.Items;
                    if (selectedIndex < collection.Count)
                    {
                        collection[selectedIndex - 1].Selected = true;
                        mCurrentFiltersListView.Select();
                    }

                    if (ChangedSlide != null)
                    {
                        ChangedSlide();
                    }
                }
            }
        }

        //*************************************************************************************************
        private void mDownFilterOrderButton_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selectedCollection = mCurrentFiltersListView.SelectedItems;
            ListView.ListViewItemCollection collection = mCurrentFiltersListView.Items;
            if (selectedCollection.Count != 0 && mCurrentEditingSlide != null)
            {
                FilterListViewItem selectedItem = selectedCollection[0] as FilterListViewItem;

                int selectedIndex = collection.IndexOf(selectedItem);
                if (selectedIndex < collection.Count - 1 && selectedIndex >=0)
                {
                    CDecoration decorA = selectedItem.mDecoration;
                    CDecoration decorB = (collection[selectedIndex + 1] as FilterListViewItem).mDecoration;

                    CImageSlide imageSlide = mCurrentEditingSlide as CImageSlide;
                    imageSlide.SwapDecorations(decorA, decorB);

                    UpdateCurrentFiltersListView();
                    collection = mCurrentFiltersListView.Items;

                    // re-select item in newly create list
                    if (selectedIndex < collection.Count -1)
                    {
                        collection[selectedIndex + 1].Selected = true;
                        mCurrentFiltersListView.Select();
                    }

                    if (ChangedSlide != null)
                    {
                        ChangedSlide();
                    }
                }
            }
        }

        //*************************************************************************************************
        private void mBrightnessContrastControl_BrightnessBChanged()
        {
            CColourTransformDecoration decor = mAuxilairyOptionDecor as CColourTransformDecoration;
            if (decor != null)
            {
                decor.BrightnessB = mBrightnessContrastControl.BrightnessB;
            }
        }

        //*************************************************************************************************
        private void mBrightnessContrastControl_BrightnessGChanged()
        {
            CColourTransformDecoration decor = mAuxilairyOptionDecor as CColourTransformDecoration;
            if (decor != null)
            {
                decor.BrightnessG = mBrightnessContrastControl.BrightnessG;
            }
        }

        //*************************************************************************************************
        private void mBrightnessContrastControl_BrightnessRChanged()
        {
            CColourTransformDecoration decor = mAuxilairyOptionDecor as CColourTransformDecoration;
            if (decor != null)
            {
                decor.BrightnessR = mBrightnessContrastControl.BrightnessR;
            }
        }

        //*************************************************************************************************
        private void mBrightnessContrastControl_ContrastRChanged()
        {
            CColourTransformDecoration decor = mAuxilairyOptionDecor as CColourTransformDecoration;
            if (decor != null)
            {
                decor.ContrastR = mBrightnessContrastControl.ContrastR;
            }
        }

        //*************************************************************************************************
        private void mBrightnessContrastControl_ContrastGChanged()
        {
            CColourTransformDecoration decor = mAuxilairyOptionDecor as CColourTransformDecoration;
            if (decor != null)
            {
                decor.ContrastG = mBrightnessContrastControl.ContrastG;
            }
        }

        //*************************************************************************************************
        private void mBrightnessContrastControl_ContrastBChanged()
        {
            CColourTransformDecoration decor = mAuxilairyOptionDecor as CColourTransformDecoration;
            if (decor != null)
            {
                decor.ContrastB = mBrightnessContrastControl.ContrastB;
            }
        }


        //*************************************************************************************************
        private void mBrightnessContrastControl_BrightnessChanged()
        {
            CColourTransformDecoration decor = mAuxilairyOptionDecor as CColourTransformDecoration;
            if (decor != null)
            {
                decor.BrightnessR = mBrightnessContrastControl.BrightnessR;
                decor.BrightnessG = mBrightnessContrastControl.BrightnessG;
                decor.BrightnessB = mBrightnessContrastControl.BrightnessB;
            }
        }


        //*************************************************************************************************
        private void mBrightnessContrastControl_ContrastChanged()
        {
            CColourTransformDecoration decor = mAuxilairyOptionDecor as CColourTransformDecoration;
            if (decor != null)
            {
                decor.ContrastR = mBrightnessContrastControl.ContrastR;
                decor.ContrastG = mBrightnessContrastControl.ContrastG;
                decor.ContrastB = mBrightnessContrastControl.ContrastB;
            }
        }

        //*************************************************************************************************
        private void mNormalBlurRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            CBlurFilterDecoration decor = mAuxilairyOptionDecor as CBlurFilterDecoration;
            if (decor != null && mNormalBlurRadioButton.Checked==true)
            {
                decor.BlurFilterType = CBlurFilterDecoration.BlurType.NORMAL;
                AuxilaryOptionChanged();
            }
        }

        //*************************************************************************************************
        private void mLensBlurRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            CBlurFilterDecoration decor = mAuxilairyOptionDecor as CBlurFilterDecoration;
            if (decor != null && mLensBlurRadioButton.Checked == true)
            {
                decor.BlurFilterType = CBlurFilterDecoration.BlurType.LENS;
                AuxilaryOptionChanged();
            }
        }

        //*************************************************************************************************
        private void mPanZoomOnAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CImageSlide imageSlide = mCurrentEditingSlide as CImageSlide;

            if (imageSlide != null)
            {
                imageSlide.PanZoom.PanZoomOnAll = mPanZoomOnAllCheckBox.Checked;
                mAuxilairyOptionSlide.PanZoom.PanZoomOnAll = mPanZoomOnAllCheckBox.Checked;
                AuxilaryOptionChanged();
            }

        }

        //*************************************************************************************************
        private void ClearSelectedButton_Click(object sender, EventArgs e)
        {
            CurrentFiltersSelectNothing(true);
        }


        //*************************************************************************************************
        private void AuxilaryOptionChanged()
        {
            ListView.SelectedListViewItemCollection collection = this.mCurrentFiltersListView.SelectedItems;
            if (collection.Count != 0)
            {
                if (ChangedSlide != null)
                {
                    ChangedSlide();
                }
            }
        }

        //*************************************************************************************************
        private void mBrightnessContrastControl_FinishedBrightnessContrastChange()
        {
            AuxilaryOptionChanged();
        }
    }
}
