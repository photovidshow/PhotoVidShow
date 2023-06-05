using System;
using System.Xml;
using System.Drawing.Imaging;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for MultipleLinesLeftRightTransitionEffect.
    /// </summary>
    public class MultipleLinesLeftRightTransitionEffect : HeightMapTransition
    {
        private int mLines = 10;
        private bool mRight = true;

        public MultipleLinesLeftRightTransitionEffect()
        {
            //
            // TODO: Add constructor logic here

        }

        public MultipleLinesLeftRightTransitionEffect(int lines, bool right, float length, int spread)
            : base(length, spread)
        {
            mLines = lines;
            mRight = right;
        }

        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            MultipleLinesLeftRightTransitionEffect t = new MultipleLinesLeftRightTransitionEffect(mLines, mRight, mLength, mSpread);
            t.Index = this.Index;
            return t;
        }

        //*******************************************************************
        public override string GetIdString()
        {
            return "9"+mLines.ToString()+mRight.ToString();
        }

        //*******************************************************************
        public override void BuildMap(int width, int height, BitmapData bitmapData)
        {
            float real_spread = mSpread * 256.0f;
            float scale = 65535.0f;

            unsafe
            {
                uint* ptr = (uint*)bitmapData.Scan0.ToPointer();
                int pitch = bitmapData.Stride;
                pitch >>= 2;

                float count = width / mLines;

                float delta = (scale - (real_spread * 2)) / count;
                float current = real_spread;

                if (mRight == true) current = scale - real_spread;

                for (int j = 0; j < width; j++)
                {
                    for (int i = 0; i < height; i++)
                    {
                        ptr[(i * pitch) + j] = (uint)current;
                    }

                    if (mRight == true)
                    {
                        current -= delta;
                        if (current < real_spread) current = scale - real_spread;
                    }
                    else
                    {
                        current += delta;
                        if (current > scale - real_spread) current = real_spread;
                    }
                }
            }
        }


        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
        {
            effect.SetAttribute("Type", "MultipleLinesLeftRightTransitionEffect");
            effect.SetAttribute("Lines", mLines.ToString());
            effect.SetAttribute("Right", mRight.ToString());

            SaveHeightMapTransitionPart(effect, doc);
            parent.AppendChild(effect);
        }


        //*******************************************************************
        public override void Load(XmlElement element)
        {
            string s1 = element.GetAttribute("Lines");
            if (s1 != "") mLines = int.Parse(s1);

            s1 = element.GetAttribute("Right");
            if (s1 != "") mRight = bool.Parse(s1);

            LoadHeightMapTransitionPart(element);
        }
    }
}
