using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Drawing;
using System.Xml;
using ManagedCore;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{

    public class CBlankStillPictureSlide : CStillPictureSlide
    {

        public Color BlankColor
        {
            get { return mBackGroundColour; }
            set
            {
                if (mBackGroundColour != value)
                {
                    mBackGroundColour = value;
                    CGlobals.mCurrentProject.DeclareChange(true, "Changed blank slide background colour");
                }
            }
        }

        //**************************************************************
        public CBlankStillPictureSlide()
        {
            this.mUsePanZoom = false;
        }


        //**************************************************************
        public CBlankStillPictureSlide(Color color)
        {
            mBackGroundColour = color;

            this.mUsePanZoom = false;
        }



        //*******************************************************************
        // Create a blank slide given a clipart decoration, to show at max size
        public CBlankStillPictureSlide(CClipArtDecoration clipartDecoration)
        {
            CImageDecoration firstDecoration = null;

            RectangleF area = new RectangleF(0, 0, 1, 1);

            float aspect = 1;
         
            aspect = clipartDecoration.mImage.Aspect;
    
            firstDecoration = clipartDecoration;

            this.UsePanZoom = true;
            firstDecoration.UseParentSlidePanZoomAsDefaultMovement = true;

            mPanZoom.CalcBestRectangleAreas(clipartDecoration.mImage);

            float ww = aspect;
            float hh = 1 / CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction();

            area = CGlobals.CalcBestFitRectagleF(area, new RectangleF(0, 0, ww, hh));
            firstDecoration.CoverageArea = area;

            if (firstDecoration.VerfifyAllMediaFilesToRenderThisExist() == true)
            {
                this.AddDecoration(firstDecoration);
            }
        }


        //*******************************************************************
        // Create a blank slide given a video decoration, to show at max size
        public CBlankStillPictureSlide(CVideoDecoration videoDecoration)
        {
            if (videoDecoration.Player == null)
            {
                return;
            }

            CImageDecoration firstDecoration = null;

            RectangleF area = new RectangleF(0, 0, 1, 1);

            float aspect = ((float)videoDecoration.Player.GetWidth()) / ((float)videoDecoration.Player.GetHeight());

            firstDecoration = videoDecoration;

            this.mDisplayTimeLength = (float)videoDecoration.GetTrimmedVideoDurationInSeconds();

            this.UsePanZoom = false;

            float ww = aspect;
            float hh = 1 / CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction();

            area = CGlobals.CalcBestFitRectagleF(area, new RectangleF(0, 0, ww, hh));
            firstDecoration.CoverageArea = area;

            if (firstDecoration.VerfifyAllMediaFilesToRenderThisExist() == true)
            {
                this.AddDecoration(firstDecoration);
            }
        }

   
        //*******************************************************************
        public override CSlide Clone()
        {
            return new CBlankStillPictureSlide(this);
        }


        //*******************************************************************
        public CBlankStillPictureSlide(CBlankStillPictureSlide copy)
            : base(copy)
        {

        }

        //**************************************************************
        public override void Rotate(int amount)
        {
            // do nothing
        }


        //*******************************************************************
        protected override CImage GetVideo(int frame, bool ignore_pan_zoom, bool ignore_decors, int req_width, int req_height, CImage render_to_this)
        {
            RenderBackgroundColour();

            return base.GetVideo(frame, ignore_pan_zoom, ignore_decors, req_width, req_height, render_to_this);
        }

        //*******************************************************************
        public override string GetSourceFileName()
        {
            return "Blank";
        }


        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc)
        {
            XmlElement slide = doc.CreateElement("Slide");

            slide.SetAttribute("Type", "BlankStillPictureSlide");

            SaveImageSlidePart(slide, doc);

            mPanZoom.Save(slide, doc);

            slide.SetAttribute("Color", this.mBackGroundColour.ToString());

            parent.AppendChild(slide);
        }


        //*******************************************************************
        public override void Load(XmlElement element)
        {
            LoadImageSlidePart(element);

            XmlElement element2 = (XmlElement)element.FirstChild;

            XmlNodeList e1 = element.GetElementsByTagName("PanZoom");
            if (e1 != null && e1.Count > 0)
            {
                mPanZoom.Load(e1[0] as XmlElement);
            }
            else
            {
               // CDebugLog.GetInstance().Warning("No pan zoom defined, setting it....");
                if (mUsePanZoom == false)
                {
                   // mPanZoom.CalcBestRectangleAreas(mImage);
                }
            }

            string s = element.GetAttribute("Color");
            if (s != "")
            {
                this.mBackGroundColour = CGlobals.ParseColor(s);
            }
        }

        //*******************************************************************
        public override CDecoration SupportsSimpleOrientationChange()
        {
            ArrayList list = GetAllAndSubDecorations();
            CDecoration foundDecoration = null;

            foreach (CDecoration d in list)
            {
                if (d is CClipArtDecoration || d is CVideoDecoration)
                {
                    if (d.IsFilter() == false &&
                        d.IsBorderDecoration() == false &&
                        d.IsBackgroundDecoration() == false &&
                        d.IsTemplateFrameworkDecoration() == false)
                    {
                        if (foundDecoration != null)
                        {
                            return null;
                        }
                        foundDecoration = d;
                    }
                }
            }
            return foundDecoration;
        }

        //*******************************************************************
        public override bool SimpleOrientateChangeClockwise()
        {
            CDecoration d = SupportsSimpleOrientationChange();

            if (d!=null)
            {
                CImageDecoration imageDec =d as CImageDecoration;
                if (imageDec != null)
                {
                    imageDec.RotateCW90();
                    imageDec.ReCalcCoverageAreaToMatchOriginalImageAspectRatio();
                    return true;
                }
            }
            return false;
        }

        //*******************************************************************
        public override bool SimpleOrientateChangeAniClockwise()
        {
            CDecoration d = SupportsSimpleOrientationChange();

            if (d !=null)
            {
                CImageDecoration imageDec = d as CImageDecoration;
                if (imageDec != null)
                {
                    imageDec.RotateCCW90();
                    imageDec.ReCalcCoverageAreaToMatchOriginalImageAspectRatio();
                    return true;
                }
            }

            return false;
        }
    }
}
