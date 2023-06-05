
//////////////////////////////////////////////////////////////////////


#include "AudioFadeOutSection.h"

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
	public ref class CManagedAudioFadeOutSection : public ManagedObject
	{
	public:

		AudioFadeOutSection* mAF;
	
		CManagedAudioFadeOutSection(CManagedBuffer^ buffer, int frequency);
		~CManagedAudioFadeOutSection();
		void Process(double startOffset, double length, float fadeRatio, float fadeDownLength, float fadeUpLength,float fadeUpEndRatio );
	};
	
	CManagedAudioFadeOutSection::CManagedAudioFadeOutSection(CManagedBuffer^ buffer, int frequency)
	{
		mAF = new AudioFadeOutSection(buffer->mBuffer, frequency);
	}

	CManagedAudioFadeOutSection::~CManagedAudioFadeOutSection()
	{
		delete mAF;
	}

	void CManagedAudioFadeOutSection::Process(double startOffset, double length, float fadeRatio, float fadeDownLength, float fadeUpLength, float fadeUpEndRatio)
	{
		mAF->Process(startOffset, length, fadeRatio, fadeDownLength, fadeUpLength, fadeUpEndRatio);
	}
}



