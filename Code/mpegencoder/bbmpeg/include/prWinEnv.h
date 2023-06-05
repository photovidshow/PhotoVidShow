//========================================================================================
//
// prWinEnv.h
//
// Part of the Adobe Premiereª 5 Plug-In Developer's Toolkit.
//
// Defines various Mac types that can be included by Windows plug-ins to provide compatibility.
//
// Copyright © 1992-99, Adobe Systems Incorporated, all rights reserved worldwide.
//
//========================================================================================

#ifndef _H_WinEnv
#define _H_WinEnv

#include <windows.h>
#include <windowsx.h>
#pragma pack(push, 1)


//--------------------------------------------------------------
// Define basic Mac types for Windows builds.
// On the Mac, they're part of the os includes. In Windows, they
// need to be defined here.
//--------------------------------------------------------------

typedef char * Ptr;
typedef Ptr * Handle;			/*  pointer to a master pointer */

typedef long Fixed;				/* fixed point arithmatic type */

typedef unsigned char Boolean;

#ifndef false
#define false 0
#endif

#ifndef true
#define true 1
#endif

#ifndef nil
#define nil 0
#endif

// Emulation of a mac fsspec for windows
struct FSSpec {
	short vRefNum;
	long  parID;
	char  name[_MAX_PATH];
};
typedef struct FSSpec FSSpec;

typedef short OSErr;			/* error code */

typedef unsigned long OSType;
typedef OSType *OSTypePtr;

typedef unsigned long ResType;
typedef ResType *ResTypePtr;

typedef unsigned char Style;

typedef unsigned char Str255[256], Str63[64], Str32[33], Str31[32], Str27[28], Str15[16],
					  *StringPtr, **StringHandle;


/* Error codes. */
#define userCanceledErr 1
#define noErr 0
#define readErr	(-19)
#define writErr (-20)
#define openErr (-23)			/*I/O System Errors*/
#define dskFulErr (-34)
#define nsvErr (-35)			/*no such volume*/
#define ioErr (-36) 			/*I/O error (bummers)*/
#define eofErr (-39)			/*End of file*/
#define tmfoErr (-42)			/*too many files open*/
#define fnfErr (-43)			/*File not found*/
#define wPrErr (-44)			/*diskette is write protected.*/
#define fLckdErr (-45)
#define vLckdErr (-46)
#define fBsyErr  (-47)			/*File is busy (delete)*/
#define dupFNErr (-48)			/*duplicate filename (rename)*/
#define paramErr (-50)			/*Drive Number specified bad*/
#define permErr (-54)			/*permissions error (on file open)*/
#define fsRnErr (-59)			/*file system internal error:during rename the old entry was deleted but could not be restored.*/
#define memFullErr (-108)		/*Not enough room in heap zone*/
#define nilHandleErr (-109)		/*Master Pointer was NIL in HandleZone or other*/
#define memWZErr (-111)
#define dirNFErr (-120)			/*Directory not found*/
#define badExtResource (-185)	/*extended resource has a bad format.*/
#define resNotFound (-192)		/*Resource not found*/

#pragma pack(pop)
#endif
