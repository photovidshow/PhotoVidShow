using System;
using System.Xml;
using System.Drawing.Imaging;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for FadeUpTransitionEffect.
	/// </summary>
	public class MultiCirclesInOutTransitionEffect : MultiShapeInOutTransitionEffect
	{   
		public MultiCirclesInOutTransitionEffect()
		{
		}

		public MultiCirclesInOutTransitionEffect(bool outEffect, float circlesW, float wRat, float hRat, float length, int spread)
           : base (outEffect, circlesW, wRat, hRat, length, spread)
		{

		}

		//*******************************************************************
		public override CTransitionEffect Clone()
		{
            MultiCirclesInOutTransitionEffect t = new MultiCirclesInOutTransitionEffect(mOut, mCirclesW, mWrat, mHrat, mLength, mSpread);
			t.Index = this.Index;
			return t;
		}

		//*******************************************************************
        public override string GetIdString()
		{
            return "MultiCircles" + base.GetIdString();
		}


        //*******************************************************************
        public override double GetMaxPossibleValue(double width, double height)
        {
            double radius = Math.Sqrt(width * width + height * height) /2;

            return radius;
        }

        //*******************************************************************
        public override double GetDist(double xdiff, double ydiff)
        {
            double dist = Math.Sqrt(ydiff * ydiff + xdiff * xdiff);

            return dist;
        }

        //******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
        {
            effect.SetAttribute("Type", "MultiCirclesInOutTransitionEffect");

            base.Save(parent, doc, effect);
        }
	}
}
