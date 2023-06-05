using System;
using System.Collections.Generic;
using DVDSlideshow.GraphicsEng;
using ManagedCore;

namespace DVDSlideshow
{
    // **********************************************************************************************
    // This class can cache individual video frames (snapshots).  This can speed up editing becuase
    // much of the editor will do a video snapshot requests over and over again for the same
    // video and for the same frame number.  E.g. in the decorations manager.
    public class CVideoSnapshotFrameCache
    {
        // ************************************************************************************
        // Sub class made public for debug purposes.
        // Do no use this class outside of this scope otherwise
        public class CVideoSnapshotFrameCacheEntry
        {
            private int mForPlayerID = -1;
            private float mSnapshotTime = -1;
            private DisposableObject<Surface> mSnapshotFrame;

            public int PlayerID
            {
                get { return mForPlayerID; }
            }

            public float Time
            {
                get { return mSnapshotTime; }           // time of -1 means last rendered frame
            }

            public DisposableObject<Surface> Snapshot
            {
                get { return mSnapshotFrame;  }
            }

            // ************************************************************************************
            public CVideoSnapshotFrameCacheEntry(int playerID, float time, Surface surface)
            {
                mForPlayerID = playerID;
                mSnapshotTime = time;
                mSnapshotFrame = new DisposableObject<Surface>(surface);
            }
        }

        private List<CVideoSnapshotFrameCacheEntry> mEntries = new List<CVideoSnapshotFrameCacheEntry>();
        private int mMaxCacheSize = 10;         // 10 video frames cache
        private int mMaxCacheSizeWhenEncoding = 4;         // 4 video frames cache when encoding

        //
        // Used for debug purpose, read only
        //
        public List<CVideoSnapshotFrameCacheEntry> Entries
        {
            get { return mEntries; }
        }

        // ************************************************************************************
        public void Clean()
        {
            foreach (CVideoSnapshotFrameCacheEntry entry in mEntries)
            {
                entry.Snapshot.Dispose();
            }

            mEntries.Clear();
        }

        // ************************************************************************************
        // 
        // A time of -1 can be passed in. This is useful when playing a video and encoding. 
        // This simply represents the 'last rendererd frame'
        // 
        public void AddSnapshot(int playerID, float time, Surface surface)
        {
            if (time >= 0 && GetSnapshot(playerID, time) != null)
            {
                //
                // Snapshot already exists for time, dispose of the one passed in and return
                //
                surface.Dispose();
                return;
            }

            int cacheSize = mMaxCacheSize;

            if (CGlobals.mVideoGenerateMode == VideoGenerateMode.FINAL_OUTPUT_MODE)
            {
                cacheSize = mMaxCacheSizeWhenEncoding;
            }

            if (mEntries.Count >= cacheSize)
            {
                RemoveOldestSnapshot();
            }

            CVideoSnapshotFrameCacheEntry entry = new CVideoSnapshotFrameCacheEntry(playerID, time, surface);
            mEntries.Add(entry);
        }

        // ************************************************************************************
        public void RemoveAllSnapshotsForPlayer(int playerID)
        {
            List<CVideoSnapshotFrameCacheEntry> toRemove = new List<CVideoSnapshotFrameCacheEntry>();

            foreach (CVideoSnapshotFrameCacheEntry entry in mEntries)
            {
                if (entry.PlayerID == playerID)
                {
                    toRemove.Add(entry);
                }
            }

            foreach (CVideoSnapshotFrameCacheEntry entry in toRemove)
            {
                entry.Snapshot.Dispose();
                mEntries.Remove(entry);
            }
        }

        // ************************************************************************************
        // Returned surface is a readers pointer and must be used and discarded before calling
        // any other method in this class.
        public Surface GetSnapshot(int playerID, float time)
        {
            //
            // Search newest entries first
            //
            for (int i=mEntries.Count-1; i >=0; i--)
            {
                CVideoSnapshotFrameCacheEntry entry = mEntries[i];
                if (entry.PlayerID == playerID &&
                    Math.Abs(time - entry.Time) < 0.01)
                {
                    return entry.Snapshot.Object;
                }
            }
            return null;
        }

        // ************************************************************************************
        private void RemoveOldestSnapshot()
        {
            if (mEntries.Count > 0)
            {
                mEntries[0].Snapshot.Dispose();
                mEntries.RemoveAt(0);
            }
        }
    }
}
