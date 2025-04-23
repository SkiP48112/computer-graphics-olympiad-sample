using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Code.RendererFeatures.CelShading
{
    public class CelShadingFeature : ScriptableRendererFeature
    {
        [SerializeField] private CelShadingSettings _celShadingSettings;
        
        private CelShadingPass _celShadingPass;

        public override void Create()
        {
            if (_celShadingPass != null)
            {
                return;
            }
             
            _celShadingPass = new CelShadingPass(_celShadingSettings)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques
            };
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            
            _celShadingPass = null;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (_celShadingSettings.Material == null)
            {
                Debug.LogWarning("Cell Shading material not set!");
                return;
            }
        
            renderer.EnqueuePass(_celShadingPass);
        }
    }
}