using System;
using System.Drawing;
using System.Xml;
using DVDSlideshow.GraphicsEng;


namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for CMenuLinkButton.
    /// This class is now split into child link menu buttons
	/// </summary>
	public abstract class CMenuLinkButton : CMenuButton
	{
        private int mMenuLinkID = -1;

        public int MenuLinkID
        {
            get { return mMenuLinkID; }
            set { mMenuLinkID = value; }
        }

        //*******************************************************************
        public CMenuLinkButton()
        { 
        }

        //*******************************************************************
        public CMenuLinkButton(CImage frame, RectangleF coverage, int link_menu_id) : base(frame, coverage) 
        {
            mMenuLinkID = link_menu_id;
        }

        //*******************************************************************
        public void LoadMenuLinkButtonPart(XmlElement element)
        {
            LoadMenuButtonPart(element);

            string s1 = element.GetAttribute("MenuLinkID");
            if (s1 != "")
            {
                mMenuLinkID = int.Parse(s1);
            }
        }

        //*******************************************************************
        public void SaveMenuLinkButtonPart(XmlElement decoration, XmlDocument doc)
        {
            SaveMenuButtonPart(decoration, doc);

            decoration.SetAttribute("MenuLinkID", this.mMenuLinkID.ToString());
		
        }

	}
}
