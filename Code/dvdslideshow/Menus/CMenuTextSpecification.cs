using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml;
using ManagedCore;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for CMenuTextSpecification.
	/// </summary>
	public class CMenuTextSpecification
	{
		private CTextStyle	mStyle;
		private PointF		mStartPosition;
		private RectangleF	mTextArea;
	
		//*******************************************************************
		public CTextStyle Style
		{
			get { return mStyle;}
		}


		//*******************************************************************
		public PointF StartPosition
		{
			get { return mStartPosition;}
		}


		//*******************************************************************
		public RectangleF TextArea
		{
			get { return mTextArea;}
		}


		//*******************************************************************
		public CMenuTextSpecification()
		{
		}

		//*******************************************************************
		public CMenuTextSpecification(CTextStyle style, PointF start_position, RectangleF text_area)
		{
			mStyle = style; 
			mStartPosition =start_position ;
			mTextArea = text_area;
		}

        //*******************************************************************
        // We have to pass in a font size value, as the current decoration coverage area
        // is probably not valid NOR is the current style font size, is this MAY represent 
        // the font size needed for last render canvis size, NOT the canvis size we calc
        // font size with.
		public RectangleF ReCalcMenuTextCoverageArea(CTextDecoration dec, float size)
		{
			float x_offset=0;

            dec.RecalcCoverageAreaForFontSize(size);
			
			if (this.mStyle.Format.Alignment == System.Drawing.StringAlignment.Center)
			{
				x_offset+= dec.CoverageArea.Width/2;
			}
			if (this.mStyle.Format.Alignment == System.Drawing.StringAlignment.Far)
			{ 
				x_offset+= dec.CoverageArea.Width;
			}

			RectangleF r = new RectangleF(this.mStartPosition.X - x_offset, this.mStartPosition.Y,dec.CoverageArea.Width,dec.CoverageArea.Height);
			return r;

		}
		
		//*******************************************************************
		public void Save(XmlElement parent, XmlDocument doc)
		{
			XmlElement TextSpec = doc.CreateElement("TextSpec");

			CGlobals.SavePointF(TextSpec,doc,"StartPosition",mStartPosition);

			mStyle.Save(TextSpec,doc);

			parent.AppendChild(TextSpec);
	
		}

		//*******************************************************************
		public void Load(XmlElement element)
		{
			this.mStartPosition = CGlobals.LoadPointF(element,"StartPosition");
			XmlNodeList list = element.GetElementsByTagName("TextStyle");
			if (list.Count!=1)
			{
				CDebugLog.GetInstance().Error("Wrong number of styles in text spec ("+list.Count+")");
			}

			if (list.Count<1) return ;

			XmlElement e = list[0] as XmlElement;
			if (e!=null)
			{
				this.mStyle = new CTextStyle();
				this.mStyle.Load(e);
			}
		}

	}
}
