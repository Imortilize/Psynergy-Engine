// Input textures (just 1 in this case)
sampler2D xRenderTargets[1];

float xShoulderStrength = 6.2f;//0.22f;// Shoulder strength (A)
float xLinearStrength = 1.7f;//0.50f;// Linear Strength (B)
float xLinearAngle = 0.30f;// Linear Angle (C)
float xToeStrength = 0.20f;// Toe Strength (D)
float xToeNumerator = 0.02f;// Toe Numerator (E)
float xToeDenominator = 0.30f;// Toe Denominator (F)
float xLinearWhite = 11.2f;

float3 F( float3 lighting )
{
	return ((lighting * (xShoulderStrength * lighting + xLinearAngle * xLinearStrength) + xToeStrength * xToeNumerator) / (lighting * (xShoulderStrength * lighting + xLinearStrength) + xToeStrength * xToeDenominator)) - xToeNumerator/xToeDenominator;
}

float F( float lighting )
{
	return ((lighting * (xShoulderStrength * lighting + xLinearAngle * xLinearStrength) + xToeStrength * xToeNumerator) / (lighting * (xShoulderStrength * lighting + xLinearStrength) + xToeStrength * xToeDenominator)) - xToeNumerator/xToeDenominator;
}

float4 ToneMap(float3 color) 
{
	float3 linearColor = F(color);
	float linearWhite = F(xLinearWhite);

	// Return the final value
	return float4((linearColor / linearWhite), 1);
} 

float4 PixelShaderFunction( float2 UV : TEXCOORD0 ) : COLOR0 
{
	// Sample the original HDR image 
	float4 vSample = tex2D(xRenderTargets[0], UV); 
	float4 vToneMapped = ToneMap(vSample.rgb);

	// Return tone mapped image
	return vToneMapped; 
}

technique ToneMapping
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}