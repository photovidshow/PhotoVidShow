//========================================================================================
//
// prSetEnv.h
//
// Part of the Adobe Premiereª 5 Plug-In Developer's Toolkit.
//
// All SDK headers should include this header at their top to ensure the proper 
// compiling environment is setup.
//
// Copyright © 1992-99, Adobe Systems Incorporated, all rights reserved worldwide.
//
//========================================================================================

#ifdef __POWERPC__
	#define PRMAC_ENV
	#undef PRWIN_ENV
	#pragma options align=mac68k
#endif

#ifndef PRWIN_ENV
	#ifdef WIN32
		#define PRWIN_ENV
		#undef PRMAC_ENV
	#endif
#endif

#ifdef PRWIN_ENV
	#pragma pack(push, 1)
	#include "prWinEnv.h"
#endif

#ifdef __cplusplus
extern "C" {
#endif
