#pragma once

#include "videodisk.h"


#define CD_STANDARD_SPEED_COUNT 15
#define DVD_STANDARD_SPEED_COUNT 10

namespace Burner
{


class CCDDisk :
	public CVideoDisk
{
protected:
	bool mAbortingBurn;
	bool mBurning;
	PVOID l__PVOID__CdvdBurnerGrabber ;


public:
	CCDDisk(CScsiAddress& address, char* name, char* device_name, char* drive_letter);
	CCDDisk(CScsiAddress& address, char* name, char* device_name,bool writedCD, bool writedvd,char* drive_letter);

	~CCDDisk(void) {}

	virtual int Burn( char* root_folder, 
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
					  bool from_bin_cue_file) ;

	virtual int ContainsABlankDisk(EDiskType type);
	virtual int EraseDisk();

//	virtual bool GetWriteSpeeds(TSpeeds * Speeds);
	bool TrySetWriteSpeed(TSpeed* Speed, void* m_CdvdBurnerGrabber);

	const DISC_TYPE GetMediaType();

	//bool RefreshMediaSize(DISC_TYPE l_MediaType, PVOID grabber);
	//const double GetMediaSize();

	bool TestUnitReady(PVOID grabber);


	
	//bool GetTrackInfo(int Track,
//						   STARBURN_TRACK_INFORMATION *l_TrackInformation,
//						   PVOID grabber);

	bool IsDiskInDriveBlank(PVOID grabber);



	virtual void AbortBurn();
};

}

