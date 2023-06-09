/*
 * Motion estimation 
 * Copyright (c) 2000,2001 Fabrice Bellard.
 * Copyright (c) 2002-2004 Michael Niedermayer
 * 
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 *
 * new Motion Estimation (X1/EPZS) by Michael Niedermayer <michaelni@gmx.at>
 */
 
/**
 * @file motion_est.c
 * Motion estimation.
 */
 
#include <stdlib.h>
#include <stdio.h>
#include <limits.h>
#include "avcodec.h"
#include "dsputil.h"
#include "mpegvideo.h"

#undef NDEBUG
#include <assert.h>

#define SQ(a) ((a)*(a))

#define P_LEFT P[1]
#define P_TOP P[2]
#define P_TOPRIGHT P[3]
#define P_MEDIAN P[4]
#define P_MV1 P[9]

static __inline int sad_hpel_motion_search(MpegEncContext * s,
				  int *mx_ptr, int *my_ptr, int dmin,
                                  int src_index, int ref_index,
                                  int size, int h);

static __inline int update_map_generation(MotionEstContext *c)
{
    c->map_generation+= 1<<(ME_MAP_MV_BITS*2);
    if(c->map_generation==0){
        c->map_generation= 1<<(ME_MAP_MV_BITS*2);
        memset(c->map, 0, sizeof(uint32_t)*ME_MAP_SIZE);
    }
    return c->map_generation;
}

/* shape adaptive search stuff */
typedef struct Minima{
    int height;
    int x, y;
    int checked;
}Minima;

static int minima_cmp(const void *a, const void *b){
    const Minima *da = (const Minima *) a;
    const Minima *db = (const Minima *) b;
    
    return da->height - db->height;
}

#define FLAG_QPEL   1 //must be 1
#define FLAG_CHROMA 2
#define FLAG_DIRECT 4

static __inline void init_ref(MotionEstContext *c, uint8_t *src[3], uint8_t *ref[3], uint8_t *ref2[3], int x, int y, int ref_index){
    const int offset[3]= {
          y*c->  stride + x,
        ((y*c->uvstride + x)>>1),
        ((y*c->uvstride + x)>>1),
    };
    int i;
    for(i=0; i<3; i++){
        c->src[0][i]= src [i] + offset[i];
        c->ref[0][i]= ref [i] + offset[i];
    }
    if(ref_index){
        for(i=0; i<3; i++){
            c->ref[ref_index][i]= ref2[i] + offset[i];
        }
    }
}

static int get_flags(MotionEstContext *c, int direct, int chroma){
    return   ((c->avctx->flags&CODEC_FLAG_QPEL) ? FLAG_QPEL : 0)
           + (direct ? FLAG_DIRECT : 0) 
           + (chroma ? FLAG_CHROMA : 0);
}

static __inline int cmp(MpegEncContext *s, const int x, const int y, const int subx, const int suby,
                      const int size, const int h, int ref_index, int src_index,
                      me_cmp_func cmp_func, me_cmp_func chroma_cmp_func, const int flags){
    MotionEstContext * const c= &s->me;
    const int stride= c->stride;
    const int uvstride= c->uvstride;
    const int qpel= flags&FLAG_QPEL;
    const int chroma= flags&FLAG_CHROMA;
    const int dxy= subx + (suby<<(1+qpel)); //FIXME log2_subpel?
    const int hx= subx + (x<<(1+qpel));
    const int hy= suby + (y<<(1+qpel));
    uint8_t * const * const ref= c->ref[ref_index];
    uint8_t * const * const src= c->src[src_index];
    int d;
    //FIXME check chroma 4mv, (no crashes ...)
    if(flags&FLAG_DIRECT){
        if(x >= c->xmin && hx <= c->xmax<<(qpel+1) && y >= c->ymin && hy <= c->ymax<<(qpel+1)){
            const int time_pp= s->pp_time;
            const int time_pb= s->pb_time;
            const int mask= 2*qpel+1;
            if(s->mv_type==MV_TYPE_8X8){
                int i;
                for(i=0; i<4; i++){
                    int fx = c->direct_basis_mv[i][0] + hx;
                    int fy = c->direct_basis_mv[i][1] + hy;
                    int bx = hx ? fx - c->co_located_mv[i][0] : c->co_located_mv[i][0]*(time_pb - time_pp)/time_pp + ((i &1)<<(qpel+4));
                    int by = hy ? fy - c->co_located_mv[i][1] : c->co_located_mv[i][1]*(time_pb - time_pp)/time_pp + ((i>>1)<<(qpel+4));
                    int fxy= (fx&mask) + ((fy&mask)<<(qpel+1));
                    int bxy= (bx&mask) + ((by&mask)<<(qpel+1));
        
                    uint8_t *dst= c->temp + 8*(i&1) + 8*stride*(i>>1);
                    if(qpel){
                        c->qpel_put[1][fxy](dst, ref[0] + (fx>>2) + (fy>>2)*stride, stride);
                        c->qpel_avg[1][bxy](dst, ref[8] + (bx>>2) + (by>>2)*stride, stride);
                    }else{
                        c->hpel_put[1][fxy](dst, ref[0] + (fx>>1) + (fy>>1)*stride, stride, 8);
                        c->hpel_avg[1][bxy](dst, ref[8] + (bx>>1) + (by>>1)*stride, stride, 8);
                    }
                }
            }else{
                int fx = c->direct_basis_mv[0][0] + hx;
                int fy = c->direct_basis_mv[0][1] + hy;
                int bx = hx ? fx - c->co_located_mv[0][0] : (c->co_located_mv[0][0]*(time_pb - time_pp)/time_pp);
                int by = hy ? fy - c->co_located_mv[0][1] : (c->co_located_mv[0][1]*(time_pb - time_pp)/time_pp);
                int fxy= (fx&mask) + ((fy&mask)<<(qpel+1));
                int bxy= (bx&mask) + ((by&mask)<<(qpel+1));
                
                if(qpel){
                    c->qpel_put[1][fxy](c->temp               , ref[0] + (fx>>2) + (fy>>2)*stride               , stride);
                    c->qpel_put[1][fxy](c->temp + 8           , ref[0] + (fx>>2) + (fy>>2)*stride + 8           , stride);
                    c->qpel_put[1][fxy](c->temp     + 8*stride, ref[0] + (fx>>2) + (fy>>2)*stride     + 8*stride, stride);
                    c->qpel_put[1][fxy](c->temp + 8 + 8*stride, ref[0] + (fx>>2) + (fy>>2)*stride + 8 + 8*stride, stride);
                    c->qpel_avg[1][bxy](c->temp               , ref[8] + (bx>>2) + (by>>2)*stride               , stride);
                    c->qpel_avg[1][bxy](c->temp + 8           , ref[8] + (bx>>2) + (by>>2)*stride + 8           , stride);
                    c->qpel_avg[1][bxy](c->temp     + 8*stride, ref[8] + (bx>>2) + (by>>2)*stride     + 8*stride, stride);
                    c->qpel_avg[1][bxy](c->temp + 8 + 8*stride, ref[8] + (bx>>2) + (by>>2)*stride + 8 + 8*stride, stride);
                }else{            
                    assert((fx>>1) + 16*s->mb_x >= -16);
                    assert((fy>>1) + 16*s->mb_y >= -16);
                    assert((fx>>1) + 16*s->mb_x <= s->width);
                    assert((fy>>1) + 16*s->mb_y <= s->height);
                    assert((bx>>1) + 16*s->mb_x >= -16);
                    assert((by>>1) + 16*s->mb_y >= -16);
                    assert((bx>>1) + 16*s->mb_x <= s->width);
                    assert((by>>1) + 16*s->mb_y <= s->height);

                    c->hpel_put[0][fxy](c->temp, ref[0] + (fx>>1) + (fy>>1)*stride, stride, 16);
                    c->hpel_avg[0][bxy](c->temp, ref[8] + (bx>>1) + (by>>1)*stride, stride, 16);
                }
            }
            d = cmp_func(s, c->temp, src[0], stride, 16);
        }else
            d= 256*256*256*32;
    }else{
        int uvdxy;
        if(dxy){
            if(qpel){
                c->qpel_put[size][dxy](c->temp, ref[0] + x + y*stride, stride); //FIXME prototype (add h)
                if(chroma){
                    int cx= hx/2;
                    int cy= hy/2;
                    cx= (cx>>1)|(cx&1);
                    cy= (cy>>1)|(cy&1);
                    uvdxy= (cx&1) + 2*(cy&1);
                    //FIXME x/y wrong, but mpeg4 qpel is sick anyway, we should drop as much of it as possible in favor for h264
                }
            }else{
                c->hpel_put[size][dxy](c->temp, ref[0] + x + y*stride, stride, h);
                if(chroma)
                    uvdxy= dxy | (x&1) | (2*(y&1));
            }
            d = cmp_func(s, c->temp, src[0], stride, h); 
        }else{
            d = cmp_func(s, src[0], ref[0] + x + y*stride, stride, h); 
            if(chroma)
                uvdxy= (x&1) + 2*(y&1);
        }
        if(chroma){
            uint8_t * const uvtemp= c->temp + 16*stride;
            c->hpel_put[size+1][uvdxy](uvtemp  , ref[1] + (x>>1) + (y>>1)*uvstride, uvstride, h>>1);
            c->hpel_put[size+1][uvdxy](uvtemp+8, ref[2] + (x>>1) + (y>>1)*uvstride, uvstride, h>>1);
            d += chroma_cmp_func(s, uvtemp  , src[1], uvstride, h>>1); 
            d += chroma_cmp_func(s, uvtemp+8, src[2], uvstride, h>>1); 
        }
    }
#if 0
    if(full_pel){
        const int index= (((y)<<ME_MAP_SHIFT) + (x))&(ME_MAP_SIZE-1);
        score_map[index]= d;
    }

    d += (c->mv_penalty[hx - c->pred_x] + c->mv_penalty[hy - c->pred_y])*c->penalty_factor;
#endif
    return d;
}

#include "motion_est_template.c"

static __inline int get_penalty_factor(MpegEncContext *s, int type){
    switch(type&0xFF){
    default:
    case FF_CMP_SAD:
        return s->lambda>>FF_LAMBDA_SHIFT;
    case FF_CMP_DCT:
        return (3*s->lambda)>>(FF_LAMBDA_SHIFT+1);
    case FF_CMP_SATD:
        return (2*s->lambda)>>FF_LAMBDA_SHIFT;
    case FF_CMP_RD:
    case FF_CMP_PSNR:
    case FF_CMP_SSE:
    case FF_CMP_NSSE:
        return s->lambda2>>FF_LAMBDA_SHIFT;
    case FF_CMP_BIT:
        return 1;
    }
}

static int zero_cmp(void *s, uint8_t *a, uint8_t *b, int stride, int h){
    return 0;
}

static void zero_hpel(uint8_t *a, const uint8_t *b, int stride, int h){
}

void ff_init_me(MpegEncContext *s){
    MotionEstContext * const c= &s->me;
    c->avctx= s->avctx;

    ff_set_cmp(&s->dsp, s->dsp.me_pre_cmp, c->avctx->me_pre_cmp);
    ff_set_cmp(&s->dsp, s->dsp.me_cmp, c->avctx->me_cmp);
    ff_set_cmp(&s->dsp, s->dsp.me_sub_cmp, c->avctx->me_sub_cmp);
    ff_set_cmp(&s->dsp, s->dsp.mb_cmp, c->avctx->mb_cmp);
    
    c->flags    = get_flags(c, 0, c->avctx->me_cmp    &FF_CMP_CHROMA);
    c->sub_flags= get_flags(c, 0, c->avctx->me_sub_cmp&FF_CMP_CHROMA);
    c->mb_flags = get_flags(c, 0, c->avctx->mb_cmp    &FF_CMP_CHROMA);

/*FIXME s->no_rounding b_type*/
    if(s->flags&CODEC_FLAG_QPEL){
        c->sub_motion_search= qpel_motion_search;
        c->qpel_avg= s->dsp.avg_qpel_pixels_tab;
        if(s->no_rounding) c->qpel_put= s->dsp.put_no_rnd_qpel_pixels_tab;
        else               c->qpel_put= s->dsp.put_qpel_pixels_tab;
    }else{
        if(c->avctx->me_sub_cmp&FF_CMP_CHROMA)
            c->sub_motion_search= hpel_motion_search;
        else if(   c->avctx->me_sub_cmp == FF_CMP_SAD 
                && c->avctx->    me_cmp == FF_CMP_SAD 
                && c->avctx->    mb_cmp == FF_CMP_SAD)
            c->sub_motion_search= sad_hpel_motion_search; // 2050 vs. 2450 cycles
        else
            c->sub_motion_search= hpel_motion_search;
    }
    c->hpel_avg= s->dsp.avg_pixels_tab;
    if(s->no_rounding) c->hpel_put= s->dsp.put_no_rnd_pixels_tab;
    else               c->hpel_put= s->dsp.put_pixels_tab;

    if(s->linesize){
        c->stride  = s->linesize; 
        c->uvstride= s->uvlinesize;
    }else{
        c->stride  = 16*s->mb_width + 32;
        c->uvstride=  8*s->mb_width + 16;
    }

    // 8x8 fullpel search would need a 4x4 chroma compare, which we dont have yet, and even if we had the motion estimation code doesnt expect it
    if((c->avctx->me_cmp&FF_CMP_CHROMA) && !s->dsp.me_cmp[2]){
        s->dsp.me_cmp[2]= zero_cmp;
    }
    if((c->avctx->me_sub_cmp&FF_CMP_CHROMA) && !s->dsp.me_sub_cmp[2]){
        s->dsp.me_sub_cmp[2]= zero_cmp;
    }
    c->hpel_put[2][0]= c->hpel_put[2][1]=
    c->hpel_put[2][2]= c->hpel_put[2][3]= zero_hpel;

    c->temp= c->scratchpad;
}
      
#if 0
static int pix_dev(uint8_t * pix, int line_size, int mean)
{
    int s, i, j;

    s = 0;
    for (i = 0; i < 16; i++) {
	for (j = 0; j < 16; j += 8) {
	    s += ABS(pix[0]-mean);
	    s += ABS(pix[1]-mean);
	    s += ABS(pix[2]-mean);
	    s += ABS(pix[3]-mean);
	    s += ABS(pix[4]-mean);
	    s += ABS(pix[5]-mean);
	    s += ABS(pix[6]-mean);
	    s += ABS(pix[7]-mean);
	    pix += 8;
	}
	pix += line_size - 16;
    }
    return s;
}
#endif

static __inline void no_motion_search(MpegEncContext * s,
				    int *mx_ptr, int *my_ptr)
{
    *mx_ptr = 16 * s->mb_x;
    *my_ptr = 16 * s->mb_y;
}

static int full_motion_search(MpegEncContext * s,
                              int *mx_ptr, int *my_ptr, int range,
                              int xmin, int ymin, int xmax, int ymax, uint8_t *ref_picture)
{
    int x1, y1, x2, y2, xx, yy, x, y;
    int mx, my, dmin, d;
    uint8_t *pix;

    xx = 16 * s->mb_x;
    yy = 16 * s->mb_y;
    x1 = xx - range + 1;	/* we loose one pixel to avoid boundary pb with half pixel pred */
    if (x1 < xmin)
	x1 = xmin;
    x2 = xx + range - 1;
    if (x2 > xmax)
	x2 = xmax;
    y1 = yy - range + 1;
    if (y1 < ymin)
	y1 = ymin;
    y2 = yy + range - 1;
    if (y2 > ymax)
	y2 = ymax;
    pix = s->new_picture.data[0] + (yy * s->linesize) + xx;
    dmin = 0x7fffffff;
    mx = 0;
    my = 0;
    for (y = y1; y <= y2; y++) {
	for (x = x1; x <= x2; x++) {
	    d = s->dsp.pix_abs[0][0](NULL, pix, ref_picture + (y * s->linesize) + x,
			     s->linesize, 16);
	    if (d < dmin ||
		(d == dmin &&
		 (abs(x - xx) + abs(y - yy)) <
		 (abs(mx - xx) + abs(my - yy)))) {
		dmin = d;
		mx = x;
		my = y;
	    }
	}
    }

    *mx_ptr = mx;
    *my_ptr = my;

#if 0
    if (*mx_ptr < -(2 * range) || *mx_ptr >= (2 * range) ||
	*my_ptr < -(2 * range) || *my_ptr >= (2 * range)) {
	fprintf(stderr, "error %d %d\n", *mx_ptr, *my_ptr);
    }
#endif
    return dmin;
}


static int log_motion_search(MpegEncContext * s,
                             int *mx_ptr, int *my_ptr, int range,
                             int xmin, int ymin, int xmax, int ymax, uint8_t *ref_picture)
{
    int x1, y1, x2, y2, xx, yy, x, y;
    int mx, my, dmin, d;
    uint8_t *pix;

    xx = s->mb_x << 4;
    yy = s->mb_y << 4;

    /* Left limit */
    x1 = xx - range;
    if (x1 < xmin)
	x1 = xmin;

    /* Right limit */
    x2 = xx + range;
    if (x2 > xmax)
	x2 = xmax;

    /* Upper limit */
    y1 = yy - range;
    if (y1 < ymin)
	y1 = ymin;

    /* Lower limit */
    y2 = yy + range;
    if (y2 > ymax)
	y2 = ymax;

    pix = s->new_picture.data[0] + (yy * s->linesize) + xx;
    dmin = 0x7fffffff;
    mx = 0;
    my = 0;

    do {
	for (y = y1; y <= y2; y += range) {
	    for (x = x1; x <= x2; x += range) {
		d = s->dsp.pix_abs[0][0](NULL, pix, ref_picture + (y * s->linesize) + x, s->linesize, 16);
		if (d < dmin || (d == dmin && (abs(x - xx) + abs(y - yy)) < (abs(mx - xx) + abs(my - yy)))) {
		    dmin = d;
		    mx = x;
		    my = y;
		}
	    }
	}

	range = range >> 1;

	x1 = mx - range;
	if (x1 < xmin)
	    x1 = xmin;

	x2 = mx + range;
	if (x2 > xmax)
	    x2 = xmax;

	y1 = my - range;
	if (y1 < ymin)
	    y1 = ymin;

	y2 = my + range;
	if (y2 > ymax)
	    y2 = ymax;

    } while (range >= 1);

#ifdef DEBUG
    fprintf(stderr, "log       - MX: %d\tMY: %d\n", mx, my);
#endif
    *mx_ptr = mx;
    *my_ptr = my;
    return dmin;
}

static int phods_motion_search(MpegEncContext * s,
                               int *mx_ptr, int *my_ptr, int range,
                               int xmin, int ymin, int xmax, int ymax, uint8_t *ref_picture)
{
    int x1, y1, x2, y2, xx, yy, x, y, lastx, d;
    int mx, my, dminx, dminy;
    uint8_t *pix;

    xx = s->mb_x << 4;
    yy = s->mb_y << 4;

    /* Left limit */
    x1 = xx - range;
    if (x1 < xmin)
	x1 = xmin;

    /* Right limit */
    x2 = xx + range;
    if (x2 > xmax)
	x2 = xmax;

    /* Upper limit */
    y1 = yy - range;
    if (y1 < ymin)
	y1 = ymin;

    /* Lower limit */
    y2 = yy + range;
    if (y2 > ymax)
	y2 = ymax;

    pix = s->new_picture.data[0] + (yy * s->linesize) + xx;
    mx = 0;
    my = 0;

    x = xx;
    y = yy;
    do {
        dminx = 0x7fffffff;
        dminy = 0x7fffffff;

	lastx = x;
	for (x = x1; x <= x2; x += range) {
	    d = s->dsp.pix_abs[0][0](NULL, pix, ref_picture + (y * s->linesize) + x, s->linesize, 16);
	    if (d < dminx || (d == dminx && (abs(x - xx) + abs(y - yy)) < (abs(mx - xx) + abs(my - yy)))) {
		dminx = d;
		mx = x;
	    }
	}

	x = lastx;
	for (y = y1; y <= y2; y += range) {
	    d = s->dsp.pix_abs[0][0](NULL, pix, ref_picture + (y * s->linesize) + x, s->linesize, 16);
	    if (d < dminy || (d == dminy && (abs(x - xx) + abs(y - yy)) < (abs(mx - xx) + abs(my - yy)))) {
		dminy = d;
		my = y;
	    }
	}

	range = range >> 1;

	x = mx;
	y = my;
	x1 = mx - range;
	if (x1 < xmin)
	    x1 = xmin;

	x2 = mx + range;
	if (x2 > xmax)
	    x2 = xmax;

	y1 = my - range;
	if (y1 < ymin)
	    y1 = ymin;

	y2 = my + range;
	if (y2 > ymax)
	    y2 = ymax;

    } while (range >= 1);

#ifdef DEBUG
    fprintf(stderr, "phods     - MX: %d\tMY: %d\n", mx, my);
#endif

    /* half pixel search */
    *mx_ptr = mx;
    *my_ptr = my;
    return dminy;
}


#define Z_THRESHOLD 256

#define CHECK_SAD_HALF_MV(suffix, x, y) \
{\
    d= s->dsp.pix_abs[size][(x?1:0)+(y?2:0)](NULL, pix, ptr+((x)>>1), stride, h);\
    d += (mv_penalty[pen_x + x] + mv_penalty[pen_y + y])*penalty_factor;\
    COPY3_IF_LT(dminh, d, dx, x, dy, y)\
}

static __inline int sad_hpel_motion_search(MpegEncContext * s,
				  int *mx_ptr, int *my_ptr, int dmin,
                                  int src_index, int ref_index,
                                  int size, int h)
{
    MotionEstContext * const c= &s->me;
    const int penalty_factor= c->sub_penalty_factor;
    int mx, my, dminh;
    uint8_t *pix, *ptr;
    int stride= c->stride;
    const int flags= c->sub_flags;
    LOAD_COMMON
    
    assert(flags == 0);

    if(c->skip){
//    printf("S");
        *mx_ptr = 0;
        *my_ptr = 0;
        return dmin;
    }
//    printf("N");
        
    pix = c->src[src_index][0];

    mx = *mx_ptr;
    my = *my_ptr;
    ptr = c->ref[ref_index][0] + (my * stride) + mx;
    
    dminh = dmin;

    if (mx > xmin && mx < xmax && 
        my > ymin && my < ymax) {
        int dx=0, dy=0;
        int d, pen_x, pen_y; 
        const int index= (my<<ME_MAP_SHIFT) + mx;
        const int t= score_map[(index-(1<<ME_MAP_SHIFT))&(ME_MAP_SIZE-1)];
        const int l= score_map[(index- 1               )&(ME_MAP_SIZE-1)];
        const int r= score_map[(index+ 1               )&(ME_MAP_SIZE-1)];
        const int b= score_map[(index+(1<<ME_MAP_SHIFT))&(ME_MAP_SIZE-1)];
        mx<<=1;
        my<<=1;

        
        pen_x= pred_x + mx;
        pen_y= pred_y + my;

        ptr-= stride;
        if(t<=b){
            CHECK_SAD_HALF_MV(y2 , 0, -1)
            if(l<=r){
                CHECK_SAD_HALF_MV(xy2, -1, -1)
                if(t+r<=b+l){
                    CHECK_SAD_HALF_MV(xy2, +1, -1)
                    ptr+= stride;
                }else{
                    ptr+= stride;
                    CHECK_SAD_HALF_MV(xy2, -1, +1)
                }
                CHECK_SAD_HALF_MV(x2 , -1,  0)
            }else{
                CHECK_SAD_HALF_MV(xy2, +1, -1)
                if(t+l<=b+r){
                    CHECK_SAD_HALF_MV(xy2, -1, -1)
                    ptr+= stride;
                }else{
                    ptr+= stride;
                    CHECK_SAD_HALF_MV(xy2, +1, +1)
                }
                CHECK_SAD_HALF_MV(x2 , +1,  0)
            }
        }else{
            if(l<=r){
                if(t+l<=b+r){
                    CHECK_SAD_HALF_MV(xy2, -1, -1)
                    ptr+= stride;
                }else{
                    ptr+= stride;
                    CHECK_SAD_HALF_MV(xy2, +1, +1)
                }
                CHECK_SAD_HALF_MV(x2 , -1,  0)
                CHECK_SAD_HALF_MV(xy2, -1, +1)
            }else{
                if(t+r<=b+l){
                    CHECK_SAD_HALF_MV(xy2, +1, -1)
                    ptr+= stride;
                }else{
                    ptr+= stride;
                    CHECK_SAD_HALF_MV(xy2, -1, +1)
                }
                CHECK_SAD_HALF_MV(x2 , +1,  0)
                CHECK_SAD_HALF_MV(xy2, +1, +1)
            }
            CHECK_SAD_HALF_MV(y2 ,  0, +1)
        }
        mx+=dx;
        my+=dy;

    }else{
        mx<<=1;
        my<<=1;
    }

    *mx_ptr = mx;
    *my_ptr = my;
    return dminh;
}

static __inline void set_p_mv_tables(MpegEncContext * s, int mx, int my, int mv4)
{
    const int xy= s->mb_x + s->mb_y*s->mb_stride;
    
    s->p_mv_table[xy][0] = mx;
    s->p_mv_table[xy][1] = my;

    /* has allready been set to the 4 MV if 4MV is done */
    if(mv4){
        int mot_xy= s->block_index[0];

        s->current_picture.motion_val[0][mot_xy  ][0]= mx;
        s->current_picture.motion_val[0][mot_xy  ][1]= my;
        s->current_picture.motion_val[0][mot_xy+1][0]= mx;
        s->current_picture.motion_val[0][mot_xy+1][1]= my;

        mot_xy += s->b8_stride;
        s->current_picture.motion_val[0][mot_xy  ][0]= mx;
        s->current_picture.motion_val[0][mot_xy  ][1]= my;
        s->current_picture.motion_val[0][mot_xy+1][0]= mx;
        s->current_picture.motion_val[0][mot_xy+1][1]= my;
    }
}

/**
 * get fullpel ME search limits.
 */
static __inline void get_limits(MpegEncContext *s, int x, int y)
{
    MotionEstContext * const c= &s->me;
/*
    if(c->avctx->me_range) c->range= c->avctx->me_range >> 1;
    else                   c->range= 16;
*/
    if (s->unrestricted_mv) {
        c->xmin = - x - 16;
        c->ymin = - y - 16;
        c->xmax = - x + s->mb_width *16;
        c->ymax = - y + s->mb_height*16;
    } else {
        c->xmin = - x;
        c->ymin = - y;
        c->xmax = - x + s->mb_width *16 - 16;
        c->ymax = - y + s->mb_height*16 - 16;
    }
}

static __inline void init_mv4_ref(MotionEstContext *c){
    const int stride= c->stride;

    c->ref[1][0] = c->ref[0][0] + 8;
    c->ref[2][0] = c->ref[0][0] + 8*stride;
    c->ref[3][0] = c->ref[2][0] + 8;
    c->src[1][0] = c->src[0][0] + 8;
    c->src[2][0] = c->src[0][0] + 8*stride;
    c->src[3][0] = c->src[2][0] + 8;
}

static __inline int h263_mv4_search(MpegEncContext *s, int mx, int my, int shift)
{
    MotionEstContext * const c= &s->me;
    const int size= 1;
    const int h=8;
    int block;
    int P[10][2];
    int dmin_sum=0, mx4_sum=0, my4_sum=0;
    int same=1;
    const int stride= c->stride;
    const int uvstride= c->uvstride;
    uint8_t *mv_penalty= c->current_mv_penalty;

    init_mv4_ref(c);
    
    for(block=0; block<4; block++){
        int mx4, my4;
        int pred_x4, pred_y4;
        int dmin4;
        static const int off[4]= {2, 1, 1, -1};
        const int mot_stride = s->b8_stride;
        const int mot_xy = s->block_index[block];

        P_LEFT[0] = s->current_picture.motion_val[0][mot_xy - 1][0];
        P_LEFT[1] = s->current_picture.motion_val[0][mot_xy - 1][1];

        if(P_LEFT[0]       > (c->xmax<<shift)) P_LEFT[0]       = (c->xmax<<shift);

        /* special case for first line */
        if (s->first_slice_line && block<2) {
            c->pred_x= pred_x4= P_LEFT[0];
            c->pred_y= pred_y4= P_LEFT[1];
        } else {
            P_TOP[0]      = s->current_picture.motion_val[0][mot_xy - mot_stride             ][0];
            P_TOP[1]      = s->current_picture.motion_val[0][mot_xy - mot_stride             ][1];
            P_TOPRIGHT[0] = s->current_picture.motion_val[0][mot_xy - mot_stride + off[block]][0];
            P_TOPRIGHT[1] = s->current_picture.motion_val[0][mot_xy - mot_stride + off[block]][1];
            if(P_TOP[1]      > (c->ymax<<shift)) P_TOP[1]     = (c->ymax<<shift);
            if(P_TOPRIGHT[0] < (c->xmin<<shift)) P_TOPRIGHT[0]= (c->xmin<<shift);
            if(P_TOPRIGHT[0] > (c->xmax<<shift)) P_TOPRIGHT[0]= (c->xmax<<shift);
            if(P_TOPRIGHT[1] > (c->ymax<<shift)) P_TOPRIGHT[1]= (c->ymax<<shift);
    
            P_MEDIAN[0]= mid_pred(P_LEFT[0], P_TOP[0], P_TOPRIGHT[0]);
            P_MEDIAN[1]= mid_pred(P_LEFT[1], P_TOP[1], P_TOPRIGHT[1]);

            c->pred_x= pred_x4 = P_MEDIAN[0];
            c->pred_y= pred_y4 = P_MEDIAN[1];
        }
        P_MV1[0]= mx;
        P_MV1[1]= my;

        dmin4 = epzs_motion_search4(s, &mx4, &my4, P, block, block, s->p_mv_table, (1<<16)>>shift);

        dmin4= c->sub_motion_search(s, &mx4, &my4, dmin4, block, block, size, h);
        
        if(s->dsp.me_sub_cmp[0] != s->dsp.mb_cmp[0]){
            int dxy;
            const int offset= ((block&1) + (block>>1)*stride)*8;
            uint8_t *dest_y = c->scratchpad + offset;
            if(s->quarter_sample){
                uint8_t *ref= c->ref[block][0] + (mx4>>2) + (my4>>2)*stride;
                dxy = ((my4 & 3) << 2) | (mx4 & 3);

                if(s->no_rounding)
                    s->dsp.put_no_rnd_qpel_pixels_tab[1][dxy](dest_y   , ref    , stride);
                else
                    s->dsp.put_qpel_pixels_tab       [1][dxy](dest_y   , ref    , stride);
            }else{
                uint8_t *ref= c->ref[block][0] + (mx4>>1) + (my4>>1)*stride;
                dxy = ((my4 & 1) << 1) | (mx4 & 1);

                if(s->no_rounding)
                    s->dsp.put_no_rnd_pixels_tab[1][dxy](dest_y    , ref    , stride, h);
                else
                    s->dsp.put_pixels_tab       [1][dxy](dest_y    , ref    , stride, h);
            }
            dmin_sum+= (mv_penalty[mx4-pred_x4] + mv_penalty[my4-pred_y4])*c->mb_penalty_factor;
        }else
            dmin_sum+= dmin4;

        if(s->quarter_sample){
            mx4_sum+= mx4/2;
            my4_sum+= my4/2;
        }else{
            mx4_sum+= mx4;
            my4_sum+= my4;
        }
            
        s->current_picture.motion_val[0][ s->block_index[block] ][0]= mx4;
        s->current_picture.motion_val[0][ s->block_index[block] ][1]= my4;

        if(mx4 != mx || my4 != my) same=0;
    }
    
    if(same)
        return INT_MAX;
    
    if(s->dsp.me_sub_cmp[0] != s->dsp.mb_cmp[0]){
        dmin_sum += s->dsp.mb_cmp[0](s, s->new_picture.data[0] + s->mb_x*16 + s->mb_y*16*stride, c->scratchpad, stride, 16);
    }
    
    if(c->avctx->mb_cmp&FF_CMP_CHROMA){
        int dxy;
        int mx, my;
        int offset;

        mx= ff_h263_round_chroma(mx4_sum);
        my= ff_h263_round_chroma(my4_sum);
        dxy = ((my & 1) << 1) | (mx & 1);
        
        offset= (s->mb_x*8 + (mx>>1)) + (s->mb_y*8 + (my>>1))*s->uvlinesize;
       
        if(s->no_rounding){
            s->dsp.put_no_rnd_pixels_tab[1][dxy](c->scratchpad    , s->last_picture.data[1] + offset, s->uvlinesize, 8);
            s->dsp.put_no_rnd_pixels_tab[1][dxy](c->scratchpad+8  , s->last_picture.data[2] + offset, s->uvlinesize, 8);
        }else{
            s->dsp.put_pixels_tab       [1][dxy](c->scratchpad    , s->last_picture.data[1] + offset, s->uvlinesize, 8);
            s->dsp.put_pixels_tab       [1][dxy](c->scratchpad+8  , s->last_picture.data[2] + offset, s->uvlinesize, 8);
        }

        dmin_sum += s->dsp.mb_cmp[1](s, s->new_picture.data[1] + s->mb_x*8 + s->mb_y*8*s->uvlinesize, c->scratchpad  , s->uvlinesize, 8);
        dmin_sum += s->dsp.mb_cmp[1](s, s->new_picture.data[2] + s->mb_x*8 + s->mb_y*8*s->uvlinesize, c->scratchpad+8, s->uvlinesize, 8);
    }
    
    c->pred_x= mx;
    c->pred_y= my;

    switch(c->avctx->mb_cmp&0xFF){
    /*case FF_CMP_SSE:
        return dmin_sum+ 32*s->qscale*s->qscale;*/
    case FF_CMP_RD:
        return dmin_sum;
    default:
        return dmin_sum+ 11*c->mb_penalty_factor;
    }
}

static __inline void init_interlaced_ref(MpegEncContext *s, int ref_index){
    MotionEstContext * const c= &s->me;

    c->ref[1+ref_index][0] = c->ref[0+ref_index][0] + s->linesize;
    c->src[1][0] = c->src[0][0] + s->linesize;
    if(c->flags & FLAG_CHROMA){
        c->ref[1+ref_index][1] = c->ref[0+ref_index][1] + s->uvlinesize;
        c->ref[1+ref_index][2] = c->ref[0+ref_index][2] + s->uvlinesize;
        c->src[1][1] = c->src[0][1] + s->uvlinesize;
        c->src[1][2] = c->src[0][2] + s->uvlinesize;
    }
}

static int interlaced_search(MpegEncContext *s, int ref_index, 
                             int16_t (*mv_tables[2][2])[2], uint8_t *field_select_tables[2], int mx, int my, int user_field_select)
{
    MotionEstContext * const c= &s->me;
    const int size=0;
    const int h=8;
    int block;
    int P[10][2];
    uint8_t * const mv_penalty= c->current_mv_penalty;
    int same=1;
    const int stride= 2*s->linesize;
    const int uvstride= 2*s->uvlinesize;
    int dmin_sum= 0;
    const int mot_stride= s->mb_stride;
    const int xy= s->mb_x + s->mb_y*mot_stride;
    
    c->ymin>>=1;
    c->ymax>>=1;
    c->stride<<=1;
    c->uvstride<<=1;
    init_interlaced_ref(s, ref_index);
    
    for(block=0; block<2; block++){
        int field_select;
        int best_dmin= INT_MAX;
        int best_field= -1;

        for(field_select=0; field_select<2; field_select++){
            int dmin, mx_i, my_i;
            int16_t (*mv_table)[2]= mv_tables[block][field_select];
            
            if(user_field_select){
                if(field_select_tables[block][xy] != field_select)
                    continue;
            }
            
            P_LEFT[0] = mv_table[xy - 1][0];
            P_LEFT[1] = mv_table[xy - 1][1];
            if(P_LEFT[0]       > (c->xmax<<1)) P_LEFT[0]       = (c->xmax<<1);
            
            c->pred_x= P_LEFT[0];
            c->pred_y= P_LEFT[1];
            
            if(!s->first_slice_line){
                P_TOP[0]      = mv_table[xy - mot_stride][0];
                P_TOP[1]      = mv_table[xy - mot_stride][1];
                P_TOPRIGHT[0] = mv_table[xy - mot_stride + 1][0];
                P_TOPRIGHT[1] = mv_table[xy - mot_stride + 1][1];
                if(P_TOP[1]      > (c->ymax<<1)) P_TOP[1]     = (c->ymax<<1);
                if(P_TOPRIGHT[0] < (c->xmin<<1)) P_TOPRIGHT[0]= (c->xmin<<1);
                if(P_TOPRIGHT[0] > (c->xmax<<1)) P_TOPRIGHT[0]= (c->xmax<<1);
                if(P_TOPRIGHT[1] > (c->ymax<<1)) P_TOPRIGHT[1]= (c->ymax<<1);
    
                P_MEDIAN[0]= mid_pred(P_LEFT[0], P_TOP[0], P_TOPRIGHT[0]);
                P_MEDIAN[1]= mid_pred(P_LEFT[1], P_TOP[1], P_TOPRIGHT[1]);
            }
            P_MV1[0]= mx; //FIXME not correct if block != field_select
            P_MV1[1]= my / 2;
            
            dmin = epzs_motion_search2(s, &mx_i, &my_i, P, block, field_select+ref_index, mv_table, (1<<16)>>1);

            dmin= c->sub_motion_search(s, &mx_i, &my_i, dmin, block, field_select+ref_index, size, h);
            
            mv_table[xy][0]= mx_i;
            mv_table[xy][1]= my_i;
            
            if(s->dsp.me_sub_cmp[0] != s->dsp.mb_cmp[0]){
                int dxy;

                //FIXME chroma ME
                uint8_t *ref= c->ref[field_select+ref_index][0] + (mx_i>>1) + (my_i>>1)*stride;
                dxy = ((my_i & 1) << 1) | (mx_i & 1);

                if(s->no_rounding){
                    s->dsp.put_no_rnd_pixels_tab[size][dxy](c->scratchpad, ref    , stride, h);
                }else{
                    s->dsp.put_pixels_tab       [size][dxy](c->scratchpad, ref    , stride, h);
                }
                dmin= s->dsp.mb_cmp[size](s, c->src[block][0], c->scratchpad, stride, h);
                dmin+= (mv_penalty[mx_i-c->pred_x] + mv_penalty[my_i-c->pred_y] + 1)*c->mb_penalty_factor;
            }else
                dmin+= c->mb_penalty_factor; //field_select bits
                
            dmin += field_select != block; //slightly prefer same field
            
            if(dmin < best_dmin){
                best_dmin= dmin;
                best_field= field_select;
            }
        }
        {
            int16_t (*mv_table)[2]= mv_tables[block][best_field];

            if(mv_table[xy][0] != mx) same=0; //FIXME check if these checks work and are any good at all
            if(mv_table[xy][1]&1) same=0;
            if(mv_table[xy][1]*2 != my) same=0; 
            if(best_field != block) same=0;
        }

        field_select_tables[block][xy]= best_field;
        dmin_sum += best_dmin;
    }
    
    c->ymin<<=1;
    c->ymax<<=1;
    c->stride>>=1;
    c->uvstride>>=1;

    if(same)
        return INT_MAX;
    
    switch(c->avctx->mb_cmp&0xFF){
    /*case FF_CMP_SSE:
        return dmin_sum+ 32*s->qscale*s->qscale;*/
    case FF_CMP_RD:
        return dmin_sum;
    default:
        return dmin_sum+ 11*c->mb_penalty_factor;
    }
}

static void clip_input_mv(MpegEncContext * s, int16_t *mv, int interlaced){
    int ymax= s->me.ymax>>interlaced;
    int ymin= s->me.ymin>>interlaced;
    
    if(mv[0] < s->me.xmin) mv[0] = s->me.xmin;
    if(mv[0] > s->me.xmax) mv[0] = s->me.xmax;
    if(mv[1] <       ymin) mv[1] =       ymin;
    if(mv[1] >       ymax) mv[1] =       ymax;
}

static __inline int check_input_motion(MpegEncContext * s, int mb_x, int mb_y, int p_type){
    MotionEstContext * const c= &s->me;
    Picture *p= s->current_picture_ptr;
    int mb_xy= mb_x + mb_y*s->mb_stride;
    int xy= 2*mb_x + 2*mb_y*s->b8_stride;
    int mb_type= s->current_picture.mb_type[mb_xy];
    int flags= c->flags;
    int shift= (flags&FLAG_QPEL) + 1;
    int mask= (1<<shift)-1;
    int x, y, i;
    int d=0;
    me_cmp_func cmpf= s->dsp.sse[0];
    me_cmp_func chroma_cmpf= s->dsp.sse[1];
    
    if(p_type && USES_LIST(mb_type, 1)){
        av_log(c->avctx, AV_LOG_ERROR, "backward motion vector in P frame\n");
        return INT_MAX;
    }
    assert(IS_INTRA(mb_type) || USES_LIST(mb_type,0) || USES_LIST(mb_type,1));
    
    for(i=0; i<4; i++){
        int xy= s->block_index[i];
        clip_input_mv(s, p->motion_val[0][xy], !!IS_INTERLACED(mb_type));
        clip_input_mv(s, p->motion_val[1][xy], !!IS_INTERLACED(mb_type));
    }

    if(IS_INTERLACED(mb_type)){
        int xy2= xy  + s->b8_stride;
        s->mb_type[mb_xy]=CANDIDATE_MB_TYPE_INTRA;
        c->stride<<=1;
        c->uvstride<<=1;
        
        if(!(s->flags & CODEC_FLAG_INTERLACED_ME)){
            av_log(c->avctx, AV_LOG_ERROR, "Interlaced macroblock selected but interlaced motion estimation disabled\n");
            return INT_MAX;
        }

        if(USES_LIST(mb_type, 0)){
            int field_select0= p->ref_index[0][xy ];
            int field_select1= p->ref_index[0][xy2];
            assert(field_select0==0 ||field_select0==1);
            assert(field_select1==0 ||field_select1==1);
            init_interlaced_ref(s, 0);

            if(p_type){
                s->p_field_select_table[0][mb_xy]= field_select0;
                s->p_field_select_table[1][mb_xy]= field_select1;
                *(uint32_t*)s->p_field_mv_table[0][field_select0][mb_xy]= *(uint32_t*)p->motion_val[0][xy ];
                *(uint32_t*)s->p_field_mv_table[1][field_select1][mb_xy]= *(uint32_t*)p->motion_val[0][xy2];
                s->mb_type[mb_xy]=CANDIDATE_MB_TYPE_INTER_I;
            }else{
                s->b_field_select_table[0][0][mb_xy]= field_select0;
                s->b_field_select_table[0][1][mb_xy]= field_select1;
                *(uint32_t*)s->b_field_mv_table[0][0][field_select0][mb_xy]= *(uint32_t*)p->motion_val[0][xy ];
                *(uint32_t*)s->b_field_mv_table[0][1][field_select1][mb_xy]= *(uint32_t*)p->motion_val[0][xy2];
                s->mb_type[mb_xy]= CANDIDATE_MB_TYPE_FORWARD_I;
            }

            x= p->motion_val[0][xy ][0]; 
            y= p->motion_val[0][xy ][1];
            d = cmp(s, x>>shift, y>>shift, x&mask, y&mask, 0, 8, field_select0, 0, cmpf, chroma_cmpf, flags);
            x= p->motion_val[0][xy2][0]; 
            y= p->motion_val[0][xy2][1];
            d+= cmp(s, x>>shift, y>>shift, x&mask, y&mask, 0, 8, field_select1, 1, cmpf, chroma_cmpf, flags);
        }
        if(USES_LIST(mb_type, 1)){
            int field_select0= p->ref_index[1][xy ];
            int field_select1= p->ref_index[1][xy2];
            assert(field_select0==0 ||field_select0==1);
            assert(field_select1==0 ||field_select1==1);
            init_interlaced_ref(s, 2);

            s->b_field_select_table[1][0][mb_xy]= field_select0;
            s->b_field_select_table[1][1][mb_xy]= field_select1;
            *(uint32_t*)s->b_field_mv_table[1][0][field_select0][mb_xy]= *(uint32_t*)p->motion_val[1][xy ];
            *(uint32_t*)s->b_field_mv_table[1][1][field_select1][mb_xy]= *(uint32_t*)p->motion_val[1][xy2];
            if(USES_LIST(mb_type, 0)){
                s->mb_type[mb_xy]= CANDIDATE_MB_TYPE_BIDIR_I;
            }else{
                s->mb_type[mb_xy]= CANDIDATE_MB_TYPE_BACKWARD_I;
            }

            x= p->motion_val[1][xy ][0]; 
            y= p->motion_val[1][xy ][1];
            d = cmp(s, x>>shift, y>>shift, x&mask, y&mask, 0, 8, field_select0+2, 0, cmpf, chroma_cmpf, flags);
            x= p->motion_val[1][xy2][0]; 
            y= p->motion_val[1][xy2][1];
            d+= cmp(s, x>>shift, y>>shift, x&mask, y&mask, 0, 8, field_select1+2, 1, cmpf, chroma_cmpf, flags);
            //FIXME bidir scores
        }
        c->stride>>=1;
        c->uvstride>>=1;
    }else if(IS_8X8(mb_type)){
        if(!(s->flags & CODEC_FLAG_4MV)){
            av_log(c->avctx, AV_LOG_ERROR, "4MV macroblock selected but 4MV encoding disabled\n");
            return INT_MAX;
        }
        cmpf= s->dsp.sse[1];
        chroma_cmpf= s->dsp.sse[1];
        init_mv4_ref(c);
        for(i=0; i<4; i++){
            xy= s->block_index[i];
            x= p->motion_val[0][xy][0]; 
            y= p->motion_val[0][xy][1];
            d+= cmp(s, x>>shift, y>>shift, x&mask, y&mask, 1, 8, i, i, cmpf, chroma_cmpf, flags);
        }
        s->mb_type[mb_xy]=CANDIDATE_MB_TYPE_INTER4V;
    }else{
        if(USES_LIST(mb_type, 0)){
            if(p_type){
                *(uint32_t*)s->p_mv_table[mb_xy]= *(uint32_t*)p->motion_val[0][xy];
                s->mb_type[mb_xy]=CANDIDATE_MB_TYPE_INTER;
            }else if(USES_LIST(mb_type, 1)){
                *(uint32_t*)s->b_bidir_forw_mv_table[mb_xy]= *(uint32_t*)p->motion_val[0][xy];
                *(uint32_t*)s->b_bidir_back_mv_table[mb_xy]= *(uint32_t*)p->motion_val[1][xy];
                s->mb_type[mb_xy]=CANDIDATE_MB_TYPE_BIDIR;
            }else{
                *(uint32_t*)s->b_forw_mv_table[mb_xy]= *(uint32_t*)p->motion_val[0][xy];
                s->mb_type[mb_xy]=CANDIDATE_MB_TYPE_FORWARD;
            }
            x= p->motion_val[0][xy][0]; 
            y= p->motion_val[0][xy][1];
            d = cmp(s, x>>shift, y>>shift, x&mask, y&mask, 0, 16, 0, 0, cmpf, chroma_cmpf, flags);
        }else if(USES_LIST(mb_type, 1)){
            *(uint32_t*)s->b_back_mv_table[mb_xy]= *(uint32_t*)p->motion_val[1][xy];
            s->mb_type[mb_xy]=CANDIDATE_MB_TYPE_BACKWARD;
           
            x= p->motion_val[1][xy][0]; 
            y= p->motion_val[1][xy][1];
            d = cmp(s, x>>shift, y>>shift, x&mask, y&mask, 0, 16, 2, 0, cmpf, chroma_cmpf, flags);
        }else
            s->mb_type[mb_xy]=CANDIDATE_MB_TYPE_INTRA;
    }
    return d;
}

void ff_estimate_p_frame_motion(MpegEncContext * s,
                                int mb_x, int mb_y)
{
    MotionEstContext * const c= &s->me;
    uint8_t *pix, *ppix;
    int sum, varc, vard, mx, my, dmin;
    int P[10][2];
    const int shift= 1+s->quarter_sample;
    int mb_type=0;
    Picture * const pic= &s->current_picture;
    
    init_ref(c, s->new_picture.data, s->last_picture.data, NULL, 16*mb_x, 16*mb_y, 0);

    assert(s->quarter_sample==0 || s->quarter_sample==1);
    assert(s->linesize == c->stride);
    assert(s->uvlinesize == c->uvstride);

    c->penalty_factor    = get_penalty_factor(s, c->avctx->me_cmp);
    c->sub_penalty_factor= get_penalty_factor(s, c->avctx->me_sub_cmp);
    c->mb_penalty_factor = get_penalty_factor(s, c->avctx->mb_cmp);
    c->current_mv_penalty= c->mv_penalty[s->f_code] + MAX_MV;

    get_limits(s, 16*mb_x, 16*mb_y);
    c->skip=0;

    /* intra / predictive decision */
    pix = c->src[0][0];
    sum = s->dsp.pix_sum(pix, s->linesize);
    varc = (s->dsp.pix_norm1(pix, s->linesize) - (((unsigned)(sum*sum))>>8) + 500 + 128)>>8;

    pic->mb_mean[s->mb_stride * mb_y + mb_x] = (sum+128)>>8;
    pic->mb_var [s->mb_stride * mb_y + mb_x] = varc;
    c->mb_var_sum_temp += varc;

    if(c->avctx->me_threshold){
        vard= (check_input_motion(s, mb_x, mb_y, 1)+128)>>8;
        
        if(vard<c->avctx->me_threshold){
            pic->mc_mb_var[s->mb_stride * mb_y + mb_x] = vard;
            c->mc_mb_var_sum_temp += vard;
            if (vard <= 64 || vard < varc) { //FIXME
                c->scene_change_score+= ff_sqrt(vard) - ff_sqrt(varc);
            }else{
                c->scene_change_score+= s->qscale;
            }
            return;
        }
        if(vard<c->avctx->mb_threshold)
            mb_type= s->mb_type[mb_x + mb_y*s->mb_stride];
    }

    switch(s->me_method) {
    case ME_ZERO:
    default:
	no_motion_search(s, &mx, &my);
        mx-= mb_x*16;
        my-= mb_y*16;
        dmin = 0;
        break;
#if 0
    case ME_FULL:
	dmin = full_motion_search(s, &mx, &my, range, ref_picture);
        mx-= mb_x*16;
        my-= mb_y*16;
        break;
    case ME_LOG:
	dmin = log_motion_search(s, &mx, &my, range / 2, ref_picture);
        mx-= mb_x*16;
        my-= mb_y*16;
        break;
    case ME_PHODS:
	dmin = phods_motion_search(s, &mx, &my, range / 2, ref_picture);
        mx-= mb_x*16;
        my-= mb_y*16;
        break;
#endif
    case ME_X1:
    case ME_EPZS:
       {
            const int mot_stride = s->b8_stride;
            const int mot_xy = s->block_index[0];

            P_LEFT[0]       = s->current_picture.motion_val[0][mot_xy - 1][0];
            P_LEFT[1]       = s->current_picture.motion_val[0][mot_xy - 1][1];

            if(P_LEFT[0]       > (c->xmax<<shift)) P_LEFT[0]       = (c->xmax<<shift);

            if(!s->first_slice_line) {
                P_TOP[0]      = s->current_picture.motion_val[0][mot_xy - mot_stride    ][0];
                P_TOP[1]      = s->current_picture.motion_val[0][mot_xy - mot_stride    ][1];
                P_TOPRIGHT[0] = s->current_picture.motion_val[0][mot_xy - mot_stride + 2][0];
                P_TOPRIGHT[1] = s->current_picture.motion_val[0][mot_xy - mot_stride + 2][1];
                if(P_TOP[1]      > (c->ymax<<shift)) P_TOP[1]     = (c->ymax<<shift);
                if(P_TOPRIGHT[0] < (c->xmin<<shift)) P_TOPRIGHT[0]= (c->xmin<<shift);
                if(P_TOPRIGHT[1] > (c->ymax<<shift)) P_TOPRIGHT[1]= (c->ymax<<shift);
        
                P_MEDIAN[0]= mid_pred(P_LEFT[0], P_TOP[0], P_TOPRIGHT[0]);
                P_MEDIAN[1]= mid_pred(P_LEFT[1], P_TOP[1], P_TOPRIGHT[1]);

                if(s->out_format == FMT_H263){
                    c->pred_x = P_MEDIAN[0];
                    c->pred_y = P_MEDIAN[1];
                }else { /* mpeg1 at least */
                    c->pred_x= P_LEFT[0];
                    c->pred_y= P_LEFT[1];
                }
            }else{
                c->pred_x= P_LEFT[0];
                c->pred_y= P_LEFT[1];
            }

        }
        dmin = epzs_motion_search(s, &mx, &my, P, 0, 0, s->p_mv_table, (1<<16)>>shift);       

        break;
    }

    /* At this point (mx,my) are full-pell and the relative displacement */
    ppix = c->ref[0][0] + (my * s->linesize) + mx;
        
    vard = (s->dsp.sse[0](NULL, pix, ppix, s->linesize, 16)+128)>>8;

    pic->mc_mb_var[s->mb_stride * mb_y + mb_x] = vard;
//    pic->mb_cmp_score[s->mb_stride * mb_y + mb_x] = dmin; 
    c->mc_mb_var_sum_temp += vard;
    
#if 0
    printf("varc=%4d avg_var=%4d (sum=%4d) vard=%4d mx=%2d my=%2d\n",
	   varc, s->avg_mb_var, sum, vard, mx - xx, my - yy);
#endif
    if(mb_type){
        if (vard <= 64 || vard < varc)
            c->scene_change_score+= ff_sqrt(vard) - ff_sqrt(varc);
        else
            c->scene_change_score+= s->qscale;

        if(mb_type == CANDIDATE_MB_TYPE_INTER){
            c->sub_motion_search(s, &mx, &my, dmin, 0, 0, 0, 16);
            set_p_mv_tables(s, mx, my, 1);
        }else{
            mx <<=shift;
            my <<=shift;
        }
        if(mb_type == CANDIDATE_MB_TYPE_INTER4V){
            h263_mv4_search(s, mx, my, shift);

            set_p_mv_tables(s, mx, my, 0);
        }
        if(mb_type == CANDIDATE_MB_TYPE_INTER_I){
            interlaced_search(s, 0, s->p_field_mv_table, s->p_field_select_table, mx, my, 1);
        }
    }else if(c->avctx->mb_decision > FF_MB_DECISION_SIMPLE){
        if (vard <= 64 || vard < varc)
            c->scene_change_score+= ff_sqrt(vard) - ff_sqrt(varc);
        else
            c->scene_change_score+= s->qscale;

        if (vard*2 + 200 > varc)
            mb_type|= CANDIDATE_MB_TYPE_INTRA;
        if (varc*2 + 200 > vard){
            mb_type|= CANDIDATE_MB_TYPE_INTER;
            c->sub_motion_search(s, &mx, &my, dmin, 0, 0, 0, 16);
            if(s->flags&CODEC_FLAG_MV0)
                if(mx || my)
                    mb_type |= CANDIDATE_MB_TYPE_SKIPED; //FIXME check difference
        }else{
            mx <<=shift;
            my <<=shift;
        }
        if((s->flags&CODEC_FLAG_4MV)
           && !c->skip && varc>50 && vard>10){
            if(h263_mv4_search(s, mx, my, shift) < INT_MAX)
                mb_type|=CANDIDATE_MB_TYPE_INTER4V;

            set_p_mv_tables(s, mx, my, 0);
        }else
            set_p_mv_tables(s, mx, my, 1);
        if((s->flags&CODEC_FLAG_INTERLACED_ME)
           && !c->skip){ //FIXME varc/d checks
            if(interlaced_search(s, 0, s->p_field_mv_table, s->p_field_select_table, mx, my, 0) < INT_MAX)
                mb_type |= CANDIDATE_MB_TYPE_INTER_I;
        }
    }else{
        int intra_score, i;
        mb_type= CANDIDATE_MB_TYPE_INTER;

        dmin= c->sub_motion_search(s, &mx, &my, dmin, 0, 0, 0, 16);
        if(c->avctx->me_sub_cmp != c->avctx->mb_cmp && !c->skip)
            dmin= get_mb_score(s, mx, my, 0, 0);

        if((s->flags&CODEC_FLAG_4MV)
           && !c->skip && varc>50 && vard>10){
            int dmin4= h263_mv4_search(s, mx, my, shift);
            if(dmin4 < dmin){
                mb_type= CANDIDATE_MB_TYPE_INTER4V;
                dmin=dmin4;
            }
        }
        if((s->flags&CODEC_FLAG_INTERLACED_ME)
           && !c->skip){ //FIXME varc/d checks
            int dmin_i= interlaced_search(s, 0, s->p_field_mv_table, s->p_field_select_table, mx, my, 0);
            if(dmin_i < dmin){
                mb_type = CANDIDATE_MB_TYPE_INTER_I;
                dmin= dmin_i;
            }
        }
                
//        pic->mb_cmp_score[s->mb_stride * mb_y + mb_x] = dmin; 
        set_p_mv_tables(s, mx, my, mb_type!=CANDIDATE_MB_TYPE_INTER4V);

        /* get intra luma score */
        if((c->avctx->mb_cmp&0xFF)==FF_CMP_SSE){
            intra_score= (varc<<8) - 500; //FIXME dont scale it down so we dont have to fix it
        }else{
            int mean= (sum+128)>>8;
            mean*= 0x01010101;
            
            for(i=0; i<16; i++){
                *(uint32_t*)(&c->scratchpad[i*s->linesize+ 0]) = mean;
                *(uint32_t*)(&c->scratchpad[i*s->linesize+ 4]) = mean;
                *(uint32_t*)(&c->scratchpad[i*s->linesize+ 8]) = mean;
                *(uint32_t*)(&c->scratchpad[i*s->linesize+12]) = mean;
            }

            intra_score= s->dsp.mb_cmp[0](s, c->scratchpad, pix, s->linesize, 16);
        }
#if 0 //FIXME
        /* get chroma score */
        if(c->avctx->mb_cmp&FF_CMP_CHROMA){
            for(i=1; i<3; i++){
                uint8_t *dest_c;
                int mean;
                
                if(s->out_format == FMT_H263){
                    mean= (s->dc_val[i][mb_x + mb_y*s->b8_stride] + 4)>>3; //FIXME not exact but simple ;)
                }else{
                    mean= (s->last_dc[i] + 4)>>3;
                }
                dest_c = s->new_picture.data[i] + (mb_y * 8  * (s->uvlinesize)) + mb_x * 8;
                
                mean*= 0x01010101;
                for(i=0; i<8; i++){
                    *(uint32_t*)(&c->scratchpad[i*s->uvlinesize+ 0]) = mean;
                    *(uint32_t*)(&c->scratchpad[i*s->uvlinesize+ 4]) = mean;
                }
                
                intra_score+= s->dsp.mb_cmp[1](s, c->scratchpad, dest_c, s->uvlinesize);
            }                
        }
#endif
        intra_score += c->mb_penalty_factor*16;
        
        if(intra_score < dmin){
            mb_type= CANDIDATE_MB_TYPE_INTRA;
            s->current_picture.mb_type[mb_y*s->mb_stride + mb_x]= CANDIDATE_MB_TYPE_INTRA; //FIXME cleanup
        }else
            s->current_picture.mb_type[mb_y*s->mb_stride + mb_x]= 0;
        
        if (vard <= 64 || vard < varc) { //FIXME
            c->scene_change_score+= ff_sqrt(vard) - ff_sqrt(varc);
        }else{
            c->scene_change_score+= s->qscale;
        }
    }

    s->mb_type[mb_y*s->mb_stride + mb_x]= mb_type;
}

int ff_pre_estimate_p_frame_motion(MpegEncContext * s,
                                    int mb_x, int mb_y)
{
    MotionEstContext * const c= &s->me;
    int mx, my, dmin;
    int P[10][2];
    const int shift= 1+s->quarter_sample;
    const int xy= mb_x + mb_y*s->mb_stride;
    init_ref(c, s->new_picture.data, s->last_picture.data, NULL, 16*mb_x, 16*mb_y, 0);
    
    assert(s->quarter_sample==0 || s->quarter_sample==1);

    c->pre_penalty_factor    = get_penalty_factor(s, c->avctx->me_pre_cmp);
    c->current_mv_penalty= c->mv_penalty[s->f_code] + MAX_MV;

    get_limits(s, 16*mb_x, 16*mb_y);
    c->skip=0;

    P_LEFT[0]       = s->p_mv_table[xy + 1][0];
    P_LEFT[1]       = s->p_mv_table[xy + 1][1];

    if(P_LEFT[0]       < (c->xmin<<shift)) P_LEFT[0]       = (c->xmin<<shift);

    /* special case for first line */
    if (s->first_slice_line) {
        c->pred_x= P_LEFT[0];
        c->pred_y= P_LEFT[1];
        P_TOP[0]= P_TOPRIGHT[0]= P_MEDIAN[0]=
        P_TOP[1]= P_TOPRIGHT[1]= P_MEDIAN[1]= 0; //FIXME 
    } else {
        P_TOP[0]      = s->p_mv_table[xy + s->mb_stride    ][0];
        P_TOP[1]      = s->p_mv_table[xy + s->mb_stride    ][1];
        P_TOPRIGHT[0] = s->p_mv_table[xy + s->mb_stride - 1][0];
        P_TOPRIGHT[1] = s->p_mv_table[xy + s->mb_stride - 1][1];
        if(P_TOP[1]      < (c->ymin<<shift)) P_TOP[1]     = (c->ymin<<shift);
        if(P_TOPRIGHT[0] > (c->xmax<<shift)) P_TOPRIGHT[0]= (c->xmax<<shift);
        if(P_TOPRIGHT[1] < (c->ymin<<shift)) P_TOPRIGHT[1]= (c->ymin<<shift);
    
        P_MEDIAN[0]= mid_pred(P_LEFT[0], P_TOP[0], P_TOPRIGHT[0]);
        P_MEDIAN[1]= mid_pred(P_LEFT[1], P_TOP[1], P_TOPRIGHT[1]);

        c->pred_x = P_MEDIAN[0];
        c->pred_y = P_MEDIAN[1];
    }

    dmin = epzs_motion_search(s, &mx, &my, P, 0, 0, s->p_mv_table, (1<<16)>>shift);       

    s->p_mv_table[xy][0] = mx<<shift;
    s->p_mv_table[xy][1] = my<<shift;
    
    return dmin;
}

static int ff_estimate_motion_b(MpegEncContext * s,
                       int mb_x, int mb_y, int16_t (*mv_table)[2], int ref_index, int f_code)
{
    MotionEstContext * const c= &s->me;
    int mx, my, dmin;
    int P[10][2];
    const int shift= 1+s->quarter_sample;
    const int mot_stride = s->mb_stride;
    const int mot_xy = mb_y*mot_stride + mb_x;
    uint8_t * const mv_penalty= c->mv_penalty[f_code] + MAX_MV;
    int mv_scale;
        
    c->penalty_factor    = get_penalty_factor(s, c->avctx->me_cmp);
    c->sub_penalty_factor= get_penalty_factor(s, c->avctx->me_sub_cmp);
    c->mb_penalty_factor = get_penalty_factor(s, c->avctx->mb_cmp);
    c->current_mv_penalty= mv_penalty;

    get_limits(s, 16*mb_x, 16*mb_y);

    switch(s->me_method) {
    case ME_ZERO:
    default:
	no_motion_search(s, &mx, &my);
        dmin = 0;
        mx-= mb_x*16;
        my-= mb_y*16;
        break;
#if 0
    case ME_FULL:
	dmin = full_motion_search(s, &mx, &my, range, ref_picture);
        mx-= mb_x*16;
        my-= mb_y*16;
        break;
    case ME_LOG:
	dmin = log_motion_search(s, &mx, &my, range / 2, ref_picture);
        mx-= mb_x*16;
        my-= mb_y*16;
        break;
    case ME_PHODS:
	dmin = phods_motion_search(s, &mx, &my, range / 2, ref_picture);
        mx-= mb_x*16;
        my-= mb_y*16;
        break;
#endif
    case ME_X1:
    case ME_EPZS:
       {
            P_LEFT[0]        = mv_table[mot_xy - 1][0];
            P_LEFT[1]        = mv_table[mot_xy - 1][1];

            if(P_LEFT[0]       > (c->xmax<<shift)) P_LEFT[0]       = (c->xmax<<shift);

            /* special case for first line */
            if (!s->first_slice_line) {
                P_TOP[0] = mv_table[mot_xy - mot_stride             ][0];
                P_TOP[1] = mv_table[mot_xy - mot_stride             ][1];
                P_TOPRIGHT[0] = mv_table[mot_xy - mot_stride + 1         ][0];
                P_TOPRIGHT[1] = mv_table[mot_xy - mot_stride + 1         ][1];
                if(P_TOP[1] > (c->ymax<<shift)) P_TOP[1]= (c->ymax<<shift);
                if(P_TOPRIGHT[0] < (c->xmin<<shift)) P_TOPRIGHT[0]= (c->xmin<<shift);
                if(P_TOPRIGHT[1] > (c->ymax<<shift)) P_TOPRIGHT[1]= (c->ymax<<shift);
        
                P_MEDIAN[0]= mid_pred(P_LEFT[0], P_TOP[0], P_TOPRIGHT[0]);
                P_MEDIAN[1]= mid_pred(P_LEFT[1], P_TOP[1], P_TOPRIGHT[1]);
            }
            c->pred_x= P_LEFT[0];
            c->pred_y= P_LEFT[1];
        }
        
        if(mv_table == s->b_forw_mv_table){
            mv_scale= (s->pb_time<<16) / (s->pp_time<<shift);
        }else{
            mv_scale= ((s->pb_time - s->pp_time)<<16) / (s->pp_time<<shift);
        }
        
        dmin = epzs_motion_search(s, &mx, &my, P, 0, ref_index, s->p_mv_table, mv_scale);
 
        break;
    }
    
    dmin= c->sub_motion_search(s, &mx, &my, dmin, 0, ref_index, 0, 16);
                                   
    if(c->avctx->me_sub_cmp != c->avctx->mb_cmp && !c->skip)
        dmin= get_mb_score(s, mx, my, 0, ref_index);

//printf("%d %d %d %d//", s->mb_x, s->mb_y, mx, my);
//    s->mb_type[mb_y*s->mb_width + mb_x]= mb_type;
    mv_table[mot_xy][0]= mx;
    mv_table[mot_xy][1]= my;

    return dmin;
}

static __inline int check_bidir_mv(MpegEncContext * s,
                   int motion_fx, int motion_fy,
                   int motion_bx, int motion_by,
                   int pred_fx, int pred_fy,
                   int pred_bx, int pred_by,
                   int size, int h)
{
    //FIXME optimize?
    //FIXME better f_code prediction (max mv & distance)
    //FIXME pointers
    MotionEstContext * const c= &s->me;
    uint8_t * const mv_penalty= c->mv_penalty[s->f_code] + MAX_MV; // f_code of the prev frame
    int stride= c->stride;
    int uvstride= c->uvstride;
    uint8_t *dest_y = c->scratchpad;
    uint8_t *ptr;
    int dxy;
    int src_x, src_y;
    int fbmin;
    uint8_t **src_data= c->src[0];
    uint8_t **ref_data= c->ref[0];
    uint8_t **ref2_data= c->ref[2];

    if(s->quarter_sample){
        dxy = ((motion_fy & 3) << 2) | (motion_fx & 3);
        src_x = motion_fx >> 2;
        src_y = motion_fy >> 2;

        ptr = ref_data[0] + (src_y * stride) + src_x;
        s->dsp.put_qpel_pixels_tab[0][dxy](dest_y    , ptr    , stride);

        dxy = ((motion_by & 3) << 2) | (motion_bx & 3);
        src_x = motion_bx >> 2;
        src_y = motion_by >> 2;
    
        ptr = ref2_data[0] + (src_y * stride) + src_x;
        s->dsp.avg_qpel_pixels_tab[size][dxy](dest_y    , ptr    , stride);
    }else{
        dxy = ((motion_fy & 1) << 1) | (motion_fx & 1);
        src_x = motion_fx >> 1;
        src_y = motion_fy >> 1;

        ptr = ref_data[0] + (src_y * stride) + src_x;
        s->dsp.put_pixels_tab[size][dxy](dest_y    , ptr    , stride, h);

        dxy = ((motion_by & 1) << 1) | (motion_bx & 1);
        src_x = motion_bx >> 1;
        src_y = motion_by >> 1;
    
        ptr = ref2_data[0] + (src_y * stride) + src_x;
        s->dsp.avg_pixels_tab[size][dxy](dest_y    , ptr    , stride, h);
    }

    fbmin = (mv_penalty[motion_fx-pred_fx] + mv_penalty[motion_fy-pred_fy])*c->mb_penalty_factor
           +(mv_penalty[motion_bx-pred_bx] + mv_penalty[motion_by-pred_by])*c->mb_penalty_factor
           + s->dsp.mb_cmp[size](s, src_data[0], dest_y, stride, h); //FIXME new_pic
           
    if(c->avctx->mb_cmp&FF_CMP_CHROMA){
    }
    //FIXME CHROMA !!!
           
    return fbmin;
}

/* refine the bidir vectors in hq mode and return the score in both lq & hq mode*/
static __inline int bidir_refine(MpegEncContext * s, int mb_x, int mb_y)
{
    const int mot_stride = s->mb_stride;
    const int xy = mb_y *mot_stride + mb_x;
    int fbmin;
    int pred_fx= s->b_bidir_forw_mv_table[xy-1][0];
    int pred_fy= s->b_bidir_forw_mv_table[xy-1][1];
    int pred_bx= s->b_bidir_back_mv_table[xy-1][0];
    int pred_by= s->b_bidir_back_mv_table[xy-1][1];
    int motion_fx= s->b_bidir_forw_mv_table[xy][0]= s->b_forw_mv_table[xy][0];
    int motion_fy= s->b_bidir_forw_mv_table[xy][1]= s->b_forw_mv_table[xy][1];
    int motion_bx= s->b_bidir_back_mv_table[xy][0]= s->b_back_mv_table[xy][0];
    int motion_by= s->b_bidir_back_mv_table[xy][1]= s->b_back_mv_table[xy][1];

    //FIXME do refinement and add flag
    
    fbmin= check_bidir_mv(s, motion_fx, motion_fy,
                          motion_bx, motion_by,
                          pred_fx, pred_fy,
                          pred_bx, pred_by,
                          0, 16);

   return fbmin;
}

static __inline int direct_search(MpegEncContext * s, int mb_x, int mb_y)
{
    MotionEstContext * const c= &s->me;
    int P[10][2];
    const int mot_stride = s->mb_stride;
    const int mot_xy = mb_y*mot_stride + mb_x;
    const int shift= 1+s->quarter_sample;
    int dmin, i;
    const int time_pp= s->pp_time;
    const int time_pb= s->pb_time;
    int mx, my, xmin, xmax, ymin, ymax;
    int16_t (*mv_table)[2]= s->b_direct_mv_table;
    
    c->current_mv_penalty= c->mv_penalty[1] + MAX_MV;
    ymin= xmin=(-32)>>shift;
    ymax= xmax=   31>>shift;

    if(IS_8X8(s->next_picture.mb_type[mot_xy])){
        s->mv_type= MV_TYPE_8X8;
    }else{
        s->mv_type= MV_TYPE_16X16;
    }

    for(i=0; i<4; i++){
        int index= s->block_index[i];
        int min, max;
    
        c->co_located_mv[i][0]= s->next_picture.motion_val[0][index][0];
        c->co_located_mv[i][1]= s->next_picture.motion_val[0][index][1];
        c->direct_basis_mv[i][0]= c->co_located_mv[i][0]*time_pb/time_pp + ((i& 1)<<(shift+3));
        c->direct_basis_mv[i][1]= c->co_located_mv[i][1]*time_pb/time_pp + ((i>>1)<<(shift+3));
//        c->direct_basis_mv[1][i][0]= c->co_located_mv[i][0]*(time_pb - time_pp)/time_pp + ((i &1)<<(shift+3);
//        c->direct_basis_mv[1][i][1]= c->co_located_mv[i][1]*(time_pb - time_pp)/time_pp + ((i>>1)<<(shift+3);

        max= FFMAX(c->direct_basis_mv[i][0], c->direct_basis_mv[i][0] - c->co_located_mv[i][0])>>shift;
        min= FFMIN(c->direct_basis_mv[i][0], c->direct_basis_mv[i][0] - c->co_located_mv[i][0])>>shift;
        max+= 16*mb_x + 1; // +-1 is for the simpler rounding
        min+= 16*mb_x - 1;
        xmax= FFMIN(xmax, s->width - max);
        xmin= FFMAX(xmin, - 16     - min);

        max= FFMAX(c->direct_basis_mv[i][1], c->direct_basis_mv[i][1] - c->co_located_mv[i][1])>>shift;
        min= FFMIN(c->direct_basis_mv[i][1], c->direct_basis_mv[i][1] - c->co_located_mv[i][1])>>shift;
        max+= 16*mb_y + 1; // +-1 is for the simpler rounding
        min+= 16*mb_y - 1;
        ymax= FFMIN(ymax, s->height - max);
        ymin= FFMAX(ymin, - 16      - min);
        
        if(s->mv_type == MV_TYPE_16X16) break;
    }
    
    assert(xmax <= 15 && ymax <= 15 && xmin >= -16 && ymin >= -16);
    
    if(xmax < 0 || xmin >0 || ymax < 0 || ymin > 0){
        s->b_direct_mv_table[mot_xy][0]= 0;
        s->b_direct_mv_table[mot_xy][1]= 0;

        return 256*256*256*64;
    }
    
    c->xmin= xmin;
    c->ymin= ymin;
    c->xmax= xmax;
    c->ymax= ymax;
    c->flags     |= FLAG_DIRECT;
    c->sub_flags |= FLAG_DIRECT;
    c->pred_x=0;
    c->pred_y=0;

    P_LEFT[0]        = clip(mv_table[mot_xy - 1][0], xmin<<shift, xmax<<shift);
    P_LEFT[1]        = clip(mv_table[mot_xy - 1][1], ymin<<shift, ymax<<shift);

    /* special case for first line */
    if (!s->first_slice_line) { //FIXME maybe allow this over thread boundary as its cliped
        P_TOP[0]      = clip(mv_table[mot_xy - mot_stride             ][0], xmin<<shift, xmax<<shift);
        P_TOP[1]      = clip(mv_table[mot_xy - mot_stride             ][1], ymin<<shift, ymax<<shift);
        P_TOPRIGHT[0] = clip(mv_table[mot_xy - mot_stride + 1         ][0], xmin<<shift, xmax<<shift);
        P_TOPRIGHT[1] = clip(mv_table[mot_xy - mot_stride + 1         ][1], ymin<<shift, ymax<<shift);
    
        P_MEDIAN[0]= mid_pred(P_LEFT[0], P_TOP[0], P_TOPRIGHT[0]);
        P_MEDIAN[1]= mid_pred(P_LEFT[1], P_TOP[1], P_TOPRIGHT[1]);
    }
 
    dmin = epzs_motion_search(s, &mx, &my, P, 0, 0, mv_table, 1<<(16-shift));
    if(c->sub_flags&FLAG_QPEL) 
        dmin = qpel_motion_search(s, &mx, &my, dmin, 0, 0, 0, 16);
    else
        dmin = hpel_motion_search(s, &mx, &my, dmin, 0, 0, 0, 16);
    
    if(c->avctx->me_sub_cmp != c->avctx->mb_cmp && !c->skip)
        dmin= get_mb_score(s, mx, my, 0, 0);
    
    get_limits(s, 16*mb_x, 16*mb_y); //restore c->?min/max, maybe not needed

    s->b_direct_mv_table[mot_xy][0]= mx;
    s->b_direct_mv_table[mot_xy][1]= my;
    c->flags     &= ~FLAG_DIRECT;
    c->sub_flags &= ~FLAG_DIRECT;

    return dmin;
}

void ff_estimate_b_frame_motion(MpegEncContext * s,
                             int mb_x, int mb_y)
{
    MotionEstContext * const c= &s->me;
    const int penalty_factor= c->mb_penalty_factor;
    int fmin, bmin, dmin, fbmin, bimin, fimin;
    int type=0;
    const int xy = mb_y*s->mb_stride + mb_x;
    init_ref(c, s->new_picture.data, s->last_picture.data, s->next_picture.data, 16*mb_x, 16*mb_y, 2);

    get_limits(s, 16*mb_x, 16*mb_y);
    
    c->skip=0;
    if(c->avctx->me_threshold){
        int vard= (check_input_motion(s, mb_x, mb_y, 0)+128)>>8;
        
        if(vard<c->avctx->me_threshold){
//            pix = c->src[0][0];
//            sum = s->dsp.pix_sum(pix, s->linesize);
//            varc = (s->dsp.pix_norm1(pix, s->linesize) - (((unsigned)(sum*sum))>>8) + 500 + 128)>>8;
        
//            pic->mb_var   [s->mb_stride * mb_y + mb_x] = varc;
             s->current_picture.mc_mb_var[s->mb_stride * mb_y + mb_x] = vard;
/*            pic->mb_mean  [s->mb_stride * mb_y + mb_x] = (sum+128)>>8;
            c->mb_var_sum_temp    += varc;*/
            c->mc_mb_var_sum_temp += vard;
/*            if (vard <= 64 || vard < varc) {
                c->scene_change_score+= ff_sqrt(vard) - ff_sqrt(varc);
            }else{
                c->scene_change_score+= s->qscale;
            }*/
            return;
        }
        if(vard<c->avctx->mb_threshold){
            type= s->mb_type[mb_y*s->mb_stride + mb_x];
            if(type == CANDIDATE_MB_TYPE_DIRECT){
                direct_search(s, mb_x, mb_y);
            }
            if(type == CANDIDATE_MB_TYPE_FORWARD || type == CANDIDATE_MB_TYPE_BIDIR){
                c->skip=0;
                ff_estimate_motion_b(s, mb_x, mb_y, s->b_forw_mv_table, 0, s->f_code);
            }
            if(type == CANDIDATE_MB_TYPE_BACKWARD || type == CANDIDATE_MB_TYPE_BIDIR){
                c->skip=0;
                ff_estimate_motion_b(s, mb_x, mb_y, s->b_back_mv_table, 2, s->b_code);
            }
            if(type == CANDIDATE_MB_TYPE_FORWARD_I || type == CANDIDATE_MB_TYPE_BIDIR_I){
                c->skip=0;
                c->current_mv_penalty= c->mv_penalty[s->f_code] + MAX_MV;
                interlaced_search(s, 0,
                                        s->b_field_mv_table[0], s->b_field_select_table[0],
                                        s->b_forw_mv_table[xy][0], s->b_forw_mv_table[xy][1], 1);
            }
            if(type == CANDIDATE_MB_TYPE_BACKWARD_I || type == CANDIDATE_MB_TYPE_BIDIR_I){
                c->skip=0;
                c->current_mv_penalty= c->mv_penalty[s->b_code] + MAX_MV;
                interlaced_search(s, 2,
                                        s->b_field_mv_table[1], s->b_field_select_table[1],
                                        s->b_back_mv_table[xy][0], s->b_back_mv_table[xy][1], 1);
            }
            return;
        }
    }

    if (s->codec_id == CODEC_ID_MPEG4)
        dmin= direct_search(s, mb_x, mb_y);
    else
        dmin= INT_MAX;
//FIXME penalty stuff for non mpeg4
    c->skip=0;
    fmin= ff_estimate_motion_b(s, mb_x, mb_y, s->b_forw_mv_table, 0, s->f_code) + 3*penalty_factor;
    
    c->skip=0;
    bmin= ff_estimate_motion_b(s, mb_x, mb_y, s->b_back_mv_table, 2, s->b_code) + 2*penalty_factor;
//printf(" %d %d ", s->b_forw_mv_table[xy][0], s->b_forw_mv_table[xy][1]);

    c->skip=0;
    fbmin= bidir_refine(s, mb_x, mb_y) + penalty_factor;
//printf("%d %d %d %d\n", dmin, fmin, bmin, fbmin);
    
    if(s->flags & CODEC_FLAG_INTERLACED_ME){
//FIXME mb type penalty
        c->skip=0;
        c->current_mv_penalty= c->mv_penalty[s->f_code] + MAX_MV;
        fimin= interlaced_search(s, 0,
                                 s->b_field_mv_table[0], s->b_field_select_table[0],
                                 s->b_forw_mv_table[xy][0], s->b_forw_mv_table[xy][1], 0);
        c->current_mv_penalty= c->mv_penalty[s->b_code] + MAX_MV;
        bimin= interlaced_search(s, 2,
                                 s->b_field_mv_table[1], s->b_field_select_table[1],
                                 s->b_back_mv_table[xy][0], s->b_back_mv_table[xy][1], 0);
    }else
        fimin= bimin= INT_MAX;

    {
        int score= fmin;
        type = CANDIDATE_MB_TYPE_FORWARD;
        
        if (dmin <= score){
            score = dmin;
            type = CANDIDATE_MB_TYPE_DIRECT;
        }
        if(bmin<score){
            score=bmin;
            type= CANDIDATE_MB_TYPE_BACKWARD; 
        }
        if(fbmin<score){
            score=fbmin;
            type= CANDIDATE_MB_TYPE_BIDIR;
        }
        if(fimin<score){
            score=fimin;
            type= CANDIDATE_MB_TYPE_FORWARD_I;
        }
        if(bimin<score){
            score=bimin;
            type= CANDIDATE_MB_TYPE_BACKWARD_I;
        }
        
        score= ((unsigned)(score*score + 128*256))>>16;
        c->mc_mb_var_sum_temp += score;
        s->current_picture.mc_mb_var[mb_y*s->mb_stride + mb_x] = score; //FIXME use SSE
    }

    if(c->avctx->mb_decision > FF_MB_DECISION_SIMPLE){
        type= CANDIDATE_MB_TYPE_FORWARD | CANDIDATE_MB_TYPE_BACKWARD | CANDIDATE_MB_TYPE_BIDIR | CANDIDATE_MB_TYPE_DIRECT;
        if(fimin < INT_MAX)
            type |= CANDIDATE_MB_TYPE_FORWARD_I;
        if(bimin < INT_MAX)
            type |= CANDIDATE_MB_TYPE_BACKWARD_I;
        if(fimin < INT_MAX && bimin < INT_MAX){
            type |= CANDIDATE_MB_TYPE_BIDIR_I;
        }
         //FIXME something smarter
        if(dmin>256*256*16) type&= ~CANDIDATE_MB_TYPE_DIRECT; //dont try direct mode if its invalid for this MB
#if 0        
        if(s->out_format == FMT_MPEG1)
            type |= CANDIDATE_MB_TYPE_INTRA;
#endif
    }

    s->mb_type[mb_y*s->mb_stride + mb_x]= type;
}

/* find best f_code for ME which do unlimited searches */
int ff_get_best_fcode(MpegEncContext * s, int16_t (*mv_table)[2], int type)
{
    if(s->me_method>=ME_EPZS){
        int score[8];
        int i, y;
        uint8_t * fcode_tab= s->fcode_tab;
        int best_fcode=-1;
        int best_score=-10000000;

        for(i=0; i<8; i++) score[i]= s->mb_num*(8-i);

        for(y=0; y<s->mb_height; y++){
            int x;
            int xy= y*s->mb_stride;
            for(x=0; x<s->mb_width; x++){
                if(s->mb_type[xy] & type){
                    int fcode= FFMAX(fcode_tab[mv_table[xy][0] + MAX_MV],
                                     fcode_tab[mv_table[xy][1] + MAX_MV]);
                    int j;
                    
                    for(j=0; j<fcode && j<8; j++){
                        if(s->pict_type==B_TYPE || s->current_picture.mc_mb_var[xy] < s->current_picture.mb_var[xy])
                            score[j]-= 170;
                    }
                }
                xy++;
            }
        }
        
        for(i=1; i<8; i++){
            if(score[i] > best_score){
                best_score= score[i];
                best_fcode= i;
            }
//            printf("%d %d\n", i, score[i]);
        }

//    printf("fcode: %d type: %d\n", i, s->pict_type);
        return best_fcode;
/*        for(i=0; i<=MAX_FCODE; i++){
            printf("%d ", mv_num[i]);
        }
        printf("\n");*/
    }else{
        return 1;
    }
}

void ff_fix_long_p_mvs(MpegEncContext * s)
{
    MotionEstContext * const c= &s->me;
    const int f_code= s->f_code;
    int y, range;
    assert(s->pict_type==P_TYPE);

    range = (((s->out_format == FMT_MPEG1) ? 8 : 16) << f_code);
    
    if(s->msmpeg4_version) range= 16;
    
    if(c->avctx->me_range && range > c->avctx->me_range) range= c->avctx->me_range;
    
//printf("%d no:%d %d//\n", clip, noclip, f_code);
    if(s->flags&CODEC_FLAG_4MV){
        const int wrap= s->b8_stride;

        /* clip / convert to intra 8x8 type MVs */
        for(y=0; y<s->mb_height; y++){
            int xy= y*2*wrap;
            int i= y*s->mb_stride;
            int x;

            for(x=0; x<s->mb_width; x++){
                if(s->mb_type[i]&CANDIDATE_MB_TYPE_INTER4V){
                    int block;
                    for(block=0; block<4; block++){
                        int off= (block& 1) + (block>>1)*wrap;
                        int mx= s->current_picture.motion_val[0][ xy + off ][0];
                        int my= s->current_picture.motion_val[0][ xy + off ][1];

                        if(   mx >=range || mx <-range
                           || my >=range || my <-range){
                            s->mb_type[i] &= ~CANDIDATE_MB_TYPE_INTER4V;
                            s->mb_type[i] |= CANDIDATE_MB_TYPE_INTRA;
                            s->current_picture.mb_type[i]= CANDIDATE_MB_TYPE_INTRA;
                        }
                    }
                }
                xy+=2;
                i++;
            }
        }
    }
}

/**
 *
 * @param truncate 1 for truncation, 0 for using intra
 */
void ff_fix_long_mvs(MpegEncContext * s, uint8_t *field_select_table, int field_select, 
                     int16_t (*mv_table)[2], int f_code, int type, int truncate)
{
    MotionEstContext * const c= &s->me;
    int y, h_range, v_range;

    // RAL: 8 in MPEG-1, 16 in MPEG-4
    int range = (((s->out_format == FMT_MPEG1) ? 8 : 16) << f_code);

    if(s->msmpeg4_version) range= 16;
    if(c->avctx->me_range && range > c->avctx->me_range) range= c->avctx->me_range;

    h_range= range;
    v_range= field_select_table ? range>>1 : range;

    /* clip / convert to intra 16x16 type MVs */
    for(y=0; y<s->mb_height; y++){
        int x;
        int xy= y*s->mb_stride;
        for(x=0; x<s->mb_width; x++){
            if (s->mb_type[xy] & type){    // RAL: "type" test added...
                if(field_select_table==NULL || field_select_table[xy] == field_select){
                    if(   mv_table[xy][0] >=h_range || mv_table[xy][0] <-h_range
                       || mv_table[xy][1] >=v_range || mv_table[xy][1] <-v_range){

                        if(truncate){
                            if     (mv_table[xy][0] > h_range-1) mv_table[xy][0]=  h_range-1;
                            else if(mv_table[xy][0] < -h_range ) mv_table[xy][0]= -h_range;
                            if     (mv_table[xy][1] > v_range-1) mv_table[xy][1]=  v_range-1;
                            else if(mv_table[xy][1] < -v_range ) mv_table[xy][1]= -v_range;
                        }else{
                            s->mb_type[xy] &= ~type;
                            s->mb_type[xy] |= CANDIDATE_MB_TYPE_INTRA;
                            mv_table[xy][0]=
                            mv_table[xy][1]= 0;
                        }
                    }
                }
            }
            xy++;
        }
    }
}
