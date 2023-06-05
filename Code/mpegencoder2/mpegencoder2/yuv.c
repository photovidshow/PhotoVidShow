/*
 * .Y.U.V image format
 * Copyright (c) 2003 Fabrice Bellard.
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
#include "avformat.h"

typedef int(*getVideoFptr)(unsigned int frameNum, unsigned int *bytesInRow, char **buffer);

//extern void rgba32_to_yuv420p(AVPicture *dst, const AVPicture *src,
//                                        int width, int height);


/* RGB to YUV MMX routine */

static short int ycoef[7][4] = {     /* 32768 scaled y table, bgra order */
    {2363, 23442, 6963, 0},
    {3736, 19235, 9798, 0},
    {3736, 19235, 9798, 0},
    {3604, 19333, 9830, 0},
    {3736, 19235, 9798, 0},
    {3736, 19235, 9798, 0},
    {2851, 22970, 6947, 0}};

static short int ucoef[7][4] = {
    {16384, -12648, -3768, 0},
    {16384, -10846, -5538, 0},
    {16384, -10846, -5538, 0},
    {16384, -10846, -5538, 0},
    {16384, -10846, -5538, 0},
    {16384, -10846, -5538, 0},
    {16384, -12583, -3801, 0}};

static short int vcoef[7][4] = {
    {-1507, -14877, 16384, 0},
    {-2654, -13730, 16384, 0},
    {-2654, -13730, 16384, 0},
    {-2589, -13795, 16384, 0},
    {-2654, -13730, 16384, 0},
    {-2654, -13730, 16384, 0},
    {-1802, -14582, 16384, 0}};

static short int *ycoefs, *ucoefs, *vcoefs;
static int done_rgb_to_yuv_mmx_init =0;


void init_rgb_to_yuv_mmx2(int coeffs)
{
  int i;

  i = coeffs;
  if (i > 8)
    i = 3;

  ycoefs = &ycoef[i-1][0];
  ucoefs = &ucoef[i-1][0];
  vcoefs = &vcoef[i-1][0];
}

void RGBtoYUVmmx2(unsigned char *src, unsigned char *desty, unsigned char *destu,
                 unsigned char *destv, int srcrowsize, int destrowsize,
                 int width, int height)
{
  unsigned char *yp, *up, *vp;
  unsigned char *prow;
  int i, j;

  _asm {
	xor       edx, edx
    	mov       eax, width
	sar       eax,1
        cmp       edx, eax
        jge       yuvexit

	mov       j, eax
	mov       eax, height
        mov       i, eax
	cmp       edx, eax
	jge       yuvexit

	mov       eax, desty
	mov       yp, eax
	mov       eax, destu
	mov       up, eax
	mov       eax, destv
	mov       vp, eax
	mov       eax, src
	mov       prow, eax
        pxor      MM7, MM7
        mov       eax, i

      heighttop:

        mov       i, eax
        mov       edi, j
        mov       ebx, prow
        mov       ecx, yp
        mov       edx, up
        mov       esi, vp

      widthtop:
        movq      MM5, [ebx]  // MM5 has 0 r2 g2 b2 0 r1 g1 b1, two pixels
        add       ebx, 8
        movq      MM6, MM5
        punpcklbw MM5, MM7 // MM5 has 0 r1 g1 b1
        punpckhbw MM6, MM7 // MM6 has 0 r2 g2 b2

        movq      MM0, MM5
        movq      MM1, MM6
        mov       eax, ycoefs
        pmaddwd   MM0, [eax] // MM0 has r1*cr and g1*cg+b1*cb
        movq      MM2, MM0
        psrlq     MM2, 32
        paddd     MM0, MM2   // MM0 has y1 in lower 32 bits
        pmaddwd   MM1, [eax] // MM1 has r2*cr and g2*cg+b2*cb
        movq      MM2, MM1
        psrlq     MM2, 32
        paddd     MM1, MM2   // MM1 has y2 in lower 32 bits
        movd      eax, MM0
        imul      eax, 219
        shr       eax, 8
        add       eax, 540672
        shr       eax, 15
        mov       [ecx], al
        inc       ecx
        movd      eax, MM1
        imul      eax, 219
        shr       eax, 8
        add       eax, 540672
        shr       eax, 15
        mov       [ecx], al
        inc       ecx

        movq      MM0, MM5
        movq      MM1, MM6
        mov       eax, ucoefs
        pmaddwd   MM0, [eax] // MM0 has r1*cr and g1*cg+b1*cb
        movq      MM2, MM0
        psrlq     MM2, 32
        paddd     MM0, MM2   // MM0 has u1 in lower 32 bits
        pmaddwd   MM1, [eax] // MM1 has r2*cr and g2*cg+b2*cb
        movq      MM2, MM1
        psrlq     MM2, 32
        paddd     MM1, MM2   // MM1 has u2 in lower 32 bits
        movd      eax, MM0
        imul      eax, 224
        sar       eax, 8
        add       eax, 4210688
        shr       eax, 15
        mov       [edx], al
        inc       edx
        movd      eax, MM1
        imul      eax, 224
        sar       eax, 8
        add       eax, 4210688
        shr       eax, 15
        mov       [edx], al
        inc       edx

        mov       eax, vcoefs
        pmaddwd   MM5, [eax] // MM5 has r1*cr and g1*cg+b1*cb
        movq      MM2, MM5
        psrlq     MM2, 32
        paddd     MM5, MM2   // MM5 has v1 in lower 32 bits
        pmaddwd   MM6, [eax] // MM6 has r2*cr and g2*cg+b2*cb
        movq      MM2, MM6
        psrlq     MM6, 32
        paddd     MM6, MM2   // MM6 has v2 in lower 32 bits
        movd      eax, MM5
        imul      eax, 224
        sar       eax, 8
        add       eax, 4210688
        shr       eax, 15
        mov       [esi], al
        inc       esi
        movd      eax, MM6
        imul      eax, 224
        sar       eax, 8
        add       eax, 4210688
        shr       eax, 15
        mov       [esi], al
        inc       esi

        dec       edi
        jnz       widthtop

        mov       eax, destrowsize
        add       yp, eax
        add       up, eax
        add       vp, eax
        mov       eax, srcrowsize
        sub       prow, eax
        mov       eax, i
        dec       eax
        jnz       heighttop

      yuvexit:
        emms
      }
}

extern getVideoFptr srg_get_video_callback ;

static int sizes[][2] = {
    { 640, 480 },
    { 720, 480 },
    { 720, 576 },
    { 352, 288 },
    { 352, 240 },
    { 160, 128 },
    { 512, 384 },
    { 640, 352 },
    { 640, 240 },
};




static int infer_size(int *width_ptr, int *height_ptr, int size)
{
    int i;

    for(i=0;i<sizeof(sizes)/sizeof(sizes[0]);i++) {
        if ((sizes[i][0] * sizes[i][1]) == size) {
            *width_ptr = sizes[i][0];
            *height_ptr = sizes[i][1];
            return 0;
        }
    }
    return -1;
}

extern int srg_frames_done;
extern int srg_encode_height;
extern int srg_encode_width;


#include <stdio.h>

static int yuv_read(ByteIOContext *f,
                    int (*alloc_cb)(void *opaque, AVImageInfo *info), void *opaque)
{
	unsigned int bytesInRow;
	char *buffer;
	AVPicture src;
//char temp[256];
//FILE* fp;

    ByteIOContext pb1, *pb = &pb1;
    int img_size, ret;
    char fname[1024], *p;
    int size;
    URLContext *h;
    AVImageInfo info1, *info = &info1;
    
    /* XXX: hack hack */
  //  h = url_fileno(f);
  //  img_size = url_seek(h, 0, SEEK_END);
  //  url_get_filename(h, fname, sizeof(fname));

	// SRG
	img_size = srg_encode_height * srg_encode_width;

  // if (infer_size(&info->width, &info->height, img_size) < 0) {
  //      return AVERROR_IO;
  //  }

	 
	info->width =srg_encode_width;
	info->height=srg_encode_height;


    info->pix_fmt = PIX_FMT_YUV420P;
    
    ret = alloc_cb(opaque, info);
    if (ret)
        return ret;
    
 //   size = info->width * info->height;
    
  //  p = strrchr(fname, '.');
 //   if (!p && (p[1] != 'Y' || p[1]!='y'))
 //       return AVERROR_IO;

	// SRG reads only 1 packet???
   // get_buffer(f, info->pict.data[0], size);


	// SRG 

  //  uint8_t *data[4];
  //  int linesize[4];       ///<

	

	srg_get_video_callback(srg_frames_done-1, &bytesInRow, &buffer);

	src.data[0] = buffer;
	src.linesize[0] = bytesInRow ;
//	if (done_rgb_to_yuv_mmx_init==0)
//	{
//		init_rgb_to_yuv_mmx2(5);
//		done_rgb_to_yuv_mmx_init=1;
//	}

//	RGBtoYUVmmx2(buffer, info->pict.data[0], info->pict.data[1],info->pict.data[2]
//			, bytesInRow, bytesInRow>>2, info->width, info->height);
    

	rgba32_to_yuv420p(&info->pict , (const AVPicture*) &src, info->width,info->height);

	/*
	sprintf(temp,"c:\\dvdtools\\ffmpeggui\\test%d.y",srg_frames_done-1);

	fp = fopen(temp,"wb");
	if (fp)
	{
		fwrite(info->pict.data[0],720*576,1,fp);
		fclose(fp);
	}


	sprintf(temp,"c:\\dvdtools\\ffmpeggui\\test%d.u",srg_frames_done-1);

	fp = fopen(temp,"wb");
	if (fp)
	{
		fwrite(info->pict.data[1],360*288,1,fp);
		fclose(fp);
	}

	sprintf(temp,"c:\\dvdtools\\ffmpeggui\\test%d.v",srg_frames_done-1);

	fp = fopen(temp,"wb");
	if (fp)
	{
		fwrite(info->pict.data[2],360*288,1,fp);
		fclose(fp);
	}
*/

		


	return 0;

	//if (url_fopen(pb, fname, URL_RDONLY) < 0)
//		    return AVERROR_IO;
//	get_buffer(pb, info->pict.data[0], size);
  //  url_fclose(pb);
	// end SRG

    
    p[1] = 'U';
    if (url_fopen(pb, fname, URL_RDONLY) < 0)
        return AVERROR_IO;

    get_buffer(pb, info->pict.data[1], size / 4);
    url_fclose(pb);
    
    p[1] = 'V';
    if (url_fopen(pb, fname, URL_RDONLY) < 0)
        return AVERROR_IO;

    get_buffer(pb, info->pict.data[2], size / 4);
    url_fclose(pb);
    return 0;
}

static int yuv_write(ByteIOContext *pb2, AVImageInfo *info)
{
    ByteIOContext pb1, *pb;
    char fname[1024], *p;
    int i, j, width, height;
    uint8_t *ptr;
    URLContext *h;
    static const char *ext = "YUV";
    
    /* XXX: hack hack */
    h = url_fileno(pb2);
    url_get_filename(h, fname, sizeof(fname));

    p = strrchr(fname, '.');
    if (!p || p[1] != 'Y')
        return AVERROR_IO;

    width = info->width;
    height = info->height;

    for(i=0;i<3;i++) {
        if (i == 1) {
            width >>= 1;
            height >>= 1;
        }

        if (i >= 1) {
            pb = &pb1;
            p[1] = ext[i];
            if (url_fopen(pb, fname, URL_WRONLY) < 0)
                return AVERROR_IO;
        } else {
            pb = pb2;
        }
    
        ptr = info->pict.data[i];
        for(j=0;j<height;j++) {
            put_buffer(pb, ptr, width);
            ptr += info->pict.linesize[i];
        }
        put_flush_packet(pb);
        if (i >= 1) {
            url_fclose(pb);
        }
    }
    return 0;
}
    
static int yuv_probe(AVProbeData *pd)
{
    if (match_ext(pd->filename, "Y"))
        return AVPROBE_SCORE_MAX;
    else
        return 0;
}

AVImageFormat yuv_image_format = {
    "yuv",
    "Y",
    yuv_probe,
    yuv_read,
    (1 << PIX_FMT_YUV420P),
    yuv_write,
};
