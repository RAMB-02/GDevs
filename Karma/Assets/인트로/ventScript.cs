using UnityEngine;
using UnityEngine.UI;

public class ventScript : MonoBehaviour
{
    [Header("대상 오브젝트들")]
    public GameObject[] toggleObjects;

    [Header("사운드")]
    public AudioSource clickSound;

    [Header("UI")]
    public GameObject interactionUI; // 예: "마우스 좌클릭으로 환풍구 열기" 텍스트

    private bool isPlayerInRange = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetMouseButtonDown(0))
        {
            foreach (GameObject obj in toggleObjects)
            {
                if (obj != null)
                    obj.SetActive(!obj.activeSelf);
            }

            if (clickSound != null)
                clickSound.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (interactionUI != null)
                interactionUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactionUI != null)
                interactionUI.SetActive(false);
        }
    }
}
