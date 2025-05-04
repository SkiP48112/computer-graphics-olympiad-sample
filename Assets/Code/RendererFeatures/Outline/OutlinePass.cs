using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Code.RendererFeatures.Outline
{
    public class OutlinePass : ScriptableRenderPass
    {
        private readonly int OUTLINE_THICKNESS_PROP = Shader.PropertyToID("_OutlineThickness");
        private readonly int DEPTH_THRESHOLD_PROP = Shader.PropertyToID("_DepthThreshold");
        private readonly int OUTLINE_COLOR_PROP = Shader.PropertyToID("_OutlineColor");
        private const string OUTLINE_BUFFER_NAME = "Outline Pass";

        private readonly OutlineSettings _settings;
        
        private RenderTargetIdentifier _source;
        private RenderTargetHandle _tempTexture;

        public OutlinePass(OutlineSettings settings)
        {
            _settings = settings;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            _source = renderingData.cameraData.renderer.cameraColorTarget;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(_tempTexture.id, cameraTextureDescriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_settings.Material == null)
            {
                return;
            }
            
            var cmd = CommandBufferPool.Get(OUTLINE_BUFFER_NAME);
            
            _settings.Material.SetFloat(OUTLINE_THICKNESS_PROP, _settings.OutlineThickness);
            _settings.Material.SetFloat(DEPTH_THRESHOLD_PROP, _settings.DepthThreshold);
            _settings.Material.SetColor(OUTLINE_COLOR_PROP, _settings.OutlineColor);

            Blit(cmd, _source, _tempTexture.Identifier(), _settings.Material);
            Blit(cmd, _tempTexture.Identifier(), _source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(_tempTexture.id);
        }
    }
}
