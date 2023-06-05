// This is the main project file for VC++ application project 
// generated using an Application Wizard.
#ifndef VIDEO_ENCODER_INCLUDE
#define VIDEO_ENCODER_INCLUDE

typedef int (*getVideoPtr) (unsigned int frameNum, unsigned int *bytesInRow, char **buffer);
typedef int (*getAudioPtr) (unsigned int frameNum, unsigned int *numSamples, unsigned int *bufferSize, char **buffer);
typedef int (*doMultiplexPtr) ();

#define DO_NOT_ENCODE -100

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
						 bool use_encoder2,
						 float outputVideoFPS,
						 getVideoPtr vid_ptr,
						 getAudioPtr aud_ptr,
                         doMultiplexPtr mux_ptr);
void unmanaged_abortMPEGEncode();
float unmanaged_get_video_percentage_done();
void unmanaged_set_current_video_frame(int frame);
void unmanaged_set_current_audio_frame(int frame);

int unmanaged_getAudio(unsigned int frameNum, unsigned int *numSamples, unsigned int *bufferSize, char **buffer);
int unmanaged_audio_needs_generating();


int unmanaged_getCurrentAACEncodeFrame();


#endif


