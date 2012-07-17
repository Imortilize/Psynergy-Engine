
#include <..\Helpers\Discard.fxh>

float2 xHalfPixel = float2(0, 0); // -1f / sceneTexture.Width, 1f / sceneTexture.Height
float2 xPixelSize = float2(0, 0); // 1 / sceneTexture.Width, 1 / sceneTexture.Height

float xBlurRadius = 5.0f;

// maximum search steps
#define MAX_SEARCH_STEPS 6

texture xSceneTexture;
sampler2D sceneSampler : register(s12) = sampler_state
{
	Texture = <xSceneTexture>;
};

sampler2D sceneLinearSampler : register(s13) = sampler_state
{
	Texture = <xSceneTexture>;
};

texture xEdgeTexture;
sampler2D edgeSampler : register(s14) = sampler_state
{
	Texture = <xEdgeTexture>;
};

texture xAreaTexture;
sampler2D areaSampler : register(s15) = sampler_state
{
	Texture = <xAreaTexture>;
};

texture xBlendedWeightsTexture;
sampler2D blendedWeightsSampler : register(s4) = sampler_state
{
	Texture = <xBlendedWeightsTexture>;
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
////////////// Blending Weight ///////////////
//////////////////////////////////////////////

float SearchXLeft(float2 texcoord)
{
    texcoord -= float2(1.5, 0.0) * xPixelSize;
    float e = 0.0;
    // We offset by 0.5 to sample between edgels, thus fetching two in a row	
	[unroll(MAX_SEARCH_STEPS)]
    for (int i = 0; i < MAX_SEARCH_STEPS; i++)
	{
        e = tex2Dlod(edgeSampler, float4(texcoord, 0, 0)).g;
        // We compare with 0.9 to prevent bilinear access precision problems		
		[flatten]
		if (e < 0.9)
			break;
        texcoord -= float2(2.0, 0.0) * xPixelSize;
    }
    // When we exit the loop without founding the end, we want to return
    // -2 * maxSearchSteps
    return max(-2.0 * i - 2.0 * e, -2.0 * MAX_SEARCH_STEPS);
}

float SearchXRight(float2 texcoord)
{
    texcoord += float2(1.5, 0.0) * xPixelSize;
    float e = 0.0;	
	[unroll(MAX_SEARCH_STEPS)]
    for (int i = 0; i < MAX_SEARCH_STEPS; i++)
	{
        e = tex2Dlod(edgeSampler, float4(texcoord, 0, 0)).g;
		[flatten]
		if (e < 0.9)
			break;
        texcoord += float2(2.0, 0.0) * xPixelSize;
    }
    return min(2.0 * i + 2.0 * e, 2.0 * MAX_SEARCH_STEPS);
}

float SearchYUp(float2 texcoord)
{
    texcoord -= float2(0.0, 1.5) * xPixelSize;
    float e = 0.0;	
	[unroll(MAX_SEARCH_STEPS)]
    for (int i = 0; i < MAX_SEARCH_STEPS; i++)
	{
        e = tex2Dlod(edgeSampler, float4(texcoord, 0, 0)).r;        
		[flatten]
		if (e < 0.9)
			break;
        texcoord -= float2(0.0, 2.0) * xPixelSize;
    }
    return max(-2.0 * i - 2.0 * e, -2.0 * MAX_SEARCH_STEPS);
}

float SearchYDown(float2 texcoord)
{
    texcoord += float2(0.0, 1.5) * xPixelSize;
    float e = 0.0;
	[unroll(MAX_SEARCH_STEPS)]
    for (int i = 0; i < MAX_SEARCH_STEPS; i++)
	{
        e = tex2Dlod(edgeSampler, float4(texcoord, 0, 0)).r;
		[flatten]
		if (e < 0.9)
			break;
        texcoord += float2(0.0, 2.0) * xPixelSize;
    }
    return min(2.0 * i + 2.0 * e, 2.0 * MAX_SEARCH_STEPS);
}

float4 mad(float4 m, float4 a, float4 b)
{
    #if defined(XBOX)
		float4 result;
		asm
		{
			mad result, m, a, b
		};
		return result;
    #else
		return m * a + b;
    #endif
}

#define NUM_DISTANCES 32
#define AREA_SIZE (NUM_DISTANCES * 5)

float2 Area(float2 distance, float e1, float e2) {
     // * By dividing by AREA_SIZE - 1.0 below we are implicitely offsetting to
     //   always fall inside of a pixel
     // * Rounding prevents bilinear access precision problems
    float2 pixcoord = NUM_DISTANCES * round(4.0 * float2(e1, e2)) + distance;
    float2 texcoord = pixcoord / (AREA_SIZE - 1.0);   
	return tex2Dlod(areaSampler, float4(texcoord, 0, 0)).rg;
}

float4 BlendingWeightCalculationPS(in float2 texcoord : TEXCOORD0) : COLOR0
{
    float4 weights = 0.0f;

    float2 e = tex2D(edgeSampler, texcoord).rg;
	if (dot(e, 1.0) == 0.0) // if there is no edge then discard.
	{		
        Discard();
    }
	
    [branch]
    if (e.g) // Edge at north
	{
        float2 d = float2(SearchXLeft(texcoord), SearchXRight(texcoord));
        
        // Instead of sampling between edgels, we sample at -0.25,
        // to be able to discern what value each edgel has.
        float4 coords = mad(float4(d.x, -0.25, d.y + 1.0, -0.25),
                            xPixelSize.xyxy, texcoord.xyxy);
        float e1 = tex2Dlod(edgeSampler, float4(coords.xy, 0, 0)).r;
        float e2 = tex2Dlod(edgeSampler, float4(coords.zw, 0, 0)).r;
        weights.rg = Area(abs(d), e1, e2);
    }
	
    [branch]
    if (e.r) // Edge at west
	{ 
        float2 d = float2(SearchYUp(texcoord), SearchYDown(texcoord));		
		
        float4 coords = mad(float4(-0.25, d.x, -0.25, d.y + 1.0),
                            xPixelSize.xyxy, texcoord.xyxy);
        float e1 = tex2Dlod(edgeSampler, float4(coords.xy, 0, 0)).g;
        float e2 = tex2Dlod(edgeSampler, float4(coords.zw, 0, 0)).g;
        weights.ba = Area(abs(d), e1, e2);
    }
	
    return weights;
}



//////////////////////////////////////////////
/////////// Neighborhood Blending ////////////
//////////////////////////////////////////////

float4 NeighborhoodBlendingPS(in float2 texcoord : TEXCOORD0) : COLOR0
{	
    float2 topLeft = tex2D(blendedWeightsSampler, texcoord).rb;
    float right = tex2D(blendedWeightsSampler, texcoord + float2(0, xPixelSize.y)).g;
    float bottom = tex2D(blendedWeightsSampler, texcoord + float2(xPixelSize.x, 0)).a;
    float4 a = float4(topLeft.x, right, topLeft.y, bottom);
		
    float sum = dot(a, 1.0);
	
    [branch]
    if (sum > 0.0)
	{		
        float4 o = a * xPixelSize.yyxx * xBlurRadius;
        float4 color = 0.0;
        color = mad(tex2Dlod(sceneLinearSampler, float4(texcoord + float2( 0.0, -o.r), 0, 0)), a.r, color);
        color = mad(tex2Dlod(sceneLinearSampler, float4(texcoord + float2( 0.0,  o.g), 0, 0)), a.g, color);
        color = mad(tex2Dlod(sceneLinearSampler, float4(texcoord + float2(-o.b,  0.0), 0, 0)), a.b, color);
        color = mad(tex2Dlod(sceneLinearSampler, float4(texcoord + float2( o.a,  0.0), 0, 0)), a.a, color);
        return (color / sum);
    }
	else
	{	
        return tex2D(sceneSampler, texcoord);
    }
}

technique BlendingWeight
{
    pass Pass1
    {
		VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 BlendingWeightCalculationPS();
    }
}

technique NeighborhoodBlending
{
    pass Pass1
    {
		VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 NeighborhoodBlendingPS();
    }
}
