#include "faacencodermain.h"

void __cdecl do_aac_encode(char* ifilename, char*  ofilename, int length_frames, float outputVideoFPS, int* currentAudioPercentage)
{
	AACEncodeAndMux(ifilename, ofilename, length_frames, outputVideoFPS, currentAudioPercentage );
}
