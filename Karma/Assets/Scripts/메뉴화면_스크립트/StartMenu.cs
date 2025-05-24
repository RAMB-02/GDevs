using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public SettingsPanelController settingsPanelController; // 인스펙터에서 설정 패널 오브젝트 연결

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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanelController != null && settingsPanelController.gameObject.activeInHierarchy)
            {
                settingsPanelController.OnClickCloseButton();
            }
        }
    }

    public void OnClickPlay()
    {
        Debug.Log("Play button clicked");
        StartCoroutine(DelayedLoadScene("IntroScene"));
    }

    public void OnClickExit()
    {
        Debug.Log("Exit button clicked");
        StartCoroutine(DelayedExit());
    }

    private System.Collections.IEnumerator DelayedLoadScene(string sceneName)
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
    }

    private System.Collections.IEnumerator DelayedExit()
    {
        yield return new WaitForSeconds(1f);
        PlayerPrefs.Save();
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
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
        Debug.Log("How To Play button clicked - Not implemented yet");
    }
}
