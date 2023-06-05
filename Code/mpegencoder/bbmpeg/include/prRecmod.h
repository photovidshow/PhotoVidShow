//========================================================================================
//
// prRecmod.h
//
// Part of the Adobe Premiereª 5 Plug-In Developer's Toolkit.
//
// Header file for Adobe Premiere recoding module plug-in. Note: Record (or capture)
// modules are Windows only!
//
// Copyright © 1992-99, Adobe Systems Incorporated, all rights reserved worldwide.
//
//========================================================================================

#ifndef	__PRRECORDMOD
#define __PRRECORDMOD

#include "prSetEnv.h"
#include "prClassData.h"
#include "prPlugMem.h"
#include "prPlugSuites.h"		// added in RECMOD_VERSION_2

#define PREMPLUGENTRY int

// Version defines for the version of the host api that the plug-in is expecting.
// This info is returned to the record module in rmStdParms.
#define RECMOD_VERSION_1		1
#define RECMOD_VERSION_2		2
#define RECMOD_VERSION			RECMOD_VERSION_2


// errors that can be returned from record functions:

enum {
	rmNoErr = 0,		// no error
	rmUnsupported,		// unsupported entry selector
	rmErrAudRecErr,		// general audio recording error
	rmErrVidRecErr,		// general video recording error
	rmErrVidDataErr,	// data rate too high to record (return this if too many frames get dropped)
	rmErrDriverErr,		// general driver error
	rmErrMemErr,		// memory error occurred
	rmErrDiskFullErr,	// disk full when recording
	rmErrDriverNotFound,// can't connect to capture driver
	rmStatusCapDone,	// returned from recmod_StartRecord if cap module completes recording and no error occured
	rmCapLimitReached,	// returned from recmod_ServiceRecord if the cap module is limiting capture timing itself 
						// and has hit the recordlimit time.
	rmBadFormatIndex,	// Invalid format index - used to stop rmGetIndAudFormat queries
	rmFormatAccept,		// The output format is valid
	rmFormatDecline,	// The module cannot capture to this format
	rmErrorPrerollAbort,// the preroll function aborted.
	rmUserAbort,		// error returned from recmodStartRecord if cap completes and user aborted - added in RECMOD_VERSION_2
	rmErrFileSizeLimitErr,// error returned from recmod_ServiceRecord if file size limit was reached - added in RECMOD_VERSION_2
	rmFramesDropped,	//  error returned from dropped frames - added in RECMOD_VERSION_2
	rmErrBadFile = -1
};


// Callback functions
//-------------------

// Status function
// The module should call this function while capturing to return status.
// If the module controls the capture, not returning from a recmodStartRecord
// call until capture is complete, it can determine if capturing should be
// aborted by checking the return value from this function; The host returns TRUE 
// if capture should be halted.
typedef int (*StatusDispFunc)(long callbackID, char *stattext, int framenum);

// Preroll function for capturing with device control
// This function MUST be called just before beginning a capture.
// Host returns TRUE if capture should be halted
// recmodID is the value passed in recCapParmsRec
typedef int (*PrerollFunc)(long callbackID);


// Audio settings record used by the audio recording services:

typedef struct {
	long			capAudRate;				// 0:11 khz, 1:22 khz, 2:44 khz
	short			capAudStereo;			// 0:mono, 1:stereo
	short			capAud16;				// 0:8 bit samples, 1:16 bit samples
	char			AudCompRec[512];		// compressed audio record (WAVEFORMATEX)
} AudioRec;


// Utility functions passed in with the Standard Parms
typedef struct
{
	ClassDataFuncsPtr	classFuncs;				// class functions
	PlugMemoryFuncsPtr	memoryFuncs;			// memory functions
} recCallbackFuncs;


// Standard parms
typedef struct
{
	int					rmInterfaceVer;		// version # of record interface (RECMOD_VERSION)
	recCallbackFuncs	*funcs;
	piSuitesPtr			piSuites;
} rmStdParms;


#define kRecMaxSetups		4

// struct for setup item info
typedef struct {
	char	name[256];
} recSetupItem;

// struct filled in by compInit message
// These are the capabilities/info for the record module 
typedef struct {
	long			recmodID;			// Runtime ID for the module - don't change this!
	long			fileType;			// The file type (AVI, MOOV etc)
										// On windows, how is this matched up to extensions?
	long			classID;			// The classID for the module
										// This is used to differentiate between compile modules that
										// support the same fileType and to cross reference between
										// different plugin types (i.e. play and record modules).
	int				canVideoCap;			// can capture video
	int				canAudioCap;			// can capture audio (*and* audio is available!)
	int				canStepCap;				// can capture async frames to a file on command
	int				canStillCap;			// can capture a still and return as a buffer
	int				canRecordLimit;			// accepts recording time limits
	int				acceptsTimebase;		// can capture to an arbitrary timebase
	int				acceptsBounds;			// can capture to an arbitrary size
	int				multipleFiles;			// may capture to multiple files or external drives
	int				canSeparateVidAud;		// can capture video & audio to different system drives
	int				canPreview;				// can display continuous preview frames
	int				wantsEvents;			// capture module wants to process messages
	int				wantsMenuInactivate;	// capture module wants to an inactivation when a menu goes down
	int				acceptsAudioSettings;	// can accept audio settings from the host. If this is false, record module does its own settings.
	int				canCountFrames;			// can count frames and quit when count is reached - added in RECMOD_VERSION_2
	int				canAbortDropped;		// can abort when frames are dropped - added in RECMOD_VERSION_2
	int				reserved[18];			// reserved capabilities
											// capture bounds limits; ignored if acceptsBounds != true 
	int				activeDuringSetup;		// don't deactivate the record module when before a recmodSetup selector.
	long			prefTimescale;			// preferred timebase to capture to (if acceptsTimebase was true)
	long			prefSamplesize;			// preferred dividend of timebase (if acceptsTimebase was true)
	long			minWidth;				// Minimum width (set min = max to allow only one size)
	long			minHeight;				// Mininum height
	long			maxWidth;				// Maximum width
	long			maxHeight;				// Maximum height
	int				prefAspect;				// 16.16: aspect of source video (4.3, 45.30 [720x486])
	long			prefPreviewWidth;		// onscreen width we prefer to display at
	long			prefPreviewHeight;		// onscreen width we prefer to display at
	char			recmodName[256];		// The displayable name for this module
	long			audioOnlyFileType;		// file type to create for audio-only captures.If 0, then type will be same as video type.
} recInfoRec, *recInfoPtr;


typedef struct {
	int				customSetups;		// number of custom setup items ( < kRecMaxSetups)
	long			enableflags;		// flags for which setups are available (flag = 1 << setupnum)
	recSetupItem	setups[kRecMaxSetups];
} recCapSetups;

//	Audio format support

#define aud8Mono		0x000000001
#define aud8Stereo		0x000000002
#define	aud16Mono		0x000000004
#define aud16Stereo		0x000000008
#define audAllFormats	(aud8Mono | aud8Stereo | aud16Mono | aud16Stereo)

#define audUncompressed	'RAW '
#define audBadFormat	0xffffffff


//  audio format support
//	Structure for the audio formats an audio compressor supports
//	ie 16bit mono @ 44100
typedef struct
{
	int				audioDepths;				// audio format support bit field (use audio format support #defines)
	long			audioRate;					//	Audio rates the compressor supports
} recAudioFormat, *recAudioFormatPtr;

typedef struct
{
	long				recmodID;			// ID for the module - don't change this!
	long				subtype;			// File subtype supported. Generally a compressionID, but not always.
											// subtype==compUncompressed is a special case for "no compression"
											// subtype==compBadFormat is an invalid format
	char				name[256];			// The displayable name for this subtype
	recAudioFormatPtr	audioFormats;		// A ptr to a list of audio formats that the compressor supports
											// if this param is nil, it supports any rates and bits
} recAudioInfoRec;


// struct that describes playback position on screen
typedef struct {
	long		wind;		// window to display into (HWND on PC, WindowPtr on Mac
	int			originTop;	// offset in pixels from top of window to display to
	int			originLeft;	// offset in pixels from top of window to display to
	int			dispWidth;	// width of display area
	int			dispHeight;	// height of display area
	int			mustresize;	// the display must fit into these bounds; see note in recmod_SetDisp
} recDisplayPos;


// playmod_Open struct
typedef struct {
	recDisplayPos	disp;				// the display area
	long			callbackID;			// instance of this open recording session. Used for callback functions
	char			*setup;				// private record module settings from previous session (or nil if none)
} recOpenParms;


// fix me: move to common header file
// file spec for open
typedef struct
{
	short	volID;			// used on Mac only
	long	parID;			// used on Mac only
	char	name[256];		// file name on Mac, full path elsewhere
} recFileSpec;

typedef struct {
	long	parentwind;
	int		setupnum;
	char	*setup;
} recSetupParms;

// capture params, passed in recmod_PrepRecord, recmod_QueryFormat
typedef struct {
	long				callbackID;				// must be passed back with statFunc, prerollFunc callbacks
	int					stepcapture;			// capture is a step capture (0 = streaming capture)
	int					capVideo;				// capture video
	int					capAudio;				// capture audio 
	int					width;					// width to capture (if acceptsBounds was true)
	int					height;					// height to capture (ditto)
												// timebase: timescale/samplesize == fps, e.g 2997/100
	long				timescale;				// timebase to capture to (if acceptsTimebase was true)
	long				samplesize;				// dividend of timebase (if acceptsTimebase was true)
	long				audSubtype;				// compressed audio format to capture (or audUncompressed)
	unsigned long		audrate;				// The audio rate in samples per second.
	int					audsamplesize;			// The samplesize, 8 or 16 (this a number not a flag)
	int					stereo;					// If 1, audio is stereo, othewise it's mono
	char				*setup;					// pointer to setup storage created by setup calls
	int					abortondrops;			// stop capturing if any frames get dropped
	int					recordlimit;			// recording limit in seconds (if canRecordLimit was true)
	recFileSpec			thefile;				// file to capture to (valid on recmod_PrepRecord only)
	StatusDispFunc		statFunc;				// function pointer used to return capture status (streaming only)
	PrerollFunc			prerollFunc;			// function to call _just_ before capture begins; used for device 
												// control preroll (streaming only)
	long				frameCount;				// if module sets canCountFrames, frameCount is # of frames to 
												// capture and no device polling is done - added in RECMOD_VERSION_2
	char				reportDrops;			// if true, report dropped frames - added in RECMOD_VERSION_2
	short				currate;				// the fps of the deck - 0, 25, or 24 - added in RECMOD_VERSION_2
} recCapParmsRec;

typedef struct {
	int					width;					// width to capture
	int					height;					// height to capture
	int					depth;					// returned depth, 24 or 32bpp allowed
	char				*pixels;				// pixels captured at depth reported, buffer allocated with memfuncs
	int					rowbytes;				// rowbytes of captured data
} recStillCapParmsRec;

//	Platform event definition
#ifdef PRWIN_ENV
typedef MSG rmPlatformEvent;
#else
typedef EventRecord		rmPlatformEvent;
#endif

// selectors for record module entry points:
enum {
	recmod_Startup,
	recmod_Shutdown,
	recmod_Open,
	recmod_Close,
	recmod_PrepRecord,
	recmod_StartRecord,
	recmod_ServiceRecord,
	recmod_StopRecord,
	recmod_CloseRecord,
	recmod_StepRecord,
	recmod_StillRecord,
	recmod_ShowOptions,
	recmod_SetActive,
	recmod_GetAudioIndFormat,
	recmod_Idle,
	recmod_SetDisp,
	recmod_DisplayFrame,
	recmod_QueryFormat,
	recmod_QueryDisplayPos,
	recmod_GetSetupInfo,
	recmod_ProcessEvent
};

typedef PREMPLUGENTRY (* RecordEntryFunc)
	(int selector, rmStdParms *stdparms, long param1, long param2);


#include "prResetEnv.h"
#endif // _PRRECORDMOD
