using System;
using System.Xml;
using System.Drawing.Imaging;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for MiddleSplitLeftRightTransition.
    /// </summary>
    public class MiddleSplitLeftRightTransition : HeightMapTransition
    {
        private bool mMiddleOut = true;
        public MiddleSplitLeftRightTransition()
        {
          
        }

        public MiddleSplitLeftRightTransition(bool middleout, float length, int spread)
            : base(length, spread)
        {
            mMiddleOut = middleout;
        }

        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            MiddleSplitLeftRightTransition t = new MiddleSplitLeftRightTransition(mMiddleOut, mLength, mSpread);
            t.Index = this.Index;
            return t;
        }

        //*******************************************************************
        public override string GetIdString()
        {
            return "11" + mMiddleOut.ToString();
        }

        //*******************************************************************
        public override void BuildMap(int width, int height, BitmapData bitmapData)
        {
            float real_spread = mSpread * 256.0f;
            float scale = 65535.0f;

            float half_spread = real_spread / 2;

            unsafe
            {
                uint* ptr = (uint*)bitmapData.Scan0.ToPointer();
                int pitch = bitmapData.Stride;
                pitch >>= 2;

                float delta = ((float)scale - real_spread) / ((float)width) * 2.0f;

                float current = half_spread;

                if (mMiddleOut == false) current = scale - half_spread;

                int half_width = width >> 1;
                if ((width % 2) == 1) half_width++;

                for (int j = 0; j < half_width; j++)
                {
                    for (int i = 0; i < height; i++)
                    {
                        ptr[(i * pitch) + j] = (uint)current;
                        ptr[(i * pitch) + (pitch - 1 - j)] = (uint)current;
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
            effect.SetAttribute("Type", "MiddleSplitLeftRightTransition");
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
