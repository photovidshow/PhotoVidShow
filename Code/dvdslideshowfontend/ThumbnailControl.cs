using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;
using DVDSlideshow;
using System.Threading;
using CustomButton;
using System.Globalization;
using DVDSlideshow.GraphicsEng;

namespace dvdslideshowfontend
{
	/// <summary>
    /// Summary description for ThumbnailControl.
	/// </summary>
	/// 

    // represents the ThumbnailControl icon in the scrollable slidehow view
    public class ThumbnailControl : UserControl
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
		private System.Windows.Forms.Panel panel;
		private ToolStripMenuItem delete;
        private ToolStripSeparator menuItem4;
        private ToolStripMenuItem addTextAndImagesItem;
        private ToolStripMenuItem insertBlankSlideItem;
        private ToolStripMenuItem insertBlankSlideAfterSlideItem;
        private ToolStripMenuItem insertBlankSlideBeforeSlideItem;
        private ToolStripMenuItem insertPictureOrVideoSlideItem;
        private ToolStripMenuItem insertPictureOrVideoSlideAfterSlideItem;
        private ToolStripMenuItem insertPictureOrVideoSlideBeforeSlideItem;

        private ToolStripMenuItem recordNarrationFromThisSlideItem;
		private System.Windows.Forms.ContextMenuStrip contextMenu;
		private System.Windows.Forms.PictureBox mPictureBox;
		private System.Windows.Forms.ComboBox SlideLengthCombo;
        private ToolStripMenuItem RotateClockwiseMenuItem;
        private ToolStripMenuItem RotateAntiClockwiseMenuItem;
        private ToolStripMenuItem menuItem2;
		private System.Windows.Forms.PictureBox mSpeakerPictureBox;
		private System.Windows.Forms.PictureBox DolbyDigitalPictureBox;
        private System.Windows.Forms.PictureBox mMultipleImagesPictureBox;
        private ToolStripSeparator menuItem1;
        private ToolStripMenuItem SetAsMenuThumbnail;
        private ToolStripMenuItem SetSlideLengthToVideoLengthItem;
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
			this.DolbyDigitalPictureBox = new System.Windows.Forms.PictureBox();
			this.mSpeakerPictureBox = new System.Windows.Forms.PictureBox();
            this.mMultipleImagesPictureBox = new System.Windows.Forms.PictureBox();
			this.SlideLengthCombo = new System.Windows.Forms.ComboBox();
			this.mPictureBox = new System.Windows.Forms.PictureBox();
            this.delete = new ToolStripMenuItem();
            this.menuItem4 = new ToolStripSeparator();
            this.RotateClockwiseMenuItem = new ToolStripMenuItem();
            this.addTextAndImagesItem = new ToolStripMenuItem();
            this.insertBlankSlideItem = new ToolStripMenuItem();
            this.insertBlankSlideAfterSlideItem = new ToolStripMenuItem();
            this.insertBlankSlideBeforeSlideItem = new ToolStripMenuItem();

            this.insertPictureOrVideoSlideItem = new ToolStripMenuItem();
            this.insertPictureOrVideoSlideAfterSlideItem = new ToolStripMenuItem();
            this.insertPictureOrVideoSlideBeforeSlideItem = new ToolStripMenuItem();

            this.recordNarrationFromThisSlideItem = new ToolStripMenuItem();
			this.contextMenu = new System.Windows.Forms.ContextMenuStrip();
            this.RotateAntiClockwiseMenuItem = new ToolStripMenuItem();
            this.menuItem2 = new ToolStripMenuItem();
            this.menuItem1 = new ToolStripSeparator();
            this.SetAsMenuThumbnail = new ToolStripMenuItem();
            this.SetSlideLengthToVideoLengthItem = new ToolStripMenuItem();
			this.panel.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel
			// 
			this.panel.BackColor = System.Drawing.SystemColors.Window;
			//	this.panel.Controls.Add(this.DolbyDigitalPictureBox);
			//	this.panel.Controls.Add(this.SpeakerPictureBox);
			this.panel.Controls.Add(this.SlideLengthCombo);
			this.panel.Controls.Add(this.mPictureBox);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Location = new System.Drawing.Point(2, 2);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(124, 128);
			this.panel.TabIndex = 0;
			// 
			// DolbyDigitalPictureBox
			// 
			this.DolbyDigitalPictureBox.Location = new System.Drawing.Point(87, 110);
			this.DolbyDigitalPictureBox.Name = "DolbyDigitalPictureBox";
			this.DolbyDigitalPictureBox.Size = new System.Drawing.Size(14, 14);
			this.DolbyDigitalPictureBox.TabIndex = 6;
			this.DolbyDigitalPictureBox.TabStop = false;

            // 
            // MultipleImagesPictureBox
            // 
            this.mMultipleImagesPictureBox.Location = new System.Drawing.Point(96, 92);
            this.mMultipleImagesPictureBox.Name = "MultipleImagesPictureBox";
            this.mMultipleImagesPictureBox.Size = new System.Drawing.Size(24, 24);
            this.mMultipleImagesPictureBox.TabIndex = 4;
            this.mMultipleImagesPictureBox.TabStop = false;


			// 
			// SpeakerPictureBox
			// 
			this.mSpeakerPictureBox.BackColor = System.Drawing.Color.Transparent;
			this.mSpeakerPictureBox.Location = new System.Drawing.Point(4, 96);
			this.mSpeakerPictureBox.Name = "SpeakerPictureBox";
			this.mSpeakerPictureBox.Size = new System.Drawing.Size(16, 16);
			this.mSpeakerPictureBox.TabIndex = 3;
			this.mSpeakerPictureBox.TabStop = false;
			// 
			// SlideLengthCombo
			// 
			this.SlideLengthCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.SlideLengthCombo.DropDownWidth = 32;
			this.SlideLengthCombo.Location = new System.Drawing.Point(34, 108);
			this.SlideLengthCombo.Name = "SlideLengthCombo";
			this.SlideLengthCombo.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.SlideLengthCombo.Size = new System.Drawing.Size(54, 21);
			this.SlideLengthCombo.TabIndex = 5;
			this.SlideLengthCombo.TabStop = false;
            this.SlideLenghComboBox.Font = new Font("Segoe UI", 8.25f);
			// 
			// pictureBox
			// 
			this.mPictureBox.BackColor = System.Drawing.SystemColors.Window;
			this.mPictureBox.Location = new System.Drawing.Point(0, 0);
			this.mPictureBox.Name = "pictureBox";
			this.mPictureBox.Size = new System.Drawing.Size(124, 78);
			this.mPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.mPictureBox.TabIndex = 1;
			this.mPictureBox.TabStop = false;
		
            //
			// delete
			// 
			this.delete.Text = "Remove slide(s)";
			this.delete.Click += new System.EventHandler(this.menuItem1_Click);
            this.delete.Image = Form1.mMainForm.GetGlobalMenuItemIconsImageList().Images[0];
			// 
			// menuItem4
			// 
			this.menuItem4.Text = "-";
			// 
			// RotateClockwiseMenuItem
			// 
			this.RotateClockwiseMenuItem.Text = "Rotate clockwise";
			this.RotateClockwiseMenuItem.Click += new System.EventHandler(this.RotateClockwiseMenuItem_Click);
            this.RotateClockwiseMenuItem.Image = Form1.mMainForm.GetGlobalMenuItemIconsImageList().Images[4];
			// 
			// menuItem3
			// 
			this.addTextAndImagesItem.Text = "Add text and images";
			this.addTextAndImagesItem.Click += new System.EventHandler(this.addTextandImagesItem_Click);
            this.addTextAndImagesItem.Image = Form1.mMainForm.GetGlobalMenuItemIconsImageList().Images[2];

            //
            // insertBlankSlideItem
            // 
            this.insertBlankSlideAfterSlideItem.Text = "after this slide";
            this.insertBlankSlideAfterSlideItem.Click += new System.EventHandler(this.insertBlankSlideAfterSlideItem_Click);
 
            this.insertBlankSlideBeforeSlideItem.Text = "before this slide";
            this.insertBlankSlideBeforeSlideItem.Click += new System.EventHandler(this.insertBlankSlideBeforeSlideItem_Click);
                     
            this.insertBlankSlideItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertBlankSlideAfterSlideItem,
            this.insertBlankSlideBeforeSlideItem});

            this.insertBlankSlideItem.Text = "Insert blank slide...";
            this.insertBlankSlideItem.Image = Form1.mMainForm.GetGlobalMenuItemIconsImageList().Images[13];

            //
            // insert picture of video slide Item
            // 
            this.insertPictureOrVideoSlideAfterSlideItem.Text = "after this slide";
            this.insertPictureOrVideoSlideAfterSlideItem.Click += new System.EventHandler(this.insertPictureOrVideoSlideAfterSlideItem_Click);

            this.insertPictureOrVideoSlideBeforeSlideItem.Text = "before this slide";
            this.insertPictureOrVideoSlideBeforeSlideItem.Click += new System.EventHandler(this.insertPictureOrVideoSlideBeforeSlideItem_Click);

            this.insertPictureOrVideoSlideItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertPictureOrVideoSlideAfterSlideItem,
            this.insertPictureOrVideoSlideBeforeSlideItem});

            this.insertPictureOrVideoSlideItem.Text = "Insert pictures or video...";
            this.insertPictureOrVideoSlideItem.Image = Form1.mMainForm.GetGlobalMenuItemIconsImageList().Images[18];

            this.recordNarrationFromThisSlideItem.Text = "Record narration from this slide";
            this.recordNarrationFromThisSlideItem.Click += new System.EventHandler(this.recordNarrationFromThisSlideItem_Click);
            this.recordNarrationFromThisSlideItem.Image = Form1.mMainForm.GetGlobalMenuItemIconsImageList().Images[17];
			// contextMenu
			// 
			this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.delete} );
			this.contextMenu.Opened += new System.EventHandler(this.contextMenu_Popup);
			// 
			// RotateAntiClockwiseMenuItem
			// 
			this.RotateAntiClockwiseMenuItem.Text = "Rotate anti-clockwise";
			this.RotateAntiClockwiseMenuItem.Click += new System.EventHandler(this.RotateAntiClockwiseMenuItem_Click);
            this.RotateAntiClockwiseMenuItem.Image = Form1.mMainForm.GetGlobalMenuItemIconsImageList().Images[3];
            // 
            // SetSlideLengthToVideoLength
            // 
            this.SetSlideLengthToVideoLengthItem.Text = "Set slide length to video length";
            this.SetSlideLengthToVideoLengthItem.Click += new System.EventHandler(this.SetSlideLengthToVideoLengthItem_Click);

			// 
			// menuItem2
			// 
			this.menuItem2.Text = "-";
			// 
			// menuItem1
			// 
			this.menuItem1.Text = "-";
			// 
			// SetAsMenuThumbnail
			// 
			this.SetAsMenuThumbnail.Text = "Set as menu thumbnail";
            this.SetAsMenuThumbnail.Click += new System.EventHandler(this.SetAsMenuThumbnailClick);
            this.SetAsMenuThumbnail.Image = Form1.mMainForm.GetGlobalMenuItemIconsImageList().Images[5];
			// 
			// Thumbnail
			// 
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.Controls.Add(this.panel);
            this.AutoScaleMode = AutoScaleMode.Dpi; ;
			this.DockPadding.All = 2;
			this.Name = "Thumbnail";
			this.Size = new System.Drawing.Size(128, 115);
			this.panel.ResumeLayout(false);
			this.ResumeLayout(false);
      

		}
		#endregion


		System.Collections.Specialized.ListDictionary mMenuItemSlidehowlink;


		public ComboBox SlideLenghComboBox
		{
			get
			{
				return SlideLengthCombo;
			}
		}

        public Thumbnail ParentThumbnail
        {
            set { mThumbnail = value; }
            get { return mThumbnail; }
        }

        private Thumbnail mThumbnail;
		public Rectangle mDragBoxFromMouseDown ;
		public Cursor MyNormalCursor;
		public Cursor MyNoDropCursor;
		bool   mDeSelectOnMouseUp =false;

		public Color mNonHighlightBorderColor;
		public Color mPanelColor;

		static private Image mSoundImage = null;
		static private Image mAC3 = null;
        static private Image mMultipleImagesImage = null;


        //*******************************************************************
        public bool SlideLengthComboEnabled
        {
            set
            {
                SlideLengthCombo.Enabled = value;
            }
        }

        //*******************************************************************
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}


		//*******************************************************************
		public void Blank()
		{
			mDragBoxFromMouseDown = Rectangle.Empty;
		
            if (mThumbnail != null)
            {
                mThumbnail.ForThumbnailControl = null;
            }

            mThumbnail = null;
		}

        //*******************************************************************
        public void SetImage(Image i)
        {
            mPictureBox.Image = i;

            if (i != null)
            {
                int newpbh = i.Height;

                if (newpbh > 93)
                {
                    newpbh = 93;
                }

                mPictureBox.Height = newpbh;
            }
        }

        //*******************************************************************
        // only call locally and from cache
        public void SetHighlightColours(bool val)
        {
            if (val == false && this.BackColor == mNonHighlightBorderColor)
            {
                return;
            }

            if (val == true && this.BackColor == FrontEndGlobals.mThumbnailSelectColour)
            {
                return;
            }

            if (val == true)
            {
                int r = mPanelColor.R - 50;
                int g = mPanelColor.G - 50;
                int b = mPanelColor.B - 50;

                if (r < 0) r = 0;
                if (g < 0) g = 0;
                if (b < 0) b = 0;

                Color back_color = Color.FromArgb(r, g, b);

                mPictureBox.BackColor = back_color;
                panel.BackColor = back_color;
                BackColor = FrontEndGlobals.mThumbnailSelectColour;
                SlideLenghComboBox.BackColor = back_color;
            }
            else
            {
                mPictureBox.BackColor = mPanelColor;
                panel.BackColor = mPanelColor;
                SlideLenghComboBox.BackColor = mPanelColor;
                this.BackColor = mNonHighlightBorderColor;
            }
        }

		//*******************************************************************
		public void SetHighlighted(bool val, bool informParentThumbnail)
		{
            if (mThumbnail == null) return;

            if (mThumbnail.Highlighted == val) return;
   
            if (val == true)
            {
                // ensure we are only one
                if (mThumbnail.Manager != null &&
                    Form1.mMainForm.mControlKeyOn == false &&
                    Form1.mMainForm.mShiftKeyOn == false)
                {
                    mThumbnail.Manager.UnHighlightAllThumbnails(false);
                }
            }

            SetHighlightColours(val);

            if (informParentThumbnail == true)
            {
                mThumbnail.SetHighlighted(val, false);
            }

            if (mThumbnail.Manager != null)
			{
                mThumbnail.Manager.HighlightChanged();
			}
		}
			
		//*******************************************************************
		public void OnMouseDownThumbnail(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            if (mThumbnail == null) return;

			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
                mThumbnail.Manager.Stop_Click(this, new EventArgs());
				SetHighlighted(true, true) ;
			} 
			else
			{
                if (mThumbnail.Manager.IsPlaying())
                {
                    mThumbnail.Manager.MoveTrackerToSlide(mThumbnail.Slide);
                    return;
                }

				// if holding down shift multi highlight
				if (Form1.mMainForm.mShiftKeyOn==true)
				{
                    mThumbnail.Manager.MultiHighlightSlidesToSlide(mThumbnail.Slide);
				}
				else
				{
                    if (mThumbnail.Highlighted == false)
					{
					
						SetHighlighted(true, true) ;
                        this.Refresh(); // Force colour chnages now decoration tab gets updated

                        if (mThumbnail.Manager.IsPlaying() == false)
                        {
                            mThumbnail.Manager.DecorateButton_Click(sender, e);
                        }
						mDeSelectOnMouseUp = false ;
					}
					else
					{
						mDeSelectOnMouseUp = true;
					}
				}
          
				// Remember the point where the mouse down occurred. The DragSize indicates
				// the size that the mouse can move before a drag event should be started.                
				Size dragSize = SystemInformation.DragSize;

				// Create a rectangle using the DragSize, with the mouse position being
				// at the center of the rectangle.
				mDragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width /2),
					e.Y - (dragSize.Height /2)), dragSize);

			}
		}

        //*******************************************************************
        public void OnMouseUpThumbnail(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (mThumbnail == null) return;

            if (e.Button == MouseButtons.Right)
            {
                this.contextMenu.Show(this, new Point(e.X, e.Y));
                return;
            }

            if (mThumbnail.Manager.IsPlaying() == true) return;
            mDragBoxFromMouseDown = Rectangle.Empty;

            if (e.Button == MouseButtons.Left && mDeSelectOnMouseUp)
            {
                if (Form1.mMainForm.mShiftKeyOn == true)
                {
                    return;
                }

                SetHighlighted(false, true);
                if (Form1.mMainForm.mControlKeyOn == false)
                {
                    mThumbnail.Manager.UnHighlightAllThumbnails(true);
                }

                this.OnMouseEnterThumbnail(this, e);
            }
        
        }

		//*******************************************************************
		public void OnDoubleClickThumbnail(object sender, System.EventArgs e)
		{
            // ### SRG TODO DOES NOT WORK
            /*
            if (mManager.IsPlaying()==true)
            {
                this.SetHighlighted(true);
                mManager.DecorateButton_Click(sender, e);
            }
             */

		}

		//*******************************************************************
		private void ListDragSource_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) 
		{
            if (mThumbnail == null || mThumbnail.Manager.IsPlaying() == true) return;

			if ((e.Button & MouseButtons.Left) == MouseButtons.Left) 
			{
				// If the mouse moves outside the rectangle, start the drag.
				if (mDragBoxFromMouseDown != Rectangle.Empty && 
					!mDragBoxFromMouseDown.Contains(e.X, e.Y)) 
				{

					mDeSelectOnMouseUp = false;
                    mThumbnail.Manager.StartDragSlide();
				}	
			}
			else
			{
				//	Console.WriteLine("mouse pos ="+e.X+" "+e.Y);
			}
		}

		//*******************************************************************
		public void OnMouseEnterThumbnail(object sender, System.EventArgs e)
		{
            if (mThumbnail == null) return;

            if (mThumbnail.Highlighted == false)
			{
                int r= mNonHighlightBorderColor.R-40;
                int g= mNonHighlightBorderColor.G-40;
                int b= mNonHighlightBorderColor.B-40;

                if (r<0) r=0;
                if (g<0) g=0;
                if (b<0) b=0;

				Color back_color = Color.FromArgb(r,g,b);
	
				this.BackColor = back_color;
			}
			else
			{
                Color back_color = FrontEndGlobals.mThumbnailSelectColour;
				this.BackColor = back_color;
			}
		}

		//*******************************************************************
		public void OnMouseLeaveThumbnail(object sender, System.EventArgs e)
		{
            if (mThumbnail == null) return;

            if (mThumbnail.Highlighted == false)
			{
				this.BackColor = mNonHighlightBorderColor;
			}
			else
			{
                this.BackColor = FrontEndGlobals.mThumbnailSelectColour;
			}
		}

		//*******************************************************************
        public ThumbnailControl()
		{
			InitializeComponent();
			
			mDragBoxFromMouseDown = Rectangle.Empty;

			mNonHighlightBorderColor = this.BackColor;
			mPanelColor = panel.BackColor;

			panel.MouseDown+=new MouseEventHandler(OnMouseDownThumbnail);
			panel.DoubleClick+=new EventHandler(OnDoubleClickThumbnail);
			panel.MouseUp+=new MouseEventHandler(OnMouseUpThumbnail);
			panel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ListDragSource_MouseMove);

			mPictureBox.MouseDown+=new MouseEventHandler(OnMouseDownThumbnail);
			mPictureBox.DoubleClick+=new EventHandler(OnDoubleClickThumbnail);
			mPictureBox.MouseUp+=new MouseEventHandler(OnMouseUpThumbnail);
			mPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ListDragSource_MouseMove);
	
			panel.MouseEnter+= new EventHandler(OnMouseEnterThumbnail);
			panel.MouseLeave+= new EventHandler(OnMouseLeaveThumbnail);

			mPictureBox.MouseEnter+= new EventHandler(OnMouseEnterThumbnail);
			mPictureBox.MouseLeave+= new EventHandler(OnMouseLeaveThumbnail);

			SlideLenghComboBox.MouseEnter+= new EventHandler(OnMouseEnterThumbnail);
			SlideLenghComboBox.MouseLeave+= new EventHandler(OnMouseLeaveThumbnail);

			mSpeakerPictureBox.MouseEnter+= new EventHandler(OnMouseEnterThumbnail);
			mSpeakerPictureBox.MouseLeave+= new EventHandler(OnMouseLeaveThumbnail);

			DolbyDigitalPictureBox.MouseEnter+=new EventHandler(OnMouseEnterThumbnail);
			DolbyDigitalPictureBox.MouseLeave+= new EventHandler(OnMouseLeaveThumbnail);
		
			if (mSoundImage==null)
			{
                try
                {
                    mSoundImage = Bitmap.FromFile(CGlobals.GetGuiImagesDirectory() + @"\sound.png", false);
                }
                catch
                {
                }
			}
            if (mAC3 == null)
            {
                try
                {
                    mAC3 = Bitmap.FromFile(CGlobals.GetGuiImagesDirectory() + @"\DolbyDigital51.png", false);
                }
                catch
                {
                }
            }

            if (mMultipleImagesImage == null)
            {
                try
                {
                    mMultipleImagesImage = Bitmap.FromFile(CGlobals.GetGuiImagesDirectory() + @"\multiple.png", false);
                }
                catch
                {
                }
            }

            SlideLengthCombo.Items.Add("0.1");
            SlideLengthCombo.Items.Add("0.25");
            SlideLengthCombo.Items.Add("0.5");
            SlideLengthCombo.Items.Add("0.75");

            for (int i=1;i<=60;i++)
			{
				SlideLengthCombo.Items.Add(i);
			}

            this.SuspendLayout();

            mSpeakerPictureBox.Image = mSoundImage;
            mSpeakerPictureBox.Visible = false;
            this.panel.Controls.Add(mSpeakerPictureBox);

            mMultipleImagesPictureBox.Image = mMultipleImagesImage;
            mMultipleImagesPictureBox.Visible = false;
            this.panel.Controls.Add(mMultipleImagesPictureBox);

            this.ResumeLayout();

			//	SlideLengthCombo.DisplayMemberChanged+= new EventHandler(this.combochanged);
			SlideLengthCombo.LostFocus+= new EventHandler(this.combochanged);
			SlideLengthCombo.SelectedValueChanged+= new EventHandler(this.combochanged);

		}


		//*******************************************************************
        public delegate void ReCalcComboTextDelegate();
		public void ReCalcComboText()
		{
            if (mThumbnail == null) return;

            if (this.InvokeRequired == true)
            {
                ReCalcComboTextDelegate my_delegate = new ReCalcComboTextDelegate(ReCalcComboText);

                this.BeginInvoke(my_delegate);
                return;
            }

            SetSlideLengthComboString(mThumbnail.Slide.DisplayLength);
		}

        //*******************************************************************
        private void SetSlideLengthComboString(float seconds)
        {
            try
            {
                if (SlideLengthCombo.Text != "")
                {
                    float current = float.Parse(SlideLengthCombo.Text);
                    if (Math.Abs(seconds - current) <= 0.05f) return;
                }
            }
            catch
            {
            }

            string text = seconds.ToString("0.##");      // rounds to 2 dp
            SlideLengthCombo.Text = text;
        }


		//*******************************************************************
		public void combochanged(object o, System.EventArgs e)
		{
            if (mThumbnail == null) return;

            float current_value = mThumbnail.Slide.DisplayLength;

			float new_value =2.0f;

			string i = this.SlideLengthCombo.Text;

			bool valid = true;
			try
			{
				new_value = float.Parse(i,CultureInfo.InvariantCulture);
				if (new_value<=0) valid = false;
			}
			catch(Exception)
			{
				valid = false ;
			}

			if (new_value >500)
			{
				new_value = 500;
				this.SlideLengthCombo.Text="500";

			}

			if (valid == false)
			{
                this.SetSlideLengthComboString(current_value);
			}

			if (new_value != current_value)
			{
                mThumbnail.Manager.mCurrentSlideShow.InValidateLength();
                mThumbnail.Slide.DisplayLength = new_value;
				ComboChangedSoInformStoryboardOfChange();
			}
		}

        //*******************************************************************
		public void ComboChangedSoInformStoryboardOfChange()
		{
            if (mThumbnail == null) return;

            mThumbnail.ComboChangedSoInformStoryboardOfChange();
		}

   
        //************************************
		private void GeneratePictureBox()
		{
            if (mThumbnail != null)
            {
                mThumbnail.GeneratePictureBox();
            }
        }

		//************************************
		public void ReDrawSideIcons()
		{
            if (mThumbnail == null) return;

            bool hasAudio = false;
            bool hasAc3 = false;

            // multiple images icon
            int count = 0;
            if (mThumbnail.Slide != null)
            {
                foreach (CDecoration d in mThumbnail.Slide.GetAllAndSubDecorations())
                {
                    if (d is CGroupedDecoration ||
                         (d is CImageDecoration == false) ||
                         d.IsFilter() ||
                         d.IsTemplateFrameworkDecoration() ||
                         d.IsBackgroundDecoration() ||
                         d.IsBorderDecoration())
                    {
                        continue;
                    }
                    count++;

                    if (count >= 2)
                    {
                        break;
                    }
                }
            }

            if (count >= 2)
            {
                this.mMultipleImagesPictureBox.Visible = true;
            }
            else
            {
                this.mMultipleImagesPictureBox.Visible = false;
            }

            // audio icons
            bool hasVideo = mThumbnail.ContainsUserVideos(mThumbnail.Slide, ref hasAudio, ref hasAc3);

            if (hasVideo == true && hasAudio == true)
            {
                this.mSpeakerPictureBox.Visible = true;
            }
            else
            {
                this.mSpeakerPictureBox.Visible = false;
            }
		}


        //*******************************************************************
		// delete
		private void menuItem1_Click(object sender, System.EventArgs e)
		{
            if (mThumbnail == null) return;

            mThumbnail.Manager.DeleteHighlightedThumbnails();
		}

        //*******************************************************************
		// decorate
		private void addTextandImagesItem_Click(object sender, System.EventArgs e)
		{
            if (mThumbnail == null) return;

            Form1.mMainForm.SuspendLayout();

            TabPage tb = Form1.mMainForm.GetAddTextAndImagesTabPage();
            Form1.mMainForm.GetDecorationsTabControl().SelectedTab = tb;

            mThumbnail.Manager.DecorateButton_Click(sender, e);

            Form1.mMainForm.ResumeLayout();
		}

        //*******************************************************************
        private void insertBlankSlideAfterSlideItem_Click(object sender, System.EventArgs e)
        {
            if (mThumbnail == null) return;

            mThumbnail.Manager.UnHighlightAllThumbnails(false);
            SetHighlighted(true, true);
            mThumbnail.mManager.AddBlankSlideButtonClicked(true);
        }

        //*******************************************************************
        private void insertBlankSlideBeforeSlideItem_Click(object sender, System.EventArgs e)
        {
            if (mThumbnail == null) return;

            mThumbnail.Manager.UnHighlightAllThumbnails(false);
            SetHighlighted(true, true);
            mThumbnail.mManager.AddBlankSlideButtonClicked(false);
        }

        //*******************************************************************
        private void insertPictureOrVideoSlideAfterSlideItem_Click(object sender, System.EventArgs e)
        {
            if (mThumbnail == null) return;

            mThumbnail.Manager.UnHighlightAllThumbnails(false);
            SetHighlighted(true, true);
            mThumbnail.mManager.AddSlidesButton_Click(true);
        }

        //*******************************************************************
        private void insertPictureOrVideoSlideBeforeSlideItem_Click(object sender, System.EventArgs e)
        {
            if (mThumbnail == null) return;

            mThumbnail.Manager.UnHighlightAllThumbnails(false);
            SetHighlighted(true, true);
            mThumbnail.mManager.AddSlidesButton_Click(false);
        }


        //*******************************************************************
        private void recordNarrationFromThisSlideItem_Click(object sender, System.EventArgs e)
        {
            Form1.mMainForm.GetSlideShowNarrationManager().RecordNarration(mThumbnail.Slide);
        }

        //*******************************************************************
		private void RotateClockwiseMenuItem_Click(object sender, System.EventArgs e)
		{
            if (mThumbnail == null) return;

            mThumbnail.Manager.RotateClockwise_Click(sender, e);
		}

		//*******************************************************************
		private void RotateAntiClockwiseMenuItem_Click(object sender, System.EventArgs e)
		{
            if (mThumbnail == null) return;

            mThumbnail.Manager.RotateAntiClockwise_Click(sender, e);
		}

        //*******************************************************************
        private void SetSlideLengthToVideoLengthItem_Click(object sender, System.EventArgs e)
        {
            if (mThumbnail == null && mThumbnail.Slide == null) return;

            CVideoDecoration dec = mThumbnail.Slide.SupportsSimpleOrientationChange() as CVideoDecoration;

            if (dec != null)
            {
                mThumbnail.Slide.SetLengthWithoutUpdate((float)dec.GetTrimmedVideoDurationInSeconds());
                dec.StartOffsetTimeRawValue = CAnimatedDecoration.SlideStart;
                dec.EndOffsetTimeRawValue = CAnimatedDecoration.SlideEnd;

                //
                //  Need to update our thumbnail, and then inform the storyboard of the change
                //
                mThumbnail.Manager.mCurrentSlideShow.InValidateLength();
                mThumbnail.InvalidateImage(true);
                CGlobals.mCurrentProject.DeclareChange(true, "Set slide length to video length");
            }
        }

		//*******************************************************************
		private void MoveToSlideshow_Click(object o, System.EventArgs e)
		{
            if (mThumbnail == null) return;

			MenuItem mi =o as MenuItem;
			CSlideShow ss =mMenuItemSlidehowlink[o] as CSlideShow;;
			if (ss==null)
			{
                ManagedCore.CDebugLog.GetInstance().Error("Could not find slideshow '" + mi.Text + "' on call to MoveToSlideshow_Click");
				return;
			}

            mThumbnail.Manager.MoveHighlightedSlidesToSlideshow(ss);
		}


		//*******************************************************************
		private void CopyToSlideshow_Click(object o, System.EventArgs e)
		{
            if (mThumbnail == null) return;

			MenuItem mi =o as MenuItem;
			CSlideShow ss =  mMenuItemSlidehowlink[o] as CSlideShow;
			if (ss==null)
			{
                ManagedCore.CDebugLog.GetInstance().Error("Could not find slideshow '" + mi.Text + "' on call to CopyToSlideshow_Click");
				return;
			}

            mThumbnail.Manager.CopyHighlightedSlidesToSlideshow(ss);
		}

        //*******************************************************************
        private void DuplicateSlide_Click(object sender, System.EventArgs e)
        {
            if (mThumbnail == null) return;

            mThumbnail.Manager.DuplicateHighlightedSlides();
        }


        //*******************************************************************
        private void SplitIntoEditableSlides_Click(object sender, System.EventArgs e)
        {
            if (mThumbnail == null) return;

            mThumbnail.SplitIntoEditableSlides();
        }

		//*******************************************************************
		private void contextMenu_Popup(object sender, System.EventArgs e)
		{
            if (mThumbnail == null) return;

			mMenuItemSlidehowlink = new
				System.Collections.Specialized.ListDictionary();

			ContextMenuStrip menu = this.contextMenu;

			CProject project = CGlobals.mCurrentProject;

			ArrayList slideshows = project.GetAllProjectSlideshows(false);


            menu.Items.Clear();
            menu.Items.Add(delete);

            CDecoration slideMainDecoration = mThumbnail.Slide.SupportsSimpleOrientationChange();
            if (slideMainDecoration != null)
            {
                menu.Items.Add(menuItem4);
                menu.Items.Add(RotateClockwiseMenuItem);
                menu.Items.Add(RotateAntiClockwiseMenuItem);

                if (mThumbnail.ShowSetSlideLengthToVideoLengthOption(slideMainDecoration as CVideoDecoration) == true)
                {
                    menu.Items.Add(SetSlideLengthToVideoLengthItem);
                }    
            }

            if (mThumbnail.Slide.AllowedToBeEditited() == true)
            {
                menu.Items.Add(addTextAndImagesItem); 
            }

            menu.Items.Add(insertBlankSlideItem);
            menu.Items.Add(insertPictureOrVideoSlideItem);
            menu.Items.Add(recordNarrationFromThisSlideItem);
            menu.Items.Add(menuItem1);

            if (project.DiskPreferences.OutputVideoType != CGlobals.VideoType.MP4)
            {
                menu.Items.Add(SetAsMenuThumbnail);
            }


            ToolStripMenuItem editSlideImages = new ToolStripMenuItem();
            editSlideImages.Image = Form1.mMainForm.GetGlobalMenuItemIconsImageList().Images[1];
            editSlideImages.Text = @"Edit slide media";
            editSlideImages.Click += new EventHandler(editSlideImages_Click);
            menu.Items.Add(editSlideImages);

            // add duplicate, move and copy
            ToolStripItem lastCurrentItem = menu.Items[menu.Items.Count - 1];
            if (lastCurrentItem is ToolStripSeparator == false)
            {
                ToolStripItem split1 = new ToolStripSeparator();
                menu.Items.Add(split1);
            }

            ToolStripMenuItem duplicate_item = new ToolStripMenuItem();
            duplicate_item.Text = @"Duplicate slide(s)";
            duplicate_item.Click += new EventHandler(DuplicateSlide_Click);
            menu.Items.Add(duplicate_item);

            CSlideShow currentSlideshow = mThumbnail.Manager.mCurrentSlideShow;
            if (currentSlideshow.ChapterMarkersTypeToUse == CSlideShow.ChapterMarkersType.SetFromSlides &&
                (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.DVD ||
                 CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.BLURAY) )
            {
                ToolStripMenuItem setAsChapterMarkerItem = new ToolStripMenuItem();

                if (mThumbnail.Slide.MarkedAsChapter == true)
                {
                    setAsChapterMarkerItem.Text = @"Unmark as chapter";
                    setAsChapterMarkerItem.Checked = true;
                }
                else
                {
                    setAsChapterMarkerItem.Text = @"Mark as chapter";
                    setAsChapterMarkerItem.Checked = false;
                }

                setAsChapterMarkerItem.Click += new EventHandler(setAsChapterMarkerItem_Click);
               
                menu.Items.Add(setAsChapterMarkerItem);
            }
                 
            if (mThumbnail.Slide is CMultiSlideSlide)
            {
                CMultiSlideSlide mss = mThumbnail.Slide as CMultiSlideSlide;
                
                // G562 for now disable this option if the slide contains video links (as possible to mess it up)
                if (mss.ContainsVideoReferenceLinks() == false)
                {
                    ToolStripItem split = new ToolStripSeparator();
                    menu.Items.Add(split);

                    ToolStripMenuItem split_item = new ToolStripMenuItem();
                    split_item.Text = @"Split into editable slides";
                    split_item.Click += new EventHandler(SplitIntoEditableSlides_Click);
                    menu.Items.Add(split_item);
                }
            }

            // If only one slideshow or output is video, then we can't move/copy between slideshows
            if (slideshows.Count < 2 || CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.MP4) return;

			bool added_move_to=false;
			// add move to slideshow menu items
			foreach (CSlideShow ss in slideshows)
			{
				if (ss.ID != Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow.ID)
				{
                    ToolStripMenuItem item = new ToolStripMenuItem();
					item.Text = @"Move to '"+ss.GetSlideshowLabelName()+@"'";
				//	item.Index = index++;
					item.Click+=new EventHandler(MoveToSlideshow_Click);
                    menu.Items.Add(item);
					mMenuItemSlidehowlink.Add(item,ss);
					added_move_to=true;
				}
			}

			if (added_move_to==true)
			{
                ToolStripSeparator split2 = new ToolStripSeparator();
                menu.Items.Add(split2);
			}

			// add copy to slideshow menu items
			foreach (CSlideShow ss in slideshows)
			{
				if (ss.ID != Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow.ID)
				{
                    ToolStripMenuItem item = new ToolStripMenuItem();
					item.Text = @"Copy to '"+ss.GetSlideshowLabelName()+@"'";
					//item.Index = index++;
					item.Click+=new EventHandler(CopyToSlideshow_Click);
                    menu.Items.Add(item);
					mMenuItemSlidehowlink.Add(item,ss);
				}
			}
		}

        //*******************************************************************
        private void setAsChapterMarkerItem_Click(object sender, EventArgs e)
        {
            if (mThumbnail.Slide.MarkedAsChapter == true)
            {
                mThumbnail.Slide.MarkedAsChapter = false;
                CGlobals.mCurrentProject.DeclareChange(true, "Slide unmarked as chapter");
            }
            else
            {
                mThumbnail.Slide.MarkedAsChapter = true;
                CGlobals.mCurrentProject.DeclareChange(true, "Slide marked as chapter");
            }
            GeneratePictureBox();
        }

        //*******************************************************************
		private void SetAsMenuThumbnailClick(object sender, System.EventArgs e)
		{
            if (mThumbnail == null) return;

            Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();
            Form1.mMainForm.GoToMainMenu();

            mThumbnail.Manager.SetSlideAsMenuThumbnail(mThumbnail.Slide);
		}

        //*******************************************************************
        private void editSlideImages_Click(object sender, System.EventArgs e)
		{
            if (mThumbnail == null) return;

            Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();
            Form1.mMainForm.GoToMainMenu();

            EditSlideMediaForm esmf = new EditSlideMediaForm(mThumbnail.Slide, mThumbnail.Manager.mCurrentSlideShow, null);
            esmf.ShowDialog();

            if (mThumbnail != null)
            {
                mThumbnail.InvalidateImage(esmf.SlideLengthChanged);
            }

            CGlobals.mCurrentProject.DeclareChange(true, "Edit slide media");
            Form1.mMainForm.GetDecorationManager().RePaint();

		}
	}
}
