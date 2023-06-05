using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DVDSlideshow;
using ManagedCore;
using System.Drawing.Drawing2D;
using DVDSlideshow.GraphicsEng;

namespace dvdslideshowfontend
{
	/// <summary>
	/// Summary description for TransitionSelectionMenu.
	/// </summary>
	public class TransitionSelectionMenu : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel mPreviewPanel;
		private System.Windows.Forms.Panel panel1;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.PictureBox mSelectionGridPB;
		private int mCurrentFrame =0;
		private Timer mTimer;
		private System.Windows.Forms.RadioButton DefaultradioButton;
		private System.Windows.Forms.RadioButton ManualRadioButton;
		private System.Windows.Forms.TrackBar trackBar1;
		private System.Windows.Forms.Button DoneButton;
		private System.Windows.Forms.Button ApplyToAllSlidesButton;
		private System.Windows.Forms.TextBox transSpeedTextBox;
		private System.Windows.Forms.Label FastLabel;
		private System.Windows.Forms.Label SlowLabel;
		private Rectangle mCurrentRect = new Rectangle(-1,-1,-1,-1);
		private System.Windows.Forms.CheckBox NoTransCheckbox;
		private CSlide	mForSlide;
		private bool mChangedFromOpen = false;
		private System.Windows.Forms.GroupBox groupBox1;
      
		private float mStartLength = 1.0f;
        private CustomButton.MiniPreviewController mPreviewController = null;
        private PictureBox[] mRecentTransitionsPBs;
        private int mHighlightedRecentPb = -1;
        private PictureBox mRecentTransitions1PB;
        private Label label1;
        private Button mPreviousPageButton;
        private Button mNextPageButton;
        private Label mCurrentPageLabel;
        private PictureBox mRecentTransitions2PB;
        private PictureBox mRecentTransitions3PB;
        private PictureBox mRecentTransitions4PB;
        private PictureBox mRecentTransitions5PB;
        private bool mTransistionTimesChanged = false;
        private bool mChangedAllTransitions = false;

        // constants
        private const int mGridsAcross = 7;
        private const int mGridsDown = 6;
        private const int mGridFrames = 48;
        private const int mMaxPages = 5;

        // cached values per class  
        static private CompressedImage[,] mFrames = null;  
        static private float mGridTofps = 0;
        static private int mPage = 0;
        static private int[] mRecentTransitionEffects;

        //*******************************************************************
        public bool ChangedAllTransitions
        {
            get { return mChangedAllTransitions; }
        }

        //*******************************************************************
        public bool TransistionTimesChanged
        {
            get { return mTransistionTimesChanged; }
        }

        //*******************************************************************
        public TransitionSelectionMenu(CSlide for_slide, CSlideShow for_slideshow)
        {
            //
            // Required for Windows Form Designer support
            //
            mForSlide = for_slide;

            InitializeComponent();

            // Adjust preview window if 4:3 project
            if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV4_3)
            {
                int oldwidth = this.mPreviewPanel.Width;
                mPreviewPanel.Width = (int)(((float)mPreviewPanel.Height) * 1.33333f);
            }

            this.mStartLength = for_slide.TransistionEffect.Length;

            mPage = GetPageIndexForDecorationIndex(for_slide.TransistionEffect.Index);

            mTransistionTimesChanged = false;

            if (mForSlide.UsesDefaultTransistionEffect == false)
            {
                this.ManualRadioButton.Checked = true;
            }
            else
            {
                this.DefaultradioButton.Checked = true;
            }

            RebuildTrackbar();

            ApplyHideUnhideState();

            mTimer = new Timer();
            mTimer.Interval = 50;
            mTimer.Tick += new EventHandler(this.TimerTick);

            mCurrentFrame = 0;

            if (mFrames == null || mFrames[mPage,0] == null)
            {
                StartGenaratingPage();
            }
            else
            {
                SetSelectionGridImage(mFrames[mPage, 0].Get());
            }

            if (mRecentTransitionEffects == null)
            {
                mRecentTransitionEffects = new int[5];
                mRecentTransitionEffects[0] = for_slide.TransistionEffect.Index;

                // if random effect, choose next
                if (mRecentTransitionEffects[0] == 0)
                {
                    mRecentTransitionEffects[0] = 1;
                }

                for (int index = 1; index < 5; index++)
                {
                    mRecentTransitionEffects[index] = -1;
                }
            }

            mRecentTransitionsPBs = new PictureBox[5];
            mRecentTransitionsPBs[0] = mRecentTransitions1PB;
            mRecentTransitionsPBs[1] = mRecentTransitions2PB;
            mRecentTransitionsPBs[2] = mRecentTransitions3PB;
            mRecentTransitionsPBs[3] = mRecentTransitions4PB;
            mRecentTransitionsPBs[4] = mRecentTransitions5PB;

            for (int i = 0; i < mRecentTransitionsPBs.Length; i++)
            {
                mRecentTransitionsPBs[i].MouseEnter += new EventHandler(RecentTransitionsPBs_MouseEnter);
                mRecentTransitionsPBs[i].MouseLeave += new EventHandler(RecentTransitionsPBs_MouseLeave);
                mRecentTransitionsPBs[i].MouseDown += new MouseEventHandler(RecentTransitionsPBs_MouseDown);
            }

            UpdateRecentTransitionEffects();

            mTimer.Start();

            this.mSelectionGridPB.MouseMove += new MouseEventHandler(this.MouseMoveGrid);
            this.mSelectionGridPB.MouseLeave += new EventHandler(this.MouseLeaveGrid);
            this.mSelectionGridPB.MouseDown += new MouseEventHandler(this.MouseDownGrid);

            this.mChangedFromOpen = false;

            float startoffset = 0;
            float endoffset = 0;
            if (for_slide.TransistionEffect !=null)
            {
                startoffset = GetStartOffset();
                endoffset = GetEndOffset();
            }

            mPreviewController = new CustomButton.MiniPreviewController(for_slideshow, for_slide, this.mPreviewPanel, startoffset, endoffset );

            UpdatePageLabels();
        }

        //*******************************************************************
        private float GetStartOffset()
        {
            if (mForSlide != null && mForSlide.TransistionEffect != null)
            {
                return mForSlide.DisplayLength - mForSlide.TransistionEffect.Length - 0.4f;
            }
            return 0;
        }


        //*******************************************************************
        private float GetEndOffset()
        {
            if (mForSlide != null && mForSlide.TransistionEffect != null)
            {
                return -0.4f;
            }
            return 0;
        }


		//*******************************************************************
		public void TimerTick(object o, EventArgs e)
		{
			mCurrentFrame++;
			if (mCurrentFrame>=mGridFrames) mCurrentFrame=0;

            SetSelectionGridImage(mFrames[mPage, mCurrentFrame].Get());        
		
			DrawHighlightRect();
			DrawCurrentTransistion();

            UpdateRecentTransitionEffects();        
		}

        //*******************************************************************
        private void TransitionSelectionMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.mTimer != null)
            {
                this.mTimer.Stop();
                this.mTimer = null;
            }

            this.mPreviewController.Stop();
        }

		//*******************************************************************
		public void ResetCurrentTransPictureBox()
		{
            if (mPreviewController == null) return; 

            mPreviewController.ResetFrameCountToStart(); 
		}


        //*******************************************************************
		private CSlideShow GetCurrentSlideShow()
		{
			return Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow;
		}

		//*******************************************************************
		public void GenerateTransistionsPictures(object o)
		{
            Font font = new Font("Segeo UI", 11);
            SolidBrush br = new SolidBrush(Color.White);
            SolidBrush br3 = new SolidBrush(Color.Snow);
            SolidBrush br2 = new SolidBrush(Color.Black);

            RenderVideoParameters renderParameters = new RenderVideoParameters();

            try
            {
                CGlobals.mCurrentProject.IgnoreProjectChanges = true;

                ManagedCore.Progress.IProgressCallback pr = (ManagedCore.Progress.IProgressCallback)o;

                if (pr != null)
                {
                    pr.Begin(0, mGridsAcross * mGridsDown);
                    pr.SetText("Loading...");
                }

                Pen blackPen = new Pen(Color.Black);

                if (mFrames == null)
                {
                    mFrames = new CompressedImage[mMaxPages, mGridFrames];
                }

                CImage i2 = new CImage(CGlobals.GetRootDirectory() + @"\guiimages\transition1.jpg", false);
                CImage i3 = new CImage(CGlobals.GetRootDirectory() + @"\guiimages\transition2.jpg", false);

                int index = GetStartIndexForCurrentPage();
                int localItemIndex = 0;

                float previewtotaltime = 2.0f;
                float averageTransitionTime = 1.5f;

                float previewfps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;
                float fGridFrames = (float)mGridFrames;
                mGridTofps = previewfps / (fGridFrames / previewtotaltime); // 1.25 for 60fps and 48

                float fAframe = fGridFrames * mGridTofps;

                float previretotalframes = previewtotaltime * previewfps;

                float spare = previewtotaltime - averageTransitionTime;      // average transisiton length

                float fhalfGridFrames = (averageTransitionTime * previewfps) + ((spare * previewfps) / 2);
                int ihlafGridGFrames = (int)(fhalfGridFrames + 0.4999f);

                Bitmap[] bitmapFrames = new Bitmap[mGridFrames];

                CTransitionEffect[] effects = CTransitionEffectTable.GetInstance().GetEffects();

                for (int j = 0; j < mGridsAcross; j++)
                {
                    for (int j1 = 0; j1 < mGridsDown; j1++)
                    {
                        DVDSlideshow.CTransitionEffect te = effects[index];

                        CSlideShow slideshow = new CSlideShow("");
                        slideshow.FadeIn = false;
                        slideshow.FadeOut = false;

                        CStillPictureSlide slide1 = new CStillPictureSlide(i2);
                        CStillPictureSlide slide2 = new CStillPictureSlide(i3);
                        ArrayList slides = new ArrayList();
                        slides.Add(slide1);
                        slides.Add(slide2);
                        slideshow.AddSlides(slides);
                        slide1.TransistionEffect = te; // must be done AFTER we've added the slides
                        slideshow.CalcLengthOfAllSlides();

                        for (int frame = 0; frame < mGridFrames; frame++)
                        {
                            if (bitmapFrames[frame] == null)
                            {
                                bitmapFrames[frame] = new Bitmap(mSelectionGridPB.Width, mSelectionGridPB.Height);
                            }

                            Bitmap i = bitmapFrames[frame] as Bitmap;

                            using (Graphics gp = Graphics.FromImage(i))
                            {

                                int width = mSelectionGridPB.Width / mGridsAcross;
                                int height = (int)(((float)width) * 0.75);

                                Rectangle r = new Rectangle(j * width, j1 * height, width, height);

                                // draw random rather than the effect itself
                                if (index == 0)
                                { 
                                    gp.FillRectangle(br2, r);
                                    gp.DrawString("Random", font, br, 10, 20);
                         
                                }
                                else
                                {
                                    renderParameters.req_height = height;
                                    renderParameters.req_width = width;

                                    float fframe = ((float)frame) * mGridTofps;
                                    int iframe = (int)(fframe + 0.4999f);

                                    renderParameters.frame = slide1.GetFrameLength() - ihlafGridGFrames + iframe;
                                    renderParameters.ignore_pan_zoom = true;
                                    Bitmap b = slideshow.RenderVideoToBitmap(renderParameters);

                                    gp.DrawImage(b, r.X, r.Y);
                                    string indexString = index.ToString();
                                    gp.DrawString(indexString, font, br2, r.X+10, r.Y+ height-20);
                                    gp.DrawString(indexString, font, br3, r.X + 11, r.Y + height - 19);
                                    b.Dispose();
                                }

                                gp.DrawRectangle(blackPen, r);
                            }

                        }
                        index++;
                        localItemIndex++;
                        if (pr != null)
                        {
                            pr.StepTo(localItemIndex);
                        }
                    }
                }

                for (int i = 0; i < mGridFrames; i++)
                {
                    if (bitmapFrames[i] != null)
                    {
                        mFrames[mPage, i] = new CompressedImage(bitmapFrames[i], 50);  // low quality to reduce memory
                        bitmapFrames[i].Dispose();
                    }
                    else
                    {
                        Log.Error(this.ToString() + "::GenerateTransistionsPictures, something went wrong with frame generating");
                    }
                }

                blackPen.Dispose();

                SetSelectionGridImage(mFrames[mPage, 0].Get());

                if (pr != null)
                {
                    pr.End();
                }

                
            }
            finally
            {
                CGlobals.mCurrentProject.IgnoreProjectChanges = false;
                font.Dispose();
                br.Dispose();
                br2.Dispose();
                br3.Dispose();
            }
		}

       
        //*******************************************************************
        private void SetSelectionGridImage(Bitmap bitmap)
        {
            if ( mSelectionGridPB.Image!=null)
            {
                mSelectionGridPB.Image.Dispose();
            }
            mSelectionGridPB.Image = bitmap;
        }


		//*******************************************************************
		public int GetGridMouseIsOn(MouseEventArgs e)
		{
			int d = (int)this.mSelectionGridPB.Width / mGridsAcross;
			int d2 = (int) (((float)d) *0.75f);

			int x = (int)(((int)e.X) / d) ;
			int y =(int)(((int)e.Y) / d2) ;

			int index = (x*mGridsDown)+y;

			Console.WriteLine("Index num was "+index);
			return index;
		}

		//*******************************************************************
		public Rectangle GetGridRectangleFromIndex(int index)
		{
			int d = (int)this.mSelectionGridPB.Width / mGridsAcross;
			int d2 = (int) (((float)d) *0.75f);

			int x = (index / mGridsDown)*d;
			int y = (index % mGridsDown)*d2;

			Rectangle r = new Rectangle(x,y,(int)d,(int)d2);
			return r;

		}


		//*******************************************************************
		public Rectangle GetGridRectangleMouseIsOn(MouseEventArgs e)
		{
			int d = (int)this.mSelectionGridPB.Width / mGridsAcross;
			int d2 = (int) (((float)d) *0.75f);

			int x = (int)((((int)e.X) / d) *d);
			int y =(int)((((int)e.Y / d2)) *d2) ;


			Rectangle r = new Rectangle(x,y,(int)d,(int)d2);
			return r;
		}

        //*******************************************************************
        private int GetStartIndexForCurrentPage()
        {
            return mPage * (mGridsDown * mGridsAcross);
        }


		//*******************************************************************
		public void MouseDownGrid(object o, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left) return ;

			// find out which item it was

            int index = GetStartIndexForCurrentPage();

			index += GetGridMouseIsOn(e);

            SelectedTransitionEffect(index);   
        }

        //*******************************************************************
        private void SelectedTransitionEffect(int index)
        {
            CTransitionEffect[] effects = CTransitionEffectTable.GetInstance().GetEffects();

            if (index <0 || index >= effects.Length) 
			{
                CDebugLog.GetInstance().Error("Index out of bounds on call to SelectedTransitionEffect:" + index);
				index =0 ;
			}

            DVDSlideshow.CTransitionEffect te = effects[index];

			this.NoTransCheckbox.Checked=false;

			if (index == mForSlide.TransistionEffect.Index)
			{
				return ;
			}

			if (this.DefaultradioButton.Checked==true)
			{
				
				this.DefaultradioButton.Checked=false;
				this.ManualRadioButton.Checked=true;
			}

			mChangedFromOpen=true;
            this.ResetCurrentTransPictureBox();

			this.mForSlide.TransistionEffect = te.Clone();				
			this.UpdateTransEffectLabel();
			this.RebuildTrackbar();

		}

		//*******************************************************************
		public void MouseLeaveGrid(object o, EventArgs e)
		{
			mCurrentRect.X =-1;
            SetSelectionGridImage(mFrames[mPage, mCurrentFrame].Get());
			mSelectionGridPB.Invalidate();
		}

		//*******************************************************************
		public void MouseMoveGrid(object o, MouseEventArgs e)
		{
			Rectangle r= GetGridRectangleMouseIsOn(e);

			if (r==this.mCurrentRect)
			{
				return;
			}

            SetSelectionGridImage(mFrames[mPage, mCurrentFrame].Get());

			mCurrentRect = r;
			mCurrentRect.Inflate(-1,-1);

			DrawHighlightRect();
			DrawCurrentTransistion();
			mSelectionGridPB.Invalidate();
		}

       
        //*******************************************************************
        private void RecentTransitionsPBs_MouseEnter(object o, EventArgs e)
        {
            Point mouseLocation = Cursor.Position;

            int count = 0;
            foreach (PictureBox pb in mRecentTransitionsPBs)
            {
                if (pb.Bounds.Contains(PointToClient(mouseLocation)))
                {
                    mHighlightedRecentPb = count;
                    break;
                }
                count++;
            }
        }

        //*******************************************************************
        private void RecentTransitionsPBs_MouseLeave(object o, EventArgs e)
        {
            mHighlightedRecentPb = -1;
            RecentTransitionsPBs_MouseEnter(o, e);
        }
        
        //*******************************************************************
        public void RecentTransitionsPBs_MouseDown(object o, MouseEventArgs e)
        {
            RecentTransitionsPBs_MouseEnter(o, e);

            if (mHighlightedRecentPb != -1)
            {
                int index = mRecentTransitionEffects[mHighlightedRecentPb];
                if (index >= 0)
                {
                    SelectedTransitionEffect(index);
                }
            }
        }

		//*******************************************************************
		public void DrawHighlightRect()
		{
			if (mCurrentRect.X <0) return ;

            using (Graphics gp = Graphics.FromImage(this.mSelectionGridPB.Image))
            {
                using (Pen p = new Pen(Color.LightGray, 1))
                {
                    gp.DrawRectangle(p, mCurrentRect);
                }
            }
		}

		//*******************************************************************
		public void DrawCurrentTransistion()
		{
			if (this.NoTransCheckbox.Checked==true)
			{
				return ;
			}

            CTransitionEffect[] effects = CTransitionEffectTable.GetInstance().GetEffects();

            foreach (CTransitionEffect e in effects)
			{
				if (this.mForSlide.TransistionEffect.Index == e.Index)
				{
                    int page = GetPageIndexForDecorationIndex(e.Index);
                    if (page == mPage)
                    {
                        int subIndex = GetSubIndexForDecorationIndex(e.Index);

                        Rectangle r = GetGridRectangleFromIndex(subIndex);
                        r.Inflate(-2, -2);
                        r.Width++;
                        r.Height++;

                        Graphics gp = Graphics.FromImage(this.mSelectionGridPB.Image);
                        Pen p = new Pen(Color.Gold, 2);
                        gp.DrawRectangle(p, r);
                        gp.Dispose();
                    }
				}
			}
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransitionSelectionMenu));
            this.mPreviewPanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.mSelectionGridPB = new System.Windows.Forms.PictureBox();
            this.DoneButton = new System.Windows.Forms.Button();
            this.ApplyToAllSlidesButton = new System.Windows.Forms.Button();
            this.DefaultradioButton = new System.Windows.Forms.RadioButton();
            this.ManualRadioButton = new System.Windows.Forms.RadioButton();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.FastLabel = new System.Windows.Forms.Label();
            this.SlowLabel = new System.Windows.Forms.Label();
            this.transSpeedTextBox = new System.Windows.Forms.TextBox();
            this.NoTransCheckbox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mPreviousPageButton = new System.Windows.Forms.Button();
            this.mNextPageButton = new System.Windows.Forms.Button();
            this.mCurrentPageLabel = new System.Windows.Forms.Label();
            this.mRecentTransitions1PB = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mRecentTransitions2PB = new System.Windows.Forms.PictureBox();
            this.mRecentTransitions3PB = new System.Windows.Forms.PictureBox();
            this.mRecentTransitions4PB = new System.Windows.Forms.PictureBox();
            this.mRecentTransitions5PB = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mSelectionGridPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mRecentTransitions1PB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mRecentTransitions2PB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mRecentTransitions3PB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mRecentTransitions4PB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mRecentTransitions5PB)).BeginInit();
            this.SuspendLayout();
            // 
            // mPreviewPanel
            // 
            this.mPreviewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mPreviewPanel.Location = new System.Drawing.Point(304, 429);
            this.mPreviewPanel.Name = "mPreviewPanel";
            this.mPreviewPanel.Size = new System.Drawing.Size(199, 112);
            this.mPreviewPanel.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.mSelectionGridPB);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(576, 368);
            this.panel1.TabIndex = 4;
            // 
            // mSelectionGridPB
            // 
            this.mSelectionGridPB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mSelectionGridPB.Location = new System.Drawing.Point(0, 0);
            this.mSelectionGridPB.Name = "mSelectionGridPB";
            this.mSelectionGridPB.Size = new System.Drawing.Size(574, 366);
            this.mSelectionGridPB.TabIndex = 0;
            this.mSelectionGridPB.TabStop = false;
            // 
            // DoneButton
            // 
            this.DoneButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.DoneButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DoneButton.Location = new System.Drawing.Point(509, 512);
            this.DoneButton.Name = "DoneButton";
            this.DoneButton.Size = new System.Drawing.Size(59, 29);
            this.DoneButton.TabIndex = 1;
            this.DoneButton.Text = "Close";
            this.DoneButton.Click += new System.EventHandler(this.doneButton_Click);
            // 
            // ApplyToAllSlidesButton
            // 
            this.ApplyToAllSlidesButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ApplyToAllSlidesButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ApplyToAllSlidesButton.ForeColor = System.Drawing.Color.Black;
            this.ApplyToAllSlidesButton.Location = new System.Drawing.Point(230, 19);
            this.ApplyToAllSlidesButton.Name = "ApplyToAllSlidesButton";
            this.ApplyToAllSlidesButton.Size = new System.Drawing.Size(59, 38);
            this.ApplyToAllSlidesButton.TabIndex = 6;
            this.ApplyToAllSlidesButton.TabStop = false;
            this.ApplyToAllSlidesButton.Text = "Apply to all slides";
            this.ApplyToAllSlidesButton.Click += new System.EventHandler(this.ApplyToAllSlidesButton_Click);
            // 
            // DefaultradioButton
            // 
            this.DefaultradioButton.Checked = true;
            this.DefaultradioButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DefaultradioButton.ForeColor = System.Drawing.Color.Black;
            this.DefaultradioButton.Location = new System.Drawing.Point(6, 82);
            this.DefaultradioButton.Name = "DefaultradioButton";
            this.DefaultradioButton.Size = new System.Drawing.Size(75, 24);
            this.DefaultradioButton.TabIndex = 7;
            this.DefaultradioButton.TabStop = true;
            this.DefaultradioButton.Text = "Default";
            this.DefaultradioButton.CheckedChanged += new System.EventHandler(this.DefaultradioButton_CheckedChanged);
            // 
            // ManualRadioButton
            // 
            this.ManualRadioButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ManualRadioButton.ForeColor = System.Drawing.Color.Black;
            this.ManualRadioButton.Location = new System.Drawing.Point(6, 22);
            this.ManualRadioButton.Name = "ManualRadioButton";
            this.ManualRadioButton.Size = new System.Drawing.Size(82, 24);
            this.ManualRadioButton.TabIndex = 8;
            this.ManualRadioButton.Text = "Custom";
            // 
            // trackBar1
            // 
            this.trackBar1.AutoSize = false;
            this.trackBar1.Location = new System.Drawing.Point(80, 14);
            this.trackBar1.Maximum = 80;
            this.trackBar1.Minimum = 1;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(113, 45);
            this.trackBar1.TabIndex = 9;
            this.trackBar1.TabStop = false;
            this.trackBar1.TickFrequency = 3;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBar1.Value = 1;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // FastLabel
            // 
            this.FastLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FastLabel.ForeColor = System.Drawing.Color.Black;
            this.FastLabel.Location = new System.Drawing.Point(86, 48);
            this.FastLabel.Name = "FastLabel";
            this.FastLabel.Size = new System.Drawing.Size(34, 16);
            this.FastLabel.TabIndex = 10;
            this.FastLabel.Text = "Fast";
            // 
            // SlowLabel
            // 
            this.SlowLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SlowLabel.ForeColor = System.Drawing.Color.Black;
            this.SlowLabel.Location = new System.Drawing.Point(162, 48);
            this.SlowLabel.Name = "SlowLabel";
            this.SlowLabel.Size = new System.Drawing.Size(48, 23);
            this.SlowLabel.TabIndex = 11;
            this.SlowLabel.Text = "Slow";
            // 
            // transSpeedTextBox
            // 
            this.transSpeedTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.transSpeedTextBox.Location = new System.Drawing.Point(193, 24);
            this.transSpeedTextBox.Name = "transSpeedTextBox";
            this.transSpeedTextBox.ReadOnly = true;
            this.transSpeedTextBox.Size = new System.Drawing.Size(28, 22);
            this.transSpeedTextBox.TabIndex = 12;
            this.transSpeedTextBox.TabStop = false;
            this.transSpeedTextBox.Text = "0.1";
            // 
            // NoTransCheckbox
            // 
            this.NoTransCheckbox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NoTransCheckbox.ForeColor = System.Drawing.Color.Black;
            this.NoTransCheckbox.Location = new System.Drawing.Point(89, 67);
            this.NoTransCheckbox.Name = "NoTransCheckbox";
            this.NoTransCheckbox.Size = new System.Drawing.Size(154, 24);
            this.NoTransCheckbox.TabIndex = 13;
            this.NoTransCheckbox.TabStop = false;
            this.NoTransCheckbox.Text = "No transition effect";
            this.NoTransCheckbox.CheckedChanged += new System.EventHandler(this.NoTransCheckbox_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.FastLabel);
            this.groupBox1.Controls.Add(this.SlowLabel);
            this.groupBox1.Controls.Add(this.trackBar1);
            this.groupBox1.Controls.Add(this.NoTransCheckbox);
            this.groupBox1.Controls.Add(this.transSpeedTextBox);
            this.groupBox1.Controls.Add(this.ApplyToAllSlidesButton);
            this.groupBox1.Controls.Add(this.ManualRadioButton);
            this.groupBox1.Controls.Add(this.DefaultradioButton);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.SteelBlue;
            this.groupBox1.Location = new System.Drawing.Point(3, 423);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(295, 118);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Transition";
            // 
            // mPreviousPageButton
            // 
            this.mPreviousPageButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mPreviousPageButton.BackgroundImage")));
            this.mPreviousPageButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.mPreviousPageButton.Location = new System.Drawing.Point(484, 381);
            this.mPreviousPageButton.Name = "mPreviousPageButton";
            this.mPreviousPageButton.Size = new System.Drawing.Size(40, 28);
            this.mPreviousPageButton.TabIndex = 15;
            this.mPreviousPageButton.TabStop = false;
            this.mPreviousPageButton.UseVisualStyleBackColor = true;
            this.mPreviousPageButton.Click += new System.EventHandler(this.mPreviousPageButton_Click);
            // 
            // mNextPageButton
            // 
            this.mNextPageButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mNextPageButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mNextPageButton.BackgroundImage")));
            this.mNextPageButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.mNextPageButton.Location = new System.Drawing.Point(528, 381);
            this.mNextPageButton.Name = "mNextPageButton";
            this.mNextPageButton.Size = new System.Drawing.Size(40, 28);
            this.mNextPageButton.TabIndex = 16;
            this.mNextPageButton.TabStop = false;
            this.mNextPageButton.UseVisualStyleBackColor = true;
            this.mNextPageButton.Click += new System.EventHandler(this.mNextPageButton_Click);
            // 
            // mCurrentPageLabel
            // 
            this.mCurrentPageLabel.AutoSize = true;
            this.mCurrentPageLabel.Location = new System.Drawing.Point(427, 388);
            this.mCurrentPageLabel.Name = "mCurrentPageLabel";
            this.mCurrentPageLabel.Size = new System.Drawing.Size(51, 13);
            this.mCurrentPageLabel.TabIndex = 17;
            this.mCurrentPageLabel.Text = "Page 1/5";
            this.mCurrentPageLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // mRecentTransitions1PB
            // 
            this.mRecentTransitions1PB.Location = new System.Drawing.Point(104, 381);
            this.mRecentTransitions1PB.Name = "mRecentTransitions1PB";
            this.mRecentTransitions1PB.Size = new System.Drawing.Size(41, 30);
            this.mRecentTransitions1PB.TabIndex = 18;
            this.mRecentTransitions1PB.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 388);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Recently used";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // mRecentTransitions2PB
            // 
            this.mRecentTransitions2PB.Location = new System.Drawing.Point(152, 381);
            this.mRecentTransitions2PB.Name = "mRecentTransitions2PB";
            this.mRecentTransitions2PB.Size = new System.Drawing.Size(41, 30);
            this.mRecentTransitions2PB.TabIndex = 20;
            this.mRecentTransitions2PB.TabStop = false;
            // 
            // mRecentTransitions3PB
            // 
            this.mRecentTransitions3PB.Location = new System.Drawing.Point(198, 381);
            this.mRecentTransitions3PB.Name = "mRecentTransitions3PB";
            this.mRecentTransitions3PB.Size = new System.Drawing.Size(41, 30);
            this.mRecentTransitions3PB.TabIndex = 21;
            this.mRecentTransitions3PB.TabStop = false;
            // 
            // mRecentTransitions4PB
            // 
            this.mRecentTransitions4PB.Location = new System.Drawing.Point(245, 381);
            this.mRecentTransitions4PB.Name = "mRecentTransitions4PB";
            this.mRecentTransitions4PB.Size = new System.Drawing.Size(41, 30);
            this.mRecentTransitions4PB.TabIndex = 22;
            this.mRecentTransitions4PB.TabStop = false;
            // 
            // mRecentTransitions5PB
            // 
            this.mRecentTransitions5PB.Location = new System.Drawing.Point(292, 381);
            this.mRecentTransitions5PB.Name = "mRecentTransitions5PB";
            this.mRecentTransitions5PB.Size = new System.Drawing.Size(41, 30);
            this.mRecentTransitions5PB.TabIndex = 23;
            this.mRecentTransitions5PB.TabStop = false;
            // 
            // TransitionSelectionMenu
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(576, 547);
            this.Controls.Add(this.mPreviousPageButton);
            this.Controls.Add(this.mRecentTransitions5PB);
            this.Controls.Add(this.mRecentTransitions4PB);
            this.Controls.Add(this.mRecentTransitions3PB);
            this.Controls.Add(this.mRecentTransitions2PB);
            this.Controls.Add(this.mRecentTransitions1PB);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mCurrentPageLabel);
            this.Controls.Add(this.mNextPageButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.DoneButton);
            this.Controls.Add(this.mPreviewPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "TransitionSelectionMenu";
            this.ShowInTaskbar = false;
            this.Text = "Transition selection";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TransitionSelectionMenu_FormClosing);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mSelectionGridPB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mRecentTransitions1PB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mRecentTransitions2PB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mRecentTransitions3PB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mRecentTransitions4PB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mRecentTransitions5PB)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion


		//*******************************************************************
		private void ApplyHideUnhideState()
		{
			if (this.DefaultradioButton.Checked==true)
			{	
				this.ApplyToAllSlidesButton.Enabled = false;
				this.trackBar1.Enabled = false;
				this.FastLabel.Enabled = false;
				this.SlowLabel.Enabled = false;
				this.transSpeedTextBox.Enabled = false;
				this.NoTransCheckbox.Enabled =false;
			}
			else
			{
				this.ApplyToAllSlidesButton.Enabled = true;

				if (this.NoTransCheckbox.Checked==false)
				{
					this.trackBar1.Enabled = true;
				}
				else
				{
					this.trackBar1.Enabled = false;
				}

				this.FastLabel.Enabled = true;
				this.SlowLabel.Enabled = true;
				this.transSpeedTextBox.Enabled = true;
				this.NoTransCheckbox.Enabled =true;
			}

            if (mForSlide.TransistionEffect.Length <= 0.0f)
			{
				this.NoTransCheckbox.Checked = true;
			}
			else
			{
				this.NoTransCheckbox.Checked = false;
			}
		}

		//*******************************************************************
		private void doneButton_Click(object sender, System.EventArgs e)
		{
            // Add to recent picture boxes if set and not random

            if (mForSlide.TransistionEffect.Length > 0.0 &&
                mForSlide.TransistionEffect.Index >0)
            {
                bool alreadyThere = false;
                foreach (int index in mRecentTransitionEffects)
                {
                    if (index == mForSlide.TransistionEffect.Index)
                    {
                        alreadyThere = true;
                        break;
                    }
                }
                if (alreadyThere == false)
                {
                    for (int i = mRecentTransitionEffects.Length - 1; i >= 1; i--)
                    {
                        mRecentTransitionEffects[i] = mRecentTransitionEffects[i - 1];
                    }

                    mRecentTransitionEffects[0] = mForSlide.TransistionEffect.Index;
                }
            }

			Close();
		}

        //*******************************************************************
		private void RebuildTrackbar()
		{
			int d = (int) (this.mForSlide.TransistionEffect.Length*10.0f) ;
			if (d<this.trackBar1.Minimum) d = this.trackBar1.Minimum;
			if (d>this.trackBar1.Maximum) d = this.trackBar1.Maximum;

			this.trackBar1.Value = d;

			UpdateTransEffectLabel();
		}

		//*******************************************************************
		protected override void OnClosed(EventArgs e)
		{
			if (this.mTimer!=null)
			{
				this.mTimer.Stop();
			}

			if (this.mForSlide.TransistionEffect.Length !=
				this.mStartLength)
			{
				mTransistionTimesChanged=true;
			}

			if (this.mChangedFromOpen==true)
			{
				CGlobals.mCurrentProject.DeclareChange(true,"Transition Effect");
			}
			base.OnClosed (e);
		}

		//*******************************************************************
		private void DefaultradioButton_CheckedChanged(object sender, System.EventArgs e)
		{				
			this.mForSlide.UsesDefaultTransistionEffect = this.DefaultradioButton.Checked;

			// got back to default from manual
			if (this.DefaultradioButton.Checked==true)
			{
                this.ResetCurrentTransPictureBox();

				CSlideShow ss = GetCurrentSlideShow();
                this.mForSlide.TransistionEffect = ss.mDefaultSlide.TransistionEffect.Clone(); 
				mChangedFromOpen = true;

                if (ss.mDefaultSlide.TransistionEffect.Length > 0.0f)
				{
					this.NoTransCheckbox.Checked=false;
				}
			}
				
			this.UpdateTransEffectLabel();
			this.RebuildTrackbar();
			
			ApplyHideUnhideState();
		}


		//*******************************************************************
		private void ApplyToAllSlidesButton_Click(object sender, System.EventArgs e)
		{
			mChangedFromOpen=true;

			mForSlide.UsesDefaultTransistionEffect = true;

			CSlideShow ss =this.GetCurrentSlideShow();

            ss.mDefaultSlide.TransistionEffect = mForSlide.TransistionEffect.Clone();
			ss.mDefaultSlide.UsesDefaultTransistionEffect = true;

			foreach (CSlide slide in ss.mSlides)
			{
				if (slide != mForSlide && slide != ss.mSlides[ss.mSlides.Count-1])
				{
                    slide.TransistionEffect = mForSlide.TransistionEffect.Clone();
					slide.UsesDefaultTransistionEffect = true;
				}
			}
			this.DefaultradioButton.Checked=true;
			mChangedAllTransitions = true;

		}

		//*******************************************************************
		private void trackBar1_Scroll(object sender, System.EventArgs e)
		{

			float v = (float) this.trackBar1.Value ;
			v/=10.0f;
			this.mForSlide.TransistionEffect.Length = v;
			UpdateTransEffectLabel();
			this.mChangedFromOpen=true;
		}

		//*******************************************************************
		private void UpdateTransEffectLabel()
		{
			
			float v = this.mForSlide.TransistionEffect.Length;
			string s =  v.ToString();
			if (s.Length >3) s = s.Substring(0,3);

			this.transSpeedTextBox.Text =s;

            // also update start/end times for preview
            if (mPreviewController != null)
            { 
                mPreviewController.StartOffsetTime = GetStartOffset();
                mPreviewController.EndOffsetTime = GetEndOffset();
                
            }
		}

		//*******************************************************************
		private void NoTransCheckbox_CheckedChanged(object sender, System.EventArgs e)
		{
			mChangedFromOpen=true;
            this.ResetCurrentTransPictureBox();

			if (this.NoTransCheckbox.Checked==true)
			{
				this.mForSlide.TransistionEffect.Length = 0.0f;
			}
			else
			{
				CSlideShow ss = GetCurrentSlideShow();
                float l = ss.mDefaultSlide.TransistionEffect.Length;
				if (l<=0.0f) l = 1.5f; // incase default was no effect
				this.mForSlide.TransistionEffect.Length = l;
			}	
		
			this.UpdateTransEffectLabel();
			this.RebuildTrackbar();
			ApplyHideUnhideState();

		}

        //*******************************************************************
        private void StartGenaratingPage()
        {
            ProgressWindow pw = new ProgressWindow(GenerateTransistionsPictures, null);//true);
            pw.StartDelegateObject = pw;
            pw.CancelButtons.Enabled = false;
            pw.Text = "Loading transition effects";
            pw.ShowDialog();
        }

        //*******************************************************************
        private void mNextPageButton_Click(object sender, EventArgs e)
        {
            if (mPage >= mMaxPages-1) return;
            mPage++;

            mPreviewController.Pause();

            if (mFrames[mPage, 0] == null)
            {      
                mTimer.Stop();
                StartGenaratingPage();
                mTimer.Start();
            }
            else
            {
                SetSelectionGridImage(mFrames[mPage, 0].Get());
            }

            this.UpdatePageLabels();

            mPreviewController.Continue();
        } 

        //*******************************************************************
        private void mPreviousPageButton_Click(object sender, EventArgs e)
        {
            if (mPage <=0) return;
            mPage--;

            mPreviewController.Pause();

            if (mFrames[mPage, 0] == null)
            {
                mTimer.Stop();
                StartGenaratingPage();
                mTimer.Start();
            }
            else
            {
                SetSelectionGridImage(mFrames[mPage, 0].Get());
            }

            this.UpdatePageLabels();

            mPreviewController.Continue();
        }

        //*******************************************************************
        // returns the page (index) a transition index is in, page 0..4 etc
        private int GetPageIndexForDecorationIndex( int index)
        {
            int pageSize = mGridsAcross * mGridsDown;
            int page = index / pageSize;
            return page;
        }

        //*******************************************************************
        // Returns the sub index for a given transition index
        // i.e. which number index it would appear on the page
        private int GetSubIndexForDecorationIndex(int index)
        {
            int pageSize = mGridsAcross * mGridsDown;
            int subIndex = index % pageSize;
            return subIndex;

        }

        //*******************************************************************
        private void UpdateRecentTransitionEffects()
        {
            if (mFrames==null) return;

            int count = -1;

            foreach (int index in mRecentTransitionEffects)
            {
                count++;

                if (index != -1)
                {
                    int page = GetPageIndexForDecorationIndex(index);
                    int subIndex = GetSubIndexForDecorationIndex(index);

                    if (mFrames[page, mCurrentFrame] == null)
                    {
                        Log.Error("Can not show transition effect, not loaded");
                        continue;
                    }

                    PictureBox pb = this.mRecentTransitionsPBs[count];

                    if (pb.Image == null)
                    {
                        pb.Image = new Bitmap(pb.Width, pb.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    }
                    Image destImage = pb.Image;
                    Image srcImage = mFrames[page, mCurrentFrame].Get();

                    using (Graphics g = Graphics.FromImage(destImage))
                    {
                        int x = subIndex / mGridsDown;
                        int y = subIndex % mGridsDown;
                        int w = pb.Width * 2;
                        int h = (pb.Height * 2) +1;
 
                        g.DrawImage(srcImage, new Rectangle(0,0, pb.Width, pb.Height), new Rectangle(x*w+1,y*h+2,w-1, h-2), GraphicsUnit.Pixel);

                        Pen blackPen = new Pen(Color.Black, 1);
                        g.DrawRectangle(blackPen, new Rectangle(0, 0, pb.Width-1, pb.Height-1));
                        blackPen.Dispose();

                        if (mHighlightedRecentPb == count)
                        {
                            Pen greyPen = new Pen(Color.LightGray, 1);
                            g.DrawRectangle(greyPen, new Rectangle(1, 1, pb.Width - 2, pb.Height - 2));
                            greyPen.Dispose();

                        }
                    }

                    pb.Refresh();
                }
            }
        }

        //*******************************************************************
        private void UpdatePageLabels()
        {
            int pageNo = mPage +1;
            mCurrentPageLabel.Text ="Page "+pageNo.ToString()+"/"+mMaxPages.ToString();
            if (pageNo == 1)
            {
                mPreviousPageButton.Enabled = false;
            }
            else
            {
                mPreviousPageButton.Enabled = true;
            }

            if (pageNo == mMaxPages)
            {
                mNextPageButton.Enabled = false;
            }
            else
            {
                mNextPageButton.Enabled = true;
            }   
        }

	}
}
