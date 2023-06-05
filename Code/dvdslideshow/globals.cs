using System;
using System.Drawing;
using MangedToUnManagedWrapper;
using System.Xml;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using ManagedCore;
using System.Windows.Forms;
using System.Globalization;
using System.Security.Principal;
using System.Threading;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for globals.
	/// </summary>
	/// 


	public enum VideoGenerateMode
	{
		EDIT_PRIVIEW_MODE,
		FINAL_OUTPUT_MODE
	};
	
	public class CGlobals
	{
		public enum VideoType
		{
			DVD,
			SVCD,
			VCD,
            MP4,
            BLURAY
		}

        public enum MP4Quality
        {
            Low,
            Good,
            High,
            Maximum

        }

		public enum OutputTVStandard
		{
			PAL,
			NTSC
		};

		public enum OutputAspectRatio
		{
			TV4_3,
			TV16_9,
			TV221_1
		};

        // user folder (i.e. normally mydocuments/photovidshow) override needed for photocruz
        private static string OverrideUserDirectory = "";

		public static  bool mDoDVDPadding = true;

		public static string mCompanyName ="Squidgy Soft";
		public static string mPurchaseURLLink = "www.photovidshow.com/purchase";
		public static string mWebSite ="www.photovidshow.com";
		public static string mVersionString = "4.5";
		public static string mVersionBuild ="1";

		public static bool ContinuePlayingAudioAfterSeeking=false;

		public static string CompleteVersionString()
		{
			return "Version "+mVersionString+"."+mVersionBuild;
		}

		public static double VersionNumber()
		{
			return ((double.Parse(mVersionString,CultureInfo.InvariantCulture)) * 100000) +
				(double.Parse(mVersionBuild,CultureInfo.InvariantCulture));
		}

		public static bool IsEncoding()
		{
			return CGlobals.mVideoGenerateMode == VideoGenerateMode.FINAL_OUTPUT_MODE;
		}

		public static string copywright_string = "Copyright (C)2004-2018";
		public static bool mShowErrorStack=false;
		public static CProjectAuthor mCurrentPA=null;
		public static bool MainMenuNeedsReRender=true;
		public static bool mIsDebug = false;
        public static bool mIsTemplateUser = false; // extra features available in Gui, that allows creation of templates
	
		public static InterpolationMode mDefaultInterpolationMode = InterpolationMode.Bilinear;

		public static float mLimitVideoSeconds = -1;	// less than 0 means don't limit (debug stuff)
		public static bool mEncodeAudio = true; // for debug purposes.

		public static double mBurnLeadInandOutTime =44.0f;
		public static string mOriginalFilesZipFilename ="OriginalFiles";
		public static CSlide BlankSlide = new CSlide();
		public static float FadeInTime = 0.5f ;
		public static float FadeOutTime = 1.0f ;
		public static float FadeInAudioTime = 4.5f ;
		public static float FadeOutAudioTime = 4.5f ;	
        public static int mMaxTemplateNumbers = 40;

	
		// getvideo configurations
		public static bool VideoSeeking = false ;
		public static bool WaitVideoSeekCompletion = false ;
		public static bool MuteSound = false;
		public static bool PlayVideoRealTime = true;    // if false means we are frame stepping (i.e. encoding)

		public static CProject mCurrentProject = null ;

		public static bool mIsRecordMode = true;		// i.e. slideshow.getvideo is being dumped to disdk rather than some preview thing
 
		public static string mDefaultMenuTemplate = "";

		public static VideoGenerateMode	mVideoGenerateMode = VideoGenerateMode.EDIT_PRIVIEW_MODE;

		public static int mCurrentProcessFrame =0;
        public static int mCurrentProcessSubFrame = 0;
        public static int mCurrentProcessSubFrameTotal = 1;
        public static int mCurrentProcessSlide = 0;

        private static bool mRunningSteamVersion = false;
        public static bool RunningSteamVersion
        {
            get { return mRunningSteamVersion; }
            set { mRunningSteamVersion = value; }
        }    

        private static bool mRunningSteamDemo = false;
        public static bool RunningSteamDemo
        {
            get { return mRunningSteamDemo; }
            set { mRunningSteamDemo = value; }
        }

        private static bool mRunningWindowsStoreVersion = false;
        public static bool RunningWindowsStoreVersion
        {
            get { return mRunningWindowsStoreVersion; }
            set { mRunningWindowsStoreVersion = value; }
        }

        private static bool mRunningWindowsStoreTrial = false;
        public static bool RunningWindowsStoreTrial
        {
            get { return mRunningWindowsStoreTrial; }
            set { mRunningWindowsStoreTrial = value; }
        }

        private static bool mRunningPhotoCruz = false;

        public static bool RunningPhotoCruz
        {
            get { return mRunningPhotoCruz; }
            set { mRunningPhotoCruz = value; }
        }
           
		public CGlobals()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        // Show a debug box around all text decorations to represent drawn area
        private static bool mShowTextBoundary = false;
        public static bool ShowTextBoundary
        { 
           get { return  mShowTextBoundary ; }
           set { mShowTextBoundary = value ;}
        }

        static public Rectangle ToRectangle(RectangleF f)
        {
            return ToRectangle(f.X, f.Y, f.Width, f.Height);
        }

		static public Rectangle ToRectangle(float X, float Y, float Width, float Height)
		{
			Rectangle r = new Rectangle();
			r.X =(int) (X +0.499f);
			r.Y =(int) (Y +0.499f);
			r.Width = (int) (Width +0.499f );
			r.Height = (int) (Height +0.499f) ;
            if (X != 0.0 && r.X == 0) r.X = 1;
            if (Y != 0.0 && r.Y == 0) r.Y = 1;
            if (Width != 0.0 && r.Width == 0) r.Width = 1;
            if (Height != 0.0 && r.Height == 0) r.Height = 1;
			return r;
		}

        // Enlarge a rectangle, by w and h pixels, but crop to 0,0, max_w, max_h
        static public void EnlargeRectangle(ref Rectangle r1, int w, int h, int max_w, int max_h)
        {
            r1.X -= w; if (r1.X < 0) r1.X = 0;
            r1.Y -= h; if (r1.Y < 0) r1.Y = 0;
            r1.Width += (w << 1); if (r1.X + r1.Width > max_w) r1.Width = max_w - r1.X;
            r1.Height += (h << 1); if (r1.Y + r1.Height > max_h) r1.Height = max_h - r1.Y;
        }

        static public  float DeltaClamp(float value)
        {
            if (value > 1) value = 1;
            if (value < 0) value = 0;
            return value;
        }

        static public int Clamp(int value, int min, int max)
        {
            if (value > max) value = max;
            if (value < min) value = min;
            return value;
        }

        static public float ClampF(float value, float  min, float max)
        {
            if (value > max) value = max;
            if (value < min) value = min;
            return value;
        }

        static float dpix = 96;
        static float dpiy = 96;

        static public float dpiX
        {
            set { dpix = value ; }
            get { return dpix; }
        }

        static public float dpiY
        {
            set { dpiy = value ; }
            get { return dpiy; }
        }

		static public void SavePointF(XmlElement element, XmlDocument doc, string a, PointF p)
		{
			XmlElement point = doc.CreateElement(a);
			point.SetAttribute("X",p.X.ToString());
			point.SetAttribute("Y",p.Y.ToString());
			element.AppendChild(point);
		}

        static public void Save3dPointF(XmlElement element, XmlDocument doc, string a, PointF p, float z)
        {
            XmlElement point = doc.CreateElement(a);
            point.SetAttribute("X", p.X.ToString());
            point.SetAttribute("Y", p.Y.ToString());
            point.SetAttribute("Z", z.ToString());
            element.AppendChild(point);
        }


		static public void SaveRectangle(XmlElement element, XmlDocument doc, string a, RectangleF r)
		{
			XmlElement rectangle = doc.CreateElement(a);

			rectangle.SetAttribute("X",r.X.ToString());
			rectangle.SetAttribute("Y",r.Y.ToString());
			rectangle.SetAttribute("Width",r.Width.ToString());
			rectangle.SetAttribute("Height",r.Height.ToString());

			element.AppendChild(rectangle);
		}


		static public PointF LoadPointF(XmlElement element,string name)
		{
			PointF p = new PointF();
			XmlNodeList list = element.GetElementsByTagName(name);
			if (list.Count <1) 
			{
				CDebugLog.GetInstance().Error("Could not load PointF from xml element :"+name);
				return p;
			}

			XmlElement e = list[0] as XmlElement;

			p.X = float.Parse(e.GetAttribute("X"),CultureInfo.InvariantCulture);
			p.Y = float.Parse(e.GetAttribute("Y"),CultureInfo.InvariantCulture);
		
			return p;

		}

        static public PointF Load3dPointF(XmlElement element, string name, ref float z)
        {
            PointF p = new PointF();
            z=0;
            XmlNodeList list = element.GetElementsByTagName(name);
            if (list.Count < 1)
            {
                CDebugLog.GetInstance().Error("Could not load 3dPointF from xml element :" + name);
                return p;
            }

            XmlElement e = list[0] as XmlElement;

            p.X = float.Parse(e.GetAttribute("X"), CultureInfo.InvariantCulture);
            p.Y = float.Parse(e.GetAttribute("Y"), CultureInfo.InvariantCulture);

            string ss = e.GetAttribute("Z");
            if (ss != "")
            {
                z = float.Parse(ss, CultureInfo.InvariantCulture);
            }

            return p;
        }




		static public RectangleF LoadRectangle(XmlElement e)
		{

			RectangleF r = new RectangleF();
			r.X = float.Parse(e.GetAttribute("X"),CultureInfo.InvariantCulture);
			r.Y = float.Parse(e.GetAttribute("Y"),CultureInfo.InvariantCulture);
			r.Width = float.Parse(e.GetAttribute("Width"),CultureInfo.InvariantCulture);
			r.Height = float.Parse(e.GetAttribute("Height"),CultureInfo.InvariantCulture);

			return r;
		}
	
		static public RectangleF LoadRectangle(XmlElement element,string name)
		{
			XmlNodeList list = element.GetElementsByTagName(name);
			if (list.Count <1) 
			{
                CDebugLog.GetInstance().Error("Could not load rectangle from xml element :" + name);
				return  new RectangleF();
			}

			XmlElement e = list[0] as XmlElement;

			return LoadRectangle(e);
		}


		public static StringAlignment ParseAlignment(string a)
		{
			if (a=="Far") return StringAlignment.Far;
			if (a=="Center") return StringAlignment.Center;
			if (a=="Near") return StringAlignment.Near;
			if (a=="NEar") return StringAlignment.Near;
			CDebugLog.GetInstance().Error("Unknown alignment type :"+a);
			return StringAlignment.Near;;
		}


		public static Color ParseColor(string name)
		{
			//	Color [A=128, R=0, G=0, B=0]
			Color c = new Color();
			
			name = name.Replace("]","");

			string [] cc = name.Split(',');

			

			if (cc.Length !=4)
			{
				// try as name;
				name = name.Replace("Color [","");
				c = Color.FromName(name);
				return c;
			}

			int A = int.Parse(cc[0].Substring(cc[0].IndexOf("A=")+2));
			int R = int.Parse(cc[1].Substring(cc[1].IndexOf("R=")+2));
			int G = int.Parse(cc[2].Substring(cc[2].IndexOf("G=")+2));
			int B = int.Parse(cc[3].Substring(cc[3].IndexOf("B=")+2));

			c = Color.FromArgb(A,R,G,B);

			return c;
		}

		public static void SetGPQuality(Graphics gp)
		{

            gp.InterpolationMode = InterpolationMode.Low;
            gp.CompositingQuality = CompositingQuality.HighSpeed;
            gp.SmoothingMode = SmoothingMode.HighSpeed;
            gp.PixelOffsetMode = PixelOffsetMode.HighSpeed;

            /*
			if (CGlobals.mVideoGenerateMode == VideoGenerateMode.FINAL_OUTPUT_MODE)
			{
			//	gp.CompositingQuality = CompositingQuality.HighQuality ;
			//	gp.SmoothingMode = SmoothingMode.HighQuality ;

                gp.SmoothingMode = SmoothingMode.HighSpeed;
                gp.CompositingQuality = CompositingQuality.HighSpeed;

				// bilnear looks just as ok and bicubic
           		gp.InterpolationMode = InterpolationMode.HighQualityBicubic ;
			}
			else
			{
                gp.InterpolationMode = mDefaultInterpolationMode;
				gp.SmoothingMode=  SmoothingMode.HighSpeed;
				gp.CompositingQuality = CompositingQuality.HighSpeed;
			}
             */
		}


		public static bool IsImageFilename(string filename)
		{
			string s = filename.ToLower(); 
			if (s.EndsWith(".jpg") ||
				s.EndsWith(".jpeg") ||
				s.EndsWith(".bmp") ||
				s.EndsWith(".png") ||
				s.EndsWith(".wmf") ||
				s.EndsWith(".gif") ||
				s.EndsWith(".dib") ||
				s.EndsWith(".tif") ||
				s.EndsWith(".tiff") ||
				s.EndsWith(".emf") ||
                s.EndsWith(".pvsi") )
			{
				return true ;
			}
			return false ;
		}

		public static string GetClipartDirector()
		{
			return CGlobals.GetRootDirectory() +@"\clipart";
		}

		public static bool IsVideoFilename(string filename)
		{ 
			string s = filename.ToLower(); 
			if (s.EndsWith(".avi") ||
                s.EndsWith(".mob") ||
                s.EndsWith(".mov") ||
				s.EndsWith(".mpg") ||
				s.EndsWith(".mpeg") ||
				s.EndsWith(".mpg2") ||
                s.EndsWith(".mp4") ||
				s.EndsWith(".mpeg2") ||
				s.EndsWith(".pmg") ||
				s.EndsWith(".m1v") ||
				s.EndsWith(".mp2") ||
				s.EndsWith(".mp2v") ||
				s.EndsWith(".mpe") ||
				s.EndsWith(".mpv2") ||
				s.EndsWith(".wm") ||
				s.EndsWith(".wmv") ||
				s.EndsWith(".asf") ||
                s.EndsWith(".mts") ||
                s.EndsWith(".pvsv") )
			{
				return true ;
			}
			return false ;
		}


		public static string GetAudioFileDialogFilter()
		{
            return "Audio files (*.mp3;*.wav;*.wma;*.m4a;*.aif;*.aiff;*.aifc;*.snd;*.au;*.mpa;*.mp2;*.asf)|*.mp3;*.wav;*.wma;*.m4a;*.aif;*.aiff;*.aifc;*.snd;*.au;*.mpa;*.mp2;*.asf";
		}

		public static string GetImageAndVideoFileDialogFilter()
		{
			return "All image/video files (*.bmp;*.gif;*.jpg;*.jpeg;*.tif;*.tiff;*.dib;*.png;*.wmf;*.emf;*.pvsi;*.avi;*.mpg;*.mpeg;*.mpg;*.mpeg2;*.mov;*.mp4;*.pmg;*.m1v;*.mp2;*.mp2v;*.mpe;*.mpv2;*.wm;*.wmv;*.asf;*.mts;*.m2ts;*.mkv;*.pvsv)|*.bmp;*.gif;*.jpg;*.jpeg;*.tif;*.tiff;*.dib;*.png;*.wmf;*.emf;*.pvsv;*.avi;*.mpg;*.mpeg;*.mpg;*.mpeg2;*.mov;*.mp4;*.pmg;*.m1v;*.mp2;*.mp2v;*.mpe;*.mpv2;*.wm;*.wmv;*.asf;*.mts;*.m2ts;*.mkv;*.pvsi";
		}

		public static string GetImageOnlyFileDialogFilter()
		{
			return "Image files (*.bmp;*.gif;*.jpg;*.jpeg;*.tif;*.tiff;*.dib;*.png;*.wmf;*.emf;*.pvsi)|*.bmp;*.gif;*.jpg;*.jpeg;*.tif;*.tiff;*.dib;*.png;*.wmf;*.emf;*.pvsi";
		}

		public static string GetVideoOnlyFileDialogFilter()
		{
            return "Video files (*.avi;*.mpg;*.mpeg;*.mpg;*.mpeg2;*.mov;*.mp4;*.pmg;*.m1v;*.mp2;*.mp2v;*.mpe;*.mpv2;*.wm;*.wmv;*.asf;*.mts;*.m2ts;*.mkv;*.pvsv)|*.avi;*.mpg;*.mpeg;*.mpg;*.mpeg2;*.mov;*.mp4;*.pmg;*.m1v;*.mp2;*.mp2v;*.mpe;*.mpv2;*.wm;*.wmv;*.asf;*.mts;*.m2ts;*.mkv;*.pvsv" ;
		}

		public static string GetTotalImageVideoFilter()
		{
			return CGlobals.GetImageAndVideoFileDialogFilter()+"|"+
				   CGlobals.GetImageOnlyFileDialogFilter() +"|"+
				   CGlobals.GetVideoOnlyFileDialogFilter() + "|All files (*.*)|*.*";
		}

		public static string GetTotalAudioFilter()
		{
			return CGlobals.GetAudioFileDialogFilter() + "|All files (*.*)|*.*";
		}

		public static bool IsMusicFilename(string filename)
		{
			string s = filename.ToLower(); 
			if (s.EndsWith(".mp3") ||
				s.EndsWith(".wav") ||
				s.EndsWith(".aif") ||
				s.EndsWith(".aiff") ||
				s.EndsWith(".aifc") ||
				s.EndsWith(".snd") ||
				s.EndsWith(".au") ||
				s.EndsWith(".mpa") ||
				s.EndsWith(".mp2") ||
				s.EndsWith(".wma") ||
				s.EndsWith(".asf") ||
				s.EndsWith(".m4a") )
			{
				return true ;
			}
			return false ;
		}
		
        public static string GetAnimatedDecorationsEffectsDirectory()
        {
            return GetRootDirectory() + @"\move effects";
        }

        public static string GetTemplateDirectory()
        {
            return GetRootDirectory() + @"\templates";
        }

        public static string GetFiltersDirectory()
        {
            return GetRootDirectory() + @"\filters";
        }

        public static string GetBordersDirectory()
        {
            return GetRootDirectory() + @"\borders";
        }

        public static string GetBackgroundsDirectory()
        {
            return GetRootDirectory() + @"\menutemplates";
        }

        public static string GetAlphaMapDirectory()
        {
            return GetRootDirectory() + @"\outlinemasks";
        }

		public static string GetTempDirectory()
		{
            return GetUserDirectory() + @"\temp";
		}

        public static string GetDownloadMediaDirectory()
        {
            return ManagedCore.IO.GetDownloadMediaDirectory();
        }

		public static string GetUserDirectory()
		{
            if (OverrideUserDirectory !="") return OverrideUserDirectory;
            return ManagedCore.IO.GetUserDirectory();
		}

        public static void SetUserDirectory(string val)
        {
            OverrideUserDirectory = val;
        }
              



		public static string GetGuiImagesDirectory()
		{
			return GetRootDirectory() + @"\Guiimages";
		}


        public static void SetUpDirectories(string root)
        {
            SetUpDirectories(root, true);
        }


        public static string GetRootDirectory()
        {
            return ManagedCore.IO.GetRootDirectory();
        }

        // if a filename is relative from photovidshow, return full pathname (i.e. needed for XP systems)
        public static string GetFullPathFilename(string filename)
        {
            if (filename.StartsWith(@"\") || filename.StartsWith(@"/") || System.IO.Path.IsPathRooted(filename) == false)
            {
                filename = filename.TrimStart('\\');
                filename = filename.TrimStart('/');

                filename = CGlobals.GetRootDirectory() + "\\" + filename;
            }
            return filename;
        }

        public static void SetUpDirectories(string root, bool makeProjetsFolder)
        {
			ManagedCore.IO.SetRootDirectory(root);
			
			// set up temp;

			try 
			{
				string temp = GetTempDirectory();

				MangedToUnManagedWrapper.CManagedMemoryBufferCache.set_temp_directory(temp);

				if (System.IO.Directory.Exists(temp)==false)
				{
					System.IO.Directory.CreateDirectory(temp);
				}

				temp =GetTempDirectory()+"\\vcdfolder";

				if (System.IO.Directory.Exists(temp)==false)
				{
					System.IO.Directory.CreateDirectory(temp);
				}
                 
				temp =GetTempDirectory()+"\\dvdfolder";

				if (System.IO.Directory.Exists(temp)==false)
				{
					System.IO.Directory.CreateDirectory(temp);
				}

                temp = GetTempDirectory() + "\\blurayfolder";

                if (System.IO.Directory.Exists(temp) == false)
                {
                    System.IO.Directory.CreateDirectory(temp);
                }

				temp = CGlobals.GetUserDirectory();

				if (System.IO.Directory.Exists(temp)==false)
				{
					System.IO.Directory.CreateDirectory(temp);
				}

                temp = GetUserDirectory() + @"\Projects";

                if (makeProjetsFolder== true && System.IO.Directory.Exists(temp) == false)
                {
                    System.IO.Directory.CreateDirectory(temp);
                }

                temp =  GetUserDirectory() + @"\Authored";
                if (System.IO.Directory.Exists(temp)==false)
				{
					System.IO.Directory.CreateDirectory(temp);
				}

                temp = CGlobals.GetDownloadMediaDirectory();
                if (System.IO.Directory.Exists(temp) == false)
                {
                    System.IO.Directory.CreateDirectory(temp);
                }

                temp = CGlobals.GetDownloadMediaDirectory() + "\\borders";
                if (System.IO.Directory.Exists(temp) == false)
                {
                    System.IO.Directory.CreateDirectory(temp);
                }
                
                temp = CGlobals.GetDownloadMediaDirectory() + "\\menutemplates";
                if (System.IO.Directory.Exists(temp) == false)
                {
                    System.IO.Directory.CreateDirectory(temp);
                }
                
                temp = CGlobals.GetDownloadMediaDirectory() + "\\filters";
                if (System.IO.Directory.Exists(temp) == false)
                {
                    System.IO.Directory.CreateDirectory(temp);
                }
			}
			catch(Exception e2)
			{
				CDebugLog.GetInstance().Error("Setting up the initial directories threw an exception :"+e2.Message);
			}
		}

        //******************************************************************************************************
		public static string CheckFileExistsElseThrow(string filename)
		{
			bool exists = File.Exists(filename);
			if (exists == false)
			{
				string nf = ManagedCore.MissingFilesDatabase.GetInstance().SearchForNewFileLink(filename);

				if (nf==null)
				{
					throw new MissingFileException(filename);
				}
                else if (nf == "alreadydeclaredmissing")  // project contains same missing file more than once
                {
                    throw new IgnoreOperationException();
                }
				else if (nf.StartsWith("Remove")==true)
				{
					throw new IgnoreOperationException();
				}
				
                // assume nf contains the new location of the missing file, but double check
                if (File.Exists(nf) == false)
                {
                    throw new MissingFileException(filename);
                }

				filename = nf;
				
			}

			return filename;
		}

		
        //*******************************************************************
        public static Rectangle CalcBestFitRectagle(Rectangle input, Rectangle raw)
        {
            return CalcBestFitRectagle(input, raw, true);
        }

		//*******************************************************************
		public static Rectangle CalcBestFitRectagle(Rectangle input, Rectangle raw, bool allowMinorStretch)
		{	
			// make us fit in a rectangle region ( i.e reduce size but keep same aspect ratio)

			Rectangle r = input;
					
			// calc bigger

			float width_rat = ((float)input.Width) / raw.Width ;
			float height_rat = ((float)input.Height) / raw.Height ;


			// if image aspect ratio within 1% of desried ratio then 
			// allow a bit of strech
			if (allowMinorStretch &&
                width_rat > 0.95f && width_rat < 1.05f &&
				height_rat > 0.95f && height_rat < 1.05f)
			{
				return r;
			}

			if (width_rat < height_rat)
			{
				float nw = raw.Width * width_rat;
				float nh = raw.Height * width_rat ;
				r.Y = (int)((((float)input.Height) - nh)/2.0f);
				r.X = 0;
				r.Width = (int)nw;
				r.Height = (int)nh;
			}
			else
			{
				float nw = raw.Width * height_rat;
				float nh = raw.Height * height_rat ;
				r.X = (int)((((float)input.Width) - nw)/2.0f);
				r.Y = 0;
				r.Width = (int)nw;
				r.Height = (int)nh;
			}
			return r;
		}

        //*******************************************************************
        // assume trying to fit into screen rectangle of 0,0,1,1
        public static RectangleF CalcBestFitZoomedRect(float outputAspect, float imageAspect)
        {    
            float slideAspect = 1 / outputAspect;

            RectangleF area = new RectangleF(0, 0, 1, 1);

            float correctImageAspect = imageAspect / slideAspect;

            if (imageAspect < slideAspect)
            {
                float w = 1;
                float h = w / correctImageAspect;
                area = new RectangleF(0, -((h - 1) / 2), w, h);
            }
            else
            {
                float h = 1;
                float w = h * correctImageAspect;
                area = new RectangleF(-((w - 1) / 2), 0, w, h);
            }

            return area;
        }

        //*******************************************************************
        public static RectangleF CalcBestFitRectagleF(RectangleF input, RectangleF raw)
        {
            return CalcBestFitRectagleF(input, raw, true);
        }

        //*******************************************************************
        public static RectangleF CalcBestFitRectagleF(RectangleF input, RectangleF raw, bool allowMinorStrech)
        {
            // make us fit in a rectangle region ( i.e reduce size but keep same aspect ratio)

            RectangleF r = input;

            // calc bigger

            float width_rat = ((float)input.Width) / raw.Width;
            float height_rat = ((float)input.Height) / raw.Height;


            // if image aspect ratio within 1% of desried ratio then 
            // allow a bit of strech
            if (width_rat > 0.95f && width_rat < 1.05f &&
                height_rat > 0.95f && height_rat < 1.05f && allowMinorStrech)
            {
                return r;
            }

            if (width_rat < height_rat)
            {
                float nw = raw.Width * width_rat;
                float nh = raw.Height * width_rat;
                r.Y = ((((float)input.Height) - nh) / 2.0f);
                r.X = 0;
                r.Width = nw;
                r.Height = nh;
            }
            else
            {
                float nw = raw.Width * height_rat;
                float nh = raw.Height * height_rat;
                r.X = ((((float)input.Width) - nw) / 2.0f);
                r.Y = 0;
                r.Width = nw;
                r.Height = nh;
            }

            r.X += input.X;
            r.Y += input.Y;

            return r;
        }

		//*******************************************************************
		// input is rectangle we need the raw to fit into
		// raw is the rectangle to fit into input rectangle
		// aspect is used to represent the true aspect ratio of input rectangle 
		// ( e.g. like svcd which has resolution of 480x480 but has aspect ratio of 
		// 4:3 when draw!)

		public static Rectangle CalcBestFitRectagle(Rectangle input, Rectangle raw, float input_true_aspect)
		{	
			// make us fit in a rectangle region ( i.e reduce size but keep same aspect ratio)

			Rectangle r = input;
			RectangleF rf = new RectangleF(r.X,r.Y,r.Width,r.Height);

			// ok first covert input rectangle into same aspect as input_true_aspect

			float trans = (input_true_aspect * rf.Width )/rf.Height;
			float inv_trans = 1/trans;

			// translate rf rectangle into its own warped space to it matches input_true_aspect in
			// real world

			rf.Height =  rf.Height * trans;
					
			// calc bigger

			float width_rat = ((float)rf.Width) / raw.Width ;
			float height_rat = ((float)rf.Height) / raw.Height ;

			// if image aspect ratio within 1% of desried ratio then 
			// allow a bit of strech
			if (width_rat > 0.95f && width_rat < 1.05f &&
				height_rat > 0.95f && height_rat < 1.05f)
			{
				// input rectangle and raw rectangle fit 1 to 1 

				return r;
			}

			if (width_rat < height_rat)
			{
				float nw = raw.Width * width_rat;
				float nh = raw.Height * width_rat ;
				r.Y = (int)  (inv_trans*((rf.Height - nh)/2.0f));
				r.X = 0;
				r.Width = (int)nw;
				r.Height = (int)(nh*inv_trans);
			}
			else
			{
				float nw = raw.Width * height_rat;
				float nh = raw.Height * height_rat ;
				r.X = (int) ((rf.Width - nw)/2.0f);
				r.Y = 0;
				r.Width = (int)nw;
				r.Height = (int)(nh*inv_trans);
			}
			return r;
		}

        //*******************************************************************
        public static int GetMaxVideosPerSlide()
        {
            return 2;
        }

		//*******************************************************************
		public static Rectangle CalcBestFitRectagleNoBlank(Rectangle input, Rectangle raw)
		{	
			// make us fit in a rectangle region ( i.e reduce size but keep same aspect ratio)

			Rectangle r = input;
					
			// calc bigger

			float width_rat = ((float)input.Width) / raw.Width ;
			float height_rat = ((float)input.Height) / raw.Height ;


			// if image aspect ratio within 1% of desried ratio then 
			// allow a bit of strech
			if (width_rat > 0.95f && width_rat < 1.05f &&
				height_rat > 0.95f && height_rat < 1.05f)
			{
				return r;
			}

			if (width_rat > height_rat)
			{
				float nw = raw.Width * width_rat;
				float nh = raw.Height * width_rat ;
				r.Y = (int)((((float)input.Height) - nh)/2.0f);
				r.X = 0;
				r.Width = (int)nw;
				r.Height = (int)nh;
			}
			else
			{
				float nw = raw.Width * height_rat;
				float nh = raw.Height * height_rat ;
				r.X = (int)((((float)input.Width) - nw)/2.0f);
				r.Y = 0;
				r.Width = (int)nw;
				r.Height = (int)nh;
			}
			return r;
		}

		public static int vol_ratio_to_dx9_dbloss(float ratio)
		{
			if (ratio <0.0f) ratio =0.0f;
			if (ratio >1.0f) ratio =1.0f;

			float db_loss = (1.0f - ratio) * 1.9f;
		    db_loss = 0.0f - ( ((float)Math.Pow(10.0f, db_loss)) * 150.0f) ;
			if (db_loss < -9800.0f) db_loss = -9800.0f;
			if (db_loss >0) db_loss = 0.0f;
			return (int) db_loss;
		}

        private static bool running_as_admin_checked = false;
        private static bool running_as_admin = false;

        public static bool RunningAsAdministrator()
        {
            if (running_as_admin_checked == false)
            {
                AppDomain.CurrentDomain.SetPrincipalPolicy(
                  PrincipalPolicy.WindowsPrincipal);
                // read current identity for this thread
                WindowsPrincipal principal = (WindowsPrincipal)Thread.CurrentPrincipal;
                WindowsIdentity identity = (WindowsIdentity)principal.Identity;
                // can the Administrator role be found?
                running_as_admin = principal.IsInRole(WindowsBuiltInRole.Administrator);
                running_as_admin_checked = true;
            }

            return running_as_admin;
        }

		public static void LoadProgramSettingsFile()
		{
			try
			{
				string filename =CGlobals.GetRootDirectory()+"\\settings.txt";
				StreamReader reader = File.OpenText(filename);
				string text = reader.ReadLine();
				while (text!=null)
				{
                    if (text == "templateuser")
                    {
                        CGlobals.mIsTemplateUser = true;
                    }

					if (text=="srgdebug") 
					{
						CGlobals.mIsDebug=true;
					}
					if (text=="showerrorstack")
					{
						CGlobals.mShowErrorStack = true;
					}

                    if (text.StartsWith("forcemajorversion="))
                    {
                        CGlobals.mVersionString = text.Substring(18);
                    }

					if (text.StartsWith("website="))
					{
						CGlobals.mWebSite = text.Substring(8);
					}

					if (text.StartsWith("nopadding"))
					{
						CGlobals.mDoDVDPadding = false;
					}

					if (text.StartsWith("purchaseurl="))
					{
						CGlobals.mPurchaseURLLink = text.Substring(12);
					}

                    if (text.StartsWith("steam"))
                    {
                        CGlobals.RunningSteamVersion = true;
                    }
                    if (text.StartsWith("windows"))
                    {
                        CGlobals.RunningWindowsStoreVersion = true;
                    }

                    if (text.StartsWith("companyname="))
					{
						CGlobals.mCompanyName = text.Substring(12);
					}

                    if (text.StartsWith("loglevel="))
                    {
                        string levelString = text.Substring(9);
                        CDebugLog.LoggingLevel level = CDebugLog.LoggingLevel.FatalError;
                        if (levelString == "error")
                        {
                            level = CDebugLog.LoggingLevel.Error;
                        }
                        else if (levelString == "warning")
                        {
                            level = CDebugLog.LoggingLevel.Warning;
                        }
                        else if (levelString == "info")
                        {
                            level = CDebugLog.LoggingLevel.Info;
                        }
                        else
                        {
                            CDebugLog.GetInstance().LogToLogFile("Log level not recognised: " + levelString);
                        }
                        CDebugLog.GetInstance().LogLevel = level;
                    }

					// srg majour changes have to be done within exe
				//	if (text.StartsWith("version="))
				//	{
				//		CGlobals.mVersionString = text.Substring(8);
				//	}

					// minor build eg. could just of changes a picture file
					if (text.StartsWith("build="))
					{
						CGlobals.mVersionBuild = text.Substring(6);
					}

					if (text.StartsWith("traceencode")==true)
					{
						ManagedCore.CDebugLog.GetInstance().TraceEncode=true;
					}

					if (text.StartsWith("url="))
					{
						ManagedCore.CNetFile.mUpdateURLList.Add(text.Substring(4));
					}

					text = reader.ReadLine();
				}

			}
			catch (Exception e)
			{
				CDebugLog.GetInstance().Error("Loading settings file threw an exception :"+e.Message);
			}
		}

        public static string StripRootHeader(string inputfilename)
        {
            return ManagedCore.IO.StripRootHeader(inputfilename);
        }

        public static string TimeStringFromSeconds(double seconds)
        {
            return TimeStringFromSeconds(seconds, true);
        }

        static public bool IsWinXPOrHigher()
        {
            OperatingSystem OS = Environment.OSVersion;
            return (OS.Platform == PlatformID.Win32NT) && ((OS.Version.Major > 5) || ((OS.Version.Major == 5) && (OS.Version.Minor >= 1)));
        }

        private static int mVistaorHigher = -1;
        static public bool IsWinVistaOrHigher()
        {
            if (mVistaorHigher == -1)
            {
                OperatingSystem OS = Environment.OSVersion;
                if ((OS.Platform == PlatformID.Win32NT) && (OS.Version.Major >= 6))
                {
                    mVistaorHigher=1;
                }
                else
                {
                    mVistaorHigher=0;
                }
            }

            return mVistaorHigher==1;
        }

        private static int mWin7orHigher = -1;
        static public bool IsWin7OrHigher()
        {
            if (mWin7orHigher == -1)
            {
                OperatingSystem OS = Environment.OSVersion;
                if ((OS.Platform == PlatformID.Win32NT) &&              
                    ((OS.Version.Major >= 6) && (OS.Version.Minor >= 1) ||
                      OS.Version.Major >= 7))
                {
                    mWin7orHigher = 1;
                }
                else
                {
                    mWin7orHigher = 0;
                }
            }

            return mWin7orHigher == 1;
        }


		public static string TimeStringFromSeconds(double seconds, bool miliseconds)
		{
			if (seconds <0) seconds=0;
			DateTime tt = new DateTime((long)(seconds*10000000.0f));

            string new_label_tt;
            if (miliseconds == true)
            {
			    new_label_tt = System.String.Format("{0:d2}:{1:d2}:{2:d2}.{3:d2}", tt.Hour,tt.Minute,tt.Second,tt.Millisecond/10);
            }
            else
            {
                new_label_tt = System.String.Format("{0:d2}:{1:d2}:{2:d2}", tt.Hour,tt.Minute,tt.Second);
            }
         

			return new_label_tt;
		}

        //*******************************************************************
        public static string GetSmoothedEdgeAlphaMapFilename()
        {
            if (CGlobals.RunningPhotoCruz == false)
            {
                return "outlinemasks\\smooth edge.jpg";
            }
            return "";
        }

  	   //*******************************************************************
	   // this method is used to set the current code position when encoding.
	   // this is useful if it locks up when enoding so we know the last thing 
	   // it did ( or last check point reached)
	    
		public static void DeclareEncodeCheckpoint(char c)
		{
			if (CGlobals.IsEncoding()==true) CDebugLog.GetInstance().DebugAddEncodePosition(c);
		}

       
        public static int MaxSlidesPerSlideshow
        {
            get { return 1000; }
        }
	}

}
