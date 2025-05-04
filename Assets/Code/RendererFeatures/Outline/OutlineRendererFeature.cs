using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Code.RendererFeatures.Outline
{
    public class OutlineRendererFeature : ScriptableRendererFeature
    {
        [SerializeField] private OutlineSettings _settings;
        
        private OutlinePass _pass;

        public override void Create()
        {
            _pass = new OutlinePass(_settings)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents
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
                Debug.LogWarning("Outline material not set!");
                return;
            }
        
            renderer.EnqueuePass(_pass);
        }
    }
}