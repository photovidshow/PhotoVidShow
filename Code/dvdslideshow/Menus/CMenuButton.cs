using System;
using System.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml;
using ManagedCore;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for CMenuButton.
	/// </summary>
	public abstract class CMenuButton : CDecoration
	{
		protected CImage		mFrame;
		private ArrayList	mAttachedChildDecorations;

		public ArrayList AttachedChildDecorations
		{
			get { return mAttachedChildDecorations ; }
		}

		public CImage Frame
		{
			get { return mFrame; }
			set { mFrame = value; }
		}

		public CMenuButton()
		{
		}

		//*******************************************************************
		public override CDecoration Clone()
		{
			ManagedCore.CDebugLog.GetInstance().Error("Menu item decorations do not support clone method");
			return null;
		}


		//*******************************************************************
		public CMenuButton(CImage frame, RectangleF coverage) :
			base(coverage, 1)
		{
			mFrame = frame;
		}

		
		//*******************************************************************
        public override Rectangle RenderToGraphics(RectangleF r, int frame_num, CSlide originating_slide, RenderSurface inputSurface)
		{
			// render frame
			if (mFrame != null)
			{
				float x = r.Width*this.mCoverageArea.X+ r.X;
				float y = r.Height*this.mCoverageArea.Y + r.Y;
				float width = r.Width*this.mCoverageArea.Width ;
				float height = r.Height*this.mCoverageArea.Height ;

				Rectangle r1 = new Rectangle((int)x,(int)y,(int)width,(int)height);

                GraphicsEngine.Current.DrawImage(mFrame, new RectangleF(0, 0, 1, 1), r1, true);
		
                return r1;
			}

            return new Rectangle(0,0,1,1);
		}

		//*******************************************************************
		public void SaveMenuButtonPart(XmlElement decoration, XmlDocument doc)
		{
			SaveDecrationPart(decoration,doc);

	//		if (mFrame!=null)
	//		{
	//			mFrame.Save(decoration,doc);
	//		}

			if (this.mAttachedChildDecorations != null)
			{
				foreach (int d in mAttachedChildDecorations)
				{
					XmlElement child = doc.CreateElement("ChildAttachmentID");

					child.SetAttribute("id",d.ToString());
					decoration.AppendChild(child);
				}
			}
		}

		//*******************************************************************
		public void LoadMenuButtonPart(XmlElement element)
		{
			LoadDecrationPart(element);

		//	XmlNodeList list =element.GetElementsByTagName("Image");
//
//			if (list.Count >0)
//			{
//				mFrame = new CImage();
//				mFrame.Load(list[0] as XmlElement,false);
//			}

			XmlNodeList child_list = element.GetElementsByTagName("ChildAttachmentID");

			if (child_list!=null)
			{
				mAttachedChildDecorations = new ArrayList();
			
				foreach (XmlElement child in child_list)
				{
					int d = int.Parse(child.GetAttribute("id"));
					mAttachedChildDecorations.Add(d);
				}
			}
		}


		//*******************************************************************
		public CMainMenu GetMenuWeAreIn()
		{
			CMainMenu top_menu = CGlobals.mCurrentProject.MainMenu;

			ArrayList menus = top_menu.GetSelfAndAllSubMenus();

			foreach (CMainMenu mm in menus)
			{
				CImageSlide ims = mm.BackgroundSlide;
				if (ims!=null)
				{
					ArrayList decorations = ims.Decorations;
					foreach (CDecoration dec in decorations)
					{
						if (dec == this)
						{
							return mm;
						}
					}
				}
			}
			CDebugLog.GetInstance().Error("Could not find menu MenuButton is in");
			return CGlobals.mCurrentProject.MainMenu;
		}


		//*******************************************************************
		public void AddChildDecoration(CDecoration child)
		{
			if (this.mAttachedChildDecorations == null)
			{
				mAttachedChildDecorations = new ArrayList();
			}

            child.AssignID();
			mAttachedChildDecorations.Add(child.ID);
		}

       //*******************************************************************
        public CTextDecoration GetNonVCDAttachedTextDecoration(CMainMenu m)
        {
            ArrayList attached_decors = this.AttachedChildDecorations;
            if (attached_decors != null)
            {
                foreach (int dd in attached_decors)
                {
                    CTextDecoration td = m.BackgroundSlide.GetDecorationFromID(dd) as CTextDecoration;
                    if (td != null)
                    {
                        if (td.VCDNumber == false)
                        {
                            return td;
                        }
                    }
                }
            }
            return null;
        }
	}
}
