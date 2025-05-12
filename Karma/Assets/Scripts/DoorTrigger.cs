using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public enum DoorType { Front, Back }
    public DoorType doorType;

    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetMouseButtonDown(0)) // ÁÂÅ¬¸¯
        {
            if (doorType == DoorType.Back)
                GameManager.Instance.MoveToBackDoor();
            else
                GameManager.Instance.MoveToFrontDoor();
        }
    }
}
