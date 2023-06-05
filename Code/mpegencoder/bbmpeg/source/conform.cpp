/* conform.c, conformance checks                                            */

/* Copyright (C) 1996, MPEG Software Simulation Group. All Rights Reserved. */

/*
 * Disclaimer of Warranty
 *
 * These software programs are available to the user without any license fee or
 * royalty on an "as is" basis.  The MPEG Software Simulation Group disclaims
 * any and all warranties, whether express, implied, or statuary, including any
 * implied warranties or merchantability or of fitness for a particular
 * purpose.  In no event shall the copyright-holder be liable for any
 * incidental, punitive, or consequential damages of any kind whatsoever
 * arising from the use of these programs.
 *
 * This disclaimer of warranty extends to the user of these programs and user's
 * customers, employees, agents, transferees, successors, and assigns.
 *
 * The MPEG Software Simulation Group does not represent or warrant that the
 * programs furnished hereunder are free of infringement of any third-party
 * patents.
 *
 * Commercial implementations of MPEG-1 and MPEG-2 video, including shareware,
 * are subject to royalty fees to patent holders.  Many of these patents are
 * general enough such that they are unavoidable regardless of implementation
 * design.
 *
 */

#include "main.h"

static double ratetab[8]=
    {24000.0/1001.0,24.0,25.0,30000.0/1001.0,30.0,50.0,60000.0/1001.0,60.0};
static int ratetab1[4] = {62668800, 47001600, 10368000, 3041280};

/* check inputs */
void input_range_checks()
{
  /* range and value checks */

  InvalidAudio = FALSE;
  InvalidVideo = FALSE;
  if (VideoAvail)
  {
    if ((horizontal_size < 1) || (horizontal_size > 1920))
    {
      //DisplayError("Horizontal size must be in the range 1 .. 1920.");
      InvalidVideo = TRUE;
    }
    if ((vertical_size < 1) || (vertical_size > 1152))
    {
   //   DisplayError("Vertical size must be in the range 1 .. 1152.");
      InvalidVideo = TRUE;
    }
    if (horizontal_size % 2 != 0)
    {
    //  DisplayError("Horizontal size must be a even.");
      InvalidVideo = TRUE;
    }

    if (vertical_size % 2 != 0)
    {
      //DisplayError("Vertical size must be a even.");
      InvalidVideo = TRUE;
    }
    if (!InvalidVideo)
    {
      if ((horizontal_size > 1440) && (level != 4))
      {
        level = 4;
        if (profile > 4)
          profile = 4;
      }
      if ((horizontal_size > 720 || vertical_size > 576) && (level > 6))
      {
        level = 6;
        if (profile > 4)
          profile = 4;
      }
      if ((horizontal_size > 352 || vertical_size > 288) && (level > 8))
        level = 8;

      while ((frame_rate_code > 3) && (horizontal_size * vertical_size * frame_rate > ratetab1[(level - 4) >> 1]))
      {
        frame_rate_code--;
        frame_rate = ratetab[frame_rate_code - 1];
      }
    }
  }

  if (AudioAvail)
  {
    if (audio_mode == MPG_MD_MONO && audio_bitrate > 10)
      audio_bitrate = 10;
    if (audioSampleSize != 16)
    {
      //DisplayError("Audio sample size must be 16 bits.");
      InvalidAudio = TRUE;
    }
    if ((audioSampleRate != 32000) &&
        (audioSampleRate != 44100) &&
        (audioSampleRate != 48000))
    {
      //DisplayError("Audio sample rate must be 32, 44.1 or 48 kHz.");
      InvalidAudio = TRUE;
    }
  }
  if (InvalidAudio)
    AudioAvail = FALSE;
  if (InvalidVideo)
    VideoAvail = FALSE;
}


int CheckVideoSettings(mpegOutSettings *set)
{
  int i;

  if ((set->N < 1) || (set->N > MAXN))
    return FALSE;
  if ((set->M < 1) || (set->M > MAXM))
    return FALSE;
  if (set->N % set->M != 0)
    return FALSE;

  /* make sure MPEG specific parameters are valid */
  if (!range_checks(set))
    return FALSE;

  if (set->video_type > MPEG_VCD)
  {
    if (!profile_and_level_checks(set))
      return FALSE;
  }
  else
  {
    /* MPEG-1 */
    if (set->constrparms)
    {
      if (horizontal_size>768
          || vertical_size>576
          || ((horizontal_size+15)/16)*((vertical_size+15)/16)>396
          || ((horizontal_size+15)/16)*((vertical_size+15)/16)*set->frame_rate>396*25
          || set->frame_rate>30.0)
        return FALSE;
    }

    if (set->constrparms)
    {
      for (i=0; i<set->M; i++)
      {
        if (set->motion_data[i].forw_hor_f_code>4)
          return FALSE;

        if (set->motion_data[i].forw_vert_f_code>4)
          return FALSE;

        if (i!=0)
        {
          if (set->motion_data[i].back_hor_f_code>4)
            return FALSE;

          if (set->motion_data[i].back_vert_f_code>4)
            return FALSE;
        }
      }
    }
  }

  /* relational checks */
  if (set->video_type < MPEG_MPEG2)
  {
    if (!set->prog_seq)
      return FALSE;

    if (set->chroma_format!=CHROMA420)
      return FALSE;

    if (set->dc_prec!=0)
      return FALSE;

    for (i=0; i<3; i++)
    {
      if (set->qscale_tab[i])
        return FALSE;

      if (set->intravlc_tab[i])
        return FALSE;

      if (set->altscan_tab[i])
        return FALSE;
    }
  }

  if ((set->video_type > MPEG_VCD) && set->constrparms)
    return FALSE;

  if (set->prog_seq && !set->prog_frame)
    return FALSE;

  if (set->prog_frame && set->fieldpic)
    return FALSE;

  if (!set->prog_frame && set->repeatfirst)
    return FALSE;

  if (set->prog_frame)
  {
    for (i=0; i<3; i++)
      if (!set->frame_pred_dct_tab[i])
        return FALSE;
  }

  if (set->prog_seq && !set->repeatfirst && set->topfirst)
    return FALSE;

  /* search windows */
  for (i=0; i < set->M; i++)
  {
    if (set->motion_data[i].sxf > (4<<set->motion_data[i].forw_hor_f_code)-1)
      return FALSE;

    if (set->motion_data[i].syf > (4<<set->motion_data[i].forw_vert_f_code)-1)
      return FALSE;

    if (i!=0)
    {
      if (set->motion_data[i].sxb > (4<<set->motion_data[i].back_hor_f_code)-1)
        return FALSE;

      if (set->motion_data[i].syb > (4<<set->motion_data[i].back_vert_f_code)-1)
        return FALSE;
    }
  }
  return TRUE;
}

int CheckAudioSettings(mpegOutSettings *set)
{
  if ((set->audio_layer < 1) || (set->audio_layer > 2))
    return FALSE;

  if ((set->psych_model < 1) || (set->psych_model > 2))
    return FALSE;

  if ((set->audio_bitrate < 0) || (set->audio_bitrate >  14))
    return FALSE;

  if ((set->emphasis != 0) && (set->emphasis != 1) && (set->emphasis != 3))
    return FALSE;

  if ((set->extension < 0) || (set->extension > 1))
    return FALSE;

  if ((set->error_protection < 0) || (set->error_protection > 1))
    return FALSE;

  if ((set->copyright < 0) || (set->copyright > 1))
    return FALSE;

  if ((set->original < 0) || (set->original > 1))
    return FALSE;

  return TRUE;
}

/* check for (level independent) parameter limits */
int range_checks(mpegOutSettings *set)
{
  int i;

  /* range and value checks */

  if (set->video_type < MPEG_MPEG2)
  {
    if (set->aspectratio<1 || set->aspectratio>14)
      return FALSE;
  }
  else
  {
    if (set->aspectratio<1 || set->aspectratio>4)
      return FALSE;
  }

  if (set->frame_rate_code<1 || set->frame_rate_code>8)
    return FALSE;

  if (set->bit_rate<=0.0)
    return FALSE;

  if (set->video_type < MPEG_MPEG2)
  {
    if (set->bit_rate > ((1<<18)-1)*400.0)
      return FALSE;
  }
  else
  {
    if (set->bit_rate > ((1<<30)-1)*400.0)
      return FALSE;
  }

  if (set->video_type < MPEG_MPEG2)
  {
    if (set->vbv_buffer_size<1 || set->vbv_buffer_size>=1024)
      return FALSE;
  }
  else
  {
    if (set->vbv_buffer_size<1 || set->vbv_buffer_size>0x3ffff)
      return FALSE;
  }

  if (set->chroma_format<CHROMA420 || set->chroma_format>CHROMA444)
    return FALSE;

  if (set->video_format<0 || set->video_format>4)
    return FALSE;

  if (set->color_primaries<1 || set->color_primaries>7 || set->color_primaries==3)
    return FALSE;

  if (set->transfer_characteristics<1 || set->transfer_characteristics>7
      || set->transfer_characteristics==3)
    return FALSE;

  if (set->matrix_coefficients<1 || set->matrix_coefficients>7 || set->matrix_coefficients==3)
    return FALSE;

  if (set->display_horizontal_size<0 || set->display_horizontal_size>16383)
    return FALSE;

  if (set->display_vertical_size<0 || set->display_vertical_size>16383)
    return FALSE;

  if (set->dc_prec<0 || set->dc_prec>3)
    return FALSE;

  for (i=0; i < set->M; i++)
  {
    if (set->video_type < MPEG_MPEG2)
    {
      if (set->motion_data[i].forw_hor_f_code<1 || set->motion_data[i].forw_hor_f_code>7)
        return FALSE;
      if (set->motion_data[i].forw_vert_f_code<1 || set->motion_data[i].forw_vert_f_code>7)
        return FALSE;
    }
    else
    {
      if (set->motion_data[i].forw_hor_f_code<1 || set->motion_data[i].forw_hor_f_code>9)
        return FALSE;
      if (set->motion_data[i].forw_vert_f_code<1 || set->motion_data[i].forw_vert_f_code>9)
        return FALSE;
    }
    if (set->motion_data[i].sxf<=0)
      return FALSE;
    if (set->motion_data[i].syf<=0)
      return FALSE;
    if (i!=0)
    {
      if (set->video_type < MPEG_MPEG2)
      {
        if (set->motion_data[i].back_hor_f_code<1 || set->motion_data[i].back_hor_f_code>7)
          return FALSE;
        if (set->motion_data[i].back_vert_f_code<1 || set->motion_data[i].back_vert_f_code>7)
          return FALSE;
      }
      else
      {
        if (set->motion_data[i].back_hor_f_code<1 || set->motion_data[i].back_hor_f_code>9)
          return FALSE;
        if (set->motion_data[i].back_vert_f_code<1 || set->motion_data[i].back_vert_f_code>9)
          return FALSE;
      }
      if (set->motion_data[i].sxb<=0)
        return FALSE;
      if (set->motion_data[i].syb<=0)
        return FALSE;
    }
  }
  return TRUE;
}

/* identifies valid profile / level combinations */
static char profile_level_defined[5][4] =
{
/* HL   H-14 ML   LL  */
  {1,   1,   1,   0},  /* HP   */
  {0,   1,   0,   0},  /* Spat */
  {0,   0,   1,   1},  /* SNR  */
  {1,   1,   1,   1},  /* MP   */
  {0,   0,   1,   0}   /* SP   */
};

static struct level_limits {
  int hor_f_code;
  int vert_f_code;
  int hor_size;
  int vert_size;
  int sample_rate;
  int bit_rate; /* Mbit/s */
  int vbv_buffer_size; /* 16384 bit steps */
} maxval_tab[4] =
{
  {9, 5, 1920, 1152, 62668800, 80, 597}, /* HL */
  {9, 5, 1440, 1152, 47001600, 60, 448}, /* H-14 */
  {8, 5,  720,  576, 10368000, 15, 112}, /* ML */
  {7, 4,  352,  288,  3041280,  4,  29}  /* LL */
};

#define SP   5
#define MP   4
#define SNR  3
#define SPAT 2
#define HP   1

#define LL  10
#define ML   8
#define H14  6
#define HL   4

int profile_and_level_checks(mpegOutSettings *set)
{
  int i;
  struct level_limits *maxval;

  if ((set->profile != 1) && (set->profile != 4) && (set->profile != 5))
    return FALSE;

  if ((set->level != 4) && (set->level != 6) && (set->level != 8) && (set->level != 10))
    return FALSE;

  maxval = &maxval_tab[(set->level-4) >> 1];

  /* check profile@level combination */
  if(!profile_level_defined[set->profile-1][(set->level-4) >> 1])
    return FALSE;

  /* profile (syntax) constraints */

  if ((set->profile == SP) && (set->M != 1))
    return FALSE;

  if ((set->profile != HP) && (set->chroma_format != CHROMA420))
    return FALSE;

  if ((set->profile == HP) && (set->chroma_format != CHROMA420) && (set->chroma_format != CHROMA422))
    return FALSE;

  if (set->profile >= MP) /* SP, MP: constrained repeat_first_field */
  {
    if (set->frame_rate_code<=2 && set->repeatfirst)
      return FALSE;
    if (set->frame_rate_code<=6 && set->prog_seq && set->repeatfirst)
      return FALSE;
  }

  if (set->profile!=HP && set->dc_prec==3)
    return FALSE;

  /* level (parameter value) constraints */

  /* Table 8-8 */
  if (set->frame_rate_code>5 && set->level>=ML)
    return FALSE;

  for (i=0; i<set->M; i++)
  {
    if (set->motion_data[i].forw_hor_f_code > maxval->hor_f_code)
      return FALSE;
    if (set->motion_data[i].forw_vert_f_code > maxval->vert_f_code)
      return FALSE;
    if (i!=0)
    {
      if (set->motion_data[i].back_hor_f_code > maxval->hor_f_code)
        return FALSE;
      if (set->motion_data[i].back_vert_f_code > maxval->vert_f_code)
        return FALSE;
    }
  }

  /* Table 8-10 */
  if (horizontal_size > maxval->hor_size)
    return FALSE;

  if (vertical_size > maxval->vert_size)
    return FALSE;

  /* Table 8-11 */
  if (horizontal_size*vertical_size*set->frame_rate > maxval->sample_rate)
    return FALSE;

  /* Table 8-12 */
  if (set->bit_rate> 1.0e6 * maxval->bit_rate)
    return FALSE;

  /* Table 8-13 */
  if (set->vbv_buffer_size > maxval->vbv_buffer_size)
    return FALSE;

  return TRUE;
}
