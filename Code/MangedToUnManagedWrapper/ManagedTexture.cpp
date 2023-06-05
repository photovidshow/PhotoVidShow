#include "ManagedTexture.h"

using namespace Direct3D;

namespace MangedToUnManagedWrapper
{
namespace ManagedDirect3D
{

ManagedTexture::ManagedTexture()
{
	mTexture = new Texture();
}

ManagedTexture::~ManagedTexture()
{
	Cleanup();
}


Texture* ManagedTexture::GetTexture()
{
	return mTexture;
}

void ManagedTexture::Cleanup()
{
	if (mTexture)
	{
		delete mTexture;
		mTexture = 0;
	}
}
 

void ManagedTexture::Init(System::IntPtr data, unsigned int width, unsigned int height, unsigned int d_pitch, unsigned int bytes_per_pixel, bool forcePower2Texture, bool createMipMaps)
{
	mTexture->Init((unsigned int*) data.ToPointer(),width, height, d_pitch, bytes_per_pixel, forcePower2Texture, createMipMaps);
}


}

}