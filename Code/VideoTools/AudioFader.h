// AudioFader.h: interface for the AudioFader class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_AudioFADER_H__84F31067_E9AA_4A62_AAB8_7BFC24491278__INCLUDED_)
#define AFX_AudioFADER_H__84F31067_E9AA_4A62_AAB8_7BFC24491278__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000


// apply a fade in or a fade out to a Audio buffer


class CBuffer;
class AudioFader  
{
public:

	enum FadeType
	{
		FADE_IN,
		FADE_OUT
	};


	AudioFader(CBuffer* buffer, int frequency);
	virtual ~AudioFader();

	void Process(FadeType type, float offset, float length);


private:
	int	mFrequency;
	CBuffer*	mBuffer;

};



#endif // !defined(AFX_AudioFADER_H__84F31067_E9AA_4A62_AAB8_7BFC24491278__INCLUDED_)
