using System;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for CVideoWritter.
	/// </summary>
	public abstract class CVideoWriter
	{
		// returns 0 if there was an general error 1 if sucess, any number above that is a particular error
        public abstract int BurnFromPath(bool do_orig);
        public virtual bool CreateIso() { return true; }
        public virtual bool CreatingIso() { return false; }
		public virtual void AbortBurn() {}
        public abstract int GetWriteSpeed();
		// how long da ya recond to burn this folder
        public abstract double GetBurnTimeEstimation();
  
	}
}
