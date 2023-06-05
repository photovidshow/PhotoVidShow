#ifndef AUDIO_RIPPER_INCLUDE
#define AUDIO_RIPPER_INCLUDE

#include <stdlib.h>
#include "AudioGraphRendererBase.h"

class CAudioRipper : public CAudioGraphRendererBase
{
public:
	static int BuildAndRunGraph(
		                    char* in_filename,
							char* out_filename,
							int desired_resample_auido_frequency,
							int* out_frequency,
							int* out_bytes_per_sample,
							int* out_stereo,
							bool useLav);
	static void SetRipAbortFlag(int val);
	static void BuildCodecArray();

private:

	static bool mAbort;

};

#endif
