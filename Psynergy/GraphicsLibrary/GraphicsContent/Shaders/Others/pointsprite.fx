//------- Constants --------
float4x4 xWorld;
float4x4 xView;
float4x4 xProjection;
float3 xCamPos;
float3 xCamUp;
float xPointSpriteSizeWidth;
float xPointSpriteSizeHeight;

// Alpha values
bool xAlphaTest = true;
bool xAlphaTestGreater = true;
float xAlphaTestValue = 0.5f;

//------- Texture Samplers --------

Texture Texture;
sampler TextureSampler = sampler_state 
{ 
	texture = <Texture>; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = LINEAR; 
	AddressU = mirror; 
	AddressV = mirror;
};

// ----- Vertex buffer ----- //
struct VertexToPixel
{
    float4 Position   	: POSITION;    
    float2 TextureCoords: TEXCOORD1;
};

//------- Technique: PointSprites --------

VertexToPixel PointSpriteVS(float3 inPos: POSITION0, float2 inTexCoord: TEXCOORD0)
{
    VertexToPixel Output = (VertexToPixel)0;

    float3 center = mul(inPos, xWorld);
    float3 eyeVector = center - xCamPos;

    float3 sideVector = cross(eyeVector, xCamUp);
    sideVector = normalize(sideVector);
    float3 upVector = cross(sideVector, eyeVector);
    upVector = normalize(upVector);

    float3 finalPosition = center;
    finalPosition += (inTexCoord.x-0.5f)*sideVector*0.5f*xPointSpriteSizeWidth;
    finalPosition += (0.5f-inTexCoord.y)*upVector*0.5f*xPointSpriteSizeHeight;

    float4 finalPosition4 = float4(finalPosition, 1);

    float4x4 preViewProjection = mul (xView, xProjection);
    Output.Position = mul(finalPosition4, preViewProjection);

    Output.TextureCoords = inTexCoord;

    return Output;
}

// ----- Pixel Buffer ----- //
struct PixelToFrame
{
    float4 Color : COLOR0;
};

PixelToFrame PointSpritePS(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;
	
	float4 color = tex2D(TextureSampler, PSIn.TextureCoords);


	if ( xAlphaTest )
		clip((color.a - xAlphaTestValue) * ( xAlphaTestGreater ? 1 : -1));

	// Set final colour
    Output.Color = color;

    return Output;
}

// ---- Techniques ----- //
technique PointSprites
{
	pass Pass0
	{   
		VertexShader = compile vs_2_0 PointSpriteVS();
		PixelShader  = compile ps_2_0 PointSpritePS();
	}
}