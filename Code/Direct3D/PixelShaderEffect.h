#pragma once

#include "Texture.h"
#include "Surface.h"
#include "ActiveReader.h"
#include "MonitoredObject.h"

struct IDirect3DPixelShader9;

namespace Direct3D 
{

class Direct3DDevice;

class PixelShaderEffect : MonitoredObject
{
public:
	PixelShaderEffect(char* sourceFile);
	~PixelShaderEffect();

	void Setup(Direct3DDevice* device);
	IDirect3DPixelShader9* GetRawPixelShader();
	void SetParameter(int index, float value);
	void SetTexture1(Texture* texture);
	void SetTexture1(Surface* surface);

	Texture* GetTexture1();

private:

	void CreateRegister(float** regPointer);

	ActiveReader<Texture> mTexture1;

	float* mC0Register;	// array 4
	float* mC1Register; // array 4

	IDirect3DPixelShader9* mPixelShader;
};

}