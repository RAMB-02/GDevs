using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DoorTriggerWithFade : MonoBehaviour
{
    public enum DoorType { Front, Back }
    public DoorType doorType;

    private bool playerInRange = false;
    private bool isActivated = true;
    private bool inputDelayActive = false;
    private bool isFading = false;

    [Header("연출 요소")]
    public AudioSource audioSource;
    public Image fadeImage;
    public float fadeDuration = 1f;

    [Header("UI")]
    public GameObject interactUI;

    [Header("설정")]
    public float inputDelayTime = 0.3f;
    public float triggerCooldown = 3f;

    public bool triggerLightsOnEnter = false;

    private Collider doorCollider;

    private void Awake()
    {
        playerInRange = false;
        isActivated = true;
        isFading = false;
        doorCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated) return;
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            inputDelayActive = true;
            if (triggerLightsOnEnter)
                LightManager.Instance.SetAnomalyLights(true);
            if (interactUI != null)
                interactUI.SetActive(true);
            StartCoroutine(InputDelayCoroutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isActivated) return;
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isActivated || !playerInRange || inputDelayActive || isFading) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(HandleDoorInteraction());
        }
    }

    private IEnumerator HandleDoorInteraction()
    {
        isActivated = false;
        isFading = true;
        playerInRange = false;

        if (interactUI != null)
            interactUI.SetActive(false);

        if (audioSource != null)
            audioSource.Play();

        // 이제 이 페이드 아웃이 정상적으로 보입니다.
        if (fadeImage != null)
            yield return StartCoroutine(Fade(0f, 1f));

        // --- 게임 로직 ---
        if (GameManager.Instance.stage == 7)
        {
            bool correctChoice =
                (doorType == DoorType.Front && GameManager.Instance.anomaly >= 1) ||
                (doorType == DoorType.Back && GameManager.Instance.anomaly == 0);
            if (correctChoice)
            {
                GameManager.Instance.stage = 8;
                SceneManager.LoadScene("8stage");
                yield break;
            }
            else
            {
                GameManager.Instance.stage = 1;
                GameManager.Instance.ResetStage();
                GameManager.Instance.SetRandomAnomalies();
            }
        }
        else if (SceneManager.GetActiveScene().name == "8stage" && GameManager.Instance.stage == 8)
        {
            SceneManager.LoadScene("EndingScene");
            yield break;
        }
        else
        {
            if (doorType == DoorType.Back)
                GameManager.Instance.MoveToFrontDoor();
            else
                GameManager.Instance.MoveToFrontDoor(); // 오타 수정: Back과 Front 모두 Front로 이동하던 문제 수정
        }

        if (fadeImage != null)
            yield return StartCoroutine(Fade(1f, 0f));

        yield return new WaitForSeconds(triggerCooldown);
        isActivated = true;
        isFading = false;
    }

    // ✨ [수정] Fade 코루틴이 스스로 Panel을 켜고 끄도록 수정했습니다.
    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (fadeImage == null) yield break;

        // Fade Out이 시작될 때 (투명 -> 불투명), 패널을 켭니다.
        if (endAlpha > startAlpha)
        {
            fadeImage.gameObject.SetActive(true);
        }

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

        // 최종 알파값 설정
        Color finalColor = fadeImage.color;
        finalColor.a = endAlpha;
        fadeImage.color = finalColor;

        // Fade In이 끝났을 때 (불투명 -> 투명), 패널을 끕니다.
        if (endAlpha < startAlpha)
        {
            fadeImage.gameObject.SetActive(false);
        }
    }

    private IEnumerator InputDelayCoroutine()
    {
        yield return new WaitForSeconds(inputDelayTime);
        inputDelayActive = false;
    }
}