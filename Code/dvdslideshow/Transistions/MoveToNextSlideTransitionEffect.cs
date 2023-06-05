using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for MoveToNextSlideTransitionEffect.
    /// </summary>
    public class MoveToNextSlideTransitionEffect : CTransitionEffect
    {
        private CEquation mMovementModel = new CNonLinear();
        private SolidBrush mBlackBrush = new SolidBrush(Color.Black);
        private SwipeDirection mDirection = SwipeDirection.RIGHT;
        private float mBorderFraction = 40;

        //*******************************************************************
        public SwipeDirection Direction
        {
            get { return mDirection; }
        }

        //*******************************************************************
        public CEquation MovementModel
        {
            get { return mMovementModel; }
            set { mMovementModel = value; }
        }

        //*******************************************************************
        public MoveToNextSlideTransitionEffect()
        {
            this.mNeedsDualRenderSurface = true;
            mForecedMotionBlur = 30;
        }

        //*******************************************************************
        public enum SwipeDirection
        {
            LEFT,
            RIGHT,
            UP,
            DOWN
        }

        //*******************************************************************
        public MoveToNextSlideTransitionEffect(float length)
            : base(length)
        {
            this.mNeedsDualRenderSurface = true;
            mForecedMotionBlur = 30;
        }

        //*******************************************************************
        public MoveToNextSlideTransitionEffect(float length, SwipeDirection d, float fraction)
            : base(length)
        {
            mDirection = d;
            mBorderFraction = fraction;
            this.mNeedsDualRenderSurface = true;
            mForecedMotionBlur = 30;
        }


        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            MoveToNextSlideTransitionEffect t = new MoveToNextSlideTransitionEffect(mLength, mDirection, mBorderFraction);
            t.Index = this.Index;
            t.MovementModel = mMovementModel;
            t.mForecedMotionBlur = mForecedMotionBlur;
            return t;
        }

        //*******************************************************************
        protected override void ProcessDualRenderSurfaces(float delta, RenderSurface thisSlideSurface, RenderSurface nextSlideSurface, int frame)
        {
            if (thisSlideSurface == null && nextSlideSurface == null)
            {
                ManagedCore.Log.Error("No surfaces provided in MoveToNextSlideTransitionEffect::ProcessDualRenderSurfaces");
                return;
            }

            int border = 0;
            if (mBorderFraction != 0)
            {
                // ### SRG TODO G390
               // border = (int)((((float)thisSlideSurface.Width) / mBorderFraction) + 0.5f);
            }

            switch (mDirection)
            {
                case SwipeDirection.RIGHT:
                {
                    doRight(thisSlideSurface, nextSlideSurface, delta, border);
                    break;
                }
                case SwipeDirection.LEFT:
                {
                    doLeft(thisSlideSurface, nextSlideSurface, delta, border);
                    break;
                }
                case SwipeDirection.UP:
                {
                    doUp(thisSlideSurface, nextSlideSurface, delta, border);
                    break;
                }
                case SwipeDirection.DOWN:
                {
                    doDown(thisSlideSurface, nextSlideSurface, delta, border);
                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        //*******************************************************************
        private void doLeft(RenderSurface thisSlideSurface, RenderSurface nextSlideSurface, float delta, int border)
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

            int dist = i1width + border;

            float dd = mMovementModel.Get(0, 1, delta);

            float x_offset = dist * dd;

            float width = i1width - x_offset;
            if (width > 0 && thisSlideSurface!=null)
            {
                engine.DrawImage(thisSlideSurface,
                               new RectangleF(0, 0, width / thisSlideSurface.Width, 1),
                               new RectangleF(x_offset, 0, width, thisSlideSurface.Height), false);
            }

            float other_x_offset = i1width - x_offset + border;
            float other_width = i1width - other_x_offset;

            if (other_x_offset < i1width && other_width > 0 && nextSlideSurface != null)
            {
                engine.DrawImage(nextSlideSurface,
                            new RectangleF(other_x_offset / nextSlideSurface.Width, 0, 1, 1),
                            new RectangleF(0, 0, other_width, nextSlideSurface.Height), false);

            }

            if (border != 0)
            {
                // ### SRG TODO G390
                //Graphics g = Graphics.FromImage(merge);
                //g.FillRectangle(mBlackBrush, width, 0, border, i1.Height);
                //g.Dispose();
            }
        }


        //*******************************************************************
        private void doRight(RenderSurface thisSlideSurface, RenderSurface nextSlideSurface, float delta, int border)
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

            int dist = i1width + border;

            float dd = mMovementModel.Get(0, 1, delta);

            float x_offset = dist * dd;

            float width = i1width - x_offset;
            if (width > 0 && thisSlideSurface!=null)
            {
               // BitmapUtils.unsafe_copy_offsets(i1Data, iMergeData, 0, 0, x_offset, 0, width, i1.Height);

                engine.DrawImage(thisSlideSurface,
                                new RectangleF(x_offset / thisSlideSurface.Width, 0, 1, 1),
                                new RectangleF(0, 0, width, thisSlideSurface.Height), false);
            }

            float other_x_offset = i1width - x_offset + border;
            float other_width = i1width - other_x_offset;

            if (other_x_offset < i1width && other_width > 0 && nextSlideSurface!=null)
            {
               // BitmapUtils.unsafe_copy_offsets(i2Data, iMergeData, other_x_offset, 0, 0, 0, other_width, i1.Height);
                engine.DrawImage(nextSlideSurface,
                                 new RectangleF(0, 0, other_width / nextSlideSurface.Width, 1),
                                 new RectangleF(other_x_offset, 0, other_width, nextSlideSurface.Height), false);
            }

            if (border != 0)
            {
                // ### SRG G390 TODO
               // Graphics g = Graphics.FromImage(merge);
               // g.FillRectangle(mBlackBrush, other_width, 0, border, i1.Height);
               // g.Dispose();
            }
        }


        //*******************************************************************
        private void doUp(RenderSurface thisSlideSurface, RenderSurface nextSlideSurface, float delta, int border)
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

            int dist = i1height + border;

            float dd = mMovementModel.Get(0, 1, delta);

            float y_offset = dist * dd;

            float height = i1height - y_offset;
            if (height > 0 && thisSlideSurface!=null)
            {
                engine.DrawImage(thisSlideSurface,
                                 new RectangleF(0, y_offset / thisSlideSurface.Height, 1, 1),
                                 new RectangleF(0, 0, thisSlideSurface.Width, height), false);
            }

            float other_y_offset = i1height - y_offset + border;
            float other_height = i1height - other_y_offset;

            if (other_y_offset < i1height && other_height > 0 && nextSlideSurface != null)
            {
                engine.DrawImage(nextSlideSurface,
                    new RectangleF(0, 0, 1, other_height / nextSlideSurface.Height),
                    new RectangleF(0, other_y_offset, nextSlideSurface.Width, other_height), false);
            }

            if (border != 0)
            {     
               // ### SRG TODO G390
               // Graphics g = Graphics.FromImage(merge);
               // g.FillRectangle(mBlackBrush, 0, height, i1.Width, border);
               //g.Dispose();
            }
        }

        //*******************************************************************
        private void doDown(RenderSurface thisSlideSurface, RenderSurface nextSlideSurface, float delta, int border)
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

            int dist = i1height + border;

            float dd = mMovementModel.Get(0, 1, delta);

            float y_offset = dist * dd;

            float height = i1height - y_offset;
            if (height > 0 && thisSlideSurface!=null)
            {
                engine.DrawImage(thisSlideSurface,
                               new RectangleF(0, 0, 1, height / thisSlideSurface.Height),
                               new RectangleF(0, y_offset, thisSlideSurface.Width, height), false);

            }

            float other_y_offset = i1height - y_offset + border;
            float other_height = i1height - other_y_offset;

            if (other_y_offset < i1height && other_height > 0 && nextSlideSurface != null)
            {
                engine.DrawImage(nextSlideSurface,
                             new RectangleF(0, other_y_offset / nextSlideSurface.Height, 1, 1),
                             new RectangleF(0, 0, nextSlideSurface.Width, other_height), false);
            }

            if (border != 0)
            {
               // ### SRG TODO G390
             //   Graphics g = Graphics.FromImage(merge);
             //   g.FillRectangle(mBlackBrush, 0, other_height, i1.Width, border);
             //   g.Dispose();
            }
        }


        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
        {
            effect.SetAttribute("Type", "MoveToNextSlideTransitionEffect");
            effect.SetAttribute("Direction", ((int)this.mDirection).ToString());
            effect.SetAttribute("BorderFrac", mBorderFraction.ToString());

            if (mForecedMotionBlur != 30)
            {
                effect.SetAttribute("mb", mForecedMotionBlur.ToString());
            }

            if (mMovementModel is CNonLinear == false)
            {
                mMovementModel.Save(effect, doc);
            }

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
            s1 = element.GetAttribute("BorderFrac");
            if (s1 != "")
            {
                mBorderFraction = float.Parse(s1);
            }

            s1 = element.GetAttribute("mb");
            if (s1 != "")
            {
                mForecedMotionBlur = int.Parse(s1);
            }

            XmlNodeList list = element.GetElementsByTagName("Equation");
            if (list.Count > 0)
            {
                XmlElement e = list[0] as XmlElement;

                mMovementModel = CEquation.CreateFromType(e.GetAttribute("Type"));
                if (mMovementModel != null)
                {
                    mMovementModel.Load(e);
                }
            }


            LoadTransitionPart(element);
        }
    }
}
