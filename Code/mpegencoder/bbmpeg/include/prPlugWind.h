//========================================================================================
//
// prPlugWind.h
//
// Part of the Adobe Premiereª 5 Plug-In Developer's Toolkit.
//
// Defines the window callback suite.
//
// Copyright © 1992-99, Adobe Systems Incorporated, all rights reserved worldwide.
//
//========================================================================================


#ifndef __PRPLUGWIN
#define __PRPLUGWIN

// window functions

typedef void (*plugUpdateAllWindowsFunc)(void);
typedef long (*plugGetMainWindFunc)(void);

typedef struct
{
	plugUpdateAllWindowsFunc	updateAllWindows;
	plugGetMainWindFunc			getMainWnd;
} PlugWindowFuncs, *PlugWindowFuncsPtr;

#endif /* __PRPLUGWIN */
