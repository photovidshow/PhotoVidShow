#pragma once

#include <streams.h>     
#include <atlbase.h>


//*****************************************************************************************
// Helper class to load and use the third party direct show source filters
// e.g. haali or LAV
//*****************************************************************************************
class ThirdPartyDirectShowSourceFilterHelper
{
public:
	ThirdPartyDirectShowSourceFilterHelper(void);
	~ThirdPartyDirectShowSourceFilterHelper(void);

	HRESULT RenderFromSourceFilter(IGraphBuilder* pGraph, IBaseFilter* sourceSplitter, WCHAR* pFilename, bool renderVideoPin, bool renderAudioPin);
};
