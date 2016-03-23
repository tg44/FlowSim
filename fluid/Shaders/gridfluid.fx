Texture3D<float3> velocity;
Texture3D<float3> density;
Texture3D<float> pressure;
Texture3D<float> divergence;
Texture3D<float> temperature;
Texture3D<float4> wall;
Texture3D<float4> velocyfield;

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

float dt=1.0/120.0;
float3 gridSize = float3(20, 20, 20);

float3 injectionPos = float3(0.5, 0.5, 0.5);
float3 injectionIntensity = float3(0.005, 0.005, 0.005);

float dissipate= 1.00f;
float decay = 0.0f;

[numthreads(128, 1, 1)]
void csAdvect( uint3 dtid : SV_DispatchThreadID)
{
	//read velocity value from velocity field
	//determine backward vector
	float3 s = velocity.Load(uint4(dtid, 0)) * dt;

	float3 xds = ((float3)dtid + float3(0.5,0.5,0.5)) / (float3)gridSize - s;
	if (any(xds < 0 || xds > 1)){
		outputVelocity[dtid] = s;
	}
	else {
		outputVelocity[dtid] = velocity.SampleLevel(clampSampler, xds, 0)
			//+  float3(0, 0.01, 0);
			//		+ normalize(xds - injectionPos) * dt * injectionIntensity * 10.01/(length(xds - injectionPos)+0.1)
			//		+ cross(normalize(xds - injectionPos),float3(0,0,1)) * dt * injectionIntensity * 100/(length(xds - injectionPos)+0.1)
			//		+ float3(0,10.1,0) * dt * injectionIntensity * multip / (length(xds - injectionPos)+0.1)
			;
	}
	outputDensity[dtid] = density.SampleLevel( zeroBoundarySampler, xds, 0) 
		//+ 1/(length(xds - injectionPos)+0.0001) * dt * injectionIntensity
		;
	
	outputTemperature[dtid] = max(0, temperature.SampleLevel(zeroBoundarySampler, xds, 0) * dissipate - decay)
		;/*
	if (outputTemperature[dtid] == 0){
		outputTemperature[dtid] = temperature[dtid];
	}*/
}

float ambientTemperature = 0.7f;
float buoyancy = 1.0f;
float weight = 0.1f;
float3 up = float3(0.0f, 1.0f, 0.0f);

[numthreads(128, 1, 1)]
void csBouyancy(uint3 dtid : SV_DispatchThreadID)
{

	uint4 dtl = uint4(dtid, 0);

	float T = temperature.Load(dtl).x;
	float D = density.Load(dtl).x;
	float3 V = velocity.Load(dtl).xyz;

	//if (T > ambientTemperature){
		V += (dt * (T - ambientTemperature) * buoyancy - D * weight) * up.xyz;
	//}

	outputVelocity[dtid] = V;
}

float energyloss = 0.001f;
[numthreads(128, 1, 1)]
void csWall(uint3 dtid : SV_DispatchThreadID)
{
	//is there a wall?
	uint4 dtl = uint4(dtid, 0);
	float4 w = wall.Load(dtl).xyzw;
	float3 v = velocity.Load(dtl).xyz;

	if (w.w > 0.1f){
		float3 norm = normalize(w.xyz);
		float3 dir = normalize(v.xyz);
		float costetha = -1 * dot(dir,norm);
		outputVelocity[dtid] = normalize((dir + norm * 2 * costetha))*length(v.xyz)*(1-energyloss);
	}
	else {
		outputVelocity[dtid] = v;
	}

	
}


float velocyFieldScale = 1.0f;
[numthreads(128, 1, 1)]
void csVelocyField(uint3 dtid : SV_DispatchThreadID)
{
	//is there a field?
	uint4 dtl = uint4(dtid, 0);
		float4 vf = velocyfield.Load(dtl).xyzw;
		float3 v = velocity.Load(dtl).xyz;

		if (vf.w > 0.1f){
		outputVelocity[dtid] = v.xyz + vf.xyz*velocyFieldScale;
		}
		else {
			outputVelocity[dtid] = v;
		}


}


[numthreads(128, 1, 1)]
void csDivergence( uint3 dtid : SV_DispatchThreadID)
{
	uint4 dtl = uint4(dtid, 0);
	uint4 idxL = uint4(max(0, dtl.x - 1), dtl.y, dtl.z, 0);
	uint4 idxR = uint4(min(gridSize.x - 1, dtl.x + 1), dtl.y, dtl.z, 0);
	uint4 idxB = uint4(dtl.x,max(0, dtl.y - 1), dtl.z, 0);
	uint4 idxT = uint4(dtl.x,min(gridSize.y - 1, dtl.y + 1), dtl.z, 0);
	uint4 idxD = uint4(dtl.x, dtl.y, max(0, dtl.z - 1), 0);
	uint4 idxU = uint4(dtl.x, dtl.y, min(gridSize.z - 1, dtl.z + 1), 0);

	float L = velocity.Load(idxL).x;
	float R = velocity.Load(idxR).x;
	float B = velocity.Load(idxB).y;
	float T = velocity.Load(idxT).y;
	float D = velocity.Load(idxD).z;
	float U = velocity.Load(idxU).z;

	if (wall.Load(idxL).w > 0.1) L = 0;
	if (wall.Load(idxR).w > 0.1) R = 0;
	if (wall.Load(idxB).w > 0.1) B = 0;
	if (wall.Load(idxT).w > 0.1) T = 0;
	if (wall.Load(idxD).w > 0.1) D = 0;
	if (wall.Load(idxU).w > 0.1) U = 0;

	outputDivergence[dtid] = (R-L+T-B+U-D) * 0.5;
}

float jacobiAlpha = -1;
float jacobiBeta = 6;

[numthreads(128, 1, 1)]
void csJacobiPressure( uint3 dtid : SV_DispatchThreadID)
{
	uint4 dtl = uint4(dtid, 0);
	uint4 idxL = uint4(max(0, dtl.x - 1), dtl.y, dtl.z, 0);
	uint4 idxR = uint4(min(gridSize.x - 1, dtl.x + 1), dtl.y, dtl.z, 0);
	uint4 idxB = uint4(dtl.x, max(0, dtl.y - 1), dtl.z, 0);
	uint4 idxT = uint4(dtl.x, min(gridSize.y - 1, dtl.y + 1), dtl.z, 0);
	uint4 idxD = uint4(dtl.x, dtl.y, max(0, dtl.z - 1), 0);
	uint4 idxU = uint4(dtl.x, dtl.y, min(gridSize.z - 1, dtl.z + 1), 0);

	float L = pressure.Load(idxL);
	float R = pressure.Load(idxR);
	float B = pressure.Load(idxB);
	float T = pressure.Load(idxT);
	float D = pressure.Load(idxD);
	float U = pressure.Load(idxU);

	float C = pressure.Load(dtl);

	if (wall.Load(idxL).w > 0.1) L = C;
	if (wall.Load(idxR).w > 0.1) R = C;
	if (wall.Load(idxB).w > 0.1) B = C;
	if (wall.Load(idxT).w > 0.1) T = C;
	if (wall.Load(idxD).w > 0.1) D = C;
	if (wall.Load(idxU).w > 0.1) U = C;



	outputPressure[dtid] = (L + R + B + T + U + D + jacobiAlpha*divergence.Load(dtl).x) / jacobiBeta;
}

[numthreads(128, 1, 1)]
void csProject( uint3 dtid : SV_DispatchThreadID)
{
	uint4 dtl = uint4(dtid, 0);
	uint4 idxL = uint4(max(0, dtl.x - 1), dtl.y, dtl.z, 0);
	uint4 idxR = uint4(min(gridSize.x - 1, dtl.x + 1), dtl.y, dtl.z, 0);
	uint4 idxB = uint4(dtl.x, max(0, dtl.y - 1), dtl.z, 0);
	uint4 idxT = uint4(dtl.x, min(gridSize.y - 1, dtl.y + 1), dtl.z, 0);
	uint4 idxD = uint4(dtl.x, dtl.y, max(0, dtl.z - 1), 0);
	uint4 idxU = uint4(dtl.x, dtl.y, min(gridSize.z - 1, dtl.z + 1), 0);

	float L = pressure.Load(idxL);
	float R = pressure.Load(idxR);
	float B = pressure.Load(idxB);
	float T = pressure.Load(idxT);
	float D = pressure.Load(idxD);
	float U = pressure.Load(idxU);

	float C = pressure.Load(dtl);
	float3 mask = float3(1, 1, 1);

	if (wall.Load(idxL).w > 0.1) {
		L = C;
		mask.x = -1;
	}
	if (wall.Load(idxR).w > 0.1) {
		R = C;
		mask.x = -1;
	}
	if (wall.Load(idxB).w > 0.1) {
		B = C;
		mask.y = -1;
	}
	if (wall.Load(idxT).w > 0.1) {
		T = C;
		mask.y = -1;
	}
	if (wall.Load(idxD).w > 0.1) {
		D = C;
		mask.z = -1;
	}
	if (wall.Load(idxU).w > 0.1) {
		U = C;
		mask.z = -1;
	}

	outputVelocity[dtid] = (velocity.Load(dtl) - float3(R-L,T-B,U-D)*0.5f)*mask;
	if (length(density.Load(dtl)) < 0.00001f){
		outputDensity[dtid] = pressure.Load(dtl);
	}
	else {
		outputDensity[dtid] = density.Load(dtl) + density.Load(dtl) * pressure.Load(dtl);
	}
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
	pass wall
	{
		SetComputeShader(CompileShader(cs_5_0, csWall()));
	}
	pass velocyfield
	{
		SetComputeShader(CompileShader(cs_5_0, csVelocyField()));
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