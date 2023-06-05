using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for AlphaSwipTransitionEffect.
	/// </summary>
    public class AlphaSwipTransitionEffect : HeightMapTransition 
	{
		private static byte [] mAlpha = null;

         
		SwipeDirection mDirection = SwipeDirection.RIGHT;


        public SwipeDirection Direction
        {
            get { return mDirection; }
        }


		public AlphaSwipTransitionEffect()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public enum SwipeDirection
		{
			LEFT,
			RIGHT
		}
	
	
		public AlphaSwipTransitionEffect(float length, SwipeDirection d, int spread) : base(length, spread)
		{
			mDirection =d ;
		}


		//*******************************************************************
		public override CTransitionEffect Clone()
		{ 
			CTransitionEffect t= new AlphaSwipTransitionEffect(mLength, mDirection, mSpread) ;
			t.Index = this.Index;
			return t;
		}

        //*******************************************************************
        public override string GetIdString()
        {
            return "AlphaSwip" + mSpread.ToString() + mDirection.ToString();
        }


        //*******************************************************************
        public override void BuildMap(int width, int height, BitmapData bitmapData)
        {
            float real_spread = mSpread * 256.0f;

            float half_spread = real_spread / 2.0f;

            unsafe
            {
                uint* ptr = (uint*)bitmapData.Scan0.ToPointer();
                int pitch = bitmapData.Stride;
                pitch >>= 2;

                float delta = ((float)65535.0f - real_spread) / ((float)width);

                float current = half_spread;

                if (mDirection == SwipeDirection.RIGHT)
                {
                    for (int j = width-1; j >=0; j--)
                    {
                        for (int i = 0; i < height; i++)
                        {
                            ptr[(i * pitch) +j] = ((uint)current);
                        }

                        current += delta;
                    }
                }
                else
                {
                    for (int j = 0; j < width; j++)
                    {
                        for (int i = 0; i < height; i++)
                        {
                            ptr[(i * pitch) + j] = ((uint)current);
                        }

                        current += delta;
                    }
                }
            }
        }

		//*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
		{
			effect.SetAttribute("Type","AlphaSwipeTransitionEffect");
			effect.SetAttribute("Direction",((int)this.mDirection).ToString());
            SaveHeightMapTransitionPart(effect, doc);
            parent.AppendChild(effect); 
		}

		
		//*******************************************************************
		public override void Load(XmlElement element)
		{
			string s1 = element.GetAttribute("Direction");
			if (s1!="")
			{
				mDirection = (SwipeDirection) int.Parse(s1);
			}
			s1 = element.GetAttribute("WidthCoef");     // legacy
			if (s1!="")
			{
                mSpread = int.Parse(s1);
			}

            LoadHeightMapTransitionPart(element);
		}
	}
}
