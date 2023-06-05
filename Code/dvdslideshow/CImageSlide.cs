/*
 * Created by SharpDevelop.
 * User: user
 * Date: 03/01/2005
 * Time: 13:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using ManagedCore;
using System.Xml;
using DVDSlideshow.GraphicsEng;
using System.Collections.Generic;

namespace DVDSlideshow
{  
	/// <summary>
	/// Description of CImageSlide.	
	/// A slide which can produce an image and contains decoraions
	/// </summary>
	public abstract class CImageSlide : CSlide
	{
		protected ArrayList	mDecorations;

        // Some slides with text are too slow to preview play when editing, so you can set this to false
        private bool mPreviewPlayWhenEditingSlideMedia = true;

        public bool PreviewPlayWhenEditingSlideMedia
        {
            get { return mPreviewPlayWhenEditingSlideMedia; }
            set { mPreviewPlayWhenEditingSlideMedia = value; }
        }

        public ArrayList Decorations
        {
            get { return mDecorations; }
            set { mDecorations = value; }
        }

        public abstract CPanZoom PanZoom
        {
            get;
            set;
        }

		public abstract string SourceFilename
		{
			get;
		}

        //*******************************************************************
        public override void ResetAllMediaStreams()
        {
            foreach (CDecoration dec in mDecorations)
            {
                dec.ResetAllMediaStreams();
            }
        }

        //*******************************************************************
        public override void StopAllNonPlayingMediaStreams(CSlide currentSlide,CSlide nextSlide)
        {
            foreach (CDecoration dec in mDecorations)
            {
                dec.StopAllNonPlayingMediaStreams(currentSlide, nextSlide);
            }
        }


        //*******************************************************************
        public override void DeclareSlideAspectChange(CGlobals.OutputAspectRatio oldAspect, CGlobals.OutputAspectRatio newAspect)
        {
            ArrayList decs = GetAllAndSubDecorations();

            foreach (CDecoration d in decs)
            {
                d.DeclareSlideAspectChange(oldAspect, newAspect);
            }
        }


		//*******************************************************************
		public CImageSlide()
		{
			mDecorations = new ArrayList() ;
		}

		//*******************************************************************
		public CImageSlide(CImageSlide copy) : base(copy)
		{
			mDecorations = new ArrayList() ;

			foreach (CDecoration d in copy.mDecorations)
			{
				CDecoration dc = d.Clone();
				mDecorations.Add(dc);
			}
		}

		//*******************************************************************
		public CDecoration GetDecorationFromID(int id)
		{
			foreach (CDecoration d in mDecorations)
			{
				if (d.ID==id)
				{
					return d;
				}
			}

            SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromSlide(this);
			CDebugLog.GetInstance().Error("Unknown decoration id ("+id+") in slideshow '"+message.Slideshow+"' slide:"+message.SlideNumber);
			return null ;
		}


        //*************************************************************************************************
        // both decorations must already exist in slide
        public virtual void SwapDecorations(CDecoration decorA, CDecoration decorB)
        {
            int indexA = mDecorations.IndexOf(decorA);
            int indexB = mDecorations.IndexOf(decorB);
            if (indexA >= 0 && indexB >= 0)
            {
                mDecorations[indexA] = decorB;
                mDecorations[indexB] = decorA;
            }
            else
            {
                // perhaps in a group
                CGroupedDecoration firstGroup = GetFirstGroupedDecoration();
                if (firstGroup != null)
                {
                    firstGroup.SwapDecorations(decorA, decorB);
                }
                else
                {
                    SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromSlide(this);
                    Log.Error("Failed to Swap decorations in slideshow '" + message.Slideshow + "' slide:" + message.SlideNumber);
                }
            }
        }

        //*************************************************************************************************
        // decoration A must exist in slide, decoration B must not
        public virtual void RepaceDecoration(CDecoration decorA, CDecoration withDecorB)
        {
            int indexA = mDecorations.IndexOf(decorA);

            if (indexA >= 0)
            {
                mDecorations[indexA] = withDecorB;
            }
            else
            {
                // perhaps in a group
                CGroupedDecoration firstGroup = GetFirstGroupedDecoration();
                if (firstGroup != null)
                {
                    firstGroup.ReplaceDecoration(decorA, withDecorB);
                }
                else
                {
                    SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromSlide(this);
                    Log.Error("Failed to replace decoration in slideshow '" + message.Slideshow + "' slide:" + message.SlideNumber);
                }
            }
        }

        //*******************************************************************
        public virtual List<CGroupedDecoration> GetAllGroupDecorations()
        {
            List<CGroupedDecoration> returnList = new List<CGroupedDecoration>();

            foreach (CDecoration d in mDecorations)
            {
                CGroupedDecoration group = d as CGroupedDecoration;
                if (group != null)
                {
                    returnList.Add(group);
                }
            }

            return returnList;
        }

        //*******************************************************************
        public CGroupedDecoration GetFirstGroupedDecoration()
        {
            foreach (CDecoration d in mDecorations)
            {
                if (d is CGroupedDecoration) return d as CGroupedDecoration;
            }
            return null;
        }

        //*******************************************************************
        // Gets all decoraions, including those inside a group decoration
        public virtual ArrayList GetAllAndSubDecorations()
        {
            CGroupedDecoration firstGroup = GetFirstGroupedDecoration();
            if (firstGroup == null)
            {
                return mDecorations;
            }

            ArrayList returnList = new ArrayList();
            foreach (CDecoration d in mDecorations)
            {
                CGroupedDecoration group = d as CGroupedDecoration;
                if (group==null)
                {
                    returnList.Add(d);
                }
                else
                {
                    List<CDecoration> grouped = group.GroupedDecorations;
                    returnList.AddRange(grouped);
                }
            }
            return returnList;
        }

		//*******************************************************************
        // If a Group exists in the slide, it adds it that
		public virtual void AddDecoration(CDecoration decoration)
		{
			CMenuButton b = decoration as CMenuButton;
			if (b!=null)
			{
				if (b.AttachedToSlideImage == true)
				{
					CDebugLog.GetInstance().Warning("CMenuButton decoration attached to menu slide. Un-attaching");
					b.AttachedToSlideImage=false ;
				}
			}

     
            // if border add at end
            if (decoration.IsBorderDecoration() == true)
            {
                mDecorations.Add(decoration);
            }
            else
            {
                // if group add to that
                CGroupedDecoration firstGroup = GetFirstGroupedDecoration();
                if (firstGroup != null)
                {
                    firstGroup.AddDecoration(decoration);
                }
                else
                {
                    // if background, add at start;
                    if (decoration.IsBackgroundDecoration() == true)
                    {
                        mDecorations.Insert(0, decoration);
                    }
                    // If last decor is border, insert before that
                    else if (mDecorations.Count > 0 &&
                            (mDecorations[mDecorations.Count - 1] as CDecoration).IsBorderDecoration())
                    {
                        mDecorations.Insert(mDecorations.Count - 1, decoration);
                    }
                    else
                    {
                        // else add at end
                        mDecorations.Add(decoration);
                    }
                }
            }       
		}

		//*******************************************************************
		// ok add decoration from just a clip art filename
		// it will size the image at 100% in the middle
		public void AddImageDecoration(string filename)
		{
			RectangleF r = new RectangleF(0.25f,0.25f,0.5f,0.5f);
			CClipArtDecoration new_decor = new CClipArtDecoration(filename, r,0);

			AddDecoration(new_decor);
		}

        //*******************************************************************
        public bool ContainsDecoration(CDecoration d)
        {
            ArrayList list = GetAllAndSubDecorations();
            foreach (CDecoration dec in list)
            {
                if (dec == d) return true;
            }
            return false;
        }


		//*******************************************************************
		public void RemoveDecoration(CDecoration decoration)
		{

            if (mDecorations.Contains(decoration) == false)
            {
                CGroupedDecoration group = this.GetFirstGroupedDecoration();
                if (group != null)
                {
                    group.RemoveDecoration(decoration);
                }
                else
                {
                    SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromSlide(this);
                    Log.Error("Failed to remove decoration in slideshow '" + message.Slideshow + "' slide:" + message.SlideNumber);
                }
            }
            else
            {
                mDecorations.Remove(decoration);
            }
		}

        //*******************************************************************
        public virtual bool HasPreTransitionLayerDecorations(int frame)
        {

            // IMPORTANT, We do not want to get sub decorations at this point, only at this level
            foreach (CDecoration d in mDecorations)
            {
                if (d.IsLayer()==true && d.RenderPostTransition==false)
                {
                    return true;
                }
            }
            return false;
        }

        //*******************************************************************
        // Returns the number of different videos that are pladed during this slide
        // Videos that persist across multiple slides (if this is a multi slide slide)
        // only count as one video. i.e. They will use the same player
        //
        public int GetNumberOfDifferentVideosInSlide()
        {
            List<CVideoPlayer> videoPlayers = new List<CVideoPlayer>();

            int count = 0;
            if (this is CVideoSlide)
            {
                count++;   // video slides are legacy and won't use a same player as a decoration in the slide.
            }

            ArrayList list = this.GetAllAndSubDecorations();
            foreach (CDecoration d in list)
            {
                CVideoDecoration vd = d as CVideoDecoration;
                if (vd != null && vd.Player != null)
                {
                    if (videoPlayers.Contains(vd.Player) == false)
                    {
                        count++;
                        videoPlayers.Insert(0,vd.Player);
                    }
                }
            }
            return count;
        }

        //*******************************************************************
        private static bool IsLastLayer(ArrayList decorations, CDecoration layer_decoration, bool postTransition)
        {
            if (layer_decoration.IsLayer() == false) return false;

            bool found = false;

            foreach (CDecoration d in decorations)
            {
                if (d.RenderPostTransition != postTransition )
                {
                    continue;
                }

                if (found==true && d.IsLayer()==true)
                {
                    return false;
                }

                if (d == layer_decoration)
                {
                    found = true;
                }
            }
            return true;
        }

        //*******************************************************************
        public override void RenderFrame(int frame, bool ignore_pan_zoom, bool ignore_decors, int req_width, int req_height)
        {
            GraphicsEngine ge = GraphicsEngine.Current;
            if (ge == null) return;

            mFinalRenderSurface = ge.GetRenderTarget();

            if (ignore_decors == false)
            {
                // To render this slide, we may have to render many layers and "blend them together"
                // Therefore we need multiple render targets. The last one rendered after the call to GetVideo should be the current one

                bool layers = HasPreTransitionLayerDecorations(frame);

                if (layers == true)
                {             
                    // Ok we have to reader this slide into a texture, for fuuter later blending
                    RenderSurface rs = ge.GenerateRenderSurface((uint)req_width, (uint)req_height, "CImageSlide::RenderFrame");

                    ge.SetRenderTarget(rs);
                }
            }

            try
            {
                GetVideo(frame, ignore_pan_zoom, ignore_decors, req_width, req_height, null);
            }
            finally
            {
                // Should never happen but just in case
                RenderSurface current_render_target = ge.GetRenderTarget();

                if (mFinalRenderSurface != current_render_target)
                {
                    SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromSlide(this);
                    ManagedCore.CDebugLog.GetInstance().Error("Current render target not the last expected target in slideshow '" + message.Slideshow + "' slide:" + message.SlideNumber);
                }
            }
        }


		//*******************************************************************
        public override void RenderUnAttachedDecorations(Rectangle r, int frame_num, bool postTrans, RenderSurface finalSurface)
        {
            RenderUnAttachedDecorations(mDecorations, r, frame_num, postTrans, finalSurface);
    
        }

        //*******************************************************************
        public void RenderUnAttachedDecorations(ArrayList decorations, Rectangle r, int frame_num, bool postTrans, RenderSurface finalSurface)
        {
            foreach (CDecoration d in decorations)
            {
                //
                // Do no render this decoration if attached or if the render post transision flags don't match.
                // Note: When frame == -1 (i.e. snapshot frame for thumbnails etc).  we ignore the post transition flags
                //
                if (d.AttachedToSlideImage == true || (frame_num !=-1 && postTrans != d.RenderPostTransition))
                {
                    continue;
                }

                RectangleF rf = new RectangleF((float)r.X, (float)r.Y, (float)r.Width, (float)r.Height);


                // draw decoration highlighted?
                CImageDecoration imageDecor = d as CImageDecoration;
                if (imageDecor != null)
                {
                    if (imageDecor.DrawHighlighted == true)
                    {
                        imageDecor.DrawDecorationHightlighted(rf, frame_num, this);
                        imageDecor.DrawHighlighted = false;
                    }
                }

                RenderSurface inputRenderSurface = null;

                if (d.IsLayer() == true)
                {
                    inputRenderSurface = GraphicsEngine.Current.GetRenderTarget();

                    RenderSurface to_render_to_surface = null;

                    if (IsLastLayer(decorations, d, postTrans) == false)
                    {
                        // Ok we have to reader this slide into a texture, for futer later blending
                        to_render_to_surface = GraphicsEngine.Current.GenerateRenderSurface((uint)r.Width, (uint)r.Height, "CImageSlide::RenderUnAttachedDecorations");
                    }
                    else
                    {
                        to_render_to_surface = finalSurface;
                    }

                    GraphicsEngine.Current.SetRenderTarget(to_render_to_surface);
                }

                using (inputRenderSurface)
                {
              
                   
                    CAnimatedDecoration animatedDecoration = d as CAnimatedDecoration;
         
                    if (animatedDecoration != null)
                    {
                        //
                        // Some animated decorations are infact multiple decorations (e.g. text width different frame
                        // offsets).  If so genereate sub animations now
                        //
                        animatedDecoration.GenerateSubAnimations(frame_num, this);

                        // Render as sub decorations instead or not?
                        if (animatedDecoration.SubAnimatedDecorations == null)
                        {
                            RenderAnimatedDecoration(animatedDecoration, frame_num, r, rf, inputRenderSurface);
                        }
                        else
                        {
                            foreach (IAnimatedDecoration subDecoration in animatedDecoration.SubAnimatedDecorations)
                            {
                                RenderAnimatedDecoration(subDecoration, frame_num, r, rf, inputRenderSurface);
                            }
                        }
                    }
                    else
                    {
                        d.RenderToGraphics(rf, frame_num, this, inputRenderSurface);
                    }
                }
            }
        }

        //****************************************************************************************************************************************
        private void RenderAnimatedDecoration(IAnimatedDecoration d, int frame_num, Rectangle r, RectangleF rf, RenderSurface inputSurface)
        {
            bool processed = false;

            float sto = d.GetStartOffsetTime( DisplayLength );
            float eto = d.GetEndOffsetTime( DisplayLength );

            int adjusted_frame_num= frame_num;

            if (frame_num >= 0)
            {
                adjusted_frame_num = d.GetAdjustedFrameNum(frame_num);
                if (adjusted_frame_num < 0) return;
            }

            // If no frame number (e.g. editing decor in decorations window) then just render normal
            if (adjusted_frame_num < 0)
            {
                d.RenderToGraphics(rf, adjusted_frame_num, this, inputSurface);
                return;
            }


            // process fade out effect ?
            if (d.MoveOutEffect != null && d.MoveOutEffect.TransitionEffect != null)
            {
                CTransitionEffect moveouteffect = d.MoveOutEffect.TransitionEffect;
                float time_since_start_of_fade_out_start = -(-GetSecondsSinceStartOfSlide(adjusted_frame_num) + eto - d.MoveOutEffect.LengthInSeconds);

                if (time_since_start_of_fade_out_start >= 0)
                {
                    float delta = time_since_start_of_fade_out_start / d.MoveOutEffect.LengthInSeconds;
                    delta = moveouteffect.GetDeltaAferDelay(delta);
                    delta = 1 - delta;

                    if (delta == 0)
                    {
                        // don't draw 
                        processed = true;
                    }
                    else if (delta > 0 && delta < 1)
                    {
                        processRenderTransitionEffect(delta, d, r, rf, adjusted_frame_num, moveouteffect, rf);
                        processed = true;
                    }
                }
            }
            
            // process fade in effect ?
            if (processed == false && d.MoveInEffect != null && d.MoveInEffect.TransitionEffect != null)
            {
                CTransitionEffect moveineffect = d.MoveInEffect.TransitionEffect;
                float time_since_start_showing_decoration = GetSecondsSinceStartOfSlide(adjusted_frame_num) - sto;

                if (time_since_start_showing_decoration >= 0.0 )
                {
                    float outEffectTime = 0;
                    if (d.MoveOutEffect != null)
                    {
                        outEffectTime = d.MoveOutEffect.LengthInSeconds;
                    }

                    // move in effect lengths may be set to be same as decoration length minus out effect time
                    float InEffectlength = d.MoveInEffect.GetLengthInSeconds(d.GetLengthTimeShownFor(DisplayLength)-outEffectTime);

                    float delta = time_since_start_showing_decoration / InEffectlength;
                    delta = moveineffect.GetDeltaAferDelay( delta );

                    if (delta == 0)
                    {
                        // don't draw 
                        processed = true;
                    }
                    else if (delta > 0 && delta < 1)
                    {
                        processRenderTransitionEffect(delta, d, r, rf, adjusted_frame_num, moveineffect, rf);
                        processed = true;
                    }
                }
            }

         
            // just process normal
            if (processed == false)
            {
                d.RenderToGraphics(rf, adjusted_frame_num, this, inputSurface);
            }
        }

        //****************************************************************************************************************************************
        private void processRenderTransitionEffect(float delta, IAnimatedDecoration d, Rectangle r, RectangleF rf, int frame_num, CTransitionEffect effect, RectangleF clipRegion)
        {
            if (delta < 0) delta = 0;
            if (delta > 1) delta = 1;

            GraphicsEngine ge = GraphicsEngine.Current;
            if (ge == null) return;

            RenderSurface rs = ge.GenerateRenderSurface((uint)r.Width, (uint)r.Height, "CImageSLide::processRenderTransitionEffect");

            using (rs)
            {
               
                RenderSurface current_surface = ge.GetRenderTarget();

                // ### SRG TODO this only needs to clear the section it is going to draw to
                ge.SetRenderTarget(rs);
                ge.ClearRenderTarget(0, 0, 0, 0);  // 0,0,0,0 means render target will not support alpha blending when being rendered to

                CImageDecoration id = d as CImageDecoration;
             
                Rectangle drawnArea = d.RenderToGraphics(rf, frame_num, this, null);

                Rectangle clips2 = new Rectangle((int)clipRegion.X, (int)clipRegion.Y, (int)clipRegion.Width, (int)clipRegion.Height);

                // clip drawArea with original gp clip area  ( if reducded video area on )
                if (clips2.IntersectsWith(drawnArea))
                {
                    drawnArea = Rectangle.Intersect(clips2, drawnArea);
                }

                // make sure drawn area also clipped to original image dimensions
                if (r.IntersectsWith(drawnArea))
                {
                    drawnArea = Rectangle.Intersect(r, drawnArea);
                }

                // increase width and height by one pixel, not sure why we need to do this as such
                drawnArea.X += 1;
                drawnArea.Y += 1;

                /*
                // make sure draw area has width and height
                if (drawnArea.Width == 0) drawnArea.Width = 1;
                if (drawnArea.Height == 0) drawnArea.Height = 1;
                */

                ge.SetRenderTarget(current_surface);

                effect.UseImage2Alpha = true;   // Force transision effect to take into account image 2 alpha

                if (d is CVideoDecoration)
                {           
                    drawnArea.Width -= 1;       //### SRG FIX ME WHY DO I NEED THESE??!
                    drawnArea.Height -= 2;
                }
              
                effect.Process(rs, r.Width, r.Height, delta, frame_num, drawnArea);
            }

        }

		//*******************************************************************
        // This decoration should a user moveable one
		public void SendToBack(CDecoration d)
		{
            // if group add to that
            CGroupedDecoration firstGroup = GetFirstGroupedDecoration();
            if (firstGroup != null)
            {
               firstGroup.SendToBack(d);
            }
            else
            {
                if (mDecorations.Contains(d) == false)
                {
                    SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromSlide(this);
                    Log.Error("Could not move decoration to back in slideshow '" + message.Slideshow + "' slide:" + message.SlideNumber);
                    return;
                }

                if (mDecorations.Count == 1) return;

                RemoveDecoration(d);
           
                if (mDecorations.Count > 0)
                {
                    bool inserted = false;
                    CImageDecoration firstDecor = mDecorations[0] as CImageDecoration;
                    if (firstDecor != null)
                    {
                        if (firstDecor.IsBackgroundDecoration() == true)
                        {
                            mDecorations.Insert(1, d);
                            inserted = true;
                        }
                    }
                    if (inserted == false)
                    {
                        mDecorations.Insert(0, d);
                    }
                }
            }
		}

		
		//*******************************************************************
		public void BringToFront(CDecoration d)
		{
            RemoveDecoration(d);
            AddDecoration(d);
		}

		//*******************************************************************
		public virtual void RenderAllDecorations(Rectangle r, int frame_num)
		{
            RenderUnAttachedDecorations(r, frame_num, false, mFinalRenderSurface);
		}

        //*******************************************************************
        public override int GetMaxRequiredMotionBlurSubFrames()
        {
            int currentMax = ForcedMotionBlurSubFrames;

            ArrayList decorations = GetAllAndSubDecorations();
            foreach (CDecoration dec in decorations)
            {
                CAnimatedDecoration animDecor = dec as CAnimatedDecoration;
                if (animDecor != null)
                {
                    if (animDecor.MoveInEffect != null)
                    {
                        int inEffectMotionBlur = animDecor.MoveInEffect.RequiredMotionBlur;
                        if (inEffectMotionBlur > currentMax)
                        {
                            currentMax = inEffectMotionBlur;
                        }
                    }

                    if (animDecor.MoveOutEffect != null)
                    {
                        int outEffectMotionBlur = animDecor.MoveOutEffect.RequiredMotionBlur;
                        if (outEffectMotionBlur > currentMax)
                        {
                            currentMax = outEffectMotionBlur;
                        }
                    }
                }
            }

            if (this.mUsePanZoom == true)
            {
                int panZoomMB = PanZoom.GetRequiredMotionBlur();
                if (panZoomMB > currentMax)
                {
                    currentMax = panZoomMB;
                }
            }

            return currentMax;
        }

        //*******************************************************************
        // returns the number of "user" videos in this slide
        // i.e. a video which would be a users personal video
        public List<CVideoDecoration> GetUserVideos()
        {
            List<CVideoDecoration> list = new List<CVideoDecoration>();

            ArrayList decorsList = GetAllAndSubDecorations();
            foreach (CDecoration decor in decorsList)
            {
                CVideoDecoration vd = decor as CVideoDecoration;
                if (vd != null)
                {
                    if (vd.IsUserDecoration() == true)
                    {
                        list.Add(vd);
                    }
                }
            }

            return list;
        }

        //*******************************************************************
        public CImageDecoration GetDecorationForOriginalTemplateNumber(int templateNumber)
        {
            ArrayList list = GetAllAndSubDecorations();

            // has a decoration beeen set as use parent slide pan/zoom?
            foreach (CDecoration d in list)
            {
                CImageDecoration imageDecor = d as CImageDecoration;
                if (imageDecor != null)
                {
                    if (imageDecor.OriginalTemplateImageNumber == templateNumber)
                    {
                        return imageDecor;
                    }
                }
            }

            return null;
        }

        //*******************************************************************
        public CImageDecoration GetPanZoomAttachedDecor()
        {
            ArrayList list = GetAllAndSubDecorations();

            // has a decoration beeen set as use parent slide pan/zoom?
            foreach (CDecoration d in list)
            {
                CImageDecoration imageDecor = d as CImageDecoration;
                if (imageDecor != null)
                {
                    if (imageDecor.UseParentSlidePanZoomAsDefaultMovement == true)
                    {
                        return imageDecor;
                    }
                }
            }

            return null;
        }

        //*******************************************************************
        public virtual void ReRadomisePanZoom()
        {
            
        }


        //*******************************************************************
		protected void SaveImageSlidePart(XmlElement e, XmlDocument doc)
		{
			SaveSlidePart(e, doc);

            if (mPreviewPlayWhenEditingSlideMedia == false)
            {
                e.SetAttribute("PreviewPlayWhenEditMedia", mPreviewPlayWhenEditingSlideMedia.ToString());
            }
          
			foreach (CDecoration d in mDecorations)
			{
				d.Save(e, doc);
			}
		}


		
		//*******************************************************************
		protected void LoadImageSlidePart(XmlElement element)
		{
			LoadSlidePart(element);

            string s1 = element.GetAttribute("PreviewPlayWhenEditMedia");

            if (s1 != "")
            {
                mPreviewPlayWhenEditingSlideMedia = bool.Parse(s1);
            }

			XmlNodeList dec_list =element.GetElementsByTagName("Decoration");

			foreach (XmlElement d in  dec_list)
			{
                // As some decorations have sub decorations (GroupedDecorations)
                // Only load child Decorations
                if (d.ParentNode == element)
                {
                    CDecoration de = CDecoration.CreateFromType(d.GetAttribute("Type"));

                    try
                    {
                        de.Load(d);

                        // If loaded without exception, also check this is not a link to a download/encrypt file, if
                        // so check this is still valid
                        if (de.VerfifyAllMediaFilesToRenderThisExist() == true)
                        {
                            mDecorations.Add(de);
                        }
                    }
                    catch (MissingFileException mfe)
                    {
                        // photocruz hack! if we've not specified something then ignore it.
                        if (CGlobals.RunningPhotoCruz == true && mfe.mFilename.ToLower().StartsWith("image") == false && mfe.mFilename.ToLower().StartsWith("video") == false)
                        {
                            throw;
                        }
                    }
                    catch (IgnoreOperationException)
                    {
                        //
                        // If an IgnoreOperationException, then continue on to the next decoration. 
                        // This may have happened if a re-link occured and if the user selected 'remove' for this missing file.
                        //
                    }
                }
			}
		}

        public override CDecoration GetPostTransisionDecoration(int frame)
        {
            foreach (CDecoration d in mDecorations)
            {
                if (d.RenderPostTransition == true)
                {
                    return d;
                }
            }
            return null;
        }

        //*******************************************************************
        public bool DoesSlideContainSingleVideoOfSameLengthAsSlide()
        {
            List<CVideoDecoration> list = GetUserVideos();
            if (list.Count != 1) return false;

            if (list[0].MatchesSlideLength(DisplayLength) == true)
            {
                return true;
            }
            return false;
        }
	}
}
