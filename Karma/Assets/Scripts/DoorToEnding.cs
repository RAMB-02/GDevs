using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // TMP ��� �� TextMeshProUGUI�� ����

public class DoorToEnding : MonoBehaviour
{
    private bool playerInRange = false;

    [Header("UI")]
    public GameObject interactUI; // "�� ���� ����" �ؽ�Ʈ ������Ʈ

    private void Start()
    {
        if (interactUI != null)
            interactUI.SetActive(false); // ���� �� ����
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactUI != null)
                interactUI.SetActive(true); // UI ǥ��
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactUI != null)
                interactUI.SetActive(false); // UI ����
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("EndingScene");
        }
    }
}
