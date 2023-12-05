using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenu("GaussianBlur")]
public class GaussianBlurSettings : VolumeComponent, IPostProcessComponent
{
    [Tooltip("The blur intensity. The distribution of the gaussian function kernel.")]
    public ClampedFloatParameter strength = new ClampedFloatParameter(0.0f, 0.0f, 100.0f);

    public bool IsActive()
    {
        return (strength.value > 0.0f) && active;
    }

    public bool IsTileCompatible()
    {
        return false;
    }
}
