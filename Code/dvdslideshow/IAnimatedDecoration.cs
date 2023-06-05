using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml;
using System.Drawing.Drawing2D;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
    public interface IAnimatedDecoration
    {
       
     
        CAnimatedDecorationEffect MoveInEffect
        {
            get ;
            set ;
         
        }

        CAnimatedDecorationEffect MoveOutEffect
        {
            get ;
            set ;
           
        }

        int GetAdjustedFrameNum(int currentFrameNum);

        float GetStartOffsetTime(float slideDisplayLength);
      
        float GetEndOffsetTime(float slideDisplayLength);


        float GetLengthTimeShownFor(float slideDisplayLength);

        Rectangle RenderToGraphics(Graphics gp, RectangleF r, int frame_num, CSlide originating_slide, CImage original_image);
        Rectangle RenderToGraphics(RectangleF r, int frame_num, CSlide originating_slide, RenderSurface inputSurface);
    }
}
