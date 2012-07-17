float4x4 xWorld;
float4x4 xView;
float4x4 xProjection;

texture xParticleTexture;
sampler2D texSampler = sampler_state 
{
	texture = <xParticleTexture>;
};
	
float2 xSize;
float3 xUp;			// Cameras up vectors
float3 xSide;		// Cameras side vector

bool xAlphaTest = true;
bool xAlphaTestGreater = true;
float xAlphaTestValue = 0.5f;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 UV		: TEXCOORD0; 
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 UV		: TEXCOORD0; 
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput Output;

	float3 position = input.Position;

	// Determine which corner of the rectangle this vertex represents
	float2 offset = float2((input.UV.x - 0.5f) * 2.0f, -(input.UV.y - 0.5f) * 2.0f);

	// Move the vertex along the cameras plane
	position += (offset.x * xSize.x * xSide + offset.y * xSize.y * xUp);

	// Transform the position by view and projection
	Output.Position = mul( float4(position, 1), mul(xView, xProjection) );
	Output.UV = input.UV;

    return Output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 color = tex2D( texSampler, input.UV );

	if ( xAlphaTest )
		clip((color.a - xAlphaTestValue) * ( xAlphaTestGreater ? 1 : -1));

    return color;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
