#region Copyright Drew Noakes 2003

/* This class is released into the public domain.  It may be freely used,
 * provided this banner is kept intact.  Please report any bugs, share any
 * modifications and provide feedback to the author.
 * 
 * drew@drewnoakes.com
 * http://drewnoakes.com/
 */

#endregion

using System;

namespace DVDSlideshow
{
	/// <summary>
	/// A general implementation of a stopwatch.  This implementation uses the <see cref="DateTime.Now"/>
	/// method to record relative points in time, and calculate elapsed periods.  This gives approximately
	/// 15ms resolution, which will be adequate for many uses.  For a more precise measuring tool, use
	/// <see cref="PrecisionStopWatch"/>, which has sub-millisecond precision.  This class depends only
	/// upon core framework classes, unlike <see cref="PrecisionStopWatch"/> which uses Interop calls to 
	/// native platform libraries.
	/// </summary>
	public class StopWatch : IStopWatch
	{
		#region Field declarations

		TimeSpan _carriedOverTime;
		DateTime _currentRunStartTime;
		bool _running = false;

		#endregion

		#region Constructor

		public StopWatch()
		{
			Reset();
		}


		#endregion
		
		#region Public methods & Properties
		
		public void Start()
		{
			if (!_running)
			{
				_running = true;
				StartTimingRun();
			}
		}

		public void Stop()
		{
			_carriedOverTime = Time;
			_running = false;
		}

		public void Reset()
		{
			_carriedOverTime = TimeSpan.Zero;
			StartTimingRun();
		}

		public void Set(long val)
		{
			_carriedOverTime = new TimeSpan(val*10000);
			StartTimingRun();
		
		}

		public bool IsRunning
		{
			get
			{
				return _running;
			}
		}

		public TimeSpan Time
		{
			get
			{
				if (_running)
				{
					return _carriedOverTime.Add(DateTime.Now - _currentRunStartTime);
				}
				return _carriedOverTime;
			}
		}


		#endregion

		#region Private helper methods

		void StartTimingRun()
		{
			_currentRunStartTime = DateTime.Now;
		}


		#endregion
	}
}
