using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml;

namespace DVDSlideshow
{
    public class CRotation : CDeltaDelayable
    {
        private float start_rotation = 0;
        private float end_rotation = 0;
       
        public float StartRotation
        {
            get { return start_rotation; }
            set { start_rotation = value; }
        }

        public float EndRotation
        {
            get { return end_rotation; }
            set { end_rotation = value; }
        }

        private float xOffset = 0;
        private float yOffset = 0;

        public float XOffset
        {
            get { return xOffset; }
            set { xOffset = value; }
        }

        public float YOffset
        {
            get { return yOffset; }
            set { yOffset = value; }
        }

        protected CEquation equation = new CLinear();

        public CEquation Equation
        {
            get { return equation; }
            set { equation = value; }
        }

        public CRotation()
        {
        }

        public CRotation(float start_rot, float end_rot)
        {
            start_rotation = start_rot;
            end_rotation = end_rot;
        }

        public float GetRotation(float delta)
        {
            float delay_delta = GetDeltaAferDelay(delta);
            float amount = equation.Get(start_rotation, end_rotation, delay_delta);
            return amount;         
        }

        public bool IsStatic()
        {
            if (start_rotation != end_rotation)
            {
                return false;
            }

            return true;
        }


        public void Save(XmlElement parent, XmlDocument doc)
        {
            if (end_rotation == 0 && start_rotation == 0 && xOffset ==0 && yOffset==0 ) return;

            XmlElement rotation = doc.CreateElement("Rotation");

            equation.Save(rotation, doc);

            SaveDeltaDelayPart(rotation, doc);

            rotation.SetAttribute("StartRotation", start_rotation.ToString());
            rotation.SetAttribute("EndRotation", end_rotation.ToString());
            rotation.SetAttribute("XOffset", xOffset.ToString());
            rotation.SetAttribute("YOffset", yOffset.ToString());  
            parent.AppendChild(rotation);
        }

        public void Load(XmlElement element)
        {
            LoadDeltaDelayPart(element);

            string s = element.GetAttribute("StartRotation");
            if (s != "")
            {
                start_rotation = float.Parse(s);
            }

            s = element.GetAttribute("EndRotation");
            if (s != "")
            {
                end_rotation = float.Parse(s);
            }

            s = element.GetAttribute("XOffset");
            if (s != "")
            {
                xOffset = float.Parse(s);
            }

            s = element.GetAttribute("YOffset");
            if (s != "")
            {
                yOffset = float.Parse(s);
            }

            XmlNodeList list = element.GetElementsByTagName("Equation");
            if (list.Count > 0)
            {
                XmlElement e = list[0] as XmlElement;

                equation = CEquation.CreateFromType(e.GetAttribute("Type"));
                if (equation != null)
                {
                    equation.Load(e);
                }
            }
        }  
    }
}
