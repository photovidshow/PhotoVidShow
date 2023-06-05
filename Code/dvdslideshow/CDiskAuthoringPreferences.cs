using System;
using System.Xml;
using System.Drawing;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for CDiskAuthoringPreferences.
	/// </summary>
	/// 

	// used to store preferences
	public class CDiskAuthoringPreferences
	{
        /// <summary>
        /// Canvis means the dimension each frame is rendered in the graphics engine when encoding
        /// </summary>
        private int mCanvasWidth = 720; //512 ; // 1920; // 1024;   
        private int mCanvasHeight = 540; // 288; // 1080; // 576; 

        //
        // Canvis area used when editing, this should be same size as the window being painted to.
        //
        private static int mPreviewCanvisWidth = 656;
        private static int mPreviewCanvisHeight = 369;

		private CGlobals.OutputAspectRatio mOutputAspectRatio = CGlobals.OutputAspectRatio.TV16_9;
		private CGlobals.OutputTVStandard mOutputType = CGlobals.OutputTVStandard.NTSC;
		private CGlobals.VideoType mVideoType = CGlobals.VideoType.MP4;
        private CGlobals.MP4Quality mVideoQuality = CGlobals.MP4Quality.Good;

        // If VideoType set to custom we use these settings to determine encoder, width, height, fps
       // private CGlobals.CustomVideoType mCustomVideoType = CGlobals.CustomVideoType.MPEG4_H264;
        private int mCutsomVideoOutputWidth = 1280;
        private int mCustomVideoOutputHeight = 720;
        private int mCustomeVideoFPS = 30;

		private int mOutputAudioFrequency = 48000;

		private int		mMaxMBOfDiskType	= 4500;
		private bool	mStoreOriginalPhotos = true;
		private bool	mStoreOriginalVideos = false;
	
		private  float mOutputVideoFPS = 25.00f;
        private bool mTurnOffMotionBlur = false;
        private int mFinalDiskCropPercent = 20;

        public bool TurnOffMotionBlur
        {
            get { return mTurnOffMotionBlur; }
            set { mTurnOffMotionBlur = value; }
        }

        public void SetPreviewCanvisDimensions(int width, int height)
        {
            mPreviewCanvisWidth = width;
            mPreviewCanvisHeight = height;
        }

      //  public CGlobals.CustomVideoType CustomVideoType
      //  {
       //     set { mCustomVideoType = value; }
        //    get { return mCustomVideoType; }
      //  }

        public int FinalDiskCropPercent
        {
            get { return mFinalDiskCropPercent; }
            set { mFinalDiskCropPercent = value; }
        }

        public int CustomVideoOutputWidth
        {
            set { mCutsomVideoOutputWidth = value; }
            get { return mCutsomVideoOutputWidth; }
        }

        public int CustomVideoOutputHeight
        {
            set { mCustomVideoOutputHeight = value; }
            get { return mCustomVideoOutputHeight; }
        }

        public int CustomVideoFPS
        {
            set { mCustomeVideoFPS = value; }
            get { return mCustomeVideoFPS; }
        }

        public float GetAspectRatioFraction()
        {
            if (OutputAspectRatio == CGlobals.OutputAspectRatio.TV16_9)
            {
                return 0.5625f;
            }
           
            return 0.75f;
        }

        public float AudioFramesPerSecond
        {
            get
            {
                // in final output mode do ntsc or pal settings
                if (CGlobals.mVideoGenerateMode == VideoGenerateMode.FINAL_OUTPUT_MODE)
                {
                    return mOutputVideoFPS;
                }
                return 60;
            }
        }

        public float OutputVideoFramesPerSecond
        {
            get
            {
                return mOutputVideoFPS;
            }
            set
            {
                mOutputVideoFPS = value;
            }
        }

        public CGlobals.MP4Quality OutputVideoQuality
        {
            get { return mVideoQuality; }
            set { mVideoQuality = value; }
        }


		public float frames_per_second
		{
			get
			{
				// in final output mode do ntsc or pal settings
				if (CGlobals.mVideoGenerateMode == VideoGenerateMode.FINAL_OUTPUT_MODE)
				{
                    return mOutputVideoFPS * RenderToEncodeFPSRatio;
				}

				// in edit mode fo the follow fps
				return 60;
			}
		}

        private int mRenderToEncodeFPSRatio = 1;



        public int RenderToEncodeFPSRatio
        {
            get
            {
                if (CGlobals.mVideoGenerateMode == VideoGenerateMode.FINAL_OUTPUT_MODE)
                {
                   //return 10;
                   return mRenderToEncodeFPSRatio;
                }

                return 1;
            }

            set
            {
                mRenderToEncodeFPSRatio = value;
            }
        }
	
        /// <summary>
        /// Output means the physical dimension of the output video being generated.
        /// For DVD,SVCD,VCD this may not actually be the same as the canvis dimension as this is packed in (ANAMORPHIC dvd image)
        /// For mpeg4 this should be the same size as the canvis
        /// </summary>
		private int mOutputVideoHeight = 540;
		private int mOutputVideoWidth = 720;


        public int OutputVideoHeight
        {
            get { return mOutputVideoHeight; }
        }

        public int OutputVideoWidth
        {
            get { return mOutputVideoWidth; }
        }

		public int AudioFrequency
		{
			get
			{
				return this.mOutputAudioFrequency;
			}
		}


		public int CanvasHeight
		{
			get 
			{ 
				if (	CGlobals.mVideoGenerateMode == VideoGenerateMode.FINAL_OUTPUT_MODE)
				{

                    return mCanvasHeight;
				 
				}

                return mPreviewCanvisHeight;
			}
		}


		public int CanvasWidth
		{
            get
            {
                if (CGlobals.mVideoGenerateMode == VideoGenerateMode.FINAL_OUTPUT_MODE)
                {

                    return mCanvasWidth;
                }

                return mPreviewCanvisWidth;
            }
		}


	    public CGlobals.OutputTVStandard TVType
		{
			get
			{
				return mOutputType;
			}
		}

		public  void SetToNTSC()
		{
            mOutputVideoFPS = 29.97f;
			mOutputType = CGlobals.OutputTVStandard.NTSC;
			OutputVideoType = mVideoType;	// reset output resoltions
		}

		public void SetToNTSC32Pulldown()
		{
            OutputVideoType = mVideoType;
            mOutputVideoFPS = 23.976f;
			mOutputType = CGlobals.OutputTVStandard.NTSC;
		
		}

		public  void SetToPAL()
		{
            mOutputVideoFPS = 25.0f;
			mOutputType =CGlobals.OutputTVStandard.PAL;
			OutputVideoType = mVideoType;	// reset output resoltions
		}

        // call this after setting OutputVideoType;
        public void SetToWidescreenTV16by9()
        {
            mOutputAspectRatio = CGlobals.OutputAspectRatio.TV16_9;

            if (mVideoType != CGlobals.VideoType.MP4 && mVideoType != CGlobals.VideoType.BLURAY)
            {
                mCanvasHeight = 576;     // ### SRG TODO what about svcd and vcd
                mCanvasWidth = 1024;
            }
        }

		public  void SetToNormalTV4by3()
		{
			mOutputAspectRatio=CGlobals.OutputAspectRatio.TV4_3;

            if (mVideoType != CGlobals.VideoType.MP4 && mVideoType != CGlobals.VideoType.BLURAY)
            {
                mCanvasHeight = 576;
                mCanvasWidth = 768;
            }
		}


		public  void SetToNormalTV221by1()
		{
			mOutputAspectRatio=CGlobals.OutputAspectRatio.TV221_1;

            if (mVideoType != CGlobals.VideoType.MP4)
            {
                mCanvasHeight = 576;
                mCanvasWidth = 1273;
            }
		}


		public CGlobals.OutputAspectRatio OutputAspectRatio
		{
			get
			{
				return mOutputAspectRatio;
			}
		}


	
        /// <summary>
        /// Sets or get the Burn Disk Type,  if setting it will setup outputvideo height and width
        /// </summary>
		public CGlobals.VideoType OutputVideoType
		{
			get { return mVideoType; } 
		    set 
			{
				mVideoType= value ; 
				if (mVideoType == CGlobals.VideoType.DVD) 
				{
					mMaxMBOfDiskType = 4500 ;
					mOutputAudioFrequency = 48000;

					if (this.mOutputType == CGlobals.OutputTVStandard.PAL)
					{
						mOutputVideoHeight = 576;
					    mOutputVideoWidth = 720;
                        mOutputVideoFPS = 25.0f;

					}
					else if (this.mOutputType == CGlobals.OutputTVStandard.NTSC)
					{
                        mOutputVideoHeight = 480;
                        mOutputVideoWidth = 720;
                        mOutputVideoFPS = 29.97f;
					}
				}

				if (mVideoType == CGlobals.VideoType.SVCD) 
				{
					mMaxMBOfDiskType = 700 ;
					mOutputAudioFrequency = 44100;
					if (this.mOutputType == CGlobals.OutputTVStandard.PAL)
					{
						mOutputVideoHeight = 576;
						mOutputVideoWidth = 480;
                        mOutputVideoFPS = 25.0f;
					}
					else if (this.mOutputType == CGlobals.OutputTVStandard.NTSC)
					{
						mOutputVideoHeight = 480;
						mOutputVideoWidth = 480;
                        mOutputVideoFPS = 29.97f;
					}
				}
				
				if (mVideoType == CGlobals.VideoType.VCD)
				{
					mMaxMBOfDiskType = 700 ;
					mOutputAudioFrequency = 44100;
					if (this.mOutputType == CGlobals.OutputTVStandard.PAL)
					{
						mOutputVideoHeight = 288;
						mOutputVideoWidth = 352;
                        mOutputVideoFPS = 25.0f;
					}
					else if (this.mOutputType == CGlobals.OutputTVStandard.NTSC)
					{
						mOutputVideoHeight = 240;
						mOutputVideoWidth = 352;
                        mOutputVideoFPS = 29.97f;
					}
				}

                if (mVideoType == CGlobals.VideoType.MP4)
                {
                    mMaxMBOfDiskType = 1000000000;
                    mOutputAudioFrequency = 48000;
                    mOutputVideoFPS = 30;
                    mOutputVideoHeight = mCustomVideoOutputHeight;
                    mOutputVideoWidth = mCutsomVideoOutputWidth;
                    mCanvasHeight = mCustomVideoOutputHeight;
                    mCanvasWidth = mCutsomVideoOutputWidth;

                    // ### SGR TODO SORT OUT TEXT GOES WRONG BELOW THESE RES so min is 640/360 in 16:9
                    if ( (mCanvasWidth==426 && mCanvasHeight==240) ||
                         (mCanvasWidth==432 && mCanvasHeight==240) ||
                         (mCanvasWidth==484 && mCanvasHeight==272) )
                    {
                        mCanvasWidth = 640;
                        mCanvasHeight = 360;
                    }
                }

                if (mVideoType == CGlobals.VideoType.BLURAY)
                {
                    mMaxMBOfDiskType = 25600;
                    mOutputAudioFrequency = 48000;
                    mOutputVideoHeight = mCustomVideoOutputHeight;
                    mOutputVideoWidth = mCutsomVideoOutputWidth;
                    mOutputVideoFPS = 24.0f;        // can be set from gui

                    Rectangle r = GetFinalDiskCropRectangle(mOutputVideoWidth, mOutputVideoHeight);

                    mCanvasHeight = r.Height;
                    mCanvasWidth = r.Width;
                }
			}
		}

		public int MaxMBOfDiskType
		{
			get { return mMaxMBOfDiskType ; }
		}


		public bool StoreOriginalPhotos
		{
			get { return mStoreOriginalPhotos; } set { mStoreOriginalPhotos = value ; }
		}


		public bool StoreOriginalVideos
		{
			get { return mStoreOriginalVideos; } set { mStoreOriginalVideos = value ; }
		}
		
		public CDiskAuthoringPreferences()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        //*******************************************************************
        // Returns the crop area rectangle of a given canvis width and height
        // after the final disk crop has been taken into account.
        public Rectangle GetFinalDiskCropRectangle(int canvisWidth, int canvisHeight)
        {
            float rw = ((float)canvisWidth) * (((float)mFinalDiskCropPercent) / 100) / 2.0f;
            float rh = ((float)canvisHeight) * (((float)mFinalDiskCropPercent) / 100) / 2.0f;
            rw += 0.5f;
            rh += 0.5f;

            int int_w = (int)rw;
            int int_h = (int)rh;

            Rectangle rec = new Rectangle(0, 0, canvisWidth, canvisHeight);
            rec.X += int_w;
            rec.Y += int_h;
            rec.Width -= int_w * 2;
            rec.Height -= int_h * 2;

            return rec;
        }

		//*******************************************************************
		public void Save(XmlElement parent, XmlDocument doc)
		{
			XmlElement preferences = doc.CreateElement("DiskPreferences");

			preferences.SetAttribute("DiskType",((int)this.mVideoType).ToString());
			preferences.SetAttribute("AspectRatio",((int)this.OutputAspectRatio).ToString());
			preferences.SetAttribute("OutputType",((int)this.mOutputType).ToString());
			preferences.SetAttribute("StoreOriginalPhotos",this.mStoreOriginalPhotos.ToString());
			preferences.SetAttribute("StoreOriginalVideos",this.mStoreOriginalVideos.ToString());


			parent.AppendChild(preferences); 
		}

		//*******************************************************************
		public void Load(XmlElement element)
		{
			this.OutputVideoType = (CGlobals.VideoType) int.Parse(element.GetAttribute("DiskType"));

            //
            // We no longer except SVCD or VCD (V4.5.3 onwards), if we have loaded a project of this type then change to DVD
            //
            if (OutputVideoType == CGlobals.VideoType.VCD ||
                OutputVideoType == CGlobals.VideoType.SVCD)
            {
                OutputVideoType = CGlobals.VideoType.DVD;
            }

            string s = element.GetAttribute("AspectRatio");
			if (s!="")
			{
				CGlobals.OutputAspectRatio or = (CGlobals.OutputAspectRatio) int.Parse(s);

				if (or == CGlobals.OutputAspectRatio.TV16_9)
				{
					this.SetToWidescreenTV16by9();
				}

				if (or == CGlobals.OutputAspectRatio.TV221_1)
				{
					this.SetToNormalTV221by1();
				}

				if (or == CGlobals.OutputAspectRatio.TV4_3)
				{
					this.SetToNormalTV4by3();
				}
			}
			else
			{
				this.SetToNormalTV4by3();
			}


			s = element.GetAttribute("OutputType");
			if (s!="")
			{
				CGlobals.OutputTVStandard t = (CGlobals.OutputTVStandard) int.Parse(s);

				if (t == CGlobals.OutputTVStandard.NTSC)
				{
					this.SetToNTSC();
				}
				else
				{
					this.SetToPAL();
				}
			}
			else
			{
				this.SetToNTSC();
			}

			this.StoreOriginalPhotos = bool.Parse(element.GetAttribute("StoreOriginalPhotos"));
			this.StoreOriginalVideos = bool.Parse(element.GetAttribute("StoreOriginalVideos"));
		}
	}
}
