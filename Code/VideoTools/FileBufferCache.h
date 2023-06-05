// FileBufferCache.h: interface for the CFileBufferCache class.
//
//////////////////////////////////////////////////////////////////////

#ifndef FILE_BUFFER_CACHE_INCLUDE
#define FILE_BUFFER_CACHE_INCLUDE

#include <string>

extern char * g_output_dir;

//
// Top level buffer abstract class
//
class CBuffer
{
public:
			CBuffer();
	virtual ~CBuffer();
	virtual void GetData(__int64 offset, unsigned char* buffer, int amount) =0 ;
	virtual void WriteData(__int64 offset, unsigned char* buffer, int amount) =0;
	virtual __int64	 GetSize() =0;
	virtual int GetCacheSize() { return (int) GetSize() ; }
	virtual char* GetIDString() { static char null[] ="";  return null; } 
	virtual bool SaveToFile(std::string filename, int skip_inital_bytes) = 0 ;
};



#define DEFUALT_FILE_CACHE 1024*128

//
// FileBufferCache class represents a mechanism to read and write binary data to an
// existing large file.  Typically the file referenced will be too big to load in one go
// so this class will only load small segments of the file at once in an internal
// buffer. This class now support files above 2.5 gig
//
class CFileBufferCache : public CBuffer
{
public:
	CFileBufferCache(std::string filename, int cache_size = DEFUALT_FILE_CACHE);

	virtual ~CFileBufferCache();

			void WriteBuffer(CBuffer* buffer, __int64 our_buffer_index_start, __int64 input_buffer_index_start=0, __int64 amount=-1);

	virtual void GetData(__int64 offset, unsigned char* buffer, int amount);
	virtual void WriteData(__int64 offset, unsigned char* buffer, int amount);

	virtual __int64	 GetSize() { return mTotalSize ; }
	virtual int	 GetCacheSize() { return mCacheSize; }
	virtual char* GetIDString() {   return (char*)mFilename.c_str(); } 
	void WriteBlank(__int64 offset, __int64 amount);

	virtual bool SaveToFile(std::string filename, int skip_initial_bytes) ;
	std::string GetFilename() { return mFilename; }

	int		ReadByte(__int64 offset);

	void WriteHackWavHead();

	static bool mAbort;


private:

	bool Seek(__int64 value);

	unsigned char* mBuffer;
	std::string	mFilename;
	__int64 	mOffset;			// Offset position of buffer from start of file
	int 		mCacheSize;			// Max size buffer we can allocate in memory at once
	__int64 	mTotalSize;			// Total virtual size of this buffer
	FILE*		mFP;
	bool		mReadOnly;
	bool		mCacheNeedsWriting;
	int 		mCurrentBufferSize;	// current size of allocated buffer in memory (may be smaller than max cache size)

};



//
// MemoryBufferCache represents a large memory buffer that would be typically too large
// to store in memory.  So the buffer is internally done as a file buffer. 
// The filename of the file buffer is decided internally. 
//

class CMemoryBufferCache : public CFileBufferCache
{
public:
	CMemoryBufferCache(__int64 total_size, int cache_size);

	virtual ~CMemoryBufferCache();


private:

	std::string CreateFileCache(__int64 total_size);

	static int mCacheId ; 
};


#endif 


