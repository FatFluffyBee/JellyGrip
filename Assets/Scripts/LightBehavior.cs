using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightBehavior : MonoBehaviour
{
    [SerializeField] private Light2D light2D;
    [SerializeField] private bool activateOnStart;

    [Header("Light Activated")]
    [SerializeField] private float onBaseRadius;
    [SerializeField] private float onOffsetRadius;
    [SerializeField] private float onLightIntensity = 1.1f;

    [Header("Light Deactivated")]
    [SerializeField] private float offBaseRadius;
    [SerializeField] private float offOffsetRadius;
    [SerializeField] private float offLightIntensity = 1.1f;

    [Header("Noise")]
    
    [SerializeField] private float lightSmoothing = 5f;
    [SerializeField] private float firstNoiseSize = 1;
    [SerializeField] private float firstNoiseImpact = 0.7f;
    [SerializeField] private float secondNoiseSize = 3;
    [SerializeField] private float secondNoiseImpact = 0.3f;
    [SerializeField] private float secondNoiseOffset = 1;

    [Header("Testing")]
    [SerializeField] private bool isActive;

    float currentMaxRadius;
    float currentIntensity;

    void Start()
    {
        isActive = activateOnStart;
    }

    void Update()
    {
        float targetRadius = ComputeLightRadius();
        currentMaxRadius = Mathf.Lerp(currentMaxRadius, targetRadius, Time.deltaTime * lightSmoothing);
        
        float targetIntensity = isActive? onLightIntensity : offLightIntensity;
        currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, Time.deltaTime * lightSmoothing);

        ApplyLight(currentMaxRadius, currentIntensity);
    }

    public float ComputeLightRadius()
    {
        float lightVariationFactor = Mathf.PerlinNoise(Time.time * firstNoiseSize, 0f) * firstNoiseImpact 
        + Mathf.PerlinNoise(Time.time * secondNoiseSize, secondNoiseOffset) * secondNoiseImpact; 

        lightVariationFactor = Mathf.SmoothStep(0f, 1f, lightVariationFactor);

        float baseRadius = isActive? onBaseRadius : offBaseRadius;
        float offsetRadius = isActive? onOffsetRadius : offOffsetRadius; 

        return baseRadius + offsetRadius * (lightVariationFactor * 2f - 1);
    }

    private void ApplyLight (float radius, float intensity)
    {
        light2D.pointLightOuterRadius = radius;
        light2D.pointLightInnerRadius = radius / 3f;
        light2D.intensity = intensity;
    }

    public void ActivateLight()
    {
        isActive = true;  
    }

    private void OnValidate()
    {
        if(light2D == null)
            return;

        float radius = isActive? onBaseRadius + onOffsetRadius / 2f : offBaseRadius + offOffsetRadius / 2f;
        float intensity = isActive? onLightIntensity : offLightIntensity;
        
        ApplyLight(radius, intensity);
    }
}
