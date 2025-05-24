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
        // else if (settingsPanelController == null) // 이 로그는 OnClickSettings 등에서 이미 처리 중
        // {
        //     Debug.LogError("PauseMenu.Awake: settingsPanelController is not assigned in the Inspector!");
        // }


        // isPaused와 Time.timeScale은 게임 씬 진입 시 StartMenu나 다른 로직에서 이미 정상화되었을 것으로 가정
        // 만약 이 씬에서 바로 시작하는 경우를 대비하려면 여기서도 초기화 필요
        // isPaused = false; // StartMenu.cs의 Start()에서 이미 처리하고 있음
        // Time.timeScale = 1f; // StartMenu.cs의 Start()에서 이미 처리하고 있음
        SetCursorState(false); // 게임 플레이 중에는 커서 잠금/숨김
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 설정 패널이 열려있을 때는 PauseMenu가 직접 ESC를 처리하지 않고,
            // SettingsPanelController의 자체 닫기 로직(있다면) 또는 여기서 닫아주고 PauseMenu로 돌아오게 함.
            if (settingsPanelController != null && settingsPanelController.gameObject.activeInHierarchy)
            {
                settingsPanelController.OnClickCloseButton(); // 설정 패널의 닫기 함수 호출
                return; // PauseMenu 토글 로직 실행 안함
            }

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

    public void OnClickRestart()
    {
        ResumeGame(); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClickMainMenu()
    {
        ResumeGame(); 
        SceneManager.LoadScene("StartMenuScene"); // 실제 메인 메뉴 씬 이름 사용
    }
}