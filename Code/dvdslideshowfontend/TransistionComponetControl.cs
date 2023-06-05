using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;
using DVDSlideshow;
using ManagedCore;
using DVDSlideshow.GraphicsEng;

namespace dvdslideshowfontend
{
	/// <summary>
	/// Summary description for Thumbnail.
	/// </summary>
	/// 
	
	// represents the thumbnail icon in the scrollable slidehow view
	public class TransitionComponentControl : UserControl
	{
		#region Fields
	
		#endregion

		#region Properties
	
	
		public bool Handled
		{
			get { return !this.mPictureBox.BackColor.Equals(SystemColors.Window); }
			set
			{
				if (value)
				{
					this.mPictureBox.BackColor = Color.FromArgb(255, 192, 128);
				}
				else
				{
					this.mPictureBox.BackColor = SystemColors.Window;
				}
			}
		}
		#endregion

		#region Generated Fields

		private System.Windows.Forms.PictureBox mPictureBox;
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
			this.mPictureBox = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// pictureBox
			// 
			this.mPictureBox.BackColor = System.Drawing.SystemColors.Window;
			this.mPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mPictureBox.Location = new System.Drawing.Point(2, 2);
			this.mPictureBox.Name = "pictureBox";
			this.mPictureBox.Size = new System.Drawing.Size(36, 27);
			this.mPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.mPictureBox.TabIndex = 1;
			this.mPictureBox.TabStop = false;
			// 
			// TransitionComponent
			// 
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.Controls.Add(this.mPictureBox);
			this.DockPadding.All = 2;
			this.Name = "TransitionComponent";
			this.Size = new System.Drawing.Size(40, 31);
			this.ResumeLayout(false);

		}
		#endregion


        private TransitionComponent mParentTransition = null;

        public TransitionComponent ParentTransition
        {
            get { return mParentTransition; }
            set { mParentTransition = value; }
        }

		private object thislock = new object();
		private Color mNonHighlightBorderColor;
	
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

		//*******************************************************************
        public TransitionComponentControl()
		{
			InitializeComponent();
	
			this.mNonHighlightBorderColor = this.BackColor;
			this.mPictureBox.MouseDown += new MouseEventHandler(this.transistion_effect_Click);
			this.mPictureBox.MouseEnter+= new EventHandler(this.OnMouseEnter);
			this.mPictureBox.MouseLeave+= new EventHandler(this.OnMouseLeave);

		}

		//*******************************************************************
		public void Blank()
		{
            this.mPictureBox.Image = null;

            if (mParentTransition != null)
            {
                mParentTransition.TransitionControl = null;
            }
            mParentTransition = null;
		}

		//*******************************************************************
		public void ReConstruct(CImageSlide for_slide, CSlideShow for_slideshow)
		{
			this.mNonHighlightBorderColor = this.BackColor;
		}

		//*******************************************************************
		public void OnMouseEnter(object o, System.EventArgs e)
		{
            int r= mNonHighlightBorderColor.R-40;
            int g=mNonHighlightBorderColor.G-40;
            int b =mNonHighlightBorderColor.B-40;
            if (r<0) r=0;
            if (g<0) g=0;
            if (b<0) b=0;
		
			Color back_color = Color.FromArgb(r,g,b);
			this.BackColor = back_color;
		}

		//*******************************************************************
		
		public void OnMouseLeave(object o, System.EventArgs e)
		{
			this.BackColor = mNonHighlightBorderColor;
		}

		//*******************************************************************
		public void transistion_effect_Click(object o, System.Windows.Forms.MouseEventArgs e)
		{
            if (mParentTransition == null) return;

            Point p = new Point();
            p.X = e.X;
            p.Y = e.Y;

            p = this.PointToScreen(p);

            // handle click at TransitionComponent level
            mParentTransition.TansitionEffectClicked(p);
		}

        //*******************************************************************
        private delegate void SetImageDelegate(Image i);
        public void SetImage(Image i)
        {
            Form1 mainGui = Form1.mMainForm;
        
            // make sure we are on gui thread
            if (mainGui != null && mainGui.InvokeRequired == true)
            {
                mainGui.BeginInvoke(new SetImageDelegate(SetImage), new object[] { i });
                return;
            }

            this.mPictureBox.Image = i;

            if (i != null)
            {
                this.mPictureBox.Height = i.Height;
                this.Height = i.Height;
            }
        }
	}
}
