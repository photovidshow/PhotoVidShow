//========================================================================================
//
// PiPLVer.h
//
// Part of the Adobe Premiereª 5 Plug-In Developer's Toolkit.
//
// Copyright © 1992-99, Adobe Systems Incorporated, all rights reserved worldwide.
//
//========================================================================================

/* This file should be included by all PiPL's. It defines a global plug-in version 
   number scheme plus a few other useful types. */

/* PiPL Build Stages - can be used in the version string. */
#define PiPLDevelop			0
#define PiPLAlpha			1
#define PiPLBeta			2
#define PiPLRelease			3

/* Effect version number - all the SDK plug-ins have been set to version 5.0 */
#define PiPLMajorVersion	5
#define PiPLMinorVersion	0
#define PiPLStage			PiPLRelease
#define PiPLBuildNum		0

/* PiPL version number, this is the version of the PiPL resource itself. Premiere 5.0
   supported PiPL version 2.1, Premiere 5.1 supports PiPL version 2.2 (and earlier). */
#define PiPLVerMajor		2
#define PiPLVerMinor		2

/* Transition corner bits - these are needed by the Pr_Effect_Info property for transitions. */
#define bitNone				0x00
#define bitTop				0x01
#define bitRight			0x02
#define	bitBottom			0x04
#define	bitLeft				0x08
#define	bitUpperRight		0x10
#define	bitLowerRight		0x20
#define	bitLowerLeft		0x40
#define	bitUpperLeft		0x80

