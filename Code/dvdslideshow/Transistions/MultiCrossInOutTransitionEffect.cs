using System;
using System.Xml;
using System.Drawing.Imaging;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for MultiCrossInOutTransitionEffect.
    /// </summary>
    public class MultiCrossInOutTransitionEffect : MultiShapeInOutTransitionEffect
    {
        public MultiCrossInOutTransitionEffect()
        {
        }

        public MultiCrossInOutTransitionEffect(bool outEffect, float circlesW, float wRat, float hRat, float length, int spread)
            : base(outEffect, circlesW, wRat, hRat, length, spread)
        {
        }

        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            MultiCrossInOutTransitionEffect t = new MultiCrossInOutTransitionEffect(mOut, mCirclesW, mWrat, mHrat, mLength, mSpread);
            t.Index = this.Index;
            return t;
        }

        //*******************************************************************
        public override string GetIdString()
        {
            return "MultiCross" + base.GetIdString();
        }


        //*******************************************************************
        public override double GetMaxPossibleValue(double width, double height)
        {
            if (width > height) return width / 2;
            return height / 2;
        }

        //*******************************************************************
        public override double GetDist(double xdiff, double ydiff, int block_w, int block_h)
        {
            if ((block_w % 2) == (block_h % 2))
            {
                if (xdiff > ydiff) return xdiff;
                return ydiff;

            }

            if (xdiff > ydiff) return ydiff;
            return xdiff;
        }


        //******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
        {
            effect.SetAttribute("Type", "MultiCrossInOutTransitionEffect");

            base.Save(parent, doc, effect);
        }

    }
}
