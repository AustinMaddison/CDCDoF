using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GaussianBlurRenderPass : ScriptableRenderPass
{
    private Material material;
    private GaussianBlurSettings blurSettings;

    private RenderTargetIdentifier source;
    private RenderTargetIdentifier blurTex;
    readonly int blurTexID = Shader.PropertyToID("_BlurTex");


    public GaussianBlurRenderPass()
    {
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        descriptor.depthBufferBits = 0; // don't need depth

        // Get camera render target (buffer)
        var renderer = renderingData.cameraData.renderer;
        source = renderer.cameraColorTargetHandle;

        // Setup render target (buffer) for output of blur
        cmd.GetTemporaryRT(blurTexID, descriptor, FilterMode.Bilinear);
        blurTex = new RenderTargetIdentifier(blurTexID);
    }

    public bool Setup(ScriptableRenderer renderer)
    {
        //source = renderer.cameraColorTargetHandle;
        blurSettings = VolumeManager.instance.stack.GetComponent<GaussianBlurSettings>();

        if (blurSettings != null && blurSettings.IsActive())
        {
            material = new Material(Shader.Find("PostProcessing/GaussianBlur")); // instance shader
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

        CommandBuffer cmd = CommandBufferPool.Get("Gaussian Blur Post Process");

        int kernelSize = Mathf.CeilToInt(blurSettings.strength.value * blurSettings.precisionFactor.value); // dimension of convolution kernel

        // kernel has to be odd for it to have a center.
        if(kernelSize % 2 == 0)
        {
            kernelSize++;
        }

        // Set the material properties
        material.SetInteger("_KernelSize", kernelSize);
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
