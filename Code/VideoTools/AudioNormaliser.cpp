// AudioNormaliser.cpp: implementation of the AudioNormaliser class.
//
//////////////////////////////////////////////////////////////////////

#include "AudioNormaliser.h"
#include "FileBufferCache.h"
#include "UnmanagedErrors.h"

bool AudioNormaliser::mAbort=false;

// ******************************************************************************************************
AudioNormaliser::AudioNormaliser(CBuffer* buffer) 
:mBuffer(buffer)
{
}

// ******************************************************************************************************
AudioNormaliser::~AudioNormaliser()
{
}

// ******************************************************************************************************
void AudioNormaliser::Process()
{
	mAbort=false;

	Trace("Normalising audio...");
	__int64 length = mBuffer->GetSize();

	if (length%2!=0)
	{
		Warning("Missing byte in audio data when normalising is it 16 bit?");
		length--;
	}

	int max =0;

	__int64 i=0;
	for (i=0;i<length;i+=2)
	{
		if (mAbort==true) 
		{
			return;
		}

		short current=0;
		mBuffer->GetData(i, (unsigned char *)&current, 2);

		if ((abs(current)) > max) max = abs(current) ;
	}

	Trace("Peak signal found when normalising was %d", max);

	double factor = (double) ((int) 1<<15) ;
	factor -=1.0f;

	factor = factor / (double) max;
	if (factor <=1.0f)
	{
		Trace("Audio already fully normalised, skipping step.");
		return ;
	}

	Trace("Multipling signal by %f", factor);

	for (i=0;i<length;i+=2)
	{
		if (mAbort==true) 
		{
			return ;
		}
		short current =0;
		mBuffer->GetData(i, (unsigned char *)&current, 2);

		double currentf = (double) current;
		currentf *=factor;

		if (currentf > 32767.0f) 
		{
			currentf = 32767.0f;
		}

		if (currentf < -32767.0f)
		{
			currentf = -32767.0f;
		}

		current = (short) currentf ;
		mBuffer->WriteData(i, (unsigned char *)&current, 2);

	}
}
