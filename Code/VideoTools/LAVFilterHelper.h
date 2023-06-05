#pragma once

#include "ThirdPartyDirectShowSourceFilterHelper.h"
#include <streams.h>     
#include <atlbase.h>


//*****************************************************************************************
// Helper class to load and use the LAV filters
//*****************************************************************************************
class LAVFilterHelper : public ThirdPartyDirectShowSourceFilterHelper
{
public:
	LAVFilterHelper(void);
	~LAVFilterHelper(void);

	// Load Lav libraries and creates an instance of the filter
	// Returns: Result. If result S_OK ppUnk is set to a pointer to the filter
	HRESULT CreateLAVSplitter(void** ppUnk);
	HRESULT CreateLAVAudioDecoder(void** ppUnk);
	HRESULT CreateLAVVideoDecoder(void** ppUnk);

	int GetFileType(char* filename);
};
