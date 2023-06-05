// AudioFader.cpp: implementation of the AudioFader class.
//
//////////////////////////////////////////////////////////////////////

#include "FileBufferCache.h"
#include "AudioFadeOutSection.h"
#include "UnmanagedErrors.h"
#include <math.h>

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////


//**********************************************************************
AudioFadeOutSection::AudioFadeOutSection(CBuffer* buffer, int frequnecy) :
	mBuffer(buffer), mFrequency(frequnecy)
{

}


//**********************************************************************
AudioFadeOutSection::~AudioFadeOutSection()
{

}

extern double db_ratio(double ratio);


//**********************************************************************
void AudioFadeOutSection::Process(double startOffset, double length, float fadeRatio, float fadeDownLength, float fadeUpLength, float fadeUpEndRatio)
{
	
	__int64 size = mBuffer->GetSize();
	if (size%2!=0)
	{
		Warning("Missing byte in audio data when doing fade process, is it 16 bit?");
		size--;
	}

	double l = ((double)size) /  (((double)mFrequency) * 4.0);

	if (startOffset+length > l || startOffset < 0.0f)
	{
		if (startOffset > l)
		{
			Error("Audio fade out section process outside buffer range. Ignoring process");
			return ;
		}

		if (startOffset < 0) 
		{
			Error("Audio fade out section process outside buffer range. Setting to start");
			startOffset =0 ;
		}

		if (startOffset+length > l)
		{
			Warning("Audio fade out section process partially outside buffer range");
			length = (float) l - startOffset;
		}
	}

	__int64 start = (__int64) (startOffset * mFrequency * 4);
	__int64 end = (__int64) ((startOffset+length) *(mFrequency * 4));

	__int64 endFadeDown = (__int64) ((startOffset+fadeDownLength) *(mFrequency * 4));
	__int64 startFadeUp = end - ((__int64) ((fadeUpLength) *(mFrequency * 4)));

	//  Make sure start and end fall on 2 byte boundary
	if (start%2==1)
	{
		start--;
	}
	if (end%2==1)
	{
		end++;
		if (end >size)
		{
			end=size;
		}
	}


	double dl = (double) endFadeDown - start;
	double ul = (double) end - startFadeUp;

	for (__int64 i=start;i<end;i+=2)
	{
		// fade down at start
		double multi = 1.0;

		if (i < endFadeDown)
		{
			double delta = 1.0 - ((double)(i-start))/dl ;	

			double vol_ratio = ((1.0 - fadeRatio) * delta) + fadeRatio;
			multi = db_ratio(vol_ratio);
		}
		// fade up at end
		else if (i >= startFadeUp)
		{
			double delta = ((double)(i-startFadeUp))/ul ;	
			double vol_ratio = ((fadeUpEndRatio - fadeRatio) * delta) + fadeRatio;
			multi = db_ratio(vol_ratio);
		}
		// fade in middle
		else
		{
			multi = db_ratio(fadeRatio);
		}
 
		if (multi <0.0f) multi = 0.0f;
		if (multi >1.0f) multi = 1.0f;

		short current ;
		mBuffer->GetData(i, (unsigned char *)&current, 2);

		double ss = (double) current;
		ss *=multi;
		current = (short) (ss+0.5);

		mBuffer->WriteData(i, (unsigned char *)&current, 2);
	
	}
}


