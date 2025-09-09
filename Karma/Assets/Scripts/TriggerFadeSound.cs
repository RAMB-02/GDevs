using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TriggerFadeSound : MonoBehaviour
{
    public AudioSource audioSource;  // 사운드 재생용 AudioSource
    public Image fadeImage;          // 화면 페이드용 UI 이미지
    public float fadeDuration = 1f;  // 페이드 인/아웃 시간

    private bool isPlayerInRange = false;
    private bool isFading = false;

    void Start()
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isFading)
        {
            StartCoroutine(FadeInOut());
        }
    }

    private IEnumerator FadeInOut()
    {
        isFading = true;

        // 사운드 재생
        if (audioSource != null)
        {
            audioSource.Play();
        }

        // 페이드 인 (검은색 점점 짙어짐)
        yield return StartCoroutine(Fade(0f, 1f));



        // 페이드 아웃 (검은색 점점 투명해짐)
        yield return StartCoroutine(Fade(1f, 0f));

        isFading = false;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;
            yield return null;
        }
        // 확실히 끝값으로 세팅
        Color finalColor = fadeImage.color;
        finalColor.a = endAlpha;
        fadeImage.color = finalColor;
    }

    // 플레이어가 트리거 안에 들어왔을 때
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    // 플레이어가 트리거 밖으로 나갔을 때
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
