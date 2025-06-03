using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour
{
    public enum DoorType { Front, Back }
    public DoorType doorType;

    private bool playerInRange = false;

    public bool triggerLightsOnEnter = false; // Ʈ���ſ� ���� �� ���� ���� ����

    [Header("UI")]
    public GameObject interactUI; // "�� ���� ����" �ؽ�Ʈ ������Ʈ

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
                interactUI.SetActive(true); // UI ǥ��
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
                interactUI.SetActive(false); // UI ����
            }
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetMouseButtonDown(0))
        {
            // UI �����
            if (interactUI != null)
            {
                interactUI.SetActive(false);
            }

            // stage�� 7�̰� ���ǿ� �´� ���� �����ߴ��� Ȯ��
            bool correctChoice =
                (doorType == DoorType.Back && GameManager.Instance.anomaly >= 1) ||
                (doorType == DoorType.Front && GameManager.Instance.anomaly == 0);

            if (GameManager.Instance.stage == 7 && correctChoice)
            {
                // ������ ������("8stage") ��ȯ
                SceneManager.LoadScene("8stage");
                return;
            }

            // �Ϲ����� ���� ó��
            if (doorType == DoorType.Back)
                GameManager.Instance.MoveToBackDoor();
            else
                GameManager.Instance.MoveToFrontDoor();
        }
    }
}
