using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            LightManager.Instance.SetAnomalyLights(true);
            triggered = true;
        }
    }

    public void ResetTrigger()
    {
        triggered = false;
        LightManager.Instance.SetAnomalyLights(false); // 조명 정상화
    }

    private void OnEnable()
    {
        ResetTrigger(); // 중요: 오브젝트가 다시 활성화될 때 초기화
    }
}