using System;
using System.Collections;

namespace ManagedCore
{
	/// <summary>
	/// Summary description for SubtitleButtonPositions.
	/// </summary>
	public class SubtitleButtonPositions
	{
		public ArrayList x0 = new ArrayList();
		public ArrayList y0 = new ArrayList();

		public SubtitleButtonPositions()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public void AddPair(int x,int y)
		{
			x0.Add(x);
			y0.Add(y);
		}
	}
}
