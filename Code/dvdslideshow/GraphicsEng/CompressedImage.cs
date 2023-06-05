using System;
using System.Drawing;
using System.IO;
using System.Collections;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using ManagedCore;

namespace DVDSlideshow.GraphicsEng
{
    // Allows you to store a bitmap compressed in memory
    public class CompressedImage : IDisposable
    {
        private MemoryStream mMemoryStream;

        private int mWidth;
        private int mHeight;

        // *********************************************************
        public int Width
        {
            get { return mWidth; }
        }

        // *********************************************************
        public int Height
        {
            get { return mHeight; }
        }


        // *********************************************************
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        // *********************************************************
        public CompressedImage(Bitmap b, int quality)
        {
            mWidth = b.Width;
            mHeight = b.Height;

            mMemoryStream = new MemoryStream();
            
            // save as memmory compressed jpg qith 100% quality

            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

            // Create an Encoder object based on the GUID
            // for the Quality parameter category.
            System.Drawing.Imaging.Encoder myEncoder =
                System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one
            // EncoderParameter object in the array.
            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality); // least compression
            myEncoderParameters.Param[0] = myEncoderParameter;

            b.Save(mMemoryStream, jgpEncoder, myEncoderParameters);
        }

        // *********************************************************
        // caller must dispose the returned bitmap
        public Bitmap Get()
        {
            mMemoryStream.Seek(0, SeekOrigin.Begin);
            Bitmap b = new Bitmap(mMemoryStream);

            if (b.PixelFormat != PixelFormat.Format32bppArgb)
            {
                Bitmap b1 = new Bitmap(mWidth, mHeight, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(b1))
                {
                    g.DrawImage(b, 0, 0); ;
                }
                b.Dispose();
                return b1;
            }

            return b;
        }

        // *********************************************************
        public long SizeInBytes
        {
            get { return mMemoryStream.Length; }
        }

        // *********************************************************
        public void Dispose()
        {
            if (mMemoryStream != null)
            {
                mMemoryStream.Dispose();
            }
            mMemoryStream = null;
        }

    }
}
