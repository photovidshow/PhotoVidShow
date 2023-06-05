// Errors.cpp: implementation of the CErrors class.
//
//////////////////////////////////////////////////////////////////////


//#include "common.h"
//#include "ltstring.h"

#define _CRT_SECURE_NO_WARNINGS 1
#include "UnmanagedErrors.h"
#include <string>
#include "stdarg.h"


static StringLogCallback ManagedTraceCallback;
static StringLogCallback ManagedWarningCallback;
static StringLogCallback ManagedErrorCallback;
static StringLogCallback ManagedFatalErrorCallback;
static StringLogCallback ManagedPopupErrorCallback;

static CharLogCallback ManagedDebugAddEncodePositionCallback;
static IntCallback ManagedGetLogLevelCallback;
static IntCallback ManagedTraceEncodeCallback;


void UnmanagedSetLogCallbacks(StringLogCallback trace,
							StringLogCallback warning,
							StringLogCallback error,
							StringLogCallback fatal_error,
							StringLogCallback popup_error,
							CharLogCallback DebugAddEncodePosition,
							IntCallback LogLevel,
							IntCallback TraceEncode)
{
  ManagedTraceCallback = trace;
  ManagedWarningCallback =warning;
  ManagedErrorCallback = error;
  ManagedFatalErrorCallback = fatal_error;
  ManagedPopupErrorCallback =popup_error;
  ManagedDebugAddEncodePositionCallback= DebugAddEncodePosition;
  ManagedGetLogLevelCallback= LogLevel;
  ManagedTraceEncodeCallback = TraceEncode;
}


int TraceEncode()
{
	if ( (*ManagedTraceEncodeCallback)()==1)
	{
		return 1;
	}
	return 0;
}

LogLevel GetLogLevel()
{
	return (LogLevel)(*ManagedGetLogLevelCallback)();
}

void Trace(char *fmt, ...)
{
	if (GetLogLevel() < LOGLEVEL_TRACE)
	{
		return ;
	}

	static BOOL doing_trace = FALSE ;
	if ( fmt )
	{
		if (doing_trace == TRUE)
		{
			return;
		}

		doing_trace = TRUE ;

		char buffer[2048] = {0};
		va_list arglist;
		va_start(arglist, fmt);
		vsprintf(buffer, fmt, arglist);
		va_end(arglist);
	
		void (*tt)(char*);
		tt = ManagedTraceCallback;
		(*tt)(buffer);

		doing_trace = FALSE ;
	}
}

void Warning(char *fmt, ...)
{
	if (GetLogLevel() < LOGLEVEL_WARNING)
	{
		return ;
	}

	static BOOL doing_warning = FALSE ;
	if ( fmt )
	{
		if (doing_warning == TRUE)
		{
			return;
		}

		doing_warning = TRUE ;
 
		char buffer[2048] = {0};
		va_list arglist;
		va_start(arglist, fmt);
		vsprintf(buffer, fmt, arglist);
		va_end(arglist);
	
		void (*tt)(char*);
		tt = ManagedWarningCallback;
		(*tt)(buffer);

		doing_warning = FALSE ;
	}
}


// ******************************************************************************************
void Error(char* fmt, ...)
{
	if (GetLogLevel() < LOGLEVEL_ERROR)
	{
		return ;
	}

//	static BOOL doing_error = FALSE ;

//	if (doing_error == TRUE) 
	//{
	//	return;
	//}
	
	//doing_error = TRUE ;

	char buffer[2048] = {0};
	va_list arglist;
	va_start(arglist, fmt);
	vsprintf(buffer, fmt, arglist);
	va_end(arglist);

	void (*tt)(char*);	
	tt = ManagedErrorCallback;
	(*tt)(buffer);

	//doing_error = FALSE ;	
}


// ******************************************************************************************

typedef void (*tt)(int);

void FatalError(char* fmt, ...)
{
	if (GetLogLevel() < LOGLEVEL_FATALERROR)
	{
		return ;
	}


	char buffer[2048] = {0 };
	va_list arglist;
	va_start(arglist, fmt);
	vsprintf(buffer, fmt, arglist);
	va_end(arglist);

	void (*tt)(char*);
	tt = ManagedFatalErrorCallback;
	(*tt)(buffer);

}

// ******************************************************************************************
void PopupError(char* fmt, ...)
{
	char buffer[2048] = {0};
	va_list arglist;
	va_start(arglist, fmt);
	vsprintf(buffer, fmt, arglist);
	va_end(arglist);

	void (*tt)(char*);
	tt = ManagedPopupErrorCallback;
	(*tt)(buffer);

}

// ******************************************************************************************
void DeclareEncodeCheckpoint(char pos)
{
   if (TraceEncode()==0) return ;
   void (*tt)(char);
   tt = ManagedDebugAddEncodePositionCallback;
   (*tt)(pos);
}






