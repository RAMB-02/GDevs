using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            LightManager.Instance.SetAnomalyLights(true);
            triggered = true; // �� ���� �۵��ϰ� ����
        }
    }
}
