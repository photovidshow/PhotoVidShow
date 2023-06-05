using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using DVDSlideshow;

namespace BitmapButton
{
	public class SimpleBitmapButton : Button
	{
		/*
		enum btnState
		{
			BUTTON_UP=0,
			BUTTON_DOWN=1,
			BUTTON_FOCUSED=2,
			BUTTON_MOUSE_ENTER=3,
			BUTTON_DISABLED=4,
		}
		*/

		bool mMouseEnter=false;
		bool mMouseDown=false;
		public Label mLikedLabel = null;

		public SimpleBitmapButton()
		{
			// enable double buffering.  Must be done by a derived class
			SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);

			// initialize event handlers
			Paint+=new PaintEventHandler(BitmapButton_Paint);
			MouseDown+=new MouseEventHandler(BitmapButton_MouseDown);
			MouseUp+=new MouseEventHandler(BitmapButton_MouseUp);
		//	GotFocus+=new EventHandler(BitmapButton_GotFocus);
		//	LostFocus+=new EventHandler(BitmapButton_LostFocus);
			MouseEnter+=new EventHandler(BitmapButton_MouseEnter);
			MouseLeave+=new EventHandler(BitmapButton_MouseLeave);
		//	KeyDown+=new KeyEventHandler(BitmapButton_KeyDown);
		//	KeyUp+=new KeyEventHandler(BitmapButton_KeyUp);
			//EnabledChanged+=new EventHandler(BitmapButton_EnabledChanged);
		}


		/*
		protected void DrawRegion()
		{
			int X = this.Width;
			int Y = this.Height;

			rects0 = new Rectangle[2];
			rects0[0] = new Rectangle(2, 4, 2, Y-8);
			rects0[1] = new Rectangle(X-4, 4, 2, Y-8);

			rects1 = new Rectangle[8];
			rects1[0] = new Rectangle(2, 1, 2, 2); 
			rects1[1] =	new Rectangle(1, 2, 2, 2);
			rects1[2] =	new Rectangle(X-4, 1, 2, 2);
			rects1[3] =	new Rectangle(X-3, 2, 2, 2);
			rects1[4] =	new Rectangle(2, Y-3, 2, 2);
			rects1[5] =	new Rectangle(1, Y-4, 2, 2);
			rects1[6] =	new Rectangle(X-4, Y-3, 2, 2);
			rects1[7] =	new Rectangle(X-3, Y-4, 2, 2);
			
			Point[] points = {
								 new Point(1, 0),
								 new Point(X-1, 0),
								 new Point(X-1, 1),
								 new Point(X, 1),
								 new Point(X, Y-1),
								 new Point(X-1, Y-1),
								 new Point(X-1, Y),
								 new Point(1, Y),
								 new Point(1, Y-1),
								 new Point(0, Y-1),
								 new Point(0, 1),
								 new Point(1, 1)};

		}
*/



		private void BitmapButton_Paint(object sender, PaintEventArgs e)
		{

			if (mLikedLabel!=null)
			{	
				mLikedLabel.BackColor = Color.FromArgb(197,212,234);
				mLikedLabel.ForeColor = Color.Black;
			}

			Graphics gr=e.Graphics;
		
			int width = Size.Width;
			int height = Size.Height;

			int color_off=0;

			if (mMouseEnter==true)
			{
				color_off=10;
			}

			if (mMouseDown==true)
			{
				color_off=-20;
			}

            Color back_color = Color.FromArgb(BackColor.R, BackColor.G, BackColor.B);

            try
            {

                int bcr1 = BackColor.R + color_off;
                int bcg1 = BackColor.G + color_off;
                int bcb1 = BackColor.B + color_off;
                if (bcr1 > 255) bcr1 = 255;
                if (bcg1 > 255) bcg1 = 255;
                if (bcb1 > 255) bcb1 = 255;
                if (bcr1 < 0) bcr1 = 0;
                if (bcg1 < 0) bcg1 = 0;
                if (bcb1 < 0) bcb1 = 0;

                back_color = Color.FromArgb(bcr1, bcg1, bcb1);
            }
            catch
            {
            }

			Brush b = new SolidBrush(back_color);
			gr.FillRectangle(b,0,0,width,height);

			int x_offset=0;
			int y_offset=0;
			if (mMouseDown==true)
			{
				x_offset=1;
				y_offset=1;
			}

			Bitmap image2 = (Bitmap)Image;

			if (this.Enabled==false)
			{
				// draw grey version of self
				image2 = (Bitmap) Image.Clone();
				DVDSlideshow.BitmapUtils.FadeToBackColor(image2,this.BackColor);

			}

	//		image2.HorizontalResolution =96;

		//	gr.DrawImage(image2, 2+x_offset, 2+y_offset, new Rectangle(0,0,Image.Width,Image.Height), GraphicsUnit.Pixel);

			
		//	gr.DrawImage(image2, 0, 0, new Rectangle(0,0,40,40), GraphicsUnit.Pixel);

			if (this.FlatStyle != FlatStyle.Flat)
			{
				gr.DrawImage(image2,new Rectangle(2+x_offset,0+y_offset,this.Width-4,this.Height-13),
					new Rectangle(0,0, Image.Width,Image.Height), GraphicsUnit.Pixel);
			}
			else
			{
				gr.DrawImage(image2,new Rectangle(2+x_offset,2+y_offset,this.Width-4,this.Height-4),
					new Rectangle(0,0, Image.Width,Image.Height), GraphicsUnit.Pixel);
			}



			if  (this.Enabled==false)
			{
				return ;
			}

			if (mMouseEnter==true||
				mMouseDown==true)
			{
				int X = width-1;
				int Y = height-1;

				Rectangle [] rects0 = new Rectangle[2];
				rects0[0] = new Rectangle(2, 4, 2, Y-8);
				rects0[1] = new Rectangle(X-4, 4, 2, Y-8);

				Rectangle [] rects1 = new Rectangle[8];
				rects1[0] = new Rectangle(2, 1, 2, 2); 
				rects1[1] =	new Rectangle(1, 2, 2, 2);
				rects1[2] =	new Rectangle(X-4, 1, 2, 2);
				rects1[3] =	new Rectangle(X-3, 2, 2, 2);
				rects1[4] =	new Rectangle(2, Y-3, 2, 2);
				rects1[5] =	new Rectangle(1, Y-4, 2, 2);
				rects1[6] =	new Rectangle(X-4, Y-3, 2, 2);
				rects1[7] =	new Rectangle(X-3, Y-4, 2, 2);
			
				Point[] points = {
									 new Point(1, 0),
									 new Point(X-1, 0),
									 new Point(X-1, 1),
									 new Point(X, 1),
									 new Point(X, Y-1),
									 new Point(X-1, Y-1),
									 new Point(X-1, Y),
									 new Point(1, Y),
									 new Point(1, Y-1),
									 new Point(0, Y-1),
									 new Point(0, 1),
									 new Point(1, 1)};

				int border_offset =+10;
				if (mMouseDown)
				{
					if (mLikedLabel!=null)
					{
						mLikedLabel.BackColor = Color.FromArgb(177,192,214);
						mLikedLabel.ForeColor = Color.White;
					}
					


					border_offset =-30;
				}

				int bc2r = System.Drawing.Color.LightSlateGray.R+border_offset;
				int bc2g = System.Drawing.Color.LightSlateGray.G+border_offset;
				int bc2b = System.Drawing.Color.LightSlateGray.B+border_offset;

				if (bc2r>255) bc2r=255;
				if (bc2g>255) bc2g=255;
				if (bc2b>255) bc2b=255;
                if (bc2r < 0) bc2r = 0;
                if (bc2g < 0) bc2g = 0;
                if (bc2b < 0) bc2b = 0;

				Color border_color = Color.FromArgb(bc2r,bc2g,bc2b);
												

				Pen p = new Pen(border_color);
				gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
				gr.DrawLines(p, points);

			}

		}

		private void BitmapButton_MouseDown(object sender, MouseEventArgs e)
		{
			mMouseDown=true;
			Invalidate();
		}

		private void BitmapButton_MouseUp(object sender, MouseEventArgs e)
		{
			mMouseDown=false;
			Invalidate();
		}


		private void BitmapButton_MouseEnter(object sender, EventArgs e)
		{
			mMouseEnter=true;
			Invalidate();
		}

		private void BitmapButton_MouseLeave(object sender, EventArgs e)
		{
			mMouseEnter=false;
			Invalidate();
		}
	}
}