#ifndef SAMPLE_DATA_INCLUDED
#define SAMPLE_DATA_INCLUDED

struct VertexInput
{
	float2 uv		  : TEXCOORD0;
	float3 normalOS	  : NORMAL;
	float4 tangentOS  : TANGENT;
	float4 positionOS : POSITION;
};

struct VertexOutput
{
	float2 uv		   : TEXCOORD0;
	float3 normalWS    : TEXCOORD1;
	float3 tangentWS   : TEXCOORD2;
	float3 bitangentWS : TEXCOORD3;
	float3 positionWS  : TEXCOORD4;
	float4 positionCS  : SV_POSITION;
};

#endif