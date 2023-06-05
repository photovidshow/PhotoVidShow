using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using MangedToUnManagedWrapper;
using ManagedCore;
using DVDSlideshow.GraphicsEng;
using System.Collections.Generic;

namespace DVDSlideshow
{

    public class NoVideoCodecInstalledError : ErrorException
    {
        public NoVideoCodecInstalledError(string message)
            : base(message)
        {
        }

        public bool mDetectedFFDShow = false;
    }


    public class CVideoPlayerDatabase
    {
        private List<CVideoPlayer> mReUsablePlayers = new List<CVideoPlayer>();
        private static CVideoPlayerDatabase mInstance = null;

        private CVideoPlayerDatabase() {}
        public static CVideoPlayerDatabase GetInstance()
        {
            if (mInstance == null)
            {
                mInstance = new CVideoPlayerDatabase();
            }
            return mInstance;
        }

        public void AddPlayer(CVideoPlayer player)
        {
            CVideoPlayer oldplayer = GetPlayer(player.StringID);
            if (oldplayer != null)
            {
                RemovePlayer(oldplayer);
            }

            mReUsablePlayers.Add(player);
        }

        public void RemovePlayer(CVideoPlayer player)
        {
            mReUsablePlayers.Remove(player);
        }

        public CVideoPlayer GetPlayer(string stringID )
        {
            foreach (CVideoPlayer player in mReUsablePlayers)
            {
                if (player.StringID == stringID)
                {
                    return player;
                }
            }
            return null;
        }
    }

	/// <summary>
	/// Summary description for CVideoPlayer.
	/// </summary>
    /// 
	public class CVideoPlayer  
	{
		public enum EStatus
		{
			EMPTY,
			STOPPED_AT_START,
			STOPPED,
			STARTED
		};

		object mThisLock = new object();
        private static object mGetVidBufferContentsLock = new object(); 

        public double mLastSampleTime = -1;
		private static int playerID=0 ;

        private bool mLoop = false;

        private string mStringID = "";  // used to reference 

		public int mID;
		public EStatus mStatus = EStatus.EMPTY;
		public bool mFrameStepping = false ;
		public int mFrameStepCount=0;	// used for conversion
        public string mFileName = "";
		private int mWidth;
        private int mHeight;
        private double mFrameRate = 1 / 23.976;
        private Int64  mDuration ;
        private bool mHasAC3Sound ;
        private bool mHasAudio;
        private int mVolume = 1;
        public double mLastSeekTime = 0.0;
		public double mLastStopSeekTime =-1;
		private CVideoFrameRateConverter mFrameRateConverter = null;
        private int mLastFrameSteppingInputFrame = -1;
        private bool mCreateMipMaps = false;

        private const int mAdvanceVideoMaxFrames = 1000;

        public bool CreateMipMaps
        {
            get { return mCreateMipMaps; }
            set { mCreateMipMaps = value; }
        }

        public string StringID
        {
            get { return mStringID; }
            set 
            {
                if (mStringID != "")
                {
                    Log.Error("Modifing the ID for a video player is not supported");
                    return;
                }

                mStringID = value;
                CVideoPlayerDatabase.GetInstance().AddPlayer(this);
            }
        }

        public bool Loop
        {
            get { return mLoop; }
            set { mLoop = value; }
        }

		//*******************************************************************
		public CVideoPlayer(string stringID)
		{
			//
			// TODO: Add constructor logic here
			//
			playerID++;
			mID = playerID;
			mStatus = EStatus.EMPTY;
			mFileName = "";
            mStringID = stringID;
            if (mStringID != "")
            {
                CVideoPlayerDatabase.GetInstance().AddPlayer(this);
            }
                
		}

        //*******************************************************************
        ~CVideoPlayer()
        {
            // just in case
            CVideoPlayerCache.GetInstance().Remove(this.mID);
         
            if (mStringID != "")
            {
                CVideoPlayerDatabase.GetInstance().RemovePlayer(this);
            }
        }


		//*******************************************************************
        // YOU HAVE TO CALL THIS FIRST BEFORE ANYTHING ELSE WILL WORK
		public bool Load(string file)
		{
            string fullFilename = CGlobals.GetFullPathFilename(file);
            file = CGlobals.CheckFileExistsElseThrow(fullFilename);

			if (mStatus!=EStatus.EMPTY)
			{
				CDebugLog.GetInstance().Warning("Video player already loaded for file '"+file+"' (ignoring load)");
				return false ;
			}

            MangedToUnManagedWrapper.CVideoInfo i = CVideoPlayerCache.GetInstance().Load(file, mID);
            
            // If null it normally means could not even find the file, could not download or could not decrypt
			if (i==null) 
			{
                throw new ErrorException("Failed To load video '" + file + "'");
            }

            // if not null this usually means it failed to load because of missing codec(s)
            if (i.mLoaded == false)
            {
                NoVideoCodecInstalledError exception = new NoVideoCodecInstalledError("Failed to load video '" + file + "'");

                // If ffdshow installed we may have this configured not to allow us to use it
                if (i.mFFDShowDisallowedThisApp==true)
                {
                    exception.mDetectedFFDShow = true;
                }
 
                throw exception;
			}

			mWidth = i.mWidth;
			mHeight = i.mHeight;
			mStatus = EStatus.STOPPED_AT_START;
			mFileName = file ;
            mFrameRate = i.mFrameRate;
            mDuration = i.mDuration;
            mHasAC3Sound = i.mHasAC3Sound;
            mHasAudio = i.mHasAudio;
            mLastSeekTime = 0.0;

			return true;

		}

		//*******************************************************************
		public bool HasDolbyAC3Sound()
		{
            return this.mHasAC3Sound;
		}

		//*******************************************************************
		public double GetFrameRate()
		{
            return this.mFrameRate;
		}

        //*******************************************************************
        public int GetWidth()
        {
            return mWidth;
        }

        //*******************************************************************
        public int GetHeight()
        {
            return mHeight;
        }

		//*******************************************************************
		public void Start(bool frame_step)
		{
          //  Console.WriteLine("START!!!");
			lock (mThisLock)
			{
              
				//	Console.WriteLine("Called start");
				mFrameStepping = frame_step;
				mLastStopSeekTime=-1;

				if (mStatus != EStatus.STOPPED && mStatus != EStatus.STOPPED_AT_START)
				{
					Console.WriteLine("ERROR: Can not start a non stopped video");
					return ;
				}
				

				// need to do conversion
				if (mFrameStepping)
				{
                    // frame rate coversion is calculated base on end video encode frame rate NOT the frame rate we are rendering at (
                    // because it may be higher due to motion blur
					mFrameRateConverter = new CVideoFrameRateConverter(1.0 / mFrameRate, 
                                                                       CGlobals.mCurrentProject.DiskPreferences.OutputVideoFramesPerSecond);	
                    mLastFrameSteppingInputFrame = -1;
				}

               
                CVideoPlayerCache.GetInstance().StartPhysicalPlayer(this, frame_step, CGlobals.MuteSound);
				mStatus = EStatus.STARTED;
                mLastSampleTime = -1;
			}
		}

		//*******************************************************************
		public void Stop(double start_time)
		{
			Stop(start_time,true);
         
           
		}

		//*******************************************************************
		private void Stop(double start_time, bool reset_to_start)
		{
            //   Console.WriteLine("STOP " + start_time.ToString() );

			lock (mThisLock)
			{
				mFrameStepping = false ;
				if (mStatus != EStatus.STARTED)
				{
					if (mStatus == EStatus.STOPPED_AT_START)
					{
						mStatus = EStatus.STOPPED;
						return ;
					}

					return ;
				}
 
                mLastSeekTime = start_time;
                CVideoPlayerCache.GetInstance().StopPhysicalPlayer(this, start_time, reset_to_start, true);
            
				mStatus = EStatus.STOPPED;
                mLastSampleTime = -1;
			}
		}

        //*******************************************************************
        public void Reset(double start_time)
        {
            if (mStatus != EStatus.STOPPED_AT_START)
			{
                //
                // Ensure frame stepping turned off
                //
                CVideoPlayerCache.GetInstance().SetFrameStepping(this, false);

                if (this.mFrameStepping==true)
				{
					Stop(start_time);
				}
				
				// srg is there a reason we do it in this order cos i has to add the above
				this.SeekToTime(start_time,true);
				Stop(start_time);
       
			}
			mStatus = EStatus.STOPPED_AT_START;
            mLastSampleTime = -1;
            mLastSeekTime = start_time;
		}

	
		//*******************************************************************
		// in terms of 100 nano second units. ouch!
		public Int64 GetDuration()
		{
            return mDuration;
		}

		//*******************************************************************
		public double GetDurationInSeconds()
		{
			Int64 d = GetDuration();
			double d1 =  (double)d;
			d1 = d1/ 10000000.0f;

			return d1;
		}


		//*******************************************************************
		public void SeekToTime(double time, bool wait_for_completion)
		{
         //   Console.WriteLine("SEEK TO TIME " + time.ToString());

          //  CGlobals.DeclareEncodeCheckpoint('!');
			lock (mThisLock)
            {

              //  CGlobals.DeclareEncodeCheckpoint('£');
   
                mLastSampleTime = -1;

				if (wait_for_completion==true &&
					(mStatus == EStatus.STOPPED || mStatus == EStatus.STOPPED_AT_START) &&
					time ==this.mLastStopSeekTime)
				{
                 //   CGlobals.DeclareEncodeCheckpoint('$');
				//	Console.WriteLine("Ignore seek as we should already be there");
					return ;
				}

				this.mLastStopSeekTime=-1;

			//	Console.WriteLine("Seek "+time);

              //  CGlobals.DeclareEncodeCheckpoint('%');
                if (this.mLoop == true)
                {
                    double duration = this.GetDurationInSeconds();
                    int count=0;

                //    CGlobals.DeclareEncodeCheckpoint('^');
                    while (time > duration && count <100000)
                    {
                        time -= duration;
                        count++;
                    }
                }
           //     CGlobals.DeclareEncodeCheckpoint('&');

                if (time < 0) time = 0;

                int t = (int)(time / GetFrameRate());
                time = ((double)t)*GetFrameRate();

                // either seek now if player is in cache

                if (wait_for_completion || Math.Abs( mLastSeekTime - time) > 0.001)
                {
                    // Calling seek on the same time twice, may not cause a sample to be produced in grabber for that time
                    CVideoPlayerCache.GetInstance().SeekToTime(this, time, wait_for_completion);
                }
                mLastSeekTime = time;

                //  CGlobals.DeclareEncodeCheckpoint('*');
				if (wait_for_completion==true && 
					(mStatus == EStatus.STOPPED || mStatus == EStatus.STOPPED_AT_START))
				{
					this.mLastStopSeekTime = time;
				}

			}
        //    CGlobals.DeclareEncodeCheckpoint('(');

		}

        //*******************************************************************
        // Will do a seek SO VERY SLOW, use this for getting individual frames
        // or doing a video seek
        //
        // This now caches away the last n snapshot frames, which should speed
        // up repeated calls to this.
        //
        // The return surface is a readers pointer see comments in GetNextImage
        public Surface GetVideoSnapshot(float time)
		{
            float adjustTime = time;
            // If time negative, then set to first frame (i.e. used by decoration manager preview)
            if (adjustTime < 0)
            {
                adjustTime = 0;
            }

            //
            // Is this video snapshot frame cached?
            //
            Surface cachedSurface = CVideoPlayerCache.GetInstance().VideoSnapshotFrameCache.GetSnapshot(this.mID, time);
            if (cachedSurface != null)
            {
                //
                // Although we have this frame cached, the video player itself may not be set at this time.
                // If status stoped at start then we also need to apply the seek to time to set the video
                // player at the correct time.
                // This is needed (for example) when selecting play on a video slide that has been preview seeked
                // whilst in the preview stopped state
                //
                if (mStatus == EStatus.STOPPED_AT_START)
                {
                    Stop(0, false);
                    SeekToTime(adjustTime, CGlobals.WaitVideoSeekCompletion);
                }
                return cachedSurface;
            }

			if (mStatus != EStatus.STOPPED)
			{
				Stop(0,false);
			}

			SeekToTime(adjustTime, CGlobals.WaitVideoSeekCompletion);

            //
            // Get snapshot and add to cache
            //
            Surface snapshot = GetVideoBufferContents();
            CVideoPlayerCache.GetInstance().VideoSnapshotFrameCache.AddSnapshot(mID, time, snapshot);

            return snapshot;
		}

        //************************************************************************************************************
        // The video player returns a readers pointer to the surface on call to GetNextImage() / GetVideoSnapshot()
        // I.e this player owns the surface and is responsible for disposing it.  This is because these
        // surfaces may be cached for speedups or when encoding it sometimes needs to store the last frame for
        // motion blur sub frames or if the device is lost.
        public Surface GetNextImage(int frame_id, float startTimeOffsetIfLoops, float endOffsetToTriggerLoop)
		{
            CVideoSnapshotFrameCache snapshotCache = CVideoPlayerCache.GetInstance().VideoSnapshotFrameCache;
            if (mStatus == EStatus.STOPPED || mStatus == EStatus.STOPPED_AT_START)
			{
				Start(!CGlobals.PlayVideoRealTime);
			}

            //
            // Only advance if next frame is an encode key frame (i.e. not motion blur sub frames)
            // i.e. videos do not advance during motion blur sub frames
            //
            if (mFrameStepping == true)
            {
                if (Encoder.NextEncodeFrameKeyFrame == false && mLastFrameSteppingInputFrame !=-1)
                {
                    Surface nextSnapshot = snapshotCache.GetSnapshot(mID, -1);

                    //
                    // No cached frame? create one
                    //
                    if (nextSnapshot == null)
                    {
                        //
                        // Remove any snapshots for this player only
                        //
                        snapshotCache.RemoveAllSnapshotsForPlayer(mID);
                        nextSnapshot = GetVideoBufferContents();

                        //
                        // Add new snapshot
                        //
                        snapshotCache.AddSnapshot(mID, -1, nextSnapshot);
                    }
                    
                    mLastFrameSteppingInputFrame = frame_id;

                    return nextSnapshot;
                }
            }

            CheckIfReachedEndAndLoopingAndNeedsResetting(startTimeOffsetIfLoops, endOffsetToTriggerLoop);
            mLastSampleTime = CVideoPlayerCache.GetInstance().LastSampleTime(mID);

            if (mFrameStepping == true)
            {
                //
                // Ok stop video advancing if this was a re-attempt, or player has eached end and stopped
                //
                if ((mLastFrameSteppingInputFrame != -1 && frame_id == mLastFrameSteppingInputFrame) ||
                    (CVideoPlayerCache.GetInstance().IsStopped(mID) == true))
                {
                    Surface nextSnapshot = snapshotCache.GetSnapshot(mID, -1);

                    //
                    // No cache frame?, create one
                    //
                    if (nextSnapshot == null)
                    {
                        snapshotCache.RemoveAllSnapshotsForPlayer(mID);
                        nextSnapshot = GetVideoBufferContents();
                        snapshotCache.AddSnapshot(mID, -1, nextSnapshot);
                    }

                    return nextSnapshot;
                }

                mLastFrameSteppingInputFrame = frame_id;

                double sampleTime = -1;
                Surface nextFrameSteppingSnapshot = GetVideoBufferContents(ref sampleTime);

                //
                // If first frame, then see if video needs advancing to start and also set base time
                //
                if (mFrameRateConverter.FrameCount == 0)
                {
                    double lastResetTime = CVideoPlayerCache.GetInstance().GetLastSeekTime(this);

                    if (sampleTime == -1)
                    {
                        Log.Error("GetVideoBufferContents returned blank for first frame for video " + mFileName + " slideshow frame:" + frame_id);
                    }
                    else
                    {
                        int count = 0;
                        double initialAdvanceTime = sampleTime;

                        while (DoesVideoNeedAdvancingToReachStart(sampleTime, lastResetTime) == true && count < mAdvanceVideoMaxFrames)
                        {
                            nextFrameSteppingSnapshot.Dispose();
                            CVideoPlayerCache.GetInstance().AdvanceFrame(mID);
                            nextFrameSteppingSnapshot = GetVideoBufferContents(ref sampleTime);
                            count++;
                        }

                        if (count >= mAdvanceVideoMaxFrames)
                        {
                            Log.Error("Video timed out advancing to expected start:" + lastResetTime + ", first sample:" + initialAdvanceTime);
                        }
                    }

                    mFrameRateConverter.BaseSampleTime = lastResetTime;
                }

                int to_step = mFrameRateConverter.FramesToStep(sampleTime);

                while (to_step > 0)
                {
                    //	Console.WriteLine("GetNextImage Advacing frame ");

                    CVideoPlayerCache.GetInstance().AdvanceFrame(mID);

                    to_step--;

                    if (to_step > 0)
                    {
                        Surface remove = GetVideoBufferContents();
                        remove.Dispose();
                    }
                }

                snapshotCache.RemoveAllSnapshotsForPlayer(mID);
                snapshotCache.AddSnapshot(mID, -1, nextFrameSteppingSnapshot);
                return nextFrameSteppingSnapshot;
            }
            else
            {
                Surface snapshot = GetVideoBufferContents();
                //
                // As playing we simply clear the snapshot cache and add our new snapshot to it
                // to be cleared out on next call to GetNextImage() or by the cache itself when full.
                // We need to hold onto this new surface as we return a readers pointer to it.  The caller
                // is responsible for using the surface immediately before calling GetNextImage() again.
                //
                CVideoPlayerCache.GetInstance().VideoSnapshotFrameCache.Clean();
                CVideoPlayerCache.GetInstance().VideoSnapshotFrameCache.AddSnapshot(mID, -1, snapshot);

                return snapshot;

            }
          
		}

        // *************************************************************************************************
        public bool DoesVideoNeedAdvancingToReachStart(double sampleTime, double inputVideoExpectedBaseStartTime)
        {
            //
            // Some videos e.g. the .mov with debbie dvd, do not seek to the exact time we reset it to if
            // the reset start time is not time 0.  Perhaps we seek to the previous key frame e.g. 0.5 seconds earlier.
            // If so, we need to skip over these frames until we receive the frame we were expecting.
            //
            if ((sampleTime + mFrameRate) > inputVideoExpectedBaseStartTime)
            {
                return false;
            }
            return true;
        }

        //*******************************************************************
        private bool CheckIfReachedEndAndLoopingAndNeedsResetting(float startTimeOffsetIfLoops, float endOffsetToTriggerLoop)
        {
            bool needs_reset = false;

            if (this.mLoop == true)
            {
                double sample_time = CVideoPlayerCache.GetInstance().LastSampleTime(mID);
                int ii = CVideoPlayerCache.GetInstance().ReachedEnd(mID);

                if (ii == 1)
                {
                    needs_reset = true;
                }
                else if (this.mLastSampleTime != -1)
                {
                    double length = GetDurationInSeconds();
 
                    if (this.mLastSampleTime > length-endOffsetToTriggerLoop)
                    {
                        needs_reset = true;
                    }
                    else if (System.Math.Abs(this.mLastSampleTime - sample_time) < 0.001)
                    {
                        if (System.Math.Abs(this.mLastSampleTime - length) < 0.1)
                        {                  
                             needs_reset = true;
                        }
                    }
                }
            }

            // looped video so start at beginning again as we've reached the end
            if (needs_reset == true)
            {
                bool framestepping = mFrameStepping;
                this.Stop(startTimeOffsetIfLoops, true);
                this.Start(framestepping);
                return true;
            }
            return false;
        }

        //*******************************************************************
        // this method makes sure this player is cached into memory.
        // i.e. call this when you know you will need it soon and prevents
        // a slight pause when it is actually needed
        public void EnsureCachedInMemory()
        {
            CVideoPlayerCache.GetInstance().RestorePlayerIfNeeded(this);
        }

        //*******************************************************************
        private Surface GetVideoBufferContents()
        {
            double sampleTime = 0;
            return GetVideoBufferContents(ref sampleTime);
        }

		//*******************************************************************
        private Surface GetVideoBufferContents(ref double sampleTime)
		{
            lock (mGetVidBufferContentsLock)
            {
                CGlobals.DeclareEncodeCheckpoint('M');

                if (GraphicsEngine.Current == null)
                {
                    Log.Error("No engine in GetVideoBufferContents");
                    return null;
                }

                int width = mWidth;
                int height = mHeight;
                bool quartered = false;

                //
                // Speed up if editing 1080p video or higher, simply quarter the frame.
                //
                if (CVideoPlayerCache.Reduce1080pVideosWhenEditing() == true && (mHeight >=1080 || mWidth >=1920 ) )
                {
                    width >>= 1;
                    height >>= 1;
                    quartered = true;
                }

                DynamicSurface surface = GraphicsEngine.Current.GenerateDynamicSurface((uint)width, (uint) height, DynamicSurface.Usage.WRITE_ONLY, this.ToString()+"::GetVideoBufferContents", mCreateMipMaps);

                try
                {
                    CVideoPlayerCache.GetInstance().GetCurrentVideoBuffer(surface, this, ref sampleTime, quartered);

                    CGlobals.DeclareEncodeCheckpoint('N');

                }
                catch
                {
                    if (surface != null)
                    {
                        surface.Dispose();
                    }
                    throw;
                }

                return surface;
            }
		}

		//*******************************************************************
		public bool ContainsAudio()
		{
            return mHasAudio;
		}

		//*******************************************************************
		public void SetVolume(int vol)
		{
            mVolume = vol;
            CVideoPlayerCache.GetInstance().SetVolume(this.mID, vol);
		}

		//*******************************************************************
		public int GetVolume()
		{
            return mVolume;
		}

		//*******************************************************************
		public bool IsVideoInCorrrectOutputDVDFormat()
		{
			string upper = mFileName.ToUpper();
			if (upper.EndsWith(".MPG")== false &&
			    upper.EndsWith(".MPEG")== false)
			{
			   return false;
			}

			if (mWidth != CGlobals.mCurrentProject.DiskPreferences.OutputVideoWidth  ||
                mHeight != CGlobals.mCurrentProject.DiskPreferences.OutputVideoHeight ||
				this.ContainsAudio() == false)
			{
			   return false;
			}

			if (System.Math.Abs(1 / GetFrameRate() -
                         CGlobals.mCurrentProject.DiskPreferences.OutputVideoFramesPerSecond) > 0.01)
			{
			   return false;
			}

			return true;

		}

        //*******************************************************************
        public string GetFirstFilterDetails()
        {
            return CVideoPlayerCache.GetInstance().GetFirstFilterDetails(this.mID);
        }

        //*******************************************************************
        public string GetNextFilterDetails()
        {
            return CVideoPlayerCache.GetInstance().GetNextFilterDetails(this.mID);
        }
    }
}
