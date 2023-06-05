// AUDIO PLAYER

#include "AudioPlayer.h"

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

#include "ManagedObject.h"


namespace MangedToUnManagedWrapper
{ 
	
    public ref class CAudioPlayer : public ManagedObject
	{
	public:
	
		CAudioPlayer(String^ filename);
		~CAudioPlayer();

		void Play();
		long GetVolume();
		void SetVolume(long vol);
		void Stop(double start_offset, bool wait);
		void SeekToTime(double time, bool wait_for_completion);
		void Pause();
		double GetPlaybackPositionInSeconds();
		Int64 GetDuration();
		double GetDurationInSeconds();

	private:
		void ReleaseAndNullAll();
		void CleanUp();         // Deletes c++ AudioPlayer, must not use object again except to release

		AudioPlayer* mAP;

	};


	// ************************************************************************************************************
	CAudioPlayer::CAudioPlayer(String^ filename)
	{
		std::string mName = ManagedToSTL(filename);
		mAP = new AudioPlayer(mName);

		bool graph_throws_exception = false;
		bool cant_build_graph =false;

		try
		{
			if (mAP->BuildGraph()==false)
			{
				ReleaseAndNullAll();
				cant_build_graph = true;
			}
		}
		catch(...)
		{
			graph_throws_exception = true;

			if (ManagedCore::CDebugLog::GetInstance()->DebugMode==true)
			{
				ManagedPopupError("AudioPlayer::RenderFile crashed");
			}
				
			char temp[1024];
			sprintf(temp,"CAudioPlayer::RenderFile probably crashed for file %s", (char*)mName.c_str());
			ManagedError(temp);
			ReleaseAndNullAll();		
		}

		//
		// Something went wrong? report it via an exception
		//
		if (graph_throws_exception==true || cant_build_graph)
		{
			throw gcnew ErrorException("Could not load AudioPlayer");
		}
	}

	// ************************************************************************************************************
	// Internal method which allows us to call BuildGraph on the c++ audio player again without having
	// to do a delete and new
	void CAudioPlayer::ReleaseAndNullAll()
	{
		try
		{
			mAP->ReleaseAndNullAll();
		}
		catch (...)
		{
			if (ManagedCore::CDebugLog::GetInstance()->DebugMode == true)
			{
				ManagedPopupError("AudioPlayer::ReleaseAndNullAll crashed");
			}

			ManagedError("AudioPlayer::ReleaseAndNullAll crashed");
		}
	}


	// ************************************************************************************************************
	CAudioPlayer::~CAudioPlayer()
	{
       CleanUp();
	}


	// ************************************************************************************************************
    void CAudioPlayer::CleanUp()
    {
        if (mAP!=NULL)
        {
		    delete mAP;
            mAP=NULL;
        }
    }


	// ************************************************************************************************************
	void CAudioPlayer::Play()
	{
		mAP->Play();
	}


	// ************************************************************************************************************
	long CAudioPlayer::GetVolume()
	{
		return mAP->GetVolume();
	}


	// ************************************************************************************************************
	void CAudioPlayer::SetVolume(long vol)
	{
		mAP->SetVolume(vol);
	}


	// ************************************************************************************************************
	void CAudioPlayer::Stop(double start_offset, bool wait)
	{
	
		SeekToTime(start_offset,wait);
		mAP->Stop();
	}


	// ************************************************************************************************************
	void CAudioPlayer::SeekToTime(double time, bool wait_for_completion)
	{
		mAP->SeekToTime(time, wait_for_completion);
	}


	// ************************************************************************************************************
	void CAudioPlayer::Pause()
	{
		mAP->Pause();
	}


	// ************************************************************************************************************
	double CAudioPlayer::GetPlaybackPositionInSeconds()
	{
		return mAP->GetPlaybackPositionInSeconds();
	}


	// ************************************************************************************************************
	Int64 CAudioPlayer::GetDuration()
	{
		return mAP->GetDuration();
	}


	// ************************************************************************************************************
	double CAudioPlayer::GetDurationInSeconds()
	{
		Int64 d = GetDuration();
		double d1 =  (double)d;
		d1 = d1/ 10000000.0f;

		return d1;
	}
}


