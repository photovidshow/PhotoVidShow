using System;
using DVDSlideshow.Memento;
using System.Xml;
using System.Collections;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for CProjectMemento.
	/// </summary>
	public class CProjectMemento :  Memento.Memento
	{
		
		private XmlDocument	mDoc;

		public XmlDocument Doc
		{
			get { return mDoc; }
		}
		
		public CProjectMemento(IOriginator org, XmlDocument doc, ArrayList handlers) :
			 base(org,handlers)
		{
			mDoc=doc;
		}
	}
}
