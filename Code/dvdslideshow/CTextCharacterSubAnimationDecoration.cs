using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Globalization;
using System.Collections.Generic;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for CTextDecoration.
    /// </summary>
    public class CTextCharacterSubAnimationDecoration : IAnimatedDecoration
    {
        private CTextDecoration mParentTextDecoration;
        private float mTimeOffsetInSeconds;
        private int mCharacterIndex;

        //*******************************************************************
        public CTextCharacterSubAnimationDecoration(CTextDecoration parentTextDecoration, float timeOffsetInSeconds, int characterIndex)
        {
            mParentTextDecoration = parentTextDecoration;
            mTimeOffsetInSeconds = timeOffsetInSeconds;
            mCharacterIndex = characterIndex;
        }

        //*******************************************************************
        public CAnimatedDecorationEffect MoveInEffect
        {
            get { return mParentTextDecoration.MoveInEffect; }
            set { }
        }

        //*******************************************************************
        public CAnimatedDecorationEffect MoveOutEffect
        {
            get { return mParentTextDecoration.MoveOutEffect; }
            set { } 

        }

        //*******************************************************************
        public float GetStartOffsetTime(float slideDisplayLength)
        {
            return mParentTextDecoration.GetStartOffsetTime(slideDisplayLength);
        }

        //*******************************************************************
        public float GetEndOffsetTime(float slideDisplayLength)
        {
            return mParentTextDecoration.GetEndOffsetTime(slideDisplayLength);
        }

        //*******************************************************************
        public Rectangle RenderToGraphics(Graphics gp, RectangleF r, int frame_num, CSlide originating_slide, CImage original_image)
        {
            return new Rectangle(0, 0, (int)r.Width, (int)r.Height);
           
            // ### SRG TODO
         //   return mParentTextDecoration.RenderToGraphics(gp, r, frame_num, originating_slide, mCharacterIndex);
        }

        //*******************************************************************
        public Rectangle RenderToGraphics(RectangleF r, int frame_num, CSlide originating_slide, RenderSurface inputSurface)
        {
            return mParentTextDecoration.RenderToGraphics(r, frame_num, originating_slide, mCharacterIndex, null, null, inputSurface);
        }

        //*******************************************************************
        public int GetAdjustedFrameNum(int currentFrameNum)
        {
            int frameoffset = (int)((mTimeOffsetInSeconds * CGlobals.mCurrentProject.DiskPreferences.frames_per_second) + 0.5f);
            currentFrameNum += frameoffset;
            return currentFrameNum;
        }


        //*******************************************************************
        public float GetLengthTimeShownFor(float slideDisplayLength)
        {
            return mParentTextDecoration.GetLengthTimeShownFor(slideDisplayLength);
        }

     
    }
}
