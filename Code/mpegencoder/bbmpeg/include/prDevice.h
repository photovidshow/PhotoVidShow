//========================================================================================
//
// prDevice.h
//
// Part of the Adobe Premiereª 5 Plug-In Developer's Toolkit.
//
// Defines the Premiere Device Control interface.
//
// Copyright © 1992-99, Adobe Systems Incorporated, all rights reserved worldwide.
//
//========================================================================================

#ifndef __PRDEVICE_H
#define __PRDEVICE_H

#include "prSetEnv.h"
#include "prplugsuites.h"

#ifdef PRWIN_ENV
#define PRDEVICEENTRY short
typedef long (*CallBackPtr)(void); 
#endif

#ifdef PRMAC_ENV
#define PRDEVICEENTRY pascal short
typedef pascal char (*CallBackPtr)(void);
#endif

typedef void (*PauseProcPtr)(void);
typedef void (*ResumeProcPtr)(void);
typedef long (*Print2TapeProcPtr)(Handle deviceHand, long selector);


// The main info record

typedef struct {
	Handle					deviceData;				// private data which the plug-in creates
	short					command;				// command to perform
	short					mode;					// New mode (in) or current mode (out)
	long					timecode;				// New timecode (in) or current timecode (out); -1 = N/A, -2 = blank
	short					timeformat;				// Format: 0=non-drop, 1=drop-frame
	short					timerate;				// fps for the timecode above
	long					features;				// feature bits (out) for the features command
	short					error;					// error code (out) from any routine
	short					preroll;				// pre-roll time (secs) for cmdLocate
	CallBackPtr				callback;				// callback for cmdLocate, returns non-zero to stop
	PauseProcPtr			PauseProc;				// callback to pause current operations
	ResumeProcPtr			ResumeProc;				// callback to restart current operations
	long					xtimecode;				// duration, for editing operations
	long					keycode;				// not used
	short					editmode;				// edit enable flags for cmdLocateInsertAsm command
	short					exteditmode;			// not used
	Print2TapeProcPtr		PrintProc;				// callback when driver is performing the edit
	long					reserved1;
	piSuitesPtr				piSuites;				// pointer to plug-in suites functions
	char					reserved[68];			// reserved;
} DeviceRec, *DevicePtr, **DeviceHand;


// Codes sent to plug-in interface

enum {
	dsInit = 0,					// Create any structure(s), pick an operating mode, no dialogs here
	dsSetup,					// prompt for any user dialogs
	dsExecute,					// perform command
	dsCleanup,					// dispose any allocated structure(s)
	dsRestart,					// re-start any features, used at program startup to reconnect device
	dsQuiet						// disconnect from device but DON'T dispose allocated structures
};

// Feature bits

enum {
	fSyncStatus		= 0x00200000,	// driver guarantees sync status call --- NEW in 5.1+
	fExportDialog	= 0x00100000,	// driver has its own export dialog --- NEW in 5.0
	fCanInsertEdit	= 0x00080000,	// supports the InsertEdit command -- NEW in 5.0
	fDrvrQuiet		= 0x00040000,	// driver supports a quiet mode --- added in 4.2
	fHasJogMode		= 0x00020000,	// device has jog capabilities --- added in 4.2
	fCanEject		= 0x00010000,	// can Eject media --- added in 4.2
	fStepFwd		= 0x00008000,	// can step forward
	fStepBack		= 0x00004000,	// can step back
	fRecord			= 0x00002000,	// can record
	fPositionInfo	= 0x00001000,	// returns position info
	fGoto			= 0x00000800,	// can seek to a specific frame (fPositionInfo must also be set)
	f1_5			= 0x00000400,	// can play at 1/5 speed
	f1_10			= 0x00000200,	// can play at 1/10 speed
	fBasic			= 0x00000100,	// supports Stop,Play,Pause,FastFwd,Rewind
	fHasOptions		= 0x00000080,	// plug-in puts up an options dialog
	fReversePlay	= 0x00000040,	// supports reverse play
	fCanLocate		= 0x00000020,	// can locate a specific timecode
	fStillFrame		= 0x00000010,	// device is frame addressable, like a laser disc
	fCanShuttle		= 0x00000008,	// supports the Shuttle command
	fCanJog			= 0x00000004	// supports the JogTo command
};

// Mode commands/states

enum {
	modeStop = 0,
	modePlay,
	modePlay1_5,
	modePlay1_10,
	modePause,
	modeFastFwd,
	modeRewind,
	modeRecord,
	modeGoto,
	modeStepFwd,
	modeStepBack,
	modePlayRev,
	modePlayRev1_5,
	modePlayRev1_10,
	modeTapeOut,			// 5.0 -- no media in device
	modeLocal				// 5.0 -- VTR in local not remote mode
};

// Commands which plug-in can perform

enum {
	cmdGetFeatures = 0,			// return feature bits
	cmdStatus,					// fill in current mode and timecode, gets called repeatedly
	cmdNewMode,					// change to the mode in 'mode'
	cmdGoto,					// goto the timecode specified in 'timecode'
	cmdLocate,					// find the timecode in 'timecode' and then return (with deck in play)
	cmdShuttle,					// shuttle at rate specified in 'mode', from -100 to +100
	cmdJogTo,					// position at 'timecode', quickly from the current location
	cmdJog,						// 4.2	Jog at rate specified in 'mode', from -25 to +25
	cmdEject,					// 4.2	eject media
	cmdLocateInsertAsm,			// 5.0  used when Premiere controlling the edit
	cmdInsertEdit,				// 5.0  used when driver is controlling the edit
	cmdStatusSync				// 5.1+ do a sync status call
};

// edit enable flags for cmdLocateInsertAsm command

enum {
        insertVideo                             = 0x0001,
        insertAudio1                            = 0x0002,
        insertAudio2                            = 0x0004,
        insertTimeCode                          = 0x0008,
        insertAssemble                          = 0x0080,
		insertPreview							= 0x1000
};

// used when device driver is handling the edit

enum {
		setupWaitProc = 1,
		idle,
		complete
};


#include "prResetEnv.h"
#endif /* __PRDEVICE_H */
