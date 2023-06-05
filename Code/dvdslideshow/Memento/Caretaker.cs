using System;
using System.Xml;
using System.Collections;
using ManagedCore;

namespace DVDSlideshow.Memento
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	/// 
	
	public class Caretaker
	{		
		private Stack mSnapshots = new Stack();
		private int mCurrentIndex = 0;

		public Stack SnapshotStack
		{
			get
			{
				return mSnapshots;
			}
		}

		public int CurrentStackIndex
		{
			get
			{
				return mCurrentIndex;
			}
		}


		public event StateChangeHandler StateChanged ;

		private Caretaker()
		{
		}

		private static Caretaker mInstance;

		public static Caretaker GetInstance()
		{
			if (mInstance==null)
			{
				mInstance= new Caretaker();
			}
			return mInstance;
		}
			

		public void Do(ArrayList originators, string description)
		{
			Snapshot s = new Snapshot(description);
			foreach (IOriginator o in originators)
			{
				s.AddMemento(o.CreateMemento());
			}

			// if not top remove items above us
			while(mSnapshots.Count >mCurrentIndex)
			{
				mSnapshots.Pop();
			}

			mSnapshots.Push(s);
			mCurrentIndex++;

			// no more than 20 items in stack.

            int max = 5;

			if (mCurrentIndex>=(max+1))
			{
		//		Console.WriteLine("Clearing memntos");
				Array the_array = mSnapshots.ToArray();

				mSnapshots.Clear();
				for (int i=(max-1);i>=0;i--)
				{
					mSnapshots.Push((Snapshot)the_array.GetValue(i));
				}
				mCurrentIndex=max;
			}

			if (StateChanged!=null) StateChanged();
		}

		public void Undo(object o)
		{
			ManagedCore.Progress.IProgressCallback pr = (ManagedCore.Progress.IProgressCallback) o;

			if (pr!=null)
			{
				pr.Begin(0,1);
				pr.SetText("Undoing...");
			}

			if (mCurrentIndex<2) 
			{
				if (pr!=null)
				{
					pr.End();
				}
				return ;
			}

			mCurrentIndex--;

			Snapshot s= mSnapshots.ToArray()[mSnapshots.Count-mCurrentIndex] as Snapshot;
			ApplySnapshot(s,pr);
			if (StateChanged!=null) StateChanged();
		}

		public void Redo()
		{
			if (mCurrentIndex>(mSnapshots.Count-1)) return ;

			mCurrentIndex++;
			Snapshot s= mSnapshots.ToArray()[mSnapshots.Count-mCurrentIndex] as Snapshot;
			ApplySnapshot(s,null);
			if (StateChanged!=null) StateChanged();
		}

		private void ApplySnapshot(Snapshot s,ManagedCore.Progress.IProgressCallback pr)
		{
			foreach (Memento m in s.mMementos)
			{
				m.Originator.SetMemento(m);
			}
		}

		public void Clear()
		{
			mSnapshots = new Stack();
			mCurrentIndex = 0;
		}




	}
}
