using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace DVDSlideshow
{
    public class CLinear : CEquation
    {

        public static string EquationString()
        {
            return "Linear";
        }

        public override float Get(float start, float end, float delta)
        {
            return ((end - start) * delta) + start;
        }


        public override void Save(XmlElement parent, XmlDocument doc)
        {
            XmlElement equation = doc.CreateElement("Equation");

            equation.SetAttribute("Type", "Linear");

            parent.AppendChild(equation);
        }

        public override void Load(XmlElement element)
        {

        }
    }
}
