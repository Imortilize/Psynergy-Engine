// Input textures (just 1 in this case)
sampler2D xRenderTargets[1];

float4 PixelShaderFunction(float4 Position : POSITION0, float2 UV : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(xRenderTargets[0], UV);

	// RGB are weighted differently as that is how our eyes see it ( green most, then red then blue )
	float intensity = (0.3f * color.r) + (0.59f * color.g) + (0.11f * color.b);

	// Return gray scale image
	return float4(intensity, intensity, intensity, color.a);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
