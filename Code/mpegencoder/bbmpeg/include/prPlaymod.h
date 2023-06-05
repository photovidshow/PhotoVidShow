//========================================================================================
//
// prPlaymod.h
//
// Part of the Adobe¨ Premiere¨ 5 Plug-In Developer's Toolkit.
//
// Defines the Premiere Playback from the timeline interface. There are actually 2 interfaces -
// single clip playback (plays back a clip at a time) and multiple clip playback using a cutlist.
// Version 2 of the interface adds interfaces for playing effects in real-time when using the RT
// version of Premiere.
//
// Copyright © 1992-99, Adobe Systems Incorporated, all rights reserved worldwide.
//
//========================================================================================

#ifndef	__PRPLAYMOD
#define __PRPLAYMOD

#include "prSetEnv.h"
#include "prClassData.h"
#include "prPlugSuites.h"

// Version defines for the version of the host api that the plug-in is expecting.
// This info is returned to the playmod in pmStdParms.
#define PLAYMOD_VERSION_1		1	// Premiere 5.0
#define PLAYMOD_VERSION_2		2	// Premiere 5.1 - adds realtime effects
#define PLAYMOD_VERSION			PLAYMOD_VERSION_2

#define PREMPLUGENTRY int


// file spec for open
typedef struct {
	int		volID;			// used on Mac only
	int		parID;			// used on Mac only
	char	name[256];		// file name on Mac, full path elsewhere
} pmFileSpec;


// Play preroll function
typedef int (* PlayPrerollFunc)(long playID);


// flags for audio callback functions
enum {
	audStereo = 0x100,
	aud16bit = 0x200
};

// forward decl
typedef struct pmDisplayPos pmDisplayPos;

// video functions
typedef void (*pmShowFileFrameFunc)(long playID, long frametime, pmDisplayPos *disp);
typedef void (*pmShowFileFrameProxyFunc)(long playID, long frametime, int proxywidth, int proxyheight, int dispwidth, int dispheight);

// audio functions
typedef int  (*pmGetFileAudioFunc)(long playID, long sec, char *buffer, unsigned int flags, long audrate);
typedef void (*pmAudioPlayPrepFunc)(long playID, long frametime, int rate, int volume);
typedef void (*pmAudioPlayFunc)(long playID, long frametime, int rate, int volume);
typedef void (*pmAudioPlayServiceFunc)(long playID);
typedef void (*pmAudioPlayFrameFunc)(long playID, long frametime, int direction, int volume);
typedef void (*pmAudioSetVolFunc)(long playID, int vol);
typedef int  (*pmEnumAudioFileFunc)(long playID, long index, pmFileSpec *spec, long *second, long *sec_duration);

// file functions
typedef long (*pmOpenFileFunc)(long playID);
typedef void (*pmReleaseFileFunc)(long fileref);


typedef struct {
	pmGetFileAudioFunc		getFileAudio;
	pmAudioPlayPrepFunc		audioPlayPrep;
	pmAudioPlayFunc			audioPlay;
	pmAudioPlayServiceFunc	audioPlayService;
	pmAudioPlayFrameFunc	audioPlayFrame;
	pmAudioSetVolFunc		audioSetVolume;
	pmEnumAudioFileFunc		enumAudioFile;
} AudioFuncs, *AudioFuncsPtr;

typedef struct {
	pmOpenFileFunc		openFile;
	pmReleaseFileFunc	releaseFile;
} FileFuncs, *FileFuncsPtr;

typedef struct {
	pmShowFileFrameFunc			showFileFrame;
	pmShowFileFrameProxyFunc	showFileFrameProxy;
} VideoFuncs, *VideoFuncsPtr;


// Utility functions passed during playmod_Startup
typedef struct {
	ClassDataFuncsPtr	classFuncs;
	FileFuncsPtr		fileFuncs;
	AudioFuncsPtr		audioFuncs;
	VideoFuncsPtr		videoFuncs;
} pmCallbackFuncs;


// errors
enum {
	playmod_ErrNone = 0,	// no error
	playmod_ErrBadFile,		// file is corrupt or unreadable
	playmod_ErrDriver,		// driver error (hardware failure)
	playmod_ErrNotPreferred,// incorrect file subtype
	playmod_BadFormatIndex,	// format index invalid (for GetIndFormat message)
	playmod_DeclinePlay,	// I decline to play this back; find another module (usually used for audio-only files)
	playmod_ListWrongType,	// can't play back this file -- wrong type; please render it.
	playmod_ListBadSpeed,	// can't play back this file at this speed;
	playmod_CantAddSegment, // the cutlist can't add a segment
	playmod_Unsupported,	// unsupported call
	playmod_AudioOverload,	// audio took more time to process than available
	playmod_OutOfRange,		// callback param out of range (pmEnumAudioFileFunc) - added in PLAYMOD_VERSION_2
	playmod_CannotRender,   // cannot render frame in real-time - added in PLAYMOD_VERSION_2
	playmod_RebuildCutlist  // return value used in playmod_GetFilePrefs - added in PLAYMOD_VERSION_2
};

// playback reference returned from playmod_Open, private to playback module
typedef long	pmPlayRef;


// struct that describes playback position on screen
struct pmDisplayPos{
	long		wind;		// window to display into (HWND on PC, WindowPtr on Mac
	int			originTop;	// offset in pixels from top of window to display to
	int			originLeft;	// offset in pixels from top of window to display to
	int			dispWidth;	// width of display area
	int			dispHeight;	// height of display area
};


// playmod_Play struct
typedef struct {
	long				inTime;			// in point -- only used for looping
	long				outTime;		// out point -- stop playing here or loop when this frame is reached
	long				startTime;		// start playing from here
	int					loop;			// true = loop until playmod_Stop is sent
	int					rate;			// -10000 to +10000 (NOTE: if rate < 0, inTime > outTime)
	int					volume;			// audio volume to play at
	PlayPrerollFunc		prerollFunc;	// preroll function to call before playback. Can be nil!
} pmPlayParms;


// mode types for pmPosRec
enum {
	playmode_Stopped,
	playmode_Playing
};


// playmod_GetPos struct
typedef struct {
	long				position;		// current position
	int					mode;			// current mode (see enum)
} pmGetPosRec;


// playmod_SetPos call struct
typedef struct {
	long	position;					// frame time to position to
	int		volume;						// audio volume for single-frame audio 'blip', 0-100
} pmSetPosRec;


// playmod_Startup struct
typedef struct {
	int				pmInterfaceVer;		// version # of playmodule interface (PLAYMOD_VERSION)
	pmCallbackFuncs	*funcs;
	piSuitesPtr		piSuites;			// added in PLAYMOD_VERSION_2
	void			*playmodPrefs;		// Private playback module data - added in PLAYMOD_VERSION_2
} pmStdParms;


// timebase struct
// Example: timeBase = 2997, samplesize = 100, fileDurations = numframes * 100
typedef struct {
	long	timeBase;					// timebase
	long	samplesize;					// size of one sample
	long	fileDuration;				// number of samples in file
} pmPlayTimebase;


// playmod_Open struct
typedef struct {
	pmDisplayPos	disp;				// the display area
	pmFileSpec		theFile;			// the file spec
	pmPlayTimebase	timebase;			// the timebase and duration of the file
	long			filetype;			// file type of this file
	long			playID;				// ID of this play instance. Used for callback functions
} pmPlayOpenParms;


// struct passed during playmod_Step message
typedef struct {
	long	position;					// frame time to start at. Filled with end time by play module
	int		direction;					// +1 or -1
	int		rate;						// % to scale frame stepping by, set by clip's speed
	int		volume;						// audio volume for single-frame audio 'blip', 0-100
} pmStepRec;


// playmod_ShuttleBegin struct
typedef struct {
	long	position;
	int		shuttlerate;					// -10000 to +10000, 100 == forward at normal speed
} pmShuttleStartRec;


// FILE info record filled in by playmod_GetInfo
typedef struct {
	int		width;					// -> 'native' file width
	int		height;					// -> 'native' file height
	int		hasVideo;				// <-> file has video
	int		hasAudio;				// <-> file has audio
	int		playsExternal;			// <-  video plays only externally, not on desktop (m100qx, pvr, dc20)
	int		playsProxy;				// <-  combine with playsExternal. Small proxy plays while full size goes to video out
	long	prefPreviewWidth;		// ->onscreen width we prefer to display at
	long	prefPreviewHeight;		// ->onscreen height we prefer to display at
} pmPlayInfoRec;


// Single file playback modules are prioritized and found in this order:
/*	filetype
		notallfiles			can only play some files of a given type
			hwsupport
			nohwsupport
		generic				can play any file of a given type
			hwsupport
			nohwsupport
	base generic			host module; plays anything.
*/


// priority flags; used for selecting correct playback module
enum {
	pmFlag_notAllFiles = 1,		// will only play some files of this type
	pmFlag_hwSupport = 2,		// uses special hardware for playback
	pmFlag_canPlayLists = 4		// can play lists of files
};


// Info rec filled in by module during playmod_GetIndFormat call
typedef struct {
	long	filetype;			// file type supported
	long	subtype;			// subfile type or zero
	long	classID;			// The classID for the module
								// This is used to differentiate between compile modules that
								// support the same fileType and to cross reference between
								// different plugin types (i.e. play and record modules).
	long	playflags;			// see enum above

// play list info - these fields were added in PLAYMOD_VERSION_2 for realtime effects
	int		listsCanRealTimeFX;	// Set to non-zero if playback module supports the real time API.
								// Will use the structs defined in prrt.h
	int		canRenderFrames;	// Set to non-zero if playback module would like
								// to accelerate rendering of frames
	int		hasSetup;			// If non-zero, the playback module has a private setup dialog
	
	int		reserved1;			// leave all the reserved fields to zero
	int		reserved2;
	int		reserved3;
	int		reserved4;
} pmModuleInfoRec;


// structure passed during playmod_listAddVid
typedef struct {
	int				black;				// 1 = no file, its blackness
	pmFileSpec		theFile;			// the file spec
	long			filetype;			// file type of this file
	pmPlayTimebase	fileTimebase;		// timebase of input file
	long			inListpos;			// in point in the play list
	long			outListpos;			// out point in the play list
	long			inFilepos;			// in point in the file - in the FILE'S timebase
	int				rate;				// FILE's playback rate, -10000 to +10000 
	float			fRate;				// FILE's playback rate as a float - added in PLAYMOD_VERSION_2
	long			privateData;		// private data from import module - added in PLAYMOD_VERSION_2
} pmVidSegRec;


// The following structures were added in PLAYMOD_VERSION_2

//	structure to hold information about a file that
//	is used in an effect
typedef struct
{
	pmFileSpec		fileSpec;			// Handle to a list of file specs
	long			filetype;			// file type of this file
	long			fileInPos;			// In point within the file
	long			fileOutPos;			// Out point within the file
} pmEffectFile;

typedef unsigned long	pmColor;

typedef struct
{
	long		x;						//	x coordinate
	long		y;						//	y coordinate
} pmPoint;


// structure passed during playmod_listAddEffect - this structure added in PLAYMOD_VERSION_2
typedef struct {
	long			effect;				// effect id - four char code
	pmPlayTimebase	**fileTimebases;	// timebase of input file
	long			inListpos;			// in point in the play list
	long			outListpos;			// out point in the play list
	int				rate;				// FILE's playback rate, -10000 to +10000 
	pmEffectFile	**theFiles;			// Handle to a list of effect files
	char			previewing;			// in preview mode?
	unsigned char	arrowFlags;			// flags for dirction arrows
	char			source;				// are the sources reversed?
	char			reverse;			// is the effect running in reverse
	char			edgeAlias;			// are the edges being anti-aliased? (0-2)
	short			startPercent;		// start percentage as defined in FX options
	short			endPercent;			// end percentage as defined in FX options
	short			edgeThick;			// edge thickness
	pmColor			edgeColor;			// color for the border
	pmPoint			startPoint;			// starting point
	pmPoint			endPoint;			// ending point
	pmPoint			centerPoint;		// center reference point
	char **			fxSpecs;			// special FX defined parameter (specsHandle)
} pmEffectSegRec;


// structure passed during playmod_Newlist
typedef struct {
	pmPlayTimebase	listTimebase;
	pmDisplayPos	disp;				// the display area
	long			playID;				// ID of this play instance. Used for callback functions
	int				stereo;				// audio is stereo?
	int				bit16;				// 16 bit?
	int				audioSpeed;			// audio rate
} pmNewListParms;


// This is the struct sent with pmGetFilePrefs - added in PLAYMOD_VERSION_2

typedef struct
{
	long	filetype;		// the requested filetype preferences
	long	subtype;		// the subtype
	long	classID;		// The classID for the module
	char	*playmodPrefs;	// Buffer to store preferences. If null, a buffer of the desired size must 
							// be allocated using the memory suite, otherwise, it's a previously allocated 
							// buffer that can be reused.
} pmGetFilePrefsRec;


// This is the struct sent with playmod_ActivateFile - added in PLAYMOD_VERSION_2
typedef struct
{
	long		activate;		// If 0, the file should be put in an inactivate state (closed)
	pmFileSpec	spec;			// The filespec.
} pmActivateFileRec;	


// Play Module entry point definition
typedef PREMPLUGENTRY (* PlayModEntryFunc)
	(int selector, pmStdParms *parms, long param1, long param2);

enum {
	playmod_Startup=1,
	playmod_Shutdown,
	playmod_Open,
	playmod_GetInfo,
	playmod_SetDisp,
	playmod_Update,
	playmod_SetPos,
	playmod_GetPos,
	playmod_Play,
	playmod_Stop,
	playmod_PlayIdle,
	playmod_Close,
	playmod_Activate,
	playmod_GetIndFormat,
	playmod_Step,
	playmod_SetVolume,
	playmod_ShuttleBegin,
	playmod_ShuttleIdle,
	playmod_ShuttleEnd,
	playmod_SetScrub,
	playmod_NewList,
	playmod_ListAddVid,
	playmod_ListAddEffect,
	playmod_ListReset,
	playmod_ListStartAdd,
	playmod_ListEndAdd,
	playmod_ShowProxy,
	playmod_SetPlayrate,
	playmod_DispMoving,
	playmod_Preroll,				// added in PLAYMOD_VERSION_2
	playmod_ListAddRTVideoSegment,	// added in PLAYMOD_VERSION_2
	playmod_GetPlayableRTRange,		// added in PLAYMOD_VERSION_2
	playmod_RenderRTFrame,			// added in PLAYMOD_VERSION_2
	playmod_GetFilePrefs,			// added in PLAYMOD_VERSION_2
	playmod_ActivateFile			// added in PLAYMOD_VERSION_2
};


#include "prResetEnv.h"
#endif /*__PRPLAYMOD */
