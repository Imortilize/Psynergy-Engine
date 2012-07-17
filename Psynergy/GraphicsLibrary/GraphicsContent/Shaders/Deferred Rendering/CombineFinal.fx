texture xColorMap;
texture xLightMap;
float2 xHalfPixel = float2(0, 0);

float3 xAmbientColor = float3( 0.15, 0.15, 0.15 );

sampler colorSampler = sampler_state 
{
    Texture = (xColorMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = ANISOTROPIC;
    MinFilter = ANISOTROPIC;
    Mipfilter = LINEAR;
};

sampler lightSampler = sampler_state 
{
    Texture = (xLightMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = ANISOTROPIC;
    MinFilter = ANISOTROPIC;
    Mipfilter = LINEAR;
};

struct VertexShaderInput
{
    float3 Position		: POSITION0;
    float2 TexCoord		: TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position		: POSITION0;
    float2 TexCoord		: TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = float4(input.Position, 1);
    output.TexCoord = (input.TexCoord - xHalfPixel);

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 diffuseColor = tex2D(colorSampler, input.TexCoord).rgb;
    float4 light = tex2D(lightSampler, input.TexCoord);
    float3 diffuseLight = light.rgb;
    float specularLight = light.a;

	float3 lighting = (xAmbientColor * diffuseColor) + (diffuseLight * diffuseColor) +  (diffuseLight * specularLight);

	// Return final colour
    return float4(lighting, 1);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
