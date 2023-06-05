#include "../Burner/DVDDisk.h"
#include "../Burner/VideoDiskManager.h"

/////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////

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


extern System::String^ STLToManaged(std::string stl);
extern std::string ManagedToSTL(System::String^ managed);


using namespace Burner;

#include "ManagedErrors.h"

namespace MangedToUnManagedWrapper
{ 

	class VideoCallback : public IBurningProgressCallback
	{
	public:

		void(*PerctangeDoneCB)(int);
		void(*BufferStatusCallbackfp)(int, int, int);
		void(*BurnPadStartedCB)();

		VideoCallback() {}

		VideoCallback( void(*pd)(int), void(*bsc)(int, int, int) , void(*bps)())
		{
			PerctangeDoneCB=pd;
			BufferStatusCallbackfp=bsc;
			BurnPadStartedCB=bps;
		}



		virtual void BufferStatusCallback(int percent_done,
										  int bufferFreeSizeInUCHARs,
										  int BufferSizeInUCHARs);

		virtual void PercentageDone(int p);


		virtual void BurnPadStarted();
	};


	void BurnPerctageDone(int percent);
	void BufferStatusCallback(int percent_done,
							  int bufferFreeSizeInUCHARs,
							  int BufferSizeInUCHARs);

	void BurnPadStarted();

	public ref class IVideoDiskCallback
	{
	public:
		virtual void BurnPercentageComplete(int amount) = 0;
		virtual void BufferStatusCallback(int percent_done, int bufferFreeSizeInUCHARs, int BufferSizeInUCHARs) = 0;
		virtual void PadStartedCallback() = 0;
	};



	public ref class CManagedVideoDiskManager
	{

		private:
		
		// dont delete this
		CVideoDiskManager* mInstance;
		
	public:
		static IVideoDiskCallback^ mCurrentVideoDiskCallback;

		CManagedVideoDiskManager(IVideoDiskCallback^ callback);

		bool RebuildAvailbleVideoDevicesList();

		void LogAvailbleVideoDevicesList();
	
		System::Collections::ArrayList^ GetVideoDiskDevices();

	

	};


//	static IVideoDiskCallback* mCurrentVideoDiskCallback ;

	public enum class EManagedDiskType
	{
		M_CD,
		M_DVD
	};


	public ref class CManagedVideoDisk
	{
	private:
		CVideoDisk* mUnmanagedPart;
		
	public:


		CManagedVideoDisk(CVideoDisk* disk)
		{
			mUnmanagedPart = disk;
		}

		bool IsSuspectedBluRayDrive()
		{
			return mUnmanagedPart->IsSuspectedBluRayDrive();
		}

		bool WritesCD()
		{
			return mUnmanagedPart->mWritesCD;
		}

		bool WritesDVD()
		{
			return mUnmanagedPart->mWritesDVD;
		}

		System::String^ Name()
		{
			String^ aa = STLToManaged(mUnmanagedPart->mName);
			return aa;
		}

		System::String^ DeviceName()
		{
			String^ aa = STLToManaged(mUnmanagedPart->mDeviceName);
			return aa;
		}
			
		int Burn(String^ root_folder, 
				  String^ volume_name,
				  String^ orgFilesZipSource,
				  int number_org_files,
				  int minute,
				  int hour,
				  int day,
				  int month,
				  int year,
				  bool do_padding,
				  bool from_bin_cue_file)
		{

			VideoCallback* disk_callback = new VideoCallback(&BurnPerctageDone, &BufferStatusCallback, &BurnPadStarted);

			std::string s1 = ManagedToSTL(root_folder);
			std::string s2 = ManagedToSTL(orgFilesZipSource);
			std::string s3 = ManagedToSTL(volume_name);

			std::vector<std::string> org_file_zip ;
			
			if (s2.length() > 1 && number_org_files > 0)
			{
				for (int ii=0;ii< number_org_files;ii++)
				{
					char sss[10] = {0};
					sprintf(sss,"%d.zip", (ii+1));
					std::string is = s2 + sss;
					org_file_zip.push_back(is);
				}
			}

			int errors = mUnmanagedPart->Burn((char*)s1.c_str(),
											  (char*)s3.c_str(),
												 minute,
												 hour,
												 day,
												 month,
												 year,
												 org_file_zip,disk_callback,do_padding,0, from_bin_cue_file);
			return errors;
		}

		void AbortBurn()
		{
			mUnmanagedPart->AbortBurn();
		}

		int GetWriteSpeed()
		{
			return mUnmanagedPart->mWriteSpeed;
		}

		int EraseDisk()
		{
			return mUnmanagedPart->EraseDisk();
		}


		 // return: 0 means it was not blank, 1 means it was blank, 2 means it was not blank but is erasable
		 int ContainsABlankDisk(int type)
		 {
			 int  aa = mUnmanagedPart->ContainsABlankDisk((EDiskType)type);
			 return aa;
		 }

		 System::Collections::ArrayList^ GetWriteSpeeds()
		 {
			 TSpeeds speeds;
		//	 mUnmanagedPart->GetWriteSpeeds(&speeds);

			 System::Collections::ArrayList^ al = gcnew System::Collections::ArrayList();
			 for (int i=0;i<speeds.size();i++)
			 {
				 al->Add(gcnew String(speeds.at(i).SpeedX));
			 }
			 return al;
		 }

		 void SetCDRecordAdsress(int a,int b, int c)
		 {
			 mUnmanagedPart->mCdRecordAdress.a=a;
			 mUnmanagedPart->mCdRecordAdress.b=b;
			  mUnmanagedPart->mCdRecordAdress.c=c;
		 }

		 int GetCdRecordAddressA()
		 {
			 return mUnmanagedPart->mCdRecordAdress.a;
		 }

		 
		 int GetCdRecordAddressB()
		 {
			 return mUnmanagedPart->mCdRecordAdress.b;
		 }

		 
		 int GetCdRecordAddressC()
		 {
			 return mUnmanagedPart->mCdRecordAdress.c;
		 }

	};
	
	CManagedVideoDiskManager::CManagedVideoDiskManager(IVideoDiskCallback^ callback)
	{
		mCurrentVideoDiskCallback=callback;
		mInstance=&CVideoDiskManager::GetInstance();
	}

	bool CManagedVideoDiskManager::RebuildAvailbleVideoDevicesList()
	{
		return mInstance->RebuildAvailbleVideoDevicesList();
	}

	void CManagedVideoDiskManager::LogAvailbleVideoDevicesList()
	{
		mInstance->LogAvailbleVideoDevicesList();
	}

	System::Collections::ArrayList^ CManagedVideoDiskManager::GetVideoDiskDevices()
	{
		System::Collections::ArrayList^ return_list = gcnew System::Collections::ArrayList();

		std::list<CVideoDisk*>& devices = mInstance->GetVideoDiskDevices();

		for (std::list<CVideoDisk*>::iterator iter = devices.begin(); iter!=devices.end();iter++)
		{
			CManagedVideoDisk^ device = gcnew CManagedVideoDisk((*iter));
			return_list->Add(device);
		}

		return return_list;
	}

	
	

	void BurnPerctageDone(int percent)
	{
		CManagedVideoDiskManager::mCurrentVideoDiskCallback->BurnPercentageComplete(percent);
	}

	void BufferStatusCallback(int percent_done,
							  int bufferFreeSizeInUCHARs,
							  int BufferSizeInUCHARs)
	{
		CManagedVideoDiskManager::mCurrentVideoDiskCallback->BufferStatusCallback(percent_done,bufferFreeSizeInUCHARs,BufferSizeInUCHARs);
	}

    void BurnPadStarted()
	{
		CManagedVideoDiskManager::mCurrentVideoDiskCallback->PadStartedCallback();
	}


	void VideoCallback::PercentageDone(int p)
	{

		this->PerctangeDoneCB(p);
	}

	void VideoCallback::BufferStatusCallback(int pd, int bf, int bs)
	{
		this->BufferStatusCallbackfp(pd,bf,bs);
	}


	void VideoCallback::BurnPadStarted()
	{
		this->BurnPadStartedCB();
	}

}

	

