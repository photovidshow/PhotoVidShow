using System;

namespace DVDSlideshow
{
	/// <summary>
	/// This class is used to detmine the number of frames to advance on an input video when encoding to an output video which may be at a different frame rate.
    /// This class also handle breaks and repeat frames in input video as it relies on sample frame times when do calculations.
	/// </summary>
	public class CVideoFrameRateConverter
	{     
        private double mInputVideoFrameRate = 0;      // Original frame rate reported of the input video (used as an initial guide, but not totally necessary needed)
        private double mOutputVideoFrameRate = 0;     // Output video frame rate
        private int mFrameCount = 0;
        private double mBaseSampleTime = 0;
        private double mLastSampleTime = -9999;
     

        private const int maxInputVideoStep = 10;

        public int FrameCount
        {
            get { return mFrameCount; }
        }

        public double BaseSampleTime
        {
            set { mBaseSampleTime = value; }
        }

		public CVideoFrameRateConverter(double inputVideoFrameRate, double outputVideoFrameRate)
		{
            mInputVideoFrameRate = inputVideoFrameRate;
            mOutputVideoFrameRate = outputVideoFrameRate;
		}
    
        //
        // Returns the number of frames the original input video should step, giving the lastest sample time of the input video.
        // Each call to this assumes the output video is advacing by one frame
        //
        public int FramesToStep(double sampleTime)
        {
            //
            // Just in case, in general this should not happen ( but if video was stopped for what ever reason it should simply return 0) 
            //
            if (sampleTime < (mLastSampleTime - 0.1))
            {
                return 0;
            }

            mFrameCount++;

            double outputVideoTime = (mFrameCount - 1) / mOutputVideoFrameRate;
            double relativeSampleTime = sampleTime - mBaseSampleTime;

            //
            // Ok we need to estimate what the next sample time will be
            // If no previous frames, just add the reported frame rate to the current sample time
            //
            double estimatedNextVideoFrameStep = 1 / mInputVideoFrameRate;
            if (mLastSampleTime >= 0)
            {
                //
                // Previous frame existed, use last frame rate
                //
                estimatedNextVideoFrameStep = relativeSampleTime - mLastSampleTime;

                //
                // If estimation no good then use original frame rate again
                //
                if (estimatedNextVideoFrameStep <= 0)
                {
                    estimatedNextVideoFrameStep = 1 / mInputVideoFrameRate;
                }
            }

            //
            // Just in case
            //
            if (estimatedNextVideoFrameStep <= 0)
            {
                estimatedNextVideoFrameStep = ManagedCore.UserSettings.GetFloatValue("VideoSettings", "FramesPerSecondForUnknownVideos");
            }

            mLastSampleTime = relativeSampleTime;

            double nextSampleTime = relativeSampleTime + estimatedNextVideoFrameStep; 
            double nextOutputVideoTime = (mFrameCount) / mOutputVideoFrameRate;

            double originalSampleTime = relativeSampleTime;
            double originalNextSampleTime = nextSampleTime;
          
            int toStep = 0;
            while (toStep <= maxInputVideoStep)
            {
                //
                // If current sampleTime more suited to nextOutputVideoTime rather than nextSampleTime then return current step number
                //
                if (Math.Abs(nextOutputVideoTime - relativeSampleTime) < Math.Abs(nextOutputVideoTime - nextSampleTime))
                {
                    return toStep;
                }
                else
                {
                    //
                    // Else increase sample and nextSample time as well as increment step
                    //
                    toStep++;
                    double offset = estimatedNextVideoFrameStep * toStep; ;
                    nextSampleTime = originalNextSampleTime + offset;
                    relativeSampleTime = originalSampleTime + offset;
                }
            }

            //
            // If step gets to more than maxInputVideoStep, something has probably gone wrong, just return
            //
            ManagedCore.Log.Error("Frame rate conversion reported more than a "+maxInputVideoStep+" frame step");
            return 1;
        }

       
	}

}
