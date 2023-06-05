/*
 * Created by SharpDevelop.
 * User: user
 * Date: 03/01/2005
 * Time: 13:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using DVDSlideshow.GraphicsEng;
using ManagedCore;

namespace DVDSlideshow
{
	/// <summary>
	/// Description of CVideo.	
	/// </summary>
	public abstract class CVideo
	{
		public CVideo()
		{
		}

        public abstract int GetNumberRequiredMotionBlurSubFrames(int frame);
        public abstract int GetMaxNumberRequiredMotionBlurSubFrames();

        private static CTextDecoration mWaterMarkTextDecor = null;

        //*********************************************************************************************************************
        public Bitmap RenderVideoToBitmap(RenderVideoParameters renderVideoParameters)
        {
            GraphicsEngine engine = GraphicsEngine.Current;
            renderVideoParameters.present_direct_to_window = false;
            renderVideoParameters.do_begin_and_end_scene = false;

            Bitmap b = null;
            if (renderVideoParameters.renderToBitmap != null)
            {
                //
                // Resuse proved bitmap if the size matches
                //
                if (renderVideoParameters.renderToBitmap.Width == renderVideoParameters.req_width &&
                    renderVideoParameters.renderToBitmap.Height == renderVideoParameters.req_height)
                {
                    b = renderVideoParameters.renderToBitmap;
                }
            }
            
            if (b==null)
            {
                b = new Bitmap(renderVideoParameters.req_width, renderVideoParameters.req_height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
  
            RenderSurface currentSurface = engine.GetRenderTarget();
            if (engine.BeginScene(null) == GraphicsEngine.State.OK)
            {
                try
                {
                    using (RenderSurface tempSurface = engine.GenerateRenderSurface((uint)renderVideoParameters.req_width, (uint)renderVideoParameters.req_height, this.ToString() + "::RenderVideoToBitmap"))
                    {
                        engine.SetRenderTarget(tempSurface);
                        RenderVideo(renderVideoParameters);
                        CImage image = new CImage(b);
                        engine.CopyDefaultSurfaceToImage(image);
                        //  b.Save("C:\\temp.bmp");
                    }
                }
                finally
                {
                    engine.SetRenderTarget(currentSurface);
                    engine.EndScene();
                }
            }
       
            return b;
        }


        //*********************************************************************************************************************
        public GraphicsEngine.State RenderVideo(RenderVideoParameters renderVideoParameters)
        {
            GraphicsEngine engine = GraphicsEngine.Current;

            if (engine == null) return GraphicsEngine.State.NO_ENGINE_DEFINED;

            GraphicsEngine.State state = GraphicsEngine.State.OK;
         
            BeginSceneParameters parameters = new BeginSceneParameters();
            parameters.surface = renderVideoParameters.render_to_surface;
            parameters.present_window = renderVideoParameters.present_window;
            parameters.width = renderVideoParameters.req_width;
            parameters.height = renderVideoParameters.req_height;

            if (renderVideoParameters.do_begin_and_end_scene)
            {
                state = engine.BeginScene(parameters);
            }

            // State not ok, end early
            if (state != GraphicsEngine.State.OK)
            {
                return state;
            }

            try
            {

                if (renderVideoParameters.preRenderDelegate != null)
                {
                    renderVideoParameters.preRenderDelegate(renderVideoParameters);
                }

                GetVideo(engine, renderVideoParameters);

                if (ManagedCore.License.License.Valid == false &&
                   CGlobals.mVideoGenerateMode == VideoGenerateMode.FINAL_OUTPUT_MODE)
                {
                    RenderWaterMark(renderVideoParameters);
                }

                if (renderVideoParameters.postRenderDelegate != null)
                {
                    renderVideoParameters.postRenderDelegate(renderVideoParameters);
                }

            }
            finally
            {
                if (renderVideoParameters.do_begin_and_end_scene)
                {
                    engine.EndScene();
                }
            }

            if (renderVideoParameters.present_direct_to_window)
            {
                state = engine.PresentToWindow(renderVideoParameters.present_window, renderVideoParameters.scale_back_buffer_to_present_window);  
            }
            
            return state;
        }


        // *************************************************************************************************8
        private void RenderWaterMark( RenderVideoParameters rvp)
        {
            GraphicsEngine engine = GraphicsEngine.Current;

            if (mWaterMarkTextDecor==null)
            {
                CTextStyle ts = CTextStyleDatabase.GetInstance().GetStyleFromName("Vanilla").Clone();
                ts.FontSize = 20;
                mWaterMarkTextDecor = new CTextDecoration("Evaluation copy of PhotoVidShow.", new RectangleF(0.23f,0.82f,0,0), 0, ts);
            }

            Rectangle r = new Rectangle(0,0,rvp.req_width, rvp.req_height);

            mWaterMarkTextDecor.RenderToGraphics(r, 0, null, null);
        }

      
        //*******************************************************************
        //  Blu-ray sizes 5 min slideshow without menus
        //
        //                        max     high     good   
        //
        //  1080 24fps           383mb   231mb    160mb
        //  720p 24fps           189mb   132mb    105mb
        //  720p 59.9fps/50fps   267mb   172mb    129mb
        //
        public long GetDiskUsageEstimation(CGlobals.VideoType outputType, CGlobals.MP4Quality quality, int height, float fps)
        {
            double seconds = GetLengthInSeconds();
            double bytesPerSecond = 0;

            if (outputType == CGlobals.VideoType.MP4)
            {
                // Not supported or needed at the moment
                return 0;
            }
            else if (outputType == CGlobals.VideoType.DVD)
            {
                double dvd_disk = ((double)CDVDVideoWriter.DVDSizeInBytes);
                double per_min = dvd_disk / 150;
                double per_second = per_min / 60;
                 
                bytesPerSecond = per_second;
            }
            else if (outputType == CGlobals.VideoType.BLURAY)
            {
                double fiveMinutesInSeconds = 5 * 60;
                double mbPerSecond = 0;

                if (height == 1080)
                {
                    if (quality == CGlobals.MP4Quality.Maximum) mbPerSecond = 383.0 / fiveMinutesInSeconds;
                    if (quality == CGlobals.MP4Quality.High) mbPerSecond = 231.0 / fiveMinutesInSeconds;
                    if (quality == CGlobals.MP4Quality.Good) mbPerSecond = 160.0 / fiveMinutesInSeconds;
                    if (quality == CGlobals.MP4Quality.Low) mbPerSecond = 80 / fiveMinutesInSeconds;
                   
                }
                else // assume 720p
                {
                    if (Math.Abs(59.94 - fps) < 0.1 || Math.Abs(50 - fps) < 0.1)
                    {
                        if (quality == CGlobals.MP4Quality.Maximum) mbPerSecond = 267.0 / fiveMinutesInSeconds;
                        if (quality == CGlobals.MP4Quality.High) mbPerSecond = 172.0 / fiveMinutesInSeconds;
                        if (quality == CGlobals.MP4Quality.Good) mbPerSecond = 129.0 / fiveMinutesInSeconds;
                        if (quality == CGlobals.MP4Quality.Low) mbPerSecond = 75.0 / fiveMinutesInSeconds;
                    }
                    else if (Math.Abs(24 - fps) < 0.1)
                    {
                        if (quality == CGlobals.MP4Quality.Maximum) mbPerSecond = 189.0 / fiveMinutesInSeconds;
                        if (quality == CGlobals.MP4Quality.High) mbPerSecond = 132.0 / fiveMinutesInSeconds;
                        if (quality == CGlobals.MP4Quality.Good) mbPerSecond = 105.0 / fiveMinutesInSeconds;
                        if (quality == CGlobals.MP4Quality.Low) mbPerSecond = 52.0 / fiveMinutesInSeconds;
                    }
                    else
                    {
                        Log.Error("Unknown fps in BD disk size estimation " + fps);
                    }
                }

                bytesPerSecond = mbPerSecond * 1024 * 1024;   
      
            }
            else if (outputType == CGlobals.VideoType.SVCD)
            {
                bytesPerSecond = 250 * 1024;
            }
            else if (outputType == CGlobals.VideoType.VCD)
            {
                bytesPerSecond = 182 * 1024;
            }
            else
            {
                Log.Error("Unknown output type (" + outputType + ") on call to GetDiskUsageEstimation");
            }

            return (long)(bytesPerSecond * seconds);
        }

        public abstract double GetLengthInSeconds();

        protected abstract void GetVideo(GraphicsEngine engine, RenderVideoParameters renderVideoParameters);
     
        protected abstract void GetVideo(int frame, bool ignore_pan_zoom, bool ignore_decorations, int req_width, int req_height, RenderSurface renderToThis);

     
        // This represents the total actual number of frames rendered for the video
		public abstract int GetTotalRenderLengthInFrames();

        // This represents the output video file total frames. 
        // For preview this is the same as GetTotalRenderLengthInFrames()
        // When encoding a video this may be different, as we may render the video at a very high FPS and then reduce for each
        // encoded video frame (e.g. motion blur effects say take 10 rendered frames and reduce to 1 average frame which then
        // gets encoded )
        public abstract int GetFinalOutputLengthInFrames();

		public abstract string GetAudioFileName();
	}
}
