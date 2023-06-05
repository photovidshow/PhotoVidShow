//========================================================================================
//
// prImport.h
//
// Part of the Adobe Premiereª 5 Plug-In Developer's Toolkit.
//
// Defines the Premiere Importer interfaces. The can be used to import a real file or
// generate a clip on the fly, known as a synthetic importer.
//
// Copyright © 1992-99, Adobe Systems Incorporated, all rights reserved worldwide.
//
//========================================================================================

#ifndef __PRIMPORT
#define __PRIMPORT

#define PREMPLUGENTRY int

#include "prSetEnv.h"
#include "prClassData.h"
#include "prPlugSuites.h"

// Version defines for the version of the host api that the plug-in is expecting.
// This info is returned to the import module in imStdParms.
#define IMPORTMOD_VERSION_1		1		// Premiere 5.0
#define IMPORTMOD_VERSION_2		2		// Premiere 5.1
#define IMPORTMOD_VERSION		IMPORTMOD_VERSION_2

// Platform file reference
#ifdef PRWIN_ENV
#define imFileRef HANDLE
#endif

#ifdef PRMAC_ENV
#define imFileRef short
#endif


// file spec 
typedef struct
{
	short	vRefNum;		// used on Mac only
	long	parID;			// used on Mac only
	char	name[256];		// file name on Mac, full path elsewhere
} imFileSpec;

// callbacks for opening/closing files
typedef long (*OpenFileFunc)(long playID);
typedef void (*ReleaseFileFunc)(long fileref);

// Utility functions passed in with the Standard Parms
typedef struct
{
	ClassDataFuncsPtr	classFuncs;
	OpenFileFunc		openFile;
	ReleaseFileFunc		releaseFile;
} imCallbackFuncs;

// Standard parms passed into xImportEntry
typedef struct
{
	int				imInterfaceVer;		// version # of import interface (IMPORTMOD_VERSION)
	imCallbackFuncs	*funcs;
	piSuitesPtr		piSuites;			// present if imInterfaceVer >= IMPORTMOD_VERSION_2
} imStdParms;

//--------------------------------------
// Flags for the xRef resource

#define xfCanOpen               0x8000
#define xfCanImport             0x4000
#define xfCanReplace			0x2000
#define xfUsesFiles             0x1000

#define xfCanWriteTimecode		0x0200	// Used by importers to save off the canWriteTimecode flag

#define xfIsTitle               0x0100
#define xfIsEffect              0x0080
#define xfIsAFilter             0x0040
#define xfIsVFilter             0x0020
#define xfIsStill               0x0010
#define xfIsMovie               0x0008
#define xfIsSound               0x0004
#define xfIsAnimation			0x0002
#define xfIsBackdrop			0x0001

#define xfTypes                 0x03FF


// struct filled in by imImportInit message

typedef struct
{
	unsigned long	importerType;		// identifier for the module
	int				canOpen;			// importer handles open/close
	int				canSave;			// importer handles "save as"
	int				canDelete;			// importer handles deleting its own files (probably because there are child files)
	int				canResize;			// importer can do resizing
	int				canDoSubsize;		// importer can rasterize a subset of an image at any size
	int				canDoContinuousTime;// importer can render frames at arbitrary times (interpolate or
										// generate synthetically). If set, the importer may be called
										// to generate fields.
	int				noFile;				// no file reference, the importer generates the image
	int				addToMenu;			// add this importer to a premiere menu (see enum)
	int				hasSetup;			// importer has a setup dialog
	int				dontCache;			// Frames should not be cached
	int				setupOnDblClk;		// If true, the setup dialog will be opened on a double click

	int				keepLoaded;			// If true, the importer plugin will never be unloaded.
										// Normally, importers are loaded and unloaded on an as needed basis
										// to minimize system load. With this flag set, an importer will never
										// be released until the app shuts down. Don't set this flag unless it's
										// really necessary.
	
	long			priority;			// New for 5.1RT LL981112
										// Used to determine priority levels for importers that handle the
										// same filetype.
										// Currently only 2 priority levels are recognized: >0 means an override
										// ==0 is a "base" importer.
										// NOTE: importers with priority>0 MUST also set canOpen.
	
	long			reserved[31];		// reserved; set to nil;
} imImportInfoRec;


// color record for alphaArbitrary matte color
typedef struct
{
	unsigned char	matteRed;
	unsigned char	matteBlue;
	unsigned char	matteGreen;
} matteColRec;


// alpha channel types for alphaType field:
enum
{
	alphaUnknown = 0,	// has alpha channel, but we don't have any other information
	alphaStraight,		// straight alpha channel
	alphaBlackMatte,	// alpha premultiplied black
	alphaWhiteMatte,	// alpha premultiplied white
	alphaArbitrary,		// alpha premultiplied with an arbitrary color
	alphaNone			// no alpha channel
};

// field types for fieldType enum
enum
{
	fieldsNone,
	fieldsUnknown,		// has fields, unknown dominance
	fieldsUpperFirst,	// topmost line is dominant
	fieldsLowerFirst	// topmost+1 line is dominant
};

enum
{
	imMenuNone = 0,
	imMenuNew
};

// values for framerate in imTimeCode
enum
{
	imPAL = 0,			
	imNTSC,
	imFILM
};

#define imUncompressed		'RAW '				// Special subType for uncompressed files
#define imSubTypeUnknown	0xffffffff			// Special subType for files which can't report a subtype.

typedef struct
{
// Image bounds info:
	long			imageWidth;		// frame width in pixels
	long			imageHeight;	// frame height in pixels
	unsigned short	pixelAspect;	// aspect of pixels x:y, e.g. D1 == 648:720 
	short			depth;			// bits per pixel - image buffers are 32bpp so this is informational only

	long			subType;		// The subType for the file.
									// This is generally the fourcc of the codec for the file
									// though not always. It's used to match files to fileType's and subTypes
									// that compile modules support.
									// For uncompressed files, this should be set to imUncompressed.
// Image format info:
	char			fieldType;		// field type -- see enum
	char			fieldsStacked;	// fields present, not interlaced
	char			hasPulldown;	// file has ntsc film 3:2 pulldown
	char			pulldownPhase;	// phase of first frame: 0-4 == a-e
	char			alphaType;		// alpha channel type -- see enum
	matteColRec		matteColor;		// color of alpha matte for alphaArbitrary type
	char			alphaInverted;	// alpha is inverted from normal
	char			isVectors;		// content has no inherent resolution

// Image draw info:
	char			drawsExternal;	// file draws only externally (ntsc/pal out)
	char			canForceInternalDraw;	// file draws only externally BUT we'll respect
	char			dontObscure;	// file plays external and internal, don't obscure onscreen drawing (T2K)

// Image time info:
	char			isStill;		// File contains a single frame. Only one frame will be cached.
	char			noDuration;		// File has no intrinsic duration so it can be stretched. Usually used for synthetic images
	char			isFramestore;	// no innate timebase (numbered stills or other framestore).
	long			reserved[32];	// reserved; set to nil
} imImageInfoRec;

// struct filled in at imFileInfo time for audio

typedef struct
{
	char			audStereo;		// 0:mono, 1:stereo
	char			aud16;			// 0:8 bit samples, 1:16 bit samples
	char			twosComp;		// 0:zero based, 1:twosComp
	long			subType;		// The audio subType for the file.
									// This is generally the fourcc of the codec for the file
									// though not always
									// For uncompressed audio, this should be set to imUncompressed.
	long			reserved[32];	// reserved; set to nil
} imAudioInfoRec;

// timebase/format struct passed to/from importer
typedef struct
{
	long	timebase;			// timebase
	long	samplesize;			// size of one sample
	long	duration;			// duration in samples
} imTimebaseRec;

// This is the struct filled in by an importer during imGetInfo
// It contains attributes for the file being imported.
typedef struct
{
	char			hasVideo;			// The file contains video
	char			hasAudio;			// The file contains audio

	imImageInfoRec	vidInfo;			// Information about the video data
	long			vidScale;			// vidScale/vidSampleSize is the preferred video timebase
	long			vidSampleSize;		// vidScale/vidSampleSize is the preferred video timebase
	long			vidDuration;		// The duration of the video in the video timebase
	char			canDraw;			// importer can draw onscreen

	imAudioInfoRec	audInfo;			// Information about the audio data
	long			audScale;			// audio sample rate
	long			audSampleSize;		// audScale/audSampleSize is the preferred audio timebase
										// Note: the real audio sample size (8 or 16 bit) is in imAudioInfoRec
	long			audDuration;		// The duration of the audio in the audio timebase
	
	long			privatedata;		// this is private instance data
	void *			prefs;				// this is persistent settings data
	char			hasDataRate;		// importer can generate datarate information for this file.
} imFileInfoRec;


// This the struct passed to an importer during imOpenFile, imQuiet
typedef struct
{
	long		importID;
	long		filetype;
	imFileSpec	filespec;
	imFileRef	fileref;
} imFileAccessRec;


// This the struct passed to an importer during imOpenFile, imQuiet
typedef struct
{
	imFileAccessRec	fileinfo;
	long			privatedata;
} imFileOpenRec;

// Onscreen image record
typedef struct
{
#ifdef PRWIN_ENV
	HWND		wnd;
	HDC			dc;
#endif
#ifdef PRMAC_ENV
	GrafPtr		port;
#endif
} DrawRec;

// flags passed during imImportImage
enum
{
	imThumbnailRender=1,		// we're requesting thumbnails; do quick&dirty resizes
	imScrubbing=2,				// user is scrubbing, playing back; stay responsive
	imForceInternal=4,			// For an importer that only draws external, force an internal
								// (computer monitor) draw and DONT draw external. 
								// Only set if canForceInternalDraw in imageInfoRec is set. 
								// If the importer draw external and this flag is NOT set, then 
								// 'force internal' situations (such as trimming, slipping, sliding) 
								// will only call the importer for offscreen drawing.

	imDraftMode=8				// Draw quickly if possible. This is passed when scrubbing and playing
								// from the timeline.
};

// field types for fieldType 
enum
{
	imFieldsNone=0,			// Full frame
	imFieldsUpperFirst,		// topmost line is dominant
	imFieldsLowerFirst		// topmost+1 line is dominant
};

// struct passed during imImportImage message
typedef struct
{
	int				onscreen;		// non-zero means we're drawing on screen
	int				dstWidth;		// destination width
	int				dstHeight;		// destination height
	int				dstOriginX;		// destination origin X (always 0 for offscreen)
	int				dstOriginY;		// destination origin Y (always 0 for offscreen)
	int				srcWidth;		// source width if subsampling
	int				srcHeight;		// source height if subsampling
	int				srcOriginX;		// source origin X if subsampling
	int				srcOriginY;		// source origin Y if subsampling
	DrawRec			drawinfo;		// onscreen info
	int				rowbytes;		// rowbytes;
	char *			pix;			// destination bits
	long			pixsize;		// number of bytes in "pix"
	int				pixdepth;		// depth of the pix (why do we need this?)
	long			flags;			// special flags, see enum above

	long			fieldType;		// If the importer can swap fields, it should render the frame with
									// the given field dominance (imFieldsUpperFirst/LowerFirst.
	
	long			scale;			// scale/sampleSize is the timebase
	long			sampleSize;		// scale/sampleSize is the timebase

									// The following are based on the timebase given by scale and sampleSize
	long			in;				// in point 
	long			out;			// out point
	long			pos;			// importTime position

	long			privatedata;	// instance data from imGetInfo
	void *			prefs;			// persistent data from imGetSettings
} imImportImageRec;


// struct passed during imImportAudio message
typedef struct
{
	long	in;						// in point 
	long	out;					// out point
	long	sample;					// current sample
	long	size;					// number of bytes to import
	char	*buffer;				// audio buffer
	long	privatedata;			// instance data from imGetInfo
	void	*prefs;					// persistent data from imGetSettings
} imImportAudioRec;


// timecode struct for imTimeInfoRec
typedef struct
{
	long	in;						// timecode in frames
	char	dropframe;				// true = DF, false = NDF
	char	framerate;				// see enum
} imTimeCode;


// struct passed during imGetTimeInfo, filled in by plug-in
typedef struct
{
	long			privatedata;			// instance data from imGetInfo
	void *			prefs;					// persistent data from imGetSettings
	char			orgtime[18];
	char			alttime[18];
	char			orgreel[40];
	char			altreel[40];
	char			logcomment[256];	// the comment from the capture
} imTimeInfoRec;							

// struct passed during imAnalysis, filled in by plug-in
typedef struct
{
	long				privatedata;	// instance data from imGetInfo
	void				*prefs;			// persistent data from imGetPrefs
	unsigned int		buffersize;		// the size of the analysis buffer
	char				*buffer;		// analysis buffer to be filled with text
} imAnalysisRec;

// struct passed during imGetPrefs, filled in by plug-in
typedef struct
{
	char		*prefs;					// Buffer to store preferences
	long		prefsLength;			// If "prefs"==0, fill this in with the
										// buffer size needed for preferences
	char		firstTime;				// If 1, then imGetPrefs is being called for the first
										// time. The importer should setup reasonable default values for prefs.
} imGetPrefsRec;

// struct passed during imGetIndFormat
typedef struct {
	long	filetype;					// unique fourcc, e.g.: 'MooV'
	long	flags;						// defines file class (video, still, etc) and loading flags (can this file be in a project?, etc.)
	long	canWriteTimecode;			// Set to 1 if timecode can be written for this filetype.
	char	FormatName[256];			// long descriptive name
	char	FormatShortName[32];		// short name
	char	PlatformExtension[256];		// file extension(s). 
										// If multiple extensions, separate with nulls, e.g.:
										// "bmp\0dib"
} imIndFormatRec;


// struct passed during imGetFrameInfo and imGetCompressedFrame to tell the importer what frame
// is being checked
typedef struct
{
	imFileRef	fileref;				// The opened file
	long		privatedata;			// instance data from imGetInfo
	long		frame;					// what frame are we interested in? (frame, not time!)
} imGetFrameInfoRec;

// struct passed during imGetFrameInfo, filled in by the importer
typedef struct
{
	long	frameSize;					// the size in bytes of the frame
	int		deltaFlags;					// imKeyFrame, imDeltaFrame
	long	nearestKey;					// nearest earlier keyframe (frame, not time!)
	long	privateFrameInfo;			// Information specific to the importer 
} imFrameInfoRec;

// struct passed during imAnalysis, filled in by plug-in
typedef struct
{
	long				privatedata;	// instance data from imGetInfo
	void				*prefs;			// persistent data from imGetPrefs
	unsigned int		buffersize;		// the size of the analysis buffer
	char				*buffer;		// analysis buffer to be filled with imDataSamples
	long				baserate;		// base data rate per second of the file MINUS the data samples. 
										// This is usually the audio data rate.
} imDataRateAnalysisRec;


// one sample entry for the imDataRateAnalysis message
typedef struct
{
	unsigned long	sampledur;			// duration of one sample in vidTimebase.samplesize increments. Hi bit set if its a keyframe
	unsigned long	samplesize;			// size of this sample in bytes. 
} imDataSample;


typedef struct
{
	long			privatedata;		// instance data from imGetInfo
	long			*prefs;				// persistent data from imGetPrefs
	imFileSpec		sourceFile;			// fsspec for the source file
	imFileSpec		destFile;			// fsspec for the dest file
	char			move;				// true if it's a move operation
} imSaveFileRec;


typedef struct
{
	long			filetype;			// instance data from imGetInfo
	imFileSpec		deleteFile;			// fsspec for the file
} imDeleteFileRec;


typedef PREMPLUGENTRY (* ImportEntryFunc)(int selector, imStdParms *stdparms, long param1, long param2);

// importer messages:
enum
{
	imInit,
	imShutdown,
	imGetPrefs,
	imSetPrefs,
	imGetInfo,
	imImportImage,
	imImportAudio,
	imOpenFile,
	imQuietFile,
	imCloseFile,
	imGetTimeInfo,
	imSetTimeInfo,
	imAnalysis,
	imDataRateAnalysis,
	imGetIndFormat,
	imGetFrameInfo,
	imDisposeFrameInfo,
	imGetCompressedFrame,
	imDisposeCompressedFrame,
	imSaveFile,
	imDeleteFile
};

// Error Return codes

enum {
	imNoErr = 0,				// No Error
	imTooWide = 1,				// File dimensions to large
	imBadFile = 2,				// Bad file format
	imUnsupported = 3,			// Damaged or unsupported file
	imMemErr = 4,				// Memory Error
	imOtherErr = 5,				// Unknown Error
	imNoContent = 6,			// No audio or video
	imBadRate = 7,				// Bad Audio rate
	imBadCompression = 8,		// Bad compression
	imBadCodec = 9,				// Codec not found
	imNotFlat = 10,				// Unflattened QuickTime movie
	imBadSndComp = 11,			// Bad sound compression
	imNoTimecode = 12,			// Timecode supported, but not found
	imMissingComponent = 13,	// Missing component needed to open the file
	imSaveErr = 14,				// error saving file
	imDeleteErr = 15			// error deleting file
};

// Other Return codes

#define imCancel			300				// Returned from imGetPrefs
#define imBadFormatIndex	301				// Returned from imGetIndFormat


#include "prResetEnv.h"
#endif
