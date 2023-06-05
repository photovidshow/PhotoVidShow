//========================================================================================
//
// prEffect.h
//
// Part of the Adobe Premiereª 5 Plug-In Developer's Toolkit.
//
// Defines the Premiere Transition and Video and Audio Filter interfaces.
//
// Copyright © 1992-99, Adobe Systems Incorporated, all rights reserved worldwide.
//
//========================================================================================

#ifndef __PRPEFFECT_H
#define __PRPEFFECT_H

#include "prSetEnv.h"
#include "prplugsuites.h"

// Transition version defines for the version of the host api that the plug-in is expecting.
// This info is returned to a transition in the EffectRecord.
#define TRANSITION_VERSION_5		5	// Premiere 5.0
#define TRANSITION_VERSION_6		6	// Premiere 5.1
#define TRANSITION_VERSION			TRANSITION_VERSION_6

// Audio filter version defines for the version of the host api that the plug-in is expecting.
// This info is returned to an audio filter in the AudioRecord.
#define AUDIOFILTER_VERSION_5		5	// Premiere 5.0
#define AUDIOFILTER_VERSION_6		6	// Premiere 5.1
#define AUDIOFILTER_VERSION			AUDIOFILTER_VERSION_6

// Video filter version defines for the version of the host api that the plug-in is expecting.
// This info is returned to a video filter in the VideoRecord.
#define VIDEOFILTER_VERSION_5		5	// Premiere 5.0
#define VIDEOFILTER_VERSION_6		6	// Premiere 5.1
#define VIDEOFILTER_VERSION			VIDEOFILTER_VERSION_6

#define PREFFECTENTRY short
#define PREMPLUGENTRY short


//========================================================================================
// Transition Effects
//========================================================================================

// Transition callback prototype

typedef PREFFECTENTRY (*FXCallBackProcPtr)
	(long frame, short track, PPixHand thePort, prRect *theBox, Handle privateData);


// Transition Data Structure

typedef struct {
	Handle					specsHandle;
	PPixHand				source1;		// source pixels 1
	PPixHand				source2;		// source pixels 2
	PPixHand				destination;	// Destination pixels
	long					part;			// part / total = % complete
	long					total;
	char					previewing;		// in preview mode?
	unsigned char			arrowFlags;		// flags for direction arrows
	char					reverse;		// is effect being reversed?
	char					source;			// are sources swapped?
	prPoint					start;			// starting point for effect
	prPoint					end;			// ending point for effect
	prPoint					center;			// the reference center point
	void					*privateData;	// Editor private data handle
	FXCallBackProcPtr		callBack;		// callback, not valid if nil
	BottleRec				*bottleNecks;	// bottleneck callback routines
	short					version;		// version of this record (TRANSITION_VERSION)
	short					sizeFlags;
	long					flags;
	TDB_TimeRecord *		tdb;			// time data base
	piSuitesPtr				piSuites;		// pointer to global callback suites
	Handle					timelineData;	// added in TRANSITION_VERSION_6
	Handle					instanceData;	// added in TRANSITION_VERSION_6
} EffectRecord, **EffectHandle;


// Selector messages
enum {
	esExecute = 0,
	esSetup,
	esAbout,			// not used
	esCanHandleComp,	// not used
	esProcessComp,		// not used
	esDisposeData		// added in TRANSITION_VERSION_6
};


// Effect Corner Bits
enum {
	bitTop =		0x01,
	bitRight =		0x02,
	bitBottom =		0x04,
	bitLeft =		0x08,
	bitUpperRight =	0x10,
	bitLowerRight =	0x20,
	bitLowerLeft =	0x40,
	bitUpperLeft =	0x80
};


// Transition plug-in entry point
typedef PREFFECTENTRY (*EffectProcPtr)(short selector, EffectHandle theData);


//========================================================================================
// Filter Effects
//========================================================================================

// Filter callback prototypes

#ifdef PRMAC_ENV
typedef PREFFECTENTRY (*VFilterCallBackProcPtr)
	(long frame, PPixHand thePort, prRect *theBox, Handle privateData);
typedef PREFFECTENTRY (*AFilterCallBackProcPtr)
	(long sample, long count, Ptr buffer, Handle privateData);
#endif

#ifdef PRWIN_ENV
typedef PREFFECTENTRY (CALLBACK *VFilterCallBackProcPtr)
	(long frame, PPixHand thePort, prRect *theBox, Handle privateData);
typedef PREFFECTENTRY (CALLBACK *AFilterCallBackProcPtr)
	(long sample, long count, Ptr buffer, Handle privateData);
#endif


// Video Filter Data Structure

typedef struct {
	Handle						specsHandle;
	PPixHand					source;
	PPixHand					destination;
	long						part;
	long						total;
	char						previewing;
	void *						privateData;
	VFilterCallBackProcPtr		callBack;
	BottleRec					*bottleNecks;
	short						version;		// version of this record (VIDEOFILTER_VERSION)
	short						sizeFlags;
	long						flags;			// see video flags below
	TDB_TimeRecord *			tdb;
	Handle						instanceData;
	piSuitesPtr					piSuites;
	Handle						timelineData;	// added in VIDEOFILTER_VERSION_6
} VideoRecord, **VideoHandle;


// Audio Filter data structure

typedef struct {
	Handle						specsHandle;
	Ptr							source;
	Ptr							destination;
	long						samplenum;
	long						samplecount;
	char						previewing;
	void *						privateData;
	AFilterCallBackProcPtr		callBack;
	long						totalsamples;
	short						flags;			// see audio flags below
	long						rate;
	BottleRec					*bottleNecks;
	short						version;		// version of this record (AUDIOFILTER_VERSION)
	long						extraFlags;
	TDB_TimeRecord *			tdb;
	Handle						instanceData;
	piSuitesPtr					piSuites;
	Handle						specsHandleEnd;	// added in AUDIOFILTER_VERSION_6
} AudioRecord, **AudioFilter, **AudioHandle;


// Selector messages
enum {
	fsExecute = 0,
	fsSetup,
	fsAbout,		// not used
	fsDisposeData
};


// Flags for video filter
enum {
	gvCache         =       0x8000,
	gvOffScreen     =       0x4000,
	gvCacheSingles  =       0x2000,
	gvFieldsFirst   =       0x1000,

	gvHalfV         =       0x0800,
	gvHalfH         =       0x0400,
	gvFieldsOdd     =       0x0200,
	gvFieldsEven    =       0x0100,
	
	gv1Bit          =       0x0001,
	gv2Bit          =       0x0002,
	gv4Bit          =       0x0004,
	gv8Bit          =       0x0008,
	gv16Bit         =       0x0010,
	gv24Bit         =       0x0018,
	gv32Bit         =       0x0020,
	
	gvFast          =       0x0040,		// Try to draw onscreen, even if not fullsize
	gvThumbs		=		0x0080,		// We're drawing a thumbnail, so draw fast

	gvNone          =       gv32Bit,

	gvDepth         =       0x003F,
	gvFlags         =       0x00FF
};


// Flags for audio filter
enum {
	ga5kHz                  =       0x0001,
	ga8kHz					=		0x0020,
	ga11kHz					=       0x0002,
	ga22kHz					=       0x0004,
	ga44kHz					=       0x0008,
	ga48kHz					=       0x0010,
	ga32kHz					=       0x0040,
	gaImage					=       0x0080,
	
	gaStereo				=       0x0100,
	ga16Bit					=       0x0200,
	
	gaDropFrame				=       0x0400,
	
	gaNonLinear				=       0x0800,
	
	gaTwosComp				=       0x1000,

	gaCache					=       0x8000,

	gaFlags					=       0x0BFF,
	gaRate					=       0x00FF,
	gaTakeLeft				=		0x2000,
	gaTakeRight 			=		0x4000
};


// Filter plug-in entry points
typedef PREFFECTENTRY (*AudioFilterProcPtr)(short selector, AudioHandle theData);
typedef PREFFECTENTRY (*VideoFilterProcPtr)(short selector, VideoHandle theData);


#include "prResetEnv.h"
#endif /* __PRPEFFECT_H */
