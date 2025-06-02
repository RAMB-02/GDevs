using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [Header("연결 오브젝트")]
    public GameObject mutant;                         // 비활성화된 보스 오브젝트
    public BossController bossController;             // BossController 스크립트
    public LightCycleManager lightCycleManager;       // LightCycleManager 스크립트
    public Transform player;                          // 플레이어 Transform (필수 연결!)

    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;

            // ✅ 보스 오브젝트 활성화
            if (mutant != null && !mutant.activeSelf)
                mutant.SetActive(true);

            // ✅ 보스 이동, 추적 시작
            if (bossController != null)
            {
                bossController.player = player;
                bossController.SetCanMove(true);
            }

            // ✅ 조명 사이클 시작
            if (lightCycleManager != null)
                lightCycleManager.enabled = true;

            Debug.Log("보스 트리거 작동됨!");
        }
    }
}