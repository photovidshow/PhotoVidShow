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
using CustomButton;

namespace dvdslideshowfontend
{
	/// <summary>
	/// Summary description for Thumbnail.
    /// Represents a general thumnbail control seen in disc menu tabs.
	/// </summary>
	///
	public class MenuSelectionThumbnail : System.Windows.Forms.UserControl
	{  
        private int mId;
        private MenuSelectionThumbnailDelegate mSelectedCallback;

		#region Fields
		private string _filePath = "";
		private System.Drawing.Image _image = null;
		#endregion

		#region Properties
		public string FilePath
		{
			get { return _filePath; }
		}
		public System.Drawing.Image Image
		{
			get { return _image; }
		}
		public bool Handled
		{
			get { return !this.pictureBox.BackColor.Equals(SystemColors.Window); }
			set
			{
				if (value)
				{
					this.pictureBox.BackColor = Color.FromArgb(255, 192, 128);
				}
				else
				{
					this.pictureBox.BackColor = SystemColors.Window;
				}
			}
		}
		#endregion

		#region Generated Fields
		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.PictureBox pictureBox;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
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
			this.panel = new System.Windows.Forms.Panel();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.panel.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel
			// 
			this.panel.BackColor = System.Drawing.SystemColors.Control;
			this.panel.Controls.Add(this.pictureBox);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Location = new System.Drawing.Point(2, 2);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(60, 44);
			this.panel.TabIndex = 0;
			// 
			// pictureBox
			// 
			this.pictureBox.BackColor = System.Drawing.SystemColors.Window;
			this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox.Location = new System.Drawing.Point(0, 0);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(60, 44);
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBox.TabIndex = 1;
			this.pictureBox.TabStop = false;
			// 
			// MenuLayoutThumbnail
			// 
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.Controls.Add(this.panel);
			this.DockPadding.All = 2;
			this.Name = "MenuLayoutThumbnail";
			this.Size = new System.Drawing.Size(64, 48);
			this.panel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		//*******************************************************************
		public void OnMouseDownThumbnail(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Form1.mMainForm.GetDecorationManager().TransferTextBoxToTextDecoration();
			Form1.mMainForm.GetDecorationManager().mPB.Focus();

            mSelectedCallback(mId);
			this.BackColor = FrontEndGlobals.mThumbnailSelectColour;
		}

		//*******************************************************************
		public void OnMouseUpThumbnail(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			this.BackColor = FrontEndGlobals.mThumbnailBorderMouseHoverColour;
		}

		//*******************************************************************
		public void OnMouseLeaveThumbnail(object sender, System.EventArgs e)
		{
			this.BackColor = FrontEndGlobals.mThumbnailBorderNormalColour;
		}

		//*******************************************************************
		public void OnMouseEnterThumbnail(object sender, System.EventArgs e)
		{
			if (this.BackColor != FrontEndGlobals.mThumbnailSelectColour &&
				this.BackColor != FrontEndGlobals.mThumbnailBorderMouseHoverColour)
			{
				this.BackColor = FrontEndGlobals.mThumbnailBorderMouseHoverColour;
			}
		}
		
		//*******************************************************************
        public MenuSelectionThumbnail(string filename, int id, MenuSelectionThumbnailDelegate callback)
		{
			InitializeComponent();
            mSelectedCallback = callback;
			mId = id;
		
			pictureBox.MouseDown+=new MouseEventHandler(OnMouseDownThumbnail);
			pictureBox.MouseEnter+= new EventHandler(OnMouseEnterThumbnail);
			pictureBox.MouseUp+=new MouseEventHandler(OnMouseUpThumbnail);
			pictureBox.MouseLeave+=new EventHandler(OnMouseLeaveThumbnail);

			CImage i = new CImage(filename,false);
			if (i==null)
			{
                CDebugLog.GetInstance().Error("Creating an image for '" + filename + "' failed in MenuLayoutThumbnail");
				return;
			}

            int width = this.pictureBox.Width;
            int height = this.pictureBox.Height;
            if (width < 5) width = 5;
            if (height < 5) height = 5;

            pictureBox.Image = i.GetThumbnailImage(width, height);       
        }
	}
}
