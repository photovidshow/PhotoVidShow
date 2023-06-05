
#include "LAVFilterHelper.h"
#include <initguid.h>


DEFINE_GUID(CLSID_LavSplitter,
	0xB98D13E7, 0x55DB, 0x4385, 0xa3, 0x3d, 0x09, 0xfd, 0x1b, 0xa2, 0x63, 0x38);

DEFINE_GUID(CLSID_LavAudioDecoder,
	0xE8E73B6B, 0x4CB3, 0x44A4, 0xbe, 0x99, 0x4f, 0x7b, 0xcb, 0x96, 0xe4, 0x91);

DEFINE_GUID(CLSID_LavVideoDecoder,
	0xEE30215D, 0x164F, 0x4A92, 0xa4, 0xeb, 0x9d, 0x4c, 0x13, 0x39, 0x0f, 0x9f);


typedef HRESULT(STDAPICALLTYPE* FN_DLLGETCLASSOBJECT)(REFCLSID clsid, REFIID iid, void** ppv);



//***************************************************************************
LAVFilterHelper::LAVFilterHelper(void)
{
}

//***************************************************************************
LAVFilterHelper::~LAVFilterHelper(void)
{
}


//***************************************************************************
HRESULT LAVFilterHelper::CreateLAVSplitter(void** ppUnk)
{
	static bool loadedLibrary = false;
	static FN_DLLGETCLASSOBJECT fn = NULL;

	REFCLSID clsid = CLSID_LavSplitter;

	if (loadedLibrary == false)
	{
		HMODULE lib = LoadLibrary(TEXT("dsfilters\\lav\\LAVSplitter.ax"));
		if (!lib)
		{
			return HRESULT_FROM_WIN32(GetLastError());
		}
		fn = (FN_DLLGETCLASSOBJECT)GetProcAddress(lib, "DllGetClassObject");
		if (fn == NULL)
		{
			return HRESULT_FROM_WIN32(GetLastError());
		}

		loadedLibrary = true;
	}

	// the entry point is an exported function

	// create a class factory
	CComQIPtr<IClassFactory> pCF;
	HRESULT hr = fn(clsid, IID_IUnknown, (void**)&pCF);
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

//
// LAV audio decoder can work on this type
//
// pcm twos compliment
//
// 736F7774 - 0000 - 0010 - 8000 - 00AA00389B71

//***************************************************************************
HRESULT LAVFilterHelper::CreateLAVAudioDecoder(void** ppUnk)
{
	static bool loadedLibrary = false;
	static FN_DLLGETCLASSOBJECT fn = NULL;

	REFCLSID clsid = CLSID_LavAudioDecoder;

	if (loadedLibrary == false)
	{
		HMODULE lib2 = LoadLibrary(TEXT("dsfilters\\lav\\LAVAudio.ax"));
		if (!lib2)
		{
			return HRESULT_FROM_WIN32(GetLastError());
		}
		fn = (FN_DLLGETCLASSOBJECT)GetProcAddress(lib2, "DllGetClassObject");
		if (fn == NULL)
		{
			return HRESULT_FROM_WIN32(GetLastError());
		}

		loadedLibrary = true;
	}

	// the entry point is an exported function

	// create a class factory
	CComQIPtr<IClassFactory> pCF;
	HRESULT hr = fn(clsid, IID_IUnknown, (void**)&pCF);
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


//***************************************************************************
HRESULT LAVFilterHelper::CreateLAVVideoDecoder(void** ppUnk)
{
	static bool loadedLibrary = false;
	static FN_DLLGETCLASSOBJECT fn = NULL;

	REFCLSID clsid = CLSID_LavVideoDecoder;

	if (loadedLibrary == false)
	{
		HMODULE lib2 = LoadLibrary(TEXT("dsfilters\\lav\\LAVVideo.ax"));
		if (!lib2)
		{
			return HRESULT_FROM_WIN32(GetLastError());
		}
		fn = (FN_DLLGETCLASSOBJECT)GetProcAddress(lib2, "DllGetClassObject");
		if (fn == NULL)
		{
			return HRESULT_FROM_WIN32(GetLastError());
		}

		loadedLibrary = true;
	}

	// the entry point is an exported function

	// create a class factory
	CComQIPtr<IClassFactory> pCF;
	HRESULT hr = fn(clsid, IID_IUnknown, (void**)&pCF);
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


//***************************************************************************
int LAVFilterHelper::GetFileType(char* filename)
{
	size_t strlength = strlen(filename);
	if (strlength >4)
	{
		if (filename[strlength - 1] == '4' &&
			(filename[strlength - 2] == 'p' || filename[strlength - 2] == 'P') &&
			(filename[strlength - 3] == 'm' || filename[strlength - 3] == 'M') &&
			filename[strlength - 4] == '.')
		{
			return 1;
		}


		if ((filename[strlength - 1] == 'v' || filename[strlength - 1] == 'V') &&
			(filename[strlength - 2] == 'o' || filename[strlength - 2] == 'O') &&
			(filename[strlength - 3] == 'm' || filename[strlength - 3] == 'M') &&
			filename[strlength - 4] == '.')
		{
			return 2;
		}

		if ((filename[strlength - 1] == 's' || filename[strlength - 1] == 'S') &&
			(filename[strlength - 2] == 't' || filename[strlength - 2] == 'T') &&
			(filename[strlength - 3] == 'm' || filename[strlength - 3] == 'M') &&
			filename[strlength - 4] == '.')
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


		if ((filename[strlength - 1] == 'v' || filename[strlength - 1] == 'V') &&
			(filename[strlength - 2] == 'k' || filename[strlength - 2] == 'K') &&
			(filename[strlength - 3] == 'm' || filename[strlength - 3] == 'M') &&
			filename[strlength - 4] == '.')
		{
			return 3;
		}


		if ((filename[strlength - 1] == 'a' || filename[strlength - 1] == 'A') &&
			(filename[strlength - 2] == '4') &&
			(filename[strlength - 3] == 'm' || filename[strlength - 3] == 'M') &&
			filename[strlength - 4] == '.')
		{
			return 4;
		}
	}
	return 0;
}


