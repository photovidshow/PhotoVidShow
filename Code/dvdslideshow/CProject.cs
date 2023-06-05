/*
 * Created by SharpDevelop.
 * User: user
 * Date: 03/01/2005
 * Time: 13:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using ManagedCore;
using DVDSlideshow.Memento;
using System.Security.Permissions;
using System.Globalization;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
	/// <summary>
	/// Description of CProject.	
	/// </summary>
	/// 

	public delegate void ProjectChanged(bool store_memento, string description);

	[StrongNameIdentityPermissionAttribute(SecurityAction.LinkDemand, PublicKey = MyPublicKey.Value)]

	public class CProject : DVDSlideshow.Memento.IOriginator
	{
        private string              mPreMenuMovie = "";
		public bool					mChangedSinceSave=false;
		public string				mFilename;
		public string 				mName="Untitled";
		private CMainMenu			mMainMenu;
		private ArrayList		 	mSlideShows;
		private bool				mForceNoMemento=false;      // if set to true each project change will NOT store a momento
        private bool                mIgnoreProjectChanges=false; // if true any project changes will not do anything.
        private CSlideShow          mPreMenuSlideshow = null;   // used on DVD 
      
		private static ArrayList			mInformWhenProjectChanges = new ArrayList();
		
		private CDiskAuthoringPreferences mDiskPreferences = new CDiskAuthoringPreferences();

        //
        // Any slideshow in a project which contains this identity string as it's name means
        // it is a pre menu (or intro) slideshow.
        //
        public const string PreMenuIdentityString = "__premenu__";

        public CSlideShow PreMenuSlideshow
        {
            get { return mPreMenuSlideshow; }
            set { mPreMenuSlideshow = value; }
        }

        public string PreMenuMovieFileName
        {
            get { return mPreMenuMovie; }
            set { mPreMenuMovie = value; }
        }

		public CMainMenu	MainMenu
		{
			get { return mMainMenu ; }
            set { mMainMenu = value; }
		}

		public bool ForceNoMemento
		{
			get
			{
				return mForceNoMemento;
			}
			set
			{
				mForceNoMemento=value;
			}
		}

        public bool IgnoreProjectChanges
        {
            get
            {
                return mIgnoreProjectChanges;
            }
            set
            {
                mIgnoreProjectChanges = value;
            }
        }

		public CDiskAuthoringPreferences DiskPreferences
		{
			get { return mDiskPreferences ; }
		}

		public static ArrayList InformWhenProjectChange
		{
			get { return mInformWhenProjectChanges ; }
		}

		//*******************************************************************
		public CProject()
		{
			mSlideShows = new ArrayList();
			CGlobals.mCurrentProject = this ;
		}


		//*******************************************************************
		public CSlideShow CreateNewSlideshow(string name, CMainMenu menu, CSlideShow template)
		{
			CSlideShow slide_show = new CSlideShow(name);
            if (template != null)
            {
                slide_show.mDefaultSlide.DisplayLength = template.mDefaultSlide.DisplayLength;
                slide_show.mDefaultSlide.UsePanZoom = template.mDefaultSlide.UsePanZoom;
            }
			AddSlideshow(slide_show, menu) ;
			return slide_show;
		}

	
		//*******************************************************************
		public void RemoveSlideshow(CSlideShow ss, out bool removed_menu)
		{
			removed_menu = false;

			CMainMenu m = GetMenuWhichContainsSlideshow(ss);

			if (m==null)
			{
				CDebugLog.GetInstance().Warning("No menu contains a link to the removed slideshow: "+ss.Name);
				return ;
			}
			mSlideShows.Remove(ss);

			m.RemoveSlideshowButton(m.GetMenuSlideshowButton(ss));

			// if it was the only slideshow in the menu delete menu

			removed_menu = CheckForNullMenu(m);

			this.RebuildVCDNumbers();

		}


		//*******************************************************************
		// check if menu has no slideshows, removes menu if this is the case
		// return true if it did infact remove it, else returns false;
		private bool CheckForNullMenu(CMainMenu m)
		{
            int c = m.GetNumSlideShowButtons();

			if (c > 0) return false;

			CMainMenu parent = m.ParentMenu;

			// removed first menu ouch !!
			if (parent==null)
			{
				if (m.SubMenus.Count <1)
				{
					CDebugLog.GetInstance().Error("Can not remove menu as it is the last one");
					return false ;
				}

				this.mMainMenu = m.SubMenus[0] as CMainMenu;
				this.mMainMenu.NullifyParent();
			}
			else
			{
				// is last item
				if (m.SubMenus.Count <1)
				{
					parent.NullifyChildren();
				}
				else
				{
					parent.SubMenus[0] = m.SubMenus[0] ;
					(m.SubMenus[0] as CMainMenu).ParentMenu = parent;
				}
			}
			return true;

		}

			


		//*******************************************************************
		public void MoveSlideshowToAnotherMenu(CSlideShow ss, CMainMenu old_menu, CMainMenu new_menu,  out bool removed_menu)
		{
			removed_menu=false ;

			if (new_menu.ID == old_menu.ID)
			{
				CDebugLog.GetInstance().Error("Can not move slideshow button to same menu");
				return ;
			}

			CMenuSlideshowButton ms = old_menu.GetMenuSlideshowButton(ss);
			if (ms==null) return ;

			// check if menu can grow

			if (new_menu.TestNeedToUpgradeMenuLayoutToCaterForBewButton()==false)
			{
				CDebugLog.GetInstance().Warning("Can not move slideshow to menu as it is full");
				return ;
			}

            new_menu.BackgroundSlide.Decorations.Add(ms);
            old_menu.BackgroundSlide.Decorations.Remove(ms);

			ArrayList child_linked_decorations = ms.AttachedChildDecorations;
			if (child_linked_decorations!=null)
			{
				foreach (int d in child_linked_decorations)
				{
                    CDecoration dec = old_menu.BackgroundSlide.GetDecorationFromID(d);
					if (dec==null)
					{
						CDebugLog.GetInstance().Error("Unknown decoration id ("+d+") in menu");
					}
					else
					{
                        new_menu.BackgroundSlide.Decorations.Add(dec);
                        old_menu.BackgroundSlide.Decorations.Remove(dec);
					}
				}
			}

			removed_menu= CheckForNullMenu(old_menu);

			new_menu.RebuildMenu();
			if (removed_menu==false)
			{
				old_menu.RebuildMenu();
			}

		}

		//*******************************************************************
		CSlideShow ContainsSlideShow(string name)
		{
			foreach (CSlideShow i in mSlideShows)
			{
				if (i.Name == name) 
				{
					return i;
				}
			}
			return null ;
		}

        //*******************************************************************
        // private method to generate next unique slideshow id name
        private string GetNextSlideShowName()
        {
            int number = 1;
            string name = "My slideshow 1";
            bool found = false;

            while (found == false)
            {
                string cand_name = "My slideshow ";
                if (number > 0) cand_name += number.ToString();

                if (ContainsSlideShow(cand_name) == null)
                {
                    found = true;
                    name = cand_name;
                }
                else
                {
                    number++;
                }
            }
            return name;
        }

		//*******************************************************************
		// no name just given it untitled
		public CSlideShow CreateNewSlideshow(CMainMenu menu, CSlideShow template)
		{
            string name = GetNextSlideShowName();
			return CreateNewSlideshow(name, menu, template);
		}

        //*******************************************************************
        public void AddSlideshow(CSlideShow slide_show, CMainMenu for_menu)
        {
            AddSlideshow(slide_show, for_menu, true);
        }

        //*******************************************************************
        public void AddSlideshow(CSlideShow slide_show, CMainMenu for_menu, bool createMenuIfNonExist)
		{
			// if first slide_show and no menu then create one (unless we specify otherwise. e.g. photocruz burn does not use menus)

			if (for_menu==null && this.mMainMenu ==null && createMenuIfNonExist == true)
			{
				CreateMenu(CGlobals.mDefaultMenuTemplate,null);
			}

			mSlideShows.Add(slide_show);

			if (for_menu==null)
			{
				if (this.mMainMenu!=null) this.mMainMenu.AddSlideShow(slide_show);
			}
			else
			{
				for_menu.AddSlideShow(slide_show);
			}
			this.RebuildVCDNumbers();

			DeclareChange("Added new Slideshow");
		}

		//*******************************************************************
		public CSlideShow GetSlideshow(string name)
		{
			foreach (CSlideShow i in mSlideShows)
			{
				if (i.Name == name) return i;
			}

            if (mPreMenuSlideshow != null && mPreMenuSlideshow.Name == name)
            {
                return mPreMenuSlideshow;
            }

			Console.WriteLine("Error: could not find slideshow "+name+" in project");
			return null ;
		}

        //*******************************************************************
        public CSlideShow GetSlideshowFromIndex(int index)
        {
            if (mSlideShows.Count > index)
            {
                return mSlideShows[index] as CSlideShow;
            }
            return null;
        }

        //*******************************************************************
        public CMainMenu GetMenu(int id)
        {
            ArrayList sb = this.mMainMenu.GetSelfAndAllSubMenus();

            for (int i = 0; i < sb.Count; i++)
            {
                if ((sb[i] as CMainMenu).ID == id)
                {
                    return sb[i] as CMainMenu;
                }
            }

            ManagedCore.CDebugLog.GetInstance().Error("Could not find menu id:" + id);
            return null;
        }
		
		//*******************************************************************
		public int GetSlideshowIndex(string name)
		{
			int count = 0;
			foreach (CSlideShow i in mSlideShows)
			{
				if (i.Name == name) return count;
				count++;

			}
			Console.WriteLine("Error: could not find slideshow "+name+" in project");
			return 0 ;
		}

		//*******************************************************************
		private XmlDocument SerialiseToDocument(bool storeRequiredAnimatedEffects)
		{
			System.Xml.XmlDocument my_doc = new XmlDocument();

			XmlElement dvdslide = my_doc.CreateElement("DVDProject");

            dvdslide.SetAttribute("Version", CGlobals.CompleteVersionString());

			my_doc.AppendChild(dvdslide);
         
            if (storeRequiredAnimatedEffects == true)
            {
                CAnimatedDecorationEffectDatabase.GetInstance().Save(dvdslide, my_doc);
            }
     
			foreach (CSlideShow i in mSlideShows)
			{
				i.Save(dvdslide,my_doc, false);
			}

            if (mPreMenuMovie != "" || mPreMenuSlideshow != null)
            {
                XmlElement pre_menu_element = my_doc.CreateElement("PreMenu");

                if (mPreMenuMovie != "")
                {
                    pre_menu_element.SetAttribute("PreMenuVideo", this.mPreMenuMovie);
                }
                else
                {
                    mPreMenuSlideshow.Save(pre_menu_element, my_doc, false);
                }

                dvdslide.AppendChild(pre_menu_element);
            }

			if (mMainMenu!=null)
			{
				mMainMenu.Save(dvdslide,my_doc);
			}

			DiskPreferences.Save(dvdslide, my_doc);

			return my_doc;
		}

		//*******************************************************************
		private void Save(string filename)
		{
			System.Xml.XmlDocument my_doc = SerialiseToDocument(true);
			my_doc.Save(filename);
			mChangedSinceSave=false;
		}


		//*******************************************************************
		public void Save()
		{
			Save(mName);
		}


		//*******************************************************************
		private void DeSerialise(XmlDocument my_doc, 
								 bool store_memento, 
			                     ManagedCore.Progress.IProgressCallback progress_callback)
		{
            ManagedCore.MissingFilesDatabase.GetInstance().AbortedLoad = false;

			System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

			mForceNoMemento = true;

			this.mDiskPreferences = new CDiskAuthoringPreferences();

            XmlNodeList project_version_list = my_doc.GetElementsByTagName("DVDProject");

            if (project_version_list.Count > 0)
            {
                XmlElement e = project_version_list[0] as XmlElement;
                if (e != null)
                {
                    // Contains Version of pds file
                    string ss = e.GetAttribute("Version");
                    if (ss != "")
                    {
                        ManagedCore.CDebugLog.GetInstance().Trace(".pds file " + ss);

                        try
                        {
                            ss = ss.Substring(8);
                            string[] ss1 = ss.Split('.');
                            int version = (int.Parse(ss1[0]) * 10000) + (int.Parse(ss1[1]) * 100) + (int.Parse(ss1[2]));

                            string our_version_string = CGlobals.CompleteVersionString();
                            our_version_string = our_version_string.Substring(8);
                            string[] our_version_string1 = our_version_string.Split('.');
                            int our_version = (int.Parse(our_version_string1[0]) * 10000) + (int.Parse(our_version_string1[1]) * 100) + (int.Parse(our_version_string1[2]));

                            if (our_version < version)
                            {
                                System.Windows.Forms.DialogResult result =
                                 UserMessage.Show("Project file was created with a newer version of PhotoVidShow V" + ss + "\n\r" +
                                       "Current installed version is V" + our_version_string + "\n\r\n\r" +
                                       @"It is recommended to update to the newest version of PhotoVidShow.  Visit http://www.photovidshow.com/download.html" + "\n\r\n\r" +
                                       @"Select 'Retry' to attempt to load this project file anyway or select 'Cancel' to abort.", "Error: Newer version of project file",
                                    System.Windows.Forms.MessageBoxButtons.RetryCancel, System.Windows.Forms.MessageBoxIcon.Exclamation);

                                if (result == System.Windows.Forms.DialogResult.Cancel)
                                {
                                    ManagedCore.MissingFilesDatabase.GetInstance().AbortedLoad = true;
                                    return;
                                }
                            }
                        }  
                        catch
                        {
                        } 
                    }
                   
                }
            }

			XmlNodeList pref_list =my_doc.GetElementsByTagName("DiskPreferences");
			if (pref_list.Count>0)
			{
				DiskPreferences.Load(pref_list[0] as XmlElement);
			}

            XmlNodeList list = my_doc.GetElementsByTagName("DecorationEffectsDatabase");

            if (list.Count > 0)
            {
                XmlElement element = list[0] as XmlElement;
                if (element != null)
                {
                    CAnimatedDecorationEffectDatabase.GetInstance().Load(element);
                }
            }

			list =my_doc.GetElementsByTagName("Slideshow");

            if (progress_callback != null)
            {
                progress_callback.Begin(0, list.Count);
                progress_callback.SetText("Loading project...");
            }

			this.mSlideShows.Clear();

			int ses=1;
			foreach(XmlElement element in list)
			{
				CSlideShow slideshow = new CSlideShow("");
				slideshow.Load(element);

                if (slideshow.Name == CProject.PreMenuIdentityString)
                {
                    mPreMenuSlideshow = slideshow;
                }
                else
                {
                    mSlideShows.Add(slideshow);
                }

				if (progress_callback!=null)
				{
					progress_callback.StepTo(ses);
				}
				ses++;
			}

            XmlNodeList PreMenuList = my_doc.GetElementsByTagName("PreMenu");

            if (PreMenuList.Count > 0)
            {
                XmlElement pm = PreMenuList[0] as XmlElement;
                string s1 = pm.GetAttribute("PreMenuVideo");
                if (s1 != "")
                {
                    this.mPreMenuMovie = s1;
                }
            }

			XmlNodeList menu_list =my_doc.GetElementsByTagName("MainMenu");
			if (menu_list.Count>0)
			{
				mMainMenu = new CMainMenu();
				mMainMenu.Load(menu_list[0] as XmlElement);
                mMainMenu.CheckAndFixMenuIntegrity(null);
			}
         
			mForceNoMemento = false;

			DeclareChange(store_memento,"Project DeSerialise");		
		}

		//*******************************************************************
		public string GetNameWithoutPath()
		{
			return System.IO.Path.GetFileNameWithoutExtension(mName);
		}


		//*******************************************************************
		public void Load(object o)
		{
			ManagedCore.Progress.IProgressCallback progress_callback = (ManagedCore.Progress.IProgressCallback) o;
						
			if (mFilename==null)
			{
				CDebugLog.GetInstance().Error("Project filename not set on call to Load");
				return ;
			}

			CDebugLog.GetInstance().Trace("Loading project '"+mFilename+"'");
			mName = mFilename ;

            bool failed = false;

      		System.Xml.XmlDocument my_doc = new XmlDocument();
			try
			{
				my_doc.Load(mFilename);
			}
			catch(Exception e)
			{
				ManagedCore.CDebugLog.GetInstance().Error("Exception occurred when loading project '"+mFilename+"' :"+e.Message);

                ManagedCore.MissingFilesDatabase.GetInstance().AbortLoad(e.Message);

                failed = true;
			}

            if (failed == false)
            {

                // If older version of PhotoVidShow, upgrade
                if (mFilename.EndsWith(".pds2") == false && mFilename.EndsWith(".pds"))
                {
                    mFilename = mFilename.Replace(".pds", ".pds2");
                    mName = mFilename;
                }

                this.DeSerialise(my_doc, false, progress_callback);
            }

            if (progress_callback != null)
            {
                progress_callback.End();
            }
			mChangedSinceSave=false;
		}


		//******************************************************************
		public CMainMenu CreateMenu(string backimage, CMainMenu previous)
		{
			if (previous==null && mMainMenu!=null)
			{
				Console.WriteLine("Error: creating first main menu but already exits !!!");
				return null;
			}

            CImageSlide menu_background = null;
            if (CGlobals.IsVideoFilename(backimage) == true)
            {
                CVideoSlide vs = new CVideoSlide(backimage); ;             
                vs.Loop = true;
                vs.SetLengthWithoutUpdate(CMainMenu.DefaultMenuLength);
                menu_background = vs;
            }
            else
            {
                menu_background = new CStillPictureSlide(backimage, false);
            }

            CTextDecoration td1 = new CTextDecoration("Menu title text here", new RectangleF(0.0f, 0.0f, 0.0f, 0.0f), 0, 15);
			td1.Header=true;
			ArrayList decorations = new ArrayList();
			decorations.Add(td1);
		
			CMainMenu ret = null;

			if (previous==null)
			{
				mMainMenu= new CMainMenu(menu_background,decorations);
				ret = mMainMenu;
			}
			else
			{
				ret = new CMainMenu(menu_background,decorations);
				previous.AddSubMenu(ret);
			}

			return ret ;
		
		}


		//*******************************************************************
		public CMainMenu GetMenuWhichContainsSlide(CSlide slide)
		{
			if (mMainMenu==null) return null;

			ArrayList sb = this.mMainMenu.GetSelfAndAllSubMenus();

			for (int i=0;i<sb.Count;i++)
			{
				if ((sb[i] as CMainMenu).BackgroundSlide.ID == slide.ID)
				{
					return sb[i] as CMainMenu ;
				}
			}
			return null;
		}

		//*******************************************************************
		public CMainMenu GetMenuWhichContainsSlideshow(CSlideShow ss)
		{
			ArrayList sb = this.mMainMenu.GetSelfAndAllSubMenus();

			for (int i=0;i<sb.Count;i++)
			{
				foreach (CMainMenu menu in sb)
				{
                    foreach (CDecoration d in menu.BackgroundSlide.Decorations)
					{
						CMenuSlideshowButton msb = d as CMenuSlideshowButton;
						if (msb !=null)
						{
							if (msb.GetInnerImageStringId() == ss.Name)
							{
								return menu ;
							}
						}
					}
				}
			}
			return null;
		}

        //*******************************************************************
        public ArrayList GetAllProjectSlideshows(bool includePreMenu)
        {
            if (mPreMenuSlideshow == null || includePreMenu == false)
            {
                return mSlideShows;
            }
            ArrayList list = mSlideShows.Clone() as ArrayList;
            list.Add(mPreMenuSlideshow);
            return list;
        }

        //*******************************************************************
        private long GetMenuRecursiveDiskUsageEstimation(CMainMenu menu, CGlobals.VideoType outputType, CGlobals.MP4Quality quality, int height, float fps)
        {
            long size = menu.GetDiskUsageEstimation(outputType, quality, height, fps);

            ArrayList subMenus = menu.SubMenus;
            foreach (CMainMenu subMenu in subMenus)
            {
                size += GetMenuRecursiveDiskUsageEstimation(subMenu, outputType, quality, height, fps);
            }

            return size;
        }

		//*******************************************************************
        public long GetDiskUsageEstimation(CGlobals.VideoType outputType, CGlobals.MP4Quality outputQuality, int height, float fps, bool include_origina_pic, bool include_org_vids, bool includeMenus)
		{       
			long total_bytes =0;
             
            ArrayList slideshows = GetAllProjectSlideshows(true);
            foreach (CSlideShow ss in slideshows)
			{
				total_bytes += ss.GetDiskUsageEstimation(outputType, outputQuality, height, fps);
			}

            if (includeMenus == true && mMainMenu !=null)
            {
                total_bytes += GetMenuRecursiveDiskUsageEstimation(mMainMenu, outputType, outputQuality, height, fps);
			}

			// include original pics?
			if (include_origina_pic==true || include_org_vids==true)
			{
				ArrayList filenames =GetSlidesSourceFilesNames();
				foreach (string s in filenames)
				{
					bool add=false;

					if (CGlobals.IsImageFilename(s) && include_origina_pic==true)
					{
						add=true;
					}
					else if (CGlobals.IsVideoFilename(s) && include_org_vids==true)
					{
						add=true;
					}

					if (add==true)
					{
						System.IO.FileInfo f = new System.IO.FileInfo(s);
						total_bytes+=f.Length;
					}
				}
			}


			return total_bytes;

		}

		//*******************************************************************
		public void DeclareChange(string description)
		{
			DeclareChange(true, description);
			
		}

		//*******************************************************************
		public void DeclareChange(bool store_memento, string description)
		{
            if (mIgnoreProjectChanges == true)
            {
                return;
            }

			if (mForceNoMemento==true)
			{
				store_memento=false;
			}

			if (store_memento==true)
			{
				mChangedSinceSave=true;
			}

			// inform any gui parts that we have changed
			foreach (ProjectChanged pc in mInformWhenProjectChanges)
			{
				pc(store_memento, description);
			}

			CGlobals.MainMenuNeedsReRender = true;
		}


		//*******************************************************************
		public void ResetAllMediaStreams()
		{
            ArrayList slideshows = GetAllProjectSlideshows(true);
            foreach (CSlideShow ss in slideshows)
			{
				ss.ResetAllMediaStreams();
			}

			if (mMainMenu!=null)
			{
				ArrayList list = mMainMenu.GetSelfAndAllSubMenus();
				foreach (CMainMenu mm in list)
				{
					mm.ResetAllMediaStreams();
				}
			}
		}

		//*******************************************************************
		public int GetTotalNumberOfSlides()
		{
            ArrayList slideshows = GetAllProjectSlideshows(true);

			int total=0;
            foreach (CSlideShow ss in slideshows)
			{
				total+=ss.mSlides.Count;
			}

			return total;
		}

		//*******************************************************************
        // recacil frame length based on current fps
		public void RecalcAllFrameLengths()
		{
            bool preview = false;
            if (CGlobals.mVideoGenerateMode == VideoGenerateMode.EDIT_PRIVIEW_MODE)
            {
                preview = true;
                CGlobals.mCurrentProject.DiskPreferences.RenderToEncodeFPSRatio = 1;
            }

            ArrayList slideshows = GetAllProjectSlideshows(true);
            foreach (CSlideShow ss in slideshows)
			{
                if (preview == false)
                {
                    CGlobals.mCurrentProject.DiskPreferences.RenderToEncodeFPSRatio = ss.GetMaxNumberRequiredMotionBlurSubFrames();
                }
                
				ss.CalcLengthOfAllSlides();
			}

			if (mMainMenu!=null)
			{
				ArrayList list = mMainMenu.GetSelfAndAllSubMenus();
				foreach (CMainMenu mm in list)
				{
                    if (preview == false)
                    {
                        CGlobals.mCurrentProject.DiskPreferences.RenderToEncodeFPSRatio = mm.GetMaxNumberRequiredMotionBlurSubFrames();

                    }
					mm.CalcLengthOfAllSlides();
				}
			}

			// from pal to ntsc is a project change
			this.DeclareChange(false,"Changed Frame rate");
		}


		//*******************************************************************
		public void RebuildToNewCanvas(CGlobals.OutputAspectRatio ratio)
		{
			// loop through all slides and re ajust their canvas size

			if (ratio == CGlobals.OutputAspectRatio.TV16_9)
			{
				this.DiskPreferences.SetToWidescreenTV16by9();
			}
			if (ratio == CGlobals.OutputAspectRatio.TV4_3)
			{
				this.DiskPreferences.SetToNormalTV4by3();
			}

			if (ratio == CGlobals.OutputAspectRatio.TV221_1)
			{
				this.DiskPreferences.SetToNormalTV221by1();
			}

            if (GraphicsEngine.Current != null)
            {
                GraphicsEngine.Current.Clean();
            }

            ArrayList slideshows = GetAllProjectSlideshows(true);
            foreach (CSlideShow ss in slideshows)
			{
				ss.RebuildToNewCanvas(ratio);
			}

			if (mMainMenu!=null)
			{
				ArrayList list = mMainMenu.GetSelfAndAllSubMenus();
				foreach (CMainMenu mm in list)
				{
					mm.RebuildToNewCanvas(ratio);
				}
			}
			this.DeclareChange(false,"Rebuild to new canvas");
		}

        //*******************************************************************
        public void DeclareSlideAspectChange(CGlobals.OutputAspectRatio oldAspect, CGlobals.OutputAspectRatio newAspect)
        {
            ArrayList slideshows = GetAllProjectSlideshows(true);
            foreach (CSlideShow ss in slideshows)
            {
                ss.DeclareSlideAspectChange(oldAspect, newAspect);
            }
        }

		//*******************************************************************
		public DVDSlideshow.Memento.Memento CreateMemento()
		{
			XmlDocument doc = SerialiseToDocument(false); // no point saving animated database as well for undo/redo
			ArrayList handles = new ArrayList();

			Memento.Memento m = new CProjectMemento(this, doc, handles);
			return m;
		}

		//*******************************************************************
		public void SetMemento(Memento.Memento m)
		{
			CProjectMemento pm = m as CProjectMemento;
			if (pm==null)
			{
				CDebugLog.GetInstance().Error("SetMemento was not given an expected ProjectMemento");
				return ;
			}

			this.DeSerialise(pm.Doc,false,null);
			mChangedSinceSave=true;
		}


        //*******************************************************************
        // gets the user text string from the menu which is linked to a 
        // slideshow i.e. The text the user has reference the slideshow
        public string GetSlideShowReferenceTextInMenu(CSlideShow ss)
        {
            ArrayList menus = this.mMainMenu.GetSelfAndAllSubMenus();
            foreach (CMainMenu mm in menus)
            {
                CMenuSlideshowButton msb = mm.GetMenuSlideshowButton(ss);
                if (msb != null)
                {
                    CTextDecoration td = msb.GetNonVCDAttachedTextDecoration(mm);
                    if (td != null)
                    {
                        return td.mText;
                    }
                    return "Unknown";
                }
            }
            return "Unknown";
        }
		

		//*******************************************************************
		// Get a string list of all the source files for all the slides in the slideshow.
		// NOTE we ignore menu and decoration source files

		public ArrayList GetSlidesSourceFilesNames()
		{
			ArrayList return_list = new ArrayList();
            ArrayList slideshows = this.GetAllProjectSlideshows(true);

            foreach (CSlideShow s in slideshows)
			{
				foreach( CSlide slide in s.mSlides)
				{
					string filename = slide.GetSourceFileName();

					if (filename!="" && (slide is CBlankStillPictureSlide == false))
					{
						if (return_list.Contains(filename)==false)
						{
							return_list.Add(filename);
						}
					}

                    CImageSlide imageSlide = slide as CImageSlide;
                    if (imageSlide != null)
                    {
                        ArrayList decors = imageSlide.GetAllAndSubDecorations();
                        foreach (CDecoration d in decors)
                        {
                            CImageDecoration imageDecor = d as CImageDecoration;
                            string decorFile = "";

                            if (imageDecor != null && imageDecor.IsUserDecoration() == true)
                            {
                                CClipArtDecoration cad = imageDecor as CClipArtDecoration;
                                if (cad != null && cad.mImage != null)
                                {
                                    decorFile = cad.mImage.ImageFilename;
                                }
                                CVideoDecoration vd = imageDecor as CVideoDecoration;
                                if (vd != null && vd.Player != null)
                                {
                                    decorFile = vd.Player.mFileName;
                                }
                            }

                            if (decorFile != "" && return_list.Contains(decorFile) == false)
                            {
                                return_list.Add(decorFile);
                            }
                        }
                    }
				}
			}
			return return_list;
		}

        //*******************************************************************
        public void ImportSlideshows(List<CSlideShow> slideshows, List<String> slideshows_menu_text)
        {
            for (int i = 0; i < slideshows.Count; i++)
            {
                CSlideShow ss = slideshows[i];
                String text = slideshows_menu_text[i];

               // this.AddSlideshow(

                ArrayList menus = mMainMenu.GetSelfAndAllSubMenus();

                CMainMenu last_menu = null;
                foreach (CMainMenu mm in menus)
                {
                    if (mm.SubMenus.Count == 0)
                    {
                        last_menu = mm;
                        break;
                    }
                }

                // should never happen
                if (last_menu == null)
                {
                    CDebugLog.GetInstance().Error("Could not find last menu when importing");
                    return;
                }

                if (last_menu.TestNeedToUpgradeMenuLayoutToCaterForBewButton() == false)
                {
                    last_menu = last_menu.CreateSubMenu();
                }

                ss.Name = GetNextSlideShowName();

                this.mSlideShows.Add(ss);
                last_menu.AddSlideShow(ss, text);

            }

            this.RebuildVCDNumbers();

            DeclareChange("Added new Slideshows");

        }

        //*******************************************************************
        // this methods recalcs all the vcd text numbers in all the menus to
        // ensure the match to how a vcd/svcd player would interpret the
        // current project

        public void RebuildVCDNumbers()
		{
			if (this.mMainMenu==null) return ;

			ArrayList menus = this.mMainMenu.GetSelfAndAllSubMenus();

			int current_vcd_number =1;

			foreach (CMainMenu m in menus)
			{
                bool recalc_vcd_coverage_area = false; 

				// number slideshow buttons first as they are always above link buttons
				ArrayList slideshow_buttons = m.GetSlideShowButtons();

				foreach (CMenuSlideshowButton msb in slideshow_buttons)
				{
					ArrayList attached_decors =msb.AttachedChildDecorations;
					if (attached_decors!=null)
					{
						foreach (int dd in attached_decors)
						{
                            CTextDecoration td = m.BackgroundSlide.GetDecorationFromID(dd) as CTextDecoration;
							if (td!=null)
							{
								if (td.VCDNumber==true)
								{
                                    if (td.mText != current_vcd_number.ToString())
                                    {
                                        recalc_vcd_coverage_area = true;
                                    }
									td.mText=current_vcd_number.ToString();
									current_vcd_number++;
								}
							}
						}
					}
				}


				// ok now the link buttons

				ArrayList link_buttons =	m.GetLinkButtons();

				// loop through forawrd ones before backwards ones

				for (int j=0;j<2;j++)
				{
                    foreach (CMenuLinkButton mlb in link_buttons)
					{
                        // SRG FIXE ME, DOES NOT WORK FOR SUBMENULINKBUTTONS
                        CMenuLinkPreviousNextButton lb = mlb as CMenuLinkPreviousNextButton;
                        if (lb != null)
                        {
                            if (j == 0 && lb.Link == CMenuLinkPreviousNextButton.LinkType.NEXT_MENU)
                            {
                                continue;
                            }

                            if (j == 1 && lb.Link == CMenuLinkPreviousNextButton.LinkType.PREVIOUS_MENU)
                            {
                                continue;
                            }


                            ArrayList attached_decors = mlb.AttachedChildDecorations;
                            if (attached_decors != null)
                            {

                                foreach (int dd in attached_decors)
                                {
                                    CTextDecoration td = m.BackgroundSlide.GetDecorationFromID(dd) as CTextDecoration;
                                    if (td != null)
                                    {
                                        if (td.VCDNumber == true)
                                        {
                                            if (td.mText != current_vcd_number.ToString())
                                            {
                                                recalc_vcd_coverage_area = true;
                                            }
                                            td.mText = current_vcd_number.ToString();
                                            current_vcd_number++;
                                        }
                                    }
                                }
                            }
                        }
					}
				}
                if (recalc_vcd_coverage_area == true)
                {
                    m.RecalcVCDLabelCoverageAreas();  // if a vcd number has changed, we best recalc it's coverage width 
                }
			}
		}

        //*******************************************************************
        // Returns every decoration which exists in the project  
        // Note (does not return CGroupDecorations as such, but does return all decorations inside them)
        public List<CDecoration> GetAllDecorations()
        {
            List<CDecoration> items = new List<CDecoration>();

            // loop through all anmated decorations and possible rename

            ArrayList slideshows = GetAllProjectSlideshows(true);
            foreach (CSlideShow slideshow in slideshows)
            {
                foreach (CSlide slide in slideshow.mSlides)
                {
                    CImageSlide imageSlide = slide as CImageSlide;
                    if (imageSlide != null)
                    {
                        ArrayList decorations = imageSlide.GetAllAndSubDecorations();
                        foreach (CDecoration dec in decorations)
                        {
                            items.Add(dec);
                        }
                    }
                }
            }

            ArrayList menus = this.mMainMenu.GetSelfAndAllSubMenus();

            foreach (CMainMenu menu in menus)
            {
                if (menu.BackgroundSlide != null)
                {
                    ArrayList decorations = menu.BackgroundSlide.GetAllAndSubDecorations();
                    foreach (CDecoration dec in decorations)
                    {
                        items.Add(dec);
                    }
                }
            }

            return items;
        }
	}
}
