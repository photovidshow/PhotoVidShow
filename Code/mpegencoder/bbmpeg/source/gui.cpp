/* gui.cpp */

#define _CRT_SECURE_NO_WARNINGS 1
#include "stdarg.h"
#include <stdio.h>
#include <stdlib.h>
#include <assert.h>
#include <ctype.h>
 

#include <assert.h>
#include <errno.h>
#include <fcntl.h>
#include <string.h>
#include <stdlib.h>
#include <stdio.h>
#include "windows.h"

int log_opened =0;


// ******************************************************************************************
void DebugOutbutMessageToFile(char *fmt, ...)
{
	/*
#ifdef _DEBUG
	if ( fmt )
	{
	

		char buffer[1024] ;
		va_list arglist;
		va_start(arglist, fmt);
		vsprintf(buffer, fmt, arglist);
		va_end(arglist);
	//	ERROR_MANAGER.Warning(buffer, (DWORD)fmt);
	
	//	System::Console::WriteLine(buffer);

		fprintf(stderr,"%s\n",buffer);

		OutputDebugString(buffer);

		FILE* fp = NULL;

		if (log_opened==0)
		{
			fp = fopen ("c:\\dev\\dvdslideshow\\debug.log","w");
			log_opened=1;
		}
		else
		{
			fp = fopen ("c:\\dev\\dvdslideshow\\debug.log","a");
		}

		if (fp)
		{
			fprintf(fp,buffer);
			fclose(fp);
		}
	}
#endif
	*/


}


#include "gui.h"
#include <stdio.h>

 char HelpFileName[MAXPATH];
 char AppName[MAXPATH];
 char About[MAXPATH];
 char Version[20];
 char StartupDir[MAXPATH];
 char DefaultExt[5];
 char ParamFilename[MAXPATH];


void bbMPEGAppWnd(HWND appWnd)
{
  //appWindow = appWnd;
}

void bbMPEGSetFocus()
{
 // if (MPEGDialog)
  //  MPEGDialog->SetFocus();
}




#include "main.h"
#include <time.h>

/*

//extern void     _OWLFUNC SetCreationWindow(TWindow*);
//extern TWindow* _OWLFUNC GetCreationWindow();

// #define TESTING

static int exit_code;
static int Encoding;

*/

#ifdef _DEBUG
void DisplayError(char *txt)
{
  char tmpStr[30];

  sprintf(tmpStr, "%s - error", AppName);
//  if (MPEGDialog)
  //  MPEGDialog->MessageBox(txt, tmpStr, MB_OK | MB_ICONERROR);
 // else
    ::MessageBox(NULL, txt, tmpStr, MB_OK | MB_ICONERROR);
}

void DisplayWarning(char *txt)
{
  char tmpStr[80];

 // if (MPEGDialog)
  //  MPEGDialog->AddMessage(txt);
 // else
  {
    sprintf(tmpStr, "%s - warning", AppName);
    ::MessageBox(NULL, txt, tmpStr, MB_OK | MB_ICONWARNING);
  }
}

#endif



void YieldTime()
{
 // if (MPEGDialog)
   // MPEGDialog->YieldTime();
}

#ifdef _DEBUG
void DisplayInfo(char *txt)
{

	DebugOutbutMessageToFile(txt);
	DebugOutbutMessageToFile("\n");


 // if (MPEGDialog)
   // MPEGDialog->AddMessage(txt);
}


void DisplayProgress(char *txt, int percent)
{
//  if (MPEGDialog)
  //{
  //  MPEGDialog->progressText->SetText(txt);
   // MPEGDialog->progressBar->SetValue(percent);
 // }
}
#endif
/*

DEFINE_RESPONSE_TABLE1(TMPEGDialog, TDialog)
  EV_COMMAND(IDC_CANCELBUTTON, CmCancel),
  EV_COMMAND(IDC_STARTBUTTON, StartMPEG),
  EV_COMMAND(IDC_SETTINGSBUTTON, RunSettings),
  EV_COMMAND(IDC_HELPBUTTON, CmHelp),
  EV_COMMAND(IDC_SUSPENDBUTTON, CmSuspend),
  EV_COMMAND(IDC_BATCHMODE, EvBatchMode),
  EV_COMMAND(IDC_WRITEMESS, EvWriteMess),
END_RESPONSE_TABLE;


TMPEGDialog::TMPEGDialog(TWindow* parent, int resourceID, TModule* module)
            :TDialog(parent, resourceID, module)
{
  progressText = new TStatic(this, IDC_PROGRESSTXT);
  cancelButton = new TButton(this, IDC_CANCELBUTTON);
  startButton = new TButton(this, IDC_STARTBUTTON);
  settingsButton = new TButton(this, IDC_SETTINGSBUTTON);
  suspendButton = new TButton(this, IDC_SUSPENDBUTTON);
  helpButton = new TButton(this, IDC_HELPBUTTON);
  writeMess = new TButton(this, IDC_WRITEMESS);
  messageLBox = new TListBox(this, IDC_WARNINGLB);
  progressBar = new TGauge(this, IDC_PROGRESSBAR);
  batchMode = new TCheckBox(this, IDC_BATCHMODE);
}

void TMPEGDialog::SetupWindow()
{
  TDialog::SetupWindow();
  MPEGDialog = this;
  messageLBox->SetHorizontalExtent(400);
  suspendButton->EnableWindow(false);

  progressBar->SetRange(0, 100);
  progressBar->SetStep(1);
  progressBar->SetValue(0);

  if (BatchMode)
    batchMode->Check();
  else
    batchMode->Uncheck();
  ShowInfo();
  if ((EncodeVideo || EncodeAudio) && BatchMode)
    PostMessage(WM_COMMAND, IDC_STARTBUTTON);
}

void TMPEGDialog::ShowInfo()
{
  char tmpStr[MAXPATH], tmpStr1[80];
  static int bitrate[2][15] = {
          {0,32,64,96,128,160,192,224,256,288,320,352,384,416,448},
          {0,32,48,56,64,80,96,112,128,160,192,224,256,320,384}};

  messageLBox->ClearList();
  if (VideoAvail)
  {
    AddMessage("Input information");
    AddMessage("   Video:");
    sprintf(tmpStr, "      width: %d, height: %d", horizontal_size, vertical_size);
    AddMessage(tmpStr);
    sprintf(tmpStr, "      first frame: %d, number of frames: %d", frame0, nframes);
    AddMessage(tmpStr);
  }
  else
    if (InvalidVideo)
      AddMessage("   Video: unsupported video format");
    else
      AddMessage("   Video: none");
  AddMessage(" ");
  if (AudioAvail)
  {
    AddMessage("   Audio:");
    sprintf(tmpStr, "      sample rate: %.1f kHz", (double) audioSampleRate / 1000.0);
    AddMessage(tmpStr);
    if (audioStereo)
      AddMessage("      channels: stereo");
    else
      AddMessage("      channels: mono");
    sprintf(tmpStr, "      bits per sample: %d", audioSampleSize);
    AddMessage(tmpStr);
    if (!UseAdobeAPI)
    {
      sprintf(tmpStr, "      number of samples: %u", makeMPEGInfo.endSample - makeMPEGInfo.startSample + 1);
      AddMessage(tmpStr);
    }
  }
  else
    if (InvalidAudio)
      AddMessage("   Audio: unsupported audio format");
    else
      AddMessage("   Audio: none");
  AddMessage(" ");
  AddMessage("Output MPEG information");
  if (EncodeVideo)
  {
    if (SaveTempVideo || !MplexVideo)
      sprintf(tmpStr, "  Video: %s", VideoFilename);
    else
      sprintf(tmpStr, "  Video: %s, deleted if multiplexed", VideoFilename);
    AddMessage(tmpStr);
    if (constant_bitrate)
      sprintf(tmpStr1, "%.2f Mbps bitrate", bit_rate / 1000000.0);
    else
      sprintf(tmpStr1, "variable bitrate, quant value = %d", mquant_value);
    switch (video_type)
    {
      case MPEG_MPEG1:
        sprintf(tmpStr, "      MPEG-1, %dx%d @ %.3f fps, %s",
                horizontal_size, vertical_size, frame_rate, tmpStr1);
        AddMessage(tmpStr);
        break;
      case MPEG_VCD:
        sprintf(tmpStr, "      VCD, %dx%d @ %.3f fps, %s",
                horizontal_size, vertical_size, frame_rate, tmpStr1);
        AddMessage(tmpStr);
        if (!vVideoCDAvail)
          AddMessage("      WARNING: Video should be 352x240 or 352x288 for VCD");
        break;
      case MPEG_MPEG2:
        switch (video_pulldown_flag)
        {
          case PULLDOWN_32:
			if (frame_rate_code == 1)
              sprintf(tmpStr, "      MPEG-2, %dx%d @ %.3f fps (29.97 fps with 3:2 pulldown), %s",
                      horizontal_size, vertical_size, frame_rate, tmpStr1);
			else
              sprintf(tmpStr, "      MPEG-2, %dx%d @ %.3f fps (30.0 fps with 3:2 pulldown), %s",
                      horizontal_size, vertical_size, frame_rate, tmpStr1);
            break;
          case PULLDOWN_23:
			if (frame_rate_code == 1)
              sprintf(tmpStr, "      MPEG-2, %dx%d @ %.3f fps (29.97 fps with 2:3 pulldown), %s",
                      horizontal_size, vertical_size, frame_rate, tmpStr1);
		    else
              sprintf(tmpStr, "      MPEG-2, %dx%d @ %.3f fps (30.0 fps with 2:3 pulldown), %s",
                      horizontal_size, vertical_size, frame_rate, tmpStr1);
            break;
          default:
            sprintf(tmpStr, "      MPEG-2, %dx%d @ %.3f fps, %s",
                    horizontal_size, vertical_size, frame_rate, tmpStr1);
        }
        AddMessage(tmpStr);
        if ((frame_rate_code > 2) &&
           ((video_pulldown_flag == PULLDOWN_32) || (video_pulldown_flag == PULLDOWN_23)))
          AddMessage("      WARNING: Frame rate should be 23.976 or 24 fps for 3:2 or 2:3 pulldown");
        break;
      case MPEG_SVCD:
        switch (video_pulldown_flag)
        {
          case PULLDOWN_32:
			if (frame_rate_code == 1)
              sprintf(tmpStr, "      SVCD, %dx%d @ %.3f fps (29.97 fps with 3:2 pulldown), %s",
                      horizontal_size, vertical_size, frame_rate, tmpStr1);
			else
              sprintf(tmpStr, "      SVCD, %dx%d @ %.3f fps (30.0 fps with 3:2 pulldown), %s",
                      horizontal_size, vertical_size, frame_rate, tmpStr1);
            break;
          case PULLDOWN_23:
			if (frame_rate_code == 1)
              sprintf(tmpStr, "      SVCD, %dx%d @ %.3f fps (29.97 fps with 2:3 pulldown), %s",
                      horizontal_size, vertical_size, frame_rate, tmpStr1);
			else
              sprintf(tmpStr, "      SVCD, %dx%d @ %.3f fps (30.0 fps with 2:3 pulldown), %s",
                      horizontal_size, vertical_size, frame_rate, tmpStr1);
            break;
          default:
            sprintf(tmpStr, "      SVCD, %dx%d @ %.3f fps, %s",
                    horizontal_size, vertical_size, frame_rate, tmpStr1);
        }
        AddMessage(tmpStr);
        if (!vSVCDAvail)
          AddMessage("      WARNING: Video should be 480x480 or 480x576 for SVCD");
        if ((frame_rate_code > 2) &&
           ((video_pulldown_flag == PULLDOWN_32) || (video_pulldown_flag == PULLDOWN_23)))
          AddMessage("      WARNING: Frame rate should be 23.976 or 24 fps for 3:2 or 2:3 pulldown");
        break;
      case MPEG_DVD:
        switch (video_pulldown_flag)
        {
          case PULLDOWN_32:
			if (frame_rate_code == 1)
              sprintf(tmpStr, "      DVD, %dx%d @ %.3f fps (29.97 fps with 3:2 pulldown), %s",
                      horizontal_size, vertical_size, frame_rate, tmpStr1);
			else
              sprintf(tmpStr, "      DVD, %dx%d @ %.3f fps (30.0 fps with 3:2 pulldown), %s",
                      horizontal_size, vertical_size, frame_rate, tmpStr1);
            break;
          case PULLDOWN_23:
			if (frame_rate_code == 1)
              sprintf(tmpStr, "      DVD, %dx%d @ %.3f fps (29.97 fps with 2:3 pulldown), %s",
                      horizontal_size, vertical_size, frame_rate, tmpStr1);
			else
              sprintf(tmpStr, "      DVD, %dx%d @ %.3f fps (30.0 fps with 2:3 pulldown), %s",
                      horizontal_size, vertical_size, frame_rate, tmpStr1);
            break;
          default:
            sprintf(tmpStr, "      DVD, %dx%d @ %.3f fps, %s",
                    horizontal_size, vertical_size, frame_rate, tmpStr1);
        }
        AddMessage(tmpStr);
        if (!vDVDAvail)
        {
          AddMessage("      WARNING: Video should be 352x240, 352x288, 352x480, 352x576,");
          AddMessage("                   704x480, 704x576, 720x480 or 720x576 for DVD");
        }
        if ((frame_rate_code > 2) &&
           ((video_pulldown_flag == PULLDOWN_32) || (video_pulldown_flag == PULLDOWN_23)))
          AddMessage("      WARNING: Frame rate should be 23.976 or 24 fps for 3:2 or 2:3 pulldown");
        break;
    }
  }
  else
    AddMessage("   Video: none");
  AddMessage(" ");

  if (EncodeAudio)
  {
    if (SaveTempAudio || !MplexAudio)
      sprintf(tmpStr, "   Audio: %s", AudioFilename);
    else
      sprintf(tmpStr, "   Audio: %s, deleted if multiplexed", AudioFilename);
    AddMessage(tmpStr);
    switch (audio_mode)
    {
      case MPG_MD_STEREO:
        sprintf(tmpStr, "      Layer %d, %d kbps, %.1f kHz, stereo",
                audio_layer, bitrate[audio_layer - 1][audio_bitrate],
                (double) audioSampleRate / 1000.0);
        break;
      case MPG_MD_JOINT_STEREO:
        sprintf(tmpStr, "      Layer %d, %d kbps, %.1f kHz, joint stereo",
                audio_layer, bitrate[audio_layer - 1][audio_bitrate],
                (double) audioSampleRate / 1000.0);
        break;
      case MPG_MD_DUAL_CHANNEL:
        sprintf(tmpStr, "      Layer %d, %d kbps, %.1f kHz, dual channel",
                audio_layer, bitrate[audio_layer - 1][audio_bitrate],
                (double) audioSampleRate / 1000.0);
        break;
      case MPG_MD_MONO:
        sprintf(tmpStr, "      Layer %d, %d kbps, %.1f kHz, mono",
                audio_layer, bitrate[audio_layer - 1][audio_bitrate],
                (double) audioSampleRate / 1000.0);
        break;
    }
    AddMessage(tmpStr);
    switch (video_type)
    {
      case MPEG_VCD:
        if (!aVideoCDAvail)
          AddMessage("      WARNING: Audio should be 44.1kHz 16-bit stereo for VCD");
        break;
      case MPEG_SVCD:
        if (!aSVCDAvail)
          AddMessage("      WARNING: Audio should be 44.1kHz 16-bit for SVCD");
        break;
      case MPEG_DVD:
        if (!aDVDAvail)
          AddMessage("      WARNING: Audio should be 48.1kHz 16-bit for DVD");
        break;
    }
  }
  else
    AddMessage("   Audio: none");
  AddMessage(" ");

  if (MplexVideo || MplexAudio)
  {
    if (MplexVideo && MplexAudio)
    {
      if (strlen(Audio1Filename))
        AddMessage("  Multiplexing: video and two audio streams");
      else
        AddMessage("  Multiplexing: video and one audio stream");
    }
    else
      if (MplexVideo)
        AddMessage("  Multiplexing: video only");
      else
      {
        if (strlen(Audio1Filename))
          AddMessage("  Multiplexing: two audio streams only");
        else
          AddMessage("  Multiplexing: one audio stream only");
      }
    switch (mplex_type)
    {
      case MPEG_MPEG1:
        AddMessage("      program stream type: MPEG-1");
        break;
      case MPEG_VCD:
        AddMessage("      program stream type: VCD");
        break;
      case MPEG_MPEG2:
        switch (mplex_pulldown_flag)
        {
          case PULLDOWN_32:
            AddMessage("      program stream type: MPEG-2 with 3:2 pulldown");
            break;
          case PULLDOWN_23:
            AddMessage("      program stream type: MPEG-2 with 2:3 pulldown");
            break;
          case PULLDOWN_AUTO:
            AddMessage("      program stream type: MPEG-2 with pulldown auto-detection");
            break;
          default:
            AddMessage("      program stream type: MPEG-2");
        }
        break;
      case MPEG_SVCD:
        switch (mplex_pulldown_flag)
        {
          case PULLDOWN_32:
            AddMessage("      program stream type: SVCD with 3:2 pulldown");
            break;
          case PULLDOWN_23:
            AddMessage("      program stream type: SVCD with 2:3 pulldown");
            break;
          case PULLDOWN_AUTO:
            AddMessage("      program stream type: SVCD with pulldown auto-detection");
            break;
          case PULLDOWN_NONE:
            AddMessage("      program stream type: SVCD");
        }
        break;
      case MPEG_DVD:
        switch (mplex_pulldown_flag)
        {
          case PULLDOWN_32:
            AddMessage("      program stream type: DVD with 3:2 pulldown");
            break;
          case PULLDOWN_23:
            AddMessage("      program stream type: DVD with 2:3 pulldown");
            break;
          case PULLDOWN_AUTO:
            AddMessage("      program stream type: DVD with pulldown auto-detection");
            break;
          case PULLDOWN_NONE:
            AddMessage("      program stream type: DVD");
        }
        break;
    }
    if (MplexVideo)
    {
      sprintf(tmpStr, "      video file: %s", VideoFilename);
      AddMessage(tmpStr);
    }
    if (MplexAudio)
    {
      sprintf(tmpStr, "      audio 1 file: %s", AudioFilename);
      AddMessage(tmpStr);
      if (strlen(Audio1Filename))
      {
        sprintf(tmpStr, "      audio 2 file: %s", Audio1Filename);
        AddMessage(tmpStr);
      }
    }
    sprintf(tmpStr, "      program file: %s", ProgramFilename);
    AddMessage(tmpStr);
  }
  else
    AddMessage("  Multiplexing: none");
  AddMessage(" ");
}

void TMPEGDialog::EvBatchMode()
{
  BatchMode = batchMode->GetCheck() == BF_CHECKED;
}

void TMPEGDialog::EvWriteMess()
{
  char tmpStr[MAXPATH];
  FILE *ofile;
  int i;

  static TOpenSaveDialog::TData data (
         OFN_PATHMUSTEXIST|OFN_OVERWRITEPROMPT|OFN_HIDEREADONLY|OFN_NOREADONLYRETURN,
         "Text Files (*.TXT)|*.txt|All Files (*.*)|*.*|",
         0, "", "TXT");

  sprintf(tmpStr, "%s - write messages to text file", AppName);
  if ((new TFileSaveDialog(this, data, 0, tmpStr))->Execute() == IDOK)
  {
    ofile = fopen(data.FileName, "w");
    if (!ofile)
    {
      sprintf(tmpStr, "Unable to open output file %s", data.FileName);
      MessageBox(tmpStr, "Message output error", MB_ICONERROR | MB_OK);
      return;
    }
    messageLBox->SetFocus();
    for (i = 0; i < messageLBox->GetCount(); i++)
    {
      messageLBox->SetCaretIndex(i, true);
      messageLBox->GetString(tmpStr, i);
      strcat(tmpStr, "\n");
      fprintf(ofile, tmpStr);
    }
    fclose(ofile);
  }
}

#ifdef TESTING
bool dovideotemp()
{
  void *buffer;
  long rowbytes;
  compGetFrameReturnRec	getFrameRet;
  int i, j, percent, repeatCount;
  char tmpStr[80];
  time_t start_t, end_t, tot_t;
  double frame_t;

  start_t = time(NULL);
  for (i=1; i<=nframes; i++)
  {
    percent = floor(((double) (i)) / ((double) nframes) * 100.0);
    sprintf(tmpStr, "Video: %d%% - frame %d of %d.", percent, i, nframes);
    MPEGDialog->progressText->SetText(tmpStr);
    MPEGDialog->progressBar->SetValue(percent);
    MPEGDialog->YieldTime();
    if (AbortMPEG)
      return false;
    if (stdParmsSave->funcs->videoFuncs->getFrame(i - 1 + frame0, &buffer,
        &rowbytes, &getFrameRet, 0, doCompileRecSave->compileSeqID) != comp_ErrNone)
      return false;
    repeatCount = getFrameRet.repeatCount;
    for (j = 0; j < frame0; j++);
  }
  sprintf(tmpStr, "Video: 100%% - frame %d of %d.", nframes, nframes);
  MPEGDialog->progressText->SetText(tmpStr);
  MPEGDialog->progressBar->SetValue(100);
  end_t = time(NULL);
  tot_t = end_t - start_t;
  frame_t = (double) tot_t / (double) nframes;
  sprintf(tmpStr, "   Video time: %ld seconds, time per frame: %.2f seconds.", tot_t, frame_t);
  MPEGDialog->AddMessage(tmpStr);
  return true;
}

bool doaudiotemp()
{
//  long blipMax;
  long start_sample, samples_read, bytes_read;
  BufferReturnType theBuffer;
  char *tmpPtr;
  char **tmpPtr1;
  char tmpStr[80];

//  blipMax = stdParmsSave->funcs->audioFuncs->getBlipMax(doCompileRecSave->compileSeqID);
//  sprintf(tmpStr, "blip max = %d", blipMax);
//  MPEGDialog->AddMessage(tmpStr);
  start_sample = 0;
  samples_read = 1;
//  tmpPtr = (char *) malloc(blipMax);
//  if (tmpPtr == NULL)
//  {
//    DisplayError("Could not allocate memory for audio buffer.");
//    return false;
//  }
  tmpPtr = NULL;
  tmpPtr1 = &tmpPtr;
  theBuffer = &tmpPtr1;
  if (stdParmsSave->funcs->audioFuncs->getAudio(start_sample, &samples_read,
      &bytes_read, 0, theBuffer, doCompileRecSave->compileSeqID) != comp_ErrNone)
  {
    sprintf(tmpStr, "  Could not get audio data at %d sample.", start_sample);
    MPEGDialog->AddMessage(tmpStr);
    return false;
  }
  sprintf(tmpStr, "  samples_read = %d, bytes_read = %d", samples_read, bytes_read);
  MPEGDialog->AddMessage(tmpStr);

//  free(tmpPtr);

  return true;
}

#endif
*/

void StartMPEG()
{

  bool cont;
  char tmpStr[MAXPATH];

  if ((EncodeVideo || MplexVideo) && !strlen(VideoFilename))
  {
//    MessageBox("A video stream filename must be specified, running settings ...", AppName, MB_OK);
  //  PostMessage(WM_COMMAND, IDC_SETTINGSBUTTON);
#ifdef _DEBUG
	  DebugOutbutMessageToFile("Error: A video stream filename must be specified\n");
#endif

    return;
  }
  if ((EncodeAudio || MplexAudio) && !strlen(AudioFilename) && !strlen(Audio1Filename))
  {
   // MessageBox("An audio stream filename must be specified, running settings ...", AppName, MB_OK);
   // PostMessage(WM_COMMAND, IDC_SETTINGSBUTTON);
#ifdef _DEBUG
	    DebugOutbutMessageToFile("Error: An audio stream filename must be specified\n");
#endif

    return;
  }

  if ((MplexVideo || MplexAudio) && !strlen(ProgramFilename))
  {
   // MessageBox("A program stream filename must be specified, running settings ...", AppName, MB_OK);
   // PostMessage(WM_COMMAND, IDC_SETTINGSBUTTON);
	  DebugOutbutMessageToFile("Error: A program stream filename must be specified\n");

    return;
  }

#ifdef _DEBUG
   DebugOutbutMessageToFile("Start mpeg1\n");
#endif


  PutTempSettings(&tempSettings1); // save off temp settings 
  if (EncodeVideo)
  {
    if (!CheckVideoSettings(&tempSettings1))
    {
   //   MessageBox("Invalid settings, running settings ...", AppName, MB_OK);
   //   PostMessage(WM_COMMAND, IDC_SETTINGSBUTTON);

#ifdef _DEBUG
	  DebugOutbutMessageToFile("encoder error: Invalid video settings!!!! in Startmpeg \n");
#endif

      return;
    }
  }
  if (EncodeAudio)
  {
    if (!CheckAudioSettings(&tempSettings1))
    {
//      MessageBox("Invalid settings, running settings ...", AppName, MB_OK);
  //    PostMessage(WM_COMMAND, IDC_SETTINGSBUTTON);

#ifdef _DEBUG
		DebugOutbutMessageToFile("encoder error: Invalid audio settings!!!! in Startmpeg \n");
#endif

      return;
    }
  }

  OutputStats = (strlen(statname) != 0);
  if (OutputStats)
  {
    // open statistics output file 
    statfile = fopen(statname, "w");
    if (statfile == NULL)
    {
    
  //    MessageBox(tmpStr, AppName, MB_OK | MB_ICONERROR);

#ifdef _DEBUG
	  sprintf(tmpStr, "Could not create statistics output file %s.", statname);
	  DebugOutbutMessageToFile("encoder error: Could not create statistics output file %s.", statname);
#endif

      return;
    }
  }
  else
    statfile = NULL;

//  writeMess->EnableWindow(false);
 // startButton->EnableWindow(false);
 // suspendButton->EnableWindow(true);
//  settingsButton->EnableWindow(false);
//  cancelButton->EnableWindow(false);
//  helpButton->EnableWindow(false);
//  Encoding = true;
  //SuspendMPEG = false;

#ifdef _DEBUG
   DebugOutbutMessageToFile("Start mpeg 2\n");
#endif

  if (EncodeVideo)
  {
#ifdef _DEBUG
	   DebugOutbutMessageToFile("Start mpeg3\n");
#endif

//    AddMessage(" ");
 //   AddMessage("Encoding Video:");
#ifdef TESTING
    cont = dovideotemp();
#else
    cont = dovideo();
#endif
  }
  else
    cont = true;
  if ((!AbortMPEG) && (cont) && (EncodeAudio))
  {
#ifdef _DEBUG
	   DebugOutbutMessageToFile("Start mpeg4\n");
#endif

 //   AddMessage(" ");
   // AddMessage("Encoding Audio:");
//    progressBar->SetValue(0);
 //   progressText->SetText("");
#ifdef TESTING
    cont = doaudiotemp();
#else
    cont = doaudio();
#endif
  }
  if (!AbortMPEG && cont && (MplexVideo || MplexAudio))
  {
#ifdef _DEBUG
	   DebugOutbutMessageToFile("Start mpeg5\n");
#endif

//    AddMessage(" ");
 //   progressBar->SetValue(0);
 //   progressText->SetText("");
 //   if (MplexVideo)
  //  {
    //  if (MplexAudio)
//        AddMessage("Multiplexing video and audio:");
    //  else
  //      AddMessage("Multiplexing video:");
//    }
//    else
  //    AddMessage("Multiplexing audio:");
#ifdef _DEBUG
	DebugOutbutMessageToFile("mplexvideo = %d\n",MplexVideo);
	DebugOutbutMessageToFile("MplexAudio = %d\n",MplexAudio);
#endif
    cont = domplex(MplexVideo, MplexAudio);
    if (cont)
    {
      if (EncodeVideo && !SaveTempVideo)
	  {
	   
        unlink(VideoFilename);
	  }
      if (EncodeAudio && !SaveTempAudio)
	  {
	
        unlink(AudioFilename);
	  }
    }
  }
//  Encoding = false;
  GetTempSettings(&tempSettings1); // restore real settings 
//  suspendButton->EnableWindow(false);
//  cancelButton->SetCaption("&Ok");
//  cancelButton->EnableWindow(true);
//  writeMess->EnableWindow(true);

  if (statfile)
    fclose(statfile);
  statfile = NULL;

  if (AbortMPEG)
  {
	  
 //   AddMessage(" ");
  //  AddMessage("ENCODING ABORTED.");
  //  progressBar->SetValue(0);
  //  progressText->SetText("");
  }
  if (cont && !AbortMPEG)
  {
	
 //   exit_code = 1;
  //  if (BatchMode)
   //   PostMessage(WM_CLOSE);
  }

	//DebugOutbutMessageToFile("all done in StartMpeg\n");
}
/*

void TMPEGDialog::CmSuspend()
{
  SuspendMPEG = true;
}

void TMPEGDialog::RunSettings()
{
  DoSettings(HWindow, 0);
  ShowInfo();  
  if (EncodeVideo || EncodeAudio || MplexVideo || MplexAudio)
    startButton->EnableWindow(true);
  else
    startButton->EnableWindow(false);
}

void TMPEGDialog::CmHelp()
{
  WinHelp(HelpFileName, HELP_CONTEXT, 1);
}

void TMPEGDialog::YieldTime()
{
  MSG msg;

  while (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
  {
    TranslateMessage(&msg);
    DispatchMessage(&msg);
  }
  if (SuspendMPEG)
  {
    SuspendMPEG = false;
    if (MessageBox("Resume encoding?", AppName, MB_YESNO | MB_ICONQUESTION) == IDNO)
      AbortMPEG = true;
  }
}

void TMPEGDialog::AddMessage(char *txt)
{
  messageLBox->SetCaretIndex(messageLBox->AddString(txt), true);
}

void TMPEGDialog::CmCancel()
{
  if (Encoding)
    AbortMPEG = true;
  else
  {
    MPEGDialog = NULL;
    TDialog::CmCancel();
  }
}


class TMyFrameWindow : public TFrameWindow
{
  public:
    TMyFrameWindow::TMyFrameWindow(TWindow* parent, const char far *title = 0,
       TWindow* clientWnd = 0, bool shrinkToClient = false, TModule* module = 0);
    void SetupWindow();
    void EvSysCommand(uint cmdType, TPoint&);
    int32 EvMenuChar(uint nChar, uint, HMENU);
    void EvSize(uint sizeType, TSize& size);
	void EvSetFocus(HWND hWndLostFocus);
    void EvClose();
    ~TMyFrameWindow();

    DECLARE_RESPONSE_TABLE(TMyFrameWindow);
};

DEFINE_RESPONSE_TABLE1(TMyFrameWindow, TFrameWindow)
  EV_WM_MENUCHAR,
  EV_WM_CLOSE,
  EV_WM_SYSCOMMAND,
  EV_WM_SIZE,
  EV_WM_SETFOCUS,
END_RESPONSE_TABLE;

TMyFrameWindow::TMyFrameWindow(TWindow* parent, const char far *title,
                               TWindow* clientWnd, bool shrinkToClient,
                               TModule* module)
       :TFrameWindow(parent, title, clientWnd, shrinkToClient, module)
{
  Attr.X = GetProfileInt(AppName, "left", 200);
  Attr.Y = GetProfileInt(AppName, "top", 75);
  Attr.Style ^= WS_MAXIMIZEBOX;
  SetIcon(module , IDI_BBMPEGICON);
  SetIconSm(module , IDI_SMMPEGICON);
}

void TMyFrameWindow::SetupWindow()
{
  HMENU sysMenu;

  TFrameWindow::SetupWindow();
  sysMenu = GetSystemMenu(false);
  AppendMenu(sysMenu, MF_SEPARATOR, 0, NULL);
  AppendMenu(sysMenu, MF_STRING, IDC_ABOUT, "&About ...");
}

void TMyFrameWindow::EvSysCommand(uint cmdType, TPoint&)
{
  if (cmdType == IDC_ABOUT)
    MessageBox(About, AppName, MB_OK);
  else
    DefaultProcessing();
}

int32 TMyFrameWindow::EvMenuChar(uint nChar, uint, HMENU)
{
  if ((nChar == 'A') || (nChar == 'a'))
  {
    MessageBox(About, AppName, MB_OK);
    return 0x10000;
  }
  else
    return DefaultProcessing();
}


TMyFrameWindow::~TMyFrameWindow()
{
  char tmpStr[10];

  sprintf(tmpStr, "%d", Attr.X);
  WriteProfileString(AppName, "left", tmpStr);
  sprintf(tmpStr, "%d", Attr.Y);
  WriteProfileString(AppName, "top", tmpStr);
}

void TMyFrameWindow::EvClose()
{
  if (Encoding)
    AbortMPEG = true;
  else
    TFrameWindow::EvClose();
}


void TMyFrameWindow::EvSetFocus(HWND hWndLostFocus)
{
  TFrameWindow::EvSetFocus(hWndLostFocus);
  BringWindowToTop();
}

void TMyFrameWindow::EvSize(uint sizeType, TSize& size)
{
  TFrameWindow::EvSize(sizeType, size);
  if (appWindow)
  {
	switch (sizeType)
	{
	  case SIZE_MINIMIZED:
        ::ShowWindow(appWindow, SW_MINIMIZE);
		break;
	  case SIZE_RESTORED:
		::ShowWindow(appWindow, SW_RESTORE);
        break;
	}
  }
}

*/

//int frame0;


int startGui()
{
#ifdef _DEBUG
 DebugOutbutMessageToFile("called startgui \n");
#endif
  int retval;
  char *strPtr;
  char tmpStr[MAXPATH];

  //MPEGDialog = NULL;
  FileOutError = false;
  AbortMPEG = false;
//  Encoding = false;
  int frame0 = doCompileRecSave->startFrame;
  nframes = doCompileRecSave->endFrame - frame0 + 1;
 // if (doCompileRecSave->outputRec.doVideo)
  {
    input_horizontal_size = doCompileRecSave->outputRec.width;
    horizontal_size = input_horizontal_size;
    input_vertical_size = doCompileRecSave->outputRec.height;
    vertical_size = input_vertical_size;
    if (horizontal_size % 2)
      horizontal_size--;
    if (vertical_size % 2)
      vertical_size--;
    VideoAvail = true;
  }
//  else
 //   VideoAvail = false;

  if (!VideoAvail)
  {
#ifdef _DEBUG
	DebugOutbutMessageToFile("Setting encode video to false \n");
#endif
    EncodeVideo = FALSE;
  }
  else
    EncodeVideo = UserEncodeVideo;

  // SRG
  // ok for not to encode vode
  if (doCompileRecSave->outputRec.doVideo==0)
  {
	  EncodeVideo=0;
  }



#ifdef _DEBUG
  DebugOutbutMessageToFile("encoder video = %d \n", EncodeVideo);
#endif

  if (doCompileRecSave->outputRec.doAudio)
  {
#ifdef _DEBUG
	 DebugOutbutMessageToFile("outputRec.doAudio is true");
#endif
    audioSampleRate = doCompileRecSave->outputRec.audrate;
    audioStereo = (doCompileRecSave->outputRec.stereo);
    if (audioStereo)
      audio_mode = MPG_MD_STEREO;
    else
      audio_mode = MPG_MD_MONO;
    audioSampleSize = doCompileRecSave->outputRec.audsamplesize;
    AudioAvail = true;
  }
  else
    AudioAvail = false;

  if (!AudioAvail)
    EncodeAudio = FALSE;
  else
    EncodeAudio = UserEncodeAudio;

  /*
  if ((EncodeAudio && EncodeVideo) || (!EncodeAudio && !EncodeVideo))
  {
    MplexVideo = true;
    MplexAudio = true;
  }
  else
  {
    MplexVideo = false;
    MplexAudio = false;
  }
  */


  // SRG WE ALWAYS MULTIPLEX VIDEO AND AUDIO
    MplexAudio = true;
	MplexVideo = true;

#ifdef _DEBUG
   DebugOutbutMessageToFile("EncodeAudio= %d \n", EncodeAudio);
   DebugOutbutMessageToFile("MplexAudio= %d \n", MplexAudio);
#endif



  vVideoCDAvail = (VideoAvail &&
                   (horizontal_size == 352) &&
                  ((vertical_size == 240) || (vertical_size == 288)));
  aVideoCDAvail = (AudioAvail &&
                  (audio_mode == MPG_MD_STEREO) &&
                  (audioSampleSize == 16) &&
                  (audioSampleRate == 44100));
  vSVCDAvail = (VideoAvail &&
                (horizontal_size == 480) &&
               ((vertical_size == 480) || (vertical_size == 576)));
  aSVCDAvail = (AudioAvail &&
               (audioSampleSize == 16) &&
               (audioSampleRate == 44100));
  vDVDAvail = (VideoAvail &&
             ((horizontal_size == 704) || (horizontal_size == 720) || (horizontal_size == 352) &&
             ((vertical_size == 480) || (vertical_size == 576) ||
              (vertical_size == 240) || (vertical_size == 288))));
  aDVDAvail = (AudioAvail &&
              (audioSampleSize == 16) &&
              (audioSampleRate == 48000));
  if (VideoAvail && auto_bitrate)
  {
    PutTempSettings(&tempSettings1);
    AutoSetBitrateData(&tempSettings1);
    GetTempSettings(&tempSettings1);
  }
  if (strlen(doCompileRecSave->outputFile.name))
    strcpy(ProgramFilename, doCompileRecSave->outputFile.name);
  else
  {
    strcpy(ProgramFilename, "temp");
    strcat(ProgramFilename, DefaultExt);
  }

  if (VideoAvail)
  {
    strcpy(VideoFilename, ProgramFilename);
    strPtr = strrchr(VideoFilename, '.');
    if (strPtr)
      strPtr[0] = '\0';
    if (video_type < MPEG_MPEG2)
      strcat(VideoFilename, ".temp");
    else
      strcat(VideoFilename, ".temp");
  }
  else
    strcpy(VideoFilename, "");

 // if (AudioAvail)
  {
    strcpy(AudioFilename, ProgramFilename);
    strPtr = strrchr(AudioFilename, '.');
    if (strPtr)
      strPtr[0] = '\0';

	if (EncodeAudio==false)
	{
	  strcat(AudioFilename, ".ac3");
#ifdef _DEBUG
	  DebugOutbutMessageToFile("Setting audiofile to = %s \n", AudioFilename);
#endif

	}
	else
	{
	 strcat(AudioFilename, ".audio");
#ifdef _DEBUG
	   DebugOutbutMessageToFile("Setting audiofile to = %s \n", AudioFilename);
#endif
	}
  }
 // else
  //  strcpy(AudioFilename, "");

  strcpy(Audio1Filename, "");

  if (AudioAvail || VideoAvail)
    input_range_checks();

//  exit_code = 0;

  strcpy(tmpStr, AppName);
  strcat(tmpStr, " - ");
  strcat(tmpStr, Version);
//  TMyFrameWindow *frameWindow = new TMyFrameWindow(0, tmpStr, new TMPEGDialog(0, IDD_MPEGDIALOG, module), true, module);

//  retval = frameWindow->Execute();

#ifdef _DEBUG
  DebugOutbutMessageToFile("encoder calling StartMPEG \n");
#endif
  StartMPEG();

//  MPEGDialog = NULL;
//  if (retval == -1)
 // {
  //  DisplayError("Unable to create main window.");
  //  return comp_InternalError;
 // }

//  if (exit_code)
    return comp_CompileDone;
 // else
   // return comp_CompileAbort;
}

