#include "StdAfx.h"
#include "Surface.h"
#include "Texture.h"
#include "DX90SDK/Include/d3dx9.h" 

namespace Direct3D 
{

//***************************************************************************
Surface::Surface() :
	mD3DSurface(NULL),
	mTexture(NULL)
{
}

	//***************************************************************************
void Surface::Init(unsigned int width, unsigned int height, TEXTURE_USAGE usage, unsigned int bytesPerPixel, int doMipMapping, bool forcePower2TextureOnMipMapTextures)
{
	mTexture = new Texture;
	mTexture->Init(width, height, usage, bytesPerPixel, doMipMapping, forcePower2TextureOnMipMapTextures);

	// Get surface information
	if (mTexture->GetD3DTexture() != NULL)
	{
		mTexture->GetD3DTexture()->GetSurfaceLevel(0,&mD3DSurface);
	}
}


//***************************************************************************
IDirect3DSurface9* Surface::GetD3DSurface()
{
	return mD3DSurface;
}

//***************************************************************************
Texture* Surface::GetTexture()
{
	return mTexture;
}

//***************************************************************************
LockedRect Surface::Lock()
{
	if (mTexture)
	{
		return mTexture->Lock();
	}

	LockedRect lr;
	return lr;
}


//***************************************************************************
void Surface::Unlock()
{
	if (mTexture)
	{
		mTexture->Unlock();
	}
}

//***************************************************************************
Surface::~Surface(void)
{
	if (mD3DSurface)
	{
		mD3DSurface->Release();
		mD3DSurface=NULL;
	}

	if (mTexture)
	{
		delete mTexture;
		mTexture=NULL;
	}
}

//***************************************************************************
bool Surface::HasD3DTexture()
{
	if (mTexture==NULL) return false;
	if (mTexture->GetD3DTexture() == NULL) return false;
	return true;
}

}
