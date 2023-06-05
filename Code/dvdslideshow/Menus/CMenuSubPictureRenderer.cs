using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using DVDSlideshow.GraphicsEng;
using ManagedCore;
using System.Collections;

namespace DVDSlideshow
{
    // used to work out what order the subtitle re arranged the menu decorations
    public class CDecorationSubtitle
    {
        public CMenuButton mForDecoration = null;
        public Rectangle mSubtitleRegion;
        public int CMenuLinkID = -1;
        public Bitmap mCreatedBitmap;   // set if called via RenderButton method else null
    }

    public class CMenuDecorationSubtitles
    {
        public ArrayList mDecorationSubtitles = new ArrayList();
    }

    public class CMenuSubPictureRenderer
    {
        private const float OutlineWidthFraction = 256.0f;

        // ************************************************************************************************
        // This method will typically be used to render the current sub picture buttons for a menu frame.
        // e.g. For rendering selected items in the preview menu.
        // And also when generating an entire sub picture frame when creating a DVD sub title video.
        public CMenuDecorationSubtitles Render(List<CMenuButton> buttons,
                                               Bitmap toBitmap,    // must be set
                                               CMainMenu owningMenu)
        {
            using (Graphics bitmapGp = Graphics.FromImage(toBitmap))
            {
                return Render(buttons, bitmapGp, toBitmap.Width, toBitmap.Height, owningMenu);
            }
        }


        // ************************************************************************************************
        public CMenuDecorationSubtitles Render(List<CMenuButton> buttons,
                                               Graphics bitmapGp,    
                                               int width,
                                               int height,
                                               CMainMenu owningMenu)  
        {
            CMenuDecorationSubtitles currentMenuDecorationSubtitles = new CMenuDecorationSubtitles();

            CMenuButtonStyle menuButtonStyle = CMenuButtonStyleDatabase.GetInstance().GetStyle(owningMenu.ButtonStyleID);
            CMenuLinkStyle menuLinkStyle = CMenuLinkStyleDatabase.GetInstance().GetStyle(owningMenu.LinkStyleID);
            CMenuSubPictureStyle subPictureStyle = owningMenu.SubPictureStyle;
            CImage subPictureSelectIcon = null; // ## SRG TO DO  CMenuSubPictureSelectIconDatabase.GetInstance().GetIcon(subPictureStyle.SubPictureSelectIconIndex);

       
            SetGpForSubTitleRendering(bitmapGp);

            foreach (CMenuButton button in buttons)
            {
                Rectangle rd = new Rectangle(0, 0, width, height);

                if ( CGlobals.mVideoGenerateMode == VideoGenerateMode.FINAL_OUTPUT_MODE)
                {
                    rd = CGlobals.mCurrentProject.DiskPreferences.GetFinalDiskCropRectangle(CGlobals.mCurrentProject.DiskPreferences.OutputVideoWidth, CGlobals.mCurrentProject.DiskPreferences.OutputVideoHeight);
                }

                CDecorationSubtitle ds = RenderButton(button, rd, bitmapGp, subPictureStyle, menuButtonStyle, menuLinkStyle, subPictureSelectIcon);
                if (ds != null)
                {
                    currentMenuDecorationSubtitles.mDecorationSubtitles.Add(ds);
                }
            }

            return currentMenuDecorationSubtitles;
        }

        // ************************************************************************************************
        // This method will typically be used to create a particlar menu button's sub picture which is
        // then returned as a bitmap.
        // This method is needed when generating grapghics to store on a Blu-ray disc.
        public CDecorationSubtitle RenderButton(CMenuButton button, CMainMenu owningMenu)
        {
            Rectangle rd = CGlobals.mCurrentProject.DiskPreferences.GetFinalDiskCropRectangle(CGlobals.mCurrentProject.DiskPreferences.OutputVideoWidth, CGlobals.mCurrentProject.DiskPreferences.OutputVideoHeight);
           
            CMenuButtonStyle menuButtonStyle = CMenuButtonStyleDatabase.GetInstance().GetStyle(owningMenu.ButtonStyleID);
            CMenuLinkStyle menuLinkStyle = CMenuLinkStyleDatabase.GetInstance().GetStyle(owningMenu.LinkStyleID);
            CMenuSubPictureStyle subPictureStyle = owningMenu.SubPictureStyle;

            CImage subPictureSelectIcon = null; // ### SRG TO DO CMenuSubPictureSelectIconDatabase.GetInstance().GetIcon(subPictureStyle.SubPictureSelectIconIndex);
   
            CDecorationSubtitle ds = RenderButton(button, rd, null, subPictureStyle, menuButtonStyle, menuLinkStyle, subPictureSelectIcon);
   
            return ds;
        }

        // ************************************************************************************************
        private void SetGpForSubTitleRendering(Graphics gp)
        {
            gp.InterpolationMode = InterpolationMode.NearestNeighbor;
            gp.SmoothingMode = SmoothingMode.None;
        }

        // ************************************************************************************************
        private CDecorationSubtitle RenderButton(CMenuButton button, 
                                                 Rectangle rd, 
                                                 Graphics bitmapGp, 
                                                 CMenuSubPictureStyle subPictureStyle, 
                                                 CMenuButtonStyle buttonStyle,
                                                 CMenuLinkStyle menuLinkStyle,
                                                 CImage subPictureSelectIcon)
        {
            RectangleF coverage = button.CoverageArea;

            // draw borders around decoration frame in subtitle

            if (button is CMenuSlideshowButton || button is CMenuLinkSubMenuButton)
            {
                Rectangle r = GetCoverageAreaForStyle(coverage, rd, subPictureStyle.SubPictureButtonStyle);

                
                CDecorationSubtitle ds = new CDecorationSubtitle();

                CMenuLinkSubMenuButton mlsb = button as CMenuLinkSubMenuButton;
                if (mlsb != null)
                {
                    ds.CMenuLinkID = mlsb.MenuLinkID;
                }

                ds.mForDecoration = button;
                ds.mSubtitleRegion = r;
                ds.mCreatedBitmap = DrawButtonRegionForStyle(bitmapGp, subPictureStyle.SubPictureButtonStyle, subPictureStyle.SubPictureColor, r, buttonStyle.SlideshowButtonFrameImage, subPictureSelectIcon);

                return ds;

            }

            CMenuLinkPreviousNextButton mlb = button as CMenuLinkPreviousNextButton;

            if (mlb != null)
            {
                Rectangle r = GetCoverageAreaForStyle(coverage, rd, subPictureStyle.SubPictureMenuLinkStyle);

                CDecorationSubtitle ds = new CDecorationSubtitle();
                ds.mForDecoration = button;
                ds.CMenuLinkID = mlb.MenuLinkID;
                ds.mSubtitleRegion = r;

                

                if (mlb.Link == CMenuLinkPreviousNextButton.LinkType.NEXT_MENU)
                {
                    ds.mCreatedBitmap = DrawButtonRegionForStyle(bitmapGp, subPictureStyle.SubPictureMenuLinkStyle, subPictureStyle.SubPictureColor, r, menuLinkStyle.NextButtonImage, subPictureSelectIcon);
                }
                else
                {
                    ds.mCreatedBitmap = DrawButtonRegionForStyle(bitmapGp, subPictureStyle.SubPictureMenuLinkStyle, subPictureStyle.SubPictureColor, r, menuLinkStyle.PreviousButtonImage, subPictureSelectIcon);
                }

                return ds;
            }

            if (button is CMenuPlayAllButton ||
                button is CMenuPlayAllLoopedButton)
            {  
                //
                // Cover style not supported for text based subtitles.  Change to coverRectangle
                //
                if (subPictureStyle.SubPicturePlayMethodsStyle == CMenuSubPictureRenderMethod.HighlightImage)
                {
                    Log.Warning("Cover not supported for text based subtitles");
                    subPictureStyle.SubPicturePlayMethodsStyle = CMenuSubPictureRenderMethod.CoveringRectangle;
                }

                Rectangle r = GetCoverageAreaForStyle(coverage, rd, subPictureStyle.SubPicturePlayMethodsStyle);

                CDecorationSubtitle ds = new CDecorationSubtitle();
                ds.mForDecoration = button;
                ds.mSubtitleRegion = r;

                ds.mCreatedBitmap = DrawButtonRegionForStyle(bitmapGp, subPictureStyle.SubPicturePlayMethodsStyle, subPictureStyle.SubPictureColor, r, null, subPictureSelectIcon);
                return ds;
            }
            return null;
        }

        // ************************************************************************************************
        private Bitmap DrawButtonRegionForStyle(Graphics gp, CMenuSubPictureRenderMethod style, Color color, Rectangle region, CImage parameter1, CImage parameter2)
        {
            bool tempGP = false;
            Bitmap bitmap = null;

            //
            // If we've not defined gp, it means create a bitmap of just the button and return that
            //
            if (gp == null)
            {
                bitmap = new Bitmap(region.Width, region.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                gp = Graphics.FromImage(bitmap);
                SetGpForSubTitleRendering(gp);
                tempGP = true;
                region = new Rectangle(0, 0, region.Width, region.Height);
            }
            try
            {

                switch (style)
                {
                    case CMenuSubPictureRenderMethod.CoveringRectangle:
                        {
                            DrawButtonRegionForCoverRectangle(gp, style, color, region);
                            break;
                        }

                    case CMenuSubPictureRenderMethod.HighlightImage:
                        {
                            DrawButtonRegionForMenuFrame(gp, style, color, region, parameter1);
                            break;
                        }

                    case CMenuSubPictureRenderMethod.OutlineRectangle:
                        {
                            DrawButtonRegionForOutlineRectangle(gp, style, color, region);
                            break;
                        }

                    case CMenuSubPictureRenderMethod.OutlineRoundedRectangle:
                        {
                            DrawButtonRegionForOutlineRoundedRectangle(gp, style, color, region);
                            break;
                        }

                    case CMenuSubPictureRenderMethod.CoveringRoundedRectangle:
                        {
                            DrawButtonRegionForCoveringRoundedRectangle(gp, style, color, region);
                            break;
                        }

                    case CMenuSubPictureRenderMethod.CircleOnRight:
                    case CMenuSubPictureRenderMethod.CircleOnLeft:
                        {
                            DrawButtonRegionForEclipse(gp, style, color, region, parameter2);
                            break;
                        }
                    case CMenuSubPictureRenderMethod.SquareOnRight:
                    case CMenuSubPictureRenderMethod.SquareOnLeft:
                    case CMenuSubPictureRenderMethod.Underline:
                        {
                            DrawButtonRegionForRectangle(gp, style, color, region, parameter2);
                            break;
                        }

                    default:
                        {
                            Log.Error("Unknown sub picture style: " + style.ToString());
                            break;
                        }
                }
            }
            finally
            {
                if (tempGP == true)
                {
                    gp.Dispose();
                }
            }

            return bitmap;
        }

        // ************************************************************************************************
        private void DrawButtonRegionForMenuFrame(Graphics gp, CMenuSubPictureRenderMethod style, Color color, Rectangle region, CImage frame)
        {
            //
            // Frame must be provided
            //
            if (frame == null)
            {
                Log.Error("Can not draw sub picture from frame as passed in frame was null");
                return;
            }

            Bitmap iii = frame.GetRawImage() as Bitmap ;

            using (Bitmap i2 = iii.Clone() as Bitmap)
            {
                BitmapUtils.MakeTransparentOneColor(i2, color, color.A);

                gp.DrawImage(i2,
                        region, 0, 0, iii.Width, iii.Height,
                        GraphicsUnit.Pixel);
            }

        }

        // ************************************************************************************************
        private void DrawButtonRegionForCoverRectangle(Graphics gp, CMenuSubPictureRenderMethod style, Color color, Rectangle region)
        {
            using (SolidBrush brush = new SolidBrush(color))
            {
                gp.FillRectangle(brush, region);
            }
        }

        // ************************************************************************************************
        private void DrawButtonRegionForOutlineRectangle(Graphics gp, CMenuSubPictureRenderMethod style, Color color, Rectangle region)
        {
            float width = CGlobals.mCurrentProject.DiskPreferences.CanvasWidth;
            float penWidth = width / OutlineWidthFraction;

            // draw in GUI
            using (Pen pen = new Pen(color, penWidth))
            {
                pen.Alignment = PenAlignment.Inset;
                Rectangle r1 = new Rectangle(region.X, region.Y, region.Width, region.Height);
                gp.DrawRectangle(pen, r1);
            }   
        }

        // ************************************************************************************************
        private void DrawButtonRegionForOutlineRoundedRectangle(Graphics gp, CMenuSubPictureRenderMethod style, Color color, Rectangle region)
        {
            // draw in GUI
            using (Pen pen = new Pen(color,2))
            {
                using (GraphicsPath path = GDIRoundedRectangle.Create(region.X, region.Y, region.Width, region.Height, region.Height / 6))
                {
                    gp.DrawPath(pen, path);
                }
            }
        }

        // ************************************************************************************************
        private void DrawButtonRegionForCoveringRoundedRectangle(Graphics gp, CMenuSubPictureRenderMethod style, Color color, Rectangle region)
        {
            // draw in GUI
            using (SolidBrush brush = new SolidBrush(color))
            {
                using (GraphicsPath path = GDIRoundedRectangle.Create(region.X, region.Y, region.Width, region.Height, region.Height / 6))
                {
                    gp.FillPath(brush, path);
                }
            }
        }

        // ************************************************************************************************
        private void DrawButtonRegionForEclipse(Graphics gp, CMenuSubPictureRenderMethod style, Color color, Rectangle region, CImage icon)
        {
            // draw in GUI
            using (SolidBrush brush = new SolidBrush(color))
            {
                gp.FillEllipse(brush, region);
            }
        }

        // ************************************************************************************************
        private void DrawButtonRegionForRectangle(Graphics gp, CMenuSubPictureRenderMethod style, Color color, Rectangle region, CImage icon)
        {
            // draw in GUI
            using (SolidBrush brush = new SolidBrush(color))
            {
                gp.FillRectangle(brush, region);
            }
        }

        // ************************************************************************************************
        public static Rectangle GetCoverageAreaForStyle(RectangleF decorationCoverage, Rectangle rd, CMainMenu menu, CMenuButton button)
        {
            CMenuSubPictureStyle style = menu.SubPictureStyle;

            if (button is CMenuSlideshowButton || button is CMenuLinkSubMenuButton)
            {
                return GetCoverageAreaForStyle(decorationCoverage, rd, style.SubPictureButtonStyle);
            }
            if (button is CMenuLinkButton)
            {
                return GetCoverageAreaForStyle(decorationCoverage, rd, style.SubPictureMenuLinkStyle);
            }
            if (button is CMenuPlayAllButton || button is CMenuPlayAllLoopedButton)
            {
                return GetCoverageAreaForStyle(decorationCoverage, rd, style.SubPicturePlayMethodsStyle);
            }
            return new Rectangle(0, 0, 1, 1);
        }

        // ************************************************************************************************
        public static Rectangle GetCoverageAreaForStyle(RectangleF decorationCoverage, Rectangle rd, CMenuSubPictureRenderMethod style)
        {
            RectangleF dest = decorationCoverage;

            float frac = CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction();

            // Assume new dest rect is square

            float iconWidth = 1.0f / 50.0f;
            float iconHeight = 1.0f / 50.0f / frac;

            if (style == CMenuSubPictureRenderMethod.CircleOnRight ||
                style == CMenuSubPictureRenderMethod.SquareOnRight)
            {
                dest = new RectangleF(((decorationCoverage.X + decorationCoverage.Width + (iconWidth / 1.7f))),
                                     ((decorationCoverage.Y + (decorationCoverage.Height / 2) - (iconHeight / 2))),
                                      iconWidth,
                                      iconHeight);
            }
            else if (style == CMenuSubPictureRenderMethod.CircleOnLeft ||
                     style == CMenuSubPictureRenderMethod.SquareOnLeft)
            {
                dest = new RectangleF(((decorationCoverage.X - iconWidth - (iconWidth / 1.7f))),
                                     ((decorationCoverage.Y + (decorationCoverage.Height / 2) - (iconHeight / 2))),
                                      iconWidth,
                                      iconHeight);
            }
            else if (style == CMenuSubPictureRenderMethod.Underline)
            {
                dest = new RectangleF(decorationCoverage.X,
                                     (decorationCoverage.Y + decorationCoverage.Height),
                                      decorationCoverage.Width,
                                      decorationCoverage.Height / 10);
            }
            else if (style == CMenuSubPictureRenderMethod.OutlineRectangle ||
                     style == CMenuSubPictureRenderMethod.OutlineRoundedRectangle)
            {
                float a = 1.0f / OutlineWidthFraction;
                dest = new RectangleF(decorationCoverage.X - a, decorationCoverage.Y - a, decorationCoverage.Width + (a*2), decorationCoverage.Height + (a*2));
            }

            float x = ((float)rd.Width) * dest.X + rd.X;
            float y = ((float)rd.Height) * dest.Y + rd.Y;
            float ww = ((float)rd.Width) * dest.Width;
            float hh = ((float)rd.Height) * dest.Height;
            Rectangle r = new Rectangle((int)(x - 0.4999f), (int)(y - 0.4999f), (int)(ww + 1f), (int)(hh + 1f));

            return r;
        }
    }
}
