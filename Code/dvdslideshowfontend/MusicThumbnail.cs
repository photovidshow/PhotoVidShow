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
	public class MusicThumbnail : System.Windows.Forms.UserControl
	{

        public enum AudioType
        {
            NARRATION,
            MUSIC
        }

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
		#endregion
        private IContainer components;
        private ContextMenuStrip mContextMenuStrip;
        private ToolStripMenuItem mEditTrimMusicToolStripMenuItem;
        private ToolStripMenuItem mEndMusicOnSelectedSlideToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem mRemoveMusicToolStripMenuItem;
		private System.Windows.Forms.Label mName;


		#region Generated Methods

		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MusicThumbnail));
            this.panel = new System.Windows.Forms.Panel();
            this.mName = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.mContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mEditTrimMusicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mEndMusicOnSelectedSlideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mRemoveMusicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.mContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.BackColor = System.Drawing.SystemColors.Control;
            this.panel.Controls.Add(this.mName);
            this.panel.Controls.Add(this.pictureBox);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel.Location = new System.Drawing.Point(2, 2);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(60, 360);
            this.panel.TabIndex = 0;
            // 
            // mName
            // 
            this.mName.BackColor = System.Drawing.Color.Lavender;
            this.mName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mName.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.mName.Location = new System.Drawing.Point(mMusicLabelTextLeftMargin, 2);
            this.mName.Name = "mName";
            this.mName.Size = new System.Drawing.Size(96, 18);
            this.mName.TabIndex = 2;
            this.mName.Text = "label1";
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.Color.Lavender;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(60, 360);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            // 
            // mContextMenuStrip
            // 
            this.mContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mEditTrimMusicToolStripMenuItem,
            this.mEndMusicOnSelectedSlideToolStripMenuItem,
            this.toolStripSeparator1,
            this.mRemoveMusicToolStripMenuItem});
            this.mContextMenuStrip.Name = "mContextMenuStrip";
            this.mContextMenuStrip.Size = new System.Drawing.Size(220, 76);
            // 
            // mEditTrimMusicToolStripMenuItem
            // 
            this.mEditTrimMusicToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mEditTrimMusicToolStripMenuItem.Image")));
            this.mEditTrimMusicToolStripMenuItem.Name = "mEditTrimMusicToolStripMenuItem";
            this.mEditTrimMusicToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.mEditTrimMusicToolStripMenuItem.Text = "Edit/trim music";
            this.mEditTrimMusicToolStripMenuItem.Click += new System.EventHandler(this.EditAudio_Click);
            // 
            // mEndMusicOnSelectedSlideToolStripMenuItem
            // 
            this.mEndMusicOnSelectedSlideToolStripMenuItem.Name = "mEndMusicOnSelectedSlideToolStripMenuItem";
            this.mEndMusicOnSelectedSlideToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.mEndMusicOnSelectedSlideToolStripMenuItem.Text = "End music on selected slide";
            this.mEndMusicOnSelectedSlideToolStripMenuItem.Click += new System.EventHandler(this.mTrimMusicOnSelectedSlideItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(216, 6);
            // 
            // mRemoveMusicToolStripMenuItem
            // 
            this.mRemoveMusicToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mRemoveMusicToolStripMenuItem.Image")));
            this.mRemoveMusicToolStripMenuItem.Name = "mRemoveMusicToolStripMenuItem";
            this.mRemoveMusicToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.mRemoveMusicToolStripMenuItem.Text = "Remove music";
            this.mRemoveMusicToolStripMenuItem.Click += new System.EventHandler(this.DeleteAudio_Click);
            // 
            // MusicThumbnail
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Gray;
            this.Controls.Add(this.panel);
            this.Name = "MusicThumbnail";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.Size = new System.Drawing.Size(64, 364);
            this.panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.mContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion
		
		public CMusicSlide mMusicSlide;

		public bool mHighLighted= false ;
        public CSlideShowAudioManager mManager;

		public Rectangle mDragBoxFromMouseDown ;
		public Cursor MyNormalCursor;
		public Cursor MyNoDropCursor;

		private Color mOriginalBackColor ;
		private Color mOriginalForeColor;
		private Color mOriginalLabelColor;

		
		public string mFileName;

        private ToolTip mToolTip = new ToolTip();
        private int mToolTipX = -1;
        private AudioType mAudioType;
        private const int mMusicLabelTextLeftMargin = 2;

        public int MusicLabelTextLeftMargin
        {
            get { return mMusicLabelTextLeftMargin; }
        }
        public Label MusicLabelText
        {
            get { return mName; }
        }
        public AudioType AudioThumbnailType
        {
            get { return mAudioType; }
        }

		//*******************************************************************
        public MusicThumbnail(CMusicSlide ms, CSlideShowAudioManager manager, AudioType type)
		{
			mMusicSlide = ms ;
            mAudioType = type;

			mManager = manager;

			InitializeComponent();

            if (mAudioType == AudioType.MUSIC)
            {
                if (CGlobals.IsWinVistaOrHigher() == true)
                {
                    string note = "\u266B";
                    this.mName.Text =  note + " ";
                }
                else
                {
                    this.mName.Text = "";
                }
            }
            else
            {
                this.mName.Text = "";
                this.mContextMenuStrip.Items.Remove(mEndMusicOnSelectedSlideToolStripMenuItem);
                this.mEditTrimMusicToolStripMenuItem.Text = "Edit/trim narration";
                this.mRemoveMusicToolStripMenuItem.Text = "Remove narration";
         
            }
           
			string a = Path.GetFileName(ms.mName);
			this.mName.Width= a.Length*20;
			this.mName.Text+= a;

			DateTime tt = new DateTime((long)(mMusicSlide.GetDurationInSeconds()*10000000.0f));

			string new_label_tt = System.String.Format("{0}:{1:d2}", tt.Minute,tt.Second);

			this.mName.Text+= "   ("+new_label_tt+")";

			mOriginalBackColor = this.BackColor;
			mOriginalForeColor = this.ForeColor;
			mOriginalLabelColor = this.mName.ForeColor;

			if (ms.IsLoopMusicSlide==false)
			{
                if (mAudioType == AudioType.NARRATION)
                {
                    MoveControlHelper.Init(mName, this, MoveControlHelper.Direction.Horizontal, MovingThumnailStartedCallback, MovingThumnailMovingCallback, MovingThumnailFinishedMoveCallback);
                    MoveControlHelper.Init(pictureBox, this, MoveControlHelper.Direction.Horizontal, MovingThumnailStartedCallback, MovingThumnailMovingCallback, MovingThumnailFinishedMoveCallback);

                    mName.MouseHover += new EventHandler(OnMouseHover);
                    pictureBox.MouseHover += new EventHandler(OnMouseHover);
                }
                else
                {
                    mName.DoubleClick += new EventHandler(EditAudio_Click);
                    this.pictureBox.DoubleClick += new EventHandler(EditAudio_Click);
                }

                
                mName.MouseUp += new MouseEventHandler(OnMouseUpThumbnail);
                pictureBox.MouseUp += new MouseEventHandler(OnMouseUpThumbnail);

				this.pictureBox.MouseEnter+= new EventHandler(this.OnMouseEnter);
				this.pictureBox.MouseLeave+= new EventHandler(this.OnMouseLeave);
				mName.MouseEnter+=new EventHandler(OnMouseEnter);
				mName.MouseLeave+=new EventHandler(OnMouseLeave);
				
			}
			else
			{
				Panel back_planel = Form1.mMainForm.GetSlideShowMusicPlanel();

				Color bc = fade_to_back(mOriginalBackColor, back_planel.BackColor,2);
				Color fc = fade_to_back(mOriginalForeColor, back_planel.BackColor,2);
				Color lc = fade_to_back(mOriginalLabelColor, back_planel.BackColor,2);

				this.BackColor=bc;
				this.ForeColor=fc;
				this.mName.ForeColor = lc;

			}
		}


        //*******************************************************************
        public void OnMouseDownThumbnail(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {

            }
            else
            {
                // Remember the point where the mouse down occurred. The DragSize indicates
                // the size that the mouse can move before a drag event should be started.                
                Size dragSize = SystemInformation.DragSize;

                // Create a rectangle using the DragSize, with the mouse position being
                // at the center of the rectangle.
                mDragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2),
                    e.Y - (dragSize.Height / 2)), dragSize);
            }
        }

        //*******************************************************************
        public void OnMouseUpThumbnail(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mDragBoxFromMouseDown = Rectangle.Empty;

            bool can_end_music_on_last_selected_thumbnail = false;

            ArrayList selected_thumnails = Form1.mMainForm.GetSlideShowManager().GetHighlightedThumbnailSlides();

            if (selected_thumnails.Count > 0)
            {
                CImageSlide image_slide = selected_thumnails[selected_thumnails.Count - 1] as CImageSlide;

                if (image_slide != null)
                {
                    int slide_end_frame = image_slide.GetGlobalEndFrameBeforeTransition();

                    double total_possible_duration_of_ms = mMusicSlide.Player.GetDurationInSeconds();
                    total_possible_duration_of_ms -= mMusicSlide.StartMusicOffset;

                    double fps = CGlobals.mCurrentProject.DiskPreferences.AudioFramesPerSecond;
                    int total_possible_length_in_frames = (int)(fps * total_possible_duration_of_ms);

                    if (this.mMusicSlide.StartFrameOffset < slide_end_frame &&
                        this.mMusicSlide.StartFrameOffset + total_possible_length_in_frames > slide_end_frame)
                    {
                        can_end_music_on_last_selected_thumbnail = true;
                    }
                }
            }

            if (can_end_music_on_last_selected_thumbnail == true &&
                Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow.SyncLengthToMusic == false)
            {
                this.mEndMusicOnSelectedSlideToolStripMenuItem.Enabled = true;
            }
            else
            {
                this.mEndMusicOnSelectedSlideToolStripMenuItem.Enabled = false;
            }

            if (e.Button == MouseButtons.Right && this.Visible==true)
            {
                this.mContextMenuStrip.Show(this, new Point(e.X, e.Y));
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

        //*******************************************************************
        public void MovingThumnailStartedCallback()
        {
            mManager.ThumbnailStartedMoving(this);
        }


        //*******************************************************************
        public void MovingThumnailMovingCallback(int mousePos)
        {
            mManager.ThumbnailMoving(this, mousePos);

            if (mousePos < 0)
            {
                this.Left = 0;
            }
        }

        //*******************************************************************
        public void MovingThumnailFinishedMoveCallback()
        {
            mManager.ThumbnailMoved(this);
        }

		//*******************************************************************
		public void OnMouseEnter(object o, System.EventArgs e)
		{
            int r= mOriginalBackColor.R-40;
            int g= mOriginalBackColor.G-40;
            int b= mOriginalBackColor.B-40;
            if (r<0) r=0;
            if (g<0) g=0;
            if (b<0) b=0;

			Color back_color = Color.FromArgb(r,g,b);
			this.BackColor = back_color;
		}

		//*******************************************************************		
		public void OnMouseLeave(object o, System.EventArgs e)
		{
			this.BackColor = mOriginalBackColor;
            mToolTip.Hide(this);
		}

		//*******************************************************************
		public void add_move_down_order()
		{
			if (this.mContextMenuStrip.Items.Count==4)
			{
                ToolStripSeparator split1 = new ToolStripSeparator();
                this.mContextMenuStrip.Items.Add(split1);
			}

            ToolStripMenuItem i = new ToolStripMenuItem();
			i.Text="Move down order";			
			i.Click += new System.EventHandler(this.move_down_order);
            i.Image = Form1.mMainForm.GetGlobalMenuItemIconsImageList().Images[9];
			this.mContextMenuStrip.Items.Add(i);

		}

		//*******************************************************************
		public void add_move_up_order()
		{
			if (this.mContextMenuStrip.Items.Count==4)
			{
                ToolStripSeparator split1 = new ToolStripSeparator();
                this.mContextMenuStrip.Items.Add(split1);
			}

            ToolStripMenuItem i = new ToolStripMenuItem();
			i.Text="Move up order";
            i.Image = Form1.mMainForm.GetGlobalMenuItemIconsImageList().Images[10];
			i.Click += new System.EventHandler(this.move_up_order);
			this.mContextMenuStrip.Items.Add(i);
		}

		//*******************************************************************
		public Color fade_to_back(Color o, Color n, int factor)
		{
			int nr = (o.R + n.R * factor ) / (factor +1);
			int ng = (o.G + (n.G * factor)) / (factor +1);
			int nb = (o.B + (n.B * factor)) / (factor +1);

            if (nr < 0) nr = 0;
            if (nr > 255) nr = 255;
            if (ng < 0) ng = 0;
            if (ng > 255) ng = 255;
            if (nb < 0) nb = 0;
            if (nb > 255) nb = 255;

			return Color.FromArgb(nr,ng,nb);
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
		//*******************************************************************
		private void DeleteAudio_Click(object sender, System.EventArgs e)
		{
			mManager.DeleteItem(this);
		}

		//*******************************************************************
		private void move_up_order(object sender, System.EventArgs e)
		{
			mManager.MoveMusicUpOrder(this);
		}

		//*******************************************************************
		private void move_down_order(object sender, System.EventArgs e)
		{
			mManager.MoveMusicDownOrder(this);
		}

		// trim music
		//*******************************************************************
		public void EditAudio_Click(object sender, System.EventArgs e)
		{
			Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();
            Form1.mMainForm.GoToMainMenu();

			TrimMusicWindow t;

			try
			{
                t = new TrimMusicWindow(this, Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow);
			}
			catch (Exception exception)
			{
				CDebugLog.GetInstance().Error("Exception occurred on creation of TrimMusicWindow: "+exception.Message);
				return ;
			}
			Point p = Cursor.Position; 
		
			t.ShowDialog();

			DoneTrim();
		
		}

		//*******************************************************************
		private void DoneTrim()
		{
            // if narration make sure we've not overlapped with other narrations
            if (mAudioType == AudioType.NARRATION)
            {
                Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow.RemoveOrShortenOverlapppingNarrations(mMusicSlide as CNarrationAudioSlide);
            }
            else
            {
                Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow.InValidateLength();
                Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow.ReCalcLoopMusicSlides();
            }

            Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow.CalcLengthOfAllSlides();

            mManager.RebuildPanel();
          
			// rebuild combo if we were synced to music
			if (Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow.SyncLengthToMusic==true && mAudioType == AudioType.MUSIC)
			{
				Form1.mMainForm.GetSlideShowManager().RebuildPanel(null);
			}

			CGlobals.mCurrentProject.DeclareChange(true,"Trim Audio Occurred");
		}
				
		//*******************************************************************
		private void mTrimMusicOnSelectedSlideItem_Click(object sender, System.EventArgs e)
		{
			Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();

	
			ArrayList selected_thumnails = Form1.mMainForm.GetSlideShowManager().GetHighlightedThumbnailSlides();

			if (selected_thumnails.Count==0) return;
		
			CImageSlide image_slide= selected_thumnails[selected_thumnails.Count-1] as CImageSlide;

			if (image_slide==null) return ;

			int slide_end_frame = image_slide.GetGlobalEndFrameBeforeTransition();

			double fps = CGlobals.mCurrentProject.DiskPreferences.AudioFramesPerSecond;

			int ms_start_frame = (int)(this.mMusicSlide.StartMusicOffset*fps);

			int total = (int) (this.mMusicSlide.Player.GetDurationInSeconds() *fps);

			// number of frames availabe
			total = total - ms_start_frame;

			int initial_silence_frames = ((int)(this.mMusicSlide.InitialSilence*fps));

            int frames_to_do = slide_end_frame - this.mMusicSlide.StartFrameOffset - initial_silence_frames;

			// make sure we don't we don't go under 2 seconds
			if (frames_to_do < 2 * fps)
			{
				// ok can we do if we remove initial silence
                if (slide_end_frame - this.mMusicSlide.StartFrameOffset >= 2 * fps)
				{
                    frames_to_do = slide_end_frame - this.mMusicSlide.StartFrameOffset;
					mMusicSlide.InitialSilence=0;
				}
				// music slide will be less than 2 seconds
				else
				{
					return ;
				}

			}

			int cut = total - frames_to_do;
			// just in case;
			if (cut <0) cut =0;

			this.mMusicSlide.EndMusicOffset = ((double)cut)/fps;
			this.mMusicSlide.FadeOut=true;
			DoneTrim();
		}

        //*******************************************************************
        public void OnMouseHover(object o, System.EventArgs e)
        {
            CNarrationAudioSlide nas = mMusicSlide as CNarrationAudioSlide;
            if (nas == null) return;

            float time = nas.StartNarrationTime;
            mToolTipX = -1;
            ShowToolTipLabel(time, 3000);
        }

        //*******************************************************************
        public void ShowToolTipLabel(float time, int length)
        {
            System.TimeSpan st = System.TimeSpan.FromSeconds(time);
            System.TimeSpan et = System.TimeSpan.FromSeconds(time + mMusicSlide.GetDurationInSeconds());

            string timerStart = string.Format("Start {0:D2}h:{1:D2}m:{2:D2}s:{3:D1}ms", st.Hours, st.Minutes, st.Seconds, st.Milliseconds / 100);
            string timerEnd = string.Format("End {0:D2}h:{1:D2}m:{2:D2}s:{3:D1}ms", et.Hours, et.Minutes, et.Seconds, et.Milliseconds / 100);

            string label = timerStart + "\n" + timerEnd;


            Point point = PointToClient(Cursor.Position);

            int currentX = Location.X;

            if (mToolTipX != currentX)
            {
                mToolTip.Show(label, this, point.X - 30, -30, length);
                mToolTipX = currentX;
            }

        }

        //*******************************************************************
        public void HideToolTipLabel()
        {
            mToolTip.Hide(this);
            mToolTipX = -1;

        }

	}
}
