#ifndef UNMANAGED_ERRORS_INCLUDE
#define UNMANAGED_ERRORS_INCLUDE


#include <stdio.h>

// basic unmanged Error commands

typedef void (*StringLogCallback)(char*);
typedef void (*CharLogCallback)(char);
typedef int (*IntCallback)();

extern void UnmanagedSetLogCallbacks(StringLogCallback trace,
							StringLogCallback warning,
							StringLogCallback error,
							StringLogCallback fatal_error,
							StringLogCallback popup_error,
							CharLogCallback DebugAddEncodePosition,
							IntCallback LogLevel,
							IntCallback TraceEncode);

extern void Trace(char *fmt, ...);
extern void Warning(char *fmt, ...);
extern void Error(char* fmt, ...);
extern void FatalError(char* fmt, ...);
extern void PopupError(char* fmt, ...);

enum LogLevel
{
	LOGLEVEL_FATALERROR=0,
	LOGLEVEL_ERROR,
	LOGLEVEL_WARNING,
	LOGLEVEL_TRACE
};

extern LogLevel GetLogLevel();

extern void DeclareEncodeCheckpoint(char pos);

typedef int BOOL;
#define NULL 0
#define FALSE 0
#define TRUE 1


#define Once(a) 						\
{										\
	static int done = 0;				\
	if (done==0) { done =1 ; (a) ;}		\
}

#define TraceOnce Once(Trace
#define WarningOnce Once(Warning
#define ErrorOnce Once(Error


#endif 