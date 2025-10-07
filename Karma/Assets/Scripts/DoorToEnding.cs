using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DoorToEnding : MonoBehaviour
{
    private bool playerInRange = false;
    private bool isFading = false;

    [Header("연출 요소")]
    public AudioSource audioSource;
    public Image fadeImage;
    public float fadeDuration = 1f;

    [Header("UI")]
    public GameObject interactUI;

    private void Awake()
    {
        playerInRange = false;
        isFading = false;

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactUI != null)
                interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (!playerInRange || isFading) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(GoToEnding());
        }
    }

    private IEnumerator GoToEnding()
    {
        isFading = true;

        if (interactUI != null)
            interactUI.SetActive(false);

        if (audioSource != null)
            audioSource.Play();

        if (fadeImage != null)
            yield return StartCoroutine(Fade(0f, 1f));

        // --- 여기가 핵심 변경 부분 ---
        // NecklessScript의 static 변수를 확인하여 분기를 결정합니다.
        if (NecklessScript.hasStolenNecklace)
        {
            SceneManager.LoadScene("BadEndingScene");
        }
        else
        {
            SceneManager.LoadScene("NormalEndingScene");
        }
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = alpha;
                fadeImage.color = c;
            }
            yield return null;
        }

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = endAlpha;
            fadeImage.color = c;
        }
    }
}