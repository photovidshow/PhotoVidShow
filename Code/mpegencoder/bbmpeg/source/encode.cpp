/**********************************************************************
Copyright (c) 1991 MPEG/audio software simulation group, All Rights Reserved
encode.c
**********************************************************************/
/**********************************************************************
 * MPEG/audio coding/decoding software, work in progress              *
 *   NOT for public distribution until verified and approved by the   *
 *   MPEG/audio committee.  For further information, please contact   *
 *   Davis Pan, 508-493-2241, e-mail: pan@3d.enet.dec.com             *
 *                                                                    *
 * VERSION 3.9t                                                       *
 *   changes made since last update:                                  *
 *   date   programmers         comment                               *
 * 3/01/91  Douglas Wong,       start of version 1.1 records          *
 *          Davis Pan                                                 *
 * 3/06/91  Douglas Wong        rename: setup.h to endef.h            *
 *                                      efilter to enfilter           *
 *                                      ewindow to enwindow           *
 *                              integrated "quantizer", "scalefactor",*
 *                              and "transmission" files              *
 *                              update routine "window_subband"       *
 * 3/31/91  Bill Aspromonte     replaced read_filter by               *
 *                              create_an_filter                      *
 * 5/10/91  W. Joseph Carter    Ported to Macintosh and Unix.         *
 *                              Incorporated Jean-Georges Fritsch's   *
 *                              "bitstream.c" package.                *
 *                              Incorporated Bill Aspromonte's        *
 *                              filterbank coefficient matrix         *
 *                              calculation routines and added        *
 *                              roundoff to coincide with specs.      *
 *                              Modified to strictly adhere to        *
 *                              encoded bitstream specs, including    *
 *                              "Berlin changes".                     *
 *                              Modified PCM sound file handling to   *
 *                              process all incoming samples and fill *
 *                              out last encoded frame with zeros     *
 *                              (silence) if needed.                  *
 *                              Located and fixed numerous software   *
 *                              bugs and table data errors.           *
 * 19jun91  dpwe (Aware)        moved "alloc_*" reader to common.c    *
 *                              Globals sblimit, alloc replaced by new*
 *                              struct 'frame_params' passed as arg.  *
 *                              Added JOINT STEREO coding, layers I,II*
 *                              Affects: *_bit_allocation,            *
 *                              subband_quantization, encode_bit_alloc*
 *                              sample_encoding                       *
 * 6/10/91  Earle Jennings      modified II_subband_quantization to   *
 *                              resolve type cast problem for MS_DOS  *
 * 6/11/91  Earle Jennings      modified to avoid overflow on MS_DOS  *
 *                              in routine filter_subband             *
 * 7/10/91  Earle Jennings      port to MsDos from MacIntosh version  *
 * 8/ 8/91  Jens Spille         Change for MS-C6.00                   *
 *10/ 1/91  S.I. Sudharsanan,   Ported to IBM AIX platform.           *
 *          Don H. Lee,                                               *
 *          Peter W. Farrett                                          *
 *10/ 3/91  Don H. Lee          implemented CRC-16 error protection   *
 *                              newly introduced function encode_CRC  *
 *11/ 8/91  Kathy Wang          Documentation of code                 *
 *                              All variablenames are referred to     *
 *                              with surrounding pound (#) signs      *
 * 2/11/92  W. Joseph Carter    Ported new code to Macintosh.  Most   *
 *                              important fixes involved changing     *
 *                              16-bit ints to long or unsigned in    *
 *                              bit alloc routines for quant of 65535 *
 *                              and passing proper function args.     *
 *                              Removed "Other Joint Stereo" option   *
 *                              and made bitrate be total channel     *
 *                              bitrate, irrespective of the mode.    *
 *                              Fixed many Small bugs & reorganized.  *
 * 6/16/92  Shaun Astarabadi    Changed I_scale_factor_calc() and     *
 *                              II_scale_factor_calc() to use scale   *
 *                              factor 0 thru 62 only and not to      *
 *                              encode index 63 into the bit stream.  *
 * 7/27/92  Mike Li             (re-)Port to MS-DOS                   *
 * 9/22/92  jddevine@aware.com  Fixed _scale_factor_calc() defs       *
 * 3/31/93  Giogio Dimino       changed II_a_bit_allocation() from:   *
 *                              if( ad > ...) to if(ad >= ...)        *
 * 8/05/93  TEST                changed I_a_bit_allocation() from:    *
 *                              if( ad > ...) to if(ad >= ...)        *
 **********************************************************************/

#include "main.h"
#include "consts1.h"

extern unsigned _stklen = 16384;

/*=======================================================================\
|                                                                       |
| This segment contains all the core routines of the encoder,           |
| except for the psychoacoustic models.                                 |
|                                                                       |
| The user can select either one of the two psychoacoustic              |
| models. Model I is a simple tonal and noise masking threshold         |
| generator, and Model II is a more sophisticated cochlear masking      |
| threshold generator. Model I is recommended for lower complexity      |
| applications whereas Model II gives better subjective quality at low  |
| bit rates.                                                            |
|                                                                       |
| Layers I and II of mono, stereo, and joint stereo modes are supported.|
| Routines associated with a given layer are prefixed by "I_" for layer |
| 1 and "II_" for layer 2.                                              |
\=======================================================================*/

static unsigned int buffer_size;
static unsigned int buffer_pointer;
static unsigned int current_frame;
static BufferReturnType theBuffer;
static char *tmpPtr;
static char **tmpPtr1;
static short insamp[2304];

#define TMP_BUFFER_SIZE 192000
static char tmpBuffer[TMP_BUFFER_SIZE];

int fill_buffer()
{
  unsigned int i, frames_read, numSamples;
  int percent;
  char tmpStr[80], *srcPtr;

  frames_read = 1;
  buffer_size = 0;

  if (UseAdobeAPI)
  {
    if ((current_frame - frame0) > (unsigned int)nframes)
      return FALSE;
    i = stdParmsSave->funcs->audioFuncs->getAudio(current_frame, (long*) &frames_read,
         (long*) &buffer_size, 0, theBuffer, doCompileRecSave->compileSeqID);
  }
  else
  {
    if (currentSample > makeMPEGInfo.endSample)
      return FALSE;
    i = makeMPEGInfo.mgetAudio(currentSample, &numSamples, &buffer_size, &tmpPtr);
  }

  if ((i != comp_ErrNone) || (buffer_size == 0))
  {
    if (UseAdobeAPI)
    {
      if ((current_frame - frame0) < (unsigned int)nframes)
      {
#ifdef _DEBUG
        sprintf(tmpStr, "  Could not get audio data at frame %d, error code %d.", current_frame, i);
        DisplayError(tmpStr);
#endif
        return FALSE;
      }
    }
    else
    {
      if (currentSample < makeMPEGInfo.endSample)
      {
#ifdef _DEBUG
        sprintf(tmpStr, "  Could not get audio data at sample %d, error code %d.", currentSample, i);
        DisplayError(tmpStr);
#endif
        return FALSE;
      }
    }
    if (!buffer_size)
      return FALSE;
  }

  if (UseAdobeAPI)
  {
    srcPtr = **theBuffer;
    if (buffer_size > TMP_BUFFER_SIZE)
    {
#ifdef _DEBUG
      DisplayError("Returned buffer size is greater than temporary audio buffer size.");
#endif
      return FALSE;
    }
    memcpy(&tmpBuffer[0], srcPtr, buffer_size);
    current_frame++;
    percent = (int)floor(((double) (current_frame - frame0)) / ((double) nframes) * 100.0);
#ifdef _DEBUG
    sprintf(tmpStr, "Audio: %d%% - frame %d of %d.", percent, current_frame - frame0, nframes);
#endif
  }
  else
  {
    currentSample += numSamples;
    percent = (int)floor(((double) (currentSample)) / ((double) nSamples) * 100.0);

#ifdef _DEBUG
    sprintf(tmpStr, "Audio: %d%% - sample %d of %d.", percent, currentSample, nSamples);
#endif
  }
  buffer_pointer = 0;

#ifdef _DEBUG
  DisplayProgress(tmpStr, percent);
#endif

  return TRUE;
}

int init_read_samples()
{
  if (UseAdobeAPI)
    current_frame = frame0;
  else
  {
    currentSample = makeMPEGInfo.startSample;
    nSamples = makeMPEGInfo.endSample - makeMPEGInfo.startSample + 1;
  }
  tmpPtr = NULL;
  tmpPtr1 = &tmpPtr;
  theBuffer = &tmpPtr1;
  return fill_buffer();
}

int read_samples(int frame_size)
{
  int i, j;
  char *destPtr, *srcPtr;
  int abort;

  if (buffer_pointer >= buffer_size)
  {
    if (!fill_buffer())
      return 0;
  }

  if (UseAdobeAPI)
    srcPtr = &tmpBuffer[0];
  else
    srcPtr = **theBuffer;
  destPtr = (char *) &insamp[0];

  abort = 0;
  i = buffer_size - buffer_pointer;
  j = frame_size << 1;
  if (i > j)
    i = j;
  memcpy(&destPtr[0], &srcPtr[buffer_pointer], i);
  buffer_pointer += i;
  if (buffer_pointer >= buffer_size)
  {
    if (!fill_buffer())
      abort = 1;
  }
  if ((i < j) && !abort)
  {
    j -= i;
    memcpy(&destPtr[i], &srcPtr[buffer_pointer], j);
    buffer_pointer += j;
    if (buffer_pointer >= buffer_size)
      fill_buffer();
    i += j;
  }

  i = i >> 1;

  if (!i)
    return 0;

  while (i < frame_size)
    insamp[i++] = 0;

  return i;
}

/************************************************************************/
/*
/* get_audio()
/*
/* PURPOSE:  reads a frame of audio data from a file to the buffer,
/*   aligns the data for future processing, and separates the
/*   left and right channels
/*
/*  SEMANTICS:
/* Calls read_samples() to read a frame of audio data from filepointer
/* #musicin# to #insampl[]#.  The data is shifted to make sure the data
/* is centered for the 1024pt window to be used by the psychoacoustic model,
/* and to compensate for the 256 sample delay from the filter bank. For
/* stereo, the channels are also demultiplexed into #buffer[0][]# and
/* #buffer[1][]#
/*
/************************************************************************/

unsigned long get_audio(
short buffer[2][1152],
int stereo, int lay)
{
   int j;
   int samples_read;

   if (lay == 1){
      if(stereo == 2){ /* layer 1, stereo */
         samples_read = read_samples(768);
         for(j=0;j<448;j++) {
            if(j<64) {
               buffer[0][j] = buffer[0][j+384];
               buffer[1][j] = buffer[1][j+384];
            }
            else {
               buffer[0][j] = insamp[2*j-128];
               buffer[1][j] = insamp[2*j-127];
            }
         }
      }
      else { /* layer 1, mono */
         samples_read = read_samples(384);
         for(j=0;j<448;j++){
            if(j<64) {
               buffer[0][j] = buffer[0][j+384];
               buffer[1][j] = 0;
            }
            else {
               buffer[0][j] = insamp[j-64];
               buffer[1][j] = 0;
            }
         }
      }
   }
   else {
      if(stereo == 2){ /* layer 2 (or 3), stereo */
         samples_read = read_samples(2304);
         for(j=0;j<1152;j++) {
            buffer[0][j] = insamp[2*j];
            buffer[1][j] = insamp[2*j+1];
         }
      }
      else { /* layer 2 (or 3), mono */
         samples_read = read_samples(1152);
         for(j=0;j<1152;j++){
            buffer[0][j] = insamp[j];
            buffer[1][j] = 0;
         }
      }
   }
   return(samples_read);
}

static double ana_table[HAN_SIZE] = {
   0.000000000, -0.000000477, -0.000000477, -0.000000477,
  -0.000000477, -0.000000477, -0.000000477, -0.000000954,
  -0.000000954, -0.000000954, -0.000000954, -0.000001431,
  -0.000001431, -0.000001907, -0.000001907, -0.000002384,
  -0.000002384, -0.000002861, -0.000003338, -0.000003338,
  -0.000003815, -0.000004292, -0.000004768, -0.000005245,
  -0.000006199, -0.000006676, -0.000007629, -0.000008106,
  -0.000009060, -0.000010014, -0.000011444, -0.000012398,
  -0.000013828, -0.000014782, -0.000016689, -0.000018120,
  -0.000019550, -0.000021458, -0.000023365, -0.000025272,
  -0.000027657, -0.000030041, -0.000032425, -0.000034809,
  -0.000037670, -0.000040531, -0.000043392, -0.000046253,
  -0.000049591, -0.000052929, -0.000055790, -0.000059605,
  -0.000062943, -0.000066280, -0.000070095, -0.000073433,
  -0.000076771, -0.000080585, -0.000083923, -0.000087261,
  -0.000090599, -0.000093460, -0.000096321, -0.000099182,
   0.000101566,  0.000103951,  0.000105858,  0.000107288,
   0.000108242,  0.000108719,  0.000108719,  0.000108242,
   0.000106812,  0.000105381,  0.000102520,  0.000099182,
   0.000095367,  0.000090122,  0.000084400,  0.000077724,
   0.000069618,  0.000060558,  0.000050545,  0.000039577,
   0.000027180,  0.000013828, -0.000000954, -0.000017166,
  -0.000034332, -0.000052929, -0.000072956, -0.000093937,
  -0.000116348, -0.000140190, -0.000165462, -0.000191212,
  -0.000218868, -0.000247478, -0.000277042, -0.000307560,
  -0.000339031, -0.000371456, -0.000404358, -0.000438213,
  -0.000472546, -0.000507355, -0.000542164, -0.000576973,
  -0.000611782, -0.000646591, -0.000680923, -0.000714302,
  -0.000747204, -0.000779152, -0.000809669, -0.000838757,
  -0.000866413, -0.000891685, -0.000915051, -0.000935555,
  -0.000954151, -0.000968933, -0.000980854, -0.000989437,
  -0.000994205, -0.000995159, -0.000991821, -0.000983715,
   0.000971317,  0.000953674,  0.000930786,  0.000902653,
   0.000868797,  0.000829220,  0.000783920,  0.000731945,
   0.000674248,  0.000610352,  0.000539303,  0.000462532,
   0.000378609,  0.000288486,  0.000191689,  0.000088215,
  -0.000021458, -0.000137329, -0.000259876, -0.000388145,
  -0.000522137, -0.000661850, -0.000806808, -0.000956535,
  -0.001111031, -0.001269817, -0.001432419, -0.001597881,
  -0.001766682, -0.001937389, -0.002110004, -0.002283096,
  -0.002457142, -0.002630711, -0.002803326, -0.002974033,
  -0.003141880, -0.003306866, -0.003467083, -0.003622532,
  -0.003771782, -0.003914356, -0.004048824, -0.004174709,
  -0.004290581, -0.004395962, -0.004489899, -0.004570484,
  -0.004638195, -0.004691124, -0.004728317, -0.004748821,
  -0.004752159, -0.004737377, -0.004703045, -0.004649162,
  -0.004573822, -0.004477024, -0.004357815, -0.004215240,
  -0.004049301, -0.003858566, -0.003643036, -0.003401756,
   0.003134727,  0.002841473,  0.002521515,  0.002174854,
   0.001800537,  0.001399517,  0.000971317,  0.000515938,
   0.000033379, -0.000475883, -0.001011848, -0.001573563,
  -0.002161503, -0.002774239, -0.003411293, -0.004072189,
  -0.004756451, -0.005462170, -0.006189346, -0.006937027,
  -0.007703304, -0.008487225, -0.009287834, -0.010103703,
  -0.010933399, -0.011775017, -0.012627602, -0.013489246,
  -0.014358521, -0.015233517, -0.016112804, -0.016994476,
  -0.017876148, -0.018756866, -0.019634247, -0.020506859,
  -0.021372318, -0.022228718, -0.023074150, -0.023907185,
  -0.024725437, -0.025527000, -0.026310921, -0.027073860,
  -0.027815342, -0.028532982, -0.029224873, -0.029890060,
  -0.030526638, -0.031132698, -0.031706810, -0.032248020,
  -0.032754898, -0.033225536, -0.033659935, -0.034055710,
  -0.034412861, -0.034730434, -0.035007000, -0.035242081,
  -0.035435200, -0.035586357, -0.035694122, -0.035758972,
   0.035780907,  0.035758972,  0.035694122,  0.035586357,
   0.035435200,  0.035242081,  0.035007000,  0.034730434,
   0.034412861,  0.034055710,  0.033659935,  0.033225536,
   0.032754898,  0.032248020,  0.031706810,  0.031132698,
   0.030526638,  0.029890060,  0.029224873,  0.028532982,
   0.027815342,  0.027073860,  0.026310921,  0.025527000,
   0.024725437,  0.023907185,  0.023074150,  0.022228718,
   0.021372318,  0.020506859,  0.019634247,  0.018756866,
   0.017876148,  0.016994476,  0.016112804,  0.015233517,
   0.014358521,  0.013489246,  0.012627602,  0.011775017,
   0.010933399,  0.010103703,  0.009287834,  0.008487225,
   0.007703304,  0.006937027,  0.006189346,  0.005462170,
   0.004756451,  0.004072189,  0.003411293,  0.002774239,
   0.002161503,  0.001573563,  0.001011848,  0.000475883,
  -0.000033379, -0.000515938, -0.000971317, -0.001399517,
  -0.001800537, -0.002174854, -0.002521515, -0.002841473,
   0.003134727,  0.003401756,  0.003643036,  0.003858566,
   0.004049301,  0.004215240,  0.004357815,  0.004477024,
   0.004573822,  0.004649162,  0.004703045,  0.004737377,
   0.004752159,  0.004748821,  0.004728317,  0.004691124,
   0.004638195,  0.004570484,  0.004489899,  0.004395962,
   0.004290581,  0.004174709,  0.004048824,  0.003914356,
   0.003771782,  0.003622532,  0.003467083,  0.003306866,
   0.003141880,  0.002974033,  0.002803326,  0.002630711,
   0.002457142,  0.002283096,  0.002110004,  0.001937389,
   0.001766682,  0.001597881,  0.001432419,  0.001269817,
   0.001111031,  0.000956535,  0.000806808,  0.000661850,
   0.000522137,  0.000388145,  0.000259876,  0.000137329,
   0.000021458, -0.000088215, -0.000191689, -0.000288486,
  -0.000378609, -0.000462532, -0.000539303, -0.000610352,
  -0.000674248, -0.000731945, -0.000783920, -0.000829220,
  -0.000868797, -0.000902653, -0.000930786, -0.000953674,
   0.000971317,  0.000983715,  0.000991821,  0.000995159,
   0.000994205,  0.000989437,  0.000980854,  0.000968933,
   0.000954151,  0.000935555,  0.000915051,  0.000891685,
   0.000866413,  0.000838757,  0.000809669,  0.000779152,
   0.000747204,  0.000714302,  0.000680923,  0.000646591,
   0.000611782,  0.000576973,  0.000542164,  0.000507355,
   0.000472546,  0.000438213,  0.000404358,  0.000371456,
   0.000339031,  0.000307560,  0.000277042,  0.000247478,
   0.000218868,  0.000191212,  0.000165462,  0.000140190,
   0.000116348,  0.000093937,  0.000072956,  0.000052929,
   0.000034332,  0.000017166,  0.000000954, -0.000013828,
  -0.000027180, -0.000039577, -0.000050545, -0.000060558,
  -0.000069618, -0.000077724, -0.000084400, -0.000090122,
  -0.000095367, -0.000099182, -0.000102520, -0.000105381,
  -0.000106812, -0.000108242, -0.000108719, -0.000108719,
  -0.000108242, -0.000107288, -0.000105858, -0.000103951,
   0.000101566,  0.000099182,  0.000096321,  0.000093460,
   0.000090599,  0.000087261,  0.000083923,  0.000080585,
   0.000076771,  0.000073433,  0.000070095,  0.000066280,
   0.000062943,  0.000059605,  0.000055790,  0.000052929,
   0.000049591,  0.000046253,  0.000043392,  0.000040531,
   0.000037670,  0.000034809,  0.000032425,  0.000030041,
   0.000027657,  0.000025272,  0.000023365,  0.000021458,
   0.000019550,  0.000018120,  0.000016689,  0.000014782,
   0.000013828,  0.000012398,  0.000011444,  0.000010014,
   0.000009060,  0.000008106,  0.000007629,  0.000006676,
   0.000006199,  0.000005245,  0.000004768,  0.000004292,
   0.000003815,  0.000003338,  0.000003338,  0.000002861,
   0.000002384,  0.000002384,  0.000001907,  0.000001907,
   0.000001431,  0.000001431,  0.000000954,  0.000000954,
   0.000000954,  0.000000954,  0.000000477,  0.000000477,
   0.000000477,  0.000000477,  0.000000477,  0.000000477};

/************************************************************************/
/*
/* window_subband()
/*
/* PURPOSE:  Overlapping window on PCM samples
/*
/* SEMANTICS:
/* 32 16-bit pcm samples are scaled to fractional 2's complement and
/* concatenated to the end of the window buffer #x#. The updated window
/* buffer #x# is then windowed by the analysis window #c# to produce the
/* windowed sample #z#
/*
/************************************************************************/

typedef double XX[2][HAN_SIZE];
static XX x;
static int init1, init2, init3, init4, init5, init6, off[2];
static int banc, berr;

void encode_init()
{
  banc = 32;
  berr = 0;
  init1 = 0;
  init2 = 0;
  init3 = 0;
  init4 = 0;
  init5 = 0;
  init6 = 0;
  off[0] = 0;
  off[1] = 0;
}

void window_subband(
short **buffer,
double z[HAN_SIZE],
int k)
{
    int i, j;
    if (!init1) {
        for (i=0;i<2;i++)
            for (j=0;j<HAN_SIZE;j++)
                x[i][j] = 0;
        init1 = 1;
    }

    /* replace 32 oldest samples with 32 new samples */
    for (i=0;i<32;i++) x[k][31-i+off[k]] = (double) *(*buffer)++/SCALE;
    /* shift samples into proper window positions */
    for (i=0;i<HAN_SIZE;i++) z[i] = x[k][(i+off[k])&HAN_SIZE-1] * ana_table[i];
    off[k] += 480;              /*offset is modulo (HAN_SIZE-1)*/
    off[k] &= HAN_SIZE-1;
}

/************************************************************************/
/*
/* create_ana_filter()
/*
/* PURPOSE:  Calculates the analysis filter bank coefficients
/*
/* SEMANTICS:
/* Calculates the analysis filterbank coefficients and rounds to the
/* 9th decimal place accuracy of the filterbank tables in the ISO
/* document.  The coefficients are stored in #filter#
/*
/************************************************************************/
 
void create_ana_filter(
double filter[SBLIMIT][64])
{
   register int i,k;

   for (i=0; i<32; i++)
      for (k=0; k<64; k++) {
          if ((filter[i][k] = 1e9*cos((double)((2*i+1)*(16-k)*PI64))) >= 0)
             modf(filter[i][k]+0.5, &filter[i][k]);
          else
             modf(filter[i][k]-0.5, &filter[i][k]);
          filter[i][k] *= 1e-9;
   }
}

/************************************************************************/
/*
/* filter_subband()
/*
/* PURPOSE:  Calculates the analysis filter bank coefficients
/*
/* SEMANTICS:
/*      The windowed samples #z# is filtered by the digital filter matrix #m#
/* to produce the subband samples #s#. This done by first selectively
/* picking out values from the windowed samples, and then multiplying
/* them by the filter matrix, producing 32 subband samples.
/*
/************************************************************************/

typedef double MM[SBLIMIT][64];
static MM m;

void filter_subband(
double z[HAN_SIZE], double s[SBLIMIT])
{
   double y[64];
   int i,j;
   if (!init2) {
       create_ana_filter(m);
       init2 = 1;
   }
   for (i=0;i<64;i++) for (j=0, y[i] = 0;j<8;j++) y[i] += z[i+64*j];
   for (i=0;i<SBLIMIT;i++)
       for (j=0, s[i]= 0;j<64;j++) s[i] += m[i][j] * y[j];
}

/************************************************************************/
/*
/* encode_info()
/*
/* PURPOSE:  Puts the syncword and header information on the output
/* bitstream.
/*
/************************************************************************/

void encode_info(frame_params *fr_ps)
{
  layer *info = fr_ps->header;

  putbits(&audiobs, 0xfff,12);                    /* syncword 12 bits */
  put1bit(&audiobs, info->version);                /* ID        1 bit  */
  putbits(&audiobs, 4-info->lay,2);               /* layer     2 bits */
  put1bit(&audiobs, !info->error_protection);     /* bit set => no err prot */
  putbits(&audiobs, info->bitrate_index,4);
  putbits(&audiobs, info->sampling_frequency,2);
  put1bit(&audiobs, info->padding);
  put1bit(&audiobs, info->extension);             /* private_bit */
  putbits(&audiobs, info->mode,2);
  putbits(&audiobs, info->mode_ext,2);
  put1bit(&audiobs, info->copyright);
  put1bit(&audiobs, info->original);
  putbits(&audiobs, info->emphasis,2);
}

/************************************************************************/
/*
/* mod()
/*
/* PURPOSE:  Returns the absolute value of its argument
/*
/************************************************************************/

double mod(
double a)
{
    return (a > 0) ? a : -a;
}

/************************************************************************/
/*
/* I_combine_LR    (Layer I)
/* II_combine_LR   (Layer II)
/*
/* PURPOSE:Combines left and right channels into a mono channel
/*
/* SEMANTICS:  The average of left and right subband samples is put into
/* #joint_sample#
/*
/* Layer I and II differ in frame length and # subbands used
/*
/************************************************************************/
 
void I_combine_LR(
double sb_sample[2][3][SCALE_BLOCK][SBLIMIT],
double joint_sample[3][SCALE_BLOCK][SBLIMIT])
{   /* make a filtered mono for joint stereo */
    int sb, smp;

   for(sb = 0; sb<SBLIMIT; ++sb)
      for(smp = 0; smp<SCALE_BLOCK; ++smp)
        joint_sample[0][smp][sb] = .5 *
                    (sb_sample[0][0][smp][sb] + sb_sample[1][0][smp][sb]);
}
 
void II_combine_LR(
double sb_sample[2][3][SCALE_BLOCK][SBLIMIT],
double joint_sample[3][SCALE_BLOCK][SBLIMIT],
int sblimit)
{  /* make a filtered mono for joint stereo */
   int sb, smp, sufr;

   for(sb = 0; sb<sblimit; ++sb)
      for(smp = 0; smp<SCALE_BLOCK; ++smp)
         for(sufr = 0; sufr<3; ++sufr)
            joint_sample[sufr][smp][sb] = .5 * (sb_sample[0][sufr][smp][sb]
                                           + sb_sample[1][sufr][smp][sb]);
}

/************************************************************************
/*
/* I_scale_factor_calc     (Layer I)
/* II_scale_factor_calc    (Layer II)
/*
/* PURPOSE:For each subband, calculate the scale factor for each set
/* of the 12 subband samples
/*
/* SEMANTICS:  Pick the scalefactor #multiple[]# just larger than the
/* absolute value of the peak subband sample of 12 samples,
/* and store the corresponding scalefactor index in #scalar#.
/*
/* Layer II has three sets of 12-subband samples for a given
/* subband.
/*
/************************************************************************/

void I_scale_factor_calc(
double sb_sample[][3][SCALE_BLOCK][SBLIMIT],
unsigned int scalar[][3][SBLIMIT],
int stereo)
{
   int i,j, k;
   double s[SBLIMIT];
 
   for (k=0;k<stereo;k++) {
     for (i=0;i<SBLIMIT;i++)
       for (j=1, s[i] = mod(sb_sample[k][0][0][i]);j<SCALE_BLOCK;j++)
         if (mod(sb_sample[k][0][j][i]) > s[i])
            s[i] = mod(sb_sample[k][0][j][i]);
 
     for (i=0;i<SBLIMIT;i++)
       for (j=SCALE_RANGE-2,scalar[k][0][i]=0;j>=0;j--) /* $A 6/16/92 */
         if (s[i] <= multiple[j]) {
            scalar[k][0][i] = j;
            break;
         }
   }
}

/******************************** Layer II ******************************/
 
#define PDS1
#ifdef PDS1
void II_scale_factor_calc(double sb_sample[][3][SCALE_BLOCK][SBLIMIT],
                       unsigned int scalar[][3][SBLIMIT],
                       int stereo,int sblimit)
{
  /* Optimized to use binary search instead of linear scan through the
     scalefactor table; guarantees to find scalefactor in only 5
     jumps/comparisons and not in {0 (lin. best) to 63 (lin. worst)}.
     Scalefactors for subbands > sblimit are no longer computed.
     Uses a single sblimit-loop.
     Patrick De Smet Oct 1999.
  */
  int k, t;
  /* Using '--' loops to avoid possible "cmp value + bne/beq" compiler  */
  /* inefficiencies. Below loops should compile to "bne/beq" only code  */
  for (k= stereo; k--; ) for (t=3; t-- ; )
      {
        int i;
        for (i=sblimit;  i-- ; )
          {
            int j;
            unsigned int l;
            register double temp;
            unsigned int scale_fac;
            /* Determination of max. over each set of 12 subband samples:  */
            /* PDS TODO: maybe this could/should ??!! be integrated into   */
            /* the subband filtering routines?                             */
            register double cur_max = mod(sb_sample[k][t][SCALE_BLOCK-1][i]);
            for ( j=SCALE_BLOCK-1; j-- ; )
              {
                if ( (temp = mod(sb_sample[k][t][j][i])) > cur_max)
                  cur_max = temp;
              }
            /* PDS: binary search in the scalefactor table: */
            /* This is the real speed up: */
            for (l =16, scale_fac=32; l ; l>>=1)
              {
                if (cur_max <= multiple[scale_fac])   scale_fac += l;
                else                                  scale_fac -= l;
              }
            if (cur_max > multiple[scale_fac])      scale_fac--;
            scalar[k][t][i] = scale_fac;
          }
      }
}

#else

void II_scale_factor_calc(
double sb_sample[][3][SCALE_BLOCK][SBLIMIT],
unsigned int scalar[][3][SBLIMIT],
int stereo, int sblimit)
{
  int i,j, k,t;
  double s[SBLIMIT];
 
  for (k=0;k<stereo;k++) for (t=0;t<3;t++) {
    for (i=0;i<sblimit;i++)
      for (j=1, s[i] = mod(sb_sample[k][t][0][i]);j<SCALE_BLOCK;j++)
        if (mod(sb_sample[k][t][j][i]) > s[i])
             s[i] = mod(sb_sample[k][t][j][i]);
 
  for (i=0;i<sblimit;i++)
    for (j=SCALE_RANGE-2,scalar[k][t][i]=0;j>=0;j--)    /* $A 6/16/92 */
      if (s[i] <= multiple[j]) {
         scalar[k][t][i] = j;
         break;
      }
      for (i=sblimit;i<SBLIMIT;i++) scalar[k][t][i] = SCALE_RANGE-1;
    }
}

#endif
/************************************************************************
/*
/* pick_scale  (Layer II)
/*
/* PURPOSE:For each subband, puts the Smallest scalefactor of the 3
/* associated with a frame into #max_sc#.  This is used
/* used by Psychoacoustic Model I.
/* (I would recommend changin max_sc to min_sc)
/*
/************************************************************************/

void pick_scale(
unsigned int scalar[2][3][SBLIMIT],
frame_params *fr_ps,
double max_sc[2][SBLIMIT])
{
  int i,j,k;
  unsigned int max;
  int stereo  = fr_ps->stereo;
  int sblimit = fr_ps->sblimit;

  for (k=0;k<stereo;k++)
    for (i=0;i<sblimit;max_sc[k][i] = multiple[max],i++)
      for (j=1, max = scalar[k][0][i];j<3;j++)
         if (max > scalar[k][j][i]) max = scalar[k][j][i];
  for (i=sblimit;i<SBLIMIT;i++) max_sc[0][i] = max_sc[1][i] = 1E-20;
}

/************************************************************************
/*
/* put_scale   (Layer I)
/*
/* PURPOSE:Sets #max_sc# to the scalefactor index in #scalar.
/* This is used by Psychoacoustic Model I
/*
/************************************************************************/

void put_scale(
unsigned int scalar[2][3][SBLIMIT],
frame_params *fr_ps,
double max_sc[2][SBLIMIT])
{
   int i,k;
   int stereo  = fr_ps->stereo;
//   int sblimit = fr_ps->sblimit;

   for (k=0;k<stereo;k++) for (i=0;i<SBLIMIT;i++)
        max_sc[k][i] = multiple[scalar[k][0][i]];
}

/************************************************************************
/*
/* II_transmission_pattern (Layer II only)
/*
/* PURPOSE:For a given subband, determines whether to send 1, 2, or
/* all 3 of the scalefactors, and fills in the scalefactor
/* select information accordingly
/*
/* SEMANTICS:  The subbands and channels are classified based on how much
/* the scalefactors changes over its three values (corresponding
/* to the 3 sets of 12 samples per subband).  The classification
/* will send 1 or 2 scalefactors instead of three if the scalefactors
/* do not change much.  The scalefactor select information,
/* #scfsi#, is filled in accordingly.
/*
/************************************************************************/

void II_transmission_pattern(
unsigned int scalar[2][3][SBLIMIT],
unsigned int scfsi[2][SBLIMIT],
frame_params *fr_ps)
{
   int stereo  = fr_ps->stereo;
   int sblimit = fr_ps->sblimit;
   int dscf[2];
   int clas[2],i,j,k;
static int pattern[5][5] = {{0x123, 0x122, 0x122, 0x133, 0x123},
                            {0x113, 0x111, 0x111, 0x444, 0x113},
                            {0x111, 0x111, 0x111, 0x333, 0x113},
                            {0x222, 0x222, 0x222, 0x333, 0x123},
                            {0x123, 0x122, 0x122, 0x133, 0x123}};

   for (k=0;k<stereo;k++)
     for (i=0;i<sblimit;i++) {
       dscf[0] =  (scalar[k][0][i]-scalar[k][1][i]);
       dscf[1] =  (scalar[k][1][i]-scalar[k][2][i]);
       for (j=0;j<2;j++) {
         if (dscf[j]<=-3) clas[j] = 0;
         else if (dscf[j] > -3 && dscf[j] <0) clas[j] = 1;
              else if (dscf[j] == 0) clas[j] = 2;
                   else if (dscf[j] > 0 && dscf[j] < 3) clas[j] = 3;
                        else clas[j] = 4;
       }
       switch (pattern[clas[0]][clas[1]]) {
         case 0x123 :    scfsi[k][i] = 0;
                         break;
         case 0x122 :    scfsi[k][i] = 3;
                         scalar[k][2][i] = scalar[k][1][i];
                         break;
         case 0x133 :    scfsi[k][i] = 3;
                         scalar[k][1][i] = scalar[k][2][i];
                         break;
         case 0x113 :    scfsi[k][i] = 1;
                         scalar[k][1][i] = scalar[k][0][i];
                         break;
         case 0x111 :    scfsi[k][i] = 2;
                         scalar[k][1][i] = scalar[k][2][i] = scalar[k][0][i];
                         break;
         case 0x222 :    scfsi[k][i] = 2;
                         scalar[k][0][i] = scalar[k][2][i] = scalar[k][1][i];
                         break;
         case 0x333 :    scfsi[k][i] = 2;
                         scalar[k][0][i] = scalar[k][1][i] = scalar[k][2][i];
                         break;
         case 0x444 :    scfsi[k][i] = 2;
                         if (scalar[k][0][i] > scalar[k][2][i])
                              scalar[k][0][i] = scalar[k][2][i];
                         scalar[k][1][i] = scalar[k][2][i] = scalar[k][0][i];
      }
   }
}

/************************************************************************
/*
/* I_encode_scale  (Layer I)
/* II_encode_scale (Layer II)
/*
/* PURPOSE:The encoded scalar factor information is arranged and
/* queued into the output fifo to be transmitted.
/*
/* For Layer II, the three scale factors associated with
/* a given subband and channel are transmitted in accordance
/* with the scfsi, which is transmitted first.
/*
/************************************************************************/

void I_encode_scale(
unsigned int scalar[2][3][SBLIMIT],
unsigned int bit_alloc[2][SBLIMIT],
frame_params *fr_ps)
{
   int stereo  = fr_ps->stereo;
//   int sblimit = fr_ps->sblimit;
   int i,j;

   for (i=0;i<SBLIMIT;i++) for (j=0;j<stereo;j++)
      if (bit_alloc[j][i])
        putbits(&audiobs, scalar[j][0][i],6);
}

/***************************** Layer II  ********************************/

void II_encode_scale(
unsigned int bit_alloc[2][SBLIMIT], unsigned int scfsi[2][SBLIMIT],
unsigned int scalar[2][3][SBLIMIT],
frame_params *fr_ps)
{
   int stereo  = fr_ps->stereo;
   int sblimit = fr_ps->sblimit;
//   int jsbound = fr_ps->jsbound;
   int i,j,k;

   for (i=0;i<sblimit;i++) for (k=0;k<stereo;k++)
     if (bit_alloc[k][i])
       putbits(&audiobs, scfsi[k][i],2);

   for (i=0;i<sblimit;i++) for (k=0;k<stereo;k++)
     if (bit_alloc[k][i])  /* above jsbound, bit_alloc[0][i] == ba[1][i] */
        switch (scfsi[k][i]) {
           case 0: for (j=0;j<3;j++)
                     putbits(&audiobs, scalar[k][j][i],6);
                   break;
           case 1:
           case 3: putbits(&audiobs, scalar[k][0][i],6);
                   putbits(&audiobs, scalar[k][2][i],6);
                   break;
           case 2: putbits(&audiobs, scalar[k][0][i],6);
        }
}
 
/*=======================================================================\
|                                                                        |
|      The following routines are done after the masking threshold       |
| has been calculated by the fft analysis routines in the Psychoacoustic |
| model. Using the MNR calculated, the actual number of bits allocated   |
| to each subband is found iteratively.                                  |
|                                                                        |
\=======================================================================*/
 
/************************************************************************
/*
/* I_bits_for_nonoise  (Layer I)
/* II_bits_for_nonoise (Layer II)
/*
/* PURPOSE:Returns the number of bits required to produce a
/* mask-to-noise ratio better or equal to the noise/no_noise threshold.
/*
/* SEMANTICS:
/* bbal = # bits needed for encoding bit allocation
/* bsel = # bits needed for encoding scalefactor select information
/* banc = # bits needed for ancillary data (header info included)
/*
/* For each subband and channel, will add bits until one of the
/* following occurs:
/* - Hit maximum number of bits we can allocate for that subband
/* - MNR is better than or equal to the minimum masking level
/*   (NOISY_MIN_MNR)
/* Then the bits required for scalefactors, scfsi, bit allocation,
/* and the subband samples are tallied (#req_bits#) and returned.
/*
/* (NOISY_MIN_MNR) is the Smallest MNR a subband can have before it is
/* counted as 'noisy' by the logic which chooses the number of JS
/* subbands.
/*
/* Joint stereo is supported.
/*
/************************************************************************/

static double snr[18] = {0.00, 7.00, 11.00, 16.00, 20.84,
                         25.28, 31.59, 37.75, 43.84,
                         49.89, 55.93, 61.96, 67.98, 74.01,
                         80.03, 86.05, 92.01, 98.01};

int I_bits_for_nonoise(
double perm_smr[2][SBLIMIT],
frame_params *fr_ps)
{
   int i,j,k;
   int stereo  = fr_ps->stereo;
//   int sblimit = fr_ps->sblimit;
   int jsbound = fr_ps->jsbound;
   int req_bits;

   /* initial b_anc (header) allocation bits */
   req_bits = 32 + 4 * ( (jsbound * stereo) + (SBLIMIT-jsbound) );

   for(i=0; i<SBLIMIT; ++i)
     for(j=0; j<((i<jsbound)?stereo:1); ++j) {
       for(k=0;k<14; ++k)
         if( (-perm_smr[j][i] + snr[k]) >= NOISY_MIN_MNR)
           break; /* we found enough bits */
         if(stereo == 2 && i >= jsbound)     /* check other JS channel */
           for(;k<14; ++k)
             if( (-perm_smr[1-j][i] + snr[k]) >= NOISY_MIN_MNR) break;
         if(k>0) req_bits += (k+1)*SCALE_BLOCK + 6*((i>=jsbound)?stereo:1);
   }
   return req_bits;
}

/***************************** Layer II  ********************************/

int II_bits_for_nonoise(
double perm_smr[2][SBLIMIT],
unsigned int scfsi[2][SBLIMIT],
frame_params *fr_ps)
{
   int sb,ch,ba;
   int stereo  = fr_ps->stereo;
   int sblimit = fr_ps->sblimit;
   int jsbound = fr_ps->jsbound;
   al_table *alloc = fr_ps->alloc;
   int req_bits, bbal = 0, berr1, banc1 = 32;
   int maxAlloc, sel_bits, sc_bits, smp_bits;
static int sfsPerScfsi[] = { 3,2,1,2 };    /* lookup # sfs per scfsi */

   /* added 92-08-11 shn */
   if (fr_ps->header->error_protection) berr1=16; else berr1=0;

   for (sb=0; sb<jsbound; ++sb)
     bbal += stereo * (*alloc)[sb][0].bits;
   for (sb=jsbound; sb<sblimit; ++sb)
     bbal += (*alloc)[sb][0].bits;
   req_bits = banc1 + bbal + berr1;

   for(sb=0; sb<sblimit; ++sb)
     for(ch=0; ch<((sb<jsbound)?stereo:1); ++ch) {
       maxAlloc = (1<<(*alloc)[sb][0].bits)-1;
//       smp_bits = 0;
       sel_bits = sc_bits = smp_bits = 0;

       for(ba=0;ba<maxAlloc-1; ++ba)
         if( (-perm_smr[ch][sb] + snr[(*alloc)[sb][ba].quant+((ba>0)?1:0)])
             >= NOISY_MIN_MNR)
            break;      /* we found enough bits */
       if(stereo == 2 && sb >= jsbound) /* check other JS channel */
         for(;ba<maxAlloc-1; ++ba)
           if( (-perm_smr[1-ch][sb]+ snr[(*alloc)[sb][ba].quant+((ba>0)?1:0)])
               >= NOISY_MIN_MNR)
             break;
       if(ba>0) {
         smp_bits = SCALE_BLOCK * ((*alloc)[sb][ba].group * (*alloc)[sb][ba].bits);
         /* scale factor bits required for subband */
         sel_bits = 2;
         sc_bits  = 6 * sfsPerScfsi[scfsi[ch][sb]];
         if(stereo == 2 && sb >= jsbound) {
           /* each new js sb has L+R scfsis */
           sel_bits += 2;
           sc_bits  += 6 * sfsPerScfsi[scfsi[1-ch][sb]];
         }
         req_bits += smp_bits+sel_bits+sc_bits;
       }
   }
   return req_bits;
}

/************************************************************************
/*
/* I_main_bit_allocation   (Layer I)
/* II_main_bit_allocation  (Layer II)
/*
/* PURPOSE:For joint stereo mode, determines which of the 4 joint
/* stereo modes is needed.  Then calls *_a_bit_allocation(), which
/* allocates bits for each of the subbands until there are no more bits
/* left, or the MNR is at the noise/no_noise threshold.
/*
/* SEMANTICS:
/*
/* For joint stereo mode, joint stereo is changed to stereo if
/* there are enough bits to encode stereo at or better than the
/* no-noise threshold (NOISY_MIN_MNR).  Otherwise, the system
/* iteratively allocates less bits by using joint stereo until one
/* of the following occurs:
/* - there are no more noisy subbands (MNR >= NOISY_MIN_MNR)
/* - mode_ext has been reduced to 0, which means that all but the
/*   lowest 4 subbands have been converted from stereo to joint
/*   stereo, and no more subbands may be converted
/*
/*     This function calls *_bits_for_nonoise() and *_a_bit_allocation().
/*
/************************************************************************/

void I_main_bit_allocation(
double perm_smr[2][SBLIMIT],
unsigned int bit_alloc[2][SBLIMIT],
int *adb,
frame_params *fr_ps)
{
   int  mode_ext, lay, i;
   int  rq_db; //, av_db = *adb;

   if(init3 == 0) {
     /* rearrange snr for layer I */
     snr[2] = snr[3];
     for (i=3;i<16;i++) snr[i] = snr[i+2];
     init3 = 1;
   }

   if((fr_ps->actual_mode) == MPG_MD_JOINT_STEREO) {
     fr_ps->header->mode = MPG_MD_STEREO;
     fr_ps->header->mode_ext = 0;
     fr_ps->jsbound = fr_ps->sblimit;
     if((rq_db = I_bits_for_nonoise(perm_smr, fr_ps)) > *adb) {
       fr_ps->header->mode = MPG_MD_JOINT_STEREO;
       mode_ext = 4;           /* 3 is least severe reduction */
       lay = fr_ps->header->lay;
       do {
          --mode_ext;
          fr_ps->jsbound = js_bound(lay, mode_ext);
          rq_db = I_bits_for_nonoise(perm_smr, fr_ps);
       } while( (rq_db > *adb) && (mode_ext > 0));
       fr_ps->header->mode_ext = mode_ext;
     }    /* well we either eliminated noisy sbs or mode_ext == 0 */
   }
   I_a_bit_allocation(perm_smr, bit_alloc, adb, fr_ps);
}

/***************************** Layer II  ********************************/

void II_main_bit_allocation(
double perm_smr[2][SBLIMIT],
unsigned int scfsi[2][SBLIMIT],
unsigned int bit_alloc[2][SBLIMIT],
int *adb,
frame_params *fr_ps)
{
   int  mode_ext, lay;
   int  rq_db; //, av_db = *adb;

   if((fr_ps->actual_mode) == MPG_MD_JOINT_STEREO) {
     fr_ps->header->mode = MPG_MD_STEREO;
     fr_ps->header->mode_ext = 0;
     fr_ps->jsbound = fr_ps->sblimit;
     if((rq_db=II_bits_for_nonoise(perm_smr, scfsi, fr_ps)) > *adb) {
       fr_ps->header->mode = MPG_MD_JOINT_STEREO;
       mode_ext = 4;           /* 3 is least severe reduction */
       lay = fr_ps->header->lay;
       do {
         --mode_ext;
         fr_ps->jsbound = js_bound(lay, mode_ext);
         rq_db = II_bits_for_nonoise(perm_smr, scfsi, fr_ps);
       } while( (rq_db > *adb) && (mode_ext > 0));
       fr_ps->header->mode_ext = mode_ext;
     }    /* well we either eliminated noisy sbs or mode_ext == 0 */
   }
   II_a_bit_allocation(perm_smr, scfsi, bit_alloc, adb, fr_ps);
}

/************************************************************************
/*
/* I_a_bit_allocation  (Layer I)
/* II_a_bit_allocation (Layer II)
/*
/* PURPOSE:Adds bits to the subbands with the lowest mask-to-noise
/* ratios, until the maximum number of bits for the subband has
/* been allocated.
/*
/* SEMANTICS:
/* 1. Find the subband and channel with the Smallest MNR (#min_sb#,
/*    and #min_ch#)
/* 2. Calculate the increase in bits needed if we increase the bit
/*    allocation to the next higher level
/* 3. If there are enough bits available for increasing the resolution
/*    in #min_sb#, #min_ch#, and the subband has not yet reached its
/*    maximum allocation, update the bit allocation, MNR, and bits
/*    available accordingly
/* 4. Repeat until there are no more bits left, or no more available
/*    subbands. (A subband is still available until the maximum
/*    number of bits for the subband has been allocated, or there
/*    aren't enough bits to go to the next higher resolution in the
/*    subband.)
/*
/************************************************************************/

int I_a_bit_allocation( /* return noisy sbs */
double perm_smr[2][SBLIMIT],
unsigned int bit_alloc[2][SBLIMIT],
int *adb,
frame_params *fr_ps)
{
   int i, k, smpl_bits, scale_bits, min_sb, min_ch, oth_ch;
   int bspl, bscf, ad, noisy_sbs, bbal ;
   double mnr[2][SBLIMIT], Small;
   char used[2][SBLIMIT];
   int stereo  = fr_ps->stereo;
//   int sblimit = fr_ps->sblimit;
   int jsbound = fr_ps->jsbound;
//   al_table *alloc = fr_ps->alloc;

   if (!init4) {
      init4 = 1;
      if (fr_ps->header->error_protection) berr = 16;  /* added 92-08-11 shn */
   }
   bbal = 4 * ( (jsbound * stereo) + (SBLIMIT-jsbound) );
   *adb -= bbal + berr + banc;
   ad= *adb;

   for (i=0;i<SBLIMIT;i++) for (k=0;k<stereo;k++) {
     mnr[k][i]=snr[0]-perm_smr[k][i];
     bit_alloc[k][i] = 0;
     used[k][i] = 0;
   }
   bspl = bscf = 0;

   do  {
     /* locate the subband with minimum SMR */
     Small = mnr[0][0]+1;    min_sb = -1; min_ch = -1;
     for (i=0;i<SBLIMIT;i++) for (k=0;k<stereo;k++)
       /* go on only if there are bits left */
       if (used[k][i] != 2 && Small > mnr[k][i]) {
         Small = mnr[k][i];
         min_sb = i;  min_ch = k;
       }
     if(min_sb > -1) {   /* there was something to find */
       /* first step of bit allocation is biggest */
       if (used[min_ch][min_sb])  { smpl_bits = SCALE_BLOCK; scale_bits = 0; }
       else                       { smpl_bits = 24; scale_bits = 6; }
       if(min_sb >= jsbound)        scale_bits *= stereo;

       /* check to see enough bits were available for */
       /* increasing resolution in the minimum band */
 
       if (ad >= bspl + bscf + scale_bits + smpl_bits) {
         bspl += smpl_bits; /* bit for subband sample */
         bscf += scale_bits; /* bit for scale factor */
         bit_alloc[min_ch][min_sb]++;
         used[min_ch][min_sb] = 1; /* subband has bits */
         mnr[min_ch][min_sb] = -perm_smr[min_ch][min_sb]
                               + snr[bit_alloc[min_ch][min_sb]];
         /* Check if subband has been fully allocated max bits */
         if (bit_alloc[min_ch][min_sb] ==  14 ) used[min_ch][min_sb] = 2;
       }
       else            /* no room to improve this band */
         used[min_ch][min_sb] = 2; /*   for allocation anymore */
       if(stereo == 2 && min_sb >= jsbound) {
         oth_ch = 1-min_ch;  /* joint-st : fix other ch */
         bit_alloc[oth_ch][min_sb] = bit_alloc[min_ch][min_sb];
         used[oth_ch][min_sb] = used[min_ch][min_sb];
         mnr[oth_ch][min_sb] = -perm_smr[oth_ch][min_sb]
                               + snr[bit_alloc[oth_ch][min_sb]];
       }
     }
   } while(min_sb>-1);     /* i.e. still some sub-bands to find */

   /* Calculate the number of bits left, add on to pointed var */
   ad -= bspl+bscf;
   *adb = ad;

   /* see how many channels are noisy */
   noisy_sbs = 0; Small = mnr[0][0];
   for(k=0; k<stereo; ++k) {
     for(i = 0; i< SBLIMIT; ++i) {
       if(mnr[k][i] < NOISY_MIN_MNR)   ++noisy_sbs;
       if(Small > mnr[k][i])           Small = mnr[k][i];
     }
   }
   return noisy_sbs;
}

/***************************** Layer II  ********************************/
 
int II_a_bit_allocation(
double perm_smr[2][SBLIMIT],
unsigned int scfsi[2][SBLIMIT],
unsigned int bit_alloc[2][SBLIMIT],
int *adb,
frame_params *fr_ps)
{
   int i, min_ch, min_sb, oth_ch, k, increment, scale, seli, ba;
   int bspl, bscf, bsel, ad, bbal=0;
   double mnr[2][SBLIMIT], Small;
   char used[2][SBLIMIT];
   int stereo  = fr_ps->stereo;
   int sblimit = fr_ps->sblimit;
   int jsbound = fr_ps->jsbound;
   al_table *alloc = fr_ps->alloc;
static int sfsPerScfsi[] = { 3,2,1,2 };    /* lookup # sfs per scfsi */

   if (!init5) {
       init5 = 1;
       if (fr_ps->header->error_protection) berr=16; /* added 92-08-11 shn */
   }
   for (i=0; i<jsbound; ++i)
     bbal += stereo * (*alloc)[i][0].bits;
   for (i=jsbound; i<sblimit; ++i)
     bbal += (*alloc)[i][0].bits;
   *adb -= bbal + berr + banc;
   ad = *adb;

   for (i=0;i<sblimit;i++) for (k=0;k<stereo;k++) {
     mnr[k][i]=snr[0]-perm_smr[k][i];
     bit_alloc[k][i] = 0;
     used[k][i] = 0;
   }
   bspl = bscf = bsel = 0;

   do  {
     /* locate the subband with minimum SMR */
     Small = 999999.0; min_sb = -1; min_ch = -1;
     for (i=0;i<sblimit;i++) for(k=0;k<stereo;++k)
       if (used[k][i]  != 2 && Small > mnr[k][i]) {
         Small = mnr[k][i];
         min_sb = i;  min_ch = k;
     }
     if(min_sb > -1) {   /* there was something to find */
       /* find increase in bit allocation in subband [min] */
       increment = SCALE_BLOCK * ((*alloc)[min_sb][bit_alloc[min_ch][min_sb]+1].group *
                        (*alloc)[min_sb][bit_alloc[min_ch][min_sb]+1].bits);
       if (used[min_ch][min_sb])
         increment -= SCALE_BLOCK * ((*alloc)[min_sb][bit_alloc[min_ch][min_sb]].group*
                           (*alloc)[min_sb][bit_alloc[min_ch][min_sb]].bits);

       /* scale factor bits required for subband [min] */
       oth_ch = 1 - min_ch;    /* above js bound, need both chans */
       if (used[min_ch][min_sb]) scale = seli = 0;
       else {          /* this channel had no bits or scfs before */
         seli = 2;
         scale = 6 * sfsPerScfsi[scfsi[min_ch][min_sb]];
         if(stereo == 2 && min_sb >= jsbound) {
           /* each new js sb has L+R scfsis */
           seli += 2;
           scale += 6 * sfsPerScfsi[scfsi[oth_ch][min_sb]];
         }
       }
       /* check to see enough bits were available for */
       /* increasing resolution in the minimum band */
       if (ad >= bspl + bscf + bsel + seli + scale + increment) {
         ba = ++bit_alloc[min_ch][min_sb]; /* next up alloc */
         bspl += increment;  /* bits for subband sample */
         bscf += scale;      /* bits for scale factor */
         bsel += seli;       /* bits for scfsi code */
         used[min_ch][min_sb] = 1; /* subband has bits */
         mnr[min_ch][min_sb] = -perm_smr[min_ch][min_sb] +
                               snr[(*alloc)[min_sb][ba].quant+1];
         /* Check if subband has been fully allocated max bits */
         if (ba >= (1<<(*alloc)[min_sb][0].bits)-1) used[min_ch][min_sb] = 2;
       }
       else used[min_ch][min_sb] = 2; /* can't increase this alloc */
       if(min_sb >= jsbound && stereo == 2) {
         /* above jsbound, alloc applies L+R */
         ba = bit_alloc[oth_ch][min_sb] = bit_alloc[min_ch][min_sb];
         used[oth_ch][min_sb] = used[min_ch][min_sb];
         mnr[oth_ch][min_sb] = -perm_smr[oth_ch][min_sb] +
                               snr[(*alloc)[min_sb][ba].quant+1];
       }
     }
   } while(min_sb > -1);   /* until could find no channel */
   /* Calculate the number of bits left */
   ad -= bspl+bscf+bsel;
   *adb = ad;
   for (k=0;k<stereo;k++) 
	 for (i=sblimit;i<SBLIMIT;i++)
	   bit_alloc[k][i]=0;

#ifdef VBRDEBUG
  noisy_sbs = 0; /* small = mnr[0][0]; Not Used MFC Nov 99 */      /* calc worst noise in case */
  for(k=0;k<stereo;++k)
    {
      for (i=0;i<sblimit;i++)
        {
          if(mnr[k][i] < NOISY_MIN_MNR)
            ++noisy_sbs; /* noise is not masked */
        }
    }
  return noisy_sbs;
#endif
  return 0;
}

/************************************************************************
/*
/* I_subband_quantization  (Layer I)
/* II_subband_quantization (Layer II)
/*
/* PURPOSE:Quantizes subband samples to appropriate number of bits
/*
/* SEMANTICS:  Subband samples are divided by their scalefactors, which
/* makes the quantization more efficient. The scaled samples are
/* quantized by the function a*x+b, where a and b are functions of
/* the number of quantization levels. The result is then truncated
/* to the appropriate number of bits and the MSB is inverted.
/*
/* Note that for fractional 2's complement, inverting the MSB for a
/* negative number x is equivalent to adding 1 to it.
/*
/************************************************************************/
 
static double a[17] = {
  0.750000000, 0.625000000, 0.875000000, 0.562500000, 0.937500000,
  0.968750000, 0.984375000, 0.992187500, 0.996093750, 0.998046875,
  0.999023438, 0.999511719, 0.999755859, 0.999877930, 0.999938965,
  0.999969482, 0.999984741 };

static double b[17] = {
  -0.250000000, -0.375000000, -0.125000000, -0.437500000, -0.062500000,
  -0.031250000, -0.015625000, -0.007812500, -0.003906250, -0.001953125,
  -0.000976563, -0.000488281, -0.000244141, -0.000122070, -0.000061035,
  -0.000030518, -0.000015259 };

#define PDS3
#ifdef PDS3

static unsigned int pds_quant_bits[17] = {
  /* for a number of quantization steps; */
  /*    3,     5,    7,    9,    15,
       31,    63,  127,  255,   511,
     1023,  2047, 4095, 8191, 16383,
    32767, 65535 
  */
  /* below we need : */
  2,      4,    4,    8,    8,
  16,    32,   64,  128,  256,
  512,  1024, 2048, 4096, 8192,
  16384, 32768
};
/* to retain succesfull quant */
/* This is only a quick and dirty tric to speed up ISO code */
/* In below quant routine : also rewrote loops to decrement */
/* Added/changed by Patrick De Smet, Nov. 1999 */

/* PDS TODO: maybe it is faster not to store pds_quant_bits */
/* but rather store (char) n, and use (1L shift left n) ;   */
/* is a shift faster than loading unsigned int from array ? */

void II_subband_quantization(unsigned int scalar[2][3][SBLIMIT],
                          double FAR sb_samples[2][3][SCALE_BLOCK][SBLIMIT],
                          unsigned int j_scale[3][SBLIMIT],
                          double FAR j_samps[3][SCALE_BLOCK][SBLIMIT],
                          unsigned int bit_alloc[2][SBLIMIT],
                          unsigned int FAR sbband[2][3][SCALE_BLOCK][SBLIMIT],
                          frame_params *fr_ps)
{
  int i, j, k, s, qnt, sig;
  int stereo  = fr_ps->stereo;
  int sblimit = fr_ps->sblimit;
  int jsbound = fr_ps->jsbound;
  double d;
  al_table *alloc = fr_ps->alloc;
  char tmpStr[80];
  
  for (s=3; s--; )
    for (j=SCALE_BLOCK ; j--; )
      for (i=sblimit; i--; )
        for (k= ( (i < jsbound) ? stereo : 1 ) ; k--; )
          if (bit_alloc[k][i])
            {
              /* scale and quantize floating point sample */
              if(stereo == 2 && i >= jsbound)    /* use j-stereo samples */
                d = j_samps[s][j][i] / multiple[j_scale[s][i]];
              else
                d = sb_samples[k][s][j][i] / multiple[scalar[k][s][i]];
              if (mod(d) > 1.0)
              {
                if (verbose)
                {
#ifdef _DEBUG
                  sprintf(tmpStr, "   Not scaled properly %d %d %d %d.",k,s,j,i);
                  DisplayInfo(tmpStr);
#endif
                }
              }

              qnt = (*alloc)[i][bit_alloc[k][i]].quant;
              d = d * a[qnt] + b[qnt];
              /* extract MSB N-1 bits from the floating point sample */
              if (d >= 0) sig = 1;
              else
                {
                  sig = 0;
                  d += 1.0;
                }
              sbband[k][s][j][i] = (unsigned int) (d * (double)
                                                   (pds_quant_bits[qnt]) );
              /* tag the inverted sign bit to sbband at position N */
              /* The bit inversion is a must for grouping with 3,5,9 steps
                 so it is done for all subbands */
              if (sig) sbband[k][s][j][i] |= (pds_quant_bits[qnt]);
            }
  for (s=3; s--; )
    for (j=sblimit; j < SBLIMIT; j++)
      for (i=SCALE_BLOCK; i--; )
        for (k=stereo; k--; ) sbband[k][s][i][j] = 0;

}
#else

/***************************** Layer II  ********************************/

void II_subband_quantization(
unsigned int scalar[2][3][SBLIMIT],
double sb_samples[2][3][SCALE_BLOCK][SBLIMIT],
unsigned int j_scale[3][SBLIMIT],
double j_samps[3][SCALE_BLOCK][SBLIMIT],
unsigned int bit_alloc[2][SBLIMIT],
unsigned int sbband[2][3][SCALE_BLOCK][SBLIMIT],
frame_params *fr_ps)
{
   int i, j, k, s, n, qnt, sig;
   int stereo  = fr_ps->stereo;
   int sblimit = fr_ps->sblimit;
   int jsbound = fr_ps->jsbound;
   unsigned int stps;
   double d;
   al_table *alloc = fr_ps->alloc;
   char tmpStr[80];

   for (s=0;s<3;s++)
     for (j=0;j<SCALE_BLOCK;j++)
       for (i=0;i<sblimit;i++)
         for (k=0;k<((i<jsbound)?stereo:1);k++)
           if (bit_alloc[k][i]) {
             /* scale and quantize floating point sample */
             if(stereo == 2 && i>=jsbound)       /* use j-stereo samples */
               d = j_samps[s][j][i] / multiple[j_scale[s][i]];
             else
               d = sb_samples[k][s][j][i] / multiple[scalar[k][s][i]];
             if (mod(d) > 1.0)
             {
               if (!quiet)
               {
#ifdef _DEBUG
                 sprintf(tmpStr, "   Not scaled properly %d %d %d %d.",k,s,j,i);
                 DisplayInfo(tmpStr);
#endif
               }
             }
             qnt = (*alloc)[i][bit_alloc[k][i]].quant;
             d = d * a[qnt] + b[qnt];
             /* extract MSB N-1 bits from the floating point sample */
             if (d >= 0) sig = 1;
             else { sig = 0; d += 1.0; }
             n = 0;
             stps = (*alloc)[i][bit_alloc[k][i]].steps;
             while ((1L<<n) < stps)
               n++;
             n--;
             sbband[k][s][j][i] = (unsigned int) (d * (double) (1L<<n));
             /* tag the inverted sign bit to sbband at position N */
             /* The bit inversion is a must for grouping with 3,5,9 steps
             so it is done for all subbands */
             if (sig) sbband[k][s][j][i] |= 1<<n;

           }
           for (k=0;k<stereo;k++)
             for (s=0;s<3;s++)
               for (i=0;i<SCALE_BLOCK;i++)
                 for (j=sblimit;j<SBLIMIT;j++)
                   sbband[k][s][i][j] = 0;
}

#endif

void I_subband_quantization(
unsigned int scalar[2][3][SBLIMIT],
double sb_samples[2][3][SCALE_BLOCK][SBLIMIT],
unsigned int j_scale[3][SBLIMIT],
double j_samps[3][SCALE_BLOCK][SBLIMIT], /* L+R for j-stereo if necess */
unsigned int bit_alloc[2][SBLIMIT],
unsigned int sbband[2][3][SCALE_BLOCK][SBLIMIT],
frame_params *fr_ps)
{
   int i, j, k, n, sig;
   int stereo  = fr_ps->stereo;
//   int sblimit = fr_ps->sblimit;
   int jsbound = fr_ps->jsbound;
   double d;

   if (!init6) {
     init6 = 1;
     /* rearrange quantization coef to correspond to layer I table */
     a[1] = a[2]; b[1] = b[2];
     for (i=2;i<15;i++) { a[i] = a[i+2]; b[i] = b[i+2]; }
   }
   for (j=0;j<SCALE_BLOCK;j++) for (i=0;i<SBLIMIT;i++)
     for (k=0;k<((i<jsbound)?stereo:1);k++)
       if (bit_alloc[k][i]) {
         /* for joint stereo mode, have to construct a single subband stream
            for the js channels.  At present, we calculate a set of mono
            subband samples and pass them through the scaling system to
            generate an alternate normalised sample stream.

            Could normalise both streams (divide by their scfs), then average
            them.  In bad conditions, this could give rise to spurious
            cancellations.  Instead, we could just select the sb stream from
            the larger channel (higher scf), in which case _that_ channel
            would be 'properly' reconstructed, and the mate would just be a
            scaled version.  Spec recommends averaging the two (unnormalised)
            subband channels, then normalising this new signal without
            actually sending this scale factor... This means looking ahead.
         */
         if(stereo == 2 && i>=jsbound)
           /* use the joint data passed in */
           d = j_samps[0][j][i] / multiple[j_scale[0][i]];
         else
           d = sb_samples[k][0][j][i] / multiple[scalar[k][0][i]];
         /* scale and quantize floating point sample */
         n = bit_alloc[k][i];
         d = d * a[n-1] + b[n-1];
         /* extract MSB N-1 bits from the floating point sample */
         if (d >= 0) sig = 1;
         else { sig = 0; d += 1.0; }
         sbband[k][0][j][i] = (unsigned int) (d * (double) (1L<<n));
         /* tag the inverted sign bit to sbband at position N */
         if (sig) sbband[k][0][j][i] |= 1<<n;
       }
}

/************************************************************************
/*
/* I_encode_bit_alloc  (Layer I)
/* II_encode_bit_alloc (Layer II)
/*
/* PURPOSE:Writes bit allocation information onto bitstream
/*
/* Layer I uses 4 bits/subband for bit allocation information,
/* and Layer II uses 4,3,2, or 0 bits depending on the
/* quantization table used.
/*
/************************************************************************/

void I_encode_bit_alloc(
unsigned int bit_alloc[2][SBLIMIT],
frame_params *fr_ps)
{
   int i,k;
   int stereo  = fr_ps->stereo;
//   int sblimit = fr_ps->sblimit;
   int jsbound = fr_ps->jsbound;

   for (i=0;i<SBLIMIT;i++)
     for (k=0;k<((i<jsbound)?stereo:1);k++)
       putbits(&audiobs, bit_alloc[k][i],4);
}

/***************************** Layer II  ********************************/

void II_encode_bit_alloc(
unsigned int bit_alloc[2][SBLIMIT],
frame_params *fr_ps)
{
   int i,k;
   int stereo  = fr_ps->stereo;
   int sblimit = fr_ps->sblimit;
   int jsbound = fr_ps->jsbound;
   al_table *alloc = fr_ps->alloc;

   for (i=0;i<sblimit;i++)
     for (k=0;k<((i<jsbound)?stereo:1);k++)
       putbits(&audiobs, bit_alloc[k][i],(*alloc)[i][0].bits);
}
 
/************************************************************************
/*
/* I_sample_encoding   (Layer I)
/* II_sample_encoding  (Layer II)
/*
/* PURPOSE:Put one frame of subband samples on to the bitstream
/*
/* SEMANTICS:  The number of bits allocated per sample is read from
/* the bit allocation information #bit_alloc#.  Layer 2
/* supports writing grouped samples for quantization steps
/* that are not a power of 2.
/*
/************************************************************************/

void I_sample_encoding(
unsigned int sbband[2][3][SCALE_BLOCK][SBLIMIT],
unsigned int bit_alloc[2][SBLIMIT],
frame_params *fr_ps)
{
   int i,j,k;
   int stereo  = fr_ps->stereo;
//   int sblimit = fr_ps->sblimit;
   int jsbound = fr_ps->jsbound;

   for(j=0;j<SCALE_BLOCK;j++) {
     for(i=0;i<SBLIMIT;i++)
       for(k=0;k<((i<jsbound)?stereo:1);k++)
         if(bit_alloc[k][i])
           putbits(&audiobs, sbband[k][0][j][i],bit_alloc[k][i]+1);
   }
}

/***************************** Layer II  ********************************/

void II_sample_encoding(
unsigned int sbband[2][3][SCALE_BLOCK][SBLIMIT],
unsigned int bit_alloc[2][SBLIMIT],
frame_params *fr_ps)
{
   unsigned int temp;
   int i,j,k,s,x,y;
   int stereo  = fr_ps->stereo;
   int sblimit = fr_ps->sblimit;
   int jsbound = fr_ps->jsbound;
   al_table *alloc = fr_ps->alloc;

   for (s=0;s<3;s++)
     for (j=0;j<SCALE_BLOCK;j+=3)
       for (i=0;i<sblimit;i++)
         for (k=0;k<((i<jsbound)?stereo:1);k++)
           if (bit_alloc[k][i]) {
             if ((*alloc)[i][bit_alloc[k][i]].group == 3) {
               for (x=0;x<3;x++)
                 putbits(&audiobs, sbband[k][s][j+x][i],(*alloc)[i][bit_alloc[k][i]].bits);
             }
             else {
               y =(*alloc)[i][bit_alloc[k][i]].steps;
               temp = sbband[k][s][j][i] +
                      sbband[k][s][j+1][i] * y +
                      sbband[k][s][j+2][i] * y * y;
               putbits(&audiobs, temp,(*alloc)[i][bit_alloc[k][i]].bits);
             }
           }
}

/************************************************************************
/*
/* encode_CRC
/*
/************************************************************************/

void encode_CRC(
unsigned int crc)
{
   putbits(&audiobs, crc, 16);
}
