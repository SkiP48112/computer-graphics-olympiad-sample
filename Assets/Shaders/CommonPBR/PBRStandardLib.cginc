#ifndef PBR_STANDARD_LIBRARY_INCLUDED
#define PBR_STANDARD_LIBRARY_INCLUDED

#define PI 3.14159265358979323846
#define DEFAULT_SURFACE_REFLECTIVITY 0.04

float DistributionGGX(float3 normal, float3 halfwayVector, float roughness)
{
    float a = roughness * roughness;
    float a2 = a * a;

    float NdotH = max(dot(normal, halfwayVector), 0.0);
    float NdotH2 = NdotH * NdotH;
    
    float denom = NdotH2 * (a2 - 1.0) + 1.0;
    denom = PI * denom * denom;
    
    return a2 / denom;
}


float GeometrySchlickGGX(float NdotV, float roughness)
{
    float r = roughness + 1.0;
    float k = r * r / 8.0;

    float num = NdotV;
    float denom = NdotV * (1.0 - k) + k;
    
    return num / denom;
}


float GeometrySmith(float3 normal, float3 viewDirection, float3 lightDir, float roughness)
{
    float NdotV = max(dot(normal, viewDirection), 0.0);
    float NdotL = max(dot(normal, lightDir), 0.0);

    float ggxL = GeometrySchlickGGX(NdotL, roughness);
    float ggxV = GeometrySchlickGGX(NdotV, roughness);
    
    return ggxL * ggxV;
}


float3 FresnelSchlick(float cosTheta, float3 surfaceReflectivity)
{
    return surfaceReflectivity + (1.0 - surfaceReflectivity) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}

#endif