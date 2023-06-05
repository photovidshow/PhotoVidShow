using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for BatmanTransitionEffect.
    /// </summary>
    public class BatmanTransitionEffect : CTransitionEffect
    {
        private CImage mMiddleImage = null;
        private CAnimatedDecorationEffect mMovementOut;
        private CAnimatedDecorationEffect mMovementIn;
        private float mStartRotation;
        private float mEndRotation;
        private string mFilename;
        private float mBrightness;
        private float mContrast;


        //*******************************************************************
        public BatmanTransitionEffect()
        {
            mForecedMotionBlur = 30;
        }

        //*******************************************************************
        public BatmanTransitionEffect(float length, string middleImageFilename, float brightness, float contrast, float startRotation, float endRotation)
            : base(length)
        {
            mStartRotation = startRotation;
            mEndRotation = endRotation;
            mFilename = middleImageFilename;
            mBrightness = brightness;
            mContrast = contrast;

            mForecedMotionBlur = 30;
        }


        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            BatmanTransitionEffect t = new BatmanTransitionEffect(mLength, mFilename, mBrightness, mContrast, mStartRotation, mEndRotation);
            t.Index = this.Index;
            return t;
        }

        //*******************************************************************
        private void BuildImage()
        {
            if (mFilename == "") return;

            float [] brightness = new float[3];
            brightness[0] = mBrightness;
            brightness[1] = mBrightness;
            brightness[2] = mBrightness;

            float [] contrast = new float[3];
            contrast[0] = mContrast;
            contrast[1] = mContrast;
            contrast[2] = mContrast;

            mMiddleImage = new CImage(mFilename, false);
            mMiddleImage.Brightness = brightness;
            mMiddleImage.Contrast = contrast;

            float midrotation = ((mEndRotation - mStartRotation) / 2) + mStartRotation;

            mMovementOut = new CAnimatedDecorationEffect();
            mMovementOut.Zoom = new CZoom(1, 12);
            mMovementOut.Rotation = new CRotation(mStartRotation, midrotation);
            mMovementOut.LengthInSeconds = mLength / 2;

            mMovementIn = new CAnimatedDecorationEffect();
            mMovementIn.Zoom = new CZoom(1, 12);
            mMovementIn.Rotation = new CRotation(mEndRotation, midrotation);
            mMovementIn.LengthInSeconds = mLength / 2;
        }

        //*******************************************************************
        protected override void Process2(float delta, RenderSurface nextSlideSurface, int frame, Rectangle? innerRegion)
        {
            if (mMiddleImage == null)
            {
                BuildImage();
            }

            if (mMiddleImage == null)
            {
                return;
            }

            CAnimatedDecorationEffect movement = mMovementOut;
            RectangleF dest_rec = new RectangleF(0, 0, nextSlideSurface.Width, nextSlideSurface.Height);

            float alpha = 1;

            if (delta >= 0.5f)
            {
                delta -= 0.5f;
                delta *= 2;
                delta = 1.0f - delta;

                RectangleF src_rec = new RectangleF(0, 0, 1, 1);
                GraphicsEngine.Current.DrawImage(nextSlideSurface, src_rec, dest_rec, false);
                movement = mMovementIn;
            }
            else
            {
                delta *= 2;
              
            }

            alpha = delta * 3;
            alpha = CGlobals.ClampF(alpha, 0, 1);

            float outAspectRatioFract = ((float)nextSlideSurface.Height) / ((float)nextSlideSurface.Width);
           
            float aspect = mMiddleImage.Aspect;
            float width =0.2f;
            float halfWidth = width /2.0f;
            float height = width / aspect;
            height /= outAspectRatioFract;
            float halfHeight = height /2.0f;

            RectangleF coverage = new RectangleF(0.5f - halfWidth, 0.5f - halfHeight, width, height);

            PointF[] points = null;

            RectangleF r = new RectangleF(0, 0, 1, 1);
            if (movement != null)
            {
                points = movement.GetRectanglePoints(coverage, dest_rec, delta);
            }
          
            GraphicsEngine.Current.DrawImageWithDiffuseAlpha(mMiddleImage, alpha, new RectangleF(0, 0, 1, 1), points[3].X, points[3].Y, points[0].X, points[0].Y, points[2].X, points[2].Y, points[1].X, points[1].Y, true);
        }

        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
        {
            effect.SetAttribute("Type", "BatmanTransitionEffect");

            SaveTransitionPart(effect, doc);

            effect.SetAttribute("Filename", CGlobals.StripRootHeader(mFilename));
            effect.SetAttribute("StartRot", mStartRotation.ToString());
            effect.SetAttribute("EndRot", mEndRotation.ToString());
            effect.SetAttribute("Brightness", mBrightness.ToString());
            effect.SetAttribute("Contrast", mContrast.ToString());

            parent.AppendChild(effect);
        }


        //*******************************************************************
        public override void Load(XmlElement element)
        {       
            LoadTransitionPart(element);

            string s = element.GetAttribute("Filename");
            if (s != "")
            {
                mFilename = s;
            }

            s = element.GetAttribute("StartRot");
            if (s != "")
            {
                mStartRotation = float.Parse(s);
            }

            s = element.GetAttribute("EndRot");
            if (s != "")
            {
                mEndRotation = float.Parse(s);
            }

            s = element.GetAttribute("Brightness");
            if (s != "")
            {
                mBrightness = float.Parse(s);
            }

            s = element.GetAttribute("Contrast");
            if (s != "")
            {
                mContrast = float.Parse(s);
            }
        }
    }
}
