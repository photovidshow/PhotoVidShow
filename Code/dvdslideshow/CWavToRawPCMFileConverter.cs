using System;
using ManagedCore;
using System.Collections;
using MangedToUnManagedWrapper;
using System.Xml;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for CWavToRawPCMFileConverter.
	/// </summary>
	public class CWavToRawPCMFileConverter
	{

		static public int GetSizeOfHeader(string source_name)
		{
			return GetSizeOfHeader(source_name,null);
		}

		static public int GetSizeOfHeader(string source_name, CManagedFileBufferCache use_cache)
		{
			CManagedFileBufferCache a = use_cache;
			
			bool had_to_open_internal=false;
			
			if (a==null)
			{
				a= new CManagedFileBufferCache(source_name);
				had_to_open_internal=true;
			}

			if (a.GetSize() <=0)
			{
				if (had_to_open_internal==true)
				{
					a.Dispose();
				}
				return 0;
			}

			// find size of header

			int i=0;
			bool found = false;
			int header_size=0;

			// check first 100 bytes then give up

			while (found==false && i<100)
			{
				if (a.ReadByte(i) == 0x64 &&
					a.ReadByte(i+1) == 0x61 &&
					a.ReadByte(i+2) == 0x74 &&
					a.ReadByte(i+3) == 0x61)
				{
					found = true;
				}
				else
				{
					i++;
				}
			}

			if (found==true)
			{
				header_size = i+8;
			}

			if (had_to_open_internal==true)
			{
				a.Dispose();
			}

			return header_size;
		}


		static public void Convert(string source_name, string dest_name)
		{
			CManagedFileBufferCache a= new CManagedFileBufferCache(source_name);
			int header_size = GetSizeOfHeader(source_name, a);
			a.SaveToFile(dest_name, header_size);
			a.Dispose();
		}

	}
}
