using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using ManagedCore;
using DVDSlideshow.GraphicsEng;
using System.Collections.Generic;
using System.Collections;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for CClipArtDecoration.
    /// </summary>
    public abstract class CImageDecoration : CAnimatedDecoration
    {
        private static CClipArtDecoration mHighlightDecor = null;


        public enum RenderMethodType
        {
            NORMAL,
            ADD,
            SUBTRACT_DEST_MINUS_SRC,
            SUBTRACT_SRC_MINUS_DEST,
            MODULATE1X,
            MODULATE2X,
            MODULATE4X,
            NORMAL_NO_IMAGE_ALPHA_BLEND,
            MASK1,         // 00ff0000
            MASK2,         // 0000ff00
            MASK3,         // 000000ff
            MASK4          // 00ff00ff
        }

        // The enum match the degree rotation ( to be backwards compatible)
        public enum OrientationType
        {
            NONE =0,
            CW90 = 90,
            CW180 = 180,
            CW270 = 270
        }

        protected OrientationType mOrientation = OrientationType.NONE;
        protected bool mXFlipped = false;
        protected bool mYFlipped = false;
        protected float mRotation = 0;
        private bool mBackgroundDecoration = false;
        private bool mMaskImage = false;        // if true image is drawn with bilinear turned off
        private bool mDrawHighlighted = false;
      
        virtual public bool MaskImage
        {
            set { mMaskImage = value; }
            get { return mMaskImage; }
        }

        public bool DrawHighlighted
        {
            get { return mDrawHighlighted; }
            set { mDrawHighlighted = value; }
        }


        public float Rotation
        {
            get { return mRotation; }
            set { mRotation = value; }
        }

        public OrientationType Orientation
        {
            get { return mOrientation; }
            set { mOrientation = value; }
        }

        public bool XFlipped
        {
            get { return mXFlipped; }
            set { mXFlipped = value; }
        }

        public bool YFlipped
        {
            get { return mYFlipped; }
            set { mYFlipped = value; }
        }

        protected RenderMethodType mRenderMethod = RenderMethodType.NORMAL;
        private CImage mAlphaMap = null;
  
        protected CImage.DrawImageWithAspectType mDrawImageAspectType = CImage.DrawImageWithAspectType.Stretch;  // only applies to clipart and video decors

        public CImage.DrawImageWithAspectType DrawImageWithAspectType
        {
            get { return mDrawImageAspectType; }
            set { mDrawImageAspectType = value; }
        }

        // Represents an image that is not displayed in the edit slide media form, i.e. is a framework image for the template
        private bool mMarkedAsTemplateFrameworkDecoration = false;

        // Decoration is marked as originally an image number, (needed to all groups of same numbers appear in edit slide media form)
        // i.e. we need to remmeber if this decoration was originally a template image number
        protected int mOriginalTemplateImageNumber = 0;

        public int OriginalTemplateImageNumber
        {
            get { return mOriginalTemplateImageNumber; }
            set { mOriginalTemplateImageNumber = value; }
        }

        public bool MarkedAsTemplateFrameworkDecoration
        {
            get { return mMarkedAsTemplateFrameworkDecoration; }
            set { mMarkedAsTemplateFrameworkDecoration = value; }
        }

        //*******************************************************************
        public virtual void MarkAsBackgroundDecoration()
        {
            mBackgroundDecoration = true;
        }

        //*******************************************************************
        public void UnMarkAsBackgroundDecoration()
        {
            mBackgroundDecoration = false;
        }

        //*******************************************************************
        public override bool IsBackgroundDecoration()
        {
            return mBackgroundDecoration;
        }

        //*******************************************************************
        public RotateFlipType GetRotateFlipType()
        {
            RotateFlipType type = RotateFlipType.RotateNoneFlipNone;

            if (mOrientation == OrientationType.NONE)
            {
                if (mXFlipped == false && mYFlipped == false) return RotateFlipType.RotateNoneFlipNone;
                if (mXFlipped == true && mYFlipped == false) return RotateFlipType.RotateNoneFlipX;
                if (mXFlipped == false && mYFlipped == true) return RotateFlipType.RotateNoneFlipY;
                if (mXFlipped == true && mYFlipped == true) return RotateFlipType.RotateNoneFlipXY;
            }

            if (mOrientation == OrientationType.CW90)
            {
                if (mXFlipped == false && mYFlipped == false) return RotateFlipType.Rotate90FlipNone;
                if (mXFlipped == true && mYFlipped == false) return RotateFlipType.Rotate90FlipX;
                if (mXFlipped == false && mYFlipped == true) return RotateFlipType.Rotate90FlipY;
                if (mXFlipped == true && mYFlipped == true) return RotateFlipType.Rotate90FlipXY;
            }

            if (mOrientation == OrientationType.CW180)
            {
                if (mXFlipped == false && mYFlipped == false) return RotateFlipType.Rotate180FlipNone;
                if (mXFlipped == true && mYFlipped == false) return RotateFlipType.Rotate180FlipX;
                if (mXFlipped == false && mYFlipped == true) return RotateFlipType.Rotate180FlipY;
                if (mXFlipped == true && mYFlipped == true) return RotateFlipType.Rotate180FlipXY;
            }

            if (mOrientation == OrientationType.CW270)
            {
                if (mXFlipped == false && mYFlipped == false) return RotateFlipType.Rotate270FlipNone;
                if (mXFlipped == true && mYFlipped == false) return RotateFlipType.Rotate270FlipX;
                if (mXFlipped == false && mYFlipped == true) return RotateFlipType.Rotate270FlipY;
                if (mXFlipped == true && mYFlipped == true) return RotateFlipType.Rotate270FlipXY;
            }

            return type;

        }

        //*************************************************************************************************
        // Called if image source media has been changed, or if a 90 rotation has occrued
        public void ReCalcCoverageAreaToMatchOriginalImageAspectRatio()
        {
            if (this.DrawImageWithAspectType == CImage.DrawImageWithAspectType.Stretch)
            {
                RectangleF currentCoverage = mCoverageArea;

                float aspect = GetOriginialImageAspectRatio();

                float newCoverageHeight = currentCoverage.Height;
                float newCoverasgWidth = currentCoverage.Height * aspect;

                newCoverasgWidth *= CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction();

                float xdiff = currentCoverage.Width - newCoverasgWidth;
                float x = currentCoverage.X + (xdiff / 2.0f);
                float y = currentCoverage.Y;

                RectangleF newCoverageArea = new RectangleF(x, y, newCoverasgWidth, newCoverageHeight);
                mCoverageArea = newCoverageArea;
            }
        }


        //*******************************************************************
        public virtual float GetOriginialImageAspectRatio()
        {
            Log.Error("CImageDecoration GetAspectRatio called");
            return 1;
        }

        //*******************************************************************
        public void ResetRotateAndFlip()
        {
            mXFlipped = false;
            mYFlipped = false;
            mOrientation = OrientationType.NONE;
        }

        //*******************************************************************
        public override bool IsTemplateFrameworkDecoration()
        {
            if (IsBorderDecoration() == true) return true;
            if (IsBackgroundDecoration() == true) return true;

            return mMarkedAsTemplateFrameworkDecoration;
        }

        public CImage AlphaMap
        {
            get { return mAlphaMap; }
            set { mAlphaMap = value; }
        }

        public RenderMethodType RenderMethod
        {
            get { return mRenderMethod; }
            set { mRenderMethod = value; }
        }

        //*******************************************************************
        public CImageDecoration()
        {
        }

        //*******************************************************************
		public CImageDecoration(RectangleF coverage, int order) : base(coverage,order)
		{
		}

		//*******************************************************************
        public CImageDecoration(CImageDecoration copy)
            : base(copy)
		{
            mOrientation = copy.mOrientation;
            mXFlipped = copy.mXFlipped;
            mYFlipped = copy.mYFlipped;
            mRenderMethod = copy.mRenderMethod;
            if (copy.mAlphaMap != null)
            {
                mAlphaMap = mAlphaMap.Clone();
            }
          
		}


        //*******************************************************************
        public void RotateCCW90()
        {
            switch (mOrientation)
            {
                case OrientationType.NONE:
                    mOrientation = OrientationType.CW270;
                    break;
                case OrientationType.CW90:
                    mOrientation = OrientationType.NONE;
                    break;
                case OrientationType.CW180:
                    mOrientation = OrientationType.CW90;
                    break;
                case OrientationType.CW270:
                    mOrientation = OrientationType.CW180;
                    break;
                default:
                    break;
            }
        }

        //*******************************************************************
        public void RotateCW90()
        {
            switch (mOrientation)
            {
                case OrientationType.NONE:
                    mOrientation = OrientationType.CW90;
                    break;
                case OrientationType.CW90:
                    mOrientation = OrientationType.CW180;
                    break;
                case OrientationType.CW180:
                    mOrientation = OrientationType.CW270;
                    break;
                case OrientationType.CW270:
                    mOrientation = OrientationType.NONE;
                    break;
                default:
                    break;
            }
        }

        //***********************************************************
        public void FlipX()
        {
            mXFlipped = !mXFlipped;
        }


        //***********************************************************
        public void FlipY()
        {
            mYFlipped = !mYFlipped;
        }


        //*******************************************************************
        public override bool IsLayer()
        {
            if (mRenderMethod == RenderMethodType.NORMAL || mRenderMethod == RenderMethodType.NORMAL_NO_IMAGE_ALPHA_BLEND)
            {
                return false;
            }
            return true;
        }


        //*******************************************************************
        public override bool IsFilter()
        {
            if (IsBorderDecoration()==true) return false;

            if (mRenderMethod == RenderMethodType.ADD ||
                mRenderMethod == RenderMethodType.MODULATE1X ||
                mRenderMethod == RenderMethodType.MODULATE2X ||
                mRenderMethod == RenderMethodType.MODULATE4X ||
                mRenderMethod == RenderMethodType.SUBTRACT_DEST_MINUS_SRC ||
                mRenderMethod == RenderMethodType.SUBTRACT_SRC_MINUS_DEST)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Matched with RenderMethodType enum
        /// </summary>
        private static string [] mPixelShaders = { "none", "add", "subdms", "subsmd", "modulate", "modulate2x", "modulate4x", "none", "mask1", "mask2", "mask3", "mask4" };


        //*******************************************************************
        private void Apply90DegreeStepRotation(PointF[] points)
        {
            if (mOrientation == OrientationType.CW90)
            {
                PointF temp = points[0];
                points[0] = points[1];
                points[1] = points[2];
                points[2] = points[3];
                points[3] = temp;
            }
            else if (mOrientation == OrientationType.CW180)
            {
                PointF temp = points[0];
                points[0] = points[2];
                points[2] = temp;
                temp = points[1];
                points[1] = points[3];
                points[3] = temp;
            }
            else if (mOrientation == OrientationType.CW270)
            {
                PointF temp = points[0];
                points[0] = points[3];
                points[3] = points[2];
                points[2] = points[1];
                points[1] = temp;
            }
        }

        //*******************************************************************
        // private method incase we cull when drawing this decoration, but have an inputsurface
        private void ReRenderInputSurface(RectangleF r, RenderSurface inputSurface)
        {
            RenderMethodType type = mRenderMethod;
            mRenderMethod = RenderMethodType.NORMAL;
            DrawImage(inputSurface, r, new RectangleF(0, 0, 1, 1), r.X, r.Y + r.Height, r.X, r.Y, r.X + r.Width, r.Y + r.Height, r.X + r.Width, r.Y, null);
            mRenderMethod = type;
        }
     
        //*******************************************************************
        protected Rectangle RenderToGraphics(DrawableObject DrawableObject, RectangleF coverageArea, float srcStartU, float srcStartV, float srcEndU, float srcEndV, RectangleF r, int framenum, CSlide originating_slide, RenderSurface inputSurface)
        {
            if (InAnimatedTimeWindow(framenum, originating_slide) == false)
            {
                // Nothing to render, but this is a layer, so just re-render the input surface as normal render type
                if (inputSurface != null)
                {
                    ReRenderInputSurface(r, inputSurface);
                }
     
                return new Rectangle(0, 0, 1, 1); ;
            }

            // Apply Y flip?
            if (mYFlipped)
            {
                float temp = srcStartV;
                srcStartV = srcEndV;
                srcEndV = temp;
            }

            // Apply X flip?
            if (mXFlipped)
            {
                float temp = srcStartU;
                srcStartU = srcEndU;
                srcEndU = temp;
            }

            RectangleF UVCoords = new RectangleF(srcStartU, srcStartV, srcEndU, srcEndV);
            CRectangleReferenceFrame positionRF = GetDecorationPosition(coverageArea, UVCoords, framenum, originating_slide);

            float x = r.Width * positionRF.position.X + r.X;
            float y = r.Height * positionRF.position.Y + r.Y;
            float width = r.Width * positionRF.position.Width;
            float height = r.Height * positionRF.position.Height;

            // Draw the base image
            Rectangle drawnArea = new Rectangle((int)(x), (int)(y), 1+(int)(width + 0.4999), 1+(int)(height + 0.4999));     // need to add 1 to width and height, not sure why
            RectangleF r2 = new RectangleF(x, y, width, height);
            PointF[] points = null;

            // Apply rotation ? e.g. 23' 30' not flipped ( This comes from our animated effect part as well as this from mRotation)
            if (positionRF.rotation != 0 || mRotation !=0)
            {
                DisposableObject<Matrix> rot_mat = new DisposableObject<Matrix>();

                float xx = x + (width / 2) + (positionRF.offsetRotX * r.Width);
                float yy = y + (height / 2) + (positionRF.offsetRoyY * r.Height);

                rot_mat.Assign( new Matrix() );
                rot_mat.Object.Rotate(positionRF.rotation + mRotation);
                rot_mat.Object.Translate(xx, yy, MatrixOrder.Append);

                points = new PointF[4];
                drawnArea = CalcDrawnAreaAfterRotation(r2, rot_mat, points);
                rot_mat.Dispose();
            }

            //
            // ### SRG todo G933 apply UVCoord rotation
            //

            // Apply rotation i.e. in 90 degree steps (This is a simpler method to do rotation than using a animated effect)
            if (mOrientation != OrientationType.NONE)
            {
                if (points == null)
                {
                    points = new PointF[4];
                    points[0] = new PointF(r2.X, r2.Y);
                    points[1] = new PointF(r2.X + r2.Width, r2.Y);
                    points[2] = new PointF(r2.X + r2.Width, r2.Y + r2.Height);
                    points[3] = new PointF(r2.X, r2.Y + r2.Height);
                }

                Apply90DegreeStepRotation(points);
            }

        
            if (points == null)
            {
                DrawImage(DrawableObject, r, positionRF.UVCoords, r2.X, r2.Y + r2.Height, r2.X, r2.Y, r2.X + r2.Width, r2.Y + r2.Height, r2.X + r2.Width, r2.Y, inputSurface);
            }
            else
            {
                DrawImage(DrawableObject, r, positionRF.UVCoords, points[3].X, points[3].Y, points[0].X, points[0].Y, points[2].X, points[2].Y, points[1].X, points[1].Y, inputSurface);
            }

            return drawnArea;
        }

        
        //*******************************************************************
        private RectangleF GetBounds(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            float min_x = x1;
            if (x2 < min_x) min_x = x2;
            if (x3 < min_x) min_x = x3;
            if (x4 < min_x) min_x = x4;

            float max_x = x1;
            if (x2 > max_x) max_x = x2;
            if (x3 > max_x) max_x = x3;
            if (x4 > max_x) max_x = x4;

            float min_y = y1;
            if (y2 < min_y) min_y = y2;
            if (y3 < min_y) min_y = y3;
            if (y4 < min_y) min_y = y4;

            float max_y = y1;
            if (y2 > max_y) max_y = y2;
            if (y3 > max_y) max_y = y3;
            if (y4 > max_y) max_y = y4;

            return new RectangleF(min_x, min_y, max_x - min_x, max_y - min_y);
        }

        //*******************************************************************
        private void DrawImage(DrawableObject srcImage, RectangleF canvasRec, RectangleF srcRect, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, RenderSurface inputSurface)
        {
            // Fast cull check if off screen
            float mw = canvasRec.Width;
            float mh = canvasRec.Height;
      
            if ((x1 < 0  && x2 < 0  && x3 < 0  && x4 < 0 )  ||
                (x1 > mw && x2 > mw && x3 > mw && x4 > mw)  ||
                (y1 < 0  && y2 < 0  && y3 < 0  && y4 < 0 )  ||
                (y1 > mh && y2 > mh && y3 > mh && y4 > mh) )
            {
                if (inputSurface != null)
                {
                    ReRenderInputSurface(canvasRec, inputSurface);
                }
                return;
            }
        
            GraphicsEngine engine = GraphicsEngine.Current;
            if (engine == null) return;

            // See if mipmap can be turned off to save memory
            if (srcImage.CreateMipMaps == true && ( IsBackgroundDecoration() || IsBorderDecoration() || IsFilter() ) )
            {
                srcImage.CreateMipMaps = false;
            }

            // Do normal image drawing?
            if (mRenderMethod == RenderMethodType.NORMAL ||
                mRenderMethod == RenderMethodType.NORMAL_NO_IMAGE_ALPHA_BLEND)
            {
                //
                // do we have an alpha map defined for this image decoration
                //

                bool includeAlphaChannel = true;

                if (mRenderMethod == RenderMethodType.NORMAL_NO_IMAGE_ALPHA_BLEND)
                {
                    includeAlphaChannel = false;
                }

                PixelShaderEffect alphaMapShader = null;

                try
                {
                    if (mAlphaMap != null)
                    {           
                                 
                        if (includeAlphaChannel == true)
                        {
                            alphaMapShader = PixelShaderEffectDatabase.GetInstance().GetPixelShader("alphamap");
                        }
                        else
                        {
                            alphaMapShader = PixelShaderEffectDatabase.GetInstance().GetPixelShader("alphamapNoImageAlpha");
                        }

                        if (alphaMapShader != null)
                        {
                            includeAlphaChannel = true;
                            alphaMapShader.SetInputImage1(mAlphaMap);
                           // alphaMapShader.SetParameter(0, 1);  //  ### SRG THIS IS NEEDED for diffuse default for DX11, but what about dx9 shader?  fix it
                            engine.SetPixelShaderEffect(alphaMapShader);
                        }
                    }

                    if (Transparency != 0)
                    {
                        engine.DrawImageWithDiffuseAlpha(srcImage, 1.0f - Transparency, srcRect, x1, y1, x2, y2, x3, y3, x4, y4, includeAlphaChannel);
                    }
                    else
                    {
                        // mask images when drawn should not do linear intepolation
                        if (mMaskImage == true)
                        {
                            engine.SetBilinearInterpolation(false);
                        }
                        engine.DrawImage(srcImage, srcRect, x1, y1, x2, y2, x3, y3, x4, y4, includeAlphaChannel);
                        if (mMaskImage == true)
                        {
                            engine.SetBilinearInterpolation(true);
                        }
                    }
                }
                finally
                {
                    if (alphaMapShader != null)
                    {
                        engine.SetPixelShaderEffect(null);
                    }
                }
            }
            else
            {

                // Ok speical layered drawing method like mod2x etc, always needs an input surface

                if (inputSurface == null)
                {
                    SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromDecoration(this);
                    Log.Error("No input surface for DrawImage with render type:" + mRenderMethod.ToString() + " in slideshow '" + message.Slideshow + "' slide:" + message.SlideNumber);
                    return;
                }

                PixelShaderEffect effect = PixelShaderEffectDatabase.GetInstance().GetPixelShader(mPixelShaders[(int)mRenderMethod]);
                if (effect == null)
                {
                    SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromDecoration(this);
                    ManagedCore.CDebugLog.GetInstance().Error("No pixel shader for render method:" + mRenderMethod.ToString() + " in slideshow '" + message.Slideshow + "' slide:" + message.SlideNumber);
                    return;
                }

                try
                {
                    if (mRenderMethod == RenderMethodType.MASK1 ||
                        mRenderMethod == RenderMethodType.MASK2 ||
                        mRenderMethod == RenderMethodType.MASK3 ||
                        mRenderMethod == RenderMethodType.MASK4)
                    {
                        using (RenderSurface newSurface = engine.GenerateRenderSurface(inputSurface.Width, inputSurface.Height, this.ToString() + "::DrawImage"))
                        {
                            RenderSurface currentTarget = engine.GetRenderTarget();
                            engine.SetRenderTarget(newSurface);

                            // This is needed incase multiple images are to be drawn in the same colour mask.  I.e. draw red on red etc
                            if (mRenderMethod == RenderMethodType.MASK1)
                            {
                                engine.ClearRenderTarget(255, 0, 0, 255);
                            }
                            if (mRenderMethod == RenderMethodType.MASK2)
                            {
                                engine.ClearRenderTarget(0, 255, 0, 255);
                            }
                            if (mRenderMethod == RenderMethodType.MASK3)
                            {
                                engine.ClearRenderTarget(0, 0, 255, 255);
                            }
                            if (mRenderMethod == RenderMethodType.MASK4)
                            {
                                engine.ClearRenderTarget(255, 0, 255, 255);
                            }

                            RectangleF rec = new RectangleF(0, 0, inputSurface.Width, inputSurface.Height);

                            // ### SRG TODO G410 this needs to only do section we draw on, but does not work
                            // one of the problems with this is the second uv coord setup in out c++ direct3d library


                            //  RectangleF destBounds = GetBounds(x1, y1, x2, y2, x3, y3, x4, y4);
                            //   destBounds = RectangleF.Intersect(rec, destBounds);
                            //   RectangleF srcBounds = new RectangleF(destBounds.X, destBounds.Y, destBounds.Width + destBounds.X, destBounds.Height + destBounds.Y);

                            engine.DrawImage(srcImage, srcRect, x1, y1, x2, y2, x3, y3, x4, y4, false);
                            engine.SetRenderTarget(currentTarget);
                            effect.SetInputImage1(inputSurface);
                            engine.SetPixelShaderEffect(effect);

                            // slower method but works
                            engine.DrawImage(newSurface, new RectangleF(0, 0, 1, 1), rec, false);
                            engine.SetPixelShaderEffect(null);
                        }
                    }
                    else
                    {
                        effect.SetInputImage1(inputSurface);
                        engine.SetPixelShaderEffect(effect);
                        engine.DrawImage(srcImage, srcRect, x1, y1, x2, y2, x3, y3, x4, y4, false);
                    }
                }
                finally
                {
                    engine.SetPixelShaderEffect(null);
                }
            }

          
        }

        //*******************************************************************
        public void DrawDecorationHightlighted(RectangleF r, int frame, CSlide originatingSlide)
        {
            if (mHighlightDecor == null)
            {
                mHighlightDecor = new CClipArtDecoration(CGlobals.GetGuiImagesDirectory() + "\\selected.png", new RectangleF(0, 0, 1, 1), 0);
            }

            float wf = mCoverageArea.Width / 20.0f;
            float wh = mCoverageArea.Height / 20.0f;

            mHighlightDecor.mCoverageArea = new RectangleF(mCoverageArea.X - wf, mCoverageArea.Y - wh, mCoverageArea.Width + (wf * 2), mCoverageArea.Height + (wh * 2));

            mHighlightDecor.Transparency = 0.5f;
            mHighlightDecor.Rotation = Rotation;
            mHighlightDecor.MoveInEffect = MoveInEffect;
            mHighlightDecor.MoveOutEffect = MoveOutEffect;
            mHighlightDecor.StartOffsetTimeRawValue = StartOffsetTimeRawValue;
            mHighlightDecor.EndOffsetTimeRawValue = EndOffsetTimeRawValue;
            mHighlightDecor.RenderToGraphics(r, frame, originatingSlide, null);
            mHighlightDecor.MoveInEffect = null;
            mHighlightDecor.MoveOutEffect = null;
        }

        //*******************************************************************
        public RectangleF CalcImageSourceClipAreaToMaintainAspectRatioWithCoverageArea(RectangleF mCoverageArea, float sw, float sh)
        {
            float ratio = CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction();

            float coverage_aspect = (mCoverageArea.Width) / (mCoverageArea.Height * ratio);

            float image_aspect = sw/sh;

            if (mOrientation == OrientationType.CW90 || mOrientation == OrientationType.CW270)
            {
                coverage_aspect = 1 / coverage_aspect;
            }

            if (image_aspect < coverage_aspect) // e.g. 4.3 on 16:9
            {
                float newh = sw / coverage_aspect;

                return new RectangleF(0, (sh - newh) / 2.0f, sw, newh);
            }
            else if (image_aspect > coverage_aspect)
            {
                float neww = sh * coverage_aspect;

                return new RectangleF( (sw - neww) / 2.0f, 0, neww, sh);
            }

            return new RectangleF(0,0,sw,sh);
        }


        //*******************************************************************
        public void SetToSmoothEdgeAlphamap()
        {
            if (IsFilter() == false && IsBackgroundDecoration() == false)
            {
                string fn = CGlobals.GetSmoothedEdgeAlphaMapFilename();

                // default is we give clipart decors a smooth edge alpha map
                if (fn != "")
                {
                    AlphaMap = new CImage(CGlobals.GetSmoothedEdgeAlphaMapFilename(), false);
                }
            }
        }


        //*******************************************************************
        // This method is to allow certain templates work at different ouput aspect ratios (e.g. pan zoom wooden)
        //
        // If a decoration is drawn to fit a background image for example, then when the backgroun drawn area id changed due to output aspect
        // change, then we have to make sure the decoration coverage area matches this change.
        // 
        // This assumes :-
        // The template was designed and alligned up with default 16:9 aspect.
        // The master image was drawn with "Fit to frame" or "Fill frame" and has a coverage area of whole screen (e.g. 0,0,1,1)
        protected RectangleF RecalcCoverageAreaBasedOnMasterMaskCoverageArea(float maskAspect, CImage.DrawImageWithAspectType masterAspectDrawnType) 
        {
            float currentFractionCGlobals = CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction();
            float imageWidth = maskAspect;
            float imageHeight = 1.0f;

            RectangleF wholeScreen = new RectangleF(0, 0, 1, 1);

            RectangleF maskNormalCoverageArea = mCoverageArea;
            RectangleF maskCurrentCoverageArea = mCoverageArea;

            if (masterAspectDrawnType == CImage.DrawImageWithAspectType.KeepAspectByChangingCoverageArea)
            {
                maskNormalCoverageArea = CGlobals.CalcBestFitRectagleF(wholeScreen, new RectangleF(0, 0, imageWidth / 1.77777f, imageHeight));
                maskCurrentCoverageArea = CGlobals.CalcBestFitRectagleF(wholeScreen, new RectangleF(0, 0, imageWidth * currentFractionCGlobals, imageHeight));
            }
            else if (masterAspectDrawnType == CImage.DrawImageWithAspectType.KeepAspectByClipping)
            {
                maskNormalCoverageArea = CGlobals.CalcBestFitZoomedRect(1 /1.77777f, maskAspect);
                maskCurrentCoverageArea = CGlobals.CalcBestFitZoomedRect(currentFractionCGlobals, maskAspect);
            }
            else
            {
                return mCoverageArea;
            }


            float normalwidth = (imageWidth / imageHeight) / 1.777777f;
            float newwidth = (imageWidth / imageHeight) * currentFractionCGlobals;

            float decLeft = mCoverageArea.X;
            float decRight = mCoverageArea.X + mCoverageArea.Width;
            float xrat = maskCurrentCoverageArea.Width / maskNormalCoverageArea.Width;
            float WidthDiff = 1 - maskNormalCoverageArea.Width;
            float HalfWidthDiff = WidthDiff / 2;
            float relLeft = (decLeft - HalfWidthDiff) * xrat;
            float relRight = (decRight - HalfWidthDiff) * xrat;

            float decTop = mCoverageArea.Y;
            float decBottom = mCoverageArea.Y + mCoverageArea.Height;
            float yrat = maskCurrentCoverageArea.Height / maskNormalCoverageArea.Height;
            float HeightDiff = 1 - maskNormalCoverageArea.Height;
            float HalfHeightDiff = HeightDiff / 2;
            float relTop = (decTop - HalfHeightDiff) * yrat;
            float relBottom = (decBottom - HalfHeightDiff) * yrat;

            RectangleF newCoverage = new RectangleF(relLeft + maskCurrentCoverageArea.X,
                                                    relTop + maskCurrentCoverageArea.Y,
                                                    relRight - relLeft,
                                                    relBottom - relTop);
            return newCoverage;
        }



        //*******************************************************************
        public virtual void SetCrop(RectangleF clip)
        {
        }

        //*******************************************************************
        public virtual RectangleF GetCrop()
        {
            return new RectangleF(0, 0, 1, 1);
        }

        //*******************************************************************
        public virtual Size GetOriginalFileDimension()
        {
            return new Size(0,0);
        }

        //*******************************************************************
        public virtual void SetAsTemplateImageNumber(int number)
        {
        }

        //*******************************************************************
        public virtual int GetTemplateImageNumber()
        {
            return 0;
        }

        //*******************************************************************
        protected void SaveImageDecorationPart(XmlElement element, XmlDocument doc)
        {

            if (mOrientation != OrientationType.NONE)
            {
                element.SetAttribute("Rotate", ((int)mOrientation).ToString());
            }

            if (mXFlipped != false)
            {
                element.SetAttribute("FlipX", mXFlipped.ToString());
            }

            if (mYFlipped != false)
            {
                element.SetAttribute("FlipY", mYFlipped.ToString());
            }

            if (mRotation != 0)
            {
                element.SetAttribute("DegreesRot", mRotation.ToString());
            }

            if (mRenderMethod != RenderMethodType.NORMAL)
            {
                element.SetAttribute("RenderMethod", ((int)mRenderMethod).ToString());
            }

            if (mMarkedAsTemplateFrameworkDecoration == true)
            {
                element.SetAttribute("TemplateFramework", mMarkedAsTemplateFrameworkDecoration.ToString());
            }

            if (mAlphaMap != null)
            {
                mAlphaMap.Save(element, doc, "AlphaMap");
            }

            if (mDrawImageAspectType == CImage.DrawImageWithAspectType.KeepAspectByClipping ||
                mDrawImageAspectType == CImage.DrawImageWithAspectType.KeepAspectByChangingCoverageArea)
            {
                element.SetAttribute("Aspect", ((int)mDrawImageAspectType).ToString());
            }

            if (mOriginalTemplateImageNumber != 0)
            {
                element.SetAttribute("OriTemImageNum", mOriginalTemplateImageNumber.ToString());
            }

            if (mBackgroundDecoration == true)
            {
                element.SetAttribute("Background", mBackgroundDecoration.ToString());
            }

            if (mMaskImage == true)
            {
                element.SetAttribute("Mask", mMaskImage.ToString());
            }
          
            SaveAnimatedDecorationPart(element, doc);
        }

        //*******************************************************************
        protected void LoadImageDecorationPart(XmlElement element)
        {
            // 90 degree rotation and flipping 
            string s = element.GetAttribute("Rotate");
            if (s != "")
            {
                mOrientation = (OrientationType) int.Parse(s);
            }

            s = element.GetAttribute("FlipX");
            if (s != "")
            {
                mXFlipped = bool.Parse(s);
            }

            s = element.GetAttribute("FlipY");
            if (s != "")
            {
               mYFlipped = bool.Parse(s);
            }

            s = element.GetAttribute("DegreesRot");
            if (s != "")
            {
                mRotation = float.Parse(s);
            }

            s = element.GetAttribute("RenderMethod");
            if (s != "")
            {
                mRenderMethod = (RenderMethodType)int.Parse(s);
            }

            s = element.GetAttribute("TemplateFramework");
            if (s != "")
            {
                mMarkedAsTemplateFrameworkDecoration = bool.Parse(s);
            }

            XmlNodeList list = element.GetElementsByTagName("AlphaMap");
            if (list.Count > 0 && list[0].ParentNode == element)
            {
                mAlphaMap = new CImage();
                mAlphaMap.Load(list[0] as XmlElement, false);
            }

            s = element.GetAttribute("Aspect");
            if (s != "")
            {
                mDrawImageAspectType = (CImage.DrawImageWithAspectType)int.Parse(s);
            }

            s = element.GetAttribute("OriTemImageNum");
            if (s != "")
            {
                this.mOriginalTemplateImageNumber = int.Parse(s);
            }


            s = element.GetAttribute("Background");
            if (s != "")
            {
                mBackgroundDecoration = bool.Parse(s);
            }

            s = element.GetAttribute("Mask");
            if (s != "")
            {
                mMaskImage = bool.Parse(s);
            }
 
            LoadAnimatedDecorationPart(element);
        }
    }
}
