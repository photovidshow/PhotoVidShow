//========================================================================================
//
// prPlugPPix.h
//
// Part of the Adobe Premiereª 5 Plug-In Developer's Toolkit.
//
// Defines the ppix callback suite. You should always check the piInterfaceVer in piSuites
// before assuming all these functions are available.
//
// Copyright © 1992-99, Adobe Systems Incorporated, all rights reserved worldwide.
//
//========================================================================================

#ifndef __PRPLUGPPIX
#define __PRPLUGPPIX

#include "prStructs.h"

// Prototypes

typedef char *(*plugppixGetPixelsFunc)(PPixHand pix);
typedef void (*plugppixGetBoundsFunc)(PPixHand pix, prRect *bounds);
typedef int (*plugppixGetRowbytesFunc)(PPixHand pix);
typedef PPixHand (*plugppixNewFunc)(prRect *bounds);
typedef void (*plugppixDisposeFunc)(PPixHand pix);
typedef void (*plugppixLockFunc)(PPixHand pix);
typedef void (*plugppixUnlockFunc)(PPixHand pix);


typedef struct
{
	plugppixGetPixelsFunc		ppixGetPixels;
	plugppixGetBoundsFunc		ppixGetBounds;
	plugppixGetRowbytesFunc		ppixGetRowbytes;
	plugppixNewFunc				ppixNew;			// added in PR_PISUITES_VERSION_2
	plugppixDisposeFunc			ppixDispose;		// added in PR_PISUITES_VERSION_2
	plugppixLockFunc			ppixLockPixels;		// added in PR_PISUITES_VERSION_2
	plugppixUnlockFunc			ppixUnlockPixels;	// added in PR_PISUITES_VERSION_2
} PlugppixFuncs, *PlugppixFuncsPtr;

#endif /* __PRPLUGPPIX */
