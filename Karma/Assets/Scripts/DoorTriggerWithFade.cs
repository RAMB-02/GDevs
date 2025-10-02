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

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
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

        // --- ✅ 7스테이지에서 anomaly 조건 검사 (기능 교체됨) ---
        if (GameManager.Instance.stage == 7)
        {
            // [수정됨] 이상 현상이 있을 때 Front가 정답, 없을 때 Back이 정답이 되도록 변경
            bool correctChoice =
                (doorType == DoorType.Front && GameManager.Instance.anomaly >= 1) ||
                (doorType == DoorType.Back && GameManager.Instance.anomaly == 0);

            if (correctChoice)
            {
                GameManager.Instance.stage = 8;   // 스테이지 갱신
                SceneManager.LoadScene("8stage"); // 8스테이지 진입
                yield break;
            }
            else
            {
                // 틀린 문 선택 → stage=1 리셋
                GameManager.Instance.stage = 1;
                GameManager.Instance.ResetStage();
                GameManager.Instance.SetRandomAnomalies();
            }
        }
        else if (SceneManager.GetActiveScene().name == "8stage" && GameManager.Instance.stage == 8)
        {
            // ✅ 8스테이지에서 나갈 때 → 엔딩
            SceneManager.LoadScene("EndingScene");
            yield break;
        }
        else
        {
            // --- 일반 문 이동 (기능 교체됨) ---
            // [수정됨] Back Door는 Front로, Front Door는 Back으로 이동하도록 변경
            if (doorType == DoorType.Back)
                GameManager.Instance.MoveToFrontDoor();
            else // doorType == DoorType.Front
                GameManager.Instance.MoveToBackDoor();
        }

        if (fadeImage != null)
            yield return StartCoroutine(Fade(1f, 0f));

        yield return new WaitForSeconds(triggerCooldown);
        isActivated = true;
        isFading = false;
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

    private IEnumerator InputDelayCoroutine()
    {
        yield return new WaitForSeconds(inputDelayTime);
        inputDelayActive = false;
    }
}