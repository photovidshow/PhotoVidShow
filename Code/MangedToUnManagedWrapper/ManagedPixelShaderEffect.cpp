#include "ManagedPixelShaderEffect.h"
#include <string>

using namespace Direct3D;
using namespace System;
using namespace System::Runtime::InteropServices; 


namespace MangedToUnManagedWrapper
{
namespace ManagedDirect3D
{

	
std::string ManagedToSTL2(System::String^ managed)
{
    //get a pointer to an array of ANSI chars
	System::IntPtr chars =  Marshal::StringToHGlobalAnsi( managed ); 

    //assign the array to an STL string
    std::string stl = (char*) chars.ToPointer(); 

    //free the memory used by the array
    //since the array is not managed, it will not be claimed by the garbage collector
	Marshal::FreeHGlobal( chars ); 

    return stl;
} 


ManagedPixelShaderEffect::ManagedPixelShaderEffect() :
mPixelShaderEffect(0)
{
}

ManagedPixelShaderEffect::~ManagedPixelShaderEffect()
{
	Cleanup();
}


void ManagedPixelShaderEffect::Cleanup()
{
	if (mPixelShaderEffect)
	{
		delete mPixelShaderEffect;
		mPixelShaderEffect = 0;
	}
}

Direct3D::PixelShaderEffect* ManagedPixelShaderEffect::GetPixelShaderEffect()
{
	return mPixelShaderEffect;
}


void ManagedPixelShaderEffect::Init(String^ s)
{
	std::string ss = ManagedToSTL2(s);

	mPixelShaderEffect = new PixelShaderEffect((char*)ss.c_str() );
}

void ManagedPixelShaderEffect::SetParameter(int index, float value)
{
	if (mPixelShaderEffect)
	{
		mPixelShaderEffect->SetParameter(index, value);
	}
}

void ManagedPixelShaderEffect::SetTexture1(ManagedTexture^ texture)
{
	if (mPixelShaderEffect && texture != nullptr)
	{
		mPixelShaderEffect->SetTexture1(texture->GetTexture());
	}
}


void ManagedPixelShaderEffect::SetTexture1(ManagedSurface^ surface)
{
	if (mPixelShaderEffect && surface != nullptr)
	{
		mPixelShaderEffect->SetTexture1(surface->GetSurface());
	}
}


}

}