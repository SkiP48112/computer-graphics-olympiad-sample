Shader "Olympiad/Sample"
{
	Properties
	{
		_BaseColor ("Base Color", Color) = (1, 1, 1, 1)
		
		[NoScaleOffset]_MainTex ("Texture", 2D) = "white" { }
		_RoughnessMap ("Roughness Map", 2D) = "white" { }
		_MetallicMap ("Metallic Map", 2D) = "white" { }
		_NormalMap ("Normal Map", 2D) = "bump" { }
		
		_RoughnessFactor ("Roughness Factor", Range(0, 1)) = 0.5
		_MetallicFactor ("Metallic Factor", Range(0, 1)) = 0.0
	}

	HLSLINCLUDE
		#pragma exclude_renderers gles

		#include "SamplePass.hlsl"
	ENDHLSL

	SubShader
	{
		Tags { "RenderType" = "Opaque" "RenderPipeline"="UniversalPipeline" }

		Pass
		{
			Name "PBR Sample Shading Pass"

			HLSLPROGRAM
				#pragma vertex Vertex
				#pragma fragment Fragment
				#pragma target 4.5
			ENDHLSL
		}
	}
}