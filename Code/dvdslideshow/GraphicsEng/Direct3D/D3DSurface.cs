using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using MangedToUnManagedWrapper.ManagedDirect3D;
using DVDSlideshow.GraphicsEng;
using ManagedCore;

namespace DVDSlideshow.GraphicsEng.Direct3D
{
    /// <summary>
    /// Represents a DirectX surface we can render to
    /// </summary>
    public class D3DSurface : RenderSurface, ID3DSurface
    {
        private static List<D3DSurface> mCurrentSurfaces = new List<D3DSurface>();
        private static object mLock = new object();

        // ***************************************************************************************************
        public static List<D3DSurface> GetCurrentSurfaces()
        {
            lock (mLock)
            {
                List<D3DSurface> clonedList = new List<D3DSurface>(mCurrentSurfaces);
                return clonedList;
            }
        }


        private ManagedSurface mManagedSurface;
        private uint mWidth = 0;
        private uint mHeight = 0;
        private Texture.Usage mUsage = Texture.Usage.MANAGED;
        private bool mDynamic = false;
        private uint mBytesPerPixel = 0;
        private string mOriginator;

        // ***************************************************************************************************
        public String Originator
        {
            get { return mOriginator; }
            set { mOriginator = value; }
        }



        // ***************************************************************************************************
        public uint BytesPerPixel
        {
            get { return mBytesPerPixel; }
        }

        // ***************************************************************************************************
        public Texture.Usage Usage
        {
            get { return mUsage; }
        }

        // ***************************************************************************************************
        public ManagedSurface ManagedSurface
        {
            get { return mManagedSurface; }
        }


        // ***************************************************************************************************
        public override uint Width
        {
            get { return mWidth; }
        }


        // ***************************************************************************************************
        public override uint Height
        {
            get { return mHeight; }
        }


        // ***************************************************************************************************
        public D3DSurface(uint width, uint height, Texture.Usage usage, uint bytesPerPixel, bool doMipMapping, string originator, bool forcePower2TextureOnMipMapTextures)
        {
            mWidth = width;
            mHeight = height;
            mOriginator = originator;
            mCreateMipMaps = doMipMapping;
            mUsage = usage;

            if (usage == Texture.Usage.MANAGED ||
                usage == Texture.Usage.SYSTEM_MEMORY)
            {
                mDynamic = true;

            }
            mBytesPerPixel = bytesPerPixel;

            int iDoMipMapping = 0;
            if (doMipMapping == true )
            {
                iDoMipMapping = 1;
            }

            mManagedSurface = new ManagedSurface();
            mManagedSurface.Init(width, height, (int)usage, bytesPerPixel, iDoMipMapping, forcePower2TextureOnMipMapTextures);

            lock (mLock)
            {
                mCurrentSurfaces.Add(this);
            }
        }


        // ***************************************************************************************************
        // used by surface cache after it is being re-used
        public void DeclareInReUse()
        {
            lock (mLock)
            {
                mCurrentSurfaces.Add(this);
            }
        }

        // ***************************************************************************************************
        public override void Dispose()
        { 
            base.Dispose();

            lock (mLock)
            {
                bool result = mCurrentSurfaces.Remove(this);
                if (result == false)
                {
                    ManagedCore.Log.Warning("Disposing a surface which does not exist in current surfaces list (ignoring)");
                }
            }

            Direct3DDevice device = GraphicsEngine.Current as Direct3DDevice;
            if (device != null)
            {
                device.FreeRenderSurface(this);
            }
        }

        // ***************************************************************************************************
        public LockedRect Lock()
        {
            if (mDynamic == false)
            {
                LockedRect lr;
                lr.Pitch = 0;
                lr.mBits = 0;
                return lr;
            }
            else
            {

                ManagedLockRect mlr = mManagedSurface.Lock();
                LockedRect lr;
                lr.Pitch = mlr.Pitch;
                lr.mBits = mlr.pBits;
                return lr;
            }

        }

        // ***************************************************************************************************
        public void Unlock()
        {
            if (mDynamic==false) return;
            mManagedSurface.Unlock();
        }


    
        // ***************************************************************************************************
        public override RenderSurface.State GetState()
        {
            if (mManagedSurface.HasD3DTexture() == true)
            {
                return State.OK;
            }
            else
            {
                return State.LOST;
            }
        }


        // ***************************************************************************************************
        public string GetDetailsString()
        {
            return mUsage.ToString() + " width=" + mWidth + ", height=" + mHeight + ",bpp=" + mBytesPerPixel.ToString() + "(" + GetMemoryUsage() + ") Originator:" + this.Originator;
        }


        // ***************************************************************************************************
        public int GetMemoryUsage()
        {
            return (int) ( mWidth * mHeight * mBytesPerPixel);
        }
    }
}
