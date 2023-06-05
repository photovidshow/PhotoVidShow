using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace DVDSlideshow
{
    public class GDITextDrawer
    {
        //*******************************************************************
        public void DrawString(Graphics gp, string text, float x, float y, CTextStyle style)
        {
            float alligment_offset = 0;

            if (style.Format.Alignment == System.Drawing.StringAlignment.Center)
            {
                alligment_offset = gp.VisibleClipBounds.Width / 2;
            }

            if (style.Format.Alignment == System.Drawing.StringAlignment.Far)
            {
                alligment_offset = gp.VisibleClipBounds.Width;
            }

            x += alligment_offset;

            using (DisposableObject<Font> font = new DisposableObject<Font>(GenerateFont(style)))
            {
                gp.SmoothingMode = SmoothingMode.AntiAlias;
                gp.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;


                if (style.Shadow == true)
                {
                    switch (style.ShadowStyle)
                    {
                        case CTextStyle.ShadowType.Solid:
                            {
                                DrawSolidShadow(gp, font, text, x, y, style);
                                break;
                            }
                        case CTextStyle.ShadowType.Diffuse:
                            {
                                DrawShadowDiffuse(gp, font, text, x, y, style);
                                break;
                            }
                        default:
                            {
                                // DrawShadowBlur();
                                break;
                            }
                    }
                }

                float outline_length = style.OutlineLength * (font.Object.SizeInPoints / 30.0f);

                // draw glow?
                if (style.Outline == true && outline_length > 1)
                {
                    DrawOutline(gp, font, text, x, y, style, outline_length);
                }

                DrawText(gp, font, text, x, y, style, outline_length);
            }

         //   gp.FillRectangle(new SolidBrush(Color.Red), 0, 0, 1000, 1000);
        }

        //*******************************************************************
        //
        // Centre must be black
        //
        //*******************************************************************
        private void DrawShadowDiffuse(Graphics gp, Font font, string text, float x, float y, CTextStyle style)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                int size = (int)((gp.DpiX * font.SizeInPoints / 72.0f)+0.499f);
                if (size > 0)
                {
                    path.AddString(text,
                            new FontFamily(font.Name),
                            (int)font.Style,
                            size,
                            new PointF(x + style.ShadowOffsetX, y + style.ShadowOffsetY),
                            style.Format);
                }

                int l = style.ShadowDiffuseLength;

                for (int i = 0; i < l; i++)
                {
                    Color nextColor = Color.FromArgb(style.ShadowAlpha, style.ShadowColor);

                    using (Pen pen = new Pen(nextColor, i))
                    {
                        pen.LineJoin = LineJoin.Round;
                        gp.DrawPath(pen, path);
                    }
                }


                gp.SmoothingMode = SmoothingMode.None;
                gp.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;

                using (SolidBrush sb = new SolidBrush(Color.FromArgb(255, 0, 0, 0)))
                {
                    gp.FillPath(sb, path);
                }

                gp.SmoothingMode = SmoothingMode.AntiAlias;
                gp.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            }

          
        }

        //*******************************************************************
        private void DrawSolidShadow(Graphics gp, Font font, string text, float x, float y, CTextStyle style)
        {
            float shadoffsetX = style.ShadowOffsetX * (font.SizeInPoints / 30.0f);  // shadow is ratio of font size
            float shadoffsetY = style.ShadowOffsetY * (font.SizeInPoints / 30.0f);

            using (GraphicsPath path = new GraphicsPath())
            {
                int size = (int)((gp.DpiX * font.SizeInPoints / 72.0f)+0.499f);
                if (size > 0)
                {
                    path.AddString(text,
                            new FontFamily(font.Name),
                            (int)font.Style,
                            size,
                            new PointF(x + shadoffsetX, y + shadoffsetY),
                            style.Format);
                }

                using (SolidBrush sb = new SolidBrush(Color.FromArgb(style.ShadowAlpha, style.ShadowColor)))
                {
                    gp.FillPath(sb, path);
                }
            }
        }


        //*******************************************************************
        //
        // Draw glow outline
        //
        //*******************************************************************
        private void DrawOutline(Graphics gp, Font font, string text, float x, float y, CTextStyle style, float outlinelength)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                int size = (int)((gp.DpiX * font.SizeInPoints / 72.0f)+0.499f);
                if (size > 0)
                {
                    path.AddString(text,
                            new FontFamily(font.Name),
                            (int)font.Style,
                            size,
                            new PointF(x, y),
                            style.Format);
                }

                float alpha = ((float)style.OutlineAlpha) / (font.SizeInPoints / 30.0f);
                int i_alpha = (int)(alpha + 0.4999f);

                if (i_alpha < 0) i_alpha = 0;
                if (i_alpha > 255) i_alpha = 255;

                for (float i = 0; i < outlinelength; i+=1.0f )
                {
                    
                    Color nextColor = Color.FromArgb(i_alpha, style.OutlineColor);

                    using (Pen pen = new Pen(nextColor, i))
                    {
                        pen.LineJoin = LineJoin.Round;
                        gp.DrawPath(pen, path);
                    }
                }
            }
        }

        //*******************************************************************
        //
        // Draw outline
        //
        //*******************************************************************
        private void DrawText(Graphics gp, Font font, string text, float x, float y, CTextStyle style, float outlinelength)
        {
            using (DisposableObject<Brush> brush = new DisposableObject<Brush>())
            {
                if (style.Gradient == false)
                {
                    brush.Assign(new SolidBrush(style.TextColor));
                }
                else
                {
                    // find accurate height of font without shadow or outline etc

                    CTextStyle ts = new CTextStyle();
                    ts.Bold = style.Bold;
                    ts.FontSize = style.FontSize;
                    ts.FontName = style.FontName;
                    ts.Format = style.Format;
                    ts.Italic = style.Italic;
                    Rectangle extents = GetAccurateFontExtents(text, ts, true, false);

                    Rectangle r = new Rectangle((int)x, (int)(extents.Y+y), (int)(x+1), extents.Height);
                    brush.Assign(new LinearGradientBrush(r, style.TextColor, style.TextColor2, LinearGradientMode.Vertical));
                }

                using (GraphicsPath path = new GraphicsPath())
                {
                    int size = (int)((gp.DpiX * font.SizeInPoints / 72.0f)+0.499f);
                    if (size > 0)
                    {
                        path.AddString(text,
                                new FontFamily(font.Name),
                                (int)font.Style,
                                size,
                                new PointF(x, y),
                                style.Format);
                   
                        gp.FillPath(brush, path);
                    }

                    if (style.Outline && outlinelength <= 1)
                    {
                        Color outlineColor = Color.FromArgb(style.OutlineAlpha, style.OutlineColor);

                        outlineColor = Color.Black;

                        using (Pen pen = new Pen(outlineColor, 1))
                        {
                            gp.DrawPath(pen, path);
                        }
                    }
                }
            }
        }

        //*******************************************************************
        public SizeF GetApproxFontExtents(string text, Graphics gp, CTextStyle style, float max_width)
        {
            SizeF stringSize = new SizeF();
            stringSize.Width = 10;
            stringSize.Height = 10;
            StringFormat newStringFormat = new StringFormat();
            newStringFormat.FormatFlags = 0;

            using (DisposableObject<Font> f = new DisposableObject<Font>())
            {
                try
                {
                    f.Assign(GenerateFont(style));
                }
                catch (Exception)
                {
                    return stringSize;
                }

                if (f.IsNull()) return stringSize;

                // Set maximum layout size.
                SizeF layoutSize = new SizeF(2000.0f, 2000.0f);
                //	SizeF layoutSize = new SizeF(max_width, 2000.0F);
                // Set string format.

                // Measure string.

                stringSize = gp.MeasureString(
                    text,
                    f,
                    layoutSize,
                    newStringFormat);

                return stringSize;
            }
        }



        //*******************************************************************
        public Font GenerateFont(CTextStyle style)
        {
            Font return_font;
            float size = style.FontSize;

            FontStyle s = FontStyle.Regular;

            if (style.Bold == true) s |= FontStyle.Bold;
            if (style.Italic == true) s |= FontStyle.Italic;
            if (style.UnderLine == true) s |= FontStyle.Underline;

            if (size <= 0.0) size = 0.1f;

            return_font = new Font(style.FontName, size, s);

            return return_font;
        }


        private static Bitmap bitmap = new Bitmap(1, 1, PixelFormat.Format32bppArgb);

        //*******************************************************************
        // If we were to drawn this string in this style at 0,0  what
        // region from 0,0 is actually drawn to.
        //*******************************************************************
        public Rectangle GetAccurateFontExtents(string text, CTextStyle style)
        {
            return GetAccurateFontExtents(text, style, true, true);
        }

        //*******************************************************************
        // If we were to drawn this string in this style at 0,0  what
        // region from 0,0 is actually written to.
        public Rectangle GetAccurateFontExtents(string text, CTextStyle style, bool doHeight, bool doWidth)
        {
            int x = 0;
            int y = 0;
            int width = 1;
            int height = 1;

            // This method gets an aprox size (should be bigger) of our string,
            // then creates a blank bitmap from this size.
            // It then draws the string, then checks the bounds of pixels actually drawn to

            try
            {
                using (Graphics gp = Graphics.FromImage(bitmap))
                {
                    SizeF size = GetApproxFontExtents(text, gp, style, 2000.0f);

                    if (size.Width < 1 || size.Height < 1) return new Rectangle(x, y, width, height);

                    using (Bitmap b2 = new Bitmap((int)(size.Width + 0.4999f), (int)(size.Height + 0.4999f), PixelFormat.Format32bppArgb))
                    {
                        using (Graphics gp2 = Graphics.FromImage(b2))
                        {
                            gp2.Clear(Color.FromArgb(0, 0, 0, 0));

                            DrawString(gp2, text, 0, 0, style);

                            BitmapData bd = b2.LockBits(new Rectangle(0, 0, b2.Width, b2.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                            unsafe
                            {

                                width = b2.Width;
                                height = b2.Height;

                                if (doHeight)
                                {
                                    uint* ptr = (uint*)bd.Scan0.ToPointer();
                                    bool found_y = false;

                                    for (int h = 0; (h < b2.Height && found_y == false); h++)
                                    {
                                        uint* ptr1 = ptr;
                                        for (int w = 0; w < b2.Width; w++)
                                        {
                                            if (*ptr1++ != 0)
                                            {
                                                y = h;
                                                found_y = true;
                                                break;
                                            }
                                        }
                                        ptr += (bd.Stride >> 2);
                                    }

                                    bool found_height = false;
                                    ptr = (uint*)bd.Scan0.ToPointer();
                                    ptr += (bd.Stride >> 2) * (b2.Height - 1);

                                    for (int h = 0; (h < b2.Height && found_height == false); h++)
                                    {
                                        uint* ptr1 = ptr;
                                        for (int w = 0;w < b2.Width; w++)
                                        {
                                            if (*ptr1++ != 0)
                                            {
                                                height = b2.Height - y - h;
                                                found_height = true;
                                                break;
                                            }
                                        }
                                        ptr -= (bd.Stride >> 2);
                                    }
                                }

                                if (doWidth)
                                {
                                    bool found_x = false;
                                    uint* ptr = (uint*)bd.Scan0.ToPointer();

                                    for (int w = 0; (w < b2.Width && found_x == false); w++)
                                    {
                                        uint* ptr1 = ptr;
                                        for (int h = 0; h < b2.Height ; h++)
                                        {
                                            if (*ptr1 != 0)
                                            {
                                                x = w;
                                                found_x = true;
                                                break;
                                            }
                                            ptr1 += (bd.Stride >> 2);
                                        }
                                        ptr++;
                                    }

                                    bool found_width = false;
                                    ptr = (uint*)bd.Scan0.ToPointer();
                                    ptr += (bd.Stride >> 2) - 1;

                                    for (int w = 0; (w < b2.Width && found_width == false); w++)
                                    {
                                        uint* ptr1 = ptr;
                                        for (int h = 0; h < b2.Height ; h++)
                                        {
                                            if (*ptr1 != 0)
                                            {
                                                width = b2.Width - x - w;
                                                found_width = true;
                                                break;
                                            }
                                            ptr1 += (bd.Stride >> 2);
                                        }
                                        ptr--;
                                    }

                                }

                            }

                            b2.UnlockBits(bd);
                        }
                    }
                }
            }
            catch
            {
            }
            return new Rectangle(x, y, width, height);
        }
    }

}

