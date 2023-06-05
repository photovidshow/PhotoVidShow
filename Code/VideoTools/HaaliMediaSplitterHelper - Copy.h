#pragma once

#include <streams.h>     
#include <atlbase.h>


//*****************************************************************************************
// Helper class to load and use the haali media splitter
//*****************************************************************************************
class HaaliMediaSplitterHelper
{
public:
	HaaliMediaSplitterHelper(void);
	~HaaliMediaSplitterHelper(void);

	// ***************************************************************************************************
	// Input: video filename
	// Returns:
	// 0 don't use this splitter on this file (all other numbers means use this splitter)
	// 1 file is mp4
	// 2 file is mov
	// 3 file is mts
	// 4 file is m4a 
	int UseForFile(char* filename);

	// Load haali media libraries and creates an instance of the filter
	// Returns: Result. If result S_OK ppUnk is set to a pointer to the filter
	HRESULT CreateHaaliMediaSplitter(void** ppUnk);

	HRESULT RenderFromHaaliMediaSplitter(IGraphBuilder* pGraph, IBaseFilter* HaaliMediaSplitter, WCHAR* pFilename, bool renderVideoPin, bool renderAudioPin);
};
