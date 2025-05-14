using UnityEngine;

public class LightManager : MonoBehaviour
{
    public static LightManager Instance { get; private set; }

    [Header("조명 리스트 (수동 등록)")]
    public Light[] lights; // 인스펙터에서 등록

    [Header("정상 상태")]
    public float normalRange = 20f;
    public float normalIntensity = 3200f;

    [Header("이상 상태")]
    public float anomalyRange = 12f;
    public float anomalyIntensity = 500f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetAnomalyLights(bool isAnomaly)
    {
        foreach (Light light in lights)
        {
            if (light == null) continue;

            light.range = isAnomaly ? anomalyRange : normalRange;
            light.intensity = isAnomaly ? anomalyIntensity : normalIntensity;
        }
    }
}
