#include "Settings.h"
#include "Direct3DDevice.h"

using namespace Direct3D;


namespace MangedToUnManagedWrapper
{
namespace ManagedDirect3D
{


// ************************************************************************************************************
public ref class ManagedDirect3DSettings
{
public:

	ManagedDirect3DSettings();
	bool GetUseManagedPoolForNormalTextures();
	void SetUseManagedPoolForNormalTextures(bool value);


};

// ************************************************************************************************************
ManagedDirect3DSettings::ManagedDirect3DSettings()
{
}


// ************************************************************************************************************
bool ManagedDirect3DSettings::GetUseManagedPoolForNormalTextures()
{
	return Direct3DDevice::GetInstance()->GetSettings().GetUseManagedPoolForNormalTextures();
}

// ************************************************************************************************************
void ManagedDirect3DSettings::SetUseManagedPoolForNormalTextures(bool value)
{
	Direct3DDevice::GetInstance()->GetSettings().SetUseManagedPoolForNormalTextures(value);
}


}
}
