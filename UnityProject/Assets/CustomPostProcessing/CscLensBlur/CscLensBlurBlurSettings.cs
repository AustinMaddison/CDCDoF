using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenu("CscLensBlur")]
public class CscLensBlurBlurSettings : VolumeComponent, IPostProcessComponent
{
    [Tooltip("The blur intensity. The distribution of the precomputed circular real + imaginary function kernel.")]
    public ClampedFloatParameter strength = new ClampedFloatParameter(0.0f, 0.0f, 100.0f);

    [HideInInspector]
    public DepthOfField depthOfField;

    public bool IsActive()
    {
        return (strength.value > 0.0f) && active;
    }

    public bool IsTileCompatible()
    {
        return false;
    }
}

public class CscLensBlurDepthOfFieldControl : MonoBehaviour
{
    private Volume volume;
    private CscLensBlurBlurSettings blurSettings;

    private void Start()
    {
        volume = gameObject.GetComponent<Volume>();
        if (volume == null)
        {
            volume = gameObject.AddComponent<Volume>();
            volume.isGlobal = true;
        }

        // Retrieve or add the CscLensBlurBlurSettings component
        blurSettings = VolumeManager.instance.stack.GetComponent<CscLensBlurBlurSettings>();
        if (blurSettings == null)
        {
            blurSettings = volume.sharedProfile.Add<CscLensBlurBlurSettings>();
        }

        // Retrieve or add the DepthOfField component
        blurSettings.depthOfField = VolumeManager.instance.stack.GetComponent<DepthOfField>();
        if (blurSettings.depthOfField == null)
        {
            blurSettings.depthOfField = volume.sharedProfile.Add<DepthOfField>();
        }
    }

    private void Update()
    {
        blurSettings.depthOfField.aperture.value = MapBlurIntensityToAperture(blurSettings.strength.value);
    }

    private float MapBlurIntensityToAperture(float blurIntensity)
    {
        float minBlurIntensity = 0.0f;
        float maxBlurIntensity = 100.0f;
        float minAperture = 1.0f;
        float maxAperture = 16.0f;

        return (blurIntensity - minBlurIntensity) / (maxBlurIntensity - minBlurIntensity) * (maxAperture - minAperture) + minAperture;
    }
}
