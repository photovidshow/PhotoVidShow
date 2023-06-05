//========================================================================================
//
// prStructs.h
//
// Part of the Adobe Premiereª 5 Plug-In Developer's Toolkit.
//
// Defines structures used by the various Premiere interfaces - PPix, TDB, BottleRec
//
// Copyright © 1992-99, Adobe Systems Incorporated, all rights reserved worldwide.
//
//========================================================================================

#ifndef __PRSTRUCT_H__
#define __PRSTRUCT_H__

#include "prSetEnv.h"


// PPix struct definition
typedef struct {
	prRect bounds;			// bounds (always 0,0 origin!)
	int	rowbytes;			// rowbytes
	int	bitsperpixel;		// currently always 32
	int	pixelformat;		// 0 == four plane chunky.
							// (no other types are currently defined)
	long	pix;			// The pixels (not accessible)
	long	reserved[32];	// excess data for internal stuff
							// currently used for field processing flags
} PPix, *PPixPtr, **PPixHand;


// Time Datebase (TDB) definition
typedef long			TDB_Time;
typedef long			TDB_SampSize;
typedef unsigned long	TDB_TimeScale;

typedef struct {
	TDB_Time		value;
	TDB_TimeScale	scale;
	TDB_SampSize	sampleSize;
} TDB_TimeRecord;


typedef struct {
		long    x;
		long    y;
} LongPoint;


// Bottlenecks definitions
typedef void (*StretchBitsPtr)(PPixHand srcPix, PPixHand dstPix, prRect *srcRect,
							   prRect *dstRect, int mode, prRgn rgn);
typedef void (*AudStretchPtr)(Ptr src, long srclen, Ptr dest, long destlen, unsigned int flags);
typedef void (*AudMixPtr)(Ptr buf1, long v1, Ptr buf2, long v2, Ptr buf3, long v3,
						  long width, Ptr dest, unsigned int flags);
typedef void (*AudLimitPtr)(Ptr src, Ptr dest, long width, unsigned int flags, long total);
typedef void (*AudSumPtr)(Ptr src, Ptr dest, long width, long scale, unsigned int flags, long part, 
						  long total);
typedef void (*DistortPolygonPtr)(PPixHand src, PPixHand dest, prRect *srcbox, prPoint *dstpts);
typedef void (*DistortFixedPolygonPtr)(PPixHand src, PPixHand dest, prRect *srcbox, LongPoint *dstpts);
typedef void (*PolyToPolyPtr)(PPixHand src, PPixHand dest, prPoint *srcpts, prPoint *dstpts);
typedef void (*FixedToFixedPtr)(PPixHand src, PPixHand dest, LongPoint *srcpts, LongPoint *dstpts);

typedef struct {
	short                              count;	// number of routines
	short                              reserved[14];
	
	StretchBitsPtr                     StretchBits;
	DistortPolygonPtr                  DistortPolygon;
	PolyToPolyPtr                      MapPolygon;
	AudStretchPtr                      AudioStretch;
	AudMixPtr                          AudioMix;
	AudSumPtr                          AudioSum;
	AudLimitPtr                        AudioLimit;
	DistortFixedPolygonPtr             DistortFixed;
	FixedToFixedPtr                    FixedToFixed;  
	long                               unused[6];
} BottleRec;


#include "prResetEnv.h"
#endif	/* __PRSTRUCT_H__ */
