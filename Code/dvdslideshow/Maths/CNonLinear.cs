using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace DVDSlideshow
{
    public class CNonLinear : CEquation
    {

        public static string EquationString()
        {
            return "NonLinear";
        }

        public override float Get(float start, float end, float delta)
        {

            delta = (float)Math.Cos(((double)delta) * Math.PI);
            delta *= 0.5f;
            delta += 0.5f;

            // just in case
            if (delta < 0.0f) delta = 0.0f;
            if (delta > 1.0f) delta = 1.0f;

            delta = 1.0f - delta;

            return ((end - start) * delta) + start;
        }

        public override void Save(XmlElement parent, XmlDocument doc)
        {
            XmlElement equation = doc.CreateElement("Equation");

            equation.SetAttribute("Type", "NonLinear");

            parent.AppendChild(equation);
        }

        public override void Load(XmlElement element)
        {

        }
    }
}
