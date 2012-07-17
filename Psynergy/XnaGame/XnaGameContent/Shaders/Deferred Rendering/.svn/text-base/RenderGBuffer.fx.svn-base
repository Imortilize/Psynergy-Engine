#include "ppshared.vsi"

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
float xMaxDepth = 1000.0f;

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
    MIPFILTER = POINT;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture2D SpecularMap;
sampler2D specularSampler : register(s1) = sampler_state 
{
    Texture = (SpecularMap);
    MagFilter = ANISOTROPIC;
    MinFilter = ANISOTROPIC;
    Mipfilter = POINT;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

texture2D NormalMap;
sampler2D normalSampler : register(s2) = sampler_state 
{
    Texture = (NormalMap);
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

texture2D LightMap;
sampler2D lightSampler : register(s3) = sampler_state 
{
    Texture = (LightMap);
    MagFilter = ANISOTROPIC;
    MinFilter = ANISOTROPIC;
    Mipfilter = POINT;
    AddressU = CLAMP;
    AddressV = CLAMP;
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

struct VertexShaderInput
{
    float4 Position		: POSITION0;
    float3 Normal		: NORMAL0;
    float2 TexCoord		: TEXCOORD0;
    float3 Binormal		: BINORMAL0;
    float3 Tangent		: TANGENT0;

#ifdef SKINNED_MESH
    int4   BoneIndices  : BLENDINDICES0;
    float4 BoneWeights  : BLENDWEIGHT0;
#endif
};

struct VertexShaderOutput
{
    float4 Position			: POSITION0;
    float2 TexCoord			: TEXCOORD0;
    float3 Depth			: TEXCOORD1;
    float3x3 tangentToWorld : TEXCOORD2;
};

VertexShaderOutput VertexShaderRenderToGBuffer(VertexShaderInput input)
{
    VertexShaderOutput output;

	float4x4 worldView = mul(xWorld, xView);

#ifdef SKINNED_MESH
	// Blend between the weighted bone matrices. 
	float4x4 skinTransform = 0; 
     
	skinTransform += xBones[input.BoneIndices.x] * input.BoneWeights.x; 
	skinTransform += xBones[input.BoneIndices.y] * input.BoneWeights.y; 
	skinTransform += xBones[input.BoneIndices.z] * input.BoneWeights.z; 
	skinTransform += xBones[input.BoneIndices.w] * input.BoneWeights.w; 

	float4 skinPos = mul(input.Position, skinTransform);
	
    float4 viewPosition = mul(skinPos, xView);
    output.Position = mul(viewPosition, xProjection);

    output.TexCoord = input.TexCoord;
    output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;
	output.Depth.z = viewPosition.z;

    // calculate tangent space to world space matrix using the world space tangent,
    // binormal, and normal as basis vectors
    output.tangentToWorld[0] = mul(input.Tangent, skinTransform);
    output.tangentToWorld[1] = mul(input.Binormal, skinTransform);
    output.tangentToWorld[2] = mul(input.Normal, skinTransform);
#else
    float4 worldViewPosition = mul(input.Position, worldView);
    output.Position = mul(worldViewPosition, xProjection);

    output.TexCoord = input.TexCoord;
    output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;
	output.Depth.z = worldViewPosition.z;

    // calculate tangent space to world space matrix using the world space tangent,
    // binormal, and normal as basis vectors
    output.tangentToWorld[0] = mul(input.Tangent, xWorld);
    output.tangentToWorld[1] = mul(input.Binormal, xWorld);
    output.tangentToWorld[2] = mul(input.Normal, xWorld);
#endif

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

	// Max Depth Target using camera far plane depth
	output.MaxDepth.r = clamp((input.Depth.x / xMaxDepth), 0, 1);

	// Store View Space depth for SSAO
	output.MaxDepth.g = input.Depth.z;

    return output;
}

/* ReconstructShading */
struct ReconstructVertexShaderInput
{
    float4 Position		: POSITION0;
    float2 TexCoord		: TEXCOORD0;

#ifdef SKINNED_MESH
    float4 BoneIndices	: BLENDINDICES0;
    float4 BoneWeights	: BLENDWEIGHT0;
#endif
};

struct ReconstructVertexShaderOutput
{
    float4 Position				: POSITION0;
    float2 TexCoord				: TEXCOORD0;
	float4 TexCoordScreenSpace	: TEXCOORD1;
};

ReconstructVertexShaderOutput VertexShaderReconstructShading(ReconstructVertexShaderInput input)
{
    ReconstructVertexShaderOutput output = (ReconstructVertexShaderOutput)0;

	float4x4 worldViewProjection = mul(mul(xWorld, xView), xProjection);

#ifdef SKINNED_MESH
	// Blend between the weighted bone matrices. 
	float4x4 skinTransform = 0; 
     
	skinTransform += xBones[input.BoneIndices.x] * input.BoneWeights.x; 
	skinTransform += xBones[input.BoneIndices.y] * input.BoneWeights.y; 
	skinTransform += xBones[input.BoneIndices.z] * input.BoneWeights.z; 
	skinTransform += xBones[input.BoneIndices.w] * input.BoneWeights.w; 

	float4 skinPos = mul(input.Position, skinTransform);
	
    float4 viewPosition = mul(skinPos, xView);
    output.Position = mul(viewPosition, xProjection);
#else
	output.Position = mul(input.Position, worldViewProjection);
#endif

    output.TexCoord = input.TexCoord; //pass the texture coordinates further
	output.TexCoordScreenSpace = output.Position;

    return output;
}

float4 PixelShaderReconstructShading(ReconstructVertexShaderOutput input) : COLOR0
{
	// Find the screen space texture coordinate and offset it
	float2 screenPos = PostProjToScreen(input.TexCoordScreenSpace) + xHalfPixel;

	float3 diffuseColor = tex2D(diffuseSampler, input.TexCoord).rgb;
	float3 lighting = diffuseColor;

	if ( xEnableLighting )
	{
		float4 light = tex2D(lightSampler, screenPos);

		// Get the diffuse and specular light
		float3 diffuseLight = light.rgb;
		float specularLight = light.a;

		lighting = (xAmbientColor * diffuseColor) + (diffuseLight * diffuseColor) +  (diffuseLight * specularLight);
	}

	// Final light colour with shadow factory taken into account
	float3 final = lighting;

	// Fogging //
	if ( xEnableFog )
	{
		// ---------------- DISTANCE FOG --------------------	
		float fog = clamp((input.TexCoordScreenSpace.z - xFogStart) / (xFogEnd - xFogStart), 0, 1);
		final = lerp(final, xFogColor, fog);
		
		// Clamp color
		final = clamp( final, 0, 1 );
	}

	// Return final colour
    return float4(final, 1);
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
        VertexShader = compile vs_2_0 VertexShaderReconstructShading();
        PixelShader = compile ps_2_0 PixelShaderReconstructShading();
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