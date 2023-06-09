
#include "avformat.h"


/* If you do not call this function, then you can select exactly which
   formats you want to support */

/**
 * Initialize libavcodec and register all the codecs and formats.
 */
void av_register_all(void)
{
    avcodec_init();
    avcodec_register_all();

    mpegps_init();
   // mpegts_init();
#ifdef CONFIG_ENCODERS
  //  crc_init();
    img_init();
#endif //CONFIG_ENCODERS
     raw_init();
    //mp3_init();
  //  rm_init();
#ifdef CONFIG_RISKY
    //asf_init();
#endif
#ifdef CONFIG_ENCODERS
  //  avienc_init();
#endif //CONFIG_ENCODERS
 //   avidec_init();
 //   ff_wav_init();
 //   swf_init();
 //   au_init();
#ifdef CONFIG_ENCODERS
 //   gif_init();
#endif //CONFIG_ENCODERS
 //   mov_init();
#ifdef CONFIG_ENCODERS
//    movenc_init();
//    jpeg_init();
#endif //CONFIG_ENCODERS
 //   ff_dv_init();
 //   fourxm_init();
#ifdef CONFIG_ENCODERS
  //  flvenc_init();
#endif //CONFIG_ENCODERS
  //  flvdec_init();
  //  str_init();
 //   roq_init();
  //  ipmovie_init();
  //  wc3_init();
  //  westwood_init();
  //  film_init();
  //  idcin_init();
 //   flic_init();
 //   vmd_init();

#if defined(AMR_NB) || defined(AMR_NB_FIXED) || defined(AMR_WB)
  //  amr_init();
#endif
    yuv4mpeg_init();
    
#ifdef CONFIG_VORBIS
  //  ogg_init();
#endif

#ifndef CONFIG_WIN32
  //  ffm_init();
#endif
#ifdef CONFIG_VIDEO4LINUX
 //   video_grab_init();
#endif
#if defined(CONFIG_AUDIO_OSS) || defined(CONFIG_AUDIO_BEOS)
 //   audio_init();
#endif

#ifdef CONFIG_DV1394
 //   dv1394_init();
#endif

//	CRASH();
 //   nut_init();
//	CRASH();
  //  matroska_init();

#ifdef CONFIG_ENCODERS
    /* image formats */
 //   av_register_image_format(&pnm_image_format);
 //   av_register_image_format(&pbm_image_format);
 //   av_register_image_format(&pgm_image_format);
 //   av_register_image_format(&ppm_image_format);
 //   av_register_image_format(&pam_image_format);
    //av_register_image_format(&pgmyuv_image_format);
	  av_register_image_format(&yuv_image_format);
#ifdef CONFIG_ZLIB
 //   av_register_image_format(&png_image_format);
#endif
  //  av_register_image_format(&jpeg_image_format);
  //  av_register_image_format(&gif_image_format);
  //  av_register_image_format(&sgi_image_format);
#endif //CONFIG_ENCODERS

    /* file protocols */
      register_protocol(&file_protocol);
//    register_protocol(&pipe_protocol);
#ifdef CONFIG_NETWORK
  //  rtsp_init();
 //   rtp_init();
  //  register_protocol(&udp_protocol);
 //   register_protocol(&rtp_protocol);
 //   register_protocol(&tcp_protocol);
 //   register_protocol(&http_protocol);
#endif
}


