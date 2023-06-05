// AudioMixer.h: interface for the CAudioMixer class.
//
//////////////////////////////////////////////////////////////////////

#ifndef Audio_RESAMPLER_INCLUDE
#define Audio_RESAMPLER_INCLUDE


#include <list>
#include <string>

class CFileBufferCache;
class CBuffer;


class CAudioResampler 
{
public:

	// bytes_per_sample must be 1 or 2 (i.e 8bit, 16bit),
	// Also process only tested for 'up sampling' e.g. 44100hz -> 48000hz 
	// (not sure if down works, but don't really need it)
	// Best use direct show re-sampler if outside these ranges but this should cover
	// majority of audio which needs re-sampling
	CAudioResampler(CBuffer* buffer, int frequency, int bytes_per_sample, bool stereo);
	virtual ~CAudioResampler();
	static bool mAbort;

	// returns new buffer with resampled audio
	void Process(int new_freqency, char* new_fn, int size_of_header);

private:

	bool CreateFileCache(__int64 total_size, char* new_fn);

	int	mFrequency;
	int mBytesPerSample;
	bool mStereo;
	CBuffer* mBuffer;

};

#endif 

