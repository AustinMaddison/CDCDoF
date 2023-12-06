using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CscLensBlurRenderPass : ScriptableRenderPass
{
    private Material material;
    private CscLensBlurBlurSettings blurSettings;

    private RenderTargetIdentifier source;
    private RenderTargetIdentifier blurTex;
    private RenderTargetIdentifier valRTex;
    private RenderTargetIdentifier valGTex;
    private RenderTargetIdentifier valBTex;
    private RenderTargetIdentifier weightTex;

    readonly int valRTexID = Shader.PropertyToID("_BufferValR");
    readonly int valGTexID = Shader.PropertyToID("_BufferValG");
    readonly int valBTexID = Shader.PropertyToID("_BufferValB");
    readonly int weightTexID = Shader.PropertyToID("_BufferWeight");

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

        cmd.GetTemporaryRT(valRTexID, descriptor, FilterMode.Bilinear);
        cmd.GetTemporaryRT(valGTexID, descriptor, FilterMode.Bilinear);
        cmd.GetTemporaryRT(valBTexID, descriptor, FilterMode.Bilinear);
        cmd.GetTemporaryRT(weightTexID, descriptor, FilterMode.Bilinear);

        valRTex = new RenderTargetIdentifier(valRTexID);
        valGTex = new RenderTargetIdentifier(valGTexID);
        valBTex = new RenderTargetIdentifier(valBTexID);
        weightTex = new RenderTargetIdentifier(weightTexID);
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

        // Blit source to intermediate buffer
        cmd.Blit(source, valRTex, material, 0);
        cmd.Blit(source, valGTex, material, 0);
        cmd.Blit(source, valBTex, material, 0);
        cmd.Blit(source, weightTex, material, 0);

        // Apply blur separately for each channel
        cmd.Blit(valRTex, source, material, 1);
        cmd.Blit(valGTex, source, material, 1);
        cmd.Blit(valBTex, source, material, 1);
        cmd.Blit(weightTex, source, material, 1);

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(valRTexID);
        cmd.ReleaseTemporaryRT(valGTexID);
        cmd.ReleaseTemporaryRT(valBTexID);
        cmd.ReleaseTemporaryRT(weightTexID);
        base.FrameCleanup(cmd);
    }
}
