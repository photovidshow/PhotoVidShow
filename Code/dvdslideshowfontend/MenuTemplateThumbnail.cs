using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;
using DVDSlideshow;
using CustomButton;

namespace dvdslideshowfontend
{
	/// <summary>
	/// Summary description for Thumbnail.
	/// </summary>
	/// 
	
	// represents the thumbnail icon in the scrollable slidehow view
	public class MenuThumbnail : System.Windows.Forms.UserControl
	{
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
		private System.Windows.Forms.ContextMenu contextMenu;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem delete;
		private System.Windows.Forms.MenuItem fullscreen;

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
			this.contextMenu = new System.Windows.Forms.ContextMenu();
			this.delete = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.fullscreen = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
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
			this.panel.Size = new System.Drawing.Size(92, 68);
			this.panel.TabIndex = 0;
			// 
			// pictureBox
			// 
			this.pictureBox.BackColor = System.Drawing.SystemColors.Window;
			this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox.Location = new System.Drawing.Point(0, 0);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(92, 68);
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBox.TabIndex = 1;
			this.pictureBox.TabStop = false;
			// 
			// contextMenu
			// 
			this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.delete,
																						this.menuItem4,
																						this.fullscreen,
																						this.menuItem3});
			// 
			// delete
			// 
			this.delete.Index = 0;
			this.delete.Text = "Delete";
			this.delete.Click += new System.EventHandler(this.menuItem1_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 1;
			this.menuItem4.Text = "-";
			// 
			// fullscreen
			// 
			this.fullscreen.Index = 2;
			this.fullscreen.Text = "View Fullscreen";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 3;
			this.menuItem3.Text = "Decorate";
			// 
			// MenuThumbnail
			// 
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.Controls.Add(this.panel);
			this.DockPadding.All = 2;
			this.Name = "MenuThumbnail";
			this.Size = new System.Drawing.Size(96, 72);
			this.panel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		
		public bool mHighLighted= false ;
		public CSlideShowManager mManager;
		public bool mControlOn = false;
		public Rectangle mDragBoxFromMouseDown ;
		public Cursor MyNormalCursor;
		public Cursor MyNoDropCursor;
		bool   mDeSelectOnMouseUp =false;
	
		
		public string mFileName;
			
		//*******************************************************************
		public void OnMouseDownThumbnail(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();
			this.BackColor = FrontEndGlobals.mThumbnailSelectColour;

			Form1.mMainForm.GetDecorationManager().TransferTextBoxToTextDecoration();
			Form1.mMainForm.GetDecorationManager().mPB.Focus();

			Form1.mMainForm.GetDecorationManager().SetMenuBackdrop(this);

			/*

			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				
			}
			else
			{
				if (mHighLighted==false)
				{
				
					mDeSelectOnMouseUp = false ;
				}
				else
				{
					mDeSelectOnMouseUp = true;
				}
          
				// Remember the point where the mouse down occurred. The DragSize indicates
				// the size that the mouse can move before a drag event should be started.                
				Size dragSize = SystemInformation.DragSize;

				// Create a rectangle using the DragSize, with the mouse position being
				// at the center of the rectangle.
				mDragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width /2),
					e.Y - (dragSize.Height /2)), dragSize);
			}
			*/

		}

		//*******************************************************************
		public void OnMouseUpThumbnail(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			this.BackColor = FrontEndGlobals.mThumbnailBorderMouseHoverColour;

		//	mDragBoxFromMouseDown = Rectangle.Empty;

		//	if(e.Button == MouseButtons.Right)
		//	{
		//		this.contextMenu.Show(this, new Point(e.X, e.Y) );      
		//	}
		}

/*
		private void ListDragSource_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) 
		{
			if ((e.Button & MouseButtons.Left) == MouseButtons.Left) 
			{
				// If the mouse moves outside the rectangle, start the drag.
				if (mDragBoxFromMouseDown != Rectangle.Empty && 
					!mDragBoxFromMouseDown.Contains(e.X, e.Y)) 
				{

					mDeSelectOnMouseUp = false;
					DragDropEffects dropEffect = DoDragDrop(this, DragDropEffects.All | DragDropEffects.Move);
				}	
			}
		}
		*/
		

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
		public MenuThumbnail(string filename)
		{
			//mManager = manager;

			mFileName = filename ;

			InitializeComponent();
			mDragBoxFromMouseDown = Rectangle.Empty;

			pictureBox.MouseDown+=new MouseEventHandler(OnMouseDownThumbnail);
			pictureBox.MouseUp+=new MouseEventHandler(OnMouseUpThumbnail);

			pictureBox.MouseEnter+= new EventHandler(OnMouseEnterThumbnail);
			pictureBox.MouseLeave+=new EventHandler(OnMouseLeaveThumbnail);

            bool isVideo = CGlobals.IsVideoFilename(filename);
      
			try
			{
                string path = System.IO.Path.GetDirectoryName(filename);
                string name = System.IO.Path.GetFileName(filename);
                string nameWithoutExtention = System.IO.Path.GetFileNameWithoutExtension(name);

                string thumbnail = path + "\\thumbnails\\" + nameWithoutExtention + ".jpg";
                if (System.IO.File.Exists(thumbnail) == true)
                {
                    _image = Image.FromFile(thumbnail);
                }
                else
                {
                   // Missing thumbnail image.  This will have already been reported in select background control.
                    _image = new Bitmap(this.pictureBox.Width, this.pictureBox.Height);
                    using (Graphics g = Graphics.FromImage(_image))
                    {
                        g.Clear(Color.LightGray);
                    }
                }
			}
			catch (System.OutOfMemoryException e)
			{
				// file does not represent a valid image
				throw e;
			}
			int max = Math.Max(this.pictureBox.Width, this.pictureBox.Height);

            int width = this.pictureBox.Width;
			int height = this.pictureBox.Height;

            Bitmap b = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(b))
            {
                RectangleF destRec = new RectangleF(0, 0, width, height);
                g.DrawImage(_image, destRec);

                if (isVideo == true)
                {
                    Image videoImage = CustomButton.SelectBackgroundControl.GetVideoBorderImage();
                    if (videoImage != null)
                    {
                        g.DrawImage(videoImage, destRec);
                    }
                }
            }
            this.pictureBox.Image = b;

            _image.Dispose();

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
				if (_image != null)
				{
					//_image.Dispose();
				}
			}
			base.Dispose( disposing );
		}


		// delete
		private void menuItem1_Click(object sender, System.EventArgs e)
		{
			mManager.DeleteHighlightedThumbnails();
		}
	}
}
