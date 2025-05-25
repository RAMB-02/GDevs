using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class OutdoorsSceneSettingsApplier : MonoBehaviour
{
    [Header("HDRP Brightness")]
    public Volume sceneGlobalVolume;
    public float minExposureValue = -2f; // SettingsPanelController와 동일하게 유지
    public float maxExposureValue = 2f; // SettingsPanelController와 동일하게 유지

    private ColorAdjustments colorAdjustments;

    void Start()
    {
        // 만약 SettingsPanelController가 DontDestroyOnLoad이고 static instance를 가진다면,
        // 여기서 StartMenu.Instance처럼 SettingsPanelController.Instance?.minExposureValue 등으로 가져올 수 있음
        // 지금은 수동으로 값 일치 가정

        if (sceneGlobalVolume == null || sceneGlobalVolume.profile == null)
        {
            Debug.LogWarning("OutdoorsSceneSettingsApplier: Scene Global Volume or Profile not assigned.");
            return;
        }

        if (!sceneGlobalVolume.profile.TryGet(out colorAdjustments))
        {
            Debug.LogError("OutdoorsSceneSettingsApplier: Color Adjustments override not found.");
            return;
        }
        ApplySavedBrightness();
    }

    void ApplySavedBrightness()
    {
        // SettingsPanelController에서 정의한 PlayerPrefs 키 상수 사용 권장
        float savedSliderValue = PlayerPrefs.GetFloat(SettingsPanelController.PREFS_BRIGHTNESS_HDRP, 0.5f);
        float exposureValue = Mathf.Lerp(minExposureValue, maxExposureValue, savedSliderValue);

        colorAdjustments.postExposure.value = exposureValue;
        Debug.Log("OutdoorsSceneSettingsApplier: HDRP Brightness applied: " + exposureValue + " (Slider: " + savedSliderValue + ")");
    }
}