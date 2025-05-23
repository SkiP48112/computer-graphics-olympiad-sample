using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Code.RendererFeatures.CelShading
{
    public class CelShadingPass : ScriptableRenderPass
    {
        private readonly int COLOR_LEVELS_PROP = Shader.PropertyToID("_ColorLevels");
        private const string CEL_SHADING_BUFFER_NAME = "Cel Shading Pass";

        private readonly CelShadingSettings _settings;
        
        private RenderTargetIdentifier _source;
        private RenderTargetHandle _tempTexture;

        public CelShadingPass(CelShadingSettings settings)
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
            
            var cmd = CommandBufferPool.Get(CEL_SHADING_BUFFER_NAME);
            
            _settings.Material.SetInt(COLOR_LEVELS_PROP, _settings.ColorLevels);

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
