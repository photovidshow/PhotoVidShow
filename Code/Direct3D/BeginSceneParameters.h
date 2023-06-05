#pragma once

#include <Windows.h>
namespace Direct3D 
{

class Surface;

class BeginSceneParameters
{
public:
	BeginSceneParameters();
	~BeginSceneParameters();

	Surface*	mRenderTarget;
	HWND		mHWnd;
	int		    mWidth;
	int		    mHeight;

};

}

