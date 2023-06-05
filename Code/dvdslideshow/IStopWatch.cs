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
	/// Interface to a class that measures periods of time.
	/// </summary>
	public interface IStopWatch
	{
		/// <summary>
		/// Start the stopwatch.  This method must be called to start measuring time.
		/// </summary>
		void Start();

		/// <summary>
		/// Stops the stopwatch.  This method may be called to cease measuring of time,
		/// and remember the current recording.  Subsequent calls to <see cref="Start"/>
		/// will resume recording from this point.
		/// </summary>
		void Stop();

		/// <summary>
		/// Resets the clock to zero.  This method may be called while stopped, or while
		/// running.
		/// </summary>
		void Reset();

		void Set(long val);

		/// <summary>
		/// Indicates whether the stopwatch is currently running.
		/// </summary>
		bool IsRunning
		{
			get;
		}
		
		/// <summary>
		/// Gets the time currently recorded by the stopwatch.
		/// </summary>
		TimeSpan Time
		{
			get;
		}
	}
}
