//========================================================================================
//
// prPlugMem.h
//
// Part of the Adobe Premiereª 5 Plug-In Developer's Toolkit.
//
// Defines the memory callback suite. You should always check the piInterfaceVer in piSuites
// before assuming all these functions are available.
//
// Copyright © 1992-99, Adobe Systems Incorporated, all rights reserved worldwide.
//
//========================================================================================

#ifndef __PRPLUGMEM
#define __PRPLUGMEM

// Prototypes

typedef char *(*plugNewPtrFunc)(long size);
typedef void (*plugSetPtrSizeFunc)(Ptr *ptr, long newsize);
typedef long (*plugGetPtrSizeFunc)(char *ptr);
typedef void (*plugDisposePtrFunc)(char *ptr);
typedef char **(*plugNewHandleFunc)(long size);
typedef short (*plugSetHandleSizeFunc)(char **Handle, long newsize);
typedef long (*plugGetHandleSizeFunc)(char **Handle);
typedef void (*plugDisposeHandleFunc)(char **Handle);
typedef char *(*plugNewPtrClearFunc)(long size);
typedef char **(*plugNewHandleClearFunc)(long size);
typedef void (*plugLockHandleFunc)(char **Handle);
typedef void (*plugUnlockHandleFunc)(char **Handle);


typedef struct
{
	plugNewPtrFunc				newPtr;
	plugSetPtrSizeFunc			setPtrSize;
	plugGetPtrSizeFunc			getPtrSize;
	plugDisposePtrFunc			disposePtr;
	plugNewHandleFunc			newHandle;
	plugSetHandleSizeFunc		setHandleSize;
	plugGetHandleSizeFunc		getHandleSize;
	plugDisposeHandleFunc		disposeHandle;
	plugNewPtrClearFunc			newPtrClear;	// added in PR_PISUITES_VERSION_2
	plugNewHandleClearFunc		newHandleClear;	// added in PR_PISUITES_VERSION_2
	plugLockHandleFunc			lockHandle;		// added in PR_PISUITES_VERSION_2
	plugUnlockHandleFunc		unlockHandle;	// added in PR_PISUITES_VERSION_2
} PlugMemoryFuncs, *PlugMemoryFuncsPtr;

#endif /* __PRPLUGMEM */
