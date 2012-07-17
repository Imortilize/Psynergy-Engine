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
float xWaveSize = 4.0f;
float xWaveLength = 100.0f;
float3 xCameraPosition = float3(0, 0, 0);
float3 xLightDirection = float3(0.5f, -0.5f, 0);
float3 xSurfaceColor = float3(0.36f, 0.664f, 0.608f);
float3 xDeepColor = float3(0.09f, 0.166f, 0.177f);
float xDeepDepth = 50.0f;
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

VertexShaderOutput VertexShaderRenderToGBuffer(VertexShaderInput input)
{
    VertexShaderOutput output;

	float4x4 worldView = mul(xWorld, xView);
    float4 viewPosition = mul(input.Position, worldView);
    output.Position = mul(viewPosition, xProjection);

    output.TexCoord = input.TexCoord;
    output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;
	output.Depth.z = viewPosition.z;

	output.ViewPos = viewPosition.xyz;

    // calculate tangent space to world space matrix using the world space tangent,
    // binormal, and normal as basis vectors
    output.tangentToWorld[0] = mul(input.Tangent, xWorld);
    output.tangentToWorld[1] = mul(input.Binormal, xWorld);
    output.tangentToWorld[2] = mul(input.Normal, xWorld);

    return output;
}

struct PixelShaderOutput
{
    float4 Color : COLOR0;
    float4 Normal : COLOR1;
    float4 Depth : COLOR2;
	float4 MaxDepth : COLOR3;
};

PixelShaderOutput PixelShaderRenderToGBuffer(VertexShaderOutput input)
{
    PixelShaderOutput output = (PixelShaderOutput)0;

    output.Color = tex2D(diffuseSampler, input.TexCoord);

    float4 specularAttributes = tex2D(specularSampler, input.TexCoord);

    // specular Intensity
    output.Color.a = specularAttributes.r;
    
	float3 normalData = input.tangentToWorld[2];

	if ( xUseNormalMap ) 
	{
		// read the normal from the normal map
		normalData = tex2D(normalSampler, input.TexCoord);

		//tranform to [-1,1]
		normalData = 2.0f * normalData - 1.0f;

		//transform into world space
		normalData = mul(normalData, input.tangentToWorld);
	}

    //normalize the result
    normalData = normalize(normalData);

    //output the normal, in [0,1] space
    output.Normal.rgb = encode(normalData);

    // specular Power
	output.Normal.a = specularAttributes.a;

	// Depth 
	output.Depth.r = clamp((input.Depth.x / input.Depth.y), 0, 1);
    //output.Depth.r = length(input.ViewPos) / xMaxDepth;

	// Max Depth Target using camera far plane depth
	//output.MaxDepth.r = length(input.ViewPos) / xMaxDepth; //clamp((input.Depth.x / xMaxDepth), 0, 1);

	// Store View Space depth for SSAO
	//output.MaxDepth.g = input.Depth.z;

    return output;
}

/* ReconstructShading */
struct ReconstructVertexShaderInput
{
    float4 Position		: POSITION0;
    float2 TexCoord		: TEXCOORD0;
};

struct ReconstructVertexShaderOutput
{
    float4 Position				: POSITION0;
    float2 TexCoord				: TEXCOORD0;
	float4 TexCoordScreenSpace	: TEXCOORD1;
	float3 ViewPos				: TEXCOORD2;
	float4 ReflClipPos			: TEXCOORD3;
	float3 WorldPos				: TEXCOORD4;
};

ReconstructVertexShaderOutput VertexShaderReconstructShading(ReconstructVertexShaderInput input)
{
    ReconstructVertexShaderOutput output = (ReconstructVertexShaderOutput)0;

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

	// Texture coordinates
    output.TexCoord = (((input.TexCoord + scroll) / xWaveSize) * 8); 

	// Copy of the position
	output.TexCoordScreenSpace = output.Position;

	// Set reflection clip position
	float4x4 waterWorldViewProjection = mul(xWorld, xWaterViewProjection);
	output.ReflClipPos = mul(input.Position, waterWorldViewProjection);

	// Set world position
	output.WorldPos = worldPosition.xyz;

    return output;
}

float4 PixelShaderReconstructShading(ReconstructVertexShaderOutput input) : COLOR0
{
	// Find the screen space texture coordinate and offset it
	float2 screenPos = ClipSpaceToTexCoord(input.TexCoordScreenSpace);

	//float3 diffuseColor = tex2D(diffuseSampler, input.TexCoord).rgb; // OLD
	/*float3 diffuseColor = pow( tex2D(diffuseSampler, input.TexCoord).rgb, 2.2);
	float3 lighting = diffuseColor;
	
	if ( xEnableLighting )
	{
		float4 light = tex2D(lightSampler, screenPos);

		// Get the diffuse and specular light
		float3 diffuseLight = light.rgb;
		float specularLight = light.a;

		lighting = (xAmbientColor * diffuseColor) + (diffuseLight * diffuseColor) +  (specularLight);
	}*/

	// Final light colour with shadow factory taken into account
	//float4 final = float4(lighting, 1); // OLD
	//float4 final =  pow( float4(lighting, 1), (1/2.2));

	// Approximation of filmic tone mapping ( takes into account the 'pow( x, 1/2.2 )' )
	//float3 x = max(0, (lighting - 0.004f));
	//float4 final = float4((x * (6.2f * x + 0.5f)) / (x * (6.2f * x + 1.7f) + 0.06f), 1);

	// Approximation of filmic tone mapping ( using uncharted 2 dudes numbers and tweakable values, doesn't include 'pow( x, 1/2.2 )' )
	//float A = 1.0f;//0.22f;// Shoulder strength
	//float B = 1.7f;//0.50f;// Linear Strength
	//float C = 0.30f;// Linear Angle
	//float D = 0.20f;// Toe Strength
	//float E = 0.01f;// Toe Numerator
	//float F = 0.30f;// Toe Denominator

	//float4 linearColor = float4(pow(lighting, (1/2.2)), 1);
	//float linearWhite = 1.2f;

	//float3 xLinearColor = ((lighting * (A * lighting + C * B) + D * E) / (lighting * (A * lighting + B) + D * F)) - E/F;
	//float xLinearWhite = ((linearWhite * (A * linearWhite + C * B) + D * E) / (linearWhite * (A * linearWhite + B) + D * F)) - E/F;

	//float4 final = float4((xLinearColor / xLinearWhite), 1);

	// Fogging //
	//if ( xEnableFog )
	//{
		// ---------------- DISTANCE FOG --------------------	
		//float fog = clamp((input.TexCoordScreenSpace.z - xFogStart) / (xFogEnd - xFogStart), 0, 1);
		//final = float4(lerp(final.rgb, xFogColor, fog), final.a);
		
		// Clamp color
		//final = clamp( final, 0, 1 );
	//}

	// Shoreline Detection
	//float2 screenTexCoords = 0.5 * (float2(input.TexCoordScreenSpace.x, input.TexCoordScreenSpace.y) / input.TexCoordScreenSpace.w) + 0.5;
    //screenTexCoords.y = 1 - screenTexCoords.y;

	// 
	// Get the terrain depth
	float sceneViewZ = ((tex2D(depthSampler, screenPos).r - 0.001f) * xMaxDepth);

	// Get the view length
	float viewLen = length(input.ViewPos);

	/* refraction and reflection */
	float4 bumpColor = (2.0f * tex2D(offsetSampler, input.TexCoord) - 1.0f);
	float2 perturbation = (bumpColor.rg / xWaveLength);

	// Get the normal
	float3 normal = (2.0f * tex2D(normalSampler, input.TexCoord).rgb - 1.0f);
	float3 normalWorld = -normalize((float3(0, 1, 0) * normal.z) + (normal.x * float3(0, 0, 1) + normal.y * float3(-1, 0, 0)));

	// Get the view direction
	float3 viewDir = normalize( input.WorldPos - xCameraPosition );

	// Get the reflection factor using fresnel term	
	float fresnelTerm = FresnelApproximation(viewDir, normalWorld, 0.3f);//dot(viewDir, normal.rbg);

	// Get the range of depth from waterplane to terrain
    float depthRange = (sceneViewZ - viewLen);

	// Shore values ( could be global )
	const float shoreFalloff = 2.0f;
    const float shoreScale = 5.0f;

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
	reflectTexCoords = (reflectTexCoords + perturbation);//ClipSpaceToTexCoordPerturb(input.ReflClipPos, dudv);
	
	float3 reflection = tex2D(reflectionSampler, reflectTexCoords) * 0.5f;
	/**/

	// My own caps so it looks ok most the time during development
	/*if ( alpha > 0.5f )
		alpha = 0.5f;
	else if ( alpha < 0.1f )
		alpha = 0.1f;*/

	// Colour based on how much reflection vs refraction
	//float3 color = lerp(reflection, refraction, saturate(fresnelTerm));
	float3 color = lerp(waterColor, reflection, fresnelTerm);

	// Calculate specular values
	float specular = PhongSpecular(normalWorld, viewDir, 256);

	// Lerp back to refraction for shoreline
	color = lerp(refraction, color, alpha) + (specular);// * alpha);

	// Return final colour
    return float4(color, 1);//lerp(otherCol, col, saturate(alpha)); //float4( colour, saturate(alpha) );
}

/* */

/* ReconstructShading */
struct ShadowMapVertexShaderInput
{
    float4 Position		: POSITION0;

#ifdef SKINNED_MESH
    float4 BoneIndices	: BLENDINDICES0;
    float4 BoneWeights	: BLENDWEIGHT0;
#endif
};

struct ShadowMapVertexShaderOutput
{
    float4 Position			: POSITION0;
	float4 ScreenPosition	: TEXCOORD0;
};

ShadowMapVertexShaderOutput VertexShaderDrawShadowMap(ShadowMapVertexShaderInput input)
{
    ShadowMapVertexShaderOutput output = (ShadowMapVertexShaderOutput)0;

	float4x4 worldViewProjection = mul(xWorld, xLightViewProjection);

#ifdef SKINNED_MESH
	// Blend between the weighted bone matrices. 
	float4x4 skinTransform = 0; 
     
	skinTransform += xBones[input.BoneIndices.x] * input.BoneWeights.x; 
	skinTransform += xBones[input.BoneIndices.y] * input.BoneWeights.y; 
	skinTransform += xBones[input.BoneIndices.z] * input.BoneWeights.z; 
	skinTransform += xBones[input.BoneIndices.w] * input.BoneWeights.w; 

	float4 skinPos = mul(input.Position, skinTransform);
	float4 clipPos = mul(skinPos, xLightViewProjection);
#else
	float4 clipPos = mul(input.Position, worldViewProjection);
#endif
	//clamp to the near plane
	clipPos.z = max(clipPos.z, 0);
	
	output.Position = clipPos;
	output.ScreenPosition = output.Position;

    return output;
}

float4 PixelShaderDrawShadowMap(ShadowMapVertexShaderOutput input) : COLOR0
{
    // Determine the depth of this vertex / by the far plane distance,
	// limited to [0, 1]
	float depth = clamp( input.ScreenPosition.z / input.ScreenPosition.w, 0, 1 );
	
	// Return only the depth value
	//return float4(depth, 0, 0, 1);
    return float4(depth, (depth * depth), 0, 0);
}

/* */
technique RenderToGBuffer
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderRenderToGBuffer();
        PixelShader = compile ps_2_0 PixelShaderRenderToGBuffer();
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

technique DrawShadowMap
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderDrawShadowMap();
        PixelShader = compile ps_2_0 PixelShaderDrawShadowMap();
    }
}