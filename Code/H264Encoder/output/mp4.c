/*****************************************************************************
* mp4.c: x264 mp4 output module
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

#include "muxers.h"
#include "../GPAC/isomedia.h"

typedef struct
{
	GF_ISOFile *p_file;
	GF_AVCConfig *p_config;
	GF_ISOSample *p_sample;
	int i_track;
	uint32_t i_descidx;
	uint32_t i_time_res;
	int i_scalefactor;
	int i_numframe;
	int i_num_delayed_frames;
	int64_t i_prev_dts[3];
	int64_t i_delayed_pts;
	int64_t i_delay_time;
} mp4_hnd_t;

static void recompute_bitrate_mp4(GF_ISOFile *p_file, int i_track)
{
	u32 i, count, di, timescale, time_wnd, rate;
	u64 offset;
	Double br;
	GF_ESD *esd;

	esd = gf_isom_get_esd(p_file, i_track, 1);
	if (!esd)
		return;

	esd->decoderConfig->avgBitrate = 0;
	esd->decoderConfig->maxBitrate = 0;
	rate = time_wnd = 0;

	timescale = gf_isom_get_media_timescale(p_file, i_track);
	count = gf_isom_get_sample_count(p_file, i_track);
	for (i = 0; i < count; i++)
	{
		GF_ISOSample *samp = gf_isom_get_sample_info(p_file, i_track, i + 1, &di, &offset);
		if (!samp)
		{
			fprintf(stderr, "mp4 [error]: failure reading back frame %u\n", i);
			break;
		}

		if (esd->decoderConfig->bufferSizeDB < samp->dataLength)
			esd->decoderConfig->bufferSizeDB = samp->dataLength;

		esd->decoderConfig->avgBitrate += samp->dataLength;
		rate += samp->dataLength;
		if (samp->DTS > time_wnd + timescale)
		{
			if (rate > esd->decoderConfig->maxBitrate)
				esd->decoderConfig->maxBitrate = rate;
			time_wnd = samp->DTS;
			rate = 0;
		}

		gf_isom_sample_del(&samp);
	}

	br = (Double)(s64)gf_isom_get_media_duration(p_file, i_track);
	br /= timescale;
	esd->decoderConfig->avgBitrate = (u32)(esd->decoderConfig->avgBitrate / br);
	/*move to bps*/
	esd->decoderConfig->avgBitrate *= 8;
	esd->decoderConfig->maxBitrate *= 8;

	gf_isom_change_mpeg4_description(p_file, i_track, 1, esd);
	gf_odf_desc_del((GF_Descriptor*)esd);
}

static int close_file(hnd_t handle, int64_t largest_pts, int64_t second_largest_pts)
{
	mp4_hnd_t *p_mp4 = handle;

	if (!p_mp4)
		return 0;

	if (p_mp4->p_config)
		gf_odf_avc_cfg_del(p_mp4->p_config);

	if (p_mp4->p_sample)
	{
		if (p_mp4->p_sample->data)
			free(p_mp4->p_sample->data);

		p_mp4->p_sample->dataLength = 0;
		gf_isom_sample_del(&p_mp4->p_sample);
	}

	if (p_mp4->p_file)
	{
		/* The mdhd duration is defined as CTS[final] - CTS[0] + duration of last frame.
		* The mdhd duration (in seconds) should be able to be longer than the tkhd duration since the track is managed by edts.
		* So, if mdhd duration is equal to the last DTS or less, we give the last composition time delta to the last sample duration.
		* And then, the mdhd duration is updated, but it time-wise doesn't give the actual duration.
		* The tkhd duration is the actual track duration. */
		uint64_t mdhd_duration = 2 * p_mp4->i_prev_dts[0] - p_mp4->i_prev_dts[1];
		if (mdhd_duration != gf_isom_get_media_duration(p_mp4->p_file, p_mp4->i_track))
		{
			uint64_t last_dts = gf_isom_get_sample_dts(p_mp4->p_file, p_mp4->i_track, p_mp4->i_numframe);
			uint32_t last_duration = (uint32_t)(mdhd_duration > last_dts ? mdhd_duration - last_dts : (largest_pts - second_largest_pts) << p_mp4->i_scalefactor);
			gf_isom_set_last_sample_duration(p_mp4->p_file, p_mp4->i_track, last_duration);
		}

		recompute_bitrate_mp4(p_mp4->p_file, p_mp4->i_track);
		gf_isom_set_pl_indication(p_mp4->p_file, GF_ISOM_PL_VISUAL, 0x15);
		gf_isom_set_storage_mode(p_mp4->p_file, GF_ISOM_STORE_FLAT);
		gf_isom_close(p_mp4->p_file);
	}

	free(p_mp4);

	return 0;
}

static int open_file(char *psz_filename, hnd_t *p_handle)
{
	mp4_hnd_t *p_mp4;

	FILE *fh = fopen(psz_filename, "w");
	*p_handle = NULL;
	if (!fh)
		return -1;
	else if (!x264_is_regular_file(fh))
	{
		fprintf(stderr, "mp4 [error]: MP4 output is incompatible with non-regular file `%s'\n", psz_filename);
		return -1;
	}
	fclose(fh);

	if (!(p_mp4 = malloc(sizeof(mp4_hnd_t))))
		return -1;

	memset(p_mp4, 0, sizeof(mp4_hnd_t));
	p_mp4->p_file = gf_isom_open(psz_filename, GF_ISOM_OPEN_WRITE, NULL);

	if (!(p_mp4->p_sample = gf_isom_sample_new()))
	{
		close_file(p_mp4, 0, 0);
		return -1;
	}

	gf_isom_set_brand_info(p_mp4->p_file, GF_ISOM_BRAND_AVC1, 0);

	*p_handle = p_mp4;

	return 0;
}

static int set_param(hnd_t handle, x264_param_t *p_param)
{
	mp4_hnd_t *p_mp4 = handle;

	p_mp4->i_time_res = p_param->i_timebase_den;
	p_mp4->i_scalefactor = 0;
	while (p_mp4->i_time_res < 200)
	{
		p_mp4->i_time_res <<= 1;
		p_mp4->i_scalefactor++;
	}
	p_mp4->i_num_delayed_frames = 0;
	p_mp4->i_delay_time = 0;

	p_mp4->i_track = gf_isom_new_track(p_mp4->p_file, 0, GF_ISOM_MEDIA_VISUAL,
		p_mp4->i_time_res);

	p_mp4->p_config = gf_odf_avc_cfg_new();
	gf_isom_avc_config_new(p_mp4->p_file, p_mp4->i_track, p_mp4->p_config,
		NULL, NULL, &p_mp4->i_descidx);

	gf_isom_set_track_enabled(p_mp4->p_file, p_mp4->i_track, 1);

	gf_isom_set_visual_info(p_mp4->p_file, p_mp4->i_track, p_mp4->i_descidx,
		p_param->i_width, p_param->i_height);

	if (p_param->vui.i_sar_width && p_param->vui.i_sar_height)
	{
		int64_t dw = p_param->i_width << 16;
		int64_t dh = p_param->i_height << 16;
		double sar = (double)p_param->vui.i_sar_width / p_param->vui.i_sar_height;
		if (sar >= 1.0)
			dw = llrint(dw * sar);
		else
			dh = llrint(dh / sar);
		gf_isom_set_pixel_aspect_ratio(p_mp4->p_file, p_mp4->i_track, p_mp4->i_descidx, p_param->vui.i_sar_width, p_param->vui.i_sar_height);
		gf_isom_set_track_layout_info(p_mp4->p_file, p_mp4->i_track, dw, dh, 0, 0, 0);
	}

	p_mp4->p_sample->data = malloc(p_param->i_width * p_param->i_height * 3 / 2);
	if (!p_mp4->p_sample->data)
		return -1;

	return 0;
}

static int write_headers(hnd_t handle, x264_nal_t *p_nal, int i_nal)
{
	mp4_hnd_t *p_mp4 = handle;
	GF_AVCConfigSlot *p_slot;

	int sps_size = p_nal[0].i_payload - 4;
	int pps_size = p_nal[1].i_payload - 4;
	int sei_size = i_nal > 2 ? p_nal[2].i_payload : 0;

	uint8_t *sps = p_nal[0].p_payload + 4;
	uint8_t *pps = p_nal[1].p_payload + 4;
	uint8_t *sei = sei_size ? p_nal[2].p_payload : 0;

	// SPS

	p_mp4->p_config->configurationVersion = 1;
	p_mp4->p_config->AVCProfileIndication = sps[1];
	p_mp4->p_config->profile_compatibility = sps[2];
	p_mp4->p_config->AVCLevelIndication = sps[3];
	p_slot = malloc(sizeof(GF_AVCConfigSlot));
	if (!p_slot)
		return -1;
	p_slot->size = sps_size;
	p_slot->data = malloc(p_slot->size);
	if (!p_slot->data)
		return -1;
	memcpy(p_slot->data, sps, sps_size);
	gf_list_add(p_mp4->p_config->sequenceParameterSets, p_slot);

	// PPS

	p_slot = malloc(sizeof(GF_AVCConfigSlot));
	if (!p_slot)
		return -1;
	p_slot->size = pps_size;
	p_slot->data = malloc(p_slot->size);
	if (!p_slot->data)
		return -1;
	memcpy(p_slot->data, pps, pps_size);
	gf_list_add(p_mp4->p_config->pictureParameterSets, p_slot);
	gf_isom_avc_config_update(p_mp4->p_file, p_mp4->i_track, 1, p_mp4->p_config);

	// SEI

	memcpy(p_mp4->p_sample->data + p_mp4->p_sample->dataLength, sei, sei_size);
	p_mp4->p_sample->dataLength += sei_size;

	return sei_size + sps_size + pps_size;
}
static int write_frame(hnd_t handle, uint8_t *p_nalu, int i_size, x264_picture_t *p_picture)
{
	mp4_hnd_t *p_mp4 = handle;
	int32_t offset = 0;
	int64_t dts;
	int64_t cts;

	//
	// SRG coded modified here, logic taken from more recent version of x264
	// Here we adjust negative dts so it starts at time 0
	//
	// This change was needed becuase was ending up with irregular DTS intervals
	// which was making LAV and (avlibs) think it has a variable frame rate and
	// also calculated frame rate was likely wrong ( i.e. next_dts - dts) 
	// See G1166
	// 
	if (p_mp4->i_numframe == 0)
	{
		p_mp4->i_delay_time = p_picture->i_dts * -1;
	}

	memcpy(p_mp4->p_sample->data + p_mp4->p_sample->dataLength, p_nalu, i_size);
	p_mp4->p_sample->dataLength += i_size;

	dts = (p_picture->i_dts + p_mp4->i_delay_time) << p_mp4->i_scalefactor;
	cts = (p_picture->i_pts + p_mp4->i_delay_time) << p_mp4->i_scalefactor;

	offset = cts - dts;

	p_mp4->p_sample->IsRAP = p_picture->b_keyframe;
	p_mp4->p_sample->DTS = dts;
	p_mp4->p_sample->CTS_Offset = offset;
	gf_isom_add_sample(p_mp4->p_file, p_mp4->i_track, p_mp4->i_descidx, p_mp4->p_sample);

	p_mp4->p_sample->dataLength = 0;
	p_mp4->i_numframe++;

	return i_size;
}

const cli_output_t mp4_output = { open_file, set_param, write_headers, write_frame, close_file };