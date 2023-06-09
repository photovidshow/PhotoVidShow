;*****************************************************************************
;* mc-a.asm: h264 encoder library
;*****************************************************************************
;* Copyright (C) 2003-2008 x264 project
;*
;* Authors: Loren Merritt <lorenm@u.washington.edu>
;*          Jason Garrett-Glaser <darkshikari@gmail.com>
;*          Laurent Aimar <fenrir@via.ecp.fr>
;*          Dylan Yudaken <dyudaken@gmail.com>
;*          Min Chen <chenm001.163.com>
;*
;* This program is free software; you can redistribute it and/or modify
;* it under the terms of the GNU General Public License as published by
;* the Free Software Foundation; either version 2 of the License, or
;* (at your option) any later version.
;*
;* This program is distributed in the hope that it will be useful,
;* but WITHOUT ANY WARRANTY; without even the implied warranty of
;* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
;* GNU General Public License for more details.
;*
;* You should have received a copy of the GNU General Public License
;* along with this program; if not, write to the Free Software
;* Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02111, USA.
;*****************************************************************************

%include "x86inc.asm"

SECTION_RODATA 32

ch_shuffle: db 0,1,1,2,2,3,3,4,4,5,5,6,6,7,7,8,8,9,9,10,10,11,11,12,12,13,13,14,14,15,0,0

SECTION .text

cextern pw_1
cextern pw_4
cextern pw_8
cextern pw_32
cextern pw_64
cextern sw_64

;=============================================================================
; implicit weighted biprediction
;=============================================================================
; assumes log2_denom = 5, offset = 0, weight1 + weight2 = 64
%ifdef ARCH_X86_64
    DECLARE_REG_TMP 0,1,2,3,4,5,10,11
    %macro AVG_START 0-1 0
        PROLOGUE 6,7,%1
%ifdef WIN64
        movsxd r5, r5d
%endif
        .height_loop:
    %endmacro
%else
    DECLARE_REG_TMP 1,2,3,4,5,6,1,2
    %macro AVG_START 0-1 0
        PROLOGUE 0,7,%1
        mov t0, r0m
        mov t1, r1m
        mov t2, r2m
        mov t3, r3m
        mov t4, r4m
        mov t5, r5m
        .height_loop:
    %endmacro
%endif

%macro SPLATW 2-3 0
%if mmsize==16
    pshuflw  %1, %2, %3*0x55
    punpcklqdq %1, %1
%else
    pshufw   %1, %2, %3*0x55
%endif
%endmacro

%macro BIWEIGHT_MMX 2
    movh      m0, %1
    movh      m1, %2
    punpcklbw m0, m5
    punpcklbw m1, m5
    pmullw    m0, m2
    pmullw    m1, m3
    paddw     m0, m1
    paddw     m0, m4
    psraw     m0, 6
%endmacro

%macro BIWEIGHT_START_MMX 0
    movd    m2, r6m
    SPLATW  m2, m2   ; weight_dst
    mova    m3, [pw_64]
    psubw   m3, m2   ; weight_src
    mova    m4, [pw_32] ; rounding
    pxor    m5, m5
%endmacro

%macro BIWEIGHT_SSSE3 2
    movh      m0, %1
    movh      m1, %2
    punpcklbw m0, m1
    pmaddubsw m0, m3
    paddw     m0, m4
    psraw     m0, 6
%endmacro

%macro BIWEIGHT_START_SSSE3 0
    movzx  t6d, byte r6m ; FIXME x86_64
    mov    t7d, 64
    sub    t7d, t6d
    shl    t7d, 8
    add    t6d, t7d
    movd    m3, t6d
    mova    m4, [pw_32]
    SPLATW  m3, m3   ; weight_dst,src
%endmacro

%macro BIWEIGHT_ROW 4
    BIWEIGHT [%2], [%3]
%if %4==mmsize/2
    packuswb   m0, m0
    movh     [%1], m0
%else
    SWAP 0, 6
    BIWEIGHT [%2+mmsize/2], [%3+mmsize/2]
    packuswb   m6, m0
    mova     [%1], m6
%endif
%endmacro

;-----------------------------------------------------------------------------
; int pixel_avg_weight_w16( uint8_t *dst, int, uint8_t *src1, int, uint8_t *src2, int, int i_weight )
;-----------------------------------------------------------------------------
%macro AVG_WEIGHT 2-3 0
cglobal pixel_avg_weight_w%2_%1
    BIWEIGHT_START
    AVG_START %3
%if %2==8 && mmsize==16
    BIWEIGHT [t2], [t4]
    SWAP 0, 6
    BIWEIGHT [t2+t3], [t4+t5]
    packuswb m6, m0
    movlps   [t0], m6
    movhps   [t0+t1], m6
%else
%assign x 0
%rep 1+%2/(mmsize*2)
    BIWEIGHT_ROW t0+x,    t2+x,    t4+x,    %2
    BIWEIGHT_ROW t0+x+t1, t2+x+t3, t4+x+t5, %2
%assign x x+mmsize
%endrep
%endif
    lea  t0, [t0+t1*2]
    lea  t2, [t2+t3*2]
    lea  t4, [t4+t5*2]
    sub  eax, 2
    jg   .height_loop
    REP_RET
%endmacro

%define BIWEIGHT BIWEIGHT_MMX
%define BIWEIGHT_START BIWEIGHT_START_MMX
INIT_MMX
AVG_WEIGHT mmxext, 4
AVG_WEIGHT mmxext, 8
AVG_WEIGHT mmxext, 16
INIT_XMM
%define pixel_avg_weight_w4_sse2 pixel_avg_weight_w4_mmxext
AVG_WEIGHT sse2, 8,  7
AVG_WEIGHT sse2, 16, 7
%define BIWEIGHT BIWEIGHT_SSSE3
%define BIWEIGHT_START BIWEIGHT_START_SSSE3
INIT_MMX
AVG_WEIGHT ssse3, 4
INIT_XMM
AVG_WEIGHT ssse3, 8,  7
AVG_WEIGHT ssse3, 16, 7

;=============================================================================
; P frame explicit weighted prediction
;=============================================================================

%macro WEIGHT_START 1
    mova     m3, [r4]
    mova     m6, [r4+16]
    movd     m5, [r4+32]
    pxor     m2, m2
%if (%1 == 20 || %1 == 12) && mmsize == 16
    movdq2q mm3, xmm3
    movdq2q mm4, xmm4
    movdq2q mm5, xmm5
    movdq2q mm6, xmm6
    pxor    mm2, mm2
%endif
%endmacro

%macro WEIGHT_START_SSSE3 1
    mova     m3, [r4]
    mova     m4, [r4+16]
    pxor     m2, m2
%if %1 == 20 || %1 == 12
    movdq2q mm3, xmm3
    movdq2q mm4, xmm4
    pxor    mm2, mm2
%endif
%endmacro

;; macro to weight mmsize bytes taking half from %1 and half from %2
%macro WEIGHT 2             ; (src1,src2)
    movh      m0, [%1]
    movh      m1, [%2]
    punpcklbw m0, m2        ;setup
    punpcklbw m1, m2        ;setup
    pmullw    m0, m3        ;scale
    pmullw    m1, m3        ;scale
    paddsw    m0, m6        ;1<<(denom-1)+(offset<<denom)
    paddsw    m1, m6        ;1<<(denom-1)+(offset<<denom)
    psraw     m0, m5        ;denom
    psraw     m1, m5        ;denom
%endmacro

%macro WEIGHT_SSSE3 2
    movh      m0, [%1]
    movh      m1, [%2]
    punpcklbw m0, m2
    punpcklbw m1, m2
    psllw     m0, 7
    psllw     m1, 7
    pmulhrsw  m0, m3
    pmulhrsw  m1, m3
    paddw     m0, m4
    paddw     m1, m4
%endmacro

%macro WEIGHT_SAVE_ROW 3        ;(src,dst,width)
%if %3 == 16
    mova     [%2], %1
%elif %3 == 8
    movq     [%2], %1
%else
    movd     [%2], %1       ; width 2 can write garbage for last 2 bytes
%endif
%endmacro

%macro WEIGHT_ROW 3         ; (src,dst,width)
    ;; load weights
    WEIGHT           %1, (%1+(mmsize/2))
    packuswb         m0, m1        ;put bytes into m0
    WEIGHT_SAVE_ROW  m0, %2, %3
%endmacro

%macro WEIGHT_SAVE_COL 2        ;(dst,size)
%if %2 == 8
    packuswb     m0, m1
    movq       [%1], m0
    movhps  [%1+r1], m0
%else
    packuswb     m0, m0
    packuswb     m1, m1
    movd       [%1], m0    ; width 2 can write garbage for last 2 bytes
    movd    [%1+r1], m1
%endif
%endmacro

%macro WEIGHT_COL 3     ; (src,dst,width)
%if %3 <= 4 && mmsize == 16
    INIT_MMX
    ;; load weights
    WEIGHT           %1, (%1+r3)
    WEIGHT_SAVE_COL  %2, %3
    INIT_XMM
%else
    WEIGHT           %1, (%1+r3)
    WEIGHT_SAVE_COL  %2, %3
%endif

%endmacro

%macro WEIGHT_TWO_ROW 3 ; (src,dst,width)
%assign x 0
%rep %3
%if (%3-x) >= mmsize
    WEIGHT_ROW    (%1+x),    (%2+x), mmsize     ; weight 1 mmsize
    WEIGHT_ROW (%1+r3+x), (%2+r1+x), mmsize     ; weight 1 mmsize
    %assign x (x+mmsize)
%else
    WEIGHT_COL (%1+x),(%2+x),(%3-x)
    %exitrep
%endif
%if x >= %3
    %exitrep
%endif
%endrep
%endmacro

;-----------------------------------------------------------------------------
;void mc_weight_wX( uint8_t *dst, int i_dst_stride, uint8_t *src, int i_src_stride, weight_t *weight, int h )
;-----------------------------------------------------------------------------

%ifdef ARCH_X86_64
%define NUMREGS 6
%define LOAD_HEIGHT
%define HEIGHT_REG r5d
%else
%define NUMREGS 5
%define LOAD_HEIGHT mov r4d, r5m
%define HEIGHT_REG r4d
%endif

%macro WEIGHTER 2
    cglobal mc_weight_w%1_%2, NUMREGS, NUMREGS, 7
    WEIGHT_START %1
    LOAD_HEIGHT
.loop:
    WEIGHT_TWO_ROW r2, r0, %1
    lea  r0, [r0+r1*2]
    lea  r2, [r2+r3*2]
    sub HEIGHT_REG, 2
    jg .loop
    REP_RET
%endmacro

INIT_MMX
WEIGHTER  4, mmxext
WEIGHTER  8, mmxext
WEIGHTER 12, mmxext
WEIGHTER 16, mmxext
WEIGHTER 20, mmxext
INIT_XMM
WEIGHTER  8, sse2
WEIGHTER 16, sse2
WEIGHTER 20, sse2
%define WEIGHT WEIGHT_SSSE3
%define WEIGHT_START WEIGHT_START_SSSE3
INIT_MMX
WEIGHTER  4, ssse3
INIT_XMM
WEIGHTER  8, ssse3
WEIGHTER 16, ssse3
WEIGHTER 20, ssse3

%macro OFFSET_OP 7
    mov%6        m0, [%1]
    mov%6        m1, [%2]
    p%5usb       m0, m2
    p%5usb       m1, m2
    mov%7      [%3], m0
    mov%7      [%4], m1
%endmacro

%macro OFFSET_TWO_ROW 4
%assign x 0
%rep %3
%if (%3-x) >= mmsize
    OFFSET_OP (%1+x), (%1+x+r3), (%2+x), (%2+x+r1), %4, u, a
    %assign x (x+mmsize)
%else
    OFFSET_OP (%1+x),(%1+x+r3), (%2+x), (%2+x+r1), %4, d, d
    %exitrep
%endif
%if x >= %3
    %exitrep
%endif
%endrep
%endmacro

;-----------------------------------------------------------------------------
;void mc_offset_wX( uint8_t *src, int i_src_stride, uint8_t *dst, int i_dst_stride, weight_t *w, int h )
;-----------------------------------------------------------------------------
%macro OFFSET 3
    cglobal mc_offset%3_w%1_%2, NUMREGS, NUMREGS
    mova m2, [r4]
    LOAD_HEIGHT
.loop:
    OFFSET_TWO_ROW r2, r0, %1, %3
    lea  r0, [r0+r1*2]
    lea  r2, [r2+r3*2]
    sub HEIGHT_REG, 2
    jg .loop
    REP_RET
%endmacro

%macro OFFSETPN 2
       OFFSET %1, %2, add
       OFFSET %1, %2, sub
%endmacro
INIT_MMX
OFFSETPN  4, mmxext
OFFSETPN  8, mmxext
OFFSETPN 12, mmxext
OFFSETPN 16, mmxext
OFFSETPN 20, mmxext
INIT_XMM
OFFSETPN 12, sse2
OFFSETPN 16, sse2
OFFSETPN 20, sse2
%undef LOAD_HEIGHT
%undef HEIGHT_REG
%undef NUMREGS



;=============================================================================
; pixel avg
;=============================================================================

;-----------------------------------------------------------------------------
; void pixel_avg_4x4( uint8_t *dst, int dst_stride,
;                     uint8_t *src1, int src1_stride, uint8_t *src2, int src2_stride, int weight );
;-----------------------------------------------------------------------------
%macro AVGH 3
cglobal pixel_avg_%1x%2_%3
    mov eax, %2
    cmp dword r6m, 32
    jne pixel_avg_weight_w%1_%3
%if mmsize == 16 && %1 == 16
    test dword r4m, 15
    jz pixel_avg_w%1_sse2
%endif
    jmp pixel_avg_w%1_mmxext
%endmacro

;-----------------------------------------------------------------------------
; void pixel_avg_w4( uint8_t *dst, int dst_stride,
;                    uint8_t *src1, int src1_stride, uint8_t *src2, int src2_stride,
;                    int height, int weight );
;-----------------------------------------------------------------------------

%macro AVG_END 0
    sub    eax, 2
    lea    t4, [t4+t5*2]
    lea    t2, [t2+t3*2]
    lea    t0, [t0+t1*2]
    jg     .height_loop
    REP_RET
%endmacro

%macro AVG_FUNC 3
cglobal %1
    AVG_START
    %2     m0, [t2]
    %2     m1, [t2+t3]
    pavgb  m0, [t4]
    pavgb  m1, [t4+t5]
    %3     [t0], m0
    %3     [t0+t1], m1
    AVG_END
%endmacro

INIT_MMX
AVG_FUNC pixel_avg_w4_mmxext, movd, movd
AVGH 4, 8, mmxext
AVGH 4, 4, mmxext
AVGH 4, 2, mmxext

AVG_FUNC pixel_avg_w8_mmxext, movq, movq
AVGH 8, 16, mmxext
AVGH 8, 8,  mmxext
AVGH 8, 4,  mmxext

cglobal pixel_avg_w16_mmxext
    AVG_START
    movq   mm0, [t2  ]
    movq   mm1, [t2+8]
    movq   mm2, [t2+t3  ]
    movq   mm3, [t2+t3+8]
    pavgb  mm0, [t4  ]
    pavgb  mm1, [t4+8]
    pavgb  mm2, [t4+t5  ]
    pavgb  mm3, [t4+t5+8]
    movq   [t0  ], mm0
    movq   [t0+8], mm1
    movq   [t0+t1  ], mm2
    movq   [t0+t1+8], mm3
    AVG_END

AVGH 16, 16, mmxext
AVGH 16, 8,  mmxext

INIT_XMM
AVG_FUNC pixel_avg_w16_sse2, movdqu, movdqa
AVGH 16, 16, sse2
AVGH 16,  8, sse2
AVGH  8, 16, sse2
AVGH  8,  8, sse2
AVGH  8,  4, sse2
AVGH 16, 16, ssse3
AVGH 16,  8, ssse3
AVGH  8, 16, ssse3
AVGH  8,  8, ssse3
AVGH  8,  4, ssse3
INIT_MMX
AVGH  4,  8, ssse3
AVGH  4,  4, ssse3
AVGH  4,  2, ssse3



;=============================================================================
; pixel avg2
;=============================================================================

;-----------------------------------------------------------------------------
; void pixel_avg2_w4( uint8_t *dst, int dst_stride,
;                     uint8_t *src1, int src_stride,
;                     uint8_t *src2, int height );
;-----------------------------------------------------------------------------
%macro AVG2_W8 2
cglobal pixel_avg2_w%1_mmxext, 6,7
    sub    r4, r2
    lea    r6, [r4+r3]
.height_loop:
    %2     mm0, [r2]
    %2     mm1, [r2+r3]
    pavgb  mm0, [r2+r4]
    pavgb  mm1, [r2+r6]
    %2     [r0], mm0
    %2     [r0+r1], mm1
    sub    r5d, 2
    lea    r2, [r2+r3*2]
    lea    r0, [r0+r1*2]
    jg     .height_loop
    REP_RET
%endmacro

AVG2_W8 4, movd
AVG2_W8 8, movq

%macro AVG2_W16 2
cglobal pixel_avg2_w%1_mmxext, 6,7
    sub    r4, r2
    lea    r6, [r4+r3]
.height_loop:
    movq   mm0, [r2]
    %2     mm1, [r2+8]
    movq   mm2, [r2+r3]
    %2     mm3, [r2+r3+8]
    pavgb  mm0, [r2+r4]
    pavgb  mm1, [r2+r4+8]
    pavgb  mm2, [r2+r6]
    pavgb  mm3, [r2+r6+8]
    movq   [r0], mm0
    %2     [r0+8], mm1
    movq   [r0+r1], mm2
    %2     [r0+r1+8], mm3
    lea    r2, [r2+r3*2]
    lea    r0, [r0+r1*2]
    sub    r5d, 2
    jg     .height_loop
    REP_RET
%endmacro

AVG2_W16 12, movd
AVG2_W16 16, movq

cglobal pixel_avg2_w20_mmxext, 6,7
    sub    r4, r2
    lea    r6, [r4+r3]
.height_loop:
    movq   mm0, [r2]
    movq   mm1, [r2+8]
    movd   mm2, [r2+16]
    movq   mm3, [r2+r3]
    movq   mm4, [r2+r3+8]
    movd   mm5, [r2+r3+16]
    pavgb  mm0, [r2+r4]
    pavgb  mm1, [r2+r4+8]
    pavgb  mm2, [r2+r4+16]
    pavgb  mm3, [r2+r6]
    pavgb  mm4, [r2+r6+8]
    pavgb  mm5, [r2+r6+16]
    movq   [r0], mm0
    movq   [r0+8], mm1
    movd   [r0+16], mm2
    movq   [r0+r1], mm3
    movq   [r0+r1+8], mm4
    movd   [r0+r1+16], mm5
    lea    r2, [r2+r3*2]
    lea    r0, [r0+r1*2]
    sub    r5d, 2
    jg     .height_loop
    REP_RET

cglobal pixel_avg2_w16_sse2, 6,7
    sub    r4, r2
    lea    r6, [r4+r3]
.height_loop:
    movdqu xmm0, [r2]
    movdqu xmm2, [r2+r3]
    movdqu xmm1, [r2+r4]
    movdqu xmm3, [r2+r6]
    pavgb  xmm0, xmm1
    pavgb  xmm2, xmm3
    movdqa [r0], xmm0
    movdqa [r0+r1], xmm2
    lea    r2, [r2+r3*2]
    lea    r0, [r0+r1*2]
    sub    r5d, 2
    jg     .height_loop
    REP_RET

%macro AVG2_W20 1
cglobal pixel_avg2_w20_%1, 6,7
    sub    r4, r2
    lea    r6, [r4+r3]
.height_loop:
    movdqu xmm0, [r2]
    movdqu xmm2, [r2+r3]
    movd   mm4,  [r2+16]
    movd   mm5,  [r2+r3+16]
%ifidn %1, sse2_misalign
    pavgb  xmm0, [r2+r4]
    pavgb  xmm2, [r2+r6]
%else
    movdqu xmm1, [r2+r4]
    movdqu xmm3, [r2+r6]
    pavgb  xmm0, xmm1
    pavgb  xmm2, xmm3
%endif
    pavgb  mm4,  [r2+r4+16]
    pavgb  mm5,  [r2+r6+16]
    movdqa [r0], xmm0
    movd   [r0+16], mm4
    movdqa [r0+r1], xmm2
    movd   [r0+r1+16], mm5
    lea    r2, [r2+r3*2]
    lea    r0, [r0+r1*2]
    sub    r5d, 2
    jg     .height_loop
    REP_RET
%endmacro

AVG2_W20 sse2
AVG2_W20 sse2_misalign

; Cacheline split code for processors with high latencies for loads
; split over cache lines.  See sad-a.asm for a more detailed explanation.
; This particular instance is complicated by the fact that src1 and src2
; can have different alignments.  For simplicity and code size, only the
; MMX cacheline workaround is used.  As a result, in the case of SSE2
; pixel_avg, the cacheline check functions calls the SSE2 version if there
; is no cacheline split, and the MMX workaround if there is.

%macro INIT_SHIFT 2
    and    eax, 7
    shl    eax, 3
    movd   %1, [sw_64]
    movd   %2, eax
    psubw  %1, %2
%endmacro

%macro AVG_CACHELINE_CHECK 3 ; width, cacheline, instruction set
cglobal pixel_avg2_w%1_cache%2_%3
    mov    eax, r2m
    and    eax, 0x1f|(%2>>1)
    cmp    eax, (32-%1)|(%2>>1)
    jle pixel_avg2_w%1_%3
;w12 isn't needed because w16 is just as fast if there's no cacheline split
%if %1 == 12
    jmp pixel_avg2_w16_cache_mmxext
%else
    jmp pixel_avg2_w%1_cache_mmxext
%endif
%endmacro

%macro AVG_CACHELINE_START 0
    %assign stack_offset 0
    INIT_SHIFT mm6, mm7
    mov    eax, r4m
    INIT_SHIFT mm4, mm5
    PROLOGUE 6,6
    and    r2, ~7
    and    r4, ~7
    sub    r4, r2
.height_loop:
%endmacro

%macro AVG_CACHELINE_LOOP 2
    movq   mm0, [r2+8+%1]
    movq   mm1, [r2+%1]
    movq   mm2, [r2+r4+8+%1]
    movq   mm3, [r2+r4+%1]
    psllq  mm0, mm6
    psrlq  mm1, mm7
    psllq  mm2, mm4
    psrlq  mm3, mm5
    por    mm0, mm1
    por    mm2, mm3
    pavgb  mm0, mm2
    %2 [r0+%1], mm0
%endmacro

pixel_avg2_w8_cache_mmxext:
    AVG_CACHELINE_START
    AVG_CACHELINE_LOOP 0, movq
    add    r2, r3
    add    r0, r1
    dec    r5d
    jg     .height_loop
    REP_RET

pixel_avg2_w16_cache_mmxext:
    AVG_CACHELINE_START
    AVG_CACHELINE_LOOP 0, movq
    AVG_CACHELINE_LOOP 8, movq
    add    r2, r3
    add    r0, r1
    dec    r5d
    jg .height_loop
    REP_RET

pixel_avg2_w20_cache_mmxext:
    AVG_CACHELINE_START
    AVG_CACHELINE_LOOP 0, movq
    AVG_CACHELINE_LOOP 8, movq
    AVG_CACHELINE_LOOP 16, movd
    add    r2, r3
    add    r0, r1
    dec    r5d
    jg .height_loop
    REP_RET

%ifndef ARCH_X86_64
AVG_CACHELINE_CHECK  8, 32, mmxext
AVG_CACHELINE_CHECK 12, 32, mmxext
AVG_CACHELINE_CHECK 16, 32, mmxext
AVG_CACHELINE_CHECK 20, 32, mmxext
AVG_CACHELINE_CHECK 16, 64, mmxext
AVG_CACHELINE_CHECK 20, 64, mmxext
%endif

AVG_CACHELINE_CHECK  8, 64, mmxext
AVG_CACHELINE_CHECK 12, 64, mmxext
AVG_CACHELINE_CHECK 16, 64, sse2
AVG_CACHELINE_CHECK 20, 64, sse2

; computed jump assumes this loop is exactly 48 bytes
%macro AVG16_CACHELINE_LOOP_SSSE3 2 ; alignment
ALIGN 16
avg_w16_align%1_%2_ssse3:
%if %2&15==0
    movdqa  xmm1, [r2+16]
    palignr xmm1, [r2], %1
    pavgb   xmm1, [r2+r4]
%else
    movdqa  xmm1, [r2+16]
    movdqa  xmm2, [r2+r4+16]
    palignr xmm1, [r2], %1
    palignr xmm2, [r2+r4], %2
    pavgb   xmm1, xmm2
%endif
    movdqa  [r0], xmm1
    add    r2, r3
    add    r0, r1
    dec    r5d
    jg     avg_w16_align%1_%2_ssse3
    rep ret
%endmacro

cglobal pixel_avg2_w16_cache64_ssse3
    mov    eax, r2m
    and    eax, 0x3f
    cmp    eax, 0x30
    jle pixel_avg2_w16_sse2
    PROLOGUE 6,7
    lea    r6, [r4+r2]
    and    r4, ~0xf
    and    r6, 0x1f
    and    r2, ~0xf
    lea    r6, [r6*3]    ;(offset + align*2)*3
    sub    r4, r2
    shl    r6, 4         ;jump = (offset + align*2)*48
%define avg_w16_addr avg_w16_align1_1_ssse3-(avg_w16_align2_2_ssse3-avg_w16_align1_1_ssse3)
%ifdef PIC
    lea   r11, [avg_w16_addr]
    add    r6, r11
%else
    lea    r6, [avg_w16_addr + r6]
%endif
%ifdef UNIX64
    jmp    r6
%else
    call   r6
    RET
%endif

%assign j 1
%assign k 2
%rep 15
AVG16_CACHELINE_LOOP_SSSE3 j, j
AVG16_CACHELINE_LOOP_SSSE3 j, k
%assign j j+1
%assign k k+1
%endrep

;=============================================================================
; pixel copy
;=============================================================================

%macro COPY4 4
    %2  m0, [r2]
    %2  m1, [r2+r3]
    %2  m2, [r2+r3*2]
    %2  m3, [r2+%4]
    %1  [r0],      m0
    %1  [r0+r1],   m1
    %1  [r0+r1*2], m2
    %1  [r0+%3],   m3
%endmacro

INIT_MMX
;-----------------------------------------------------------------------------
; void mc_copy_w4( uint8_t *dst, int i_dst_stride,
;                  uint8_t *src, int i_src_stride, int i_height )
;-----------------------------------------------------------------------------
cglobal mc_copy_w4_mmx, 4,6
    cmp     dword r4m, 4
    lea     r5, [r3*3]
    lea     r4, [r1*3]
    je .end
    COPY4 movd, movd, r4, r5
    lea     r2, [r2+r3*4]
    lea     r0, [r0+r1*4]
.end:
    COPY4 movd, movd, r4, r5
    RET

cglobal mc_copy_w8_mmx, 5,7
    lea     r6, [r3*3]
    lea     r5, [r1*3]
.height_loop:
    COPY4 movq, movq, r5, r6
    lea     r2, [r2+r3*4]
    lea     r0, [r0+r1*4]
    sub     r4d, 4
    jg      .height_loop
    REP_RET

cglobal mc_copy_w16_mmx, 5,7
    lea     r6, [r3*3]
    lea     r5, [r1*3]
.height_loop:
    movq    mm0, [r2]
    movq    mm1, [r2+8]
    movq    mm2, [r2+r3]
    movq    mm3, [r2+r3+8]
    movq    mm4, [r2+r3*2]
    movq    mm5, [r2+r3*2+8]
    movq    mm6, [r2+r6]
    movq    mm7, [r2+r6+8]
    movq    [r0], mm0
    movq    [r0+8], mm1
    movq    [r0+r1], mm2
    movq    [r0+r1+8], mm3
    movq    [r0+r1*2], mm4
    movq    [r0+r1*2+8], mm5
    movq    [r0+r5], mm6
    movq    [r0+r5+8], mm7
    lea     r2, [r2+r3*4]
    lea     r0, [r0+r1*4]
    sub     r4d, 4
    jg      .height_loop
    REP_RET

INIT_XMM
%macro COPY_W16_SSE2 2
cglobal %1, 5,7
    lea     r6, [r3*3]
    lea     r5, [r1*3]
.height_loop:
    COPY4 movdqa, %2, r5, r6
    lea     r2, [r2+r3*4]
    lea     r0, [r0+r1*4]
    sub     r4d, 4
    jg      .height_loop
    REP_RET
%endmacro

COPY_W16_SSE2 mc_copy_w16_sse2, movdqu
; cacheline split with mmx has too much overhead; the speed benefit is near-zero.
; but with SSE3 the overhead is zero, so there's no reason not to include it.
COPY_W16_SSE2 mc_copy_w16_sse3, lddqu
COPY_W16_SSE2 mc_copy_w16_aligned_sse2, movdqa



;=============================================================================
; prefetch
;=============================================================================
; FIXME assumes 64 byte cachelines

;-----------------------------------------------------------------------------
; void prefetch_fenc( uint8_t *pix_y, int stride_y,
;                     uint8_t *pix_uv, int stride_uv, int mb_x )
;-----------------------------------------------------------------------------
%ifdef ARCH_X86_64
cglobal prefetch_fenc_mmxext, 5,5
    mov    eax, r4d
    and    eax, 3
    imul   eax, r1d
    lea    r0,  [r0+rax*4+64]
    prefetcht0  [r0]
    prefetcht0  [r0+r1]
    lea    r0,  [r0+r1*2]
    prefetcht0  [r0]
    prefetcht0  [r0+r1]

    and    r4d, 6
    imul   r4d, r3d
    lea    r2,  [r2+r4+64]
    prefetcht0  [r2]
    prefetcht0  [r2+r3]
    RET

%else
cglobal prefetch_fenc_mmxext
    mov    r2, [esp+20]
    mov    r1, [esp+8]
    mov    r0, [esp+4]
    and    r2, 3
    imul   r2, r1
    lea    r0, [r0+r2*4+64]
    prefetcht0 [r0]
    prefetcht0 [r0+r1]
    lea    r0, [r0+r1*2]
    prefetcht0 [r0]
    prefetcht0 [r0+r1]

    mov    r2, [esp+20]
    mov    r1, [esp+16]
    mov    r0, [esp+12]
    and    r2, 6
    imul   r2, r1
    lea    r0, [r0+r2+64]
    prefetcht0 [r0]
    prefetcht0 [r0+r1]
    ret
%endif ; ARCH_X86_64

;-----------------------------------------------------------------------------
; void prefetch_ref( uint8_t *pix, int stride, int parity )
;-----------------------------------------------------------------------------
cglobal prefetch_ref_mmxext, 3,3
    dec    r2d
    and    r2d, r1d
    lea    r0,  [r0+r2*8+64]
    lea    r2,  [r1*3]
    prefetcht0  [r0]
    prefetcht0  [r0+r1]
    prefetcht0  [r0+r1*2]
    prefetcht0  [r0+r2]
    lea    r0,  [r0+r1*4]
    prefetcht0  [r0]
    prefetcht0  [r0+r1]
    prefetcht0  [r0+r1*2]
    prefetcht0  [r0+r2]
    RET



;=============================================================================
; chroma MC
;=============================================================================

    %define t0 rax
%ifdef ARCH_X86_64
    %define t1 r10
%else
    %define t1 r1
%endif

%macro MC_CHROMA_START 0
    movifnidn r2,  r2mp
    movifnidn r3d, r3m
    movifnidn r4d, r4m
    movifnidn r5d, r5m
    mov       t0d, r5d
    mov       t1d, r4d
    sar       t0d, 3
    sar       t1d, 3
    imul      t0d, r3d
    add       t0d, t1d
    movsxdifnidn t0, t0d
    add       r2,  t0            ; src += (dx>>3) + (dy>>3) * src_stride
%endmacro

;-----------------------------------------------------------------------------
; void mc_chroma( uint8_t *dst, int dst_stride,
;                 uint8_t *src, int src_stride,
;                 int dx, int dy,
;                 int width, int height )
;-----------------------------------------------------------------------------
%macro MC_CHROMA 1-2 0
cglobal mc_chroma_%1
%if mmsize == 16
    cmp dword r6m, 4
    jle mc_chroma_mmxext
%endif
    PROLOGUE 0,6,%2
    MC_CHROMA_START
    pxor       m3, m3
    and       r4d, 7         ; dx &= 7
    jz .mc1dy
    and       r5d, 7         ; dy &= 7
    jz .mc1dx

    movd       m5, r4d
    movd       m6, r5d
    SPLATW     m5, m5        ; m5 = dx
    SPLATW     m6, m6        ; m6 = dy

    mova       m4, [pw_8]
    mova       m0, m4
    psubw      m4, m5        ; m4 = 8-dx
    psubw      m0, m6        ; m0 = 8-dy

    mova       m7, m5
    pmullw     m5, m0        ; m5 = dx*(8-dy) =     cB
    pmullw     m7, m6        ; m7 = dx*dy =         cD
    pmullw     m6, m4        ; m6 = (8-dx)*dy =     cC
    pmullw     m4, m0        ; m4 = (8-dx)*(8-dy) = cA

    mov       r4d, r7m
%ifdef ARCH_X86_64
    mov       r10, r0
    mov       r11, r2
%else
    mov        r0, r0mp
    mov        r1, r1m
    mov        r5, r2
%endif

.loop2d:
    movh       m1, [r2+r3]
    movh       m0, [r2]
    punpcklbw  m1, m3        ; 00 px1 | 00 px2 | 00 px3 | 00 px4
    punpcklbw  m0, m3
    pmullw     m1, m6        ; 2nd line * cC
    pmullw     m0, m4        ; 1st line * cA
    paddw      m0, m1        ; m0 <- result

    movh       m2, [r2+1]
    movh       m1, [r2+r3+1]
    punpcklbw  m2, m3
    punpcklbw  m1, m3

    paddw      m0, [pw_32]

    pmullw     m2, m5        ; line * cB
    pmullw     m1, m7        ; line * cD
    paddw      m0, m2
    paddw      m0, m1
    psrlw      m0, 6

    packuswb m0, m3          ; 00 00 00 00 px1 px2 px3 px4
    movh       [r0], m0

    add        r2,  r3
    add        r0,  r1       ; dst_stride
    dec        r4d
    jnz .loop2d

%if mmsize == 8
    sub dword r6m, 8
    jnz .finish              ; width != 8 so assume 4
%ifdef ARCH_X86_64
    lea        r0, [r10+4]   ; dst
    lea        r2, [r11+4]   ; src
%else
    mov        r0, r0mp
    lea        r2, [r5+4]
    add        r0, 4
%endif
    mov       r4d, r7m       ; height
    jmp .loop2d
%else
    REP_RET
%endif ; mmsize

.mc1dy:
    and       r5d, 7
    movd       m6, r5d
    mov        r5, r3        ; pel_offset = dx ? 1 : src_stride
    jmp .mc1d
.mc1dx:
    movd       m6, r4d
    mov       r5d, 1
.mc1d:
    mova       m5, [pw_8]
    SPLATW     m6, m6
    mova       m7, [pw_4]
    psubw      m5, m6
    movifnidn r0,  r0mp
    movifnidn r1d, r1m
    mov       r4d, r7m
%if mmsize == 8
    cmp dword r6m, 8
    je .loop1d_w8
%endif

.loop1d_w4:
    movh       m0, [r2+r5]
    movh       m1, [r2]
    punpcklbw  m0, m3
    punpcklbw  m1, m3
    pmullw     m0, m6
    pmullw     m1, m5
    paddw      m0, m7
    paddw      m0, m1
    psrlw      m0, 3
    packuswb   m0, m3
    movh     [r0], m0
    add        r2, r3
    add        r0, r1
    dec        r4d
    jnz .loop1d_w4
.finish:
    REP_RET

%if mmsize == 8
.loop1d_w8:
    movu       m0, [r2+r5]
    mova       m1, [r2]
    mova       m2, m0
    mova       m4, m1
    punpcklbw  m0, m3
    punpcklbw  m1, m3
    punpckhbw  m2, m3
    punpckhbw  m4, m3
    pmullw     m0, m6
    pmullw     m1, m5
    pmullw     m2, m6
    pmullw     m4, m5
    paddw      m0, m7
    paddw      m2, m7
    paddw      m0, m1
    paddw      m2, m4
    psrlw      m0, 3
    psrlw      m2, 3
    packuswb   m0, m2
    mova     [r0], m0
    add        r2, r3
    add        r0, r1
    dec        r4d
    jnz .loop1d_w8
    REP_RET
%endif ; mmsize
%endmacro ; MC_CHROMA

INIT_MMX
MC_CHROMA mmxext
INIT_XMM
MC_CHROMA sse2, 8

%macro MC_CHROMA_SSSE3 2
INIT_MMX
cglobal mc_chroma_ssse3%1, 0,6,%2
    MC_CHROMA_START
    and       r4d, 7
    and       r5d, 7
    mov       t0d, r4d
    shl       t0d, 8
    sub       t0d, r4d
    mov       r4d, 8
    add       t0d, 8
    sub       r4d, r5d
    imul      r5d, t0d ; (x*255+8)*y
    imul      r4d, t0d ; (x*255+8)*(8-y)
    cmp dword r6m, 4
    jg .width8
    mova       m5, [pw_32]
    movd       m6, r5d
    movd       m7, r4d
    movifnidn  r0, r0mp
    movifnidn r1d, r1m
    movifnidn r4d, r7m
    SPLATW     m6, m6
    SPLATW     m7, m7
    mov        r5, r2
    and        r2, ~3
    and        r5, 3
%ifdef PIC
    lea       r11, [ch_shuffle]
    movu       m5, [r11 + r5*2]
%else
    movu       m5, [ch_shuffle + r5*2]
%endif
    movu       m0, [r2]
    pshufb     m0, m5
.loop4:
    movu       m1, [r2+r3]
    pshufb     m1, m5
    movu       m3, [r2+2*r3]
    pshufb     m3, m5
    lea        r2, [r2+2*r3]
    mova       m2, m1
    mova       m4, m3
    pmaddubsw  m0, m7
    pmaddubsw  m1, m6
    pmaddubsw  m2, m7
    pmaddubsw  m3, m6
    paddw      m0, [pw_32]
    paddw      m2, [pw_32]
    paddw      m1, m0
    paddw      m3, m2
    mova       m0, m4
    psrlw      m1, 6
    psrlw      m3, 6
    packuswb   m1, m1
    packuswb   m3, m3
    movh     [r0], m1
    movh  [r0+r1], m3
    sub       r4d, 2
    lea        r0, [r0+2*r1]
    jg .loop4
    REP_RET

INIT_XMM
.width8:
    movd       m6, r5d
    movd       m7, r4d
    movifnidn  r0, r0mp
    movifnidn r1d, r1m
    movifnidn r4d, r7m
    SPLATW     m6, m6
    SPLATW     m7, m7
%ifidn %1, _cache64
    mov        r5, r2
    and        r5, 0x3f
    cmp        r5, 0x38
    jge .split
%endif
    mova       m5, [pw_32]
    movh       m0, [r2]
    movh       m1, [r2+1]
    punpcklbw  m0, m1
.loop8:
    movh       m1, [r2+1*r3]
    movh       m2, [r2+1*r3+1]
    movh       m3, [r2+2*r3]
    movh       m4, [r2+2*r3+1]
    punpcklbw  m1, m2
    punpcklbw  m3, m4
    lea        r2, [r2+2*r3]
    mova       m2, m1
    mova       m4, m3
    pmaddubsw  m0, m7
    pmaddubsw  m1, m6
    pmaddubsw  m2, m7
    pmaddubsw  m3, m6
    paddw      m0, m5
    paddw      m2, m5
    paddw      m1, m0
    paddw      m3, m2
    mova       m0, m4
    psrlw      m1, 6
    psrlw      m3, 6
    packuswb   m1, m3
    movh     [r0], m1
    movhps [r0+r1], m1
    sub       r4d, 2
    lea        r0, [r0+2*r1]
    jg .loop8
    REP_RET
%ifidn %1, _cache64
.split:
    and        r2, ~7
    and        r5, 7
%ifdef PIC
    lea       r11, [ch_shuffle]
    movu       m5, [r11 + r5*2]
%else
    movu       m5, [ch_shuffle + r5*2]
%endif
    movu       m0, [r2]
    pshufb     m0, m5
%ifdef ARCH_X86_64
    mova       m8, [pw_32]
    %define round m8
%else
    %define round [pw_32]
%endif
.splitloop8:
    movu       m1, [r2+r3]
    pshufb     m1, m5
    movu       m3, [r2+2*r3]
    pshufb     m3, m5
    lea        r2, [r2+2*r3]
    mova       m2, m1
    mova       m4, m3
    pmaddubsw  m0, m7
    pmaddubsw  m1, m6
    pmaddubsw  m2, m7
    pmaddubsw  m3, m6
    paddw      m0, round
    paddw      m2, round
    paddw      m1, m0
    paddw      m3, m2
    mova       m0, m4
    psrlw      m1, 6
    psrlw      m3, 6
    packuswb   m1, m3
    movh     [r0], m1
    movhps [r0+r1], m1
    sub       r4d, 2
    lea        r0, [r0+2*r1]
    jg .splitloop8
    REP_RET
%endif
; mc_chroma 1d ssse3 is negligibly faster, and definitely not worth the extra code size
%endmacro

MC_CHROMA_SSSE3 , 8
MC_CHROMA_SSSE3 _cache64, 9
