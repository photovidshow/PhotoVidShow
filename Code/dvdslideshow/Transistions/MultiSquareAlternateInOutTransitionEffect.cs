using System;
using System.Xml;
using System.Drawing.Imaging;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for MultiSquaresAlternateInOutTransitionEffect.
    /// </summary>
    public class MultiSquaresAlternateInOutTransitionEffect : MultiShapeInOutTransitionEffect
    {
        public MultiSquaresAlternateInOutTransitionEffect()
        {
        }

        public MultiSquaresAlternateInOutTransitionEffect(bool outEffect, float circlesW, float wRat, float hRat, float length, int spread)
            : base(outEffect, circlesW, wRat, hRat, length, spread)
        {
        }

        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            MultiSquaresAlternateInOutTransitionEffect t = new MultiSquaresAlternateInOutTransitionEffect(mOut, mCirclesW, mWrat, mHrat, mLength, mSpread);
            t.Index = this.Index;
            return t;
        }

        //*******************************************************************
        public override string GetIdString()
        {
            return "MultiSquaresAlternate" + base.GetIdString();
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
                return xdiff;
            }
            return ydiff;           
        }


        //******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
        {
            effect.SetAttribute("Type", "MultiSquaresAlternateInOutTransitionEffect");

            base.Save(parent, doc, effect);
        }

    }
}
