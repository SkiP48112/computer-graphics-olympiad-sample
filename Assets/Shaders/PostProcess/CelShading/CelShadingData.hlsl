#ifndef CEL_SHADING_DATA_INCLUDED
#define CEL_SHADING_DATA_INCLUDED

struct VertexInput
{
	float2 uv		  : TEXCOORD0;
	float4 positionOS : POSITION;
};

struct VertexOutput
{
	float2 uv		   : TEXCOORD0;
	float4 positionCS  : SV_POSITION;
};

#endif