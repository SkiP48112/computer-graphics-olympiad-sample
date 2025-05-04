#ifndef OUTLINE_PASS_INCLUDED
#define OUTLINE_PASS_INCLUDED

#include "OutlineData.hlsl"
#include "OutlineInput.hlsl"

#define SUM_DEPTH_3X3 9


TEXTURE2D(_MainTex);
TEXTURE2D(_CameraDepthTexture);

SAMPLER(sampler_MainTex);
SAMPLER(sampler_CameraDepthTexture);


CBUFFER_START(UnityPerMaterial)
    float _OutlineThickness;
    float _DepthThreshold;
    float4 _OutlineColor;
CBUFFER_END


float SampleDepth(float2 uv)
{
    return Linear01Depth(SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r, _ZBufferParams);
}


VertexOutput Vertex(VertexInput input)
{
    VertexOutput output;
    
    output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
    output.uv = input.uv;
    
    return output;
}


half4 Fragment(VertexOutput input) : SV_Target
{
    half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
    float2 texelSize = _OutlineThickness / _ScreenParams.xy;
    
    float depth = SampleDepth(input.uv);
    float depthDiff = 0;

    UNITY_UNROLL
    for(int x = -1; x <= 1; x++)
    {
        UNITY_UNROLL
        for(int y = -1; y <= 1; y++)
        {
            float2 offset = float2(x,y) * texelSize;
            depthDiff += SampleDepth(input.uv + offset);
        }
    }

    depthDiff = abs(depth * SUM_DEPTH_3X3 - depthDiff) / SUM_DEPTH_3X3;
    if(depthDiff > _DepthThreshold)
    {
        color.rgb = _OutlineColor.rgb;
    }

    return color;
}


#endif