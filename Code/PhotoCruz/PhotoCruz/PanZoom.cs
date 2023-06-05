using System;
using System.Collections.Generic;
using System.Text;
using DVDSlideshow;
using System.Drawing.Drawing2D;
using System.Drawing;
using DVDSlideshow.GraphicsEng;


namespace PhotoCruz
{
    //**************************************************************************
    public enum PanZoomType
    {
        RANDOM,
        NONE,
        // landscape
        PL2R, //= pan left to right
        PL2RZI, //= pan left to right while zooming in
        PL2RZO, //= pan left to right while zooming out*
        PR2L, //= pan right to left
        PR2LZI, //= pan right to left while zooming in 
        PR2LZO, //= pan right to left while zooming out*
        //Portrait pictures;
        PT2B, //= pan top to bottom
        PT2BZI, //= pan top to bottom while zooming in
        PT2BZO, //= pan top to bottom while zooming out*
        PB2T, //= pan bottom to top
        PB2TZI,// = pan bottom to top while zooming in
        PB2TZO, //= pan bottom to top while zooming out*
        //(*zoom outs will start zoomed in a bit, then slowly zooming out during the transition)
        PZI, // = pan zoom in
        PZO  // = pan zoom out
    } 

    public class PanZoom
    {

      //**************************************************************************
      static public PanZoomType ParsePanZoomType(string name, PanZoomType default_type)
      {
          string upper = name.ToUpper();
          if (upper.Contains("NULL")) return default_type;
          if (upper.Contains("RANDOM")) return PanZoomType.RANDOM;
          if (upper.Contains("NONE")) return PanZoomType.NONE;

          if (upper.Contains("PL2RZI")) return PanZoomType.PL2RZI;
          if (upper.Contains("PL2RZO")) return PanZoomType.PL2RZO;
          if (upper.Contains("PL2R")) return PanZoomType.PL2R;

          if (upper.Contains("PR2LZI")) return PanZoomType.PR2LZI;
          if (upper.Contains("PR2LZO")) return PanZoomType.PR2LZO;
          if (upper.Contains("PR2L")) return PanZoomType.PR2L;

          if (upper.Contains("PT2BZI")) return PanZoomType.PT2BZI;
          if (upper.Contains("PT2BZO")) return PanZoomType.PT2BZO;
          if (upper.Contains("PT2B")) return PanZoomType.PT2B;

          if (upper.Contains("PB2TZI")) return PanZoomType.PB2TZI;
          if (upper.Contains("PB2TZO")) return PanZoomType.PB2TZO;
          if (upper.Contains("PB2T")) return PanZoomType.PB2T;
          if (upper.Contains("PZI")) return PanZoomType.PZI;
          if (upper.Contains("PZO")) return PanZoomType.PZO;
       

          return default_type;
      }

      // ******************************************************************
      public  static DVDSlideshow.CPanZoom GeneratePhotoVidShowPanZoom(PanZoomType type, CImage image)
      {
          CPanZoom pz = new CPanZoom();

          switch (type)
          {
              case PanZoomType.RANDOM:
              {
                  if (image!=null) pz.CalcBestRectangleAreas(image);
                  break;
              }
                case PanZoomType.NONE:
              {
                  pz.StartArea = new RectangleF(0.0f, 0.0f, 1.0f, 1.0f);
                  pz.EndArea = new RectangleF(0.0f, 0.0f, 1.0f, 1.0f);
                  break;
              }

              //= pan left to right
              case PanZoomType.PL2R:
              {
                  pz.StartArea = new RectangleF(-0.06f,0.05f, 0.9f,0.9f);
                  pz.EndArea = new RectangleF(0.15f, 0.05f, 0.9f, 0.9f);

                //  pz.EndArea = new RectangleF(0.2f,0.0f,0.8f,1.0f);
                  break;
              }

              //= pan left to right while zooming in
              case PanZoomType. PL2RZI:
              {
                  pz.StartArea = new RectangleF(0.0f,0.0f,1.0f,1.0f);
                  pz.EndArea = new RectangleF(0.4f,0.2f,0.6f,0.6f);
                  break;
              }
              //= pan left to right while zooming out*
              case PanZoomType. PL2RZO:
              {
                  pz.StartArea = new RectangleF(0.0f,0.2f,0.6f,0.6f);
                  pz.EndArea = new RectangleF(0.0f,0.0f,1.0f,1.0f);
                  break;
              }

              //= pan right to left
              case PanZoomType.  PR2L:
              {
                  pz.StartArea = new RectangleF(0.15f, 0.05f, 0.9f, 0.9f);
                  pz.EndArea = new RectangleF(-0.06f, 0.05f, 0.9f, 0.9f);
                  break;
              }

              //= pan right to left while zooming in 
              case PanZoomType. PR2LZI:
              {
                  pz.StartArea = new RectangleF(0.0f, 0.0f, 1.0f, 1.0f);
                  pz.EndArea = new RectangleF(0.0f, 0.2f, 0.6f, 0.6f);
                  break;
              }

              //= pan right to left while zooming out*
              case PanZoomType. PR2LZO:
              {
                  pz.StartArea = new RectangleF(0.4f, 0.2f, 0.6f, 0.6f);
                  pz.EndArea = new RectangleF(0.0f, 0.0f, 1.0f, 1.0f);
                  break;
              }

              //= pan top to bottom
              case PanZoomType. PT2B:
              {
                  pz.StartArea = new RectangleF(0.1f,0.0f,0.8f,0.8f);
                  pz.EndArea = new RectangleF(0.1f,0.2f,0.8f,0.8f);
                  break;
              }

              //= pan top to bottom while zooming in
              case PanZoomType. PT2BZI:
              {
                  pz.StartArea = new RectangleF(0.0f, 0.0f, 1.0f, 1.0f);
                  pz.EndArea = new RectangleF(0.2f, 0.4f, 0.6f, 0.6f);
                  break;
              }

              //= pan top to bottom while zooming out*
              case PanZoomType. PT2BZO:
              {
                  pz.StartArea = new RectangleF(0.2f, 0.0f, 0.6f, 0.6f);
                  pz.EndArea = new RectangleF(0.0f, 0.0f, 1.0f, 1.0f);
                  break;
              }

              //= pan bottom to top
              case PanZoomType. PB2T:
              {
                  pz.StartArea = new RectangleF(0.1f, 0.2f, 0.8f, 0.8f);
                  pz.EndArea = new RectangleF(0.1f, 0.0f, 0.8f, 0.8f);
                  break;
              }

              // = pan bottom to top while zooming in
              case PanZoomType. PB2TZI:
              {
                  pz.StartArea = new RectangleF(0.0f, 0.0f, 1.0f, 1.0f);
                  pz.EndArea = new RectangleF(0.2f, 0.0f, 0.6f, 0.6f);
                  break;
              }

              //= pan bottom to top while zooming out*
              case PanZoomType.PB2TZO:
              {
                  pz.StartArea = new RectangleF(0.2f, 0.4f, 0.6f, 0.6f);
                  pz.EndArea = new RectangleF(0.0f, 0.0f, 1.0f, 1.0f);
                  break;
              }

              case PanZoomType.PZI:
              {
                  pz.StartArea = new RectangleF(0.0f, 0.0f, 1.0f, 1.0f);
                  pz.EndArea = new RectangleF(0.2f, 0.2f, 0.6f, 0.6f);
                  break;
              }

              case PanZoomType.PZO:
              {
                  pz.StartArea = new RectangleF(0.2f, 0.2f, 0.6f, 0.6f);
                  pz.EndArea = new RectangleF(0.0f, 0.0f, 1.0f, 1.0f);
                  break;
              }

          }

          return pz;
      }
       
    }
}
