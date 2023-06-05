using System;
using System.Collections.Generic;
using System.Text;
using MangedToUnManagedWrapper.ManagedDirect3D;

namespace DVDSlideshow.GraphicsEng.Direct3D
{
    // This class is really a wrapper around a dynamic D3D Surface
    public class D3DDynamicSurface : DynamicSurface, ID3DSurface
    {
        private D3DSurface mSurface;

        public D3DDynamicSurface(D3DSurface surface) // passed in surface MUST be dynamic
        {
            mSurface = surface;
        }

        public D3DDynamicSurface(uint width, uint height, uint bytesPerPixel, DynamicSurface.Usage usage, string originator, bool forcePower2Texture)
        {
            if (usage == Usage.WRITE_ONLY)
            {
                mSurface = new D3DSurface(width, height, Texture.Usage.MANAGED, bytesPerPixel, false, originator, forcePower2Texture);
            }
            else
            {
                mSurface = new D3DSurface(width, height, Texture.Usage.SYSTEM_MEMORY, bytesPerPixel, false, originator, forcePower2Texture);
            }
        }

        public override void Dispose()
        {
            mSurface.Dispose();
        }

        public override uint Width
        {
            get { return mSurface.Width; }
        }

        public override uint Height 
        {
            get { return mSurface.Height; }
        }

        public override LockedRect Lock()
        {
            return mSurface.Lock();
        }

        public override void Unlock()
        {
            mSurface.Unlock();
        }

        public ManagedSurface ManagedSurface
        {
            get { return mSurface.ManagedSurface; }
        }
    }
}
