using UnityEngine;

public class ShowOnTrigger : MonoBehaviour
{
    public GameObject targetObject;     // 숨길 오브젝트
    public AudioClip soundEffect;       // 재생할 소리
    public AudioSource audioSource;     // 소리를 재생할 오디오 소스

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 오브젝트 보이기
            if (targetObject != null)
            {
                targetObject.SetActive(true);
            }

            // 소리 재생
            if (audioSource != null && soundEffect != null)
            {
                audioSource.PlayOneShot(soundEffect);
            }
        }
    }
}
