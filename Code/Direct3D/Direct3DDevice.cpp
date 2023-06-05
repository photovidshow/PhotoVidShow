// This is the main DLL file.

#include "stdafx.h"
#include "Direct3DDevice.h"
#include "Texture.h"
#include "BeginSceneParameters.h"
#include "Surface.h"
#include "windows.h"
#include "DX90SDK/Include/d3dx9.h" 
#include "Errors.h"

namespace
{
	const char* g_failed_to_set_render_state_string = "Failed to set render state";
	const char* g_failed_to_set_sampler_stage_string = "Failed to set sampler stage";
	const char* g_failed_to_create_vertex_buffer_string = "Failed to create vertex buffer";
	const char* g_failed_to_clear_render_target_string = "Failed to clear render target";
	const char* g_failed_to_set_render_target_string = "Failed to set render target";
	const char* g_failed_to_draw_primitive_string = "Failed to draw primitive";
	const char* g_failed_to_set_texture_stage_state_string = "Failed to set texture stage state";
	const char* g_failed_to_set_texture = "Failed to set texture";
	const char* g_failed_to_set_vertex_format_string = "Failed to set vertex format";
	const char* g_failed_to_set_stream_source_string ="Failed to set stream source";
	const char* g_failed_to_unlock_vertex_buffer_string ="Failed to unlock vertex buffer";
	const char* g_failed_to_lock_vertex_buffer_string="Failed to lock vertex buffer";
	const char* g_failed_to_set_viewport_string = "Failed to set viewport";
}

namespace Direct3D 
{

struct D3DVERTEX_T1_T2
{
	float x, y, z, rhw; float tu; float tv; float tu2; float tv2;
};

struct D3DVERTEX_D1_T1_T2
{
	float x, y, z, rhw; DWORD colour;  float tu; float tv; float tu2; float tv2;
};

Direct3DDevice* Direct3DDevice::mSingleInstance = NULL;

#define MAX_TEXTURE_STAGES 2 

//***************************************************************************
Direct3DDevice::Direct3DDevice( HWND hWnd, unsigned int width, unsigned int height ) :
	mDrawSingleImageVertexBuffer(NULL),
	mDrawDiffuseImageVertexBuffer(NULL),
	mDirect3D(NULL),
	mDirect3DDevice(NULL),
	mBegunScene(false),
	mStoredBackBuffer(NULL),
	mCurrentRenderTargetSupportsAlphaBlending(true),
	mStage(0)
{
	mSingleInstance = this;
	mDirect3D = Direct3DCreate9(D3D_SDK_VERSION);
	if (mDirect3D==NULL)
	{
		Error("Failed to create DirectX9 object");
		return;
	}

	memset(&mPresentParams, 0, sizeof(D3DPRESENT_PARAMETERS));

	mPresentParams.Windowed = TRUE;
	mPresentParams.SwapEffect = D3DSWAPEFFECT_DISCARD;
	mPresentParams.BackBufferFormat = D3DFMT_X8R8G8B8;
	mPresentParams.PresentationInterval = D3DPRESENT_INTERVAL_IMMEDIATE;

	// Create backbuffer to maximum we will ever need
	mPresentParams.BackBufferWidth = width;				// 1920
	mPresentParams.BackBufferHeight = height;		    // 1080

	CHECK_RETURN_EXIT(mDirect3D->CreateDevice(D3DADAPTER_DEFAULT,
		                                 D3DDEVTYPE_HAL, 
										 hWnd, 
										 D3DCREATE_SOFTWARE_VERTEXPROCESSING | D3DCREATE_MULTITHREADED ,
										 &mPresentParams,
										 &mDirect3DDevice), "Failed to create DirectX9 device");

	mDefaultWindowX=0;
	mDefaultWindowY=0;
	mDefaultWindowWidth = width;
	mDefaultWindowHeight= height;

	bool result = mCapabilities.InitFromDevice(mDirect3D, mDirect3DDevice);
	if (result==false)
	{
		ReleaseD3DDevice();
		return;
	}

	SetupDeviceResources();

	// Set initial viewport to same as given width, height

	mCurrentViewport.X      = 0;
	mCurrentViewport.Y      = 0;
	mCurrentViewport.Width  = mDefaultWindowWidth;
	mCurrentViewport.Height = mDefaultWindowHeight;
	mCurrentViewport.MinZ   = 0.0f;
	mCurrentViewport.MaxZ   = 1.0f;

	CHECK_RETURN(mDirect3DDevice->SetViewport(&mCurrentViewport), g_failed_to_set_viewport_string);
}

//***************************************************************************
bool Direct3DDevice::HalDeviceCreated()
{
	return mDirect3DDevice!=NULL;
}

//***************************************************************************
void Direct3DDevice::SetupDeviceResources()
{
	// Create a standard image vertex buffer
	CHECK_RETURN(mDirect3DDevice->CreateVertexBuffer(4*sizeof(D3DVERTEX_T1_T2), 0, D3DFVF_XYZRHW|D3DFVF_TEX1|D3DFVF_TEX2, D3DPOOL_DEFAULT, &mDrawSingleImageVertexBuffer, NULL), g_failed_to_create_vertex_buffer_string);

	// Create a image vertex buffer with colour diffuse (needed for when drawing with alpha applied)
	CHECK_RETURN(mDirect3DDevice->CreateVertexBuffer(4*sizeof(D3DVERTEX_D1_T1_T2), 0, D3DFVF_XYZRHW|D3DFVF_DIFFUSE|D3DFVF_TEX1|D3DFVF_TEX2, D3DPOOL_DEFAULT, &mDrawDiffuseImageVertexBuffer, NULL), g_failed_to_create_vertex_buffer_string);

	// Disable z buffer ( we don't do real 3D stuff )
	CHECK_RETURN(mDirect3DDevice->SetRenderState(D3DRS_ZENABLE,D3DZB_FALSE),g_failed_to_set_render_state_string);

	
	for (int stage=0; stage < MAX_TEXTURE_STAGES; stage++)
	{
		// Clamp all texture addressing
		CHECK_RETURN(mDirect3DDevice->SetSamplerState(stage, D3DSAMP_ADDRESSU, D3DTADDRESS_MIRROR), g_failed_to_set_sampler_stage_string);
		CHECK_RETURN(mDirect3DDevice->SetSamplerState(stage, D3DSAMP_ADDRESSV, D3DTADDRESS_MIRROR), g_failed_to_set_sampler_stage_string);

		SetBilinearInterpolation(1);

	}

	// Set alpha blenind type
	CHECK_RETURN(mDirect3DDevice->SetRenderState(D3DRS_SRCBLEND,D3DBLEND_SRCALPHA), g_failed_to_set_render_state_string);
	CHECK_RETURN(mDirect3DDevice->SetRenderState(D3DRS_DESTBLEND,D3DBLEND_INVSRCALPHA), g_failed_to_set_render_state_string);

	// Set Cull to none (to allow mirrored images)
	CHECK_RETURN(mDirect3DDevice->SetRenderState(D3DRS_CULLMODE, D3DCULL_NONE), g_failed_to_set_render_state_string);
}

//***************************************************************************
void Direct3DDevice::SetBilinearInterpolation(int value)
{
	for (int stage=0; stage < MAX_TEXTURE_STAGES; stage++)
	{ 
		if (value==1)
		{
			CHECK_RETURN(mDirect3DDevice->SetSamplerState(stage, D3DSAMP_MINFILTER, D3DTEXF_LINEAR ), g_failed_to_set_sampler_stage_string);
			CHECK_RETURN(mDirect3DDevice->SetSamplerState(stage, D3DSAMP_MAGFILTER, D3DTEXF_LINEAR ), g_failed_to_set_sampler_stage_string);
		    CHECK_RETURN(mDirect3DDevice->SetSamplerState(stage, D3DSAMP_MIPFILTER, D3DTEXF_LINEAR ), g_failed_to_set_sampler_stage_string);
		}
		else
		{
			CHECK_RETURN(mDirect3DDevice->SetSamplerState(stage, D3DSAMP_MINFILTER, D3DTEXF_POINT ), g_failed_to_set_sampler_stage_string);
			CHECK_RETURN(mDirect3DDevice->SetSamplerState(stage, D3DSAMP_MAGFILTER, D3DTEXF_POINT ), g_failed_to_set_sampler_stage_string);
		    CHECK_RETURN(mDirect3DDevice->SetSamplerState(stage, D3DSAMP_MIPFILTER, D3DTEXF_POINT ), g_failed_to_set_sampler_stage_string);
		}
	}
}


//***************************************************************************
DEVICE_STATE Direct3DDevice::TestCooperativeLevel()
{
	HRESULT result = mDirect3DDevice->TestCooperativeLevel();

	if (result == D3D_OK)
	{
		return DEVICE_OK;
	}
	if (result == D3DERR_DEVICENOTRESET)
	{
		return DEVICE_NOT_RESET;
	}
	return DEVICE_LOST;
}

//***************************************************************************
DEVICE_STATE Direct3DDevice::Reset() 
{
	ReleaseDeviceResources();

	HRESULT result = mDirect3DDevice->Reset(&mPresentParams);

	if (FAILED(result) )
	{
		return DEVICE_LOST;
	}

	SetupDeviceResources();

	return DEVICE_OK;
}

//***************************************************************************
DEVICE_STATE Direct3DDevice::Reset(unsigned int width, unsigned int height) 
{
	mPresentParams.BackBufferWidth = width;
	mPresentParams.BackBufferHeight = height;

	return Reset();
}

//***************************************************************************
void Direct3DDevice::ReleaseDeviceResources()
{
	if (mDrawSingleImageVertexBuffer)
	{
		mDrawSingleImageVertexBuffer->Release();
		mDrawSingleImageVertexBuffer=NULL;
	}

	if (mDrawDiffuseImageVertexBuffer)
	{
		mDrawDiffuseImageVertexBuffer->Release();
		mDrawDiffuseImageVertexBuffer=NULL;
	}

	if (mStoredBackBuffer)
	{
		mStoredBackBuffer->Release();
		mStoredBackBuffer=NULL;
	}
}

//***************************************************************************
void Direct3DDevice::ReleaseD3DDevice()
{
	ReleaseDeviceResources();

	if (mDirect3D)
	{
		mDirect3D->Release();
		mDirect3D=NULL;
	}
 
	if ( mDirect3DDevice)
	{
		mDirect3DDevice->Release();
		mDirect3DDevice=NULL;
	}
}


//***************************************************************************
Direct3DDevice::~Direct3DDevice()
{
	ReleaseD3DDevice();
}

//***************************************************************************
void Direct3DDevice::BeginScene( BeginSceneParameters& begin_scene_parameters )
{
	if (mBegunScene) return ;

	mDirect3DDevice->BeginScene();

	CHECK_RETURN(mDirect3DDevice->Clear(0, NULL, D3DCLEAR_TARGET, D3DCOLOR_RGBA(0, 0, 0, 255),  1.0f, 0),g_failed_to_clear_render_target_string);

	// Passed in a target to render to?
	if ( begin_scene_parameters.mRenderTarget != NULL)
	{
		SetRenderTarget(begin_scene_parameters.mRenderTarget);
	}

	mBegunScene = true;
}

//***************************************************************************
void Direct3DDevice::SetRenderTargetToBackBuffer()
{
	SetRenderTarget(NULL);
}

//***************************************************************************
void Direct3DDevice::ClearRenderTarget(int r, int g, int b, int a)
{
	CHECK_RETURN(mDirect3DDevice->Clear(0, NULL, D3DCLEAR_TARGET, D3DCOLOR_RGBA(r, g, b, a), 1.0f, 0), g_failed_to_clear_render_target_string);

	if (r==0 && g ==0 && b==0 && a==0)
	{
		mCurrentRenderTargetSupportsAlphaBlending = false; // disable alpha blending when rendering to the current RenderTarget
	}
    else
    {
        mCurrentRenderTargetSupportsAlphaBlending = true;
    }
}

//***************************************************************************
void Direct3DDevice::SetRenderTarget( Surface* surface)
{
	// Reset alpha blending to be enabled
	mCurrentRenderTargetSupportsAlphaBlending = true;

	if (surface == NULL)
	{
		// Restore backbuffer (i.e. current render targer does no equal back buffer )?
		if (mStoredBackBuffer != NULL)
		{
			CHECK_RETURN(mDirect3DDevice->SetRenderTarget(0, mStoredBackBuffer),g_failed_to_set_render_target_string);
			mStoredBackBuffer->Release();
			mStoredBackBuffer = NULL;
		}
	}
	else
	{
		if (mStoredBackBuffer == NULL)
		{
			// Selected other surface to render to
			CHECK_RETURN(mDirect3DDevice->GetRenderTarget(0, &mStoredBackBuffer), "Failed to get render target");
		}

		if ( surface->GetD3DSurface() != NULL)
		{
			CHECK_RETURN(mDirect3DDevice->SetRenderTarget(0, surface->GetD3DSurface()), g_failed_to_set_render_target_string);
		}
	}
}

//***************************************************************************
void Direct3DDevice::EndScene()
{
	if (!mBegunScene) return ;

	// ensure we set render tatget back to backbuffer and release our handle to it
	if (mStoredBackBuffer != NULL)
	{
		SetRenderTarget(NULL);
	}

	CHECK_RETURN(mDirect3DDevice->EndScene(), "EndScene failed");
	mBegunScene= false;
}

// If first paremeter null presents to default window.
// Second paremater is only used if first parameter is not null, and if set true, scales the current 
// backbuffer to the given window else does a direct copy from the back buffer to the window.
//***************************************************************************
DEVICE_STATE Direct3DDevice::PresentToWindow(HWND hWnd, bool scaleBackBufferToWindow)	
{
	RECT rct;
	rct.left=0;
	rct.top=0;
	rct.right = mCurrentViewport.Width;
	rct.bottom = mCurrentViewport.Height;

	RECT Dstrct;
	Dstrct.left=0;
	Dstrct.top=0;
	Dstrct.right=0;
	Dstrct.bottom=0;

	if (hWnd != NULL)
	{
		GetWindowRect(hWnd, &Dstrct);
		
		Dstrct.right = Dstrct.right - Dstrct.left;
		Dstrct.bottom = Dstrct.bottom - Dstrct.top;
		Dstrct.left=0;
		Dstrct.top=0;

        if (scaleBackBufferToWindow==FALSE)
        {
		    rct = Dstrct;
        }
	}
	else
	{
		Dstrct.left = mDefaultWindowX;
		Dstrct.top = mDefaultWindowY;
		Dstrct.right = mDefaultWindowWidth + mDefaultWindowX;
		Dstrct.bottom = mDefaultWindowHeight + mDefaultWindowY;
	}

	HRESULT result = mDirect3DDevice->Present(&rct, &Dstrct, hWnd, NULL);

	// Excepted failure (i.e. locked computer or CTRL-ALT-DELETE etc )
	if (result == D3DERR_DEVICELOST)
	{
		return DEVICE_LOST;
	}
	else if (FAILED(result) )
	{
		CHECK_RETURN(result,"Failed to present");
		return DEVICE_LOST;
	}

	return DEVICE_OK;
}

//***************************************************************************
void Direct3DDevice::SetViewport(unsigned int width, unsigned int height)
{
	mCurrentViewport.Width  = width;
	mCurrentViewport.Height = height;

    HRESULT hr = mDirect3DDevice->SetViewport(&mCurrentViewport);
    if (FAILED(hr))
    {
        Error("%s %d,%d code:%d", g_failed_to_set_viewport_string, width, height, hr);
    }
}



//***************************************************************************
// Setups a vertexbufer buffer with a triangle strip which represents rotated image rectangle
void Direct3DDevice::SetupSingleImageVertexBuffer(Texture* texture, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2,float dest_x3, float dest_y3,float dest_x4, float dest_y4 )
{
	D3DVERTEX_T1_T2 vertices[4];

	texture->ConvertSrcToUVCoordinates(&src_x, &src_y, &src_width, &src_height);

	float text2SrcLeft=0;
	float text2SrcTop=0;
	float text2SrcRight=1;
	float text2SrcBottom=1;

	// If we have a pixelshader set and with input texture set. Then we should add half a pixel to T2 uv coords
	if (mCurrentPixelShaderEffect.ToRead() != NULL)
	{
		Texture* input_texture = mCurrentPixelShaderEffect->GetTexture1();
		if (input_texture != NULL)
		{	
			input_texture->ConvertSrcToUVCoordinates(&text2SrcLeft, &text2SrcTop, &text2SrcRight, &text2SrcBottom);
		}
	}

	vertices[0].x = dest_x1; 
	vertices[0].y = dest_y1; 
	vertices[0].z = 0; 
	vertices[0].rhw = 1.0f; 
	vertices[0].tu = src_x; 
	vertices[0].tv = src_height;
	vertices[0].tu2 = text2SrcLeft;
	vertices[0].tv2 = text2SrcBottom;

	vertices[1].x = dest_x2; 
	vertices[1].y = dest_y2; 
	vertices[1].z = 0; 
	vertices[1].rhw = 1.0f; 
	vertices[1].tu = src_x; 
	vertices[1].tv = src_y; 
	vertices[1].tu2 = text2SrcLeft;
	vertices[1].tv2 = text2SrcTop;

	vertices[2].x = dest_x3; 
	vertices[2].y = dest_y3; 
	vertices[2].z = 0; 
	vertices[2].rhw = 1.0f; 
	vertices[2].tu = src_width ;
	vertices[2].tv = src_height ;
	vertices[2].tu2 = text2SrcRight  ;
	vertices[2].tv2 = text2SrcBottom;

	vertices[3].x = dest_x4; 
	vertices[3].y = dest_y4; 
	vertices[3].z = 0; 
	vertices[3].rhw = 1.0f; 
	vertices[3].tu = src_width;
	vertices[3].tv = src_y; 
	vertices[3].tu2 = text2SrcRight ;
	vertices[3].tv2 = text2SrcTop;

	void *pVertexBufferPtr = NULL; 

	if (mDrawSingleImageVertexBuffer==NULL) return ;

	CHECK_RETURN(mDrawSingleImageVertexBuffer->Lock(0, 4*sizeof(D3DVERTEX_T1_T2), &pVertexBufferPtr, 0),g_failed_to_lock_vertex_buffer_string);

	memcpy(pVertexBufferPtr, vertices, 4*sizeof(D3DVERTEX_T1_T2));
	CHECK_RETURN(mDrawSingleImageVertexBuffer->Unlock(),g_failed_to_unlock_vertex_buffer_string);

	CHECK_RETURN(mDirect3DDevice->SetStreamSource(0, mDrawSingleImageVertexBuffer, 0, sizeof(D3DVERTEX_T1_T2)),g_failed_to_set_stream_source_string);
	CHECK_RETURN(mDirect3DDevice->SetFVF(D3DFVF_XYZRHW|D3DFVF_TEX1|D3DFVF_TEX2), g_failed_to_set_vertex_format_string);
}

//***************************************************************************
// Setups a vertexbufer buffer with a triangle strip which represents an image rectangle
void Direct3DDevice::SetupDiffuseImageVertexBuffer(Texture* texture, float alpha, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2,float dest_x3, float dest_y3,float dest_x4, float dest_y4)
{
	D3DVERTEX_D1_T1_T2 vertices[4];

	texture->ConvertSrcToUVCoordinates(&src_x, &src_y, &src_width, &src_height);

	float text2SrcLeft=0;
	float text2SrcTop=0;
	float text2SrcRight=1;
	float text2SrcBottom=1;

	// If we have a pixelshader set and with input texture set. Then we should add half a pixel to T2 uv coords
	if (mCurrentPixelShaderEffect.ToRead() != NULL)
	{
		Texture* input_texture = mCurrentPixelShaderEffect->GetTexture1();
		if (input_texture != NULL)
		{	
			input_texture->ConvertSrcToUVCoordinates(&text2SrcLeft, &text2SrcTop, &text2SrcRight, &text2SrcBottom);
		}
	}

	int i_alpha = (int)(alpha+0.499f);
	if (i_alpha<0) i_alpha=0;
	if (i_alpha>255) i_alpha=255;

	vertices[0].x = dest_x1; 
	vertices[0].y = dest_y1; 
	vertices[0].z = 0; 
	vertices[0].rhw = 1.0f; 
	vertices[0].colour = D3DCOLOR_ARGB(i_alpha, 255, 255, 255);
	vertices[0].tu = src_x; 
	vertices[0].tv = src_height; 
	vertices[0].tu2 = text2SrcLeft ;
	vertices[0].tv2 = text2SrcBottom;

	vertices[1].x = dest_x2; 
	vertices[1].y = dest_y2; 
	vertices[1].z = 0; 
	vertices[1].rhw = 1.0f; 
	vertices[1].colour = D3DCOLOR_ARGB(i_alpha, 255, 255, 255);
	vertices[1].tu = src_x; 
	vertices[1].tv = src_y; 
	vertices[1].tu2 = text2SrcLeft;
	vertices[1].tv2 = text2SrcTop;

	vertices[2].x = dest_x3; 
	vertices[2].y = dest_y3; 
	vertices[2].z = 0; 
	vertices[2].rhw = 1.0f; 
	vertices[2].colour = D3DCOLOR_ARGB(i_alpha, 255, 255, 255);
	vertices[2].tu = src_width; 
	vertices[2].tv = src_height; 
	vertices[2].tu2 = text2SrcRight ;
	vertices[2].tv2 = text2SrcBottom;
 
	vertices[3].x = dest_x4; 
	vertices[3].y = dest_y4; 
	vertices[3].z = 0; 
	vertices[3].rhw = 1.0f; 
	vertices[3].colour = D3DCOLOR_ARGB(i_alpha, 255, 255, 255);
	vertices[3].tu = src_width; 
	vertices[3].tv = src_y; 
	vertices[3].tu2 = text2SrcRight;
	vertices[3].tv2 = text2SrcTop;

	void *pVertexBufferPtr = NULL; 

	if (mDrawDiffuseImageVertexBuffer==NULL) return ;

	CHECK_RETURN(mDrawDiffuseImageVertexBuffer->Lock(0, 4 * sizeof(D3DVERTEX_D1_T1_T2), &pVertexBufferPtr, 0), g_failed_to_lock_vertex_buffer_string) ;

	memcpy(pVertexBufferPtr, vertices, 4 * sizeof(D3DVERTEX_D1_T1_T2));
	CHECK_RETURN(mDrawDiffuseImageVertexBuffer->Unlock(), g_failed_to_unlock_vertex_buffer_string);

	CHECK_RETURN(mDirect3DDevice->SetStreamSource(0, mDrawDiffuseImageVertexBuffer, 0, sizeof(D3DVERTEX_D1_T1_T2)), g_failed_to_set_stream_source_string);
	CHECK_RETURN(mDirect3DDevice->SetFVF(D3DFVF_XYZRHW|D3DFVF_DIFFUSE|D3DFVF_TEX1|D3DFVF_TEX2), g_failed_to_set_vertex_format_string) ;

}

//***************************************************************************
void Direct3DDevice::DrawImage(Texture* texture, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2,float dest_x3, float dest_y3,float dest_x4, float dest_y4, bool alpha)
{
	if (texture==NULL)
	{
		ErrorOnce("Direct3DDevice::DrawImage No texture to render with"));
		return;
	}

	if (mBegunScene == false)
	{
		ErrorOnce("Direct3DDevice::DrawImage Scene not begun can not render image"));
		return;
	}

	IDirect3DTexture9* t= texture->GetD3DTexture();
	if (t==NULL) 
	{
		ErrorOnce("Direct3DDevice::DrawImage No raw texture to render with"));
		return;
	}

	CHECK_RETURN(mDirect3DDevice->SetTexture(0,t), g_failed_to_set_texture);
	CHECK_RETURN(mDirect3DDevice->SetRenderState(D3DRS_ALPHABLENDENABLE, mCurrentRenderTargetSupportsAlphaBlending && alpha), g_failed_to_set_render_state_string);

	if ( alpha )
	{
		CHECK_RETURN(mDirect3DDevice->SetTextureStageState(0, D3DTSS_ALPHAOP,D3DTOP_SELECTARG1),g_failed_to_set_texture_stage_state_string);
		CHECK_RETURN(mDirect3DDevice->SetTextureStageState(0, D3DTSS_ALPHAARG1,D3DTA_TEXTURE),g_failed_to_set_texture_stage_state_string);
	}
	else
	{
		// Even though we won't be doing alpha blending we also don't want to copy image alpha channel to target surface
		CHECK_RETURN(mDirect3DDevice->SetTextureStageState(0, D3DTSS_ALPHAOP,D3DTOP_DISABLE),g_failed_to_set_texture_stage_state_string);
	}

	CHECK_RETURN(mDirect3DDevice->SetTextureStageState(0,D3DTSS_COLOROP,D3DTOP_SELECTARG1),g_failed_to_set_texture_stage_state_string);
	CHECK_RETURN(mDirect3DDevice->SetTextureStageState(0,D3DTSS_COLORARG1,D3DTA_TEXTURE),g_failed_to_set_texture_stage_state_string);
	CHECK_RETURN(mDirect3DDevice->SetTextureStageState(1,D3DTSS_COLOROP,D3DTOP_DISABLE),g_failed_to_set_texture_stage_state_string);

	SetupSingleImageVertexBuffer(texture, src_x,src_y,src_width,src_height,dest_x1,dest_y1, dest_x2, dest_y2, dest_x3, dest_y3, dest_x4, dest_y4);

	CHECK_RETURN(mDirect3DDevice->DrawPrimitive(D3DPT_TRIANGLESTRIP, 0, 2), g_failed_to_draw_primitive_string);

}

//***************************************************************************
void Direct3DDevice::DrawImage(Surface* surface, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2,float dest_x3, float dest_y3,float dest_x4, float dest_y4, bool alpha)
{
	if (surface == NULL)
	{
		ErrorOnce("Direct3DDevice::DrawImage No surface to render with"));
		return ;
	}
	DrawImage(surface->GetTexture(), src_x, src_y, src_width, src_height, dest_x1, dest_y1, dest_x2, dest_y2, dest_x3, dest_y3, dest_x4, dest_y4, alpha);
}

//***************************************************************************
void Direct3DDevice::DrawImageWithDiffuseAlpha(Texture* texture, float alpha, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2,float dest_x3, float dest_y3,float dest_x4, float dest_y4, bool include_image_alpha)
{
	if (texture==NULL)
	{
		ErrorOnce("Direct3DDevice::DrawImageWithDiffuseAlpha No texture to render with"));
		return ;
	}

	if (mBegunScene == false)
	{
		ErrorOnce("Direct3DDevice::DrawImageWithDiffuseAlpha Scene not begun can not render image"));
		return;
	}
	
	IDirect3DTexture9* t= texture->GetD3DTexture();
	if (t==NULL) 
	{
		ErrorOnce("Direct3DDevice::DrawImageWithDiffuseAlpha No raw texture to render with"));
		return;
	}

	CHECK_RETURN(mDirect3DDevice->SetTexture(0, t), g_failed_to_set_texture);

	CHECK_RETURN(mDirect3DDevice->SetRenderState(D3DRS_ALPHABLENDENABLE, mCurrentRenderTargetSupportsAlphaBlending), g_failed_to_set_render_state_string);
	CHECK_RETURN(mDirect3DDevice->SetTextureStageState(0, D3DTSS_COLOROP,D3DTOP_SELECTARG1), g_failed_to_set_texture_stage_state_string);
	CHECK_RETURN(mDirect3DDevice->SetTextureStageState(0, D3DTSS_COLORARG1,D3DTA_TEXTURE),g_failed_to_set_texture_stage_state_string);

	if ( include_image_alpha )
	{
		CHECK_RETURN(mDirect3DDevice->SetTextureStageState(0, D3DTSS_ALPHAOP,D3DTOP_MODULATE),g_failed_to_set_texture_stage_state_string);
		CHECK_RETURN(mDirect3DDevice->SetTextureStageState(0, D3DTSS_ALPHAARG1,D3DTA_DIFFUSE),g_failed_to_set_texture_stage_state_string);
		CHECK_RETURN(mDirect3DDevice->SetTextureStageState(0, D3DTSS_ALPHAARG2,D3DTA_TEXTURE),g_failed_to_set_texture_stage_state_string);
	}
	else
	{
		CHECK_RETURN(mDirect3DDevice->SetTextureStageState(0, D3DTSS_ALPHAOP,D3DTOP_SELECTARG1),g_failed_to_set_texture_stage_state_string);
		CHECK_RETURN(mDirect3DDevice->SetTextureStageState(0, D3DTSS_ALPHAARG1,D3DTA_DIFFUSE),g_failed_to_set_texture_stage_state_string);
	}

	CHECK_RETURN(mDirect3DDevice->SetTextureStageState(1, D3DTSS_COLOROP,D3DTOP_DISABLE),g_failed_to_set_texture_stage_state_string);

	SetupDiffuseImageVertexBuffer(texture, alpha, src_x, src_y, src_width ,src_height, dest_x1, dest_y1, dest_x2, dest_y2, dest_x3, dest_y3, dest_x4, dest_y4);

	CHECK_RETURN(mDirect3DDevice->DrawPrimitive(D3DPT_TRIANGLESTRIP, 0, 2), g_failed_to_draw_primitive_string);
}

//***************************************************************************
void Direct3DDevice::DrawImageWithDiffuseAlpha(Surface* surface, float alpha, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2,float dest_x3, float dest_y3,float dest_x4, float dest_y4, bool include_image_alpha)
{
	if (surface==NULL) 
	{
		ErrorOnce("Direct3DDevice::DrawImageWithDiffuseAlpha No surface to render with"));
		return;
	}

	DrawImageWithDiffuseAlpha(surface->GetTexture(), alpha, src_x, src_y, src_width, src_height, dest_x1, dest_y1, dest_x2, dest_y2, dest_x3, dest_y3, dest_x4, dest_y4, include_image_alpha);
}

//***************************************************************************
void Direct3DDevice::SetPixelShaderEffect(PixelShaderEffect* effect)
{
	if ( effect )
	{
		effect->Setup(this);
	}
	else
	{
		CHECK_RETURN(mDirect3DDevice->SetPixelShader(NULL), "Failed to set pixel shader to NULL");	// default pipeline 
	}

	mCurrentPixelShaderEffect = effect;
}

//***************************************************************************
Direct3DDevice* Direct3DDevice::GetInstance()
{
	return mSingleInstance;
}

//***************************************************************************
IDirect3DDevice9* Direct3DDevice::GetRawDevice()
{
	return mDirect3DDevice;
}

//***************************************************************************
void Direct3DDevice::CopyDefaultSurfaceToBitmap(unsigned int* bmp_buffer, int width, int height, int d_pitch)
{
	IDirect3DSurface9* pBackBuffer = NULL;

	//HRESULT rslt = mDirect3DDevice->GetBackBuffer(0, 0, D3DBACKBUFFER_TYPE_MONO, &pBackBuffer);
	HRESULT rslt = mDirect3DDevice->GetRenderTarget(0, &pBackBuffer);

	if (SUCCEEDED(rslt))
	{
		D3DSURFACE_DESC Desc;
		rslt = pBackBuffer->GetDesc(&Desc);

		if (SUCCEEDED(rslt) && Desc.Width >= width && Desc.Height >= height)
		{
			IDirect3DTexture9* pSystemTexture = NULL;
			rslt = mDirect3DDevice->CreateTexture(Desc.Width, Desc.Height, 1, 0 , Desc.Format, D3DPOOL_SYSTEMMEM, &pSystemTexture, NULL);
			if (SUCCEEDED(rslt))
			{
				IDirect3DSurface9* pSystemSurface = NULL;
				rslt = pSystemTexture->GetSurfaceLevel(0, &pSystemSurface);
				if (SUCCEEDED(rslt))
				{	
					rslt = mDirect3DDevice->GetRenderTargetData(pBackBuffer, pSystemSurface);

					if (SUCCEEDED(rslt) )
					{
						D3DLOCKED_RECT lr;
						RECT rect;
						rect.left = 0;
						rect.right = Desc.Width;
						rect.top = 0;
						rect.bottom = Desc.Height;
						// Lock the surface to read pixels
						rslt = pSystemSurface->LockRect( &lr, &rect, D3DLOCK_READONLY );
						if ( SUCCEEDED(rslt))
						{
							int d_surface = (lr.Pitch - (Desc.Width *4)) >> 2;
							d_surface +=Desc.Width - width;

							unsigned int* pointer = (unsigned int*) lr.pBits;

							for (unsigned int h=0;h<height; h++)
							{
								for (unsigned int w=0;w<width; w++)
								{
									*bmp_buffer++ = (*pointer++) | 0xff000000;
								}

								pointer+= d_surface;
								bmp_buffer += d_pitch;
							}

							pSystemSurface->UnlockRect();
						}
					}

					pSystemSurface->Release();
					pSystemSurface = NULL;
				}
				pSystemTexture->Release();
				pSystemTexture = NULL;
			}
		}
		else
		{
			Error("CopyDefaultSurfaceToBitmap Failed, dest surface smaller than bitmap size:%d %d, bitmap size:%d %d", Desc.Width,Desc.Height, width, height);
		}
		pBackBuffer->Release();
		pBackBuffer = NULL;
	}

	CHECK_RETURN(rslt, "Failed in CopyDefaultSurfaceToBitmap");
}

//***************************************************************************
// Passed in surface and current render target must be 32 Bit
// mStage is a temp used in this build as this function has been seen to 
// crash on a clients samsung laptop
void Direct3DDevice::CopyDefaultSurfaceToDynamicSurface(Surface* surface)
{
	mStage =1;

	if (surface==NULL)
	{
		ErrorOnce("Surface null passed to CopyDefaultSurfaceToDynamicSurface"));
		return;
	}

	if (mDirect3DDevice==NULL)
	{
		ErrorOnce("Device null on call to CopyDefaultSurfaceToDynamicSurface"));
		return;
	}

	mStage =2;

	IDirect3DSurface9* pBackBuffer = NULL;

	//HRESULT rslt = mDirect3DDevice->GetBackBuffer(0, 0, D3DBACKBUFFER_TYPE_MONO, &pBackBuffer);
    HRESULT rslt = mDirect3DDevice->GetRenderTarget(0, &pBackBuffer);

	bool success = false;
	mStage =3;

	if (SUCCEEDED(rslt))
	{
		D3DSURFACE_DESC backBufferDesc;
		memset(&backBufferDesc, 0, sizeof(D3DSURFACE_DESC));
		rslt = pBackBuffer->GetDesc(&backBufferDesc);
		if (SUCCEEDED(rslt)==false)
		{
			ErrorOnce("Failed to get backbuffer description in Direct3DDevice::CopyDefaultSurfaceToDynamicSurface"));
			return;
		}

		mStage =4;

		Texture* texture = surface->GetTexture();
		if (texture==NULL)
		{
			ErrorOnce("NULL texture in Direct3DDevice::CopyDefaultSurfaceToDynamicSurface"));
			return;
		}

		if (backBufferDesc.Width == texture->GetRawSurfaceWidth() && backBufferDesc.Height == texture->GetRawSurfaceHeight() )
		{
			mStage =5;

			IDirect3DTexture9* pSystemTexture = texture->GetD3DTexture();
			if ( pSystemTexture != NULL)
			{
				mStage =6;

				IDirect3DSurface9* pSystemSurface = NULL;
				rslt = pSystemTexture->GetSurfaceLevel(0, &pSystemSurface);
				if (SUCCEEDED(rslt))
				{	
					mStage =7;

					rslt = mDirect3DDevice->GetRenderTargetData(pBackBuffer, pSystemSurface);

					mStage =8;

					if (SUCCEEDED(rslt) )
					{
						success=true;
					}
					else
					{
						CHECK_RETURN(rslt, "Failed to get render target data");
					}
					
					mStage =9;

					pSystemSurface->Release();
					pSystemSurface = NULL;
				
				}
			}
		}
		else
		{
			ErrorOnce("Backbuffer bigger diffent size than dynamic surface"));
		}
		
		mStage =10;

		pBackBuffer->Release();
		pBackBuffer = NULL;
	}

	if (success==false)
	{
		ErrorOnce("Failed to copy render surface to dynamic surface"));
	}
	mStage =11;

	CHECK_RETURN(rslt,"Failed in CopyDefaultSurfaceToDynamicSurface");
}

//***************************************************************************
int Direct3DDevice::GetLastCopyDefaultSurfaceToDynamicSurfaceStage()
{
	return mStage;
}


//***************************************************************************
int Direct3DDevice::GetBackBufferWidth()
{
	return mPresentParams.BackBufferWidth;
}


//***************************************************************************
int Direct3DDevice::GetBackBufferHeight()
{
	return mPresentParams.BackBufferHeight;
}

//***************************************************************************
void Direct3DDevice::ResetDefaultWindowLocationAndSize(int x, int y, int width, int height)
{
	mDefaultWindowX = x;
	mDefaultWindowY = y;
	mDefaultWindowWidth = width;
	mDefaultWindowHeight = height;
}

//***************************************************************************
unsigned int Direct3DDevice::GetAvailableTextureMem()
{
	return mDirect3DDevice->GetAvailableTextureMem();
}

//***************************************************************************
Settings& Direct3DDevice::GetSettings()
{
	return mSettings;
}

//***************************************************************************
Capabilities& Direct3DDevice::GetCapabilities()
{
	return mCapabilities;
}



}