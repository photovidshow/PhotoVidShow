// AudioMixer.h: interface for the CAudioMixer class.
//
//////////////////////////////////////////////////////////////////////

#ifndef Audio_MIXER_INCLUDE
#define Audio_MIXER_INCLUDE


#include <list>
#include <string>

class CFileBufferCache;
class CBuffer;

// takes n streams of Audio audio data and mixes them together to produce one stream
// must be 16 bit and stereo.   Input on construction takes frequency
class CAudioMixer  
{
public:
	CAudioMixer(int frequency=44100);
	virtual ~CAudioMixer();
	static bool mAbort;


	void AddStream(CBuffer* buffer, float volume);

	// returns which stream is output
	int Process();

private:

	class	CStream
	{
	public:
		CStream(CBuffer* buffer, float volume) : mBuffer(buffer), mVolume(volume) {} 

		CBuffer* mBuffer;
		float mVolume;
	};


	void MixAudio(__int64 index, unsigned char* from_buffer, int amount, CFileBufferCache* to_buffer, float vol_coef, bool master_cahce);

	std::list<CStream>	mStreams;
	int	mFrequency;

	typedef std::list<CStream>::iterator StreamIterator;

};

#endif 

