
#include "asyncio.h"
#include "asyncrdr.h"

class CFileBufferCache;

//
// This class is used by the Mp3TagStripperSourceFilter class to stream the mp3 data
//
class CMemStream : public CAsyncStream
{
public:
	CMemStream();
	~CMemStream();

    /*  Initialization */
	void Init(char* filename, LONGLONG llLength, DWORD m_TagLength, DWORD dwKBPerSec = INFINITE);
  
	HRESULT SetPointer(LONGLONG llPos);
  
	HRESULT Read(PBYTE pbBuffer,
		DWORD dwBytesToRead,
		BOOL bAlign,
		LPDWORD pdwBytesRead);
   
	LONGLONG Size(LONGLONG *pSizeAvailable);
  
	DWORD Alignment();

	void Lock();

	void Unlock();
 

private:
    CCritSec       m_csLock;
    PBYTE          m_pbData;
    LONGLONG       m_llLength;
    LONGLONG       m_llPosition;
    DWORD          m_dwKBPerSec;
    DWORD          m_dwTimeStart;
	char*		   m_filename;
	CFileBufferCache* m_bufferCache;
	DWORD		   m_TagLength;
};
