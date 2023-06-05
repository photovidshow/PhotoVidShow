// DSCodec.cpp: implementation of the CDSCodec class.
//
//////////////////////////////////////////////////////////////////////

#include <windows.h>
#include "qedit.h"
#include <uuids.h>



//#include "DShowEncoder.h"
#include "DSCodec.h"
#include "DSCodecFormat.h"
#include "UnmanagedErrors.h"


//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

//**********************************************************************
CDSCodec::CDSCodec()
{
	m_pMoniker = NULL;
	mTheCodecFormat44100 = NULL ;
	mTheCodecFormat48000 = NULL ;
}


//**********************************************************************
CDSCodec::~CDSCodec()
{
	
	if (mTheCodecFormat44100!=NULL) delete mTheCodecFormat44100;
	if (mTheCodecFormat48000!=NULL) delete mTheCodecFormat48000;


	if (m_pMoniker != NULL) {
		m_pMoniker->Release();
		m_pMoniker = NULL;
	}

  

}

//**********************************************************************
void CDSCodec::BuildCodecFormatArray()
{
	
	if (m_pMoniker == NULL) return;

	HRESULT			hr;
	IBaseFilter		*pBaseFilter = NULL;

	// Retrieve the IBaseFilter
	hr = m_pMoniker->BindToObject(NULL, NULL, IID_IBaseFilter, (void**) &pBaseFilter);
	if (FAILED(hr)) 
	{
		Error("DSCodec could not bind filter");
		return;
	}

	// Enumerate Pin
	IEnumPins	*pEnumPins = NULL;
	hr = pBaseFilter->EnumPins(&pEnumPins);
	if (FAILED(hr))
	{
		Error("DSCodec could not enumerate pins on base filter");
		pBaseFilter->Release();
		return;
	}

	// Find the output Pin
	IPin	* pPin = NULL;
	while (pEnumPins->Next(1, &pPin, 0) == S_OK) 
	{
		PIN_DIRECTION direction;
		pPin->QueryDirection(&direction);
		if (direction == PINDIR_OUTPUT) 
		{
			// Retrieve the IAMStreamConfig
			IAMStreamConfig	*pStreamConfig = NULL;
			hr = pPin->QueryInterface(IID_IAMStreamConfig, (void**) &pStreamConfig);
			if (SUCCEEDED(hr)) 
			{
				int nCount = 0, nSize = 0;
				pStreamConfig->GetNumberOfCapabilities(&nCount, &nSize);
				for (int i=0; i<nCount; i++) 
				{
					AM_MEDIA_TYPE* pMediaType = NULL;
					AUDIO_STREAM_CONFIG_CAPS confCaps;
					hr = pStreamConfig->GetStreamCaps(i, &pMediaType, (BYTE*)&confCaps);
					if (SUCCEEDED(hr)) 
					{
						CDSCodecFormat *pCodecFormat = new CDSCodecFormat();
						pCodecFormat->m_pMediaType = pMediaType;

						Trace("Format: Channels = %d sps = %d, bits p s = %d", 
						   pCodecFormat->NumberOfChannels(),
						   pCodecFormat->SamplesPerSecond(),
						   pCodecFormat->BitsPerSample());

					

						if (pCodecFormat->NumberOfChannels()==2 &&
							pCodecFormat->SamplesPerSecond()==44100 &&
							pCodecFormat->BitsPerSample()==16)
						{
							if (mTheCodecFormat44100) delete mTheCodecFormat44100;

							mTheCodecFormat44100 = pCodecFormat;
						}
						else if (pCodecFormat->NumberOfChannels()==2 &&
							pCodecFormat->SamplesPerSecond()==48000 &&
							pCodecFormat->BitsPerSample()==16)
						{
							if (mTheCodecFormat48000) delete mTheCodecFormat48000;

							mTheCodecFormat48000 = pCodecFormat;
						}
						else
						{
							delete pCodecFormat;
						}
					}
				}
				pStreamConfig->Release();
			}
		}
		pPin->Release();
	}

	pEnumPins->Release();
	pBaseFilter->Release();
	

}
