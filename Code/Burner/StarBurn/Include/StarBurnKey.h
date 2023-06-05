
/*++

Copyright (c) Rocket Division Software 2001-2006. All rights reserved.

Module Name:

	StarBurnKey.h

Abstract:

	StarBurn SDK free registration key. Used for non-commercial license
	initialization.

Example:

	This example initializes StarBurn toolkit for non-commercial use.

	#include "StarBurnKey.h"

	Somewhere in the data region
	EXCEPTION_NUMBER l__EXCEPTION_NUMBER = EN_SUCCESS;

	// Try to initialize toolkit
	l__EXCEPTION_NUMBER =
		StarBurn_UpStartEx(
			( PVOID )( &g__UCHAR__RegistrationKey ),
			sizeof( g__UCHAR__RegistrationKey )
			);

	// Check for success
	if ( l__EXCEPTION_NUMBER != EN_SUCCESS )
	{
		// Handle error here...
	}

Author:

	Anton A. Kolomyeytsev

Environment:

	Windows 95/98/ME/NT/2K/XP/2003 UserMode only

Notes:

Known bugs/Illegal behavior:

ToDo:

--*/



//extern unsigned char g__UCHAR__RegistrationKey[ ] ;
