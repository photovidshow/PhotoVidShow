using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml;
using ManagedCore;
using System.Globalization;
using System.Threading;
using System.Collections.Generic;
using DVDSlideshow.GraphicsEng;
 
namespace DVDSlideshow
{
    // *********************************************************************************************
    public class Encoder : MangedToUnManagedWrapper.IEncodeCallback
    {
        // *********************************************************************************************
        public class EncodedFrame
        {
            public DisposableObject<DynamicSurface> mImage = new DisposableObject<DynamicSurface>();
            public int mFrame = -1;
        }

        private static bool mNextEncodeFrameKeyFrame = true;
        private static bool mCurrentEncodeFrameKeyFrame = true;
        private static Bitmap mEncodeFailedToGenerateFrameBitmap = null;
        private static IntPtr mEncodeFailedToGenerateFrameBitmapPtr = new IntPtr();
        private static bool mHardwareSupportsMotionBlur = true;


        // *********************************************************************************************
        public static bool HardwareSupportsMotionBlur
        {
            get { return mHardwareSupportsMotionBlur; }
            set { mHardwareSupportsMotionBlur = value; }
        }

        // *********************************************************************************************
        public static bool NextEncodeFrameKeyFrame
        {
            get
            {
                if (CGlobals.mVideoGenerateMode == VideoGenerateMode.FINAL_OUTPUT_MODE)
                {
                    return mNextEncodeFrameKeyFrame;
                }
                return true;
            }

            set { mNextEncodeFrameKeyFrame = value; }
        }

        // *********************************************************************************************
        public static bool CurrentEncodeFrameKeyFrame
        {
            get
            {
                if (CGlobals.mVideoGenerateMode == VideoGenerateMode.FINAL_OUTPUT_MODE)
                {
                    return mCurrentEncodeFrameKeyFrame;
                }
                return true;
            }
            set { mCurrentEncodeFrameKeyFrame = value; }
        }


        private EncodedFrame mCurrentlyBeingEncodedFrame; // keep track of the current encode frame being encoded 
        private Queue<EncodedFrame> mEncodedFrames = new Queue<EncodedFrame>(); // used to pass rendered encoded frames to the encode thread
        private Queue<EncodedFrame> mFreedEncodedFrames = new Queue<EncodedFrame>(); // used to pass back freed up encode frames back to the renderer
        private object EncodeMutex = new object(); // mutex for above two queues as being accessed from multiple threads
        private Semaphore mEncodeSemaphore = null;  // semaphore to signal encoder to encode a frame (from mEncodedFrames queue)
        private Semaphore mRenderSemaphore = null;  // semaphore to signal renderer to render the next frame (from mFreedEncodedFrames queue)

        private int mBackgroundEncodeFrameTotalFrameCount = 0;
        private CVideo mForVideo = null;
        private int mLastFrameGenerated = -1;

        private bool mGenerateFramesInBackGround = false;
        private System.Threading.Thread mGenerateThread;

        private int mEnocdeFramesCreated = 0; // debug info to tell us how many encode frames the encoder created

        // ****************************************************************************************************************
        public Encoder(CVideo forVideo)
        {
            mForVideo = forVideo;
            mEncodeSemaphore = new Semaphore(0, 1000);
            mRenderSemaphore = new Semaphore(0, 1000);
        }

        // ****************************************************************************************************************
        private void GenerateFramesInBackgroundStart()
        {
            if (GraphicsEngine.Current == null) return;

            uint width = (uint)CGlobals.mCurrentProject.DiskPreferences.CanvasWidth;
            uint height = (uint)CGlobals.mCurrentProject.DiskPreferences.CanvasHeight;
            // Set graphics engine viewport to match endode video size 
            GraphicsEngine.Current.Reset(width, height);
            GraphicsEngine.Current.SetRenderTargetToDefault();

            while (mGenerateFramesInBackGround == true && mLastFrameGenerated <= mBackgroundEncodeFrameTotalFrameCount - 1)
            {
                if (mRenderSemaphore.WaitOne(1000, false) == true)
                {
                    if (mGenerateFramesInBackGround == true)
                    {
                        mLastFrameGenerated++;
                        EncodedFrame encoded_frame = GetCurrentVideo(mLastFrameGenerated);

                        lock (EncodeMutex)
                        {
                            mEncodedFrames.Enqueue(encoded_frame);
                            mEncodeSemaphore.Release();
                        }
                    }
                }
            }

            ManagedCore.CDebugLog.GetInstance().Trace("Finished generate encode frames in background");
            // finished
        }


        // ****************************************************************************************************************
        public void StartGenerateFramesInBackground()
        {
            CGlobals.mCurrentProject.DiskPreferences.RenderToEncodeFPSRatio = mForVideo.GetMaxNumberRequiredMotionBlurSubFrames();
           
            mGenerateFramesInBackGround = true;
            mBackgroundEncodeFrameTotalFrameCount = mForVideo.GetFinalOutputLengthInFrames();

            //
            // Max number of encode frames that can exist at once (this is less important now things are done by GPU)
            //
            int maxEncodeFrames = 3;

            int outputVideoHeight = CGlobals.mCurrentProject.DiskPreferences.OutputVideoHeight;

            //
            // The Intel HD 4000 graphics drivers seem to have an internal maximum amount of memory we can use for encoded frames.
            // If this gets exceeded then we get strange crashes in GetRenderTargetData().  After tests, the following maximums 
            // are set to be able to work on all graphics drivers.
            //
            if (outputVideoHeight >= 768)
            {
                maxEncodeFrames = 1;
            }
            else if (outputVideoHeight >= 640)
            {
                maxEncodeFrames = 2;
            }

            mRenderSemaphore.Release(maxEncodeFrames);

            // calc what encode mex frame rate 

            System.Threading.ThreadStart ts = new System.Threading.ThreadStart(GenerateFramesInBackgroundStart);
            mGenerateThread = new Thread(ts);
            mGenerateThread.Start();
        }

        // ****************************************************************************************************************
        public void StopGeneratingInBackGroundAndCleanup()
        {
            if (mGenerateThread == null) return;

            mGenerateFramesInBackGround = false;
            bool result = mGenerateThread.Join(10000);
            if (result == false)
            {
                ManagedCore.CDebugLog.GetInstance().Error("Failed to stop encoder generate frames thread");
            }

            mGenerateThread = null;

            // Make sure no frames still left in encode queue
            foreach (EncodedFrame frame in mEncodedFrames)
            {
                mFreedEncodedFrames.Enqueue(frame);
            }

            mEncodedFrames.Clear();

            if (mCurrentlyBeingEncodedFrame != null)
            {
                mCurrentlyBeingEncodedFrame.mImage.Object.Unlock();
                mFreedEncodedFrames.Enqueue(mCurrentlyBeingEncodedFrame);
                mCurrentlyBeingEncodedFrame = null;
            }

            // Clear out freed encode frames

            foreach (EncodedFrame frame in mFreedEncodedFrames)
            {
                frame.mImage.Dispose();
            }

            mFreedEncodedFrames.Clear();
        }

      
        // ****************************************************************************************************************
        public override int GetPointerToEncodedFrameData(int frame)
        {
            CGlobals.DeclareEncodeCheckpoint('A');

            EncodedFrame next_frame = null;

            if (mGenerateFramesInBackGround == true)
            {
                if (mCurrentlyBeingEncodedFrame != null)
                {
                    mCurrentlyBeingEncodedFrame.mImage.Object.Unlock();

                    lock (EncodeMutex)
                    {  
                        mFreedEncodedFrames.Enqueue(mCurrentlyBeingEncodedFrame);
                        mRenderSemaphore.Release();
                    }

                    mCurrentlyBeingEncodedFrame = null;  
                }

                while (mGenerateFramesInBackGround == true)
                {
                    if (mEncodeSemaphore.WaitOne(1000, false) == true)
                    {
                        lock (EncodeMutex)
                        {
                            if (mEncodedFrames.Count > 0)
                            {
                                next_frame = mEncodedFrames.Peek();

                                // If something gone wrong abort background processing
                                if (next_frame.mFrame != frame)
                                {
                                    ManagedCore.CDebugLog.GetInstance().Error("Encoder problem.  Next frame in queue is " + next_frame.mFrame + " but we wanted frame " + frame);
                                    mGenerateFramesInBackGround = false;
                                    next_frame = null;
                                    break;
                                }
                                else
                                {
                                    mEncodedFrames.Dequeue();
                                    break;
                                }
                            }
                            else
                            {
                                ManagedCore.Log.Error("Encode queue empty?");
                            }
                        }
                    }
                }
            }

            if (next_frame != null)
            {
                LockedRect lr = next_frame.mImage.Object.Lock();
                mCurrentlyBeingEncodedFrame = next_frame;
  
                CGlobals.DeclareEncodeCheckpoint('B');
                return lr.mBits;
            }
           
            ManagedCore.CDebugLog.GetInstance().Error("Encoder problem.  No generated frame, returning invalid frame");

            // This should never happen but just in case
            if (mEncodeFailedToGenerateFrameBitmap == null)
            {
                int w = CGlobals.mCurrentProject.DiskPreferences.OutputVideoWidth;
                int h = CGlobals.mCurrentProject.DiskPreferences.OutputVideoHeight;

                mEncodeFailedToGenerateFrameBitmap = new Bitmap(w, h, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(mEncodeFailedToGenerateFrameBitmap))
                {
                    g.Clear(Color.Red);
                }
                System.Drawing.Imaging.BitmapData bd = mEncodeFailedToGenerateFrameBitmap.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                mEncodeFailedToGenerateFrameBitmapPtr = bd.Scan0;
            }

            return mEncodeFailedToGenerateFrameBitmapPtr.ToInt32();

        }

        // ****************************************************************************************************************
        public override void GenerateAudioCallback()
        {
            if (CGlobals.mCurrentPA != null)
            {
                CGlobals.mCurrentPA.AudioGenerateCallback();
            }

        }

        // ****************************************************************************************************************
        public override void GenerateMultiplexCallback()
        {
            if (CGlobals.mCurrentPA != null)
            {
                CGlobals.mCurrentPA.MultiplexCallback();
            }
        }

        // ****************************************************************************************************************
        public override int GetVideoFrameLength()
        {
            return mForVideo.GetFinalOutputLengthInFrames();
        }

   

        // ****************************************************************************************************************
        private EncodedFrame GetCurrentVideo(int frame)
        {
            CGlobals.mCurrentProcessFrame++;

            if (CGlobals.mCurrentProcessFrame % 250 == 0)
            {
                GC.Collect();
            }

            int exceptionTries = 0;
            bool got_video = false;

            // incase need to clean up mem

            EncodedFrame encoded_frame = null;

            lock (EncodeMutex)
            {
                if (mFreedEncodedFrames.Count > 0)
                {
                    encoded_frame = mFreedEncodedFrames.Dequeue();
                }
                else
                {
                    encoded_frame = new EncodedFrame();

                    mEnocdeFramesCreated++;
                    ManagedCore.CDebugLog.GetInstance().Trace("Created encode frame:" + mEnocdeFramesCreated + " on frame:" + frame);
                }
            }

            while (got_video == false && mGenerateFramesInBackGround && exceptionTries < 2)
            {
                int reqWidth = CGlobals.mCurrentProject.DiskPreferences.OutputVideoWidth;
                int reqHeight = CGlobals.mCurrentProject.DiskPreferences.OutputVideoHeight;

                if (encoded_frame.mImage.IsNull() == true)
                {
                    encoded_frame.mImage.Assign(GraphicsEngine.Current.GenerateDynamicSurface((uint)reqWidth, (uint)reqHeight, DynamicSurface.Usage.READ_WRITE, this.ToString() + "::GetCurrentVideo", false));
                }

                if (encoded_frame.mImage.IsNull() == false &&
                    (encoded_frame.mImage.Object.Width != reqWidth ||
                     encoded_frame.mImage.Object.Height != reqHeight))
                {
                    encoded_frame.mImage.Assign(GraphicsEngine.Current.GenerateDynamicSurface((uint)reqWidth, (uint)reqHeight, DynamicSurface.Usage.READ_WRITE, this.ToString() + "::GetCurrentVideo2", false));
                }

                try
                {
                    if (GetMotionBlurredFrameFromMultipleRenderFrames(frame, encoded_frame.mImage) == GraphicsEngine.State.OK)
                    {
                        got_video = true;
                    }
                }
                catch (Exception e4)
                {
                    GC.Collect();
                    ManagedCore.CDebugLog.GetInstance().Error("Exception thrown whilst getting encoded frame :" + frame + ", exception:" + e4.Message);
                    if (exceptionTries == 1)
                    {
                        UserMessage.Show("Error in encode image callback frame " + frame + "\n \n" + e4.Message + "\n\n" + e4.GetType() +
                            "\n\nStack Trace:\n" +
                            e4.StackTrace, "Application error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);

                    }
                    exceptionTries++;
                }
            }

            encoded_frame.mFrame = frame;
            return encoded_frame;
        }

        // ****************************************************************************************************************
        /// <summary>
        /// What we do here is get X number of render frames, then average them out to produce 1 video frame.
        /// It averages out the frame which will cause motion blur effect
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="image"></param>
        private GraphicsEngine.State GetMotionBlurredFrameFromMultipleRenderFrames(int frame, DynamicSurface surface)
        {
            CGlobals.mCurrentProcessSubFrame = 1;
            mCurrentEncodeFrameKeyFrame = true;

            int encode_ratio = 1;  
            int desired_motion_blur_sub_frames = 1;
            int encode_frame = frame * encode_ratio;

            if (CGlobals.mCurrentProject.DiskPreferences.TurnOffMotionBlur == false &&
                mHardwareSupportsMotionBlur == true)
            {
                encode_ratio = CGlobals.mCurrentProject.DiskPreferences.RenderToEncodeFPSRatio;
                encode_frame = frame * encode_ratio;
                desired_motion_blur_sub_frames = mForVideo.GetNumberRequiredMotionBlurSubFrames(encode_frame);
            }

       
            RenderVideoParameters renderParameters = new RenderVideoParameters();
            renderParameters.frame = encode_frame;
            renderParameters.req_width = CGlobals.mCurrentProject.DiskPreferences.CanvasWidth;;
            renderParameters.req_height = CGlobals.mCurrentProject.DiskPreferences.CanvasHeight;;
            renderParameters.do_begin_and_end_scene = false;

      
            // Only 1 sub frame needed?, no point doing motion blur then.
            if (desired_motion_blur_sub_frames == 1)
            {
                return GetMotionBlurredFrameFromSingleRenderFrames(renderParameters, surface);
            }

            return GetMotionBlurredFrameFromMultipleRenderFrames(renderParameters, surface, encode_ratio, desired_motion_blur_sub_frames);
        }


        // ****************************************************************************************************************
        /// <summary>
        /// Generate encode video frame from one output engine frame
        /// </summary>
        private GraphicsEngine.State GetMotionBlurredFrameFromSingleRenderFrames(RenderVideoParameters renderParameters, DynamicSurface encodeVideoSurface)
        {
            GraphicsEngine ge = GraphicsEngine.Current;
            if (ge == null) return GraphicsEngine.State.NO_ENGINE_DEFINED;

            CGlobals.mCurrentProcessSubFrameTotal = 1;

            NextEncodeFrameKeyFrame = true;

            GraphicsEngine.State state = GraphicsEngine.Current.BeginScene(null);

            if (state != GraphicsEngine.State.OK)
            {
                return state;
            }

            try
            {
                          
                DisposableObject<RenderSurface> engineOuputSurface = new DisposableObject<RenderSurface>(GenerateRequireEngineOutputRenderSurface());
                using (engineOuputSurface)
                {
                    RenderSurface originalSurface = ge.GetRenderTarget();
                    try
                    {
                        GraphicsEngine.Current.SetRenderTarget(engineOuputSurface);
                        mForVideo.RenderVideo(renderParameters);
                    }
                    finally
                    {
                        GraphicsEngine.Current.SetRenderTarget(originalSurface);
                    }
                    CopyEngineOutputFrameToVideoEncodeSurface(engineOuputSurface, encodeVideoSurface);
                }
                               
            }
            finally
            {
                GraphicsEngine.Current.EndScene();
            }

            return GraphicsEngine.Current.PresentToWindow();
        }


        // ****************************************************************************************************************
        /// <summary>
        /// Generate encode video frame from multiple output engine frames (motion blur)
        /// </summary>
        private GraphicsEngine.State GetMotionBlurredFrameFromMultipleRenderFrames(RenderVideoParameters renderParameters, DynamicSurface encodeVideoSurface, int encodeRatio, int desired_motion_blur_sub_frames)
        {
            GraphicsEngine ge = GraphicsEngine.Current;
            if (ge == null) return GraphicsEngine.State.NO_ENGINE_DEFINED;

            int width = renderParameters.req_width;
            int height = renderParameters.req_height;
            int encode_frame = renderParameters.frame;

            CurrentEncodeFrameKeyFrame = true;

            BeginSceneParameters parameters = new BeginSceneParameters();
            GraphicsEngine.State state = ge.BeginScene(parameters);

            if (state != GraphicsEngine.State.OK)
            {
                return state;
            }
            try
            {
                DisposableObject<RenderSurface> engineOuputSurface = new DisposableObject<RenderSurface>(GenerateRequireEngineOutputRenderSurface());
                using (engineOuputSurface)
                {
                    RenderSurface originalSurface = ge.GetRenderTarget();
                    try
                    {
                        if (engineOuputSurface.IsNull() == false)
                        {
                            ge.SetRenderTarget(engineOuputSurface);
                        }

                        // ok, full on motion blur

                        // Create temporary surfaces which are guaranteed to be cleaned up 
                        using (DisposableObject<RenderSurface> accumulativeSurface = new DisposableObject<RenderSurface>())
                        using (DisposableObject<RenderSurface> accumulativeNextSurface = new DisposableObject<RenderSurface>())
                        using (DisposableObject<RenderSurface> nextRenderVideoSurface = new DisposableObject<RenderSurface>())
                        {
                            accumulativeSurface.Assign(ge.GenerateRenderSurface((uint)width, (uint)height, 8, this.ToString() + "::GetMotionBlurredFrameFromMultipleRenderFrames"));  // Create 64 bit accumulative surface

                            PixelShaderEffect accum_pixel_shader = PixelShaderEffectDatabase.GetInstance().GetPixelShader("accumultive");
                            PixelShaderEffect accum_pixel_shader_initial = PixelShaderEffectDatabase.GetInstance().GetPixelShader("accumultiveInitial");

                            if (accum_pixel_shader == null)
                            {
                                CDebugLog.GetInstance().Error("Accumulative pixel shader not found");
                                return GraphicsEngine.State.OK;
                            }

                            if (accum_pixel_shader_initial == null)
                            {
                                CDebugLog.GetInstance().Error("Initial accumulative pixel shader not found");
                                return GraphicsEngine.State.OK; ;
                            }

                            RectangleF srcRecf = new RectangleF(0, 0, 1, 1);
                            RectangleF recf = new RectangleF(0, 0, width, height);

                            float step = ((float)encodeRatio) / ((float)desired_motion_blur_sub_frames);

                            CGlobals.mCurrentProcessSubFrameTotal = (int)desired_motion_blur_sub_frames;

                            RenderSurface finalRendertarget = ge.GetRenderTarget();

                            // Calc number of subframe we will generate
                            int total_subframes = 0;
                            for (float i = 0; i < encodeRatio; i += step) total_subframes++;

                            // This is important, as multiple video frames now equals one frame, we take over the begin/end scene sequenece
                            renderParameters.do_begin_and_end_scene = false;

                            int sub_frame_count = 0;

                            for (float i = 0; i < encodeRatio; i += step)
                            {
                                sub_frame_count++;
                                nextRenderVideoSurface.Assign(ge.GenerateRenderSurface((uint)width, (uint)height, this.ToString() + "::GetMotionBlurredFrameFromMultipleRenderFrames2"));

                                ge.SetRenderTarget(nextRenderVideoSurface);
                                ge.ClearRenderTarget(0, 0, 0, 255);

                                if (i != 0)
                                {
                                    CurrentEncodeFrameKeyFrame = false;
                                }

                                if (i + step >= encodeRatio)
                                {
                                    NextEncodeFrameKeyFrame = true;
                                }
                                else
                                {
                                    NextEncodeFrameKeyFrame = false;
                                }

                                // Inform globals of progress ( fed up to GUI)
                                int sub_frame = (int)(i + 0.4999f);
                                CGlobals.mCurrentProcessSubFrame = 1 + (int)(((float)sub_frame) / step);

                                renderParameters.frame = encode_frame + sub_frame;

                                mForVideo.RenderVideo(renderParameters);

                                // Add frame to our accumlative texture
                                if (i == 0)
                                {
                                    // First frame, don't mix, just add
                                    ge.SetRenderTarget(accumulativeSurface);
                                    accum_pixel_shader_initial.SetParameter(0, total_subframes);
                                    ge.SetPixelShaderEffect(accum_pixel_shader_initial);
                                    ge.DrawImage(nextRenderVideoSurface, srcRecf, recf, false);
                                }
                                else
                                {
                                    // Last frame render to final target
                                    if (sub_frame_count == total_subframes)
                                    {
                                        ge.SetRenderTarget(finalRendertarget);
                                    }
                                    else
                                    {
                                        // Else render to next temp accumulative surface
                                        accumulativeNextSurface.Assign(ge.GenerateRenderSurface((uint)width, (uint)height, 8, this.ToString() + "::GetMotionBlurredFrameFromMultipleRenderFrames3"));
                                        ge.SetRenderTarget(accumulativeNextSurface);
                                    }

                                    // Mix and add frame
                                    accum_pixel_shader.SetInputImage1(accumulativeSurface);
                                    accum_pixel_shader.SetParameter(0, total_subframes);
                                    ge.SetPixelShaderEffect(accum_pixel_shader);
                                    ge.DrawImage(nextRenderVideoSurface, srcRecf, recf, false);
                                    accumulativeSurface.Assign(accumulativeNextSurface);  // transfer ownership of accumulativeNextSurface
                                }

                                ge.SetPixelShaderEffect(null);

                                nextRenderVideoSurface.Dispose(); // clean up now (so cache will re-use it on next frame)
                            }
                        }

                        CopyEngineOutputFrameToVideoEncodeSurface(engineOuputSurface, encodeVideoSurface);
                    }
                    finally
                    {

                        ge.SetPixelShaderEffect(null); // just in case
                        ge.SetRenderTarget(originalSurface);
                    }
                }
            }
            finally
            {
                ge.EndScene();
            }

            return GraphicsEngine.Current.PresentToWindow();
        }


        // ****************************************************************************************************************
        /// <summary>
        /// Copy engine output frame to video encode surface, this will typically be a straight copy.
        /// For things like DVD the encode surface will be a different size and aspect ratio and may contain
        /// a blank safe area border etc
        /// </summary>
        /// <param name="surface"></param>
        private void CopyEngineOutputFrameToVideoEncodeSurface(RenderSurface engineOutputFrameSurface, DynamicSurface encodeVideoSurface)
        {
            GraphicsEngine engine = GraphicsEngine.Current;

            // If the engineOutputFrameSurface is not the default surface, then this means we need to make scaled copy (i.e the two input surfaces will be a different dimension

            if ( engineOutputFrameSurface != null )
            {
                CDiskAuthoringPreferences preferences = CGlobals.mCurrentProject.DiskPreferences;

                using (RenderSurface copiedSurface = engine.GenerateRenderSurface((uint)preferences.OutputVideoWidth, (uint)preferences.OutputVideoHeight, false, this.ToString() + "::CopyEngineOutputFrameToVideoEncodeSurface"))
                {
                    RenderSurface currentSurface = engine.GetRenderTarget();
                    try
                    {
                        engine.SetRenderTarget(copiedSurface);
                        engine.ClearRenderTarget(0, 0, 0, 255);

                        Rectangle rec = CGlobals.mCurrentProject.DiskPreferences.GetFinalDiskCropRectangle((int)copiedSurface.Width, (int)copiedSurface.Height);

                        engine.DrawImage(engineOutputFrameSurface, new RectangleF(0, 0, 1, 1), new RectangleF((float)rec.X, (float)rec.Y, (float)rec.Width, (float)rec.Height), false);
                        GraphicsEngine.Current.CopyDefaultSurfaceToDynamicSurface(encodeVideoSurface);
                        
                    }
                    finally
                    {
                        engine.SetRenderTarget(currentSurface);
                    }

                }
            }
            else
            {
                engine.CopyDefaultSurfaceToDynamicSurface(encodeVideoSurface);
            }
        }


        // ****************************************************************************************************************
        /// <summary>
        /// Generates the final engine ouput render surface.
        /// If this returns null, the means the default render surface is the final surace and will
        /// match same dimension as the encodeVideoSurface
        /// </summary>
        /// <returns></returns>
        private RenderSurface GenerateRequireEngineOutputRenderSurface()
        {
            CDiskAuthoringPreferences preferences = CGlobals.mCurrentProject.DiskPreferences;

            //Check if two dimensions match, if not have to scale copy
            if (preferences.CanvasWidth != preferences.OutputVideoWidth ||
                preferences.CanvasHeight != preferences.OutputVideoHeight)
            {
                return GraphicsEngine.Current.GenerateRenderSurface((uint)preferences.CanvasWidth, (uint)preferences.CanvasHeight, this.ToString()+"::GenerateRequireEngineOutputRenderSurface");
            }

            return null;
        }
    }
}
