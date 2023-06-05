using System;
using System.Xml;
using System.Drawing.Imaging;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for MultiSquaresInOutTransitionEffect.
    /// </summary>
    public class MultiSquaresInOutTransitionEffect : MultiShapeInOutTransitionEffect
    {
        public MultiSquaresInOutTransitionEffect()
        {
        }

        public MultiSquaresInOutTransitionEffect(bool outEffect, float circlesW, float wRat, float hRat, float length, int spread)
            : base(outEffect, circlesW, wRat, hRat, length, spread)
        {

        }

        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            MultiSquaresInOutTransitionEffect t = new MultiSquaresInOutTransitionEffect(mOut, mCirclesW, mWrat, mHrat, mLength, mSpread);
            t.Index = this.Index;
            return t;
        }

        //*******************************************************************
        public override string GetIdString()
        {
            return "MultiSquares" + base.GetIdString();
        }


        //*******************************************************************
        public override double GetMaxPossibleValue(double width, double height)
        {
            if (width > height) return width/2;
            return height / 2;
        }

        //*******************************************************************
        public override double GetDist(double xdiff, double ydiff)
        {
            if (xdiff > ydiff) return xdiff;
            return ydiff;
        }


        //******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
        {
            effect.SetAttribute("Type", "MultiSquaresInOutTransitionEffect");

            base.Save(parent, doc, effect);
        }

    }
}
