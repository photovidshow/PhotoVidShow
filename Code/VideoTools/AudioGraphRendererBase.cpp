
#include "AudioGraphRendererBase.h"
#include "UnmanagedErrors.h"
#include "LAVFilterHelper.h"
#include "CustomFilters/MP3TagStripperSourceFilter.h"

#include <initguid.h>
#include <stdio.h>
#include <windows.h>
#include <string>
#include "qedit.h"
#include <Dshow.h>
#include "DSCodec.h"
#include <dmodshow.h>

extern WCHAR * ConvertStr(char * szIn, WCHAR* to_str);

//
// mp3 decoder used on xp ( MP3 Fraunhofer Filter)  
//
DEFINE_GUID(CLSID_Mpeg3DecoderXP,
	0x38BE3000, 0xDBF4, 0x11D0, 0x86, 0x0e, 0x0, 0xa0, 0x24, 0xcf, 0xef, 0x6d);

//
// mp3 decoder dmo used on windows vista 7 / 8 / 10
//
DEFINE_GUID(CLSID_CMP3DecMediaObject,
	0xbbeea841, 0x0a63, 0x4f52, 0xa7, 0xab, 0xa9, 0xb3, 0xa8, 0x4e, 0xd3, 0x8a);





//*********************************************************************************************************************
static HRESULT CreateMP3DecoderDMOFilter(CComQIPtr<IBaseFilter>& mp3decoder)
{
	HRESULT hr = CoCreateInstance(CLSID_DMOWrapperFilter, NULL, CLSCTX_INPROC_SERVER, IID_IBaseFilter,
		reinterpret_cast<void**>(&mp3decoder));

	if (SUCCEEDED(hr))
	{
		// Query for IDMOWrapperFilter.
		IDMOWrapperFilter *pDmoWrapper;
		hr = mp3decoder->QueryInterface(IID_IDMOWrapperFilter, reinterpret_cast<void**>(&pDmoWrapper));

		if (SUCCEEDED(hr))
		{
			// Initialize the filter.
			hr = pDmoWrapper->Init(CLSID_CMP3DecMediaObject, CLSID_LegacyAmFilterCategory);
			pDmoWrapper->Release();
		}
	}

	return hr;
}

//*********************************************************************************************************************
BOOL CAudioGraphRendererBase::CreateAndRenderGraph(char* filename, 
												   IGraphBuilder* graphBuilder,
												   BOOL useLav)
{
	if (useLav == FALSE)
	{
		CreateDefaultFilters(graphBuilder);
	}

	return RenderGraph(filename, graphBuilder, useLav);
}



//*********************************************************************************************************************
// lets add our default set filters first (this causes render file to use these filters, rather than
// some crappy 3rd paty thing which has overwritten the defallt filters and causing us to crash).
// If the graph actually uses these filters depends on the media file provided, but they do
// no harm if they are not needed

void CAudioGraphRendererBase::CreateDefaultFilters(IGraphBuilder* graphBuilder)
{
	CComQIPtr<IBaseFilter>  Mpeg1Splitter;
	CComQIPtr<IBaseFilter>  mp3decoder;

	//
	// Add MPEG1 splitter (used for playing/ripping MP3's)
	//
	HRESULT hr = CoCreateInstance(CLSID_MPEG1Splitter, NULL, CLSCTX_INPROC_SERVER,
		IID_IBaseFilter, (void **)&Mpeg1Splitter);
	if (SUCCEEDED(hr))
	{
		hr = graphBuilder->AddFilter(Mpeg1Splitter, L"Mpeg1 Splitter");
		if (FAILED(hr))
		{
			Error("Failed to add mpeg1 splitter filter");
		}
		else
		{
			Trace("Added mpeg1 splitter to graph");
		}
	}


	//
	// Try default mp3 decoder used on vista, windows 7 ,8, 10
	//
	hr = CreateMP3DecoderDMOFilter(mp3decoder);

	if (SUCCEEDED(hr))
	{
		hr = graphBuilder->AddFilter(mp3decoder, L"Mpeg Layer3-decoder dmo");
		if (FAILED(hr))
		{
			Error("Failed to add mpeg3 decoder dmo");
		}
		else
		{
			Trace("Added mp3 decoder dmo to graph");
		}
	}
	else
	{
		//
		// Add default mp3 decoder as used on xp (this will probably fail for vista / 7/ 8 / 10 because it's not istalled on them)
		//
		hr = CoCreateInstance(CLSID_Mpeg3DecoderXP, NULL,
			CLSCTX_INPROC, IID_IBaseFilter, (void **)&mp3decoder);

		if (SUCCEEDED(hr))
		{
			hr = graphBuilder->AddFilter(mp3decoder, L"SRG Mpeg Layer3-decoder");
			if (FAILED(hr))
			{
				Error("Failed to add mpeg3 decoder");
			}
			else
			{
				Trace("Added xp mp3 decoder to graph");
			}
		}
	}

}


//******************************************************************************************************************************
BOOL CAudioGraphRendererBase::RenderGraph(char* filename,
										  IGraphBuilder* graphBuilder,
										  BOOL useLav)
{
	CComQIPtr<IBaseFilter>  ThirdPartySourceSplitterFilter;
	CComQIPtr<IBaseFilter>  LAVAudioDecoder;

	static WCHAR w_in_filename[4096];

	for (int i = 0; i < 4096; i++)
	{
		w_in_filename[i] = 0;
	}

	ConvertStr(filename, w_in_filename);

	LAVFilterHelper MyLavSplitterHelper;

	if (useLav == TRUE)
	{
		//
		// load and add lav splitter
		//
		HRESULT hr = MyLavSplitterHelper.CreateLAVSplitter((void**)&ThirdPartySourceSplitterFilter);

		if (SUCCEEDED(hr))
		{
			hr = graphBuilder->AddFilter(ThirdPartySourceSplitterFilter, L"Lav splitter");
			if (FAILED(hr))
			{
				Error("Failed to add LAV splitter to filter when audio ripping");
			}

			//
			// load and add lav audio decoder
			//
			hr = MyLavSplitterHelper.CreateLAVAudioDecoder((void**)&LAVAudioDecoder);

			if (SUCCEEDED(hr))
			{
				hr = graphBuilder->AddFilter(LAVAudioDecoder, L"Lav audio decoder");
				if (FAILED(hr))
				{
					Error("Failed to add lav audio decoder to filter when audio ripping");
				}
			}
		}
	}

	if (ThirdPartySourceSplitterFilter != NULL)
	{
		HRESULT hr = MyLavSplitterHelper.RenderFromSourceFilter(graphBuilder, ThirdPartySourceSplitterFilter, w_in_filename, false, true);

		//
		// If failed remove haali splitter and try old method
		//
		if (FAILED(hr))
		{
			graphBuilder->RemoveFilter(ThirdPartySourceSplitterFilter);
			ThirdPartySourceSplitterFilter.Release();
			hr = graphBuilder->RenderFile(w_in_filename, NULL);// 'L' macro makes it WCHAR like we need it to be

			if (FAILED(hr))
			{
				return FALSE;
			}
		}
	}
	else
	{
		//
		// If not using lav and a mp3 file attempt to use our custom filter as this removes any problem frames in the ID tag
		//
		if (useLav == FALSE && IsMp3File(filename) == TRUE)
		{
			BOOL result = RenderFromMp3TagStripperSourceFilter(graphBuilder, w_in_filename);

			if (result == TRUE)
			{
				return TRUE;
			}
		}
		

		//
		// Render using default Direct Show source filter
		//
		HRESULT hr = graphBuilder->RenderFile(w_in_filename, NULL);

		if (FAILED(hr))
		{
			Error("Could not render file media file %s", filename);
			return FALSE;
		}
	}

	return TRUE;
}


//******************************************************************************************************************************
BOOL CAudioGraphRendererBase::IsMp3File(char* filename)
{
	int length = strlen(filename);
	if (length <= 4)
	{
		return FALSE;
	}

	if ((filename[length - 1] == '3') &&
		(filename[length - 2] == 'p' || filename[length - 2] == 'P') &&
		(filename[length - 3] == 'm' || filename[length - 3] == 'M') &&
		(filename[length - 4] == '.'))
	{
		return TRUE;
	}

	return FALSE;
}

//******************************************************************************************************************************
BOOL CAudioGraphRendererBase::RenderFromMp3TagStripperSourceFilter(IGraphBuilder* graphBuilder, WCHAR* filename)
{
	CComQIPtr<IBaseFilter> mp3TagStripperSourceFilterBaseFilter;

	CMP3TagStripperSourceFilter* mp3TagStripperSourceFilter = new CMP3TagStripperSourceFilter(NULL, NULL);

	//
	// COM now takes ownership of filter and will release when local reference when function returns
	//
	CComQIPtr<IFileSourceFilter> sourceFilterHandle(mp3TagStripperSourceFilter);

	HRESULT hr = mp3TagStripperSourceFilter->QueryInterface(IID_IBaseFilter, (void**)&mp3TagStripperSourceFilterBaseFilter);
	if (FAILED(hr))
	{
		Error("DirectShowPlayer could not create the Filter Graph Manager.");

		return FALSE;
	}

	hr = graphBuilder->AddFilter(mp3TagStripperSourceFilterBaseFilter, L"mp3 tag stripper source filter");
	if (FAILED(hr))
	{
		Error("DirectShowPlayer failed to add mp3 tag stripper source filter");
		return FALSE;
	}

	//
	// Set media type to stream and mpeg1 audio
	//
	AM_MEDIA_TYPE mp3mediaType;
	memset(&mp3mediaType, 0, sizeof(mp3mediaType));
	mp3mediaType.majortype = MEDIATYPE_Stream;
	mp3mediaType.subtype = MEDIASUBTYPE_MPEG1Audio;

	//
	// 
	//
	hr = mp3TagStripperSourceFilter->Load(filename, &mp3mediaType);
	if (FAILED(hr))
	{
		//
		// This is not necessarliy a fail, it just means we can't use our custom filter, so
		// should fail, and then try again without this filter.
		//
		graphBuilder->RemoveFilter(mp3TagStripperSourceFilter);
		return FALSE;
	}

	hr = graphBuilder->Render(mp3TagStripperSourceFilter->GetPin(0));

	if (FAILED(hr))
	{
		Error("Could not render from the mp3 tag stripper source filter");
		graphBuilder->RemoveFilter(mp3TagStripperSourceFilter);
		return FALSE;
	}

	return TRUE;
}
