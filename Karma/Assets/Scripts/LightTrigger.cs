using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            // ✅ 로그 추가
            Debug.Log("LightTrigger: 플레이어가 진입했습니다. 조명을 어둡게 변경 요청.");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetAnomalyLightState(true);
            }
            triggered = true;
        }
    }

    public void ResetTrigger()
    {
        triggered = false;
        // ✅ 로그 추가
        Debug.Log("LightTrigger: 트리거 상태를 리셋했습니다. triggered = false");
    }
}