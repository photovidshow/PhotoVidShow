/**********************************************************************
Copyright (c) 1991 MPEG/audio software simulation group, All Rights Reserved
musicin.c
**********************************************************************/
/**********************************************************************
 * MPEG/audio coding/decoding software, work in progress              *
 *   NOT for public distribution until verified and approved by the   *
 *   MPEG/audio committee.  For further information, please contact   *
 *   Davis Pan, 508-493-2241, e-mail: pan@3d.enet.dec.com             *
 *                                                                    *
 * VERSION 4.0                                                        *
 *   changes made since last update:                                  *
 *   date   programmers         comment                               *
 * 3/01/91  Douglas Wong,       start of version 1.1 records          *
 *          Davis Pan                                                 *
 * 3/06/91  Douglas Wong,       rename: setup.h to endef.h            *
 *                              removed extraneous variables          *
 * 3/21/91  J.Georges Fritsch   introduction of the bit-stream        *
 *                              package. This package allows you      *
 *                              to generate the bit-stream in a       *
 *                              binary or ascii format                *
 * 3/31/91  Bill Aspromonte     replaced the read of the SB matrix    *
 *                              by an "code generated" one            *
 * 5/10/91  W. Joseph Carter    Ported to Macintosh and Unix.         *
 *                              Incorporated Jean-Georges Fritsch's   *
 *                              "bitstream.c" package.                *
 *                              Modified to strictly adhere to        *
 *                              encoded bitstream specs, including    *
 *                              "Berlin changes".                     *
 *                              Modified user interface dialog & code *
 *                              to accept any input & output          *
 *                              filenames desired.  Also added        *
 *                              de-emphasis prompt and final bail-out *
 *                              opportunity before encoding.          *
 *                              Added AIFF PCM sound file reading     *
 *                              capability.                           *
 *                              Modified PCM sound file handling to   *
 *                              process all incoming samples and fill *
 *                              out last encoded frame with zeros     *
 *                              (silence) if needed.                  *
 *                              Located and fixed numerous software   *
 *                              bugs and table data errors.           *
 * 27jun91  dpwe (Aware Inc)    Used new frame_params struct.         *
 *                              Clear all automatic arrays.           *
 *                              Changed some variable names,          *
 *                              simplified some code.                 *
 *                              Track number of bits actually sent.   *
 *                              Fixed padding slot, stereo bitrate    *
 *                              Added joint-stereo : scales L+R.      *
 * 6/12/91  Earle Jennings      added fix for MS_DOS in obtain_param  *
 * 6/13/91  Earle Jennings      added stack length adjustment before  *
 *                              main for MS_DOS                       *
 * 7/10/91  Earle Jennings      conversion of all float to FLOAT      *
 *                              port to MsDos from MacIntosh completed*
 * 8/ 8/91  Jens Spille         Change for MS-C6.00                   *
 * 8/22/91  Jens Spille         new obtain_parameters()               *
 *10/ 1/91  S.I. Sudharsanan,   Ported to IBM AIX platform.           *
 *          Don H. Lee,                                               *
 *          Peter W. Farrett                                          *
 *10/ 3/91  Don H. Lee          implemented CRC-16 error protection   *
 *                              newly introduced functions are        *
 *                              I_CRC_calc, II_CRC_calc and encode_CRC*
 *                              Additions and revisions are marked    *
 *                              with "dhl" for clarity                *
 *11/11/91 Katherine Wang       Documentation of code.                *
 *                                (variables in documentation are     *
 *                                surround by the # symbol, and an '*'*
 *                                denotes layer I or II versions)     *
 * 2/11/92  W. Joseph Carter    Ported new code to Macintosh.  Most   *
 *                              important fixes involved changing     *
 *                              16-bit ints to long or unsigned in    *
 *                              bit alloc routines for quant of 65535 *
 *                              and passing proper function args.     *
 *                              Removed "Other Joint Stereo" option   *
 *                              and made bitrate be total channel     *
 *                              bitrate, irrespective of the mode.    *
 *                              Fixed many small bugs & reorganized.  *
 * 2/25/92  Masahiro Iwadare    made code cleaner and more consistent *
 * 8/07/92  Mike Coleman        make exit() codes return error status *
 *                              made slight changes for portability   *
 *19 aug 92 Soren H. Nielsen    Changed MS-DOS file name extensions.  *
 * 8/25/92  Shaun Astarabadi    Replaced rint() function with explicit*
 *                              rounding for portability with MSDOS.  *
 * 9/22/92  jddevine@aware.com  Fixed _scale_factor_calc() calls.     *
 *10/19/92  Masahiro Iwadare    added info->mode and info->mode_ext   *
 *                              updates for AIFF format files         *
 * 3/10/93  Kevin Peterson      In parse_args, only set non default   *
 *                              bit rate if specified in arg list.    *
 *                              Use return value from aiff_read_hdrs  *
 *                              to fseek to start of sound data       *
 * 7/26/93  Davis Pan           fixed bug in printing info->mode_ext  *
 *                              value for joint stereo condition      *
 * 8/27/93 Seymour Shlien,      Fixes in Unix and MSDOS ports,        *
 *         Daniel Lauzon, and                                         *
 *         Bill Truerniet                                             *
 **********************************************************************/

#include "main.h"
#include "consts1.h"
#include <dos.h>

/* Implementations */

/************************************************************************
/*
/* main
/*
/* PURPOSE:  MPEG I Encoder supporting layers 1 and 2, and
/* psychoacoustic models 1 (MUSICAM) and 2 (AT&T)
/*
/* SEMANTICS:  One overlapping frame of audio of up to 2 channels are
/* processed at a time in the following order:
/* (associated routines are in parentheses)
/*
/* 1.  Filter sliding window of data to get 32 subband
/* samples per channel.
/* (window_subband,filter_subband)
/*
/* 2.  If joint stereo mode, combine left and right channels
/* for subbands above #jsbound#.
/* (*_combine_LR)
/*
/* 3.  Calculate scalefactors for the frame, and if layer 2,
/* also calculate scalefactor select information.
/* (*_scale_factor_calc)
/*
/* 4.  Calculate psychoacoustic masking levels using selected
/* psychoacoustic model.
/* (*_Psycho_One, psycho_anal)
/*
/* 5.  Perform iterative bit allocation for subbands with low
/* mask_to_noise ratios using masking levels from step 4.
/* (*_main_bit_allocation)
/*
/* 6.  If error protection flag is active, add redundancy for
/* error protection.
/* (*_CRC_calc)
/*
/* 7.  Pack bit allocation, scalefactors, and scalefactor select
/* information (layer 2) onto bitstream.
/* (*_encode_bit_alloc,*_encode_scale,II_transmission_pattern)
/*
/* 8.  Quantize subbands and pack them into bitstream
/* (*_subband_quantization, *_sample_encoding)
/*
/************************************************************************/

// ******************************************************************************************
void DebugOutputMessage(char *fmt, ...)
{
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
	}
#endif

}


int doaudio()
{
typedef double SBS[2][3][SCALE_BLOCK][SBLIMIT];
    SBS         *sb_sample;
typedef double JSBS[3][SCALE_BLOCK][SBLIMIT];
    JSBS        *j_sample;
typedef double INS[2][HAN_SIZE];
    INS          *win_que;
typedef unsigned int SUB[2][3][SCALE_BLOCK][SBLIMIT];
    SUB         *subband;

    frame_params fr_ps;
    layer info;
    short **win_buf;
static short buffer[2][1152];
static unsigned int bit_alloc[2][SBLIMIT], scfsi[2][SBLIMIT];
static unsigned int scalar[2][3][SBLIMIT], j_scale[3][SBLIMIT];
static double FAR ltmin[2][SBLIMIT], lgmin[2][SBLIMIT], max_sc[2][SBLIMIT];
    double snr32[32];
    short sam[2][1344]; // was [1056]
    int whole_SpF, extra_slot = 0;
    double avg_slots_per_frame, frac_SpF, slot_lag;
static unsigned int crc;
    int i, j, k, adb, stereo;
    int code;
    unsigned long bitsPerSlot, samplesPerFrame, frameNum = 0;
    unsigned long frameBits;
    double sentBits = 0.0;
    char tmpStr[MAXPATH];

    sb_sample = NULL;
    j_sample = NULL;
    win_que = NULL;
    subband = NULL;
    win_buf = NULL;
    code = FALSE;

    if (!init_putbits(&audiobs, AudioFilename))
      return FALSE;

    if (!init_read_samples())
      goto exit1;

    tonal_init();
//    subs_init();
    encode_init();
    psy_init();

  switch (audioSampleRate)
  {
    case 48000:
      info.sampling_frequency = 1;
      break;
    case 44100:
      info.sampling_frequency = 0;
      break;
    case 32000:
      info.sampling_frequency = 2;
      break;
    default:
      //DisplayError("Unknown audio sampling rate");
      goto exit1;
  }

#ifdef _DEBUG
  DebugOutputMessage("!!!audioSampleRate= %d",audioSampleRate);
#endif 

  /* Most large variables are declared dynamically to ensure
     compatibility with smaller machines */

  sb_sample = (SBS FAR *) malloc(sizeof(SBS));
  j_sample = (JSBS FAR *) malloc(sizeof(JSBS));
  win_que = (INS FAR *) malloc(sizeof(INS));
  subband = (SUB FAR *) malloc(sizeof(SUB));
  win_buf = (short FAR **) malloc(sizeof(short *)*2);

  if (!sb_sample || !j_sample || !win_que || !subband || !win_buf)
  {
    //DisplayError("Could not allocate memory for audio buffers.");
    goto exit1;
  }

  /* clear buffers */
  memset((char *) buffer, 0, sizeof(buffer));
  memset((char *) bit_alloc, 0, sizeof(bit_alloc));
  memset((char *) scalar, 0, sizeof(scalar));
  memset((char *) j_scale, 0, sizeof(j_scale));
  memset((char *) scfsi, 0, sizeof(scfsi));
  memset((char *) ltmin, 0, sizeof(ltmin));
  memset((char *) lgmin, 0, sizeof(lgmin));
  memset((char *) max_sc, 0, sizeof(max_sc));
  memset((char *) snr32, 0, sizeof(snr32));
  memset((char *) sam, 0, sizeof(sam));

  fr_ps.header = &info;
  fr_ps.tab_num = -1;             /* no table loaded */
  fr_ps.alloc = NULL;
  info.version = MPEG_AUDIO_ID;

  info.lay = audio_layer;
  info.mode = audio_mode;
//  if (audio_mode != MPG_MD_JOINT_STEREO)
    info.mode_ext = 0;

  info.bitrate_index = audio_bitrate;
  info.emphasis = emphasis;
  info.extension = extension;
  info.error_protection = error_protection;
  info.copyright = copyright;
  info.original = original;

  hdr_to_frps(&fr_ps);
  stereo = fr_ps.stereo;

    if (info.lay == 1) { bitsPerSlot = 32; samplesPerFrame = 384;  }
    else               { bitsPerSlot = 8;  samplesPerFrame = 1152; }
    /* Figure average number of 'slots' per frame. */
    /* Bitrate means TOTAL for both channels, not per side. */
    avg_slots_per_frame = ((double)samplesPerFrame /
                           s_freq[info.sampling_frequency]) *
                          ((double)bitrate[info.lay-1][info.bitrate_index] /
                           (double)bitsPerSlot);
    whole_SpF = (int) avg_slots_per_frame;
    if (verbose)
    {
#ifdef _DEBUG
      sprintf(tmpStr, "   slots/frame = %d.",whole_SpF);
      DisplayInfo(tmpStr);
#endif
    }

    frac_SpF  = avg_slots_per_frame - (double)whole_SpF;
    slot_lag  = -frac_SpF;
    if (verbose)
    {
#ifdef _DEBUG
      sprintf(tmpStr, "   frac SpF=%.3f, tot bitrate=%d kbps, s freq=%.1f kHz.",
           frac_SpF, bitrate[info.lay-1][info.bitrate_index],
           s_freq[info.sampling_frequency]);
      DisplayInfo(tmpStr);
#endif
    }

    if (frac_SpF != 0)
    {
      if (verbose)
      {
#ifdef _DEBUG
        sprintf(tmpStr, "   Fractional number of slots, padding required.");
        DisplayInfo(tmpStr);
#endif
      }
    }
    else info.padding = 0;

    while (get_audio(buffer, stereo, info.lay) > 0)
    {
      YieldTime();
      if (AbortMPEG)
        goto exit1;

      if (FileOutError)
      {
#ifdef _DEBUG
        sprintf(tmpStr, "Could not write to file %s", AudioFilename);
        DisplayError(tmpStr);
#endif
        goto exit1;
      }

      frameNum++;

       win_buf[0] = &buffer[0][0];
       win_buf[1] = &buffer[1][0];
       if (frac_SpF != 0) {
          if (slot_lag > (frac_SpF-1.0) ) {
             slot_lag -= frac_SpF;
             extra_slot = 0;
             info.padding = 0;
             /*  printf("No padding for this frame\n"); */
          }
          else {
             extra_slot = 1;
             info.padding = 1;
             slot_lag += (1-frac_SpF);
             /*  printf("Padding for this frame\n");    */
          }
       }
       adb = (whole_SpF+extra_slot) * bitsPerSlot;

       switch (info.lay) {

/***************************** Layer I **********************************/

          case 1 :
             for (j=0;j<SCALE_BLOCK;j++)
             for (k=0;k<stereo;k++) {
                window_subband(&win_buf[k], &(*win_que)[k][0], k);
                filter_subband(&(*win_que)[k][0], &(*sb_sample)[k][0][j][0]);
             }

             I_scale_factor_calc(*sb_sample, scalar, stereo);
             if(fr_ps.actual_mode == MPG_MD_JOINT_STEREO) {
                I_combine_LR(*sb_sample, *j_sample);
                I_scale_factor_calc(*sb_sample, scalar, 1);
//                I_scale_factor_calc(j_sample, &j_scale, 1);
             }

             put_scale(scalar, &fr_ps, max_sc);

             if (psych_model == 1) I_Psycho_One(buffer, max_sc, ltmin, &fr_ps);
             else {
                for (k=0;k<stereo;k++) {
                   psycho_anal(&buffer[k][0],&sam[k][0], k, info.lay, snr32,
                               (double)s_freq[info.sampling_frequency]*1000);
                   for (i=0;i<SBLIMIT;i++) ltmin[k][i] = (double) snr32[i];
                }
             }

             I_main_bit_allocation(ltmin, bit_alloc, &adb, &fr_ps);

             if (error_protection) I_CRC_calc(&fr_ps, bit_alloc, &crc);

             encode_info(&fr_ps);

             if (error_protection)
               encode_CRC(crc);

             I_encode_bit_alloc(bit_alloc, &fr_ps);
             I_encode_scale(scalar, bit_alloc, &fr_ps);
             I_subband_quantization(scalar, *sb_sample, j_scale, *j_sample,
                                    bit_alloc, *subband, &fr_ps);
             I_sample_encoding(*subband, bit_alloc, &fr_ps);
             for (i=0;i<adb;i++)
              put1bit(&audiobs, 0);

          break;

/***************************** Layer 2 **********************************/

          case 2 :
             for (i=0;i<3;i++) for (j=0;j<SCALE_BLOCK;j++)
                for (k=0;k<stereo;k++) {
                   window_subband(&win_buf[k], &(*win_que)[k][0], k);
                   filter_subband(&(*win_que)[k][0], &(*sb_sample)[k][i][j][0]);
                }

                II_scale_factor_calc(*sb_sample, scalar, stereo, fr_ps.sblimit);
                pick_scale(scalar, &fr_ps, max_sc);
                if(fr_ps.actual_mode == MPG_MD_JOINT_STEREO) {
                   II_combine_LR(*sb_sample, *j_sample, fr_ps.sblimit);
                   II_scale_factor_calc(j_sample, &j_scale, 1, fr_ps.sblimit);
                }       /* this way we calculate more mono than we need */
                        /* but it is cheap */

                if (psych_model == 1)
                  II_Psycho_One(buffer, max_sc, ltmin, &fr_ps);
                else {
                   for (k=0;k<stereo;k++) {
                      psycho_anal(&buffer[k][0],&sam[k][0], k,
                                 info.lay, snr32,
                                 (double)s_freq[info.sampling_frequency]*1000);
                      for (i=0;i<SBLIMIT;i++) ltmin[k][i] = (double) snr32[i];
                   }
                }
                II_transmission_pattern(scalar, scfsi, &fr_ps);
                II_main_bit_allocation(ltmin, scfsi, bit_alloc, &adb, &fr_ps);

                if (error_protection)
                   II_CRC_calc(&fr_ps, bit_alloc, scfsi, &crc);
                encode_info(&fr_ps);
                if (error_protection)
                  encode_CRC(crc);
                II_encode_bit_alloc(bit_alloc, &fr_ps);
                II_encode_scale(bit_alloc, scfsi, scalar, &fr_ps);
                II_subband_quantization(scalar, *sb_sample, j_scale,
                                      *j_sample, bit_alloc, *subband, &fr_ps);
                II_sample_encoding(*subband, bit_alloc, &fr_ps);
                for (i=0;i<adb;i++)
                  put1bit(&audiobs, 0);
          break;

/***************************** Layer 3 **********************************/

          case 3 : break;

       }

       frameBits = (unsigned long) (bitcount(&audiobs) - sentBits);
       if(frameBits%bitsPerSlot)   /* a program failure */
       {
#ifdef _DEBUG
          sprintf(tmpStr,"   Sent %ld bits = %ld slots plus %ld\n",
                  frameBits, frameBits/bitsPerSlot,
                  frameBits%bitsPerSlot);
          DisplayInfo(tmpStr);
#endif
       }
       sentBits += frameBits;

    }

#ifdef _DEBUG
    sprintf(tmpStr, "   Avg slots/frame = %.3f; b/smp = %.2f; br = %.3f kbps.",
           sentBits / (frameNum * bitsPerSlot),
           sentBits / (frameNum * samplesPerFrame),
           sentBits / (frameNum * samplesPerFrame) *
           s_freq[info.sampling_frequency]);
    DisplayInfo(tmpStr);
#endif

    code = TRUE;

exit1:
  finish_putbits(&audiobs);
  if (sb_sample)
    free(sb_sample);
  if (j_sample)
    free(j_sample);
  if (win_que)
    free(win_que);
  if (subband)
    free(subband);
  if (win_buf)
    free(win_buf);

  return code;
}


