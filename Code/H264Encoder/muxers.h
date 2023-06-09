/*****************************************************************************
 * muxers.h: h264 file i/o modules
 *****************************************************************************
 * Copyright (C) 2003-2009 x264 project
 *
 * Authors: Laurent Aimar <fenrir@via.ecp.fr>
 *          Loren Merritt <lorenm@u.washington.edu>
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

#ifndef X264_MUXERS_H
#define X264_MUXERS_H

#include "common/common.h"
#include "x264.h"

#ifndef _MSC_VER
#include "config.h"
#endif

#ifdef _WIN32
#include <Windows.h>
#endif

typedef void *hnd_t;

static inline int64_t gcd( int64_t a, int64_t b )
{
    while( 1 )
    {
        int64_t c = a % b;
        if( !c )
            return b;
        a = b;
        b = c;
    }
}

static inline int64_t lcm( int64_t a, int64_t b )
{
    return ( a / gcd( a, b ) ) * b;
}

static inline char *get_filename_extension( char *filename )
{
    char *ext = filename + strlen( filename );
    while( *ext != '.' && ext > filename )
        ext--;
    ext += *ext == '.';
    return ext;
}

#include "input/input.h"
#include "output/output.h"

int __stdcall comp_luma(uint8_t *dstplane, uint8_t *srcplane, int w, int h, int thresh, int mb_thresh, int mb_max);
int __stdcall comp_chroma(uint8_t *dstplane, uint8_t *srcplane, int size, int thresh);
#endif
