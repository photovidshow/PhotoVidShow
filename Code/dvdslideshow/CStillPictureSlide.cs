using System;
using System.Collections;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using ManagedCore;
using DVDSlideshow.GraphicsEng;
using System.Text;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for CStillPictureSlide.
	/// </summary>
	public class CStillPictureSlide : CImageSlide
	{
		private Object thislock = new Object();
        protected CImage mImage;             // legacy used on old vanilla CStillPictureSlide slides ( v3.1.4 and below)

        public CImage Image
        {
            get { return mImage; }
        }

		protected CPanZoom		mPanZoom;

        private bool mRenderHighQuality = false;
		
		public override CPanZoom PanZoom
		{
			get { return mPanZoom ; }
            set { mPanZoom = value; }
		}

		public override string SourceFilename
		{
			get
			{
                return mImage.ImageFilename;
			}
		}


        //*******************************************************************
        public override CImage BackgroundImage
        {
            set 
            { 
                mBackgroundImage = value;
                if (mImage != null)
                {
                    mImage.BackgroundImage = value;
                }
            }
        }

		//*******************************************************************
		public CStillPictureSlide()
		{
			mPanZoom = new CPanZoom();
		}

		//*******************************************************************
		public CStillPictureSlide(string file)
		{
			mImage = new CImage(file);
			mPanZoom = new CPanZoom();

			Init();
			mPanZoom.CalcBestRectangleAreas(mImage);

		
		}

		//*******************************************************************
		public CStillPictureSlide(CImage image)
		{
			mImage = image;
			mPanZoom = new CPanZoom();

			Init();
			mPanZoom.CalcBestRectangleAreas(mImage);
		}


		//*******************************************************************
		static public CStillPictureSlide GenerateTestCard()
		{
			int w = CGlobals.mCurrentProject.DiskPreferences.CanvasWidth;
			int h= CGlobals.mCurrentProject.DiskPreferences.CanvasHeight;
			Bitmap bitmap = new Bitmap(w,
								  h,
								  System.Drawing.Imaging.PixelFormat.Format32bppRgb);

			Graphics g = Graphics.FromImage(bitmap);
		
			Pen white_pen = new Pen(Color.White);

			Brush bb = new SolidBrush(Color.Black);
			Brush wb = new SolidBrush(Color.White);

			g.FillRectangle(bb,0,0,w,h);

			Font font = new Font("Ariel",w/50);
		
			SolidBrush br = new SolidBrush(Color.White);

			for (int i=0;i<50;i+=5)
			{
				int xx = (int) ((((float)w) / 100.0f) * i);
				int xw = w - xx*2;

				int yy = (int) ((((float)h) / 100.0f) * i);
				int yw = h - yy*2;

				g.DrawRectangle(white_pen,xx,yy,xw,yw);

				g.DrawString(i.ToString(),font,wb,xx,h/2);
				g.DrawString(i.ToString(),font,wb,w-xx-10,h/2);
				g.DrawString(i.ToString(),font,wb,w/2,yy);
				g.DrawString(i.ToString(),font,wb,w/2,h-yy-10);


			}

			g.Dispose();

			CImage i1 = new CImage(bitmap);

			bitmap.Save("c:\\testcard.bmp");
			return new CStillPictureSlide(i1);
		}


		//*******************************************************************
		public CStillPictureSlide(string file, bool have_zoom)
		{
			mImage = new CImage(file);
			mPanZoom = new CPanZoom();

			this.UsePanZoom = have_zoom;

			Init();
			if (UsePanZoom==true)
			{
				mPanZoom.CalcBestRectangleAreas(mImage);
			}
		}

		
		//*******************************************************************
		public override CSlide Clone()
		{
			return new CStillPictureSlide(this);
		}


		//*******************************************************************
		public CStillPictureSlide (CStillPictureSlide copy) : base(copy)
		{
			mImage=null;
			if (copy.mImage!=null) 
			{
				mImage = new CImage(copy.mImage);
			}
			else
			{
                SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromSlide(this);
				ManagedCore.CDebugLog.GetInstance().Error("Image inside slide "+ message.SlideNumber+" for slideshow '"+message.Slideshow+"' was NULL on call to copy");
			}

			mPanZoom = copy.mPanZoom.Clone();
		}

		//*******************************************************************
		private void Init()
		{
			//mImageWithDecorations = mImage.Clone();
		}

		//*******************************************************************
		public override void Rotate(int amount)
		{
			base.Rotate(amount);

            RotateFlipType type = GetRotationFlipType();

            float zoom = 1.0f;
            // make sure we have bigger enough image to take in account that we still
            // want max resolutoon when zoomed in

            if (CGlobals.mVideoGenerateMode != VideoGenerateMode.EDIT_PRIVIEW_MODE)
            {
                zoom = this.mPanZoom.GetMaxZoom();
            }

            bool bo_ba = false;
            if (this.mImage != null &&
                this.mImage.NoBlankArea == true)
            {
                bo_ba = true;
            }

            bool resize_to_dvd = mImage.DrawImageWithAspect == CImage.DrawImageWithAspectType.KeepAspectByAddingPillarBox;

            mImage = new CImage(mImage.ImageFilename, resize_to_dvd, type, zoom);
            mImage.BackgroundImage = mBackgroundImage;
            mImage.NoBlankArea = bo_ba;

			mPanZoom = new CPanZoom();
			if (UsePanZoom==true)
			{
				mPanZoom.CalcBestRectangleAreas(mImage);
			}
		}
		

		//*******************************************************************
		// This generates the slide image with any attached decorations in it.
        // If a RenderSurface is returned, this MUST be disposed of this render frame
        //
        // Post v.1.5 
        //
        // We now only return a CImage as nothing can be attached anymore
		private DrawableObject GetImageWithAttachedDecors()
		{
            return mImage;
		}

        //*******************************************************************
        // private method, only call this when you know what you're doing
        private void GroupAll()
        {
            // should never happen, but just in case
            CGroupedDecoration gd = this.GetFirstGroupedDecoration();
            if (gd != null)
            {
                SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromSlide(this);
                Log.Warning("Can not group slide, already grouped in slideshow '" + message.Slideshow + "' slide:" + message.SlideNumber);
                return;
            }

            CGroupedDecoration group = new CGroupedDecoration(new RectangleF(0, 0, 1, 1), 0);

            foreach (CDecoration d in mDecorations)
            {
                if (d.IsGroupableDecoration() == true)
                {
                    group.GroupedDecorations.Add(d);
                }
            }
      
            foreach (CDecoration d in group.GroupedDecorations)
            {
                mDecorations.Remove(d);
            }

            // make sure we add decoration in correct place
            this.AddDecoration(group);
        }

        //*******************************************************************
        // private method, only call this when you know what you're doing
        private void UnGroupAll()
        {
            CGroupedDecoration group = GetFirstGroupedDecoration();
            if (group == null) return;

            foreach (CDecoration d in group.GroupedDecorations)
            {
                mDecorations.Add(d);
            }

            mDecorations.Remove(group);
        }

        //*******************************************************************
        private void CheckIfNeedToGroupOrUngroupAllDueToPanZoom()
        {
            // num group decorations       
            int nongroupdecors = GetNumGroupableDecorations();

            CGroupedDecoration group = GetFirstGroupedDecoration();
            if (group != null)
            {
                // if only one top decoration (i.e. the group and it has no movement)
                if (nongroupdecors <= 1 && group.MoveInEffect == null && group.MoveOutEffect == null)
                {
                    // only one decoration in the group
                    if (group.GroupedDecorations.Count <= 1)
                    {
                        bool doUngroup = true;

                        if (group.GroupedDecorations.Count == 1 && mUsePanZoom == true)
                        {
                            CAnimatedDecoration dd = group.GroupedDecorations[0] as CAnimatedDecoration;

                            if (dd != null)
                            {
                                if (dd.MoveInEffect != null || dd.MoveOutEffect != null)
                                {
                                    doUngroup = false;
                                }
                            }
                        }

                        if (doUngroup == true)
                        {
                            Log.Info("Ungrouping static group, because it only has one image decoration");
                            UnGroupAll();
                        }
                        return;
                    }
                }

                if ((mPanZoom.PanZoomOnAll == false || mUsePanZoom == false) && 
                    group.MoveInEffect == null && group.MoveOutEffect == null)
                {

                    if (mUsePanZoom == false)
                    {
                        Log.Info("Ungrouping group, because group has no movement and pan zoom turned off");
                        UnGroupAll();
                        return;
                    }

                    // ok if image decoration has no motion, ungroup
                    foreach (CDecoration d in this.GetAllAndSubDecorations())
                    {
                        CImageDecoration imageDecor = d as CImageDecoration;
                        if (imageDecor != null)
                        {
                            if (imageDecor.UseParentSlidePanZoomAsDefaultMovement == true)
                            {
                                if (imageDecor.MoveInEffect == null && imageDecor.MoveOutEffect == null)
                                {
                                    Log.Info("Ungrouping group, because group has no movement and image decoration has no motion");
                                    UnGroupAll();
                                }
                                break;
                            }
                        }
                    }

                    if (group.UseParentSlidePanZoomAsDefaultMovement == true && mPanZoom.PanZoomOnAll == false )
                    {
                       // if no image/video sub decorations have movement unlink
                        bool foundMovement = false;
                        foreach (CDecoration d in this.GetAllAndSubDecorations())
                        {
                            CImageDecoration imageDecor = d as CImageDecoration;
                            if (imageDecor !=null && (imageDecor is CTextDecoration == false))
                            {
                                if (imageDecor.MoveOutEffect !=null ||
                                    imageDecor.MoveInEffect!=null)
                                {
                                    foundMovement = true;
                                    break;
                                }
                            }
                        }
                        if (foundMovement == false)
                        {
                            Log.Info("Ungrouping group, because pan zoom on all turned off and no sub decors have movement");
             
                            UnGroupAll();
                            EnsureCorrectDecorationIsSetToMoveAsPanZoom();  // need to re-select image to apply pan/zoom too
                        }
                    }

                    return;
                }
            }
            else
            {
                if (ShouldGroupAll(nongroupdecors, true) == true)
                {
                    GroupAll();
                }
            }
        }

        //*******************************************************************
        private bool ShouldGroupAll(int nongroupdecors, bool showLog)
        {
            if (mUsePanZoom == true)
            {
                if (mPanZoom.PanZoomOnAll == true && nongroupdecors > 1)
                {
                    if (showLog == true)
                    {
                        Log.Info("Grouping all, because pan zoom on all selected and there are multiple decorations");
                    }

                    return true;
                }
                else
                {

                    foreach (CDecoration d in this.GetAllAndSubDecorations())
                    {
                        CImageDecoration imageDecor = d as CImageDecoration;
                        if (imageDecor != null)
                        {
                            if (imageDecor.UseParentSlidePanZoomAsDefaultMovement == true)
                            {
                                if (imageDecor.MoveInEffect != null || imageDecor.MoveOutEffect != null)
                                {
                                    if (showLog == true)
                                    {
                                        Log.Info("Grouping all, because pan zoom on and image decoration has motion set");
                                    }
                                    return true;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            return false;
        }


        //*******************************************************************
        private int GetNumGroupableDecorations()
        {
            int count = 0;
            ArrayList list = GetAllAndSubDecorations();
            foreach (CDecoration d in list)
            {
                if (d.IsGroupableDecoration() == true)
                {
                    count++;
                }
            }
            return count;
        }

        //*******************************************************************
        private void IfNotGroupEnsureCorrectDecorationIsSetToMoveAsPanZoom()
        {
            // ok pan zoom is selected, we should have already by now added group or not, so basically if not a group
            // mark the first "user" image else mark the group 

            CGroupedDecoration group = GetFirstGroupedDecoration();
            if (group != null)
            {
                // If using pan/zooom remove and in/out effects (can't have both)

                if (mUsePanZoom == true)
                {
                    group.MoveInEffect = null;
                    group.MoveOutEffect = null;
                    group.UseParentSlidePanZoomAsDefaultMovement = true;
                }

                foreach (CDecoration d in group.GroupedDecorations)
                {
                    CImageDecoration imageDecor = d as CImageDecoration;
                    if (imageDecor != null)
                    {
                        imageDecor.UseParentSlidePanZoomAsDefaultMovement = false;
                    }
                }
            }
            else
            {
                EnsureCorrectDecorationIsSetToMoveAsPanZoom();
            }
        }

        //*******************************************************************
        private void EnsureCorrectDecorationIsSetToMoveAsPanZoom()
        {
            bool found = GetPanZoomAttachedDecor() != null;

            ArrayList list = GetAllAndSubDecorations();

            // if not set first valid one
            if (found == false)
            {
                foreach (CDecoration d in list)
                {
                    CImageDecoration imageDecor = d as CImageDecoration;
                    if (imageDecor != null)
                    {
                        if (found == false &&
                            imageDecor.IsBorderDecoration() == false &&
                            imageDecor.IsBackgroundDecoration() == false &&
                            imageDecor.IsFilter() == false &&
                            imageDecor.MoveInEffect == null &&
                            imageDecor.MoveOutEffect == null &&
                            (imageDecor is CClipArtDecoration || imageDecor is CVideoDecoration))
                        {
                            imageDecor.UseParentSlidePanZoomAsDefaultMovement = true;
                            found = true;
                        }
                        else
                        {
                            imageDecor.UseParentSlidePanZoomAsDefaultMovement = false;
                        }
                    }
                }
            }
        }

        //*******************************************************************
        public override void RenderFrame(int frame, bool ignore_pan_zoom, bool ignore_decors, int req_width, int req_height)
        {
            // ok new pan zoom method / i.e. movement done on decoration or group. 
            // We may need to create/remove a group.  If so do it now before we work out if we need layers etc 
            // (i.e. we may be moving a layered decoration)

            if (mImage == null && ignore_pan_zoom == false)
            {
                CheckIfNeedToGroupOrUngroupAllDueToPanZoom();

                if (mUsePanZoom == true)
                {
                    IfNotGroupEnsureCorrectDecorationIsSetToMoveAsPanZoom();
                }
            }

            base.RenderFrame(frame, ignore_pan_zoom, ignore_decors, req_width, req_height);
        }

		//*******************************************************************
		protected override CImage GetVideo(int frame, bool ignore_pan_zoom, bool ignore_decors, int req_width, int req_height, CImage render_to_this)
		{
			CGlobals.DeclareEncodeCheckpoint('G');

			lock(thislock)
			{
                try
                {
                    RectangleF destRectangle = new RectangleF(0, 0, req_width, req_height);
                    DrawableObject i = mImage;

                    int iwidth = 1;
                    int iheight = 1;

                    //
                    // Legacy, for blank still picture slide i will be null
                    //
                    if (i != null)  // maybe null e.g. if this is a blank slide
                    {
                        try
                        {
                            if (ignore_decors == true)
                            {
                                i = mImage;
                            }
                            else
                            {
                                i = GetImageWithAttachedDecors();
                            }

                            RectangleF srcRectangle = new RectangleF(0, 0, 1, 1);

                            if (GetFrameLength() > 0 && mUsePanZoom == true && ignore_pan_zoom == false)
                            {
                                int a = CalcEndFrame() - frame;
                                if (a < 0)
                                {
                                    a = 0;
                                }

                                float delta = 1.0f - (((float)a) / ((float)this.GetFrameLength()));

                                if (delta > 1.0f)
                                {
                                    delta = 1.0f;
                                }

                                float rotation = 0;
                                srcRectangle = mPanZoom.Process(delta, iwidth, iheight, ref rotation);
                            }

                            srcRectangle.Width += srcRectangle.X;
                            srcRectangle.Height += srcRectangle.Y;

                            GraphicsEngine.Current.DrawImage(i, srcRectangle, destRectangle, false);
                        }
                        finally
                        {
                            if (i != null && i is RenderSurface)
                            {
                                RenderSurface rs = i as RenderSurface;
                                rs.Dispose();
                            }
                        }
                    }

                    if (ignore_decors == false)
                    {
                        RenderUnAttachedDecorations(new Rectangle(0, 0, req_width, req_height), frame, false, mFinalRenderSurface);
                    }

                    CGlobals.DeclareEncodeCheckpoint('I');
                    return null;
                }
                catch (Exception e)
                {
                    HandleGetVideoException(e, frame);
                }
                
			}
			CGlobals.DeclareEncodeCheckpoint('J');
			return null;
		}

        //*******************************************************************
        private void HandleGetVideoException(Exception e, int frame)
        {
            CGlobals.DeclareEncodeCheckpoint('*');
            GC.Collect();

            //
            // Generate addition info to tell user what slide and slideshow the problem was in
            //
            SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromSlide(this);
         
            string additionInfo = "Problem was in slide:" + message.SlideNumber + " in slideshow '" + message.Slideshow + "'";

            CDebugLog.GetInstance().Error("Exception thrown in CStillPictureSlide::GetVideo, "+ additionInfo + " frame:" + frame + " :" + e.ToString() );
        }

		//*******************************************************************
		public void RenderAttachedDecorations(Rectangle r, int frame)
		{
			// move all current deocrations down a layer
			foreach (CDecoration d in mDecorations)
			{
				if (d.AttachedToSlideImage==true)
				{
					d.RenderToGraphics(r, frame, this, null);
				}
			}
		}

		//*******************************************************************
		public override void RenderAllDecorations(Rectangle r, int frame_num)
		{
			RenderAttachedDecorations(r,frame_num);
			//RenderUnAttachedDecorations( r,frame_num, false, mFinalRenderSurface);
		}
		
		//*******************************************************************
		public override void Save(XmlElement parent, XmlDocument doc)
		{
			XmlElement slide = doc.CreateElement("Slide");

			slide.SetAttribute("Type","StillPictureSlide");

            if (mRenderHighQuality == true)
            {
                slide.SetAttribute("RenderHighQuality", mRenderHighQuality.ToString());
            }

			SaveImageSlidePart(slide, doc);

            mImage.Save(slide, doc, "Image");
		
			mPanZoom.Save(slide,doc);

			parent.AppendChild(slide); 
		}

		//*******************************************************************
		public override void Load(XmlElement element)
		{
			LoadImageSlidePart(element);

            string s1 = element.GetAttribute("RenderHighQuality");

            if (s1 != "")
            {
                mRenderHighQuality = bool.Parse(s1);
            }

			XmlElement element2= (XmlElement)element.FirstChild;

			// hack to skip over decorations images
			while (element2.Name != "Image")
			{
				element2 = (XmlElement) element2.NextSibling;
			}

			mImage = new CImage();

			mImage.RotateFlipType = GetRotationFlipType();

			try
			{
				mImage.Load(element2 as XmlElement, true);
			}
			// if a image cant ignore just give it a blank image !!
			catch (IgnoreOperationException ioe)
			{
				if (this.PartOfAMenu!=true)
				{
					throw ioe;
				}
			}

            s1 = element.GetAttribute("BackgroundImage");

            if (s1 != "")
            {
                // PhotoCruz SRG TO DO FIX ME
                if (s1 != "default" && s1.ToLower() !="image1")
                {
                    this.BackgroundImage = new CImage(s1, false);
                }
            }

			XmlNodeList e1=element.GetElementsByTagName("PanZoom");
			if (e1!=null && e1.Count >0 )
			{
				mPanZoom.Load(e1[0] as XmlElement);
			}
			else if (mUsePanZoom==true)
			{
                SlideshowAndSlideIdentifier message = SlideshowAndSlideIdentifier.FromSlide(this);
                Log.Warning("No pan zoom defined on picture slide, setting it in slideshow '" + message.Slideshow + "' slide:" + message.SlideNumber);
			    mPanZoom.CalcBestRectangleAreas(mImage);
			}


			Init();
		}

		//*******************************************************************
		public override string GetSourceFileName()
		{
            return mImage.ImageFilename;
		}

        //*******************************************************************
        public override void ReRadomisePanZoom()
        {
            ReRadomisePanZoom(null);
        }

        //*******************************************************************
        // Allows is to define to image to base out pan/zoom calculations on
        // This passed in parameter is used by PhotoCruz
        public void ReRadomisePanZoom(CImage originalMainImage)
        {
            CImage forImage = mImage;

            if (originalMainImage !=null)
            {
                forImage = originalMainImage;
            }

            // ok if forImage null then probably a blank slide, if so find first decoration that has been marked to use
            // parent slide pan zoom effect
            if (forImage == null)
            {
                CImageDecoration id = GetPanZoomAttachedDecor();
                if (id != null)
                {
                    CClipArtDecoration cad = id as CClipArtDecoration;
                    if (cad != null)
                    {
                        forImage = cad.mImage;
                    }
                }
            }

            // forImage may still be null, which is fine, the calculation will just ignore it      
            mPanZoom.EndRotation = 0;
            mPanZoom.StartRotation = 0;
            mPanZoom.InitialDelay = 0.0f;
            mPanZoom.EndDelay = 1.0f;
            mPanZoom.MovementEquation = new CLinear();
            mPanZoom.CalcBestRectangleAreas(forImage, true);
            CGlobals.mCurrentProject.DeclareChange("Re-Randimize PanZoom for Slide");

        }

		//*******************************************************************
		public override void RebuildToNewCanvas(CGlobals.OutputAspectRatio ratio)
        {

		}
	}
}
