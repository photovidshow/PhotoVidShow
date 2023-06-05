using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace DVDSlideshow
{
    public class CQuickSlow : CEquation
    {

        public static string EquationString()
        {
            return "QuickSlow";
        }

        public override float Get(float start, float end, float delta)
        {
            float total = end - start;

            float quickTime = 0.05f;
            float quickMovement = 0.92f;

            if (delta < quickTime)
            {
                delta *= (1 / quickTime);
                delta *= quickMovement;

                if (delta > 1) delta = 1;
            }
            else
            {
                float remainTime = 1.0f - quickTime;
                float remainMovement = 1.0f - quickMovement;

                delta = delta - quickTime;

                float v = 1 / remainTime;
                delta *= v;

                delta *= remainMovement; 
                delta += quickMovement;
                if (delta > 1) delta=1;
            }

            return ((end - start) * delta) + start;
        }


        public override void Save(XmlElement parent, XmlDocument doc)
        {
            XmlElement equation = doc.CreateElement("Equation");

            equation.SetAttribute("Type", "QuickSlow");

            parent.AppendChild(equation);
        }

        public override void Load(XmlElement element)
        {

        }
    }
}
