using System;
using System.Collections.Generic;
using DVDSlideshow.GraphicsEng;
using ManagedCore;

namespace DVDSlideshow
{
    public class CVideoPlayerCache
    {

        private Object thisLock = new Object();
        private List<int> mCachedPlayers;
        private int mCacheSize = 2;
        private CVideoSnapshotFrameCache mVideoSnapshotFrameCache = new CVideoSnapshotFrameCache();

        public CVideoSnapshotFrameCache VideoSnapshotFrameCache
        {
            get { return mVideoSnapshotFrameCache; }
        }

        static CVideoPlayerCache mTheCacheVideoPlayerDatabase= null;

        public int CacheSize
        {
            get { return mCacheSize; }
        }

        public List<int> CachedPlayers
        {
            get
            {
                List<int> returnList = null;
                lock (thisLock)
                {
                    returnList = new List<int>(mCachedPlayers.ToArray());
                }
           
                return returnList;
            }
        }

		//*******************************************************************
		public static CVideoPlayerCache GetInstance()
		{
			if (mTheCacheVideoPlayerDatabase==null)
			{
				mTheCacheVideoPlayerDatabase = new CVideoPlayerCache();
			}
			return mTheCacheVideoPlayerDatabase;
		}

		//*******************************************************************
        // Removed any stopped players from cache (typically called before encoding starts)
		public void CleanCache()
		{
            lock (thisLock)
            {
                while (mCachedPlayers.Count > 0)
                {
                    if (RemoveOldestStoppedPlayer() == false)
                    {
                        break;
                    }
                }

                mVideoSnapshotFrameCache.Clean();
            }
		}

        //*******************************************************************
        public void Remove(int id)
        {
            lock (thisLock)
            {
                if (DoesCachedPlayerExist(id) == false) return;

                mCachedPlayers.Remove(id);
                MangedToUnManagedWrapper.VideoPlayer.Shutdownplayer(id);
                ShutdownPlayer(id);
            }
        }


		//*******************************************************************
        public CVideoPlayerCache()
		{
			mCachedPlayers = new List<int>();
		}

        //*******************************************************************
        public static bool Reduce1080pVideosWhenEditing()
        {
            if (CGlobals.mVideoGenerateMode == VideoGenerateMode.EDIT_PRIVIEW_MODE &&
                    UserSettings.GetBoolValue("VideoSettings", "ReduceHDResolutionVideosWhenEditing") == true)
            {
                return true;
            }
            return false;
        }


    //*******************************************************************
    private void CleanUpMemoryBeforeVideoLoad()
    {
            //
            // Remove players before loading
            //
            RemovePlayersIfCacheFull();

            GC.Collect();
     }

    //*******************************************************************
    public MangedToUnManagedWrapper.CVideoInfo Load(string filename, int id)
        {
            lock (thisLock)
            {
                bool downloadIfMissing = CGlobals.mVideoGenerateMode == VideoGenerateMode.EDIT_PRIVIEW_MODE;

                string fn = ManagedCore.CryptoFS.GetNonCryptoFilename(filename, downloadIfMissing);

                // Usually means could not download, give up
                if (fn == "")
                {
                    return null;
                }

                bool quarter1080Videos = Reduce1080pVideosWhenEditing();

                CleanUpMemoryBeforeVideoLoad();

                MangedToUnManagedWrapper.CVideoInfo i = MangedToUnManagedWrapper.VideoPlayer.Load(
                    fn, 
                    id, 
                    UserSettings.GetBoolValue("VideoSettings", "ForceUseFFDShowIfInstalledForMp4"),
                    UserSettings.GetBoolValue("VideoSettings", "ForceUseFFDShowIfInstalledForMov"),
                    UserSettings.GetBoolValue("VideoSettings", "ForceUseFFDShowIfInstalledForMts"),
                    quarter1080Videos);

                AddCachedPlayer(id);
                if (i == null)
                {
                    return null;
                }

                return i;
            }
        }

        //*******************************************************************
        public bool IsStopped(int id)
        {
            lock (thisLock)
            {
                if (DoesCachedPlayerExist(id) == false) return true; ;
                return MangedToUnManagedWrapper.VideoPlayer.IsStopped(id) != 0;
            }
        }

        //*******************************************************************
        private bool RemoveOldestStoppedPlayer()
        {
            //
            // If all players are currently playing, then this does not stop any player
            //
            if (mCachedPlayers.Count <= 0) return false;

            bool removed_player = false;

            for (int ii = 0; ii < mCachedPlayers.Count; ii++)
            {
                int id = mCachedPlayers[ii];
                if (MangedToUnManagedWrapper.VideoPlayer.IsStopped(id) == 1)
                {
                    mCachedPlayers.RemoveAt(ii);
                    ShutdownPlayer(id);
                    removed_player = true;
                    break;
                }
            }

            return removed_player;
        }

        //*******************************************************************
        public bool RemoveThisPlayerIfCacheFull(CVideoPlayer player)
         {

            if (mCachedPlayers.Count <= mCacheSize)
            {
                return false;
            }

            lock (thisLock)
            {
                bool removedPlayer = false;

                for (int ii = 0; ii < mCachedPlayers.Count; ii++)
                {
                    if (player.mID == mCachedPlayers[ii])
                    {
                        if (MangedToUnManagedWrapper.VideoPlayer.IsStopped(player.mID) == 0)
                        {
                            MangedToUnManagedWrapper.VideoPlayer.Stop(player.mID, 0.0f, false, true);
                        }
                        mCachedPlayers.RemoveAt(ii);
                        ShutdownPlayer(player.mID);

                        removedPlayer = true;
                    }
                }
                return removedPlayer;
            }
            
        }

        //*******************************************************************
        private void ShutdownPlayer(int id)
        {
            MangedToUnManagedWrapper.VideoPlayer.Shutdownplayer(id);

            if (CGlobals.mVideoGenerateMode == VideoGenerateMode.FINAL_OUTPUT_MODE)
            {
                //
                // Remove any cached snapshots
                //
                mVideoSnapshotFrameCache.RemoveAllSnapshotsForPlayer(id);
            }
        }


        //*******************************************************************
        public void RemovePlayersIfCacheFull()
        {
            //
            // Do we have room, if not remove oldest stopped player
            //
            while (mCachedPlayers.Count >= mCacheSize)
            {
                if (RemoveOldestStoppedPlayer() == false)
                {
                    break;
                }
            }
        }

        //*******************************************************************
        public double CalcEstimatedCurrentVideoMemoryFootprintFactor(int currentSize)
        {
            int i = MangedToUnManagedWrapper.VideoPlayer.GetEstimatedMemoryFootprintOnLoadedPlayers();

            double normal = 921600 * currentSize; // normal = 10 x 720p videos  (720*1280* currentSize)
            double factor =  normal / ((double)i);
            if (factor > 1)
            {
                factor = 1;
            }
            return factor;
        }

        //*******************************************************************
        public int CalcNormalCacheSize()
        { 
            int cacheSize = 10;
            //
            // Allow 10 players if edititing or authoring a disk
            //
            if (CGlobals.mVideoGenerateMode == VideoGenerateMode.EDIT_PRIVIEW_MODE)
            {
                cacheSize = 10;

                //
                // Reduce footprint size if slides count over 400
                //
                int slides = CGlobals.mCurrentProject.GetTotalNumberOfSlides();
                if (slides > 400)
                {
                    cacheSize = 6;
                }
            }
            else
            {
                CGlobals.VideoType outputType = CGlobals.mCurrentProject.DiskPreferences.OutputVideoType;

                //
                // If VCD/SVCD/DVD, then set cache to 6 players else only 4 (i.e HD output types)
                //
                if (outputType == CGlobals.VideoType.SVCD || outputType == CGlobals.VideoType.VCD || outputType == CGlobals.VideoType.DVD)
                {
                    cacheSize = 6;
                }
                else
                {
                    cacheSize = 0;      // don't cache any 'non playing' videos
                }
            }

            return cacheSize; 
        }


        //*******************************************************************
        private void AddCachedPlayer(int id)
        {
            mCacheSize = CalcNormalCacheSize();

            int normalCacheSize = mCacheSize;

            //
            // The above normal cache size was orignally based on 720p input videos but now it's common
            // for users to use higher resolution video, so needs to be adjuested for this.
            //
            double factor = CalcEstimatedCurrentVideoMemoryFootprintFactor(mCacheSize);
            double adjustedCacheSizeDouble = ((double)mCacheSize) * factor;
            int adjustedCacheSize = (int)(adjustedCacheSizeDouble + 0.4999);

            //
            // Make sure adjusted cache size is not above normally used.
            //
            if (adjustedCacheSize < normalCacheSize)
            {
                adjustedCacheSize = normalCacheSize;
            }
            if (adjustedCacheSize > mCacheSize)
            {
                adjustedCacheSize = mCacheSize;
            }

            if (mCacheSize != adjustedCacheSize)
            {
                mCacheSize = adjustedCacheSize;
            }

            //
            // Now make sure we are not bigger than the user hard coded limit
            //
            int userMaximum = UserSettings.GetIntValue("VideoSettings", "MaximumVideoPlayersCached");
            if (mCacheSize > userMaximum)
            {
                mCacheSize = userMaximum;
            }

            //
            // Just in case
            //
            RemovePlayersIfCacheFull();

            mCachedPlayers.Add(id);
        }

        //*******************************************************************
        private bool DoesCachedPlayerExist(int id)
        {
            return mCachedPlayers.Contains(id);      
        }


        //*******************************************************************
        public void SetFrameStepping(CVideoPlayer for_player, bool value)
        {
            lock (thisLock)
            {
                if (DoesCachedPlayerExist(for_player.mID) == true)
                {
                    MangedToUnManagedWrapper.VideoPlayer.SetFrameStepping(for_player.mID, value);
                }
            }
        }

        //*******************************************************************
        public void StopPhysicalPlayer(CVideoPlayer for_player, double start_time, bool reset_to_start, bool wait)
        {
            lock (thisLock)
            {
                if (DoesCachedPlayerExist(for_player.mID) == true)
                {
                    MangedToUnManagedWrapper.VideoPlayer.Stop(for_player.mID, start_time, reset_to_start, wait);

                    // if encoding clean internals buffers in stopped video player to reduce memory
                    // (as there is a good chance we will not use it again)
                    if (CGlobals.IsEncoding() == true)
                    {
                        MangedToUnManagedWrapper.VideoPlayer.CleanInternalBuffers(for_player.mID);
                    }
                }
            }
        }

        //*******************************************************************
        // if not in cache then no matter, just ignore command
        public void SeekToTime(CVideoPlayer for_player, double time, bool wait_for_completion)
        {
            lock (thisLock)
            {
                if (DoesCachedPlayerExist(for_player.mID) == true)
                {
                    MangedToUnManagedWrapper.VideoPlayer.SeekToTime(time, wait_for_completion, for_player.mID);
                }
            }
        }

        //*******************************************************************
        // if not in cache then no matter, just ignore command
        public double GetLastSeekTime(CVideoPlayer for_player)
        {
            lock (thisLock)
            {
                if (DoesCachedPlayerExist(for_player.mID) == true)
                {
                    return MangedToUnManagedWrapper.VideoPlayer.GetLastSeekTime(for_player.mID);
                }
                return 0;
            }
        }


        //*******************************************************************
        // if not in cache then no matter, just ignore command
        public void SetVolume(int id, int vol)
        {
            lock (thisLock)
            {
                if (DoesCachedPlayerExist(id) == true)
                {
                    MangedToUnManagedWrapper.VideoPlayer.SetVolume(id, vol);
                }
            }
        }

        //*******************************************************************
        // PLAYER MUST BE IN CACHE TO WORK!
        public double LastSampleTime(int id)
        {
            lock (thisLock)
            {
                if (DoesCachedPlayerExist(id) == true)
                {
                    return MangedToUnManagedWrapper.VideoPlayer.LastSampleTime(id);
                }
                else
                {
                    ManagedCore.CDebugLog.GetInstance().Error("Called video LastSampleTime on a player that is not in the cache");
                }
            }
            return 0;
        }


        //*******************************************************************
        // PLAYER MUST BE IN CACHE TO WORK!
        public int ReachedEnd(int id)
        {
            lock (thisLock)
            {
                if (DoesCachedPlayerExist(id) == true)
                {
                    return MangedToUnManagedWrapper.VideoPlayer.ReachedEnd(id);
                }
                else
                {
                    ManagedCore.CDebugLog.GetInstance().Error("Called video ReachedEnd on a player that is not in the cache");
                }
            }
            return 0;
        }


        //*******************************************************************
        public string GetFirstFilterDetails(int id)
        {
            lock (thisLock)
            {
                if (DoesCachedPlayerExist(id) == true)
                {
                    return MangedToUnManagedWrapper.VideoPlayer.GetFirstFilterDetails(id);
                }
                else
                {
                    return "";
                }
            }
        }

        //*******************************************************************
        public string GetNextFilterDetails(int id)
        {
            lock (thisLock)
            {
                if (DoesCachedPlayerExist(id) == true)
                {
                    return MangedToUnManagedWrapper.VideoPlayer.GetNextFilterDetails(id);
                }
                else
                {
                    return "";
                }
            }
        }

        //*******************************************************************
        // This should be called when player is in the cahce
        public void AdvanceFrame(int id)
        {
            lock (thisLock)
            {
                if (DoesCachedPlayerExist(id) == true)
                {
                    MangedToUnManagedWrapper.VideoPlayer.AdvanceFrame(id);
                }
                else
                {
                    ManagedCore.CDebugLog.GetInstance().Warning("Called video advance frame on a player that is not loaded in the cache");
                }
            }
        }

        //*******************************************************************
        public void RestorePlayerIfNeeded(CVideoPlayer for_player)
        {
            lock (thisLock)
            {
                bool exists = DoesCachedPlayerExist(for_player.mID);

                // not cached in memory so load first
                if (exists == true)
                {
                    return;
                }

                if (for_player.mFileName == "")
                {
                    return;
                }

                //ManagedCore.CDebugLog.GetInstance().Warning("Cashing in player " + for_player.mID + " " + for_player.mFileName);

                bool downloadIfMissing = CGlobals.mVideoGenerateMode == VideoGenerateMode.EDIT_PRIVIEW_MODE;

                string fn = ManagedCore.CryptoFS.GetNonCryptoFilename(for_player.mFileName, downloadIfMissing);

                bool quarter1080Videos = Reduce1080pVideosWhenEditing();

                CleanUpMemoryBeforeVideoLoad();

                MangedToUnManagedWrapper.VideoPlayer.Load(
                    fn,
                    for_player.mID,
                    UserSettings.GetBoolValue("VideoSettings", "ForceUseFFDShowIfInstalledForMp4"),
                    UserSettings.GetBoolValue("VideoSettings", "ForceUseFFDShowIfInstalledForMov"),
                    UserSettings.GetBoolValue("VideoSettings", "ForceUseFFDShowIfInstalledForMts"),
                    quarter1080Videos);

                AddCachedPlayer(for_player.mID);

                // as we've just brought this player into memory need to do a seek to position player in correct place
                // SRG i think we always need to do this to get first frame into buffer

                // SRG HACK FIX ME
                for_player.mLastStopSeekTime = -1;

                // 
                // SRG 7/10/16
                // Not doing this can sometimes cause a (seek) later on to fail with tested mp4/mov files
                //
                StartPhysicalPlayer(for_player, false, true);
                StopPhysicalPlayer(for_player, for_player.mLastSeekTime, true, true);
            }
        }

    
        //*******************************************************************
        public void StartPhysicalPlayer(CVideoPlayer for_player, bool frame_step, bool mute_sound)
        {
            lock (thisLock)
            {
                RestorePlayerIfNeeded(for_player);

                //
                // Set if we need to frame step
                //
                MangedToUnManagedWrapper.VideoPlayer.SetFrameStepping(for_player.mID, frame_step);

                //
                // Start player
                //
                MangedToUnManagedWrapper.VideoPlayer.Start(for_player.mID, mute_sound);          
            }

        }

        //*******************************************************************
        public void GetCurrentVideoBuffer(DynamicSurface surface, CVideoPlayer for_player, ref double sampleTime, bool quartered)
        {
            lock (thisLock)
            {
                RestorePlayerIfNeeded(for_player);

                LockedRect lr = surface.Lock();

                try
                {
                    sampleTime = MangedToUnManagedWrapper.VideoPlayer.GetCurrentVideoBuffer(lr.mBits, lr.Pitch, surface.Width, surface.Height, for_player.mID, quartered);
                }
                finally
                {
                    surface.Unlock();
                }
            }

        }

    }
}
