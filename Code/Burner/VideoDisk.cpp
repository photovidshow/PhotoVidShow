#include "StdAfx.h"
#include ".\videodisk.h"

namespace Burner
{

bool operator == (TSpeed first,TSpeed second)
{
        if(first.SpeedKBps == second.SpeedKBps)
        {
                return true;
        }
        else
        {
                return false;
        }
};

CVideoDisk::CVideoDisk(CScsiAddress& address,
					   char* name, 
					   char* device_name,
					   bool writesCD, 
					   bool writesdvd,
					   char* drive_letter) :
	mScsiAddress(address),
	mName(std::string(name)),
	mDeviceName(std::string(device_name)),
	mDriveLetter(std::string(drive_letter)),
	mWritesCD(writesCD),
	mWritesDVD(writesdvd),
	mDiscType(DISC_TYPE_UNKNOWN)
{

	mWriteSpeed=5440;	// x4
	mMediaSize=0;
}


CVideoDisk::~CVideoDisk(void)
{
}

bool CVideoDisk::IsSuspectedBluRayDrive()
{
	//
	// Default is false, overriten by sub classes
	//
	return false;
}



}

