float2 xHalfPixel;
sampler xScene : register(s0);
sampler xSSAO : register(s1);

struct VertexShaderInput
{
	float3 Position : POSITION0;
	float2 UV		: TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 UV		: TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

	//Pass Position
	output.Position = float4(input.Position, 1);

	//Pass Texcoord's
	output.UV = input.UV + xHalfPixel;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	// Sample Scene
	float4 scene = tex2D(xScene, input.UV);

	// Sample SSAO
	float4 ssao = tex2D(xSSAO, input.UV);

	// Return
	return (scene * ssao);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
