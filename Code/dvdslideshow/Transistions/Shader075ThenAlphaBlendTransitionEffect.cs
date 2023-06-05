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
    /// Summary description for Shader075ThenAlphaBlendTransitionEffect.
    /// </summary>

    //*******************************************************************
    public class Shader075ThenAlphaBlendTransitionEffect : CTransitionEffect
    {
        private string mShaderName;

        //*******************************************************************
        public Shader075ThenAlphaBlendTransitionEffect()
        {
            this.mNeedsDualRenderSurface = true;
        }

        //*******************************************************************
        public Shader075ThenAlphaBlendTransitionEffect(float length, string shaderName)
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

                PixelShaderEffect shader = PixelShaderEffectDatabase.GetInstance().GetPixelShader(mShaderName);

                if (shader == null)
                {
                    Log.Error("Can not find '" + mShaderName + "' pixel shader on call to ProcessDualRenderSurfaces");
                    return;
                }

                float d2 = delta * 2;
                if (d2 > 1.0f) d2 = 1.0f;

                shader.SetParameter(0, d2);
                shader.SetParameter(1, 1 - d2);

                ge.SetPixelShaderEffect(shader);

                try
                {
                    ge.DrawImage(thisSlideSurface, src_rec, dest_rec, false);
                }
                finally
                {
                    ge.SetPixelShaderEffect(null);
                }

                return;
            }
            else
            {
                float d3 = 0;
                if (delta < 0.75)
                {
                    return;
                }
                else
                {
                    d3 = (delta - 0.75f) * 4;
                }

                RectangleF dest_rec = new RectangleF(0, 0, nextSlideSurface.Width, nextSlideSurface.Height);

                GraphicsEngine.Current.DrawImageWithDiffuseAlpha(nextSlideSurface, d3, src_rec, dest_rec, false);

            }
            
        }

        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            CTransitionEffect t = new Shader075ThenAlphaBlendTransitionEffect(mLength,mShaderName);
            t.Index = this.Index;
            return t;
        }


        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
        {
            effect.SetAttribute("Type", "Shader075ThenAlphaBlendTransitionEffect");
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
