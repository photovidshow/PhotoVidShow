using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace DVDSlideshow
{
    class BrightnessContrast
    {

        public static ColorMatrix GenerateMatrix(float brightness, float contrast)
        {
            // SRG TODO
            return new ColorMatrix();
        }

        public static void ApplyBrightnessContrastGamma(Bitmap bitmap, float brightness, float contrast, float gamma)
        {
            ApplyBrightnessContrastGamma(bitmap, brightness, brightness, brightness, contrast, contrast, contrast, gamma);
        }

        public static void ApplyBrightnessContrastGamma(Bitmap bitmap, 
                                                         float brightnessR,
                                                         float brightnessG,
                                                         float brightnessB,
                                                         float contrastR,
                                                         float contrastG,
                                                         float contrastB,
                                                         float gamma )
        {

            Bitmap originalImage = bitmap.Clone() as Bitmap;
            Bitmap adjustedImage = bitmap;

            // create matrix that will brighten and contrast the image
            float[][] ptsArray ={
                    new float[] {contrastR, 0, 0, 0, 0}, // scale red
                    new float[] {0, contrastG, 0, 0, 0}, // scale green
                    new float[] {0, 0, contrastB, 0, 0}, // scale blue
                    new float[] {0, 0, 0, 1.0f, 0}, // don't scale alpha
                    new float[] {brightnessR -1, brightnessG -1, brightnessB -1, 0, 1}};

            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.ClearColorMatrix();
            imageAttributes.SetColorMatrix(new ColorMatrix(ptsArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            if (gamma != 1)
            {
                imageAttributes.SetGamma(gamma, ColorAdjustType.Bitmap);
            }

            Graphics g = Graphics.FromImage(adjustedImage);
            g.DrawImage(originalImage, new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height)
                , 0, 0, originalImage.Width, originalImage.Height,
                GraphicsUnit.Pixel, imageAttributes);

            originalImage.Dispose();
        }
    }
}
