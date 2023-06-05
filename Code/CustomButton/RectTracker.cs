using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DVDSlideshow.GraphicsEng;
using DVDSlideshow;

namespace CustomButton
{
	public class CRectTracker
	{
		private Rectangle m_TopLeft;
		private Rectangle m_TopCenter;
		private Rectangle m_TopRight;
		private Rectangle m_CenterLeft;
		private Rectangle m_CenterRight;
		private Rectangle m_BottomLeft;
		private Rectangle m_BottomCenter;
		private Rectangle m_BottomRight;
        private RectangleF m_Rotation;

        // used to work backwards from tracker point, to original rectagle point
        private Rectangle m_TopLeftNonRotate;
        private Rectangle m_TopCenterNonRotate;
        private Rectangle m_TopRightNonRotate;
        private Rectangle m_CenterLeftNonRotate;
        private Rectangle m_CenterRightNonRotate;
        private Rectangle m_BottomLeftNonRotate;
        private Rectangle m_BottomCenterNonRotate;
        private Rectangle m_BottomRightNonRotate;

		private int m_nOffset = 0;
        private int m_nSize = 8;
		private bool mDrawDotted = true;
		private bool mDrawEnlargeInside = false;
        private const int mMaxPixelsDimensionToShowRotateIcon = 100;

        private static CImage mRotateImage = null;

        public static Cursor RotateCursor = null;

        public Point TopLeftOffset
        {
            get { return new Point(m_TopLeftNonRotate.X - m_TopLeft.X, m_TopLeftNonRotate.Y - m_TopLeft.Y); }
        }

        public Point TopCenterOffset
        {
            get { return new Point(m_TopCenterNonRotate.X - m_TopCenter.X, m_TopCenterNonRotate.Y - m_TopCenter.Y); }
        }

        public Point TopRightOffset
        {
            get { return new Point(m_TopRightNonRotate.X - m_TopRight.X, m_TopRightNonRotate.Y - m_TopRight.Y); }
        }

        public Point CenterRightOffset
        {
            get { return new Point(m_CenterRightNonRotate.X - m_CenterRight.X, m_CenterRightNonRotate.Y - m_CenterRight.Y); }
        }

        public Point BottomRightOffset
        {
            get { return new Point(m_BottomRightNonRotate.X - m_BottomRight.X, m_BottomRightNonRotate.Y - m_BottomRight.Y); }
        }

        public Point BottomCenterOffset
        {
            get { return new Point(m_BottomCenterNonRotate.X - m_BottomCenter.X, m_BottomCenterNonRotate.Y - m_BottomCenter.Y); }
        }

        public Point BottomLeftOffset
        {
            get { return new Point(m_BottomLeftNonRotate.X - m_BottomLeft.X, m_BottomLeftNonRotate.Y - m_BottomLeft.Y); }
        }

        public Point CenterLeftOffset
        {
            get { return new Point(m_CenterLeftNonRotate.X - m_CenterLeft.X, m_CenterLeftNonRotate.Y - m_CenterLeft.Y); }
        }
       
        public int Offset
        {
            get { return m_nOffset; }
            set { m_nOffset = value; }
        }

        private bool mShowRotation = false;

        //*******************************************************************
        public bool ShowRotation
        {
            get { return mShowRotation; }
            set { mShowRotation = value; }
        }

        //*******************************************************************
		public CRectTracker(bool draw_dotted, bool draw_enlarge_inside)
		{
            if (mRotateImage == null)
            {
                mRotateImage = new CImage(CGlobals.GetGuiImagesDirectory() + "\\rotate.png", false);
            }


			mDrawDotted = draw_dotted;
			if (mDrawDotted==true)
			{
				int i=1;
				i++;
			}

			mDrawEnlargeInside = draw_enlarge_inside;
		}


        //*******************************************************************
        public void DrawSelectionGhostRect(Rectangle rc, Image bm)
        {
            DrawSelectionGhostRect(rc, 0, bm);
        }
 
        //*******************************************************************
        private static PointF[] ApplyRotation(RectangleF r1, float rotation)
        {
            using (Matrix mat = new Matrix())
            {
                mat.Rotate(rotation);

                float offsetx = (((float)r1.X) + (((float)r1.Width) / 2.0f));
                float offsety = (((float)r1.Y) + (((float)r1.Height) / 2.0f));

                PointF[] points = new PointF[4];

                points[0] = new PointF(r1.X - offsetx, r1.Y - offsety);  // top  left
                points[1] = new PointF((r1.X + r1.Width) - offsetx, r1.Y - offsety); // top right
                points[2] = new PointF((r1.X + r1.Width) - offsetx, (r1.Y + r1.Height) - offsety);
                points[3] = new PointF(r1.X - offsetx, (r1.Y + r1.Height) - offsety);

                mat.TransformVectors(points);

                for (int i = 0; i < 4; i++)
                {
                    points[i].X += offsetx;
                    points[i].Y += offsety;
                }

                return points;
            }
        }


        //*******************************************************************
        private static PointF ApplyRotation(PointF point, float rotation, RectangleF r1)
        {
            using (Matrix mat = new Matrix())
            {
                mat.Rotate(rotation);

                float offsetx = (((float)r1.X) + (((float)r1.Width) / 2.0f));
                float offsety = (((float)r1.Y) + (((float)r1.Height) / 2.0f));

                PointF[] points = new PointF[1];

                points[0] = point;
                points[0].X -= offsetx;
                points[0].Y -= offsety;

                mat.TransformVectors(points);
                
                points[0].X += offsetx;
                points[0].Y += offsety;
          
                return points[0];
            }
        }


        /*
        //*******************************************************************
        public static PointF ApplyInverseToPoint(PointF point, float rotation, RectangleF r1)
        {
            using (Matrix mat = new Matrix())
            {
                mat.Rotate(-rotation);

                float offsetx = (((float)r1.X) + (((float)r1.Width) / 2.0f));
                float offsety = (((float)r1.Y) + (((float)r1.Height) / 2.0f));

                offsetx = -offsetx;
                offsety = -offsety;

                point.X += offsetx;
                point.Y += offsety;

                PointF[] points = new PointF[1];
                points[0] = point;

                mat.TransformVectors(points);

                point = points[0];

                point.X -= offsetx;
                point.Y -= offsety;

                return point;
            }
        }
         */

        //*******************************************************************
		public void DrawSelectionGhostRect(Rectangle rc, float rotation, Image bm)
		{
			// Draws dotted lines

            DrawSelectionGhostRectForRotation(rc, rotation, bm);

            /*
            if (rotation != 0)
            {
                DrawSelectionGhostRectForRotation(rc, rotation, bm);
            }
             */


		}

        //*******************************************************************
        private void DrawSelectionGhostRectForRotation(Rectangle rc, float rotation, Image bm)
        {
            using (Graphics g = Graphics.FromImage(bm))
            {
                g.InterpolationMode = InterpolationMode.Bicubic;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                Pen p = new Pen(new SolidBrush(Color.FromArgb(255, 255, 255)), 2);

                p.DashStyle = DashStyle.Dot;
                if (this.mDrawDotted == true)
                {
                    PointF[] points = ApplyRotation(rc, rotation);

                    g.DrawLine(p, points[0], points[1]);
                    g.DrawLine(p, points[1], points[2]);
                    g.DrawLine(p, points[2], points[3]);
                    g.DrawLine(p, points[3], points[0]);

                    //ControlPaint.DrawFocusRectangle(g, rc, Color.Black, Color.Transparent);
                }
            }
        }
        //*******************************************************************
        public void DrawHighlightObject(RectangleF rc, Image image)
        {
            // Draws yellow box around object (used for slideshow selected from menu)
            if (image == null) return;
            using (Graphics g = Graphics.FromImage(image))
            {
                using (Pen p = new Pen(Color.Yellow, (int) ((float)image.Width / 120.0f)))
                {
                    rc.Inflate(3, 3);
                    g.DrawRectangle(p, rc.X, rc.Y, rc.Width, rc.Height);
                }
            }
        }

        //*******************************************************************
        private PointF[] RotateAntiClockwise(PointF[] points)
        {
            PointF a = points[0];
            points[0] = points[3];
            points[3] = points[2];
            points[2] = points[1];
            points[1] = a;
            return points;
        }

        //*******************************************************************
        private PointF[] RotateClockwise(PointF[] points)
        {
            PointF a = points[0];
            points[0] = points[1];
            points[1] = points[2];
            points[2] = points[3];
            points[3] = a;
            return points;
        }

        //*******************************************************************
        private Rectangle MidPoint(Rectangle a, Rectangle b)
        {
            int x = a.X + ((b.X - a.X) / 2);
            int y = a.Y + ((b.Y - a.Y) / 2);

            return new Rectangle(x, y, a.Width, a.Height);
        }


        //*******************************************************************
		public void InitTrackerRects(Rectangle rc, float rotation)
		{
            while (rotation < -180)
            {
                rotation += 360;
            }
            while (rotation > 180)
            {
                rotation -=360;
            }
             
            PointF[] points = ApplyRotation(rc, rotation);

			Size sz = new Size(m_nSize, m_nSize);

            // tracker points must stay in the qurrent quadrant, so round to neerest 90 degrees
            if (rotation > 45)
            {
                points = RotateAntiClockwise(points);
            }
            if (rotation > (45 + 90))
            {
                points = RotateAntiClockwise(points);
            }

            if (rotation < -45)
            {
                points = RotateClockwise(points);
            }

            if (rotation < (-45 -90))
            {
                points = RotateClockwise(points);
            }


            m_TopLeft = new Rectangle(new Point(((int)points[0].X) - m_nOffset, ((int)points[0].Y) - m_nOffset), sz);
            m_TopRight = new Rectangle(new Point(((int)points[1].X), ((int)points[1].Y) - m_nOffset), sz);
            m_TopCenter = MidPoint(m_TopLeft, m_TopRight);

            m_BottomLeft = new Rectangle(new Point(((int)points[3].X) - m_nOffset, ((int)points[3].Y)), sz);	
            m_BottomRight = new Rectangle(new Point(((int)points[2].X), ((int)points[2].Y) ), sz);

            m_BottomCenter = MidPoint(m_BottomLeft, m_BottomRight);

            m_CenterLeft = MidPoint(m_BottomLeft, m_TopLeft);
            m_CenterRight = MidPoint(m_BottomRight, m_TopRight);

            
            m_TopLeftNonRotate = new Rectangle(new Point(rc.X - m_nOffset, rc.Y - m_nOffset), sz);
            m_TopCenterNonRotate = new Rectangle(new Point(rc.X + (rc.Width / 2) - (m_nOffset / 2), rc.Y - m_nOffset), sz);
            m_TopRightNonRotate = new Rectangle(new Point(rc.X + rc.Width, rc.Y - m_nOffset), sz);

            m_CenterLeftNonRotate = new Rectangle(new Point(rc.X - m_nOffset, rc.Y + (rc.Height / 2) - (m_nOffset / 2)), sz);
            m_CenterRightNonRotate = new Rectangle(new Point(rc.X + rc.Width, rc.Y + (rc.Height / 2) - (m_nOffset / 2)), sz);

            m_BottomLeftNonRotate = new Rectangle(new Point(rc.X - m_nOffset, rc.Y + rc.Height), sz);
            m_BottomCenterNonRotate = new Rectangle(new Point(rc.X + (rc.Width / 2) - (m_nOffset / 2), rc.Y + rc.Height), sz);
            m_BottomRightNonRotate = new Rectangle(new Point(rc.X + rc.Width, rc.Y + rc.Height), sz);

             
            PointF rotationPoint = new Point(rc.X + (rc.Width / 2) - (16 / 2), rc.Y + (rc.Height / 10));
            m_Rotation = new RectangleF(ApplyRotation(rotationPoint, rotation, rc), new Size(16,16));
            
            
            int diff =m_nSize - m_nOffset;
         
            if (diff != 0)
            {
              
                m_BottomLeft.Y -= diff;
                m_BottomCenter.Y -= diff;
                m_BottomRight.Y -= diff;
                m_BottomRight.X -= diff;
                m_CenterRight.X -= diff;
                m_TopRight.X -= diff;
            }
		}

        //*******************************************************************
        public void DrawSelectionTrackers(Rectangle rc, Image bm)
        {
            DrawSelectionTrackers(rc, 0, bm);
        }

        //*******************************************************************
		public void DrawSelectionTrackers(Rectangle rc, float rotation, Image bm)
		{
			// draw resize boxes around image
            using (Brush b = new SolidBrush(Color.Yellow))
            {
                InitTrackerRects(rc, rotation);

                // drawing to image
                using (Graphics g = Graphics.FromImage(bm))
                {
                    g.FillRectangle(b, m_TopLeft);
                    g.FillRectangle(b, m_TopCenter);
                    g.FillRectangle(b, m_TopRight);
                    g.FillRectangle(b, m_CenterLeft);
                    g.FillRectangle(b, m_CenterRight);
                    g.FillRectangle(b, m_BottomLeft);
                    g.FillRectangle(b, m_BottomCenter);
                    g.FillRectangle(b, m_BottomRight);
                    if (mShowRotation && rc.Width > mMaxPixelsDimensionToShowRotateIcon && rc.Height > mMaxPixelsDimensionToShowRotateIcon)
                    {
                        g.DrawImage(mRotateImage.GetRawImage(), new PointF(this.m_Rotation.X, this.m_Rotation.Y));
                    }
                }
            }
		}

        //*******************************************************************
		public Cursor CursorCheck(Rectangle rc, Point pt, ref int enResult)
		{
			if(m_TopCenter.Contains(pt))
			{
				enResult = 1;
				return Cursors.SizeNS;
			}
			if(m_CenterRight.Contains(pt))
			{
				enResult = 2;
				return Cursors.SizeWE;
			}
			else if(m_BottomCenter.Contains(pt))
			{
				enResult = 3;
				return Cursors.SizeNS;
			}
			else if(m_CenterLeft.Contains(pt))
			{
				enResult = 4;
				return Cursors.SizeWE;
			}
			else if(m_TopLeft.Contains(pt))
			{
				enResult = 5;
				return Cursors.SizeNWSE;
			}
			else if(m_TopRight.Contains(pt))
			{
				enResult = 6;
				return Cursors.SizeNESW;
			}
			else if(m_BottomLeft.Contains(pt) )
			{
				enResult = 7;
				return Cursors.SizeNESW;
			}
			else if(m_BottomRight.Contains(pt))
			{
				enResult = 8;
				return Cursors.SizeNWSE;
			}
            else if (mShowRotation && 
                     rc.Width > mMaxPixelsDimensionToShowRotateIcon &&
                     rc.Height > mMaxPixelsDimensionToShowRotateIcon &&
                     this.m_Rotation.Contains(pt))
            {
                enResult = 9;
                // create any bitmap

                if (RotateCursor == null)
                {
                    Bitmap b = new Bitmap(32, 32);
                    CImage rot1 = new CImage(CGlobals.GetGuiImagesDirectory() + "\\rotateicon.png", false);
                    Graphics g = Graphics.FromImage(b);
                    g.DrawImage(rot1.GetRawImage(), new PointF(0, 0));
                    g.Dispose();
                    IntPtr ptr = b.GetHicon();
                    RotateCursor = new Cursor(ptr);
                  
                }

                return RotateCursor;
            }
            else if (rc.Contains(pt))
            {
                return Cursors.Hand;
            }

            else
                return Cursors.Default;
		}
	}
}
