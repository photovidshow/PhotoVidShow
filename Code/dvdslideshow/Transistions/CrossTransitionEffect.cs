using System;
using System.Xml;
using System.Drawing.Imaging;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for CrossTransitionEffect.
    /// </summary>
    public class CrossTransitionEffect : HeightMapTransition
    {
        private bool mOutEffect = false;

        public CrossTransitionEffect()
        {
        }

        public CrossTransitionEffect(float length, int spread, bool outEffect)
            : base(length, spread)
        {
            mOutEffect = outEffect;
        }

        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            CrossTransitionEffect t = new CrossTransitionEffect(mLength, mSpread, mOutEffect);
            t.Index = this.Index;
            return t;
        }

        //*******************************************************************
        public override string GetIdString()
        {
            return "CrossTransitionEffect" +mOutEffect.ToString();
        }

        //*******************************************************************
        public override void BuildMap(int width, int height, BitmapData bitmapData)
        {
            float real_spread = mSpread * 256.0f;

            float half_spread = real_spread / 2;

            int intHalfSpread = (int)(half_spread +0.49999f);

            unsafe
            {
                uint* ptr = (uint*)bitmapData.Scan0.ToPointer();
                int pitch = bitmapData.Stride;
                pitch >>= 2;

                float delta = ((float)65535.0f - real_spread) / ((float)height);

                float current = half_spread;

                double radius = width / 2;

                if (height < width)
                {
                    radius = height / 2;
                }

                float ratio = ((float)width) / ((float)height);

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        double ydiff = Math.Abs((height / 2) - i);
                        double xdiff = Math.Abs((width / 2) - j);

                        xdiff /= ratio;

                        double dist = ydiff < xdiff ? ydiff : xdiff;
                        
                        if (mOutEffect==false)
                        {
                            dist = radius-dist;
                        }

                        double r1 = (dist / radius);
                        int p = 65535 - (intHalfSpread + (int)((r1 * (65535.0f - real_spread))));

                        ptr[(i * pitch) + j] = (uint)p;
                    }

                    current += delta;
                }
            }
        }

        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
        {
            effect.SetAttribute("Type", "CrossTransitionEffect");
            SaveHeightMapTransitionPart(effect, doc);

            effect.SetAttribute("OutEffect", mOutEffect.ToString());
            parent.AppendChild(effect);
        }


        //*******************************************************************
        public override void Load(XmlElement element)
        {
            LoadHeightMapTransitionPart(element);

            string s = element.GetAttribute("OutEffect");
            if (s != "")
            {
                mOutEffect = bool.Parse(s);
            }
        }
    }
}
