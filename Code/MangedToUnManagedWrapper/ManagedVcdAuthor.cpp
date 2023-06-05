

extern "C"
{
//#include "../vcdimager-0.7.22/vcdimagerapi.h"
}


char* g_my_vcd_xml_buffer;
char* g_my_vcd_output_folder;


void create_vcd_string_buffer(int size)
{
	g_my_vcd_xml_buffer = new char[size] ;
	g_my_vcd_xml_buffer[0]=0;
	g_my_vcd_xml_buffer[size-1] =0;
}


void delete_vcd_string_buffer()
{
	delete [] g_my_vcd_xml_buffer;
	g_my_vcd_xml_buffer=0;
}

void do_vcdauthor()
{
	// SRG old, no longer supported
	// 
	//srg_build_vcd_structure(g_my_vcd_xml_buffer, g_my_vcd_output_folder);
}
    

#include <iostream>
#include <string>
using namespace std; 
#using <mscorlib.dll>
#using <System.Drawing.dll>


#include <tchar.h>

using namespace System;
using namespace Drawing;
using namespace Imaging;


using namespace System::Text;
using namespace System::Runtime::InteropServices; 


extern std::string ManagedToSTL(System::String^ managed);

namespace MangedToUnManagedWrapper
{ 
  
	public ref class VCDAuthor
	{
	public:
	     
		// create vcd/svcd file/folder structure 
		static void AuthorVCD(String^ xml,String^ output_directory)
		{ 
			if (xml!=nullptr)
			{ 
				std::string string = ManagedToSTL(xml);
				create_vcd_string_buffer(strlen((char*) string.c_str())+1);
				strcpy(g_my_vcd_xml_buffer ,(char*) string.c_str());
			}
			else
			{  
				create_vcd_string_buffer(1);
				g_my_vcd_xml_buffer[0]=0;
			}
	 
			std::string output_dir_s = ManagedToSTL(output_directory);
			g_my_vcd_output_folder = (char*) output_dir_s.c_str();

			do_vcdauthor();

			delete_vcd_string_buffer();
		}
	};
}






