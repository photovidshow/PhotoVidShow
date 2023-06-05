// STEAM 

#include "ManagedErrors.h"
#include <iostream>
//#include "c:\dev\PhotoVidShow\SteamSDK\sdk\public\steam\steam_api.h"
#using <mscorlib.dll>

using namespace System;
using namespace System::Text;
using namespace System::Runtime::InteropServices;
using namespace ManagedCore;
using namespace std;


namespace MangedToUnManagedWrapper
{
	public ref class CSteamAPI
	{
	public:

		CSteamAPI();

		~CSteamAPI();

		bool RestartAppIfNecessary(unsigned int id);
	};

	CSteamAPI::CSteamAPI()
	{
	}

	CSteamAPI::~CSteamAPI()
	{
	}

	bool CSteamAPI::RestartAppIfNecessary(unsigned int id)
	{
		// SRG FOR STEAM VERSION
	//	bool result = SteamAPI_RestartAppIfNecessary((uint32)id);
	//	return result;

		return true;
	}
}


