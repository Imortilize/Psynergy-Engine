float4x4 xWorld;
float4x4 xView;
float4x4 xProjection;
float3 xCameraPosition;

// Fogging values
bool xEnableFog = true;
float xFogStart = 500;
float xFogEnd = 1000;
float3 xFogColor = float3( 1, 1, 1 );

float4 xClipPlane;
bool xClipPlaneEnabled = false;

texture Texture;
samplerCUBE CubeMapSampler = sampler_state 
{
	texture = <Texture>;
	minfilter = ANISOTROPIC;
	MagFilter = ANISOTROPIC;
	MipFilter = Linear;
	AddressU = Mirror;
	AddressV = Mirror;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position				: POSITION0;
	float4 PositionCopy			: TEXCOORD0;
	float3 WorldPosition 		: TEXCOORD1;
	float3 TextureCoordinate	: TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput Output;

	float4 worldPosition =  mul(input.Position, xWorld);
	float4 viewPosition = mul(worldPosition, xView);

	Output.Position = mul(viewPosition, xProjection);
	Output.PositionCopy = Output.Position;
	Output.WorldPosition = worldPosition;
	Output.TextureCoordinate = ( worldPosition - xCameraPosition ); 

    return Output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 color = float3(0, 0, 0);
	color = texCUBE(CubeMapSampler, normalize(input.TextureCoordinate));

	// Fogging //
	if ( xEnableFog )
	{
		// ---------------- DISTANCE FOG --------------------	
		float fog = clamp((input.PositionCopy.z - xFogStart) / (xFogEnd - xFogStart), 0, 1);
		color = lerp(color, xFogColor, fog);
		
		// Clamp color
		color = clamp( color, 0, 1 );
	}

    return float4(color, 1);
}

technique Textured
{
    pass Pass1
    {
		CullMode = None;
		// We don't want it to obscure objects with a Z < 1
        ZWriteEnable = false; 

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();

		ZWriteEnable = true; 
		CullMode = CCW;
    }
}
