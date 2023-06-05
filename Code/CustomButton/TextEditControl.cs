using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;
using System.Drawing.Imaging;
using System.Collections;

namespace CustomButton
{
    public partial class TextEditControl : UserControl
    {
        public delegate void ChangedCallbackDelegate();

        public event ChangedCallbackDelegate PVSTextChanged;

        private MiniPreviewController mPriviewController;
        private CTextDecoration mForDecoration;
        private int mSnapShotFrame = 0;
        private CSlide mForSlide;

        //*************************************************************************************************
        public TextEditControl()
        {
            InitializeComponent();  
        }

        //*************************************************************************************************
        public void SetForDecoration(CTextDecoration td, Control window, CSlide forSlide, CSlideShow forSlideshow)
        {
            mForDecoration = td;
            mForSlide = forSlide;

            window.Visible = true;

          //  forPb.Visible = false;

            if (td.TemplateEditable == CTextDecoration.TemplateEditableType.SingleLine)
            {
                mRichTextBox1.Multiline = false;
                mRichTextBox1.Height = 24;
                mRichTextBox1.MaxLength=100;
            }
            else
            {
                mRichTextBox1.Multiline = true;
                mRichTextBox1.Height = 300;
                mRichTextBox1.MaxLength=1000;
            }

            // if multi slide slide, find slide decor is in
            if (forSlide is CMultiSlideSlide)
            {
                mSnapShotFrame = GetPreviewFrameForMultiSlideSlide(forSlide as CMultiSlideSlide, mForDecoration);
            }
            else
            {
                mSnapShotFrame = GetPreviewFrameForSingleImageSlide(forSlide as CImageSlide, mForDecoration);
            }

            mPriviewController = new MiniPreviewController(forSlideshow, forSlide, window, false);

            Size s = mPriviewController.GetFrameSize();
            if (s.Height != window.Height)
            {
                window.Location = new Point(window.Location.X, window.Location.Y + ((window.Height - s.Height) / 2));
                window.Height = s.Height;
            }

            mRichTextBox1.Text = td.mText;
            mPriviewController.DoSnapshot(mSnapShotFrame, false);

            window.Paint += new PaintEventHandler(Parent_Paint);
        
        }

        //*************************************************************************************************
        // Basically we want to find a frame that best show the text we are editting.  This is done
        // by finding the mid point time in the slide the decoration most appears in.
        private int GetPreviewFrameForMultiSlideSlide(CMultiSlideSlide mss, CTextDecoration textDec)
        {
            int frame =0;
            CImageSlide candidateSlide = null;

            bool foundSlide = false;

            foreach (CImageSlide imageslide in mss.SubSlides)
            {
                if (foundSlide == true)
                {
                    break;
                }
                ArrayList decs = imageslide.GetAllAndSubDecorations();
                foreach (CDecoration dec in decs)
                {
                    CTextDecoration candiateTextDec = dec as CTextDecoration;
                    if (candiateTextDec !=null)
                    {
                        bool matchDecor = false;

                        if ( candiateTextDec.OriginalTemplateImageNumber!=0)
                        {
                            matchDecor = candiateTextDec.OriginalTemplateImageNumber == textDec.OriginalTemplateImageNumber;
                        }
                        else
                        {
                            matchDecor = textDec== candiateTextDec;
                        }

                        if (matchDecor == true)
                        {
                            //Has decor hinted a time to best edit it
                            if (candiateTextDec.HintedEditTime >= 0)
                            {
                                return (int) (CGlobals.mCurrentProject.DiskPreferences.frames_per_second * candiateTextDec.HintedEditTime);
                            }

                            // Has it been hinted to use the slide
                            if (candiateTextDec.SetTextHint == true)
                            {
                                // found break
                                foundSlide = true;
                                candidateSlide = imageslide;
                                break;
                            }

                            if (candidateSlide==null)
                            {
                                candidateSlide = imageslide;
                            }
                            else if(candidateSlide.DisplayLength < imageslide.DisplayLength)
                            {
                                candidateSlide = imageslide;
                            }
                        }
                    }
                }
            }

            if (candidateSlide!=null)
            {
                int offset = candidateSlide.mStartFrameOffset - mss.mStartFrameOffset;
              
                frame = (int)((candidateSlide.DisplayLength / 2.0f) * CGlobals.mCurrentProject.DiskPreferences.frames_per_second);
                frame +=offset;
            }

            return frame;
        }

        //*************************************************************************************************
        private int GetPreviewFrameForSingleImageSlide(CImageSlide forSlide, CTextDecoration textDec)
        {
            int frame =(int)((forSlide.DisplayLength / 2.0f) * CGlobals.mCurrentProject.DiskPreferences.frames_per_second);

            ArrayList decs = forSlide.GetAllAndSubDecorations();
            foreach (CDecoration dec in decs)
            {
                CTextDecoration candiateTextDec = dec as CTextDecoration;
                if (candiateTextDec != null)
                {
                    if (textDec == candiateTextDec)
                    {
                        //Has decor hinted a time to best edit it
                        if (candiateTextDec.HintedEditTime >= 0)
                        {
                            return (int)(CGlobals.mCurrentProject.DiskPreferences.frames_per_second * candiateTextDec.HintedEditTime);
                        }
                    }
                }
            }
            return frame;
        }

        //*************************************************************************************************
        void Parent_Paint(object sender, PaintEventArgs e)
        {
            mRichTextBox1_TextChanged(sender, e);
        }

        //*************************************************************************************************
        private void mRichTextBox1_TextChanged(object sender, EventArgs e)
        {
            float size = mForDecoration.GetFontSizeForCoverageArea();

            if (mRichTextBox1.Text.Length == 0)
            {
                mForDecoration.mText = " ";
            }
            else
            {
                mForDecoration.mText = mRichTextBox1.Text;
            }

            mForDecoration.RecalcCoverageAreaForFontSize(size);

            ReplaceMatchingDecorationsTextOrAndStyle(true, size, false);

            if (PVSTextChanged != null)
            {
                PVSTextChanged();
            }

            // ### SRG TODO G536
            // Force no video seeking, this looks a bit odd but just too slow with it on
            mPriviewController.DoSnapshot(mSnapShotFrame, true);
        }

        
        //*************************************************************************************************
        private void ReplaceMatchingDecorationsTextOrAndStyle(bool text, float size,  bool style)
        {
            // ok replace any other text that has same temple number as us
            CImageSlide imageSlide = mForSlide as CImageSlide;
            if (imageSlide != null)
            {
                ArrayList decs = imageSlide.GetAllAndSubDecorations();

                foreach (CDecoration dec in decs)
                {
                    CTextDecoration td = dec as CTextDecoration;
                    if (td != null && td != mForDecoration)
                    {
                        if (td.OriginalTemplateImageNumber != 0 &&
                            td.OriginalTemplateImageNumber == mForDecoration.OriginalTemplateImageNumber)
                        {
                            if (text == true)
                            {
                                td.mText = mForDecoration.mText;
                                td.RecalcCoverageAreaForFontSize(size);
                            }
                            if (style == true)
                            {
                                td.TextStyle = mForDecoration.TextStyle.Clone();
                                td.InvalidateFont();
                            }
                        }
                    }
                }
            }
        }

        //*************************************************************************************************
        private void mFontButton_Click(object sender, EventArgs e)
        {
            MiniPreviewController controller = MiniPreviewController.GetCurrentPlayingController();
            if (controller != null)
            {
                controller.Pause();
            }

            FontSelectorForm fsm = new FontSelectorForm();
            fsm.SetForTextStyle(mForDecoration.TextStyle, mForDecoration.mText);
            fsm.ShowDialog();
            fsm.Style.Format = mForDecoration.TextStyle.Format;
            fsm.Style.FontSize = mForDecoration.TextStyle.FontSize;

            CTextStyle newStyle = fsm.Style.Clone();
            newStyle.Format = mForDecoration.TextStyle.Format;
            newStyle.FontSize = mForDecoration.TextStyle.FontSize;
            CTextStyleDatabase.GetInstance().AddStyleIfNotAlreadyInDatabase(newStyle);

            mForDecoration.TextStyle = newStyle;

            ReplaceMatchingDecorationsTextOrAndStyle(false, 0, true);

            mRichTextBox1_TextChanged(sender, e);
            if (controller != null)
            {
                controller.Continue();
            }
        }

        //*************************************************************************************************
        private void mClearTextButton_Click(object sender, EventArgs e)
        {
            mRichTextBox1.Text = "";

        }

    }
}
