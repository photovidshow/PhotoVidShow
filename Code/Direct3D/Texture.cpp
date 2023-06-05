#include "StdAfx.h"
#include "Texture.h"
#include "Direct3DDevice.h"
#include "Errors.h"
#include "DX90SDK/Include/d3dx9.h" 

namespace Direct3D 
{

// ****************************************************************************************************
Texture::Texture()
{
	mTexture = NULL;
	mImageWidth=0;
	mImageHeight=0;
	mRawSurfaceWidth=0;
	mRawSurfaceHeight=0;
	mCurrentLockedSurface=NULL;
}

// ****************************************************************************************************
// Creates a texture with given image data to populate it with
void Texture::Init(unsigned int* ptr, unsigned int width, unsigned int height, unsigned int d_pitch, unsigned int bytes_per_pixel, bool forcePower2Texture, bool createMipMaps)
{
	if (mTexture != NULL) 
	{	
		Error("Texture alread initialised");
		return ;
	}

	if (bytes_per_pixel!=4)
	{
		Error("Only support 32 bit textures int Texture::Init with data ptr");
		return;
	}

	mImageWidth = width;
	mImageHeight = height;

	int D3DUsage = 0;
	int mipMapLevels =1;

	if (createMipMaps)
	{
		mipMapLevels=5;
		D3DUsage = D3DUSAGE_AUTOGENMIPMAP;
	}

	if (forcePower2Texture == true)
	{
		width = GetNextPower2Size(width);
		height = GetNextPower2Size(height);
		if (width > height) height = width;
		if (height > width) width = height;
	}

	// should be overwritten later
	mRawSurfaceWidth=width;
	mRawSurfaceHeight=height;

	IDirect3DDevice9* device = Direct3DDevice::GetInstance()->GetRawDevice();

	D3DPOOL pool = D3DPOOL_DEFAULT;

	if (Direct3DDevice::GetInstance()->GetSettings().GetUseManagedPoolForNormalTextures() == true)
	{
		pool = D3DPOOL_MANAGED;
	}

	// Used mostly for images store from file used as decorations and slides
	CHECK_RETURN_EXIT(D3DXCreateTexture(device, width ,height, mipMapLevels, D3DUsage, D3DFMT_A8R8G8B8, pool, &mTexture)," Failed to create texture in default poo2l");

	IDirect3DSurface9* pSurface = NULL;

	CHECK_RETURN(mTexture->GetSurfaceLevel(0, &pSurface),"Failed to get surface of default texture");
	SetupRawDimensionsAttributesFromSurface(pSurface);

	if (pool==D3DPOOL_DEFAULT)
	{	
		pSurface->Release();
		IDirect3DTexture9* mLocalTexture = NULL;
		CHECK_RETURN(D3DXCreateTexture(device, mImageWidth ,mImageHeight, 1, 0, D3DFMT_A8R8G8B8, D3DPOOL_SYSTEMMEM, &mLocalTexture),"Failed to create texture in system memory");	
		if (SUCCEEDED(mLocalTexture->GetSurfaceLevel(0, &pSurface)))
		{
			CopyDataToSurface(pSurface, ptr, d_pitch, bytes_per_pixel);
			pSurface->Release();
		}

		CHECK_RETURN(device->UpdateTexture(mLocalTexture, mTexture),"Failed up update texture");
		mLocalTexture->Release();
	}
	else
	{
		CopyDataToSurface(pSurface, ptr, d_pitch, bytes_per_pixel);
		pSurface->Release();
	}
}

// ****************************************************************************************************
void Texture::CopyDataToSurface(IDirect3DSurface9* pSurface, unsigned int* ptr, unsigned int d_pitch, unsigned int bytes_per_pixel)
{
    D3DLOCKED_RECT lr;
	RECT rect;
	rect.left = 0;
	rect.right = mImageWidth;
	rect.top = 0;
	rect.bottom = mImageHeight;
	// Lock the surface to write pixels
	if (SUCCEEDED(pSurface->LockRect( &lr, &rect, 0 ) ) )
	{
		int diff = (lr.Pitch - (mImageWidth*bytes_per_pixel)) / bytes_per_pixel;

		if (mImageWidth == mRawSurfaceWidth && mImageHeight == mRawSurfaceHeight && diff==0)
		{
			memcpy(lr.pBits, ptr, lr.Pitch* mImageHeight);
		}
		else if (mImageWidth <= mRawSurfaceWidth && mImageHeight <=mRawSurfaceHeight)
		{
			// This is surface has to be pow2 size and image size is not (and the drivers can't handle that)
			int ptrStrideDWord = mImageWidth+d_pitch;
			int pBitsStrideDword = lr.Pitch / bytes_per_pixel;
			unsigned int* pBits = (unsigned int*)lr.pBits;

			for (int i=0;i<mImageHeight;i++)
			{
				memcpy(pBits, ptr, mImageWidth*bytes_per_pixel);
				ptr+=ptrStrideDWord;
				pBits+=pBitsStrideDword;
			}
		}

		pSurface->UnlockRect();
	}
}


// ****************************************************************************************************
//  Creates a texture with a given texture usage
//  SYSTEM_MEMORY usage will not have mipmaps no matter what doMipMapping is set to
//  
void Texture::Init(unsigned int width, unsigned int height, TEXTURE_USAGE usage, unsigned int bytes_per_pixel, int doMipMapping, bool forcePower2TextureOnMipMapTextures)
{
	if (mTexture != NULL) return ;

	mImageWidth = width;
	mImageHeight = height;

	// should be overwritten later
	mRawSurfaceWidth=width;
	mRawSurfaceHeight=height;

	int D3DUsage = 0;
	int mipMapLevels =1;

	if (doMipMapping==1)
	{
		mipMapLevels=5;
		D3DUsage = D3DUSAGE_AUTOGENMIPMAP;

		if (forcePower2TextureOnMipMapTextures==true)
		{
			// To force test power2 textures
			width = GetNextPower2Size(width);
			height = GetNextPower2Size(height);
			if (width > height) height = width;
			if (height > width) width = height;
		}
	}

	IDirect3DDevice9* device = Direct3DDevice::GetInstance()->GetRawDevice();

	D3DFORMAT format = D3DFMT_A8R8G8B8;

	if (bytes_per_pixel ==8)
	{
		format = D3DFMT_A16B16G16R16;
	}
	
	switch (usage)
	{
		case MANAGED:
		{
			// Used by things like video textures,
			CHECK_RETURN(D3DXCreateTexture(device, width ,height, mipMapLevels, D3DUsage, format, D3DPOOL_MANAGED, &mTexture), "Could not create managed pool texture");
			break;
		} 
		case RENDER_TARGET:
		{
			CHECK_RETURN(D3DXCreateTexture(device, width ,height, mipMapLevels, D3DUSAGE_RENDERTARGET | D3DUsage, format, D3DPOOL_DEFAULT, &mTexture), "Could not create surface with RENDER_TARGET usage");

			break;
		}
		case RENDER_TARGET_WITH_NO_ALPHA:
		{
			D3DFORMAT format = D3DFMT_X8R8G8B8;
			CHECK_RETURN(D3DXCreateTexture(device, width ,height, mipMapLevels, D3DUSAGE_RENDERTARGET | D3DUsage, format, D3DPOOL_DEFAULT, &mTexture), "Could not create surface with RENDER_TARGET_WITH_NO_ALPHA usage");

			break;
		}
		case SYSTEM_MEMORY:
		{
			// This can not have mip mapping
			format = D3DFMT_X8R8G8B8;
			CHECK_RETURN(device->CreateTexture(width, height, 1, 0 , format, D3DPOOL_SYSTEMMEM, &mTexture, NULL), "Could not create surface in system memory") ;
			break;
		}
		default:
		{
			Error("Unknown or unsupported usage when creating texture");
			break;
		}
	}

	if (mTexture !=NULL)
	{
		IDirect3DSurface9* pSurface = NULL;

		HRESULT rslt = mTexture->GetSurfaceLevel(0, &pSurface);
		
		if (SUCCEEDED(rslt))
		{
			SetupRawDimensionsAttributesFromSurface(pSurface);
			
			pSurface->Release();
		}
		else
		{
			Error("Could not get surface level 0 from texture");
		}
	}
	else
	{
		Error("Null texture after init, ran out of memory? %d,%d", width, height);
	}
}

// ****************************************************************************************************
// Input: source coordinates as in 0..1 
// Converts them into the real uv coordinates, as image may only cover part of the texture.
// Also does the UV half pixel correction
void Texture::ConvertSrcToUVCoordinates(float *src_x, float *src_y, float *src_width, float *src_height)
{ 
	float widthRatio = ((float)mImageWidth) / ((float)mRawSurfaceWidth);
	float heightRatio = ((float)mImageHeight) / ((float)mRawSurfaceHeight);

	float realSrcX = (*src_x) * widthRatio;
	float realSrcY = (*src_y) * heightRatio;

	*src_x = realSrcX + (0.5f / mRawSurfaceWidth );
	*src_y = realSrcY + (0.5f / mRawSurfaceHeight ) ;

	float realSrcWidth = (*src_width) * widthRatio;
	float realSrcHeight = (*src_height) * heightRatio;

 	*src_width = realSrcWidth + (0.5f / mRawSurfaceWidth ) ;
	*src_height = realSrcHeight + (0.5f / mRawSurfaceHeight ) ;
}

// ****************************************************************************************************
// Lock the texture and return D3DLOCKED_RECT
LockedRect Texture::Lock()
{
	if (mCurrentLockedSurface != NULL) 
	{
		Unlock();
	}

	LockedRect lr;
	memset(&lr, 0, sizeof(lr));

	if (mTexture == NULL)
	{
		Error("No texture on call to lock");
		return lr;
	}

	bool locked = false;

	HRESULT rslt = mTexture->GetSurfaceLevel(0, &mCurrentLockedSurface);
	if (SUCCEEDED(rslt))
	{
		D3DLOCKED_RECT d3dlr;
		RECT rect;
		rect.left = 0;
		rect.right = mImageWidth;
		rect.top = 0;
		rect.bottom = mImageHeight;
		// Lock the surface to write pixels
		rslt = mCurrentLockedSurface->LockRect(&d3dlr, &rect, 0);
		if ( SUCCEEDED(rslt) )
		{
			lr.pBits = d3dlr.pBits;
			lr.Pitch = d3dlr.Pitch;
			locked = true;
		}
		else
		{
			Error("Call to lock failed: %d", rslt);
		}
	}
	else
	{
		Error("Could not get surface level 0 from texture on call to lock: %d", rslt);
	}
	
	if (locked == false && mCurrentLockedSurface != NULL)
	{
		mCurrentLockedSurface->Release();
		mCurrentLockedSurface = NULL;
	}

	return lr;
}

// ****************************************************************************************************
// Set up are raw width,height size form the level 0 surface of the texture
void Texture::SetupRawDimensionsAttributesFromSurface(IDirect3DSurface9* pSurface)
{
	D3DSURFACE_DESC surface_desc;
	memset(&surface_desc, 0, sizeof(D3DSURFACE_DESC));
	pSurface->GetDesc(&surface_desc);
	mRawSurfaceWidth = surface_desc.Width;
	mRawSurfaceHeight = surface_desc.Height;
}


// ****************************************************************************************************
int Texture::GetNextPower2Size(int inputSize)
{
	int power=1;
	while (power<inputSize)
	{
		power<<=1;
	}
	return power;
}



// ****************************************************************************************************
void Texture::Unlock()
{
	if (mCurrentLockedSurface == NULL) return;

	mCurrentLockedSurface->UnlockRect();
	mCurrentLockedSurface->Release();
	mCurrentLockedSurface = NULL;
}


// ****************************************************************************************************
unsigned int Texture::GetWidth()
{
	return mImageWidth;
}

// ****************************************************************************************************
unsigned int Texture::GetHeight()
{
	return mImageHeight;
}

// ****************************************************************************************************
IDirect3DTexture9* Texture::GetD3DTexture()
{
	return mTexture;
}


// ****************************************************************************************************
unsigned int Texture::GetRawSurfaceWidth()
{
	return mRawSurfaceWidth;
}


// ****************************************************************************************************
unsigned int Texture::GetRawSurfaceHeight()
{
	return mRawSurfaceHeight;
}


// ****************************************************************************************************
Texture::~Texture(void)
{
	if (mCurrentLockedSurface != NULL)
	{
		Unlock();
	}

	if (mTexture)
	{
		mTexture->Release();
		mTexture=NULL;
	}
}

}
