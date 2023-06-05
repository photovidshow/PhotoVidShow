using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;
using DVDSlideshow;
using ManagedCore;

namespace dvdslideshowfontend
{
	/// <summary>
	/// Summary description for Thumbnail.
	/// </summary>
	/// 
	
	// represents the thumbnail icon in the scrollable slidehow view
    public class SlideshowTimeLabelComponentControl : UserControl
	{
		#region Fields
		#endregion
		private System.Windows.Forms.Label time;

        SlideshowTimeLabelComponent mParentTimeLabel = null;
	
        public SlideshowTimeLabelComponent ParentTimeLabel
        {
            get { return mParentTimeLabel; }
            set { mParentTimeLabel = value; }
        }

		#region Properties
	
		
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
            this.time = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // time
            // 
            this.time.BackColor = System.Drawing.Color.Transparent;
            this.time.Dock = System.Windows.Forms.DockStyle.Fill;
            this.time.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.time.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.time.ForeColor = System.Drawing.SystemColors.ControlText;
            this.time.Location = new System.Drawing.Point(2, 2);
            this.time.Name = "time";
            this.time.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.time.Size = new System.Drawing.Size(40, 12);
            this.time.TabIndex = 0;
            this.time.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SlideshowTimeLabelComponent
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            this.Controls.Add(this.time);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SlideshowTimeLabelComponent";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Size = new System.Drawing.Size(44, 16);
            this.ResumeLayout(false);

		}
		#endregion	

		//*******************************************************************
		public void Blank()
		{
            if (mParentTimeLabel != null)
            {
                mParentTimeLabel.TimeLabelControl = null;
            }
            mParentTimeLabel = null;
		}

		//*******************************************************************
        public SlideshowTimeLabelComponentControl()
		{
		    InitializeComponent();
            Form1.ReduceFontSizeToMatchDPI(this.Controls);		
		}

        //*******************************************************************
        private delegate void SetTimeDelegate(string t);
        public void SetTime(string t)
        {
            if (Form1.mMainForm.InvokeRequired == true)
            {
                SetTimeDelegate my_delegate = new SetTimeDelegate(SetTime);
                Form1.mMainForm.BeginInvoke(my_delegate, new object[] { t });
                return;
            }

            time.Text = t;
        }
	}
}
