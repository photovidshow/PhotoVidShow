using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using dvdslideshowfontend;
using DVDSlideshow;

using System.Drawing.Imaging;

namespace ZBobb
{
	/// <summary>
	/// AlphaBlendTextBox: A .Net textbox that can be translucent to the background.
	/// (C) 2003 Bob Bradley / ZBobb@hotmail.com
	/// </summary>
	/// 



	public class AlphaBlendTextBox : System.Windows.Forms.TextBox
	{
		#region private variables

		private uPictureBox myPictureBox;
		private  bool   myUpToDate = false;
		private  bool   myCaretUpToDate = false;
		private  Bitmap myBitmap;
		private  Bitmap myAlphaBitmap;

        private const int mMaxWidth = 1920;
        private const int mMaxHeight = 1080;

		private float myFontHeight = 26;

		private System.Windows.Forms.Timer myTimer1;

		private bool myCaretState = true;

		private bool myPaintedFirstTime = false;

		private Color myBackColor = Color.Black;
		private int myBackAlpha = 0;		
		
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion // end private variables

		public string mFontString;
		public bool mItalic;
		public bool mBold;
		public Color mColor;
		public float mStoredFontSize;   // the virtual  univeral font size we store things with (not the raw font size to draw with)

		public Point mTopLeft;
		public float mXRat;
		public float mYRat;
		public bool do_caret;
		public PictureBox mPB;
		private bool mCaretOnNewLine=false;

		private string mLastText = "";

		private Bitmap mImageCopy;

		public CDecorationsManager mManager;
		private CTextDecoration mTD;    // internal editable part, maybe different object to passed in ITextDecoration
        private CDecoration mOwningDecoration;   // original decoration we are editing

        public CDecoration Decoration
        {
            get { return mOwningDecoration; }
        }

		#region public methods and overrides


        public void Clean()
        {
            if (mImageCopy != null)
            {
                mImageCopy.Dispose();
            }

            if (myPictureBox.Image != null)
            {
                myPictureBox.Image.Dispose();
            }

            ClearAlphaBitmap();
            ClearBitmap();
        }


        public AlphaBlendTextBox(CDecoration owningDecoration, PictureBox pb, CDecorationsManager manager, Bitmap bb)
		{
            ITextDecorationContainer itdc = owningDecoration as ITextDecorationContainer;
            if (itdc != null)
            {
                mTD = itdc.TextDecoration;
            }
            else
            {
                ManagedCore.Log.Error("AlphaBlendTextBox passed in decoration does not have ITextDecoration interface");
                return;
            }

			mPB =pb;
            this.mOwningDecoration = owningDecoration;
			mManager = manager;

            mFontString = mTD.TextStyle.FontName;
            mItalic = mTD.TextStyle.Italic;
            mBold = mTD.TextStyle.Bold;
            mColor = mTD.TextStyle.TextColor;
			mImageCopy =bb;
			
			float cw = pb.Image.Width;
			float w = pb.ClientRectangle.Width;

			float ch = pb.Image.Height;
			float h = pb.ClientRectangle.Height;

			if (h<1) return ;
			if (w<1) return ;

			float w2= w/cw;
			float h2 =h/ch;

			this.mXRat =w2;
			this.mYRat = h2;

			//this.mTopLeft.X = (int)(((float)pb.Image.Width)*d.mCoverageArea.X)+5;
			//this.mTopLeft.Y = (int)(((float)pb.Image.Height)*d.mCoverageArea.Y)+1;

            float font_size = mTD.TextStyle.FontSize * w2;
            RectangleF e2e = new RectangleF(0, 0, pb.Image.Width, pb.Image.Height);

            if (mTD.mText != "")
            {

                font_size = mTD.GetFontSizeForCoverageArea(e2e);
                font_size *= w2;
                mStoredFontSize = mTD.GetFontSizeForCoverageArea(); // NOT the same thing as myFontheight
            }
            else
            {
                mTD.mText = "abcdef";
                mTD.RecalcCoverageAreaForFontSize(font_size);
                font_size = mTD.GetFontSizeForCoverageArea(e2e);
                font_size *= w2;
                mStoredFontSize = mTD.GetFontSizeForCoverageArea(); // NOT the same thing as myFontheight
                mTD.mText = "";
            }

         
			this.myFontHeight = font_size ;

			// This call is required by the Windows.Forms Form Designer.
			//

			InitializeComponent();
			// TODO: Add any initialization after the InitializeComponent call

			this.BackColor = Color.FromArgb(128,128,128); 

			this.SetStyle(ControlStyles.UserPaint,false);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint,true);
			this.SetStyle(ControlStyles.DoubleBuffer,true);
 

			myPictureBox = new uPictureBox();
			this.Controls.Add(myPictureBox);
			myPictureBox.Dock = DockStyle.Fill;
			this.BorderStyle = BorderStyle.None;
			System.Drawing.FontStyle s = FontStyle.Regular;
			if (mItalic) s |= FontStyle.Italic;
			if (mBold) s |= FontStyle.Bold;

            if (mTD.TextStyle.Format.Alignment == System.Drawing.StringAlignment.Center)
			{
				this.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			}


            if (mTD.TextStyle.Format.Alignment == System.Drawing.StringAlignment.Far)
			{
				this.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			}

			this.ForeColor = mColor;
		
			this.Multiline=true;

			this.Font = new Font(mFontString,myFontHeight, s);
			this.WordWrap=false;

            this.Text = mTD.mText;

            int maxWidth = mMaxWidth;
            int maxHeight = mMaxHeight;

            this.Size = new Size(maxWidth, maxHeight);	
         
			// calculate top left/position if client text box;

			this.Left = (int)(((float)pb.ClientRectangle.Width)*mTD.CoverageArea.X)+5;
			this.Top = (int)(((float)pb.ClientRectangle.Height)*mTD.CoverageArea.Y)+1;
			
       
			if (this.TextAlign==System.Windows.Forms.HorizontalAlignment.Center)
			{
                this.Left -= (int)(maxWidth / 2.0f);
				this.Left += (int)(((float)pb.ClientRectangle.Width)*mTD.CoverageArea.Width/2);
			}

				
			if (this.TextAlign==System.Windows.Forms.HorizontalAlignment.Right)
			{
                this.Left -= (int)maxWidth;
				this.Left += (int)(((float)pb.ClientRectangle.Width)*mTD.CoverageArea.Width);
			}
             

			// calculate equivenlent position insise Image bitmap;

			mTopLeft.X = (int)(((float)this.Left)/this.mXRat);
			mTopLeft.Y = (int)(((float)this.Top)/this.mYRat);
		
			this.MouseDown +=new MouseEventHandler(this.MouseDownHandle);
			this.Cursor = System.Windows.Forms.Cursors.Arrow;

		}

		private void MouseDownHandle(object o, MouseEventArgs e)
		{
		//	Console.WriteLine("mouse down!");

			if (this.Text=="")
			{
				mPB.Focus();
				return;

			}

			RectangleF coverage = mTD.GetCoverageAreaForFontSize(mStoredFontSize);

			Point ptCursor = Cursor.Position; 
			ptCursor = mPB.PointToClient(ptCursor); 

			float w = mPB.ClientRectangle.Width;
			float h = mPB.ClientRectangle.Height;

			RectangleF tect = new RectangleF(w*coverage.X, 
											 h*coverage.Y,
											 w*coverage.Width, 
											 h*coverage.Height);

		
			if (tect.Contains(ptCursor)==false)
			{
				mPB.Focus();
			}
		}

		private void DrawBoundingBox(Graphics g)
		{
			float w = mPB.ClientRectangle.Width;
			float h = mPB.ClientRectangle.Height;

			RectangleF rr= new RectangleF(0,0,w,h);

			this.findCaret();
			string td = mTD.mText;
			if ((mCaretOnNewLine==true) ||( mTD.mText=="")) td+=" ";
            RectangleF coverage = mTD.GetCoverageAreaForFontSize(td, mStoredFontSize);

			RectangleF tect = new RectangleF((w*coverage.X)/this.mXRat, 
											 (h*coverage.Y)/this.mYRat,
											 (w*coverage.Width)/this.mXRat, 
											 (h*coverage.Height)/this.mYRat);

		//	Console.WriteLine("t="+tect.X+" "+ tect.Y+" "+ tect.Width+" "+ tect.Height);


			Pen p = new Pen(Color.Black,1);
			g.DrawRectangle(p,tect.X,tect.Y,tect.Width,tect.Height);
			p.Dispose();
		}

		protected override void OnResize(EventArgs e)
		{
      	    base.OnResize (e);

            int w = this.ClientRectangle.Width;
            int h = this.ClientRectangle.Height;

            // ignore font changes
            if (w < mMaxWidth) return;

            ClearBitmap();
            ClearAlphaBitmap();

			this.myBitmap = new Bitmap(w,h);
			this.myAlphaBitmap = new Bitmap(w,h);

			myUpToDate = false;
			this.Invalidate();
		}


		//Some of these should be moved to the WndProc later

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown (e);
			myUpToDate = false;
			do_caret=true;
			this.Invalidate();
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp (e);
			myUpToDate = false;
			do_caret=true;
			this.Invalidate();

		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress (e);
			do_caret=true;
			myUpToDate = false;

		//	if (char.IsControl(e.KeyChar)==true)
			{
			//this.mManager.RePaint();
			}

			this.Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp (e);
			this.Invalidate();
		}

		protected override void OnGiveFeedback(GiveFeedbackEventArgs gfbevent)
		{
			base.OnGiveFeedback (gfbevent);
			myUpToDate = false;
			do_caret=true;
			this.Invalidate();
		}


		protected override void OnMouseLeave(EventArgs e)
		{
			//found this code to find the current cursor location
			//at http://www.syncfusion.com/FAQ/WinForms/FAQ_c50c.asp#q597q

			Point ptCursor = Cursor.Position; 
			
			Form f = this.FindForm();
			if (f!=null)
			{
				ptCursor = f.PointToClient(ptCursor); 
				if( !this.Bounds.Contains(ptCursor) )  
					base.OnMouseLeave (e);
			}
		}		
		
					
		protected override void OnChangeUICues(UICuesEventArgs e)
		{
			base.OnChangeUICues (e);
			myUpToDate = false;
			do_caret=true;
			this.Invalidate();
		}
		
		
//--
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus (e);
			myCaretUpToDate = false;
			myUpToDate = false;
			do_caret=true;
			this.Invalidate();
			

			myTimer1 = new System.Windows.Forms.Timer(this.components);
			myTimer1.Interval = (int) win32.GetCaretBlinkTime(); //  usually around 500;
			myTimer1.Tick +=new EventHandler(myTimer1_Tick);
			myTimer1.Enabled = true;
				
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus (e);
			myCaretUpToDate = false;
			myUpToDate = false;
			this.Invalidate();
			myTimer1.Dispose();
		}

//--		

		protected override void OnFontChanged(EventArgs e)
		{
			if (this.myPaintedFirstTime)
				this.SetStyle(ControlStyles.UserPaint,false);

			base.OnFontChanged (e);

			if (this.myPaintedFirstTime)
				this.SetStyle(ControlStyles.UserPaint,true);

				
			myFontHeight = GetFontHeight();


			myUpToDate = false;
			this.Invalidate();
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged (e);
			myUpToDate = false;
			do_caret=true;
			this.Invalidate();
		}

		
		protected override void WndProc(ref Message m)
		{
            try
            {
                base.WndProc(ref m);

                // need to rewrite as a big switch

                if (m.Msg == win32.WM_PAINT)
                {
                    myPaintedFirstTime = true;

                    if (!myUpToDate || !myCaretUpToDate)
                        GetBitmaps();
                    myUpToDate = true;
                    myCaretUpToDate = true;

                    if (myPictureBox.Image != null) myPictureBox.Image.Dispose();
                    myPictureBox.Image = (Image)myAlphaBitmap.Clone();

                }

                else if (m.Msg == win32.WM_HSCROLL || m.Msg == win32.WM_VSCROLL)
                {
                    myUpToDate = false;
                    this.Invalidate();
                }

                else if (m.Msg == win32.WM_LBUTTONDOWN
                    || m.Msg == win32.WM_RBUTTONDOWN
                    || m.Msg == win32.WM_LBUTTONDBLCLK
                    //  || m.Msg == win32.WM_MOUSELEAVE  ///****
                    )
                {
                    myUpToDate = false;
                    this.Invalidate();
                }

                else if (m.Msg == win32.WM_MOUSEMOVE)
                {
                    if (m.WParam.ToInt32() != 0)  //shift key or other buttons
                    {
                        myUpToDate = false;
                        this.Invalidate();
                    }
                }
            }
            catch (Exception e)
            {
                ManagedCore.CDebugLog.GetInstance().Error("Exception occurred in AlphaBlendTextBox::WndProc: "+e.Message);
            }

		}


		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				//this.BackColor = Color.Pink;
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion		//end public method and overrides


		#region public property overrides

		public new BorderStyle BorderStyle
		{
			get {return base.BorderStyle;}
			set 
			{
				if (this.myPaintedFirstTime)
					this.SetStyle(ControlStyles.UserPaint,false);

				base.BorderStyle = value;
				
				if (this.myPaintedFirstTime)
					this.SetStyle(ControlStyles.UserPaint,true);

                ClearBitmap();
                ClearAlphaBitmap();
				myUpToDate = false;
				this.Invalidate();
			}
		}

		public  new Color BackColor
		{
			get
			{
				return Color.FromArgb(base.BackColor.R, base.BackColor.G, base.BackColor.B);
			}
			set
			{
				myBackColor = value;
				base.BackColor = value;
				myUpToDate = false;
			}
		}
		public override bool Multiline
		{
			get{return base.Multiline;}
			set
			{
				if (this.myPaintedFirstTime)
					this.SetStyle(ControlStyles.UserPaint,false);
				
				base.Multiline = value;

				if (this.myPaintedFirstTime)
					this.SetStyle(ControlStyles.UserPaint,true);

                ClearBitmap();
                ClearAlphaBitmap();
				myUpToDate = false;
				this.Invalidate();
			}
		}

		
		#endregion    //end public property overrides


		#region private functions and classes

		private int GetFontHeight()
		{
			Graphics g = this.CreateGraphics();
			SizeF sf_font = g.MeasureString("X",this.Font);
			g.Dispose();
			return  (int) sf_font.Height;
		}

        private void ClearBitmap()
        {
            if (myBitmap != null)
            {
                myBitmap.Dispose();
            }
            myBitmap = null;
        }

        private void ClearAlphaBitmap()
        {
            if (myAlphaBitmap != null)
            {
                myAlphaBitmap.Dispose();
            }
            myAlphaBitmap = null;
        }
		
		private void GetBitmaps()
		{
            if (this.ClientRectangle.Width <= 0 || this.ClientRectangle.Height <= 0)
            {
                return;
            }

			if (myBitmap == null
				|| myAlphaBitmap == null
				|| myBitmap.Width != Width 
				|| myBitmap.Height != Height		
				|| myAlphaBitmap.Width != Width 
				|| myAlphaBitmap.Height != Height)
			{
                ClearBitmap();
                ClearAlphaBitmap();
			}

		    if (myBitmap == null)
			{
			//	Console.WriteLine("2");

				myBitmap = new Bitmap(this.ClientRectangle.Width,this.ClientRectangle.Height);//(Width,Height);
				myUpToDate = false;
			//	myBitmap.Save("2.bmp");
			
			
			}


			if (!myUpToDate)
			{
			//	Console.WriteLine("1");
				//Capture the TextBox control window

				this.SetStyle(ControlStyles.UserPaint,false);
				
				win32.CaptureWindow(this,ref myBitmap);

				this.SetStyle(ControlStyles.UserPaint,true);
				this.SetStyle(ControlStyles.SupportsTransparentBackColor,true);
				this.BackColor = Color.FromArgb(myBackAlpha,myBackColor);


			//	Console.WriteLine("3");
				//myBitmap.Save("c:\\3.bmp");
			}
			else
			{
				//Console.WriteLine("2");
			}
			//--
			

		
			Rectangle r2 = new Rectangle(0,0,this.ClientRectangle.Width,this.ClientRectangle.Height);
			ImageAttributes tempImageAttr = new ImageAttributes();
						

			//Found the color map code in the MS Help

			ColorMap[] tempColorMap = new ColorMap[1];
			tempColorMap[0] = new ColorMap();
			tempColorMap[0].OldColor = Color.FromArgb(255,myBackColor); 
			tempColorMap[0].NewColor = Color.FromArgb(myBackAlpha,myBackColor);

			tempImageAttr.SetRemapTable(tempColorMap);


            ClearAlphaBitmap();
			myAlphaBitmap = new Bitmap(this.ClientRectangle.Width,this.ClientRectangle.Height);//(Width,Height);

			
			//Graphics tempGraphics1 = Graphics.FromImage(myAlphaBitmap);
			//tempGraphics1.DrawImage(myBitmap,r2,0,0,this.ClientRectangle.Width,this.ClientRectangle.Height,GraphicsUnit.Pixel,tempImageAttr);
			//tempGraphics1.Dispose();


			if (this.Text == mLastText && do_caret==false) return;
            if (this.mPB.Image != null) this.mPB.Image.Dispose();
			this.mPB.Image = (Bitmap)mImageCopy.Clone();
			this.mTD.mText = this.Text;
			mLastText = this.Text;

	
			Rectangle r3 = new Rectangle(this.mTopLeft.X,this.mTopLeft.Y,(int)(this.ClientRectangle.Width/this.mXRat), (int)(this.ClientRectangle.Height/this.mYRat));
		
			Graphics tempGraphics2 = Graphics.FromImage(this.mPB.Image);
		//	myBitmap.Save("c:\\stu.bmp");

			tempGraphics2.DrawImage(myBitmap,r3,0,0,this.ClientRectangle.Width,this.ClientRectangle.Height,GraphicsUnit.Pixel,tempImageAttr);
			DrawBoundingBox(tempGraphics2);

			tempGraphics2.Dispose();
			
			if (this.Focused && (this.SelectionLength == 0))
			{	
				if (myCaretState)
				{
                    Graphics tempGraphics3 = Graphics.FromImage(myAlphaBitmap);
					Point caret = this.findCaret();
					Pen p = new Pen(this.ForeColor,3);
					tempGraphics3.DrawLine(p,caret.X,caret.Y + 0,caret.X,caret.Y + myFontHeight);
					tempGraphics3.Dispose();
                    p.Dispose();
				}
						
			}

			do_caret=false;
		}


		private Point findCaret() 
		{
			mCaretOnNewLine=false;

			/*  Find the caret translated from code at 
			 * http://www.vb-helper.com/howto_track_textbox_caret.html
			 * 
			 * and 
			 * 
			 * http://www.microbion.co.uk/developers/csharp/textpos2.htm
			 * 
			 * Changed to EM_POSFROMCHAR
			 * 
			 * This code still needs to be cleaned up and debugged
			 * */

			Point pointCaret = new Point(0);
			int i_char_loc = this.SelectionStart;
			IntPtr pi_char_loc = new IntPtr(i_char_loc);

			int i_point = win32.SendMessage(this.Handle,win32.EM_POSFROMCHAR,pi_char_loc,IntPtr.Zero);
			pointCaret = new Point(i_point);

			/*
			if (i_char_loc == 0) 
			{
				pointCaret = new Point(0);

				if (this.TextAlign==System.Windows.Forms.HorizontalAlignment.Center)
				{
					pointCaret = new Point(this.ClientRectangle.Width/2);
				}
				if (this.TextAlign==System.Windows.Forms.HorizontalAlignment.Right)
				{
					pointCaret = new Point(this.ClientRectangle.Width);
				}
			}*/

			
			if (i_char_loc >= this.Text.Length)
			{
				pi_char_loc = new IntPtr(i_char_loc - 1);
				i_point = win32.SendMessage(this.Handle,win32.EM_POSFROMCHAR,pi_char_loc,IntPtr.Zero);
			//	Console.WriteLine("i="+i_point);
				if (i_point<0) 
				{
					i_point=1;
					if (this.TextAlign==System.Windows.Forms.HorizontalAlignment.Center)
					{
						i_point =this.ClientRectangle.Width/2;
					}
					if (this.TextAlign==System.Windows.Forms.HorizontalAlignment.Right)
					{
						i_point = (this.ClientRectangle.Width)-1;
					}
				}

				pointCaret = new Point(i_point);

				if (this.Text.Length!=0)
				{
					Graphics g = this.CreateGraphics();
					String t1 = this.Text.Substring(this.Text.Length-1,1) + "X";
					SizeF sizet1 = g.MeasureString(t1,this.Font);
					SizeF sizex  = g.MeasureString("X",this.Font);
					g.Dispose();
					int xoffset = (int)(sizet1.Width - sizex.Width);
					pointCaret.X = pointCaret.X + xoffset;

					if (i_char_loc == this.Text.Length)
					{
						String slast = this.Text.Substring(Text.Length-1,1);
						if (slast == "\n")
						{
							mCaretOnNewLine=true;
							pointCaret.X = 1;
							if (this.TextAlign==System.Windows.Forms.HorizontalAlignment.Center)
							{
								pointCaret.X = this.ClientRectangle.Width/2;
							}
							if (this.TextAlign==System.Windows.Forms.HorizontalAlignment.Right)
							{
								pointCaret.X = this.ClientRectangle.Width;
							}

							pointCaret.Y = pointCaret.Y + (int)myFontHeight;
						}
					}
				}

			}
		
			return pointCaret;
		}


		private void myTimer1_Tick(object sender, EventArgs e)
		{
		//	Timer used to turn caret on and off for focused control

			myCaretState = !myCaretState;
			myCaretUpToDate = false;
			do_caret=true;

			this.Invalidate();
		}


		private class uPictureBox : PictureBox 
		{
			public uPictureBox() 
			{
				this.SetStyle(ControlStyles.Selectable,false);
				this.SetStyle(ControlStyles.UserPaint,true);
				this.SetStyle(ControlStyles.AllPaintingInWmPaint,true);
				this.SetStyle(ControlStyles.DoubleBuffer,true);

				this.Cursor = null;
				this.Enabled = true; 
				this.SizeMode = PictureBoxSizeMode.Normal;
				
			}




			//uPictureBox
			protected override void WndProc(ref Message m)
			{
				try
				{
					if (m.Msg == win32.WM_LBUTTONDOWN 
						|| m.Msg == win32.WM_RBUTTONDOWN
						|| m.Msg == win32.WM_LBUTTONDBLCLK
						|| m.Msg == win32.WM_MOUSELEAVE
						|| m.Msg == win32.WM_MOUSEMOVE )
					{
						//Send the above messages back to the parent control
						win32.PostMessage(this.Parent.Handle,(uint) m.Msg,m.WParam,m.LParam);
					}

					else if (m.Msg == win32.WM_LBUTTONUP)
					{
						//??  for selects and such
						this.Parent.Invalidate();
					}


					base.WndProc (ref m);
				}
				catch (Exception e)
				{
                    ManagedCore.CDebugLog.GetInstance().Error("Exception occurred in uPictureBox::WndProc: "+e.Message);
				}
			}


		}   // End uPictureBox Class


		#endregion  // end private functions and classes


		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion


		#region New Public Properties

		[
		Category("Appearance"),
		Description("The alpha value used to blend the control's background. Valid values are 0 through 255."),
		Browsable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)

		]
		public int BackAlpha
		{
			get { return myBackAlpha; }
			set 
			{
				int v = value;
				if (v > 255)
					v = 255;
				myBackAlpha = v;
				myUpToDate = false; 
				Invalidate();
			}
		}

		#endregion



	}  // End AlphaTextBox Class


}  // End namespace ZBobb


//----
