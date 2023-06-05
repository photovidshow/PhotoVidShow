#include "VertexShaderEffect.h"
#include "Direct3D11Device.h"
#include "d3dcompiler.h"
#include <d3d11.h> 
#include "Errors.h"

namespace Direct3D
{

	//***************************************************************************
	VertexShaderEffect::VertexShaderEffect(char* sourceFile, bool useDiffuse) :
		mVertexShader(NULL)
	{

	
		UINT flags = D3DCOMPILE_ENABLE_STRICTNESS;

		wchar_t wtext[1024] = { 0 };
		mbstowcs(wtext, sourceFile, strlen(sourceFile) + 1);//Plus null

		ID3DBlob* shaderBlob = nullptr;
		ID3DBlob* errorBlob = nullptr;				// SRG TO SET profile proper
		HRESULT result = D3DCompileFromFile(wtext, NULL, NULL, "TextureVertexShader", "vs_5_0", flags, 0, &shaderBlob, &errorBlob);

		CHECK_RETURN(result, "Invalid pixel shader code");

		if (FAILED(result))
		{
			if (errorBlob)
			{
				char* errorMessage = (char*)errorBlob->GetBufferPointer();

				Error("Could not compile %s %s", sourceFile, errorMessage);
				errorBlob->Release();
			}

			if (shaderBlob)
				shaderBlob->Release();

			return;
		}

		if (useDiffuse == TRUE)
		{
			CreateVertexShaderWithDiffuse((void*)shaderBlob);
		}
		else
		{
			CreateNormalVertexShader((void*)shaderBlob);
		}

		shaderBlob->Release();
	}

	//***************************************************************************
	void VertexShaderEffect::CreateNormalVertexShader(void* shaderBlobV)
	{
		D3D11_INPUT_ELEMENT_DESC polygonLayout[3];
		unsigned int numElements;
		ID3DBlob* shaderBlob = (ID3DBlob*)shaderBlobV;
		ID3D11Device* device = Direct3D11Device::GetInstance()->GetRawDevice();

		CHECK_RETURN(device->CreateVertexShader((DWORD*)shaderBlob->GetBufferPointer(), shaderBlob->GetBufferSize(), NULL, &mVertexShader), "Failed to create vertex shader");


		// Create the vertex input layout description.
		// This setup needs to match the VertexType stucture in the ModelClass and in the shader.
		polygonLayout[0].SemanticName = "POSITION";
		polygonLayout[0].SemanticIndex = 0;
		polygonLayout[0].Format = DXGI_FORMAT_R32G32B32_FLOAT;
		polygonLayout[0].InputSlot = 0;
		polygonLayout[0].AlignedByteOffset = 0;
		polygonLayout[0].InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
		polygonLayout[0].InstanceDataStepRate = 0;

		polygonLayout[1].SemanticName = "TEXCOORD";
		polygonLayout[1].SemanticIndex = 0;
		polygonLayout[1].Format = DXGI_FORMAT_R32G32_FLOAT;
		polygonLayout[1].InputSlot = 0;
		polygonLayout[1].AlignedByteOffset = D3D11_APPEND_ALIGNED_ELEMENT;
		polygonLayout[1].InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
		polygonLayout[1].InstanceDataStepRate = 0;

		
		polygonLayout[2].SemanticName = "TEXCOORD";
		polygonLayout[2].SemanticIndex = 1;
		polygonLayout[2].Format = DXGI_FORMAT_R32G32_FLOAT;
		polygonLayout[2].InputSlot = 0;
		polygonLayout[2].AlignedByteOffset = D3D11_APPEND_ALIGNED_ELEMENT;
		polygonLayout[2].InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
		polygonLayout[2].InstanceDataStepRate = 0;
		

		// Get a count of the elements in the layout.
		numElements = sizeof(polygonLayout) / sizeof(polygonLayout[0]);

		// Create the vertex input layout.
	    HRESULT result = device->CreateInputLayout(polygonLayout, numElements, shaderBlob->GetBufferPointer(),
			shaderBlob->GetBufferSize(), &mLayout);
		if (FAILED(result))
		{
			Error("Failed to create vertex layout");
			return ;
		}
	}

	//***************************************************************************
	void VertexShaderEffect::CreateVertexShaderWithDiffuse(void* shaderBlobV)
	{
		D3D11_INPUT_ELEMENT_DESC polygonLayout[4];
		unsigned int numElements;
		ID3DBlob* shaderBlob = (ID3DBlob*)shaderBlobV;
		ID3D11Device* device = Direct3D11Device::GetInstance()->GetRawDevice();

		CHECK_RETURN(device->CreateVertexShader((DWORD*)shaderBlob->GetBufferPointer(), shaderBlob->GetBufferSize(), NULL, &mVertexShader), "Failed to create vertex diffuse shader");


		// Create the vertex input layout description.
		// This setup needs to match the VertexType stucture in the ModelClass and in the shader.
		polygonLayout[0].SemanticName = "POSITION";
		polygonLayout[0].SemanticIndex = 0;
		polygonLayout[0].Format = DXGI_FORMAT_R32G32B32_FLOAT;
		polygonLayout[0].InputSlot = 0;
		polygonLayout[0].AlignedByteOffset = 0;
		polygonLayout[0].InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
		polygonLayout[0].InstanceDataStepRate = 0;

		polygonLayout[1].SemanticName = "COLOR";
		polygonLayout[1].SemanticIndex = 0;
		polygonLayout[1].Format = DXGI_FORMAT_R32G32B32A32_FLOAT;
		polygonLayout[1].InputSlot = 0;
		polygonLayout[1].AlignedByteOffset = D3D11_APPEND_ALIGNED_ELEMENT;
		polygonLayout[1].InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
		polygonLayout[1].InstanceDataStepRate = 0;

		polygonLayout[2].SemanticName = "TEXCOORD";
		polygonLayout[2].SemanticIndex = 0;
		polygonLayout[2].Format = DXGI_FORMAT_R32G32_FLOAT;
		polygonLayout[2].InputSlot = 0;
		polygonLayout[2].AlignedByteOffset = D3D11_APPEND_ALIGNED_ELEMENT;
		polygonLayout[2].InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
		polygonLayout[2].InstanceDataStepRate = 0;

		polygonLayout[3].SemanticName = "TEXCOORD";
		polygonLayout[3].SemanticIndex = 1;
		polygonLayout[3].Format = DXGI_FORMAT_R32G32_FLOAT;
		polygonLayout[3].InputSlot = 0;
		polygonLayout[3].AlignedByteOffset = D3D11_APPEND_ALIGNED_ELEMENT;
		polygonLayout[3].InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
		polygonLayout[3].InstanceDataStepRate = 0;

		// Get a count of the elements in the layout.
		numElements = sizeof(polygonLayout) / sizeof(polygonLayout[0]);

		// Create the vertex input layout.
		HRESULT result = device->CreateInputLayout(polygonLayout, numElements, shaderBlob->GetBufferPointer(),
			shaderBlob->GetBufferSize(), &mLayout);
		if (FAILED(result))
		{
			Error("Failed to create vertex layout");
			return;
		}

	}

	//***************************************************************************
	VertexShaderEffect::~VertexShaderEffect()
	{
		if (mVertexShader != NULL)
		{
			mVertexShader->Release();
			mVertexShader = NULL;
		}

		if (mLayout != NULL)
		{
			mLayout->Release();
			mLayout = NULL;
		}
	}

	//***************************************************************************
	void VertexShaderEffect::Setup(Direct3D11Device* device)
	{
		if (mVertexShader != NULL)
		{
			ID3D11VertexShader* current_shader = NULL;
			ID3D11ClassInstance* instances;
			UINT count;

			device->GetDeviceContext()->VSGetShader(&current_shader, &instances, &count);

			if (current_shader != mVertexShader)
			{
				device->GetDeviceContext()->IASetInputLayout(mLayout);		
				device->GetDeviceContext()->VSSetShader(mVertexShader, NULL, 0);
			}
			if (current_shader != NULL)
			{
				current_shader->Release();
			}

			if (instances != NULL)
			{
			//	instances->Release();
			}
		}
	}

	//***************************************************************************
	ID3D11VertexShader* VertexShaderEffect::GetRawVertexShader()
	{
		return mVertexShader;
	}

	//***************************************************************************
	ID3D11InputLayout* VertexShaderEffect::GetRawLayout()
	{
		return mLayout;
	}


};
