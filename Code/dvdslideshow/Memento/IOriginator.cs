using System;

namespace DVDSlideshow.Memento
{
	/// <summary>
	/// Summary description for IOriginator.
	/// </summary>
	/// 


	public interface IOriginator
	{	
		void SetMemento(Memento m);
		Memento CreateMemento();
	}
}
