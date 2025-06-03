using UnityEngine;

public class HideOnTrigger : MonoBehaviour
{
    public GameObject targetObject;     // 숨길 오브젝트
    public AudioClip soundEffect;       // 재생할 소리
    public AudioSource audioSource;     // 소리를 재생할 오디오 소스

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;

            // 오브젝트 숨기기
            if (targetObject != null)
            {
                targetObject.SetActive(false);
            }

            // 소리 재생
            if (audioSource != null && soundEffect != null)
            {
                audioSource.PlayOneShot(soundEffect);
            }
        }
    }

    public void ResetTrigger()
    {
        hasTriggered = false; // 트리거 상태 초기화

        if (targetObject != null)
            targetObject.SetActive(true);  // 맵 초기화 시 오브젝트 다시 활성화
        else
            gameObject.SetActive(true);    // targetObject 없으면 자신을 활성화
    }
}
