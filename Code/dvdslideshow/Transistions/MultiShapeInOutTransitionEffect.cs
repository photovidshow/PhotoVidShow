using System;
using System.Xml;
using System.Drawing.Imaging;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for MultiShapeInOutTransitionEffect.
    /// </summary>
    public abstract class MultiShapeInOutTransitionEffect : HeightMapTransition
    {
        protected bool mOut = false;
        protected float mCirclesW = 15;
        protected float mHrat = 1.0f;  // must be above 1.0
        protected float mWrat = 1.0f;  // must be above 1.0

        public bool OutEffect
        {
            get { return mOut; }
        }

        public MultiShapeInOutTransitionEffect()
        {
            //
            // TODO: Add constructor logic here


        }

        public MultiShapeInOutTransitionEffect(bool outEffect, float circlesW, float wRat, float hRat, float length, int spread)
            : base(length, spread)
        {
            mOut = outEffect;
            mCirclesW = circlesW;
            mHrat = hRat;
            mWrat = wRat;
        }

        //*******************************************************************
        public override string GetIdString()
        {
            return mOut.ToString() + mCirclesW.ToString() + mHrat.ToString() + mWrat.ToString();
        }

        //*******************************************************************
        public abstract double GetMaxPossibleValue(double width, double height);

        //*******************************************************************
        public virtual double GetDist(double xdiff, double ydiff)
        {
            return 0;
        }

        //*******************************************************************
        public virtual double GetDist(double xdiff, double ydiff, int block_w, int block_h)
        {
            return GetDist(xdiff, ydiff);
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

                int circle_block_sw = (int)(((float)width / mCirclesW) * mWrat);
                int circle_block_sh = (int)(((float)width / mCirclesW) * mHrat);
                int circle_block_rw = circle_block_sw >> 1;
                int circle_block_rh = circle_block_sh >> 1;

                double max = GetMaxPossibleValue(circle_block_sw, circle_block_sh);

                int circle_h = 0;
                int block_h = 0;

                for (int i = 0; i < height; i++)
                {
                    int circle_w = 0;
                    int block_w = 0;

                    for (int j = 0; j < width; j++)
                    {
                        double ydiff = Math.Abs(circle_block_rh - circle_h) / mHrat;
                        double xdiff = Math.Abs(circle_block_rw - circle_w) / mWrat;

                        double dist = GetDist(xdiff, ydiff, block_w, block_h);

                        double r1 = (dist / max);

                        int p = 0;
                        if (mOut)
                        {
                            p = 65536 - ((int)half_spread) - (int)((r1 * (65535.0f - real_spread)));
                        }
                        else
                        {
                            p = ((int)half_spread) + (int)((r1 * (65535.0f - real_spread)));
                        }

                        ptr[(i * pitch) + j] = (uint)p;

                        circle_w++;

                        if (circle_w > circle_block_sw)
                        {
                            block_w++;
                            circle_w = 0;
                        }
                    }

                    circle_h++;
                    if (circle_h > circle_block_sh)
                    {
                        circle_h = 0;
                        block_h++;
                    }

                }
            }
        }

        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
        {
            effect.SetAttribute("Out", mOut.ToString());
            effect.SetAttribute("Hrat", mHrat.ToString());
            effect.SetAttribute("Wrat", mWrat.ToString());
            effect.SetAttribute("CirclesW", mCirclesW.ToString());

            SaveHeightMapTransitionPart(effect, doc);
            parent.AppendChild(effect);
        }


        //*******************************************************************
        public override void Load(XmlElement element)
        {
            string s1 = element.GetAttribute("Out");
            if (s1 != "") mOut = bool.Parse(s1);

            s1 = element.GetAttribute("Hrat");
            if (s1 != "") mHrat = float.Parse(s1);

            s1 = element.GetAttribute("Wrat");
            if (s1 != "") mWrat = float.Parse(s1);

            s1 = element.GetAttribute("CirclesW");
            if (s1 != "") mCirclesW = float.Parse(s1);

            LoadHeightMapTransitionPart(element);
        }
    }
}
