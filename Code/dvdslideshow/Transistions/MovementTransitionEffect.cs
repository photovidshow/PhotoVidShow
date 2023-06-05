using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for MovementTransitionEffect.
    /// </summary>
    public class MovementTransitionEffect : CTransitionEffect
    {
        private CAnimatedDecorationEffect mCurrentSlideMovement;
        private CAnimatedDecorationEffect mNextSlideMovement;
        private bool mDoAlphaBlend = false;

        public bool DoAlphaBlend
        {
            get { return mDoAlphaBlend; }
            set { mDoAlphaBlend = value; }
        }

        //*******************************************************************
        public MovementTransitionEffect()
        {
            this.mNeedsDualRenderSurface = true;
            mForecedMotionBlur = 30;
        }

        //*******************************************************************
        public MovementTransitionEffect(float length, CAnimatedDecorationEffect currentSlideMovement, CAnimatedDecorationEffect nextSlideMovement, bool renderNextSlideFirst)
            : base(length)
        {
            mCurrentSlideMovement = currentSlideMovement;
            mNextSlideMovement = nextSlideMovement;
            mForecedMotionBlur = 30;

            this.mNeedsDualRenderSurface = true;
            this.mRenderNextSlideBeforeCurrentSlide = renderNextSlideFirst;
        }


        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            MovementTransitionEffect t = new MovementTransitionEffect(mLength, mCurrentSlideMovement, mNextSlideMovement, mRenderNextSlideBeforeCurrentSlide);
            t.Index = this.Index;

            t.DoAlphaBlend = mDoAlphaBlend;

            return t;
        }

        //*******************************************************************
        private void DrawSurface(RenderSurface surface, CAnimatedDecorationEffect movement, float delta, bool alpha)
        {
            GraphicsEngine engine = GraphicsEngine.Current;

            PointF[] points = null;

            RectangleF r = new RectangleF(0, 0, 1, 1);
            if (movement != null)
            {
                points = movement.GetRectanglePoints(r, new RectangleF(0, 0, surface.Width, surface.Height), delta);
            }
            else
            {
                r = new RectangleF(r.X * surface.Width, r.Y * surface.Height, r.Width * surface.Width, r.Height * surface.Height);
            }

            if (points == null)
            {
                if (alpha == true)
                {
                    engine.DrawImageWithDiffuseAlpha(surface, delta, new RectangleF(0, 0, 1, 1), r, false);
                }
                else
                {
                    engine.DrawImage(surface, new RectangleF(0, 0, 1, 1), r, false);
                }
            }
            else
            {
                if (alpha==true)
                {
                    engine.DrawImageWithDiffuseAlpha(surface, delta, new RectangleF(0, 0, 1, 1), points[3].X, points[3].Y, points[0].X, points[0].Y, points[2].X, points[2].Y, points[1].X, points[1].Y, false);
                }
                else
                {
                    engine.DrawImage(surface, new RectangleF(0, 0, 1, 1), points[3].X, points[3].Y, points[0].X, points[0].Y, points[2].X, points[2].Y, points[1].X, points[1].Y, false);
                }
            }
        }


        //*******************************************************************
        protected override void ProcessDualRenderSurfaces(float delta, RenderSurface thisSlideSurface, RenderSurface nextSlideSurface, int frame)
        {   
            if (nextSlideSurface != null)
            {
                DrawSurface(nextSlideSurface, mNextSlideMovement, delta, mDoAlphaBlend);
            }

            if (thisSlideSurface != null)
            {
                DrawSurface(thisSlideSurface, mCurrentSlideMovement, delta, false);
            }
        }

        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
        {
            effect.SetAttribute("Type", "MovementTransitionEffect");
       
            SaveTransitionPart(effect, doc);

            if (mCurrentSlideMovement != null)
            {
                XmlElement currentSlideMovement = doc.CreateElement("CurrentSlideMovement");

                mCurrentSlideMovement.Save(currentSlideMovement, doc);

                effect.AppendChild(currentSlideMovement);
            }

            if (mNextSlideMovement != null)
            {
                XmlElement nextSlideMovement = doc.CreateElement("NextSlideMovement");

                mNextSlideMovement.Save(nextSlideMovement, doc);

                effect.AppendChild(nextSlideMovement);
            }

            if (mDoAlphaBlend == true)
            {
                effect.SetAttribute("AlphaBlend", mDoAlphaBlend.ToString());
            }

            parent.AppendChild(effect);

        }


        //*******************************************************************
        public override void Load(XmlElement element)
        {
            LoadTransitionPart(element);

            XmlNodeList list = element.GetElementsByTagName("CurrentSlideMovement");

            if (list.Count != 0)
            {
                XmlElement subelement = list[0] as XmlElement;
                if (subelement != null)
                {
                    mCurrentSlideMovement = new CAnimatedDecorationEffect();
                    mCurrentSlideMovement.Load(subelement);
                }
            }

            list = element.GetElementsByTagName("NextSlideMovement");

            if (list.Count != 0)
            {
                XmlElement subelement = list[0] as XmlElement;
                if (subelement != null)
                {
                    mNextSlideMovement = new CAnimatedDecorationEffect();
                    mNextSlideMovement.Load(subelement);
                }
            }

            string s = element.GetAttribute("AlphaBlend");
            if (s != "")
            {
                mDoAlphaBlend = bool.Parse(s);
            }
        }
    }
}
