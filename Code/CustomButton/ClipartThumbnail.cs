using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;
using DVDSlideshow;

namespace CustomButton
{
	/// <summary>
	/// Summary description for Thumbnail.
	/// </summary>
	/// 
	
	// represents the thumbnail icon in the scrollable slidehow view
	public class ClipartThumbnail : System.Windows.Forms.UserControl
	{
		#region Fields
		private string _filePath = "";
	//	private System.Drawing.Image _image = null;
		#endregion

		#region Properties
		public string FilePath
		{
			get { return _filePath; }
		}

	//	public System.Drawing.Image Image
	//	{
		//	get { return _image; }
	//}
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
		private System.Windows.Forms.MenuItem addToSlide;

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
            this.addToSlide = new System.Windows.Forms.MenuItem();
            this.panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.BackColor = System.Drawing.SystemColors.Control;
            this.panel.Controls.Add(this.pictureBox);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(2, 2);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(60, 60);
            this.panel.TabIndex = 0;
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.SystemColors.Window;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(60, 60);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            // 
            // contextMenu
            // 
            this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.addToSlide});
            // 
            // addToSlide
            // 
            this.addToSlide.Index = 0;
            this.addToSlide.Text = "Add to slide";
            this.addToSlide.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // ClipartThumbnail
            // 
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.panel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ClipartThumbnail";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.Size = new System.Drawing.Size(64, 64);
            this.panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		
		public bool mHighLighted= false ;
		public Rectangle mDragBoxFromMouseDown ;
		public Cursor MyNormalCursor;
		public Cursor MyNoDropCursor;

        private ClipartSelectForm mParentForm;

		public string mFileName;
			
		//*******************************************************************
		public void OnMouseDownThumbnail(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				
			}
			else
			{
			
   
			}
		}

		//*******************************************************************
		public void OnMouseUpThumbnail(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			mDragBoxFromMouseDown = Rectangle.Empty;

			if(e.Button == MouseButtons.Right)
			{
				this.contextMenu.Show(this, new Point(e.X, e.Y) );      
			}
		}

        //*******************************************************************
		private void ListDragSource_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) 
		{
			if ((e.Button & MouseButtons.Left) == MouseButtons.Left) 
			{
				// If the mouse moves outside the rectangle, start the drag.
				if (mDragBoxFromMouseDown != Rectangle.Empty && 
					!mDragBoxFromMouseDown.Contains(e.X, e.Y)) 
				{
					DragDropEffects dropEffect = DoDragDrop(this, DragDropEffects.All | DragDropEffects.Move);
				}	
			}
		}
		private void ListDragSource_GiveFeedback(object sender, System.Windows.Forms.GiveFeedbackEventArgs e) 
		{
		}

		private void ListDragSource_QueryContinueDrag(object sender, System.Windows.Forms.QueryContinueDragEventArgs e) 
		{
		}


		//*******************************************************************
		public void OnMouseLeaveThumbnail(object sender, System.EventArgs e)
		{
            if (mHighLighted == true)
            {
                this.BackColor = FrontEndGlobals.mThumbnailSelectColour;
            }
            else
            {
                this.BackColor = FrontEndGlobals.mThumbnailBorderNormalColour;
            }
		}

        //*******************************************************************
        public void UnHighlight()
        {
            this.BackColor = FrontEndGlobals.mThumbnailBorderNormalColour;
            mHighLighted = false;
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
		public void OnDoubleClick(Object o, System.EventArgs e)
		{
            mParentForm.SelectClipArt(this.mFileName);
		}

        //*******************************************************************
        public void OnClick(Object o, System.EventArgs e)
        {
            if (mHighLighted == false)
            {

                mHighLighted = true;
                this.BackColor = FrontEndGlobals.mThumbnailSelectColour;
                mParentForm.SetHighlighted(this);
            }
            else
            {
                mHighLighted = false;
                this.BackColor = FrontEndGlobals.mThumbnailBorderNormalColour;
                mParentForm.SetHighlighted(null);
            }


        }

		//*******************************************************************
		public ClipartThumbnail(string filename, ClipartSelectForm parent)
		{
			mFileName = filename ;
            mParentForm = parent;

			InitializeComponent();
			mDragBoxFromMouseDown = Rectangle.Empty;


			pictureBox.MouseDown+=new MouseEventHandler(OnMouseDownThumbnail);
			pictureBox.MouseUp+=new MouseEventHandler(OnMouseUpThumbnail);

			pictureBox.MouseEnter+= new EventHandler(OnMouseEnterThumbnail);
			pictureBox.MouseLeave+=new EventHandler(OnMouseLeaveThumbnail);
			pictureBox.DoubleClick+=new EventHandler(OnDoubleClick);
            pictureBox.Click += new EventHandler(OnClick);

			pictureBox.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(this.ListDragSource_QueryContinueDrag);
			pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ListDragSource_MouseMove);
			pictureBox.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.ListDragSource_GiveFeedback);


			//	panel.MouseEnter+= new MouseEventHandler(OnMouseDown2);

            Image _image = null;
			//_filePath = filePath;
			try
			{
				_image = Image.FromFile(filename);
			}
			catch (System.OutOfMemoryException e)
			{
				// file does not represent a valid image
				throw e;
			}
			int max = Math.Max(this.pictureBox.Width, this.pictureBox.Height);
		//	max+=15;
			int width = _image.Width;
			int height = _image.Height;
			// determine the size for the thumbnail image
			if (_image.Width > max || _image.Height > max)
			{
				if (_image.Width > _image.Height)
				{
					width = max;
					height = (int) (_image.Height * max / _image.Width);
				}
				else
				{
					width = (int) (_image.Width * max / _image.Height);
					height = max;
				}
			}
			// set feedback information

            // draw with black background or not
            if (filename.EndsWith("snowflake.png") == false)
            {
                this.pictureBox.Image = new Bitmap(_image, width, height);
            }
            else
            {
                Bitmap b = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics gg = Graphics.FromImage(b);
                gg.Clear(Color.Black);
                gg.DrawImage(_image, 0, 0, b.Width, b.Height);
                gg.Dispose();
                this.pictureBox.Image = b;
            }
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
			}
			base.Dispose( disposing );
		}


		// delete
		private void menuItem1_Click(object sender, System.EventArgs e)
		{
            mParentForm.SelectClipArt(this.mFileName);
		}
	}
}
