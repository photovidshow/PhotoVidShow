#pragma once

#include "PixelShaderEffect.h"
#include "ManagedTexture.h"
#include "ManagedSurface.h"

namespace MangedToUnManagedWrapper
{
namespace ManagedDirect3D
{

public ref class ManagedPixelShaderEffect
{
public:

	ManagedPixelShaderEffect();
	~ManagedPixelShaderEffect();
	void Init(System::String^ s);
	void Cleanup();
	void SetParameter(int index, float value);
	void SetTexture1(ManagedTexture^ texture);
	void SetTexture1(ManagedSurface^ surface);

	Direct3D::PixelShaderEffect* GetPixelShaderEffect();

private:

	Direct3D::PixelShaderEffect* mPixelShaderEffect;

};

}

}
