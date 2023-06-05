#pragma once


#include "ActiveReader.h"
#include "MonitoredObject.h"

struct ID3D11VertexShader;
struct ID3D11InputLayout;
namespace Direct3D
{

	class Direct3D11Device;

	class VertexShaderEffect : MonitoredObject
	{
	public:
		VertexShaderEffect(char* sourceFile, bool diffuse);
		~VertexShaderEffect();
		void Setup(Direct3D11Device* device);
		ID3D11VertexShader* GetRawVertexShader();
		ID3D11InputLayout* GetRawLayout();

	private:

		void CreateNormalVertexShader(void* shaderBlob);
		void CreateVertexShaderWithDiffuse(void* shaderBlob);

		ID3D11VertexShader* mVertexShader;	
		ID3D11InputLayout* mLayout;
	};

}