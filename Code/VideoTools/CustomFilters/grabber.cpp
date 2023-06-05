
#include <streams.h>     // Active Movie (includes windows.h)
#include <initguid.h>    // declares DEFINE_GUID to declare an EXTERN_C const.

#include "qedit.h"
#include "grabber.h"
#include "ColorSpaceConverter.h"
#include "../UnmanagedErrors.h"

#pragma warning(disable: 4800)


// doesn't matter what these are private filter i believe
DEFINE_GUID(CLSID_PVSGrabber, 
0x2fa4f053, 0x6d60, 0x4cb0, 0x95, 0x3, 0x8e, 0x89, 0x23, 0x99, 0x99, 0x99);

DEFINE_GUID(IID_IPVSGrabber, 
0x6b652fff, 0x11fe, 0x4fce, 0x92, 0xad, 0x02, 0x66, 0xb5, 0x99, 0x99, 0x99);

//----------------------------------------------------------------------------
//
//----------------------------------------------------------------------------

CPVSGrabber::CPVSGrabber( IUnknown * pOuter, HRESULT * phr, BOOL ModifiesData )
                : CTransInPlaceFilter( TEXT("SampleGrabber"), (IUnknown*) pOuter, 
                                       CLSID_PVSGrabber, phr, (BOOL)ModifiesData )
                , m_callback( NULL )
				, mBufferedSample( NULL )
				, mBufferedSampleSize ( 0 )
                , mBufferedSampleTime ( -1 )
				, m_converter( NULL )
				, mWidth( 0 )
				, mHeight( 0 )
				, mFrameRate( 0 )
				, mBufferedSampleType(UNKNOWN)
				, mProcessAndQuarterSampleWhenRecevied(FALSE)

{
    // this is used to override the input pin with our own   
    m_pInput = (CTransInPlaceInputPin*) new CPVSGrabberInPin( this, phr );
    if( !m_pInput )
    {
        if (phr)
            *phr = E_OUTOFMEMORY;
    }
    
    // Ensure that the output pin gets created.  This is necessary because our
    // SetDeliveryBuffer() method assumes that the input/output pins are created, but
    // the output pin isn't created until GetPin() is called.  The 
    // CTransInPlaceFilter::GetPin() method will create the output pin, since we
    // have not already created one.
    IPin *pOutput = GetPin(1);
    // The pointer is not AddRef'ed by GetPin(), so don't release it

	//Error("Creating grabber %p", this);
}


CPVSGrabber::~CPVSGrabber()
{
	if (m_converter != NULL)
	{
		delete m_converter;
		m_converter = NULL;
	}

	ClearBuffers();
}

STDMETHODIMP CPVSGrabber::NonDelegatingQueryInterface( REFIID riid, void ** ppv) 
{
    CheckPointer(ppv,E_POINTER);

    if(riid == IID_IPVSGrabber) {                
        return GetInterface((IPVSGrabber *) this, ppv);
    }
    else {
        return CTransInPlaceFilter::NonDelegatingQueryInterface(riid, ppv);
    }
}


//----------------------------------------------------------------------------
// This is where you force the sample grabber to connect with one type
// or the other. What you do here is crucial to what type of data your
// app will be dealing with in the sample grabber's callback. For instance,
// if you don't enforce right-side-up video in this call, you may not get
// right-side-up video in your callback. It all depends on what you do here.
//----------------------------------------------------------------------------

HRESULT CPVSGrabber::CheckInputType( const CMediaType * pmt )
{
    CheckPointer(pmt,E_POINTER);
    CAutoLock lock( &m_Lock );

	double reportFrameRate = 0;

    if (pmt->formattype == FORMAT_VideoInfo) 
	{
	    VIDEOINFOHEADER *pVih;
		pVih = reinterpret_cast<VIDEOINFOHEADER*>(pmt->pbFormat);
		
		//Get the information about the bitmap
		mWidth = pVih->bmiHeader.biWidth;
		mHeight = pVih->bmiHeader.biHeight;

		reportFrameRate = (double)pVih->AvgTimePerFrame;
		reportFrameRate /= 10000000.0f;
	}
	else if (pmt->formattype == FORMAT_VideoInfo2)
	{
		VIDEOINFOHEADER2 *pVih;
		pVih = reinterpret_cast<VIDEOINFOHEADER2*>(pmt->pbFormat);

		mWidth = pVih->bmiHeader.biWidth;
		mHeight = pVih->bmiHeader.biHeight;
		reportFrameRate = (double)pVih->AvgTimePerFrame;
		reportFrameRate /= 10000000.0f;
    }

	//
	// Check if the frame rate has changed (this may happen when we actaully start receiving samples. i.e. the initially reported
	// rate was actually wrong!!
	//
	if (mFrameRate!=0 && reportFrameRate !=0 && mFrameRate != reportFrameRate )
	{
		// to fix in G831
        int f=3;
        f++;
	}

 	mFrameRate = reportFrameRate;

    // if the major type is not set, then accept anything

    GUID g = *m_mtAccept.Type( );
    if( g == GUID_NULL )
    {
        return NOERROR;
    }

    // if the major type is set, don't accept anything else

    if( g != *pmt->Type( ) )
    {
        return VFW_E_INVALID_MEDIA_TYPE;
    }

    // subtypes must match, if set. if not set, accept anything

    g = *m_mtAccept.Subtype( );
    if( g == GUID_NULL )
    {
        return NOERROR;
    }
    if( g != *pmt->Subtype( ) )
    {
        return VFW_E_INVALID_MEDIA_TYPE;
    }

    // format types must match, if one is set

    g = *m_mtAccept.FormatType( );
    if( g == GUID_NULL )
    {
        return NOERROR;
    }
    if( g != *pmt->FormatType( ) )
    {
        return VFW_E_INVALID_MEDIA_TYPE;
    }

    // at this point, for this sample code, this is good enough,
    // but you may want to make it more strict

    return NOERROR;
}


//----------------------------------------------------------------------------
// This bit is almost straight out of the base classes.
// We override this so we can handle Transform( )'s error
// result differently.
//----------------------------------------------------------------------------

HRESULT CPVSGrabber::Receive( IMediaSample * pms )
{
    CheckPointer(pms,E_POINTER);

    HRESULT hr;
    AM_SAMPLE2_PROPERTIES * const pProps = m_pInput->SampleProps();

	//Error("Grabber Receive sample %p", this);
    if (pProps->dwStreamId != AM_STREAM_MEDIA) 
    {
        if( m_pOutput->IsConnected() )
            return m_pOutput->Deliver(pms);
        else
            return NOERROR;
    }

    if (UsingDifferentAllocators()) 
    {
        // We have to copy the data.

        pms = Copy(pms);

        if (pms == NULL) 
        {
            return E_UNEXPECTED;
        }
    }

	//Error("Grabber transform sample %p", this);
    // have the derived class transform the data
    hr = Transform(pms);

    if (FAILED(hr)) 
    {
        Error("Error from transform when grabbing sample");
        if (UsingDifferentAllocators()) 
        {
            pms->Release();
        }
        return hr;
    }

    if (hr == NOERROR) 
    {
        hr = m_pOutput->Deliver(pms);
    }
    
    // release the output buffer. If the connected pin still needs it,
    // it will have addrefed it itself.
    if (UsingDifferentAllocators()) 
    {
        pms->Release();
    }


    return hr;
}


//----------------------------------------------------------------------------
// Transform
//----------------------------------------------------------------------------

HRESULT CPVSGrabber::Transform ( IMediaSample * pms )
{
	if (pms==NULL)
	{
		return NOERROR;
	}

    if( m_callback )
    {
		double doubleStartTime=0;
		{
			//
			// Lock and release before doing callback
			//
			CheckPointer(pms,E_POINTER);
			CAutoLock lock( &m_Lock );

			REFERENCE_TIME StartTime, StopTime;
			pms->GetTime( &StartTime, &StopTime);

			StartTime += m_pInput->CurrentStartTime( );
			StopTime  += m_pInput->CurrentStartTime( );
	
			doubleStartTime =(((double)StartTime)/10000000.0);

			if (mWidth !=0 && mHeight !=0)
			{
				BYTE* pointer;
				HRESULT result = pms->GetPointer(&pointer);
			
				if (result == S_OK)
				{
					if (m_converter == NULL)
					{
						m_converter = new CColorSpaceConverter(m_mtAccept.subtype, mWidth, mHeight);
					}

					//
					// Process sample now?
					//
					if (mProcessAndQuarterSampleWhenRecevied == TRUE && (mHeight >=1080 || mWidth >= 1920))
					{
						int size = ((mWidth+2) >> 1) * ((mHeight+2) >> 1) * 4;				// Make buffer just a little big bigger than needed, just in case

						if (mBufferedSampleSize != size && mBufferedSample != NULL)
						{
							delete[] mBufferedSample;
							mBufferedSample = NULL;
						}

						long sampleSize = pms->GetSize();

						if (mBufferedSample == NULL)
						{
							mBufferedSample = new byte[size];
							mBufferedSampleSize = size;
						}

						mBufferedSampleTime = doubleStartTime;			
						mBufferedSampleType = YUV_TO_RGB_PROCESSED_AND_QUARTERED;
						m_converter->convert_to_rgb32(mBufferedSample, pointer, size, sampleSize, true);
						
					}
					//
					// clone and process later
					//
					else
					{
						if (mBufferedSampleSize != pms->GetSize() && mBufferedSample != NULL)
						{
							delete[] mBufferedSample;
							mBufferedSample = NULL;
						}

						mBufferedSampleSize = pms->GetSize();

						if (mBufferedSample == NULL)
						{
							mBufferedSample = new BYTE[mBufferedSampleSize];
						}

						memcpy(mBufferedSample, pointer, mBufferedSampleSize);
						mBufferedSampleTime = doubleStartTime;
						mBufferedSampleType = CLONE_OF_MEDIA_SAMPLE;
					}
				}
			}
		}

	//	Error("Grabeber calling callback %p", this);

		// release lock before calling this
        HRESULT hr = m_callback->SampleCB(doubleStartTime, pms); 

        return hr;
    }

    return NOERROR;
}


//----------------------------------------------------------------------------
//	ClearBuffers
//----------------------------------------------------------------------------
STDMETHODIMP CPVSGrabber::ClearBuffers()
{
	CAutoLock lock( &m_Lock );

	if (mBufferedSample!=NULL)
	{
		delete [] mBufferedSample ;
		mBufferedSample=NULL;
	}

	return NOERROR;
}

//----------------------------------------------------------------------------
// SetAcceptedMediaType
//----------------------------------------------------------------------------

STDMETHODIMP CPVSGrabber::SetAcceptedMediaType( const CMediaType * pmt )
{
    CAutoLock lock( &m_Lock );

    if( !pmt )
    {
        m_mtAccept = CMediaType( );
        return NOERROR;        
    }

    HRESULT hr;
    hr = CopyMediaType( &m_mtAccept, pmt );

    return hr;
}

//----------------------------------------------------------------------------
// GetAcceptedMediaType
//----------------------------------------------------------------------------

STDMETHODIMP CPVSGrabber::GetConnectedMediaType( CMediaType * pmt )
{
    if( !m_pInput || !m_pInput->IsConnected( ) )
    {
        return VFW_E_NOT_CONNECTED;
    }

    return m_pInput->ConnectionMediaType( pmt );
}


//----------------------------------------------------------------------------
// SetCallback
//----------------------------------------------------------------------------

STDMETHODIMP CPVSGrabber::SetCallback( ISampleGrabberCB* Callback )
{
    CAutoLock lock( &m_Lock );

    m_callback = Callback;

    return NOERROR;
}


//----------------------------------------------------------------------------
// inform the input pin of the allocator buffer we wish to use. See the
// input pin's SetDeliverBuffer method for comments. 
//----------------------------------------------------------------------------

STDMETHODIMP CPVSGrabber::SetDeliveryBuffer( ALLOCATOR_PROPERTIES props, BYTE * m_pBuffer )
{
    // have the input/output pins been created?
    if( !InputPin( ) || !OutputPin( ) )
    {
        return E_POINTER;
    }

    // they can't be connected if we're going to be changing delivery buffers
    //
    if( InputPin( )->IsConnected( ) || OutputPin( )->IsConnected( ) )
    {
        return E_INVALIDARG;
    }

    return ((CPVSGrabberInPin*)m_pInput)->SetDeliveryBuffer( props, m_pBuffer );
}

//----------------------------------------------------------------------------
// Fill the give data buffer with a copy of the last received buffer
//----------------------------------------------------------------------------

STDMETHODIMP CPVSGrabber::GetCurrentBuffer(BYTE * data, long inputBufferSize, double* sampleTime, BOOL quartered)
{
	CAutoLock lock( &m_Lock );

	// Nothing sampled yet
	if (mBufferedSample == NULL)
	{
		Error("Nothing sampled on video returning blank");
		memset(data, 0, inputBufferSize);
		return 1;
	}

	if (mBufferedSampleType == CLONE_OF_MEDIA_SAMPLE && quartered == TRUE)
	{
		WarningOnce("CPVSGrabber::GetCurrentBuffer request for quartered sample, unexpected."));

		m_converter->convert_to_rgb32(data, mBufferedSample, inputBufferSize, mBufferedSampleSize, true);

		//
		// For future samples received make sure they are processed and quartered first
		//
		mProcessAndQuarterSampleWhenRecevied = TRUE;
	}
	else if (mBufferedSampleType == YUV_TO_RGB_PROCESSED_AND_QUARTERED)
	{
		if (quartered == FALSE)
		{
			//
			// This should not happen, but if it does, just return blank frame
			//
			memset(data, 0x64, inputBufferSize);
			mProcessAndQuarterSampleWhenRecevied = FALSE;
			WarningOnce("CPVSGrabber::GetCurrentBuffer request for normal sample but sample was quartered."));
		}
		else
		{

			if (mBufferedSampleSize < inputBufferSize)
			{
				//
				// Something gone wrong, just return blank frame
				//
				memset(data, 0, inputBufferSize);
				WarningOnce("CPVSGrabber::GetCurrentBuffer buffered sample smaller than input buffer size"));
			}
			else
			{
				memcpy(data, mBufferedSample, inputBufferSize);
			}
		}
	}
	else
	{
		//
		// Process now   (i.e. buffered sample is a clone and input quartered= false
		//
		m_converter->convert_to_rgb32(data, mBufferedSample, inputBufferSize, mBufferedSampleSize, false);
	}

    *sampleTime = mBufferedSampleTime;

	return NOERROR;
}


//----------------------------------------------------------------------------
// Return the average frame rate reported when connection was made
//----------------------------------------------------------------------------
STDMETHODIMP CPVSGrabber::GetFrameRate(double* rate)
{
	*rate = mFrameRate;
	return NOERROR;
}

//----------------------------------------------------------------------------
// Sets if we are processing and quartering samples when received.
// (speed up when editing 1080p video)
//----------------------------------------------------------------------------
STDMETHODIMP CPVSGrabber::SetProcessAndQuarterSampleWhenReceived(BOOL value)
{
	mProcessAndQuarterSampleWhenRecevied = value;
	return NOERROR;
}

//----------------------------------------------------------------------------
// used to help speed input pin connection times. We return a partially
// specified media type - only the main type is specified. If we return
// anything BUT a major type, some codecs written improperly will crash
//----------------------------------------------------------------------------

HRESULT CPVSGrabberInPin::GetMediaType( int iPosition, CMediaType * pMediaType )
{
    CheckPointer(pMediaType,E_POINTER);

    if (iPosition < 0) {
        return E_INVALIDARG;
    }
    if (iPosition > 0) {
        return VFW_S_NO_MORE_ITEMS;
    }

    *pMediaType = CMediaType( );
    pMediaType->SetType( ((CPVSGrabber*)m_pFilter)->m_mtAccept.Type( ) );

    return S_OK;
}

//----------------------------------------------------------------------------
// override the CTransInPlaceInputPin's method, and return a new enumerator
// if the input pin is disconnected. This will allow GetMediaType to be
// called. If we didn't do this, EnumMediaTypes returns a failure code
// and GetMediaType is never called. 
//----------------------------------------------------------------------------

STDMETHODIMP CPVSGrabberInPin::EnumMediaTypes( IEnumMediaTypes **ppEnum )
{
    CheckPointer(ppEnum,E_POINTER);
    ValidateReadWritePtr(ppEnum,sizeof(IEnumMediaTypes *));

    // if the output pin isn't connected yet, offer the possibly 
    // partially specified media type that has been set by the user

    if( !((CPVSGrabber*)m_pTIPFilter)->OutputPin( )->IsConnected() )
    {
        // Create a new reference counted enumerator

        *ppEnum = new CEnumMediaTypes( this, NULL );

        return (*ppEnum) ? NOERROR : E_OUTOFMEMORY;
    }

    // if the output pin is connected, offer it's fully qualified media type

    return ((CPVSGrabber*)m_pTIPFilter)->OutputPin( )->GetConnected()->EnumMediaTypes( ppEnum );
}


//----------------------------------------------------------------------------
//
//----------------------------------------------------------------------------

STDMETHODIMP CPVSGrabberInPin::NotifyAllocator( IMemAllocator *pAllocator, BOOL bReadOnly )
{
    if( m_pPrivateAllocator )
    {
        if( pAllocator != m_pPrivateAllocator )
        {
            return E_FAIL;
        }
        else
        {
            // if the upstream guy wants to be read only and we don't, then that's bad
            // if the upstream guy doesn't request read only, but we do, that's okay
            if( bReadOnly && !SampleGrabber( )->IsReadOnly( ) )
            {
                return E_FAIL;
            }
        }
    }

    return CTransInPlaceInputPin::NotifyAllocator( pAllocator, bReadOnly );
}


//----------------------------------------------------------------------------
//
//----------------------------------------------------------------------------

STDMETHODIMP CPVSGrabberInPin::GetAllocator( IMemAllocator **ppAllocator )
{
    if( m_pPrivateAllocator )
    {
        CheckPointer(ppAllocator,E_POINTER);

        *ppAllocator = m_pPrivateAllocator;
        m_pPrivateAllocator->AddRef( );
        return NOERROR;
    }
    else
    {
        return CTransInPlaceInputPin::GetAllocator( ppAllocator );
    }
}

//----------------------------------------------------------------------------
// GetAllocatorRequirements: The upstream filter calls this to get our
// filter's allocator requirements. If the app has set the buffer, then
// we return those props. Otherwise, we use the default TransInPlace behavior.
//----------------------------------------------------------------------------

HRESULT CPVSGrabberInPin::GetAllocatorRequirements( ALLOCATOR_PROPERTIES *pProps )
{
    CheckPointer(pProps,E_POINTER);

    if (m_pPrivateAllocator)
    {
        *pProps = m_allocprops;
        return S_OK;
    }
    else
    {
        return CTransInPlaceInputPin::GetAllocatorRequirements(pProps);
    }
}




//----------------------------------------------------------------------------
//
//----------------------------------------------------------------------------

HRESULT CPVSGrabberInPin::SetDeliveryBuffer( ALLOCATOR_PROPERTIES props, BYTE * pBuffer )
{
    // don't allow more than one buffer

    if( props.cBuffers != 1 )
    {
        return E_INVALIDARG;
    }
    if( !pBuffer )
    {
        return E_POINTER;
    }

    m_allocprops = props;
    m_pBuffer = pBuffer;

    // If there is an existing allocator, make sure that it is released
    // to prevent a memory leak
    if (m_pPrivateAllocator)
    {
        m_pPrivateAllocator->Release();
        m_pPrivateAllocator = NULL;
    }

    HRESULT hr = S_OK;

    m_pPrivateAllocator = new CPVSGrabberAllocator( this, &hr );
    if( !m_pPrivateAllocator )
    {
        return E_OUTOFMEMORY;
    }

    m_pPrivateAllocator->AddRef( );
    return hr;
}


//----------------------------------------------------------------------------
//
//----------------------------------------------------------------------------

HRESULT CPVSGrabberInPin::SetMediaType( const CMediaType *pmt )
{
    m_bMediaTypeChanged = TRUE;

    return CTransInPlaceInputPin::SetMediaType( pmt );
}


//----------------------------------------------------------------------------
// don't allocate the memory, just use the buffer the app provided
//----------------------------------------------------------------------------

HRESULT CPVSGrabberAllocator::Alloc( )
{
    // look at the base class code to see where this came from!

    CAutoLock lck(this);

    // Check he has called SetProperties
    HRESULT hr = CBaseAllocator::Alloc();
    if (FAILED(hr)) {
        return hr;
    }

    // If the requirements haven't changed then don't reallocate
    if (hr == S_FALSE) {
        ASSERT(m_pBuffer);
        return NOERROR;
    }
    ASSERT(hr == S_OK); // we use this fact in the loop below

    // Free the old resources
    if (m_pBuffer) {
        ReallyFree();
    }

    // Compute the aligned size
    LONG lAlignedSize = m_lSize + m_lPrefix;
    if (m_lAlignment > 1) 
    {
        LONG lRemainder = lAlignedSize % m_lAlignment;
        if (lRemainder != 0) 
        {
            lAlignedSize += (m_lAlignment - lRemainder);
        }
    }

    // Create the contiguous memory block for the samples
    // making sure it's properly aligned (64K should be enough!)
    ASSERT(lAlignedSize % m_lAlignment == 0);

    // don't create the buffer - use what was passed to us
    //
    m_pBuffer = m_pPin->m_pBuffer;

    if (m_pBuffer == NULL) {
        return E_OUTOFMEMORY;
    }

    LPBYTE pNext = m_pBuffer;
    CMediaSample *pSample;

    ASSERT(m_lAllocated == 0);

    // Create the new samples - we have allocated m_lSize bytes for each sample
    // plus m_lPrefix bytes per sample as a prefix. We set the pointer to
    // the memory after the prefix - so that GetPointer() will return a pointer
    // to m_lSize bytes.
    for (; m_lAllocated < m_lCount; m_lAllocated++, pNext += lAlignedSize) 
    {
        pSample = new CMediaSample(
                                NAME("Sample Grabber memory media sample"),
                                this,
                                &hr,
                                pNext + m_lPrefix,      // GetPointer() value
                                m_lSize);               // not including prefix

        ASSERT(SUCCEEDED(hr));
        if (pSample == NULL)
            return E_OUTOFMEMORY;

        // This CANNOT fail
        m_lFree.Add(pSample);
    }

    m_bChanged = FALSE;
    return NOERROR;
}


//----------------------------------------------------------------------------
// don't really free the memory
//----------------------------------------------------------------------------

void CPVSGrabberAllocator::ReallyFree()
{
    // look at the base class code to see where this came from!

    // Should never be deleting this unless all buffers are freed

    ASSERT(m_lAllocated == m_lFree.GetCount());

    // Free up all the CMediaSamples

    CMediaSample *pSample;
    for (;;) 
    {
        pSample = m_lFree.RemoveHead();
        if (pSample != NULL) 
        {
            delete pSample;
        } 
        else 
        {
            break;
        }
    }

    m_lAllocated = 0;

    // don't free the buffer - let the app do it
}


//----------------------------------------------------------------------------
// SetProperties: Called by the upstream filter to set the allocator
// properties. The application has already allocated the buffer, so we reject 
// anything that is not compatible with that, and return the actual props.
//----------------------------------------------------------------------------

HRESULT CPVSGrabberAllocator::SetProperties(
    ALLOCATOR_PROPERTIES *pRequest, 
    ALLOCATOR_PROPERTIES *pActual
)
{
    HRESULT hr = CMemAllocator::SetProperties(pRequest, pActual);

    if (FAILED(hr))
    {
        return hr;
    }
    
    ALLOCATOR_PROPERTIES *pRequired = &(m_pPin->m_allocprops);
    if (pRequest->cbAlign != pRequired->cbAlign)
    {
        return VFW_E_BADALIGN;
    }
    if (pRequest->cbPrefix != pRequired->cbPrefix)
    {
        return E_FAIL;
    }
    if (pRequest->cbBuffer > pRequired->cbBuffer)
    {
        return E_FAIL;
    }
    if (pRequest->cBuffers > pRequired->cBuffers)
    {
        return E_FAIL;
    }

    *pActual = *pRequired;

    m_lCount = pRequired->cBuffers;
    m_lSize = pRequired->cbBuffer;
    m_lAlignment = pRequired->cbAlign;
    m_lPrefix = pRequired->cbPrefix;

    return S_OK;
}
