using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml;
using ManagedCore;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
    class CMenuButtonInnerVideoRenderer
    {

        private CVideo mInnerVideo;
        private ActiveReader<RenderSurface> mCachedEditModeImage = new ActiveReader<RenderSurface>();
        private CImage mMask;

        public CImage Mask
        {
            get { return mMask; }
            set { mMask = value; }
        }

        //*******************************************************************
        public CMenuButtonInnerVideoRenderer(CVideo inner_video)
        {
            mInnerVideo = inner_video;
        }

        //*******************************************************************
        // ### SRG TODO NEEDS TO DISPOSE RENDERSURFACE better than this
        ~CMenuButtonInnerVideoRenderer()
        {
            if (mCachedEditModeImage.IsNull() == false)
            {
                mCachedEditModeImage.Dispose();
            }
            mCachedEditModeImage = null;
        }

        //*******************************************************************
        public void ResetAllMediaStreams()
        {
            if (mInnerVideo == null) return;

            CSlideShow ss = mInnerVideo as CSlideShow;
            if (ss == null) return;
            ss.ResetAllMediaStreams();
        }

        //*******************************************************************
        public void Render(Rectangle r1, int frame_num, bool playing_thumbnails, bool ignore_inner_decors, float transparency)
        {
			if (mInnerVideo!=null)
			{
                GraphicsEngine engine = GraphicsEngine.Current;
			
                bool reGeneratedCachedImage = true;

				// used cached image if we are in editor or we do not need moving thumbnails
				if ((CGlobals.PlayVideoRealTime== true || playing_thumbnails==false) &&
					mCachedEditModeImage!=null && 
					CGlobals.MainMenuNeedsReRender==false)
				{
					reGeneratedCachedImage = false;
				}

                if (mCachedEditModeImage.IsNull() == true  || reGeneratedCachedImage == true)
                {

                    //
                    // Make sure sound is muted
                    //
                    bool mute_sound = CGlobals.MuteSound;
                    CGlobals.MuteSound = true;

                    bool ignore_pan_zoom = true;

                    if (CGlobals.mVideoGenerateMode == VideoGenerateMode.FINAL_OUTPUT_MODE)
                    {
                        ignore_pan_zoom = false;
                    }

                    int two_seconds = (2 * ((int)CGlobals.mCurrentProject.DiskPreferences.frames_per_second) );
                   
                    int the_frame = frame_num + two_seconds;

                    if (playing_thumbnails == false)
                    {
                        the_frame = two_seconds;
                        ignore_pan_zoom = true;
                    }

                    CSlideShow ss = mInnerVideo as CSlideShow;
                    if (ss != null)
                    {
                        ss.CalcLengthOfAllSlides(); // ### hack incase slideshow was encoded a different fps for motion blue

                        if (ss.MenuThumbnailSlide != null)
                        {
                            CSlide prev = ss.GetPreviousSlide(ss.MenuThumbnailSlide);
                            if (prev == null)
                            {
                                // should never happen
                                the_frame += ss.MenuThumbnailSlide.mStartFrameOffset;
                            }
                            else
                            {
                                the_frame += ss.MenuThumbnailSlide.mStartFrameOffset + prev.TransistionEffect.GetTotalRenderLengthInFrames() + 1 - two_seconds;
                            }
                        }
                    }

                    mCachedEditModeImage.Dispose();

                    //  Make button bigger when rendering then use mipmapping when re-rendering button at actual size
                    int textureWidth = r1.Width * 2;
                    int textureHeight = r1.Height * 2;

                    // just in case it's too big, reduce size
                    while (textureWidth > CGlobals.mCurrentProject.DiskPreferences.OutputVideoWidth && textureWidth > 1)
                    {
                        textureWidth /= 2;
                        textureHeight /= 2;
                    }

                    if (textureWidth <= 0) textureWidth = 1;
                    if (textureHeight <= 0) textureHeight = 1;

                   
                    mCachedEditModeImage = engine.GeneratePersistentRenderSurface((uint)textureWidth, (uint)textureHeight, true, this.ToString() + "::Render");

                    RenderSurface current = engine.GetRenderTarget();
                    try
                    {
                        engine.SetRenderTarget(mCachedEditModeImage.Object);
                        //
                        // Bug in radeon r7 drivers, clear method not always working with reused RT surfaces with mipmapping.  Using an alternative method
                        //
                        engine.ClearRenderTargetByBlankTexture();
                        // engine.ClearRenderTarget(0, 0, 0, 255);

                        RenderVideoParameters parameters = new RenderVideoParameters();
                        parameters.frame = the_frame;
                        parameters.ignore_pan_zoom = ignore_pan_zoom;
                        parameters.ignore_decorations = ignore_inner_decors;
                        parameters.req_width = textureWidth;
                        parameters.req_height = textureHeight;
                        parameters.present_direct_to_window = false;
                        parameters.do_begin_and_end_scene = false;

                        mInnerVideo.RenderVideo(parameters);               
                    }
                    finally
                    {
                        engine.SetRenderTarget(current);
                    }

                    CGlobals.MuteSound = mute_sound;
                }

				// Draw the base image

                bool alpha = false;

                if (mMask != null)
                {
                    PixelShaderEffect mask = PixelShaderEffectDatabase.GetInstance().GetPixelShader("alphamapNoImageAlpha");
                    if (mask == null)
                    {
                        Log.Error("Could not find 'alphamapNoImageAlpha' pixel shader");
                    }
                    else
                    {
                        mask.SetInputImage1(mMask);
                        engine.SetPixelShaderEffect(mask);
                        alpha = true;
                    }
                }

                engine.DrawImageWithDiffuseAlpha(mCachedEditModeImage.Object, 1-transparency, new RectangleF(0, 0, 1, 1), r1, alpha);
                engine.SetPixelShaderEffect(null);

		    }
        }
    }
}
