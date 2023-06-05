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
    /// Summary description for ShaderPlusAlphaBlendTransitionEffect.
    /// </summary>

    //*******************************************************************
    public class ShaderPlusAlphaBlendTransitionEffect : CTransitionEffect
    {
        private string mShaderName;

        //*******************************************************************
        public ShaderPlusAlphaBlendTransitionEffect()
        {
            this.mNeedsDualRenderSurface = true;
        }

        //*******************************************************************
        public ShaderPlusAlphaBlendTransitionEffect(float length, string shaderName)
            : base(length)
        {
            this.mNeedsDualRenderSurface = true;
            mShaderName = shaderName;
        }


        //*******************************************************************
        protected override void ProcessDualRenderSurfaces(float delta, RenderSurface thisSlideSurface, RenderSurface nextSlideSurface, int frame)
        {
            GraphicsEngine ge = GraphicsEngine.Current;
            RectangleF src_rec = new RectangleF(0, 0, 1, 1);
       
            if (thisSlideSurface != null)
            {
                RectangleF dest_rec = new RectangleF(0, 0, thisSlideSurface.Width, thisSlideSurface.Height);

                PixelShaderEffect shaderEffect = PixelShaderEffectDatabase.GetInstance().GetPixelShader(mShaderName);

                if (shaderEffect == null)
                {
                    Log.Error("Can not load '"+mShaderName+"' pixel shader in ShaderPlusAlphaBlendTransition");
                    return;
                }

                shaderEffect.SetParameter(0, delta);
                ge.SetPixelShaderEffect(shaderEffect);
                ge.DrawImage(thisSlideSurface, src_rec, dest_rec, false);
                ge.SetPixelShaderEffect(null);
            }
            else
            {
                RectangleF dest_rec = new RectangleF(0, 0, nextSlideSurface.Width, nextSlideSurface.Height);

                GraphicsEngine.Current.DrawImageWithDiffuseAlpha(nextSlideSurface, delta, src_rec, dest_rec, false);               
            }
        }

        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            CTransitionEffect t = new ShaderPlusAlphaBlendTransitionEffect(mLength,mShaderName);
            t.Index = this.Index;
            return t;
        }


        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
        {
            effect.SetAttribute("Type", "ShaderPlusAlphaBlendTransitionEffect");
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
