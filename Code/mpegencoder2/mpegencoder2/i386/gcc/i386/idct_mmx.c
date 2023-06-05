/*
 * Note: For libavcodec, this code can also be used under the LGPL license
 */
/*
 * idct_mmx.c
 * Copyright (C) 1999-2001 Aaron Holtzman <aholtzma@ess.engr.uvic.ca>
 *
 * This file is part of mpeg2dec, a free MPEG-2 video stream decoder.
 *
 * mpeg2dec is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * mpeg2dec is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

#include "../common.h"
#include "../dsputil.h"

#include "mmx.h"

#define ATTR_ALIGN(align) __attribute__ ((__aligned__ (align)))

#define ROW_SHIFT 11
#define COL_SHIFT 6

#define round(bias) ((int)(((bias)+0.5) * (1<<ROW_SHIFT)))
#define rounder(bias) {round (bias), round (bias)}

#if 0
/* C row IDCT - its just here to document the MMXEXT and MMX versions */
static inline void idct_row (int16_t * row, int offset,
			     int16_t * table, int32_t * rounder)
{
    int C1, C2, C3, C4, C5, C6, C7;
    int a0, a1, a2, a3, b0, b1, b2, b3;

    row += offset;

    C1 = table[1];
    C2 = table[2];
    C3 = table[3];
    C4 = table[4];
    C5 = table[5];
    C6 = table[6];
    C7 = table[7];

    a0 = C4*row[0] + C2*row[2] + C4*row[4] + C6*row[6] + *rounder;
    a1 = C4*row[0] + C6*row[2] - C4*row[4] - C2*row[6] + *rounder;
    a2 = C4*row[0] - C6*row[2] - C4*row[4] + C2*row[6] + *rounder;
    a3 = C4*row[0] - C2*row[2] + C4*row[4] - C6*row[6] + *rounder;

    b0 = C1*row[1] + C3*row[3] + C5*row[5] + C7*row[7];
    b1 = C3*row[1] - C7*row[3] - C1*row[5] - C5*row[7];
    b2 = C5*row[1] - C1*row[3] + C7*row[5] + C3*row[7];
    b3 = C7*row[1] - C5*row[3] + C3*row[5] - C1*row[7];

    row[0] = (a0 + b0) >> ROW_SHIFT;
    row[1] = (a1 + b1) >> ROW_SHIFT;
    row[2] = (a2 + b2) >> ROW_SHIFT;
    row[3] = (a3 + b3) >> ROW_SHIFT;
    row[4] = (a3 - b3) >> ROW_SHIFT;
    row[5] = (a2 - b2) >> ROW_SHIFT;
    row[6] = (a1 - b1) >> ROW_SHIFT;
    row[7] = (a0 - b0) >> ROW_SHIFT;
}
#endif


/* MMXEXT row IDCT */

#define mmxext_table(c1,c2,c3,c4,c5,c6,c7)	{  c4,  c2, -c4, -c2,	\
						   c4,  c6,  c4,  c6,	\
						   c1,  c3, -c1, -c5,	\
						   c5,  c7,  c3, -c7,	\
						   c4, -c6,  c4, -c6,	\
						  -c4,  c2,  c4, -c2,	\
						   c5, -c1,  c3, -c1,	\
						   c7,  c3,  c7, -c5 }

static inline void mmxext_row_head (int16_t * row, int offset, int16_t * table)
{
    movq_m2r (*(row+offset), mm2);	// mm2 = x6 x4 x2 x0

    movq_m2r (*(row+offset+4), mm5);	// mm5 = x7 x5 x3 x1
    movq_r2r (mm2, mm0);		// mm0 = x6 x4 x2 x0

    movq_m2r (*table, mm3);		// mm3 = -C2 -C4 C2 C4
    movq_r2r (mm5, mm6);		// mm6 = x7 x5 x3 x1

    movq_m2r (*(table+4), mm4);		// mm4 = C6 C4 C6 C4
    pmaddwd_r2r (mm0, mm3);		// mm3 = -C4*x4-C2*x6 C4*x0+C2*x2

    pshufw_r2r (mm2, mm2, 0x4e);	// mm2 = x2 x0 x6 x4
}

static inline void mmxext_row (int16_t * table, int32_t * rounder)
{
    movq_m2r (*(table+8), mm1);		// mm1 = -C5 -C1 C3 C1
    pmaddwd_r2r (mm2, mm4);		// mm4 = C4*x0+C6*x2 C4*x4+C6*x6

    pmaddwd_m2r (*(table+16), mm0);	// mm0 = C4*x4-C6*x6 C4*x0-C6*x2
    pshufw_r2r (mm6, mm6, 0x4e);	// mm6 = x3 x1 x7 x5

    movq_m2r (*(table+12), mm7);	// mm7 = -C7 C3 C7 C5
    pmaddwd_r2r (mm5, mm1);		// mm1 = -C1*x5-C5*x7 C1*x1+C3*x3

    paddd_m2r (*rounder, mm3);		// mm3 += rounder
    pmaddwd_r2r (mm6, mm7);		// mm7 = C3*x1-C7*x3 C5*x5+C7*x7

    pmaddwd_m2r (*(table+20), mm2);	// mm2 = C4*x0-C2*x2 -C4*x4+C2*x6
    paddd_r2r (mm4, mm3);		// mm3 = a1 a0 + rounder

    pmaddwd_m2r (*(table+24), mm5);	// mm5 = C3*x5-C1*x7 C5*x1-C1*x3
    movq_r2r (mm3, mm4);		// mm4 = a1 a0 + rounder

    pmaddwd_m2r (*(table+28), mm6);	// mm6 = C7*x1-C5*x3 C7*x5+C3*x7
    paddd_r2r (mm7, mm1);		// mm1 = b1 b0

    paddd_m2r (*rounder, mm0);		// mm0 += rounder
    psubd_r2r (mm1, mm3);		// mm3 = a1-b1 a0-b0 + rounder

    psrad_i2r (ROW_SHIFT, mm3);		// mm3 = y6 y7
    paddd_r2r (mm4, mm1);		// mm1 = a1+b1 a0+b0 + rounder

    paddd_r2r (mm2, mm0);		// mm0 = a3 a2 + rounder
    psrad_i2r (ROW_SHIFT, mm1);		// mm1 = y1 y0

    paddd_r2r (mm6, mm5);		// mm5 = b3 b2
    movq_r2r (mm0, mm4);		// mm4 = a3 a2 + rounder

    paddd_r2r (mm5, mm0);		// mm0 = a3+b3 a2+b2 + rounder
    psubd_r2r (mm5, mm4);		// mm4 = a3-b3 a2-b2 + rounder
}

static inline void mmxext_row_tail (int16_t * row, int store)
{
    psrad_i2r (ROW_SHIFT, mm0);		// mm0 = y3 y2

    psrad_i2r (ROW_SHIFT, mm4);		// mm4 = y4 y5

    packssdw_r2r (mm0, mm1);		// mm1 = y3 y2 y1 y0

    packssdw_r2r (mm3, mm4);		// mm4 = y6 y7 y4 y5

    movq_r2m (mm1, *(row+store));	// save y3 y2 y1 y0
    pshufw_r2r (mm4, mm4, 0xb1);	// mm4 = y7 y6 y5 y4

    /* slot */

    movq_r2m (mm4, *(row+store+4));	// save y7 y6 y5 y4
}

static inline void mmxext_row_mid (int16_t * row, int store,
				   int offset, int16_t * table)
{
    movq_m2r (*(row+offset), mm2);	// mm2 = x6 x4 x2 x0
    psrad_i2r (ROW_SHIFT, mm0);		// mm0 = y3 y2

    movq_m2r (*(row+offset+4), mm5);	// mm5 = x7 x5 x3 x1
    psrad_i2r (ROW_SHIFT, mm4);		// mm4 = y4 y5

    packssdw_r2r (mm0, mm1);		// mm1 = y3 y2 y1 y0
    movq_r2r (mm5, mm6);		// mm6 = x7 x5 x3 x1

    packssdw_r2r (mm3, mm4);		// mm4 = y6 y7 y4 y5
    movq_r2r (mm2, mm0);		// mm0 = x6 x4 x2 x0

    movq_r2m (mm1, *(row+store));	// save y3 y2 y1 y0
    pshufw_r2r (mm4, mm4, 0xb1);	// mm4 = y7 y6 y5 y4

    movq_m2r (*table, mm3);		// mm3 = -C2 -C4 C2 C4
    movq_r2m (mm4, *(row+store+4));	// save y7 y6 y5 y4

    pmaddwd_r2r (mm0, mm3);		// mm3 = -C4*x4-C2*x6 C4*x0+C2*x2

    movq_m2r (*(table+4), mm4);		// mm4 = C6 C4 C6 C4
    pshufw_r2r (mm2, mm2, 0x4e);	// mm2 = x2 x0 x6 x4
}


/* MMX row IDCT */

#define mmx_table(c1,c2,c3,c4,c5,c6,c7)	{  c4,  c2,  c4,  c6,	\
					   c4,  c6, -c4, -c2,	\
					   c1,  c3,  c3, -c7,	\
					   c5,  c7, -c1, -c5,	\
					   c4, -c6,  c4, -c2,	\
					  -c4,  c2,  c4, -c6,	\
					   c5, -c1,  c7, -c5,	\
					   c7,  c3,  c3, -c1 }

static inline void mmx_row_head (int16_t * row, int offset, int16_t * table)
{
    movq_m2r (*(row+offset), mm2);	// mm2 = x6 x4 x2 x0

    movq_m2r (*(row+offset+4), mm5);	// mm5 = x7 x5 x3 x1
    movq_r2r (mm2, mm0);		// mm0 = x6 x4 x2 x0

    movq_m2r (*table, mm3);		// mm3 = C6 C4 C2 C4
    movq_r2r (mm5, mm6);		// mm6 = x7 x5 x3 x1

    punpckldq_r2r (mm0, mm0);		// mm0 = x2 x0 x2 x0

    movq_m2r (*(table+4), mm4);		// mm4 = -C2 -C4 C6 C4
    pmaddwd_r2r (mm0, mm3);		// mm3 = C4*x0+C6*x2 C4*x0+C2*x2

    movq_m2r (*(table+8), mm1);		// mm1 = -C7 C3 C3 C1
    punpckhdq_r2r (mm2, mm2);		// mm2 = x6 x4 x6 x4
}

static inline void mmx_row (int16_t * table, int32_t * rounder)
{
    pmaddwd_r2r (mm2, mm4);		// mm4 = -C4*x4-C2*x6 C4*x4+C6*x6
    punpckldq_r2r (mm5, mm5);		// mm5 = x3 x1 x3 x1

    pmaddwd_m2r (*(table+16), mm0);	// mm0 = C4*x0-C2*x2 C4*x0-C6*x2
    punpckhdq_r2r (mm6, mm6);		// mm6 = x7 x5 x7 x5

    movq_m2r (*(table+12), mm7);	// mm7 = -C5 -C1 C7 C5
    pmaddwd_r2r (mm5, mm1);		// mm1 = C3*x1-C7*x3 C1*x1+C3*x3

    paddd_m2r (*rounder, mm3);		// mm3 += rounder
    pmaddwd_r2r (mm6, mm7);		// mm7 = -C1*x5-C5*x7 C5*x5+C7*x7

    pmaddwd_m2r (*(table+20), mm2);	// mm2 = C4*x4-C6*x6 -C4*x4+C2*x6
    paddd_r2r (mm4, mm3);		// mm3 = a1 a0 + rounder

    pmaddwd_m2r (*(table+24), mm5);	// mm5 = C7*x1-C5*x3 C5*x1-C1*x3
    movq_r2r (mm3, mm4);		// mm4 = a1 a0 + rounder

    pmaddwd_m2r (*(table+28), mm6);	// mm6 = C3*x5-C1*x7 C7*x5+C3*x7
    paddd_r2r (mm7, mm1);		// mm1 = b1 b0

    paddd_m2r (*rounder, mm0);		// mm0 += rounder
    psubd_r2r (mm1, mm3);		// mm3 = a1-b1 a0-b0 + rounder

    psrad_i2r (ROW_SHIFT, mm3);		// mm3 = y6 y7
    paddd_r2r (mm4, mm1);		// mm1 = a1+b1 a0+b0 + rounder

    paddd_r2r (mm2, mm0);		// mm0 = a3 a2 + rounder
    psrad_i2r (ROW_SHIFT, mm1);		// mm1 = y1 y0

    paddd_r2r (mm6, mm5);		// mm5 = b3 b2
    movq_r2r (mm0, mm7);		// mm7 = a3 a2 + rounder

    paddd_r2r (mm5, mm0);		// mm0 = a3+b3 a2+b2 + rounder
    psubd_r2r (mm5, mm7);		// mm7 = a3-b3 a2-b2 + rounder
}

static inline void mmx_row_tail (int16_t * row, int store)
{
    psrad_i2r (ROW_SHIFT, mm0);		// mm0 = y3 y2

    psrad_i2r (ROW_SHIFT, mm7);		// mm7 = y4 y5

    packssdw_r2r (mm0, mm1);		// mm1 = y3 y2 y1 y0

    packssdw_r2r (mm3, mm7);		// mm7 = y6 y7 y4 y5

    movq_r2m (mm1, *(row+store));	// save y3 y2 y1 y0
    movq_r2r (mm7, mm4);		// mm4 = y6 y7 y4 y5

    pslld_i2r (16, mm7);		// mm7 = y7 0 y5 0

    psrld_i2r (16, mm4);		// mm4 = 0 y6 0 y4

    por_r2r (mm4, mm7);			// mm7 = y7 y6 y5 y4

    /* slot */

    movq_r2m (mm7, *(row+store+4));	// save y7 y6 y5 y4
}

static inline void mmx_row_mid (int16_t * row, int store,
				int offset, int16_t * table)
{
    movq_m2r (*(row+offset), mm2);	// mm2 = x6 x4 x2 x0
    psrad_i2r (ROW_SHIFT, mm0);		// mm0 = y3 y2

    movq_m2r (*(row+offset+4), mm5);	// mm5 = x7 x5 x3 x1
    psrad_i2r (ROW_SHIFT, mm7);		// mm7 = y4 y5

    packssdw_r2r (mm0, mm1);		// mm1 = y3 y2 y1 y0
    movq_r2r (mm5, mm6);		// mm6 = x7 x5 x3 x1

    packssdw_r2r (mm3, mm7);		// mm7 = y6 y7 y4 y5
    movq_r2r (mm2, mm0);		// mm0 = x6 x4 x2 x0

    movq_r2m (mm1, *(row+store));	// save y3 y2 y1 y0
    movq_r2r (mm7, mm1);		// mm1 = y6 y7 y4 y5

    punpckldq_r2r (mm0, mm0);		// mm0 = x2 x0 x2 x0
    psrld_i2r (16, mm7);		// mm7 = 0 y6 0 y4

    movq_m2r (*table, mm3);		// mm3 = C6 C4 C2 C4
    pslld_i2r (16, mm1);		// mm1 = y7 0 y5 0

    movq_m2r (*(table+4), mm4);		// mm4 = -C2 -C4 C6 C4
    por_r2r (mm1, mm7);			// mm7 = y7 y6 y5 y4

    movq_m2r (*(table+8), mm1);		// mm1 = -C7 C3 C3 C1
    punpckhdq_r2r (mm2, mm2);		// mm2 = x6 x4 x6 x4

    movq_r2m (mm7, *(row+store+4));	// save y7 y6 y5 y4
    pmaddwd_r2r (mm0, mm3);		// mm3 = C4*x0+C6*x2 C4*x0+C2*x2
}


#if 0
// C column IDCT - its just here to document the MMXEXT and MMX versions
static inline void idct_col (int16_t * col, int offset)
{
/* multiplication - as implemented on mmx */
#define F(c,x) (((c) * (x)) >> 16)

/* saturation - it helps us handle torture test cases */
#define S(x) (((x)>32767) ? 32767 : ((x)<-32768) ? -32768 : (x))

    int16_t x0, x1, x2, x3, x4, x5, x6, x7;
    int16_t y0, y1, y2, y3, y4, y5, y6, y7;
    int16_t a0, a1, a2, a3, b0, b1, b2, b3;
    int16_t u04, v04, u26, v26, u17, v17, u35, v35, u12, v12;

    col += offset;

    x0 = col[0*8];
    x1 = col[1*8];
    x2 = col[2*8];
    x3 = col[3*8];
    x4 = col[4*8];
    x5 = col[5*8];
    x6 = col[6*8];
    x7 = col[7*8];

    u04 = S (x0 + x4);
    v04 = S (x0 - x4);
    u26 = S (F (T2, x6) + x2);
    v26 = S (F (T2, x2) - x6);

    a0 = S (u04 + u26);
    a1 = S (v04 + v26);
    a2 = S (v04 - v26);
    a3 = S (u04 - u26);

    u17 = S (F (T1, x7) + x1);
    v17 = S (F (T1, x1) - x7);
    u35 = S (F (T3, x5) + x3);
    v35 = S (F (T3, x3) - x5);

    b0 = S (u17 + u35);
    b3 = S (v17 - v35);
    u12 = S (u17 - u35);
    v12 = S (v17 + v35);
    u12 = S (2 * F (C4, u12));
    v12 = S (2 * F (C4, v12));
    b1 = S (u12 + v12);
    b2 = S (u12 - v12);

    y0 = S (a0 + b0) >> COL_SHIFT;
    y1 = S (a1 + b1) >> COL_SHIFT;
    y2 = S (a2 + b2) >> COL_SHIFT;
    y3 = S (a3 + b3) >> COL_SHIFT;

    y4 = S (a3 - b3) >> COL_SHIFT;
    y5 = S (a2 - b2) >> COL_SHIFT;
    y6 = S (a1 - b1) >> COL_SHIFT;
    y7 = S (a0 - b0) >> COL_SHIFT;

    col[0*8] = y0;
    col[1*8] = y1;
    col[2*8] = y2;
    col[3*8] = y3;
    col[4*8] = y4;
    col[5*8] = y5;
    col[6*8] = y6;
    col[7*8] = y7;
}
#endif


// MMX column IDCT
static inline void idct_col (int16_t * col, int offset)
{
#define T1 13036
#define T2 27146
#define T3 43790
#define C4 23170

    static short _T1[] ATTR_ALIGN(8) = {T1,T1,T1,T1};
    static short _T2[] ATTR_ALIGN(8) = {T2,T2,T2,T2};
    static short _T3[] ATTR_ALIGN(8) = {T3,T3,T3,T3};
    static short _C4[] ATTR_ALIGN(8) = {C4,C4,C4,C4};

    /* column code adapted from peter gubanov */
    /* http://www.elecard.com/peter/idct.shtml */

    movq_m2r (*_T1, mm0);		// mm0 = T1

    movq_m2r (*(col+offset+1*8), mm1);	// mm1 = x1
    movq_r2r (mm0, mm2);		// mm2 = T1

    movq_m2r (*(col+offset+7*8), mm4);	// mm4 = x7
    pmulhw_r2r (mm1, mm0);		// mm0 = T1*x1

    movq_m2r (*_T3, mm5);		// mm5 = T3
    pmulhw_r2r (mm4, mm2);		// mm2 = T1*x7

    movq_m2r (*(col+offset+5*8), mm6);	// mm6 = x5
    movq_r2r (mm5, mm7);		// mm7 = T3-1

    movq_m2r (*(col+offset+3*8), mm3);	// mm3 = x3
    psubsw_r2r (mm4, mm0);		// mm0 = v17

    movq_m2r (*_T2, mm4);		// mm4 = T2
    pmulhw_r2r (mm3, mm5);		// mm5 = (T3-1)*x3

    paddsw_r2r (mm2, mm1);		// mm1 = u17
    pmulhw_r2r (mm6, mm7);		// mm7 = (T3-1)*x5

    /* slot */

    movq_r2r (mm4, mm2);		// mm2 = T2
    paddsw_r2r (mm3, mm5);		// mm5 = T3*x3

    pmulhw_m2r (*(col+offset+2*8), mm4);// mm4 = T2*x2
    paddsw_r2r (mm6, mm7);		// mm7 = T3*x5

    psubsw_r2r (mm6, mm5);		// mm5 = v35
    paddsw_r2r (mm3, mm7);		// mm7 = u35

    movq_m2r (*(col+offset+6*8), mm3);	// mm3 = x6
    movq_r2r (mm0, mm6);		// mm6 = v17

    pmulhw_r2r (mm3, mm2);		// mm2 = T2*x6
    psubsw_r2r (mm5, mm0);		// mm0 = b3

    psubsw_r2r (mm3, mm4);		// mm4 = v26
    paddsw_r2r (mm6, mm5);		// mm5 = v12

    movq_r2m (mm0, *(col+offset+3*8));	// save b3 in scratch0
    movq_r2r (mm1, mm6);		// mm6 = u17

    paddsw_m2r (*(col+offset+2*8), mm2);// mm2 = u26
    paddsw_r2r (mm7, mm6);		// mm6 = b0

    psubsw_r2r (mm7, mm1);		// mm1 = u12
    movq_r2r (mm1, mm7);		// mm7 = u12

    movq_m2r (*(col+offset+0*8), mm3);	// mm3 = x0
    paddsw_r2r (mm5, mm1);		// mm1 = u12+v12

    movq_m2r (*_C4, mm0);		// mm0 = C4/2
    psubsw_r2r (mm5, mm7);		// mm7 = u12-v12

    movq_r2m (mm6, *(col+offset+5*8));	// save b0 in scratch1
    pmulhw_r2r (mm0, mm1);		// mm1 = b1/2

    movq_r2r (mm4, mm6);		// mm6 = v26
    pmulhw_r2r (mm0, mm7);		// mm7 = b2/2

    movq_m2r (*(col+offset+4*8), mm5);	// mm5 = x4
    movq_r2r (mm3, mm0);		// mm0 = x0

    psubsw_r2r (mm5, mm3);		// mm3 = v04
    paddsw_r2r (mm5, mm0);		// mm0 = u04

    paddsw_r2r (mm3, mm4);		// mm4 = a1
    movq_r2r (mm0, mm5);		// mm5 = u04

    psubsw_r2r (mm6, mm3);		// mm3 = a2
    paddsw_r2r (mm2, mm5);		// mm5 = a0

    paddsw_r2r (mm1, mm1);		// mm1 = b1
    psubsw_r2r (mm2, mm0);		// mm0 = a3

    paddsw_r2r (mm7, mm7);		// mm7 = b2
    movq_r2r (mm3, mm2);		// mm2 = a2

    movq_r2r (mm4, mm6);		// mm6 = a1
    paddsw_r2r (mm7, mm3);		// mm3 = a2+b2

    psraw_i2r (COL_SHIFT, mm3);		// mm3 = y2
    paddsw_r2r (mm1, mm4);		// mm4 = a1+b1

    psraw_i2r (COL_SHIFT, mm4);		// mm4 = y1
    psubsw_r2r (mm1, mm6);		// mm6 = a1-b1

    movq_m2r (*(col+offset+5*8), mm1);	// mm1 = b0
    psubsw_r2r (mm7, mm2);		// mm2 = a2-b2

    psraw_i2r (COL_SHIFT, mm6);		// mm6 = y6
    movq_r2r (mm5, mm7);		// mm7 = a0

    movq_r2m (mm4, *(col+offset+1*8));	// save y1
    psraw_i2r (COL_SHIFT, mm2);		// mm2 = y5

    movq_r2m (mm3, *(col+offset+2*8));	// save y2
    paddsw_r2r (mm1, mm5);		// mm5 = a0+b0

    movq_m2r (*(col+offset+3*8), mm4);	// mm4 = b3
    psubsw_r2r (mm1, mm7);		// mm7 = a0-b0

    psraw_i2r (COL_SHIFT, mm5);		// mm5 = y0
    movq_r2r (mm0, mm3);		// mm3 = a3

    movq_r2m (mm2, *(col+offset+5*8));	// save y5
    psubsw_r2r (mm4, mm3);		// mm3 = a3-b3

    psraw_i2r (COL_SHIFT, mm7);		// mm7 = y7
    paddsw_r2r (mm0, mm4);		// mm4 = a3+b3

    movq_r2m (mm5, *(col+offset+0*8));	// save y0
    psraw_i2r (COL_SHIFT, mm3);		// mm3 = y4

    movq_r2m (mm6, *(col+offset+6*8));	// save y6
    psraw_i2r (COL_SHIFT, mm4);		// mm4 = y3

    movq_r2m (mm7, *(col+offset+7*8));	// save y7

    movq_r2m (mm3, *(col+offset+4*8));	// save y4

    movq_r2m (mm4, *(col+offset+3*8));	// save y3

#undef T1
#undef T2
#undef T3
#undef C4
}

static int32_t rounder0[] ATTR_ALIGN(8) =
    rounder ((1 << (COL_SHIFT - 1)) - 0.5);
static int32_t rounder4[] ATTR_ALIGN(8) = rounder (0);
static int32_t rounder1[] ATTR_ALIGN(8) =
    rounder (1.25683487303);	/* C1*(C1/C4+C1+C7)/2 */
static int32_t rounder7[] ATTR_ALIGN(8) =
    rounder (-0.25);		/* C1*(C7/C4+C7-C1)/2 */
static int32_t rounder2[] ATTR_ALIGN(8) =
    rounder (0.60355339059);	/* C2 * (C6+C2)/2 */
static int32_t rounder6[] ATTR_ALIGN(8) =
    rounder (-0.25);		/* C2 * (C6-C2)/2 */
static int32_t rounder3[] ATTR_ALIGN(8) =
    rounder (0.087788325588);	/* C3*(-C3/C4+C3+C5)/2 */
static int32_t rounder5[] ATTR_ALIGN(8) =
    rounder (-0.441341716183);	/* C3*(-C5/C4+C5-C3)/2 */

#undef COL_SHIFT
#undef ROW_SHIFT

#define declare_idct(idct,table,idct_row_head,idct_row,idct_row_tail,idct_row_mid)	\
void idct (int16_t * block)					\
{									\
    static int16_t table04[] ATTR_ALIGN(16) =				\
	table (22725, 21407, 19266, 16384, 12873,  8867, 4520);		\
    static int16_t table17[] ATTR_ALIGN(16) =				\
	table (31521, 29692, 26722, 22725, 17855, 12299, 6270);		\
    static int16_t table26[] ATTR_ALIGN(16) =				\
	table (29692, 27969, 25172, 21407, 16819, 11585, 5906);		\
    static int16_t table35[] ATTR_ALIGN(16) =				\
	table (26722, 25172, 22654, 19266, 15137, 10426, 5315);		\
									\
    idct_row_head (block, 0*8, table04);				\
    idct_row (table04, rounder0);					\
    idct_row_mid (block, 0*8, 4*8, table04);				\
    idct_row (table04, rounder4);					\
    idct_row_mid (block, 4*8, 1*8, table17);				\
    idct_row (table17, rounder1);					\
    idct_row_mid (block, 1*8, 7*8, table17);				\
    idct_row (table17, rounder7);					\
    idct_row_mid (block, 7*8, 2*8, table26);				\
    idct_row (table26, rounder2);					\
    idct_row_mid (block, 2*8, 6*8, table26);				\
    idct_row (table26, rounder6);					\
    idct_row_mid (block, 6*8, 3*8, table35);				\
    idct_row (table35, rounder3);					\
    idct_row_mid (block, 3*8, 5*8, table35);				\
    idct_row (table35, rounder5);					\
    idct_row_tail (block, 5*8);						\
									\
    idct_col (block, 0);						\
    idct_col (block, 4);						\
}

void ff_mmx_idct(DCTELEM *block);
void ff_mmxext_idct(DCTELEM *block);

declare_idct (ff_mmxext_idct, mmxext_table,
	      mmxext_row_head, mmxext_row, mmxext_row_tail, mmxext_row_mid)

declare_idct (ff_mmx_idct, mmx_table,
	      mmx_row_head, mmx_row, mmx_row_tail, mmx_row_mid)


		  #include "../simple_idct.h"

/*
23170.475006
22725.260826
21406.727617
19265.545870
16384.000000
12872.826198
8866.956905
4520.335430
*/
#define C0 23170 //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5
#define C1 22725 //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5
#define C2 21407 //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5
#define C3 19266 //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5
#if 0
#define C4 16384 //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5
#else
#define C4 16383 //cos(i*M_PI/16)*sqrt(2)*(1<<14) - 0.5
#endif
#define C5 12873 //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5
#define C6 8867 //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5
#define C7 4520 //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5

#define ROW_SHIFT 11
#define COL_SHIFT 20 // 6

static const uint64_t attribute_used __attribute__((aligned(8))) wm1010= 0xFFFF0000FFFF0000ULL;
static const uint64_t attribute_used __attribute__((aligned(8))) d40000= 0x0000000000040000ULL;

static const int16_t __attribute__((aligned(8))) coeffs[]= {
	1<<(ROW_SHIFT-1), 0, 1<<(ROW_SHIFT-1), 0,
//	1<<(COL_SHIFT-1), 0, 1<<(COL_SHIFT-1), 0,
//	0, 1<<(COL_SHIFT-1-16), 0, 1<<(COL_SHIFT-1-16),
	1<<(ROW_SHIFT-1), 1, 1<<(ROW_SHIFT-1), 0,
	// the 1 = ((1<<(COL_SHIFT-1))/C4)<<ROW_SHIFT :)
//	0, 0, 0, 0,
//	0, 0, 0, 0,

 C4,  C4,  C4,  C4,
 C4, -C4,  C4, -C4,
 
 C2,  C6,  C2,  C6,
 C6, -C2,  C6, -C2,
 
 C1,  C3,  C1,  C3,
 C5,  C7,  C5,  C7,
 
 C3, -C7,  C3, -C7,
-C1, -C5, -C1, -C5,
 
 C5, -C1,  C5, -C1,
 C7,  C3,  C7,  C3,
 
 C7, -C5,  C7, -C5,
 C3, -C1,  C3, -C1
};

#if 0
static void unused_var_killer(){
	int a= wm1010 + d40000;
	temp[0]=a;
}

static void inline idctCol (int16_t * col, int16_t *input)
{
#undef C0
#undef C1
#undef C2
#undef C3
#undef C4
#undef C5
#undef C6
#undef C7
	int a0, a1, a2, a3, b0, b1, b2, b3;
	const int C0 = 23170; //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5
	const int C1 = 22725; //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5
	const int C2 = 21407; //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5
	const int C3 = 19266; //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5
	const int C4 = 16383; //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5
	const int C5 = 12873; //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5
	const int C6 = 8867; //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5
	const int C7 = 4520; //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5
/*
	if( !(col[8*1] | col[8*2] |col[8*3] |col[8*4] |col[8*5] |col[8*6] | col[8*7])) {
		col[8*0] = col[8*1] = col[8*2] = col[8*3] = col[8*4] =
			col[8*5] = col[8*6] = col[8*7] = col[8*0]<<3;
		return;
	}*/

col[8*0] = input[8*0 + 0];
col[8*1] = input[8*2 + 0];
col[8*2] = input[8*0 + 1];
col[8*3] = input[8*2 + 1];
col[8*4] = input[8*4 + 0];
col[8*5] = input[8*6 + 0];
col[8*6] = input[8*4 + 1];
col[8*7] = input[8*6 + 1];

	a0 = C4*col[8*0] + C2*col[8*2] + C4*col[8*4] + C6*col[8*6] + (1<<(COL_SHIFT-1));
	a1 = C4*col[8*0] + C6*col[8*2] - C4*col[8*4] - C2*col[8*6] + (1<<(COL_SHIFT-1));
	a2 = C4*col[8*0] - C6*col[8*2] - C4*col[8*4] + C2*col[8*6] + (1<<(COL_SHIFT-1));
	a3 = C4*col[8*0] - C2*col[8*2] + C4*col[8*4] - C6*col[8*6] + (1<<(COL_SHIFT-1));

	b0 = C1*col[8*1] + C3*col[8*3] + C5*col[8*5] + C7*col[8*7];
	b1 = C3*col[8*1] - C7*col[8*3] - C1*col[8*5] - C5*col[8*7];
	b2 = C5*col[8*1] - C1*col[8*3] + C7*col[8*5] + C3*col[8*7];
	b3 = C7*col[8*1] - C5*col[8*3] + C3*col[8*5] - C1*col[8*7];

	col[8*0] = (a0 + b0) >> COL_SHIFT;
	col[8*1] = (a1 + b1) >> COL_SHIFT;
	col[8*2] = (a2 + b2) >> COL_SHIFT;
	col[8*3] = (a3 + b3) >> COL_SHIFT;
	col[8*4] = (a3 - b3) >> COL_SHIFT;
	col[8*5] = (a2 - b2) >> COL_SHIFT;
	col[8*6] = (a1 - b1) >> COL_SHIFT;
	col[8*7] = (a0 - b0) >> COL_SHIFT;
}

static void inline idctRow (int16_t * output, int16_t * input)
{
	int16_t row[8];

	int a0, a1, a2, a3, b0, b1, b2, b3;
	const int C0 = 23170; //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5
	const int C1 = 22725; //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5
	const int C2 = 21407; //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5
	const int C3 = 19266; //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5
	const int C4 = 16383; //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5
	const int C5 = 12873; //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5
	const int C6 = 8867; //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5
	const int C7 = 4520; //cos(i*M_PI/16)*sqrt(2)*(1<<14) + 0.5

row[0] = input[0];
row[2] = input[1];
row[4] = input[4];
row[6] = input[5];
row[1] = input[8];
row[3] = input[9];
row[5] = input[12];
row[7] = input[13];

	if( !(row[1] | row[2] |row[3] |row[4] |row[5] |row[6] | row[7]) ) {
		row[0] = row[1] = row[2] = row[3] = row[4] =
			row[5] = row[6] = row[7] = row[0]<<3;
	output[0] = row[0];
	output[2] = row[1];
	output[4] = row[2];
	output[6] = row[3];
	output[8] = row[4];
	output[10] = row[5];
	output[12] = row[6];
	output[14] = row[7];
		return;
	}

	a0 = C4*row[0] + C2*row[2] + C4*row[4] + C6*row[6] + (1<<(ROW_SHIFT-1));
	a1 = C4*row[0] + C6*row[2] - C4*row[4] - C2*row[6] + (1<<(ROW_SHIFT-1));
	a2 = C4*row[0] - C6*row[2] - C4*row[4] + C2*row[6] + (1<<(ROW_SHIFT-1));
	a3 = C4*row[0] - C2*row[2] + C4*row[4] - C6*row[6] + (1<<(ROW_SHIFT-1));

	b0 = C1*row[1] + C3*row[3] + C5*row[5] + C7*row[7];
	b1 = C3*row[1] - C7*row[3] - C1*row[5] - C5*row[7];
	b2 = C5*row[1] - C1*row[3] + C7*row[5] + C3*row[7];
	b3 = C7*row[1] - C5*row[3] + C3*row[5] - C1*row[7];

	row[0] = (a0 + b0) >> ROW_SHIFT;
	row[1] = (a1 + b1) >> ROW_SHIFT;
	row[2] = (a2 + b2) >> ROW_SHIFT;
	row[3] = (a3 + b3) >> ROW_SHIFT;
	row[4] = (a3 - b3) >> ROW_SHIFT;
	row[5] = (a2 - b2) >> ROW_SHIFT;
	row[6] = (a1 - b1) >> ROW_SHIFT;
	row[7] = (a0 - b0) >> ROW_SHIFT;

	output[0] = row[0];
	output[2] = row[1];
	output[4] = row[2];
	output[6] = row[3];
	output[8] = row[4];
	output[10] = row[5];
	output[12] = row[6];
	output[14] = row[7];
}
#endif

static inline void idct(int16_t *block)
{
	int64_t __attribute__((aligned(8))) align_tmp[16];
	int16_t * const temp= (int16_t*)align_tmp;

	asm volatile(
#if 0 //Alternative, simpler variant

#define ROW_IDCT(src0, src4, src1, src5, dst, rounder, shift) \
	"movq " #src0 ", %%mm0			\n\t" /* R4	R0	r4	r0 */\
	"movq " #src4 ", %%mm1			\n\t" /* R6	R2	r6	r2 */\
	"movq " #src1 ", %%mm2			\n\t" /* R3	R1	r3	r1 */\
	"movq " #src5 ", %%mm3			\n\t" /* R7	R5	r7	r5 */\
	"movq 16(%2), %%mm4			\n\t" /* C4	C4	C4	C4 */\
	"pmaddwd %%mm0, %%mm4			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"movq 24(%2), %%mm5			\n\t" /* -C4	C4	-C4	C4 */\
	"pmaddwd %%mm5, %%mm0			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	"movq 32(%2), %%mm5			\n\t" /* C6	C2	C6	C2 */\
	"pmaddwd %%mm1, %%mm5			\n\t" /* C6R6+C2R2	C6r6+C2r2 */\
	"movq 40(%2), %%mm6			\n\t" /* -C2	C6	-C2	C6 */\
	"pmaddwd %%mm6, %%mm1			\n\t" /* -C2R6+C6R2	-C2r6+C6r2 */\
	"movq 48(%2), %%mm7			\n\t" /* C3	C1	C3	C1 */\
	"pmaddwd %%mm2, %%mm7			\n\t" /* C3R3+C1R1	C3r3+C1r1 */\
	#rounder ", %%mm4			\n\t"\
	"movq %%mm4, %%mm6			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"paddd %%mm5, %%mm4			\n\t" /* A0		a0 */\
	"psubd %%mm5, %%mm6			\n\t" /* A3		a3 */\
	"movq 56(%2), %%mm5			\n\t" /* C7	C5	C7	C5 */\
	"pmaddwd %%mm3, %%mm5			\n\t" /* C7R7+C5R5	C7r7+C5r5 */\
	#rounder ", %%mm0			\n\t"\
	"paddd %%mm0, %%mm1			\n\t" /* A1		a1 */\
	"paddd %%mm0, %%mm0			\n\t" \
	"psubd %%mm1, %%mm0			\n\t" /* A2		a2 */\
	"pmaddwd 64(%2), %%mm2			\n\t" /* -C7R3+C3R1	-C7r3+C3r1 */\
	"paddd %%mm5, %%mm7			\n\t" /* B0		b0 */\
	"movq 72(%2), %%mm5			\n\t" /* -C5	-C1	-C5	-C1 */\
	"pmaddwd %%mm3, %%mm5			\n\t" /* -C5R7-C1R5	-C5r7-C1r5 */\
	"paddd %%mm4, %%mm7			\n\t" /* A0+B0		a0+b0 */\
	"paddd %%mm4, %%mm4			\n\t" /* 2A0		2a0 */\
	"psubd %%mm7, %%mm4			\n\t" /* A0-B0		a0-b0 */\
	"paddd %%mm2, %%mm5			\n\t" /* B1		b1 */\
	"psrad $" #shift ", %%mm7		\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"movq %%mm1, %%mm2			\n\t" /* A1		a1 */\
	"paddd %%mm5, %%mm1			\n\t" /* A1+B1		a1+b1 */\
	"psubd %%mm5, %%mm2			\n\t" /* A1-B1		a1-b1 */\
	"psrad $" #shift ", %%mm1		\n\t"\
	"psrad $" #shift ", %%mm2		\n\t"\
	"packssdw %%mm1, %%mm7			\n\t" /* A1+B1	a1+b1	A0+B0	a0+b0 */\
	"packssdw %%mm4, %%mm2			\n\t" /* A0-B0	a0-b0	A1-B1	a1-b1 */\
	"movq %%mm7, " #dst "			\n\t"\
	"movq " #src1 ", %%mm1			\n\t" /* R3	R1	r3	r1 */\
	"movq 80(%2), %%mm4			\n\t" /* -C1	C5	-C1 	C5 */\
	"movq %%mm2, 24+" #dst "		\n\t"\
	"pmaddwd %%mm1, %%mm4			\n\t" /* -C1R3+C5R1	-C1r3+C5r1 */\
	"movq 88(%2), %%mm7			\n\t" /* C3	C7	C3 	C7 */\
	"pmaddwd 96(%2), %%mm1			\n\t" /* -C5R3+C7R1	-C5r3+C7r1 */\
	"pmaddwd %%mm3, %%mm7			\n\t" /* C3R7+C7R5	C3r7+C7r5 */\
	"movq %%mm0, %%mm2			\n\t" /* A2		a2 */\
	"pmaddwd 104(%2), %%mm3			\n\t" /* -C1R7+C3R5	-C1r7+C3r5 */\
	"paddd %%mm7, %%mm4			\n\t" /* B2		b2 */\
	"paddd %%mm4, %%mm2			\n\t" /* A2+B2		a2+b2 */\
	"psubd %%mm4, %%mm0			\n\t" /* a2-B2		a2-b2 */\
	"psrad $" #shift ", %%mm2		\n\t"\
	"psrad $" #shift ", %%mm0		\n\t"\
	"movq %%mm6, %%mm4			\n\t" /* A3		a3 */\
	"paddd %%mm1, %%mm3			\n\t" /* B3		b3 */\
	"paddd %%mm3, %%mm6			\n\t" /* A3+B3		a3+b3 */\
	"psubd %%mm3, %%mm4			\n\t" /* a3-B3		a3-b3 */\
	"psrad $" #shift ", %%mm6		\n\t"\
	"packssdw %%mm6, %%mm2			\n\t" /* A3+B3	a3+b3	A2+B2	a2+b2 */\
	"movq %%mm2, 8+" #dst "			\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"packssdw %%mm0, %%mm4			\n\t" /* A2-B2	a2-b2	A3-B3	a3-b3 */\
	"movq %%mm4, 16+" #dst "		\n\t"\

#define COL_IDCT(src0, src4, src1, src5, dst, rounder, shift) \
	"movq " #src0 ", %%mm0			\n\t" /* R4	R0	r4	r0 */\
	"movq " #src4 ", %%mm1			\n\t" /* R6	R2	r6	r2 */\
	"movq " #src1 ", %%mm2			\n\t" /* R3	R1	r3	r1 */\
	"movq " #src5 ", %%mm3			\n\t" /* R7	R5	r7	r5 */\
	"movq 16(%2), %%mm4			\n\t" /* C4	C4	C4	C4 */\
	"pmaddwd %%mm0, %%mm4			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"movq 24(%2), %%mm5			\n\t" /* -C4	C4	-C4	C4 */\
	"pmaddwd %%mm5, %%mm0			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	"movq 32(%2), %%mm5			\n\t" /* C6	C2	C6	C2 */\
	"pmaddwd %%mm1, %%mm5			\n\t" /* C6R6+C2R2	C6r6+C2r2 */\
	"movq 40(%2), %%mm6			\n\t" /* -C2	C6	-C2	C6 */\
	"pmaddwd %%mm6, %%mm1			\n\t" /* -C2R6+C6R2	-C2r6+C6r2 */\
	#rounder ", %%mm4			\n\t"\
	"movq %%mm4, %%mm6			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"movq 48(%2), %%mm7			\n\t" /* C3	C1	C3	C1 */\
	#rounder ", %%mm0			\n\t"\
	"pmaddwd %%mm2, %%mm7			\n\t" /* C3R3+C1R1	C3r3+C1r1 */\
	"paddd %%mm5, %%mm4			\n\t" /* A0		a0 */\
	"psubd %%mm5, %%mm6			\n\t" /* A3		a3 */\
	"movq %%mm0, %%mm5			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	"paddd %%mm1, %%mm0			\n\t" /* A1		a1 */\
	"psubd %%mm1, %%mm5			\n\t" /* A2		a2 */\
	"movq 56(%2), %%mm1			\n\t" /* C7	C5	C7	C5 */\
	"pmaddwd %%mm3, %%mm1			\n\t" /* C7R7+C5R5	C7r7+C5r5 */\
	"pmaddwd 64(%2), %%mm2			\n\t" /* -C7R3+C3R1	-C7r3+C3r1 */\
	"paddd %%mm1, %%mm7			\n\t" /* B0		b0 */\
	"movq 72(%2), %%mm1			\n\t" /* -C5	-C1	-C5	-C1 */\
	"pmaddwd %%mm3, %%mm1			\n\t" /* -C5R7-C1R5	-C5r7-C1r5 */\
	"paddd %%mm4, %%mm7			\n\t" /* A0+B0		a0+b0 */\
	"paddd %%mm4, %%mm4			\n\t" /* 2A0		2a0 */\
	"psubd %%mm7, %%mm4			\n\t" /* A0-B0		a0-b0 */\
	"paddd %%mm2, %%mm1			\n\t" /* B1		b1 */\
	"psrad $" #shift ", %%mm7		\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"movq %%mm0, %%mm2			\n\t" /* A1		a1 */\
	"paddd %%mm1, %%mm0			\n\t" /* A1+B1		a1+b1 */\
	"psubd %%mm1, %%mm2			\n\t" /* A1-B1		a1-b1 */\
	"psrad $" #shift ", %%mm0		\n\t"\
	"psrad $" #shift ", %%mm2		\n\t"\
	"packssdw %%mm7, %%mm7			\n\t" /* A0+B0	a0+b0 */\
	"movd %%mm7, " #dst "			\n\t"\
	"packssdw %%mm0, %%mm0			\n\t" /* A1+B1	a1+b1 */\
	"movd %%mm0, 16+" #dst "		\n\t"\
	"packssdw %%mm2, %%mm2			\n\t" /* A1-B1	a1-b1 */\
	"movd %%mm2, 96+" #dst "		\n\t"\
	"packssdw %%mm4, %%mm4			\n\t" /* A0-B0	a0-b0 */\
	"movd %%mm4, 112+" #dst "		\n\t"\
	"movq " #src1 ", %%mm0			\n\t" /* R3	R1	r3	r1 */\
	"movq 80(%2), %%mm4			\n\t" /* -C1	C5	-C1 	C5 */\
	"pmaddwd %%mm0, %%mm4			\n\t" /* -C1R3+C5R1	-C1r3+C5r1 */\
	"movq 88(%2), %%mm7			\n\t" /* C3	C7	C3 	C7 */\
	"pmaddwd 96(%2), %%mm0			\n\t" /* -C5R3+C7R1	-C5r3+C7r1 */\
	"pmaddwd %%mm3, %%mm7			\n\t" /* C3R7+C7R5	C3r7+C7r5 */\
	"movq %%mm5, %%mm2			\n\t" /* A2		a2 */\
	"pmaddwd 104(%2), %%mm3			\n\t" /* -C1R7+C3R5	-C1r7+C3r5 */\
	"paddd %%mm7, %%mm4			\n\t" /* B2		b2 */\
	"paddd %%mm4, %%mm2			\n\t" /* A2+B2		a2+b2 */\
	"psubd %%mm4, %%mm5			\n\t" /* a2-B2		a2-b2 */\
	"psrad $" #shift ", %%mm2		\n\t"\
	"psrad $" #shift ", %%mm5		\n\t"\
	"movq %%mm6, %%mm4			\n\t" /* A3		a3 */\
	"paddd %%mm0, %%mm3			\n\t" /* B3		b3 */\
	"paddd %%mm3, %%mm6			\n\t" /* A3+B3		a3+b3 */\
	"psubd %%mm3, %%mm4			\n\t" /* a3-B3		a3-b3 */\
	"psrad $" #shift ", %%mm6		\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"packssdw %%mm2, %%mm2			\n\t" /* A2+B2	a2+b2 */\
	"packssdw %%mm6, %%mm6			\n\t" /* A3+B3	a3+b3 */\
	"movd %%mm2, 32+" #dst "		\n\t"\
	"packssdw %%mm4, %%mm4			\n\t" /* A3-B3	a3-b3 */\
	"packssdw %%mm5, %%mm5			\n\t" /* A2-B2	a2-b2 */\
	"movd %%mm6, 48+" #dst "		\n\t"\
	"movd %%mm4, 64+" #dst "		\n\t"\
	"movd %%mm5, 80+" #dst "		\n\t"\

	
#define DC_COND_ROW_IDCT(src0, src4, src1, src5, dst, rounder, shift) \
	"movq " #src0 ", %%mm0			\n\t" /* R4	R0	r4	r0 */\
	"movq " #src4 ", %%mm1			\n\t" /* R6	R2	r6	r2 */\
	"movq " #src1 ", %%mm2			\n\t" /* R3	R1	r3	r1 */\
	"movq " #src5 ", %%mm3			\n\t" /* R7	R5	r7	r5 */\
	"movq "MANGLE(wm1010)", %%mm4		\n\t"\
	"pand %%mm0, %%mm4			\n\t"\
	"por %%mm1, %%mm4			\n\t"\
	"por %%mm2, %%mm4			\n\t"\
	"por %%mm3, %%mm4			\n\t"\
	"packssdw %%mm4,%%mm4			\n\t"\
	"movd %%mm4, %%eax			\n\t"\
	"orl %%eax, %%eax			\n\t"\
	"jz 1f					\n\t"\
	"movq 16(%2), %%mm4			\n\t" /* C4	C4	C4	C4 */\
	"pmaddwd %%mm0, %%mm4			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"movq 24(%2), %%mm5			\n\t" /* -C4	C4	-C4	C4 */\
	"pmaddwd %%mm5, %%mm0			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	"movq 32(%2), %%mm5			\n\t" /* C6	C2	C6	C2 */\
	"pmaddwd %%mm1, %%mm5			\n\t" /* C6R6+C2R2	C6r6+C2r2 */\
	"movq 40(%2), %%mm6			\n\t" /* -C2	C6	-C2	C6 */\
	"pmaddwd %%mm6, %%mm1			\n\t" /* -C2R6+C6R2	-C2r6+C6r2 */\
	"movq 48(%2), %%mm7			\n\t" /* C3	C1	C3	C1 */\
	"pmaddwd %%mm2, %%mm7			\n\t" /* C3R3+C1R1	C3r3+C1r1 */\
	#rounder ", %%mm4			\n\t"\
	"movq %%mm4, %%mm6			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"paddd %%mm5, %%mm4			\n\t" /* A0		a0 */\
	"psubd %%mm5, %%mm6			\n\t" /* A3		a3 */\
	"movq 56(%2), %%mm5			\n\t" /* C7	C5	C7	C5 */\
	"pmaddwd %%mm3, %%mm5			\n\t" /* C7R7+C5R5	C7r7+C5r5 */\
	#rounder ", %%mm0			\n\t"\
	"paddd %%mm0, %%mm1			\n\t" /* A1		a1 */\
	"paddd %%mm0, %%mm0			\n\t" \
	"psubd %%mm1, %%mm0			\n\t" /* A2		a2 */\
	"pmaddwd 64(%2), %%mm2			\n\t" /* -C7R3+C3R1	-C7r3+C3r1 */\
	"paddd %%mm5, %%mm7			\n\t" /* B0		b0 */\
	"movq 72(%2), %%mm5			\n\t" /* -C5	-C1	-C5	-C1 */\
	"pmaddwd %%mm3, %%mm5			\n\t" /* -C5R7-C1R5	-C5r7-C1r5 */\
	"paddd %%mm4, %%mm7			\n\t" /* A0+B0		a0+b0 */\
	"paddd %%mm4, %%mm4			\n\t" /* 2A0		2a0 */\
	"psubd %%mm7, %%mm4			\n\t" /* A0-B0		a0-b0 */\
	"paddd %%mm2, %%mm5			\n\t" /* B1		b1 */\
	"psrad $" #shift ", %%mm7		\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"movq %%mm1, %%mm2			\n\t" /* A1		a1 */\
	"paddd %%mm5, %%mm1			\n\t" /* A1+B1		a1+b1 */\
	"psubd %%mm5, %%mm2			\n\t" /* A1-B1		a1-b1 */\
	"psrad $" #shift ", %%mm1		\n\t"\
	"psrad $" #shift ", %%mm2		\n\t"\
	"packssdw %%mm1, %%mm7			\n\t" /* A1+B1	a1+b1	A0+B0	a0+b0 */\
	"packssdw %%mm4, %%mm2			\n\t" /* A0-B0	a0-b0	A1-B1	a1-b1 */\
	"movq %%mm7, " #dst "			\n\t"\
	"movq " #src1 ", %%mm1			\n\t" /* R3	R1	r3	r1 */\
	"movq 80(%2), %%mm4			\n\t" /* -C1	C5	-C1 	C5 */\
	"movq %%mm2, 24+" #dst "		\n\t"\
	"pmaddwd %%mm1, %%mm4			\n\t" /* -C1R3+C5R1	-C1r3+C5r1 */\
	"movq 88(%2), %%mm7			\n\t" /* C3	C7	C3 	C7 */\
	"pmaddwd 96(%2), %%mm1			\n\t" /* -C5R3+C7R1	-C5r3+C7r1 */\
	"pmaddwd %%mm3, %%mm7			\n\t" /* C3R7+C7R5	C3r7+C7r5 */\
	"movq %%mm0, %%mm2			\n\t" /* A2		a2 */\
	"pmaddwd 104(%2), %%mm3			\n\t" /* -C1R7+C3R5	-C1r7+C3r5 */\
	"paddd %%mm7, %%mm4			\n\t" /* B2		b2 */\
	"paddd %%mm4, %%mm2			\n\t" /* A2+B2		a2+b2 */\
	"psubd %%mm4, %%mm0			\n\t" /* a2-B2		a2-b2 */\
	"psrad $" #shift ", %%mm2		\n\t"\
	"psrad $" #shift ", %%mm0		\n\t"\
	"movq %%mm6, %%mm4			\n\t" /* A3		a3 */\
	"paddd %%mm1, %%mm3			\n\t" /* B3		b3 */\
	"paddd %%mm3, %%mm6			\n\t" /* A3+B3		a3+b3 */\
	"psubd %%mm3, %%mm4			\n\t" /* a3-B3		a3-b3 */\
	"psrad $" #shift ", %%mm6		\n\t"\
	"packssdw %%mm6, %%mm2			\n\t" /* A3+B3	a3+b3	A2+B2	a2+b2 */\
	"movq %%mm2, 8+" #dst "			\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"packssdw %%mm0, %%mm4			\n\t" /* A2-B2	a2-b2	A3-B3	a3-b3 */\
	"movq %%mm4, 16+" #dst "		\n\t"\
	"jmp 2f					\n\t"\
	"1:					\n\t"\
	"pslld $16, %%mm0			\n\t"\
	"#paddd "MANGLE(d40000)", %%mm0		\n\t"\
	"psrad $13, %%mm0			\n\t"\
	"packssdw %%mm0, %%mm0			\n\t"\
	"movq %%mm0, " #dst "			\n\t"\
	"movq %%mm0, 8+" #dst "			\n\t"\
	"movq %%mm0, 16+" #dst "		\n\t"\
	"movq %%mm0, 24+" #dst "		\n\t"\
	"2:					\n\t"


//IDCT(      src0,   src4,   src1,   src5,    dst,    rounder, shift)
ROW_IDCT(    (%0),  8(%0), 16(%0), 24(%0),  0(%1),paddd 8(%2), 11)
/*ROW_IDCT(  32(%0), 40(%0), 48(%0), 56(%0), 32(%1), paddd (%2), 11)
ROW_IDCT(  64(%0), 72(%0), 80(%0), 88(%0), 64(%1), paddd (%2), 11)
ROW_IDCT(  96(%0),104(%0),112(%0),120(%0), 96(%1), paddd (%2), 11)*/

DC_COND_ROW_IDCT(  32(%0), 40(%0), 48(%0), 56(%0), 32(%1),paddd (%2), 11)
DC_COND_ROW_IDCT(  64(%0), 72(%0), 80(%0), 88(%0), 64(%1),paddd (%2), 11)
DC_COND_ROW_IDCT(  96(%0),104(%0),112(%0),120(%0), 96(%1),paddd (%2), 11)


//IDCT(      src0,   src4,   src1,    src5,    dst, rounder, shift)
COL_IDCT(    (%1), 64(%1), 32(%1),  96(%1),  0(%0),/nop, 20)
COL_IDCT(   8(%1), 72(%1), 40(%1), 104(%1),  4(%0),/nop, 20)
COL_IDCT(  16(%1), 80(%1), 48(%1), 112(%1),  8(%0),/nop, 20)
COL_IDCT(  24(%1), 88(%1), 56(%1), 120(%1), 12(%0),/nop, 20)

#else

#define DC_COND_IDCT(src0, src4, src1, src5, dst, rounder, shift) \
	"movq " #src0 ", %%mm0			\n\t" /* R4	R0	r4	r0 */\
	"movq " #src4 ", %%mm1			\n\t" /* R6	R2	r6	r2 */\
	"movq " #src1 ", %%mm2			\n\t" /* R3	R1	r3	r1 */\
	"movq " #src5 ", %%mm3			\n\t" /* R7	R5	r7	r5 */\
	"movq "MANGLE(wm1010)", %%mm4		\n\t"\
	"pand %%mm0, %%mm4			\n\t"\
	"por %%mm1, %%mm4			\n\t"\
	"por %%mm2, %%mm4			\n\t"\
	"por %%mm3, %%mm4			\n\t"\
	"packssdw %%mm4,%%mm4			\n\t"\
	"movd %%mm4, %%eax			\n\t"\
	"orl %%eax, %%eax			\n\t"\
	"jz 1f					\n\t"\
	"movq 16(%2), %%mm4			\n\t" /* C4	C4	C4	C4 */\
	"pmaddwd %%mm0, %%mm4			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"movq 24(%2), %%mm5			\n\t" /* -C4	C4	-C4	C4 */\
	"pmaddwd %%mm5, %%mm0			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	"movq 32(%2), %%mm5			\n\t" /* C6	C2	C6	C2 */\
	"pmaddwd %%mm1, %%mm5			\n\t" /* C6R6+C2R2	C6r6+C2r2 */\
	"movq 40(%2), %%mm6			\n\t" /* -C2	C6	-C2	C6 */\
	"pmaddwd %%mm6, %%mm1			\n\t" /* -C2R6+C6R2	-C2r6+C6r2 */\
	"movq 48(%2), %%mm7			\n\t" /* C3	C1	C3	C1 */\
	"pmaddwd %%mm2, %%mm7			\n\t" /* C3R3+C1R1	C3r3+C1r1 */\
	#rounder ", %%mm4			\n\t"\
	"movq %%mm4, %%mm6			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"paddd %%mm5, %%mm4			\n\t" /* A0		a0 */\
	"psubd %%mm5, %%mm6			\n\t" /* A3		a3 */\
	"movq 56(%2), %%mm5			\n\t" /* C7	C5	C7	C5 */\
	"pmaddwd %%mm3, %%mm5			\n\t" /* C7R7+C5R5	C7r7+C5r5 */\
	#rounder ", %%mm0			\n\t"\
	"paddd %%mm0, %%mm1			\n\t" /* A1		a1 */\
	"paddd %%mm0, %%mm0			\n\t" \
	"psubd %%mm1, %%mm0			\n\t" /* A2		a2 */\
	"pmaddwd 64(%2), %%mm2			\n\t" /* -C7R3+C3R1	-C7r3+C3r1 */\
	"paddd %%mm5, %%mm7			\n\t" /* B0		b0 */\
	"movq 72(%2), %%mm5			\n\t" /* -C5	-C1	-C5	-C1 */\
	"pmaddwd %%mm3, %%mm5			\n\t" /* -C5R7-C1R5	-C5r7-C1r5 */\
	"paddd %%mm4, %%mm7			\n\t" /* A0+B0		a0+b0 */\
	"paddd %%mm4, %%mm4			\n\t" /* 2A0		2a0 */\
	"psubd %%mm7, %%mm4			\n\t" /* A0-B0		a0-b0 */\
	"paddd %%mm2, %%mm5			\n\t" /* B1		b1 */\
	"psrad $" #shift ", %%mm7		\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"movq %%mm1, %%mm2			\n\t" /* A1		a1 */\
	"paddd %%mm5, %%mm1			\n\t" /* A1+B1		a1+b1 */\
	"psubd %%mm5, %%mm2			\n\t" /* A1-B1		a1-b1 */\
	"psrad $" #shift ", %%mm1		\n\t"\
	"psrad $" #shift ", %%mm2		\n\t"\
	"packssdw %%mm1, %%mm7			\n\t" /* A1+B1	a1+b1	A0+B0	a0+b0 */\
	"packssdw %%mm4, %%mm2			\n\t" /* A0-B0	a0-b0	A1-B1	a1-b1 */\
	"movq %%mm7, " #dst "			\n\t"\
	"movq " #src1 ", %%mm1			\n\t" /* R3	R1	r3	r1 */\
	"movq 80(%2), %%mm4			\n\t" /* -C1	C5	-C1 	C5 */\
	"movq %%mm2, 24+" #dst "		\n\t"\
	"pmaddwd %%mm1, %%mm4			\n\t" /* -C1R3+C5R1	-C1r3+C5r1 */\
	"movq 88(%2), %%mm7			\n\t" /* C3	C7	C3 	C7 */\
	"pmaddwd 96(%2), %%mm1			\n\t" /* -C5R3+C7R1	-C5r3+C7r1 */\
	"pmaddwd %%mm3, %%mm7			\n\t" /* C3R7+C7R5	C3r7+C7r5 */\
	"movq %%mm0, %%mm2			\n\t" /* A2		a2 */\
	"pmaddwd 104(%2), %%mm3			\n\t" /* -C1R7+C3R5	-C1r7+C3r5 */\
	"paddd %%mm7, %%mm4			\n\t" /* B2		b2 */\
	"paddd %%mm4, %%mm2			\n\t" /* A2+B2		a2+b2 */\
	"psubd %%mm4, %%mm0			\n\t" /* a2-B2		a2-b2 */\
	"psrad $" #shift ", %%mm2		\n\t"\
	"psrad $" #shift ", %%mm0		\n\t"\
	"movq %%mm6, %%mm4			\n\t" /* A3		a3 */\
	"paddd %%mm1, %%mm3			\n\t" /* B3		b3 */\
	"paddd %%mm3, %%mm6			\n\t" /* A3+B3		a3+b3 */\
	"psubd %%mm3, %%mm4			\n\t" /* a3-B3		a3-b3 */\
	"psrad $" #shift ", %%mm6		\n\t"\
	"packssdw %%mm6, %%mm2			\n\t" /* A3+B3	a3+b3	A2+B2	a2+b2 */\
	"movq %%mm2, 8+" #dst "			\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"packssdw %%mm0, %%mm4			\n\t" /* A2-B2	a2-b2	A3-B3	a3-b3 */\
	"movq %%mm4, 16+" #dst "		\n\t"\
	"jmp 2f					\n\t"\
	"1:					\n\t"\
	"pslld $16, %%mm0			\n\t"\
	"paddd "MANGLE(d40000)", %%mm0		\n\t"\
	"psrad $13, %%mm0			\n\t"\
	"packssdw %%mm0, %%mm0			\n\t"\
	"movq %%mm0, " #dst "			\n\t"\
	"movq %%mm0, 8+" #dst "			\n\t"\
	"movq %%mm0, 16+" #dst "		\n\t"\
	"movq %%mm0, 24+" #dst "		\n\t"\
	"2:					\n\t"

#define Z_COND_IDCT(src0, src4, src1, src5, dst, rounder, shift, bt) \
	"movq " #src0 ", %%mm0			\n\t" /* R4	R0	r4	r0 */\
	"movq " #src4 ", %%mm1			\n\t" /* R6	R2	r6	r2 */\
	"movq " #src1 ", %%mm2			\n\t" /* R3	R1	r3	r1 */\
	"movq " #src5 ", %%mm3			\n\t" /* R7	R5	r7	r5 */\
	"movq %%mm0, %%mm4			\n\t"\
	"por %%mm1, %%mm4			\n\t"\
	"por %%mm2, %%mm4			\n\t"\
	"por %%mm3, %%mm4			\n\t"\
	"packssdw %%mm4,%%mm4			\n\t"\
	"movd %%mm4, %%eax			\n\t"\
	"orl %%eax, %%eax			\n\t"\
	"jz " #bt "				\n\t"\
	"movq 16(%2), %%mm4			\n\t" /* C4	C4	C4	C4 */\
	"pmaddwd %%mm0, %%mm4			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"movq 24(%2), %%mm5			\n\t" /* -C4	C4	-C4	C4 */\
	"pmaddwd %%mm5, %%mm0			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	"movq 32(%2), %%mm5			\n\t" /* C6	C2	C6	C2 */\
	"pmaddwd %%mm1, %%mm5			\n\t" /* C6R6+C2R2	C6r6+C2r2 */\
	"movq 40(%2), %%mm6			\n\t" /* -C2	C6	-C2	C6 */\
	"pmaddwd %%mm6, %%mm1			\n\t" /* -C2R6+C6R2	-C2r6+C6r2 */\
	"movq 48(%2), %%mm7			\n\t" /* C3	C1	C3	C1 */\
	"pmaddwd %%mm2, %%mm7			\n\t" /* C3R3+C1R1	C3r3+C1r1 */\
	#rounder ", %%mm4			\n\t"\
	"movq %%mm4, %%mm6			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"paddd %%mm5, %%mm4			\n\t" /* A0		a0 */\
	"psubd %%mm5, %%mm6			\n\t" /* A3		a3 */\
	"movq 56(%2), %%mm5			\n\t" /* C7	C5	C7	C5 */\
	"pmaddwd %%mm3, %%mm5			\n\t" /* C7R7+C5R5	C7r7+C5r5 */\
	#rounder ", %%mm0			\n\t"\
	"paddd %%mm0, %%mm1			\n\t" /* A1		a1 */\
	"paddd %%mm0, %%mm0			\n\t" \
	"psubd %%mm1, %%mm0			\n\t" /* A2		a2 */\
	"pmaddwd 64(%2), %%mm2			\n\t" /* -C7R3+C3R1	-C7r3+C3r1 */\
	"paddd %%mm5, %%mm7			\n\t" /* B0		b0 */\
	"movq 72(%2), %%mm5			\n\t" /* -C5	-C1	-C5	-C1 */\
	"pmaddwd %%mm3, %%mm5			\n\t" /* -C5R7-C1R5	-C5r7-C1r5 */\
	"paddd %%mm4, %%mm7			\n\t" /* A0+B0		a0+b0 */\
	"paddd %%mm4, %%mm4			\n\t" /* 2A0		2a0 */\
	"psubd %%mm7, %%mm4			\n\t" /* A0-B0		a0-b0 */\
	"paddd %%mm2, %%mm5			\n\t" /* B1		b1 */\
	"psrad $" #shift ", %%mm7		\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"movq %%mm1, %%mm2			\n\t" /* A1		a1 */\
	"paddd %%mm5, %%mm1			\n\t" /* A1+B1		a1+b1 */\
	"psubd %%mm5, %%mm2			\n\t" /* A1-B1		a1-b1 */\
	"psrad $" #shift ", %%mm1		\n\t"\
	"psrad $" #shift ", %%mm2		\n\t"\
	"packssdw %%mm1, %%mm7			\n\t" /* A1+B1	a1+b1	A0+B0	a0+b0 */\
	"packssdw %%mm4, %%mm2			\n\t" /* A0-B0	a0-b0	A1-B1	a1-b1 */\
	"movq %%mm7, " #dst "			\n\t"\
	"movq " #src1 ", %%mm1			\n\t" /* R3	R1	r3	r1 */\
	"movq 80(%2), %%mm4			\n\t" /* -C1	C5	-C1 	C5 */\
	"movq %%mm2, 24+" #dst "		\n\t"\
	"pmaddwd %%mm1, %%mm4			\n\t" /* -C1R3+C5R1	-C1r3+C5r1 */\
	"movq 88(%2), %%mm7			\n\t" /* C3	C7	C3 	C7 */\
	"pmaddwd 96(%2), %%mm1			\n\t" /* -C5R3+C7R1	-C5r3+C7r1 */\
	"pmaddwd %%mm3, %%mm7			\n\t" /* C3R7+C7R5	C3r7+C7r5 */\
	"movq %%mm0, %%mm2			\n\t" /* A2		a2 */\
	"pmaddwd 104(%2), %%mm3			\n\t" /* -C1R7+C3R5	-C1r7+C3r5 */\
	"paddd %%mm7, %%mm4			\n\t" /* B2		b2 */\
	"paddd %%mm4, %%mm2			\n\t" /* A2+B2		a2+b2 */\
	"psubd %%mm4, %%mm0			\n\t" /* a2-B2		a2-b2 */\
	"psrad $" #shift ", %%mm2		\n\t"\
	"psrad $" #shift ", %%mm0		\n\t"\
	"movq %%mm6, %%mm4			\n\t" /* A3		a3 */\
	"paddd %%mm1, %%mm3			\n\t" /* B3		b3 */\
	"paddd %%mm3, %%mm6			\n\t" /* A3+B3		a3+b3 */\
	"psubd %%mm3, %%mm4			\n\t" /* a3-B3		a3-b3 */\
	"psrad $" #shift ", %%mm6		\n\t"\
	"packssdw %%mm6, %%mm2			\n\t" /* A3+B3	a3+b3	A2+B2	a2+b2 */\
	"movq %%mm2, 8+" #dst "			\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"packssdw %%mm0, %%mm4			\n\t" /* A2-B2	a2-b2	A3-B3	a3-b3 */\
	"movq %%mm4, 16+" #dst "		\n\t"\

#define ROW_IDCT(src0, src4, src1, src5, dst, rounder, shift) \
	"movq " #src0 ", %%mm0			\n\t" /* R4	R0	r4	r0 */\
	"movq " #src4 ", %%mm1			\n\t" /* R6	R2	r6	r2 */\
	"movq " #src1 ", %%mm2			\n\t" /* R3	R1	r3	r1 */\
	"movq " #src5 ", %%mm3			\n\t" /* R7	R5	r7	r5 */\
	"movq 16(%2), %%mm4			\n\t" /* C4	C4	C4	C4 */\
	"pmaddwd %%mm0, %%mm4			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"movq 24(%2), %%mm5			\n\t" /* -C4	C4	-C4	C4 */\
	"pmaddwd %%mm5, %%mm0			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	"movq 32(%2), %%mm5			\n\t" /* C6	C2	C6	C2 */\
	"pmaddwd %%mm1, %%mm5			\n\t" /* C6R6+C2R2	C6r6+C2r2 */\
	"movq 40(%2), %%mm6			\n\t" /* -C2	C6	-C2	C6 */\
	"pmaddwd %%mm6, %%mm1			\n\t" /* -C2R6+C6R2	-C2r6+C6r2 */\
	"movq 48(%2), %%mm7			\n\t" /* C3	C1	C3	C1 */\
	"pmaddwd %%mm2, %%mm7			\n\t" /* C3R3+C1R1	C3r3+C1r1 */\
	#rounder ", %%mm4			\n\t"\
	"movq %%mm4, %%mm6			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"paddd %%mm5, %%mm4			\n\t" /* A0		a0 */\
	"psubd %%mm5, %%mm6			\n\t" /* A3		a3 */\
	"movq 56(%2), %%mm5			\n\t" /* C7	C5	C7	C5 */\
	"pmaddwd %%mm3, %%mm5			\n\t" /* C7R7+C5R5	C7r7+C5r5 */\
	#rounder ", %%mm0			\n\t"\
	"paddd %%mm0, %%mm1			\n\t" /* A1		a1 */\
	"paddd %%mm0, %%mm0			\n\t" \
	"psubd %%mm1, %%mm0			\n\t" /* A2		a2 */\
	"pmaddwd 64(%2), %%mm2			\n\t" /* -C7R3+C3R1	-C7r3+C3r1 */\
	"paddd %%mm5, %%mm7			\n\t" /* B0		b0 */\
	"movq 72(%2), %%mm5			\n\t" /* -C5	-C1	-C5	-C1 */\
	"pmaddwd %%mm3, %%mm5			\n\t" /* -C5R7-C1R5	-C5r7-C1r5 */\
	"paddd %%mm4, %%mm7			\n\t" /* A0+B0		a0+b0 */\
	"paddd %%mm4, %%mm4			\n\t" /* 2A0		2a0 */\
	"psubd %%mm7, %%mm4			\n\t" /* A0-B0		a0-b0 */\
	"paddd %%mm2, %%mm5			\n\t" /* B1		b1 */\
	"psrad $" #shift ", %%mm7		\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"movq %%mm1, %%mm2			\n\t" /* A1		a1 */\
	"paddd %%mm5, %%mm1			\n\t" /* A1+B1		a1+b1 */\
	"psubd %%mm5, %%mm2			\n\t" /* A1-B1		a1-b1 */\
	"psrad $" #shift ", %%mm1		\n\t"\
	"psrad $" #shift ", %%mm2		\n\t"\
	"packssdw %%mm1, %%mm7			\n\t" /* A1+B1	a1+b1	A0+B0	a0+b0 */\
	"packssdw %%mm4, %%mm2			\n\t" /* A0-B0	a0-b0	A1-B1	a1-b1 */\
	"movq %%mm7, " #dst "			\n\t"\
	"movq " #src1 ", %%mm1			\n\t" /* R3	R1	r3	r1 */\
	"movq 80(%2), %%mm4			\n\t" /* -C1	C5	-C1 	C5 */\
	"movq %%mm2, 24+" #dst "		\n\t"\
	"pmaddwd %%mm1, %%mm4			\n\t" /* -C1R3+C5R1	-C1r3+C5r1 */\
	"movq 88(%2), %%mm7			\n\t" /* C3	C7	C3 	C7 */\
	"pmaddwd 96(%2), %%mm1			\n\t" /* -C5R3+C7R1	-C5r3+C7r1 */\
	"pmaddwd %%mm3, %%mm7			\n\t" /* C3R7+C7R5	C3r7+C7r5 */\
	"movq %%mm0, %%mm2			\n\t" /* A2		a2 */\
	"pmaddwd 104(%2), %%mm3			\n\t" /* -C1R7+C3R5	-C1r7+C3r5 */\
	"paddd %%mm7, %%mm4			\n\t" /* B2		b2 */\
	"paddd %%mm4, %%mm2			\n\t" /* A2+B2		a2+b2 */\
	"psubd %%mm4, %%mm0			\n\t" /* a2-B2		a2-b2 */\
	"psrad $" #shift ", %%mm2		\n\t"\
	"psrad $" #shift ", %%mm0		\n\t"\
	"movq %%mm6, %%mm4			\n\t" /* A3		a3 */\
	"paddd %%mm1, %%mm3			\n\t" /* B3		b3 */\
	"paddd %%mm3, %%mm6			\n\t" /* A3+B3		a3+b3 */\
	"psubd %%mm3, %%mm4			\n\t" /* a3-B3		a3-b3 */\
	"psrad $" #shift ", %%mm6		\n\t"\
	"packssdw %%mm6, %%mm2			\n\t" /* A3+B3	a3+b3	A2+B2	a2+b2 */\
	"movq %%mm2, 8+" #dst "			\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"packssdw %%mm0, %%mm4			\n\t" /* A2-B2	a2-b2	A3-B3	a3-b3 */\
	"movq %%mm4, 16+" #dst "		\n\t"\

//IDCT(         src0,   src4,   src1,   src5,    dst,   rounder, shift)
DC_COND_IDCT(  0(%0),  8(%0), 16(%0), 24(%0),  0(%1),paddd 8(%2), 11)
Z_COND_IDCT(  32(%0), 40(%0), 48(%0), 56(%0), 32(%1),paddd (%2), 11, 4f)
Z_COND_IDCT(  64(%0), 72(%0), 80(%0), 88(%0), 64(%1),paddd (%2), 11, 2f)
Z_COND_IDCT(  96(%0),104(%0),112(%0),120(%0), 96(%1),paddd (%2), 11, 1f)

#undef IDCT
#define IDCT(src0, src4, src1, src5, dst, rounder, shift) \
	"movq " #src0 ", %%mm0			\n\t" /* R4	R0	r4	r0 */\
	"movq " #src4 ", %%mm1			\n\t" /* R6	R2	r6	r2 */\
	"movq " #src1 ", %%mm2			\n\t" /* R3	R1	r3	r1 */\
	"movq " #src5 ", %%mm3			\n\t" /* R7	R5	r7	r5 */\
	"movq 16(%2), %%mm4			\n\t" /* C4	C4	C4	C4 */\
	"pmaddwd %%mm0, %%mm4			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"movq 24(%2), %%mm5			\n\t" /* -C4	C4	-C4	C4 */\
	"pmaddwd %%mm5, %%mm0			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	"movq 32(%2), %%mm5			\n\t" /* C6	C2	C6	C2 */\
	"pmaddwd %%mm1, %%mm5			\n\t" /* C6R6+C2R2	C6r6+C2r2 */\
	"movq 40(%2), %%mm6			\n\t" /* -C2	C6	-C2	C6 */\
	"pmaddwd %%mm6, %%mm1			\n\t" /* -C2R6+C6R2	-C2r6+C6r2 */\
	#rounder ", %%mm4			\n\t"\
	"movq %%mm4, %%mm6			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"movq 48(%2), %%mm7			\n\t" /* C3	C1	C3	C1 */\
	#rounder ", %%mm0			\n\t"\
	"pmaddwd %%mm2, %%mm7			\n\t" /* C3R3+C1R1	C3r3+C1r1 */\
	"paddd %%mm5, %%mm4			\n\t" /* A0		a0 */\
	"psubd %%mm5, %%mm6			\n\t" /* A3		a3 */\
	"movq %%mm0, %%mm5			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	"paddd %%mm1, %%mm0			\n\t" /* A1		a1 */\
	"psubd %%mm1, %%mm5			\n\t" /* A2		a2 */\
	"movq 56(%2), %%mm1			\n\t" /* C7	C5	C7	C5 */\
	"pmaddwd %%mm3, %%mm1			\n\t" /* C7R7+C5R5	C7r7+C5r5 */\
	"pmaddwd 64(%2), %%mm2			\n\t" /* -C7R3+C3R1	-C7r3+C3r1 */\
	"paddd %%mm1, %%mm7			\n\t" /* B0		b0 */\
	"movq 72(%2), %%mm1			\n\t" /* -C5	-C1	-C5	-C1 */\
	"pmaddwd %%mm3, %%mm1			\n\t" /* -C5R7-C1R5	-C5r7-C1r5 */\
	"paddd %%mm4, %%mm7			\n\t" /* A0+B0		a0+b0 */\
	"paddd %%mm4, %%mm4			\n\t" /* 2A0		2a0 */\
	"psubd %%mm7, %%mm4			\n\t" /* A0-B0		a0-b0 */\
	"paddd %%mm2, %%mm1			\n\t" /* B1		b1 */\
	"psrad $" #shift ", %%mm7		\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"movq %%mm0, %%mm2			\n\t" /* A1		a1 */\
	"paddd %%mm1, %%mm0			\n\t" /* A1+B1		a1+b1 */\
	"psubd %%mm1, %%mm2			\n\t" /* A1-B1		a1-b1 */\
	"psrad $" #shift ", %%mm0		\n\t"\
	"psrad $" #shift ", %%mm2		\n\t"\
	"packssdw %%mm7, %%mm7			\n\t" /* A0+B0	a0+b0 */\
	"movd %%mm7, " #dst "			\n\t"\
	"packssdw %%mm0, %%mm0			\n\t" /* A1+B1	a1+b1 */\
	"movd %%mm0, 16+" #dst "		\n\t"\
	"packssdw %%mm2, %%mm2			\n\t" /* A1-B1	a1-b1 */\
	"movd %%mm2, 96+" #dst "		\n\t"\
	"packssdw %%mm4, %%mm4			\n\t" /* A0-B0	a0-b0 */\
	"movd %%mm4, 112+" #dst "		\n\t"\
	"movq " #src1 ", %%mm0			\n\t" /* R3	R1	r3	r1 */\
	"movq 80(%2), %%mm4			\n\t" /* -C1	C5	-C1 	C5 */\
	"pmaddwd %%mm0, %%mm4			\n\t" /* -C1R3+C5R1	-C1r3+C5r1 */\
	"movq 88(%2), %%mm7			\n\t" /* C3	C7	C3 	C7 */\
	"pmaddwd 96(%2), %%mm0			\n\t" /* -C5R3+C7R1	-C5r3+C7r1 */\
	"pmaddwd %%mm3, %%mm7			\n\t" /* C3R7+C7R5	C3r7+C7r5 */\
	"movq %%mm5, %%mm2			\n\t" /* A2		a2 */\
	"pmaddwd 104(%2), %%mm3			\n\t" /* -C1R7+C3R5	-C1r7+C3r5 */\
	"paddd %%mm7, %%mm4			\n\t" /* B2		b2 */\
	"paddd %%mm4, %%mm2			\n\t" /* A2+B2		a2+b2 */\
	"psubd %%mm4, %%mm5			\n\t" /* a2-B2		a2-b2 */\
	"psrad $" #shift ", %%mm2		\n\t"\
	"psrad $" #shift ", %%mm5		\n\t"\
	"movq %%mm6, %%mm4			\n\t" /* A3		a3 */\
	"paddd %%mm0, %%mm3			\n\t" /* B3		b3 */\
	"paddd %%mm3, %%mm6			\n\t" /* A3+B3		a3+b3 */\
	"psubd %%mm3, %%mm4			\n\t" /* a3-B3		a3-b3 */\
	"psrad $" #shift ", %%mm6		\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"packssdw %%mm2, %%mm2			\n\t" /* A2+B2	a2+b2 */\
	"packssdw %%mm6, %%mm6			\n\t" /* A3+B3	a3+b3 */\
	"movd %%mm2, 32+" #dst "		\n\t"\
	"packssdw %%mm4, %%mm4			\n\t" /* A3-B3	a3-b3 */\
	"packssdw %%mm5, %%mm5			\n\t" /* A2-B2	a2-b2 */\
	"movd %%mm6, 48+" #dst "		\n\t"\
	"movd %%mm4, 64+" #dst "		\n\t"\
	"movd %%mm5, 80+" #dst "		\n\t"


//IDCT(  src0,   src4,   src1,    src5,    dst, rounder, shift)
IDCT(    (%1), 64(%1), 32(%1),  96(%1),  0(%0),/nop, 20)
IDCT(   8(%1), 72(%1), 40(%1), 104(%1),  4(%0),/nop, 20)
IDCT(  16(%1), 80(%1), 48(%1), 112(%1),  8(%0),/nop, 20)
IDCT(  24(%1), 88(%1), 56(%1), 120(%1), 12(%0),/nop, 20)
	"jmp 9f					\n\t"

	"#.balign 16				\n\t"\
	"4:					\n\t"
Z_COND_IDCT(  64(%0), 72(%0), 80(%0), 88(%0), 64(%1),paddd (%2), 11, 6f)
Z_COND_IDCT(  96(%0),104(%0),112(%0),120(%0), 96(%1),paddd (%2), 11, 5f)

#undef IDCT
#define IDCT(src0, src4, src1, src5, dst, rounder, shift) \
	"movq " #src0 ", %%mm0			\n\t" /* R4	R0	r4	r0 */\
	"movq " #src4 ", %%mm1			\n\t" /* R6	R2	r6	r2 */\
	"movq " #src5 ", %%mm3			\n\t" /* R7	R5	r7	r5 */\
	"movq 16(%2), %%mm4			\n\t" /* C4	C4	C4	C4 */\
	"pmaddwd %%mm0, %%mm4			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"movq 24(%2), %%mm5			\n\t" /* -C4	C4	-C4	C4 */\
	"pmaddwd %%mm5, %%mm0			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	"movq 32(%2), %%mm5			\n\t" /* C6	C2	C6	C2 */\
	"pmaddwd %%mm1, %%mm5			\n\t" /* C6R6+C2R2	C6r6+C2r2 */\
	"movq 40(%2), %%mm6			\n\t" /* -C2	C6	-C2	C6 */\
	"pmaddwd %%mm6, %%mm1			\n\t" /* -C2R6+C6R2	-C2r6+C6r2 */\
	#rounder ", %%mm4			\n\t"\
	"movq %%mm4, %%mm6			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	#rounder ", %%mm0			\n\t"\
	"paddd %%mm5, %%mm4			\n\t" /* A0		a0 */\
	"psubd %%mm5, %%mm6			\n\t" /* A3		a3 */\
	"movq %%mm0, %%mm5			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	"paddd %%mm1, %%mm0			\n\t" /* A1		a1 */\
	"psubd %%mm1, %%mm5			\n\t" /* A2		a2 */\
	"movq 56(%2), %%mm1			\n\t" /* C7	C5	C7	C5 */\
	"pmaddwd %%mm3, %%mm1			\n\t" /* C7R7+C5R5	C7r7+C5r5 */\
	"movq 72(%2), %%mm7			\n\t" /* -C5	-C1	-C5	-C1 */\
	"pmaddwd %%mm3, %%mm7			\n\t" /* -C5R7-C1R5	-C5r7-C1r5 */\
	"paddd %%mm4, %%mm1			\n\t" /* A0+B0		a0+b0 */\
	"paddd %%mm4, %%mm4			\n\t" /* 2A0		2a0 */\
	"psubd %%mm1, %%mm4			\n\t" /* A0-B0		a0-b0 */\
	"psrad $" #shift ", %%mm1		\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"movq %%mm0, %%mm2			\n\t" /* A1		a1 */\
	"paddd %%mm7, %%mm0			\n\t" /* A1+B1		a1+b1 */\
	"psubd %%mm7, %%mm2			\n\t" /* A1-B1		a1-b1 */\
	"psrad $" #shift ", %%mm0		\n\t"\
	"psrad $" #shift ", %%mm2		\n\t"\
	"packssdw %%mm1, %%mm1			\n\t" /* A0+B0	a0+b0 */\
	"movd %%mm1, " #dst "			\n\t"\
	"packssdw %%mm0, %%mm0			\n\t" /* A1+B1	a1+b1 */\
	"movd %%mm0, 16+" #dst "		\n\t"\
	"packssdw %%mm2, %%mm2			\n\t" /* A1-B1	a1-b1 */\
	"movd %%mm2, 96+" #dst "		\n\t"\
	"packssdw %%mm4, %%mm4			\n\t" /* A0-B0	a0-b0 */\
	"movd %%mm4, 112+" #dst "		\n\t"\
	"movq 88(%2), %%mm1			\n\t" /* C3	C7	C3 	C7 */\
	"pmaddwd %%mm3, %%mm1			\n\t" /* C3R7+C7R5	C3r7+C7r5 */\
	"movq %%mm5, %%mm2			\n\t" /* A2		a2 */\
	"pmaddwd 104(%2), %%mm3			\n\t" /* -C1R7+C3R5	-C1r7+C3r5 */\
	"paddd %%mm1, %%mm2			\n\t" /* A2+B2		a2+b2 */\
	"psubd %%mm1, %%mm5			\n\t" /* a2-B2		a2-b2 */\
	"psrad $" #shift ", %%mm2		\n\t"\
	"psrad $" #shift ", %%mm5		\n\t"\
	"movq %%mm6, %%mm1			\n\t" /* A3		a3 */\
	"paddd %%mm3, %%mm6			\n\t" /* A3+B3		a3+b3 */\
	"psubd %%mm3, %%mm1			\n\t" /* a3-B3		a3-b3 */\
	"psrad $" #shift ", %%mm6		\n\t"\
	"psrad $" #shift ", %%mm1		\n\t"\
	"packssdw %%mm2, %%mm2			\n\t" /* A2+B2	a2+b2 */\
	"packssdw %%mm6, %%mm6			\n\t" /* A3+B3	a3+b3 */\
	"movd %%mm2, 32+" #dst "		\n\t"\
	"packssdw %%mm1, %%mm1			\n\t" /* A3-B3	a3-b3 */\
	"packssdw %%mm5, %%mm5			\n\t" /* A2-B2	a2-b2 */\
	"movd %%mm6, 48+" #dst "		\n\t"\
	"movd %%mm1, 64+" #dst "		\n\t"\
	"movd %%mm5, 80+" #dst "		\n\t"	

//IDCT(  src0,   src4,   src1,    src5,    dst, rounder, shift)
IDCT(    (%1), 64(%1), 32(%1),  96(%1),  0(%0),/nop, 20)
IDCT(   8(%1), 72(%1), 40(%1), 104(%1),  4(%0),/nop, 20)
IDCT(  16(%1), 80(%1), 48(%1), 112(%1),  8(%0),/nop, 20)
IDCT(  24(%1), 88(%1), 56(%1), 120(%1), 12(%0),/nop, 20)
	"jmp 9f					\n\t"

	"#.balign 16				\n\t"\
	"6:					\n\t"
Z_COND_IDCT(  96(%0),104(%0),112(%0),120(%0), 96(%1),paddd (%2), 11, 7f)

#undef IDCT
#define IDCT(src0, src4, src1, src5, dst, rounder, shift) \
	"movq " #src0 ", %%mm0			\n\t" /* R4	R0	r4	r0 */\
	"movq " #src5 ", %%mm3			\n\t" /* R7	R5	r7	r5 */\
	"movq 16(%2), %%mm4			\n\t" /* C4	C4	C4	C4 */\
	"pmaddwd %%mm0, %%mm4			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"movq 24(%2), %%mm5			\n\t" /* -C4	C4	-C4	C4 */\
	"pmaddwd %%mm5, %%mm0			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	#rounder ", %%mm4			\n\t"\
	"movq %%mm4, %%mm6			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	#rounder ", %%mm0			\n\t"\
	"movq %%mm0, %%mm5			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	"movq 56(%2), %%mm1			\n\t" /* C7	C5	C7	C5 */\
	"pmaddwd %%mm3, %%mm1			\n\t" /* C7R7+C5R5	C7r7+C5r5 */\
	"movq 72(%2), %%mm7			\n\t" /* -C5	-C1	-C5	-C1 */\
	"pmaddwd %%mm3, %%mm7			\n\t" /* -C5R7-C1R5	-C5r7-C1r5 */\
	"paddd %%mm4, %%mm1			\n\t" /* A0+B0		a0+b0 */\
	"paddd %%mm4, %%mm4			\n\t" /* 2A0		2a0 */\
	"psubd %%mm1, %%mm4			\n\t" /* A0-B0		a0-b0 */\
	"psrad $" #shift ", %%mm1		\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"movq %%mm0, %%mm2			\n\t" /* A1		a1 */\
	"paddd %%mm7, %%mm0			\n\t" /* A1+B1		a1+b1 */\
	"psubd %%mm7, %%mm2			\n\t" /* A1-B1		a1-b1 */\
	"psrad $" #shift ", %%mm0		\n\t"\
	"psrad $" #shift ", %%mm2		\n\t"\
	"packssdw %%mm1, %%mm1			\n\t" /* A0+B0	a0+b0 */\
	"movd %%mm1, " #dst "			\n\t"\
	"packssdw %%mm0, %%mm0			\n\t" /* A1+B1	a1+b1 */\
	"movd %%mm0, 16+" #dst "		\n\t"\
	"packssdw %%mm2, %%mm2			\n\t" /* A1-B1	a1-b1 */\
	"movd %%mm2, 96+" #dst "		\n\t"\
	"packssdw %%mm4, %%mm4			\n\t" /* A0-B0	a0-b0 */\
	"movd %%mm4, 112+" #dst "		\n\t"\
	"movq 88(%2), %%mm1			\n\t" /* C3	C7	C3 	C7 */\
	"pmaddwd %%mm3, %%mm1			\n\t" /* C3R7+C7R5	C3r7+C7r5 */\
	"movq %%mm5, %%mm2			\n\t" /* A2		a2 */\
	"pmaddwd 104(%2), %%mm3			\n\t" /* -C1R7+C3R5	-C1r7+C3r5 */\
	"paddd %%mm1, %%mm2			\n\t" /* A2+B2		a2+b2 */\
	"psubd %%mm1, %%mm5			\n\t" /* a2-B2		a2-b2 */\
	"psrad $" #shift ", %%mm2		\n\t"\
	"psrad $" #shift ", %%mm5		\n\t"\
	"movq %%mm6, %%mm1			\n\t" /* A3		a3 */\
	"paddd %%mm3, %%mm6			\n\t" /* A3+B3		a3+b3 */\
	"psubd %%mm3, %%mm1			\n\t" /* a3-B3		a3-b3 */\
	"psrad $" #shift ", %%mm6		\n\t"\
	"psrad $" #shift ", %%mm1		\n\t"\
	"packssdw %%mm2, %%mm2			\n\t" /* A2+B2	a2+b2 */\
	"packssdw %%mm6, %%mm6			\n\t" /* A3+B3	a3+b3 */\
	"movd %%mm2, 32+" #dst "		\n\t"\
	"packssdw %%mm1, %%mm1			\n\t" /* A3-B3	a3-b3 */\
	"packssdw %%mm5, %%mm5			\n\t" /* A2-B2	a2-b2 */\
	"movd %%mm6, 48+" #dst "		\n\t"\
	"movd %%mm1, 64+" #dst "		\n\t"\
	"movd %%mm5, 80+" #dst "		\n\t"	


//IDCT(  src0,   src4,   src1,    src5,    dst, rounder, shift)
IDCT(    (%1), 64(%1), 32(%1),  96(%1),  0(%0),/nop, 20)
IDCT(   8(%1), 72(%1), 40(%1), 104(%1),  4(%0),/nop, 20)
IDCT(  16(%1), 80(%1), 48(%1), 112(%1),  8(%0),/nop, 20)
IDCT(  24(%1), 88(%1), 56(%1), 120(%1), 12(%0),/nop, 20)
	"jmp 9f					\n\t"

	"#.balign 16				\n\t"\
	"2:					\n\t"
Z_COND_IDCT(  96(%0),104(%0),112(%0),120(%0), 96(%1),paddd (%2), 11, 3f)

#undef IDCT
#define IDCT(src0, src4, src1, src5, dst, rounder, shift) \
	"movq " #src0 ", %%mm0			\n\t" /* R4	R0	r4	r0 */\
	"movq " #src1 ", %%mm2			\n\t" /* R3	R1	r3	r1 */\
	"movq " #src5 ", %%mm3			\n\t" /* R7	R5	r7	r5 */\
	"movq 16(%2), %%mm4			\n\t" /* C4	C4	C4	C4 */\
	"pmaddwd %%mm0, %%mm4			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"movq 24(%2), %%mm5			\n\t" /* -C4	C4	-C4	C4 */\
	"pmaddwd %%mm5, %%mm0			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	#rounder ", %%mm4			\n\t"\
	"movq %%mm4, %%mm6			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"movq 48(%2), %%mm7			\n\t" /* C3	C1	C3	C1 */\
	#rounder ", %%mm0			\n\t"\
	"pmaddwd %%mm2, %%mm7			\n\t" /* C3R3+C1R1	C3r3+C1r1 */\
	"movq %%mm0, %%mm5			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	"movq 56(%2), %%mm1			\n\t" /* C7	C5	C7	C5 */\
	"pmaddwd %%mm3, %%mm1			\n\t" /* C7R7+C5R5	C7r7+C5r5 */\
	"pmaddwd 64(%2), %%mm2			\n\t" /* -C7R3+C3R1	-C7r3+C3r1 */\
	"paddd %%mm1, %%mm7			\n\t" /* B0		b0 */\
	"movq 72(%2), %%mm1			\n\t" /* -C5	-C1	-C5	-C1 */\
	"pmaddwd %%mm3, %%mm1			\n\t" /* -C5R7-C1R5	-C5r7-C1r5 */\
	"paddd %%mm4, %%mm7			\n\t" /* A0+B0		a0+b0 */\
	"paddd %%mm4, %%mm4			\n\t" /* 2A0		2a0 */\
	"psubd %%mm7, %%mm4			\n\t" /* A0-B0		a0-b0 */\
	"paddd %%mm2, %%mm1			\n\t" /* B1		b1 */\
	"psrad $" #shift ", %%mm7		\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"movq %%mm0, %%mm2			\n\t" /* A1		a1 */\
	"paddd %%mm1, %%mm0			\n\t" /* A1+B1		a1+b1 */\
	"psubd %%mm1, %%mm2			\n\t" /* A1-B1		a1-b1 */\
	"psrad $" #shift ", %%mm0		\n\t"\
	"psrad $" #shift ", %%mm2		\n\t"\
	"packssdw %%mm7, %%mm7			\n\t" /* A0+B0	a0+b0 */\
	"movd %%mm7, " #dst "			\n\t"\
	"packssdw %%mm0, %%mm0			\n\t" /* A1+B1	a1+b1 */\
	"movd %%mm0, 16+" #dst "		\n\t"\
	"packssdw %%mm2, %%mm2			\n\t" /* A1-B1	a1-b1 */\
	"movd %%mm2, 96+" #dst "		\n\t"\
	"packssdw %%mm4, %%mm4			\n\t" /* A0-B0	a0-b0 */\
	"movd %%mm4, 112+" #dst "		\n\t"\
	"movq " #src1 ", %%mm0			\n\t" /* R3	R1	r3	r1 */\
	"movq 80(%2), %%mm4			\n\t" /* -C1	C5	-C1 	C5 */\
	"pmaddwd %%mm0, %%mm4			\n\t" /* -C1R3+C5R1	-C1r3+C5r1 */\
	"movq 88(%2), %%mm7			\n\t" /* C3	C7	C3 	C7 */\
	"pmaddwd 96(%2), %%mm0			\n\t" /* -C5R3+C7R1	-C5r3+C7r1 */\
	"pmaddwd %%mm3, %%mm7			\n\t" /* C3R7+C7R5	C3r7+C7r5 */\
	"movq %%mm5, %%mm2			\n\t" /* A2		a2 */\
	"pmaddwd 104(%2), %%mm3			\n\t" /* -C1R7+C3R5	-C1r7+C3r5 */\
	"paddd %%mm7, %%mm4			\n\t" /* B2		b2 */\
	"paddd %%mm4, %%mm2			\n\t" /* A2+B2		a2+b2 */\
	"psubd %%mm4, %%mm5			\n\t" /* a2-B2		a2-b2 */\
	"psrad $" #shift ", %%mm2		\n\t"\
	"psrad $" #shift ", %%mm5		\n\t"\
	"movq %%mm6, %%mm4			\n\t" /* A3		a3 */\
	"paddd %%mm0, %%mm3			\n\t" /* B3		b3 */\
	"paddd %%mm3, %%mm6			\n\t" /* A3+B3		a3+b3 */\
	"psubd %%mm3, %%mm4			\n\t" /* a3-B3		a3-b3 */\
	"psrad $" #shift ", %%mm6		\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"packssdw %%mm2, %%mm2			\n\t" /* A2+B2	a2+b2 */\
	"packssdw %%mm6, %%mm6			\n\t" /* A3+B3	a3+b3 */\
	"movd %%mm2, 32+" #dst "		\n\t"\
	"packssdw %%mm4, %%mm4			\n\t" /* A3-B3	a3-b3 */\
	"packssdw %%mm5, %%mm5			\n\t" /* A2-B2	a2-b2 */\
	"movd %%mm6, 48+" #dst "		\n\t"\
	"movd %%mm4, 64+" #dst "		\n\t"\
	"movd %%mm5, 80+" #dst "		\n\t"

//IDCT(  src0,   src4,   src1,    src5,    dst, rounder, shift)
IDCT(    (%1), 64(%1), 32(%1),  96(%1),  0(%0),/nop, 20)
IDCT(   8(%1), 72(%1), 40(%1), 104(%1),  4(%0),/nop, 20)
IDCT(  16(%1), 80(%1), 48(%1), 112(%1),  8(%0),/nop, 20)
IDCT(  24(%1), 88(%1), 56(%1), 120(%1), 12(%0),/nop, 20)
	"jmp 9f					\n\t"

	"#.balign 16				\n\t"\
	"3:					\n\t"
#undef IDCT
#define IDCT(src0, src4, src1, src5, dst, rounder, shift) \
	"movq " #src0 ", %%mm0			\n\t" /* R4	R0	r4	r0 */\
	"movq " #src1 ", %%mm2			\n\t" /* R3	R1	r3	r1 */\
	"movq 16(%2), %%mm4			\n\t" /* C4	C4	C4	C4 */\
	"pmaddwd %%mm0, %%mm4			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"movq 24(%2), %%mm5			\n\t" /* -C4	C4	-C4	C4 */\
	"pmaddwd %%mm5, %%mm0			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	#rounder ", %%mm4			\n\t"\
	"movq %%mm4, %%mm6			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"movq 48(%2), %%mm7			\n\t" /* C3	C1	C3	C1 */\
	#rounder ", %%mm0			\n\t"\
	"pmaddwd %%mm2, %%mm7			\n\t" /* C3R3+C1R1	C3r3+C1r1 */\
	"movq %%mm0, %%mm5			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	"movq 64(%2), %%mm3			\n\t"\
	"pmaddwd %%mm2, %%mm3			\n\t" /* -C7R3+C3R1	-C7r3+C3r1 */\
	"paddd %%mm4, %%mm7			\n\t" /* A0+B0		a0+b0 */\
	"paddd %%mm4, %%mm4			\n\t" /* 2A0		2a0 */\
	"psubd %%mm7, %%mm4			\n\t" /* A0-B0		a0-b0 */\
	"psrad $" #shift ", %%mm7		\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"movq %%mm0, %%mm1			\n\t" /* A1		a1 */\
	"paddd %%mm3, %%mm0			\n\t" /* A1+B1		a1+b1 */\
	"psubd %%mm3, %%mm1			\n\t" /* A1-B1		a1-b1 */\
	"psrad $" #shift ", %%mm0		\n\t"\
	"psrad $" #shift ", %%mm1		\n\t"\
	"packssdw %%mm7, %%mm7			\n\t" /* A0+B0	a0+b0 */\
	"movd %%mm7, " #dst "			\n\t"\
	"packssdw %%mm0, %%mm0			\n\t" /* A1+B1	a1+b1 */\
	"movd %%mm0, 16+" #dst "		\n\t"\
	"packssdw %%mm1, %%mm1			\n\t" /* A1-B1	a1-b1 */\
	"movd %%mm1, 96+" #dst "		\n\t"\
	"packssdw %%mm4, %%mm4			\n\t" /* A0-B0	a0-b0 */\
	"movd %%mm4, 112+" #dst "		\n\t"\
	"movq 80(%2), %%mm4			\n\t" /* -C1	C5	-C1 	C5 */\
	"pmaddwd %%mm2, %%mm4			\n\t" /* -C1R3+C5R1	-C1r3+C5r1 */\
	"pmaddwd 96(%2), %%mm2			\n\t" /* -C5R3+C7R1	-C5r3+C7r1 */\
	"movq %%mm5, %%mm1			\n\t" /* A2		a2 */\
	"paddd %%mm4, %%mm1			\n\t" /* A2+B2		a2+b2 */\
	"psubd %%mm4, %%mm5			\n\t" /* a2-B2		a2-b2 */\
	"psrad $" #shift ", %%mm1		\n\t"\
	"psrad $" #shift ", %%mm5		\n\t"\
	"movq %%mm6, %%mm4			\n\t" /* A3		a3 */\
	"paddd %%mm2, %%mm6			\n\t" /* A3+B3		a3+b3 */\
	"psubd %%mm2, %%mm4			\n\t" /* a3-B3		a3-b3 */\
	"psrad $" #shift ", %%mm6		\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"packssdw %%mm1, %%mm1			\n\t" /* A2+B2	a2+b2 */\
	"packssdw %%mm6, %%mm6			\n\t" /* A3+B3	a3+b3 */\
	"movd %%mm1, 32+" #dst "		\n\t"\
	"packssdw %%mm4, %%mm4			\n\t" /* A3-B3	a3-b3 */\
	"packssdw %%mm5, %%mm5			\n\t" /* A2-B2	a2-b2 */\
	"movd %%mm6, 48+" #dst "		\n\t"\
	"movd %%mm4, 64+" #dst "		\n\t"\
	"movd %%mm5, 80+" #dst "		\n\t"


//IDCT(  src0,   src4,   src1,    src5,    dst, rounder, shift)
IDCT(    (%1), 64(%1), 32(%1),  96(%1),  0(%0),/nop, 20)
IDCT(   8(%1), 72(%1), 40(%1), 104(%1),  4(%0),/nop, 20)
IDCT(  16(%1), 80(%1), 48(%1), 112(%1),  8(%0),/nop, 20)
IDCT(  24(%1), 88(%1), 56(%1), 120(%1), 12(%0),/nop, 20)
	"jmp 9f					\n\t"

	"#.balign 16				\n\t"\
	"5:					\n\t"
#undef IDCT
#define IDCT(src0, src4, src1, src5, dst, rounder, shift) \
	"movq " #src0 ", %%mm0			\n\t" /* R4	R0	r4	r0 */\
	"movq " #src4 ", %%mm1			\n\t" /* R6	R2	r6	r2 */\
	"movq 16(%2), %%mm4			\n\t" /* C4	C4	C4	C4 */\
	"pmaddwd %%mm0, %%mm4			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"movq 24(%2), %%mm5			\n\t" /* -C4	C4	-C4	C4 */\
	"pmaddwd %%mm5, %%mm0			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	"movq 32(%2), %%mm5			\n\t" /* C6	C2	C6	C2 */\
	"pmaddwd %%mm1, %%mm5			\n\t" /* C6R6+C2R2	C6r6+C2r2 */\
	"movq 40(%2), %%mm6			\n\t" /* -C2	C6	-C2	C6 */\
	"pmaddwd %%mm6, %%mm1			\n\t" /* -C2R6+C6R2	-C2r6+C6r2 */\
	#rounder ", %%mm4			\n\t"\
	"movq %%mm4, %%mm6			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"paddd %%mm5, %%mm4			\n\t" /* A0		a0 */\
	#rounder ", %%mm0			\n\t"\
	"psubd %%mm5, %%mm6			\n\t" /* A3		a3 */\
	"movq %%mm0, %%mm5			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	"paddd %%mm1, %%mm0			\n\t" /* A1		a1 */\
	"psubd %%mm1, %%mm5			\n\t" /* A2		a2 */\
	"movq 8+" #src0 ", %%mm2		\n\t" /* R4	R0	r4	r0 */\
	"movq 8+" #src4 ", %%mm3		\n\t" /* R6	R2	r6	r2 */\
	"movq 16(%2), %%mm1			\n\t" /* C4	C4	C4	C4 */\
	"pmaddwd %%mm2, %%mm1			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"movq 24(%2), %%mm7			\n\t" /* -C4	C4	-C4	C4 */\
	"pmaddwd %%mm7, %%mm2			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	"movq 32(%2), %%mm7			\n\t" /* C6	C2	C6	C2 */\
	"pmaddwd %%mm3, %%mm7			\n\t" /* C6R6+C2R2	C6r6+C2r2 */\
	"pmaddwd 40(%2), %%mm3			\n\t" /* -C2R6+C6R2	-C2r6+C6r2 */\
	#rounder ", %%mm1			\n\t"\
	"paddd %%mm1, %%mm7			\n\t" /* A0		a0 */\
	"paddd %%mm1, %%mm1			\n\t" /* 2C0		2c0 */\
	#rounder ", %%mm2			\n\t"\
	"psubd %%mm7, %%mm1			\n\t" /* A3		a3 */\
	"paddd %%mm2, %%mm3			\n\t" /* A1		a1 */\
	"paddd %%mm2, %%mm2			\n\t" /* 2C1		2c1 */\
	"psubd %%mm3, %%mm2			\n\t" /* A2		a2 */\
	"psrad $" #shift ", %%mm4		\n\t"\
	"psrad $" #shift ", %%mm7		\n\t"\
	"psrad $" #shift ", %%mm3		\n\t"\
	"packssdw %%mm7, %%mm4			\n\t" /* A0	a0 */\
	"movq %%mm4, " #dst "			\n\t"\
	"psrad $" #shift ", %%mm0		\n\t"\
	"packssdw %%mm3, %%mm0			\n\t" /* A1	a1 */\
	"movq %%mm0, 16+" #dst "		\n\t"\
	"movq %%mm0, 96+" #dst "		\n\t"\
	"movq %%mm4, 112+" #dst "		\n\t"\
	"psrad $" #shift ", %%mm5		\n\t"\
	"psrad $" #shift ", %%mm6		\n\t"\
	"psrad $" #shift ", %%mm2		\n\t"\
	"packssdw %%mm2, %%mm5			\n\t" /* A2-B2	a2-b2 */\
	"movq %%mm5, 32+" #dst "		\n\t"\
	"psrad $" #shift ", %%mm1		\n\t"\
	"packssdw %%mm1, %%mm6			\n\t" /* A3+B3	a3+b3 */\
	"movq %%mm6, 48+" #dst "		\n\t"\
	"movq %%mm6, 64+" #dst "		\n\t"\
	"movq %%mm5, 80+" #dst "		\n\t"	
	

//IDCT(  src0,   src4,   src1,    src5,    dst, rounder, shift)
IDCT(    0(%1), 64(%1), 32(%1),  96(%1),  0(%0),/nop, 20)
//IDCT(   8(%1), 72(%1), 40(%1), 104(%1),  4(%0),/nop, 20)
IDCT(  16(%1), 80(%1), 48(%1), 112(%1),  8(%0),/nop, 20)
//IDCT(  24(%1), 88(%1), 56(%1), 120(%1), 12(%0),/nop, 20)
	"jmp 9f					\n\t"


	"#.balign 16				\n\t"\
	"1:					\n\t"
#undef IDCT
#define IDCT(src0, src4, src1, src5, dst, rounder, shift) \
	"movq " #src0 ", %%mm0			\n\t" /* R4	R0	r4	r0 */\
	"movq " #src4 ", %%mm1			\n\t" /* R6	R2	r6	r2 */\
	"movq " #src1 ", %%mm2			\n\t" /* R3	R1	r3	r1 */\
	"movq 16(%2), %%mm4			\n\t" /* C4	C4	C4	C4 */\
	"pmaddwd %%mm0, %%mm4			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"movq 24(%2), %%mm5			\n\t" /* -C4	C4	-C4	C4 */\
	"pmaddwd %%mm5, %%mm0			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	"movq 32(%2), %%mm5			\n\t" /* C6	C2	C6	C2 */\
	"pmaddwd %%mm1, %%mm5			\n\t" /* C6R6+C2R2	C6r6+C2r2 */\
	"movq 40(%2), %%mm6			\n\t" /* -C2	C6	-C2	C6 */\
	"pmaddwd %%mm6, %%mm1			\n\t" /* -C2R6+C6R2	-C2r6+C6r2 */\
	#rounder ", %%mm4			\n\t"\
	"movq %%mm4, %%mm6			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"movq 48(%2), %%mm7			\n\t" /* C3	C1	C3	C1 */\
	#rounder ", %%mm0			\n\t"\
	"pmaddwd %%mm2, %%mm7			\n\t" /* C3R3+C1R1	C3r3+C1r1 */\
	"paddd %%mm5, %%mm4			\n\t" /* A0		a0 */\
	"psubd %%mm5, %%mm6			\n\t" /* A3		a3 */\
	"movq %%mm0, %%mm5			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	"paddd %%mm1, %%mm0			\n\t" /* A1		a1 */\
	"psubd %%mm1, %%mm5			\n\t" /* A2		a2 */\
	"movq 64(%2), %%mm1			\n\t"\
	"pmaddwd %%mm2, %%mm1			\n\t" /* -C7R3+C3R1	-C7r3+C3r1 */\
	"paddd %%mm4, %%mm7			\n\t" /* A0+B0		a0+b0 */\
	"paddd %%mm4, %%mm4			\n\t" /* 2A0		2a0 */\
	"psubd %%mm7, %%mm4			\n\t" /* A0-B0		a0-b0 */\
	"psrad $" #shift ", %%mm7		\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"movq %%mm0, %%mm3			\n\t" /* A1		a1 */\
	"paddd %%mm1, %%mm0			\n\t" /* A1+B1		a1+b1 */\
	"psubd %%mm1, %%mm3			\n\t" /* A1-B1		a1-b1 */\
	"psrad $" #shift ", %%mm0		\n\t"\
	"psrad $" #shift ", %%mm3		\n\t"\
	"packssdw %%mm7, %%mm7			\n\t" /* A0+B0	a0+b0 */\
	"movd %%mm7, " #dst "			\n\t"\
	"packssdw %%mm0, %%mm0			\n\t" /* A1+B1	a1+b1 */\
	"movd %%mm0, 16+" #dst "		\n\t"\
	"packssdw %%mm3, %%mm3			\n\t" /* A1-B1	a1-b1 */\
	"movd %%mm3, 96+" #dst "		\n\t"\
	"packssdw %%mm4, %%mm4			\n\t" /* A0-B0	a0-b0 */\
	"movd %%mm4, 112+" #dst "		\n\t"\
	"movq 80(%2), %%mm4			\n\t" /* -C1	C5	-C1 	C5 */\
	"pmaddwd %%mm2, %%mm4			\n\t" /* -C1R3+C5R1	-C1r3+C5r1 */\
	"pmaddwd 96(%2), %%mm2			\n\t" /* -C5R3+C7R1	-C5r3+C7r1 */\
	"movq %%mm5, %%mm3			\n\t" /* A2		a2 */\
	"paddd %%mm4, %%mm3			\n\t" /* A2+B2		a2+b2 */\
	"psubd %%mm4, %%mm5			\n\t" /* a2-B2		a2-b2 */\
	"psrad $" #shift ", %%mm3		\n\t"\
	"psrad $" #shift ", %%mm5		\n\t"\
	"movq %%mm6, %%mm4			\n\t" /* A3		a3 */\
	"paddd %%mm2, %%mm6			\n\t" /* A3+B3		a3+b3 */\
	"psubd %%mm2, %%mm4			\n\t" /* a3-B3		a3-b3 */\
	"psrad $" #shift ", %%mm6		\n\t"\
	"packssdw %%mm3, %%mm3			\n\t" /* A2+B2	a2+b2 */\
	"movd %%mm3, 32+" #dst "		\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"packssdw %%mm6, %%mm6			\n\t" /* A3+B3	a3+b3 */\
	"movd %%mm6, 48+" #dst "		\n\t"\
	"packssdw %%mm4, %%mm4			\n\t" /* A3-B3	a3-b3 */\
	"packssdw %%mm5, %%mm5			\n\t" /* A2-B2	a2-b2 */\
	"movd %%mm4, 64+" #dst "		\n\t"\
	"movd %%mm5, 80+" #dst "		\n\t"
	

//IDCT(  src0,   src4,   src1,    src5,    dst, rounder, shift)
IDCT(    (%1), 64(%1), 32(%1),  96(%1),  0(%0),/nop, 20)
IDCT(   8(%1), 72(%1), 40(%1), 104(%1),  4(%0),/nop, 20)
IDCT(  16(%1), 80(%1), 48(%1), 112(%1),  8(%0),/nop, 20)
IDCT(  24(%1), 88(%1), 56(%1), 120(%1), 12(%0),/nop, 20)
	"jmp 9f					\n\t"


	"#.balign 16				\n\t"
	"7:					\n\t"
#undef IDCT
#define IDCT(src0, src4, src1, src5, dst, rounder, shift) \
	"movq " #src0 ", %%mm0			\n\t" /* R4	R0	r4	r0 */\
	"movq 16(%2), %%mm4			\n\t" /* C4	C4	C4	C4 */\
	"pmaddwd %%mm0, %%mm4			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"movq 24(%2), %%mm5			\n\t" /* -C4	C4	-C4	C4 */\
	"pmaddwd %%mm5, %%mm0			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	#rounder ", %%mm4			\n\t"\
	#rounder ", %%mm0			\n\t"\
	"psrad $" #shift ", %%mm4		\n\t"\
	"psrad $" #shift ", %%mm0		\n\t"\
	"movq 8+" #src0 ", %%mm2		\n\t" /* R4	R0	r4	r0 */\
	"movq 16(%2), %%mm1			\n\t" /* C4	C4	C4	C4 */\
	"pmaddwd %%mm2, %%mm1			\n\t" /* C4R4+C4R0	C4r4+C4r0 */\
	"movq 24(%2), %%mm7			\n\t" /* -C4	C4	-C4	C4 */\
	"pmaddwd %%mm7, %%mm2			\n\t" /* -C4R4+C4R0	-C4r4+C4r0 */\
	"movq 32(%2), %%mm7			\n\t" /* C6	C2	C6	C2 */\
	#rounder ", %%mm1			\n\t"\
	#rounder ", %%mm2			\n\t"\
	"psrad $" #shift ", %%mm1		\n\t"\
	"packssdw %%mm1, %%mm4			\n\t" /* A0	a0 */\
	"movq %%mm4, " #dst "			\n\t"\
	"psrad $" #shift ", %%mm2		\n\t"\
	"packssdw %%mm2, %%mm0			\n\t" /* A1	a1 */\
	"movq %%mm0, 16+" #dst "		\n\t"\
	"movq %%mm0, 96+" #dst "		\n\t"\
	"movq %%mm4, 112+" #dst "		\n\t"\
	"movq %%mm0, 32+" #dst "		\n\t"\
	"movq %%mm4, 48+" #dst "		\n\t"\
	"movq %%mm4, 64+" #dst "		\n\t"\
	"movq %%mm0, 80+" #dst "		\n\t"	

//IDCT(  src0,   src4,   src1,    src5,    dst, rounder, shift)
IDCT(   0(%1), 64(%1), 32(%1),  96(%1),  0(%0),/nop, 20)
//IDCT(   8(%1), 72(%1), 40(%1), 104(%1),  4(%0),/nop, 20)
IDCT(  16(%1), 80(%1), 48(%1), 112(%1),  8(%0),/nop, 20)
//IDCT(  24(%1), 88(%1), 56(%1), 120(%1), 12(%0),/nop, 20)


#endif

/*
Input
 00 40 04 44 20 60 24 64
 10 30 14 34 50 70 54 74
 01 41 03 43 21 61 23 63
 11 31 13 33 51 71 53 73
 02 42 06 46 22 62 26 66
 12 32 16 36 52 72 56 76
 05 45 07 47 25 65 27 67
 15 35 17 37 55 75 57 77
  
Temp
 00 04 10 14 20 24 30 34
 40 44 50 54 60 64 70 74
 01 03 11 13 21 23 31 33
 41 43 51 53 61 63 71 73
 02 06 12 16 22 26 32 36
 42 46 52 56 62 66 72 76
 05 07 15 17 25 27 35 37
 45 47 55 57 65 67 75 77
*/

"9: \n\t"
		:: "r" (block), "r" (temp), "r" (coeffs)
		: "%eax"
	);
}

void ff_simple_idct_mmx(int16_t *block)
{
    idct(block);
}

//FIXME merge add/put into the idct

void put_pixels_clamped_mmx_srg2(const DCTELEM *block, uint8_t *pixels, int line_size)
{
    const DCTELEM *p;
    uint8_t *pix;

    /* read the pixels */
    p = block;
    pix = pixels;
    /* unrolled loop */
	__asm __volatile(
		"movq	%3, %%mm0\n\t"
		"movq	8%3, %%mm1\n\t"
		"movq	16%3, %%mm2\n\t"
		"movq	24%3, %%mm3\n\t"
		"movq	32%3, %%mm4\n\t"
		"movq	40%3, %%mm5\n\t"
		"movq	48%3, %%mm6\n\t"
		"movq	56%3, %%mm7\n\t"
		"packuswb %%mm1, %%mm0\n\t"
		"packuswb %%mm3, %%mm2\n\t"
		"packuswb %%mm5, %%mm4\n\t"
		"packuswb %%mm7, %%mm6\n\t"
		"movq	%%mm0, (%0)\n\t"
		"movq	%%mm2, (%0, %1)\n\t"
		"movq	%%mm4, (%0, %1, 2)\n\t"
		"movq	%%mm6, (%0, %2)\n\t"
		::"r" (pix), "r" (line_size), "r" (line_size*3), "m"(*p)
		:"memory");
        pix += line_size*4;
        p += 32;

    // if here would be an exact copy of the code above
    // compiler would generate some very strange code
    // thus using "r"
    __asm __volatile(
	    "movq	(%3), %%mm0\n\t"
	    "movq	8(%3), %%mm1\n\t"
	    "movq	16(%3), %%mm2\n\t"
	    "movq	24(%3), %%mm3\n\t"
	    "movq	32(%3), %%mm4\n\t"
	    "movq	40(%3), %%mm5\n\t"
	    "movq	48(%3), %%mm6\n\t"
	    "movq	56(%3), %%mm7\n\t"
	    "packuswb %%mm1, %%mm0\n\t"
	    "packuswb %%mm3, %%mm2\n\t"
	    "packuswb %%mm5, %%mm4\n\t"
	    "packuswb %%mm7, %%mm6\n\t"
	    "movq	%%mm0, (%0)\n\t"
	    "movq	%%mm2, (%0, %1)\n\t"
	    "movq	%%mm4, (%0, %1, 2)\n\t"
	    "movq	%%mm6, (%0, %2)\n\t"
	    ::"r" (pix), "r" (line_size), "r" (line_size*3), "r"(p)
	    :"memory");
}

#define MOVQ_ZERO(regd)  __asm __volatile ("pxor %%" #regd ", %%" #regd ::)

void add_pixels_clamped_mmx_srg2(const DCTELEM *block, uint8_t *pixels, int line_size)
{
    const DCTELEM *p;
    uint8_t *pix;
    int i;

    /* read the pixels */
    p = block;
    pix = pixels;
    MOVQ_ZERO(mm7);
    i = 4;
    do {
	__asm __volatile(
		"movq	(%2), %%mm0\n\t"
		"movq	8(%2), %%mm1\n\t"
		"movq	16(%2), %%mm2\n\t"
		"movq	24(%2), %%mm3\n\t"
		"movq	%0, %%mm4\n\t"
		"movq	%1, %%mm6\n\t"
		"movq	%%mm4, %%mm5\n\t"
		"punpcklbw %%mm7, %%mm4\n\t"
		"punpckhbw %%mm7, %%mm5\n\t"
		"paddsw	%%mm4, %%mm0\n\t"
		"paddsw	%%mm5, %%mm1\n\t"
		"movq	%%mm6, %%mm5\n\t"
		"punpcklbw %%mm7, %%mm6\n\t"
		"punpckhbw %%mm7, %%mm5\n\t"
		"paddsw	%%mm6, %%mm2\n\t"
		"paddsw	%%mm5, %%mm3\n\t"
		"packuswb %%mm1, %%mm0\n\t"
		"packuswb %%mm3, %%mm2\n\t"
		"movq	%%mm0, %0\n\t"
		"movq	%%mm2, %1\n\t"
		:"+m"(*pix), "+m"(*(pix+line_size))
		:"r"(p)
		:"memory");
        pix += line_size*2;
        p += 16;
    } while (--i);
}

void ff_simple_idct_put_mmx(uint8_t *dest, int line_size, DCTELEM *block)
{
    idct(block);
    put_pixels_clamped_mmx_srg2(block, dest, line_size);
}
void ff_simple_idct_add_mmx(uint8_t *dest, int line_size, DCTELEM *block)
{
//	mmx_print_message("doing ff_simple_idct_add_mmx\n");
    idct(block);
    add_pixels_clamped_mmx_srg2(block, dest, line_size);
}


