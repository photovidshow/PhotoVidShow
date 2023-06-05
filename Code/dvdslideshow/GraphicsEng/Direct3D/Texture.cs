using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using MangedToUnManagedWrapper.ManagedDirect3D;
using ManagedCore;

namespace DVDSlideshow.GraphicsEng.Direct3D
{
    /// <summary>
    /// Represents a DirectX texture, most likely stored in graphics card memory
    /// </summary>
    public class Texture : IDisposable
    {
        public enum Usage
        {
            MANAGED,
            RENDER_TARGET,
            SYSTEM_MEMORY,
            RENDER_TARGET_WITH_NO_ALPHA,
        };

        private ManagedTexture mManagedTexture  = null;
        private int mWidth = 0;
        private int mHeight = 0;
        private int mBytesPerPixel = 4;
        private int mApproxMemoryUsage = 0;
      
        public int Width
        {
            get { return mWidth;} 
        }

        public int Height
        {
            get { return mHeight; }
        }

        // *************************************************************************
        public ManagedTexture ManagedTexture
        {
            get { return mManagedTexture; }
        }

        // *************************************************************************
        public Texture(CImage image, SizeF requestedDimension, bool forcePower2Texture)
        {
            Bitmap b = image.GetRawImage(requestedDimension) as Bitmap;
            if (b == null)
            {
                mWidth = (int)requestedDimension.Width;
                mHeight = (int)requestedDimension.Height;
                return;
            }

            BitmapData i1Data = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                  ImageLockMode.ReadOnly, b.PixelFormat);

            int scanline = i1Data.Stride;
            System.IntPtr Scan0 = i1Data.Scan0;
            int d_pitch = (i1Data.Stride - (b.Width * mBytesPerPixel)) / mBytesPerPixel;

            try
            {
                mManagedTexture = new ManagedTexture();
                mManagedTexture.Init(Scan0, (uint)b.Width, (uint)b.Height, (uint)d_pitch, (uint)mBytesPerPixel, forcePower2Texture, image.CreateMipMaps);
            }
            catch (Exception exception)
            {
                ManagedCore.CDebugLog.GetInstance().Error("Failed to create D3D texture from image.  Request size:" + requestedDimension.Width+","+ requestedDimension.Height+", exception"+ exception.Message);
            }
            finally
            {
                b.UnlockBits(i1Data);
            }

            mWidth = b.Width;
            mHeight = b.Height;
        }

        // *************************************************************************
        public void Dispose()
        {
            if (mManagedTexture != null)
            {
                mManagedTexture.Cleanup();
            }
        }

        // *************************************************************************
        public string GetDetailsString()
        {
            return "Width=" + mWidth + ",Height=" + mHeight + ", bpp=" + mBytesPerPixel + "(" + GetMemoryUsage() + ")";
        }

        
        // ****************************************************************************************************
        private int GetNextPower2Size(int inputSize)
        {
	        int power=1;
	        while (power<inputSize)
	        {
		        power<<=1;
	        }
	        return power;
        }


        // *************************************************************************
        public int GetMemoryUsage()
        {
            if (mApproxMemoryUsage != 0)
            {
                return mApproxMemoryUsage;
            }

            int size = mWidth * mHeight * mBytesPerPixel;

            /*
            // assume deep down power2 texture and mipmapping
            int w = GetNextPower2Size(mWidth);
            int h = GetNextPower2Size(mHeight);
            if (w > h) h = w;
            if (h > w) w = h;

            int full = w * h * mBytesPerPixel;
            int size = full;
            size += full >> 2;
            size += full >> 4;
            size += full >> 6;
            size += full >> 8;
            */

            mApproxMemoryUsage = size;

            return mApproxMemoryUsage;
        }
    }
}
