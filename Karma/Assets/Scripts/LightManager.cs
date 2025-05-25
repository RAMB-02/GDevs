using UnityEngine;

public class LightManager : MonoBehaviour
{
    public static LightManager Instance { get; private set; }

    [Header("���� ����Ʈ (���� ���)")]
    public Light[] lights; // �ν����Ϳ��� ���

    [Header("���� ����")]
    public float normalRange = 20f;
    public float normalIntensity = 3200f;

    [Header("�̻� ����")]
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
