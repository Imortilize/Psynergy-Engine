#define NUMSAMPLES 8

//Projection matrix
float4x4 xProjection;

//Corner Frustum
float3 xCornerFrustum;

//Sample Radius
float xSampleRadius;

//Distance Scale
float xDistanceScale;

//GBuffer Texture Size
float2 xGBufferSize;

//Samplers
sampler GBuffer1 : register(s1);
sampler GBuffer2 : register(s2);
sampler RandNormal : register(s3);

//Vertex Input Structure
struct VSI
{
	float3 Position : POSITION0;
	float2 UV : TEXCOORD0;
};

//Vertex Output Structure
struct VSO
{
	float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
	float3 ViewDirection : TEXCOORD1;
};

//Vertex Shader
VSO VS(VSI input)
{
	//Initialize Output
	VSO output;

	//Just Straight Pass Position
	output.Position = float4(input.Position, 1);

	//Set up UV's
	output.UV = input.UV - float2(1.0f / xGBufferSize.xy);

	//Set up ViewDirection vector
	output.ViewDirection = float3(-xCornerFrustum.x * input.Position.x, xCornerFrustum.y * input.Position.y, xCornerFrustum.z);

	//Return
	return output;
}

//Normal Decoding Function
float3 decode(float3 enc)
{
	return (2.0f * enc.xyz- 1.0f);
}

//Pixel Shader
float4 PS(VSO input) : COLOR0
{
	//Sample Vectors
	float4 samples[8] =
	{
		float4(0.355512, 	-0.709318, 	-0.102371,	0.0 ),
		float4(0.534186, 	0.71511, 	-0.115167,	0.0 ),
		float4(-0.87866, 	0.157139, 	-0.115167,	0.0 ),
		float4(0.140679, 	-0.475516, 	-0.0639818,	0.0 ),
		float4(-0.207641, 	0.414286, 	0.187755,	0.0 ),
		float4(-0.277332, 	-0.371262, 	0.187755,	0.0 ),
		float4(0.63864, 	-0.114214, 	0.262857,	0.0 ),
		float4(-0.184051, 	0.622119, 	0.262857,	0.0 )
	};

	//Normalize the input ViewDirection
	float3 ViewDirection = normalize(input.ViewDirection);

	//Sample the depth
	float depth = tex2D(GBuffer2, input.UV).g;
	
	//ignore areas where we have no depth/normal information
	clip(-depth + 0.9999f);

	//Calculate the depth at this pixel along the view direction
	float3 se = depth * ViewDirection;

	//Sample a random normal vector
	float3 randNormal = (tex2D(RandNormal, input.UV * 100.0f).xyz * 2.0f - 1.0f);
	randNormal = normalize(randNormal);

	//Sample the Normal for this pixel
	float3 normal = decode(tex2D(GBuffer1, input.UV).xyz);
	
	//No assymetry in HLSL, workaround
	float finalColor = 0.0f;
	
	//SSAO loop
	for (int i = 0; i < NUMSAMPLES; i++)
	{
		//Calculate the Reflection Ray
		float3 ray = reflect(samples[i].xyz, randNormal) * xSampleRadius;
		
		//Test the Reflection Ray against the surface normal
		if(dot(ray, normal) < 0) ray += normal * xSampleRadius;
		
		//Calculate the Sample vector
		float4 sample = float4(se + ray, 1.0f);
		
		//Project the Sample vector into ScreenSpace
		float4 ss = mul(sample, xProjection);

		//Convert SS into UV space
		float2 sampleTexCoord = 0.5f * ss.xy / ss.w + float2(0.5f, 0.5f);
		
		//Sample the Depth along the ray
		float sampleDepth = tex2D(GBuffer2, sampleTexCoord).g;
		
		//Check the sampled depth value
		if (sampleDepth == 1.0)
		{
			//Non-Occluded sample
			finalColor++;
		}
		else
		{	
			//Calculate Occlusion
			float occlusion = xDistanceScale * max(sampleDepth - depth, 0.0f);
			
			//Accumulate to finalColor
			finalColor += 1.0f / (1.0f + occlusion * occlusion * 0.1);
		}
	}

	//Output the Average of finalColor
	return float4(finalColor / NUMSAMPLES, finalColor / NUMSAMPLES, finalColor / NUMSAMPLES, 1.0f);
}

technique Default
{
	pass p0
	{
		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_3_0 PS();
	}
}