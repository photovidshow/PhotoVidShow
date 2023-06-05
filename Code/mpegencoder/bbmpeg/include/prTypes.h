//========================================================================================
//
// prTypes.h
//
// Part of the Adobe Premiereª 5 Plug-In Developer's Toolkit.
//
// Defines common types used by the various Premiere interfaces. Every plug-in will 
// probably want to include this one first.
//
// Copyright © 1992-99, Adobe Systems Incorporated, all rights reserved worldwide.
//
//========================================================================================

#ifndef PREM_TYPES
#define PREM_TYPES

#include "prSetEnv.h"


// non-platform-specific items:
//---------------------------------------------------------------------------------

typedef unsigned long	prColor;
typedef long			PrClipID;
typedef short			PrFileID;


// Mac specific items:
//---------------------------------------------------------------------------------
#ifdef PRMAC_ENV

typedef Rect prRect;

struct prPoint {
	short		y;
	short		x;
};
typedef struct prPoint prPoint;

typedef struct {
	unsigned char alpha;
	unsigned char red;
	unsigned char green;
	unsigned char blue;
} prPixel;

typedef RgnHandle	prRgn;

#endif /* PRMAC_ENV */


// Win specific items:
//---------------------------------------------------------------------------------
#ifdef PRWIN_ENV

typedef RECT			prRect;
typedef POINT			prPoint;

typedef struct {
	unsigned char blue;
	unsigned char green;
	unsigned char red;
	unsigned char alpha;
} prPixel;

typedef HRGN	prRgn;

#endif /* PRWIN_ENV */


#include "prResetEnv.h"
#endif	/* PREM_TYPES */
