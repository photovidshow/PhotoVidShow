using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;
using DVDSlideshow.GraphicsEng;
using DVDSlideshow.GraphicsEng.Direct3D;
using System.Drawing.Imaging;
using ManagedCore;
using System.IO;
using System.Collections;

namespace CustomButton
{
    public partial class EditSlideMediaForm : Form
    {
        private CDecoration mSelectedDecoration;

        private Image mPreLoadedImage = null;
        private bool mThumbnailNeedsUpdate = false;
        private bool mTimerUpdateWithPB = true;
        private Timer mUpdateThumbnailTimer;
        private Timer mTrimVideoThumnailsTimer;

        private MiniPreviewController mPreviewController = null;
        private CSlideShow mForSlideshow;
        private CImageSlide mForSlide;
        private static OpenFileDialog mOpenFileDialog = null;
        private List<TabPage> mAllTabPages = new List<TabPage>();
        private bool mSlideLengthChanged = false;
        private static bool mPreviewSlide = false;

        public bool SlideLengthChanged
        {
            get { return mSlideLengthChanged; }
        }
     
        //*************************************************************************************************
        public EditSlideMediaForm(CImageSlide forSlide, CSlideShow forSlideshow, CImageDecoration selectDecor)
        {
            mForSlideshow = forSlideshow;

            InitializeComponent();

            // Adjust preview window if 4:3 project
            if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV4_3)
            {
                int oldwidth = this.mPreviewPanel.Width;
                mPreviewPanel.Width = (int)(((float)mPreviewPanel.Height) * 1.33333f);
            }

            if (CGlobals.mIsDebug == true)
            {
                mEncryptFile.Visible = true;
            }

            mTextPBPanel.Location = mPreviewPictureBox.Location;
            mTextPBPanel.Size = mPreviewPictureBox.Size;
            mTextPBPanelDarkBackGround.Location = mPreviewPictureBox.Location;
            mTextPBPanelDarkBackGround.Size = mPreviewPictureBox.Size;

            foreach (TabPage t in mControlTabs.TabPages)
            {
                mAllTabPages.Add(t);
            }
          
            mUpdateThumbnailTimer = new Timer();
            mUpdateThumbnailTimer.Interval = 1000;
            mUpdateThumbnailTimer.Tick += new EventHandler(TimerTick);

            mTrimVideoThumnailsTimer = new Timer();
            mTrimVideoThumnailsTimer.Interval = 1000;
            mTrimVideoThumnailsTimer.Tick += new EventHandler(TrimVideoTimerTick);

            SetForSlide(forSlide, selectDecor);

            if (mOpenFileDialog == null)
            {
                mOpenFileDialog = new OpenFileDialog();
                string myPictures = DefaultFolders.GetFolder("MyPictures");
                mOpenFileDialog.InitialDirectory = myPictures;
                mOpenFileDialog.Title = "Open image or video";
                mOpenFileDialog.Multiselect = false;
                mOpenFileDialog.Filter = CGlobals.GetTotalImageVideoFilter();
            }

            //
            // Don't allow user to preview if it is too slow to do so.
            //
            if (mForSlide != null)
            {
                if (mForSlide.PreviewPlayWhenEditingSlideMedia == false)
                {
                    mShowPreviewCheckBox.Visible = false;
                }
                else
                {
                    mShowPreviewCheckBox.Checked = mPreviewSlide;
                }
            }      
        }

         //*************************************************************************************************
        private void SetForSlide(CImageSlide slide, CImageDecoration selectedImage)
        {
            CustomButton.MiniPreviewController.StopAnyPlayingController();

            mForSlide = slide;
            bool selectedDecor = mEditTemplateMediaOrder1.SetForSlide(mForSlide, selectedImage);

            if (selectedDecor == false)
            {
                mSelectedDecoration = null;
                selectedImage = null;
            }

            int index = mForSlideshow.mSlides.IndexOf(slide);

            if (index < 1)
            {
                mPreviousSlideButton.Enabled = false;
            }
            else
            {
                mPreviousSlideButton.Enabled = true;
            }

            if (index >= mForSlideshow.mSlides.Count - 1)
            {
                mNextSlideButton.Enabled = false;
            }
            else
            {
                mNextSlideButton.Enabled = true;
            }

            EnableDisableDecorationOptions(selectedImage);
        }


        //*************************************************************************************************
        private void mDoneButton_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }

        //*************************************************************************************************
        private void DisposeCurrentPB()
        {
            if (mPreviewPictureBox.Image != null)
            {
                Image i = mPreviewPictureBox.Image;
                mPreviewPictureBox.Image = null;
                i.Dispose();
            }
        }


        //*************************************************************************************************
        private void mEditTemplateMediaOrder1_MediaSelectedIndexChanged(CDecoration decoration)
        {
            if (mSelectedDecoration == decoration)
            {
                return;
            }

            this.SuspendLayout();

            mSelectedDecoration = decoration;

            // reset preview panel/picture box (maybe adjusted by previous item)
            mPreviewPictureBox.Visible = true;
            mTextPBPanel.Visible = false;
            mTextPBPanel.Location = mPreviewPictureBox.Location;
            mTextPBPanel.Size = mPreviewPictureBox.Size;
    
            // clear any preloaded image
            if (mPreLoadedImage!=null)
            {
                mPreLoadedImage.Dispose();
                mPreLoadedImage=null;
            }

        
            if (decoration == null)
            {
                this.mAdjustImageColoursControl1.SetForDecoration(null);
                this.mSelectAlphaMapControl1.SetForDecoration(null);
                this.mOrientateControl1.SetForDecoration(null);

                DisposeCurrentPB();
                EnableDisableDecorationOptions(decoration);
                if (mAllTabPages.Count > 0)
                {
                    mControlTabs.SelectedTab = mAllTabPages[0];
                }
                return;
            }

            CTextDecoration td = decoration as CTextDecoration;
            if (td!=null)
            {
                EnableDisableDecorationOptions(decoration);
                DisposeCurrentPB();

                List<TabPage> pages = new List<TabPage>();
                pages.Add(mTextTabPage);
                SelectVisibleTabs(pages);
                mPreviewPictureBox.Visible = false;
                this.mTextEditControl1.SetForDecoration(td, this.mTextPBPanel, mForSlide, mForSlideshow);
            }

            CClipArtDecoration cad = decoration as CClipArtDecoration;
            if (cad != null)
            {
                SetFilenameString(cad.mImage.ImageFilename);

                List<TabPage> pages = new List<TabPage>();
                pages.Add(mAdjustTab);
                pages.Add(mCropTabPage);
                pages.Add(mOrientationTab);
                pages.Add(mAlphaMapTabPage);             
                SelectVisibleTabs(pages);

                List<CImageDecoration> decs = GetAllImageDecorationsForDecor(cad);
                this.mAdjustImageColoursControl1.SetForDecoration(decs); 
                this.mSelectAlphaMapControl1.SetForDecoration(decs);    
                this.mOrientateControl1.SetForDecoration(decs);
            }

            CVideoDecoration vd = decoration as CVideoDecoration;
            if (vd != null)
            {
                SetFilenameString(vd.GetFilename());
                List<TabPage> pages = new List<TabPage>();             
                pages.Add(mTrimVideoTabPage);
                pages.Add(mOrientationTab);
                pages.Add(mAlphaMapTabPage);
                SelectVisibleTabs(pages);

                List<CImageDecoration> decs = GetAllImageDecorationsForDecor(vd);
                this.mTrimVideoControl1.SetForDecoration(vd, mForSlide);        // can only change first image, rest is done my link stuff anyway
                this.mSelectAlphaMapControl1.SetForDecoration(decs);             
                this.mOrientateControl1.SetForDecoration(decs);
            }
        
            UpdatePBImageAndThumbnail(false, true);
  
            mControlTabs.SelectedTab = mControlTabs.TabPages[0];

            EnableDisableDecorationOptions(decoration);

            this.ResumeLayout();
        }

        //*************************************************************************************************
        private void SetFilenameString(string filename)
        {
            if (filename.Length > 45)
            {
                filename = filename.Substring(filename.Length - 45, 45);
            }
            mFilenameTextBox.Text = filename;

            mSelectmageButton.Enabled = true;
        }


        //*************************************************************************************************
        private void EnableDisableDecorationOptions(CDecoration d)
        {

            if (d == null)
            {
                mControlTabs.Enabled = false;
                mSelectmageButton.Enabled = false;
                mResetButton.Enabled = false;
                mFilenameTextBox.Text = "";
                return;
            }

            CClipArtDecoration cad = d as CClipArtDecoration;

            // allow us to change things (e.g. alpha mask, if template user on template files)
            if (cad != null && CGlobals.mIsTemplateUser == false)
            {
                if (CImage.IsTemplateFilename(cad.mImage.ImageFilename) == true)
                {
                    mControlTabs.Enabled = false;
                    mSelectmageButton.Enabled = true;
                    mResetButton.Enabled = false;
                    mFilenameTextBox.Text = "";
                    return;
                }
            }                   

            mControlTabs.Enabled = true;
            mResetButton.Enabled = true;

            if (cad != null)
            {
                string filename = cad.mImage.ImageFilename;
                if (filename.Length > 60)
                {
                    filename = filename.Substring(filename.Length - 60, 60);
                }
                mFilenameTextBox.Text = filename;
                mSelectmageButton.Enabled = true;
            }
        }


        //*************************************************************************************************
        private void SelectVisibleTabs(List<TabPage> pages)
        {
            mControlTabs.TabPages.Clear();

            foreach (TabPage t in mAllTabPages)
            {
                // add if missing
                if (pages.Contains(t) == true)
                {
                    mControlTabs.TabPages.Add(t);
                }
            }
        }

        //*************************************************************************************************
        //This is called when one of the tabs has changed the image
        private void AdjustColourImageCallback()
        {
            LoadPreLoadImage();
            UpdatePBImageAndThumbnail(true, false);
        }


        //*************************************************************************************************
        private void LoadPreLoadImage()
        {
            // Store the raw image on memory to speed up future changes
            if (mPreLoadedImage == null)
            {
                
                CClipArtDecoration cad = mSelectedDecoration as CClipArtDecoration;
                if (cad != null)
                {
                    FileStream fs = CImage.GenerateFileStreamForImage(cad.mImage.ImageFilename);
      
                    Image ii = Image.FromStream(fs, true, false);

                    // try to reduce size of preview image as it can get very slow with full size images
                    // roughly reduce size of image, to take into account orientation changes
                    float previewWidth = mPreviewPictureBox.Image.Width;
                    float previewHeight = mPreviewPictureBox.Image.Height;

                    float w_zoom = ((float)1) / ((float)cad.mImage.Crop.Width);
                    float h_zoom = ((float)1) / ((float)cad.mImage.Crop.Height);

                    float zoom = w_zoom > h_zoom ? w_zoom : h_zoom;

                    float maxPreviewSize = previewWidth > previewHeight ? previewWidth : previewHeight;

                    float aspect = ((float)ii.Width) / ((float)ii.Height);

                    float imageWidth = ii.Width;
                    float imageHeight = ii.Height;

                    if (imageWidth < imageHeight)
                    {
                        imageWidth = maxPreviewSize;
                        imageHeight = imageWidth / aspect;
                    }
                    else
                    {
                        imageHeight = maxPreviewSize;
                        imageWidth = imageHeight * aspect;
                    }

                    int reqw = (int)(imageWidth * zoom);
                    int reqh = (int)(imageHeight * zoom);

                    // if bigger than original image than don't bother!
                    if (reqw > ii.Width || reqh > ii.Height)
                    {
                        reqw = ii.Width;
                        reqh = ii.Height;
                    }

                    mPreLoadedImage = new Bitmap(ii, reqw, reqh);
                    ii.Dispose();
                    fs.Close();
                }               

            }
        }

        //*************************************************************************************************
        private void UpdatePBImageAndThumbnail(bool delayThumbnailUpdate, bool cacheResult)
        {
            if (mSelectedDecoration is CTextDecoration) return;

            Image i = DecorationThumbnailGenerator.GenerateThumbnailForDecoration(mSelectedDecoration, mPreviewPictureBox.Width, mPreviewPictureBox.Height, mPreLoadedImage, true, false, cacheResult);

            if (i != null)
            {
                DisposeCurrentPB();
                mPreviewPictureBox.Image = i;
            }
            else
            {
                DisposeCurrentPB();
                Log.Error("Null image returned from EditTemplateMediaOrder.GetImageForDecoration");
            }

            if (delayThumbnailUpdate == true)
            {
                mUpdateThumbnailTimer.Interval = 1000;
            }

            UpdateThumnails(delayThumbnailUpdate, mPreviewPictureBox.Image, true);
        }

        //*************************************************************************************************  
        private void UpdateThumnails(bool delayThumbnailUpdate, Image PreviewPB, bool pausePreview)
        {
            if (delayThumbnailUpdate==false)
            {
                if (mPreviewPictureBox.Image != null)
                {
                    this.mEditTemplateMediaOrder1.UpdateThumbnailImage(mSelectedDecoration, PreviewPB as Bitmap);
                }  
     
                return;
            }

            if (PreviewPB == null)
            {
                mTimerUpdateWithPB = false;
            }
            else
            {
                mTimerUpdateWithPB = true;
            }

            if (mThumbnailNeedsUpdate == false)
            {
                if (pausePreview == true && mPreviewController !=null)
                {
                    mPreviewController.Pause();
                }

                mThumbnailNeedsUpdate = true;
            }
             // reset timer
            mUpdateThumbnailTimer.Stop();     
            mUpdateThumbnailTimer.Start();
        }


        //*************************************************************************************************s
        private void TimerTick(object sender, EventArgs e)
        {
            if (mThumbnailNeedsUpdate == true)
            {
                CleanD3dTexturesForImageSlide();

                if (mPreviewPictureBox.Image != null && mTimerUpdateWithPB && mControlTabs.SelectedTab != mCropTabPage)
                {
                    this.mEditTemplateMediaOrder1.UpdateThumbnailImage(mSelectedDecoration, mPreviewPictureBox.Image as Bitmap);
                }
                else
                {
                    this.mEditTemplateMediaOrder1.UpdateThumbnailImage(mSelectedDecoration, null);
                }
            }

            if (mPreviewController != null)
            {
                mPreviewController.Continue();
            }

            mUpdateThumbnailTimer.Stop();
            mThumbnailNeedsUpdate = false;

        }

        //*************************************************************************************************
        private void CleanD3dTexturesForImageSlide()
        {
            if (mForSlide == null) return;

            ArrayList list = mForSlide.GetAllAndSubDecorations();

            foreach (CDecoration d in list)
            {
                CClipArtDecoration cad = d as CClipArtDecoration;
                if (cad != null && cad.mImage != null)
                {
                    GraphicsEngine.Current.ClearCachedTexuresForImage(cad.mImage);
                }
            }

            CGroupedDecoration group = mForSlide.GetFirstGroupedDecoration();
            if (group != null)
            {
                group.ClearCachedImage();
            }
        }


        //*************************************************************************************************
        private void mSelectAlphaMapControl1_AlphaMapSelected()
        {
            UpdatePBImageAndThumbnail(false, true);
        }

        //*************************************************************************************************
        private void EditSlideMediaForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // make sure this happens before we leave
            mUpdateThumbnailTimer.Stop();
            mTrimVideoThumnailsTimer.Stop();

            if (mPreviewController != null)
            {
                mPreviewController.Stop();
                mPreviewController = null;
            }

            mForSlide.ReCalcAllVideoLinkStartEndTimes();
            CleanD3dTexturesForImageSlide();
        }

        //*************************************************************************************************
        // From one decoration get all decorations that have that template number, included the one passed in
        // If the decor is not have template numbers, then a list of the passed in decor is returned 
        private List<CImageDecoration> GetAllImageDecorationsForDecor(CImageDecoration originDecor)
        {
            List<CImageDecoration> returnList = new List<CImageDecoration>();

            int templateNum = originDecor.OriginalTemplateImageNumber;

            if (templateNum == 0)
            {
                returnList.Add(originDecor);
                return returnList;
            }

            ArrayList decs = mForSlide.GetAllAndSubDecorations();
            foreach (CDecoration dec in decs)
            {
                CImageDecoration imageDec = dec as CImageDecoration;
                if (imageDec != null)
                {
                    if (imageDec.OriginalTemplateImageNumber == templateNum)
                    {
                        returnList.Add(imageDec);
                    }
                }
            }
            return returnList;
        }


        //*************************************************************************************************
        private void mSelectmageButton_Click(object sender, EventArgs e)
        {
            try
            {
                CImageDecoration oldDecor = mSelectedDecoration as CImageDecoration;

                if (oldDecor is CTextDecoration) return;

                if (mPreviewController != null)
                {
                    mPreviewController.Stop();
                }

                if (mOpenFileDialog.ShowDialog().Equals(System.Windows.Forms.DialogResult.OK))
                {
                    if (mOpenFileDialog.FileNames.Length > 0)
                    {
                        if (ManagedCore.IO.IsDriveOkToUse(mOpenFileDialog.FileNames[0]) == false) return;
                        mOpenFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(mOpenFileDialog.FileNames[0]);  // rember current folder  

                        string file = mOpenFileDialog.FileNames[0];

                        int templateNum = oldDecor.OriginalTemplateImageNumber;

                        List<CImageDecoration> decorationsToReplace = GetAllImageDecorationsForDecor(oldDecor);

                        string videoReferenceId = CVideoDecoration.GenerateRandomStringId();
                        int count = 0;

                        CImageDecoration firstNewDecor = null;

                        foreach (CImageDecoration decToReplace in decorationsToReplace)
                        {
                            CImageDecoration newDecor;

                            if (CGlobals.IsImageFilename(file))
                            {
                                newDecor = new CClipArtDecoration(file, new RectangleF(0, 0, 1, 1), 0);
                            }
                            else
                            {
                                //  May need to do linked vd id second decToReplace or more
                                if (count == 0)
                                {
                                    newDecor = new CVideoDecoration(file, new RectangleF(0, 0, 1, 1), 0);

                                    // if more than one decoration to replace give first one a string id
                                    if (decorationsToReplace.Count > 0)
                                    {
                                        (newDecor as CVideoDecoration).StringID = videoReferenceId;
                                    }
                                }
                                else
                                {
                                    newDecor = new CVideoDecoration(videoReferenceId);
                                }
                            }

                            newDecor.CoverageArea = decToReplace.CoverageArea;
                            newDecor.DrawImageWithAspectType = decToReplace.DrawImageWithAspectType;
                            newDecor.RenderMethod = decToReplace.RenderMethod;
                            newDecor.MoveInEffect = decToReplace.MoveInEffect;
                            newDecor.MoveOutEffect = decToReplace.MoveOutEffect;
                            newDecor.StartOffsetTimeRawValue = decToReplace.StartOffsetTimeRawValue;
                            newDecor.EndOffsetTimeRawValue = decToReplace.EndOffsetTimeRawValue;
                            newDecor.AlphaMap = decToReplace.AlphaMap;

                            // Re calc image aspect if not an original template image or draw aspect type is not stretch)
                            if (decToReplace.OriginalTemplateImageNumber == 0 ||
                                decToReplace.DrawImageWithAspectType != CImage.DrawImageWithAspectType.Stretch)
                            {
                                newDecor.ReCalcCoverageAreaToMatchOriginalImageAspectRatio();
                            }

                            newDecor.Transparency = decToReplace.Transparency;
                            newDecor.Orientation = decToReplace.Orientation;
                            newDecor.Rotation = decToReplace.Rotation;
                            newDecor.XFlipped = decToReplace.XFlipped;
                            newDecor.YFlipped = decToReplace.YFlipped;
                            newDecor.OriginalTemplateImageNumber = decToReplace.OriginalTemplateImageNumber;
                            newDecor.UseParentSlidePanZoomAsDefaultMovement = decToReplace.UseParentSlidePanZoomAsDefaultMovement;

                            // ok if switching between video/image, also switch yflipped
                            if ((oldDecor is CVideoDecoration && newDecor is CClipArtDecoration) ||
                                (oldDecor is CClipArtDecoration && newDecor is CVideoDecoration) )
                            {
                                newDecor.YFlipped = ! newDecor.YFlipped;
                            }

                            // If replacing image with image, reset bright/contrast/b&w to old images original settings (i.e. ones created in original template)
                            // If image was not a template, it just sets them to default values
                            CClipArtDecoration newCad = newDecor as CClipArtDecoration;
                            CClipArtDecoration oldCad = oldDecor as CClipArtDecoration;
                            if (newCad != null && oldCad != null)
                            {
                                newCad.OriginalContrast = oldCad.OriginalContrast;
                                newCad.Contrast = oldCad.OriginalContrast;

                                newCad.OriginalBrightness = oldCad.OriginalBrightness;
                                newCad.Brightness = oldCad.OriginalBrightness;

                                newCad.OriginalBlackAndWhite = oldCad.OriginalBlackAndWhite;
                                newCad.BlackAndWhite = oldCad.OriginalBlackAndWhite;
                            }

                            mForSlide.RepaceDecoration(decToReplace, newDecor);
                            mEditTemplateMediaOrder1.ReplaceDecoration(decToReplace, newDecor);

                            if (oldDecor == decToReplace)
                            {
                                // The call to MediaSelectedIndexChanged has to be done AFTER we replaced all images, as the replaced images
                                // then get passed on to controls like crop
                                firstNewDecor = newDecor;
                            }

                            count++;
                        }

                        // Ok we've change an image, things like pan/zoom or slide length may now need to be re-calculated
                        if (firstNewDecor != null)
                        {
                            PredefinedSlideDesignsControl.ReCalcSlideSettingsAfterTemplateImageChanged(mForSlideshow, mForSlide, firstNewDecor);
                        }

                        if (firstNewDecor != null)
                        {
                            mEditTemplateMediaOrder1_MediaSelectedIndexChanged(firstNewDecor);
                        }
                        else
                        {
                            Log.Error("No 'new' first decor after replacement in EditSLideMediaForm");
                        }

                    }
                }
            }
            catch
            {
            }

            if (mPreviewController != null)
            {
                mPreviewController.Continue();
            }
        }


        //*************************************************************************************************
        private void mControlsTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            if (mSelectedDecoration is CClipArtDecoration)
            {
                if (mControlTabs.SelectedTab != mCropTabPage)
                {
                    mCropUserControl1.DisableCrop();
                    if (mPreLoadedImage == null)
                    {
                        UpdatePBImageAndThumbnail(false, true);
                    }
                }
                else
                {
                    mPreLoadedImage = null;

                    CImageDecoration imageDecor = mSelectedDecoration as CImageDecoration;
                    if (imageDecor != null)
                    {
                        List<CImageDecoration> decs = GetAllImageDecorationsForDecor(imageDecor);
                        mCropUserControl1.SetForDecoration(decs, this.mPreviewPictureBox);
                    }
                }
            }
        }

        //*************************************************************************************************
        private void mCropUserControl1_ImageChanged()
        {
            mUpdateThumbnailTimer.Interval = 5000;
            this.UpdateThumnails(true, null, false);
        }

        //*************************************************************************************************
        private void mCropUserControl1_MovingCropArea()
        {
       //     mUpdateThumbnailTimer.Interval = 5000;
        //    this.UpdateThumnails(true, null, false);
        }


        //*************************************************************************************************
        private void mOrientateControl1_ImageChanged()
        {
            CImageDecoration dec = mSelectedDecoration as CImageDecoration;
            if (dec != null)
            {
                dec.ReCalcCoverageAreaToMatchOriginalImageAspectRatio();
            }

            this.UpdatePBImageAndThumbnail(false, true);
        }

        //*************************************************************************************************
        private void mResetButton_Click(object sender, EventArgs e)
        {
            CClipArtDecoration cad = mSelectedDecoration as CClipArtDecoration;
            if (cad !=null)
            {
                float [] blank = new float[3] {1,1,1};
                cad.Brightness = blank;
                cad.Contrast = blank;
                cad.BlackAndWhite = false;
                cad.SetCrop(new RectangleF(0,0,1,1));
                cad.AlphaMap = null;
                cad.mImage.RotateFlipType = RotateFlipType.RotateNoneFlipNone;
                cad.ResetRotateAndFlip();
                if (cad.mImage != null)
                {
                    GraphicsEngine.Current.ClearCachedTexuresForImage(cad.mImage);
                }
                UpdatePBImageAndThumbnail(false, true);
                mEditTemplateMediaOrder1_MediaSelectedIndexChanged(mSelectedDecoration);

                mAdjustImageColoursControl1.mResetButton_Click(sender, e);
                mCropUserControl1.CropEnabledCheckBox.Checked = false;
            }

            CVideoDecoration vd = mSelectedDecoration as CVideoDecoration;
            if (vd != null)
            {
                vd.SetCrop(new RectangleF(0, 0, 1, 1));
                vd.AlphaMap = null;
                vd.Orientation = CImageDecoration.OrientationType.NONE;
                vd.ResetRotateAndFlip();
                vd.XFlipped = false;
                vd.YFlipped = true;
                UpdatePBImageAndThumbnail(false, true);
                mEditTemplateMediaOrder1_MediaSelectedIndexChanged(mSelectedDecoration);

                if (mTrimVideoControl1 != null)
                {
                    mTrimVideoControl1.Reset(this, new EventArgs());
                }
            }
        }

        //*************************************************************************************************
        private void mTrimVideoControl1_Trimmed(double previousLength)
        {
            mSlideLengthChanged = true;

            // ok we may want to change the length of the slide, i.e. the video decor determines
            // the length of the slide

            CImageSlide id = mForSlide as CImageSlide;
            if (id != null)
            {
                List<CVideoDecoration> videos = id.GetUserVideos();
               
                if (videos.Count <= 1)
                {
                    // ok, only one video (us), if we've not set our start/end time offset and the slide video length was matched before
                    // then this video decor has control over slide length
                    CVideoDecoration thisVD = mSelectedDecoration as CVideoDecoration;
                    if (thisVD != null)
                    {
                        if (thisVD.MatchesSlideLength(mForSlide.DisplayLength, (float)previousLength) == true)
                        {
                            mForSlide.SetLengthWithoutUpdate((float)thisVD.GetTrimmedVideoDurationInSeconds());
                        }
                    }
                }
            }

            mTrimVideoThumnailsTimer.Stop();
            mTrimVideoThumnailsTimer.Start();
        }

        //*************************************************************************************************s
        private void TrimVideoTimerTick(object sender, EventArgs e)
        {
            UpdatePBImageAndThumbnail(false, true);

            if (mPreviewController != null)
            {
                  mPreviewController.ResetFrameCountToStart();
                  mPreviewController.Continue();
            }

            mTrimVideoThumnailsTimer.Stop();
        }



        //*************************************************************************************************
        private void mTrimVideoControl1_ScrollStarted()
        {
            if (mPreviewController != null)
            {
                mPreviewController.Stop();
            }
        }


        //*************************************************************************************************
        private void mPreviousSlideButton_Click(object sender, EventArgs e)
        {
            int index = mForSlideshow.mSlides.IndexOf(mForSlide);

            if (index > 0)
            {
                CImageSlide s = mForSlideshow.mSlides[index - 1] as CImageSlide;
                if (s != null)
                {
                    SetForSlide(s, null);
                }
            }   
        }

        //*************************************************************************************************
        private void mNextSlideButton_Click(object sender, EventArgs e)
        {
            int index = mForSlideshow.mSlides.IndexOf(mForSlide);

            if (index < mForSlideshow.mSlides.Count - 1)
            {
                CImageSlide s = mForSlideshow.mSlides[index + 1] as CImageSlide;
                if (s != null)
                {
                    SetForSlide(s,null);
                }
            }
        }

        //*************************************************************************************************
        private void mEditTemplateMediaOrder1_MediaSourceChangeRequest(CDecoration decoration)
        {
            // make sure it is selected
            mEditTemplateMediaOrder1_MediaSelectedIndexChanged(decoration);

            mSelectmageButton_Click(this, new EventArgs());
        }


        //*************************************************************************************************
        private void mEncryptFile_Click(object sender, EventArgs e)
        {
            if (mSelectedDecoration == null) return;

            CVideoDecoration vd = mSelectedDecoration as CVideoDecoration;
            if (vd != null)
            {
                string name = vd.GetFilename();
                ManagedCore.CryptoFS.CreateVideoPFile(name);
               
            }

            CClipArtDecoration cad = mSelectedDecoration as CClipArtDecoration;
            if (cad != null)
            {
                string name = cad.mImage.ImageFilename;
                ManagedCore.CryptoFS.CreateImagePFile(name);
            }

            MessageBox.Show("All Done!");
        }

        //*******************************************************************
        // Returns true if a slide contains something that can be edited in this form; else returns false
        public static bool ContainsEditableDecoration(CImageSlide slide)
        {
            if (slide == null) return false;

            bool containsEditableSlide = false;

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

                CTextDecoration td = d as CTextDecoration;
                if (td != null)
                {
                    if (td.TemplateEditable == CTextDecoration.TemplateEditableType.None)
                    {
                        continue;
                    }
                }
                return true;
            }
            return false;
        }

        // ******************************************************************************
        private void mShowPreviewCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mShowPreviewCheckBox.Checked == true)
            {
                mPreviewSlide = true;
                if (mForSlide != null && mForSlide.PreviewPlayWhenEditingSlideMedia == true)
                {
                    mPreviewController = new MiniPreviewController(
                    mForSlideshow,
                    mForSlide,
                    this.mPreviewPanel);
                }
            }
            else
            {
                mPreviewSlide = false;
                mPreviewController.Stop();
                mPreviewController = null;
                mPreviewPanel.Refresh();
            }
        }
    }
}
