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
            {
                GameManager.Instance.MoveToFrontDoor();
            }
            else // doorType == DoorType.Front
            {
                // ✨ [수정] Front 문을 열었을 때 MoveToBackDoor()가 호출되도록 수정했습니다.
                GameManager.Instance.MoveToBackDoor();
            }
        }

        if (fadeImage != null)
            yield return StartCoroutine(Fade(1f, 0f));

        yield return new WaitForSeconds(triggerCooldown);
        isActivated = true;
        isFading = false;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (fadeImage == null) yield break;

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

        Color finalColor = fadeImage.color;
        finalColor.a = endAlpha;
        fadeImage.color = finalColor;

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