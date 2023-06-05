//========================================================================================
//
// prCompile.h
//
// Part of the Adobe Premiereª 5 Plug-In Developer's Toolkit.
//
// Defines the Premiere Plug-in Movie Compiler interface.
//
// Copyright © 1992-99, Adobe Systems Incorporated, all rights reserved worldwide.
//
//========================================================================================

#ifndef	__PRCOMPILEMOD
#define __PRCOMPILEMOD

#include "prSetEnv.h"
#include "prClassData.h"
#include "prPlugSuites.h"	// Added in COMPILEMOD_VERSION_2

// Version defines for the version of the host api that the plug-in is expecting.
// This info is returned to the compile module in compInfoRec and compStdParms.
#define COMPILEMOD_VERSION_1		1	// Premiere 5.0
#define COMPILEMOD_VERSION_2		2	// Premiere 5.1
#define COMPILEMOD_VERSION			COMPILEMOD_VERSION_2

#define PREMPLUGENTRY int

#ifdef PRWIN_ENV
#define compFileRef HANDLE
#define compPalette	HPALETTE
#endif

#ifdef PRMAC_ENV
#pragma options align=power // Unlike the other API's, compile module interface uses PPC alignment
#include <Palettes.h>
#define compFileRef short
#define compPalette PaletteHandle
#endif


//========================================================================================
// Structures passed with the callbacks
//========================================================================================

// file spec for open
typedef struct
{
	short	volID;			// used on Mac only
	long	parID;			// used on Mac only
	char	name[256];		// file name on Mac, full path elsewhere
} compFileSpec;


// This is info passed back to the compile module in response to calling
// compGetFrame

typedef struct
{
	int				returnVal;			// comp_ErrNone - continue compiling
										// comp_CompileAbort - user aborted the compile
										// comp_CompileDone - finished the compile
										// ... or an error code
	int				repeatCount;		// The frame should be repeated for repeatCount frames
										// in the output files (write null frames, change the frame
										// duration or whatever is appropriate)
	int				makeKeyFrame;		// This frame should be compressed as a keyframe
	int				frameIsCompressed;	// Frame is already compressed to match compOutputRec
	unsigned long	startframe;			// The start time for this frame (in the timebase of OutputRec)
	int				frameOnMarker;		// There is a marker on this frame.
} compGetFrameReturnRec;


// This is info passed back to the compile module in response to calling
// compGetFrameInfo

typedef struct
{
	long			frameFlags;		// compNeedsRendering
	long			frameType;		// fourcc (i.e. 'Moov', 'AVI ')
	long			frameWidth;
	long			frameHeight;
	long			frameSize;		// bytes 
	long			deltaFlags;
	long			privateFrameInfo;
	long			uniqueID;		// uniqueID for this file
	long			frameNum;		// frame number of this sample (in frames, not time)
	long			nearestKey;		// nearest keyframe before this frame.

	// The following fields are only returned for COMPILEMOD_VERSION_2 or greater.
	
	compFileSpec	filespec;		// The spec for the file containing this frame.
	long			repeatCount;	// The number of contiguous frames from this file.
	long			isPreviewFile;	// 1 if the frame comes from a Preview File.
} compFrameInfoRec;


typedef long *compHistogramPtr;
typedef struct
{
	int				width;			// width of image
	int				height;			// height of image
	short			rowBytes;		// # of bytes in a row
	char			*pixels;		// ptr to the pixels
} compHistogramFrameInfoRec;


#ifdef PRWIN_ENV
typedef struct
{
	LPBITMAPINFOHEADER	lpbiIn;		//	dib format
	LPBYTE				pbIn;		//	input pixels
	LPBITMAPINFOHEADER	lpbiOut;	//	output dib format
	LPBYTE				pbOut;		//	output pixels
	HPALETTE			hpal;		//	palette to use
} compPixelInfoRec;
#endif


#ifdef PRMAC_ENV
typedef struct
{
	CTabHandle			hpal;		//	palette to use
} compPixelInfoRec;
#endif


//========================================================================================
// Callbacks
//========================================================================================

typedef char***		BufferReturnType;

// audio functions

// compGetAudio returns an error if the end of buffer was reached
// frame is the frame number of the compile
// frameCount coming in is the number of frames requested. frameCount out is the number of frames we got
// size return the size in bytes that we got
// offset is the number of bytes offset into the audioBuffer to put the data
// theBuffer returns the audioBuffer. This handle points at the beginning of the buffer not the offset.  
// This is a handle because QT for one needs the handle compileSeqID is the ID to the current compile sequence
typedef int  (*compGetAudio)(long frame, long *frameCount, long *size, long offset,
							 BufferReturnType theBuffer, long compileSeqID);

// compGetMaxBlip returns the maximum size of a blip
// compileSeqID is the ID to the current compile sequence
typedef long (*compGetMaxBlip)(long compileSeqID);

// video functions

// compGetFrame returns:
//		comp_ErrNone - continue compiling
//		comp_CompileAbort - user aborted the compile
//		comp_CompileDone - finished the compile
//		... or an error code
typedef int (*compGetFrame)(long frame, void **buffer, long *rowbytes, compGetFrameReturnRec *getFrameReturn, 
							char getCompressed, long compileSeqID);

// compGetFrameInfo - Get information about a given frame
// return values:
//		compFFNeedsRendering - the frame needs to be rendered.
//		compFFSingleFile - the frame comes from a single file. Information about the
//							frame is returned in the compFrameInfoRec
typedef int (*compGetFrameInfo)(long frame, compFrameInfoRec *frameInfo, long compileSeqID);


//	the palette creating section of the video functions

typedef compHistogramPtr (*compHistogramInit)();
typedef void (*compHistogramFree)(compHistogramPtr histogram);
typedef int (*compHistogramProcessPixels)(compHistogramFrameInfoRec *histogramInfo, compHistogramPtr histogram);
typedef compPalette (*compHistogramToPalette)(compHistogramPtr histogram, char *lp16to8, int nColors);
typedef void (*compHistogramDisposePalette)(compPalette palette); 
typedef void (*compReleaseGetFrameBuffer)(long compileID, void *buffer);
typedef void (*compHistogramReducePixels)(compPixelInfoRec *histogramPixelInfo, compPalette palette, char *lp16to8); 

// file functions

typedef long (*compGetInfoChunkSize)(int chunkID);
typedef void (*compGetInfoChunk)(int chunkID, void *buffer);

// memory functions

typedef char *(*compNewPtrFunc)(long size);
typedef long (*compSetPtrSizeFunc)(char *ptr, long newsize);
typedef long (*compGetPtrSizeFunc)(char *ptr);
typedef void (*compDisposePtrFunc)(char *ptr);

// progress functions

// Set the string for the progress bar. Pass nil to reset it to the default string.
typedef void (*compSetProgressStrFunc)(char *ptr, long compileSeqID);

// Update the progress bar. This normally only needs to be called for referencing or
// accelerated compiles. This function is only available if compInterfaceVer is >= COMPILEMOD_VERSION_2.
// returns comp_ErrNone if successful.
typedef int (*compUpdateProgressFunc)(long frame, long compileSeqID);


// debugging functions

typedef void (*compStartDebugFunc)(long playID);
typedef void (*compDebugStrFunc)(long playID, char *cstr);


typedef struct
{
	compStartDebugFunc	startDebug;
	compDebugStrFunc	debugStr;
} compDebugFuncs, *compDebugFuncsPtr;

typedef struct
{
	compGetAudio		getAudio;			//	callback to get the audio
	compGetMaxBlip		getBlipMax;			//	Get the max blip size
} compAudioFuncs, *compAudioFuncsPtr;

typedef struct
{
	compGetFrame			getFrame;
	compGetFrameInfo		getFrameInfo;
	compHistogramInit		histogramInit;
	compHistogramFree		histogramFree;
	compHistogramProcessPixels	histogramProcessPixels;
	compHistogramToPalette	histogramToPalette;
	compHistogramDisposePalette	histogramDisposePalette; 
	compHistogramReducePixels	histogramReducePixels;
} compVideoFuncs, *compVideoFuncsPtr;

typedef struct
{
	compNewPtrFunc			newPtr;
	compSetPtrSizeFunc		setPtrSize;
	compGetPtrSizeFunc		getPtrSize;
	compDisposePtrFunc		disposePtr;
} compMemoryFuncs, *compMemoryFuncsPtr;

typedef struct
{
	compSetProgressStrFunc	setProgressStr;
	compUpdateProgressFunc	updateProgress;
} compProgressFuncs, *compProgressFuncsPtr;


// Utility functions passed during comp_Startup

typedef struct
{
	ClassDataFuncsPtr		classFuncs;
	compAudioFuncsPtr		audioFuncs;
	compVideoFuncsPtr		videoFuncs;
	compDebugFuncsPtr		debugFuncs;
	compMemoryFuncsPtr		memoryFuncs;
	compProgressFuncsPtr	progressFuncs;
} compCallbackFuncs;


//========================================================================================
// Structures passed at the main entry point
//========================================================================================

// Standard parms passed into xCompileEntry

typedef struct
{
	int					compInterfaceVer;	// version # of compile interface (currently COMPILEMOD_VERSION)
	compCallbackFuncs	*funcs;
	piSuitesPtr			piSuites;			// present if compInterfaceVer is >= COMPILEMOD_VERSION_2
} compStdParms;


// values for "frameFlags" in compFrameInfoRec

enum
{
	compFFNeedsRendering = 1,
	compFFSingleFile
};

// return codes

enum
{
	comp_ErrNone = 0,			// no error
	comp_BadFormatIndex,		// Invalid format index - used to stop compGetIndFormat queries
	comp_CompileAbort,			// User aborted the compile
	comp_CompileDone,			// Compile finished normally
	comp_InternalError,
	comp_OutputFormatAccept,	// The output format is valid
	comp_OutputFormatDecline,	// The compile module cannot compile to the output format
	comp_OutOfDiskSpace,		// Out of disk space error
	comp_BufferFull,			// The offset into the audio buffer would overflow it
	comp_ErrOther,				// Someone set gCompileErr
	comp_ErrMemory,				// ran out of memory
	comp_ErrFileNotFound,		// File not found
	comp_ErrTooManyOpenFiles,	// Too many open files
	comp_ErrPermErr,			// Permission violation
	comp_ErrOpenErr,			// Unable to open the file
	comp_ErrInvalidDrive,		// Drive isn't valie.
	comp_ErrDupFile,			// Duplicate Filename
	comp_ErrIo,					// File io error
	comp_ErrInUse,				// File is in use
	comp_RebuildCutlist			// Return value from compGetFilePrefs used to force Premiere to bebuild its cutlist
								// Works if compInterfaceVer is >= COMPILEMOD_VERSION_2
};


// field types for fieldType enum

enum
{
	compFieldsNone,
	compFieldsUnknown,		// has fields, unknown dominance
	compFieldsUpperFirst,	// topmost line is dominant
	compFieldsLowerFirst	// topmost+1 line is dominant
};

// values for audio interleave
#define prInterleaveNone		0
#define prInterleave1Frame		1
#define prInterleaveHalfSec		2
#define prInterleave1Sec		3
#define prInterleave2Sec		4


// struct filled in by compInit message
// These are the capabilities of the compile module 

typedef struct
{
	long			compilerID;			// Runtime ID for the module - don't change this!
	
	long			fileType;			// The file type (AVI, MOOV etc)
										// On windows, how is this matched up to extensions?

	long			classID;			// The classID for the module
										// This is used to differentiate between compile modules that
										// support the same fileType and to cross reference between
										// different plugin types (i.e. play and record modules).

	int				hasSetup;			// If 1, the compiler has a private setup dialog
										// Note: this is for the fileType, not the compressor.
										// The compiler should display its custom dialog during compGetFilePrefs.
	
	int				compilesVideo;		// The compiler can output video
	int				compilesAudio;		// The compiler can output audio

	int				hasAllocator;		// The compiler has its own memory allocator
										// ...necessary? Could just have the compiler optionally pass a buffer
										// to GetFrame - if it doensn't pass one, Premiere uses its own mem allocator
	
	int				canCopyFrames;		// The compile can copy compressed frames that match its output format
										// ...should we just assume this is true???

	int				canOpen;			// 
	
	int				singleFrame;		// Module compiles a single frame (still image exporter) - this
										// may be called by a wrapper to compile numbered stills.
	
	char			compilerName[256];	// The displayable name for this compiler
	
	long			version;			// The version of the api that the compiler supports

	long			reserved[31];		// reserved; set to nil;
} compInfoRec;


//	Video depth support
#define compDepth1		0x000000001
#define compDepth4		0x000000002
#define	compDepth8		0x000000004
#define compDepth16		0x000000008
#define compDepth24		0x000000010
#define compDepth32		0x000000020

// Use compDepthAny if the compiler can export at the most common depths.
// Note that this excludes 1 and 4-bit depths because they're not generally usefull.
#define compDepthAny	(compDepth8 | compDepth16 | compDepth24 | compDepth32)

//	Audio format support
#define comp8Mono		0x000000001
#define comp8Stereo		0x000000002
#define	comp16Mono		0x000000004
#define comp16Stereo	0x000000008
#define compAnyAudio	(comp8Mono | comp8Stereo | comp16Mono | comp16Stereo)

#define compUncompressed	'RAW '
#define compBadFormat		0xffffffff


// This specifies the info for a particular file (based on a compressionID). This
// is different than compCapabilities which specifies capabilities for the
// entire module.
//
// For a particular subtype, the compile module can specify what properties it
// can set.

typedef struct
{
	long			compilerID;			// ID for the module - don't change this!
	long			subtype;			// File subtype supported. Generally this will be a compressionID,
										// but not always.
										// subtype==compUncompressed is a special case for "no compression"
										// subtype==compBadFormat is an invalid format

	char			name[256];			// The displayable name for this subtype
	
	char			canForceKeyframes;	// 

	int				depthsSupported;	// compDepth8 or compDepth16 ... compDepthAny

	int				paletteAtDepth;		// This subtype accepts a palette when it is at the given depth.
										// compDepth8, compDepth16 ... compDepthAny
										// Set to 0 if the subtype never wants a palette.
	
	long			frameMultiple;		// Restrict output video frame sizes to this multiple (default is 1)
	long			minWidth;			// Minimum width (set min = max to allow only one size)
	long			minHeight;			// Mininum height
	long			maxWidth;			// Maximum width
	long			maxHeight;			// Maximum height

	char			canDoFields;		// Fields can be specified
	char			canDoQuality;		// Quality can be specified
	char			canSetDataRate;		// A datarate can be specified
	char			hasSetup;			// The compressor has other private settings. 
										// These can be prompted for during compGetSubTypePrefs.
	char			canDelta;			// Not restricted to keyframes
	
	char			fixedTimebase;		// only compiles to one timebase
	long			fixedScale;			// fixed timebase scale
	long			fixedSampleSize;	// fixed timebase sample size

	int				defaultQuality;		// The preferred quality setting. If -1, this is ignored.
										// Range is 0-100
	int				defaultKeyFrameRate;// The preferred key frame rate. If -1, this is ignored.
} compFileInfoRec;


// This specifies the info for a particular audio compressor (based on a compressionID). This
// is different than compCapabilities which specifies capabilities for the
// entire module.
//
// For a particular audio compress, the compile module can specify what properties it
// can set.

//	Maximum number of rates
#define	kMaxSupportedRates			16
//	Constant to support any audio rate
#define	kSupportAnyRate				-1


//	Structure for the audio formats a compressor supports
//	ie 16bit mono @ 44100
typedef struct
{
	int				audioDepths;				// audio format support bit field (use audio format support #defines)
												// comp8Mono, comp16Mono etc. compAnyAudio for all depths.

	long			audioRate;					// Audio rates the compressor supports
												// kSupportAnyRate means the compressor supports all rates including
												// arbitrary ones (the "Other--" selection).
} compAudioFormat, *compAudioFormatPtr;


typedef struct
{
	long				compilerID;			// ID for the module - don't change this!
	long				subtype;			// File subtype supported. Generally this will be a compressionID,
											// but not always.
											// subtype==compUncompressed is a special case for "no compression"
											// subtype==compBadFormat is an invalid format

	char				hasSetup;			// The compressor has other private settings. 
											// These can be prompted for during compGetAudSubTypePrefs.

	char				name[256];			// The displayable name for this subtype
	
	compAudioFormatPtr	audioFormats;		// A ptr to a list of audio formats that the compressor supports
											// if this param is nil, it supports any rates and bits
} compAudioInfoRec;


typedef struct
{
	long			scale;				// timebase
	long			sampleSize;			// size of one sample
	unsigned long	value;
} compTimebaseRec;


// This is the struct sent with compGetFilePrefs

typedef struct
{
	char		*compilerPrefs;			// Buffer to store preferences
										// If null, a buffer of the desired size must be allocated using the memory suite
										// Otherwise, it's a previously allocated buffer that can be reused.
} compGetFilePrefsRec;


// This is the struct sent with compGetSubTypePrefs

typedef struct
{
	long		subType;				// The subType that the prefs are for.
	char		*subTypePrefs;			// Buffer to store preferences
										// If null, a buffer of the desired size must be allocated *using the memory suite*
										// Otherwise, it's a previously allocated buffer that can be reused.
} compGetSubTypePrefsRec;


// settings for recompressWhen

enum {
	recompNever,		// don't recompress if it can be avoided
	recompAlways,		// always recompress
	recompMaintain		// only recompress is requestedDataRate is exceeded
};


// This is the compression record passed when compiling

typedef struct
{
	long				subtype;				// Compile using this video subtype (codec)
	int					depth;					// compDepth 8/16/24/32
	int					quality;				// qt has CodecQuality AND TemporalQuality - are both needed?
												// Range is 0-100
	int					temporalQuality;		// qt WOULD like both
	int					requestedDataRate;		// in K/sec -- 0 == no requested datarate
	int					recompressWhen;			// When to recompress; see enum above
	int					keyFrameEvery;			// Make a keyframe every n frames.
												// If 0, this is turned off.
												// This is also done in the host, but is here because qt is using it in CompressSequenceBegin

	void				*subTypePrefs;			// Codec private settings (from compGetSubTypePrefs)

	char				expandStills;			// If true, then single frames should always be added, even
												// if repeatCount > 1.
												// If false, single frames can be optimized (add a single frame
												// with a long duration, or a single frame followed by null frames)
	compPalette			palette;				// palette to use for compression operations
} compVidCompression;


typedef struct
{
	void				*subTypePrefs;			// Codec private settings (from compGetSubTypePrefs)
	long				subtype;
	char				AudCompRec[512];		// compressed audio record (WAVEFORMATEX on Windows)
} compAudCompression;


// This is the output format

typedef struct
{
	int					doVideo;		// 1 if video is being compiled
	int					doAudio;		// 1 if audio is being compiled
	
	compVidCompression	vidCompression;	// Video compression settings
	compAudCompression	audCompression;	// Audio compression settings

	int					width;			// The output video width
	int					height;			// The output video height
	compTimebaseRec		timebase;		// timebase of the output movie
	int					fieldType;		// Field output type, see enum

	unsigned long		audrate;		// The audio rate in samples per second.
	int					audsamplesize;	// The samplesize, 8 or 16 (this a number not a flag)
	int					stereo;			// If 1, audio is stereo, othewise it's mono
	int					audchunksize;	// Size of audio chunks
										// If 0, use a default size
	int					audInterleave;	// See defines (prInterleaveNone etc)
} compOutputRec;


typedef struct
{
	int				cropTop;            // amount to crop inwards on left, top, right, bottom
	int				cropLeft;
	int				cropBottom;
	int				cropRight;
	char            ResizeToCropRect;   // 0 = output size will be adjusted to match cropping
	int				gammaCorrection;	// Amount to gamma correct output
} compPostProcessing;


// This is the struct sent with compDoCompile

typedef struct
{
	long				compilerID;
	void				*compilerPrefs;		// Private compiler data
	compOutputRec		outputRec;
	compPostProcessing	postProcessing;
	compFileSpec		outputFile;
	compFileRef			outputFileRef;
	long				startFrame;
	long				endFrame;			// This is exclusive (i.e. for (frame=startFrame; frame<endFrame; frame++) )
	
	long				compileSeqID;		// This must be passed to compGetFrame and GetAudio (audio callback)
											// Used to be getFrameData - it is opaque data, so rename it to curb
											// the temptation to figure out what is in it.
	Handle				timelineData;		// Handle that can be used with the prPlugTimeline.h callback suite
											// Present if compInterfaceVer is >= COMPILEMOD_VERSION_2
} compDoCompileInfo;


// Main entry prototype
typedef PREMPLUGENTRY (* CompileEntryFunc)(int selector, compStdParms *stdparms, long param1, long param2);


// Selectors 
enum
{
	compStartup,
	compDoShutdown,
	compGetIndFormat,
	compGetAudioIndFormat,
	compQueryOutputFormat,
	compGetFilePrefs,
	compGetSubTypePrefs,
	compGetAudSubTypePrefs,
	compDoCompile,
	compDoSample,
	compQueryOutputSize
};


#ifdef PRMAC_ENV
#pragma options align=reset
#endif

#include "prResetEnv.h"	
		
#endif // _PRCOMPILEMOD
