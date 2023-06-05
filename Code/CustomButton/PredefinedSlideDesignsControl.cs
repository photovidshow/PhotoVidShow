using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;
using System.IO;
using ManagedCore;
using System.Xml;
using System.Collections;
using DVDSlideshow.GraphicsEng;

namespace CustomButton
{
  
    public partial class PredefinedSlideDesignsControl : UserControl
    {
        public delegate void SlideDesignChangedCallbackDelegate(CImageSlide forSlide);
        public delegate void SlideImagesChangedCallbackDelegate(bool slideLengthChanged);

        public event SlideImagesChangedCallbackDelegate SlideImagesChanged;         // After doing a edit slide images
        public event SlideDesignChangedCallbackDelegate SelectedPreviewDesignChanged;      // if preview design changed

        private List<CandidateDecorationForTemplate> mPreviewUsingAdditionDecorations = null;
        private CSlideShow mPreviewSlideTemplateSlideshow = null;
        private MiniPreviewController mPreviewController = null;
        private SlideDesignChangedCallbackDelegate mGuiRebuildSlideshowCallback = null;
        private Control mPreviewInControl;

        private CSlide mCurrentEditingSlide;
        private CSlideShow mCurrentEditingSlideshow;


  

        //**********************************************************************************************
        // Decorations from current slide and next slides that could be used to created template design
        private class CandidateDecorationForTemplate
        {
            public CandidateDecorationForTemplate(CImageDecoration decoration,
                                                  CSlide slide)
            {
                mDecoration = decoration;
                mFromSlide = slide;
            }

            public CImageDecoration mDecoration;
            public CSlide mFromSlide;
        }

        //**********************************************************************************************
        private class TemplateListViewItem : ListViewItem
        {
            public TemplateListViewItem(string name, string filename) :
                base(name)
            {
                mFilename = filename;
            }
            public string mFilename;
        }
     
        public Button GetApplyDesignButton()
        {
            return mApplyDesignButton;
        }

        public Button GetEditSlideMediaButton()
        {
            return mEditSlideMediaButton;
        }

        public CheckBox GetUseNextSlidePicturesToPopulateDesignCheckBox()
        {
            return mUseNextSlidePicturesToPopulateDesignCheckBox;
        }

        public CSlide CurrentEditingSlide
        {
            get { return mCurrentEditingSlide; }
        }

        //*************************************************************************************************
        public PredefinedSlideDesignsControl()
        {
            InitializeComponent();

            bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);

            if (designMode == true) return;

            if (CGlobals.mIsTemplateUser == false)
            {
                mTemplateMotionBlurCombo.Visible = false;
                mTemplateMotionLabel.Visible = false;
                mPreviewTimeLabel.Visible = false;
                mPreviewTimeTextBox.Visible = false;
                mGenerateImages.Visible = false;
                mReCalcSlideLengthOnImage1Change.Visible = false;
            }

            string templatesDirectory = CGlobals.GetTemplateDirectory();
            string[] templates = Directory.GetFiles(templatesDirectory);

            ListViewItem[] items = new ListViewItem[templates.Length];

            int index =0;

            string thumbnailsDirectory = GetThumbnailsDirectory();
            FileInfo [] files = null;
            try
            {
                System.IO.DirectoryInfo directoryInfo = new DirectoryInfo(thumbnailsDirectory);

                files = directoryInfo.GetFiles();
            }
            catch (Exception e)
            {
                Log.Warning("Could not get template thumbnails file info in folder " + thumbnailsDirectory + " " + e.Message);
            }

            foreach (string template in templates)
            {     
                string name = Path.GetFileNameWithoutExtension(template);
                TemplateListViewItem lvu = new TemplateListViewItem(name, template);
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
                            mSlideDesignsImageList.Images.Add(thumbnail.Clone() as Bitmap);
                            fs.Close();
                            fs.Dispose();
                            thumbnail.Dispose();
                            lvu.ImageIndex = mSlideDesignsImageList.Images.Count - 1;
                            break;
                        }
                    }
                }
            }

            mSlideDesignsListView.Items.AddRange(items);

        }

        //*************************************************************************************************
        private void EditSlideMediaButton_Click(object sender, EventArgs e)
        {
            CustomButton.MiniPreviewController.StopAnyPlayingController();
            mPreviewController = null;

            EditSlideMediaForm esmf = new EditSlideMediaForm(mCurrentEditingSlide as CImageSlide, mCurrentEditingSlideshow, null);
            esmf.ShowDialog();
            GC.Collect();

            if (SlideImagesChanged != null)
            {
                SlideImagesChanged(esmf.SlideLengthChanged);
            }
        }
     
        //*************************************************************************************************
        private void mSlideDesignsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            CImageSlide imageSlide = null;

            try
            {
                // We are creating a preview slideshow, ingore project updates and storing momentos
                CGlobals.mCurrentProject.IgnoreProjectChanges = true;
              
                ListView.SelectedListViewItemCollection collection = mSlideDesignsListView.SelectedItems;
                if (collection.Count == 0)
                {
                    // Incase this gets multiple called with the same details.
                    if (mPreviewController != null &&
                        mPreviewController.IsRunning() == true &&
                        mPreviewController.CurrentSlideshow == mCurrentEditingSlideshow &&
                        mPreviewController.CurrentSlide == mCurrentEditingSlide)
                    {
                        return;
                    }

                    MiniPreviewController.StopAnyPlayingController();
                    mPreviewController = null;

                    // Just show current slide
                    if (mCurrentEditingSlide != null && mCurrentEditingSlideshow != null)
                    {
                       

                        mPreviewController = new MiniPreviewController(
                            mCurrentEditingSlideshow,
                            mCurrentEditingSlide,
                            mPreviewInControl);
                    }

                    this.mApplyDesignButton.Enabled = false;
                    UpdateDesignLabels("", "", true, 1);
                    return;
                }

                MiniPreviewController.StopAnyPlayingController();
                mPreviewController = null;

                // Should only be one selected
                TemplateListViewItem selectedItem = collection[0] as TemplateListViewItem;
                int index = selectedItem.Index;

                this.mApplyDesignButton.Enabled = true;

                string templateToLoad = selectedItem.mFilename;

                string name = "";
                string description = "";
                mPreviewUsingAdditionDecorations = null;
                mPreviewSlideTemplateSlideshow = GenerateSlideshowForTemplate(templateToLoad, ref name, ref description);

                if (mPreviewSlideTemplateSlideshow == null)
                {
                    return;
                }

                CSlide slide = mPreviewSlideTemplateSlideshow.mSlides[0] as CSlide;

                bool editable = slide.AllowedToBeEditited();

                ReplacePreviewSlideImagesWithCurrentSlideImages();

                imageSlide = slide as CImageSlide;

                CImageDecoration id = imageSlide.GetDecorationForOriginalTemplateNumber(1);
                if (id != null)
                {
                    ReCalcSlideSettingsAfterTemplateImageChanged(mCurrentEditingSlideshow, imageSlide, id);
                }

                mPreviewController = new MiniPreviewController(
                    mPreviewSlideTemplateSlideshow,
                    slide,
                    mPreviewInControl);

                int images = 0;

                // determine number of inputs

                if (imageSlide != null)
                {
                    ArrayList decs = imageSlide.GetAllAndSubDecorations();
                    foreach (CDecoration dec in decs)
                    {
                        CClipArtDecoration cad = dec as CClipArtDecoration;
                        if (cad != null)
                        {
                            int templateNumber = cad.OriginalTemplateImageNumber;
                            if (templateNumber > images)
                            {
                                images = templateNumber;
                            }
                        }
                    }
                }

                UpdateDesignLabels(name, description, editable, images);
            }
            finally
            {
                if (SelectedPreviewDesignChanged != null)
                {
                    SelectedPreviewDesignChanged(imageSlide);
                }
                CGlobals.mCurrentProject.IgnoreProjectChanges = false;
            }
        }



        //*************************************************************************************************
        private CSlideShow GenerateSlideshowForTemplate(string templateToLoad, ref string name, ref string description)
        {
            System.Xml.XmlDocument my_doc = new XmlDocument();
            try
            {
                my_doc.Load(templateToLoad);
            }
            catch (Exception exception1)
            {
                Log.Error("Could not read template file:" + templateToLoad + " " + exception1.Message);
                return null;
            }

            // Load in name and description, if defined
            XmlNodeList templateList = my_doc.GetElementsByTagName("Template");
            if (templateList.Count > 0)
            {
                XmlElement template = templateList[0] as XmlElement;
                if (template != null)
                {
                    name = template.GetAttribute("Name");
                    description = template.GetAttribute("Description");
                }
            }

            // ok load decoration effects
            XmlNodeList decorationEffectList = my_doc.GetElementsByTagName("DecorationEffectsDatabase");

            if (decorationEffectList.Count > 0)
            {
                XmlElement element = decorationEffectList[0] as XmlElement;
                CAnimatedDecorationEffectDatabase.GetInstance().Append(element, false, true);
            }

            CSlideShow previewSlideTemplateSlideshow = new CSlideShow("TemplatePreviewSlideshow");
            previewSlideTemplateSlideshow.FadeIn = false;
            previewSlideTemplateSlideshow.FadeOut = false;

            XmlNodeList list = my_doc.GetElementsByTagName("Slideshow");

            if (list.Count > 0)
            {
                XmlElement element = list[0] as XmlElement;
                try
                {
                    ManagedCore.MissingFilesDatabase.GetInstance().Clear();
                    previewSlideTemplateSlideshow.Load(element);

                    ArrayList missingFilesList = ManagedCore.MissingFilesDatabase.GetInstance().GetUnlinkedFiles();
                    if (missingFilesList.Count > 0)
                    {
                        string message = "\n\r\n\rFailed to find file(s):-\n\r";
                        foreach (MissingFile mf in missingFilesList)
                        {
                            message += mf.mName +"\n\r";
                        }
                        throw new Exception(message);
                    }
                }
                catch (Exception exception2)
                {
                    string message = "Could not create slide from template '" + System.IO.Path.GetFileNameWithoutExtension(templateToLoad) + "' " + exception2.Message;
                    MessageBox.Show(message, "Failed to load template design", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.Error(message);
                    return null;
                }
            }
            else
            {
                Log.Error("No slideshow defined in template:" + templateToLoad);
                return null;
            }

            if (previewSlideTemplateSlideshow.mSlides.Count == 0)
            {
                Log.Error("Failed to create a slide after reading template:" + templateToLoad);
                return null;
            }

           
            // If we're at a different aspect to that the template was designed with (16:9) we need to declare an aspect change on the slideshow
            if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio != CGlobals.OutputAspectRatio.TV16_9)
            {
                previewSlideTemplateSlideshow.DeclareSlideAspectChange(CGlobals.OutputAspectRatio.TV16_9, CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio);
            }

            return previewSlideTemplateSlideshow;
        }
    
        //*************************************************************************************************
        private void UpdateDesignLabels(string name, string description, bool editable, int images)
        {
            if (name == "")
            {
                mDesignLabel.Text = "Design: None" + name;
                mEditableLabel.Text = "";
                mInputImagesLabel.Text = "";
                mDescriptionLabel.Text = "";
            }
            else
            {
                this.mDesignLabel.Text = "Design: " + name;

                mInputImagesLabel.Text = "Input images: " + images.ToString();
                if (editable == true)
                {
                    this.mEditableLabel.Text = "Editable: Yes";
                }
                else
                {
                    this.mEditableLabel.Text = "Editable: No";
                }

                this.mDescriptionLabel.Text = description;
            }
        }


        //*************************************************************************************************
        private bool IsDecorationAppropriateForCandidateReplacement(CDecoration dec, ref int videocount, int maxVideosPerSlide, bool[] templatesUsedAlready)
        {
            if (dec.IsBackgroundDecoration() ||
                dec.IsBorderDecoration() ||
                dec.IsFilter())
            {
                return false;
            }

            if (dec.IsTemplateFrameworkDecoration() == false &&
                (dec is CClipArtDecoration || dec is CVideoDecoration))
            {
                CImageDecoration imageDecor = dec as CImageDecoration;

                // Have we already added a candidate with the same original template number?
                int origTemplateNumber = imageDecor.OriginalTemplateImageNumber;
                if (origTemplateNumber != 0 && origTemplateNumber < templatesUsedAlready.Length)
                {
                    // already used this template number
                    if (templatesUsedAlready[origTemplateNumber] == true)
                    {
                        return false;
                    }
                    templatesUsedAlready[origTemplateNumber] = true;
                }

                if (dec is CVideoDecoration)
                {
                    if (videocount >= maxVideosPerSlide)
                    {
                        return false;
                    }
                    else
                    {
                        videocount++;
                    }
                }
                else
                {
                    CClipArtDecoration cad = dec as CClipArtDecoration;
                    if (cad != null)
                    {
                        string root = CGlobals.GetRootDirectory().ToLower();
                        // Igore non template images that are still part of our template library
                        if (cad.mImage.ImageFilename.ToLower().Contains(root + "\\templates\\") == true ||
                            cad.mImage.ImageFilename.ToLower().StartsWith("templates\\") == true ||
                            cad.mImage.ImageFilename.ToLower().Contains(root + "\\menutemplates\\") == true ||
                            cad.mImage.ImageFilename.ToLower().StartsWith("menutemplates\\") == true ||
                            CImage.IsTemplateFilename(cad.mImage.ImageFilename) == true)
                        {
                            return false;
                        }
                    }
                }

                return true;
      
            }
            return false;
        }

        //*************************************************************************************************
        // Basically some templates have OriginalTemplateImageNumber 1 set to something.  E.g a background.
        // So the first candidate image wants to now replace IMAGE2 instead
        private int FindFirstTemplateNumberWeCanReplace(CImageSlide previewSlide)
        {
            ArrayList previewDecorations = previewSlide.GetAllAndSubDecorations();

            int lowest = 100;

            // has to be index loop, as we possible change the list we are iterating through
            for (int previewIndex = 0; previewIndex < previewDecorations.Count; previewIndex++)
            {
                CDecoration dec = previewDecorations[previewIndex] as CDecoration;

                // All templates inputs are text or clip art decors
                CClipArtDecoration preivewCad = dec as CClipArtDecoration;
                if (preivewCad != null &&
                    preivewCad.IsTemplateFrameworkDecoration() == false)
                {
                    string filename = preivewCad.mImage.ImageFilename.ToLower();

                    int i = filename.IndexOf("image");
                    if (i != -1 && preivewCad.OriginalTemplateImageNumber < lowest)
                    {
                        lowest = preivewCad.OriginalTemplateImageNumber;
                    }
                }
            }

            return lowest;
        }

        //*************************************************************************************************
        private void ReplacePreviewSlideImagesWithCurrentSlideImages()
        {
            if (mCurrentEditingSlide == null || mCurrentEditingSlideshow == null)
            {
                Log.Error("No current set slide or slideshow in ReplacePreviewSlideImagesWithCurrentSlideImages");
                return;
            }

            CImageSlide previewSlide = mPreviewSlideTemplateSlideshow.mSlides[0] as CImageSlide;

            int videocount = previewSlide.GetNumberOfDifferentVideosInSlide();
            int maxVideosPerSlide = CGlobals.GetMaxVideosPerSlide();

            // OK populate current slide pictures to be use in template
            CImageSlide currentImageSlide = mCurrentEditingSlide as CImageSlide;

            List<CandidateDecorationForTemplate> currentSlideDecorations = new List<CandidateDecorationForTemplate>();

            int usableDecorationsInSelectedSlide = 0;

            ArrayList decorations = currentImageSlide.GetAllAndSubDecorations();

            bool[] usedAlready = new bool[CGlobals.mMaxTemplateNumbers];

            for (int index = 0; index < CGlobals.mMaxTemplateNumbers; index++)
            {
                usedAlready[index] = false;
            }

            foreach (CDecoration dec in decorations)
            {
                if (IsDecorationAppropriateForCandidateReplacement(dec, ref videocount, maxVideosPerSlide, usedAlready) == true)
                {
                    currentSlideDecorations.Add( new CandidateDecorationForTemplate(
                        dec as CImageDecoration, currentImageSlide));
                    usableDecorationsInSelectedSlide++;
                }
            }

            
            // use next slides images?
            if (mUseNextSlidePicturesToPopulateDesignCheckBox.Checked == true &&
                mUseNextSlidePicturesToPopulateDesignCheckBox.Enabled == true)
            {
                int currentSlideIndex = mCurrentEditingSlideshow.mSlides.IndexOf(mCurrentEditingSlide);
                currentSlideIndex++;

                for (int index = 0; index < CGlobals.mMaxTemplateNumbers; index++)
                {
                    usedAlready[index] = false;
                }

                while (currentSlideIndex >= 0 &&
                       mCurrentEditingSlideshow.mSlides.Count > currentSlideIndex && 
                       currentSlideDecorations.Count <20)
                {
                    CImageSlide candiateSlide = mCurrentEditingSlideshow.mSlides[currentSlideIndex] as CImageSlide;

                    ArrayList candidateDecorations = candiateSlide.GetAllAndSubDecorations();

                    if (candidateDecorations.Count == 1 &&
                        (candidateDecorations[0] is CClipArtDecoration ||
                         candidateDecorations[0] is CVideoDecoration))
                    {
                        CImageDecoration imageDec = candidateDecorations[0] as CImageDecoration;
                        if (IsDecorationAppropriateForCandidateReplacement(imageDec, ref videocount, maxVideosPerSlide, usedAlready) == true)
                        {
                            currentSlideDecorations.Add(new CandidateDecorationForTemplate(
                                        imageDec, mCurrentEditingSlideshow.mSlides[currentSlideIndex] as CSlide));
                        }
                    }
                    currentSlideIndex++;
                }
            }


            // used for linked video stuff
            CVideoDecoration[] videoDecorsIndex = new CVideoDecoration[CGlobals.mMaxTemplateNumbers];

            // ok found some candidate

            if (currentSlideDecorations.Count > 0)
            {

                int lowest = FindFirstTemplateNumberWeCanReplace(previewSlide);
                mPreviewUsingAdditionDecorations = new List<CandidateDecorationForTemplate>();

                ArrayList previewDecorations = previewSlide.GetAllAndSubDecorations();

                // has to be index loop, as we possible change the list we are iterating through
                for (int previewIndex=0; previewIndex < previewDecorations.Count;previewIndex++)         
                {
                    CDecoration dec = previewDecorations[previewIndex] as CDecoration;

                    // All templates inputs are text or clip art decors
                    CClipArtDecoration preivewCad = dec as CClipArtDecoration;
                    if (preivewCad != null &&
                        preivewCad.IsTemplateFrameworkDecoration() == false)
                    {
                        string filename = preivewCad.mImage.ImageFilename.ToLower();

                        int i = filename.IndexOf("image");

                        if (i !=-1)
                        {
                            try
                            {
                                string ss = filename.Substring(i + 5);
                                ss = ss.Replace(".jpg", "");
                                int index = int.Parse(ss);
                                index-=lowest;

                                if (currentSlideDecorations.Count > index && index>=0)
                                {
                                    if (currentSlideDecorations[index].mDecoration is CClipArtDecoration)
                                    {
                                        CClipArtDecoration currentCad = currentSlideDecorations[index].mDecoration as CClipArtDecoration;

                                        preivewCad.ReplaceWithImage(currentCad.mImage.ImageFilename);
                                    }
                                    else if (currentSlideDecorations[index].mDecoration is CVideoDecoration)
                                    {
                                        CVideoDecoration currentVD = currentSlideDecorations[index].mDecoration as CVideoDecoration;

                                        CVideoDecoration newVd = null;

                                        // may need to create a linked video decor instead
                                        if (videoDecorsIndex[index] == null)
                                        {
                                            newVd = CVideoDecoration.FromClipArtDecoration(preivewCad, currentVD.GetFilename(), currentVD.StartVideoOffset, currentVD.EndVideoOffset);
                                            videoDecorsIndex[index] = newVd;
                                        }
                                        else
                                        {
                                            CVideoDecoration firstVideoDecor = videoDecorsIndex[index];
                                            if (firstVideoDecor.StringID == "")
                                            {
                                                firstVideoDecor.StringID = CVideoDecoration.GenerateRandomStringId();
                                            }
                                        
                                            newVd = CVideoDecoration.FromClipArtDecoration(preivewCad, firstVideoDecor.StringID, currentVD.StartVideoOffset, currentVD.EndVideoOffset);
                                        }

                                        // replace cad with vd
                                        previewSlide.RepaceDecoration(preivewCad, newVd);
                                    }

                                    // record any decoration used from a different slide
                                    if (currentSlideDecorations[index].mFromSlide != mCurrentEditingSlide)
                                    {
                                        mPreviewUsingAdditionDecorations.Add(currentSlideDecorations[index]);
                                    }
                                }
                            }
                            catch (Exception exception)
                            {
                                Log.Error("Failed to determine image number in template (exception occurred) :" + exception.Message);
                            }
                        }
                    }
                }
            }
        }

        //*************************************************************************************************
        public void SetSlide(CSlide slide, CSlideShow slideshow, SlideDesignChangedCallbackDelegate applyDesignCallback, Control previewControl)
        {
            mPreviewInControl = previewControl;
            mGuiRebuildSlideshowCallback = applyDesignCallback;
            mCurrentEditingSlide = slide;
            mCurrentEditingSlideshow = slideshow;

            SelectNothing();
            SetMotionBlurComboAndPreviewTime();
        }

        //*************************************************************************************************
        private void SetMotionBlurComboAndPreviewTime()
        {
            if (mCurrentEditingSlide == null) return;

            if (this.mTemplateMotionBlurCombo.Visible == true)
            {
                mTemplateMotionBlurCombo.Text = mCurrentEditingSlide.ForcedMotionBlurSubFrames.ToString();
                mPreviewTimeTextBox.Text = mCurrentEditingSlide.ThumbnailPreviewTime.ToString();
                mReCalcSlideLengthOnImage1Change.Checked = mCurrentEditingSlide.ReCalcSlideLengthOnImage1DecorationChange;
            }    
        }

        //*************************************************************************************************
        private void SelectNothing()
        {
            // select nothing, better way of doing it?
            ListView.SelectedListViewItemCollection collection = mSlideDesignsListView.SelectedItems;
            if (collection.Count != 0)
            {
                collection[0].Selected = false;
            }
            else
            {
                mSlideDesignsListView_SelectedIndexChanged(this, new EventArgs());
            }
        }


        //*************************************************************************************************
        public void mApplyDesignButton_Click(object sender, EventArgs e)
        {
            CustomButton.MiniPreviewController.StopAnyPlayingController();
            mPreviewController = null;

            if (mCurrentEditingSlide == null)
            {
                Log.Error("No current set slide in mApplyDesignButton_Click");
                return;
            }

            if ( mPreviewSlideTemplateSlideshow==null)
            {
                Log.Error("No mPreviewSlideTemplateSlideshow in mApplyDesignButton_Click");
                return;
            }
            if (mPreviewSlideTemplateSlideshow.mSlides.Count < 1)
            {
                Log.Error("No mPreviewSlideTemplateSlideshow slides in mApplyDesignButton_Click");
                return;
            }

            CImageSlide previewSlide = mPreviewSlideTemplateSlideshow.mSlides[0] as CImageSlide;   
     
            if (mPreviewUsingAdditionDecorations != null &&
                mPreviewUsingAdditionDecorations.Count > 0)
            {
                ArrayList toRemove = new ArrayList(mPreviewUsingAdditionDecorations.Count);

                foreach (CandidateDecorationForTemplate usedDecoration in mPreviewUsingAdditionDecorations)
                {
                    toRemove.Add(usedDecoration.mFromSlide);
                }

                mCurrentEditingSlideshow.RemoveSlides(toRemove, false);

            }
           
            int index=0;
      
            bool found = false;

            foreach (CSlide slide in mCurrentEditingSlideshow.mSlides)
            {
                if (slide == mCurrentEditingSlide)
                {
                    found = true;
                    break;
                }
                index++;
            }
            if (found == false)
            {
                Log.Error("Could not find slide to replace in mApplyDesignButton_Click");
                return;
            }


            mCurrentEditingSlideshow.mSlides[index] = previewSlide;

            ArrayList additionalToRemove = new ArrayList();
            additionalToRemove.Add(mCurrentEditingSlide);
            mCurrentEditingSlideshow.RemoveSlides(additionalToRemove, false);
            mCurrentEditingSlide = previewSlide;
            mCurrentEditingSlide.ReCalcAllVideoLinkStartEndTimes();
            mCurrentEditingSlideshow.CalcLengthOfAllSlides();

            if (mGuiRebuildSlideshowCallback != null)
            {
                mGuiRebuildSlideshowCallback(mCurrentEditingSlide as CImageSlide);
            }

            SelectNothing();
            SetMotionBlurComboAndPreviewTime();

            CGlobals.mCurrentProject.DeclareChange("Applied template design");

        }


        //*************************************************************************************************
        private void mUseNextSlidePicturesToPopulateDesignCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // re-creates preview slide
            mSlideDesignsListView_SelectedIndexChanged(sender, e);
        }


        //*************************************************************************************************
        private void mTemplateMotionBlurCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mCurrentEditingSlide == null) return;

            int mb = mCurrentEditingSlide.ForcedMotionBlurSubFrames;

            try
            {
                mb = int.Parse(mTemplateMotionBlurCombo.Text);
            }
            catch
            {
            }

            if (mCurrentEditingSlide.ForcedMotionBlurSubFrames != mb)
            {
                mCurrentEditingSlide.ForcedMotionBlurSubFrames = mb;
            }
        }

        //*************************************************************************************************
        private void mPreviewTimeTextBox_TextChanged(object sender, EventArgs e)
        {
            if (mCurrentEditingSlide == null) return;

            try
            {
                float time = float.Parse(mPreviewTimeTextBox.Text);
                mCurrentEditingSlide.ThumbnailPreviewTime = time;
            }
            catch
            {
                mPreviewTimeTextBox.Text = mCurrentEditingSlide.ThumbnailPreviewTime.ToString();
            }
        }

        //*************************************************************************************************
        private Bitmap GenerateImagesForListViewItem(string template)
        {
            string name = "";
            string description = "";

            CSlideShow slideshow = GenerateSlideshowForTemplate(template, ref name, ref description);

            if (slideshow == null)
            {
                return null;
            }

            if (slideshow.mSlides.Count == 0)
            {
                return null;
            }

            RenderVideoParameters rvp = new RenderVideoParameters();

            CSlide slide = slideshow.mSlides[0] as CSlide;
            if (slide != null && slide.ThumbnailPreviewTime != -1)
            {
                rvp.frame = (int)(slide.ThumbnailPreviewTime * CGlobals.mCurrentProject.DiskPreferences.frames_per_second);
            }

            Bitmap endBitmap = new Bitmap(mSlideDesignsImageList.ImageSize.Width, mSlideDesignsImageList.ImageSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int w = CGlobals.mCurrentProject.DiskPreferences.CanvasWidth;
            int h = CGlobals.mCurrentProject.DiskPreferences.CanvasHeight;

            rvp.req_width =w;
            rvp.req_height = h;

            Bitmap bitmap = slideshow.RenderVideoToBitmap(rvp);

            using (Graphics g = Graphics.FromImage(endBitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                g.DrawImage(bitmap, 0, 0, endBitmap.Width, endBitmap.Height);

                using (Pen blackPen = new Pen(Color.Black))
                {
                    g.DrawRectangle(blackPen, 0, 0, endBitmap.Width-1, endBitmap.Height-1);
                }
            }

            return endBitmap;
        }

        //*************************************************************************************************
        private string GetThumbnailsDirectory()
        {
            return CGlobals.GetTemplateDirectory() + "\\thumbnails";
        }

        //*************************************************************************************************
        private void mGenerateImages_Click(object sender, EventArgs e)
        {
            ListView.ListViewItemCollection collection = mSlideDesignsListView.Items;

            string thumbnailsDirectory = GetThumbnailsDirectory();

            System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(thumbnailsDirectory);

            foreach (FileInfo file in downloadedMessageInfo.GetFiles())
            {
                file.Delete();
            }

            foreach (ListViewItem lvi in collection)
            {
                TemplateListViewItem tlvi = lvi as TemplateListViewItem;
                if (tlvi != null)
                {
                    Bitmap bitmap = GenerateImagesForListViewItem(tlvi.mFilename);

                    if (bitmap == null)
                    {
                        Log.Error("Could not generate image for template:" + tlvi.Name + " filename:"+ tlvi.mFilename);
                        tlvi.ImageIndex = 0;
                    }
                    else
                    {
                        mSlideDesignsImageList.Images.Add(bitmap);
                        tlvi.ImageIndex = mSlideDesignsImageList.Images.Count - 1;

                        string fn = thumbnailsDirectory +"\\" + tlvi.Text + ".jpg";
                        try
                        {
                            bitmap.Save(fn, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                        catch
                        {
                        }
                    }
                }

                mSlideDesignsListView.RedrawItems(0, collection.Count - 1, false);
            }
        }

        //*************************************************************************************************
        private void mClearSelectedButton_Click(object sender, EventArgs e)
        {
            SelectNothing();
        }

        //*************************************************************************************************
        private void mReCalcSlideLengthOnImage1Change_CheckedChanged(object sender, EventArgs e)
        {
            if (mCurrentEditingSlide != null &&
                mCurrentEditingSlide.ReCalcSlideLengthOnImage1DecorationChange != mReCalcSlideLengthOnImage1Change.Checked)
            {
                mCurrentEditingSlide.ReCalcSlideLengthOnImage1DecorationChange = mReCalcSlideLengthOnImage1Change.Checked;
            }
        }


       //*************************************************************************************************
        // Ok we've change a template image, things like pan/zoom or slide length may now need to be re-calculated
       public static void ReCalcSlideSettingsAfterTemplateImageChanged(CSlideShow forSlideshow, CImageSlide forSlide, CImageDecoration forDecor)
       {
            if (forDecor == null) return;

            CVideoDecoration vd = forDecor as CVideoDecoration;

            // change image do we need to change pan zoom?  This only applies to first image
            if (forSlide.PanZoom.ReGenerateOnImageChange == true)
            {
                CStillPictureSlide sps = forSlide as CStillPictureSlide;
                if (sps != null && forDecor.OriginalTemplateImageNumber == 1)
                {
                    if (vd == null)
                    {
                        sps.UsePanZoom = forSlideshow.mDefaultSlide.UsePanZoom;
                        sps.ReRadomisePanZoom();
                    }
                    else
                    {
                        sps.UsePanZoom = false;
                    }
                }
            }

            // Need to recalc slide length?
            if (forSlide.ReCalcSlideLengthOnImage1DecorationChange == true &&
                forDecor.OriginalTemplateImageNumber == 1)
            {
                if (vd != null)
                {
                    forSlide.DisplayLength = (float)vd.GetTrimmedVideoDurationInSeconds();
                }
                else
                {
                    forSlide.DisplayLength = forSlideshow.mDefaultSlide.DisplayLength;
                }
            }
        }
    }
}
