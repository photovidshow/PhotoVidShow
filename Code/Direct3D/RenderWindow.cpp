#include "stdafx.h"
#include "RenderWindow.h"
#include "errors.h"
#include "Direct3D11Device.h"

namespace Direct3D
{
	//***************************************************************************
	RenderWindow::RenderWindow(HWND hWnd) :
		m_swapChain(NULL),
		m_renderTargetView(NULL),
		//m_depthStencilBuffer(NULL),
		//m_depthStencilState(NULL),
		//m_depthStencilView(NULL),
		m_hWnd(hWnd)
	{
	}

	//***************************************************************************
	RenderWindow::~RenderWindow()
	{
		// Before shutting down set to windowed mode or when you release the swap chain it will throw an exception.
		if (m_swapChain)
		{
			m_swapChain->SetFullscreenState(false, NULL);
		}

		/*
		if (m_depthStencilView)
		{
			m_depthStencilView->Release();
			m_depthStencilView = 0;
		}

		if (m_depthStencilState)
		{
			m_depthStencilState->Release();
			m_depthStencilState = 0;
		}

		if (m_depthStencilBuffer)
		{
			m_depthStencilBuffer->Release();
			m_depthStencilBuffer = 0;
		}
		*/

		if (m_renderTargetView)
		{
			m_renderTargetView->Release();
			m_renderTargetView = 0;
		}

		if (m_swapChain)
		{
			m_swapChain->Release();
			m_swapChain = 0;
		}
	}

	//***************************************************************************
	void RenderWindow::SetupBackBuffers(int width, int height)
	{
		mBackBufferWidth = width;
		mBackBufferHeight = height;

		ID3D11Texture2D* backBufferPtr;
		D3D11_TEXTURE2D_DESC depthBufferDesc;
		//D3D11_DEPTH_STENCIL_DESC depthStencilDesc;
		//D3D11_DEPTH_STENCIL_VIEW_DESC depthStencilViewDesc;

		ID3D11Device* device = Direct3D11Device::GetInstance()->GetRawDevice();
		ID3D11DeviceContext* deviceContext = Direct3D11Device::GetInstance()->GetDeviceContext();


		// Get the pointer to the back buffer.
		HRESULT result = m_swapChain->GetBuffer(0, __uuidof(ID3D11Texture2D), (LPVOID*)&backBufferPtr);
		if (FAILED(result))
		{
			return;
		}

		// Create the render target view with the back buffer pointer.
		result = device->CreateRenderTargetView(backBufferPtr, NULL, &m_renderTargetView);
		if (FAILED(result))
		{
			return;
		}

		// Release pointer to the back buffer as we no longer need it.
		backBufferPtr->Release();
		backBufferPtr = 0;

		/*
		// Initialize the description of the depth buffer.
		ZeroMemory(&depthBufferDesc, sizeof(depthBufferDesc));

		// Set up the description of the depth buffer.
		depthBufferDesc.Width = width;
		depthBufferDesc.Height = height;
		depthBufferDesc.MipLevels = 1;
		depthBufferDesc.ArraySize = 1;
		depthBufferDesc.Format = DXGI_FORMAT_D24_UNORM_S8_UINT;
		depthBufferDesc.SampleDesc.Count = 1;
		depthBufferDesc.SampleDesc.Quality = 0;
		depthBufferDesc.Usage = D3D11_USAGE_DEFAULT;
		depthBufferDesc.BindFlags = D3D11_BIND_DEPTH_STENCIL;
		depthBufferDesc.CPUAccessFlags = 0;
		depthBufferDesc.MiscFlags = 0;
		
		
		// Create the texture for the depth buffer using the filled out description.
	//	result = device->CreateTexture2D(&depthBufferDesc, NULL, &m_depthStencilBuffer);
	//	if (FAILED(result))
	//	{
		//	return;
		//}

		// Initialize the description of the stencil state.
		ZeroMemory(&depthStencilDesc, sizeof(depthStencilDesc));

		// Set up the description of the stencil state.
		depthStencilDesc.DepthEnable = true;
		depthStencilDesc.DepthWriteMask = D3D11_DEPTH_WRITE_MASK_ALL;
		depthStencilDesc.DepthFunc = D3D11_COMPARISON_LESS;

		depthStencilDesc.StencilEnable = true;
		depthStencilDesc.StencilReadMask = 0xFF;
		depthStencilDesc.StencilWriteMask = 0xFF;

		// Stencil operations if pixel is front-facing.
		depthStencilDesc.FrontFace.StencilFailOp = D3D11_STENCIL_OP_KEEP;
		depthStencilDesc.FrontFace.StencilDepthFailOp = D3D11_STENCIL_OP_INCR;
		depthStencilDesc.FrontFace.StencilPassOp = D3D11_STENCIL_OP_KEEP;
		depthStencilDesc.FrontFace.StencilFunc = D3D11_COMPARISON_ALWAYS;

		// Stencil operations if pixel is back-facing.
		depthStencilDesc.BackFace.StencilFailOp = D3D11_STENCIL_OP_KEEP;
		depthStencilDesc.BackFace.StencilDepthFailOp = D3D11_STENCIL_OP_DECR;
		depthStencilDesc.BackFace.StencilPassOp = D3D11_STENCIL_OP_KEEP;
		depthStencilDesc.BackFace.StencilFunc = D3D11_COMPARISON_ALWAYS;

		// Create the depth stencil state.
		result = device->CreateDepthStencilState(&depthStencilDesc, &m_depthStencilState);
		if (FAILED(result))
		{
			return;
		}

		// Set the depth stencil state.
		deviceContext->OMSetDepthStencilState(m_depthStencilState, 1);

		// Initialize the depth stencil view.
		ZeroMemory(&depthStencilViewDesc, sizeof(depthStencilViewDesc));

		// Set up the depth stencil view description.
		depthStencilViewDesc.Format = DXGI_FORMAT_D24_UNORM_S8_UINT;
		depthStencilViewDesc.ViewDimension = D3D11_DSV_DIMENSION_TEXTURE2D;
		depthStencilViewDesc.Texture2D.MipSlice = 0;

		// Create the depth stencil view.
		result = device->CreateDepthStencilView(m_depthStencilBuffer, &depthStencilViewDesc, &m_depthStencilView);
		if (FAILED(result))
		{
			Error("Could not create stencil view");
			return;
		}
		*/


		SetAsCurrentRenderTarget();

		return;
	}

	//***************************************************************************
	void RenderWindow::SetAsCurrentRenderTarget()
	{
		D3D11_VIEWPORT viewport;

		ID3D11DeviceContext* deviceContext = Direct3D11Device::GetInstance()->GetDeviceContext();
		// Setup the viewport for rendering.
		viewport.Width = (float)mBackBufferWidth;
		viewport.Height = (float)mBackBufferHeight;
		viewport.MinDepth = 0.0f;
		viewport.MaxDepth = 1.0f;
		viewport.TopLeftX = 0.0f;
		viewport.TopLeftY = 0.0f;

		// Create the viewport.
		deviceContext->RSSetViewports(1, &viewport);
		deviceContext->OMSetRenderTargets(1, &m_renderTargetView, NULL);
		Direct3D11Device::GetInstance()->ClearCurrentRenderTargetTexture();

	}

	//***************************************************************************
	IDXGISwapChain* RenderWindow::GetSwapChain()
	{
		return m_swapChain;
	}

	//***************************************************************************
	void RenderWindow::SetSwapChain(IDXGISwapChain* chain)
	{
		m_swapChain = chain;
	}

	//***************************************************************************
	HWND RenderWindow::GetWindowHandle()
	{
		return m_hWnd;
	}

	//***************************************************************************
	ID3D11RenderTargetView* RenderWindow::GetRenderTargetView()
	{
		return m_renderTargetView;
	}

	//***************************************************************************
	void RenderWindow::PresentToWindow()
	{
		m_swapChain->Present(0, 0);
	}

	//***************************************************************************
	int RenderWindow::GetBackBufferWidth()
	{
		return mBackBufferWidth;
	}

	//***************************************************************************
	int RenderWindow::GetBackBufferHeight()
	{
		return mBackBufferHeight;
	}
	
	

}