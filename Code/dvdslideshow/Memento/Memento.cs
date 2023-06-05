using System;
using System.Collections;
using System.Xml;

namespace DVDSlideshow.Memento
{
	/// <summary>
	/// Summary description for State.
	/// </summary>
	/// 
	
	public class Memento
	{
		private ArrayList mHandlers;
		private IOriginator mOriginator;

		public ArrayList Handlers
		{
			get { return mHandlers; }
		}

		public IOriginator Originator
		{
			get { return mOriginator; }
		}

		public Memento(IOriginator org, ArrayList handlers)
		{
			mOriginator =org;
			mHandlers = handlers;
		}
	}
}
