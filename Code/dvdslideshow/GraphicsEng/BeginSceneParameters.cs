using System;
using System.Collections.Generic;
using System.Text;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow.GraphicsEng
{
    public class BeginSceneParameters
    {
        public RenderSurface surface = null;       // null equals default surface
        public IntPtr present_window = new IntPtr();
        public int width;
        public int height;
    }
}
