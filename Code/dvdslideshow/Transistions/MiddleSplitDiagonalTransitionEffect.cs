using System;
using System.Xml;
using System.Drawing.Imaging;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for MiddleSplitDiagonalTransitionEffect.
    /// </summary>
    public class MiddleSplitDiagonalTransitionEffect : HeightMapTransition
    {
        private bool mMiddleOut = true;
        private bool mTopLeftBottomRight = true;
        public MiddleSplitDiagonalTransitionEffect()
        {
          
        }

        public MiddleSplitDiagonalTransitionEffect(bool middleout, bool topLeftBottomRight, float length, int spread)
            : base(length, spread)
        {
            mMiddleOut = middleout;
            mTopLeftBottomRight = topLeftBottomRight;
        }

        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            MiddleSplitDiagonalTransitionEffect t = new MiddleSplitDiagonalTransitionEffect(mMiddleOut, mTopLeftBottomRight, mLength, mSpread);
            t.Index = this.Index;
            return t;
        }

        //*******************************************************************
        public override string GetIdString()
        {
            return "12" + mMiddleOut.ToString() + mTopLeftBottomRight.ToString();
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

                float delta = ((float)65535.0f - real_spread) / ((float)height) * 2.0f;

                float current = half_spread;

                if (mMiddleOut == false) current = 65535.0f - half_spread;

                int half_height = height >> 1;
                if ((half_height % 2) == 1) half_height++;
                float range = 65535.0f - half_spread;

                float normalLength = (float) Math.Sqrt( width*width + height*height );
                float halfLength = normalLength / 2;

                for (int h = 0; h < height; h++)
                {
                    for (int w = 0; w < width; w++)
                    {
                        float dist = 0;

                        if (mTopLeftBottomRight == true)
                        {
                            dist = ((float)Math.Abs((w * height) - (h * width))) / normalLength;
                        }
                        else
                        {
                            dist = ((float)Math.Abs(((width-w) * height) - (h * width))) / normalLength;
                        }

                        float r1 = 0;
                        if (dist != 0)
                        {
                            r1 = (dist / halfLength) * range;
                        }
                        if (mMiddleOut == true)
                        {
                            r1 =  (range - r1);
                        }        

                        ptr[(h * pitch) + w] = (uint)r1;
                    }
                }
            }
        }

        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
        {
            effect.SetAttribute("Type", "MiddleSplitDiagonalTransitionEffect");
            effect.SetAttribute("MiddleOut", mMiddleOut.ToString());
            effect.SetAttribute("Tlbr", mTopLeftBottomRight.ToString());
            SaveHeightMapTransitionPart(effect, doc);
            parent.AppendChild(effect);
        }


        //*******************************************************************
        public override void Load(XmlElement element)
        {
            string s1 = element.GetAttribute("MiddleOut");
            if (s1 != "") mMiddleOut = bool.Parse(s1);

            s1 = element.GetAttribute("Tlbr");
            if (s1 != "") mTopLeftBottomRight = bool.Parse(s1);

            LoadHeightMapTransitionPart(element);
        }
    }
}
