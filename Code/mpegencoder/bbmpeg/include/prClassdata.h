//========================================================================================
//
// prClassdata.h
//
// Part of the Adobe Premiereª 5 Plug-In Developer's Toolkit.
//
// Copyright © 1992-99, Adobe Systems Incorporated, all rights reserved worldwide.
//
//========================================================================================

#ifndef __PRCLASSDATA
#define __PRCLASSDATA

// ClassData functions - available to media abstraction plug-ins.

typedef long (* GetClassDataFunc)(long theclass);
typedef int (* SetClassDataFunc)(long theclass, long info);

typedef struct {
	SetClassDataFunc	setClassData;
	GetClassDataFunc	getClassData;
} ClassDataFuncs, *ClassDataFuncsPtr;

#endif /* __PRCLASSDATA */
