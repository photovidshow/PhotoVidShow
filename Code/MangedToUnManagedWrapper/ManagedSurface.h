#pragma once

#include "Surface.h"

namespace MangedToUnManagedWrapper
{
namespace ManagedDirect3D
{

public value struct ManagedLockRect
{
public:

	int pBits;
	int Pitch ;
};

public ref class ManagedSurface
{
public:

	 ManagedSurface();
	 ~ManagedSurface();
	 void Init(unsigned int width, unsigned int height, int usage, unsigned int bytes_per_pixel, int doMipMapping, bool forcePower2TextureOnMipMapTextures);
	 void Cleanup();
	 bool HasD3DTexture();

	 Direct3D::Surface* GetSurface();

	 ManagedLockRect Lock();
	 void Unlock();

private:

	Direct3D::Surface* mSurface;

};

}

}

