#include <streams.h>


#pragma warning(disable:4710)  // 'function' not inlined (optimization)
#include "memstream.h"
#include "../FileBufferCache.h"

// *****************************************************************************************
CMemStream::CMemStream() :
	m_llPosition(0),
	m_bufferCache(NULL)
{
}


// *****************************************************************************************
CMemStream::~CMemStream()
{
	if (m_bufferCache != NULL)
	{
		delete m_bufferCache;
		m_bufferCache = NULL;
	}
}

// *****************************************************************************************
void CMemStream::Init(char* filename, LONGLONG llLength, DWORD tagLength, DWORD dwKBPerSec)
{
	m_llLength = llLength - tagLength;
	m_dwKBPerSec = dwKBPerSec;
	m_dwTimeStart = timeGetTime();
	m_TagLength = tagLength;
	m_filename = filename;

	//
	// Mpeg 1 splitter seems to want things in 32k chunks, so
	// we set our cache size to match this.
	//
	m_bufferCache = new CFileBufferCache(filename, 32 * 1024);

}

// *****************************************************************************************
HRESULT CMemStream::SetPointer(LONGLONG llPos)
{
	if (llPos < 0 || llPos > m_llLength) {
		return S_FALSE;
	}
	else {
		m_llPosition = llPos;
		return S_OK;
	}
}

// *****************************************************************************************
HRESULT CMemStream::Read(PBYTE pbBuffer,
	DWORD dwBytesToRead,
	BOOL bAlign,
	LPDWORD pdwBytesRead)
{
	CAutoLock lck(&m_csLock);
	DWORD dwReadLength;

	/*  Wait until the bytes are here! */
	DWORD dwTime = timeGetTime();

	if (m_llPosition + dwBytesToRead > m_llLength) {
		dwReadLength = (DWORD)(m_llLength - m_llPosition);
	}
	else {
		dwReadLength = dwBytesToRead;
	}
	DWORD dwTimeToArrive =
		((DWORD)m_llPosition + dwReadLength) / m_dwKBPerSec;

	if (dwTime - m_dwTimeStart < dwTimeToArrive) {
		Sleep(dwTimeToArrive - dwTime + m_dwTimeStart);
	}

	static DWORD maxLength = 0;

	m_bufferCache->GetData(m_llPosition + m_TagLength, pbBuffer, dwReadLength);

	if (dwReadLength > maxLength)
	{
		maxLength = dwReadLength;
	}

	m_llPosition += dwReadLength;
	*pdwBytesRead = dwReadLength;
	return S_OK;
}


// *****************************************************************************************
LONGLONG CMemStream::Size(LONGLONG *pSizeAvailable)
{
	LONGLONG llCurrentAvailable =
		static_cast <LONGLONG> (UInt32x32To64((timeGetTime() - m_dwTimeStart), m_dwKBPerSec));

	*pSizeAvailable = min(m_llLength, llCurrentAvailable);
	return m_llLength;
}


// *****************************************************************************************
DWORD CMemStream::Alignment()
{
	return 1;
}


// *****************************************************************************************
void CMemStream::Lock()
{
	m_csLock.Lock();
}


// *****************************************************************************************
void CMemStream::Unlock()
{
	m_csLock.Unlock();
}

