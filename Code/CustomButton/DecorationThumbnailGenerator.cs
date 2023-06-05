using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;
using DVDSlideshow.GraphicsEng;
using DVDSlideshow.GraphicsEng.Direct3D;
using System.Drawing.Imaging;
using ManagedCore;
using System.IO;
using System.Collections;

namespace CustomButton
{
    public class DecorationThumbnailGenerator
    {
        //*************************************************************************************************
        public static Image GenerateThumbnailForDecoration(CDecoration d, int boxwidth, int boxheight, Image preloadedImage, bool drawAlphamap, bool ignoreCrop, bool cacheResult)
        {
            Image rawimage = null;

            CImageDecoration imageDecor = d as CImageDecoration;

            float thumbnailAspect = ((float)boxwidth) / ((float)boxheight);

            int width = boxwidth;
            int height = boxheight;

            bool needsDisposing = false;

            float aspect = 1;
            CClipArtDecoration cad = d as CClipArtDecoration;
            if (cad != null)
            {
                CImage image = cad.mImage;

                if (ignoreCrop == true)
                {
                    CImage uncroppedImage = new CImage(cad.mImage.ImageFilename, false);
                    uncroppedImage.Brightness = cad.Brightness;
                    uncroppedImage.Contrast = cad.Contrast;
                    uncroppedImage.BlackAndWhite = cad.BlackAndWhite;
                    image = uncroppedImage;
                }

                aspect = image.Aspect;

                if (cad.Orientation == CImageDecoration.OrientationType.CW90 ||
                    cad.Orientation == CImageDecoration.OrientationType.CW270)
                {
                    int temp = boxwidth;
                    boxwidth = boxheight;
                    boxheight = temp;
                    thumbnailAspect = ((float)boxwidth) / ((float)boxheight);
                    height = boxheight;
                    width = boxwidth;

                }
                if (aspect <= thumbnailAspect)
                {
                    width = (int)((aspect * ((float)height)));
                }
                else
                {
                    height = (int)(((float)width) / aspect);
                }

                if (width < 1) width = 1;
                if (height < 1) height = 1;

                rawimage = image.GetRawImage(new SizeF(width, height), false, preloadedImage, !cacheResult, out needsDisposing);

            }

            CVideoDecoration vd = d as CVideoDecoration;
            if (vd != null)
            {
                aspect = ((float)vd.GetVideoWidth()) / ((float)vd.GetVideoHeight());
                if (vd.Orientation == CImageDecoration.OrientationType.CW90 ||
                    vd.Orientation == CImageDecoration.OrientationType.CW270)
                {
                    aspect = 1 / aspect;
                    int temp = width;
                    width = height;
                    height = width;
                }

                if (aspect <= thumbnailAspect)
                {
                    width = (int)((aspect * ((float)height)));
                }
                else
                {
                    height = (int)(((float)width) / aspect);
                }

                if (width < 1) width = 1;
                if (height < 1) height = 1;

                TrimVideoControl tvs = new TrimVideoControl();
                tvs.SetForDecoration(vd, false, false);
                rawimage = tvs.RenderToImage(true, 0, width, height, null); // we set frame to 0, so it matches start fame box in video trim manager

            }

            if (rawimage != null)   // resize as may give bigger image than requested
            {
                Image oldimage = rawimage;
                rawimage = new Bitmap(rawimage, new Size(width, height));

                if (cad != null)
                {
                    rawimage.RotateFlip(imageDecor.GetRotateFlipType());
                }

                width = rawimage.Width;
                height = rawimage.Height;

                // apply alpha map

                if (drawAlphamap == true && imageDecor.AlphaMap != null)
                {
                    Bitmap alphamap = new Bitmap(imageDecor.AlphaMap.GetRawImage(), new Size(width, height));
                    BitmapUtils.AddAlpha(rawimage as Bitmap, alphamap, 0, 0, width, height);
                    alphamap.Dispose();
                }

                if (needsDisposing)
                {
                    oldimage.Dispose();
                }
            }

            return rawimage;
        }
    }
}
