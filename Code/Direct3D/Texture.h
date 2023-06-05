#pragma once

#include "MonitoredObject.h"

struct IDirect3DTexture9;
struct IDirect3DSurface9;

namespace Direct3D 
{

enum TEXTURE_USAGE
{
	MANAGED,		// Use to create textures than can be locked and written to	(e.g. normal picture decoration)
	RENDER_TARGET,  // Use to create a render target							(e.g. Group decoration, filter decoration input)
	SYSTEM_MEMORY,   // Use to copy from render target to system memory			(e.g. Create bitmaps, encoding to pass to encoder)
	RENDER_TARGET_WITH_NO_ALPHA	// use this if you want to use CopyDefaultSurfaceToDynamicSurface
};


class LockedRect
{
public:
	LockedRect() :pBits(0), Pitch(0) {}
	void* pBits;
	int Pitch;
};

class Texture : MonitoredObject
{
public:
	Texture();

	void Init(unsigned int* ptr, unsigned int width, unsigned int height, unsigned int d_pitch, unsigned int bytes_per_pixel, bool forcePower2Texture, bool createMipMaps); 
	void Init(unsigned int width, unsigned int height, TEXTURE_USAGE usage, unsigned int bytes_per_pixel, int doMipMapping, bool forcePower2TextureOnMipMapTextures);
	void clear_with_alpha();
	LockedRect Lock();
	void Unlock();

	unsigned int GetWidth();				// Returns texture width, i.e. the size passed in at construction 
	unsigned int GetHeight();				// Returns texture height, i.e. the size passed in at construction 

	unsigned int GetRawSurfaceWidth();		// Return underlying raw D3D surface level 0 width (as may be power2 size)
	unsigned int GetRawSurfaceHeight();		// Return underlying raw D3D surface level 0 height (as may be power2 size)


	void ConvertSrcToUVCoordinates(float *src_x, float *src_y, float *src_width, float *src_height);

	IDirect3DTexture9* GetD3DTexture();
	~Texture();

private:

	void CopyDataToSurface(IDirect3DSurface9* pSurface, unsigned int* ptr, unsigned int d_pitch, unsigned int bytes_per_pixel);
	int GetNextPower2Size(int inputSize);
	void SetupRawDimensionsAttributesFromSurface(IDirect3DSurface9* pSurface);

	unsigned int mImageWidth;
	unsigned int mImageHeight;
	unsigned int mRawSurfaceWidth;
	unsigned int mRawSurfaceHeight;

	IDirect3DTexture9* mTexture;

	// Temp handle when doing lock/unlock
	IDirect3DSurface9* mCurrentLockedSurface;
};

}

