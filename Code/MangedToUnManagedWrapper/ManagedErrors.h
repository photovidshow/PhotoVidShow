#using <mscorlib.dll>
using namespace System;
#include "unmanagederrors.h"
#include <string>
using namespace ManagedCore;


extern System::String^ STLToManaged(std::string stl);

extern void ManagedTrace(char* message);
extern void ManagedError(char* message);
extern void ManagedError(String^ message);
extern void ManagedWarning(char* message);
extern void ManagedFatalError(char* message);
extern void ManagedPopupError(char* a);
extern void ManagedPopupWarning(char* a);
extern void ManagedDebugAddEncodePosition(char pos);
extern int  ManagedLogLevel();
extern int  ManagedTraceEncode();

