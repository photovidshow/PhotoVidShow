using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace DVDSlideshow
{
    public class CSpring : CEquation
    {
        private bool mInvert = false;

        public bool Inverted
        {
            get { return mInvert; }
            set { mInvert = value; }
        }

        public static string EquationString()
        {
            return "Spring";
        }

        public override float Get(float start, float end, float delta)
        {
            if (mInvert == true)
            {
                delta = 1 - delta;
            }

            float inv_delta = 1 - delta;

            float dist = inv_delta * inv_delta * inv_delta;

            delta *= (float)((Math.PI * 2) + (Math.PI / 2));
            float dd = (float)Math.Cos(((double)delta));

            dd *= dist;

            if (mInvert == true)
            {
                float temp_e = end;
                end = start;
                start = temp_e;
            }

            return ((start - end) * dd) + end;
        }


        public override void Save(XmlElement parent, XmlDocument doc)
        {
            XmlElement equation = doc.CreateElement("Equation");

            equation.SetAttribute("Type", "Spring");

            if (mInvert == true)
            {
                equation.SetAttribute("Inverted", mInvert.ToString());
            }

            parent.AppendChild(equation);
        }

        public override void Load(XmlElement element)
        {
            string s = element.GetAttribute("Inverted");
            if (s != "")
            {
                mInvert = bool.Parse(s);
            }
        }
    }
}
