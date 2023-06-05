

#include "FileBufferCache.h"
#include "AudioApplyDBLoss.h"
#include "UnmanagedErrors.h"
#include <math.h>

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

//**********************************************************************
AudioApplyDBLoss::AudioApplyDBLoss(CBuffer* buffer, int frequnecy) :
	mBuffer(buffer), mFrequency(frequnecy)
{
}

//**********************************************************************
AudioApplyDBLoss::~AudioApplyDBLoss()
{
}

extern double db_ratio(double ratio);

//**********************************************************************
void AudioApplyDBLoss::Process(float offset, float length, float vol)
{
	__int64 size = mBuffer->GetSize();
	if (size%2!=0)
	{
		Warning("Missing byte in audio data when doing fade process is it 16 bit?");
		length--;
	}

	double l = ((double)size) /  (((double)mFrequency) * 4.0);

	if (offset+length > l || offset < 0.0f)
	{
		if (offset > l)
		{
			Error("Audio fade process outside buffer range. Ignoring process");
			return ;
		}

		if (offset < 0) 
		{
			Error("Audio fade process outside buffer range. Setting to start");
			offset =0 ;
		}

		if (offset+length > l)
		{
			Warning("Audio fade process partially outside buffer range");
		//	length = (float) l - offset;
		}
	}

	double d_offset = offset;

	__int64 start = (__int64) (d_offset * mFrequency * 4);
	__int64 end = (__int64) ((d_offset+length) *(mFrequency * 4));

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

	double multi = db_ratio(vol);
		
	if (multi <0.0f) multi = 0.0f;
	if (multi >1.0f) multi = 1.0f;

	for (__int64 i=start;i<end;i+=2)
	{
		short current ;
		mBuffer->GetData(i, (unsigned char *)&current, 2);

		double ss = (double) current;
		ss *=multi;
		current = (short) ss;

		mBuffer->WriteData(i, (unsigned char *)&current, 2);
	}
}

