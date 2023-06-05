// This is the main project file for VC++ application project 
// generated using an Application Wizard.
#define _WIN32_DCOM 
#define _CRT_SECURE_NO_WARNINGS 1
#include "VideoEncoder.h"

extern int AbortMPEG;


#include <windows.h>
#include <iostream>
#include <fstream>
#include "FileBufferCache.h"
#include "AACEncoder.h"
#include "UnmanagedErrors.h"
#include "../mpegencoder/bbmpeg/include/Bbmpgapi.h"
#include "../mpegencoder2/mpegencoder2/mpegencoder2.h"
#include "../H264Encoder/H264Encoder.h"

static int current_video_frame= 0;
static int current_audio_frame =0;
static int total_frames = 0;
static int thebuffersize = 1920;
static int current_aac_audio_frame =0;


#include "..\mpegencoder\bbmpeg\include\bbmpgapi.h"
//#include "aspmanaged.h"
#include <stdio.h>

HINSTANCE            dllPtr;          // dll handle
//bbMPEGSetFocusEntry  bbMPEGSetFocus;  // bbMPEG setfocus function pointer
//bbMPEGInitEntry      bbMPEGInit;      // bbMPEG init function pointer
//bbMPEGMakeMPEGEntry  bbMPEGMakeMPEG;  // bbMPEG makempeg function pointer
//bbMPEGShutdownEntry  bbMPEGShutdown;  // bbMPEG shutdown function pointer
char                *audioBuffer;     // local audio buffer
unsigned int         audioBufferSize; // local audio buffer size

unsigned int         width, height;   // output width and height


char outputFilename[256];

 static bool loaded = false ;
 static bool music_file_exists = false;
 

 CFileBufferCache* mMusicBuffer=NULL;

// example of the audio callback

// must be able to hold atlead 1 frame of data

 
// example of the video callback


extern int AbortMPEG;

void start_aac_encode(getAudioPtr getAudio, int length, float outputVideoFPS, int* currentAudioPercentage);


void shutdown()
{
  if (audioBuffer)
    free(audioBuffer);

  if (mMusicBuffer!=NULL)
  {
	  delete mMusicBuffer;
	  mMusicBuffer=NULL;
  }
}


// ****************************************************************************************************
float unmanaged_get_video_percentage_done()
{

	float video_done = ((float)current_video_frame)/((float)total_frames);
	float audio_done = ((float)current_audio_frame)/((float)total_frames);

	float p = (video_done *95.0f) + (audio_done *5.f);

	return p ;	
}


// ****************************************************************************************************
void unmanaged_set_current_video_frame(int frame)
{
	current_video_frame = frame ;
}
// ****************************************************************************************************
void unmanaged_set_current_audio_frame(int frame)
{
	current_audio_frame =frame;
}


// ****************************************************************************************************
int unmanaged_start_encode_mpeg_1or2(char* output_file, 
						 int length, 
						 int width, 
						 int height, 
						 int frequency,
						 int pal,
						 int widescreen, 
						 int disk_type,
						 bool ac3_mplex,
						 bool do_3_2_pulldown,
						 bool use_mpegencoder_2,
						 getVideoPtr getVideo,
						 getAudioPtr getAudio)
{
	thebuffersize = 1920;

	Trace("Encode mpeg 1/2 started");

	if (ac3_mplex==true)
	{
		Trace("Required ac3 multiplex");
	}
	else
	{
		Trace("No ac3 multiplex");
	}
	

	if (pal==1)
	{
		if (frequency==48000)
		{
			thebuffersize=1920;
		}
		if (frequency==44100)
		{
			thebuffersize=1764;
		}
	}
	else
	{
		if (frequency==48000)
		{
			if (do_3_2_pulldown==false)
			{
				thebuffersize=1601;
			}
			else
			{
				thebuffersize=2002;
			}
		}
		if (frequency==44100)
		{
			thebuffersize=1471;
		}
	}

	makeMPEGRecInfo makeMPEGInfo;

	bool encode_audio = true;

	if (ac3_mplex==true)
	{
		encode_audio = false;
	}

	if (MPEGEncoderInit(pal, widescreen, disk_type, encode_audio, do_3_2_pulldown) != bbErrNone)
	{
		Error("Could not initialise Encoder!!!!");
		current_video_frame= 0;
		current_audio_frame =0 ;
		return 0 ;
	}
	
	// bufer size = FREQUENCY divided by frame rate

	audioBufferSize = thebuffersize; // 
	audioBufferSize *= 2;   // stereo, two bytes per sample
	audioBufferSize *= 2;   // 16 bit samples, two bytes per sample

	Trace("Creating audio buffer");
	audioBuffer = (char*) malloc(audioBufferSize); // create audio buffer
	if (!audioBuffer)
	{
		Error("Could not get memory for audio buffer");
		current_video_frame= 0;
		current_audio_frame =0 ;
		return 0;
	}

	Trace("Done audio buffer");
	
	Trace("Done video buffer %d %d", width,height);
	
	makeMPEGInfo.doVideo = 1;        // set to zero if video encoding is not required
	makeMPEGInfo.width = width;      // width of video in pixels
	makeMPEGInfo.height = height;    // height of video in pixels
	makeMPEGInfo.startFrame = 0;     // starting frame number
	makeMPEGInfo.endFrame = length;     // ending frame number, must be >= startFrame

	if (ac3_mplex==false)
	{
		makeMPEGInfo.doAudio = 1;        // set to zero if audio encoding is not required
	}
	else
	{
		makeMPEGInfo.doAudio = 0;
	}

	makeMPEGInfo.audioRate = frequency;  // sample rate, valid values are 32000, 44100 and 48000
	makeMPEGInfo.stereo = 1;         // 1 = stereo, 0 = mono
	makeMPEGInfo.sampleSize = 16;    // sample size, MUST be 16 bits
	makeMPEGInfo.startSample = 0;    // first sample number
	makeMPEGInfo.endSample = length * thebuffersize - 1; // last sample number
		
	makeMPEGInfo.getVideo = getVideo;  // fill in the video callback field
	makeMPEGInfo.mgetAudio = getAudio;  // fill in the audio callback field

	makeMPEGInfo.pal = pal;
	
	makeMPEGInfo.outputFilename = outputFilename; // output MPEG filename

	Trace("Encoding...");
	if (AbortMPEG==0)
	{
		// USE OLD BBMPEG ENCODER
		if (use_mpegencoder_2==false)
		{
			MPEGEncoderStart(&makeMPEGInfo);     // call the makeMPEG routine to create an MPEG
			Trace("Finished Encoding...");
		}
		else
		// use ffmpeg for video encodinf
		{
			int ntsc=1;
			if (pal==1)
			{
				ntsc=0;
			}

			// ffmpeg encode point
			
			do_mpeg_encode2(ntsc, widescreen, disk_type, (int)do_3_2_pulldown, getVideo, length, outputFilename);

			if (AbortMPEG==0)
			{
			    // now use bbmeg to do audio
				makeMPEGInfo.doVideo = 0;
				DeclareEncodeCheckpoint('y');
				MPEGEncoderStart(&makeMPEGInfo);
				DeclareEncodeCheckpoint('z');
			}
		}
	}

	MPEGEncoderShutdown();  // call bbMPEG's shutdown routine
 
	shutdown();

	return 0;

}

// ****************************************************************************************************
int run_null_encoder(int length, getVideoPtr getVideo)
{
	char* buffer;
	unsigned int bytesinRow;

	for (int frame =0; frame < length; frame++)
	{
		if (AbortMPEG==1)
		{
			return 0;
		}

		(*getVideo)(frame, &bytesinRow, &buffer);
	}

	return 0;
}

// __declspec(dllimport) int DoEncode2(int length, int width, int height, int ifps, int quality, float outputVideoFPS, getVideoPtr getVideo, getAudioPtr getAudio);

// ****************************************************************************************************
int unmanaged_start_encode_mpeg4_h264(int length, int width, int height, int ifps, int quality, float outputVideoFPS, getVideoPtr getVideo, getAudioPtr getAudio)
{
	//DoEncode2(length, width, height, ifps, quality, outputVideoFPS, getVideo, getAudio);

	
	do_mpeg4_h264_encode(width, height, ifps, quality,  getVideo, length, 0, outputFilename);

	if (AbortMPEG==0)
	{
		start_aac_encode(getAudio, length, outputVideoFPS, &current_aac_audio_frame);	
	}

	shutdown();

	return 0;
}


// ****************************************************************************************************
int unmanaged_start_encode_bluray_h264(int length, int width, int height, int ifps, int quality, float outputVideoFPS, getVideoPtr getVideo, getAudioPtr getAudio, doMultiplexPtr mux_ptr)
{
    int outfileLen = strlen(outputFilename);
    if (outfileLen <=4)
    {
        return 1;
    }

    char* h264OutputFile = new char[outfileLen];
    for (int i=0; i < outfileLen;i++)
    {
        h264OutputFile[i]=0;
    }
    strncpy(h264OutputFile,  outputFilename, outfileLen-4);
    h264OutputFile[outfileLen-2] = '4';
    h264OutputFile[outfileLen-3] = '6';
    h264OutputFile[outfileLen-4] = '2';

	do_mpeg4_h264_encode(width, height, ifps, quality,  getVideo, length, 1, h264OutputFile);

	if (AbortMPEG==0)
	{
        //
        // Generate PCM audio for blu ray m2ts file
        //
        int sample = DO_NOT_ENCODE;
	    unsigned int numSample = 0;
	    unsigned int buffer_size=0;
	    char buffer[1];

		(*getAudio)(sample, &numSample, &buffer_size, (char**)&buffer);

        //
        // DO multiplex back in c#
        //
        if (AbortMPEG==0)
        {
            (*mux_ptr)();
        }
	}

	shutdown();

    delete h264OutputFile;

	return 0;
}


// ****************************************************************************************************
int unmanaged_start_encode(char* output_file, 
						 int length, 
						 int width, 
						 int height, 
						 int frequency,
						 int pal,
						 int widescreen, 
						 int disk_type,
						 int ifps,			// only used on mp4 video
						 int quality,		// only used on mp4 video
						 bool ac3_mplex,
						 bool do_3_2_pulldown,
						 bool dont_encode_mpeg,
						 bool use_mpegencoder_2,
						 float outputVideoFPS,		// used my aac encoder
						 getVideoPtr getVideo,
						 getAudioPtr getAudio,
                         doMultiplexPtr mux_ptr)
{
	
	if (dont_encode_mpeg==true)
	{
		return run_null_encoder(length, getVideo);
	}

    AbortMPEG=0;
    current_video_frame= 0;
	current_audio_frame =0 ;
	current_aac_audio_frame=0;
	total_frames = length ;
	loaded= false ;
	music_file_exists=false;
	
	audioBuffer = NULL;
	strcpy(outputFilename, output_file);
	
	if (disk_type==3)
	{
        // mpeg4 video, h264 and AAC
		return unmanaged_start_encode_mpeg4_h264(length, width, height, ifps, quality, outputVideoFPS, getVideo, getAudio);
	} 
	else if (disk_type==4)
    {
        // bluray m2ts with h264 and LPCM audio
        return unmanaged_start_encode_bluray_h264(length, width, height, ifps, quality, outputVideoFPS, getVideo, getAudio, mux_ptr);
    }
    else
	{
		// old disc video types i.e. VCD(mpeg1), SVCD(mpeg2), DVD(mpeg2).
		return unmanaged_start_encode_mpeg_1or2(output_file, length, width, height, frequency, pal, widescreen, disk_type, ac3_mplex, do_3_2_pulldown, use_mpegencoder_2, getVideo, getAudio);
	}
}

// ****************************************************************************************************
int unmanaged_getCurrentAACEncodeFrame()
{
	return current_aac_audio_frame;
}



// ****************************************************************************************************
void start_aac_encode(getAudioPtr getAudio, int length, float outputVideoFPS, int* currentAudioPercentage)
{
	int sample = DO_NOT_ENCODE;
	unsigned int numSample = 0;
	unsigned int buffer_size=0;
	char buffer[1];

	// generate pcm file
	(*getAudio)(sample, &numSample, &buffer_size, (char**)&buffer);

	char ifilename[512] = {0 };
	char ofilename[512] = {0};
	
	strcpy(ifilename, outputFilename);

	int len = strlen(ifilename);

	ifilename[len-3] ='p';
	ifilename[len-2] ='c';
	ifilename[len-1] ='m';

	strcpy(ofilename, outputFilename);

	len = strlen(ofilename);

	ofilename[len-3] ='m';
	ofilename[len-2] ='p';
	ofilename[len-1] ='4';

	// mp4's don't have to have sound unlike things like dvd. So if no sound file, return early now
	FILE* fp = fopen(ifilename,"r");
	if (fp==NULL)
	{
		return;
	}
	fclose(fp);

	do_aac_encode(ifilename, ofilename, length, outputVideoFPS, currentAudioPercentage);

	if (AbortMPEG==1)
	{
		return;
	}
}

// ****************************************************************************************************
void unmanaged_abortMPEGEncode()
{
 AbortMPEG=1;
 abort_mpeg_encode2();
 abort_h264_encoder();

}


// ****************************************************************************************************
int unmanaged_audio_needs_generating()
{
	if (loaded==false)
	{
		return 1;
	}
	return 0;
}


// ****************************************************************************************************
int unmanaged_getAudio(unsigned int frameNum, unsigned int *numSamples, unsigned int *bufferSize, char **buffer)
{
  memset(audioBuffer, 0, audioBufferSize); // zero the buffer

  __int64 offset = frameNum * 4;

  if (loaded == false)
  {
	  	char filename[256];
		strcpy(filename, outputFilename);

		int len = strlen(filename);

		filename[len-3] ='P';
		filename[len-2] ='C';
		filename[len-1] ='M';


		std::ifstream in(filename);

		if (in.is_open()== false)
		{
			music_file_exists=false ;
			loaded = true ;
			mMusicBuffer=NULL;
		}
		else
		{
			Trace("Loading audio file '%s' for muxing into mpg",filename);
			mMusicBuffer = new CFileBufferCache(filename, thebuffersize*50);
			loaded = true ;
			music_file_exists = true ;
		}
  }

  if (music_file_exists==true)
  {
	  DeclareEncodeCheckpoint('^');
	  // offset buffer than buffer retrun blank audio
	  if (mMusicBuffer->GetSize() <= offset)
	  {
		  memset(audioBuffer,0,audioBufferSize);
	  }

	  // buffer insize offset collect data
	  else if (mMusicBuffer->GetSize() >= offset+audioBufferSize)
	  {
		mMusicBuffer->GetData(offset,(unsigned char*)audioBuffer, audioBufferSize);
	  }

	  // some of buffer inside and some outside
	  else
	  {
		  int to_get = (int) (offset+audioBufferSize - mMusicBuffer->GetSize());

		  if (to_get >= audioBufferSize)
		  {
			  Warning("Bytes to get in buffer is bigger than expected in unmanaged_getAudio %d %d", to_get, audioBufferSize);
			  to_get = audioBufferSize-1;
		  }
		  mMusicBuffer->GetData(offset,(unsigned char*)audioBuffer, to_get);

		  int to_mem_set = audioBufferSize-to_get;
		  if (to_mem_set <=1)
		  {
			  Warning("Unexpected no bytes to get in unmanaged_getAudio %d", to_mem_set);
			  to_mem_set = 1;
		  }

		  memset((unsigned char*)audioBuffer+to_get,0,to_mem_set);
	  }
	  DeclareEncodeCheckpoint('*');
  }


  *numSamples = thebuffersize;            // return the number of samples
  *bufferSize = audioBufferSize; // return the size in bytes of samples
  *buffer = audioBuffer;         // return a pointer to the audio data

  current_audio_frame = frameNum/thebuffersize;

  return 0;              // no errors
}


