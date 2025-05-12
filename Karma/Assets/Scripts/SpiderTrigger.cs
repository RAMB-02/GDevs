using System.Collections;
using UnityEngine;

public class SpiderTrigger : MonoBehaviour
{
    public Monster monster;
    public float chaseDuration = 5f; // ← 60초 → 5초로 수정
    private bool triggered = false;

    private void OnEnable()
    {
        triggered = false; // 트리거 오브젝트 재활성화 시 초기화
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            Debug.Log("거미 추적 시작");
            monster.StartChasing();
            StartCoroutine(EndChaseAfterDelay());
        }
    }

    private IEnumerator EndChaseAfterDelay()
    {
        Debug.Log("추적 대기 시작");
        yield return new WaitForSeconds(chaseDuration);
        Debug.Log("추적 종료 → 거미 리셋");
        monster.ResetToInitialPosition();
        gameObject.SetActive(false); // 트리거 자체 비활성화
    }
}
