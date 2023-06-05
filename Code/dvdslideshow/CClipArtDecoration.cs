using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using ManagedCore;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for CClipArtDecoration.
    /// </summary>
    public class CClipArtDecoration : CImageDecoration
    {
        public CImage mImage;
        private bool mBlackAndWhite = false;
        private float[] mBrightness = { 1, 1, 1 };
        private float[] mContrast = { 1, 1, 1 };

        private bool mOriginalBlackAndWhite = false;
        private float[] mOriginalBrightness = { 1, 1, 1 };  // as loaded from the template
        private float[] mOriginalContrast = { 1, 1, 1 }; // as loaded from the template

        private bool mBorderDecoration = false;
 
        override public bool MaskImage
        {
            set 
            {
                if (base.MaskImage != value)
                {
                    base.MaskImage = value;
                    if (mImage != null)
                    {
                        mImage.ImageIsUsedAsMask = value;
                        GraphicsEngine.Current.ClearCachedTexuresForImage(mImage);
                    }
                }
            }
            get { return base.MaskImage; }
        }

        // *************************************************************************
        public bool BlackAndWhite
        {
            get { return mBlackAndWhite; }
            set
            {
                mBlackAndWhite = value;
                if (mImage != null)
                {
                    mImage.BlackAndWhite = value;
                }
            }
        }

        // *************************************************************************
        public bool OriginalBlackAndWhite
        {
            get { return mOriginalBlackAndWhite; }
            set { mOriginalBlackAndWhite = value; }
        }


        // *************************************************************************
        public float[] Brightness
        {
            set
            {
                if (mImage != null)
                {
                    mImage.Brightness = value;
                }
                mBrightness = value;
            }
            get
            {
                return mBrightness;
            }
        }

        // *************************************************************************
        public float[] OriginalBrightness
        {
            get { return mOriginalBrightness; }
            set { mOriginalBrightness = value; }
        }


        // *************************************************************************
        public float[] Contrast
        {
            set
            {
                if (mImage != null)
                {
                    mImage.Contrast = value;
                }
                mContrast = value;
            }
            get
            {
                return mContrast;
            }
        }

        // *************************************************************************
        public float[] OriginalContrast
        {
            get { return mOriginalContrast; }
            set { mOriginalContrast = value; }
        }


        //*******************************************************************
        public override bool IsBorderDecoration()
        {
            return mBorderDecoration;
        }    

        //*******************************************************************
        public void MarkAsBorderDecoration()
        {
            mBorderDecoration = true;
        }

        //*******************************************************************
        public CClipArtDecoration()
        {
            mAttachedToSlideImage = false;
        }

        //*******************************************************************
        public CClipArtDecoration(string filename, RectangleF coverage, int order)
            : base(coverage, order)
        {

            mImage = new CImage(filename, false, RotateFlipType.RotateNoneFlipNone);

            mAttachedToSlideImage = false;
        }

        //*******************************************************************
        public CClipArtDecoration(CImage image, RectangleF coverage, int order)
            : base(coverage, order)
        {
            mImage = image;
            mAttachedToSlideImage = false;
        }


        //*******************************************************************
        public CClipArtDecoration(CClipArtDecoration copy)
            : base(copy)
        {
            mImage = copy.mImage.Clone();
        }

        //*******************************************************************
        public static CClipArtDecoration FromStillPictureSlide(CStillPictureSlide slide)
        {
            CClipArtDecoration cad = new CClipArtDecoration(slide.GetSourceFileName(), new RectangleF(0, 0, 1, 1), 0);
            return cad;
        }

        //*******************************************************************
        public override CDecoration Clone()
        {
            return new CClipArtDecoration(this);
        }

        //*******************************************************************
        public override Rectangle RenderToGraphics(Graphics gp, RectangleF r, int framenum, CSlide originating_slide, CImage original_image)
        {
            return new Rectangle(0, 0, 1, 1);
        }

        //*******************************************************************
        public override Rectangle RenderToGraphics(RectangleF r, int framenum, CSlide originating_slide, RenderSurface inputSurface)
        {
            if (mImage == null)  
            {
                return new Rectangle(0, 0, 1, 1);
            }

            float sw = 1;
            float sh = 1;
            float sx = 0;
            float sy = 0;

            RectangleF coverageArea = mCoverageArea;

            // Do we need to adjust our coverage area because of a master background image?
            // this only applies if we are currently not 16:9 (default)
            if (mOriginalTemplateImageNumber != 0 &&
                originating_slide.MasterBackgroundImageAspect != 0 &&
                CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio != CGlobals.OutputAspectRatio.TV16_9)
            {
                coverageArea = RecalcCoverageAreaBasedOnMasterMaskCoverageArea(originating_slide.MasterBackgroundImageAspect, originating_slide.MasterBackgroundImageAspectType);
            }
             
            if (mUsePreviousDecorationCoverageArea == false)
            {
                // If Keep aspect by clipping enabled, clip some of the source uv coords so we keep our original source image aspect ratio (needed for templates)
                if (mDrawImageAspectType == CImage.DrawImageWithAspectType.KeepAspectByClipping)
                {
                    // HAS to be aspect non rotated, the rotation is worked out in the call to CalcImageSourceClipAreaToMaintainAspectRatioWithCoverageArea
                    float imageWidth = 1.0f;
                    float imageHeight = 1.0f / mImage.Aspect;

                    RectangleF srcRec = CalcImageSourceClipAreaToMaintainAspectRatioWithCoverageArea(coverageArea, imageWidth, imageHeight);
                    sx = srcRec.X;
                    sy = srcRec.Y / imageHeight;
                    sw = (srcRec.Width + sx);
                    sh = (srcRec.Height + srcRec.Y) / imageHeight;
                }
                // Keep aspect ratio by force changing the coverage area (either shriking width, or height, keeping centre point the same)
                else if (mDrawImageAspectType == CImage.DrawImageWithAspectType.KeepAspectByChangingCoverageArea)
                {
                    float imageWidth = 1.0f;
                    float imageHeight = 1.0f / GetOriginialImageAspectRatio();
                    coverageArea = CGlobals.CalcBestFitRectagleF(coverageArea, new RectangleF(0, 0, imageWidth, imageHeight / CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction()));
                }
                mLastCoverageArea = coverageArea;
                mLastRotation = mRotation;
            }
            else
            {
                coverageArea = GetLastCoverageAreaPlusGainPercent();
                mRotation = mLastRotation;
            }

            // just incase 
            if ( mRenderMethod == RenderMethodType.NORMAL_NO_IMAGE_ALPHA_BLEND)
            {
                mRenderMethod = RenderMethodType.NORMAL;
            }

            return RenderToGraphics(mImage, coverageArea, sx, sy, sw, sh, r, framenum, originating_slide, inputSurface);
        }

        //*******************************************************************
        public override void SetCrop(RectangleF clip)
        {
            if (mImage != null)
            {
                mImage.Crop = clip;
            }
        }

        //*******************************************************************
        public override RectangleF GetCrop()
        {
            if (mImage != null)
            {
                return mImage.Crop;
            }
            return base.GetCrop();
        }

        //*******************************************************************
        public override Size GetOriginalFileDimension()
        {
            if (mImage != null)
            {
                return mImage.MaxSizeImageFromFile;
            }
            return base.GetOriginalFileDimension();
        }

        //*******************************************************************
        public override float GetOriginialImageAspectRatio()
        {
            if (mImage != null)
            {
                if (mOrientation == OrientationType.CW90 ||
                    mOrientation == OrientationType.CW270)
                {
                    return 1.0f / mImage.Aspect;
                }
                return mImage.Aspect;
            }
            return base.GetOriginialImageAspectRatio();
        }

        //*******************************************************************
        public override void SetAsTemplateImageNumber(int number)
        {
            mImage.ImageFilename = "image" + number.ToString();
            mOriginalTemplateImageNumber = number;
        }

        //*******************************************************************
        public override int GetTemplateImageNumber()
        {
            if (mImage.ImageFilename.ToLower().StartsWith("image"))
            {
                return int.Parse(mImage.ImageFilename.Substring(5));
            }

            return 0;
        }

        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc)
        {
            XmlElement decoration = doc.CreateElement("Decoration");

            decoration.SetAttribute("Type", "ImageDecoration");
        
            SaveImageDecorationPart(decoration, doc);

            if (mBlackAndWhite == true)
            {
                decoration.SetAttribute("BW", mBlackAndWhite.ToString());
            }

            if (mBrightness[0] != 1 || mBrightness[1] != 1 || mBrightness[2] != 1)
            {
                decoration.SetAttribute("Brightness", mBrightness[0].ToString() + "," + mBrightness[1].ToString() + "," + mBrightness[2].ToString());
            }

            if (mContrast[0] != 1 || mContrast[1] != 1 || mContrast[2] != 1)
            {
                decoration.SetAttribute("Contrast", mContrast[0].ToString() + "," + mContrast[1].ToString() + "," + mContrast[2].ToString());
            }

            if (mBorderDecoration == true)
            {
                decoration.SetAttribute("Border", mBorderDecoration.ToString());
            }

            mImage.Save(decoration, doc, "Image");
            parent.AppendChild(decoration);
        }

        //*******************************************************************
        public void ReplaceWithImage(string filename)
        {
            mImage = new CImage();
            mImage.BlackAndWhite = mBlackAndWhite;
            mImage.Brightness = mBrightness;
            mImage.Contrast = mContrast;
            mImage.Init(filename, false, RotateFlipType.RotateNoneFlipNone, 1.0f);
        }

        //*******************************************************************
        public override void DeclareSlideAspectChange(CGlobals.OutputAspectRatio oldAspect, CGlobals.OutputAspectRatio newAspect)
        {
            if (IsBorderDecoration() == false &&
                IsBackgroundDecoration() == false &&
                IsFilter() == false &&
                mDrawImageAspectType == CImage.DrawImageWithAspectType.Stretch)
            {
                AdjustCoverageAreaForNewAspect(oldAspect, newAspect);
            }
        }

        //*******************************************************************
        public override bool VerfifyAllMediaFilesToRenderThisExist()
        {
            bool result = true;
            if (mImage != null)
            {
                if (mImage.ImageFilename != "")
                {
                    result = CImage.VerifyMediaExistsForFilename(mImage.ImageFilename);
                }
            }

            return result;
        }

        //*******************************************************************
        public override void Load(XmlElement element)
        {
            LoadImageDecorationPart(element);

            mImage = new CImage();

            if (MaskImage == true)
            {
                mImage.ImageIsUsedAsMask = true;
            }

            string s = element.GetAttribute("BW");
            if (s != "")
            {
                mBlackAndWhite = bool.Parse(s);
                mImage.BlackAndWhite = mBlackAndWhite;
            }

            s = element.GetAttribute("Brightness");
            if (s != "")
            {
                string[] vars = s.Split(',');
                mBrightness[0] = float.Parse(vars[0]);
                mBrightness[1] = float.Parse(vars[1]);
                mBrightness[2] = float.Parse(vars[2]);
                mImage.Brightness = mBrightness;
            }


            s = element.GetAttribute("Contrast");
            if (s != "")
            {
                string[] vars = s.Split(',');
                mContrast[0] = float.Parse(vars[0]);
                mContrast[1] = float.Parse(vars[1]);
                mContrast[2] = float.Parse(vars[2]);
                mImage.Contrast = mContrast;
            }

      //      mImage.Aspect = mCoverageArea.Width / mCoverageArea.Height;
        //    mImage.LoadImageWithAspect = CImage.LoadImageWithAspectType.KeepAspectByClipping;


            s = element.GetAttribute("Border");
            if (s != "")
            {
                mBorderDecoration = bool.Parse(s);
            }

            mImage.Load(element.GetElementsByTagName("Image")[0] as XmlElement, false);

            int templateNumber = GetTemplateImageNumber();

            // is this a template image
            if (templateNumber != 0)
            {
                mOriginalTemplateImageNumber = templateNumber;
                mOriginalBlackAndWhite = mBlackAndWhite;
                mOriginalBrightness = mBrightness;
                mOriginalContrast = mContrast;

            }
        }

        //*******************************************************************
        public override bool HasTemplateImageFilename()
        {
            if (mImage != null)
            {
                return CImage.IsTemplateFilename(mImage.ImageFilename);
            }

            return false;
        }
    }
}
