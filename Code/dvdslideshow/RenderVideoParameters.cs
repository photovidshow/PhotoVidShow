
using System;
using System.Drawing;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
    public class RenderVideoParameters
    {
        public delegate void RenderCallbackDelegate(RenderVideoParameters rvp);

        public int frame = -1;  // edit video mode 
        public bool ignore_pan_zoom = false;
        public bool ignore_decorations = false;
        public int req_width =10;
        public int req_height =10;
        public bool present_direct_to_window = false;
        public IntPtr present_window = new IntPtr();
        public RenderSurface render_to_surface = null;
        public bool do_begin_and_end_scene = true;
        public RenderCallbackDelegate postRenderDelegate = null;    
        public RenderCallbackDelegate preRenderDelegate = null;
        public Bitmap renderToBitmap = null;
        public bool scale_back_buffer_to_present_window = false;
    }
}
