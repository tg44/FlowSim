///////////////////////
////   GLOBALS
///////////////////////
float4x4 worldMatrix = {{0.9995065, 0, -0.03141076, 0}, {0, 1, 0, 0}, {0.03141076, 0, 0.9995065, 0}, {3, 1, 3, 1}};
float4x4 viewMatrix = {{-0.841471, 0, 0.5403023, 0}, {-0.4741599, 0.4794255, -0.7384602, 0}, {-0.2590347, -0.8775826, -0.4034227, 10.53099}, {0, 0, 0, 1}};
float4x4 projectionMatrix = {{2.013038, 0, 0, 0}, {0, 2.414213, 0, 0}, {0, 0, 1.0001, -0.10001}, {0, 0, 1, 0}};

//////////////////////
////   TYPES
//////////////////////
struct VertexInputType
{
	float4 position : POSITION;
	float4 color : COLOR;
};

struct PixelInputType
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
};

/////////////////////////////////////
/////   Vertex Shader
/////////////////////////////////////
PixelInputType ColorVertexShader(VertexInputType input)
{
	PixelInputType output;

	// Change the position vector to be 4 units for proper matrix calculations.
	input.position.w = 1.0f;

	// Calculate the position of the vertex against the world, view, and projection matrices.
	output.position = mul(input.position, worldMatrix);
	output.position = mul(output.position, viewMatrix);
	output.position = mul(output.position, projectionMatrix);

	// Store the input color for the pixel shader to use.
	output.color = input.color;

	return output;
}
//////////////////////
////   Pixel Shader
/////////////////////
float4 ColorPixelShader(PixelInputType input) : SV_TARGET
{
	return input.color;
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
		SetGeometryShader(0);
		SetVertexShader(CompileShader(vs_5_0, ColorVertexShader()));
		SetPixelShader(CompileShader(ps_5_0, ColorPixelShader()));

		SetRasterizerState(WireframeRS);

        //VertexShader = compile vs_5_0 ColorVertexShader();
        //PixelShader = compile ps_5_0 ColorPixelShader();
    }
}