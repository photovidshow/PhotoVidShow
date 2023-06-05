
//////////////////////////////////////////////////////////////////////

#ifndef AUDIO_APPLY_DB_LOSS
#define AUDIO_APPLY_DB_LOSS

// apply a fade in or a fade out to a Audio buffer


class CBuffer;
class AudioApplyDBLoss
{
public:


	AudioApplyDBLoss(CBuffer* buffer, int frequency);
	virtual ~AudioApplyDBLoss();

	// vol of 1.0 means appy no db loss i.e. you want volume 100%
	// vol of 0.0 meand apply max db loss i.e. you want sound 0%
	void Process(float offset, float length, float vol);


private:
	int	mFrequency;
	CBuffer*	mBuffer;

};


#endif 
