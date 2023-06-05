using System;
using System.Xml;
using System.Drawing.Imaging;

namespace DVDSlideshow
{
	/// <summary>
    /// Summary description for RadialTransitionEffect.
	/// </summary>
	public class RadialTransitionEffect : HeightMapTransition
	{
        private bool mRight = false;        // spin right or left
        private bool mTop = false;          // start from top or bottom
        private bool mSplit = false;    // 2 sections
        private bool mSplitSplit = false; // 4 sections;
        private int mPos = 0;   // centre of radial,  0 = center, 1..4 = corners

		public RadialTransitionEffect()
		{
		}

		public RadialTransitionEffect(float length, int spread, bool right, bool top, bool split, bool splitsplit, int pos) : base(length, spread)
		{
            mRight = right;
            mTop = top;
            mSplit = split;
            mSplitSplit = splitsplit;
            mPos = pos;
		}

		//*******************************************************************
		public override CTransitionEffect Clone()
		{ 
			RadialTransitionEffect t= new RadialTransitionEffect(mLength, mSpread, mRight, mTop, mSplit, mSplitSplit, mPos) ;
			t.Index = this.Index;
			return t;
		}

		//*******************************************************************
		public override string GetIdString()
		{
            return "Radial" + mSpread.ToString() + mRight.ToString() + mTop.ToString() + mSplit.ToString() + mSplitSplit.ToString();
		}

		//*******************************************************************
        public override void BuildMap(int width, int height, BitmapData bitmapData)
		{
            float real_spread = mSpread * 256;

            float half_spread = real_spread / 2;

			unsafe
			{
                uint* ptr = (uint*)bitmapData.Scan0.ToPointer();
                int d_pitch = bitmapData.Stride - (width *4);
                d_pitch >>= 2;
         
				double radius = Math.Sqrt(width*width+height*height)/2;
                double half_width = width / 2;
                double half_height = height / 2;
                double m_minus_spread = 65535.0 - real_spread;

                double max = 360;
                if (mSplit == true) max = 180;
                if (mSplitSplit == true) max = 90;

                int widthpos = width/2;
                int heightpos = height/2;

                if (mPos != 0)
                {
                    if (mPos == 1)
                    {
                        widthpos = 0;
                        heightpos = 0;    
                    }

                    if (mPos == 2)
                    {
                        widthpos = 0;
                        heightpos = height;
                    }

                    if (mPos == 3)
                    {
                        widthpos = width;
                        heightpos = height;
                    }
                    if (mPos == 4)
                    {
                        widthpos = width;
                        heightpos = 0;
                    }

                    max = 90;
                }

			
				for (int i=0;i<height;i++)
				{
					for (int j=0;j<width;j++)
					{
						double ydiff = Math.Abs(half_height - i);
						double xdiff = Math.Abs(half_width - j);

                        float hh = i;
                        if (mTop == true) hh = height - hh;

                        double angleRadians = Math.Atan2(widthpos - j, heightpos - hh);

                        double degrees = angleRadians * 180 / Math.PI;

                        if (mSplit == false)
                        {
                            degrees += 180;
                            if (mRight == false) degrees = 360 - degrees;
                        }
                        else
                        {
                            degrees = Math.Abs(degrees);

                            if (mSplitSplit)
                            {
                                if (degrees > 90) degrees = 90 - (degrees - 90);
                            }
                        }

                        double r1 = (degrees / max);
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
			effect.SetAttribute("Type","RadialTransitionEffect");
			SaveHeightMapTransitionPart(effect,doc);
            effect.SetAttribute("Right", this.mRight.ToString());
            effect.SetAttribute("Top", this.mTop.ToString());
            effect.SetAttribute("Split", this.mSplit.ToString());
            effect.SetAttribute("Mirror", this.mSplitSplit.ToString());
            effect.SetAttribute("Pos", this.mPos.ToString());


			parent.AppendChild(effect); 
		}

		
		//*******************************************************************
		public override void Load(XmlElement element)
		{
			LoadHeightMapTransitionPart(element);

            string s = element.GetAttribute("Right");

            if (s != "")
            {
                mRight = bool.Parse(s);
            }

            s = element.GetAttribute("Top");
            if (s != "")
            {
                mTop = bool.Parse(s);
            }

            s = element.GetAttribute("Split");
            if (s != "")
            {
                mSplit = bool.Parse(s);
            }

            s = element.GetAttribute("Mirror");
            if (s != "")
            {
                mSplitSplit = bool.Parse(s);
            }

            s = element.GetAttribute("Pos");
            if (s != "")
            {
                mPos = int.Parse(s);
            }
		}
	}
}
