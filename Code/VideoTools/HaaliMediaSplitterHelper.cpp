
#include "HaaliMediaSplitterHelper.h"
#include <initguid.h>

//Display Name      : Haali Media Splitter
//Filename          : C:\Program Files (x86)\Win7codecs\filters\Splitter.ax
//Filter Merit      : 0x00800001
//CLSID             : {55DA30FC-F16B-49FC-BAA5-AE59FC65F82D}
DEFINE_GUID(CLSID_HaaliMediaSplitterVideoCodec,
0x55DA30FC, 0xF16B, 0x49FC, 0xba, 0xa5, 0xae, 0x59, 0xfc, 0x65, 0xf8, 0x2d);

typedef HRESULT (STDAPICALLTYPE* FN_DLLGETCLASSOBJECT)(REFCLSID clsid, REFIID iid, void** ppv);


//***************************************************************************
HaaliMediaSplitterHelper::HaaliMediaSplitterHelper(void)
{
}

//***************************************************************************
HaaliMediaSplitterHelper::~HaaliMediaSplitterHelper(void)
{
}

//***************************************************************************
int HaaliMediaSplitterHelper::UseForFile(char* filename)
{										   
	size_t strlength = strlen(filename);
	if (strlength >4)
	{
		if (filename[strlength-1] == '4' &&
			(filename[strlength-2] == 'p' || filename[strlength-2] == 'P') &&
			(filename[strlength-3] == 'm' || filename[strlength-3] == 'M') &&
			filename[strlength-4] == '.')
		{
			return 1;
		}

		
		if ((filename[strlength-1] == 'v' || filename[strlength-1] == 'V') &&
			(filename[strlength-2] == 'o' || filename[strlength-2] == 'O') &&
			(filename[strlength-3] == 'm' || filename[strlength-3] == 'M') &&
			filename[strlength-4] == '.')
		{
			return 2;
		}

		if ((filename[strlength-1] == 's' || filename[strlength-1] == 'S') &&
			(filename[strlength-2] == 't' || filename[strlength-2] == 'T') &&
			(filename[strlength-3] == 'm' || filename[strlength-3] == 'M') &&
			filename[strlength-4] == '.')
		{
			return 3;
		}

		if (strlength >5 &&
			(filename[strlength - 1] == 's' || filename[strlength - 1] == 'S') &&
			(filename[strlength - 2] == 't' || filename[strlength - 2] == 'T') &&
			(filename[strlength - 3] == '2' || filename[strlength - 3] == '2') &&
			(filename[strlength - 4] == 'm' || filename[strlength - 4] == 'M') &&
			filename[strlength - 5] == '.')
		{
			return 3;
		}


		if ((filename[strlength-1] == 'v' || filename[strlength-1] == 'V') &&
			(filename[strlength-2] == 'k' || filename[strlength-2] == 'K') &&
			(filename[strlength-3] == 'm' || filename[strlength-3] == 'M') &&
			 filename[strlength-4] == '.')
		{
			return 3;
		}

		
		if ((filename[strlength-1] == 'a' || filename[strlength-1] == 'A') &&
			(filename[strlength-2] == '4'							     ) &&
			(filename[strlength-3] == 'm' || filename[strlength-3] == 'M') &&
			 filename[strlength-4] == '.')
		{
			return 4;
		}	
	}
	return 0;
}

//***************************************************************************
HRESULT HaaliMediaSplitterHelper::CreateHaaliMediaSplitter(void** ppUnk)
{
	static bool loadedLibrary = false;
	static FN_DLLGETCLASSOBJECT fn = NULL;
	static FN_DLLGETCLASSOBJECT fn2 = NULL;

	REFCLSID clsid = CLSID_HaaliMediaSplitterVideoCodec;

	if (loadedLibrary == false)
	{
		// ******** WE MUST ALSO have REGISTERED mkx.dll, mp4.dll and ts.dll via regsvr32  *********

		//mkx is haali file source filter
		//mp4 is container reader for mov and mp4 files
		//ts is container reader for mts files

		HMODULE lib4 = LoadLibrary("dsfilters\\mkzlib.dll");
		if (!lib4)
		{
			return HRESULT_FROM_WIN32(GetLastError());
		}

		HMODULE lib5 = LoadLibrary("dsfilters\\mkunicode.dll");
		if (!lib5)
		{
			return HRESULT_FROM_WIN32(GetLastError());
		}
		// load the target DLL directly
		HMODULE lib = LoadLibrary(TEXT("dsfilters\\splitter.ax"));
		if (!lib)
		{
			return HRESULT_FROM_WIN32(GetLastError());
		}
		fn = (FN_DLLGETCLASSOBJECT)GetProcAddress(lib, "DllGetClassObject");
		if (fn == NULL)
		{
			return HRESULT_FROM_WIN32(GetLastError());
		}

		loadedLibrary=true;
	}

	// the entry point is an exported function
	
	// create a class factory
	CComQIPtr<IClassFactory> pCF;
	HRESULT hr = fn(clsid,  IID_IUnknown,  (void**)&pCF);
	if (SUCCEEDED(hr))
	{
		if (pCF == NULL)
		{
			hr = E_NOINTERFACE;
		}
		else
		{
			// ask the class factory to create the object
			hr = pCF->CreateInstance(NULL, IID_IBaseFilter, ppUnk);
		}
	}

	return hr;
}

