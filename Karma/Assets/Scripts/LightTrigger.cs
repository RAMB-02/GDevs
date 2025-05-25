using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            LightManager.Instance.SetAnomalyLights(true);
            triggered = true; // 한 번만 작동하게 막기
        }
    }
}
