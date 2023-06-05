using System;
using System.Xml;
using System.Drawing.Imaging;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for FadeUpTransitionEffect.
	/// </summary>
	public class FadeUpTransitionEffect : HeightMapTransition
	{
		public FadeUpTransitionEffect()
		{
			//
			// TODO: Add constructor logic here
		
			
		}

		public FadeUpTransitionEffect(float length, int spread) : base(length, spread)
		{
			
		}

		//*******************************************************************
		public override CTransitionEffect Clone()
		{ 
			FadeUpTransitionEffect t= new FadeUpTransitionEffect(mLength, mSpread) ;
			t.Index = this.Index;
			return t;
		}

		//*******************************************************************
        public override string GetIdString()
		{
			return "1";
		}

		//*******************************************************************
		public override void BuildMap(int width, int height, BitmapData bitmapData)
		{
            float real_spread = mSpread * 256.0f;

			float half_spread = mSpread/2;

			unsafe
			{
                uint* ptr = (uint*)bitmapData.Scan0.ToPointer();
                int pitch = bitmapData.Stride;
                pitch >>= 2;

				float delta = ((float)65535.0f - mSpread)/ ((float)height);

                float current = half_spread;
			
				for (int i=0;i<height;i++)
				{
					for (int j=0;j<width;j++)
					{
						ptr[(i*pitch)+j] = (uint)(current+0.4999);
					}

					current += delta;
				}
			}
		}

		//*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
		{
			effect.SetAttribute("Type","FadeUpTransitionEffect");
			SaveHeightMapTransitionPart(effect,doc);
			parent.AppendChild(effect); 
		}

		
		//*******************************************************************
		public override void Load(XmlElement element)
		{
			LoadHeightMapTransitionPart(element);
		}
	}
}
