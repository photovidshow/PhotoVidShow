using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml;
using System.Globalization;
using ManagedCore;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for CTextStyle.
	/// </summary>
	public class CTextStyle
	{
        public enum ShadowType
        {
            Solid,
            Diffuse,
            Blur
        };

        private StringFormat mStringFormat = new StringFormat();
        private string mFontName = "Verdana";
        private bool mBold = false;
        private bool mShadow = false;
        private ShadowType mShadowStyle = ShadowType.Solid;
        private bool mOutline = false;
        private bool mItalic = false;

        // this is nasty... this font size value could mean 1 of 3 things
        // 1) if width and height are 0 on the decor, we calc coverage area from this value against the global font canvas size
        // 2) this may contain the font size as from the global font canvis size (and coverage is set)
        // 3) this may contain the font size as from the last canvas we rendered with (and coverage is set)
        private float mFontSize = 16.0f;
        private Color mColor = Color.White;
        private Color mColor2 = Color.White;            // only used if mGradient is true
        private Color mShadowColor = Color.Black;
        private bool mGradient = false;
        private int mShadowDiffuseLength = 8;
        private int mShadowAlpha = 192;
        private float mShadowOffsetX = 2;
        private float mShadowOffsetY = 2;
        private int mOutlineLength = 1;
        private Color mOutlineColor = Color.Black;
        private int mOutlineAlpha = 255;
        private bool mUnderLine = false;


        private string mName = "";

        private static CTextStyle defaultStyle = new CTextStyle();

        //*******************************************************************
        public CTextStyle Clone()
        {
            CTextStyle d = new CTextStyle();

            d.mStringFormat = (StringFormat)this.mStringFormat.Clone();
            d.mFontSize = mFontSize;

            CopyComparablePartsToStyle(d);

            return d;
        }

        //*******************************************************************
        private void CopyComparablePartsToStyle(CTextStyle d)
        {
            d.mFontName = (string)mFontName.Clone();
            d.mBold = mBold;
            d.mShadow = mShadow;
            d.mShadowStyle = mShadowStyle;
            d.mOutline = mOutline;
            d.mItalic = mItalic;
            d.mColor = mColor;
            d.mColor2 = mColor2;
            d.mShadowColor = mShadowColor;
            d.mGradient = mGradient;
            d.mShadowDiffuseLength = mShadowDiffuseLength;
            d.mShadowAlpha = mShadowAlpha;
            d.mShadowOffsetX = mShadowOffsetX;
            d.mShadowOffsetY = mShadowOffsetY;
            d.mOutlineLength = mOutlineLength;
            d.mOutlineColor = mOutlineColor;
            d.mOutlineAlpha = mOutlineAlpha;
            d.mUnderLine = mUnderLine;
        }

        //*******************************************************************
        public bool IsSameAs(CTextStyle otherStyle)
        {
            // ignore name, justification and size

            if (mFontName != otherStyle.mFontName) return false;
            if (mBold != otherStyle.mBold) return false;
            if (mShadow != otherStyle.mShadow) return false;
            if (mShadowStyle != otherStyle.mShadowStyle) return false;
            if (mOutline != otherStyle.mOutline) return false;
            if (mItalic != otherStyle.mItalic) return false;
            if (mColor != otherStyle.mColor) return false;
            if (mColor2 != otherStyle.mColor2) return false;
            if (mShadowColor != otherStyle.mShadowColor) return false;
            if (mGradient != otherStyle.mGradient) return false;
            if (mShadowDiffuseLength != otherStyle.mShadowDiffuseLength) return false;
            if (mShadowAlpha != otherStyle.mShadowAlpha) return false;
            if (mShadowOffsetX != otherStyle.mShadowOffsetX) return false;
            if (mShadowOffsetY != otherStyle.mShadowOffsetY) return false;
            if (mOutlineLength != otherStyle.mOutlineLength) return false;
            if (mOutlineColor != otherStyle.mOutlineColor) return false;
            if (mOutlineAlpha != otherStyle.mOutlineAlpha) return false;
            if (mUnderLine != otherStyle.mUnderLine) return false;

            return true;
        }


        //*******************************************************************
        public ShadowType ShadowStyle
        {
            get { return mShadowStyle; }
            set { mShadowStyle = value; }
        }


        //*******************************************************************
        public int OutlineLength
        {
            get { return mOutlineLength; }
            set { mOutlineLength = value; }
        }


        //*******************************************************************
        public Color OutlineColor
        {
            get { return mOutlineColor; }
            set { mOutlineColor = value; }
        }


        //*******************************************************************
        public int OutlineAlpha
        {
            get { return mOutlineAlpha; }
            set { mOutlineAlpha = value; }
        }

        //*******************************************************************
        public float ShadowOffsetX
        {
            get { return mShadowOffsetX; }
            set { mShadowOffsetX = value; }
        }

        //*******************************************************************
        public float ShadowOffsetY
        {
            get { return mShadowOffsetY; }
            set { mShadowOffsetY = value; }
        }

        //*******************************************************************
        public int ShadowDiffuseLength
        {
            get { return mShadowDiffuseLength; }
            set { mShadowDiffuseLength = value; }
        }


        //*******************************************************************
        public int ShadowAlpha
        {
            get { return mShadowAlpha; }
            set { mShadowAlpha = value; }
        }


        //*******************************************************************
        public bool Italic
        {
            get { return mItalic; }
            set { mItalic = value; }
        }

        //*******************************************************************
        public bool UnderLine
        {
            get { return mUnderLine; }
            set { mUnderLine = value; }
        }


        //*******************************************************************
        public Color TextColor
        {
            get { return mColor; }
            set { mColor = value; }
        }

        //*******************************************************************
        public Color TextColor2
        {
            get { return mColor2; }
            set { mColor2 = value; }
        }


        //*******************************************************************
        public bool Gradient
        {
            get { return mGradient; }
            set { mGradient = value; }
        }

        //*******************************************************************
        public Color ShadowColor
        {
            get { return mShadowColor; }
            set { mShadowColor = value; }
        }

        //*******************************************************************
        public StringFormat Format
        {
            get { return mStringFormat; }
            set { mStringFormat = value; }
        }

        //*******************************************************************
        public string FontName
        {
            get { return mFontName; }
            set { mFontName = value; }
        }

        //*******************************************************************
        public bool Bold
        {
            get { return mBold; }
            set { mBold = value; }
        }

        //*******************************************************************
        public bool Shadow
        {
            get { return mShadow; }
            set { mShadow = value; }
        }

        //*******************************************************************
        public bool Outline
        {
            get { return mOutline; }
            set { mOutline = value; }
        }

        //*******************************************************************
        public float FontSize
        {
            get { return mFontSize; }
            set { mFontSize = value; }
        }

        //*******************************************************************
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        //*******************************************************************
        public float GetShadowLength()
        {
            return (float)Math.Sqrt((mShadowOffsetX * mShadowOffsetX) + (mShadowOffsetY * mShadowOffsetY));
        }

        //*******************************************************************
        public float GetShadowAngle()
        {
            double angle = Math.Atan2(mShadowOffsetX, mShadowOffsetY);
            angle /= 0.0174532925;
            if (angle < 0) angle += 360;
            return (float) angle;

        }

        //*******************************************************************
        public void SetShadowLengthAndAndAngle(float length, float angleDegrees)
        {
            double rad = angleDegrees * 0.0174532925;
            mShadowOffsetX = (float) Math.Sin(rad) * length;
            mShadowOffsetY = (float) Math.Cos(rad) * length;
        }


        //*******************************************************************
        public CTextStyle()
        {
            mStringFormat.Alignment = StringAlignment.Near;
        }


		//*******************************************************************
		public void Save(XmlElement parent, XmlDocument doc)
        {
            XmlElement TextStyle = doc.CreateElement("TextStyle");


            // if pre-defined style save that out

            string name = CTextStyleDatabase.GetInstance().GetMatchingStyleName(this);

            if (name !="")
            {
                TextStyle.SetAttribute("Name", name);
            }
         
            if (mFontName != defaultStyle.FontName)
            {
                TextStyle.SetAttribute("FontName", mFontName);
            }

            if (mFontSize != defaultStyle.FontSize)
            {
                TextStyle.SetAttribute("FontSize", mFontSize.ToString());
            }

            if (mColor != defaultStyle.TextColor)
            {
                TextStyle.SetAttribute("FontColor", mColor.ToString());
            }

            if (mBold != defaultStyle.Bold)
            {
                TextStyle.SetAttribute("Bold", mBold.ToString());
            }

            if (mUnderLine != defaultStyle.UnderLine)
            {
                TextStyle.SetAttribute("Underline", mUnderLine.ToString());
            }

            if (mShadow != defaultStyle.Shadow)
            {
                TextStyle.SetAttribute("Shadow", mShadow.ToString());
                TextStyle.SetAttribute("ShadowColor", mShadowColor.ToString());
            }

            if (mOutline != defaultStyle.Outline)
            {
                TextStyle.SetAttribute("OutLine", mOutline.ToString());
                TextStyle.SetAttribute("OutLineColour", mOutlineColor.ToString());
            }

            if (mItalic != defaultStyle.Italic)
            {
                TextStyle.SetAttribute("Italic", mItalic.ToString());
            }

            if (mStringFormat.Alignment != defaultStyle.Format.Alignment)
            {
                TextStyle.SetAttribute("Alignment", mStringFormat.Alignment.ToString());
            }

            if (mColor2 != defaultStyle.TextColor2)
            {
                TextStyle.SetAttribute("FontColor2", mColor2.ToString());
            }

            if (mGradient != defaultStyle.Gradient)
            {
                TextStyle.SetAttribute("Gradient", mGradient.ToString());
            }

            if (mShadowDiffuseLength != defaultStyle.ShadowDiffuseLength)
            {
                TextStyle.SetAttribute("ShadowDiffuseLength", mShadowDiffuseLength.ToString());
            }

            if (mShadowAlpha != defaultStyle.ShadowAlpha)
            {
                TextStyle.SetAttribute("ShadowAlpha", mShadowAlpha.ToString());
            }

            if (mShadowOffsetX != defaultStyle.ShadowOffsetX)
            {
                TextStyle.SetAttribute("ShadowOffsetX", mShadowOffsetX.ToString());
            }

            if (mShadowOffsetY != defaultStyle.ShadowOffsetY)
            {
                TextStyle.SetAttribute("ShadowOffsetY", mShadowOffsetY.ToString());
            }

            if (mOutlineLength != defaultStyle.OutlineLength)
            {
                TextStyle.SetAttribute("OutlineLength", mOutlineLength.ToString());
            }

            if (mOutlineAlpha != defaultStyle.OutlineAlpha)
            {
                TextStyle.SetAttribute("OutlineAlpha", mOutlineAlpha.ToString());
            }


            parent.AppendChild(TextStyle);
        }


		//*******************************************************************
		public void Load(XmlElement element)
		{
            string s = element.GetAttribute("FontSize");
            if (s != "")
            {
                mFontSize = float.Parse(s, CultureInfo.InvariantCulture);

                // adjust font size according to DPI
                if (Math.Abs(CGlobals.dpiX - 96.0) > 0.01)
                {
                    mFontSize = mFontSize * (96.0f / CGlobals.dpiX);
                }
            }

            s = element.GetAttribute("Alignment");
            if (s != "")
            {
                mStringFormat.Alignment = CGlobals.ParseAlignment(s);
            }


            s = element.GetAttribute("Name");
            if (s != "")
            {
                CTextStyle ts = CTextStyleDatabase.GetInstance().GetStyleFromName(s);
                if (ts == null)
                {
                    Log.Warning("Text style '" + s + "' not found, ignoring");
                }
                else
                {
                    ts.CopyComparablePartsToStyle(this);
                    return;
                }
            }

            s = element.GetAttribute("FontName");
            if (s != "")
            {
                mFontName = s;
            }

            s = element.GetAttribute("Font");   // legacy
            if (s != "")
            {
                mFontName = s;
            }

            s = element.GetAttribute("FontColor");
            if (s != "")
            {
                mColor = CGlobals.ParseColor(s);
            }
            s = element.GetAttribute("Color");      // legacy 
            if (s != "")
            {
                mColor = CGlobals.ParseColor(s);
            }

            s = element.GetAttribute("Bold");
            if (s != "")
            {
                mBold = bool.Parse(s);
            }

            s = element.GetAttribute("Underline");
            if (s != "")
            {
                mUnderLine = bool.Parse(s);
            }

            s = element.GetAttribute("Shadow");
            if (s != "")
            {
                mShadow = bool.Parse(s);
            }

            s = element.GetAttribute("ShadowColor");
            if (s != "")
            {
                mShadowColor = CGlobals.ParseColor(s);
            }

            s = element.GetAttribute("OutLine");
            if (s != "")
            {
                mOutline = bool.Parse(s);
            }
            s = element.GetAttribute("Outline");  // legacy
            if (s != "")
            {
                mOutline = bool.Parse(s);
            }

            s = element.GetAttribute("OutLineColour");  // legacy
            if (s != "")
            {
                this.mOutlineColor = CGlobals.ParseColor(s);
            }

            s = element.GetAttribute("Italic");
            if (s != "")
            {
                mItalic = bool.Parse(s);
            }

            s = element.GetAttribute("FontColor2");
            if (s != "")
            {
                mColor2 = CGlobals.ParseColor(s);
            }

            s = element.GetAttribute("Gradient");
            if (s != "")
            {
                mGradient = bool.Parse(s);
            }

            s = element.GetAttribute("ShadowDiffuseLength");
            if (s != "")
            {
                mShadowDiffuseLength = int.Parse(s);
            }

            s = element.GetAttribute("ShadowAlpha");
            if (s != "")
            {
                ShadowAlpha = int.Parse(s);
            }

            s = element.GetAttribute("ShadowOffsetX");
            if (s != "")
            {
                mShadowOffsetX = float.Parse(s);
            }

            s = element.GetAttribute("ShadowOffsetY");
            if (s != "")
            {
                mShadowOffsetY = float.Parse(s);
            }

            s = element.GetAttribute("OutlineLength");
            if (s != "")
            {
                mOutlineLength = int.Parse(s);
            }

            s = element.GetAttribute("OutlineAlpha");
            if (s != "")
            {
                mOutlineAlpha = int.Parse(s);
            }

       

            CTextStyleDatabase.GetInstance().AddStyleIfNotAlreadyInDatabase(this);
		}

	}
}
