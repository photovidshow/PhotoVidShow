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
using System.Runtime.InteropServices;

namespace DVDSlideshow
{
	/// <summary>
	/// A stopwatch that can measure time intervals to 10th-of-a-millisecond precision.
	/// </summary>
	/// <remarks>
	/// Comparing <see cref="DateTime.Now"/> values gives 15ms resolution, and comparing
	/// <see cref="Environment.TickCount"/> gives 500ms resolution.  This class uses
	/// Win32 Interop calls to kernel32.dll, allowing 0.1ms resolution.
	/// If this class is not used on Win32, a <see cref="NotSupportedException"/> will be
	/// raised by most methods.
	/// </remarks>
	/// <example>
	/// <code>
	/// PrecisionStopWatch stopWatch = new PrecisionStopWatch();
	/// stopWatch.Start();
	/// // ...do your task here...
	/// TimeSpan time = stopWatch.Time();
	/// </code>
	/// You can use the <see cref="Stop"/> and <see cref="Reset"/> method to use one
	/// StopWatch instance multiple times, or to sum separate recordings together.
	/// </example>
	public sealed class PrecisionStopWatch : IStopWatch
	{
		#region Field declarations

		/// <summary>
		/// The frequency of the high-resolution performance counter.
		/// </summary>
		double _frequency;

		/// <summary>
		/// The time at which the most recent <see cref="Start"/> method was called.  This value
		/// does not include any time accrued between a previous Start-to-Stop measurement.
		/// </summary>
		long _currentRunStartTime;

		/// <summary>
		/// Any time accrued prior to the most recent call to <see cref="Start"/>.
		/// Calling <see cref="Reset"/> sets this value to zero.
		/// </summary>
		TimeSpan _carriedOverTime;

		bool _running = false;

		#endregion

		#region Interop method declarations

		/// <summary>
		/// The QueryPerformanceCounter function retrieves the current value of the high-resolution performance counter.
		/// </summary>
		/// <param name="x">Pointer to a variable that receives the current performance-counter value, in counts.</param>
		/// <returns>If the function succeeds, the return value is nonzero.</returns>
		[DllImport("kernel32.dll")]
		static extern int QueryPerformanceCounter(ref long x);

		/// <summary>
		/// The QueryPerformanceFrequency function retrieves the frequency of the high-resolution performance
		/// counter, if one exists. The frequency cannot change while the system is running.
		/// </summary>
		/// <param name="x">Pointer to a variable that receives the current performance-counter frequency, in
		/// counts per second. If the installed hardware does not support a high-resolution performance counter,
		/// this parameter can be zero.</param>
		/// <returns>If the installed hardware supports a high-resolution performance counter, the return value is nonzero.</returns>
		[DllImport("kernel32.dll")]
		static extern int QueryPerformanceFrequency(ref long x);

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the PrecisionStopWatch class, and starts timing.
		/// </summary>
		/// <exception cref="NotSupportedException">The system does not have a high-resolution performance counter.</exception>
		public PrecisionStopWatch()
		{
			// initialise the frequency
			_frequency = GetFrequency();

			// set the starting time to now
			Reset();
		}


		#endregion

		#region Public methods & Properties

		public void Reset()
		{
			StartTimingRun();
			_carriedOverTime = TimeSpan.Zero;
		}

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

		public void Set(long val)
		{
			StartTimingRun();
			_carriedOverTime = new TimeSpan(val*10000);
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
					double millis = GetCurrentRunMilliseconds();
					long ticks = (long)(millis * TimeSpan.TicksPerMillisecond);
					TimeSpan currentRunTime = TimeSpan.FromTicks(ticks);
					return _carriedOverTime.Add(currentRunTime);
				}
				return _carriedOverTime;
			}
		}

		#endregion

		#region Private helper methods

		/// <summary>
		/// Retrieves the current value of the high-resolution performance counter.
		/// </summary>
		/// <exception cref="NotSupportedException">The system does not have a high-resolution performance counter.</exception>
		/// <returns>A long that contains the current performance-counter value, in counts.</returns>
		long GetValue()
		{
			long ret = 0;
			if (QueryPerformanceCounter(ref ret) == 0)
			{
				// if the return value is 0, the dll call didn't work
				throw new NotSupportedException("Error while querying the high-resolution performance counter.");
			}
			return ret;
		}

		/// <summary>
		/// Retrieves the frequency of the high-resolution performance counter, if one exists. The frequency will not
		/// change while the system is running.
		/// </summary>
		/// <exception cref="NotSupportedException">The system does not have a high-resolution performance counter.</exception>
		/// <returns>A long that contains the current performance-counter frequency, in counts per second.</returns>
		long GetFrequency()
		{
			long ret = 0;
			if (QueryPerformanceFrequency(ref ret) == 0)
			{
				// if the return value is 0, the dll call didn't work
				throw new NotSupportedException("Error while querying the high-resolution performance counter frequency.");
			}
			return ret;
		}

		/// <summary>
		/// Returns the time that has passed since the object was constructed, or the <see cref="Reset()"/>
		/// method was called, in milliseconds.
		/// </summary>
		/// <returns></returns>
		double GetCurrentRunMilliseconds()
		{
			double millis = ((double)(GetValue() - _currentRunStartTime) / _frequency) * 1000d;
			return millis;
		}

		void StartTimingRun()
		{
			_currentRunStartTime = GetValue();
		}


		#endregion
	}
}