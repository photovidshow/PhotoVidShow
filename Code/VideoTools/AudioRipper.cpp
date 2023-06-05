// VBTest.cpp : Defines the entry point for the console application.
//

#include "AudioRipper.h"
#include "UnmanagedErrors.h"
#include "LAVFilterHelper.h"
#include "CustomFilters/WavDest.h"
#include <atlbase.h>
#include <initguid.h>
#include <stdio.h>
#include <windows.h>
#include <string>
#include "qedit.h"
#include <Dshow.h>



#define QueryFilterInfoReleaseGraph(fi) if ((fi).pGraph) (fi).pGraph->Release();

#include "DSCodec.h"

bool CAudioRipper::mAbort=false;


#include <rpcdce.h>
// Audio ripper	 anything to stero PCM or AC3 if input has 5.1 support

// THIS CODE NEEDS THE WAVDEST FILTER INSTALLED AT THE MOMENT

#include <dmodshow.h>

#define WAVE_FORMAT_DOLBY_AC3 0x2000
// {00002000-0000-0010-8000-00aa00389b71}
DEFINE_GUID(MEDIASUBTYPE_WAVE_DOLBY_AC3, 
WAVE_FORMAT_DOLBY_AC3, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

//
// microsoft audio decoder DTV-DVD  E1F1A0B8-BEEE-490D-BA7C-066C40B5E2B9
//
DEFINE_GUID(CLSID_MSAudioDecoderDTV,
	0xE1F1A0B8, 0xBEEE, 0x490D, 0xba, 0x7c, 0x06, 0x6c, 0x40, 0xb5, 0xe2, 0xb9);


extern void LogGraph(IFilterGraph *pGraphm, char* filename);

//*******************************************************************************************

// usefull direct show functions

extern HRESULT GetUnconnectedPin(
    IBaseFilter *pFilter,   // Pointer to the filter.
    PIN_DIRECTION PinDir,   // Direction of the pin to find.
    IPin **ppPin)  ;         // Receives a pointer to the pin
  

extern HRESULT ConnectFilters(
    IGraphBuilder *pGraph, // Filter Graph Manager.
    IPin *pOut,            // Output pin on the upstream filter.
    IBaseFilter *pDest) ;   // Downstream filter.


extern HRESULT ConnectFilters(
    IGraphBuilder *pGraph, 
    IBaseFilter *pSrc, 
    IBaseFilter *pDest);
 

extern WCHAR * ConvertStr(char * szIn, WCHAR* to_char);

static int nChannel =0;
static int nBitsPerSample=0;
static int nSamplesPerSec =0;
static int nAvgBytesPerSec=0;


//***************************************************************************
BOOL SetFilterFormat(AM_MEDIA_TYPE* new_type, IPin* pPin)
{
	BOOL retVal = FALSE;

	HRESULT hr;

	PIN_DIRECTION sDirection;
	pPin->QueryDirection(&sDirection);


	// debug, found out what pin stream type is

	CComQIPtr<IAMStreamConfig> pStreamConfig;
	hr = pPin->QueryInterface(IID_IAMStreamConfig, (void**) &pStreamConfig);
	if (SUCCEEDED(hr)) 
	{
		AM_MEDIA_TYPE* audio_format;
		hr = pStreamConfig->GetFormat(&audio_format);
	
		if (SUCCEEDED(hr)) 
		{
			WAVEFORMATEX* format = (WAVEFORMATEX*) audio_format->pbFormat;

			if (sDirection == PINDIR_OUTPUT)
			{
				Trace("------ Output for acm wrapper was -----");
			}
			else
			{
				Trace("------ Input for acm wrapper was -----");
			}
			Trace("Channels = %d", format->nChannels);
			Trace("Bits per sample = %d", format->wBitsPerSample);
			Trace("Samples per second = %d", format->nSamplesPerSec);
			Trace("Average bytes per second = %d", format->nAvgBytesPerSec);

			nChannel =format->nChannels;
			nBitsPerSample=format->wBitsPerSample;
			nSamplesPerSec =format->nSamplesPerSec;
			nAvgBytesPerSec=format->nAvgBytesPerSec;
		}
	}
	else
	{
		Error("Audio ripper could not get a stream config interface 1");
		return FALSE;
	}


	// Output Pin ?
	if (sDirection == PINDIR_OUTPUT) 
	{
		CComQIPtr<IAMStreamConfig> pStreamConfig2;
		hr = pPin->QueryInterface(IID_IAMStreamConfig, (void**) &pStreamConfig2);
		if (SUCCEEDED(hr)) 
		{
			hr = pStreamConfig2->SetFormat(new_type);
		
			if (SUCCEEDED(hr)) 
			{
				retVal = TRUE;
			}
		}
		else
		{
			Error("Audio ripper could not get a stream config interface 2");
			return FALSE;
		}
	}

	return retVal;

}



//***************************************************************************
BOOL SetFilterFormat(AM_MEDIA_TYPE* new_type, CComQIPtr<IBaseFilter> pBaseFilter)
{
	HRESULT hr;
	BOOL retVal = FALSE;

	// Pin enumeration
	CComQIPtr<IEnumPins> pEnumPins ;
	hr = pBaseFilter->EnumPins(&pEnumPins);
	if (FAILED(hr)) 
	{
		Error("Audio ripper could not enumerate pins on a base filter");
		return FALSE;
	}

	IPin* pPin;
	while (pEnumPins->Next(1, &pPin, 0) == S_OK) 
	{	
		retVal = SetFilterFormat(new_type,pPin);
		pPin->Release();
	}

	return retVal;
}

//***************************************************************************
BOOL GetOutputAudioInfo(IPin* pPin, int* out_frequency, int* out_bytes_per_sample, int* out_stereo)
{
	BOOL retVal = FALSE;

	HRESULT hr;

	PIN_DIRECTION sDirection;
	pPin->QueryDirection(&sDirection);

	// debug, found out what pin stream type is

	CComQIPtr<IAMStreamConfig> pStreamConfig;
	hr = pPin->QueryInterface(IID_IAMStreamConfig, (void**) &pStreamConfig);
	if (SUCCEEDED(hr)) 
	{
	//	return TRUE;

		AM_MEDIA_TYPE* audio_format;
		hr = pStreamConfig->GetFormat(&audio_format);
	
		if (SUCCEEDED(hr)) 
		{
			WAVEFORMATEX* format = (WAVEFORMATEX*) audio_format->pbFormat;

		//	if (sDirection == PINDIR_OUTPUT)
			{
				if (format->nChannels==2) *out_stereo=1; else *out_stereo=0;
				*out_bytes_per_sample=0;
				if (format->wBitsPerSample==8) *out_bytes_per_sample=1;
				if (format->wBitsPerSample==16) *out_bytes_per_sample=2;
				*out_frequency = format->nSamplesPerSec;


			//	char temp[512];
			//	sprintf(temp,"Found auido input format, sps=%d, ch=%d, bps=%d\n", i1,i2,i3);
			//	Error(temp);
				return TRUE;
			}
		}
	}

	return FALSE;
}

//***************************************************************************
BOOL GetOutputAudioInfo(CComQIPtr<IBaseFilter> pBaseFilter, int* out_frequency, int* out_bytes_per_sample, int* out_stereo)
{
	HRESULT hr;
	BOOL retVal = FALSE;

	// Pin enumeration
	CComQIPtr<IEnumPins> pEnumPins ;
	hr = pBaseFilter->EnumPins(&pEnumPins);
	if (FAILED(hr)) 
	{
		Error("Audio ripper could not enumerate pins on a base filter");
		return FALSE;
	}

	IPin* pPin;
	while (pEnumPins->Next(1, &pPin, 0) == S_OK) 
	{	
		retVal = GetOutputAudioInfo(pPin, out_frequency, out_bytes_per_sample, out_stereo);
		pPin->Release();

		if (retVal==TRUE) return TRUE;
	}

	return retVal;
}



//***************************************************************************
bool IsConnectedPinTypeAudio(IPin* pPin, bool* is_ac3_51)
{
	AM_MEDIA_TYPE mt1;

	HRESULT hr = pPin->ConnectionMediaType(&mt1);

	if (SUCCEEDED(hr))
	{
		Trace("Audio ripper looking for majour type audio");

		if (mt1.majortype == MEDIATYPE_Audio)
		{
			if (is_ac3_51 != NULL)
			{
				if (mt1.subtype == MEDIASUBTYPE_DOLBY_AC3 ||
					mt1.subtype == MEDIASUBTYPE_DOLBY_AC3_SPDIF ||
					mt1.subtype == MEDIASUBTYPE_WAVE_DOLBY_AC3)
				{
					Trace("Subtype was ac3 audio");
					*is_ac3_51 = true;

					// is ac3 stereo or mono then treat as non ac3 as we are really only intrested in ac3 5.1
					if (mt1.cbFormat >= sizeof(PCMWAVEFORMAT))
					{
						WAVEFORMATEX *pwfx = (WAVEFORMATEX *)mt1.pbFormat;
						if (pwfx->nChannels <= 2)
						{
							Trace("Although ac3, it had two or less channels");
							*is_ac3_51 = false;
						}

					}
				}
			}
			return true;
		}
	}

	return false;
}

// MEDIASUBTYPE_DOLBY_AC3 
// MEDIASUBTYPE_DOLBY_AC3_SPDIF


//***************************************************************************
bool IsConnectedPinTypeVideo(IPin* pPin)
{

	Trace("Found streamconfig");
	AM_MEDIA_TYPE mt1;

	HRESULT hr = pPin->ConnectionMediaType(&mt1);

	if (SUCCEEDED(hr))
	{
		Trace("Looking for majour type video ");
		if (mt1.majortype == MEDIATYPE_Video) return true;
	}

	return false;

}

// this the_pcm_codec contains:-  ( this is used to describe the format we want any ripped sound to be in)
// 16bit 44100 stereo (VCD,SVCD)  and..
// 16bit 48000 stereo (DVD)   

CDSCodec* the_pcm_codec = NULL ;

//***************************************************************************
void CAudioRipper::BuildCodecArray()
{
	HRESULT				hr;
	CComQIPtr<ICreateDevEnum>		pSysDevEnum;
	CComQIPtr<IEnumMoniker>		pEnum;
	IMoniker*			pMoniker;


	// aready build return !
	static bool built_codec_array = false;
	if (built_codec_array==true) return ;

	wchar_t PCM_NAME[] = L"PCM" ;

	// System Device Enumerator
	hr = CoCreateInstance(CLSID_SystemDeviceEnum, NULL, CLSCTX_INPROC_SERVER, IID_ICreateDevEnum, (void**) &pSysDevEnum);
	if (FAILED(hr)) 
	{
		Error("Audio ripper could not create SystemDeviceEnum");
		return;
	}

	// Moniker Enumerator
	hr = pSysDevEnum->CreateClassEnumerator(CLSID_AudioCompressorCategory, &pEnum, 0);
	if (FAILED(hr))
	{
		Error("Audio ripper could not create Class Enumerator for Audio Compressor Category");
		return;
	}

	// Cycle throught IMoniker collection
	while (pEnum->Next(1, &pMoniker, NULL) == S_OK) 
	{
		// New instance of CDSCodec

		// Retrieve codec name
		CComQIPtr<IPropertyBag> pPropertyBag;
		hr = pMoniker->BindToStorage(NULL, NULL, IID_IPropertyBag, (void**) &pPropertyBag);
		if (SUCCEEDED(hr)) {
			VARIANT var;
			VariantInit(&var);
			pPropertyBag->Read(L"FriendlyName", &var, NULL);
			BSTR s = var.bstrVal;

			if (memcmp(PCM_NAME, s,6)==0)
			{
				the_pcm_codec = new CDSCodec();
				the_pcm_codec->m_pMoniker = pMoniker;
				the_pcm_codec->BuildCodecFormatArray();
			}
			
		//	pCodec->m_szCodecName = szTempName;
			VariantClear(&var);
			// Add new instance to collection
		//	Add(pCodec);
		//	pCodec->BuildCodecFormatArray();
		} else {
			//delete pCodec;
		}
	}

	// Libération des enumerators
	built_codec_array=true ;
}


void CAudioRipper::SetRipAbortFlag(int val)
{
	if (val == 0)
	{
		mAbort = false;
	}
	else
	{
		mAbort = true;
	}
}



// ***********************************************************************************************************
// This will remove all video based filters starting with the video renderer filter and working back upstream
// until we find the splitter filter.  This is needed as some graphs won't run without a renderer if it contains 
// a video decoder filter, but it is slow to run with one (even with the null renderer)
static void RemoveVideoFilters(CComQIPtr<IGraphBuilder> graphBuilder, CComQIPtr<IBaseFilter> videoRenderer)
{
	bool currentFilterIsSpliiter = false;
	CComQIPtr<IBaseFilter> currentFilter = videoRenderer;

	//
	// Iterate upstream until we find the splitter filter
	//
	while (currentFilterIsSpliiter == false)
	{
		//
		// Enumerate all pins on current filter
		//
		CComPtr<IEnumPins>	EnumPins;
		HRESULT hr = currentFilter->EnumPins(&EnumPins);
		if (FAILED(hr)) 
		{
			Error("RemoveVideoFilters failed to enumerate filter pins ");
			break ;
		}

		EnumPins->Reset();
		CComPtr<IPin> currentFilterPin;
		CComPtr<IPin> upstreamFilterOutPin;

		bool foundPins = false;

		while (EnumPins->Next(1, &currentFilterPin.p, NULL) == S_OK)
		{
			foundPins = true;

			//
			// Query pin on filter
			//
			PIN_INFO pinfo;
			currentFilterPin->QueryPinInfo(&pinfo);
			pinfo.pFilter->Release();
			
			if (pinfo.dir == PINDIR_INPUT )
			{
				//
				// Set upstream pin
				//
				currentFilterPin->ConnectedTo(&upstreamFilterOutPin.p);	
			}

			if (pinfo.dir == PINDIR_OUTPUT )
			{
				//
				// Is output pin connected with audio media 
				//
				bool ac3=false;
				if (IsConnectedPinTypeAudio(currentFilterPin, &ac3)==true)
				{
					// 
					// Assume this is spliiter filter, break
					//
					currentFilterIsSpliiter = true;
					break;
				}
			}
		}

		if (foundPins==false || upstreamFilterOutPin == NULL)
		{
			//
			// If no pins detected on filter or no upstream pin set, then quit
			//
			break;
		}

		if (currentFilterIsSpliiter == false)
		{
			//
			//  Remove current filter from graph and set next filter to the upstream filter
			//
			PIN_INFO pinfo;
			upstreamFilterOutPin->QueryPinInfo(&pinfo);
			graphBuilder->RemoveFilter(currentFilter);
			currentFilter = pinfo.pFilter;
			pinfo.pFilter->Release();
		}	
	}
}

//***************************************************************************
static void RemoveFilterIfNotUsed(IBaseFilter* filter, IGraphBuilder* grapgh)
{
	PIN_INFO pinfo;
	IPin* InPin = NULL;
	CComQIPtr<IEnumPins> EnumPins;
	ULONG			fetched;
	if (filter == NULL) return;

	HRESULT hr = filter->EnumPins(&EnumPins);

	if (FAILED(hr)) return;

	bool filter_used = false;

	while (EnumPins->Next(1, &InPin, &fetched) == S_OK)
	{
		if (InPin == NULL) break;
		InPin->QueryPinInfo(&pinfo);
		pinfo.pFilter->Release();

		IPin* other_pin = NULL;
		hr = InPin->ConnectedTo(&other_pin);
		if (other_pin != NULL)
		{
			other_pin->Release();
		}

		if (pinfo.dir == PINDIR_INPUT &&
			hr != VFW_E_NOT_CONNECTED)
		{
			filter_used = true;
		}
		InPin->Release();
	}

	if (filter_used == false)
	{
		if (!SUCCEEDED(grapgh->RemoveFilter(filter)))
		{
			Error("Failed to remove filter from audio ripper graph");
		}
	}
}



//***************************************************************************
// result
// 0 ripped and resampled OR AC3 Audio (i.e. no more processing required (re sample only occurs because outside of normal PhotoVidShow resampler range))
// 1 failed
// 2 No longer used
// 3 ripped but did not resample  (the normal case now for pcm audio as photovidshow does the resampling process)
// 4 failed because no audio renderer found.

int CAudioRipper::BuildAndRunGraph(char* in_filename, 
					 char* out_filename,
					 int desired_resample_auido_frequency,
					 int* out_frequency,
					 int* out_bytes_per_sample,
					 int* out_stereo,
					 bool useLav)
{
	bool ripped_without_resample = false;

	CComQIPtr<IGraphBuilder> g_pGraphBuilder; 
	CComQIPtr<IEnumFilters>	EnumFilters ;
	CComQIPtr<IMediaFilter>  pMediaFilter ;
	CComQIPtr<IBaseFilter>	AudioRenderer;
	CComQIPtr<IBaseFilter>	ac3filter;
	CComQIPtr<IBaseFilter>	VideoRenderer;
	CComQIPtr<IBaseFilter>	WavDest ;
	CComQIPtr<IBaseFilter>	pDest ;
	CComQIPtr<IBaseFilter>  pNullRenderer;

	bool		FoundRenderer = false;	
	CComQIPtr<IPin>		temp_video_in_pin;
	CComQIPtr<IPin>		ac3_currentFilterPin;
	ULONG		fetched = 0;
	CComQIPtr<IBaseFilter> pACMWrapper;
	bool audio_is_ac3 = false;
	mAbort=false;

	Trace("Starting audio ripping for media file '%s'",in_filename);

	CComQIPtr<IMediaControl>pControl ;
	PIN_INFO	pinfo;
	CComQIPtr<IFileSinkFilter> pSink;
	CComQIPtr<IFileSourceFilter> pSourceFilter;
	CComQIPtr<IBaseFilter> LavVideoDecoderFilter;

	nChannel =0;
	nBitsPerSample=0;
	nSamplesPerSec =0;
	nAvgBytesPerSec=0;

	HRESULT hr;
	hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER, 
                        IID_IGraphBuilder, (void **)&g_pGraphBuilder);
    if (FAILED(hr))
    {
        Error("Audio ripper could not create the filter graph manager.");
        return 1;
    }

	int		numoutputpins = 0;	

	static WCHAR w_out_filename[4096];

	for (int i=0;i<4096;i++)
	{
		w_out_filename[i]=0;
	}
	ConvertStr(out_filename, w_out_filename);


	LAVFilterHelper MyLAVFilterHelper;

	if (useLav == true)
	{
		//
		// Add the lav video decoder just in case. (We don't need this, but perhaps stops other video filters being used instead and making the graph initially crash)
		//
		hr = MyLAVFilterHelper.CreateLAVVideoDecoder((void**)&LavVideoDecoderFilter);

		if (SUCCEEDED(hr))
		{
			hr = g_pGraphBuilder->AddFilter(LavVideoDecoderFilter, L"Lav video decoder");
			if (FAILED(hr))
			{
				Error("Failed to add lav video filter, when audio ripping");
			}
		}
	}

	BOOL useLavSplitterAndAudioDecoder = (useLav == true ? TRUE : FALSE);

	BOOL result = CreateAndRenderGraph(in_filename, g_pGraphBuilder, useLavSplitterAndAudioDecoder);

	if (result == FALSE)
	{
		Error("Audio ripper failed to build graph for media file '%s' code:%08x",in_filename, hr);
		return 1;
	}

	if (GetLogLevel() >= LOGLEVEL_TRACE)
	{
		LogGraph(g_pGraphBuilder, in_filename);
	}

	//Trace("looking for audio renderers in graph!");

	g_pGraphBuilder->EnumFilters(&EnumFilters);
	EnumFilters->Reset();
	while (1==1)
	{
		CComQIPtr<IBaseFilter> Renderer;
		if (EnumFilters==NULL) break ;

		if ( S_OK != EnumFilters->Next(1, &Renderer, &fetched) )
		{
			break ;
		}
	
		if (FAILED(hr)) break ;

		if (Renderer==NULL) break ;

		CComQIPtr<IEnumPins>	EnumPins;

		hr = Renderer->EnumPins(&EnumPins);
		if (FAILED(hr)) break ;

		if(EnumPins==NULL) break ;

		EnumPins->Reset();
		numoutputpins = 0;
		bool has_audio_in_pin = false ;
		bool has_video_in_pin = false ;
		
		CComQIPtr<IPin> temp_in_pin ;
		IPin* currentFilterPin;

		while (EnumPins->Next(1, &currentFilterPin, &fetched) == S_OK)
		{
			if (currentFilterPin==NULL) break ;

			currentFilterPin->QueryPinInfo(&pinfo);
			pinfo.pFilter->Release();
			
			if (pinfo.dir == PINDIR_INPUT )
			{
				//currentFilterPin->ConnectedTo(&OutPin);	// Disconnect the filters - note that we have to call Disconnect for both pins
				bool ac3 = false;
				if (IsConnectedPinTypeAudio(currentFilterPin,&ac3))
				{
					if (ac3==true)
					{
						audio_is_ac3=true;
					}

					temp_in_pin = currentFilterPin;
					has_audio_in_pin=true;
				}
				else if (IsConnectedPinTypeVideo(currentFilterPin))
				{
					has_video_in_pin=true;
					temp_video_in_pin = currentFilterPin;
				}

			}

			if (pinfo.dir == PINDIR_OUTPUT )
			{
				numoutputpins++;
				//break ;
			}
			currentFilterPin->Release();
		}
	
		if (numoutputpins == 0)
		{
			if (has_audio_in_pin)
			{
				AudioRenderer = Renderer;
				Trace("Found audio renderer");
			}

			if (has_video_in_pin)
			{
				VideoRenderer = Renderer;
				Trace("Found video renderer");
			}

		}
		else if (numoutputpins == 1)
		{
			if (has_audio_in_pin)
			{
				ac3filter = Renderer;
				ac3_currentFilterPin = temp_in_pin;
				Trace("Found possible ac3 filter");
			}
		}
	}	

	if (AudioRenderer==NULL)
	{
		Warning("No audio renderer found when building audio ripper graph");
		return 4 ;
	}

	// Some media files which contain video streams as well won't run unless they are connected
	// to a null video renderer. So remove all the video based filters
	if (VideoRenderer!=NULL)
	{
		RemoveVideoFilters(g_pGraphBuilder, VideoRenderer);
	}

	//
	// Remove this filter (may not be connected at all)
	//
	RemoveFilterIfNotUsed(LavVideoDecoderFilter, g_pGraphBuilder);

	CComQIPtr<IEnumPins> AudioEnumPins;

	hr= AudioRenderer->EnumPins(&AudioEnumPins);
	if (FAILED(hr))
	{
		Error("Audio ripper failed to enumerate pins for the audio renderer");
		return 1;
	}

	CComQIPtr<IPin> AudioInPin;
	CComQIPtr<IPin> AudioOutPin;
	AudioEnumPins->Reset();
	AudioEnumPins->Next(1, &AudioInPin, &fetched);	// first one is only one
	
	hr =AudioInPin->ConnectedTo(&AudioOutPin);	// Disconnect the filters - note that we have to call Disconnect for both pins
	if (FAILED(hr))
	{
		Error("Audio ripper failed to find the pin connected to another pin");
		return 1;
	}

	hr = g_pGraphBuilder->Disconnect(AudioInPin);
	if (FAILED(hr))
	{
		Error("Audio ripper failed to disconnect a pin");
		return 1;
	}
	hr = g_pGraphBuilder->Disconnect(AudioOutPin);
	if (FAILED(hr))
	{
		Error("Audio ripper failed to disconnect a pin");
		return 1;
	}

	hr = g_pGraphBuilder->RemoveFilter(AudioRenderer);
	if (FAILED(hr))
	{
		Error("Audio ripper failed to remove audio renderer filter from graph");
		return 1;
	}

	if (the_pcm_codec == NULL)
	{
		Error("Audio ripper had NULL PCM filter, Windows registry or components could be corrupt");
		return 1;
	}

	CComQIPtr<IMoniker> pMoniker = the_pcm_codec->m_pMoniker;;
	pMoniker->BindToObject(NULL, NULL, IID_IBaseFilter, (void**) &pACMWrapper);
	hr = g_pGraphBuilder->AddFilter(pACMWrapper, L"ACM Codec");

	if (FAILED(hr))
	{
		Error("Audio ripper failed to add PCM ACM wrapper to graph");
		return 1;
	}
	
	HRESULT wavDestCreeationResult = S_OK;
	CWavDestFilter* wavDestFilter = new CWavDestFilter(NULL, &wavDestCreeationResult);
	if (FAILED(wavDestCreeationResult))
	{
		Error("DirectShowPlayer could not create the PVS WAV Dest Filter.");
		return 1;
	}

	//
	// COM now takes ownership of filter and will release when local reference when function returns
	//
	hr = wavDestFilter->QueryInterface(IID_IBaseFilter, (void**)&WavDest);
	if (FAILED(hr))
	{
		Error("DirectShowPlayer could not query interface PVS WAV Dest Filter.");
		return 1;
	}

	hr = g_pGraphBuilder->AddFilter(WavDest, NULL);
	if (FAILED(hr))
	{
		Error("Audio ripper failed add WavDest filter");
		return 1;
	}

	//
	// found ac3 filter fix with G825
	//
	/*
	if (1==2 && audio_is_ac3==true && ac3_currentFilterPin != NULL)
	{
		Trace("Audio is being ripped as ac3 data");
		ac3_currentFilterPin->ConnectedTo(&AudioOutPin);	// Disconnect the filters - note that we have to call Disconnect for both pins

		g_pGraphBuilder->Disconnect(ac3_currentFilterPin);
		g_pGraphBuilder->Disconnect(AudioOutPin);

		hr= ConnectFilters(g_pGraphBuilder, AudioOutPin, WavDest);
		if (FAILED(hr))
		{
			Error("Audio ripper could not connect filters OutPin and WavDest");
			return 1;
		}
	}
	else
	*/

	{
		Trace("Audio is being ripped as pcm data");

		hr = ConnectFilters(g_pGraphBuilder, AudioOutPin, pACMWrapper);
		if (FAILED(hr))
		{
			Error("Audio ripper could not connect filters OutPin and ACMWrapper");
			return 1;
		}

		// set output format of audio stream BEFORE connecting to wav dest filter as this
		// use to crash with certain videos ("e.g. stus' 16 bit 8112 mono videos);

		// ok only set filter format if WE DO want this to resampled for us
		// i.e PhotoVidShow can't hadle the re-sample

		if (GetOutputAudioInfo(pACMWrapper, out_frequency, out_bytes_per_sample, out_stereo)==TRUE)
		{
			if (*out_frequency <= desired_resample_auido_frequency &&
			    (*out_bytes_per_sample==1 || *out_bytes_per_sample==2))
			{
				ripped_without_resample=true;
			}
		}

		// causing program not to use in built re sampling
		//ripped_without_resample=false;

		if (ripped_without_resample==false)
		{
			if (desired_resample_auido_frequency==44100)
			{ 
				SetFilterFormat(the_pcm_codec->mTheCodecFormat44100->m_pMediaType, pACMWrapper);
			}
			else
			{
				SetFilterFormat(the_pcm_codec->mTheCodecFormat48000->m_pMediaType, pACMWrapper);
			}
		}
		
		hr= ConnectFilters(g_pGraphBuilder, pACMWrapper, WavDest);
		if (FAILED(hr))
		{
			// ok could not connect to wav dest, we've seen a case of this when the average
			// bytes per sec did not macth frequewncy.

			if (nAvgBytesPerSec != nSamplesPerSec * nChannel * (nBitsPerSample>>3) )
			{
				PopupError("Corrupt media file when ripping audio! channels=%d, BitePerSample=%d, SamplesPerSecond=%d, BytesPerSecond=%d", 
				nChannel,nBitsPerSample,nSamplesPerSec,nAvgBytesPerSec);
			}

			Error("Audio ripper could not connect filters pACMWrapper and WavDest");
			return 1;
		}

	}

	// Output file filter

	hr = CoCreateInstance(CLSID_FileWriter, NULL, CLSCTX_INPROC_SERVER, IID_IBaseFilter, (void**)&pDest);
	if (FAILED(hr))
	{
		Error("Audio ripper Could not create file writer filter");
		return 1;
	}

	hr = g_pGraphBuilder->AddFilter(pDest, NULL);
	if (FAILED(hr))
	{
		Error("Audio ripper could not add file writer to graph");
		return 1;
	}

	hr= ConnectFilters(g_pGraphBuilder, WavDest,  pDest);
	if (FAILED(hr))
	{
		Error("Audio ripper could not connect filters WavDest with FileWriter");
		return 1;
	}

	hr= pDest->QueryInterface(IID_IFileSinkFilter, (void**) &pSink);
	if (FAILED(hr))
	{
		Error("Auido ripper could not get interface for FileSinkFilter");
		return 1;
	}

	hr= pSink->SetFileName(w_out_filename, NULL);
	if (FAILED(hr))
	{
		Error("Audio ripper could not set output filename on the file sink filter");
		return 1;
	}

	hr = g_pGraphBuilder->QueryInterface(IID_IMediaControl, (void **)&pControl);
    if (FAILED(hr))
    {
        Error("Audio ripper could not get the media control interface.");
        return 1 ;
	}


	hr = pControl->Run();

	if (FAILED(hr))
	{	
		Error("Audio ripper could not rip auido.");
		  return 1;
	}

	long nCode = 0;
	CComQIPtr<IMediaEvent> pMediaEvent;
	hr = g_pGraphBuilder->QueryInterface(IID_IMediaEvent, (void**) &pMediaEvent);
	if (FAILED(hr))
	{
		Error("Audio ripper could got get an media event interface");
		return 1;
	}

	int nPercentComplete = 0;

	// Wait until job complete
	while (nCode != EC_COMPLETE && mAbort==false)
	{
		hr = pMediaEvent->WaitForCompletion(1000, &nCode);
		Trace("Ripping Audio....");
		// get position
	}

	pControl->Stop();

	Trace("Finished ripping audio");

	if (ripped_without_resample==true) return 3;

	return 0;
}

