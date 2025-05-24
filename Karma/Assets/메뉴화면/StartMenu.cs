using UnityEngine;
using UnityEngine.SceneManagement;
// using UnityEngine.UI; // 직접 UI 요소 제어 안 함
// using TMPro; // 직접 UI 요소 제어 안 함
// using UnityEngine.Audio; // 직접 Mixer 제어 안 함
// using UnityEngine.Rendering; // 직접 Volume 제어 안 함
// using UnityEngine.Rendering.HighDefinition; // 직접 Volume 제어 안 함

public class StartMenu : MonoBehaviour
{
    public SettingsPanelController settingsPanelController; // 인스펙터에서 설정 패널 오브젝트(SettingsPanelController가 붙은) 연결

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;

        if (PauseMenu.isPaused) // 다른 씬에서 일시정지 상태로 돌아왔을 경우 대비
        {
            PauseMenu.isPaused = false;
        }

        // 메인 메뉴 시작 시 설정 패널은 비활성화 상태여야 함
        if (settingsPanelController != null && settingsPanelController.gameObject.activeSelf)
        {
            settingsPanelController.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // 메인 메뉴에서 ESC 키로 설정 패널 닫기 (만약 설정 패널에 자체 ESC 닫기 로직이 없다면)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanelController != null && settingsPanelController.gameObject.activeInHierarchy)
            {
                settingsPanelController.OnClickCloseButton(); // 설정 패널의 닫기 함수 호출
            }
        }
    }

    public void OnClickPlay()
    {
        Debug.Log("Play button clicked");
        // 필요한 경우 Time.timeScale = 1f; PauseMenu.isPaused = false; 등을 여기서도 확인
        SceneManager.LoadScene("OutdoorsScene"); // 실제 게임 씬 이름
    }

    public void OnClickSettings()
    {
        Debug.Log("Settings button clicked on Main Menu");
        if (settingsPanelController != null)
        {
            // 메인 메뉴에서 설정창을 닫았을 때 특별히 할 동작이 없다면 null 콜백 전달
            settingsPanelController.OpenPanel(null);
        }
        else
        {
            Debug.LogError("SettingsPanelController is not assigned in StartMenu!");
        }
    }

    public void OnClickHow() // 게임 방법 버튼 (구현 필요)
    {
        Debug.Log("How To Play button clicked - Not implemented yet");
    }

    public void OnClickExit()
    {
        Debug.Log("Exit button clicked");
        PlayerPrefs.Save(); // 종료 전 모든 설정 저장
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}