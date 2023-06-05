/* dovideo.c */
#include "main.h"
#include "consts1.h"
#include <time.h>
#include <sys\stat.h>

#define MAXINT 2147483647

static int putMaxBitrate();

static FILE *vFile;
static char *max_name;
static char doVBRLimit;

static int ReadQuantMat()
{
  int i,v;
  FILE *fd;

  if (strlen(iqname) == 0)
  {
    /* use default intra matrix */
    load_iquant = 0;
    for (i=0; i<64; i++)
      intra_q[i] = default_intra_quantizer_matrix[i];
  }
  else
  {
    /* read customized intra matrix */
    load_iquant = 1;
    fd = fopen(iqname,"r");
    if (fd == NULL)
    {
 //     sprintf(errortext,"Could not open intra quant matrix file %s.",iqname);
      //DisplayError(errortext);
      return FALSE;
    }

    for (i=0; i<64; i++)
    {
      fscanf(fd,"%d",&v);
      if (v<1 || v>255)
      {
      //  sprintf(errortext, "Invalid value in intra quant matrix file %s.", iqname);
        //DisplayError(errortext);
        return FALSE;
      }
      intra_q[i] = (unsigned char) v;
    }

    fclose(fd);
  }

  if (strlen(niqname) == 0)
  {
    /* use default non-intra matrix */
    load_niquant = 0;
    for (i=0; i<64; i++)
    {
      inter_q[i] = default_non_intra_quantizer_matrix[i];
      s_inter_q[i] = default_non_intra_quantizer_matrix[i];
      i_inter_q[i] = (int)(((double)IQUANT_SCALE) / ((double)s_inter_q[i]));
    }
  }
  else
  {
    if (!strcmp(niqname, "mpeg default"))
    {
      /* use the MPEG default non-intra matrix */
      load_niquant = 0;
      for (i=0; i<64; i++)
      {
        inter_q[i] = 16;
        s_inter_q[i] = 16;
        i_inter_q[i] = (int)(((double)IQUANT_SCALE) / ((double)16));
      }
    }
    else
    {
      /* read customized non-intra matrix */
      load_niquant = 1;
      fd = fopen(niqname,"r");
      if (fd == NULL)
      {
     //   sprintf(errortext,"Could not open non-intra quant matrix file %s.",niqname);
        //DisplayError(errortext);
        return FALSE;
      }

      for (i=0; i<64; i++)
      {
        fscanf(fd,"%d",&v);
        if (v<1 || v>255)
        {
    //      sprintf(errortext, "Invalid value in non-intra quant matrix file %s.", niqname);
    //      DisplayError(errortext);
          return FALSE;
        }
        inter_q[i] = (unsigned char) v;
        s_inter_q[i] = (unsigned short) v;
        i_inter_q[i] = (int)(((double)IQUANT_SCALE) / ((double)v));
      }

      fclose(fd);
    }
  }
  return TRUE;
}

static void FinishVideo()
{
  int i;

  finish_putbits(&videobs);
  end_readframe();

  if (vFile)
    fclose(vFile);

  if (orgclp)
  {
    free(orgclp);
    orgclp = NULL;
    clp = NULL;
  }

  for (i=0; i<3; i++)
  {
    if (newrefframe[i])
      free(newrefframe[i]);
    newrefframe[i] = NULL;
    if (oldrefframe[i])
      free(oldrefframe[i]);
    oldrefframe[i] = NULL;
    if (auxframe[i])
      free(auxframe[i]);
    auxframe[i] = NULL;
    if (neworgframe[i])
      free(neworgframe[i]);
    neworgframe[i] = NULL;
    if (oldorgframe[i])
      free(oldorgframe[i]);
    oldorgframe[i] = NULL;
    if (auxorgframe[i])
      free(auxorgframe[i]);
    auxorgframe[i] = NULL;
    if (predframe[i])
      free(predframe[i]);
    predframe[i] = NULL;

    if (unewrefframe[i])
      free(unewrefframe[i]);
    unewrefframe[i] = NULL;
    if (uoldrefframe[i])
      free(uoldrefframe[i]);
    uoldrefframe[i] = NULL;
    if (uneworgframe[i])
      free(uneworgframe[i]);
    uneworgframe[i] = NULL;
    if (uoldorgframe[i])
      free(uoldorgframe[i]);
    uoldorgframe[i] = NULL;
    if (uauxframe[i])
      free(uauxframe[i]);
    uauxframe[i] = NULL;
    if (uauxorgframe[i])
      free(uauxorgframe[i]);
    uauxorgframe[i] = NULL;
    if (upredframe[i])
      free(upredframe[i]);
    upredframe[i] = NULL;
  }

  if (mbinfo)
    free(mbinfo);
  mbinfo = NULL;

  if (umbinfo)
    free(umbinfo);
  umbinfo = NULL;

  if (blocks)
    free(blocks);
  blocks = NULL;

  if (ublocks)
    free(ublocks);
  ublocks = NULL;

  if (ubuffer)
    free(ubuffer);
  ubuffer = NULL;

  if (maxmotion > 7)
    finish_motion_est();
}

static int InitVideo()
{
  int i, size;
  static int block_count_tab[3] = {6,8,12};

  vFile = NULL;
  orgclp = NULL;
  clp = NULL;
  for (i = 0; i < 3; i++)
  {
    newrefframe[i] = NULL;
    oldrefframe[i] = NULL;
    auxframe[i] = NULL;
    neworgframe[i] = NULL;
    oldorgframe[i] = NULL;
    auxorgframe[i] = NULL;
    predframe[i] = NULL;
    unewrefframe[i] = NULL;
    uoldrefframe[i] = NULL;
    uauxframe[i] = NULL;
    uneworgframe[i] = NULL;
    uoldorgframe[i] = NULL;
    uauxorgframe[i] = NULL;
    upredframe[i] = NULL;
  }
  mbinfo = NULL;
  umbinfo = NULL;
  blocks = NULL;
  ublocks = NULL;
  ubuffer = NULL;

  /* open output file */
  if (!init_putbits(&videobs, VideoFilename))
    return FALSE;

  /* read quantization matrices */
  if (!ReadQuantMat())
    return FALSE;

  if ((video_type > MPEG_VCD) && !constant_bitrate && !max_bit_rate)
  {
    max_name = tempnam("./","tmp_s");
    vFile = fopen(max_name, "wb");
    if (vFile == NULL)
    {
      //DisplayError("Could not create temporary max bitrate output file.");
      return FALSE;
    }
  }

  doVBRLimit = !constant_bitrate && (max_bit_rate || avg_bit_rate || min_bit_rate);

  init_fdct();
  init_idct();

  /* round picture dimensions to nearest multiple of 16 or 32 */
  mb_width = (horizontal_size+15)/16;
  mb_height = prog_seq ? (vertical_size+15)/16 : 2*((vertical_size+31)/32);
  mb_height2 = fieldpic ? mb_height>>1 : mb_height; /* for field pictures */
  width = 16*mb_width;
  height = 16*mb_height;

  chrom_width = (chroma_format==CHROMA444) ? width : width>>1;
  chrom_height = (chroma_format!=CHROMA420) ? height : height>>1;

  height2 = fieldpic ? height>>1 : height;
  width2 = fieldpic ? width<<1 : width;
  chrom_width2 = fieldpic ? chrom_width<<1 : chrom_width;

  block_count = block_count_tab[chroma_format-1];

  vbvOverflows = 0;
  vbvUnderflows = 0;
  paddingSum = 0.0;
  maxPadding = 0;
  headerSum = 0.0;

  /* clip table */
  clp = (unsigned char *)malloc(1024);
  if (clp == NULL)
  {
    //DisplayError("Cannot allocate memory for clip table.");
    return FALSE;
  }
  orgclp = clp;
  clp+= 384;
  for (i = -384; i < 640; i++)
    clp[i] = (unsigned char) ((i<0) ? 0 : ((i>255) ? 255 : i));

  for (i=0; i<3; i++)
  {
    size = (i==0) ? width*height : chrom_width*chrom_height;

    newrefframe[i] = (unsigned char *)malloc(size);
    if (newrefframe[i] == NULL)
    {
      //DisplayError("Cannot allocate memory for new ref frame.");
      return FALSE;
    }
    oldrefframe[i] = (unsigned char *)malloc(size);
    if (oldrefframe[i] == NULL)
    {
//      DisplayError("Cannot allocate memory for old ref frame.");
      return FALSE;
    }
    auxframe[i] = (unsigned char *)malloc(size);
    if (auxframe[i] == NULL)
    {
  //    DisplayError("Cannot allocate memory for aux frame.");
      return FALSE;
    }
    neworgframe[i] = (unsigned char *)malloc(size);
    if (neworgframe[i] == NULL)
    {
      //DisplayError("Cannot allocate memory for new org frame.");
      return FALSE;
    }
    oldorgframe[i] = (unsigned char *)malloc(size);
    if (oldorgframe[i] == NULL)
    {
      //DisplayError("Cannot allocate memory for old org frame.");
      return FALSE;
    }
    auxorgframe[i] = (unsigned char *)malloc(size);
    if (auxorgframe[i] == NULL)
    {
     // DisplayError("Cannot allocate memory for aux org frame.");
      return FALSE;
    }
    predframe[i] = (unsigned char *)malloc(size);
    if (predframe[i] == NULL)
    {
      //DisplayError("Cannot allocate memory for pred frame.");
      return FALSE;
    }

    if (doVBRLimit)
    {
      unewrefframe[i] = (unsigned char *)malloc(size);
      if (unewrefframe[i] == NULL)
      {
      //  DisplayError("Cannot allocate memory for new ref frame.");
        return FALSE;
      }
      uoldrefframe[i] = (unsigned char *)malloc(size);
      if (uoldrefframe[i] == NULL)
      {
        //DisplayError("Cannot allocate memory for old ref frame.");
        return FALSE;
      }
      uauxframe[i] = (unsigned char *)malloc(size);
      if (uauxframe[i] == NULL)
      {
      //  DisplayError("Cannot allocate memory for aux frame.");
        return FALSE;
      }
      uneworgframe[i] = (unsigned char *)malloc(size);
      if (uneworgframe[i] == NULL)
      {
        //DisplayError("Cannot allocate memory for new org frame.");
        return FALSE;
      }
      uoldorgframe[i] = (unsigned char *)malloc(size);
      if (uoldorgframe[i] == NULL)
      {
      //  DisplayError("Cannot allocate memory for old org frame.");
        return FALSE;
      }
      uauxorgframe[i] = (unsigned char *)malloc(size);
      if (uauxorgframe[i] == NULL)
      {
        //DisplayError("Cannot allocate memory for aux org frame.");
        return FALSE;
      }
      upredframe[i] = (unsigned char *)malloc(size);
      if (upredframe[i] == NULL)
      {
      //  DisplayError("Cannot allocate memory for pred frame.");
        return FALSE;
      }
    }
  }

  mbinfo = (struct mbinfo *)malloc(mb_width*mb_height2*sizeof(struct mbinfo));
  if (!mbinfo)
  {
    //DisplayError("Cannot allocate memory for mb info.");
    return FALSE;
  }

  if (doVBRLimit)
  {
    umbinfo = (struct mbinfo *)malloc(mb_width*mb_height2*sizeof(struct mbinfo));
    if (!umbinfo)
    {
   //   DisplayError("Cannot allocate memory for mb info.");
      return FALSE;
    }
  }

  blocks =
    (short (*)[64])malloc(mb_width*mb_height2*block_count*sizeof(short [64]));
  if (!blocks)
  {
    //DisplayError("Cannot allocate memory for blocks.");
    return FALSE;
  }

  if (doVBRLimit)
  {
    ublocks =
      (short (*)[64])malloc(mb_width*mb_height2*block_count*sizeof(short [64]));
    if (!ublocks)
    {
   //   DisplayError("Cannot allocate memory for blocks.");
      return FALSE;
    }

    ubuffer = (unsigned char*)malloc(BUFFER_SIZE);
    if (!ubuffer)
    {
      //DisplayError("Cannot allocate memory for undo file buffer.");
      return FALSE;
    }
  }

  if (!init_readframe())
    return FALSE;

  if (maxmotion > 7)
    if (!init_motion_est2())
      return FALSE;

  return TRUE;
}

int GetFCode(int sw)
{
  if (sw < 8)
    return 1;
  if (sw < 16)
    return 2;
  if (sw < 32)
    return 3;
  if (sw < 64)
    return 4;
  if (sw < 128)
    return 5;
  if (sw < 256)
    return 6;
  if (sw < 512)
    return 7;
  if (sw < 1024)
    return 8;
  if (sw < 2048)
    return 9;
  return 1;
}

void DoVarMotion()
{
  char tmpStr[256];

  switch (pict_type)
  {
    case P_TYPE:
      if (Sxf > maxmotion + 5)
      {
     //   sprintf(tmpStr, "Warning, horz forward motion vector larger than max, vector = %d, max = %d.", Sxf, maxmotion + 5);
     //   DisplayInfo(tmpStr);
      }
      if (Syf > maxmotion + 5)
      {
   //     sprintf(tmpStr, "Warning, vert forward motion vector larger than max, vector = %d, max = %d.", Syf, maxmotion + 5);
    //    DisplayInfo(tmpStr);
      }
      forw_hor_f_code = GetFCode(Sxf);
      forw_vert_f_code = GetFCode(Syf);

      /* keep MPEG-1 horz/vert f_codes the same */
      if ((video_type < MPEG_MPEG2) && (forw_hor_f_code != forw_vert_f_code))
      {
        if (forw_hor_f_code > forw_vert_f_code)
          forw_vert_f_code = forw_hor_f_code;
        else
          forw_hor_f_code = forw_vert_f_code;
      }
//      sprintf(tmpStr, "Sxf,Syf = %d,%d", Sxf, Syf);
//      DisplayInfo(tmpStr);
      break;

    case B_TYPE:
      if (Sxf > maxmotion + 5)
      {
     //   sprintf(tmpStr, "Warning, horz forward motion vector larger than max, vector = %d, max = %d.", Sxf, maxmotion + 5);
     //   DisplayInfo(tmpStr);
      }
      if (Syf > maxmotion + 5)
      {
     //   sprintf(tmpStr, "Warning, vert forward motion vector larger than max, vector = %d, max = %d.", Syf, maxmotion + 5);
     //   DisplayInfo(tmpStr);
      }
      if (Sxb > maxmotion + 5)
      {
      //  sprintf(tmpStr, "Warning, horz backward motion vector larger than max, vector = %d, max = %d.", Sxb, maxmotion + 5);
      //  DisplayInfo(tmpStr);
      }
      if (Syb > maxmotion + 5)
      {
     //   sprintf(tmpStr, "Warning, vert backward motion vector larger than max, vector = %d, max = %d.", Syb, maxmotion + 5);
      //  DisplayInfo(tmpStr);
      }

      forw_hor_f_code = GetFCode(Sxf);
      forw_vert_f_code = GetFCode(Syf);
      back_hor_f_code = GetFCode(Sxb);
      back_vert_f_code = GetFCode(Syb);

      /* keep MPEG-1 forw horz/vert f_codes the same */
      if ((video_type < MPEG_MPEG2) && (forw_hor_f_code != forw_vert_f_code))
      {
        if (forw_hor_f_code > forw_vert_f_code)
          forw_vert_f_code = forw_hor_f_code;
        else
          forw_hor_f_code = forw_vert_f_code;
      }

      /* keep MPEG-1 back horz/vert f_codes the same */
      if ((video_type < MPEG_MPEG2) && (back_hor_f_code != back_vert_f_code))
      {
        if (back_hor_f_code > back_vert_f_code)
          back_vert_f_code = back_hor_f_code;
        else
          back_hor_f_code = back_vert_f_code;
      }
//      sprintf(tmpStr, "Sxf,Syf = %d,%d, Sxb,Syb = %d,%d", Sxf, Syf, Sxb, Syb);
//      DisplayInfo(tmpStr);
      break;
  }
}

int dovideo()
{
  /* this routine assumes (N % M) == 0 */
  int i, f0, f, n;
  int j, k, np, nb, undoi;
  int sxf, syf, sxb, syb;
  int ipflag, gopCount;
  char tmpStr[255];
  unsigned char *neworg[3], *newref[3], *uneworg[3], *unewref[3];
  static char ipb[5] = {' ','I','P','B','D'};
  unsigned int percent, hours, min, sec, tot_t, byteCount;
  time_t start_t, end_t;
  double frame_t, lastbitrate, fSize;
  int availablebr, sparedbitrate, calcbr, originalmquant;
  int averagebitrate, absmaxbitrate, absminbitrate, maxbr, minbr;
  int new_mquant, size, max_done;
  bitstream undobs;
  int initial_mq;
  double factor;

  if (!InitVideo())
  {
    FinishVideo();
    return FALSE;
  }

  time(&start_t);

  if (maxmotion > 7)
  {
    submotiop = maxmotion >> 2;
    submotiob = (submotiop * 3) >> 2;
  }

  init_motion_est();
  init_rgb_to_yuv_mmx(matrix_coefficients);
  init_transform();
  rc_init_seq(); /* initialize rate control */

  alignbits(&videobs); // moved from puthdr
  if (vFile)
  {
    byteCount = (unsigned int) floor(bitcount(&videobs) / 8.0) + 8;
    if (fwrite(&byteCount, 1, sizeof(byteCount), vFile) != sizeof(byteCount))
    {
      FinishVideo();
      //DisplayError("Unable to write to temporary max bitrate file.");
      return FALSE;
    }
  }
  /* sequence header, sequence extension and sequence display extension */
  putseqhdr();
  if (video_type > MPEG_VCD)
  {
    putseqext();
    putseqdispext();
  }

  /* optionally output some text data (description, copyright or whatever) */
//  if (strlen(id_string))
//    putuserdata(id_string);

  if (doVBRLimit)
  {
    prepareundo(&videobs,&undobs);
    undoi = 0;
    lastbitrate = bitcount(&videobs);

    if (max_bit_rate == 0)
      absmaxbitrate = MAXINT;
    else
      absmaxbitrate = (int)floor((max_bit_rate * N) / frame_rate);

    if (min_bit_rate == 0)
      absminbitrate = 0;
    else
      absminbitrate = (int)floor((min_bit_rate * N) / frame_rate);

    originalmquant = mquant_value;

    if ((avg_bit_rate > 0) && (!constant_bitrate))
    {
      sparedbitrate = 40000000;
      averagebitrate = (int)floor((avg_bit_rate * N) / frame_rate);
    }
    else
    {
      sparedbitrate = 0;
      averagebitrate = MAXINT;
    }
    save_rc_max();
    initial_mq = 0;
    max_done = 0;
    new_mquant = mquant_value;
  }
  gopCount = 1;
  frame_t = 0.0;
  hours = min = sec = 0;

  /* loop through all frames in encoding/decoding order */
  for (i=0; i<nframes; i++)
  {
    YieldTime();
    if (AbortMPEG)
    {
      FinishVideo();
      return FALSE;
    }

    if (FileOutError)
    {
      FinishVideo();
      //DisplayError("Unable to write to output file.");
      return FALSE;
    }

    percent = (unsigned int)floor(((double) (i)) / ((double) nframes) * 100.0);
    if ((i >= 10) && (i % 10 == 0))
    {
      time(&end_t);
      tot_t = (unsigned int) end_t - start_t;
      frame_t = (double) i / (double) tot_t; // frame_t = fps
      tot_t = (unsigned int) floor((double) nframes / frame_t + 0.5) - tot_t;
      sec = tot_t % 60;
      min = tot_t / 60;
      hours = min / 60;
      min = min % 60;
    }
    fSize = bitcount(&videobs) / 8388608;
  //  sprintf(tmpStr, "Video: %d%% - frame %d of %d,  %.1ffps,  %dhr %dmin left,  cur/est size: %.0f/%.0fMB",
   //                 percent, i, nframes, frame_t, hours, min, fSize, fSize *  ((double)nframes / (double)i));
  //  DisplayProgress(tmpStr, percent);


    /* f0: lowest frame number in current GOP
     *
     * first GOP contains N-(M-1) frames,
     * all other GOPs contain N frames
     */
    f0 = N*((i+(M-1))/N) - (M-1);

    if (f0<0)
      f0=0;

    if (doVBRLimit && (i==f0) && (i!=0)) // Check on correct bitrate if we are limiting the bitrate
    {
      availablebr = (int)floor((sparedbitrate / ((10*frame_rate)/N)) + averagebitrate); // 10 second buffer
      calcbr = (int)(bitcount(&videobs) - lastbitrate);

      if (verbose)
      {
        if (!initial_mq)
        {
         // sprintf(tmpStr, "  GOP #%5d, bitrate = %u, act mquant = %d", gopCount, (int)(calcbr * frame_rate / N), mquant_value);
          //DisplayInfo(tmpStr);
          initial_mq = 1;
        }
        else
        {
        //  sprintf(tmpStr, "              bitrate = %u, new mquant = %d", (int)(calcbr * frame_rate / N), mquant_value);
        //  DisplayInfo(tmpStr);
        }
      }
      if ((((calcbr > absmaxbitrate) || (calcbr > availablebr)) && (mquant_value < 31)) ||
           ((calcbr < absminbitrate) && !max_done && (mquant_value > 1)))
      {
        restore_rc_max();
        undochanges(&videobs,&undobs);
        i = undoi;
        f0 = N*((i+(M-1))/N) - (M-1);

        if (f0<0)
          f0=0;

        if (calcbr < absminbitrate)
        {
          minbr = availablebr;
          if (minbr > absminbitrate)
            minbr = absminbitrate;

          factor = (double)calcbr / (double)minbr;
          new_mquant = (int)floor((double)mquant_value * factor + 0.5) + 1;
          if (new_mquant >= mquant_value)
            mquant_value--;
          else
            mquant_value = new_mquant;
        }
        else
        {
          max_done = 1;
          maxbr = availablebr;
          if (maxbr > absmaxbitrate)
            maxbr = absmaxbitrate;

          factor = (double)calcbr / (double)maxbr;
          new_mquant = (int)floor((double)mquant_value * factor + 0.5); // + 1;
          if (new_mquant <= mquant_value)
            mquant_value++;
          else
            mquant_value = new_mquant;
        }

        if (mquant_value < 1)
          mquant_value = 1;
        if (mquant_value > 31)
          mquant_value = 31;

        memcpy(mbinfo, umbinfo, mb_width*mb_height2*sizeof(struct mbinfo));
        memcpy(blocks,ublocks,mb_width*mb_height2*block_count*sizeof(short [64]));
        for (j=0; j<MAXM; j++)
          motion_data[j] = umotion_data[j];
        for (j=0; j<3; j++)
        {
          size = (j==0) ? width*height : chrom_width*chrom_height;

          memcpy(oldorgframe[j],uoldorgframe[j],size);
          memcpy(oldrefframe[j],uoldrefframe[j],size);
          memcpy(auxframe[j],uauxframe[j],size);
          memcpy(neworgframe[j],uneworgframe[j],size);
          memcpy(newrefframe[j],unewrefframe[j],size);
          memcpy(auxorgframe[j],uauxorgframe[j],size);
          memcpy(predframe[j],upredframe[j],size);
          neworg[j] = uneworg[j];
          newref[j] = unewref[j];
          dc_dct_pred[j] = udc_dct_pred[j];
        }

      }
      else  // Everythings ok with last GOP
      {
        gopCount++;
        initial_mq = 0;
        max_done = 0;
        save_rc_max();
        if ((avg_bit_rate > 0) && (!constant_bitrate))
          sparedbitrate += (averagebitrate - calcbr);

        prepareundo(&videobs,&undobs);
        undoi = i;

        lastbitrate = bitcount(&videobs);
        mquant_value = originalmquant;

        memcpy(umbinfo, mbinfo, mb_width*mb_height2*sizeof(struct mbinfo));
        memcpy(ublocks,blocks,mb_width*mb_height2*block_count*sizeof(short [64]));
        for (j=0; j<MAXM; j++)
          umotion_data[j] = motion_data[j];
        for (j=0; j<3; j++)
        {
          size = (j==0) ? width*height : chrom_width*chrom_height;

          memcpy(uoldorgframe[j],oldorgframe[j],size);
          memcpy(uoldrefframe[j],oldrefframe[j],size);
          memcpy(uauxframe[j],auxframe[j],size);
          memcpy(uneworgframe[j],neworgframe[j],size);
          memcpy(unewrefframe[j],newrefframe[j],size);
          memcpy(uauxorgframe[j],auxorgframe[j],size);
          memcpy(upredframe[j],predframe[j],size);
          uneworg[j] = neworg[j];
          unewref[j] = newref[j];
          udc_dct_pred[j] = dc_dct_pred[j];
        }
      }
    }

    if (i==0 || (i-1)%M==0)
    {
      /* I or P frame */
      for (j=0; j<3; j++)
      {
        /* shuffle reference frames */
        neworg[j] = oldorgframe[j];
        newref[j] = oldrefframe[j];
        oldorgframe[j] = neworgframe[j];
        oldrefframe[j] = newrefframe[j];
        neworgframe[j] = neworg[j];
        newrefframe[j] = newref[j];
      }

      /* f: frame number in display order */
      f = (i==0) ? 0 : i+M-1;
      if (f>=nframes)
        f = nframes - 1;

      if (i==f0) /* first displayed frame in GOP is I */
      {
        /* I frame */
        pict_type = I_TYPE;
        forw_hor_f_code = forw_vert_f_code = 15;
        back_hor_f_code = back_vert_f_code = 15;

        /* n: number of frames in current GOP
         *
         * first GOP contains (M-1) less (B) frames
         */
        n = (i==0) ? N-(M-1) : N;

        /* last GOP may contain less frames */
        if (n > nframes-f0)
          n = nframes-f0;

        /* number of P frames */
        if (i==0)
          np = (n + 2*(M-1))/M - 1; /* first GOP */
        else
          np = (n + (M-1))/M - 1;

        /* number of B frames */
        nb = n - np - 1;

        if (constant_bitrate)
          rc_init_GOP(np,nb);
        if (i)
        {
          alignbits(&videobs); // moved from puthdr
          if (vFile)
          {
            byteCount = (unsigned int) floor(bitcount(&videobs) / 8.0) + 8;
            if (fwrite(&byteCount, 1, sizeof(byteCount), vFile) != sizeof(byteCount))
            {
              FinishVideo();
              //DisplayError("Unable to write to temporary max bitrate file.");
              return FALSE;
            }
          }
          putseqhdr();
          if (video_type > MPEG_VCD)
          {
            putseqext();
            putseqdispext();
          }
        }
        putgophdr(f0,i==0); /* set closed_GOP in first GOP only */
      }
      else
      {
        /* P frame */
        pict_type = P_TYPE;
        forw_hor_f_code = motion_data[0].forw_hor_f_code;
        forw_vert_f_code = motion_data[0].forw_vert_f_code;
        back_hor_f_code = back_vert_f_code = 15;
        sxf = motion_data[0].sxf;
        syf = motion_data[0].syf;
      }
    }
    else
    {
      /* B frame */
      for (j=0; j<3; j++)
      {
        neworg[j] = auxorgframe[j];
        newref[j] = auxframe[j];
      }

      /* f: frame number in display order */
      f = i - 1;
      pict_type = B_TYPE;
      n = (i-2)%M + 1; /* first B: n=1, second B: n=2, ... */
      forw_hor_f_code = motion_data[n].forw_hor_f_code;
      forw_vert_f_code = motion_data[n].forw_vert_f_code;
      back_hor_f_code = motion_data[n].back_hor_f_code;
      back_vert_f_code = motion_data[n].back_vert_f_code;
      sxf = motion_data[n].sxf;
      syf = motion_data[n].syf;
      sxb = motion_data[n].sxb;
      syb = motion_data[n].syb;
    }

    if (maxmotion > 7)
	{
      Sxf = Syf = Sxb = Syb = 0;
	  sxf = syf = sxb = syb =0 ;
	}

    temp_ref = f - f0;
    frame_pred_dct = frame_pred_dct_tab[pict_type-1];
    q_scale_type = qscale_tab[pict_type-1];
    intravlc = intravlc_tab[pict_type-1];
    altscan = altscan_tab[pict_type-1];
    if (OutputStats)
    {
    //  fprintf(statfile,"\nFrame %d (#%d in display order):\n",i,f);
    //  fprintf(statfile," picture_type=%c\n",ipb[pict_type]);
    //  fprintf(statfile," temporal_reference=%d\n",temp_ref);
    //  fprintf(statfile," frame_pred_frame_dct=%d\n",frame_pred_dct);
    //  fprintf(statfile," q_scale_type=%d\n",q_scale_type);
    //  fprintf(statfile," intra_vlc_format=%d\n",intravlc);
    //  fprintf(statfile," alternate_scan=%d\n",altscan);

      if (pict_type!=I_TYPE)
      {
    //    fprintf(statfile," forward search window: %d...%d / %d...%d\n",
    //      -sxf,sxf,-syf,syf);
     //   fprintf(statfile," forward vector range: %d...%d.5 / %d...%d.5\n",
      //    -(4<<forw_hor_f_code),(4<<forw_hor_f_code)-1,
      //    -(4<<forw_vert_f_code),(4<<forw_vert_f_code)-1);
      }

      if (pict_type==B_TYPE)
      {
    //    fprintf(statfile," backward search window: %d...%d / %d...%d\n",
      //    -sxb,sxb,-syb,syb);
     //   fprintf(statfile," backward vector range: %d...%d.5 / %d...%d.5\n",
     //     -(4<<back_hor_f_code),(4<<back_hor_f_code)-1,
      //    -(4<<back_vert_f_code),(4<<back_vert_f_code)-1);
      }
    }

    currentFrame = f + frame0;
    if (!readframe(neworg, currentFrame))
    {
      if (!i)
      {
        FinishVideo();
        return FALSE;
      }
      goto lastpic;
    }

    if (fieldpic)
    {
      pict_struct = topfirst ? TOP_FIELD : BOTTOM_FIELD;
      if (!motion_estimation(oldorgframe[0],neworgframe[0],
                        oldrefframe[0],newrefframe[0],
                        neworg[0],newref[0],
                        sxf,syf,sxb,syb,mbinfo,0,0))
      {
        FinishVideo();
        return FALSE;
      }

      if (maxmotion > 7)
        DoVarMotion();

      predict(oldrefframe,newrefframe,predframe,0,mbinfo);
      dct_type_estimation(predframe[0],neworg[0],mbinfo);
      transform(predframe,neworg,mbinfo,blocks);
      if (!putpict(neworg[0]))
      {
        FinishVideo();
        return FALSE;
      }

      for (k=0; k<mb_height2*mb_width; k++)
      {
        if (mbinfo[k].mb_type & MB_INTRA)
          for (j=0; j<block_count; j++)
            iquant_intra(blocks[k*block_count+j],blocks[k*block_count+j],
                         dc_prec,intra_q,mbinfo[k].mquant);
        else
          for (j=0;j<block_count;j++)
            iquant_non_intra(blocks[k*block_count+j],blocks[k*block_count+j],
                             inter_q,mbinfo[k].mquant);
      }

      itransform(predframe,newref,mbinfo,blocks);
      if (OutputStats)
      {
        calcSNR(neworg,newref);
        stats();
      }

      if (maxmotion > 7)
        Sxf = Syf = Sxb = Syb = 0;

      pict_struct = topfirst ? BOTTOM_FIELD : TOP_FIELD;

      ipflag = (pict_type==I_TYPE);
      if (ipflag)
      {
        /* first field = I, second field = P */
        pict_type = P_TYPE;
        forw_hor_f_code = motion_data[0].forw_hor_f_code;
        forw_vert_f_code = motion_data[0].forw_vert_f_code;
        back_hor_f_code = back_vert_f_code = 15;
        sxf = motion_data[0].sxf;
        syf = motion_data[0].syf;
      }

      if (!motion_estimation(oldorgframe[0],neworgframe[0],
                        oldrefframe[0],newrefframe[0],
                        neworg[0],newref[0],
                        sxf,syf,sxb,syb,mbinfo,1,ipflag))
      {
        FinishVideo();
        return FALSE;
      }

      if (maxmotion > 7)
        DoVarMotion();

      predict(oldrefframe,newrefframe,predframe,1,mbinfo);
      dct_type_estimation(predframe[0],neworg[0],mbinfo);
      transform(predframe,neworg,mbinfo,blocks);
      if (!putpict(neworg[0]))
      {
        FinishVideo();
        return FALSE;
      }

      for (k=0; k<mb_height2*mb_width; k++)
      {
        if (mbinfo[k].mb_type & MB_INTRA)
          for (j=0; j<block_count; j++)
            iquant_intra(blocks[k*block_count+j],blocks[k*block_count+j],
                         dc_prec,intra_q,mbinfo[k].mquant);
        else
          for (j=0;j<block_count;j++)
            iquant_non_intra(blocks[k*block_count+j],blocks[k*block_count+j],
                             inter_q,mbinfo[k].mquant);
      }

      itransform(predframe,newref,mbinfo,blocks);
      if (OutputStats)
      {
        calcSNR(neworg,newref);
        stats();
      }
    }
    else
    {
      pict_struct = FRAME_PICTURE;
      if ((video_pulldown_flag != PULLDOWN_NONE) &&
          (video_pulldown_flag != PULLDOWN_AUTO))
      {
		tmp_prog_frame = 1;
        if (video_pulldown_flag == PULLDOWN_32)
        {
          switch (currentFrame % 4)
          {
            case 0:
              tmp_topfirst = 1;
//              tmp_prog_frame = 1;
              tmp_repeatfirst = 1;
              break;
            case 1:
              tmp_topfirst = 0;
//              tmp_prog_frame = 0;
              tmp_repeatfirst = 0;
              break;
            case 2:
              tmp_topfirst = 0;
//              tmp_prog_frame = 1;
              tmp_repeatfirst = 1;
              break;
            case 3:
              tmp_topfirst = 1;
//              tmp_prog_frame = 0;
              tmp_repeatfirst = 0;
              break;
          }
        }
        else
          switch (currentFrame % 4)
          {
            case 0:
                tmp_topfirst = 1;
//                tmp_prog_frame = 0;
                tmp_repeatfirst = 0;
                break;
            case 1:
                tmp_topfirst = 1;
//                tmp_prog_frame = 1;
                tmp_repeatfirst = 1;
                break;
            case 2:
                tmp_topfirst = 0;
//                tmp_prog_frame = 0;
                tmp_repeatfirst = 0;
                break;
            case 3:
                tmp_topfirst = 0;
//                tmp_prog_frame = 1;
                tmp_repeatfirst = 1;
                break;
          }
      }
	  else
	  {
	    tmp_topfirst = topfirst;
		tmp_repeatfirst = repeatfirst;
		tmp_prog_frame = prog_frame;
	  }


      /* do motion_estimation
       *
       * uses source frames (...orgframe) for full pel search
       * and reconstructed frames (...refframe) for half pel search
       */
      if (!motion_estimation(oldorgframe[0],neworgframe[0],
                        oldrefframe[0],newrefframe[0],
                        neworg[0],newref[0],
                        sxf,syf,sxb,syb,mbinfo,0,0))
      {
        FinishVideo();
        return FALSE;
      }
      if (maxmotion > 7)
        DoVarMotion();

      predict(oldrefframe,newrefframe,predframe,0,mbinfo);
      dct_type_estimation(predframe[0],neworg[0],mbinfo);
      transform(predframe,neworg,mbinfo,blocks);
      if (!putpict(neworg[0]))
      {
        FinishVideo();
        return FALSE;
      }

      for (k=0; k<mb_height*mb_width; k++)
      {
        if (mbinfo[k].mb_type & MB_INTRA)
          for (j=0; j<block_count; j++)
            iquant_intra(blocks[k*block_count+j],blocks[k*block_count+j],
                         dc_prec,intra_q,mbinfo[k].mquant);
        else
          for (j=0;j<block_count;j++)
            iquant_non_intra(blocks[k*block_count+j],blocks[k*block_count+j],
                             inter_q,mbinfo[k].mquant);
      }

      itransform(predframe,newref,mbinfo,blocks);
      if (OutputStats)
      {
        calcSNR(neworg,newref);
        stats();
      }
    }
  }
lastpic:
  putseqend();
  FinishVideo();

  if (constant_bitrate)
  {
    if (!verbose && vbvOverflows)
    {
   //   sprintf(tmpStr, "   VBV delay overflows = %d", vbvOverflows);
    //  DisplayInfo(tmpStr);
    }
    if (!verbose && vbvUnderflows)
    {
    //  sprintf(tmpStr, "   VBV delay underflows = %d", vbvUnderflows);
    //  DisplayInfo(tmpStr);
    }



  }
  /*
  sprintf(tmpStr, "   Min bitrate of any one frame = %u bits", min_frame_bitrate);
  DisplayInfo(tmpStr);
  sprintf(tmpStr, "   Max bitrate of any one frame = %u bits", max_frame_bitrate);
  DisplayInfo(tmpStr);
  sprintf(tmpStr, "   Min bitrate over any one second = %u bps", min_bitrate);
  DisplayInfo(tmpStr);
  sprintf(tmpStr, "   Avg bitrate over any one second = %.0f bps", bitcount(&videobs) / (nframes / frame_rate));
  DisplayInfo(tmpStr);
  sprintf(tmpStr, "   Max bitrate over any one second = %u bps", max_bitrate);
  DisplayInfo(tmpStr);
  */

  if (constant_bitrate)
  {
    if (maxPadding)
    {
      //  sprintf(tmpStr, "   Avg padding bits over any one second = %.0f", frame_rate * paddingSum/(double) nframes);
      //  DisplayInfo(tmpStr);
      //  sprintf(tmpStr, "   Max padding in any one frame = %u bits", maxPadding);
      //  DisplayInfo(tmpStr);
    }

   // sprintf(tmpStr, "   Avg header bits over any one second = %.0f", frame_rate * headerSum/(double) nframes);
   // DisplayInfo(tmpStr);
 
	/*
    sprintf(tmpStr, "   Min mquant = %u", min_mquant);
    DisplayInfo(tmpStr);
    sprintf(tmpStr, "   Avg mquant = %.3f", avg_mquant);
    DisplayInfo(tmpStr);
    sprintf(tmpStr, "   Max mquant = %u", max_mquant);
    DisplayInfo(tmpStr);
	*/

  }
 // sprintf(tmpStr, "Video: 100%% - frame %u of %u.", nframes, nframes);
  //DisplayProgress(tmpStr, 100);
  time(&end_t);
  tot_t = (unsigned int) end_t - start_t;
  frame_t = (double) tot_t / (double) nframes;
  sec = tot_t % 60;
  min = tot_t / 60;
  hours = min / 60;
  min = min % 60;
//  sprintf(tmpStr, "   Total time: %d seconds (%02d:%02d:%02d), %.2f frames/sec, %.3f sec/frame.", tot_t, hours, min, sec, 1.0 / frame_t, frame_t);
//  DisplayInfo(tmpStr);
  if ((video_type > MPEG_VCD) && !constant_bitrate && !max_bit_rate)
  {
    i = putMaxBitrate();
    unlink(max_name);
    return !i;
  }
  return TRUE;
}

int putMaxBitrate()
{
  unsigned int byteCount, totalPatches, patchCount, percent;
  DWORD i, j;
  long hiLong;
  unsigned char patchVal[3];
  HANDLE vStream;
  struct stat statbuf;
  char tmpStr[132];

  vStream = CreateFile(VideoFilename, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ, NULL,
                       OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
  if (vStream == INVALID_HANDLE_VALUE)
  {
    //DisplayError("Unable to open video stream.");
    return 1;
  }

  vFile = fopen(max_name, "rb");
  if (vFile == NULL)
  {
    //DisplayError("Unable to open temporary max bitrate file.");
    CloseHandle(vStream);
    return 1;
  }

  if (stat(max_name, &statbuf))
  {
    //DisplayError("Unable to get size of temporary max bitrate file.");
    CloseHandle(vStream);
    fclose(vFile);
    return 1;
  }

  totalPatches = statbuf.st_size / 4;
  patchCount = 0;
  DisplayInfo(" ");
  DisplayInfo("Embedding max bitrate value in sequence headers ...");

  /* note, this only works when the max_bit_rate is < 104857600 bps */
  byteCount = (unsigned int)ceil(max_bitrate / 400.0);
  patchVal[0] = (unsigned char)((byteCount & 0x3FC00) >> 10);
  patchVal[1] = (unsigned char)((byteCount & 0x03FC) >> 2);
  patchVal[2] = (unsigned char)(((byteCount & 0x3) << 6) | 0x20 | ((vbv_buffer_size & 0x03E0) >> 5));

  while (patchCount < totalPatches)
  {
    percent = (int)floor(((double) (patchCount + 1)) / ((double) totalPatches) * 100.0);
  //  sprintf(tmpStr, "Embedding max bitrate values: %d%% - %d of %d.", percent, patchCount + 1, totalPatches);
    //DisplayProgress(tmpStr, percent);
    if (fread(&byteCount, 1, sizeof(byteCount), vFile) != sizeof(byteCount))
    {
      //DisplayError("Unable to read from temporary max bitrate file.");
      fclose(vFile);
      CloseHandle(vStream);
      return 1;
    }
    hiLong = 0;
    i = SetFilePointer(vStream, byteCount, &hiLong, FILE_BEGIN);
    if ((i == 0xFFFFFFFF) && (GetLastError() != NO_ERROR))
    {
   //   sprintf(tmpStr, "Unable to seek in video stream, offset = %u.", byteCount);
      //DisplayError(tmpStr);
      fclose(vFile);
      CloseHandle(vStream);
      return 1;
    }

    i = WriteFile(vStream, &patchVal, 3L, &j, NULL);
    if (!i || (j != 3))
    {
      //DisplayError("Unable to write to video stream");
      fclose(vFile);
      CloseHandle(vStream);
      return 1;
    }
    patchCount++;
  }
  fclose(vFile);
  CloseHandle(vStream);
  return 0;
}
