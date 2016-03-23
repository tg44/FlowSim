Texture2D<float4> velocity;
Texture2D<float4> density;
Texture2D<float> pressure;
Texture2D<float> divergence;
Texture2D<float> temperature;
Texture2D<float> statictemp;
Texture2D<float4> wall;
Texture2D<float4> velocyfield;


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
	borderColor = float4(0, 0, 0, 0);
};

SamplerState clampSampler
{
	filter = MIN_MAG_MIP_LINEAR;
	addressU = Clamp;
	addressV = Clamp;
};



RWTexture2D<float4> outputVelocity;
RWTexture2D<float4> outputDensity;
RWTexture2D<float> outputPressure;
RWTexture2D<float> outputDivergence;
RWTexture2D<float> outputTemperature;



float dt = 1.0 / 120.0;
float2 gridSize = float2(400, 400);
float2 inputTextureSize = float2(400, 400);

float3 injectionPos = float3(0.5, 0.5, 0.5);
float3 injectionIntensity = float3(0.005, 0.005, 0.005);

float dissipate = 1.00f;
float decay = 0.0f;

float ambientTemperature = 0.7f;

float2 normalize2(float2 input) {
	if (length(input) > 0.0f) {
		return normalize(input);
	}
	return float2(0, 0);
}


[numthreads(128, 1, 1)]
void csAdvect(uint2 dtid : SV_DispatchThreadID)
{
	//read velocity value from velocity field
	//determine backward vector
	float3 s = velocity.Load(uint4(dtid,0, 0)) * dt;

	float2 xds = ((float2)dtid + float2(0.5, 0.5)) / (float2)gridSize - s;
	if (any(xds < 0 || xds > 1)) {
		outputVelocity[dtid] = float4(s,0);
	}
	else {
		outputVelocity[dtid] = velocity.SampleLevel(clampSampler, xds, 0)
			//+  float3(0, 0.01, 0);
			//		+ normalize(xds - injectionPos) * dt * injectionIntensity * 10.01/(length(xds - injectionPos)+0.1)
			//		+ cross(normalize(xds - injectionPos),float3(0,0,1)) * dt * injectionIntensity * 100/(length(xds - injectionPos)+0.1)
			//		+ float3(0,10.1,0) * dt * injectionIntensity * multip / (length(xds - injectionPos)+0.1)
			;
	}
	outputDensity[dtid] = density.SampleLevel(zeroBoundarySampler, xds, 0)
		//+ 1/(length(xds - injectionPos)+0.0001) * dt * injectionIntensity
		;
	float statict = statictemp.SampleLevel(zeroBoundarySampler, xds, 0);
	if (statict < 0)statict = 0;
	outputTemperature[dtid] = max(0, temperature.SampleLevel(zeroBoundarySampler, xds, 0) * dissipate - decay) + max(0,(statict - ambientTemperature));
		;/*
		 if (outputTemperature[dtid] == 0){
		 outputTemperature[dtid] = temperature[dtid];
		 }*/
}

float buoyancy = 0.3f;
float weight = 0.1f;
float2 up = float2(0.0f, 1.0f);

[numthreads(128, 1, 1)]
void csBouyancy(uint2 dtid : SV_DispatchThreadID)
{

	uint4 dtl = uint4(dtid,0, 0);

	float T = temperature.Load(dtl).x;
	float D = density.Load(dtl).x;
	float2 V = velocity.Load(dtl).xy;

	if (T > ambientTemperature){
	  V += (dt * (T - ambientTemperature) * buoyancy - D * weight) * up.xy;
	}

	outputVelocity[dtid] = float4(V,0,0);
}

float energyloss = 0.001f;
[numthreads(128, 1, 1)]
void csWall(uint2 dtid : SV_DispatchThreadID)
{
	//is there a wall?
	uint4 dtl = uint4(dtid,0, 0);
	float4 w = wall.Load(dtl).xyzw;
	float2 v = velocity.Load(dtl).xy;

	if (w.w > 0.1f) {
		float2 norm = normalize2(w.xy);
		float2 dir = normalize2(v.xy);
		float costetha = dot(dir, norm);
		if (costetha > 0)
			costetha *= -1;
		outputVelocity[dtid] = float4(normalize2((dir + norm * 2 * costetha))*length(v.xy)*(1 - energyloss),0,0);
	}
	else {
		outputVelocity[dtid] = float4(v,0,0);
	}


}

float velocyFieldScale = 1.0f;
[numthreads(128, 1, 1)]
void csVelocyField(uint2 dtid : SV_DispatchThreadID)
{
	//is there a field?
	uint4 dtl = uint4(dtid,0, 0);
	float4 vf = velocyfield.Load(dtl).xyzw;
	vf.y = -vf.y;
	float3 v = velocity.Load(dtl).xyz;

	outputVelocity[dtid] = float4(v.xy + normalize2(vf.xy)*vf.z*velocyFieldScale,v.z,0);
	

}


[numthreads(128, 1, 1)]
void csDivergence(uint2 dtid : SV_DispatchThreadID)
{
	uint4 dtl = uint4(dtid,0, 0);
	uint4 idxL = uint4(max(0, dtl.x - 1), dtl.y, 0, 0);
	uint4 idxR = uint4(min(gridSize.x - 1, dtl.x + 1), dtl.y, 0, 0);
	uint4 idxB = uint4(dtl.x, max(0, dtl.y - 1), 0, 0);
	uint4 idxT = uint4(dtl.x, min(gridSize.y - 1, dtl.y + 1), 0, 0);

	float L = velocity.Load(idxL).x;
	float R = velocity.Load(idxR).x;
	float B = velocity.Load(idxB).y;
	float T = velocity.Load(idxT).y;

	// Set velocities to 0 for solid cells:
	if (wall.Load(idxL).w > 0.1) L = 0;
	if (wall.Load(idxR).w > 0.1) R = 0;
	if (wall.Load(idxB).w > 0.1) B = 0;
	if (wall.Load(idxT).w > 0.1) T = 0;

	outputDivergence[dtid] = (R - L + T - B) * 0.5;
}

float jacobiAlpha = -1;
float jacobiBeta = 6;

[numthreads(128, 1, 1)]
void csJacobiPressure(uint2 dtid : SV_DispatchThreadID)
{
	uint4 dtl = uint4(dtid,0, 0);
	uint4 idxL = uint4(max(0, dtl.x - 1), dtl.y, dtl.z, 0);
	uint4 idxR = uint4(min(gridSize.x - 1, dtl.x + 1), dtl.y, dtl.z, 0);
	uint4 idxB = uint4(dtl.x, max(0, dtl.y - 1), dtl.z, 0);
	uint4 idxT = uint4(dtl.x, min(gridSize.y - 1, dtl.y + 1), dtl.z, 0);
	

	float L = pressure.Load(idxL);
	float R = pressure.Load(idxR);
	float B = pressure.Load(idxB);
	float T = pressure.Load(idxT);

	float C = pressure.Load(dtl);

	if (wall.Load(idxL).w > 0.1) L = C;
	if (wall.Load(idxR).w > 0.1) R = C;
	if (wall.Load(idxB).w > 0.1) B = C;
	if (wall.Load(idxT).w > 0.1) T = C;

	outputPressure[dtid] = (L + R + B + T + jacobiAlpha*divergence.Load(dtl).x) / jacobiBeta;
}

[numthreads(128, 1, 1)]
void csProject(uint2 dtid : SV_DispatchThreadID)
{
	uint4 dtl = uint4(dtid,0, 0);
	uint4 idxL = uint4(max(0, dtl.x - 1), dtl.y, dtl.z, 0);
	uint4 idxR = uint4(min(gridSize.x - 1, dtl.x + 1), dtl.y, dtl.z, 0);
	uint4 idxB = uint4(dtl.x, max(0, dtl.y - 1), dtl.z, 0);
	uint4 idxT = uint4(dtl.x, min(gridSize.y - 1, dtl.y + 1), dtl.z, 0);

	float L = pressure.Load(idxL);
	float R = pressure.Load(idxR);
	float B = pressure.Load(idxB);
	float T = pressure.Load(idxT);

	float C = pressure.Load(dtl);
	float3 mask = float3(1, 1, 1);

	if (wall.Load(idxL).w > 0.1f) {
		L = C;
		mask.x = -1;
	}
	if (wall.Load(idxR).w > 0.1f) {
		R = C;
		mask.x = -1;
	}
	if (wall.Load(idxB).w > 0.1f) {
		B = C;
		mask.y = -1;
	}
	if (wall.Load(idxT).w > 0.1f) {
		T = C;
		mask.y = -1;
	}

	outputVelocity[dtid] = float4((velocity.Load(dtl) - float3(R - L, T - B, 0)*0.5f)*mask,0);
	if (length(density.Load(dtl)) < 0.00001f) {
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
	float3 r = 0;
	for (int u = 0; u<32; u++)
		r += density.SampleLevel(linearSampler, float3(input.tex, 0.5), u / 32.0).xyzz;
	return r.xyzz / 32.0;
	////	return density.SampleLevel(linearSampler, float3(input.tex, 0.5), 0).xxxx * float4(10, -1, -0.1, 0);
	return velocity.SampleLevel(linearSampler, float3(input.tex, 0.5), 0).xyzz;
};


///////////////////////////////////////////
float4x4 WorldViewProj;

float2 Heatmap=float2(0,1);
float2 Sensitivity=float2(0,1);

struct VertexShaderInput
{
	float4 Position : POSITION;
	float2 texC		: TEXCOORD;
};

struct VertexShaderOutput
{
	float4 Position		: SV_POSITION;
	float2 texC			: TEXCOORD0;
	float4 pos			: TEXCOORD1;
};

VertexShaderOutput vs2D(VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.texC = input.texC;
	input.Position.w = 1.0f;
	output.Position = mul(input.Position /* * ScaleFactor*/, WorldViewProj);
	output.pos = output.Position;

	return output;
}

float4 ps2D(VertexShaderOutput input) : SV_TARGET
{
	float press = divergence.Sample(zeroBoundarySampler, float2(1-input.texC.x,input.texC.y));
	float temp = temperature.Sample(zeroBoundarySampler, float2(1-input.texC.x, input.texC.y));
	if (temp < 0)temp = 0;
	float4 src = 0;
	
	if (temp < (Heatmap.x - Heatmap.y) / 2.0f) {
		src = float4(0.0f, (temp - Heatmap.y) / (Heatmap.x - Heatmap.y)*2.0f, 1.0f - (temp - Heatmap.y) / (Heatmap.x - Heatmap.y)*2.0f, (press - Sensitivity.y) / (Sensitivity.x - Sensitivity.y));
	}
	else {
		src = float4(((temp - Heatmap.y) / (Heatmap.x - Heatmap.y) - 0.5f)*2.0f, 2.0f - (temp - Heatmap.y) / (Heatmap.x - Heatmap.y)*2.0f, 0.0f, (press - Sensitivity.y) / (Sensitivity.x - Sensitivity.y));
	}
	

	//src = float4(press,0,0.5f, press);
	//src = float4(1, 0, 0, 1);
	return src;
}

//////////////////////////////////////////////////////////



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
		SetComputeShader(CompileShader(cs_5_0, csAdvect()));
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
		SetComputeShader(CompileShader(cs_5_0, csDivergence()));
	}
	pass jacobiPressure
	{
		SetComputeShader(CompileShader(cs_5_0, csJacobiPressure()));
	}
	pass project
	{
		SetComputeShader(CompileShader(cs_5_0, csProject()));
	}
	pass rayMarching
	{
		SetVertexShader(CompileShader(vs_4_0, vsQuad()));
		SetGeometryShader(NULL);
		SetRasterizerState(defaultRasterizer);
		SetPixelShader(CompileShader(ps_4_0, psRayMarching()));
		SetDepthStencilState(defaultCompositor, 0);
		SetBlendState(defaultBlender, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xffffffff);
	}

	pass visualize
	{
		SetVertexShader(CompileShader(vs_4_0, vs2D()));
		SetPixelShader(CompileShader(ps_4_0, ps2D()));
	}
}