// This is the main project file for VC++ application project 
// generated using an Application Wizard.

#include "VideoEncoder.h"

#using <mscorlib.dll>
#using <System.Drawing.dll>

#include <tchar.h>

using namespace System;
using namespace Drawing;
using namespace Imaging;
using namespace ManagedCore;


#include <iostream>
#include <string>

extern std::string ManagedToSTL(System::String^ managed);

int getAudio(unsigned int frameNum, unsigned int *numSamples, unsigned int *bufferSize, char **buffer);
int getVideo(unsigned int frameNum, unsigned int *bytesInRow, char **buffer);
int doMultiplex();


// ****************************************************************************************************
// ****************************************************************************************************
// ****************************************************************************************************

namespace MangedToUnManagedWrapper
{

	// interface a managed class needs to implement in order for this encoder
	// to perform callbacks on it to get video audio info
	public ref class IEncodeCallback abstract
	{
		public:

	    // get video for frame
		virtual int GetPointerToEncodedFrameData(int frame) =0;
		 
		// total number of frames to encode;
		virtual int	GetVideoFrameLength() =0 ;

		// callback from encoder to tell class to
		// now generate a pcm file which this will then encode
		// into the video

		// i.e. delay ripping audio because we normally 
		// want to encode video before audio stuff in burn wizard
		virtual void GenerateAudioCallback() =0;

        //
        // If multiplex is done from the c# e.g. m2ts muxing
        //
        virtual void GenerateMultiplexCallback() =0;
		

	};

	public ref class CDVDEncode
	{
		
		public: 
		static IEncodeCallback^ current_callback;
		static int mEncodeWidth =0;
	
		// ****************************************************************************************************
		// This is the entry point for this application
		static int Encode(IEncodeCallback^ cb,
			              String^ output_file, 
			              int width,
						  int height,
						  int frequency,
						  int pal,
						  int widescreen, 
						  int disk_type,	// only used on disk media 
						  int fps,		// only used on mp4 video
						  int quality,	// only used on mp4 video
						  bool ac3_mplex, bool do_3_2_pulldown, bool dont_encode_mpeg, float outputVideoFPS) 
		{
			current_callback = cb;
			std::string string = ManagedToSTL(output_file);
			char* out_file = (char*) string.c_str();

			int length =cb->GetVideoFrameLength();

			mEncodeWidth = width;

			unmanaged_start_encode(out_file, length, width, height, frequency, pal, widescreen, 
				 disk_type, fps, quality, ac3_mplex, do_3_2_pulldown, dont_encode_mpeg, true, outputVideoFPS, getVideo, getAudio, doMultiplex);
			return 1;
		}

		// ****************************************************************************************************
		static int GetAACCurentEncodeFrame()
		{
			return unmanaged_getCurrentAACEncodeFrame();
		}

		// ****************************************************************************************************
		static float GetPercentageDone()
		{
			return unmanaged_get_video_percentage_done();
	
		}

	
		// ****************************************************************************************************
		static void Abort()
		{
			unmanaged_abortMPEGEncode();
		}


		// ****************************************************************************************************
		static void GenerateAudioCallback()
		{
			((IEncodeCallback^)current_callback)->GenerateAudioCallback();
		}

        // ****************************************************************************************************
        static void GenerateMultiplexCallback()
        {
            ((IEncodeCallback^)current_callback)->GenerateMultiplexCallback();
        }
		
		
		// ****************************************************************************************************
		static void ResetProgressState()
		{	
			unmanaged_set_current_video_frame(0);
			unmanaged_set_current_audio_frame(0);
		}

		// ****************************************************************************************************
		// callback to DVDSlideshow to get frame
		static char* GetVideoFromDVDSlideShow(int frameNum)
		{
			// do managed part (i.e generate the video frame for the given frame)
			// the managed part knows what slideshow/menu is currently being encoded.
			int i = ((IEncodeCallback^)current_callback)->GetPointerToEncodedFrameData(frameNum);

			char* start_of_data = (char*) i;

			return start_of_data;
		}
	};
}


// ****************************************************************************************************
int getVideo(unsigned int frameNum, unsigned int *bytesInRow, char **buffer)
{
 // CDebugLog::GetInstance()->DebugAddEncodePosition('w');
 
  *buffer = MangedToUnManagedWrapper::CDVDEncode::GetVideoFromDVDSlideShow(frameNum);
  *bytesInRow = MangedToUnManagedWrapper::CDVDEncode::mEncodeWidth*4 ;

  unmanaged_set_current_video_frame(frameNum);
 // CDebugLog::GetInstance()->DebugAddEncodePosition('x');

  return 0;
}



// ****************************************************************************************************
int getAudio(unsigned int frameNum, unsigned int *numSamples, unsigned int *bufferSize, char **buffer)
{
  int code =0;
  // do managed part (i.e. encode the audio)
  if (unmanaged_audio_needs_generating() == 1)
  {
	 // encode audio now
	 MangedToUnManagedWrapper::CDVDEncode::GenerateAudioCallback();
  }

  // If not encode as aac, then do normal audio encode stuff
  if (frameNum!=DO_NOT_ENCODE)
  {
		int code = unmanaged_getAudio(frameNum, numSamples, bufferSize, buffer);
  }
  return code;
}

// ****************************************************************************************************
int doMultiplex()
{
	 MangedToUnManagedWrapper::CDVDEncode::GenerateMultiplexCallback();
     return 1;
}

