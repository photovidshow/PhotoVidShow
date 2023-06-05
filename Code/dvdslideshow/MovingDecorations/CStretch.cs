using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml;
using ManagedCore;

namespace DVDSlideshow
{
    public abstract class CStretch : CDeltaDelayable
    {
        public static CStretch CreateFromType(string type)
        {
            if (type == "VerticalStretch") return new CVerticalStretch();
            if (type == "HorizontalStretch") return new CHorizontalStretch();

            CDebugLog.GetInstance().Error("Trying to create unknown stretch type:" + type);

            return new CVerticalStretch();
        }

        public abstract RectangleF ApplyStretch(RectangleF initialRectangle, float delta);

        protected CEquation equation = new CLinear();

        protected float mStartMultiply = 1;
        protected float mEndMultiply = 1;

        public float StartMultiplier
        {
            get { return mStartMultiply; }
            set { mStartMultiply = value; }
        }

        public float EndMultiplier
        {
            get { return mEndMultiply; }
            set { mEndMultiply = value; }
        }

        public CEquation Equation
        {
            get { return equation; }
            set { equation = value; }
        }

        public abstract string ToStretchString();


        public bool IsStatic()
        {
            if (mStartMultiply != mEndMultiply)
            {
                return false;
            }
            return true;
        }

        public abstract void Save(XmlElement parent, XmlDocument doc);
        public abstract void Load(XmlElement element);

        protected void SaveStretchPart(XmlElement element, XmlDocument doc)
        {
            equation.Save(element, doc);

            element.SetAttribute("StartMultiplier", mStartMultiply.ToString());
            element.SetAttribute("EndMultiplier", mEndMultiply.ToString());
          
            SaveDeltaDelayPart(element, doc);
        }

        protected void LoadStretchPart(XmlElement element)
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

            string s1 = element.GetAttribute("StartMultiplier");
            if (s1 != "")
            {
                mStartMultiply = float.Parse(s1);
            }

            s1 = element.GetAttribute("EndMultiplier");
            if (s1 != "")
            {
                mEndMultiply = float.Parse(s1);
            }
        }  
    }
}
