using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    public AudioClip soundClip; // 재생할 소리 클립
    private AudioSource audioSource; // AudioSource 컴포넌트

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // AudioSource 컴포넌트가 없으면 추가
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 소리 재생
            audioSource.PlayOneShot(soundClip);
        }
    }
}
