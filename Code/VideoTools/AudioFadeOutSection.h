// AudioFader.h: interface for the AudioFader class.
//
//////////////////////////////////////////////////////////////////////

#ifndef AUDIO_FADE_OUT_SECTION_H
#define AUDIO_FADE_OUT_SECTION_H


class CBuffer;
class AudioFadeOutSection  
{
public:

	AudioFadeOutSection(CBuffer* buffer, int frequency);
	virtual ~AudioFadeOutSection();

	void Process(double startOffset, double length, float fadeRatio, float fadeDownLength, float fadeUpLength, float fadeUpEndRatio);


private:
	int	mFrequency;
	CBuffer*	mBuffer;

};



#endif 
