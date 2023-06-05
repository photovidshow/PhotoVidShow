using System;
using System.Collections.Generic;
using System.Text;
using MangedToUnManagedWrapper.ManagedDirect3D;

namespace DVDSlideshow.GraphicsEng.Direct3D
{
    // Interface to a D3Dsurface, any child class needs to implement the ManagedRenderSurface function
    public interface ID3DSurface
    {
        ManagedSurface ManagedSurface
        {
            get;
        }
    }
}
