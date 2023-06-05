#pragma once
#include <dxgi.h>
#include <d3dcommon.h>
#include <d3d11.h>
#include <DXGI1_2.h>

namespace Direct3D
{
	//
	// This class represents a window which we can rendered to.  Each window in DX 11 needs it's own swap chain
	// and render target view
	//
	class RenderWindow
	{
	public:
		RenderWindow(HWND hWnd);
		~RenderWindow();
		void SetupBackBuffers(int width, int height);

		IDXGISwapChain* GetSwapChain();
		ID3D11RenderTargetView* GetRenderTargetView();
		void SetSwapChain(IDXGISwapChain* chain);
		HWND GetWindowHandle();
		void PresentToWindow();
		void SetAsCurrentRenderTarget();
		int GetBackBufferWidth();
		int GetBackBufferHeight();
	
	private:

		//ID3D11Texture2D* m_depthStencilBuffer;
		//ID3D11DepthStencilState* m_depthStencilState;
		//ID3D11DepthStencilView* m_depthStencilView;
		IDXGISwapChain* m_swapChain;
		ID3D11RenderTargetView* m_renderTargetView;
		HWND m_hWnd;
		int mBackBufferWidth;
		int mBackBufferHeight;
	};

}
