using System;
using System.Drawing;
using System.IO;
using System.Collections;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using ManagedCore;

//using LoadImageQuick;

namespace DVDSlideshow.GraphicsEng
{
    // This class represents an image held in the ImageCache class.  This image will be stored in memory and will be compressed internally if created 
    // with a jpg based image
    public class CMemoryCachedImage : IDisposable
    {
        private static Image mGDIImage;                 
        private static CMemoryCachedImage mGDIImageIsFor;   // hold onto last image, this is then disposed on next Get call

        private CompressedImage mCompressedImage = null;    // if jpeg
        private Image mUnCompressedImage = null;            // for any other format

        public String mName;
        public int mCounter;
        public Size mMaxSizeImageFromFile;
        public float mAspect;

        // ***********************************************************************************************
        public void SetImage(Image image, ImageFormat orignalFormat)
        {
            Clean();

            // If the original format is not a jpg then the passed in image may have alha information,
            // and saving as compressed .png did not always work for me.  For now just cache it un-compressed
            if (orignalFormat != ImageFormat.Jpeg)
            {
                mUnCompressedImage = image;
                return;
            }

            mGDIImage = image;
            mGDIImageIsFor = this;

            mGDIImage = null;           // set both to null to test GetImage with out need to re-cache in
            mGDIImageIsFor = null;

            mCompressedImage = new CompressedImage(image as Bitmap, 100);
        }


        // ***********************************************************************************************
        public void Dispose()    
        {
            Clean();
        }

        // ***********************************************************************************************
        private void Clean()
        {
            if (mGDIImageIsFor == this)
            {
                mGDIImageIsFor = null;

                if (mGDIImage != null)
                {
                    mGDIImage.Dispose();
                    mGDIImage = null;
                }
            }

            if (mCompressedImage != null)
            {
                mCompressedImage.Dispose();
                mCompressedImage = null;
            }

            if (mUnCompressedImage!=null)
            {
                mUnCompressedImage.Dispose();
                mUnCompressedImage = null;
            }
        }

        // ***********************************************************************************************
        // returns the amount of bytes needed to hold the cached image
        public long GetSizeInBytes()
        {
            if (mUnCompressedImage != null)
            {
                return mUnCompressedImage.Width * mUnCompressedImage.Height * 4;
            }

            if (mCompressedImage != null)
            {
                return mCompressedImage.SizeInBytes;
            }
            return 0;
        }
            

        // ***********************************************************************************************
        // Returns the cached image as a GDI Image (bitmap), the returned image is disposed of iternally later in sequential
        // calls to GetImage() on an instance of this class.  I.e the caller should use the returned image imediadly and not hold 
        // a reference to it.
        public Image GetImage()
        {
            if (mUnCompressedImage != null)
            {
                return mUnCompressedImage;
            }

            if (mGDIImageIsFor == this && mGDIImage != null)
            {
                return mGDIImage;
            }

            if (mGDIImage != null)
            {
                mGDIImage.Dispose();
            }

            mGDIImage = mCompressedImage.Get();
            mGDIImageIsFor = this;

            return mGDIImage;
        }



    }
}
