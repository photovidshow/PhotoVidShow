using System;
using System.Collections;
using System.Xml;
using ManagedCore;
using DVDSlideshow;
using DVDSlideshow.Memento;
using System.Security.Permissions;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using DVDSlideshow.GraphicsEng;

namespace PhotoCruz
{ 
	/// <summary>
	/// Description of PCFilesToPDSFileConverter.	
	/// </summary>
	/// 

    //**************************************************************************
	public enum SlideType
	{
		FULL_SCREEN,
		COMMENTS,
		NO_COMMENTS
	}

    public enum ConverterType
    {
        PHOTOCRUZ_CONVERTER,
        LIVCREATOR_COVERTER
    }

	
    //**************************************************************************
    //**************************************************************************
    //**************************************************************************
    public class PhotoCruzErrorEx : Exception
    {
        public string mErrorString;
        public PhotoCruzErrorEx(string error) { mErrorString = error; }
    }
   

    // useful to keep track of our video slideshows;
    class VideoSlideShow
    {
        public int number;
        public CSlideShow ss;
        public int mm_number;      // 0 = main menu else sub sub menu
        public CMainMenu menu;
        public CMainMenu mSubMenu;
    }

    class SubMenu
    {
        public SubMenu(int num, CMainMenu menu)
        {
            mNum = num;
            mMenu = menu;
        }
        public int mNum;
        public CMainMenu mMenu;
    }

    //**************************************************************************
    //**************************************************************************
    //**************************************************************************

    [StrongNameIdentityPermissionAttribute(SecurityAction.LinkDemand, PublicKey = MyPublicKey.Value)]

	public class PCFilesToPDSFileConverter 
	{

		static int mSeed =-1;
        private ArrayList mVideoSlideShows = new ArrayList();
        private ArrayList mSubMenus = new ArrayList();

        private int mCurrentLayout = 0;
		private string mProjectPath;
        private SlideType mDefaultSlideType = SlideType.FULL_SCREEN;
		private string mThemeName ="";
        bool mHasILMSlide = false;
        public float mAspect = 4.3f;
        private string mIntroVideo;
        bool mHasScrollyPoem = false;
        bool mHasEndLogo = false;
        string mProjectName ="";
        string mRootPath = "";
        string mPreMenuFileName ="";
        CProject mCurrentProject = null;
        int mChapelLayout =-1;

        private ManagedCore.Progress.IProgressCallback mProgressCallback = null;

		private string mBirthDate;
		private string mDeathDate;
		private string mFirstName;
		private string mLastName;

        private bool mProjectHasChapelMenu = false;

        private ConverterType mConverterType;

        public bool HasChapelMenu()
        {
            return mProjectHasChapelMenu;
        }

		
		private CSlideShow mMainSlideShow = null;


	  //****************************************************************************
		public PCFilesToPDSFileConverter(string root_path,
                                         string project_path,
                                         string project_name,
                                         ConverterType type)
		{
		  mProjectPath = project_path;
          mProjectName = project_name;
          mRootPath = root_path;
          mConverterType = type;
		}

	  //****************************************************************************
        public string GetRootDirectory()
        {
            return mRootPath;
        }


	  //****************************************************************************
        public void ReadThemeFile()
        {
            StreamReader reader = null;
            string filename = this.mProjectPath + "\\files\\theme";
            try
		    {
			    reader = File.OpenText(filename);
		    }
		    catch
		    {
               throw new PhotoCruzErrorEx("Could not locate theme file");
		    }

            try
            {

                mThemeName = reader.ReadLine();
                string type = reader.ReadLine();
                mDefaultSlideType = this.ParseSlideType(type, mDefaultSlideType);
                string end_logo = reader.ReadLine().ToUpper();
                if (end_logo.Contains("NS") == true)
                {
                    this.mHasEndLogo = false;
                }
                else if (end_logo.Contains("S") == true)
                {
                    this.mHasEndLogo = true;
                }

                string scrolly = reader.ReadLine().ToUpper();
                if (scrolly.Contains("SC") == true)
                {
                 this.mHasScrollyPoem = true;
                }

                string ilm = reader.ReadLine().ToUpper();
                if (ilm.Contains("ILM") == true && 
                    ilm.Contains("NILM") == false)
                {
                    this.mHasILMSlide = true;
                }

                if (reader.EndOfStream == false)
                {
                    string aspect = reader.ReadLine().ToUpper();
                    if (aspect.Contains("16:9") == true)
                    {
                        this.mAspect = 16.9f;
                    }
                }
            }
            catch
            {
            }

            reader.Close();
        }


	   //****************************************************************************
 		public CProject Convert(ManagedCore.Progress.IProgressCallback progress_callback)
		{
            this.mProgressCallback = progress_callback;

         
			CProject project = new CProject();
            project.ForceNoMemento = true;
            project.DiskPreferences.SetToNormalTV4by3();
            project.DiskPreferences.SetToNTSC();
            CGlobals.mCurrentProject.DiskPreferences.OutputVideoType = CGlobals.VideoType.DVD;

            if (mConverterType == ConverterType.PHOTOCRUZ_CONVERTER)
            {
                ReadThemeFile();

                if (this.mAspect == 16.9f)
                {
                    project.DiskPreferences.SetToWidescreenTV16by9();
                    project.DiskPreferences.SetToNTSC();
                }

                GenerateMenus(project);

                project.mName = this.mProjectPath + "\\" + this.mProjectName + ".pds";
                project.Save();
            }
            else if (mConverterType == ConverterType.LIVCREATOR_COVERTER)
            {

                // test many menus

                /*
                int count =1;

                CMainMenu previous_menu = null;

                for (int i = 0; i < 120; i++)
                {
                    CStillPictureSlide sps = new CStillPictureSlide("c:\\blank.jpg");
                    sps.SetLengthWithoutUpdate(6);
                    CTextStyle ts = new CTextStyle();
                    ts.FontSize = 16;
                    ts.Format.Alignment = StringAlignment.Center;
                    RectangleF text_coverage = new RectangleF(0.5f, 0.13f, 0f, 0f);
                    CTextDecoration comments_dec = new CTextDecoration(count.ToString(), text_coverage, 0, ts);
                    comments_dec.mAttachedToSlideImage = true;
                    sps.AddDecoration(comments_dec);


                    ArrayList decs = new ArrayList();
                    CMainMenu mm = new CMainMenu(sps, decs);
                    mm.Length = 10;
                    mm.MusicSlide = null;


                    if (i == 0)
                    {
                        this.mCurrentProject = project;
                        mCurrentProject.MainMenu = mm;
                        previous_menu = mm;
                    }
                    else
                    {
                        previous_menu.AddSubMenu(mm);
                        previous_menu = mm;
                    }

                    CSlideShow ss = new CSlideShow(count.ToString());
                    ArrayList slides = new ArrayList();
                    slides.Add(sps);

                    ss.AddSlides(slides);

                    this.mCurrentProject.AddSlideshow(ss, mm);


                    count++;
                }


                project.mName = this.mProjectPath + "\\" + this.mProjectName + ".pds";
                project.Save();
                */
                

                CImage i = new CImage(new Bitmap(1, 1));

                CImageSlide s = new CStillPictureSlide(i);
                ArrayList decs = new ArrayList();
                CMainMenu mm = new CMainMenu(s, decs);
                mm.SetLinkStyle(0);
                this.mCurrentProject = project; 
                mCurrentProject.MainMenu = mm;
                CSlideShow ss = this.GenerateMainSlideshow(this.mCurrentProject, 0, 1);
                mMainSlideShow = ss;
                this.mCurrentProject.AddSlideshow(ss, this.mCurrentProject.MainMenu);
#if (DEBUG) 
                project.mName = this.mProjectPath + "\\" + this.mProjectName + ".pds";
                project.Save();
#endif
                 
            }

         
            return project;
		}
         

        private void AddIntroVideo(CSlideShow slideshow)
        {
            CImageSlide slide = CreateSlideFromFilename(mIntroVideo);

            ArrayList slides = new ArrayList();
            slides.Add(slide);

            slideshow.AddSlides(slides);
        }

	    //****************************************************************************
	    public CSlideShow GenerateMainSlideshow(CProject project, int mm, int number)
	    {
		    // open slides. doc

            ArrayList slide_lines = new ArrayList();

            CSlideShow ss = new CSlideShow("MainSlideshow");
             
            if (mIntroVideo != null)
            {
                AddIntroVideo(ss);
            }

		    // if ilm screen add it

		    if (mHasILMSlide)
		    {
			    AddILMSlide(ss);
		    }

		    // if scolly poem add that to

		    if (mHasScrollyPoem)
		    {
			    AddScrollySlide(ss);
		    }
    	
            StreamReader reader = null;

		    string filename = this.mProjectPath+"\\files\\slides";

            if (this.mConverterType == ConverterType.LIVCREATOR_COVERTER)
            {
                filename = this.mProjectPath+"\\files\\livcreatorslides";
                if (System.IO.File.Exists(filename) == false)
                {
                    filename = this.mProjectPath + "\\files\\slides";
                }
            }

		    try
		    {
			    reader = File.OpenText(filename);
		    }
		    catch
		    {
                if (this.mConverterType == ConverterType.LIVCREATOR_COVERTER)
                {
                    throw new PhotoCruzErrorEx("Could not locate files/slides file.");
                }

                ReadAndParseFilesInPhotoDirectory(ss);
		    }

            try
            {  
                if (reader != null)
                {
                    string text = reader.ReadLine();
                    while (text != null)
                    {
                        slide_lines.Add(text);
                        text = reader.ReadLine();
                    }
                }
            }
            catch (PhotoCruzErrorEx error)
            {
                throw error;
            }
            catch
            {
            }

            if (reader != null)
            {
                reader.Close();
            }


            try
            {
                SlideType last_slide_type = SlideType.FULL_SCREEN;
                int count=0;
                foreach(string line in slide_lines)
                {
                    if (mConverterType == ConverterType.PHOTOCRUZ_CONVERTER)
                    {
                        last_slide_type = ProcessSlideLinePhotoCruz(line, ss, last_slide_type);
                    }
                    else if (mConverterType == ConverterType.LIVCREATOR_COVERTER)
                    {
                        last_slide_type = ProcessSlideLineLivCreator(line, ss, last_slide_type);
                    }
                    
                    count++;
                    if (this.mProgressCallback != null)
                    {
                        float tt =  ((float)count) / ((float)slide_lines.Count);
                        tt *= 100.0f;
                        if (tt < 0) tt = 0;
                        if (tt > 100) tt = 100;
                        this.mProgressCallback.StepTo((int)tt);
                     
                    }
                }
            }
            catch (PhotoCruzErrorEx error)
            {
                throw error;
            }
            catch (Exception ee)
            {
                throw new PhotoCruzErrorEx(ee.Message);
            }

		    // if has end logo, add that too

		    if (mHasEndLogo)
		    {
			    AddEndLogoSlide(ss);
		    }
		    
		    if (mHasILMSlide)
		    {
			    AddILMSlide(ss);
		    }

            AddBackgroundMusic(ss);


            // set menu thumbnail if has intro video
            if (mIntroVideo != null)
            {
                ArrayList slides = ss.mSlides;
                if (slides.Count > 2)
                {
                    CSlide thumbnail_slide = slides[1] as CSlide;
                    if (thumbnail_slide != null)
                    {
                        ss.MenuThumbnailSlide = thumbnail_slide;
                    }
                }
            }

            return ss;

	    }

	   //****************************************************************************
       // Adds background music to main slideshow
        public void AddBackgroundMusic(CSlideShow ss)
        {
            StreamReader reader;
            ArrayList music_slides = new ArrayList();

            try
            {
                reader = File.OpenText(mProjectPath + "\\files\\bgmusic");
                string next_line = reader.ReadLine();
                while (next_line!=null && next_line != "")
                {
                    try
                    {
                        CMusicSlide ms = new CMusicSlide(this.GetRootDirectory() + "\\music\\" + next_line);
                        music_slides.Add(ms);
                    }
                    catch
                    {
                        // SRG ERROR COULD NOT LOAD FILE, BUT NOT FATAL
                    }

                    next_line = reader.ReadLine();
                }
               
                reader.Close();
            }
            catch
            {
                throw new PhotoCruzErrorEx("Could not locate bgmusic file");
            }

            try
            {
                ss.AddMusicSlides(music_slides);   
            }
            catch
            {
            }
        }

	   //****************************************************************************
       public void ReadAndParseFilesInPhotoDirectory(CSlideShow ss)
        {
            try
            {
                string[] files = System.IO.Directory.GetFiles(this.mProjectPath + "//photos");
                SlideType last_slide_type = SlideType.FULL_SCREEN;

                int count = 0;
                foreach (string file in files)
                {
                    if (CGlobals.IsImageFilename(file) ||
                        CGlobals.IsVideoFilename(file))
                    {
                        if (this.mConverterType == ConverterType.PHOTOCRUZ_CONVERTER)
                        {
                            last_slide_type = this.ProcessSlideLinePhotoCruz(System.IO.Path.GetFileName(file), ss, last_slide_type);
                        }
                        else
                        {
                            last_slide_type = this.ProcessSlideLineLivCreator(System.IO.Path.GetFileName(file), ss, last_slide_type);
                        }

                        count++;
                        if (this.mProgressCallback != null)
                        {
                            float tt = ((float)count) / ((float)files.Length);
                            tt *= 100.0f;
                            if (tt < 0) tt = 0;
                            if (tt > 100) tt = 100;
                            this.mProgressCallback.StepTo((int)tt);

                        }
                    }
                }
            }
            catch
            {
                throw new PhotoCruzErrorEx("Error while reading photos directory");
            }
        }

	   //****************************************************************************

        public CImageSlide CreateSlideFromFilename(string fn)
        {
            return CreateSlideFromFilename(fn, null);
        }

        private static int video_player_id = 1;

	    private CImageSlide CreateSlideFromFilename(string fn, CVideoSlide lastVideoSlide)
	    {
		    CImageSlide slide;

		    if (CGlobals.IsImageFilename(fn)== true)
		    {
			    slide = new CStillPictureSlide(fn);
		    }
		    else if (CGlobals.IsVideoFilename(fn)== true)
		    {
                if (lastVideoSlide != null)
                {
                    string reference = lastVideoSlide.ReferenceStringID;

                    if (reference == "")
                    {
                        if (lastVideoSlide.StringID == "")
                        {
                            lastVideoSlide.StringID = ".video" + video_player_id.ToString();
                            video_player_id++;
                        }
        
                        reference = lastVideoSlide.StringID;
                    }

                    slide = new CVideoSlide(reference);
                }
                else
                {
                    slide = new CVideoSlide(fn);
                }
		    }
		    else
		    {
		       throw new PhotoCruzErrorEx("Invalid file extension for file: ("+fn+")");
		    }

		    return slide;
	    }

	   //****************************************************************************
	    public CDecoration CreateDecorationForFilename(string fn, RectangleF coverage)
	    {
		    CDecoration decoration;

		    if (System.IO.File.Exists(fn)==false)
		    {
			    throw new PhotoCruzErrorEx("Can not locate file ("+fn+")");
		    }

		    if (CGlobals.IsImageFilename(fn)== true)
		    {
                decoration = new CClipArtDecoration(fn, coverage, 0);
		    }
		    else if (CGlobals.IsVideoFilename(fn)== true)
		    {
                CVideoDecoration vd = new CVideoDecoration(fn, coverage, 0);

                decoration = vd;

                decoration.AttachedToSlideImage = false;
		    }
		    else
		    {
		       throw new PhotoCruzErrorEx("Invalid file extension for file ("+fn+")");
		    }

		    return decoration;
	    }


	   //****************************************************************************
        public SlideType ParseSlideType(string name, SlideType default_type)
        {
            string upper = name.ToUpper();
            if (upper.Contains("NULL")) return default_type;
            if (upper.Contains("FS")) return SlideType.FULL_SCREEN;
            if (upper.Contains("NC")) return SlideType.NO_COMMENTS;
            if (upper.Contains("C")) return SlideType.COMMENTS;
         
            return default_type;
        }

	    //****************************************************************************
        public CTextStyle GenerateSlideTextStyle()
        {
            CTextStyle text_style = new CTextStyle();
            text_style.Bold = true;
            text_style.Format = new StringFormat();
            text_style.Format.Alignment = System.Drawing.StringAlignment.Center;
            text_style.FontName = "Microsoft Sans Serif";
            text_style.FontSize = 20;
            text_style.TextColor = Color.FromArgb(255, 48, 48, 48);

            return text_style;
        }

	   //****************************************************************************
        public CTextStyle GenerateMenuTextStyle()
        {
            CTextStyle text_style = new CTextStyle();
            text_style.Bold = true;
            text_style.Shadow = true;
            text_style.Format = new StringFormat();
            text_style.Format.Alignment = System.Drawing.StringAlignment.Center;
            text_style.FontName = "Microsoft Sans Serif";
            text_style.FontSize = 22;
            text_style.TextColor = Color.FromArgb(255, 255, 255, 255);

            return text_style;
        }

		//****************************************************************************
		private CTransitionEffect GenerateCorNCTransEffect()
		{
			float length = 1.5f;
            CTransitionEffect new_effect = new SimpleAlphaBlendTransitionEffect(length);
			if (mSeed==-1)
			{
				mSeed = DateTime.Now.Second * DateTime.Now.Minute;
			}

			Random r = new Random(mSeed);
			mSeed+=3433;
			int effect = r.Next()%14;

			if (effect==0) new_effect =  new SimpleAlphaBlendTransitionEffect(length);
            if (effect == 1) new_effect = new AlphaSwipTransitionEffect(length, AlphaSwipTransitionEffect.SwipeDirection.RIGHT, 11);
			if (effect==2) new_effect =  new AlphaSwipTransitionEffect(length,AlphaSwipTransitionEffect.SwipeDirection.LEFT,11);
			if (effect==3) new_effect =  new AlphaSwipTransitionEffect(length,AlphaSwipTransitionEffect.SwipeDirection.RIGHT,5);
			if (effect==4) new_effect =  new AlphaSwipTransitionEffect(length,AlphaSwipTransitionEffect.SwipeDirection.LEFT,5);
			if (effect==5) new_effect =  new FadeUpTransitionEffect(length,64);
			if (effect==6) new_effect =  new FadeDownTransitionEffect(length,64);
			if (effect==7) new_effect =  new FadeUpTransitionEffect(length,32);
			if (effect==8) new_effect =  new FadeDownTransitionEffect(length,32);
			if (effect==9) new_effect =  new RandomFizzTransition(length,16);
			if (effect==10) new_effect =  new CircleInTransitionEffect(length,32,1,1);
			if (effect==11) new_effect =  new CircleOutTransitionEffect(length,32,1,1);
			if (effect==12) new_effect =  new CircleInTransitionEffect(length,128,1,1);
			if (effect==13) new_effect =  new CircleOutTransitionEffect(length,128,1,1);

			return new_effect;
		}


        //****************************************************************************
		//
		// processes a line in the files/slides file
        //
		//****************************************************************************
        // PhotoCruz format = filename, layouttype, panzoom, comments, background
       
        public SlideType ProcessSlideLinePhotoCruz(string line, CSlideShow ss, SlideType last_slide_type)
	    {
		    string [] parameters = line.Split(',');

            if (parameters.Length <= 0) return SlideType.FULL_SCREEN;
            if (line == "") return SlideType.FULL_SCREEN;

            // is this a template
            if (parameters[0].Contains(".xml"))
            {
                return ProcessTemplateLine(parameters, ss);
            }

		    SlideType slide_type = mDefaultSlideType;
            PanZoomType pan_zoom_type = PanZoomType.RANDOM;
		    string background_img = "";
            string comments = "";
            double initial_delay = 0.0;
            double end_delay = 0.0;
            float music_volume = 1.0f;

			string data_file_file_name = parameters[0] ;

            string slidefn = this.mProjectPath + "\\photos\\" + data_file_file_name;

			// if path info is specified then this means file is from root directory
			// not photos directory
			if (data_file_file_name.Contains("/") ||
				data_file_file_name.Contains(@"\") )
			{
				slidefn = this.GetRootDirectory() + "\\"+ data_file_file_name;
			}

            if (System.IO.File.Exists(slidefn) == false) return SlideType.FULL_SCREEN;

		    if (parameters.Length >=2)
		    {
			    slide_type = ParseSlideType(parameters[1], slide_type);
		    }
    		
		    if (parameters.Length >=3)
		    {
			    pan_zoom_type = PanZoom.ParsePanZoomType(parameters[2], PanZoomType.RANDOM);
		    }

            if (parameters.Length >= 4)
            {
                comments = parameters[3];
            }

		    if (parameters.Length >=5)
		    {
                string b1 = parameters[4];
                b1 = b1.Trim();
			    background_img = this.GetRootDirectory() + "\\shared\\"+ this.mThemeName + "\\"+b1;
		    }

            // initial delay
            if (parameters.Length >= 6)
            {
                try
                {
                    initial_delay = double.Parse(parameters[5]);
                }
                catch { }
            }

            // end delay
            if (parameters.Length >= 7)
            {
                try
                {
                    end_delay = double.Parse(parameters[6]);
                }
                catch { }
            }


            // music volume
            if (parameters.Length >= 8)
            {
                try
                {
                    music_volume = float.Parse(parameters[7]);
                }
                catch { }
            }

		    // if full screen slide  =  slidefn
		    CImageSlide slide;
            RectangleF picture_placeholder = new RectangleF(0.3611111f, 0.1134259f, 0.5798611f, 0.7708333f);

            float slide_length = 10.0f;

            if (slide_type == SlideType.FULL_SCREEN)
		    {
			    slide = CreateSlideFromFilename(slidefn);

                CStillPictureSlide sps = slide as CStillPictureSlide;
                if (sps != null)
                {
                    sps.PanZoom = PanZoom.GeneratePhotoVidShowPanZoom(pan_zoom_type, sps.Image);
                }

                // set background image;

                if (background_img != "" &&
                    System.IO.File.Exists(background_img) == true)
                {
                    slide.BackgroundImage = new CImage(background_img, false);
                }

                CVideoSlide vs = slide as CVideoSlide;
                if (vs != null)
                {
                    vs.StartVideoOffset = initial_delay;
                    vs.EndVideoOffset = end_delay;
                    vs.MusicFadeWhilePlayingVideo = music_volume;
                    vs.EnableMusicFadeWhilePlayingVideo = true;
                }
		    }

		    // comments
		    // no comments
		    else
		    {
			    string pre_back_name = "nocomm";
                if (slide_type == SlideType.COMMENTS) pre_back_name = "comm";
    		
			    string backimage_fn = GetRootDirectory()+ "\\shared\\"+ mThemeName +"\\"+ mThemeName+ pre_back_name+".jpg";
			    if (System.IO.File.Exists(backimage_fn)==false)
			    {
                    backimage_fn = GetRootDirectory() + "\\shared\\" + mThemeName + "\\" + mThemeName + pre_back_name + ".mpg";
                    if (System.IO.File.Exists(backimage_fn) == false)
				    {
                        backimage_fn = GetRootDirectory() + "\\shared\\" + mThemeName + "\\" + mThemeName + pre_back_name + ".avi";
                        if (System.IO.File.Exists(backimage_fn) == false)
					    {
                            backimage_fn = GetRootDirectory() + "\\shared\\" + mThemeName + "\\" + mThemeName + pre_back_name + ".wmv";
                            if (System.IO.File.Exists(backimage_fn) == false)
                            {
                                backimage_fn = GetRootDirectory() + "\\shared\\" + mThemeName + "\\" + mThemeName + pre_back_name + ".png";
                                if (System.IO.File.Exists(backimage_fn) == false)
                                {
                                    throw new PhotoCruzErrorEx("Can not find back image for theme: " + mThemeName);
                                }
                            }
					    }
				    } 
			    }

                CImageSlide last_slide2 = null; 

                // reuse mplayer from last slide
                ArrayList current_slides2 = ss.mSlides;

                CVideoSlide lastVideoSlide = null ; 
                if (current_slides2.Count > 0)
                {
                    last_slide2 = (CImageSlide)current_slides2[current_slides2.Count - 1];

                    CVideoSlide last_slide_as_video_slide = last_slide2 as CVideoSlide;
                    if (last_slide_as_video_slide != null &&
                        last_slide_as_video_slide.Player != null)
                    {
                        if (last_slide_as_video_slide.Player.mFileName == backimage_fn)
                        {
                            lastVideoSlide = last_slide_as_video_slide;
                        }
                    }
                }

                slide = CreateSlideFromFilename(backimage_fn, lastVideoSlide);

                CVideoSlide vs = slide as CVideoSlide;
                if (vs != null)
                {
                    // ok make it a looped slide !!!
                    vs.Loop = true;
                }

                // slide has no PAN ZOOM if its not full screen
                CStillPictureSlide sps = slide as CStillPictureSlide;
                if (sps != null)
                {
                    sps.PanZoom = PanZoom.GeneratePhotoVidShowPanZoom(PanZoomType.NONE, null);
                }

			    // add other bits
                if (slide_type == SlideType.COMMENTS)
                {
                    picture_placeholder = new RectangleF(0.3836806f, 0.2152778f, 0.5486111f, 0.6782407f);
                }

                CDecoration picture_dec = GenerateImageDecorForPlaceHolder(slidefn, picture_placeholder);

                CVideoDecoration v_dec = picture_dec as CVideoDecoration;
                if (v_dec != null)
                {
                    slide_length = (float)v_dec.Player.GetDurationInSeconds();

                    slide_length -= (float)initial_delay;
                    slide_length -= (float)end_delay;

                    v_dec.StartVideoOffset = initial_delay;
                    v_dec.EndVideoOffset = end_delay;
                    v_dec.MusicFadeWhilePlayingVideo = music_volume;
                    v_dec.EnableMusicFadeWhilePlayingVideo = true;
                }
               
			    slide.AddDecoration(picture_dec);
           
                AddPersonPartcularsToSlide(slide, slide_type == SlideType.COMMENTS);

			    // if comments type slide add comments
                if (slide_type == SlideType.COMMENTS)
			    {
                    CTextStyle ts = GenerateSlideTextStyle();
                    ts.FontSize = 16;
                    ts.Format.Alignment = StringAlignment.Center;
                    RectangleF text_coverage = new RectangleF(0.5f,0.13f,0f,0f);
                    CTextDecoration comments_dec = new CTextDecoration(comments, text_coverage, 0, ts);
                    comments_dec.AttachedToSlideImage = true;
				    slide.AddDecoration(comments_dec);
			    }
		    }

            // set last slide transition effect

            bool do_video_match_up = false;
            CImageSlide last_slide = null; 

            ArrayList current_slides = ss.mSlides;
            if (current_slides.Count > 0)
            {
                last_slide = (CImageSlide) current_slides[current_slides.Count - 1];

                if (last_slide_type == SlideType.FULL_SCREEN ||
                    slide_type == SlideType.FULL_SCREEN ||
                    slide_type != last_slide_type)
                {
                    last_slide.TransistionEffect = new RandomTransitionEffect(1.5f);
                }
                // ok either last slide and this slide is comment OR
                // last slide and this slide is no comment
                else
                {
                    last_slide.TransistionEffect = GenerateCorNCTransEffect();
                    RectangleF r2 = picture_placeholder;
                    r2.X -= 0.02f;
                    r2.Width += 0.04f;
                    r2.Y -= 0.02f;
                    r2.Height += 0.04f;
                    last_slide.TransistionEffect.TransitionRegion = r2;

                    // IF there were video background best match up videos
                    do_video_match_up = true;
                }
            }

            ArrayList slides = new ArrayList();
            slides.Add(slide);
		    ss.AddSlides(slides);

            CVideoSlide vss = slide as CVideoSlide;
            if (vss == null || vss.Loop == true)
            {
                slide.DisplayLength = slide_length;
            }

            ss.CalcLengthOfAllSlides();

            if (do_video_match_up == true)
            {
             //   DoPossibleVideoMatchUp(last_slide, slide);
            }
        
            return slide_type;
	    }


        //****************************************************************************
        //
        // Processes a line in the files/slides file for LivCreator
        //
        //****************************************************************************
        // LivCreator format = filename, cut start (video only), cut end (video only), music vol (vid only), length (slide only), text 

        public SlideType ProcessSlideLineLivCreator(string line, CSlideShow ss, SlideType last_slide_type)
        {
            string[] parameters = line.Split(',');

            if (parameters.Length <= 0) return SlideType.FULL_SCREEN;
            if (line == "") return SlideType.FULL_SCREEN;

            double initial_delay = 0.0;
            double end_delay = 0.0;
            string background = "";
            float x_s_pos = 0.5f; ;
            float y_s_pos = 0.87f;
            float x_width = 1.0f;
            float y_height = 1.0f;

            float slide_length = ss.mDefaultSlide.DisplayLength;
            string caption = "";
            float music_volume = 1.0f;

            PanZoomType pan_zoom_type = PanZoomType.RANDOM;
  
            string data_file_file_name = parameters[0];

            string slidefn = this.mProjectPath + "\\photos\\" + data_file_file_name;

            // if path info is specified then this means file is from root directory
            // not photos directory
            if (data_file_file_name.Contains("/") ||
                data_file_file_name.Contains(@"\"))
            {
                slidefn = this.GetRootDirectory() + "\\" + data_file_file_name;
            }

            if (System.IO.File.Exists(slidefn) == false) return SlideType.FULL_SCREEN;

            // initial delay
            if (parameters.Length >= 2)
            {
                try
                {
                    initial_delay = double.Parse(parameters[1]);
                }
                catch { }
            }

            // end delay
            if (parameters.Length >= 3)
            {
                try
                {
                    end_delay = double.Parse(parameters[2]);
                }
                catch { }
            }


            // music volume
            if (parameters.Length >= 4)
            {
                try
                {
                    music_volume = float.Parse(parameters[3]);
                }
                catch { }
            }

            // slide length
            if (parameters.Length >= 4)
            {
                try
                {
                    slide_length = float.Parse(parameters[4]);
                }
                catch { }
            }   

            // text
            if (parameters.Length >= 6)
            {
                caption = parameters[5];
            }

            // background
            if (parameters.Length >= 7)
            {
                background = parameters[6];
            }


            // start x_pos
            if (parameters.Length >= 8)
            {
                x_s_pos = float.Parse(parameters[7]);
            }

            // start y_pos
            if (parameters.Length >= 9)
            {
                y_s_pos = float.Parse(parameters[8]);
            }

            // width
            if (parameters.Length >= 10)
            {
                x_width = float.Parse(parameters[9]);
            }

            // height
            if (parameters.Length >= 11)
            {
                y_height = float.Parse(parameters[10]);
            }

            // if full screen slide  =  slidefn
            CImageSlide slide;

            float text_y_pos = 0.87f;

            if (x_width < 1.0 || y_height < 1.0)
            {
                background = background.Trim();

                string backimage_fn = GetRootDirectory() + "\\shared\\LivCreator\\"+background;

                if (System.IO.File.Exists(backimage_fn) == false)
			    {
                    throw new PhotoCruzErrorEx("Can not find back image for theme: " + backimage_fn);
                }

                slide = CreateSlideFromFilename(backimage_fn, null);

                CStillPictureSlide sps = slide as CStillPictureSlide;
                if (sps != null)
                {
                    sps.PanZoom = PanZoom.GeneratePhotoVidShowPanZoom(PanZoomType.NONE, null);
                }

                CVideoSlide vs = slide as CVideoSlide;
                if (vs != null)
                {
                    vs.Loop = true;
                }

                // height will be calculated later

                float start_x_pos = x_s_pos - (x_width / 2.0f);
                float start_y_pos = y_s_pos - (x_width / 2.0f);

                RectangleF picture_placeholder = new RectangleF(start_x_pos, start_y_pos, x_width, x_width);
             
                CDecoration picture_dec = GenerateImageDecorForPlaceHolder(slidefn, picture_placeholder);

                CVideoDecoration v_dec = picture_dec as CVideoDecoration;
                if (v_dec != null)
                {
                    slide_length = (float)v_dec.Player.GetDurationInSeconds();

                    slide_length -= (float)initial_delay;
                    slide_length -= (float)end_delay;

                    v_dec.StartVideoOffset = initial_delay;
                    v_dec.EndVideoOffset = end_delay;
                    v_dec.MusicFadeWhilePlayingVideo = music_volume;
                    v_dec.EnableMusicFadeWhilePlayingVideo = true;
                }

                slide.AddDecoration(picture_dec);

               // text_y_pos = start_y_pos + picture_dec.mCoverageArea.Height; // +((start_y_pos + picture_placeholder.Height) * 0.8f);

            }
            else
            {
                slide = CreateSlideFromFilename(slidefn);

                CStillPictureSlide sps = slide as CStillPictureSlide;
                if (sps != null)
                {
                    if (initial_delay < 0.5)
                    {
                        sps.PanZoom = PanZoom.GeneratePhotoVidShowPanZoom(pan_zoom_type, sps.Image);
                    }
                    else
                    {
                        sps.PanZoom = PanZoom.GeneratePhotoVidShowPanZoom(PanZoomType.NONE, null);
                    }
                }

                CVideoSlide vs = slide as CVideoSlide;
                if (vs != null)
                {
                    vs.StartVideoOffset = initial_delay;
                    vs.EndVideoOffset = end_delay;
                    vs.MusicFadeWhilePlayingVideo = music_volume;
                    vs.EnableMusicFadeWhilePlayingVideo = true;
                }
            }

            if (caption != "")
            {
                CTextStyle ts = GenerateMenuTextStyle();
                ts.FontSize = 16;
                ts.Format.Alignment = StringAlignment.Center;
                RectangleF text_coverage = new RectangleF(x_s_pos, text_y_pos, 0f, 0f);
                CTextDecoration comments_dec = new CTextDecoration(caption, text_coverage, 0, ts);
                comments_dec.SetBackplane(true);
                comments_dec.AttachedToSlideImage = false;
                slide.AddDecoration(comments_dec);
            }

        
            ArrayList slides = new ArrayList();
            slides.Add(slide);
            ss.AddSlides(slides);

            // set length of slide after adding it
            slide.DisplayLength = slide_length;
            slide.TransistionEffect = new RandomTransitionEffect(1.5f);

            ss.CalcLengthOfAllSlides();

            return SlideType.FULL_SCREEN;
        }


                
        //****************************************************************************
        //
        // Try all known paths to find where file is located
        //
        //****************************************************************************
        private string GetTemplateFilePath(string filename)
        {
            string fullpath = GetRootDirectory() + "\\" + filename;
            if (System.IO.File.Exists(fullpath)) return fullpath;
            fullpath = this.mProjectPath + "\\photos\\" + filename;
            if (System.IO.File.Exists(fullpath)) return fullpath;
            fullpath = this.mProjectPath + "\\photos\\thumb\\" + filename;
            if (System.IO.File.Exists(fullpath)) return fullpath;
            fullpath = GetRootDirectory() + "\\shared\\" + mThemeName + "\\" + filename;
            if (System.IO.File.Exists(fullpath)) return fullpath;
            fullpath = GetRootDirectory() + "\\slideshows\\" + filename;
            if (System.IO.File.Exists(fullpath)) return fullpath;
            fullpath = this.mProjectPath + "\\files\\" + filename;
            if (System.IO.File.Exists(fullpath)) return fullpath;
            fullpath = GetRootDirectory() + "\\globalpics\\" + filename;
            if (System.IO.File.Exists(fullpath)) return fullpath;
            fullpath = GetRootDirectory() + "\\globalpics\\overlays\\" + filename;
            if (System.IO.File.Exists(fullpath)) return fullpath;
            fullpath = GetRootDirectory() + "\\globalpics\\alphamaps\\" + filename;
            if (System.IO.File.Exists(fullpath)) return fullpath;
            throw new PhotoCruzErrorEx("Can non locate file:" + filename);
        }

        
        //****************************************************************************
        //
        // Processes a template line
        //
        //****************************************************************************
        private SlideType ProcessTemplateLine(string[] parameters, CSlideShow ss)
        {
            System.Xml.XmlDocument my_doc = new XmlDocument();
            try
            {
                my_doc.Load(@"templates\\"+parameters[0]);
            }
            catch (Exception e)
            {
                throw new PhotoCruzErrorEx("Could not read template file:"+parameters[0]+ "\r\n\r\n"+ e.Message);
            }

            // ok load decoration effects
            XmlNodeList decorationEffectList = my_doc.GetElementsByTagName("DecorationEffectsDatabase");

            if (decorationEffectList.Count > 0)
            {
                XmlElement element = decorationEffectList[0] as XmlElement;
                CAnimatedDecorationEffectDatabase.GetInstance().Append(element, false, true);
            }

            // ok first thing check for TIME=n, which allows slide file to manually change slide time
            for (int paremeterIndex = 1; paremeterIndex < parameters.Length; paremeterIndex++)
            {
                int index = parameters[paremeterIndex].ToLower().IndexOf("time=");
                if (index != -1)
                {
                    string subtime = parameters[paremeterIndex].Substring(index + 5);
                    float newSlideTime = 0;
                    try
                    {
                        newSlideTime = float.Parse(subtime);
                        string doc = my_doc.InnerXml;
                        int displayTimeLengthIndex = doc.IndexOf("DisplayTimeLength=");
                        if (displayTimeLengthIndex != -1)
                        {
                            displayTimeLengthIndex += 19;
                            string new_doc = doc.Substring(0, displayTimeLengthIndex);
                            int end_replace = doc.IndexOf('"', displayTimeLengthIndex);
                            if (end_replace != -1)
                            {
                                new_doc += newSlideTime.ToString();
                                new_doc += doc.Substring(end_replace);
                                try
                                {
                                    my_doc.InnerXml = new_doc;
                                    doc = new_doc;
                                }
                                catch
                                {
                                    throw new PhotoCruzErrorEx("Failed to modify TIME= on template ");
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                // check if pan zoom set
                index = parameters[paremeterIndex].ToLower().IndexOf("pz=");
                if (index != -1)
                {
                    string pz = parameters[paremeterIndex].Substring(index + 3);
                    PanZoomType pzt = PanZoom.ParsePanZoomType(pz, PanZoomType.NONE);

                    CPanZoom mypz = PanZoom.GeneratePhotoVidShowPanZoom(pzt, null);
                    mypz.PanZoomOnAll = true;
                    XmlDocument doc = new XmlDocument();
                    XmlElement top = doc.CreateElement("top");
                    mypz.Save(top, doc);
                    string xmldoc = my_doc.InnerXml;

                    int panzoomindex = xmldoc.IndexOf("<PanZoom");
                    if (panzoomindex != -1)
                    {
                        string newDoc = xmldoc.Substring(0, panzoomindex);
                        int endindex = xmldoc.IndexOf("</PanZoom>");
                        if (endindex != -1)
                        {
                            newDoc += top.InnerXml;
                            newDoc += xmldoc.Substring(endindex + 10);
                            my_doc.InnerXml = newDoc;
                        }
                    }
                    else
                    {
                        // no pan/zoom defined, append to end
                        int image2index = xmldoc.IndexOf("</Slide>");
                        if (image2index != -1)
                        {
                            xmldoc = xmldoc.Insert(image2index, top.InnerXml);
                            my_doc.InnerXml = xmldoc;
                        }
                    }
                }
            }


            string[] slidesReplaceStrings = { "image", "video", "text" };
            string[] templateReplaceStrings = { "Image Filename=", "VideoName=", "Text=" };
            bool[] replacementNeedsFilePath = { true, true, false };

            // look through tokens for images, videos and text
            for (int replaceTokenIndex =0; replaceTokenIndex < slidesReplaceStrings.Length; replaceTokenIndex++)
            {
                // loop through all parameters in slides file
                for(int paremeterIndex = 1; paremeterIndex < parameters.Length; paremeterIndex++)
                {
                    string doc = my_doc.InnerXml;

                    // test up to 100 parameters
                    for (int i = 1; i < 100; i++)
                    {
                        string paremeterLower = parameters[paremeterIndex].ToLower();
                        string copy_string = "";

                        int decor_numer = paremeterLower.IndexOf('$');

                        int index = paremeterLower.IndexOf(slidesReplaceStrings[replaceTokenIndex] + i+" ");
                        if (index == -1)
                        {
                            index = paremeterLower.IndexOf(slidesReplaceStrings[replaceTokenIndex] + i + "=");
                        }
                        if (index == -1)
                        {
                            index = paremeterLower.IndexOf(slidesReplaceStrings[replaceTokenIndex] + i + "(");
                        }
                        int start_copy_index=-1;

                        if (index != -1)
                        {
                            start_copy_index = paremeterLower.IndexOf("=", index);
                            if (start_copy_index != -1)
                            {
                                copy_string = parameters[paremeterIndex].Substring(start_copy_index + 1);
                            }
                            else
                            {
                                throw new PhotoCruzErrorEx("Invalid template parameter:" + parameters[paremeterIndex]);
                            }
                        }
                        else
                        {
                            continue;
                        }

                        // if a file , find full path name
                        if (replacementNeedsFilePath[replaceTokenIndex])
                        {
                            copy_string = GetTemplateFilePath(copy_string);
                        }
                        else
                        {
                            // maybe text, check we're not referencing a file instead
                            if (copy_string.Contains(".txt"))
                            {
                                string file = GetTemplateFilePath(copy_string);
                                if (file != "")
                                {
                                    copy_string = System.IO.File.ReadAllText(file); // System.Text.Encoding.Unicode);
                                }
                                else
                                {
                                    throw new PhotoCruzErrorEx("Can not load text file:" + copy_string);
                                }
                            }
                        }

                        string fileToReplaceString = copy_string;

                        // if a video parhaps start offset/end offset/ music fade defined?
                        if (slidesReplaceStrings[replaceTokenIndex] == "image" || slidesReplaceStrings[replaceTokenIndex] == "video" )
                        {
                            string beforestring = paremeterLower.Substring(0, start_copy_index);

                            int bracketstart = beforestring.IndexOf('(');
                            if (bracketstart != -1)
                            {
                                int bracketend = beforestring.IndexOf(')');
                                if (bracketend != -1)
                                {
                                    string video_values_all = beforestring.Substring(bracketstart + 1, (bracketend - bracketstart) - 1);
                                    string[] video_values = video_values_all.Split(' ');
                                    if (video_values.Length == 3)
                                    {
                                        float start_video_offset = 0;
                                        float end_video_offset = 0;
                                        float music_fade = 0;

                                        start_video_offset = float.Parse(video_values[0]);
                                        end_video_offset = float.Parse(video_values[1]);
                                        music_fade = float.Parse(video_values[2]);
                                        copy_string += '"' + " StartVideoOffset=" + '"' + start_video_offset.ToString() + '"' + " EndVideoOffset=" + '"' +
                                            end_video_offset.ToString() + '"' + " EnableMusicFade=" + '"' + "True" + '"' + " MusicFade=" + '"' + music_fade.ToString();
                                    }
                                }
                            }
                        }

                        int start_replace = -1;

                        // new method find IMAGE1, IMAGE2 VIDEO2 in capital

                        bool continue_search = true;
                        bool found_at_least_one_new_method = false;

                        while (continue_search == true)
                        {
                            continue_search = false;

                            if (decor_numer == -1)
                            {
                                string lookfor = slidesReplaceStrings[replaceTokenIndex].ToUpper() + i.ToString() +'"';
                                start_replace = doc.IndexOf(lookfor);

                                // not found try lower case as this is how PVS creates templates, although not for text
                                if (start_replace == -1 && slidesReplaceStrings[replaceTokenIndex] != "text")
                                {   
                                   
                                    lookfor = slidesReplaceStrings[replaceTokenIndex].ToLower() + i.ToString() + '"';
                                    start_replace = doc.IndexOf(lookfor);
                                }

                                if (start_replace!=-1)
                                {
                                    found_at_least_one_new_method = true;
                                    continue_search = true;
                                }
                                // no more finish
                                else if (found_at_least_one_new_method == true)
                                {
                                    break;
                                }
                            }

                            if (start_replace==-1)
                            {
                                // old method, replace "ImageFilename"
                                for (int replace_number = 0; replace_number < i; replace_number++)
                                {
                                    start_replace = doc.IndexOf(templateReplaceStrings[replaceTokenIndex], start_replace + 1);
                                    if (start_replace == -1)
                                    {
                                        throw new PhotoCruzErrorEx("Can not find anywhere in template '" + parameters[0] + "' to replace " + slidesReplaceStrings[replaceTokenIndex] + i);
                                    }
                                }
                                int template_token_size = templateReplaceStrings[replaceTokenIndex].Length + 1;

                                start_replace += template_token_size;
                            }


                          

                            string new_doc = doc.Substring(0, start_replace);
                            int end_replace = doc.IndexOf('"', start_replace);

                            if (end_replace != -1)
                            {
                                // If test store the original template text too  (this is needed to find out original font size of template
                                if (slidesReplaceStrings[replaceTokenIndex] == "text")
                                {
                                    // already process this, but original text was "text1"  !!!
                                    if (copy_string.Contains("OriginalTextInTemplate") == true)
                                    {
                                        // finish search
                                        break;
                                    }

                                    string original_text = doc.Substring(start_replace, end_replace - start_replace);
                                    copy_string += '"' + " OriginalTextInTemplate=" + '"' + original_text;
                                }

                                new_doc += copy_string;
                                new_doc += doc.Substring(end_replace);

                                // Ok we have video filename but loading in as imageX
                                if (replaceTokenIndex == 0 && CGlobals.IsVideoFilename(fileToReplaceString) == true)
                                {
                                    new_doc = ReplaceImageDecorAsVideoDecor(new_doc, copy_string, start_replace);
                                }

                                try
                                {
                                    my_doc.InnerXml = new_doc;
                                    doc = new_doc;
                                }
                                catch (Exception ee)
                                {
                                    throw new PhotoCruzErrorEx("Failed to modify on template file " + parameters[0] +"\r\n\r\n" + ee.Message);
                                }
                            }
                        }
                    }
                }
            }

            XmlNodeList	list =my_doc.GetElementsByTagName("Slideshow");

            if (list.Count > 0)
            {
                XmlElement element = list[0] as XmlElement;
                ss.Load(element);
            }

            ReCalcSlideLengthsForUpdatedDecors(ss);

            return SlideType.FULL_SCREEN;
        }

       
        //****************************************************************************
        private void ReCalcSlideLengthsForUpdatedDecors(CSlideShow ss)
        {
            // ok loaded slideshow, some slides need re-adjusting i.e. ones with decors with ReCalcSlideLengthOnImage1Change set

            if (ss.mSlides.Count <=0) return;

            CSlide slide = ss.mSlides[ss.mSlides.Count - 1] as CSlide;

            CImageSlide imageSlide = slide as CImageSlide;

            // change image do we need to change pan zoom?  This only applies to first image
            if (imageSlide.PanZoom.ReGenerateOnImageChange == true)
            {
                ArrayList decors = imageSlide.GetAllAndSubDecorations();
                foreach (CDecoration d in decors)
                {
                    CStillPictureSlide sps = slide as CStillPictureSlide;
                    CImageDecoration imageDecoration = d as CImageDecoration;

                    // This assumes the template has template image2 as the main image, (i.e. image1 is a background)
                    if (sps != null && imageDecoration !=null && imageDecoration.OriginalTemplateImageNumber == 2)
                    {
                        CClipArtDecoration cad = imageDecoration as CClipArtDecoration;
                        if (cad != null)
                        {
                            sps.UsePanZoom = ss.mDefaultSlide.UsePanZoom;
                            sps.ReRadomisePanZoom(cad.mImage);
                        }
                        else
                        {
                            sps.UsePanZoom = false;
                        }
                    }
                }
            }

            if (imageSlide != null && slide.ReCalcSlideLengthOnImage1DecorationChange == true)
            {
                ArrayList decors = imageSlide.GetAllAndSubDecorations();
                foreach (CDecoration d in decors)
                {
                    CVideoDecoration vd = d as CVideoDecoration;

                    if (vd!=null && vd.OriginalTemplateImageNumber == 1)
                    {
                        slide.DisplayLength = (float)vd.GetTrimmedVideoDurationInSeconds();
                    }
                }
            }


            // ok if we've replaced text we need to re-calc coverage area
            if (imageSlide != null)
            {
                ArrayList decors = imageSlide.GetAllAndSubDecorations();
                foreach (CDecoration d in decors)
                {
                    CTextDecoration td = d as CTextDecoration;
                    if (td != null && td.CoverageArea.Width > 0 && td.CoverageArea.Height > 0 && td.TextStyle.FontSize > 0 && td.OriginalTemplateText != "")
                    {
                        string newText = td.mText;
                        td.mText = td.OriginalTemplateText;
                        float size = td.GetFontSizeForCoverageArea();
                        td.mText = newText;
                        td.RecalcCoverageAreaForFontSize(size);
                    }
                }
            }    
        }
                
        //****************************************************************************
        private string ReplaceImageDecorAsVideoDecor(string forDoc, string videoFullFilename, int index)
        {
            int currentPtr = forDoc.IndexOf("<Decoration Type=");
            if (currentPtr == -1) return forDoc;

            int lastPtr = currentPtr;
            while (currentPtr != -1 && currentPtr < index)
            {
                currentPtr = forDoc.IndexOf("<Decoration Type=", lastPtr + 17);
                if (currentPtr != -1 && currentPtr < index)
                {
                    lastPtr = currentPtr;
                }
            }

            int endPtr = forDoc.IndexOf("</Decoration>", lastPtr + 17);
            if (endPtr == -1) return forDoc;

            endPtr -= lastPtr;

            string DecorationXml = forDoc.Substring(lastPtr, endPtr + 13);

            DecorationXml = ReplaceImageDecorAsVideoDecor(DecorationXml, videoFullFilename);

            forDoc = forDoc.Substring(0, lastPtr) + DecorationXml + forDoc.Substring(lastPtr + endPtr +13);
            
            return forDoc;
        }


        // Passed in xml should look something like this
        //<Decoration Type="ImageDecoration"><CoverageArea X="0" Y="0" Width="1" Height="1" />
        //<Image Filename="\PhotoCruz\Deployment Debug\shared\scenic02\blankbutterfly02sc.jpg" />
        //</Decoration>
        //****************************************************************************
        private string ReplaceImageDecorAsVideoDecor(string imageDecoraionXML, string videoFullFilename)
        {
            string extraVideoSettings = "";

            if (videoFullFilename.Contains(@"\shared\")==true || videoFullFilename.Contains(@"\globalpics\") == true)
            {
                extraVideoSettings = " GlobalPlayer=" + '"' + "True" + '"';
            }

            imageDecoraionXML = imageDecoraionXML.Replace("Type=" + '"' + "ImageDecoration" + '"',
                                                          "Type="+'"' + "VideoDecoration"+ '"'+" VideoName="+'"' + videoFullFilename+'"' + extraVideoSettings);

            return imageDecoraionXML;
        }


		//****************************************************************************
        private void DoPossibleVideoMatchUp(CImageSlide last_slide, CImageSlide slide)
        {
            CVideoSlide last_video_slide = last_slide as CVideoSlide;
            CVideoSlide video_slide = slide as CVideoSlide;
            if (last_video_slide == null ||
                video_slide == null)
            {
                return;
            }

            float last_f_off = last_video_slide.StartTime;
            float this_f_off = video_slide.StartTime;

            float time_diff = this_f_off - last_f_off;

            if (time_diff < 0) return;

            time_diff += (float) last_video_slide.StartVideoOffset;

            float length = (float) video_slide.Player.GetDurationInSeconds();

            int count = 0;
            while (time_diff > length && count < 100)
            {
                time_diff -= length;
                count++;
            }

            video_slide.StartVideoOffset = time_diff + 0.05;
            video_slide.ResetVideoPlayer();

        }

		//****************************************************************************
        public void ReadSepearteProjectInfoFiles()
        {
            try
            {
                StreamReader reader;
                reader = File.OpenText(mProjectPath + "\\files\\bdate");
                this.mBirthDate = reader.ReadLine();
                reader.Close();
                reader = File.OpenText(mProjectPath + "\\files\\ddate");
                this.mDeathDate = reader.ReadLine();
                reader.Close();
                reader = File.OpenText(mProjectPath + "\\files\\name");
                this.mFirstName = reader.ReadLine();
                this.mLastName = reader.ReadLine();
                reader.Close();
            }
            catch
            {
                throw new PhotoCruzErrorEx("Could not locate project info or date files");
            }

        }

		//****************************************************************************
		public void CheckNeedToLoadProjectInfoFile()
		{

			// already loaded ?
			if (mFirstName=="" || mFirstName==null)
            {
                StreamReader reader;
                try
                {
                    reader = File.OpenText(mProjectPath + "\\files\\project info");
                    try
                    {
                        mFirstName = reader.ReadLine();
                        mLastName = reader.ReadLine();
                        mBirthDate = reader.ReadLine();
                        mDeathDate = reader.ReadLine();
                    }
                    catch
                    {
                        throw new PhotoCruzErrorEx("Corrupt project info file");
                    }

                    reader.Close();
                }
                catch
                {
                    ReadSepearteProjectInfoFiles();
                }
            }

		}

		//****************************************************************************
		public void AddPersonPartcularsToSlide(CImageSlide slide, bool comments)
		{

            RectangleF thumb_placeholder = new RectangleF(0.0625f, 0.2106481f, 0.2760417f, 0.3842593f);

            if (comments == false)
            {
                thumb_placeholder = new RectangleF(0.06597222f, 0.1481481f, 0.2482639f, 0.400463f);
            }
            CDecoration thumb_dec = this.GenerateThumbImage(thumb_placeholder);
			slide.AddDecoration(thumb_dec);

			CheckNeedToLoadProjectInfoFile();

            CTextStyle ts = GenerateSlideTextStyle();
            CTextStyle ts3 = GenerateSlideTextStyle();
        
            RectangleF name_coverage = new RectangleF(0.19f, 0.5957407f, 0.0f, 0.0f);

            // woops won't fit in, adjust
            if (mFirstName.Length > 12)
            {
                name_coverage = new RectangleF(0.04f, 0.61f, 0.31f, 0.2f);
            }

            RectangleF name_coverage2 = new RectangleF(0.19f, 0.66f, 0f, 0f);

            // woops won't fit in, adjust
            if (mLastName.Length > 12)
            {
                name_coverage2 = new RectangleF(0.04f, 0.66f, 0.31f, 0.2f);
            }

            if (comments)
            {
                name_coverage.X += 0.005f;
                name_coverage.Y += 0.01f;
                name_coverage2.X += 0.005f;
                name_coverage2.Y += 0.01f;
            }


         //  CDecoration name_dec = new CTextDecoration(mFirstName + "\n\r" + mLastName, name_coverage, 0, ts);
            CDecoration name_dec1 = new CTextDecoration(mFirstName, name_coverage, 0, ts3);
            name_dec1.AttachedToSlideImage = true;
            CDecoration name_dec2 = new CTextDecoration(mLastName, name_coverage2, 0, ts);
            name_dec2.AttachedToSlideImage = true;
		
			slide.AddDecoration(name_dec1);
            slide.AddDecoration(name_dec2);

            CTextStyle ts2 = ts.Clone();
            ts2.FontSize = 12;
            RectangleF death_coverage = new RectangleF(0.195f,0.766574f,0f, 0f);

            if (comments)
            {
                death_coverage.X += 0.005f;
                death_coverage.Y -= 0.005f;
            }

            CDecoration dates_dec = new CTextDecoration(mBirthDate + "\n\r" + mDeathDate, death_coverage, 0, ts2);
            dates_dec.AttachedToSlideImage = true;
		
			slide.AddDecoration(dates_dec);
		}

	
		//****************************************************************************
		public CImageSlide GenerateILMAndScrollyBackSlide()
		{

			string pre_back_name = "sc";
          
			string backimage_fn = GetRootDirectory()+ "\\shared\\"+ mThemeName +"\\"+ mThemeName+ pre_back_name+".jpg";
			if (System.IO.File.Exists(backimage_fn)==false)
			{
                backimage_fn = GetRootDirectory() + "\\shared\\" + mThemeName + "\\" + mThemeName + pre_back_name + ".mpg";
                if (System.IO.File.Exists(backimage_fn) == false)
				{ 
                    backimage_fn = GetRootDirectory() + "\\shared\\" + mThemeName + "\\" + mThemeName + pre_back_name + ".avi";
                    if (System.IO.File.Exists(backimage_fn) == false)
					{
                        backimage_fn = GetRootDirectory() + "\\shared\\" + mThemeName + "\\" + mThemeName + pre_back_name + ".wmv";
                        if (System.IO.File.Exists(backimage_fn) == false)
                        {
                            throw new PhotoCruzErrorEx("Can not find back scolly image for theme: " + mThemeName);
                        }
					}
				}
			}
			CImageSlide slide = CreateSlideFromFilename(backimage_fn);

            CVideoSlide vs = slide as CVideoSlide;
            if (vs != null)
            {
                // ok make it a looped slide !!!
                vs.Loop = true;
            }

            // slide has no PAN ZOOM if its not full screen
            CStillPictureSlide sps = slide as CStillPictureSlide;
            if (sps != null)
            {
                sps.PanZoom = PanZoom.GeneratePhotoVidShowPanZoom(PanZoomType.NONE, null);
            }

			return slide;
		}

		//****************************************************************************
        public  CDecoration GenerateThumbImage(RectangleF place_holder)
        {
            string fn = this.mProjectPath + "\\photos\\thumb\\thumb.jpg";

            return GenerateImageDecorForPlaceHolder(fn, place_holder);


        }

		//****************************************************************************
        public CDecoration GenerateImageDecorForPlaceHolder(string fn, RectangleF place_holder)
        {
            CDecoration thumb_dec = CreateDecorationForFilename(fn, place_holder);
            CClipArtDecoration cad = thumb_dec as CClipArtDecoration;

             RectangleF rec_thumb = new RectangleF(0, 0, 0,0);

            if (cad != null)
            {
                Bitmap raw_thumb = cad.mImage.GetRawImage() as Bitmap;

              
                rec_thumb = new RectangleF(0, 0, raw_thumb.Width, raw_thumb.Height);
             
            }

            CVideoDecoration vid_dec = thumb_dec as CVideoDecoration;
            if (vid_dec != null && vid_dec.Player!=null)
            {
                rec_thumb = new RectangleF(0, 0, vid_dec.Player.GetWidth(), vid_dec.Player.GetHeight());
            }

            // convert 0..1 for x and to 4:3 aspect.
            rec_thumb.Width *= 3.0f / 4.0f;

            if (rec_thumb.Width == 0 ||
                rec_thumb.Height == 0)
            {
                return thumb_dec;
            }

            // calc best fit to fit thumb into placeholder while keeping aspect
            RectangleF new_cov = CGlobals.CalcBestFitRectagleF(place_holder, rec_thumb);
            thumb_dec.CoverageArea = new_cov;

            return thumb_dec;
        }

		//****************************************************************************
        void AddILMSlide(CSlideShow ss)
        {
			
			CImageSlide slide = GenerateILMAndScrollyBackSlide();

			CheckNeedToLoadProjectInfoFile();

			// add thumb

            RectangleF thumb_placeholder = new RectangleF(0.1336806f, 0.1226852f, 0.2621528f, 0.337963f);
            CDecoration thumb_dec = GenerateThumbImage(thumb_placeholder);
			slide.AddDecoration(thumb_dec);


			// add name bdate and ddate

			CTextStyle ts = GenerateSlideTextStyle();
            ts.Format.Alignment = StringAlignment.Center;
        
            CTextStyle ts2 = ts.Clone();
            ts2.FontSize = 19;

            ts.FontSize = 28;
            ts.Bold = true;
            ts.TextColor = Color.FromArgb(255, 0, 0, 0);

            RectangleF ilm_cov = new RectangleF(0.5f, 0.4857407f, 0f, 0f);
            CDecoration iym_dec = new CTextDecoration("In Loving Memory of", ilm_cov, 0, ts2);
            iym_dec.AttachedToSlideImage = true;

            slide.AddDecoration(iym_dec);

            RectangleF name_coverage = new RectangleF(0.495f, 0.5557407f, 0f, 0f);
            CDecoration name_dec = new CTextDecoration(mFirstName + "\n\r" + mLastName, name_coverage, 0, ts);
            name_dec.AttachedToSlideImage = true;
			slide.AddDecoration(name_dec);

            CTextStyle ts3 = ts2.Clone();
            ts3.FontSize = 15;

            RectangleF death_coverage = new RectangleF(0.5f,0.761574f,0f, 0f);
            CDecoration dates_dec = new CTextDecoration(mBirthDate + " - " + mDeathDate, death_coverage, 0, ts3);
            dates_dec.AttachedToSlideImage = true;
		
			slide.AddDecoration(dates_dec);

			ArrayList slides = new ArrayList();
            slides.Add(slide);
		    ss.AddSlides(slides);
            slide.DisplayLength = 8;
        }
		
		//****************************************************************************
        void AddScrollySlide(CSlideShow ss)
        {
			CImageSlide slide = GenerateILMAndScrollyBackSlide();
			// add thumb

            RectangleF thumb_placeholder = new RectangleF(0.1336806f, 0.1226852f, 0.2621528f, 0.337963f);
            CDecoration thumb_dec = this.GenerateThumbImage(thumb_placeholder);

			slide.AddDecoration(thumb_dec);

            string scrolly_text = "";
            int num_lines = 0;

			// load scrolly file
			StreamReader reader;
            try
            {
                reader = File.OpenText(mProjectPath + "\\files\\scrolly");

			
				string next_line = reader.ReadLine();

                while (next_line !=null)
                {
                    num_lines++;

                    for (int ii=0;ii < next_line.Length;ii++)
                    {
                        char c = next_line[ii];
                        if (char.IsControl(c) == false &&
                            c!='|')
                        {
                            scrolly_text+=c;
                        }
                    }

                    scrolly_text+="\n\r";
                    next_line = reader.ReadLine();
                }
               
                reader.Close();
            }
            catch
            {
                throw new PhotoCruzErrorEx("Can not find scrolly file");
            }

			
         //   RectangleF dec_coverage = new RectangleF(0.1f, 0.8425f, 0,0);

            RectangleF dec_coverage = new RectangleF(0.5f, 0.8425f,0, 0);
            RectangleF clip = new RectangleF(0.08333334f, 0.4722222f, 0.8333333f, 0.3703704f);

            CTextStyle style = this.GenerateSlideTextStyle();
            style.FontSize = 16.5f;

            CScrollingTextDecoration scrolly_dec = new CScrollingTextDecoration(
                                        scrolly_text,
                                        dec_coverage,
                                        0,
                                        style,
                                        clip,
                                        0.03121875f);

            scrolly_dec.AttachedToSlideImage = false;

            slide.AddDecoration(scrolly_dec);

            ArrayList slides = new ArrayList();
            slides.Add(slide);
            ss.AddSlides(slides);

            slide.DisplayLength = 11.75f + (1.82f * ((float)num_lines));
        }


		//****************************************************************************
        void AddEndLogoSlide(CSlideShow ss)
        {
			string pre_back_name = "splash";
          
			string backimage_fn = GetRootDirectory()+ "\\dvd\\"+ pre_back_name+".jpg";
			if (System.IO.File.Exists(backimage_fn)==false)
			{
                backimage_fn = GetRootDirectory() + "\\dvd\\" + pre_back_name + ".mpg";
                if (System.IO.File.Exists(backimage_fn) == false)
				{
                    backimage_fn = GetRootDirectory() + "\\dvd\\" + pre_back_name + ".avi";
                    if (System.IO.File.Exists(backimage_fn) == false)
					{
                        backimage_fn = GetRootDirectory() + "\\dvd\\" + pre_back_name + ".wmv";
                        if (System.IO.File.Exists(backimage_fn) == false)
                        {
                            throw new PhotoCruzErrorEx("Can not find splash for theme: " + mThemeName);
                        }
					}
				}
			}
			CImageSlide slide = CreateSlideFromFilename(backimage_fn);

            // slide has no PAN ZOOM if its not full screen
            CStillPictureSlide sps = slide as CStillPictureSlide;
            if (sps != null)
            {
                sps.PanZoom = PanZoom.GeneratePhotoVidShowPanZoom(PanZoomType.NONE, null);
            }

			ArrayList slides = new ArrayList();
            slides.Add(slide);
		    ss.AddSlides(slides);

            slide.DisplayLength = 10;

        }


		//****************************************************************************
        void GenerateMenus(CProject project)
        {
            this.mCurrentProject = project;

           
            ArrayList menu_lines = new ArrayList();
            // load dvdsettings file
            StreamReader reader;
            try
            {
                reader = File.OpenText(mProjectPath + "\\files\\dvdsettings");
                string next_line = reader.ReadLine();

                while (next_line != null)
                {
                    menu_lines.Add(next_line);
                    next_line = reader.ReadLine();
                }

                reader.Close();
            }
            catch
            {
                throw new PhotoCruzErrorEx("Can not find dvdsettings file");
            }

            // process sub menus (backwards)

            for (int sm = 99; sm >= 1; sm--)
            {

                foreach (string line in menu_lines)
                {
                    string ltu = line.ToUpper();
                    string tltu = ltu.Trim();
                    if (tltu.StartsWith("SM" + sm))
                    {
                        parse_menu_line(line, sm);
                    }
                }
            }
            

            // and now main menu, pre menu
          
            foreach (string line in menu_lines)
            {
                string ltu = line.ToUpper();
                string tltu = ltu.Trim();
                if (tltu.StartsWith("MM") ||
                    tltu.StartsWith("INTRO") || 
                    tltu.StartsWith("PREMENU"))
                {
                    parse_main_menu_line(line);
                }
            }
          
            // check for chapel layout (need to do this first)
            // as CMBG will set the CMVIDEO too
            
             // parse chapel menu
            foreach (string line in menu_lines)
            {
            	string ltu = line.ToUpper();
            	string tltu = ltu.Trim();
            	if (tltu.StartsWith("CMLAYOUT"))
            	{
            		string sub1 = tltu.Substring(line.IndexOf('=') + 1);
                	sub1 = sub1.Trim();
              		int tnum =-1;
              		try 
              		{
                	   tnum = int.Parse(sub1);
              		}
              		catch
              		{
              		}
              		
              		mChapelLayout = tnum;
              		
              		
            	}
            }
            
            // parse chapel menu
            foreach (string line in menu_lines)
            {
            	string ltu = line.ToUpper();
            	string tltu = ltu.Trim();
            	if (tltu.StartsWith("CM"))
            	{
            		parse_chapel_menu_line(line);
            	}
            }
        }

		//****************************************************************************

        private CMainMenu CreateMenuFromImage(string bg_image)
        {
            CImageSlide s = this.CreateSlideFromFilename(bg_image);
            ArrayList decs = new ArrayList();
            CMainMenu mm = new CMainMenu(s, decs);
            mm.SetLinkStyle(0);
            mm.PlayingThumbnails = false;
   
            // if a video menu background
            // remove background music and set length to video length
            if (s is CVideoSlide == true)
            {
                mm.FadeAudioOut = false;
                mm.MusicSlide = null;

                CVideoSlide vs = s as CVideoSlide;
                if (vs.Player != null)
                {
                    float video_len = (float)vs.Player.GetDurationInSeconds();

                    // if over 1 minute then TOO LONG
                    if (video_len < 60)
                    {
                        mm.Length = video_len;
                    }
                }
            }
            else
            {
                mm.MusicSlide = null;
                mm.Length = 20.0f;
            }
            return mm;
        }
        
        //****************************************************************************
		//
		//  Parse a line that starts with CM
		//
		//****************************************************************************
		private void parse_chapel_menu_line(string line)
        {
		    string[] strings;
            string upper = line.ToUpper();
			//CMBG
            if (upper.Contains("CMBG") == true)
            {
                mCurrentLayout = 3;
               // Console.WriteLine("Found CMBG");
                strings = upper.Split('=');
                if (strings.Length >= 1)
                {
                	// there should always be a mainmenu by now
                    if (CGlobals.mCurrentProject.MainMenu == null)
                    {
                        return;
                    }

                    string bg_image = this.GetRootDirectory() + "\\"+strings[1].Trim();

                    if (System.IO.File.Exists(bg_image) == false)
                    {
                        throw new PhotoCruzErrorEx("Can not locate file '" + bg_image + "'");
                    }
                    
                    CMainMenu chapel_menu = CreateMenuFromImage(bg_image);
                    
                    SubMenu sm = new SubMenu(99, chapel_menu);
               
                    this.mSubMenus.Add(sm);
            
                    if (mChapelLayout==-1) 
                    {
                      chapel_menu.SetLayout(3);
                    }
                    else
                    {
                    	chapel_menu.SetLayout(mChapelLayout);
                    }
                   
                   // Console.WriteLine("Set chapel menu to " + bg_image);
                    
				    AddSubMenuButtonToMenu(chapel_menu, 0, CGlobals.mCurrentProject.MainMenu, 1, 99, false, false);
				    CGlobals.mCurrentProject.MainMenu = chapel_menu;
                    chapel_menu.AddSlideShow(this.mMainSlideShow);
                    mProjectHasChapelMenu = true;

                    // remove fading in/out as slideshow repeats 
                //    mMainSlideShow.FadeOut = false;
                //    mMainSlideShow.Fade = false;
				    
				    // get slideshowbutton decoration
				    
				    ArrayList dec = chapel_menu.GetSlideShowButtons();
				    if (dec.Count!=0)
				    {
				    	CMenuSlideshowButton sb = dec[0] as CMenuSlideshowButton;
				    	if (sb !=null)
				        {	
                            sb.ReturnToMenuAfterPlay = false;
				    	}
				    }
				    
				    VideoSlideShow vss = new VideoSlideShow();
                	vss.number = 2;
                	vss.mm_number = 99;
                	vss.menu = chapel_menu;
                	vss.ss = mMainSlideShow;
               		this.mVideoSlideShows.Add(vss);    
                }
                else
                {
                    throw new PhotoCruzErrorEx("Can not process dvdsettings line '" + line + "'");
                }
                
                
                return;
            }
            
            for (int i = 0; i < 6; i++)
            {
                string mt = "CMTITLE" + i;
                if (upper.Contains(mt))
                {
                    process_menu_video_title_name(i, 99, line);
                }
            }        
		}
	
		
		//****************************************************************************
		//
		//  Parse a line that starts with MM or PREMENU
		//
		//****************************************************************************
        private void parse_main_menu_line(string line)
        {
            string[] strings;
            string upper = line.ToUpper();
            if (upper.Contains("PREMENU") == true)
            {
                //Console.WriteLine("Found premenu");
                strings = upper.Split('=');
                if (strings.Length >= 1)
                {
                    this.mPreMenuFileName = this.GetRootDirectory() + "\\"+ strings[1].Trim();
                    
                    if (System.IO.File.Exists(mPreMenuFileName) == false)
                    {
                        throw new PhotoCruzErrorEx("Can not locate pre menu video file '" + mPreMenuFileName + "'");
                    }
                    
                    //Console.WriteLine("premenu filename = " + mPreMenuFileName);
                    this.mCurrentProject.PreMenuMovieFileName = mPreMenuFileName;
                }
                else
                {
                    throw new PhotoCruzErrorEx("Can not process dvdsettings line '" + line + "'");
                }
                return;
            }

            if (upper.Contains("MMBG") == true)
            {
                mCurrentLayout = 0;
                //Console.WriteLine("Found MMBG");
                strings = upper.Split('=');
                if (strings.Length >= 1)
                {
                    if (CGlobals.mCurrentProject.MainMenu != null)
                    {
                        return;
                    }

                    string bg_image = this.GetRootDirectory() + "\\"+strings[1].Trim();

                    if (System.IO.File.Exists(bg_image) == false)
                    {
                        throw new PhotoCruzErrorEx("Can not locate file '" + bg_image + "'");
                    }


                    CMainMenu mm = CreateMenuFromImage(bg_image);

                    CGlobals.mCurrentProject.MainMenu = mm;
                    //Console.WriteLine("Set main menu to " + bg_image);
                }
                else
                {
                    throw new PhotoCruzErrorEx("Can not process dvdsettings line '" + line + "'");
                }
                return;
            }
           
            for (int i = 0; i < 6; i++)
            {
                string mvd = "MMVIDEOFILE" + i;
                if (upper.Contains(mvd))
                {
                    process_menu_video_file(i, 0, upper);
                }

                string mt = "MMTITLE" + i;
                if (upper.Contains(mt))
                {
                    process_menu_video_title_name(i, 0, line);
                }
            }

            if (upper.Contains("MMLAYOUT"))
            {
                this.ProcessSetMenuLayout(0, line);
            }

            if (upper.Contains("INTRO"))
            {
                this.ProcessIntroVideoLine(line);
            }
           
        }

        //****************************************************************************
        private void ProcessIntroVideoLine(string line)
        {
            try
            {
                //Console.WriteLine("Found INTRO");

                string sub1 = line.Substring(line.IndexOf('=') + 1);
                sub1 = sub1.Trim();

                string file = this.GetRootDirectory() + "\\"+sub1;

                if (System.IO.File.Exists(file) == false)
                {
                    throw new PhotoCruzErrorEx("Can not locate file '" + file + "'");
                }

                this.mIntroVideo = file;

                return;
            }
            catch
            {
                throw new PhotoCruzErrorEx("Can not process line '" + line + "'");
            }
        }

        
		//****************************************************************************
        private void parse_menu_line(string line, int sub_menu)
        {
            string[] strings;
            string upper = line.ToUpper();
           
            if (upper.Contains("SM"+sub_menu+"BG") == true)
            {
                mCurrentLayout = 0;

                //Console.WriteLine("Found SM"+sub_menu+"BG");
                strings = upper.Split('=');
                if (strings.Length >= 1)
                {
                    string bg_image = this.GetRootDirectory() + "\\" + strings[1].Trim();

                    if (System.IO.File.Exists(bg_image) == false)
                    {
                        throw new PhotoCruzErrorEx("Can not locate file '" + bg_image + "'");
                    }

                    CMainMenu mm = CreateMenuFromImage(bg_image);
                    mSubMenus.Add(new SubMenu(sub_menu, mm));
                }
                else
                {
                    throw new PhotoCruzErrorEx("Can not process dvdsettings line '" + line + "'");
                }
                return;
            }

            for (int i = 0; i < 6; i++)
            {
                string mvd = "SM"+sub_menu+"VIDEOFILE" + i;
                if (upper.Contains(mvd))
                {
                    process_menu_video_file(i, sub_menu, upper);
                }

                string mt = "SM"+sub_menu+"TITLE" + i;
                if (upper.Contains(mt))
                {
                    process_menu_video_title_name(i, sub_menu, line);
                }
            }

            string mt2 = "SM" + sub_menu + "MORE";
            if (upper.Contains(mt2))
            {
                process_menu_video_file(0, sub_menu, upper, true);
            }

            string ml2 = "SM" + sub_menu + "LAYOUT";
            if (upper.Contains(ml2))
            {
                ProcessSetMenuLayout(sub_menu, upper);
            }

        }

	    //****************************************************************************
        private void ProcessSetMenuLayout(int sub_menu, string line)
        {
            try
            {
                string sub1 = line.Substring(line.IndexOf('=') + 1);
                sub1 = sub1.Trim();

                int num = int.Parse(sub1);

                CMainMenu mm = this.GetMenu(sub_menu);
                if (mm!=null)
                {
                    mm.SetLayout(num);
                    mCurrentLayout = num;
                }
            }
            catch
            {
                throw new PhotoCruzErrorEx("Can not process line '" + line + "'");
            }
        }


		//****************************************************************************
        private void FitMenuTextNicely(CTextDecoration td, string text)
        {
            // split text into two lines;
            string[] words = text.Split(' ');
            if (words.Length > 1 && text.Length > 14)
            {
                int message_length = text.Length;
                int ideal = message_length / 2;

                // find nearest space to middle
                bool found = false;
                int offset=0;
                int final_offset=0;
                try
                {
                    while (found == false)
                    {
                        char c = text[ideal + offset];
                        if (c == ' ')
                        {
                            final_offset = ideal + offset;
                            found = true;
                        }
                        c = text[ideal - offset];
                        if (c == ' ')
                        {
                            final_offset = ideal - offset;
                            found = true;
                        }
                        offset++;
                    }
                }
                catch
                {
                }
                if (final_offset != 0)
                {
                    string after = text.Substring(final_offset + 1);
                    string before = text.Substring(0, final_offset);
                    text = before + "\n\r" + after;
                }
            }

            CTextStyle old_ts = td.TextStyle;

            td.mText = text;
            CTextStyle ts = GenerateMenuTextStyle().Clone();
         
            ts.Format.Alignment = old_ts.Format.Alignment;
            td.TextStyle = ts;

            if (this.mCurrentLayout >= 6)
            {
                td.TextStyle.FontSize = 18;
            }



            float x = td.CoverageArea.X;
       
            if (ts.Format.Alignment == StringAlignment.Far)
            {
                x += td.CoverageArea.Width;
            }

            else if (ts.Format.Alignment == StringAlignment.Center)
            {
                x += td.CoverageArea.Width/2;
            }

            RectangleF newCoverage = td.CoverageArea;
            newCoverage.X = x;
            newCoverage.Width = 0;// rebuild font with OUR Text Style size
            newCoverage.Height = 0;// rebuild font with OUR Text Style size

            td.CoverageArea = newCoverage;
        }

	  //****************************************************************************
	  // Process MMTITLE or SMTITLE
	  //
	  //****************************************************************************
        private void process_menu_video_title_name(int number, int mm, string line)
        {
            VideoSlideShow the_vss = null;

            foreach (VideoSlideShow vss in this.mVideoSlideShows)
            {
                if (vss.number == number &&
                    vss.mm_number == mm)
                {
                    the_vss = vss;
                    break;
                }
            }
            if (the_vss == null)
            {
                return;
            }
            string[] strings = line.Split('=');
            if (strings.Length < 1)
            {
                throw new PhotoCruzErrorEx("Can not process dvdsettings line '" + line + "'");
            }

            // a link to a slideshow ?
            if (the_vss.ss!=null)
            {

                ArrayList buttons = the_vss.menu.GetSlideShowButtons();

                foreach (CMenuSlideshowButton button in buttons)
                {
                    if (button.GetInnerImageStringId() == the_vss.ss.Name)
                    {
                        ArrayList textattach = button.AttachedChildDecorations;

                        foreach (int d in textattach)
                        {
                            CDecoration dec = the_vss.menu.BackgroundSlide.GetDecorationFromID(d);
                            if (dec != null)
                            {
                                CTextDecoration td = dec as CTextDecoration;
                                if (td != null &&
                                    td.VCDNumber == false)
                                {
                                    this.FitMenuTextNicely(td, strings[1].Trim());
                                    td.AttachedToSlideImage = true;
                               
                                    return;

                                }
                            }
                        }
                    }
                }

			}
            // a link to a submenu
            else if (the_vss.mSubMenu != null)
            {

                ArrayList buttons = the_vss.menu.GetLinkSubMenuButtons();

                foreach (CMenuLinkSubMenuButton button in buttons)
                {
                    if (button.MenuLinkID == the_vss.mSubMenu.ID)
                    {
                        ArrayList textattach = button.AttachedChildDecorations;

                        foreach (int d in textattach)
                        {
                            CDecoration dec = the_vss.menu.BackgroundSlide.GetDecorationFromID(d);
                            if (dec != null)
                            {
                                CTextDecoration td = dec as CTextDecoration;
                                if (td != null &&
                                    td.VCDNumber == false)
                                {
                                    this.FitMenuTextNicely(td, strings[1].Trim());

                                    return;

                                }
                            }
                        }
                    }
                    
                }

            }

		}

	    //****************************************************************************
        private void process_menu_video_file(int number, int mm, string line)
        {
            process_menu_video_file(number, mm, line, false);
        }


	    //****************************************************************************
        private void process_menu_video_file(int number,int mm,  string line, bool as_more_link)
        {
            //Console.WriteLine("Found Menu Video slideshow");
            string[] strings = line.Split('=');
            if (strings.Length < 1)
            {
                throw new PhotoCruzErrorEx("Can not process dvdsettings line '" + line + "'");
            }

            string fn = strings[1].Trim();
            string fn_upper = fn.ToUpper();
            if (fn_upper.Contains("MAINSLIDESHOW") && mm == 0)
            {
                CSlideShow ss = this.GenerateMainSlideshow(this.mCurrentProject, mm, number);
                mMainSlideShow = ss;
                this.mCurrentProject.AddSlideshow(ss, this.mCurrentProject.MainMenu);

                VideoSlideShow vss = new VideoSlideShow();
                vss.number = number;
                vss.mm_number = mm;
                vss.menu = this.mCurrentProject.MainMenu;
                vss.ss = ss;

                this.mVideoSlideShows.Add(vss);

                return;
            }
            else if (fn_upper.Contains("SUBMENU"))
            {
                for (int i = 0; i < 99; i++)
                {
                    if (fn_upper.Contains("SUBMENU" + i))
                    {
                        CMainMenu sub_mm = GetMenu(i);
                        if (sub_mm != null)
                        {
                            CMainMenu tmm = GetMenu(mm);

                            AddSubMenuButtonToMenu(tmm, i, sub_mm, number, mm, as_more_link, true);
                        }
                        break;
                    }
                }
            }
            else
            {
                // must be a file
                string the_fn = this.GetRootDirectory() + "\\" + fn_upper;
                this.CreateVideoSlideShowInMenu(number, mm, the_fn);
            }

        }

	    //****************************************************************************
        private CMainMenu GetMenu(int mm)
        {
            if (mm == 0) return mCurrentProject.MainMenu;
            CMainMenu for_menu = null;
            foreach (SubMenu sm in this.mSubMenus)
            {
                if (sm.mNum == mm)
                {
                    for_menu = sm.mMenu;
                    break;
                }
            }
            return for_menu;
        }

	    //****************************************************************************
        private void CreateVideoSlideShowInMenu(int number, int mm , string fn)
        {
            CMainMenu for_menu = GetMenu(mm);

            if (for_menu == null) return ;

            if (System.IO.File.Exists(fn) == false)
            {
                throw new PhotoCruzErrorEx("Can not find file '" + fn + "'");
            }

            if (CGlobals.IsVideoFilename(fn) == false)
            {
                throw new PhotoCruzErrorEx("file is not video file '" + fn + "'");
            }

            CSlideShow ss = new CSlideShow("menu" + mm + "slideshow" + number);

            CVideoSlide vs = new CVideoSlide(fn);
            ArrayList slides = new ArrayList();
            slides.Add(vs);
            ss.AddSlides(slides);

			// assume this is pre encoded slideshow
			// the author process will double check and do appropriate thing

			string fn2 = GetRootDirectory() + "//slideshows//"+ System.IO.Path.GetFileName(fn);
			if (System.IO.File.Exists(fn2)==true)
			{
                if (vs.Player != null &&
                    vs.Player.IsVideoInCorrrectOutputDVDFormat() == true)
                {
                    ss.PreEncodedSlideshow = true;
                }
			}

            VideoSlideShow vss = new VideoSlideShow();
            vss.number = number;
            vss.mm_number = mm;
            vss.menu = for_menu;
            vss.ss = ss;

            this.mVideoSlideShows.Add(vss);

            this.mCurrentProject.AddSlideshow(ss, for_menu);

        }

	    //****************************************************************************
        public void AddSubMenuButtonToMenu(CMainMenu menu, 
                                           int sub_menu_num, 
                                           CMainMenu sub_menu, 
                                           int number,
                                           int mm,
                                           bool as_more_link,
                                           bool add_previous_back_button_to_submenu)
        {
            if (menu == null || sub_menu == null)
            {
                return;
            }

            if (as_more_link)
            {
                menu.AddSubMenu(sub_menu);

                // add more text

                CTextStyle ts = GenerateMenuTextStyle();
                RectangleF text_coverage = new RectangleF(0.72f, 0.8f, 0f, 0f);
                CTextDecoration more_dec = new CTextDecoration("More", text_coverage, 0, ts);
                more_dec.AttachedToSlideImage = true;
                menu.BackgroundSlide.AddDecoration(more_dec);

            }
            else
            {
            	sub_menu.ParentMenu = menu;
            	if (add_previous_back_button_to_submenu==true)
            	{
                	sub_menu.AddMenuLinkButton(CMenuLinkPreviousNextButton.LinkType.PREVIOUS_MENU, menu);
            	}
                menu.SubMenus.Add(sub_menu);
              
                int ssb = menu.GetNumSlideShowButtons();

                int nb = ssb;

                if (menu.TestNeedToUpgradeMenuLayoutToCaterForBewButton() == false)
                {
                    throw new PhotoCruzErrorEx("Too many slideshows for menu, we only support up to " + nb);
                }

                CMenuLayout layout = CMenuLayoutDatabase.GetInstance().GetLayout(menu.Layout);

                CMenuButtonStyle style = CMenuButtonStyleDatabase.GetInstance().GetStyle(menu.ButtonStyle);

                CImage frame = style.SlideshowButtonFrameImage;
                CImage mask = style.SlideshowButtonMaskImage;

                CMenuButton button1 = new CMenuLinkSubMenuButton(sub_menu, frame, mask, layout.GetSlideshowButtonPosition(nb));
                button1.AttachedToSlideImage = false;

                // Add side text spec
                CMenuTextSpecification spec = layout.GetSlideshowTextSpec(nb);
                if (spec == null)
                {
                    spec = layout.GetSlideshowTextSpec(0);
                }

                CTextStyle ts = new CTextStyle();

                if (spec != null)
                {
                    ts = spec.Style.Clone();
                }

                RectangleF c = new RectangleF(0, 0, 0, 0);
                CTextDecoration td = new CTextDecoration("SubMenu", c, 0, ts);
                td.AttachedToSlideImage = true;
                td.CoverageArea = spec.ReCalcMenuTextCoverageArea(td,ts.FontSize);

                button1.AddChildDecoration(td);

                menu.BackgroundSlide.AddDecoration(button1);
                menu.BackgroundSlide.AddDecoration(td);
            }

            if (add_previous_back_button_to_submenu)
            {
	            // add back text
	
	            CTextStyle back_ts = GenerateMenuTextStyle();
	            RectangleF back_text_coverage = new RectangleF(0.28f, 0.8f, 0f, 0f);
	            CTextDecoration back_dec = new CTextDecoration("Back", back_text_coverage, 0, back_ts);
                back_dec.AttachedToSlideImage = true;
                sub_menu.BackgroundSlide.AddDecoration(back_dec);
            }
	
	
            VideoSlideShow vss = new VideoSlideShow();
            vss.number = number;
            vss.mm_number = mm;
            vss.menu = menu;
            vss.mSubMenu = sub_menu; 

            this.mVideoSlideShows.Add(vss);
        }
	}	
}

