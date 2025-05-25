using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndingCutscene : MonoBehaviour
{
    public Image[] endingImages;
    public float displayTime = 2f;

    private int currentIndex = 0;
    private float timer = 0f;
    private bool isLastImage => currentIndex >= endingImages.Length - 1;

    void Start()
    {
        ShowImage(0);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= displayTime && !isLastImage)
        {
            currentIndex++;
            timer = 0f;
            ShowImage(currentIndex);
        }

        if (isLastImage && Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("MainView");
        }
    }

    void ShowImage(int index)
    {
        for (int i = 0; i < endingImages.Length; i++)
        {
            endingImages[i].gameObject.SetActive(i == index);
        }
    }
}
