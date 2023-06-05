using System;
using System.Drawing;
using System.Collections;
using ManagedCore;
using System.Xml;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for CMenuLayout.
	/// </summary>
	public class CMenuLayout
	{

		private RectangleF mNextButtonPosition;
		private RectangleF mPreviousButtonPosition;
		private CMenuTextSpecification mNextButtonVCDTextSpec;
		private CMenuTextSpecification mPreviousButtonVCDTextSpec;

		private CMenuTextSpecification mHeaderTextSpec;
		private ArrayList mSlideshowVCDNumberTextSpec;	// where the number are printer for vcd/svcd
		private ArrayList mSlideshowTextSpec;			// where the slideshow attached text goes
		private ArrayList mSlideshowButtonPosition;		// the the slideshow buttons go

		private string	mLayoutImage;

		//*******************************************************************
		public CMenuLayout()
		{
		}

		//*******************************************************************
		public CMenuLayout(
			RectangleF next_button_position,
			RectangleF previous_button_position,
			ArrayList slideshow_button_positions,
			ArrayList slideshow_text_specs,
			CMenuTextSpecification header_text_spec,
			string layout_image)
		{
			mNextButtonPosition= next_button_position;
			mPreviousButtonPosition = previous_button_position;
			mSlideshowTextSpec = slideshow_text_specs;
			mSlideshowButtonPosition = slideshow_button_positions;
			mHeaderTextSpec =header_text_spec;
			mLayoutImage = layout_image;
		}


		//*******************************************************************
		public string GetLayoutImage()
		{
			return mLayoutImage ;
		}

		//*******************************************************************
		public RectangleF  GetNextButtonPosition()
		{
			return mNextButtonPosition;
		}

		
		//*******************************************************************
		public CMenuTextSpecification  GetNextButtonVCDTextSpec()
		{
			return this.mNextButtonVCDTextSpec;
		}

		//*******************************************************************
		public CMenuTextSpecification  GetPreviousButtonVCDTextSpec()
		{
			return this.mPreviousButtonVCDTextSpec;
		}


		//*******************************************************************
		public RectangleF GetPreviousButtonPosition()
		{
			return mPreviousButtonPosition;
		}


		//*******************************************************************
		public CMenuTextSpecification GetHeaderTextSpec()
		{
			return mHeaderTextSpec;
		}


		//*******************************************************************
		public CMenuTextSpecification GetVCDNumberTextSpec(int index)
		{
			if (mSlideshowVCDNumberTextSpec ==null) return null;
			if (mSlideshowVCDNumberTextSpec.Count <=0) return null ;
			if (index > mSlideshowVCDNumberTextSpec.Count-1)
			{
				CDebugLog.GetInstance().Error("Wrong index on call to GetVCDNumberTextSpec, given index was "+index+" but max is "+mSlideshowVCDNumberTextSpec.Count);
				index =0;
			}

			return (CMenuTextSpecification) mSlideshowVCDNumberTextSpec[index];
		}

		//*******************************************************************
		public CMenuTextSpecification GetSlideshowTextSpec(int index)
		{
			if (mSlideshowTextSpec==null) return null;
			if (mSlideshowTextSpec.Count <=0) return null ;
			if (index > mSlideshowTextSpec.Count-1)
			{
				CDebugLog.GetInstance().Error("Wrong index on call to GetSlideshowTextSpec, given index was "+index+" but max is "+mSlideshowTextSpec.Count);
				index =0;
			}

			return (CMenuTextSpecification) mSlideshowTextSpec[index];
		}

		//*******************************************************************
		public RectangleF GetSlideshowButtonPosition(int index)
		{
			if (index > mSlideshowButtonPosition.Count-1)
			{
				CDebugLog.GetInstance().Error("Wrong index on call to GetSlideshowButtonPosition, given index was "+index+" but max is "+mSlideshowButtonPosition.Count);

				index =0;
			}

			return (RectangleF) mSlideshowButtonPosition[index];
		}

		//*******************************************************************
		public int GetNumButtonPositions()
		{
			return this.mSlideshowButtonPosition.Count;
		}

		//*******************************************************************
		public void Save(XmlElement parent, XmlDocument doc)
		{
			XmlElement MenuLayout = doc.CreateElement("MenuLayout");

			MenuLayout.SetAttribute("LayoutImage",this.mLayoutImage);
		
			CGlobals.SaveRectangle(MenuLayout, doc, "NextButtonPosition",mNextButtonPosition);
			CGlobals.SaveRectangle(MenuLayout, doc, "PreviousButtonPosition",mPreviousButtonPosition);


			XmlElement Header = doc.CreateElement("Header");
			mHeaderTextSpec.Save(Header,doc);
			MenuLayout.AppendChild(Header); 

			XmlElement buttons = doc.CreateElement("SlideShowButtons");

			for (int count=0;count<mSlideshowButtonPosition.Count;count++)
			{
				RectangleF pos = (RectangleF) mSlideshowButtonPosition[count];
				CGlobals.SaveRectangle(buttons,doc,"SlideshowButtonPosition",pos);

				CMenuTextSpecification spec =GetSlideshowTextSpec(count);
				if (spec!=null)
				{
					spec.Save(buttons, doc);
				}
			}

			MenuLayout.AppendChild(buttons);


			parent.AppendChild(MenuLayout); 
		}

		//*******************************************************************
		public void Load(XmlElement element)
		{
			mLayoutImage = element.GetAttribute("LayoutImage");
		
			// link buttons


			XmlNodeList Linkbuttons_list = element.GetElementsByTagName("LinkButtons");

			if (Linkbuttons_list.Count!=1)
			{
				CDebugLog.GetInstance().Error("Wrong number of link buttons sections in Menu, links found ("+Linkbuttons_list.Count+"), should be 1");
			}
			if (Linkbuttons_list.Count>0)
			{
				XmlElement e = Linkbuttons_list[0] as XmlElement;
				if (e!=null)
				{
					mNextButtonPosition = CGlobals.LoadRectangle(e,"NextButtonPosition");
					mPreviousButtonPosition = CGlobals.LoadRectangle(e,"PreviousButtonPosition");
				}

				
				XmlNodeList vcd_text_spec_list = e.GetElementsByTagName("VCDNumberTextSpec");

				mNextButtonVCDTextSpec=null;
				mPreviousButtonVCDTextSpec=null;

				if (vcd_text_spec_list.Count!=2)
				{
                    CDebugLog.GetInstance().Warning("Missing VCD numbers for link buttons :" + vcd_text_spec_list.Count);
			
				}
				else
				{
					XmlElement e1 = (XmlElement)vcd_text_spec_list[0];
					CMenuTextSpecification ts = new CMenuTextSpecification();
					ts.Load(e1);
					mNextButtonVCDTextSpec =ts;

					XmlElement e2 = (XmlElement)vcd_text_spec_list[1];
					CMenuTextSpecification ts2 = new CMenuTextSpecification();
					ts2.Load(e2);
					mPreviousButtonVCDTextSpec = ts2 ;
				}
			}


			// header
			XmlNodeList header_list = element.GetElementsByTagName("Header");
			if (header_list.Count!=1)
			{
				CDebugLog.GetInstance().Error("Wrong number of header elements in Menu layout ("+header_list.Count+")");
			}
			if (header_list.Count>0)
			{
				XmlElement e = header_list[0] as XmlElement;
				if (e!=null)
				{
					XmlNodeList header_text_spec_list = e.GetElementsByTagName("TextSpec");
					if (header_text_spec_list.Count!=1)
					{
						CDebugLog.GetInstance().Error("Wrong number of header text spec elements in Menu layout ("+header_text_spec_list.Count+")");
					}
					if (header_text_spec_list.Count>0)
					{
						XmlElement e1 = header_text_spec_list[0] as XmlElement;
						if (e1!=null)
						{
							this.mHeaderTextSpec = new CMenuTextSpecification();
							this.mHeaderTextSpec.Load(e1);
						}
					}
				}
			}


			// slideshow buttons
			XmlNodeList slideshow_list = element.GetElementsByTagName("SlideShowButtons");

			if (slideshow_list.Count!=1)
			{
				CDebugLog.GetInstance().Error("Wrong number of slideshow buttons sections in Menu layout ("+slideshow_list.Count+")");
			}
			if (slideshow_list.Count>0)
			{
				XmlElement e = slideshow_list[0] as XmlElement;
				if (e!=null)
				{
					XmlNodeList text_spec_list = e.GetElementsByTagName("TextSpec");

					this.mSlideshowTextSpec=null;

					if (text_spec_list.Count!=0)
					{
						this.mSlideshowTextSpec = new ArrayList();
					}

					foreach (XmlElement e1 in text_spec_list)
					{
						CMenuTextSpecification ts = new CMenuTextSpecification();
						ts.Load(e1);
						mSlideshowTextSpec.Add(ts);
					}

					XmlNodeList vcd_text_spec_list = e.GetElementsByTagName("VCDNumberTextSpec");

					this.mSlideshowVCDNumberTextSpec=null;

					if (vcd_text_spec_list.Count!=0)
					{
						this.mSlideshowVCDNumberTextSpec = new ArrayList();
					}

					foreach (XmlElement e1 in vcd_text_spec_list)
					{
						CMenuTextSpecification ts = new CMenuTextSpecification();
						ts.Load(e1);
						mSlideshowVCDNumberTextSpec.Add(ts);
					}


					XmlNodeList button_pos_list = e.GetElementsByTagName("SlideshowButtonPosition");

					this.mSlideshowButtonPosition = null;
					if (button_pos_list.Count!=0)
					{
						mSlideshowButtonPosition = new ArrayList() ;
					}

					foreach (XmlElement e2 in button_pos_list)
					{
						RectangleF bp = CGlobals.LoadRectangle(e2);
						
						CDebugLog.GetInstance().Trace("Loaded slideshow button for pos: "+bp.X+" "+bp.Y+" "+ bp.Width+" "+bp.Height);
						mSlideshowButtonPosition.Add(bp);
					}

				}
			}
		}

	}
}
