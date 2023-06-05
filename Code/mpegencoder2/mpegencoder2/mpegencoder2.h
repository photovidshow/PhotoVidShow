#ifndef MPEG_ENCODER2
#define MPEG_ENCODER2

#ifdef __cplusplus
extern "C"
{
#endif

typedef int(*getVideoFptr)(unsigned int frameNum, unsigned int *bytesInRow, char **buffer);

int do_mpeg_encode2(int ntsc,
					int widescreen,
					int disk_type,
					int do_32_pulldown,
					getVideoFptr get_video, 
					int number_of_frames,
					char* outputname);


void abort_mpeg_encode2();


#ifdef __cplusplus
}
#endif


#endif
