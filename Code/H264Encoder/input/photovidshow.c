/*****************************************************************************
 * photovidshow.c: x264 photovidshow input module
 *****************************************************************************
 * Takes RGB video frames from PhotoVidShow video generator
 *****************************************************************************/

#include "photovidshow.h"

getVideoFptr video_callback ;


static int g_total_frames ;
static int g_width ;
static int g_height ;

void photovidshow_set_input_parameters( getVideoFptr video, int width, int height, int frames)
{
	video_callback= video;
	g_width = width;
	g_height = height;
	g_total_frames = frames;
}


#include "muxers.h"


typedef struct AVPicture {
    uint8_t *data[4];
    int linesize[4];       ///< number of bytes per line
} AVPicture;


extern void rgba32_to_yuv420p(AVPicture *dst, const AVPicture *src, int width, int height);


static int open_file( char *psz_filename, hnd_t *p_handle, video_info_t *info, cli_input_opt_t *opt )
{
	info->width = g_width;
	info->height = g_height;
	info->vfr     = 0;

    return 0;
}

static int get_frame_total( hnd_t handle )
{
  return g_total_frames;
}

static int frame_count =0;

static int read_frame( x264_picture_t *p_pic, hnd_t handle, int i_frame )
{
	unsigned int bytesInRow;
	char *buffer;
	AVPicture src;
	AVPicture dst;

	video_callback(i_frame, &bytesInRow, &buffer);

	src.data[0] = buffer;
	src.linesize[0] = bytesInRow ;

	dst.data[0] = p_pic->img.plane[0] ;
	dst.data[1] = p_pic->img.plane[1] ;
	dst.data[2] = p_pic->img.plane[2] ;
	dst.linesize[0] = p_pic->img.i_stride[0];
	dst.linesize[1] = p_pic->img.i_stride[1];
	dst.linesize[2] = p_pic->img.i_stride[2];

	rgba32_to_yuv420p(&dst , (const AVPicture*) &src, g_width, g_height);

    return 0;
}

static int close_file( hnd_t handle )
{
	return 0;
}

const cli_input_t photovidshow_input = { open_file, get_frame_total, x264_picture_alloc, read_frame, NULL, x264_picture_clean, close_file };

