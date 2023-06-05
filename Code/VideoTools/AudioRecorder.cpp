
#include "AudioRecorder.h"
#include "CustomFilters/WavDest.h"
#include "UnmanagedErrors.h"

// Code mostly taken from the AudioCap example from directshow examples

#define DEFAULT_BUFFER_TIME ((float) 0.05)  /* 50 milliseconds*/
#ifndef SAFE_RELEASE
#define SAFE_RELEASE(x)  if(x) {x->Release(); x=0;}
#endif

#define DEFAULT_FILENAME    TEXT("c:\\test.wav\0")

//***************************************************************************
static std::string ConvertWCSToMBS(const wchar_t* pstr, long wslen)
{
    int len = ::WideCharToMultiByte(CP_ACP, 0, pstr, wslen, NULL, 0, NULL, NULL);

    std::string dblstr(len, '\0');
    len = ::WideCharToMultiByte(CP_ACP, 0 /* no flags */,
                                pstr, wslen /* not necessary NULL-terminated */,
                                &dblstr[0], len,
                                NULL, NULL /* no default char */);

    return dblstr;
}


//***************************************************************************
static std::string ConvertBSTRToMBS(BSTR bstr)
{
    int wslen = ::SysStringLen(bstr);
    return ConvertWCSToMBS((wchar_t*)bstr, wslen);
}


//***************************************************************************
static HRESULT GetPin( IBaseFilter * pFilter, PIN_DIRECTION dirrequired, int iNum, IPin **ppPin)
{
    CComPtr< IEnumPins > pEnum;
    *ppPin = NULL;

    if (!pFilter)
       return E_POINTER;

    HRESULT hr = pFilter->EnumPins(&pEnum);
    if(FAILED(hr)) 
        return hr;

    ULONG ulFound;
    IPin *pPin;
    hr = E_FAIL;

    while(S_OK == pEnum->Next(1, &pPin, &ulFound))
    {
        PIN_DIRECTION pindir = (PIN_DIRECTION)3;

        pPin->QueryDirection(&pindir);
        if(pindir == dirrequired)
        {
            if(iNum == 0)
            {
                *ppPin = pPin;  // Return the pin's interface
                hr = S_OK;      // Found requested pin, so clear error
                break;
            }
            iNum--;
        } 

        pPin->Release();
    } 

    return hr;
}



//
// General purpose function to delete a heap allocated AM_MEDIA_TYPE structure.
// This is useful when calling IEnumMediaTypes::Next, as the interface
// implementation allocates the structures which you must later delete.
// The format block may also be a pointer to an interface to release.
//
// This code was exerpted from the BaseClasses' mtype.cpp by combining
// DeleteMediaType() and FreeMediaType().  Since some applications link with
// the base classes library (instead of simply linking with strmiids.lib)
// strictly because they need the DeleteMediaType() method, this utility
// method can be used instead to prevent unnecessary overhead.

//***************************************************************************
static void UtilDeleteMediaType(AM_MEDIA_TYPE *pmt)
{
    // Allow NULL pointers for coding simplicity
    if (pmt == NULL) {
        return;
    }

    // Free media type's format data
    if (pmt->cbFormat != 0) 
    {
        CoTaskMemFree((PVOID)pmt->pbFormat);

        // Strictly unnecessary but tidier
        pmt->cbFormat = 0;
        pmt->pbFormat = NULL;
    }

    // Release interface
    if (pmt->pUnk != NULL) 
    {
        pmt->pUnk->Release();
        pmt->pUnk = NULL;
    }

    // Free media type
    CoTaskMemFree((PVOID)pmt);
}

//***************************************************************************
static void AddFilterToListWithMoniker(std::string& szFilterName, 
                                       IMoniker *pMoniker, std::vector<AudioRecordDevice>& ListFilters)
{
  
	AudioRecordDevice device;
	device.mMoniker = pMoniker;
    device.mName = szFilterName;

	ListFilters.push_back( device);
    // Add the category name and a pointer to its CLSID to the list box
  //  int nSuccess  = ListFilters.AddString(szFilterName);
  //  int nIndexNew = ListFilters.FindStringExact(-1, szFilterName);

   // nSuccess = ListFilters.SetItemDataPtr(nIndexNew, pMoniker);
}

//***************************************************************************
static HRESULT EnumFiltersAndMonikersToList(IEnumMoniker *pEnumCat, std::vector<AudioRecordDevice>& ListFilters)
{
    HRESULT hr=S_OK;
    IMoniker *pMoniker=0;
    ULONG cFetched=0;
    VARIANT varName={0};
    int nFilters=0;

    // If there are no filters of a requested type, show default string
    if (!pEnumCat)
    {
        return S_FALSE;
    }

    // Enumerate all items associated with the moniker
    while(pEnumCat->Next(1, &pMoniker, &cFetched) == S_OK)
    {
        IPropertyBag *pPropBag;
        ASSERT(pMoniker);

        // Associate moniker with a file
        hr = pMoniker->BindToStorage(0, 0, IID_IPropertyBag, 
                                    (void **)&pPropBag);
        ASSERT(SUCCEEDED(hr));
        ASSERT(pPropBag);
        if (FAILED(hr))
            continue;

        // Read filter name from property bag
        varName.vt = VT_BSTR;
        hr = pPropBag->Read(L"FriendlyName", &varName, 0);
        if (FAILED(hr))
            continue;

        // Get filter name (converting BSTR name to a CString)
		std::string str = ConvertBSTRToMBS(varName.bstrVal);
        SysFreeString(varName.bstrVal);
        nFilters++;

        // Add the filter name and moniker to the listbox
        AddFilterToListWithMoniker(str, pMoniker, ListFilters);
       
        // Cleanup interfaces
        SAFE_RELEASE(pPropBag);

        // Intentionally DO NOT release the pMoniker, since it is
        // stored in a listbox for later use
    }

    return hr;
}

//***************************************************************************
static HRESULT EnumFiltersWithMonikerToList(ICreateDevEnum *pSysDevEnum, 
											const GUID *clsid, std::vector<AudioRecordDevice>& List)
{
    HRESULT hr;
    IEnumMoniker *pEnumCat = NULL;

    // Instantiate the system device enumerator if it doesn't exist
    if (pSysDevEnum == NULL)
    {
        hr = CoCreateInstance(CLSID_SystemDeviceEnum, NULL, 
                              CLSCTX_INPROC, IID_ICreateDevEnum, 
                              (void **)&pSysDevEnum);
        if FAILED(hr)
            return hr;
    }

    // Enumerate all filters of the selected category  
    hr = pSysDevEnum->CreateClassEnumerator(*clsid, &pEnumCat, 0);
    if (SUCCEEDED(hr))
    {
        // Enumerate all filters using the category enumerator
        hr = EnumFiltersAndMonikersToList(pEnumCat, List);

        SAFE_RELEASE(pEnumCat);
    }

    pSysDevEnum->Release();
    return hr;
}


//***************************************************************************
AudioRecorder::AudioRecorder(void)
{
	m_pInputDevice = NULL;
	m_pGB= NULL;
	m_pCapture=NULL;
	m_pWAVDest=NULL;
    m_pFileWriter = NULL;
    m_pSplitter=NULL;
	m_pMC = NULL;
	m_pRenderer= NULL;
	mDeviceIterator=0;

	GetRecordDevices();
}

//***************************************************************************
AudioRecorder::~AudioRecorder(void)
{
	FreeInterfaces();
}

//***************************************************************************
void AudioRecorder::ResetCapture()
{
	FreeInterfaces();
}

//***************************************************************************
void AudioRecorder::FreeInterfaces()
{
	SAFE_RELEASE(m_pInputDevice);
	SAFE_RELEASE(m_pWAVDest);
	SAFE_RELEASE(m_pFileWriter);
	SAFE_RELEASE(m_pSplitter);
	SAFE_RELEASE(m_pMC);
    SAFE_RELEASE(m_pGB);
    SAFE_RELEASE(m_pCapture);
	SAFE_RELEASE(m_pRenderer);
}


//***************************************************************************
HRESULT AudioRecorder::SetupInterfaces()
{
    HRESULT hr;

    // Create the filter graph.
    hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC,
                          IID_IGraphBuilder, (void **)&m_pGB);
    if (FAILED(hr) || !m_pGB)
        return E_NOINTERFACE;

    // Create the capture graph builder.
    hr = CoCreateInstance(CLSID_CaptureGraphBuilder2, NULL, CLSCTX_INPROC, 
                          IID_ICaptureGraphBuilder2, (void **)&m_pCapture);
    if (FAILED(hr) || !m_pCapture)
        return E_NOINTERFACE;

    // Associate the filter graph with the capture graph builder
    hr = m_pCapture->SetFiltergraph(m_pGB);    
    if (FAILED(hr))
        return hr;

	hr = m_pGB->QueryInterface(IID_IMediaControl, (void **)&m_pMC);
	if (FAILED(hr))
	    return hr;

	return hr;
}


//***************************************************************************
HRESULT AudioRecorder::GetRecordDevices() 
{
    HRESULT hr;
    
    // Enumerate and display the audio input devices installed in the system
    hr = EnumFiltersWithMonikerToList(NULL, &CLSID_AudioInputDeviceCategory, mRecordDevicesList);
    if (FAILED(hr))
        return hr;

	/*
    // If there are no audio input devices, then the enum will return S_FALSE
    if (hr == S_FALSE)
    {
        MessageBox(TEXT("There were no audio capture devices detected on this system.\r\n\r\n")
                   TEXT("If your system has an audio capture device (sound card, video camera, etc.),\r\n")
                   TEXT("then make sure that it is working properly and that you are not running\r\n")
                   TEXT("this sample through a Windows XP Remote Desktop connection.\r\n\r\n")
                   TEXT("This sample will now exit.\0"),
                   TEXT("No audio capture hardware!"), MB_ICONERROR);
        return E_FAIL;
   }

    // Select the first audio device in the list for the default
    m_ListInputs.SetCurSel(0);
    OnSelchangeListInputDevices();
	*/

    return hr;
}

//***************************************************************************
void AudioRecorder::Record(char* outputWavFile, int selectedDevice, int echoRecording) 
{
    HRESULT hr;
    IMoniker *pMoniker=0;

    // Release and deleteany previous capture filter graph
    if (m_pInputDevice)
	{
		ResetCapture();
	}
	SetupInterfaces();


    // Get the currently selected category name
   // int nItem = m_ListInputs.GetCurSel();

    // Read the stored moniker pointer from the list box's item data
	if (selectedDevice >= (int) mRecordDevicesList.size()) return;

	pMoniker = mRecordDevicesList[selectedDevice].mMoniker;

    if (!pMoniker)
       return;

    // Use the moniker to create the specified audio capture device
    hr = pMoniker->BindToObject(0, 0, IID_IBaseFilter, (void**)&m_pInputDevice);   
    if (FAILED(hr))
        return;

    // Add the capture filter to the filter graph
    hr = m_pGB->AddFilter(m_pInputDevice, L"Audio Capture");
    if (FAILED(hr))
        return;

    // List and display the available input pins on the capture filter.
    // Many devices offer multiple pins for input (microphone, CD, LineIn, etc.)
    //  hr = EnumPinsOnFilter(m_pInputDevice, PINDIR_INPUT, m_ListInputPins);
    // if (FAILED(hr))
    //    return;

	if (echoRecording !=0)
	{
		RenderPreviewStream();
	}

	RenderCaptureStream(outputWavFile);

	if (m_pMC)
    {
        hr = m_pMC->Run();
       // Say(TEXT("Auditioning/recording audio..."));
    }
}

//***************************************************************************
void AudioRecorder::Stop()
{
	 HRESULT hr;

  // Stop recording
	if (m_pMC)
	{
		hr = m_pMC->StopWhenReady();
	}

    DestroyPreviewStream();
    DestroyCaptureStream();  
}


//***************************************************************************
void AudioRecorder::Pause()
{
	if (m_pMC)
	{
		m_pMC->Pause();
	}
}

//***************************************************************************
void AudioRecorder::Continue()
{
	if (m_pMC)
	{
		m_pMC->Run();
	}
}

//***************************************************************************
HRESULT AudioRecorder::SetAudioProperties()
{
    HRESULT hr=0;
    IPin *pPin=0;
    IAMBufferNegotiation *pNeg=0;
    IAMStreamConfig *pCfg=0;
    int nFrequency=0;

    // Determine audio properties
    int nChannels = 2 ; // m_btnMono.GetCheck() ? 1 : 2;
    int nBytesPerSample = 2 ; // m_btn8BIT.GetCheck() ? 1 : 2;

    // Determine requested frequency
   // if (IsDlgButtonChecked(IDC_RADIO_11KHZ))      
    //    nFrequency = 11025;
    //else if (IsDlgButtonChecked(IDC_RADIO_22KHZ)) 
     //   nFrequency = 22050;
    //else 
        nFrequency = 44100;

    // Find number of bytes in one second
    long lBytesPerSecond = (long) (nBytesPerSample * nFrequency * nChannels);

    // Set to 50ms worth of data    
    long lBufferSize = (long) ((float) lBytesPerSecond * DEFAULT_BUFFER_TIME);

    // Get the buffer negotiation interface for each pin,
    // since there could be both a Capture and a Preview pin.
    for (int i=0; i<2; i++)
    {
        hr = GetPin(m_pInputDevice, PINDIR_OUTPUT, i, &pPin);
        if (SUCCEEDED(hr))
        {
            // Get buffer negotiation interface
            hr = pPin->QueryInterface(IID_IAMBufferNegotiation, (void **)&pNeg);
            if (FAILED(hr))
            {
                pPin->Release();
                break;
            }

            // Set the buffer size based on selected settings
            ALLOCATOR_PROPERTIES prop={0};
            prop.cbBuffer = lBufferSize;
            prop.cBuffers = 6;
            prop.cbAlign = nBytesPerSample * nChannels;
            hr = pNeg->SuggestAllocatorProperties(&prop);
            pNeg->Release();

            // Now set the actual format of the audio data
            hr = pPin->QueryInterface(IID_IAMStreamConfig, (void **)&pCfg);
            if (FAILED(hr))
            {
                pPin->Release();
                break;
            }            

            // Read current media type/format
            AM_MEDIA_TYPE *pmt={0};
            hr = pCfg->GetFormat(&pmt);

            if (SUCCEEDED(hr))
            {
                // Fill in values for the new format
                WAVEFORMATEX *pWF = (WAVEFORMATEX *) pmt->pbFormat;
                pWF->nChannels = (WORD) nChannels;
                pWF->nSamplesPerSec = nFrequency;
                pWF->nAvgBytesPerSec = lBytesPerSecond;
                pWF->wBitsPerSample = (WORD) (nBytesPerSample * 8);
                pWF->nBlockAlign = (WORD) (nBytesPerSample * nChannels);

                // Set the new formattype for the output pin
                hr = pCfg->SetFormat(pmt);
                UtilDeleteMediaType(pmt);
            }

            // Release interfaces
            pCfg->Release();
            pPin->Release();
        }
        // No more output pins on this filter
        else
            break;
    }

    return hr;
}



//***************************************************************************
HRESULT AudioRecorder::RenderCaptureStream(char* outputWavFilename)
{
    USES_CONVERSION;
    HRESULT hr=0;
    IFileSinkFilter2 *pFileSink;

	HRESULT wavDestCreeationResult = S_OK;
	CWavDestFilter* wavDestFilter = new CWavDestFilter(NULL, &wavDestCreeationResult);
	if (FAILED(wavDestCreeationResult))
	{
		Error("AudioRecorder could not create the PVS WAV Dest Filter.");
		return E_NOINTERFACE;
	}

	//
	// COM now takes ownership of the wavdest filter and will delete it when we release the interface
	//
	hr = wavDestFilter->QueryInterface(IID_IBaseFilter, (void**)&m_pWAVDest);
	if (FAILED(hr))
	{
		Error("AudioRecorder could not query interface PVS WAV Dest Filter.");
		return E_NOINTERFACE;
	}

    // Create the FileWriter filter
    hr = CoCreateInstance(CLSID_FileWriter, NULL, CLSCTX_INPROC,
                          IID_IFileSinkFilter2, (void **)&pFileSink);
    if (FAILED(hr))
        return E_NOINTERFACE; 

    // Get the file sink interface from the File Writer
    hr = pFileSink->QueryInterface(IID_IBaseFilter, (void **)&m_pFileWriter);
    if (FAILED(hr))
        return E_NOINTERFACE; 

    // Add the WAVDest filter to the graph
    hr = m_pGB->AddFilter(m_pWAVDest, L"WAV Dest");
    if (FAILED(hr))
        return hr;

    // Add the FileWriter filter to the graph
    hr = m_pGB->AddFilter((IBaseFilter *)m_pFileWriter, L"File Writer");
    if (FAILED(hr))
        return hr;

    // Set filter to always overwrite the file
    hr = pFileSink->SetMode(AM_FILE_OVERWRITE);

    // Set the output filename, which must be a wide string

    WCHAR wszFilename[1024];
	swprintf(wszFilename, 1024, L"%hs", outputWavFilename);
    wszFilename[1023] = 0;    // Ensure NULL termination

    hr = pFileSink->SetFileName(wszFilename, NULL);
    if (FAILED(hr))
        return hr; 

    // Get the pin interface for the capture pin
    IPin *pPin=0;
    if (m_pSplitter)
        hr = GetPin(m_pSplitter, PINDIR_OUTPUT, 0, &pPin);
    else
        hr = GetPin(m_pInputDevice, PINDIR_OUTPUT, 0, &pPin);

    // Connect the new filters
    if (pPin)
    {
        hr = m_pGB->Render(pPin);
        pPin->Release();
    }

    // Release the FileSinkFilter2 interface, since it's no longer needed
    SAFE_RELEASE(pFileSink);
    return hr;
}

//***************************************************************************
HRESULT AudioRecorder::DestroyCaptureStream()
{
    HRESULT hr=0;

    // Destroy Audio renderer filter, if it exists
    if (m_pWAVDest)
    {
        hr = m_pGB->RemoveFilter(m_pWAVDest);
        SAFE_RELEASE(m_pWAVDest);
    }

    // Destroy Smart Tee filter, if it exists
    if (m_pFileWriter)
    {
        hr = m_pGB->RemoveFilter(m_pFileWriter);
        SAFE_RELEASE(m_pFileWriter);
    }

    return hr;
}

//***************************************************************************
HRESULT AudioRecorder::RenderPreviewStream()
{
    USES_CONVERSION;
    HRESULT hr=0;
    WCHAR wszFilter[128];

    // If we've already configured the stream, just exit
    if (m_pRenderer)
        return S_OK;

    // Set the requested audio properties - buffer sizes, channels, freq, etc.
    hr = SetAudioProperties();

    // Render the preview stream
    hr = m_pCapture->RenderStream(
            &PIN_CATEGORY_PREVIEW, 
            &MEDIATYPE_Audio, 
            m_pInputDevice, 
            NULL,   // No compression filter.
            NULL    // Default renderer.
        );

    // Some capture sources will have only a Capture pin (no Preview pin).
    // In that case, the Capture Graph builder will automatically insert
    // a SmartTee filter, which will split the stream into a Capture stream
    // and a Preview stream.  In that case, it's not an error.
    if (hr == VFW_S_NOPREVIEWPIN)
    {
        wcscpy_s(wszFilter, T2W(TEXT("Smart Tee\0")));

        // Get the interface for the Splitter filter for later use
        hr = m_pGB->FindFilterByName(wszFilter, &m_pSplitter);
        if (FAILED(hr))
            return hr;
    }
    else if (FAILED(hr))
        return hr;

    // Get the interface for the audio renderer filter for later use
    wcscpy_s(wszFilter, T2W(TEXT("Audio Renderer\0")));
    hr = m_pGB->FindFilterByName(wszFilter, &m_pRenderer);   

    return hr;
}

//***************************************************************************
HRESULT AudioRecorder::DestroyPreviewStream()
{
    HRESULT hr=0;

    // Destroy Smart Tee filter, if it exists
    if (m_pSplitter)
    {
        hr = m_pGB->RemoveFilter(m_pSplitter);
        SAFE_RELEASE(m_pSplitter);
    }

    // Destroy Audio renderer filter, if it exists
    if (m_pRenderer)
    {
        hr = m_pGB->RemoveFilter(m_pRenderer);
        SAFE_RELEASE(m_pRenderer);
    }

    return hr;
}

//***************************************************************************
std::string AudioRecorder::GetFirstDeviceName()
{
	mDeviceIterator = 0;

	return GetNextDeviceName();
}

//***************************************************************************
std::string AudioRecorder::GetNextDeviceName()
{
	if ((int)mRecordDevicesList.size() > mDeviceIterator)
	{
		return mRecordDevicesList[mDeviceIterator++].mName;
	}
	return std::string("");
}


