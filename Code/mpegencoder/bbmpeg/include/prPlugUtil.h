//========================================================================================
//
// prPlugUtil.h
//
// Part of the Adobe Premiereª 5 Plug-In Developer's Toolkit.
//
// Defines the utility callback suite. You should always check the piInterfaceVer in piSuites
// before assuming all these functions are available.
//
// This entire header was added in PR_PISUITES_VERSION_2.
//
// Copyright © 1992-99, Adobe Systems Incorporated, all rights reserved worldwide.
//
//========================================================================================


#ifndef __PRPLUGUTIL
#define __PRPLUGUTIL

// utility functions for plug-ins

typedef long (*plugGetSettingsFunc)(long settingsSelector);

typedef struct
{
	plugGetSettingsFunc			getSettings;	// added in PR_PISUITES_VERSION_2
} PlugUtilFuncs, *PlugUtilFuncsPtr;

// selectors for getSettings

#define kSettingsVidWidth				1
#define kSettingsVidHeight				2
#define kSettingsCapDrive				3
#define kSettingsTempVideo				4
#define kSettingsTempAudio				5
#define kSettingsFieldType				6
#define kSettingsVideoFileType			7
#define kSettingsIsStereo				8
#define kSettingsIs16BitAudio			9
#define kSettingsGetAudRate				10
#define kSettingsProjectSampleSize		11
#define kSettingsProjectScale			12
#define kSettingsTimeFormat				13
#define kSettingsCompilerPrivate		14
#define kSettingsSubtypePrivate			15
#define kSettingsAudSubtypePrivate		16
#define kSettingsProjectDrive			17
#define kSettingsPlaymodPrivate			18
#define kSettingsSubtype				19


#endif /* __PRPLUGMEM */
