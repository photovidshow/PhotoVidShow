#include "FileBufferCache.h"
#include "ManagedErrors.h"

#include <iostream>
#include <string>
using namespace std; 
#using <mscorlib.dll>


#include <tchar.h>

using namespace System;
using namespace System::Text;
using namespace System::Runtime::InteropServices; 
using namespace ManagedCore;
using namespace std;


extern std::string ManagedToSTL(String^ a);
extern String^ STLToManaged(std::string);

#include "ManagedObject.h"
#include "ManagedFileBufferCache.h"


namespace MangedToUnManagedWrapper
{ 

	CManagedBuffer::CManagedBuffer() : mBuffer(NULL)
	{
	}

	CManagedBuffer::~CManagedBuffer()
	{
		CleanUp();
		
	}

	void CManagedBuffer::CleanUp()
	{
		if (mBuffer!=NULL)
		{
			delete mBuffer;
			mBuffer=NULL;
		}
	}

	public ref class CManagedFileBufferCache : public CManagedBuffer
	{
	public:
	
		CManagedFileBufferCache()
		{
		}


		CManagedFileBufferCache(String^ filename)
		{
			std::string mName = ManagedToSTL(filename);
			mBuffer = new CFileBufferCache((char*)mName.c_str());
		}

		void SaveToFile(String^ filename, int skip_initial_bytes)
		{
			if (mBuffer==NULL)
			{
				ManagedError("CManagedFileBufferCache::SaveToFile Buffer=NULL");
				return ;
			}

			std::string mName = ManagedToSTL(filename);
			((CFileBufferCache*)mBuffer)->SaveToFile((char*)mName.c_str(), skip_initial_bytes);
		}

		void SaveToFile(String^ filename)
		{
			if (mBuffer==NULL)
			{
				ManagedError("CManagedFileBufferCache::SaveToFile Buffer=NULL");
				return ;
			}

			std::string mName = ManagedToSTL(filename);
			((CFileBufferCache*)mBuffer)->SaveToFile((char*)mName.c_str(), 0);
		}

		String^ GetFilename() { return STLToManaged(((CFileBufferCache*)mBuffer)->GetFilename()) ; }
		
		void WriteHackWavHead()
		{
			if (mBuffer==NULL)
			{
				ManagedError("CManagedFileBufferCache::WriteHackWavHead Buffer=NULL");
				return ;
			}

			((CFileBufferCache*)mBuffer)->WriteHackWavHead();
		}

		void WriteBlank(__int64 offset, __int64 amount)
		{
			if (mBuffer==NULL)
			{
				ManagedError("CManagedFileBufferCache::WriteBlank Buffer=NULL");
				return ;
			}
			((CFileBufferCache*)mBuffer)->WriteBlank(offset,amount); 
		}


		__int64 GetSize() 
		{ 
			if (mBuffer==NULL)
			{
				ManagedError("CManagedFileBufferCache::GetSize Buffer=NULL");
				return 0;
			}

			return ((CFileBufferCache*)mBuffer)->GetSize(); 
		}

		int GetCacheSize() 
		{ 
			if (mBuffer==NULL)
			{
				ManagedError("CManagedFileBufferCache::GetCahceSize Buffer=NULL");
				return 0;
			}

			return ((CFileBufferCache*)mBuffer)->GetCacheSize(); 
		}

		int ReadByte(__int64 offset)
		{
			if (mBuffer==NULL)
			{
				ManagedError("CManagedFileBufferCache::ReadByte Buffer=NULL");
				return 0;
			}

			return ((CFileBufferCache*)mBuffer)->ReadByte(offset); 
		}
	};

	public ref class CManagedMemoryBufferCache : public CManagedFileBufferCache
	{
	public:

		CManagedMemoryBufferCache()
		{
		}

		static void set_temp_directory(String^ output_dir)
		{
			std::string output_dir_s = ManagedToSTL(output_dir);
			g_output_dir = new char[output_dir_s.length()+1];
			strcpy(g_output_dir,(char*) output_dir_s.c_str());
		}


		CManagedMemoryBufferCache(__int64 total_size, int cache_size)
		{
			mBuffer = new CMemoryBufferCache(total_size,cache_size);
		}

		CManagedMemoryBufferCache(CMemoryBufferCache* in)
		{
			mBuffer =in;
		}

		void WriteBuffer(CManagedBuffer^ buffer, __int64 start, __int64 input_buffer_index_start, __int64 amount)
		{
			if (mBuffer==NULL)
			{
				ManagedError("CManagedMemoryBufferCache::WriteBuffer Buffer=NULL");
				return ;
			}

			((CFileBufferCache*)mBuffer)->WriteBuffer(buffer->mBuffer, start, input_buffer_index_start, amount);
		}

		static void Abort()
		{
			CFileBufferCache::mAbort=true;
		}

	};
}



