using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for SimpleAlphaBlendTransitionEffect.
	/// </summary>
    /// 
 	
	//*******************************************************************
	public class SimpleAlphaBlendTransitionEffect : CTransitionEffect
	{

        static PixelShaderEffect mShaderEffect = null;  // only one shader effect needed per instance of this class

		public SimpleAlphaBlendTransitionEffect()
		{
		}


        public SimpleAlphaBlendTransitionEffect(float length)
            : base(length)
        {
        }

		//*******************************************************************
		public override CTransitionEffect Clone()
		{
			CTransitionEffect t =  new SimpleAlphaBlendTransitionEffect(mLength) ;
			t.Index = this.Index;
			return t;
		}

        private CImage testImage = null;


        //*******************************************************************
        protected override void Process2(float delta, RenderSurface nextSlideSurface, int frame, Rectangle? innerRegion)
        {
            RectangleF dest_rec = new RectangleF(0, 0, nextSlideSurface.Width, nextSlideSurface.Height);
            RectangleF src_rec = new RectangleF(0,0,1,1);
            if (innerRegion.HasValue)
            {
                dest_rec = new RectangleF(innerRegion.Value.X, innerRegion.Value.Y, innerRegion.Value.Width, innerRegion.Value.Height);
                src_rec = new RectangleF(innerRegion.Value.X , innerRegion.Value.Y, innerRegion.Value.Width + innerRegion.Value.X, innerRegion.Value.Height + innerRegion.Value.Y);
                src_rec.X /= nextSlideSurface.Width;
                src_rec.Y /= nextSlideSurface.Height;
                src_rec.Width /= nextSlideSurface.Width;
                src_rec.Height /= nextSlideSurface.Height;
            }

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
			effect.SetAttribute("Type","SimpleAlphaBlendTransitionEffect");
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
