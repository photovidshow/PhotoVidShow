
using System;
using System.Drawing;
using System.Collections.Generic;

using DVDSlideshow.GraphicsEng.Direct3D;

namespace DVDSlideshow.GraphicsEng
{
    //
    // Graphics API for drawing drawable object to given destination RenderSurface
    //
    // the Default RenderSurface is null, which can be presneted to the window set up in any child class 
    //
    public abstract class GraphicsEngine
    {
        public enum State
        {
            OK,                 // Ok to use
            NOT_RESET,          // Just needs a reset
            LOST_DEVICE,        // Can not use, call RefreshState periodically
            NO_ENGINE_DEFINED   // A graphics engine has not been defined
        }

        protected State mLastKnownState = State.OK;
        protected bool mForcePower2Textures = false;

        public State GetLastKnownState()
        {
            return mLastKnownState;
        }

        virtual public bool ForcePower2Textures
        {
            get { return mForcePower2Textures; }
            set { mForcePower2Textures = value; }
        }

        public abstract State RefreshState();
    
        protected abstract void SetDefaultSurfaceViewport(uint width, uint height);

        public abstract GraphicsEngine.State BeginScene(BeginSceneParameters parameters);
        public abstract void EndScene();
        public State PresentToWindow() // presents default surface to window
        {
            return PresentToWindow(new IntPtr());
        }

        public State PresentToWindow(IntPtr alternativeWindowHandle)
        {
            return PresentToWindow(new IntPtr(), false);
        }

        public abstract State PresentToWindow(IntPtr alternativeWindowHandle, bool scaleBackBufferToWindow); 

        public void DrawImage(DrawableObject drawableObject, RectangleF srcRect, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, bool alpha)
        {
            CImage image = drawableObject as CImage;
            if (image != null)
            {              
                DrawImage(image, srcRect, x1, y1, x2, y2, x3, y3, x4, y4, alpha);               
                return;
            }

            Surface surface = drawableObject as Surface;
            if (surface != null)
            {
                DrawImage(surface, srcRect, x1, y1, x2, y2, x3, y3, x4, y4, alpha);
            }
        }
        public void DrawImage(DrawableObject drawableObject, RectangleF srcRect, RectangleF dstRect, bool incluceImageAlpha)
        {
            CImage image = drawableObject as CImage;
            if (image != null)
            {
                DrawImage(image, srcRect, dstRect.X, dstRect.Y + dstRect.Height, dstRect.X, dstRect.Y, dstRect.X + dstRect.Width, dstRect.Y + dstRect.Height, dstRect.X + dstRect.Width, dstRect.Y, incluceImageAlpha);
                return;
            }

            Surface surface = drawableObject as Surface;
            if (surface != null)
            {
                DrawImage(surface, srcRect, dstRect.X, dstRect.Y + dstRect.Height, dstRect.X, dstRect.Y,dstRect.X + dstRect.Width, dstRect.Y + dstRect.Height, dstRect.X + dstRect.Width, dstRect.Y, incluceImageAlpha);
            }

        }
        public void DrawImageWithDiffuseAlpha(DrawableObject drawableObject, float alpha, RectangleF srcRect, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, bool includeImageAlpha)
        {
            CImage image = drawableObject as CImage;
            if (image != null)
            {
                DrawImageWithDiffuseAlpha(image, alpha, srcRect, x1, y1, x2, y2, x3, y3, x4, y4, includeImageAlpha);
                return;
            }

            Surface surface = drawableObject as Surface;
            if (surface != null)
            {
                DrawImageWithDiffuseAlpha(surface, alpha, srcRect, x1, y1, x2, y2, x3, y3, x4, y4, includeImageAlpha);
            }
        }
        public void DrawImageWithDiffuseAlpha(DrawableObject drawableObject, float alpha, RectangleF srcRect, RectangleF dstRect, bool includeImageAlpha)
        {
            CImage image = drawableObject as CImage;
            if (image != null)
            {
                DrawImageWithDiffuseAlpha(image, alpha, srcRect, dstRect.X, dstRect.Y + dstRect.Height, dstRect.X, dstRect.Y, dstRect.X + dstRect.Width, dstRect.Y + dstRect.Height, dstRect.X + dstRect.Width, dstRect.Y, includeImageAlpha);
                return;
            }

            Surface surface = drawableObject as Surface;
            if (surface != null)
            {
                DrawImageWithDiffuseAlpha(surface, alpha, srcRect, dstRect.X, dstRect.Y + dstRect.Height, dstRect.X, dstRect.Y, dstRect.X + dstRect.Width, dstRect.Y + dstRect.Height, dstRect.X + dstRect.Width, dstRect.Y, includeImageAlpha);
            }
        }

        public abstract void SetBilinearInterpolation(bool value);

        protected abstract void DrawImage(CImage image, RectangleF srcRect, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, bool alpha);
        protected abstract void DrawImage(Surface image, RectangleF srcRect, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, bool alpha);
        protected abstract void DrawImageWithDiffuseAlpha(CImage image, float alpha, RectangleF srcRect, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, bool includeImageAlpha);
        protected abstract void DrawImageWithDiffuseAlpha(Surface image, float alpha, RectangleF srcRect, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, bool includeImageAlpha);

        // Persistent means the RenderSurface handle is allowed to exists after EndScene is called 
        public abstract ActiveReader<RenderSurface> GeneratePersistentRenderSurface(uint width, uint height, bool doMipMapping, string id); // default 4 bytes per pixel
        public abstract ActiveReader<RenderSurface> GeneratePersistentRenderSurface(uint width, uint height, uint bytesPerPixel, bool doMipMapping, string id);

        // The returned renderSurface is not valid after EndScene is called, and dispose should have already been called on it

        public  RenderSurface GenerateRenderSurface(uint width, uint height, string id) // default 4 bytes per pixel
        {
            return GenerateRenderSurface(width, height, true, id);
        }
        public abstract RenderSurface GenerateRenderSurface(uint width, uint height, bool alpha, string id); // default 4 bytes per pixel
        public abstract RenderSurface GenerateRenderSurface(uint width, uint height, uint bytesPerPixel, string id);
        public abstract DynamicSurface GenerateDynamicSurface(uint width, uint height, DynamicSurface.Usage usage, string id, bool useMipMaps);

        public abstract void SetRenderTarget(RenderSurface surface);
        public abstract RenderSurface GetRenderTarget(); 

        public abstract void SetRenderTargetToDefault();
        public abstract void CopyDefaultSurfaceToImage(CImage image);
        public abstract void CopyDefaultSurfaceToDynamicSurface(DynamicSurface surface);
        public abstract void ClearRenderTarget(int r, int g, int b, int a);
        public abstract void ClearRenderTargetByBlankTexture();

        public abstract void Clean(); // Clean out any cached images ( e.g. when doing a Rebuild To New Canvas )

        public abstract State Reset();  // Call if PresentToWindows returns false
        public abstract State Reset(uint width, uint height);  // Call if want to change default render surface dimension (needed when encoding)

        public abstract int GetDefaultSurfaceWidth();
        public abstract int GetDefaultSurfaceHeight();
        public abstract void ResetDefaultWindowLocationAndSize(Point location, Size size); // Need to call this if you resize the default present window

        public abstract PixelShaderEffect CreatePixelShaderEffect(string name);
        public abstract void SetPixelShaderEffect(PixelShaderEffect effect);

        public static GraphicsEngine Current
        {
            get { return mGraphicsEngine; }
            set { mGraphicsEngine = value; }
        }

        static private GraphicsEngine mGraphicsEngine = null;

        // The graphics engine owns any rendersurface created, this is where we hold them
        protected List<RenderSurface> mInUseRenderSurfaces = new List<RenderSurface>();

        // method should only be called from monitor object
        public void RenderSurfaceNotReferenced(IMonitoredObject surface)  // callback
        {
            RenderSurface rs_surface = surface as RenderSurface;
            if (rs_surface != null)
            {
                RenderSurface rs = GetRenderTarget();
                if (rs == rs_surface)
                {
                    ManagedCore.CDebugLog.GetInstance().Error("Current Render Target not referenced by anything");
                }
                else
                {
                    rs_surface.Dispose();
                }

                if (mInUseRenderSurfaces.Remove(rs_surface) == false)
                {
                    ManagedCore.CDebugLog.GetInstance().Error("RenderSurface not owned by GraphicsEngine");
                }
            }
            else
            {
                ManagedCore.CDebugLog.GetInstance().Error("RenderSurfaceNotReferenced called with non RenderSurface");
            }
        }

        public abstract string GetLogCapabilitiesString();

        // returns the phsical texture dimension as used by the engine
        public abstract RectangleF GetRawImageDimension(CImage image);

        // returns the maximum texture width size the engine supports
        public abstract float GetMaxTextureWidth();


        public abstract System.Windows.Forms.Control HiddenWindow
        {
            get;
        }

        // Tells graphics enginge that we have change a CImage, so any cacheing of that image needs to be cleared
        public abstract void ClearCachedTexuresForImage(CImage image);

    }
}
