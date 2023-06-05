#include "Direct3DDevice.h"
#include "ManagedTexture.h"
#include "BeginSceneParameters.h"
#include "ManagedSurface.h"
#include "ManagedPixelShaderEffect.h"

using namespace Direct3D;


namespace MangedToUnManagedWrapper
{
namespace ManagedDirect3D
{



public ref class ManagedDirect3DDevice
{
public:

	 ManagedDirect3DDevice(System::IntPtr hWnd, unsigned int width, unsigned int height);
	 ~ManagedDirect3DDevice();

	 void BeginScene(ManagedSurface^ surface, System::IntPtr hwnd, int width, int height);
	 void EndScene();
	 int PresentToWindow(System::IntPtr hWnd, bool scaleBackBufferToWindow);
	 int Reset();
	 int Reset(unsigned int width, unsigned int height);
	 int GetBackBufferWidth();
	 int GetBackBufferHeight();

	 int TestCooperateLevel();

	 void SetRenderTarget(ManagedSurface^ surface);
	 void ClearRenderTarget(int r, int g, int b, int a);
	 void SetRenderTargetToBackBuffer();

	 void SetViewport(unsigned int width, unsigned int height);

	 void DrawImage(ManagedTexture^ texture, System::Drawing::RectangleF srcRect, float x1, float y1, float x2, float y2, float x3, float y3, float x4,float y4, bool alpha);
     void DrawImage(ManagedSurface^ surface, System::Drawing::RectangleF srcRect,  float x1, float y1, float x2, float y2, float x3, float y3, float x4,float y4, bool alpha);
	 void DrawImageWithDiffuseAlpha(ManagedTexture^ texture, float alpha, System::Drawing::RectangleF srcRect, float x1, float y1, float x2, float y2, float x3, float y3, float x4,float y4, bool includeImageAlpha);
	 void DrawImageWithDiffuseAlpha(ManagedSurface^ surface, float alpha, System::Drawing::RectangleF srcRect, float x1, float y1, float x2, float y2, float x3, float y3, float x4,float y4, bool includeImageAlpha);

	 void CopyDefaultSurfaceToBitmap(System::IntPtr bitmapPtr, int width, int height, int d_pitch);
     void CopyDefaultSurfaceToDynamicSurface(ManagedSurface^ surface);
	 int GetLastCopyDefaultSurfaceToDynamicSurfaceStage();

	 void SetPixelShaderEffect(ManagedPixelShaderEffect^ effect);
	 void ResetDefaultWindowLocationAndSize(int x, int y, int width, int height);

	 unsigned int GetAvailableTextureMem();

	 void SetBilinearInterpolation(int value);

	 bool HalDeviceCreated();

	// int Render(System::IntPtr bitmapPtr, int pitch);

private:

	Direct3DDevice* mDevice;
};

ManagedDirect3DDevice::ManagedDirect3DDevice(System::IntPtr hWnd, unsigned int width, unsigned int height)
{
	HWND handle = (HWND)hWnd.ToPointer();
	mDevice = new Direct3DDevice(handle, width, height);
}

ManagedDirect3DDevice::~ManagedDirect3DDevice()
{
	if (mDevice != NULL)
	{
		delete mDevice;
		mDevice = NULL;
	}
}

void ManagedDirect3DDevice::DrawImage(ManagedTexture^ texture, System::Drawing::RectangleF srcRect, float x1, float y1, float x2, float y2, float x3, float y3, float x4,float y4, bool alpha)
{
	mDevice->DrawImage(texture->GetTexture(), srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, x1,y1,x2,y2,x3,y3,x4,y4, alpha);
}


void ManagedDirect3DDevice::DrawImage(ManagedSurface^ surface, System::Drawing::RectangleF srcRect, float x1, float y1, float x2, float y2, float x3, float y3, float x4,float y4, bool alpha)
{
	mDevice->DrawImage(surface->GetSurface(), srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, x1,y1,x2,y2,x3,y3,x4,y4, alpha);
}


void ManagedDirect3DDevice::DrawImageWithDiffuseAlpha(ManagedTexture^ texture, float alpha, System::Drawing::RectangleF srcRect, float x1, float y1, float x2, float y2, float x3, float y3, float x4,float y4, bool includeImageAlpha)
{
	mDevice->DrawImageWithDiffuseAlpha(texture->GetTexture(), alpha, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, x1,y1,x2,y2,x3,y3,x4,y4, includeImageAlpha);
}

void ManagedDirect3DDevice::DrawImageWithDiffuseAlpha(ManagedSurface^ surface, float alpha, System::Drawing::RectangleF srcRect, float x1, float y1, float x2, float y2, float x3, float y3, float x4,float y4, bool includeImageAlpha)
{
	mDevice->DrawImageWithDiffuseAlpha(surface->GetSurface(), alpha, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, x1,y1,x2,y2,x3,y3,x4,y4, includeImageAlpha);
}


int ManagedDirect3DDevice::PresentToWindow(System::IntPtr hWnd, bool scaleBackBufferToWindow)
{
	HWND handle = (HWND)hWnd.ToPointer();
	return (int)mDevice->PresentToWindow(handle, scaleBackBufferToWindow);
}

int ManagedDirect3DDevice::TestCooperateLevel()
{
	return (int)mDevice->TestCooperativeLevel();
}

void ManagedDirect3DDevice::BeginScene(ManagedSurface^ surface, System::IntPtr hwnd, int width, int height)
{
	BeginSceneParameters parameters;
	if (surface != nullptr)
	{
		parameters.mRenderTarget = surface->GetSurface();
	
	}
	parameters.mHWnd = (HWND)hwnd.ToPointer();
	parameters.mWidth = width;
	parameters.mHeight = height;

	mDevice->BeginScene( parameters );
}

void ManagedDirect3DDevice::EndScene()
{
	mDevice->EndScene();
}

void ManagedDirect3DDevice::SetRenderTarget(ManagedSurface^ surface)
{
	mDevice->SetRenderTarget(surface->GetSurface());
}


void ManagedDirect3DDevice::ClearRenderTarget(int r, int g, int b, int a)
{
	mDevice->ClearRenderTarget(r,g,b,a);
}



void ManagedDirect3DDevice::SetRenderTargetToBackBuffer()
{
	mDevice->SetRenderTargetToBackBuffer();
}


void ManagedDirect3DDevice::SetViewport(unsigned int width, unsigned int height)
{
	mDevice->SetViewport(width,height);
}

void ManagedDirect3DDevice::CopyDefaultSurfaceToBitmap(System::IntPtr bitmapPtr, int width, int height, int d_pitch)
{
	return mDevice->CopyDefaultSurfaceToBitmap( (unsigned int*) bitmapPtr.ToPointer(), width, height, d_pitch );
}

void ManagedDirect3DDevice::CopyDefaultSurfaceToDynamicSurface(ManagedSurface^ surface)
{
    mDevice->CopyDefaultSurfaceToDynamicSurface( surface->GetSurface() );
}

int ManagedDirect3DDevice::GetLastCopyDefaultSurfaceToDynamicSurfaceStage()
{
    return mDevice->GetLastCopyDefaultSurfaceToDynamicSurfaceStage();
}

void ManagedDirect3DDevice::SetPixelShaderEffect(ManagedPixelShaderEffect^ effect)
{
	if (effect==nullptr)
	{
		mDevice->SetPixelShaderEffect(0);
		return;
	}

	PixelShaderEffect* r_effect = effect->GetPixelShaderEffect();
	if (r_effect)
	{
		mDevice->SetPixelShaderEffect(r_effect);
	}
}

int ManagedDirect3DDevice::Reset()
{
	return (int) mDevice->Reset();
}

int ManagedDirect3DDevice::Reset(unsigned int width, unsigned int height)
{
	return (int) mDevice->Reset(width, height);
}

int ManagedDirect3DDevice::GetBackBufferWidth()
{
	return mDevice->GetBackBufferWidth();
}


int ManagedDirect3DDevice::GetBackBufferHeight()
{
	return mDevice->GetBackBufferHeight();
}

void ManagedDirect3DDevice::ResetDefaultWindowLocationAndSize(int x, int y, int width, int height)
{
	mDevice->ResetDefaultWindowLocationAndSize(x,y,width,height);
}


unsigned int ManagedDirect3DDevice::GetAvailableTextureMem()
{
	return mDevice->GetAvailableTextureMem();
}


void ManagedDirect3DDevice::SetBilinearInterpolation(int value)
{
	mDevice->SetBilinearInterpolation(value);
}

bool ManagedDirect3DDevice::HalDeviceCreated()
{
	return mDevice->HalDeviceCreated();
}

}

}
