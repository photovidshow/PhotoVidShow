using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for NextSlideBlurInTransitionEffect.
    /// </summary>
    /// 

     /// ### SRG THIS NEARLY WORKS BUT NOT QUITE.......

    //*******************************************************************
    public class NextSlideBlurInTransitionEffect : CTransitionEffect
    {

        private BlurTransitionEffect mInnerBlurEffect = new BlurTransitionEffect(1.5f);

        public NextSlideBlurInTransitionEffect()
        {
        }


        public NextSlideBlurInTransitionEffect(float length)
            : base(length)
        {
        }

        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            CTransitionEffect t = new NextSlideBlurInTransitionEffect(mLength);
            t.Index = this.Index;
            return t;
        }

        //*******************************************************************
        protected override void Process2(float delta, RenderSurface nextSlideSurface, int frame, Rectangle? innerRegion)
        {
            RectangleF dest_rec = new RectangleF(0, 0, nextSlideSurface.Width, nextSlideSurface.Height);
            RectangleF src_rec = new RectangleF(0, 0, 1, 1);

            GraphicsEngine ge = GraphicsEngine.Current;
            RenderSurface originalSurface = ge.GetRenderTarget();

            mInnerBlurEffect.Length = mLength;

            RenderSurface tempSurface = ge.GenerateRenderSurface((uint)dest_rec.Width, (uint)dest_rec.Height, this.ToString() + "::Process2");

            ge.SetRenderTarget(tempSurface);
            ge.ClearRenderTarget(255, 255, 255, 0);
            mInnerBlurEffect.SetRenderSurface(tempSurface);

            ge.SetRenderTarget(nextSlideSurface);

            mInnerBlurEffect.SimpleProcessDualRenderSurfaces(delta, null, nextSlideSurface, frame);

            ge.SetRenderTarget(originalSurface);

            GraphicsEngine.Current.DrawImageWithDiffuseAlpha(nextSlideSurface, delta, src_rec, dest_rec, mUseImage2Alpha);
        }


        //*******************************************************************
        protected override CImage Process(float delta, CImage ii1, CImage ii2, int frame, CImage render_to_this)
        {
            return null;
        }

        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
        {
            effect.SetAttribute("Type", "NextSlideBlurInTransitionEffect");
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
