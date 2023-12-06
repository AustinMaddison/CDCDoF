using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CscLensBlurRenderPass : ScriptableRenderPass
{
    private Material material;
    private CscLensBlurBlurSettings blurSettings;

    private RenderTargetIdentifier source;
    private RenderTargetIdentifier blurTex;
    readonly int blurTexID = Shader.PropertyToID("_BlurTex");

    public CscLensBlurRenderPass()
    {
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        descriptor.depthBufferBits = 0;

        var renderer = renderingData.cameraData.renderer;
        source = renderer.cameraColorTargetHandle;

        cmd.GetTemporaryRT(blurTexID, descriptor, FilterMode.Bilinear);
        blurTex = new RenderTargetIdentifier(blurTexID);
    }

    public bool Setup(ScriptableRenderer renderer)
    {
        source = renderer.cameraColorTargetHandle;
        blurSettings = VolumeManager.instance.stack.GetComponent<CscLensBlurBlurSettings>();

        if (blurSettings != null && blurSettings.IsActive())
        {
            material = new Material(Shader.Find("PostProcessing/CscLensBlur"));
            return true;
        }

        return false;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (blurSettings == null || !blurSettings.IsActive())
        {
            return;
        }

        CommandBuffer cmd = CommandBufferPool.Get("Csc Blur Blur Post Process");

        int gridSize = Mathf.CeilToInt(blurSettings.strength.value * 5.0f);

        if(gridSize % 2 == 0)
        {
            gridSize++;
        }

        material.SetInteger("_KernelSize", gridSize);
        material.SetFloat("_Spread", blurSettings.strength.value);

        cmd.Blit( source, blurTex, material, 0);
        cmd.Blit(blurTex, source, material, 1);

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(blurTexID);
        base.FrameCleanup(cmd);
    }
}
