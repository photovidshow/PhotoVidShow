#pragma once

#include "Texture.h"

namespace MangedToUnManagedWrapper
{
namespace ManagedDirect3D
{
public ref class ManagedTexture
{
public:

	 ManagedTexture();
	 ~ManagedTexture();
	 void Init(System::IntPtr data, unsigned int width, unsigned int height, unsigned int d_pitch, unsigned int bytes_per_pixel, bool forcePower2Texture, bool createMipMaps);
	 void Cleanup();
	 Direct3D::Texture* GetTexture();

private:

	Direct3D::Texture* mTexture;

	// TODO: Add your methods for this class here.
};

}

}

