using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using ManagedCore;
using System.Collections.Generic;
using System.Collections;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
 
    /// <summary>
    /// This class reprenetns a group of decorations which result in one ImageDecoration
    /// Which can then be animated
    /// </summary>
    /// 
    public class CGroupedDecoration : CImageDecoration
    {
        //*******************************************************************
        private static List<CGroupedDecoration> mCached = new List<CGroupedDecoration>();
        private static void DeclareBeingRendered(CGroupedDecoration decoration)
        {
            if (mCached.Contains(decoration) == false)
            {
                // free up memory from old grouped decors
                if (mCached.Count > 1)
                {
                    mCached[0].ClearCachedImage();
                    mCached.RemoveAt(0);
                }

                mCached.Add(decoration);
            }
        }

        private List<CDecoration> mGroupedDecorations = new List<CDecoration>();

        // All sub decors are also rendered with parent slide motion blur settings.
        // Normally this is not needed, but when on is very very slow at encoding
        private bool mForceSubDecorMotionBlue = false;

        private ActiveReader<RenderSurface> mRenderedSurface = null;

        //*******************************************************************
        public List<CDecoration> GroupedDecorations
        {
            get { return mGroupedDecorations; }
        }

        //*******************************************************************
        //*******************************************************************
        public override bool IsTemplateFrameworkDecoration()
        {
            return true;
        }

        //*******************************************************************
        public CGroupedDecoration()
        {
        }

        //*******************************************************************
        public void AddDecoration(CDecoration d)
        {
            // if background, add at start;
            if (d.IsBackgroundDecoration() == true)
            {
                mGroupedDecorations.Insert(0, d);
            }
            else
            {
                // If last decor is border, insert before that
                if (mGroupedDecorations.Count > 0 &&
                    mGroupedDecorations[mGroupedDecorations.Count - 1].IsBorderDecoration())
                {
                    mGroupedDecorations.Insert(mGroupedDecorations.Count - 1, d);
                }
                else
                {
                    mGroupedDecorations.Add(d);
                }
            }
        }

        //*******************************************************************
        public void RemoveDecoration(CDecoration d)
        {
            if (mGroupedDecorations.Remove(d) == false)
            {
                SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromDecoration(this);
                Log.Error("Failed to remove decoration in GroupedDecoration in slideshow '" + message.Slideshow + "' slide:" + message.SlideNumber);
            }
        }

        //*******************************************************************
        public void SwapDecorations(CDecoration decorA, CDecoration decorB)
        {
            int indexA = mGroupedDecorations.IndexOf(decorA);
            int indexB = mGroupedDecorations.IndexOf(decorB);
            if (indexA >= 0 && indexB >= 0)
            {
                mGroupedDecorations[indexA] = decorB;
                mGroupedDecorations[indexB] = decorA;
            }
            else
            {
                SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromDecoration(this);
                Log.Error("Could not swap decorations in GroupedDecoration in slideshow '" + message.Slideshow + "' slide:" + message.SlideNumber);
            }
        }

        //*******************************************************************
        public void ReplaceDecoration(CDecoration decorA, CDecoration withDecorB)
        {
            int indexA = mGroupedDecorations.IndexOf(decorA);
            if (indexA >= 0 )
            {
                mGroupedDecorations[indexA] = withDecorB;
            }
            else
            {
                SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromDecoration(this);
                Log.Error("Could not replace decoration in GroupedDecoration in slideshow '" + message.Slideshow + "' slide:" + message.SlideNumber);
            }
        }

        //*******************************************************************
        // This decoration should a user moveable one
        public void SendToBack(CDecoration d)
        {
            if (mGroupedDecorations.Contains(d) == false)
            {
                SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromDecoration(this);
                Log.Error("Could not move decoration to back in GroupedDecoration in slideshow '" + message.Slideshow + "' slide:" + message.SlideNumber);
                return;
            }

            if (mGroupedDecorations.Count == 1) return;

            RemoveDecoration(d);

            if (mGroupedDecorations.Count > 0)
            {
                bool inserted = false;
                CImageDecoration firstDecor = mGroupedDecorations[0] as CImageDecoration;
                if (firstDecor != null)
                {
                    if (firstDecor.IsBackgroundDecoration() == true)
                    {
                        mGroupedDecorations.Insert(1, d);
                        inserted = true;
                    }
                }
                if (inserted == false)
                {
                    mGroupedDecorations.Insert(0, d);
                }
            }
        }

        //*******************************************************************
        public CGroupedDecoration(RectangleF coverage, int order)
            : base(coverage, order)
        {
        }

        //*******************************************************************
        public CGroupedDecoration(CGroupedDecoration copy)
            : base(copy)
        {
        }


        //*******************************************************************
        public override CDecoration Clone()
        {
            return new CGroupedDecoration(this);
        }

        //*******************************************************************
        public void ClearCachedImage()
        {
            if (mRenderedSurface != null)
            {
                mRenderedSurface.Dispose();
                mRenderedSurface = null;
            }
        }

        //*******************************************************************
        private bool HasLayerDecorations()
        {
            foreach (CDecoration d in this.mGroupedDecorations)
            {
                if (d.IsLayer() == true)
                {
                    return true;
                }
            }
            return false;
        }


        //*******************************************************************
        public override void ResetAllMediaStreams()
        {
            foreach (CDecoration dec in this.mGroupedDecorations)
            {
                dec.ResetAllMediaStreams();
            }

            base.ResetAllMediaStreams();
        }

        //*******************************************************************
        public override void StopAllNonPlayingMediaStreams(CSlide currentSlide, CSlide nextSlide)
        {
            foreach (CDecoration dec in this.mGroupedDecorations)
            {
                dec.StopAllNonPlayingMediaStreams(currentSlide, nextSlide);
            }

            base.StopAllNonPlayingMediaStreams(currentSlide, nextSlide);
        }

        //*******************************************************************
        private float CalculateMaxZoomOfGroup(CSlide originalSlide)
        {
            float maxzoom = 1.0f;

            bool set = false;

            CAnimatedDecorationEffect mie = MoveInEffect;
            if (mie != null)
            {
                float miezoom = mie.GetMaxZoom();
                if (miezoom > maxzoom)
                {
                    maxzoom = miezoom;
                    set = true;
                }
            }

            CAnimatedDecorationEffect moe = MoveOutEffect;
            if (moe != null)
            {
                float moezoom = moe.GetMaxZoom();
                if (moezoom > maxzoom)
                {
                    maxzoom = moezoom;
                    set = true;
                }
            }

            // maybe we using pan/zoom on group?
            if (set == false)
            {
                if (UseParentSlidePanZoomAsDefaultMovement == true)
                {
                    CImageSlide imageSlide = originalSlide as CImageSlide;
                    if (imageSlide != null)
                    {
                        if (imageSlide.UsePanZoom == true)
                        {
                            float mz = imageSlide.PanZoom.GetMaxZoom();
                            if (mz > maxzoom)
                            {
                                maxzoom = mz;
                            }
                        }
                    }
                }
            }


            return maxzoom;
        }

        //*******************************************************************
        public static float GetMaxGroupDecorationWidth()
        {
            // this matches max with in CImage size
            return 2560;
        }

        //*******************************************************************
        private bool IsStaticGroupDecoration()
        {
            // if all members of the group are non video and have no animations effects, then this group decoration is static
            // this is useful to know as can be a big speed up when rendering

            foreach (CDecoration d in mGroupedDecorations)
            {
                if (d is CVideoDecoration)
                {
                    return false;
                }

                CAnimatedDecoration ad = d as CAnimatedDecoration;
                if (ad != null)
                {
                    if (ad.MoveInEffect != null)
                    {
                        if (ad.MoveInEffect.IsStatic() == false)
                        {
                            return false;
                        }
                    }

                    if (ad.MoveOutEffect != null)
                    {
                        if (ad.MoveOutEffect.IsStatic() == false)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        //*******************************************************************
        private bool SubDecorsHaveMotion()
        {
            foreach (CDecoration d in mGroupedDecorations)
            {
                CAnimatedDecoration ad = d as CAnimatedDecoration;
                if (ad != null)
                {
                    if (ad.MoveInEffect != null && ad.MoveInEffect.RequiredMotionBlur > 1)
                    {
                        return true;
                    }
                    if (ad.MoveOutEffect != null && ad.MoveOutEffect.RequiredMotionBlur > 1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        //*******************************************************************
        public override Rectangle RenderToGraphics(RectangleF r, int framenum, CSlide originating_slide, RenderSurface inputSurface)
        {
            GraphicsEngine ge = GraphicsEngine.Current;

            CImageSlide imageSlide = originating_slide as CImageSlide;

            // Pre conditions
            if (ge == null || mGroupedDecorations.Count ==0 || imageSlide == null)
            {
                return new Rectangle(0, 0, 1, 1);
            }

            bool doSubDecorMotionBlur = mForceSubDecorMotionBlue;

            // If final mode, need to test if group has motion
            if (doSubDecorMotionBlur == false && CGlobals.mVideoGenerateMode == VideoGenerateMode.FINAL_OUTPUT_MODE)
            {
                doSubDecorMotionBlur = SubDecorsHaveMotion();
            }

            // do this BEFORE creating the surface, to save memory
            // i.e. clear cache space then create the surface
            DeclareBeingRendered(this);

            float maxzoom  = CalculateMaxZoomOfGroup(originating_slide);
            float max_allowed_zoom = GetMaxGroupDecorationWidth() / r.Width;
            if (maxzoom > max_allowed_zoom)
            {
                maxzoom = max_allowed_zoom;
            }
             
            float fwidth = r.Width * maxzoom;
            float fheight = r.Height * maxzoom;

            uint iwidth = (uint)fwidth;
            uint iheight = (uint)fheight;

            bool update_rendersurace = true;
            bool regenerated = false;

            // ok make sure we have a cached bitmap to render the group to
            if (mRenderedSurface== null ||
                mRenderedSurface.IsNull() == true ||
                mRenderedSurface.Object.Width != iwidth ||
                mRenderedSurface.Object.Height != iheight)
            {
                if (mRenderedSurface != null)
                {
                    mRenderedSurface.Dispose();
                }

                // Create new renderSurface 
                mRenderedSurface = ge.GeneratePersistentRenderSurface(iwidth, iheight, true, this.ToString()+"::RenderToGraphics");
                regenerated= true;
            }
            else
            {
                // We already have a cached surface. 
                // If not a slide motion blur keyframe and we have sub decor motion(blur)
                // turned off, simply render the last generated image (cached bitmap) with parent
                if (Encoder.CurrentEncodeFrameKeyFrame == false && doSubDecorMotionBlur == false)
                {
                    update_rendersurace = false;
                }
                // This is a massive speed up, i.e. don't bother re-creating a static group decoration when encoding ( as it would not have notchanged)
                // PhotoCruz does not have edit ability, so this is also true for that with it's big group decorations
                else if ((CGlobals.RunningPhotoCruz == true || CGlobals.mVideoGenerateMode == VideoGenerateMode.FINAL_OUTPUT_MODE) &&
                         IsStaticGroupDecoration() == true)
                {
                    update_rendersurace = false;
                }
            }

            if (update_rendersurace == true)
            {
                if (mGroupedDecorations.Count != 0)
                {
                    // ok create one out of sub decorations;

                    RenderSurface original_surface = ge.GetRenderTarget();

                    ge.SetRenderTarget(mRenderedSurface);
                    try
                    {
                        Color blankColor = Color.Black;

                        // if Parent slide a CBlankStillPictureSlide, render that blank colour
                        CBlankStillPictureSlide bsps = originating_slide as CBlankStillPictureSlide;
                        if (bsps != null)
                        {
                            blankColor = bsps.BlankColor;
                        }

                        ge.ClearRenderTarget(blankColor.R, blankColor.G, blankColor.B, 255);

                        ArrayList list =new ArrayList();
                        list.AddRange(mGroupedDecorations);

                        if (HasLayerDecorations() == true)
                        {
                            ge.SetRenderTarget(ge.GenerateRenderSurface(iwidth, iheight, this.ToString()+"::RenderToGraphics2"));
                            ge.ClearRenderTarget(blankColor.R, blankColor.G, blankColor.B, 255);
                        }

                        Rectangle rec = new Rectangle(0, 0, (int)iwidth, (int)iheight);
                        // Let our parent slide draw our sub group decorations

                        bool next_frame_key_frame = Encoder.NextEncodeFrameKeyFrame;

                        // Ok if sub decors don't have motion and we had a cache image then next frame MUST BE A KEY FRAME, this is needed to advance video frames
                        if (regenerated == false && doSubDecorMotionBlur == false)
                        {
                            Encoder.NextEncodeFrameKeyFrame = true;
                        }
                        try
                        {
                            imageSlide.RenderUnAttachedDecorations(list, rec, framenum, false, mRenderedSurface);
                        }
                        finally
                        {
                            Encoder.NextEncodeFrameKeyFrame = next_frame_key_frame;
                        }
                    }
                    finally
                    {
                        ge.SetRenderTarget(original_surface);
                    }
                }
            }
    
            RenderMethod = RenderMethodType.NORMAL_NO_IMAGE_ALPHA_BLEND;

            return base.RenderToGraphics(mRenderedSurface, mCoverageArea, 0,0, 1,1, r, framenum, originating_slide, null);
        }

        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc)
        {
            XmlElement decoration = doc.CreateElement("Decoration");

            decoration.SetAttribute("Type", "GroupedDecoration");
            decoration.SetAttribute("MotionBlur", mForceSubDecorMotionBlue.ToString());

            SaveImageDecorationPart(decoration, doc);

            foreach (CDecoration d in mGroupedDecorations)
            {
                d.Save(decoration, doc);
            }

            parent.AppendChild(decoration);
        }


        //*******************************************************************
        public override void Load(XmlElement element)
        {
            LoadImageDecorationPart(element);

            string s = element.GetAttribute("MotionBlur");
            if (s != "")
            {
                mForceSubDecorMotionBlue = bool.Parse(s);
            }

            XmlNodeList dec_list = element.GetElementsByTagName("Decoration");

            foreach (XmlElement d in dec_list)
            {
                CDecoration de = CDecoration.CreateFromType(d.GetAttribute("Type"));

                try
                {
                    de.Load(d);
                    mGroupedDecorations.Add(de);
                }
                catch (MissingFileException mfe)
                {
                    // photocruz hack! if we've not specified something then ignore it.
                    if (CGlobals.RunningPhotoCruz == true && mfe.mFilename.ToLower().StartsWith("image") == false)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
