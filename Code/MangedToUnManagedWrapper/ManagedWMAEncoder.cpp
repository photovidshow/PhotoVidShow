// AUDIO Recorder

#include "enc_wma.h"
#include <string>

#include "ManagedErrors.h"
#include <iostream>
using namespace std; 
#using <mscorlib.dll>


#include <tchar.h>

using namespace System;
using namespace System::Text;
using namespace System::Runtime::InteropServices; 
using namespace ManagedCore;
using namespace std;

extern std::string ManagedToSTL(String^ a);

#include "ManagedObject.h"

namespace MangedToUnManagedWrapper
{ 
	
    public ref class CManagedWMAEncoder 
	{
	public:
	
		CManagedWMAEncoder();

		~CManagedWMAEncoder();

		void Encode(String^ inputFilename, String^ outputFilename);
	};



	CManagedWMAEncoder::CManagedWMAEncoder()
	{
	}

	CManagedWMAEncoder::~CManagedWMAEncoder()
	{
	}

	void CManagedWMAEncoder::Encode(String^ inputFilename, String^ outputFilename)
	{
		std::string input = ManagedToSTL(inputFilename);
		std::string output = ManagedToSTL(outputFilename);
		do_wma_encode((char*)input.c_str(), (char*)output.c_str());
	}


}


