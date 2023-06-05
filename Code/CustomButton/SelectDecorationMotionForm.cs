using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;
using System.IO;
using System.Collections;
using DVDSlideshow.GraphicsEng;

namespace CustomButton
{

    public delegate void ChangedStartEndTimesForDecorationDelegate(CAnimatedDecoration d, float startTime, float endTime);

    public partial class SelectDecorationMotionForm : Form
    {
        private CImageSlide mForSlide;
        private CSlideShow mForSlideShow;
        private CAnimatedDecoration mForDecoration;
        private MiniPreviewController mPreviewController = null;
        private List<CAnimatedDecoration> mDecorationsList = new List<CAnimatedDecoration>();
        private ChangedStartEndTimesForDecorationDelegate mTimesChangeCallback = null;

        private const string mSlideDefinedMovementString = "Slide defined movement";

        //******************************************************************************************
        public SelectDecorationMotionForm(CAnimatedDecoration forDecoration, CImageSlide forSlide, CSlideShow forSlideShow, ChangedStartEndTimesForDecorationDelegate timesChangeCallback)
        {
            mForSlide = forSlide;
            mForSlideShow = forSlideShow;
            mForDecoration = forDecoration;
            mTimesChangeCallback = timesChangeCallback;

            InitializeComponent();

            mSetStartEndSlideTimes.TimesChangedCallback += mSetStartEndSlideTimes_TimesChangedCallback;
            //
            // Make start/end time trackbar color match ours
            //
            mSetStartEndSlideTimes.SetTrackbarBacgroundColor(this.BackColor);

            if (CGlobals.mIsTemplateUser == true)
            {
                mMotionEffectsEditorButton.Visible = true;
            }

            this.mHighlightSelectedDecorationCheckBox.Checked = FrontEndGlobals.mShowSelectedMotionDecoration;

            if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV4_3)
            {
                int oldwidth = mPreviewPictureBox.Width;

                mPreviewPictureBox.Width = (int)(((float)mPreviewPictureBox.Height) * 1.33333f);

                this.ClientSize = new Size(this.ClientSize.Width - (oldwidth - mPreviewPictureBox.Width), this.ClientSize.Height);
            }

            RebuildForCurrentSlide();
        }

        //******************************************************************************************
        private void RebuildForCurrentSlide()
        {
            this.SuspendLayout();

            if (mPreviewController != null)
            {
                mPreviewController.Stop();
                mPreviewController = null;
            }

            UpdateAndSetControls();

            mSlideLengthTextBox.Text = mForSlide.DisplayLength.ToString();

            ArrayList list = mForSlide.GetAllAndSubDecorations();

            mDecorationsList.Clear();
            mDecorationsColorListBox.Clear();

             // ok here we are allowed to set the motion effects of the group.
            CGroupedDecoration group = mForSlide.GetFirstGroupedDecoration();
            if (group != null)
            {
                mDecorationsColorListBox.AddTop("All Grouped", Color.DarkSlateBlue);
                mDecorationsList.Insert(0, group);
            }

            foreach (CDecoration d in list)
            {          
                if (d.IsBorderDecoration() || d.IsBackgroundDecoration() || d.IsFilter())
                {
                    continue;
                }

                if (d is CImageDecoration == false)
                {
                    continue ;
                }

                CClipArtDecoration cad = d as CClipArtDecoration;
                if (cad != null)
                {                   
                    mDecorationsColorListBox.AddTop("(Image) "+ Path.GetFileName(cad.mImage.ImageFilename), Color.Black);

                    mDecorationsList.Insert(0,cad);                  
                    continue;
                }

                CVideoDecoration vd = d as CVideoDecoration;
                if (vd != null)
                {
                   mDecorationsColorListBox.AddTop("(Video) " + Path.GetFileName(vd.GetFilename()), Color.Black);
                   mDecorationsList.Insert(0,vd);
                   continue;
                }

                CTextDecoration td = d as CTextDecoration;
                if (td != null)
                {
                    if (td.VCDNumber == true && (
                        CGlobals.mCurrentProject.DiskPreferences.OutputVideoType != CGlobals.VideoType.SVCD &&
                        CGlobals.mCurrentProject.DiskPreferences.OutputVideoType != CGlobals.VideoType.VCD))
                    {
                        continue;
                    }

                    string text = td.mText;
                    if (text.Length > 40) text = text.Substring(0, 40);
                    mDecorationsColorListBox.AddTop("(Text) " + '"' + text + '"', Color.Green);
                    mDecorationsList.Insert(0, td);
                    continue;
                }
            }

            mPreviewController = new MiniPreviewController(
                       mForSlideShow,
                       mForSlide,
                       mPreviewPictureBox);

            mPreviewController.PreRenderCallback = this.PreRenderCallback;

            if (mForDecoration != null)
            {
                SelectListItem(mForDecoration);
            }

            this.ResumeLayout();
        }


        //******************************************************************************************
        private void PreRenderCallback(RenderVideoParameters rvp)
        {
            if (FrontEndGlobals.mShowSelectedMotionDecoration == false) return;
            if (mForDecoration == null) return;

            CImageDecoration imageDecor = mForDecoration as CImageDecoration;
            if (imageDecor != null)
            {
                imageDecor.DrawHighlighted = true;
            }
        }
  
        //******************************************************************************************
        private void mDoneButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //******************************************************************************************
        private void SelectListItem(CAnimatedDecoration decoration)
        {
            if (decoration == null) return;

            int index = 0;
            foreach (CAnimatedDecoration d in mDecorationsList)
            {
                if (d == decoration)
                {
                    mDecorationsColorListBox.SelectedIndex = index;
                    break;
                }
                index++;
            }
        }


        //******************************************************************************************
        private void RebuildEffectsListComboBoxs()
        {
            mMotionInEffectComboBox.Items.Clear();

            if (mForDecoration == null)
            {
                mMotionInEffectComboBox.Enabled = false;
                mMotionOutEffectComboBox.Enabled = false;
                return;
            }
            else
            {
                mMotionInEffectComboBox.Enabled = true;
                mMotionOutEffectComboBox.Enabled = true;
            }

            bool showTemplateEffects = CGlobals.mIsTemplateUser;

            List<CAnimatedDecorationEffect> inEffects = CAnimatedDecorationEffectDatabase.GetInstance().GetInEffects(showTemplateEffects);

            List<string> inObjects = new List<string>();
            inObjects.Add("None (Default)");

            if (mForDecoration.OriginalTemplateDefinedMoveInEffect != "")
            {
                inObjects.Add(mSlideDefinedMovementString);
            }
            
            foreach (CAnimatedDecorationEffect effect in inEffects)
            {
                bool include = true;

                if (effect.HasCharacterTimeDelay == true && (mForDecoration is CTextDecoration == false))
                {
                    include = false;
                }

                if (include == true)
                {
                    inObjects.Add(effect.Name);
                }
            }

            mMotionInEffectComboBox.Items.AddRange(inObjects.ToArray());


            // out effects

            this.mMotionOutEffectComboBox.Items.Clear();

            List<CAnimatedDecorationEffect> outEffects = CAnimatedDecorationEffectDatabase.GetInstance().GetOutEffects(showTemplateEffects);

            List<string> outObjects = new List<string>();
            outObjects.Add("None (Default)");

            if (mForDecoration.OriginalTemplateDefinedMoveOutEffect != "")
            {
                inObjects.Add(mSlideDefinedMovementString);
            }

            foreach (CAnimatedDecorationEffect effect in outEffects)
            {
                bool include = true;

                if (effect.HasCharacterTimeDelay == true && (mForDecoration is CTextDecoration == false))
                {
                    include = false;
                }

                if (include == true)
                {
                    outObjects.Add(effect.Name);
                }
            }

            mMotionOutEffectComboBox.Items.AddRange(outObjects.ToArray());

        }

        //******************************************************************************************
        private void SelectEffectsInCombos()
        {
            if (mForDecoration == null)
            {
                return;
            }

            mMotionOutEffectComboBox.Enabled = true;
            mMotionInEffectComboBox.Enabled = true;

            bool templateUser = CGlobals.mIsTemplateUser;

            CAnimatedDecorationEffect inEffect = mForDecoration.MoveInEffect;
            if (inEffect == null)
            {
                mMotionInEffectComboBox.Text="None (Default)";
                mMotionInLengthTextBox.Text = "";
            }
            else
            {
                if (templateUser == true || inEffect.TemplateOnlyEffect == false)
                {
                    mMotionInEffectComboBox.Text = inEffect.Name;
                    if (inEffect.LengthSetToDecorationLength == false)
                    {
                        mMotionInLengthTextBox.Text = inEffect.LengthInSeconds.ToString();
                    }
                    else
                    {
                        mMotionInLengthTextBox.Text = "";
                        DisableAndNullOutEffect();
                    }

                }
                else
                {
                    mMotionInEffectComboBox.Text = mSlideDefinedMovementString;
                    mMotionInLengthTextBox.Text = "";
                }
            }

            CAnimatedDecorationEffect outEffect = mForDecoration.MoveOutEffect;
            if (outEffect == null)
            {
                mMotionOutEffectComboBox.Text="None (Default)";
                mMotionOutLengthTextBox.Text = "";
            }
            else
            {
                if (templateUser == true || outEffect.TemplateOnlyEffect == false)
                {
                    mMotionOutEffectComboBox.Text = outEffect.Name;
                    mMotionOutLengthTextBox.Text = outEffect.LengthInSeconds.ToString();
                }
                else
                {
                    mMotionOutEffectComboBox.Text = mSlideDefinedMovementString;
                    mMotionOutLengthTextBox.Text = "";
                }
            }
        }

        //******************************************************************************************
        private void UpdateAndSetControls()
        {
            RebuildEffectsListComboBoxs();
            SelectEffectsInCombos();
            SetStartEndTimes();
        }


        //******************************************************************************************
        private void SetStartEndTimes()
        {
            if (mForDecoration == null)
            {
                mSetStartEndSlideTimes.Enabled = false;
                mSetStartEndSlideTimes.Set(0, 8, 8);
                return;
            }


            float slideLength = mForSlide.DisplayLength;


            mSetStartEndSlideTimes.Enabled = true;
            mSetStartEndSlideTimes.Set(mForDecoration.GetStartOffsetTime(slideLength),
                                       mForDecoration.GetEndOffsetTime(slideLength),
                                                    slideLength);
        }

        //******************************************************************************************
        private void mSetStartEndSlideTimes_TimesChangedCallback()
        {
            if (mTimesChangeCallback != null && mForDecoration != null)
            {
                mTimesChangeCallback(mForDecoration, mSetStartEndSlideTimes.StartTime, mSetStartEndSlideTimes.EndTime);
            }
        }


        //******************************************************************************************
        private void mDecorationsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SuspendLayout();
            try
            {
                int selelectedIndex = mDecorationsColorListBox.SelectedIndex;

                if (selelectedIndex >= 0)
                {
                    mForDecoration = mDecorationsList[selelectedIndex] as CAnimatedDecoration;
                }
                UpdateAndSetControls();
            }
            finally
            {
                this.ResumeLayout();
            }   
        }


        //******************************************************************************************
        private void mMotionEffectsEditorButton_Click(object sender, EventArgs e)
        {
            if (mForDecoration == null) return;

            AnimatedDecorationsEditor editor = new AnimatedDecorationsEditor(mForSlideShow, mForSlide, mForDecoration);
            editor.ShowDialog();
            RebuildForCurrentSlide();
        }
  

        //******************************************************************************************
        private void mMotionInEffectComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mForDecoration == null) return;

            bool changed = false;
            bool disableOutEffect = false;

            if (mMotionInEffectComboBox.SelectedIndex == 0)
            {
                if (mForDecoration.MoveInEffect != null)
                {
                    mForDecoration.MoveInEffect = null;
                    mMotionInLengthTextBox.Text = "";
                    changed = true;
                }

            }
            else
            {
                CAnimatedDecorationEffect inEffect=null;

                if (mMotionInEffectComboBox.Text == mSlideDefinedMovementString)
                {
                    if (mForDecoration.OriginalTemplateDefinedMoveInEffect != "")
                    {
                        inEffect = CAnimatedDecorationEffectDatabase.GetInstance().Get(mForDecoration.OriginalTemplateDefinedMoveInEffect);
                    }
                }
                else
                {
                    inEffect = CAnimatedDecorationEffectDatabase.GetInstance().Get(mMotionInEffectComboBox.Text);
                }

                if (inEffect != null && mForDecoration.MoveInEffect != inEffect)
                {
                    // switch off pan zoom if group, cant have both
                    if (mForDecoration is CGroupedDecoration)
                    {
                        mForSlide.UsePanZoom = false;
                    }

                    mForDecoration.MoveInEffect = inEffect;

                    if (inEffect.LengthSetToDecorationLength == false)
                    {
                        mMotionInLengthTextBox.Text = inEffect.LengthInSeconds.ToString();
                    }
                    else
                    {
                        mMotionInLengthTextBox.Text = "";
                        DisableAndNullOutEffect();
                        disableOutEffect=true;
                    }
                    changed = true;
                }
            }

            if (disableOutEffect == false && mMotionOutEffectComboBox.Enabled == false)
            {
                mMotionOutEffectComboBox.Enabled = true;
            }

            if (changed)
            {
                mPreviewController.ResetFrameCountToSlideTime(mForDecoration.GetStartOffsetTime(mForSlide.DisplayLength));
            }
        }

        //******************************************************************************************
        private void DisableAndNullOutEffect()
        {
            mMotionOutEffectComboBox.SelectedIndex = 0;
            mMotionOutEffectComboBox.Enabled = false;
        }


        //******************************************************************************************
        private void mMotionOutEffectComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mForDecoration == null) return;

            bool changed = false;

            if (mMotionOutEffectComboBox.SelectedIndex == 0)
            {
                if (mForDecoration.MoveOutEffect != null)
                {
                    mForDecoration.MoveOutEffect = null;
                    mMotionOutLengthTextBox.Text = "";
                    changed = true;
                }

            }
            else
            {
                CAnimatedDecorationEffect outEffect = CAnimatedDecorationEffectDatabase.GetInstance().Get(mMotionOutEffectComboBox.Text);

                if (outEffect != null && mForDecoration.MoveOutEffect != outEffect)
                {
                    // switch off pan zoom if group, cant have both
                    if (mForDecoration is CGroupedDecoration)
                    {
                        mForSlide.UsePanZoom = false;
                    }

                    mForDecoration.MoveOutEffect = outEffect;
                    mMotionOutLengthTextBox.Text = outEffect.LengthInSeconds.ToString();
                    changed = true;
                }
            }

            if (changed)
            {
                float skipToTime =0;

                if (mForDecoration.MoveOutEffect!=null)
                {
                    skipToTime = mForDecoration.GetEndOffsetTime(mForSlide.DisplayLength) - mForDecoration.MoveOutEffect.LengthInSeconds;
                }

                // move to out effect for decoration
                mPreviewController.ResetFrameCountToSlideTime(skipToTime);
            }
        }

        //******************************************************************************************
        private void SelectDecorationMotionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MiniPreviewController.StopAnyPlayingController();
        }

        //******************************************************************************************
        private void mHighlightSelectedDecorationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            FrontEndGlobals.mShowSelectedMotionDecoration = this.mHighlightSelectedDecorationCheckBox.Checked;      
        }
    }
}
