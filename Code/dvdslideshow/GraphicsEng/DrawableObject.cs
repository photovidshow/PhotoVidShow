
using System;
using System.Drawing;

namespace DVDSlideshow.GraphicsEng
{
    // Represents an object than can be drawn in the Graphics Engine
    public abstract class DrawableObject
    {
        protected bool mCreateMipMaps = true;                    // gives hint to graphics engine if mipmaps should be created for this image

        public bool CreateMipMaps
        {
            get { return mCreateMipMaps; }
            set { mCreateMipMaps = value; }
        }
    }
}
