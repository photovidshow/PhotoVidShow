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
    /// Summary description for MiddleSaturateToColorTransitionEffect.
	/// </summary>
	public class MiddleSaturateToColorTransitionEffect : CTransitionEffect 
	{
		protected Color mColor;

		//*******************************************************************
		public MiddleSaturateToColorTransitionEffect()
		{
            this.mNeedsDualRenderSurface = true;
		}

		//*******************************************************************
		public MiddleSaturateToColorTransitionEffect(Color c, float length) : base(length)
		{
			mColor = c;
            this.mNeedsDualRenderSurface = true;
		}


		//*******************************************************************
		public override CTransitionEffect Clone()
		{
			CTransitionEffect t= new MiddleSaturateToColorTransitionEffect(mColor, mLength) ;
			t.Index = this.Index;
			return t;
		}

        //*******************************************************************
        protected override void ProcessDualRenderSurfaces(float delta, RenderSurface thisSlideSurface, RenderSurface nextSlideSurface, int frame)
        {
            int width = 0;
            int height = 0;

            if (thisSlideSurface != null)
            {
                width = (int)thisSlideSurface.Width;
                height = (int)thisSlideSurface.Height;
            }
            else if (nextSlideSurface != null)
            {
                width = (int)nextSlideSurface.Width;
                height = (int)nextSlideSurface.Height;
            }
            else
            {
                ManagedCore.Log.Warning("No surface defined in ProcessDualRenderSurfaces");
                return;
            }

            try
            {
                ProcessRectangle(delta, 0, 0, width, height, thisSlideSurface, nextSlideSurface);
            }
            finally
            {
                GraphicsEngine.Current.SetPixelShaderEffect(null);
            }
		}

        //*******************************************************************
        public void ProcessRectangle(float delta, int x, int y, int width, int height, RenderSurface thisSlideSurface, RenderSurface nextSlideSurface)
        {
            GraphicsEngine engine = GraphicsEngine.Current;

            int aa = (int)(delta * 255.0f);
            if (aa < 0) aa = 0;
            if (aa > 255) aa = 255;

            if (aa >= 128)
            {
                if (nextSlideSurface != null)
                {
                    Color cc = Color.White;
                    float rr = (float)cc.R;
                    rr /= 255.0f;
                    rr *= 255.0f - ((aa - 128) * 2);
                    if (rr < 0) rr = 0;
                    if (rr > 255) rr = 255;

                    float gg = (float)cc.G;
                    gg /= 255.0f;
                    gg *= 255.0f - ((aa - 128) * 2);
                    if (gg < 0) gg = 0;
                    if (gg > 255) gg = 255;

                    float bb = (float)cc.B;
                    bb /= 255.0f;
                    bb *= 255.0f - ((aa - 128) * 2);
                    if (bb < 0) bb = 0;
                    if (bb > 255) bb = 255;

                    PixelShaderEffect shader = null;

                    if (this.mColor == Color.Black)
                    {
                        shader = PixelShaderEffectDatabase.GetInstance().GetPixelShader("subColour");
                    }
                    else
                    {
                        shader = PixelShaderEffectDatabase.GetInstance().GetPixelShader("addColour");
                    }
                    if (shader == null)
                    {
                        Log.Error("Can not find shader needed for middle saturate colour transition effect");
                        return;
                    }

                    shader.SetParameter(0, rr / 255.0f);
                    shader.SetParameter(1, gg / 255.0f);
                    shader.SetParameter(2, bb / 255.0f);
                    shader.SetParameter(3, 1);

                    engine.SetPixelShaderEffect(shader);

                    RectangleF src_area = new RectangleF(x, y, x + width, y + height);
                    RectangleF dst_area = new RectangleF(x, y, width, height);

                    src_area.X /= ((float)nextSlideSurface.Width);  // may be different to width/height
                    src_area.Y /= ((float)nextSlideSurface.Height);
                    src_area.Width /= ((float)nextSlideSurface.Width);
                    src_area.Height /= ((float)nextSlideSurface.Height);

                    engine.DrawImage(nextSlideSurface, src_area, dst_area, false);
                }
            }
            else
            {
                if (thisSlideSurface != null)
                {
                    Color cc = Color.White;
                    float rr = (float)cc.R;
                    rr /= 255.0f;
                    rr *= (aa * 2);
                    if (rr < 0) rr = 0;
                    if (rr > 255) rr = 255;

                    float gg = (float)cc.G;
                    gg /= 255.0f;
                    gg *= (aa * 2);
                    if (gg < 0) gg = 0;
                    if (gg > 255) gg = 255;

                    float bb = (float)cc.B;
                    bb /= 255.0f;
                    bb *= (aa * 2);
                    if (bb < 0) bb = 0;
                    if (bb > 255) bb = 255;

                    PixelShaderEffect shader = null;

                    if (this.mColor == Color.Black)
                    {
                        shader = PixelShaderEffectDatabase.GetInstance().GetPixelShader("subColour");
                    }
                    else
                    {
                        shader = PixelShaderEffectDatabase.GetInstance().GetPixelShader("addColour");
                    }

                    if (shader == null)
                    {
                        Log.Error("Can not find shader needed for middle saturate colour transition effect");
                        return;
                    }

                    shader.SetParameter(0, rr / 255.0f);
                    shader.SetParameter(1, gg / 255.0f);
                    shader.SetParameter(2, bb / 255.0f);
                    shader.SetParameter(3, 1);

                    engine.SetPixelShaderEffect(shader);

                    RectangleF src_area = new RectangleF(x, y, x + width, y + height);
                    RectangleF dst_area = new RectangleF(x, y, width, height);

                    src_area.X /= ((float)thisSlideSurface.Width); // may be different to width/height
                    src_area.Y /= ((float)thisSlideSurface.Height);
                    src_area.Width /= ((float)thisSlideSurface.Width);
                    src_area.Height /= ((float)thisSlideSurface.Height);

                    engine.DrawImage(thisSlideSurface, src_area, dst_area, false);
                }
            }
        }

		//*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
		{
            effect.SetAttribute("Type", "MiddleSaturateToColorTransitionEffect");
            SaveMiddleSaturatePart(effect, doc);	
			parent.AppendChild(effect); 
		}

        //*******************************************************************
        protected void SaveMiddleSaturatePart(XmlElement effect, XmlDocument doc)
        {
            effect.SetAttribute("Color", this.mColor.ToString());

            SaveTransitionPart(effect, doc);
        }

		
		//*******************************************************************
		public override void Load(XmlElement element)
		{
			LoadTransitionPart(element);

			string s = element.GetAttribute("Color");

			if (s!="")
			{
				mColor = CGlobals.ParseColor(s);
			}
		}
	}
}
