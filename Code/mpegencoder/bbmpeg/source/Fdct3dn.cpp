/* fdct3dn_extracomments.c, forward discrete cosine transform, 3D-Now! implementation
 *
 * -----------------------------------
 * All 3D-Now instructions converted to "_EMIT " primitives
 * (thanks to Microsoft Visual C++ preprocesor compile option "/P")
 * -----------------------------------
 *
 * This software is a derivative work of the MPEG Software Simulation Group.
 * Use of this software is restricted by the distribution rights granted
 * by the MPEG Software Simulation Group.
 * 
 *
 * AAN float-DCT (single-precision) with 3D-Now acceleration
 * -----------------------------------------------------------------
 * This file contains a Visual C++ 6 source-code listing for a
 * 3D-Now implementation of the AAN-DCT algorithm.  The AAN algorithm
 * is one of several fast-approaches to perform a discrete-cosine 
 * transform on an 8x8 block of pixel-data.  This code was developed
 * and tested under Visual C++ 6.0 Professional Edition.  You will
 * need the "amd3dx.h" header-file from AMD's SDK to compile this
 * code as-is.
 *
 * The 3D-Now implementation was converted from BBMPEG's floating
 * point AAN-DCT.  The 3D-Now implementation is mathematically identical
 * except for 2 differences :
 * 
 *         (1) 3D-Now implementation is single-precision (32-bit)
 *
 *         (2) 3D-Now implementation introduces a slight (1/32768)
 *             negative offset for the DCT output elements which
 *             are negative.  Please see the embedded code comments for
 *             more information.  The offset occurs during the final
 *             post-scaling/rounding step.  It can be eliminated at the
 *             expense of slower execution.  However, the amount of the
 *             offset is insignificant compared to the overall 
 *             systematic error of the AAN.
 *
 * The code-listing is is compatible with all 3D-Now capable processors 
 * (AMD K6/2 and later, IDT Winchip2, Cyrix M3.)  The code has only been 
 * thoroughly tested on an AMD K6/2-500.
 * 
 * Although the code is reasonably well optimized, not all optimization 
 * possibilities were implemented, as many of them would render
 * the listing virtually unreadable.  
 * Some segments could be shortened by using the DSP-extensions of 
 * the K7-Athlon ("3D-Now enhanced.")  Of particular interest are PFPNACC,
 * PSWAPD, and PSHUFW.
 *
 *
 * Revision history
 * ----------------
 *
 * v1.0  Single-precision floating point AAN-DCT (from BBMPEG)
 *       using the AMD 3D-Now! instruction-set.  Tested on AMD K6/2.
 *
 *
 *  liaor@iname.com v1.0 07/13/2000
 *
 * Other references/links:
 * -----------------------
 *
 *  BBMPEG (Brent Beyeler) : http://members.home.net/beyeler
 *  IEEE-1180 test info    : http://www.mpeg.org
 *  AMD SDK                : http://www.amd.com
 */

/* Original AAN-DCT from <mpeg2codec v1.2> copyright (C) 1996, 
   MPEG Software Simulation Group. All Rights Reserved. */

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

/* A 2-D DCT can be done by 1-D DCT on each row followed by 1-D DCT
 * on each column.  Direct algorithms are also available, but they are
 * much more complex and seem not to be any faster when reduced to code.
 *
 * This implementation is based on Arai, Agui, and Nakajima's algorithm for
 * scaled DCT.  Their original paper (Trans. IEICE E-71(11):1095) is in
 * Japanese, but the algorithm is described in the Pennebaker & Mitchell
 * JPEG textbook (see REFERENCES section in file README).  The following code
 * is based directly on figure 4-8 in P&M.
 * While an 8-point DCT cannot be done in less than 11 multiplies, it is
 * possible to arrange the computation so that many of the multiplies are
 * simple scalings of the final outputs.  These multiplies can then be
 * folded into the multiplications or divisions by the JPEG quantization
 * table entries.  The AA&N method leaves only 5 multiplies and 29 adds
 * to be done in the DCT itself.
 * The primary disadvantage of this method is that with a fixed-point
 * implementation, accuracy is lost due to imprecise representation of the
 * scaled quantization values.  However, that problem does not arise if
 * we use floating point arithmetic.
 */
#ifdef __BORLANDC__

#define EMIT db

#else

#define EMIT _emit

#endif

#include<math.h> // for floor() function
//#include "amd3dx.h" // macros-file from AMD 3D-Now SDK, http://www.amd.com

static float aanscales[64];

// PSCF_xxx is for postscaling/rounding operation.
// For 'accurate-rounding' (see C-code), use these alternate definitions
/*
#define PSCF_SCALE 1.0
#define PSCF_SHIFT 0
#define PSCF_MASK  0x0000000000000000 // [+0,+0] dword,dword
*/
#define PSCF_SCALE 32768.0
#define PSCF_SHIFT 15
#define PSCF_MASK  0x0000400000004000 // [+32768,+32768] dword,dword


#define NC_COS6      0.382683432365089771728459984030399//cos(6*pi/16)

#define NC_R_SQRT2   0.707106781186547524400844362104849// 1/sqrt(2)

#define NC_COS1SQRT2 1.38703984532214746182161919156644 //cos(1*pi/16)*sqrt(2)
#define NC_COS2SQRT2 1.30656296487637652785664317342719 //cos(2*pi/16)*sqrt(2)
#define NC_COS3SQRT2 1.17587560241935871697446710461126 //cos(3*pi/16)*sqrt(2)
#define NC_COS5SQRT2 0.785694958387102181277897367657217//cos(5*pi/16)*sqrt(2)
#define NC_COS6SQRT2 0.541196100146196984399723205366389//cos(6*pi/16)*sqrt(2)
#define NC_COS7SQRT2 0.275899379282943012335957563669373//cos(7*pi/16)*sqrt(2)


void init_fdct_3dnow()
{
  int i, j;
  static const double aansf[8] = {
    1.0, 
    NC_COS1SQRT2,
    NC_COS2SQRT2,
    NC_COS3SQRT2,
    1.0,  // cos(4*pi/16) * sqrt(2) = 1.0 exactly
    NC_COS5SQRT2,
    NC_COS6SQRT2,
    NC_COS7SQRT2
  };

  for (i = 0; i < 8; i++)
    for (j = 0; j < 8; j++)
      aanscales[(i<<3)+j] = (float)((double)(PSCF_SCALE) / (aansf[i]*aansf[j]*8.0));
//      aanscales[(i << 3) + j] = 1.0 / (aansf[i] * aansf[j] * 8.0);
}



void fdct_3dnow(short *block)
{
  static float data[64]; // temporary data array
  static __int64  tmp3tmp2, tmp0tmp1, tmp7tmp6, tmp4tmp5;
  static __int64  tmp10tmp11, tmp13tmp12, tmp15tmp14, tmp17tmp16;
//  static __int64  z2z3, z5z4;
  static const __int64 mmMask00001000 = 0x80000000; 
  static const __int64 mmMaskRnd = PSCF_MASK; // post "round-up" mask

  // multiplication constants for DCT processing
  static const float CONSTANTS[] = { 
      - (float)NC_R_SQRT2, (float)NC_R_SQRT2,     //-0.7071,0.7071 --> [ 0.7071,-0.7071 ]
        (float)NC_COS2SQRT2, (float)NC_COS6SQRT2, // 1.3066,0.5412 --> [ 0.5412, 1.3066 ]
        (float)NC_R_SQRT2, (float)NC_COS6         // 0.7071,0.3827 --> [ 0.3827, 0.7071 ]
  };

/*
  debugging variables 
  float *tmp0 = (float *) &(tmp0tmp1);
  float *tmp1 = (float *) &(tmp0tmp1);
  float *tmp2 =(float *) &(tmp3tmp2);
  float *tmp3 =(float *) &(tmp3tmp2);
  float *tmp4 =(float *) &(tmp4tmp5);
  float *tmp5 =(float *) &(tmp4tmp5);
  float *tmp6 =(float *) &(tmp7tmp6);
  float *tmp7 = (float *) &(tmp7tmp6);
  float *tmp10 = (float *) &(tmp10tmp11);
  float *tmp11 = (float *) &(tmp10tmp11);
  float *tmp12 = (float *) &(tmp13tmp12);
  float *tmp13 = (float *) &(tmp13tmp12);
  float *tmp14 = (float *) &(tmp15tmp14);
  float *tmp15 = (float *) &(tmp15tmp14);
  float *tmp16 = (float *) &(tmp17tmp16);
  float *z2    = (float *) &(z2z3);
  float *z3    = (float *) &(z2z3);
  float *z4    = (float *) &(z5z4);
  float *z5    = (float *) &(z5z4);
  float  z1, z11, z13;
*/
  static float  *dataptr, *dataptr2;
//  short *blkptr;
//  int i,j;

/*
  debugging pointer variables, generate proper address for the following
  pointers
  ++tmp0;
  ++tmp3;
  ++tmp4;
  ++tmp7;

  ++tmp10;
  ++tmp13;
  ++tmp15;

  ++z2;
  ++z5;
*/

  // The initialize routine init_fdct() must be called prior to
  // using this function!

  /*************************************************************
   *
   * Pass 1: process rows, transpose intermediate result
   *
   *************************************************************/
  // 
 
//  blkptr = block;
  dataptr = data;  
  dataptr2 = data;

//  for (i = 0; i < 8; i++)
//  {
    /*
    *tmp0 = blkptr[0] + blkptr[7];
    *tmp7 = blkptr[0] - blkptr[7];
    *tmp1 = blkptr[1] + blkptr[6];
    *tmp6 = blkptr[1] - blkptr[6];
    *tmp2 = blkptr[2] + blkptr[5];
    *tmp5 = blkptr[2] - blkptr[5];
    *tmp3 = blkptr[3] + blkptr[4];
    *tmp4 = blkptr[3] - blkptr[4];
*/
     __asm {
     //////////
     // tdn_dct_row1 computes the fDCT for 1 input-row.
     // tdn_dct_row1 transposes the output, so result is stored as column.
     // sio
     // source data is assumed



     mov eax, dword ptr [block]; 
      pxor mm7, mm7;              // mm7 <= 0x0000_0000_0000_0000

     mov edx, dword ptr [dataptr];// edx <= &dataptr[0]
      mov edi, 0x08;               // edi = 'i' // for ( i = 8; i > 0; i=i+1 )

     lea ebx, dword ptr [CONSTANTS];// ebx <= &CONSTANTS[0]

tdn_dct_row1: // 3d_now_dct_row1 loop-point

     movq mm5, qword ptr [eax];   // mm5 <= blkptr[i3_i2_i1_i0]
      pxor mm0, mm0;              // clear mm0

     movq mm6, qword ptr [eax+8]; // mm6 <= blkptr[i7_i6_i5_i4]
      pxor mm2, mm2;              // clear mm2

     movq mm1, mm5;               // mm1 <= blkptr[i3_i2_i1_i0]
      punpckhwd mm0, mm5;        // mm0 <= produce [i3,00,i2,00] (sword)

     movq mm3, mm6;               // mm3 <= blkptr[i7_i6_i5_i4]
      psrlq mm5, 16;              // mm5 <= blkptr[00_i3_i2_i1]
      
     punpckhwd mm2, mm6;         // mm2 <= produce [i7,00,i6,00] (sword)
      psrlq mm6, 16;              // mm6 <= blkptr[00_i7_i6_i5]

     punpcklwd mm5, mm1;          // mm5 <= [i1,i2,i0,i1] (sword)
      pxor mm1, mm1;              // clear mm1

     psrad mm0, 16;
      punpcklwd mm1, mm5;         // mm1 <= produce [i0,00,i1,00] (sword)

     punpcklwd mm6, mm3;          // mm6 <= [i5,i6,i4,i5] (sword)
      pxor mm3, mm3;              // clear mm3

     psrad mm2, 16;
      punpcklwd mm3, mm6;        // mm3 <= produce [i4,00,i5,00] (sword)

     psrad mm3, 16;

     add eax, 16;                // dataptr += 8 (short ints)
     psrad mm1, 16;

// 1a) [in7,in6] = [blkptr7, blkptr6]; // dword, dword
// 1b) [in4,in5] = [blkptr4, blkptr5]; //dword, dword
// 1c) [in0,in1] = [blkptr0, blkptr1]; // dword,dword
// 1d) [in3,in2] = [blkptr3, blkptr2]; //dword, dword

#define in7in6 mm2
#define in4in5 mm3
#define in0in1 mm1
#define in3in2 mm0
#define in0in1_2 mm4
#define in3in2_2 mm5

    movq in3in2_2, in3in2; // 2nd copy of in3in2
     movq in0in1_2, in0in1;  // 2nd copy of in0in1

// tmp0= inptr[0]                         +                         inptr[7];
// tmp1=         inptr[1]                 +                 inptr[6];

// tmp7= inptr[0]                         -                         inptr[7];
// tmp6=         inptr[1]                 -                 inptr[6];

// tmp3=                         inptr[3] + inptr[4];
// tmp2=                 inptr[2]         +         inptr[5];

// tmp4=                         inptr[3] - inptr[4];
// tmp5=                 inptr[2]         -         inptr[5];



#define t0t1 in0in1
#define t3t2 in3in2
#define t4t5 in3in2_2
#define t7t6 in0in1_2
//  st1_0 <= [tmp0,tmp1]  (float, float)
//  st1_1 <= [tmp7,tmp6]  (float, float)
//  st1_2 <= [tmp3,tmp2]  (float, float)
//  st1_3 <= [tmp4,tmp5]  (float, float)


     psubd t7t6, in7in6; // [tmp7,tmp6] <= [in0,in1] - [in7,in6] 
      psubd t4t5, in4in5; // [tmp4,tmp5] <= [in3,in2] - [in4,in5]

//     PI2FD( t7t6, t7t6);    // convert t7t6(int,int) -> t7t6(float,float)
     EMIT 0x0f
     EMIT 0x0f
     EMIT 0xe4 //((0xc4 & 0x3f) << 3) | 0xc4
     EMIT 0x0d

      paddd t3t2, in4in5;  // [tmp3,tmp2] <= [in3,in2] + [in4,in5]

//     PI2FD( t4t5, t4t5);   // convert t4t5(int,int) -> t4t5(float,float)
     EMIT 0x0f
     EMIT 0x0f
     EMIT 0xed //((0xc5 & 0x3f) << 3) | 0xc5
     EMIT 0x0d

      paddd t0t1, in7in6;  // [tmp0,tmp1] <= [in0,in1] + [in7,in6]

     movq [tmp7tmp6], t7t6;
//      PI2FD( t0t1, t0t1);    // convert t0t1(int,int) -> t0t1(float,float)
      EMIT 0x0f
      EMIT 0x0f
      EMIT 0xc9 //((0xc1 & 0x3f) << 3) | 0xc1
      EMIT 0x0d

     movq [tmp4tmp5], t4t5;
//      PI2FD( t3t2, t3t2);   // convert t3t2(int,int) -> t3t2(float,float)
     EMIT 0x0f
     EMIT 0x0f
     EMIT 0xc0 //((0xc0 & 0x3f) << 3) | 0xc0
     EMIT 0x0d


//    movq [tmp0tmp1], t0t1;


//    movq [tmp3tmp2], t3t2;



    /* Even part */

//    *tmp10 = (*tmp0) + (*tmp3);    /* phase 2 */
//    *tmp11 = (*tmp1) + (*tmp2);
//    *tmp13 = (*tmp0) - (*tmp3);
//    *tmp12 = (*tmp1) - (*tmp2);

// 2a) [tmp10,tmp11] <= [tmp0,tmp1] + [tmp3,tmp2]
// 2b) [tmp13,tmp12] <= [tmp0,tmp1] - [tmp3,tmp2]

#define t10t11 mm6
#define t13t12 t0t1

    movq t10t11, t0t1;       // copy t0t1, prepare t10t11 calculation

//    PFADD( t10t11, t3t2 );       //  [tmp10,tmp11] <= [tmp0,tmp1] + [tmp3,tmp2]
    EMIT 0x0f
    EMIT 0x0f
    EMIT 0xf0 // ((0xc6 & 0x3f) << 3) | 0xc0
    EMIT 0x9e

//    PFSUB( t13t12, t3t2 );      //  [tmp13,tmp12] <= [tmp0,tmp1] - [tmp3,tmp2]
    EMIT 0x0f
    EMIT 0x0f
    EMIT 0xc8 // ((0xc1 & 0x3f) << 3) | 0xc0
    EMIT 0x9a

#define t10mt11 mm2 // mm0, mm4, mm5 not ok
#define t13t13  mm7 // mm1 not ok

//  st1_20 <= [tmp10,tmp11]
//  st1_21 <= [tmp10,-tmp11]   // negated tmp11
//  st1_22 <= [tmp13,tmp12] 
//  st1_23 <= [tmp13,tmp13] // tmp13 duplicated

//    movq [tmp10tmp11], t10t11;
     movq t10mt11, t10t11;     

//    movq [tmp13tmp12], t13t12;
    pxor t10mt11, [mmMask00001000]; // t10mt11 = [tmp10, -tmp11]
     movq t13t13, t13t12;

// 3a)  dataptr[4] = (*tmp10) - (*tmp11);
// 3a)  dataptr[0] = (*tmp10) + (*tmp11); /* phase 3 */

    punpckhdq t13t13, t13t13;      // t13t13 = [tmp13, tmp13]
     EMIT 0x0f
     EMIT 0x0f
     EMIT 0xf2 //((0xc6 & 0x3f) << 3) | 0xc2
     EMIT 0xae

//     PFACC( t10t11, t10mt11 ); // produce t10t11 = [dataptr[4], dataptr[0] ]

    EMIT 0x0f
    EMIT 0x0f
    EMIT 0xc9 //((0xc1 & 0x3f) << 3) | 0xc1
    EMIT 0xae
     
//    PFACC( t13t12, t13t12); // t13t12 = [z1a,z1a] <= [ tmp12+tmp13,tmp12+tmp13 ] 


#define z1mz1   t13t12      // [z1, -z1]

#define out4out0 t10t11
#define Z1B_CONST _ebx + 0 // [0.07071, -0.7071]

//    movd dword ptr [ edx + 4*0 ], out4out0; // dataptr[0] <= final result
      movd dword ptr [ edx + 32*0 ], out4out0; // dataptr[0] <= final result
     psrlq out4out0, 32;    // [ __, out4 ]

//    z1 = ((*tmp12) + (*tmp13)) * ((float ) NC_R_SQRT2); /* c4 */
// 4b)  [z1,-z1] <=  [z1a,z1a] * [ 0.7071,-0.7071 ];
    EMIT 0x0f
    EMIT 0x0f
    EMIT 0x4b //(((0xc1 & 0x3f) << 3) | 0x03 + 0 | 0x40)
    EMIT 0
    EMIT 0xb4
     
//    PFMULM( z1mz1, Z1B_CONST, 0 ); // z1mz1<= [z1, -z1]

//    movd dword ptr [ edx + 4*4 ], out4out0; // dataptr[4] <= final result
    movd dword ptr [ edx + 32*4 ], out4out0; // dataptr[4] <= final result

//  [dataptr2,dataptr6] <=   [tmp13,tmp13] + [z1,-z1]; // pfadd
    EMIT 0x0f
    EMIT 0x0f
    EMIT 0xf9 //((0xc7 & 0x3f) << 3) | 0xc1
    EMIT 0x9e
      
//    PFADD(t13t13, z1mz1 );  // produce t13t13 = [dataptr2,dataptr6]

// 5a)  dataptr[2] = (*tmp13) + z1;    /* phase 5 */
// 5a)  dataptr[6] = (*tmp13) - z1;

#define out2out6 t13t13
//    movd dword ptr [edx + 4*6], out2out6; // dataptr[6] <= final result
    movd dword ptr [edx + 32*6], out2out6; // dataptr[6] <= final result
     psrlq out2out6, 32;    // [ __, out2 ]

//    movd dword ptr [edx + 4*2], out2out6; // dataptr[2] <= final result
    movd dword ptr [edx + 32*2], out2out6; // dataptr[2] <= final result

#define t14t16   mm0
#define t7t6_2 mm2  // temp copy of t7t6
#define t15t15   mm1

//    *tmp14 = (*tmp4) + (*tmp5);    /* phase 2 */
//    *tmp15 = (*tmp5) + (*tmp6);
//    *tmp16 = (*tmp6) + (*tmp7);

// 6a) [tmp14,tmp16] <= [ (tmp4+tmp5), (tmp7+tmp6) ]; //pfacc
// 6b) [      tmp15] <= [tmp7,tmp6] + [tmp4,tmp5]; 
    movq t14t16, [tmp7tmp6];  // prepare [tmp14,tmp16] generation

    movq t15t15, [tmp4tmp5];
     movq t7t6_2, t14t16;     // make copy of [tmp7,tmp6]

//    PFACC(t14t16, t15t15); // t14t16 <= [ (tmp4+tmp5), (tmp7+tmp6) ]
     EMIT 0x0f
     EMIT 0x0f
     EMIT 0xc1 //((0xc0 & 0x3f) << 3) | 0xc1
     EMIT 0xae 
   
//    PFADD(t15t15, t7t6_2); // t15t15 <= [(x),tmp5+tmp6]
    EMIT 0x0f
    EMIT 0x0f
    EMIT 0xca //((0xc1 & 0x3f) << 3) | 0xc2
    EMIT 0x9e 

#define t14mt16 mm2
#define t15t14 mm3
    movq t15t14, t14t16;   // prepare to generate [tmp15,tmp14]
//     movd [tmp17tmp16], t14t16; // produce [tmp17,tmp16] <= [...,t16]

    punpckldq t15t15,t15t15; // t15t15 <= [tmp5+tmp6,tmp5+tmp6]
     movq t14mt16, t14t16;   // prepare to generate [tmp14, -tmp16]

    punpckhdq t15t14, t15t15; // mm3 <= [t15,t14]

//    movq [tmp15tmp14], t15t14; 

    /* The rotator is modified from fig 4-8 to avoid extra negations. */
//    (*z5) = ((*tmp14) - (*tmp16)) * ((float ) 0.382683433); /* c6 */
//    (*z2) = ((float ) 0.541196100) * (*tmp14) + (*z5); /* c2-c6 */
//    (*z4) = ((float ) 1.306562965) * (*tmp16) + (*z5); /* c2+c6 */
//    (*z3) = (*tmp15) * ((float ) NC_R_SQRT2); /* c4 */


//     form [z5a,z3a] <= [tmp14-tmp16, tmp15]
// 7a) [z2a,z4a] <= [tmp14,tmp16] * [0.5411,1.3066]
// 7b) [z5 ,z3 ] <= [tmp14-tmp16,tmp15] * [0.3827,0.7071]

#define ___t15  t15t15
    pxor t14mt16, [mmMask00001000]; // t14mt16 = [tmp14,-tmp16]
     psllq ___t15, 32;             // create ___t15 = [ t15, 000]

//#define Z2AZ4A_CONST _ebx + 8 // [0.5411, 1.3066]
#define Z2AZ4A_CONST _ebx // [0.5411, 1.3066]
#define z5az3a ___t15      // [z5a,z3a] <= [t14-t16,t15] <= t14mt16
#define z2az4a t14t16   // [z2a,z4a] = [t14t16]*[0.5411,1.30666]

//    PFMULM( z2az4a, Z2AZ4A_CONST, 8 ); // form [z2a,z4a]
//     PFACC( z5az3a, t14mt16 );  // [z5a,z3a] = [tmp14-tmp16, tmp15]
    EMIT 0x0f
    EMIT 0x0f
    EMIT 0x43 //(((0xc0 & 0x3f) << 3) | 0x03 | 0x40)
    EMIT 8
    EMIT 0xb4

    EMIT 0x0f
    EMIT 0x0f
    EMIT 0xca //((0xc1 & 0x3f) << 3) | 0xc2
    EMIT 0xae  

#define Z5AZ3A_CONST _ebx  // [0.3827, 0.7071]
//#define Z5AZ3A_CONST _ebx + 16 // [0.3827, 0.7071]

//    PFMULM( z5az3a, Z5AZ3A_CONST, 16 ); // z5az3a = [z5,z3]
    EMIT 0x0f
    EMIT 0x0f
    EMIT 0x4b //(((0xc1 & 0x3f) << 3) | 0x03 | 0x40)
    EMIT 16
    EMIT 0xb4

#define z5z3 z5az3a
#define z5z5 mm7
#define z3z3 mm6
    movq z5z5, z5z3;        // start to form z5z5
     movq z3z3, z5z3;        // start to form z3z3

    punpckhdq z5z5,z5z5;    // z5z5 = [z5,z5]
     punpckldq z3z3,z3z3;    // z3z3 = [z3,z3]

//    movq [z5z4], z5z5;      // produce z5

#define z3mz3 z3z3
#define z2z4 z2az4a
    pxor z3mz3, [mmMask00001000];  // z3mz3 = [z3,-z3]
// 7c) [z2,z4]  <= [z2a,z4a] + [z5,z5];
//    PFADD( z2z4, z5z5 );        // z2az4a <= [z2,z4]
    EMIT 0x0f
    EMIT 0x0f
    EMIT 0xc7 //((0xc0 & 0x3f) << 3) | 0xc7
    EMIT 0x9e

#define t7t7 mm5
    movq t7t7, [tmp7tmp6];    // begin forming [tmp7,tmp7]

    punpckhdq t7t7,t7t7;    // t7t7 = [tmp7,tmp7]

//    z11 = (*tmp7) + (*z3);        /* phase 5 */
//    z13 = (*tmp7) - (*z3);

#define z13z11 t7t7
#define out5out1 mm4
#define out3out7 z13z11
// 7d) [z13,z11] <= [tmp7,tmp7] - [z3,-z3]
//    PFSUB( z13z11, z3mz3);   // z13z11 = [z13,z11]
    EMIT 0x0f
    EMIT 0x0f
    EMIT 0xee //((0xc5 & 0x3f) << 3) | 0xc6
    EMIT 0x9a
       
     movq out5out1, z2z4;    // begin forming out5out1

//    dataptr[5] = z13 + (*z2);  /* phase 6 */
//    dataptr[3] = z13 - (*z2);
//    dataptr[1] = z11 + (*z4);
//   dataptr[7] = z11 - (*z4);

// 8a) [dataptr5,dataptr1] <= [z13,z11] + [z2,z4]
// 8b) [dataptr3,dataptr7] <= [z13,z11] - [z2,z4]

//    PFADD( out5out1, z13z11); // produce [out5,out1]
    EMIT 0x0f
    EMIT 0x0f
    EMIT 0xe5 //((0xc4 & 0x3f) << 3) | 0xc5
    EMIT 0x9e

//    PFSUB( out3out7, z2z4 );  // produce [out3,out7]
    EMIT 0x0f
    EMIT 0x0f
    EMIT 0xe8 //((0xc5 & 0x3f) << 3) | 0xc0
    EMIT 0x9a  

//      movd [edx + 4*1], out5out1; // store out[1]
      movd [edx + 32*1], out5out1; // store out[1]
  
     psrlq out5out1, 32;      // out5out1 <= [ 000, out5 ]

//    movd [edx + 4*7], out3out7; // store out[7]
    movd [edx + 32*7], out3out7; // store out[7]
     psrlq out3out7, 32;      // out3out7 <= [ 000, out3 ]

//    movd [edx + 4*5], out5out1; // store out[5]
    movd [edx + 32*5], out5out1; // store out[5]
     sub edi, 0x01;          // i = i - 1

//    movd [edx + 4*3], out3out7; // store out[3]
    movd [edx + 32*3], out3out7; // store out[3]
//     add edx, 32;            // 32=4*8, outptr += 8 (floats)
     add edx, 4;            // 4=4*1, outptr += 1 (floats)

    cmp edi, 0x00;          // end for ( i=8; i >= 0; i=i-1)
     jg tdn_dct_row1;  // branch until (edi == 0)

//    dataptr += 8;       /* advance pointer to next row */
//    blkptr += 8;
//  } // end for (i=0; i<8; i++);

// ----------------------- end of dct_row processing

// undefine the aliases used during dct_row processing, just in case
// we want to change the register allocation for the dct_col processing.

#undef in7in6
#undef in4in5
#undef in0in1
#undef in3in2
#undef in0in1_2
#undef in3in2_2
#undef t0t1
#undef t3t2
#undef t4t5
#undef t7t6
#undef t10t11
#undef t13t12
#undef t10mt11
#undef t13t13
#undef z1mz1
#undef out4out0
#undef Z1B_CONST
#undef out2out6
#undef t14t16
#undef t7t6_2
#undef t15t15
#undef t14mt16
#undef t15t14
#undef ___t15
#undef Z2AZ4A_CONST
#undef z5az3a
#undef z2az4a
#undef Z5AZ3A_CONST
#undef z5z3
#undef z5z5
#undef z3z3
#undef z3mz3
#undef z2z4
#undef t7t7
#undef z13z11
#undef out5out1
#undef out3out7

  /*************************************************************
   *
   * Pass 2: DCT process columns
   *
   *************************************************************/

     // The 2nd-pass uses the same base-code as the 1st-pass, the two
     // code loops differ only in the handling of input/output data.
     //
     // 1) Since the 1st-pass produces float-data, the 2nd-pass's
     //    input must accept floating point data.
     // 2) Since the 2nd-pass processes input row-by-row, and the source
     //    is already located in the temp array, the output is also
     //    stored row-by-row.  (We don't want to allocate an additional
     //    float[64] array.  This wastes cache RAM.)
     //    Therefore, the final post-processing (descaling) must
     //    additionally transpose the final output.

  //    s
  // 
//    for (i = 0; i < 8; i++)
//    {

//   3_2_1_0 -> x_3_2_1
     // mm0, mm1 = 3_2_1_0,  mm2, mm3 = x_3_2_1
     // mm4 = 7_6_5_4

     mov eax, dword ptr [dataptr]; 
      pxor mm7, mm7;              // mm7 <= 0x0000_0000_0000_0000

     mov edx, dword ptr [dataptr2];// edx <= &dataptr[64]
      mov edi, 0x08;               // edi = 'i' // for ( i = 8; i > 0; i=i+1 )

     lea ebx, dword ptr [CONSTANTS];// ebx <= &CONSTANTS[0]
     // tdn_dct_col1 computes the fDCT for 1 input-row.
     // tdn_dct_col1 stored the FDCT result back into the same row of the
     //    temp array.

tdn_dct_col1: // 3d_now_dct_col1 loop-point

// 1a) [in7,in6] = [blkptr7, blkptr6]; // dword, dword
// 1b) [in4,in5] = [blkptr4, blkptr5]; //dword, dword
// 1c) [in0,in1] = [blkptr0, blkptr1]; // dword,dword
// 1d) [in3,in2] = [blkptr3, blkptr2]; //dword, dword

#define in7in6 mm2
#define in4in5 mm3
#define in0in1 mm1
#define in3in2 mm0
#define in0in1_2 mm4
#define in3in2_2 mm5

     movd in0in1, dword ptr [eax+4*1];  // in0in1 <= [ 000, in1 ]

     punpckldq in0in1, qword ptr [eax+4*0];  // in0in1 <= [ in0, in1 ]

     movd in4in5, dword ptr [eax+4*5];  // in4in5 <= [ 000, in5 ]
      movq in0in1_2, in0in1;            // copy in0in1 to in0in1_2

     punpckldq in4in5, qword ptr [eax+4*4];  // in4in5 <= [ in4, in5 ]

     movq in3in2, qword ptr [eax+4*2];  // get [in3,in2]

     movq in7in6, qword ptr [eax+4*6];  // get [in7,in6]
      movq in3in2_2, in3in2;            // copy in3in2 to in3in2_2

/*
    *tmp0 = dataptr[0] + dataptr[7];
    *tmp7 = dataptr[0] - dataptr[7];
    *tmp1 = dataptr[1] + dataptr[6];
    *tmp6 = dataptr[1] - dataptr[6];
    *tmp2 = dataptr[2] + dataptr[5];
    *tmp5 = dataptr[2] - dataptr[5];
    *tmp3 = dataptr[3] + dataptr[4];
    *tmp4 = dataptr[3] - dataptr[4];
*/

// tmp0= inptr[0]                         +                         inptr[7];
// tmp1=         inptr[1]                 +                 inptr[6];

// tmp7= inptr[0]                         -                         inptr[7];
// tmp6=         inptr[1]                 -                 inptr[6];

// tmp3=                         inptr[3] + inptr[4];
// tmp2=                 inptr[2]         +         inptr[5];

// tmp4=                         inptr[3] - inptr[4];
// tmp5=                 inptr[2]         -         inptr[5];


#define t0t1 in0in1
#define t3t2 in3in2
#define t4t5 in3in2_2
#define t7t6 in0in1_2
//  st1_0 <= [tmp0,tmp1]  (float, float)
//  st1_1 <= [tmp7,tmp6]  (float, float)
//  st1_2 <= [tmp3,tmp2]  (float, float)
//  st1_3 <= [tmp4,tmp5]  (float, float)


     EMIT 0x0f
     EMIT 0x0f
     EMIT 0xe2 //((0xc4 & 0x3f) << 3) | 0xc2
     EMIT 0x9a

//     PFSUB(t7t6, in7in6); // [tmp7,tmp6] <= [in0,in1] - [in7,in6]
      add eax, 4*8;       // increment inptr+=8 (floats)

     EMIT 0x0f
     EMIT 0x0f
     EMIT 0xeb //((0xc5 & 0x3f) << 3) | 0xc3
     EMIT 0x9a

//     PFSUB(t4t5, in4in5);// [tmp4,tmp5] <= [in3,in2] - [in4,in5]

     EMIT 0x0f
     EMIT 0x0f
     EMIT 0xc3 //((0xc0 & 0x3f) << 3) | 0xc3
     EMIT 0x9e

//     PFADD(t3t2, in4in5);  // [tmp3,tmp2] <= [in3,in2] + [in4,in5]
     movq [tmp7tmp6], t7t6;

     EMIT 0x0f
     EMIT 0x0f
     EMIT 0xca //((0xc1 & 0x3f) << 3) | 0xc2
     EMIT 0x9e
     
//     PFADD(t0t1, in7in6); // [tmp0,tmp1] <= [in0,in1] + [in7,in6]
     movq [tmp4tmp5], t4t5;

//     movq [tmp3tmp2], t3t2;

//     movq [tmp0tmp1], t0t1;

    // Even part 

//    *tmp10 = (*tmp0) + (*tmp3);    // phase 2 
//    *tmp11 = (*tmp1) + (*tmp2);
//    *tmp13 = (*tmp0) - (*tmp3);
//    *tmp12 = (*tmp1) - (*tmp2);

// 2a) [tmp10,tmp11] <= [tmp0,tmp1] + [tmp3,tmp2]
// 2b) [tmp13,tmp12] <= [tmp0,tmp1] - [tmp3,tmp2]

#define t10t11 mm6
#define t13t12 t0t1

    movq t10t11, t0t1;       // copy t0t1, prepare t10t11 calculation

//    PFADD( t10t11, t3t2 );      //  [tmp10,tmp11] <= [tmp0,tmp1] + [tmp3,tmp2]
    EMIT 0x0f
    EMIT 0x0f
    EMIT 0xf0 //((0xc6 & 0x3f) << 3) | 0xc0
    EMIT 0x9e      

//    PFSUB( t13t12, t3t2 );      //  [tmp13,tmp12] <= [tmp0,tmp1] - [tmp3,tmp2]
    EMIT 0x0f
    EMIT 0x0f
    EMIT 0xc8 //((0xc1 & 0x3f) << 3) | 0xc0
    EMIT 0x9a      

#define t10mt11 mm2 // mm0, mm4, mm5 not ok
#define t13t13  mm7 // mm1 not ok

//  st1_20 <= [tmp10,tmp11]
//  st1_21 <= [tmp10,-tmp11]   // negated tmp11
//  st1_22 <= [tmp13,tmp12] 
//  st1_23 <= [tmp13,tmp13] // tmp13 duplicated

    movq [tmp10tmp11], t10t11;
     movq t10mt11, t10t11;     

    movq [tmp13tmp12], t13t12;
    pxor t10mt11, [mmMask00001000]; // t10mt11 = [tmp10, -tmp11]
     movq t13t13, t13t12;

// 3a)   dataptr2[4] = (*tmp10) - (*tmp11);
// 3a)   dataptr2[0] = (*tmp10) + (*tmp11); // phase 3

    punpckhdq t13t13, t13t13;      // t13t13 = [tmp13, tmp13]
     EMIT 0x0f
     EMIT 0x0f
     EMIT 0xf2 //((0xc6 & 0x3f) << 3) | 0xc2
     EMIT 0xae

//     PFACC( t10t11, t10mt11 ); // produce t10t11 = [dataptr[4], dataptr[0] ]

    EMIT 0x0f
    EMIT 0x0f
    EMIT 0xc9 //((0xc1 & 0x3f) << 3) | 0xc1
    EMIT 0xae

//    PFACC( t13t12, t13t12); // t13t12 = [z1a,z1a] <= [tmp12+tmp13,tmp12+tmp13]


#define z1mz1   t13t12      // [z1, -z1]

#define out4out0 t10t11
#define Z1B_CONST _ebx + 0 // [0.07071, -0.7071]

    movd dword ptr [ edx + 4*0 ], out4out0; // dataptr[0] <= final result
//      movd dword ptr [ edx + 32*0 ], out4out0; // dataptr[0] <= final result
     psrlq out4out0, 32;    // [ __, out4 ]

//    z1 = ((*tmp12) + (*tmp13)) * ((float ) NC_R_SQRT2); // c4 
// 4b)   [z1,-z1] <=  [z1a,z1a] * [ 0.7071,-0.7071 ];
//    PFMULM( z1mz1, Z1B_CONST, 0 ); // z1mz1<= [z1, -z1]
    EMIT 0x0f
    EMIT 0x0f
    EMIT 0x4b //(((0xc1 & 0x3f) << 3) | 0x03 + 0 | 0x40)
    EMIT 0
    EMIT 0xb4

    movd dword ptr [ edx + 4*4 ], out4out0; // dataptr[4] <= final result
//    movd dword ptr [ edx + 32*4 ], out4out0; // dataptr[4] <= final result

//  [dataptr2,dataptr6] <=   [tmp13,tmp13] + [z1,-z1]; // pfadd
//    PFADD(t13t13, z1mz1 );  // produce t13t13 = [dataptr2,dataptr6]
    EMIT 0x0f
    EMIT 0x0f
    EMIT 0xf9 //((0xc7 & 0x3f) << 3) | 0xc1
    EMIT 0x9e  

// 5a)  dataptr[2] = (*tmp13) + z1;    // phase 5 
// 5a)  dataptr[6] = (*tmp13) - z1;

#define out2out6 t13t13
    movd dword ptr [edx + 4*6], out2out6; // dataptr[6] <= final result
//    movd dword ptr [edx + 32*6], out2out6; // dataptr[6] <= final result
     psrlq out2out6, 32;    // [ __, out2 ]

    movd dword ptr [edx + 4*2], out2out6; // dataptr[2] <= final result
//    movd dword ptr [edx + 32*2], out2out6; // dataptr[2] <= final result

#define t14t16   mm0
#define t7t6_2 mm2  // temp copy of t7t6
#define t15t15   mm1

//    *tmp14 = (*tmp4) + (*tmp5);    // phase 2 
//    *tmp15 = (*tmp5) + (*tmp6);
//    *tmp16 = (*tmp6) + (*tmp7);

// 6a) [tmp14,tmp16] <= [ (tmp4+tmp5), (tmp7+tmp6) ]; //pfacc
// 6b) [      tmp15] <= [tmp7,tmp6] + [tmp4,tmp5]; 
    movq t14t16, [tmp7tmp6];  // prepare [tmp14,tmp16] generation

    movq t15t15, [tmp4tmp5];
     movq t7t6_2, t14t16;     // make copy of [tmp7,tmp6]

    EMIT 0x0f
    EMIT 0x0f
    EMIT 0xc1 //((0xc0 & 0x3f) << 3) | 0xc1
    EMIT 0xae

//    PFACC(t14t16, t15t15); // t14t16 <= [ (tmp4+tmp5), (tmp7+tmp6) ]

    EMIT 0x0f
    EMIT 0x0f
    EMIT 0xca //((0xc1 & 0x3f) << 3) | 0xc2
    EMIT 0x9e

//    PFADD(t15t15, t7t6_2); // t15t15 <= [(x),tmp5+tmp6]

#define t14mt16 mm2
#define t15t14 mm3
    movq t15t14, t14t16;   // prepare to generate [tmp15,tmp14]
//     movd [tmp17tmp16], t14t16; // produce [tmp17,tmp16] <= [...,t16]

    punpckldq t15t15,t15t15; // t15t15 <= [tmp5+tmp6,tmp5+tmp6]
     movq t14mt16, t14t16;   // prepare to generate [tmp14, -tmp16]

    punpckhdq t15t14, t15t15; // mm3 <= [t15,t14]

//    movq [tmp15tmp14], t15t14; 

    // The rotator is modified from fig 4-8 to avoid extra negations. 
//    (*z5) = ((*tmp14) - (*tmp16)) * ((float ) 0.382683433); // c6 
//    (*z2) = ((float ) 0.541196100) * (*tmp14) + (*z5); // c2-c6 
//    (*z4) = ((float ) 1.306562965) * (*tmp16) + (*z5); // c2+c6
//    (*z3) = (*tmp15) * ((float ) NC_R_SQRT2);          // c4


//     form [z5a,z3a] <= [tmp14-tmp16, tmp15]
// 7a) [z2a,z4a] <= [tmp14,tmp16] * [0.5411,1.3066]
// 7b) [z5 ,z3 ] <= [tmp14-tmp16,tmp15] * [0.3827,0.7071]

#define ___t15  t15t15
    pxor t14mt16, [mmMask00001000]; // t14mt16 = [tmp14,-tmp16]
     psllq ___t15, 32;             // create ___t15 = [ t15, 000]

//#define Z2AZ4A_CONST _ebx + 8 // [0.5411, 1.3066]
#define Z2AZ4A_CONST _ebx // [0.5411, 1.3066]
#define z5az3a ___t15      // [z5a,z3a] <= [t14-t16,t15] <= t14mt16
#define z2az4a t14t16   // [z2a,z4a] = [t14t16]*[0.5411,1.30666]

//    PFMULM( z2az4a, Z2AZ4A_CONST, 8 ); // form [z2a,z4a]
//     PFACC( z5az3a, t14mt16 );  // [z5a,z3a] = [tmp14-tmp16, tmp15]
    EMIT 0x0f
    EMIT 0x0f
    EMIT 0x43 //(((0xc0 & 0x3f) << 3) | 0x03 | 0x40)
    EMIT 8
    EMIT 0xb4

    EMIT 0x0f
    EMIT 0x0f
    EMIT 0xca //((0xc1 & 0x3f) << 3) | 0xc2
    EMIT 0xae  

#define Z5AZ3A_CONST _ebx  // [0.3827, 0.7071]
//#define Z5AZ3A_CONST _ebx + 16 // [0.3827, 0.7071]

//    PFMULM( z5az3a, Z5AZ3A_CONST, 16 ); // z5az3a = [z5,z3]
    EMIT 0x0f
    EMIT 0x0f
    EMIT 0x4b //(((0xc1 & 0x3f) << 3) | 0x03 | 0x40)
    EMIT 16
    EMIT 0xb4 

#define z5z3 z5az3a
#define z5z5 mm7
#define z3z3 mm6
    movq z5z5, z5z3;        // start to form z5z5
     movq z3z3, z5z3;        // start to form z3z3

    punpckhdq z5z5,z5z5;    // z5z5 = [z5,z5]
     punpckldq z3z3,z3z3;    // z3z3 = [z3,z3]

//    movq [z5z4], z5z5;      // produce z5

#define z3mz3 z3z3
#define z2z4 z2az4a
    pxor z3mz3, [mmMask00001000];  // z3mz3 = [z3,-z3]
// 7c) [z2,z4]  <= [z2a,z4a] + [z5,z5];
//    PFADD( z2z4, z5z5 );        // z2az4a <= [z2,z4]
    EMIT 0x0f
    EMIT 0x0f
    EMIT 0xc7 //((0xc0 & 0x3f) << 3) | 0xc7
    EMIT 0x9e        

#define t7t7 mm5
    movq t7t7, [tmp7tmp6];    // begin forming [tmp7,tmp7]

    punpckhdq t7t7,t7t7;    // t7t7 = [tmp7,tmp7]

//    z11 = (*tmp7) + (*z3);        // phase 5
//    z13 = (*tmp7) - (*z3);

#define z13z11 t7t7
#define out5out1 mm4
#define out3out7 z13z11
// 7d) [z13,z11] <= [tmp7,tmp7] - [z3,-z3]
//    PFSUB( z13z11, z3mz3);   // z13z11 = [z13,z11]
    EMIT 0x0f
    EMIT 0x0f
    EMIT 0xee //((0xc5 & 0x3f) << 3) | 0xc6
    EMIT 0x9a
       
     movq out5out1, z2z4;    // begin forming out5out1

//    dataptr[5] = z13 + (*z2);  // phase 6 
//    dataptr[3] = z13 - (*z2);
//    dataptr[1] = z11 + (*z4);
//   dataptr[7] = z11 - (*z4);

// 8a) [dataptr5,dataptr1] <= [z13,z11] + [z2,z4]
// 8b) [dataptr3,dataptr7] <= [z13,z11] - [z2,z4]

//    PFADD( out5out1, z13z11); // produce [out5,out1]
    EMIT 0x0f
    EMIT 0x0f
    EMIT 0xe5 //((0xc4 & 0x3f) << 3) | 0xc5
    EMIT 0x9e 

//    PFSUB( out3out7, z2z4 );  // produce [out3,out7]
    EMIT 0x0f
    EMIT 0x0f
    EMIT 0xe8 //((0xc5 & 0x3f) << 3) | 0xc0
    EMIT 0x9a  

      movd [edx + 4*1], out5out1; // store out[1]
//      movd [edx + 32*1], out5out1; // store out[1]
  
     psrlq out5out1, 32;      // out5out1 <= [ 000, out5 ]

    movd [edx + 4*7], out3out7; // store out[7]
//    movd [edx + 32*7], out3out7; // store out[7]
     psrlq out3out7, 32;      // out3out7 <= [ 000, out3 ]

    movd [edx + 4*5], out5out1; // store out[5]
//    movd [edx + 32*5], out5out1; // store out[5]
     sub edi, 0x01;          // i = i - 1

    movd [edx + 4*3], out3out7; // store out[3]
//    movd [edx + 32*3], out3out7; // store out[3]
     add edx, 32;            // 32=4*8, outptr += 8 (floats)
//     add edx, 4;            // 4=4*1, outptr += 1 (floats)

    cmp edi, 0x00;          // end for ( i=8; i >= 0; i=i-1)
     jg tdn_dct_col1;  // branch until (edi == 0)

//    FEMMS;
//    };
//    dataptr2 += 8;       // advance pointer to next row 
//    blkptr += 8;
//  } // end for (i=0; i<8; i++); 

// --------------------------- end of dct_column processing
// output matrix is transposed with respect to input.  Before this result
// can be used, the matrix must be transposed.


  // descale and transpose the output 
//  dataptr2 = &data[0];
  
/*
  // To enable precise rounding uncomment this C-code, and the #defines
  // near the beginning of this file (important!)  
  // of course, this code will run much slower!
  for (i = 0; i < 8; i++)
    for ( j = 0; j < 8; j++)
      // scale, round, and transpose output matrix
      block[i+(j<<3)] = (short int) floor(dataptr2[j+(i<<3)] * aanscales[i+(j<<3)] + 0.5);
*/

    // The following loop transposes and descales the 3dn_dct result.
    // There is a slight systematic error in the descaling algorithm.
    // The x86/x87 float->int convert uses a truncation policy :
    // The floating-point value's least significant digit (LSD) is 
    // rounded to integer of lesser MAGNITUDE.  For positive numbers, this
    // policy is the same as standard-truncation (+5.x -> +5.0)
    // But problems arise with negative numbers : -5.x(float) -> -5(int), 
    // For negative numbers, the policy is *not* truncation, but rather
    // unconditional-roundup.  
    //
    // Since negative#'s and postive#'s are rounded differently, the naive 
    // "add +0.5" compensation method fails.  This rounding issue ruins an 
    // otherwise "correct" f_DCT algorithm.
    //
    // To correct the rounding problem, negative numbers are 'precompensated'
    // with '-0.5'.  Postive numbers continue to 'precompensated' with '+0.5.'
    // This policy produces accurate rounding, but is data-dependent : 
    // The CPU must examine each input float-value in order to select the
    // proper rounding compensation-value (-0.5 or +0.5.)
    //
    // In practice, data-decision policies are to be avoided. 
    //
    // An alternative "fast-round"-algorithm offers faster performance at the
    // expense of accuracy. The float-value is first multiplied by a 
    // factor:"2^N", where N is an integer.  Then, the scaled-float is 
    // converted to integer using the standard CPU instruction (yes, the one
    // with the troublesome truncation-policy.)  Now, the scaled-integer is
    // summed with a scaled-precompensation value ("+2^(N-1)".)  During the
    // last step, the compenasted-integer is descaled by right shifting 
    // N-bits, yielding the final result.
    //
    // The forward(float)-scaling and back(int)-decsaling effectively
    // "pushes" the rounding-inaccuracy far to the right of the decimal-point,
    // where its impact is less.  Using this method, negative#s still exhibit
    // a slight-shift (the shift is exactly equal to 2^(1-N)), but with a
    // suitably large N, the shift is negligble.

    // For N=15, the shift is 1 part per 32768, or 1/32768 (~0.0000305)

    // As a final note, the -0.5/+0.5 (data-dependent) rounding policy can
    // be implemented using the partition-compare MMX instructions pcmpXXX.
    // Statistical tests for both precise and imprecise rounding policies 
    // were compared.  For N=15, the tests revealed no loss in precision
    // for the "fast-round" approach.


// row  col->  (source)        col->   (destination)
//  #    0 1 2 3 4 5 6 7            0 1 2 3   4 5 6 7
//  -    ---------------            -----------------
//  0    A I ...                    A B C D   E F G H 
//  1    B J ...                    I J K L   M N O P
//  2    C K ...(source)    ------>   (destination)
//  3    D L ...
//
//  4    E M ...
//  5    F N ...
//  6    G O ...
//  7    H P ...

#define mmRoundup mm7

     mov eax, dword ptr [dataptr2];// eax = upper half of source matrix
      mov edi, 0x04;               // edi = 'i' // for ( i = 4; i > 0; i=i+1 )

     lea ebx, dword ptr [aanscales];//ebx <= aanscales[] (table of constants)
      mov ecx, eax;                 // copy eax -> ecx

     movq mmRoundup, [mmMaskRnd]; // "round up" (+0.5,+0.5) mask
      add ecx, 128;                // ecx = lower half of source matrix

     mov edx, dword ptr [block];   // edx <= block[] (output)
      sub ebx, 64;

tdn_dct_postproc: // 3d_now_dct post-processing jump-point
     // Each loop iteration converts a 2x8 (colxrow) segment.  The loop
     // executes 4 times to produce the final 8x8 output matrix.

     movq mm0, qword ptr [eax+32*0];  // mm0 <= (1,0 : 0) [A,I]
      add ebx, 64;

     movq mm2, qword ptr [eax+32*1];  // mm2 <= (1,0 : 1) [B,J]
      movq mm1, mm0; // mm1 = copy of (1,0 : 0) [A,I]
  
     movq mm3, qword ptr [eax+32*2]; // mm3 <= (1,0 : 2) [C,K]
      punpckldq mm0,mm2; // mm0 = ( 0 : 1,0 ) [B,A]

     movq mm4, qword ptr [eax+32*3]; // mm4 <= (1,0 : 3) [D,L]
      punpckhdq mm1, mm2; // mm1 = ( 1 : 1,0 ) [J,I]

     movq mm2, mm3;  // mm2 = copy of (1,0 : 2) [C,K]
//      PFMULM( mm0, _ebx, 0 );  // mm0 <= scale ( 0 : 1,0 ) [B,A]
      EMIT 0x0f
      EMIT 0x0f
      EMIT 0x43 //(((0xc0 & 0x3f) << 3) | 0x03 | 0x40)
      EMIT 0
      EMIT 0xb4

     punpckldq mm2, mm4; // mm2 = ( 0 : 3,2 ) [D,C]
//      PFMULM( mm1, _ebx, 0+32 ); // mm1 <= scale ( 1 : 1,0 ) [J,K]
      EMIT 0x0f
      EMIT 0x0f
      EMIT 0x4b //(((0xc1 & 0x3f) << 3) | 0x03 | 0x40)
      EMIT 0+32
      EMIT 0xb4

     punpckhdq mm3, mm4; // mm3 = ( 1 : 3,2 ) [L,K]
//      PFMULM( mm2, _ebx, 8 ); // mm2 <= scale ( 0 : 3,2 ) [D,C]
      EMIT 0x0f
      EMIT 0x0f
      EMIT 0x53 //(((0xc2 & 0x3f) << 3) | 0x03 | 0x40)
      EMIT 8
      EMIT 0xb4 

//     PFMULM( mm3, _ebx, 8+32 ); // mm1 <= scale ( 1: 3,2 ) [L,K]
//      PF2ID( mm0, mm0 ); // mm0 <= float -> integer [B,A]
     EMIT 0x0f
     EMIT 0x0f
     EMIT 0x5b //(((0xc3 & 0x3f) << 3) | 0x03 | 0x40)
     EMIT 8+32
     EMIT 0xb4

     EMIT 0x0f
     EMIT 0x0f
     EMIT 0xc0 //((0xc0 & 0x3f) << 3) | 0xc0
     EMIT 0x1d 

//     PF2ID( mm1, mm1 ); // mm1 <= float -> integer [J,I]
     EMIT 0x0f
     EMIT 0x0f
     EMIT 0xc9 //((0xc1 & 0x3f) << 3) | 0xc1
     EMIT 0x1d

      add eax, 8;

//     PF2ID( mm2, mm2 ); // mm2 <= float -> integer [D,C]
     EMIT 0x0f
     EMIT 0x0f
     EMIT 0xd2 //((0xc2 & 0x3f) << 3) | 0xc2
     EMIT 0x1d

      paddd  mm0, mmRoundup; // roundup compensation [B,A]

     paddd  mm1, mmRoundup; // roundup compensation [J,I]
//      PF2ID( mm3, mm3 ); // mm3 <= float -> integer [L,K]
     EMIT 0x0f
     EMIT 0x0f
     EMIT 0xdb //((0xc3 & 0x3f) << 3) | 0xc3
     EMIT 0x1d

     paddd  mm2, mmRoundup; // roundup compensation [D,C]
      psrad mm0, PSCF_SHIFT; // DESCALE [B,A]

     paddd  mm3, mmRoundup; // roundup compensation [L,K]
      psrad mm2, PSCF_SHIFT; // DESCALE [D,C]

     packssdw mm0, mm2;     // form [D,C,B,A]
      psrad mm1, PSCF_SHIFT; // DESCALE [J,I]

     movq qword ptr [edx], mm0; // store [D,C,B,A]
      psrad mm3, PSCF_SHIFT; // DESCALE [L,K]

     packssdw mm1, mm3;    // form [L,K,J,I]
//      sub edi, 1;      // i = i - 1
//  row->0 1
//  col  ---
//  4    E M ...
//  5    F N ...
//  6    G O ...
//  7    H P ...

      movq mm0, qword ptr [ecx+32*0];  // mm0 <= (1,0 : 0) [E,M]

     movq qword ptr [edx+16], mm1; // store [L,K,J,I]

     movq mm2, qword ptr [ecx+32*1];  // mm2 <= (1,0 : 1) [F,N]
      movq mm1, mm0; // mm1 = copy of (1,0 : 0) [E,M]
  
     movq mm3, qword ptr [ecx+32*2]; // mm3 <= (1,0 : 2) [G,O]
      punpckldq mm0,mm2; // mm0 = ( 0 : 1,0 ) [F,E]

     movq mm4, qword ptr [ecx+32*3]; // mm4 <= (1,0 : 3) [H,P]
      punpckhdq mm1, mm2; // mm1 = ( 1 : 1,0 ) [N,M]

     movq mm2, mm3;  // mm2 = copy of (1,0 : 2) [G,O]
//      PFMULM( mm0, _ebx, 16);  // mm0 <= scale ( 0 : 1,0 ) [F,E]
      EMIT 0x0f
      EMIT 0x0f
      EMIT 0x43 //(((0xc0 & 0x3f) << 3) | 0x03 | 0x40)
      EMIT 16
      EMIT 0xb4  

     punpckldq mm2, mm4; // mm2 = ( 0 : 3,2 ) [H,G]
//      PFMULM( mm1, _ebx, 16+32 ); // mm1 <= scale ( 1 : 1,0 ) [J,K]
      EMIT 0x0f
      EMIT 0x0f
      EMIT 0x4b //(((0xc1 & 0x3f) << 3) | 0x03 | 0x40)
      EMIT 16+32
      EMIT 0xb4 

     punpckhdq mm3, mm4; // mm3 = ( 1 : 3,2 ) [P,O]
//      PFMULM( mm2, _ebx, 16+8); // mm2 <= scale ( 0 : 3,2 ) [H,G]
      EMIT 0x0f
      EMIT 0x0f
      EMIT 0x53 //(((0xc2 & 0x3f) << 3) | 0x03 | 0x40)
      EMIT 16+8
      EMIT 0xb4 

//     PFMULM( mm3, _ebx, 16+8+32 ); // mm1 <= scale ( 1: 3,2 ) [L,K]
//      PF2ID( mm0, mm0 ); // mm0 <= float -> integer [F,E]
     EMIT 0x0f
     EMIT 0x0f
     EMIT 0x5b //(((0xc3 & 0x3f) << 3) | 0x03 | 0x40)
     EMIT 16+8+32
     EMIT 0xb4

     EMIT 0x0f
     EMIT 0x0f
     EMIT 0xc0 //((0xc0 & 0x3f) << 3) | 0xc0
     EMIT 0x1d

//     PF2ID( mm1, mm1 ); // mm1 <= float -> integer [N,M]
     EMIT 0x0f
     EMIT 0x0f
     EMIT 0xc9 //((0xc1 & 0x3f) << 3) | 0xc1
     EMIT 0x1d

      add ecx, 8;

//     PF2ID( mm2, mm2 ); // mm2 <= float -> integer [H,G]
     EMIT 0x0f
     EMIT 0x0f
     EMIT 0xd2 //((0xc2 & 0x3f) << 3) | 0xc2
     EMIT 0x1d

      paddd  mm0, mmRoundup; // roundup compensation [F,E]

     paddd  mm1, mmRoundup; // roundup compensation [N,M]
//      PF2ID( mm3, mm3 ); // mm3 <= float -> integer [P,O]
      EMIT 0x0f
      EMIT 0x0f
      EMIT 0xdb //((0xc3 & 0x3f) << 3) | 0xc3
      EMIT 0x1d 

     paddd  mm2, mmRoundup; // roundup compensation [H,G]
      psrad mm0, PSCF_SHIFT; // DESCALE [F,E]

     paddd  mm3, mmRoundup; // roundup compensation [P,O]
      psrad mm2, PSCF_SHIFT; // DESCALE [H,G]

     packssdw mm0, mm2;     // form [H,G,F,E]
      psrad mm1, PSCF_SHIFT; // DESCALE [N,M]

     movq qword ptr [edx+8], mm0; // store [H,G,F,E]
      psrad mm3, PSCF_SHIFT; // DESCALE [P,O]

     packssdw mm1, mm3;    // form [P,O,N,M]
      sub edi, 1;      // i = i - 1

     movq qword ptr [edx+8+16], mm1; // store [P,O,N,M]
      add edx, 32;

     cmp edi, 0x00;   // (i==0) ?

     jg tdn_dct_postproc;  // end for ( i =8; i > 0; i = i - 1)
//     FEMMS;
     EMIT 0x0f
     EMIT 0x0e  // this is the EMMS instruction the compiler complains about
   };

}
