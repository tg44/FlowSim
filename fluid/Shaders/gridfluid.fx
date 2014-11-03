Texture3D<float3> velocity;
Texture3D<float3> density;
Texture3D<float> pressure;
Texture3D<float> divergence;
Texture3D<float> temperature;

SamplerState linearSampler
{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

SamplerState zeroBoundarySampler
{
	filter = MIN_MAG_MIP_LINEAR;
	addressU = Border;
	addressV = Border;
	addressW = Border;
	borderColor = float4(0,0,0,0);
};

SamplerState clampSampler
{
	filter = MIN_MAG_MIP_LINEAR;
	addressU = Clamp;
	addressV = Clamp;
	addressW = Clamp;
};

RWTexture3D<float3> outputVelocity;
RWTexture3D<float3> outputDensity;
RWTexture3D<float> outputPressure;
RWTexture3D<float> outputDivergence;
RWTexture3D<float> outputTemperature;

float dt=1.0/20.0;
uint3 gridSize = uint3(20, 20, 20);

float3 injectionPos = float3(0.5, 0.5, 0.5);
float3 injectionIntensity = float3(0.005, 0.005, 0.005);

float dissipate= 0.995f;
float decay = 0.0f;

[numthreads(1, 1, 1)]
void csAdvect( uint3 dtid : SV_DispatchThreadID)
{
	//read velocity value from velocity field
	//determine backward vector
	float3 s = velocity.Load(uint4(dtid, 0)) * dt;

	float3 xds = ((float3)dtid + float3(0.5,0.5,0.5)) / (float3)gridSize - s;
	float multip = 1;
	if(any(xds < 0 || xds > 1))
		multip *= -1;
	outputVelocity[dtid] = velocity.SampleLevel(clampSampler, xds, 0) * multip
		//+  float3(0, 0.01, 0);
		//		+ normalize(xds - injectionPos) * dt * injectionIntensity * 10.01/(length(xds - injectionPos)+0.1)
		//		+ cross(normalize(xds - injectionPos),float3(0,0,1)) * dt * injectionIntensity * 100/(length(xds - injectionPos)+0.1)
		//		+ float3(0,10.1,0) * dt * injectionIntensity * multip / (length(xds - injectionPos)+0.1)
		;
	outputDensity[dtid] = density.SampleLevel( zeroBoundarySampler, xds, 0) 
		//+ 1/(length(xds - injectionPos)+0.0001) * dt * injectionIntensity
		;
	outputTemperature[dtid] = max(0, temperature.SampleLevel(linearSampler,xds,0) * dissipate - decay)
		;
}

float ambientTemperature = 0.0f;
float buoyancy = 1.0f;
float weight = 0.0f;
float3 up = (0, 0, 1);

[numthreads(1, 1, 1)]
void csBouyancy(uint3 dtid : SV_DispatchThreadID)
{

	uint4 dtl = uint4(dtid, 0);

	float T = temperature.Load(dtl).x;
	float D = density.Load(dtl).x;
	float3 V = velocity.Load(dtl).xyz;

		if (T > ambientTemperature)
			V += (dt * (T - ambientTemperature) * buoyancy - D * weight) * up.xyz;

	outputVelocity[dtid] = V;
}

[numthreads(1, 1, 1)]
void csDivergence( uint3 dtid : SV_DispatchThreadID)
{
	uint4 dtl = uint4(dtid, 0);
	dtl.x -= 1;
	float divergence = -velocity.Load(dtl).x;
	dtl.x += 2;
	divergence += velocity.Load(dtl).x;
	dtl.xy += uint2(-1,-1);
	divergence -= velocity.Load(dtl).y;
	dtl.y += 2;
	divergence += velocity.Load(dtl).y;
	dtl.yz += uint2(-1,-1);
	divergence -= velocity.Load(dtl).z;
	dtl.z += 2;
	divergence += velocity.Load(dtl).z;

	outputDivergence[dtid] = divergence * 0.5;
}

float jacobiAlpha = -1;
float jacobiBeta = 6;

[numthreads(1, 1, 1)]
void csJacobiPressure( uint3 dtid : SV_DispatchThreadID)
{
	uint4 dtl = uint4(dtid, 0);
	float p = divergence.Load(dtl) * jacobiAlpha;
	dtl.x -= 1;
	p += pressure.Load(dtl);
	dtl.x += 2;
	p += pressure.Load(dtl);
	dtl.xy += uint2(-1,-1);
	p += pressure.Load(dtl);
	dtl.y += 2;
	p += pressure.Load(dtl);
	dtl.yz += uint2(-1,-1);
	p += pressure.Load(dtl);
	dtl.z += 2;
	p += pressure.Load(dtl);

	outputPressure[dtid] = p / jacobiBeta;
}

[numthreads(1, 1, 1)]
void csProject( uint3 dtid : SV_DispatchThreadID)
{
	uint4 dtl = uint4(dtid, 0);
	float3 gradp;
	dtl.x -= 1;
	gradp.x = -pressure.Load(dtl);
	dtl.x += 2;
	gradp.x += pressure.Load(dtl);
	dtl.xy += uint2(-1,-1);
	gradp.y = -pressure.Load(dtl);
	dtl.y += 2;
	gradp.y += pressure.Load(dtl);
	dtl.yz += uint2(-1,-1);
	gradp.z = -pressure.Load(dtl);
	dtl.z += 2;
	gradp.z += pressure.Load(dtl);
	dtl.z -= 1;

	outputVelocity[dtid] = velocity.Load(dtl) - gradp;
}

float4x4 orientProjMatrixInverse;

struct QuadInput
{
	float4  pos			: POSITION;
	float2  tex			: TEXCOORD0;
};

struct QuadOutput
{
	float4 pos				: SV_POSITION;
	float2 tex				: TEXCOORD0;
	float3 viewDir			: TEXCOORD1;
};

QuadOutput vsQuad(QuadInput input)
{
	QuadOutput output = (QuadOutput)0;

	output.pos = input.pos;
	float4 hWorldPosMinusEye = mul(input.pos, orientProjMatrixInverse);
	hWorldPosMinusEye /= hWorldPosMinusEye.w;
	output.viewDir = hWorldPosMinusEye.xyz;
	output.pos.z = 0.99999;
	output.tex = input.tex;
	return output;
};

float4 psRayMarching(QuadOutput input) : SV_TARGET
{
	float3 r=0;
	for(int u=0; u<32; u++)
		r += density.SampleLevel(linearSampler, float3(input.tex, 0.5), u / 32.0).xyzz;
	return r.xyzz / 32.0;
////	return density.SampleLevel(linearSampler, float3(input.tex, 0.5), 0).xxxx * float4(10, -1, -0.1, 0);
	return velocity.SampleLevel(linearSampler, float3(input.tex, 0.5), 0).xyzz;
};

BlendState defaultBlender
{
};

DepthStencilState defaultCompositor
{
};

RasterizerState defaultRasterizer
{
	CullMode = Back;
};

technique11 gridFluid
{
	pass advect
	{
		SetComputeShader(CompileShader( cs_5_0, csAdvect() ));
	}
	pass bouyancy
	{
		SetComputeShader(CompileShader(cs_5_0, csBouyancy()));
	}
	pass divergence
	{
		SetComputeShader(CompileShader( cs_5_0, csDivergence() ));
	}
	pass jacobiPressure
	{
		SetComputeShader(CompileShader( cs_5_0, csJacobiPressure() ));
	}
	pass project
	{
		SetComputeShader(CompileShader( cs_5_0, csProject() ));
	}
	pass rayMarching
	{
		SetVertexShader ( CompileShader( vs_4_0, vsQuad() ) );
		SetGeometryShader( NULL );
		SetRasterizerState( defaultRasterizer );
		SetPixelShader( CompileShader( ps_4_0, psRayMarching() ) );
		SetDepthStencilState( defaultCompositor, 0);
		SetBlendState(defaultBlender, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xffffffff);
	}
}