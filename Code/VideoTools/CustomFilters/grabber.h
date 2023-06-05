
class IPVSGrabber : public IUnknown
{
    public:
        
        virtual HRESULT STDMETHODCALLTYPE SetAcceptedMediaType( 
            const CMediaType *pType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetConnectedMediaType( 
            CMediaType *pType) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetCallback( 
            ISampleGrabberCB* Callback) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetDeliveryBuffer( 
            ALLOCATOR_PROPERTIES props,
            BYTE *pBuffer) = 0;

		virtual HRESULT STDMETHODCALLTYPE GetCurrentBuffer(
			BYTE * data, 
			long inputBufferSize,
            double* sampleTime,
			BOOL quartered) =0;

		virtual HRESULT STDMETHODCALLTYPE ClearBuffers() =0;

		virtual HRESULT STDMETHODCALLTYPE GetFrameRate(double* rate) =0;

		virtual HRESULT STDMETHODCALLTYPE SetProcessAndQuarterSampleWhenReceived(BOOL value) = 0;
};
        

class CPVSGrabberInPin;
class CPVSGrabber;

//----------------------------------------------------------------------------
// This is a special allocator that KNOWS that the person who is creating it
// will only create one of them. It allocates CMediaSamples that only 
// reference the buffer location that is set in the pin's renderer's
// data variable
//----------------------------------------------------------------------------

class CPVSGrabberAllocator : public CMemAllocator
{
    friend class CPVSGrabberInPin;
    friend class CPVSGrabber;

protected:

    // our pin who created us
    //
    CPVSGrabberInPin * m_pPin;

public:

    CPVSGrabberAllocator( CPVSGrabberInPin * pParent, HRESULT *phr ) 
        : CMemAllocator( TEXT("SampleGrabberAllocator\0"), NULL, phr ) 
        , m_pPin( pParent )
    {
    };

    ~CPVSGrabberAllocator( )
    {
        // wipe out m_pBuffer before we try to delete it. It's not an allocated
        // buffer, and the default destructor will try to free it!
        m_pBuffer = NULL;
    }

    HRESULT Alloc( );

    void ReallyFree();

    // Override this to reject anything that does not match the actual buffer
    // that was created by the application
    STDMETHODIMP SetProperties(ALLOCATOR_PROPERTIES *pRequest, ALLOCATOR_PROPERTIES *pActual);

};

//----------------------------------------------------------------------------
// we override the input pin class so we can provide a media type
// to speed up connection times. When you try to connect a filesourceasync
// to a transform filter, DirectShow will insert a splitter and then
// start trying codecs, both audio and video, video codecs first. If
// your sample grabber's set to connect to audio, unless we do this, it
// will try all the video codecs first. Connection times are sped up x10
// for audio with just this minor modification!
//----------------------------------------------------------------------------

class CPVSGrabberInPin : public CTransInPlaceInputPin
{
    friend class CPVSGrabberAllocator;
    friend class CPVSGrabber;

    CPVSGrabberAllocator * m_pPrivateAllocator;
    ALLOCATOR_PROPERTIES m_allocprops;
    BYTE * m_pBuffer;
    BOOL m_bMediaTypeChanged;

protected:

    CPVSGrabber * SampleGrabber( ) { return (CPVSGrabber*) m_pFilter; }
    HRESULT SetDeliveryBuffer( ALLOCATOR_PROPERTIES props, BYTE * m_pBuffer );

public:

    CPVSGrabberInPin( CTransInPlaceFilter * pFilter, HRESULT * pHr ) 
        : CTransInPlaceInputPin( TEXT("SampleGrabberInputPin\0"), pFilter, pHr, L"Input\0" )
        , m_pPrivateAllocator( NULL )
        , m_pBuffer( NULL )
        , m_bMediaTypeChanged( FALSE )
    {
        memset( &m_allocprops, 0, sizeof( m_allocprops ) );
    }

    ~CPVSGrabberInPin( )
    {
        if( m_pPrivateAllocator ) delete m_pPrivateAllocator;
    }

    // override to provide major media type for fast connects

    HRESULT GetMediaType( int iPosition, CMediaType *pMediaType );

    // override this or GetMediaType is never called

    STDMETHODIMP EnumMediaTypes( IEnumMediaTypes **ppEnum );

    // override this to refuse any allocators besides
    // the one the user wants, if this is set

    STDMETHODIMP NotifyAllocator( IMemAllocator *pAllocator, BOOL bReadOnly );

    // override this so we always return the special allocator, if necessary

    STDMETHODIMP GetAllocator( IMemAllocator **ppAllocator );

    HRESULT SetMediaType( const CMediaType *pmt );

    // we override this to tell whoever's upstream of us what kind of
    // properties we're going to demand to have
    //
    STDMETHODIMP GetAllocatorRequirements( ALLOCATOR_PROPERTIES *pProps );



};

//----------------------------------------------------------------------------
//
//----------------------------------------------------------------------------

class CColorSpaceConverter;

class CPVSGrabber : public CTransInPlaceFilter,
                    public IPVSGrabber
{
    friend class CPVSGrabberInPin;
    friend class CPVSGrabberAllocator;

protected:

    CMediaType m_mtAccept;
    ISampleGrabberCB* m_callback;
    CCritSec m_Lock; // serialize access to our data


	//
	// What type of data have we buffered away
	//
	enum EBufferdSampleType
	{
		UNKNOWN,
		CLONE_OF_MEDIA_SAMPLE,
		YUV_TO_RGB_PROCESSED_AND_QUARTERED
	};

	BYTE* mBufferedSample;
	EBufferdSampleType mBufferedSampleType;
    double mBufferedSampleTime;
	int mBufferedSampleSize;
	BOOL mProcessAndQuarterSampleWhenRecevied;			// speed up when editing >= 1080p hd video. If true this means that when we receive a sample transform to RGB and quarter resolution.

	int mWidth;
	int mHeight;
	double mFrameRate;	// Stores the frame rate reported on pin negotiation/connection

	CColorSpaceConverter* m_converter;

    BOOL IsReadOnly( ) { return !m_bModifiesData; }

    // PURE, override this to ensure we get 
    // connected with the right media type
    HRESULT CheckInputType( const CMediaType * pmt );

    // PURE, override this to callback 
    // the user when a sample is received
    HRESULT Transform( IMediaSample * pms );

    // override this so we can return S_FALSE directly. 
    // The base class CTransInPlace
    // Transform( ) method is called by it's 
    // Receive( ) method. There is no way
    // to get Transform( ) to return an S_FALSE value 
    // (which means "stop giving me data"),
    // to Receive( ) and get Receive( ) to return S_FALSE as well.

    HRESULT Receive( IMediaSample * pms );

public:

    // Expose ISampleGrabber
    STDMETHODIMP NonDelegatingQueryInterface(REFIID riid, void ** ppv);
    DECLARE_IUNKNOWN;

    CPVSGrabber( IUnknown * pOuter, HRESULT * pHr, BOOL ModifiesData );
	~CPVSGrabber();

    // IPVSGrabber
    STDMETHODIMP SetAcceptedMediaType( const CMediaType * pmt );
    STDMETHODIMP GetConnectedMediaType( CMediaType * pmt );
    STDMETHODIMP SetCallback( ISampleGrabberCB* Callback );
    STDMETHODIMP SetDeliveryBuffer( ALLOCATOR_PROPERTIES props, BYTE * m_pBuffer );
    STDMETHODIMP GetCurrentBuffer( BYTE * data, long inputBufferSize, double* sampleTime, BOOL quartered);
    STDMETHODIMP ClearBuffers();
	STDMETHODIMP GetFrameRate(double* rate);
	STDMETHODIMP SetProcessAndQuarterSampleWhenReceived(BOOL value);

	BYTE* GetBufferedSample() { return mBufferedSample; }

};
