using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using DVDSlideshow;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using ZBobb;
using System.IO;
using ManagedCore;
using System.Globalization;
using DVDSlideshow.GraphicsEng;
using CustomButton;

namespace dvdslideshowfontend
{
    public class CDecorationManagerSettingEffects
    {
      //  private ComboBox mDecorationEffectInEffectComboBox;
       // private ComboBox mDecorationEffectOutEffectComboBox;

      //  private ComboBox mStartDecorationTimeCombo;
      //  private ComboBox mEndDecorationTimeCombo;

        private SetStartEndSlideTime mDecorationStartEndSlideTimeControl;

        private ComboBox mRenderMethodCombo;
        private ComboBox mTextTemplateTypeCombo;
        private ComboBox mTemplateAspectCombo;
        private ComboBox mTemplateImageNumberCombo;
        private CheckBox mTemplateFrameworkImageCheckbox;

        private Form1 mMainForm;
        private CAnimatedDecoration mSelectedDecoration;
        private Button mEffectsEditorButton;

        private const string SlideStartString = "Slide start";
        private const string SlideEndString = "Slide end";
        private const string NoneString = "None";

        //*******************************************************************
        public CDecorationManagerSettingEffects(Form1 mainForm)
        {
            mMainForm = mainForm;

            if (mainForm == null) return;

 
            if (CGlobals.mIsTemplateUser == true)
            {
                mTextTemplateTypeCombo = mMainForm.GetTextTemplateTypeCombo();
                mMainForm.GetTextTemplateTypeLabel().Visible = true;
                mTextTemplateTypeCombo.Visible = true;
                mTextTemplateTypeCombo.SelectedIndexChanged += new EventHandler(mTextTemplateTypeCombo_SelectedIndexChanged);

                mRenderMethodCombo = mMainForm.GetRenderMethodCombo();
                mRenderMethodCombo.Visible = true;
                mRenderMethodCombo.Items.Add("Normal");
                mRenderMethodCombo.Items.Add("Add");
                mRenderMethodCombo.Items.Add("Dest minus src");
                mRenderMethodCombo.Items.Add("Src minus dest");
                mRenderMethodCombo.Items.Add("Modulate");
                mRenderMethodCombo.Items.Add("Modulate2x");
                mRenderMethodCombo.Items.Add("Modulate4x");
                mRenderMethodCombo.Items.Add("Normal no alpha blend");
                mRenderMethodCombo.Items.Add("Red Mask");
                mRenderMethodCombo.Items.Add("Green Mask");
                mRenderMethodCombo.Items.Add("Blue Mask");
                mRenderMethodCombo.Items.Add("Purple Mask");
                mRenderMethodCombo.SelectedIndexChanged += new EventHandler(mRenderMethodCombo_SelectedIndexChanged);

                mTemplateAspectCombo = mMainForm.GetTemplateAspectCombo();
                mTemplateAspectCombo.Visible = true;
                mTemplateAspectCombo.SelectedIndexChanged += new EventHandler(mTemplateAspectCombo_SelectedIndexChanged);

                mTemplateImageNumberCombo = mMainForm.GetTemplateImageNumberCombo();
                mTemplateImageNumberCombo.Visible = true;
                mTemplateImageNumberCombo.SelectedIndexChanged += new EventHandler(mTemplateImageNumberCombo_SelectedIndexChanged);

                mTemplateFrameworkImageCheckbox = mMainForm.GetTemplateFrameworkImageCheckbox();
                mTemplateFrameworkImageCheckbox.Visible = true;
                mTemplateFrameworkImageCheckbox.CheckStateChanged += new EventHandler(mTemplateFrameworkImageCheckbox_CheckStateChanged);

            }

            mDecorationStartEndSlideTimeControl = mainForm.GetDecorationStartEndSlideTimeControl();
            mDecorationStartEndSlideTimeControl.TimesChangedCallback += new SetStartEndSlideTime.TimesChangedCallbackDelegate(mDecorationStartEndSlideTimeControl_TimesChangedCallback);

            mEffectsEditorButton = mMainForm.GetEffectsEditorButton();

            mEffectsEditorButton.Click += new System.EventHandler(this.mEffectsEditor_Click);
        }

        //*******************************************************************
        void mRenderMethodCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mSelectedDecoration == null) return;

            CImageDecoration imageDecoration = mSelectedDecoration as CImageDecoration;
            if (imageDecoration != null)
            {
                imageDecoration.RenderMethod = (CImageDecoration.RenderMethodType)mRenderMethodCombo.SelectedIndex;
            }
        }

        //*******************************************************************
        void mTemplateAspectCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mSelectedDecoration == null) return;

            CImageDecoration imageDecoration = mSelectedDecoration as CImageDecoration;
            if (imageDecoration != null)
            {
                imageDecoration.DrawImageWithAspectType = (CImage.DrawImageWithAspectType)mTemplateAspectCombo.SelectedIndex;
                Form1.mMainForm.GetDecorationManager().RePaint();
            }
        }

        //*******************************************************************
        void mTemplateImageNumberCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mSelectedDecoration == null) return;

            CImageDecoration imageDecoration = mSelectedDecoration as CImageDecoration;
            if (imageDecoration != null)
            {
                if (mTemplateImageNumberCombo.Text == "MASK")
                {
                    imageDecoration.MaskImage = true;
                    imageDecoration.OriginalTemplateImageNumber = 0;
                }
                else
                {
                    imageDecoration.MaskImage = false;
                    int index = mTemplateImageNumberCombo.SelectedIndex;
                    if (index != 0)
                    {
                        imageDecoration.SetAsTemplateImageNumber(mTemplateImageNumberCombo.SelectedIndex);
                    }
                }
            }
        }

        //*******************************************************************
        void mTemplateFrameworkImageCheckbox_CheckStateChanged(object sender, EventArgs e)
        {
            if (mSelectedDecoration == null) return;

            CImageDecoration imageDecoration = mSelectedDecoration as CImageDecoration;
            if (imageDecoration != null)
            {
                imageDecoration.MarkedAsTemplateFrameworkDecoration = mTemplateFrameworkImageCheckbox.Checked;
            }
        }



        //*******************************************************************
        void mTextTemplateTypeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mSelectedDecoration == null) return;

            CTextDecoration textDecoration = mSelectedDecoration as CTextDecoration;
            if (textDecoration != null)
            {
                textDecoration.TemplateEditable = (CTextDecoration.TemplateEditableType)mTextTemplateTypeCombo.SelectedIndex;
            }
        }

       //*******************************************************************
        private void SetMotionEffectsEnabled(CImageSlide forSlide)
        {
            if (forSlide == null)
            {
                mEffectsEditorButton.Enabled = false;
                return;
            }

            ArrayList list = forSlide.GetAllAndSubDecorations();
            foreach (CDecoration d in list)
            {
                if (d.IsUserDecoration() == true)
                {
                    mEffectsEditorButton.Enabled = true;
                    return;
                }
            }

            mEffectsEditorButton.Enabled = false;
        }
     
        //*******************************************************************
        public void EnableControls(CDecoration decoration, CImageSlide forSlide)
        {
            float slideLength = forSlide.DisplayLength;

            mSelectedDecoration = decoration as CAnimatedDecoration;

            SetMotionEffectsEnabled(forSlide);

            if (mSelectedDecoration != null)
            {
                mDecorationStartEndSlideTimeControl.Enabled = true;
                mDecorationStartEndSlideTimeControl.Set(mSelectedDecoration.GetStartOffsetTime(slideLength),
                                                        mSelectedDecoration.GetEndOffsetTime(slideLength),
                                                        slideLength);
            }

            CImageDecoration imageDecoration = decoration as CImageDecoration;

            if (imageDecoration != null && mRenderMethodCombo !=null)
            {
                if (mRenderMethodCombo != null)
                {
                    mRenderMethodCombo.SelectedIndex = (int)imageDecoration.RenderMethod;
                }

                if (mTemplateAspectCombo != null)
                {
                    mTemplateAspectCombo.SelectedIndex = (int)imageDecoration.DrawImageWithAspectType;
                }

                if (mTemplateImageNumberCombo != null)
                {
                    if (imageDecoration.MaskImage == true)
                    {
                        mTemplateImageNumberCombo.Text = "MASK";
                    }
                    else
                    {
                        mTemplateImageNumberCombo.SelectedIndex = imageDecoration.GetTemplateImageNumber();
                    }
                }

                if (mTemplateFrameworkImageCheckbox != null)
                {
                    mTemplateFrameworkImageCheckbox.Checked = imageDecoration.MarkedAsTemplateFrameworkDecoration;
                }
            }

            CTextDecoration textDecoration = decoration as CTextDecoration;
            if (textDecoration !=null)
            {
                if (mTextTemplateTypeCombo != null)
                {
                    mTextTemplateTypeCombo.SelectedIndex = (int)textDecoration.TemplateEditable;
                }
            }

        }

        //*******************************************************************
        private string OffsetTimeToString(float time)
        {
            if (time <= CAnimatedDecoration.SlideStart) return SlideStartString;
            if (time >= CAnimatedDecoration.SlideEnd) return SlideEndString;
            return time.ToString();
        }

        //*******************************************************************
        public void DisableControls(CImageSlide forSlide)
        {
            SetMotionEffectsEnabled(forSlide);

            mSelectedDecoration = null;

            mDecorationStartEndSlideTimeControl.Enabled = false;
            mDecorationStartEndSlideTimeControl.Set(0, 8, 8);
        }


        //*******************************************************************
        public void RefreshSlideLength(float slideLength)
        {
            if (mSelectedDecoration != null)
            {
                CAnimatedDecoration animDec = mSelectedDecoration as CAnimatedDecoration;

                if (animDec != null)
                {
                    mDecorationStartEndSlideTimeControl.Set(mSelectedDecoration.GetStartOffsetTime(slideLength),
                                                            mSelectedDecoration.GetEndOffsetTime(slideLength),
                                                            slideLength);
                }
            }

        }

        //*******************************************************************
        void mDecorationStartEndSlideTimeControl_TimesChangedCallback()
        {
            ChangedStartEndTimesForDecoration(mSelectedDecoration, mDecorationStartEndSlideTimeControl.StartTime, mDecorationStartEndSlideTimeControl.EndTime);  
        }

        //*******************************************************************
        private void ChangedStartEndTimesForDecoration(CAnimatedDecoration decoration, float startTime, float endTime )
        {
            if (decoration == null)
            {
                return;
            }

            decoration.StartOffsetTimeRawValue = startTime;

            if (Math.Abs(endTime - mDecorationStartEndSlideTimeControl.MaxTime) < 0.001)
            {
                endTime = CAnimatedDecoration.SlideEnd;
            }

            decoration.EndOffsetTimeRawValue = endTime;

            // If this is a video decoration, then changing start/end time may validate/invalidate the thumbnail slide length combo.
            // Update it now
            if (decoration is CVideoDecoration)
            {
                Thumbnail t = Form1.mMainForm.GetDecorationManager().SlideThumbnail;
                if (t != null)
                {
                    t.UpdateSlideLengthComboEnabled();
                }
            }

            CGlobals.mCurrentProject.DeclareChange("Decoration time changed");
        }

        //*******************************************************************
        private void mEffectsEditor_Click(object sender, EventArgs e)
        {
            CSlide slide = this.mMainForm.GetDecorationManager().mCurrentSlide;

            if (slide != null)
            {
                CSlideShow slideshow = null;
                if (slide.PartOfAMenu == false)
                {
                    slideshow = Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow;
                }

                ShowSelectDecorationMotionForm(mSelectedDecoration, slide, slideshow);
            }
        }

        //*******************************************************************
        public void ShowSelectDecorationMotionForm(CAnimatedDecoration forDecoration, CSlide slide, CSlideShow slideshow)
        {
            //
            // The decoration start/time can be altereted in this class as well as the select decoration motion form. We set a 
            // callback for changing the decoration times here so we don't end up with duplicate code.
            //
            ChangedStartEndTimesForDecorationDelegate myCallback = new ChangedStartEndTimesForDecorationDelegate(ChangedStartEndTimesForDecoration);

            SelectDecorationMotionForm selectMotionForm = new SelectDecorationMotionForm(mSelectedDecoration, slide as CImageSlide, slideshow, myCallback);
            selectMotionForm.ShowDialog();

            CGlobals.mCurrentProject.DeclareChange("Edit motion effects");

            //
            // Update our currently selected decoration in case values have changed
            //
            if (mSelectedDecoration !=null )
            {
                CImageSlide imageSlide = slide as CImageSlide;
                if (imageSlide != null)
                {
                    EnableControls(mSelectedDecoration, imageSlide);
                }
            }
        }
    }
}
