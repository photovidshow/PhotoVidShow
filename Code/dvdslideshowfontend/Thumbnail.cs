using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;
using DVDSlideshow;
using System.Threading;
using CustomButton;
using System.Globalization;
using DVDSlideshow.GraphicsEng;

namespace dvdslideshowfontend
{
	/// <summary>
	/// Summary description for Thumbnail.
	/// </summary>
	/// 
	
	// represents the thumbnail icon in the scrollable slidehow view
    public class Thumbnail : StoryboardComponent
	{
        private const int pictureBoxWidth = 124;
        private const int pictureBoxHeight = 78; 

        private ThumbnailControl mThumbnailControl = null;
		private System.Drawing.Image mImage = null;
        private bool mImageIsSetWithCM = false;
        public CImageSlide mSlide;
        public bool mHighLighted = false;
        public CSlideShowManager mManager;
        private bool mImageValid = false;
        private bool mSlideLengthComboEnabled = true;
        private static int invalidImageCount = 0;

		public System.Drawing.Image Image
		{
			get { return mImage; }
		}

        public bool ImageIsSetWithCM
        {
            get { return mImageIsSetWithCM ; }
        }

        public ThumbnailControl ForThumbnailControl
        {
            get { return mThumbnailControl; }
            set 
            { 
                mThumbnailControl = value;
                if (mThumbnailControl != null)
                {
                    mThumbnailControl.ParentThumbnail = this;
                }
            }
        }

        public CImageSlide Slide
        {
            get { return mSlide; }
        }

        public bool Highlighted
        {
            get { return mHighLighted; }
        }

        public bool ImageValid
        {
            get { return mImageValid; }
        }

        public CSlideShowManager Manager
        {
            get { return mManager; }
        }

        public bool SlideLengthComboEnabled
        {
            get { return mSlideLengthComboEnabled; }
            set
            {
                mSlideLengthComboEnabled = value;

                if (mThumbnailControl != null)
                {
                    mThumbnailControl.SlideLengthComboEnabled = value ;
                }
            }
        }

		//*******************************************************************
		public void Blank()
		{
			mSlide=null;
			mManager=null;
            mHighLighted = false;
            mSlideLengthComboEnabled = true;
            mImageValid = false;

            if (mImage != null)
            {
                mImage.Dispose();
            }

            mImage = null;

            if (mThumbnailControl != null)
            {
                mThumbnailControl.ParentThumbnail = null;
            }

            mThumbnailControl = null;
		}

        
		//*******************************************************************
        public void SetHighlighted(bool val)
        {
            SetHighlighted(val, true);
        }
	
		//*******************************************************************
		public void SetHighlighted(bool val, bool informControl)
		{
            if (mHighLighted == val) return;
    
            if (informControl == true && mThumbnailControl != null)
            {
                mThumbnailControl.SetHighlighted(val, false);
            }
            mHighLighted = val;
		}
	
		//*******************************************************************
		public Thumbnail(CImageSlide slide, CSlideShowManager manager)
		{
			mSlide = slide;
			mManager = manager;

            mWidth = 128;
            mHeight = 118;
		}

        //*******************************************************************
        public void UpdateSlideLengthComboEnabled()
        {
            if (mSlide as CVideoSlide != null ||
                mSlide as CMultiSlideSlide != null ||
                DoesSlideContainSingleVideoOfSameLengthAsSlide(mSlide) ||
                mManager.mCurrentSlideShow.SyncLengthToMusic == true)
            {
                mSlideLengthComboEnabled = false;
            }
            else
            {
                mSlideLengthComboEnabled = true;
            }

            if (mThumbnailControl != null)
            {
                mThumbnailControl.SlideLengthComboEnabled = mSlideLengthComboEnabled ;
            }
        }

        //*******************************************************************
        private bool DoesSlideContainSingleVideoOfSameLengthAsSlide(CSlide slide)
        {
            CImageSlide imageSlide = slide as CImageSlide;
            if (imageSlide==null) return false;

            return imageSlide.DoesSlideContainSingleVideoOfSameLengthAsSlide();
        }

		//*******************************************************************
		public void ResetToSlideAndManager(CImageSlide slide, CSlideShowManager manager)
		{
			mSlide = slide;
			mManager = manager;
            mImageValid = false;
		}


        //*******************************************************************
		public void ReCalcComboText()
		{
            if (mThumbnailControl != null)
            {
                mThumbnailControl.ReCalcComboText();
            }
		}

        //*******************************************************************
        public void InvalidateImage()
        {
            InvalidateImage(false);
        }

        //*******************************************************************
        public delegate void InvalidateImageDelgate(bool slideLengthChanged);
        public void InvalidateImage(bool slideLengthChanged)
		{
            if (Form1.mMainForm.InvokeRequired == true)
            {
                Form1.mMainForm.Invoke(new InvalidateImageDelgate(InvalidateImage), new Object[] { slideLengthChanged });
                return;
            }

            // Every 20th time we invalidate an image, do a gc collect
            invalidImageCount++;
            if (invalidImageCount > 20)
            {
                GC.Collect();
               invalidImageCount = 0;
            }

            mImageValid = false;
			GeneratePictureBox();
            ReCalcComboText();
            UpdateSlideLengthComboEnabled();

            if (slideLengthChanged)
            {
                ComboChangedSoInformStoryboardOfChange();
            }
		}

        //*******************************************************************
        private void CreateImageBitmap(int w, int h)
        {
            if (mImage != null)
            {
                mImage.Dispose();
            }

            mImage = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        }

        //*******************************************************************
		public void GeneratePictureBox()
		{
            float ratio = CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction();
            int w = pictureBoxWidth;
            int h = (int)(((float)pictureBoxWidth) * ratio);

    		try
			{
				bool vs = CGlobals.VideoSeeking;
				bool wbsc = CGlobals.WaitVideoSeekCompletion;
				bool mute_sound = CGlobals.MuteSound;

				CGlobals.VideoSeeking=true;
				CGlobals.WaitVideoSeekCompletion = true ;
				CGlobals.MuteSound=true;

                GraphicsEngine engine = GraphicsEngine.Current;

                RenderSurface previousSurface = engine.GetRenderTarget();
                if (engine.BeginScene(null) == GraphicsEngine.State.OK)
                {
                    try
                    {
                        int imageSizeMultiplier = 1;

                        // if slide has decorations, draw at higer res- with bi-cubic stuff etc
                        if (mSlide.GetAllAndSubDecorations().Count > 1)
                        {
                            imageSizeMultiplier = 5;
                        }

                        using (RenderSurface surface = engine.GenerateRenderSurface((uint)(w * imageSizeMultiplier), (uint)(h * imageSizeMultiplier), this.ToString() + "::GeneratePictureBox"))
                        {
                            engine.SetRenderTarget(surface);
                            engine.ClearRenderTarget(0, 0, 0, 255);

                            int frame = -1;

                            if (mSlide.ThumbnailPreviewTime >= 0)
                            {
                                frame = mSlide.mStartFrameOffset + (int)(mSlide.ThumbnailPreviewTime * CGlobals.mCurrentProject.DiskPreferences.frames_per_second);
                            }

                            if (mImage == null || mImage.Width != w || mImage.Height != h)
                            {
                                CreateImageBitmap(w, h);

                            }
                            CImage tempcimage = new CImage(mImage);

                            
                            // higher res?
                            if (imageSizeMultiplier != 1)
                            {
                                Bitmap temp = new Bitmap(w * imageSizeMultiplier, h * imageSizeMultiplier, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                                CImage imageTemp = new CImage(temp);

                                mSlide.RenderFrame(frame, true, false, w * imageSizeMultiplier, h * imageSizeMultiplier);
                                engine.CopyDefaultSurfaceToImage(imageTemp);

                                Graphics g = Graphics.FromImage(mImage);
                                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
                                g.DrawImage(temp, 0, 0, w, h);
                                g.Dispose();
                                temp.Dispose();
                            }
                            else
                            {
                                // normal res
                                mSlide.RenderFrame(frame, true, false, w * imageSizeMultiplier, h * imageSizeMultiplier);
                                engine.CopyDefaultSurfaceToImage(tempcimage);
                            }
                            

                            mImageValid = true;
                            //_image.Save("C:\\ergh.jpg");
                        }                      
                    }
                    finally
                    {
                        engine.SetRenderTarget(previousSurface);
                        engine.EndScene();
                        GraphicsEngine.State state = engine.PresentToWindow(engine.HiddenWindow.Handle); // not really neeaded, but to be safe present to gidden window

                        if (state != GraphicsEngine.State.OK)
                        {
                            mImageValid = false;
                        }
     
                        CGlobals.WaitVideoSeekCompletion = wbsc;
                        CGlobals.VideoSeeking = vs;
                        CGlobals.MuteSound = mute_sound;
                    }
                }
			}
			catch (System.OutOfMemoryException e)
			{
				// file does not represent a valid image
				throw e;
			}
			int max = Math.Max(pictureBoxWidth, pictureBoxHeight);
			max+=15;

            if (mImage == null)
            {
                mImageValid = false;    // should be false already but just in case
                CreateImageBitmap(w, h);
            }

            if (mImage == null) return;

			int width = mImage.Width;
			int height = mImage.Height;
			// determine the size for the thumbnail image
			if (mImage.Width > max || mImage.Height > max)
			{
				if (mImage.Width > mImage.Height)
				{
					width = max;
					height = (int) (mImage.Height * max / mImage.Width);
				}
				else
				{
					width = (int) (mImage.Width * max / mImage.Height);
					height = max;
				}
			}
			// set feedback information
		

			// ok image is a video slide and video border

            bool hasAudio = false;
            bool hasAc3 = false;

            if (ContainsUserVideos(mSlide, ref hasAudio, ref hasAc3) == true)
			{
                Image filmborder = CustomButton.SelectBackgroundControl.GetVideoBorderImage();
                if (filmborder != null)
                {
                    int iw = mImage.Width;
                    int ih = mImage.Height;

                    Bitmap imageWithVideoBorder = new Bitmap(iw, ih);

                    Graphics g = Graphics.FromImage(imageWithVideoBorder);

                    Rectangle r = new Rectangle(0, 0, iw, ih);

                    int draw_w = iw - 18;
                    int draw_h = (int)(((float)draw_w) * ratio);

                    int org_h = ih;

                    int diff = org_h - draw_h;

                    Rectangle r21 = new Rectangle(9, 0, iw - 18, ih);

                    Rectangle r2 = new Rectangle(9, diff / 2, draw_w, draw_h);


                    SolidBrush br2 = new SolidBrush(Color.Black);
                    SolidBrush br = new SolidBrush(Color.White);

                    g.FillRectangle(br2, r21);

                    // draw inner video
                    g.DrawImage(mImage, r2, 0, 0, mImage.Width, mImage.Height, GraphicsUnit.Pixel);

                    // draw boarder
                    g.DrawImage(filmborder, r, 0, 0, filmborder.Width, filmborder.Height, GraphicsUnit.Pixel);

                    g.Dispose();

                    Image oldImage = mImage;
                    mImage = imageWithVideoBorder;
                    oldImage.Dispose();
                }
			}
            
            if (mSlide.MarkedAsChapter == true &&
                Manager.mCurrentSlideShow.ChapterMarkersTypeToUse == CSlideShow.ChapterMarkersType.SetFromSlides &&
                (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.DVD ||
                 CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.BLURAY))
            {
                // Write CM if a chapter marker

                using (Graphics g = Graphics.FromImage(mImage))
                using (Font font = new Font("Segoe UI", 11))
                using (SolidBrush br = new SolidBrush(Color.Gold))
                using (SolidBrush br2 = new SolidBrush(Color.Black))
                {
                    string s = "CM";
                    g.DrawString(s, font, br2, 11, 6);
                    g.DrawString(s, font, br, 10, 5);

                    
                }

                mImageIsSetWithCM = true;
            }
            else
            {
                mImageIsSetWithCM = false;
            }

            if (mThumbnailControl!=null)
            {
                mThumbnailControl.SetImage(mImage);
                mThumbnailControl.ReDrawSideIcons();
            }
		}
  
        //************************************
        public bool ContainsUserVideos(CSlide slide, ref bool hasAudio, ref bool hasAc3)
        {
            // legacy
            if (slide is CVideoSlide) return true;

            CImageSlide imageSlide = slide as CImageSlide;
            if (imageSlide == null) return false;

            bool hasVideos = false;

            ArrayList list = imageSlide.GetAllAndSubDecorations();
            foreach (CDecoration d in list)
            {
                if (d is CVideoDecoration)
                {
                    if (d.IsUserDecoration() == true)
                    {
                        CVideoDecoration vd = d as CVideoDecoration;
                        if (vd.ContainsAudio() == true)
                        {
                            hasAudio = true;
                        }

                        if (vd.Player.HasDolbyAC3Sound() == true)
                        {
                            hasAc3 = true;
                        }

                        hasVideos = true;
                    }
                }
            }

            return hasVideos;
        }

        // ********************************************************************************************************
        public void ComboChangedSoInformStoryboardOfChange()
        {
            if (mManager == null) return;

            mManager.mCurrentSlideShow.InValidateLength();

            if (mManager.mCurrentSlideShow.LoopMusic == true)
            {
                mManager.mCurrentSlideShow.ReCalcLoopMusicSlides();
            }
            Form1.mMainForm.GetSlideShowMusicManager().RebuildPanel();
            Form1.mMainForm.GetSlideShowNarrationManager().RebuildPanel();
            Form1.mMainForm.GetSlideShowManager().RebuildAllLabelTimes();
            Form1.mMainForm.GetDecorationManager().InformOfSlideLengthChange();
        }


        // ********************************************************************************************************
        public void SplitIntoEditableSlides()
        {
            if (mSlide == null) return;

            CMultiSlideSlide mss =mSlide as CMultiSlideSlide;
            if (mss != null)
            {
                mManager.StopIfPlayingAndWaitForCompletion();
                Form1.mMainForm.GoToMainMenu();
                mManager.UnHighlightAllThumbnails(false);
                mManager.mCurrentSlideShow.ConvertMultiSlideSlideToSlides(mss);
                mManager.RebuildPanel();

                CGlobals.mCurrentProject.DeclareChange(true, "Slide split");
            }
        }

        //*******************************************************************
        public bool ShowSetSlideLengthToVideoLengthOption(CVideoDecoration dec)
        {
            if (dec == null || mSlide == null)
            {
                return false;
            }

            if (Math.Abs(mSlide.DisplayLength - ((float)dec.GetTrimmedVideoDurationInSeconds())) > 0.001)
            {
                return true;
            }

            return false;
        }
	}
}
