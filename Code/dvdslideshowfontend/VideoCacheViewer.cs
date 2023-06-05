using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;
using DVDSlideshow.GraphicsEng;
using DVDSlideshow.GraphicsEng.Direct3D;
using MangedToUnManagedWrapper;

namespace dvdslideshowfontend
{
   
    public partial class VideoCacheViewer : Form
    {
        private class memblock
        {
            public byte[] mMem;
            public memblock(int meg)
            {
                try
                { 
                    mMem = new byte[1024 * 1024 * meg];
                }
                catch
                {
                    ManagedCore.UserMessage.Show("Out of memory", "out of memory");
                }
            }
        }
        private List<memblock> myMemBlocks = new List<memblock>();

        public VideoCacheViewer()
        {
            InitializeComponent();
            UpdateStats();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateStats();
        }

         // **********************************************************************************************************
        private CVideoPlayer GetPlayerForSlide(CSlide slided, int id)
        {
            CVideoSlide vs = slided as CVideoSlide;
            if (vs !=null && vs.Player !=null)
            {
                if (vs.Player.mID == id)
                {
                    return vs.Player;
                }
            }
            else
            {
                CImageSlide ims = slided as CImageSlide;
                ArrayList decors = ims.GetAllAndSubDecorations();
                foreach (CDecoration d in decors)
                {
                    CVideoDecoration vd = d as CVideoDecoration;
                    if (vd!=null && vd.Player !=null)
                    {
                        if (vd.Player.mID ==id)
                        {
                            return vd.Player;
                        }
                    }
                }
            }
            return null;
        }

        // **********************************************************************************************************
        private CVideoPlayer GetPlayerFromId(int id)
        {
            CProject p = CGlobals.mCurrentProject;
            ArrayList slideshows = p.GetAllProjectSlideshows(true);
            foreach(CSlideShow s in slideshows)
            {
                foreach (CSlide slided in s.mSlides)
                {
                    CVideoPlayer player = GetPlayerForSlide(slided, id);
                    if (player!=null)
                    {
                        return player;
                    }
                }
            }

            ArrayList menus = p.MainMenu.GetSelfAndAllSubMenus();
            foreach (CMainMenu menu in menus)
            {
                if (menu.BackgroundSlide !=null)
                {
                    CVideoPlayer player = GetPlayerForSlide(menu.BackgroundSlide, id);
                    if (player!=null)
                    {
                        return player;
                    }

                }
            }
            return null;
        }

         // **********************************************************************************************************
        private void UpdateStats()
        {
            this.SuspendLayout();

            int size = MangedToUnManagedWrapper.VideoPlayer.GetEstimatedMemoryFootprintOnLoadedPlayers();
            mEstimatedFootPrint.Text = size.ToString();

            int normalSize = CVideoPlayerCache.GetInstance().CalcNormalCacheSize();
            mNormalSizeLabel.Text = normalSize.ToString();

            double adjustmentFactor = CVideoPlayerCache.GetInstance().CalcEstimatedCurrentVideoMemoryFootprintFactor(normalSize);
            mAdjustmentFactorLabel.Text = adjustmentFactor.ToString();

            int currentCacheSize = CVideoPlayerCache.GetInstance().CacheSize;
            mCacheSize.Text = currentCacheSize.ToString();

            List<int> players =  CVideoPlayerCache.GetInstance().CachedPlayers;

            mNumberPlayersLabel.Text = players.Count.ToString();
            mVideoPlayerListBox.Items.Clear();

            foreach (int i in players)
            {
                String line = "";
                CVideoPlayer player = this.GetPlayerFromId(i);
                if (player == null)
                {
                    line = "Unknown";
                }
                else
                {
                    line = player.mFileName + " " + player.GetWidth() + "x" + player.GetHeight();
                }

                mVideoPlayerListBox.Items.Add(line);
            }


            // not do snapshot frames cache

            List<CVideoSnapshotFrameCache.CVideoSnapshotFrameCacheEntry> snapshotEntries = CVideoPlayerCache.GetInstance().VideoSnapshotFrameCache.Entries;
            mNumberSnapshotFrames.Text = snapshotEntries.Count.ToString();
            mVideoSnapshotFramesListBox.Items.Clear();

           
            foreach (CVideoSnapshotFrameCache.CVideoSnapshotFrameCacheEntry entry in snapshotEntries)
            {
                String line = "";
                CVideoPlayer player = this.GetPlayerFromId(entry.PlayerID);
                if (player == null)
                {
                    line = "Unknown";
                }
                else
                {
                    line = entry.PlayerID.ToString() + " " + System.IO.Path.GetFileName(player.mFileName) + " " + entry.Time + " " + player.GetWidth() + "," + player.GetHeight();
               
                }

                mVideoSnapshotFramesListBox.Items.Add(line);
            }
           
            this.ResumeLayout();
        }

        // *********************************************************************
        private void mMake200megButton_Click(object sender, EventArgs e)
        {
            memblock b = new memblock(200);
            myMemBlocks.Add(b);
        }

        // *********************************************************************
        private void mMake500megButton_Click(object sender, EventArgs e)
        {
            memblock b = new memblock(500);
            myMemBlocks.Add(b);
        }

        // *********************************************************************
        private void mMake1gButton_Click(object sender, EventArgs e)
        {
            memblock b = new memblock(1024);
            myMemBlocks.Add(b);
        }

        // *********************************************************************
        private void mMake2gigButton_Click(object sender, EventArgs e)
        {
            memblock b = new memblock(2048);
            myMemBlocks.Add(b);
        }

        // *********************************************************************
        private void mClearButton_Click(object sender, EventArgs e)
        {
            myMemBlocks.Clear();
        }
    }
}
