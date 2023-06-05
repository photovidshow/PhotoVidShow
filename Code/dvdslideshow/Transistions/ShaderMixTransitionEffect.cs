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
    /// Summary description for ShaderMixTransitionEffect.
    /// </summary>

    //*******************************************************************
    public class ShaderMixTransitionEffect : CTransitionEffect
    {
        private RenderSurface mRenderSurface = null;
        private string mShaderName;

        //*******************************************************************
        public ShaderMixTransitionEffect()
        {
            this.mNeedsDualRenderSurface = true;
        }

        //*******************************************************************
        public ShaderMixTransitionEffect(float length, string shaderName)
            : base(length)
        {
            mShaderName = shaderName;
            this.mNeedsDualRenderSurface = true;
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

                PixelShaderEffect pixelshadereffect = PixelShaderEffectDatabase.GetInstance().GetPixelShader(mShaderName);

                if (pixelshadereffect == null)
                {
                    Log.Error("Can not load '"+mShaderName+"' pixel shader in ShaderMixTransitionEffect");
                    return;
                }

                float d = (float) Math.Sin(delta*Math.PI);

                ge.SetRenderTarget(originalSurface);

                pixelshadereffect.SetParameter(0, d);
                pixelshadereffect.SetParameter(1, 1-d);
                ge.SetPixelShaderEffect(pixelshadereffect);
             
                ge.DrawImage(mRenderSurface, src_rec, dest_rec, false);
                ge.SetPixelShaderEffect(null);
                mRenderSurface.Dispose();
                mRenderSurface = null;
            }
        }

        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            CTransitionEffect t = new ShaderMixTransitionEffect(mLength, mShaderName);
            t.Index = this.Index;
            return t;
        }


        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
        {
            effect.SetAttribute("Type", "ShaderMixTransitionEffect");
            SaveTransitionPart(effect, doc);
            effect.SetAttribute("Shader", this.mShaderName);
            parent.AppendChild(effect);
        }


        //*******************************************************************
        public override void Load(XmlElement element)
        {
            LoadTransitionPart(element);
            mShaderName = element.GetAttribute("Shader");
        }

    }

}
