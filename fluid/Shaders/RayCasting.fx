
float4x4 World;
float4x4 WorldViewProj;
float4x4 WorldInvTrans;

float3 StepSize;
int Iterations;

int Side = 2;

float4 ScaleFactor;

Texture2D Front;
Texture2D Back;

Texture2D WDepth;
Texture2D FDepth;
Texture2D BDepth;

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

SamplerState WDepthS
{
	Filter = MIN_MAG_MIP_LINEAR;

	AddressU = Wrap;
	AddressV = Wrap;
};
SamplerState FDepthS
{
	Filter = MIN_MAG_MIP_LINEAR;

	AddressU = Wrap;
	AddressV = Wrap;
};
SamplerState BDepthS
{
	Filter = MIN_MAG_MIP_LINEAR;

	AddressU = Wrap;
	AddressV = Wrap;
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

float4 ConstantColorPS(VertexShaderOutput input) : SV_TARGET
{
	return float4(1.0f, .7f, 0.5f, 1.0f);
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
    
    return float4(back - front, .6f);
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

		float wdepth = WDepth.Sample(WDepthS, texC).r;
	float bdepth = BDepth.Sample(BDepthS, texC).r;
	float fdepth = FDepth.Sample(FDepthS, texC).r;

	float3 dir = normalize(back - front);
		float4 pos = float4(front, 0);

		float4 dst = float4(0, 0, 0, 0);
		float4 src = 0;

		float2 value = float2(0, 0);
		float3 Step = dir * StepSize;

		/////////////////
		//felveszek egy intet az iterations helyett
		int iter = Iterations;
		//ha van melyseg az elso es hatso lap kozt
		if (bdepth>wdepth && fdepth<wdepth){
			////kinullazom az intet
			iter = 0;
			////kezdek egy forciklust amiben a steppel novelem a post
			[loop]
			for (int i = 0; i < Iterations; i++)
			{
				//////novelem az intet
				//////kilepek ha az egyik koordinata>1 vagy <0
				//////max iterations lepesszamban
				pos.xyz += Step;
				iter++;
				if (pos.x > 1.0f || pos.y > 1.0f || pos.z > 1.0f)
					break;
				if (pos.x < 0.0f || pos.y < 0.0f || pos.z < 0.0f)
					break;
			}
			iter = (wdepth - fdepth)*iter / (bdepth - fdepth);
			////az uj int ertek jo lesz az elolap-hatlap lepesszamra
			////nekem az elolap-koztes elem kell, aranyositom az uj intet ez alapjan
		}
		pos = float4(front, 0);
	//az also loop az uj intig megy csak!
	////////////////
    [loop]
    for(int i = 0; i < iter; i++)
    {
		pos.w = 0;
		value = Volume.SampleLevel(VolumeS, pos,0).xy;
		//src = Volume.SampleLevel(VolumeS, pos, 0).rgba;
		/*if (value.y < 0.375){
			src = float4(0, 0.6f*value.y, 1-(value.y-0.375f)*1.6f, value.x);
		}
		else if (value.y > 0.625){
			src = float4((value.y-0.375f)*1.6f, 0.6f*(1-value.y), 0, value.x);
		}
		else {
			src = float4((value.y - 0.375f)*1.6f, 3 * (-1.0f*(value.y - 0.5f)*(value.y - 0.5f))+0.6f, 1 - (value.y - 0.375f)*1.6f, value.x);
			}*/
		if (value.y < 0.5f){
			src = float4(0.0f,value.y*2.0f,1.0f-value.y*2.0f,value.x);
		}
		else{
			src = float4((value.y-0.5f)*2.0f, 2.0f-value.y*2.0f, 0.0f, value.x);
		}
		
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

float4 VolumeTestPixelShader(VertexShaderOutput input) : SV_TARGET
{
	//calculate projective texture coordinates
	//used to project the front and back position textures onto the cube
	float2 texC = input.pos.xy /= input.pos.w;
	texC.x = 0.5f*texC.x + 0.5f;
	texC.y = -0.5f*texC.y + 0.5f;

	float3 front = Front.Sample(FrontS, texC).xyz;
		float3 back = Back.Sample(BackS, texC).xyz;

		float3 dir = normalize(back - front);
		float4 pos = float4(front, 0);

		float4 dst = float4(0, 0, 0, 0);
		float4 src = 0;

		float2 value = float2(0, 0);

		float3 Step = dir * StepSize;
		[loop]
	for (int i = 0; i < Iterations; i++)
	{
		pos.w = 0;
		value = Volume.SampleLevel(VolumeS, pos, 0).rg;
		src = float4((1.0f - value.g), 0, value.g, value.r);
		//src = Volume.SampleLevel(VolumeS, pos, 0).rgba;

		//src = (float4)((1.0f - value.y), 0, value.y, value.x);
		//src.a *= .1f; //reduce the alpha to have a more transparent result
		//this needs to be adjusted based on the step size
		//i.e. the more steps we take, the faster the alpha will grow	

		//Front to back blending
		// dst.rgb = dst.rgb + (1 - dst.a) * src.a * src.rgb
		// dst.a   = dst.a   + (1 - dst.a) * src.a		
		//src.rgb *= src.a;
		//dst = (1.0f - dst.a)*src + dst;
		dst = src;
		//break from the loop when alpha gets high enough
		if (dst.a >= .95f)
			break;

		//advance the current position
		pos.xyz += Step;

		//break if the position is greater than <1, 1, 1>
		if (pos.x > 1.0f || pos.y > 1.0f || pos.z > 1.0f)
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

technique11 RenderConstansColor
{
	pass Pass0
	{
		SetVertexShader(CompileShader(vs_4_0, VolumeVertexShader()));
		SetPixelShader(CompileShader(ps_4_0, ConstantColorPS()));
		//SetRasterizerState(FrontRS);
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

technique11 RayCastTest
{
	pass Pass0
	{
		SetVertexShader(CompileShader(vs_4_0, VolumeVertexShader()));
		SetPixelShader(CompileShader(ps_4_0, VolumeTestPixelShader()));
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
