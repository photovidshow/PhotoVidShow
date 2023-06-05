
using System;
using System.Drawing;

namespace DVDSlideshow.GraphicsEng
{
    //
    // Represents a surface that can be drawn to a RenderSurface
    //
    public abstract class Surface : DrawableObject, IDisposable
    {
        public abstract void Dispose();
        public abstract uint Width { get; }
        public abstract uint Height { get; }
    }
}
