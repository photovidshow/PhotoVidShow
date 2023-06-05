#ifndef H264ENCODE_INCLUDE
#define H264ENCODE_INCLUDE


extern "C"
{
	extern int do_mpeg4_h264_encode(int width, int height, int ifps, int quality, getVideoFptr getVideo, int length, int bluray, char* outputFilename) ;
	extern void abort_h264_encoder();
}

#endif;
