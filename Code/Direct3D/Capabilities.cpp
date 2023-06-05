#include "StdAfx.h"
#include "Capabilities.h"
#include "DX90SDK/Include/d3dx9.h" 
#include "Errors.h"


namespace Direct3D 
{

//***************************************************************************
Capabilities::Capabilities() :
	 mCanDoPixelShader20(false),
	 mCanDo64BitTextures(false),
	 mGetCanDoTrilinearFiltering(false),
	 mCanDoAutoMipMappingCreation(false),
	 mCanDoNonPower2Textures(false),
	 mMaxTextureWidth(0),
	 mMaxTextureHeight(0)
{
}

	
//***************************************************************************
bool Capabilities::InitFromDevice(IDirect3D9* direct3D, IDirect3DDevice9* device)
{
	D3DCAPS9 caps;
	memset(&caps, 0, sizeof(D3DCAPS9));
	HRESULT hr = device->GetDeviceCaps(&caps) ;
	if (FAILED(hr))
	{
		Error("Failed to get device capabilities");
		return false;
	}

	
	mMaxTextureWidth=caps.MaxTextureWidth;
	mMaxTextureHeight=caps.MaxTextureHeight;

	UINT ps_major = D3DSHADER_VERSION_MAJOR(caps.PixelShaderVersion);
	mCanDoPixelShader20 = ps_major >=2;

	hr = direct3D->CheckDeviceFormat(D3DADAPTER_DEFAULT,
									 D3DDEVTYPE_HAL, 
									 D3DFMT_X8R8G8B8,
									 D3DUSAGE_RENDERTARGET,
                                     D3DRTYPE_TEXTURE,
									 D3DFMT_A16B16G16R16);

	if (FAILED(hr))
	{
		mCanDo64BitTextures = false;
	}
	else
	{
		mCanDo64BitTextures = true;
	}

	mGetCanDoTrilinearFiltering = ((caps.TextureFilterCaps & D3DPTFILTERCAPS_MIPFLINEAR) !=0);

	mCanDoAutoMipMappingCreation = ((caps.Caps2 & D3DCAPS2_CANAUTOGENMIPMAP) != 0);

	if ( mCanDoAutoMipMappingCreation==true)
	{
		 hr = direct3D->CheckDeviceFormat(D3DADAPTER_DEFAULT,
									      D3DDEVTYPE_HAL, 
									      D3DFMT_X8R8G8B8,
										  D3DUSAGE_AUTOGENMIPMAP,
                                          D3DRTYPE_TEXTURE,
										  D3DFMT_A8R8G8B8);
		if (hr == D3DOK_NOAUTOGEN || FAILED(hr))
		{
			mCanDoAutoMipMappingCreation= false;
		}
	}

	// If both of these are not set this means unconditional support is provided for 2D textures.
	// If one of these are set then this often means some conitions apply.  I.e. non power2 textures 
	// will only work without mipmapping.
	// As our textures use mipmapping, we test for unconditional use.
	mCanDoNonPower2Textures = ((caps.TextureCaps & D3DPTEXTURECAPS_POW2) ==0) &&
							  ((caps.TextureCaps & D3DPTEXTURECAPS_NONPOW2CONDITIONAL ) ==0);


	return true;
	 
}

//***************************************************************************
bool Capabilities::GetCanDoPixelShader20()
{
	return mCanDoPixelShader20;
}

//***************************************************************************
bool Capabilities::GetCanDo64BitTextures()
{
	return mCanDo64BitTextures;
}

//***************************************************************************
bool Capabilities::GetCanDoTrilinearFiltering()
{
	return mGetCanDoTrilinearFiltering;
}

//***************************************************************************
bool Capabilities::GetCanDoAutoMipMappingCreation()
{
	return mCanDoAutoMipMappingCreation;
}

//***************************************************************************
bool Capabilities::GetCanDoNonPower2Textures()
{
	return mCanDoNonPower2Textures;
}

//***************************************************************************
int Capabilities::GetMaxTextureWidth()
{
	return mMaxTextureWidth;
}


//***************************************************************************
int Capabilities::GetMaxTextureHeight()
{
	return mMaxTextureHeight;
}

}