#ifndef UNMANAGED_ERRORS_INCLUDE
#define UNMANAGED_ERRORS_INCLUDE

// basic unmanged Error commands

extern void Trace(char *fmt, ...);
extern void Warning(char *fmt, ...);
extern void Error(char* fmt, ...);
extern void FatalError(char* fmt, ...);
extern void PopupError(char* fmt, ...);

													
#define Once(a) 						\
{										\
	static int done = 0;				\
	if (done==0) { done =1 ; (a) ;}		\
}

#define TraceOnce Once(Trace
#define WarningOnce Once(Warning
#define ErrorOnce Once(Error


#endif 