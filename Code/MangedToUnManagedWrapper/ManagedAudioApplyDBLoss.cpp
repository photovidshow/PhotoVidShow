
//////////////////////////////////////////////////////////////////////


#include "AudioApplyDBLoss.h"

#include <iostream>
#include <string>
using namespace std; 
#using <mscorlib.dll>


#include <tchar.h>
#include "ManagedFileBufferCache.h"

using namespace System;
using namespace System::Text;
using namespace System::Runtime::InteropServices; 
using namespace ManagedCore;
using namespace std;


#include "ManagedObject.h"


namespace MangedToUnManagedWrapper
{ 
	public ref class CManagedAudioApplyDBLoss : public ManagedObject
	{ 
		public:
 
 
		AudioApplyDBLoss* mADBloss;
	
		CManagedAudioApplyDBLoss(CManagedBuffer^ buffer, int frequency);
		~CManagedAudioApplyDBLoss();
 
		void Process(float offset, float length,float vol);
	};
 

	CManagedAudioApplyDBLoss::CManagedAudioApplyDBLoss(CManagedBuffer^ buffer, int frequency)
	{
	  mADBloss = new AudioApplyDBLoss(buffer->mBuffer, frequency);
	}

	CManagedAudioApplyDBLoss::~CManagedAudioApplyDBLoss()
	{
		delete mADBloss;
	}

	void CManagedAudioApplyDBLoss::Process(float offset, float length,float vol)
	{
		mADBloss->Process(offset,length,vol);
	}
}
