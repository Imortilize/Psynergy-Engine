float2 xHalfPixel = float2(0, 0); // -1f / sceneTexture.Width, 1f / sceneTexture.Height
float2 xPixelSize = float2(0, 0); // 1 / sceneTexture.Width, 1 / sceneTexture.Height

float xThresholdColor = 0.2f;
float xThresholdDepth = 0.2f;

texture xSceneTexture;
sampler2D sceneSampler : register(s10) = sampler_state
{
	Texture = <xSceneTexture>;
};

texture xDepthTexture;
sampler2D depthSampler : register(s11) = sampler_state
{
	Texture = <xDepthTexture>;
	MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

//////////////////////////////////////////////
////////////// Data Structs //////////////////
//////////////////////////////////////////////
struct VertexStructureInput
{
	float4 Position		: POSITION;
	float2 UV			: TEXCOORD0;
};

struct VertexStructureOutput
{
	float4 Position		: POSITION;
	float2 UV			: TEXCOORD0;
};

//////////////////////////////////////////////
////////////// Vertex Shader /////////////////
//////////////////////////////////////////////

VertexStructureOutput VertexShaderFunction(VertexStructureInput input)
{
	VertexStructureOutput output = (VertexStructureOutput)0;
	
	output.Position = input.Position;
	output.Position.xy += xHalfPixel; // 
	output.UV = input.UV; 
	
	return output;
} // vs_main


//////////////////////////////////////////////
////////////// Edge Detection ////////////////
//////////////////////////////////////////////

float4 EdgeDetectionColorPS(in float2 UV : TEXCOORD0) : COLOR0
{
    float3 weights = float3(0.2126f, 0.7152f, 0.0722f);

    float l        = dot(tex2D(sceneSampler, UV).rgb, weights);
    float lLeft    = dot(tex2D(sceneSampler, UV + float2(-xPixelSize.x, 0)).rgb, weights);
    float lTop     = dot(tex2D(sceneSampler, UV + float2(0, -xPixelSize.y)).rgb, weights);

    float2 delta = abs(l.xx - float2(lLeft, lTop));
    float2 edges = step(xThresholdColor.xx, delta);

    return float4(edges, 0, 0);
} // EdgeDetectionColorPS

float4 EdgeDetectionDepthPS(in float2 UV : TEXCOORD0) : COLOR0
{
    float d       = tex2D(depthSampler, UV).r;
    float dLeft   = tex2D(depthSampler, UV + float2(-xPixelSize.x, 0)).r;
    float dTop    = tex2D(depthSampler, UV + float2(0, -xPixelSize.y)).r;

    float2 delta = abs(d.xx - float2(dLeft, dTop));

    // Dividing by 10 give us results similar to the color-based detection
    float2 edges = step(xThresholdDepth.xx / 10, delta);

    return float4(edges, 0, 0);
} // EdgeDetectionDepthPS

float4 EdgeDetectionColorDepthPS(VertexStructureOutput input) : COLOR0
{
    float3 weights = float3(0.2126f, 0.7152f, 0.0722f);

    float l        = dot(tex2D(sceneSampler, input.UV).rgb, weights);
    float lLeft    = dot(tex2D(sceneSampler, input.UV + float2(-xPixelSize.x, 0)).rgb, weights);
    float lTop     = dot(tex2D(sceneSampler, input.UV + float2(0, -xPixelSize.y)).rgb, weights);

    float2 delta = abs(l.xx - float2(lLeft, lTop));
    float2 edgescolor = step(xThresholdColor.xx, delta);
	
    float d       = tex2D(depthSampler, input.UV).r;
    float dLeft   = tex2D(depthSampler, input.UV + float2(-xPixelSize.x, 0)).r;
    float dTop    = tex2D(depthSampler, input.UV + float2(0, -xPixelSize.y)).r;

    delta = abs(d.xx - float2(dLeft, dTop));

    // Dividing by 10 give us results similar to the color-based detection
    float2 edgesdepth = step(xThresholdDepth.xx / 10, delta);

    return float4(edgescolor + edgesdepth, 0, 0);
} // EdgeDetectionDepthPS

technique EdgesColorDepth
{
    pass Pass1
    {
		VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 EdgeDetectionColorDepthPS();
    }
}
