using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering; // Volume 사용
using UnityEngine.Rendering.HighDefinition; // HDRP 사용
using UnityEngine.UI;

public class SettingsPanelController : MonoBehaviour
{
    [Header("UI References")]
    public Slider sensitivitySlider;
    public Slider volumeSlider;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Slider brightnessSliderHDRP;
    // public Button closeButton; // 닫기 버튼은 인스펙터에서 OnClick()으로 OnClickCloseButton() 연결

    [Header("System References")]
    public AudioMixer mainMixer;
    public Volume globalVolume; // 이 설정 패널이 제어할 Global Volume

    [Header("Settings Values & Ranges")]
    public static float mouseSensitivity = 1f;
    public float minExposureValue = -2f;
    public float maxExposureValue = 2f;
    // PlayerPrefs Keys
    public const string PREFS_MOUSE_SENSITIVITY = "MouseSensitivity";
    public const string PREFS_MASTER_VOLUME = "MasterVolume";
    public const string PREFS_RESOLUTION_WIDTH = "ResolutionWidth";
    public const string PREFS_RESOLUTION_HEIGHT = "ResolutionHeight";
    public const string PREFS_RESOLUTION_REFRESH_NUM = "ResolutionRefreshNumerator";
    public const string PREFS_RESOLUTION_REFRESH_DENOM = "ResolutionRefreshDenominator";
    public const string PREFS_FULLSCREEN = "FullscreenPreference";
    public const string PREFS_BRIGHTNESS_HDRP = "ScreenBrightnessHDRP";

    private ColorAdjustments colorAdjustments;
    private Resolution[] allResolutions;
    private List<Resolution> filteredResolutions;
    private int currentResolutionIndex = 0;
    private Action _onCloseCallback;

    void Awake()
    {
        if (globalVolume != null && globalVolume.profile != null)
        {
            if (!globalVolume.profile.TryGet(out colorAdjustments))
            {
                Debug.LogError("SettingsPanelController: Color Adjustments override not found. HDRP Brightness may not work.", gameObject);
            }
        }
        else if (brightnessSliderHDRP != null && brightnessSliderHDRP.gameObject.activeSelf) // activeSelf 대신 activeInHierarchy가 더 정확할 수 있음
        {
             Debug.LogWarning("SettingsPanelController: Global Volume is not assigned. HDRP Brightness control will not work.", gameObject);
        }
    }

    void OnEnable()
    {
        Debug.Log("SettingsPanelController: Panel Enabled. Refreshing UI from PlayerPrefs.");
        LoadAndApplyAllSettings();
        SetupAllListeners();
    }

    public void OpenPanel(Action onCloseCallback = null)
    {
        _onCloseCallback = onCloseCallback;
        gameObject.SetActive(true);
    }

    public void OnClickCloseButton()
    {
        PlayerPrefs.Save();
        gameObject.SetActive(false);
        _onCloseCallback?.Invoke();
        Debug.Log("SettingsPanelController: Settings panel closed. Callback invoked.");
    }

    void LoadAndApplyAllSettings()
    {
        SetupMouseSensitivityUI();
        SetupAudioUI();
        SetupScreenSettingsUI();
        SetupBrightnessUI_HDRP();
    }

    void SetupAllListeners()
    {
        if (sensitivitySlider != null) {
            sensitivitySlider.onValueChanged.RemoveAllListeners();
            sensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);
        }
        if (volumeSlider != null) {
            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }
        if (resolutionDropdown != null) {
            resolutionDropdown.onValueChanged.RemoveAllListeners();
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }
        if (fullscreenToggle != null) {
            fullscreenToggle.onValueChanged.RemoveAllListeners();
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }
        if (brightnessSliderHDRP != null) {
            brightnessSliderHDRP.onValueChanged.RemoveAllListeners();
            brightnessSliderHDRP.onValueChanged.AddListener(SetBrightnessHDRP);
        }
    }

    #region Mouse Sensitivity
    void SetupMouseSensitivityUI()
    {
        if (sensitivitySlider != null)
        {
            mouseSensitivity = PlayerPrefs.GetFloat(PREFS_MOUSE_SENSITIVITY, 1f);
            sensitivitySlider.value = mouseSensitivity;
        }
    }
    public void SetMouseSensitivity(float sensitivity)
    {
        mouseSensitivity = sensitivity;
        PlayerPrefs.SetFloat(PREFS_MOUSE_SENSITIVITY, sensitivity);
    }
    #endregion

    #region Audio Settings
    void SetupAudioUI()
    {
        if (volumeSlider != null) // mainMixer null 체크는 ApplyMasterVolumeToMixer에서 함
        {
            float savedVolume = PlayerPrefs.GetFloat(PREFS_MASTER_VOLUME, 1f);
            volumeSlider.value = savedVolume; // 슬라이더 UI는 항상 PlayerPrefs 값으로 업데이트

            // ★★★ 수정된 부분 ★★★
            // PauseMenu에 의해 게임이 일시정지된 상태가 아닐 때만 Mixer 볼륨을 실제로 변경
            if (!PauseMenu.isPaused)
            {
                ApplyMasterVolumeToMixer(savedVolume);
                Debug.Log("SettingsPanelController.SetupAudioUI: Applied PlayerPrefs volume to mixer (game not paused by PauseMenu).");
            }
            else
            {
                Debug.Log("SettingsPanelController.SetupAudioUI: Game IS paused by PauseMenu. Mixer volume NOT changed from snapshot. Slider shows PlayerPrefs value.");
            }
        }
    }
    public void SetMasterVolume(float volume) // 슬라이더 값 (0.0001 ~ 1)
    {
        PlayerPrefs.SetFloat(PREFS_MASTER_VOLUME, volume); // PlayerPrefs에는 항상 최신 슬라이더 값 저장
        Debug.Log("SettingsPanelController: Master Volume PlayerPrefs updated to: " + volume);

        // ★★★ 수정된 부분 ★★★
        // PauseMenu에 의해 게임이 일시정지된 상태가 아닐 때만 Mixer 볼륨을 실제로 변경
        if (!PauseMenu.isPaused)
        {
            ApplyMasterVolumeToMixer(volume);
        }
        else
        {
            Debug.LogWarning("SettingsPanelController.SetMasterVolume: Game IS paused by PauseMenu. Actual mixer volume update SKIPPED. PlayerPrefs was updated.");
        }
    }
    void ApplyMasterVolumeToMixer(float volume)
    {
        if (mainMixer != null)
        {
            float dbVolume = (volume <= 0.0001f) ? -80f : Mathf.Log10(volume) * 20;
            mainMixer.SetFloat("MasterVolume", dbVolume);
        }
        else { Debug.LogError("SettingsPanelController: MainMixer is NOT assigned!"); }
    }
    #endregion

    #region Brightness Settings (HDRP)
    void SetupBrightnessUI_HDRP()
    {
        if (brightnessSliderHDRP == null) return;
        if (globalVolume == null || colorAdjustments == null) {
            Debug.LogWarning("SettingsPanelController: Global Volume or ColorAdjustments not available. Hiding HDRP Brightness slider.", gameObject);
            brightnessSliderHDRP.gameObject.SetActive(false);
            return;
        }
        brightnessSliderHDRP.gameObject.SetActive(true);
        float savedSliderValue = PlayerPrefs.GetFloat(PREFS_BRIGHTNESS_HDRP, 0.5f);
        brightnessSliderHDRP.value = savedSliderValue;
        ApplyBrightnessToHDRP(savedSliderValue);
    }
    public void SetBrightnessHDRP(float sliderValue)
    {
        ApplyBrightnessToHDRP(sliderValue);
        PlayerPrefs.SetFloat(PREFS_BRIGHTNESS_HDRP, sliderValue);
    }
    void ApplyBrightnessToHDRP(float sliderValue)
    {
        if (colorAdjustments != null)
        {
            float exposureValue = Mathf.Lerp(minExposureValue, maxExposureValue, sliderValue);
            colorAdjustments.postExposure.value = exposureValue;
        }
    }
    #endregion

    #region Screen Settings
    void SetupScreenSettingsUI()
    {
        if (resolutionDropdown == null || fullscreenToggle == null) return;

        allResolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        var uniqueDisplayModes = allResolutions
            .Select(res => new { res.width, res.height, refreshRate = GetRefreshRateValue(res) })
            .Distinct()
            .OrderByDescending(res => res.width * res.height)
            .ThenByDescending(res => res.refreshRate);

        foreach (var displayMode in uniqueDisplayModes)
        {
            Resolution actualResolution = allResolutions.FirstOrDefault(r => r.width == displayMode.width && r.height == displayMode.height && GetRefreshRateValue(r) == displayMode.refreshRate);
            if (actualResolution.width > 0)
            {
                 filteredResolutions.Add(actualResolution);
                 options.Add(FormatResolution(actualResolution));
            }
        }
        
        resolutionDropdown.AddOptions(options);
        LoadAndApplyScreenSettingsFromPrefs();
        resolutionDropdown.RefreshShownValue();
    }

    void LoadAndApplyScreenSettingsFromPrefs()
    {
        bool fs = PlayerPrefs.GetInt(PREFS_FULLSCREEN, Screen.fullScreen ? 1 : 0) == 1;
        fullscreenToggle.isOn = fs;

        int savedWidth = PlayerPrefs.GetInt(PREFS_RESOLUTION_WIDTH, Screen.currentResolution.width);
        int savedHeight = PlayerPrefs.GetInt(PREFS_RESOLUTION_HEIGHT, Screen.currentResolution.height);
        RefreshRate currentRefreshRateStruct = Screen.currentResolution.refreshRateRatio;
        #if UNITY_2022_2_OR_NEWER
        currentRefreshRateStruct.numerator = (uint)PlayerPrefs.GetInt(PREFS_RESOLUTION_REFRESH_NUM, (int)currentRefreshRateStruct.numerator);
        currentRefreshRateStruct.denominator = (uint)PlayerPrefs.GetInt(PREFS_RESOLUTION_REFRESH_DENOM, (int)currentRefreshRateStruct.denominator);
        #endif
        
        bool foundSavedResolution = false;
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            if (filteredResolutions[i].width == savedWidth &&
                filteredResolutions[i].height == savedHeight &&
                CompareRefreshRates(filteredResolutions[i].refreshRateRatio, currentRefreshRateStruct))
            {
                currentResolutionIndex = i;
                resolutionDropdown.value = i;
                foundSavedResolution = true;
                break;
            }
        }
        if (!foundSavedResolution && filteredResolutions.Count > 0)
        {
            for (int i = 0; i < filteredResolutions.Count; i++) {
                if (filteredResolutions[i].width == Screen.currentResolution.width &&
                    filteredResolutions[i].height == Screen.currentResolution.height &&
                    CompareRefreshRates(filteredResolutions[i].refreshRateRatio, Screen.currentResolution.refreshRateRatio)) {
                    currentResolutionIndex = i;
                    resolutionDropdown.value = i;
                    foundSavedResolution = true;
                    break;
                }
            }
            if (!foundSavedResolution) {
                 currentResolutionIndex = 0;
                 resolutionDropdown.value = 0;
            }
        }
        // ApplyScreenSettingsAtStartup(); // 게임 시작 시 한 번 적용하는 로직 (선택적)
    }
    
    bool CompareRefreshRates(RefreshRate r1, RefreshRate r2) {
        #if UNITY_2022_2_OR_NEWER
        return r1.numerator == r2.numerator && r1.denominator == r2.denominator;
        #else
        return GetRefreshRateValue(new Resolution{refreshRateRatio = r1}) == GetRefreshRateValue(new Resolution{refreshRateRatio = r2});
        #endif
    }

    string FormatResolution(Resolution resolution) { return resolution.width + " x " + resolution.height + " @ " + GetRefreshRateValue(resolution) + " Hz"; }

    int GetRefreshRateValue(Resolution resolution)
    {
        #if UNITY_2022_2_OR_NEWER
        if (resolution.refreshRateRatio.denominator == 0) return (int)resolution.refreshRateRatio.value; // Hz로 바로 변환
        return (int)Mathf.Round((float)resolution.refreshRateRatio.numerator / resolution.refreshRateRatio.denominator);
        #else
        return resolution.refreshRate;
        #endif
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex < 0 || resolutionIndex >= filteredResolutions.Count) return;
        currentResolutionIndex = resolutionIndex;
        ApplyCurrentSelectedScreenSettings();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        PlayerPrefs.SetInt(PREFS_FULLSCREEN, isFullscreen ? 1 : 0);
        ApplyCurrentSelectedScreenSettings();
    }

    void ApplyCurrentSelectedScreenSettings()
    {
        if (filteredResolutions == null || currentResolutionIndex < 0 || currentResolutionIndex >= filteredResolutions.Count) return;

        Resolution resolution = filteredResolutions[currentResolutionIndex];
        bool fullscreen = PlayerPrefs.GetInt(PREFS_FULLSCREEN, Screen.fullScreen ? 1 : 0) == 1;
        FullScreenMode modeToSet = fullscreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
        
        // 현재 해상도/모드와 동일하면 변경하지 않음 (불필요한 깜빡임 방지)
        if (Screen.currentResolution.width == resolution.width &&
            Screen.currentResolution.height == resolution.height &&
            Screen.fullScreenMode == modeToSet &&
            CompareRefreshRates(Screen.currentResolution.refreshRateRatio, resolution.refreshRateRatio)) {
            // Debug.Log("Screen settings are already applied.");
            return;
        }

        Screen.SetResolution(resolution.width, resolution.height, modeToSet, resolution.refreshRateRatio);

        PlayerPrefs.SetInt(PREFS_RESOLUTION_WIDTH, resolution.width);
        PlayerPrefs.SetInt(PREFS_RESOLUTION_HEIGHT, resolution.height);
        #if UNITY_2022_2_OR_NEWER
        PlayerPrefs.SetInt(PREFS_RESOLUTION_REFRESH_NUM, (int)resolution.refreshRateRatio.numerator);
        PlayerPrefs.SetInt(PREFS_RESOLUTION_REFRESH_DENOM, (int)resolution.refreshRateRatio.denominator);
        #endif
        Debug.Log("SettingsPanelController: Screen settings applied - " + FormatResolution(resolution) + ", Fullscreen: " + fullscreen);
    }
    #endregion
}