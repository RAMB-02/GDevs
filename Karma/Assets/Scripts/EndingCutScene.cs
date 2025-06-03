using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndingCutscene : MonoBehaviour
{
    public Image[] endingImages;
    private int currentIndex = 0;
    private bool isLastImage => currentIndex >= endingImages.Length - 1;

    void Start()
    {
        ShowImage(0);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isLastImage)
            {
                SceneManager.LoadScene("MainView");
            }
            else
            {
                currentIndex++;
                ShowImage(currentIndex);
            }
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
