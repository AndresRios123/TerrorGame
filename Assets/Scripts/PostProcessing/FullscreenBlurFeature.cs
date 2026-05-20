using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FullscreenBlurFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class BlurSettings
    {
        [Tooltip("Activa o desactiva el efecto de blur")]
        public bool enable = true;

        [Tooltip("Intensidad del blur (0 = nada, 1 = muy borroso)"), Range(0f, 1f)]
        public float intensity = 0.3f;

        [Tooltip("Material que contiene el shader de blur")]
        public Material blurMaterial;
    }

    public BlurSettings settings = new BlurSettings();
    private BlurRenderPass blurPass;

    public override void Create()
    {
        blurPass = new BlurRenderPass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Preview)
            return;

        if (!settings.enable)
            return;

        if (settings.blurMaterial == null)
        {
            Debug.LogWarning("[TetricBlur] Blur Material es null.");
            return;
        }

        renderer.EnqueuePass(blurPass);
    }

    class BlurRenderPass : ScriptableRenderPass
    {
        private BlurSettings settings;
        private RTHandle tempTexture;
        private const string ProfilerTag = "Tetric Blur Pass";

        public BlurRenderPass(BlurSettings settings)
        {
            this.settings = settings;
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref tempTexture, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_TempBlurTexture");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (settings.blurMaterial == null) return;

            CommandBuffer cmd = CommandBufferPool.Get(ProfilerTag);

            RTHandle cameraTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

            settings.blurMaterial.SetFloat("_Intensity", settings.intensity);

            // Blit: camara -> textura temporal (con material/blur)
            Blitter.BlitCameraTexture(cmd, cameraTarget, tempTexture, settings.blurMaterial, 0);
            // Blit: textura temporal -> camara (sin material, solo copia)
            Blitter.BlitCameraTexture(cmd, tempTexture, cameraTarget);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }
}
