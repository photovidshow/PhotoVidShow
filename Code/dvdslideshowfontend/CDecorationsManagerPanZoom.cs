using System;
using System.Collections.Generic;
using System.Text;
using DVDSlideshow;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace dvdslideshowfontend
{
    public class CDecorationsManagerPanZoom
    {
        private static float mSafeArea = 20;

	    //*******************************************************************
		public static void DrawPanZoomRectangles(CImageSlide forSlide, Image pictureBox, Form1 mainWindow)
		{
            using (Graphics g = Graphics.FromImage(pictureBox))
            {
                StringFormat newStringFormat = new StringFormat();

                RectangleF f = forSlide.PanZoom.StartArea;

                Color redTransparentColor = Color.FromArgb(45, 175, 45, 45);
                Color greenTransparentColor = Color.FromArgb(45, 45, 175, 45);

                int width = pictureBox.Width;
                int height = pictureBox.Height;

                float x = f.X * (float)width;
                float y = f.Y * (float)height;
                float w = f.Width * (float)width;
                float h = f.Height * (float)height;

                PointF centreStartGreen = new PointF(x + (w/2), y+ (h/2));

                Pen greenPen = new Pen(Color.Green);
                Brush greenTransparentBrush = new SolidBrush(greenTransparentColor);


                Matrix m = new Matrix();
                m.RotateAt(forSlide.PanZoom.StartRotation, centreStartGreen);

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddRectangle(new RectangleF(x, y, w, h));
                    path.Transform(m);

                    // draw rotated point
                    g.FillPath(greenTransparentBrush, path);
                    g.DrawPath(greenPen, path);
                }

                greenPen.Dispose();
                greenTransparentBrush.Dispose();

                Brush greenBrush = new SolidBrush(Color.FromArgb(200, 0, 255, 0));
                Font bigfont = new Font("Ariel", 14);

                g.TranslateTransform(centreStartGreen.X-5, centreStartGreen.Y-5);
                g.RotateTransform(forSlide.PanZoom.StartRotation);
                g.TranslateTransform(-(centreStartGreen.X-5), -(centreStartGreen.Y-5));
                g.DrawString("Start Region", bigfont, greenBrush, x, y, newStringFormat);
                g.ResetTransform();

                greenBrush.Dispose();

                f = forSlide.PanZoom.EndArea;

                x = f.X * (float)width;
                y = f.Y * (float)height;
                w = f.Width * (float)width;
                h = f.Height * (float)height;

                PointF centreEndRed = new PointF(x + (w / 2), y + (h / 2));

                Pen redPen = new Pen(Color.Red);
                Brush redTransparentBrush = new SolidBrush(redTransparentColor);

                m = new Matrix();
                m.RotateAt(forSlide.PanZoom.EndRotation, centreEndRed);

                using (GraphicsPath path = new GraphicsPath())
                {

                    path.AddRectangle(new RectangleF(x, y, w, h));
                    path.Transform(m);

                    // draw rotated point
                    g.FillPath(redTransparentBrush, path);
                    g.DrawPath(redPen, path);
                }

                redTransparentBrush.Dispose();
                redPen.Dispose();

                Brush redBrush = new SolidBrush(Color.FromArgb(200, 255, 0, 0));
                if (f.Width > 0.2 &&
                    f.Height > 0.2)
                {
                    g.TranslateTransform(centreEndRed.X - 5, centreEndRed.Y - 5);
                    g.RotateTransform(forSlide.PanZoom.EndRotation);
                    g.TranslateTransform(-(centreEndRed.X - 5), -(centreEndRed.Y - 5));
                    g.DrawString("End Region", bigfont, redBrush, x, y, newStringFormat);
                    g.ResetTransform();
                }

                redBrush.Dispose();
            }
		}
    }
}

