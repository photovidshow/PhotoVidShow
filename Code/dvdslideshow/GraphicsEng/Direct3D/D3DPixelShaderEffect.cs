using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using MangedToUnManagedWrapper.ManagedDirect3D;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow.GraphicsEng.Direct3D
{
    public class D3DPixelShaderEffect : PixelShaderEffect
    {
        private ManagedPixelShaderEffect mManagedEffect;

        // **********************************************************************************
        public ManagedPixelShaderEffect ManagedPixelShaderEffect
        {
            get { return mManagedEffect; }
        }

        // **********************************************************************************
        public D3DPixelShaderEffect(string name)
            : base(name)
        {
            mManagedEffect = new ManagedPixelShaderEffect();
            mManagedEffect.Init(CGlobals.GetRootDirectory()+"\\shaders\\"+name+".psh");
        }

        // **********************************************************************************
        public override void SetParameter(int index, float value)
        {
           mManagedEffect.SetParameter(index, value); 
        }

        // **********************************************************************************
        public override void SetInputImage1(DrawableObject drawableObject)
        {
            Direct3DDevice device = Direct3DDevice.Current as Direct3DDevice;
            if (device != null)
            {
                CImage image = drawableObject as CImage;
                if (image != null)
                {
                    // As this is a pixel shader input texture, if the image is filebase, to prevent duplication of textures, any texture with
                    // the same filename will do (e.g. smooth edge alpha maps)
                    Texture t=  device.TextureManager.GetTextureForMatchingImageFilename(image);

                    if (t != null && t.ManagedTexture !=null)
                    {
                        mManagedEffect.SetTexture1(t.ManagedTexture);
                    }
                    return ;

                }

                ID3DSurface surface = drawableObject as ID3DSurface;
                if (surface != null)
                {
                    mManagedEffect.SetTexture1(surface.ManagedSurface);
                    return;
                }
            }
        }

        // **********************************************************************************
        public override void Dispose()
        {
            if (mManagedEffect != null)
            {
                mManagedEffect.Cleanup();
                mManagedEffect = null;
            }
        }

    }
}
