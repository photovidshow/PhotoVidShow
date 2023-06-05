/* domplex.cpp */

#include "main.h"

int domplex(int mplex_video, int mplex_audio)
{
  char	*video_units = NULL;
  char	*audio_units = NULL;
  char  *audio1_units = NULL;

  Video_struc video_info;
  Audio_struc audio_info;
  Audio_struc audio1_info;
  unsigned int audio_total, audio1_total, video_total;
  double audio_bytes, audio1_bytes, video_bytes;
  unsigned int which_streams=0;
  double	startup_delay=0;

  if (mplex_video)
    which_streams |= STREAMS_VIDEO;
  if (mplex_audio && strlen(AudioFilename))
    which_streams |= STREAMS_AUDIO;
  if (mplex_audio && (mplex_type != MPEG_VCD) && strlen(Audio1Filename))
    which_streams |= STREAMS_AUDIO1;

  if (!check_files(&audio_bytes, &audio1_bytes, &video_bytes, which_streams))
    return FALSE;

  empty_video_struc(&video_info);
  empty_audio_struc(&audio_info);
  empty_audio_struc(&audio1_info);

  if (which_streams & STREAMS_VIDEO)
  {
    video_units = tempnam("./","tty2");
    if (!get_info_video (VideoFilename, video_units, &video_info, &startup_delay,
			&video_total, video_bytes))
      return FALSE;
    DisplayProgress("", 0);
  }
  else
    video_total = 0;

  if (which_streams & STREAMS_AUDIO)
  {
    audio_units = tempnam ("./","tty");
    if (!get_info_audio (AudioFilename, audio_units, &audio_info, &startup_delay,
			&audio_total, audio_bytes))
      return FALSE;
    DisplayProgress("", 0);
  }
  else
    audio_total = 0;

  if (which_streams & STREAMS_AUDIO1)
  {
    audio1_units = tempnam ("./","tty1");
    if (!get_info_audio (Audio1Filename, audio1_units, &audio1_info, &startup_delay,
			&audio1_total, audio1_bytes))
      return FALSE;
    DisplayProgress("", 0);
  }
  else
    audio1_total = 0;

  return outputstream (doCompileRecSave->outputFileRef, video_units,
                       &video_info, audio_units, &audio_info, audio1_units, &audio1_info,
                       video_total, audio_total, audio1_total, which_streams );
}


