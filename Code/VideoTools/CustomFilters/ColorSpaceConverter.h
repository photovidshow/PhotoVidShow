#pragma once

#include "windows.h"
#include "streams.h"
#include "Dvdmedia.h"
#include "wmsdkidl.h"

class CColorSpaceConverter
{
public:
	CColorSpaceConverter(const GUID mediaType, int width, int height);
	virtual ~CColorSpaceConverter(void);
	void convert_to_rgb32(BYTE* outFrameBuffer, BYTE* inSampleBuffer, int outFrameBufferSize, int inSampleBufferSize, BOOL quarter);

private:

	GUID m_mediaType;
	int m_width; 
	int m_height;

	int m_uPlanePos;
	int m_vPlanePos;
};

