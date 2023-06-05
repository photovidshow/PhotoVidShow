using System;
using System.Xml;
using System.Drawing.Imaging;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for FanTransitionEffect.
    /// </summary>
    public class FanTransitionEffect : HeightMapTransition
    {
     
        private float mAngleMult = 0;           // fan curve factor (0 == straight line from centre)
        private bool mDistSinMult = false;      // depth sin wave is factored by dist from centre, true, false
        private int mSegments = 6;              // number of fan segments

        public FanTransitionEffect()
        {
        }

        public FanTransitionEffect(float length, int spread, float angleMult, bool distSinMult, int segments)
            : base(length, spread)
        {
            mAngleMult = angleMult;
            mDistSinMult = distSinMult;
            mSegments = segments;

        }

        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            FanTransitionEffect t = new FanTransitionEffect(mLength, mSpread, mAngleMult, mDistSinMult, mSegments);
            t.Index = this.Index;
            return t;
        }

        //*******************************************************************
        public override string GetIdString()
        {
            return "FanTransitionEffect" + mAngleMult.ToString() + mDistSinMult.ToString() + mSegments.ToString();
        }

        //*******************************************************************
        public override void BuildMap(int width, int height, BitmapData bitmapData)
        {
            float real_spread = mSpread * 256;

            float half_spread = real_spread / 2;

            unsafe
            {
                uint* ptr = (uint*)bitmapData.Scan0.ToPointer();
                int d_pitch = bitmapData.Stride - (width * 4);
                d_pitch >>= 2;

                double radius = Math.Sqrt(width * width + height * height) / 2;
                double half_width = width / 2;
                double half_height = height / 2;
                double m_minus_spread = 65535.0 - real_spread;

                double max = 360;
   
                int widthpos = width / 2;
                int heightpos = height / 2;

                double maxdist = Math.Sqrt(width * width + height * height) / 2;

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        double ydiff = Math.Abs(half_height - i);
                        double xdiff = Math.Abs(half_width - j);

                        float hh = i;

                        double angleRadians = Math.Atan2(widthpos - j, heightpos - hh);

                        double dist = Math.Sqrt(xdiff * xdiff + ydiff * ydiff) / 2;

                        double angledist = (dist * mAngleMult) / maxdist;
                        angleRadians += angledist;

                        double sinmulti = 180;
                        if (mDistSinMult == true)
                        {
                            sinmulti = 180 * (dist / maxdist);
                        }

                        double degrees = (Math.Sin(angleRadians * mSegments) * sinmulti);
                        degrees += sinmulti;

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
            effect.SetAttribute("Type", "FanTransitionEffect");
            SaveHeightMapTransitionPart(effect, doc);

            if (mAngleMult != 0)
            {
                effect.SetAttribute("AngleMult", mAngleMult.ToString());
            }

            if (mDistSinMult == true)
            {
                effect.SetAttribute("DistSinMult", mDistSinMult.ToString());
            }

            effect.SetAttribute("Segments", mSegments.ToString());

          
            parent.AppendChild(effect);
        }


        //*******************************************************************
        public override void Load(XmlElement element)
        {
            LoadHeightMapTransitionPart(element);

            string s = element.GetAttribute("AngleMult");
            if (s != "")
            {
                mAngleMult = float.Parse(s);
            }

            s = element.GetAttribute("DistSinMult");
            if (s != "")
            {
                mDistSinMult = bool.Parse(s);
            }

            s = element.GetAttribute("Segments");
            if (s != "")
            {
                mSegments = int.Parse(s);
            }
        }
    }
}
