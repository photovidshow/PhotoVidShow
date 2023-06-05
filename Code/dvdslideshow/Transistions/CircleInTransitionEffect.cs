using System;
using System.Xml;
using System.Drawing.Imaging;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for FadeUpTransitionEffect.
	/// </summary>
	public class CircleInTransitionEffect : HeightMapTransition
	{
        private float mYmult =1;
        private float mXmult = 1;

		public CircleInTransitionEffect()
		{
		}

		public CircleInTransitionEffect(float length, int spread, float xMult, float yMult) : base(length, spread)
		{
            mYmult = yMult;
            mXmult = xMult;
			
		}

		//*******************************************************************
		public override CTransitionEffect Clone()
		{ 
			CircleInTransitionEffect t= new CircleInTransitionEffect(mLength, mSpread, mXmult, mYmult) ;
			t.Index = this.Index;
			return t;
		}

		//*******************************************************************
		public override string GetIdString()
		{
			return "CirlceIn "+mXmult.ToString()+" "+mYmult.ToString();
		}

		//*******************************************************************
        public override void BuildMap(int width, int height, BitmapData bitmapData)
		{
            float real_spread = mSpread * 256.0f;

            float half_spread = real_spread / 2;

			unsafe
			{
                uint* ptr = (uint*)bitmapData.Scan0.ToPointer();
                int d_pitch = bitmapData.Stride - (width *4);
                d_pitch >>= 2;

                double ww = width * mXmult;
                double hh = height * mYmult;
         
				double max = Math.Sqrt(ww*ww+hh*hh)/2;
                double half_width = width / 2;
                double half_height = height / 2;
                double m_minus_spread = 65535.0 - real_spread;
			
				for (int i=0;i<height;i++)
				{
					for (int j=0;j<width;j++)
					{
						double ydiff = Math.Abs(half_height - i);
						double xdiff = Math.Abs(half_width - j);

                        ydiff *= mYmult;
                        xdiff *= mXmult;

						double dist = Math.Sqrt(ydiff*ydiff + xdiff*xdiff);

                        double r1 = (dist / max);
                        uint p = (uint)(half_spread + (int)((r1 * (m_minus_spread)) + 0.4999));

                        *ptr++ = p;
					}
                    ptr += d_pitch;
				}
			}
		}

		//*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
		{
			effect.SetAttribute("Type","CircleInTransitionEffect");
			SaveHeightMapTransitionPart(effect,doc);
            if (mYmult != 1)
            {
                effect.SetAttribute("YMult", mYmult.ToString());
            }

            if (mXmult != 1)
            {
                effect.SetAttribute("XMult", mXmult.ToString());
            }

			parent.AppendChild(effect); 
		}

		
		//*******************************************************************
		public override void Load(XmlElement element)
		{
			LoadHeightMapTransitionPart(element);

            string s1 = element.GetAttribute("YMult");
            if (s1 != "") mYmult = float.Parse(s1);

            s1 = element.GetAttribute("XMult");
            if (s1 != "") mXmult = float.Parse(s1);
		}
	}
}
