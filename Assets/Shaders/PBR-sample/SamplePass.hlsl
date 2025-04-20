#ifndef SAMPLE_PASS_INCLUDED
#define SAMPLE_PASS_INCLUDED

#include "SampleData.hlsl"
#include "SampleInput.hlsl"

#define MIN_DENOMINATOR 0.001

TEXTURE2D(_MainTex);
TEXTURE2D(_RoughnessMap);
TEXTURE2D(_MetallicMap);
TEXTURE2D(_NormalMap);

SAMPLER(sampler_MainTex);
SAMPLER(sampler_RoughnessMap);
SAMPLER(sampler_MetallicMap);
SAMPLER(sampler_NormalMap);

CBUFFER_START(UnityPerMaterial)
	float4 _BaseColor;
	float _RoughnessFactor;
	float _MetallicFactor;
CBUFFER_END


float3 CalculateDirectLight(float3 albedo, float metallic, float roughness, float3 viewDirection, float3 normal, float3 surfaceReflectivity)
{
	Light light = GetMainLight();
	float3 lightDirection = light.direction;
	float3 lightColor = light.color * light.distanceAttenuation;

	float3 halfwayVector = normalize(viewDirection + lightDirection);

	float NDF = DistributionGGX(normal, halfwayVector, roughness);
	float geometry = GeometrySmith(normal, viewDirection, lightDirection, roughness);
	float3 fresnel = FresnelSchlick(max(dot(halfwayVector, viewDirection), 0.0), surfaceReflectivity);

	float3 kD = (1.0 - fresnel) * (1.0 - metallic);
	float3 diffuse = kD * albedo / PI;
	float3 specular = NDF * geometry * fresnel / max(4.0 * max(dot(normal, viewDirection), 0.0) * max(dot(normal, lightDirection), 0.0),  MIN_DENOMINATOR);
	
	float NdotL = max(dot(normal, lightDirection), 0.0);
	
	return (diffuse + specular) * lightColor * NdotL;
}

float3 CalculateAmbient(float3 albedo, float3 normal, float3 viewDirection, float roughness, float metallic, float3 surfaceReflectivity)
{
	float3 ambientDiffuse = SampleSH(normal) * albedo;

	float3 reflection = reflect(-viewDirection, normal);
	float mipLevel = roughness * (1.7 - 0.7 * roughness) * UNITY_SPECCUBE_LOD_STEPS;
	
	float4 specularSample = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, reflection, mipLevel);
	float3 ambientSpecular = DecodeHDREnvironment(specularSample, unity_SpecCube0_HDR);

	float3 fresnel = FresnelSchlickRoughness(max(dot(normal, viewDirection), 0.0), surfaceReflectivity, roughness);
	ambientSpecular *= fresnel;

	float3 kD = (1.0 - fresnel) * (1.0 - metallic);
	
	return kD * ambientDiffuse + ambientSpecular;
}

VertexOutput Vertex(VertexInput input)
{
	VertexOutput output;
	
	output.uv = input.uv;
	output.positionCS = TransformObjectToHClip(input.positionOS.xyz);

	output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
	output.normalWS = TransformObjectToWorldNormal(input.normalOS);
	output.tangentWS = TransformObjectToWorldDir(input.tangentOS.xyz);
	output.bitangentWS = cross(output.normalWS, output.tangentWS) * input.tangentOS.w;

	return output;
}

half4 Fragment(VertexOutput input) : SV_Target
{
	float4 diffuseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
	float roughness = SAMPLE_TEXTURE2D(_RoughnessMap, sampler_RoughnessMap, input.uv) * _RoughnessFactor;
	float metallic = SAMPLE_TEXTURE2D(_MetallicMap, sampler_MetallicMap, input.uv) * _MetallicFactor;
	float3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv));

	float3x3 TBN = float3x3(input.tangentWS, input.bitangentWS, input.normalWS);
	float3 normalWS = normalize(mul(normalTS, TBN));

	float3 albedo = diffuseTex.rgb * _BaseColor.rgb;
	float3 viewDirection = GetWorldSpaceViewDir(input.positionWS);
	float3 surfaceReflectivity = lerp(DEFAULT_SURFACE_REFLECTIVITY, albedo, metallic);

	float3 directLight = CalculateDirectLight(albedo, metallic, roughness, viewDirection, normalWS, surfaceReflectivity);
	float3 ambient = CalculateAmbient(albedo, normalWS, viewDirection, roughness, metallic, surfaceReflectivity);
	//float3 ambient = float3(0.03, 0.03, 0.03) * albedo;
	
	return float4(directLight + ambient, 1.0);
}

#endif