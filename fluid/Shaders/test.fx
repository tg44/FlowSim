///////////////////////
////   GLOBALS
///////////////////////
float4x4 WorldViewProj;
//float4x4 viewMatrix;
//float4x4 projectionMatrix;

//////////////////////
////   TYPES
//////////////////////
struct VertexInputType
{
	float4 position : POSITION;
	float2 color : TEXCOORD;
};

struct PixelInputType
{
	float4 position : SV_POSITION;
	float2 color : TEXCOORD;
};

/////////////////////////////////////
/////   Vertex Shader
/////////////////////////////////////
PixelInputType ColorVertexShader(VertexInputType input)
{
	PixelInputType output= (PixelInputType)0;

	// Change the position vector to be 4 units for proper matrix calculations.
	input.position.w = 1.0f;

	// Calculate the position of the vertex against the world, view, and projection matrices.
	output.position = mul(input.position, WorldViewProj);
	//output.position = mul(output.position, viewMatrix);
	//output.position = mul(output.position, projectionMatrix);

	// Store the input color for the pixel shader to use.
	output.color = input.color;

	return output;
}
//////////////////////
////   Pixel Shader
/////////////////////
float4 ColorPixelShader(PixelInputType input) : SV_TARGET
{
	return float4(1.0f,0.5f,0.5f,1.0f);
}

RasterizerState WireframeRS
{
FillMode = Wireframe;
CullMode = Back;
FrontCounterClockwise = false;
};

technique11 RenderColor
{
    pass Pass0
    {		
		SetVertexShader(CompileShader(vs_4_0, ColorVertexShader()));
		SetPixelShader(CompileShader(ps_4_0, ColorPixelShader()));

		//SetRasterizerState(WireframeRS);

        //VertexShader = compile vs_5_0 ColorVertexShader();
        //PixelShader = compile ps_5_0 ColorPixelShader();
    }
}