using UnityEngine;

namespace Code.RendererFeatures.CelShading
{
    [System.Serializable]
    public class CelShadingSettings
    {
        public Material Material;
        
        [Range(1, 16)] public int ColorLevels = 8;
    }
}
