using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using DVDSlideshow;
using System.Drawing;
using ManagedCore;
using System.Text;


namespace dvdslideshowfontend
{
	/// <summary>
	/// Summary description for CSlideShowManager.
	/// </summary>
	public class CSlideShowMusicManager : CSlideShowAudioManager
	{
		
		private System.Windows.Forms.OpenFileDialog openFileDialog;

		//*******************************************************************
		public CSlideShowMusicManager(Form1 main_window) : base(main_window)
		{
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();

			openFileDialog.Filter = CGlobals.GetTotalAudioFilter();

            string myMusic = DefaultFolders.GetFolder("MyMusic");
			openFileDialog.InitialDirectory =myMusic;
			openFileDialog.Title ="Open music";

            // 
			// TODO: Add constructor logic here
			//

			InitializeComponent();

			mMainForm.GetSlideShowMusicPlanel().DragOver += new System.Windows.Forms.DragEventHandler(this.ListDragTarget_DragOver);
			mMainForm.GetSlideShowMusicPlanel().DragDrop += new System.Windows.Forms.DragEventHandler(this.ListDragTarget_DragDrop);
			mMainForm.GetSlideShowMusicPlanel().DragEnter += new System.Windows.Forms.DragEventHandler(this.ListDragTarget_DragEnter);
			mMainForm.GetSlideShowMusicPlanel().DragLeave += new System.EventHandler(this.ListDragTarget_DragLeave);

		}

        //*******************************************************************
        protected override int GetThumnailsYPosition()
        {
            return 128;
        }

        //*******************************************************************
        protected override ICollection GetMusicSlides()
        {
            return GetCurrentSlideShow().mMusicSlides;
        }


		//*******************************************************************
		private void ListDragTarget_DragOver(object sender, System.Windows.Forms.DragEventArgs e) 
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop)) 
			{
				e.Effect = DragDropEffects.Copy;
			}
		}

		//*******************************************************************
		private void ListDragTarget_DragDrop(object sender, System.Windows.Forms.DragEventArgs e) 
		{
			this.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();

			// try for dragging images to it
			try
			{
				Array a = (Array)e.Data.GetData(DataFormats.FileDrop);

				if ( a != null )
				{
					String[] s = new String[a.Length]	;
					for (int i=0;i<a.Length;i++)
					{
						s[i] = a.GetValue(i).ToString();

                        if (CGlobals.IsMusicFilename(s[i]) == true)
                        {
                              if (ManagedCore.IO.IsDriveOkToUse(s[i]) == false) return;
                        }
					}

                    // Call OpenFile asynchronously.
                    // Explorer instance from which file is dropped is not responding
                    // all the time when DragDrop handler is active, so we need to return
                    // immidiately (especially if OpenFile shows MessageBox).

                    bool musicStartsAfterSlideshowEnds =false;
					GetCurrentSlideShow().AddBackgroundMusicSlides(s, out musicStartsAfterSlideshowEnds);

                    if (musicStartsAfterSlideshowEnds == true)
                    {
                        AudioStartsAfterSlideshowEndsWarning("music");
                    }

                    RebuildPanel();
					// rebuild combo if we were synced to music
					if (GetCurrentSlideShow().SyncLengthToMusic==true)
					{
						Form1.mMainForm.GetSlideShowManager().RebuildPanel(null);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error in DragDrop function: " + ex.Message);

				// don't show MessageBox here - Explorer is waiting !
			}

		}
		
		//*******************************************************************
		private void ListDragTarget_DragEnter(object sender, System.Windows.Forms.DragEventArgs e) 
		{
			// Reset the label text.
			int i=1;
			i++;
			i++;

			//DropLocationLabel.Text = "None";
		}

		
		//*******************************************************************
		private void ListDragTarget_DragLeave(object sender, System.EventArgs e) 
		{
			// Reset the label text.
			int i=1;
			i++;
			i++;

			//DropLocationLabel.Text = "None";
		}



		//*******************************************************************
		public void InitializeComponent()
		{
			this.openFileDialog.Multiselect = true;
		}

		//*******************************************************************
		public void AddMusicButton_Click(object sender, System.EventArgs e)
		{
            this.mMainForm.GetSlideShowManager().StopIfPlayingAndWaitForCompletion();
            mMainForm.GoToMainMenu();

            if (AddMusicDialogStart() == true)
            {
                this.RebuildPanel();

                // rebuild combo if we were synced to music
                if (GetCurrentSlideShow().SyncLengthToMusic == true)
                {
                    Form1.mMainForm.GetSlideShowManager().RebuildPanel(null);
                }
            }
        }

        //*******************************************************************
        public bool AddMusicDialogStart()
        {	
			if (openFileDialog.ShowDialog().Equals(System.Windows.Forms.DialogResult.OK))
			{
				if (GetCurrentSlideShow()==null)
				{
                    CDebugLog.GetInstance().Error("Trying to add music to a null slideshow in AddMusicDialogStart");
					return false;
				}

                if (openFileDialog.FileNames.Length > 0)
                {
                    if (ManagedCore.IO.IsDriveOkToUse(openFileDialog.FileNames[0]) == false) return false;

                    openFileDialog.InitialDirectory = openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(openFileDialog.FileNames[0]);  // rember current folder

                    bool musicStartsAfterSlideshowEnds = false;
				    GetCurrentSlideShow().AddBackgroundMusicSlides(openFileDialog.FileNames, out musicStartsAfterSlideshowEnds);

                    if (musicStartsAfterSlideshowEnds == true)
                    {
                        AudioStartsAfterSlideshowEndsWarning("music");
                    }

                    return true;
                  
                }
			}
            return false;

		}

        //*******************************************************************
        public void AudioStartsAfterSlideshowEndsWarning(string type)
        {
            bool showUser = true;
            if ( UserSettings.GetInstance().EntryExists("Music", "warnUserwhenAudioOutslideSlidehow") == true)
            {
                showUser = UserSettings.GetBoolValue("Music", "warnUserwhenAudioOutslideSlidehow");
            }

            CSlideShow ss = GetCurrentSlideShow();
            bool noSlidesInSlideshow = false;
            if (ss!=null)
            {
                double length = ss.GetLengthInSeconds();
                if (length <=0)
                {
                    noSlidesInSlideshow = true;
                }
            }
            if (showUser == true)
            {
                StringBuilder sb = new StringBuilder("");
                sb.Append("One or more ");
                sb.Append(type);
                sb.Append(" tracks imported into the slideshow starts after the current slideshow ends.");
                if (noSlidesInSlideshow == true)
                {
                    sb.Append("\r\n\r\nThis may be because no slides exist in the slideshow yet.");
                }

                if (type == "music")
                {
                    sb.Append("\r\n\r\nYou can still view the music tracks imported by editing the background music track list, select arrow ");
                    sb.Append("next to import music icon in the storyboard and select 'Add or edit background music track list'.");
                }

                UserMessage.Show(sb.ToString(), "Warning, imported audio outside slideshow", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        //*******************************************************************
        protected override void AddThumbnailsRightClickOptions(MusicThumbnail thumbnail, int music_index, int total_music)
        {
            CMusicSlide ms = thumbnail.mMusicSlide;

            if (ms.IsLoopMusicSlide == false && music_index != 0)
            {
                thumbnail.add_move_up_order();
            }
            if (music_index + 1 < total_music)
            {
                if (((CMusicSlide)GetCurrentSlideShow().mMusicSlides[music_index + 1]).IsLoopMusicSlide == false)
                {
                    thumbnail.add_move_down_order();
                }
            }
        }

        //*******************************************************************
        protected override MusicThumbnail CreateThumbnail(CMusicSlide ms)
        {
            return new MusicThumbnail(ms, this, MusicThumbnail.AudioType.MUSIC);
        }


        //*******************************************************************
        public override void DeleteItem(CMusicSlide item)
        {

            ArrayList to_remove = new ArrayList();

            to_remove.Add(item);

            if (GetCurrentSlideShow() == null)
            {
                Log.Error("Can not delete audio slides, null slideshow in DeleteItem");
                return;
            }

            GetCurrentSlideShow().RemoveMusicSlides(to_remove);
          
        }

        //*******************************************************************
        public void EditMusicTracksClick(object sender, System.EventArgs e)
        {
            //
            // If no music slides, then automatically bring up the music select browser
            //
            if (mCurrentMusicSlideThumbnails.Count == 0)
            {
                AddMusicButton_Click(sender, e);
            }

            //
            // If music exists, bring up the trim music window with first music track
            //
            if (mCurrentMusicSlideThumbnails.Count != 0)
            {
                MusicThumbnail firstThumbanil = mCurrentMusicSlideThumbnails[0];
                firstThumbanil.EditAudio_Click(sender, e);
            }
        }
	}
}
