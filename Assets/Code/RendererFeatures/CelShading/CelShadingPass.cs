using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Code.RendererFeatures.CelShading
{
    class CelShadingPass : ScriptableRenderPass
    {
        private readonly int COLOR_LEVELS_PROP = Shader.PropertyToID("_ColorLevels");
        private readonly int OUTLINE_THICKNESS_PROP = Shader.PropertyToID("_OutlineThickness");
        private readonly int DEPTH_THRESHOLD_PROP = Shader.PropertyToID("_DepthThreshold");
        private readonly int OUTLINE_COLOR_PROP = Shader.PropertyToID("_OutlineColor");
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
            ConfigureInput(ScriptableRenderPassInput.Depth);
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
            _settings.Material.SetFloat(OUTLINE_THICKNESS_PROP, _settings.OutlineThickness);
            _settings.Material.SetFloat(DEPTH_THRESHOLD_PROP, _settings.DepthThreshold);
            _settings.Material.SetColor(OUTLINE_COLOR_PROP, _settings.OutlineColor);

            Blit(cmd, _source, _tempTexture.Identifier(), _settings.Material, 0);
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
