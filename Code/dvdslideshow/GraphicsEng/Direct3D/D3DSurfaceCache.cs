using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using MangedToUnManagedWrapper.ManagedDirect3D;
using ManagedCore;

namespace DVDSlideshow.GraphicsEng.Direct3D
{
    public class D3DSurfaceCache
    {
        private List<D3DSurface> mCachedSurfaces = new List<D3DSurface>();

        public event ManagedCore.StateChangeHandler StateChanged;

        private object mLock = new object();

        // **********************************************************************************************************
        public List<D3DSurface> GetCachedSurfaces()
        {
            lock (mLock)
            {
                List<D3DSurface> clonedlist = new List<D3DSurface>(mCachedSurfaces);

                return clonedlist;
            }
        }

        // **********************************************************************************************************
        public D3DSurfaceCache()
        {
        }

        // **********************************************************************************************************
        public D3DSurface GetSurface(uint width, uint height, Texture.Usage usage, uint bytesPerPixel, string id, bool forcePower2Texture)
        {
            return GetSurface(width, height, usage, bytesPerPixel, false, id, forcePower2Texture);
        }

        // **********************************************************************************************************
        public D3DSurface GetSurface(uint width, uint height, Texture.Usage usage, uint bytesPerPixel, bool doMipMapping, string id, bool forcePower2Texture)
        {
            lock (mLock)
            {
                foreach (D3DSurface surface in mCachedSurfaces)
                {
                    if (surface.Width == width && 
                        surface.Height == height &&
                        surface.Usage == usage &&
                        surface.BytesPerPixel == bytesPerPixel &&
                        surface.CreateMipMaps == doMipMapping)
                    {
                        mCachedSurfaces.Remove(surface);
                        surface.Originator = id;
                        surface.DeclareInReUse();

                        if (StateChanged != null) StateChanged();

                        return surface;
                    }
                }

                D3DSurface newSurface = new D3DSurface(width, height, usage, bytesPerPixel, doMipMapping, id, forcePower2Texture);

                if (StateChanged != null) StateChanged();

                return newSurface;
            }
        }

        // **********************************************************************************************************
        private int GetMaxSurfaceCacheSize()
        {
            // THIS NUMBER SEEMS TO WORK

            // Freeing surfaces on the laptop seems to be mega slow whe....

            // but having 200meg of surface on the desktop kills directX !!!!!

            return 8;
        }

        // **********************************************************************************************************
        public void FreeSurface(D3DSurface surface)
        {
            lock (mLock)
            {
                if (mCachedSurfaces.Contains(surface))
                {
                    Log.Warning("Freed surface already freed");
                    return;
                }
                while (mCachedSurfaces.Count >= GetMaxSurfaceCacheSize())
                {
                    D3DSurface to_remove = mCachedSurfaces[0];
                    mCachedSurfaces.RemoveAt(0);
                    to_remove.ManagedSurface.Cleanup();
                }

                mCachedSurfaces.Add(surface);

                if (StateChanged != null) StateChanged();
            }
        }

        // **********************************************************************************************************
        // called when a reset is needed
        public void FreeAllSurfaces()
        {
            lock (mLock)
            {
                while (mCachedSurfaces.Count > 0)
                {
                    D3DSurface to_remove = mCachedSurfaces[0];
                    mCachedSurfaces.RemoveAt(0);
                    to_remove.ManagedSurface.Cleanup();
                }
                if (StateChanged != null) StateChanged();
            }
        }
    }
}
