/**********************************************************************
Copyright (c) 1991 MPEG/audio software simulation group, All Rights Reserved
tonal.c
**********************************************************************/
/**********************************************************************
 * MPEG/audio coding/decoding software, work in progress              *
 *   NOT for public distribution until verified and approved by the   *
 *   MPEG/audio committee.  For further information, please contact   *
 *   Davis Pan, 508-493-2241, e-mail: pan@3d.enet.dec.com             *
 *                                                                    *
 * VERSION 3.9t                                                       *
 *   changes made since last update:                                  *
 *   date   programmers         comment                               *
 * 2/25/91  Douglas Wong        start of version 1.1 records          *
 * 3/06/91  Douglas Wong        rename: setup.h to endef.h            *
 *                              updated I_psycho_one and II_psycho_one*
 * 3/11/91  W. J. Carter        Added Douglas Wong's updates dated    *
 *                              3/9/91 for I_Psycho_One() and for     *
 *                              II_Psycho_One().                      *
 * 5/10/91  W. Joseph Carter    Ported to Macintosh and Unix.         *
 *                              Located and fixed numerous software   *
 *                              bugs and table data errors.           *
 * 6/11/91  Davis Pan           corrected several bugs                *
 *                              based on comments from H. Fuchs       *
 * 01jul91  dpwe (Aware Inc.)   Made pow() args float                 *
 *                              Removed logical bug in I_tonal_label: *
 *                              Sometimes *tone returned == STOP      *
 * 7/10/91  Earle Jennings      no change necessary in port to MsDos  *
 * 11sep91  dpwe@aware.com      Subtracted 90.3dB from II_f_f_t peaks *
 * 10/1/91  Peter W. Farrett    Updated II_Psycho_One(),I_Psycho_One()*
 *                              to include comments.                  *
 *11/29/91  Masahiro Iwadare    Bug fix regarding POWERNORM           *
 *                              fixed several other miscellaneous bugs*
 * 2/11/92  W. Joseph Carter    Ported new code to Macintosh.  Most   *
 *                              important fixes involved changing     *
 *                              16-bit ints to long or unsigned in    *
 *                              bit alloc routines for quant of 65535 *
 *                              and passing proper function args.     *
 *                              Removed "Other Joint Stereo" option   *
 *                              and made bitrate be total channel     *
 *                              bitrate, irrespective of the mode.    *
 *                              Fixed many small bugs & reorganized.  *
 * 2/12/92  Masahiro Iwadare    Fixed some potential bugs in          *
 *          Davis Pan           subsampling()                         *
 * 2/25/92  Masahiro Iwadare    Fixed some more potential bugs        *
 * 6/24/92  Tan Ah Peng         Modified window for FFT               * 
 *                              (denominator N-1 to N)                *
 *                              Updated all critical band rate &      *
 *                              absolute threshold tables and critical*
 *                              boundaries for use with Layer I & II  *  
 *                              Corrected boundary limits for tonal   *
 *                              component computation                 *
 *                              Placement of non-tonal component at   *
 *                              geometric mean of critical band       *
 *                              (previous placement method commented  *
 *                               out - can be used if desired)        *
 * 3/01/93  Mike Li             Infinite looping fix in noise_label() *
 * 3/19/93  Jens Spille         fixed integer overflow problem in     *
 *                              psychoacoutic model 1                 *
 * 3/19/93  Giorgio Dimino      modifications to better account for   *
 *                              tonal and non-tonal components        *
 * 5/28/93 Sriram Jayasimha     "London" mod. to psychoacoustic model1*
 * 8/05/93 Masahiro Iwadare     noise_label modification "option"     *
 **********************************************************************/
#include "main.h"
#include "consts1.h"
#define LONDON                  /* enable "LONDON" modification */
#define MAKE_SENSE              /* enable "MAKE_SENSE" modification */
#define MI_OPTION               /* enable "MI_OPTION" modification */
static int onecb0[25] = {1, 2, 3, 5, 6, 8, 9, 11, 13, 15, 17, 20, 23, 27,
                         32, 37, 45, 52, 62, 74, 88, 108, 132, 180, 232};
static int onecb1[26] = {1, 2, 3, 4, 5, 6, 7, 9, 10, 12, 14, 16, 19, 21,
                         25, 29, 35, 41, 50, 58, 68, 82, 100, 124, 164, 216};
static int onecb2[24] = {1, 3, 5, 7, 9, 11, 13, 15, 18, 21, 24, 27, 32, 37,
                         44, 52, 62, 74, 88, 104, 124, 148, 184, 240};
static int twocb0[27] = {1, 2, 3, 5, 7, 10, 13, 16, 19, 22, 26, 30, 35,
                         40, 46, 54, 64, 76, 90, 104, 124, 148, 176, 216,
                         264, 360, 464};
static int twocb1[27] = {1, 2, 3, 5, 7, 9, 12, 14, 17, 20, 24, 27, 32, 37,
                         42, 50, 58, 70, 82, 100, 116, 136, 164, 200, 248,
                         328, 432};
static int twocb2[25] = {1, 3, 6, 10, 13, 17, 21, 25, 30, 35, 41, 47, 54,
                         64, 74, 88, 104, 124, 148, 176, 208, 248, 296,
                         368, 480};
static g_thres oneth0[108] = { {0, 0, 0, 0},
  {1, 0.850, 25.87, 0.0}, {2, 1.694, 14.85, 0.0}, {3, 2.525, 10.72, 0.0},
  {4, 3.337, 8.50, 0.0}, {5, 4.124, 7.10, 0.0}, {6, 4.882, 6.11, 0.0},
  {7, 5.608, 5.37, 0.0}, {8, 6.301, 4.79, 0.0}, {9, 6.959, 4.32, 0.0},
  {10, 7.581, 3.92, 0.0}, {11, 8.169, 3.57, 0.0}, {12, 8.723, 3.25, 0.0},
  {13, 9.244, 2.95, 0.0}, {14, 9.734, 2.67, 0.0}, {15, 10.195, 2.39, 0.0},
  {16, 10.629, 2.11, 0.0}, {17, 11.037, 1.83, 0.0}, {18, 11.421, 1.53, 0.0},
  {19, 11.783, 1.23, 0.0}, {20, 12.125, 0.90, 0.0}, {21, 12.448, 0.56, 0.0},
  {22, 12.753, 0.21, 0.0}, {23, 13.042, -0.17, 0.0}, {24, 13.317, -0.56, 0.0},
  {25, 13.578, -0.96, 0.0}, {26, 13.826, -1.38, 0.0}, {27, 14.062, -1.79, 0.0},
  {28, 14.288, -2.21, 0.0}, {29, 14.504, -2.63, 0.0}, {30, 14.711, -3.03, 0.0},
  {31, 14.909, -3.41, 0.0}, {32, 15.100, -3.77, 0.0}, {33, 15.284, -4.09, 0.0},
  {34, 15.460, -4.37, 0.0}, {35, 15.631, -4.60, 0.0}, {36, 15.796, -4.78, 0.0},
  {37, 15.955, -4.91, 0.0}, {38, 16.110, -4.97, 0.0}, {39, 16.260, -4.98, 0.0},
  {40, 16.406, -4.92, 0.0}, {41, 16.547, -4.81, 0.0}, {42, 16.685, -4.65, 0.0},
  {43, 16.820, -4.43, 0.0}, {44, 16.951, -4.17, 0.0}, {45, 17.079, -3.87, 0.0},
  {46, 17.205, -3.54, 0.0}, {47, 17.327, -3.19, 0.0}, {48, 17.447, -2.82, 0.0},
  {50, 17.680, -2.06, 0.0}, {52, 17.905, -1.32, 0.0}, {54, 18.121, -0.64, 0.0},
  {56, 18.331, -0.04, 0.0}, {58, 18.534, 0.47, 0.0}, {60, 18.731, 0.89, 0.0},
  {62, 18.922, 1.23, 0.0}, {64, 19.108, 1.51, 0.0}, {66, 19.289, 1.74, 0.0},
  {68, 19.464, 1.93, 0.0}, {70, 19.635, 2.11, 0.0}, {72, 19.801, 2.28, 0.0},
  {74, 19.963, 2.46, 0.0}, {76, 20.120, 2.63, 0.0}, {78, 20.273, 2.82, 0.0},
  {80, 20.421, 3.03, 0.0}, {82, 20.565, 3.25, 0.0}, {84, 20.705, 3.49, 0.0},
  {86, 20.840, 3.74, 0.0}, {88, 20.972, 4.02, 0.0}, {90, 21.099, 4.32, 0.0},
  {92, 21.222, 4.64, 0.0}, {94, 21.342, 4.98, 0.0}, {96, 21.457, 5.35, 0.0},
  {100, 21.677, 6.15, 0.0}, {104, 21.882, 7.07, 0.0}, {108, 22.074, 8.10, 0.0},
  {112, 22.253, 9.25, 0.0}, {116, 22.420, 10.54, 0.0}, {120, 22.576, 11.97, 0.0},
  {124, 22.721, 13.56, 0.0}, {128, 22.857, 15.31, 0.0}, {132, 22.984, 17.23, 0.0},
  {136, 23.102, 19.34, 0.0}, {140, 23.213, 21.64, 0.0}, {144, 23.317, 24.15, 0.0},
  {148, 23.415, 26.88, 0.0}, {152, 23.506, 29.84, 0.0}, {156, 23.592, 33.05, 0.0},
  {160, 23.673, 36.52, 0.0}, {164, 23.749, 40.25, 0.0}, {168, 23.821, 44.27, 0.0},
  {172, 23.888, 48.59, 0.0}, {176, 23.952, 53.22, 0.0}, {180, 24.013, 58.18, 0.0},
  {184, 24.070, 63.49, 0.0}, {188, 24.125, 68.00, 0.0}, {192, 24.176, 68.00, 0.0},
  {196, 24.225, 68.00, 0.0}, {200, 24.271, 68.00, 0.0}, {204, 24.316, 68.00, 0.0},
  {208, 24.358, 68.00, 0.0}, {212, 24.398, 68.00, 0.0}, {216, 24.436, 68.00, 0.0},
  {220, 24.473, 68.00, 0.0}, {224, 24.508, 68.00, 0.0}, {228, 24.542, 68.00, 0.0},
  {232, 24.574, 68.00, 0.0}};
static g_thres oneth1[104] = { {0, 0, 0, 0},
  {1, 0.925, 24.17, 0.0}, {2, 1.842, 13.87, 0.0}, {3, 2.742, 10.01, 0.0},
  {4, 3.618, 7.94, 0.0}, {5, 4.463, 6.62, 0.0}, {6, 5.272, 5.70, 0.0},
  {7, 6.041, 5.00, 0.0}, {8, 6.770, 4.45, 0.0}, {9, 7.457, 4.00, 0.0},
  {10, 8.103, 3.61, 0.0}, {11, 8.708, 3.26, 0.0}, {12, 9.275, 2.93, 0.0},
  {13, 9.805, 2.63, 0.0}, {14, 10.301, 2.32, 0.0}, {15, 10.765, 2.02, 0.0},
  {16, 11.199, 1.71, 0.0}, {17, 11.606, 1.38, 0.0}, {18, 11.988, 1.04, 0.0},
  {19, 12.347, 0.67, 0.0}, {20, 12.684, 0.29, 0.0}, {21, 13.002, -0.11, 0.0},
  {22, 13.302, -0.54, 0.0}, {23, 13.586, -0.97, 0.0}, {24, 13.855, -1.43, 0.0},
  {25, 14.111, -1.88, 0.0}, {26, 14.354, -2.34, 0.0}, {27, 14.585, -2.79, 0.0},
  {28, 14.807, -3.22, 0.0}, {29, 15.018, -3.62, 0.0}, {30, 15.221, -3.98, 0.0},
  {31, 15.415, -4.30, 0.0}, {32, 15.602, -4.57, 0.0}, {33, 15.783, -4.77, 0.0},
  {34, 15.956, -4.91, 0.0}, {35, 16.124, -4.98, 0.0}, {36, 16.287, -4.97, 0.0},
  {37, 16.445, -4.90, 0.0}, {38, 16.598, -4.76, 0.0}, {39, 16.746, -4.55, 0.0},
  {40, 16.891, -4.29, 0.0}, {41, 17.032, -3.99, 0.0}, {42, 17.169, -3.64, 0.0},
  {43, 17.303, -3.26, 0.0}, {44, 17.434, -2.86, 0.0}, {45, 17.563, -2.45, 0.0},
  {46, 17.688, -2.04, 0.0}, {47, 17.811, -1.63, 0.0}, {48, 17.932, -1.24, 0.0},
  {50, 18.166, -0.51, 0.0}, {52, 18.392, 0.12, 0.0}, {54, 18.611, 0.64, 0.0},
  {56, 18.823, 1.06, 0.0}, {58, 19.028, 1.39, 0.0}, {60, 19.226, 1.66, 0.0},
  {62, 19.419, 1.88, 0.0}, {64, 19.606, 2.08, 0.0}, {66, 19.788, 2.27, 0.0},
  {68, 19.964, 2.46, 0.0}, {70, 20.135, 2.65, 0.0}, {72, 20.300, 2.86, 0.0},
  {74, 20.461, 3.09, 0.0}, {76, 20.616, 3.33, 0.0}, {78, 20.766, 3.60, 0.0},
  {80, 20.912, 3.89, 0.0}, {82, 21.052, 4.20, 0.0}, {84, 21.188, 4.54, 0.0},
  {86, 21.318, 4.91, 0.0}, {88, 21.445, 5.31, 0.0}, {90, 21.567, 5.73, 0.0},
  {92, 21.684, 6.18, 0.0}, {94, 21.797, 6.67, 0.0}, {96, 21.906, 7.19, 0.0},
  {100, 22.113, 8.33, 0.0}, {104, 22.304, 9.63, 0.0}, {108, 22.482, 11.08, 0.0},
  {112, 22.646, 12.71, 0.0}, {116, 22.799, 14.53, 0.0}, {120, 22.941, 16.54, 0.0},
  {124, 23.072, 18.77, 0.0}, {128, 23.195, 21.23, 0.0}, {132, 23.309, 23.94, 0.0},
  {136, 23.415, 26.90, 0.0}, {140, 23.515, 30.14, 0.0}, {144, 23.607, 33.67, 0.0},
  {148, 23.694, 37.51, 0.0}, {152, 23.775, 41.67, 0.0}, {156, 23.852, 46.17, 0.0},
  {160, 23.923, 51.04, 0.0}, {164, 23.991, 56.29, 0.0}, {168, 24.054, 61.94, 0.0},
  {172, 24.114, 68.00, 0.0}, {176, 24.171, 68.00, 0.0}, {180, 24.224, 68.00, 0.0},
  {184, 24.275, 68.00, 0.0}, {188, 24.322, 68.00, 0.0}, {192, 24.368, 68.00, 0.0},
  {196, 24.411, 68.00, 0.0}, {200, 24.452, 68.00, 0.0}, {204, 24.491, 68.00, 0.0},
  {208, 24.528, 68.00, 0.0}, {212, 24.564, 68.00, 0.0}, {216, 24.597, 68.00, 0.0}};
static g_thres oneth2[104] = { {0, 0, 0, 0},
  {1, 0.925, 24.17, 0.0}, {2, 1.842, 13.87, 0.0}, {3, 2.742, 10.01, 0.0},
  {4, 3.618, 7.94, 0.0}, {5, 4.463, 6.62, 0.0}, {6, 5.272, 5.70, 0.0},
  {7, 6.041, 5.00, 0.0}, {8, 6.770, 4.45, 0.0}, {9, 7.457, 4.00, 0.0},
  {10, 8.103, 3.61, 0.0}, {11, 8.708, 3.26, 0.0}, {12, 9.275, 2.93, 0.0},
  {13, 9.805, 2.63, 0.0}, {14, 10.301, 2.32, 0.0}, {15, 10.765, 2.02, 0.0},
  {16, 11.199, 1.71, 0.0}, {17, 11.606, 1.38, 0.0}, {18, 11.988, 1.04, 0.0},
  {19, 12.347, 0.67, 0.0}, {20, 12.684, 0.29, 0.0}, {21, 13.002, -0.11, 0.0},
  {22, 13.302, -0.54, 0.0}, {23, 13.586, -0.97, 0.0}, {24, 13.855, -1.43, 0.0},
  {25, 14.111, -1.88, 0.0}, {26, 14.354, -2.34, 0.0}, {27, 14.585, -2.79, 0.0},
  {28, 14.807, -3.22, 0.0}, {29, 15.018, -3.62, 0.0}, {30, 15.221, -3.98, 0.0},
  {31, 15.415, -4.30, 0.0}, {32, 15.602, -4.57, 0.0}, {33, 15.783, -4.77, 0.0},
  {34, 15.956, -4.91, 0.0}, {35, 16.124, -4.98, 0.0}, {36, 16.287, -4.97, 0.0},
  {37, 16.445, -4.90, 0.0}, {38, 16.598, -4.76, 0.0}, {39, 16.746, -4.55, 0.0},
  {40, 16.891, -4.29, 0.0}, {41, 17.032, -3.99, 0.0}, {42, 17.169, -3.64, 0.0},
  {43, 17.303, -3.26, 0.0}, {44, 17.434, -2.86, 0.0}, {45, 17.563, -2.45, 0.0},
  {46, 17.688, -2.04, 0.0}, {47, 17.811, -1.63, 0.0}, {48, 17.932, -1.24, 0.0},
  {50, 18.166, -0.51, 0.0}, {52, 18.392, 0.12, 0.0}, {54, 18.611, 0.64, 0.0},
  {56, 18.823, 1.06, 0.0}, {58, 19.028, 1.39, 0.0}, {60, 19.226, 1.66, 0.0},
  {62, 19.419, 1.88, 0.0}, {64, 19.606, 2.08, 0.0}, {66, 19.788, 2.27, 0.0},
  {68, 19.964, 2.46, 0.0}, {70, 20.135, 2.65, 0.0}, {72, 20.300, 2.86, 0.0},
  {74, 20.461, 3.09, 0.0}, {76, 20.616, 3.33, 0.0}, {78, 20.766, 3.60, 0.0},
  {80, 20.912, 3.89, 0.0}, {82, 21.052, 4.20, 0.0}, {84, 21.188, 4.54, 0.0},
  {86, 21.318, 4.91, 0.0}, {88, 21.445, 5.31, 0.0}, {90, 21.567, 5.73, 0.0},
  {92, 21.684, 6.18, 0.0}, {94, 21.797, 6.67, 0.0}, {96, 21.906, 7.19, 0.0},
  {100, 22.113, 8.33, 0.0}, {104, 22.304, 9.63, 0.0}, {108, 22.482, 11.08, 0.0},
  {112, 22.646, 12.71, 0.0}, {116, 22.799, 14.53, 0.0}, {120, 22.941, 16.54, 0.0},
  {124, 23.072, 18.77, 0.0}, {128, 23.195, 21.23, 0.0}, {132, 23.309, 23.94, 0.0},
  {136, 23.415, 26.90, 0.0}, {140, 23.515, 30.14, 0.0}, {144, 23.607, 33.67, 0.0},
  {148, 23.694, 37.51, 0.0}, {152, 23.775, 41.67, 0.0}, {156, 23.852, 46.17, 0.0},
  {160, 23.923, 51.04, 0.0}, {164, 23.991, 56.29, 0.0}, {168, 24.054, 61.94, 0.0},
  {172, 24.114, 68.00, 0.0}, {176, 24.171, 68.00, 0.0}, {180, 24.224, 68.00, 0.0},
  {184, 24.275, 68.00, 0.0}, {188, 24.322, 68.00, 0.0}, {192, 24.368, 68.00, 0.0},
  {196, 24.411, 68.00, 0.0}, {200, 24.452, 68.00, 0.0}, {204, 24.491, 68.00, 0.0},
  {208, 24.528, 68.00, 0.0}, {212, 24.564, 68.00, 0.0}, {216, 24.597, 68.00, 0.0}};
static g_thres twoth0[132] = { {0, 0, 0, 0},
  {1, 0.425, 45.05, 0.0}, {2, 0.850, 25.87, 0.0}, {3, 1.273, 18.70, 0.0},
  {4, 1.694, 14.85, 0.0}, {5, 2.112, 12.41, 0.0}, {6, 2.525, 10.72, 0.0},
  {7, 2.934, 9.47, 0.0}, {8, 3.337, 8.50, 0.0}, {9, 3.733, 7.73, 0.0},
  {10, 4.124, 7.10, 0.0}, {11, 4.507, 6.56, 0.0}, {12, 4.882, 6.11, 0.0},
  {13, 5.249, 5.72, 0.0}, {14, 5.608, 5.37, 0.0}, {15, 5.959, 5.07, 0.0},
  {16, 6.301, 4.79, 0.0}, {17, 6.634, 4.55, 0.0}, {18, 6.959, 4.32, 0.0},
  {19, 7.274, 4.11, 0.0}, {20, 7.581, 3.92, 0.0}, {21, 7.879, 3.74, 0.0},
  {22, 8.169, 3.57, 0.0}, {23, 8.450, 3.40, 0.0}, {24, 8.723, 3.25, 0.0},
  {25, 8.987, 3.10, 0.0}, {26, 9.244, 2.95, 0.0}, {27, 9.493, 2.81, 0.0},
  {28, 9.734, 2.67, 0.0}, {29, 9.968, 2.53, 0.0}, {30, 10.195, 2.39, 0.0},
  {31, 10.416, 2.25, 0.0}, {32, 10.629, 2.11, 0.0}, {33, 10.836, 1.97, 0.0},
  {34, 11.037, 1.83, 0.0}, {35, 11.232, 1.68, 0.0}, {36, 11.421, 1.53, 0.0},
  {37, 11.605, 1.38, 0.0}, {38, 11.783, 1.23, 0.0}, {39, 11.957, 1.07, 0.0},
  {40, 12.125, 0.90, 0.0}, {41, 12.289, 0.74, 0.0}, {42, 12.448, 0.56, 0.0},
  {43, 12.603, 0.39, 0.0}, {44, 12.753, 0.21, 0.0}, {45, 12.900, 0.02, 0.0},
  {46, 13.042, -0.17, 0.0}, {47, 13.181, -0.36, 0.0}, {48, 13.317, -0.56, 0.0},
  {50, 13.577, -0.96, 0.0}, {52, 13.826, -1.38, 0.0}, {54, 14.062, -1.79, 0.0},
  {56, 14.288, -2.21, 0.0}, {58, 14.504, -2.63, 0.0}, {60, 14.711, -3.03, 0.0},
  {62, 14.909, -3.41, 0.0}, {64, 15.100, -3.77, 0.0}, {66, 15.284, -4.09, 0.0},
  {68, 15.460, -4.37, 0.0}, {70, 15.631, -4.60, 0.0}, {72, 15.796, -4.78, 0.0},
  {74, 15.955, -4.91, 0.0}, {76, 16.110, -4.97, 0.0}, {78, 16.260, -4.98, 0.0},
  {80, 16.406, -4.92, 0.0}, {82, 16.547, -4.81, 0.0}, {84, 16.685, -4.65, 0.0},
  {86, 16.820, -4.43, 0.0}, {88, 16.951, -4.17, 0.0}, {90, 17.079, -3.87, 0.0},
  {92, 17.205, -3.54, 0.0}, {94, 17.327, -3.19, 0.0}, {96, 17.447, -2.82, 0.0},
  {100, 17.680, -2.06, 0.0}, {104, 17.905, -1.32, 0.0}, {108, 18.121, -0.64, 0.0},
  {112, 18.331, -0.04, 0.0}, {116, 18.534, 0.47, 0.0}, {120, 18.731, 0.89, 0.0},
  {124, 18.922, 1.23, 0.0}, {128, 19.108, 1.51, 0.0}, {132, 19.289, 1.74, 0.0},
  {136, 19.464, 1.93, 0.0}, {140, 19.635, 2.11, 0.0}, {144, 19.801, 2.28, 0.0},
  {148, 19.963, 2.46, 0.0}, {152, 20.120, 2.63, 0.0}, {156, 20.273, 2.82, 0.0},
  {160, 20.421, 3.03, 0.0}, {164, 20.565, 3.25, 0.0}, {168, 20.705, 3.49, 0.0},
  {172, 20.840, 3.74, 0.0}, {176, 20.972, 4.02, 0.0}, {180, 21.099, 4.32, 0.0},
  {184, 21.222, 4.64, 0.0}, {188, 21.342, 4.98, 0.0}, {192, 21.457, 5.35, 0.0},
  {200, 21.677, 6.15, 0.0}, {208, 21.882, 7.07, 0.0}, {216, 22.074, 8.10, 0.0},
  {224, 22.253, 9.25, 0.0}, {232, 22.420, 10.54, 0.0}, {240, 22.576, 11.97, 0.0},
  {248, 22.721, 13.56, 0.0}, {256, 22.857, 15.31, 0.0}, {264, 22.984, 17.23, 0.0},
  {272, 23.102, 19.34, 0.0}, {280, 23.213, 21.64, 0.0}, {288, 23.317, 24.15, 0.0},
  {296, 23.415, 26.88, 0.0}, {304, 23.506, 29.84, 0.0}, {312, 23.592, 33.05, 0.0},
  {320, 23.673, 36.52, 0.0}, {328, 23.749, 40.25, 0.0}, {336, 23.821, 44.27, 0.0},
  {344, 23.888, 48.59, 0.0}, {352, 23.952, 53.22, 0.0}, {360, 24.013, 58.18, 0.0},
  {368, 24.070, 63.49, 0.0}, {376, 24.125, 68.00, 0.0}, {384, 24.176, 68.00, 0.0},
  {392, 24.225, 68.00, 0.0}, {400, 24.271, 68.00, 0.0}, {408, 24.316, 68.00, 0.0},
  {416, 24.358, 68.00, 0.0}, {424, 24.398, 68.00, 0.0}, {432, 24.436, 68.00, 0.0},
  {440, 24.473, 68.00, 0.0}, {448, 24.508, 68.00, 0.0}, {456, 24.542, 68.00, 0.0},
  {464, 24.574, 68.00, 0.0}};
static g_thres twoth1[128] = { {0, 0, 0, 0},
  {1, 0.463, 42.10, 0.0}, {2, 0.925, 24.17, 0.0}, {3, 1.385, 17.47, 0.0},
  {4, 1.842, 13.87, 0.0}, {5, 2.295, 11.60, 0.0}, {6, 2.742, 10.01, 0.0},
  {7, 3.184, 8.84, 0.0}, {8, 3.618, 7.94, 0.0}, {9, 4.045, 7.22, 0.0},
  {10, 4.463, 6.62, 0.0}, {11, 4.872, 6.12, 0.0}, {12, 5.272, 5.70, 0.0},
  {13, 5.661, 5.33, 0.0}, {14, 6.041, 5.00, 0.0}, {15, 6.411, 4.71, 0.0},
  {16, 6.770, 4.45, 0.0}, {17, 7.119, 4.21, 0.0}, {18, 7.457, 4.00, 0.0},
  {19, 7.785, 3.79, 0.0}, {20, 8.103, 3.61, 0.0}, {21, 8.410, 3.43, 0.0},
  {22, 8.708, 3.26, 0.0}, {23, 8.996, 3.09, 0.0}, {24, 9.275, 2.93, 0.0},
  {25, 9.544, 2.78, 0.0}, {26, 9.805, 2.63, 0.0}, {27, 10.057, 2.47, 0.0},
  {28, 10.301, 2.32, 0.0}, {29, 10.537, 2.17, 0.0}, {30, 10.765, 2.02, 0.0},
  {31, 10.986, 1.86, 0.0}, {32, 11.199, 1.71, 0.0}, {33, 11.406, 1.55, 0.0},
  {34, 11.606, 1.38, 0.0}, {35, 11.800, 1.21, 0.0}, {36, 11.988, 1.04, 0.0},
  {37, 12.170, 0.86, 0.0}, {38, 12.347, 0.67, 0.0}, {39, 12.518, 0.49, 0.0},
  {40, 12.684, 0.29, 0.0}, {41, 12.845, 0.09, 0.0}, {42, 13.002, -0.11, 0.0},
  {43, 13.154, -0.32, 0.0}, {44, 13.302, -0.54, 0.0}, {45, 13.446, -0.75, 0.0},
  {46, 13.586, -0.97, 0.0}, {47, 13.723, -1.20, 0.0}, {48, 13.855, -1.43, 0.0},
  {50, 14.111, -1.88, 0.0}, {52, 14.354, -2.34, 0.0}, {54, 14.585, -2.79, 0.0},
  {56, 14.807, -3.22, 0.0}, {58, 15.018, -3.62, 0.0}, {60, 15.221, -3.98, 0.0},
  {62, 15.415, -4.30, 0.0}, {64, 15.602, -4.57, 0.0}, {66, 15.783, -4.77, 0.0},
  {68, 15.956, -4.91, 0.0}, {70, 16.124, -4.98, 0.0}, {72, 16.287, -4.97, 0.0},
  {74, 16.445, -4.90, 0.0}, {76, 16.598, -4.76, 0.0}, {78, 16.746, -4.55, 0.0},
  {80, 16.891, -4.29, 0.0}, {82, 17.032, -3.99, 0.0}, {84, 17.169, -3.64, 0.0},
  {86, 17.303, -3.26, 0.0}, {88, 17.434, -2.86, 0.0}, {90, 17.563, -2.45, 0.0},
  {92, 17.688, -2.04, 0.0}, {94, 17.811, -1.63, 0.0}, {96, 17.932, -1.24, 0.0},
  {100, 18.166, -0.51, 0.0}, {104, 18.392, 0.12, 0.0}, {108, 18.611, 0.64, 0.0},
  {112, 18.823, 1.06, 0.0}, {116, 19.028, 1.39, 0.0}, {120, 19.226, 1.66, 0.0},
  {124, 19.419, 1.88, 0.0}, {128, 19.606, 2.08, 0.0}, {132, 19.788, 2.27, 0.0},
  {136, 19.964, 2.46, 0.0}, {140, 20.135, 2.65, 0.0}, {144, 20.300, 2.86, 0.0},
  {148, 20.461, 3.09, 0.0}, {152, 20.616, 3.33, 0.0}, {156, 20.766, 3.60, 0.0},
  {160, 20.912, 3.89, 0.0}, {164, 21.052, 4.20, 0.0}, {168, 21.188, 4.54, 0.0},
  {172, 21.318, 4.91, 0.0}, {176, 21.445, 5.31, 0.0}, {180, 21.567, 5.73, 0.0},
  {184, 21.684, 6.18, 0.0}, {188, 21.797, 6.67, 0.0}, {192, 21.906, 7.19, 0.0},
  {200, 22.113, 8.33, 0.0}, {208, 22.304, 9.63, 0.0}, {216, 22.482, 11.08, 0.0},
  {224, 22.646, 12.71, 0.0}, {232, 22.799, 14.53, 0.0}, {240, 22.941, 16.54, 0.0},
  {248, 23.072, 18.77, 0.0}, {256, 23.195, 21.23, 0.0}, {264, 23.309, 23.94, 0.0},
  {272, 23.415, 26.90, 0.0}, {280, 23.515, 30.14, 0.0}, {288, 23.607, 33.67, 0.0},
  {296, 23.694, 37.51, 0.0}, {304, 23.775, 41.67, 0.0}, {312, 23.852, 46.17, 0.0},
  {320, 23.923, 51.04, 0.0}, {328, 23.991, 56.29, 0.0}, {336, 24.054, 61.94, 0.0},
  {344, 24.114, 68.00, 0.0}, {352, 24.171, 68.00, 0.0}, {360, 24.224, 68.00, 0.0},
  {368, 24.275, 68.00, 0.0}, {376, 24.322, 68.00, 0.0}, {384, 24.368, 68.00, 0.0},
  {392, 24.411, 68.00, 0.0}, {400, 24.452, 68.00, 0.0}, {408, 24.491, 68.00, 0.0},
  {416, 24.528, 68.00, 0.0}, {424, 24.564, 68.00, 0.0}, {432, 24.597, 68.00, 0.0}};
static g_thres twoth2[134] = { {0, 0, 0, 0},
  {1, 0.309, 58.23, 0.0}, {2, 0.617, 33.44, 0.0}, {3, 0.925, 24.17, 0.0},
  {4, 1.232, 19.20, 0.0}, {5, 1.538, 16.05, 0.0}, {6, 1.842, 13.87, 0.0},
  {7, 2.145, 12.26, 0.0}, {8, 2.445, 11.01, 0.0}, {9, 2.742, 10.01, 0.0},
  {10, 3.037, 9.20, 0.0}, {11, 3.329, 8.52, 0.0}, {12, 3.618, 7.94, 0.0},
  {13, 3.903, 7.44, 0.0}, {14, 4.185, 7.00, 0.0}, {15, 4.463, 6.62, 0.0},
  {16, 4.736, 6.28, 0.0}, {17, 5.006, 5.97, 0.0}, {18, 5.272, 5.70, 0.0},
  {19, 5.533, 5.44, 0.0}, {20, 5.789, 5.21, 0.0}, {21, 6.041, 5.00, 0.0},
  {22, 6.289, 4.80, 0.0}, {23, 6.532, 4.62, 0.0}, {24, 6.770, 4.45, 0.0},
  {25, 7.004, 4.29, 0.0}, {26, 7.233, 4.14, 0.0}, {27, 7.457, 4.00, 0.0},
  {28, 7.677, 3.86, 0.0}, {29, 7.892, 3.73, 0.0}, {30, 8.103, 3.61, 0.0},
  {31, 8.309, 3.49, 0.0}, {32, 8.511, 3.37, 0.0}, {33, 8.708, 3.26, 0.0},
  {34, 8.901, 3.15, 0.0}, {35, 9.090, 3.04, 0.0}, {36, 9.275, 2.93, 0.0},
  {37, 9.456, 2.83, 0.0}, {38, 9.632, 2.73, 0.0}, {39, 9.805, 2.63, 0.0},
  {40, 9.974, 2.53, 0.0}, {41, 10.139, 2.42, 0.0}, {42, 10.301, 2.32, 0.0},
  {43, 10.459, 2.22, 0.0}, {44, 10.614, 2.12, 0.0}, {45, 10.765, 2.02, 0.0},
  {46, 10.913, 1.92, 0.0}, {47, 11.058, 1.81, 0.0}, {48, 11.199, 1.71, 0.0},
  {50, 11.474, 1.49, 0.0}, {52, 11.736, 1.27, 0.0}, {54, 11.988, 1.04, 0.0},
  {56, 12.230, 0.80, 0.0}, {58, 12.461, 0.55, 0.0}, {60, 12.684, 0.29, 0.0},
  {62, 12.898, 0.02, 0.0}, {64, 13.104, -0.25, 0.0}, {66, 13.302, -0.54, 0.0},
  {68, 13.493, -0.83, 0.0}, {70, 13.678, -1.12, 0.0}, {72, 13.855, -1.43, 0.0},
  {74, 14.027, -1.73, 0.0}, {76, 14.193, -2.04, 0.0}, {78, 14.354, -2.34, 0.0},
  {80, 14.509, -2.64, 0.0}, {82, 14.660, -2.93, 0.0}, {84, 14.807, -3.22, 0.0},
  {86, 14.949, -3.49, 0.0}, {88, 15.087, -3.74, 0.0}, {90, 15.221, -3.98, 0.0},
  {92, 15.351, -4.20, 0.0}, {94, 15.478, -4.40, 0.0}, {96, 15.602, -4.57, 0.0},
  {100, 15.841, -4.82, 0.0}, {104, 16.069, -4.96, 0.0}, {108, 16.287, -4.97, 0.0},
  {112, 16.496, -4.86, 0.0}, {116, 16.697, -4.63, 0.0}, {120, 16.891, -4.29, 0.0},
  {124, 17.078, -3.87, 0.0}, {128, 17.259, -3.39, 0.0}, {132, 17.434, -2.86, 0.0},
  {136, 17.605, -2.31, 0.0}, {140, 17.770, -1.77, 0.0}, {144, 17.932, -1.24, 0.0},
  {148, 18.089, -0.74, 0.0}, {152, 18.242, -0.29, 0.0}, {156, 18.392, 0.12, 0.0},
  {160, 18.539, 0.48, 0.0}, {164, 18.682, 0.79, 0.0}, {168, 18.823, 1.06, 0.0},
  {172, 18.960, 1.29, 0.0}, {176, 19.095, 1.49, 0.0}, {180, 19.226, 1.66, 0.0},
  {184, 19.356, 1.81, 0.0}, {188, 19.482, 1.95, 0.0}, {192, 19.606, 2.08, 0.0},
  {200, 19.847, 2.33, 0.0}, {208, 20.079, 2.59, 0.0}, {216, 20.300, 2.86, 0.0},
  {224, 20.513, 3.17, 0.0}, {232, 20.717, 3.51, 0.0}, {240, 20.912, 3.89, 0.0},
  {248, 21.098, 4.31, 0.0}, {256, 21.275, 4.79, 0.0}, {264, 21.445, 5.31, 0.0},
  {272, 21.606, 5.88, 0.0}, {280, 21.760, 6.50, 0.0}, {288, 21.906, 7.19, 0.0},
  {296, 22.046, 7.93, 0.0}, {304, 22.178, 8.75, 0.0}, {312, 22.304, 9.63, 0.0},
  {320, 22.424, 10.58, 0.0}, {328, 22.538, 11.60, 0.0}, {336, 22.646, 12.71, 0.0},
  {344, 22.749, 13.90, 0.0}, {352, 22.847, 15.18, 0.0}, {352, 22.941, 16.54, 0.0},
  {368, 23.030, 18.01, 0.0}, {376, 23.114, 19.57, 0.0}, {384, 23.195, 21.23, 0.0},
  {382, 23.272, 23.01, 0.0}, {400, 23.345, 24.90, 0.0}, {408, 23.415, 26.90, 0.0},
  {416, 23.482, 29.03, 0.0}, {424, 23.546, 31.28, 0.0}, {432, 23.607, 33.67, 0.0},
  {440, 23.666, 36.19, 0.0}, {448, 23.722, 38.86, 0.0}, {456, 23.775, 41.67, 0.0},
  {464, 23.827, 44.63, 0.0}, {472, 23.876, 47.76, 0.0}, {480, 23.923, 51.04, 0.0}};

static int init1, init2, init3, init4, init5, init6, off[2];

#define LN10_OVER_10 0.230258509299   // ln(10)/10

static double power10(double a)
{
  return exp(a * LN10_OVER_10);
}

void tonal_init()
{
  init1 = 0;
  init2 = 0;
  init3 = 0;
  init4 = 0;
  init5 = 0;
  init6 = 0;
  off[0] = 256;
  off[1] = 256;
}

/**********************************************************************/
/*
/*        This module implements the psychoacoustic model I for the
/* MPEG encoder layer II. It uses simplified tonal and noise masking
/* threshold analysis to generate SMR for the encoder bit allocation
/* routine.
/*
/**********************************************************************/
int crit_band;
int *cbound;
int sub_size;
void read_cbound(           /* this function reads in critical */
int lay, int freq)              /* band boundaries                 */
{
  switch (freq)
  {
    case 0:
      if (lay == 1)
      {
        cbound = &onecb0[0];
        crit_band = 25;
      }
      else
      {
        cbound = &twocb0[0];
        crit_band = 27;
      }
      break;
    case 1:
      if (lay == 1)
      {
        cbound = &onecb1[0];
        crit_band = 26;
      }
      else
      {
        cbound = &twocb1[0];
        crit_band = 27;
      }
      break;
    case 2:
      if (lay == 1)
      {
        cbound = &onecb2[0];
        crit_band = 24;
      }
      else
      {
        cbound = &twocb2[0];
        crit_band = 25;
      }
      break;
  }
}
void read_freq_band(               /* this function reads in   */
g_ptr *ltg,                        /* frequency bands and bark */
int lay, int freq)                 /* values                   */
{
  switch (freq)
  {
    case 0:
      if (lay == 1)
      {
        *ltg = &oneth0[0];
        sub_size = 108;
      }
      else
      {
        *ltg = &twoth0[0];
        sub_size = 132;
      }
      break;
    case 1:
      if (lay == 1)
      {
        *ltg = &oneth1[0];
        sub_size = 104;
      }
      else
      {
        *ltg = &twoth1[0];
        sub_size = 128;
      }
      break;
    case 2:
      if (lay == 1)
      {
        *ltg = &oneth2[0];
        sub_size = 104;
      }
      else
      {
        *ltg = &twoth2[0];
        sub_size = 134;
      }
      break;
  }
}
void make_map(                  /* this function calculates the */
mask power[HAN_SIZE],   /* global masking threshold     */
g_thres *ltg)
{
 int i,j;
 for(i=1;i<sub_size;i++) for(j=ltg[i-1].line;j<=ltg[i].line;j++)
    power[j].map = i;
}
double add_db(
double a, double b)
{
// a = pow(10.0,a/10.0);
// b = pow(10.0,b/10.0);
 a = power10(a);
 b = power10(b);
 return 10 * log10(a+b);
}
static DFFT x_r;
static DFFT x_i;
static DFFT energy;
static IFFT rev;
static D10 w_r;
static D10 w_i;
/****************************************************************/
/*
/*        Fast Fourier transform of the input samples.
/*
/****************************************************************/
void II_f_f_t(                    /* this function calculates an */
double sample[FFT_SIZE],  /* FFT analysis for the freq.  */
mask power[HAN_SIZE])     /* domain                      */
{
 int i,j,k,L,l=0;
 int ip, le, le1;
 double t_r, t_i, u_r, u_i;
 static int M, MM1, N;
 for(i=0;i<FFT_SIZE;i++) x_r[i] = x_i[i] = energy[i] = 0;
 if(!init1){
    M = 10;
    MM1 = 9;
    N = FFT_SIZE;
    for(L=0;L<M;L++){
       le = 1 << (M-L);
       le1 = le >> 1;
       w_r[L] = cos(PI/le1);
       w_i[L] = -sin(PI/le1);
    }
    for(i=0;i<FFT_SIZE;rev[i] = l,i++) for(j=0,l=0;j<10;j++){
       k=(i>>j) & 1;
       l |= (k<<(9-j));
    }
    init1 = 1;
 }
 memcpy( (char *) x_r, (char *) sample, sizeof(double) * FFT_SIZE);
 for(L=0;L<MM1;L++){
    le = 1 << (M-L);
    le1 = le >> 1;
    u_r = 1;
    u_i = 0;
    for(j=0;j<le1;j++){
       for(i=j;i<N;i+=le){
          ip = i + le1;
          t_r = x_r[i] + x_r[ip];
          t_i = x_i[i] + x_i[ip];
          x_r[ip] = x_r[i] - x_r[ip];
          x_i[ip] = x_i[i] - x_i[ip];
          x_r[i] = t_r;
          x_i[i] = t_i;
          t_r = x_r[ip];
          x_r[ip] = x_r[ip] * u_r - x_i[ip] * u_i;
          x_i[ip] = x_i[ip] * u_r + t_r * u_i;
       }
       t_r = u_r;
       u_r = u_r * w_r[L] - u_i * w_i[L];
       u_i = u_i * w_r[L] + t_r * w_i[L];
    }
 }
 for(i=0;i<N;i+=2){
    ip = i + 1;
    t_r = x_r[i] + x_r[ip];
    t_i = x_i[i] + x_i[ip];
    x_r[ip] = x_r[i] - x_r[ip];
    x_i[ip] = x_i[i] - x_i[ip];
    x_r[i] = t_r;
    x_i[i] = t_i;
    energy[i] = x_r[i] * x_r[i] + x_i[i] * x_i[i];
 }
 for(i=0;i<FFT_SIZE;i++) if(i<rev[i]){
    t_r = energy[i];
    energy[i] = energy[rev[i]];
    energy[rev[i]] = t_r;
 }
 for(i=0;i<HAN_SIZE;i++){    /* calculate power density spectrum */
    if (energy[i] < 1E-20) energy[i] = 1E-20;
    power[i].x = 10 * log10(energy[i]) + POWERNORM;
    power[i].next = STOP;
    power[i].type = FALSE;
 }
}
static DFFT window;
/****************************************************************/
/*
/*         Window the incoming audio signal.
/*
/****************************************************************/
void II_hann_win(                 /* this function calculates a  */
double sample[FFT_SIZE])  /* Hann window for PCM (input) */
{                                 /* samples for a 1024-pt. FFT  */
 register int i;
 register double sqrt_8_over_3;
 if(!init2){  /* calculate window function for the Fourier transform */
    sqrt_8_over_3 = pow(8.0/3.0, 0.5);
    for(i=0;i<FFT_SIZE;i++){
       /* Hann window formula */
       window[i]=sqrt_8_over_3*0.5*(1-cos(2.0*PI*i/(FFT_SIZE)))/FFT_SIZE;
    }
    init2 = 1;
 }
 for(i=0;i<FFT_SIZE;i++) sample[i] *= window[i];
}
/*******************************************************************/
/*
/*        This function finds the maximum spectral component in each
/* subband and return them to the encoder for time-domain threshold
/* determination.
/*
/*******************************************************************/
#ifndef LONDON
void II_pick_max(
mask power[HAN_SIZE],
double spike[SBLIMIT])
{
 double max;
 int i,j;
 for(i=0;i<HAN_SIZE;spike[i>>4] = max, i+=16)      /* calculate the      */
 for(j=0, max = DBMIN;j<16;j++)                    /* maximum spectral   */
    max = (max>power[i+j].x) ? max : power[i+j].x; /* component in each  */
}                                                  /* subband from bound */
                                                   /* 4-16               */
#else
void II_pick_max(
mask power[HAN_SIZE],
double spike[SBLIMIT])
{
 double sum;
 int i,j;
 for(i=0;i<HAN_SIZE;spike[i>>4] = 10.0*log10(sum), i+=16)
                                                   /* calculate the      */
// for(j=0, sum = pow(10.0,0.1*DBMIN);j<16;j++)      /* sum of spectral   */
//   sum += pow(10.0,0.1*power[i+j].x);              /* component in each  */
 for(j=0, sum = power10(DBMIN);j<16;j++)      /* sum of spectral   */
   sum += power10(power[i+j].x);              /* component in each  */
}                                                  /* subband from bound */
                                                   /* 4-16               */
#endif
/****************************************************************/
/*
/*        This function labels the tonal component in the power
/* spectrum.
/*
/****************************************************************/
void II_tonal_label(              /* this function extracts (tonal) */
mask power[HAN_SIZE],     /* sinusoidals from the spectrum  */
int *tone)
{
 int i,j, last = LAST, first, run, last_but_one = LAST; /* dpwe */
 double max;
 *tone = LAST;
 for(i=2;i<HAN_SIZE-12;i++){
    if(power[i].x>power[i-1].x && power[i].x>=power[i+1].x){
       power[i].type = TONE;
       power[i].next = LAST;
       if(last != LAST) power[last].next = i;
       else first = *tone = i;
       last = i;
    }
 }
 last = LAST;
 first = *tone;
 *tone = LAST;
 while(first != LAST){               /* the conditions for the tonal          */
    if(first<3 || first>500) run = 0;/* otherwise k+/-j will be out of bounds */
    else if(first<63) run = 2;       /* components in layer II, which         */
    else if(first<127) run = 3;      /* are the boundaries for calc.          */
    else if(first<255) run = 6;      /* the tonal components                  */
    else run = 12;
    max = power[first].x - 7;        /* after calculation of tonal   */
    for(j=2;j<=run;j++)              /* components, set to local max */
       if(max < power[first-j].x || max < power[first+j].x){
          power[first].type = FALSE;
          break;
       }
    if(power[first].type == TONE){   /* extract tonal components */
       int help=first;
       if(*tone==LAST) *tone = first;
       while((power[help].next!=LAST)&&(power[help].next-first)<=run)
          help=power[help].next;
       help=power[help].next;
       power[first].next=help;
       if((first-last)<=run){
          if(last_but_one != LAST) power[last_but_one].next=first;
       }
       if(first>1 && first<500){     /* calculate the sum of the */
          double tmp;                /* powers of the components */
          tmp = add_db(power[first-1].x, power[first+1].x);
          power[first].x = add_db(power[first].x, tmp);
       }
       for(j=1;j<=run;j++){
          power[first-j].x = power[first+j].x = DBMIN;
          power[first-j].next = power[first+j].next = STOP;
          power[first-j].type = power[first+j].type = FALSE;
       }
       last_but_one=last;
       last = first;
       first = power[first].next;
    }
    else {
       int ll;
       if(last == LAST); /* *tone = power[first].next; dpwe */
       else power[last].next = power[first].next;
       ll = first;
       first = power[first].next;
       power[ll].next = STOP;
    }
 }
}
/****************************************************************/
/*
/*        This function groups all the remaining non-tonal
/* spectral lines into critical band where they are replaced by
/* one single line.
/*
/****************************************************************/

void noise_label(
mask *power,
int *noise,
g_thres *ltg)
{
 int i,j, centre, last = LAST;
 double index, weight, sum;
                              /* calculate the remaining spectral */
 for(i=0;i<crit_band-1;i++){  /* lines for non-tonal components   */
     for(j=cbound[i],weight = 0.0,sum = DBMIN;j<cbound[i+1];j++){
        if(power[j].type != TONE){
           if(power[j].x != DBMIN){
              sum = add_db(power[j].x,sum);
/* the line below and others under the "MAKE_SENSE" condition are an alternate
   interpretation of "geometric mean". This approach may make more sense but
   it has not been tested with hardware. */
#ifdef MAKE_SENSE
//              weight += pow(10.0, power[j].x/10.0) * (ltg[power[j].map].bark-i);
              weight += power10(power[j].x) * (ltg[power[j].map].bark-i);
#endif
              power[j].x = DBMIN;
           }
        }   /*  check to see if the spectral line is low dB, and if  */
     }      /* so replace the center of the critical band, which is */
            /* the center freq. of the noise component              */
#ifdef MAKE_SENSE
     if(sum <= DBMIN)  centre = (cbound[i+1]+cbound[i]) /2;
     else {
//        index = weight/pow(10.0,sum/10.0);
        index = weight/power10(sum);
        centre = cbound[i] + (int) (index * (double) (cbound[i+1]-cbound[i]) );
     } 
#else
     index = (double)( ((double)cbound[i]) * ((double)(cbound[i+1]-1)) );
     centre = (int)(pow(index,0.5)+0.5);
#endif
    /* locate next non-tonal component until finished; */
    /* add to list of non-tonal components             */
#ifdef MI_OPTION
     /* Masahiro Iwadare's fix for infinite looping problem? */
     if(power[centre].type == TONE) 
       if (power[centre+1].type == TONE) centre++; else centre--;
#else
     /* Mike Li's fix for infinite looping problem */
     if(power[centre].type == FALSE) centre++;
     if(power[centre].type == NOISE){
       if(power[centre].x >= ltg[power[i].map].hear){
         if(sum >= ltg[power[i].map].hear) sum = add_db(power[j].x,sum);
         else
         sum = power[centre].x;
       }
     }
#endif
     if(last == LAST) *noise = centre;
     else {
        power[centre].next = LAST;
        power[last].next = centre;
     }
     power[centre].x = sum;
     power[centre].type = NOISE;        
     last = centre;
 }
}
/****************************************************************/
/*
/*        This function reduces the number of noise and tonal
/* component for further threshold analysis.
/*
/****************************************************************/
void subsampling(
mask power[HAN_SIZE],
g_thres *ltg,
int *tone, int *noise)
{
 int i, old;
 i = *tone; old = STOP;    /* calculate tonal components for */
 while(i!=LAST){           /* reduction of spectral lines    */
    if(power[i].x < ltg[power[i].map].hear){
       power[i].type = FALSE;
       power[i].x = DBMIN;
       if(old == STOP) *tone = power[i].next;
       else power[old].next = power[i].next;
    }
    else old = i;
    i = power[i].next;
 }
 i = *noise; old = STOP;    /* calculate non-tonal components for */
 while(i!=LAST){            /* reduction of spectral lines        */
    if(power[i].x < ltg[power[i].map].hear){
       power[i].type = FALSE;
       power[i].x = DBMIN;
       if(old == STOP) *noise = power[i].next;
       else power[old].next = power[i].next;
    }
    else old = i;
    i = power[i].next;
 }
 i = *tone; old = STOP;
 while(i != LAST){                              /* if more than one */
    if(power[i].next == LAST)break;             /* tonal component  */
    if(ltg[power[power[i].next].map].bark -     /* is less than .5  */
       ltg[power[i].map].bark < 0.5) {          /* bark, take the   */
       if(power[power[i].next].x > power[i].x ){/* maximum          */
          if(old == STOP) *tone = power[i].next;
          else power[old].next = power[i].next;
          power[i].type = FALSE;
          power[i].x = DBMIN;
          i = power[i].next;
       }
       else {
          power[power[i].next].type = FALSE;
          power[power[i].next].x = DBMIN;
          power[i].next = power[power[i].next].next;
          old = i;
       }
    }
    else {
      old = i;
      i = power[i].next;
    }
 }
}
/****************************************************************/
/*
/*        This function calculates the individual threshold and
/* sum with the quiet threshold to find the global threshold.
/*
/****************************************************************/
void threshold(
mask power[HAN_SIZE],
g_thres *ltg,
int *tone, int *noise, int bit_rate)
{
 int k, t;
 double dz, tmps, vf;
 for(k=1;k<sub_size;k++){
    ltg[k].x = DBMIN;
    t = *tone;          /* calculate individual masking threshold for */
    while(t != LAST){   /* components in order to find the global     */
       if(ltg[k].bark-ltg[power[t].map].bark >= -3.0 && /*threshold (LTG)*/
          ltg[k].bark-ltg[power[t].map].bark <8.0){
          dz = ltg[k].bark-ltg[power[t].map].bark; /* distance of bark value*/
          tmps = -1.525-0.275*ltg[power[t].map].bark - 4.5 + power[t].x;
             /* masking function for lower & upper slopes */
          if(-3<=dz && dz<-1) vf = 17*(dz+1)-(0.4*power[t].x +6);
          else if(-1<=dz && dz<0) vf = (0.4 *power[t].x + 6) * dz;
          else if(0<=dz && dz<1) vf = (-17*dz);
          else if(1<=dz && dz<8) vf = -(dz-1) * (17-0.15 *power[t].x) - 17;
          tmps += vf;        
          ltg[k].x = add_db(ltg[k].x, tmps);
       }
       t = power[t].next;
    }
    t = *noise;        /* calculate individual masking threshold  */
    while(t != LAST){  /* for non-tonal components to find LTG    */
       if(ltg[k].bark-ltg[power[t].map].bark >= -3.0 &&
          ltg[k].bark-ltg[power[t].map].bark <8.0){
          dz = ltg[k].bark-ltg[power[t].map].bark; /* distance of bark value */
          tmps = -1.525-0.175*ltg[power[t].map].bark -0.5 + power[t].x;
             /* masking function for lower & upper slopes */
          if(-3<=dz && dz<-1) vf = 17*(dz+1)-(0.4*power[t].x +6);
          else if(-1<=dz && dz<0) vf = (0.4 *power[t].x + 6) * dz;
          else if(0<=dz && dz<1) vf = (-17*dz);
          else if(1<=dz && dz<8) vf = -(dz-1) * (17-0.15 *power[t].x) - 17;
          tmps += vf;
          ltg[k].x = add_db(ltg[k].x, tmps);
       }
       t = power[t].next;
    }
    if(bit_rate<96)ltg[k].x = add_db(ltg[k].hear, ltg[k].x);
    else ltg[k].x = add_db(ltg[k].hear-12.0, ltg[k].x);
 }
}
/****************************************************************/
/*
/*        This function finds the minimum masking threshold and
/* return the value to the encoder.
/*
/****************************************************************/
void II_minimum_mask(
g_thres *ltg,
double ltmin[SBLIMIT],
int sblimit)
{
 double min;
 int i,j;
 j=1;
 for(i=0;i<sblimit;i++)
    if(j>=sub_size-1)                   /* check subband limit, and       */
       ltmin[i] = ltg[sub_size-1].hear; /* calculate the minimum masking  */
    else {                              /* level of LTMIN for each subband*/
       min = ltg[j].x;
       while(((ltg[j].line>>4) == i) && (j < sub_size)){
       if(min>ltg[j].x)  min = ltg[j].x;
       j++;
    }
    ltmin[i] = min;
 }
}
/*****************************************************************/
/*
/*        This procedure is called in musicin to pick out the
/* smaller of the scalefactor or threshold.
/*
/*****************************************************************/
void II_smr(
double ltmin[SBLIMIT], double spike[SBLIMIT], double scale[SBLIMIT], 
int sblimit)
{
 int i;
 double max;
 for(i=0;i<sblimit;i++){                     /* determine the signal   */
    max = 20 * log10(scale[i] * 32768) - 10; /* level for each subband */
    if(spike[i]>max) max = spike[i];         /* for the maximum scale  */
    max -= ltmin[i];                         /* factors                */
    ltmin[i] = max;
 }
}
static DFFT sample;
static D2SBL spike;
static D1408 fft_buf[2];
static mask power[HAN_SIZE];
/****************************************************************/
/*
/*        This procedure calls all the necessary functions to
/* complete the psychoacoustic analysis.
/*
/****************************************************************/
void II_Psycho_One(
short buffer[2][1152],
double scale[2][SBLIMIT], double ltmin[2][SBLIMIT],
frame_params *fr_ps)
{
 layer *info = fr_ps->header;
 int   stereo = fr_ps->stereo;
 int   sblimit = fr_ps->sblimit;
 int k,i, tone=0, noise=0;
 static g_ptr ltg;
     /* call functions for critical boundaries, freq. */
 if(!init3){  /* bands, bark values, and mapping */
    read_cbound(info->lay,info->sampling_frequency);
    read_freq_band(&ltg,info->lay,info->sampling_frequency);
    make_map(power,ltg);
    for (i=0;i<1408;i++) fft_buf[0][i] = fft_buf[1][i] = 0;
    init3 = 1;
 }
 for(k=0;k<stereo;k++){  /* check pcm input for 3 blocks of 384 samples */
    for(i=0;i<1152;i++) fft_buf[k][(i+off[k])%1408]= (double)buffer[k][i]/SCALE;
    for(i=0;i<FFT_SIZE;i++) sample[i] = fft_buf[k][(i+1216+off[k])%1408];
    off[k] += 1152;
    off[k] %= 1408;
                            /* call functions for windowing PCM samples,*/
    II_hann_win(sample);    /* location of spectral components in each  */
    for(i=0;i<HAN_SIZE;i++) power[i].x = DBMIN;  /*subband with labeling*/
    II_f_f_t(sample, power);                     /*locate remaining non-*/
    II_pick_max(power, &spike[k][0]);            /*tonal sinusoidals,   */
    II_tonal_label(power, &tone);                /*reduce noise & tonal */
    noise_label(power, &noise, ltg);             /*components, find     */
    subsampling(power, ltg, &tone, &noise);      /*global & minimal     */
    threshold(power, ltg, &tone, &noise,         /*threshold, and sgnl- */
      bitrate[info->lay-1][info->bitrate_index]/stereo); /*to-mask ratio*/
    II_minimum_mask(ltg, &ltmin[k][0], sblimit);
    II_smr(&ltmin[k][0], &spike[k][0], &scale[k][0], sblimit);
 }
}
/**********************************************************************/
/*
/*        This module implements the psychoacoustic model I for the
/* MPEG encoder layer I. It uses simplified tonal and noise masking
/* threshold analysis to generate SMR for the encoder bit allocation
/* routine.
/*
/**********************************************************************/
/****************************************************************/
/*
/*        Fast Fourier transform of the input samples.
/*
/****************************************************************/
void I_f_f_t(                       /* this function calculates */
double sample[FFT_SIZE/2],  /* an FFT analysis for the  */
mask power[HAN_SIZE/2])     /* freq. domain             */
{
 int i,j,k,L,l=0;
 int ip, le, le1;
 double t_r, t_i, u_r, u_i;
 static int M, MM1, N;
 for(i=0;i<FFT_SIZE/2;i++) x_r[i] = x_i[i] = energy[i] = 0;
 if(!init4){
    M = 9;
    MM1 = 8;
    N = FFT_SIZE/2;
    for(L=0;L<M;L++){
       le = 1 << (M-L);
       le1 = le >> 1;
       w_r[L] = cos(PI/le1);
       w_i[L] = -sin(PI/le1);
    }
    for(i=0;i<FFT_SIZE/2;rev[i] = l,i++) for(j=0,l=0;j<9;j++){
       k=(i>>j) & 1;
       l |= (k<<(8-j));
    }
    init4 = 1;
 }
 memcpy( (char *) x_r, (char *) sample, sizeof(double) * FFT_SIZE/2);
 for(L=0;L<MM1;L++){
    le = 1 << (M-L);
    le1 = le >> 1;
    u_r = 1;
    u_i = 0;
    for(j=0;j<le1;j++){
       for(i=j;i<N;i+=le){
          ip = i + le1;
          t_r = x_r[i] + x_r[ip];
          t_i = x_i[i] + x_i[ip];
          x_r[ip] = x_r[i] - x_r[ip];
          x_i[ip] = x_i[i] - x_i[ip];
          x_r[i] = t_r;
          x_i[i] = t_i;
          t_r = x_r[ip];
          x_r[ip] = x_r[ip] * u_r - x_i[ip] * u_i;
          x_i[ip] = x_i[ip] * u_r + t_r * u_i;
       }
       t_r = u_r;
       u_r = u_r * w_r[L] - u_i * w_i[L];
       u_i = u_i * w_r[L] + t_r * w_i[L];
    }
 }
 for(i=0;i<N;i+=2){
    ip = i + 1;
    t_r = x_r[i] + x_r[ip];
    t_i = x_i[i] + x_i[ip];
    x_r[ip] = x_r[i] - x_r[ip];
    x_i[ip] = x_i[i] - x_i[ip];
    x_r[i] = t_r;
    x_i[i] = t_i;
    energy[i] = x_r[i] * x_r[i] + x_i[i] * x_i[i];
 }
 for(i=0;i<FFT_SIZE/2;i++) if(i<rev[i]){
    t_r = energy[i];
    energy[i] = energy[rev[i]];
    energy[rev[i]] = t_r;
 }
 for(i=0;i<HAN_SIZE/2;i++){                     /* calculate power  */
    if(energy[i] < 1E-20) energy[i] = 1E-20;    /* density spectrum */
       power[i].x = 10 * log10(energy[i]) + POWERNORM;
       power[i].next = STOP;
       power[i].type = FALSE;
 }
}
/****************************************************************/
/*
/*         Window the incoming audio signal.
/*
/****************************************************************/
void I_hann_win(                    /* this function calculates a  */
double sample[FFT_SIZE/2])  /* Hann window for PCM (input) */
{                                   /* samples for a 512-pt. FFT   */
 register int i;
 register double sqrt_8_over_3;
 if(!init5){  /* calculate window function for the Fourier transform */
    sqrt_8_over_3 = pow(8.0/3.0, 0.5);
    for(i=0;i<FFT_SIZE/2;i++){
      /* Hann window formula */
      window[i]=sqrt_8_over_3*0.5*(1-cos(2.0*PI*i/(FFT_SIZE/2)))/(FFT_SIZE/2);
    }
    init5 = 1;
 }
 for(i=0;i<FFT_SIZE/2;i++) sample[i] *= window[i];
}
/*******************************************************************/
/*
/*        This function finds the maximum spectral component in each
/* subband and return them to the encoder for time-domain threshold
/* determination.
/*
/*******************************************************************/
#ifndef LONDON
void I_pick_max(
mask power[HAN_SIZE/2],
double spike[SBLIMIT])
{
 double max;
 int i,j;
 /* calculate the spectral component in each subband */
 for(i=0;i<HAN_SIZE/2;spike[i>>3] = max, i+=8)
    for(j=0, max = DBMIN;j<8;j++) max = (max>power[i+j].x) ? max : power[i+j].x;
}
#else
void I_pick_max(
mask power[HAN_SIZE],
double spike[SBLIMIT])
{
 double sum;
 int i,j;
 for(i=0;i<HAN_SIZE/2;spike[i>>3] = 10.0*log10(sum), i+=8)
                                                   /* calculate the      */
// for(j=0, sum = pow(10.0,0.1*DBMIN);j<8;j++)       /* sum of spectral   */
//   sum += pow(10.0,0.1*power[i+j].x);              /* component in each  */
 for(j=0, sum = power10(DBMIN);j<8;j++)       /* sum of spectral   */
   sum += power10(power[i+j].x);              /* component in each  */
}                                                  /* subband from bound */
#endif
/****************************************************************/
/*
/*        This function labels the tonal component in the power
/* spectrum.
/*
/****************************************************************/
void I_tonal_label(                 /* this function extracts   */
mask power[HAN_SIZE/2],     /* (tonal) sinusoidals from */
int *tone)                          /* the spectrum             */
{
 int i,j, last = LAST, first, run;
 double max;
 int last_but_one= LAST;
 *tone = LAST;
 for(i=2;i<HAN_SIZE/2-6;i++){
    if(power[i].x>power[i-1].x && power[i].x>=power[i+1].x){
       power[i].type = TONE;
       power[i].next = LAST;
       if(last != LAST) power[last].next = i;
       else first = *tone = i;
       last = i;
    }
 }
 last = LAST;
 first = *tone;
 *tone = LAST;
 while(first != LAST){                /* conditions for the tonal     */
    if(first<3 || first>250) run = 0; /* otherwise k+/-j will be out of bounds*/
    else if(first<63) run = 2;        /* components in layer I, which */
    else if(first<127) run = 3;       /* are the boundaries for calc.   */
    else run = 6;                     /* the tonal components          */
    max = power[first].x - 7;
    for(j=2;j<=run;j++)  /* after calc. of tonal components, set to loc.*/
       if(max < power[first-j].x || max < power[first+j].x){   /* max   */
          power[first].type = FALSE;
          break;
       }
    if(power[first].type == TONE){    /* extract tonal components */
       int help=first;
       if(*tone == LAST) *tone = first;
       while((power[help].next!=LAST)&&(power[help].next-first)<=run)
          help=power[help].next;
       help=power[help].next;
       power[first].next=help;
       if((first-last)<=run){
          if(last_but_one != LAST) power[last_but_one].next=first;
       }
       if(first>1 && first<255){     /* calculate the sum of the */
          double tmp;                /* powers of the components */
          tmp = add_db(power[first-1].x, power[first+1].x);
          power[first].x = add_db(power[first].x, tmp);
       }
       for(j=1;j<=run;j++){
          power[first-j].x = power[first+j].x = DBMIN;
          power[first-j].next = power[first+j].next = STOP; /*dpwe: 2nd was .x*/
          power[first-j].type = power[first+j].type = FALSE;
       }
       last_but_one=last;
       last = first;
       first = power[first].next;
    }
    else {
       int ll;
       if(last == LAST) ; /* *tone = power[first].next; dpwe */
       else power[last].next = power[first].next;
       ll = first;
       first = power[first].next;
       power[ll].next = STOP;
    }
 }
}                        
                                
/****************************************************************/
/*
/*        This function finds the minimum masking threshold and
/* return the value to the encoder.
/*
/****************************************************************/
void I_minimum_mask(
g_thres *ltg,
double ltmin[SBLIMIT])
{
 double min;
 int i,j;
 j=1;
 for(i=0;i<SBLIMIT;i++)
    if(j>=sub_size-1)                   /* check subband limit, and       */
       ltmin[i] = ltg[sub_size-1].hear; /* calculate the minimum masking  */
    else {                              /* level of LTMIN for each subband*/
       min = ltg[j].x;
       while(((ltg[j].line>>3) == i) && (j < sub_size)){
          if (min>ltg[j].x)  min = ltg[j].x;
          j++;
       }
       ltmin[i] = min;
    }
}
/*****************************************************************/
/*
/*        This procedure is called in musicin to pick out the
/* smaller of the scalefactor or threshold.
/*
/*****************************************************************/
void I_smr(
double ltmin[SBLIMIT], double spike[SBLIMIT], double scale[SBLIMIT])
{
 int i;
 double max;
 for(i=0;i<SBLIMIT;i++){                      /* determine the signal   */
    max = 20 * log10(scale[i] * 32768) - 10;  /* level for each subband */
    if(spike[i]>max) max = spike[i];          /* for the scalefactor    */
    max -= ltmin[i];
    ltmin[i] = max;
 }
}
        
/****************************************************************/
/*
/*        This procedure calls all the necessary functions to
/* complete the psychoacoustic analysis.
/*
/****************************************************************/
void I_Psycho_One(
short buffer[2][1152],
double scale[2][SBLIMIT], double ltmin[2][SBLIMIT],
frame_params *fr_ps)
{
 int stereo = fr_ps->stereo;
 the_layer info = fr_ps->header;
 int k,i, tone=0, noise=0;
 static int off[2] = {256,256};
 static g_ptr ltg;
            /* call functions for critical boundaries, freq. */
 if(!init6){ /* bands, bark values, and mapping              */
    read_cbound(info->lay,info->sampling_frequency);
    read_freq_band(&ltg,info->lay,info->sampling_frequency);
    make_map(power,ltg);
    for(i=0;i<640;i++) fft_buf[0][i] = fft_buf[1][i] = 0;
    init6 = 1;
 }
 for(k=0;k<stereo;k++){    /* check PCM input for a block of */
    for(i=0;i<384;i++)     /* 384 samples for a 512-pt. FFT  */
       fft_buf[k][(i+off[k])%640]= (double) buffer[k][i]/SCALE;
    for(i=0;i<FFT_SIZE/2;i++)
       sample[i] = fft_buf[k][(i+448+off[k])%640];
    off[k] += 384;
    off[k] %= 640;
                        /* call functions for windowing PCM samples,   */
    I_hann_win(sample); /* location of spectral components in each     */
    for(i=0;i<HAN_SIZE/2;i++) power[i].x = DBMIN;   /* subband with    */
    I_f_f_t(sample, power);              /* labeling, locate remaining */
    I_pick_max(power, &spike[k][0]);     /* non-tonal sinusoidals,     */
    I_tonal_label(power, &tone);         /* reduce noise & tonal com., */
    noise_label(power, &noise, ltg);     /* find global & minimal      */
    subsampling(power, ltg, &tone, &noise);  /* threshold, and sgnl-   */
    threshold(power, ltg, &tone, &noise,     /* to-mask ratio          */
      bitrate[info->lay-1][info->bitrate_index]/stereo);
    I_minimum_mask(ltg, &ltmin[k][0]);
    I_smr(&ltmin[k][0], &spike[k][0], &scale[k][0]);
 }
}
