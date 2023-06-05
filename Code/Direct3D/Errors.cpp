
#include "DX90SDK/Include/d3dx9.h" 
#include "DX90SDK/Include/dxerr9.h"
#include "Errors.h"
#include <stdio.h>
int (WINAPIV * __vsnwprintf)(wchar_t *, size_t, const wchar_t*, va_list) = _vsnwprintf;


//***************************************************************************
void check_return(HRESULT hr, char* file, int line, const char* string)
{
	if (FAILED(hr))
	{
		const char* err_str =DXGetErrorString9A(hr);
		const char* erro_str_dec =DXGetErrorDescription9A(hr);
		Error("File:%s Line:%d ,%s, %s, %s", file, line, string , err_str, erro_str_dec);
	}
}

