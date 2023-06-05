using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;

namespace CustomButton
{
    public partial class FontSelectorForm : Form
    {
        private CTextStyle mStyle;

        public CTextStyle Style
        {
            get { return mStyle; }
        }

        public FontSelectorForm()
        {
            InitializeComponent();
        }

        public void SetForTextStyle(CTextStyle style, string previewText)
        {
            mStyle = style;
            mFontSelectorControl1.SetForTextStyle(style, previewText);
        }

        private void mFontSelectorControl1_Done(CTextStyle style)
        {
            if (style != null)
            {
                mStyle = style;
                DialogResult = DialogResult.OK;
            }
            else
            {
                DialogResult = DialogResult.Cancel;
            }
            this.Close();
        }

        private void FontSelectorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mFontSelectorControl1.DisposeAll();
        }
    }
}
