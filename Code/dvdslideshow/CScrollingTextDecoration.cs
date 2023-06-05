using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Globalization;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for CTextDecoration.
	/// </summary>
    public class CScrollingTextDecoration : CTextDecoration
	{
        private RectangleF mClipRegion;

        // scroll speed in terms of canvas fraction per second
        private float mScrollSpeed;

		public RectangleF ClipRegion
		{
			get { return mClipRegion; }
		}

		public float ScrollSpeed
		{
			get { return mScrollSpeed; }
		}

		//*******************************************************************
		public CScrollingTextDecoration()
		{
		}

		//*******************************************************************
        public CScrollingTextDecoration(CScrollingTextDecoration copy)
            : base(copy)
		{
            mScrollSpeed = copy.mScrollSpeed;
            mClipRegion = copy.mClipRegion;
		}


		//*******************************************************************
		public override CDecoration Clone()
		{
            return new CScrollingTextDecoration(this);
		}

		//*******************************************************************
		public CScrollingTextDecoration(string text,
                                        RectangleF coverage,
                                        int order,
                                        float font_size,
                                        RectangleF clip_region,
                                        float scroll_speed)
            : base(text, coverage, order, font_size)
		{
			mScrollSpeed = scroll_speed;
			mClipRegion = clip_region;
		}

		//*******************************************************************
		public CScrollingTextDecoration(string text,
                                        RectangleF coverage, 
                                        int order, 
                                        CTextStyle style,
                                        RectangleF clip_region,
                                        float scroll_speed)
              : base(text, coverage, order, style)
		{
            mScrollSpeed = scroll_speed;
            mClipRegion = clip_region;
		}



        //*******************************************************************
        public override Rectangle RenderToGraphics(RectangleF r, int frame_num, CSlide originating_slide, RenderSurface input_image)
        {
            Rectangle drawn_region = new Rectangle(0, 0, 1, 1);

            if (InAnimatedTimeWindow(frame_num, originating_slide) == false)
            {
                return drawn_region;
            }

            CheckIfNeedToGenerateCoverageAreaFromFontSize();

            // re calc y offset for coverage area
            int frames_in = frame_num - originating_slide.mStartFrameOffset;
            if (frames_in < 0) return drawn_region;

            float fps = (float)CGlobals.mCurrentProject.DiskPreferences.frames_per_second;
            float seconds_from_start_of_slide = ((float)frames_in) / fps;
            seconds_from_start_of_slide -= this.StartOffsetTimeRawValue;

            float dist_scrolled =  mScrollSpeed * seconds_from_start_of_slide;      // this is in 0...1 coordinate system

            float add_y =0;
            float src_y_chopped_from_top = 0;
            float cut_short = 0;

            if (dist_scrolled < mClipRegion.Height)
            {
                add_y = mClipRegion.Height - dist_scrolled;
            }
            else
            {
                src_y_chopped_from_top = dist_scrolled - mClipRegion.Height;
            }

            // check if we need to clip the bottom
            if (src_y_chopped_from_top + mClipRegion.Height > mCoverageArea.Height)
            {
                cut_short = (src_y_chopped_from_top + mClipRegion.Height) - mCoverageArea.Height;
                if (cut_short >= mClipRegion.Height)
                {
                    // completely scrolled off now, just return
                    return drawn_region;
                }
            }

            float src_x = 0;
            float src_y = src_y_chopped_from_top * r.Height ;
            float src_width = r.Width * mCoverageArea.Width;
            float src_height = (mClipRegion.Height - add_y - cut_short) * r.Height;

            float x = mCoverageArea.X;
            float y = mClipRegion.Y + add_y;
            float width = mCoverageArea.Width;
            float height = mClipRegion.Height - add_y - cut_short;

            RectangleF srcRec = new RectangleF(src_x, src_y, src_width + src_x, src_height + src_y);
            RectangleF destRec = new RectangleF(x, y , width, height);

            drawn_region = base.RenderToGraphics(r, frame_num, originating_slide, -1, srcRec, destRec, input_image);

            return drawn_region;
		}

		
		//*******************************************************************
		public override void Save(XmlElement parent, XmlDocument doc)
		{
			XmlElement decoration = doc.CreateElement("Decoration");

			decoration.SetAttribute("Type","ScrollingTextDecoration");

			SaveTextDecrationPart(decoration,doc);

          		CGlobals.SaveRectangle(decoration, doc, "ClipArea", this.mClipRegion);
            	decoration.SetAttribute("ScrollSpeed", this.mScrollSpeed.ToString());
		
			parent.AppendChild(decoration); 
		}

		
		//*******************************************************************
        public override void Load(XmlElement element)
        {
            mClipRegion = CGlobals.LoadRectangle(element, "ClipArea");
            string scrolly_speed = element.GetAttribute("ScrollSpeed");
            if (scrolly_speed != "")
            {
                mScrollSpeed = float.Parse(scrolly_speed);
            }

            LoadTextDecorationPart(element);
        }
	}
}
