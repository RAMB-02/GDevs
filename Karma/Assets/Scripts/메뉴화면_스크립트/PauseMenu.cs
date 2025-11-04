using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public static bool isPaused = false;
    public AudioMixerSnapshot unpausedSnapshot;
    public AudioMixerSnapshot pausedSnapshot;
    public float snapshotTransitionTime = 0.0f;
    public AudioMixer mainMixer; // 스냅샷 전환 전 SetFloat을 위해 참조

    public SettingsPanelController settingsPanelController; // ★★★ 인스펙터에서 설정 패널 오브젝트 연결 ★★★

    // ▼▼▼ 1. 'How To Play' 패널 참조 변수 추가 ▼▼▼
    public GameObject howToPlayPanel; // ★★★ 인스펙터에서 'How To Play' 패널 오브젝트 연결 ★★★

    void Awake()
    {
        // 게임 씬이 로드될 때 일시정지 메뉴는 기본적으로 비활성화
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // ★★★ 게임 씬 시작 시 설정 패널도 비활성화 ★★★
        if (settingsPanelController != null && settingsPanelController.gameObject != null)
        {
            settingsPanelController.gameObject.SetActive(false);
            Debug.Log("PauseMenu.Awake: SettingsPanelController's GameObject has been deactivated.");
        }
        
        // ▼▼▼ 2. 'How To Play' 패널도 시작 시 비활성화 ▼▼▼
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
        // ▲▲▲

        SetCursorState(false); // 게임 플레이 중에는 커서 잠금/숨김
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 설정 패널이 열려있으면 닫기
            if (settingsPanelController != null && settingsPanelController.gameObject.activeInHierarchy)
            {
                settingsPanelController.OnClickCloseButton(); // 설정 패널의 닫기 함수 호출
                return; // PauseMenu 토글 로직 실행 안함
            }

            // ▼▼▼ 3. 'How To Play' 패널이 열려있으면 닫기 (Update에 추가) ▼▼▼
            if (howToPlayPanel != null && howToPlayPanel.activeInHierarchy)
            {
                OnClickCloseHowToPlayPanel(); // 'How To Play' 패널 닫기
                return;
            }
            // ▲▲▲

            // (위의 패널들이 모두 닫혀있을 때)
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    public void PauseGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true; 

        if (mainMixer != null) mainMixer.SetFloat("MasterVolume", -80f);
        if (pausedSnapshot != null) pausedSnapshot.TransitionTo(snapshotTransitionTime);

        SetCursorState(true);
        Debug.Log("GamePaused. isPaused = " + isPaused);
    }

    public void ResumeGame()
    {
        // howToPlayPanel이 열려있을 수 있으므로 Resume 시 강제로 닫아줌
        if (howToPlayPanel != null && howToPlayPanel.activeInHierarchy)
        {
            howToPlayPanel.SetActive(false);
        }
        
        if (pauseMenuPanel != null && pauseMenuPanel.activeInHierarchy) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false; 
        Debug.Log("<color=green>PauseMenu.ResumeGame: 'isPaused' 플래그가 이제 'false'로 설정됨. 현재 값: " + isPaused + "</color>");

        if (mainMixer != null)
        {
            float lastSliderVolume = PlayerPrefs.GetFloat(SettingsPanelController.PREFS_MASTER_VOLUME, 1f); // 상수 사용
            float targetDb = (lastSliderVolume <= 0.0001f) ? -80f : Mathf.Log10(lastSliderVolume) * 20;
            mainMixer.SetFloat("MasterVolume", targetDb); 
        }
        if (unpausedSnapshot != null) unpausedSnapshot.TransitionTo(snapshotTransitionTime);
        
        SetCursorState(false);
    }

    void SetCursorState(bool paused)
    {
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }

    public void OnClickResume() { ResumeGame(); }

    public void OnClickSettings()
    {
        Debug.Log("PauseMenu: Settings button clicked.");
        if (settingsPanelController != null)
        {
            settingsPanelController.OpenPanel(ShowPausePanelAfterSettingsClosed); // 콜백 전달
        }
        else
        {
            Debug.LogError("PauseMenu: SettingsPanelController is not assigned!");
        }
    }

    public void ShowPausePanelAfterSettingsClosed() // SettingsPanelController가 호출할 콜백
    {
        Debug.Log("PauseMenu: ShowPausePanelAfterSettingsClosed called by SettingsPanelController.");
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        SetCursorState(true); // 일시정지 메뉴이므로 커서 다시 보이기
    }

    // ▼▼▼ 4. OnClickRestart() 함수를 주석 처리 (또는 삭제) ▼▼▼
    /*
    public void OnClickRestart()
    {
        ResumeGame(); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    */
    // ▲▲▲

    // ▼▼▼ 5. 'How To Play' 관련 함수 2개 추가 ▼▼▼
    
    /**
     * 'How To Play' 버튼 클릭 시 호출됩니다.
     */
    public void OnClickHow()
    {
        Debug.Log("PauseMenu: How To Play button clicked");
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("PauseMenu: HowToPlayPanel is not assigned!");
        }
    }

    /**
     * 'How To Play' 패널의 닫기 버튼 또는 ESC 키로 호출됩니다.
     */
    public void OnClickCloseHowToPlayPanel()
    {
        Debug.Log("PauseMenu: Close How To Play panel button clicked");
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
    }
    // ▲▲▲


    public void OnClickMainMenu()
    {
        ResumeGame(); 
        SceneManager.LoadScene("StartMenuScene"); // 실제 메인 메뉴 씬 이름 사용
    }
}