using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml;

namespace DVDSlideshow
{
    public class CMenuSubPictureStyle
    {
        public static Color DefaultSubPictureColor = Color.FromArgb(128, 255, 255, 128);

        private CMenuSubPictureRenderMethod mMenuSubPictureButtonStyle = CMenuSubPictureRenderMethod.HighlightImage;
        private CMenuSubPictureRenderMethod mMenuSubPictureMenuLinkStyle = CMenuSubPictureRenderMethod.HighlightImage;
        private CMenuSubPictureRenderMethod mMenuSubPicturePlayMethodsStyle = CMenuSubPictureRenderMethod.Underline;
        private int mMenuSubPictureSelectIconIndex = 0;        // addtional images used for above 
        private Color mSubPictureColor = DefaultSubPictureColor;

        public CMenuSubPictureRenderMethod SubPictureButtonStyle
        {
            get { return mMenuSubPictureButtonStyle; }
            set { mMenuSubPictureButtonStyle = value; }
        }

        public CMenuSubPictureRenderMethod SubPictureMenuLinkStyle
        {
            get { return mMenuSubPictureMenuLinkStyle; }
            set { mMenuSubPictureMenuLinkStyle = value; }
        }


        public CMenuSubPictureRenderMethod SubPicturePlayMethodsStyle
        {
            get { return mMenuSubPicturePlayMethodsStyle; }
            set { mMenuSubPicturePlayMethodsStyle = value; }
        }

        public int SubPictureSelectIconIndex
        {
            get { return mMenuSubPictureSelectIconIndex; }
            set { mMenuSubPictureSelectIconIndex = value; }
        }

        public Color SubPictureColor
        {
            get { return mSubPictureColor; }
            set { mSubPictureColor = value; }
        }


        //*******************************************************************
        public void Save(XmlElement parent, XmlDocument doc)
        {
            XmlElement subPictureElement = doc.CreateElement("SubPictureStyle");

            subPictureElement.SetAttribute("ButtonStyle", ((int)mMenuSubPictureButtonStyle).ToString());
            subPictureElement.SetAttribute("MenuLinkStyle", ((int)mMenuSubPictureMenuLinkStyle).ToString());
            subPictureElement.SetAttribute("PlayButtonsStyle", ((int)mMenuSubPicturePlayMethodsStyle).ToString());
            subPictureElement.SetAttribute("Color", mSubPictureColor.ToString());
            if (mMenuSubPictureSelectIconIndex != 0)
            {
                subPictureElement.SetAttribute("SelectIcon", mMenuSubPictureSelectIconIndex.ToString());
            }
      
            parent.AppendChild(subPictureElement);
        }

        //*******************************************************************
        public void Load(XmlElement element)
        {
            string s = element.GetAttribute("ButtonStyle");
            if (s!="")
            {
                mMenuSubPictureButtonStyle = (CMenuSubPictureRenderMethod)int.Parse(s);
            }
            s = element.GetAttribute("MenuLinkStyle");
            if (s != "")
            {
                mMenuSubPictureMenuLinkStyle = (CMenuSubPictureRenderMethod)int.Parse(s);
            }

            s = element.GetAttribute("PlayButtonsStyle");
            if (s != "")
            {
                mMenuSubPicturePlayMethodsStyle = (CMenuSubPictureRenderMethod)int.Parse(s);
            }

            s = element.GetAttribute("Color");
            if (s != "")
            {
                mSubPictureColor = CGlobals.ParseColor(s);
            }

            s = element.GetAttribute("SelectIcon");
            if (s != "")
            {
                mMenuSubPictureSelectIconIndex = int.Parse(s);
            }
        }
    }
}
