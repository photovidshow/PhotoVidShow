
//////////////////////////////////////////////////////////////////////


#include "AudioFader.h"

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
	public ref class CManagedAudioFader : public ManagedObject
	{
	public:

		enum class ManagedFadeType
		{
			FADE_IN,
			FADE_OUT
		};


		AudioFader* mAF;
	
		CManagedAudioFader(CManagedBuffer^ buffer, int frequency);
		~CManagedAudioFader();
		void Process(ManagedFadeType type, float offset, float length);
	};

	
	CManagedAudioFader::CManagedAudioFader(CManagedBuffer^ buffer, int frequency)
	{
		mAF = new AudioFader(buffer->mBuffer, frequency);
	}

	CManagedAudioFader::~CManagedAudioFader()
	{
		delete mAF;
	}

	void CManagedAudioFader::Process(ManagedFadeType type, float offset, float length)
	{
		mAF->Process(((AudioFader::FadeType)type), offset,length);
	}
}



