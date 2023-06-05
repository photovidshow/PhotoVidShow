#include "asyncio.h"
#include "asyncrdr.h"
#include "memstream.h"

// *********************************************************************************************
// This class can replace the normal 'Async source filter' when loading MP3's that contain
// tag frames that are not compatible with the 'Mpeg1 splitter' filter.  e.g. Large picture frames
// (ID3 APIC frames)
// This class simply strips out the id3 TAG and frames, and passes the raw mpeg1-layer 3 audio
// to the mpeg 1 splitter.
// *********************************************************************************************
class CMP3TagStripperSourceFilter : public CAsyncReader, public IFileSourceFilter
{
public:
	CMP3TagStripperSourceFilter(LPUNKNOWN pUnk, HRESULT *phr);

	~CMP3TagStripperSourceFilter();
   
    static CUnknown * WINAPI CreateInstance(LPUNKNOWN, HRESULT *);

    DECLARE_IUNKNOWN

	STDMETHODIMP NonDelegatingQueryInterface(REFIID riid, void **ppv);
    
    /*  IFileSourceFilter methods */

    //  Load a (new) file
	STDMETHODIMP Load(LPCOLESTR lpwszFileName, const AM_MEDIA_TYPE *pmt);

    // Modeled on IPersistFile::Load
    // Caller needs to CoTaskMemFree or equivalent.

	STDMETHODIMP GetCurFile(LPOLESTR * ppszFileName, AM_MEDIA_TYPE *pmt);

private:
    BOOL ParseTheFile(LPCTSTR lpszFileName);
	BOOL StripMp3TagFrames(PBYTE mem);
	int CovertSyncSafeIntToInt(BYTE syncSafeInt[4]);

private:
    LPWSTR     m_pFileName;
    LONGLONG   m_llSize;
    DWORD      m_totalTagSize;
    CMemStream m_Stream;
};
