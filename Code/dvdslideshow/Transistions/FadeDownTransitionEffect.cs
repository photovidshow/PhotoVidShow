using System;
using System.Xml;
using System.Drawing.Imaging;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for FadeUpTransitionEffect.
	/// </summary>
	public class FadeDownTransitionEffect : HeightMapTransition
	{
		public FadeDownTransitionEffect()
		{
			//
			// TODO: Add constructor logic here
		
		}

		public FadeDownTransitionEffect(float length, int spread) : base(length, spread)
		{
			
		}

		//*******************************************************************
		public override CTransitionEffect Clone()
		{ 
			FadeDownTransitionEffect t= new FadeDownTransitionEffect(mLength, mSpread) ;
			t.Index = this.Index;
			return t;
		}

		//*******************************************************************
        public override string GetIdString()
		{
			return "2";
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

				float delta = ((float)65535.0f - real_spread)/ ((float)height);
				float current =65535.0f-half_spread;

				for (int i=0;i<height;i++)
				{
					for (int j=0;j<width;j++)
					{
						ptr[(i*pitch)+j] = (uint)current;
					}

					current -= delta;
				}
			}
		}

		
		//*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
		{
			effect.SetAttribute("Type","FadeDownTransitionEffect");
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
