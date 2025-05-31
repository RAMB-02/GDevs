using System.Collections;
using UnityEngine;

public class MummyTrigger : MonoBehaviour
{
    public Mummy mummy;
    //public float chaseDuration = 10f; // ← 60초 → 5초로 수정
    private bool triggered = false;

    public void OnEnable()
    {
        triggered = false; // 트리거 오브젝트 재활성화 시 초기화
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            Debug.Log("미라라 추적 시작");
            mummy.MummyChasing();
            //StartCoroutine(EndChaseAfterDelay());
        }
    }

    
}
