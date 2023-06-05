
#include "ThirdPartyDirectShowSourceFilterHelper.h"

//***************************************************************************
static bool IsUnConnectedPinTypeVideo(IPin* pPin)
{
	CComQIPtr<IEnumMediaTypes> mediaTypes;
	HRESULT hr = pPin->EnumMediaTypes(&mediaTypes);
	if (SUCCEEDED(hr))
	{
		AM_MEDIA_TYPE* mediaType;
		if (mediaTypes->Next(1, &mediaType, NULL) == S_OK)
		{
			if (mediaType->majortype == MEDIATYPE_Video)
			{
				return true;
			}
		}
	}
	return false;
}


//***************************************************************************
static bool IsUnConnectedPinTypeAudio(IPin* pPin)
{
	CComQIPtr<IEnumMediaTypes> mediaTypes;
	HRESULT hr = pPin->EnumMediaTypes(&mediaTypes);
	if (SUCCEEDED(hr))
	{
		AM_MEDIA_TYPE* mediaType;
		if (mediaTypes->Next(1, &mediaType, NULL) == S_OK)
		{
			if (mediaType->majortype == MEDIATYPE_Audio)
			{
				return true;
			}
		}
	}
	return false;
}


//***************************************************************************
ThirdPartyDirectShowSourceFilterHelper::ThirdPartyDirectShowSourceFilterHelper(void)
{
}

//***************************************************************************
ThirdPartyDirectShowSourceFilterHelper::~ThirdPartyDirectShowSourceFilterHelper(void)
{
}


//***************************************************************************
HRESULT ThirdPartyDirectShowSourceFilterHelper::RenderFromSourceFilter(IGraphBuilder* pGraph, IBaseFilter* sourceSplitter, WCHAR* pFilename, bool renderVideoPin, bool renderAudioPin)
{
	HRESULT hr;
	CComQIPtr<IFileSourceFilter> sourceFilter;
	hr = sourceSplitter->QueryInterface(IID_IFileSourceFilter, (void **)&sourceFilter);
	bool renderedFromSourceSplitter = false;
	if (SUCCEEDED(hr))
	{
		hr = sourceFilter->Load(pFilename, NULL);

		if (SUCCEEDED(hr))
		{
			// render from out pin
			CComQIPtr<IEnumPins> SourceMediaSplitterPins;
			hr = sourceSplitter->EnumPins(&SourceMediaSplitterPins);
			if (SUCCEEDED(hr))
			{
				SourceMediaSplitterPins->Reset();
				CComQIPtr<IPin> SourcePin;
				bool foundVideoOutputPin = false;
				bool foundAudioOutputPin = false;

				while (SourceMediaSplitterPins->Next(1, &SourcePin, NULL) == S_OK)
				{
					PIN_INFO SourcePinInfo;
					SourcePin->QueryPinInfo(&SourcePinInfo);
					SourcePinInfo.pFilter->Release();

					if (SourcePinInfo.dir == PINDIR_OUTPUT)
					{
						if (renderVideoPin && IsUnConnectedPinTypeVideo(SourcePin) == true)
						{
							if (foundVideoOutputPin == false)
							{
								hr = pGraph->Render(SourcePin);
								foundVideoOutputPin = true;
								renderedFromSourceSplitter = true;
							}
						}

						if (renderAudioPin && IsUnConnectedPinTypeAudio(SourcePin) == true)
						{
							if (foundAudioOutputPin == false)
							{
								hr = pGraph->Render(SourcePin);
								foundAudioOutputPin = true;
								renderedFromSourceSplitter = true;
							}
						}
					}
					SourcePin.Release();
				}
			}
		}
	}

	if (FAILED(hr) || renderedFromSourceSplitter == false)
	{
		return S_FALSE;
	}

	return S_OK;
}
