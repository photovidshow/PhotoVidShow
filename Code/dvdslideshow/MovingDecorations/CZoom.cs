using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml;

namespace DVDSlideshow
{
    public class CZoom : CDeltaDelayable
    {  
        private float startZoomRat = 1.0f;
        private float endZoomRat = 1.0f;

        public float StartZoom
        {
            get { return startZoomRat; }
            set { startZoomRat = value; }
        }

        protected CEquation equation = new CNonLinear();

        protected bool oneOverZ = true;

        public bool OneOverZ
        {
            get { return oneOverZ; }
            set { oneOverZ = value; }
        }

        public CEquation Equation
        {
            get { return equation; }
            set { equation = value; }
        }

        public float EndZoom
        {
            get { return endZoomRat; }
            set { endZoomRat = value; }
        }

        public CZoom()
        {
        }

        public CZoom(float start_rat, float end_rat)
        {
            startZoomRat = start_rat;
            endZoomRat = end_rat;
        }

        public RectangleF GetZoom(RectangleF current, float delta)
        {
            float delay_delta = GetDeltaAferDelay(delta);

            delay_delta = equation.Get(0, 1, delay_delta);

            RectangleF old = current;

            float zoom = 1;

            if (oneOverZ)
            {
                float start_Z = 1 / startZoomRat;
                float end_z = 1 / endZoomRat;

                float one_over_zoom = ((end_z - start_Z) * delay_delta) + start_Z;

                zoom = 1 / one_over_zoom;
            }
            else
            {
                zoom = ((endZoomRat - startZoomRat) * delay_delta) + startZoomRat;
            }

            current.X -= ( ((current.Width * zoom) - current.Width ) / 2);
            current.Y -= ( ((current.Height * zoom) - current.Height ) / 2);
            current.Width *= zoom;
            current.Height *=zoom;

            return current;
        }

        public bool IsStatic()
        {
            if (startZoomRat != endZoomRat)
            {
                return false;
            }

            return true;
        }


        public void Save(XmlElement parent, XmlDocument doc)
        {
            if (endZoomRat == 1 && startZoomRat == 1) return;

            XmlElement zoom = doc.CreateElement("Zoom");

            SaveDeltaDelayPart(zoom, doc);

            zoom.SetAttribute("StartZoom", startZoomRat.ToString());
            zoom.SetAttribute("EndZoom", endZoomRat.ToString());

            if (oneOverZ == false)
            {
                zoom.SetAttribute("OneOverZ", oneOverZ.ToString());
            }

            equation.Save(zoom, doc);

            parent.AppendChild(zoom);
        }

        public void Load(XmlElement element)
        {
            LoadDeltaDelayPart(element);

            string s = element.GetAttribute("StartZoom");
            if (s != "")
            {
                startZoomRat = float.Parse(s);
            }

            s = element.GetAttribute("EndZoom");
            if (s != "")
            {
                endZoomRat = float.Parse(s);
            }

            s = element.GetAttribute("OneOverZ");
            if (s != "")
            {
                oneOverZ = bool.Parse(s);
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
