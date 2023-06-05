using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for MoveToNextSlideTransitionEffectStaticNextSlide.
    /// </summary>
    public class MoveToNextSlideTransitionEffectStaticNextSlide : CTransitionEffect
    {
        private CNonLinear mMovementModel = new CNonLinear();

        SwipeDirection mDirection = SwipeDirection.RIGHT;
    
        public SwipeDirection Direction
        {
            get { return mDirection; }
        }


        public MoveToNextSlideTransitionEffectStaticNextSlide()
        {
            this.mNeedsDualRenderSurface = true;
        }

        public enum SwipeDirection
        {
            LEFT,
            RIGHT,
            UP,
            DOWN
        } 

        public MoveToNextSlideTransitionEffectStaticNextSlide(float length)
            : base(length)
        {
            this.mNeedsDualRenderSurface = true;
        }


        public MoveToNextSlideTransitionEffectStaticNextSlide(float length, SwipeDirection d)
            : base(length)
        {
            mDirection = d;
            this.mNeedsDualRenderSurface = true;
        }


        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            CTransitionEffect t = new MoveToNextSlideTransitionEffectStaticNextSlide(mLength, mDirection);
            t.Index = this.Index;
            return t;
        }

        //*******************************************************************
        protected override void ProcessDualRenderSurfaces(float delta, RenderSurface thisSlideSurface, RenderSurface nextSlideSurface, int frame)
        {
            if (thisSlideSurface == null && nextSlideSurface == null)
            {
                ManagedCore.Log.Error("No surfaces provided in MoveToNextSlideTransitionEffectStaticNextSlide::ProcessDualRenderSurfaces");
                return;
            }
 
            switch (mDirection)
            {
                case SwipeDirection.RIGHT:
                {
                    doRight(thisSlideSurface, nextSlideSurface, delta);
                    break;
                }
                case SwipeDirection.LEFT:
                {
                    doLeft(thisSlideSurface, nextSlideSurface, delta);
                    break;
                }
                case SwipeDirection.UP:
                {
                    doUp(thisSlideSurface, nextSlideSurface, delta);
                    break;
                }
                case SwipeDirection.DOWN:
                {
                    doDown(thisSlideSurface, nextSlideSurface, delta);
                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        //*******************************************************************
        private void doLeft(RenderSurface thisSlideSurface, RenderSurface nextSlideSurface, float delta)
        {
            GraphicsEngine engine = GraphicsEngine.Current;

            int i1width = 0;
            if (thisSlideSurface != null)
            {
                i1width = ((int)thisSlideSurface.Width);
            }
            else
            {
                i1width = ((int)nextSlideSurface.Width);
            }

            int dist = i1width;

            float dd = mMovementModel.Get(0, 1, delta);

            float x_offset = dist * dd;

            float width = i1width - x_offset;
            if (width > 0 && thisSlideSurface!=null)
            {
                engine.DrawImage(thisSlideSurface,
                               new RectangleF(0, 0, width / thisSlideSurface.Width, 1),
                               new RectangleF(x_offset, 0, width, thisSlideSurface.Height), false);
            }

            float other_x_offset = i1width - x_offset;
            float other_width = i1width - other_x_offset;

            if (other_x_offset < i1width && other_width > 0 && nextSlideSurface != null)
            {
                engine.DrawImage(nextSlideSurface,
                            new RectangleF(0, 0, 1 - (other_x_offset / nextSlideSurface.Width), 1),
                            new RectangleF(0, 0, other_width, nextSlideSurface.Height), false);

            } 
        }


        //*******************************************************************
        private void doRight(RenderSurface thisSlideSurface, RenderSurface nextSlideSurface, float delta)
        {
            GraphicsEngine engine = GraphicsEngine.Current;

            int i1width = 0;
            if (thisSlideSurface != null)
            {
                i1width = ((int)thisSlideSurface.Width);
            }
            else
            {
                i1width = ((int)nextSlideSurface.Width);
            }

            int dist = i1width;

            float dd = mMovementModel.Get(0, 1, delta);

            float x_offset = dist * dd;

            float width = i1width - x_offset;
            if (width > 0 && thisSlideSurface!=null)
            {
                engine.DrawImage(thisSlideSurface,
                                new RectangleF(x_offset / thisSlideSurface.Width, 0, 1, 1),
                                new RectangleF(0, 0, width, thisSlideSurface.Height), false);
            }

            float other_x_offset = i1width - x_offset;
            float other_width = i1width - other_x_offset;

            if (other_x_offset < i1width && other_width > 0 && nextSlideSurface!=null)
            {
           
                engine.DrawImage(nextSlideSurface,
                                 new RectangleF(1-(other_width / nextSlideSurface.Width), 0, 1 , 1),
                                 new RectangleF(other_x_offset, 0, other_width, nextSlideSurface.Height), false);
            }
        }


        //*******************************************************************
        private void doUp(RenderSurface thisSlideSurface, RenderSurface nextSlideSurface, float delta)
        {
            GraphicsEngine engine = GraphicsEngine.Current;

            int i1height = 0;
            if (thisSlideSurface != null)
            {
                i1height = ((int)thisSlideSurface.Height);
            }
            else
            {
                i1height = ((int)nextSlideSurface.Height);
            }

            int dist = i1height;

            float dd = mMovementModel.Get(0, 1, delta);

            float y_offset = dist * dd;

            float height = i1height - y_offset;
            if (height > 0 && thisSlideSurface!=null)
            {
                engine.DrawImage(thisSlideSurface,
                                 new RectangleF(0, y_offset / thisSlideSurface.Height, 1, 1),
                                 new RectangleF(0, 0, thisSlideSurface.Width, height), false);
            }

            float other_y_offset = i1height - y_offset;
            float other_height = i1height - other_y_offset;

            if (other_y_offset < i1height && other_height > 0 && nextSlideSurface != null)
            {
                engine.DrawImage(nextSlideSurface,
                    new RectangleF(0, 1-(other_height / nextSlideSurface.Height), 1, 1 ),
                    new RectangleF(0, other_y_offset, nextSlideSurface.Width, other_height), false);
            }
        }

        //*******************************************************************
        private void doDown(RenderSurface thisSlideSurface, RenderSurface nextSlideSurface, float delta)
        {
            GraphicsEngine engine = GraphicsEngine.Current;

            int i1height = 0;
            if (thisSlideSurface != null)
            {
                i1height = ((int)thisSlideSurface.Height);
            }
            else
            {
                i1height = ((int)nextSlideSurface.Height);
            }

            int dist = i1height;

            float dd = mMovementModel.Get(0, 1, delta);

            float y_offset = dist * dd;

            float height = i1height - y_offset;
            if (height > 0 && thisSlideSurface!=null)
            {
                engine.DrawImage(thisSlideSurface,
                               new RectangleF(0, 0, 1, height / thisSlideSurface.Height),
                               new RectangleF(0, y_offset, thisSlideSurface.Width, height), false);

            }

            float other_y_offset = i1height - y_offset;
            float other_height = i1height - other_y_offset;

            if (other_y_offset < i1height && other_height > 0 && nextSlideSurface != null)
            {
                engine.DrawImage(nextSlideSurface,
                             new RectangleF(0, 0, 1, 1-(other_y_offset / nextSlideSurface.Height)),
                             new RectangleF(0, 0, nextSlideSurface.Width, other_height), false);
            }
        }


        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
        {
            effect.SetAttribute("Type", "MoveToNextSlideTransitionEffectStaticNextSlide");
            effect.SetAttribute("Direction", ((int)this.mDirection).ToString());
            SaveTransitionPart(effect, doc);

            parent.AppendChild(effect);
        }


        //*******************************************************************
        public override void Load(XmlElement element)
        {
            string s1 = element.GetAttribute("Direction");
            if (s1 != "")
            {
                mDirection = (SwipeDirection)int.Parse(s1);
            }        

            LoadTransitionPart(element);
        }
    }
}
