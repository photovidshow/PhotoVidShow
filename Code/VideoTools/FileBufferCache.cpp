// FileBufferCache.cpp: implementation of the CFileBufferCache class.
//
//////////////////////////////////////////////////////////////////////

#define _CRT_SECURE_NO_WARNINGS 1

#include <iostream>
#include <fstream>
#include "windows.h"

#include "FileBufferCache.h"
#include "UnmanagedErrors.h"

using namespace std;
//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

bool CFileBufferCache::mAbort=false;
char * g_output_dir;


//***************************************************************************
CBuffer::CBuffer()
{
	Trace("Created buffer %p",this);
}


//***************************************************************************
CBuffer::~CBuffer()
{
	Trace("Deleting buffer %p",this);
}

//***************************************************************************
CFileBufferCache::CFileBufferCache(std::string filename, int cache_size)
{
	mBuffer = new unsigned char[cache_size];
	mFilename = filename;
	mCacheSize = cache_size;
	mReadOnly= false;
	mCacheNeedsWriting = false ;
	mCurrentBufferSize = mCacheSize;
	mTotalSize=0;

	mFP = fopen(filename.c_str(),"r+b");
	if (mFP==NULL) 
	{
		// try read only
		mFP = fopen(filename.c_str(),"rb");
		if (mFP==NULL)
		{
			Error("File not found %s\n", mFilename.c_str() );
			return ;
		}
		Warning("Reading file buffer cache '%s' as read only.",mFilename.c_str() );
		mReadOnly = true;
	}

	HANDLE hFile = CreateFile(filename.c_str(), GENERIC_READ, 
		FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING,
        FILE_ATTRIBUTE_NORMAL, NULL);
    if (hFile==INVALID_HANDLE_VALUE)
	{
		Error("Unable to read file buffer cache '%s'", mFilename.c_str());
        return ;
	}

    LARGE_INTEGER size;
    if (!GetFileSizeEx(hFile, &size))
    {
		Error("Unable to get size of file buffer cache '%s'", mFilename.c_str());
        CloseHandle(hFile);
        return;
    }

    CloseHandle(hFile);

    mTotalSize = size.QuadPart;

	if (mTotalSize < mCacheSize && mTotalSize >0)
	{
		Trace("Cache size bigger than file size, reducing...");
		mCacheSize = (int)mTotalSize ;
	}

	Seek(0);
}

//***************************************************************************
bool CFileBufferCache::Seek(__int64 offset)
{
	if (mCacheNeedsWriting==true)
	{
		_fseeki64(mFP,mOffset,SEEK_SET);
		fwrite(mBuffer,1,mCurrentBufferSize,mFP);
		mCacheNeedsWriting=false;
	}

	int to_read = mCacheSize;

	if (offset > mTotalSize)
	{
		Warning("Trying to seek beyond buffer %I64u %I64u '%s'", offset, mTotalSize, mFilename.c_str());
		return false;
	}

	// just read end bits
	if (offset > mTotalSize - mCacheSize)
	{
		to_read = (int) (mTotalSize - offset);
	}
	if (offset <0)
	{
		Warning("Trying to seek before start of buffer %I64u '%s'", offset, mFilename.c_str());
		offset = 0;
	}

	int i = _fseeki64(mFP,offset,SEEK_SET);
	if (i!=0)
	{
		Error("_fseeki64 failed");
		return false;
	}

	int read = (int) fread(mBuffer,1,to_read,mFP);

	mOffset =offset;
	mCurrentBufferSize = to_read;

	if (read != to_read)
	{
		Error("fread failed to read correct number of bytes.  Read %d expected %d", read, to_read);
		return false ;
	}

	return true;
}


//***************************************************************************
int	CFileBufferCache::ReadByte(__int64 offset)
{	
	unsigned char bloc;

	GetData(offset, &bloc,1);

	return (int) bloc;
}


//***************************************************************************
void CFileBufferCache::GetData(__int64 offset, unsigned char* buffer, int amount)
{
	mAbort=false;

	bool seek_needed = true;
	// is start inside cache
	if (offset >= mOffset && offset < mOffset+mCacheSize)
	{
		seek_needed= false;
	}

	// in loop rather than recursive call to prevent stack overflow when buffer small and amount is massive !!

	while (1==1)
	{
		if (mAbort==true) return ;

		if (seek_needed==true)
		{
			if ( Seek(offset)==false)
			{
				return;
			}
		}

		// is total get inside cache
		if (offset+amount <= mOffset+mCacheSize)
		{
			memcpy(buffer, mBuffer+(offset-mOffset),amount);
			return ;
		}

		// copy what data is in this cache and then swap
		int bytes_to_write = mCacheSize-( (int)(offset-mOffset));

		memcpy(buffer, mBuffer+(offset-mOffset), bytes_to_write);

		offset = mOffset+mCacheSize ;
		buffer = buffer+bytes_to_write;
		amount-= bytes_to_write;
		seek_needed= true;
	}

	return ;
}



//***************************************************************************
void CFileBufferCache::WriteBuffer(CBuffer* buffer, __int64 start, __int64 input_buffer_start_index, __int64 amount)
{

	mAbort=false;
	
	int cache_size= buffer->GetCacheSize();

	unsigned char* temp_buffer = new unsigned char[cache_size];
	__int64 current=input_buffer_start_index;
	__int64 write_current=0;

	__int64 total = buffer->GetSize() - input_buffer_start_index;

	if (amount!=-1)
	{
		total = amount ;
	}

	__int64 our_size = GetSize();
	if (our_size < total+start)
	{
		Trace("There's not enough room to perform WriteBuffer clipping end",GetSize());
	}

	while (total > cache_size && mAbort==false)
	{
		buffer->GetData(current, temp_buffer, cache_size);

		int to_write = cache_size;

		if (write_current+start+to_write > our_size)
		{
			to_write =(int)( our_size - (current+start) ) ;
		}

		if (to_write >0)
		{
			this->WriteData(write_current+start, temp_buffer, to_write);
		}


		current+=cache_size;
		write_current+=cache_size;
		total-=cache_size;
	}

	if (mAbort==false)
	{
		//
		// total should now be less than cache_size, so safe to downcast
		//
		int to_write = (int)total;

		buffer->GetData(current, temp_buffer, to_write);

		if (write_current+start+total > our_size)
		{
			to_write = (int) (our_size - (current+start)) ;
		}

		if (to_write >0 && mAbort==false)
		{
			this->WriteData(write_current+start, temp_buffer, to_write);
		}
	}

	delete [] temp_buffer;

	Trace("Finished call to WriteBuffer" );
}


//***************************************************************************
void CFileBufferCache::WriteBlank(__int64 offset, __int64 amount)
{
	// 100k blank audio buffer
	unsigned char* temp = new unsigned char[1024*100];
	for (int i=0;i<1024*100;i++)
	{
		temp[i]=0;
	}

	while (amount > 1024*100)
	{
		WriteData(offset, temp, 1024*100);
		offset+=1024*100;
		amount -=1024*100;
	}

	// fiddly bit at end
	WriteData(offset, temp, (int)amount);

	// clean up
	delete [] temp;
}


//***************************************************************************
void CFileBufferCache::WriteData(__int64 offset, unsigned char* buffer, int amount)
{
	if (mReadOnly==true)
	{
		Error("Can't call CFileBufferCache::WriteData on read only buffer\n");
		return ;
	}

	bool seek_needed = true;
	// is start inside cache
	if (offset >= mOffset && offset < mOffset+mCacheSize)
	{
		seek_needed= false;
	}

	// in loop rather than recursive call to prevent stack overflow when buffer small and amount is massive !!
	while (1==1)
	{
		if (mAbort==true) return;

		if (seek_needed==true)
		{
			if (Seek(offset)==false) return ;
		}

		// is total get inside cache
		if (offset+amount <= mOffset+mCacheSize)
		{
			memcpy(mBuffer+(offset-mOffset),buffer, amount);
			mCacheNeedsWriting = true;
			return ;
		}

		// copy what data is in this cache and then swap
		int bytes_to_write =(int)( mCacheSize-(offset-mOffset));

		memcpy(mBuffer+(offset-mOffset), buffer, bytes_to_write);
		mCacheNeedsWriting = true;

		offset = mOffset+mCacheSize ;
		buffer = buffer+bytes_to_write;
		amount-= bytes_to_write;
		seek_needed= true;
	}

	return ;


}



//***************************************************************************
CFileBufferCache::~CFileBufferCache()
{
	if (mCacheNeedsWriting==true)
	{
		Seek(0);
	}

	if (mFP) fclose(mFP);
	delete [] mBuffer;
}


//***************************************************************************
bool CFileBufferCache::SaveToFile(std::string filename, int skip_inital_bytes)
{
	mAbort=false;

	Seek(0);	// force any memory cache to be written

	int b_s = this->GetCacheSize();
	__int64 total_size = this->GetSize()-skip_inital_bytes;

	FILE* fp = fopen(filename.c_str(),"wb");
	if (fp==NULL)
	{
		Error("Unable to copy FileBufferCache '%s' to new file '%s'", mFilename.c_str(), filename.c_str());
		return false;
	}

	unsigned char*m = new unsigned char[b_s];

	int offset =skip_inital_bytes;
	while (total_size > b_s && mAbort==false)
	{
		this->GetData(offset, m, b_s);
		fwrite(m,1,b_s,fp);
		total_size -= b_s;
		offset+=b_s;
	}

	if (total_size >0 && mAbort==false)
	{
		this->GetData(offset, m, (int)total_size);	// total size will be less than b_s size so safe to downcast
		fwrite(m,1,(int)total_size,fp);
	}

	fclose(fp);
	delete [] m;
	
	return true;
}


//***************************************************************************
void CFileBufferCache::WriteHackWavHead()
{
	unsigned char data[16*3] = {  0x52, 0x49, 0x46, 0x46, 0x26, 0xf4, 0x27, 0x02,     0x57,0x41,0x56,0x45,0x66,0x6d,0x74,0x20,
								  0x12, 0x00, 0x00, 0x00, 0x01, 0x00, 0x02, 0x00,		0x44,0xac,0x00,0x00,0x10,0xb1,0x02,0x00,
								 0x04, 0x00, 0x10, 0x00, 0x00, 0x00, 0x64, 0x61,		0x74,0x61,0x00,0xf4,0x27,0x02,0x00,0x00 };

	this->WriteData(0,data,16*3);

}



//***************************************************************************
int CMemoryBufferCache::mCacheId =0;

CMemoryBufferCache::CMemoryBufferCache(__int64 total_size, int cache_size) :
   CFileBufferCache(CreateFileCache(total_size), cache_size)
{
	   mCacheId++;
}

//***************************************************************************
CMemoryBufferCache::~CMemoryBufferCache()
{
}

//***************************************************************************
std::string CMemoryBufferCache::CreateFileCache(__int64 total_size)
{
	mAbort=false;

	char name[512];
	if (g_output_dir!=NULL)
	{
		sprintf(name,"%s\\cache%d",g_output_dir,mCacheId);
	}
	else
	{
		sprintf(name,"c:\\cache%d",mCacheId);
	}
	int b_s = 1024*128;

	unsigned char*m = new unsigned char[b_s];
	memset(m,0,b_s);

	FILE* fp = fopen(name,"wb");
	if (fp==NULL)
	{
		Error("Unable to create memory file buffer '%s'", name);
		return std::string(name);
	}

	while (total_size > b_s && mAbort==false)
	{
		fwrite(m,1,b_s,fp);
		total_size -= b_s;
	}

	if (total_size >0 && mAbort==false)
	{
		fwrite(m,1,(int)total_size,fp);
	}
	fclose(fp);
	delete [] m;
	return std::string(name);
}


