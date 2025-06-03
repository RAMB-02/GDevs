using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour
{
    public enum DoorType { Front, Back }
    public DoorType doorType;

    private bool playerInRange = false;

    public bool triggerLightsOnEnter = false; // 트리거에 들어갔을 때 조명 조작 여부

    [Header("UI")]
    public GameObject interactUI; // "문 열고 들어가기" 텍스트 오브젝트

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (triggerLightsOnEnter)
            {
                LightManager.Instance.SetAnomalyLights(true);
            }

            if (interactUI != null)
            {
                interactUI.SetActive(true); // UI 표시
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactUI != null)
            {
                interactUI.SetActive(false); // UI 숨김
            }
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetMouseButtonDown(0))
        {
            // UI 숨기기
            if (interactUI != null)
            {
                interactUI.SetActive(false);
            }

            // stage가 7이고 조건에 맞는 문을 선택했는지 확인
            bool correctChoice =
                (doorType == DoorType.Back && GameManager.Instance.anomaly >= 1) ||
                (doorType == DoorType.Front && GameManager.Instance.anomaly == 0);

            if (GameManager.Instance.stage == 7 && correctChoice)
            {
                // 마지막 씬으로("8stage") 전환
                SceneManager.LoadScene("8stage");
                return;
            }

            // 일반적인 루프 처리
            if (doorType == DoorType.Back)
                GameManager.Instance.MoveToBackDoor();
            else
                GameManager.Instance.MoveToFrontDoor();
        }
    }
}
