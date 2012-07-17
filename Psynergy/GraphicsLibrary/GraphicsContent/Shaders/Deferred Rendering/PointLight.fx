#include <..\Helpers\Discard.fxh>

float4x4 xWorld;
float4x4 xView;
float4x4 xProjection;

// Color of the light
float3 xLightColor = float3(0, 0, 0);

// Position of the camera. for specular light
float3 xCameraPosition = float3(0, 0, 0);

// This is used to compute the world position
float4x4 xInvertViewProjection;

// Position of the light
float3 xLightPosition = float3(0, 0, 0);

// How far does the light reach?
float xLightRadius = 0.0f;

// Control the brightness of the light
float xLightIntensity = 0.0f;

// Diffuse Color and the Specular Intensity in the alpha channel
texture2D xColorMap;

// Normals and Specular Power in the alpha channel
texture2D xNormalMap;

// Depth
texture2D xDepthMap;

// Half Pixel
float2 xHalfPixel = float2(0, 0);

// Far Plane
float xFarPlane = 2000;

// Whether inside the light bounding or not
bool xInsideBoundingObject = false;

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
/* */

struct VertexShaderInput
{
    float3 Position			: POSITION0;
};

struct VertexShaderOutput
{
	float4 Position			: POSITION0;
    float4 ScreenPosition	: TEXCOORD0;
	float4 ViewPosition		: TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

	// Processing geometry coordinates
    float4 worldPosition = mul(float4(input.Position, 1), xWorld);
    float4 viewPosition = mul(worldPosition, xView);

	// Save positions
    output.Position = mul(viewPosition, xProjection);
    output.ScreenPosition = output.Position;
	
	// View position
	output.ViewPosition = viewPosition;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	// Obtain screen position
    input.ScreenPosition.xy /= input.ScreenPosition.w;

    // Obtain textureCoordinates corresponding to the current pixel
    // the screen coordinates are in [-1,1]*[1,-1]
    // the texture coordinates need to be in [0,1]*[0,1]
    float2 texCoord = 0.5f * (float2(input.ScreenPosition.x, -input.ScreenPosition.y) + 1);

    // Allign texels to pixels
    texCoord += xHalfPixel;

    // Read depth
    float depthVal = tex2D(depthSampler, texCoord).r;

	// Optimization. We can't implement stencil optimizations, but at least this will allow us to avoid the normal map fetch and some other calculations.
	/*if (xInsideBoundingObject)
	{
		if (depthVal > -input.ViewPosition.z / xFarPlane)
		{
			Discard();
		}
	}
	else
	{
		if (depthVal < -input.ViewPosition.z / xFarPlane)
		{
			Discard();
		}
	}*/

    // Get normal data from the normalMap
    float4 normalData = tex2D(normalSampler, texCoord);

    // Tranform normal back into [-1,1] range
    float3 normal = (2.0f * normalData.xyz - 1.0f);

    // Get specular intensity from the colorMap
    float specularIntensity = clamp(tex2D(colorSampler, texCoord).a, 0, 1);
	float specularPower = clamp((normalData.a * 255), 0, 255);

    // Compute screen-space position
    float4 position;
    position.xy = input.ScreenPosition.xy;
    position.z = depthVal;
    position.w = 1.0f;

    // Transform to world space
    position = mul(position, xInvertViewProjection);
    position /= position.w;

    // Surface-to-light vector
    float3 lightVector = (xLightPosition - position);

    // Compute attenuation based on distance - linear attenuation
    float attenuation = saturate(1.0f - length(lightVector) / xLightRadius); 

    // Normalize light vector
    lightVector = normalize(lightVector); 

    // Compute diffuse light
    float NdL = max(0, dot(normal, lightVector));
    float3 diffuseLight = (NdL * xLightColor.rgb);

    // Reflection vector
    float3 reflectionVector = normalize(reflect(-lightVector, normal));

    // Camera-to-surface vector
    float3 directionToCamera = normalize(xCameraPosition - position);

    // Compute specular light
    float specularLight = specularIntensity * pow( saturate(dot(reflectionVector, directionToCamera)), 9.6f);
    
	// Take into account attenuation and lightIntensity.
    return (attenuation * xLightIntensity * float4(diffuseLight.rgb, specularLight));
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
