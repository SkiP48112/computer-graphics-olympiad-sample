#ifndef OUTLINE_DATA_INCLUDED
#define OUTLINE_DATA_INCLUDED

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