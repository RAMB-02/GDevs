using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroCutscene : MonoBehaviour
{
    public Image[] cutsceneImages;
    public float displayTime = 2f;

    private int currentIndex = 0;
    private float timer = 0f;
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
                SceneManager.LoadScene("MainGameScene");
            return;
        }

        timer += Time.deltaTime;

        if (timer >= displayTime && !isLastImage)
        {
            currentIndex++;
            timer = 0f;
            ShowImage(currentIndex);
        }

        if (isLastImage && Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("MainGameScene");
        }
    }

    void ShowImage(int index)
    {
        for (int i = 0; i < cutsceneImages.Length; i++)
        {
            cutsceneImages[i].gameObject.SetActive(i == index);
        }
    }
}
