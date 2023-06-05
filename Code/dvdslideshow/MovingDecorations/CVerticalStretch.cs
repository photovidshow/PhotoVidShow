using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml;

namespace DVDSlideshow
{
    public class CVerticalStretch : CStretch
    {
        public CVerticalStretch()
        {
        }

        public CVerticalStretch(float startMultiplier, float endMultiplier)
        {
            mStartMultiply = startMultiplier;
            mEndMultiply = endMultiplier;
        }

        public override string ToStretchString()
        {
            return StretchString();
        }

        public static string StretchString()
        {
            return "Vertical Stretch";
        }

        public override RectangleF ApplyStretch(RectangleF initialRectangle, float delta)
        {
            float delay_delta = GetDeltaAferDelay(delta);
            float multiplier = equation.Get(mStartMultiply, mEndMultiply, delay_delta);
  
            if (multiplier <= 0)
            {
                return new RectangleF(-10, -10, 0.0001f, 0.0001f);
            }

            float new_height = initialRectangle.Height * multiplier;
            float old_middle = initialRectangle.Y + (initialRectangle.Height / 2.0f);

            return new RectangleF(initialRectangle.X,
                                  old_middle - (new_height / 2),
                                  initialRectangle.Width,
                                  new_height);
        }

        public override void Save(XmlElement parent, XmlDocument doc)
        {
            if (mStartMultiply == 1 && mEndMultiply == 1)
            {
                return;
            }

            XmlElement stretch = doc.CreateElement("Stretch");

            stretch.SetAttribute("Type", "VerticalStretch");

            SaveStretchPart(stretch, doc);

            parent.AppendChild(stretch);
        }

        public override void Load(XmlElement element)
        {   
            LoadStretchPart(element);
        }
    }
}
