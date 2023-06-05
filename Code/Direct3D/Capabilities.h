#pragma once

struct IDirect3D9;
struct IDirect3DDevice9;

namespace Direct3D 
{
// Class used to store D3D capabilities that PhotoVidShow will need or is intrested in
class Capabilities
{
public:

	Capabilities();
	bool InitFromDevice(IDirect3D9* direct3D, IDirect3DDevice9* device);

	bool GetCanDoPixelShader20();
	bool GetCanDo64BitTextures();
	bool GetCanDoTrilinearFiltering();
	bool GetCanDoAutoMipMappingCreation();
	bool GetCanDoNonPower2Textures();
	int GetMaxTextureWidth();
	int GetMaxTextureHeight();

private:

	bool mCanDoPixelShader20;		
	bool mCanDo64BitTextures;
	bool mGetCanDoTrilinearFiltering;
	bool mCanDoAutoMipMappingCreation;
	bool mCanDoNonPower2Textures;			// This also includes when doing MipMapped textures
	int mMaxTextureWidth;
	int mMaxTextureHeight;

	
};

}
