using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for CameraSnapshotTransitionEffect.
    /// </summary>
    public class CameraSnapshotTransitionEffect : CTransitionEffect
    {

        MiddleSplitDiagonalTransitionEffect mMidSplitEffectOut = null;
        MiddleSplitDiagonalTransitionEffect mMidSplitEffectIn = null;


        //*******************************************************************
        public CameraSnapshotTransitionEffect()
        {
            mMidSplitEffectOut = new MiddleSplitDiagonalTransitionEffect(false, true, mLength, 16);
            mMidSplitEffectIn = new MiddleSplitDiagonalTransitionEffect(true, true, mLength, 16);
            mForecedMotionBlur = 30;
        }

        //*******************************************************************
        public CameraSnapshotTransitionEffect(float length)
            : base(length)
        {
            mMidSplitEffectOut = new MiddleSplitDiagonalTransitionEffect(false, true, mLength, 16);
            mMidSplitEffectIn = new MiddleSplitDiagonalTransitionEffect(true, true, mLength, 16);
            mForecedMotionBlur = 30;
        }

        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            CTransitionEffect t = new CameraSnapshotTransitionEffect(mLength);
            t.Index = this.Index;
            return t;
        }

        //*******************************************************************
        protected override CImage Process(float delta, CImage ii1, CImage ii2, int frame, CImage render_to_this)
        {
            return null;
        }

        //*******************************************************************
        protected override void Process2(float delta, RenderSurface nextSlideSurface, int frame, Rectangle? innerRegion)
        {
            GraphicsEngine engine = GraphicsEngine.Current;

            // snap out
            if (delta < 0.5)
            {
                RenderSurface currentSurface = engine.GetRenderTarget();
                engine.SetRenderTarget(nextSlideSurface);
                engine.ClearRenderTarget(0, 0, 0, 255);
                engine.SetRenderTarget(currentSurface);

                mMidSplitEffectOut.Process(nextSlideSurface, (int)nextSlideSurface.Width, (int)nextSlideSurface.Height, delta * 2, frame, null);
            }
            else  // snap in
            {
                engine.ClearRenderTarget(0, 0, 0, 255);
                mMidSplitEffectIn.Process(nextSlideSurface, (int) nextSlideSurface.Width, (int) nextSlideSurface.Height, (delta - 0.5f) * 2, frame, null);
            }
        }

        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
        {
            effect.SetAttribute("Type", "CameraSnapshotTransitionEffect");

            SaveTransitionPart(effect, doc);

            parent.AppendChild(effect);
        }


        //*******************************************************************
        public override void Load(XmlElement element)
        {
            LoadTransitionPart(element);
        }
    }
}
