// AudioResampler.cpp: implementation of the CAudioResampler class.
//
//////////////////////////////////////////////////////////////////////
#define _CRT_SECURE_NO_WARNINGS 1
#include "AudioResampler.h"
#include "FileBufferCache.h"
#include "AudioNormaliser.h"
#include "UnmanagedErrors.h"
#include <math.h>

bool CAudioResampler::mAbort=false;

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////


//****************************************************************************
CAudioResampler::CAudioResampler(CBuffer* buffer, int frequency, int bytes_per_sample, bool stereo) : 
	mFrequency(frequency),
	mBytesPerSample(bytes_per_sample),
	mStereo(stereo),
	mBuffer(buffer)
{
}


//****************************************************************************
CAudioResampler::~CAudioResampler()
{
}

//****************************************************************************
/*
for samples a, b, and c, where:
x is a value between b and c;
k is a linear error for predicting c;
f is a curve effecting the weight of k in the result (note, the 0.125 was
trial and errored out as well, and may have something to do with "slope" for
all I know, then again, it may be a constant, the actual slope being
contained entirely in k);
g is the prediction (a modified linear prediction, f*k gives the predicted
error).
*/


/*
x=0.25;
k=(c-(2*b-a));
f=(0.125*(1.0-4*(x-0.5)*(x-0.5)));
g=b*(1-x) + c*x - k*f;
*/

//***************************************************************************
bool CAudioResampler::CreateFileCache(__int64 total_size, char* new_fn)
{
	CAudioResampler::mAbort=false;

	FILE* fp = fopen(new_fn,"wb");

	if (fp==NULL)
	{
		Error("Unable to create resample audio file '%s'", new_fn);
		return false;
	}

	int b_s = 1024*128;

	unsigned char*m = new unsigned char[b_s];
	memset(m,0,b_s);

	while (total_size > b_s && CAudioResampler::mAbort==false)
	{
		fwrite(m,1,b_s,fp);
		total_size -= b_s;
	}

	if (total_size >0 && CAudioResampler::mAbort==false)
	{
		fwrite(m,1,(int)total_size,fp);
	}
	fclose(fp);
	delete [] m;
	return true;
}

//***************************************************************************
void CAudioResampler::Process(int new_frequency, char* new_fn, int size_of_header)
{
	mAbort=false;

	//
	// Create memory buffer for resampled buffer
	//
	__int64 original_buffer_size = mBuffer->GetSize();

	original_buffer_size-=size_of_header;

	double ratio = ((double)new_frequency)/ ((double)mFrequency) ; //  for 44100 -> 48000 = 1.0884353741496598639455782312925;

	double one_over_ratio = 1/ ratio;

	double size = (double) original_buffer_size;
	size/=mBytesPerSample;
	if (mStereo==true) size/=2;

	size *= ratio;
	size *= 4 ;  //(stereo, 16 bit)

	__int64 new_buffer_size = (__int64)size;
	
	int cache_size = 1024*1024*1;

	if (CreateFileCache(new_buffer_size+size_of_header, new_fn)==false)
	{
		return;
	}

	CFileBufferCache* mb = new CFileBufferCache(std::string(new_fn),cache_size);

	if (size_of_header > 0)
	{
		unsigned char* header_buffer = new unsigned char[size_of_header];
		mBuffer->GetData(0, header_buffer, size_of_header);
		mb->WriteData(0, header_buffer, size_of_header);
		delete [] header_buffer;
	}

	int size_of_full_sample = mBytesPerSample;
	if (mStereo) 
	{
		size_of_full_sample*=2;
	}

	int size_of_3_full_samples = size_of_full_sample * 3;
	double size_of_full_sample_double = size_of_full_sample;

	for (__int64 i=0;i< new_buffer_size;i+=4)
	{
		double sample_a =0;
		double sample_b = 0;
		double sample_c = 0;
		double sample_ar = 0;
		double sample_br = 0;
		double sample_cr = 0;

		double pos_in_old_buffer_if_16_bit_stereo = ((double)i)* one_over_ratio;
		__int64 pos_in_old_buffer_int = (__int64) (pos_in_old_buffer_if_16_bit_stereo);

		pos_in_old_buffer_int >>=2;
		__int64 pos_in_old_buffer_int_if_16_bit_stero =  pos_in_old_buffer_int << 2;

		if (mBytesPerSample==2)
		{
			pos_in_old_buffer_int <<=1;
		}

		if (mStereo) 
		{
			pos_in_old_buffer_int <<=1;
		}
		
		unsigned char samples[4*12] ;
		short* samples_short = (short*)samples;

		if (pos_in_old_buffer_int>=size_of_full_sample)
		{
			mBuffer->GetData(size_of_header+pos_in_old_buffer_int-size_of_full_sample, (unsigned char *)&samples, size_of_3_full_samples);
		}
		else
		{
			mBuffer->GetData(size_of_header+pos_in_old_buffer_int, (unsigned char *)&samples, size_of_3_full_samples);
		}

		if (mBytesPerSample==2 && mStereo)
		{
			sample_a = (double) samples_short[0];
			sample_b = (double) samples_short[2];
			sample_c = (double) samples_short[4];

			sample_ar = (double) samples_short[1];
			sample_br = (double) samples_short[3];
			sample_cr = (double) samples_short[5];
		}
		else if (mBytesPerSample==2 && !mStereo)
		{
			sample_a = (double) samples_short[0];
			sample_b = (double) samples_short[1];
			sample_c = (double) samples_short[2];

			sample_ar = sample_a;
			sample_br = sample_b;
			sample_cr = sample_c;
		}
		else if (mBytesPerSample==1 && mStereo)
		{
			sample_a = (double) samples[0] ;
			sample_b = (double) samples[2] ;
			sample_c = (double) samples[4] ;

			sample_ar = (double) samples[1];
			sample_br = (double) samples[3];
			sample_cr = (double) samples[5];
		}
		else if (mBytesPerSample==1 && !mStereo)
		{
			sample_a = (double) samples[0];
			sample_b = (double) samples[1];
			sample_c = (double) samples[2];

			sample_ar = sample_a;
			sample_br = sample_b;
			sample_cr = sample_c;
		}

		double fract = (pos_in_old_buffer_if_16_bit_stereo - ((double)pos_in_old_buffer_int_if_16_bit_stero)) / 4.0;
		double f=(0.125*(1.0-4*(fract-0.5)*(fract-0.5)));

		double k=(sample_c-(2*sample_b-sample_a));
		double g=sample_b*(1-fract) + sample_c*fract - k*f;

		double k1=(sample_cr-(2*sample_br-sample_ar));
		double g1=sample_br*(1-fract) + sample_cr*fract - k*f;

		// scale 8 bit up to 16 bit audio
		if (mBytesPerSample==1)
		{
			g=g *256;
			g=g-32768.0;
			g1=g1 *256.0;
			g1=g1-32768.0; 
		}

		// ok some naughty tracks already have distortion in them which can cause this resample curve algorithm
		// to produce values above 16 bit range. therefore we need to cap values (i.e keep distortion)
		if (g <-32768) g=-32768;
		if (g1 <-32768) g1=-32768;
		if (g >32767) g=32767;
		if (g1 >32767) g1 = 32767;

		samples_short[0] = (short) g;
		samples_short[1] = (short) g1;
	
		mb->WriteData(i+size_of_header,(unsigned char*)samples, 4);

	}
 
	if (mb)
	{
		delete mb ;
	}
}






