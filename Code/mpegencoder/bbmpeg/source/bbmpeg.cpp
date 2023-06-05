//
// bbmpeg.cpp, by Brent Beyeler
//
// Outputs video and audio to MPEG files

extern void DebugOutbutMessageToFile(char *fmt, ...);

#if defined(__BORLANDC__)
#include <condefs.h>
USEUNIT("gui.cpp");
USEUNIT("settings.cpp");
USEUNIT("systems.cpp");
USEUNIT("multplex.cpp");
USEUNIT("interact.cpp");
USEUNIT("inptstrm.cpp");
USEUNIT("inits.cpp");
USEUNIT("domplex.cpp");
USEUNIT("timecode.cpp");
USEUNIT("common.cpp");
USEUNIT("tonal.cpp");
USEUNIT("subs.cpp");
USEUNIT("encode.cpp");
USEUNIT("psy.cpp");
USEUNIT("dovideo.cpp");
USEUNIT("conform.cpp");
USEUNIT("stats.cpp");
USEUNIT("readpic.cpp");
USEUNIT("ratectl.cpp");
USEUNIT("quantize.cpp");
USEUNIT("putvlc.cpp");
USEUNIT("putpic.cpp");
USEUNIT("putmpg.cpp");
USEUNIT("puthdr.cpp");
USEUNIT("predict.cpp");
USEUNIT("motion.cpp");
USEUNIT("idct.cpp");
USEUNIT("fdctref.cpp");
USEUNIT("transfrm.cpp");
USEUNIT("bits.cpp");
USEUNIT("buffer.cpp");
USEUNIT("doaudio.cpp");
USEUNIT("mmxasm.cpp");
USEUNIT("idctmm32.cpp");
USEUNIT("fdctmm32.cpp");
USEUNIT("fdctam32.cpp");
USEUNIT("Quantasm.cpp");
USEDEF("..\res\bbmpeg_bor.def");
USERC("..\res\bbmpeg.rc");
//---------------------------------------------------------------------------
#endif


#define MPEGMAIN
#include "gui.h"
#include "main.h"
#include "consts.h"
#undef MPEGMAIN

#define MPEGtype	0x626D7067L		// 'bmpg'

//----------------------------------------------------------
// Adobe API Prototypes

int bbMPEGStartup(compInfoRec *infoRec, int pal, int widescreen, int disk_type, bool encode_audio, bool do_3_2_pulldown);
int bbMPEGGetFilePrefs(compGetFilePrefsRec *filePrefsRec);
int bbMPEGGetIndFormat(compFileInfoRec *fileInfoRec, int idx);
int bbMPEGGetAudioIndFormat(compAudioInfoRec *audioInfoRec, int idx);
int bbMPEGDoCompile(compStdParms *stdParms, compDoCompileInfo *doCompileRec);
int bbMPEGQueryOutputFormat(compOutputRec *outputRec);
void bbDoShutdown();

// general API routines

// disk type
// 0 = dvd
// 1 = svcd
// 2 = vcd

int MPEGEncoderInit(int pal, int widescreen, int disk_type, bool encode_audio, bool do_3_2_pulldown)
{
  compInfoRec infoRec;

  UseAdobeAPI = false;
  return bbMPEGStartup(&infoRec, pal, widescreen, disk_type, encode_audio, do_3_2_pulldown);
}

int MPEGEncoderStart(makeMPEGRecInfo *makeInfo)
{
 // sxf = syf = sxb = syb =0;
 int ret;

  
#ifdef _DEBUG
  DebugOutbutMessageToFile("makeInfo do audio = %d\n",makeInfo->doAudio);
  fprintf(stdout,"Testing 4567\n");
  fprintf(stderr,"Testing 3456\n");
  OutputDebugString("Testing 789\n");
  DebugOutbutMessageToFile("encoder : called bbMPEGMakeMPEG\n");

#endif

  compStdParms stdParms;
  compDoCompileInfo doCompileInfo;
  compOutputRec outputRec;
  
  makeMPEGInfo = *makeInfo;
  outputRec.doVideo = makeInfo->doVideo;
  outputRec.width = makeInfo->width;
  outputRec.height = makeInfo->height;
  doCompileInfo.startFrame = makeInfo->startFrame;
  doCompileInfo.endFrame = makeInfo->endFrame;
  outputRec.doAudio = makeInfo->doAudio;
  outputRec.audrate = makeInfo->audioRate;
  outputRec.stereo = makeInfo->stereo;
  outputRec.audsamplesize = makeInfo->sampleSize;
  strcpy(doCompileInfo.outputFile.name, makeInfo->outputFilename);
  doCompileInfo.outputFileRef = NULL;

  doCompileInfo.outputRec = outputRec;
 // DebugOutbutMessageToFile("encoder started compile\n");
  ret = bbMPEGDoCompile(&stdParms, &doCompileInfo);
  if (ret == comp_CompileDone)
    return bbMakeDone;
  else
    if (ret == comp_CompileAbort)
      return bbMakeAbort;
  return ret;
}

void MPEGEncoderShutdown()
{
  bbDoShutdown();
}


void SetDefaultMMXMode()
{
  MMXMode = MODE_NONE;
  if (MMXAvail & FEATURE_MMXEXT)
    MMXMode = MODE_SSE;
  else
    if (MMXAvail & FEATURE_MMX)
      MMXMode = MODE_MMX;
  if (MMXAvail & FEATURE_3DNOWEXT)
    MMXMode = MODE_3DNOWEXT;
  else
    if (MMXAvail & FEATURE_3DNOW)
      MMXMode = MODE_3DNOW;
}


//---------------------------------------------------------
// Process compStartup
//
// Specify attributes for this compiler

// disk type
// 0 = dvd
// 1 = svcd
// 2 = vcd

int bbMPEGStartup(compInfoRec *infoRec, int pal, int widescreen, int disk_type, bool encode_audio, bool do_3_2_pulldown)
{
  char tmpStr[MAXPATH];

  MMXAvail = get_feature_flags();
  SetDefaultMMXMode();
  strcpy(AppName, "");
  strcpy(Version, "");
  strcpy(About, AppName);
  strcat(About, "");
  strcat(About, Version);
  strcat(About, "");
  strcpy(ParamFilename, "");
  strcpy(VideoFilename, "");
  strcpy(AudioFilename, "");
  strcpy(Audio1Filename, "");
  strcpy(DefaultExt, ".mpg");
  infoRec->fileType = MPEGtype;
  infoRec->hasSetup = 1;
  infoRec->compilesVideo = 1;
  infoRec->compilesAudio = 1;
  infoRec->hasAllocator = 1;
  infoRec->canCopyFrames = 0;
  infoRec->canOpen = 0;
  infoRec->singleFrame = 0;
  strcpy(infoRec->compilerName, "");
  audioStereo = 1;
  horizontal_size = 0;
  vertical_size = 0;
//  PALDefaults = ::GetProfileInt(AppName, "PALDefaults", 0);
 // BatchMode = ::GetProfileInt(AppName, "Batch", 0);
 // MMXMode = ::GetProfileInt(AppName, "MMXMode", MMXMode);
 // SetMPEG1Defaults(&tempSettings1, PALDefaults);
  // see if the default.ini file present
//  strcpy(tmpStr, StartupDir);
 // strcat(tmpStr, "default.ini");
 // if (ReadSettings(tmpStr, &tempSettings1))
  //{
  //  if ((!CheckVideoSettings(&tempSettings1)) || (!CheckAudioSettings(&tempSettings1)))
   //   SetMPEG1Defaults(&tempSettings1, PALDefaults);
 // }
 // else
 //   SetMPEG1Defaults(&tempSettings1, PALDefaults);

  // pal 1 // nsct 0
  if (disk_type==0)
  {
	 SetDVDDefaults(&tempSettings1, pal, widescreen, encode_audio, do_3_2_pulldown);
	 SetDVDMplex(&tempSettings1);
  }
  if (disk_type==1)
  { 
	 SetSVCDDefaults(&tempSettings1, pal, widescreen, encode_audio, do_3_2_pulldown);
	 SetSVCDMplex(&tempSettings1);
  }
  if (disk_type==2)
  {
#ifdef _DEBUG
	 DebugOutbutMessageToFile("ERR: encoder Setting disk type to vcd \n");
#endif

	 SetVCDDefaults(&tempSettings1, pal, widescreen, encode_audio);
	 SetVCDMplex(&tempSettings1);
  }

#ifdef _DEBUG
  DebugOutbutMessageToFile("just called start up \n");
#endif
  GetTempSettings(&tempSettings1);
  return comp_ErrNone;
}

//---------------------------------------------------------
// Process compGetFilePrefs
//
// This is called to setup compiler options

int bbMPEGGetFilePrefs(compGetFilePrefsRec *filePrefsRec, int initPage)
{
  AudioAvail = true;
  VideoAvail = true;
  vVideoCDAvail = true;
  aVideoCDAvail = true;
  vSVCDAvail = true;
  aSVCDAvail = true;
  vDVDAvail = true;
  aDVDAvail = true;
  input_horizontal_size = horizontal_size = 352;
  input_vertical_size = vertical_size = 240;
  DoSettings(NULL, initPage);
  return comp_ErrNone;
}


//---------------------------------------------------------
// Process compGetIndFormat
//
// This is called iteratively to specify what subtypes the compiler
// supports.

int bbMPEGGetIndFormat(compFileInfoRec *fileInfoRec, int idx)
{
  int retval = comp_ErrNone;

  switch (idx)
  {
    case 0:
      fileInfoRec->subtype = compUncompressed;
      fileInfoRec->depthsSupported = compDepth24;
      strcpy(fileInfoRec->name, "");
      fileInfoRec->canForceKeyframes = 0;
      fileInfoRec->frameMultiple = 1;
      fileInfoRec->minWidth = 1;
      fileInfoRec->minHeight = 1;
      fileInfoRec->maxWidth = 1920;
      fileInfoRec->maxHeight = 1152;
      fileInfoRec->canDoFields = 0;
      fileInfoRec->canDoQuality = 0;
      fileInfoRec->canSetDataRate = 0;
      fileInfoRec->hasSetup = 1;
      fileInfoRec->canDelta = 0;
      fileInfoRec->fixedTimebase = 0;
      fileInfoRec->fixedScale = 0;
      fileInfoRec->fixedSampleSize = 0;
      fileInfoRec->defaultQuality = 0;
      break;

    default:
      retval = comp_BadFormatIndex;
  }
  return retval;
}


//--------------------------
//	compMPEGGetAudioIndFormat
//
//	return the name of supported audio compressors

int bbMPEGGetAudioIndFormat(compStdParms *stdParms, compAudioInfoRec *audioInfoRec, int idx)
{
  int retval = comp_ErrNone;
  compAudioFormatPtr ptr;

  switch (idx)
  {
    case 0:
      ptr = (compAudioFormatPtr) stdParms->funcs->memoryFuncs->newPtr(sizeof(compAudioFormat) * 4);
      if (!ptr)
        retval = comp_BadFormatIndex;
      else
      {
	audioInfoRec->subtype = compUncompressed;
        audioInfoRec->hasSetup = 1;
        strcpy(audioInfoRec->name, "");
        ptr[0].audioDepths = comp16Mono | comp16Stereo;
        ptr[0].audioRate = 0;
        ptr[1].audioDepths = comp16Mono | comp16Stereo;
        ptr[1].audioRate = 32000;
        ptr[2].audioDepths = comp16Mono | comp16Stereo;
        ptr[2].audioRate = 44100;
        ptr[3].audioDepths = comp16Mono | comp16Stereo;
        ptr[3].audioRate = 48000;
        audioInfoRec->audioFormats = ptr;
      }
      break;

    default:
      retval = comp_BadFormatIndex;
  }
  return retval;
}


//---------------------------------------------------------
// Process compDoCompile
//
// Do the compile. Output file and format are in compDoCompileInfo.

int bbMPEGDoCompile(compStdParms *stdParms, compDoCompileInfo *doCompileRec)
{
  stdParmsSave = stdParms;

#ifdef _DEBUG
  DebugOutbutMessageToFile("bbMPEGDoCompile doCompileRec do audio = %d\n",doCompileRec->outputRec.doAudio);
#endif
  doCompileRecSave = doCompileRec;

 // return 1;

  return startGui();
}


//---------------------------------------------------------
// Process compQueryOutputFormat
//
// Examine compOutputRec and determine if this compiler can
// output to the requested format.
// Return either comp_OutputFormatAccept or comp_OutputFormatDecline

int bbMPEGQueryOutputFormat(compOutputRec *outputRec)
{
  if (outputRec->doVideo)
  {
    if ((outputRec->width < 1) ||
        (outputRec->height < 1) ||
        (outputRec->width > 1920) ||
        (outputRec->height > 1152))
      return comp_OutputFormatDecline;

    if ((outputRec->width % 2 != 0) ||
        (outputRec->height % 2 != 0))
      return comp_OutputFormatDecline;
  }

  if (outputRec->doAudio)
  {
    if ((outputRec->audrate != 32000) &&
        (outputRec->audrate != 44100) &&
        (outputRec->audrate != 48000))
      return comp_OutputFormatDecline;

    if (outputRec->audsamplesize != 16)
      return comp_OutputFormatDecline;
  }

  return comp_OutputFormatAccept;
}


void bbDoShutdown()
{
  char tmpStr[10];

  //sprintf(tmpStr, "%d", BatchMode);
 // WriteProfileString(AppName, "Batch", tmpStr);
}

//---------------------------------------------------------
// Compile module entry point

PREMPLUGENTRY xCompileEntry (int selector, compStdParms *stdParms, long param1, long param2)
{
	/*
  int result = comp_ErrNone;

  switch (selector)
  {
    case compStartup:
      UseAdobeAPI = true;
      result = bbMPEGStartup((compInfoRec *) param1);
      break;

    case compDoShutdown:
      bbDoShutdown();
      break;

    case compGetFilePrefs:
      result = bbMPEGGetFilePrefs((compGetFilePrefsRec *) param1, 1);
      break;

    case compGetSubTypePrefs:
      result = bbMPEGGetFilePrefs((compGetFilePrefsRec *) param1, 2);
      break;

    case compGetAudSubTypePrefs:
      result = bbMPEGGetFilePrefs((compGetFilePrefsRec *) param1, 3);
      break;

    case compGetIndFormat:
      result = bbMPEGGetIndFormat((compFileInfoRec *) param1, param2);
      break;

    case compGetAudioIndFormat:
      result = bbMPEGGetAudioIndFormat(stdParms, (compAudioInfoRec *) param1, param2);
      break;

    case compQueryOutputFormat:
      result = bbMPEGQueryOutputFormat((compOutputRec *) param1);
      break;

    case compDoCompile:
      result = bbMPEGDoCompile(stdParms, (compDoCompileInfo *) param1);
      break;
  }
*/
  return 0;
}

static int doInit = 0;


//#ifdef __BORLANDC__
//BOOL WINAPI DllEntryPoint(HINSTANCE instance, uint32 /*flag*/, LPVOID)
//#else
//BOOL WINAPI DllMain(HINSTANCE instance, UINT32 /*flag*/, LPVOID)
//#endif

/*
{
  char *tmpPtr;

  if (!doInit)
  {
    if (!InitSettings(instance))
        return FALSE;
    if (!GetModuleFileName(instance, StartupDir, MAXPATH))
      return FALSE;
    tmpPtr = strrchr(StartupDir, '\\');
    if (tmpPtr)
      tmpPtr[1] = 0;
    else
      strcat(StartupDir, "\\");
    strcpy(HelpFileName, StartupDir);
    strcat(HelpFileName, "bbmpeg.hlp");
	appWindow = NULL;
    doInit = 1;
  }
  return TRUE;
}


#define DllEntryPoint
*/

