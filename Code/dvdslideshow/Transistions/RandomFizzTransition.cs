using System;
using System.Xml;
using System.Drawing.Imaging;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for FadeUpTransitionEffect.
	/// </summary>
	public class RandomFizzTransition : HeightMapTransition
	{
		public RandomFizzTransition()
		{
			//
			// TODO: Add constructor logic here
		
			
		}

		public RandomFizzTransition(float length, int spread) : base(length, spread)
		{
			
		}

		//*******************************************************************
		public override CTransitionEffect Clone()
		{ 
			RandomFizzTransition t= new RandomFizzTransition(mLength, mSpread) ;
			t.Index = this.Index;
			return t;
		}

		//*******************************************************************
        public override string GetIdString()
		{
			return "3";
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

				float current =half_spread;

				Random r = new System.Random(12324);
			
				for (int i=0;i<height;i+=2)
				{
					for (int j=0;j<width;j+=2)
					{
                        uint nf = (uint)(half_spread + (r.Next() % (65535 - ((int)real_spread)))); ;
                        ptr[(i * pitch) + j] = nf;

						if (j+1 <width)
						{
                            ptr[(i * pitch) + (j + 1)] = nf;
						}
						if (i+1 < height)
						{
                            ptr[((i + 1) * pitch) + j] = nf;

							if (j+1 < width)
							{
                                ptr[((i + 1) * pitch) + (j + 1)] = nf;
							}
						}

					}

					current += delta;
				}
			}
		}

		//*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
		{
			effect.SetAttribute("Type","RandomFizzTransitionEffect");
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
