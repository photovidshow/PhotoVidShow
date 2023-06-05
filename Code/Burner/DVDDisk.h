#pragma once
#include "VideoDisk.h"
#include "DDisk.h"

namespace Burner
{

	
//
// Represents a DVD or BLU burner drive
//
class CDVDDisk :public CCDDisk
{
private:
	
	CDB_FAILURE_INFORMATION  m_CDBFailureInfo;
	TDeviceStatus m_DeviceStatus;
	EXCEPTION_NUMBER m_ExceptionNumber;
    char m_ExceptionText[ 1024 ];
    unsigned long  m_SystemError;
	void* m_CdvdBurnerGrabber;
	bool m_IsSuspectedBluRayDrive;		// Probably a blu-ray drive, simply used to best order drives when burning a blu-ray disc

public:

	CDVDDisk(CScsiAddress& address, char* name, char* device_name, char* drive_letter);
	~CDVDDisk(void);

	virtual int Burn( char* folder_or_file, 
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
					   bool from_bin_cue_file);

	virtual bool IsSuspectedBluRayDrive();		

private:

    bool CdvdBurnerGrabberCreate(PCALLBACK FCallback);
	bool TrySetWriteSpeed(TSpeed* Speed);


	int BurnUsingDVDVideo(char* root_foler, 
						   char* volume_name,
					       int minute,
					       int hour,
					       int day,
					       int month,
					       int year,
						   IBurningProgressCallback* prog_callback,
						   bool do_padding,
					       int test);

   int BurnImage(char* isoFile, bool doPadding, IBurningProgressCallback* pg);

   int StartUpStarburn();
   int SetDeviceReadyForBurn();
   int CheckForSuccess(bool* error);
   void CleanUp();

};


  
}
