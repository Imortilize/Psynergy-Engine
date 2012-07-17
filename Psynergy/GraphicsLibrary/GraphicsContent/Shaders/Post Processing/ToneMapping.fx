#include <ToneMapping.fxh>

// Input textures (just 1 in this case)
sampler2D xRenderTargets[1];

float g_fMiddleGrey = 0.01f; 
float g_fMaxLuminance = 16.0f; 

// Logarithmic
float xWhiteLevel = 16;
float xLuminanceSaturation = 0.6f;

// Level adjusters
float xInputWhite = 1.0f;
float xInputBlack = 0.0f;
float xInputGamma = 1.0f;
float xOutputBlack = 0.0f;
float xOutputWhite = 1.0f;

static const float3 LUM_CONVERT = float3(0.299f, 0.587f, 0.114f); 

float3 ExposureColor(float3 color, float2 uv)
{
	float exposure;
	//[branch]
	/*if (autoExposure)
	{
		// From MJP http://mynameismjp.wordpress.com/ License: Microsoft_Permissive_License
	 	// The mip maps are used to obtain a median luminance value.
		float avgLuminance = exp(tex2Dlod(lastLuminanceSampler, float4(uv, 0, 10)).x); // This could be used to convert this global operator to local.
		// Use geometric mean
		avgLuminance = max(avgLuminance, 0.001f);		
		float keyValue = 1.03f - (2.0f / (2 + log10(avgLuminance + 1)));
		float linearExposure = (keyValue / avgLuminance);
		exposure = max(linearExposure, 0.0001f);
	}
    else	*/	
		exposure = exp2(0.1f);

	// Multiply the incomming light by the lens exposure value. Think of this in terms of a camera:
	// Exposure time on a camera adjusts how long the camera collects light on the main sensor.
	// This is a simple multiplication factor of the incomming light.	
	return color * exposure;
} // ExposureColor

// Approximates luminance from an RGB value
float CalcLuminance(float3 color)
{
    return max(dot(color, LUM_CONVERT), 0.0001f);
}

float3 ToneMap(float3 color) 
{
	float pixelLuminance = CalcLuminance(color);
	float toneMappedLuminance = pixelLuminance * (1.0f + (pixelLuminance / (xWhiteLevel * xWhiteLevel))) / (1.0f + pixelLuminance);
	return LinearToGamma(toneMappedLuminance * pow(color / pixelLuminance, xLuminanceSaturation));
} 

float3 AdjustLevels(float3 color)
{
	float3 inputLevels = pow(saturate(color - xInputBlack) / (xInputWhite - xInputBlack), xInputGamma);

	//return adjusted colour levels
	return float3(inputLevels * (xOutputWhite - xOutputBlack) + xOutputBlack);
} // AdjustLevels

float4 PixelShaderFunction( float2 UV : TEXCOORD0 ) : COLOR0 
{
	// Sample the original HDR image 
	float4 vSample = tex2D(xRenderTargets[0], UV); 

	// get exposure of the linear HDR colour
	vSample.rgb = ExposureColor(vSample.rgb, UV);

	//float3 vHDRColor = LogLuvDecode(vSample); 

	// Do the tone-mapping 
	float3 vToneMapped = ToneMap(vSample.rgb); 

	// Adjust colour levels
	vToneMapped = AdjustLevels(vToneMapped);

	// Return tone mapped image
	return float4(vToneMapped, 1.0f); 
}

technique ToneMapping
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}