/* quantize.c, quantization / inverse quantization                          */

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

#include "main.h"

extern void iquant1_intra(short *src, short *dst,
  int dc_prec, unsigned char *quant_mat, int mquant);
extern void iquant1_non_intra(short *src, short *dst,
  unsigned char *quant_mat, int mquant);


// N.b I've editted out a lot of scaffolding in the original used for
// instrumentation and testing and to allow adjustment to mquant if
// saturation occurs even in the 32-bit C code...
// A mis-matched bracket or two may have crept in.
// If you're interested in the math I can email you the analysis....
// In gcc/egcs "abs" is a very fast compiler intrinsic (probably uses
// x86 conditional moves) samesign is the bit-hackery I posted
// on delphi...
//
//#include <math.h>

#define fabsshift ((8*sizeof(unsigned int))-1)
#define fastabs(x) (((x)-(((unsigned int)(x))>>fabsshift)) ^ ((x)>>fabsshift))
#define signmask(x) (((int)x)>>fabsshift)
#define samesign(x,y) (y+(signmask(x) & -(y<<1)))

int do_slow_quant_non_intra(
short *src, short *dst,
unsigned short *quant_mat,
unsigned short *i_quant_mat,
int mquant)
{
  int i;
  int x, y, d;
  int nzflag;
  int clipvalue  = (video_type < MPEG_MPEG2) ? 255 : 2047;

  /* MMX Quantizer maintains its own local buffer... dst will be unchanged if
        it flags saturation...
    */
 
  nzflag = 0;
  for (i = 0; i < 64; i++)
  {
    /* RJ: save one divide operation */
    /* AS: Lets make this a little more accurate... */
    x = abs(src[i]);
    d = quant_mat[i];
    /* N.b. accurate would be: y = (int)rint(32.0*((double)x)/((double)(d*2*mquant))); */
    /* Code below does *not* compute this always! */
	//y = (int)floor(32.0*((double)x)/((double)(d*2*mquant)));

    y = (32*abs(x) + (d>>1))/(d*2*mquant);
    if (y > clipvalue)
      y = clipvalue;
    nzflag |= (dst[i] = samesign(src[i],y));
  }

  return !!nzflag;
}

/*
// MPEG-2 inverse quantization 
void iquant_intra(
short *src, short *dst,
int dc_prec,
unsigned char *quant_mat,
int mquant)
{
  int i, val, sum;

  if (video_type < MPEG_MPEG2)
    iquant1_intra(src,dst,dc_prec,quant_mat,mquant);
  else
  {
    sum = dst[0] = src[0] << (3-dc_prec);
    for (i=1; i<64; i++)
    {
      val = (int)(src[i]*quant_mat[i]*mquant)/16;
      sum+= dst[i] = (val>2047) ? 2047 : ((val<-2048) ? -2048 : val);
    }

    // mismatch control 
    if ((sum&1)==0)
      dst[63]^= 1;
  }
}

void iquant_non_intra(
short *src, short *dst,
unsigned char *quant_mat,
int mquant)
{
  int i, val, sum;

  if (video_type < MPEG_MPEG2)
    iquant1_non_intra(src,dst,quant_mat,mquant);
  else
  {
    sum = 0;
    for (i=0; i<64; i++)
    {
      val = src[i];
      if (val!=0)
        val = (int)((2*val+(val>0 ? 1 : -1))*quant_mat[i]*mquant)/32;
      sum+= dst[i] = (val>2047) ? 2047 : ((val<-2048) ? -2048 : val);
    }

    // mismatch control 
    if ((sum&1)==0)
      dst[63]^= 1;
  }
}
*/


/*

// MPEG-1 inverse quantization 
static void iquant1_intra(
short *src, short *dst,
int dc_prec,
unsigned char *quant_mat,
int mquant)
{
  int i, val;

  dst[0] = src[0] << (3-dc_prec);
  for (i=1; i<64; i++)
  {
    val = (int)(src[i]*quant_mat[i]*mquant)/16;

    // mismatch control 
    if ((val&1)==0 && val!=0)
      val+= (val>0) ? -1 : 1;

    // saturation 
    dst[i] = (val>2047) ? 2047 : ((val<-2048) ? -2048 : val);
  }
}

static void iquant1_non_intra(
short *src, short *dst,
unsigned char *quant_mat,
int mquant)
{
  int i, val;

  for (i=0; i<64; i++)
  {
    val = src[i];
    if (val!=0)
    {
      val = (int)((2*val+(val>0 ? 1 : -1))*quant_mat[i]*mquant)/32;

      // mismatch control 
      if ((val&1)==0 && val!=0)
        val+= (val>0) ? -1 : 1;
    }

    // saturation 
    dst[i] = (val>2047) ? 2047 : ((val<-2048) ? -2048 : val);
  }
}
*/

