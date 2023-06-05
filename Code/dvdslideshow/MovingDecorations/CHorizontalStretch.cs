using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml;

namespace DVDSlideshow
{
    public class CHorizontalStretch : CStretch
    {

        public CHorizontalStretch()
        {
        }

        public CHorizontalStretch(float startMultiplier, float endMultiplier)
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
            return "Horizontal Stretch";
        }

        public override RectangleF ApplyStretch(RectangleF initialRectangle, float delta)
        {
            float delay_delta = GetDeltaAferDelay(delta);
            float multiplier = equation.Get(mStartMultiply, mEndMultiply, delay_delta);

            if (multiplier <= 0)
            {
                return new RectangleF(-10, -10, 0.0001f, 0.0001f);
            }

            float new_width = initialRectangle.Width * multiplier;

            float old_middle = initialRectangle.X + (initialRectangle.Width / 2.0f);

            return new RectangleF(old_middle - (new_width / 2),
                                  initialRectangle.Y,
                                  new_width,
                                  initialRectangle.Height);
        }

        public override void Save(XmlElement parent, XmlDocument doc)
        {
            if (mStartMultiply == 1 && mEndMultiply == 1)
            {
                return;
            }

            XmlElement stretch = doc.CreateElement("Stretch");

            stretch.SetAttribute("Type", "HorizontalStretch");

            SaveStretchPart(stretch, doc);

            parent.AppendChild(stretch);
        }

        public override void Load(XmlElement element)
        {
            LoadStretchPart(element);
        }
    }
}
