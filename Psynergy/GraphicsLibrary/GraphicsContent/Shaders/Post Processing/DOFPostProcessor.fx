// Input textures (just 1 in this case)
sampler2D xRenderTargets[3];

// Max depth
float xMaxDepth = 1000;

// Distance at which blur starts
float xBlurStart = 200;

// Distance at which scene is fully blurred
float xBlurEnd = 500;

float4 PixelShaderFunction(float4 Position : POSITION0, float2 UV : TEXCOORD0) : COLOR0
{
	// Determine depth
	float depth = tex2D(xRenderTargets[2], UV).r * xMaxDepth;

	// Get blurred and unblurred render of scene
	float4 unblurred = tex2D(xRenderTargets[1], UV);
	float4 blurred = tex2D(xRenderTargets[0], UV);

	// Determine blur amount (similar to fog calculation)
	float blurAmount = clamp((depth - xBlurStart) / (xBlurEnd - xBlurStart), 0, 1);

	// Blend between unblurred and blurred images
	float4 mix = lerp(unblurred, blurred, blurAmount);

	return mix;
}

technique DepthOfField
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
