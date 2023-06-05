#ifndef MANAGED_BUFFER_INCLUDE
#define MANAGED_BUFFER_INCLUDE

#include <iostream>
#include <string>
using namespace std; 
#using <mscorlib.dll>

#include "ManagedObject.h"

class CBuffer;

namespace MangedToUnManagedWrapper
{ 
	public ref class CManagedBuffer : public ManagedObject
	{
	public:

		CBuffer* mBuffer;

		CManagedBuffer() ;
		~CManagedBuffer();
		void CleanUp();
	};
}


#endif 


