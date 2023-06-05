using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Globalization;
using System.Collections.Generic;
using System.Collections;
using DVDSlideshow.GraphicsEng;
using ManagedCore;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for CTextDecoration.
	/// </summary>
    public class CTextDecoration : CImageDecoration, IImageCreator, ITextDecorationContainer
	{
        // wrap rectangleF into object
        private class CharacterPosition
        {
            public CharacterPosition(RectangleF position)
            {
                mPosition = position;
            }

            private RectangleF mPosition;

            public RectangleF Position
            {
                get { return mPosition;}
                set { mPosition = value; }
            }
        }

        public CTextDecoration TextDecoration
        {
            get { return this; }
        }

        private static GDITextDrawer mTextDrawer = new GDITextDrawer();
		public string mText;
        private bool mBackplane = false;
		private float mBackplaneTransparency = 128;
        private Color mBackplaneColor;
		private bool mHeader = false;
		private bool mVCDNumber = false;
        private CImage mTextDecorationImage = null;
        private CTextStyle mStyle;
        private string mOriginalTemplateText = "";          // used by PhotoCruz, not saved out
      
        // Used when character delay enabled
        private ArrayList mIndividualCharactersCoverage = null;
        private ArrayList mIndividualCharactersCoverageUV = null;

        // Until another way works better
        private const float mIncreaseImageWidthMutlipierForIndividualCharacters = 2.1f;
        private const float mIndividualCharactersWidthGapMultiplier = 2.0f;

        private const float mIncreaseImageHeightMutlipierForIndividualCharacters = 1.1f;
     
        private static Bitmap mMeasureStringBitmap = new Bitmap(10, 10, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        private bool mSetTextHint = false;  // Used by templates to hint this is the decor we should preview when editting this text in the
                                            // edit slide media form  (i.e. same text may exists in multi slides)

        private float mHintedEditTime = -1; // Similar to above, but in this case we specift an exact time that best suits to edit this decor
        private int mBlurAmount = 0;

        public string OriginalTemplateText
        {
            get { return mOriginalTemplateText; }
            set { mOriginalTemplateText = value; }
        }

        public CTextStyle TextStyle
        {
            get { return mStyle; }
            set 
            { 
                mStyle = value;
                this.InvalidateFont();
            }
        }


        public bool SetTextHint
        {
            get { return mSetTextHint; }
        }

        public float HintedEditTime
        {
            get { return mHintedEditTime; }
        }

        public enum TemplateEditableType
        {
            None,
            SingleLine,
            MultipleLine
        };

        // If true, this is editbale from the template media input screen
        private TemplateEditableType mTemplateEditable = TemplateEditableType.None;

        public TemplateEditableType TemplateEditable
        {
            get { return mTemplateEditable; }
            set { mTemplateEditable = value; }
        }


		public bool Header
		{
			get
			{
				return mHeader;
			}
			set
			{
				mHeader=value;
			}
		}

		public bool VCDNumber
		{
			get
			{
				return mVCDNumber;
			}
			set
			{
				mVCDNumber=value;
			}
		}

		public bool Editable
		{
			get 
			{ 
				return mVCDNumber==false;
			}
		}

		public float BackPlaneTransparency
		{
			get
			{
				return mBackplaneTransparency;
			}
		}

	
		public Color BackColor
		{
			get
			{
				Color c = Color.FromArgb(mBackplaneColor.R, mBackplaneColor.G,mBackplaneColor.B);
				return c;
			}
		}

        public bool Background
        {
            get { return mBackplane; }
         //   set { mBackground = value; }
        }


		//*******************************************************************
		public CTextDecoration()
		{
			mStyle =new CTextStyle();
		}

		//*******************************************************************
		public CTextDecoration(CTextDecoration copy) : base(copy)
		{
			mText =copy.mText;
			mBackplane = copy.mBackplane;
			mBackplaneTransparency= copy.mBackplaneTransparency;
		    mBackplaneColor = copy.mBackplaneColor;
			mHeader = copy.mHeader;
			mVCDNumber = copy.mVCDNumber;
			mStyle = copy.mStyle.Clone();
		}


        //*******************************************************************
        public RectangleF GetCoverageAreaIncludingShadow()
        {
            // shadow ? add a bit more
            if (mStyle.Shadow == true)
            {
                RectangleF rf = new RectangleF();
                rf.X = mCoverageArea.X;
                rf.Y = mCoverageArea.Y;
                rf.Width = mCoverageArea.Width;
                rf.Height = mCoverageArea.Height;
                rf.Width += mCoverageArea.Width * 0.05f;
                rf.Height += mCoverageArea.Height * 0.05f;
                return rf;

            }
          
            return this.mCoverageArea;
        }

		//*******************************************************************
		public override CDecoration Clone()
		{
			return new CTextDecoration(this);
		}

        //*******************************************************************
        public void InvalidateFont()
        {
            mTextDecorationImage = null;
        }

		//*******************************************************************
		public void ResetLookToDefault()
		{
			mStyle =new CTextStyle();
			base.mAttachedToSlideImage=false;
		    mBackplane =false;
		    mBackplaneTransparency =128;
			mBackplaneColor =   Color.FromArgb ( (int) mBackplaneTransparency, 0, 0, 0 ) ;
		}


		//*******************************************************************
		public CTextDecoration(string text, RectangleF coverage, int order, float font_size) : base(coverage, order)
		{
			mText = text;
			Transparency = 0;
			mStyle =new CTextStyle();
			mAttachedToSlideImage = false;
			mBackplane = false;
			mBackplaneTransparency = 128;
			mStyle.FontSize = font_size;

			mBackplaneColor =   Color.FromArgb ( (int) mBackplaneTransparency, 0, 0, 0 ) ;	
			Init();
		}

		//*******************************************************************
		public CTextDecoration(string text, RectangleF coverage, int order, CTextStyle style) : base(coverage, order)
		{
			mStyle = style;
			mText = text;
			Transparency = 0.0f;
	
			mAttachedToSlideImage = false;
			mBackplane = false;
			mBackplaneTransparency = 128;
			SetBackPlaneColor(Color.Black);
			Init();
		}


		//*******************************************************************
		public void Init()
		{
		}

        //*******************************************************************
        public RectangleF GetCoverageAreaForFontSize(RectangleF r, string text, float font_size)
        {
            using (Graphics gp = Graphics.FromImage(mMeasureStringBitmap))
            {
                float max_width = GetMaxTextWidth();

                mStyle.FontSize = font_size;
                //	Console.WriteLine(mCoverageArea.X+" "+mCoverageArea.Y+" "+mCoverageArea.Width+" "+mCoverageArea.Height);
                RectangleF coverage_area = new RectangleF(mCoverageArea.X, mCoverageArea.Y, mCoverageArea.Width, mCoverageArea.Height);
                SizeF fonte = mTextDrawer.GetApproxFontExtents(text, gp, mStyle, max_width);

                float bw = this.mCoverageArea.Width;

                coverage_area.Width = fonte.Width / r.Width;
                coverage_area.Height = fonte.Height / r.Height;


                if (this.mStyle.Format.Alignment == System.Drawing.StringAlignment.Center)
                {
                    coverage_area.X -= (coverage_area.Width - bw) / 2;
                }

                if (this.mStyle.Format.Alignment == System.Drawing.StringAlignment.Far)
                {
                    coverage_area.X -= (coverage_area.Width - bw);
                }

                return coverage_area;     
            }  
        }

        //*******************************************************************
        public RectangleF GetCoverageAreaForFontSize(string text, float font_size)
        {
            RectangleF r = GetMeasureStringRegion();
            return GetCoverageAreaForFontSize(r, text, font_size);
        }
		
		//*******************************************************************
		public RectangleF GetCoverageAreaForFontSize(float font_size)
		{
			return GetCoverageAreaForFontSize(mText,font_size);
		}

		//*******************************************************************
		public void RecalcCoverageAreaForFontSize(float font_size)
		{
			mCoverageArea = GetCoverageAreaForFontSize(font_size);
            mTextDecorationImage = null;
		}

        //*******************************************************************
        public static float GetMaxTextWidth()
        {
            return 4096;
        }

        //*******************************************************************
        private RectangleF GetMeasureStringRegion()
        {
            // Used on some templates and photocruz work.  Here we give a font size rather than coverage area
            float ratio = CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction();
            RectangleF region = new RectangleF(0, 0, 432.0f / ratio, 432);    // size is not important but aspect is

            return region;
        }

		//*******************************************************************
		public float GetFontSizeForCoverageArea()
		{
            float maxWidth = GetMaxTextWidth();

            if (mCoverageArea.Width == 0)
            {
                if (mStyle.FontSize != 0)
                {
                    return mStyle.FontSize;
                }
                return 26;
            }

            using (Graphics gp = Graphics.FromImage(mMeasureStringBitmap))
            {
                RectangleF r = GetMeasureStringRegion();

                return GetFontSizeForCoverageArea(gp, r, maxWidth, mCoverageArea);
            }
		}


        //*******************************************************************
        public float GetFontSizeForCoverageArea(RectangleF region)
        {
            float maxWidth = GetMaxTextWidth();

            using (Graphics gp = Graphics.FromImage(mMeasureStringBitmap))
            {
                return GetFontSizeForCoverageArea(gp, region, maxWidth, mCoverageArea);
            }
        }
        
        //*******************************************************************
        private float GetFontSizeForCoverageArea(Graphics gp, RectangleF r, float max_width, RectangleF coverage)
        {
            float width = r.Width * coverage.Width;
            SizeF fonte = mTextDrawer.GetApproxFontExtents(mText, gp, this.mStyle, max_width);	// any size does not matter

            if (fonte.Width == 0)
            {
                SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromDecoration(this);
                Log.Warning("Font size 0 in call to GetFontSizeForCoverageArea in slideshow '" + message.Slideshow + "' slide:" + message.SlideNumber);
                return 1;
            }

            float the_size = mStyle.FontSize * (width / fonte.Width);

            return the_size;
        }

        //*******************************************************************
        private int GetNumberOfLinesInMultiLineText(string text)
        {
            int returnCount = 1;
            int characterLines = 1;

            for (int indx = 0; indx < text.Length; indx++)
            {
                if (mText[indx] == '\r')
                {
                    returnCount++;  // return becomes valid once we find the next printable character
                }
                else
                {
                    if (char.IsControl(mText[indx]) == false)
                    {
                        characterLines = returnCount;
                    }
                }

            }
            return characterLines;
        }

        //*******************************************************************
        private void CalcCoverageAreaForIndividualCharacters(Graphics gp, float width, float height)
        {
            float xOffset = 0;
            float xUVoffset = 0;
            float yOffset =0;
            float yUVoffset = 0;
            float nextCharWidth =0 ;
            bool multiLine = false;

            Font font = mTextDrawer.GenerateFont(mStyle);

            SizeF space_size = gp.MeasureString(" ", font, Point.Empty, StringFormat.GenericDefault);

   
            float line_height_uv = space_size.Height;
            float space_width = space_size.Width;

            this.mIndividualCharactersCoverage = new ArrayList(mText.Length);
            this.mIndividualCharactersCoverageUV = new ArrayList(mText.Length);

            int numberLines = GetNumberOfLinesInMultiLineText(mText);

            float lineCoverageHeight = (mCoverageArea.Height / numberLines) * height;

            for (int indx = 0; indx < mText.Length; indx++)
            {
                xOffset += nextCharWidth;
                xUVoffset += nextCharWidth *mIndividualCharactersWidthGapMultiplier;

                if (mText[indx] == '\r')
                {
                    xOffset = 0;
                    xUVoffset = 0;
                    nextCharWidth = 0;
                    yOffset += lineCoverageHeight;
                    yUVoffset += line_height_uv;

                    multiLine = true;
                    mIndividualCharactersCoverage.Add(new CharacterPosition( new RectangleF(0,0,0,0)));
                    mIndividualCharactersCoverageUV.Add(new RectangleF(0, 0, 0, 0));
                    continue;
                }

                if (char.IsControl( mText[indx] )  )
                {
                    nextCharWidth = 0;
                    mIndividualCharactersCoverage.Add(new CharacterPosition( new RectangleF(0,0,0,0)));
                    mIndividualCharactersCoverageUV.Add(new RectangleF(0, 0, 0, 0));
                    continue;
                }  
                else if (mText[indx] != ' ')
                {
                    nextCharWidth = gp.MeasureString(mText[indx].ToString(), font, Point.Empty, StringFormat.GenericTypographic).Width;
                }
                else
                {
                    nextCharWidth = space_width;
                }

                if (nextCharWidth <= 0)
                {
                    nextCharWidth = 1;
                }

                float f_offset = xOffset / width;
                float f_nextLength = ((nextCharWidth * mIndividualCharactersWidthGapMultiplier) / width);
                float f_h_offset = yOffset / height;
                float f_line_height = lineCoverageHeight / height;

                mIndividualCharactersCoverageUV.Add(new RectangleF(xUVoffset, yUVoffset, nextCharWidth * mIndividualCharactersWidthGapMultiplier, line_height_uv));  
                mIndividualCharactersCoverage.Add( new CharacterPosition(new RectangleF(mCoverageArea.X + f_offset, mCoverageArea.Y + f_h_offset, f_nextLength, f_line_height) ));  
            }

            if (mStyle.Format.Alignment != StringAlignment.Near && multiLine == true)
            {
                AlterIndividualCharacterCoverageAreaToCaterForJustification();
            }
        }


        //*******************************************************************
        private void AlterIndividualCharacterCoverageAreaToCaterForJustification()
        {
            List<List<CharacterPosition>> mAreas = new List<List<CharacterPosition>>();

            // ok create a list of lines ( each list/group represents characters at same y position)
            foreach (CharacterPosition box in mIndividualCharactersCoverage)
            {
                if (box.Position.Height == 0) continue;

                bool found = false;
                foreach (List<CharacterPosition> group in mAreas)
                {
                    foreach (CharacterPosition groupbox in group)
                    {
                        if (groupbox.Position.Y == box.Position.Y)
                        {
                            found = true;
                            group.Add(box);
                            break;
                        }
                    }
                    if (found == true)
                    {
                        break;
                    }
                }

                if (found == false)
                {
                    List<CharacterPosition> newgroup = new List<CharacterPosition>();
                    newgroup.Add(box);
                    mAreas.Add(newgroup);
                }
            }

            if (mAreas.Count <= 1) return;

            // Go through each line/group and adjust justification to fit inside mCoverageArea
            // , near justification need do nothing
            foreach (List<CharacterPosition> group in mAreas)
            {
                float minLeft = 1;
                float maxRight = 0;

                foreach (CharacterPosition groupbox in group)
                {
                    float left = groupbox.Position.X;
                    float right = groupbox.Position.X + (groupbox.Position.Width / mIndividualCharactersWidthGapMultiplier);
                    if (left < minLeft) minLeft = left;
                    if (right > maxRight) maxRight = right;
                }

                // Did we find atleast one character?
                if (maxRight > 0)
                {
                    // center justification
                    if (mStyle.Format.Alignment == StringAlignment.Center)
                    {
                        float centreGroup = (maxRight - minLeft) / 2;
                        float centreCoverage = mCoverageArea.Width / 2;
                        float different = centreCoverage - centreGroup;

                        foreach (CharacterPosition groupbox in group)
                        {
                            RectangleF pos = groupbox.Position;
                            pos.X += different;
                            groupbox.Position = pos;
                        }
                    }

                    // right/far justification
                    if (mStyle.Format.Alignment == StringAlignment.Far)
                    {
                        float groupWidth = maxRight - minLeft;
                        float different = mCoverageArea.Width - groupWidth;

                        foreach (CharacterPosition groupbox in group)
                        {
                            RectangleF pos = groupbox.Position;
                            pos.X += different;
                            groupbox.Position = pos;
                        }
                    }
                }
            }
        }

        //*******************************************************************
        public override Rectangle RenderToGraphics(RectangleF r, int frame_num, CSlide originating_slide, RenderSurface input_image)
        {
            return RenderToGraphics(r, frame_num, originating_slide, -1, null, null, input_image);
        }

        //*******************************************************************
        // Implement the IImageCreator interface
        public Bitmap CreateImage(int width, int height, ref bool createMipMaps)
        {
            createMipMaps = true;
            Bitmap png = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics gg = Graphics.FromImage(png))
            {
                Color backColour = Color.FromArgb(0, 0, 0, 0);

                // ### SRG Basically individial chars with back color not working well
                if (mIndividualCharactersCoverage == null && this.mBackplane == true)
                {
                    int trans = (int)(this.mBackplaneTransparency);
                    trans = CGlobals.Clamp(trans, 0, 255);
                    backColour = Color.FromArgb(trans, this.mBackplaneColor.R, this.mBackplaneColor.G, this.mBackplaneColor.B);
                }

                gg.Clear(backColour);

                if (mIndividualCharactersCoverage != null)
                {
                    int index = 0;
                    // Render individual characers into bitmap
                    foreach (RectangleF charRec in mIndividualCharactersCoverageUV)
                    {
                        if (charRec.Width == 0)
                        {
                            index++;
                            continue;
                        }

                        // ok when drawing individual chars to image ( we ignore justification),  justification
                        // is calculated later when individual characters coverage areas are calculated
                        CTextStyle ts = mStyle.Clone();
                        ts.Format.Alignment = StringAlignment.Near;

                        mTextDrawer.DrawString(gg, mText[index++].ToString(), charRec.X, charRec.Y, ts);
                    }
                }
                else
                {
                    // Render whole text into bitmap
              
                    mTextDrawer.DrawString(gg, mText, 0, 0, mStyle);
                }
            }

            if (mBlurAmount != 0)
            {
                GaussianBlur gb = new GaussianBlur();
                CImage input = new CImage(png);

                float ratio = ((float)CGlobals.mCurrentProject.DiskPreferences.CanvasWidth) / 656.0f;

                float ff = ((float)mBlurAmount) * ratio;
                int amount = (int)(ff + 0.49999);

                gb.Process(input, amount);
            }

           // png.Save("c:\\output.bmp");
            return png;
        }

 
        //*******************************************************************
        protected void CheckIfNeedToGenerateCoverageAreaFromFontSize()
        {
            // HACK stupid getfontextens stuff needs graphics to calc size????
            // ok if we have not defined width or height, we just dont know the coverage
            // size calculate it now.

            if (this.mCoverageArea.Width == 0 || this.mCoverageArea.Height == 0)
            {
                RectangleF region = GetMeasureStringRegion();

                mCoverageArea = GetCoverageAreaForFontSize(region, this.mText, this.mStyle.FontSize);
                mTextDecorationImage = null;
            }

        }

		//*******************************************************************
        public Rectangle RenderToGraphics(RectangleF r, int frame_num, CSlide originating_slide, int drawOnlyCharacterIndex, Nullable<RectangleF> srcRec, Nullable<RectangleF> destRec, RenderSurface input_image)
		{
            Rectangle drawn_region = new Rectangle(0,0,1,1);

            if (this.mText == "") return drawn_region;

            if (InAnimatedTimeWindow(frame_num, originating_slide) == false)
            {
                return drawn_region;
            }
   
            try
            {
                CheckIfNeedToGenerateCoverageAreaFromFontSize();

                // dont render if vcd number set but not a vcd output
                if (this.mVCDNumber == true &&
                    (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.DVD ||
                     CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.BLURAY ))
                {
                    return drawn_region; 
                }
  
                RectangleF coverage = mCoverageArea;

                if (destRec.HasValue)
                {
                    coverage = destRec.Value;
                }

                float completeTextWidth=1;
                float completeTextHeight=1;

                float textSurfaceFontCalcWidth = r.Width;
                float textSurfaceFontCalcHeight = r.Height;
                float textSurfaceAspect = r.Width / r.Height;

                // This ensures all text fonts within this slide are generated from same (max) surface size (unless a scrolly one)
                if (this is CScrollingTextDecoration == false && drawOnlyCharacterIndex == -1)
                {
                    float newWidth = textSurfaceFontCalcWidth; 
                    if (textSurfaceFontCalcWidth < 1024)
                    {
                        newWidth = 1024;
                    }

                    // ### SRG TODO Correct this (problem was in xmas tempalte) G535
                    if (OriginalTemplateImageNumber != 0 && CoverageArea.Width < 0.5)
                    {
                        newWidth = CGroupedDecoration.GetMaxGroupDecorationWidth();
                    }

                    textSurfaceFontCalcWidth = newWidth;
                    textSurfaceFontCalcHeight = textSurfaceFontCalcWidth / textSurfaceAspect;
                }

                float src_x = 0;
                float src_y = 0;
                float src_width = textSurfaceFontCalcWidth * coverage.Width;
                float src_height = textSurfaceFontCalcHeight * coverage.Height;

                if (drawOnlyCharacterIndex != -1)
                {
                    completeTextWidth = ((textSurfaceFontCalcWidth * mCoverageArea.Width * mIncreaseImageWidthMutlipierForIndividualCharacters)) +1;
                    completeTextHeight = ((textSurfaceFontCalcHeight * mCoverageArea.Height * mIncreaseImageHeightMutlipierForIndividualCharacters)) +1;
                }
                else
                {
                    completeTextWidth = ((textSurfaceFontCalcWidth * mCoverageArea.Width)) +1;
                    completeTextHeight = ((textSurfaceFontCalcHeight * mCoverageArea.Height)) +1 ;
                }

                int iCompleteTextWidth = (int)(completeTextWidth + 0.49999f);
                int iCompleteTextHeight = (int)(completeTextHeight + 0.49999f);

                // Only draw part of the text (e.g. when scrolling)
                if (srcRec.HasValue)
                {
                    src_x = srcRec.Value.X;
                    src_y = srcRec.Value.Y;
                    src_width = srcRec.Value.Width;
                    src_height = srcRec.Value.Height;
                }

                if (src_width <= 0) src_width = 1;
                if (src_height <= 0) src_height = 1;

                // Is image too big?
                if (iCompleteTextHeight >= 4096 || iCompleteTextWidth >= 4096)
                {
                    if (iCompleteTextWidth > iCompleteTextHeight)
                    {
                        iCompleteTextWidth = 4096;
                        iCompleteTextHeight = (int)(((float)iCompleteTextWidth) / textSurfaceAspect);
                    }
                    else
                    {
                        iCompleteTextHeight = 4096;
                        iCompleteTextWidth = (int)(((float)iCompleteTextHeight) * textSurfaceAspect);
                    }
                }

                // Test if we have to invalidate image
                if (mTextDecorationImage != null)
                {
                    RectangleF rr = mTextDecorationImage.GetRectangle();
                    int currentWidth = (int)rr.Width;
                    int currentHeight = (int)rr.Height;

                    bool needsTextDelay = drawOnlyCharacterIndex != -1;
                    bool hasTextDelay = mIndividualCharactersCoverage != null;

                    // ok if we different size or our text delay requirement has changed
                    if ( currentWidth != iCompleteTextWidth ||
                         Math.Abs(currentHeight - iCompleteTextHeight) > 10 ||      //  aspect rounding errors means this varies a bit
                         needsTextDelay != hasTextDelay)
                    {
                        //    Log.Info("Text bitmap dimension changed, old:" + currentWidth + "," + currentHeight + " new:" + iCompleteTextWidth + "," + iCompleteTextHeight);
                        mTextDecorationImage = null;
                        mIndividualCharactersCoverage = null;
                    }
                }

                // generate cimage it does not exists
                if (mTextDecorationImage == null)
                {
                  //  Console.WriteLine("Create in text bitmap, surface calc from=" + textSurfaceFontCalcWidth.ToString() + "," + textSurfaceFontCalcHeight.ToString() + 
                   //    "  bitmap size=" +iCompleteTextWidth.ToString() + "," + iCompleteTextHeight.ToString());
                    mTextDecorationImage = new CImage(this, iCompleteTextWidth, iCompleteTextHeight);
                   
                    float the_size = GetFontSizeForCoverageArea( new RectangleF(0,0, textSurfaceFontCalcWidth, textSurfaceFontCalcHeight));
                    mStyle.FontSize = the_size;
                }

                // Take into account we need a bigger cimage if individual characters are rendered
                if (drawOnlyCharacterIndex != -1)
                {
                    if (mIndividualCharactersCoverage == null)
                    {
                        using (Graphics gp = Graphics.FromImage(mMeasureStringBitmap))
                        {
                            CalcCoverageAreaForIndividualCharacters(gp, textSurfaceFontCalcWidth, textSurfaceFontCalcHeight);
                        }
                    }

                    coverage = ((CharacterPosition)mIndividualCharactersCoverage[drawOnlyCharacterIndex]).Position;
                    RectangleF src_uv = (RectangleF)mIndividualCharactersCoverageUV[drawOnlyCharacterIndex];
                    src_x = src_uv.X;
                    src_y = src_uv.Y;
                    src_width = (src_uv.Width + src_x);
                    src_height = (src_uv.Height + src_y);
                }

                RectangleF originalTotalTextCoverageArea = mCoverageArea;
                try
                {
                    mCoverageArea = coverage;

                    RectangleF rrr = mTextDecorationImage.GetRectangle();

                    src_x /= rrr.Width ;
                    src_y /= rrr.Height; ;
                    src_width /= rrr.Width;
                    src_height /= rrr.Height;

                    drawn_region = RenderToGraphics(mTextDecorationImage, mCoverageArea, src_x, src_y, src_width, src_height, r, frame_num, originating_slide, input_image);
                }
                finally
                {
                    mCoverageArea = originalTotalTextCoverageArea;
                }
            }
            // exception occured e.g. rare bug where text completely off screen
            // whatever just ignore
            catch
            {
            }

            return drawn_region;
		}

        //*******************************************************************
        public override void DeclareSlideAspectChange(CGlobals.OutputAspectRatio oldAspect, CGlobals.OutputAspectRatio newAspect)
        {
           // Re-generate coverage area based on new aspect
           float size = GetFontSizeForCoverageArea();
           this.RecalcCoverageAreaForFontSize(size);
        }

	
		//*******************************************************************
		public override void Save(XmlElement parent, XmlDocument doc)
		{
			XmlElement decoration = doc.CreateElement("Decoration");

			decoration.SetAttribute("Type","TextDecoration");

            SaveTextDecrationPart(decoration, doc);
       		
			parent.AppendChild(decoration); 
		}

        //*******************************************************************
		public void SaveTextDecrationPart(XmlElement decoration, XmlDocument doc)
		{
            SaveImageDecorationPart(decoration, doc);

            mStyle.Save(decoration, doc);

            if (mBackplane == true)
            {
                decoration.SetAttribute("Backplane", this.mBackplane.ToString());
                decoration.SetAttribute("BackplaneColor", this.mBackplaneColor.ToString());
                decoration.SetAttribute("BackplaneAlpha", this.mBackplaneTransparency.ToString());
            }
        
			decoration.SetAttribute("Text",this.mText);

            if (mHeader == true)
            {
                decoration.SetAttribute("Header", this.mHeader.ToString());
            }

            // if we've not generate the coverage area yet we better store the font size instead
            if (this.mCoverageArea.Width == 0)
            {
                decoration.SetAttribute("FontSize", this.mStyle.FontSize.ToString());
            }

			if (VCDNumber==true)
			{
				decoration.SetAttribute("VCDNumber", this.mVCDNumber.ToString());
			}

            if (mTemplateEditable != TemplateEditableType.None)
            {
                decoration.SetAttribute("TemplateEditable", ((int)mTemplateEditable).ToString());
            }

            if (mSetTextHint == true)
            {
                decoration.SetAttribute("SetText", mSetTextHint.ToString());
            }

            if (mHintedEditTime >= 0)
            {
                decoration.SetAttribute("HintEditTime", mHintedEditTime.ToString());
            }

            if (mBlurAmount != 0)
            {
                decoration.SetAttribute("BlurAmount", mBlurAmount.ToString());
            }
        }
		
		//*******************************************************************
		public override void Load(XmlElement element)
		{
            LoadTextDecorationPart(element);
        }

        //*******************************************************************
        public void LoadTextDecorationPart(XmlElement element)
        {
            LoadImageDecorationPart(element);

            XmlNodeList list = element.GetElementsByTagName("TextStyle");
            if (list.Count > 0)
            {
                XmlElement e = list[0] as XmlElement;
                mStyle.Load(e);
            }
            else
            {
                mStyle.Load(element);  // legacy
            }

            string s = element.GetAttribute("Backplane");
            if (s != "")
            {
                mBackplane = bool.Parse(s);

                s = element.GetAttribute("BackplaneColor");
                if (s != "")
                {
                    mBackplaneColor = CGlobals.ParseColor(s);
                }

                s = element.GetAttribute("BackplaneAlpha");
                if (s != "")
                {
                    mBackplaneTransparency = float.Parse(s, CultureInfo.InvariantCulture);
                }
            }

			mText = element.GetAttribute("Text");

            s = element.GetAttribute("OriginalTextInTemplate");
            if (s != "")
            {
                mOriginalTemplateText = s;
            }

			s = element.GetAttribute("Header");
			if (s!="") this.mHeader = bool.Parse(s);

			s = element.GetAttribute("VCDNumber");
			if (s!="")
			{
				this.mVCDNumber=bool.Parse(s);
			}

            // if there is no coverage area is there a font size? 
            if (this.mCoverageArea.Width == 0)
            {
                s = element.GetAttribute("FontSize");
                if (s != "")
                {
                    this.mStyle.FontSize = float.Parse(s);
                }
                else if (mStyle.FontSize==0)
                {
                    SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromDecoration(this);
                    ManagedCore.CDebugLog.GetInstance().Warning("No coverage area or font size defined for text '" + this.mText + "' in slideshow '" + message.Slideshow + "' slide:" + message.SlideNumber);
                }
            }

			if (mStyle.FontSize==0.0) mStyle.FontSize = 26.0f;

            s = element.GetAttribute("TemplateEditable");
            if (s != "")
            {
                mTemplateEditable = (TemplateEditableType)int.Parse(s);
            }

            s = element.GetAttribute("SetText");
            if (s != "")
            {
                mSetTextHint = bool.Parse(s);
            }

            s = element.GetAttribute("HintEditTime");
            if (s != "")
            {
                mHintedEditTime = float.Parse(s);
            }


            s = element.GetAttribute("BlurAmount");
            if (s != "")
            {
                mBlurAmount = int.Parse(s);
            }

			Init();
		}

		//*******************************************************************
		public void SetColor(Color c)
		{
			mStyle.TextColor = Color.FromArgb ( (int) ((1.0f - Transparency)*255.0f), c.R, c.G, c.B ) ;
		}

		//*******************************************************************
		public void SetFont(string font)
		{
			mStyle.FontName = font;
		}

		//*******************************************************************
		public void SetItalic(bool val)
		{
			mStyle.Italic = val;
		}

		//*******************************************************************
		public void SetBold(bool val)
		{
			mStyle.Bold = val;
		}

        // ### TODO SRG attached decors don't work
		//*******************************************************************
	//	public void SetAttchedToSlide(bool val)
	//	{
	//		this.mAttachedToSlideImage = val;
	//	}

		//*******************************************************************
		public void SetShadow(bool val)
		{
			this.mStyle.Shadow = val;
		}

		//*******************************************************************
		public void SetBackplane(bool val)
		{
            if (this.mBackplane != val)
            {
                this.mBackplane = val;
                this.InvalidateFont();
            }
		}

		//*******************************************************************
		public void SetOutline(bool val)
		{
			this.mStyle.Outline=val;
		}

		//*******************************************************************
		public void SetBackPlaneColor(Color c)
		{
			mBackplaneColor =   Color.FromArgb ( (int) mBackplaneTransparency, c.R, c.G, c.B ) ;	
		}

		//*******************************************************************
		public void SetBackPlaneTransparency(float val)
		{
			if (val<0.0f) val=0.0f;
			if (val>1.0f) val=1.0f;
			this.mBackplaneTransparency=val*255.0f;
			SetBackPlaneColor(this.mBackplaneColor);
		}


        //*******************************************************************
        public override void MarkAsBackgroundDecoration()
        {
            SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromDecoration(this);
            Log.Error("Tried to mark a text decor as a background decoration in slideshow '" + message.Slideshow + "' slide:" + message.SlideNumber);
            UnMarkAsBackgroundDecoration();
        }

        //*******************************************************************
        // calculated each frame 
        // SRG TODO OPTIMISE (DOES NOT NEED TO CALC THIS EVERY FRAME)
        protected override void GenerateSubAnimations(CAnimatedDecorationEffect effect, int framenum)
        {
            mSubAnimatedDocorations = null;

            if (!effect.HasCharacterTimeDelay)
            {
                return;
            }

            int numCharacters = mText.Length;
            if (numCharacters <= 1) return;

            float initialTimeOffset =0;
            float nextCharecterTimeOffset = effect.CharacerTimeDelayInSeconds;

            if (effect.CharacterOrder == CAnimatedDecorationEffect.CharacterDelayOrder.LAST_TO_FIRST)
            {
                initialTimeOffset = -(nextCharecterTimeOffset * numCharacters);
                nextCharecterTimeOffset = -nextCharecterTimeOffset;
            }
     
            mSubAnimatedDocorations = new List<IAnimatedDecoration>();
            float currentTimeOffset = initialTimeOffset;

            if (effect.CharacterOrder == CAnimatedDecorationEffect.CharacterDelayOrder.RANDOM)
            {
              // SRG TODO
            }
            else
            {
                for (int characterIndex = 0; characterIndex < numCharacters; characterIndex++)
                {
                    CTextCharacterSubAnimationDecoration nextChacterTextDecoration = new CTextCharacterSubAnimationDecoration(this, currentTimeOffset, characterIndex);

                    mSubAnimatedDocorations.Add(nextChacterTextDecoration);
                    currentTimeOffset -= nextCharecterTimeOffset;
                }
            }
        }
    }
}
