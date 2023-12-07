using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class CscLensBlurRenderPass : ScriptableRenderPass
{
    private Material material;
    private CscLensBlurBlurSettings blurSettings;

    public RenderTargetIdentifier source;
    public RenderTargetIdentifier horizontalPassTexR;
    public RenderTargetIdentifier horizontalPassTexG;
    public RenderTargetIdentifier horizontalPassTexB;
    public RenderTargetIdentifier horizontalPassTexW;
    
    readonly int horizontalPassTexRID = Shader.PropertyToID("_TexR");
    readonly int horizontalPassTexGID = Shader.PropertyToID("_TexG");
    readonly int horizontalPassTexBID = Shader.PropertyToID("_TexB");
    readonly int horizontalPassTexWID = Shader.PropertyToID("_TexW");

    public RenderTexture renderTexR;
    public RenderTexture renderTexG;
    public RenderTexture renderTexB;
    public RenderTexture renderTexW;


    public int width;
    public int height;
    
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


        width = renderingData.cameraData.cameraTargetDescriptor.width;
        height = renderingData.cameraData.cameraTargetDescriptor.height;

        cmd.GetTemporaryRT(horizontalPassTexRID, descriptor, FilterMode.Bilinear);
        cmd.GetTemporaryRT(horizontalPassTexGID, descriptor, FilterMode.Bilinear);
        cmd.GetTemporaryRT(horizontalPassTexBID, descriptor, FilterMode.Bilinear);
        cmd.GetTemporaryRT(horizontalPassTexWID, descriptor, FilterMode.Bilinear);

        horizontalPassTexR = new RenderTargetIdentifier(horizontalPassTexRID);
        horizontalPassTexG = new RenderTargetIdentifier(horizontalPassTexGID);
        horizontalPassTexB = new RenderTargetIdentifier(horizontalPassTexBID);
        horizontalPassTexW = new RenderTargetIdentifier(horizontalPassTexWID);

        renderTexR = new RenderTexture(width, height, 0);
        renderTexG = new RenderTexture(width, height, 0);
        renderTexB = new RenderTexture(width, height, 0);
        renderTexW = new RenderTexture(width, height, 0);
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

        material.SetFloat("_Spread", blurSettings.strength.value);

        // Horizontal
        cmd.Blit(source, horizontalPassTexR, material, 0);
        cmd.Blit(source, horizontalPassTexG, material, 1);
        cmd.Blit(source, horizontalPassTexB, material, 2);
        cmd.Blit(source, horizontalPassTexW, material, 3);

        cmd.Blit(horizontalPassTexR, renderTexR);
        cmd.Blit(horizontalPassTexG, renderTexG);
        cmd.Blit(horizontalPassTexB, renderTexB);
        cmd.Blit(horizontalPassTexW, renderTexW);

        material.SetTexture("_TexR", renderTexR);
        material.SetTexture("_TexG", renderTexG);
        material.SetTexture("_TexB", renderTexB);
        material.SetTexture("_TexW", renderTexW);

        cmd.Blit(null, source, material, 4);

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(horizontalPassTexRID);
        cmd.ReleaseTemporaryRT(horizontalPassTexGID);
        cmd.ReleaseTemporaryRT(horizontalPassTexBID);
        cmd.ReleaseTemporaryRT(horizontalPassTexWID);

        base.FrameCleanup(cmd);
    }
}
