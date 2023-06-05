#include "main.h"
/*************************************************************************
    Startbildschirm und Anzahl der Argumente

    Intro Screen and argument check
*************************************************************************/

/******************************************************************
	Status_Info

	prints a status line during multiplexing
******************************************************************/

void status_info (
unsigned int nsectors_a,
unsigned int nsectors_a1,
unsigned int nsectors_v,
unsigned int nsectors_p,
double nbytes,
unsigned int buf_v,
unsigned int buf_a,
unsigned int buf_a1)
{
 // fprintf(statfile, "  | %7d | %7d | %7d | %7d | %11.0f | %6d | %6d | %6d |\n",
   //   nsectors_a, nsectors_a1, nsectors_v, nsectors_p, nbytes, buf_a, buf_a1, buf_v);
}

void status_header(char *filename)
{
 // fprintf(statfile, "\n");
 // fprintf(statfile, "Multiplexing file %s\n\n", filename);
 // status_footer();
 // fprintf(statfile, "  |  Audio1 |  Audio2 |  Video  | Padding | Bytes  MPEG | Audio1 | Audio2 | Video  |\n");
 // fprintf(statfile, "  | Sectors | Sectors | Sectors | Sectors | System File | Buffer | Buffer | Buffer |\n");
 // status_footer();
}


void status_message (
unsigned char what)
{
	/*
  switch (what)
  {
    case STATUS_AUDIO_END:
      fprintf(statfile, "  |file  end|         |         |         |             |        |        |        |\n");
      break;
    case STATUS_AUDIO_TIME_OUT:
      fprintf(statfile, "  |time  out|         |         |         |             |        |        |        |\n");
      break;
    case STATUS_AUDIO1_END:
      fprintf(statfile, "  |         |file  end|         |         |             |        |        |        |\n");
      break;
    case STATUS_AUDIO1_TIME_OUT:
      fprintf(statfile, "  |         |time  out|         |         |             |        |        |        |\n");
      break;
    case STATUS_VIDEO_END:
      fprintf(statfile, "  |         |         |file  end|         |             |        |        |        |\n");
      break;
    case STATUS_VIDEO_TIME_OUT:
      fprintf(statfile, "  |         |         |time  out|         |             |        |        |        |\n");
  }
  */
}

void status_footer ()
{
 // fprintf(statfile, "  +---------+---------+---------+---------+-------------+--------+--------+--------+\n");
}
