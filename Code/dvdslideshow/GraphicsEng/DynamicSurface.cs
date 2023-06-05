
using System;
using System.Drawing;

namespace DVDSlideshow.GraphicsEng
{
    //
    // Represents a surface that can be locked and pixels read/writen too
    //
    // This class reprents a faster way to draw and update a dynamic surface ( e.g. used for rendering video ) than say using the CImage class
    //
    // You can not render to this type of surface
    //
    public abstract class DynamicSurface : Surface
    {
        public enum Usage
        {
            WRITE_ONLY,
            READ_WRITE
        };

        public abstract LockedRect Lock();
        public abstract void Unlock();

    }
}
