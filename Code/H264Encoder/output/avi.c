/*****************************************************************************
 * avi.c: x264 avi output module
 *****************************************************************************
 * Copyright (C) 2009 Zhou Zongyi <zhouzy@os.pku.edu.cn>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02111, USA.
 *****************************************************************************/

#include "muxers.h"

#ifdef MAKEFOURCC
#undef MAKEFOURCC
#endif

#ifndef WORDS_BIGENDIAN
#define MAKEFOURCC(ch0, ch1, ch2, ch3) \
                ((uint32_t)(uint8_t)(ch0) | ((uint32_t)(uint8_t)(ch1) << 8) | \
                ((uint32_t)(uint8_t)(ch2) << 16) | ((uint32_t)(uint8_t)(ch3) << 24 ))
#define LE32(x) (x)
#define LE16(x) (x)
#else
#define MAKEFOURCC(ch0, ch1, ch2, ch3) \
                ((uint32_t)(uint8_t)(ch3) | ((uint32_t)(uint8_t)(ch2) << 8) | \
                ((uint32_t)(uint8_t)(ch1) << 16) | ((uint32_t)(uint8_t)(ch0) << 24 ))

uint32_t inline LE32(uint32_t x)
{
    return (x>>24) + ((x>>8)&0xFF00) + ((x<<8)&0xFF0000) + (x<<24);
}

uint16_t inline LE16(uint16_t x)
{
    return (x>>8) + (x<<8);
}
#endif

typedef struct
{
    uint32_t dwList;
    uint32_t dwSize;
    uint32_t fcc;
} list_header_t;

typedef struct
{
    list_header_t riff_hdr;
    list_header_t hdrl_hdr;
    uint32_t avih;
    uint32_t avih_size;
    uint32_t dwMicroSecPerFrame; // frame display rate (or 0)
    uint32_t dwMaxBytesPerSec; // max. transfer rate
    uint32_t dwPaddingGranularity; // pad to multiples of this
    uint32_t dwavihFlags; // the ever-present flags
    uint32_t dwTotalFrames; // # frames in file
    uint32_t dwavihInitialFrames;
    uint32_t dwStreams;
    uint32_t dwavihSuggestedBufferSize;
    uint32_t dwWidth;
    uint32_t dwHeight;
    uint32_t dwReserved[4];

    list_header_t strl_hdr;
    uint32_t strh;
    uint32_t strh_size;
    uint32_t fccType;
    uint32_t fccHandler;
    uint32_t dwstrhFlags;
    uint16_t wPriority;
    uint16_t wLanguage;
    uint32_t dwstrhInitialFrames;
    uint32_t dwScale;
    uint32_t dwRate; /* dwRate / dwScale == samples/second */
    uint32_t dwStart;
    uint32_t dwLength; /* In units above... */
    uint32_t dwstrhSuggestedBufferSize;
    uint32_t dwQuality;
    uint32_t dwSampleSize;
    uint16_t rcFrameLeft, rcFrameTop, rcFrameRight, rcFrameBottom;

    uint32_t strf;
    uint32_t strf_size;
    uint32_t biSize;
    uint32_t biWidth;
    int32_t  biHeight;
    uint16_t biPlanes;
    uint16_t biBitCount;
    uint32_t biCompression;
    uint32_t biSizeImage;
    int32_t  biXPelsPerMeter;
    int32_t  biYPelsPerMeter;
    uint32_t biClrUsed;
    uint32_t biClrImportant;

    list_header_t movi_hdr;
} avi_header_t;

typedef struct
{
    uint32_t dwFourCC;
    uint32_t dwSize;
} chunk_header_t;

typedef struct
{
    uint32_t ckid;
    uint32_t dwFlags;
    uint32_t dwChunkOffset;
    uint32_t dwChunkLength;
} avi_index_entry_t;

typedef struct index_list_s
{
    avi_index_entry_t entries[1024];
    struct index_list_s *next;
} index_list_t;

typedef struct
{
    FILE *f;
    index_list_t *p_curr_index;
    double d_scalefactor;
    int    i_currframe;
    int    i_frame;
    int    i_spsppslen;
    int    b_shortstartcode;
    chunk_header_t chunk_hdr;
    avi_header_t avi_hdr;
    char spsppsbuf[4096];
    index_list_t index;
} avi_t;

static int open_file( char *psz_filename, hnd_t *p_handle )
{
    avi_t *p_avi = malloc(sizeof(avi_t));

    if (!p_avi)
        return -1;

    memset(p_avi, 0, sizeof(*p_avi));
    if (!(p_avi->f = fopen(psz_filename,"wb"))){
        free(p_avi);
        return -1;
    }

    p_avi->avi_hdr.riff_hdr.dwList = MAKEFOURCC('R','I','F','F');
    p_avi->avi_hdr.riff_hdr.fcc  = MAKEFOURCC('A','V','I',' ');
    p_avi->avi_hdr.hdrl_hdr.dwList = MAKEFOURCC('L','I','S','T');
    p_avi->avi_hdr.hdrl_hdr.fcc  = MAKEFOURCC('h','d','r','l');
    p_avi->avi_hdr.strl_hdr.dwList = MAKEFOURCC('L','I','S','T');
    p_avi->avi_hdr.strl_hdr.fcc  = MAKEFOURCC('s','t','r','l');
    p_avi->avi_hdr.movi_hdr.dwList = MAKEFOURCC('L','I','S','T');
    p_avi->avi_hdr.movi_hdr.fcc  = MAKEFOURCC('m','o','v','i');
    p_avi->avi_hdr.avih = MAKEFOURCC('a','v','i','h');
    p_avi->avi_hdr.strh = MAKEFOURCC('s','t','r','h');
    p_avi->avi_hdr.strf = MAKEFOURCC('s','t','r','f');
    p_avi->avi_hdr.fccType = MAKEFOURCC('v','i','d','s');
    p_avi->avi_hdr.fccHandler = p_avi->avi_hdr.biCompression = MAKEFOURCC('H','2','6','4');
    p_avi->chunk_hdr.dwFourCC = MAKEFOURCC('0','0','d','c');

    p_avi->avi_hdr.dwMaxBytesPerSec = LE32(0xFFFFFFFF);
    p_avi->avi_hdr.dwavihFlags = LE32(0x0910);
    p_avi->avi_hdr.dwStreams = LE32(1);
    p_avi->avi_hdr.dwQuality = LE32(-1);
    p_avi->avi_hdr.hdrl_hdr.dwSize = LE32(4 + 4*16 + 12 + 4*16 + 4*12);
    p_avi->avi_hdr.strl_hdr.dwSize = LE32(4 + 4*16 + 4*12);
    p_avi->avi_hdr.avih_size = LE32(4*16 - 8);
    p_avi->avi_hdr.strh_size = LE32(4*16 - 8);
    p_avi->avi_hdr.strf_size = LE32(4*12 - 8);
    p_avi->avi_hdr.biSize = LE32(4*12 - 8);
    p_avi->avi_hdr.biPlanes = LE16(1);
    p_avi->avi_hdr.biBitCount = LE16(24);

    p_avi->p_curr_index = &p_avi->index;

    *p_handle = p_avi;
    return 0;
}

static int set_param( hnd_t handle, x264_param_t *p_param )
{
    avi_t *p_avi = handle;
    uint32_t imgsize;

    p_avi->avi_hdr.dwWidth = p_avi->avi_hdr.biWidth = LE32(p_param->i_width);
    p_avi->avi_hdr.rcFrameRight = LE16((uint16_t)p_param->i_width);
    p_avi->avi_hdr.dwHeight = p_avi->avi_hdr.biHeight = LE32(p_param->i_height);
    p_avi->avi_hdr.rcFrameBottom = LE16((uint16_t)p_param->i_height);
    imgsize = p_param->i_width * p_param->i_height;
    p_avi->avi_hdr.dwSampleSize = LE32(imgsize);
    p_avi->avi_hdr.biSizeImage = LE16(imgsize);

    p_avi->avi_hdr.dwRate  = LE32(p_param->i_fps_num);
    p_avi->avi_hdr.dwScale = LE32(p_param->i_fps_den);
    p_avi->d_scalefactor = (double)p_param->i_fps_num * p_param->i_timebase_num / p_param->i_timebase_den;

    fwrite(&p_avi->avi_hdr,sizeof(avi_header_t),1,p_avi->f);
    return 0;
}

static int write_headers( hnd_t handle, x264_nal_t *p_nal, int i_nal )
{
    avi_t *p_avi = handle;
    int size = 0, i = 0;
    for (;i < i_nal;i++) size += p_nal[i].i_payload;
    memcpy(p_avi->spsppsbuf, p_nal[0].p_payload, size);
    p_avi->i_spsppslen = size;
    p_avi->b_shortstartcode = 0;
    return size;
}

static int write_frame( hnd_t handle, uint8_t *p_nalu, int i_size, x264_picture_t *p_picture )
{
    avi_t *p_avi = handle;
    avi_index_entry_t *p_entry;
    int idr = p_picture->b_keyframe << 4;
    uint64_t dts;
    int useshortstartcode = 0;

    if (dts = llrint(p_picture->i_dts * p_avi->d_scalefactor)) {

        uint32_t interval = p_avi->avi_hdr.dwScale;
        uint64_t currdts = (uint64_t)(p_avi->i_frame) * interval;
        for(currdts -= (interval + 1) / 2;dts >= currdts;currdts += interval){
            // insert zero-byte chunks
            avi_index_entry_t *p_entry = &p_avi->p_curr_index->entries[p_avi->i_frame & 1023];
            p_avi->i_frame++;
            p_entry->ckid = MAKEFOURCC('0','0','d','c');
            p_entry->dwFlags = 0;
            p_entry->dwChunkOffset = LE32((uint32_t)ftell(p_avi->f));
            p_entry->dwChunkLength = 0;
            p_avi->chunk_hdr.dwSize = 0;
            fwrite(&p_avi->chunk_hdr, sizeof(chunk_header_t), 1, p_avi->f);

            if (!(p_avi->i_frame & 1023)){
                if (!(p_avi->p_curr_index->next = malloc(sizeof(index_list_t))))
                    return -1;
                p_avi->p_curr_index = p_avi->p_curr_index->next;
                p_avi->p_curr_index->next = NULL;
            }
        }
    }
    p_avi->i_currframe++;

    p_entry = &p_avi->p_curr_index->entries[p_avi->i_frame & 1023];
    p_avi->i_frame++;
    p_entry->ckid = MAKEFOURCC('0','0','d','c');
    p_entry->dwFlags = LE32(idr);
    p_entry->dwChunkOffset = LE32((uint32_t)ftell(p_avi->f));

    p_avi->chunk_hdr.dwSize = i_size;
    if (p_avi->i_spsppslen > 0)
        p_avi->chunk_hdr.dwSize += p_avi->i_spsppslen - 1;
    if (p_avi->b_shortstartcode)
        p_avi->chunk_hdr.dwSize--;
    if (p_avi->chunk_hdr.dwSize & 1){
        useshortstartcode = 1;
        p_avi->chunk_hdr.dwSize++;
    }
    p_avi->chunk_hdr.dwSize = LE32(p_avi->chunk_hdr.dwSize);
    if (LE32(p_avi->chunk_hdr.dwSize) > LE32(p_avi->avi_hdr.dwstrhSuggestedBufferSize))
        p_avi->avi_hdr.dwstrhSuggestedBufferSize = p_avi->chunk_hdr.dwSize;
    fwrite(&p_avi->chunk_hdr, sizeof(chunk_header_t), 1, p_avi->f);

    if (p_avi->i_spsppslen > 0){
        fwrite(p_avi->spsppsbuf + 1, p_avi->i_spsppslen - 1, 1, p_avi->f);
        p_avi->i_spsppslen = -1;
    }

    if (p_avi->b_shortstartcode)
        fwrite(p_nalu + 1, i_size - 1, 1, p_avi->f);
    else
        fwrite(p_nalu, i_size, 1, p_avi->f);

    if (useshortstartcode)
        fputc(0, p_avi->f);
    p_avi->b_shortstartcode = useshortstartcode;

    p_entry->dwChunkLength = p_avi->chunk_hdr.dwSize;
    if (!(p_avi->i_frame & 1023)){
        if (!(p_avi->p_curr_index->next = malloc(sizeof(index_list_t))))
            return -1;
        p_avi->p_curr_index = p_avi->p_curr_index->next;
        p_avi->p_curr_index->next = NULL;
    }

    return i_size;
}

static int close_file( hnd_t handle, int64_t largest_pts, int64_t second_largest_pts )
{
    avi_t *p_avi = handle;
    index_list_t *p;
    chunk_header_t index_hdr;

    if (!p_avi)
        return 0;

    p_avi->avi_hdr.dwavihSuggestedBufferSize = p_avi->avi_hdr.dwstrhSuggestedBufferSize;
    p_avi->avi_hdr.dwTotalFrames = p_avi->avi_hdr.dwLength = LE32(p_avi->i_frame);
    p_avi->avi_hdr.movi_hdr.dwSize = LE32((uint32_t)ftell(p_avi->f) - sizeof(avi_header_t) + 4);

    index_hdr.dwFourCC = MAKEFOURCC('i','d','x','1');
    index_hdr.dwSize = LE32(p_avi->i_frame * sizeof(avi_index_entry_t));
    fwrite(&index_hdr, sizeof(chunk_header_t), 1, p_avi->f);

    p = &p_avi->index;
    do {
        fwrite(p->entries,sizeof(avi_index_entry_t),p_avi->i_frame >= 1024? 1024:p_avi->i_frame,p_avi->f);
        p_avi->i_frame -= 1024;
    }while (p = p->next);

    for (p = p_avi->index.next;p;){
        index_list_t *q = p;
        p = p->next;
        free(q);
    }

    p_avi->avi_hdr.riff_hdr.dwSize = LE32((uint32_t)ftell(p_avi->f) - 8);
    fseek(p_avi->f,0,SEEK_SET);
    fwrite(&p_avi->avi_hdr,sizeof(avi_header_t),1,p_avi->f);

    fclose(p_avi->f);
    free( p_avi );
    return 0;
}

const cli_output_t avi_output = { open_file, set_param, write_headers, write_frame, close_file };
