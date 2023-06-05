#pragma once

#include "Burner.h"

#include <vector>
#include <string>

namespace Burner
{

class IBurningProgressCallback
{
public:
	virtual void PercentageDone(int p)=0;
	virtual void BufferStatusCallback(int percent_done,
									  int bufferFreeSizeInUCHARs,
									  int BufferSizeInUCHARs)=0;
	virtual void BurnPadStarted()=0;
};


enum EDiskType
{
	CD,
	DVD,
    BLURAY
};


struct TSpeed
{
//public:

        ULONG SpeedKBps;  //CD/DVD device speed in KB per second
        char *SpeedX;     //CD/DVD device speed in char* for printout
};

bool operator == (TSpeed first,TSpeed second);

typedef std::vector<TSpeed> TSpeeds;


struct TBurnOptions
{
        bool OPC;
        TSpeed Speed;
        int WriteMethod;
};

enum TDeviceStatus
{
        NoAction,
        Burning,
        Erasing,
        Ejecting,
        Canceling
};


class CScsiAddress
{
public:
	int a;
	int b;
	int c;
	int d;
};


class CCDRecordAddress
{
public:
	int a;
	int b;
	int c;
};


class CVideoDisk
{
public:
	CScsiAddress mScsiAddress;
	CCDRecordAddress mCdRecordAdress;
	bool mWritesCD;
	bool mWritesDVD;
	std::string mName;
	std::string mDeviceName;
	std::string mDriveLetter;
	int mWriteSpeed;	// only set after contain blank disk is called
	double mMediaSize;

	DISC_TYPE mDiscType;

	CVideoDisk(CScsiAddress& address, char* name, char* device_name,bool writedCD, bool writedvd, char* drive_letter);
	~CVideoDisk(void);

	virtual int Burn(char* root_folder, 
					  char* volume_name,
					  int minute,
					  int hour,
					  int day,
					  int month,
					  int year,
					  std::vector<std::string>& original_files_list, 
					  IBurningProgressCallback* prog_callback,
					  bool do_padding,
					  int test,
					  bool from_bin_cue_file)=0;

	virtual int ContainsABlankDisk(EDiskType type)=0;
	virtual int EraseDisk()=0;
//	virtual bool GetWriteSpeeds(TSpeeds * Speeds)=0;

	virtual bool IsSuspectedBluRayDrive();


	virtual void AbortBurn()=0;

};


}
