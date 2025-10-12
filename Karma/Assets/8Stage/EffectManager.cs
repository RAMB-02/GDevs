using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class EffectManager : MonoBehaviour
{
    public Volume postProcessVolume;
    public VolumeProfile cursedProfile;
    private Coroutine runningEffectCoroutine;

    void Start()
    {
        if (postProcessVolume != null)
        {
            postProcessVolume.profile = cursedProfile;
            // 8스테이지는 저주 상태로 시작하므로 Weight를 1로 설정
            postProcessVolume.weight = 1f;
        }
    }

    public void FadeOut(float duration)
    {
        if (runningEffectCoroutine != null) StopCoroutine(runningEffectCoroutine);
        runningEffectCoroutine = StartCoroutine(FadeWeight(0f, duration));
    }

    private IEnumerator FadeWeight(float targetWeight, float duration)
    {
        float startWeight = postProcessVolume.weight;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            postProcessVolume.weight = Mathf.Lerp(startWeight, targetWeight, timer / duration);
            yield return null;
        }
        postProcessVolume.weight = targetWeight;
    }
}