// Input textures (just 1 in this case)
sampler2D xRenderTargets[1];

float2 xOffsets[15];
float xWeights[15];

float4 PixelShaderFunction(float4 Position : POSITION0, float2 UV : TEXCOORD0) : COLOR0
{
	float4 output = float4(0, 0, 0, 1);

	for (int i = 0; i < 15; i++)
		output += tex2D(xRenderTargets[0], (UV + xOffsets[i])) * xWeights[i];

	return output;
}

technique Horizontal
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

technique Vertical
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}