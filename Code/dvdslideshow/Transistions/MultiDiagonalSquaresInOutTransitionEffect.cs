using System;
using System.Xml;
using System.Drawing.Imaging;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for MultiDiagonalSquaresInOutTransitionEffect.
    /// </summary>
    public class MultiDiagonalSquaresInOutTransitionEffect : MultiShapeInOutTransitionEffect
    {
        public MultiDiagonalSquaresInOutTransitionEffect()
        {
        }

        public MultiDiagonalSquaresInOutTransitionEffect(bool outEffect, float circlesW, float wRat, float hRat, float length, int spread)
            : base(outEffect, circlesW, wRat, hRat, length, spread)
        {
        }

        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            MultiDiagonalSquaresInOutTransitionEffect t = new MultiDiagonalSquaresInOutTransitionEffect(mOut, mCirclesW, mWrat, mHrat, mLength, mSpread);
            t.Index = this.Index;
            return t;
        }

        //*******************************************************************
        public override string GetIdString()
        {
            return "MultiDiagonalSquares" + base.GetIdString();
        }


        //*******************************************************************
        public override double GetMaxPossibleValue(double width, double height)
        {
            return (width + height) /2;
        }

        //*******************************************************************
        public override double GetDist(double xdiff, double ydiff)
        {
            return (xdiff + ydiff) ;
        }

        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
        {
            effect.SetAttribute("Type", "MultiDiagonalSquaresInOutTransitionEffect");

            base.Save(parent, doc, effect);
        }


    }
}
