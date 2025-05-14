using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public enum DoorType { Front, Back }
    public DoorType doorType;

    private bool playerInRange = false;

    public bool triggerLightsOnEnter = false; // Ʈ���ſ� ���� �� ���� ���� ����

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (triggerLightsOnEnter)
            {
                LightManager.Instance.SetAnomalyLights(true);
            }
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
        if (playerInRange && Input.GetMouseButtonDown(0))
        {
            // �� �̵� ���� ���� ���� ����
            LightManager.Instance.SetAnomalyLights(false);

            if (doorType == DoorType.Back)
                GameManager.Instance.MoveToBackDoor();
            else
                GameManager.Instance.MoveToFrontDoor();
        }
    }
}
