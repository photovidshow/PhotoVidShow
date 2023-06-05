using System;
using System.Collections;
using System.Drawing;
using System.Xml;
using ManagedCore;
using System.Globalization;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for CMainMenu.
	/// </summary>
	public class CMainMenu : CVideo
	{
        public const int MaxSlideshowButtons = 6;

        public const float DefaultMenuLength = 48.0f;

		private CImageSlide mBackground;

        private float mLength = DefaultMenuLength; 
		private int		mId;
		private bool mPlayingThumbnails = true;
        private bool mZoomingBackground = false;
        private bool mFadeAudioOut = true;

        private CMenuSubPictureStyle mMenuSubPictureStyle = new CMenuSubPictureStyle();

		private static int mGlobalID =0;

		private CMusicSlide	mMusicSlide ;

        // used by dvd author only (a bit of a hack i'm afraid;
        private int vml_menu_author_number = 1;


		private ArrayList	mSubMenus;
		private CMainMenu	mParentMenu;

		private	int	mLayout=-1;
		private	int	mButtonStyle=6;
        private int mLinkStyle = 1;

        public CMenuSubPictureStyle SubPictureStyle
        {
            get { return mMenuSubPictureStyle; }
            set { mMenuSubPictureStyle = value; }
        }

        public int VmlMenuAuthorNumber
        {
            get { return vml_menu_author_number; }
        }

        public int Layout
        {
            get { return mLayout; }
        }

        public int ButtonStyle
        {
            get { return mButtonStyle; }
        }

        public bool FadeAudioOut
        {
            get { return mFadeAudioOut; } 
            set { mFadeAudioOut = value; }
        }

		public CMusicSlide MusicSlide
		{
			get
			{
				return mMusicSlide ;
			}
			set
			{
				mMusicSlide = value;
			}
		}


		public bool PlayingThumbnails
		{
			get
			{
				return mPlayingThumbnails ;
			}
			set
			{
				mPlayingThumbnails = value;
			}
		}

        public bool ZoomingBackground
        {
            get
            {
                return mZoomingBackground;
            }
            set
            {
                mZoomingBackground = value;
                ResetPanZoom();
            }
        }


		public float Length
		{
			get { return mLength ; }
			set
			{
				mLength = value ;
			}
		}

		public int ButtonStyleID
		{
			get { return mButtonStyle ; }
		}

        public int LinkStyleID
        {
            get { return mLinkStyle; }
        }

		public int LayoutStyleID
		{
			get { return mLayout ; }
		}

		public ArrayList SubMenus
		{
			get { return mSubMenus ; }
		}

		public int ID
		{
			get { return mId;}
		}

		public CMainMenu ParentMenu
		{
			get { return mParentMenu ; }
			set { mParentMenu = value ; }
		}

        public CImageSlide BackgroundSlide
        {
            get { return mBackground; }
        }

		//	public ArrayList mDecorations ;
		//	public ArrayList mButtons ;
		//	public CMenuLayout  mLayout;

		//*******************************************************************
		public CMainMenu()
		{
			mSubMenus = new ArrayList();
			mId =mGlobalID ++;

			//SetUpFreeMusic();
		}


		//*******************************************************************
		public CMainMenu(CImageSlide slide, ArrayList decorations)
		{
			mSubMenus = new ArrayList();
			mId =mGlobalID ++;

			mBackground = slide;

			CStillPictureSlide sps = mBackground as CStillPictureSlide;
            if (sps != null && sps.Image != null)
			{
                sps.Image.NoBlankArea = true;
			}

			mBackground.PartOfAMenu = true;
			mBackground.UsePanZoom = false ;

			foreach (CDecoration decoration in decorations)
			{
				decoration.AttachedToSlideImage = false;
				mBackground.AddDecoration(decoration);
			}

			this.SetLayout(2);

			SetUpFreeMusic();
		}

		//*******************************************************************
		public void SetUpFreeMusic()
		{
            string mp = CGlobals.GetRootDirectory() + "\\music\\New Stories (Highway Blues).wma";

			if (System.IO.File.Exists(mp)==true)
			{
				this.SetMusicSlide(mp);
			}
		}

        //*******************************************************************
        public override int GetNumberRequiredMotionBlurSubFrames(int frame)
        {
            return 1;
        }

        //*******************************************************************
        public override int GetMaxNumberRequiredMotionBlurSubFrames()
        {
            if (CGlobals.mCurrentProject.DiskPreferences.TurnOffMotionBlur == true)
            {
                return 1;
            }

            return 1;
        }

		//*******************************************************************
		public ArrayList GetSelfAndAllSubMenus()
		{
			ArrayList a = new ArrayList();
			a.Add(this);

			for (int i=0;i<this.mSubMenus.Count;i++)
			{
				a.AddRange((mSubMenus[i] as CMainMenu).GetSelfAndAllSubMenus());
			}
			return a;
		}

	
		//*******************************************************************
		// returns number of slideshow buttons in a menu
        // DOES NOT WORK WITH SLIDESHOW LINK BUTTONS !!!!
		public ArrayList GetSlideShowButtons()
		{
			ArrayList a = new ArrayList();

			foreach (CDecoration decoration in mBackground.Decorations)
			{
				if (decoration as CMenuSlideshowButton != null)
				{
					a.Add(decoration);
				}
			}
			return a;
		}


        //*******************************************************************
        // For PhotoCruz
        public ArrayList GetLinkSubMenuButtons()
        {
            ArrayList a = new ArrayList();

            foreach (CDecoration decoration in mBackground.Decorations)
            {
                if (decoration as CMenuLinkSubMenuButton != null)
                {
                    a.Add(decoration);
                }
            }
            return a;
        }


        //*******************************************************************
        public int GetNumSlideShowButtons()
        {
            ArrayList a = new ArrayList();

            foreach (CDecoration decoration in mBackground.Decorations)
            {
                if (decoration as CMenuSlideshowButton != null ||
                    decoration as CMenuLinkSubMenuButton != null)
                {
                    a.Add(decoration);
                }
            }
            return a.Count;
        }


		//*******************************************************************
		// returns number of slideshow buttons in a menu
		public ArrayList GetLinkButtons()
		{
			ArrayList a = new ArrayList();

			foreach (CDecoration decoration in mBackground.Decorations)
			{
				if (decoration as CMenuLinkButton != null)
				{
					a.Add(decoration);
				}
			}
			return a;
		}

		
		//*******************************************************************
		public bool TestNeedToUpgradeMenuLayoutToCaterForBewButton()
		{

            int nb = GetNumSlideShowButtons();

			int required_buttons = nb+1;
			CMenuLayout layout = CMenuLayoutDatabase.GetInstance().GetLayout(mLayout);

			if (layout.GetNumButtonPositions() <required_buttons )
			{
				int layout_id = CMenuLayoutDatabase.GetInstance().GetFirstLayoutThatSupportsNButtons(required_buttons);
				if (layout_id <0)
				{
				
					return false ;
				}

				this.SetLayout(layout_id);

			}

			return true;
		}

		//*******************************************************************
		// Are we alowed to use a given layout.  usefull if for example the
		// layout has maxd 2 slideshows but we current have 4 or something.
		public bool CanUseMenuLayout(int layout)
		{
            int ssb = GetNumSlideShowButtons();
			CMenuLayoutDatabase db =CMenuLayoutDatabase.GetInstance();
			CMenuLayout the_layout =db.GetLayout(layout);
			if ( ssb > the_layout.GetNumButtonPositions())
			{
				return false;
			}
			return true;
		}

        //*******************************************************************
		public void	AddSlideShow(CSlideShow show)
		{
            AddSlideShow(show, "");
        }

		//*******************************************************************
		public void	AddSlideShow(CSlideShow show, string menu_text)
		{
			// SRG add formatations

            int nb = GetNumSlideShowButtons();


			//	CImage frame = new CImage(@"c:\dev\dvdslideshow\clipart\thinkbubble.png");

			
		
			if (TestNeedToUpgradeMenuLayoutToCaterForBewButton()==false)
			{
				CDebugLog.GetInstance().Error("Too many slideshows for menu, we only support up to "+nb);
				return ;
			}

			CMenuLayout layout = CMenuLayoutDatabase.GetInstance().GetLayout(mLayout);

			CMenuButtonStyle style = CMenuButtonStyleDatabase.GetInstance().GetStyle(this.mButtonStyle);

			CImage frame = style.SlideshowButtonFrameImage;
			CImage mask = style.SlideshowButtonMaskImage;

			CMenuButton button1 = new CMenuSlideshowButton(show.Name,frame,mask,layout.GetSlideshowButtonPosition(nb));
			button1.AttachedToSlideImage=false;

			// Add side text spec
			CMenuTextSpecification spec =  layout.GetSlideshowTextSpec(nb);
			if (spec==null)
			{
				CDebugLog.GetInstance().Error("Unknown layout spec:"+nb+" on call to CMainMenu::AddSlideShow");
				spec =  layout.GetSlideshowTextSpec(0);
			}

			CTextStyle ts = new CTextStyle();
			
			if (spec!=null)
			{
				ts = spec.Style.Clone();
			}
				
			RectangleF c= new RectangleF(0,0,0,0);

            CTextDecoration td;

            if (menu_text == "")
            {
                td = new CTextDecoration(show.Name, c, 0, ts);
            }
            else
            {
                td = new CTextDecoration(menu_text, c, 0, ts);
            }

            td.CoverageArea = spec.ReCalcMenuTextCoverageArea(td, ts.FontSize);

			button1.AddChildDecoration(td);

			this.mBackground.AddDecoration(button1);
			this.mBackground.AddDecoration(td);


			// add vcd stuff
			CMenuTextSpecification vcd_spec =  layout.GetVCDNumberTextSpec(nb);// number get calculate later
			CTextStyle ts1 = new CTextStyle();
			
			if (vcd_spec!=null)
			{
				ts1 = vcd_spec.Style.Clone();
			}
				
			RectangleF c1= new RectangleF(0,0,0,0);
			CTextDecoration td1 = new CTextDecoration("1", c1,0, ts1);
			td1.VCDNumber=true;
			if (vcd_spec!=null)
			{
                td1.CoverageArea = vcd_spec.ReCalcMenuTextCoverageArea(td1, ts1.FontSize);
			}
			else
			{
				CDebugLog.GetInstance().Error("Missing vcd number on slideshow layout");
                td1.CoverageArea = new RectangleF(-2000, -2000, 0, 0);
			}

			button1.AddChildDecoration(td1);
			this.mBackground.AddDecoration(td1);
		}

        protected override void GetVideo(GraphicsEngine engine, RenderVideoParameters renderVideoParameters)
        {
            GetVideo(renderVideoParameters.frame, renderVideoParameters.ignore_pan_zoom, renderVideoParameters.ignore_decorations, renderVideoParameters.req_width, renderVideoParameters.req_height, renderVideoParameters.render_to_surface);
        }


		//*******************************************************************
        protected override void GetVideo(int frame, bool ignore_pan_zoom, bool ignore_decorations, int req_width, int req_height, RenderSurface surface)
		{
			// loop through all buttons
            
            GraphicsEngine.Current.ClearRenderTarget(0, 0, 0, 255);

            mBackground.RenderFrame(frame, false, ignore_decorations, req_width, req_height);

            if (ignore_decorations==false)
            {
                RenderUnhighlightedSubPictures(req_width, req_height);
            }

			// SRG HACK to make encoding use cache

			// this basicaly says that no one has changed anything to do with the menu
			if ( CGlobals.mVideoGenerateMode == VideoGenerateMode.FINAL_OUTPUT_MODE)
			{
				CGlobals.MainMenuNeedsReRender=false;
			}

			return;
		}

        //*******************************************************************
        public void RenderUnhighlightedSubPictures(int req_width, int req_height)
        {
            CImage selectIcon = null; // ### SRG TO DO CMenuSubPictureSelectIconDatabase.GetInstance().GetIcon(mMenuSubPictureStyle.SubPictureSelectIconIndex);
            RectangleF source = new RectangleF(0,0,1,1);
            Rectangle rd = new Rectangle(0,0,req_width, req_height);

            foreach (CDecoration d in mBackground.Decorations)
            {
                CMenuButton mb = d as CMenuButton;
                if (mb != null)
                {
                    CMenuSubPictureRenderMethod style = CMenuSubPictureRenderMethod.Invalid;

                    if (mb is CMenuSlideshowButton ||
                        mb is CMenuLinkSubMenuButton)
                    {
                        style = mMenuSubPictureStyle.SubPictureButtonStyle;
                    }
                    else if (mb is CMenuLinkButton)
                    {
                        style = mMenuSubPictureStyle.SubPictureMenuLinkStyle;
                    }
                    else if (mb is CMenuPlayAllButton ||
                             mb is CMenuPlayAllLoopedButton)
                    {
                        style = mMenuSubPictureStyle.SubPicturePlayMethodsStyle;
                    }

                    /*
                     * SRG to do in future builds
                    if (style == CMenuSubPictureRenderMethod.SelectIconOnRight ||
                         style == CMenuSubPictureRenderMethod.SelectIconOnLeft)
                    {
                        RectangleF coverage = mb.CoverageArea;

                        Rectangle dest = CMenuSubPictureRenderer.GetCoverageAreaForStyle(coverage, rd, mMenuSubPictureStyle.SubPictureButtonStyle);

                        GraphicsEngine.Current.DrawImage(selectIcon, source, dest, true);
                    }
                    */
                }
            }
        }

		//*******************************************************************
		public override string GetAudioFileName()
		{
			if (mMusicSlide==null) return "";
			return mMusicSlide.mName;
		}


		//*******************************************************************
		public override int GetTotalRenderLengthInFrames()
		{
			return (int) (mLength * CGlobals.mCurrentProject.DiskPreferences.frames_per_second);
		}

        //*******************************************************************
        public override int GetFinalOutputLengthInFrames()
        {
            return (int)(mLength * CGlobals.mCurrentProject.DiskPreferences.OutputVideoFramesPerSecond );
        }

		//*******************************************************************
		public void SetMusicSlide(string filename)
		{
			try
			{
                if (mMusicSlide != null)
                {
                    mMusicSlide.Dispose();
                }
				mMusicSlide	= new CMusicSlide(filename);
				mMusicSlide.CalcLengthInFrame(0);

			}
			catch (ErrorException exception)
			{
				CDebugLog.GetInstance().Error("Failed in setting menu music to '"+filename+"' :"+exception.Message);
				mMusicSlide = null;
			}
		}

		//*******************************************************************
		public void CalcLengthOfAllSlides()
		{
			if (this.mMusicSlide!=null)
			{
				this.mMusicSlide.CalcLengthInFrame(0);
			}
		}


		//*******************************************************************
		public void SetBackground(string filename)
		{ 
			CImageSlide menu_background;

            bool isVideo = false;

			if (CGlobals.IsVideoFilename(filename) ==true)
			{
                CVideoSlide vs = new CVideoSlide(filename);
                vs.Loop = true;
                vs.SetLengthWithoutUpdate(this.Length);
                menu_background = vs;
                isVideo = true;
                 
			}
			else if (CGlobals.IsImageFilename(filename)==true)
			{
				menu_background = new CStillPictureSlide(filename);
                menu_background.SetLengthWithoutUpdate(this.Length);

                ((CStillPictureSlide)menu_background).Image.NoBlankArea = true;

			}
			else
			{
				CDebugLog.GetInstance().Error("Unknown file format to set menu background '"+filename+"'");
				return ;
			}

			menu_background.PartOfAMenu=true;
			menu_background.Decorations = mBackground.Decorations;

			mBackground = menu_background;

            if (isVideo == false)
            {
                ResetPanZoom();
            }
		}

		//*******************************************************************
		public void Save(XmlElement parent, XmlDocument doc)
		{
			XmlElement main_menu = doc.CreateElement("MainMenu");

			main_menu.SetAttribute("ButtonStyle",this.mButtonStyle.ToString());
            main_menu.SetAttribute("MenuLinkStyle", this.mLinkStyle.ToString());
			main_menu.SetAttribute("MenuLayout",this.mLayout.ToString());
			main_menu.SetAttribute("Length",this.mLength.ToString());
			main_menu.SetAttribute("PlayingThumbnails", this.mPlayingThumbnails.ToString());

            if (mZoomingBackground == true)
            {
                main_menu.SetAttribute("ZoomingBackground", this.mZoomingBackground.ToString());
            }

            mMenuSubPictureStyle.Save(main_menu, doc);

            main_menu.SetAttribute("FadeAudioOut" , this.mFadeAudioOut.ToString());
           
            main_menu.SetAttribute("ID", this.mId.ToString());
       
			mBackground.Save(main_menu,doc);

			if (mMusicSlide!=null)
			{
				mMusicSlide.Save(main_menu,doc);
			}

			foreach (CMainMenu menu in this.mSubMenus)
			{
				menu.Save(main_menu, doc);
			}

			parent.AppendChild(main_menu); 
		}

		//*******************************************************************
        // Note: creates a next/previous type sub menu for this menu. 
        // Sub menu from a MenuLinkSubMenuButton can only be defined from
        // .pds file

		public CMainMenu CreateSubMenu()
		{
			if (this.mSubMenus.Count >0)
			{
				CDebugLog.GetInstance().Warning("Already a sub menu defined. Only one sub menu allowed");
				return null;
			}

			return CGlobals.mCurrentProject.CreateMenu(CGlobals.mDefaultMenuTemplate, this);
		}


		//*******************************************************************
		// private method that physicalys adds the CMenuLinkButton object
        public void AddMenuLinkButton(CMenuLinkPreviousNextButton.LinkType type, CMainMenu link_menu)
		{
			CMenuLayout layout = CMenuLayoutDatabase.GetInstance().GetLayout(mLayout);
			CMenuLinkStyle linkStyle = CMenuLinkStyleDatabase.GetInstance().GetStyle(mLinkStyle);
		
			CMenuTextSpecification vcd_spec=null;
            CMenuLinkPreviousNextButton link_button = null;

            if (type == CMenuLinkPreviousNextButton.LinkType.NEXT_MENU)
			{
				vcd_spec = layout.GetNextButtonVCDTextSpec();
                CImage next_frame = linkStyle.NextButtonImage;
				RectangleF next_coverage = layout.GetNextButtonPosition();
                link_button = new CMenuLinkPreviousNextButton(CMenuLinkPreviousNextButton.LinkType.NEXT_MENU, 
                                                     next_frame,
                                                     next_coverage,
                                                     link_menu.ID);
				mBackground.AddDecoration(link_button );
			}
			else
			{
				vcd_spec = layout.GetPreviousButtonVCDTextSpec();
                CImage prev_frame = linkStyle.PreviousButtonImage;
				RectangleF prev_coverage = layout.GetPreviousButtonPosition();
                link_button = new CMenuLinkPreviousNextButton(CMenuLinkPreviousNextButton.LinkType.PREVIOUS_MENU,
                    prev_frame, 
                    prev_coverage, 
                    link_menu.ID);
				mBackground.AddDecoration(link_button);
			}
 
			CTextStyle ts1 = new CTextStyle();
			
			if (vcd_spec!=null)
			{
				ts1 = vcd_spec.Style.Clone();
			}
				
			RectangleF c1= new RectangleF(0,0,0,0);
			CTextDecoration td1 = new CTextDecoration("1", c1,0, ts1);	// text is calculated later
			td1.VCDNumber=true;
			if (vcd_spec!=null)
			{
                td1.CoverageArea = vcd_spec.ReCalcMenuTextCoverageArea(td1, ts1.FontSize);
			}
			else
			{
				CDebugLog.GetInstance().Error("Missing vcd number on slideshow link button layout");
                td1.CoverageArea = new RectangleF(-2000, -2000, 0, 0);
			}

			link_button.AddChildDecoration(td1);
			mBackground.AddDecoration(td1);
		}

		//*******************************************************************
		public void AddSubMenu(CMainMenu menu)
		{
			if (this.mSubMenus.Count >0) return ;

			mSubMenus.Add(menu);
			menu.ParentMenu = this;

			// add next decoration

            this.AddMenuLinkButton(CMenuLinkPreviousNextButton.LinkType.NEXT_MENU,menu);
            menu.AddMenuLinkButton(CMenuLinkPreviousNextButton.LinkType.PREVIOUS_MENU, this);
		}
		
		//*******************************************************************
		public void Load(XmlElement element)
		{
			string ss = element.GetAttribute("ButtonStyle");
			if (ss!="")
			{
				this.mButtonStyle = int.Parse(ss);
			}

            ss = element.GetAttribute("MenuLinkStyle");
            if (ss != "")
            {
                this.mLinkStyle = int.Parse(ss);
            }

			ss = element.GetAttribute("MenuLayout");
			if (ss!="")
			{
				this.mLayout = int.Parse(ss);
			}

            ss = element.GetAttribute("FadeAudioOut");
            if (ss != "")
            {
                this.mFadeAudioOut = bool.Parse(ss);
            }

			string sl = element.GetAttribute("Length");
			if (sl!="")
			{
				this.mLength = float.Parse(sl,CultureInfo.InvariantCulture);
			}

			sl= element.GetAttribute("PlayingThumbnails");
			if (sl!="")
			{
				this.mPlayingThumbnails = bool.Parse(sl);
			}

            sl = element.GetAttribute("ZoomingBackground");
            if (sl != "")
            {
                this.mZoomingBackground = bool.Parse(sl);
            }

            XmlNodeList subPictureStyleElementList = element.GetElementsByTagName("SubPictureStyle");
            if (subPictureStyleElementList.Count > 0)
            {
                XmlElement subPictureStyleElement = subPictureStyleElementList[0] as XmlElement;
                mMenuSubPictureStyle.Load(subPictureStyleElement);
            }

			XmlElement e = element.GetElementsByTagName("Slide")[0] as XmlElement;
			if (e==null)
			{
				Log.Error("No background defined for menu on load");
				return ;
			}

            sl = element.GetAttribute("ID");
            if (sl != "")
            {
                mId = int.Parse(sl);
            }

            // if loading ID make sure any new one is alway after this
            if (mGlobalID <= mId)
            {
                mGlobalID = mId + 1;
            }

			mBackground = CSlide.CreateFromType(e.GetAttribute("Type")) as CImageSlide;

			try
			{
				mBackground.PartOfAMenu = true;
				mBackground.Load(e);

				CStillPictureSlide sps = mBackground as CStillPictureSlide;
                if (sps != null && sps.Image != null)
				{
                    sps.Image.NoBlankArea = true;
				}
			}
			catch (MissingFileException)
			{
			}
			catch (IgnoreOperationException)
			{
			}


			XmlNodeList list = element.GetElementsByTagName("MusicSlide");

            //
            // This list may contain more than one item.  The first item is the music for this music slide.
            // The other items are for sub menus and get loaded later.
            //
            if (list.Count >0)
			{
                XmlElement ee = list[0] as XmlElement;

                try
				{
					CMusicSlide ms = new CMusicSlide();
					ms.Load(ee);
                    if (mMusicSlide != null)
                    {
                        mMusicSlide.Dispose();
                    }
					mMusicSlide = ms;
					mMusicSlide.CalcLengthInFrame(0);
				}
				catch (ErrorException exception)
				{
					CDebugLog.GetInstance().Error("Failed To load music... Ignoring :"+exception.Message);
					mMusicSlide=null;
				}
				catch (MissingFileException)
				{
					mMusicSlide=null;
				}
				catch (IgnoreOperationException)
				{
					mMusicSlide=null;
				}

			}

			list = element.GetElementsByTagName("MainMenu");

            foreach (XmlElement main_menu_element in list)
			{
				CMainMenu menu = new CMainMenu();
                menu.Load(main_menu_element);
				menu.ParentMenu = this;
				this.mSubMenus.Add(menu);

                // SRG FIXME PHOTOCRUZ  ( this code does not work for multiple sub menus )
                break;
			}

			SetButtonStyle(mButtonStyle);
            SetLinkStyle(mLinkStyle);         
		}

		//*******************************************************************
		// useful function to determine what slideshows we link to
		public ArrayList GetSlideshowsSelectableFromMenu()
		{
			ArrayList return_list = new ArrayList();
			ArrayList all_shows = CGlobals.mCurrentProject.GetAllProjectSlideshows(false);

			foreach (CDecoration d in this.mBackground.Decorations)
			{
				CMenuSlideshowButton msb = d as CMenuSlideshowButton;
				if (msb!=null)
				{
					foreach (CSlideShow s in all_shows)
					{
						if (s.Name == msb.GetInnerImageStringId())
						{
							return_list.Add(s);
						}
					}
				}
			}

			return return_list;
	
		}

		//*******************************************************************
		public CMenuSlideshowButton GetMenuSlideshowButton(CSlideShow ss)
		{
			foreach (CDecoration d in this.mBackground.Decorations)
			{
				CMenuSlideshowButton msb = d as CMenuSlideshowButton;
				if (msb!=null)
				{
					if (msb.GetInnerImageStringId() ==ss.Name)
					{
						return msb;
					}
				}
			}
			return null;
		}



		//*******************************************************************
		public void RemoveSlideshowButton(CMenuSlideshowButton button)
		{
			this.mBackground.Decorations.Remove(button);

			if (button.AttachedChildDecorations!=null)
			{
				foreach (int id in button.AttachedChildDecorations)
				{
					CDecoration d = mBackground.GetDecorationFromID(id);
					mBackground.Decorations.Remove(d);
				}
			}

			this.RebuildMenu();
		}


		
		//*******************************************************************
		public void NullifyParent()
		{
			this.mParentMenu = null;

			RemovePreviousLinksButtons();
		}


			
		//*******************************************************************
		public void NullifyChildren()
		{
			this.mSubMenus.Clear();
			RemoveNextLinksButtons();
		}

			
		//*******************************************************************
		private void RemoveNextLinksButtons()
		{
			ArrayList a = this.GetLinkButtons();
            foreach (CMenuLinkButton lb in a)
			{
                CMenuLinkPreviousNextButton mlb = lb as CMenuLinkPreviousNextButton;
                if (mlb!=null)
                {
                    if (mlb.Link == CMenuLinkPreviousNextButton.LinkType.NEXT_MENU)
                    {
                        this.mBackground.Decorations.Remove(mlb);
                        if (mlb.AttachedChildDecorations != null)
                        {
                            foreach (int id in mlb.AttachedChildDecorations)
                            {
                                CDecoration d = mBackground.GetDecorationFromID(id);
                                mBackground.Decorations.Remove(d);
                            }
                        }
                    }
                }
			}
		}


		//*******************************************************************
		private void RemovePreviousLinksButtons()
		{
			ArrayList a = this.GetLinkButtons();
            foreach (CMenuLinkPreviousNextButton lb in a)
			{
                CMenuLinkPreviousNextButton mlb = lb as CMenuLinkPreviousNextButton;
                if (mlb!=null)
                {

                    if (mlb.Link == CMenuLinkPreviousNextButton.LinkType.PREVIOUS_MENU)
                    {
                        this.mBackground.Decorations.Remove(mlb);
                        if (mlb.AttachedChildDecorations != null)
                        {
                            foreach (int id in mlb.AttachedChildDecorations)
                            {
                                CDecoration d = mBackground.GetDecorationFromID(id);
                                mBackground.Decorations.Remove(d);
                            }
                        }
                    }
                }
			}
		}


        // After we delete, move or add slideshows.  The vcd numbers mCoverage area
        // may need to change ( becuase it's number may go from a single digit to
        // a double digit eg. "9" to "10" )
        // Rebuilding the whole menu may do too many changes to user edited
        // menus, so here we just change the mCoverage width and height of vcd
        // text decorations
        //*******************************************************************
        public void RecalcVCDLabelCoverageAreas()
        {
            CMenuLayout layout = CMenuLayoutDatabase.GetInstance().GetLayout(this.mLayout);
            int slideshow_index = 0;

            foreach (CDecoration d in this.mBackground.Decorations)
            {
                CMenuSlideshowButton bsm = d as CMenuSlideshowButton;
                if (bsm != null)
                {
                    ArrayList children = bsm.AttachedChildDecorations;
                    if (children != null)
                    {
                        foreach (int c_d in children)
                        {
                            CTextDecoration dec = mBackground.GetDecorationFromID(c_d) as CTextDecoration;

                            if (dec != null && dec.VCDNumber == true)
                            {
                                CMenuTextSpecification ts = null;

                                ts = layout.GetVCDNumberTextSpec(slideshow_index);
                                
                                if (ts == null)
                                {
                                    CDebugLog.GetInstance().Error("Missing VCD spec in RecalcVCDLabelCoverageAreas");
                                    dec.CoverageArea = new RectangleF(-2000, -2000, 0.1f, 0.1f);
                                }
                                else
                                {
                                    RectangleF r = ts.ReCalcMenuTextCoverageArea(dec, ts.Style.FontSize); ;
                                    r.X = dec.CoverageArea.X;
                                    r.Y = dec.CoverageArea.Y;
                                    dec.CoverageArea = r;
                                }
                            }
                        }
                    }
                    slideshow_index++;
                }

                // rebuild link buittons

                CMenuLinkPreviousNextButton blb = d as CMenuLinkPreviousNextButton;
                if (blb != null)
                {
                    ArrayList children = blb.AttachedChildDecorations;
                    if (children != null)
                    {
                        foreach (int c_d in children)
                        {
                            CTextDecoration dec = mBackground.GetDecorationFromID(c_d) as CTextDecoration;

                            if (dec != null)
                            {
                                CMenuTextSpecification ts = null;

                                if (blb.Link == CMenuLinkPreviousNextButton.LinkType.PREVIOUS_MENU)
                                {
                                    ts = layout.GetPreviousButtonVCDTextSpec();
                                }
                                else
                                {
                                    ts = layout.GetNextButtonVCDTextSpec();
                                }

                                if (ts == null)
                                {
                                    CDebugLog.GetInstance().Error("Missing VCD spec in RecalcVCDLabelCoverageAreas2");
                                    dec.CoverageArea = new RectangleF(-2000, -2000, 0.1f, 0.1f);
                                }
                                else
                                {
                                    RectangleF r = ts.ReCalcMenuTextCoverageArea(dec, ts.Style.FontSize);
                                    r.X = dec.CoverageArea.X;
                                    r.Y = dec.CoverageArea.Y;
                                    dec.CoverageArea = r;
                                }
                            }
                        }
                    }
                }
            }
        }

		//*******************************************************************
		public void RebuildMenu()
		{
			CMenuLayout layout = CMenuLayoutDatabase.GetInstance().GetLayout(this.mLayout);
			int slideshow_index=0;
	
			foreach (CDecoration d in this.mBackground.Decorations)
			{
				CMenuSlideshowButton bsm = d as CMenuSlideshowButton;
				if (bsm!=null)
				{
					bsm.CoverageArea = layout.GetSlideshowButtonPosition(slideshow_index) ;

					// assume only text
					ArrayList children = bsm.AttachedChildDecorations;
					if (children!=null)
					{
						foreach (int c_d in children)
						{
							CTextDecoration dec = mBackground.GetDecorationFromID(c_d) as CTextDecoration;

							if (dec!=null)
							{
								CMenuTextSpecification ts=null;

								if (dec.VCDNumber==false)
								{
									 ts = layout.GetSlideshowTextSpec(slideshow_index);
								}
								else
								{
									 ts = layout.GetVCDNumberTextSpec(slideshow_index);
								}

								if (ts==null)
								{
                                    CDebugLog.GetInstance().Error("Missing VCD spec in RecalcVCDLabelCoverageAreas3");
									dec.CoverageArea = new RectangleF(-2000,-2000,0.1f,0.1f);
								}
								else
								{
									dec.ResetLookToDefault();
									dec.TextStyle = ts.Style.Clone();
									dec.CoverageArea = ts.ReCalcMenuTextCoverageArea(dec, dec.TextStyle.FontSize);
								}
							}
						}
					}
					slideshow_index++;
				}

				// rebuild link buittons

                CMenuLinkPreviousNextButton blb = d as CMenuLinkPreviousNextButton;
				if (blb!=null)
				{
                    if (blb.Link == CMenuLinkPreviousNextButton.LinkType.NEXT_MENU)
					{
						blb.CoverageArea = layout.GetNextButtonPosition();
					}
                    if (blb.Link == CMenuLinkPreviousNextButton.LinkType.PREVIOUS_MENU)
					{
						blb.CoverageArea = layout.GetPreviousButtonPosition();
					}
					ArrayList children = blb.AttachedChildDecorations;
					if (children!=null)
					{
						foreach (int c_d in children)
						{
							CTextDecoration dec = mBackground.GetDecorationFromID(c_d) as CTextDecoration;

							if (dec!=null)
							{
								CMenuTextSpecification ts=null;

                                if (blb.Link == CMenuLinkPreviousNextButton.LinkType.PREVIOUS_MENU)
								{
									ts = layout.GetPreviousButtonVCDTextSpec();
								}
								else
								{
									ts = layout.GetNextButtonVCDTextSpec();
								}

								if (ts==null)
								{
                                    CDebugLog.GetInstance().Error("Missing VCD spec in RecalcVCDLabelCoverageAreas4");
									dec.CoverageArea = new RectangleF(-2000,-2000,0.1f,0.1f);
								}
								else
								{
									dec.ResetLookToDefault();
                                    dec.TextStyle = ts.Style.Clone();
                                    dec.CoverageArea = ts.ReCalcMenuTextCoverageArea(dec, dec.TextStyle.FontSize);
								}
							}
						}
					}
				}


				// rebuild header

				CTextDecoration td = d as CTextDecoration;
				if (td!=null)
				{
					if (td.Header==true)
					{
						td.ResetLookToDefault();
                        td.TextStyle = layout.GetHeaderTextSpec().Style.Clone();
                        td.CoverageArea = layout.GetHeaderTextSpec().ReCalcMenuTextCoverageArea(td, td.TextStyle.FontSize);
					}
				}
			}

			// better inform project to recalc vcd numbers in other menus etc
			CGlobals.mCurrentProject.RebuildVCDNumbers();
		}

		//*******************************************************************
		public void SetButtonStyle(int id)
		{
			this.mButtonStyle = id;

			CMenuButtonStyle ms = CMenuButtonStyleDatabase.GetInstance().GetStyle(id);

			foreach (CDecoration d in this.mBackground.Decorations)
			{

				if (d is CMenuSlideshowButton)
				{
                    CMenuSlideshowButton mb = d as CMenuSlideshowButton;
                    mb.Frame = ms.SlideshowButtonFrameImage;
                    mb.Mask = ms.SlideshowButtonMaskImage;
				}

                if (d is CMenuLinkSubMenuButton)
				{
                    CMenuLinkSubMenuButton mb = d as CMenuLinkSubMenuButton;
                    mb.Frame = ms.SlideshowButtonFrameImage;
                    mb.Mask = ms.SlideshowButtonMaskImage;
				}
			}
		}

        //*******************************************************************
        public void SetLinkStyle(int id)
        {
            mLinkStyle = id;

            CMenuLinkStyle ms = CMenuLinkStyleDatabase.GetInstance().GetStyle(id);

            foreach (CDecoration d in this.mBackground.Decorations)
            {
                CMenuLinkPreviousNextButton mlb = d as CMenuLinkPreviousNextButton;

                if (mlb != null)
                {
                    if (mlb.Link == CMenuLinkPreviousNextButton.LinkType.NEXT_MENU)
                    {
                        mlb.Frame = ms.NextButtonImage;
                    }
                    if (mlb.Link == CMenuLinkPreviousNextButton.LinkType.PREVIOUS_MENU)
                    {
                        mlb.Frame = ms.PreviousButtonImage;
                    }
                }
            }
        }

		//*******************************************************************
		public void SetLayout(int id)
		{
			this.mLayout = id;
			this.RebuildMenu();
		}

		//*******************************************************************
		public void ResetAllMediaStreams()
		{
			CVideoSlide vs =this.mBackground as CVideoSlide;
			if (vs!=null)
			{
				vs.ResetVideoPlayer();
			}
		}   

		//*******************************************************************
		public void RebuildToNewCanvas(CGlobals.OutputAspectRatio ratio)
		{
			this.mBackground.RebuildToNewCanvas(ratio);
		}

		
		//*******************************************************************
		public bool IsAMenuSubtitledDecoration(CDecoration dec)
		{
			if (dec as CMenuButton != null)
			{
				return true;
			}
			return false;
		}

		//*******************************************************************
		public bool IsAnySubtitledDecorationIllegal()
		{
			foreach (CDecoration d in this.mBackground.Decorations)
			{
				if (IsAMenuSubtitledDecoration(d)==true)
				{
					if (IsPartOfDecorationOffMenu(d)==true)
					{
						return true;
					}

					foreach (CDecoration d1 in this.mBackground.Decorations)
					{
						if (IsAMenuSubtitledDecoration(d1)==true &&
							d1 != d)
						{
							if (IsDecorationIntersecting(d,d1)==true)
							{
								return true;
							}
						}
					}
				}
			}

			return false;
		}


		//*******************************************************************
		static public bool IsDecorationIntersecting(CDecoration a, CDecoration b)
		{
			RectangleF dd = a.CoverageArea;
			dd.Inflate(0.02f,0.02f);
			if (dd.IntersectsWith(b.CoverageArea) == true)
			{
				return true;
			}
			return false;
		}

		//*******************************************************************
		static public bool IsPartOfDecorationOffMenu(CDecoration dec)
		{
			if (dec.CoverageArea.X <0 ||
				dec.CoverageArea.Y <0 ||
				dec.CoverageArea.X + dec.CoverageArea.Width > 1 ||
				dec.CoverageArea.Y + dec.CoverageArea.Height >1)
			{
				return true;
			}
			return false;
		}

        //*******************************************************************
        // this function checks the integrity of the menu link system, which
        // was introduced in version 2.11.1 onwards.  I.e. for .pds files
        // which were created earlier than this version, then the menu link
        // ids will be -1, so we need hook them up accordinagly.
        public void CheckAndFixMenuIntegrity(CMainMenu parent)
        {
            // fix any next links on this menu
            ArrayList link_buttons = this.GetLinkButtons();
            foreach (CMenuLinkButton mlb in link_buttons)
            {
                CMenuLinkPreviousNextButton mpnlb = mlb as CMenuLinkPreviousNextButton;
                if (mpnlb!=null &&
                    mpnlb.MenuLinkID<0)
                {
                    if (mpnlb.Link == CMenuLinkPreviousNextButton.LinkType.PREVIOUS_MENU)
                    {
                      if (parent!=null)
                      {
                        mpnlb.MenuLinkID = parent.ID;
                      }
                      else
                      {
                          CDebugLog.GetInstance().Error("Error in CheckAndFixMenuIntegrity, parent null");
                      }
                    }
                    else if (mpnlb.Link == CMenuLinkPreviousNextButton.LinkType.NEXT_MENU)
                    {
                        if (this.mSubMenus.Count >1 ||
                             this.mSubMenus.Count==0)
                        {
                            CDebugLog.GetInstance().Error("Error in CheckAndFixMenuIntegrity, invalid submenus count");
                        }
                        else
                        {
                            mpnlb.MenuLinkID = (this.mSubMenus[0] as CMainMenu).ID;
                        }
                    }
                }
            }

            foreach (CMainMenu sm in this.mSubMenus)
            {
                sm.CheckAndFixMenuIntegrity(this);
            }
        }

        //*******************************************************************
        private void ResetPanZoom()
        {
            if (mZoomingBackground == false)
            { 
                mBackground.PanZoom.StartArea = new RectangleF(0, 0, 1, 1);
                mBackground.PanZoom.EndArea = new RectangleF(0, 0, 1, 1);
                mBackground.UsePanZoom = false;
            }
            else
            {
                mBackground.PanZoom.StartArea = new RectangleF(0, 0, 1, 1);
                mBackground.PanZoom.EndArea = new RectangleF(0.2f, 0.2f, 0.6f, 0.6f);
                mBackground.UsePanZoom = true;
            }
        }

        //*******************************************************************      
        public override double GetLengthInSeconds()
        {
            return (double)mLength;
        }
	}
}
