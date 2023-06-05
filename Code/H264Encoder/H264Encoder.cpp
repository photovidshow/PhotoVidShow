

#include "Input/Photovidshow.h"
extern int original_x264_main(int argc, char **argv) ; 

// quality 0 = low
// quality 1 = good
// quality 2 = high
// quality 3 = maximum


// fps 0 = 12
// fps 1 = 15
// fps 2 = 20
// fps 3 = 24
// fps 4 = 25
// fps 5 = 30
// fps 6 = 50
// fps 7 = 60
// fps 8 = 23.976
// fps 9 = 59.94


int do_mpeg4_h264_encode(int width, int height, int ifps, int quality, getVideoFptr getVideo, int length, int bluRay, char* outputFilename)
{
	char* i0 = "prog";
	char* i1 = "photovidshow.pvs";


	char* i2 = "--preset";
	char* i3 = "veryslow";

	//char* i2 = "--no-8x8dct";
//	char* i3 = "--aq-mode";
	char* i4 = "0";
	char* i5 = "--b-adapt";
	char* i6 = "0";  
	char* i7 = "--bframes";			
	char* i8 = "0";

	// this option means there is only 1 frame delay on video output (pts time) 
	// else there is a 2 frame delay as --bframes = 3 by default and pyrmid means b frames can be referenced by other frames
	char* i8a = "--b-pyramid";		
	char* i8b = "none";

	char* i9 = "--no-cabac";
	char* i10 = "--no-deblock";
	 char* i11 ="--no-mbtree";
	 char* i12 ="--me";
	 char* i13 ="dia";
	 char* i14 ="--no-mixed-refs";
	 char* i15 ="--partitions";
	 char* i16 ="none";


	// Reduces memory usage heavly, reduces video size, but more blocking between different images

	 // SRG this probably can be higher at lower resolutions to reduce size
	char* i17 ="--rc-lookahead";	
    char* i18 ="1";

	
     // needed as bug in encode software ( stops video getting currupt in places)
	 char* i19 ="--ref";
	 char* i20 ="1";


	 char* i21 ="--scenecut";
	 char* i22 ="0";

	  char* i23 = "--crf";
	  char* i24 = "23";
	  char* i24b = "19";
      char* i24c = "15";
      char* i24d = "29";

	       // This is needed for blu-ray
      //
      char* i25 = "--bluray-compat";

	 // fixed

	/*
	\n"
        "                                  --bframes 0 --no-cabac --no-deblock\n"
        "                                  --no-mbtree --me dia --no-mixed-refs\n"

		   --partitions none --rc-lookahead 0 --ref 1\n"
        "                                  --scenecut 0 --subme 0 --trellis 0\n"
        "                                  --no-weightb --weightp 0\n"



*/

	// 23.976 and 29.97  do this,   output mp4 is corrupt though fix
	// "24000/1001" "30000/1001"


	char* fps = "--fps";
	char* fpsnum = "12";
	char* fpsnum1 = "15";
	char* fpsnum2 = "20";
	char* fpsnum3 = "24";
	char* fpsnum4 = "25";
	char* fpsnum5 = "30";
	char* fpsnum6 = "50";
	char* fpsnum7 = "60";
    char* fpsnum8 = "23.976";
    char* fpsnum9 = "59.94";



	char* iout = "-o";
	char* iname = outputFilename;
	//char *argv2[] = {i0,i1,i2,i3, i4, i5,i6,i7,i8, i9, i10, i11,i12,i13,i14,i15,i16, i17,i18, i19,i20, iout, iname };
    char *argv2[] = {i0,i1,i8a,i8b,i17,i18, i19,i20, i23,i24, fps, fpsnum, iout, iname };		// SRG turned off b-prymids to prevent 2 frame delay
    char *argv2Bluray[] = {i0,i1, i17,i18, i19,i20, i23,i24, fps, fpsnum, i25, iout, iname };
    int argc2 =14;
    int argc2Bluray =13;

	if (quality==2)
	{
		i24 = i24b;
	}
	if (quality==3)
	{
		i24 = i24c;
	}
    if (quality==0)
	{
		i24 = i24d;
	}

	if (ifps==1)
	{
		fpsnum = fpsnum1;
	}
	if (ifps==2)
	{
		fpsnum = fpsnum2;
	}
	if (ifps==3)
	{
		fpsnum = fpsnum3;
	}
	if (ifps==4)
	{
		fpsnum = fpsnum4;
	}
	if (ifps==5)
	{
		fpsnum = fpsnum5;
	}
	if (ifps==6)
	{
		fpsnum = fpsnum6;
	}
	if (ifps==7)
	{
		fpsnum = fpsnum7;
	}
    if (ifps==8)
	{
		fpsnum = fpsnum8;
	}
    if (ifps==9)
	{
		fpsnum = fpsnum9;
	}

	// reset quality and fps
	argv2[9] = i24;
	argv2[11] = fpsnum;
    argv2Bluray[7] = i24;
	argv2Bluray[9] = fpsnum;

	photovidshow_set_input_parameters(getVideo, width, height, length);

    if (bluRay==0)
    {
	    return original_x264_main( argc2, argv2 );
    }
    else
    {
        return original_x264_main( argc2Bluray, argv2Bluray );
    }

}
