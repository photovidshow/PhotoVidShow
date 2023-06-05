using System;
using System.Drawing;
using System.Collections;
using ManagedCore;
using System.Xml;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for CMenuLayoutDatabase.
	/// </summary>
	public class CMenuLayoutDatabase
	{ 
		private static  CMenuLayoutDatabase mInstance;
	
		public static CMenuLayoutDatabase GetInstance()
		{
			if (mInstance==null)
			{
				mInstance = new CMenuLayoutDatabase();
			}
			return mInstance;
		}


		//*******************************************************************
		//*******************************************************************
		//*******************************************************************

		private ArrayList mEntries ;

		
		//*******************************************************************
		private CMenuLayoutDatabase()
		{
			this.Load(CGlobals.GetRootDirectory()+@"\MenuLayouts\layouts.xml");
			//BuildDatabase();
		}


		//*******************************************************************
		private void BuildDatabase()
		{
			mEntries = new ArrayList(3);

			{
				CTextStyle style1 = new CTextStyle();

				PointF header_start = new PointF(0.2f,0.1f);
				RectangleF area = new RectangleF(0.2f,0.1f,0.5f,0.2f);
				CMenuTextSpecification header_spec = new CMenuTextSpecification(style1, header_start, area);

				RectangleF next_pos = new RectangleF(0.8f,0.8f,0.1f,0.1f);
				RectangleF previous_pos = new RectangleF(0.1f,0.8f,0.1f,0.1f);

				ArrayList slideshow_pos_list = new ArrayList();
				ArrayList text_spec_list = new ArrayList();

				for (int nb =0; nb <3;nb++)
				{
					RectangleF but_pos = new RectangleF(0.5f,0.32f+(0.17f*nb),0.14f,0.14f);
					RectangleF text_area = new RectangleF(0.15f,0.34f+(0.17f*nb),0.3f, 0.1f);
					PointF test_point = new PointF(0.15f,0.34f+(0.17f*nb));

					CMenuTextSpecification text_spec = new CMenuTextSpecification(style1, test_point, text_area);

					slideshow_pos_list.Add(but_pos);
					text_spec_list.Add(text_spec);
				}

				CMenuLayout layout1 = new CMenuLayout(next_pos,
					previous_pos, 
					slideshow_pos_list,
					text_spec_list,
					header_spec,
					"Layout3-1.bmp");

				this.mEntries.Add(layout1);
			}

		{
			CTextStyle style1 = new CTextStyle();

			PointF header_start = new PointF(0.2f,0.1f);
			RectangleF area = new RectangleF(0.2f,0.1f,0.5f,0.2f);
			CMenuTextSpecification header_spec = new CMenuTextSpecification(style1, header_start, area);

			RectangleF next_pos = new RectangleF(0.8f,0.8f,0.1f,0.1f);
			RectangleF previous_pos = new RectangleF(0.1f,0.8f,0.1f,0.1f);

			ArrayList slideshow_pos_list = new ArrayList();
			ArrayList text_spec_list = new ArrayList();

			for (int nb =0; nb <2;nb++)
			{
				RectangleF but_pos = new RectangleF(0.5f,0.32f+(0.23f*nb),0.2f,0.2f);
				RectangleF text_area = new RectangleF(0.15f,0.34f+(0.23f*nb),0.3f, 0.1f);
				PointF test_point = new PointF(0.15f,0.34f+(0.23f*nb));

				CMenuTextSpecification text_spec = new CMenuTextSpecification(style1, test_point, text_area);

				slideshow_pos_list.Add(but_pos);
				text_spec_list.Add(text_spec);
			}

			CMenuLayout layout1 = new CMenuLayout(next_pos,
				previous_pos, 
				slideshow_pos_list,
				text_spec_list,
				header_spec,
				"Layout2-2.bmp");

			this.mEntries.Add(layout1);
		}

			{
				CTextStyle style1 = new CTextStyle();

				PointF header_start = new PointF(0.2f,0.1f);
				RectangleF area = new RectangleF(0.2f,0.1f,0.5f,0.2f);
				CMenuTextSpecification header_spec = new CMenuTextSpecification(style1, header_start, area);

				RectangleF next_pos = new RectangleF(0.8f,0.8f,0.1f,0.1f);
				RectangleF previous_pos = new RectangleF(0.1f,0.8f,0.1f,0.1f);

				ArrayList slideshow_pos_list = new ArrayList();
				ArrayList text_spec_list = new ArrayList();

				for (int nb =0; nb <2;nb++)
				{
					
					for (int nbi =0; nbi <2;nbi++)
					{
						RectangleF but_pos = new RectangleF(0.3f+(nbi*0.4f),0.32f+(0.23f*nb),0.2f,0.2f);
						slideshow_pos_list.Add(but_pos);
					}
				}

				CMenuLayout layout1 = new CMenuLayout(next_pos,
					previous_pos, 
					slideshow_pos_list,
					null,
					header_spec,
					"Layout4-2.bmp");

				this.mEntries.Add(layout1);
			}

			this.Save(CGlobals.GetRootDirectory()+@"\MenuLayouts\layouts.xml");
		 
		}
	
		//*******************************************************************
		public CMenuLayout	GetLayout(int index)
		{
			if (index > mEntries.Count-1)
			{
				CDebugLog.GetInstance().Error("Entry "+index+" not in menu layout database");
				index =0;
			}

			return (CMenuLayout) mEntries[index];
		}

		//*******************************************************************
		public int GetNumEntries()
		{
			return mEntries.Count;
		}


		//*******************************************************************
		public int	GetFirstLayoutThatSupportsNButtons(int amount)
		{	
			int count=0;
			foreach (CMenuLayout l in mEntries)
			{
				if (l.GetNumButtonPositions() == amount)
				{
					return count;
				}
				count++;
			}
			return -1;
		}

		//*******************************************************************
		public void Save(string filename)
		{
			System.Xml.XmlDocument my_doc = new XmlDocument();

			XmlElement LayoutDB = my_doc.CreateElement("LayoutDB");
			my_doc.AppendChild(LayoutDB); 

			foreach (CMenuLayout l in mEntries)
			{
				l.Save(LayoutDB,my_doc);
			}

			my_doc.Save(filename);
		}


		//*******************************************************************
		public void Load(string filename)
		{
			CDebugLog.GetInstance().Trace("Loading layouts xml");

			System.Xml.XmlDocument my_doc = new XmlDocument();
			CDebugLog.GetInstance().Trace("Loading layouts xml 2 ");

			try
			{
				my_doc.Load(filename);
			}
			catch(Exception e)
			{
				CDebugLog.GetInstance().Error("Exception occurred when trying to read '"+filename+"' :"+e.Message);
				return ;
			}

			CDebugLog.GetInstance().Trace("Loading layouts xml 3");

			XmlNodeList list =my_doc.GetElementsByTagName("MenuLayout");

			this.mEntries = null;
			if (list.Count!=0)
			{
				mEntries = new ArrayList();
			}
			foreach (XmlElement e in list)
			{
				CDebugLog.GetInstance().Trace("Loading layouts xml 4");

				CMenuLayout ml = new CMenuLayout();
				ml.Load(e);
				mEntries.Add(ml);
			}

			CDebugLog.GetInstance().Trace("Loading layouts xml 5");

		}


	}
}
