// Input textures (just 1 in this case)
sampler2D xRenderTargets[1];

// Bloom threshold
float xThreshold = 0.3f;

float4 BloomPixelShader(float2 UV : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(xRenderTargets[0], UV); 

    // Get the bright areas that is brighter than Threshold and return it.
    return saturate((color - xThreshold) / (1 - xThreshold));
}

technique Bloom
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 BloomPixelShader();
    }
}
