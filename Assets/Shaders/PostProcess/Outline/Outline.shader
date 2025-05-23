Shader "PostProcess/Outline"
{
	Properties
    {
    	_MainTex ("Texture", 2D) = "white" {}
	    _OutlineThickness ("Outline Thickness", Float) = 1
        _DepthThreshold ("Depth Threshold", Float) = 0.01
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
    }

	HLSLINCLUDE
		#pragma exclude_renderers gles
		#include "OutlinePass.hlsl"
	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		
		Pass
		{
			Name "Outline Pass"

			HLSLPROGRAM
				#pragma vertex Vertex
				#pragma fragment Fragment
				#pragma target 4.5
			ENDHLSL
		}
	}
}