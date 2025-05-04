#ifndef CEL_SHADING_PASS_INCLUDED
#define CEL_SHADING_PASS_INCLUDED

#include "CelShadingData.hlsl"
#include "CelShadingInput.hlsl"


TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);


CBUFFER_START(UnityPerMaterial)
    int _ColorLevels;
CBUFFER_END


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
    float3 hsv = RgbToHsv(color.rgb);
    
    hsv.z = floor(hsv.z * _ColorLevels) / _ColorLevels;
    color.rgb = HsvToRgb(hsv);

    return color;
}


#endif