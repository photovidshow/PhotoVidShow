// link to dvd author library


#include <iostream>
#include <string>
using namespace std; 
#using <mscorlib.dll>
#using <System.Drawing.dll>



#include "../dvd-author/dvdauthor-0.6.10/src/dvdauthor.h"
#include <iostream>

char* g_my_xml_buffer;


char* g_input_file;
char* g_output_file;
void* g_main_image;
void* g_high_image;
void* g_select_image;




static struct centre_button_buttons g_button_positions;

using namespace std;

void create_dvd_author_string_buffer(int size, char* temp)
{
	//cout <<"Creating create_dvd_author_string_buffer buffer size "<< size << endl;
	//cout <<"copy from  "<< temp << endl;
	g_my_xml_buffer = new char[size] ;
	g_my_xml_buffer[0]=0;
	g_my_xml_buffer[size-1] =0;
	strcpy(g_my_xml_buffer, temp);
}


void delete_dvd_author_string_buffer()
{
	//cout <<"deleting create_dvd_author_string_buffer buffer size " << endl;
	delete [] g_my_xml_buffer;
	g_my_xml_buffer=0;
}



/*

<subpictures>
   <stream>
      <spu start="start-time" [ end="end-time" ] [ image="picture.png" ]
           [ highlight="picture.png" ] [ select="picture.png" ]
           [ transparent="color-code" ] [ force="yes" ]
           [ autooutline="infer" [ outlinewidth="width" ]
             [ autoorder="rows" | autoorder="columns" ] ]
           [ xoffset="x-coord" yoffset="y-coord" ] >
         <button [ name="name" ] [ x0="x0" ] [ y0="y0" ] [ x1="x1" ]
                 [ y1="y1" ] [ up="name" ] [ down="name" ]
                 [ left="name" ] [ right="name" ] />
         <action [ name="name" ] />
      </spu>
   </stream>
</subpictures>
*/

// mode 0 = dvd,  1 = vcd,  2 = svcd  
void do_submux(int mode, int width, int height)
{
	//std::cout << "Entered submux"<<std::endl;

//	std::cout << "Entered submux2"<<std::endl;
//	std::cout << "g_input_file = "<<g_input_file<<std::endl;;;
//	std::cout << "g_output_file = "<<g_output_file<<std::endl;;;

//	std::cout << "buffer = "<<g_my_xml_buffer;

 
//	std::cout << "Entered submux3"<<std::endl;



	g_button_positions.num_buttons=0;

	subuxmain(g_my_xml_buffer,g_input_file, g_output_file, (char*)g_main_image, (char*)g_high_image, (char*)g_select_image, mode, width, height, &g_button_positions);
}


void do_dvdauthor()
{
	createdvdfiles(g_my_xml_buffer,g_output_file);
}




#include <tchar.h>

using namespace System;
using namespace Drawing;
using namespace Imaging;
using namespace ManagedCore;


using namespace System::Text;
using namespace System::Runtime::InteropServices; 



extern std::string ManagedToSTL(System::String^ managed);

namespace MangedToUnManagedWrapper
{
	public ref class DVDAuthor
	{
	public:
	 
		// add subtitles to a mpeg2 movie (i.e. menus buittons etc)

		static void AddSubtitleToVideo(String^ xml,
			                           String^ input_file,
									   String^ output_file,
									   Bitmap^ main_image,
									   Bitmap^ high_image,
									   Bitmap^ select_image,
									   int the_mode,
									   int width,
									   int height,
									   SubtitleButtonPositions^ mysubpos)
		{
			Drawing::Rectangle r = Drawing::Rectangle(0,0,main_image->Width,main_image->Height);
			BitmapData^ bmpDataMain = main_image->LockBits(r,ImageLockMode::ReadOnly, main_image->PixelFormat);
			g_main_image = (void*) bmpDataMain->Scan0;

			BitmapData^ bmpDataHigh = high_image->LockBits(r,ImageLockMode::ReadOnly, high_image->PixelFormat);
			g_high_image = (void*) bmpDataHigh->Scan0;

			BitmapData^ bmpDataSelect = select_image->LockBits(r,ImageLockMode::ReadOnly, select_image->PixelFormat);
			g_select_image = (void*) bmpDataSelect->Scan0;
		  
			std::string string = ManagedToSTL(xml);

			create_dvd_author_string_buffer(string.length()+1, (char*) string.c_str());
			std::string input_file_s = ManagedToSTL(input_file);
			g_input_file = new char[input_file_s.length()+1];
			strcpy(g_input_file,(char*) input_file_s.c_str());


			std::string output_file_s = ManagedToSTL(output_file);
			g_output_file = new char[output_file_s.length()+1];
			strcpy(g_output_file,(char*) output_file_s.c_str());

			do_submux(the_mode,  width,  height);

			for (int ii=0;ii<g_button_positions.num_buttons;ii++)
			{
				mysubpos->AddPair(g_button_positions.x0[ii], g_button_positions.y0[ii]);
			}

			main_image->UnlockBits(bmpDataMain);
			high_image->UnlockBits(bmpDataHigh);
			select_image->UnlockBits(bmpDataSelect);

			delete_dvd_author_string_buffer();
			delete [] g_input_file;
			g_input_file=NULL;
			delete [] g_output_file;
			g_output_file=NULL;
		}

		// create dvd file/folder structure i.e vobs, ifo, etc 
		static void AuthorDVD(String^ xml,String^ output_directory)
		{
			std::string string = ManagedToSTL(xml);
	
	
		//	g_my_xml_buffer = new char[4*1024];
		//	strcpy(g_my_xml_buffer,  (char*) string.c_str());

			create_dvd_author_string_buffer(string.length()+1, (char*) string.c_str());
			
			std::string output_file_s = ManagedToSTL(output_directory);
			g_output_file = (char*) output_file_s.c_str();

			do_dvdauthor();

			delete_dvd_author_string_buffer();
		}
	};
}




#pragma unmanaged

