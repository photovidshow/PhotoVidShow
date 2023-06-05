//========================================================================================
//
// prPlugTimeline.h
//
// Part of the Adobe Premiereª 5 Plug-In Developer's Toolkit.
//
// Defines the timeline callback suite. You should always check the piInterfaceVer in piSuites
// before assuming all these functions are available.
//
// This entire header was added with PR_PISUITES_VERSION_2.
//
// Copyright © 1992-99, Adobe Systems Incorporated, all rights reserved worldwide.
//
//========================================================================================

#ifndef __PRPLUGTIMELINE
#define __PRPLUGTIMELINE

#include "prrt.h"

// utility functions for plugins that want to access timeline information
typedef prtSegmentRecPtr (*plugGetVideoSegmentFunc)(long frame, Handle timelineData);
typedef long (*plugDisposeSegmentFunc)(prtSegmentRecPtr segment);
typedef long (*plugGetClipVideoFunc)(long frame, PPixHand thePort, prRect *bounds, long flags, Handle clipData);

typedef struct
{
   plugGetVideoSegmentFunc    getVideoSegment;	// added in PR_PISUITES_VERSION_2
   plugDisposeSegmentFunc     disposeSegment;	// added in PR_PISUITES_VERSION_2
   plugGetClipVideoFunc       getClipVideo;		// added in PR_PISUITES_VERSION_2
} PlugTimelineFuncs, *PlugTimelineFuncsPtr;

#endif /* __PRPLUGTIMELINE */
