using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // TMP 사용 시 TextMeshProUGUI로 변경

public class DoorToEnding : MonoBehaviour
{
    private bool playerInRange = false;

    [Header("UI")]
    public GameObject interactUI; // "문 열고 들어가기" 텍스트 오브젝트

    private void Start()
    {
        if (interactUI != null)
            interactUI.SetActive(false); // 시작 시 숨김
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactUI != null)
                interactUI.SetActive(true); // UI 표시
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactUI != null)
                interactUI.SetActive(false); // UI 숨김
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
