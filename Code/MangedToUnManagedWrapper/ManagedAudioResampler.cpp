/////////////////////////////////////////////////////////////////////////////////////////////

#include "AudioResampler.h"

#include <iostream>
#include <string>
using namespace std; 
#using <mscorlib.dll>


#include <tchar.h>

using namespace System;
using namespace System::Text;
using namespace System::Runtime::InteropServices; 
using namespace ManagedCore;
using namespace std;



extern std::string ManagedToSTL(String^ a);

#include "ManagedObject.h"
#include "ManagedFileBufferCache.h"

namespace MangedToUnManagedWrapper
{ 
	public ref class CManagedAudioResampler : public ManagedObject
	{
	public:
	
		static String^ mNewFile = nullptr;

		CAudioResampler* mAR;

		CManagedAudioResampler(CManagedBuffer^ buffer, int frequency, int bytes_per_sample, int stereo);

		~CManagedAudioResampler();

		// returns the which stream the result is in
		void Process(String^ in_media_file, int new_frequency, int size_of_header);

		static void Abort();

	};

	CManagedAudioResampler::CManagedAudioResampler(CManagedBuffer^ buffer,
		                                           int frequency,
												   int bytes_per_sample,
												   int stereo)
	{
		mAR = new CAudioResampler(buffer->mBuffer, frequency, bytes_per_sample, stereo == 1 ? true : false);
	}

	CManagedAudioResampler::~CManagedAudioResampler()
	{
		delete mAR;
	}

	void CManagedAudioResampler::Process(String^ in_media_file,
										 int new_frequency,
										 int size_of_header)
	{
		mNewFile = dynamic_cast<String^>(in_media_file->Clone());
		std::string in = ManagedToSTL(mNewFile);
		mAR->Process(new_frequency, (char*)in.c_str(), size_of_header);
	}

	void CManagedAudioResampler::Abort()
	{
		CAudioResampler::mAbort=true;
	}
}




