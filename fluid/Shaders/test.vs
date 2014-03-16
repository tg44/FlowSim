float4x4 World;
float4x4 WorldViewProj;
float4x4 WorldInvTrans;



float4 ScaleFactor;



struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 texC		: TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position		: POSITION0;
    float3 texC			: TEXCOORD0;
    float4 pos			: TEXCOORD1;
};

VertexShaderOutput VolumeVertexShader(VertexShaderInput input)
{
    VertexShaderOutput output;
	
    output.Position = mul(input.Position * ScaleFactor, WorldViewProj);
    
    output.texC = input.Position;
    output.pos = output.Position;

    return output;
}