using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using DVDSlideshow;
using DVDSlideshow.GraphicsEng;

namespace dvdslideshowfontend
{
	/// <summary>
	/// Summary description for CPreviewManager.
	/// </summary>
	public class CPreviewManager
	{

		public System.Windows.Forms.Button PreviewDoneButton;
		public System.Windows.Forms.Button PreviewMenuButton;
		public System.Windows.Forms.Button PreviewPausePlayButton;
		public System.Windows.Forms.Button PreviewSelectButton;
		public System.Windows.Forms.Button PreviewLeftButon;
		public System.Windows.Forms.Button PreviewRightButton;
		public System.Windows.Forms.Button PreviewDownButton;
		public System.Windows.Forms.Button PreviewUpButton;

		public Timer mTimer;

		public CMainMenu	mCurrentMenu ;
		public CSlideShow	mCurrentSlideShow;
		public int			mFrame =0;
		public int			mCurrentItem =0;

		public CPreviewManager()
		{

			PreviewDoneButton = Form1.mMainForm.GetPreviewDoneButton();
			PreviewMenuButton = Form1.mMainForm.GetPreviewMenuButton();
			PreviewPausePlayButton = Form1.mMainForm.GetPreviewPausePlayButton() ;
			PreviewSelectButton = Form1.mMainForm.GetPreviewSelectButton() ;
			PreviewLeftButon = Form1.mMainForm.GetPreviewLeftButon() ;
			PreviewRightButton =   Form1.mMainForm.GetPreviewRightButton() ;
			PreviewDownButton = Form1.mMainForm.GetPreviewDownButton() ;
			PreviewUpButton =  Form1.mMainForm.GetPreviewUpButton() ;


			PreviewDownButton.Click+=new EventHandler(this.PreviewButtonDown_Click) ;
			PreviewUpButton.Click+=new EventHandler(this.PreviewButtonUp_Click) ;
			PreviewDoneButton.Click +=new EventHandler(this.DoneButtonClick);
			PreviewSelectButton.Click +=new EventHandler(this.SelectButtonClick);
			PreviewMenuButton.Click +=new EventHandler(this.MenuButtonClick);
			PreviewDownButton.Click+= new EventHandler(this.DownButtonClick);
			PreviewUpButton.Click+= new EventHandler(this.UpButtonClick);

			mTimer = new Timer();
			mTimer.Interval = 40;
			mTimer.Tick += new EventHandler(this.TimerTick);
			mTimer.Stop();

		}


		public void PreviewButtonDown_Click(object o, EventArgs e)
		{


		}


		public void PreviewButtonUp_Click(object o, EventArgs e)
		{


		}

		public void TimerTick(object o, EventArgs e)
		{

			CVideo video= null;
			if (mCurrentMenu!=null)
			{
				video = mCurrentMenu;
			}
			else if (mCurrentSlideShow!=null)
			{
				video = mCurrentSlideShow;
			} 

			if (video==null) return ;

			CImage i = video.GetVideo(mFrame, CGlobals.mCurrentProject.DiskPreferences.CanvasWidth, CGlobals.mCurrentProject.DiskPreferences.CanvasHeight, false);

			mFrame++;

            if (mFrame > video.GetTotalRenderLengthInFrames())
			{
				mFrame =0;
			}

			Form1.mMainForm.GetPreviewPictureBox().Image = i.GetRawImage();
		}



		public void Start()
		{
			CGlobals.mCurrentProject.ResetAllMediaStreams();
			mTimer.Start();
			mCurrentMenu = CGlobals.mCurrentProject.MainMenu;
			mFrame=0;
			mCurrentItem=0;
		}


		public void DoneButtonClick(object o, EventArgs e)
		{
			mTimer.Stop();
			Form1.mMainForm.EndPreview();
		}


		public void SelectButtonClick(object o, EventArgs e)
		{
			if (mCurrentMenu!=null)
			{
				this.mCurrentSlideShow = CGlobals.mCurrentProject.mSlideShows[mCurrentItem] as CSlideShow;
				mCurrentMenu=null;
			}
		}


		public void MenuButtonClick(object o, EventArgs e)
		{
			if (mCurrentSlideShow!=null)
			{
				mCurrentSlideShow=null;
			}
			Start();
		}


		public void DownButtonClick(object o, EventArgs e)
		{
			if (CGlobals.mCurrentProject.mSlideShows.Count-1 > mCurrentItem)
			{
				mCurrentItem++;
			}

		}


		
		public void UpButtonClick(object o, EventArgs e)
		{
			if (mCurrentItem > 0)
			{
				mCurrentItem--;
			}

		}



	}
}
