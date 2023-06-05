#include "ManagedSurface.h"

using namespace Direct3D;

namespace MangedToUnManagedWrapper
{
namespace ManagedDirect3D
{


ManagedSurface::ManagedSurface() :
mSurface(0)
{
	mSurface = new Surface;
}

ManagedSurface::~ManagedSurface()
{
	Cleanup();
}

Surface* ManagedSurface::GetSurface()
{
	return mSurface;
}

void ManagedSurface::Cleanup()
{
	if (mSurface)
	{
		delete mSurface;
		mSurface = 0;
	}
}


ManagedLockRect ManagedSurface::Lock()
{
	ManagedLockRect mlr;
	if (mSurface)
	{
		LockedRect lr = mSurface->Lock();
		mlr.Pitch = lr.Pitch;
		mlr.pBits = (int) lr.pBits;
	}
	else
	{
		mlr.Pitch = 0;
		mlr.pBits = 0;
	}

	return mlr;
}

void ManagedSurface::Unlock()
{
	if (mSurface)
	{
		mSurface->Unlock();
	}
}

void ManagedSurface::Init(unsigned int width, unsigned int height, int usage, unsigned int bytesPerPixel, int doMipMapping, bool forcePower2TextureOnMipMapTextures)
{
	mSurface->Init(width, height, (TEXTURE_USAGE) usage, bytesPerPixel, doMipMapping, forcePower2TextureOnMipMapTextures);
}

bool ManagedSurface::HasD3DTexture()
{
	return mSurface->HasD3DTexture();
}

}

}