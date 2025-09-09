using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DoorTrigger : MonoBehaviour
{
    public enum DoorType { Front, Back }
    public DoorType doorType;

    private bool playerInRange = false;
    private bool isActivated = true;
    private bool inputDelayActive = false;

    public bool triggerLightsOnEnter = false;

    [Header("UI")]
    public GameObject interactUI;

    [Header("설정")]
    public float disableDuration = 3f;       // E키 재사용 금지 시간
    public float inputDelayTime = 0.3f;       // 트리거 진입 후 E 입력 딜레이 시간

    private Collider doorCollider;

    private void Awake()
    {
        doorCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated) return;

        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            inputDelayActive = true; // 딜레이 시작

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
        if (!isActivated || !playerInRange || inputDelayActive) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            isActivated = false;

            if (interactUI != null)
                interactUI.SetActive(false);

            if (doorCollider != null)
                doorCollider.enabled = false;

            bool correctChoice =
                (doorType == DoorType.Back && GameManager.Instance.anomaly >= 1) ||
                (doorType == DoorType.Front && GameManager.Instance.anomaly == 0);

            if (GameManager.Instance.stage == 7 && correctChoice)
            {
                SceneManager.LoadScene("8stage");
                return;
            }

            if (doorType == DoorType.Back)
                GameManager.Instance.MoveToBackDoor();
            else
                GameManager.Instance.MoveToFrontDoor();

            LightManager.Instance.SetAnomalyLights(false);

            StartCoroutine(ReenableAfterDelay());
        }
    }

    private IEnumerator InputDelayCoroutine()
    {
        yield return new WaitForSeconds(inputDelayTime);
        inputDelayActive = false;
    }

    private IEnumerator ReenableAfterDelay()
    {
        yield return new WaitForSeconds(disableDuration);

        if (doorCollider != null)
            doorCollider.enabled = true;

        isActivated = true;
    }
}
