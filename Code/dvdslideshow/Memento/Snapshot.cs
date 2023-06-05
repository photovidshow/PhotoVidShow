using System;
using System.Collections;

namespace DVDSlideshow.Memento
{
	/// <summary>
	/// Summary description for State.
	/// </summary>
	public class Snapshot
	{
		public string mDescription;	// what happened to make us take this snapshot
		public ArrayList	mMementos = new ArrayList();

		public Snapshot(string s)
		{
			mDescription = s;
		}

		public void AddMemento(Memento m)
		{
			mMementos.Add(m);
		}
	}
}
