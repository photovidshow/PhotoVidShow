using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml;
using ManagedCore;

namespace DVDSlideshow
{
    public abstract class CMovement : CDeltaDelayable
    {
        public static CMovement CreateFromType(string type)
        {
            if (type == "StraightLineMovement") return new CStraightLineMovement();

            CDebugLog.GetInstance().Error("Trying to create unknown movement type:" + type);

            return new CStraightLineMovement();
        }

        protected CEquation equation = new CLinear();

        public abstract RectangleF GetPosition(RectangleF initialRec, float delta);

        public CEquation Equation
        {
            get { return equation; }
            set { equation = value; }
        }

        public abstract string ToMovementString();

		public abstract void Save(XmlElement parent, XmlDocument doc);
        public abstract void Load(XmlElement element);

        public abstract bool IsStatic();

        protected void SaveMovementPart(XmlElement element, XmlDocument doc)
        {
            equation.Save(element, doc);

            SaveDeltaDelayPart(element, doc);
        }

        protected void LoadMovementPart(XmlElement element)
        {
            LoadDeltaDelayPart(element);

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
