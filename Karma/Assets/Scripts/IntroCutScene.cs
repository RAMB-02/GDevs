using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroCutscene : MonoBehaviour
{
    public Image[] cutsceneImages;        // 컷신 이미지들
    public AudioClip[] audioClips;        // 컷마다 재생할 소리
    public AudioSource audioSource;       // 공통 AudioSource

    private int currentIndex = 0;
    private bool isLastImage => currentIndex >= cutsceneImages.Length - 1;

    void Start()
    {
        ShowImage(0);
    }

    void Update()
    {
        if (cutsceneImages.Length <= 1)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (audioSource != null) audioSource.Stop();
                SceneManager.LoadScene("MainGameScene");
            }
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (isLastImage)
            {
                if (audioSource != null) audioSource.Stop();
                SceneManager.LoadScene("MainGameScene");
                return;
            }

            currentIndex++;
            ShowImage(currentIndex);
        }
    }

    void ShowImage(int index)
    {
        // 이미지 전환
        for (int i = 0; i < cutsceneImages.Length; i++)
        {
            cutsceneImages[i].gameObject.SetActive(i == index);
        }

        // 소리 재생
        if (audioSource != null && audioClips != null && index < audioClips.Length)
        {
            audioSource.clip = audioClips[index];
            audioSource.Play();
        }
    }
}
