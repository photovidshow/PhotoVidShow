using System;
using System.Drawing;
using System.Xml;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
	/// <summary>
    /// Summary description for CMenuLinkPreviousNextButton.
	/// </summary>
	public class CMenuLinkPreviousNextButton : CMenuLinkButton
	{
		public enum LinkType
		{
			NEXT_MENU,
			PREVIOUS_MENU
		}

		private LinkType mLinkType;

		public LinkType Link
		{
			get { return mLinkType; }
		}

        //*******************************************************************
        public CMenuLinkPreviousNextButton() 
		{
		}

        //*******************************************************************
        public CMenuLinkPreviousNextButton(LinkType link_type, CImage frame, RectangleF coverage, int menu_link_id)
            :
            base(frame, coverage, menu_link_id)
		{
			mLinkType = link_type;
			//
			// TODO: Add constructor logic here
			//
		}

		//*******************************************************************
		public override void Save(XmlElement parent, XmlDocument doc)
		{
			XmlElement decoration = doc.CreateElement("Decoration");
			decoration.SetAttribute("Type","ButtonLinkNextPreviousDecoration");
			decoration.SetAttribute("LinkType",((int)this.mLinkType).ToString());
			SaveMenuLinkButtonPart(decoration, doc);
			parent.AppendChild(decoration); 
		}

		//*******************************************************************
		public override void Load(XmlElement element)
		{
			this.mLinkType = (LinkType) int.Parse(element.GetAttribute("LinkType"));

            LoadMenuLinkButtonPart(element);
		}
	}
}
