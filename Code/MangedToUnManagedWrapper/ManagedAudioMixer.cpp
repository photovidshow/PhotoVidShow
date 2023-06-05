/////////////////////////////////////////////////////////////////////////////////////////////

#include "AudioMixer.h"

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


#include "ManagedObject.h"
#include "ManagedFileBufferCache.h"


namespace MangedToUnManagedWrapper
{ 
	public ref class CManagedAudioMixer : public ManagedObject
	{
	public:
	
		CAudioMixer* mAM;

		CManagedAudioMixer(int frequency);

		~CManagedAudioMixer();

		void  AddStream(CManagedBuffer^ buffer, float volume);

		// returns the which stream the result is in
		int Process();

		static void Abort();

	};

	CManagedAudioMixer::CManagedAudioMixer(int frequency)
	{
		mAM = new CAudioMixer(frequency);
	}

	CManagedAudioMixer::~CManagedAudioMixer()
	{
		delete mAM;
	}

	void  CManagedAudioMixer::AddStream(CManagedBuffer^ buffer, float volume)
	{
		mAM->AddStream(buffer->mBuffer, volume);
	}

	// returns the which stream the result is in
	int CManagedAudioMixer::Process()
	{
		return mAM->Process();
	}

	void CManagedAudioMixer::Abort()
	{
		CAudioMixer::mAbort=true;
	}
}




