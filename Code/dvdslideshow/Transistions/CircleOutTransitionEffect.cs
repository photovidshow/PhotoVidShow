using System;
using System.Xml;
using System.Drawing.Imaging;

namespace DVDSlideshow
{
	/// <summary>
    /// Summary description for CircleOutTransitionEffect.
	/// </summary>
	public class CircleOutTransitionEffect : HeightMapTransition
	{
        private float mYmult = 1;
        private float mXmult = 1;

        //*******************************************************************
		public CircleOutTransitionEffect()
		{
		}

        //*******************************************************************
        public CircleOutTransitionEffect(float length, int spread, float xMult, float yMult)
            : base(length, spread)
		{
            mXmult = xMult;
            mYmult = yMult;
		}

		//*******************************************************************
		public override CTransitionEffect Clone()
		{
            CircleOutTransitionEffect t = new CircleOutTransitionEffect(mLength, mSpread, mXmult, mYmult);
			t.Index = this.Index;
			return t;
		}

		//*******************************************************************
        public override string GetIdString()
		{
            return "CirlceOut " + mXmult.ToString() + " " + mYmult.ToString();
		}

		//*******************************************************************
        public override void BuildMap(int width, int height, BitmapData bitmapData)
		{
            float real_spread = mSpread * 256.0f;

            float half_spread = real_spread / 2;

			unsafe
			{
                uint* ptr = (uint*)bitmapData.Scan0.ToPointer();
                int pitch = bitmapData.Stride;
                pitch >>= 2;

                float delta = ((float)65535.0f - real_spread) / ((float)height);

				float current =half_spread;

                double ww = width * mXmult;
                double hh = height * mYmult;

                double max = Math.Sqrt(ww * ww + hh * hh) / 2;

				for (int i=0;i<height;i++)
				{
					for (int j=0;j<width;j++)
					{
						double ydiff = Math.Abs((height/2) - i);
						double xdiff = Math.Abs((width/2) - j);

                        ydiff *= mYmult;
                        xdiff *= mXmult;

						double dist = Math.Sqrt(ydiff*ydiff + xdiff*xdiff);

                        double r1 = (dist / max);
                        int p = 65535 - ( ((int)half_spread) + (int)((r1 * (65535.0f - real_spread))));

						ptr[(i*pitch)+j] = (uint) p;
					}

					current += delta;
				}
			}
		}

		//*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
		{
			effect.SetAttribute("Type","CircleOutTransitionEffect");
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
