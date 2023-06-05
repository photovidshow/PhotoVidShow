//========================================================================================
//
// prPlugSuites.h
//
// Part of the Adobe Premiereª 5 Plug-In Developer's Toolkit.
//
// Defines all the callback suites which are available in all the plug-in interfaces.
//
// Copyright © 1992-99, Adobe Systems Incorporated, all rights reserved worldwide.
//
//========================================================================================

#ifndef __PRPLUGSUITE
#define __PRPLUGSUITE

#include "prSetEnv.h"
#include "prplugmem.h"
#include "prplugwind.h"
#include "prplugppix.h"
#include "prplugutil.h"
#include "prplugtimeline.h"

#define PR_PISUITES_VERSION_1		1
#define PR_PISUITES_VERSION_2		2
#define PR_PISUITES_VERSION			PR_PISUITES_VERSION_2

// Premiere plug-in suites

typedef struct
{
	int						piInterfaceVer;	// version of plug-in interface (PR_PISUITES_VERSION)
	PlugMemoryFuncsPtr		memFuncs;
	PlugWindowFuncsPtr		windFuncs;
	PlugppixFuncsPtr		ppixFuncs;
	PlugUtilFuncsPtr		utilFuncs;		// added in PR_PISUITES_VERSION_2
	PlugTimelineFuncsPtr	timelineFuncs;	// added in PR_PISUITES_VERSION_2
} piSuites, *piSuitesPtr;

#include "prResetEnv.h"
#endif /* __PRPLUGSUITE */
