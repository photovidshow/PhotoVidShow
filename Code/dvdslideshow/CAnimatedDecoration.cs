using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml;
using System.Drawing.Drawing2D;

namespace DVDSlideshow
{
    public abstract class CAnimatedDecoration : CDecoration, IAnimatedDecoration
    {
        public const float SlideStart = 0.0f;
        public const float SlideEnd = 1000000.0f;

        private float startOffsetTime = SlideStart;
        private float endOffsetTime = SlideEnd;

        private string moveInEffect ="";
        private string moveOutEffect = "";

        private string mOriginalTemplateDefinedMoveInEffect = "";
        private string mOriginalTemplateDefinedmoveOutEffect = "";


        // if no effect defined, we can also move based on parent slide pan/zoom settings
        private bool mUseParentSlidePanZoomAsDefaultMovement = false;

        public string OriginalTemplateDefinedMoveInEffect
        {
            get { return mOriginalTemplateDefinedMoveInEffect; }
        }

        public string OriginalTemplateDefinedMoveOutEffect
        {
            get { return mOriginalTemplateDefinedmoveOutEffect; }
        }

        public bool UseParentSlidePanZoomAsDefaultMovement
        {
            get { return mUseParentSlidePanZoomAsDefaultMovement; }
            set { mUseParentSlidePanZoomAsDefaultMovement = value; }
        }

        //
        // Used to render this Animated Decoration via lots of sub animated decorations (e.g. split text characters)
        // This is calculated when needed as does not need to be copied or saved etc
        //
        protected List<IAnimatedDecoration> mSubAnimatedDocorations = null;

        public List<IAnimatedDecoration> SubAnimatedDecorations
        {
            get { return mSubAnimatedDocorations; }
        }

        public float StartOffsetTimeRawValue
        {
            get { return startOffsetTime; }
            set { startOffsetTime = value; }
        }

        private float GetOffsetTime(float offsetTime, float slideDisplayLength)
        {
            if (Math.Abs(offsetTime - SlideEnd) < 0.0001)
            {
                return slideDisplayLength;
            }
            return offsetTime;
        }

        public float GetStartOffsetTime(float slideDisplayLength)
        {
            return GetOffsetTime(startOffsetTime, slideDisplayLength);
        }


        public float EndOffsetTimeRawValue
        {
            get { return endOffsetTime; }
            set { endOffsetTime = value; }
        }

        public float GetEndOffsetTime(float slideDisplayLength)  // from start of time showing slide
        {
            return GetOffsetTime(endOffsetTime, slideDisplayLength);
        }

        public float GetLengthTimeShownFor(float slideDisplayLength)
        {
            return GetOffsetTime(endOffsetTime, slideDisplayLength) - GetStartOffsetTime(slideDisplayLength);
        }

        public string MoveInEffectName
        {
            get { return moveInEffect; }
            set { moveInEffect = value; }
        }

        public string MoveOutEffectName
        {
            get { return moveOutEffect; }
            set { moveOutEffect = value; }
        }

        public CAnimatedDecorationEffect MoveInEffect
        {
            get { return CAnimatedDecorationEffectDatabase.GetInstance().Get(moveInEffect); }
            set 
            {
                if (value == null)
                {
                    moveInEffect = "";
                }
                else
                {
                    moveInEffect = value.Name;
                }
            }
        }

        public CAnimatedDecorationEffect MoveOutEffect
        {
            get { return CAnimatedDecorationEffectDatabase.GetInstance().Get(moveOutEffect); }
            set 
            {
                if (value == null)
                {
                    moveOutEffect = "";
                }
                else
                {
                    moveOutEffect = value.Name;
                }
            }
        }



        //*******************************************************************
        public CAnimatedDecoration()
            : base()
        {
        }


        //*******************************************************************
        public CAnimatedDecoration(CAnimatedDecoration copy)
            : base(copy)
        {
            startOffsetTime = copy.startOffsetTime;
            endOffsetTime = copy.endOffsetTime;
            moveInEffect = copy.moveInEffect;
            moveOutEffect = copy.moveOutEffect; 
        }


        //*******************************************************************
        public CAnimatedDecoration(RectangleF coverage, int order) 
            : base( coverage, order) 
		{

		}

        //*******************************************************************
        // Called by child RenderToGraphics methods, to test if we should render or not
        protected bool InAnimatedTimeWindow(int framenum, CSlide originating_slide)
        {
            // if used just to render to some graphics (i.e. no slide, then always return true)
            if (originating_slide == null) return true;

            // If editing slide, framenum will be -1
            if (framenum == -1) return true;

            // If we are attached we don't do animated stuff
            if (mAttachedToSlideImage == true) return true;

            float slide_length = originating_slide.DisplayLength;
            if (framenum < originating_slide.mStartFrameOffset) return false;
            float time_since_start_of_slide = originating_slide.GetSecondsSinceStartOfSlide(framenum);
   
            if ( time_since_start_of_slide >= endOffsetTime) return false;
            if ( time_since_start_of_slide < startOffsetTime) return false;

            return true;
        }

        //*******************************************************************
        public int GetAdjustedFrameNum(int currentFrameNum)
        {
            //
            // Needed to implement IAnimatedDecoration (only used by sub animated decorations)
            //
            return currentFrameNum;
        }

        //*******************************************************************
        public void GenerateSubAnimations(int framenum, CSlide originating_slide)
        {
            if (originating_slide == null || framenum == -1)
            {
                mSubAnimatedDocorations = null;
                return;
            }

            float delta = 0;
            CAnimatedDecorationEffect effect = GetAnimatedDecorationEffectToUseFromFrameNumAndSlide(framenum, originating_slide, out delta);
            if (effect == null)
            {
                mSubAnimatedDocorations = null;
                return;
            }

            GenerateSubAnimations(effect, framenum);
        }
       
        //*******************************************************************
        protected virtual void GenerateSubAnimations(CAnimatedDecorationEffect effect, int framenum)
        {
            mSubAnimatedDocorations = null;
        }

        //*******************************************************************
        private CAnimatedDecorationEffect GetAnimatedDecorationEffectToUseFromFrameNumAndSlide( int framenum, CSlide originating_slide, out float delta)
        {
            delta = 0;

            float seconds_since_start_of_slide = originating_slide.GetSecondsSinceStartOfSlide(framenum);
            float time_since_start_showing_decoration = seconds_since_start_of_slide - GetStartOffsetTime(originating_slide.DisplayLength);

            CAnimatedDecorationEffect moveOutEffect = MoveOutEffect;
            if (moveOutEffect != null)
            {
                float time_since_start_of_move_out_start = -(-seconds_since_start_of_slide + GetEndOffsetTime(originating_slide.DisplayLength) - moveOutEffect.LengthInSeconds);

                if (time_since_start_of_move_out_start >= 0)
                {
                    delta = CGlobals.DeltaClamp(time_since_start_of_move_out_start / moveOutEffect.LengthInSeconds);

                    return moveOutEffect;
                }
            }

            CAnimatedDecorationEffect moveInEffect = MoveInEffect;
            if (moveInEffect != null)
            {
                float outEffectTime = 0;
                if (moveOutEffect != null)
                {
                    outEffectTime = moveOutEffect.LengthInSeconds;
                }

                // move in effect lengths may be set to be same as decoration length minus out effect time
                float InEffectlength = moveInEffect.GetLengthInSeconds(this.GetLengthTimeShownFor(originating_slide.DisplayLength) - outEffectTime);
                if (time_since_start_showing_decoration <= InEffectlength )
                {
                    delta = CGlobals.DeltaClamp(time_since_start_showing_decoration / InEffectlength);

                    return moveInEffect;
                }
            }
          
            // ok if we are here we are between inEffect and outEffect, process movement from inEffect delta 1 (if it exists)
            if (moveInEffect != null)
            {
                delta = 1;
                return moveInEffect;             
            }

            return null;

        }


        //*******************************************************************
        protected CRectangleReferenceFrame GetDecorationPosition(RectangleF coverageArea, RectangleF UVCoords, int framenum, CSlide originating_slide)
        {
            if (originating_slide == null ||
                framenum == -1 ||
                mAttachedToSlideImage)
            {
                return new CRectangleReferenceFrame(coverageArea, 0, 0, 0, UVCoords,0,0,0);
            }

            float delta =0;

            CAnimatedDecorationEffect effect = GetAnimatedDecorationEffectToUseFromFrameNumAndSlide(framenum, originating_slide, out delta);

            if (effect != null)
            {
                return effect.GetRectanglePosition(coverageArea, UVCoords, delta);
            }
            // perhaps move docoration based on parent pan/zoom settings
            else if (mUseParentSlidePanZoomAsDefaultMovement == true && originating_slide.UsePanZoom == true)
            {
                CImageSlide imageSlide = originating_slide as CImageSlide;
                if (imageSlide != null)
                {

                    float seconds_since_start_of_slide = originating_slide.GetSecondsSinceStartOfSlide(framenum);

                    delta = CGlobals.DeltaClamp(seconds_since_start_of_slide / originating_slide.DisplayLength);

                    return GetRectanglePosition(coverageArea, UVCoords, imageSlide.PanZoom, delta);
                }
            }

            // no effects, just return coverage area
            return new CRectangleReferenceFrame(coverageArea, 0, 0, 0, UVCoords,0,0,0);
        }


        //*******************************************************************
        // Calc animated CRectangleReferenceFrame from the parent CPanZoom movement effect
        private static CRectangleReferenceFrame GetRectanglePosition(RectangleF coverageArea, RectangleF UVCoords, CPanZoom panZoom, float delta)
        {
            float rotation = 0;
            RectangleF rec = panZoom.Process(delta, 1, 1, ref rotation);

            RectangleF newcoverage = coverageArea;

            // ok decoration centre movement from centre

            float originalDecorMoveX = newcoverage.X + (newcoverage.Width / 2);
            float originalDecorMoveY = newcoverage.Y + (newcoverage.Height / 2);
            originalDecorMoveX = originalDecorMoveX - 0.5f;
            originalDecorMoveY = originalDecorMoveY - 0.5f;

            // move decor to centre
            newcoverage.X -= originalDecorMoveX;
            newcoverage.Y -= originalDecorMoveY;

            float zoom = 1 / rec.Width;

            // apply zoom
            newcoverage.Width *= zoom;
            newcoverage.Height *= zoom;

            newcoverage.X -= (newcoverage.Width - coverageArea.Width) / 2.0f;
            newcoverage.Y -= (newcoverage.Height - coverageArea.Height) / 2.0f;

            float centrex = rec.X + (rec.Width / 2);
            float centrey = rec.Y + (rec.Height / 2);

            float movex = centrex - 0.5f;
            float movey = centrey - 0.5f;

            // apply movement from pan and also undo original movement from centre
            newcoverage.X -= (movex - originalDecorMoveX) * zoom;
            newcoverage.Y -= (movey - originalDecorMoveY) * zoom;

            // after we've calculated newcoverage area rect, the centre rotation point will always be around 0.5, 0.5 screen/display space
            // so work out offset from current newcoverage area

            originalDecorMoveX = newcoverage.X + (newcoverage.Width / 2);
            originalDecorMoveY = newcoverage.Y + (newcoverage.Height / 2);

            float xdiff = originalDecorMoveX - 0.5f;
            float ydiff = originalDecorMoveY - 0.5f;

            return new CRectangleReferenceFrame(newcoverage, -rotation, -xdiff, -ydiff, UVCoords,0,0,0);
        }


        //*******************************************************************
        protected Rectangle CalcDrawnAreaAfterRotation(RectangleF r1, System.Drawing.Drawing2D.Matrix mat, PointF [] out_points)
        {
            float offsetx = mat.OffsetX;
            float offsety = mat.OffsetY;

            PointF[] points = null;

            if (out_points != null)
            {
                points = out_points;
            }
            else
            {
                points = new PointF[4];
            }

            points[0] = new PointF(r1.X - offsetx, r1.Y - offsety);
            points[1] = new PointF((r1.X + r1.Width) - offsetx, r1.Y - offsety);
            points[2] = new PointF((r1.X + r1.Width) - offsetx, (r1.Y + r1.Height) - offsety);
            points[3] = new PointF(r1.X - offsetx, (r1.Y + r1.Height) - offsety);

            mat.TransformVectors(points);

            for (int i = 0; i < 4; i++)
            {
                points[i].X += offsetx;
                points[i].Y += offsety;
            }
          
            float min_x = points[0].X;
            float max_x = points[0].X;
            float max_y = points[0].Y;
            float min_y = points[0].Y;

            for (int i = 1; i < 4; i++)
            {
                if (points[i].X < min_x) min_x = points[i].X;
                if (points[i].X > max_x) max_x = points[i].X;
                if (points[i].Y < min_y) min_y = points[i].Y;
                if (points[i].Y > max_y) max_y = points[i].Y;
            }

            return new Rectangle((int)(min_x ), (int)(min_y ), 1+(int) ((max_x - min_x)+0.4999f), 1+(int) ((max_y - min_y)+0.4999f)); // need to add 1 to width and height not sure why
        }

        //*******************************************************************
        protected void SaveAnimatedDecorationPart(XmlElement element, XmlDocument doc)
        {
            if (startOffsetTime != SlideStart)
            {
                element.SetAttribute("StartOffsetTime", startOffsetTime.ToString());
            }

            if (endOffsetTime != SlideEnd)
            {
                element.SetAttribute("EndOffsetTime", endOffsetTime.ToString());
            }

            if (moveInEffect !="" )
            {
                element.SetAttribute("InEffect", moveInEffect);
            }

            if (moveOutEffect != "" )
            {
                element.SetAttribute("OutEffect", moveOutEffect);
            }

            if (mUseParentSlidePanZoomAsDefaultMovement == true)
            {
                element.SetAttribute("MoveAsSlidePanZoom", mUseParentSlidePanZoomAsDefaultMovement.ToString());
            }

            SaveDecrationPart(element, doc);
        }



        //*******************************************************************
        protected void LoadAnimatedDecorationPart(XmlElement element)
        {
            LoadDecrationPart(element);

            string s = element.GetAttribute("StartOffsetTime");
            if (s != "")
            {
                startOffsetTime = float.Parse(s);
            }

            s = element.GetAttribute("EndOffsetTime");
            if (s != "")
            {
                endOffsetTime = float.Parse(s);
            }

            s = element.GetAttribute("InEffect");
            if (s != "")
            {
                CAnimatedDecorationEffect effect = CAnimatedDecorationEffectDatabase.GetInstance().Get(s);

                if (effect!=null)
                {
                    moveInEffect = s;
                    if (effect.TemplateOnlyEffect == true)
                    {
                        mOriginalTemplateDefinedMoveInEffect = s;
                    }
                }
            }

            s = element.GetAttribute("OutEffect");
            if (s != "")
            {
                CAnimatedDecorationEffect effect = CAnimatedDecorationEffectDatabase.GetInstance().Get(s);

                if (effect !=null)
                {
                    moveOutEffect = s;
                    if (effect.TemplateOnlyEffect == true)
                    {
                        mOriginalTemplateDefinedmoveOutEffect = s;
                    }
                }
            }

            s = element.GetAttribute("MoveAsSlidePanZoom");
            if (s != "")
            {
                mUseParentSlidePanZoomAsDefaultMovement = bool.Parse(s);
            }
        }
    }
}
