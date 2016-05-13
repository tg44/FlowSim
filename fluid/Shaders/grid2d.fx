Texture2D<float4> velocity;
Texture2D<float> density;
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
RWTexture2D<float> outputDensity;
RWTexture2D<float> outputPressure;
RWTexture2D<float> outputDivergence;
RWTexture2D<float> outputTemperature;



float dt = 1.0 / 1.0;
float2 gridSize = float2(512, 512);
float2 inputTextureSize = float2(512, 512);

float dissipate = 1.00f;
float decay = 0.0f;

float ambientTemperature = 0.0f;

float2 normalize2(float2 input) {
	if (length(input) > 0.0f) {
		return normalize(input);
	}
	return float2(0, 0);
}


[numthreads(128, 1, 1)]
void csAdvect(uint2 dtid : SV_DispatchThreadID)
{
	uint4 dtl = uint4(dtid, 0, 0);
	float2 trans = float2(1, 1);
	float2 u = velocity.Load(dtl);
	float2 add = float2(0.5f, 0.5f);
	float2 inverseSize = float2(1.0f / (gridSize.x), 1.0f / (gridSize.y));
	float2 coord = (((float2)dtid+add) - u*dt*trans) * inverseSize;
	float2 actualPos = (((float2)dtid + add)) * inverseSize;
	//float2 coord = (((float2)dtid+add)) * inverseSize;
	//float2 coord = float2((dtid.x)*inverseSize.x, (dtid.y)*inverseSize.y);

	float w = wall.Load(dtl).w;
	
	float statict = statictemp.Load(dtl).x;
	if (statict < 0)statict = 0;

	float2 step = normalize(coord-actualPos)*inverseSize;
	float2 pos = actualPos;
	bool isThereWall = w > 0.1f;
	/*[loop]
	for (int i = 0; i < length(u); i++)
	{
		isThereWall = wall.SampleLevel(zeroBoundarySampler, pos, 0).w > 0.1f;
		if (isThereWall) {
			break;
		}
		pos += step;
	}*/



	float t = temperature.SampleLevel(zeroBoundarySampler, actualPos, 0).x;
	
	float d = 0.0f;
	float4 v = float4(0, 0, 0, 0);


	if (isThereWall) {
		//v = velocity.SampleLevel(zeroBoundarySampler, actualPos, 0);
		//d = density.SampleLevel(zeroBoundarySampler, actualPos, 0).x;
	}
	else if(w<0.1f) {
		v = velocity.SampleLevel(zeroBoundarySampler, coord, 0);
		d = density.SampleLevel(zeroBoundarySampler, coord, 0).x;
		t = temperature.SampleLevel(zeroBoundarySampler, coord, 0).x;
	}
	
	outputVelocity[dtid] = v;
	outputDensity[dtid] = d;
	outputTemperature[dtid] = min(statict + t, 1);
	
}

float buoyancy = 2.0f;
float weight = 0.005f;
float2 up = float2(0.0f, -1.0f);

[numthreads(128, 1, 1)]
void csBouyancy(uint2 dtid : SV_DispatchThreadID)
{

	uint4 dtl = uint4(dtid,0, 0);

	float T = temperature.Load(dtl).x;
	float2 V = velocity.Load(dtl).xy;

	if (T > ambientTemperature){
	  V = V + (dt * (T - ambientTemperature) * buoyancy /*- D * weight*/) * up.xy;
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

	outputVelocity[dtid] = float4(v.xy + normalize2(vf.xy)*vf.z*velocyFieldScale,0,0);
	

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
float jacobiBeta = 4;

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

	float d = divergence.Load(dtl).x;

	outputPressure[dtid] = (L + R + B + T + jacobiAlpha * d) / jacobiBeta;
}


float temperatureDisciplate = 0.000000f;
float temperatureDiffusivity = 4.0f;

[numthreads(128, 1, 1)]
void csTermoTransfer(uint2 dtid : SV_DispatchThreadID) {
	uint4 dtl = uint4(dtid, 0, 0);
	uint4 idxL = uint4(max(0, dtl.x - 1), dtl.y, 0, 0);
	uint4 idxR = uint4(min(gridSize.x - 1, dtl.x + 1), dtl.y, 0, 0);
	uint4 idxB = uint4(dtl.x, max(0, dtl.y - 1), 0, 0);
	uint4 idxT = uint4(dtl.x, min(gridSize.y - 1, dtl.y + 1), 0, 0);

	float L = temperature.Load(idxL).x / 2;
	float R = temperature.Load(idxR).x / 2;
	float B = temperature.Load(idxB).x / 2;
	float T = temperature.Load(idxT).x / 2;

	float C = temperature.Load(dtl).x / 2;

	float Lm = (C - L)*(C - L)* (C > L ? -1 : 1);
	float Rm = (C - R)*(C - R)* (C > R ? -1 : 1);
	float Bm = (C - B)*(C - B)* (C > B ? -1 : 1);
	float Tm = (C - T)*(C - T)* (C > T ? -1 : 1);

	outputTemperature[dtid] = max(0,((Lm + Rm + Bm + Tm) / temperatureDiffusivity * dt + C * 2)*(1 - temperatureDisciplate*dt));
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

	outputVelocity[dtid] = float4((velocity.Load(dtl) - float3(R - L, (T - B)*1, 0)*1),0);
	/*if (length(density.Load(dtl)) < 0.00001f) {
		outputDensity[dtid] = pressure.Load(dtl);
	}
	else {
		outputDensity[dtid] = density.Load(dtl) + density.Load(dtl) * pressure.Load(dtl);
	}*/
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
	float r = 0;
	for (int u = 0; u<32; u++)
		r += density.SampleLevel(linearSampler, float3(input.tex, 0.5), u / 32.0).x;
	return r.xxxx / 32.0;
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
	float press = density.Sample(zeroBoundarySampler, float2(1-input.texC.x,input.texC.y));
	float temp = temperature.Sample(zeroBoundarySampler, float2(1-input.texC.x, input.texC.y));
	if (temp < 0)temp = 0;
	float4 src = 0;
	
	if (temp < (Heatmap.x + Heatmap.y) / 2.0f) {
		src = float4(0.0f, (temp - Heatmap.y) / (Heatmap.x - Heatmap.y), 1.0f - (temp - Heatmap.y) / (Heatmap.x - Heatmap.y), (press - Sensitivity.y) / (Sensitivity.x - Sensitivity.y));
	}
	else {
		src = float4(((temp - Heatmap.y) / (Heatmap.x - Heatmap.y)),1.0f - (temp - Heatmap.y) / (Heatmap.x - Heatmap.y), 0.0f, (press - Sensitivity.y) / (Sensitivity.x - Sensitivity.y));
	}
	

	//src = float4(press,0,0.5f, press);
	//src = float4(1, 0, 0, 1);
	return src;
}

float4 ps2Dvel(VertexShaderOutput input) : SV_TARGET
{
float2 vel = velocity.Sample(zeroBoundarySampler, float2(1 - input.texC.x,input.texC.y)).xy;
float4 src = 0;

src = float4((vel - Sensitivity.y) / (Sensitivity.x - Sensitivity.y),0,0);


//src = float4(press,0,0.5f, press);
//src = float4(1, 0, 0, 1);
return src;
}

float4 ps2Df1(VertexShaderOutput input) : SV_TARGET
{
	float val = density.Sample(zeroBoundarySampler, float2(1 - input.texC.x,input.texC.y)).x;
	float4 src = 0;

	src = float4((val - Sensitivity.y) / (Sensitivity.x - Sensitivity.y),0,0,0);


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
	pass termotransfer
	{
		SetComputeShader(CompileShader(cs_5_0, csTermoTransfer()));
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

	pass visualizeVel
	{
		SetVertexShader(CompileShader(vs_4_0, vs2D()));
		SetPixelShader(CompileShader(ps_4_0, ps2Dvel()));
	}
	pass visualizeF1
	{
		SetVertexShader(CompileShader(vs_4_0, vs2D()));
		SetPixelShader(CompileShader(ps_4_0, ps2Df1()));
	}
}