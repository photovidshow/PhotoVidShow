/*
 * MPEG1 codec / MPEG2 decoder
 * Copyright (c) 2000,2001 Fabrice Bellard.
 * Copyright (c) 2002-2004 Michael Niedermayer <michaelni@gmx.at> 
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
 */
 
/**
 * @file mpeg12.c
 * MPEG1/2 codec
 */
 
//#define DEBUG
#include "avcodec.h"
#include "dsputil.h"
#include "mpegvideo.h"


#include "mpeg12data.h"

//#undef NDEBUG
//#include <assert.h>


/* Start codes. */
#define SEQ_END_CODE		0x000001b7
#define SEQ_START_CODE		0x000001b3
#define GOP_START_CODE		0x000001b8
#define PICTURE_START_CODE	0x00000100
#define SLICE_MIN_START_CODE	0x00000101
#define SLICE_MAX_START_CODE	0x000001af
#define EXT_START_CODE		0x000001b5
#define USER_START_CODE		0x000001b2

#define DC_VLC_BITS 9
#define MV_VLC_BITS 9
#define MBINCR_VLC_BITS 9
#define MB_PAT_VLC_BITS 9
#define MB_PTYPE_VLC_BITS 6
#define MB_BTYPE_VLC_BITS 6
#define TEX_VLC_BITS 9

#ifdef CONFIG_ENCODERS
static void mpeg1_encode_block(MpegEncContext *s, 
                         DCTELEM *block, 
                         int component);
static void mpeg1_encode_motion(MpegEncContext *s, int val, int f_or_b_code);    // RAL: f_code parameter added
#endif //CONFIG_ENCODERS

static void exchange_uv(MpegEncContext *s);

#ifdef HAVE_XVMC
extern int XVMC_field_start(MpegEncContext *s, AVCodecContext *avctx);
extern int XVMC_field_end(MpegEncContext *s);
extern void XVMC_pack_pblocks(MpegEncContext *s,int cbp);
extern void XVMC_init_block(MpegEncContext *s);//set s->block
#endif

const enum PixelFormat pixfmt_yuv_420[]= {PIX_FMT_YUV420P,-1};
const enum PixelFormat pixfmt_yuv_422[]= {PIX_FMT_YUV422P,-1};
const enum PixelFormat pixfmt_yuv_444[]= {PIX_FMT_YUV444P,-1};
const enum PixelFormat pixfmt_xvmc_mpg2_420[] = {
                                           PIX_FMT_XVMC_MPEG2_IDCT,
                                           PIX_FMT_XVMC_MPEG2_MC,
					   -1};
#ifdef CONFIG_ENCODERS
static uint8_t (*mv_penalty)[MAX_MV*2+1]= NULL;
static uint8_t fcode_tab[MAX_MV*2+1];

static uint32_t uni_mpeg1_ac_vlc_bits[64*64*2];
static uint8_t  uni_mpeg1_ac_vlc_len [64*64*2];

/* simple include everything table for dc, first byte is bits number next 3 are code*/
static uint32_t mpeg1_lum_dc_uni[512];
static uint32_t mpeg1_chr_dc_uni[512];

static uint8_t mpeg1_index_run[2][64];
static int8_t mpeg1_max_level[2][64];
#endif //CONFIG_ENCODERS

static void init_2d_vlc_rl(RLTable *rl)
{
    int i;
    
    init_vlc(&rl->vlc, TEX_VLC_BITS, rl->n + 2, 
             &rl->table_vlc[0][1], 4, 2,
             &rl->table_vlc[0][0], 4, 2);

    
    rl->rl_vlc[0]= av_malloc(rl->vlc.table_size*sizeof(RL_VLC_ELEM));
    for(i=0; i<rl->vlc.table_size; i++){
        int code= rl->vlc.table[i][0];
        int len = rl->vlc.table[i][1];
        int level, run;
    
        if(len==0){ // illegal code
            run= 65;
            level= MAX_LEVEL;
        }else if(len<0){ //more bits needed
            run= 0;
            level= code;
        }else{
            if(code==rl->n){ //esc
                run= 65;
                level= 0;
            }else if(code==rl->n+1){ //eob
                run= 0;
                level= 127;
            }else{
                run=   rl->table_run  [code] + 1;
                level= rl->table_level[code];
            }
        }
        rl->rl_vlc[0][i].len= len;
        rl->rl_vlc[0][i].level= level;
        rl->rl_vlc[0][i].run= run;
    }
}

#ifdef CONFIG_ENCODERS
static void init_uni_ac_vlc(RLTable *rl, uint32_t *uni_ac_vlc_bits, uint8_t *uni_ac_vlc_len){
    int i;

    for(i=0; i<128; i++){
        int level= i-64;
        int run;
        for(run=0; run<64; run++){
            int len, bits, code;
            
            int alevel= ABS(level);
            int sign= (level>>31)&1;

            if (alevel > rl->max_level[0][run])
                code= 111; /*rl->n*/
            else
                code= rl->index_run[0][run] + alevel - 1;

            if (code < 111 /* rl->n */) {
	    	/* store the vlc & sign at once */
                len=   mpeg1_vlc[code][1]+1;
                bits= (mpeg1_vlc[code][0]<<1) + sign;
            } else {
                len=  mpeg1_vlc[111/*rl->n*/][1]+6;
                bits= mpeg1_vlc[111/*rl->n*/][0]<<6;

                bits|= run;
                if (alevel < 128) {
                    bits<<=8; len+=8;
                    bits|= level & 0xff;
                } else {
                    bits<<=16; len+=16;
                    bits|= level & 0xff;
                    if (level < 0) {
                        bits|= 0x8001 + level + 255;
                    } else {
                        bits|= level & 0xffff;
                    }
                }
            }

            uni_ac_vlc_bits[UNI_AC_ENC_INDEX(run, i)]= bits;
            uni_ac_vlc_len [UNI_AC_ENC_INDEX(run, i)]= len;
        }
    }
}


static int find_frame_rate_index(MpegEncContext *s){
    int i;
    int64_t dmin= INT64_MAX;
    int64_t d;

    for(i=1;i<14;i++) {
        int64_t n0= 1001LL/frame_rate_tab[i].den*frame_rate_tab[i].num*s->avctx->frame_rate_base;
        int64_t n1= 1001LL*s->avctx->frame_rate;
        if(s->avctx->strict_std_compliance >= 0 && i>=9) break;

        d = ABS(n0 - n1);
        if(d < dmin){
            dmin=d;
            s->frame_rate_index= i;
        }
    }
    if(dmin)
        return -1;
    else
        return 0;
}

static int encode_init(AVCodecContext *avctx)
{
    MpegEncContext *s = avctx->priv_data;

    if(MPV_encode_init(avctx) < 0)
        return -1;

    if(find_frame_rate_index(s) < 0){
        if(s->strict_std_compliance >=0){
            av_log(avctx, AV_LOG_ERROR, "MPEG1/2 doesnt support %d/%d fps\n", avctx->frame_rate, avctx->frame_rate_base);
            return -1;
        }else{
            av_log(avctx, AV_LOG_INFO, "MPEG1/2 doesnt support %d/%d fps, there may be AV sync issues\n", avctx->frame_rate, avctx->frame_rate_base);
        }
    }
    
    return 0;
}

static void put_header(MpegEncContext *s, int header)
{
    align_put_bits(&s->pb);
    put_bits(&s->pb, 16, header>>16);
    put_bits(&s->pb, 16, header&0xFFFF);
}

/* put sequence header if needed */
static void mpeg1_encode_sequence_header(MpegEncContext *s)
{
        unsigned int vbv_buffer_size;
        unsigned int fps, v;
        int i;
        uint64_t time_code;
        float best_aspect_error= 1E10;
        float aspect_ratio= av_q2d(s->avctx->sample_aspect_ratio);
        int constraint_parameter_flag;
        
        if(aspect_ratio==0.0) aspect_ratio= 1.0; //pixel aspect 1:1 (VGA)
        
        if (s->current_picture.key_frame) {
            AVRational framerate= frame_rate_tab[s->frame_rate_index];

            /* mpeg1 header repeated every gop */
            put_header(s, SEQ_START_CODE);
 
            put_bits(&s->pb, 12, s->width);
            put_bits(&s->pb, 12, s->height);
            
            for(i=1; i<15; i++){
                float error= aspect_ratio;
                if(s->codec_id == CODEC_ID_MPEG1VIDEO || i <=1)
                    error-= 1.0/mpeg1_aspect[i];
                else
                    error-= av_q2d(mpeg2_aspect[i])*s->height/s->width;
             
                error= ABS(error);
                
                if(error < best_aspect_error){
                    best_aspect_error= error;
                    s->aspect_ratio_info= i;
                }
            }
            
            put_bits(&s->pb, 4, s->aspect_ratio_info);
            put_bits(&s->pb, 4, s->frame_rate_index);
            
            if(s->avctx->rc_max_rate){
                v = (s->avctx->rc_max_rate + 399) / 400;
                if (v > 0x3ffff && s->codec_id == CODEC_ID_MPEG1VIDEO)
                    v = 0x3ffff;
            }else{
                v= 0x3FFFF;
            }

            if(s->avctx->rc_buffer_size)
                vbv_buffer_size = s->avctx->rc_buffer_size;
            else
                /* VBV calculation: Scaled so that a VCD has the proper VBV size of 40 kilobytes */
                vbv_buffer_size = (( 20 * s->bit_rate) / (1151929 / 2)) * 8 * 1024;
            vbv_buffer_size= (vbv_buffer_size + 16383) / 16384;

            put_bits(&s->pb, 18, v & 0x3FFFF);
            put_bits(&s->pb, 1, 1); /* marker */
            put_bits(&s->pb, 10, vbv_buffer_size & 0x3FF);

            constraint_parameter_flag= 
                s->width <= 768 && s->height <= 576 && 
                s->mb_width * s->mb_height <= 396 &&
                s->mb_width * s->mb_height * framerate.num <= framerate.den*396*25 &&
                framerate.num <= framerate.den*30 &&
                vbv_buffer_size <= 20 &&
                v <= 1856000/400 &&
                s->codec_id == CODEC_ID_MPEG1VIDEO;
                
            put_bits(&s->pb, 1, constraint_parameter_flag);
            
            ff_write_quant_matrix(&s->pb, s->avctx->intra_matrix);
            ff_write_quant_matrix(&s->pb, s->avctx->inter_matrix);

            if(s->codec_id == CODEC_ID_MPEG2VIDEO){
                put_header(s, EXT_START_CODE);
                put_bits(&s->pb, 4, 1); //seq ext
                put_bits(&s->pb, 1, 0); //esc
                put_bits(&s->pb, 3, 4); //profile
                put_bits(&s->pb, 4, 8); //level
                put_bits(&s->pb, 1, s->progressive_sequence);
                put_bits(&s->pb, 2, 1); //chroma format 4:2:0
                put_bits(&s->pb, 2, 0); //horizontal size ext
                put_bits(&s->pb, 2, 0); //vertical size ext
                put_bits(&s->pb, 12, v>>18); //bitrate ext
                put_bits(&s->pb, 1, 1); //marker
                put_bits(&s->pb, 8, vbv_buffer_size >>10); //vbv buffer ext
                put_bits(&s->pb, 1, s->low_delay);
                put_bits(&s->pb, 2, 0); // frame_rate_ext_n
                put_bits(&s->pb, 5, 0); // frame_rate_ext_d
            }
            
            put_header(s, GOP_START_CODE);
            put_bits(&s->pb, 1, 0); /* do drop frame */
            /* time code : we must convert from the real frame rate to a
               fake mpeg frame rate in case of low frame rate */
            fps = (framerate.num + framerate.den/2)/ framerate.den;
            time_code = s->current_picture_ptr->coded_picture_number;

            s->gop_picture_number = time_code;
            put_bits(&s->pb, 5, (uint32_t)((time_code / (fps * 3600)) % 24));
            put_bits(&s->pb, 6, (uint32_t)((time_code / (fps * 60)) % 60));
            put_bits(&s->pb, 1, 1);
            put_bits(&s->pb, 6, (uint32_t)((time_code / fps) % 60));
            put_bits(&s->pb, 6, (uint32_t)((time_code % fps)));
            put_bits(&s->pb, 1, !!(s->flags & CODEC_FLAG_CLOSED_GOP));
            put_bits(&s->pb, 1, 0); /* broken link */
        }
}

static __inline void encode_mb_skip_run(MpegEncContext *s, int run){
    while (run >= 33) {
        put_bits(&s->pb, 11, 0x008);
        run -= 33;
    }
    put_bits(&s->pb, mbAddrIncrTable[run][1], 
             mbAddrIncrTable[run][0]);
}
#endif //CONFIG_ENCODERS

static void common_init(MpegEncContext *s)
{

    s->y_dc_scale_table=
    s->c_dc_scale_table= mpeg2_dc_scale_table[s->intra_dc_precision];

}

void ff_mpeg1_clean_buffers(MpegEncContext *s){
    s->last_dc[0] = 1 << (7 + s->intra_dc_precision);
    s->last_dc[1] = s->last_dc[0];
    s->last_dc[2] = s->last_dc[0];
    memset(s->last_mv, 0, sizeof(s->last_mv));
}

#ifdef CONFIG_ENCODERS

void ff_mpeg1_encode_slice_header(MpegEncContext *s){
    put_header(s, SLICE_MIN_START_CODE + s->mb_y);
    put_bits(&s->pb, 5, s->qscale); /* quantizer scale */
    put_bits(&s->pb, 1, 0); /* slice extra information */
}

void mpeg1_encode_picture_header(MpegEncContext *s, int picture_number)
{
    mpeg1_encode_sequence_header(s);

    /* mpeg1 picture header */
    put_header(s, PICTURE_START_CODE);
    /* temporal reference */

    // RAL: s->picture_number instead of s->fake_picture_number
    put_bits(&s->pb, 10, (s->picture_number - 
                          s->gop_picture_number) & 0x3ff); 
    put_bits(&s->pb, 3, s->pict_type);

    s->vbv_delay_ptr= s->pb.buf + put_bits_count(&s->pb)/8;
    put_bits(&s->pb, 16, 0xFFFF); /* vbv_delay */
    
    // RAL: Forward f_code also needed for B frames
    if (s->pict_type == P_TYPE || s->pict_type == B_TYPE) {
        put_bits(&s->pb, 1, 0); /* half pel coordinates */
        if(s->codec_id == CODEC_ID_MPEG1VIDEO)
            put_bits(&s->pb, 3, s->f_code); /* forward_f_code */
        else
            put_bits(&s->pb, 3, 7); /* forward_f_code */
    }
    
    // RAL: Backward f_code necessary for B frames
    if (s->pict_type == B_TYPE) {
        put_bits(&s->pb, 1, 0); /* half pel coordinates */
        if(s->codec_id == CODEC_ID_MPEG1VIDEO)
            put_bits(&s->pb, 3, s->b_code); /* backward_f_code */
        else
            put_bits(&s->pb, 3, 7); /* backward_f_code */
    }

    put_bits(&s->pb, 1, 0); /* extra bit picture */

    s->frame_pred_frame_dct = 1;
    if(s->codec_id == CODEC_ID_MPEG2VIDEO){
        put_header(s, EXT_START_CODE);
        put_bits(&s->pb, 4, 8); //pic ext
        if (s->pict_type == P_TYPE || s->pict_type == B_TYPE) {
            put_bits(&s->pb, 4, s->f_code);
            put_bits(&s->pb, 4, s->f_code);
        }else{
            put_bits(&s->pb, 8, 255);
        }
        if (s->pict_type == B_TYPE) {
            put_bits(&s->pb, 4, s->b_code);
            put_bits(&s->pb, 4, s->b_code);
        }else{
            put_bits(&s->pb, 8, 255);
        }
        put_bits(&s->pb, 2, s->intra_dc_precision);
        
        assert(s->picture_structure == PICT_FRAME);
        put_bits(&s->pb, 2, s->picture_structure);
        if (s->progressive_sequence) {
            put_bits(&s->pb, 1, 0); /* no repeat */
        } else {
            put_bits(&s->pb, 1, s->current_picture_ptr->top_field_first);
        }
        /* XXX: optimize the generation of this flag with entropy
           measures */
        s->frame_pred_frame_dct = s->progressive_sequence;
        
        put_bits(&s->pb, 1, s->frame_pred_frame_dct);
        put_bits(&s->pb, 1, s->concealment_motion_vectors);
        put_bits(&s->pb, 1, s->q_scale_type);
        put_bits(&s->pb, 1, s->intra_vlc_format);
        put_bits(&s->pb, 1, s->alternate_scan);
        put_bits(&s->pb, 1, s->repeat_first_field);
        put_bits(&s->pb, 1, s->chroma_420_type=1);
        s->progressive_frame = s->progressive_sequence;
        put_bits(&s->pb, 1, s->progressive_frame);
        put_bits(&s->pb, 1, 0); //composite_display_flag
    }
    if(s->flags & CODEC_FLAG_SVCD_SCAN_OFFSET){
        int i;

        put_header(s, USER_START_CODE);
        for(i=0; i<sizeof(svcd_scan_offset_placeholder); i++){
            put_bits(&s->pb, 8, svcd_scan_offset_placeholder[i]);
        }
    }
    
    s->mb_y=0;
    ff_mpeg1_encode_slice_header(s);
}

static __inline void put_mb_modes(MpegEncContext *s, int n, int bits, 
                                int has_mv, int field_motion)
{
    put_bits(&s->pb, n, bits);
    if (!s->frame_pred_frame_dct) {
        if (has_mv) 
            put_bits(&s->pb, 2, 2 - field_motion); /* motion_type: frame/field */
        put_bits(&s->pb, 1, s->interlaced_dct);
    }
}

void mpeg1_encode_mb(MpegEncContext *s,
                     DCTELEM block[6][64],
                     int motion_x, int motion_y)
{
    int i, cbp;
    const int mb_x = s->mb_x;
    const int mb_y = s->mb_y;
    const int first_mb= mb_x == s->resync_mb_x && mb_y == s->resync_mb_y;

    /* compute cbp */
    cbp = 0;
    for(i=0;i<6;i++) {
        if (s->block_last_index[i] >= 0)
            cbp |= 1 << (5 - i);
    }
    
    if (cbp == 0 && !first_mb && s->mv_type == MV_TYPE_16X16 &&
        (mb_x != s->mb_width - 1 || (mb_y != s->mb_height - 1 && s->codec_id == CODEC_ID_MPEG1VIDEO)) && 
        ((s->pict_type == P_TYPE && (motion_x | motion_y) == 0) ||
        (s->pict_type == B_TYPE && s->mv_dir == s->last_mv_dir && (((s->mv_dir & MV_DIR_FORWARD) ? ((s->mv[0][0][0] - s->last_mv[0][0][0])|(s->mv[0][0][1] - s->last_mv[0][0][1])) : 0) |
        ((s->mv_dir & MV_DIR_BACKWARD) ? ((s->mv[1][0][0] - s->last_mv[1][0][0])|(s->mv[1][0][1] - s->last_mv[1][0][1])) : 0)) == 0))) {
        s->mb_skip_run++;
        s->qscale -= s->dquant;
        s->skip_count++;
        s->misc_bits++;
        s->last_bits++;
        if(s->pict_type == P_TYPE){
            s->last_mv[0][1][0]= s->last_mv[0][0][0]= 
            s->last_mv[0][1][1]= s->last_mv[0][0][1]= 0;
        }
    } else {
        if(first_mb){
            assert(s->mb_skip_run == 0);
            encode_mb_skip_run(s, s->mb_x);
        }else{
            encode_mb_skip_run(s, s->mb_skip_run);
        }
        
        if (s->pict_type == I_TYPE) {
            if(s->dquant && cbp){
                put_mb_modes(s, 2, 1, 0, 0); /* macroblock_type : macroblock_quant = 1 */
                put_bits(&s->pb, 5, s->qscale);
            }else{
                put_mb_modes(s, 1, 1, 0, 0); /* macroblock_type : macroblock_quant = 0 */
                s->qscale -= s->dquant;
            }
            s->misc_bits+= get_bits_diff(s);
            s->i_count++;
        } else if (s->mb_intra) {
            if(s->dquant && cbp){
                put_mb_modes(s, 6, 0x01, 0, 0);
                put_bits(&s->pb, 5, s->qscale);
            }else{
                put_mb_modes(s, 5, 0x03, 0, 0);
                s->qscale -= s->dquant;
            }
            s->misc_bits+= get_bits_diff(s);
            s->i_count++;
            memset(s->last_mv, 0, sizeof(s->last_mv));
        } else if (s->pict_type == P_TYPE) { 
            if(s->mv_type == MV_TYPE_16X16){
                if (cbp != 0) {
                    if ((motion_x|motion_y) == 0) {
                        if(s->dquant){
                            put_mb_modes(s, 5, 1, 0, 0); /* macroblock_pattern & quant */
                            put_bits(&s->pb, 5, s->qscale);
                        }else{
                            put_mb_modes(s, 2, 1, 0, 0); /* macroblock_pattern only */
                        }
                        s->misc_bits+= get_bits_diff(s);
                    } else {
                        if(s->dquant){
                            put_mb_modes(s, 5, 2, 1, 0); /* motion + cbp */
                            put_bits(&s->pb, 5, s->qscale);
                        }else{
                            put_mb_modes(s, 1, 1, 1, 0); /* motion + cbp */
                        }
                        s->misc_bits+= get_bits_diff(s);
                        mpeg1_encode_motion(s, motion_x - s->last_mv[0][0][0], s->f_code);    // RAL: f_code parameter added
                        mpeg1_encode_motion(s, motion_y - s->last_mv[0][0][1], s->f_code);    // RAL: f_code parameter added
                        s->mv_bits+= get_bits_diff(s);
                    }
                } else {
                    put_bits(&s->pb, 3, 1); /* motion only */
                    if (!s->frame_pred_frame_dct)
                        put_bits(&s->pb, 2, 2); /* motion_type: frame */
                    s->misc_bits+= get_bits_diff(s);
                    mpeg1_encode_motion(s, motion_x - s->last_mv[0][0][0], s->f_code);    // RAL: f_code parameter added
                    mpeg1_encode_motion(s, motion_y - s->last_mv[0][0][1], s->f_code);    // RAL: f_code parameter added
                    s->qscale -= s->dquant;
                    s->mv_bits+= get_bits_diff(s);
                }
                s->last_mv[0][1][0]= s->last_mv[0][0][0]= motion_x;
                s->last_mv[0][1][1]= s->last_mv[0][0][1]= motion_y;
            }else{
                assert(!s->frame_pred_frame_dct && s->mv_type == MV_TYPE_FIELD);

                if (cbp) {
                    if(s->dquant){
                        put_mb_modes(s, 5, 2, 1, 1); /* motion + cbp */
                        put_bits(&s->pb, 5, s->qscale);
                    }else{
                        put_mb_modes(s, 1, 1, 1, 1); /* motion + cbp */
                    }
                } else {
                    put_bits(&s->pb, 3, 1); /* motion only */
                    put_bits(&s->pb, 2, 1); /* motion_type: field */
                    s->qscale -= s->dquant;
                }
                s->misc_bits+= get_bits_diff(s);
                for(i=0; i<2; i++){
                    put_bits(&s->pb, 1, s->field_select[0][i]);
                    mpeg1_encode_motion(s, s->mv[0][i][0] -  s->last_mv[0][i][0]    , s->f_code);
                    mpeg1_encode_motion(s, s->mv[0][i][1] - (s->last_mv[0][i][1]>>1), s->f_code);
                    s->last_mv[0][i][0]=   s->mv[0][i][0];
                    s->last_mv[0][i][1]= 2*s->mv[0][i][1];
                }
                s->mv_bits+= get_bits_diff(s);
            }
            if(cbp)
                put_bits(&s->pb, mbPatTable[cbp][1], mbPatTable[cbp][0]);
            s->f_count++;
        } else{  
            static const int mb_type_len[4]={0,3,4,2}; //bak,for,bi

            if(s->mv_type == MV_TYPE_16X16){
                if (cbp){    // With coded bloc pattern
                    if (s->dquant) {
                        if(s->mv_dir == MV_DIR_FORWARD)
                            put_mb_modes(s, 6, 3, 1, 0);
                        else
                            put_mb_modes(s, mb_type_len[s->mv_dir]+3, 2, 1, 0);
                        put_bits(&s->pb, 5, s->qscale);
                    } else {
                        put_mb_modes(s, mb_type_len[s->mv_dir], 3, 1, 0);
                    }
                }else{    // No coded bloc pattern
                    put_bits(&s->pb, mb_type_len[s->mv_dir], 2);
                    if (!s->frame_pred_frame_dct)
                        put_bits(&s->pb, 2, 2); /* motion_type: frame */
                    s->qscale -= s->dquant;
                }
                s->misc_bits += get_bits_diff(s);
                if (s->mv_dir&MV_DIR_FORWARD){
                    mpeg1_encode_motion(s, s->mv[0][0][0] - s->last_mv[0][0][0], s->f_code); 
                    mpeg1_encode_motion(s, s->mv[0][0][1] - s->last_mv[0][0][1], s->f_code); 
                    s->last_mv[0][0][0]=s->last_mv[0][1][0]= s->mv[0][0][0];
                    s->last_mv[0][0][1]=s->last_mv[0][1][1]= s->mv[0][0][1];
                    s->f_count++;
                }
                if (s->mv_dir&MV_DIR_BACKWARD){
                    mpeg1_encode_motion(s, s->mv[1][0][0] - s->last_mv[1][0][0], s->b_code); 
                    mpeg1_encode_motion(s, s->mv[1][0][1] - s->last_mv[1][0][1], s->b_code); 
                    s->last_mv[1][0][0]=s->last_mv[1][1][0]= s->mv[1][0][0];
                    s->last_mv[1][0][1]=s->last_mv[1][1][1]= s->mv[1][0][1];
                    s->b_count++;
                }
            }else{
                assert(s->mv_type == MV_TYPE_FIELD);
                assert(!s->frame_pred_frame_dct);
                if (cbp){    // With coded bloc pattern
                    if (s->dquant) {
                        if(s->mv_dir == MV_DIR_FORWARD)
                            put_mb_modes(s, 6, 3, 1, 1);
                        else
                            put_mb_modes(s, mb_type_len[s->mv_dir]+3, 2, 1, 1);
                        put_bits(&s->pb, 5, s->qscale);
                    } else {
                        put_mb_modes(s, mb_type_len[s->mv_dir], 3, 1, 1);
                    }
                }else{    // No coded bloc pattern
                    put_bits(&s->pb, mb_type_len[s->mv_dir], 2);
                    put_bits(&s->pb, 2, 1); /* motion_type: field */
                    s->qscale -= s->dquant;
                }
                s->misc_bits += get_bits_diff(s);
                if (s->mv_dir&MV_DIR_FORWARD){
                    for(i=0; i<2; i++){
                        put_bits(&s->pb, 1, s->field_select[0][i]);
                        mpeg1_encode_motion(s, s->mv[0][i][0] -  s->last_mv[0][i][0]    , s->f_code);
                        mpeg1_encode_motion(s, s->mv[0][i][1] - (s->last_mv[0][i][1]>>1), s->f_code);
                        s->last_mv[0][i][0]=   s->mv[0][i][0];
                        s->last_mv[0][i][1]= 2*s->mv[0][i][1];
                    }
                    s->f_count++;
                }
                if (s->mv_dir&MV_DIR_BACKWARD){
                    for(i=0; i<2; i++){
                        put_bits(&s->pb, 1, s->field_select[1][i]);
                        mpeg1_encode_motion(s, s->mv[1][i][0] -  s->last_mv[1][i][0]    , s->b_code);
                        mpeg1_encode_motion(s, s->mv[1][i][1] - (s->last_mv[1][i][1]>>1), s->b_code);
                        s->last_mv[1][i][0]=   s->mv[1][i][0];
                        s->last_mv[1][i][1]= 2*s->mv[1][i][1];
                    }
                    s->b_count++;
                }
            }
            s->mv_bits += get_bits_diff(s);
            if(cbp)
                put_bits(&s->pb, mbPatTable[cbp][1], mbPatTable[cbp][0]);
        }
        for(i=0;i<6;i++) {
            if (cbp & (1 << (5 - i))) {
                mpeg1_encode_block(s, block[i], i);
            }
        }
        s->mb_skip_run = 0;
        if(s->mb_intra)
            s->i_tex_bits+= get_bits_diff(s);
        else
            s->p_tex_bits+= get_bits_diff(s);
    }
}

// RAL: Parameter added: f_or_b_code
static void mpeg1_encode_motion(MpegEncContext *s, int val, int f_or_b_code)
{
    int code, bit_size, l, bits, range, sign;

    if (val == 0) {
        /* zero vector */
        code = 0;
        put_bits(&s->pb,
                 mbMotionVectorTable[0][1], 
                 mbMotionVectorTable[0][0]); 
    } else {
        bit_size = f_or_b_code - 1;
        range = 1 << bit_size;
        /* modulo encoding */
        l= INT_BIT - 5 - bit_size;
        val= (val<<l)>>l;

        if (val >= 0) {
            val--;
            code = (val >> bit_size) + 1;
            bits = val & (range - 1);
            sign = 0;
        } else {
            val = -val;
            val--;
            code = (val >> bit_size) + 1;
            bits = val & (range - 1);
            sign = 1;
        }

        assert(code > 0 && code <= 16);

        put_bits(&s->pb,
                 mbMotionVectorTable[code][1], 
                 mbMotionVectorTable[code][0]); 

        put_bits(&s->pb, 1, sign);
        if (bit_size > 0) {
            put_bits(&s->pb, bit_size, bits);
        }
    }
}

void ff_mpeg1_encode_init(MpegEncContext *s)
{
    static int done=0;

    common_init(s);

    if(!done){
        int f_code;
        int mv;
	int i;

        done=1;
        init_rl(&rl_mpeg1);

	for(i=0; i<64; i++)
	{
		mpeg1_max_level[0][i]= rl_mpeg1.max_level[0][i];
		mpeg1_index_run[0][i]= rl_mpeg1.index_run[0][i];
	}
        
        init_uni_ac_vlc(&rl_mpeg1, uni_mpeg1_ac_vlc_bits, uni_mpeg1_ac_vlc_len);

	/* build unified dc encoding tables */
	for(i=-255; i<256; i++)
	{
		int adiff, index;
		int bits, code;
		int diff=i;

		adiff = ABS(diff);
		if(diff<0) diff--;
		index = av_log2(2*adiff);

		bits= vlc_dc_lum_bits[index] + index;
		code= (vlc_dc_lum_code[index]<<index) + (diff & ((1 << index) - 1));
		mpeg1_lum_dc_uni[i+255]= bits + (code<<8);
		
		bits= vlc_dc_chroma_bits[index] + index;
		code= (vlc_dc_chroma_code[index]<<index) + (diff & ((1 << index) - 1));
		mpeg1_chr_dc_uni[i+255]= bits + (code<<8);
	}

        mv_penalty= av_mallocz( sizeof(uint8_t)*(MAX_FCODE+1)*(2*MAX_MV+1) );

        for(f_code=1; f_code<=MAX_FCODE; f_code++){
            for(mv=-MAX_MV; mv<=MAX_MV; mv++){
                int len;

                if(mv==0) len= mbMotionVectorTable[0][1];
                else{
                    int val, bit_size, range, code;

                    bit_size = f_code - 1;
                    range = 1 << bit_size;

                    val=mv;
                    if (val < 0) 
                        val = -val;
                    val--;
                    code = (val >> bit_size) + 1;
                    if(code<17){
                        len= mbMotionVectorTable[code][1] + 1 + bit_size;
                    }else{
                        len= mbMotionVectorTable[16][1] + 2 + bit_size;
                    }
                }

                mv_penalty[f_code][mv+MAX_MV]= len;
            }
        }
        

        for(f_code=MAX_FCODE; f_code>0; f_code--){
            for(mv=-(8<<f_code); mv<(8<<f_code); mv++){
                fcode_tab[mv+MAX_MV]= f_code;
            }
        }
    }
    s->me.mv_penalty= mv_penalty;
    s->fcode_tab= fcode_tab;
    if(s->codec_id == CODEC_ID_MPEG1VIDEO){
        s->min_qcoeff=-255;
        s->max_qcoeff= 255;
    }else{
        s->min_qcoeff=-2047;
        s->max_qcoeff= 2047;
    }
    s->intra_ac_vlc_length=
    s->inter_ac_vlc_length=
    s->intra_ac_vlc_last_length=
    s->inter_ac_vlc_last_length= uni_mpeg1_ac_vlc_len;
}

static __inline void encode_dc(MpegEncContext *s, int diff, int component)
{
  if(((unsigned) (diff+255)) >= 511){
        int index;

        if(diff<0){
            index= av_log2_16bit(-2*diff);
            diff--;
        }else{
            index= av_log2_16bit(2*diff);
        }
        if (component == 0) {
            put_bits(
                &s->pb, 
                vlc_dc_lum_bits[index] + index,
                (vlc_dc_lum_code[index]<<index) + (diff & ((1 << index) - 1)));
        }else{
            put_bits(
                &s->pb, 
                vlc_dc_chroma_bits[index] + index,
                (vlc_dc_chroma_code[index]<<index) + (diff & ((1 << index) - 1)));
        }
  }else{
    if (component == 0) {
        put_bits(
	    &s->pb, 
	    mpeg1_lum_dc_uni[diff+255]&0xFF,
	    mpeg1_lum_dc_uni[diff+255]>>8);
    } else {
        put_bits(
            &s->pb, 
	    mpeg1_chr_dc_uni[diff+255]&0xFF,
	    mpeg1_chr_dc_uni[diff+255]>>8);
    }
  }
}

static void mpeg1_encode_block(MpegEncContext *s, 
                               DCTELEM *block, 
                               int n)
{
    int alevel, level, last_non_zero, dc, diff, i, j, run, last_index, sign;
    int code, component;
//    RLTable *rl = &rl_mpeg1;

    last_index = s->block_last_index[n];

    /* DC coef */
    if (s->mb_intra) {
        component = (n <= 3 ? 0 : n - 4 + 1);
        dc = block[0]; /* overflow is impossible */
        diff = dc - s->last_dc[component];
        encode_dc(s, diff, component);
        s->last_dc[component] = dc;
        i = 1;
/*
        if (s->intra_vlc_format)
            rl = &rl_mpeg2;
        else
            rl = &rl_mpeg1;
*/
    } else {
        /* encode the first coefficient : needs to be done here because
           it is handled slightly differently */
        level = block[0];
        if (abs(level) == 1) {
                code = ((uint32_t)level >> 31); /* the sign bit */
                put_bits(&s->pb, 2, code | 0x02);
                i = 1;
        } else {
            i = 0;
            last_non_zero = -1;
            goto next_coef;
        }
    }

    /* now quantify & encode AC coefs */
    last_non_zero = i - 1;

    for(;i<=last_index;i++) {
        j = s->intra_scantable.permutated[i];
        level = block[j];
    next_coef:
#if 0
        if (level != 0)
            dprintf("level[%d]=%d\n", i, level);
#endif            
        /* encode using VLC */
        if (level != 0) {
            run = i - last_non_zero - 1;
            
            alevel= level;
            MASK_ABS(sign, alevel)
            sign&=1;

//            code = get_rl_index(rl, 0, run, alevel);
            if (alevel <= mpeg1_max_level[0][run]){
                code= mpeg1_index_run[0][run] + alevel - 1;
	    	/* store the vlc & sign at once */
                put_bits(&s->pb, mpeg1_vlc[code][1]+1, (mpeg1_vlc[code][0]<<1) + sign);
            } else {
		/* escape seems to be pretty rare <5% so i dont optimize it */
                put_bits(&s->pb, mpeg1_vlc[111/*rl->n*/][1], mpeg1_vlc[111/*rl->n*/][0]);
                /* escape: only clip in this case */
                put_bits(&s->pb, 6, run);
                if(s->codec_id == CODEC_ID_MPEG1VIDEO){
                    if (alevel < 128) {
                        put_bits(&s->pb, 8, level & 0xff);
                    } else {
                        if (level < 0) {
                            put_bits(&s->pb, 16, 0x8001 + level + 255);
                        } else {
                            put_bits(&s->pb, 16, level & 0xffff);
                        }
                    }
                }else{
                    put_bits(&s->pb, 12, level & 0xfff);
                }
            }
            last_non_zero = i;
        }
    }
    /* end of block */
    put_bits(&s->pb, 2, 0x2);
}
#endif //CONFIG_ENCODERS


#ifdef CONFIG_ENCODERS

/*
typedef struct AVCodec {
    const char *name;
    enum CodecType type;
    int id;
    int priv_data_size;
    int (*init)(AVCodecContext *);
    int (*encode)(AVCodecContext *, uint8_t *buf, int buf_size, void *data);
    int (*close)(AVCodecContext *);
    int (*decode)(AVCodecContext *, void *outdata, int *outdata_size,
                  uint8_t *buf, int buf_size);
    int capabilities;
    const AVOption *options;
    struct AVCodec *next;
    void (*flush)(AVCodecContext *);
    const AVRational *supported_framerates; ///array of supported framerates, or NULL if any, array is terminated by {0,0}
    const enum PixelFormat *pix_fmts;       ///array of supported pixel formats, or NULL if unknown, array is terminanted by -1
} AVCodec;
*/


AVCodec mpeg1video_encoder = {
    "mpeg1video",
    CODEC_TYPE_VIDEO,
    CODEC_ID_MPEG1VIDEO,
    sizeof(MpegEncContext),
    encode_init,
    MPV_encode_picture,
    MPV_encode_end,
	NULL,
	CODEC_CAP_DELAY,
	NULL,
	NULL,
	NULL,
	frame_rate_tab+1,
	NULL
};

#ifdef CONFIG_RISKY

AVCodec mpeg2video_encoder = {
    "mpeg2video",
    CODEC_TYPE_VIDEO,
    CODEC_ID_MPEG2VIDEO,
    sizeof(MpegEncContext),
    encode_init,
    MPV_encode_picture,
    MPV_encode_end,
	NULL,
	CODEC_CAP_DELAY,
	NULL,
	NULL,
	NULL,
	frame_rate_tab+1,
	NULL
};
#endif
#endif




/* this is ugly i know, but the alternative is too make 
   hundreds of vars global and prefix them with ff_mpeg1_
   which is far uglier. */
//#include "mdec.c"  
