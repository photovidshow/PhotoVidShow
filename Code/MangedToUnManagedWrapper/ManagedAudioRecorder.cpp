// AUDIO Recorder

#include "AudioRecorder.h"

#include "ManagedErrors.h"
#include <iostream>
using namespace std; 
#using <mscorlib.dll>


#include <tchar.h>

using namespace System;
using namespace System::Text;
using namespace System::Runtime::InteropServices; 
using namespace ManagedCore;
using namespace std;


extern std::string ManagedToSTL(String^ a);
extern System::String^ STLToManaged(std::string stl);

#include "ManagedObject.h"


namespace MangedToUnManagedWrapper
{ 
	
	 public ref class CManagedAudioRecorder : public ManagedObject
	{
	public:
	
		AudioRecorder* mAR;

		CManagedAudioRecorder();

		~CManagedAudioRecorder();

		void CleanUp();

		void Record(String^ outputWavFilename, int selectedDevice, int echoRecording);
		void Stop();
		void Pause();
		void Continue();

		String^ GetFirstDeviceName();
		String^ GetNextDeviceName();

	};

    //*******************************************************************
	void CManagedAudioRecorder::CleanUp()
	{
	    delete mAR;
		mAR = NULL;
		Release();
	}

    //*******************************************************************
	CManagedAudioRecorder::CManagedAudioRecorder()
	{
		mAR = new AudioRecorder();
	}

	//*******************************************************************
	CManagedAudioRecorder::~CManagedAudioRecorder()
	{
		if (mAR!=NULL)
		{
			delete mAR;
			mAR=NULL;
		}
	}

	//*******************************************************************
	void CManagedAudioRecorder::Record(String^ outputWavFilename, int selectedDevice, int echoRecording)
	{
		std::string output = ManagedToSTL(outputWavFilename);
		mAR->Record((char*)output.c_str(), selectedDevice, echoRecording);
	}

	//*******************************************************************
	void CManagedAudioRecorder::Stop()
	{
		return mAR->Stop();
	}

	//*******************************************************************
	void CManagedAudioRecorder::Pause()
	{
		mAR->Pause();
	}

	//*******************************************************************
	void CManagedAudioRecorder::Continue()
	{
		mAR->Continue();
	}

	//*******************************************************************
	String^ CManagedAudioRecorder::GetFirstDeviceName()
	{
		return STLToManaged(mAR->GetFirstDeviceName());
	}

	//*******************************************************************
	String^ CManagedAudioRecorder::GetNextDeviceName()
	{
		return STLToManaged(mAR->GetNextDeviceName());
	}
}


