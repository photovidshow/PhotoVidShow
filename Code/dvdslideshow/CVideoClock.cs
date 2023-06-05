using System;
using System.Threading;
using System.Collections;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for CVideoClock.
	/// </summary>
	/// 

	public delegate void VideoClockTickHandler(int frame);

	public delegate void VideoStopHandler();

 

	public class CVideoClockCallback
	{
		public VideoClockTickHandler for_object;
		private bool locked = false;
	   
		private CVideoClock mFromClock;

		public CVideoClockCallback(VideoClockTickHandler fo, CVideoClock fc)
		{
			for_object = fo;
			mFromClock=fc;
		}

		public void Tick()
		{
			CallObject();
		}
		private void CallObject()
		{
			if (locked==true)
			{
				Console.WriteLine("LOCKED!!!");
				return ;
			}

			locked=true;
			for_object(this.mFromClock.Frame);
			locked = false;

		}

	}

	public class CVideoClock
	{
		IStopWatch mStopWatch = new PrecisionStopWatch();
		private long mNextTime=0;
		private long mStep=0;
		public bool mStopping=false;
		public event VideoStopHandler Stopped;
		private bool mNoCallbacks=false;
        private object mMutex = new object();
        private ArrayList mCallbacks = new ArrayList();
        private int mCurrentFrame = 0;
        private bool mRunning = false;
        public Thread mRunningThread;
        private object objectRunLock = new object();

		private bool mSendTicks=true;

        public bool Running 
        {
            get
            {
                return mRunning;
            }
        }

        public bool Stopping
        {
            get
            {
                return mStopping;
            }
        }

		public bool SendTicks
		{
			get
			{
				return mSendTicks;
			}
			set
			{
                mSendTicks = value;
			}
		}
	
		public event VideoClockTickHandler Tick
		{
			add
			{
				this.mCallbacks.Add( new CVideoClockCallback(value, this));
			}
			remove
			{ 
				foreach (CVideoClockCallback vc in mCallbacks)
				{
					if (vc.for_object.GetHashCode() == value.GetHashCode())
					{
						this.mCallbacks.Remove(vc);
						return ;
					}
				}
			}
		}

	
		
		public int Frame
		{
			get
			{
				return mCurrentFrame;
			}
			set
			{
                lock (objectRunLock)
                {
                    long m = (long)(((double)value) * (1000 / CGlobals.mCurrentProject.DiskPreferences.frames_per_second));
                    mStopWatch.Set(m);
                    mNextTime = m + mStep;
                }
			}
		}

		public CVideoClock()
		{
			//
			// TODO: Add constructor logic here
			//
		}


		public void Start()
		{
			if (mRunning==true || mStopping==true) return ;
		
			mRunning=true;
			mNoCallbacks=false;
		
			System.Threading.ThreadStart s = new System.Threading.ThreadStart(this.RunInternalClock);
			mRunningThread = new Thread(s);
			mRunningThread.Start();
		}


		public void Stop()
		{
            lock (mMutex)
            {
			    if (mRunning == true)
			    {
				    mRunning=false;
				    mStopping = true;
			    }
            }
		}


		public void StopAndDontSendAnyCallbacks()
		{
            lock (mMutex)
            {
			    if (mRunning == true)
			    {
				    mRunning=false;
				    mStopping = true;
				    mNoCallbacks=true;
			    }
            }
		}

        public void GainRunLock()
        {
            Monitor.Enter(objectRunLock);
        }

        public void ReleaseRunLock()
        {
            Monitor.Exit(objectRunLock);
        }

		// keep posting message out every n secons until we receieve a stop command
		private void RunInternalClock()
		{
			int skip=0;

            if (mStopping == true)
            {
                return;
            }

            lock (mMutex)
            {
			    this.mStopWatch.Start();

			    float frame_in_mili = 1000.0f/CGlobals.mCurrentProject.DiskPreferences.frames_per_second;
			    mStep = (long)frame_in_mili;

			    mNextTime = mStep;

		    //	Console.WriteLine("in");

			    mRunning=true;
            }

			while(mRunning==true)
			{
				//	Console.WriteLine("here11");
				Thread.Sleep(1);

				//	Console.WriteLine("here12");

				if (mRunning==true)
				{
                    lock (objectRunLock)
                    {
					    double current_time = mStopWatch.Time.TotalMilliseconds;
    					
					    if (current_time>= mNextTime)
					    {
						    //Console.WriteLine("here");
						    mNextTime = mNextTime + mStep;
    						
						    int f = (int) ((current_time/(1000.0f/CGlobals.mCurrentProject.DiskPreferences.frames_per_second)));
						    if (f<0) f=0;

						    //	Console.WriteLine("Doing frame "+f+" skip="+skip);
						    mCurrentFrame = f;

						    // this will start of threading
						    foreach (CVideoClockCallback vc in mCallbacks)
						    {
							    if (mNoCallbacks==false && SendTicks==true)
							    {
								    vc.Tick();
							    }
						    }
						    skip=0;
					    }
					    else
					    {
						    skip++;
					    }
                    }
				}
			
			}

            lock (mMutex)
            {
			    this.mStopWatch.Stop();
			    mStopping=false;
			    if (mNoCallbacks==false)
			    {
				    Stopped();
			    }
            }
		}
	
	}
}
