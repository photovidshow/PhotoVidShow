#include "stdafx.h"
#include "VertexShaderEffect.h"
#include "Direct3D11Device.h"
#include "Texture.h"
#include "errors.h"
#include "PixelShaderEffect.h"
#include "BeginSceneParameters.h"


namespace Direct3D
{

	Direct3D11Device* Direct3D11Device::mSingleInstance = NULL;

	struct myvector
	{
		float x;
		float y;
		float z;

		myvector() {}
		myvector(float _x, float _y, float _z) : x(_x), y(_y), z(_z) {}

	};

	struct mycolor
	{
		float r;
		float g;
		float b;
		float a;

		mycolor() {}
		mycolor(float _r, float _g, float _b, float _a) : r(_r), g(_g), b(_b), a(_a) {}

	};

	struct mytextcord
	{
		float x;
		float y;

		mytextcord() {}
		mytextcord(float _x, float _y) : x(_x), y(_y)  {}

	};

	struct VertexType
	{
		myvector position;
		mytextcord texture1;
		mytextcord texture2;
	};


	Direct3D11Device::Direct3D11Device(HWND hWnd, unsigned int width, unsigned int height)
	{
		HRESULT result;
		IDXGIAdapter* adapter;
		IDXGIOutput* adapterOutput;
		unsigned int numModes, i, numerator, denominator, stringLength;
		DXGI_MODE_DESC* displayModeList;
		DXGI_ADAPTER_DESC adapterDesc;
		int error;
		DXGI_SWAP_CHAIN_DESC swapChainDesc;
		D3D_FEATURE_LEVEL featureLevel;
	

		bool fullscreen = false;

		m_device = 0;
		m_deviceContext = 0;
		m_rasterState = 0;
		m_vertexBuffer = 0;
		m_indexBuffer = 0;
		m_sampleState = 0;
		m_blendState = 0;
		m_blendNoAlphaState = 0;
		mFactory = 0;
		mFactory2 = 0;
		mMainRenderWindow = 0;
		mAuxRenderWindow = 0;
		mCurrentRenderTarget = NULL;
		mCaptureTexture = NULL;
		mCaptureHeight = 0;
		mCaptureWidth = 0;

		mSingleInstance = this;

		//width = 1280;
		//	height = 718;

		// Store the vsync setting.
		m_vsync_enabled = false;

		// Create a DirectX graphics interface factory.
		result = CreateDXGIFactory(__uuidof(IDXGIFactory), (void**)&mFactory);
		if (FAILED(result))
		{
			Error("Failed to create DXGI factory");
			return;
		}

		// Create a DirectX graphics interface factory2 needed to create special swap chains.
		result = mFactory->QueryInterface(__uuidof(IDXGIFactory2), reinterpret_cast<void**>(&mFactory2));

		if (FAILED(result))
		{
			Error("Failed to create DXGI factory2");
			return;
		}

		// Use the factory to create an adapter for the primary graphics interface (video card).
		result = mFactory->EnumAdapters(0, &adapter);
		if (FAILED(result))
		{
			return;
		}

		// Enumerate the primary adapter output (monitor).
		result = adapter->EnumOutputs(0, &adapterOutput);
		if (FAILED(result))
		{
			return;
		}

		// Get the number of modes that fit the DXGI_FORMAT_R8G8B8A8_UNORM display format for the adapter output (monitor).
		result = adapterOutput->GetDisplayModeList(DXGI_FORMAT_R8G8B8A8_UNORM, DXGI_ENUM_MODES_INTERLACED, &numModes, NULL);
		if (FAILED(result))
		{
			return;
		}

		// Create a list to hold all the possible display modes for this monitor/video card combination.
		displayModeList = new DXGI_MODE_DESC[numModes];
		if (!displayModeList)
		{
			return;
		}

		// Now fill the display mode list structures.
		result = adapterOutput->GetDisplayModeList(DXGI_FORMAT_R8G8B8A8_UNORM, DXGI_ENUM_MODES_INTERLACED, &numModes, displayModeList);
		if (FAILED(result))
		{
			return;
		}

		// Now go through all the display modes and find the one that matches the screen width and height.
		// When a match is found store the numerator and denominator of the refresh rate for that monitor.
		for (i = 0; i < numModes; i++)
		{
			if (displayModeList[i].Width == (unsigned int)width)
			{
				if (displayModeList[i].Height == (unsigned int)height)
				{
					numerator = displayModeList[i].RefreshRate.Numerator;
					denominator = displayModeList[i].RefreshRate.Denominator;
				}
			}
		}

		// Get the adapter (video card) description.
		result = adapter->GetDesc(&adapterDesc);
		if (FAILED(result))
		{
			return;
		}

		// Store the dedicated video card memory in megabytes.
		m_videoCardMemory = (int)(adapterDesc.DedicatedVideoMemory / 1024 / 1024);

		// Convert the name of the video card to a character array and store it.
		error = wcstombs_s(&stringLength, m_videoCardDescription, 128, adapterDesc.Description, 128);
		if (error != 0)
		{
			return;
		}

		// Release the display mode list.
		delete[] displayModeList;
		displayModeList = 0;

		// Release the adapter output.
		adapterOutput->Release();
		adapterOutput = 0;

		// Release the adapter.
		adapter->Release();
		adapter = 0;

		// Set the feature level to DirectX 11.
		featureLevel = D3D_FEATURE_LEVEL_11_0;

		mMainRenderWindow = new RenderWindow(hWnd);
		mCurrentRenderWindow = mMainRenderWindow;
		CreateSwapChainDesc(hWnd, width, height, &swapChainDesc);

		IDXGISwapChain* chain;
		// Create the swap chain, Direct3D device, and Direct3D device context.
		result = D3D11CreateDeviceAndSwapChain(NULL, D3D_DRIVER_TYPE_HARDWARE, NULL, 0, &featureLevel, 1,
			D3D11_SDK_VERSION, &swapChainDesc, &chain, &m_device, NULL, &m_deviceContext);
		if (FAILED(result))
		{
			return;
		}

		mMainRenderWindow->SetSwapChain(chain);
		mMainRenderWindow->SetupBackBuffers(width, height);

		// Bind the render target view and depth stencil buffer to the output render pipeline.

		D3D11_RASTERIZER_DESC rasterDesc;
		// Setup the raster description which will determine how and what polygons will be drawn.
		memset(&rasterDesc, 0, sizeof(D3D11_RASTERIZER_DESC));

		rasterDesc.AntialiasedLineEnable = false;
		rasterDesc.CullMode = D3D11_CULL_BACK;
		rasterDesc.DepthBias = 0;
		rasterDesc.DepthBiasClamp = 0.0f;
		rasterDesc.DepthClipEnable = true;
		rasterDesc.FillMode = D3D11_FILL_SOLID;
		rasterDesc.FrontCounterClockwise = false;
		rasterDesc.MultisampleEnable = false;
		rasterDesc.ScissorEnable = false;
		rasterDesc.SlopeScaledDepthBias = 0.0f;

		// Create the rasterizer state from the description we just filled out.
		result = m_device->CreateRasterizerState(&rasterDesc, &m_rasterState);
		if (FAILED(result))
		{
			Error("Could not create rasterizer state");
			return;
		}

		// Now set the rasterizer state.
		m_deviceContext->RSSetState(m_rasterState);

		D3D11_SAMPLER_DESC samplerDesc;

		// Create a texture sampler state description.
		samplerDesc.Filter = D3D11_FILTER_MIN_MAG_MIP_LINEAR;
		samplerDesc.AddressU = D3D11_TEXTURE_ADDRESS_WRAP;
		samplerDesc.AddressV = D3D11_TEXTURE_ADDRESS_WRAP;
		samplerDesc.AddressW = D3D11_TEXTURE_ADDRESS_WRAP;
		samplerDesc.MipLODBias = 0.0f;
		samplerDesc.MaxAnisotropy = 1;
		samplerDesc.ComparisonFunc = D3D11_COMPARISON_ALWAYS;
		samplerDesc.BorderColor[0] = 0;
		samplerDesc.BorderColor[1] = 0;
		samplerDesc.BorderColor[2] = 0;
		samplerDesc.BorderColor[3] = 0;
		samplerDesc.MinLOD = 0;
		samplerDesc.MaxLOD = D3D11_FLOAT32_MAX;

		// Create the texture sampler state.
		result = m_device->CreateSamplerState(&samplerDesc, &m_sampleState);


		D3D11_BLEND_DESC blendDesc;
		ZeroMemory(&blendDesc, sizeof(D3D11_BLEND_DESC));
		blendDesc.AlphaToCoverageEnable = false;
		blendDesc.IndependentBlendEnable = false;
		blendDesc.RenderTarget[0].BlendEnable = true;
		blendDesc.RenderTarget[0].SrcBlend = D3D11_BLEND_SRC_ALPHA;
		blendDesc.RenderTarget[0].DestBlend = D3D11_BLEND_INV_SRC_ALPHA;
		blendDesc.RenderTarget[0].BlendOp = D3D11_BLEND_OP_ADD;

		blendDesc.RenderTarget[0].SrcBlendAlpha = D3D11_BLEND_ZERO;        ///tryed D3D11_BLEND_ONE ... (and others desperate combinations ... )
		blendDesc.RenderTarget[0].DestBlendAlpha = D3D11_BLEND_ONE;     ///tryed D3D11_BLEND_ONE ... (and others desperate combinations ... )
		blendDesc.RenderTarget[0].BlendOpAlpha = D3D11_BLEND_OP_ADD;
		blendDesc.RenderTarget[0].RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;
		result = m_device->CreateBlendState(&blendDesc, &m_blendState);
		if (FAILED(result))
		{
			Error("Failed to create blend state");
		}

		D3D11_BLEND_DESC blendNoAlphaDesc;
		ZeroMemory(&blendNoAlphaDesc, sizeof(D3D11_BLEND_DESC));
		blendNoAlphaDesc.AlphaToCoverageEnable = false;
		blendNoAlphaDesc.IndependentBlendEnable = false;
		blendNoAlphaDesc.RenderTarget[0].BlendEnable = true;
		blendNoAlphaDesc.RenderTarget[0].SrcBlend = D3D11_BLEND_ONE;
		blendNoAlphaDesc.RenderTarget[0].DestBlend = D3D11_BLEND_ZERO;
		blendNoAlphaDesc.RenderTarget[0].BlendOp = D3D11_BLEND_OP_ADD;
		blendNoAlphaDesc.RenderTarget[0].SrcBlendAlpha = D3D11_BLEND_ZERO;        ///tryed D3D11_BLEND_ONE ... (and others desperate combinations ... )
		blendNoAlphaDesc.RenderTarget[0].DestBlendAlpha = D3D11_BLEND_ONE;     ///tryed D3D11_BLEND_ONE ... (and others desperate combinations ... )
		blendNoAlphaDesc.RenderTarget[0].BlendOpAlpha = D3D11_BLEND_OP_ADD;
		blendNoAlphaDesc.RenderTarget[0].RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;

		result = m_device->CreateBlendState(&blendNoAlphaDesc, &m_blendNoAlphaState);
		if (FAILED(result))
		{
			Error("Failed to create blend state no alpha");
		}

		SetupDeviceResources();

		CreateDefaultShaders();
	}

	void Direct3D11Device::CreateSwapChainDesc(HWND hWnd, int width, int height, DXGI_SWAP_CHAIN_DESC* swapChainDesc)
	{
		// Initialize the swap chain description.
		ZeroMemory(swapChainDesc, sizeof(*swapChainDesc));

		// Set to a single back buffer.
		swapChainDesc->BufferCount = 1;

		// Set the width and height of the back buffer.
		swapChainDesc->BufferDesc.Width = width;
		swapChainDesc->BufferDesc.Height = height;

		// Set regular 32-bit surface for the back buffer.
		swapChainDesc->BufferDesc.Format = DXGI_FORMAT_B8G8R8A8_UNORM; //DXGI_FORMAT_R8G8B8A8_UNORM;

		swapChainDesc->BufferDesc.RefreshRate.Numerator = 0;
		swapChainDesc->BufferDesc.RefreshRate.Denominator = 1;

		// Set the usage of the back buffer.
		swapChainDesc->BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;

		// Set the handle for the window to render to.
		swapChainDesc->OutputWindow = hWnd;

		// Turn multisampling off.
		swapChainDesc->SampleDesc.Count = 1;
		swapChainDesc->SampleDesc.Quality = 0;

		swapChainDesc->Windowed = true;

		// Set the scan line ordering and scaling to unspecified.
		swapChainDesc->BufferDesc.ScanlineOrdering = DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED;
		swapChainDesc->BufferDesc.Scaling = DXGI_MODE_SCALING_UNSPECIFIED;

		// Discard the back buffer contents after presenting.
		swapChainDesc->SwapEffect = DXGI_SWAP_EFFECT_DISCARD;

		// Don't set the advanced flags.
		swapChainDesc->Flags = 0;
	}

	void Direct3D11Device::SetCurrentRenderWindow(HWND Hwnd, int width, int height)
	{
		//
		// Render to main render window?
		//
		if (Hwnd== NULL || mMainRenderWindow->GetWindowHandle() == Hwnd)
		{
			if (mCurrentRenderWindow != mMainRenderWindow)
			{
				mCurrentRenderWindow = mMainRenderWindow;
				mCurrentRenderWindow->SetAsCurrentRenderTarget();
			}
			return;
		}
		else
		{
			//
			// Render to auxialry window
			//
			if (mAuxRenderWindow != NULL)
			{
				if (mAuxRenderWindow->GetWindowHandle() == Hwnd)
				{
					if (mCurrentRenderWindow != mAuxRenderWindow)
					{
						mCurrentRenderWindow = mAuxRenderWindow;
						mCurrentRenderWindow->SetAsCurrentRenderTarget();
					}
					return;
				}

				//
				// Delete current auxilary window and create a new one
				//
				delete mAuxRenderWindow;
				mAuxRenderWindow = NULL;
			}
	
			// Create new swap chain and view

			DXGI_SWAP_CHAIN_DESC1 minChain;
			ZeroMemory(&minChain, sizeof(minChain));
			minChain.BufferCount = 1;
			minChain.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
			minChain.SampleDesc.Count = 1;
			minChain.SampleDesc.Quality = 0;
			minChain.SwapEffect = DXGI_SWAP_EFFECT_DISCARD;
			minChain.Flags = 0;
			minChain.Width = width;
			minChain.Height = height;
			minChain.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
			minChain.Stereo = false;
			minChain.Scaling = DXGI_SCALING_STRETCH;
	
			IDXGISwapChain1* newChain;
			HRESULT result = mFactory2->CreateSwapChainForHwnd(m_device, Hwnd,  &minChain,NULL, NULL, &newChain);

			mAuxRenderWindow = new RenderWindow(Hwnd);
			mAuxRenderWindow->SetSwapChain(newChain);
			mAuxRenderWindow->SetupBackBuffers(width, height);
			mCurrentRenderWindow = mAuxRenderWindow;
		}
	}

	


	ID3D11DeviceContext* Direct3D11Device::GetDeviceContext()
	{
		return m_deviceContext;
	}

	void Direct3D11Device::ReleaseD3DDevice()
	{

		// Release the factory.
		if (mFactory != NULL)
		{
			mFactory->Release();
			mFactory = NULL;
		}

		if (mFactory2 != NULL)
		{
			mFactory2->Release();
			mFactory2 = NULL;
		}

		if (m_blendState != NULL)
		{
			m_blendState->Release();
			m_blendState = NULL;
		}

		if (m_blendNoAlphaState != NULL)
		{
			m_blendNoAlphaState->Release();
			m_blendNoAlphaState = NULL;
		}

		if (mCaptureTexture != NULL)
		{
			mCaptureTexture->Release();
			mCaptureTexture = NULL;
		}

		if (m_rasterState)
		{
			m_rasterState->Release();
			m_rasterState = 0;
		}

		if (mDefaultVertexShader)
		{
			delete mDefaultVertexShader;
			mDefaultVertexShader = NULL;
		}

		if (mDefaultPixelShader)
		{
			delete mDefaultPixelShader;
			mDefaultPixelShader = NULL;
		}

		if (mDiffusePixelShader)
		{
			delete mDiffusePixelShader;
			mDiffusePixelShader = NULL;
		}

		// Release the index buffer.
		if (m_indexBuffer)
		{
			m_indexBuffer->Release();
			m_indexBuffer = 0;
		}

		// Release the vertex buffer.
		if (m_vertexBuffer)
		{
			m_vertexBuffer->Release();
			m_vertexBuffer = 0;
		}

		if (m_deviceContext)
		{
			m_deviceContext->Release();
			m_deviceContext = 0;
		}

		if (m_device)
		{
			m_device->Release();
			m_device = 0;
		}
	}


	//***************************************************************************
	Direct3D11Device::~Direct3D11Device()
	{
		ReleaseD3DDevice();
	}


	//***************************************************************************
	Direct3D11Device* Direct3D11Device::GetInstance()
	{
		return mSingleInstance;
	}

	//***************************************************************************
	ID3D11Device* Direct3D11Device::GetRawDevice()
	{
		return m_device;
	}


	bool Direct3D11Device::Render()
	{	
		return true;
	}


	//***************************************************************************
	void Direct3D11Device::SetupDeviceResources()
	{
		VertexType* vertices;
		unsigned long* indices;
		D3D11_BUFFER_DESC vertexBufferDesc, indexBufferDesc;
		D3D11_SUBRESOURCE_DATA vertexData, indexData;
		HRESULT result;

		// Set the number of vertices in the vertex array.
		m_vertexCount = 4;

		// Set the number of indices in the index array.
		m_indexCount = 4;

		// Create the vertex array.
		vertices = new VertexType[m_vertexCount];
		if (!vertices)
		{
			return;
		}

		// Create the index array.
		indices = new unsigned long[m_indexCount];
		if (!indices)
		{
			Error("Failed to create index buffer");
			return;
		}

		// Load the vertex array with data.		## SRG do we need to do this?
		vertices[0].position = myvector(-1.0f, -1.0f, 0.0f);  // Bottom left.
		vertices[0].texture1 = mytextcord(0.0f, 1.0f);
		vertices[1].position = myvector(-1.0f, 1.0f, 0.0f);  // Top left.
		vertices[1].texture1 = mytextcord(0.0f, 0.0f);
		vertices[2].position = myvector(1.0f, -1.0f, 0.0f);  // Bottom right.
		vertices[2].texture1 = mytextcord(1.0f, 1.0f);
		vertices[3].position = myvector(1.0f, 1.0f, 0.0f);  // Top right.
		vertices[3].texture1 = mytextcord(1.0f, 0.0f);


		// Load the index array with data.
		indices[0] = 0;  // Bottom left.
		indices[1] = 1;  // Top left.
		indices[2] = 2;  // Bottom right.
		indices[3] = 3;  // Top right.

		// Set up the description of the static vertex buffer.
		ZeroMemory(&vertexBufferDesc, sizeof(vertexBufferDesc));
		vertexBufferDesc.Usage = D3D11_USAGE_DYNAMIC;
		vertexBufferDesc.ByteWidth = sizeof(VertexType) * m_vertexCount;
		vertexBufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
		vertexBufferDesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
		vertexBufferDesc.MiscFlags = 0;
		vertexBufferDesc.StructureByteStride = 0;

		// Give the subresource structure a pointer to the vertex data.
		ZeroMemory(&vertexData, sizeof(vertexData));
		vertexData.pSysMem = vertices;
		vertexData.SysMemPitch = 0;
		vertexData.SysMemSlicePitch = 0;

		// Now create the vertex buffer.
		result = m_device->CreateBuffer(&vertexBufferDesc, &vertexData, &m_vertexBuffer);
		if (FAILED(result))
		{
			Error("Failed to create vertex buffer");
			return ;
		}

		// Set up the description of the static index buffer.
		indexBufferDesc.Usage = D3D11_USAGE_DEFAULT;
		indexBufferDesc.ByteWidth = sizeof(unsigned long) * m_indexCount;
		indexBufferDesc.BindFlags = D3D11_BIND_INDEX_BUFFER;
		indexBufferDesc.CPUAccessFlags = 0;
		indexBufferDesc.MiscFlags = 0;
		indexBufferDesc.StructureByteStride = 0;

		// Give the subresource structure a pointer to the index data.
		indexData.pSysMem = indices;
		indexData.SysMemPitch = 0;
		indexData.SysMemSlicePitch = 0;

		// Create the index buffer.
		result = m_device->CreateBuffer(&indexBufferDesc, &indexData, &m_indexBuffer);
		if (FAILED(result))
		{
			Error("Failed to create index buffer");
			return;
		}

		// Release the arrays now that the vertex and index buffers have been created and loaded.
		delete[] vertices;
		vertices = 0;

		delete[] indices;
		indices = 0;

		return;

	}

	//***************************************************************************
	// Setups a vertexbufer buffer with a triangle strip which represents an image rectangle
	void Direct3D11Device::SetupSingleImageVertexBuffer(Texture* texture, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2, float dest_x3, float dest_y3, float dest_x4, float dest_y4)
	{
		if (mCurrentRenderWindow == NULL)
		{
			return;
		}

		VertexType vertices[4];

		float text2SrcLeft = 0;
		float text2SrcTop = 0;
		float text2SrcRight = 1;
		float text2SrcBottom = 1;

		// If we have a pixelshader set and with input texture set. Then we should add half a pixel to T2 uv coords
		if (mCurrentPixelShaderEffect.ToRead() != NULL)
		{
			Texture* input_texture = mCurrentPixelShaderEffect->GetTexture1();
			if (input_texture != NULL)
			{
				input_texture->ConvertSrcToUVCoordinates(&text2SrcLeft, &text2SrcTop, &text2SrcRight, &text2SrcBottom);
			}
		}

		int backBufferWidth = mCurrentRenderWindow->GetBackBufferWidth();
		int backBufferHeight = mCurrentRenderWindow->GetBackBufferHeight();


		texture->ConvertSrcToUVCoordinates(&src_x, &src_y, &src_width, &src_height);

		float width = (float)backBufferWidth;
		width /= 2;
		float height = (float)backBufferHeight;
		height /= 2;

		float d1x = (dest_x1 / width) - 1.0f;
		float d1y = (dest_y1 / height) -1.0f;
		float d2x = (dest_x2 / width) - 1.0f;
		float d2y = (dest_y2 / height) - 1.0f;
		float d3x = (dest_x3 / width) - 1.0f;
		float d3y = (dest_y3 / height) - 1.0f;
		float d4x = (dest_x4 / width) - 1.0f;
		float d4y = (dest_y4 / height) - 1.0f;


		// Load the vertex array with data.
		vertices[0].position = myvector(d1x, -d1y, 0.0f);  // Bottom left.
		vertices[0].texture1 = mytextcord(src_x, src_height);
		vertices[0].texture2 = mytextcord(text2SrcLeft, text2SrcBottom);

		vertices[1].position = myvector(d2x, -d2y, 0.0f);  // Top left.
		vertices[1].texture1 = mytextcord(src_x, src_y);
		vertices[1].texture2 = mytextcord(text2SrcLeft, text2SrcTop);

		vertices[2].position = myvector(d3x, -d3y, 0.0f);  // Bottom right.
		vertices[2].texture1 = mytextcord(src_width, src_height);
		vertices[2].texture2 = mytextcord(text2SrcRight, text2SrcBottom);

		vertices[3].position = myvector(d4x, -d4y, 0.0f);  // Top right.
		vertices[3].texture1 = mytextcord(src_width, src_y);
		vertices[3].texture2 = mytextcord(text2SrcRight, text2SrcTop);

		D3D11_MAPPED_SUBRESOURCE resource;
		HRESULT hr = m_deviceContext->Map(m_vertexBuffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &resource);
		if (hr != S_OK)
		{
			ErrorOnce("Failed to map vertex buffer %d", hr));
			return;
		}
		memcpy(resource.pData, &vertices[0], sizeof(vertices));
		m_deviceContext->Unmap(m_vertexBuffer, 0);

	}

	//***************************************************************************
	void Direct3D11Device::CreateDefaultShaders()
	{

		mDefaultVertexShader = new VertexShaderEffect("shaders\\default.vsh", FALSE);
		mDefaultPixelShader = new PixelShaderEffect("shaders\\default.psh");
		mDiffusePixelShader = new PixelShaderEffect("shaders\\defaultDiffuse.psh");

		SetVertexShaderEffect(NULL);
		SetPixelShaderEffect(NULL);
	}

	//***************************************************************************
	bool Direct3D11Device::HalDeviceCreated()
	{
		return true;
	}

	//***************************************************************************
	void Direct3D11Device::SetBilinearInterpolation(int value)
	{
		
	}


	//***************************************************************************
	DEVICE_STATE Direct3D11Device::TestCooperativeLevel()
	{
		return DEVICE_OK;
	}

	//***************************************************************************
	DEVICE_STATE Direct3D11Device::Reset()
	{
		/*
		// 1. Clear the existing references to the backbuffer
		ID3D11RenderTargetView* nullViews[] = { nullptr };
		m_deviceContext->OMSetRenderTargets(ARRAYSIZE(nullViews), nullViews, nullptr);
		m_renderTargetView->Release(); // Microsoft::WRL::ComPtr here does a Release();
		m_depthStencilView->Release();
		m_deviceContext->Flush();

		// 2. Resize the existing swapchain
		HRESULT hr = m_swapChain->ResizeBuffers(0, mBackBufferWidth, mBackBufferHeight, DXGI_FORMAT_UNKNOWN, 0);
		if (hr == DXGI_ERROR_DEVICE_REMOVED || hr == DXGI_ERROR_DEVICE_RESET)
			// You have to destroy the device, swapchain, and all resources and
			// recreate them to recover from this case. The device was hardware reset,
			// physically removed, or the driver was updated and/or restarted
		{
			ErrorOnce("Could not reset device"));
			return DEVICE_OK;
		}


		SetupBackBuffers();

		// Bind the render target view and depth stencil buffer to the output render pipeline.
		m_deviceContext->OMSetRenderTargets(1, &m_renderTargetView, NULL);
		mUsingMainChain = TRUE;
		*/
		return DEVICE_OK;
		
	}

	//***************************************************************************
	DEVICE_STATE Direct3D11Device::Reset(unsigned int width, unsigned int height)
	{
		if (width < 1000)
		{
			return DEVICE_OK;
		}
	//	if (mBackBufferWidth == width &&
	//		mBackBufferHeight == height)
	//	{
	//		return DEVICE_OK;
	//	}
		Reset();
		return DEVICE_OK;
	}


	//***************************************************************************
	void Direct3D11Device::BeginScene(BeginSceneParameters& begin_scene_parameters)
	{
		SetCurrentRenderWindow(begin_scene_parameters.mHWnd, begin_scene_parameters.mWidth, begin_scene_parameters.mHeight);
	}

	//***************************************************************************
	void Direct3D11Device::ClearCurrentRenderTargetTexture()
	{
		mCurrentRenderTarget = NULL;
	}

	//***************************************************************************
	void Direct3D11Device::SetRenderTargetToBackBuffer()
	{
		mCurrentRenderTarget = NULL;

		if (mCurrentRenderWindow != NULL)
		{
			mCurrentRenderWindow->SetAsCurrentRenderTarget();
		}
	}

	//***************************************************************************
	void Direct3D11Device::ClearRenderTarget(int r, int g, int b, int a)
	{
		ID3D11RenderTargetView* view = NULL;

		//
		// If render target set use that
		//
		if (mCurrentRenderTarget != NULL)
		{
			view = mCurrentRenderTarget->GetRenderTargetView();
		}
		else
		{
			// else use current render window back buffer view
			view = mCurrentRenderWindow->GetRenderTargetView();
		}

		// ### SRG To to color
		float color[4];
		// Setup the color to clear the buffer to.
		color[0] = 0;
		color[1] = 0;
		color[2] = 0;
		color[3] = 1;

		this->m_deviceContext->ClearRenderTargetView(view, color);
		//deviceContext->ClearDepthStencilView(m_depthStencilView, D3D11_CLEAR_DEPTH, 1.0f, 0);

		/*
		CHECK_RETURN(mDirect3DDevice->Clear(0, NULL, D3DCLEAR_TARGET, D3DCOLOR_RGBA(r, g, b, a), 1.0f, 0), g_failed_to_clear_render_target_string);

		if (r == 0 && g == 0 && b == 0 && a == 0)
		{
		mCurrentRenderTargetSupportsAlphaBlending = false; // disable alpha blending when rendering to the current RenderTarget
		}
		else
		{
		mCurrentRenderTargetSupportsAlphaBlending = true;
		}*/
	}

	//***************************************************************************
	void Direct3D11Device::SetRenderTarget(Surface* surface)
	{
		if (surface == NULL)
		{
			SetRenderTargetToBackBuffer();
			return;
		}

		Texture* texture = surface->GetTexture();
		if (texture == NULL)
		{
			return;
		}
		ID3D11RenderTargetView* RTview = texture->GetRenderTargetView();
		if (RTview == NULL)
		{
			return;
		}

		D3D11_VIEWPORT viewport;

		ID3D11DeviceContext* deviceContext = Direct3D11Device::GetInstance()->GetDeviceContext();
	
		m_deviceContext->OMSetRenderTargets(1, &RTview, NULL);
		mCurrentRenderTarget = texture;

	}

	//***************************************************************************
	void Direct3D11Device::EndScene()
	{
		
	}

	// If first paremeter null presents to default window.
	// Second paremater is only used if first parameter is not null, and if set true, scales the current 
	// backbuffer to the given window else does a direct copy from the back buffer to the window.
	//***************************************************************************
	DEVICE_STATE Direct3D11Device::PresentToWindow(HWND hWnd, bool scaleBackBufferToWindow)
	{
		if (mCurrentRenderWindow != NULL)
		{
			mCurrentRenderWindow->PresentToWindow();
		}

		return DEVICE_OK;
	}

	//***************************************************************************
	void Direct3D11Device::SetViewport(unsigned int width, unsigned int height)
	{
	
	}

	//***************************************************************************
	void Direct3D11Device::DrawImage(Texture* texture, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2, float dest_x3, float dest_y3, float dest_x4, float dest_y4, bool alpha)
	{
		//if (texture==NULL)
	//	{
			//ErrorOnce("Direct3DDevice::DrawImage No texture to render with"));
		//	return;
	//	}

//		if (mBegunScene == false)
	//	{
			//ErrorOnce("Direct3DDevice::DrawImage Scene not begun can not render image"));
			//return;
//		}

		ID3D11Texture2D* t= texture->GetD3DTexture();
		if (t==NULL)
		{
			ErrorOnce("Direct3DDevice::DrawImage No raw texture to render with"));
			return;
		}

		unsigned int stride;
		unsigned int offset;


		// Set vertex buffer stride and offset.
		stride = sizeof(VertexType);
		offset = 0;

		// Set the vertex buffer to active in the input assembler so it can be rendered.
		m_deviceContext->IASetVertexBuffers(0, 1, &m_vertexBuffer, &stride, &offset);

		// Set the index buffer to active in the input assembler so it can be rendered.
		m_deviceContext->IASetIndexBuffer(m_indexBuffer, DXGI_FORMAT_R32_UINT, 0);


		SetupSingleImageVertexBuffer(texture, src_x, src_y, src_width, src_height, dest_x1, dest_y1, dest_x2, dest_y2, dest_x3,  dest_y3, dest_x4, dest_y4);

		// Set the type of primitive that should be rendered from this vertex buffer, in this case triangles.
		m_deviceContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP);

		m_deviceContext->IASetInputLayout(this->mDefaultVertexShader->GetRawLayout());
		m_deviceContext->VSSetShader(this->mDefaultVertexShader->GetRawVertexShader(), NULL, 0);

		if (mCurrentPixelShaderEffect.ToRead() == NULL)
		{
			m_deviceContext->PSSetShader(this->mDefaultPixelShader->GetRawPixelShader(), NULL, 0);
		}
		else
		{
			mCurrentPixelShaderEffect->Setup(this);
		}

		m_deviceContext->PSSetSamplers(0, 1, &m_sampleState);

		if (alpha == TRUE)
		{
			m_deviceContext->OMSetBlendState(m_blendState, NULL, 0xffffffff);
		}
		else
		{
			m_deviceContext->OMSetBlendState(m_blendNoAlphaState, NULL, 0xffffffff);
		}

		if (texture)
		{
			ID3D11ShaderResourceView* t = texture->GetTextureView();
			if (t != NULL)
			{
				m_deviceContext->PSSetShaderResources(0, 1, &t);
				//CHECK_RETURN(device->GetRawDevice()->SetTexture(1, mTexture1->GetD3DTexture() ), "Failed to set texture");
			}
		}

		//
		// If current pixel shader has texture, set that up as well
		//
		if (mCurrentPixelShaderEffect.ToRead() != NULL)
		{
			Texture* input_texture = mCurrentPixelShaderEffect->GetTexture1();
			if (input_texture != NULL)
			{
				ID3D11ShaderResourceView* t = input_texture->GetTextureView();
				if (t != NULL)
				{
					m_deviceContext->PSSetShaderResources(1, 1, &t);
				}
			}
		}

		// Render the triangle.
		m_deviceContext->DrawIndexed(4, 0, 0);

	}

	//***************************************************************************
	void Direct3D11Device::DrawImage(Surface* surface, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2, float dest_x3, float dest_y3, float dest_x4, float dest_y4, bool alpha)
	{
		if (surface == NULL)
		{
			ErrorOnce("Direct3DDevice::DrawImage No surface to render with"));
			return;
		}
		DrawImage(surface->GetTexture(), src_x, src_y, src_width, src_height, dest_x1, dest_y1, dest_x2, dest_y2, dest_x3, dest_y3, dest_x4, dest_y4, alpha);
	}

	//***************************************************************************
	void Direct3D11Device::DrawImageWithDiffuseAlpha(Texture* texture, float alpha, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2, float dest_x3, float dest_y3, float dest_x4, float dest_y4, bool include_image_alpha)
	{
		ID3D11Texture2D* t = texture->GetD3DTexture();
		if (t == NULL)
		{
			ErrorOnce("Direct3DDevice::DrawImage No raw texture to render with"));
			return;
		}

		unsigned int stride;
		unsigned int offset;

		// Set vertex buffer stride and offset.
		stride = sizeof(VertexType);

		// Set the vertex buffer to active in the input assembler so it can be rendered.
		m_deviceContext->IASetVertexBuffers(0, 1, &m_vertexBuffer, &stride, &offset);

		// Set the index buffer to active in the input assembler so it can be rendered.
		m_deviceContext->IASetIndexBuffer(m_indexBuffer, DXGI_FORMAT_R32_UINT, 0);

		SetupSingleImageVertexBuffer(texture, src_x, src_y, src_width, src_height, dest_x1, dest_y1, dest_x2, dest_y2, dest_x3, dest_y3, dest_x4, dest_y4);

		// Set the type of primitive that should be rendered from this vertex buffer, in this case triangles.
		m_deviceContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP);

		m_deviceContext->IASetInputLayout(this->mDefaultVertexShader->GetRawLayout());
		m_deviceContext->VSSetShader(this->mDefaultVertexShader->GetRawVertexShader(), NULL, 0);
	
		m_deviceContext->PSSetSamplers(0, 1, &m_sampleState);

		
		if (mCurrentPixelShaderEffect.ToRead() != NULL)
		{
			//
			// Must be an alpha map shader, setup diffuse here
			//
			mCurrentPixelShaderEffect->SetParameter(0, alpha/255);
			mCurrentPixelShaderEffect->Setup(this);		
		}
		else
		{ 
			if (include_image_alpha == TRUE)
			{
				mDiffusePixelShader->SetParameter(0, alpha/255);
				mDiffusePixelShader->Setup(this);
			}
			else
			{
				//
				// Ok we still need OM blending because the diffuse itself may contain alpha, so instead of switching off OM blending we
				// use a different pixel shader
				//
				// ### SRG USE A DIFFERENT VERSION OF PIXEL SHADER  (ELSE VIDEO WITH TRANSPARENCY WONT WORK)
				mDiffusePixelShader->SetParameter(0, alpha / 255);
				mDiffusePixelShader->Setup(this);
			}
		}

		m_deviceContext->OMSetBlendState(m_blendState, NULL, 0xffffffff);

		if (texture)
		{
			ID3D11ShaderResourceView* t = texture->GetTextureView();
			if (t != NULL)
			{
				m_deviceContext->PSSetShaderResources(0, 1, &t);
				//CHECK_RETURN(device->GetRawDevice()->SetTexture(1, mTexture1->GetD3DTexture() ), "Failed to set texture");
			}
		}

		// Render the triangle.
		m_deviceContext->DrawIndexed(4, 0, 0);

	}

	//***************************************************************************
	void Direct3D11Device::DrawImageWithDiffuseAlpha(Surface* surface, float alpha, float src_x, float src_y, float src_width, float src_height, float dest_x1, float dest_y1, float dest_x2, float dest_y2, float dest_x3, float dest_y3, float dest_x4, float dest_y4, bool include_image_alpha)
	{
		if (surface == NULL)
		{
			ErrorOnce("Direct3DDevice::DrawImageWithDiffuseAlpha No surface to render with"));
			return;
		}
		DrawImageWithDiffuseAlpha(surface->GetTexture(), alpha, src_x, src_y, src_width, src_height, dest_x1, dest_y1, dest_x2, dest_y2, dest_x3, dest_y3, dest_x4, dest_y4, include_image_alpha);

	}

	//***************************************************************************
	void Direct3D11Device::SetPixelShaderEffect(PixelShaderEffect* effect)
	{
		mCurrentPixelShaderEffect = effect;
	}

	//***************************************************************************
	void Direct3D11Device::SetVertexShaderEffect(VertexShaderEffect* effect)
	{

		if (effect)
		{
			effect->Setup(this);
		}
		else
		{
			if (mDefaultVertexShader != NULL)
			{
				mDefaultVertexShader->Setup(this);
			}
		}
	}

	//***************************************************************************
	void Direct3D11Device::CopyDefaultSurfaceToBitmap(unsigned int* bmp_buffer, int width, int height, int d_pitch)
	{
		ID3D11Texture2D* pSurface;

		if (mCurrentRenderTarget == NULL)
		{
			HRESULT hr = mMainRenderWindow->GetSwapChain()->GetBuffer(0, __uuidof(ID3D11Texture2D), reinterpret_cast<void**>(&pSurface));
		}
		else
		{
			pSurface = mCurrentRenderTarget->GetD3DTexture();
		}

		if (pSurface)
		{
			D3D11_TEXTURE2D_DESC description;
			pSurface->GetDesc(&description);
			description.BindFlags = 0;
			description.CPUAccessFlags = D3D11_CPU_ACCESS_READ | D3D11_CPU_ACCESS_WRITE;
			description.Usage = D3D11_USAGE_STAGING;

			if (mCaptureTexture != NULL && (mCaptureWidth != width || mCaptureHeight != height))
			{
				mCaptureTexture->Release();
				mCaptureTexture = NULL;
			}

			if (mCaptureTexture == NULL)
			{
				HRESULT hr = m_device->CreateTexture2D(&description, NULL, &mCaptureTexture);
				if (hr != S_OK)
				{
					mCaptureTexture = NULL;
					// error
					return;
				}
				mCaptureWidth = width;
				mCaptureHeight = height;
			}

			if (mCaptureTexture)
			{
				m_deviceContext->CopyResource(mCaptureTexture, pSurface);
				D3D11_MAPPED_SUBRESOURCE resource;
				unsigned int subresource = D3D11CalcSubresource(0, 0, 0);
				HRESULT hr = m_deviceContext->Map(mCaptureTexture, subresource, D3D11_MAP_READ_WRITE, 0, &resource);
				if (hr == S_OK)
				{
					const int pitch = width << 2;
					const unsigned char* source = static_cast<const unsigned char*>(resource.pData);
					unsigned char* dest = (unsigned char*)bmp_buffer;

					for (int i = 0; i < height; ++i)
					{
						memcpy(dest, source, width * 4);
						source += resource.RowPitch; // <------
						dest += pitch;
					}
				}
				else
				{
					// error
				}

				m_deviceContext->Unmap(mCaptureTexture, subresource);

			}
		}
	}

	//***************************************************************************
	// Passed in surface and current render target must be 32 Bit
	// mStage is a temporty used in this build as this function has been seen to 
	// crash on a clients samsung laptop
	void Direct3D11Device::CopyDefaultSurfaceToDynamicSurface(Surface* surface)
	{
		ID3D11Texture2D* pSurface;

		HRESULT hr = 0;

		if (mCurrentRenderTarget == NULL)
		{
			hr = mMainRenderWindow->GetSwapChain()->GetBuffer(0, __uuidof(ID3D11Texture2D), reinterpret_cast<void**>(&pSurface));
		}
		else
		{
			pSurface = mCurrentRenderTarget->GetD3DTexture();
		}
		if (pSurface)
		{

			Texture* texture = surface->GetTexture();
			if (texture)
			{
			
				// SRG FIX ME may not be backbuffer ?
				if (texture->GetWidth() != mMainRenderWindow->GetBackBufferWidth() ||
					texture->GetHeight() != mMainRenderWindow->GetBackBufferHeight() )
				{
					ErrorOnce("Direct3D11Device::source and destination resolutions do not match %d %d %d %d", texture->GetWidth(), mMainRenderWindow->GetBackBufferWidth(), texture->GetHeight(), mMainRenderWindow->GetBackBufferHeight()));
					return;
				}

				ID3D11Texture2D* rawSurfaceTexture = texture->GetD3DTexture();

				if (rawSurfaceTexture)
				{
					m_deviceContext->CopyResource(rawSurfaceTexture, pSurface);
				}
				else
				{
					ErrorOnce("Direct3D11Device::CopyDefaultSurfaceToDynamicSurface no raw surface to copy from"));
				}
			}
			else
			{
				ErrorOnce("Direct3D11Device::CopyDefaultSurfaceToDynamicSurface no raw texture to copy to"));
			}
		}
		else
		{
			ErrorOnce("Direct3D11Device::Failed to get back buffer %d", hr));
		}
	}

	//***************************************************************************
	int Direct3D11Device::GetLastCopyDefaultSurfaceToDynamicSurfaceStage()
	{
		return 1;
	}


	//***************************************************************************
	int Direct3D11Device::GetBackBufferWidth()
	{
		return mMainRenderWindow->GetBackBufferWidth();
	}


	//***************************************************************************
	int Direct3D11Device::GetBackBufferHeight()
	{
		return mMainRenderWindow->GetBackBufferHeight();
	}

	//***************************************************************************
	void Direct3D11Device::ResetDefaultWindowLocationAndSize(int x, int y, int width, int height)
	{

	}

	//***************************************************************************
	unsigned int Direct3D11Device::GetAvailableTextureMem()
	{
		return 1000;
	}

	//***************************************************************************
	Settings& Direct3D11Device::GetSettings()
	{
		return mSettings;
	}

	//***************************************************************************
	Capabilities& Direct3D11Device::GetCapabilities()
	{
		return mCapabilities;
	}



}



