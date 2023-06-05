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
	/// Summary description for FadeToBW.
	/// </summary>
	public class HBlurWhiteout : CTransitionEffect
	{
        private RenderSurface mRenderSurface = null;

        //*******************************************************************
		public HBlurWhiteout()
		{
            this.mNeedsDualRenderSurface = true;
		}

		//*******************************************************************
		public HBlurWhiteout(float length) : base(length)
	    {
            this.mNeedsDualRenderSurface = true;
	    }

		//*******************************************************************
		public override CTransitionEffect Clone()
		{
			CTransitionEffect t= new HBlurWhiteout(mLength) ;
			t.Index = this.Index;
			return t;
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

                RenderSurface nextInputSurface = mRenderSurface;
                mRenderSurface = null;

                int passes = 10;
                CBlurFilterDecoration vfd = new CBlurFilterDecoration();
                vfd.Strength = 0;

                for (int i = 0; i < passes; i++)
                {
                    RenderSurface nextOutputSurface = null;

                    nextOutputSurface = ge.GenerateRenderSurface((uint)dest_rec.Width, (uint)dest_rec.Height, this.ToString() + "::ProcessDualRenderSurfaces2");             
                    ge.SetRenderTarget(nextOutputSurface);

                    float max = 1.0f / 120.0f;      // more heavy than the BlurTransisionEffect, works with the white saturation

                    if (delta < 0.5)
                    {
                        vfd.BlurStep = max * (delta * 2);
                    }
                    else
                    {
                        vfd.BlurStep = max - (((delta - 0.5f) * 2) * max);
                    }

                    vfd.RenderToGraphics(dest_rec, frame, null, nextInputSurface);

                    nextInputSurface.Dispose();
                    nextInputSurface = null;

                    nextInputSurface = nextOutputSurface;
                }

                // now do white out part
                MiddleSaturateToColorTransitionEffect mstcte= new MiddleSaturateToColorTransitionEffect(Color.White, this.mLength);

                ge.SetRenderTarget(originalSurface);

                try
                {
                    mstcte.ProcessRectangle(delta, 0, 0, (int)dest_rec.Width, (int)dest_rec.Height, nextInputSurface, nextInputSurface);
                }
                finally
                {
                    nextInputSurface.Dispose();
                    nextInputSurface = null;
                    ge.SetPixelShaderEffect(null);
                }
            }
        }

		//*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
		{
			effect.SetAttribute("Type","HBlurWhiteout");
			SaveTransitionPart(effect,doc);
			parent.AppendChild(effect); 
		}

			
		//*******************************************************************
		public override void Load(XmlElement element)
		{
			LoadTransitionPart(element);
		}
	}
	
}
