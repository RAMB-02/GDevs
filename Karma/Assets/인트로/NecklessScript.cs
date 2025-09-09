using UnityEngine;
using UnityEngine.UI;

public class NecklessScript : MonoBehaviour
{
    [Header("대상 오브젝트들")]
    public GameObject[] toggleObjects;

    [Header("사운드")]
    public AudioSource clickSound;

    [Header("UI")]
    public GameObject interactionUI;

    private bool isPlayerInRange = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
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
