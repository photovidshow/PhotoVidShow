using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml;
using System.Collections;
using ManagedCore;
using DVDSlideshow.GraphicsEng;


namespace DVDSlideshow
{

    // A slide that is really made out of other slides, played one after another. But
    // shown in the storyboard as one slide.
    //
    // This is created from a slide templates only, and can not be editied in any other way
    public class CMultiSlideSlide : CImageSlide
    {
        private List<CImageSlide> mSlides = new List<CImageSlide>();
        private CPanZoom mPanZoom = new CPanZoom();

        public override CPanZoom PanZoom
        {
            get { return mPanZoom; }
            set { }
        }

        public override string SourceFilename
        {
            get { return ""; }
        }

        public List<CImageSlide> SubSlides
        {
            get { return mSlides; }
            set { mSlides = value; }
        }
        

        //**************************************************************
        public CMultiSlideSlide()
        {
            this.mUsePanZoom = false;
        }

        //*******************************************************************
        // Create a blank slide given a clipart decoration, to show at max size
        public CMultiSlideSlide(List<CImageSlide> slides)
        {
            this.mUsePanZoom = false;
            mSlides = slides;
        }


        //*******************************************************************
        public override CSlide Clone()
        {
            return new CMultiSlideSlide(this);
        }


        //*******************************************************************
        public CMultiSlideSlide(CMultiSlideSlide copy)
            : base(copy)
        {
            Log.Error("CMultiSlideSlide copy, Not implemented");
        }


        //***************************************************************************************
        public override int GetMaxRequiredMotionBlurSubFrames()
        {
            int maxMB = 1;

            foreach (CImageSlide subSlide in mSlides)
            {
                int maxMbForSubSlide = subSlide.GetMaxRequiredMotionBlurSubFrames();
                if (maxMbForSubSlide > maxMB)
                {
                    maxMB = maxMbForSubSlide;
                }
            }

            return maxMB;
        }

        //***************************************************************************************
        public override int GetNumberRequiredMotionBlurSubFrames(int frame)
        {
            CImageSlide s = GetSlideForFrame(frame);
            if (s != null)
            {
                return s.GetNumberRequiredMotionBlurSubFrames(frame);
            }
            return 1;
        }

        //*******************************************************************
        // return the current slide which would be show given a frame number
        private CImageSlide GetSlideForFrame(int frame)
        {
            if (mSlides.Count == 0)
            {
                SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromSlide(this);
                Log.Error("MultiSlideSlide can't get slide for frame:" + frame + " as it contains no slides. In slideshow '" + message.Slideshow + "' slide:" + message.SlideNumber);
                return null;
            }

            int current_slide = 0;
            int current_total_frames = this.mStartFrameOffset;

            CSlide cs = (CSlide)mSlides[current_slide];

            while (frame > current_total_frames + cs.GetFrameLength())
            {
                // if we're on last slide it must be that
                if (current_slide >= mSlides.Count - 1)
                {
                  //  Console.WriteLine("Warning frame number " + frame + " exceeds slide show length returning last frame");
                    return cs as CImageSlide;
                }

                current_slide++;
                cs = (CSlide)mSlides[current_slide];
                current_total_frames = cs.mStartFrameOffset;

            }

            return (CSlide)mSlides[current_slide] as CImageSlide;
        }

        //*******************************************************************
        // return the next slide which would be show given a frame number
        private CImageSlide GetNextSlideForFrame(int frame)
        {
            CImageSlide cs = GetSlideForFrame(frame);
            return GetNextSlide(cs);
        }

        //*******************************************************************
        // return the next slide which would be show given the current slide
        private CImageSlide GetNextSlide(CImageSlide current_slide)
        {
            int i = 0;
            for (i = 0; i < mSlides.Count - 1; i++)
            {
                if (current_slide == (CSlide)mSlides[i])
                {
                    return (CSlide)mSlides[i + 1] as CImageSlide;
                }
            }
            return null;
        }

        //*******************************************************************
        private void MixSlides(CSlide current_slide, CSlide next_slide, int frame, int req_width, int req_height, bool ignore_pan_zoom, bool ignore_decors, float delta_with_next_slide)
        {
            if (next_slide == null)
            {
                current_slide.RenderFrame(frame, ignore_pan_zoom, ignore_decors, req_width, req_height);
                return;
            }

            CTransitionEffect effect = current_slide.TransistionEffect;

            // munge slides together
            effect.Process(delta_with_next_slide, current_slide, next_slide, frame, req_width, req_height, ignore_pan_zoom);

            CGlobals.DeclareEncodeCheckpoint('F');
        }
     
        //*******************************************************************
        public override void RenderFrame(int frame, bool ignore_pan_zoom, bool ignore_decors, int req_width, int req_height)
        {
            CImageSlide imageSlide = GetSlideForFrame(frame);
            CImageSlide nextSlide = GetNextSlideForFrame(frame);

            if (imageSlide != null)
            {
                float delta_with_next_slide = CSlideShow.CalcDeltaBetweenCurrentSlideAndNextSlide(imageSlide, nextSlide, frame);
                MixSlides(imageSlide, nextSlide, frame, req_width, req_height, ignore_pan_zoom, ignore_decors, delta_with_next_slide);
            }
        }

        //*******************************************************************
        public override void RenderUnAttachedDecorations(Rectangle r, int frame_num, bool postTrans, RenderSurface finalSurface)
        {
            CImageSlide imageSlide = GetSlideForFrame(frame_num);
            if (imageSlide != null)
            {
                imageSlide.RenderUnAttachedDecorations(r, frame_num, postTrans, finalSurface);
            }
        }

        //*******************************************************************
        public override ArrayList GetAllAndSubDecorations()
        {
            ArrayList list = new ArrayList();

            foreach (CImageSlide slide in mSlides)
            {
                list.AddRange(slide.GetAllAndSubDecorations());
            }

            return list;
        }

        //*******************************************************************
        public override List<CGroupedDecoration> GetAllGroupDecorations()
        {
            List<CGroupedDecoration> returnList = new List<CGroupedDecoration>();

            foreach (CImageSlide slide in mSlides)
            {
                List<CGroupedDecoration> slideGroupList = slide.GetAllGroupDecorations();

                returnList.AddRange(slideGroupList);
            }

            return returnList;
        }


        //*******************************************************************
        public override void ResetAllMediaStreams()
        {
            ArrayList decs =GetAllAndSubDecorations();
            foreach (CDecoration d in decs)
            {
                d.ResetAllMediaStreams();
            }
        }

        //*******************************************************************
        public override void StopAllNonPlayingMediaStreams(CSlide current_previewing_slide, CSlide next_preview_slide)
        {
            // ### SRG TODO, Basically it won't stop video decors in a sub slide, even if it's not current playing
            ArrayList decs = GetAllAndSubDecorations();
            foreach (CDecoration d in decs)
            {
                d.StopAllNonPlayingMediaStreams(current_previewing_slide, next_preview_slide);
            }
        }


        //*******************************************************************
        public override bool HasPreTransitionLayerDecorations(int frame)
        {
            CImageSlide imageSlide = this.GetSlideForFrame(frame);
            if (imageSlide != null)
            {
                return imageSlide.HasPreTransitionLayerDecorations(frame);
            }

            return false;
        }

        //*******************************************************************
        public override CDecoration GetPostTransisionDecoration(int frame)
        {
            CImageSlide imageSlide = this.GetSlideForFrame(frame);
            if (imageSlide != null)
            {
                return imageSlide.GetPostTransisionDecoration(frame);
            }
            return null;
        }

        //*******************************************************************
        public override int CalcLengthInFrame(int offset)
        {
            int originalOffset = offset;

            foreach (CImageSlide slide in mSlides)
            {
                offset = slide.CalcLengthInFrame(offset);
            }

            return base.CalcLengthInFrame(originalOffset);
        }

        //*************************************************************************************************
        // decoration A must exist in slide, decoration B must not
        public override void RepaceDecoration(CDecoration decorA, CDecoration withDecorB)
        {
            foreach (CImageSlide slide in mSlides)
            {
                ArrayList decs = slide.GetAllAndSubDecorations();
                if (decs.Contains(decorA) == true)
                {
                    slide.RepaceDecoration(decorA, withDecorB);
                    return;
                }
            }
            SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromSlide(this);
            Log.Error("Did not replace decoration in CMultiSlideSlide in slideshow '" + message.Slideshow + "' slide:" + message.SlideNumber);
        }


        //*******************************************************************
        public override void DeclareSlideAspectChange(CGlobals.OutputAspectRatio oldAspect, CGlobals.OutputAspectRatio newAspect)
        {
            foreach (CImageSlide slide in mSlides)
            {
                slide.DeclareSlideAspectChange(oldAspect, newAspect);
            }
        }


        //*************************************************************************************************
        // basically CVideoDecoration with video links need to have there start/times set such that seeking does
        // something sensible.  Currently this can only happen with CMultiSlideSLide
        public override void ReCalcAllVideoLinkStartEndTimes()
        {
            List<CVideoDecoration> foundVideoDecs = new List<CVideoDecoration>();
            List<float> foundVideoDecsNextTime = new List<float>();

            foreach (CImageSlide imageSlide in mSlides)
            {
                ArrayList list = imageSlide.GetAllAndSubDecorations();
                foreach (CDecoration d in list)
                {
                    CVideoDecoration vd = d as CVideoDecoration;
                    if (vd != null)
                    {
                        if (vd.StringID != "")
                        {
                            foundVideoDecs.Add(vd);
                            foundVideoDecsNextTime.Add((float)vd.StartVideoOffset + vd.GetLengthTimeShownFor(imageSlide.DisplayLength) - (imageSlide.TransistionEffect.Length));
                        }
                        else
                        {
                            for (int index = 0; index < foundVideoDecs.Count; index++)
                            {
                                if (foundVideoDecs[index].StringID == vd.ReferenceStringId)
                                {
                                    vd.StartVideoOffset = foundVideoDecsNextTime[index];
                                    vd.EndVideoOffset = 0;
                                    foundVideoDecsNextTime[index] += (vd.GetLengthTimeShownFor(imageSlide.DisplayLength) - (imageSlide.TransistionEffect.Length));
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }


       //*************************************************************************************************
        // both decorations must already exist in slide
        public override void SwapDecorations(CDecoration decorA, CDecoration decorB)
        {
            foreach (CImageSlide slide in mSlides)
            {
                ArrayList decs = slide.GetAllAndSubDecorations();
                if (decs.Contains(decorA) == true)
                {
                    slide.SwapDecorations(decorA, decorB);
                    return;
                }
            }

            SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromSlide(this);
            Log.Error("Did not swap decoration in CMultiSlideSlide in slideshow '" + message.Slideshow + "' slide:" + message.SlideNumber);
        }

        //*******************************************************************
        public override void AddDecoration(CDecoration decoration)
        {
            if (mSlides.Count > 0)
            {
                mSlides[0].AddDecoration(decoration);
            }
        }

        //*******************************************************************
        // Returns true if any containing slide contains a video decor which is a link to another video decor
        // (This may be useful if so, becuase splitting this multi slide slide would not be allowed for example)
        public bool ContainsVideoReferenceLinks()
        {
            ArrayList list = GetAllAndSubDecorations();
            foreach (CDecoration d in list)
            {
                CVideoDecoration videoDecor = d as CVideoDecoration;
                if (videoDecor != null)
                {
                    if (videoDecor.ReferenceStringId != "")
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc)
        {
            XmlElement slide = doc.CreateElement("Slide");

            slide.SetAttribute("Type", "MultiSlideSlide");

            SaveImageSlidePart(slide, doc);

            foreach (CImageSlide imageSlide in mSlides)
            {
                imageSlide.Save(slide, doc);
            }

            parent.AppendChild(slide);
        }

      
        //*******************************************************************
        public override void Load(XmlElement element)
        {
            LoadImageSlidePart(element);

            XmlNodeList list = element.GetElementsByTagName("Slide");

            foreach (XmlElement e in list)
            {
                CImageSlide slide = CSlide.CreateFromType(e.GetAttribute("Type")) as CImageSlide;

                if (slide == null) continue;

                try
                {
                    slide.Load(e);
                    mSlides.Add(slide);                
                }
                catch (ErrorException exception)
                {
                    CDebugLog.GetInstance().Error("Failed to load slide in CMultiSlideSlide. Ignoring :" +exception.Message);
                    if (CGlobals.RunningPhotoCruz == true)
                    {
                        throw;
                    }
                }
                catch (ManagedCore.MissingFileException)
                {
                    if (CGlobals.RunningPhotoCruz == true)
                    {
                        throw;
                    }
                }
                catch (IgnoreOperationException)
                {
                }
            }

            //
            // If we failed to load any slides, then throw an IgnoreOperationException 
            // This then tells the load process not to add this slide to the slideshow.
            //
            if (mSlides.Count == 0)
            {
                throw new IgnoreOperationException();
            }
        }

        //*******************************************************************
        public override bool AllowedToBeEditited()
        {
            // currently do not allow the editting of a CMultiSlideSLide
            return false;
        }
    }
}
