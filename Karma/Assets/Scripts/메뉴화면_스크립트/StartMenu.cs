using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public SettingsPanelController settingsPanelController;
    public GameObject howToPlayPanel;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;

        if (PauseMenu.isPaused)
        {
            PauseMenu.isPaused = false;
        }

        if (settingsPanelController != null && settingsPanelController.gameObject.activeSelf)
        {
            settingsPanelController.gameObject.SetActive(false);
        }
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (howToPlayPanel != null && howToPlayPanel.activeInHierarchy)
            {
                OnClickCloseHowToPlayPanel();
            }
            if (settingsPanelController != null && settingsPanelController.gameObject.activeInHierarchy)
            {
                settingsPanelController.OnClickCloseButton();
            }
        }
    }

    public void OnClickPlay()
    {
        Debug.Log("Play button clicked");
        Invoke(nameof(LoadGameScene), 1f); // 1초 후 씬 로드
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene("IntroScene");
    }

    public void OnClickSettings()
    {
        Debug.Log("Settings button clicked on Main Menu");
        if (settingsPanelController != null)
        {
            settingsPanelController.OpenPanel(null);
        }
        else
        {
            Debug.LogError("SettingsPanelController is not assigned in StartMenu!");
        }
    }

    public void OnClickHow()
    {
        Debug.Log("How To Play button clicked");
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("HowToPlayPanel is not assigned in StartMenu!");
        }
    }

    public void OnClickCloseHowToPlayPanel()
    {
        Debug.Log("Close How To Play panel button clicked");
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
            Debug.Log("닫힘");
        }
    }

    public void OnClickExit()
    {
        Debug.Log("Exit button clicked");
        Invoke(nameof(DelayedExit), 1f); // 1초 후 종료
    }

    void DelayedExit()
    {
        PlayerPrefs.Save();
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
