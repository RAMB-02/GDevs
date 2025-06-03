using UnityEngine;
using UnityEngine.UI;

public class ventScript : MonoBehaviour
{
    [Header("��� ������Ʈ��")]
    public GameObject[] toggleObjects;

    [Header("����")]
    public AudioSource clickSound;

    [Header("UI")]
    public GameObject interactionUI; // ��: "���콺 ��Ŭ������ ȯǳ�� ����" �ؽ�Ʈ

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
