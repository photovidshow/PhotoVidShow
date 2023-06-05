using System;
using System.Drawing;
using System.Xml;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
	/// <summary>
    /// Summary description for CMenuLinkPreviousNextButton.
	/// </summary>
	public class CMenuPlayAllButton : CMenuButton, ITextDecorationContainer
	{
        private CTextDecoration mInnerTextDecoration = null;

        public CTextDecoration TextDecoration
        {
            get { return mInnerTextDecoration; }
        }
  
        //*******************************************************************
        public CMenuPlayAllButton() 
		{
		}

        //*******************************************************************
        public CMenuPlayAllButton(RectangleF coverage, CTextStyle style, string text)
            :
            base(null, coverage)
		{
            mInnerTextDecoration = new CTextDecoration(text, coverage, 1, style);
		}

        //*******************************************************************
        public override Rectangle RenderToGraphics(RectangleF r, int frame_num, CSlide originating_slide, RenderSurface inputSurface)
        {
            //
            // Simply don't render this buttons when in SVCD or VCD output
            //
            if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.SVCD ||
                CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.VCD )
            {
                return new Rectangle(0, 0, 1, 1);
            }

            return mInnerTextDecoration.RenderToGraphics(r, frame_num, originating_slide, inputSurface);
        }

        //*******************************************************************
        public override RectangleF CoverageArea
        {
            set 
            { 
                mCoverageArea = value;
                mInnerTextDecoration.CoverageArea = value;
            }
        }       

		//*******************************************************************
		public override void Save(XmlElement parent, XmlDocument doc)
		{
			XmlElement decoration = doc.CreateElement("Decoration");
          //  SaveMenuButtonPart(decoration, doc);

			decoration.SetAttribute("Type","PlayAllButton");

            mInnerTextDecoration.SaveTextDecrationPart(decoration, doc);

			parent.AppendChild(decoration); 
		}

		//*******************************************************************
		public override void Load(XmlElement element)
		{
          //  LoadMenuButtonPart(element);

            mInnerTextDecoration = new CTextDecoration();
            mInnerTextDecoration.LoadTextDecorationPart(element);

           // mInnerTextDecoration = new CTextDecoration("Play All", mCoverageArea,1, new CTextStyle());

            mCoverageArea = mInnerTextDecoration.CoverageArea;
		}
	}
}
