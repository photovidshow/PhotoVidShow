using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using MangedToUnManagedWrapper.ManagedDirect3D;
using DVDSlideshow.GraphicsEng;
using System.Windows.Forms;
using ManagedCore;

namespace DVDSlideshow.GraphicsEng.Direct3D
{
    public class Direct3DDevice : GraphicsEngine
    {
        private ManagedDirect3DDevice mManagedDevice = null;
        private TextureManager mTextureManager = new TextureManager();
        private D3DSurfaceCache mRenderSurfaceCache = new D3DSurfaceCache();
        private RenderSurface mCurrentRenderSurface = null;
        private Control mParentWindow = null;       // The window used the create the D3D device with
        private Control mHiddenWindow = null;
        private bool mBegin = false;
        private D3DSettings mSettings = null;
        private D3DCapabilities mCapabilities = null;
        private CImage mBlankImage = null;
     
        public Direct3DDevice()
        {
         
        }

        override public bool ForcePower2Textures
        {
            set 
            {
                base.ForcePower2Textures = value;
                mTextureManager.ForcePower2Textures = value;
            }
        }

        public D3DSettings Settings
        {
            get { return mSettings; }
        }

        public D3DCapabilities Capabilities
        {
            get { return mCapabilities; }
        }

        public override Control HiddenWindow
        {
            get { return mHiddenWindow; }
        }

        public D3DSurfaceCache D3DSurfaceCache
        {
            get { return mRenderSurfaceCache; }
        }

        public TextureManager TextureManager
        {
            get { return mTextureManager; }
        }

        public override GraphicsEngine.State BeginScene(BeginSceneParameters parameters)
        {
            if (mBegin == true)
            {
                Log.Warning("D3D scene already begun (ignoring)");
            }

            mBegin = true;

            GraphicsEngine engine = GraphicsEngine.Current;

            GraphicsEngine.State state = engine.GetLastKnownState();

            if (state != GraphicsEngine.State.OK)
            {
                if (state == GraphicsEngine.State.LOST_DEVICE)
                {
                    Log.Info("Device lost....");
                    state = RefreshState();
                    if (state == GraphicsEngine.State.LOST_DEVICE)
                    {
                        System.Threading.Thread.Sleep(1000);
                        mBegin = false;
                        return state;
                    }
                }

                if (state == GraphicsEngine.State.NOT_RESET)
                {
                    state = engine.Reset();
                    if (state != GraphicsEngine.State.OK)
                    {
                        System.Threading.Thread.Sleep(1000);
                        Log.Error("D3D graphics engine reset failed");
                        mBegin = false;
                        return state;
                    }
                }
            }

            ManagedSurface managed_d3d_surface = null;

            IntPtr hwnd = new IntPtr(0);
            int width = 0;
            int height = 0;
            if (parameters != null)
            {
                D3DSurface d3dsurface = parameters.surface as D3DSurface;

                if (d3dsurface != null)
                {
                    managed_d3d_surface = d3dsurface.ManagedSurface;
                }
                hwnd = parameters.present_window;
                width = parameters.width;
                height = parameters.height;
            }

            mManagedDevice.BeginScene(managed_d3d_surface, hwnd, width, height);

            return state;
        }

        //*******************************************************************
        public override void EndScene()
        {
            if (mBegin == false)
            {
                Log.Warning("D3D scene not begun on call to EndScene");
            }

            mBegin = false;

            mManagedDevice.EndScene();
        }

        //*******************************************************************
        public override GraphicsEngine.State PresentToWindow(IntPtr alternativeWindowHandle, bool scaleToBackBuffer)
        {
            mLastKnownState = (GraphicsEngine.State)mManagedDevice.PresentToWindow(alternativeWindowHandle, scaleToBackBuffer);
            return mLastKnownState;
        }


        //*******************************************************************
        // Must be called on the windows messaging thread.  The passed in parent Control must have also been created on this thread.
        public GraphicsEngineInitialiseResult Initialise(Control control, uint width, uint height, Control hiddenWindow)
        {
            mParentWindow = control;
            mHiddenWindow = hiddenWindow;

            mManagedDevice = new ManagedDirect3DDevice(control.Handle, width, height);

            bool hasCreated = mManagedDevice.HalDeviceCreated();
            if (hasCreated)
            {
                mSettings = new D3DSettings();
                mCapabilities = new D3DCapabilities();

                // If Windows XP use managed pool textures by default
                if (CGlobals.IsWinVistaOrHigher() == false)
                {
                    mSettings.SetUseManagedPoolForNormalTextures(true);
                }
            }

            GraphicsEngineInitialiseResult result = null;

            if (hasCreated == true)
            {
                result = new GraphicsEngineInitialiseResult(hasCreated, GenerateCapabilities());
            }
            else
            {
                result = new GraphicsEngineInitialiseResult(hasCreated, new GraphicsEngineCapabilities());
            }

            return result;
        }

        //*******************************************************************
        private GraphicsEngineCapabilities GenerateCapabilities()
        {
            GraphicsEngineCapabilities capabilities = new GraphicsEngineCapabilities();

            capabilities.CanDo64BitTextures = mCapabilities.GetCanDo64BitTextures();
            capabilities.CanDoAutoMipMappingCreation = mCapabilities.GetCanDoAutoMipMappingCreation();
            capabilities.CanDoNonPower2Textures = mCapabilities.GetCanDoNonPower2Textures();
            capabilities.CanDoPixelShader20 = mCapabilities.GetCanDoPixelShader20();
            capabilities.GetCanDoTrilinearFiltering = mCapabilities.GetCanDoTrilinearFiltering();
            capabilities.MaxTextureHeight = mCapabilities.GetMaxTextureHeight();
            capabilities.MaxTextureWidth = mCapabilities.GetMaxTextureWidth();

            return capabilities;
        }

        //*******************************************************************
        protected override void DrawImage(CImage image, RectangleF srcRect, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, bool alpha)
        {
            Size requestedTextureDimension = CalcRequestTextureDimension(x1, y1, x2, y2, x3, y3, x4, y4, srcRect);
            // Retrieve direct3d Texture (i.e. verify its loaded on graphics card)

            Texture texture = mTextureManager.GetTextureForImage(image, requestedTextureDimension);

            if (texture.ManagedTexture != null)
            {
                mManagedDevice.DrawImage(texture.ManagedTexture, srcRect, x1, y1, x2, y2, x3, y3, x4, y4, alpha);
            }
        }

        //*******************************************************************
        protected override void DrawImage(Surface image, RectangleF srcRect, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, bool alpha)
        {
            ID3DSurface d3dimage = image as ID3DSurface;
            if (d3dimage != null)
            {
                mManagedDevice.DrawImage(d3dimage.ManagedSurface, srcRect, x1, y1, x2, y2, x3, y3, x4, y4, alpha);
            }
        }

        //*******************************************************************
        protected override void DrawImageWithDiffuseAlpha(CImage image, float alpha, RectangleF srcRect, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, bool includeImageAlpha)
        {
            Size requestedTextureDimension = CalcRequestTextureDimension(x1, y1, x2, y2, x3, y3, x4, y4, srcRect);

            // Retrieve direct3d Texture (i.e. verify its loaded on graphics card)
            Texture texture = mTextureManager.GetTextureForImage(image, requestedTextureDimension);

            if (texture.ManagedTexture != null)
            {
                mManagedDevice.DrawImageWithDiffuseAlpha(texture.ManagedTexture, alpha * 255, srcRect, x1, y1, x2, y2, x3, y3, x4, y4, includeImageAlpha);
            }
        }

        //*******************************************************************
        protected override void DrawImageWithDiffuseAlpha(Surface image, float alpha, RectangleF srcRect, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, bool includeImageAlpha)
        {
            ID3DSurface d3dimage = image as ID3DSurface;
            if (d3dimage != null)
            {
                mManagedDevice.DrawImageWithDiffuseAlpha(d3dimage.ManagedSurface, alpha * 255, srcRect, x1, y1, x2, y2, x3, y3, x4, y4, includeImageAlpha);
            }
        }

        //*******************************************************************
        public override ActiveReader<RenderSurface> GeneratePersistentRenderSurface(uint width, uint height, bool doMipMapping, string id)
        {
            ActiveReader<RenderSurface> reader = new ActiveReader<RenderSurface>(mRenderSurfaceCache.GetSurface(width, height, Texture.Usage.RENDER_TARGET, 4, doMipMapping, id, mForcePower2Textures));

            reader.Object.Monitor.NoMoreReaders += new NoMoreReadersCallback(this.RenderSurfaceNotReferenced);
            mInUseRenderSurfaces.Add(reader.Object);
            return reader;
        }

        //*******************************************************************
        public override ActiveReader<RenderSurface> GeneratePersistentRenderSurface(uint width, uint height, uint bytesPerPixel, bool doMipMapping, string id)
        {
            ActiveReader<RenderSurface> reader = new ActiveReader<RenderSurface>(mRenderSurfaceCache.GetSurface(width, height, Texture.Usage.RENDER_TARGET, bytesPerPixel, doMipMapping, id, mForcePower2Textures));

            reader.Object.Monitor.NoMoreReaders += new NoMoreReadersCallback(this.RenderSurfaceNotReferenced);
            mInUseRenderSurfaces.Add(reader.Object);
            return reader;
        }

        //*******************************************************************
        public override RenderSurface GenerateRenderSurface(uint width, uint height, bool alpha, string id)
        {
            if (alpha == true)
            {
                return mRenderSurfaceCache.GetSurface(width, height, Texture.Usage.RENDER_TARGET, 4, id, mForcePower2Textures);
            }
            else
            {
                return mRenderSurfaceCache.GetSurface(width, height, Texture.Usage.RENDER_TARGET_WITH_NO_ALPHA, 4, id, mForcePower2Textures);
            }
        }

        //*******************************************************************
        public override RenderSurface GenerateRenderSurface(uint width, uint height, uint bytesPerPixel, string id)
        {
            return mRenderSurfaceCache.GetSurface(width, height, Texture.Usage.RENDER_TARGET, bytesPerPixel, id, mForcePower2Textures);
        }

        //*******************************************************************
        public override DynamicSurface GenerateDynamicSurface(uint width, uint height, DynamicSurface.Usage usage, string id, bool useMipMaps)
        {
            if (usage == DynamicSurface.Usage.WRITE_ONLY)
            {
                return new D3DDynamicSurface(mRenderSurfaceCache.GetSurface(width, height, Texture.Usage.MANAGED,4, useMipMaps, id, mForcePower2Textures));
            }
            return new D3DDynamicSurface(mRenderSurfaceCache.GetSurface(width, height, Texture.Usage.SYSTEM_MEMORY, 4, id, mForcePower2Textures));
        }

        //*******************************************************************
        public void FreeRenderSurface(D3DSurface surface)
        {
            mRenderSurfaceCache.FreeSurface(surface);
            if (mCurrentRenderSurface == surface)
            {
                mCurrentRenderSurface = null;
            }
        }

        //*******************************************************************
        public override void SetRenderTarget(RenderSurface surface)
        {
            if (surface == null)
            {
                SetRenderTargetToDefault();
                return;
            }

            D3DSurface d3dsurface = surface as D3DSurface;
            if (d3dsurface != null)
            {
                mManagedDevice.SetRenderTarget(d3dsurface.ManagedSurface);
                mCurrentRenderSurface = surface;
            }
        }

        //*******************************************************************
        public override RenderSurface GetRenderTarget()
        {
            return mCurrentRenderSurface;
        }

        //*******************************************************************
        public override void ClearRenderTarget(int r, int g, int b, int a)
        {
            mManagedDevice.ClearRenderTarget(r, g, b, a);
        }

        //***************************************************************************
        // This method will blank the current render target by drawing a blank texture on it.
        // This is because Clear() method above does not always work in some circumstances with some drivers (e.g. RT surface with mipmapping on radeon R7xxx drivers)
        public override void ClearRenderTargetByBlankTexture()
        {
            if (mCurrentRenderSurface==null)
            {
                return;
            }

            if (mBlankImage==null)
            {
                mBlankImage = new CImage(new Bitmap(5, 5, PixelFormat.Format32bppArgb));
            }

            SetBilinearInterpolation(false);
            DrawImage(mBlankImage, new RectangleF(0, 0, 1, 1), new RectangleF(0, 0, mCurrentRenderSurface.Width, mCurrentRenderSurface.Height), false);
            SetBilinearInterpolation(true);
        }

        //*******************************************************************
        public override void SetRenderTargetToDefault()
        {
            mCurrentRenderSurface = null;
            mManagedDevice.SetRenderTargetToBackBuffer();
        }

        //*******************************************************************
        protected override void SetDefaultSurfaceViewport(uint width, uint height)
        {
            mManagedDevice.SetViewport(width, height);
        }

        //*******************************************************************
        public override void CopyDefaultSurfaceToDynamicSurface(DynamicSurface surface)
        {
            ID3DSurface d3drs = surface as ID3DSurface;
            if (d3drs != null)
            {
                //
                // Calls to this has been reported to crash on a clients Samsng laptop
                //
                try
                {
                   mManagedDevice.CopyDefaultSurfaceToDynamicSurface(d3drs.ManagedSurface);
                }
                catch (Exception e)
                {
                    // 
                    // Determine what stage it crashed on
                    //
                    int stage = mManagedDevice.GetLastCopyDefaultSurfaceToDynamicSurfaceStage();

                    Log.Error("Exception occurred on call to CopyDefaultSurfaceToDynamicSurface. Stage:" + stage + ", exception:" + e.Message);

                    throw new Exception("Stage exception occurred was:" + stage + "\r\n" + e.Message);
                }
            }
            else
            {
                Log.Error("ID3DSurface null on call to CopyDefaultSurfaceToDynamicSurface");
            }
        } 

        //*******************************************************************
        public override void CopyDefaultSurfaceToImage(CImage image)
        {
            Bitmap b = image.GetRawImage() as Bitmap;

            BitmapData iImageData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            System.IntPtr Scan0 = iImageData.Scan0;

            int stride_d = (iImageData.Stride - (b.Width * 4)) >> 2;

            mManagedDevice.CopyDefaultSurfaceToBitmap(Scan0, b.Width, b.Height, stride_d);

            b.UnlockBits(iImageData);

        }

        //*******************************************************************
        public override void Clean()
        {
            // Remove all cached textures on GPU
            mTextureManager.Clean();
            CImagesCache.GetInstance().ClearCache();
        }

        //*******************************************************************
        public override PixelShaderEffect CreatePixelShaderEffect(string name)
        {
            return new D3DPixelShaderEffect(name);
        }

        //*******************************************************************
        public override void SetPixelShaderEffect(PixelShaderEffect effect)
        {
            if (effect == null)
            {
                mManagedDevice.SetPixelShaderEffect(null);
                return ;
            }

            D3DPixelShaderEffect d3deffect = effect as D3DPixelShaderEffect;
           
            if (effect != null)
            {
                mManagedDevice.SetPixelShaderEffect(d3deffect.ManagedPixelShaderEffect);
            }
        }

        //*******************************************************************
        // TestCooperativeLevel() must be called on the windows messaging thread
        private delegate GraphicsEngine.State RefreshStateDelegate();
        public override GraphicsEngine.State RefreshState()
        {
            if (mParentWindow.InvokeRequired == true)
            {
                IAsyncResult aResult = mParentWindow.BeginInvoke(new RefreshStateDelegate(RefreshState));
                aResult.AsyncWaitHandle.WaitOne();
                return (GraphicsEngine.State)mParentWindow.EndInvoke(aResult); ;
            }

            mLastKnownState = (GraphicsEngine.State)mManagedDevice.TestCooperateLevel();
            return mLastKnownState;
        }

        //*******************************************************************
        private void DisposeRenderSurfaces()
        {
            // Ok clear up D3DPOOL_DEFAULT created objects 
            foreach (RenderSurface surface in mInUseRenderSurfaces)
            {
                Log.Info("Found an active RenderSurface, setting reader to null");
                surface.Dispose();  // informs any readers that we're about to be disposed, then passes it to the cache
            }

            mInUseRenderSurfaces.Clear();

            this.mRenderSurfaceCache.FreeAllSurfaces();  // clean the cache

        }

        //*******************************************************************
        // Reset() must be called on the windows messaging thread
        private delegate GraphicsEngine.State ResetDelegate();
        public override GraphicsEngine.State Reset()
        {
            if (mParentWindow.InvokeRequired == true)
            {
                IAsyncResult aResult = mParentWindow.BeginInvoke(new ResetDelegate(Reset));
                aResult.AsyncWaitHandle.WaitOne();
                return (GraphicsEngine.State)mParentWindow.EndInvoke(aResult); ;
            }

            Log.Info("Reset called on d3d device");

            mTextureManager.Clean();    // remove textures
            DisposeRenderSurfaces();    // remove surfaces
          
            mLastKnownState = (GraphicsEngine.State) mManagedDevice.Reset();
            return mLastKnownState;
        }

        //*******************************************************************
        // Reset(width, height) must be called on the windows messaging thread
        private delegate GraphicsEngine.State ResetWidthDimensionDelegate(uint width, uint height);
        public override State Reset(uint width, uint height)
        {
            if (mParentWindow.InvokeRequired == true)
            {
                IAsyncResult aResult = mParentWindow.BeginInvoke(new ResetWidthDimensionDelegate(Reset), new object[] { width, height } );
                aResult.AsyncWaitHandle.WaitOne();
                return (GraphicsEngine.State)mParentWindow.EndInvoke(aResult); ;
            }

            Log.Info("Reset called on d3d device width="+width+ ", height="+height);

            mTextureManager.Clean();    // remove textures
            DisposeRenderSurfaces();    // remove surfaces

            mLastKnownState = (GraphicsEngine.State) mManagedDevice.Reset(width, height);
            SetDefaultSurfaceViewport(width, height);
            return mLastKnownState;
       }


        //*******************************************************************
        public override int GetDefaultSurfaceWidth()
        {
            return mManagedDevice.GetBackBufferWidth();
        }


        //*******************************************************************
        public override int GetDefaultSurfaceHeight()
        {
            return mManagedDevice.GetBackBufferHeight();
        }

        //*******************************************************************
        public override void ResetDefaultWindowLocationAndSize(Point location, Size size)
        {
            mManagedDevice.ResetDefaultWindowLocationAndSize(location.X, location.Y, size.Width, size.Height);
        }


        //*******************************************************************
        public override RectangleF GetRawImageDimension(CImage image)
        {
            Texture t = mTextureManager.GetTextureForImage(image);
            if (t != null)
            {
                return new RectangleF(0, 0, t.Width, t.Height);
            }
            return new RectangleF(0, 0, 1, 1);
        }


        //*******************************************************************
        public override float GetMaxTextureWidth()
        {
            return 4096;
        }

        //*******************************************************************
        private Size CalcRequestTextureDimension(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, RectangleF srcRec)
        {
            float xx = x3 - x1;
            xx *=xx;
            float yy = y3 - y1;
            yy *=yy;

            float width = (float)Math.Sqrt(xx+yy);

            xx = x2 - x1;
            xx *= xx;
            yy = y2 - y1;
            yy *= yy;

            float height = (float) Math.Sqrt(xx + yy);

            float srcxfrac = 1.0f / (srcRec.Width - srcRec.X);
            width *= srcxfrac;

            float srcyfrac = 1.0f / (srcRec.Height - srcRec.Y);
            height *= srcyfrac;

            width = Math.Abs(width);
            height = Math.Abs(height);

            return new Size((int)(width+0.4999f), (int)(height+0.4999f));
        }

        //*******************************************************************
        public uint GetAvailableTextureMem()
        {
            return mManagedDevice.GetAvailableTextureMem();
        }

        //*******************************************************************
        public override void SetBilinearInterpolation(bool value)
        {
            if (value == true)
            {
                mManagedDevice.SetBilinearInterpolation(1);
            }
            else
            {
                mManagedDevice.SetBilinearInterpolation(0);
            }
        }

        //*******************************************************************
        public override void ClearCachedTexuresForImage(CImage image)
        {
             mTextureManager.RemoveCachedTexuresForImage(image);
        }

        // ************************************************************************************************************
        public override string GetLogCapabilitiesString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("Graphics card capabilities\r\n");
            stringBuilder.Append("===========================================\r\n");
            stringBuilder.AppendFormat("Using managed pool for normal textures:{0}\r\n", Settings.GetUseManagedPoolForNormalTextures().ToString());
            stringBuilder.AppendFormat("Max texture size:{0},{1}\r\n", Capabilities.GetMaxTextureWidth().ToString(), Capabilities.GetMaxTextureHeight().ToString());
            stringBuilder.AppendFormat("Supports 64bit render targets:{0}\r\n", Capabilities.GetCanDo64BitTextures().ToString());
            stringBuilder.AppendFormat("Can do auto mip-mapping:{0}\r\n", Capabilities.GetCanDoAutoMipMappingCreation().ToString());
            stringBuilder.AppendFormat("Supports pixel shader 2.0:{0}\r\n", Capabilities.GetCanDoPixelShader20().ToString());
            stringBuilder.AppendFormat("Support tri-linear filtering:{0}\r\n", Capabilities.GetCanDoTrilinearFiltering().ToString());
            stringBuilder.AppendFormat("Supports non pow2 textures:{0}\r\n", Capabilities.GetCanDoNonPower2Textures().ToString());
            stringBuilder.AppendFormat("Reported total graphics memory:{0} Mb\r\n", GetAvailableTextureMem() /1024 /1024);
            stringBuilder.Append("===========================================");

            return stringBuilder.ToString();
        }


    } 
}
