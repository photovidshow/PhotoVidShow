using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml;

namespace DVDSlideshow
{
    public class CDeltaDelayable
    {
        protected float initialDelay = 0;
        protected float endDelay = 0;

        public float InitialDelay
        {
            get { return initialDelay; }
            set { initialDelay = value; }
        }

        public float EndDelay
        {
            get { return endDelay; }
            set { endDelay = value; }
        }

        protected float GetDeltaAferDelay(float delta)
        {
            if (delta < initialDelay) return 0;
            if (delta > 1.0 - endDelay) return 1;
            if (initialDelay + endDelay >= 1) return 1;

            delta -= initialDelay;
            delta *= (1.0f / (1.0f - initialDelay - endDelay));
            if (delta < 0) delta = 0;
            if (delta > 1) delta = 1;
            return delta;
        }

        protected void SaveDeltaDelayPart(XmlElement element, XmlDocument doc)
        {
            element.SetAttribute("InitialDelay", initialDelay.ToString());
            element.SetAttribute("EndDelay", endDelay.ToString());
        }

        protected void LoadDeltaDelayPart(XmlElement element)
        {
            string s1 = element.GetAttribute("InitialDelay");

            if (s1 != "")
            {
                initialDelay = float.Parse(s1);
            }

            string s = element.GetAttribute("EndDelay");
            if (s != "")
            {
                endDelay = float.Parse(s);
            }
        }  
    }
}
