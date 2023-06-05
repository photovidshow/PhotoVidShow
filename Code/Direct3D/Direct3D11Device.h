#pragma once

#include <dxgi.h>
#include <d3dcommon.h>
#include <d3d11.h>
#include <DXGI1_2.h>
//#include <d3dx10math.h>
#include "DeviceState.h"
#include "Settings.h"
#include "Capabilities.h"
#include "ActiveReader.h"
#include "RenderWindow.h"
#include "PixelShaderEffect.h"

namespace Direct3D
{
	class Texture;
	class Surface;
	class BeginSceneParameters;
	class PixelShaderEffect;
	class VertexShaderEffect;

	class Direct3D11Device
	{
	public:
		Direct3D11Device(HWND hWnd, unsigned int width, unsigned int height);
		~Direct3D11Device();

		bool Render();


		DEVICE_STATE Reset(); // Assumes all surfaces and textures freed up before calling
		DEVICE_STATE Reset(unsigned int width, unsigned int height);
		DEVICE_STATE TestCooperativeLevel();  // Call this after PrensetToWindows fails, to see if we can reset yet

		void BeginScene(BeginSceneParameters& begin_scene_parameters);
		void EndScene();
		DEVICE_STATE PresentToWindow(HWND hWnd, bool scaleBackBufferToWindow); // Presents and switches backbuffer to the given window

		void DrawImage(Texture* texture, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2, float dest_x3, float dest_y3, float dest_x4, float dest_y4, bool alpha_enabled);
		void DrawImage(Surface* surface, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2, float dest_x3, float dest_y3, float dest_x4, float dest_y4, bool alpha_enabled);
		void DrawImageWithDiffuseAlpha(Texture* texture, float alpha, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2, float dest_x3, float dest_y3, float dest_x4, float dest_y4, bool include_image_alpha);
		void DrawImageWithDiffuseAlpha(Surface* texture, float alpha, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2, float dest_x3, float dest_y3, float dest_x4, float dest_y4, bool include_image_alpha);

		void SetRenderTargetToBackBuffer();
		void SetRenderTarget(Surface* surface);
		void ClearRenderTarget(int r, int g, int b, int a);
		int GetBackBufferWidth();
		int GetBackBufferHeight();

		void SetViewport(unsigned int width, unsigned int height);

		void CopyDefaultSurfaceToBitmap(unsigned int* bmp_buffer, int width, int height, int d_pitch);
		void CopyDefaultSurfaceToDynamicSurface(Surface* surface);

		void SetCurrentRenderWindow(HWND Hwnd, int with, int height);

		void SetPixelShaderEffect(PixelShaderEffect* effect);
		void SetVertexShaderEffect(VertexShaderEffect* effect);
		void ResetDefaultWindowLocationAndSize(int x, int y, int width, int height);
		unsigned int GetAvailableTextureMem();
		void SetBilinearInterpolation(int value);
		Settings& GetSettings();
		Capabilities& GetCapabilities();

		ID3D11Device* GetRawDevice();
		static Direct3D11Device* GetInstance();
		bool HalDeviceCreated();
		int GetLastCopyDefaultSurfaceToDynamicSurfaceStage();
		ID3D11DeviceContext* GetDeviceContext();
		void ClearCurrentRenderTargetTexture();

	private:

		static Direct3D11Device* mSingleInstance;

		void ReleaseD3DDevice();
		void CreateDefaultShaders();
		void SetupDeviceResources();
		void SetupSingleImageVertexBuffer(Texture* texture, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2, float dest_x3, float dest_y3, float dest_x4, float dest_y4);
	//	void SetupDiffuseImageVertexBuffer(Texture* texture, float alpha, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2, float dest_x3, float dest_y3, float dest_x4, float dest_y4);

		void CreateSwapChainDesc(HWND hWnd, int width, int height, DXGI_SWAP_CHAIN_DESC* swapChainDesc);
		bool m_vsync_enabled;
		int m_videoCardMemory;
		char m_videoCardDescription[128];

		RenderWindow* mCurrentRenderWindow;
		RenderWindow* mMainRenderWindow;
		RenderWindow* mAuxRenderWindow;

		ID3D11Device* m_device;
		ID3D11DeviceContext* m_deviceContext;
		ID3D11RasterizerState* m_rasterState;

		Settings mSettings;		// used to configure D3D settings
		Capabilities mCapabilities; // used to find out graphics capabilities

		//
		// DirectX 11 has no default pipeline, so have to create the default (null) pixel and vertex shaders
		//
		PixelShaderEffect* mDefaultPixelShader;
		VertexShaderEffect* mDefaultVertexShader;
		PixelShaderEffect* mDiffusePixelShader;
	//	VertexShaderEffect* mDiffuseVertexShader;

		ActiveReader<PixelShaderEffect> mCurrentPixelShaderEffect;

		IDXGIFactory* mFactory;
		IDXGIFactory2* mFactory2;

		ID3D11Buffer *m_vertexBuffer;
	//	ID3D11Buffer *m_vertexDiffuseBuffer;
		ID3D11Buffer *m_indexBuffer;
		int m_vertexCount, m_indexCount;

		ID3D11SamplerState* m_sampleState;
		ID3D11BlendState* m_blendState;
		ID3D11BlendState* m_blendNoAlphaState;

		ID3D11Texture2D* mCaptureTexture;
		int mCaptureWidth;
		int mCaptureHeight;

		Texture* mCurrentRenderTarget;		// null=current back buffer else it is a pointer to a render target texture
	};

}
