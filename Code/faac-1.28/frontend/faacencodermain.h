#pragma once

extern "C"
{

extern void AACEncodeAndMux(char* waveFileName, char* videoMP4FileName, int lengthFrames, float outputVideoFPS, int* currentAudioPercentage);


}


