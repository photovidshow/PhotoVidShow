// Burner.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "DVDDisk.h"
#include "VideoDiskManager.h"

#include <iostream>

using namespace std;

namespace Burner
{

class TestBurner : public IBurningProgressCallback
{
public:
	virtual void PercentageDone(int p)
	{
		cout << "!!!!!!!Progress update at" << p << "%"<< endl;
	}
	virtual void BufferStatusCallback(int percent_done,
									  int bufferFreeSizeInUCHARs,
									  int BufferSizeInUCHARs)
	{
	}


};


}

using namespace Burner;

/*


void main()
{

	CVideoDiskManager& manager = CVideoDiskManager::GetInstance();

	bool val = manager.RebuildAvailbleVideoDevicesList();

	std::list<CVideoDisk*>& items = manager.GetVideoDiskDevices();

	CDVDDisk* dvddisk = (CDVDDisk*)items.front();

	bool blank_dvd_disk = dvddisk->ContainsABlankDisk(DVD);
	bool blank_cd_disk = dvddisk->ContainsABlankDisk(CD);



	TestBurner* tb = new TestBurner();
	CScsiAddress ad;
	ad.a=2; ad.b=0;ad.c=0;ad.d=0;
	CDVDDisk* disk = new CDVDDisk(ad,"Stu's DVD burner");
	std::vector<std::string> org_list;

	org_list.push_back(std::string("c:\\mediadata\\0004.jpg"));
	org_list.push_back(std::string("c:\\mediadata\\0005.jpg"));
	org_list.push_back(std::string("c:\\mediadata\\005.jpg"));
	org_list.push_back(std::string("c:\\mediadata\\05.jpg"));
	org_list.push_back(std::string("c:\\mediadata\\5as.jpg"));
	//org_list.push_back(std::string("c:\\mediadata\\5as.jpg"));
	//org_list.push_back(std::string("c:\\mediadata\\5aj.jpg"));
	org_list.push_back(std::string("c:\\mediadata\\0005fg.jpg"));

	disk->Burn("c:\\dvdfolder",org_list, tb, 0);

}

*/



