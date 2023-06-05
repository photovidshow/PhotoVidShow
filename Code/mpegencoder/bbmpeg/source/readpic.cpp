/* readpic.c, read source pictures                                          */

/* Copyright (C) 1996, MPEG Software Simulation Group. All Rights Reserved. */

/*
 * Disclaimer of Warranty
 *
 * These software programs are available to the user without any license fee or
 * royalty on an "as is" basis.  The MPEG Software Simulation Group disclaims
 * any and all warranties, whether express, implied, or statuary, including any
 * implied warranties or merchantability or of fitness for a particular
 * purpose.  In no event shall the copyright-holder be liable for any
 * incidental, punitive, or consequential damages of any kind whatsoever
 * arising from the use of these programs.
 *
 * This disclaimer of warranty extends to the user of these programs and user's
 * customers, employees, agents, transferees, successors, and assigns.
 *
 * The MPEG Software Simulation Group does not represent or warrant that the
 * programs furnished hereunder are free of infringement of any third-party
 * patents.
 *
 * Commercial implementations of MPEG-1 and MPEG-2 video, including shareware,
 * are subject to royalty fees to patent holders.  Many of these patents are
 * general enough such that they are unavoidable regardless of implementation
 * design.
 *
 */
#define _CRT_SECURE_NO_WARNINGS 1
#include "main.h"

/* private prototypes */

static void conv444to422(unsigned char *src, unsigned char *dst);

static void conv422to420(unsigned char *src, unsigned char *dst);

static void border_extend(unsigned char *frame, int w1, int h1,
  int w2, int h2);

static unsigned char *u444, *v444, *u422, *v422;
static unsigned char *buffers[MAXN + 1][3];
static unsigned char *tmpPtrs[MAXN + 1][3];
static int prev_temp_ref, buffer_idx;
static int bufferstart;
static unsigned char *repeatBuffer;
static int saveRowBytes;
static int repeatStart;
static int repeatEnd;
static long rowbytes, buffer0size, buffer1size, buffer2size;
static void *buffer;

int init_readframe()
{
  int i;

  repeatBuffer = NULL;
  prev_temp_ref = 0;
  buffer_idx = 0;
  bufferstart = frame0 - N - 1;
  memset(buffers, 0, sizeof(buffers));
  buffer0size = width * height;
  switch (chroma_format)
  {
    case CHROMA444:
      buffer1size = buffer0size;
      break;
    case CHROMA422:
      buffer1size = (width >> 1) * height;
      break;
    default:
      buffer1size = (width >> 1) * (height >> 1);
      break;
  }
  buffer2size = buffer1size;

  for (i = 0; i <= N; i++)
  {
    buffers[i][0] = (unsigned char*)malloc(buffer0size);
    buffers[i][1] = (unsigned char*)malloc(buffer1size);
    buffers[i][2] = (unsigned char*)malloc(buffer2size);

    if (!buffers[i][0] || !buffers[i][1] || !buffers[i][2])
    {
      DisplayError("Could not allocate memory for picture buffers.");
      end_readframe();
      return FALSE;
    }
  }

  u444 = NULL;
  v444 = NULL;
  u422 = NULL;
  v422 = NULL;

  if (chroma_format != CHROMA444)
  {
    u444 = (unsigned char *)malloc(width * height);
    if (u444 == NULL)
    {
      DisplayError("Could not allocate memory for u444.");
      end_readframe();
      return FALSE;
    }
    v444 = (unsigned char *)malloc(width * height);
    if (v444 == NULL)
    {
      DisplayError("Could not allocate memory for v444.");
      end_readframe();
      return FALSE;
    }
    if (chroma_format==CHROMA420)
    {
      u422 = (unsigned char *)malloc((width >> 1) * height);
      if (u422 == NULL)
      {
        DisplayError("Could not allocate memory for u422.");
        end_readframe();
        return FALSE;
      }
      v422 = (unsigned char *)malloc((width >> 1) * height);
      if (v422 == NULL)
      {
        DisplayError("Could not allocate memory for v422.");
        end_readframe();
        return FALSE;
      }
    }
  }

  if (UseAdobeAPI)
  {
    repeatBuffer = (unsigned char*) malloc(input_horizontal_size * vertical_size * 4);
    if (!repeatBuffer)
    {
      DisplayError("Cannot allocate memory for repeat buffer.");
      end_readframe();
      return FALSE;
    }
    repeatCount = 0;
  }
  rowbytes = input_horizontal_size << 2;
  return TRUE;
}

void end_readframe()
{
  int i;

  for (i = 0; i <= N; i++)
  {
    if (buffers[i][0])
    {
      free(buffers[i][0]);
      buffers[i][0] = NULL;
    }
    if (buffers[i][1])
    {
      free(buffers[i][1]);
      buffers[i][1] = NULL;
    }
    if (buffers[i][2])
    {
      free(buffers[i][2]);
      buffers[i][2] = NULL;
    }
  }

  if (u444)
  {
    free(u444);
    u444 = NULL;
  }
  if (v444)
  {
    free(v444);
    v444 = NULL;
  }

  if (u422)
  {
    free(u422);
    u422 = NULL;
  }
  if (v422)
  {
    free(v422);
    v422 = NULL;
  }

  if (UseAdobeAPI)
  {
    if (repeatBuffer)
      free(repeatBuffer);
    repeatBuffer = NULL;
  }
}

static int getframe(int framenum)
{
  int i;
  char tmpStr[132];
  compGetFrameReturnRec	getFrameRet;

  if (UseAdobeAPI)
  {
    if ((repeatCount) && (framenum >= repeatStart) && (framenum <= repeatEnd))
    {
      repeatCount--;
      buffer = (unsigned char*)repeatBuffer; // + (vertical_size - 1) * saveRowBytes;
      rowbytes = saveRowBytes;
    }
    else
    {
      // Call the Adobe getFrame callback to get the pixels
      i = stdParmsSave->funcs->videoFuncs->getFrame(framenum, &buffer,
          &rowbytes, &getFrameRet, 0, doCompileRecSave->compileSeqID);

      // if we're done, we should reset the endframe number
      // mainly for FlaskMPEG
      // handle CompileAbort like CompileDone
      if ( (i == comp_CompileDone) || (i == comp_CompileAbort) )
      {
          // fixup endframe and number of frames
          doCompileRecSave->endFrame = framenum - 1;
          nframes = framenum - frame0;
          // display a less scary message
	  #ifdef _DEBUG
          sprintf(tmpStr, "   Video Encoding finished at frame # %d.", framenum - 1);
          DisplayInfo(tmpStr);
#endif
          return FALSE;
      }
      if (i != comp_ErrNone)
      {
  #ifdef _DEBUG
        sprintf(tmpStr, "  Could not get frame # %d, error code %d.", framenum, i);
        DisplayInfo(tmpStr);
#endif
        return FALSE;
      }
      repeatCount = getFrameRet.repeatCount;
      if (repeatCount)
      {
        repeatStart = framenum;
        repeatEnd = framenum + repeatCount - 1;
        memcpy(repeatBuffer, buffer, rowbytes * vertical_size);
        saveRowBytes = rowbytes;
      }
    }
  }
  else
  {
    // Call the general getVideo callback to get the pixels
    i = makeMPEGInfo.getVideo(framenum, (unsigned int*)&rowbytes, (char **)&buffer);
    if (i != bbErrNone)
    {
	#ifdef _DEBUG
      sprintf(tmpStr, "  Could not get frame # %d, error code %d.", framenum, i);
      DisplayInfo(tmpStr);
#endif
      return FALSE;
    }
  }
  return TRUE;
}

static int convertframe(unsigned char *frame[], int framenum)
{
  unsigned char *yp, *up, *vp;
  unsigned char *src, *prow;
  int i, j;
  int r, g, b;
  int y, u, v;
  double fy, fu, fv;
  int cyr, cyg, cyb, cur, cug, cub, cvr, cvg, cvb;
  double fcyr, fcyg, fcyb, fcur, fcug, fcub, fcvr, fcvg, fcvb;
  static int acoef[7][9] = {     /* alternate 32768 scaled table */
    {6963, 23442, 2363, -3768, -12648, 16384, 16384, -14877, -1507},
    {9798, 19235, 3736, -5538, -10846, 16384, 16384, -13730, -2654},
    {9798, 19235, 3736, -5538, -10846, 16384, 16384, -13730, -2654},
    {9830, 19333, 3604, -5538, -10846, 16384, 16384, -13795, -2589},
    {9798, 19235, 3736, -5538, -10846, 16384, 16384, -13730, -2654},
    {9798, 19235, 3736, -5538, -10846, 16384, 16384, -13730, -2654},
    {6947, 22970, 2851, -3801, -12583, 16384, 16384, -14582, -1802}};
  static double afycoef[7][3] = {
    {0.2125,0.7154,0.0721}, /* ITU-R Rec. 709 (1990) */
    {0.299, 0.587, 0.114},  /* unspecified */
    {0.299, 0.587, 0.114},  /* reserved */
    {0.30,  0.59,  0.11},   /* FCC */
    {0.299, 0.587, 0.114},  /* ITU-R Rec. 624-4 System B, G */
    {0.299, 0.587, 0.114},  /* SMPTE 170M */
    {0.212, 0.701, 0.087}}; /* SMPTE 240M (1987) */
  static double afucoef[7][3] = {
    {-0.115, -0.386, 0.500}, /* ITU-R Rec. 709 (1990) */
    {-0.169, -0.331, 0.500},  /* unspecified */
    {-0.169, -0.331, 0.500},  /* reserved */
    {-0.169, -0.331, 0.500},   /* FCC */
    {-0.169, -0.331, 0.500},  /* ITU-R Rec. 624-4 System B, G */
    {-0.169, -0.331, 0.500},  /* SMPTE 170M */
    {-0.116, -0.384, 0.500}}; /* SMPTE 240M (1987) */
  static double afvcoef[7][3] = {
    { 0.500, -0.454, -0.046}, /* ITU-R Rec. 709 (1990) */
    { 0.500, -0.419, -0.081},  /* unspecified */
    { 0.500, -0.419, -0.081},  /* reserved */
    { 0.500, -0.421, -0.079},   /* FCC */
    { 0.500, -0.419, -0.081},  /* ITU-R Rec. 624-4 System B, G */
    { 0.500, -0.419, -0.081},  /* SMPTE 170M */
    { 0.500, -0.445, -0.055}}; /* SMPTE 240M (1987) */


  if (!getframe(framenum))
    return FALSE;

  src = (unsigned char *) buffer + (vertical_size - 1) * rowbytes;

  if (chroma_format == CHROMA444)
  {
    u444 = frame[1];
    v444 = frame[2];
  }
  if (MMXMode && !UseFP)
  {
    switch (MMXMode)
    {
      case MODE_3DNOWEXT: // AMD 3DNOW extensions, use MMX for now
      case MODE_3DNOW:    // AMD 3DNOW, use MMX for now
      case MODE_SSE:      // Intel SIMD, use MMX for now
      case MODE_MMX:      // Intel MMX
        RGBtoYUVmmx(src, frame[0], u444, v444, rowbytes, width,
                    horizontal_size, vertical_size);
    }
  }
  else
  {
    i = matrix_coefficients;
    if (i>8)
      i = 3;

    if (UseFP)
    {
      fcyr = afycoef[i-1][0];
      fcyg = afycoef[i-1][1];
      fcyb = afycoef[i-1][2];
      fcur = afucoef[i-1][0];
      fcug = afucoef[i-1][1];
      fcub = afucoef[i-1][2];
      fcvr = afvcoef[i-1][0];
      fcvg = afvcoef[i-1][1];
      fcvb = afvcoef[i-1][2];
//      fcur = 0.5/(1.0-fcyb);
//      fcvr = 0.5/(1.0-fcyr);
      for (i=0; i<vertical_size; i++)
      {
        yp = frame[0] + i*width;
        up = u444 + i*width;
        vp = v444 + i*width;
        prow = src - (i * rowbytes);
        for (j=0; j<horizontal_size; j++)
        {
          b = *prow++;
          g = *prow++;
          r = *prow++;
          prow++;
          /* convert to YUV */
          /* floating point version */
          fy = fcyr*r + fcyg*g + fcyb*b;
          fu = fcur*r + fcug*g + fcub*b;
          fv = fcvr*r + fcvg*g + fcvb*b;
//          fu = fcur*(b-fy);
//          fv = fcvr*(r-fy);
          yp[j] = (unsigned char)((219.0/256.0)*fy + 16.5);  /* nominal range: 16..235 */
          up[j] = (unsigned char)((224.0/256.0)*fu + 128.5); /* 16..240 */
          vp[j] = (unsigned char)((224.0/256.0)*fv + 128.5); /* 16..240 */
        }
      }
    }
    else
    {
      cyr = acoef[i-1][0];
      cyg = acoef[i-1][1];
      cyb = acoef[i-1][2];
      cur = acoef[i-1][3];
      cug = acoef[i-1][4];
      cub = acoef[i-1][5];
      cvr = acoef[i-1][6];
      cvg = acoef[i-1][7];
      cvb = acoef[i-1][8];
//      cyr = coef[i-1][0];
//      cyg = coef[i-1][1];
//      cyb = coef[i-1][2];
//      cur = coef[i-1][3];
//      cvr = coef[i-1][4];
      for (i=0; i<vertical_size; i++)
      {
        yp = frame[0] + i*width;
        up = u444 + i*width;
        vp = v444 + i*width;
        prow = src - (i * rowbytes);
        for (j=0; j<horizontal_size; j++)
        {
          b = *prow++;
          g = *prow++;
          r = *prow++;
          prow++;
          /* convert to YUV */
          /* 32768 scaled version */
          y = cyr*r + cyg*g + cyb*b;
          u = cur*r + cug*g + cub*b;
          v = cvr*r + cvg*g + cvb*b;
          yp[j] = (((219 * y) >> 8) + 540672) >> 15;  /* nominal range: 16..235 */
          up[j] = (((224 * u) >> 8) + 4210688) >> 15; /* 16..240 */
          vp[j] = (((224 * v) >> 8) + 4210688) >> 15; /* 16..240 */
//          u = cur*(((b << 15) - y)>>15);
//          v = cvr*(((r << 15) - y)>>15);
//          yp[j] = (((219 * y) >> 8) + 540672) >> 15;  /* nominal range: 16..235 */
//          up[j] = ((u >> 8) + 4210688) >> 15; /* 16..240 */
//          vp[j] = ((v >> 8) + 4210688) >> 15; /* 16..240 */
        }
      }
    }
  }

  border_extend(frame[0],horizontal_size,vertical_size,width,height);
  if (!B_W)
  {
    border_extend(u444,horizontal_size,vertical_size,width,height);
    border_extend(v444,horizontal_size,vertical_size,width,height);

    if (chroma_format==CHROMA422)
    {
      conv444to422(u444,frame[1]);
      conv444to422(v444,frame[2]);
    }

    if (chroma_format==CHROMA420)
    {
      conv444to422(u444,u422);
      conv444to422(v444,v422);
      conv422to420(u422,frame[1]);
      conv422to420(v422,frame[2]);
    }
  }
  return TRUE;
}


int readframe(unsigned char *frame[], int framenum)
{
  int i, j, k;

  if (framenum > bufferstart + N)
  {
    j = framenum - (bufferstart + N);
    // save off the pointers we are shifting out
    for (i = 0; i < j; i++)
    {
      tmpPtrs[i][0] = buffers[i][0];
      tmpPtrs[i][1] = buffers[i][1];
      tmpPtrs[i][2] = buffers[i][2];
    }
    for (i = 0; i <= N - j; i++)
    {
      buffers[i][0] = buffers[i + j][0];
      buffers[i][1] = buffers[i + j][1];
      buffers[i][2] = buffers[i + j][2];
    }
    // put the saved pointers at the end
    for (k = 0; k < j; k++)
    {
      buffers[i][0] = tmpPtrs[k][0];
      buffers[i][1] = tmpPtrs[k][1];
      buffers[i][2] = tmpPtrs[k][2];
      i++;
    }
    bufferstart += j;
    for (i = 0; i < j; i++)
    {
      if (!convertframe(buffers[N - j + i + 1], framenum - j + i + 1))
        return FALSE;
    }
    i--;
    memcpy(frame[0], buffers[N - j + i + 1][0], buffer0size);
    if (B_W)
    {
      memset(frame[1], 128, buffer1size);
      memset(frame[2], 128, buffer2size);
    }
    else
    {
      memcpy(frame[1], buffers[N - j + i + 1][1], buffer1size);
      memcpy(frame[2], buffers[N - j + i + 1][2], buffer2size);
    }
  }
  else
  {
    if (framenum < bufferstart)
      bufferstart = framenum;
    else
    {
      i = framenum - bufferstart;
      memcpy(frame[0], buffers[i][0], buffer0size);
      if (B_W)
      {
        memset(frame[1], 128, buffer1size);
        memset(frame[2], 128, buffer2size);
      }
      else
      {
        memcpy(frame[1], buffers[i][1], buffer1size);
        memcpy(frame[2], buffers[i][2], buffer2size);
      }
    }
  }
  return TRUE;
}

static void border_extend(unsigned char *frame, int w1, int h1, int w2, int h2)
{
  int i, j;
  unsigned char *fp;

  /* horizontal pixel replication (right border) */

  for (j=0; j<h1; j++)
  {
    fp = frame + j*w2;
    for (i=w1; i<w2; i++)
      fp[i] = fp[i-1];
  }

  /* vertical pixel replication (bottom border) */

  for (j=h1; j<h2; j++)
  {
    fp = frame + j*w2;
    for (i=0; i<w2; i++)
      fp[i] = fp[i-w2];
  }
}

/* horizontal filter and 2:1 subsampling */
void conv444to422(unsigned char *src, unsigned char *dst)
{
  int i, j, w; //, im5, im4, im3, im2, im1, ip1, ip2, ip3, ip4, ip5, ip6;

  w = width >> 1;
  if (video_type < MPEG_MPEG2)
  {
    for (j=0; j<height; j++)
    {
      dst[0] = clp[(int)(228*(src[0]+src[1])
                         +70*(src[0]+src[2])
                         -37*(src[0]+src[3])
                         -21*(src[0]+src[4])
                         +11*(src[0]+src[5])
                         + 5*(src[0]+src[6])+256)>>9];

      dst[1] = clp[(int)(228*(src[2]+src[3])
                         +70*(src[1]+src[4])
                         -37*(src[0]+src[5])
                         -21*(src[0]+src[6])
                         +11*(src[0]+src[7])
                         + 5*(src[0]+src[8])+256)>>9];

      dst[2] = clp[(int)(228*(src[4]+src[5])
                         +70*(src[3]+src[6])
                         -37*(src[2]+src[7])
                         -21*(src[1]+src[8])
                         +11*(src[0]+src[9])
                         + 5*(src[0]+src[10])+256)>>9];

      for (i=6; i<width-6; i+=2)
      {
//      im5 = (i<5) ? 0 : i-5;
//      im4 = (i<4) ? 0 : i-4;
//      im3 = (i<3) ? 0 : i-3;
//      im2 = (i<2) ? 0 : i-2;
//      im1 = (i<1) ? 0 : i-1;
//      ip1 = (i<width-1) ? i+1 : width-1;
//      ip2 = (i<width-2) ? i+2 : width-1;
//      ip3 = (i<width-3) ? i+3 : width-1;
//      ip4 = (i<width-4) ? i+4 : width-1;
//      ip5 = (i<width-5) ? i+5 : width-1;
//      ip6 = (i<width-6) ? i+6 : width-1;

        /* FIR filter with 0.5 sample interval phase shift */
        dst[i>>1] = clp[(int)(228*(src[i]+src[i+1])
                         +70*(src[i-1]+src[i+2])
                         -37*(src[i-2]+src[i+3])
                         -21*(src[i-3]+src[i+4])
                         +11*(src[i-4]+src[i+5])
                         + 5*(src[i-5]+src[i+6])+256)>>9];
//        dst[i>>1] = clp[(int)(228*(src[i]+src[ip1])
//                         +70*(src[im1]+src[ip2])
//                         -37*(src[im2]+src[ip3])
//                         -21*(src[im3]+src[ip4])
//                         +11*(src[im4]+src[ip5])
//                         + 5*(src[im5]+src[ip6])+256)>>9];
      }
      dst[w-3] = clp[(int)(228*(src[width-6]+src[width-5])
                           +70*(src[width-7]+src[width-4])
                           -37*(src[width-8]+src[width-3])
                           -21*(src[width-9]+src[width-2])
                           +11*(src[width-10]+src[width-1])
                           + 5*(src[width-11]+src[width-1])+256)>>9];

      dst[w-2] = clp[(int)(228*(src[width-4]+src[width-3])
                           +70*(src[width-5]+src[width-2])
                           -37*(src[width-6]+src[width-1])
                           -21*(src[width-7]+src[width-1])
                           +11*(src[width-8]+src[width-1])
                           + 5*(src[width-9]+src[width-1])+256)>>9];

      dst[w-1] = clp[(int)(228*(src[width-2]+src[width-1])
                           +70*(src[width-3]+src[width-1])
                           -37*(src[width-4]+src[width-1])
                           -21*(src[width-5]+src[width-1])
                           +11*(src[width-6]+src[width-1])
                           + 5*(src[width-7]+src[width-1])+256)>>9];

      src+= width;
      dst+= width>>1;
    }
  }
  else
  {
    /* MPEG-2 */
    for (j=0; j<height; j++)
    {
      dst[0] = clp[(int)(  22*(src[0]+src[5])-52*(src[0]+src[3])
                       +159*(src[0]+src[1])+256*src[0]+256)>>9];

      dst[1] = clp[(int)(  22*(src[0]+src[7])-52*(src[0]+src[5])
                       +159*(src[1]+src[3])+256*src[2]+256)>>9];

      dst[2] = clp[(int)(  22*(src[0]+src[9])-52*(src[1]+src[7])
                       +159*(src[3]+src[5])+256*src[4]+256)>>9];

      for (i=6; i<width-6; i+=2)
      {
//        im5 = (i<5) ? 0 : i-5;
//        im3 = (i<3) ? 0 : i-3;
//        im1 = (i<1) ? 0 : i-1;
//        ip1 = (i<width-1) ? i+1 : width-1;
//        ip3 = (i<width-3) ? i+3 : width-1;
//        ip5 = (i<width-5) ? i+5 : width-1;

        /* FIR filter coefficients (*512): 22 0 -52 0 159 256 159 0 -52 0 22 */
        dst[i>>1] = clp[(int)(  22*(src[i-5]+src[i+5])-52*(src[i-3]+src[i+3])
                         +159*(src[i-1]+src[i+1])+256*src[i]+256)>>9];
//        dst[i>>1] = clp[(int)(  22*(src[im5]+src[ip5])-52*(src[im3]+src[ip3])
//                         +159*(src[im1]+src[ip1])+256*src[i]+256)>>9];
      }
      dst[w-3] = clp[(int)(  22*(src[width-11]+src[width-1])-52*(src[width-9]+src[width-3])
                      +159*(src[width-7]+src[width-5])+256*src[width-6]+256)>>9];

      dst[w-2] = clp[(int)(  22*(src[width-9]+src[width-1])-52*(src[width-7]+src[width-1])
                      +159*(src[width-5]+src[width-3])+256*src[width-4]+256)>>9];

      dst[w-1] = clp[(int)(  22*(src[width-7]+src[width-1])-52*(src[width-5]+src[width-1])
                      +159*(src[width-3]+src[width-1])+256*src[width-2]+256)>>9];

      src+= width;
      dst+= w;
//      dst+= width>>1;
    }
  }
}

/* vertical filter and 2:1 subsampling */
void conv422to420(unsigned char *src, unsigned char *dst)
{
  int w, h2, i, j; // , jm6, jm5, jm4, jm3, jm2, jm1;
//  int jp1, jp2, jp3, jp4, jp5, jp6;

  w = width>>1;
  h2 = height>>1;

  if (prog_frame)
  {
    /* intra frame */
    for (i=0; i<w; i++)
    {
      dst[0] = clp[(int)(228*(src[0]+src[w])
                         +70*(src[0]+src[w<<1])
                         -37*(src[0]+src[w*3])
                         -21*(src[0]+src[w<<2])
                         +11*(src[0]+src[w*5])
                         + 5*(src[0]+src[w*6])+256)>>9];

      dst[w] = clp[(int)(228*(src[w<<1]+src[w*3])
                         +70*(src[w*1]+src[w<<2])
                         -37*(src[0]+src[w*5])
                         -21*(src[0]+src[w*6])
                         +11*(src[0]+src[w*7])
                         + 5*(src[0]+src[w<<3])+256)>>9];

      dst[width] = clp[(int)(228*(src[w<<2]+src[w*5])
                             +70*(src[w*3]+src[w*6])
                             -37*(src[w<<1]+src[w*7])
                             -21*(src[w*1]+src[w<<3])
                             +11*(src[0]+src[w*9])
                             + 5*(src[0]+src[w*10])+256)>>9];


      for (j=6; j<height-6; j+=2)
      {
//        jm5 = (j<5) ? 0 : j-5;
//        jm4 = (j<4) ? 0 : j-4;
//        jm3 = (j<3) ? 0 : j-3;
//        jm2 = (j<2) ? 0 : j-2;
//        jm1 = (j<1) ? 0 : j-1;
//        jp1 = (j<height-1) ? j+1 : height-1;
//        jp2 = (j<height-2) ? j+2 : height-1;
//        jp3 = (j<height-3) ? j+3 : height-1;
//        jp4 = (j<height-4) ? j+4 : height-1;
//        jp5 = (j<height-5) ? j+5 : height-1;
//        jp6 = (j<height-6) ? j+6 : height-1;

        /* FIR filter with 0.5 sample interval phase shift */
        dst[w*(j>>1)] = clp[(int)(228*(src[w*j]+src[w*(j+1)])
                                  +70*(src[w*(j-1)]+src[w*(j+2)])
                                  -37*(src[w*(j-2)]+src[w*(j+3)])
                                  -21*(src[w*(j-3)]+src[w*(j+4)])
                                  +11*(src[w*(j-4)]+src[w*(j+5)])
                                  + 5*(src[w*(j-5)]+src[w*(j+6)])+256)>>9];
//        dst[w*(j>>1)] = clp[(int)(228*(src[w*j]+src[w*jp1])
//                             +70*(src[w*jm1]+src[w*jp2])
//                             -37*(src[w*jm2]+src[w*jp3])
//                             -21*(src[w*jm3]+src[w*jp4])
//                             +11*(src[w*jm4]+src[w*jp5])
//                             + 5*(src[w*jm5]+src[w*jp6])+256)>>9];
      }
      dst[w*(h2-3)] = clp[(int)(228*(src[w*(height-6)]+src[w*(height-5)])
                                +70*(src[w*(height-7)]+src[w*(height-4)])
                                -37*(src[w*(height-8)]+src[w*(height-3)])
                                -21*(src[w*(height-9)]+src[w*(height-2)])
                                +11*(src[w*(height-10)]+src[w*(height-1)])
                                + 5*(src[w*(height-11)]+src[w*(height-1)])+256)>>9];

      dst[w*(h2-2)] = clp[(int)(228*(src[w*(height-4)]+src[w*(height-3)])
                                +70*(src[w*(height-5)]+src[w*(height-2)])
                                -37*(src[w*(height-6)]+src[w*(height-1)])
                                -21*(src[w*(height-7)]+src[w*(height-1)])
                                +11*(src[w*(height-8)]+src[w*(height-1)])
                                + 5*(src[w*(height-9)]+src[w*(height-1)])+256)>>9];

      dst[w*(h2-1)] = clp[(int)(228*(src[w*(height-2)]+src[w*(height-1)])
                                +70*(src[w*(height-3)]+src[w*(height-1)])
                                -37*(src[w*(height-4)]+src[w*(height-1)])
                                -21*(src[w*(height-5)]+src[w*(height-1)])
                                +11*(src[w*(height-6)]+src[w*(height-1)])
                                + 5*(src[w*(height-7)]+src[w*(height-1)])+256)>>9];

      src++;
      dst++;
    }
  }
  else
  {
    /* intra field */
    for (i=0; i<w; i++)
    {

      dst[0] = clp[(int)(8*src[0] +5*src[0] -30*src[0]
                          -18*src[0] +113*src[0] +242*src[0]
                         +192*src[w<<1] +35*src[w<<2] -38*src[w*6]
                          -10*src[w<<3] +11*src[w*10] +2*src[w*12]+256)>>9];

      dst[w] = clp[(int)(8*src[w*13] +5*src[w*11] -30*src[w*9]
                              -18*src[w*7] +113*src[w*5] +242*src[w*3]
                             +192*src[w] +35*src[w] -38*src[w]
                              -10*src[w] +11*src[w] +2*src[w]+256)>>9];

      dst[w<<1] = clp[(int)(8*src[0] +5*src[0] -30*src[0]
                          -18*src[0] +113*src[w<<1] +242*src[w<<2]
                         +192*src[w*6] +35*src[w<<3] -38*src[w*10]
                          -10*src[w*12] +11*src[w*14] +2*src[w<<4]+256)>>9];

      dst[w*3] = clp[(int)(8*src[w*17] +5*src[w*15] -30*src[w*13]
                              -18*src[w*11] +113*src[w*9] +242*src[w*7]
                             +192*src[w*5] +35*src[w*3] -38*src[w]
                              -10*src[w] +11*src[w] +2*src[w]+256)>>9];

      dst[w<<2] = clp[(int)(8*src[0] +5*src[0] -30*src[w<<1]
                          -18*src[w<<2] +113*src[w*6] +242*src[w<<3]
                         +192*src[w*10] +35*src[w*12] -38*src[w*14]
                          -10*src[w<<4] +11*src[w*18] +2*src[w*20]+256)>>9];

      dst[w*5] = clp[(int)(8*src[w*21] +5*src[w*19] -30*src[w*17]
                              -18*src[w*15] +113*src[w*13] +242*src[w*11]
                             +192*src[w*9] +35*src[w*7] -38*src[w*5]
                              -10*src[w*3] +11*src[w] +2*src[w]+256)>>9];

      for (j=12; j<height-14; j+=4)
      {
        /* top field */
//        jm5 = (j<10) ? 0 : j-10;
//        jm4 = (j<8) ? 0 : j-8;
//        jm3 = (j<6) ? 0 : j-6;
//        jm2 = (j<4) ? 0 : j-4;
//        jm1 = (j<2) ? 0 : j-2;
//        jp1 = (j<height-2) ? j+2 : height-2;
//        jp2 = (j<height-4) ? j+4 : height-2;
//        jp3 = (j<height-6) ? j+6 : height-2;
//        jp4 = (j<height-8) ? j+8 : height-2;
//        jp5 = (j<height-10) ? j+10 : height-2;
//        jp6 = (j<height-12) ? j+12 : height-2;

        /* FIR filter with 0.25 sample interval phase shift */
        dst[w*(j>>1)] = clp[(int)(8*src[w*(j-10)] +5*src[w*(j-8)] -30*src[w*(j-6)]
                           -18*src[w*(j-4)] +113*src[w*(j-2)] +242*src[w*j]
                          +192*src[w*(j+2)] +35*src[w*(j+4)] -38*src[w*(j+6)]
                           -10*src[w*(j+8)] +11*src[w*(j+10)] +2*src[w*(j+12)]+256)>>9];
//        dst[w*(j>>1)] = clp[(int)(8*src[w*jm5] +5*src[w*jm4] -30*src[w*jm3]
//                           -18*src[w*jm2] +113*src[w*jm1] +242*src[w*j]
//                          +192*src[w*jp1] +35*src[w*jp2] -38*src[w*jp3]
//                           -10*src[w*jp4] +11*src[w*jp5] +2*src[w*jp6]+256)>>9];

        /* bottom field */
//        jm6 = (j<9) ? 1 : j-9;
//        jm5 = (j<7) ? 1 : j-7;
//        jm4 = (j<5) ? 1 : j-5;
//        jm3 = (j<3) ? 1 : j-3;
//        jm2 = (j<1) ? 1 : j-1;
//        jm1 = (j<height-1) ? j+1 : height-1;
//        jp1 = (j<height-3) ? j+3 : height-1;
//        jp2 = (j<height-5) ? j+5 : height-1;
//        jp3 = (j<height-7) ? j+7 : height-1;
//        jp4 = (j<height-9) ? j+9 : height-1;
//        jp5 = (j<height-11) ? j+11 : height-1;
//        jp6 = (j<height-13) ? j+13 : height-1;

        /* FIR filter with 0.25 sample interval phase shift */
        dst[w*((j>>1)+1)] = clp[(int)(8*src[w*(j+13)] +5*src[w*(j+11)]
                               -30*src[w*(j+9)] -18*src[w*(j+7)]
                              +113*src[w*(j+5)] +242*src[w*(j+3)]
                              +192*src[w*(j+1)] +35*src[w*(j-1)]
                               -38*src[w*(j-3)] -10*src[w*(j-5)]
                               +11*src[w*(j-7)] +2*src[w*(j-9)]+256)>>9];
//        dst[w*((j>>1)+1)] = clp[(int)(8*src[w*jp6] +5*src[w*jp5]
//                             -30*src[w*jp4] -18*src[w*jp3]
//                            +113*src[w*jp2] +242*src[w*jp1]
//                            +192*src[w*jm1] +35*src[w*jm2]
//                             -38*src[w*jm3] -10*src[w*jm4]
//                             +11*src[w*jm5] +2*src[w*jm6]+256)>>9];
      }



      dst[w*(h2-6)] = clp[(int)(8*src[w*(height-22)] +5*src[w*(height-20)] -30*src[w*(height-18)]
                              -18*src[w*(height-16)] +113*src[w*(height-14)] +242*src[w*(height-12)]
                         +192*src[w*(height-10)] +35*src[w*(height-8)] -38*src[w*(height-6)]
                          -10*src[w*(height-4)] +11*src[w*(height-2)] +2*src[w*(height-2)]+256)>>9];

      dst[w*(h2-5)] = clp[(int)(8*src[w*(height-1)] +5*src[w*(height-1)] -30*src[w*(height-3)]
                              -18*src[w*(height-5)] +113*src[w*(height-7)] +242*src[w*(height-9)]
                             +192*src[w*(height-11)] +35*src[w*(height-13)] -38*src[w*(height-15)]
                              -10*src[w*(height-17)] +11*src[w*(height-19)] +2*src[w*(height-21)]+256)>>9];

      dst[w*(h2-4)] = clp[(int)(8*src[w*(height-18)] +5*src[w*(height-16)] -30*src[w*(height-14)]
                          -18*src[w*(height-12)] +113*src[w*(height-10)] +242*src[w*(height-8)]
                         +192*src[w*(height-6)] +35*src[w*(height-4)] -38*src[w*(height-2)]
                          -10*src[w*(height-2)] +11*src[w*(height-2)] +2*src[w*(height-2)]+256)>>9];

      dst[w*(h2-3)] = clp[(int)(8*src[w*(height-1)] +5*src[w*(height-1)] -30*src[w*(height-1)]
                              -18*src[w*(height-1)] +113*src[w*(height-3)] +242*src[w*(height-5)]
                             +192*src[w*(height-7)] +35*src[w*(height-9)] -38*src[w*(height-11)]
                              -10*src[w*(height-13)] +11*src[w*(height-15)] +2*src[w*(height-17)]+256)>>9];

      dst[w*(h2-2)] = clp[(int)(8*src[w*(height-14)] +5*src[w*(height-12)] -30*src[w*(height-10)]
                          -18*src[w*(height-8)] +113*src[w*(height-6)] +242*src[w*(height-4)]
                         +192*src[w*(height-2)] +35*src[w*(height-2)] -38*src[w*(height-2)]
                          -10*src[w*(height-2)] +11*src[w*(height-2)] +2*src[w*(height-2)]+256)>>9];

      dst[w*(h2-1)] = clp[(int)(8*src[w*(height-1)] +5*src[w*(height-1)] -30*src[w*(height-1)]
                              -18*src[w*(height-1)] +113*src[w*(height-1)] +242*src[w*(height-1)]
                             +192*src[w*(height-3)] +35*src[w*(height-5)] -38*src[w*(height-7)]
                              -10*src[w*(height-9)] +11*src[w*(height-11)] +2*src[w*(height-13)]+256)>>9];



      src++;
      dst++;
    }
  }
}


