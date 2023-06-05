using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;
using DVDSlideshow;

namespace dvdslideshowfontend
{
	/// <summary>
    /// Summary description for SlideShowSeperator.
	/// </summary>
	/// 

    // represents the seperator in the scrollable slidehow view
    public class SlideShowSeperator : StoryboardComponent
	{
        private SlideShowSeperatorControl mSeperatorControl = null;
        private Color mBackColor = System.Drawing.SystemColors.WindowFrame;
		
        // **************************************************************************************
        public SlideShowSeperatorControl SeperatorControl
        {
            get { return mSeperatorControl; }
            set
            {
                mSeperatorControl = value;
                if (mSeperatorControl != null)
                {
                    mSeperatorControl.ParentSeperator = this;
                }
            }
        }

        // **************************************************************************************
        public SlideShowSeperator()
        {
            this.Width = 96;
            this.Height = 104;
            mBackColor = Form1.mMainForm.GetSlideShowPanel().BackColor;

        }

        // **************************************************************************************
        public Color BackColor
        {
            get
            {
                return mBackColor;  
            }
            set
            {
                mBackColor = value;
                if (mSeperatorControl != null)
                {
                    mSeperatorControl.BackColor = value;
                }
            }
        }

        // **************************************************************************************
        public void Blank()
        {
            if (mSeperatorControl != null)
            {
                mSeperatorControl.ParentSeperator = null;
            }
            mSeperatorControl = null;
            mBackColor = Form1.mMainForm.GetSlideShowPanel().BackColor; ;
        }

	}
}
