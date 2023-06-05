#include "ManagedErrors.h"


namespace MangedToUnManagedWrapper
{ 
	public ref class CManagedErrors 
	{
		public:

		static void SetUnmanagedCallbacks()
		{

			UnmanagedSetLogCallbacks(ManagedTrace,
									 ManagedWarning,
									 ManagedError,
									 ManagedFatalError,
									 ManagedPopupError,
									 ManagedDebugAddEncodePosition,
									 ManagedLogLevel,
									 ManagedTraceEncode);
		}
	};
}

void ManagedError(char* a)
{
	String^ aa = STLToManaged(std::string(a));

	ManagedCore::CDebugLog::GetInstance()->Error(aa);

}

void ManagedError(String^ a)
{
	ManagedCore::CDebugLog::GetInstance()->Error(a);
}

void ManagedTrace(char* a)
{
	String^ aa = STLToManaged(std::string(a));

	ManagedCore::CDebugLog::GetInstance()->Trace(aa);

}

void ManagedFatalError(char* a)
{
	String^ aa = STLToManaged(std::string(a));

	ManagedCore::CDebugLog::GetInstance()->FatalError(aa);

}

void ManagedPopupError(char* a)
{
	String^ aa = STLToManaged(std::string(a));

	ManagedCore::CDebugLog::GetInstance()->PopupError(aa);
}


void ManagedPopupWarning(char* a)
{
	String^ aa = STLToManaged(std::string(a));
	ManagedCore::CDebugLog::GetInstance()->PopupWarning(aa);
}



void ManagedWarning(char* a)
{
	String^ aa = STLToManaged(std::string(a));

	ManagedCore::CDebugLog::GetInstance()->Warning(aa);

}

void ManagedDebugAddEncodePosition(char pos)
{
   ManagedCore::CDebugLog::GetInstance()->DebugAddEncodePosition(pos);
}


int  ManagedTraceEncode()
{
	return  ManagedCore::CDebugLog::GetInstance()->TraceEncode==true ? 1 : 0;
}

int ManagedLogLevel()
{
	return (int) ManagedCore::CDebugLog::GetInstance()->LogLevel;
}


