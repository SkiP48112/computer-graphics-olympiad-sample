using UnityEngine;

namespace Code.RendererFeatures.CelShading
{
    [System.Serializable]
    public class CelShadingSettings
    {
        public Material Material;
        public Color OutlineColor = Color.black;

        [Range(1, 16)] public int ColorLevels = 8;
        [Range(1, 5)] public float OutlineThickness = 2;
        [Range(0.1f, 1.0f)] public float DepthThreshold = 0.1f;
    }
}
