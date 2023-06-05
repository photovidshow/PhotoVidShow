/*
 * Multiple format streaming server
 * Copyright (c) 2000, 2001, 2002 Fabrice Bellard
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
#define HAVE_AV_CONFIG_H
#include "avformat.h"
#include "avcodec.h"



/* add a codec and set the default parameters */
/*
static void add_codec(FFStream *stream, AVCodecContext *av)
{
    AVStream *st;

    // compute default parameters 
    switch(av->codec_type) {
    case CODEC_TYPE_AUDIO:
        if (av->bit_rate == 0)
            av->bit_rate = 64000;
        if (av->sample_rate == 0)
            av->sample_rate = 22050;
        if (av->channels == 0)
            av->channels = 1;
        break;
    case CODEC_TYPE_VIDEO:
        if (av->bit_rate == 0)
            av->bit_rate = 64000;
        if (av->frame_rate == 0){
            av->frame_rate = 5;
            av->frame_rate_base = 1;
        }
        if (av->width == 0 || av->height == 0) {
            av->width = 160;
            av->height = 128;
        }
        // Bitrate tolerance is less for streaming 
        if (av->bit_rate_tolerance == 0)
            av->bit_rate_tolerance = av->bit_rate / 4;
        if (av->qmin == 0)
            av->qmin = 3;
        if (av->qmax == 0)
            av->qmax = 31;
        if (av->max_qdiff == 0)
            av->max_qdiff = 3;
        av->qcompress = 0.5;
        av->qblur = 0.5;

        if (!av->rc_eq)
            av->rc_eq = "tex^qComp";
        if (!av->i_quant_factor)
            av->i_quant_factor = -0.8;
        if (!av->b_quant_factor)
            av->b_quant_factor = 1.25;
        if (!av->b_quant_offset)
            av->b_quant_offset = 1.25;
        if (!av->rc_min_rate)
            av->rc_min_rate = av->bit_rate / 2;
        if (!av->rc_max_rate)
            av->rc_max_rate = av->bit_rate * 2;

        break;
    default:
        av_abort();
    }

    st = av_mallocz(sizeof(AVStream));
    if (!st)
        return;
    stream->streams[stream->nb_streams++] = st;
    memcpy(&st->codec, av, sizeof(AVCodecContext));
}
*/


static int opt_audio_codec(const char *arg)
{
    AVCodec *p;

    p = first_avcodec;
    while (p) {
        if (!strcmp(p->name, arg) && p->type == CODEC_TYPE_AUDIO)
            break;
        p = p->next;
    }
    if (p == NULL) {
        return CODEC_ID_NONE;
    }

    return p->id;
}

static int opt_video_codec(const char *arg)
{
    AVCodec *p;

    p = first_avcodec;
    while (p) {
        if (!strcmp(p->name, arg) && p->type == CODEC_TYPE_VIDEO)
            break;
        p = p->next;
    }
    if (p == NULL) {
        return CODEC_ID_NONE;
    }

    return p->id;
}

/* simplistic plugin support */


