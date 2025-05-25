using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroCutscene : MonoBehaviour
{
    public Image[] cutsceneImages;        // �ƽ� �̹�����
    public AudioClip[] audioClips;        // �Ƹ��� ����� �Ҹ�
    public AudioSource audioSource;       // ���� AudioSource

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
        // �̹��� ��ȯ
        for (int i = 0; i < cutsceneImages.Length; i++)
        {
            cutsceneImages[i].gameObject.SetActive(i == index);
        }

        // �Ҹ� ���
        if (audioSource != null && audioClips != null && index < audioClips.Length)
        {
            audioSource.clip = audioClips[index];
            audioSource.Play();
        }
    }
}
