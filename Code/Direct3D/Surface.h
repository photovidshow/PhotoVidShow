#pragma once

#include "Texture.h"
#include "ActiveReader.h"

struct IDirect3DSurface9;

namespace Direct3D 
{
// Represents a D3D surface.  Only dynamic surfaces can be locked and only non-dynamic surfaces can be rendered to
class Surface
{
public:
	Surface();
	~Surface();

	void Init(unsigned int width, unsigned int height, TEXTURE_USAGE usage, unsigned int bytesPerPixel, int doMipMapping, bool forcePower2TextureOnMipMapTextures);
	LockedRect Lock();
	void Unlock();
	bool HasD3DTexture();

	IDirect3DSurface9* GetD3DSurface();

	Texture*	GetTexture();

private:

	IDirect3DSurface9* mD3DSurface;
	Texture* mTexture;	// Render surface as a texture
};

}
