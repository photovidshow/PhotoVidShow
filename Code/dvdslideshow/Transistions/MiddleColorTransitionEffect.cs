using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
	/// <summary>
    /// Summary description for MiddleColorTransitionEffect.
	/// </summary>
	public class MiddleColorTransitionEffect : CTransitionEffect 
	{
     
		private Color mColor;

		//*******************************************************************
		public MiddleColorTransitionEffect()
		{
		}

		//*******************************************************************
		public MiddleColorTransitionEffect(Color c, float length) : base(length)
		{
			mColor = c;
		}


		//*******************************************************************
		public override CTransitionEffect Clone()
		{
			CTransitionEffect t= new MiddleColorTransitionEffect(mColor, mLength) ;
			t.Index = this.Index;
			return t;
		}

        //*******************************************************************
        protected override void Process2(float delta, RenderSurface nextSlideSurface, int frame, Rectangle? innerRegion)
        {
            GraphicsEngine engine = GraphicsEngine.Current;

			if (delta < 0.5f)
			{
                RenderSurface current = engine.GetRenderTarget();
                engine.SetRenderTarget(nextSlideSurface);
                engine.ClearRenderTarget(mColor.R, mColor.G, mColor.B, 255);
                engine.SetRenderTarget(current);
                delta *= 2;
			}
			else
			{
                engine.ClearRenderTarget(mColor.R, mColor.G, mColor.B, 255);
                delta -= 0.5f;
                delta *= 2;
			}

            RectangleF src_rec = new RectangleF(0, 0, 1, 1);
            RectangleF dest_rec = new RectangleF(0, 0, nextSlideSurface.Width, nextSlideSurface.Height);;

            if (innerRegion.HasValue)
            {
                dest_rec = new RectangleF(innerRegion.Value.X, innerRegion.Value.Y, innerRegion.Value.Width, innerRegion.Value.Height);
                src_rec = new RectangleF(innerRegion.Value.X, innerRegion.Value.Y, innerRegion.Value.Width + innerRegion.Value.X, innerRegion.Value.Height + innerRegion.Value.Y);

                src_rec.X /= nextSlideSurface.Width;
                src_rec.Y /= nextSlideSurface.Height;
                src_rec.Width /= nextSlideSurface.Width;
                src_rec.Height /= nextSlideSurface.Height;
            }

            GraphicsEngine.Current.DrawImageWithDiffuseAlpha(nextSlideSurface, delta, src_rec, dest_rec, false);
		}

		//*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
		{
            effect.SetAttribute("Type", "MiddleSaturateToColorTransitionEffect");
			effect.SetAttribute("Color",this.mColor.ToString());
			SaveTransitionPart(effect,doc);
			parent.AppendChild(effect); 
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
