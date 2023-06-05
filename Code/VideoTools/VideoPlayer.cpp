
#define _CRT_SECURE_NO_WARNINGS 1
#include "VideoPlayer.h"
#include "UnmanagedErrors.h"
#include <initguid.h>
#include <amvideo.h>
#include <uuids.h>
#include <Dvdmedia.h>
#include "LAVFilterHelper.h"

// elecard
//@device:sw:{083863F1-70DE-11D0-BD40-00A0C911CE86}\{BC4EB321-771F-4E9F-AF67-37C631ECA106}

//@device:sw:{083863F1-70DE-11D0-BD40-00A0C911CE86}\{CE1B27BE-851B-45DD-AB26-44389A8F71B4}

// GPL mpeg 2 
DEFINE_GUID(CLSID_MPEG2GPVideoCodec,
0xCE1B27BE, 0x851B, 0x45DD, 0xab, 0x26, 0x44, 0x38, 0x9a, 0x8f, 0x71, 0xb4);

// ffdshow video decoder
DEFINE_GUID(CLSID_FFDSHOWVideoCodec,
0x04FE9017, 0xF873, 0x410E, 0x87, 0x1E, 0xab, 0x91, 0x66, 0x1a, 0x4e, 0xf7);

// ms video decoder dtv-dvd // 212690FB-83E5-4526-8FD7-74478B7939CD	
DEFINE_GUID(CLSID_MSVideoDecoderDTV,
	0x212690FB, 0x83E5, 0x4526, 0x8f, 0xd7, 0x74, 0x47, 0x8b, 0x79, 0x39, 0xcd);

//
// microsoft audio decoder DTV-DVD  E1F1A0B8-BEEE-490D-BA7C-066C40B5E2B9
//
DEFINE_GUID(CLSID_MSAudioDecoderDTV,
	0xE1F1A0B8, 0xBEEE, 0x490D, 0xba, 0x7c, 0x06, 0x6c, 0x40, 0xb5, 0xe2, 0xb9);



WCHAR * ConvertStr(char * szIn, WCHAR* szOut)
{
 DWORD dwSize = strlen(szIn);
 DWORD count = mbstowcs(NULL, szIn, dwSize);
 mbstowcs(szOut, szIn, dwSize);
 return szOut;
}

double fcurrent=0;

extern void bmpcopy(void* src, void* dest,int width,int height, int srcbpp, int destbpp);

//***************************************************************************
static void LogDisplayType(char* label, const AM_MEDIA_TYPE *pmtIn)
{
	// SRG TO DO needs GUIDE NAMES
    // Dump the GUID types and a short description 

    Trace("");
    Trace("%s  M type %hs  S type %hs", label,
	    GuidNames[pmtIn->majortype],
	    GuidNames[pmtIn->subtype]);
    Trace("Subtype description %s",GetSubtypeName(&pmtIn->subtype));

    // Dump the generic media types 

    if (pmtIn->bTemporalCompression) {
	Trace("Temporally compressed");
    } else {
	Trace("Not temporally compressed");
    }

    if (pmtIn->bFixedSizeSamples) {
	Trace("Sample size %d",pmtIn->lSampleSize);
    } else {
	Trace("Variable size samples");
    }

    if (pmtIn->formattype == FORMAT_VideoInfo) {
	// Dump the contents of the BITMAPINFOHEADER structure 
	BITMAPINFOHEADER *pbmi = HEADER(pmtIn->pbFormat);
	VIDEOINFOHEADER *pVideoInfo = (VIDEOINFOHEADER *)pmtIn->pbFormat;

	Trace("Source rectangle (Left %d Top %d Right %d Bottom %d)",
	       pVideoInfo->rcSource.left,
	       pVideoInfo->rcSource.top,
	       pVideoInfo->rcSource.right,
	       pVideoInfo->rcSource.bottom);

	Trace("Target rectangle (Left %d Top %d Right %d Bottom %d)",
	       pVideoInfo->rcTarget.left,
	       pVideoInfo->rcTarget.top,
	       pVideoInfo->rcTarget.right,
	       pVideoInfo->rcTarget.bottom);

	Trace("Size of BITMAPINFO structure %d",pbmi->biSize);
	if (pbmi->biCompression < 256) {
	    Trace("%dx%dx%d bit  (%d)",
		    pbmi->biWidth, pbmi->biHeight,
		    pbmi->biBitCount, pbmi->biCompression);
	} else {
	    Trace("%dx%dx%d bit '%4.4hs'",
		    pbmi->biWidth, pbmi->biHeight,
		    pbmi->biBitCount, &pbmi->biCompression);
	}

	Trace("Image size %d",pbmi->biSizeImage);
	Trace("Planes %d",pbmi->biPlanes);
	Trace("X Pels per metre %d",pbmi->biXPelsPerMeter);
	Trace("Y Pels per metre %d",pbmi->biYPelsPerMeter);
	Trace("Colours used %d",pbmi->biClrUsed);

    } else if (pmtIn->majortype == MEDIATYPE_Audio) {
	Trace("     Format type %hs",
	    GuidNames[pmtIn->formattype]);
	Trace("     Subtype %hs",
	    GuidNames[pmtIn->subtype]);

	if ((pmtIn->subtype != MEDIASUBTYPE_MPEG1Packet)
	  && (pmtIn->cbFormat >= sizeof(PCMWAVEFORMAT)))
	{
	    // Dump the contents of the WAVEFORMATEX type-specific format structure 

	    WAVEFORMATEX *pwfx = (WAVEFORMATEX *) pmtIn->pbFormat;
            Trace("wFormatTag %u", pwfx->wFormatTag);
            Trace("nChannels %u", pwfx->nChannels);
            Trace("nSamplesPerSec %lu", pwfx->nSamplesPerSec);
            Trace("nAvgBytesPerSec %lu", pwfx->nAvgBytesPerSec);
            Trace("nBlockAlign %u", pwfx->nBlockAlign);
            Trace("wBitsPerSample %u", pwfx->wBitsPerSample);

            // PCM uses a WAVEFORMAT and does not have the extra size field 

            if (pmtIn->cbFormat >= sizeof(WAVEFORMATEX)) {
                Trace("cbSize %u", pwfx->cbSize);
            }
	} else {
	}

    } else {
	Trace("     Format type %hs",
	    GuidNames[pmtIn->formattype]);
	// !!!! should add code to dump wave format, others
    }
	

}

//***************************************************************************
void LogGraph(IFilterGraph *pGraph, char* filename)
{
	if( !pGraph )
    {
        return;
    }

	Trace("------------------------------");

    IEnumFilters *pFilters;

    Trace("DumpGraph [%x] %s", pGraph, filename);

    if (FAILED(pGraph->EnumFilters(&pFilters))) 
	{
		Trace("EnumFilters failed");
		return;
    }

    IBaseFilter *pFilter;
    ULONG	n;
    while (pFilters->Next(1, &pFilter, &n) == S_OK) {
	FILTER_INFO	info;

	if (FAILED(pFilter->QueryFilterInfo(&info))) {
	    Trace("    Filter [%x]  -- failed QueryFilterInfo", pFilter);
	} else {
	    QueryFilterInfoReleaseGraph(info);

	    // !!! should QueryVendorInfo here!
	
	    Trace("    Filter [%x]  '%ls'", pFilter, info.achName);

	    IEnumPins *pins;

	    if (FAILED(pFilter->EnumPins(&pins))) {
		Trace("EnumPins failed!");
	    } else {

		IPin *pPin;
		while (pins->Next(1, &pPin, &n) == S_OK) {
		    PIN_INFO	info;

		    if (FAILED(pPin->QueryPinInfo(&info))) {
			Trace("          Pin [%x]  -- failed QueryPinInfo", pPin);
		    } else {
			QueryPinInfoReleaseFilter(info);

			IPin *pPinConnected = NULL;

			HRESULT hr = pPin->ConnectedTo(&pPinConnected);

			if (pPinConnected) {
			    Trace("          Pin [%x]  '%ls' [%sput]  Connected to pin [%x]",
				    pPin, info.achName,
				    info.dir == PINDIR_INPUT ?"In" : "Out",
				    pPinConnected);

			    pPinConnected->Release();

			    // perhaps we should really dump the type both ways as a sanity
			    // check?
			    if (info.dir == PINDIR_OUTPUT) {
				AM_MEDIA_TYPE mt;

				hr = pPin->ConnectionMediaType(&mt);

				if (SUCCEEDED(hr)) {
				    LogDisplayType("Connection type", &mt);

				    FreeMediaType(mt);
				}
			    }
			} else {
			    Trace("          Pin [%x]  '%ls' [%sput]",
				    pPin, info.achName,
				    info.dir == PINDIR_INPUT ? "In" : "Out");

			}
		    }

		    pPin->Release();
		}

		pins->Release();
	    }
	}
	
	pFilter->Release();
    }

    pFilters->Release();
	Trace("------------------------------");
	

}


//*******************************************************************************************
std::string CDirectShowPlayer::GetFirstFilterDetails()
{
	std::string result("");

	if (!pGraph)
	{
		return result;
	}

	if (FAILED(pGraph->EnumFilters(&mFiltersListIterator)))
	{
		Error("EnumFilters failed");
		return result;
	}

	return GetNextFilterDetails();
}


//*******************************************************************************************
std::string CDirectShowPlayer::GetNextFilterDetails()
{
	std::string result("");

	if (mFiltersListIterator == NULL)
	{
		return result;
	}

	IBaseFilter *pFilter;
	ULONG	n;
	if (mFiltersListIterator->Next(1, &pFilter, &n) == S_OK) {
		FILTER_INFO	info;

		if (FAILED(pFilter->QueryFilterInfo(&info))) {
			Error("    Filter [%x]  -- failed QueryFilterInfo", pFilter);
		}
		else {
			QueryFilterInfoReleaseGraph(info);

			// !!! should QueryVendorInfo here!

			char temp[1024] = { 0 };
			sprintf(temp,"Filter '%ls'", info.achName);
			result = std::string(temp);
		}

		pFilter->Release();
	}
	else
	{
		mFiltersListIterator->Release();
		mFiltersListIterator = NULL;
	}
	return result;
}


//*******************************************************************************************

// usefull direct show functions

HRESULT GetUnconnectedPin(
    IBaseFilter *pFilter,   // Pointer to the filter.
    PIN_DIRECTION PinDir,   // Direction of the pin to find.
    IPin **ppPin)           // Receives a pointer to the pin.
{
    *ppPin = 0;
    IEnumPins *pEnum = 0;
    IPin *pPin = 0;
    HRESULT hr = pFilter->EnumPins(&pEnum);
    if (FAILED(hr))
    {
        return hr;
    }
    while (pEnum->Next(1, &pPin, NULL) == S_OK)
    {
        PIN_DIRECTION ThisPinDir;
        pPin->QueryDirection(&ThisPinDir);
        if (ThisPinDir == PinDir)
        {
            IPin *pTmp = 0;
            hr = pPin->ConnectedTo(&pTmp);
            if (SUCCEEDED(hr))  // Already connected, not the pin we want.
            {
                pTmp->Release();
            }
            else  // Unconnected, this is the pin we want.
            {
                pEnum->Release();
                *ppPin = pPin;
                return S_OK;
            }
        }
        pPin->Release();
    }
    pEnum->Release();
    // Did not find a matching pin.
    return E_FAIL;
}


//***************************************************************************
HRESULT ConnectFilters(
    IGraphBuilder *pGraph, // Filter Graph Manager.
    IPin *pOut,            // Output pin on the upstream filter.
    IBaseFilter *pDest)    // Downstream filter.
{
    if ((pGraph == NULL) || (pOut == NULL) || (pDest == NULL))
    {
        return E_POINTER;
    }
#ifdef debug
        PIN_DIRECTION PinDir;
        pOut->QueryDirection(&PinDir);
        _ASSERTE(PinDir == PINDIR_OUTPUT);
#endif

    // Find an input pin on the downstream filter.
    IPin *pIn = 0;
    HRESULT hr = GetUnconnectedPin(pDest, PINDIR_INPUT, &pIn);
    if (FAILED(hr))
    {
        return hr;
    }
    // Try to connect them.
    hr = pGraph->Connect(pOut, pIn);
    pIn->Release();
    return hr;
}

//***************************************************************************
HRESULT ConnectFilters(
	IGraphBuilder *pGraph,	  // Filter Graph Manager.
	IBaseFilter *pSource,    // Upstream filter
	IPin *pIn)				 // Downstream pin.
{
	if ((pGraph == NULL) || (pIn == NULL) || (pSource == NULL))
	{
		return E_POINTER;
	}

	// Find an output pin on the upstream filter.
	IPin *pOut = 0;
	HRESULT hr = GetUnconnectedPin(pSource, PINDIR_OUTPUT, &pOut);
	if (FAILED(hr))
	{
		return hr;
	}
	// Try to connect them.
	hr = pGraph->Connect(pOut, pIn);
	pIn->Release();
	return hr;
}

HRESULT ConnectFilters(
    IGraphBuilder *pGraph, 
    IBaseFilter *pSrc, 
    IBaseFilter *pDest)
{
    if ((pGraph == NULL) || (pSrc == NULL) || (pDest == NULL))
    {
        return E_POINTER;
    }

    // Find an output pin on the first filter.
    IPin *pOut = 0;
    HRESULT hr = GetUnconnectedPin(pSrc, PINDIR_OUTPUT, &pOut);
    if (FAILED(hr)) 
    {
        return hr;
    }
    hr = ConnectFilters(pGraph, pOut, pDest);
    pOut->Release();
    return hr;
}
 

#define CheckPointer(p,ret) {if((p)==NULL) return (ret);}

//***************************************************************************
//***************************************************************************
//***************************************************************************
//***************************************************************************
//*******************************************************************************************
class CDirectShowPlayer;

class CDirectShowPlayerCallback : public ISampleGrabberCB 
{
	CDirectShowPlayer* for_player ;
	int hack_counter;

public:

	CDirectShowPlayerCallback(CDirectShowPlayer* player) { for_player = player;hack_counter=0;}

    STDMETHODIMP_(ULONG) AddRef()  { return 2; }
    STDMETHODIMP_(ULONG) Release() { return 1; }
 
    STDMETHODIMP QueryInterface(REFIID riid, void ** ppv)
    {
        CheckPointer(ppv, E_POINTER);
        
        if (riid == IID_ISampleGrabberCB || riid == IID_IUnknown) 
        {
            *ppv = (void *) static_cast<ISampleGrabberCB *>(this);
            return NOERROR;
        }    
        return E_NOINTERFACE;
    }

    STDMETHODIMP SampleCB( double SampleTime, IMediaSample * pSample );

    STDMETHODIMP BufferCB( double SampleTime, BYTE * pBuffer, long BufferLen )
	{
        return 0;
    }
   
};



//***************************************************************************

CDirectShowPlayer::CDirectShowPlayer(int id) 
{ 
	mID = id; 
	next_frame_ready = false;
	mFrameStepping=false; 
	hack_counter=0;
	mStopped = true;
	mStoppedAtStart=true;
	mLastSeekTime=0;
	mCachedVolume =0;
	mHasAudio = false;
	mHasAC3=false;
	mFrameRate= 0.0;

	pGrabber=NULL;
	pGrabberF=NULL;
	pSeeking=NULL;
	pFilter=NULL;
	pGraph=NULL;
	pMediaFilter=NULL;;
	pControl =NULL;;
	pEvent=NULL; ;
	pNullRenderer=NULL;;
	pDSoundRenderer=NULL;
	pBasicAudio=NULL;
	mLastSampleTime=-1;
	mInternalVideoFrameNum=0;
	hack_just_seeked_waiting_to_render=false;

	mNextFrameReadySignal = NULL;
	mWaitForAdvanceFrameSingnal = NULL;
	pCB=NULL;
	pCallback=NULL;
	mRunningNullClock=false;
	mFilename = NULL;
	framesCountedOnFrameStepping = 0;
	mFiltersListIterator = NULL;
} 

//***************************************************************************
CDirectShowPlayer::	~CDirectShowPlayer()
{
	ReleaseAndNullAll();
}


//***************************************************************************
void CDirectShowPlayer::ReleaseAndNullAll()
{
	if (GetLogLevel() >= LOGLEVEL_TRACE)
	{
		Trace("Releasing video %s", mFilename);
	}
	
	//
	// Make sure the graph is stopped because it may just be paused which can
	// cause a lock up when releasing the graph builder (noticed on windows 8)
	//
	if (pControl!=NULL)
	{
		if (FAILED(pControl->Stop()))
		{
			Error("Failed to stop video when releasing video");
		}

		long result = GetControlState();
		if (result == -1)
		{
			Error("Get control state failed in Release and Null");
		}

	}

	if (pGrabber!=NULL)
	{
		pGrabber->Release();
	}

	if (pGrabberF!=NULL)
	{
		pGrabberF->Release();
	}

	if (pSeeking!=NULL)
	{
		pSeeking->Release();
	}

	if (pCB!=NULL)
	{
		pCB->Release();
	}

	if (pFilter!=NULL)
	{
		pFilter->Release();
	}
	if (pMediaFilter!=NULL)
	{
		pMediaFilter->Release();
	}

	if (pControl!=NULL)
	{
		pControl->Release();
	}

	if (pEvent!=NULL)
	{
		pEvent->Release();
	}

	if (pNullRenderer!=NULL)
	{
		pNullRenderer->Release();
	}

	if (pDSoundRenderer!=NULL)
	{
		pDSoundRenderer->Release();
	}

	if (pBasicAudio!=NULL)
	{
		pBasicAudio->Release();
	}


	if (pGraph!=NULL)
	{
		pGraph->Release();
	}


	if (mNextFrameReadySignal!=NULL)
	{
		CloseHandle(mNextFrameReadySignal);
		mNextFrameReadySignal = NULL;
	}

	if (mWaitForAdvanceFrameSingnal!=NULL)
	{
		CloseHandle(mWaitForAdvanceFrameSingnal);
		mWaitForAdvanceFrameSingnal = NULL;
	}

	pGrabber=NULL;
	pGrabberF=NULL;
	pCB=NULL;
	pSeeking=NULL;
	pFilter=NULL;
	pGraph=NULL;
	pMediaFilter=NULL;;
	pControl =NULL;;
	pEvent=NULL; ;
	pNullRenderer=NULL;;
	pDSoundRenderer=NULL;
	pBasicAudio=NULL;
	hack_just_seeked_waiting_to_render=false;

	if (pCallback)
	{
		delete pCallback ;
		pCallback = NULL;
	}

	if (mFilename != NULL)
	{
		delete [] mFilename;
		mFilename = NULL;
	}
}



//***************************************************************************
void CDirectShowPlayer::GetDuration(hyper* h) 
{ 
	*h = mDuration ; 
}


//***************************************************************************
double CDirectShowPlayer::GetFrameRate()
{
	return mFrameRate;
}



//***************************************************************************
long CDirectShowPlayer::GetVolume()
{
	if (pBasicAudio==NULL) return 0;

	long vol=0;
	pBasicAudio->get_Volume(&vol);
	return vol;
}

//***************************************************************************
void  CDirectShowPlayer::SetVolume(long vol)
{
	if (pBasicAudio!=NULL) 
	{
		pBasicAudio->put_Volume(vol);
		this->mCachedVolume = vol;
	}
}

//***************************************************************************
bool CDirectShowPlayer::ContainsAudio()
{
	return mHasAudio;
}


//***************************************************************************
bool CDirectShowPlayer::HasDolbyAC3Sound()
{
	return mHasAC3;
}



//***************************************************************************
void CDirectShowPlayer::SeekToTime(double time, bool wait_for_completion)
{
#ifdef FULL_VIDEO_TRACE
	Trace("Seeking to time %f",time);
#endif

	if (GetLogLevel() >= LOGLEVEL_TRACE)
	{
		Trace("Seeking to time %f for %s", ((float)time), mFilename);
	}

	if (this->pSeeking==NULL)
	{
		Error("DirectShowPlayer can't seek to time as seeking interface was NULL");
		return ;
	}

	// 
	// If seeking to the very end of a video, then rewide 0.1 of a second.
	// On many videos, seeking 'right' to the end just does not work in DirectShow ImediaSeeking::setpostions
	// (i.e. a frame is never generated and hack_just_seeked_waiting_to_render is not set)
	//
	if (wait_for_completion == true)
	{
		double durationInSeconds = mDuration / 10000000.0;

		durationInSeconds -= 0.1;
		if (durationInSeconds >0.5 && time > durationInSeconds)	// only reduce time if video > 0.6 seconds long
		{
			time = durationInSeconds;
		}
	}

	hack_just_seeked_waiting_to_render = true;
	framesCountedOnFrameStepping = 0;

	REFERENCE_TIME Start = (REFERENCE_TIME)(time * 10000000.0f);
	{
		HRESULT videoSeekResult = pSeeking->SetPositions( &Start,    AM_SEEKING_AbsolutePositioning,  NULL, AM_SEEKING_NoPositioning );

		if (FAILED(videoSeekResult))
		{
			Error("pSeeking set positions failed %d", videoSeekResult);
		}
	}
	
	if (this->mStopped==true)
	{	
		pControl->Pause(); 
		if (wait_for_completion==true)
		{
			//	Trace("STOPED in SEEK SO WAITING FOR UPDATE");
			// SRG DV CAMERA
			
			OAFilterState state = GetControlState();
			if (state == -1)
			{
				Error("GetControlState failed in Seek to time %f for %s", ((float)time), mFilename);
			}

			//Error("State after seek = %d %s", state, mFilename);

			// ok mega hack, some mpeg2 decoders finish the get state but still have not
			// rendered !!!
			// wait until our sampler callback has recieved a frame
			// to stop infinite loop a give up of around 5 seconds.
			int give_up=0;
			while (hack_just_seeked_waiting_to_render==true && give_up<10)
			{
				::Sleep(100);
				give_up++;
			}

			if (give_up >= 10)
			{
				Error("Gave up waiting to render seeked frame %f %s", ((float)time), mFilename);
			}
		}
	}

	// don't really need this as mLastSampleTime is used when encoding and we never
	// seek when encoding
	mLastSampleTime=time-this->mFrameRate;
	mLastSeekTime=time;

	if (GetLogLevel() >= LOGLEVEL_TRACE)
	{
		Trace("Done seek to time %f for %s", ((float)time), mFilename);
	}
}

//***************************************************************************
double CDirectShowPlayer::GetLastSeekTime()
{
    return mLastSeekTime;
}


//***************************************************************************
bool CDirectShowPlayer::ReachedEnd()
{
	double duration = (((double)this->mDuration)/10000000.0);

	duration -=this->mFrameRate;
	duration -=0.001;


	if (mLastSampleTime >= duration) //((((double)this->mDuration)/10000000.0) - 0.001))
	{
		return true;
	}
	return false;

	/*

	if (!pSeeking) return true ;

	
	LONGLONG current;
	LONGLONG stop;

	pSeeking->GetPositions(&current,&stop);

	fcurrent = ((double)current)/10000000.0;

	//Trace("re %d %d %d", current/10000000, stop/10000000, mDuration/10000000);

	// DO NOT UNCOMMENT
//	Trace("finished reached end");
	if (current>=this->mDuration) 
	{
	//	Trace("Argggh!");
		return true ;
	}
	return false;
	*/

}

//***************************************************************************
OAFilterState CDirectShowPlayer::GetControlState()
{
	OAFilterState state = -1;

	HRESULT result = S_FALSE;
	int attempts =0;
	int reportedProblem =0;

	// Give it 10 seconds to find state else give up (just in case, prevents infinite lock up)
	int maxTries = 10;
	while (result != S_OK && attempts <maxTries)	// (SRG perhapd 300 when output video)
	{
		// ### SRG TO DO the above number should be low like 2 when seeking, higher when creating thumbanils, and perhaps 300 when output video etc
		result = pControl->GetState(100, &state); 

		if (result != S_OK) 
		{
			if (result == VFW_S_CANT_CUE)
			{
				// 
				// If the control can't CUE, then there is no point re-trying later, this has been seen on mpeg-2 files on windows7.
				// I.e. can not cue data frames up when in pause or stopped state.
				// 
				break;
			}

			if (result == VFW_S_STATE_INTERMEDIATE)
			{
				reportedProblem=1;
			}

			if(result== E_FAIL)
			{
				reportedProblem=2;
			}

			::Sleep(100);
			attempts++;
		}			
	}

	if (attempts>= maxTries)
	{
		Error("Media control gave up waiting for state (problem:%d) %s", reportedProblem, mFilename);
		state = -1;
	}

	return state;
}

//***************************************************************************
void CDirectShowPlayer::SetFrameStepping(bool value)
{
	mFrameStepping = value;
}


//***************************************************************************
void CDirectShowPlayer::Start(bool mute_sound)
{
#ifdef FULL_VIDEO_TRACE
	Trace("Called video player start mStopped = %d", mStopped);
#endif

	//Trace("Called video player start %s", mFilename);

//	if (mStopped = false)
//	{
//		Trace("Finished start");
//		return ;
//	}

	framesCountedOnFrameStepping = 0;

	if (mute_sound==true)
	{
		this->SetVolume(-10000);
	}
	else
	{
		this->SetVolume(this->mCachedVolume);
	}

	if (mFrameStepping==false)
	{
		//
		//  Check if the reference clock needs to be reset (after encoding)
		//
		if (mRunningNullClock==true)
		{
			if (FAILED(pControl->Stop()))
			{
				Error("Failed to stop video when resetting clock");
			}
			else
			{
				if (FAILED(pGraph->SetDefaultSyncSource()))
				{
					Error("Failed to set default clock on video");
				}
				else
				{
					mRunningNullClock = false;
				}
			}
		}
	}
	else
	{
		//
		// We can only set the clock when in stopped mode. We are most likely just paused.
		// Stopping does not seek back to time 0 on the video
		//
		if (FAILED(pControl->Stop()))
		{
			Error("Failed to stop video when setting sync source");
		}

		//
		// This makes the video playback as fast as possible.  Audio will still
		// play at the normal rate, but as this is not used and is mutted, this does
		// not effect us.  The video is also stopped after it has been encoded.
		//
		if (FAILED(pMediaFilter->SetSyncSource(NULL)))
		{
			Error("Failed to remove video file reference clock");
		}
		else
		{
			mRunningNullClock=true;
		}
	}

	next_frame_ready = false ;
	mLastSampleTime=-1;
	mInternalVideoFrameNum=0;

	if (pControl)
	{
		mStopped = false ;
		mStoppedAtStart=false;
		HRESULT result = pControl->Run();
		if (FAILED(result))
		{
			Error("Failed on call to pControl->Run %s %d", mFilename, result);
		}
	}

#ifdef FULL_VIDEO_TRACE
	Trace("Finished start");
#endif

}

//***************************************************************************
bool CDirectShowPlayer::IsStopped()
{
	if (mStopped==true) return true;
	return false;
}

//***************************************************************************
void CDirectShowPlayer::Stop(double reset_to_time, bool do_reset, bool do_pause)
{
	//Trace("Called video player stop %s", mFilename);


	if (mStopped == true &&
		(mStoppedAtStart==true || do_reset==false))
	{
	//	Trace("Finished stop");
		return ;
	}

	mStopped = true ;
	next_frame_ready = false ;

	// stop any thread stuff
	if (mFrameStepping == true)
	{
		ReleaseSemaphore( mWaitForAdvanceFrameSingnal,1, NULL); 
	}

	mStoppedAtStart=false;

	if (pControl)
	{
		REFERENCE_TIME Start = (REFERENCE_TIME)(reset_to_time * 10000000.0f);
		if (pSeeking && do_reset==true)
		{		
			//Error("Seeking to time %f %s", ((float)reset_to_time), mFilename);
			pSeeking->SetPositions(&Start, AM_SEEKING_AbsolutePositioning, NULL, AM_SEEKING_NoPositioning );
			mStoppedAtStart=true;
			mLastSeekTime=reset_to_time;
		}

		if (do_pause==true)
		{
			pControl->Pause();
		}
		else
		{
			pControl->Stop();
		}

		long result = GetControlState();	
		if (result == -1)
		{
			Error("GetControlState failed in Stop %f %d %d %s", ((float)reset_to_time), do_reset, do_pause, mFilename);
		}
		
	//	Sleep(20);
	}

	framesCountedOnFrameStepping = 0;

	//Trace("Finsihed stop= %s", mFilename);

}

//***************************************************************************
void CDirectShowPlayer::CleanInternalBuffers()
{
	if (pGrabber!=NULL)
	{	
		pGrabber->ClearBuffers();
	}
}


//***************************************************************************
void CDirectShowPlayer::AdvanceFrame()
{
#ifdef FULL_VIDEO_TRACE
	Trace("called advance frame, frame stepping is %d %d %f,",mFrameStepping, mInternalVideoFrameNum, fcurrent);
#endif


	if (mFrameStepping == false) return ;

	next_frame_ready = false ;

	// Tell the other thread it can render the next frame
	ReleaseSemaphore( mWaitForAdvanceFrameSingnal,1, NULL); 
}


//***************************************************************************
bool CDirectShowPlayer::GetImageInfo( ) 
{
    CMediaType mt;
    
	mWidth=0;
	mHeight=0;
	mBpp=0;
	mPlanes=0;

	if (pGrabber==NULL) return false ;

    pGrabber->GetConnectedMediaType(&mt);
     
    if (mt.formattype == FORMAT_VideoInfo) 
	{
	    VIDEOINFOHEADER *pVih;
		pVih = reinterpret_cast<VIDEOINFOHEADER*>(mt.pbFormat);
		
		//Get the information about the bitmap
		mWidth = pVih->bmiHeader.biWidth;
		mHeight = pVih->bmiHeader.biHeight;
		mBpp = pVih->bmiHeader.biBitCount;
		mPlanes = pVih->bmiHeader.biPlanes;
	}
	else if (mt.formattype == FORMAT_VideoInfo2)
	{
		VIDEOINFOHEADER2 *pVih;
		pVih = reinterpret_cast<VIDEOINFOHEADER2*>(mt.pbFormat);

		mWidth = pVih->bmiHeader.biWidth;
		mHeight = pVih->bmiHeader.biHeight;
		mBpp =pVih->bmiHeader.biBitCount;
		mPlanes = pVih->bmiHeader.biPlanes;
    }
	else
	{
		Error("Unknown format type connected to grabber, can not import video");
		return FALSE;
	}
    
    return TRUE;
}

extern bool IsConnectedPinTypeAudio(IPin *pPin, bool* is_ac3);
extern bool IsConnectedPinTypeVideo(IPin *pPin);


//***************************************************************************
void CDirectShowPlayer::RemoveFilterIfNotUsed(IBaseFilter* filter)
{
	PIN_INFO pinfo;
	IPin* InPin = NULL;
	CComQIPtr<IEnumPins> EnumPins;
	ULONG			fetched;
	if (filter==NULL) return ;

	HRESULT hr = filter->EnumPins(&EnumPins);

	if (FAILED(hr)) return ;

	bool filter_used = false;

	while (EnumPins->Next(1, &InPin, &fetched) == S_OK)
	{
		if (InPin==NULL) break ;
		InPin->QueryPinInfo(&pinfo);
		pinfo.pFilter->Release();
			
		IPin* other_pin = NULL;
		hr = InPin->ConnectedTo(&other_pin);
		if (other_pin!=NULL)
		{
			other_pin->Release();
		}

		if (pinfo.dir == PINDIR_INPUT &&
			hr!=VFW_E_NOT_CONNECTED)
		{
			filter_used = true;
		}
		InPin->Release();
	}

	if (filter_used==false)
	{
		pGraph->RemoveFilter(filter);
		Trace("Removed a video decoder Filter");
	}
}


//***************************************************************************
// return result meaning
// 0 loaded ok
// 1 Failed to load, probably no codec available
// 2 Same as 1 but ffdshow was detected and disabled for this app
int CDirectShowPlayer::Load(char* filename, bool useffdshowformp4, bool useffdshowformov, bool useffdshowformts, bool quarter1080pVideos)
{
	// try default codecs

	if (mFilename != NULL)
	{
		delete [] mFilename;
	}
	mFilename = new char[strlen(filename) + 1];
	strcpy(mFilename, filename);

	int filter =1;			// try to use lav for everything
	int filter_mask = 0;	

	LAVFilterHelper MyLavFilterHelper;
	int filetype = MyLavFilterHelper.GetFileType(filename);

	if (filetype == 1 && useffdshowformp4 == true)
	{
		filter_mask |= 1;
	}
	else if (filetype == 2 && useffdshowformov == true)
	{
		filter_mask |= 1;
	}
	else if (filetype == 3 && useffdshowformts == true)
	{
		filter_mask |= 1;
	}

	int detecedFFdShowButDisableForThisApp = 0;
	int res = 0;

	res = LoadWithResults(filename, filter, filter_mask, quarter1080pVideos);
	if (res == 3)
	{
		detecedFFdShowButDisableForThisApp = 1;
	}

	//
	// failed to load with lav filters but error code suggest trying again and letting directshow decide
	//
	if (res == 2 || res == 3)
	{
		filter = 0;

		ReleaseAndNullAll();

		res = LoadWithResults(filename, filter, filter_mask, quarter1080pVideos);

		if (res == 3)
		{
			detecedFFdShowButDisableForThisApp = 1;
		}
	}
	

	// worked
	if (res==1) 
	{
		return 0;
	}

	// failed but detected ffdshow
	if (res!=1 && detecedFFdShowButDisableForThisApp==1)
	{
		return 2;
	}

	// failed, reson no codec
	return 1;
}


// try filters
// 0 = just use default filters (let directshow decide)
// 1 = use lav spliiter and either lav decoders or possible ffshow (see mask settings below)

// filters mask (this parameter only applies if 1 is set in try_filters)
// 1 add ffdshow video filter before render instead of lav video decoder 

// return value
// 0 = did not work, reason unknown
// 1 = worked
// 2 = Failed to join sample grabber ( this means defualt filters perhaps don't work, try calling this again with 0 as try_filters )
// 3 = FFDShow detected but disabled for this app

//***************************************************************************
int CDirectShowPlayer::LoadWithResults(char* filename, int try_filters, int filters_mask, bool quarter1080pVideos)
{
#ifdef FULL_VIDEO_TRACE
//	Trace("Direct show player Loading.....%s", g_filename);
#endif

	FILE*fp= 0;

	static WCHAR pFilename[4096];

	if (GetLogLevel() >= LOGLEVEL_TRACE)
	{
		Trace("Loading video %s %d %d %d", filename, try_filters, filters_mask, quarter1080pVideos);
	}

	for (int i=0;i<4096;i++)
	{
		pFilename[i]=0;
	}
	ConvertStr(filename,pFilename);
	IBaseFilter*	AudioRenderer = NULL;
	IBaseFilter*	VideoRenderer = NULL;
	IEnumPins*		EnumPins;
	bool			FoundRenderer = false;	
	IEnumFilters*	EnumFilters;
	IBaseFilter*	Renderer;
	ULONG			fetched;
	IPin*			InPin;	// renderer input
	IPin*			OutPin;	// decoder or other filter output;
	PIN_INFO		pinfo;

	CComQIPtr<IBaseFilter> FFDShowDecoder;
	CComQIPtr<IBaseFilter> ThirdPartySourceSplitterFilter;
	CComQIPtr<IBaseFilter> LavAudioDecoderFilter;
	CComQIPtr<IBaseFilter> LavVideoDecoderFilter;

	LAVFilterHelper MyLAVFilterHelper;

    HRESULT hr;
    BOOL bFoundVideo = FALSE;
	
	if (pCallback != NULL) delete pCallback;
    pCallback = new CDirectShowPlayerCallback(this);

	if (mNextFrameReadySignal!=NULL)  CloseHandle(mNextFrameReadySignal);
    mNextFrameReadySignal = CreateSemaphore( 
        NULL,           // default security attributes
        0,  // initial count
        1,  // maximum count
        NULL);          // unnamed semaphore

	if (mWaitForAdvanceFrameSingnal!=NULL)  CloseHandle(mWaitForAdvanceFrameSingnal);
    mWaitForAdvanceFrameSingnal = CreateSemaphore( 
        NULL,           // default security attributes
        0,  // initial count
        1,  // maximum count
        NULL);          // unnamed semaphore

    if (!pFilename) return 0;

    // Create the filter graph manager and query for interfaces.
    hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER, 
                        IID_IGraphBuilder, (void **)&pGraph);
    if (FAILED(hr))
    {
		Error("DirectShowPlayer::Load Could not create the Filter Graph Manager.");
        return 0 ;
    }

	//
	// try_filters 1 = use lav / ffdshow 
	//
	if (try_filters==1)
	{	
		//
		// Create lav splitter
		//
		hr = MyLAVFilterHelper.CreateLAVSplitter((void**)&ThirdPartySourceSplitterFilter);

		if (SUCCEEDED(hr))
		{
			hr = pGraph->AddFilter(ThirdPartySourceSplitterFilter, L"Lav splitter");
			if (FAILED(hr))
			{
				Error("Failed to add lav splitter to filter");
			}
		}

		if (filters_mask == 0)
		{
			//
			// Add the lav video decoder
			//
			hr = MyLAVFilterHelper.CreateLAVVideoDecoder((void**)&LavVideoDecoderFilter);

			if (SUCCEEDED(hr))
			{
				hr = pGraph->AddFilter(LavVideoDecoderFilter, L"Lav video decoder");
				if (FAILED(hr))
				{
					Error("Failed to add lav video filter");
				}
			}
		}
		else
		{	
			//
			// Add ffdshow video decoder
			//
			hr = CoCreateInstance(CLSID_FFDSHOWVideoCodec, NULL, CLSCTX_INPROC_SERVER,
				IID_IBaseFilter, (void **)&FFDShowDecoder);
			if (SUCCEEDED(hr))
			{
				hr = pGraph->AddFilter(FFDShowDecoder, L"FFdshow video decoder");
				if (FAILED(hr))
				{
					Error("Failed to add ffdshow video decoder filter");
				}
			}

			if (hr == E_NOINTERFACE)
			{
				return 3;
			}			
		}

		//
		// Add the lav audio decoder
		//
		hr = MyLAVFilterHelper.CreateLAVAudioDecoder((void**)&LavAudioDecoderFilter);

		if (SUCCEEDED(hr))
		{
			hr = pGraph->AddFilter(LavAudioDecoderFilter, L"Lav audio decoder");
			if (FAILED(hr))
			{
				Error("Failed to add lav audio filter");
			}
		}
	}

	if (ThirdPartySourceSplitterFilter !=NULL)
	{
		hr = MyLAVFilterHelper.RenderFromSourceFilter(pGraph, ThirdPartySourceSplitterFilter, pFilename, true, true);

		// if failed remove lav splitter and try again
		if (FAILED(hr))
		{
			pGraph->RemoveFilter(ThirdPartySourceSplitterFilter);
			ThirdPartySourceSplitterFilter.Release();
			hr =  pGraph->RenderFile(pFilename,NULL);
		}
	}
	else
	{
		hr =  pGraph->RenderFile(pFilename,NULL);
	}

	if (FAILED(hr))
    {
        char tempStr[128] = {0};
        sprintf(tempStr,"DirectShowPlayer::Load could not render file. %d", hr);
		Error(tempStr);
        return 0 ;
    }

	RemoveFilterIfNotUsed(LavVideoDecoderFilter);	
	RemoveFilterIfNotUsed(FFDShowDecoder);
	RemoveFilterIfNotUsed(LavAudioDecoderFilter);
	
	if (GetLogLevel() >= LOGLEVEL_TRACE)
	{
		LogGraph(pGraph, mFilename);
	}

    hr = pGraph->QueryInterface(IID_IMediaControl, (void **)&pControl);
    if (FAILED(hr))
    {
        Error("DirectShowPlayer::Load could not get a media control interface.");
        return 0 ;
	}

    hr = pGraph->QueryInterface(IID_IMediaEvent, (void **)&pEvent);
	if (FAILED(hr))
    {
        Error("DirectShowPlayer::Load could not get a media event interface.");
        return  0 ;
	}

	hr = pGraph->QueryInterface(IID_IBasicAudio, (void **)&pBasicAudio);
	if (FAILED(hr))
    {
       Warning("DirectShowPlayer::Load could not get a basic adudio  interface.");
	}

	if (FAILED(hr))
	{
		Error("DirectShowPlayer::Load failed to render for media file '%s'",pFilename);
		return 0;;

	}

	pGraph->EnumFilters(&EnumFilters);
	EnumFilters->Reset();
	while (1==1)
	{
		if (EnumFilters==NULL) break ;

		if ( S_OK != EnumFilters->Next(1, &Renderer, &fetched) )
		{
			break ;
		}

		if (FAILED(hr)) break ;

		if (Renderer==NULL) break ;

		hr = Renderer->EnumPins(&EnumPins);
		if (FAILED(hr)) break ;

		if(EnumPins==NULL) break ;

		EnumPins->Reset();
		int numoutputpins = 0;
		bool has_audio_in_pin = false ;
		bool has_video_in_pin = false ;
		
		while (EnumPins->Next(1, &InPin, &fetched) == S_OK)
		{
			if (InPin==NULL) break ;

			InPin->QueryPinInfo(&pinfo);
			pinfo.pFilter->Release();
			
			if (pinfo.dir == PINDIR_INPUT )
			{
				bool ac3=false;
				if (IsConnectedPinTypeAudio(InPin,&ac3))
				{
					has_audio_in_pin=true;

					// only intrested if it is ac3 AND has more than 2 channels
					if (ac3==true)
					{
						mHasAC3=true;
					}

				}
				else if (IsConnectedPinTypeVideo(InPin))
				{
					has_video_in_pin=true;
				}

			}

			if (pinfo.dir == PINDIR_OUTPUT )
			{
				numoutputpins++;
			}
			InPin->Release();
			
		}
		EnumPins->Release();
		if (numoutputpins == 0)
		{
			Trace("Found filter with no output");
			if (has_audio_in_pin)
			{
				AudioRenderer = Renderer;
				Trace("DirectShowPlayer found audio renderer");
			}

			if (has_video_in_pin)
			{
				VideoRenderer = Renderer;
				Trace("DirectShowPlayer found video renderer");
			}

		}

		else
			Renderer->Release();
	}	
	EnumFilters->Release();		// Find renderer input

	
	mHasAudio = true;
	if (AudioRenderer==NULL)
	{
		Trace("There was no audio in this media file"); 
		mHasAudio = false;
	}
	else
	{
		AudioRenderer->Release();
	}


	// remove video renderer
	if (VideoRenderer==NULL)
	{
		Trace("There was no video in this media file");
		return 2 ;
	}
	
	//
	// Ok lets get the desired playback frame rate
	//
	IBasicVideo* basic_video = NULL;
	hr =VideoRenderer->QueryInterface(IID_IBasicVideo,(void**)&basic_video);
	if (FAILED(hr))
    {
        Error("DirectShowPlayer could not Get IBasicVideo interface.");
        return 0 ;
	}
	if (basic_video)
	{
		REFTIME fr;
		hr = basic_video->get_AvgTimePerFrame(&fr);
		if (FAILED(hr))
		{
			//	fprintf(fp,"DirectShowPlayer could not retrive frame rate from IBasicVideo interface.\n");
			 Warning("DirectShowPlayer could not retrive frame rate from IBasicVideo interface.");
		}
		else
		{
			mFrameRate = (double)fr;
		}

		basic_video->Release();
	}

	VideoRenderer->EnumPins(&EnumPins);
	EnumPins->Reset();
	EnumPins->Next(1, &InPin, &fetched);	// first one is only one
	EnumPins->Release();	// Find ouput pin on filter it is connected to
	InPin->ConnectedTo(&OutPin);	// Disconnect the filters - note that we have to call Disconnect for both pins
	pGraph->Disconnect(InPin);
	pGraph->Disconnect(OutPin);
	InPin->Release();
	OutPin->Release();

	pGraph->RemoveFilter(VideoRenderer);
	VideoRenderer->Release();

	// Create the Sample Grabber.
	pGrabber = new CPVSGrabber(NULL, NULL, FALSE);
	if (quarter1080pVideos == true)
	{
		pGrabber->SetProcessAndQuarterSampleWhenReceived(TRUE);
	}

	pGrabber->AddRef();

	hr =pGrabber->QueryInterface(IID_IBaseFilter, (void**)&pGrabberF);
	if (FAILED(hr))
    {
        Error("DirectShowPlayer could not create the Filter Graph Manager.");
        return 0 ;
	}

	if (FAILED(hr))
	{
		Error("DirectShowPlayer failed to Create SampleGrabber");
		return 0 ;
	}
	hr = pGraph->AddFilter(pGrabberF, L"Sample Grabber");
	if (FAILED(hr))
	{
		Error("DirectShowPlayer failed to add SampleGrabber");
		return 0 ;
	}
 
	CMediaType mt;
	ZeroMemory(&mt, sizeof(AM_MEDIA_TYPE));
	mt.majortype = MEDIATYPE_Video;
	mt.subtype = MEDIASUBTYPE_RGB32;
	hr = pGrabber->SetAcceptedMediaType(&mt);

	
	hr = CoCreateInstance(CLSID_NullRenderer, NULL, CLSCTX_INPROC_SERVER, IID_IBaseFilter, (void **)&pNullRenderer);
	if (FAILED(hr))
    {
        Error("DirectShowPlayer could not create the pNullRenderer filter.");
        return 0; 
	}
        
	// add null renderer to graph
	hr = pGraph->AddFilter(pNullRenderer, L"Null Renderer");
	if (FAILED(hr))
    {
        Error("DirectShowPlayer could not add pNullRenderer filter.");
        return 0;
	}

	//
	// Default try rgb32 as negotiated media type
	//
	hr = ConnectFilters(pGraph, OutPin, pGrabberF);

	if (hr == VFW_E_CANNOT_CONNECT)
    {
		//
		// Try NV12 instead as grabber can convert
		// (Microsoft DTV-DVD decoder defaults to this type and does not support rgb)
		//
		ZeroMemory(&mt, sizeof(AM_MEDIA_TYPE));
		mt.majortype = MEDIATYPE_Video;
		mt.subtype = MEDIASUBTYPE_NV12;
		hr = pGrabber->SetAcceptedMediaType(&mt);

		hr = ConnectFilters(pGraph, OutPin, pGrabberF);
		if(FAILED(hr))
		{
			Error("DirectShowPlayer could not connect grabber filter");
			return 2;
		}
	}

	hr = ConnectFilters(pGraph, pGrabberF, pNullRenderer);
	if (FAILED(hr))
    {
        Error("DirectShowPlayer could not connect null renderer filter.");
        return 0;
	}

	hr = pCallback->QueryInterface( IID_ISampleGrabberCB, reinterpret_cast<void**>(&pCB) );
	if (FAILED(hr))
	{
		Error("ERROR - Could not get sample grabber callback interface.");
        return 0;
	}

    hr = pGrabber->SetCallback(pCB);
    if( FAILED( hr ) ) 
    {
        Error("Failed in SetOneShot!  hr=0x%x\r\n", hr );
        return 0;
    }

	hr = pGraph->QueryInterface( IID_IMediaSeeking, reinterpret_cast<void**>(&pSeeking) );
	if (FAILED(hr))
	{
		Error("ERROR - Could not get media seeking interface.");
        return 0;
	}

	pSeeking->GetDuration( &mDuration );

	//Obtain reference to MediaFilter from graph
    hr = pGraph->QueryInterface( IID_IMediaFilter, reinterpret_cast<void**>(&pMediaFilter) );

	if (FAILED(hr))
    {
        Error("DirectShowPlayer could not get media filter");
        return 0 ;
	}

	GetImageInfo();

	if (this->pBasicAudio!=NULL)
	{
		mCachedVolume = GetVolume();
	}

	/*
	//
	// Use this to test problem with blank thumbanils and mp4 with MS-DTV decoder
	//

	HRESULT ourResult = pControl->Run();
	//	Trace("Called video player run = %s", mFilename);

	if (ourResult != S_OK)
	{
		Error("failed 1 %s %d", mFilename, ourResult);
		int whoops = 3;
		whoops++;
	}

	OAFilterState state = GetControlState();

	if (state != State_Running)
	{
		Error("failed 3 %s not running", mFilename);
		int whoops = 3;
		whoops++;
	}

	int count = 0;
	BYTE* buffer = NULL;
	while (buffer == NULL && count <50)
	{
		buffer = ((CPVSGrabber*)pGrabber)->GetBufferedSample();
		if (buffer == NULL)
		{
			::Sleep(100);
			count++;
		}
	}

	if (count == 50)
	{
		int failedToGetSample = 2;
		failedToGetSample++;

		pControl->Stop();
		state = GetControlState();
		pControl->Run();
		state = GetControlState();

		count = 0;
	    buffer = NULL;
		while (buffer == NULL && count <50)
		{
			buffer = ((CPVSGrabber*)pGrabber)->GetBufferedSample();
			if (buffer == NULL)
			{
				::Sleep(100);
				count++;
			}
		}

		if (count == 50)
		{
			int f = 33;
			f++;
		}
		else
		{
			int g = 33;
			g++;
		}
	}

	pControl->Stop();

	state = GetControlState();
	if (state != State_Stopped)
	{
		Error("failed 2 %s %d", mFilename, ourResult);

		int whoops = 3;
		whoops++;
	}
	*/


	return 1;
}

// *******************************************************************************************************************
int CDirectShowPlayer::GetWidth()
{
    return mWidth;
}

// *******************************************************************************************************************
int CDirectShowPlayer::GetHeight()
{
    return mHeight;
}

// *******************************************************************************************************************
void  CDirectShowPlayer::GetCurrentVideoBuffer(unsigned char* data, int pitch, unsigned int width, unsigned int height, double* sampleTime, BOOL quartered)
{
	static bool being_called=false;

	if (GetLogLevel() >= LOGLEVEL_TRACE)
	{
		Trace("Getting video buffer for %s", mFilename);
	}
	
	if (being_called==true)
	{
		Trace("STILL IN GetCurrentVideoBuffer returning... %s", mFilename);
		return;
	}

	being_called=true;
    bool forceStop = false;

#ifdef FULL_VIDEO_TRACE
	Trace("called GetCurrentVideoBuffer waiting for all clear to get contents");
#endif
	// if frame stepping wait for next frame to become available
	if (mFrameStepping == true)
	{
		int count_count=0;

		while (next_frame_ready==false && !ReachedEnd() && !mStopped && !forceStop)
		{
			if (GetLogLevel() >= LOGLEVEL_TRACE)
			{
				Trace("Getting sample %d %d %d", count_count, mStopped, forceStop);
			}

			count_count++;

			// ok massive hack, has to change reachedend, and can't 100% trust it at mo and may
			// cause infinite loop if there is a problem.

			// if we don't get a new frame within a second and we're within 2 seconds of the end
			// of the video, then just stop the video, which will cause repeated end frames.

			// SRG also added a pEvent->WaitForCompletion whcih should return a non E_ABORT method
			// if the gragh has finished rendering all its data.

			if (count_count > 10)
			{
				double duration = (((double)this->mDuration)/10000000.0);

				duration -=this->mFrameRate;
				duration -=2.0;


				if (mLastSampleTime >= duration) //((((double)this->mDuration)/10000000.0) - 0.001))
				{
					DeclareEncodeCheckpoint('@');
					Error("Had to do forced stop method 1 %s", mFilename);
                    forceStop=true;
					break;
				}

				
				// ok lets double check it's not finished rendering all data
				if (pEvent!=NULL)
				{				
					if (GetLogLevel() >= LOGLEVEL_TRACE)
					{
						Trace("Getting sample AA");
					}

					long pEvCode;
					HRESULT hr= pEvent->WaitForCompletion(100,&pEvCode);

					// if it didn't have to abort we are not running!
					if (hr !=E_ABORT)
					{
                        forceStop=true;
						Trace("Had to do forced stop method 2 %s",mFilename);
						break;
					}

					if (GetLogLevel() >= LOGLEVEL_TRACE)
					{
						Trace("Getting sample BB");
					}
				}

			}

			if (GetLogLevel() >= LOGLEVEL_TRACE)
			{
				Trace("Getting sample CC");
			}

			DWORD waitLength = 1000L;	// frame not recieved default is to wait a second...
			
			//
			// If waiting for first frame wait for a minute for first frame
			//
			if (framesCountedOnFrameStepping == 0)
			{
				waitLength = 60000L;
			}

			DWORD result = WaitForSingleObject( mNextFrameReadySignal, waitLength) ;

			if (GetLogLevel() >= LOGLEVEL_TRACE)
			{
				Trace("Getting sample DD");
			}

			//
			// If 1 minute and still not received first frame, give up!
			//
			if (result == WAIT_TIMEOUT && framesCountedOnFrameStepping == 0)
			{			
				Error("Had to do force stop method 5 %s", mFilename);
				forceStop = true;
			}

			//
			// No frame arrived? pehaps the video finished playing event reveived, if so end now.
			// Don't do this on before the video has started playing as it may take events off the queue that were meant for other parts
			// of the grapgh???? SRG
			//
            if (result == WAIT_TIMEOUT && framesCountedOnFrameStepping !=0 )
            {
				if (GetLogLevel() >= LOGLEVEL_TRACE)
				{
					Trace("Getting sample EE");
				}

                //
                // See if we have completed, sometimes we will receive a complete event which speeds things up as we dont have to fully wait for a time out.
                //
                long eventCode = 0;
                LONG_PTR ptrParam1 = 0;
                LONG_PTR ptrParam2 = 0;
                int eventCount=0;
                long timeoutMs = 100;

                //
                // Look up to next 10 events queued up for complete signal
                //
				HRESULT geteventresult = pEvent->GetEvent(&eventCode, &ptrParam1, &ptrParam1, timeoutMs);

                while (eventCount <10 )
                {
					Trace("Getting sample FF");
					eventCount++;

					//
					// Has times out waiting for event
					//
					if (FAILED(geteventresult))
					{						
						//
						// If 10th time waiting to recieve sample and 10th time
						// no complete event received then give up!
						//
						if (eventCount >=10 && count_count >= 10)
						{
							Error("Had to do force stop method 4 %s", mFilename);
							forceStop = true;
							break;
						}
					}
					else
					{
						//
						// Free the event
						//
						pEvent->FreeEventParams(eventCode, ptrParam1, ptrParam1);
						if (eventCode == EC_COMPLETE)
						{
							Error("Had to do force stop method 3 %s", mFilename);
							forceStop = true;
							break;
						}
					}

					if (eventCount < 10)
					{
						geteventresult = pEvent->GetEvent(&eventCode, &ptrParam1, &ptrParam1, timeoutMs);
					}
                }
                //if (eventCount >=10)
                //{
                //    Error("Received more than 10 events in GetCurrentVideoBuffer %s", mFilename);
                //}
            }
		}
	}

#ifdef FULL_VIDEO_TRACE
	Trace("Got all clear. Gettting buffer contents");
#endif

	hack_counter=0;

	if (GetLogLevel() >= LOGLEVEL_TRACE)
	{
		Trace("Got buffer sample");
	}

	if (pGrabber==NULL)
	{
		Error("Grabber was null in GetCurrentVideoBuffer");
		being_called=false;
		return ;
	}

	
	// ### SGR TODO, if same surface then we could just return here  (like 1% speed up type thing)
	// repeat frame when encoding
	/*
	if (mDoingRepeatFrame==true)
	{
		being_called = false;
		return;
	}
	*/

	if (GetLogLevel() >= LOGLEVEL_TRACE)
	{
		Trace("CopyA");
	}

    CopyCurrentBufferContents(data,pitch,width,height,sampleTime, quartered);
	framesCountedOnFrameStepping++;

	if (GetLogLevel() >= LOGLEVEL_TRACE)
	{
		Trace("CopyB");
	}

    if (forceStop)
    {
        Stop(true,true,true);
    }
   
	being_called = false;
}

// *********************************************************************************************************************
void CDirectShowPlayer::CopyCurrentBufferContents(unsigned char* data, int pitch, unsigned int width, unsigned int height, double* sampleTime, BOOL quartered)
{
	int size = width * height * 4;

	if (pitch == width * 4)
	{
		HRESULT result = pGrabber->GetCurrentBuffer((BYTE*)data, size, sampleTime, quartered);
		if (result != S_OK)
		{
			Error("Failed to get buffer for %s", mFilename);
		}

	}
	else if (pitch > ((int)width)*4)
	{
		HRESULT result = pGrabber->GetCurrentBuffer((BYTE*)data, size, sampleTime, quartered);

		if (result != S_OK)
		{
			Error("Failed(2) to get buffer for %s", mFilename);
		}

		// Raw data surface may be a power2 texture, therefore is not quite the same size as video dimenision.
		// But in this case it will always be bigger, so some moving of bytes can sort this out.

		int videoBufferPitch = width*4;
		unsigned char* destPtr = data+((height-1)*pitch);;
		unsigned char* srcPtr = data+((height-1)*videoBufferPitch);
			
		for (unsigned int h=0;h<height;h++)
		{
			memcpy(destPtr,srcPtr, videoBufferPitch);
			destPtr-=pitch;
			srcPtr-=videoBufferPitch;
		}
	}
	else
	{
		ErrorOnce("CDirectShowPlayer::GetCurrentVideoBuffer width greater than pitch"));
	}	
}


// *********************************************************************************************************************
STDMETHODIMP CDirectShowPlayerCallback::SampleCB( double SampleTime, IMediaSample * pSample )
{ 
	for_player->mInternalVideoFrameNum++;
	for_player->hack_just_seeked_waiting_to_render = false;

	if (for_player->mFrameStepping==false) 
	{
		for_player->mLastSampleTime = SampleTime;
		return 0;
	}

	if (for_player->mStopped==false && !for_player->ReachedEnd())
	{
		for_player->next_frame_ready = true ;

		// Release semaphore to tell other thread that frame is ready
		ReleaseSemaphore(for_player->mNextFrameReadySignal, 1, NULL);

	#ifdef FULL_VIDEO_TRACE
		int count=0;
	#endif

        //
		// Wait until other thread has received frame and called advanceframe
        //
		while (for_player->next_frame_ready==true && for_player->mStopped==false && !for_player->ReachedEnd() )
		{
	#ifdef FULL_VIDEO_TRACE
			count++;
			if (count > 100)
			{
				Error("Video player stepper may be stuck (%d), next_frame_ready=%d, stopped = %d, Reached_end=%d",
					count, for_player->next_frame_ready, for_player->mStopped, for_player->ReachedEnd());
			}
	#endif
			DWORD result = WaitForSingleObject( for_player->mWaitForAdvanceFrameSingnal, 1000L); 

            if (result == WAIT_TIMEOUT ) //0x00000102L
            {
                int f =3;
                f++;
            }

		}
		hack_counter=0;
	}
    else
    {
        int gothere=0;
        gothere++;
    }


	//Trace("Control given back to DS");

	for_player->mLastSampleTime = SampleTime;

    return 0;
}



