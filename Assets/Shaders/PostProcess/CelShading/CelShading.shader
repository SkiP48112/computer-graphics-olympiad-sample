Shader "PostProcess/CelShading"
{
	Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorLevels ("Color Levels", Int) = 8
    }

	HLSLINCLUDE
		#pragma exclude_renderers gles
		#include "CelShadingPass.hlsl"
	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		
		Pass
		{
			Name "Cel Shading Pass"

			HLSLPROGRAM
				#pragma vertex Vertex
				#pragma fragment Fragment
				#pragma target 4.5
			ENDHLSL
		}
	}
}