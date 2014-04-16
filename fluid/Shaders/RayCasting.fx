
float4x4 World;
float4x4 WorldViewProj;
float4x4 WorldInvTrans;

float3 StepSize;
int Iterations;

int Side = 2;

float4 ScaleFactor;

Texture2D Front;
Texture2D Back;
Texture3D Volume;


SamplerState FrontS
{
	Filter = MIN_MAG_MIP_LINEAR;

	AddressU = Wrap;				
    AddressV = Wrap;				
};

SamplerState BackS
{
	Filter = MIN_MAG_MIP_LINEAR;
	
	AddressU = Wrap;				
    AddressV = Wrap;				
};

SamplerState VolumeS
{
    Filter = MIN_MAG_MIP_LINEAR;

	AddressU = Wrap;				
    AddressV = Wrap;				
    AddressW = Wrap;
};


struct VertexShaderInput
{
    float4 Position : POSITION;
    float2 texC		: TEXCOORD;
};

struct VertexShaderOutput
{
    float4 Position		: SV_POSITION;
    float3 texC			: TEXCOORD0;
    float4 pos			: TEXCOORD1;
	//float4 color		: COLOR;
};

VertexShaderOutput VolumeVertexShader(VertexShaderInput input)
{
    VertexShaderOutput output= (VertexShaderOutput)0;

	output.texC = input.Position;

	input.Position.w = 1.0f;
		
	output.Position = mul(input.Position /* * ScaleFactor*/, WorldViewProj);
	//output.Position = input.Position;
	//output.Position.w = 1.0f;
	//output.texC = float4(0.0f, 0.0f, 0.0f, 0.0f);
	//output.color.rgba = float4(1.0f,0.5f,1.0f,1.0f);
	//output.pos = float4(0.0f, 0.0f, 0.0f, 0.0f);
	
	output.pos = output.Position;

    return output;
}

float4 PositionPS(VertexShaderOutput input) : SV_TARGET
{
	//return input.color;
    return float4(input.texC, 1.0f);
}

float4 WireFramePS(VertexShaderOutput input) : SV_TARGET
{
    return float4(1.0f, .5f, 0.0f, .85f);
}

//draws the front or back positions, or the ray direction through the volume
float4 DirectionPS(VertexShaderOutput input) : SV_TARGET
{
	float2 texC = input.pos.xy /= input.pos.w;
	texC.x =  0.5f*texC.x + 0.5f; 
	texC.y = -0.5f*texC.y + 0.5f;
	
    float3 front = Front.Sample(FrontS, texC).rgb;
    float3 back = Back.Sample(BackS, texC).rgb;
	
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

float4 VolumePixelShader(VertexShaderOutput input) : SV_TARGET
{ 
	//calculate projective texture coordinates
	//used to project the front and back position textures onto the cube
	float2 texC = input.pos.xy /= input.pos.w;
	texC.x =  0.5f*texC.x + 0.5f; 
	texC.y = -0.5f*texC.y + 0.5f;  
	
    float3 front = Front.Sample(FrontS, texC).xyz;
    float3 back = Back.Sample(BackS, texC).xyz;
    
    float3 dir = normalize(back - front);
    float4 pos = float4(front, 0);
    
    float4 dst = float4(0, 0, 0, 0);
    float4 src = 0;
    
    float value = 0;
	
	float3 Step = dir * StepSize;
    [unroll(50)]
    for(int i = 0; i < Iterations; i++)
    {
		pos.w = 0;
		value = Volume.Sample(VolumeS, pos).r;
				
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

		//break if the position is greater than <1, 1, 1> or smaller then <0,0,0>
		if (pos.x < 0.0f || pos.y < 0.0f || pos.z < 0.0f)
			break;
    }
    
    return dst;
}
/*
RasterizerState WireframeRS
{
	FillMode = Wireframe;
	CullMode = Back;
	FrontCounterClockwise = false;
};*/
RasterizerState BackRS
{
	//FillMode = Normal;
	//CullMode = Back;
	FrontCounterClockwise = true;
};
RasterizerState FrontRS
{
	//FillMode = Normal;
	//CullMode = Back;
	FrontCounterClockwise = false;
};

technique11 RenderPositionBack
{
    pass Pass0
    {		
		SetVertexShader(CompileShader(vs_4_0, VolumeVertexShader()));
		SetPixelShader(CompileShader(ps_4_0, PositionPS()));
		SetRasterizerState(BackRS);
        //VertexShader = compile vs_2_0 PositionVS();
        //PixelShader = compile ps_2_0 PositionPS();
    }
}
technique11 RenderPositionFront
{
	pass Pass0
	{
		SetVertexShader(CompileShader(vs_4_0, VolumeVertexShader()));
		SetPixelShader(CompileShader(ps_4_0, PositionPS()));
		SetRasterizerState(FrontRS);
		//VertexShader = compile vs_2_0 PositionVS();
		//PixelShader = compile ps_2_0 PositionPS();
	}
}

technique11 RayCastDirection
{
    pass Pass0
    {		
		SetVertexShader(CompileShader(vs_4_0, VolumeVertexShader()));
		SetPixelShader(CompileShader(ps_4_0, DirectionPS()));
        //VertexShader = compile vs_2_0 PositionVS();
        //PixelShader = compile ps_2_0 DirectionPS();
    }
}

technique11 RayCastSimple
{
    pass Pass0
    {		
		SetVertexShader(CompileShader(vs_4_0, VolumeVertexShader()));
		SetPixelShader(CompileShader(ps_4_0, VolumePixelShader()));
		//VertexShader = compile vs_3_0 PositionVS();
        //PixelShader = compile ps_3_0 RayCastSimplePS();
    }
}

technique11 WireFrame
{
    pass Pass0
    {		
		SetVertexShader(CompileShader(vs_4_0, VolumeVertexShader()));
		SetPixelShader(CompileShader(ps_4_0, WireFramePS()));
        //VertexShader = compile vs_2_0 PositionVS();
        //PixelShader = compile ps_2_0 WireFramePS();
    }
}
