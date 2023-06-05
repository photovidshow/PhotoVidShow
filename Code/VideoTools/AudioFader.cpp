// AudioFader.cpp: implementation of the AudioFader class.
//
//////////////////////////////////////////////////////////////////////

#include "FileBufferCache.h"
#include "AudioFader.h"
#include "UnmanagedErrors.h"
#include <math.h>

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////


//**********************************************************************
AudioFader::AudioFader(CBuffer* buffer, int frequnecy) :
	mBuffer(buffer), mFrequency(frequnecy)
{

}


//**********************************************************************
AudioFader::~AudioFader()
{

}

double db_ratio(double ratio)
{
	double r = 1.0 - ratio;

	if (r <0.0f) r =0.0f;
	if (r >1.0f) r =1.0f;

	double db_loss = (1.0f - r) * 1.8f;
	db_loss = 0.0f - ( ((double)pow(10.0, db_loss)) * 155.23f) ;

/*
	double db_loss = (1.0f - r) * 2.0f;
	db_loss = 0.0f - ( ((double)pow(10.0, db_loss)) * 100.0f) ;
	*/

	if (db_loss < -9800.0f) db_loss = -9800.0f;
	if (db_loss >0) db_loss = 0.0f;

	return 1.0 - ((db_loss+9800)/9800) ;
}

//**********************************************************************
void AudioFader::Process(FadeType type, float offset, float length)
{
	__int64 size = mBuffer->GetSize();
	if (size%2!=0)
	{
		Warning("Missing byte in audio data when doing fade process is it 16 bit?");
		size--;
	}

	double l = ((double)size) /  (((double)mFrequency) * 4.0);

	if (offset+length > l || offset < 0.0f)
	{
		if (offset > l)
		{		
			//
			// The audio mixed buffer may be shorter than slideshow as the auido buffer gets padded later.  Nothing to do
			//
			return ;
		}

		if (offset < 0) 
		{
			Error("Audio fade process outside buffer range. Setting to start");
			offset =0 ;
		}

		if (offset+length > l)
		{
			// SRG FIX ME G858 (This is because audio mixed buffer may be shorter than slideshow - the auido buffer gets padded later)
			//	Warning("Audio fade process partially outside buffer range");
			length = (float) l - offset;
		}
	}

	double d_offset = offset;

	__int64 start = (__int64) (d_offset * mFrequency * 4);
	__int64 end = (__int64) ((d_offset+length) *(mFrequency * 4));

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


	double ll = (double) end - start;

	for (__int64 i=start;i<end;i+=2)
	{
		double delta = ((double)(i-start))/ll ;	
		double ratio = db_ratio(delta);
		double multi = 1.0f * ratio;
			
		if (type == FADE_OUT)
		{
			multi = db_ratio(1.0 -delta);
		}

		if (multi <0.0f) multi = 0.0f;
		if (multi >1.0f) multi = 1.0f;

		short current ;
		mBuffer->GetData(i, (unsigned char *)&current, 2);

		double ss = (double) current;
		ss *=multi;
		current = (short) ss;

		mBuffer->WriteData(i, (unsigned char *)&current, 2);
	
	}

	if (type == FADE_OUT)
	{
		// if fade out continue till end of buffer
		short blank =0;
		for (__int64 i=end;i<size;i+=2)
		{
			mBuffer->WriteData(i, (unsigned char *)&blank, 2);
		}
	}
}


