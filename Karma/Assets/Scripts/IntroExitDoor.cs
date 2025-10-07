using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro; // TextMeshPro를 사용할 경우를 위해 추가

public class IntroExitDoor : MonoBehaviour
{
    [Header("연출 요소")]
    public AudioSource audioSource;     // 문 열리는 소리
    public Image fadeImage;             // 페이드 효과에 사용할 UI 이미지
    public float fadeDuration = 1.5f;   // 페이드 연출에 걸리는 시간

    [Header("UI 요소")]
    public GameObject interactUI;       // "상호작용 (E)" 와 같은 기본 안내 UI
    public GameObject warningUI;        // 경고 메시지를 표시할 UI 오브젝트 (패널 + 텍스트)
    public TextMeshProUGUI warningText; // 경고 메시지를 표시할 TextMeshPro 컴포넌트
    public float warningDisplayTime = 2.5f; // 경고 UI가 표시될 시간

    [Header("이동 설정")]
    public string nextSceneName = "MainGameScene"; // 이동할 씬의 이름

    private bool isPlayerInRange = false;
    private bool isInteracting = false; // 중복 상호작용을 방지하기 위한 플래그

    void Start()
    {
        if (interactUI != null) interactUI.SetActive(false);
        if (warningUI != null) warningUI.SetActive(false);

        if (fadeImage != null)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (interactUI != null && !isInteracting)
            {
                interactUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactUI != null)
            {
                interactUI.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (isPlayerInRange && !isInteracting && Input.GetKeyDown(KeyCode.E))
        {
            if (NecklessScript.hasStolenNecklace)
            {
                StartCoroutine(TransitionSequence());
            }
            else
            {
                StartCoroutine(ShowWarningSequence());
            }
        }
    }

    private IEnumerator TransitionSequence()
    {
        isInteracting = true;
        interactUI.SetActive(false);

        if (audioSource != null)
        {
            audioSource.Play();
        }

        yield return StartCoroutine(FadeEffect(0f, 1f));

        // ? 씬을 로드하기 직전에 신호를 true로 설정
        NecklessScript.playSoundOnNextSceneLoad = true;

        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator ShowWarningSequence()
    {
        isInteracting = true;
        interactUI.SetActive(false);

        if (audioSource != null)
        {
            audioSource.Play();
        }

        if (warningUI != null && warningText != null)
        {
            warningText.text = "목걸이를 훔치고 탈출해야 합니다.";
            warningUI.SetActive(true);
        }

        yield return new WaitForSeconds(warningDisplayTime);

        if (warningUI != null)
        {
            warningUI.SetActive(false);
        }

        if (isPlayerInRange)
        {
            interactUI.SetActive(true);
        }

        isInteracting = false;
    }

    private IEnumerator FadeEffect(float startAlpha, float endAlpha)
    {
        if (fadeImage == null) yield break;

        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }
}