using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using DVDSlideshow;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using ZBobb;
using System.IO;
using ManagedCore;
using System.Globalization;
using DVDSlideshow.GraphicsEng;
using CustomButton;
using System.Collections.Generic;

namespace dvdslideshowfontend
{

	enum MOUSE_MODE	// mouse mode
	{
		MM_NONE,
		MM_MENU,
		MM_OBJ_SELECTED
	}
	   
	enum RESIZE_BORDER// keep track of which border of the box is to be resized.
	{
		RB_NONE = 0,
		RB_TOP = 1,
		RB_RIGHT = 2,
		RB_BOTTOM = 3,
		RB_LEFT = 4,
		RB_TOPLEFT = 5,
		RB_TOPRIGHT = 6,
		RB_BOTTOMLEFT = 7,
		RB_BOTTOMRIGHT = 8,
        RB_ROTATE =9
	}

	 
	public class Node	// Linked list of the box created. boxes are stored in Z Order.
	{
		public RectangleF rc;
		public CDecoration mDecor;
		public bool draw_dotted = true;
		public bool keep_aspect = false;
		public bool draw_enlarge_inside = false ;
		public bool mSelected_pan_start = false;
		public bool mSelected_pan_end = false;
		public bool mChangedSinceSelected=false;
		public RectangleF mOriginalCoverageArea;
        public float rotation = 0;


	}

	/// <summary>
	/// Summary description for CDecorationsManager.
	/// </summary>
	public class CDecorationsManager
	{
        // need by photocruz
        public bool mDisableMenuTextEditing = false;
	 	private object thislock = new object();
		private bool mOnlyChangeFontSizeText = false;
		private Panel mHoverPanelBox = null;
		public CImageSlide	mCurrentSlide;	
		public PictureBox mPB;
		public Panel mPanel ;
		private bool mMouseDown = false;
        private DateTime mMouseDownTime = DateTime.Now;
        private TimeSpan mTimeSpanUntilAllowMove = new TimeSpan(0, 0, 0, 0, 200);
		private Point ptStart = new Point(0,0);
		private MOUSE_MODE m_enMM;
		private RESIZE_BORDER m_enRBorder;
		private TrackBar mImageDecorationTransparentTrackbar;
        private NumericUpDown mImageDecorationTransparentNumericalArrows;
		public Label mImageDecorationTransparentLabel;
		public TrackBar mBackPlaneTransparentTrackbar;
		public Label mBackPlaneTransparentLabel;
		private bool mHackDontChangeTD=false;
		private bool mInMenuDecor=false;
        private bool mNoRepaint = false;
		ArrayList mImageRectangles;
		private Node SelectedNode;		// used for movement of selected box.
		private CRectTracker mTracker;
		private AlphaBlendTextBox mCurrentTextBox;
		private TabPage mAddTextTabPage;
		public ContextMenuStrip mDecorationContextMenu;

        private ToolStripMenuItem mOrderLayersAspectMenuItem = new ToolStripMenuItem();
        private ToolStripSeparator mSplit1MenuItem = new ToolStripSeparator();
        private ToolStripMenuItem mOriginalAspectMenuItem = new ToolStripMenuItem();
        private ToolStripMenuItem mFitToSlideMenuItem = new ToolStripMenuItem();
        private ToolStripMenuItem mFillSlideMenuItem = new ToolStripMenuItem();
        private ToolStripMenuItem mStrechToSlideMenuItem = new ToolStripMenuItem();
        private ToolStripSeparator mSplit2MenuItem = new ToolStripSeparator();
        private ToolStripSeparator mSplit3MenuItem = new ToolStripSeparator();
        private ToolStripMenuItem mEditImageMenuItem = new ToolStripMenuItem();
        private ToolStripMenuItem mAnimateDecorationMenuItem = new ToolStripMenuItem();
        private ToolStripMenuItem mAnimateDecorationSetMotionMenuItem = new ToolStripMenuItem();
        private ToolStripMenuItem mAnimateDecorationSetToFadeMenuItem = new ToolStripMenuItem();
        private ToolStripMenuItem mAnimateDecorationSetToFadeInMenuItem = new ToolStripMenuItem();
        private ToolStripMenuItem mAnimateDecorationSetToFadeInAndOutMenuItem = new ToolStripMenuItem();
        private ToolStripMenuItem mAnimateDecorationSetToFadeOutMenuItem = new ToolStripMenuItem();
        private ToolStripMenuItem mAnimateDecorationSetToNoneMenuItem = new ToolStripMenuItem();
        private ToolStripMenuItem mSetSlideLengthToVideoLengthItem = new ToolStripMenuItem();

        private ToolStripMenuItem mEditTextMenuItem = new ToolStripMenuItem();
        private ToolStripMenuItem mEditFontMenuItem = new ToolStripMenuItem();

		public Form1 mMainWindow;
		private bool mTransparentTrackerMoved = false;
		private bool mBackPlaneTransparentTrackerMoved = false;
		static float mSafeArea= 20;	// percent	( 5 percents either side = 10 percent
		private bool mStartPanTrackerChanged=false;
        private bool mStartPanRotationTrackerChanged = false;
        private bool mEndPanRotationTrackerChanged = false;

		private bool mEndPanTrackerChanged=false;
        private bool mPanInitialDelayTrackerChanged=false;
		private bool mPanEndDelayTrackerChanged=false;
		// how do you do this in c#, examples dont work?
		private Thumbnail mForThumbnail = null;
        private MiniPreviewController mPanZoomMiniPreviewController = null;
        private CDecorationManagerSettingEffects mDecorationManagerSettingEffects = null;
        private OpenFileDialog mOpenImageVideoFileDialog = new OpenFileDialog();
        private ComboBox mSelectedDecorationsCombo = null;
        private List<CDecoration> mSelectedDecorations = new List<CDecoration>();
        private NumericUpDown mPositionTop = null;
        private NumericUpDown mPositionLeft = null;
        private NumericUpDown mPositionWidth = null;
        private NumericUpDown mPositionHeight = null;
        private TrackBar mRotationTrackbar = null;
        private NumericUpDown mRotationNumericalArrows = null;
        private Label mRotationLabel = null;
        private CheckBox mPanZoomOnAllCheckBox;
        private bool mRotationTrackerMoved = false;
        private ColorDialog BackPanecolorDialog = new ColorDialog();
        private CTextStyle mCurrentSetTextStyle = null;
        private bool mNodeSetBySelectCombo = false; // if true it means we should not re-select another node on mouse down event on preview panel
        private ClipartSelectForm mClipArtSelectionForm = null;
        private bool mSettingSlide = false;

		public Thumbnail SlideThumbnail
		{
			get 
			{
				return mForThumbnail;
			}
		}

        public CDecorationManagerSettingEffects DecorationManagerSettingEffects
        {
            get { return mDecorationManagerSettingEffects;  }
        }

        //*******************************************************************
        public CDecorationsManager(System.Windows.Forms.TabPage textclip_tab,
			System.Windows.Forms.Panel form, 
			PictureBox pb, 
			ContextMenuStrip cm,
			Form1 mainform)
		{
            mDecorationManagerSettingEffects = new CDecorationManagerSettingEffects(mainform);
			mMainWindow = mainform;

			mAddTextTabPage = null;

            if (textclip_tab != null)
            {
                for (int i = 0; i < textclip_tab.Controls.Count; i++)
                {
                    Control c = textclip_tab.Controls[i];

                    TabControl tc = c as TabControl;

                    if (tc != null)
                    {
                        // sub tabs
                        for (int ii = 0; ii < c.Controls.Count; ii++)
                        {
                            Control Sub_c = c.Controls[ii];

                            if (Sub_c.Name == "AddTextTab")
                            {
                                this.mAddTextTabPage = Sub_c as TabPage;
                            }
                        }
                    }
                }
            }
			
			mPB = pb;
			mPanel=form;
			mDecorationContextMenu = cm;

            if (mDecorationContextMenu != null)
            {
                // hack
                ToolStripMenuItem deleteItem = mDecorationContextMenu.Items[2] as ToolStripMenuItem;
                mDecorationContextMenu.Items.Remove(deleteItem);

                mOrderLayersAspectMenuItem.Text = "Order layers";
                mOrderLayersAspectMenuItem.Click += new EventHandler(OrderLayersButton_Click);
                mOrderLayersAspectMenuItem.Image = mMainWindow.GetGlobalMenuItemIconsImageList().Images[8];
                mDecorationContextMenu.Items.Add(mOrderLayersAspectMenuItem);

                mSplit1MenuItem.Text = "-";
                mDecorationContextMenu.Items.Add(mSplit1MenuItem);

                mOriginalAspectMenuItem.Text = "Original aspect";
                mDecorationContextMenu.Items.Add(mOriginalAspectMenuItem);
                mOriginalAspectMenuItem.Click += new EventHandler(mOriginalAspectMenuItem_Click);

                mFitToSlideMenuItem.Text = "Fit to slide";
                mFitToSlideMenuItem.Click += new EventHandler(FitToSlideMenuItem_Click);
                mFitToSlideMenuItem.Image = mMainWindow.GetGlobalMenuItemIconsImageList().Images[12];

                mEditTextMenuItem.Text = "Edit text";
                mEditTextMenuItem.Click += new EventHandler(EditTextButton_Click);
                mEditTextMenuItem.Image = mMainWindow.GetGlobalMenuItemIconsImageList().Images[7];

                mEditFontMenuItem.Text = "Edit font";
                mEditFontMenuItem.Click += new EventHandler(SelectTextFont_click);
                mEditFontMenuItem.Image = mMainWindow.GetGlobalMenuItemIconsImageList().Images[6];

                mFillSlideMenuItem.Text = "Fill slide";
                mFillSlideMenuItem.Click += new EventHandler(FillSlideMenuItem_Click);

                mStrechToSlideMenuItem.Text = "Stretch to slide";
                mStrechToSlideMenuItem.Click += new EventHandler(mStrechToSlideMenuItem_Click);

                mDecorationContextMenu.Items.Add(mFitToSlideMenuItem);
                mDecorationContextMenu.Items.Add(mFillSlideMenuItem);
                mDecorationContextMenu.Items.Add(mStrechToSlideMenuItem);
                mDecorationContextMenu.Items.Add(mEditTextMenuItem);
                mDecorationContextMenu.Items.Add(mEditFontMenuItem);

                mSplit2MenuItem.Text = "-";
                mDecorationContextMenu.Items.Add(mSplit2MenuItem);

                mEditImageMenuItem.Text = "Edit image";
                mEditImageMenuItem.Click += new EventHandler(this.EditImageButton_click);
                mEditImageMenuItem.Image = mMainWindow.GetGlobalMenuItemIconsImageList().Images[1];

                mDecorationContextMenu.Items.Add(mEditImageMenuItem);

                mAnimateDecorationMenuItem.Text = "Motion effects";
                mAnimateDecorationMenuItem.Image = mMainWindow.GetGlobalMenuItemIconsImageList().Images[11];

                mAnimateDecorationSetMotionMenuItem.Text = "Set motion effect";
             
            
                mAnimateDecorationSetMotionMenuItem.Click += new EventHandler(AnimateDecorationButton_click);
                mAnimateDecorationSetMotionMenuItem.Image = mMainWindow.GetGlobalMenuItemIconsImageList().Images[11];
                mAnimateDecorationMenuItem.DropDownItems.Add(mAnimateDecorationSetMotionMenuItem);

                mAnimateDecorationSetToFadeMenuItem.Text = "Set to fade";
                mAnimateDecorationSetToFadeMenuItem.Image = mMainWindow.GetGlobalMenuItemIconsImageList().Images[14];

               
                mAnimateDecorationSetToFadeInMenuItem.Text ="Fade in";
                mAnimateDecorationSetToFadeInMenuItem.Click += new EventHandler(mAnimateDecorationSetToFadeInMenuItem_click);
                mAnimateDecorationSetToFadeInMenuItem.Image = mMainWindow.GetGlobalMenuItemIconsImageList().Images[14];

                mAnimateDecorationSetToFadeInAndOutMenuItem.Text = "Fade in and out";
                mAnimateDecorationSetToFadeInAndOutMenuItem.Click += new EventHandler(mAnimateDecorationSetToFadeInAndOutMenuItem_click);
                mAnimateDecorationSetToFadeInAndOutMenuItem.Image = mMainWindow.GetGlobalMenuItemIconsImageList().Images[15];

                mAnimateDecorationSetToFadeOutMenuItem.Text = "Fade out";
                mAnimateDecorationSetToFadeOutMenuItem.Click += new EventHandler(mAnimateDecorationSetToFadeOutMenuItem_click);
                mAnimateDecorationSetToFadeOutMenuItem.Image = mMainWindow.GetGlobalMenuItemIconsImageList().Images[16];


                mAnimateDecorationSetToFadeMenuItem.DropDownItems.Add(mAnimateDecorationSetToFadeInMenuItem);
                mAnimateDecorationSetToFadeMenuItem.DropDownItems.Add(mAnimateDecorationSetToFadeInAndOutMenuItem);
                mAnimateDecorationSetToFadeMenuItem.DropDownItems.Add(mAnimateDecorationSetToFadeOutMenuItem);

                mAnimateDecorationMenuItem.DropDownItems.Add(mAnimateDecorationSetToFadeMenuItem);

                mAnimateDecorationSetToNoneMenuItem.Text = "Set to none";
                mAnimateDecorationSetToNoneMenuItem.Click += new EventHandler(mAnimateDecorationSetToNoneMenuItem_click);

                mAnimateDecorationMenuItem.DropDownItems.Add(mAnimateDecorationSetToNoneMenuItem);

                mDecorationContextMenu.Items.Add(mAnimateDecorationMenuItem);

                mSetSlideLengthToVideoLengthItem.Text = "Set slide length to video length";
                mDecorationContextMenu.Items.Add(mSetSlideLengthToVideoLengthItem);
                mSetSlideLengthToVideoLengthItem.Click+=new EventHandler(mSetSlideLengthToVideoLengthItem_Click);

                mSplit3MenuItem.Text = "-";
                mDecorationContextMenu.Items.Add(mSplit3MenuItem);
                mDecorationContextMenu.Items.Add(deleteItem);
            }
             
            if (mainform!=null)
            {
			    this.mImageDecorationTransparentTrackbar = mainform.GetClipartTransparentTrackbar();
                this.mImageDecorationTransparentNumericalArrows = mainform.GetClipartTransparencyNumerical();
			    this.mImageDecorationTransparentLabel = mainform.GetClipartTransparentLabel();

			    this.mBackPlaneTransparentTrackbar = mainform.GetBackPlaneTransparentTrackbar();
			    this.mBackPlaneTransparentLabel = mainform.GetBackPlaneTransparentLabel();

                mImageDecorationTransparentNumericalArrows.ValueChanged += new EventHandler(ImageDecorationTransparentNumericalArrows_ValueChanged);
			    mImageDecorationTransparentTrackbar.ValueChanged += new EventHandler(this.ClipartTransparentTrackbar_ValueChanged);
			    mImageDecorationTransparentTrackbar.MouseUp += new MouseEventHandler(this.ClipartTransparentTrackbar_MouseUp);
			    mImageDecorationTransparentTrackbar.Scroll += new EventHandler(this.ClipartTransparentTrackbar_Scroll);

			    mBackPlaneTransparentTrackbar.ValueChanged += new EventHandler(this.BackPlaneTransparentTrackbar_ValueChanged);
			    mBackPlaneTransparentTrackbar.MouseUp += new MouseEventHandler(this.BackPlaneTransparentTrackbar_MouseUp);
			    mBackPlaneTransparentTrackbar.Scroll += new EventHandler(this.BackPlaneTransparentTrackbar_Scroll);


			    mMainWindow.GetAddNewImageButton().Click+= new System.EventHandler(this.AddNewImageButton_click);
                mMainWindow.GetAddClipArtButton().Click += new System.EventHandler(this.AddClipArtButton_click);
                mMainWindow.GetEditImageButton().Click += new EventHandler(EditImageButton_click);

                mMainWindow.GetTextFontButton().Click += new EventHandler(SelectTextFont_click);
                mMainWindow.GetDoneDecoratingMenuLayoutButton().Click += new EventHandler(SelectDoneDecoratingMenuLayoutButton);
                mMainWindow.GetOrderLayersButton().Click += new EventHandler(OrderLayersButton_Click);

                Form1.mMainForm.GetSelectBackgroundControl().ChangedBackground += this.SetBackgroundChangedSlideCallback;
                Form1.mMainForm.GetSelectBackgroundControl().ChangedAllSlidesBackground += this.BackgroundChangedOnAllSlidesCallback;

                Form1.mMainForm.GetSlideFiltersControl().ChangedSlide += new SlideFiltersControl.ChangedSlideCallback(SetFilterSlideChangedCallback);


                Form1.mMainForm.GetBordersControl().ChangedBorder += this.CommitChangesOutsideDecorationsEditor; ;

                mSelectedDecorationsCombo = mMainWindow.GetSelectedDecorationCombo();
                mSelectedDecorationsCombo.SelectedIndex = 0;
                mSelectedDecorationsCombo.SelectedIndexChanged += this.SetSelectedDecorationChanged;

                PredefinedSlideDesignsControl control = Form1.mMainForm.GetPreDefinedSlideDesignsControl();
                control.SlideImagesChanged += this.SlideImagesEditedCallback;
            }

			mPanel.DragOver += new System.Windows.Forms.DragEventHandler(this.ListDragTarget_DragOver);
			mPanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.ListDragTarget_DragDrop);
			mPanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.ListDragTarget_DragEnter);
			mPanel.DragLeave += new System.EventHandler(this.ListDragTarget_DragLeave);

			mPanel.Resize += new System.EventHandler(this.ResizeDecorationsWindow);
           
			mPB.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MyMouseDown);
			//	mPB.DoubleClick += new System.EventHandler(this.MyDoubleClick);
			
			mPB.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MyMouseUp);
		//	mPB.Paint += new System.Windows.Forms.PaintEventHandler(this.MyPaint);
			mPB.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MyMouseMove);

			if (mMainWindow!=null) mMainWindow.GetDecorationsTabControl().SelectedIndexChanged += new System.EventHandler(this.DecorationControlTabChanged);

			mPB.LostFocus += new System.EventHandler(this.PBLostFocus);

			m_enMM = MOUSE_MODE.MM_NONE;
			m_enRBorder = RESIZE_BORDER.RB_NONE;
			mImageRectangles = new ArrayList();
			mTracker = new CRectTracker(true,false);
            mTracker.ShowRotation = true;


            if (mMainWindow != null)
            {
                mainform.GetStartPanTrackBar().Scroll += new System.EventHandler(this.StartPan_Scroll);
                mainform.GetStartPanTrackBar().MouseUp += new MouseEventHandler(this.StartPan_MouseUp);

                mainform.GetStartPanRotationTrackBar().Scroll += new System.EventHandler(this.StartPanRotation_Scroll);
                mainform.GetStartPanRotationTrackBar().MouseUp += new MouseEventHandler(this.StartPanRotation_MouseUp);

                mainform.GetEndPanTrackBar().Scroll += new System.EventHandler(this.EndPan_Scroll);
                mainform.GetEndPanTrackBar().MouseUp += new MouseEventHandler(this.EndPan_MouseUp);

                mainform.GetEndPanRotationTrackBar().Scroll += new System.EventHandler(this.EndPanRotation_Scroll);
                mainform.GetEndPanRotationTrackBar().MouseUp += new MouseEventHandler(this.EndPanRotation_MouseUp);


                mainform.GetTurnOffPanZoomForSlideTickBox().CheckedChanged += new EventHandler(this.TurnOnOffPanZoomForSlideCheckedChaged);
                mainform.GetPanZoomInitialDelayTrackBar().Scroll += new EventHandler(this.PanZoomInitialDelayScroll);
                mainform.GetPanZoomEndDelayTrackBar().Scroll += new EventHandler(this.PanZoomEndDelayScroll);
                mainform.GetPanZoomInitialDelayTrackBar().MouseUp += new MouseEventHandler(this.PanZoomInitialDelayScroll_MouseUp);
                mainform.GetPanZoomEndDelayTrackBar().MouseUp += new MouseEventHandler(this.PanZoomEndDelayScroll_MouseUp);

                mainform.GetPanZoomEquationComboBox().SelectedIndexChanged += new EventHandler(this.PanZoomSmoothedComboBoxChanged);
                mainform.GetReCalcPanZoomTemplateCheckbox().CheckedChanged += new EventHandler(this.ReCalcPanZoomTemplateCheckbox_CheckedChanged);

                mainform.GetRotateDecorationClockWiseButton().Click += new System.EventHandler(this.RotateDecorationClockWiseButton_Click);
                mainform.GetRotateDecorationAntiClockWiseButton().Click += new System.EventHandler(this.RotateDecorationAntiClockWiseButton_Click);
                mainform.GetFlipDecorationLeftRightButton().Click += new System.EventHandler(this.FlipDecorationLeftRightButton_Click);
                mainform.GetFlipDecorationUpDownButton().Click += new System.EventHandler(this.FlipDecorationUpDownButton_Click);

                mainform.GetClipArtLockAspectCheckBox().Click += new System.EventHandler(this.ClipArtLockAspectCheckBox_toggled);
               
                this.mMainWindow.GetShowSelectedSlideShowMenuItem().Click += new System.EventHandler(this.ToggleShowSelectedSlideshow);
     
                this.mMainWindow.GetFontSizeComboBox().SelectedIndexChanged += new System.EventHandler(this.FontSizeIndexChange);

                this.mMainWindow.GetPanZoomLinkStartAreaButton().Click += new EventHandler(PanZoomLinkStartAreaButton_Click);

                mPositionTop = mMainWindow.GetDecorationPositionTopTextBox();
                mPositionLeft = mMainWindow.GetDecorationPositionLeftTextBox();
                mPositionWidth = mMainWindow.GetDecorationPositionWidthTextBox();
                mPositionHeight = mMainWindow.GetDecorationPositionHeightTextBox();
                mRotationTrackbar = mMainWindow.GetDecorationRotationTrackbar();
                mRotationNumericalArrows = mMainWindow.GetDecorationRotationNumericArrows();
                mRotationLabel = mMainWindow.GetDecorationRotationLabel();

                mPositionTop.ValueChanged += new EventHandler(mPositionTop_ValueChanged);
                mPositionLeft.ValueChanged += new EventHandler(mPositionLeft_ValueChanged);
                mPositionWidth.ValueChanged += new EventHandler(mPositionWidth_ValueChanged);
                mPositionHeight.ValueChanged += new EventHandler(mPositionHeight_ValueChanged);

                mRotationTrackbar.ValueChanged += new EventHandler(mRotation_ValueChanged);
                mRotationNumericalArrows.ValueChanged += new EventHandler(mRotationNumericalArrowsChanged);
                mRotationTrackbar.MouseUp += new MouseEventHandler(mRotation_Trackbar_MouseUp);
                mRotationTrackbar.Scroll += new EventHandler(mRotation_Trackbar_Scroll);

                mPanZoomOnAllCheckBox = mMainWindow.GetPanZoomOnAllCheckBox();

                mPanZoomOnAllCheckBox.CheckedChanged += new EventHandler(mPanZoomOnAllCheckBox_CheckedChanged);

                mMainWindow.GetEditTextButton().Click += new EventHandler(EditTextButton_Click);

                mCurrentSetTextStyle = CTextStyleDatabase.GetInstance().GetStyleFromName("Vanilla");
                UpdatePreviewFontPictureBox();
            }
		}


        // ******************************************************************
        private void UpdatePreviewFontPictureBox()
        {
            PictureBox pb = this.mMainWindow.GetFontPreviewPictureBox();
            if (pb.Image == null)
            {
                pb.Image = new Bitmap(pb.Width, pb.Height, PixelFormat.Format32bppArgb);
            }

            using (Graphics g = Graphics.FromImage(pb.Image))
            {
                int grey = (mCurrentSetTextStyle.TextColor.R + mCurrentSetTextStyle.TextColor.G + mCurrentSetTextStyle.TextColor.B) /3;

                Color backColour = Color.White;
                if (grey > 200)
                {
                    backColour = Color.Black;
                }

                g.Clear(backColour);
                GDITextDrawer drawer = new GDITextDrawer();

                CTextStyle previewStyle = mCurrentSetTextStyle.Clone();

                previewStyle.FontSize = 12;
                previewStyle.Format = new StringFormat();
                drawer.DrawString(g, "AbCdEeFf", 0, 0, previewStyle);
            }

            pb.Refresh();
        }


        // ******************************************************************
        void EditTextButton_Click(object sender, EventArgs e)
        {
            if (SelectedNode == null) return;
            if (SelectedNode.mDecor == null) return;
            ITextDecorationContainer decor = SelectedNode.mDecor as ITextDecorationContainer;
            if (decor == null) return;

            AddTextBox(10, 10);
        }

        //*******************************************************************
        void SelectDoneDecoratingMenuLayoutButton(object sender, EventArgs e)
        {
            mMainWindow.GoToMainMenu();
        }

        //*******************************************************************
        void SelectTextFont_click(object sender, EventArgs e)
        {
            if (mCurrentSetTextStyle == null) return;

            CTextStyle ts = mCurrentSetTextStyle.Clone();
            string text = "Text text text";

            ITextDecorationContainer decor = null;
            CTextDecoration td = null;

            if (SelectedNode !=null &&
                SelectedNode.mDecor != null)
            {
                decor = SelectedNode.mDecor as ITextDecorationContainer;
                if (decor != null)
                {
                    td = decor.TextDecoration;
                    ts = td.TextStyle;
                    text = td.mText;
                }
            }

            FontSelectorForm fsm = new FontSelectorForm();
            fsm.SetForTextStyle(ts, text);
            DialogResult dr = fsm.ShowDialog();

            if (dr == DialogResult.OK)
            {
                if (decor != null)
                {
                    CTextStyle newStyle = fsm.Style.Clone();
                    newStyle.Format = td.TextStyle.Format;
                    newStyle.FontSize = td.TextStyle.FontSize;
                    CTextStyleDatabase.GetInstance().AddStyleIfNotAlreadyInDatabase(newStyle);

                    float size = td.GetFontSizeForCoverageArea(); // get font size before style change

                    td.TextStyle = newStyle;
                    mCurrentSetTextStyle = td.TextStyle;

                    td.RecalcCoverageAreaForFontSize(size); // re-calc coverage area based on new font

                    if (td != SelectedNode.mDecor)
                    {
                        SelectedNode.mDecor.CoverageArea = td.CoverageArea;
                    }

                    SelectedNode = null;

                    // re-select the text decor (as it should be still the selected item in the combo)
                    this.SetSelectedDecorationChanged(this, new EventArgs());

                    // just in case, this should never happen really
                    if (SelectedNode == null)
                    {
                        ResetSelectedNode();
                    }
      
                    CommitChangesOutsideDecorationsEditor(false, "Changed font");
                }
                else
                {
                    mCurrentSetTextStyle = fsm.Style.Clone();
                }

                UpdatePreviewFontPictureBox();
            }
        }


		//*******************************************************************
		// user wants to add their own clipart
        public void AddClipArtButton_click(object o, System.EventArgs e)
        {
            if (mClipArtSelectionForm == null)
            {
                mClipArtSelectionForm = new ClipartSelectForm();
            }
            mClipArtSelectionForm.ShowDialog();
            string toload = mClipArtSelectionForm.SelectedItemFilename;
            if (toload !=null && toload != "")
            {
                Form1.mMainForm.GetDecorationManager().AddDecoration(toload, false);
            }

        }

        //*******************************************************************
        public void EditImageButton_click(object o, System.EventArgs e)
        {
            if (SelectedNode == null) return;
            if (SelectedNode.mDecor == null) return;

            EditSlideMediaForm esmf = new EditSlideMediaForm(mCurrentSlide, Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow, SelectedNode.mDecor as CImageDecoration);
            esmf.ShowDialog();
            this.ResetSelectedNode();
            this.RePaint();
            CommitChangesOutsideDecorationsEditor(esmf.SlideLengthChanged);
        }

          //*******************************************************************
        private CAnimatedDecoration GetSelectedAnimationDecoration()
        {
            if (SelectedNode == null || mCurrentSlide == null || SelectedNode.mDecor == null)
            {
                return null;
            }

            CAnimatedDecoration ad = SelectedNode.mDecor as CAnimatedDecoration;
            return ad;
        }

        //*******************************************************************
        public void AnimateDecorationButton_click(object o, System.EventArgs e)
        {
            CAnimatedDecoration ad = GetSelectedAnimationDecoration();
            if (ad != null)
            {
                CSlideShow ss = mMainWindow.GetSlideShowManager().mCurrentSlideShow;
                if (mCurrentSlide.PartOfAMenu == true)
                {
                    ss = null;
                }

                mDecorationManagerSettingEffects.ShowSelectDecorationMotionForm(ad, mCurrentSlide, ss);
                RePaint();
            }
        }

        //*******************************************************************
        public void mAnimateDecorationSetToFadeInMenuItem_click(object o, System.EventArgs e)
        {
            CAnimatedDecoration ad = GetSelectedAnimationDecoration();
            if (ad != null)
            {
                ad.MoveInEffect = CAnimatedDecorationEffectDatabase.GetInstance().Get("Fade");
                ad.MoveOutEffect = null;
            }
        }

        //*******************************************************************
        public void mAnimateDecorationSetToFadeInAndOutMenuItem_click(object o, System.EventArgs e)
        {
            CAnimatedDecoration ad = GetSelectedAnimationDecoration();
            if (ad != null)
            {
                ad.MoveInEffect = CAnimatedDecorationEffectDatabase.GetInstance().Get("Fade");
                ad.MoveOutEffect = CAnimatedDecorationEffectDatabase.GetInstance().Get("Fade "); // space at end means out effect
            }
        }

        //*******************************************************************
        public void mAnimateDecorationSetToFadeOutMenuItem_click(object o, System.EventArgs e)
        {
            CAnimatedDecoration ad = GetSelectedAnimationDecoration();
            if (ad != null)
            {
                ad.MoveInEffect = null;
                ad.MoveOutEffect = CAnimatedDecorationEffectDatabase.GetInstance().Get("Fade "); // space at end means out effect
            }

        }

        //*******************************************************************
        public void mAnimateDecorationSetToNoneMenuItem_click(object o, System.EventArgs e)
        {
            CAnimatedDecoration ad = GetSelectedAnimationDecoration();
            if (ad != null)
            {
                ad.MoveInEffect = null;
                ad.MoveOutEffect = null;
            }
        }

        //*******************************************************************
        private void mSetSlideLengthToVideoLengthItem_Click(object o, System.EventArgs e)
        {
            if (SelectedNode == null || mForThumbnail==null || mCurrentSlide == null || SelectedNode.mDecor == null)
            {
                return ;
            }

            CVideoDecoration dec = SelectedNode.mDecor as CVideoDecoration;
            if (dec == null)
            {
                return;
            }

            mCurrentSlide.SetLengthWithoutUpdate((float)dec.GetTrimmedVideoDurationInSeconds());
            dec.StartOffsetTimeRawValue = CAnimatedDecoration.SlideStart;
            dec.EndOffsetTimeRawValue = CAnimatedDecoration.SlideEnd;  

            CommitChangesOutsideDecorationsEditor(true);
        }

        //*******************************************************************
        // add image from file
		public void AddNewImageButton_click(object o, System.EventArgs e)
		{
            mOpenImageVideoFileDialog.Filter = CGlobals.GetImageAndVideoFileDialogFilter();

            if (mOpenImageVideoFileDialog.InitialDirectory == "")
            {
                string myPictures = DefaultFolders.GetFolder("MyPictures");
                mOpenImageVideoFileDialog.InitialDirectory = myPictures;
            }

            mOpenImageVideoFileDialog.Title = "Open image or video";

            if (mOpenImageVideoFileDialog.ShowDialog().Equals(System.Windows.Forms.DialogResult.OK))
			{
                if (CGlobals.IsImageFilename(mOpenImageVideoFileDialog.FileName) ||
                    CGlobals.IsVideoFilename(mOpenImageVideoFileDialog.FileName))
				{
                    if (ManagedCore.IO.IsDriveOkToUse(mOpenImageVideoFileDialog.FileName) == false) return;
                    AddDecorationInMiddle(mOpenImageVideoFileDialog.FileName, true);

                    mOpenImageVideoFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(mOpenImageVideoFileDialog.FileNames[0]);  // rember current folder 

				}	
			}
		}

        //*******************************************************************
        public void OrderLayersButton_Click(object o, System.EventArgs e)
        {
            CSlideShow slideshow = null;
            if (mCurrentSlide.PartOfAMenu == false)
            {
                slideshow = mMainWindow.GetSlideShowManager().mCurrentSlideShow;
            }

            OrderLayersForm olf = new OrderLayersForm(mCurrentSlide as CImageSlide, slideshow);
            olf.ShowDialog();
            this.ResetSelectedNode();
            this.RePaint();
            CommitChangesOutsideDecorationsEditor();
        }

		//*******************************************************************
		public void ClipartTransparentTrackbar_Scroll(object o, System.EventArgs e)
		{
			this.mTransparentTrackerMoved=true;
		}


        //*******************************************************************
        private void ImageDecorationTransparentNumericalArrows_ValueChanged(object o, System.EventArgs e)
        {
            if (this.mImageDecorationTransparentTrackbar.Value != (int)mImageDecorationTransparentNumericalArrows.Value)
            {
                this.mImageDecorationTransparentTrackbar.Value = (int)mImageDecorationTransparentNumericalArrows.Value;
                this.CommitChangesOutsideDecorationsEditor();
            }
        }


		//*******************************************************************
		public void ClipartTransparentTrackbar_ValueChanged(object o, System.EventArgs e)
		{
			int i = this.mImageDecorationTransparentTrackbar.Value;
			this.mImageDecorationTransparentLabel.Text = i+@"%";

            if (mImageDecorationTransparentNumericalArrows.Value != this.mImageDecorationTransparentTrackbar.Value)
            {
                mImageDecorationTransparentNumericalArrows.Value = this.mImageDecorationTransparentTrackbar.Value;
            }

			if (this.mHackDontChangeTD==true) return ;

			if (this.SelectedNode != null)
			{
                CImageDecoration dec = this.SelectedNode.mDecor as CImageDecoration;
				if (dec !=null)
				{
					dec.Transparency = ((float)i) / 100.0f;
				}
				this.RePaint();
			}
		}

		//*******************************************************************
		public void ClipartTransparentTrackbar_MouseUp(object o, System.Windows.Forms.MouseEventArgs e)
		{
			if (mTransparentTrackerMoved==true)
			{
				this.CommitChangesOutsideDecorationsEditor();
			}
			mTransparentTrackerMoved=false;
		}


		//*******************************************************************
		public void BackPlaneTransparentTrackbar_Scroll(object o, System.EventArgs e)
		{
			this.mBackPlaneTransparentTrackerMoved=true;
		}

		//*******************************************************************
		public void BackPlaneTransparentTrackbar_ValueChanged(object o, System.EventArgs e)
		{
			int i = this.mBackPlaneTransparentTrackbar.Value;
			this.mBackPlaneTransparentLabel.Text = i+@"%";

			if (mHackDontChangeTD==true) return ;

			if (this.SelectedNode != null)
			{
                ITextDecorationContainer itdc = SelectedNode.mDecor as ITextDecorationContainer;
                if (itdc != null)
				{
                    CTextDecoration td = itdc.TextDecoration;
					td.SetBackPlaneTransparency(1.0f-(((float)i) / 100.0f));
                    td.InvalidateFont();
				}
				RePaint();
			}
		}

		//*******************************************************************
		public void BackPlaneTransparentTrackbar_MouseUp(object o, System.Windows.Forms.MouseEventArgs e)
		{
			if (mBackPlaneTransparentTrackerMoved==true)
			{
				this.CommitChangesOutsideDecorationsEditor();
			}
			mBackPlaneTransparentTrackerMoved=false;
		}

		//*******************************************************************
		public void RotateDecorationClockWiseButton_Click(object o, System.EventArgs e)
		{
			if (this.SelectedNode !=null)
			{
                CImageDecoration d = this.SelectedNode.mDecor as CImageDecoration;
				if (d!=null)
				{
					d.RotateCW90();
                    if (mMainWindow.GetClipArtLockAspectCheckBox().Checked == true)
                    {
                        d.ReCalcCoverageAreaToMatchOriginalImageAspectRatio();
                        SelectedNode = null;
                        SetSelectedDecorationChanged(this, new EventArgs());  // force outline tracker to change                  
                    }
				}
				this.RePaint();
				CommitChangesOutsideDecorationsEditor();
			}
		}

		//*******************************************************************
		public void RotateDecorationAntiClockWiseButton_Click(object o, System.EventArgs e)
		{
			if (this.SelectedNode !=null)
			{
                CImageDecoration d = this.SelectedNode.mDecor as CImageDecoration;
				if (d!=null)
				{
					d.RotateCCW90();
                    if (mMainWindow.GetClipArtLockAspectCheckBox().Checked == true)
                    {
                        d.ReCalcCoverageAreaToMatchOriginalImageAspectRatio();
                        SelectedNode = null;
                        SetSelectedDecorationChanged(this, new EventArgs());  // force outline tracker to change    
                    }
				}

            
				this.RePaint();
				CommitChangesOutsideDecorationsEditor();
			}
		}

		//*******************************************************************
		public void FlipDecorationLeftRightButton_Click(object o, System.EventArgs e)
		{
			if (this.SelectedNode !=null)
			{
                CImageDecoration d = this.SelectedNode.mDecor as CImageDecoration;
				if (d!=null)
				{
					d.FlipX();
				}
				this.RePaint();
				CommitChangesOutsideDecorationsEditor();
			}
		}

		//*******************************************************************
		public void FlipDecorationUpDownButton_Click(object o, System.EventArgs e)
		{
			if (this.SelectedNode !=null)
			{
                CImageDecoration d = this.SelectedNode.mDecor as CImageDecoration;
				if (d!=null)
				{
					d.FlipY();
				}
				this.RePaint();
				CommitChangesOutsideDecorationsEditor();
			}
		}

        //*******************************************************************
        public void ClipArtLockAspectCheckBox_toggled(object o, System.EventArgs e)
        {
            if (this.SelectedNode != null)
            {
                this.SelectedNode.keep_aspect = mMainWindow.GetClipArtLockAspectCheckBox().Checked;
            }
        }
           
		//*******************************************************************
		public void PBLostFocus(object sender,System.EventArgs e)
		{
			//if (m_enMM==MOUSE_MODE.MM_MENU) return ;

		//	ResetSelectedNode();
		}


		//*******************************************************************
		public void ResetSelectedNode()
		{
			this.SelectedNode=null;
            if (mSelectedDecorationsCombo != null)
            {
                mSelectedDecorationsCombo.SelectedIndex = 0;
            }

            this.mNodeSetBySelectCombo = false;

			if (this.IsCurrentSlideAMenuSlide()==true && mInMenuDecor==false)
			{
				m_enMM = MOUSE_MODE.MM_MENU;
			}
			else
			{
				m_enMM = MOUSE_MODE.MM_NONE;
			}

			mPB.Cursor = Cursors.Default;
			ApplyHideUnhideState();
            this.RebuildSelectedDecorationsCombo();

		//	RePaint();
		}


		//*******************************************************************
		public void DecorationControlTabChanged(object sender, System.EventArgs e)
		{
			this.ResetSelectedNode();

            CustomButton.MiniPreviewController.StopAnyPlayingController();

            SetClipPanControls(false);

			RePaint();
		}

		//*******************************************************************
		public void ResizeDecorationsWindow(object sender, System.EventArgs e)
		{
			if (mCurrentSlide==null) return ;

            TransferTextBoxToTextDecoration();
			if (this.SelectedNode != null)
			{
                ResetSelectedNode();
                RePaint();
			}
		} 


		//*******************************************************************
		public bool IsCurrentSlideAMenuSlide()
		{
			if (this.mCurrentSlide==null) return false;

			if (this.mCurrentSlide.PartOfAMenu == true )
			{
				return true ;
			}
			return false;
		}

		//*******************************************************************
		public void SetCurrentSlide(CImageSlide slide)
		{
			SetCurrentSlide(slide,null);
		}

     
		//*******************************************************************
		public void SetCurrentSlide(CImageSlide slide, Thumbnail for_thumbnail)
		{
            //
            // It is possible for this to be recursive
            //
            if (mSettingSlide == true)
            {
                return;
            }

            mSettingSlide = true;

            try
            {
                lock (thislock)
                {
                    if (mCurrentTextBox != null)
                    {
                        this.TransferTextBoxToTextDecoration();
                    }

                    this.mForThumbnail = for_thumbnail;

                    bool was_in_decor_mode = mInMenuDecor;

                    mInMenuDecor = false;

                    if (slide == null)
                    {
                        // turn off any preview preview stuff
                        if (mPanZoomMiniPreviewController != null)
                        {
                            mPanZoomMiniPreviewController.Stop();
                            mPanZoomMiniPreviewController = null;
                        }

                        mCurrentSlide = null;
                        return;
                    }

                    if (mCurrentSlide != null && slide.ID == mCurrentSlide.ID)
                    {
                        if (IsCurrentSlideAMenuSlide() == true)
                        {
                            this.m_enMM = MOUSE_MODE.MM_MENU;

                            // re draw highlight around selected menu
                            if (was_in_decor_mode == true)
                            {
                                this.RePaint();
                            }
                        }
                        if (mMainWindow != null)
                        {
                            mMainWindow.GetDiskMenuManager().RebuildLabels();
                        }

                        return;
                    }

                    mCurrentSlide = slide as CImageSlide;

                    //	if (mCurrentSlide == null)
                    //	{
                    //		Console.WriteLine("Error: fix for video slides !!!");
                    //		return ;
                    //	}

                    // ok make sure we're on decorations tab


                    if (mMainWindow != null)
                    {
                        if (this.IsCurrentSlideAMenuSlide() == true)
                        {
                            this.mMainWindow.SetMainTabControlToAppropriateDiskTab();
                            CGlobals.MainMenuNeedsReRender = true;
                        }
                        else
                        {
                            SetMainTabControlToDecorationsTab();
                        }

                        mMainWindow.GetDiskMenuManager().RebuildLabels();
                    }

                    SetClipPanControls(false);

                    this.ResetSelectedNode();

                    this.RePaint();
                }


            }
            finally
            {
                mSettingSlide = false;
            }
		}


        //*******************************************************************
        public void SetMainTabControlToDecorationsTab()
        {
            mMainWindow.SuspendLayout();

            List<TabPage> pages = mMainWindow.GetOriginalDecorationTabOptions();

            TabControl control = mMainWindow.GetDecorationsTabControl();

            TabPage preDesignTabPage = mMainWindow.GetPreDesignedSlidesTabPage();

            if (control.TabPages.Count == 0) return;

            // If slide is part of a menu, only thing we can do here is add text/images
            if (mCurrentSlide.PartOfAMenu == true)
            {
                control.TabPages.Clear();
                control.TabPages.Add(mMainWindow.GetTextAndImagesTabPage());

                //
                // Make done (back to menu) button visible
                //
                mMainWindow.GetDoneDecoratingMenuLayoutButton().Visible = true;
            }
            else
            {
                mMainWindow.GetDoneDecoratingMenuLayoutButton().Visible = false;

                // come away from menu, change so we just have predesign tab
                if (control.TabPages.Count == 1 && control.TabPages[0] == mMainWindow.GetTextAndImagesTabPage())
                {
                    control.TabPages.Clear();
                    control.TabPages.Add(preDesignTabPage);
                }

                // Ok we need to select which tab options are available, depending on what slide it is
                if (control.TabPages.Count != 1 && (mCurrentSlide.AllowedToBeEditited() == false))
                {
                    while (control.TabPages.Count > 1)
                    {
                        control.TabPages.RemoveAt(control.TabPages.Count - 1);
                    }
                }
                else if (control.TabPages.Count == 1 && mCurrentSlide.AllowedToBeEditited())
                {
                    List<TabPage> tabs = mMainWindow.GetOriginalDecorationTabOptions();

                    // Reduce height by 4 as windows forms now uses size 4 pixels bigger??!! which causes tab page to be too big.
                    control.ImageList.ImageSize = new Size(control.ItemSize.Height, control.ItemSize.Width - 4);    // 90 degrees tabs, so w/h reversed

                    TabPage[] newPages = new TabPage[tabs.Count - 1];
                    int index = 0;

                    for (int i = 0; i < tabs.Count; i++)
                    {
                        if (tabs[i] != preDesignTabPage)
                        {
                            newPages[index++] = tabs[i];
                        }
                    }

                    control.TabPages.AddRange(newPages);    // add in one go to reduce flicker
                }

            }

            if (this.mMainWindow.GetMainTabControl().SelectedTab != mMainWindow.GetDecorationsTabPage())
            {
                this.mMainWindow.GetMainTabControl().SelectedTab = mMainWindow.GetDecorationsTabPage();
            }

            mMainWindow.ResumeLayout();
        }


		//*******************************************************************
		// user has selected the decorations tab
		public void SelectedDecorationsTab()
		{
			// if the menu is current slide then switch to decorations mode

			if (IsCurrentSlideAMenuSlide()==true)
			{
				this.m_enMM = MOUSE_MODE.MM_NONE;
				this.mInMenuDecor = true;

				// remove yellow highlight thing around selected slideshow
				if (this.IsCurrentSlideAMenuSlide()==true)
				{
					this.RePaint();
				}
			}
		}

        //*******************************************************************
        private bool IsDecorationTabForSlide()
        {
            if (mMainWindow == null) return false;

            if (mMainWindow.GetMainTabControl().SelectedTab != mMainWindow.GetDecorationsTabPage())
            {
                return false;
            }

            if (IsCurrentSlideAMenuSlide() == true)
            {
                return false;
            }

            return true;
        }


		//*******************************************************************
		public bool IsPanZoomEditMode()
		{
            if (IsDecorationTabForSlide() == false) return false;

			if (this.mMainWindow.GetDecorationsTabControl().SelectedTab != mMainWindow.GetClipPanTabPage())
			{
				return false ;
			}

			return true;
		}


        //*******************************************************************
        public bool IsPreDefinedSlideDesignMode()
        {
            if (IsDecorationTabForSlide() == false) return false;

            if (this.mMainWindow.GetDecorationsTabControl().SelectedTab != mMainWindow.GetPreDesignedSlidesTabPage())
            {
                return false;
            }

            return true;
        }

        //*******************************************************************
        public bool IsSelectFiltersMode()
        {
            if (IsDecorationTabForSlide() == false) return false;

            if (this.mMainWindow.GetDecorationsTabControl().SelectedTab != mMainWindow.GetSelectFiltersTabPage())
            {
                return false;
            }

            return true;
        }

        //*******************************************************************
        public bool IsSelectBackgroundMode()
        {
            if (IsDecorationTabForSlide() == false) return false;

            if (this.mMainWindow.GetDecorationsTabControl().SelectedTab != mMainWindow.GetBackgroundTabPage())
            {
                return false;
            }

            return true;

        }

        //*******************************************************************
        public bool IsBordersTabMode()
        {
             if (IsDecorationTabForSlide() == false) return false;

            if (this.mMainWindow.GetDecorationsTabControl().SelectedTab != mMainWindow.GetBordersTabPage())
            {
                return false;
            }

            return true;
        }

		//*******************************************************************
        private GraphicsEngine.State RenderCurrentSlideVideo(Rectangle r)
		{
           // Console.WriteLine(" Decoration man RenderCurrentSlideVideo");

            if (mCurrentSlide == null) return GraphicsEngine.State.OK;
			bool vs = CGlobals.VideoSeeking;
			bool wbsc = CGlobals.WaitVideoSeekCompletion;
			bool mute_sound = CGlobals.MuteSound;

			CGlobals.VideoSeeking=true;
			CGlobals.WaitVideoSeekCompletion = true ;
			CGlobals.MuteSound=true;

            GraphicsEngine engine = GraphicsEngine.Current;

            RenderSurface previousSurface = engine.GetRenderTarget();

            CMainMenu menu = null;
            if (mCurrentSlide.PartOfAMenu == true)
            {
                menu = CGlobals.mCurrentProject.GetMenuWhichContainsSlide(this.mCurrentSlide);
            }

            GraphicsEngine.State state = engine.BeginScene(null);
            if (state == GraphicsEngine.State.OK)
            {
                try
                {
                    int w = CGlobals.mCurrentProject.DiskPreferences.CanvasWidth;
                    int h = CGlobals.mCurrentProject.DiskPreferences.CanvasHeight;

                    using (RenderSurface surface = engine.GenerateRenderSurface((uint)w, (uint)h, this.ToString() + "::GeneratePictureBox"))
                    {
                        engine.SetRenderTarget(surface);
                        engine.ClearRenderTarget(0, 0, 0, 255);
                        mCurrentSlide.RenderFrame(-1, true, false, w, h);
                        if (menu != null)
                        {
                            menu.RenderUnhighlightedSubPictures(w, h);
                        }
                  
                        CImage image = new CImage(this.mPB.Image);
                        engine.CopyDefaultSurfaceToImage(image);                     
                    }
                }
                finally
                {
                    engine.SetRenderTarget(previousSurface);
                    engine.EndScene();
                    state = engine.PresentToWindow(engine.HiddenWindow.Handle); // not really neeaded, but to be safe present to gidden window

                    CGlobals.WaitVideoSeekCompletion = wbsc;
                    CGlobals.VideoSeeking = vs;
                    CGlobals.MuteSound = mute_sound;
                }
            }

			return state;
		}


		//*******************************************************************
		public void DrawSafeArea(Image image)
		{
            if (image == null) return;

            using (Graphics g = Graphics.FromImage(image))
            {
                float pa = mSafeArea;

                Pen p = new Pen(Color.FromArgb(180, 255, 255, 255), 2);
                p.DashStyle = DashStyle.Dot;

                float w = (float)image.Width;
                float h = (float)image.Height;

                g.DrawRectangle(p, w / pa, h / pa, w - ((w / pa) * 2), h - ((h / pa) * 2));
            }
		}

        //*******************************************************************
        private void TemplateDesignChagedCallback(CImageSlide newSlide)
        {
            bool noRepaint = mNoRepaint;
            mNoRepaint = true;
            try
            {
                Form1.mMainForm.GetSlideShowManager().RebuildPanel();
            }
            finally
            {
                mNoRepaint = noRepaint;
            }     

            Thumbnail thumb =  Form1.mMainForm.GetSlideShowManager().HighlightSlide(newSlide);
            if (thumb !=null)
            {
                SetCurrentSlide(newSlide, thumb);
                return;
            }
            Log.Warning("Could not find thumbnail for changed slide design");
        }


        //*******************************************************************
        private void SlideImagesEditedCallback(bool slideLengthChanged)
        {
            this.RePaint();
            this.CommitChangesOutsideDecorationsEditor(slideLengthChanged);

        }

            
        //*******************************************************************
        private void RePaintForMiniPreviewControllersTabs()
        {
            // I.e. refresh

            // turn off any preview preview stuff
            if (mPanZoomMiniPreviewController != null)     // legacy
            {
                mPanZoomMiniPreviewController.Stop();
                mPanZoomMiniPreviewController = null;
            }

            // Refresh/set slide for predefined slide tab if selected
            if (IsPreDefinedSlideDesignMode() == true)
            {
                PredefinedSlideDesignsControl control = Form1.mMainForm.GetPreDefinedSlideDesignsControl();
                control.SetSlide(mCurrentSlide,
                                 Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow,
                                 TemplateDesignChagedCallback,
                                 mPB);
            }
          
            else if (IsSelectFiltersMode() == true)
            {
                SlideFiltersControl sfc = Form1.mMainForm.GetSlideFiltersControl();
                sfc.SetSlide(mCurrentSlide,
                              Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow,
                              Form1.mMainForm.GetSlideShowManager().RebuildPanel,
                              mPB);

            }
            else if (IsBordersTabMode() == true)
            {
                OverlaySelectionControl osc = Form1.mMainForm.GetBordersControl();
                osc.SetSlide(mCurrentSlide,
                              Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow,
                              Form1.mMainForm.GetSlideShowManager().RebuildPanel,
                              mPB);
            }

        }

		//*******************************************************************
		public void RePaint()
		{           
            if (mNoRepaint == true)
            {
                return;
            }

            // if in ay of these modes, the area is painted by mini preview controller
            if (IsBordersTabMode() == true ||
                IsPreDefinedSlideDesignMode() == true ||
                IsSelectFiltersMode() == true)
            {
                RePaintForMiniPreviewControllersTabs();
            }
            else
            {
                RePaintForNonPreviewTabs();
            }

            // defafault is render straight to window
            if (mPB.Visible == false)
            {
                this.mPB.Visible = true;
            }
            else
            {
                this.mPB.Refresh();
            }
			CStillPictureSlide cs = mCurrentSlide as CStillPictureSlide;

			if (cs !=null &&
				cs.PartOfAMenu==true)
			{
				CGlobals.MainMenuNeedsReRender=false;
			}
		}

        //*******************************************************************
        private void RePaintForNonPreviewTabs()
        {
            if (IsSelectBackgroundMode() == true)
            {
                CustomButton.SelectBackgroundControl sbc = Form1.mMainForm.GetSelectBackgroundControl();
                sbc.SetSlide(mCurrentSlide, Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow);
            }

            Rectangle r = new Rectangle(0, 0, CGlobals.mCurrentProject.DiskPreferences.CanvasWidth, CGlobals.mCurrentProject.DiskPreferences.CanvasHeight);


            bool isPanZoomMode = IsPanZoomEditMode();

            try
            {
                if (this.mPB.Image == null ||
                this.mPB.Image.Width != r.Width ||
                this.mPB.Image.Height != r.Height)
                {
                    if (this.mPB.Image != null)
                    {
                        this.mPB.Image.Dispose();
                    }
                    this.mPB.Image = new Bitmap(r.Width, r.Height, PixelFormat.Format32bppArgb);
                }

                if (IsCurrentSlideAMenuSlide() && ShouldShowMenus() == false)
                {
                    // No menu, just draw blank screen;
                    using (Graphics g = Graphics.FromImage(mPB.Image))
                    {
                        g.Clear(Color.Black);
                    }
                }
                else
                {
                    // render slide
                    if (isPanZoomMode == false)
                    {
                        MiniPreviewController.StopAnyPlayingController();
                    }

                    GraphicsEngine.State state = RenderCurrentSlideVideo(r);
                    if (state != GraphicsEngine.State.OK) return;

                    this.HighlightCurrentSlideshow();
                }

                if (SelectedNode != null)
                {
                    if (this.m_enMM != MOUSE_MODE.MM_MENU)
                    {
                        float rotation = SelectedNode.rotation;

                        mTracker.DrawSelectionGhostRect(CGlobals.ToRectangle(SelectedNode.rc), rotation, (Bitmap)this.mPB.Image);
                        mTracker.DrawSelectionTrackers(CGlobals.ToRectangle(SelectedNode.rc), rotation, (Bitmap)this.mPB.Image);
                    }
                }

                if (this.mMainWindow != null)
                {
                    if (isPanZoomMode == true)
                    {
                         // In case we selected another slide whilst remaining in pan/zoom tab
                        if (mPanZoomMiniPreviewController != null &&
                            mPanZoomMiniPreviewController.ForSlide != mCurrentSlide)
                        {
                            mPanZoomMiniPreviewController.Stop();
                            mPanZoomMiniPreviewController = null;
                        }

                        // if preview timer needs setting up then set it up
                        if (mPanZoomMiniPreviewController == null)
                        {
                            mPanZoomMiniPreviewController = new MiniPreviewController(
                                Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow,
                                mCurrentSlide,
                                this.mMainWindow.GetPreviewPanZoomPictureBox());
                        }
                      
                        if (mCurrentSlide.UsePanZoom == true)
                        {
                            CDecorationsManagerPanZoom.DrawPanZoomRectangles(mCurrentSlide, this.mPB.Image, mMainWindow);
                        }
                    }
                    else
                    {

                        // turn off any preview preview stuff
                        if (mPanZoomMiniPreviewController != null)     // legacy
                        {
                            mPanZoomMiniPreviewController.Stop();
                            mPanZoomMiniPreviewController = null;
                        }
                     
                    }
                }
            }

            catch (Exception e)
            {
                CDebugLog.GetInstance().Error("Exception occurred in Decoration manager repaint: "+e.Message);
            }
        }

        //*******************************************************************
        private void ListDragTarget_DragOver(object sender, System.Windows.Forms.DragEventArgs e) 
		{
			// allow drop of thumnails
			if ( e.Data.GetDataPresent(typeof(Thumbnail)) == true)
			{
				e.Effect = DragDropEffects.Copy;
			}

			if ( e.Data.GetDataPresent(typeof(ClipartThumbnail)) == true)
			{
				e.Effect = DragDropEffects.Copy;
			}

			if ( e.Data.GetDataPresent(typeof(MenuThumbnail)) == true)
			{
				e.Effect = DragDropEffects.Copy;
			}
		}

		//*******************************************************************
        private void AddDecorationInMiddle(string filename, bool smoothAlphaMap) 
		{
			if (mCurrentSlide == null) return ;
			
			float width = (float) this.mPB.ClientRectangle.Width;
			float height = (float) this.mPB.ClientRectangle.Height;

			float rr2 = height/width;

			RectangleF r = new RectangleF();
			r.X = 0.4f;
			r.Y = 0.4f;
			r.Width = 0.2f * rr2;
			r.Height = 0.2f;

            AddDecoration(filename, r, smoothAlphaMap);
		}


		//*******************************************************************
        public void AddDecoration(string filename, bool smoothAlphaMap) 
		{
            AddDecorationInMiddle(filename, smoothAlphaMap);

		}

		//*******************************************************************
		private void AddDecoration(string filename, RectangleF area, bool smoothAlphaMap)
		{
			if (mCurrentSlide == null) return ;
		
            CImageDecoration d = null;

            float ww = 1;
            float hh = 1;

            if (CGlobals.IsVideoFilename(filename) )
            {
                CVideoDecoration videoDecoration = new CVideoDecoration(filename, area, 0);
                videoDecoration.AttachedToSlideImage = false;
                if (videoDecoration.Player == null) return;

                ww = videoDecoration.GetVideoWidth();
                hh = videoDecoration.GetVideoHeight();
                d = videoDecoration;
              
            }
            else
            {
                CClipArtDecoration clipartDecoration = new CClipArtDecoration(filename, area, 0);

                ww = clipartDecoration.mImage.GetRawImage().Width;
                hh = clipartDecoration.mImage.GetRawImage().Height;

                d = clipartDecoration;
            }

            if (d.VerfifyAllMediaFilesToRenderThisExist() == true)
            {
                if (smoothAlphaMap == true)
                {
                    d.SetToSmoothEdgeAlphamap();
                }

                float ratio = ww / hh;

                area.Width *= ratio;
                d.CoverageArea = area;


                mCurrentSlide.AddDecoration(d);
            }

            RebuildSelectedDecorationsCombo();
			RePaint();

			this.CommitChangesOutsideDecorationsEditor();

			//this.SetCurrentSlide(tn.mSlide);

		}



		//*******************************************************************
		private void ListDragTarget_DragDrop(object sender, System.Windows.Forms.DragEventArgs e) 
		{
			// slide 
			Thumbnail tn = e.Data.GetData(typeof(Thumbnail)) as Thumbnail;

			if (tn!=null)
			{
				this.SetCurrentSlide(tn.mSlide, tn);
				return ;
			}

			// clip art
			ClipartThumbnail catn = e.Data.GetData(typeof(ClipartThumbnail)) as ClipartThumbnail;

			if (catn!=null)
			{ 
				AddDecoration(catn.mFileName, true);
				return ;
			}


			MenuThumbnail mtt = e.Data.GetData(typeof(MenuThumbnail)) as MenuThumbnail;

			if (mtt!=null)
			{
				SetMenuBackdrop(mtt);
			}
		}

		//*******************************************************************
		// another way of doing drop on menuthumbnail
		public void SetMenuBackdrop(MenuThumbnail mtt)
		{
            if (mtt != null && mCurrentSlide!=null)
			{
				CMainMenu menu = mMainWindow.mCurrentProject.GetMenuWhichContainsSlide(this.mCurrentSlide);

				menu.SetBackground(mtt.mFileName);

                SetCurrentSlide(menu.BackgroundSlide);

				RePaint();

				mMainWindow.GetDiskMenuManager().RebuildLabels();

				this.CommitChangesOutsideDecorationsEditor();
			}

		}
		
		//*******************************************************************
		private void ListDragTarget_DragEnter(object sender, System.Windows.Forms.DragEventArgs e) 
		{
			// Reset the label text.
			int i=1;
			i++;
			i++;

			//DropLocationLabel.Text = "None";
		}

		
		//*******************************************************************
		private void ListDragTarget_DragLeave(object sender, System.EventArgs e) 
		{
			// Reset the label text.
			int i=1;
			i++;
			i++;

			//DropLocationLabel.Text = "None";
		}


		//*******************************************************************
		private void MyMouseUp(object sender, System.Windows.Forms.MouseEventArgs e1)
		{	
			Point e = this.ClientPointToImagePoint(new Point(e1.X, e1.Y));

			// bring up contxct menu if in main menu and not decorating
			if(IsCurrentSlideAMenuSlide()==true && this.mInMenuDecor==false)
			{	// adding the new rectangle in the list.
				
				mPB.Cursor = Cursors.Default;
				if (this.SelectedNode!=null && e1.Button == MouseButtons.Right)
				{
					CMenuSlideshowButton msb = SelectedNode.mDecor as CMenuSlideshowButton;
					if (msb!=null && this.mMainWindow != null)
					{
						Form1.mMainForm.GetDiskMenuManager().RightSelectedSlideShow(sender,e1,msb.GetInnerImageStringId(), mMainWindow.mCurrentProject.GetMenuWhichContainsSlide(this.mCurrentSlide));

						this.RePaint();
					}	
				}
			}
			else if(m_enMM == MOUSE_MODE.MM_OBJ_SELECTED)
			{
				if (IsPanZoomEditMode() == true)
				{
					mMouseDown = false;		
					if (this.SelectedNode.mChangedSinceSelected==true)
					{
						CommitChangesOutsideDecorationsEditor();
					}

					return ;
				}

			//	if(bShouldRePaint)
			//	{ 
			//		this.RePaint();
			//		bShouldRePaint = false;
			//	}

				if(e1.Button == MouseButtons.Right)
				{
					// ergghh make sure we can't remove a slideshowbutton or link

					if (this.SelectedNode !=null &&
						this.SelectedNode.mDecor!=null)
					{
                        // If clipart, add items options

                        CClipArtDecoration cad = this.SelectedNode.mDecor as CClipArtDecoration;
                        CVideoDecoration vd = this.SelectedNode.mDecor as CVideoDecoration;
                        if (cad != null || vd !=null)
                        {
                            mOriginalAspectMenuItem.Visible = true;
                            mFitToSlideMenuItem.Visible = true;
                            mFillSlideMenuItem.Visible = true;
                            mStrechToSlideMenuItem.Visible = true;
                            mEditImageMenuItem.Visible = true;
                            mSplit2MenuItem.Visible = true;
                            mEditTextMenuItem.Visible = true;
                            mEditFontMenuItem.Visible = true;
                            mAnimateDecorationMenuItem.Visible = true;
                            mSetSlideLengthToVideoLengthItem.Visible = ShowSetSlideLengthToVideoLengthOption(vd);
                           
                        }
                        else
                        {
                            // this is a menu item most likely 

                            mOriginalAspectMenuItem.Visible = false;
                            mFitToSlideMenuItem.Visible = false;
                            mFillSlideMenuItem.Visible = false;
                            mStrechToSlideMenuItem.Visible = false;
                            mEditImageMenuItem.Visible = false;
                            mSplit2MenuItem.Visible = false;
                            mAnimateDecorationMenuItem.Visible = false;
                            mSetSlideLengthToVideoLengthItem.Visible = false;
                        }

                        ITextDecorationContainer itdc = SelectedNode.mDecor as ITextDecorationContainer;
                        bool enabledText = false;
                        if (itdc != null)
                        {
                            CTextDecoration td = itdc.TextDecoration;
                            if (td.Editable)
                            {
                                mEditTextMenuItem.Visible = true;
                                mEditFontMenuItem.Visible = true;

                                if (SelectedNode.mDecor is CMenuButton == false)
                                {
                                    mAnimateDecorationMenuItem.Visible = true;
                                }
                                enabledText = true;
                            }
                        }
                        
                        if (enabledText==false)
                        {
                            mEditTextMenuItem.Visible = false;
                            mEditFontMenuItem.Visible = false;
                        }

                        ToolStripItem delete_menu_item_index = null;

						// ok retrive delete from context menu
                        foreach (ToolStripItem item in mDecorationContextMenu.Items)
						{
							if (item.Text=="Delete")
							{
								delete_menu_item_index = item;
							}
						}
							
						if (delete_menu_item_index!=null)
						{
							if (IsDecorationAllowedToBeDeletedByUser(this.SelectedNode.mDecor) == false)
							{
								delete_menu_item_index.Visible=false;
                                mSplit1MenuItem.Visible = false;
                                mSplit2MenuItem.Visible = false;
                                mSplit3MenuItem.Visible = false;
							}
							else
							{
								delete_menu_item_index.Visible=true;
                                // 2 already done above
                                mSplit3MenuItem.Visible = true;
                                mSplit1MenuItem.Visible = true;
							}
						}

						mDecorationContextMenu.Show(this.mPB, new Point(e1.X, e1.Y));   
					}
				}
				else
				{
					this.RePaint();

					if (this.SelectedNode!=null)
					{
						// ok check we've not moved a slideshowbutton or link to an iligal position

						bool changedsince_selected = this.SelectedNode.mChangedSinceSelected;
						bool res = CheckIfSelectNodeNeedsRepositiong(this.SelectedNode);

						if (changedsince_selected==true)
						{
							CommitChangesOutsideDecorationsEditor();
						}
					}
				}
			}

			mMouseDown = false;		
		}

        //*******************************************************************
        private bool IsDecorationAllowedToBeDeletedByUser(CDecoration d)
        {
            if (d==null)
            {
                return false;
            }

            if (d is CMenuSlideshowButton || d is CMenuLinkButton)
            {
                return false;
            }
            return true;
        }

        //*******************************************************************
        private bool ShowSetSlideLengthToVideoLengthOption(CVideoDecoration dec)
        {
            if (dec == null || mCurrentSlide == null || IsCurrentSlideAMenuSlide() )
            {
                return false; 
            }

            return mForThumbnail.ShowSetSlideLengthToVideoLengthOption(dec);
        }

		//*******************************************************************
		// this function is for testing if a moved node which is a slideshowbutton
		// or slideshow link is know in a iligal position, if so move it back to
		// where it was before

		private bool CheckIfSelectNodeNeedsRepositiong(Node for_node)
		{
			if (for_node.mDecor as CMenuSlideshowButton == null &&
				for_node.mDecor as CMenuLinkButton == null)
			{
				return true;;
			}

			bool move_decor_back = false;

			if (CMainMenu.IsPartOfDecorationOffMenu(for_node.mDecor)==true)
			{
				move_decor_back=true;
			}

			foreach (CDecoration dec in this.mCurrentSlide.Decorations)
			{
				if  ((dec as CMenuSlideshowButton != null ||
					 dec as CMenuLinkButton != null) &&
					 dec != for_node.mDecor)
				{
					if (CMainMenu.IsDecorationIntersecting(dec, for_node.mDecor)==true)
					{
						move_decor_back=true;
					}
					
				}
			}

			if (move_decor_back==true)
			{
				for_node.mDecor.CoverageArea = for_node.mOriginalCoverageArea;
				this.ResetSelectedNode();
				this.RePaint();
				m_enMM = MOUSE_MODE.MM_NONE;
				mPB.Cursor = Cursors.Default;
				m_enRBorder = RESIZE_BORDER.RB_NONE;

				return false;
			}

			return true;
		}

        //*******************************************************************
        private void CommitChangesOutsideDecorationsEditor()
        {
            CommitChangesOutsideDecorationsEditor(false);
        }

        //*******************************************************************
        private void CommitChangesOutsideDecorationsEditor(bool slideLengthChanged)
        {
            CommitChangesOutsideDecorationsEditor(slideLengthChanged, "Slide decoration change");
        }

        //*******************************************************************
        // commit changes to the slide's internal decoration image 
        private void CommitChangesOutsideDecorationsEditor(bool slideLengthChanged, string momentoString)
        { 
            if (slideLengthChanged == true)
            {
                mMainWindow.GetSlideShowManager().mCurrentSlideShow.InValidateLength();
            }

			CGlobals.mCurrentProject.DeclareChange(true, momentoString);

			if (mForThumbnail!=null)
			{
                mForThumbnail.InvalidateImage(slideLengthChanged);
			}
		}

		//*******************************************************************
		private void AddTextBox(int x,int y)
		{
			Point e = this.ClientPointToImagePoint(new Point(x, y));

			float cw = this.mPB.Image.Width;
			float w = this.mPB.ClientRectangle.Width;

			float ch = this.mPB.Image.Height;
			float h = this.mPB.ClientRectangle.Height;

			if (h<1) return ;
			if (w<1) return ;

			float w2= w/cw;
			float h2 =h/ch;

			float rat =  ((float)mPB.ClientRectangle.Width)/ ((float)mPB.Image.Width);

			bool added = false;

			if (this.SelectedNode !=null)
			{
                ITextDecorationContainer itdc = SelectedNode.mDecor as ITextDecorationContainer;

                if (itdc != null)
				{
                    CDecoration d = SelectedNode.mDecor;
                    CTextDecoration td = itdc.TextDecoration;

                    if (td.Editable == false)
                    {
                        return;
                    }

					this.ResetSelectedNode();
					m_enMM = MOUSE_MODE.MM_NONE;

					added=true;

                    RectangleF originalCoverageArea = d.CoverageArea;
                    d.CoverageArea = new RectangleF(-1, -1, 0, 0);
				
					this.RePaint();
					Bitmap bb = (Bitmap)this.mPB.Image.Clone();
                    d.CoverageArea = originalCoverageArea;
                    mCurrentTextBox = new AlphaBlendTextBox(d, this.mPB, this, bb);
				
				}
			} 
			
			// create a new text box
			if (added==false)
			{
                this.ResetSelectedNode();

				m_enMM = MOUSE_MODE.MM_NONE;
                CTextStyle style = GetCurrentStyle();

                // Prevent us starting with font size too big
                if (style.FontSize > 100)
                {
                    style.FontSize = 26;
                }

				RectangleF coverage = new RectangleF(e.X/cw,(e.Y-style.FontSize/2)/ch,0.00f,0.00f); // 13 = half
				this.RePaint();
				Bitmap bb = (Bitmap)this.mPB.Image.Clone();
				CTextDecoration d= new  CTextDecoration("", coverage, 0,style.FontSize);

                d.TextStyle = style;
				mCurrentTextBox = new AlphaBlendTextBox(d, this.mPB, this,bb);
			} 

			// set the position for the thumbnail and add it to the panel's controls
		
			mPB.Controls.Add(mCurrentTextBox);
			mCurrentTextBox.Focus();
            try
            {
                int l = mCurrentTextBox.Text.Length;
                mCurrentTextBox.Select(l, 0);
            }
            catch
            {
            }

			mCurrentTextBox.LostFocus += new System.EventHandler(this.MyTextBoxLostFocus);
			
			RePaint();

		}

		//*******************************************************************
		private void MyTextBoxLostFocus(object sender,System.EventArgs e)
		{
            TransferTextBoxToTextDecoration();
            mCurrentTextBox = null;

			RePaint();
		}

        //*******************************************************************
        public bool IsTextBoxEditing()
        {
            return mCurrentTextBox != null;
        }


		//*******************************************************************
		private void MyDoubleClick(object sender,System.Windows.Forms.MouseEventArgs e1)
		{
			Point e = this.ClientPointToImagePoint(new Point(e1.X, e1.Y));
			if (this.m_enMM == MOUSE_MODE.MM_MENU) return ;
			if (this.IsPanZoomEditMode()==true) return ;

			// check we not already over text block
			if (mCurrentTextBox!=null)
			{
				Console.WriteLine("Error: all ready got textbox ???");
				return ;
			}

			AddTextBox(e1.X, e1.Y);
		
		}

		//*******************************************************************
		private void MainPaintBoxGainFocus()
		{
			this.mPB.Focus();
		}

		//*******************************************************************
		public void SelectFirstSlideshowInCurrentMenu()
		{
			if (this.mCurrentSlide==null) return;
			if (this.IsCurrentSlideAMenuSlide()==false) return;

			CMainMenu the_menu = CGlobals.mCurrentProject.GetMenuWhichContainsSlide(this.mCurrentSlide);
			if (the_menu==null) return ;

			ArrayList slideshows = the_menu.GetSlideshowsSelectableFromMenu();
			if (slideshows.Count>0)
			{
				CSlideShow s2 = slideshows[0] as CSlideShow;
				CSlideShowManager.Instance.SetCurrentSlideShow(s2);
                
                // PHOTOCRUZ
                if (this.mMainWindow == null)
                {
                    CSlideShowManager.Instance.ReCalcTrackerBar();
                }

			}
		}

        //*******************************************************************
        private bool ShouldShowMenus()
        {
            if (mMainWindow==null) 
            {
                return true;
            }

            //
            // If creating a video don't show menus
            //
            if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.MP4)
            {
                return false;
            }
            //
            // Also don't show menus if current seleted slideshow is the pre menu slideshow
            //
            CSlideShow ss = mMainWindow.GetSlideShowManager().mCurrentSlideShow;
            if (ss != null && CGlobals.mCurrentProject.PreMenuSlideshow == ss)
            {
                return false;
            }

            return true;

        }

		//*******************************************************************
		public void MyMouseDown(object sender, System.Windows.Forms.MouseEventArgs e1)
		{
            if (m_enMM == MOUSE_MODE.MM_MENU && ShouldShowMenus() == false)
            {
                return;
            }

			if (CSlideShowManager.Instance.IsPlaying()==true ||
                IsBordersTabMode() ||
                IsPreDefinedSlideDesignMode() ||
                IsSelectFiltersMode() ||
                IsSelectBackgroundMode() )
			{
                mMouseDown = false;
				return ;
			}

			Point e = ClientPointToImagePoint(new Point(e1.X, e1.Y));

            if (mMouseDown == false)
            {
                mMouseDown = true;
                mMouseDownTime = DateTime.Now;
            }

			MainPaintBoxGainFocus();

			if (e1.Clicks>1)
			{
				MyDoubleClick(sender, e1);
				return ;
			}

			switch(m_enMM)
			{
				case MOUSE_MODE.MM_MENU:
				{
					this.ResetSelectedNode();
					if(FindSelectedRect(new Point(e.X, e.Y), SelectedNode))
					{
						// is text field we can edit

                        if (mDisableMenuTextEditing == false)
                        {
                            ITextDecorationContainer itdc = SelectedNode.mDecor as ITextDecorationContainer;
                            if (itdc != null)
                            {
                                this.AddTextBox(e.X, e.Y);
                                return;
                            }
                        }

						CMenuSlideshowButton b = SelectedNode.mDecor as CMenuSlideshowButton;
						if (b!=null)
						{
                            if (this.mMainWindow != null)
                            {
                                this.mMainWindow.SelectedSlideShow(b.GetInnerImageStringId());
                            }
                            else
                            {
                                // photocruz
                                CSlideShowManager.Instance.mCurrentSlideShow = CGlobals.mCurrentProject.GetSlideshow(b.GetInnerImageStringId());
                                CSlideShowManager.Instance.ReCalcTrackerBar();
                               // CSlideShowManager.Instance.RebuildAllLabelTimes();

                            }

							RePaint();
							return;
						}

                        CMenuLinkButton b2 = SelectedNode.mDecor as CMenuLinkButton;
						if (b2!=null)
						{
                            CMainMenu link_menu = CGlobals.mCurrentProject.GetMenu(b2.MenuLinkID);

                            this.SelectMenuAndSlideshow(link_menu, null);

							return;
						}


						this.ResetSelectedNode(); 
					}

			//		ptStart = new Point(e.X, e.Y);

					// if user straignt away lift the button without moving it, then 
					// DrawingRect and TrackerImage must have some values otherwise it will crash.
			//		DrawingRect = new Rectangle(ptStart.X, ptStart.Y, 10, 10);
			//		TrackerImage = (Bitmap)this.mPB.Image.Clone();
					break;
				}
				case MOUSE_MODE.MM_NONE:
				case MOUSE_MODE.MM_OBJ_SELECTED:
				{
					if(m_enMM == MOUSE_MODE.MM_OBJ_SELECTED && 
						(mPB.Cursor == Cursors.SizeNS || 
						 mPB.Cursor == Cursors.SizeWE ||
						 mPB.Cursor == Cursors.SizeNESW || 
						 mPB.Cursor == Cursors.SizeNWSE ||
                         mPB.Cursor == CRectTracker.RotateCursor))
					{  
						RePaint();
						return; // do nothing, it requires resizing of the selected object.
					}

					ptStart = new Point(e.X, e.Y);

                    bool alreadySelected = false;

                    // If we selected node from combo and mouse down does contain selecte node, then continue
                    if (mNodeSetBySelectCombo == true && SelectedNode.mDecor !=null)                 
                    {
                        alreadySelected = IsPointInsideDecor(ptStart, SelectedNode.mDecor);
                    }

                    if (alreadySelected == false)
                    {
                        if (FindSelectedRect(ptStart, SelectedNode))
                        {	
                            if (SelectedNode != null)
                            {                  
                                RePaint();                               
                            }
                            else
                            {
                                this.ResetSelectedNode();
                            }
                        }
                        else
                        {	// if nothing is selected then only draw the main image agian.
                            //	DrawImgOnForm(this.mPB.Image);
                            this.ResetSelectedNode();
                            RePaint();
                            m_enMM = MOUSE_MODE.MM_NONE;
                        }

                       
                    }
                    break;
				}   
			}
            // reset if the node was last selected from combo
            mNodeSetBySelectCombo = false;				
		}

        //*******************************************************************
        //
        // This method will select a slideshow and change the menu if required
        //
        // Either menu and/or slideshow must be set. 
        // If menu not set, it will find the menu for you and select the slideshow
        // if slideshow not set then it will select first slideshow in provided menu
        //
        public void SelectMenuAndSlideshow(CMainMenu menu, CSlideShow slideshow)
        {
            if (slideshow != null && menu == null)
            {
                menu = CGlobals.mCurrentProject.GetMenuWhichContainsSlideshow(slideshow);
            }

            if (menu != null)
            {
                bool noRepaint = mNoRepaint;
                mNoRepaint = true;
                try
                {
                    SetCurrentSlide(menu.BackgroundSlide);
                }
                finally
                {
                    mNoRepaint = noRepaint;
                }

                if (this.mMainWindow != null)
                {
                    this.mMainWindow.GetDiskMenuManager().DoHideUnHide();
                }

                if (slideshow == null)
                {
                    SelectFirstSlideshowInCurrentMenu();
                }
                else
                {
                    this.mMainWindow.SelectedSlideShow(slideshow.Name);
                }
            }

            RePaint();
            if (mMainWindow != null)
            {
                mMainWindow.GetDiskMenuManager().RebuildLabels();
            }
        }

		//*******************************************************************
		private RectangleF ToRatioRectF(RectangleF val,int w, int h)
		{
			val.X/=(float)w;
			val.Width /= (float)w;
			val.Y/=(float)h;
			val.Height/=(float)h;

			return val;
		}


        //*******************************************************************
        private void UpdateDecorXYWidthHeigthTextBoxs(CDecoration decor)
        {
            float x = decor.CoverageArea.X * 100;
            x = CGlobals.ClampF(x, -999, 999);
            float y = decor.CoverageArea.Y * 100;
            y = CGlobals.ClampF(y, -999, 999);
            float width = decor.CoverageArea.Width * 100;
            width = CGlobals.ClampF(width, -999, 999);
            float height = decor.CoverageArea.Height * 100;
            height = CGlobals.ClampF(height, -999, 999);

            mPositionTop.Value = (decimal)y;
            mPositionLeft.Value = (decimal)x;
            mPositionWidth.Value = (decimal)width;
            mPositionHeight.Value = (decimal)height;

            CImageDecoration imageDecor = decor as CImageDecoration;
            if (imageDecor != null)
            {
                int rot = (int)(imageDecor.Rotation);
                rot = CGlobals.Clamp(rot, -180, 180);
                if (rot != mRotationTrackbar.Value)
                {
                    mRotationTrackbar.Value = rot;
                }
            }
        }

        private RectangleF rotateCentre(RectangleF oldrc, RectangleF newrc, float rotation)
        {
            float oldcentrex = oldrc.X + (oldrc.Width / 2.0f);
            float oldcentrey = oldrc.Y + (oldrc.Height / 2.0f);

            float newcentrex = newrc.X + (newrc.Width / 2.0f);
            float newcentrey = newrc.Y + (newrc.Height / 2.0f);

            float xoffset = newcentrex - oldcentrex;
            float yoffset = newcentrey - oldcentrey;

            Matrix mat= new Matrix();
            mat.Rotate(-rotation);

            PointF[] points = new PointF[1];
            points[0] = new PointF(xoffset, yoffset);

            mat.TransformVectors(points);

            newcentrex = oldcentrex + points[0].X;
            newcentrey = oldcentrey + points[0].Y;

            RectangleF newnewrc = new RectangleF(newcentrex - (newrc.Width / 2), newcentrey - (newrc.Height / 2), newrc.Width, newrc.Height);
            return newnewrc;
        }


        
      private RectangleF AdjustTopLeft(PointF newTopLeft, RectangleF oldrc)
      {
          float oldcentrex = oldrc.X + (oldrc.Width / 2.0f);
          float oldcentrey = oldrc.Y + (oldrc.Height / 2.0f);

          float width = 2 * (oldcentrex - newTopLeft.X);
          float height = 2 * (oldcentrey - newTopLeft.Y);

          RectangleF newnewrc = new RectangleF(oldcentrex - (width / 2), oldcentrey - (height / 2), width, height);

          float newcentrex = newnewrc.X + (newnewrc.Width / 2.0f);
          float newcentrey = newnewrc.Y + (newnewrc.Height / 2.0f);


          return newnewrc;
      }
       


        /*
        private RectangleF AdjustTopLeft(PointF newTopLeft, RectangleF oldrc)
        {
            float oldcentrex = oldrc.X + (oldrc.Width / 2.0f);
            float oldcentrey = oldrc.Y + (oldrc.Height / 2.0f);

            float width = 2 * (oldcentrex - newTopLeft.X);
            float height = 2 * (oldcentrey - newTopLeft.Y);



            RectangleF newnewrc = new RectangleF(oldcentrex - (width / 2), oldcentrey - (height / 2), width, height);

            float newcentrex = newnewrc.X + (newnewrc.Width / 2.0f);
            float newcentrey = newnewrc.Y + (newnewrc.Height / 2.0f);


            return newnewrc;
        }
         */

        

		//*******************************************************************
		private void MyMouseMove(object sender, System.Windows.Forms.MouseEventArgs e1)
		{
            if (m_enMM == MOUSE_MODE.MM_MENU && ShouldShowMenus() == false)
            {
                return;
            }

            float rotation = 0;
            if (SelectedNode !=null)
            {
                rotation = SelectedNode.rotation;
            }

			try
			{
				Point e = this.ClientPointToImagePoint(new Point(e1.X,e1.Y));

				if(mMouseDown)
				{
                    DateTime now = DateTime.Now;
                    TimeSpan span = now- mMouseDownTime ;

                    if (span < mTimeSpanUntilAllowMove)
                    {
                        return;
                    }
					
					if(m_enMM == MOUSE_MODE.MM_MENU)
					{	// draw the rubberband tracker of the box.
						// eack time copy of main image is made and on it 
				
					}
					if(this.m_enRBorder != RESIZE_BORDER.RB_NONE)
					{	// resize the box
						//TrackerImage.Dispose();
						//	TrackerImage = (Bitmap)this.mPB.Image.Clone();
						//		Graphics g = Graphics.FromImage(TrackerImage);

						RectangleF before_rc = SelectedNode.rc;

							
						int w = mPB.Image.Width;
						int h = mPB.Image.Height;

						// this is a hack if not a text decoration ratio is that of the image

						// i.e this only ever gets used in pan zoom!!

						float ratio = ((float)w)/((float)h);

						// if selected node is a animated decor get ratio from that
						// this hack is here because we get a slight drift which is ok for
						// text but NOT ok for pan zoom stuff

                     
						if (this.SelectedNode.mDecor!=null)
						{
                            ITextDecorationContainer itdc = SelectedNode.mDecor as ITextDecorationContainer;
							if (this.SelectedNode.mDecor as CAnimatedDecoration!=null || itdc !=null)
							{
								ratio=SelectedNode.rc.Width/SelectedNode.rc.Height;
							}
						}

						bool corner=true;

						switch(this.m_enRBorder)
						{
                            case RESIZE_BORDER.RB_TOP:
                            {
                                Point tcoffset = mTracker.TopCenterOffset;
                                Point tce = new Point(e.X + tcoffset.X, e.Y + tcoffset.Y);

                                SelectedNode.rc.Height =
                                    SelectedNode.rc.Y + SelectedNode.rc.Height - tce.Y;
                                SelectedNode.rc.Y = tce.Y;
                                corner = false;
                                break;
                             }
                            case RESIZE_BORDER.RB_TOPLEFT:
                            {
                                Point tloffset = mTracker.TopLeftOffset;
                                Point tle = new Point(e.X + tloffset.X, e.Y + tloffset.Y);

                                SelectedNode.rc.Height =
                                    SelectedNode.rc.Y + SelectedNode.rc.Height - tle.Y;
                                SelectedNode.rc.Y = tle.Y;
                                SelectedNode.rc.Width =
                                    SelectedNode.rc.X + SelectedNode.rc.Width - tle.X;
                                SelectedNode.rc.X = tle.X;

                                break;
                            }
                            case RESIZE_BORDER.RB_TOPRIGHT:
                            {

                                Point troffset = mTracker.TopRightOffset;
                                Point tre = new Point(e.X + troffset.X, e.Y + troffset.Y);

                                SelectedNode.rc.Height =
                                    SelectedNode.rc.Y + SelectedNode.rc.Height - tre.Y;
                                SelectedNode.rc.Y = tre.Y;
                                SelectedNode.rc.Width = tre.X - SelectedNode.rc.X;
                                break;
                            }
                            case RESIZE_BORDER.RB_RIGHT:
                            {
                                Point croffset = mTracker.CenterRightOffset;
                                Point cre = new Point(e.X + croffset.X, e.Y + croffset.Y);

                                SelectedNode.rc.Width = cre.X - SelectedNode.rc.X;
                                corner = false;
                                break;
                            }
                            case RESIZE_BORDER.RB_BOTTOM:
                            {
                                Point bcoffset = mTracker.BottomCenterOffset;
                                Point bce = new Point(e.X + bcoffset.X, e.Y + bcoffset.Y);

                                SelectedNode.rc.Height = bce.Y - SelectedNode.rc.Y;
                                corner = false;
                                break;
                            }
                            case RESIZE_BORDER.RB_BOTTOMLEFT:
                            {
                                Point bloffset = mTracker.BottomLeftOffset;
                                Point ble = new Point(e.X + bloffset.X, e.Y + bloffset.Y);

                                SelectedNode.rc.Height = ble.Y - SelectedNode.rc.Y;
                                SelectedNode.rc.Width =
                                    SelectedNode.rc.X + SelectedNode.rc.Width - ble.X;
                                SelectedNode.rc.X = ble.X;
                                break;
                            }
                            case RESIZE_BORDER.RB_BOTTOMRIGHT:
                            {
                                Point broffset = mTracker.BottomRightOffset;
                                Point bre = new Point(e.X + broffset.X, e.Y + broffset.Y);

                                SelectedNode.rc.Height = bre.Y - SelectedNode.rc.Y;
                                SelectedNode.rc.Width = bre.X - SelectedNode.rc.X;
                                break;
                            }
                            case RESIZE_BORDER.RB_LEFT:
                            {
                                Point cloffset = mTracker.CenterLeftOffset;
                                Point cle = new Point(e.X + cloffset.X, e.Y + cloffset.Y);

                                SelectedNode.rc.Width =
                                    SelectedNode.rc.X + SelectedNode.rc.Width - cle.X;
                                SelectedNode.rc.X = cle.X;
                                corner = false;
                                break;
                            }

                            case RESIZE_BORDER.RB_ROTATE:
                            {

                                float centrex = SelectedNode.rc.X + (SelectedNode.rc.Width / 2.0f);
                                float centrey = SelectedNode.rc.Y + (SelectedNode.rc.Height / 2.0f);

                                float diffx = e.X - centrex;
                                float diffy = e.Y - centrey;

                                SelectedNode.rotation = (float)(57.2957795 * Math.Atan2(diffx, -diffy));

                                break;
                            }
						}
						float x_diff = before_rc.Width - SelectedNode.rc.Width;
						float y_diff = before_rc.Height - SelectedNode.rc.Height;


						//		Console.WriteLine("xdiff ="+x_diff+"  y_diff="+y_diff);

						if (m_enRBorder == RESIZE_BORDER.RB_LEFT || m_enRBorder == RESIZE_BORDER.RB_BOTTOMLEFT || m_enRBorder == RESIZE_BORDER.RB_TOPLEFT)
						{
							if (SelectedNode.rc.Width <10) { SelectedNode.rc.X = before_rc.X+ before_rc.Width -10; SelectedNode.rc.Width = 10 ; }
						}
						else
						{
							if (SelectedNode.rc.Width <10) SelectedNode.rc.Width=10;
						}

						if (m_enRBorder == RESIZE_BORDER.RB_TOPLEFT || m_enRBorder == RESIZE_BORDER.RB_TOP || m_enRBorder == RESIZE_BORDER.RB_TOPRIGHT)
						{
							if (SelectedNode.rc.Height <10) { SelectedNode.rc.Y = before_rc.Y+ before_rc.Height -10; SelectedNode.rc.Height = 10 ; }
						}
						else
						{
							if (SelectedNode.rc.Height <10) SelectedNode.rc.Height=10;
						}
			

						//	Console.WriteLine("rw ="+SelectedNode.rc.Width+" rh="+SelectedNode.rc.Height+" xdiff="+x_diff+" ydiff ="+y_diff);


						float abs_xdiff = x_diff; 
						if (abs_xdiff < 0) abs_xdiff=-abs_xdiff;

						float abs_ydiff = y_diff; 
						if (abs_ydiff < 0) abs_ydiff=-abs_ydiff;

					
					
						// this just works ok....  i fiddle until it worked
						if (SelectedNode.keep_aspect==true)
						{
							if (corner==false)
							{
								if (abs_xdiff < (abs_ydiff*ratio))
								{
									SelectedNode.rc.Width = SelectedNode.rc.Height * (ratio);
								}
								else
								{
									SelectedNode.rc.Height= SelectedNode.rc.Width / ratio;
								}
							}
							else if (this.m_enRBorder == RESIZE_BORDER.RB_BOTTOMRIGHT)
							{
								if (x_diff > (y_diff*ratio))
								{
									SelectedNode.rc.Width = SelectedNode.rc.Height * (ratio);
								}
								else
								{
									SelectedNode.rc.Height= SelectedNode.rc.Width / ratio;
								}
							}
							else 
							{
								if (x_diff > (y_diff*ratio))
								{
									SelectedNode.rc.Width = SelectedNode.rc.Height * (ratio);
								}
								else
								{
									SelectedNode.rc.Height= SelectedNode.rc.Width / ratio;
								}

								if (this.m_enRBorder == RESIZE_BORDER.RB_TOPLEFT ||
									this.m_enRBorder == RESIZE_BORDER.RB_BOTTOMLEFT)
								{
									SelectedNode.rc.X = before_rc.X+before_rc.Width-SelectedNode.rc.Width;

								}

								if (this.m_enRBorder == RESIZE_BORDER.RB_TOPLEFT ||
									this.m_enRBorder == RESIZE_BORDER.RB_TOPRIGHT)
								{
									SelectedNode.rc.Y = before_rc.Y+before_rc.Height-SelectedNode.rc.Height;
								}

							}
						}

						// clamp to bound of screen if in pan/zoom mode

						if (SelectedNode.mSelected_pan_start==true ||
							SelectedNode.mSelected_pan_end==true)
						{
							for (int ii=1;ii<2;ii++)
							{

								if (SelectedNode.rc.Width > w ||
									SelectedNode.rc.Height > h)
								{
									SelectedNode.rc =new RectangleF(0,0,w,h);
									break;
								}
							

								if (SelectedNode.rc.X <0)
								{
									SelectedNode.rc.Width += -SelectedNode.rc.X;
									SelectedNode.rc.X =0;
								}

								if (SelectedNode.rc.Y <0)
								{
									SelectedNode.rc.Height += -SelectedNode.rc.Y;
									SelectedNode.rc.Y =0;
								}

								
								if (SelectedNode.rc.X+SelectedNode.rc.Width > w)
								{
									SelectedNode.rc.X -= (SelectedNode.rc.X+SelectedNode.rc.Width)-w;
								}

								if (SelectedNode.rc.Y+SelectedNode.rc.Height > h)
								{
									SelectedNode.rc.Y -= (SelectedNode.rc.Y+SelectedNode.rc.Height)-h;
								}
							}
						}

						if (SelectedNode.mDecor!=null)
						{
							SelectedNode.mDecor.CoverageArea = ToRatioRectF(SelectedNode.rc,w,h);

                            CImageDecoration id = SelectedNode.mDecor as CImageDecoration;
                            if (id != null)
                            {
                                id.Rotation = SelectedNode.rotation;
                            }

                            UpdateDecorXYWidthHeigthTextBoxs(SelectedNode.mDecor);

                            ITextDecorationContainer itdc = SelectedNode.mDecor as ITextDecorationContainer;
                            if (itdc != null)
							{
                                CTextDecoration td = itdc.TextDecoration;
								mOnlyChangeFontSizeText = true;
								SetTextSizeComboTextToTextDecoration(td);
								mOnlyChangeFontSizeText = false;

							}
						}

						if (SelectedNode.mSelected_pan_start==true)
						{
							this.mCurrentSlide.PanZoom.StartArea = ToRatioRectF(SelectedNode.rc,w,h);
                            this.mCurrentSlide.PanZoom.StartRotation = SelectedNode.rotation;
						}

						if (SelectedNode.mSelected_pan_end==true)
						{
							this.mCurrentSlide.PanZoom.EndArea = ToRatioRectF(SelectedNode.rc,w,h);
                            this.mCurrentSlide.PanZoom.EndRotation = SelectedNode.rotation;
						}

                        if (this.IsPanZoomEditMode() == true)
                        {
                            SetClipPanControls(true);
                        }

						// while resizing the box keep drawing the ghost rect and trackers.
						//	CRectTracker tracker = new CRectTracker();
						//	tracker.DrawSelectionGhostRect(SelectedNode.rc, (Bitmap)TrackerImage);
						//	tracker.DrawSelectionTrackers(SelectedNode.rc, (Bitmap)TrackerImage);
						//	DrawImgOnForm(TrackerImage);
						
						//	g.Dispose();

						this.SelectedNode.mChangedSinceSelected=true;
                        RePaint();


					} 
					else if(m_enMM == MOUSE_MODE.MM_OBJ_SELECTED)
					{
						//use TrackerImage to move the selected the box with SelectedNode"
						// gives the current node coordinates from the list.
						//	TrackerImage.Dispose();
						//	TrackerImage = (Bitmap)this.mPB.Image.Clone();
						//	Graphics g = Graphics.FromImage(TrackerImage);


                        // Move event occured, but we've not actually moved
                        if (e.X == ptStart.X &&
                            e.Y == ptStart.Y)
                        {
                            return;
                        }
						
						int w = mPB.Image.Width;
						int h = mPB.Image.Height;

						SelectedNode.rc.X += e.X - ptStart.X; 
						SelectedNode.rc.Y += e.Y - ptStart.Y;

	 
						//if ((e.X >=0 && e.X <= w) || (SelectedNode.rc.X > 0 && SelectedNode.rc.X+SelectedNode.rc.Width < w)) SelectedNode.rc.X += e.X - ptStart.X; 
						//if ((e.Y >=0 && e.Y <= h) || (SelectedNode.rc.Y > 0 && SelectedNode.rc.Y+SelectedNode.rc.Height< h)) SelectedNode.rc.Y += e.Y - ptStart.Y; 
				
						//	Console.WriteLine("e.x = "+e.X);

						if (this.SelectedNode.mSelected_pan_end==false &&
							this.SelectedNode.mSelected_pan_start==false)
						{
							// allow user to move object off screen but at least 10 pixels must in screen
							if (SelectedNode.rc.X+SelectedNode.rc.Width > (w+SelectedNode.rc.Width-10)) SelectedNode.rc.X = w -10;
							if (SelectedNode.rc.Y+SelectedNode.rc.Height > (h+SelectedNode.rc.Height-10)) SelectedNode.rc.Y = h - 10;
							if (SelectedNode.rc.X < (-SelectedNode.rc.Width+10)) SelectedNode.rc.X = -SelectedNode.rc.Width+10;
							if (SelectedNode.rc.Y < (-SelectedNode.rc.Height+10)) SelectedNode.rc.Y = -SelectedNode.rc.Height+10;
						}
						else
						{
							// no part of the object is allowed offscreen
							if (SelectedNode.rc.X+SelectedNode.rc.Width > w) SelectedNode.rc.X = w - SelectedNode.rc.Width;
							if (SelectedNode.rc.Y+SelectedNode.rc.Height > h) SelectedNode.rc.Y = h - SelectedNode.rc.Height;
							if (SelectedNode.rc.X < 0) SelectedNode.rc.X = 0;
							if (SelectedNode.rc.Y <0) SelectedNode.rc.Y = 0;
						}


						if (SelectedNode.mDecor!=null)
						{
							SelectedNode.mDecor.CoverageArea = ToRatioRectF(SelectedNode.rc,w,h);

                            UpdateDecorXYWidthHeigthTextBoxs(SelectedNode.mDecor);
						}

						if (SelectedNode.mSelected_pan_start==true)
						{
							this.mCurrentSlide.PanZoom.StartArea = ToRatioRectF(SelectedNode.rc,w,h);
						}

						if (SelectedNode.mSelected_pan_end==true)
						{
							this.mCurrentSlide.PanZoom.EndArea = ToRatioRectF(SelectedNode.rc,w,h);
						}

                        if (this.IsPanZoomEditMode() == true)
                        {
                            SetClipPanControls(true);
                        }
	  
						ptStart = new Point(e.X, e.Y);   // required.

						// while moving the box keep drawing the ghost rect and trackers.
						//		CRectTracker tracker = new CRectTracker();
						//		tracker.DrawSelectionGhostRect(SelectedNode.rc, (Bitmap)TrackerImage);
						//		tracker.DrawSelectionTrackers(SelectedNode.rc, (Bitmap)TrackerImage);
						//		DrawImgOnForm(TrackerImage);
						 
						//	this.RePaint();
						//		g.Dispose();

						this.SelectedNode.mChangedSinceSelected=true;
						RePaint();
					}
				}
			
				else // mouse is not down but only moving.
				{
					if(m_enMM == MOUSE_MODE.MM_OBJ_SELECTED)
					{	
						if (this.SelectedNode!=null)
						{
						
							//RESIZE_BORDER enRB = RESIZE_BORDER.RB_NONE;
							int nResult = 0;
							mPB.Cursor = mTracker.CursorCheck(CGlobals.ToRectangle(SelectedNode.rc), new Point(e.X, e.Y), ref nResult);
							this.m_enRBorder = (RESIZE_BORDER)nResult;
						}
						else
						{
							CDebugLog.GetInstance().Error("No selected node when in MM_OBJ_SELECTED state");
						}
					}

					if(m_enMM == MOUSE_MODE.MM_MENU)
					{
						if (mHoverPanelBox!=null)
						{
							//		mPB.Controls.Remove(mHoverPanelBox);
							//		mHoverPanelBox=null;
						}

						mPB.Cursor = Cursors.Default;

						Node test_node = new Node();
						if(FindSelectedRect(new Point(e.X, e.Y), test_node ,false))
						{
                            ITextDecorationContainer itdc = this.SelectedNode.mDecor as ITextDecorationContainer;
                            if (itdc != null &&
                                itdc.TextDecoration.VCDNumber == false && 
                                mDisableMenuTextEditing==false)
							{
								mPB.Cursor = Cursors.IBeam;
							}

							CMenuSlideshowButton b = SelectedNode.mDecor as CMenuSlideshowButton;
							if (b!=null)
							{
								mPB.Cursor = Cursors.Hand;
							}


                            CMenuLinkButton b2 = SelectedNode.mDecor as CMenuLinkButton;
							if (b2!=null)
							{
								mPB.Cursor = Cursors.Hand;
							}
						}
					}

					
				}
			}
			catch (Exception e4)
			{
                CDebugLog.GetInstance().Error("Exception thrown in DecorationManager::MyMouseMove: "+e4.Message);
			}
		}


		//*******************************************************************
		// gets a rectangle which represent the actual pixels it currently
		// used on our picture box image
		private Rectangle GetImageRectangleOfRectF(RectangleF d)
		{
            float cw = this.mPB.Size.Width;
            float ch = this.mPB.Size.Height;

            if (this.mPB.Image != null)
            {
                cw = this.mPB.Image.Width;
                ch = this.mPB.Image.Height;
            }

            float x = cw * d.X;
            float y = ch * d.Y;
            float width = cw * d.Width;
            float height = ch * d.Height;

            Rectangle r1 = new Rectangle((int)x, (int)y, (int)width, (int)height);
            
            return r1;
		}

		//*******************************************************************
		// gets a rectangle which represent the actual pixels it currently
		// used on the picture box client window
		
		private Rectangle GetPhysicalRectangleOfRectF(RectangleF d)
		{
			float cw = this.mPB.ClientRectangle.Width;
			float ch = this.mPB.ClientRectangle.Height;

			float x = cw*d.X;
			float y = ch*d.Y;
			float width = cw*d.Width ;
			float height = ch*d.Height ;

			Rectangle r1 = new Rectangle((int)x,(int)y,(int)width,(int)height);

			return r1;
		}
		


		//*******************************************************************
		public Point ClientPointToImagePoint(Point e)
		{
            return e;

            // ### TODO SRG NOT NEEDED ?
            /*
			Point rp = new Point();
			rp.X = (int) (((float)e.X) *  (((float)mPB.Image.Width)) / (((float)mPB.ClientRectangle.Width) ));
			rp.Y = (int) (((float)e.Y) *  ( ((float)mPB.Image.Height) /((float)mPB.ClientRectangle.Height)) );

			return rp;
             */
		}

		
		//*******************************************************************
		public Point ImagePointToClientPoint(Point e)
		{
            return e;

            // ### TODO SRG NOT NEEDED ?
            /*
			Point rp = new Point();
			try
			{
				
				rp.X = (int) (((float)e.X) /  (((float)mPB.Image.Width)) / (((float)mPB.ClientRectangle.Width) ));
				rp.Y = (int) (((float)e.Y) /  ( ((float)mPB.Image.Height) /((float)mPB.ClientRectangle.Height)) );
			}
			catch (Exception e2)
			{
				ManagedCore.CDebugLog.GetInstance().Error("Exception occured in ImagePointToClientPoint!");
			}

			return rp;
             */
		}

		//*******************************************************************
		private bool FindSelectedRect(Point pt, Node n)
		{   
			return FindSelectedRect(pt,n,true);
		}

        //*******************************************************************
        private bool IsPointInsideDecor(Point pt, CDecoration d)
        {
            float rotation = 0;
            CImageDecoration imageDecor = d as CImageDecoration;
            if (imageDecor != null)
            {
                rotation = imageDecor.Rotation;
            }

            Rectangle r1 = GetImageRectangleOfRectF(d.CoverageArea);

            return RotatedRectangleContainsPoint(r1, rotation, pt);
        }

		//*******************************************************************
		private bool FindSelectedRect(Point pt, Node n, bool from_button_down)
		{   
			if (mCurrentSlide==null) 
			{
				this.ResetSelectedNode();
				return false ;
			}

            bool lockedApsect = false;
            if (mMainWindow != null)
            {
                lockedApsect = mMainWindow.GetClipArtLockAspectCheckBox().Checked;
            }

			ArrayList cand_nodes = new ArrayList();
		
			if (IsPanZoomEditMode()==false)
			{
                ArrayList decs = mCurrentSlide.GetAllAndSubDecorations();

                foreach (CDecoration d in decs)
				{
                    // is the a menu button?
                    if (d is CMenuButton == false)
                    {
                        // if not make sure we can select/change it
                        if (d.IsBackgroundDecoration() ||
                            d.IsBorderDecoration() ||
                            d.IsFilter() ||
                            (d is CImageDecoration == false))
                        {
                            continue;
                        }
                    }

                    bool containsPoint = IsPointInsideDecor(pt, d);

                    if (containsPoint)
					{
                        Node n1 = CreateNodeFromDecoration(d, lockedApsect);
                        cand_nodes.Add(n1);
					}
				}
			}
			else if (mCurrentSlide.UsePanZoom == true)
			{
				Rectangle r2 = GetImageRectangleOfRectF(this.mCurrentSlide.PanZoom.StartArea);
                float rotation = this.mCurrentSlide.PanZoom.StartRotation;

                bool containsPoint = RotatedRectangleContainsPoint(r2, rotation, pt);

                if (containsPoint)
				{
					Node n1 = new Node();
					n1.draw_dotted = false;
					n1.draw_enlarge_inside = true;
					n1.keep_aspect = true;
					n1.rc = r2;
					n1.mDecor = null;
					n1.mSelected_pan_start = true;
                    n1.rotation = mCurrentSlide.PanZoom.StartRotation;
					cand_nodes.Add(n1);
				}

				r2 = GetImageRectangleOfRectF(this.mCurrentSlide.PanZoom.EndArea);
                rotation = this.mCurrentSlide.PanZoom.EndRotation;

                containsPoint = RotatedRectangleContainsPoint(r2, rotation, pt);

				if (containsPoint)
				{
					Node n1 = new Node();
					n1.draw_dotted = false;
					n1.draw_enlarge_inside = true;
					n1.keep_aspect = true;
					n1.rc = r2;
					n1.mDecor = null;
					n1.mSelected_pan_end = true;
                    n1.rotation = mCurrentSlide.PanZoom.EndRotation;
					cand_nodes.Add(n1);
				}
			}

			
			// return smallest rectangle

			int smallest_vol = 10000000;

			if (cand_nodes.Count > 0)
			{
				int j;

                Node smallestNode = null;

				for (j=0;j<cand_nodes.Count;j++)
				{
					Node rt = (Node) cand_nodes[j];
					int vol = (int)( rt.rc.Width * rt.rc.Height);
					if (vol < smallest_vol)
					{
                        smallestNode = rt;
                        smallest_vol = vol;
					}
				}

                if (smallestNode != null)
                {
                    SetToSelectedNode(smallestNode, from_button_down);  
                }

				return true;
			}
			
			this.ResetSelectedNode();
		
			return false;
		}

        //*******************************************************************
        private bool RotatedRectangleContainsPoint(Rectangle rec, float rotation, Point p)
        {
            float centerRotX = (rec.Width / 2) + rec.X;
            float centerRotY = (rec.Height / 2) + rec.Y;

            float offsetX = p.X - centerRotX;
            float offsetY = p.Y - centerRotY;

            PointF[] points = new PointF[1];

            using (Matrix m = new Matrix())
            {
                points[0] = new PointF(offsetX, offsetY);
                m.Rotate(-rotation);
                m.TransformPoints(points);
            }

            points[0].X += centerRotX;
            points[0].Y += centerRotY;

            RectangleF r1 = new RectangleF(rec.X, rec.Y, rec.Width, rec.Height);
            
            return r1.Contains(points[0]);
        }

        //*******************************************************************
        private Node CreateNodeFromDecoration(CDecoration d, bool lockedApsect)
        {
            Rectangle r1 = GetImageRectangleOfRectF(d.CoverageArea);
				
            Node n1 = new Node();
            n1.draw_dotted = true;
            n1.draw_enlarge_inside = false;
            n1.keep_aspect = false;
            n1.rc = r1;

            CImageDecoration id = d as CImageDecoration;
            if (id != null)
            {
                n1.rotation = id.Rotation;
            }

            n1.mDecor = d;
       

            if (d as ITextDecorationContainer != null ||
                d as CMenuSlideshowButton != null ||
                lockedApsect == true)
            {
                n1.keep_aspect = true;
            }
            n1.mOriginalCoverageArea = d.CoverageArea;
            return n1;

        }

        //*******************************************************************
        private void SetToSelectedNode(Node node, bool updateGuiTabsAndControls)
        {
            SelectedNode = node;

            if (updateGuiTabsAndControls == true && m_enMM != MOUSE_MODE.MM_MENU)
            {
                int index = 0;
                foreach (CDecoration d in mSelectedDecorations)
                {
                    if (d == SelectedNode.mDecor)
                    {
                        mSelectedDecorationsCombo.SelectedIndex = index;
                        break;
                    }
                    index++;
                }

                if (SelectedNode.mDecor is CClipArtDecoration ||
                    SelectedNode.mDecor is CVideoDecoration)
                {
                    mMainWindow.GetDecorationTabOptions().SelectedTab = mMainWindow.GetClipArtTab();
                }
                else if (SelectedNode.mDecor is ITextDecorationContainer)
                {
                    mMainWindow.GetDecorationTabOptions().SelectedTab = mMainWindow.GetAddTextTab();
                }

                ApplyHideUnhideState();

                if (node != null)
                {
                    m_enMM = MOUSE_MODE.MM_OBJ_SELECTED; // change the status
                    mTracker = new CRectTracker(SelectedNode.draw_dotted, SelectedNode.draw_enlarge_inside);

                    // can't rotate menubuttons
                    if (SelectedNode.mDecor is CMenuButton)
                    {
                        mTracker.ShowRotation = false;
                    }
                    else
                    {
                        mTracker.ShowRotation = true;
                    }

                    mPB.Cursor = Cursors.Hand;

                    if (node.mDecor != null)
                    {
                        UpdateDecorXYWidthHeigthTextBoxs(node.mDecor);
                    }
                }
            }
        }

        //*******************************************************************
		public CSlideShow CurrentSelectedSlideShow()
		{
			if (mCurrentSlide==null) return null;
			if (this.IsCurrentSlideAMenuSlide()==false) return null;

            CMainMenu menu = null;

            if (mMainWindow != null)
            {
                if (mMainWindow.mCurrentProject == null) return null;

                menu = mMainWindow.mCurrentProject.GetMenuWhichContainsSlide(this.mCurrentSlide);
                if (menu == null) return null;

                CSlideShow ss = mMainWindow.GetSlideShowManager().mCurrentSlideShow;
                return ss;

            }

            // used by photocruz
            if (CGlobals.mCurrentProject != null)
            {
                menu = CGlobals.mCurrentProject.GetMenuWhichContainsSlide(this.mCurrentSlide);
                if (menu == null) return null;

                CSlideShow ss = CSlideShowManager.Instance.mCurrentSlideShow;
                return ss;
                
            }

            return null;
		}


		//*******************************************************************
		// highlights current slideshow if we are displaying a menu
		public void HighlightCurrentSlideshow()
		{
            if (this.mMainWindow != null &&
                 (this.mMainWindow.GetShowSelectedSlideShowMenuItem().Checked == false ||
                   this.mMainWindow.mCurrentProject.GetAllProjectSlideshows(false).Count <=1 ||
				   mInMenuDecor==true) )
			{
				return ;
			}

            if (mCurrentSlide == null || mCurrentSlide.PartOfAMenu == false)
            {
                return;
            }

			CSlideShow ss = CurrentSelectedSlideShow();
			if (ss==null) return ;
			
			foreach (CDecoration d in this.mCurrentSlide.Decorations)
			{
				CMenuSlideshowButton b = d as CMenuSlideshowButton;

				if (b!=null)
				{
					if (b.GetInnerImageStringId() ==ss.Name && mPB.Image != null)
					{
                        Image image = mPB.Image;

                        List<CMenuButton> buttons = new List<CMenuButton>();
                        buttons.Add(b);

                        CMainMenu menu = CGlobals.mCurrentProject.GetMenuWhichContainsSlide(this.mCurrentSlide);
                        if (menu != null)
                        {
                            CMenuSubPictureRenderer renderer = new CMenuSubPictureRenderer();
                            renderer.Render(buttons, mPB.Image as Bitmap, menu);
                        }

						return ;
					}
				}
			}

			Console.WriteLine("Error: no menu button for slideshow "+ss.Name);
		
			return ;

		}

		//*******************************************************************
		private void RePaintCallback(object o, System.EventArgs e)
		{
			RePaint();
		}

     
        //*******************************************************************
        private void ToggleShowSelectedSlideshow(object o, System.EventArgs e)
        {
            if (this.mMainWindow.GetShowSelectedSlideShowMenuItem().Checked == true)
            {
                this.mMainWindow.GetShowSelectedSlideShowMenuItem().Checked = false;
            }
            else
            {
                this.mMainWindow.GetShowSelectedSlideShowMenuItem().Checked = true;
            }
            RePaint();
        }


		//*******************************************************************
		public void RemoveCurrentTextBox()
		{
			if (this.mCurrentTextBox==null) return ;

			AlphaBlendTextBox temp = this.mCurrentTextBox;
			this.mCurrentTextBox=null;
			mPB.Controls.Remove(temp);
            temp.Clean();
            GC.Collect();
		}

		//*******************************************************************
		public void TransferTextBoxToTextDecoration()
		{
            try
            {
                if (this.mCurrentTextBox == null)
                {
                    return;
                }

                lock (this)
                {
                    if (mCurrentTextBox.Text == "")
                    {
                        if (mCurrentSlide.ContainsDecoration(mCurrentTextBox.Decoration))
                        {
                            this.mCurrentSlide.RemoveDecoration(mCurrentTextBox.Decoration);
                        }
                        RemoveCurrentTextBox();

                    }
                    else
                    {
                        ITextDecorationContainer itdc = mCurrentTextBox.Decoration as ITextDecorationContainer;
                        if (itdc != null)
                        {
                            CTextDecoration td = itdc.TextDecoration;
                            td.RecalcCoverageAreaForFontSize(mCurrentTextBox.mStoredFontSize);

                            if (mCurrentTextBox.Decoration != td)
                            {
                                mCurrentTextBox.Decoration.CoverageArea = td.CoverageArea;
                            }
                        }

                        if (mCurrentSlide.ContainsDecoration(mCurrentTextBox.Decoration) == false)
                        {
                            this.mCurrentSlide.AddDecoration(mCurrentTextBox.Decoration);
                        }

                        RemoveCurrentTextBox();
                        this.CommitChangesOutsideDecorationsEditor();
                    }

                    // SRG HACK
                    if (this.IsCurrentSlideAMenuSlide() == true && mInMenuDecor == false)
                    {
                        //
                        // May have changed slideshow alias name, force update of slideshow list
                        //
                        mMainWindow.GetDiskMenuManager().UpdateSlideshowList(true);

                        m_enMM = MOUSE_MODE.MM_MENU;
                    }

                    RebuildSelectedDecorationsCombo();

                }
            }
            finally
            {
                mCurrentTextBox = null;
            }
		}

		//*******************************************************************
		public void AddTextButtonPushed()
		{
			if (mCurrentSlide==null) return ;

			TransferTextBoxToTextDecoration();

			int w = mPB.ClientRectangle.Width;
			int h = mPB.ClientRectangle.Height;

			this.ResetSelectedNode(); 

			AddTextBox(w/2-w/4,h/2-h/4);
		}


		//*******************************************************************
		// returns a reference to the current selected textdecoration 
		// else return null if no such decoration is being selected
		private CTextDecoration GetSelectedTextDecoration()
		{
			if (mCurrentSlide==null) return null ;
			if (this.mCurrentTextBox!=null)
			{
                CDebugLog.GetInstance().Warning("Edit TextBox non null in call to GetSelectedTextDecoration");
				return null ;
			} 
			if (this.SelectedNode!=null)
			{
                ITextDecorationContainer itdc = SelectedNode.mDecor as ITextDecorationContainer;
                if (itdc != null)
                {
                    return itdc.TextDecoration;
                }
			}
			return null ;
		}

		//*******************************************************************
		public void SelectLeftAllignment()
		{
			CTextDecoration d = GetSelectedTextDecoration();
			if (d!=null)
			{
				if (this.SelectedNode!=null) this.SelectedNode.mChangedSinceSelected=true;
                d.TextStyle.Format.Alignment = System.Drawing.StringAlignment.Near;
                d.InvalidateFont();
                RePaint();
                CommitChangesOutsideDecorationsEditor();
			}
		
		}

		//*******************************************************************
		public void SelectCentreAllignment()
		{
			CTextDecoration d = GetSelectedTextDecoration();
			if (d!=null)
			{
				if (this.SelectedNode!=null) this.SelectedNode.mChangedSinceSelected=true;
                d.TextStyle.Format.Alignment = System.Drawing.StringAlignment.Center;
                d.InvalidateFont();
                RePaint();
                CommitChangesOutsideDecorationsEditor();
			}	
		}


		//*******************************************************************
		public void SelectRightAllignment()
		{
			CTextDecoration d = GetSelectedTextDecoration();
			if (d!=null)
			{
				if (this.SelectedNode!=null) this.SelectedNode.mChangedSinceSelected=true;
                d.TextStyle.Format.Alignment = System.Drawing.StringAlignment.Far;
                d.InvalidateFont();
                RePaint();
                CommitChangesOutsideDecorationsEditor();
			}
			
		}

		//*******************************************************************
		public void SelectBackColorPushed()
		{
            DialogResult result = this.BackPanecolorDialog.ShowDialog();

            if (result == DialogResult.Cancel || result == DialogResult.Abort) return;

            Color c = BackPanecolorDialog.Color;
            this.mMainWindow.GetCurrentBackPlaneColorButton().BackColor = c;

			CTextDecoration d = GetSelectedTextDecoration();
			if (d!=null)
			{
				if (this.SelectedNode!=null) this.SelectedNode.mChangedSinceSelected=true;
				d.SetBackPlaneColor(c);
                d.InvalidateFont();
                RePaint();
                CommitChangesOutsideDecorationsEditor();
			}
		}
		//*******************************************************************
		public void SelectedAttahcedCheckBox(bool val)
		{
			CTextDecoration d = GetSelectedTextDecoration();

			if (d!=null)
			{
				if (this.SelectedNode!=null) this.SelectedNode.mChangedSinceSelected=true;
				// ### SRG TODO FIX d.SetAttchedToSlide(val);
			}
	
			RePaint();
		}

		//*******************************************************************
		public void SelectedBackPlaneCheckBox(bool val)
		{
			CTextDecoration d = GetSelectedTextDecoration();

			if (d!=null)
			{
				if (this.SelectedNode!=null) this.SelectedNode.mChangedSinceSelected=true;
				d.SetBackplane(val);
				this.InformTextGroupBoxSelectedTextDecoration(d);
                RePaint();
                CommitChangesOutsideDecorationsEditor();
			}
	
		}

		//*******************************************************************
		public DVDSlideshow.CTextStyle GetCurrentStyle()
		{
            CTextStyle st = mCurrentSetTextStyle.Clone();

			for (int i=0;i< this.mAddTextTabPage.Controls.Count;i++)
			{
				Control c =  this.mAddTextTabPage.Controls[i];

				if (c.Name == "LeftAlignedRadioBox")
				{
					RadioButton rb =c as RadioButton;
					if (rb !=null && rb.Checked==true)
					{
						st.Format.Alignment =System.Drawing.StringAlignment.Near;
					}
				}

				if (c.Name == "CentreAlignedRadioBox")
				{
					RadioButton rb =c as RadioButton;
					if (rb !=null && rb.Checked==true)
					{
						st.Format.Alignment =System.Drawing.StringAlignment.Center;
					}
				}

				if (c.Name == "RightAlignedRadioBox")
				{
					RadioButton rb  = c as RadioButton;
					if (rb != null && rb.Checked==true)
					{
						st.Format.Alignment =System.Drawing.StringAlignment.Far;
					}
				}
			}

			st.FontSize = 26.0f;
	
			try
			{
				st.FontSize = float.Parse(this.mMainWindow.GetFontSizeComboBox().Text,CultureInfo.InvariantCulture);
			}
			catch
			{
				st.FontSize=26.0f;
			}

			return st;
		}


		//*******************************************************************
		// set the text size string the the font size combo to the font
		// size of a textdecoration
		private void SetTextSizeComboTextToTextDecoration(CTextDecoration d)
		{
            string ss = System.String.Format("{0:f0}", d.GetFontSizeForCoverageArea() + 0.049f);
			this.mMainWindow.GetFontSizeComboBox().Text = ss;
			this.mMainWindow.GetFontSizeComboBox().Refresh();
		}



		//*******************************************************************
		public void InformTextGroupBoxSelectedTextDecoration(CTextDecoration d)
		{
			if (this.mAddTextTabPage==null) return ;

			if (d!=null)
			{
				SetTextSizeComboTextToTextDecoration(d);
			}

			for (int i=0;i< this.mAddTextTabPage.Controls.Count;i++)
			{
				Control c =  this.mAddTextTabPage.Controls[i];

				// is null decoration? unenable all controls except the AddTextButton
				if (d==null && c.Name!="AddTextButton")
				{
				//	c.Enabled=false;
					continue;
				}
				
				c.Enabled = true;
                if (c.Name == "ColourPicker") c.BackColor = d.TextStyle.TextColor;
				if (c.Name == "ColourPickerBackPlane") c.BackColor = d.BackColor;
                if (c.Name == "FontCombo") c.Text = d.TextStyle.FontName;
				if (c.Name == "ItalicCheckBox")
				{
					CheckBox cb = c as CheckBox;
                    if (cb != null) cb.Checked = d.TextStyle.Italic;
				}
				if (c.Name == "BoldCheckBox")
				{
					CheckBox cb = c as CheckBox;
                    if (cb != null) cb.Checked = d.TextStyle.Bold;
				}

				if (c.Name == "AttachedCheckbox")
				{ 
					CheckBox cb = c as CheckBox;
                    if (cb != null)
                    {
                        // if video slide then disable attach checkbox
                        if (this.mCurrentSlide != null && mCurrentSlide is CVideoSlide)
                        {
                            cb.Enabled = false;
                        }
                        else
                        {
                            cb.Checked = d.AttachedToSlideImage;
                        }
                    }
				} 

				if (c.Name == "BackPlaneCheckbox")
				{
					CheckBox cb = c as CheckBox;
                    if (cb != null) cb.Checked = d.Background;
				}

                if (c.Name == "mEditTextButton")
                {
                    Button b = c as Button;
                    if (b != null) b.Enabled = true;
                }

				if (c.Name == "ShadowTickbox")
				{
					CheckBox cb = c as CheckBox;
                    if (cb != null) cb.Checked = d.TextStyle.Shadow;
				}

				if (c.Name == "LeftAlignedRadioBox")
				{
					RadioButton rb =c as RadioButton;
                    bool check = (d.TextStyle.Format.Alignment == System.Drawing.StringAlignment.Near);
					if (rb!=null) rb.Checked= check;
				}

				if (c.Name == "CentreAlignedRadioBox")
				{
					RadioButton rb =c as RadioButton;
                    bool check = (d.TextStyle.Format.Alignment == System.Drawing.StringAlignment.Center);
					if (rb!=null) rb.Checked= check;
				}
				

				if (c.Name == "RightAlignedRadioBox")
				{
					RadioButton rb =c as RadioButton;
                    bool check = (d.TextStyle.Format.Alignment == System.Drawing.StringAlignment.Far);
					if (rb!=null) rb.Checked= check;
				}

				// out of memory!! great
				if (c.Name =="outlinecheckbox")
				{
					CheckBox cb = c as CheckBox;
                    if (cb != null) cb.Checked = d.TextStyle.Outline;
				}

				if (c.Name =="BackPlaneTransparencyTrackBar")
				{
					mHackDontChangeTD = true;
					TrackBar cb = c as TrackBar;
                    if (d.Background == false)
					{
						cb.Enabled=false;
						cb.Value=0;
					}
					else
					{
						cb.Enabled=true;
						int val = (int)(((1.0f-(d.BackPlaneTransparency/255.0f)))*100.0f);
						if (val <0) val=0; 
						if (val >100)val=100;
						if (cb!=null) cb.Value = val; 
					}
					mHackDontChangeTD = false;
				}

				if (c.Name =="BackPlaneTransparencyLabel")
				{
					Label ll = c as Label;
                    if (d.Background == false)
					{
						ll.Enabled=false;
						ll.Text="0%";
					}
					else
					{
						ll.Enabled=true;
					}
				}

				if (c.Name =="ColourPickerBackPlane")
				{
					Button bb = c as Button;
                    if (d.Background == false)
					{
						bb.Enabled=false;
						bb.BackColor=Color.Black;
					}
					else
					{
						bb.Enabled=true;
					}
				}   
			}
		}

		//*******************************************************************
		public bool DeleteCurrentSelectedDecoration()
		{
			if (mCurrentSlide==null) return  false ;

			if (SelectedNode==null) return false ;

            if (IsDecorationAllowedToBeDeletedByUser(SelectedNode.mDecor) == false)
            {
                return false;
            }

			mCurrentSlide.RemoveDecoration(this.SelectedNode.mDecor);
			this.ResetSelectedNode();
			m_enMM = MOUSE_MODE.MM_NONE;
		
			RePaint();
			CommitChangesOutsideDecorationsEditor();
            return true;

		}

		//*******************************************************************
		public void SendToBack()
		{
			if (mCurrentSlide==null) return  ;
			if (this.SelectedNode==null) return ;

			mCurrentSlide.SendToBack(SelectedNode.mDecor);
			RePaint();
			CommitChangesOutsideDecorationsEditor();
		}

		
		//*******************************************************************
		public void BringToFront()
		{
			if (mCurrentSlide==null) return  ;
			if (this.SelectedNode==null) return ;

			mCurrentSlide.BringToFront(SelectedNode.mDecor);
			RePaint();
			CommitChangesOutsideDecorationsEditor();
		}

		//*******************************************************************
		private void StartPan_MouseUp(object sender, MouseEventArgs e)
		{
			if (mStartPanTrackerChanged==true)
			{
				CGlobals.mCurrentProject.DeclareChange(true,"Start Pan Area Changed");
			}

			mStartPanTrackerChanged=false;
		}

		//*******************************************************************
		private RectangleF CapPanRectangle(RectangleF f)
		{
			float w = 1.0f;
			float h = 1.0f;

			for (int ii=1;ii<2;ii++)
			{	
				if (f.X+f.Width > w)
				{
					f.X -= (f.X+f.Width)-w;
				}

				if (f.Y+f.Height > h)
				{
					f.Y -= (f.Y+f.Height)-h;
				}

				
				if (f.Width > w ||
					f.Height > h)
				{
					f =new RectangleF(0,0,w,h);
					break;
				}
						
			}
			return f;
		}

		//*******************************************************************
		private void StartPan_Scroll(object sender, System.EventArgs e)
		{
			Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();

			ResetSelectedNode();

			mStartPanTrackerChanged=true;

			RectangleF f = this.mCurrentSlide.PanZoom.StartArea;

			float p = (float) this.mMainWindow.GetStartPanTrackBar().Value;

			p = p / 100.0f;

			f.Width = p;
			f.Height = p;

			f =CapPanRectangle(f);
	
			this.mCurrentSlide.PanZoom.StartArea = f;
	
			SetClipPanControls(true);

			this.RePaint();
		}

        //*******************************************************************
        private void StartPanRotation_Scroll(object sender, System.EventArgs e)
        {
            Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();
            ResetSelectedNode();
            mStartPanRotationTrackerChanged = true;
            this.mCurrentSlide.PanZoom.StartRotation = this.mMainWindow.GetStartPanRotationTrackBar().Value;

            // Because manual scrolling won't let us go to 0.  If we are on -1 or 1, consider this 0
            if (this.mCurrentSlide.PanZoom.StartRotation == 1 || this.mCurrentSlide.PanZoom.StartRotation == -1)
            {
                this.mCurrentSlide.PanZoom.StartRotation = 0;
            }

            SetClipPanControls(true);
            this.RePaint();
        }

        //*******************************************************************
        private void StartPanRotation_MouseUp(object sender, MouseEventArgs e)
        {
            if (mStartPanRotationTrackerChanged == true)
            {
                CGlobals.mCurrentProject.DeclareChange(true, "Start Pan Rotation Changed");
            }
            mStartPanRotationTrackerChanged = false;
        }

        //*******************************************************************
        private void EndPanRotation_Scroll(object sender, System.EventArgs e)
        {
            Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();
            ResetSelectedNode();
            mEndPanRotationTrackerChanged = true;
            this.mCurrentSlide.PanZoom.EndRotation = this.mMainWindow.GetEndPanRotationTrackBar().Value;

            // Because manual scrolling won't let us go to 0.  If we are on -1 or 1, consider this 0
            if (this.mCurrentSlide.PanZoom.EndRotation == 1 || this.mCurrentSlide.PanZoom.EndRotation == -1)
            {
                this.mCurrentSlide.PanZoom.EndRotation = 0;
            }

            SetClipPanControls(true);
            this.RePaint();
        }

        //*******************************************************************
        private void EndPanRotation_MouseUp(object sender, MouseEventArgs e)
        {
            if (mEndPanRotationTrackerChanged == true)
            {
                CGlobals.mCurrentProject.DeclareChange(true, "End Pan Rotation Changed");
            }

            mEndPanRotationTrackerChanged = false;
        }


		//*******************************************************************
		private void EndPan_MouseUp(object sender, MouseEventArgs e)
		{
			if (mEndPanTrackerChanged==true)
			{
				CGlobals.mCurrentProject.DeclareChange(true,"End Pan Area Changed");
			}

			mEndPanTrackerChanged=false;
		}

		//*******************************************************************
		private void FontSizeIndexChange(object sender, System.EventArgs e)
		{
			if (mOnlyChangeFontSizeText==true) return ;
			if (this.SelectedNode==null) return;
			if (this.SelectedNode.mDecor==null) return;

            ITextDecorationContainer itdc = SelectedNode.mDecor as ITextDecorationContainer;
            if (itdc == null) return;

			RectangleF r = new RectangleF(0,0,mPB.Image.Width, mPB.Image.Height);
			CTextStyle st = GetCurrentStyle();

            CTextDecoration td = itdc.TextDecoration;
			td.RecalcCoverageAreaForFontSize(st.FontSize);
            if (SelectedNode.mDecor != td)
            {
                SelectedNode.mDecor.CoverageArea = td.CoverageArea;
            }

			SelectedNode.rc = GetImageRectangleOfRectF(this.SelectedNode.mDecor.CoverageArea) ;
			
			RePaint();
			CommitChangesOutsideDecorationsEditor();
		}

		//*******************************************************************
		private void EndPan_Scroll(object sender, System.EventArgs e)
		{
			Form1.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();

			ResetSelectedNode();

			mEndPanTrackerChanged=true;

			RectangleF f = this.mCurrentSlide.PanZoom.EndArea;

			float p = (float) this.mMainWindow.GetEndPanTrackBar().Value;
    
			p = p / 100.0f;

			f.Width = p;
			f.Height = p;

			f =CapPanRectangle(f);

			this.mCurrentSlide.PanZoom.EndArea = f;
		
			SetClipPanControls(true);

			this.RePaint();
		}
		

		//*******************************************************************
		// sets the controls to the correct values
		private void SetClipPanControls(bool fromMoveOrScroll)
		{
            if (this.mMainWindow == null || mCurrentSlide == null) return;
            if (mCurrentSlide is CVideoSlide) return;

			TrackBar t = mMainWindow.GetStartPanTrackBar();
			int val = (int) (this.mCurrentSlide.PanZoom.StartArea.Width * 100.0f);
			if (val <10) val =10;
			if (val > 100) val =100;

			if (t.Value != val) t.Value = val;
			Label l =mMainWindow.GetStartPanTrackBarLabel(); 
			l.Text = val +"%";
			l.Refresh();

            t = mMainWindow.GetStartPanRotationTrackBar();
            val = (int)(this.mCurrentSlide.PanZoom.StartRotation);
            if (val < -360) val = 360;
            if (val > 360) val = 360;

            if (t.Value != val) t.Value = val;
            l = mMainWindow.GetStartRotationTrackBarLabel();
            l.Text = val + "°";
            l.Refresh();

			t = mMainWindow.GetEndPanTrackBar();
			val = (int) (this.mCurrentSlide.PanZoom.EndArea.Width * 100.0f);
			if (val <10) val =10;
			if (val > 100) val =100;

			if (t.Value != val) t.Value = val;
			l =mMainWindow.GetEndPanTrackBarLabel(); 
			l.Text = val +"%";
			l.Refresh();

            t = mMainWindow.GetEndPanRotationTrackBar();
            val = (int)(this.mCurrentSlide.PanZoom.EndRotation );
            if (val < -360) val = 360;
            if (val > 360) val = 360;

            if (t.Value != val) t.Value = val;
            l = mMainWindow.GetEndRotationTrackBarLabel();
            l.Text = val + "°";
            l.Refresh();

            int sval = (int)(mCurrentSlide.PanZoom.InitialDelay * 100.0f);
            if (sval <0) sval = 0;
            if (sval > 100) sval =100;

            mMainWindow.GetPanZoomInitialDelayTrackBar().Value = sval;

            mMainWindow.GetStartPanDelayLabel().Text = string.Format("{0:0.0}s\n\r({1:0.0}s)", mCurrentSlide.DisplayLength * mCurrentSlide.PanZoom.InitialDelay, mCurrentSlide.DisplayLength);

            mDecorationManagerSettingEffects.RefreshSlideLength(mCurrentSlide.DisplayLength);

            int eval = (int)(mCurrentSlide.PanZoom.EndDelay * 100.0f);
            if (eval < 0) eval = 0;
            if (eval > 100) eval = 100;

            mMainWindow.GetPanZoomEndDelayTrackBar().Value = eval;

            mMainWindow.GetEndPanDelayLabel().Text = string.Format("{0:0.0}s\n\r({1:0.0}s)", mCurrentSlide.DisplayLength * mCurrentSlide.PanZoom.EndDelay, mCurrentSlide.DisplayLength);

            bool rePaint = mNoRepaint;
            mNoRepaint = true;
            mMainWindow.GetTurnOffPanZoomForSlideTickBox().Checked = !mCurrentSlide.UsePanZoom;

            if (mCurrentSlide.PanZoom.MovementEquation is CLinear)
            {
                mMainWindow.GetPanZoomEquationComboBox().SelectedIndex = 0;
            }
            else if (mCurrentSlide.PanZoom.MovementEquation is CNonLinear)
            {
                mMainWindow.GetPanZoomEquationComboBox().SelectedIndex = 1;
            }
            else if (mCurrentSlide.PanZoom.MovementEquation is CSpring)
            {
                mMainWindow.GetPanZoomEquationComboBox().SelectedIndex = 2;
            }
            else if (mCurrentSlide.PanZoom.MovementEquation is CQuickSlow)
            {
                mMainWindow.GetPanZoomEquationComboBox().SelectedIndex = 3;
            }

            mMainWindow.GetPanZoomOnAllCheckBox().Checked = mCurrentSlide.PanZoom.PanZoomOnAll;
            mMainWindow.GetReCalcPanZoomTemplateCheckbox().Checked = mCurrentSlide.PanZoom.ReGenerateOnImageChange;

            mNoRepaint = rePaint;

            PictureBox pb = this.mMainWindow.GetPreviewPanZoomPictureBox();

            double hd = ((double)pb.Width) *(3.0/4.0) ;

            if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV16_9) hd = ((double)pb.Width) * (9.0 / 16.0);
            if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV221_1) hd = ((double)pb.Width) * (1.0 / 2.21);

            int h = (int)hd;

            if (pb.Height != h) pb.Height = h;

            bool currentSlideIsFirstSlide = false;

            CSlideShow currentShow = Form1.mMainForm.GetSlideShowManager().mCurrentSlideShow;
            if (currentShow != null)
            {
                if (currentShow.mSlides.Count > 0)
                {
                    if (currentShow.mSlides[0] == mCurrentSlide)
                    {
                        currentSlideIsFirstSlide = true;
                        mMainWindow.GetPanZoomLinkStartAreaButton().Enabled = false;   
                    }
                }
            }
         
            if (mCurrentSlide.UsePanZoom == false)
            {
                mMainWindow.GetStartPanTrackBar().Enabled = false;
                mMainWindow.GetStartPanRotationTrackBar().Enabled = false;
                mMainWindow.GetStartPanTrackBarLabel().Enabled = false;
                mMainWindow.GetStartRotationTrackBarLabel().Enabled = false;

                mMainWindow.GetEndPanTrackBar().Enabled = false;
                mMainWindow.GetEndPanRotationTrackBar().Enabled = false;
                mMainWindow.GetEndPanTrackBarLabel().Enabled = false;
                mMainWindow.GetEndRotationTrackBarLabel().Enabled = false;

                mMainWindow.GetPanZoomInitialDelayTrackBar().Enabled = false;
                mMainWindow.GetPanZoomEndDelayTrackBar().Enabled = false;
                mMainWindow.GetStartPanDelayLabel().Enabled = false;
                mMainWindow.GetEndPanDelayLabel().Enabled = false;
                mMainWindow.GetPanZoomEquationComboBox().Enabled = false;
                mMainWindow.GetPanZoomLinkStartAreaButton().Enabled = false;
                mMainWindow.GetPanZoomOnAllCheckBox().Enabled = false;
            }
            else
            {
                mMainWindow.GetStartPanTrackBar().Enabled = true;
                mMainWindow.GetStartPanRotationTrackBar().Enabled = true;
                mMainWindow.GetStartPanTrackBarLabel().Enabled = true;
                mMainWindow.GetStartRotationTrackBarLabel().Enabled = true;

                mMainWindow.GetEndPanTrackBar().Enabled = true;
                mMainWindow.GetEndPanRotationTrackBar().Enabled = true;
                mMainWindow.GetEndPanTrackBarLabel().Enabled = true;
                mMainWindow.GetEndRotationTrackBarLabel().Enabled = true;

                mMainWindow.GetPanZoomInitialDelayTrackBar().Enabled = true;
                mMainWindow.GetPanZoomEndDelayTrackBar().Enabled = true;
                mMainWindow.GetStartPanDelayLabel().Enabled = true;
                mMainWindow.GetEndPanDelayLabel().Enabled = true;
                mMainWindow.GetPanZoomEquationComboBox().Enabled = true;
                mMainWindow.GetPanZoomOnAllCheckBox().Enabled = true;

                if (currentSlideIsFirstSlide == false)
                {
                    mMainWindow.GetPanZoomLinkStartAreaButton().Enabled = true;
                }
            }

            if (mPanZoomMiniPreviewController != null && fromMoveOrScroll==false)
            {
                mPanZoomMiniPreviewController.ResetFrameCountToStart();
            }

		//	mMainWindow.GetClipPanTabPage().Invalidate();

		}

		//*******************************************************************
		public void FillInGuiMemento(CGuiMemento m)
		{
			if (this.mCurrentSlide==null)
			{
				return ;
			}

			m.mTanControl2Index=mMainWindow.GetDecorationsTabControl().SelectedIndex;
		
			if (IsCurrentSlideAMenuSlide()==true)
			{
				m.mShowingMainMenu = true;
				ArrayList a =CGlobals.mCurrentProject.MainMenu.GetSelfAndAllSubMenus();
				int index =0;

				foreach (CMainMenu mm in a)
				{
                    if (mm.BackgroundSlide.ID == this.mCurrentSlide.ID)
					{
						m.mMenuIndex=index;
					}
					index++;
				}
			}
			else
			{
				m.mShowingMainMenu = false;
			
				CSlideShow ss =this.mMainWindow.GetSlideShowManager().mCurrentSlideShow;

				int index=0;

				foreach (CSlide s in ss.mSlides)
				{
					if (s.ID == this.mCurrentSlide.ID)
					{
						m.mSlideIndex = index;
					}
					index++;
				}
			}
		}

		//*******************************************************************
		public void SetMemento(CGuiMemento m)
		{
			if (m.mShowingMainMenu==true)
			{
				ArrayList a =CGlobals.mCurrentProject.MainMenu.GetSelfAndAllSubMenus();
                this.SetCurrentSlide(((CMainMenu)a[m.mMenuIndex]).BackgroundSlide as CImageSlide);
			}
			else
			{
				CSlideShow ss =this.mMainWindow.GetSlideShowManager().mCurrentSlideShow;

                //
                // Slideshow may now not contain any slides
                //
                if (ss !=null && ss.mSlides.Count > 0)
                {
                    this.SetCurrentSlide(ss.mSlides[m.mSlideIndex] as CImageSlide);
                }
			}
			this.mMainWindow.GetDecorationsTabControl().SelectedIndex =m.mTanControl2Index;		
		}


		
		//*******************************************************************
		private void ApplyHideUnhideState()
		{
            if (this.mMainWindow == null) return;
			// clipart image

			bool is_not_text_deccor=false;

            bool disableAll = false;
            if (this.SelectedNode == null)
            {
                disableAll = true;
            }
            else if (SelectedNode.mDecor is CMenuButton)
            {
                if (SelectedNode.mDecor is ITextDecorationContainer == false)
                {
                    disableAll = true;
                }
                //
                // Allow exact positing of menu buttons though
                //
                mPositionTop.Enabled = true;
                mPositionLeft.Enabled = true;
                mPositionWidth.Enabled = true;
                mPositionHeight.Enabled = true;
                mRotationTrackbar.Enabled = true;
            }

            if (disableAll == false)
			{
                // image
                CImageDecoration imageDecoration = this.SelectedNode.mDecor as CImageDecoration;

                if (imageDecoration != null)
                {
                    int ii = (int)(imageDecoration.Transparency * 100.0f);
                    if (ii < 0) ii = 0;
                    if (ii > 100) ii = 100;
                    this.mHackDontChangeTD = true;
                    mImageDecorationTransparentTrackbar.Value = ii;
                    this.mHackDontChangeTD = false;
                    mImageDecorationTransparentTrackbar.Enabled = true;
                    this.mImageDecorationTransparentLabel.Enabled = true;

                    mPositionTop.Enabled = true;
                    mPositionLeft.Enabled = true;
                    mPositionWidth.Enabled = true;
                    mPositionHeight.Enabled = true;
                    mRotationTrackbar.Enabled = true;

                    int rot = (int)(imageDecoration.Rotation);
                    rot = CGlobals.Clamp(rot, -180,180);
                    if (mRotationTrackbar.Value != rot)
                    {
                        mRotationTrackbar.Value = rot;
                    }
                    mRotationLabel.Enabled = true;
                }

				// clip art or video
                if (this.SelectedNode.mDecor is CClipArtDecoration ||
                    this.SelectedNode.mDecor is CVideoDecoration)
				{
					is_not_text_deccor=true;

					
					mMainWindow.GetRotateDecorationClockWiseButton().Enabled=true;
					mMainWindow.GetRotateDecorationAntiClockWiseButton().Enabled=true;
					mMainWindow.GetFlipDecorationLeftRightButton().Enabled=true;
					mMainWindow.GetFlipDecorationUpDownButton().Enabled=true;
                    mMainWindow.GetEditImageButton().Enabled = true;
				}

				// text
                ITextDecorationContainer itdc = this.SelectedNode.mDecor as ITextDecorationContainer;
                if (itdc != null)
				{
                    CTextDecoration td = itdc.TextDecoration;
					InformTextGroupBoxSelectedTextDecoration(td);
                    if (td.TextStyle != null)
                    {
                        mCurrentSetTextStyle = td.TextStyle;
                        UpdatePreviewFontPictureBox();
                    }

                    mMainWindow.GetRotateDecorationClockWiseButton().Enabled = false;
                    mMainWindow.GetRotateDecorationAntiClockWiseButton().Enabled = false;
                    mMainWindow.GetFlipDecorationLeftRightButton().Enabled = false;
                    mMainWindow.GetFlipDecorationUpDownButton().Enabled = false;
                    mMainWindow.GetEditImageButton().Enabled = false;
				}

                mDecorationManagerSettingEffects.EnableControls( SelectedNode.mDecor, mCurrentSlide );
			}
			
            if (disableAll == true || SelectedNode.mDecor is CMenuButton)
			{
                mRotationLabel.Enabled = false;
                mRotationTrackbar.Enabled = false;
				mImageDecorationTransparentTrackbar.Value =0;
				mImageDecorationTransparentTrackbar.Enabled = false;
				mImageDecorationTransparentLabel.Enabled = false;
				mMainWindow.GetRotateDecorationClockWiseButton().Enabled=false;
				mMainWindow.GetRotateDecorationAntiClockWiseButton().Enabled=false;
				mMainWindow.GetFlipDecorationLeftRightButton().Enabled=false;
				mMainWindow.GetFlipDecorationUpDownButton().Enabled=false;
                mMainWindow.GetEditImageButton().Enabled = false;
				this.InformTextGroupBoxSelectedTextDecoration(null);
                mDecorationManagerSettingEffects.DisableControls(mCurrentSlide);

                if (this.SelectedNode == null)
                {
                    mPositionTop.Value = 0;
                    mPositionLeft.Value = 0;
                    mPositionWidth.Value = 0;
                    mPositionHeight.Value = 0;
                    mRotationTrackbar.Value = 0;
                }

                if (SelectedNode == null || (SelectedNode.mDecor is CMenuButton == false))
                {
                    mPositionTop.Enabled = false;
                    mPositionLeft.Enabled = false;
                    mPositionWidth.Enabled = false;
                    mPositionHeight.Enabled = false;
                    mRotationTrackbar.Enabled = false;
                    mRotationLabel.Enabled = false;
                }
			}

            if (disableAll == true || is_not_text_deccor == true)
			{
				mMainWindow.GetCurrentBackPlaneColorButton().BackColor=Color.Black;
				mMainWindow.GetCurrentBackPlaneColorButton().Enabled=false;
				mMainWindow.GetBackPlaneCheckbox().Checked=false;
				mMainWindow.GetBackPlaneCheckbox().Enabled=false;
                mMainWindow.GetEditTextButton().Enabled = false;
				this.mBackPlaneTransparentTrackbar.Value=0;
				this.mBackPlaneTransparentTrackbar.Enabled=false;
				this.mBackPlaneTransparentLabel.Enabled=false;
              
			}
		}

		//*******************************************************************
		public void RandomPanZoomButton_Click()
		{
			ResetSelectedNode();

			CSlide s = this.mCurrentSlide;
			if (s==null) return;

            if (s.UsePanZoom == false) return;

			CStillPictureSlide ss = s as CStillPictureSlide;
			if (ss!=null)
			{
				ss.ReRadomisePanZoom();         

				SetClipPanControls(false);
				this.RePaint();
			}
		}

        //******************************************************************
        private void TurnOnOffPanZoomForSlideCheckedChaged(object o, EventArgs e)
        {
            if (this.mCurrentSlide==null) return ;

            if (IsCurrentSlideAMenuSlide() == true) return;
       
            this.mCurrentSlide.UsePanZoom = !this.mMainWindow.GetTurnOffPanZoomForSlideTickBox().Checked;

            if (mPanZoomMiniPreviewController != null)
            {
                mPanZoomMiniPreviewController.ResetFrameCountToStart();
            }

            this.ResetSelectedNode();

            if (mNoRepaint == false)
            {
                CGlobals.mCurrentProject.DeclareChange(true, "Pan Zoom turned on/off");
                SetClipPanControls(false);
                this.RePaint();         
            }
        }

        //******************************************************************
        public void PanZoomInitialDelayScroll(object o, EventArgs e)
        {
            if (mMainWindow==null  || mCurrentSlide== null) return;

            mPanInitialDelayTrackerChanged = true;

            // turn on smoothing if first time setting start/end delay and current movement is linear
            if (mCurrentSlide.PanZoom.InitialDelay == 0.0f &&
                 mCurrentSlide.PanZoom.EndDelay == 1.0f)
            {
                bool noRepaint = mNoRepaint;
                mNoRepaint = true;
                if (this.mMainWindow.GetPanZoomEquationComboBox().SelectedIndex == 0)
                {
                    this.mMainWindow.GetPanZoomEquationComboBox().SelectedIndex = 1;    // non linear
                }
                mNoRepaint = noRepaint;
            }


            if (mMainWindow.GetPanZoomInitialDelayTrackBar().Value >
                mMainWindow.GetPanZoomEndDelayTrackBar().Value)
            {
                int new_val = mMainWindow.GetPanZoomEndDelayTrackBar().Value -1;
                if (new_val <0) new_val =0;
                mMainWindow.GetPanZoomInitialDelayTrackBar().Value = new_val;
            }


            int s_val = mMainWindow.GetPanZoomInitialDelayTrackBar().Value;

            mCurrentSlide.PanZoom.InitialDelay = ((float)s_val)/100.0f;

            SetClipPanControls(true);
        }

        //******************************************************************
        public void PanZoomEndDelayScroll(object o, EventArgs e)
        {
            if (mMainWindow == null || mCurrentSlide == null) return;

            mPanEndDelayTrackerChanged = true;

            // turn on smoothing if first time setting start/end delay and current movement is linear
            if (mCurrentSlide.PanZoom.InitialDelay == 0.0f &&
                mCurrentSlide.PanZoom.EndDelay == 1.0f)
            {
                bool noRepaint = mNoRepaint;
                mNoRepaint = true;
                if (this.mMainWindow.GetPanZoomEquationComboBox().SelectedIndex == 0)
                {
                    this.mMainWindow.GetPanZoomEquationComboBox().SelectedIndex = 1; // non linear
                }
                mNoRepaint = noRepaint;
            }


            if (mMainWindow.GetPanZoomEndDelayTrackBar().Value <
                mMainWindow.GetPanZoomInitialDelayTrackBar().Value)
            {
                int new_val = mMainWindow.GetPanZoomInitialDelayTrackBar().Value +1;
                if (new_val > 100) new_val = 100;
                mMainWindow.GetPanZoomEndDelayTrackBar().Value = new_val;
            }

            int e_val = mMainWindow.GetPanZoomEndDelayTrackBar().Value;

            mCurrentSlide.PanZoom.EndDelay = ((float)e_val) / 100.0f;
     
            SetClipPanControls(true);
        }

        //******************************************************************
        public void PanZoomSmoothedComboBoxChanged(object o, EventArgs e)
        {
            if (this.mCurrentSlide == null) return;
            if (IsCurrentSlideAMenuSlide() == true) return;

            int selectedIndex = this.mMainWindow.GetPanZoomEquationComboBox().SelectedIndex;

            if (selectedIndex == 0)
            {
                if (mCurrentSlide.PanZoom.MovementEquation is CLinear) return;
                mCurrentSlide.PanZoom.MovementEquation = new CLinear();
            }
            else if (selectedIndex == 1)
            {
                if (mCurrentSlide.PanZoom.MovementEquation is CNonLinear) return;
                mCurrentSlide.PanZoom.MovementEquation = new CNonLinear();
            }
            else if (selectedIndex == 2)
            {
                if (mCurrentSlide.PanZoom.MovementEquation is CSpring) return;
                mCurrentSlide.PanZoom.MovementEquation = new CSpring();
            }
            else if (selectedIndex == 3)
            {
                if (mCurrentSlide.PanZoom.MovementEquation is CQuickSlow) return;
                mCurrentSlide.PanZoom.MovementEquation = new CQuickSlow();
            }
            else
            {
                Log.Error("Unknown pan/zoom movement type: "+selectedIndex);
                return;
            }

            if (mPanZoomMiniPreviewController != null)
            {
                mPanZoomMiniPreviewController.ResetFrameCountToStart();
            }
     
            if (mNoRepaint == false)
            {
                CGlobals.mCurrentProject.DeclareChange(true, "Smooth on/off pan zoom changed");

                SetClipPanControls(false);
                this.RePaint();
            }
        }

        //******************************************************************
        private void ReCalcPanZoomTemplateCheckbox_CheckedChanged(object o, EventArgs e)
        {
            if (mCurrentSlide != null &&
                mCurrentSlide.PanZoom.ReGenerateOnImageChange != mMainWindow.GetReCalcPanZoomTemplateCheckbox().Checked)
            {
                mCurrentSlide.PanZoom.ReGenerateOnImageChange = mMainWindow.GetReCalcPanZoomTemplateCheckbox().Checked;
            }
        }


        //******************************************************************
        public void PanZoomInitialDelayScroll_MouseUp(object o, EventArgs e)
        {
            if (mPanInitialDelayTrackerChanged==true)
			{
				CGlobals.mCurrentProject.DeclareChange(true,"Start Pan Area Initial Delay Changed");
                if (mPanZoomMiniPreviewController != null)
                {
                    mPanZoomMiniPreviewController.ResetFrameCountToStart();
                }
			}                     
        }

        //******************************************************************
        public void PanZoomEndDelayScroll_MouseUp(object o, EventArgs e)
        {
            if (mPanEndDelayTrackerChanged == true)
            {
                CGlobals.mCurrentProject.DeclareChange(true, "End Pan Area Delay Changed");
                if (mPanZoomMiniPreviewController != null)
                {
                    mPanZoomMiniPreviewController.ResetFrameCountToStart();
                }
            }
        }

        //******************************************************************
        public void PanZoomLinkStartAreaButton_Click(object o, EventArgs e)
        {
            CSlideShow slideshow = mMainWindow.GetSlideShowManager().mCurrentSlideShow;
            if (slideshow == null) return;

            ArrayList slides = slideshow.mSlides;
            for (int i = 0; i < slides.Count; i++)
            {
                if (slides[i] == mCurrentSlide)
                {
                    if (i > 0)
                    {
                        CImageSlide previousSlide = slides[i - 1] as CImageSlide;
                        if (previousSlide != null)
                        {
                            ResetSelectedNode();
                            mCurrentSlide.PanZoom.StartArea = previousSlide.PanZoom.EndArea;
                            mCurrentSlide.PanZoom.StartRotation = previousSlide.PanZoom.EndRotation;
                            SetClipPanControls(false);
                            this.RePaint();
                            return;
                        }
                    }
                }
            }
        }

        //******************************************************************
        public void InformOfSlideLengthChange()
        {
            if (this.mCurrentSlide != null &&
                this.IsCurrentSlideAMenuSlide() == false)
            {
                SetClipPanControls(false);
            }
        }

        //******************************************************************
        private void SetBackgroundChangedSlideCallback()
        {
            RePaint();
            CommitChangesOutsideDecorationsEditor();
        }

        //******************************************************************
        private void SetFilterSlideChangedCallback()
        {
            CommitChangesOutsideDecorationsEditor();
        }

        //******************************************************************
        private void BackgroundChangedOnAllSlidesCallback()
        {
            Form1.mMainForm.GetSlideShowManager().InvalidateAllThumbnails();
            CGlobals.mCurrentProject.DeclareChange("Changed background on all slides");
        }

        //******************************************************************
        private void RebuildSelectedDecorationsCombo()
        {
            if (mCurrentSlide == null) return;
            if (mSelectedDecorationsCombo == null) return;

            mSelectedDecorationsCombo.SuspendLayout();
            mSelectedDecorationsCombo.Items.Clear();
            mSelectedDecorations.Clear();
            mSelectedDecorations.Add(null);
            mSelectedDecorationsCombo.Items.Add("None");

            ArrayList list = mCurrentSlide.GetAllAndSubDecorations();

            foreach (CDecoration d in list)
            {
                CImageDecoration imageDecor = d as CImageDecoration;
                if (imageDecor != null)
                {
                    if (imageDecor.IsBorderDecoration() || imageDecor.IsBackgroundDecoration() || imageDecor.IsFilter() )
                    {
                        continue;
                    }

                }

                CClipArtDecoration cad = imageDecor as CClipArtDecoration;

                if (cad != null)
                {
                    mSelectedDecorationsCombo.Items.Add(Path.GetFileName(cad.mImage.ImageFilename));
                    mSelectedDecorations.Add(cad);
                }

                CVideoDecoration vd = d as CVideoDecoration;
                if (vd != null)
                {
                    mSelectedDecorationsCombo.Items.Add(Path.GetFileName(vd.GetFilename()));
                    mSelectedDecorations.Add(vd);

                }

                ITextDecorationContainer itdc = d as ITextDecorationContainer;
                if (itdc != null)
                {
                    CTextDecoration td = itdc.TextDecoration;

                    if (td.VCDNumber == true && (
                        CGlobals.mCurrentProject.DiskPreferences.OutputVideoType != CGlobals.VideoType.SVCD &&
                        CGlobals.mCurrentProject.DiskPreferences.OutputVideoType != CGlobals.VideoType.VCD))
                    {
                        continue;
                    }

                    string text = td.mText;
                    if (text.Length > 40) text = text.Substring(0, 40);
                    mSelectedDecorationsCombo.Items.Add(text);
                    mSelectedDecorations.Add(d);
                    continue;
                }


                CMenuButton msb = d as CMenuSlideshowButton;
                if (msb != null)
                {
                    mSelectedDecorationsCombo.Items.Add("Slideshow button");
                    mSelectedDecorations.Add(msb);
                    continue;
                }

                CMenuButton mlb = d as CMenuLinkButton;
                if (mlb != null)
                {
                    mSelectedDecorationsCombo.Items.Add("Menu link button");
                    mSelectedDecorations.Add(mlb);
                    continue;
                }
            }

            mSelectedDecorationsCombo.SelectedIndex = 0;
            mSelectedDecorationsCombo.ResumeLayout();
        }


        //*******************************************************************
        public void SetSelectedDecorationChanged(object sender, System.EventArgs e)
        {         
            int index = mSelectedDecorationsCombo.SelectedIndex;

            if (index < mSelectedDecorations.Count)
            {
                CDecoration selectedDecor = mSelectedDecorations[index];

                if (SelectedNode == null && selectedDecor == null)
                {
                    return;
                }

                if (SelectedNode != null)
                {
                    if (SelectedNode.mDecor == selectedDecor)
                    {
                        return;
                    }
                }

                if (selectedDecor == null)
                {
                    this.ResetSelectedNode();
                    this.RePaint();
                    return;
                }

                bool lockedApsect = false;
                if (mMainWindow != null)
                {
                    lockedApsect = mMainWindow.GetClipArtLockAspectCheckBox().Checked;
                }


                Node n1 = CreateNodeFromDecoration(selectedDecor, lockedApsect);

                if (n1 != null)
                {
                    SetToSelectedNode(n1, true);
                    mNodeSetBySelectCombo = true; // remember this node was selected by the combo
                }
                this.RePaint();
              
            }
        }

        //*******************************************************************
        private void mPositionTop_ValueChanged(object sender, EventArgs e)
        {
            if (this.SelectedNode == null) return;
            if (this.SelectedNode.mDecor == null) return;

            CDecoration d = SelectedNode.mDecor;

            float y = (float) mPositionTop.Value;
            y /= 100.0f;

            if (Math.Abs(y - d.CoverageArea.Y) > 0.00005)
            {
                RectangleF newCoverage = d.CoverageArea;
                newCoverage.Y = y;
                d.CoverageArea = newCoverage;
                SelectedNode.rc = GetImageRectangleOfRectF(d.CoverageArea);
                RePaint();
                CommitChangesOutsideDecorationsEditor();
            }
        }

        //*******************************************************************
        private void mPositionLeft_ValueChanged(object sender, EventArgs e)
        {
            if (this.SelectedNode == null) return;
            if (this.SelectedNode.mDecor == null) return;

            CDecoration d = SelectedNode.mDecor;

            float x = (float)mPositionLeft.Value;
            x /= 100.0f;

            if (Math.Abs(x - d.CoverageArea.X) >0.00005)
            {
                RectangleF newCoverage = d.CoverageArea;
                newCoverage.X = x;
                d.CoverageArea = newCoverage;
                SelectedNode.rc = GetImageRectangleOfRectF(d.CoverageArea);
                RePaint();
                CommitChangesOutsideDecorationsEditor();
            }
        }

        //*******************************************************************
        private void mPositionWidth_ValueChanged(object sender, EventArgs e)
        {
            if (this.SelectedNode == null) return;
            if (this.SelectedNode.mDecor == null) return;

            CDecoration d = SelectedNode.mDecor;

            float width = (float)mPositionWidth.Value;
            width /= 100.0f;

            if (Math.Abs(width - d.CoverageArea.Width) >=0.00005)
            {
                bool lockedAspect = false;

                if (mMainWindow != null)
                {
                    lockedAspect = mMainWindow.GetClipArtLockAspectCheckBox().Checked;
                }

                if (d is ITextDecorationContainer)
                {
                    lockedAspect = true;
                }

                if (lockedAspect)
                {
                    float aspect = d.CoverageArea.Width / d.CoverageArea.Height;
                    RectangleF newHeightCoverage = d.CoverageArea;
                    newHeightCoverage.Height = width / aspect;
                    d.CoverageArea = newHeightCoverage;
                }

                RectangleF newWidthCoverage = d.CoverageArea;
                newWidthCoverage.Width = width;
                d.CoverageArea = newWidthCoverage;
                SelectedNode.rc = GetImageRectangleOfRectF(d.CoverageArea);
                mPositionHeight.Value = (decimal)(d.CoverageArea.Height * 100);
                RePaint();
                CommitChangesOutsideDecorationsEditor();
  
            }
        }

        //*******************************************************************
        private void mPositionHeight_ValueChanged(object sender, EventArgs e)
        {
            if (this.SelectedNode == null) return;
            if (this.SelectedNode.mDecor == null) return;

            CDecoration d = SelectedNode.mDecor;

            float height = (float)mPositionHeight.Value;
            height /= 100.0f;
          
            if (Math.Abs( height - d.CoverageArea.Height) >= 0.00005)
            {
                bool lockedAspect = false;

                if (mMainWindow != null)
                {
                    lockedAspect = mMainWindow.GetClipArtLockAspectCheckBox().Checked;
                }

                if (d is ITextDecorationContainer)
                {
                    lockedAspect = true;
                }

                if (lockedAspect)
                {
                    float aspect = d.CoverageArea.Width / d.CoverageArea.Height;
                    RectangleF newWidthCoverage = d.CoverageArea;
                    newWidthCoverage.Width = height * aspect;
                    d.CoverageArea = newWidthCoverage;
                }

                RectangleF newHeightCoverage = d.CoverageArea;
                newHeightCoverage.Height = height;
                d.CoverageArea = newHeightCoverage;
                SelectedNode.rc = GetImageRectangleOfRectF(d.CoverageArea);
                mPositionWidth.Value = (decimal)(d.CoverageArea.Width * 100);
                RePaint();
                CommitChangesOutsideDecorationsEditor();

            }
        }


        //*******************************************************************
        public void mRotation_Trackbar_Scroll(object o, System.EventArgs e)
        {
            this.mRotationTrackerMoved = true;
        }

         //*******************************************************************
        public void mRotationNumericalArrowsChanged(object o, System.EventArgs e)
        {
            if (mRotationTrackbar.Value != (int)mRotationNumericalArrows.Value)
            {
                mRotationTrackbar.Value = (int)mRotationNumericalArrows.Value;
                CommitChangesOutsideDecorationsEditor();
            }
        }

        //*******************************************************************
        void mRotation_ValueChanged(object sender, EventArgs e)
        {
            string emptystring="0°";

            if (this.SelectedNode == null || this.SelectedNode.mDecor == null)
            {
                mRotationLabel.Text = emptystring;
                return;
            }

            CImageDecoration imageDecor = SelectedNode.mDecor as CImageDecoration;
            if (imageDecor == null)
            {
                mRotationLabel.Text = emptystring;
                return;
            } 
            float rotation = (float)this.mRotationTrackbar.Value;
         
            mRotationLabel.Text = this.mRotationTrackbar.Value.ToString() + "°";

            if (mRotationNumericalArrows.Value != mRotationTrackbar.Value)
            {
               mRotationNumericalArrows.Value = mRotationTrackbar.Value;
            }

            if (Math.Abs(rotation - imageDecor.Rotation) >= 0.999999f)
            {
                imageDecor.Rotation = rotation;
                SelectedNode.rotation = rotation;         
                RePaint();
            }   
        }

        //*******************************************************************
        public void mRotation_Trackbar_MouseUp(object o, System.Windows.Forms.MouseEventArgs e)
        {
            if (mRotationTrackerMoved == true)
            {
                this.CommitChangesOutsideDecorationsEditor();
            }
            mRotationTrackerMoved = false;
        }



        //*******************************************************************
        private void UpateAfterContextMenuMovementChange(CDecoration dec)
        {
            this.UpdateDecorXYWidthHeigthTextBoxs(dec);
            this.RePaint();
            this.CommitChangesOutsideDecorationsEditor();
        }

        //*******************************************************************
        private void mOriginalAspectMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedNode == null) return;
            CImageDecoration imageDecor = SelectedNode.mDecor as CImageDecoration;
            if (imageDecor == null) return;

            float slideAspect = CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction();

            float originalAspect = imageDecor.GetOriginialImageAspectRatio();

            originalAspect *= slideAspect;

            RectangleF newCoverage = imageDecor.CoverageArea;
            newCoverage.Height = imageDecor.CoverageArea.Width / originalAspect;
            imageDecor.CoverageArea = newCoverage;
            SelectedNode.rc = GetImageRectangleOfRectF(imageDecor.CoverageArea);
            UpateAfterContextMenuMovementChange(imageDecor);
        }

        //*******************************************************************
        private void FitToSlideMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedNode == null) return;
            CImageDecoration imageDecor = SelectedNode.mDecor as CImageDecoration;
            if (imageDecor == null) return;

            RectangleF area = new RectangleF(0, 0, 1, 1);

            float aspect = imageDecor.GetOriginialImageAspectRatio();

            float ww = aspect;
            float hh = 1 / CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction();

            area = CGlobals.CalcBestFitRectagleF(area, new RectangleF(0, 0, ww, hh));
            imageDecor.CoverageArea = area;
            imageDecor.Rotation = 0;
            SelectedNode.rc = GetImageRectangleOfRectF(imageDecor.CoverageArea);
            SelectedNode.rotation = 0;
            UpateAfterContextMenuMovementChange(imageDecor);
        }


        //*******************************************************************
        private void FillSlideMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedNode == null) return;
            CImageDecoration imageDecor = SelectedNode.mDecor as CImageDecoration;
            if (imageDecor == null) return;
   
            float imageAspect = imageDecor.GetOriginialImageAspectRatio();

            RectangleF area = CGlobals.CalcBestFitZoomedRect(CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction(), imageAspect);

            imageDecor.CoverageArea = area;
            imageDecor.Rotation = 0;
            SelectedNode.rc = GetImageRectangleOfRectF(imageDecor.CoverageArea);
            SelectedNode.rotation = 0;
            UpateAfterContextMenuMovementChange(imageDecor);
        }

        //*******************************************************************
        private void mStrechToSlideMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedNode == null) return;
            CImageDecoration imageDecor = SelectedNode.mDecor as CImageDecoration;
            if (imageDecor == null) return;

            RectangleF area = new RectangleF(0, 0, 1, 1);
            imageDecor.CoverageArea = area;
            imageDecor.Rotation = 0;
            SelectedNode.rc = GetImageRectangleOfRectF(imageDecor.CoverageArea);
            SelectedNode.rotation = 0;
            UpateAfterContextMenuMovementChange(imageDecor);
        }


        //*******************************************************************
        private void mPanZoomOnAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mCurrentSlide == null) return;

            if (mPanZoomOnAllCheckBox.Checked != mCurrentSlide.PanZoom.PanZoomOnAll)
            {
                mCurrentSlide.PanZoom.PanZoomOnAll = mPanZoomOnAllCheckBox.Checked;
                CGlobals.mCurrentProject.DeclareChange(true, "Pan/zoom all toggle");
            }
        }
	}
}
