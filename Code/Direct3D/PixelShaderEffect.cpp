
#include "PixelShaderEffect.h"
#include "Direct3DDevice.h"
#include "Errors.h"
#include "DX90SDK/Include/d3dx9.h" 


namespace Direct3D 
{

//***************************************************************************
PixelShaderEffect::PixelShaderEffect(char* sourceFile) :
mPixelShader(NULL),
mTexture1(NULL),
mC0Register(NULL),
mC1Register(NULL)
{
	LPD3DXBUFFER                 code = NULL; //Temporary buffer (NEW)

		//set up Pixel Shader (NEW)
	HRESULT result = D3DXCompileShaderFromFileA(sourceFile,   //filepath
									   NULL,          //macro's            
									   NULL,          //includes           
									   "ps_main",     //main function      
									   "ps_2_0",      //shader profile     
									   0,             //flags              
									   &code,         //compiled operations
									   NULL,          //errors
									   NULL);         //constants

	CHECK_RETURN(result, "Invalid pixel shader code");

	if (FAILED(result))
	{
		return ; 
	}

	CHECK_RETURN(Direct3DDevice::GetInstance()->GetRawDevice()->CreatePixelShader((DWORD*)code->GetBufferPointer(), &mPixelShader), "Failed to create pixel shader");

	code->Release();
}

//***************************************************************************
PixelShaderEffect::~PixelShaderEffect()
{
	if (mC0Register != NULL)
	{
		delete [] mC0Register;
		mC0Register=NULL;
	}

	if (mC1Register != NULL)
	{
		delete [] mC1Register;
		mC1Register=NULL;
	}

	if (mPixelShader != NULL)
	{
		mPixelShader->Release();
		mPixelShader=NULL;
	}
}

//***************************************************************************
void PixelShaderEffect::Setup(Direct3DDevice* device)
{
	if (mPixelShader != NULL)
	{
		IDirect3DPixelShader9* current_shader = NULL;
		CHECK_RETURN(device->GetRawDevice()->GetPixelShader(&current_shader),"Failed to get pixel shader") ;

		if (current_shader != mPixelShader)
		{
			CHECK_RETURN(device->GetRawDevice()->SetPixelShader(mPixelShader), "Failed to set pixel shader");
		}
		if (current_shader!=NULL)
		{
			current_shader->Release();
		}

		if (mTexture1)
		{
			IDirect3DTexture9* t = mTexture1->GetD3DTexture();
			if (t!=NULL)
			{
				CHECK_RETURN(device->GetRawDevice()->SetTexture(1, mTexture1->GetD3DTexture() ), "Failed to set texture");
			}
		}

		if (mC0Register!=NULL)
		{
			CHECK_RETURN(device->GetRawDevice()->SetPixelShaderConstantF(0, mC0Register, 1), "Failed to set pixel shader constant");
		}
		if (mC1Register!=NULL)
		{
			CHECK_RETURN(device->GetRawDevice()->SetPixelShaderConstantF(1, mC1Register, 1), "Failed to set pixel shader constant");
		}
	}
}

//***************************************************************************
void PixelShaderEffect::CreateRegister(float** regPointer)
{ 
	*regPointer = new float[4];
	for (int i=0;i<4;i++)
	{
		(*regPointer)[i] =0;
	}
}

//***************************************************************************
void PixelShaderEffect::SetParameter(int index, float value)
{
	if (index < 4)
	{
		if (mC0Register == NULL)
		{
			CreateRegister(&mC0Register);
		}
		mC0Register[index] = value;
	}
	else if (index <8)
	{
		if (mC1Register == NULL)
		{
			CreateRegister(&mC1Register);
		}
		mC1Register[index - 4] = value;
	}
}
 

//***************************************************************************
void PixelShaderEffect::SetTexture1(Texture* texture)
{
	mTexture1 = texture;
}

//***************************************************************************
void PixelShaderEffect::SetTexture1(Surface* surface)
{
	if (surface==NULL) return;
	mTexture1 = surface->GetTexture();
}

//***************************************************************************
Texture* PixelShaderEffect::GetTexture1()
{
	return mTexture1;
}

//***************************************************************************
IDirect3DPixelShader9* PixelShaderEffect::GetRawPixelShader()
{
	return mPixelShader;
}

};
