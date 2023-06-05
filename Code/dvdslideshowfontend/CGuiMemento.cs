using System;
using DVDSlideshow;
using DVDSlideshow.Memento;
using System.Xml;
using System.Collections;

namespace dvdslideshowfontend
{
	/// <summary>
	/// Summary description for CGuiMemento.
	/// </summary>
	public class CGuiMemento : Memento
	{
		public bool mShowingMainMenu=true;
		public int mMenuIndex=0;
		public int mSlideShowIndex=0;
		public int mSlideIndex=0;
		public bool mSelectedDecoration=false;
		public int mDecorationIndex=0;

		public int mTabControl1Index=0;
		public int mTanControl2Index=0;

		public CGuiMemento(IOriginator org, ArrayList handlers) :
			base(org,handlers)
		{
			
		}
	}
}
