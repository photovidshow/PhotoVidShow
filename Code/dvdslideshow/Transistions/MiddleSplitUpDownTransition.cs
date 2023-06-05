using System;
using System.Xml;
using System.Drawing.Imaging;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for MiddleSplitUpDownTransition.
    /// </summary>
    public class MiddleSplitUpDownTransition : HeightMapTransition
    {
        private bool mMiddleOut = true;
        public MiddleSplitUpDownTransition()
        {
          
        }

        public MiddleSplitUpDownTransition(bool middleout, float length, int spread)
            : base(length, spread)
        {
            mMiddleOut = middleout;
        }

        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            MiddleSplitUpDownTransition t = new MiddleSplitUpDownTransition(mMiddleOut, mLength, mSpread);
            t.Index = this.Index;
            return t;
        }

        //*******************************************************************
        public override string GetIdString()
        {
            return "10" + mMiddleOut.ToString();
        }

        //*******************************************************************
        public override void BuildMap(int width, int height, BitmapData bitmapData)
        {
            float real_spread = mSpread * 256.0f;

            float half_spread = mSpread / 2;

            unsafe
            {
                uint* ptr = (uint*)bitmapData.Scan0.ToPointer();
                int pitch = bitmapData.Stride;
                pitch >>= 2;

                float delta = (((float)65535.0f - real_spread) / ((float)height)) * 2.0f;

                float current = half_spread;

                if (mMiddleOut == false) current = 65535.0f - half_spread;

                int half_height = height >> 1;
                if ((half_height % 2) == 1) half_height++;

                for (int i = 0; i <= half_height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        ptr[(i * pitch) + j] = (uint)current;
                        ptr[((height - i - 1) * pitch) + j] = (uint)current;
                    }

                    if (mMiddleOut == true)
                    {
                        current += delta;
                    }
                    else
                    {
                        current -= delta;
                    }
                }
            }
        }

        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
        {
            effect.SetAttribute("Type", "MiddleSplitUpDownTransition");
            effect.SetAttribute("MiddleOut", mMiddleOut.ToString());
            SaveHeightMapTransitionPart(effect, doc);
            parent.AppendChild(effect);
        }


        //*******************************************************************
        public override void Load(XmlElement element)
        {
            string s1 = element.GetAttribute("MiddleOut");
            if (s1 != "") mMiddleOut = bool.Parse(s1);
            LoadHeightMapTransitionPart(element);
        }
    }
}
