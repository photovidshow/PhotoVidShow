
// represents a timeline description of all the individual audio which will occur in a video.


using System;
using System.Collections;
using MangedToUnManagedWrapper;
using System.Xml;
using ManagedCore;
using System.Windows.Forms;
using System.Text;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for CMusicTrack.
    /// </summary>
    /// 

    //*******************************************************************
    //*******************************************************************
    //*******************************************************************
    public class CMusicPeformanceAudioUnit
    {
        public string mFilename;
        public int mStartFrameOffset; // offset from start of slideshow

        // this offset is needed video and audio file trimming
        public int mWavStartOffset; // what point in in wav file we start (i.e. because start has been trimmed)


        public int mLengthInFrames;
        public bool mProcessed = false;
        public string mUsedCachedFile;
        public bool mFadeIn = false;
        public float mFadeInLength = 1.0f;
        public bool mFadeOut = false;
        public float mFadeOutLength = 1.0f;
        public float mVolume = 1.0f;
        public float mInitialSilence = 0.0f;
        public double mTotalLengthOfAudioInSeconds = 0.0;

        //
        // Calculates the preview volume of this unit given a preview playback time
        //
        public double GetPreviewVolumeForTime(double time)
        {
            float fps = CGlobals.mCurrentProject.DiskPreferences.frames_per_second;
            int frame = (int) ((time*fps )+0.49999f);
            int endFrameVolumeOffset = mStartFrameOffset + mLengthInFrames;
            int initialSilenceFramesStartOffset = (int)((mInitialSilence * fps) + 0.49999f);
            int startFrameVolumeOffset = mStartFrameOffset + initialSilenceFramesStartOffset;

            //
            // Is frame inside this audio unit play time
            //
            if (frame < startFrameVolumeOffset || frame > endFrameVolumeOffset)
            {
                //
                // No, return no volume
                //
                return 0;
            }

            //
            // Do we have fade effects
            //
            if (mFadeIn == false && mFadeOut == false)
            {
                //
                // No fade effect simply return volume
                //
                return mVolume;
            }

            double returnVolume = mVolume;

            if (mFadeIn == true && mFadeInLength >0)
            {
                double fadeUpStart = (((double)mStartFrameOffset / fps)) + mInitialSilence;
                double diff = time - fadeUpStart;
                if (diff <0)
                {
                    return 0;
                }
                if (diff <= mFadeInLength)
                {
                    returnVolume *= (diff / mFadeInLength);
                }
            }

            if (mFadeOut == true && mFadeOutLength >0)
            {
                double fadeEndTime = (((double)mStartFrameOffset) / fps) + (((double)mLengthInFrames)/ fps);
                double diff = fadeEndTime - time;
                if (diff < 0)
                {
                    return 0;
                }
                if (diff <= mFadeOutLength)
                {
                    returnVolume *= (diff / mFadeOutLength);
                }
            }

            return returnVolume;
        }
    }


	//*******************************************************************
    //*******************************************************************
    //*******************************************************************
	public class CMusicPeformanceFadeSectionRequest
	{
		public double mStartOffset; // seconds
		public double mLength; // seconds
		public float mFadeRatio;   // 0.0 (0%) ... 1.0 (100%) of original 
		public float mFadeDownLength =0.5f; // seconds
		public float mFadeUpLength = 0.5f; // seconds
        public float mFadeUpEndRatio = 1.0f; // needed when merging with next request with differerent ratio
	} 


    //*******************************************************************
    //*******************************************************************
    //*******************************************************************
    public class CMusicPerformaceChannel
    {
        private ArrayList mMusicPerformanceAudioUnits;
		private ArrayList mMusicPeformanceFadeSectionRequests;
        public static int cc = 0;
        static private bool mAbort = false;
        public float mVolume = 1.0f;

        public static bool mCreatingAudioBuffer = false;
        public static bool mReSamplingAudio = false;

        static public void Abort()
        {
            mAbort = true;
        }

        public ArrayList MusicPerformanceAudioUnits
        {
            get
            {
                return mMusicPerformanceAudioUnits;
            }
        }


		public ArrayList MusicPerformanceFadeSectionRequests
		{
			get
			{
				return mMusicPeformanceFadeSectionRequests;
			}
		}


        //*******************************************************************
        public CMusicPerformaceChannel()
        {
            mMusicPerformanceAudioUnits = new ArrayList();
			mMusicPeformanceFadeSectionRequests = new ArrayList();
        }

        //*******************************************************************
        // ASSUME IN THE ORDER THAT THEY APPEAT IN THE CHANNEL
        public void AddAudioUnit(CMusicPeformanceAudioUnit unit)
        {
            mMusicPerformanceAudioUnits.Add(unit);
        }


	    //*******************************************************************
		// ASSUME IN THE ORDER THAT THEY APPEAR IN THE CHANNEL
		public void AddFadeRequests(ArrayList requests)
		{
			// overwrite
			mMusicPeformanceFadeSectionRequests = requests;
		}

        //*******************************************************************
        public CMusicPeformanceAudioUnit GetAudioUnit(int index)
        {
            return (CMusicPeformanceAudioUnit)mMusicPerformanceAudioUnits[index];
        }

		//*******************************************************************
		public CMusicPeformanceFadeSectionRequest GetFadeSectionRequest(int index)
		{
			return (CMusicPeformanceFadeSectionRequest)mMusicPeformanceFadeSectionRequests[index];
		}

        //*******************************************************************
        public bool WillOverlapWith(CMusicPerformaceChannel other_channel)
        {
            // SRG PLEASE CODE ME
            return true;
        }

        //*******************************************************************
        public int GetEndFrame()
        {
            if (mMusicPerformanceAudioUnits.Count < 1) return 0;

            CMusicPeformanceAudioUnit mu = (CMusicPeformanceAudioUnit)mMusicPerformanceAudioUnits[mMusicPerformanceAudioUnits.Count - 1];

            int end_frame = mu.mStartFrameOffset + mu.mLengthInFrames;

            return end_frame;
        }


        //*******************************************************************
        public CAudioRipResultsInformation RipStereo(string filename, string r_filename, int freq, double total_len)
        {
            mReSamplingAudio = false;

            string temp = CGlobals.GetTempDirectory() + "\\temp1.pcm";
            CAudioRipResultsInformation ai = CManagedAudioRipper.RipMediaFile(filename,
                                    temp,
                                    freq,
                                    total_len);

            if (ai == null) return null;

            // do we need to resample the ripped audio ? i.e. resampling not already done and output is different
            // compared with what we need
            if (ai.mReSamplingOccuredDuringRipping == false &&
                (ai.mFrequency != freq ||
                 ai.mBytesPerSample != 2 ||
                 ai.mStereo != true))
            {
                if (System.IO.File.Exists(temp) == true)
                {
                    mReSamplingAudio = true;
                    CManagedFileBufferCache buffer = new CManagedFileBufferCache(temp);
                    CManagedAudioResampler ras = new CManagedAudioResampler(buffer, ai.mFrequency, ai.mBytesPerSample, ai.mStereo == true ? 1 : 0);
                    int size_of_header = DVDSlideshow.CWavToRawPCMFileConverter.GetSizeOfHeader(temp, buffer);
                    ras.Process(r_filename, freq, size_of_header);

                    GC.KeepAlive(buffer);
                    GC.KeepAlive(ras);
                    buffer.Dispose();
                    buffer.Release();
                    ras.Release();
                    mReSamplingAudio = false;
                }

                GC.Collect();
                try
                {
                    System.IO.File.Delete(temp);
                }
                catch
                {
                }
            }
            else
            {
                System.IO.File.Move(temp, r_filename);
            }
            GC.Collect();

            return ai;
        }



        //*******************************************************************
        public void RipAC3(string filename)
        {
            mAbort = false;

            string temp = CGlobals.GetTempDirectory();

            // SRG TO DO, support more than 1 ac3 video in slideshow
            foreach (CMusicPeformanceAudioUnit unit in this.mMusicPerformanceAudioUnits)
            {
                string r_filename = filename + "ww";
                string dd_filename = filename;
                if (ManagedCore.IO.ForceDeleteIfExists(r_filename, true) == false) continue;
                if (ManagedCore.IO.ForceDeleteIfExists(dd_filename, true) == false) continue;
                CAudioRipResultsInformation ai = CManagedAudioRipper.RipMediaFile(unit.mFilename,
                    r_filename,
                    CGlobals.mCurrentProject.DiskPreferences.AudioFrequency,
                    unit.mTotalLengthOfAudioInSeconds);

                if (mAbort == true) return;

                if (ai == null)
                {
                    UserMessage.Show("An error occurred when trying to rip audio file '" + unit.mFilename + "'", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                DVDSlideshow.CWavToRawPCMFileConverter.Convert(r_filename, dd_filename);

                ManagedCore.IO.ForceDeleteIfExists(r_filename, true);
            }
        }

        //*******************************************************************
        // private method to calculate offter inside a audio file
        private Int64 CalcAudioOffsetFor16bitStereo(double multiple, int frame)
        {
            double bytes_in_double = multiple * frame;
            Int64 bytes_in = (Int64)bytes_in_double;

            // as everything must be 4 byte align 
            bytes_in >>= 2;
            bytes_in <<= 2;

            return bytes_in;
        }



        //*******************************************************************
        public CManagedFileBufferCache Encode(int frequency)
        {
            CGlobals.DeclareEncodeCheckpoint('r');
            mAbort = false;
            string temp = CGlobals.GetTempDirectory();
            Int64 total = GetEndFrame() + 1;
            int encode_frequency = CGlobals.mCurrentProject.DiskPreferences.AudioFrequency;

            //
            // Multiply is the number of bytes in the ripped audio file which represents 
            // 1 frame of video.  It is then used to calculate the offset position
            // in the final audio file we need to insert the ripped audio such that it
            // is sync with the video.  Default is PAL (25 fps)
            //
            double multiply = encode_frequency == 48000 ? 1920 : 1764;
            float output_fps = CGlobals.mCurrentProject.DiskPreferences.OutputVideoFramesPerSecond;
            CGlobals.VideoType outputType = CGlobals.mCurrentProject.DiskPreferences.OutputVideoType;

            //
            // Is this VCD/SVCD/DVD output.  If so test for NTSC settings and re-set audio mutiplier
            //
            // SRG (15/6/15) i'm not totally sure this is needed anymore as will always be encode_frequency / output_fps ?
            // Also 29.97 is actually 30000/1001 etc
            //
            if (outputType == CGlobals.VideoType.VCD || outputType == CGlobals.VideoType.SVCD || outputType == CGlobals.VideoType.DVD)
            {
                // ntsc (29.97 fps)
                if (System.Math.Abs(output_fps - 29.97) < 0.01)
                {
                    multiply = encode_frequency == 48000 ? 1601.6016016016016016016016016016 :
                                                           1471.47147147147147147147147147151471;

                }
                // 3:2 pulldown (i.e. ntsc 23.976)
                else if (System.Math.Abs(output_fps - 23.976) < 0.01)
                {
                    if (encode_frequency == 48000)
                    {
                        multiply = 2002.002002002002002002002002002;
                    }
                    else
                    {
                        multiply = 1839.3393393393393393393393393393;
                    }
                } 
            }
            else
            {
                //
                //  Other output type, so use output_fps 
                //
                multiply = encode_frequency / output_fps;
            }

            multiply *= 4.0;    // 16 bit stereo (i.e. x4)
            total = (Int64)(((double)total) * multiply);

            //
            // Make sure total aligned to an complete audio sample 
            //
            while (total % 4 != 0)
            {
                total++;
            }

            mCreatingAudioBuffer = true;

            CManagedMemoryBufferCache mb = new CManagedMemoryBufferCache(total, 128 * 1024);

            mCreatingAudioBuffer = false;

            if (mAbort == true)
            {
                CGlobals.DeclareEncodeCheckpoint('s');
                return mb;
            }

			// Rip audio units and apply to memory buffer
			if ( EncodeMusicAudioUnits(mb, multiply, frequency) == false)
			{
				return mb;
			}

			MangedToUnManagedWrapper.ManagedObject.Clear();
            CGlobals.DeclareEncodeCheckpoint('t');

			// Apply fades requests (e.g. fade out section of background music as video audio will be playing when channels
			// are mixed.
            ApplyFadeSectionRequests(mb, multiply, frequency);

			MangedToUnManagedWrapper.ManagedObject.Clear();
			CGlobals.DeclareEncodeCheckpoint('(');

            return mb;
		}

		//*******************************************************************
		private void   ApplyFadeSectionRequests(CManagedMemoryBufferCache mb, double multiply, int frequency)
		{
	        foreach (CMusicPeformanceFadeSectionRequest request in this.mMusicPeformanceFadeSectionRequests)
			{
					if (mAbort == true)
					{
						return;
					}

                    float fps = CGlobals.mCurrentProject.DiskPreferences.AudioFramesPerSecond;
                    CManagedAudioFadeOutSection fader = new CManagedAudioFadeOutSection(mb, frequency);
                    fader.Process(request.mStartOffset, request.mLength, request.mFadeRatio, request.mFadeDownLength, request.mFadeUpLength, request.mFadeUpEndRatio);
                    fader.Release();
			}
		}	

		
        //*******************************************************************
		private bool EncodeMusicAudioUnits(CManagedMemoryBufferCache mb, double multiply, int frequency)
		{
            foreach (CMusicPeformanceAudioUnit unit in this.mMusicPerformanceAudioUnits)
            {
                unit.mProcessed = false;
                unit.mUsedCachedFile = "";
            }

            foreach (CMusicPeformanceAudioUnit unit in this.mMusicPerformanceAudioUnits)
            {

                CAudioRipResultsInformation ai = null;

                if (mAbort == true)
                {
                    CGlobals.DeclareEncodeCheckpoint('s');
                    return false;
                }

                string r_filename = CGlobals.GetTempDirectory() + @"\" + "rippedaudio" + cc + ".wav";
                string dd_filename = CGlobals.GetTempDirectory() + @"\" + "rippedaudio" + cc + ".pcm";


                if (ManagedCore.IO.ForceDeleteIfExists(r_filename, true) == false)
                {
                    CGlobals.DeclareEncodeCheckpoint('s');
                    return false;
                }

                if (ManagedCore.IO.ForceDeleteIfExists(dd_filename, true) == false)
                {
                    CGlobals.DeclareEncodeCheckpoint('s');
                    return false;
                }

                if (unit.mUsedCachedFile != "" &&
                    System.IO.File.Exists(unit.mUsedCachedFile) == true)
                {
                    Log.Info("Using already ripped media file " + unit.mUsedCachedFile);
                    r_filename = unit.mUsedCachedFile;
                }
                else
                {
                    Log.Info("Ripping");
                    bool finishedRipAttempt = false;
                    while (finishedRipAttempt == false)
                    {
                        ai = this.RipStereo(unit.mFilename, r_filename, CGlobals.mCurrentProject.DiskPreferences.AudioFrequency, unit.mTotalLengthOfAudioInSeconds);
  
                        if (ai == null)
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            stringBuilder.Append("An error occurred when trying to rip audio file '");
                            stringBuilder.Append(unit.mFilename);
                            Log.Error(stringBuilder.ToString());
                            stringBuilder.Append("'\r\n\r\n");
                            stringBuilder.Append("Installing the AC3Filter package may fix this problem.\r\n\r\n");
                            if (CGlobals.RunningPhotoCruz == false)
                            {
                                stringBuilder.Append("This can be either downloaded from http://www.photovidshow.com/extras.html\r\n\r\n");
                                stringBuilder.Append("Or can be found on the Install CD in the video codec folder.");
                            }
                           
                            UserMessage.Show(stringBuilder.ToString(), "Error ripping audio",  MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }

                        finishedRipAttempt = true;
                    }

                    if (ai == null)
                    {
                        continue;
                    }
                }

                if (mAbort == true)
                {
                    CGlobals.DeclareEncodeCheckpoint('s');
                    MangedToUnManagedWrapper.ManagedObject.Clear();
                    return false;
                }

                Log.Info("Converting to PCM");

                int size_of_header = DVDSlideshow.CWavToRawPCMFileConverter.GetSizeOfHeader(r_filename);

                //	DVDSlideshow.CWavToRawPCMFileConverter.Convert(r_filename, dd_filename);

                if (mAbort == true)
                {
                    CGlobals.DeclareEncodeCheckpoint('s');
                    MangedToUnManagedWrapper.ManagedObject.Clear();
                    return false;
                }

                Log.Info("Applying audio gaps and fades");

                if (System.IO.File.Exists(r_filename) == false)
                {
                    continue;
                }

                CManagedFileBufferCache fb = new CManagedFileBufferCache(r_filename);

                Int64 mbsize = mb.GetSize();
                Int64 fbsize = fb.GetSize() - size_of_header;
                Int64 sdiff = fbsize - mbsize;
                if (sdiff > 0)
                {
                    Log.Info("Ripped audio bigger than required buffer. Was " + sdiff + " bytes out");
                }

                int so = 0;
                if (unit.mStartFrameOffset < 0)
                {
                    CDebugLog.GetInstance().Error("Start frame offset less than 0 when encoding music slide");
                }
                else
                {
                    so = unit.mStartFrameOffset;
                }

                // write initial silence
                float fps = CGlobals.mCurrentProject.DiskPreferences.AudioFramesPerSecond;

                int initial_silent_frames = (int)(unit.mInitialSilence * fps);
                if (initial_silent_frames != 0)
                {
                    mb.WriteBlank(CalcAudioOffsetFor16bitStereo(multiply, so),
                                  CalcAudioOffsetFor16bitStereo(multiply, initial_silent_frames));
                }

                Int64 to_write = CalcAudioOffsetFor16bitStereo(multiply, (unit.mLengthInFrames - initial_silent_frames));

                long offsetIntoFb = size_of_header + CalcAudioOffsetFor16bitStereo(multiply, unit.mWavStartOffset);

                //
                // It maybe the case that the ripped audio is not as long as reported (e.g. a video might be longer than audio in it).
                // To prevent warnings being generated we cap the amount to write to the end of the audio file.
                //
                if (to_write + offsetIntoFb > fb.GetSize())
                {
                    to_write = fb.GetSize() - offsetIntoFb;
                    if (to_write < 0) to_write = 0;
                } 

                mb.WriteBuffer(fb,
                    CalcAudioOffsetFor16bitStereo(multiply, (so + initial_silent_frames)),
                    offsetIntoFb,
                    to_write);


                float fadeInlength = unit.mFadeInLength;
                float fadeOutLength = unit.mFadeOutLength;
                float so_seconds = so / fps;

                // apply volume change accross individual music unit

                if (unit.mVolume < 1.0)
                {
                    CManagedAudioApplyDBLoss al = new CManagedAudioApplyDBLoss(mb, frequency);
                    al.Process(so_seconds, ((float)unit.mLengthInFrames) / fps, unit.mVolume);
                    al.Release();
                }

                // peform any fading on individual music unit

                if (unit.mLengthInFrames < fadeInlength * fps)
                {
                    fadeInlength = ((float)unit.mLengthInFrames) / fps;
                }

                if (unit.mLengthInFrames < fadeOutLength * fps)
                {
                    fadeOutLength = ((float)unit.mLengthInFrames) / fps;
                }

                if (unit.mFadeIn == true)
                {
                    CManagedAudioFader fader = new CManagedAudioFader(mb, frequency);
                    float sfi = so_seconds + unit.mInitialSilence - 0.1f;
                    if (sfi < 0) sfi = 0;
                    fader.Process(CManagedAudioFader.ManagedFadeType.FADE_IN, sfi, fadeOutLength);
                    fader.Release();
                }

                if (unit.mFadeOut == true)
                {
                    CManagedAudioFader fader = new CManagedAudioFader(mb, frequency);
                    float offset = so_seconds + (((float)unit.mLengthInFrames) / CGlobals.mCurrentProject.DiskPreferences.AudioFramesPerSecond) - fadeOutLength;
                    if (offset < so_seconds) offset = so_seconds;
                    fader.Process(CManagedAudioFader.ManagedFadeType.FADE_OUT, offset, fadeOutLength + 0.1f);
                    fader.Release();
                }

                cc++;
                Log.Info("Finished audio gaps and fades");

                // show we cache sound file for another unit ??

                bool delete_sound_file = true;
                unit.mProcessed = true;

                foreach (CMusicPeformanceAudioUnit unit1 in this.mMusicPerformanceAudioUnits)
                {
                    if (unit1.mProcessed == false &&
                        unit1 != unit &&
                        unit1.mFilename == unit.mFilename)
                    {
                        Log.Info("Caching rip audio for file " + unit1.mFilename + " for later use");

                        unit1.mUsedCachedFile = r_filename;
                        delete_sound_file = false;
                        break;
                    }
                }

                fb.Dispose();

                if (delete_sound_file == true)
                {
                    Log.Info("Deleting rip audio for file " + unit.mFilename);

                    ManagedCore.IO.ForceDeleteIfExists(r_filename, true);
                }
            }

			return true;
        }
    }
}


	



