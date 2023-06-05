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

namespace CustomButton
{
    public partial class OrderLayersForm : Form
    {
        private CImageSlide mForSlide;
        private MiniPreviewController mPreviewController = null;
        private List<CDecoration> mDecorationsList = new List<CDecoration>();

        //******************************************************************************************
        public OrderLayersForm(CImageSlide forSlide, CSlideShow forSlideShow)
        {
         
            mForSlide = forSlide;
        
            InitializeComponent();

            this.mHighlightSelectedDecorationCheckBox.Checked = FrontEndGlobals.mShowSelectedMotionDecoration;

            if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV4_3)
            {
                int oldwidth = mPreviewPictureBox.Width;

                mPreviewPictureBox.Width = (int)(((float)mPreviewPictureBox.Height) * 1.33333f);

                this.ClientSize = new Size(this.ClientSize.Width - (oldwidth - mPreviewPictureBox.Width), this.ClientSize.Height);
            }

            this.SuspendLayout();
            ArrayList list = mForSlide.GetAllAndSubDecorations();

            foreach (CDecoration d in list)
            {
                string templatestring="";
                CImageDecoration imageDecor = d as CImageDecoration;
                if (imageDecor != null)
                {
                    int templateNumber = imageDecor.OriginalTemplateImageNumber;
                    if (templateNumber != 0)
                    {
                        templatestring = " (" + templateNumber.ToString() + ")";
                    }
                }

                CClipArtDecoration cad = d as CClipArtDecoration;
                if (cad != null)
                {
                    if (cad.IsBorderDecoration())
                    {                      
                        string  name ="(Border) ";
                        name += FilterTabControlBase.GetFilterNameFromFilename(cad.mImage.ImageFilename);
                        name += templatestring;
                        mColorListBox.AddTop(name, Color.Gray);
                    }
                    else if (cad.IsBackgroundDecoration())
                    {
                        string name = "(Background) ";
                        name += FilterTabControlBase.GetFilterNameFromFilename(cad.mImage.ImageFilename);
                        name += templatestring;
                        mColorListBox.AddTop(name, Color.Gray);
                    }
                    else if (cad.IsFilter())
                    {
                        string name = "(Filter) " + FilterTabControlBase.GetFilterNameFromFilename(cad.mImage.ImageFilename);
                        name += templatestring;
                        mColorListBox.AddTop(name, Color.Blue);
                    }
                    else
                    {
                        mColorListBox.AddTop("(Image) " + Path.GetFileName(cad.mImage.ImageFilename) + templatestring, Color.Black);
                    }

                    mDecorationsList.Insert(0,cad);
                    continue;
                }

                CVideoDecoration vd = d as CVideoDecoration;
                if (vd != null)
                {
                    if (vd.IsBorderDecoration())
                    {
                        string name = "(Border) ";
                        name += FilterTabControlBase.GetFilterNameFromFilename(vd.GetFilename());
                        name += templatestring;
                        mColorListBox.AddTop(name, Color.Gray);
                    }

                    else if (vd.IsFilter())
                    {
                        string name = "(Filter) " + FilterTabControlBase.GetFilterNameFromFilename(vd.GetFilename());
                        mColorListBox.AddTop(name, Color.Blue);
                    }
                    else
                    {
                        mColorListBox.AddTop("(Video) "+ Path.GetFileName(vd.GetFilename())+templatestring, Color.Black);
                    }

                    mDecorationsList.Insert(0,vd);
                    continue;
                }

                CBlurFilterDecoration bd = d as CBlurFilterDecoration;
                if (bd != null)
                {
                    mColorListBox.AddTop("(Filter) Blur", Color.Purple);
                    mDecorationsList.Insert(0, bd);
                    continue;
                }

                CColourTransformDecoration ctd = d as CColourTransformDecoration;
                if (ctd != null)
                {
                    mColorListBox.AddTop("(Filter) Adjust Colour", Color.Red);
                    mDecorationsList.Insert(0, ctd);
                    continue;
                }

                CMonotoneTransformDecoration mtd = d as CMonotoneTransformDecoration;
                if (mtd != null)
                {
                    mColorListBox.AddTop("(Filter) Monotone", Color.DarkGray);
                    mDecorationsList.Insert(0, mtd);
                    continue;
                }

                CSepiaTransformDecoration std = d as CSepiaTransformDecoration;
                if (std != null)
                {
                    mColorListBox.AddTop("(Filter) Sepia", Color.SaddleBrown);
                    mDecorationsList.Insert(0, std);
                    continue;
                }

                CTextDecoration td = d as CTextDecoration;
                if (td != null)
                {
                    if (td.VCDNumber == true && (
                        CGlobals.mCurrentProject.DiskPreferences.OutputVideoType != CGlobals.VideoType.SVCD &&
                        CGlobals.mCurrentProject.DiskPreferences.OutputVideoType != CGlobals.VideoType.VCD) )
                    {
                        continue;
                    }

                    string text = td.mText;
                    if (text.Length > 40) text = text.Substring(0, 40);
                    mColorListBox.AddTop("(Text) " + '"' + text + '"', Color.Green);
                    mDecorationsList.Insert(0, td);
                    continue;
                }

                CMenuButton msb = d as CMenuSlideshowButton;
                if (msb != null)
                {
                    mColorListBox.AddTop("(Menu item) Slideshow button", Color.DarkOrchid);
                    mDecorationsList.Insert(0, msb);
                    continue;
                }

                CMenuButton mlb = d as CMenuLinkButton;
                if (mlb != null)
                {
                    mColorListBox.AddTop("(Menu item) Menu link button", Color.DarkOrchid);
                    mDecorationsList.Insert(0, mlb);
                    continue;
                }

                CMenuPlayAllButton mpab = d as CMenuPlayAllButton;
                if (mpab != null)
                {
                    mColorListBox.AddTop("(Menu item) Menu "+'"'+ "Play all" + '"' + " button" , Color.DarkOrchid);
                    mDecorationsList.Insert(0, mpab);
                    continue;
                }

                CMenuPlayAllLoopedButton mpalb = d as CMenuPlayAllLoopedButton;
                if (mpalb != null)
                {
                    mColorListBox.AddTop("(Menu item) Menu " +'"' + "Play all looped" +'"' +" button", Color.DarkOrchid);
                    mDecorationsList.Insert(0, mpalb);
                    continue;
                }


            }
        
            mPreviewController = new MiniPreviewController(
                       forSlideShow,
                       mForSlide,
                       mPreviewPictureBox);

            mPreviewController.PreRenderCallback = this.PreRenderCallback;

            this.ResumeLayout();
        }


        //******************************************************************************************
        private void PreRenderCallback(RenderVideoParameters rvp)
        {
            if (FrontEndGlobals.mShowSelectedMotionDecoration == false) return;

            int selectedIndex = mColorListBox.SelectedIndex;
            if (selectedIndex == -1) return;

            if (selectedIndex >= mDecorationsList.Count) return;

            CDecoration selectedDecoration = mDecorationsList[selectedIndex];

            CImageDecoration imageDecor = selectedDecoration as CImageDecoration;
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
        private void mColorListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SuspendLayout();
            try
            {
                int selelectedIndex = mColorListBox.SelectedIndex;
                if (selelectedIndex == -1)
                {
                    mMoveUpButton.Enabled = false;
                    mMoveDownButton.Enabled = false;
                    mRemoveButton.Enabled = false;
                    return;
                }

                CDecoration selectedDecoration = mDecorationsList[selelectedIndex];

                int count = mDecorationsList.Count;

                if (count > 0)
                {
                    if (mDecorationsList[count - 1].IsBackgroundDecoration() == true)
                    {
                        count--;
                    }
                }
                if (count == 0) return;

                // if seleted border, then disable all options except remove
                if (selectedDecoration.IsBorderDecoration() == true ||
                    selectedDecoration.IsBackgroundDecoration() == true)
                {
                    mMoveDownButton.Enabled = false;
                    mMoveUpButton.Enabled = false;
                    mRemoveButton.Enabled = true;
                    return;
                }

                // is there a border on top?
                int top = 0;
                if (count > 0 && mDecorationsList[0].IsBorderDecoration() == true)
                {
                    top = 1;
                }

                if (selelectedIndex > top)
                {
                    mMoveUpButton.Enabled = true;
                }
                else
                {
                    mMoveUpButton.Enabled = false;
                }

                if (selelectedIndex < count - 1)
                {
                    mMoveDownButton.Enabled = true;
                }
                else
                {
                    mMoveDownButton.Enabled = false;
                }

                //
                // If a menu slideshow button or a link button, then disable remove.
                //
                if (selectedDecoration is CMenuSlideshowButton || selectedDecoration is CMenuLinkButton)
                {
                    mRemoveButton.Enabled = false;
                }
                else
                {
                    mRemoveButton.Enabled = true;
                }
            }
            finally
            {
                this.ResumeLayout();
            }   
        }

        //******************************************************************************************
        private void mMoveUpButton_Click(object sender, EventArgs e)
        {    
            int selelectedIndex = mColorListBox.SelectedIndex;
            if (selelectedIndex > 0)
            {
                CDecoration decora = mDecorationsList[selelectedIndex - 1];
                CDecoration decorb = mDecorationsList[selelectedIndex];

                mForSlide.SwapDecorations(decora, decorb);

                mColorListBox.Swap(selelectedIndex - 1, selelectedIndex);

                mDecorationsList[selelectedIndex - 1] = decorb;
                mDecorationsList[selelectedIndex] = decora;

                mColorListBox.SetSelected(selelectedIndex - 1, true);
            }
        }

        //******************************************************************************************
        private void mMoveDownButton_Click(object sender, EventArgs e)
        {
            int selelectedIndex = mColorListBox.SelectedIndex;
            int count = mColorListBox.Items.Count;
            if (selelectedIndex >= 0 && selelectedIndex < count-1)
            {
                CDecoration decora = mDecorationsList[selelectedIndex + 1];
                CDecoration decorb = mDecorationsList[selelectedIndex];

                mForSlide.SwapDecorations(decora, decorb);

                mColorListBox.Swap(selelectedIndex + 1, selelectedIndex);

                mDecorationsList[selelectedIndex + 1] = decorb;
                mDecorationsList[selelectedIndex] = decora;

                mColorListBox.SetSelected(selelectedIndex + 1, true);
            }
        }

        //******************************************************************************************
        private void mRemoveButton_Click(object sender, EventArgs e)
        {
            int selelectedIndex = mColorListBox.SelectedIndex;
            if (selelectedIndex >= 0)
            {
                CDecoration decor = mDecorationsList[selelectedIndex];
                mForSlide.RemoveDecoration(decor);
                mColorListBox.RemoveAt(selelectedIndex);
                mDecorationsList.RemoveAt(selelectedIndex);
            }
        }

        //******************************************************************************************
        private void OrderLayersForm_FormClosing(object sender, FormClosingEventArgs e)
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
