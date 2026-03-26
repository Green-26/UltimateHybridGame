// ============================================
// Ultimate Hybrid Game - Player Controller
// Copyright (c) 2026 Bertin ABIJURU
// All Rights Reserved
// ============================================

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingSetup : MonoBehaviour
{
    [Header("Post Processing Volume")]
    public PostProcessVolume postProcessVolume;
    
    [Header("Effects")]
    public bool enableBloom = true;
    public bool enableMotionBlur = true;
    public bool enableVignette = true;
    public bool enableColorGrading = true;
    public bool enableDepthOfField = false;
    
    [Header("Bloom Settings")]
    [Range(0f, 2f)]
    public float bloomIntensity = 0.5f;
    [Range(0f, 10f)]
    public float bloomThreshold = 0.8f;
    
    [Header("Motion Blur")]
    [Range(0f, 1f)]
    public float motionBlurAmount = 0.3f;
    
    [Header("Vignette")]
    [Range(0f, 1f)]
    public float vignetteIntensity = 0.3f;
    public Color vignetteColor = Color.black;
    
    [Header("Color Grading")]
    public float contrast = 0f;
    public float saturation = 0f;
    public Color colorFilter = Color.white;
    
    void Start()
    {
        if (postProcessVolume == null)
        {
            postProcessVolume = GetComponent<PostProcessVolume>();
            if (postProcessVolume == null)
            {
                postProcessVolume = gameObject.AddComponent<PostProcessVolume>();
                postProcessVolume.isGlobal = true;
            }
        }
        
        ApplyPostProcessing();
    }
    
    void ApplyPostProcessing()
    {
        PostProcessProfile profile = postProcessVolume.profile;
        if (profile == null)
        {
            profile = ScriptableObject.CreateInstance<PostProcessProfile>();
            postProcessVolume.profile = profile;
        }
        
        // Bloom
        if (enableBloom)
        {
            Bloom bloom;
            if (!profile.TryGetSettings(out bloom))
            {
                bloom = profile.AddSettings<Bloom>();
            }
            bloom.intensity.value = bloomIntensity;
            bloom.threshold.value = bloomThreshold;
        }
        
        // Motion Blur
        if (enableMotionBlur)
        {
            MotionBlur motionBlur;
            if (!profile.TryGetSettings(out motionBlur))
            {
                motionBlur = profile.AddSettings<MotionBlur>();
            }
            motionBlur.shutterAngle.value = motionBlurAmount * 360f;
        }
        
        // Vignette
        if (enableVignette)
        {
            Vignette vignette;
            if (!profile.TryGetSettings(out vignette))
            {
                vignette = profile.AddSettings<Vignette>();
            }
            vignette.intensity.value = vignetteIntensity;
            vignette.color.value = vignetteColor;
        }
        
        // Color Grading
        if (enableColorGrading)
        {
            ColorGrading colorGrading;
            if (!profile.TryGetSettings(out colorGrading))
            {
                colorGrading = profile.AddSettings<ColorGrading>();
            }
            colorGrading.contrast.value = contrast;
            colorGrading.saturation.value = saturation;
            colorGrading.colorFilter.value = colorFilter;
        }
        
        Debug.Log("Post Processing applied!");
    }
    
    public void EnablePostProcessing(bool enable)
    {
        if (postProcessVolume)
        {
            postProcessVolume.enabled = enable;
        }
    }
    
    public void SetBloomIntensity(float intensity)
    {
        bloomIntensity = intensity;
        ApplyPostProcessing();
    }
    
    public void SetMotionBlur(float amount)
    {
        motionBlurAmount = amount;
        ApplyPostProcessing();
    }
}