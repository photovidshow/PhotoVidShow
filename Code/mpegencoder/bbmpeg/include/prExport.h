//========================================================================================
//
// prExport.h
//
// Part of the Adobe Premiereª 5 Plug-In Developer's Toolkit.
//
// Defines the Premiere EDL Export interface. Note - the file export interface, which
// use to be contained in this header, has been removed. If you're interested in writing 
// a file exporter, you should now use the new compile module interface.
//
// Copyright © 1992-99, Adobe Systems Incorporated, all rights reserved worldwide.
//
//========================================================================================

#ifndef __PREXPORT_H
#define __PREXPORT_H

#include "prSetEnv.h"
#include "prTypes.h"
#include "prplugsuites.h"

#define PREMPLUGENTRY short 


//========================================================================================
// EDL defined block types
//========================================================================================

enum {
//			Type			ID		Parent		Data								Description
//			----------		------ 	----------	----------------------------------	--------------------------------
	bBLOK =	'BLOK',		//	0		none		(L)wrk strt, (L)wrk end, sub-blks	Container for everything

	bTRKB =	'TRKB',		//	0		BLOK		track blocks						Container for all of the tracks
	bTRAK =	'TRAK',		//	ID		TRKB		(S)flags, TREC blocks				Contains all of the blocks for an entire track
	bFVID =	'FVID',		//	0		TRAK		none								indicates that a track contains video records
	bFSUP =	'FSUP',		//	0		TRAK		none								indicates that a track contains superimpose records
	bFAUD =	'FAUD',		//	0		TRAK		none								indicates that a track contains audio records
	bAMAP = 'AMAP',		//	0		FAUD		(S)mapping bits						bits indicate target tracks
	bFF_X =	'FF_X',		//	0		TRAK		none								indicates that a track contains F/X records
	bTREC =	'TREC',		//	n		TRAK		(S)clipID,(L)strt,(L)end,sub-blocks	Contains the blocks for a single track item
	bRBND =	'RBND',		//	0		TREC		(S)max, RPNT blocks					[optional] The rubber band info for a track item
	bPBND =	'PBND',		//	0		TREC		(S)max, RPNT blocks					[optional] The pan band info for a track item
	bRPNT =	'RPNT',		//	0-n		RBND,PBND	(L)h, (S)v							rubber band point
	bFXOP =	'FXOP',		//	0		TREC		(C)crnr,(C)dir,(S)strt,(S)end,blks	[optional] The options controlling F/X options
	bFXDF =	'FXDF',		//	0		FXOP		OSType								the base type of the effect
	bEDGE =	'EDGE',		//	0		FXOP		(S)thickness, COLR block			[optional] describes edge thickness
	bMPNT =	'MPNT',		//	0		FXOP		Point								[optional] reference point for next to types
	bSPNT =	'SPNT',		//	0		FXOP		Point								[optional] user specified open point
	bEPNT =	'EPNT',		//	0		FXOP		Point								[optional] user specified close point
	bOVER =	'OVER',		//	0		TREC		(S)type, info blocks				[optional] The parameters for an overlay item
	bCOLR =	'COLR',		//	0		OVER,FILE	RGBColor							[optional] key or fill color
	bSIMI =	'SIMI',		//	0		OVER		(S)similarity						[optional] similarity value
	bBLND =	'BLND',		//	0		OVER		(S)blend							[optional] blend value
	bTHRS =	'THRS',		//	0		OVER		(S)threshold						[optional] threshold value
	bCUTO =	'CUTO',		//	0		OVER		(S)cutoff							[optional] cutoff value
	bALIA =	'ALIA',		//	0		OVER		(S)level							[optional] anti-aliasing level
	bSHAD =	'SHAD',		//	0		OVER		none								[optional] if present, shadowing is on
	bRVRS =	'RVRS',		//	0		OVER		none								[optional] if present, key is reversed
	bGARB =	'GARB',		//	0		OVER		(R)ref rect,point blocks			garbage matte points
	bPONT =	'PONT',		//	0-n		GARB,RBND	Point
	bMATI =	'MATI',		//	0		OVER		(S)clipID							[optional] The ID of the clip describing an overlay Matte
	bVFLT =	'VFLT',		//	0		TREC		sub-blocks							[optional] followed by individual filter blocks
	bAFLT =	'AFLT',		//	0		TREC		sub-blocks							[optional] followed by individual filter blocks
	bFILT =	'FILT',		//	0-n		VFLT,AFLT	(S)fileID,data block				The (short) fileID, followed by a variable amount of data
	bMOTN =	'MOTN',		//	0		TREC		(R)ref rect,sub blocks				[optional] A record describing the motion path for a track item
	bSMTH =	'SMTH',		//	0		MOTN		none								If present, motion path is smoothed
	bMREC =	'MREC',		//	0-n		MOTN		(S)zoom,(P)spot,(P)dest[4]			describes each motion point
	bDATA =	'DATA',		//	0		any			data block							[optional]generic data block, for storing parameter handles

	bCLPB =	'CLPB',		//	0		BLOK		clip blocks							Contains all of the clip blocks
	bCLIP =	'CLIP',		//	ID		CLPB		(S)fileID,(L)in,(L)out				the descriptive info for a clip
	bMARK =	'MARK',		//	0-9		CLIP		(L)location							[optional] For set markers, defines the markers
	bLOCK =	'LOCK',		//	0		CLIP		none								[optional] If present, clip has locked aspect
	bRATE =	'RATE',		//	0		CLIP		(S)rate * 100						[optional] Defines a rate other than 1.00

	bFILB =	'FILB',		//	0		BLOK		file blocks							Contains all of the file blocks
	bFILE =	'FILE',		//	ID		FILB		info blocks							the descriptive blocks for a file
	bMACS =	'MACS',		//	0		FILE		FSSpec								the mac file spec
	bMACP =	'MACP',		//	0		FILE		string								the full mac pathname
	bFRMS =	'FRMS',		//	0		FILE		(L)#frames							[optional] number of frames for a file w/content
	bVIDI =	'VIDI',		//	0		FILE		(L)video frame,(S)depth				[optional] Describes the video portion of the file
	bAUDI =	'AUDI',		//	0		FILE		(S)aud flags,(L)aud rate			[optional] Describes the audio portion of the file
	bTIMC =	'TIMC',		//	0		FILE		timecode							[optional] Specifies the timecode for the first file frame
	bTIMB = 'TIMB',		//	0		FILE		(L)frame,(C)dropframe,(C)format		[optional] Specifies the binary timecode, as above
	bREEL =	'REEL',		//	0		FILE		(STR)reel name						[optional] String describing the source reel for the file
	bDOSF =	'DOSF'		//  0		FILE		string								the full DOS file and path
};


// Header definition for data blocks
//
// Blocks passed to an EDL export module have a header followed by any
// static data, then any sub-blocks, which themselves have
// headers, static data, and sub-blocks...

typedef struct {
	long			size;				// total size of this block, including static data and sub-blocks
	long			dataSize;			// the static data size for this block
	long			type;				// the block type (basically, an OSType)
	long			theID;				// the block ID, for blocks which do not need an ID, 0 is used
} BlockRec;


// These are the structures for the static data for several of the blocks

typedef struct {
	long			start;				// starting position for the work area
	long			end;				// ending position for the work area
	long			zero;				// beginning time code for timeline
	long			tlDisplay;			// timeline display mode, one of the above
} Rec_BLOK;

typedef struct {
	PrFileID		fileID;				// the dependent file ID
	long			in;					// the IN point within the source material
	long			out;				// the OUT point within the source material minus 1
} Rec_CLIP;

typedef struct {
	PrClipID		clipID;				// the dependent clip ID
	long			start;				// the clip starting position
	long			end;				// the clip ending position
} Rec_TREC;

typedef struct {
	short			zoom;				// zoom factor 1 to 400, 100 is normal
	short			time;				// time location 1 to 1000
	short			rotation;			// rotation factor -360 to 360, 0 is normal
	short			delay;				// delay factor 0 to 100, 0 is normal
	prPoint			spot;				// the center point for the image at this point
} Rec_MREC;

typedef struct {
	unsigned char	corners;			// the 'corner' flags, one bit each, from the user settings for the effect
	char			direction;			// the direction flag, 0= A-->B, 1=B-->A
	short			startPercent;		// starting percentage times 100
	short			endPercent;			// ending percentage times 100
} Rec_FXOP;

typedef struct {
	long			h;
	short			v;
	short			value;				//	The actual level
} Rec_RPNT;

typedef struct {
	prRect			frame;				// bounding frame for video data
	short			depth;				// bit depth for video data
} Rec_VIDI;

typedef struct {
	long	frames;						// binary frame count
	char	dropframe;					// true = DF, false = NDF
	char 	format;						// true = NTSC(30), false = PAL(25), 2=Film(24)
} Rec_TIMB;


// These are the basic effect types, each effect falls back to one of these
enum {
	fDISS =	'DISS',			// cross dissolve
	fTAKE =	'TAKE',			// 'take' or cut
	fWI00 = 'WI00',			// vertical wipe from the left edge
	fWI01 = 'WI01',			// horizontal wipe from the top edge
	fWI02 = 'WI02',			// vertical wipe from the right edge
	fWI03 = 'WI03',			// horizontal wipe from the bottom edge
	fWI04 = 'WI04',			// diagonal wipe from upper left corner
	fWI05 = 'WI05',			// diagonal wipe from upper right corner
	fWI06 = 'WI06',			// diagonal wipe from lower right corner
	fWI07 = 'WI07',			// diagonal wipe from lower left corner
	fWI08 = 'WI08',			// vertical split wipe
	fWI09 = 'WI09',			// horizontal split wipe
	fWI10 = 'WI10',			// horizontal/vertical split wipe
	fWI11 = 'WI11',			// box wipe out from the center
	fWI12 =	'WI12',			// circular wipe from the center
	fWI13 = 'WI13',			// inset wipe from upper left
	fWI14 = 'WI14',			// inset wipe from upper right
	fWI15 = 'WI15',			// inset wipe from lower right
	fWI16 = 'WI16'			// inset wipe from lower left
};


//---------------------------
// EDL Export Data Structure

typedef struct {
	Handle					dataHandle;			// data handle
	short					timeBase;			// current default timebase
	StringPtr				projectName;		// pointer to current project name
	piSuitesPtr				piSuites;			// pointer to plug-in suites functions
} ExportRecord, **ExportHandle;


// Main plug-in entry point
typedef PREMPLUGENTRY (*ExportProcPtr)(short selector, ExportHandle theData);


// Selector messages
enum {
	exExecute = 0,
	exTrue30fps
};

enum {
	aflag5KHz =			0x0001,
	aflag11KHz =		0x0002,
	aflag22KHz =		0x0004,
	aflag44KHz =		0x0008,
	aflagSpecial =		0x0040,
	aflagStereo =		0x0100,
	aflag16Bit =		0x0200,
	aflagDropFrame =	0x0400
};


// file spec for open
typedef struct
{
	short	volID;			// used on Mac only
	long	parID;			// used on Mac only
	char	name[256];		// file name on Mac, full path elsewhere
} exportFileSpec;


#include "prResetEnv.h"
#endif /* __PREXPORT_H */
