#define SKINNED_EFFECT_MAX_BONES   58
float4x4 xBones[SKINNED_EFFECT_MAX_BONES];

float4x4 xWorld;
float4x4 xView;
float4x4 xProjection;

float xFarPlane = 5000;

struct VertexShaderInput
{
    float4 Position		: POSITION0;
    int4   BoneIndices  : BLENDINDICES0;
    float4 BoneWeights  : BLENDWEIGHT0;
};

struct VertexShaderOutput
{
    float4 Position			: POSITION0;
	float4 ScreenPosition	: TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput Output = (VertexShaderOutput)0;

    float4x4 worldViewProjection = mul( xWorld, mul(xView, xProjection));
	float4 position = mul(input.Position, worldViewProjection);
	
    Output.Position = position;
	Output.ScreenPosition = position;

    return Output;
}

VertexShaderOutput VertexShaderFunctionSkinned(VertexShaderInput input)
{
    VertexShaderOutput Output = (VertexShaderOutput)0;

	// Blend between the weighted bone matrices. 
    float4x4 skinTransform = 0; 
     
    skinTransform += xBones[input.BoneIndices.x] * input.BoneWeights.x; 
    skinTransform += xBones[input.BoneIndices.y] * input.BoneWeights.y; 
    skinTransform += xBones[input.BoneIndices.z] * input.BoneWeights.z; 
    skinTransform += xBones[input.BoneIndices.w] * input.BoneWeights.w; 
	
	float4 worldPosition = mul( input.Position, skinTransform );
	float4 position = mul( worldPosition, mul(xView, xProjection) );

	//Output.Normal = mul(input.Normal, worldPosition);
    //float4x4 worldViewProjection = mul( xWorld, mul(xView, xProjection));
	//float4 position = mul(input.Position, worldViewProjection);
	
    Output.Position = position;
	Output.ScreenPosition = position;

    return Output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // Determine the depth of this vertex / by the far plane distance,
	// limited to [0, 1]
	float depth = clamp( input.ScreenPosition.z / input.ScreenPosition.w, 0, 1 );
	
	// Return only the depth value
	//return float4(depth, 0, 0, 1);
    return float4(depth, (depth * depth), 0, 0);
}

technique ShadowDepthMap
{
    pass Pass1
    {
        VertexShader = compile vs_1_1 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

technique ShadowDepthMapSkinned
{
    pass Pass1
    {
        VertexShader = compile vs_1_1 VertexShaderFunctionSkinned();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
