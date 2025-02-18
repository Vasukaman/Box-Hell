using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class ScreenEffectManager : MonoBehaviour
{
    
    public static ScreenEffectManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            InitializePostProcessing();
        }
    }

    [SerializeField] private Volume postProcessVolume;
    [SerializeField] private Bloom bloom;
    [SerializeField] private ChromaticAberration chromaticAberration;
    [SerializeField] private ColorAdjustments colorAdjustments;


    private void InitializePostProcessing()
    {
        if (!postProcessVolume)
        {
            postProcessVolume = GameObject.FindObjectOfType<Volume>();
            if (!postProcessVolume)
            {
                Debug.LogError("ScreenEffectManager: No PostProcessVolume found in the scene.");
                return;
            }
        }

        if (!bloom)
            postProcessVolume.profile.TryGet(out bloom);
        if (!chromaticAberration)
            postProcessVolume.profile.TryGet(out chromaticAberration);
        if (!colorAdjustments)
            postProcessVolume.profile.TryGet(out colorAdjustments);

    }

    private void SetBloom(float newIntensity)
    {
        bloom.intensity.value = newIntensity;
    }
    private void SetChromAbber(float newIntensity)
    {
        chromaticAberration.intensity.value = newIntensity;
    } 
    
    private void SetColourPostExposure(float newIntensity)
    {
        colorAdjustments.postExposure.value = newIntensity;
    }
    private Tween AnimateBloomIntensity(float startValue, float endValue, float duration)
    {
        float currentValue = startValue;
        Tweener tween = DOTween.To(SetBloom, currentValue, endValue, duration)
                                .SetEase(Ease.Linear)           ;
        return tween;
    }

    private Tween AnimateChromaticAberration(float startValue, float endValue, float duration)
    {
        float currentValue = startValue;
        Tweener tween = DOTween.To(SetChromAbber, currentValue, endValue, duration);
        return tween;
    }  
    
    private Tween AnimatePostExposure(float startValue, float endValue, float duration)
    {
        float currentValue = startValue;
        Tweener tween = DOTween.To(SetColourPostExposure, currentValue, endValue, duration);
        return tween;
    }

    public void HitEffect(float duration)
    {
        float initialBloomIntensity = bloom.intensity.value;
        float initialChromaticAberration = chromaticAberration.intensity.value;
        float initialPostExposure = colorAdjustments.postExposure.value;


        AnimateBloomIntensity(10f, initialBloomIntensity, duration);
        AnimateChromaticAberration(1f, initialChromaticAberration, duration);
        AnimatePostExposure(10f, initialPostExposure, duration/2);

            
       // Invoke(nameof(ResetEffects), duration);
    }

    private void ResetEffects()
    {
        bloom.intensity.value = bloom.intensity.value;
        chromaticAberration.intensity.value = chromaticAberration.intensity.value;

    }

    private void OnDestroy()
    {
        if (IsInvoking())
            CancelInvoke();

        DOTween.Kill(this);
    }
}