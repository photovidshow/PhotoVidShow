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
    /// Summary description for SlideShowSeperatorControl.
	/// </summary>
	/// 
	
    public class SlideShowSeperatorControl : UserControl
	{
        private SlideShowSeperator mParentSeperator = null;

        public SlideShowSeperator ParentSeperator
        {
            get { return mParentSeperator; }
            set { mParentSeperator = value; }
        }

		#region Fields
	
		#endregion
	
		#region Properties
		
		public bool Handled
		{
			get { return true; }
			set
			{
			}
		}

		#endregion

		#region Generated Fields

		/// <summary> 
		/// Required designer variable.
		/// </summary>
	
		#endregion
		
		#region Generated Methods

		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
	
			this.SuspendLayout();
			// 
			// button1
			// 
		
			// 
			// SlideShowSeperator
			// 
			this.BackColor = System.Drawing.SystemColors.WindowFrame;
		
			this.DockPadding.All = 2;
			this.Name = "";
			this.Size = new System.Drawing.Size(96, 104);
			this.ResumeLayout(false);

		}

        // ***********************************************************************
        public void Blank()
        {
            if (mParentSeperator != null)
            {
                mParentSeperator.SeperatorControl = null;
            }

            mParentSeperator = null;
        }


		#endregion

	}
}
