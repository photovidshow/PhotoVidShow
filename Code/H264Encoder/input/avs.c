/*****************************************************************************
 * avs.c: x264 avisynth input module
 *****************************************************************************
 * Copyright (C) 2009 x264 project
 *
 * Authors: Steven Walters <kemuri9@gmail.com>
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

/* the AVS interface currently uses __declspec to link function declarations to their definitions in the dll.
   this has a side effect of preventing program execution if the avisynth dll is not found,
   so define __declspec(dllimport) to nothing and work around this */
#undef __declspec
#define __declspec(i)
#undef EXTERN_C

#if HAVE_AVISYNTH_C_H
#include <avisynth_c.h>
#else
#include "extras/avisynth_c.h"
#endif

/* AVS uses a versioned interface to control backwards compatibility */
/* YV12 support is required */
#define AVS_INTERFACE_YV12 2
/* when AVS supports other planar colorspaces, a workaround is required */
#define AVS_INTERFACE_OTHER_PLANAR 5

/* maximum size of the sequence of filters to try on non script files */
#define AVS_MAX_SEQUENCE 5

#define LOAD_AVS_FUNC(name, continue_on_fail)\
{\
    h->func.name = (void*)GetProcAddress( h->library, #name );\
    if( !continue_on_fail && !h->func.name )\
        goto fail;\
}

typedef struct
{
    AVS_Clip *clip;
    AVS_ScriptEnvironment *env;
    HMODULE library;
    int num_frames;
    /* declare function pointers for the utilized functions to be loaded without __declspec,
       as the avisynth header does not compensate for this type of usage */
    struct
    {
        const char *(__stdcall *avs_clip_get_error)( AVS_Clip *clip );
        AVS_ScriptEnvironment *(__stdcall *avs_create_script_environment)( int version );
        void (__stdcall *avs_delete_script_environment)( AVS_ScriptEnvironment *env );
        AVS_VideoFrame *(__stdcall *avs_get_frame)( AVS_Clip *clip, int n );
        int (__stdcall *avs_get_version)( AVS_Clip *clip );
        const AVS_VideoInfo *(__stdcall *avs_get_video_info)( AVS_Clip *clip );
        int (__stdcall *avs_function_exists)( AVS_ScriptEnvironment *env, const char *name );
        AVS_Value (__stdcall *avs_invoke)( AVS_ScriptEnvironment *env, const char *name,
            AVS_Value args, const char **arg_names );
        void (__stdcall *avs_release_clip)( AVS_Clip *clip );
        void (__stdcall *avs_release_value)( AVS_Value value );
        void (__stdcall *avs_release_video_frame)( AVS_VideoFrame *frame );
        AVS_Clip *(__stdcall *avs_take_clip)( AVS_Value, AVS_ScriptEnvironment *env );
    } func;
} avs_hnd_t;

/* load the library and functions we require from it */
static int avs_load_library( avs_hnd_t *h )
{
    h->library = LoadLibrary( "avisynth" );
    if( !h->library )
        return -1;
    LOAD_AVS_FUNC( avs_clip_get_error, 0 );
    LOAD_AVS_FUNC( avs_create_script_environment, 0 );
    LOAD_AVS_FUNC( avs_delete_script_environment, 1 );
    LOAD_AVS_FUNC( avs_get_frame, 0 );
    LOAD_AVS_FUNC( avs_get_version, 0 );
    LOAD_AVS_FUNC( avs_get_video_info, 0 );
    LOAD_AVS_FUNC( avs_function_exists, 0 );
    LOAD_AVS_FUNC( avs_invoke, 0 );
    LOAD_AVS_FUNC( avs_release_clip, 0 );
    LOAD_AVS_FUNC( avs_release_value, 0 );
    LOAD_AVS_FUNC( avs_release_video_frame, 0 );
    LOAD_AVS_FUNC( avs_take_clip, 0 );
    return 0;
fail:
    FreeLibrary( h->library );
    return -1;
}

static AVS_Value update_clip( avs_hnd_t *h, const AVS_VideoInfo **vi, AVS_Value res, AVS_Value release )
{
    h->func.avs_release_clip( h->clip );
    h->clip = h->func.avs_take_clip( res, h->env );
    h->func.avs_release_value( release );
    *vi = h->func.avs_get_video_info( h->clip );
    return res;
}

static int open_file( char *psz_filename, hnd_t *p_handle, video_info_t *info, cli_input_opt_t *opt )
{
    avs_hnd_t *h;
    AVS_Value arg, res;
    char *filename_ext;
    int avs_version;
    const AVS_VideoInfo *vi;
    FILE *fh = fopen( psz_filename, "r" );
    if( !fh )
        return -1;
    else if( !x264_is_regular_file( fh ) )
    {
        fprintf( stderr, "avs [error]: AVS input is incompatible with non-regular file `%s'\n", psz_filename );
        return -1;
    }
    fclose( fh );

    h = malloc( sizeof(avs_hnd_t) );
    if( !h )
        return -1;
    if( avs_load_library( h ) )
    {
        fprintf( stderr, "avs [error]: failed to load avisynth\n" );
        return -1;
    }
    h->env = h->func.avs_create_script_environment( AVS_INTERFACE_YV12 );
    if( !h->env )
    {
        fprintf( stderr, "avs [error]: failed to initiate avisynth\n" );
        return -1;
    }
    arg = avs_new_value_string( psz_filename );
    filename_ext = get_filename_extension( psz_filename );

    {
        AVS_Value mt_test;
        int mt_mode;
        res = h->func.avs_invoke( h->env, "Import", arg, NULL );
        if( avs_is_error( res ) )
        {
            fprintf( stderr, "avs [error]: %s\n", avs_as_string( res ) );
            return -1;
        }
        /* check if the user is using a multi-threaded script and apply distributor if necessary.
           adapted from avisynth's vfw interface */
        mt_test = h->func.avs_invoke( h->env, "GetMTMode", avs_new_value_bool( 0 ), NULL );
        mt_mode = avs_is_int( mt_test ) ? avs_as_int( mt_test ) : 0;
        h->func.avs_release_value( mt_test );
        if( mt_mode > 0 && mt_mode < 5 )
        {
            AVS_Value temp = h->func.avs_invoke( h->env, "Distributor", res, NULL );
            h->func.avs_release_value( res );
            res = temp;
        }
    }
    if( !avs_is_clip( res ) )
    {
        fprintf( stderr, "avs [error]: `%s' didn't return a video clip\n", psz_filename );
        return -1;
    }
    h->clip = h->func.avs_take_clip( res, h->env );
    avs_version = h->func.avs_get_version( h->clip );
    vi = h->func.avs_get_video_info( h->clip );
    if( !avs_has_video( vi ) )
    {
        fprintf( stderr, "avs [error]: `%s' has no video data\n", psz_filename );
        return -1;
    }
    /* if the clip is made of fields instead of frames, call weave to make them frames */
    if( avs_is_field_based( vi ) )
    {
        AVS_Value tmp = h->func.avs_invoke( h->env, "Weave", res, NULL );
        fprintf( stderr, "avs [warning]: detected fieldbased (separated) input, weaving to frames\n" );
        if( avs_is_error( tmp ) )
        {
            fprintf( stderr, "avs [error]: couldn't weave fields into frames\n" );
            return -1;
        }
        res = update_clip( h, &vi, tmp, res );
        info->interlaced = 1;
        info->tff = avs_is_tff( vi );
    }
    if( vi->width&1 || vi->height&1 )
    {
        fprintf( stderr, "avs [error]: input clip width or height not divisible by 2 (%dx%d)\n",
                 vi->width, vi->height );
        return -1;
    }
    /* always call ConvertToYV12 to convert non YV12 planar colorspaces to YV12 when user's AVS supports them,
       as all planar colorspaces are flagged as YV12. If it is already YV12 in this case, the call does nothing */
    if( !avs_is_yv12( vi ) || avs_version >= AVS_INTERFACE_OTHER_PLANAR )
    {
        const char *arg_name[2] = { NULL, "interlaced" };
        AVS_Value arg_arr[2];
        AVS_Value res2;
        fprintf( stderr, "avs %s\n", !avs_is_yv12( vi ) ? "[warning]: converting input clip to YV12"
               : "[info]: avisynth 2.6+ detected, forcing conversion to YV12" );
        arg_arr[0] = res; arg_arr[1] = avs_new_value_bool( info->interlaced );
        res2 = h->func.avs_invoke( h->env, "ConvertToYV12", avs_new_value_array( arg_arr, 2 ), arg_name );
        if( avs_is_error( res2 ) )
        {
            fprintf( stderr, "avs [error]: couldn't convert input clip to YV12\n" );
            return -1;
        }
        res = update_clip( h, &vi, res2, res );
    }
    h->func.avs_release_value( res );

    info->width = vi->width;
    info->height = vi->height;
    info->fps_num = vi->fps_numerator;
    info->fps_den = vi->fps_denominator;
    h->num_frames = vi->num_frames;
    info->csp = X264_CSP_YV12;
    info->vfr = 0;

    *p_handle = h;
    return 0;
}

static int get_frame_total( hnd_t handle )
{
    avs_hnd_t *h = handle;
    return h->num_frames;
}

static int picture_alloc( x264_picture_t *pic, int i_csp, int i_width, int i_height )
{
    x264_picture_init( pic );
    pic->img.i_csp = i_csp;
    pic->img.i_plane = 3;
    return 0;
}

static int read_frame( x264_picture_t *p_pic, hnd_t handle, int i_frame )
{
    static int plane[3] = { AVS_PLANAR_Y, AVS_PLANAR_V, AVS_PLANAR_U };
    avs_hnd_t *h = handle;
    AVS_VideoFrame *frm;
    int i;
    const char *err;
    if( i_frame >= h->num_frames )
        return -1;
    frm = p_pic->opaque = h->func.avs_get_frame( h->clip, i_frame );
    err = h->func.avs_clip_get_error( h->clip );
    if( err )
    {
        fprintf( stderr, "avs [error]: %s occurred while reading frame %d\n", err, i_frame );
        return -1;
    }
    for( i = 0; i < 3; i++ )
    {
        /* explicitly cast away the const attribute to avoid a warning */
        p_pic->img.plane[i] = (uint8_t*)avs_get_read_ptr_p( frm, plane[i] );
        p_pic->img.i_stride[i] = avs_get_pitch_p( frm, plane[i] );
    }
    return 0;
}

static int release_frame( x264_picture_t *pic, hnd_t handle )
{
    avs_hnd_t *h = handle;
    h->func.avs_release_video_frame( pic->opaque );
    return 0;
}

static void picture_clean( x264_picture_t *pic )
{
    memset( pic, 0, sizeof(x264_picture_t) );
}

static int close_file( hnd_t handle )
{
    avs_hnd_t *h = handle;
    h->func.avs_release_clip( h->clip );
    if( h->func.avs_delete_script_environment )
        h->func.avs_delete_script_environment( h->env );
    FreeLibrary( h->library );
    free( h );
    return 0;
}

const cli_input_t avs_input = { open_file, get_frame_total, picture_alloc, read_frame, release_frame, picture_clean, close_file };
