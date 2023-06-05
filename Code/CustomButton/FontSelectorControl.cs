using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;
using System.Drawing.Imaging;

namespace CustomButton
{
    public partial class FontSelectorControl : UserControl
    {
        private class TextStyleListViewItem : ListViewItem
        {
            public CTextStyle mStyle;
            public TextStyleListViewItem(string name, CTextStyle style)
                : base(name)
            {
                mStyle = style;
            }

        }

        // Only have one, so it remembers custom colours
        static ColorDialog mColorPicker = new ColorDialog();

        public delegate void DoneCallbackDelegate(CTextStyle style);

        public event DoneCallbackDelegate Done; 

        private CTextStyle mForTextStyle;
        private CTextStyle mOriginalStyle;
        private GDITextDrawer mTextDrawer = new GDITextDrawer();
        private string mPreviewText = "AbCdEeFf";

        //*******************************************************************
        public FontSelectorControl()
        {
            InitializeComponent();
            mFontCombo.Populate(false);
            mColorPicker.AnyColor = true;
            mTextExamplePictureBox.Image = new Bitmap(mTextExamplePictureBox.Width, mTextExamplePictureBox.Height, PixelFormat.Format32bppArgb);

            mStylesListView.BackColor = Color.DarkGray;
        }

        //*******************************************************************
        public void SetForTextStyle( CTextStyle forTextStyle, string previewText)
        {
            mPreviewText = previewText;
            mOriginalStyle = forTextStyle;
            mForTextStyle = forTextStyle.Clone();

            mTextColourPicker.BackColor = mForTextStyle.TextColor;
            mFontCombo.Text = mForTextStyle.FontName;
            mBoldCheckBox.Checked = mForTextStyle.Bold;
            mItalicCheckBox.Checked = mForTextStyle.Italic;
            mUnderlineCheckBox.Checked = mForTextStyle.UnderLine;

            mOutlineCheckBox.Checked = mForTextStyle.Outline;
            mOutlineGroupBox.Enabled = mForTextStyle.Outline;

            mOutlineColourButton.BackColor = mForTextStyle.OutlineColor;
            mOutlineWidthTrackBar.Value = mForTextStyle.OutlineLength;
            mOutlineWidthText.Text = mOutlineWidthTrackBar.Value.ToString();
            mOutlineAlphaTrackBar.Value = mForTextStyle.OutlineAlpha;
            mOutlineStrengthTextBox.Text = ((int)(((float)mOutlineAlphaTrackBar.Value) / 2.55f)).ToString();

            mGradientCheckBox.Checked = mForTextStyle.Gradient;
            mGradientGroupBox.Enabled = mGradientCheckBox.Checked;
            mGradientColourButton.BackColor = mForTextStyle.TextColor2;

            mShadowTickbox.Checked = mForTextStyle.Shadow;
            mShadowColourButton.BackColor = mForTextStyle.ShadowColor;

            int length = (int)((mForTextStyle.GetShadowLength() * 5)+0.4999f);
            if (length < 0) length = 0;
            if (length > 100) length = 100;

            mShadowLengthTrackBar.Value = length;
            mShadowLengthTextBox.Text = length.ToString();
            mShadowAngleTrackBar.Value = (int)(mForTextStyle.GetShadowAngle()+0.4999f);
            mShadowAngleTextBox.Text = mShadowAngleTrackBar.Value.ToString();
            mShadowAlphaTrackBar.Value = mForTextStyle.ShadowAlpha;
            mShadowGroupBox.Enabled = mShadowTickbox.Checked;
            mShadowStrengthTextBox.Text = ((int)(((float)mShadowAlphaTrackBar.Value) / 2.55f)).ToString();
            DrawPrieviewText();

            if (mStylesListView.Items.Count == 0)
            {
                PopulateStylesListView();
            }
        }

        //*******************************************************************
        private void PopulateStylesListView()
        {
            List<CTextStyle> styles = CTextStyleDatabase.GetInstance().Styles;

            TextStyleListViewItem[] items = new TextStyleListViewItem[styles.Count];
            
            int index =0;
            bool foundSelect = false;

            foreach (CTextStyle style in styles)
            {
                string name = style.Name;

                if (name == "") name = "User Defined";

                TextStyleListViewItem tslvi = new TextStyleListViewItem(name, style);
                items[index] = tslvi;

                int w = mImageList.ImageSize.Width;
                int h = mImageList.ImageSize.Height;

                Bitmap b = new Bitmap(w, h, PixelFormat.Format32bppArgb);

                using (Graphics g = Graphics.FromImage(b))
                {
                    CTextStyle st = style.Clone();
                    st.FontSize = 18;
                    st.Format = new StringFormat();
                    g.Clear(Color.DarkGray);
                    mTextDrawer.DrawString(g, "AbCd", 0, 5, st);
                }

                mImageList.Images.Add(b);
                tslvi.ImageIndex = index;

                index++;

                if (style.IsSameAs(mForTextStyle) == true && foundSelect == false)
                {
                    tslvi.Selected = true;
                    foundSelect = true;
                }
            }

            mStylesListView.Items.AddRange(items);
            mStylesListView.Select();

        }


        //*******************************************************************
        private void mTextColourPicker_Click(object sender, EventArgs e)
        {
            mColorPicker.ShowDialog();
            mTextColourPicker.BackColor = mColorPicker.Color;
            mForTextStyle.TextColor = mColorPicker.Color;
            DrawPrieviewText();
        }


        //*******************************************************************
        private void DrawPrieviewText()
        {
            using (Graphics g = Graphics.FromImage(mTextExamplePictureBox.Image))
            {
                CTextStyle st = mForTextStyle.Clone();
                st.FontSize = 54;
                st.Format = new StringFormat();
                g.Clear(Color.DarkGray);
                string preview = mPreviewText;
                preview = preview.Trim();
                if (preview.Length > 8)
                {
                    preview = preview.Substring(0, 8);
                }

                mTextDrawer.DrawString(g, preview, 0, 5, st);
                mTextExamplePictureBox.Invalidate();
            }
        }

        //*******************************************************************
        private void mOutlineCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mForTextStyle.Outline != mOutlineCheckBox.Checked)
            {
                mOutlineGroupBox.Enabled = mOutlineCheckBox.Checked;
                mForTextStyle.Outline = mOutlineCheckBox.Checked;
                DrawPrieviewText();
            }
        }
   

        //*******************************************************************
        private void mOutlineWidthTrackBar_Scroll(object sender, EventArgs e)
        {
            if (mForTextStyle.OutlineLength != mOutlineWidthTrackBar.Value)
            {
                mForTextStyle.OutlineLength = mOutlineWidthTrackBar.Value;
                mOutlineWidthText.Text = mOutlineWidthTrackBar.Value.ToString();
                DrawPrieviewText();
            }
        }

        //*******************************************************************
        private void mOutlineAlphaTrackBar_Scroll(object sender, EventArgs e)
        {
            if (mForTextStyle.OutlineAlpha != mOutlineAlphaTrackBar.Value)
            {
                mForTextStyle.OutlineAlpha = mOutlineAlphaTrackBar.Value;
                mOutlineStrengthTextBox.Text = ((int)(((float)mOutlineAlphaTrackBar.Value) / 2.55f)).ToString();
                DrawPrieviewText();
            }
        }

        //*******************************************************************
        private void mOutlineColourButton_Click(object sender, EventArgs e)
        {
            mColorPicker.ShowDialog();
            this.mOutlineColourButton.BackColor = mColorPicker.Color;
            mForTextStyle.OutlineColor = mColorPicker.Color;
            DrawPrieviewText();
        }


        //*******************************************************************
        private void mGradientCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mForTextStyle.Gradient != mGradientCheckBox.Checked)
            {
                mForTextStyle.Gradient = mGradientCheckBox.Checked;
                mGradientGroupBox.Enabled = mGradientCheckBox.Checked;
                DrawPrieviewText();
            }
        }

        //*******************************************************************
        private void mGradientColourButton_Click(object sender, EventArgs e)
        {
            mColorPicker.ShowDialog();
            this.mGradientColourButton.BackColor = mColorPicker.Color;
            mForTextStyle.TextColor2 = mColorPicker.Color;
            DrawPrieviewText();
        }

      
        //*******************************************************************
        private void mShadowTickbox_CheckedChanged(object sender, EventArgs e)
        {
            if (mForTextStyle.Shadow != mShadowTickbox.Checked)
            {
                mShadowGroupBox.Enabled = mShadowTickbox.Checked;
                mForTextStyle.Shadow = mShadowTickbox.Checked;
                DrawPrieviewText();
            }
        }


        //*******************************************************************
        private void mShadowColourButton_Click(object sender, EventArgs e)
        {
            mColorPicker.ShowDialog();
            this.mShadowColourButton.BackColor = mColorPicker.Color;
            mForTextStyle.ShadowColor = mColorPicker.Color;
            DrawPrieviewText();
        }


        //*******************************************************************
        private void mShadowLengthTrackBar_Scroll(object sender, EventArgs e)
        {
            float val = ((float)mShadowLengthTrackBar.Value) /5.0f;

            if (mForTextStyle.GetShadowLength() != val)
            {
                mShadowLengthTextBox.Text = mShadowLengthTrackBar.Value.ToString();

                mForTextStyle.SetShadowLengthAndAndAngle(val, mShadowAngleTrackBar.Value);
                DrawPrieviewText();
            }

        }

        //*******************************************************************
        private void mShadowAngleTrackBar_Scroll(object sender, EventArgs e)
        {
            float length = mForTextStyle.GetShadowLength();

            mForTextStyle.SetShadowLengthAndAndAngle(length, mShadowAngleTrackBar.Value);
            mShadowAngleTextBox.Text = mShadowAngleTrackBar.Value.ToString();
            DrawPrieviewText();
        }

        //*******************************************************************
        private void mShadowAlphaTrackBar_Scroll(object sender, EventArgs e)
        {
            if (mForTextStyle.ShadowAlpha != mShadowAlphaTrackBar.Value)
            {
                mForTextStyle.ShadowAlpha = mShadowAlphaTrackBar.Value;
                mShadowStrengthTextBox.Text = ((int)(((float)mShadowAlphaTrackBar.Value) / 2.55f)).ToString();

                DrawPrieviewText();
            }

        }

        //*******************************************************************
        private void mFontCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mForTextStyle != null)
            {
                if (mFontCombo.Text != mForTextStyle.FontName)
                {
                    mForTextStyle.FontName = mFontCombo.Text;
                    DrawPrieviewText();
                }
            }
        }

        //*******************************************************************
        private void mBoldCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mForTextStyle.Bold != mBoldCheckBox.Checked)
            {
                mForTextStyle.Bold = mBoldCheckBox.Checked;
                DrawPrieviewText();
            }
        }

        //*******************************************************************
        private void mUnderlineCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mForTextStyle.UnderLine != mUnderlineCheckBox.Checked)
            {
                mForTextStyle.UnderLine = mUnderlineCheckBox.Checked;
                DrawPrieviewText();
            }
        }

        //*******************************************************************
        private void mItalicCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mForTextStyle.Italic != mItalicCheckBox.Checked)
            {
                mForTextStyle.Italic = mItalicCheckBox.Checked;
                DrawPrieviewText();
            }
        }

        //*******************************************************************
        public void DisposeAll()
        {
            Image i = mTextExamplePictureBox.Image;
            mTextExamplePictureBox.Image = null;
            if (i != null)
            {
                i.Dispose();
            }
        }

        //*******************************************************************
        private void mApplyButton_Click(object sender, EventArgs e)
        {
            DisposeAll();

            if (Done!=null)
            {
                Done(mForTextStyle);
            }
        }
  
        //*******************************************************************
        private void mCancelButton_Click(object sender, EventArgs e)
        {
            DisposeAll();

            if (Done != null)
            {
                Done(null);
            }
        }

        //*******************************************************************
        private void mStylesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selection = mStylesListView.SelectedItems;
            if (selection.Count > 0)
            {
                TextStyleListViewItem item = selection[0] as TextStyleListViewItem;
                if (item != null)
                {
                    SetForTextStyle(item.mStyle, mPreviewText);
                }
            }
        }
    }
}
