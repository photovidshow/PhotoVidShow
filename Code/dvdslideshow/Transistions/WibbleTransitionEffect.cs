using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml;
using DVDSlideshow.GraphicsEng;
using ManagedCore;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for WibbleTransitionEffect.
    /// </summary>

    //*******************************************************************
    public class WibbleTransitionEffect : CTransitionEffect
    {
        private RenderSurface mRenderSurface = null;

        private float mXfreq;
        private float mXamp;
        private float mYfreq;
        private float mYamp;

        //*******************************************************************
        public WibbleTransitionEffect()
        {
            this.mNeedsDualRenderSurface = true;
        }

        //*******************************************************************
        public WibbleTransitionEffect(float length, float xfreq, float xamp, float yfreq, float yamp)
            : base(length)
        {
            this.mNeedsDualRenderSurface = true;

            mXfreq = xfreq;
            mXamp = xamp;
            mYfreq = yfreq;
            mYamp = yamp;
        }


        //*******************************************************************
        protected override void ProcessDualRenderSurfaces(float delta, RenderSurface thisSlideSurface, RenderSurface nextSlideSurface, int frame)
        {
            GraphicsEngine ge = GraphicsEngine.Current;
            RectangleF src_rec = new RectangleF(0, 0, 1, 1);

            RenderSurface originalSurface = ge.GetRenderTarget();

            if (thisSlideSurface != null)
            {
                RectangleF dest_rec = new RectangleF(0, 0, thisSlideSurface.Width, thisSlideSurface.Height);

                mRenderSurface = ge.GenerateRenderSurface((uint)dest_rec.Width, (uint)dest_rec.Height, this.ToString() + "::ProcessDualRenderSurfaces");
                try
                {
                    ge.SetRenderTarget(mRenderSurface);
                    ge.DrawImage(thisSlideSurface, src_rec, dest_rec, false);
                }
                finally
                {
                    ge.SetRenderTarget(originalSurface);
                }
                return;
            }
            else
            {
                if (mRenderSurface == null)
                {
                    Log.Error("Null render surface in " + this.ToString() + "::ProcessDualRenderSurfaces");
                    return;
                }

                RectangleF dest_rec = new RectangleF(0, 0, nextSlideSurface.Width, nextSlideSurface.Height);

                ge.SetRenderTarget(mRenderSurface);
                ge.DrawImageWithDiffuseAlpha(nextSlideSurface, delta, src_rec, dest_rec, false);

                PixelShaderEffect wibblePixelShader = PixelShaderEffectDatabase.GetInstance().GetPixelShader("wibble");
                if (wibblePixelShader == null)
                {
                    Log.Error("Can not find wibble shader");
                    ge.SetRenderTarget(originalSurface);
                }
                else
                {
                    float d = (float)Math.Sin(delta * Math.PI);

                    wibblePixelShader.SetParameter(0, mXfreq);  // xfreq
                    wibblePixelShader.SetParameter(1, mXamp);  // xamp
                    wibblePixelShader.SetParameter(2, mYfreq);  // yfreq
                    wibblePixelShader.SetParameter(3, mYamp);  // yamp
                    wibblePixelShader.SetParameter(4, d);  // delta

                    ge.SetPixelShaderEffect(wibblePixelShader);

                    ge.SetRenderTarget(originalSurface);
                    ge.DrawImage(mRenderSurface, src_rec, dest_rec, false);

                    ge.SetPixelShaderEffect(null);
                }

                mRenderSurface.Dispose();
                mRenderSurface = null;
            }
        }

        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            CTransitionEffect t = new WibbleTransitionEffect(mLength, mXfreq, mXamp, mYfreq, mYamp );
            t.Index = this.Index;
            return t;
        }


        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
        {
            effect.SetAttribute("Type", "WibbleTransitionEffect");
            SaveTransitionPart(effect, doc);
            effect.SetAttribute("xfreq", this.mXfreq.ToString());
            effect.SetAttribute("xamp", this.mXamp.ToString());
            effect.SetAttribute("yfreq", this.mYfreq.ToString());
            effect.SetAttribute("yamp", this.mYamp.ToString());

            parent.AppendChild(effect);
        }


        //*******************************************************************
        public override void Load(XmlElement element)
        {
            LoadTransitionPart(element);

            string s = element.GetAttribute("xfreq");
            if (s != "")
            {
                mXfreq = float.Parse(s);
            }
            s = element.GetAttribute("xamp");
            if (s != "")
            {
                mXamp = float.Parse(s);
            }
            s = element.GetAttribute("yfreq");
            if (s != "")
            {
                mYfreq = float.Parse(s);
            }
            s = element.GetAttribute("yamp");
            if (s != "")
            {
                mYamp = float.Parse(s);
            }
        }
    }

}
