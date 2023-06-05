// AudioMixer.cpp: implementation of the CAudioMixer class.
//
//////////////////////////////////////////////////////////////////////

#include "AudioMixer.h"
#include "FileBufferCache.h"
#include "AudioNormaliser.h"
#include "UnmanagedErrors.h"
#include <math.h>

bool CAudioMixer::mAbort=false;

extern double db_ratio(double ratio);

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CAudioMixer::CAudioMixer(int frequency) : mFrequency(frequency)
{
}

// ******************************************************************************************************
CAudioMixer::~CAudioMixer()
{
}

// ******************************************************************************************************
void CAudioMixer::AddStream(CBuffer* buffer, float volume)
{
	mStreams.push_back(CStream(buffer, volume));
}

// ******************************************************************************************************
// Returns which stream resulting mix is in
int CAudioMixer::Process()
{
	mAbort=false;

	//
	// Create memory buffer to largest stream;
	//
	Trace("Mixing streams");
	StreamIterator iter;

	__int64 biggest_size =0;
	int big_stream_no = 0;

	int count2=0;
	for ( iter = mStreams.begin(); iter != mStreams.end(); ++iter )
    {
		__int64 size = (*iter).mBuffer->GetSize() ;//+ (int)((*iter).mOffset * mFrequency * 4);
		if (size > biggest_size)
		{
			biggest_size = size ;
			big_stream_no=count2;
		}
		count2++;
	}

	Trace("Out mixed file will be %d bytes long", biggest_size);

	int cache_size = 1024*1024*1;

	int num_streams = (int) mStreams.size();

	// calculate vol coefficient to apply to each stream volume ( i.e such that when mixed together nothing
	// will be more that 1.  Each stream can have a volume of 0...1
	float total_vols = 0;
	for ( iter = mStreams.begin(); iter != mStreams.end(); ++iter )
    {
		CStream& s = (*iter);
		total_vols += s.mVolume;
	}

	float coef = 1/total_vols;

	float total_vols2 = 0;
	for ( iter = mStreams.begin(); iter != mStreams.end(); ++iter )
    {
		CStream& s = (*iter);
		float ratio =  (float) db_ratio(s.mVolume * coef);
		total_vols2 += ratio;
	}

	float coef2 = 1/total_vols2;

	CFileBufferCache* mb = NULL;

	//
	// Loop round twice
	// First loop process biggest stream only and make output stream
	//
	for (int jj=0;jj<2;jj++)
	{
		int count=0;

		for ( iter = mStreams.begin(); iter != mStreams.end(); ++iter )
		{
			if ((jj==1 && count != big_stream_no) ||
				(jj==0 && count == big_stream_no) )
			{
				bool master_cache=false;

				CStream& s = (*iter);

				//
				// Ok first buffer is the output buffer
				//
				if (count==big_stream_no)
				{
					mb = (CFileBufferCache*) s.mBuffer;
					master_cache=true;
				}

				if (mb==NULL)
				{
					Error("Master cache when mixing audio was null");
					mb = (CFileBufferCache*) s.mBuffer;
					master_cache=true;
				}

			
				Trace("Processing stream %s...\n", s.mBuffer->GetIDString() );

				CBuffer& fb = *(*iter).mBuffer;

				__int64 to_process = fb.GetSize();
				int max_cache_size = fb.GetCacheSize();

				unsigned char* temp_buffer = new unsigned char[max_cache_size];

				__int64 index = 0 ;//(int)(s.mOffset * mFrequency * 4);

				__int64 i_index = 0;

				while (to_process > 0)
				{
					if (mAbort==true)
					{
						to_process=0;
					}
					else
					{
						int this_process = max_cache_size;
						if (to_process < max_cache_size)
						{
							this_process = (int)to_process ;
						}

						fb.GetData(i_index, temp_buffer, this_process);

						double vol = db_ratio(s.mVolume* coef);

						MixAudio(index,temp_buffer,this_process, mb, (float) vol * coef2, master_cache);
					
						to_process -=this_process;
						index+=this_process;
						i_index+=this_process;
					}
				}

				delete [] temp_buffer;
			}
			count++;
		}
	}

	if (mAbort==false)
	{
		AudioNormaliser normaliser(mb);
		normaliser.Process();
	}

	return big_stream_no;
}

// ******************************************************************************************************
void CAudioMixer::MixAudio(__int64 index, unsigned char* from_buffer, int amount, CFileBufferCache* to_buffer, float volume_coef, bool master_cache)
{
	mAbort=false;

	//
	// Align to 16 bit audio
	//
	if (amount %2 !=0)
	{
		Warning("Missing byte in audio when mixing streams, is it 16 bit?");
		amount--;
	}

	for (int i=0;i<(amount>>1);i++)
	{
		if (mAbort==true)
		{
			return ;
		}

		short data = *((short*)from_buffer);
		double dataf = (double) data;

		dataf *= volume_coef;
		data = (short) dataf;

		short current ;
		to_buffer->GetData(index, (unsigned char *)&current, 2);

		int cc = (int)current;

		if (master_cache==true)
		{
			cc = data;
		}
		else
		{
			cc+=data;
		}

		if (cc > 32767)
		{
			cc= 32767;
		}

		if (cc <-32767)
		{
			cc =-32767;
		}
 
		current = (short) cc;
		to_buffer->WriteData(index, (unsigned char*)&current ,2);

		index+=2;
		from_buffer+=2;
	}
}




