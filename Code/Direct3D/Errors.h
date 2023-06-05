#ifndef UNMANAGED_ERRORS_INCLUDE
#define UNMANAGED_ERRORS_INCLUDE

#include <stdio.h>


// basic unmanged Error commands

extern void Trace(char *fmt, ...);
extern void Warning(char *fmt, ...);
extern void Error(char* fmt, ...);
extern void FatalError(char* fmt, ...);
extern void PopupError(char* fmt, ...);

extern void check_return(HRESULT hr, char* file, int line, const char* string);

#define CHECK_RETURN(a,s) check_return(a, __FILE__,__LINE__, s);

#define CHECK_RETURN_EXIT(a,s)								\
{															\
	HRESULT tresult = a;									\
	check_return(tresult, __FILE__,__LINE__, s);			\
	if (FAILED(tresult))									\
	{														\
		return;												\
	}														\
}															

#define Once(a) 						\
{										\
	static int done = 0;				\
	if (done==0) { done =1 ; (a) ;}		\
}

#define TraceOnce Once(Trace
#define WarningOnce Once(Warning
#define ErrorOnce Once(Error


#endif 