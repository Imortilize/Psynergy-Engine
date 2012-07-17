#include <Filmgrain.fxh>

float2 xHalfPixel = float2(0, 0);

//////////////////////////////////////////////
///////////////// Textures ///////////////////
//////////////////////////////////////////////

texture xSceneTexture;
sampler2D sceneSampler : register(s9) = sampler_state
{
	Texture = <xSceneTexture>;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 UV		: TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 UV		: TEXCOORD0;
};

//////////////////////////////////////////////
////////////// Vertex Shader /////////////////
//////////////////////////////////////////////

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	
	output.Position = input.Position;
	output.Position.xy += xHalfPixel; // 
	output.UV = input.UV; 
	
	return output;
} // vs_main

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 color = tex2D(sceneSampler, input.UV);	// HDR Linear space	
	
	// Return film grain
    return float4(FilmGrain(color, input.UV), 1);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
