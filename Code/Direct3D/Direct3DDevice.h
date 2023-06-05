// Direct3D.h

#pragma once
#include <Windows.h>
#include <d3d9types.h>
#include "PixelShaderEffect.h"
#include "Settings.h"
#include "Capabilities.h"
#include "DeviceState.h"

struct IDirect3D9;
struct IDirect3DDevice9;
struct IDirect3DTexture9;
struct IDirect3DVertexBuffer9;
struct IDirect3DSurface9;
struct IDirect3DPixelShader9;

namespace Direct3D 
{

class Texture;
class Surface;
class BeginSceneParameters;
class PixelShaderEffect;


class Direct3DDevice
{
public:
	Direct3DDevice(HWND hWnd, unsigned int width, unsigned int height);
	~Direct3DDevice();

	DEVICE_STATE Reset(); // Assumes all surfaces and textures freed up before calling
	DEVICE_STATE Reset(unsigned int width, unsigned int height); 
	DEVICE_STATE TestCooperativeLevel();  // Call this after PrensetToWindows fails, to see if we can reset yet

	void BeginScene( BeginSceneParameters& begin_scene_parameters );
	void EndScene();
	DEVICE_STATE PresentToWindow(HWND hWnd, bool scaleBackBufferToWindow); // Presents and switches backbuffer to the given window

	void DrawImage(Texture* texture, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2,float dest_x3, float dest_y3,float dest_x4, float dest_y4, bool alpha_enabled);
	void DrawImage(Surface* surface, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2,float dest_x3, float dest_y3,float dest_x4, float dest_y4, bool alpha_enabled);
	void DrawImageWithDiffuseAlpha(Texture* texture, float alpha, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2,float dest_x3, float dest_y3,float dest_x4, float dest_y4, bool include_image_alpha);
	void DrawImageWithDiffuseAlpha(Surface* texture, float alpha, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2,float dest_x3, float dest_y3,float dest_x4, float dest_y4, bool include_image_alpha);

	void SetRenderTargetToBackBuffer();
	void SetRenderTarget( Surface* surface);
	void ClearRenderTarget(int r, int g, int b, int a);
	int GetBackBufferWidth();
	int GetBackBufferHeight();

	void SetViewport(unsigned int width, unsigned int height);

	void CopyDefaultSurfaceToBitmap(unsigned int* bmp_buffer, int width, int height, int d_pitch);
	void CopyDefaultSurfaceToDynamicSurface(Surface* surface);

	IDirect3DDevice9* GetRawDevice();
	static Direct3DDevice* GetInstance();

	void SetPixelShaderEffect(PixelShaderEffect* effect);
	void ResetDefaultWindowLocationAndSize(int x, int y, int width, int height);
	unsigned int GetAvailableTextureMem();
	void SetBilinearInterpolation(int value);
	Settings& GetSettings();
	Capabilities& GetCapabilities();


	bool HalDeviceCreated();

	int GetLastCopyDefaultSurfaceToDynamicSurfaceStage();

private:

	void SetupSingleImageVertexBuffer(Texture* texture, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2,float dest_x3, float dest_y3,float dest_x4, float dest_y4 );
	void SetupDiffuseImageVertexBuffer(Texture* texture, float alpha, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2,float dest_x3, float dest_y3,float dest_x4, float dest_y4);
	void SetupDeviceResources();
	void ReleaseDeviceResources();
	void ReleaseD3DDevice();

	static Direct3DDevice* mSingleInstance;

	bool mBegunScene;

	IDirect3D9*      mDirect3D;
	IDirect3DDevice9* mDirect3DDevice;

	IDirect3DVertexBuffer9* mDrawSingleImageVertexBuffer;
	IDirect3DVertexBuffer9* mDrawDiffuseImageVertexBuffer;

	IDirect3DSurface9* mStoredBackBuffer;

	// If a RenderTarget surface has been cleared with ClearRenderTarget with values 0,0,0,0.  This means it does not support alpha blending, because we are saying there
	// is nothing to blend it with. So when drawing an image to it, the image alpha is simply copied to the render target (i.e. as the R,G,B )
	bool mCurrentRenderTargetSupportsAlphaBlending;

	D3DVIEWPORT9 mCurrentViewport;

	unsigned int mDefaultWindowX;
	unsigned int mDefaultWindowY;
	unsigned int mDefaultWindowWidth ;
	unsigned int mDefaultWindowHeight;

	ActiveReader<PixelShaderEffect> mCurrentPixelShaderEffect;

	D3DPRESENT_PARAMETERS mPresentParams;  // used when initialising/and resetting device

	Settings mSettings;		// used to configure D3D settings
	Capabilities mCapabilities; // used to find out graphics capabilities

	int mStage; // used to track crash bugs

};

extern Direct3DDevice* TheDirect3DDevice ;

}
