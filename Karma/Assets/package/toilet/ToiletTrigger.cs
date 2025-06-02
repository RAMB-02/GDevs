using UnityEngine;
using System.Collections;

public class ToiletTrigger : MonoBehaviour
{
    private bool triggered = false;
    public Toilet toilet;
    public void OnEnable()
    {
        triggered = false; // 트리거 오브젝트 재활성화 시 초기화
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;

            if (toilet != null)
            {
                StartCoroutine(PlaySoundsWithDelay());
            }
        }
    }
    private IEnumerator PlaySoundsWithDelay()
    {
        toilet.PlaySound(); // 첫 번째 소리
        yield return new WaitForSeconds(2f); // 3초 대기
        toilet.PlaySound2(); // 두 번째 소리
    }

}
