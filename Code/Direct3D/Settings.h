#pragma once


namespace Direct3D 
{
// Class used to store D3D settings
class Settings
{
public:

	Settings();

	bool GetUseManagedPoolForNormalTextures();
	void SetUseManagedPoolForNormalTextures(bool value);

private:

	bool mUseManagedPoolForNormalTextures;		// If true the engine will use D3DPOOL_MANAGED for textures (uses 2x more memory but perhaps more stable when developing)
	
};

}
