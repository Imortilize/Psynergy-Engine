#include "ppshared.vsi"
#include <ToneMapping.fxh>

#ifdef SKINNED_MESH
/* Skinning Variables */
#define SKINNED_EFFECT_MAX_BONES   58
float4x4 xBones[SKINNED_EFFECT_MAX_BONES];
#endif

// Transform
float4x4 xWorld;
float4x4 xView;
float4x4 xProjection;
float4x4 xLightViewProjection; //used when rendering to shadow map

// Light values
bool xEnableLighting = true;
float3 xAmbientColor = float3( 0.15, 0.15, 0.15 );
float xSpecularIntensity = 0.8f;
float xSpecularPower = 0.5f;

// Half pixel size
float2 xHalfPixel = float2(0, 0);
float xMaxDepth = 3000.0f;

// Fogging values
bool xEnableFog = false;
float xFogStart = 500;
float xFogEnd = 1000;
float3 xFogColor = float3( 1, 1, 1 );

bool xUseNormalMap = false;

texture2D Texture;
sampler2D diffuseSampler : register(s0) = sampler_state 
{
    Texture = (Texture);
    MAGFILTER = ANISOTROPIC;
    MINFILTER = ANISOTROPIC;
    MIPFILTER = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture2D SpecularMap;
sampler2D specularSampler : register(s1) = sampler_state 
{
    Texture = (SpecularMap);
    MagFilter = ANISOTROPIC;
    MinFilter = ANISOTROPIC;
    Mipfilter = LINEAR;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

texture2D DepthMap;
sampler2D depthSampler : register(s2) = sampler_state 
{
    Texture = (DepthMap);
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

texture2D NormalMap;
sampler2D normalSampler : register(s3) = sampler_state 
{
    Texture = (NormalMap);
    MagFilter = ANISOTROPIC;
    MinFilter = ANISOTROPIC;
    Mipfilter = LINEAR;
    AddressU = WRAP;
    AddressV = WRAP;
};

texture2D LightMap;
sampler2D lightSampler : register(s4) = sampler_state 
{
    Texture = (LightMap);
    MagFilter = ANISOTROPIC;
    MinFilter = ANISOTROPIC;
    Mipfilter = LINEAR;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

/* Reflection variables */
texture2D xReflectionMap;
sampler2D reflectionSampler : register(s5) = sampler_state 
{
    Texture = (xReflectionMap);
    MagFilter = ANISOTROPIC;
    MinFilter = ANISOTROPIC;
    Mipfilter = LINEAR;
    AddressU = MIRROR;
    AddressV = MIRROR;
};

texture2D xRefractionMap;
sampler2D refractionSampler : register(s6) = sampler_state 
{
    Texture = (xRefractionMap);
    MagFilter = ANISOTROPIC;
    MinFilter = ANISOTROPIC;
    Mipfilter = LINEAR;
    AddressU = MIRROR;
    AddressV = MIRROR;
};

texture2D xOffsetMap;
sampler2D offsetSampler : register(s7) = sampler_state 
{
    Texture = (xOffsetMap);
    MagFilter = ANISOTROPIC;
    MinFilter = ANISOTROPIC;
    Mipfilter = LINEAR;
    AddressU = MIRROR;
    AddressV = MIRROR;
};


float4x4 xWaterViewProjection;	// Used when reflecting off the water
float xScrollTime = 0.0f;
float xWaveSize = 8.0f;
float xWaveLength = 100.0f;
float3 xCameraPosition = float3(0, 0, 0);
float3 xLightDirection = float3(0.5f, -0.5f, 0);
float3 xSurfaceColor = float3(0.36f, 0.664f, 0.608f);
float3 xDeepColor = float3(0.09f, 0.166f, 0.177f);
float xDeepDepth = 75.0f;
/**/

struct VertexShaderInput
{
    float4 Position		: POSITION0;
    float3 Normal		: NORMAL0;
    float2 TexCoord		: TEXCOORD0;
    float3 Binormal		: BINORMAL0;
    float3 Tangent		: TANGENT0;
};

struct VertexShaderOutput
{
    float4 Position			: POSITION0;
    float2 TexCoord			: TEXCOORD0;
    float3 Depth			: TEXCOORD1;
	float3 ViewPos			: TEXCOORD2;
    float3x3 tangentToWorld : TEXCOORD3;
};

//Normal Encoding Function
half3 encode(half3 n)
{
	n = normalize(n);

	n.xyz = 0.5f * (n.xyz + 1.0f);

	return n;
}

//Normal Decoding Function
half3 decode(half4 enc)
{
	return (2.0f * enc.xyz- 1.0f);
}

// Texture coordinate coverter helpers
float2 ClipSpaceToTexCoord(float4 input)
{
	return float2(0.5f, -0.5f) * (input.xy / input.w) + 0.5f;
}

float2 ClipSpaceToTexCoordPerturb(float4 input, float2 perturb)
{
	return float2(0.5f, -0.5f) * ((input.xy + perturb) / input.w) + 0.5f;
}
/**/

//
float FresnelApproximation(float3 lightDir, float3 normal, float offset)
{
	float3 reflectedViewDir = -reflect(lightDir, normal);

	float viewDotNorm = abs(dot(lightDir, normal));
	float fresnelFactor = 1 - pow(viewDotNorm, 0.5f);

	return saturate(fresnelFactor + offset);
}
/**/

//
float PhongSpecular(float3 normal, float3 viewDir, float specularDecay)
{
	float nDotL = dot(normal, xLightDirection);

	float3 reflection = (2.0f * normal * nDotL + xLightDirection);
	reflection = normalize(reflection);

	float rDotV = saturate(dot(reflection, viewDir));

	return pow(rDotV, specularDecay);
}
/**/

/* ReconstructShading */
struct ReconstructVertexShaderInput
{
    float4 Position		: POSITION0;
    float2 TexCoord		: TEXCOORD0;
	float3 Normal		: NORMAL;
};

struct ReconstructVertexShaderOutput
{
    float4 Position				: POSITION0;
    float2 TexCoord				: TEXCOORD0;
	float4 TexCoordScreenSpace	: TEXCOORD1;
	float3 ViewPos				: TEXCOORD2;
	float4 ReflClipPos			: TEXCOORD3;
	float3 WorldPos				: TEXCOORD4;
	float2 SecondWaterTexCoord	: TEXCOORD5;
};

ReconstructVertexShaderOutput VertexShaderReconstructShading(ReconstructVertexShaderInput input)
{
    ReconstructVertexShaderOutput output = (ReconstructVertexShaderOutput)0;

	/* deform shader */
	/*float angle = (xScrollTime % 360) * 2;
	float freqX = 0.4f + sin(xScrollTime) * 1.0f;
	float freqY = 1.0f + sin(xScrollTime * 1.3f) * 2.0f;
	float freqZ = 1.1f + sin(xScrollTime * 1.1f) * 3.0f;
	float amp = 1.0f + sin(xScrollTime * 1.4f) * 10.0f;
	float f = sin(input.Normal.x * freqX + xScrollTime) * sin(input.Normal.y * freqY + xScrollTime) * sin(input.Normal.z * freqZ + xScrollTime);*/

	float4x4 worldViewProjection = mul(mul(xWorld, xView), xProjection);

	// Calculate the view position
	float4 worldPosition = mul(input.Position, xWorld);
	float4 viewPosition = mul(worldPosition, xView);

	// Save projected position
	output.Position = mul(viewPosition, xProjection);

	// Set view position
	output.ViewPos = viewPosition.xyz;

	// Calculate the scroll
	float2 scrollDirection = float2(0.0f, 1.0f);
	float time = (xScrollTime / 1000.0f);
	float2 scroll = (scrollDirection * time * 10.1f);
	float2 scroll2 = (-scrollDirection * time * 5.1f);

	// Texture coordinates
    output.TexCoord = (((input.TexCoord + scroll) / xWaveSize) * 8); 
	output.SecondWaterTexCoord = (((input.TexCoord + scroll2) / xWaveSize) * 8); 

	// Copy of the position
	output.TexCoordScreenSpace = output.Position;

	// Set reflection clip position
	float4x4 waterWorldViewProjection = mul(xWorld, xWaterViewProjection);
	output.ReflClipPos = mul(input.Position, waterWorldViewProjection);

	// Set world position
	output.WorldPos = worldPosition.xyz;

	
	// Convert positions
	/*output.Position.z += input.Normal.z * freqZ * amp * f;
	output.Position.x += input.Normal.x * freqX * amp * f;
	output.Position.y += input.Normal.y * freqY * amp * f;*/
	/**/


    return output;
}

float4 PixelShaderReconstructShading(ReconstructVertexShaderOutput input) : COLOR0
{
	// Find the screen space texture coordinate and offset it
	float2 screenPos = ClipSpaceToTexCoord(input.TexCoordScreenSpace);

	// Get the terrain depth
	float sceneViewZ = (tex2D(depthSampler, screenPos).r * xMaxDepth);

	// Get the view length
	float viewLen = length(input.ViewPos);

	/* refraction and reflection */
	float4 bumpColor = (2.0f * tex2D(offsetSampler, input.TexCoord) - 1.0f);
	float2 perturbation = (bumpColor.rg / xWaveLength);

	// Get the normal
	float3 normal = (2.0f * tex2D(normalSampler, input.TexCoord).rgb - 1.0f);
	float3 normal2 = (2.0f * tex2D(normalSampler, input.SecondWaterTexCoord).rgb - 1.0f);

	normal = (normal + normal2);
	float3 normalWorld = -normalize((float3(0, 1, 0) * normal.z) + (normal.x * float3(0, 0, 1) + normal.y * float3(-1, 0, 0)));

	// Get the view direction
	float3 viewDir = normalize( input.WorldPos - xCameraPosition );

	// Get the reflection factor using fresnel term	
	float fresnelTerm = FresnelApproximation(viewDir, normalWorld, 0.0f);//dot(viewDir, normal.rbg);

	// Get the range of depth from waterplane to terrain
    float depthRange = (sceneViewZ - viewLen);

	// Shore values ( could be global )
	const float shoreFalloff = 2.0f;
    const float shoreScale = 10.0f;

	// calculate a transparency value using a power function
    float alpha = saturate(max(pow(depthRange / xMaxDepth, shoreFalloff) * xMaxDepth * shoreScale, 0.0f));
	
	// Refraction
	float3 refraction = tex2D(refractionSampler, (screenPos + perturbation)).rgb;

	// Calculate depth colour value
	float3 waterColor = (xDeepColor * refraction);

	if ( depthRange < xDeepDepth)
	{
		float difference = (xDeepDepth - depthRange);
		float factor = saturate(pow((difference / depthRange), shoreFalloff));

		waterColor = lerp(waterColor, xSurfaceColor * refraction, factor);
	}

	// Get reflection coords
	float2 reflectTexCoords = float2(0.5, -0.5) * (float2(input.ReflClipPos.x, input.ReflClipPos.y) / input.ReflClipPos.w) + 0.5f;
	reflectTexCoords = (reflectTexCoords + perturbation);
	
	float3 reflection = tex2D(reflectionSampler, reflectTexCoords) * 0.3f;
	/**/

	// Colour based on how much reflection vs refraction
	float3 color = lerp(waterColor, reflection, fresnelTerm);

	// Calculate specular values
	float specular = PhongSpecular(normalWorld, viewDir, 256);

	// Lerp back to refraction for shoreline
	color = lerp(refraction, color, alpha) + (specular * alpha);

	// Return final colour
    return float4(color, 1);
}

/* */
technique ReconstructShadingTemp
{
    pass Pass1
    {
        /*VertexShader = compile vs_3_0 VertexShaderReconstructShading();
        PixelShader = compile ps_3_0 PixelShaderReconstructShading();*/
    }
}

technique ReconstructShading
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderReconstructShading();
        PixelShader = compile ps_3_0 PixelShaderReconstructShading();
    }
}

