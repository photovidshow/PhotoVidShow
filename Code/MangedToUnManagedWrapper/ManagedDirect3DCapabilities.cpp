#include "Settings.h"
#include "Direct3DDevice.h"

using namespace Direct3D;


namespace MangedToUnManagedWrapper
{
namespace ManagedDirect3D
{


// ************************************************************************************************************
public ref class ManagedDirect3DCapabilities
{
public:

	ManagedDirect3DCapabilities();
	
	bool GetCanDoPixelShader20();
	bool GetCanDo64BitTextures();
	bool GetCanDoTrilinearFiltering();
	bool GetCanDoAutoMipMappingCreation();
	bool GetCanDoNonPower2Textures();
	int GetMaxTextureWidth();
	int GetMaxTextureHeight();


};

// ************************************************************************************************************
ManagedDirect3DCapabilities::ManagedDirect3DCapabilities()
{
}

// ************************************************************************************************************
bool ManagedDirect3DCapabilities::GetCanDoPixelShader20()
{
	return Direct3DDevice::GetInstance()->GetCapabilities().GetCanDoPixelShader20();
}

// ************************************************************************************************************
bool ManagedDirect3DCapabilities::GetCanDo64BitTextures()
{
	return Direct3DDevice::GetInstance()->GetCapabilities().GetCanDo64BitTextures();
}

// ************************************************************************************************************
bool ManagedDirect3DCapabilities::GetCanDoTrilinearFiltering()
{
	return Direct3DDevice::GetInstance()->GetCapabilities().GetCanDoTrilinearFiltering();
}

// ************************************************************************************************************
bool ManagedDirect3DCapabilities::GetCanDoAutoMipMappingCreation()
{
	return Direct3DDevice::GetInstance()->GetCapabilities().GetCanDoAutoMipMappingCreation();
}

// ************************************************************************************************************
bool ManagedDirect3DCapabilities::GetCanDoNonPower2Textures()
{
	return Direct3DDevice::GetInstance()->GetCapabilities().GetCanDoNonPower2Textures();
}

// ************************************************************************************************************
int ManagedDirect3DCapabilities::GetMaxTextureWidth()
{
	return Direct3DDevice::GetInstance()->GetCapabilities().GetMaxTextureWidth();
}

// ************************************************************************************************************
int ManagedDirect3DCapabilities::GetMaxTextureHeight()
{
	return Direct3DDevice::GetInstance()->GetCapabilities().GetMaxTextureHeight();
}

}
}
