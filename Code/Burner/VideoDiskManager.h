#pragma once


#include "VideoDisk.h"
#include <list>

namespace Burner
{



class CVideoDiskManager
{

	std::list<CVideoDisk*>	mVideoDiskDevices;

	static CVideoDiskManager* mInstance;
public:

	static CVideoDiskManager&	GetInstance();

private:
	CVideoDiskManager(void);
	~CVideoDiskManager(void);

	void RemoveAllVideoDiskDevices();


public:

	bool RebuildAvailbleVideoDevicesList();
	void LogAvailbleVideoDevicesList();
	void AddNewVideoDiskDevice(CVideoDisk* disk);
	std::list<CVideoDisk*>& GetVideoDiskDevices();

};



}

