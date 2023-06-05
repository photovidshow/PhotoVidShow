using System;
using System.Xml;
using System.Drawing.Imaging;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for MultipleLinesUpDownTransitionEffect.
    /// </summary>
    public class MultipleLinesUpDownTransitionEffect : HeightMapTransition
    {
        private int mLines = 10;
        private bool mDown = true;

        public MultipleLinesUpDownTransitionEffect()
        {
            //
            // TODO: Add constructor logic here

        }

        public MultipleLinesUpDownTransitionEffect(int lines, bool down, float length, int spread)
            : base(length, spread)
        {
            mLines = lines;
            mDown = down;
        }

        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            MultipleLinesUpDownTransitionEffect t = new MultipleLinesUpDownTransitionEffect(mLines, mDown, mLength, mSpread);
            t.Index = this.Index;
            return t;
        }

        //*******************************************************************
        public override string GetIdString()
        {
            return "6"+mLines.ToString()+mDown.ToString();
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

                float count = height / mLines;

                float delta = (scale - (real_spread * 2)) / count;
                float current = real_spread;

                if (mDown == true) current = scale - real_spread;

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        ptr[(i * pitch) + j] = (uint)current;
                    }

                    if (mDown == true)
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
            effect.SetAttribute("Type", "MultipleLinesUpDownTransitionEffect");
            effect.SetAttribute("Lines", mLines.ToString());
            effect.SetAttribute("Down", mDown.ToString());

            SaveHeightMapTransitionPart(effect, doc);
            parent.AppendChild(effect);
        }


        //*******************************************************************
        public override void Load(XmlElement element)
        {
            string s1 = element.GetAttribute("Lines");
            if (s1 != "") mLines = int.Parse(s1);

            s1 = element.GetAttribute("Down");
            if (s1 != "") mDown = bool.Parse(s1);

            LoadHeightMapTransitionPart(element);
        }
    }
}
