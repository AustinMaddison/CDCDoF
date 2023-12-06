using UnityEngine.Rendering.Universal;

public class CscLensBlurRendererFeature : ScriptableRendererFeature
{
    CscLensBlurRenderPass cscLensBlurRenderPass;

    public override void Create()
    {
        cscLensBlurRenderPass = new CscLensBlurRenderPass();
        name = "CscLensBlur";
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if(cscLensBlurRenderPass.Setup(renderer))
        {
            renderer.EnqueuePass(cscLensBlurRenderPass);
        }
    }
}
