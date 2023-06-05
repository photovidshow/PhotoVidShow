
#include "AudioRipper.h"
#include "ManagedErrors.h"

#include <iostream>
#include <string>
using namespace std; 
#using <mscorlib.dll>


#include <tchar.h>

using namespace System;
using namespace System::Text;
using namespace System::Runtime::InteropServices; 

extern std::string ManagedToSTL(String^ a);

#include "ManagedObject.h"


namespace MangedToUnManagedWrapper
{ 
	 
	public ref class CAudioRipResultsInformation
	{
	public:
		int mFrequency;
		int mBytesPerSample;
		bool mStereo;
		bool mReSamplingOccuredDuringRipping;

		CAudioRipResultsInformation::CAudioRipResultsInformation()
		{
			mFrequency=0;
			mBytesPerSample=0;
			mStereo=false;
			mReSamplingOccuredDuringRipping=false;
		}
	};
 
    public ref class CManagedAudioRipper 
	{
	public:
		
		static bool mDoingRip = false;
		static String^ mRippingFile = nullptr;
		static String^ mOutFile = nullptr;
		static bool mBuildingCodecArray = false;
	//	static int mDesiredResampleFrequency = 0;
		static double mRipLength =0;
	
		static CAudioRipResultsInformation^ RipMediaFile(String^ in_media_file,
			                     String^ out_ripped_file, 
								 int desired_resample_audio_frequency,
								 double length_in_seconds);

		static void Abort();


	};


    CAudioRipResultsInformation^ CManagedAudioRipper::RipMediaFile(String^ in_media_file,
			                     String^ out_ripped_file, 
								 int desired_resample_audio_frequency,
								 double length_in_seconds)
	{
		mRippingFile = dynamic_cast<String^>(in_media_file->Clone());
		mOutFile = dynamic_cast<String^>(out_ripped_file->Clone());

		if (mRippingFile==nullptr ||
			mOutFile== nullptr)
		{
			return nullptr;
		}

		CAudioRipResultsInformation^ ai = gcnew CAudioRipResultsInformation();

		mRipLength=length_in_seconds;
	//	mDesiredResampleFrequency=desired_resample_audio_frequency;
		std::string in = ManagedToSTL(mRippingFile);
		std::string out = ManagedToSTL(mOutFile);
		mDoingRip=true;


		mBuildingCodecArray=true;
		CAudioRipper::BuildCodecArray();
		mBuildingCodecArray=false;

		int res =0;

		int attempts =0;

		bool loaded_or_cant = false;
		bool useLav=true;	

		while (loaded_or_cant==false && attempts<20)
		{
			attempts++;

			try
			{
			
				int out_frequency=0;
				int out_bytes_per_sample=0;
				int out_stereo=0;
				res = CAudioRipper::BuildAndRunGraph(
										   (char*)in.c_str(), 
										   (char*)out.c_str(),
										   desired_resample_audio_frequency,
										   &out_frequency,
										   &out_bytes_per_sample,
										   &out_stereo,
										   useLav);

				//
				// Check if no audio renderer detected in graph
				//
				if (res==4)
				{
					//
					// If using lav, try again and let DS decide
					//
					if (useLav == true)
					{
						useLav = false;
						continue;
					}
				}
				else if (res!=1)
				{
					ai->mBytesPerSample = out_bytes_per_sample;
					ai->mFrequency = out_frequency;
					ai->mStereo = out_stereo==1;
					if (res==0)
					{
						ai->mReSamplingOccuredDuringRipping=true;
					}
				}
			
				loaded_or_cant=true;
			}
			catch(...)
			{
				if (ManagedCore::CDebugLog::GetInstance()->DebugMode==true)
				{
				  ManagedPopupError("CAudioRipper::RenderFile crashed");
				}
			
				ManagedError("CAudioRipper::RenderFile probably crashed"  );

			}
	
		}

		mDoingRip=false;
		if (res==1) return nullptr;

		return ai;
	}

	void CManagedAudioRipper::Abort()
	{
		CAudioRipper::SetRipAbortFlag(1);
	}
}



