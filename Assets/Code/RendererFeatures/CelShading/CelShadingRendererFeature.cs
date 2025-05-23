using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Code.RendererFeatures.CelShading
{
    public class CelShadingFeature : ScriptableRendererFeature
    {
        [SerializeField] private CelShadingSettings _settings;
        
        private CelShadingPass _pass;

        public override void Create()
        {
            _pass = new CelShadingPass(_settings)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques
            };
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            
            _pass = null;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (_settings.Material == null)
            {
                Debug.LogWarning("Cell Shading material not set!");
                return;
            }
        
            renderer.EnqueuePass(_pass);
        }
    }
}