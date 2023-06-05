using System;
using System.Threading;
using System.Collections;
using System.Drawing;

namespace DVDSlideshow.GraphicsEng
{
    /// <summary>
    /// Interface to an object than can build GDI bitmap images 
    /// 
    /// This is implemented by classes that can generate a bitmap from code, i.e. rather than from an image on a disk
    /// </summary>
    public interface IImageCreator
    {
        Bitmap CreateImage(int width, int height, ref bool createMipMaps);
    }
}
