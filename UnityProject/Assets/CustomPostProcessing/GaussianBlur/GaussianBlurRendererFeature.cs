using UnityEngine.Rendering.Universal;

public class GaussianBlurRendererFeature : ScriptableRendererFeature
{
    GaussianBlurRenderPass GaussianBlurRenderPass;

    public override void Create()
    {
        GaussianBlurRenderPass = new GaussianBlurRenderPass();
        name = "GaussianBlur";
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if(GaussianBlurRenderPass.Setup(renderer))
        {
            renderer.EnqueuePass(GaussianBlurRenderPass);
        }
    }
}
