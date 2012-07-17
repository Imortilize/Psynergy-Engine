#include "ppshared.vsi"
#include "shadow.vsi"

// Direction of the light
float3 xLightDirection = float3(0, 0, 0);

// Color of the light
float3 xLightColor = float3(0, 0, 0);

// Position of the camera. for specular light
float3 xCameraPosition = float3(0, 0, 0);
float4x4 xCameraTransform;

// This is used to compute the world position
float4x4 xInvertViewProjection;

// Diffuse Color and the Specular Intensity in the alpha channel
texture2D xColorMap;

// Normals and Specular Power in the alpha channel
texture2D xNormalMap;

// Depth
texture2D xDepthMap;

// Shadow Map
texture2D xShadowMap;

// Half Pixel
float2 xHalfPixel = float2(0, 0);
float2 xShadowMapHalfPixel = float2(0, 0);

// Shadow values
bool xEnableShadows = false;
float4x4 xLightViewProjection;
float xShadowMult = 0.0f;
float xShadowBias = (1.0f / 1300.0f);
float2 xShadowMapSize = float2(2048, 2048);

/* SAMPLERS */
sampler2D colorSampler : register(s0) = sampler_state 
{
    Texture = (xColorMap);
    MagFilter = ANISOTROPIC;
    MinFilter = ANISOTROPIC;
    Mipfilter = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

sampler2D normalSampler : register(s1) = sampler_state 
{
    Texture = (xNormalMap);
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

sampler2D depthSampler : register(s2) = sampler_state 
{
    Texture = (xDepthMap);
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

sampler2D shadowSampler : register(s3) = sampler_state 
{
	texture = <xShadowMap>;
	MinFilter = POINT;
	MagFilter = POINT;
	MipFilter = POINT;
	AddressU = CLAMP;
    AddressV = CLAMP;
};
/* */

struct VertexShaderInput
{
    float3 Position				: POSITION0;
    float2 TexCoord				: TEXCOORD0;

};

struct VertexShaderOutput
{
    float4 Position		: POSITION0;
    float2 TexCoord		: TEXCOORD0;
	float4 LightPosition : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

	// Position
    output.Position = float4(input.Position, 1);
	output.LightPosition = output.Position;

    //align texture coordinates
    output.TexCoord = (input.TexCoord + xHalfPixel);

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	// read depth
	float depthVal = tex2D(depthSampler, input.TexCoord).r;

	// If depth value == 1, we can assume its a background value, so skip it
	clip(-depthVal + 0.9999f);

	// Compute screen-space position
	float4 position;
	position.x = input.TexCoord.x * 2.0f - 1.0f;
	position.y = -(input.TexCoord.y * 2.0f - 1.0f);
	position.z = depthVal;
	position.w = 1.0f;

	// Transform to world space
	position = mul(position, xInvertViewProjection);
	position /= position.w;

	// Get normal data from the normalMap
    float4 normalData = tex2D(normalSampler, input.TexCoord);

    // Tranform normal back into [-1,1] range
    float3 normal = 2.0f * normalData.xyz - 1.0f;

	float3 lightVector = -normalize(xLightDirection);
	float nl = saturate(dot(normal, lightVector));
	float spec = 0;
	float4 finalColor = 0;
	float shadow = 1;

	clip(nl - 0.00001f);
	{
		// SHADOWS //
		// Get the actual shadow map depth
		if ( xEnableShadows )
		{
			// Calculate Homogenous Position with respect to light
			float4 lightScreenPos = mul(position, xLightViewProjection);
			float2 shadowTexCoord = PostProjToScreen(lightScreenPos) + xShadowMapHalfPixel;

			//Load the Projected Depth from the Shadow Map, do manual linear filtering
			//float depth = SampleShadowMap(shadowSampler, shadowTexCoord).r;

			// Get the real shadow map depth
			float realDepth = ((lightScreenPos.z / lightScreenPos.w) - xShadowBias);

			// Check to see if this pixel is in front or behind the value in the shadow map
			/*if (depth < realDepth)
			{
				// Shadow the pixel by lowering the intensity
				shadow = xShadowMult;
			};*/

			shadow = SampleShadowMapPCFSoft( shadowSampler, realDepth, shadowTexCoord, 5, xShadowMapSize, xShadowMult, xShadowBias );
		}
	}

    // camera-to-surface vector
    float3 directionToCamera = normalize(position - xCameraPosition);

    // Get specular intensity from the colorMap
    float specularIntensity = clamp(tex2D(colorSampler, input.TexCoord).a, 0, 1);
	float specularPower = clamp((normalData.a * 255), 0, 20);

	// reflexion vector
	float3 h = normalize(reflect(lightVector, normal)); 
	spec = specularIntensity * pow(saturate(dot(directionToCamera, h)), specularPower);
	finalColor = float4(xLightColor.rgb * nl, spec);

	//output the two lights
    return shadow * finalColor;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}