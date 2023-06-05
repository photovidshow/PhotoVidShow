#include <streams.h>

#include "asyncio.h"
#include "asyncrdr.h"

#pragma warning(disable:4710)  // 'function' not inlined (optimization)
#include "MP3TagStripperSourceFilter.h"
#include "../UnmanagedErrors.h"

#define MP3_HEADER_SIZE 10

// **************************************************************************************
CMP3TagStripperSourceFilter::CMP3TagStripperSourceFilter(LPUNKNOWN pUnk, HRESULT *phr) :
	CAsyncReader(NAME("Mem Reader"), pUnk, &m_Stream, phr),
	m_pFileName(NULL),
	m_totalTagSize(0)
{
}


// **************************************************************************************
CMP3TagStripperSourceFilter::~CMP3TagStripperSourceFilter()
{
	if (m_pFileName != NULL)
	{
		delete[] m_pFileName;
		m_pFileName = NULL;
	}
}

// **************************************************************************************
STDMETHODIMP CMP3TagStripperSourceFilter::NonDelegatingQueryInterface(REFIID riid, void **ppv)
{
	if (riid == IID_IFileSourceFilter) {
		return GetInterface((IFileSourceFilter *)this, ppv);
	}
	else {
		return CAsyncReader::NonDelegatingQueryInterface(riid, ppv);
	}
}


// **************************************************************************************
// Create a new instance of this class
CUnknown * WINAPI CMP3TagStripperSourceFilter::CreateInstance(LPUNKNOWN pUnk, HRESULT *phr)
{
    ASSERT(phr);

    //  DLLEntry does the right thing with the return code and
    //  the returned value on failure

    return new CMP3TagStripperSourceFilter(pUnk, phr);
}


// **************************************************************************************
STDMETHODIMP CMP3TagStripperSourceFilter::Load(LPCOLESTR lpwszFileName, const AM_MEDIA_TYPE *pmt)
{
	CheckPointer(lpwszFileName, E_POINTER);

	// lstrlenW is one of the few Unicode functions that works on win95
	int cch = lstrlenW(lpwszFileName) + 1;

#ifndef UNICODE
	TCHAR *lpszFileName = 0;
	lpszFileName = new char[cch * 2];
	if (!lpszFileName) {
		return E_OUTOFMEMORY;
	}
	WideCharToMultiByte(GetACP(), 0, lpwszFileName, -1,
		lpszFileName, cch, NULL, NULL);
#else
	TCHAR lpszFileName[MAX_PATH] = { 0 };
	lstrcpy(lpszFileName, lpwszFileName);
#endif
	CAutoLock lck(&m_csFilter);

	/*  Check the file type */
	CMediaType cmt;
	if (NULL == pmt) {
		cmt.SetType(&MEDIATYPE_Stream);
		cmt.SetSubtype(&MEDIASUBTYPE_NULL);
	}
	else {
		cmt = *pmt;
	}

	if (!ParseTheFile(lpszFileName)) {
#ifndef UNICODE
		delete[] lpszFileName;
#endif
		return E_FAIL;
	}

	m_Stream.Init(lpszFileName, m_llSize, m_totalTagSize );

	m_pFileName = new WCHAR[cch];

	if (m_pFileName != NULL)
		CopyMemory(m_pFileName, lpwszFileName, cch*sizeof(WCHAR));

	// this is not a simple assignment... pointers and format
	// block (if any) are intelligently copied
	m_mt = cmt;

	/*  Work out file type */
	cmt.bTemporalCompression = TRUE;	       //???
	cmt.lSampleSize = 1;

	return S_OK;
}

// **************************************************************************************
STDMETHODIMP CMP3TagStripperSourceFilter::GetCurFile(LPOLESTR * ppszFileName, AM_MEDIA_TYPE *pmt)
{
	CheckPointer(ppszFileName, E_POINTER);
	*ppszFileName = NULL;

	if (m_pFileName != NULL) {
		DWORD n = sizeof(WCHAR)*(1 + lstrlenW(m_pFileName));

		*ppszFileName = (LPOLESTR)CoTaskMemAlloc(n);
		if (*ppszFileName != NULL) {
			CopyMemory(*ppszFileName, m_pFileName, n);
		}
	}

	if (pmt != NULL) {
		CopyMediaType(pmt, &m_mt);
	}

	return NOERROR;
}


// **************************************************************************************
BOOL CMP3TagStripperSourceFilter::ParseTheFile(LPCTSTR lpszFileName)
{
	DWORD dwBytesRead = 0;

	//
    // Open the requested file
	//
    HANDLE hFile = CreateFile(lpszFileName,
                              GENERIC_READ,
                              FILE_SHARE_READ | FILE_SHARE_WRITE,
                              NULL,
                              OPEN_EXISTING,
                              0,
                              NULL);
    if (hFile == INVALID_HANDLE_VALUE) 
    {
		Error("MP3 tag stripper source filter could not open file %s", lpszFileName);
        return FALSE;
    }

	//
    // Determine the file size
	//
    ULARGE_INTEGER uliSize;
	memset(&uliSize, 0, sizeof(uliSize));
    uliSize.LowPart = GetFileSize(hFile, &uliSize.HighPart);

	if (uliSize.LowPart < MP3_HEADER_SIZE)
	{
		return FALSE;
	}

	BYTE pbMem[MP3_HEADER_SIZE] = { 0 };

	//
    // Read the data from the file
	//
    if (!ReadFile(hFile,
                  (LPVOID) pbMem,
				  MP3_HEADER_SIZE,
                  &dwBytesRead,
                  NULL) ||
        (dwBytesRead != MP3_HEADER_SIZE))
    {
		Error("MP3 tag stripper source filter could not read header %s", lpszFileName);
        CloseHandle(hFile);
        return FALSE;
    }

	m_llSize = uliSize.QuadPart;

	BOOL result = StripMp3TagFrames(pbMem);
	if (result == FALSE)
	{
		//
		// Unable to strip tags from mp3 file.  It may not contain any, so this is
		// not necessarily an error. 
		//
		return FALSE;
	}

	//
    // Close the file
	//
    CloseHandle(hFile);

    return TRUE;
}

// **************************************************************************************
BOOL CMP3TagStripperSourceFilter::StripMp3TagFrames(PBYTE mem)
{
	BOOL startwithI = ((mem[0] == 'I') || (mem[1] == 'i'));
	BOOL thenD = ((mem[1] == 'D') || (mem[1] == 'd'));

	if ((startwithI == FALSE) || (thenD == FALSE) || (mem[2] != '3') ||
		(mem[3] == 0xFF) || (mem[4] == 0xFF) ||
		(mem[6] > 0x80) || (mem[7] > 0x80) || (mem[8] > 0x80) || (mem[9] > 0x80))
	{
		//
		// this stream has no tag or invalid tag length
		//
		return FALSE;
	}

	//
	// Calculate total size of headers and tags so we can strip out the raw mpeg-layer 3 part
	//
	// The ID3v2 tag size is the size of the complete tag after unsychronisation, including padding / footer,
	// excluding the header but not excluding the extended header(total tag size - 10).
	// Only 28 bits(representing up to 256MB) are used in the size description to avoid the
	// introducuction of 'false syncsignals'.
	//
	BYTE syncSafeLength[4];
	syncSafeLength[0] = mem[6];
	syncSafeLength[1] = mem[7];
	syncSafeLength[2] = mem[8];
	syncSafeLength[3] = mem[9];
	
	m_totalTagSize = (DWORD) CovertSyncSafeIntToInt(syncSafeLength);

	//
	// Sanity check that tagLength would fall inside file
	//
	if (m_totalTagSize <= 0 || m_totalTagSize >= m_llSize)
	{
		return FALSE;
	}

	return TRUE;
}

// **************************************************************************************
int CMP3TagStripperSourceFilter::CovertSyncSafeIntToInt(BYTE syncSafeInt[4])
{
	BYTE buffer[4];

	//
	// first shift in all bytes, keeping it in network (MSB) order
	//
	buffer[3] = syncSafeInt[3];
	buffer[3] |= (BYTE)(syncSafeInt[2] << 7);

	buffer[2] = (BYTE)(syncSafeInt[2] >> 1);
	buffer[2] |= (BYTE)(syncSafeInt[1] << 6);

	buffer[1] = (BYTE)(syncSafeInt[1] >> 2);
	buffer[1] |= (BYTE)(syncSafeInt[0] << 5);

	buffer[0] = (BYTE)(syncSafeInt[0] >> 3);

	//
	// swap to LSB
	//
	BYTE lsb[4];
	lsb[0] = buffer[3];
	lsb[1] = buffer[2];
	lsb[2] = buffer[1];
	lsb[3] = buffer[0];

	//
	// Convert to an int
	//
	int output = *((int*)lsb);

	return output;
}

