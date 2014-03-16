
float4x4 World;
float4x4 WorldViewProj;
float4x4 WorldInvTrans;

float3 StepSize;
int Iterations;

int Side = 2;

float4 ScaleFactor;

texture2D Front;
texture2D Back;
texture3D Volume;

sampler2D FrontS = sampler_state
{
	Texture = <Front>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
	
	AddressU = Border;				// border sampling in U
    AddressV = Border;				// border sampling in V
    BorderColor = float4(0,0,0,0);	// outside of border should be black
};

sampler2D BackS = sampler_state
{
	Texture = <Back>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
	
	AddressU = Border;				// border sampling in U
    AddressV = Border;				// border sampling in V
    BorderColor = float4(0,0,0,0);	// outside of border should be black
};

sampler3D VolumeS = sampler_state
{
	Texture = <Volume>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
	
	AddressU = Border;				// border sampling in U
    AddressV = Border;				// border sampling in V
    AddressW = Border;
    BorderColor = float4(0,0,0,0);	// outside of border should be black
};


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

VertexShaderOutput PositionVS(VertexShaderInput input)
{
    VertexShaderOutput output;
	
    output.Position = mul(input.Position * ScaleFactor, WorldViewProj);
    
    output.texC = input.Position;
    output.pos = output.Position;

    return output;
}

float4 PositionPS(VertexShaderOutput input) : COLOR0
{
    return float4(input.texC, 1.0f);
}

float4 WireFramePS(VertexShaderOutput input) : COLOR0
{
    return float4(1.0f, .5f, 0.0f, .85f);
}

//draws the front or back positions, or the ray direction through the volume
float4 DirectionPS(VertexShaderOutput input) : COLOR0
{
	float2 texC = input.pos.xy /= input.pos.w;
	texC.x =  0.5f*texC.x + 0.5f; 
	texC.y = -0.5f*texC.y + 0.5f;
	
    float3 front = tex2D(FrontS, texC).rgb;
    float3 back = tex2D(BackS, texC).rgb;
	
	if(Side == 0)
	{
		return float4(front, .9f);
	}
	if(Side == 1)
	{
		return float4(back, .9f);
	}
    
    return float4(back - front, .9f);
}

float4 RayCastSimplePS(VertexShaderOutput input) : COLOR0
{ 
	//calculate projective texture coordinates
	//used to project the front and back position textures onto the cube
	float2 texC = input.pos.xy /= input.pos.w;
	texC.x =  0.5f*texC.x + 0.5f; 
	texC.y = -0.5f*texC.y + 0.5f;  
	
    float3 front = tex2D(FrontS, texC).xyz;
    float3 back = tex2D(BackS, texC).xyz;
    
    float3 dir = normalize(back - front);
    float4 pos = float4(front, 0);
    
    float4 dst = float4(0, 0, 0, 0);
    float4 src = 0;
    
    float value = 0;
	
	float3 Step = dir * StepSize;
    
    for(int i = 0; i < Iterations; i++)
    {
		pos.w = 0;
		value = tex3Dlod(VolumeS, pos).r;
				
		src = (float4)value;
		src.a *= .1f; //reduce the alpha to have a more transparent result
					  //this needs to be adjusted based on the step size
					  //i.e. the more steps we take, the faster the alpha will grow	
			
		//Front to back blending
		// dst.rgb = dst.rgb + (1 - dst.a) * src.a * src.rgb
		// dst.a   = dst.a   + (1 - dst.a) * src.a		
		src.rgb *= src.a;
		dst = (1.0f - dst.a)*src + dst;		
		
		//break from the loop when alpha gets high enough
		if(dst.a >= .95f)
			break;	
		
		//advance the current position
		pos.xyz += Step;
		
		//break if the position is greater than <1, 1, 1>
		if(pos.x > 1.0f || pos.y > 1.0f || pos.z > 1.0f)
			break;
    }
    
    return dst;
}

technique RenderPosition
{
    pass Pass1
    {		
        VertexShader = compile vs_2_0 PositionVS();
        PixelShader = compile ps_2_0 PositionPS();
    }
}

technique RayCastDirection
{
    pass Pass1
    {		
        VertexShader = compile vs_2_0 PositionVS();
        PixelShader = compile ps_2_0 DirectionPS();
    }
}

technique RayCastSimple
{
    pass Pass1
    {		
        VertexShader = compile vs_3_0 PositionVS();
        PixelShader = compile ps_3_0 RayCastSimplePS();
    }
}

technique WireFrame
{
    pass Pass1
    {		
        VertexShader = compile vs_2_0 PositionVS();
        PixelShader = compile ps_2_0 WireFramePS();
    }
}
