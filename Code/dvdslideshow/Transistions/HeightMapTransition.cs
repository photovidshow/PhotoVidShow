using System;
using System.Xml;
using System.Drawing;
using System.Drawing.Imaging;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for HeightMapTransition.
	/// </summary>
	public abstract class HeightMapTransition : CTransitionEffect, IImageCreator
	{
		protected int mSpread = 64;
        private CImage mMapImage = null;

		static object mMapObject = new object();

		public HeightMapTransition()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		//*******************************************************************
		public HeightMapTransition(float length, int spread) : base(length)
		{
			mSpread = spread;
		}

		//*******************************************************************
        private void TestIfHeightMapImageValid(int width, int height)
		{
            if (mMapImage != null)
            {
                if (mMapImage.GetRectangle().Width == width &&
                    mMapImage.GetRectangle().Height == height)
                {
                    return;
                }
            }

            // Ok invalid.
            // Create a new mapImage ( note we do not physically create the image yet, we simply pass in a ImageCreator object i.e this )
            mMapImage = new CImage(this, width, height);
		}

        //*******************************************************************
        // Implements method from IImageCreator
        public Bitmap CreateImage(int width, int height, ref bool createMipMaps)
        {
            createMipMaps = false;

            Bitmap b = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData bdata = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            BuildMap(width, height, bdata);
            b.UnlockBits(bdata);
       //     b.Save("C:\\heightmap.bmp");
            return b;
        }
         
		//*******************************************************************
		public abstract void BuildMap(int width, int height, BitmapData bitmapData);

		//*******************************************************************
        public abstract string GetIdString();

        //*******************************************************************
        protected override void Process2(float delta, RenderSurface nextSlideSurface, int frame, Rectangle? innerRegion)
        {
            // sometime this can be called same time in two different threads but only
			// 1 static map so lock it here

            lock (mMapObject)
            {
                int ww = (int)nextSlideSurface.Width;
                int hh = (int)nextSlideSurface.Height;
                int x = 0;
                int y = 0;
                 
                string shader = "heightmap";

                if (UseImage2Alpha == true)
                {
                    shader = "heightmapa"; 
                }

                if (innerRegion.HasValue)
                {
                    x = innerRegion.Value.X;
                    y = innerRegion.Value.Y;
                    ww = innerRegion.Value.Width;
                    hh = innerRegion.Value.Height;
                }

                TestIfHeightMapImageValid((int)ww, (int)hh);

                if (mMapImage == null)
                {
                    return;
                }

                RectangleF srcRec = new RectangleF(x, y, ww + x, hh + y);

                srcRec.X /= nextSlideSurface.Width;
                srcRec.Y /= nextSlideSurface.Height;
                srcRec.Width /= nextSlideSurface.Width; ;
                srcRec.Height /= nextSlideSurface.Height; 

                float passed_spread = (1.0f / (mSpread)) * 255.0f;

                PixelShaderEffect effect = PixelShaderEffectDatabase.GetInstance().GetPixelShader(shader);
                if (effect == null) return;

                effect.SetInputImage1(mMapImage);
                effect.SetParameter(0, 1.0f - delta);
                effect.SetParameter(1, passed_spread);
                effect.SetParameter(2, 1.0f / passed_spread / 2.0f);

                try
                {
                    GraphicsEngine.Current.SetPixelShaderEffect(effect);
                    GraphicsEngine.Current.DrawImage(nextSlideSurface, srcRec, x, y + hh, x, y, x + ww, y + hh, x + ww, y, true);
                }
                finally
                {
                    GraphicsEngine.Current.SetPixelShaderEffect(null);
                }
 
            }
        }

		//*******************************************************************
        protected override CImage Process(float delta, CImage ii1, CImage ii2, int frame, CImage render_to_this)
		{
		      return null;
		}

		//*******************************************************************
        public void SaveHeightMapTransitionPart(XmlElement parent, XmlDocument doc)
		{			
			parent.SetAttribute("Spread",this.mSpread.ToString());
			SaveTransitionPart(parent,doc);
		}

		
		//*******************************************************************
		public void LoadHeightMapTransitionPart(XmlElement element)
		{
			string s1 = element.GetAttribute("Spread");
			if (s1!="")
			{
				mSpread = int.Parse(s1);

			}
			LoadTransitionPart(element);
		}
	}
}
