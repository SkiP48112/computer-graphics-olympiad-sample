using UnityEngine;

namespace Code.RendererFeatures.Outline
{
    [System.Serializable]
    public class OutlineSettings
    {
        public Material Material;
        public Color OutlineColor = Color.black;
        
        [Range(0.0f, 5.0f)] public float OutlineThickness = 1.0f;
        [Range(0.0f, 1.0f)] public float DepthThreshold = 0.00001f;
    }
}
