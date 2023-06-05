//========================================================================================
//
// prRTFilter.h
//
// Part of the Adobe Premiere� 5 Plug-In Developer's Toolkit.
//
// Defines Adobe Premiere RealTime Effects playback and internal Premiere filter 
// definitions. The API defined in this header is valid only with the RT version of Premiere.
//
// Copyright � 1992-99, Adobe Systems Incorporated, all rights reserved worldwide.
//
//========================================================================================

#ifndef __PRRTFILTER_H_
#define __PRRTFILTER_H_

#include "prSetEnv.h"
#include "prrt.h"


//////////////////////////////////////////////////////////////////
//  This section contains the filter type name strings.
//     These strings are found in the "name" field of the
//     "prtFilterRec" structure.
//////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////
// Video Gain (Opacity Rubberband) filter type,
// as set on V2 and above
#define PRT_FILTER_PREM50_VIDEO_GAIN   "Prem50 Video Gain"

//////////////////////////////////////////////////////////////////
// Audio Gain (Volume Rubberband) filter type,
#define PRT_FILTER_PREM50_AUDIO_GAIN   "Prem50 Audio Gain"

//////////////////////////////////////////////////////////////////
// Audio Pan (Pan Rubberband) filter type
#define PRT_FILTER_PREM50_AUDIO_PAN    "Prem50 Audio Pan"

//////////////////////////////////////////////////////////////////
// Video Transparency (keying) filter type
#define PRT_FILTER_PREM50_TRANSPARENCY "Prem50 Transparency"

//////////////////////////////////////////////////////////////////
// Video Motion (DVE) filter type
#define PRT_FILTER_PREM50_MOTION       "Prem50 Motion"


//////////////////////////////////////////////////////////////////
//  This section contains definitions for rubberband
//////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////
// This struct defines a rubberband keyframe.
//    It simply consists of the value of the rubber point
//    in the range 0..256 (incl).  It is used in the "keySpecs"
//    field of the prtKeyframeRec structure with the following 
//    filter type names:
// PRT_FILTER_PREM50_VIDEO_GAIN  ("Prem50 Video Gain")
// PRT_FILTER_PREM50_AUDIO_GAIN��("Prem50 Audio Gain")
// PRT_FILTER_PREM50_AUDIO_PAN���("Prem50 Audio Pan")
typedef struct
{
   long  value;   // the value of the point (0..256)
} prtRubberbandKFRec, **prtRubberbandKFHandle;


//////////////////////////////////////////////////////////////////
//  This section contains definitions for transparency (keying)
//////////////////////////////////////////////////////////////////

// transparency types
#define PRT_TRANSTYPE_NONE                0
#define PRT_TRANSTYPE_CHROMA              1
#define PRT_TRANSTYPE_RGB_DIFFERENCE      2
#define PRT_TRANSTYPE_LUMINANCE           3
#define PRT_TRANSTYPE_ALPHA_CHANNEL       4
#define PRT_TRANSTYPE_BLACK_ALPHA_MATTE   5
#define PRT_TRANSTYPE_WHITE_ALPHA_MATTE   6
#define PRT_TRANSTYPE_IMAGE_MATTE         7
#define PRT_TRANSTYPE_DIFFERENCE_MATTE    8
#define PRT_TRANSTYPE_BLUE_SCREEN         9
#define PRT_TRANSTYPE_GREEN_SCREEN        10
#define PRT_TRANSTYPE_MULTIPLY            11
#define PRT_TRANSTYPE_SCREEN              12
#define PRT_TRANSTYPE_TRACK_MATTE         13
#define PRT_TRANSTYPE_NON_RED             14

// garbage matte image size
// NOTE: the garbage points in the transparency structure are given 
//       in the scale of this rectangle, (0,0) being at the upper-left 
//       corner and (117,87) being at the lower-right corner
#define PRT_GARBAGE_MATTE_WIDTH           117
#define PRT_GARBAGE_MATTE_HEIGHT          87

//////////////////////////////////////////////////
// This struct describes a matte file.
//    It is used in the prtTransparencyKFRec
//    of the following structure to specify the
//    location of the keying matte file.
// It is used only with the PRT_TRANSTYPE_IMAGE_MATTE
//    and the PRT_TRANSTYPE_DIFFERENCE_MATTE keying
//    types.
typedef struct
{
   prtFileSpec    fileSpec;   // filename
   long           type;       // file type id - four char code 
   long           subtype;    // file subtype - four char code
   Handle         fileSpecs;  // private data previously generated by the 
                              //    import module of the file (NULL if none)
   Handle         clipData;   // clip data specific to Premiere
} prtMatteFileRec;


//////////////////////////////////////////////////
// This struct defines a transparency keyframe.
//    It is used in the "keySpecs" field of the
//    prtKeyframeRec structure of the following 
//    filter type name:
// PRT_FILTER_PREM50_TRANSPARENCY ("Prem50 Transparency")
// This filter type always has only one keyframe,
//    since the keying is static for the duration of
//    the clip.
typedef struct
{
   long        size;          // the size of this structure
   long        version;       // set to 1
   long        type;          // keying type, see enum above
   long        similarity;    // similarity slider value (0..100)
   long        blend;         // blend slider value (0..100)
   long        threshold;     // threshold slider value (0..256)
   long        cutoff;        // cutoff slider value (0..256)
   long        aliasing;      // aliasing level (0..2)
   long        shadow;        // 0=shadow off, 1=on
   long        reverseKey;    // 0=reverse off, 1=on
   long        maskOnly;      // 0="mask only" off, 1=on
   prtColor    color;         // matte color
   prtPoint    garbage[4];    // garbage points, relative to matte size
                              //   described above
   prtMatteFileRec matteFile; // matte file info (for image matte and
                              //   difference matte only)
} prtTransparencyKFRec, **prtTransparencyKFHandle;


//////////////////////////////////////////////////////////////////
//  This section contains definitions for motion keyframes
//////////////////////////////////////////////////////////////////

// values for the "smoothMethod" field
#define PRT_SMOOTH_METHOD_SMOOTH             0
#define PRT_SMOOTH_METHOD_SMOOTH_MORE        1
#define PRT_SMOOTH_METHOD_AVERAGING_LOW      2
#define PRT_SMOOTH_METHOD_AVERAGING_MED      3
#define PRT_SMOOTH_METHOD_AVERAGING_HIGH     4

// values for the "motionMethod" field
#define PRT_MOTION_METHOD_LINEAR       0
#define PRT_MOTION_METHOD_ACCELERATE   1
#define PRT_MOTION_METHOD_DECELERATE   2

// the full size of all reference motion rectangles
// NOTE: all points in the structure below are given in the scale
//       of this rectangle, (0,0) being at the upper-left corner
//       and (8000,6000) being at the lower-right corner
#define PRT_MOTION_IMAGE_WIDTH   8000
#define PRT_MOTION_IMAGE_HEIGHT  6000


//////////////////////////////////////////////////
// This struct defines a motion keyframe.
//    It is used in the "keySpecs" field of the
//    prtKeyframeRec structure of the following 
//    filter type name:
// PRT_FILTER_PREM50_MOTION ("Prem50 Motion")
typedef struct
{
   long     size;             // the size of this structure
   long     version;          // set to 1
   long     smooth;           // 0=smoothing off, 1=on
   long     smoothMethod;     // see defines above
   long     motionMethod;     // see defines above
   prtColor fillColor;        // filling color
   long     fillAlpha;        // 0=keep clip's alpha, 1=replace with $FF
   prtPoint centerPoint;      // center point of the image
   long     delay;            // delay percentage
   long     rotation;         // rotation, in degrees
   prtPoint distortPoints[4]; // points defining distort polygon
} prtMotionKFRec, **prtMotionKFHandle;


#include "prResetEnv.h"
#endif //__PRRTFILTER_H__